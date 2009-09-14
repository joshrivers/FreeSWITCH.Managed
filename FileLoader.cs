using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FreeSWITCH.Managed
{
    public class FileLoader
    {
        public static IDirectoryController directories;
        public static ILogger logger;
        static DefaultAppDomainSetupFactory appDomainSetupFactory;
        static readonly object loaderLock = new object();

        static readonly PluginInfoList pluginInfos = new PluginInfoList();
        public static SynchronizedDictionary<string, AppPluginExecutor> appExecs = new SynchronizedDictionary<string, AppPluginExecutor>(StringComparer.OrdinalIgnoreCase);
        public static SynchronizedDictionary<string, ApiPluginExecutor> apiExecs = new SynchronizedDictionary<string, ApiPluginExecutor>(StringComparer.OrdinalIgnoreCase);

        public static void init()
        {
            appDomainSetupFactory = new DefaultAppDomainSetupFactory(directories);
            AppDomain.CurrentDomain.AssemblyResolve += (_, rargs) =>
            {
                logger.Info(string.Format("Resolving assembly '{0}'.", rargs.Name));
                if (rargs.Name == Assembly.GetExecutingAssembly().FullName) return Assembly.GetExecutingAssembly();
                var parts = rargs.Name.Split(',');
                var path = Path.Combine(directories.PluginDirectoryPath, parts[0] + ".dll");
                logger.Info(string.Format("Resolving to: '" + path + "'."));
                return File.Exists(path) ? Assembly.LoadFile(path) : null;
            };
        }


        public static void loadFile(string fileName)
        {
            try
            {
                if ((Path.GetExtension(fileName).ToLowerInvariant() == ".config"))
                {
                    fileName = Path.ChangeExtension(fileName, null);
                }
                if (pluginInfos.NoReloadEnabled(fileName)) { return; }
                // Attempts to load the file. On failure, it will call unload.
                // Loading part does not take out a lock. 
                // Lock is only done after loading is finished and dictionaries need updating.

                // We might get a load for a file that's no longer there. Just unload the old one.
                if (!File.Exists(fileName))
                {
                    unloadFile(fileName);
                    return;
                }

                Type pmType;
                switch (Path.GetExtension(fileName).ToLowerInvariant())
                {
                    case ".dll":
                        pmType = typeof(AsmPluginManager);
                        break;
                    case ".exe": // TODO these need to come from config
                    case ".fsx":
                    case ".vbx":
                    case ".csx":
                    case ".jsx":
                        pmType = typeof(ScriptPluginManager);
                        break;
                    default:
                        pmType = null;
                        break;
                }
                if (pmType == null) return;

                // App domain setup
                var setup = appDomainSetupFactory.CreateSetup(fileName);

                // Create domain and load PM inside
                var domain = AppDomain.CreateDomain(setup.ApplicationName, null, setup);

                PluginManager pm;
                try
                {
                    pm = (PluginManager)domain.CreateInstanceAndUnwrap(pmType.Assembly.FullName, pmType.FullName, null);
                    if (!pm.Load(fileName))
                    {
                        AppDomain.Unload(domain);
                        unloadFile(fileName);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // On an exception, we will unload the current file so an old copy doesnt stay active
                    logger.Alert(string.Format("Exception loading {0}: {1}", fileName, ex.ToString()));
                    AppDomain.Unload(domain);
                    unloadFile(fileName);
                    return;
                }

                // Update dictionaries atomically
                lock (loaderLock)
                {
                    unloadFile(fileName);

                    var pi = new PluginInfo { FileName = fileName, Domain = domain, Manager = pm };
                    pluginInfos.Add(pi);
                    pm.AppExecutors.ForEach(x => x.Aliases.ForEach(y => appExecs[y] = x));
                    pm.ApiExecutors.ForEach(x => x.Aliases.ForEach(y => apiExecs[y] = x));
                    Action<PluginExecutor, string> printLoaded = (pe, type) =>
                    {
                        var aliases = pe.Aliases.Aggregate((acc, x) => acc += ", " + x);
                        logger.Notice(string.Format("Loaded {3} {0}, aliases '{1}', into domain {2}.", pe.Name, aliases, pi.Domain.FriendlyName, type));
                    };
                    pm.AppExecutors.ForEach(x => printLoaded(x, "App"));
                    pm.ApiExecutors.ForEach(x => printLoaded(x, "Api"));
                    logger.Info(string.Format("Finished loading {0} into domain {1}.", pi.FileName, pi.Domain.FriendlyName));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Exception loading change from {0}: {1}", fileName, ex.ToString()));
            }
        }

        static void unloadFile(string fileName)
        {
            List<PluginInfo> pisToRemove;
            lock (loaderLock)
            {
                pisToRemove = pluginInfos[fileName];
                if (pisToRemove.Count == 0) return; // Done

                var apisToRemove = pisToRemove.SelectMany(x => x.Manager.ApiExecutors).ToList();
                var appsToRemove = pisToRemove.SelectMany(x => x.Manager.AppExecutors).ToList();
                pluginInfos.RemoveAll(pisToRemove);
                appsToRemove.ForEach((plugin) => appExecs.RemoveValue(plugin));
                apisToRemove.ForEach((plugin) => apiExecs.RemoveValue(plugin));

                Action<PluginExecutor, string> printRemoved = (pe, type) =>
                {
                    logger.Notice(string.Format("Unloaded {0} {1} (file {2}).", type, pe.Name, fileName));
                };
                apisToRemove.ForEach(x => printRemoved(x, "API"));
                appsToRemove.ForEach(x => printRemoved(x, "App"));
            }

            pisToRemove.ForEach(pi => pi.Remove());
        }
    }
}

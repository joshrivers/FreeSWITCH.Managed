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

        static readonly object loaderLock = new object();

        static readonly PluginInfoList pluginInfos = new PluginInfoList();
        public static SynchronizedDictionary<string, AppPluginExecutor> appExecs = new SynchronizedDictionary<string, AppPluginExecutor>(StringComparer.OrdinalIgnoreCase);
        public static SynchronizedDictionary<string, ApiPluginExecutor> apiExecs = new SynchronizedDictionary<string, ApiPluginExecutor>(StringComparer.OrdinalIgnoreCase);



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
                    removeReferencesToModuleAndPlugin(fileName);
                    return;
                }
                var module = new Module(fileName,FileLoader.logger,FileLoader.directories);
                module.Initialize();

                // Update dictionaries atomically
                lock (loaderLock)
                {
                    removeReferencesToModuleAndPlugin(fileName);

                    pluginInfos.Add(module);
                    module.Proxy.AppExecutors.ForEach(x => x.Aliases.ForEach(y => appExecs[y] = x));
                    module.Proxy.ApiExecutors.ForEach(x => x.Aliases.ForEach(y => apiExecs[y] = x));
                    Action<PluginExecutor, string> printLoaded = (pe, type) =>
                    {
                        var aliases = pe.Aliases.Aggregate((acc, x) => acc += ", " + x);
                        logger.Notice(string.Format("Loaded {3} {0}, aliases '{1}', into domain {2}.", pe.Name, aliases, module.Domain.FriendlyName, type));
                    };
                    module.Proxy.AppExecutors.ForEach(x => printLoaded(x, "App"));
                    module.Proxy.ApiExecutors.ForEach(x => printLoaded(x, "Api"));
                    logger.Info(string.Format("Finished loading {0} into domain {1}.", module.FileName, module.Domain.FriendlyName));
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Exception loading change from {0}: {1}", fileName, ex.ToString()));
            }
        }

        // This method is completely unneccessary except to manage all of the plugin collection overhead.
        public static void removeReferencesToModuleAndPlugin(string fileName)
        {
            List<Module> pisToRemove;
            lock (loaderLock)
            {
                pisToRemove = pluginInfos[fileName];
                if (pisToRemove.Count == 0) return; // Done

                var apisToRemove = pisToRemove.SelectMany(x => x.Proxy.ApiExecutors).ToList();
                var appsToRemove = pisToRemove.SelectMany(x => x.Proxy.AppExecutors).ToList();
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

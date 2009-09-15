using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FreeSWITCH.Managed
{
    public class Module
    {
        public string FileName { get; private set; }
        public AppDomain Domain { get; private set; }
        public ModuleProxy Proxy { get; private set; }
        private ILogger logger;
        private IDirectoryController directories;

        public Module(string filename, ILogger logger, IDirectoryController directories)
        {
            this.FileName = filename;
            this.logger = logger;
            this.directories = directories;
        }

        public void Initialize()
        {
            Type proxyType;
            switch (Path.GetExtension(FileName).ToLowerInvariant())
            {
                case ".dll":
                    proxyType = typeof(AsmModuleProxy);
                    break;
                case ".exe": // TODO these need to come from config
                case ".fsx":
                case ".vbx":
                case ".csx":
                case ".jsx":
                    proxyType = typeof(ScriptModuleProxy);
                    break;
                default:
                    proxyType = null;
                    break;
            }
            if (proxyType == null) return;

            DefaultAppDomainSetupFactory appDomainSetupFactory = new DefaultAppDomainSetupFactory(directories);
            // App domain setup
            var setup = appDomainSetupFactory.CreateSetup(FileName);

            // Create domain and load PM inside
            this.Domain = AppDomain.CreateDomain(setup.ApplicationName, null, setup);
            
            try
            {
                Proxy = (ModuleProxy)Domain.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName, null);
                if (!Proxy.Load(FileName))
                {
                    AppDomain.Unload(Domain);
                    throw new Exception("Unable to Initialize Module");
                }
            }
            catch (Exception ex)
            {
                // On an exception, we will unload the current file so an old copy doesnt stay active
                logger.Alert(string.Format("Exception loading {0}: {1}", FileName, ex.ToString()));
                AppDomain.Unload(Domain);
                throw;
            }
        }

        public void Remove()
        {
            var t = new System.Threading.Thread(() =>
            {
                var friendlyName = this.Domain.FriendlyName;
                logger.Info(string.Format("Starting to unload {0}, domain {1}.", this.FileName, friendlyName));
                try
                {
                    var d = this.Domain;
                    this.Proxy.BlockUntilUnloadIsSafe();
                    this.Proxy = null;
                    this.Domain = null;
                    AppDomain.Unload(d);
                    logger.Info(string.Format("Unloaded {0}, domain {1}.", this.FileName, friendlyName));
                }
                catch (Exception ex)
                {
                    logger.Alert(string.Format("Could not unload {0}, domain {1}: {2}", this.FileName, friendlyName, ex.ToString()));
                }
            }) { Priority = System.Threading.ThreadPriority.BelowNormal, IsBackground = true };
            t.Start();
        }

        public bool NoReload
        {
            get
            {
                return this.Proxy.ApiExecutors.Any(x => (x.PluginOptions & PluginOptions.NoAutoReload) == PluginOptions.NoAutoReload) ||
                                   this.Proxy.AppExecutors.Any(x => (x.PluginOptions & PluginOptions.NoAutoReload) == PluginOptions.NoAutoReload);
            }
        }

    }
}

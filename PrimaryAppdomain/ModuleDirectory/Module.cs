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
        public string FilePath { get; private set; }
        public AppDomain Domain { get; private set; }
        public ModuleProxy Proxy { get; private set; }
        private LogDirector logger;
        private IDirectoryController directories;

        public Module(LogDirector logger, IDirectoryController directories)
        {
            this.logger = logger;
            this.directories = directories;
        }

        public void Initialize(string filePath)
        {
            this.FilePath = filePath;
            Type proxyType = GetProxyType();
            if (proxyType == null) return;

            DefaultAppDomainSetupFactory appDomainSetupFactory = new DefaultAppDomainSetupFactory(directories);
            var setup = appDomainSetupFactory.CreateSetup(FilePath);
            this.Domain = AppDomain.CreateDomain(setup.ApplicationName, null, setup);

            try
            {
                this.Proxy = (ModuleProxy)Domain.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName, null);
                this.logger.Add(this.Proxy.LogDirector);
                this.Proxy.Logger.Add(this.logger);
                this.Proxy.MasterAssemblyPath = this.FilePath;
                bool success = this.Proxy.AssemblyLoader.Load(this.FilePath);
                if (!success)
                {
                    throw new Exception("Unable to Initialize Module");
                }
            }
            catch (Exception ex)
            {
                logger.Alert(string.Format("Exception loading {0}: {1}", FilePath, ex.ToString()));
                AppDomain.Unload(this.Domain);
                throw;
            }
        }

        private Type GetProxyType()
        {
            Type proxyType;
            switch (FilePath.GetLoweredFileExtension())
            {
                case ".dll":
                case ".exe": // TODO these need to come from config
                case ".fsx":
                case ".vbx":
                case ".csx":
                case ".jsx":
                    proxyType = typeof(ModuleProxy);
                    break;
                default:
                    proxyType = null;
                    break;
            }
            return proxyType;
        }
        public void Remove()
        {
            var t = new System.Threading.Thread(() =>
            {
                var friendlyName = this.Domain.FriendlyName;
                logger.Info(string.Format("Starting to unload {0}, domain {1}.", this.FilePath, friendlyName));
                try
                {
                    var d = this.Domain;
                    this.Proxy.PluginHandlerOrchestrator.Unload();
                    this.Proxy = null;
                    this.Domain = null;
                    AppDomain.Unload(d);
                    logger.Info(string.Format("Unloaded {0}, domain {1}.", this.FilePath, friendlyName));
                }
                catch (Exception ex)
                {
                    logger.Alert(string.Format("Could not unload {0}, domain {1}: {2}", this.FilePath, friendlyName, ex.ToString()));
                }
            }) { Priority = System.Threading.ThreadPriority.BelowNormal, IsBackground = true };
            t.Start();
        }
    }
}

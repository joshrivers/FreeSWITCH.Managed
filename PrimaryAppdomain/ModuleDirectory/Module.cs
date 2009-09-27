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
        public IModuleProxy Proxy { get; private set; }
        private IAppDomainFactory appDomainFactory;
        private ILoggerContainer primaryLoggerContainer;
        private ModuleProxyTypeDictionary proxyTypes;
        private IDirectoryController directories;

        public Module(ILoggerContainer primaryLoggerContainer,
            IDirectoryController directories,
            IAppDomainFactory appDomainFactory,
            ModuleProxyTypeDictionary proxyTypes)
        {
            this.proxyTypes = proxyTypes;
            this.appDomainFactory = appDomainFactory;
            this.primaryLoggerContainer = primaryLoggerContainer;
            this.directories = directories;
        }

        public void Initialize(string filePath)
        {
            this.FilePath = filePath;
            if (!proxyTypes.ContainsKey(filePath.GetLoweredFileExtension())) { return; }
            Type proxyType = proxyTypes[filePath.GetLoweredFileExtension()];

            DefaultAppDomainFactory appDomainFactory = new DefaultAppDomainFactory(directories);
            this.Domain = appDomainFactory.CreateAppDomain(FilePath);

            try
            {
                this.Proxy = (IModuleProxy)Domain.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName, null);
                this.primaryLoggerContainer.Add(this.Proxy.LogDirector);
                this.Proxy.Logger.Add(this.primaryLoggerContainer);
                this.Proxy.MasterAssemblyPath = this.FilePath;
                bool success = this.Proxy.AssemblyLoader.Load(this.FilePath);
                if (!success)
                {
                    throw new Exception("Unable to Initialize Module");
                }
            }
            catch (Exception ex)
            {
                primaryLoggerContainer.Alert(string.Format("Exception loading {0}: {1}", FilePath, ex.ToString()));
                AppDomain.Unload(this.Domain);
                throw;
            }
        }

        public void Remove()
        {
            this.primaryLoggerContainer.Remove(this.Proxy.LogDirector);
            var t = new System.Threading.Thread(() =>
            {
                var friendlyName = this.Domain.FriendlyName;
                primaryLoggerContainer.Info(string.Format("Starting to unload {0}, domain {1}.", this.FilePath, friendlyName));
                try
                {
                    var d = this.Domain;
                    this.Proxy.PluginHandlerOrchestrator.Unload();
                    this.Proxy = null;
                    this.Domain = null;
                    AppDomain.Unload(d);
                    primaryLoggerContainer.Info(string.Format("Unloaded {0}, domain {1}.", this.FilePath, friendlyName));
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    primaryLoggerContainer.Alert(string.Format("Could not unload {0}, domain {1}: {2}", this.FilePath, friendlyName, ex.ToString()));
                }
            }) { Priority = System.Threading.ThreadPriority.BelowNormal, IsBackground = true };
            t.Start();
        }
    }
}

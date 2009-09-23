using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class DefaultAppDomainFactory : IAppDomainFactory
    {
        private IDirectoryController directories;
        private int appDomainCount = 0;

        public DefaultAppDomainFactory(IDirectoryController directories)
        {
            this.directories = directories;
        }

        public AppDomain CreateAppDomain(string filePath)
        {
            var setup = this.CreateSetup(filePath);
            return AppDomain.CreateDomain(setup.ApplicationName, null, setup);
        }

        public AppDomainSetup CreateSetup(string filePath)
        {
            System.Threading.Interlocked.Increment(ref appDomainCount);
            var setup = new AppDomainSetup();
            setup.ApplicationBase = Native.freeswitch.SWITCH_GLOBAL_dirs.mod_dir;
            setup.ShadowCopyDirectories = String.Format("{0};", directories.ModuleDirectoryPath);
            setup.LoaderOptimization = LoaderOptimization.MultiDomainHost; // TODO: would MultiDomain work better since FreeSWITCH.Managed isn't gac'd?
            setup.CachePath = directories.ShadowDirectoryPath;
            setup.ShadowCopyFiles = "true";
            setup.PrivateBinPath = "managed";
            setup.ApplicationName = Path.GetFileName(filePath) + ";" + appDomainCount;
            if (File.Exists(String.Format("{0}.config", filePath)))
            {
                setup.ConfigurationFile = String.Format("{0}.config", filePath);
            }
            return setup;
        }
    }
}

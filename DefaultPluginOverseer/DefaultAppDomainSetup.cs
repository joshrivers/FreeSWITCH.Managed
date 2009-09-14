using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class DefaultAppDomainSetupFactory
    {
        private IDirectoryController directories;
        private int appDomainCount = 0;

        public DefaultAppDomainSetupFactory(IDirectoryController directories)
        {
            this.directories = directories;
        }

        public AppDomainSetup CreateSetup(string filePath)
        {
            System.Threading.Interlocked.Increment(ref appDomainCount);
            var setup = new AppDomainSetup();
            setup.ApplicationBase = Native.freeswitch.SWITCH_GLOBAL_dirs.mod_dir;
            setup.ShadowCopyDirectories = directories.PluginDirectoryPath + ";";
            setup.LoaderOptimization = LoaderOptimization.MultiDomainHost; // TODO: would MultiDomain work better since FreeSWITCH.Managed isn't gac'd?
            setup.CachePath = directories.ShadowDirectoryPath;
            setup.ShadowCopyFiles = "true";
            setup.PrivateBinPath = "managed";
            setup.ApplicationName = Path.GetFileName(filePath) + "_" + appDomainCount;
            if (File.Exists(filePath + ".config"))
            {
                setup.ConfigurationFile = filePath + ".config";
            }
            return setup;
        }
    }
}

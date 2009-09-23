using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FreeSWITCH.Managed
{
    public class DefaultModuleDirectorySupervisor
    {
        public IDirectoryController directories { get; private set; }
        public ILogger logger { get; private set; }
        public ModuleFileLoadQueue filesToLoad { get; private set; }
        public ModuleFileLoadTimer loadTimer { get; private set; }

        public DefaultModuleDirectorySupervisor(IDirectoryController directories, ILogger logger, IModuleLoader moduleLoader)
        {
            this.directories = directories;
            this.logger = logger;
            this.filesToLoad = new ModuleFileLoadQueue(directories.PluginDirectoryPath);
            this.loadTimer = new ModuleFileLoadTimer(filesToLoad,moduleLoader);
        }
        public bool Initialize()
        {
            logger.Debug(string.Format("FreeSWITCH.Managed loader is starting with directory '{0}'.", directories.PluginDirectoryPath));
            if (!Directory.Exists(directories.PluginDirectoryPath))
            {
                logger.Error(string.Format("Managed directory not found: {0}", directories.PluginDirectoryPath));
                return false;
            }
            filesToLoad.Initialize();
            loadTimer.Start();
            return true;
        }
    }
}

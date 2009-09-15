using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FreeSWITCH.Managed
{
    public class DefaultPluginDirectoryWatcher
    {
        public IDirectoryController directories { get; private set; }
        public ILogger logger { get; private set; }
        public PluginFileLoadQueue filesToLoad { get; private set; }
        public PluginFileLoadTimer loadTimer { get; private set; }

        public DefaultPluginDirectoryWatcher(IDirectoryController directories, ILogger logger, IModuleLoader moduleLoader)
        {
            this.directories = directories;
            this.logger = logger;
            this.filesToLoad = new PluginFileLoadQueue(directories.PluginDirectoryPath);
            this.loadTimer = new PluginFileLoadTimer(filesToLoad,moduleLoader);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;

namespace FreeSWITCH.Managed
{
    public class PluginFileLoadQueue
    {
        private ManagedDirectoryWatcher watcher;
        public SynchronizedList<string> Files { get; private set; }

        public PluginFileLoadQueue(string pluginDirectoryPath)
        {
            this.watcher = new ManagedDirectoryWatcher(pluginDirectoryPath);
            this.watcher.FileChanged = this.ChangedFileFound;

            this.Files = new SynchronizedList<string>();

            var allFiles = Directory.GetFiles(pluginDirectoryPath);
            foreach (var file in allFiles)
            {
                this.Files.Add(file);
            }
        }

        private void ChangedFileFound(string fullPath)
        {
            this.Files.Add(fullPath);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;

namespace FreeSWITCH.Managed
{
    public class ModuleFileLoadQueue
    {
        private ManagedDirectoryWatcher watcher;
        private string pluginDirectoryPath;
        public SynchronizedList<string> Files { get; private set; }

        public ModuleFileLoadQueue(string pluginDirectoryPath)
        {
            this.pluginDirectoryPath = pluginDirectoryPath;
            this.Files = new SynchronizedList<string>();
        }

        public void Initialize()
        {
            this.watcher = new ManagedDirectoryWatcher(pluginDirectoryPath);
            this.watcher.FileChanged = this.ChangedFileFound;
            var allFiles = Directory.GetFiles(this.pluginDirectoryPath);
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

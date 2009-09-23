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
        private IDirectoryController directories;
        private ManagedDirectoryWatcher watcher;
        public SynchronizedList<string> Files { get; private set; }

        public ModuleFileLoadQueue(IDirectoryController directories)
        {
            this.directories = directories;
            this.Files = new SynchronizedList<string>();
        }

        public void Initialize()
        {
            this.watcher = new ManagedDirectoryWatcher(directories.ModuleDirectoryPath);
            this.watcher.FileChanged = this.ChangedFileFound;
            var allFiles = Directory.GetFiles(directories.ModuleDirectoryPath);
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

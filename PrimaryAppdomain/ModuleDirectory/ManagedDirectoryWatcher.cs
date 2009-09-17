using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class ManagedDirectoryWatcher : FileSystemWatcher
    {
        public Action<string> FileChanged { get; set; }

        public ManagedDirectoryWatcher(string path) : base(path)
        {
            this.IncludeSubdirectories = false;
            this.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            this.EnableRaisingEvents = true;
            this.Changed += watcher_Changed;
            this.Created += watcher_Changed;
            this.Deleted += watcher_Changed;
            this.Renamed += watcher_Changed; 
        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (this.FileChanged != null)
            {
                this.FileChanged.Invoke(e.FullPath);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace FreeSWITCH.Managed
{
    public class PluginFileLoadTimer
    {
        private Timer loadTick;
        private PluginFileLoadQueue filesToLoad;

        public PluginFileLoadTimer(PluginFileLoadQueue filesToLoad)
        {
            this.filesToLoad = filesToLoad;
            loadTick = new System.Threading.Timer(this.OnTick);
        }

        public void Start()
        {
            loadTick.Change(1000, 1000);
        }

        public void Stop()
        {
            loadTick.Change(System.Threading.Timeout.Infinite,
                System.Threading.Timeout.Infinite);
        }

        public void Load()
        {
            var items = filesToLoad.Files.Items;
            filesToLoad.Files.Clear();
            foreach (var file in items)
            {
                //Todo: eliminate this static reference.
                FileLoader.loadFile(file);
            }
        }

        private void OnTick(object state)
        {
            this.Load();
        }
    }
}

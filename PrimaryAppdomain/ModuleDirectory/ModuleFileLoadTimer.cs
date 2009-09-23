using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace FreeSWITCH.Managed
{
    public class ModuleFileLoadTimer
    {
        private Timer loadTick;
        private ModuleFileLoadQueue modulesToLoad;
        private IModuleLoader moduleLoader;

        public ModuleFileLoadTimer(ModuleFileLoadQueue modulesToLoad, IModuleLoader moduleLoader)
        {
            this.modulesToLoad = modulesToLoad;
            this.moduleLoader = moduleLoader;
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
            var items = modulesToLoad.Files.Items;
            modulesToLoad.Files.Clear();
            foreach (var file in items)
            {
                //Todo: eliminate this static reference.
                moduleLoader.LoadModule(file);
            }
        }

        private void OnTick(object state)
        {
            this.Load();
        }
    }
}

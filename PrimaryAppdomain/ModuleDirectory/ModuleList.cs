using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class ModuleList : SynchronizedList<Module>, IModuleList
    {
        public List<Module> this[string fileName]
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.innerCollection.Where(p => string.Equals(fileName, p.FilePath, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
        }

        public bool NoReloadEnabled(string fileName)
        {
            lock (this.syncRoot)
            {
                foreach (var module in this.innerCollection.Where(p => string.Equals(fileName, p.FilePath, StringComparison.OrdinalIgnoreCase)))
                {
                    if (module.Proxy.PluginHandlerOrchestrator.NoReload)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public new void Add(Module item)
        {
            lock (this.syncRoot)
            {
                this.innerCollection.Add(item);
            }
        }

        public new bool Remove(Module item)
        {
            lock (this.syncRoot)
            {
                return this.innerCollection.Remove(item);
            }
        }

        public new void Clear()
        {
            lock (this.syncRoot)
            {
                this.innerCollection.Clear();
            }
        }

        public void RemoveAll(List<Module> items)
        {
            lock (this.syncRoot)
            {
                this.innerCollection.RemoveAll(info => items.Contains(info));
            }
        }
    }
}

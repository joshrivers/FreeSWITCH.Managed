using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class PluginInfoList : SynchronizedList<Module>
    {
        public List<Module> this[string fileName]
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.innerCollection.Where(p => string.Equals(fileName, p.FileName, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
        }

        public bool NoReloadEnabled(string fileName)
        {
            lock (this.syncRoot)
            {
                foreach (var info in this.innerCollection.Where(p => string.Equals(fileName, p.FileName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (info.NoReload)
                    {
                        return true;
                    }
                }
                return false;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class ModuleList : SynchronizedList<Module>
    {
        public SynchronizedDictionary<string, AppPluginExecutor> appExecs = new SynchronizedDictionary<string, AppPluginExecutor>(StringComparer.OrdinalIgnoreCase);
        public SynchronizedDictionary<string, ApiPluginExecutor> apiExecs = new SynchronizedDictionary<string, ApiPluginExecutor>(StringComparer.OrdinalIgnoreCase);

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

        public new void Add(Module item)
        {
            lock (this.syncRoot)
            {
                this.innerCollection.Add(item);
                item.Proxy.AppExecutors.ForEach(x => x.Aliases.ForEach(y => appExecs[y] = x));
                item.Proxy.ApiExecutors.ForEach(x => x.Aliases.ForEach(y => apiExecs[y] = x));
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
                var apisToRemove = items.SelectMany(x => x.Proxy.ApiExecutors).ToList();
                var appsToRemove = items.SelectMany(x => x.Proxy.AppExecutors).ToList();
                appsToRemove.ForEach((plugin) => appExecs.RemoveValue(plugin));
                apisToRemove.ForEach((plugin) => apiExecs.RemoveValue(plugin));
            }
        }
    }
}

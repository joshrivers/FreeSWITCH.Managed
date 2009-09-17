using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class SynchronizedList<T> : ISynchronizedList<T>
    {
        protected List<T> innerCollection;
        protected object syncRoot;

        public SynchronizedList()
        {
            this.syncRoot = new object();
            this.innerCollection = new List<T>();
        }

        public void Add(T item)
        {
            lock (this.syncRoot)
            {
                this.innerCollection.Add(item);
            }
        }

        public bool Remove(T item)
        {
            lock (this.syncRoot)
            {
                return this.innerCollection.Remove(item);
            }
        }

        public void Clear()
        {
            lock (this.syncRoot)
            {
                this.innerCollection.Clear();
            }
        }

        public ReadOnlyCollection<T> Items
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.innerCollection.ToList().AsReadOnly();
                }
            }
        }
    }
}
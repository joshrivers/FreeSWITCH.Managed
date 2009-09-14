using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class SynchronizedDictionary<TKey, TValue>
    {
        protected Dictionary<TKey, TValue> innerCollection;
        protected object syncRoot;

        public SynchronizedDictionary()
        {
            this.syncRoot = new object();
            this.innerCollection = new Dictionary<TKey, TValue>();
        }

        public SynchronizedDictionary(IEqualityComparer<TKey> comparer)
        {
            this.syncRoot = new object();
            this.innerCollection = new Dictionary<TKey, TValue>(comparer);
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.innerCollection[key];
                }
            }
            set
            {
                lock (this.syncRoot)
                {
                    this.innerCollection[key]=value;
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (this.syncRoot)
            {
                this.innerCollection.Add(key, value);
            }
        }

        public bool Remove(TKey key)
        {
            lock (this.syncRoot)
            {
                return this.innerCollection.Remove(key);
            }
        }

        public void RemoveValue(TValue value)
        {
            lock (this.syncRoot)
            {
                List<TKey> keys = (from pair in this.innerCollection
                                   where pair.Value.Equals(value)
                                   select pair.Key).ToList();
                keys.ForEach(key => this.innerCollection.Remove(key));
            }
        }

        public void Clear()
        {
            lock (this.syncRoot)
            {
                this.innerCollection.Clear();
            }
        }


        public Dictionary<TKey, TValue> ToDictionary()
        {
            lock (this.syncRoot)
            {
                var result = new Dictionary<TKey, TValue>();
                this.innerCollection.ForEach((pair) => result.Add(pair.Key, pair.Value));
                return result;
            }
        }
        public ReadOnlyCollection<TKey> Keys
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.innerCollection.Keys.ToList().AsReadOnly();
                }
            }
        }

        public ReadOnlyCollection<TValue> Values
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.innerCollection.Values.ToList().AsReadOnly();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public interface ISynchronizedDictionary<TKey, TValue>
    {
        TValue this[TKey key] { get; set; }
        void Add(TKey key, TValue value);
        bool Remove(TKey key);
        void RemoveValue(TValue value);
        void Clear();
        Dictionary<TKey, TValue> ToDictionary();
        ReadOnlyCollection<TKey> Keys { get; }
        ReadOnlyCollection<TValue> Values { get; }
    }
}

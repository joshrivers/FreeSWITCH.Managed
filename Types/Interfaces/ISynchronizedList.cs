using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public interface ISynchronizedList<T>
    {
        void Add(T item);
        bool Remove(T item);
        void Clear();
        ReadOnlyCollection<T> Items { get; }
    }
}

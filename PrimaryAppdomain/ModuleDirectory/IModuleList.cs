using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public interface IModuleList
    {
        List<Module> this[string fileName] { get; }
        bool NoReloadEnabled(string fileName);
        void RemoveAll(List<Module> items);
        void Add(Module item);
        bool Remove(Module item);
        void Clear();
    }
}

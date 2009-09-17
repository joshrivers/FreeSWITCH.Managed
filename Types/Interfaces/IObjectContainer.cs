using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public interface IObjectContainer
    {
        SynchronizedDictionary<string, object> Configuration { get; }
        void DeclareSingleton(Type type);
        void Register<T>(Creator creator);
        void RegisterSingleton<T>(Creator creator);
        T Create<T>();
        T GetConfiguration<T>(string name);
        string ToString();
        void Reset();
    }
}

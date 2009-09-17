using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class ModuleServiceLocator : ObjectContainer
    {
        public static ModuleServiceLocator Container { get; private set; }
        public static string MasterAssemblyPath { get; set; }
        static ModuleServiceLocator()
        {
            ModuleServiceLocator.Container = new ModuleServiceLocator();
            var registry = new ModuleRegistry();
            registry.Register(ModuleServiceLocator.Container);
        }
    }
}

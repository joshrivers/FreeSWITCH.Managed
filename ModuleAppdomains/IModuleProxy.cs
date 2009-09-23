using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public interface IModuleProxy
    {
        object InitializeLifetimeService();
        PluginHandlerOrchestrator PluginHandlerOrchestrator { get; }
        LogDirector LogDirector { get; }
        LogDirector Logger { get; }
        ModuleAssemblyLoader AssemblyLoader { get; }
        string MasterAssemblyPath { get; set; }
    }
}

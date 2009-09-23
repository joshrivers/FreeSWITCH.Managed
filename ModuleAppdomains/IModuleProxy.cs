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
        IPluginHandlerOrchestrator PluginHandlerOrchestrator { get; }
        ILoggerContainer LogDirector { get; }
        ILoggerContainer Logger { get; }
        IModuleAssemblyLoader AssemblyLoader { get; }
        string MasterAssemblyPath { get; set; }
    }
}

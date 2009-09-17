using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public interface IPluginHandler : IRunCommand, IExecuteBackgroundCommand, IExecuteCommand
    {
        bool NoReload { get; }
        void LoadPlugins(Assembly assembly);
        void Unload();
    }
}

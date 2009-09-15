using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class DefaultRunCommand : IRunCommand
    {
        public IModuleListContainer moduleListContainer;
        public DefaultRunCommand(IModuleListContainer moduleListContainer)
        {
            this.moduleListContainer = moduleListContainer;
        }

        public bool Run(string command, IntPtr sessionHandle)
        {
            try
            {
                Log.WriteLine(LogLevel.Debug, "FreeSWITCH.Managed: attempting to run application '{0}'.", command);
                System.Diagnostics.Debug.Assert(sessionHandle != IntPtr.Zero, "sessionHandle is null.");
                var parsed = command.FreeSWITCHCommandParse();
                if (parsed == null) return false;
                var fullName = parsed[0];
                var args = parsed[1];

                AppPluginExecutor exec;
                var execs = moduleListContainer.ModuleList.appExecs.ToDictionary();
                if (!execs.TryGetValue(fullName, out exec))
                {
                    Log.WriteLine(LogLevel.Error, "App plugin {0} not found.", fullName);
                    return false;
                }
                return exec.Execute(args, sessionHandle);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Exception in Run({0}): {1}", command, ex.ToString());
                return false;
            }
        }
    }
}

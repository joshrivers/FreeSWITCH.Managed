using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class DefaultExecuteCommand : IExecuteCommand
    {
        private IModuleListContainer moduleListContainer;

        public DefaultExecuteCommand(IModuleListContainer moduleListContainer)
        {
            this.moduleListContainer = moduleListContainer;
        }

        public bool Execute(string command, IntPtr streamHandle, IntPtr eventHandle)
        {
            try
            {
                System.Diagnostics.Debug.Assert(streamHandle != IntPtr.Zero, "streamHandle is null.");
                var parsed = command.FreeSWITCHCommandParse();
                if (parsed == null) return false;
                var fullName = parsed[0];
                var args = parsed[1];

                var execs = moduleListContainer.ModuleList.apiExecs.ToDictionary();
                ApiPluginExecutor exec;
                if (!execs.TryGetValue(fullName, out exec))
                {
                    Log.WriteLine(LogLevel.Error, "API plugin {0} not found.", fullName);
                    return false;
                }
                var res = exec.ExecuteApi(args, streamHandle, eventHandle);
                return res;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Exception in Execute({0}): {1}", command, ex.ToString());
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class DefaultExecuteBackgroundCommand : IExecuteBackgroundCommand
    {
        public IModuleListContainer moduleListContainer;
        public DefaultExecuteBackgroundCommand(IModuleListContainer moduleListContainer)
        {
            this.moduleListContainer = moduleListContainer;
        }

        public bool ExecuteBackground(string command)
        {
            try
            {
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
                return exec.ExecuteApiBackground(args);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Exception in ExecuteBackground({0}): {1}", command, ex.ToString());
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class ExecuteBackgroundCommandDirector
    {
        public SynchronizedList<IExecuteBackgroundCommand> Commands { get; private set; }
        private ILogger logDirector;

        public ExecuteBackgroundCommandDirector(ILogger logDirector)
        {
            this.logDirector = logDirector;
            this.Commands = new SynchronizedList<IExecuteBackgroundCommand>();
        }

        public bool ExecuteBackground(string command)
        {
            try
            {
                bool runResult = true;
                foreach (var commandObject in Commands.Items)
                {
                    bool success = commandObject.ExecuteBackground(command);
                    if (!success)
                    {
                        runResult = false;
                    }
                }
                return runResult;
            }
            catch (Exception ex)
            {
                logDirector.Error(string.Format("Exception in ExecuteBackground({0}): {1}", command, ex.ToString()));
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class ExecuteCommandDirector
    {
        public SynchronizedList<IExecuteCommand> Commands { get; private set; }
        private ILogger logDirector;

        public ExecuteCommandDirector(ILogger logDirector)
        {
            this.logDirector = logDirector;
            this.Commands = new SynchronizedList<IExecuteCommand>();
        }

        public bool Execute(string command, IntPtr streamHandle, IntPtr eventHandle)
        {
            try
            {
                bool runResult = true;
                foreach (var commandObject in Commands.Items)
                {
                    bool success = commandObject.Execute(command, streamHandle, eventHandle);
                    if (!success)
                    {
                        runResult = false;
                    }
                }
                return runResult;
            }
            catch (Exception ex)
            {
                logDirector.Error(string.Format("Exception in Execute({0}): {1}", command, ex.ToString()));
                return false;
            }
        }
    }
}

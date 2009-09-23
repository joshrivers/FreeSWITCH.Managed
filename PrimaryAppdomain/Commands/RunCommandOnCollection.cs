using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class RunCommandOnCollection : IRunCommand
    {
        public SynchronizedList<IRunCommand> Commands { get; private set; }
        private ILogger logDirector;

        public RunCommandOnCollection(ILogger logDirector)
        {
            this.logDirector = logDirector;
            this.Commands = new SynchronizedList<IRunCommand>();
        }

        public bool Run(string command, IntPtr sessionHandle)
        {
            try
            {
                bool runResult = true;
                foreach (var commandObject in Commands.Items)
                {
                    bool success = commandObject.Run(command, sessionHandle);
                    if (!success)
                    {
                        runResult = false;
                    }
                }
                return runResult;
            }
            catch (Exception ex)
            {
                logDirector.Error(string.Format("Exception in Run({0}): {1}", command, ex.ToString()));
                return false;
            }
        }
    }
}

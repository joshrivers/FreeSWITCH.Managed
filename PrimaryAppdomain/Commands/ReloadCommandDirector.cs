using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class ReloadCommandOnCollection : IReloadCommand
    {
        public SynchronizedList<IReloadCommand> Commands { get; private set; }
        private ILogger logDirector;

        public ReloadCommandOnCollection(ILogger logDirector)
        {
            this.logDirector = logDirector;
            this.Commands = new SynchronizedList<IReloadCommand>();
        }

        public bool Reload(string command)
        {
            try
            {
                bool runResult = true;
                foreach (var commandObject in Commands.Items)
                {
                    bool success = commandObject.Reload(command);
                    if (!success)
                    {
                        runResult = false;
                    }
                }
                return runResult;
            }
            catch (Exception ex)
            {
                logDirector.Error(string.Format("Error reloading {0}: {1}", command, ex.ToString()));
                return false;
            }
        }
    }
}

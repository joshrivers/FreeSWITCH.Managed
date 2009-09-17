using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class DefaultRunCommand : IRunCommand
    {
        public ModuleList moduleList;
        public DefaultRunCommand(ModuleList moduleList)
        {
            this.moduleList = moduleList;
        }

        public bool Run(string command, IntPtr sessionHandle)
        {
            bool returnValue = false;
            this.moduleList.Items.ForEach(module =>
            {
                bool result = module.Proxy.PluginHandlerOrchestrator.Run(command, sessionHandle);
                if (result == true) { returnValue = true; }
            });
            return returnValue;
        }
    }
}

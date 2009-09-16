using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class DefaultExecuteBackgroundCommand : IExecuteBackgroundCommand
    {
        public ModuleList moduleList;
        public DefaultExecuteBackgroundCommand(ModuleList moduleList)
        {
            this.moduleList = moduleList;
        }

        public bool ExecuteBackground(string command)
        {
            bool returnValue = false;
            this.moduleList.Items.ForEach(module =>
            {
                bool result = module.Proxy.PluginHandlerOrchestrator.ExecuteBackground(command);
                if (result == true) { returnValue = true; }
            });
            return returnValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public class ExecuteCommandOnModuleList : IExecuteCommand
    {
        public ModuleList moduleList;
        public ExecuteCommandOnModuleList(ModuleList moduleList)
        {
            this.moduleList = moduleList;
        }

        public bool Execute(string command, IntPtr streamHandle, IntPtr eventHandle)
        {
            bool returnValue = false;
            this.moduleList.Items.ForEach(module =>
            {
                bool result = module.Proxy.PluginHandlerOrchestrator.Execute(command, streamHandle, eventHandle);
                if (result == true) { returnValue = true; }
            });
            return returnValue;
        }
    }
}

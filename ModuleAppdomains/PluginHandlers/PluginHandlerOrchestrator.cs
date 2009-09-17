using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class PluginHandlerOrchestrator: MarshalByRefObject, IPluginHandler
    {
        public PluginHandlerOrchestrator()
        {
        }

        public bool NoReload
        {
            get
            {
                foreach (var handler in handlers)
                {
                    if (handler.NoReload) { return true; };
                }
                return false;
            }
        }
        public string FileName { get; set; }
        private List<IPluginHandler> handlers = new List<IPluginHandler>();

        public void LoadPlugins(Assembly assembly)
        {
            var newHandlers = InternalAppdomainServiceLocator.Container.Create<List<IPluginHandler>>();
            newHandlers.ForEach(h => h.LoadPlugins(assembly));
            this.handlers.AddRange(newHandlers);
        }

        public void Unload()
        {
            foreach (var handler in handlers)
            {
                handler.Unload();
            }
        }

        public bool Run(string command, IntPtr sessionHandle)
        {
            bool returnValue = false;
            handlers.ForEach(handler =>
            {
                bool result = handler.Run(command, sessionHandle);
                if (result == true) { returnValue = true; }
            });
            return returnValue;
        }

        public bool ExecuteBackground(string command)
        {
            bool returnValue = false;
            handlers.ForEach(handler =>
            {
                bool result = handler.ExecuteBackground(command);
                if (result == true) { returnValue = true; }
            });
            return returnValue;
        }

        public bool Execute(string command, IntPtr streamHandle, IntPtr eventHandle)
        {
            bool returnValue = false;
            handlers.ForEach(handler =>
            {
                bool result = handler.Execute(command, streamHandle, eventHandle);
                if (result == true) { returnValue = true; }
            });
            return returnValue;
        }
    }
}

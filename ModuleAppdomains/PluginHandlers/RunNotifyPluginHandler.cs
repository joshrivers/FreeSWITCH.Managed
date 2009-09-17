using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class RunNotifyPluginHandler : IPluginHandler
    {
        public bool NoReload {get{return false;}}
        public void LoadPlugins(Assembly assembly)
        {
            var allTypes = assembly.GetExportedTypes();
            // Run Load on all the load plugins
            var ty = typeof(ILoadNotificationPlugin);
            var pluginTypes = allTypes.Where(x => ty.IsAssignableFrom(x) && !x.IsAbstract).ToList();
            if (pluginTypes.Count == 0) return;
            foreach (var pt in pluginTypes)
            {
                var plugin = ((ILoadNotificationPlugin)Activator.CreateInstance(pt, true));
                if (!plugin.Load())
                {
                    throw new RunNotifyException(string.Format("Type {0} requested no loading. Assembly will not be loaded.", pt.FullName));
                }
            }
            return;
        }

        public void Unload()
        {
            return;
        }

        public bool Run(string command, IntPtr sessionHandle)
        {
            return false;
        }

        public bool ExecuteBackground(string command)
        {
            return false;
        }

        public bool Execute(string command, IntPtr streamHandle, IntPtr eventHandle)
        {
            return false;
        }
    }
}

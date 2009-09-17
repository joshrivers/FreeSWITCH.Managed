using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FreeSWITCH
{
    public sealed class AppPluginExecutor : PluginExecutor
    {
        readonly Func<IAppPlugin> createPlugin;

        public AppPluginExecutor(string name, List<string> aliases, Func<IAppPlugin> creator, PluginOptions pluginOptions)
            : base(name, aliases, pluginOptions)
        {
            if (creator == null) throw new ArgumentNullException("Creator cannot be null.");
            this.createPlugin = creator;
        }

        public bool Execute(string args, IntPtr sessionHandle)
        {
            IncreaseUse();
            try
            {
                using (var session = new Native.ManagedSession(new Native.SWIGTYPE_p_switch_core_session(sessionHandle, false)))
                {
                    session.Initialize();
                    session.SetAutoHangup(false);
                    try
                    {
                        var plugin = createPlugin();
                        var context = new AppContext(args, session); ;
                        plugin.Run(context);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogException("Run", Name, ex);
                        return false;
                    }
                }
            }
            finally
            {
                DecreaseUse();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FreeSWITCH
{
    public sealed class ApiPluginExecutor : PluginExecutor
    {

        readonly Func<IApiPlugin> createPlugin;

        public ApiPluginExecutor(string name, List<string> aliases, Func<IApiPlugin> creator, PluginOptions pluginOptions)
            : base(name, aliases, pluginOptions)
        {
            if (creator == null) throw new ArgumentNullException("Creator cannot be null.");
            this.createPlugin = creator;
        }

        public bool ExecuteApi(string args, IntPtr streamHandle, IntPtr eventHandle)
        {
            IncreaseUse();
            try
            {
                using (var stream = new Native.Stream(new Native.switch_stream_handle(streamHandle, false)))
                using (var evt = eventHandle == IntPtr.Zero ? null : new Native.Event(new Native.switch_event(eventHandle, false), 0))
                {
                    try
                    {
                        var context = new ApiContext(args, stream, evt);
                        var plugin = createPlugin();
                        plugin.Execute(context);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogException("Execute", Name, ex);
                        return false;
                    }
                }
            }
            finally
            {
                DecreaseUse();
            }
        }

        public bool ExecuteApiBackground(string args)
        {
            // Background doesn't affect use count
            new System.Threading.Thread(() =>
            {
                try
                {
                    var context = new ApiBackgroundContext(args);
                    var plugin = createPlugin();
                    plugin.ExecuteBackground(context);
                    Log.WriteLine(LogLevel.Debug, "ExecuteBackground in {0} completed.", Name);
                }
                catch (Exception ex)
                {
                    LogException("ExecuteBackground", Name, ex);
                }
            }).Start();
            return true;
        }
    }
}

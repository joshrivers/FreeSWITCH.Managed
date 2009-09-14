using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class PluginInfo
    {
        public string FileName { get; set; }
        public AppDomain Domain { get; set; }
        public PluginManager Manager { get; set; }

        public void Remove()
        {
            var t = new System.Threading.Thread(() =>
            {
                var friendlyName = this.Domain.FriendlyName;
                //logger.Info(string.Format("Starting to unload {0}, domain {1}.", this.FileName, friendlyName));
                try
                {
                    var d = this.Domain;
                    this.Manager.BlockUntilUnloadIsSafe();
                    this.Manager = null;
                    this.Domain = null;
                    AppDomain.Unload(d);
                    //logger.Info(string.Format("Unloaded {0}, domain {1}.", this.FileName, friendlyName));
                }
                catch (Exception)
                {
                    //logger.Alert(string.Format("Could not unload {0}, domain {1}: {2}", this.FileName, friendlyName, ex.ToString()));
                }
            });
            t.Priority = System.Threading.ThreadPriority.BelowNormal;
            t.IsBackground = true;
            t.Start();
        }

        public bool NoReload
        {
            get
            {
                return this.Manager.ApiExecutors.Any(x => (x.PluginOptions & PluginOptions.NoAutoReload) == PluginOptions.NoAutoReload) ||
                                   this.Manager.AppExecutors.Any(x => (x.PluginOptions & PluginOptions.NoAutoReload) == PluginOptions.NoAutoReload);
            }
        }
    }
}

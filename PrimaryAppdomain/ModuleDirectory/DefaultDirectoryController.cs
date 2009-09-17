using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FreeSWITCH.Managed
{
    public class DefaultDirectoryController : IDirectoryController
    {
        public string PluginDirectoryPath { get; private set; }
        public string ShadowDirectoryPath { get; private set; }

        public DefaultDirectoryController()
        {
            try
            {
                this.PluginDirectoryPath = Path.Combine(Native.freeswitch.SWITCH_GLOBAL_dirs.mod_dir, "managed");
                this.ShadowDirectoryPath = Path.Combine(this.PluginDirectoryPath, "shadow");
            }
            catch
            {
                this.PluginDirectoryPath = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(this.PluginDirectoryPath);
                this.ShadowDirectoryPath = Path.Combine(this.PluginDirectoryPath, "shadow");
            }
        }

        public bool PluginDirectoryExists()
        {
            return Directory.Exists(this.PluginDirectoryPath);
        }

        public void ClearShadowDirecory()
        {
            if (Directory.Exists(this.ShadowDirectoryPath))
            {
                Directory.Delete(this.ShadowDirectoryPath, true);
                Directory.CreateDirectory(this.ShadowDirectoryPath);
            }
        }
    }
}

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
        public string ModuleDirectoryPath { get; private set; }
        public string ShadowDirectoryPath { get; private set; }

        public DefaultDirectoryController()
        {
            try
            {
                this.ModuleDirectoryPath = Path.Combine(Native.freeswitch.SWITCH_GLOBAL_dirs.mod_dir, "managed");
                this.ShadowDirectoryPath = Path.Combine(this.ModuleDirectoryPath, "shadow");
            }
            catch
            {
                this.ModuleDirectoryPath = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(this.ModuleDirectoryPath);
                this.ShadowDirectoryPath = Path.Combine(this.ModuleDirectoryPath, "shadow");
            }
        }

        public bool ModuleDirectoryExists()
        {
            return Directory.Exists(this.ModuleDirectoryPath);
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

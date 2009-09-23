using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class ReloadCommandOnModuleList : IReloadCommand
    {
        private IDirectoryController directories;
        public ModuleList moduleList;
        private IModuleLoader moduleLoader;

        public ReloadCommandOnModuleList(IDirectoryController directories, ModuleList moduleList, IModuleLoader moduleLoader)
        {
            this.directories = directories;
            this.moduleList = moduleList;
            this.moduleLoader = moduleLoader;
        }

        public bool Reload(string command)
        {
            try
            {
                if (Path.IsPathRooted(command))
                {
                    moduleLoader.LoadModule(command);
                }
                else
                {
                    moduleLoader.LoadModule(Path.Combine(this.directories.ModuleDirectoryPath, command));
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Error reloading {0}: {1}", command, ex.ToString());
                return false;
            }
        }
    }
}

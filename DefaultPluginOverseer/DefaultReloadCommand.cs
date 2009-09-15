using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class DefaultReloadCommand : IReloadCommand
    {
        private string pluginDirectoryPath;
        private IModuleListContainer moduleListContainer;
        private IModuleLoader moduleLoader;

        public DefaultReloadCommand(string pluginDirectoryPath, IModuleListContainer moduleListContainer, IModuleLoader moduleLoader)
        {
            this.moduleListContainer = moduleListContainer;
            this.moduleLoader = moduleLoader;
            this.pluginDirectoryPath = pluginDirectoryPath;
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
                    moduleLoader.LoadModule(Path.Combine(this.pluginDirectoryPath, command));
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

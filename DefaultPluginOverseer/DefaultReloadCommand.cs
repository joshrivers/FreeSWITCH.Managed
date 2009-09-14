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

        public DefaultReloadCommand(string pluginDirectoryPath)
        {
            this.pluginDirectoryPath = pluginDirectoryPath;
        }
        public bool Reload(string command)
        {
            try
            {
                if (Path.IsPathRooted(command))
                {
                    FileLoader.loadFile(command);
                }
                else
                {
                    FileLoader.loadFile(Path.Combine(this.pluginDirectoryPath, command));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class DefaultModuleLoader : IModuleLoader
    {
        private ModuleList moduleList;
        private ILogger logger;
        public DefaultModuleLoader(ILogger logger, ModuleList moduleList)
        {
            this.moduleList = moduleList;
            this.logger = logger;
        }
        public void LoadModule(string fileName)
        {
            try
            {
                fileName = RemoveDotConfigExtensionIfPresent(fileName);
                if (moduleList.NoReloadEnabled(fileName)) { return; }
                // Attempts to load the file. On failure, it will call unload.

                // We might get a load for a file that's no longer there. Just unload the old one.
                if (!File.Exists(fileName))
                {
                    moduleList.RemoveAll(moduleList[fileName]);
                    return;
                }
                var module = DefaultServiceLocator.Container.Create<Module>();

                module.Initialize(fileName);
                moduleList.RemoveAll(moduleList[fileName]);
                moduleList.Add(module);

                logger.Info(string.Format("Finished loading {0} into domain {1}.", module.FilePath, module.Domain.FriendlyName));
            }
            catch (Exception ex)
            {
                moduleList.RemoveAll(moduleList[fileName]);
                logger.Error(string.Format("Exception loading change from {0}: {1}", fileName, ex.ToString()));
            }
        }

        private static void RemoveDotConfigExtensionIfPresent(string fileName)
        {
            if ((Path.GetExtension(fileName).ToLowerInvariant() == ".config"))
            {
                fileName = Path.ChangeExtension(fileName, null);
            }
            return fileName;
        }
    }
}

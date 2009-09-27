using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class PrimaryDomainExtensionModuleLoader : IModuleLoader
    {
        private ILogger logger;
        public PrimaryDomainExtensionModuleLoader(ILogger logger)
        {
            this.logger = logger;

        }

        public void LoadModule(string fileName)
        {
            this.logger.Notice(string.Format("Loading file {0}", fileName));
            if (File.Exists(fileName))
            {
                Assembly loaded = Assembly.LoadFile(fileName);
                var types = loaded.GetExportedTypes();
                foreach (var item in types)
                {
                    this.logger.Notice(string.Format("Checking type {0}", item.FullName));
                    if (typeof(IPrimaryAppdomainExtension).IsAssignableFrom(item))
                    {
                        this.logger.Notice(string.Format("Found extension {0}", item.FullName));
                        var instance = loaded.CreateInstance(item.FullName) as IPrimaryAppdomainExtension;
                        instance.Load();
                    }
                }

            }
        }
    }
}

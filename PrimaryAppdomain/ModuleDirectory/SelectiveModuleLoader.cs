using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class SelectiveModuleLoader : IModuleLoader
    {
        private PrimaryDomainExtensionModuleLoader primaryLoader;
        private DefaultModuleLoader defaultLoader;
        public SelectiveModuleLoader(DefaultModuleLoader defaultLoader, PrimaryDomainExtensionModuleLoader primaryLoader)
        {
            this.primaryLoader = primaryLoader;
            this.defaultLoader = defaultLoader;
        }
        public void LoadModule(string fileName)
        {
            if (fileName.Contains(".primary"))
            {
                this.primaryLoader.LoadModule(fileName);
            }
            else
            {
                this.defaultLoader.LoadModule(fileName);
            }
        }
    }
}

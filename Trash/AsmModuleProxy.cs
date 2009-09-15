using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    internal class AsmModuleProxy : ModuleProxy
    {

        protected override bool LoadInternal(string fileName)
        {
            DefaultLoader.Loader.PluginOverSeer.Logger.Notice(string.Format("Log 2" + fileName.GetLoweredFileExtension()));
            Assembly asm = DefaultLoader.Loader.PluginOverSeer.AssemblyComposers[fileName.GetLoweredFileExtension()].GetComposer().GetAssembly(fileName);
            DefaultLoader.Loader.PluginOverSeer.Logger.Notice(string.Format("Log 3" + fileName.GetLoweredFileExtension()));
            this.LoadPlugins(asm, fileName);
            return true;
        }
    }
}

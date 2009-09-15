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
            Assembly asm = DefaultLoader.Loader.PluginOverSeer.AssemblyComposers[fileName.GetLoweredFileExtension()].GetComposer().GetAssembly(fileName);
            this.LoadPlugins(asm, fileName);
            return true;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FreeSWITCH
{
    internal class AsmPluginManager : PluginManager
    {

        protected override bool LoadInternal(string fileName)
        {
            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(fileName);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Info, "Couldn't load {0}: {1}", fileName, ex.Message);
                return false;
            }

            // Ensure it's a plugin assembly
            var ourName = Assembly.GetExecutingAssembly().GetName().Name;
            if (!asm.GetReferencedAssemblies().Any(n => n.Name == ourName))
            {
                Log.WriteLine(LogLevel.Debug, "Assembly {0} doesn't reference FreeSWITCH.Managed, not loading.");
                return false;
            }

            // See if it wants to be loaded
            var allTypes = asm.GetExportedTypes();
            if (!RunLoadNotify(allTypes)) return false;

            var opts = GetOptions(allTypes);

            AddApiPlugins(allTypes, opts);
            AddAppPlugins(allTypes, opts);

            return true;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class DefaultRegistry
    {
        public void Register(DefaultServiceLocator registry)
        {
            ConfigureProxyTypes(registry);
            registry.RegisterSingleton<IDirectoryController>(container =>
            {
                return new DefaultDirectoryController();
            });
            registry.DeclareSingleton(typeof(LogDirector));
            registry.RegisterSingleton<ILogger>(container =>
            {
                return container.Create<LogDirector>();
            });
            registry.Register<IModuleLoader>(container =>
            {
                return container.Create<DefaultModuleLoader>();
            });
            registry.DeclareSingleton(typeof(ModuleList));
            registry.DeclareSingleton(typeof(RunCommandOnCollection));
            registry.DeclareSingleton(typeof(ExecuteCommandOnCollection));
            registry.DeclareSingleton(typeof(ExecuteBackgroundCommandOnCollection));
            registry.DeclareSingleton(typeof(ReloadCommandOnCollection));
            registry.DeclareSingleton(typeof(DefaultPluginOverseer));
            registry.DeclareSingleton(typeof(DefaultModuleDirectorySupervisor));
            registry.DeclareSingleton(typeof(AssemblyResolver));
        }

        public void ConfigureProxyTypes(DefaultServiceLocator registry)
        {
            ModuleProxyTypeDictionary dictionary = new ModuleProxyTypeDictionary();
            dictionary.Add(".dll",typeof(ModuleProxy));
            dictionary.Add(".exe",typeof(ModuleProxy));
            dictionary.Add(".fsx",typeof(ModuleProxy));
            dictionary.Add(".vbx",typeof(ModuleProxy));
            dictionary.Add(".csx",typeof(ModuleProxy));
            dictionary.Add(".jsx",typeof(ModuleProxy));
            registry.RegisterSingleton<ModuleProxyTypeDictionary>(container =>
            {
                return dictionary;
            });
        }
    }
}

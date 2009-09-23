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
    }
}

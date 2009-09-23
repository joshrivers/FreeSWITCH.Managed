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
            //System.Diagnostics.Debugger.Break(); 
            ConfigureProxyTypes(registry);
            registry.RegisterSingleton<IDirectoryController>(container =>
            {
                return new DefaultDirectoryController();
            });
            registry.RegisterSingleton<LoggerContainer>(container =>
            {
                var logger = new LoggerContainer();
                logger.Add(new DefaultLogger());
                return logger;
            });
            registry.Register<ILogger>(container =>
            {
                return container.Create<LoggerContainer>();
            });
            registry.Register<ILoggerContainer>(container =>
            {
                return container.Create<LoggerContainer>();
            });
            registry.Register<IModuleLoader>(container =>
            {
                return container.Create<DefaultModuleLoader>();
            });
            registry.Register<IAppDomainFactory>(container =>
            {
                return container.Create<DefaultAppDomainFactory>();
            });
            registry.DeclareSingleton(typeof(ModuleList));
            registry.DeclareSingleton(typeof(RunCommandOnCollection));
            registry.DeclareSingleton(typeof(ExecuteCommandOnCollection));
            registry.DeclareSingleton(typeof(ExecuteBackgroundCommandOnCollection));
            registry.DeclareSingleton(typeof(ReloadCommandOnCollection));
            registry.DeclareSingleton(typeof(DefaultModuleDirectorySupervisor));
            registry.DeclareSingleton(typeof(AssemblyResolver));
            //registry.DeclareSingleton(typeof(DefaultLoader));
            AddCommandHandlersToCollections();

        }

        private static void AddCommandHandlersToCollections()
        {
            RunCommandOnModuleList runcommand = DefaultServiceLocator.Container.Create<RunCommandOnModuleList>();
            DefaultServiceLocator.Container.Create<RunCommandOnCollection>().Commands.Add(runcommand);

            ExecuteCommandOnModuleList executecommmand = DefaultServiceLocator.Container.Create<ExecuteCommandOnModuleList>();
            DefaultServiceLocator.Container.Create<ExecuteCommandOnCollection>().Commands.Add(executecommmand);

            ExecuteBackgroundCommandOnModuleList executebackgroundcommmand = DefaultServiceLocator.Container.Create<ExecuteBackgroundCommandOnModuleList>();
            DefaultServiceLocator.Container.Create<ExecuteBackgroundCommandOnCollection>().Commands.Add(executebackgroundcommmand);

            ReloadCommandOnModuleList reloadcommand = DefaultServiceLocator.Container.Create<ReloadCommandOnModuleList>();
            DefaultServiceLocator.Container.Create<ReloadCommandOnCollection>().Commands.Add(reloadcommand);
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

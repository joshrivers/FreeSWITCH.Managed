using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class DefaultPluginOverseer
    {
        private DefaultModuleDirectorySupervisor watcher;
        private IDirectoryController directories;
        private AssemblyResolver resolver;
        private ILogger logger;

        public DefaultPluginOverseer(IDirectoryController directories, LogDirector logger,
            DefaultModuleDirectorySupervisor watcher, AssemblyResolver resolver)
        {
            this.resolver = resolver;
            this.directories = directories;
            this.logger = logger;
            logger.Add(new DefaultLogger());
        }

        public bool Load()
        {
            DefaultRunCommand runcommand = DefaultServiceLocator.Container.Create<DefaultRunCommand>();
            DefaultServiceLocator.Container.Create<RunCommandDirector>().Commands.Add(runcommand);

            DefaultExecuteCommand executecommmand = DefaultServiceLocator.Container.Create<DefaultExecuteCommand>();
            DefaultServiceLocator.Container.Create<ExecuteCommandOnCollection>().Commands.Add(executecommmand);

            ExecuteBackgroundCommandOnModuleList executebackgroundcommmand = DefaultServiceLocator.Container.Create<ExecuteBackgroundCommandOnModuleList>();
            DefaultServiceLocator.Container.Create<ExecuteBackgroundCommandOnCollection>().Commands.Add(executebackgroundcommmand);

            ReloadCommandOnModuleList reloadcommand = DefaultServiceLocator.Container.Create<ReloadCommandOnModuleList>();
            DefaultServiceLocator.Container.Create<ReloadCommandDirector>().Commands.Add(reloadcommand);

            this.resolver.AttachDefaultAssemblyResolver();

            this.watcher = DefaultServiceLocator.Container.Create<DefaultModuleDirectorySupervisor>();
            return this.watcher.Initialize();
        }
    }
}

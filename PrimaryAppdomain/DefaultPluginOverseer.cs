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
        private DefaultPluginDirectoryWatcher watcher;
        private IDirectoryController directories;
        private AssemblyResolver resolver;
        private ILogger logger;

        public DefaultPluginOverseer(IDirectoryController directories, LogDirector logger,
            DefaultPluginDirectoryWatcher watcher, AssemblyResolver resolver)
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
            DefaultServiceLocator.Container.Create<ExecuteCommandDirector>().Commands.Add(executecommmand);

            DefaultExecuteBackgroundCommand executebackgroundcommmand = DefaultServiceLocator.Container.Create<DefaultExecuteBackgroundCommand>();
            DefaultServiceLocator.Container.Create<ExecuteBackgroundCommandDirector>().Commands.Add(executebackgroundcommmand);

            DefaultReloadCommand reloadcommand = DefaultServiceLocator.Container.Create<DefaultReloadCommand>();
            DefaultServiceLocator.Container.Create<ReloadCommandDirector>().Commands.Add(reloadcommand);

            this.resolver.AttachDefaultAssemblyResolver();

            this.watcher = DefaultServiceLocator.Container.Create<DefaultPluginDirectoryWatcher>();
            return this.watcher.Initialize();
        }
    }
}

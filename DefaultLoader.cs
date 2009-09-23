using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class DefaultLoader
    {
        private DefaultModuleDirectorySupervisor directorySupervisor;
        private ExecuteBackgroundCommandOnCollection executeBackgroundCommand;
        private ExecuteCommandOnCollection executeCommand;
        private ILoggerContainer logger;
        private ReloadCommandOnCollection reloadCommand;
        private AssemblyResolver resolver;
        private RunCommandOnCollection runCommand;
        // Thread-safe singleton implementation from: http://www.yoda.arachsys.com/csharp/singleton.html
        static DefaultLoader() { DefaultLoader.Loader = DefaultServiceLocator.Container.Create<DefaultLoader>(); }

        public static DefaultLoader Loader { get; private set; }

        public DefaultLoader(RunCommandOnCollection runCommand,
            ExecuteCommandOnCollection executeCommand,
            ExecuteBackgroundCommandOnCollection executeBackgroundCommand,
            ReloadCommandOnCollection reloadCommand,
            AssemblyResolver resolver,
            DefaultModuleDirectorySupervisor directorySupervisor,
            ILoggerContainer logger)
        {
            this.logger = logger;
            this.directorySupervisor = directorySupervisor;
            this.resolver = resolver;
            this.reloadCommand = reloadCommand;
            this.executeBackgroundCommand = executeBackgroundCommand;
            this.executeCommand = executeCommand;
            this.runCommand = runCommand;
        }

        public bool Load()
        {
            CoreDelegates.Run = this.runCommand.Run;
            CoreDelegates.Execute = this.executeCommand.Execute;
            CoreDelegates.ExecuteBackground = this.executeBackgroundCommand.ExecuteBackground;
            CoreDelegates.Reload = this.reloadCommand.Reload;

            this.logger.Info("Load completed.");
            this.resolver.AttachDefaultAssemblyResolver();
            return this.directorySupervisor.Initialize();
        }
    }
}

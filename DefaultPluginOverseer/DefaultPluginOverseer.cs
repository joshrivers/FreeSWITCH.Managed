using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class DefaultPluginOverseer
    {
        public IDirectoryController directories { get; private set; }
        public LogDirector logger { get; private set; }
        public IDefaultServiceLocator locator { get; private set; }
        public DefaultPluginDirectoryWatcher watcher { get; private set; }

        public DefaultPluginOverseer(IDefaultServiceLocator locator, IDirectoryController directories, LogDirector logger)
        {
            this.directories = directories;
            this.logger = logger;
            this.locator = locator;
            this.watcher = new DefaultPluginDirectoryWatcher(this.directories, this.logger);
        }

        public bool Load()
        {
            DefaultRunCommand runcommand = new DefaultRunCommand();
            this.locator.RunCommandDirector.Commands.Add(runcommand);

            DefaultExecuteCommand executecommmand = new DefaultExecuteCommand();
            this.locator.ExecuteCommandDirector.Commands.Add(executecommmand);

            DefaultExecuteBackgroundCommand executebackgroundcommmand = new DefaultExecuteBackgroundCommand();
            this.locator.ExecuteBackgroundCommandDirector.Commands.Add(executebackgroundcommmand);

            DefaultReloadCommand reloadcommand = new DefaultReloadCommand(this.directories.PluginDirectoryPath);
            this.locator.ReloadCommandDirector.Commands.Add(reloadcommand);

            this.logger.Loggers.Add(new DefaultLogger());

            FileLoader.directories = this.directories;
            FileLoader.logger = this.logger;
            FileLoader.init();
            return this.watcher.Initialize();
        }
    }
}

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
        public IDirectoryController directories { get; private set; }
        public LogDirector logger { get; private set; }
        public IDefaultServiceLocator locator { get; private set; }
        public DefaultPluginDirectoryWatcher watcher { get; private set; }
        private ResolveEventHandler defaultEventResolver;

        public DefaultPluginOverseer(IDefaultServiceLocator locator, IDirectoryController directories, LogDirector logger)
        {
            this.directories = directories;
            this.logger = logger;
            this.locator = locator;
            this.watcher = new DefaultPluginDirectoryWatcher(this.directories, this.logger);
            this.defaultEventResolver = new ResolveEventHandler(DefaultAssemblyResolver);
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

            this.AttachDefaultAssemblyResolver();

            FileLoader.directories = this.directories;
            FileLoader.logger = this.logger;
            return this.watcher.Initialize();
        }


        public void AttachDefaultAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += defaultEventResolver;
        }

        public void DetatchDefaultAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= defaultEventResolver;
        }

        // This event handler resolves the filename of the requested assembly.
        // http://support.microsoft.com/kb/837908
        private Assembly DefaultAssemblyResolver(object sender, ResolveEventArgs args)
        {
            logger.Info(string.Format("Resolving assembly '{0}'.", args.Name));
            string currentAssemblyName = Assembly.GetExecutingAssembly().FullName;
            if (args.Name == currentAssemblyName)
            {
                return Assembly.GetExecutingAssembly();
            }
            string[] assemblyRefernceComponents = args.Name.Split(',');
            string newAssemblyName = assemblyRefernceComponents[0];
            string newAssemblyPath = Path.Combine(directories.PluginDirectoryPath, string.Format("{0}.dll", newAssemblyName));
            logger.Info(string.Format("Resolving to: '{0}'.", newAssemblyPath));
            if (File.Exists(newAssemblyPath))
            {
                return Assembly.LoadFile(newAssemblyPath);
            }
            else
            {
                return null;
            }
        }
    }
}

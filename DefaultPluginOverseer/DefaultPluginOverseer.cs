using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class DefaultPluginOverseer : IModuleListContainer, IModuleLoader
    {
        public IDirectoryController Directories { get; private set; }
        public LogDirector Logger { get; private set; }
        public IDefaultServiceLocator Locator { get; private set; }
        public DefaultPluginDirectoryWatcher Watcher { get; private set; }
        private ResolveEventHandler defaultEventResolver;
        public ModuleList ModuleList { get; private set; }

        public DefaultPluginOverseer(IDefaultServiceLocator locator, IDirectoryController directories, LogDirector logger)
        {
            this.Directories = directories;
            this.Logger = logger;
            this.Locator = locator;
            this.Watcher = new DefaultPluginDirectoryWatcher(this.Directories, this.Logger, this);
            this.defaultEventResolver = new ResolveEventHandler(DefaultAssemblyResolver);
            this.ModuleList = new ModuleList();
        }

        public bool Load()
        {
            DefaultRunCommand runcommand = new DefaultRunCommand(this);
            this.Locator.RunCommandDirector.Commands.Add(runcommand);

            DefaultExecuteCommand executecommmand = new DefaultExecuteCommand(this);
            this.Locator.ExecuteCommandDirector.Commands.Add(executecommmand);

            DefaultExecuteBackgroundCommand executebackgroundcommmand = new DefaultExecuteBackgroundCommand(this);
            this.Locator.ExecuteBackgroundCommandDirector.Commands.Add(executebackgroundcommmand);

            DefaultReloadCommand reloadcommand = new DefaultReloadCommand(this.Directories.PluginDirectoryPath,this,this);
            this.Locator.ReloadCommandDirector.Commands.Add(reloadcommand);

            this.Logger.Loggers.Add(new DefaultLogger());

            this.AttachDefaultAssemblyResolver();

            FileLoader.directories = this.Directories;
            FileLoader.logger = this.Logger;
            return this.Watcher.Initialize();
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
            Logger.Info(string.Format("Resolving assembly '{0}'.", args.Name));
            string currentAssemblyName = Assembly.GetExecutingAssembly().FullName;
            if (args.Name == currentAssemblyName)
            {
                return Assembly.GetExecutingAssembly();
            }
            string[] assemblyRefernceComponents = args.Name.Split(',');
            string newAssemblyName = assemblyRefernceComponents[0];
            string newAssemblyPath = Path.Combine(Directories.PluginDirectoryPath, string.Format("{0}.dll", newAssemblyName));
            Logger.Info(string.Format("Resolving to: '{0}'.", newAssemblyPath));
            if (File.Exists(newAssemblyPath))
            {
                return Assembly.LoadFile(newAssemblyPath);
            }
            else
            {
                return null;
            }
        }

        public void LoadModule(string fileName)
        {
            try
            {
                if ((Path.GetExtension(fileName).ToLowerInvariant() == ".config"))
                {
                    fileName = Path.ChangeExtension(fileName, null);
                }
                if (ModuleList.NoReloadEnabled(fileName)) { return; }
                // Attempts to load the file. On failure, it will call unload.
                // Loading part does not take out a lock. 
                // Lock is only done after loading is finished and dictionaries need updating.

                // We might get a load for a file that's no longer there. Just unload the old one.
                if (!File.Exists(fileName))
                {
                    ModuleList.RemoveAll(ModuleList[fileName]);
                    return;
                }
                var module = new Module(fileName, FileLoader.logger, FileLoader.directories);
                try
                {
                    module.Initialize();
                    ModuleList.RemoveAll(ModuleList[fileName]);
                    ModuleList.Add(module);
                }
                catch (Exception)
                {
                    ModuleList.RemoveAll(ModuleList[fileName]);
                }
                Logger.Info(string.Format("Finished loading {0} into domain {1}.", module.FileName, module.Domain.FriendlyName));
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Exception loading change from {0}: {1}", fileName, ex.ToString()));
            }
        }
    }
}

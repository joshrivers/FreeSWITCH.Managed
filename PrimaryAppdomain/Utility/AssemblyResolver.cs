using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class AssemblyResolver
    {
        private ResolveEventHandler defaultEventResolver;
        private IDirectoryController directories;
        private ILogger logger;
        public AssemblyResolver(ILogger logger, IDirectoryController directories)
        {
            this.directories = directories;
            this.logger = logger;
            this.defaultEventResolver = new ResolveEventHandler(DefaultAssemblyResolver);
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

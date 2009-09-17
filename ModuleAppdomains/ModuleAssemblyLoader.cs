using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class ModuleAssemblyLoader : MarshalByRefObject
    {
        private AssemblyComposerDictionary assemblyComposers;
        private PluginHandlerOrchestrator orchestrator;
        private ILogger logger;
        public ModuleAssemblyLoader(ILogger logger,
           AssemblyComposerDictionary assemblyComposers,
           PluginHandlerOrchestrator orchestrator)
        {
            this.orchestrator = orchestrator;
            this.assemblyComposers = assemblyComposers;
            this.logger = logger;
        }

        public bool Load(string filePath)
        {
            this.logger.Info(string.Format("Loading {0} from domain {1}", filePath, AppDomain.CurrentDomain.FriendlyName));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("Path parameter cannot be null or empty.");
            if (AppDomain.CurrentDomain.IsDefaultAppDomain()) throw new InvalidOperationException("Module must load in its own AppDomain.");
            bool result;
            try
            {
                var assemblyComposer = this.assemblyComposers[filePath.GetLoweredFileExtension()].Invoke();
                Assembly assembly = assemblyComposer.GetAssembly(filePath);
                this.ThrowExceptionIfNotPlugin(assembly);
                assembly.EntryPoint.CallEntryPoint();
                this.orchestrator.LoadPlugins(assembly);
                result = true;
            }
            catch (Exception ex)
            {
                this.logger.Info(string.Format("Couldn't load {0}: {1}", filePath, ex.Message));
                result = false;
            }
            return result;
        }

        protected void ThrowExceptionIfNotPlugin(Assembly assembly)
        {
            var ourName = Assembly.GetExecutingAssembly().GetName().Name;
            if (!assembly.GetReferencedAssemblies().Any(n => n.Name == ourName))
            {
                throw new
                    ModuleDoesNotReferenceFreeswitchManagedDllException("Assembly {0} doesn't reference FreeSWITCH.Managed, not loading.");
            }
        }
    }
}

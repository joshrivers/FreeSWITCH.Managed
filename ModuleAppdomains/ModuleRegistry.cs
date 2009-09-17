using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class ModuleRegistry
    {
        public void Register(ModuleServiceLocator registry)
        {
            registry.Register<AssemblyComposerDictionary>(container =>
            {
                var dictionary = new AssemblyComposerDictionary();
                Func<IAssemblyComposer> factory1 = () => { return container.Create<DllAssemblyComposer>(); };
                Func<IAssemblyComposer> factory2 = () => { return container.Create<ScriptAssemblyComposer>(); };
                dictionary.Add(".dll", factory1);
                dictionary.Add(".exe", factory1);
                dictionary.Add(".fsx", factory2);
                dictionary.Add(".csx", factory2);
                dictionary.Add(".vbx", factory2);
                dictionary.Add(".jsx", factory2);
                return dictionary;
            });
            registry.Register<IAssemblyCompiler>(container => { return container.Create<AssemblyCompiler>(); });
            registry.DeclareSingleton(typeof(PluginHandlerOrchestrator));
            registry.DeclareSingleton(typeof(LogDirector));
            registry.RegisterSingleton<ILogger>(container => { return new LogDirector(); });
            //registry.RegisterSingleton<PluginHandlerOrchestrator>(container => { return new PluginHandlerOrchestrator(); });
            registry.Register<ModuleAssemblyLoader>(container =>
                {
                    return new ModuleAssemblyLoader(container.Create<ILogger>(),
                       container.Create<AssemblyComposerDictionary>(),
                       container.Create<PluginHandlerOrchestrator>());
                });
            registry.Register<List<IPluginHandler>>(container =>
                {
                    var list = new List<IPluginHandler>();
                    list.Add(container.Create<RunNotifyPluginHandler>());
                    list.Add(container.Create<DefaultPluginHandler>());
                    return list;
                });

        }
    }
}

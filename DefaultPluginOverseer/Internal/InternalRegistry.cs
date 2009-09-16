using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class InternalRegistry
    {
        public void Register(InternalAppdomainServiceLocator registry)
        {
            registry.Register<AssemblyComposerFactoryDictionary>(container =>
            {
                var dictionary = new AssemblyComposerFactoryDictionary();
                var factory1 = new DllComposerFactory();
                var factory2 = new ScriptComposerFactory();
                dictionary.Add(".dll", factory1);
                dictionary.Add(".exe", factory1);
                dictionary.Add(".fsx", factory2);
                dictionary.Add(".csx", factory2);
                dictionary.Add(".vbx", factory2);
                dictionary.Add(".jsx", factory2);
                return dictionary;
            });

            registry.Register<PluginHandlerOrchestrator>(container =>
                { return InternalAppdomainServiceLocator.PluginHandlerOrchestrator; });
            registry.Register<ILogger>(container =>
                { return InternalAppdomainServiceLocator.LogSender; });
            registry.Register<ModuleAssemblyLoader>(container =>
                {
                    return new ModuleAssemblyLoader(container.Create<ILogger>(),
                       container.Create<AssemblyComposerFactoryDictionary>(),
                       container.Create<PluginHandlerOrchestrator>());
                });
            registry.Register<List<IPluginHandler>>(container =>
                {
                    var list = new List<IPluginHandler>();
                    list.Add(new DefaultPluginHandler(container.Create<PluginHandlerOrchestrator>()));
                    list.Add(new RunNotifyPluginHandler());
                    return list;
                });

        }
    }
}

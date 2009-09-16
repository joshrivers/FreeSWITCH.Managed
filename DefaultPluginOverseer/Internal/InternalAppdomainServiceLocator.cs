using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class InternalAppdomainServiceLocator : ServiceLocator
    {
        public static PluginHandlerOrchestrator PluginHandlerOrchestrator { get; private set; }
        public static LogDirector LogReciever { get; private set; }
        public static ILogger LogSender { get; private set; }
        public static InternalAppdomainServiceLocator Container { get; private set; }
        public static string MasterAssemblyPath { get; set; }
        static InternalAppdomainServiceLocator()
        {
            InternalAppdomainServiceLocator.LogReciever = new LogDirector();
            InternalAppdomainServiceLocator.LogSender = new LogDirector();
            InternalAppdomainServiceLocator.PluginHandlerOrchestrator = new PluginHandlerOrchestrator();
            InternalAppdomainServiceLocator.Container = new InternalAppdomainServiceLocator();
            var registry = new InternalRegistry();
            registry.Register(InternalAppdomainServiceLocator.Container);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests.TestHelpers
{
    public static class ModuleServiceLocatorSetup
    {
        public static void ContainerReset()
        {
            ModuleServiceLocator.Container.Reset();
            ModuleRegistry registry = new ModuleRegistry();
            registry.Register(ModuleServiceLocator.Container);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests.TestHelpers
{
    public static class DefaultServiceLocatorSetup
    {
        public static void ContainerReset()
        {
            DefaultServiceLocator.Container.Reset();
            DefaultRegistry registry = new DefaultRegistry();
            registry.Register(DefaultServiceLocator.Container);
        }
    }
}

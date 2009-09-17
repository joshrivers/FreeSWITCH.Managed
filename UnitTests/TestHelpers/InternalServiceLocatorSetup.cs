using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests.TestHelpers
{
    public static class InternalServiceLocatorSetup
    {
        public static void ContainerReset()
        {
            InternalAppdomainServiceLocator.Container.Reset();
            InternalRegistry registry = new InternalRegistry();
            registry.Register(InternalAppdomainServiceLocator.Container);
        }
    }
}

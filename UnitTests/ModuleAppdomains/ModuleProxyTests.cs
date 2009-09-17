using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains
{
    [TestFixture]
    public class ModuleProxyTests
    {
        [SetUp]
        public void Setup()
        {
            TestHelpers.InternalServiceLocatorSetup.ContainerReset();
        }

        [TearDown]
        public void TearDown()
        {
            TestHelpers.InternalServiceLocatorSetup.ContainerReset();
        }

        [Test]
        public void ClassHasDefaultConstructor()
        {
            var createdObject = new ModuleProxy();
            Assert.IsInstanceOf<ModuleProxy>(createdObject);
        }

        [Test]
        public void Logger_Get_ReturnsLoggerFromContainer()
        {
            var createdObject = new ModuleProxy();
            var logger = InternalAppdomainServiceLocator.Container.Create<ILogger>();
            Assert.AreSame(createdObject.Logger, logger);
        }

        [Test]
        public void Logger_Get_ReturnsLogDirectorFromContainer()
        {
            var createdObject = new ModuleProxy();
            var logger = InternalAppdomainServiceLocator.Container.Create<LogDirector>();
            Assert.AreSame(createdObject.LogDirector, logger);
        }
    }
}

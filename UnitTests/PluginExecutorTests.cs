using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using FreeSWITCH;

namespace FreeSWITCH.Managed.Tests
{
    [TestFixture]
    public class PluginExecutorTests
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
        private class TestPluginExecutorImplementation : PluginExecutor
        {
            public TestPluginExecutorImplementation(string name, List<string> aliases, PluginOptions pluginOptions)
                : base(name, aliases, pluginOptions)
            {
            }
        }
        [Test]
        public void Constructor_Requires_ILogger()
        {
            var logger = new Mock<ILogger>();
            bool called = false;
            InternalAppdomainServiceLocator.Container.Register<ILogger>(c =>
            {
                called = true; return logger.Object;
            });
            var objectUnderTest = new TestPluginExecutorImplementation("Name", new List<string>{"One"}, new PluginOptions());
            Assert.IsTrue(called);
        }
    }
}

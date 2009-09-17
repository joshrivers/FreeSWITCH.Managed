using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using FreeSWITCH;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains.PluginTypes
{
    [TestFixture]
    public class DefaultPluginExecutorTests
    {
        [SetUp]
        public void Setup()
        {
            TestHelpers.ModuleServiceLocatorSetup.ContainerReset();
        }
        [TearDown]
        public void TearDown()
        {
            TestHelpers.ModuleServiceLocatorSetup.ContainerReset();
        }
        private class TestPluginExecutorImplementation : DefaultPluginExecutor
        {
            public TestPluginExecutorImplementation(string name, List<string> aliases, PluginOptions pluginOptions)
                : base(name, aliases, pluginOptions)
            {
            }
        }
        [Test]
        public void Constructor_Requires_ILogger()
        {
            ModuleServiceLocator.Container.Reset();
            var logger = new Mock<ILogger>();
            bool called = false;
            ModuleServiceLocator.Container.Register<ILogger>(c =>
            {
                called = true; return logger.Object;
            });
            var objectUnderTest = new TestPluginExecutorImplementation("Name", new List<string>{"One"}, new PluginOptions());
            Assert.IsTrue(called);
        }
    }
}

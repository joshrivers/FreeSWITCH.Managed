using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains
{
    [TestFixture]
    public class ModuleServiceLocatorTests
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

        private void ConfirmSingletonDeclaration<T>()
        {
            var createdObject1 = ModuleServiceLocator.Container.Create<T>();
            var createdObject2 = ModuleServiceLocator.Container.Create<T>();
            Assert.IsInstanceOf<T>(createdObject1);
            Assert.AreSame(createdObject1, createdObject2);
        }

        [Test]
        public void LocatorResolvesIDirectoryController()
        {
            var createdObject = ModuleServiceLocator.Container.Create<IAssemblyCompiler>();
            Assert.IsInstanceOf<AssemblyCompiler>(createdObject);
        }
        [Test]
        public void LocatorResolvesAssemblyComposerDictionary()
        {
            var createdObject = ModuleServiceLocator.Container.Create<AssemblyComposerDictionary>();
            Assert.IsInstanceOf<AssemblyComposerDictionary>(createdObject);
            Assert.IsInstanceOf<Func<IAssemblyComposer>>(createdObject[".dll"]);
            Assert.IsInstanceOf<Func<IAssemblyComposer>>(createdObject[".exe"]);
            Assert.IsInstanceOf<Func<IAssemblyComposer>>(createdObject[".fsx"]);
            Assert.IsInstanceOf<Func<IAssemblyComposer>>(createdObject[".csx"]);
            Assert.IsInstanceOf<Func<IAssemblyComposer>>(createdObject[".vbx"]);
            Assert.IsInstanceOf<Func<IAssemblyComposer>>(createdObject[".jsx"]);
        }

        [Test]
        public void LocatorResolvesILogger()
        {
            ConfirmSingletonDeclaration<ILogger>();
        }

        [Test]
        public void LocatorResolvesLogDirector()
        {
            ConfirmSingletonDeclaration<LogDirector>();
        }

        [Test]
        public void LocatorResolvesPluginHandlerOrchestrator()
        {
            var createdObject1 = ModuleServiceLocator.Container.Create<IPluginHandlerOrchestrator>();
            var createdObject2 = ModuleServiceLocator.Container.Create<IPluginHandlerOrchestrator>();
            Assert.IsInstanceOf<PluginHandlerOrchestrator>(createdObject1);
            Assert.AreSame(createdObject1, createdObject2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests.PrimaryAppdomain
{
    [TestFixture]
    public class DefaultServiceLocatorTests
    {
        [SetUp]
        public void Setup()
        {
            TestHelpers.DefaultServiceLocatorSetup.ContainerReset();
        }
        [TearDown]
        public void TearDown()
        {
            TestHelpers.DefaultServiceLocatorSetup.ContainerReset();
        }
        [Test]
        public void LocatorResolvesIDirectoryController()
        {
            var createdObject = DefaultServiceLocator.Container.Create<IDirectoryController>();
            Assert.IsInstanceOf<DefaultDirectoryController>(createdObject);
        }

        [Test]
        public void LocatorResolvesLogger()
        {
            var createdObject = DefaultServiceLocator.Container.Create<ILogger>();
            var createdObject2 = DefaultServiceLocator.Container.Create<LogDirector>();
            Assert.IsInstanceOf<LogDirector>(createdObject);
            Assert.AreSame(createdObject, createdObject2);
        }

        [Test]
        public void LocatorResolvesIModuleLoader()
        {
            var createdObject = DefaultServiceLocator.Container.Create<IModuleLoader>();
            Assert.IsInstanceOf<DefaultModuleLoader>(createdObject);
        }

        private void ConfirmSingletonDeclaration<T>()
        {
            var createdObject1 = DefaultServiceLocator.Container.Create<T>();
            var createdObject2 = DefaultServiceLocator.Container.Create<T>();
            Assert.IsInstanceOf<T>(createdObject1);
            Assert.AreSame(createdObject1, createdObject2);
        }

        [Test]
        public void LocatorResolvesRunCommandDirector()
        {
            ConfirmSingletonDeclaration<RunCommandOnCollection>();
        }

        [Test]
        public void LocatorResolvesExecuteCommandDirector()
        {
            ConfirmSingletonDeclaration<ExecuteCommandOnCollection>();
        }

        [Test]
        public void LocatorResolvesExecuteBackgroundCommandDirector()
        {
            ConfirmSingletonDeclaration<ExecuteBackgroundCommandOnCollection>();
        }

        [Test]
        public void LocatorResolvesReloadCommandDirector()
        {
            ConfirmSingletonDeclaration<ReloadCommandOnCollection>();
        }

        [Test]
        public void LocatorResolvesDefaultPluginOverseer()
        {
            ConfirmSingletonDeclaration<DefaultPluginOverseer>();
        }

        [Test]
        public void LocatorResolvesDefaultPluginDirectoryWatcher()
        {
            ConfirmSingletonDeclaration<DefaultModuleDirectorySupervisor>();
        }

        [Test]
        public void LocatorResolvesModuleList()
        {
            ConfirmSingletonDeclaration<ModuleList>();
        }

        [Test]
        public void LocatorResolvesAssemblyResolver()
        {
            ConfirmSingletonDeclaration<AssemblyResolver>();
        }
    }
}

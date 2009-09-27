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
            var createdObject2 = DefaultServiceLocator.Container.Create<LoggerContainer>();
            var createdObject3 = DefaultServiceLocator.Container.Create<ILoggerContainer>();
            Assert.IsInstanceOf<LoggerContainer>(createdObject);
            Assert.AreSame(createdObject, createdObject2);
            Assert.AreSame(createdObject, createdObject3);
        }

        [Test]
        public void LocatorResolvesIModuleLoader()
        {
            var createdObject = DefaultServiceLocator.Container.Create<IModuleLoader>();
            Assert.IsInstanceOf<SelectiveModuleLoader>(createdObject);
        }

        [Test]
        public void LocatorResolvesModuleProxyTypeDictionary()
        {
            var createdObject = DefaultServiceLocator.Container.Create<ModuleProxyTypeDictionary>();
            Assert.IsInstanceOf<ModuleProxyTypeDictionary>(createdObject);
        }

        [Test]
        public void LocatorResolvesIAppDomainFactory()
        {
            var createdObject = DefaultServiceLocator.Container.Create<IAppDomainFactory>();
            Assert.IsInstanceOf<DefaultAppDomainFactory>(createdObject);
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

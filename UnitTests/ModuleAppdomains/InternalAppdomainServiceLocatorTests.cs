using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains
{
    [TestFixture]
    public class InternalAppdomainServiceLocatorTests
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

        private void ConfirmSingletonDeclaration<T>()
        {
            var createdObject1 = DefaultServiceLocator.Container.Create<T>();
            var createdObject2 = DefaultServiceLocator.Container.Create<T>();
            Assert.IsInstanceOf<T>(createdObject1);
            Assert.AreSame(createdObject1, createdObject2);
        }

        [Test]
        public void LocatorResolvesIDirectoryController()
        {
            var createdObject = InternalAppdomainServiceLocator.Container.Create<IAssemblyCompiler>();
            Assert.IsInstanceOf<AssemblyCompiler>(createdObject);
        }
        [Test]
        public void LocatorResolvesAssemblyComposerDictionary()
        {
            var createdObject = InternalAppdomainServiceLocator.Container.Create<AssemblyComposerDictionary>();
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
    }        
}

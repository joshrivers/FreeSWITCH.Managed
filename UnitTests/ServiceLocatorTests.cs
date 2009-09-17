using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        [Test]
        public void CreateSimpleObject()
        {
            ObjectContainer locator = new ObjectContainer();
            var result = locator.Create<object>();
            Assert.IsAssignableFrom<object>(result);
        }

        [Test]
        public void CreateComplexObject()
        {
            ObjectContainer locator = new ObjectContainer();
            var dc = new Mock<IDirectoryController>();
            var log = new Mock<ILogger>(); 
            var ml = new Mock<IModuleLoader>();
            locator.Register<IDirectoryController>(c => { return dc.Object; });
            locator.Register<ILogger>(c => { return log.Object; });
            locator.Register<IModuleLoader>(c => { return ml.Object; });
            var result = locator.Create<DefaultPluginOverseer>();
            Assert.IsAssignableFrom<DefaultPluginOverseer>(result);
        }

        [Test]
        public void CreateSingletonObject()
        {
            ObjectContainer locator = new ObjectContainer();
            locator.DeclareSingleton(typeof(object));
            var result = locator.Create<object>();
            Assert.IsAssignableFrom<object>(result);
            var result2 = locator.Create<object>();
            Assert.AreSame(result, result2);
        }


        [Test]
        public void RegisterSingletonObject()
        {
            ObjectContainer locator = new ObjectContainer();
            locator.RegisterSingleton<object>(c => { return new object(); });
            var result = locator.Create<object>();
            Assert.IsAssignableFrom<object>(result);
            var result2 = locator.Create<object>();
            Assert.AreSame(result, result2);
        }
    }
}

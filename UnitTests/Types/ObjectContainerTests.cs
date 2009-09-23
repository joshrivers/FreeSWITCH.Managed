using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.Types
{
    [TestFixture]
    public class ObjectContainerTests
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
            var log = new Mock<ILoggerContainer>();
            var log1 = new Mock<ILogger>();
            var ml = new Mock<IModuleLoader>();
            locator.Register<IDirectoryController>(c => { return dc.Object; });
            locator.Register<ILoggerContainer>(c => { return log.Object; });
            locator.Register<ILogger>(c => { return log1.Object; });
            locator.Register<IModuleLoader>(c => { return ml.Object; });
            var result = locator.Create<DefaultLoader>();
            Assert.IsAssignableFrom<DefaultLoader>(result);
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

        [Test]
        public void ToString_NoParameters_ReturnsDescription()
        {
            ObjectContainer locator = new ObjectContainer();
            locator.RegisterSingleton<object>(c => { return new object(); });
            var log = new Mock<ILogger>();
            locator.Register<ILogger>(c => { return log.Object; });
            locator.Configuration["Object"]=new object();
            var result = locator.ToString();
            Assert.IsInstanceOf<string>(result);
            //Console.WriteLine(result);
        }

        [Test]
        public void RegisterReplacesPreviousRegistration
            ()
        {
            ObjectContainer locator = new ObjectContainer();
            var log = new Mock<ILogger>();
            locator.Register<ILogger>(c => { return log.Object; });
            var log2 = new Mock<ILogger>();
            locator.Register<ILogger>(c => { return log2.Object; });
            var result = locator.Create<ILogger>();
            Assert.AreSame(result,log2.Object);
        }
    }
}

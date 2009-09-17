using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains.PluginHandlers
{
    [TestFixture]
    public class DefaultPluginHandlerTests
    {
        [Test]
        public void ConstructorRequiresILogger()
        {
            var logger = new Mock<ILogger>();
            var objectUnderTest = new DefaultPluginHandler(logger.Object);
            Assert.IsNotNull(objectUnderTest);
        }
    }
}

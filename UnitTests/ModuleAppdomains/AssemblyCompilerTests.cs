using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using FreeSWITCH;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains
{
    [TestFixture]
    public class AssemblyCompilerTests
    {
        [Test]
        public void Constructor_Requires_ILogger()
        {
            var logger = new Mock<ILogger>();
            var objectUnderTest = new AssemblyCompiler(logger.Object);
            Assert.IsNotNull(objectUnderTest);
        }
    }
}

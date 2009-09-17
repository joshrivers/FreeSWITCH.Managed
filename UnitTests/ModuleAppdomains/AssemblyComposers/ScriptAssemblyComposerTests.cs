using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains.AssemblyComposers
{
    [TestFixture]
    public class ScriptAssemblyComposerTests
    {
        [Test]
        public void Constructor_Requires_IAssemblyCompiler()
        {
            var logger = new Mock<IAssemblyCompiler>();
            var objectUnderTest = new ScriptAssemblyComposer(logger.Object);
            Assert.IsNotNull(objectUnderTest);
        }
    }
}

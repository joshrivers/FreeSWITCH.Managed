using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains
{
    [TestFixture]
    public class ModuleProxyTests
    {
        [Test]
        public void ClassHasDefaultConstructor()
        {
            var createdObject = new ModuleProxy();
            Assert.IsInstanceOf<ModuleProxy>(createdObject);
        }
    }
}

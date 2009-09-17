using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.ModuleAppdomains.PluginHandlers
{
    [TestFixture]
    public class RunNotifyPluginHandlerTests
    {
        [Test]
        public void NoReload_Get_ReturnsFalse()
        {
            var createdObject = new RunNotifyPluginHandler();
            Assert.IsFalse(createdObject.NoReload);
        }
    }
}

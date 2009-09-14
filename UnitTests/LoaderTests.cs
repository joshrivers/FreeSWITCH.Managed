using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests
{
    [TestFixture]
    public class LoaderTests
    {
        [Test]
        public void LoadMethodExists()
        {
            // This is a "compilation test". It is necessary that this method and class exist.
            // It is hard to test pass the pinvoke boundary.
            try
            {
                Loader.Load();
            }
            catch (DllNotFoundException)
            {
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests
{
    [TestFixture]
    public class DefaultLoaderTests
    {
        [SetUp]
        public void Setup()
        {
            CoreDelegates.Run = null;
            CoreDelegates.Execute = null;
            CoreDelegates.ExecuteBackground = null;
            CoreDelegates.Reload = null;
        }
        
        [Test]
        public void SingletonFactory()
        {
            var firstLoader = DefaultLoader.Loader;
            var secondLoader = DefaultLoader.Loader;
            Assert.AreEqual(firstLoader, secondLoader);
        }

        [Test]
        public void Load_Empty_ReturnsTrue()
        {
            Assert.IsTrue(DefaultLoader.Loader.Load());
        }

        [Test]
        public void Load_Empty_SetsRunDelegateToRunCommand()
        {
            DefaultLoader.Loader.Load();
            Assert.IsNotNull(CoreDelegates.Run);
        }

        [Test]
        public void Load_Empty_SetsRunDelegateToExecuteCommand()
        {
            DefaultLoader.Loader.Load();
            Assert.IsNotNull(CoreDelegates.Execute);
        }

        [Test]
        public void Load_Empty_SetsRunDelegateToExecuteBackgroundCommand()
        {
            DefaultLoader.Loader.Load();
            Assert.IsNotNull(CoreDelegates.ExecuteBackground);
        }

        [Test]
        public void Load_Empty_SetsRunDelegateToReloadCommand()
        {
            DefaultLoader.Loader.Load();
            Assert.IsNotNull(CoreDelegates.Reload);
        }

    }
}

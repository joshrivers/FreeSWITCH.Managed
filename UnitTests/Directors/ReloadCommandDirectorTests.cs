using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.Directors
{
    [TestFixture]
    public class ReloadCommandDirectorTests
    {
        [Test]
        public void ClassContainsCommandsCollection()
        {
            var logdirector = new Mock<ILogger>();
            var objectUnderTest = new ReloadCommandDirector(logdirector.Object);
            Assert.IsNotNull(objectUnderTest.Commands);
        }

        [Test]
        public void Reload_Empty_ReturnsTrue()
        {
            var logdirector = new Mock<ILogger>();
            var objectUnderTest = new ReloadCommandDirector(logdirector.Object);
            Assert.IsTrue(objectUnderTest.Reload(string.Empty));
        }

        [Test]
        public void Reload_FalseReturningCommand_ReturnsFalse()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand = new Mock<IReloadCommand>();
            runcommand.Setup(foo => foo.Reload(string.Empty)).Returns(false);

            var objectUnderTest = new ReloadCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);

            Assert.IsFalse(objectUnderTest.Reload(string.Empty));
        }

        [Test]
        public void Reload_ExceptionThrowingCommand_ReturnsFalse()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand = new Mock<IReloadCommand>();
            runcommand.Setup(foo => foo.Reload(string.Empty)).Throws(new Exception("Test Exception"));

            var objectUnderTest = new ReloadCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);

            Assert.IsFalse(objectUnderTest.Reload(string.Empty));
        }

        [Test]
        public void Reload_ExceptionThrowingCommand_LogsError()
        {
            var logdirector = new Mock<ILogger>();
            logdirector.Setup(foo => foo.Error(It.IsRegex("Error reloading")));
            var runcommand = new Mock<IReloadCommand>();
            runcommand.Setup(foo => foo.Reload(string.Empty)).Throws(new Exception("Test Exception"));

            var objectUnderTest = new ReloadCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);
            objectUnderTest.Reload(string.Empty);

            logdirector.VerifyAll();
        }

        [Test]
        public void Reload_MultipleCommands_RunsAll()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand1 = new Mock<IReloadCommand>();
            runcommand1.Setup(foo => foo.Reload(string.Empty)).Returns(true);
            var runcommand2 = new Mock<IReloadCommand>();
            runcommand2.Setup(foo => foo.Reload(string.Empty)).Returns(true);

            var objectUnderTest = new ReloadCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand1.Object);
            objectUnderTest.Commands.Add(runcommand2.Object);
            objectUnderTest.Reload(string.Empty);

            runcommand1.VerifyAll();
            runcommand2.VerifyAll();
        }
    }
}

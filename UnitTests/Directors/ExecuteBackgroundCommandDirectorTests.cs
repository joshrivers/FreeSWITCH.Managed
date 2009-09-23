using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.Directors
{
    [TestFixture]
    public class ExecuteBackgroundCommandDirectorTests
    {
        [Test]
        public void ClassContainsCommandsCollection()
        {
            var logdirector = new Mock<ILogger>();
            var objectUnderTest = new ExecuteBackgroundCommandOnCollection(logdirector.Object);
            Assert.IsNotNull(objectUnderTest.Commands);
        }

        [Test]
        public void ExecuteBackground_Empty_ReturnsTrue()
        {
            var logdirector = new Mock<ILogger>();
            var objectUnderTest = new ExecuteBackgroundCommandOnCollection(logdirector.Object);
            Assert.IsTrue(objectUnderTest.ExecuteBackground(string.Empty));
        }

        [Test]
        public void ExecuteBackground_FalseReturningCommand_ReturnsFalse()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand = new Mock<IExecuteBackgroundCommand>();
            runcommand.Setup(foo => foo.ExecuteBackground(string.Empty)).Returns(false);

            var objectUnderTest = new ExecuteBackgroundCommandOnCollection(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);

            Assert.IsFalse(objectUnderTest.ExecuteBackground(string.Empty));
        }

        [Test]
        public void ExecuteBackground_ExceptionThrowingCommand_ReturnsFalse()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand = new Mock<IExecuteBackgroundCommand>();
            runcommand.Setup(foo => foo.ExecuteBackground(string.Empty)).Throws(new Exception("Test Exception"));

            var objectUnderTest = new ExecuteBackgroundCommandOnCollection(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);

            Assert.IsFalse(objectUnderTest.ExecuteBackground(string.Empty));
        }

        [Test]
        public void ExecuteBackground_ExceptionThrowingCommand_LogsError()
        {
            var logdirector = new Mock<ILogger>();
            logdirector.Setup(foo => foo.Error(It.IsRegex("Exception in ExecuteBackground")));
            var runcommand = new Mock<IExecuteBackgroundCommand>();
            runcommand.Setup(foo => foo.ExecuteBackground(string.Empty)).Throws(new Exception("Test Exception"));

            var objectUnderTest = new ExecuteBackgroundCommandOnCollection(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);
            objectUnderTest.ExecuteBackground(string.Empty);

            logdirector.VerifyAll();
        }

        [Test]
        public void ExecuteBackground_MultipleCommands_RunsAll()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand1 = new Mock<IExecuteBackgroundCommand>();
            runcommand1.Setup(foo => foo.ExecuteBackground(string.Empty)).Returns(true);
            var runcommand2 = new Mock<IExecuteBackgroundCommand>();
            runcommand2.Setup(foo => foo.ExecuteBackground(string.Empty)).Returns(true);

            var objectUnderTest = new ExecuteBackgroundCommandOnCollection(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand1.Object);
            objectUnderTest.Commands.Add(runcommand2.Object);
            objectUnderTest.ExecuteBackground(string.Empty);

            runcommand1.VerifyAll();
            runcommand2.VerifyAll();
        }
    }
}

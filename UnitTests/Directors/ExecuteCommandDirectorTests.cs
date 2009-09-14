using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.Directors
{
    [TestFixture]
    public class ExecuteCommandDirectorTests
    {
        [Test]
        public void ClassContainsCommandsCollection()
        {
            var logdirector = new Mock<ILogger>();
            var objectUnderTest = new ExecuteCommandDirector(logdirector.Object);
            Assert.IsNotNull(objectUnderTest.Commands);
        }

        [Test]
        public void Execute_Empty_ReturnsTrue()
        {
            var logdirector = new Mock<ILogger>();
            var objectUnderTest = new ExecuteCommandDirector(logdirector.Object);
            Assert.IsTrue(objectUnderTest.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero));
        }

        [Test]
        public void Execute_FalseReturningCommand_ReturnsFalse()
        {
            var logdirector = new Mock<ILogger>();
            var executecommand = new Mock<IExecuteCommand>();
            executecommand.Setup(foo => foo.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero)).Returns(false);

            var objectUnderTest = new ExecuteCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(executecommand.Object);

            Assert.IsFalse(objectUnderTest.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero));
        }

        [Test]
        public void Execute_ExceptionThrowingCommand_ReturnsFalse()
        {
            var logdirector = new Mock<ILogger>();
            var executecommand = new Mock<IExecuteCommand>();
            executecommand.Setup(foo => foo.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero)).Throws(new Exception("Test Exception"));

            var objectUnderTest = new ExecuteCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(executecommand.Object);

            Assert.IsFalse(objectUnderTest.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero));
        }

        [Test]
        public void Execute_ExceptionThrowingCommand_LogsError()
        {
            var logdirector = new Mock<ILogger>();
            logdirector.Setup(foo => foo.Error(It.IsRegex("Exception in Execute")));
            var executecommand = new Mock<IExecuteCommand>();
            executecommand.Setup(foo => foo.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero)).Throws(new Exception("Test Exception"));

            var objectUnderTest = new ExecuteCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(executecommand.Object);
            objectUnderTest.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero);

            logdirector.VerifyAll();
        }

        [Test]
        public void Execute_MultipleCommands_RunsAll()
        {
            var logdirector = new Mock<ILogger>();
            var executecommand1 = new Mock<IExecuteCommand>();
            executecommand1.Setup(foo => foo.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero)).Returns(true);
            var executecommand2 = new Mock<IExecuteCommand>();
            executecommand2.Setup(foo => foo.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero)).Returns(true);

            var objectUnderTest = new ExecuteCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(executecommand1.Object);
            objectUnderTest.Commands.Add(executecommand2.Object);

            Assert.IsTrue(objectUnderTest.Execute(string.Empty, IntPtr.Zero, IntPtr.Zero));
            executecommand1.VerifyAll();
            executecommand2.VerifyAll();
        }
    }
}

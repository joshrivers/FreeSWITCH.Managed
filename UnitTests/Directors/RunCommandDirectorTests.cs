using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.Directors
{
    [TestFixture]
    public class RunCommandDirectorTests
    {
        [Test]
        public void ClassContainsCommandsCollection()
        {
            var logdirector = new Mock<ILogger>();
            var objectUnderTest = new RunCommandDirector(logdirector.Object);
            Assert.IsNotNull(objectUnderTest.Commands);
        }

        [Test]
        public void Run_Empty_ReturnsTrue()
        {
            var logdirector = new Mock<ILogger>();
            var objectUnderTest = new RunCommandDirector(logdirector.Object);
            Assert.IsTrue(objectUnderTest.Run(string.Empty,IntPtr.Zero));
        }

        [Test]
        public void Run_FalseReturningCommand_ReturnsFalse()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand = new Mock<IRunCommand>();
            runcommand.Setup(foo => foo.Run(string.Empty, IntPtr.Zero)).Returns(false);

            var objectUnderTest = new RunCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);

            Assert.IsFalse(objectUnderTest.Run(string.Empty, IntPtr.Zero));
        }

        [Test]
        public void Run_ExceptionThrowingCommand_ReturnsFalse()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand = new Mock<IRunCommand>();
            runcommand.Setup(foo => foo.Run(string.Empty, IntPtr.Zero)).Throws(new Exception("Test Exception"));

            var objectUnderTest = new RunCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);

            Assert.IsFalse(objectUnderTest.Run(string.Empty, IntPtr.Zero));
        }

        [Test]
        public void Run_ExceptionThrowingCommand_LogsError()
        {
            var logdirector = new Mock<ILogger>();
            logdirector.Setup(foo => foo.Error(It.IsRegex("Exception in Run")));
            var runcommand = new Mock<IRunCommand>();
            runcommand.Setup(foo => foo.Run(string.Empty, IntPtr.Zero)).Throws(new Exception("Test Exception"));

            var objectUnderTest = new RunCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand.Object);
            objectUnderTest.Run(string.Empty, IntPtr.Zero);

            logdirector.VerifyAll();
        }

        [Test]
        public void Run_MultipleCommands_RunsAll()
        {
            var logdirector = new Mock<ILogger>();
            var runcommand1 = new Mock<IRunCommand>();
            runcommand1.Setup(foo => foo.Run(string.Empty, IntPtr.Zero)).Returns(true);
            var runcommand2 = new Mock<IRunCommand>();
            runcommand2.Setup(foo => foo.Run(string.Empty, IntPtr.Zero)).Returns(true);

            var objectUnderTest = new RunCommandDirector(logdirector.Object);
            objectUnderTest.Commands.Add(runcommand1.Object);
            objectUnderTest.Commands.Add(runcommand2.Object);
            objectUnderTest.Run(string.Empty, IntPtr.Zero);

            runcommand1.VerifyAll();
            runcommand2.VerifyAll();
        }
    }
}
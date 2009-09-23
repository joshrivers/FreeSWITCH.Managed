using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace FreeSWITCH.Managed.Tests.Directors
{
    [TestFixture]
    public class LogDirectorTests
    {
        [Test]
        public void ClassContainsCommandsCollection()
        {
            var objectUnderTest = new LoggerContainer();
            Assert.IsNotNull(objectUnderTest.Loggers);
        }

        [Test]
        public void Debug_Message_CallsLogger()
        {
            var subLogger = new Mock<ILogger>();
            subLogger.Setup(foo => foo.Debug("Test Message"));
            var objectUnderTest = new LoggerContainer();
            objectUnderTest.Loggers.Add(subLogger.Object);
            objectUnderTest.Debug("Test Message");
            subLogger.VerifyAll();
        }

        [Test]
        public void Alert_Message_CallsLogger()
        {
            var subLogger = new Mock<ILogger>();
            subLogger.Setup(foo => foo.Alert("Test Message"));
            var objectUnderTest = new LoggerContainer();
            objectUnderTest.Loggers.Add(subLogger.Object);
            objectUnderTest.Alert("Test Message");
            subLogger.VerifyAll();
        }


        [Test]
        public void Critical_Message_CallsLogger()
        {
            var subLogger = new Mock<ILogger>();
            subLogger.Setup(foo => foo.Critical("Test Message"));
            var objectUnderTest = new LoggerContainer();
            objectUnderTest.Loggers.Add(subLogger.Object);
            objectUnderTest.Critical("Test Message");
            subLogger.VerifyAll();
        }


        [Test]
        public void Error_Message_CallsLogger()
        {
            var subLogger = new Mock<ILogger>();
            subLogger.Setup(foo => foo.Error("Test Message"));
            var objectUnderTest = new LoggerContainer();
            objectUnderTest.Loggers.Add(subLogger.Object);
            objectUnderTest.Error("Test Message");
            subLogger.VerifyAll();
        }


        [Test]
        public void Notice_Message_CallsLogger()
        {
            var subLogger = new Mock<ILogger>();
            subLogger.Setup(foo => foo.Notice("Test Message"));
            var objectUnderTest = new LoggerContainer();
            objectUnderTest.Loggers.Add(subLogger.Object);
            objectUnderTest.Notice("Test Message");
            subLogger.VerifyAll();
        }


        [Test]
        public void Warning_Message_CallsLogger()
        {
            var subLogger = new Mock<ILogger>();
            subLogger.Setup(foo => foo.Warning("Test Message"));
            var objectUnderTest = new LoggerContainer();
            objectUnderTest.Loggers.Add(subLogger.Object);
            objectUnderTest.Warning("Test Message");
            subLogger.VerifyAll();
        }

        [Test]
        public void Info_LoggerWithException_Continues()
        {
            var subLogger1 = new Mock<ILogger>();
            subLogger1.Setup(foo => foo.Info("Test Message"));
            var subLogger2 = new Mock<ILogger>();
            subLogger2.Setup(foo => foo.Info("Test Message")).Throws(new Exception());
            var subLogger3 = new Mock<ILogger>();
            subLogger3.Setup(foo => foo.Info("Test Message"));
            var objectUnderTest = new LoggerContainer();
            objectUnderTest.Loggers.Add(subLogger1.Object);
            objectUnderTest.Loggers.Add(subLogger2.Object);
            objectUnderTest.Loggers.Add(subLogger3.Object);
            objectUnderTest.Info("Test Message");
            subLogger1.VerifyAll();
            subLogger2.VerifyAll();
            subLogger3.VerifyAll();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FreeSWITCH.Managed.Tests
{
    public interface IConsoleWriter
    {
        void WriteLine(string line);
    }
    public class ConsoleWriter : MarshalByRefObject, IConsoleWriter
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
            throw new Exception(line);
        }
    }
    public interface IConsoleProxy
    {
        IConsoleWriter writer { get; set; }
    }
    public class ConsoleProxy : MarshalByRefObject, IConsoleProxy
    {
        public IConsoleWriter writer { get; set; }
        public ConsoleProxy()
        {
            this.writer = new ConsoleWriter();
        }
    }

    [TestFixture]
    public class AppdomainExplorationTests
    {
        [Test]
        [ExpectedException(ExpectedMessage="Smoke Test")]
        public void CreateAndUnwrapInstance()
        {
            AppDomainSetup appSetup = new AppDomainSetup();
            appSetup.ShadowCopyFiles = "true";
            appSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            appSetup.ActivationArguments = AppDomain.CurrentDomain.SetupInformation.ActivationArguments;
            Type writer = typeof(ConsoleWriter);
            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, appSetup);
            var instance = (ConsoleWriter)domain.CreateInstanceAndUnwrap(writer.Assembly.FullName,writer.FullName);
            instance.WriteLine("Smoke Test");
            AppDomain.Unload(domain);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Smoke Test")]
        public void CreateAndUnwrapInterface()
        {
            AppDomainSetup appSetup = new AppDomainSetup();
            appSetup.ShadowCopyFiles = "true";
            appSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            appSetup.ActivationArguments = AppDomain.CurrentDomain.SetupInformation.ActivationArguments;
            Type writer = typeof(ConsoleWriter);
            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, appSetup);
            var instance = (IConsoleWriter)domain.CreateInstanceAndUnwrap(writer.Assembly.FullName, writer.FullName);
            instance.WriteLine("Smoke Test");
            AppDomain.Unload(domain);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Smoke Test")]
        public void CreateAndUnwrapProxy()
        {
            AppDomainSetup appSetup = new AppDomainSetup();
            appSetup.ShadowCopyFiles = "true";
            appSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            appSetup.ActivationArguments = AppDomain.CurrentDomain.SetupInformation.ActivationArguments;
            Type writer = typeof(ConsoleProxy);
            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, appSetup);
            var instance = (ConsoleProxy)domain.CreateInstanceAndUnwrap(writer.Assembly.FullName, writer.FullName);
            instance.writer.WriteLine("Smoke Test");
            AppDomain.Unload(domain);
        }


        [Test]
        [ExpectedException(ExpectedMessage = "Smoke Test")]
        public void CreateAndUnwrapProxyInterface()
        {
            AppDomainSetup appSetup = new AppDomainSetup();
            appSetup.ShadowCopyFiles = "true";
            appSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            appSetup.ActivationArguments = AppDomain.CurrentDomain.SetupInformation.ActivationArguments;
            Type writer = typeof(ConsoleProxy);
            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, appSetup);
            var instance = (IConsoleProxy)domain.CreateInstanceAndUnwrap(writer.Assembly.FullName, writer.FullName);
            instance.writer.WriteLine("Smoke Test");
            AppDomain.Unload(domain);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Smoke Test")]
        public void CreateAndUnwrapProxyInterfaceWithLocalWriter()
        {
            AppDomainSetup appSetup = new AppDomainSetup();
            appSetup.ShadowCopyFiles = "true";
            appSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            appSetup.ActivationArguments = AppDomain.CurrentDomain.SetupInformation.ActivationArguments;
            Type writer = typeof(ConsoleProxy);
            AppDomain domain = AppDomain.CreateDomain("TestDomain", null, appSetup);
            var instance = (IConsoleProxy)domain.CreateInstanceAndUnwrap(writer.Assembly.FullName, writer.FullName);
            instance.writer = new ConsoleWriter();
            instance.writer.WriteLine("Smoke Test");
            AppDomain.Unload(domain);
        }
    }
}

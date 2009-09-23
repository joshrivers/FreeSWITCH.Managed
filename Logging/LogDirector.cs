using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class LogDirector : MarshalByRefObject, ILogger, ILogDirector
    {
        public SynchronizedList<ILogger> Loggers { get; private set; }

        public LogDirector()
        {
            this.Loggers = new SynchronizedList<ILogger>();
        }

        public void Add(ILogger logger)
        {
            this.Loggers.Add(logger);
        }
        
        public void Remove(ILogger logger)
        {
            this.Loggers.Remove(logger);
        }
        
        private void sendMessage(Action<ILogger> action)
        {
            foreach (var logger in Loggers.Items)
            {
                try
                {
                    action(logger);
                }
                catch { }
            }
        }
        public void Debug(string message)
        {
            this.sendMessage((logger) => logger.Debug(message));
        }

        public void Info(string message)
        {
            this.sendMessage((logger) => logger.Info(message));
        }

        public void Error(string message)
        {
            this.sendMessage((logger) => logger.Error(message));
        }

        public void Critical(string message)
        {
            this.sendMessage((logger) => logger.Critical(message));
        }

        public void Alert(string message)
        {
            this.sendMessage((logger) => logger.Alert(message));
        }

        public void Warning(string message)
        {
            this.sendMessage((logger) => logger.Warning(message));
        }

        public void Notice(string message)
        {
            this.sendMessage((logger) => logger.Notice(message));
        }
    }
}

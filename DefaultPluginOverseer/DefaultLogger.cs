using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class DefaultLogger : ILogger
    {
        public void Debug(string message)
        {
            Log.WriteLine(LogLevel.Debug, message);
        }

        public void Info(string message)
        {
            Log.WriteLine(LogLevel.Info, message);
        }

        public void Error(string message)
        {
            Log.WriteLine(LogLevel.Error, message);
        }

        public void Critical(string message)
        {
            Log.WriteLine(LogLevel.Critical, message);
        }

        public void Alert(string message)
        {
            Log.WriteLine(LogLevel.Alert, message);
        }

        public void Warning(string message)
        {
            Log.WriteLine(LogLevel.Warning, message);
        }

        public void Notice(string message)
        {
            Log.WriteLine(LogLevel.Notice, message);
        }
    }
}

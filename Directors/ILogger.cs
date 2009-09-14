using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Error(string message);
        void Critical(string message);
        void Alert(string message);
        void Warning(string message);
        void Notice(string message);
    }
}

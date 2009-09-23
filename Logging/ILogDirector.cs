using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public interface ILogDirector : ILogger
    {
        SynchronizedList<ILogger> Loggers { get; }
        void Add(ILogger logger);
        void Remove(ILogger logger);
    }
}

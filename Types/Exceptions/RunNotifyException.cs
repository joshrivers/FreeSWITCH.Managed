using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace FreeSWITCH
{
    public class RunNotifyException : Exception
    {
        public RunNotifyException()
        {

        }
        public RunNotifyException(string message)
            : base(message)
        {

        }
        public RunNotifyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        protected RunNotifyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}

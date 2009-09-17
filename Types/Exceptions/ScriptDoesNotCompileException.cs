using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace FreeSWITCH
{
    public class ScriptDoesNotCompileException : Exception
    {
        public ScriptDoesNotCompileException()
        {

        }
        public ScriptDoesNotCompileException(string message)
            : base(message)
        {

        }
        public ScriptDoesNotCompileException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        protected ScriptDoesNotCompileException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {

        }
    }
}

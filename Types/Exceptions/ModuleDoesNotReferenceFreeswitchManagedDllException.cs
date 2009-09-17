using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace FreeSWITCH
{
    public class ModuleDoesNotReferenceFreeswitchManagedDllException : Exception
    {
        public ModuleDoesNotReferenceFreeswitchManagedDllException()
        {
            
        }
        public ModuleDoesNotReferenceFreeswitchManagedDllException(string message)
            : base(message)
        {
            
        }
        public ModuleDoesNotReferenceFreeswitchManagedDllException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
        protected ModuleDoesNotReferenceFreeswitchManagedDllException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FreeSWITCH
{
    public static class FSUtil
    {
        // IntPtr cPtr, bool futureUse
        static readonly Type[] swigConstructorTypes = new[] { typeof(IntPtr), typeof(bool) };
        public static T CreateSwigTypePointer<T>(this IntPtr cPtr)
        {
            var ty = typeof(T);
            var bflags = BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var cons = ty.GetConstructor(bflags, null, swigConstructorTypes, null);
            if (cons == null) throw new ArgumentException(ty.Name + " constructor not found.");
            return (T)cons.Invoke(new object[] { cPtr, false });
        }
    }
}

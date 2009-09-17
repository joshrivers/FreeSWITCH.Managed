using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace FreeSWITCH
{
    public static class EntryPointExtensionMethods
    {
        public static void CallEntryPoint(this MethodInfo entryPoint)
        {
            try
            {
                var internalDelegate = entryPoint.GetEntryDelegate();
                internalDelegate();
            }
            catch { }
        }

        public static Action GetEntryDelegate(this MethodInfo entryPoint)
        {
            if (!entryPoint.IsPublic || !entryPoint.DeclaringType.IsPublic)
            {
                Log.WriteLine(LogLevel.Error, "Entry point: {0}.{1} is not public. This may cause errors with Mono.",
                    entryPoint.DeclaringType.FullName, entryPoint.Name);
            }
            var dm = new DynamicMethod(entryPoint.DeclaringType.Assembly.GetName().Name + "_entrypoint_" + entryPoint.DeclaringType.FullName + entryPoint.Name, null, null, true);
            var il = dm.GetILGenerator();
            var args = entryPoint.GetParameters();
            if (args.Length > 1) throw new ArgumentException("Cannot handle entry points with more than 1 parameter.");
            if (args.Length == 1)
            {
                if (args[0].ParameterType != typeof(string[])) throw new ArgumentException("Entry point paramter must be a string array.");
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Newarr, typeof(string));
            }
            il.EmitCall(OpCodes.Call, entryPoint, null);
            if (entryPoint.ReturnType != typeof(void))
            {
                il.Emit(OpCodes.Pop);
            }
            il.Emit(OpCodes.Ret);
            return (Action)dm.CreateDelegate(typeof(Action));
        }

    }
}

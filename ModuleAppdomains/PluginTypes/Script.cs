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
    public static class Script
    {

        [ThreadStatic]
        internal static ScriptContextType contextType;
        [ThreadStatic]
        internal static object context;

        public static ScriptContextType ContextType { get { return contextType; } }

        public static ApiContext GetApiContext()
        {
            return getContext<ApiContext>(ScriptContextType.Api);
        }
        public static ApiBackgroundContext GetApiBackgroundContext()
        {
            return getContext<ApiBackgroundContext>(ScriptContextType.ApiBackground);
        }
        public static AppContext GetAppContext()
        {
            return getContext<AppContext>(ScriptContextType.App);
        }

        public static T getContext<T>(ScriptContextType sct)
        {
            var ctx = context;
            if (ctx == null) throw new InvalidOperationException("Current context is null.");
            if (contextType != sct) throw new InvalidOperationException("Current ScriptContextType is not " + sct.ToString() + ".");
            return (T)ctx;
        }

    }
}

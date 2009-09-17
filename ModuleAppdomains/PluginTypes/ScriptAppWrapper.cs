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
    public class ScriptAppWrapper : IAppPlugin
    {

        readonly Action entryPoint;
        public ScriptAppWrapper(Action entryPoint)
        {
            this.entryPoint = entryPoint;
        }

        public void Run(AppContext context)
        {
            Script.contextType = ScriptContextType.App;
            Script.context = context;
            try
            {
                entryPoint();
            }
            finally
            {
                Script.contextType = ScriptContextType.None;
                Script.context = null;
            }
        }

    }
}

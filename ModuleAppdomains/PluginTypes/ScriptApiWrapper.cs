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
    public class ScriptApiWrapper : IApiPlugin
    {

        readonly Action entryPoint;
        public ScriptApiWrapper(Action entryPoint)
        {
            this.entryPoint = entryPoint;
        }

        public void Execute(ApiContext context)
        {
            Script.contextType = ScriptContextType.Api;
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

        public void ExecuteBackground(ApiBackgroundContext context)
        {
            Script.contextType = ScriptContextType.ApiBackground;
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

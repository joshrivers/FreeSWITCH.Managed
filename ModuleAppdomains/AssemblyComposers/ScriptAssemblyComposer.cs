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
    public class ScriptAssemblyComposer : IAssemblyComposer
    {
        private IAssemblyCompiler compiler;
        public ScriptAssemblyComposer(IAssemblyCompiler compiler)
        {
            this.compiler = compiler;
         
        }
        public Assembly GetAssembly(string filePath)
        {
            Assembly asm;
            if (Path.GetExtension(filePath).ToLowerInvariant() == ".exe")
            {
                asm = Assembly.LoadFrom(filePath);
            }
            else
            {
                asm = this.compiler.CompileAssembly(filePath);
            }
            return asm;
        }
    }
}

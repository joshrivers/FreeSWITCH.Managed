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
        public Assembly GetAssembly(string filePath)
        {
            Assembly asm;
            if (Path.GetExtension(filePath).ToLowerInvariant() == ".exe")
            {
                asm = Assembly.LoadFrom(filePath);
            }
            else
            {
                var compiler = new AssemblyCompiler();
                asm = compiler.CompileAssembly(filePath);
            }
            return asm;
        }
    }
}

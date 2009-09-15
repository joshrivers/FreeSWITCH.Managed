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
    public class DllAssemblyComposer : IAssemblyComposer
    {
        public Assembly GetAssembly(string filePath)
        {
            Assembly asm = Assembly.LoadFrom(filePath);
            return asm;
        }
    }
}

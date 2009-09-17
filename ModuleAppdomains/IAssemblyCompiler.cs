using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public interface IAssemblyCompiler
    {
        Assembly CompileAssembly(string fileName);
    }
}

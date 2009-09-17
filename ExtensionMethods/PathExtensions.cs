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
    public static class PathExtensions
    {
        public static string GetLoweredFileExtension(this string path)
        {
            return Path.GetExtension(path).ToLowerInvariant();
        }
    }
}

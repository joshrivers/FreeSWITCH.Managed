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
    public class AssemblyCompiler
    {
        public Assembly CompileAssembly(string fileName)
        {
            var fileExtension = fileName.GetLoweredFileExtension();
            CodeDomProvider codeDom = GetCodeDomProvider(fileExtension);
            CompilerParameters parameters = GetCompilerParameters(fileExtension);
            Log.WriteLine(LogLevel.Info, "Compiling {0}", fileName);
            var compilerResult = codeDom.CompileAssemblyFromFile(parameters, fileName);

            var errors = compilerResult.Errors.Cast<CompilerError>().Where(x => !x.IsWarning).ToList();
            if (errors.Count > 0)
            {
                LogErrors(fileName, errors);
                throw new ScriptDoesNotCompileException(fileName);
            }
            Log.WriteLine(LogLevel.Info, "File {0} compiled successfully.", fileName);
            return compilerResult.CompiledAssembly;
        }

        private static void LogErrors(string fileName, List<CompilerError> errors)
        {
            Log.WriteLine(LogLevel.Error, "There were {0} errors compiling {1}.", errors.Count, fileName);
            foreach (var err in errors)
            {
                if (string.IsNullOrEmpty(err.FileName))
                {
                    Log.WriteLine(LogLevel.Error, "{0}: {1}", err.ErrorNumber, err.ErrorText);
                }
                else
                {
                    Log.WriteLine(LogLevel.Error, "{0}: {1}:{2}:{3} {4}", err.ErrorNumber, err.FileName, err.Line, err.Column, err.ErrorText);
                }
            }
        }
        private static CodeDomProvider GetCodeDomProvider(string fileExtension)
        {
            CodeDomProvider cdp;
            switch (fileExtension)
            {
                case ".fsx":
                    cdp = CodeDomProvider.CreateProvider("f#");
                    break;
                case ".csx":
                    cdp = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });
                    break;
                case ".vbx":
                    cdp = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });
                    break;
                case ".jsx":
                    // Have to figure out better JS support
                    cdp = CodeDomProvider.CreateProvider("js");
                    break;
                default:
                    if (CodeDomProvider.IsDefinedExtension(fileExtension))
                    {
                        cdp = CodeDomProvider.CreateProvider(CodeDomProvider.GetLanguageFromExtension(fileExtension));
                    }
                    else
                    {
                        throw new ScriptDoesNotCompileException("Unknown extension.");
                    }
                    break;
            }
            return cdp;
        }

        private static CompilerParameters GetCompilerParameters(string fileExtension)
        {
            var comp = new CompilerParameters();
            var mainRefs = new List<string> {
				                Path.Combine(Native.freeswitch.SWITCH_GLOBAL_dirs.mod_dir, "FreeSWITCH.Managed.dll"),
				                "System.dll", "System.Xml.dll", "System.Data.dll"
				            };
            var extraRefs = new List<string> {
				                "System.Core.dll",
				                "System.Xml.Linq.dll",
				            };
            comp.ReferencedAssemblies.AddRange(mainRefs.ToArray());
            comp.ReferencedAssemblies.AddRange(extraRefs.ToArray());
            switch (fileExtension)
            {
                case ".fsx":
                    comp.ReferencedAssemblies.AddRange(mainRefs.ToArray());
                    comp.ReferencedAssemblies.AddRange(extraRefs.ToArray());
                    break;
                case ".csx":
                    comp.ReferencedAssemblies.AddRange(mainRefs.ToArray());
                    comp.ReferencedAssemblies.AddRange(extraRefs.ToArray());
                    break;
                case ".vbx":
                    comp.ReferencedAssemblies.AddRange(extraRefs.ToArray());
                    break;
                case ".jsx":
                    comp.ReferencedAssemblies.AddRange(mainRefs.ToArray());
                    break;
                default:
                    comp.ReferencedAssemblies.AddRange(mainRefs.ToArray());
                    comp.ReferencedAssemblies.AddRange(extraRefs.ToArray());
                    break;
            }
            comp.GenerateInMemory = true;
            comp.GenerateExecutable = true;
            return comp;
        }
    }
}

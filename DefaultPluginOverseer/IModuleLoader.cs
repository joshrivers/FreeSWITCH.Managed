using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public interface IModuleLoader
    {
        void LoadModule(string fileName);
    }
}

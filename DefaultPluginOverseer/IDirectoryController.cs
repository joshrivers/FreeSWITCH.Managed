using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FreeSWITCH.Managed
{
    public interface IDirectoryController
    {
        string PluginDirectoryPath { get; }
        string ShadowDirectoryPath { get; }
        bool PluginDirectoryExists();
        void ClearShadowDirecory();
    }
}

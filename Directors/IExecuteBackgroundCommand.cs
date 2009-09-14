using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FreeSWITCH.Managed
{
    public interface IExecuteBackgroundCommand
    {
        bool ExecuteBackground(string command);
    }
}

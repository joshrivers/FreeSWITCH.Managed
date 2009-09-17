using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeSWITCH
{
    public interface IAppPlugin
    {
        void Run(AppContext context);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeSWITCH
{
    public interface IApiPlugin
    {
        void Execute(ApiContext context);
        void ExecuteBackground(ApiBackgroundContext context);
    }
}

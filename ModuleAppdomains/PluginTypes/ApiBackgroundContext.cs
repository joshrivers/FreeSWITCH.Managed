using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeSWITCH
{
    public class ApiBackgroundContext
    {
        readonly string arguments;

        public ApiBackgroundContext(string arguments)
        {
            this.arguments = arguments;
        }

        public string Arguments { get { return arguments; } }
    }
}

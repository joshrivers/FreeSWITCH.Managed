using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeSWITCH
{
    public class AppContext
    {
        readonly string arguments;
        readonly Native.ManagedSession session;

        public AppContext(string arguments, Native.ManagedSession session)
        {
            this.arguments = arguments;
            this.session = session;
        }

        public string Arguments { get { return arguments; } }
        public Native.ManagedSession Session { get { return session; } }
    }
}

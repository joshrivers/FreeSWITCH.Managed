using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeSWITCH
{
    public class ApiContext
    {
        readonly string arguments;
        readonly Native.Stream stream;
        readonly Native.Event evt;

        public ApiContext(string arguments, Native.Stream stream, Native.Event evt)
        {
            this.arguments = arguments;
            this.stream = stream;
            this.evt = evt;
        }

        public string Arguments { get { return arguments; } }
        public Native.Stream Stream { get { return stream; } }
        public Native.Event Event { get { return evt; } }
    }
}

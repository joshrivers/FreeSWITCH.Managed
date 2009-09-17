using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace FreeSWITCH.Managed
{
    public class DefaultServiceLocator : ObjectContainer
    {
        public static DefaultServiceLocator Container { get; private set; }

        static DefaultServiceLocator()
        {
            DefaultServiceLocator.Container = new DefaultServiceLocator();
            var registry = new DefaultRegistry();
            registry.Register(DefaultServiceLocator.Container);
        }
    }
}

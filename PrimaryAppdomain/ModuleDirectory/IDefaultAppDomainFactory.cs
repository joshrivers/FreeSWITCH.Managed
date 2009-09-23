using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FreeSWITCH.Managed
{
    public interface IAppDomainFactory
    {
        AppDomain CreateAppDomain(string filePath);
    }
}

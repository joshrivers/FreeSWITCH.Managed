using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeSWITCH.Managed;
using System.Windows.Forms;

namespace WinFormLogger
{
    public class FormLoader : IPrimaryAppdomainExtension
    {
        public void Load()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var container = DefaultServiceLocator.Container.Create<LoggerContainer>();
            var form = new LogForm();
            container.Add(new FormLogger(form));
            Application.Run(form);
        }
    }
}

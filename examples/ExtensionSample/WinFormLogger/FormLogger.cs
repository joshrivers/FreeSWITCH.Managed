using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeSWITCH.Managed;
using System.Windows.Forms;

namespace WinFormLogger
{
    public class FormLogger : ILogger
    {
        private LogForm form;
        public FormLogger(LogForm form)
        {
            this.form = form;

        }
        public void Debug(string message)
        {
            this.form.AddMessage(message);
        }

        public void Info(string message)
        {
            this.form.AddMessage(message);
        }

        public void Error(string message)
        {
            this.form.AddMessage(message);
        }

        public void Critical(string message)
        {
            this.form.AddMessage(message);
        }

        public void Alert(string message)
        {
            this.form.AddMessage(message);
        }

        public void Warning(string message)
        {
            this.form.AddMessage(message);
        }

        public void Notice(string message)
        {
            this.form.AddMessage(message);
        }
    }
}

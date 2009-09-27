using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormLogger
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public void AddMessage(string message)
        {
            this.Messages.Text += message;
            this.Messages.Text += Environment.NewLine;
        }
    }
}

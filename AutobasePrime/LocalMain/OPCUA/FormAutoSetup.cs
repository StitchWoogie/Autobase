using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalMain
{
    public partial class FormAutoSetup : Form
    {
        public bool UseReconnect { get; set; } = true;
        public int ReconnectIntervalSec { get; set; } = 30;
        public FormAutoSetup()
        {
            InitializeComponent();

            this.checkBoxRestart.Checked = UseReconnect;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            UseReconnect = this.checkBoxRestart.Checked;
            ReconnectIntervalSec = (int) this.numRestartInterval.Value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

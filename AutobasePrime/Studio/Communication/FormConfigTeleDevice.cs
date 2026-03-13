using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;

namespace Studio.Communication
{
    public partial class FormConfigTeleDevice : Form
    {
        public FormConfigTeleDevice()
        {
            InitializeComponent();
        }

        public int nComPort;

        private void FormConfigTeleDevice_Load(object sender, EventArgs e)
        {
            string filename = String.Format("{0}\\scan\\TeleDevi.ini", TotalConfig.sDirWorkProject);
            string section = String.Format("COM{0}", nComPort);

            string buf = "";
            Profile.GetPrivateProfileStringA(section, "Init Command", "ATZ", ref buf, filename);
            this.textBoxTeleInitCommand.Text = buf;
	        Profile.GetPrivateProfileStringA(section, "Connect Command", "##1", ref buf, filename);
            this.textBoxTeleConnectCommand.Text = buf;

            int val = Profile.GetPrivateProfileIntA(section, "nTimeOutWaitConnect", 30, filename);
            Tools.SetNumericUpDownValue(this.numericUpDownTeleTimeout, val);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string filename = String.Format("{0}\\scan\\TeleDevi.ini", TotalConfig.sDirWorkProject);
            string section = String.Format("COM{0}", nComPort);

            Profile.WritePrivateProfileStringA(section, "Init Command", textBoxTeleInitCommand.Text, filename);
            Profile.WritePrivateProfileStringA(section, "Connect Command", textBoxTeleConnectCommand.Text, filename);
            int val = ConvertTool.ToInt32(numericUpDownTeleTimeout.Value);
            Profile.WritePrivateProfileStringA(section, "nTimeOutWaitConnect", val.ToString(), filename);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

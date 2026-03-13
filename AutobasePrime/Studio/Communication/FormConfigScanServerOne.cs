using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;

namespace Studio.Communication
{
    public partial class FormConfigScanServerOne : Form
    {
        public int nPort;

        public FormConfigScanServerOne()
        {
            InitializeComponent();
        }

        private void FormConfigScanServerOne_Load(object sender, EventArgs e)
        {
            SCAN_SERVER_LIST pt = new SCAN_SERVER_LIST();
            pt.struct_no = nPort;

            FormConfigScanServer.ScanServerReadOne(pt);

            this.radioButtonDeviceType0.Checked = (pt.nDeviceType == 0);
            this.radioButtonDeviceType1.Checked = (pt.nDeviceType == 1);
            this.radioButtonDeviceType2.Checked = (pt.nDeviceType == 2);
            this.radioButtonDeviceType3.Checked = (pt.nDeviceType == 3);
            this.radioButtonDeviceType4.Checked = (pt.nDeviceType == 4);

            this.checkBoxThread.Checked = pt.bThreadFlag;

            this.textBoxSerialPort.Text = pt.sComString;
            this.textBoxTcpPort.Text = pt.tcpipPort.ToString();
            this.textBoxSendDelay.Text = pt.nSendDelay.ToString();
            this.textBoxSendBlockSize.Text = pt.nSendBlockSize.ToString();

            //EnableDisablePort();
            //EnableDisableLineDuplex();
            //EnableDisableComputerDuplex();
            //EnableDisablePortThread();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string filename;

            string buf = "";

            filename = String.Format("{0}\\SCAN\\SERVER\\serv{1:0000}.ini", TotalConfig.sDirWorkProject, nPort);

            if (this.radioButtonDeviceType1.Checked) buf = "NULL";
            else if (this.radioButtonDeviceType2.Checked) buf = "Modem";
            else if (this.radioButtonDeviceType3.Checked) buf = "TCP/IP";
            else if (this.radioButtonDeviceType4.Checked) buf = "UDP/IP";
            else buf = "None";
            
            Profile.WritePrivateProfileStringA("config", "Connect Method", buf, filename);

            Profile.WritePrivateProfileStringA("config", "Thread", this.checkBoxThread.Checked ? "1" : "0", filename);

            Profile.WritePrivateProfileStringA("config", "nSendDelay", this.textBoxSendDelay.Text, filename);
            Profile.WritePrivateProfileStringA("config", "nSendBlockSize", this.textBoxSendBlockSize.Text, filename);

            // 모뎀 설정 찾기
            CommaBlockString comma = new CommaBlockString();
            comma.Set(this.textBoxSerialPort.Text);

            comma.GetString(ref buf);
            Profile.WritePrivateProfileStringA("Modem", "port", buf, filename);

            comma.GetString(ref buf);
            Profile.WritePrivateProfileStringA("Modem", "baud", buf, filename);

            comma.GetString(ref buf);
            Profile.WritePrivateProfileStringA("Modem", "parity", buf, filename);

            comma.GetString(ref buf);
            Profile.WritePrivateProfileStringA("Modem", "data", buf, filename);

            comma.GetString(ref buf);
            Profile.WritePrivateProfileStringA("Modem", "stop", buf, filename);

            //Profile.GetPrivateProfileString("Modem", "Initial Command", "AT &C1 B0", ref buf, filename);
            //conn.sModemInitCommand = buf;

            Profile.WritePrivateProfileStringA("tcp/ip", "port", this.textBoxTcpPort.Text, filename);

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}

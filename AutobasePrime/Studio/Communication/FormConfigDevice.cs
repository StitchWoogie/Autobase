using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;
using System.IO;
using System.Collections;

namespace Studio.Communication
{
    public partial class FormConfigDevice : Form
    {
        public FormConfigDevice()
        {
            InitializeComponent();
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {

        }

        int nDeviceType;
        int nComPort = 1;
        int nComBaud = 9600;
        int nComParity;
        int nComDataBit;
        int nComStopBit;
        int nComTxMethod;
        int nComRxMethod;
        int nComReadDelay;
        int nComWriteDelay;
        int nComStartReadDelay;
        int nComStartWriteDelay;
        int nComRtsToggle;
        string sTcpIP = "192.168.0.1";
        int nTcpPort = 2004;
        string sExtraString;
        string sSharedMemoryName = "SharedName";
        int nTcpServerTimeout = 60;

        public void Set(string options)
        {
            // TODO: Add extra initialization here
            CommaBlockString comma = new CommaBlockString();
            string buf = "";
            int value = 0;
            int i;

            comma.Set(options);
            comma.GetString(ref buf);
            if (String.Compare(buf, 0, "COM", 0, 3, true) == 0)
            {	// com port
                nDeviceType = 1;
                nComPort = ConvertTool.ToInt32(buf.Substring(3));
                comma.GetInt(ref nComBaud);
                comma.GetInt(ref nComParity);
                comma.GetInt(ref value);
                if (value == 7) nComDataBit = 1;
                else nComDataBit = 0;
                comma.GetInt(ref value);
                if (value == 2) nComStopBit = 1;
                else nComStopBit = 0;

                comma.GetString(ref buf);
                if (String.Compare(buf, "TxRTS", true) == 0)        nComTxMethod = 1;
                else if (String.Compare(buf, "TxDTR", true) == 0)   nComTxMethod = 2;
                else nComTxMethod = 0;

                comma.GetString(ref buf);
                if (String.Compare(buf, "RxECHO", true) == 0) nComRxMethod = 1;
                else nComRxMethod = 0;

                comma.GetInt(ref nComReadDelay);
                comma.GetInt(ref nComWriteDelay);
                comma.GetInt(ref nComStartReadDelay);
                comma.GetInt(ref nComStartWriteDelay);
                comma.GetInt(ref nComRtsToggle);
            }
            else if (String.Compare(buf, 0, "MODEM", 0, 5, true) == 0)
            {	// modem
                nDeviceType = 2;
                nComPort = ConvertTool.ToInt32(buf.Substring(5));
                comma.GetInt(ref nComBaud);
                comma.GetInt(ref nComParity);
                comma.GetInt(ref value);
                if (value == 7) nComDataBit = 1;
                else nComDataBit = 0;
                comma.GetInt(ref value);
                if (value == 2) nComStopBit = 1;
                else nComStopBit = 0;
            }
            else if (String.Compare(buf, "TCP/IP", true) == 0)
            {	// tcp/ip
                nDeviceType = 3;

                comma.GetString(ref sTcpIP);
                comma.GetInt(ref nTcpPort);
                comma.GetStringTotalRemain(ref sExtraString);
            }
            else if (String.Compare(buf, "UDP/IP", true) == 0)
            {	// udp/ip
                nDeviceType = 4;

                comma.GetString(ref sTcpIP);
                comma.GetInt(ref nTcpPort);
            }
            else if (String.Compare(buf, "TCP-Server", true) == 0)
            {	// 
                nDeviceType = 5;

                comma.GetInt(ref nTcpPort);
                comma.GetInt(ref nTcpServerTimeout);
                if (nTcpServerTimeout <= 0) nTcpServerTimeout = 60;
                comma.GetStringTotalRemain(ref sExtraString);
            }
            else if (String.Compare(buf, 0, "TeleDevice", 0, 10, true) == 0)
            {	// Tele Device
                nDeviceType = 7;

                nComPort = ConvertTool.ToInt32(buf.Substring(10));
                comma.GetInt(ref nComBaud);
                comma.GetInt(ref nComParity);
                comma.GetInt(ref value);
                if (value == 7) nComDataBit = 1;
                else nComDataBit = 0;
                comma.GetInt(ref value);
                if (value == 2) nComStopBit = 1;
                else nComStopBit = 0;
            }
            else if (String.Compare(buf, "NetClient", true) == 0)
            {		// Net Client Device
                nDeviceType = 8;

                //comma.GetString(buf, sizeof(buf));
                //m_ipAddress = buf;
                //comma.GetInt(m_ipPort);
            }
            else if (String.Compare(buf, "SharedMemory", true) == 0)
            {	// Net Client Device
                nDeviceType = 9;

                comma.GetString(ref sSharedMemoryName);
            }
            else
            {
                nDeviceType = 0;
            }

            this.radioButtonDeviceType0.Checked = (nDeviceType == 0);
            this.radioButtonDeviceType1.Checked = (nDeviceType == 1);
            this.radioButtonDeviceType2.Checked = (nDeviceType == 2);
            this.radioButtonDeviceType3.Checked = (nDeviceType == 3);
            this.radioButtonDeviceType4.Checked = (nDeviceType == 4);
            this.radioButtonDeviceType5.Checked = (nDeviceType == 5);
            this.radioButtonDeviceType6.Checked = (nDeviceType == 6);
            this.radioButtonDeviceType7.Checked = (nDeviceType == 7);
            this.radioButtonDeviceType8.Checked = (nDeviceType == 8);
            this.radioButtonDeviceType9.Checked = (nDeviceType == 9);

            this.radioButtonComParityBit0.Checked = (nComParity == 0);
            this.radioButtonComParityBit1.Checked = (nComParity == 1);
            this.radioButtonComParityBit2.Checked = (nComParity == 2);

            this.radioButtonComDataBit0.Checked = (nComDataBit == 0);
            this.radioButtonComDataBit1.Checked = (nComDataBit == 1);

            this.radioButtonComStopBit0.Checked = (nComStopBit == 0);
            this.radioButtonComStopBit1.Checked = (nComStopBit == 1);

            this.radioButtonComTx0.Checked = (nComTxMethod == 0);
            this.radioButtonComTx1.Checked = (nComTxMethod == 1);
            this.radioButtonComTx2.Checked = (nComTxMethod == 2);

            this.radioButtonComRx0.Checked = (nComRxMethod == 0);
            this.radioButtonComRx1.Checked = (nComRxMethod == 1);
            this.radioButtonComRx2.Checked = (nComRxMethod == 2);

            Tools.SetNumericUpDownValue(this.numericUpDownComEndDelayRead, nComReadDelay);
            Tools.SetNumericUpDownValue(this.numericUpDownComEndDelayWrite, nComWriteDelay);
            Tools.SetNumericUpDownValue(this.numericUpDownComStartDelayRead, nComStartReadDelay);
            Tools.SetNumericUpDownValue(this.numericUpDownComStartDelayWrite, nComStartWriteDelay);

            this.checkBoxRtsMethod.Checked = (nComRtsToggle == 1);

            for (i = 0; i < 256; i++)
            {
                buf = String.Format("COM{0}", i + 1);
                comboBoxComPort.Items.Add(buf);
            }
            

            for (i = 0; i < MAX_COM_BAUD; i++)
            {
                buf = nSampleBaudType[i].ToString();
                comboBoxComBaud.Items.Add(buf);
            }
            

            this.textBoxTcpIP.Text = sTcpIP;
            Tools.SetNumericUpDownValue(this.numericUpDownTcpPort,nTcpPort);

            this.textBoxSharedMemoryName.Text = sSharedMemoryName;

            Tools.SetNumericUpDownValue(this.numericUpDownTcpServerTimeout, nTcpServerTimeout);
        }

        const int MAX_COM_BAUD = 13;
        int[] nSampleBaudType = { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 56000, 57600, 115200, 128000, 256000 };
        string[] sTxMethod = { "TxON", "TxRTS", "TxDTR" };
        string[] sRxMethod = { "RxON", "RxECHO", "RxETC" };

        string sDeviceString = "";

        public string Get()
        {
            return sDeviceString;
        }

        private void FormConfigDevice_Load(object sender, EventArgs e)
        {
            string buf;
            
            buf = String.Format("COM{0}", nComPort);
            comboBoxComPort.SelectedText = buf; // 로딩 시 선택해야 선택이 잘된다.

            buf = nComBaud.ToString();
            comboBoxComBaud.SelectedText = buf;

            EnableDisable();
        }

        int GetDeviceType()
        {
            int type = 0;

            if (this.radioButtonDeviceType0.Checked) type = 0;
            else if (this.radioButtonDeviceType1.Checked) type = 1;
            else if (this.radioButtonDeviceType2.Checked) type = 2;
            else if (this.radioButtonDeviceType3.Checked) type = 3;
            else if (this.radioButtonDeviceType4.Checked) type = 4;
            else if (this.radioButtonDeviceType5.Checked) type = 5;
            else if (this.radioButtonDeviceType6.Checked) type = 6;
            else if (this.radioButtonDeviceType7.Checked) type = 7;
            else if (this.radioButtonDeviceType8.Checked) type = 8;
            else if (this.radioButtonDeviceType9.Checked) type = 9;
            else type = 0;

            return type;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string buf;

            nDeviceType = GetDeviceType();
            
            if (this.radioButtonComParityBit0.Checked) nComParity = 0;
            else if (this.radioButtonComParityBit1.Checked) nComParity = 1;
            else if (this.radioButtonComParityBit2.Checked) nComParity = 2;
            else  nComParity = 0;

            if (this.radioButtonComDataBit0.Checked) nComDataBit = 0;
            else if (this.radioButtonComDataBit1.Checked) nComDataBit = 1;
            else nComDataBit = 0;

            if (this.radioButtonComStopBit0.Checked) nComStopBit = 0;
            else if (this.radioButtonComStopBit1.Checked) nComStopBit = 1;
            else nComStopBit = 0;

            if (this.radioButtonComTx0.Checked) nComTxMethod = 0;
            else if (this.radioButtonComTx1.Checked) nComTxMethod = 1;
            else if (this.radioButtonComTx2.Checked) nComTxMethod = 2;
            else nComTxMethod = 0;

            if (this.radioButtonComRx0.Checked) nComRxMethod = 0;
            else if (this.radioButtonComRx1.Checked) nComRxMethod = 1;
            else if (this.radioButtonComRx2.Checked) nComRxMethod = 2;
            else nComRxMethod = 0;

            buf = comboBoxComPort.Text;
            nComPort = ConvertTool.ToInt32(buf.Substring(3));

            buf = comboBoxComBaud.Text;
            nComBaud = ConvertTool.ToInt32(buf);

            nComReadDelay = ConvertTool.ToInt32(this.numericUpDownComEndDelayRead.Value);
            nComWriteDelay = ConvertTool.ToInt32(this.numericUpDownComEndDelayWrite.Value);
            nComStartReadDelay = ConvertTool.ToInt32(this.numericUpDownComStartDelayRead.Value);
            nComStartWriteDelay = ConvertTool.ToInt32(this.numericUpDownComStartDelayWrite.Value);

            nComRtsToggle = this.checkBoxRtsMethod.Checked ? 1 : 0;

            sTcpIP = this.textBoxTcpIP.Text;
            nTcpPort = ConvertTool.ToInt32(this.numericUpDownTcpPort.Value);
            nTcpServerTimeout = ConvertTool.ToInt32(this.numericUpDownTcpServerTimeout.Value);

            sSharedMemoryName = this.textBoxSharedMemoryName.Text;

            if (nDeviceType == 1)
            {
                sDeviceString = String.Format("COM{0},{1},{2},{3},{4},", nComPort, nComBaud, nComParity, nComDataBit == 1 ? 7 : 8, nComStopBit == 1 ? 2 : 1);
                if (nComTxMethod != 0 || nComRxMethod != 0)
                {
                    buf = String.Format("{0},{1},", sTxMethod[nComTxMethod], sRxMethod[nComRxMethod]);
                    sDeviceString += buf;

                    //if (nComReadDelay != 0 || nComWriteDelay != 0)
                    //{
                        buf = String.Format("{0},{1},", nComReadDelay, nComWriteDelay);
                        sDeviceString += buf;
                    //}

                    //if (nComStartReadDelay != 0 || nComStartWriteDelay != 0)
                    //{
                        buf = String.Format("{0},{1},", nComStartReadDelay, nComStartWriteDelay);
                        sDeviceString += buf;
                    //}
                        sDeviceString += nComRtsToggle.ToString();
                }
            }
            else if (nDeviceType == 2)
            {
                sDeviceString = String.Format("MODEM{0},{1},{2},{3},{4},", nComPort, nComBaud, nComParity, nComDataBit == 1 ? 7 : 8, nComStopBit == 1 ? 2 : 1);
            }
            else if (nDeviceType == 3)
            {
                sDeviceString = String.Format("TCP/IP, {0}, {1},", sTcpIP, nTcpPort);
                sDeviceString += sExtraString;
            }
            else if (nDeviceType == 4)
            {
                sDeviceString = String.Format("UDP/IP, {0}, {1},", sTcpIP, nTcpPort);
            }
            else if (nDeviceType == 5)
            {
                sDeviceString = String.Format("TCP-Server, {0}, {1}", nTcpPort, nTcpServerTimeout);
            }
            else if (nDeviceType == 7)
            {
                sDeviceString = String.Format("TeleDevice{0},{1},{2},{3},{4},", nComPort, nComBaud, nComParity, nComDataBit == 1 ? 7 : 8, nComStopBit == 1 ? 2 : 1);
            }
            else if (nDeviceType == 8)
            {
                sDeviceString = String.Format("NetClient");
            }
            else if (nDeviceType == 9)
            {
                sDeviceString = String.Format("SharedMemory,{0}", sSharedMemoryName);
            }
            else
            {
                sDeviceString = String.Format("None");
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonTeleDeviceOption_Click(object sender, EventArgs e)
        {
            FormConfigTeleDevice dialog = new FormConfigTeleDevice();

            dialog.nComPort = ConvertTool.ToInt32(comboBoxComPort.Text.Substring(3));

            dialog.ShowDialog(this);
        }

        private void radioButtonDeviceType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType2_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType3_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType4_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType5_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType6_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType7_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType8_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonDeviceType9_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        void EnableDisable()
        {
            int type = GetDeviceType();

            bool flag_com = false;
            bool flag_485 = false;
            bool flag_ip = false;
            bool flag_port = false;
            bool flag_tele = false;
            bool flag_share = false;
            bool flag_tcpserver_timeout = false;

            if (type == 1)
            {
                flag_com = true;
                flag_485 = true;
            }
            else if (type == 2)
            {
                flag_com = true;
            }
            else if (type == 3)
            {
                flag_ip = true;
                flag_port = true;
            }
            else if (type == 4)
            {
                flag_ip = true;
                flag_port = true;
            }
            else if (type == 5)
            {
                flag_ip = false;
                flag_port = true;
                flag_tcpserver_timeout = true;
            }
            else if (type == 7)
            {
                flag_com = true;
                flag_tele = true;
            }
            else if (type == 9)
            {
                flag_share = true;
            }

            this.comboBoxComPort.Enabled = flag_com;
            this.comboBoxComBaud.Enabled = flag_com;

            this.groupBoxParityBit.Enabled = flag_com;
            this.groupBoxDataBit.Enabled = flag_com;
            this.groupBoxStopBit.Enabled = flag_com;

            this.groupBoxTxFlow.Enabled = flag_485;
            this.groupBoxRxFlow.Enabled = flag_485;
            this.groupBoxStartDelay.Enabled = flag_485;
            this.groupBoxEndDelay.Enabled = flag_485;

            this.checkBoxRtsMethod.Enabled = flag_485;

            this.textBoxTcpIP.Enabled = flag_ip;
            this.numericUpDownTcpPort.Enabled = flag_port;

            this.groupBoxTeleDevice.Enabled = flag_tele;
            this.groupBoxSharedMemory.Enabled = flag_share;

            this.numericUpDownTcpServerTimeout.Enabled = flag_tcpserver_timeout;
        }

        

    }

    
}

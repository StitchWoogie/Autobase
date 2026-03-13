using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AutoLibLocal;
using NetTools;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using NetTools.Cryptography;

namespace Studio.Communication
{
    public partial class FormConfigPort : Form
    {
        public int nPort = 0;
        public FormConfigPort()
        {
            InitializeComponent();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        string[] protocolNameDefine = { 
		"GANTNER",	
		//"SPC-300",
		"MF2",		
		//"AOJ2-C214",
		//"WR3380",	
		"DPM2",		
		//"GLOFA",	
		//"MASTER-K",	
		//"ABPLC5",	
		"LA250",	
		"BL-2300",	
		"PM-170E",	
		//"MJ71E71",	
		//"MODICON",	
		"TMTC",		
		"HOST-LINK",
		"PM-B",		
		"PT-L",		
		//"RLINK",	
		//"ADAM",		
		"SP30",		
		//"GMPC",		
		"PCD",		
		//"PCD-SBUS",	
		"MASTER-K10/60/200",	
		"MASTER-K30/50",	
		//"GE-SNP",		
		"Network Client Multi",		
		"Network Client Virtual",		
                                      };

        void FillComboProtocol(ComboBox combo)
        {
            DLL_PROTOCOL_LIST item;

            for (int i = 0; i < protocolNameDefine.Length; i++)
            {
                combo.Items.Add(protocolNameDefine[i]);
            }
            
            for (int i = 0; i < blockProtocolList.Count; i++)
            {
                item = (DLL_PROTOCOL_LIST)blockProtocolList[i];

                if (item.bProtocol == 1)
                {
                    combo.Items.Add("DLL-" + item.title);
                }
            }
        }

        private void FormConfigPort_Load(object sender, EventArgs e)
        {
            PORT_STRUCT pt = new PORT_STRUCT();
            pt.no = nPort;

            FormConfigAllPort.CommDeviceRead(pt);
            blockProtocolList = LoadProtocolList();

            // 일단은 프로토콜 정보 읽기가 불안할 수 있으므로 파일이 없을 때만 사용한다. 10.1.1
            if(blockProtocolList.Count == 0)
                CompareDllProtocolList(false);

            FillComboProtocol(this.comboBoxProtocol);
            FillComboProtocol(this.comboBoxSecondaryProtocol);

            this.checkBoxActive.Checked = pt.bActiveFlag;
            this.textBoxDescription.Text = pt.sTitle;
            this.textBoxDevice.Text = pt.sScanDevice;
            this.comboBoxProtocol.Text = pt.sScanProtocol;
            this.textBoxProtocolOption.Text = pt.sScanProtocolOption[0];
            

            this.checkBoxThread.Checked = pt.bActiveThread;
            Tools.SetNumericUpDownValue(this.numericUpDownThreadCycle, pt.nThreadCycle);

            Tools.SetNumericUpDownValue(this.numericUpDownReadCycle, pt.nLocalReadScanTime);
            Tools.SetNumericUpDownValue(this.numericUpDownWriteCycle, pt.nLocalWriteScanTime);

            Tools.SetNumericUpDownValue(this.numericUpDownSizeWORD, pt.nBufSizeWORD);
            Tools.SetNumericUpDownValue(this.numericUpDownSizeFLOAT, pt.nBufSizeFLOAT);
            Tools.SetNumericUpDownValue(this.numericUpDownSizeDWORD, pt.nBufSizeDWORD);
            Tools.SetNumericUpDownValue(this.numericUpDownSizeSTRING, pt.nBufSizeSTRING);
            Tools.SetNumericUpDownValue(this.numericUpDownSizeDOUBLE, pt.nBufSizeDOUBLE);
            Tools.SetNumericUpDownValue(this.numericUpDownSizeINT64, pt.nBufSizeINT64);

            Tools.SetNumericUpDownValue(this.numericUpDownTimeoutRead, pt.TIMEOUT_MILLI_READ);
            Tools.SetNumericUpDownValue(this.numericUpDownTimeoutWrite, pt.TIMEOUT_MILLI_WRITE);

            this.checkBoxUseStationInfomation.Checked = pt.bUseStationInfo;
            this.checkBoxUseDeviceInformation.Checked = pt.bUseDeviceInfo;


            this.textBoxTelNumber.Text = pt.sTelNumber;
            Tools.SetNumericUpDownValue(this.numericUpDownTelConnectionCycle, pt.nTelConnectCycle);
            Tools.SetNumericUpDownValue(this.numericUpDownTelConnectionTimeAuto, pt.nTelConnectingTime);
            Tools.SetNumericUpDownValue(this.numericUpDownTelConnectionTimeManual, pt.nTelConnectingTimeOnManual);
            this.checkBoxTelAutoConnection.Checked = pt.bTelAutoConnection;

            this.checkBoxUseLineDuplex.Checked = pt.bDualActive;
            this.textBoxSecondaryDevice.Text = pt.sDualDevice;
            Tools.SetNumericUpDownValue(this.numericUpDownLineDuplexTimeout, pt.nDualCauseTimeOut);
            Tools.SetNumericUpDownValue(this.numericUpDownLineDuplexCodebad, pt.nDualCauseCodeBad);
            this.checkBoxUseSecondaryProtocol.Checked = pt.bDualUseProtocol;
            this.comboBoxSecondaryProtocol.Text = pt.sDualProtocol;
            this.textBoxSecondaryProtocolOption.Text = pt.sScanProtocolOption[1];

            this.checkBoxUseComputerDuplex.Checked = pt.bComputerDualActive;
            this.checkBoxComputerDuplexThread.Checked = pt.bComputerDualThread;
            this.textBoxComputerDuplexIP.Text = pt.sComputerDualIP;
            Tools.SetNumericUpDownValue(this.numericUpDownComputerDualPort, pt.nComputerDualPort);
            Tools.SetNumericUpDownValue(this.numericUpDownComputerDuplexTimeout, pt.nComputerDualTimeout);
            
            this.textBoxReadMethod.Text = pt.sEditBuf;

            this.textBoxPortNumber.Text = nPort.ToString("0");

            this.userControlConfigCryptography1.Set(pt.ec);

            EnableDisablePort();
            EnableDisableLineDuplex();
            EnableDisableComputerDuplex();
            EnableDisablePortThread();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            CryptoCommunication ec = new CryptoCommunication();
            if (!userControlConfigCryptography1.GetWithMessage(ec))
            {
                return;
            }

            string filename;

            filename = String.Format("{0}\\SCAN", TotalConfig.sDirWorkProject);

            if (!Directory.Exists(filename)) Directory.CreateDirectory(filename);

            filename = String.Format("{0}\\SCAN\\SCAN.{1:000}", TotalConfig.sDirWorkProject, this.nPort);

            TextWriter writer = new StreamWriter(filename, false, Encoding.Default);

            if (writer == null)
            {
                MessageBox.Show("파일을 저장할 수 없습니다.", filename);
                return;
            }

            const int FILE_VERSION_Major =	10;
            const int FILE_VERSION_Minor  = 3;
            const int FILE_VERSION_Build = 4;
            
            writer.WriteLine("FileVersion,{0}.{1}.{2}\r\n", FILE_VERSION_Major, FILE_VERSION_Minor, FILE_VERSION_Build);
            writer.WriteLine("ACTIVE,{0},", this.checkBoxActive.Checked ? "ON" : "OFF");
            writer.WriteLine("TITLE,{0},", this.textBoxDescription.Text);
            writer.WriteLine("BUF_LENGTH,{0},", this.numericUpDownSizeWORD.Value);
            writer.WriteLine("BUF_LENGTH_FLOAT,{0},", this.numericUpDownSizeFLOAT.Value);
            writer.WriteLine("BUF_LENGTH_DWORD,{0},", this.numericUpDownSizeDWORD.Value);
            writer.WriteLine("BUF_LENGTH_STRING,{0},", this.numericUpDownSizeSTRING.Value);
            writer.WriteLine("BUF_LENGTH_DOUBLE,{0},", this.numericUpDownSizeDOUBLE.Value);
            writer.WriteLine("BUF_LENGTH_INT64,{0},", this.numericUpDownSizeINT64.Value);
            writer.WriteLine("DEVICE,{0}", this.textBoxDevice.Text);
            writer.WriteLine("PROTOCOL,{0},{1}", this.comboBoxProtocol.Text, this.textBoxProtocolOption.Text);

            // 10.2 이전 버전을 위하여 초 단위의 TIMEOUT도 같이 기록해 준다. 이 부분이 TIMEOUT_MILLI_???? 보다 앞에 있어야 한다.
            int old_timeout_read = ConvertTool.ToInt32(this.numericUpDownTimeoutRead.Value) / 1000;
	        if(old_timeout_read < 2)	old_timeout_read = 2;
            writer.WriteLine("MAX_TIME_OUT_READ,{0},", old_timeout_read);
            int old_timeout_write = ConvertTool.ToInt32(this.numericUpDownTimeoutWrite.Value) / 1000;
	        if(old_timeout_write < 2)	old_timeout_write = 2;
	        writer.WriteLine("MAX_TIME_OUT_WRITE,{0},", old_timeout_write);

            writer.WriteLine("TIMEOUT_MILLI_READ,{0},", this.numericUpDownTimeoutRead.Value);
            writer.WriteLine("TIMEOUT_MILLI_WRITE,{0},", this.numericUpDownTimeoutWrite.Value);

            writer.WriteLine("TELNUMBER,{0},", this.textBoxTelNumber.Text);// , 저장 문제점
            writer.WriteLine("TelConnectCicle,{0},", this.numericUpDownTelConnectionCycle.Value);
            writer.WriteLine("TelConnectingTime,{0},", this.numericUpDownTelConnectionTimeAuto.Value);
            writer.WriteLine("TelAutoConnection,{0},", this.checkBoxTelAutoConnection.Checked ? 1 : 0);
            writer.WriteLine("TelConnectingTimeOnManual,{0},", this.numericUpDownTelConnectionTimeManual.Value);
            writer.WriteLine("ScanTimeRead,{0},", this.numericUpDownReadCycle.Value);
            writer.WriteLine("ScanTimeWrite,{0},", this.numericUpDownWriteCycle.Value);
            writer.WriteLine("DualActive,{0},", this.checkBoxUseLineDuplex.Checked ? 1 : 0);
            writer.WriteLine("DualDevice,{0}", this.textBoxSecondaryDevice.Text);
            writer.WriteLine("DualCauseTimeOut,{0},", this.numericUpDownLineDuplexTimeout.Value);
            writer.WriteLine("DualCauseCodeBad,{0},", this.numericUpDownLineDuplexCodebad.Value);
            writer.WriteLine("DualUseProtocol,{0},", this.checkBoxUseSecondaryProtocol.Checked ? 1 : 0);
            writer.WriteLine("DualProtocol,{0},{1}", this.comboBoxSecondaryProtocol.Text, this.textBoxSecondaryProtocolOption.Text);

            writer.WriteLine("ActiveThread,{0},", this.checkBoxThread.Checked ? 1 : 0);
            writer.WriteLine("ThreadCycle,{0},", this.numericUpDownThreadCycle.Value);

            writer.WriteLine("ComputerDual,{0},{1},{2},{3},{4},", this.checkBoxUseComputerDuplex.Checked ? 1 : 0,
                this.checkBoxComputerDuplexThread.Checked ? 1 : 0,
                this.textBoxComputerDuplexIP.Text, this.numericUpDownComputerDualPort.Value, this.numericUpDownComputerDuplexTimeout.Value);
            writer.WriteLine("UseStationInfo,{0},", this.checkBoxUseStationInfomation.Checked ? 1 : 0);
            writer.WriteLine("UseDeviceInfo,{0},", this.checkBoxUseDeviceInformation.Checked ? 1 : 0);

            writer.Write("{0}", textBoxReadMethod.Text);

            writer.Close();


            filename = String.Format("{0}\\SCAN\\Config_{1:000}.cfg", TotalConfig.sDirWorkProject, this.nPort);

            CryptoTextWriter cfw = new CryptoTextWriter(filename);
            ec.Save(cfw);
            cfw.Close();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void numericUpDownThreadCycle_ValueChanged(object sender, EventArgs e)
        {

        }

        private void buttonDevice_Click(object sender, EventArgs e)
        {
            FormConfigDevice dialog = new FormConfigDevice();

            dialog.Set(this.textBoxDevice.Text);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxDevice.Text = dialog.Get();
            }

        }

        private void buttonSecondaryDevice_Click(object sender, EventArgs e)
        {
            FormConfigDevice dialog = new FormConfigDevice();

            dialog.Set(this.textBoxSecondaryDevice.Text);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxSecondaryDevice.Text = dialog.Get();
            }
        }

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisablePort();
        }

        void EnableDisablePort()
        {
            bool flag = this.checkBoxActive.Checked;

            this.tabControl1.Enabled = flag;
            this.textBoxReadMethod.Enabled = flag;
        }

        private void checkBoxUseLineDuplex_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableLineDuplex();
        }

        void EnableDisableLineDuplex()
        {
            bool flag = this.checkBoxUseLineDuplex.Checked;

            this.groupBoxLineDuplexCondition.Enabled = flag;
            this.groupBoxLineDuplexDevice.Enabled = flag;
            this.groupBoxLineDuplexProtocol.Enabled = flag;
        }

        private void checkBoxUseComputerDuplex_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableComputerDuplex();
        }

        void EnableDisableComputerDuplex()
        {
            bool flag = this.checkBoxUseComputerDuplex.Checked;

            this.checkBoxComputerDuplexThread.Enabled = flag;
            this.groupBoxComputerDuplexAnotherComputer.Enabled = flag;
            this.groupBoxComputerDuplexCondition.Enabled = flag;
        }

        private void checkBoxThread_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisablePortThread();
        }

        void EnableDisablePortThread()
        {
            bool flag = this.checkBoxThread.Checked;

            this.numericUpDownThreadCycle.Enabled = flag;
        }

        bool CompareProtocolOne(DLL_PROTOCOL_LIST item)
        {
            int l;
            DLL_PROTOCOL_LIST src;

            for (l = 0; l < blockProtocolList.Count; l++)
            {
                src = (DLL_PROTOCOL_LIST)blockProtocolList[l];

                if (String.Compare(src.filename, item.filename, true) == 0)
                {
                    if (item.size != src.size) return false;
                    if (item.ft != src.ft) return false;

                    item.bProtocol = src.bProtocol;
                    item.title = src.title;
                    item.VersionMajor = src.VersionMajor;
                    item.VersionMinor = src.VersionMinor;

                    blockProtocolList.RemoveAt(l);
                    return true;
                }
            }

            return false;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ProtocolGetDriverTitle(StringBuilder str);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ushort ProtocolGetDriverVersionMajor();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ushort ProtocolGetDriverVersionMinor();

        void CompareDllProtocolList(bool all_update)
        {
            string directory = String.Format("{0}\\protocol", Application.StartupPath);

            DirectoryInfo di = new DirectoryInfo(directory);
            DLL_PROTOCOL_LIST item;
            ArrayList block = new ArrayList();
            bool change_flag = false;
            string msg;
            IntPtr proc;

            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {
                item = new DLL_PROTOCOL_LIST();

                item.filename = fi.Name;
                item.title = "Not protocol file";
                item.bProtocol = 0;
                item.size = fi.Length;
                item.ft = fi.LastWriteTime;

                if (all_update == false && CompareProtocolOne(item))
                {
                    goto next;
                }

                change_flag = true;

                IntPtr hdll = NativeMethods.LoadLibrary(fi.FullName);

                if (hdll == null)
                {
                    msg = String.Format("LoadLibrary failed ({0} file)", fi.FullName);
                    MessageBox.Show(msg, "DLL Error");
                    return;
                }

                proc = NativeMethods.GetProcAddress(hdll, "ProtocolGetDriverTitle");
                if (proc == IntPtr.Zero)
                {
                    NativeMethods.FreeLibrary(hdll);
                    goto next;
                }
                ProtocolGetDriverTitle procGetDriverTitle = (ProtocolGetDriverTitle)Marshal.GetDelegateForFunctionPointer(proc,typeof(ProtocolGetDriverTitle));
                StringBuilder text = new StringBuilder("", 1000);
                procGetDriverTitle(text);
                item.title = text.ToString();

                proc = NativeMethods.GetProcAddress(hdll, "ProtocolGetDriverVersionMajor");
                if (proc == IntPtr.Zero)
                {
                    msg = String.Format("DLL Major Version information not found.\nfilename={0}", fi.FullName);
                    MessageBox.Show(msg, "Unknown Version");
                    NativeMethods.FreeLibrary(hdll);
                    goto next;
                }
                ProtocolGetDriverVersionMajor procGetDriverVersionMajor = (ProtocolGetDriverVersionMajor)Marshal.GetDelegateForFunctionPointer(proc, typeof(ProtocolGetDriverVersionMajor));
                item.VersionMajor = (short)procGetDriverVersionMajor();

                int PLCSCAN_VERSION = 8;

                if (item.VersionMajor != PLCSCAN_VERSION)
                {
                    msg = String.Format("DLL Major Version mismatched.\nPLC_SCAN Version = {0}\n{1} file Version = {2}", PLCSCAN_VERSION, fi.FullName, item.VersionMajor);
                    MessageBox.Show(msg, "Major Version Mismatched");
                    NativeMethods.FreeLibrary(hdll);
                    goto next;
                }

                proc = NativeMethods.GetProcAddress(hdll, "ProtocolGetDriverVersionMinor");
                if (proc == IntPtr.Zero)
                {
                    msg = String.Format("DLL Minor Version information not found.\nfilename{0}", fi.FullName);
                    MessageBox.Show(msg, "Unknown Version");
                    NativeMethods.FreeLibrary(hdll);
                    goto next;
                }
                ProtocolGetDriverVersionMinor procGetDriverVersionMinor = (ProtocolGetDriverVersionMinor)Marshal.GetDelegateForFunctionPointer(proc, typeof(ProtocolGetDriverVersionMinor));
                item.VersionMinor = (short)procGetDriverVersionMinor();

                NativeMethods.FreeLibrary(hdll);

                item.bProtocol = 1;

            next: ;
                block.Add(item);
            }

            blockProtocolList = block;

            if (change_flag)
            {
                SaveProtocolList();
            }
        }

        void SaveProtocolList()
        {
            string filename;

            filename = String.Format("{0}\\protocol\\protocol.lst", Application.StartupPath);
            TextWriter writer;
            CommaBlockString comma = new CommaBlockString();
            DLL_PROTOCOL_LIST item;
            int l;

            writer = new StreamWriter(filename, false, Encoding.Default);
            if (writer == null)
            {
                string msg;
                msg = String.Format("Can't write file.\n{0}", filename);
                MessageBox.Show(msg, "File Open Error");
                return;
            }

            long ft;
            uint loDateTime, hiDateTime;

            for (l = 0; l < blockProtocolList.Count; l++)
            {
                item = (DLL_PROTOCOL_LIST)blockProtocolList[l];
                writer.Write("{0},", item.filename);

                ft = item.ft.ToFileTime();
                loDateTime = (uint)(ft & 0xFFFFFFFF);
                hiDateTime = (uint)((ft >> 32) & 0xFFFFFFFF);

                writer.Write("{0:X},", loDateTime);
                writer.Write("{0:X},", hiDateTime);
                writer.Write("{0},", item.size);
                writer.Write("{0},", item.bProtocol);
                writer.Write("{0},", item.title);
                writer.Write("{0},", item.VersionMajor);
                writer.Write("{0},", item.VersionMinor);
                writer.WriteLine();
            }

            writer.Close();
        }

        public static ArrayList LoadProtocolList()
        {
            string filename;
            ArrayList block = new ArrayList();

            filename = String.Format("{0}\\protocol\\protocol.lst", Application.StartupPath);

            string buf;
            CommaBlockString comma = new CommaBlockString();
            DLL_PROTOCOL_LIST item;
            uint loDateTime=0, hiDateTime=0;

            if (!File.Exists(filename)) return block;

            TextReader reader = new StreamReader(filename, Encoding.Default);
            if (reader == null) return block;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;
                if (buf.Length == 0) continue;

                comma.Set(buf);

                item = new DLL_PROTOCOL_LIST();

                comma.GetString(ref item.filename);
                comma.GetHexDWORD(ref loDateTime);
                comma.GetHexDWORD(ref hiDateTime);
                item.ft = DateTime.FromFileTime(loDateTime + hiDateTime * 0x100000000);
                comma.GetLong(ref item.size);
                comma.GetChar(ref item.bProtocol);
                comma.GetString(ref item.title);
                comma.GetInt(ref item.VersionMajor);
                comma.GetInt(ref item.VersionMinor);
                block.Add(item);
            }

            reader.Close();

            return block;
        }

        ArrayList blockProtocolList = new ArrayList();

        void SelectProtocolOption(ComboBox combo, TextBox textbox)
        {
            string protocol_name = combo.Text;

            if (String.Compare(protocol_name, 0, "DLL-", 0, 4) != 0)
            {
                MessageBox.Show(protocol_name, "Not protocol option");
                return;
            }

            string filename;
            string msg;
            int retn;

            DLL_PROTOCOL_LIST item;
            int l;

            for (l = 0; l < blockProtocolList.Count; l++)
            {
                item = (DLL_PROTOCOL_LIST)blockProtocolList[l];

                if (String.Compare(protocol_name.Substring(4), item.title) != 0) continue;	// not seek

                filename = String.Format("{0}\\protocol\\{1}", Application.StartupPath, item.filename);

                IntPtr hdll = NativeMethods.LoadLibrary(filename);

                if (hdll == null)
                {
                    msg = String.Format("LoadLibrary failed ({0} file)", filename);
                    MessageBox.Show(msg, "DLL Error");
                    return;
                }

                IntPtr pProtocolConfigOption = NativeMethods.GetProcAddress(hdll, "ProtocolConfigOption_8_2");

                ProtocolConfigOption protocolConfigOption = (ProtocolConfigOption)Marshal.GetDelegateForFunctionPointer(
                                                                                        pProtocolConfigOption,
                                                                                        typeof(ProtocolConfigOption));

                StringBuilder options = new StringBuilder(textbox.Text, 1000);

                retn = protocolConfigOption(this.Handle, this.textBoxReadMethod.Handle, this.nPort, options);

                if (retn != 0)
                {
                    textbox.Text = options.ToString();
                }

                NativeMethods.FreeLibrary(hdll);

                return;
            }

            //MessageBoxNoConfig(hwnd, protocol_name);
            return;
        }

        private void buttonProtocolOption_Click(object sender, EventArgs e)
        {
            SelectProtocolOption(this.comboBoxProtocol, this.textBoxProtocolOption);
            /*
            string protocol_name = this.comboBoxProtocol.Text;

            if (String.Compare(protocol_name, 0, "DLL-", 0, 4) != 0)
            {
                MessageBox.Show(protocol_name, "Not protocol option");
                return;
            }

            string filename;
            string msg;
            int retn;
            
            DLL_PROTOCOL_LIST item;
            int l;
            
            for (l = 0; l < blockProtocolList.Count; l++)
            {
                item = (DLL_PROTOCOL_LIST)blockProtocolList[l];
                
                if (String.Compare(protocol_name.Substring(4), item.title) != 0) continue;	// not seek

                filename = String.Format("{0}\\protocol\\{1}", Application.StartupPath, item.filename);

                IntPtr hdll = NativeMethods.LoadLibrary(filename);

                if (hdll == null)
                {
                    msg = String.Format("LoadLibrary failed ({0} file)", filename);
                    MessageBox.Show(msg, "DLL Error");
                    return;
                }

                IntPtr pProtocolConfigOption = NativeMethods.GetProcAddress(hdll, "ProtocolConfigOption_8_2");

                ProtocolConfigOption protocolConfigOption = (ProtocolConfigOption)Marshal.GetDelegateForFunctionPointer(
                                                                                        pProtocolConfigOption,
                                                                                        typeof(ProtocolConfigOption));

                StringBuilder options = new StringBuilder(this.textBoxProtocolOption.Text, 1000);
                
                retn = protocolConfigOption(this.Handle, this.textBoxReadMethod.Handle, this.nPort, options);

                if (retn != 0)
                {
                    this.textBoxProtocolOption.Text = options.ToString();
                }

                NativeMethods.FreeLibrary(hdll);

                return;
            }

            //MessageBoxNoConfig(hwnd, protocol_name);
            return;*/
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int ProtocolConfigOption(IntPtr HWND, IntPtr hwndEdit, int port, StringBuilder str);

        private void buttonCancel_Click(object sender, EventArgs e)
        {
                    
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxUseSecondaryProtocol_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonSecondaryProtocolOption_Click(object sender, EventArgs e)
        {
            SelectProtocolOption(this.comboBoxSecondaryProtocol, this.textBoxSecondaryProtocolOption);
        }

    }

    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    class DLL_PROTOCOL_LIST
    {
        public string filename;
        public string title;
        public sbyte bProtocol;
        public DateTime ft;
        public long size;
        public short VersionMajor;
        public short VersionMinor;
    }

    
}


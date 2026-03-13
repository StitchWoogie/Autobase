using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using System.IO;
using NetTools;
using System.Diagnostics;
using FutureVersion;
using System.Collections;
using NetTools.Cryptography;

namespace Studio.Communication
{
    public partial class FormConfigAllPort : Form
    {
        public FormConfigAllPort()
        {
            InitializeComponent();
        }

        static int MAX_PORT = TotalConfigProject.LoadConfig("PlcScan", "Config", "MaxPorts", 256);

        static bool IsVersionEqualOrHigher(int file_major, int file_minor, int file_build, int file_revision, int major, int minor, int build, int revision)
        {
            if (file_major > major) return true;
            if (file_major < major) return false;
            if (file_minor > minor) return true;
            if (file_minor < minor) return false;
            if (file_build > build) return true;
            if (file_build < build) return false;
            if (file_revision >= revision) return true;

            return false;
        }

        public static void CommDeviceRead(PORT_STRUCT pt)
        {
            LoadHashedConfig(pt);

            string filename;

            filename = String.Format("{0}\\SCAN\\SCAN.{1:000}", TotalConfig.sDirWorkProject, pt.no);

            if (!File.Exists(filename)) return;

            TextReader reader = new StreamReader(filename, Encoding.Default);

            if (reader == null)
            {
                MessageBox.Show("파일을 읽을 수 없습니다.", filename);
                return;
            }

            CommaBlockString commaBuf = new CommaBlockString();
            string one_line;
            string imsi = "";
            sbyte flag = 0;

            // FileVersion 10.3.3 버전까지는 버전정보가 파일에 없다.  파일 정보가 없는것은 10.3.3 이하 버전이다.
            int FileVersionMajor = 10;
            int FileVersionMinor = 3;
            int FileVersionBuild = 3;

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                if (one_line.Length == 0) continue;

                commaBuf.Set(one_line);

                commaBuf.GetString(ref imsi);

                if (imsi == "ACTIVE")
                {
                    string on_off = "";
                    commaBuf.GetString(ref on_off);
                    if (on_off == "ON")
                    {
                        pt.bActiveFlag = true;
                    }
                    else if (on_off == "OFF")
                    {
                        pt.bActiveFlag = false;
                    }
                }
                else if (String.Compare(imsi, "BUF_LENGTH") == 0)
                {
                    commaBuf.GetInt(ref pt.nBufSizeWORD);
                }
                else if (String.Compare(imsi, "BUF_LENGTH_FLOAT") == 0)
                {
                    commaBuf.GetInt(ref pt.nBufSizeFLOAT);
                }
                else if (String.Compare(imsi, "BUF_LENGTH_DWORD") == 0)
                {
                    commaBuf.GetInt(ref pt.nBufSizeDWORD);
                }
                else if (String.Compare(imsi, "BUF_LENGTH_STRING") == 0)
                {
                    commaBuf.GetInt(ref pt.nBufSizeSTRING);
                }
                else if (String.Compare(imsi, "BUF_LENGTH_DOUBLE") == 0)
                {
                    commaBuf.GetInt(ref pt.nBufSizeDOUBLE);
                }
                else if (String.Compare(imsi, "BUF_LENGTH_INT64") == 0)
                {
                    commaBuf.GetInt(ref pt.nBufSizeINT64);
                }
                else if (String.Compare(imsi, "MAX_TIME_OUT_READ") == 0)
                {
                    commaBuf.GetInt(ref pt.MAX_TIME_OUT_READ);
                    pt.TIMEOUT_MILLI_READ = pt.MAX_TIME_OUT_READ*1000;
                }
                else if (String.Compare(imsi, "MAX_TIME_OUT_WRITE") == 0)
                {
                    commaBuf.GetInt(ref pt.MAX_TIME_OUT_WRITE);
                    pt.TIMEOUT_MILLI_WRITE = pt.MAX_TIME_OUT_WRITE*1000;
                }
                else if (String.Compare(imsi, "TIMEOUT_MILLI_READ") == 0)
                {
                    commaBuf.GetInt(ref pt.TIMEOUT_MILLI_READ);
                }
                else if (String.Compare(imsi, "TIMEOUT_MILLI_WRITE") == 0)
                {
                    commaBuf.GetInt(ref pt.TIMEOUT_MILLI_WRITE);
                }
                else if (imsi == "TITLE")
                {
                    commaBuf.GetString(ref pt.sTitle);
                }
                else if (imsi == "DEVICE")
                {
                    commaBuf.GetStringTotalRemain(ref pt.sScanDevice);
                }
                else if (imsi == "PROTOCOL")
                {
                    commaBuf.GetString(ref pt.sScanProtocol);
                    commaBuf.GetStringTotalRemain(ref pt.sScanProtocolOption[0]);
                }

                else if (String.Compare(imsi, "TELNUMBER") == 0)
                {
                    commaBuf.GetString(ref pt.sTelNumber);
                }
                else if (String.Compare(imsi, "TelConnectCicle") == 0)
                {
                    commaBuf.GetInt(ref pt.nTelConnectCycle);
                }
                else if (String.Compare(imsi, "TelConnectingTime") == 0)
                {
                    commaBuf.GetInt(ref pt.nTelConnectingTime);
                }
                else if (String.Compare(imsi, "TelAutoConnection") == 0)
                {
                    commaBuf.GetChar(ref flag);
                    pt.bTelAutoConnection = (flag == 1);
                }
                else if (String.Compare(imsi, "TelConnectingTimeOnManual") == 0)
                {
                    commaBuf.GetInt(ref pt.nTelConnectingTimeOnManual);
                }
                else if (String.Compare(imsi, "DualActive", true) == 0)
                {
                    commaBuf.GetChar(ref flag);
                    pt.bDualActive = (flag == 1);
                }
                else if (String.Compare(imsi, "DualDevice", true) == 0)
                {
                    commaBuf.GetStringTotalRemain(ref pt.sDualDevice);
                }
                else if (String.Compare(imsi, "DualCauseTimeOut", true) == 0)
                {
                    commaBuf.GetInt(ref pt.nDualCauseTimeOut);
                }
                else if (String.Compare(imsi, "DualCauseCodeBad", true) == 0)
                {
                    commaBuf.GetInt(ref pt.nDualCauseCodeBad);
                }
                else if (String.Compare(imsi, "DualProtocol", true) == 0)
                {
                    commaBuf.GetString(ref pt.sDualProtocol);

                    // 10.3.4 이상 버전의 파일은 이중화용 프로토콜 옵션이 있다. 이전버전은 기본을 복사해서 사용한다.
                    if (IsVersionEqualOrHigher(FileVersionMajor, FileVersionMinor, FileVersionBuild, 0, 10, 3, 4, 0))
                        commaBuf.GetStringTotalRemain(ref pt.sScanProtocolOption[1]);
                    else
                        pt.sScanProtocolOption[1] = pt.sScanProtocolOption[0];
                }
                else if (String.Compare(imsi, "DualUseProtocol", true) == 0)
                {
                    commaBuf.GetChar(ref flag);
                    pt.bDualUseProtocol = (flag == 1);
                }
                else if (String.Compare(imsi, "ScanTime") == 0 ||	// 구 버전
                        String.Compare(imsi, "ScanTimeRead") == 0)
                {
                    commaBuf.GetInt(ref pt.nLocalReadScanTime);
                }
                else if (String.Compare(imsi, "ScanTimeWrite") == 0)
                {
                    commaBuf.GetInt(ref pt.nLocalWriteScanTime);
                }
                else if (String.Compare(imsi, "ActiveThread", true) == 0)
                {
                    commaBuf.GetChar(ref flag);
                    pt.bActiveThread = (flag == 1);
                }
                else if (String.Compare(imsi, "ThreadCycle", true) == 0)
                {
                    commaBuf.GetInt(ref pt.nThreadCycle);
                }
                else if (String.Compare(imsi, "ComputerDual", true) == 0)
                {
                    commaBuf.GetChar(ref flag);
                    pt.bComputerDualActive = (flag == 1);
                    commaBuf.GetChar(ref flag);
                    pt.bComputerDualThread = (flag == 1);
                    commaBuf.GetString(ref pt.sComputerDualIP);
                    commaBuf.GetInt(ref pt.nComputerDualPort);
                    commaBuf.GetInt(ref pt.nComputerDualTimeout);
                }
                else if (String.Compare(imsi, "UseStationInfo", true) == 0)
                {
                    commaBuf.GetChar(ref flag);
                    pt.bUseStationInfo = (flag == 1);
                }
                else if (String.Compare(imsi, "UseDeviceInfo", true) == 0)
                {
                    commaBuf.GetChar(ref flag);
                    pt.bUseDeviceInfo = (flag == 1);
                }

                else if (String.Compare(imsi, "READ", true) == 0 ||
                        String.Compare(imsi, ";READ", true) == 0)
                {
                    if (pt.sEditBuf.Length > 0) pt.sEditBuf += "\r\n";

                    pt.sEditBuf += one_line;
                }
                else if (String.Compare(imsi, "FLOAT", true) == 0 ||
                        String.Compare(imsi, ";FLOAT", true) == 0)
                {
                    if (pt.sEditBuf.Length > 0) pt.sEditBuf += "\r\n";

                    pt.sEditBuf += one_line;
                }
                else if (String.Compare(imsi, "DWORD", true) == 0 ||
                        String.Compare(imsi, ";DWORD", true) == 0)
                {
                    if (pt.sEditBuf.Length > 0) pt.sEditBuf += "\r\n";

                    pt.sEditBuf += one_line;
                }
                else if (String.Compare(imsi, "STRING", true) == 0 ||
                        String.Compare(imsi, ";STRING", true) == 0)
                {
                    if (pt.sEditBuf.Length > 0) pt.sEditBuf += "\r\n";

                    pt.sEditBuf += one_line;
                }
                else if (String.Compare(imsi, "DOUBLE", true) == 0 ||
                        String.Compare(imsi, ";DOUBLE", true) == 0)
                {
                    if (pt.sEditBuf.Length > 0) pt.sEditBuf += "\r\n";

                    pt.sEditBuf += one_line;
                }
                else if (String.Compare(imsi, "INT64", true) == 0 ||
                        String.Compare(imsi, ";INT64", true) == 0)
                {
                    if (pt.sEditBuf.Length > 0) pt.sEditBuf += "\r\n";

                    pt.sEditBuf += one_line;
                }
                else if (String.Compare(imsi, "FileVersion", true) == 0)
                {
                    string version;
                    version = commaBuf.GetString();
                    CommaBlockString comma_v = new CommaBlockString();
                    comma_v.SetBlockCode('.');
                    comma_v.Set(version);
                    FileVersionMajor = comma_v.GetInt();
                    FileVersionMinor = comma_v.GetInt();
                    FileVersionBuild = comma_v.GetInt();
                }
            }

            reader.Close();

            
        }

        static void LoadHashedConfig(PORT_STRUCT pt)
        {
            string filename;

            filename = String.Format("{0}\\SCAN\\Config_{1:000}.cfg", TotalConfig.sDirWorkProject, pt.no);

            if (!File.Exists(filename)) return;

            string one_line;
            CommaTextReader comma = new CommaTextReader();
            string command;
            CryptoTextReader cfr;

            try
            {
                cfr = new CryptoTextReader(filename);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, filename);
                return;
            }

            while (true)
            {
                one_line = cfr.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);
                command = comma.GetString();

                if(pt.ec.IsCommand(command))
                    pt.ec.Load(cfr);
            }
            cfr.Close();
        }

        void ScanPortInitOne(PORT_STRUCT pt)
        {
            CommDeviceRead(pt);
        }

        void SetListItem(PORT_STRUCT pt, ListViewItem lvi)
        {
            lvi.SubItems[0].Text = String.Format("{0:000}", pt.no);
            if (pt.bActiveFlag)
            {
                lvi.SubItems[1].Text = pt.sTitle;
                lvi.SubItems[2].Text = pt.sScanDevice;
                lvi.SubItems[3].Text = pt.sScanProtocol;
            }
            else
            {
                lvi.SubItems[1].Text = "";
                lvi.SubItems[2].Text = "Not Used";
                lvi.SubItems[3].Text = "";
            }
        }

        PORT_STRUCT[] ports = new PORT_STRUCT[MAX_PORT];

        private void FormConfigAllPort_Load(object sender, EventArgs e)
        {
            ListViewItem lvi;

            for (int i = 0; i < MAX_PORT; i++)
            {
                ports[i] = new PORT_STRUCT();
                ports[i].no = i;

                ScanPortInitOne(ports[i]);

                lvi = new ListViewItem("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                SetListItem(ports[i], lvi);

                this.listView1.Items.Add(lvi);
            }

            FutureVersion.FormConfigCompactVersion.FillComboCompactProgramVersion(this.comboBoxProgramVersion);

            this.groupBoxDllDeployment.Enabled = TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Modify();
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            Modify();
        }

        void Modify()
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                if(Tools.IsLangKorean())
                    MessageBox.Show("수정할 항목을 선택하세요.", "선택 오류");
                else
                    MessageBox.Show("Select the item to modify.", "Modify error");

                return;
            }

            int index = this.listView1.SelectedItems[0].Index;
            PORT_STRUCT pt = ports[index];

            FormConfigPort dialog = new FormConfigPort();

            dialog.nPort = index;
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ports[index].bEditChanged = true;
                CommDeviceRead(ports[index]);
                SetListItem(ports[index], this.listView1.Items[index]);
            }
        }

        bool SendCommandToProcess(string processname, EnumIdmPublic wParam, int lParam)
        {
            Process p = StudioMain.SeekProcess(processname);

            if (p == null)
            {
                string filename = String.Format("{0}\\{1}.exe", Application.StartupPath, processname);

                try
                {
                    System.Diagnostics.Process.Start(filename);
                }
                catch (Exception exception)
                {
                    if (Tools.IsLangKorean())
                    {
                        System.Windows.Forms.MessageBox.Show(exception.Message, "실행 오류");
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(exception.Message, "Execute Error");
                    }
                }

                return true;
            }

            if (p != null)
            {
                Win32Function.PostMessage(p.MainWindowHandle, 0x111, (IntPtr)(int)wParam, (IntPtr)lParam);
            }

            return true;
        }

        void RestartPort4CE(int port)
        {
            RemoteApi remote = new RemoteApi();

            if (!remote.Init(true)) return;

            PORT_STRUCT pt = ports[port];

            string target_dir = FormConfigCompactVersion.GetDeployTargetFolder();

            if (pt.bEditChanged)
            {
                string source_file;// = String.Format("{0}\\Scan\\Scan.{1:000}", TotalConfig.sDirWorkProject, port);
                string target_file;

                target_file = String.Format("{0}\\Project\\Scan", target_dir);
                remote.CreateDirectory(target_file);    // SCAN 폴더가 없는 경우를 대비해서 만든다.

                source_file = String.Format("{0}\\Scan\\Scan.{1:000}", TotalConfig.sDirWorkProject, port);
                target_file = String.Format("{0}\\Project\\Scan\\Scan.{1:000}", target_dir, port);

                if (!remote.CopyFileToDevice(source_file, target_file))
                {
                    string msg = String.Format("Can't copy the file\nSource={0}\nTarget={1}", source_file, target_file);
                    DialogResult result = MessageBox.Show(msg, "Copy error");

                    remote.Uninit();

                    return;
                }

                //filename = String.Format("{0}\\SCAN\\Config_{1:000}.cfg", TotalConfig.sDirWorkProject, pt.no);

                // Hash Config 파일 복사를 해준다.
                source_file = String.Format("{0}\\Scan\\Config_{1:000}.cfg", TotalConfig.sDirWorkProject, port);
                target_file = String.Format("{0}\\Project\\Scan\\Config_{1:000}.cfg", target_dir, port);

                if (!remote.CopyFileToDevice(source_file, target_file))
                {
                    string msg = String.Format("Can't copy the file\nSource={0}\nTarget={1}", source_file, target_file);
                    DialogResult result = MessageBox.Show(msg, "Copy error");

                    remote.Uninit();

                    return;
                }

                pt.bEditChanged = false;
            }

            string filename = String.Format("{0}\\Runtime\\CompactScan.exe", target_dir);
            remote.CreateProcess(filename, "PortReset=" + (port + 5000).ToString());

            remote.Uninit();
        }

        /* 프로토콜을 복사하고 통신 프로그램을 재시작하지 않는다. 2016-12-01
        void ProtocolListReLoad4CE()
        {
            RemoteApi remote = new RemoteApi();

            if (!remote.Init(true)) return;

            string target_dir = FormConfigCompactVersion.GetDeployTargetFolder();

            string filename = String.Format("{0}\\Runtime\\CompactScan.exe", target_dir);
            remote.CreateProcess(filename, "ProtocolListReLoad");

            remote.Uninit();
        }*/

        private void buttonPortRestart_Click(object sender, EventArgs e)
        {
            if(this.listView1.SelectedItems.Count == 0) {
                if(Tools.IsLangKorean())
                    MessageBox.Show("리셋할 포트를 선택하세요.", "선택 오류");
                else
                    MessageBox.Show("Select the port to reset.", "Selection error");
                return;
            }

            int port = this.listView1.SelectedItems[0].Index;

            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
            {
                RestartPort4CE(port);
            }
            else
            {
                SendCommandToProcess("PLC_SCAN", EnumIdmPublic.IDM_PUBLIC_PLC_SCAN_PORT_RESTART, 5000 + port);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

        void DeployDll(string dll_file)
        {
            RemoteApi remote = new RemoteApi();

            string target_dir = FormConfigCompactVersion.GetDeployTargetFolder();
            string source_dir = "C:\\Autobase.Smart\\"+this.comboBoxProgramVersion.Text;

            string source_file = String.Format("{0}\\Protocol\\{1}", source_dir, dll_file);

            if (!File.Exists(source_file))
            {
                MessageBox.Show(source_file, "Dll File not exist");
                return;
            }

            string target_file = String.Format("{0}\\Runtime\\Protocol\\{1}", target_dir, dll_file);

            if (!remote.Init(true)) return;

            remote.CreateDirectory(target_dir + "\\Runtime\\Protocol");

            if (!remote.CopyFileToDevice(source_file, target_file))
            {
                string msg;

                if (Tools.IsLangKorean())
                {
                    msg = String.Format("파일을 복사할 수 없습니다.\nSource={0}\nTarget={1}\n\nCE장치에서 'CompactScan.exe' 가 실행중이면 종료하고 복사하세요.", source_file, target_file);
                    DialogResult result = MessageBox.Show(msg, "복사 오류");
                }
                else
                {
                    msg = String.Format("Can't copy the file\nSource={0}\nTarget={1}\n\nIf 'CompactScan.exe' is running, exit the program.", source_file, target_file);
                    DialogResult result = MessageBox.Show(msg, "Copy error");
                }

                remote.Uninit();

                return;
            }

            remote.Uninit();

            if(Tools.IsLangKorean())
                MessageBox.Show(dll_file, "DLL 배포 완료");
            else
                MessageBox.Show(dll_file, "Deployment Succeeded.");
        }

        private void buttonDeploy_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the port to deploy.", "Selection error");
                return;
            }

            int port = this.listView1.SelectedItems[0].Index;

            ArrayList block = FormConfigPort.LoadProtocolList();

            DLL_PROTOCOL_LIST item;

            for (int i = 0; i < block.Count; i++)
            {
                item = (DLL_PROTOCOL_LIST)block[i];

                if (item.bProtocol == 1)
                {
                    if (ports[port].sScanProtocol == "DLL-"+item.title)
                    {
                        DeployDll("Smart"+item.filename);
                        goto matched;
                    }
                }
            }

            MessageBox.Show("Protocol not found in Protocol List", ports[port].sScanProtocol);

        matched: ;

            //프로토콜을 복사하고 통신 프로그램을 재시작하지 않는다. 2016-12-01
            //ProtocolListReLoad4CE();
        }
    }

    public class PORT_STRUCT{
        public int no;
        public bool bActiveFlag = false;
        public string sTitle;
        public string sScanDevice;
        public string sScanProtocol;
        public string[] sScanProtocolOption = new string[2];

        public int nBufSizeWORD=200;
        public int nBufSizeFLOAT;
        public int nBufSizeDWORD;
        public int nBufSizeSTRING;
        public int nBufSizeDOUBLE;
        public int nBufSizeINT64;

        public int MAX_TIME_OUT_READ;
        public int MAX_TIME_OUT_WRITE;

        public int TIMEOUT_MILLI_READ;
        public int TIMEOUT_MILLI_WRITE;

        public string sTelNumber;
        public int nTelConnectCycle;
        public int nTelConnectingTime;
        public bool bTelAutoConnection;
        public int nTelConnectingTimeOnManual;

        public int nLocalReadScanTime;
        public int nLocalWriteScanTime;

        public bool bDualActive;
        public string sDualDevice;
        public int nDualCauseTimeOut;
        public int nDualCauseCodeBad;
        public bool bDualUseProtocol;
        public string sDualProtocol;

        public bool bActiveThread=true;
        public int nThreadCycle;

        public bool bComputerDualActive;
        public bool bComputerDualThread;
        public string sComputerDualIP;
        public int nComputerDualPort;
        public int nComputerDualTimeout;

        public bool bUseStationInfo;
        public bool bUseDeviceInfo;

        public string sEditBuf = "";

        public bool bEditChanged;

        public CryptoCommunication ec = new CryptoCommunication();
    }

    
}

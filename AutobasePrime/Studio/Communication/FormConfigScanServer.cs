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

namespace Studio.Communication
{
    public partial class FormConfigScanServer : Form
    {
        public FormConfigScanServer()
        {
            InitializeComponent();
        }

        const int MAX_SCAN_SERVER_LIST = 256;
        
        public static void ScanServerReadOne(SCAN_SERVER_LIST conn)
        {
            
            string filename;

            string buf = "";

            filename = String.Format("{0}\\SCAN\\SERVER\\serv{1:0000}.ini", TotalConfig.sDirWorkProject, conn.struct_no);

            // 접속 방법 찾기.
            Profile.GetPrivateProfileStringA("config", "Connect Method", "None", ref buf, filename);

            if (String.Compare(buf, "NULL") == 0) conn.nDeviceType = 1;
            else if (String.Compare(buf, "Modem") == 0) conn.nDeviceType = 2;
            else if (String.Compare(buf, "TCP/IP") == 0) conn.nDeviceType = 3;
            else if (String.Compare(buf, "UDP/IP") == 0) conn.nDeviceType = 4;
            else conn.nDeviceType = 0;	// null modem

            Profile.GetPrivateProfileStringA("config", "Thread", "1", ref buf, filename);
            conn.bThreadFlag = (buf == "1");

            conn.nSendDelay = Profile.GetPrivateProfileIntA("config", "nSendDelay", 0, filename);
            //conn.bSendOnlyChange = Profile.GetPrivateProfileInt("config", "bSendOnlyChange", 0, filename);
            conn.nSendBlockSize = Profile.GetPrivateProfileIntA("config", "nSendBlockSize", 500, filename);

            if (conn.nSendBlockSize < 0) conn.nSendBlockSize = 1;
            if (conn.nSendBlockSize > 500) conn.nSendBlockSize = 500;

            // 모뎀 설정 찾기
            conn.sComString = "";
            Profile.GetPrivateProfileStringA("Modem", "port", "COM1", ref buf, filename);
            conn.sComString += buf + ",";

            Profile.GetPrivateProfileStringA("Modem", "baud", "19200", ref buf, filename);
            conn.sComString += buf + ",";

            Profile.GetPrivateProfileStringA("Modem", "parity", "0", ref buf, filename);
            conn.sComString += buf + ",";

            Profile.GetPrivateProfileStringA("Modem", "data", "8", ref buf, filename);
            conn.sComString += buf + ",";

            Profile.GetPrivateProfileStringA("Modem", "stop", "1", ref buf, filename);
            conn.sComString += buf;

            Profile.GetPrivateProfileStringA("Modem", "Initial Command", "AT &C1 B0", ref buf, filename);
            conn.sModemInitCommand = buf;

            string default_port;

            default_port = String.Format("{0}", 6000 + conn.struct_no);

            Profile.GetPrivateProfileStringA("tcp/ip", "port", default_port, ref buf, filename);
            conn.tcpipPort = ConvertTool.ToInt32(buf);
        }

        void ScanPortInitOne(SCAN_SERVER_LIST pt)
        {
            ScanServerReadOne(pt);
        }

        void SetListItem(SCAN_SERVER_LIST pt, ListViewItem lvi)
        {
            lvi.SubItems[0].Text = String.Format("{0:000}", pt.struct_no);

            string buf;
            if (pt.nDeviceType == 1)
            {
                buf = pt.sComString;
            }
            else if (pt.nDeviceType == 2)
            {
                buf = "Modem " + pt.sComString;
            }
            else if (pt.nDeviceType == 3)
            {
                buf = "TCP/IP, Port=" + pt.tcpipPort.ToString();
            }
            else if (pt.nDeviceType == 4)
            {
                buf = "UDP/IP, Port=" + pt.tcpipPort.ToString();
            }
            else
            {
                buf = "Not Used";
            }

            lvi.SubItems[1].Text = buf;

            if (pt.nDeviceType == 0)
            {
                lvi.SubItems[2].Text = "";
                lvi.SubItems[3].Text = "";
            }
            else
            {
                lvi.SubItems[2].Text = pt.nSendDelay.ToString();
                lvi.SubItems[3].Text = pt.nSendBlockSize.ToString();
            }
        }

        SCAN_SERVER_LIST[] ports = new SCAN_SERVER_LIST[MAX_SCAN_SERVER_LIST];

        private void FormConfigAllPort_Load(object sender, EventArgs e)
        {
            ListViewItem lvi;

            for (int i = 0; i < MAX_SCAN_SERVER_LIST; i++)
            {
                ports[i] = new SCAN_SERVER_LIST();
                ports[i].struct_no = i;

                ScanPortInitOne(ports[i]);

                lvi = new ListViewItem("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                SetListItem(ports[i], lvi);

                this.listView1.Items.Add(lvi);
            }
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
                MessageBox.Show("Select the item to modify.", "Modify error");
            }

            int index = this.listView1.SelectedItems[0].Index;
            SCAN_SERVER_LIST pt = ports[index];

            FormConfigScanServerOne dialog = new FormConfigScanServerOne();

            dialog.nPort = index;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ports[index].bEditChanged = true;
                ScanServerReadOne(ports[index]);
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

            SCAN_SERVER_LIST pt = ports[port];

            string target_dir = FormConfigCompactVersion.GetDeployTargetFolder();

            if (pt.bEditChanged)
            {
                string source_file = String.Format("{0}\\Scan\\SERVER\\serv{1:0000}.ini", TotalConfig.sDirWorkProject, port);
                string target_file = String.Format("{0}\\Project\\Scan\\SERVER\\serv{1:0000}.ini", target_dir, port);

                string config_dir = String.Format("{0}\\Project\\Scan\\SERVER", target_dir);
                remote.CreateDirectory(config_dir);    // SCAN\SERVER 폴더가 없는 경우를 대비해서 만든다.

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
            remote.CreateProcess(filename, "ScanServerReset=" + (port + 5000).ToString());

            remote.Uninit();
        }

        private void buttonPortRestart_Click(object sender, EventArgs e)
        {
            if(this.listView1.SelectedItems.Count == 0) {
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
                SendCommandToProcess("PLC_SCAN", EnumIdmPublic.IDM_PUBLIC_PLC_SCAN_SCAN_SERVER_RESTART, 5000 + port);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }
    }

    public class SCAN_SERVER_LIST
    {
        public int struct_no;
        public int nDeviceType;				// 연결 장치 종류.
        //char bInitialFlag;				// 초기화가 되었느냐?

        public bool bThreadFlag;				// Thread를 사용하느냐?
        //HANDLE	hThread;
        //DWORD	idThread;
        //char	bThreadEnd;
        //char	bThreadDo;
        //char	bThreadPause;			// thread를 잠시정지한다.
        //char	bThreadPauseACK;		// thread function이 thread가 pause되었을 때 ON을 보내준다.

        public int nSendDelay;
        //public bool bSendOnlyChange;
        public int nSendBlockSize;			// 한번에 보내는 개수
        public string sComString;
        /*
	//TimeOutMiliSecClass timeoutSendDelay;

	//HGATE hGate;
	
	//char bConnectFlag;				// 상태편과 접속되었느냐?
	
	WORD	wCastPort[16];			// 공급해줄 포트.
	TimeOutClass timeoutDisconnect;	

	TimeOutClass timeoutTryInit;
	int  nTryInitCount;

	SOCKET sock_listen;				// tcp/ip 응답용 socket*/
        public int tcpipPort;
        /*
        MODEM_STRUCT modem;*/
        public string sModemInitCommand;
        /*
	int		nRecvHap;
	char	recvBuf[MAX_RECV_BUF];

	WORD	nReadPosWORD[256];
	WORD	nReadPosFLOAT[256];
	WORD	nReadPosDWORD[256];
	WORD	nReadPosSTRING[256];
	WORD	nReadPosDOUBLE[256];
	WORD	nReadPosINT64[256];

	bool	bSignal_FD_CLOSE;	// TCP socket Close 가 들어오면 이 플래그를 살려준다.

	short nVersionMajor;		// 상대편의 통신 드라이버 버전
	short nVersionMinor;		// 상대편의 통신 드라이버 버전

	//int		nBlockSize;*/
        public bool bEditChanged;
    }
    
}

using AutoLib;
using AutoLibLocal;
using AutoLibLocal.KeyLock;
using DatabaseConnection;
using DialogConfigUser;
using GraphicModule;
using HelpLib;
using LocalMain.Alarm;
using LocalMain.Barcode;
using LocalMain.DemandNew;
using LocalMain.OPCUA;
using LocalMain.PythonAi;
using NetTools;
using Opc.Ua;
using PublicStudioLocalMain.Recipe;
using PublicStudioLocalMain.Schedule;
using ReportModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ViewMainPublic;
using static AutoLibLocal.LanguageManager;

namespace LocalMain
{
    public partial class FormLocalMain : Form
    {
        public static SharedLocalMain sharedLocalMain;
        private LanguageManager _langManager;

        public FormLocalMain()
        {
            formMain = this;
            TotalConfig.threadMain = Thread.CurrentThread;
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                this.ShowIcon = false;
            }
            else if (TotalConfig.eOemType == EnumOemType.UYeG_GS || TotalConfig.eOemType == EnumOemType.UYeG_Normal)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("UYeG.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
            }
            else if (TotalConfig.eOemType == EnumOemType.OPEN_SCADA)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("OemOpenScadaLocalMain.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
            }
            else if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("SBAS.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
            }
            else if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("FT-LocalMain.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
            }
            else if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("MBSSCADA.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
            }
            else if (TotalConfig.eOemType == EnumOemType.KobasAI)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("KOBAS-AI.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
            }

            TotalConfig.bLocalMain = true;
            ConfigVarTotal.Init(null);
            SystemInformation.tStartup = DateTime.Now;

            // LanguageManager 인스턴스 가져오기 251031 PSU
            _langManager = LanguageManager.Instance;
            SmLog.InitializeLogProcessor(); //251001 PSU 로그시스템 초기화
            AlarmProcessor.Initialize(); //251001 PSU 경보저장시스템 초기화
            LanguageManager.EventLanguageChanged += OnLanguageChanged;
        }

        /// <summary>
        /// 언어 변경 이벤트 핸들러
        /// </summary>
        /// <param name="newLanguageCode">새로운 언어 코드</param>
        private void OnLanguageChanged(string newLanguageCode)
        {
            // 언어가 변경되면 텍스트 업데이트
            var culture = new CultureInfo(newLanguageCode);
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            ComponentResourceManager resources = new ComponentResourceManager(this.GetType());

            this.SuspendLayout();
            ApplyResourcesRecursive(resources, this, culture);
            this.ResumeLayout(true);
            this.Refresh();
        }

        private void ApplyResourcesRecursive(ComponentResourceManager res, Control ctrl, CultureInfo culture)
        {
            // 일반 컨트롤
            res.ApplyResources(ctrl, ctrl.Name, culture);

            // 자식 컨트롤 재귀
            foreach (Control child in ctrl.Controls)
                ApplyResourcesRecursive(res, child, culture);

            // MenuStrip 및 ToolStripMenuItem 처리A
            if (ctrl is MenuStrip menu)
            {
                foreach (ToolStripItem item in menu.Items)
                    ApplyResourcesToToolStripItem(res, item, culture);
            }
        }

        private void ApplyResourcesToToolStripItem(ComponentResourceManager res, ToolStripItem item, CultureInfo culture)
        {
            res.ApplyResources(item, item.Name, culture);

            if (item is ToolStripMenuItem menuItem)
            {
                foreach (ToolStripItem child in menuItem.DropDownItems)
                    ApplyResourcesToToolStripItem(res, child, culture);
            }
        }


        private async void FilelogInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await C_protect.AutoBaseLogIn();
        }

        private async void FilelogOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await C_protect.AutoBaseLogOut(true);
        }

        private void FilewriteMemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            C_mail.SendMail();
        }

        private void FilereadMemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            C_mail.ReadMail();
        }

        private void FileprintScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemFileScreenPrint_Click(this);
        }

        private void FilescreenSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemFileScreenSave_Click(this);
        }

        private void FilesaveSelectionZoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemFileSelectScreenSave_Click(this);
        }

        Process SeekProcess(string processname)
        {
            Process[] p;

            p = Process.GetProcessesByName(processname);

            if (p.Length > 0)
            {
                return p[0];
            }

            p = Process.GetProcessesByName(processname + ".vshost");
            if (p.Length > 0)
            {
                return p[0];
            }

            return null;
        }

        private void FilerunStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form child = this.ActiveMdiChild;

            string argument = "";

            if (child != null)
            {
                if (child.Name == "FormGraphicFrame")
                {
                    FormGraphicFrame form = (FormGraphicFrame)child;
                    argument = form.formChild.sFileName;
                }
                else if (child.Name == "FormReportMainRun")
                {
                    FormReportMainRun form = (FormReportMainRun)child;
                    argument = form.formChild.sFilename;
                }
                else if (String.Compare(child.Name, "ViewAnalogInputMain", true) == 0)
                {
                    argument = "local.tagx";
                }
                else if (String.Compare(child.Name, "ViewAnalogOutputMain", true) == 0)
                {
                    argument = "local.tagx";
                }
                else if (String.Compare(child.Name, "ViewDigitalInputMain", true) == 0)
                {
                    argument = "local.tagx";
                }
                else if (String.Compare(child.Name, "ViewDigitalOutputMain", true) == 0)
                {
                    argument = "local.tagx";
                }
                else if (String.Compare(child.Name, "ViewStringTagMain", true) == 0)
                {
                    argument = "local.tagx";
                }
                else if (String.Compare(child.Name, "ViewGroupTagListMainNew", true) == 0)
                {
                    argument = "local.tagx";
                }
                else if (String.Compare(child.Name, "FormAlwaysScript", true) == 0)
                {
                    argument = "test.ctlx";
                }
                else { }
            }

            CheckTagSave();	// 스튜디오를 호출하기전에 태그를 먼저 저장한다.

            ScriptFunctionTag.bTagSaveAll = false;	// TagSaveAll함수를 사용하지 않는다.
            // 사용하니까 스튜디오에서 태그로딩시 오류가 난다.

            //Process p = SeekProcess("Studio");

            //if (p == null)
            //{
            //    argument = '"' + argument + '"';	// 스페이스가 있는 파일명을 위해서 ""를 둘러싼다. .NET에서는 argument에서 자동으로 ""가 빠진다.

            //    string filename = Application.StartupPath + "\\Studio.exe";

            //    if (!File.Exists(filename))
            //    {
            //        if (Tools.IsLangKorean())
            //            MessageBox.Show(filename, "실행 파일이 존재하지 않습니다.");
            //        else
            //            MessageBox.Show(filename, "The exe file does not exist.");

            //        return;
            //    }

            //    if (argument.Length == 0)
            //        Process.Start(filename);
            //    else
            //        Process.Start(filename, argument);
            //}
            //else
            //{
            //    TotalConfig.SaveRegAutoBaseConfig("Execute", null, "ActiveDocument", argument);
            //    Win32Function.PostMessage(p.MainWindowHandle, 0x111, (int)EnumIdmPublic.IDM_PUBLIC_ACTIVE_DOCUMENT, 0);
            //    Win32Function.SetForegroundWindow(p.MainWindowHandle);
            //}

            //20250214 PSU 좀비프로세스 종료 추가 + Autobase Studio만 사용 
            string studioPath = Application.StartupPath + "\\Studio.exe";
            if (!File.Exists(studioPath))
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show(studioPath, "실행 파일이 존재하지 않습니다.");
                else
                    MessageBox.Show(studioPath, "The exe file does not exist.");
                return;
            }

            // 실행 가능한 정상 Studio 프로세스 찾기
            Process[] processes = Process.GetProcessesByName("Studio");
            Process normalProcess = null;

            foreach (Process p in processes)
            {
                try
                {
                    string processPath = p.MainModule.FileName;
                    if (processPath.Equals(studioPath, StringComparison.OrdinalIgnoreCase))
                    {
                        if (p.MainWindowHandle != IntPtr.Zero)
                        {
                            normalProcess = p;
                            break;
                        }
                        else
                        {
                            // 좀비 프로세스는 종료 시도
                            try
                            {
                                p.Kill();
                            }
                            catch
                            {
                                // 종료 실패시 무시
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (normalProcess == null)
            {
                // 정상 프로세스가 없으면 새로 실행
                argument = '"' + argument + '"';
                // 스페이스가 있는 파일명을 위해서 ""를 둘러싼다. .NET에서는 argument에서 자동으로 ""가 빠진다.

                if (argument.Length == 0)
                    Process.Start(studioPath);
                else
                    Process.Start(studioPath, argument);
            }
            else
            {
                // 정상 프로세스가 있으면 메시지 전달
                TotalConfig.SaveRegAutoBaseConfig("Execute", null, "ActiveDocument", argument);
                Win32Function.PostMessage(normalProcess.MainWindowHandle, 0x111, (IntPtr)(int)EnumIdmPublic.IDM_PUBLIC_ACTIVE_DOCUMENT, IntPtr.Zero);
                Win32Function.SetForegroundWindow(normalProcess.MainWindowHandle);
            }
        }

        public static bool bCloseByCallEditor = false;

        void CheckTagSave()
        {
            if (TagLib.bChangedByLocalMain && !DialogTag.TagEditor.Editor.checkTagFileChanged.IsChanged())
            {
                TerminalClass.SaveTag(TagLib.groupRoot);
            }
        }

        bool bExit = false;

        private void FileexitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bExit = true;
            Close();
        }

        private void FileSaveResultToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;
            if (this.ActiveMdiChild.GetType() == typeof(ReportModule.FormReportMainRun))
            {
                FormReportMainRun form = (FormReportMainRun)this.ActiveMdiChild;
                form.formChild.menuItemFileSaveResult_Click();
            }
        }

        async Task< bool> LogInOnProgramStart()
        {
            if (!ConfigViewMain.bDisplayLogInBoxOnStart) return true;	// 기능을 사용하지 않을때는 로그인 성공으로 표시

            FormLogIn login = new FormLogIn();
            login.StartPosition = FormStartPosition.CenterParent;

            if (login.ShowDialog(this) == DialogResult.OK)
            {
                await C_protect.LogInByUsername(login.textBoxUsername.Text);
                return true;
            }
            else
            {
                bFailLogInOnStart = true;
                Close();
                return false;
            }
        }

        public void InitNotifyIcon()
        {
            this.notifyIcon1.Visible = ConfigViewMain.bUseNotifyIcon;
        }

        void SetTitle()
        {
            string ver = TotalConfig.AutoBaseIntGetVersionString();

            string text;
            string oem = TotalConfig.AutoBaseIniGetOemProgramName();

            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                text = oem;
            }
            else if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                if (TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite && oem.IndexOf("Lite") == -1)
                {
                    oem += " Lite";
                }

                if (Tools.IsLangKorean())
                    text = String.Format("{0} {1}", oem, ver);
                else
                    text = String.Format("{0} {1}", oem, ver);
            }
            else
            {
                if (TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite && oem.IndexOf("Lite") == -1)
                {
                    oem += " Lite";
                }

                if (Tools.IsLangKorean())
                    text = String.Format("{0} 감시 {1}", oem, ver);
                else if (Tools.IsLangJapanese())
                    text = String.Format("{0} 監視 {1}", oem, ver);
                else if (Tools.IsLangChinese())
                    text = String.Format("{0} 监视 {1}", oem, ver);
                else
                    text = String.Format("{0} SCADA Server {1}", oem, ver);
            }

            if (SharedLocalMain.bTestMode)
            {
                text += " (Test Mode)";
            }

            if (bDebugMode)
            {
                text += " (Debug Mode)";
            }

            //if(ConfigViewMain.bShowMainTitle)
                this.Text = text;

            this.notifyIcon1.Text = text;
        }

        public static string[] mainArgs;
        public static FormLocalMain formMain;
        public static bool bDebugMode = false;

        void ExecuteArgument()
        {
            if (mainArgs.Length > 0)
            {
                string argument = mainArgs[0];
                string ext = Path.GetExtension(argument);

                if (String.Compare(ext, ".modx", true) == 0 || String.Compare(ext, ".mod", true) == 0)
                {
                    string path = argument;
                    if (Path.GetPathRoot(argument).Length > 0)
                        path = argument;
                    else
                        path = String.Format("{0}\\graphic\\{1}", TotalConfig.sDirWorkProject, argument);

                    if (File.Exists(path))
                    {
                        GraphicTool.RestoreGraphicWindow(path, -1, 0, 0);
                        return;
                    }
                }
                else if (String.Compare(ext, ".rptx", true) == 0 || String.Compare(ext, ".rpt", true) == 0)
                {
                    string path = argument;
                    if (Path.GetPathRoot(argument).Length > 0)
                        path = argument;
                    else
                        path = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, argument);

                    if (File.Exists(path))
                    {
                        FormDialogOpenReport.OpenReportAtViewMain(path);
                        return;
                    }
                }
                else if (String.Compare(ext, ".ctlx", true) == 0)
                {
                    if (FormAlwaysScript.ringScript.OneMdiCheck()) return;

                    FormAlwaysScript.ringScript.OneMdiCreate(this, new FormAlwaysScript());
                }
                else if (String.Compare(ext, ".tagx", true) == 0)
                {
                    ViewMainPublic.PublicMenu.menuItemViewAllTagView_Click(this);
                }
                else
                {

                }
            }
            else
            {
                if (SharedData.userInfo.bAutoOpenStartPage)
                {
                    string path = String.Format("{0}\\graphic\\{1}", TotalConfig.sDirWorkProject, SharedData.userInfo.sStartPage);
                    GraphicTool.RestoreGraphicWindow(path, -1, 0, 0);
                }
                else
                    ViewMainPublic.PublicMenu.menuItemViewGraphic_Click();	// 사용자 그래픽을 보여준다.
            }
        }

        void CheckTagOverFlow()
        {
            if (SharedLocalMain.bTestMode) return;

            int tag_used = 0;
            int tag_size = KeyLock.nTagSize;
            string serial_number = KeyLock.sSerialNumber;
            tag_used = FormInformationKeyLock.KeyLockGetTotalTagUsed();

            if (tag_size == 0) return;	// 무제한 태그
            if (tag_used <= tag_size) return;

            string buf;

            if (Tools.IsLangKorean())
            {
                buf = String.Format("태그의 사용개수가 제한개수를 넘었습니다.\n테스트 모드로 동작합니다.\n\n태그 제한개수 = {0}\n현재 사용개수 = {1}\n\nS/N:{2}",
                    tag_size, tag_used, serial_number);
                MessageBox.Show(buf, "태그 사용개수 초과");
            }
            else if (Tools.IsLangJapanese())
            {
                buf = String.Format("タグの使用個数が制限個数を超えました.\nテストモードに動作します。\n\nタグ制限数 = {0}\n現在使用数 = {1}\n\nS/N:{2}",
                    tag_size, tag_used, serial_number);
                MessageBox.Show(buf, "태그 사용개수 초과");
            }
            else if (Tools.IsLangChinese())
            {
                buf = String.Format("标记个数已超过了限制个数.\n要运行为测试模式.\n\n标记限制个数 = {0}\n当前使用个数 = {1}\n\nS/N:{2}",
                    tag_size, tag_used, serial_number);
                MessageBox.Show(buf, "超过了标记使用个数");
            }
            else
            {
                buf = String.Format("Used TAG count over.\nProgram run by TestMode.\n\nUsable Tag size = {0}\nCurrent Tag count = {1}\n\nS/N:{2}",
                    tag_size, tag_used, serial_number);
                MessageBox.Show(buf, "Tag count over");
            }

            SharedLocalMain.bTestMode = true;
            GraphicModule.SharedLocalMain.bTestMode = true; //20241010 PSU

            Log.Write(LogLevel.ERROR, LogCategory.SYSTEM, "TestMode Start by Tag-count over"); //20241010 PSU
        }

        void CheckKeyLockVersionLow()
        {
            if (SharedLocalMain.bTestMode) return;

            int tag_color = KeyLock.nKeyLockVersion;
            string serial_number = KeyLock.sSerialNumber;

            // 2005년1월1일부터는 KeyLock Version 6을 출시
            // 2006년도부터는 Version 6

            if (tag_color > 7) return;	    // 2008년 출시된 키부터 프라임을 사용할 수 있다.

            string buf;

            if (Tools.IsLangKorean())
            {
                buf = String.Format("구버전에서 사용하는 키락입니다.\n이 키락은 현재버전에서 사용할 수 없습니다.\n테스트 모드로 동작합니다.\n\n키락 버전 = {0}\n\nS/N:{1}",
                    tag_color, serial_number);
                MessageBox.Show(buf, "구키락 사용불가");
            }
            else if (Tools.IsLangChinese())
            {
                buf = String.Format("这是旧版本上使用的keylock。 /n此keylock不能使用在此版本。/n运行为测试模式。\n\n키락 버전 = {0}\n\nS/N:{1}",
                    tag_color, serial_number);
                MessageBox.Show(buf, "不可用旧keylock");
            }
            else
            {
                buf = String.Format("Old version Keylock.\nProgram run by TestMode.\n\nKeyLock Version = {0}\n\nS/N:{1}",
                    tag_color, serial_number);
                MessageBox.Show(buf, "Old Keylock");
            }

            SharedLocalMain.bTestMode = true;
            GraphicModule.SharedLocalMain.bTestMode = true; //20241010 PSU

            Log.Write(LogLevel.ERROR, LogCategory.SYSTEM, "TestMode Start by Old Keylock"); //20241010 PSU
        }

        //void RestoreMainFormLocationAndSize()
        //{
        //    if (!ConfigViewMain.bRestoreLocationSizeOnStartup) return;

        //    bool maximized = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "Maximized", true);

        //    if (maximized) return;

        //    int x = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "LocationX", 10);
        //    int y = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "LocationY", 10);
        //    int w = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "Width", 640);
        //    int h = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "Height", 480);

        //    if (x < 0 || y < 0) return; // minimized 되면 x,y가모두 -32000일 경우가 있다.

        //    Point p = new Point(x, y);
        //    Size s = new Size(w, h);

        //    this.WindowState = FormWindowState.Normal;
        //    this.Location = p;
        //    this.Size = s;

        //    //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Maximized", (this.WindowState == FormWindowState.Maximized));
        //    //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "LocationX", p.X);
        //    //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "LocationY", p.Y);
        //    //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Width", s.Width);
        //    //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Height", s.Height);
        //}

        //전체화면 시 적용안되는 점 수정 20241010 PSU
        void RestoreMainFormLocationAndSize()
        {
           // if (!ConfigViewMain.bRestoreLocationSizeOnStartup) return;

            // 전체 화면 설정 해제
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.Sizable;


            bool maximized = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "Maximized", true);

            //if (maximized) return;

            int x = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "LocationX", 10);
            int y = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "LocationY", 10);
            int w = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "Width", 640);
            int h = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "Height", 480);

            int monitorIndex = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "MonitorIndex", 0);

            //if (x < 0 || y < 0) return; // minimized 되면 x,y가모두 -32000일 경우가 있다.
            if (x == -32000 && y == -32000) return; // minimized 되면 x,y가모두 -32000일 경우가 있다.

            // 저장된 모니터 인덱스가 유효한지 확인합니다.
            if (monitorIndex >= 0 && monitorIndex < Screen.AllScreens.Length)
            {
                Screen targetScreen = Screen.AllScreens[monitorIndex];
                this.Location = targetScreen.Bounds.Location;

                // 저장된 위치가 대상 모니터 내에 있는지 확인하고 조정합니다.
                //x = Math.Max(targetScreen.WorkingArea.Left, Math.Min(x, targetScreen.WorkingArea.Right - w));
                //y = Math.Max(targetScreen.WorkingArea.Top, Math.Min(y, targetScreen.WorkingArea.Bottom - h));
            }
            else
            {
                // 유효하지 않은 모니터 인덱스인 경우 주 모니터를 사용합니다.
                Screen primaryScreen = Screen.PrimaryScreen;
                x = Math.Max(primaryScreen.WorkingArea.Left, Math.Min(x, primaryScreen.WorkingArea.Right - w));
                y = Math.Max(primaryScreen.WorkingArea.Top, Math.Min(y, primaryScreen.WorkingArea.Bottom - h));
            }


            Point p = new Point(x, y);
            Size s = new Size(w, h);

            this.WindowState = FormWindowState.Normal;
            this.Location = p;
            this.Size = s;

            if (maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Maximized", (this.WindowState == FormWindowState.Maximized));
            //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "LocationX", p.X);
            //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "LocationY", p.Y);
            //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Width", s.Width);
            //TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Height", s.Height);
        }

        //LocalMain 시작모니터 지정 추가 20241010 PSU
        private void SetFormLocation()
        {
            //// **전체화면 모드(Maximized)에서는 운영체제가 창의 크기와 위치를 직접 제어하여, Location 속성 설정이 무시된다.
            //// 현재 폼의 상태를 저장
            //FormWindowState originalState = this.WindowState;
            //FormBorderStyle originalStyle = this.FormBorderStyle;

            //// 전체 화면 설정 해제
            //this.WindowState = FormWindowState.Normal;
            //this.FormBorderStyle = FormBorderStyle.Sizable;

            //// 원하는 모니터의 Screen 객체를 가져옵니다.
            //int monitorIndex = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "StartMonitorIndex", 0);
            //Screen targetScreen;

            //// monitorIndex가 유효한지 확인
            //if (monitorIndex >= 0 && monitorIndex < Screen.AllScreens.Length)
            //{
            //    targetScreen = Screen.AllScreens[monitorIndex];
            //}
            //else
            //{
            //    // 유효하지 않은 경우 주 모니터로 설정
            //    targetScreen = Screen.PrimaryScreen;
            //}

            //// Form의 위치를 설정합니다.
            //this.Location = targetScreen.Bounds.Location;

            //// 전체 화면으로 다시 설정
            //this.WindowState = FormWindowState.Maximized;
            //this.FormBorderStyle = FormBorderStyle.None;

            //// 원래 상태로 복원 (필요한 경우)
            ////this.WindowState = originalState;
            //this.FormBorderStyle = originalStyle;

            //FormLocalMain 창 maximized -> normal 로 변경. 260220 PSU.
            int monitorIndex =
            TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "StartMonitorIndex", 0);

            Screen targetScreen =
                (monitorIndex >= 0 && monitorIndex < Screen.AllScreens.Length)
                ? Screen.AllScreens[monitorIndex]
                : Screen.PrimaryScreen;

            this.StartPosition = FormStartPosition.Manual;

            // 먼저 위치 설정
            this.Location = targetScreen.WorkingArea.Location;

            // 마지막에 딱 한 번만
            this.WindowState = FormWindowState.Maximized;
        }

        //protected override void OnShown(EventArgs e)
        //{
        //    base.OnShown(e);
        //    SetFormLocation();
        //    //RestoreMainFormLocationAndSize();
        //}

        private async void FormLocalMain_Load(object sender, EventArgs e)
        {
            await C_init.ViewProgrammStart(this);

            if (!ConfigViewMain.bRestoreLocationSizeOnStartup)
            {
                SetFormLocation();            //20241010 PSU
            }
            else
                RestoreMainFormLocationAndSize();

            InitNotifyIcon();

            // ViewProgramStart가 밑에 있으면 로그인 실패 종료 시 너무 많은 오류가 난다.
            if (await LogInOnProgramStart() == false)
            {
                return;
            }

            ToolBarModule.LoadList();
            ToolBarModule.FrameToolBarCreate(this);

            MenuStrip userMainMenu = C_Menu.LoadAutoBaseMenu(new C_Menu.CallBack(this.CallBackUserMenu));

            SetMenu(userMainMenu);

            // this.WindowState = FormWindowState.Maximized; 

            KeyLock keylock = new KeyLock();
            bool bKeyLock = keylock.CheckKeyLockLocal();

            

            if (SharedLocalMain.bTestMode == true)
            {
                SmLog.LogInfo(LogCategory.SYSTEM, "TestMode Start");  //20241024 테스트모드 로그기록 추가 PSU , 20260225 PSU 메시지 사용안해도 로그로 테스트모드 시작 기록하도록 변경

                if (ConfigViewMain.bUseTestModeMessage)
                {
                    if (Tools.IsLangKorean())
                    {
                        MessageBox.Show("프로그램이 현재 테스트 모드로 실행 중입니다.\n태그 값은 실제 통신을 통해서 얻어진 값이 아닌 자동으로 변화되는 값입니다.\n통신 모드로 실행하려면 작업선택 프로그램에서 '테스트 모드로 실행' 을 OFF로 변경하기 바랍니다.", "테스트 모드 실행");
                    }
                    else if (Tools.IsLangJapanese())
                    {
                        MessageBox.Show("プログラムが今現在テストモードで実行しています。\nタグの値は実際の通信を通じて取り出した値ではなく自動に変化される値です。\n通信モードに実行しようとすると作業選択のプログラムで'テストモードに実行'をOFFに変更してください。", "テストモードに実行");
                    }
                    else
                    {
                        MessageBox.Show("The Program Test Mode is currently running.\nTag Values are not from actual communication but modified automatically.\nTo run Communication Mode, modify 'Running to Test Mode' into OFF in the Work Selection Program.", "Test Mode");
                    }
                }
            }
            else
            {
                //데모 2시간 연장 글자 수정 20241010 PSU
                if (!bKeyLock)
                {
                    if (Tools.IsLangKorean())
                    {
                        string msg = String.Format("{0} KeyLock이 설치되지 않았습니다.\n2시간 동안만 정상 사용할 수 있습니다.", TotalConfig.AutoBaseIniGetOemProgramName());
                        MessageBox.Show(msg, "키락 없음");
                    }
                    else if (Tools.IsLangChinese())
                    {
                        string msg = String.Format("没有安装{0} KeyLock。\n只能用2小时。", TotalConfig.AutoBaseIniGetOemProgramName());
                        MessageBox.Show(msg, "没有KeyLock");
                    }
                    else
                    {
                        string msg = String.Format("{0} Keylock does not exist.\nYou can run this program only 2 hours.", TotalConfig.AutoBaseIniGetOemProgramName());
                        MessageBox.Show(msg, "Keylock not found");
                    }
                }
                else //20241024 키락인식로그 추가 PSU
                {
                    SmLog.LogInfo(LogCategory.SYSTEM, String.Format("Local KeyLock recognized. Serial Number: {0}", KeyLock.sSerialNumber));
                    KeyLock.PreviousKeyLockStatus = true;
                }
            }

            if (TotalConfig.eOemType == EnumOemType.SCADA_LITE || TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite)
            {
                ViewreportsToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Report); // 보기-리포터
                ViewmilliDatasToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.MilliData);    // 보기-미세자료 보기
                ViewdBIntegrationsToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Database);//보기-DB적산

                configExcelPathToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.ExcelReport);   // 환경설정-엑셀 경로 설정
                databaseToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Database);          // 환경설정-데이터베이스
                ConfigautomaticReportPrintToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Report);// 환경설정-리포터 자동 인쇄
            }

            SystemInfo.SystemInfoStatus.SystemInfoInit();   // GPIO 체크는 보드의 정보를 사용하기 때문에 키락을 체크한 다음 초기화한다.

            CheckTagOverFlow(); 



            //키락이 있을 때만 체크한다.
            if (SharedLocalMain.bTestMode == false && bKeyLock)
            {
                CheckKeyLockVersionLow();
            }

            if (userMainMenu == null)
            {
                if (!SystemInfo.SystemInfoStatus.IsAble) 
                {
                    helpToolStripMenuItem.DropDownItems.Remove(toolStripMenuItemHelpBoardInformation); // 보드 정보 메뉴를 삭제한다.
                }
            }

            SetTitle();

            KeyLock.StartKeyLockChecking(); //20241024 키락체크로직 추가 PSU

            timerMain.Enabled = true;

            // U-UE-G OEM은 태그값을 100mm sec 이하로 SQL 서버에 올리는 역할을 해야 하므로 태그값을 바로저장해야 한다.
            if (TotalConfig.eOemType == EnumOemType.UYeG_GS || TotalConfig.eOemType == EnumOemType.UYeG_Normal)
            {
                ScriptFunctionTag.bTagShareUpdateOption = true;
            }

            await C_init.PlayScriptWhenProgrammStartEnd(this, "START");

            FormInformationSoftLock.DisplayLockInfoOnStartUp();

            ExecuteArgument();	// 이부분이 SetTitle보다 위에 있으면 그래픽을 불러올 경우 정보가 Title 에 함께 섞여서 나온다.
            // timerMain.Enabled = true가 앞에 있어도 Dialog Module을 기본으로 로딩 시 타이머가 발생하지 않는다.

            if (NextVersion.bScript11)
            {
                PrepareDebugger();
            }

            if (NextVersion.bAutobase11)
            {
                this.dataGateServerToolStripMenuItem.Visible = true;
            }

            CheckAdminPassword();
        }


        void SetMenu(MenuStrip userMainMenu)
        {
            if (userMainMenu != null)
            {
                this.menuStripMain.Items.Clear();

                int count = userMainMenu.Items.Count;

                for (int i = 0; i < count; i++)
                {
                    this.menuStripMain.Items.Add(userMainMenu.Items[0]);    // Add하는 순간 userMainManu에 있는 아이템이 하나씩 자동으로 빠지는 현상이 있어서 count를 저장해서 사용 
                }
            }
            else
            {
                if (TotalConfig.eOemType == EnumOemType.ZIoT)
                {
                    toolStripSeparator3.Visible = false;
                    FilerunStudioToolStripMenuItem.Visible = false; // 파일-스튜디오 실행 삭제

                    helpToolStripMenuItem.DropDownItems.Clear();
                }
                else if (TotalConfig.eOemType == EnumOemType.FiveTek)
                {
                    WindowarrangeIconsToolStripMenuItem.Visible = false;
                }
                else if (TotalConfig.eOemType == EnumOemType.UYeG_GS || TotalConfig.eOemType == EnumOemType.UYeG_Normal)
                {
                    WindowarrangeIconsToolStripMenuItem.Visible = false;

                    helpToolStripMenuItem1.Visible = false;
                    HelpremoteASToolStripMenuItem.Visible = false;
                    toolStripSeparator13.Visible = false;
                }
                else if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
                {
                    toolStripSeparator8.Visible = false;
                    ViewnavigationToolStripMenuItem.Visible = false;

                    // 인증이 끝난후 업체에서 추가 2021-2-1
                    helpToolStripMenuItem1.Visible = false;
                    HelpremoteASToolStripMenuItem.Visible = false;
                    toolStripSeparator13.Visible = false;
                    HelpuserNameToolStripMenuItem.Visible = false;
                }
                else if (TotalConfig.eOemType == EnumOemType.KobasAI)
                {
                    toolStripSeparator1.Visible = false;

                    FilewriteMemoToolStripMenuItem.Visible = false;
                    FilereadMemoToolStripMenuItem.Visible = false;

                    toolStripSeparator2.Visible = false;
                    FileprintScreenToolStripMenuItem.Visible = false;
                    FilescreenSaveToolStripMenuItem.Visible = false;
                    FilesaveSelectionZoneToolStripMenuItem.Visible = false;

                    ViewalarmFilesToolStripMenuItem.Text = "이상 탐지";
                    ViewalarmEventsToolStripMenuItem.Text = "이상 탐지창";

                    HelpremoteASToolStripMenuItem.Visible = false;
                }
            }

            ViewMainPublic.PublicMenu.EnableDisableMainTitle(this);
            ViewMainPublic.PublicMenu.EnableDisableMainMenu(this, this.menuStripMain);
        }

        public bool UserMenuChange(string menuName)
        {
            MenuStrip userMainMenu = C_Menu.LoadAutoBaseMenu(menuName);
            if (userMainMenu != null)
            {
                SetMenu(userMainMenu);
                return true;
            }
            else
            {
                return false;
            }
        }

        void CheckAdminPassword()
        {
            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {

            }
            else
            {
                return;
            }

            string supervisor = TotalConfig.AutoBaseIniGetOemSupervisorName();
            string pass = TotalConfig.LoadRegOemConfig("Supervisor", null, "PassCode2", AutoLibLocal.UserInfoStruct.ZipPassword(supervisor, supervisor.ToLower()));
            string pass_basic = AutoLibLocal.UserInfoStruct.ZipPassword(supervisor, supervisor.ToLower());

            if (pass == pass_basic)
            {
                MessageBox.Show("관리자의 암호가 기본 암호입니다. 암호를 변경해야 합니다.\n암호를 변경하는 화면으로 이동합니다.", "관리자 암호 변경 필요");

                FormConfigUser.SetAdminPassword(this, false);
            }
        }

        void PrepareDebugger()
        {
            //ScriptLibRun.Debugger.FormDebugger.formMain = this;
            //ScriptLibRun.Debugger.FormDebugger.sDirWorkProject = TotalConfig.sDirWorkProject;
        }

        public System.EventHandler CallBackUserMenu(ToolStripMenuItem mi, string s)
        {
            if (String.Compare(s, "LOGIN") == 0) return new System.EventHandler(this.FilelogInToolStripMenuItem_Click);
            else if (String.Compare(s, "LOGOUT") == 0) return new System.EventHandler(this.FilelogOutToolStripMenuItem_Click);
            else if (String.Compare(s, "SEND_MAIL") == 0) return new System.EventHandler(this.FilewriteMemoToolStripMenuItem_Click);
            else if (String.Compare(s, "READ_MAIL") == 0) return new System.EventHandler(this.FilereadMemoToolStripMenuItem_Click);
            else if (String.Compare(s, "PRINT_SCREEN") == 0) return new System.EventHandler(this.FileprintScreenToolStripMenuItem_Click);
            else if (String.Compare(s, "SAVE_SCREEN") == 0) return new System.EventHandler(this.FilescreenSaveToolStripMenuItem_Click);
            else if (String.Compare(s, "SAVE_SCREEN_ZONE") == 0) return new System.EventHandler(this.FilesaveSelectionZoneToolStripMenuItem_Click);
            else if (String.Compare(s, "RUN_STUDIO") == 0) return new System.EventHandler(this.FilerunStudioToolStripMenuItem_Click);
            else if (String.Compare(s, "EXIT") == 0) return new System.EventHandler(this.FileexitToolStripMenuItem_Click);
            else if (String.Compare(s, "SAVE_RESULT_TO_FILE") == 0) return new System.EventHandler(this.FileSaveResultToFileToolStripMenuItem_Click);

            else if (String.Compare(s, "VIEW_GRAPHIC") == 0) return new System.EventHandler(this.ViewgraphicsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ALLTAG_LIST") == 0) return new System.EventHandler(this.ViewentireTagsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ANALOG_INPUT") == 0) return new System.EventHandler(this.ViewanalogInputsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ANALOG_OUTPUT") == 0) return new System.EventHandler(this.ViewanalogOutputsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_DIGITAL_INPUT") == 0) return new System.EventHandler(this.ViewdigitalInputsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_DIGITAL_OUTPUT") == 0) return new System.EventHandler(this.ViewdigitalOutputsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ALARM") == 0) return new System.EventHandler(this.ViewalarmFilesToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ALARM_EVENT_BOX") == 0) return new System.EventHandler(this.ViewalarmEventsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_LOG") == 0) return new System.EventHandler(this.ViewlogFilesToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_PCL") == 0) return new System.EventHandler(this.ViewscriptsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_SCANBUF") == 0) return new System.EventHandler(this.ViewcommunicationServerToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_STRING_TAG") == 0) return new System.EventHandler(this.ViewstringTagsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_GROUP_SHOW") == 0) return new System.EventHandler(this.ViewregisteredGroupsToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_REPORT") == 0) return new System.EventHandler(this.ViewreportsToolStripMenuItem_Click);

            else if (String.Compare(s, "VIEW_GROUPTAG_DO") == 0) return null;

            else if (String.Compare(s, "VIEW_DEMAND_CONTROL") == 0) return new System.EventHandler(this.ViewdemandControlsToolStripMenuItem_Click);

            else if (String.Compare(s, "VIEW_MILLI_DATA") == 0) return new System.EventHandler(this.ViewmilliDatasToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_SCHEDULE") == 0) return new System.EventHandler(this.ViewschedulesToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_DB_ADDITION") == 0) return new System.EventHandler(this.ViewdBIntegrationsToolStripMenuItem_Click);

            else if (String.Compare(s, "VIEW_RECIPE") == 0) return new System.EventHandler(this.ViewrecipesToolStripMenuItem_Click); //20260227 PSU add

            else if (String.Compare(s, "VIEW_PRESET") == 0) return new System.EventHandler(this.ViewpresetsToolStripMenuItem_Click); //20260303 PSU add

            else if (String.Compare(s, "VIEW_NAVIGATOR") == 0) return new System.EventHandler(this.ViewnavigationToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_REST_API_MONITOR") == 0) return new System.EventHandler(this.ViewRestApiMonitorToolStripMenuItem_Click);

            else if (String.Compare(s, "CONFIG_ALARM") == 0) return new System.EventHandler(this.configAlarmToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_FONT") == 0) return new System.EventHandler(this.configFontToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_COLOR") == 0) return new System.EventHandler(this.ConfigbasicColorsToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_ALARM_COLOR") == 0) return new System.EventHandler(this.ConfigalarmColorsToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_DATA") == 0) return new System.EventHandler(this.configDataToolStripMenuItem_Click);

            else if (String.Compare(s, "CONFIG_FLOW_WATCH") == 0 || String.Compare(s, "CONFIG_FLOW_VIEW") == 0)
                return new System.EventHandler(this.configFlowWatchToolStripMenuItem_Click);

            else if (String.Compare(s, "CONFIG_USER") == 0) return new System.EventHandler(this.configUsersToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_DDE") == 0) return null;
            else if (String.Compare(s, "CONFIG_GRAPHIC_MODULE_DATA") == 0) return null;
            else if (String.Compare(s, "CONFIG_PRINT_SCRIPT_PRINT_MODULE") == 0) return null;
            else if (String.Compare(s, "CONFIG_ETC") == 0) return new System.EventHandler(this.configOtherToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_EXCEL_PATH") == 0) return new System.EventHandler(this.configExcelPathToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_DEMAND_NEW") == 0) return new System.EventHandler(this.configDemandNewToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_BARCODE") == 0) return new System.EventHandler(this.configBarcodeToolStripMenuItem_Click);

            else if (String.Compare(s, "CONFIG_DSN") == 0) return new System.EventHandler(this.ConfigconnectionStringToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_SHARED_DATABASE") == 0) return new System.EventHandler(this.ConfigsharedDatabaseToolStripMenuItem_Click);

            else if (String.Compare(s, "CONFIG_SCHEDULE_FIXED") == 0) return new System.EventHandler(this.ConfigyearlyFixedSchedulesToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_SCHEDULE_ADDITIONAL") == 0) return new System.EventHandler(this.ConfigyearlyAdditionalSchedulesToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_SCHEDULE_MODEL") == 0) return new System.EventHandler(this.ConfigmodelsToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_SCHEDULE_WEEK") == 0) return new System.EventHandler(this.ConfigweeklySchedulesToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_SCHEDULE_DESIGNER") == 0) return new System.EventHandler(this.ConfigScheduleDesignerToolStripMenuItem_Click); //25-03-25 추가 hsjeong

            else if (String.Compare(s, "CONFIG_REPORT_AUTO_PRINT") == 0) return new System.EventHandler(this.ConfigautomaticReportPrintToolStripMenuItem_Click);

            //else if(String.Compare(s, "CONFIG_AUTO_DATAFILE_MAKE") == 0)	idm = 1;
            //else if(String.Compare(s, "CONFIG_FORECAST_DIAGNOSIS") == 0)	idm = 1;
            else if (String.Compare(s, "CONFIG_CAPTION") == 0) return new System.EventHandler(this.ConfigwindowTitleToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_MENU") == 0) return new System.EventHandler(this.ConfigmainMenuToolStripMenuItem_Click);

            //else if(String.Compare(s, "TOOL_VIEWBOX_NONE") == 0)	idm = 1;
            //else if(String.Compare(s, "TOOL_VIEWBOX_FLOAT") == 0)	idm = 1;
            //else if(String.Compare(s, "TOOL_VIEWBOX_TOP") == 0)	idm = 1;
            //else if(String.Compare(s, "TOOL_VIEWBOX_BOTTOM") == 0)idm = 1;
            //else if(String.Compare(s, "TOOL_VIEWBOX_LEFT") == 0)	idm = 1;
            //else if(String.Compare(s, "TOOL_VIEWBOX_RIGHT") == 0)	idm = 1;

            //else if(String.Compare(s, "DEBUG_DDE_TAG") == 0)		idm = 1;
            //else if(String.Compare(s, "TOOL_CALCULATE") == 0)		idm = 1;

            else if (String.Compare(s, "WINDOW_CASCADE") == 0) return new System.EventHandler(this.WindowcascadeToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_TILE") == 0) return new System.EventHandler(this.WindowtileHorizontallyToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_TILE_HORZ") == 0) return new System.EventHandler(this.WindowtileHorizontallyToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_TILE_VERT") == 0) return new System.EventHandler(this.WindowtileVerticallyToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_ARRANGE") == 0) return new System.EventHandler(this.WindowarrangeIconsToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_CLOSE") == 0) return new System.EventHandler(this.WindowcloseToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_CLOSEALL") == 0) return new System.EventHandler(this.WindowcloseAllToolStripMenuItem_Click);

            else if (String.Compare(s, "HELP") == 0) return new System.EventHandler(this.helpToolStripMenuItem1_Click);
            else if (String.Compare(s, "WHOAMI") == 0) return new System.EventHandler(this.HelpuserNameToolStripMenuItem_Click);
            else if (String.Compare(s, "BOARD_INFO") == 0) return new System.EventHandler(this.toolStripMenuItemHelpSystemInformation_Click);  // 2010-10-19 추가
            else if (String.Compare(s, "ABOUT") == 0) return new System.EventHandler(this.HelpaboutToolStripMenuItem_Click);
            else if (String.Compare(s, "KEYLOCK_INFO") == 0) return new System.EventHandler(this.HelpkeylockInformationToolStripMenuItem_Click);
            else if (String.Compare(s, "SYSTEM_INFO") == 0) return new System.EventHandler(this.toolStripMenuItemHelpSystemInformation_Click_1);

            else if (String.Compare(s, "TOOL_MODULE_EDIT") == 0) return new System.EventHandler(this.FilerunStudioToolStripMenuItem_Click);
            else if (String.Compare(s, 0, "MENU_SCRIPT_", 0, 12) == 0)
            {	// user menu script define
                int id = ConvertTool.ToInt32(s.Substring(12));
                mi.Tag = id;
                return C_Menu.SeekMenuScript(id);
            }

            else
            {	// 그 이외 일 경우는 숫자로 판단한다.
                return null;
            }
        }

        ToolStripMenuItem RecurseCallBackMenuItemGetFromTitle(ToolStripItemCollection tsic, string title)
        {
            ToolStripMenuItem mi;

            for (int i = 0; i < tsic.Count; i++)
            {
                if (tsic[i].GetType() != typeof(ToolStripMenuItem)) continue;

                mi = (ToolStripMenuItem)tsic[i];

                if (mi.Text == title)
                {
                    return mi;
                }

                mi = RecurseCallBackMenuItemGetFromTitle(mi.DropDownItems, title);

                if (mi != null) return mi;
            }

            return null;
        }

        public ToolStripMenuItem CallBackMenuItemGetFromTitle(string title)
        {
            return RecurseCallBackMenuItemGetFromTitle(this.menuStripMain.Items, title);
        }
        
        static byte flagTimer;

        // 260226 PSU, 메인타이머 성능 프로파일링. 설정값(ms) 이상 지연 시 로그 기록.
        private const int PROFILE_LOG_INTERVAL_SEC = 10;    // 동일 구간 로그 빈도 제한 (초)
        private Dictionary<string, long> _profileLastLogTick = new Dictionary<string, long>();

        void ProfileLog(Stopwatch sw, ref long prev, string name)
        {
            long now = sw.ElapsedMilliseconds;
            long elapsed = now - prev;
            prev = now;

            // 사용자용 — threshold 이상 지연 시 로그 기록 (동일 구간 N초에 1회)
            int threshold = ConfigRunMain.nMainTimerProfileThresholdMs;
            if (ConfigRunMain.bMainTimerProfile && elapsed >= threshold)
            {
                long tick = Environment.TickCount;
                _profileLastLogTick.TryGetValue(name, out long lastTick);
                if (tick - lastTick >= PROFILE_LOG_INTERVAL_SEC * 1000)
                {
                    _profileLastLogTick[name] = tick;
                    Log.Write(LogLevel.WARNING, LogCategory.PERFORMANCE, "[MainTimer] {0}: {1}ms", name, elapsed);
                }
            }

            // 개발자용
            int devThreshold = 100;
            if (elapsed >= devThreshold)
                Debug.WriteLine($"[MainTimer] {name}: {elapsed}ms");
        }

        async Task PlanToolStatusLocal()
        {
            WatchDogInfo.SetTimer(EnumWatchDogInfo.WDI_LocalMain, 0);

            if (!CheckEngineTagChange.IsScanStart()) return;		// 아직 스캔을 시작할 시간이 되지 않았다.

            Stopwatch _profSw = Stopwatch.StartNew();
            long _profPrev = 0;

            await CheckEngineTagChange.CheckSignalChange().ConfigureAwait(false);					// 입력 값이 바뀌었나를 체크한다.
            ProfileLog(_profSw, ref _profPrev, "CheckSignalChange");

            await CheckEngineTimeChange.CheckTimeChange(); 					// 시간이 바뀌었는가를 체크한다.
            ProfileLog(_profSw, ref _profPrev, "CheckTimeChange");

            await CheckEngineAlwaysScript.PclProgrammStatus(this);			// 자동 프로그램을 체크한다.
            ProfileLog(_profSw, ref _profPrev, "PclProgrammStatus");

            // FormGraphicFrame.ModuleScriptAlwaysTimer();					// 각 모듈의 상시 스크립트를 실행한다.  FormGraphicChild에서 타이머를 처리한다.
            FormConfigFlowView.FlowViewStatus();						// 순차 감시 부분을 체크한다.
            ProfileLog(_profSw, ref _profPrev, "FlowViewStatus");
            // CheckAutoDataFileMake();  								// 자동 자료 저장부분을 체크한다.

            Alarm.AlarmConfirm.AlarmConfirmationStatus();				// 계속경보를 검사한다.
            ProfileLog(_profSw, ref _profPrev, "AlarmConfirmationStatus");

            C_DdeTag.ConnectTry();										// DDE tag를 계속 접속 시킨다.
            ProfileLog(_profSw, ref _profPrev, "DdeConnectTry");

            await CheckEngineTagChange.CheckAiDiSubTag();						// AI & DI Sub tag를 체크한다.
            ProfileLog(_profSw, ref _profPrev, "CheckAiDiSubTag");

            CheckEngineNetworkToViewMain.Check();						// Network에서 오는 신호를 체크한다.
            ProfileLog(_profSw, ref _profPrev, "NetworkCheck");


            //CheckHandOperationResultAlarm();							// 수동 조작 후 조작이 잘 되었는가를 검사한다.
            AlarmToDigitalOut.CheckAlarmStatusToDigitalOutList();
            ProfileLog(_profSw, ref _profPrev, "AlarmToDigitalOut");

            await CheckEngineDemandControl.FunctionBlockDemandControlStatus();
            ProfileLog(_profSw, ref _profPrev, "DemandControl");

            await CheckEngineDemandNew.TickAsync();
            ProfileLog(_profSw, ref _profPrev, "DemandNew");

            CheckEngineMinuteChanged.StatusRemainTrendSave();           // 시간 나는대로 트랜드를 저장해 준다.
            ProfileLog(_profSw, ref _profPrev, "TrendSave");

            await CheckEngineMilliData.CheckMilliData();
            ProfileLog(_profSw, ref _profPrev, "MilliData");

            await CheckEngineSchedule.CheckScheduleStatus();
            ProfileLog(_profSw, ref _profPrev, "Schedule");

            // 태그가 많을때는 속도가 너무 늦어서 CheckSignalChange속에 포함시켰다.
            //CheckEngineTagChange.CheckWriteFromSharedTag();

            ReportAutoPrint.AutoPrintCheck();
            ProfileLog(_profSw, ref _profPrev, "AutoPrint");

            // PlcScan의 메모리 상황이 바뀌었는가를 검사한다.
            PlcScanEvent.Check();
            ProfileLog(_profSw, ref _profPrev, "PlcScanEvent");

            CheckRealTimeTestData.CheckRealTimeTest();

            SystemInfo.SystemInfoStatus.SystemInfoCheck();

            long totalMs = _profSw.ElapsedMilliseconds;

            // 사용자용 — Total도 빈도 제한 적용
            int threshold = ConfigRunMain.nMainTimerProfileThresholdMs;
            if (ConfigRunMain.bMainTimerProfile && totalMs >= threshold)
            {
                long tick = Environment.TickCount;
                _profileLastLogTick.TryGetValue("Total", out long lastTick);
                if (tick - lastTick >= PROFILE_LOG_INTERVAL_SEC * 1000)
                {
                    _profileLastLogTick["Total"] = tick;
                    Log.Write(LogLevel.WARNING, LogCategory.PERFORMANCE, "[MainTimer] Total: {0}ms", totalMs);
                }
            }

            // 개발자용
            if (totalMs > 100)
                Debug.WriteLine($"[MainTimer] ──── Total: {totalMs}ms ────");
        }

        // 일정시간 동안 마우스 응답이 없으면 자동으로 로그아웃하는 기능
       async Task CheckAutoLogOutWhenNoResponse()
        {
            if (TotalConfig.eOemType != EnumOemType.SBAS) return;

            if (String.Compare(SharedData.userInfo.sUsername, "_PUBLIC_", true) == 0 ||
                    String.Compare(SharedData.userInfo.sUsername, "PUBLIC", true) == 0) return; // 이미 로그아웃되어 있다.

            DateTime t = DateTime.Now;

            long cur = TimeUtil.GetSecHap(t);
            const int limit = 600;

            long gab_mouse = cur - TimeUtil.GetSecHap(SharedData.dtLastMouseMove);

            if (gab_mouse < limit) return;

            long gab_keyboard = cur - TimeUtil.GetSecHap(SharedData.dtLastKeyDown);

            if (gab_keyboard < limit) return;

            await C_protect.AutoBaseLogOut(false);  // 메시지가 없어야 할 듯

            FormPopupLogouted form = new FormPopupLogouted();
            form.Owner = TotalConfig.formMain;
            form.StartPosition = FormStartPosition.CenterParent;
            form.Show();  //250828 PSU TotalConfig.formMain 제거
        }

        bool bTimerRunning = false;
        bool _excetpionFlag = false;

        double fOldMainTimer = 0;   // 1mmsec 안에 끝나는 경우도 있다.


        /// <summary>
        /// 다른 곳에서 Forms.Timer 를 사용하면 병목이 생기므로 주의. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void timerMain_Tick(object sender, EventArgs e)
        {
            //flagTimer++;
            //flagTimer %= 2;
            //if (flagTimer == 1)
            //{
            //    Thread.Sleep(1);
            //    return;
            //}

            if (bTimerRunning || _excetpionFlag)   // 260226 PSU  && >> || 로 변경. 
                return;

            bTimerRunning = true;

            try
            {
                Stopwatch swTick = Stopwatch.StartNew();
                long tickPrev = 0;

                await PlanToolStatusLocal();
                ProfileLog(swTick, ref tickPrev, "PlanToolStatusLocal");

                if (SharedLocalMain.bTestMode == false && KeyLock.CheckTimeOutLocalKey())
                {
                    SharedLocalMain.bTestMode = true;
                    GraphicModule.SharedLocalMain.bTestMode = true; //20241010 PSU, TestMode 시 스크립트실행방지.
                    SetTitle();
                }

                //SharedViewMain.EventGoTimer();

                await SharedViewMain.EventGoTimerAsync();
                ProfileLog(swTick, ref tickPrev, "EventGoTimerAsync");

                MessageDisplay.CheckMessage();  // 쓰레드에서 호출한 메시지를 표시하기 위해서 필요하다.

                await CheckAutoLogOutWhenNoResponse();

                LocalMain.Alarm.FormPopupAlarmConfirmation.CheckAlarmConfirmOnMainTimer();    // 쓰레드에서 생성된 경보확인창을 표시하기 위해서 필요하다. 2016-3-30

                AlarmDisplay.WaitAlarmOnTimer();
                ProfileLog(swTick, ref tickPrev, "AlarmDisplay");

                fOldMainTimer = (fOldMainTimer + swTick.ElapsedMilliseconds) / 2.0;
                SystemInformation.SetValue(EnumSystemInformation.MainTimer, fOldMainTimer);

            }
            catch(Exception ex)
            {
                _excetpionFlag = true;
                Program.HandleFatalException(ex);            
            }
            finally 
            {
                bTimerRunning = false; 
            }
        }

        //protected override void WndProc(ref Message m)
        //{
        //    int WM_QUERYENDSESSION = 0x11;

        //    if (m.Msg == WM_QUERYENDSESSION)
        //    {
        //        systemShutDown = true;
        //    }

        //    base.WndProc(ref m);
        //}

        //윈도우 디바이스 변경 알림 추가 20241025 PSU
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVNODES_CHANGED = 0x0007;  //디바이스 노드 변경.
        private const int WM_COPYDATA = 0x004A;
        private const int WM_QUERYENDSESSION = 0x11;
        private const int WM_ENTERSIZEMOVE = 0x0231;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private bool _isUserResizing = false;

        // RemoteProjectAgent 명령 ID
        private const int CYCOPYDATA_CYCNAVIGATE = 1001;

        [StructLayout(LayoutKind.Sequential)]
        private struct CYCOPYDATA_CYSTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                HandleCopyData(m);
                return;
            }
            if (m.Msg == WM_QUERYENDSESSION)
            {
                systemShutDown = true;
            }
            if (m.Msg == WM_DEVICECHANGE)
            {
                switch ((int)m.WParam)
                {
                    case DBT_DEVNODES_CHANGED:
                        if (KeyLock.DeviceCheckFlag != 1)
                        {
                            KeyLock.DeviceCheckFlag = 1;
                        }
                        break;
                }
            }
            // MDI Parent 리사이즈 시 자식 FormGraphicFrame + 툴바모듈 그래픽 갱신 억제
            if (m.Msg == WM_ENTERSIZEMOVE)
            {
                _isUserResizing = true;
                foreach (Form child in this.MdiChildren)
                {
                    if (child is FormGraphicFrame frame && frame.formChild != null)
                        frame.formChild.SuspendGraphicUpdate();
                }
                ToolBarModule.SuspendAllGraphicUpdate();
            }
            else if (m.Msg == WM_EXITSIZEMOVE)
            {
                _isUserResizing = false;
                // 툴바 위치를 최종 크기로 한 번 반영
                ToolBarModule.MoveFrameToolBar(this);
                ToolBarModule.ResumeAllGraphicUpdate();
                foreach (Form child in this.MdiChildren)
                {
                    if (child is FormGraphicFrame frame && frame.formChild != null)
                        frame.formChild.ResumeGraphicUpdate();
                }
            }
            base.WndProc(ref m);
        }

        private void HandleCopyData(Message m)
        {
            try
            {
                var cds = (CYCOPYDATA_CYSTRUCT)Marshal.PtrToStructure(m.LParam, typeof(CYCOPYDATA_CYSTRUCT));
                int commandId = (int)cds.dwData;

                if (commandId == CYCOPYDATA_CYCNAVIGATE && cds.cbData > 0)
                {
                    string pageName = Marshal.PtrToStringUni(cds.lpData, cds.cbData / 2);
                    if (!string.IsNullOrEmpty(pageName))
                    {
                        NavigateToPage(pageName);
                    }
                }
            }
            catch (Exception ex)
            {
                SmLog.Error($"HandleCopyData error: {ex.Message}");
            }
        }

        private void NavigateToPage(string pageName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(NavigateToPage), pageName);
                return;
            }

            string path;
            if (Path.IsPathRooted(pageName))
                path = pageName;
            else
                path = Path.Combine(TotalConfig.sDirWorkProject, "graphic", pageName);

            if (File.Exists(path))
            {
                GraphicTool.RestoreGraphicWindow(path, -1, 0, 0);
            }
        }

        private void ViewgraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewGraphic_Click();
        }

        private void ViewentireTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewAllTagView_Click(this);
        }

        private void ViewanalogInputsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewAnalogInput_Click(this);
        }

        private void ViewanalogOutputsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewAnalogOutput_Click(this);
        }

        private void ViewdigitalInputsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewDigitalInput_Click(this);
        }

        private void ViewdigitalOutputsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewDigitalOutput_Click(this);
        }

        private void ViewstringTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewStringTag_Click(this);
        }

        private void ViewregisteredGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewRegisterdGroup_Click(this);
        }

        private void ViewalarmFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewAlarmFile_Click(this);
        }

        private void ViewalarmEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Alarm.FormPopupAlarmConfirmation.IsPopupAlarmConfirmation())
                Alarm.FormPopupAlarmConfirmation.CreatePopupAlarmConfirmation(false);
            else
                Alarm.FormPopupAlarmConfirmation.CreatePopupAlarmConfirmation(true);
        }

        private void ViewlogFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewLogFile_Click(this);
        }

        private void ViewscriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormAlwaysScript.ringScript.OneMdiCheck()) return;

            FormAlwaysScript.ringScript.OneMdiCreate(this, new FormAlwaysScript());
        }

        public static void RunPlcScan(bool bMinimized)
        {
            IntPtr hwnd = Win32Function.FindWindow("PlcScanMainFrame", null);
            if (hwnd != IntPtr.Zero)
            {
                if (!bMinimized)
                {
                    Win32Function.ShowWindow(hwnd, (int)EnumShowWindow.SW_SHOW);
                    Win32Function.SetForegroundWindow(hwnd);
                }
                return;
            }

            string path = String.Format("{0}\\plc_scan.exe", Application.StartupPath);

            try
            {
                if (bMinimized)
                    Process.Start(path, "Minimized=true");
                else
                    Process.Start(path, "");
            }
            catch (Exception exception)
            {
                if (Tools.IsLangKorean())
                {
                    System.Windows.Forms.MessageBox.Show(exception.Message, "PLC_SCAN.exe 실행 오류");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(exception.Message, "PLC_SCAN Execute Error");
                }
            }
        }

        private void ViewcommunicationServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunPlcScan(false);
        }

        private void ViewreportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewReport_Click();
        }

        private void ViewdemandControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DemandControl.blockDemandControl.Count == 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("현재 사용하고 있는 디맨드 제어는 없습니다.", "디맨드 제어 없음");
                }
                else if (Tools.IsLangChinese())
                {
                    MessageBox.Show("现在没有使用中的点播控制。", "没有需要控制");
                }
                else if (Tools.IsLangVietnamese())
                {
                    MessageBox.Show("Khối điều khiển theo nhu cầu không tìm thấy.", "Điều khiển theo nhu cầu không tồn tại.");
                }
                else
                {
                    MessageBox.Show("Demand Control Block not found.", "Demand control not exist");
                }
                return;
            }

            FormDemandControl child = new FormDemandControl();
            
            child.MdiParent = this;
            child.Show();
        }

        private void ViewmilliDatasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormMilliData.ringForm.OneMdiCheck()) return;

            if (!TotalConfig.GetOemTypeRightAndUnavailableMsg(EnumOemTypeRight.MilliData)) return;

            FormMilliData.ringForm.OneMdiCreate(this, new FormMilliData());
        }

        private void ViewschedulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormSchedule.ringSchedule.OneMdiCheck()) return;

            FormSchedule.ringSchedule.OneMdiCreate(this, new FormSchedule());
        }

        private void ViewrecipesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormRecipe.ringRecipe.OneMdiCheck()) return;

            FormRecipe.ringRecipe.OneMdiCreate(this, new FormRecipe());
        }

        private void ViewpresetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormConfigPreset.ringPreset.OneMdiCheck()) return;

            FormConfigPreset dialog = new FormConfigPreset();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void ViewdBIntegrationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TotalConfig.GetOemTypeRightAndUnavailableMsg(EnumOemTypeRight.Database)) return;
            
            DialogAddition.FormMain dialog = new DialogAddition.FormMain();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void ViewnavigationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewNavigator_Click(this);
        }

        private AutobaseRESTAPIMonitor.FormRestApiMonitor _formRestApiMonitor;
        private void ViewRestApiMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_formRestApiMonitor == null || _formRestApiMonitor.IsDisposed)
            {
                _formRestApiMonitor = new AutobaseRESTAPIMonitor.FormRestApiMonitor();
                _formRestApiMonitor.Show(this);
            }
            else
            {
                _formRestApiMonitor.BringToFront();
                _formRestApiMonitor.Activate();
            }
        }

        private void configAlarmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigAlarm dialog = new FormConfigAlarm();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }
        private void configDemandNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DemandNew.FormDemandNewSettings dialog = new DemandNew.FormDemandNewSettings();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void configBarcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBarcodeConfig dialog = new FormBarcodeConfig();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void configFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemConfigFont_Click();
        }

        private void ConfigbasicColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemConfigTotalColor_Click(this);
        }

        private void ConfigalarmColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemConfigAlarmColor_Click(this);
        }

        private void configDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigData dialog = new FormConfigData();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void configFlowWatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigFlowView dialog = new FormConfigFlowView();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void configUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                if (!SharedData.userInfo.IsHaveAllRights())
                {
                    MessageBox.Show("사용자 설정은 관리자만 할 수 있습니다.", "권한 오류");
                    return;
                }
                
            }

            ViewMainPublic.PublicMenu.menuItemConfigUser_Click();
        }

        private void configOtherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigEtc dialog = new FormConfigEtc();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void configExcelPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TotalConfig.GetOemTypeRightAndUnavailableMsg(EnumOemTypeRight.ExcelReport)) return;
            
            FormConfigExcel dialog = new FormConfigExcel();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void schedulesToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        

        private void ConfigconnectionStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TotalConfig.GetOemTypeRightAndUnavailableMsg(EnumOemTypeRight.Database)) return;
            
            FormDatabaseConnection dialog = new FormDatabaseConnection(DbTool.dsnList);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_CONFIG_ETC))
                dialog.EnableButtonOK(false);
                //this.buttonOK.Enabled = false;

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void ConfigsharedDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TotalConfig.GetOemTypeRightAndUnavailableMsg(EnumOemTypeRight.Database)) return;
            
            FormConfigSharedDatabase dialog = new FormConfigSharedDatabase();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void ConfigyearlyFixedSchedulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSchedule.ConfigFixedSchedule();
        }

        private void ConfigyearlyAdditionalSchedulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSchedule.ConfigScheduleAdditional();
        }

        private void ConfigmodelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSchedule.ScheduleModelConfig(ref Schedule.blockScheduleModel);
        }

        private void ConfigweeklySchedulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSchedule.ConfigScheduleWeek();
        }

        private void ConfigScheduleDesignerToolStripMenuItem_Click(object sender, EventArgs e) //25-03-25 추가 hsjeong
        {
            FormSchedule.ConfigScheduleDesigner();
        }

        private void ConfigautomaticReportPrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TotalConfig.GetOemTypeRightAndUnavailableMsg(EnumOemTypeRight.Report)) return;
            
            FormConfigAutoPrint dialog = new FormConfigAutoPrint();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(FormLocalMain.formMain);
        }

        private void ConfigwindowTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewWindowTitle_Click(this);
            SetTitle();
        }

        private void ConfigmainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewMainMenu_Click(this, this.menuStripMain);
        }

        private void WindowcascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.Cascade);
        }

        private void WindowtileHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.TileHorizontal);
        }

        private void WindowtileVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.TileVertical);
        }

        private void WindowarrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.ArrangeIcons);
        }

        private void WindowcloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
                this.ActiveMdiChild.Close();
        }

        private void WindowcloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form[] childForm = this.MdiChildren;
            //Make sure to ask for saving the doc before exiting the app 

            for (int i = 0; i < childForm.Length; i++)
                childForm[i].Close();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClassHelp.ShowHelp(this, "LocalMain.chm", "", false);
        }

        private void HelpremoteASToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PublicStudioLocalMain.ClassPublic.CallRemoteAS(KeyLock.sSerialNumber, KeyLock.GetVersion().ToString(), KeyLock.nTagSize.ToString());
        }

        private void HelpuserNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemUserInformation_Click();
        }

        private void HelpkeylockInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormInformationKeyLock.InformationKeylock();
        }

        private void HelpaboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogCommon.FormAbout dialog = new DialogCommon.FormAbout();
            dialog.ProgramIcon = this.Icon;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
                dialog.sFixTextProgram = "SBAS 감시";

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(FormLocalMain.formMain);
        }

        private void FormLocalMain_MdiChildActivate(object sender, EventArgs e)
        {
            ViewNavigator.NavigatorChangeModule();

            // ToolStripManager.Merge((ToolStrip)this.ActiveMdiChild.Controls["menuStrip1"], this.menuStripMain);
            Form child = this.ActiveMdiChild;
            PublicMenu.SetFormBorderStyleMdiChild(this, child);
        }

        private void FormLocalMain_SizeChanged(object sender, EventArgs e)
        {
            if (!_isUserResizing)
                ToolBarModule.MoveFrameToolBar(this);
        }

        /*
        bool PlayKeyScript(string key_string, string when)
        {
            ScriptClass control;

            string fullpath;

            fullpath = String.Format("{0}\\control\\{1}\\{2}.CTLX", TotalConfig.sDirWorkProject, when, key_string);
            if (!File.Exists(fullpath))
            {
                fullpath = String.Format("{0}\\control\\{1}\\{2}.CTL", TotalConfig.sDirWorkProject, when, key_string);
            }

            if (!File.Exists(fullpath)) return false;	// file not found

            control = new ScriptClass();

            control.LoadFromFile(fullpath);
            control.SetHandOperation();
            control.Run(this);

            if (control.IsError())
            {
                string message;
                message = control.GetError();
                MessageDisplay.Show(message);
            }

            return true;
        }

        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == 0x100)	// key Down
            {
                string sKeyString;
                if (KeyScript.KeyScriptCodeToString(msg.WParam.ToInt32(), out sKeyString))
                {
                    if (PlayKeyScript(sKeyString, "KeyDown"))
                    {
                        return true;
                    }
                }
            }
            else if (msg.Msg == 0x101) // 여기는 안들어 온다 그래서 밑에 추가
            {	// key up
                string sKeyString;
                if (KeyScript.KeyScriptCodeToString(msg.WParam.ToInt32(), out sKeyString))
                {
                    if (PlayKeyScript(sKeyString, "KeyUp"))
                    {
                        return true;
                    }
                }
            }
            else if (msg.Msg == 0x104) // WM_SYSKEYDOWN
            {
                if (keyData == Keys.F10)
                {
                    if (this.menuStripMain.Visible == false)
                    {
                        this.menuStripMain.Visible = true;
                        ConfigViewMain.bShowMainMenu = true;
                        ConfigViewMain.Save();
                        return true;
                    }
                }
            }
            else { }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            if (m.Msg == 0x101)
            {	// key up
                string sKeyString;
                if (KeyScript.KeyScriptCodeToString(m.WParam.ToInt32(), out sKeyString))
                {
                    if (PlayKeyScript(sKeyString, "KeyUp"))
                    {
                        return true;
                    }
                }
            }

            return base.ProcessKeyEventArgs(ref m);
        }*/

        private void fileToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            this.FilerunStudioToolStripMenuItem.Enabled = TotalConfig.GetAutoBaseEditEnable();

            if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                bool login = false;

                if (String.Compare(SharedData.userInfo.sUsername, "_PUBLIC_", true) == 0 ||
                    String.Compare(SharedData.userInfo.sUsername, "PUBLIC", true) == 0)
                    login = false;
                else
                    login = true;

                // 로그인 상태일때는 로그아웃만 보이게 하고 로그아웃 상태에서는 로그인만 보이게 한다.
                FilelogInToolStripMenuItem.Visible = !login;
                FilelogOutToolStripMenuItem.Visible = login;
            }
        }

        private void configToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            this.ConfigwindowTitleToolStripMenuItem.Checked = ConfigViewMain.bShowMainTitle;
            this.ConfigmainMenuToolStripMenuItem.Checked = ConfigViewMain.bShowMainMenu;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.configUsersToolStripMenuItem.Enabled = SharedData.userInfo.IsHaveAllRights();
            }
        }

        void ShowMessenger()
        {
            this.Visible = true;
            this.Activate();
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowMessenger();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMessenger();
        }

        private void toolStripMenuItemHelpSystemInformation_Click(object sender, EventArgs e)
        {
            SystemInfo.FormSystemInformation dialog = new LocalMain.SystemInfo.FormSystemInformation();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(FormLocalMain.formMain);
        }

        bool bNotPrimeTester = false;
        bool bFailLogInOnStart = false;
        bool systemShutDown = false;

        private void FormLocalMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bNotPrimeTester)
            {
                return; // 테스트용 키락이 없으면 그냥 종료
            }

            if (bCloseByCallEditor || bFailLogInOnStart)         // 이 때는 무조건 종료
            {
                // 스튜디오를 부르는 메뉴를 불렀을때
                // 초기 화면에서 로그온 실패시
            }
            else
            {
                if (ConfigViewMain.bUseNotifyIcon)
                {
                    if (bExit || systemShutDown)	// 종료를 눌렀을 때나 윈도우 메뉴의 다시 시작이나 종료일 때만 
                    {
                        bExit = false;              // 종료를 취소할 수도 있기 때문에 초기화한다.
                        systemShutDown = false;     // 종료를 취소할 수도 있기 때문에 초기화한다.
                    }
                    else
                    {
                        // this.Visible = false; //Visible을 false로 하면 다른 프로세서에서 열어도 Visible을 못한다.
                        this.WindowState = FormWindowState.Minimized;
                        e.Cancel = true;
                        return;
                    }
                }

                if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_PROGRAMM_END))
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("프로그램을 끝낼 권한이 없습니다.\n종료 권한이 있는 이름으로 로그인하여 사용하세요.", "권한 없음");
                    else if (Tools.IsLangJapanese())
                        MessageBox.Show("プログラムを終える権限がありません。\n終了権限がある名前でログインしてお使いください。", "権限ない");
                    else if (Tools.IsLangChinese())
                        MessageBox.Show("您没有退出程序的权限，\n以有可退出权限的用户名注册。", "没有权限");
                    else
                        MessageBox.Show("You have not a right of Programm Exit.", "Access denied");
                    e.Cancel = true;
                    return;
                }

                string text, caption;

                if (ConfigRunMain.bEndPrompt)
                {
                    if (Tools.IsLangKorean())
                    {
                        if(TotalConfig.eOemType == EnumOemType.SBAS)
                            text = "SBAS 감시 1.0을 종료하면\n각종 감시 자료가 생성되지 않습니다.\n끝낼까요?";
                        else
                            text = "감시 프로그램(LocalMain)을 종료하면\n각종 감시 자료가 생성되지 않습니다.\n감시 프로그램을 끝낼까요?";
                        caption = "종료 확인";
                    }
                    else if (Tools.IsLangJapanese())
                    {
                        text = "監視プログラム(LocalMain)を終了すると\n各種の監視資料が生成出来ません。\n監視プログラムを終えてもいいですか。";
                        caption = "終了確認";
                    }
                    else if (Tools.IsLangChinese())
                    {
                        text = "如果关闭了监控程序,\n就不再会建立各种监视资料.\n要关闭监视程序吗?";
                        caption = "退出确认";
                    }
                    else
                    {
                        text = "If you exit this (LocalMain) program then cannot save to database.\nAre you sure you want to exit?";
                        caption = "Prgram exit";
                    }

                    if (MessageBox.Show(text, caption, MessageBoxButtons.OKCancel) == DialogResult.OK)
                        e.Cancel = false;
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            KeyLock.StopKeyLockChecking(); //20241024 키락체크로직 종료 PSU

            this.timerMain.Enabled = false;	// Timer를 사용하지 않는다.

            /*
            // 이 부분을 Form_Closed로 빼는 것은 어떤지?
            CheckTagSave();

            foreach (Form child in this.MdiChildren)
            {
                child.Close();
            }

            ToolBarModule.Close();			// ToolBarModule을 Close하지 않으니까 GraphicChild에 Close가 호출되지 않아서 환경 저장 등 정리가 안됨

            C_init.ViewProgrammEnd(this);	// 여기서 MessageDisplay를 사용하는데 이부분을 Closed에 사용하니까 MessageDisplay에서 사용할 FormMain이 이미 Dispose되어서 오류가 나는 경우가 있어서 Closing에 넣었다.
            EndAnotherProgramm(); */
        }

        const int WM_APP_EXIT = 0x8001;

        void EndAnotherProgramm()
        {

            if (ConfigRunMain.bEndWithPlcScan)
            {
                if (!bCloseByCallEditor)    // 스튜디오에서 불렀을 때는 통신 프로그램을 종료하지 않는다. 2010-10-6
                {
                    IntPtr hwnd = Win32Function.FindWindow("PlcScanMainFrame", null);
                    if (hwnd != IntPtr.Zero)
                    {
                        //SendMessage(hwnd, WM_COMMAND, IDM_PUBLIC_DESTROY_WINDOW, 0L);
                        Win32Function.PostMessage(hwnd, 0x111, (IntPtr)(int)EnumIdmPublic.IDM_PUBLIC_DESTROY_WINDOW, (IntPtr)12345678);
                    }
                }
            }

            if (ConfigRunMain.bEndWithOpcUaClient) //260225 PSU 추가
            {
                if (!bCloseByCallEditor)    // 스튜디오에서 불렀을 때는 통신 프로그램을 종료하지 않는다. 
                {
                    OpcUaIpcManager.SendShutdown(); // OPCUA Client 종료 명령 (트레이 상태에서도 종료)
                }
            }
        }

        void SaveFormLocationSize()
        {
            Point p = this.Location;
            Size s = this.Size;

            TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Maximized", (this.WindowState == FormWindowState.Maximized));
            TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "LocationX", p.X);
            TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "LocationY", p.Y);
            TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Width", s.Width);
            TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "Height", s.Height);
        }

        private async void FormLocalMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 컬렉션이 수정 되었습니다. 열거 작업이 실행되지 않을 수도 있습니다. Collection was modified; enumeration operation may not execute
            // http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/0c351b6b-ee83-4cc7-8d8f-d0abab41325d
            // 위의 오류가 나서 Form_Closing에 있는 것을 Closed로 옮겼다. 위의 링크에서 보면 Closing에서는 안하는것이 좋을 것 같아서 Closed로 뺏다.
            // 2010-7-1 수정

            CheckTagSave();

            foreach (Form child in this.MdiChildren)
            {
                child.Close();
            }

            ToolBarModule.Close();			// ToolBarModule을 Close하지 않으니까 GraphicChild에 Close가 호출되지 않아서 환경 저장 등 정리가 안됨

            SaveFormLocationSize();

            await C_init.ViewProgrammEnd(this);	// 여기서 MessageDisplay를 사용하는데 이부분을 Closed에 사용하니까 MessageDisplay에서 사용할 FormMain이 이미 Dispose되어서 오류가 나는 경우가 있어서 Closing에 넣었다.
            EndAnotherProgramm();
            Debug.WriteLine("FormLocalMain_FormClosed 완료");
        }

        private void FormLocalMain_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGateServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunMain.DataGateServer.FormDataGateServer dialog = new RunMain.DataGateServer.FormDataGateServer();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                FormAbout_ZloT dialog = new FormAbout_ZloT();
                dialog.StartPosition = FormStartPosition.CenterParent;

                dialog.ShowDialog(this);
            }
        }

        private void toolStripMenuItemHelpSystemInformation_Click_1(object sender, EventArgs e)
        {
            if (FormSystemInformation.formThis == null)
            {
                FormSystemInformation form = new FormSystemInformation();
                form.StartPosition = FormStartPosition.CenterParent;
                form.Owner = this;
                form.Show(this);
            }
        }

        //20251110 PSU 다국어 메뉴 추가.
        private void vietnameseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("vi");
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("en");
        }

        private void koreanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("ko");
        }

        private void chineseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("zh-CN");
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("ru");
        }

        private void japaneseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("ja");
        }

        private void globalizationToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            string lang = CultureInfo.DefaultThreadCurrentUICulture.Name.ToLower();

            koreanToolStripMenuItem.Checked = lang.StartsWith("ko");
            chineseToolStripMenuItem.Checked = lang.StartsWith("zh");
            japaneseToolStripMenuItem.Checked = lang.StartsWith("ja");
            englishToolStripMenuItem.Checked = lang.StartsWith("en");
            russianToolStripMenuItem.Checked = lang.StartsWith("ru");
            vietnameseToolStripMenuItem.Checked = lang.StartsWith("vi");
        }

        private FormOpcUaServer _opcServerForm;
        private FormPythonAiDashboard _pythonAiForm;

        private void oPCUAServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_opcServerForm == null || _opcServerForm.IsDisposed)
            {
                _opcServerForm = new FormOpcUaServer();
                _opcServerForm.StartPosition = FormStartPosition.CenterScreen;
                _opcServerForm.FormClosed += (s, args) =>
                {
                    _opcServerForm = null;
                };
                _opcServerForm.Show(this);
            }
            else
            {
                _opcServerForm.BringToFront();
            }
        }

        private void oPCUAClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clientPath = Application.StartupPath + "\\Autobase_OPCUA_Client.exe";
            if (!File.Exists(clientPath))
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show(clientPath, "실행 파일이 존재하지 않습니다.");
                else
                    MessageBox.Show(clientPath, "The exe file does not exist.");
                return;
            }

            // 실행 중인 OPCUA Client 프로세스 찾기 (트레이 상태 포함)
            Process[] processes = Process.GetProcessesByName("Autobase_OPCUA_Client");
            Process runningProcess = null;

            foreach (Process p in processes)
            {
                try
                {
                    string processPath = p.MainModule.FileName;
                    if (processPath.Equals(clientPath, StringComparison.OrdinalIgnoreCase))
                    {
                        runningProcess = p;
                        break;
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (runningProcess == null)
            {
                // 프로세스가 없으면 새로 실행
                Process.Start(clientPath);
            }
            else
            {
                // 이미 실행 중이면 창 활성화 (트레이 상태 포함)
                IntPtr hwnd = runningProcess.MainWindowHandle;
                if (hwnd != IntPtr.Zero)
                {
                    Win32Function.ShowWindow(hwnd, (int)EnumShowWindow.SW_RESTORE);
                    Win32Function.SetForegroundWindow(hwnd);
                }
                else
                {
                    // 트레이 상태: WM_COMMAND로 창 표시 요청
                    Win32Function.PostMessage(runningProcess.MainWindowHandle, 0x111, (IntPtr)(int)EnumIdmPublic.IDM_PUBLIC_ACTIVE_DOCUMENT, IntPtr.Zero);
                }
            }
        }

        private void rESTAPIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ViewRestApiMonitorToolStripMenuItem_Click( sender,  e);
        }

        private void recipeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewrecipesToolStripMenuItem_Click(sender, e);
        }

        private void presetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewpresetsToolStripMenuItem_Click(sender, e);
        }

        private void pythonAIDashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pythonAiForm == null || _pythonAiForm.IsDisposed)
            {
                _pythonAiForm = new FormPythonAiDashboard();
                _pythonAiForm.StartPosition = FormStartPosition.CenterScreen;
                _pythonAiForm.FormClosed += (s, args) =>
                {
                    _pythonAiForm = null;
                };
                _pythonAiForm.Show(this);
            }
            else
            {
                _pythonAiForm.BringToFront();
            }
        }
    }

    /// <summary>
    /// MainForm에 override 할때는 잘되지 않고 메시지 필터를 Application.AddMessageFilter(new MessageFilter()); 와 같이 등록에서 사용하면 잘 된다.
    /// </summary>
    public class MessageFilter : System.Windows.Forms.IMessageFilter
    {
        string sKeyString;

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x200: // WM_MOUSEMOVE
                    SharedData.dtLastMouseMove = DateTime.Now;      // 작업관리자가 떠 있으면 마우스를 움직이지 않아도 WM_MOUSEMOVE 이벤트가 들어오는 경우가 있다.
                    break;

                case 0x111: // WM_COMMAND
                    if (m.WParam == new IntPtr((int)EnumIdmPublic.IDM_PUBLIC_DESTROY_WINDOW) && m.LParam == new IntPtr(12345678))
                    {
                        FormLocalMain.bCloseByCallEditor = true;
                        FormLocalMain.formMain.Close();
                    }

                    break;

                // 바코드 키보드 웨지 스캐너 - WM_CHAR 메시지 처리
                case 0x102: // WM_CHAR
                    {
                        char ch = (char)m.WParam.ToInt32();
                        var kpe = new System.Windows.Forms.KeyPressEventArgs(ch);
                        if (BarcodeManager.ProcessKeyPress(kpe))
                        {
                            return true; // 바코드 스캐너 입력으로 소비됨
                        }
                    }
                    break;

                // ProcessCmdKeys와 ProcessKeyEventArgs에서 override해서 사용하다가 KeyUp메시지를 받을 수 없어서 여기서 0x100과 0x101을 체크한다. 2010.10.27
                case 0x100:
                    SharedData.dtLastKeyDown = DateTime.Now;    // 마지막 키가 눌러진 시간

                    // 바코드 키보드 웨지 스캐너 - KeyDown 메시지 처리 (Enter 키 감지)
                    {
                        var barcodeKeyEventArgs = new System.Windows.Forms.KeyEventArgs((System.Windows.Forms.Keys)m.WParam.ToInt32());
                        if (BarcodeManager.ProcessKeyDown(barcodeKeyEventArgs))
                        {
                            return true; // 바코드 스캐너 완료 키로 소비됨
                        }
                    }

                    if (KeyScript.KeyScriptCodeToString(m.WParam.ToInt32(), out sKeyString))
                    {
                        if (PlayKeyScript(sKeyString, "KeyDown").GetAwaiter().GetResult())
                        {
                            return true;
                        }
                    }

                    break;
                case 0x101:
                    if (KeyScript.KeyScriptCodeToString(m.WParam.ToInt32(), out sKeyString))
                    {
                        if (PlayKeyScript(sKeyString, "KeyUp").GetAwaiter().GetResult())
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;

        }

       async Task<bool> PlayKeyScript(string key_string, string when)
        {
            ScriptClass control;

            string fullpath;

            fullpath = String.Format("{0}\\control\\{1}\\{2}.CTLX", TotalConfig.sDirWorkProject, when, key_string);
            if (!File.Exists(fullpath))
            {
                fullpath = String.Format("{0}\\control\\{1}\\{2}.CTL", TotalConfig.sDirWorkProject, when, key_string);
            }

            if (!File.Exists(fullpath)) return false;	// file not found

            control = new ScriptClass();

            control.LoadFromFile(fullpath);
            control.SetHandOperation();
            await control.RunAsync(FormLocalMain.formMain, null);

            if (control.IsError())
            {
                string message;
                message = control.GetError();
                MessageDisplay.Show(message);
            }

            return true;
        }

    }
}

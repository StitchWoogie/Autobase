using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using AutoLib;
using GraphicModule;
using ViewMainPublic;
using System.Threading;
using NetTools;
using DialogConfigUser;
using System.IO;
using ReportModule;
using System.ServiceModel;
using System.Security.Cryptography;
using AutoLibLocal.KeyLock;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Globalization;
using static AutoLibLocal.LanguageManager;

namespace ViewMain
{
    public partial class FormViewMain : Form
    {
        static Task _dataTask; //task로 변경 251226 PSU
        private CancellationTokenSource _cts;
        ToolBarModule toolBarModule;
        public bool bErrorSecurity = false;

        public FormViewMain()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            // ConfigVarTotal.Init(Program.mainArgs.Length > 0 ? Program.mainArgs[0] : null);

            //MakeFilePath.ResetAllFileFlags();

            TotalConfig.formMain = this;
            TotalConfig.threadMain = Thread.CurrentThread;  // 2016-10-17 추가
            //SharedViewMain.Prepare();

            // @MenuMessage에서 사용할 콜백함수를 등록한다.
            ScriptFunctionMenu.procCallBackMenuMessage = new ScriptFunctionMenu.CallBack(this.CallBackUserMenu);
            ScriptFunctionMenu.procCallBackMenuItemGetFromTitle = new ScriptFunctionMenu.CallBackMenuItemGetFromTitle(CallBackMenuItemGetFromTitle);

            // @MenuChange에서 사용할 콜백함수를 등록한다. 260112 PSU
            ScriptFunctionMenu.procCallBackMenuChange = new ScriptFunctionMenu.CallBackMenuChange(this.UserMenuChange);

            toolBarModule = new ToolBarModule();

            timerMain.Enabled = true;


            /* http://localhost:63497/AutoWeb/Service/ServiceDataGateServer.svc 를 reference 해서 연결하면 잘 동작한다.
            ServiceReference1.ServiceDataGateServerClient s = new ViewMain.ServiceReference1.ServiceDataGateServerClient();

            int conn = s.Connect(); */
        }

        private async void fileLogInToolStripMenuItem_Click(object sender, EventArgs e)
        {
           await LogInOut.AutoBaseLogIn();
        }

        private async void fileLogOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LogInOut.AutoBaseLogOut(true);
        }

        private void filePrintScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemFileScreenPrint_Click(this);
        }

        private void fileScreenSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemFileScreenSave_Click(this);
        }

        private void fileSelectionScreenSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemFileSelectScreenSave_Click(this);
        }

        private void fileExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void viewGraphicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewGraphic_Click();
        }

        private void fileAllTagViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewAllTagView_Click(this);
        }

        private void fileAnalogInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewAnalogInput_Click(this);
        }

        private void fileAnalogOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewAnalogOutput_Click(this);
        }

        private void fileDigitalInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewDigitalInput_Click(this);
        }

        private void fileDigitalOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewDigitalOutput_Click(this);
        }

        private void fileStringTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewStringTag_Click(this);
        }

        private void fileRegisteredGroupViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewRegisterdGroup_Click(this);
        }

        private void fileReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewReport_Click();
        }

        private void fileAlarmFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewAlarmFile_Click(this);
        }

        private void fileLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewLogFile_Click(this);
        }

        private void fileNavigatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewNavigator_Click(this);
        }

        private void configFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemConfigFont_Click();
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void configTotalColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemConfigTotalColor_Click(this);
        }

        private void configAlarmColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemConfigAlarmColor_Click(this);
        }

        private void configUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemConfigUser_Click();
        }

        private void configEtcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigEtc dialog = new FormConfigEtc();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(TotalConfig.formMain);
        }

        private void configWindowTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewWindowTitle_Click(this);
        }

        private void configMainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemViewMainMenu_Click(this, this.menuStrip1);
        }

        private void windowCascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.Cascade);
        }

        private void windowTileHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.TileHorizontal);
        }

        private void windowTileVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.TileVertical);
        }

        private void windowArrangeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.ArrangeIcons);
        }

        private void windowCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
                this.ActiveMdiChild.Close();
        }

        void CloseAllChilds()
        {
            Form[] childForm = this.MdiChildren;
            //Make sure to ask for saving the doc before exiting the app 

            for (int i = 0; i < childForm.Length; i++)
                childForm[i].Close();
        }

        private void windowCloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChilds();            
        }

        private void windowUsernameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewMainPublic.PublicMenu.menuItemUserInformation_Click();
        }

        private void windowAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogCommon.FormAbout dialog = new DialogCommon.FormAbout();

            dialog.ProgramIcon = this.Icon;
            dialog.sFixTextProgram = "Web Viewer (ViewMain)";
            dialog.bDisplayCompany = false;

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(TotalConfig.formMain);
        }

        

        /*
        private void FormViewMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ThreadDataChange.bThreadContinue = false;

            TimeOutClass timeout = new TimeOutClass();
            while (!ThreadDataChange.bThreadEnded)
            {
                if (timeout.IsTimeOut(2)) break;
            }

            timerMain.Enabled = false;

            Form[] childForm = this.MdiChildren;
            //Make sure to ask for saving the doc before exiting the app 
            for (int i = 0; i < childForm.Length; i++)
                childForm[i].Close();

            if (bLogInStatus) // 현재 로그인 중일 때만 로그 아웃.
            {
                DataGate gate = new DataGate();
                gate.LogOut();
            }
        }*/

        private void FormViewMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnInitSite();

            timerMain.Enabled = false;
        }

        void CheckEventTagChanged(TagGrClass gr)
        {
            //COMM_EVENT_STRUCT tagevent = new COMM_EVENT_STRUCT();

            TagPublicClass tp;

            for (int k = 0; k < gr.arrayTag.Count; k++)
            {
                tp = (TagPublicClass)gr.arrayTag[k];
                if (tp.enumTagType == EnumTagType.GR)
                {
                    CheckEventTagChanged((TagGrClass)tp);
                }
                else
                {
                    if (!tp.bChangedDataCurr) continue;

                    tp.bChangedDataCurr = false;

                    /*
                    tagevent.tag = tp.tag;
                    tagevent.tag_type = tp.enumTagType;
                    //tagevent.tag_pos = k;
                    tagevent.message_type = 0;
                    tagevent.tp = tp;*/

                    SharedViewMain.EventGoTagChanged(tp);
                }
            }
        }

        DateTime dtMainTimeOld = DateTimeServer.Now;

        async Task CheckEventTimeChanged()
        {
            DateTime dt = DateTimeServer.Now;

            if (dt.Minute != dtMainTimeOld.Minute)
            {
                await SharedViewMain.EventGoMinuteChanged();
                if (dt.Hour != dtMainTimeOld.Hour)
                {
                    SharedViewMain.EventGoHourChanged();
                    if (dt.Day != dtMainTimeOld.Day)
                    {
                        SharedViewMain.EventGoDayChanged();
                        if (dt.Month != dtMainTimeOld.Month)
                        {
                            SharedViewMain.EventGoMonthChanged();
                            if (dt.Year != dtMainTimeOld.Year)
                            {
                                SharedViewMain.EventGoYearChanged();
                            }
                        }
                    }
                }
            }

            dtMainTimeOld = dt;
        }

        bool bFlagTimerTwice = false;

        int nOldMilli = 0;

        private bool _isTimerProcessing = false;

        //private bool demoMode = false; //250819 PSU

        private async void timerMain_Tick(object sender, EventArgs e)
        {
            if (_isTimerProcessing) return; // 이전 처리가 진행 중이면 건너뛰기

            _isTimerProcessing = true;
            try
            {
                bFlagTimerTwice = !bFlagTimerTwice;
                if (bFlagTimerTwice) return;

                DateTime dt = DateTimeServer.Now;
                int curr = dt.Millisecond + dt.Second * 1000;

                if (curr < nOldMilli) curr = curr + 60000 - nOldMilli;
                else curr = curr - nOldMilli;

                if (curr < timerMain.Interval) return;
                nOldMilli = curr;

                if (WebCommInfo.nFailCount > 0)
                {
                    MessageDisplay.Show(WebCommInfo.sErrorMessage);
                }

                if (DataGate.WebDemo == true)
                {
                    CheckDemoTimeout();  //250825 PSU 추가
                }

                //ThreadDataChange.TimerMain();	// 스레드로 사용하지 않고 테스트 목적으로 돌릴때

                CheckEventTagChanged(TagLib.GetRootGroup());
                await CheckEventTimeChanged();
                await SharedViewMain.EventGoTimerAsync();

                MessageDisplay.CheckMessage();  // 쓰레드에서 호출한 메시지를 표시하기 위해서 필요하다.
                                                // LocalMain.Alarm.AlarmConfirm.CheckAlarmConfirmOnMainTimer();    // 쓰레드에서 생성된 경보확인창을 표시하기 위해서 필요하다. 2016-3-30

                // FormGraphicFrame.ModuleScriptAlwaysTimer();				// 각 모듈의 상시 스크립트를 실행한다. 2013-1-31 부터 FormGraphicChild에서 자체적으로 수행한다.

                //Alarm.AlarmConfirm.AlarmConfirmationStatus();				// 계속경보를 검사한다.
                CheckAlarmEvent.Check();

                dt = DateTimeServer.Now;
                nOldMilli = dt.Millisecond + dt.Second * 1000;
            }
            finally
            {
                _isTimerProcessing = false;
            }
        }

        bool bLogInStatus = false; //WebServer 로그인(접속) 시 true

        private TimeSpan _totalDemoTime = TimeSpan.FromMinutes(30);
        private TimeSpan _remainingDemoTime = TimeSpan.FromMinutes(30);
        private DateTime _startTime = DateTime.Now;
        private DateTime _lastCheckTime = DateTime.MinValue;
        private readonly TimeSpan _minCheckInterval = TimeSpan.FromSeconds(1);

        private void CheckDemoTimeout()   //250825 PSU 추가
        {
            try
            {
                // 마지막 체크로부터 1초가 지나지 않았으면 리턴
                DateTime now = DateTime.Now;
                if (now - _lastCheckTime < _minCheckInterval)
                    return;

                _lastCheckTime = now;

                TimeSpan elapsedTime = DateTime.Now - _startTime;
                _remainingDemoTime = _totalDemoTime - elapsedTime;

                if (_remainingDemoTime.TotalSeconds > 0)
                {
                    string timeText = String.Format("{0:00}:{1:00}", _remainingDemoTime.Minutes, _remainingDemoTime.Seconds);
                    string menuText = Tools.IsLangKorean() ? String.Format("체험판 {0}", timeText) : String.Format("Trial Time {0}", timeText);
                    SetTitle(menuText);
                }


                if (_remainingDemoTime.TotalSeconds <= 0)
                {
                    //CloseAllChilds();
                    //UnInitSite();
                    //SetTitle();
                    CallBackWebClientChangeSite("Local");
                    DataGate.WebDemo = false;

                    if (Tools.IsLangKorean()) MessageBox.Show("30분이 경과하여 체험판이 종료되었습니다.", "체험판");
                    else MessageBox.Show("The trial version has ended as 30 minutes have passed.", "Trial Vesion");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Demo timeout check error: {0}", ex.Message));
            }
        }


        bool LogInOnProgramStart()
        {
            string err_msg = "";
            DataGate gate = new DataGate();

            if (ConfigVarTotal.bLocalFlag)
            {
                string path = MakeFilePath.Users("PUBLIC");
                SharedData.userInfo.LoadUser(out err_msg, path, "PUBLIC");
            }
            else
            {
                string username;
                string gateResult;

                if (gate.DefaultUserCheck(out gateResult, out username, out bool trialMode))
                {
                    err_msg = gateResult;

                    string path = MakeFilePath.Users(username);
                    SharedData.userInfo.LoadUser(out err_msg, path, username);

                    if (!ConfigVarTotal.bLocalFlag && gateResult.ToLower().Contains("mode=trial")) //250825 PSU 추가
                    {
                        if (Tools.IsLangKorean())
                            MessageBox.Show("서버에 키락이 없습니다. 30분이 경과하면 연결이 종료됩니다.", "체험판");
                        else
                            MessageBox.Show("There is no KeyLock on the server. The connection will be terminated after 30 minutes.", "Trial Version");

                        DataGate.WebDemo = true;
                    }
                }
                else
                {
                    FormLogIn login = new FormLogIn();
                    login.StartPosition = FormStartPosition.CenterParent;

                    if (login.ShowDialog(this) != DialogResult.OK)
                    {
                        //Close(); //20250711 PSU 로그인 취소시 프로그램 종료되는 현상 수정
                        return false;
                    }
                }
                bLogInStatus = true;
                HeartbeatService.Start(); //20250825 PSU 추가
            }

            // bLogInStatus = true;
            return true;
        }

        void ExecuteArgument()
        {
            if (Program.mainArgs.Length > 0)
            {
                string argument = Program.mainArgs[0];
                string filename = Path.GetFileName(argument);
                string ext = Path.GetExtension(argument);

                if (String.Compare(ext, ".modx", true) == 0 || String.Compare(ext, ".mod", true) == 0)
                {
                    string path = argument;

                    if (File.Exists(path))
                    {
                        GraphicTool.RestoreGraphicWindow(filename, -1, 0, 0);
                        return;
                    }
                }
                else if (String.Compare(ext, ".rptx", true) == 0 || String.Compare(ext, ".rpt", true) == 0)
                {
                    string path = argument;
                    if (File.Exists(path))
                    {
                        FormDialogOpenReport.OpenReportAtViewMain(path);
                        return;
                    }
                }
                else { }
            }

            if (SharedData.userInfo.bAutoOpenStartPage)
            {

                string filename = MakeFilePath.Graphic(SharedData.userInfo.sStartPage);
                GraphicTool.RestoreGraphicWindow(filename, -1, 0, 0);
            }
        }

        void DisableOnWebRun()
        {
            if (ConfigVarTotal.bLocalFlag) return;

            this.configUserToolStripMenuItem.Enabled = false;
        }

        void SetTitle()
        {
            string text;

            if (ConfigVarTotal.bLocalFlag)
                text = String.Format("Web Viewer (Local)");
            else
            {
                text = String.Format("Web Viewer ({0})", ConfigVarTotal.MakeRootUrl());
            }

            if (!ConfigVarTotal.bWebServiceAlive)
            {
                if(Tools.IsLangKorean())
                    text += " 접속 실패";
                else
                    text += " Connection failed";
            }

            this.Text = text;
        }

        //250825 PSU 추가
        void SetTitle(string demoString)
        {
            string text;

            if (ConfigVarTotal.bLocalFlag)
                text = String.Format("Web Viewer (Local)");
            else
            {
                text = String.Format("Web Viewer {1} ({0})", ConfigVarTotal.MakeRootUrl(), demoString);
            }

            //if (!ConfigVarTotal.bWebServiceAlive)
            //{
            //    if (Tools.IsLangKorean())
            //        text += " 접속 실패";
            //    else
            //        text += " Connection failed";
            //}
            this.Text = text;
        }


        void InitViewMain()
        {
            //SmLog.Message(EnumEventID.ProgramStart, "LocalMain Program Start");

            //TotalConfig.formMain = mainform;

            //tStartProgramm = DateTimeServer.Now;

            AutoLibLocal._LibraryInit.SetGuid(HashTool.MakeHash(AutoLibLocal._LibraryInit.GetGuid() + "AutoLibLocal.dll"));

            //// Write 명령시 이벤트를 받을 위치를 등록한다.
            //TagWrite.localMainEngineSetTagValue = new TagWrite.LocalMainEngineSetTagValue(PlcScan.SetTagValue);
            //TagWrite.localMainEngineSetTagValueDelaySec = new TagWrite.LocalMainEngineSetTagValueDelaySec(PlcScan.SetTagValueDelaySec);

            ScriptFunctionLog.procAutoBaseLogIn = new ScriptFunctionLog.BoolProcVoid(LogInOut.AutoBaseLogIn);
            ScriptFunctionLog.procAutoBaseLogOut = new ScriptFunctionLog.VoidProcBool(LogInOut.AutoBaseLogOut);
            ScriptFunctionLog.procLogInByUsername = new ScriptFunctionLog.VoidProcString(LogInOut.LogInByUsername);

            //ScriptFunctionPlcScan.procReadDelayCommandToPlcScan = new ScriptFunctionPlcScan.IntProcIntIntStringIntInt(PlcScan.ReadDelayCommandToPlcScan);

            //// @MenuMessage에서 사용할 콜백함수를 등록한다.
            //ScriptFunctionMenu.procCallBackMenuMessage = new ScriptFunctionMenu.CallBack(mainform.CallBackUserMenu);
            //// @SetVipScan 에서 사용할 콜백함수를 등록한다.
            //ScriptFunctionSet.procSetVipScan = new ScriptFunctionSet.CallBackSetVipScan(PlcScan.SetVipScan);

            //SharedViewMain.procSaveTrendRemainAI = new AutoLib.SharedViewMain.DelegateVoidProcAi(CheckEngineMinuteChanged.SaveTrendRemainAI);
            //SharedViewMain.procSaveTrendRemainDI = new AutoLib.SharedViewMain.DelegateVoidProcDi(CheckEngineMinuteChanged.SaveTrendRemainDI);

            //SharedViewMain.procAlarmDisplayAI = new AutoLib.SharedViewMain.DelegateAlarmDisplayAI(AlarmDisplay.AlarmDisplayAI);
            //SharedViewMain.procAlarmDisplayDI = new AutoLib.SharedViewMain.DelegateAlarmDisplayDI(AlarmDisplay.AlarmDisplayDI);

            //LibComNetServer.procSendEventProgramToNetwork = new AutoLibLocal.LibComNetServer.DelegateSendEventProgramToNetwork(CheckEngineNetworkToViewMain.SendEventProgramToNetwork);

            SystemValue.Init();     // SystemValueGet, Set을 위한 콜백 함수를 등록한다.

            //// 사용자 정의 제어 상자를 위한 콜백 등록
            DialogControl.ControlBoxAnalogInputGo.procUserControl = new DialogControl.ControlBoxAnalogInputGo.DelegateUserControl(GraphicTool.DisplayUserControlBox);

            //TerminalClass.Init();

            //FormLocalMain.sharedLocalMain = new SharedLocalMain();

            //TagStatus.Load(); 	// 태그 버퍼를 초기화(TagPrapare) 한 다음 한다.
            //SmLog.Message("Tag Load O.K");

            //FormConfigFlowView.LoadFlowWatch();		// 순차 감시파일을 불러온다.

            //C_protect.ReadProtectLevel(ConfigRunMain.sUserName);	// prepare protect
            //// 현재 사용자의 보호 수준을 읽어온다.

            //System.Threading.Thread thread = System.Threading.Thread.CurrentThread;

            //PlcScanEvent.Init();
            //SystemStatusMemory.Init();

            //C_DdeTag.ClientInit();		            // Dde를 태그로 사용하기 위해 준비
            //C_DdeTag.DdeTagLinkAll();				// Dde Tag를 연결해 준다.

            //CheckEngineOnOffList.LoadOnOffListDigitalStatus();

            //LinePrinter.Init();				// line printer를 찾는다.
            //SmsManager.Init();

            //Alarm.AlarmConfirm.LoadAlarmConfirmList();

            //DemandControl.FunctionBlockDemandControlLoad(DemandControl.blockDemandControl);
            //DemandControl.FunctionBlockDemandControlInit(DemandControl.blockDemandControl);

            //CheckEngineSchedule.ScheduleChangeOnDateChanged();	// 스케쥴 파일을 모두 읽어온 다음 오늘 운전할 목록을 준비한다.

            MilliData.LoadMilliData(MilliData.blockMilliData);      // CheckEngineMilliData의 생성자에 있었는데 이곳으로 뺏다. 9.5.2  
                                                                    // ViewMain에서 Local로 사용할 경우가 있으므로 일단 불러온다. 2019-10-10. 불러오기가 부하가 걸리므로 미세자료를 ViewMain에서 볼 수 있도록 가능성을 본다.

            //WatchDogInfo.Init();
            //WatchDogInfo.SetTimer(EnumWatchDogInfo.WDI_LocalMain, 0);

            //DialogTag.TagEditor.FormTagProperty.procOpcList = new DialogTag.TagEditor.FormTagProperty.DelegateOpcList(PublicStudioLocalMain.OpcTool.SelectItem);
            //GraphicModule.ScriptFunctionSms.procSmsSend = new GraphicModule.ScriptFunctionSms.DelegateSmsSend(SmsManager.SmsSend);
            //ScriptFunctionTag.procTagStatusSave = new GraphicModule.ScriptFunctionTag.DelegateTagStatusSave(TagStatus.Save);
            //ScriptFunctionTag.procTagCheckLoop = new GraphicModule.ScriptFunctionTag.DelegateTagCheckLoop(CheckEngineTagChange.CheckSignalChange);

            //DialogTag.TagEditor.Editor.procTagPropertyChanged = new DialogTag.TagEditor.Editor.DelegateTagPropertyChanged(OnTagPropertyChanged);

            //AlarmUtil.procAlarmDataSave = new AutoLibLocal.AlarmUtil.DelegateAlarmDataSave(AlarmDisplay.AlarmDataSave);

            //CheckEngineTagChange.InitSharedTag();
            //CheckEngineNetworkToViewMain.Init();
            //CheckEngineTagChangeThread.Init();
            //PlcScan.OpcInit();

            //ScriptFunctionSchedule.procModuleGetTime = new ScriptFunctionSchedule.DelegateModuleGetTime(Schedule.ByScriptModelGetTime);
            //ScriptFunctionSchedule.procModuleSetTime = new ScriptFunctionSchedule.DelegateModuleSetTime(Schedule.ByScriptModelSetTime);
            //ScriptFunctionSchedule.procReLoad = new ScriptFunctionSchedule.DelegateReLoad(FormSchedule.OnScheduleStructChanged);
            //ScriptFunctionSchedule.procModelSave = new ScriptFunctionSchedule.DelegateModelSave(Schedule.ModelSave);

            //Script.TagEventScript.Init();

            //CheckRealTimeTestData.Init();

            //ConfigViewMain.SetOpcServerStarting();

            //string tag_file = String.Format("{0}\\TAG\\local.tagx", TotalConfig.sDirWorkProject);
            //DialogTag.TagEditor.Editor.checkTagFileChanged.Register(tag_file);  
        }

        void TranslateServerInformations(string infos)
        {
            CommaTextReader comma = new CommaTextReader();

            string buf;

            comma.Set(infos);

            while (!comma.IsEOS())
            {
                buf = comma.GetString();

                if (String.Compare(buf, 0, "TimeZoneSeconds=", 0, 16) == 0)
                {
                    int seconds = ConvertTool.ToInt32(buf.Substring(16));

                    // DateTimeServer.nServerTimeZoneSeconds = seconds; ID가 없을 때는 이것을 사용해야 될 수도 있다. 현재는 ID만을 사용한다.
                }
                else if (String.Compare(buf, 0, "TimeZoneID=", 0, 11) == 0)
                {
                    string id = buf.Substring(11);

                    DateTimeServer.ServierTimeZoneID = id;
                }
                else if (String.Compare(buf, 0, "SecurityLevel=", 0, 14) == 0)
                {
                    string level = buf.Substring(14);

                    ConfigVarTotal.nWebServerSecurityLevel = ConvertTool.ToInt32(level);
                    //DateTimeServer.ServierTimeZoneID = id;
                }
            }
        }

        public void InitSite(string site, Rectangle clientrect)
        {
            WebCommInfo.Reset(); //20250825 PSU 오류 초기화.
            ConfigVarTotal.bWebServiceAlive = true;

            ConfigVarTotal.Init(site);

            ServiceLib.ClearConnection();
            ServiceLibSvcDataGate.ClearConnection();

            if (!ConfigVarTotal.bLocalFlag)
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_Connect");
                    if (retn == 1)
                    {
                        ServiceLibSvcDataGate.nConnectionID = ConvertTool.ToInt32(sldg.GetResultString(0));
                    }
                    else
                    {
                        MessageBox.Show("Cannot connect to server.\n" + sldg.sErrorMessage, ConfigVarTotal.sSiteRootName);
                    }

                    retn = sldg.Command("V2_GetServerVersion");
                    if (retn == 1)
                    {
                        ConfigVarTotal.serverVersion = new Version(sldg.GetResultString(0));
                    }
                    else
                    {
                        MessageBox.Show("Cannot get the Server Version information.\n" + sldg.sErrorMessage, ConfigVarTotal.sSiteRootName);
                    }
                }
                else
                {
                    AutoLib.ServiceReferenceDataTag2.WebServiceDataTag2Client datatag = ServiceLib.GetServiceDataTag2();
                    // 10.2.0.4 버전부터 DataTag2.asmx를 지원한다.

                    try
                    {
                        int retn_val = datatag.CheckServiceAlive(0x555);
                    }
                    catch (Exception exception)
                    {
                        string msg = String.Format("Cannot connect to the Web Server.\nCheck the Web Server status or URL name.\n\nMessage={0}", exception.Message);
                        MessageBox.Show(msg, "Site="+ConfigVarTotal.sSiteRootName);
                        ConfigVarTotal.bWebServiceAlive = false;
                        SetTitle();
                        return;
                    }

                    AutoLib.ServiceReferenceDataTag2.WebServiceDataTag2Client datatag2 = ServiceLib.GetServiceDataTag2();
                    
                    try
                    {
                        string version = datatag2.GetServerVersion();
                        ConfigVarTotal.serverVersion = new Version(version);
                    }
                    catch
                    {
                        ConfigVarTotal.serverVersion = null;    // 버전을 읽을 수가 없다.
                    }

                    if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 2, 9, 11))
                    {
                        string infos = "";

                        infos = datatag2.GetServerInformations();
                        
                        TranslateServerInformations(infos);
                    }
                }

                // 서버 정보 가져오기 250827 PSU
                if (!ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 3, 7, 5))
                {

                    string msg, caption;
                    if (Tools.IsLangKorean())
                    {
                        msg = String.Format("웹 서버 버전이 낮습니다. (웹 서버 버전: {0})\n웹 서버를 v10.3.7.5 이상으로 업그레이드 후 사용하세요.",
                           ConfigVarTotal.serverVersion.ToString());
                        caption = "접속 불가";
                    }
                    else
                    {
                        msg =
                        String.Format("The version of the web server is low. (Web server version: {0})\nPlease upgrade the web server to v10.3.7.5 or higher before using.",
                            ConfigVarTotal.serverVersion.ToString());
                        caption = "Connection unavailable";
                    }

                    MessageBox.Show(msg, caption);
                    return;
                }

                int MAX_CLIENT_SECURITY_LEVEL = 4;  // 클라이언트가 지원하는 최고 보안 레벨

                if (ConfigVarTotal.nWebServerSecurityLevel > MAX_CLIENT_SECURITY_LEVEL)
                {
                    string msg;

                    if (Tools.IsLangKorean())
                    {
                        msg = String.Format("웹서버가 요구하는 보안레벨(SecurityLevel)로 접속해야 합니다.\n(요구되는 레벨={0}, 클라이언트 레벨={1})\n웹클라이언트를 업그레이드 한 후 사용하세요.", ConfigVarTotal.nWebServerSecurityLevel, MAX_CLIENT_SECURITY_LEVEL);
                        MessageBox.Show(msg, "보안레벨 부족");
                    }
                    else
                    {
                        msg = String.Format("Web Server SecurityLevel is too high.\n(Needed Level={0}, Current Level={1})\nUpgrade WebClient program.", ConfigVarTotal.nWebServerSecurityLevel, MAX_CLIENT_SECURITY_LEVEL);
                        MessageBox.Show(msg, "Security Level Mismatched");
                    }

                    return;
                }

                // SecurityLevel 3 부터 대화 ID가 있다.
                if (ConfigVarTotal.nWebServerSecurityLevel == 3)
                {
                    AutoLib.ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

                    Random rand = new Random();

                    string key2_enc, key3_enc, key4_enc, key5_enc;
                    string key_enc = service.GetConversationKey(rand.Next().ToString(), out key2_enc, out key3_enc, out key4_enc, out key5_enc);
                    DateTime t = DateTime.UtcNow;
                    string key = ServiceLib.Decrypt(key_enc);

                    ConfigVarTotal.nTickGab = (t.Ticks - ConvertTool.ToInt64(key));
                }
                // SecurityLevel 3 부터 대화 ID가 있다.
                else if (ConfigVarTotal.nWebServerSecurityLevel == 4)   // 3과 같고 Hash만 다르다.
                {
                    AutoLib.ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

                    Random rand = new Random();

                    string key2_enc, key3_enc, key4_enc, key5_enc;
                    string key_enc = service.GetConversationKey(rand.Next().ToString(), out key2_enc, out key3_enc, out key4_enc, out key5_enc);
                    DateTime t = DateTime.UtcNow;
                    string key = ServiceLib.Decrypt(key_enc);

                    ConfigVarTotal.nTickGab = (t.Ticks - ConvertTool.ToInt64(key));
                }
                else
                {

                }
            }
                                                
            MakeFilePath.ResetAllFileFlags();

            SharedViewMain.Prepare();

            TerminalClass.Init();

            if (LogInOnProgramStart() == false)
            {
                return;
            }

            //로그인 후 시작으로 변경 20250711 PSU
            // 스레드가 이미 존재하는지 확인
            if (_dataTask == null || _dataTask.IsCompleted)
            {
                _cts = new CancellationTokenSource();

                _dataTask = Task.Run(() =>
                     ThreadDataChange.ThreadMainAsync(_cts.Token),
                     _cts.Token);

                //thread1 = new Thread(new ThreadStart(ThreadDataChange.ThreadMainAsync));
                //thread1.Name = "ViewMainDataGather";
                //thread1.Start();
            }
            else
            {
                // Task 실행 중이므로 종료 없이 site 변경만 반영
                ThreadDataChange.bThreadContinue = true;
            }

            ToolBarModule.LoadList();
            ToolBarModule.FrameToolBarCreate(this);

            // 각 사이트마다 환경이 다를 수 있으므로 사용자 정의 박스가 들어있는 파일을 다시 읽어준다. 2011.6.27
            TotalConfigProject.sConfigFileNameOnWebView = MakeFilePath.Project("Config", "ProjectConfig.inix");
            ConfigViewMain.Load();

            _startTime = DateTime.Now; //demo mode에 필요한 시작시간 초기화 250825 PSU
        }

        public void UnInitSite()
        {
            ToolBarModule.Close();

            _cts?.Cancel();

            ThreadDataChange.bThreadContinue = false;

            TimeOutClass timeout = new TimeOutClass();
            while (!ThreadDataChange.bThreadEnded)
            {
                if (timeout.IsTimeOut(2)) break;
            }

            //250825 PSU 추가
            DataGate.WebDemo = false;
            _remainingDemoTime = TimeSpan.FromMinutes(30); //데모사용시간 초기화
            HeartbeatService.Stop();

            if (bLogInStatus) // 현재 로그인 중일 때만 로그 아웃.
            {
                bLogInStatus = false;   //250825 PSU 추가
                DataGate gate = new DataGate();
                gate.LogOut();
            }
        }

        /// <summary>
        /// argument를 분석한다.
        /// </summary>
        /// <returns>site 이름을 반환한다. null 이면 없는 것</returns>
        string ArgumentSplit()
        {
            string site_name = null;

            string arg;
            for (int i = 0; i < Program.mainArgs.Length; i++)
            {
                arg = Program.mainArgs[i];
                // WCF로 통신하고 싶으면 프로그램 시작 시 Service=wcf 를 추가하면 된다.
                if (String.Compare(arg, 0, "Service=", 0, 8, true) == 0)
                {
                    string val = arg.Substring(8);

                    if (String.Compare(val, "Wcf", true) == 0 ||
                        String.Compare(val, "WcfService", true) == 0)
                    {
                        ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
                    }
                    /*
                    if (String.Compare(val, "", true) == 0)
                    {
                        ServiceLibSvcDataGate.eBindType = EnumDataGateBindingType.NetTcp;
                        ServiceLib.eServiceType = EnumServiceType.WcfService;
                    }
                    else if (String.Compare(val, "NetTcp", true) == 0)
                    {
                        ServiceLibSvcDataGate.eBindType = EnumDataGateBindingType.NetTcp;
                        ServiceLib.eServiceType = EnumServiceType.WcfService;
                    }
                    else if (String.Compare(val, "http", true) == 0)
                    {
                        ServiceLibSvcDataGate.eBindType = EnumDataGateBindingType.BasicHttp;
                        ServiceLib.eServiceType = EnumServiceType.WcfService;
                    }
                    else
                    {
                        ServiceLib.eServiceType = EnumServiceType.WebService;
                    }*/
                }
                else if (String.Compare(arg, 0, "Site=", 0, 5, true) == 0)
                {
                    /*
                    string val = arg.Substring(8);
                    if(String.Compare(val, 0, "tcp.net", 0, 7, true) == 0)
                        ServiceLib.eServiceType = EnumServiceType.WcfService;*/

                    site_name = arg;
                }
                /*
                else if (String.Compare(arg, 0, "SSL=", 0, 4, true) == 0)
                {
                    ConfigVarTotal.bSSL = ConvertTool.ToBoolean(arg.Substring(4));
                }*/
            }

            return site_name;
        }

        private void FormViewMain_Load(object sender, EventArgs e)
        {
            InitViewMain();

            DisableOnWebRun();

            string site_name = ArgumentSplit();

            InitSite(site_name, this.ClientRectangle);

            //if (!bLogInStatus) //250826 PSU 삭제
            //{
            //    Close();
            //    return;
            //}

            SetTitle();

            MenuStrip userMainMenu = C_Menu.LoadAutoBaseMenuWeb(new C_Menu.CallBack(this.CallBackUserMenu));

            if (userMainMenu != null)
            {
                this.menuStrip1.Items.Clear();

                int count = userMainMenu.Items.Count;

                for (int i = 0; i < count; i++)
                {
                    this.menuStrip1.Items.Add(userMainMenu.Items[0]);    // Add하는 순간 userMainManu에 있는 아이템이 하나씩 자동으로 빠지는 현상이 있어서 count를 저장해서 사용 
                }
            }

            ViewMainPublic.PublicMenu.EnableDisableMainTitle(this);
            ViewMainPublic.PublicMenu.EnableDisableMainMenu(this, this.menuStrip1);

            this.Activate();

            ExecuteArgument();

            ScriptFunctionWeb.procWebClientChangeSite = new ScriptFunctionWeb.DelegateWebClientChangeSite(CallBackWebClientChangeSite);

            LanguageManager.EventLanguageChanged += OnLanguageChanged; //다국어 이벤트 추가 20251119 PSU 
        }

        public bool UserMenuChange(string menuName)
        {
            MenuStrip userMainMenu = C_Menu.LoadAutoBaseMenuWeb(menuName);

            if (userMainMenu != null)
            {
                this.menuStrip1.Items.Clear();

                int count = userMainMenu.Items.Count;

                for (int i = 0; i < count; i++)
                {
                    this.menuStrip1.Items.Add(userMainMenu.Items[0]);    // Add하는 순간 userMainManu에 있는 아이템이 하나씩 자동으로 빠지는 현상이 있어서 count를 저장해서 사용 
                }

                ViewMainPublic.PublicMenu.EnableDisableMainTitle(this);
                ViewMainPublic.PublicMenu.EnableDisableMainMenu(this, this.menuStrip1);

                return true;
            }
            else return false;
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

            // MenuStrip 및 ToolStripMenuItem 처리
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


        public System.EventHandler CallBackUserMenu(ToolStripMenuItem mi, string s)
        {
            if (String.Compare(s, "LOGIN") == 0) return new System.EventHandler(this.fileLogInToolStripMenuItem_Click);
            else if (String.Compare(s, "LOGOUT") == 0) return new System.EventHandler(this.fileLogOutToolStripMenuItem_Click);
            //else if (String.Compare(s, "SEND_MAIL") == 0) return new System.EventHandler(this.menuItemFileSendMail_Click);
            //else if (String.Compare(s, "READ_MAIL") == 0) return new System.EventHandler(this.menuItemFileReadMail_Click);
            else if (String.Compare(s, "PRINT_SCREEN") == 0) return new System.EventHandler(this.filePrintScreenToolStripMenuItem_Click);
            else if (String.Compare(s, "SAVE_SCREEN") == 0) return new System.EventHandler(this.fileScreenSaveToolStripMenuItem_Click);
            else if (String.Compare(s, "SAVE_SCREEN_ZONE") == 0) return new System.EventHandler(this.fileSelectionScreenSaveToolStripMenuItem_Click);
            //else if (String.Compare(s, "RUN_STUDIO") == 0) return new System.EventHandler(this.menuItemRunEditor_Click);
            else if (String.Compare(s, "EXIT") == 0) return new System.EventHandler(this.fileExitToolStripMenuItem_Click);

            else if (String.Compare(s, "VIEW_GRAPHIC") == 0) return new System.EventHandler(this.viewGraphicToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ALLTAG_LIST") == 0) return new System.EventHandler(this.fileAllTagViewToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ANALOG_INPUT") == 0) return new System.EventHandler(this.fileAnalogInputToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ANALOG_OUTPUT") == 0) return new System.EventHandler(this.fileAnalogOutputToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_DIGITAL_INPUT") == 0) return new System.EventHandler(this.fileDigitalInputToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_DIGITAL_OUTPUT") == 0) return new System.EventHandler(this.fileDigitalOutputToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_ALARM") == 0) return new System.EventHandler(this.fileAlarmFileToolStripMenuItem_Click);
            //else if (String.Compare(s, "VIEW_ALARM_EVENT_BOX") == 0) return new System.EventHandler(this.menuItemViewAlarmEvent_Click);
            else if (String.Compare(s, "VIEW_LOG") == 0) return new System.EventHandler(this.fileLogFileToolStripMenuItem_Click);
            //else if (String.Compare(s, "VIEW_PCL") == 0) return new System.EventHandler(this.menuItemViewAlwaysScript_Click);
            //else if (String.Compare(s, "VIEW_SCANBUF") == 0) return new System.EventHandler(this.menuItemViewPlcScan_Click);
            else if (String.Compare(s, "VIEW_STRING_TAG") == 0) return new System.EventHandler(this.fileStringTagToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_GROUP_SHOW") == 0) return new System.EventHandler(this.fileRegisteredGroupViewToolStripMenuItem_Click);
            else if (String.Compare(s, "VIEW_REPORT") == 0) return new System.EventHandler(this.fileReportToolStripMenuItem_Click);

            else if (String.Compare(s, "VIEW_GROUPTAG_DO") == 0) return null;

            //else if (String.Compare(s, "VIEW_DEMAND_CONTROL") == 0) return new System.EventHandler(this.menuItemViewDemandControl_Click);

            //else if (String.Compare(s, "VIEW_MILLI_DATA") == 0) return new System.EventHandler(this.menuItemViewMilliData_Click);
            //else if (String.Compare(s, "VIEW_SCHEDULE") == 0) return new System.EventHandler(this.menuItemViewSchedule_Click);
            //else if (String.Compare(s, "VIEW_DB_ADDITION") == 0) return new System.EventHandler(this.menuItemViewDbAddition_Click);

            else if (String.Compare(s, "VIEW_NAVIGATOR") == 0) return new System.EventHandler(this.fileNavigatorToolStripMenuItem_Click);

            //else if (String.Compare(s, "CONFIG_ALARM") == 0) return new System.EventHandler(this.menuItemConfigAlarm_Click);
            else if (String.Compare(s, "CONFIG_FONT") == 0) return new System.EventHandler(this.configFontToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_COLOR") == 0) return new System.EventHandler(this.configTotalColorToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_ALARM_COLOR") == 0) return new System.EventHandler(this.configAlarmColorToolStripMenuItem_Click);
            //else if (String.Compare(s, "CONFIG_DATA") == 0) return new System.EventHandler(this.menuItemConfigBasicData_Click);

            //else if (String.Compare(s, "CONFIG_FLOW_WATCH") == 0 || String.Compare(s, "CONFIG_FLOW_VIEW") == 0)
            //   return new System.EventHandler(this.menuItemConfigFlowWatch_Click);

            else if (String.Compare(s, "CONFIG_USER") == 0) return new System.EventHandler(this.configUserToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_DDE") == 0) return null;
            else if (String.Compare(s, "CONFIG_GRAPHIC_MODULE_DATA") == 0) return null;
            else if (String.Compare(s, "CONFIG_PRINT_SCRIPT_PRINT_MODULE") == 0) return null;
            else if (String.Compare(s, "CONFIG_ETC") == 0) return new System.EventHandler(this.configEtcToolStripMenuItem_Click);
            //else if (String.Compare(s, "CONFIG_EXCEL_PATH") == 0) return new System.EventHandler(this.menuItemExcelPath_Click);

            //else if (String.Compare(s, "CONFIG_DSN") == 0) return new System.EventHandler(this.menuItemConfigDatabaseConnection_Click);
            //else if (String.Compare(s, "CONFIG_SHARED_DATABASE") == 0) return new System.EventHandler(this.menuItemConfigSharedDatabase_Click);

            //else if (String.Compare(s, "CONFIG_SCHEDULE_FIXED") == 0) return new System.EventHandler(this.menuItemConfigScheduleFixed_Click);
            //else if (String.Compare(s, "CONFIG_SCHEDULE_ADDITIONAL") == 0) return new System.EventHandler(this.menuItemConfigScheduleAdditional_Click);
            //else if (String.Compare(s, "CONFIG_SCHEDULE_MODEL") == 0) return new System.EventHandler(this.menuItemConfigScheduleModel_Click);
            //else if (String.Compare(s, "CONFIG_SCHEDULE_WEEK") == 0) return new System.EventHandler(this.menuItemConfigScheduleWeek_Click);

            //else if (String.Compare(s, "CONFIG_REPORT_AUTO_PRINT") == 0) return new System.EventHandler(this.menuItemConfigReportAutoPrint_Click);

            else if (String.Compare(s, "CONFIG_CAPTION") == 0) return new System.EventHandler(this.configWindowTitleToolStripMenuItem_Click);
            else if (String.Compare(s, "CONFIG_MENU") == 0) return new System.EventHandler(this.configMainMenuToolStripMenuItem_Click);

            else if (String.Compare(s, "WINDOW_CASCADE") == 0) return new System.EventHandler(this.windowCascadeToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_TILE") == 0) return new System.EventHandler(this.windowTileHorizontallyToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_TILE_HORZ") == 0) return new System.EventHandler(this.windowTileHorizontallyToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_TILE_VERT") == 0) return new System.EventHandler(this.windowTileVerticallyToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_ARRANGE") == 0) return new System.EventHandler(this.windowArrangeIconToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_CLOSE") == 0) return new System.EventHandler(this.windowCloseToolStripMenuItem_Click);
            else if (String.Compare(s, "WINDOW_CLOSEALL") == 0) return new System.EventHandler(this.windowCloseAllToolStripMenuItem_Click);

            //else if (String.Compare(s, "HELP") == 0) return new System.EventHandler(this.menuItemHelp_Click);
            else if (String.Compare(s, "WHOAMI") == 0) return new System.EventHandler(this.windowUsernameToolStripMenuItem_Click);
            else if (String.Compare(s, "ABOUT") == 0) return new System.EventHandler(this.windowAboutToolStripMenuItem_Click);
            //else if (String.Compare(s, "KEYLOCK_INFO") == 0) return new System.EventHandler(this.menuItemHelpKeyLock_Click);

            //else if (String.Compare(s, "TOOL_MODULE_EDIT") == 0) return new System.EventHandler(this.menuItemRunEditor_Click);
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
            return RecurseCallBackMenuItemGetFromTitle(this.menuStrip1.Items, title);
        }

        private void viewToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            fileNavigatorToolStripMenuItem.Checked = (SharedData.formNavigator != null);
        }

        private void FormViewMain_MdiChildActivate(object sender, EventArgs e)
        {
            ViewNavigator.NavigatorChangeModule();
        }

        /*
        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }*/

        private void FormViewMain_SizeChanged(object sender, EventArgs e)
        {
            ToolBarModule.MoveFrameToolBar(this);
        }

        private void configToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            this.configWindowTitleToolStripMenuItem.Checked = ConfigViewMain.bShowMainTitle;
            this.configMainMenuToolStripMenuItem.Checked = ConfigViewMain.bShowMainMenu;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == 0x100)	// key Down
            {
                /*
                if (keyData == Keys.F5)
                {
                    if (this.Menu == null)
                    {
                        this.Menu = mainMenu1;
                        ConfigViewMain.bShowMainMenu = true;
                        ConfigViewMain.Save();
                        return true;
                    }
                }*/
            }
            else if (msg.Msg == 0x104)  // WM_SYSKEYDOWN
            {
                if (keyData == Keys.F10)
                {
                    if (this.menuStrip1.Visible == false)
                    {
                        this.menuStrip1.Visible = true;
                        ConfigViewMain.bShowMainMenu = true;
                        ConfigViewMain.Save();
                        return true;
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void CallBackWebClientChangeSite(string site)
        {
            CloseAllChilds();

            UnInitSite();
            InitSite(site, this.ClientRectangle);
            SetTitle();

            //250825 PSU 추가
            if (!ConfigVarTotal.bLocalFlag && bLogInStatus) HeartbeatService.Start();

            if (bLogInStatus)
            {
                ExecuteArgument();
            }
        }

        private void toolStripMenuItemFileSite_Click(object sender, EventArgs e)
        {
            FormSelectSite dialog = new FormSelectSite();

            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(TotalConfig.formMain) == DialogResult.OK)
            {
                CallBackWebClientChangeSite(dialog.sSelectedSite);

                /*
                CloseAllChilds();

                UnInitSite();
                InitSite(dialog.sSelectedSite, this.ClientRectangle);
                SetTitle();

                if (bLogInStatus)
                {
                    ExecuteArgument();
                }*/
            }
        }

        private void FormViewMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void toolStripMenuItemViewAlarmEvent_Click(object sender, EventArgs e)
        {
            if (Alarm.AlarmConfirm.IsPopupAlarmConfirmation())
                Alarm.AlarmConfirm.CreatePopupAlarmConfirmation(false);
            else
                Alarm.AlarmConfirm.CreatePopupAlarmConfirmation(true);
        }

        private void toolStripMenuItemHelpWebServerVersion_Click(object sender, EventArgs e)
        {
            string msg;

            if (Tools.IsLangKorean())
            {
                msg = String.Format("현재 접속되어 있는 웹서버의 버전입니다.\n\n버전={0}", ConfigVarTotal.serverVersion);

                MessageBox.Show(msg, "웹서버 버전");
            }
            else
            {
                msg = String.Format("Web Server Version currently connected.\n\nVersion={0}", ConfigVarTotal.serverVersion);

                MessageBox.Show(msg, "Web Server Version");
            }
        }

        private async void currentUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ConfigVarTotal.bLocalFlag)
            {
                try
                {
                    int realKeylockCount = 2;
                    DataGate gate = new DataGate();
                    var result = await gate.GetUserCount();
                    if (result.KeylockCountk__BackingField< 2) realKeylockCount = 0;
                    else realKeylockCount = result.KeylockCountk__BackingField;

                    string msg = Tools.IsLangKorean() ? String.Format("라이센스 사용자 : {0}/{1}\n체험판 사용자 : {2}/2", result.LicenseCountk__BackingField, realKeylockCount, result.TrialCountk__BackingField)
                        : String.Format("License Users: {0}/{1}\nTrial Users: {2}/2", result.LicenseCountk__BackingField, realKeylockCount, result.TrialCountk__BackingField);
                    MessageBox.Show(msg, Tools.IsLangKorean() ? "동시접속자 수" : "Current Users");
                }
                catch
                {
                    if (Tools.IsLangKorean())
                    {
                        MessageBox.Show("동시접속자 수 확인 불가", "오류");
                    }
                    else
                    {
                        MessageBox.Show("Cannot check the number of simultaneous connections.", "Error");
                    }
                }

            }
        }

        //20251119 PSU 다국어 메뉴 추가.
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

        private void japaneseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("ja");
        }

        private void russainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("ru");
        }

        private void vietanmeseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("vi");
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
    }
}

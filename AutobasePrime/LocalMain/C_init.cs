using System;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using GraphicModule;
using System.IO;
using System.Runtime.InteropServices;
using RunMain;
using NetTools;
using System.Threading.Tasks;
using LocalMain.Alarm;
using System.Collections.Generic;
using LocalMain.OPCUA;
using LocalMain.DemandNew;
using LocalMain.PythonAi;
using LocalMain.Barcode;
using OPCUA.Client.Core;
using System.Diagnostics;

namespace LocalMain
{
    /// <summary>
    /// Summary description for C_init.
    /// </summary>
    public class C_init
    {
        public C_init()
        {
            //
            // TODO: Add constructor logic here 
            //
        }

        static DateTime tStartProgramm;

        public static async Task ViewProgrammStart(FormLocalMain mainform)
        {
            Log.procLog = new Log.DelegateLog(SmLog.Message);

            if (TotalConfig.eOemType == EnumOemType.SBAS)
                SmLog.Message(EnumEventID.ProgramStart, "SBAS 감시 프로그램 시작");
            else
                SmLog.Message(EnumEventID.ProgramStart, "LocalMain Program Start");

            TotalConfig.formMain = mainform;

            tStartProgramm = DateTime.Now;

            AutoLibLocal._LibraryInit.SetGuid(HashTool.MakeHash(AutoLibLocal._LibraryInit.GetGuid() + "AutoLibLocal.dll"));

            // Write 명령시 이벤트를 받을 위치를 등록한다.
            TagWrite.localMainEngineSetTagValue = new TagWrite.LocalMainEngineSetTagValue(PlcScan.SetTagValue);
            TagWrite.localMainEngineSetTagValueDelaySec = new TagWrite.LocalMainEngineSetTagValueDelaySec(PlcScan.SetTagValueDelaySec);

            ScriptFunctionLog.procAutoBaseLogIn = new ScriptFunctionLog.BoolProcVoid(C_protect.AutoBaseLogIn);
            ScriptFunctionLog.procAutoBaseLogOut = new ScriptFunctionLog.VoidProcBool(C_protect.AutoBaseLogOut);
            ScriptFunctionLog.procLogInByUsername = new ScriptFunctionLog.VoidProcString(C_protect.LogInByUsername);

            ScriptFunctionPlcScan.procReadDelayCommandToPlcScan = new ScriptFunctionPlcScan.IntProcIntIntStringIntInt(PlcScan.ReadDelayCommandToPlcScan);
            ScriptFunctionPlcScan.procAnalogOutput = new ScriptFunctionPlcScan.DelegateAnalogOutput(PlcScan.AnalogOutputToPlcScan);
            ScriptFunctionPlcScan.procDigitalOutput = new ScriptFunctionPlcScan.DelegateDigitalOutput(PlcScan.DigitalOutputToPlcScan);
            ScriptFunctionPlcScan.procBlockOutput = new ScriptFunctionPlcScan.DelegateBlockOutput(PlcScan.BlockOutputToPlcScan);

            // @MenuMessage에서 사용할 콜백함수를 등록한다.
            ScriptFunctionMenu.procCallBackMenuMessage = new ScriptFunctionMenu.CallBack(mainform.CallBackUserMenu);
            ScriptFunctionMenu.procCallBackMenuItemGetFromTitle = new ScriptFunctionMenu.CallBackMenuItemGetFromTitle(mainform.CallBackMenuItemGetFromTitle);

            //@MenuChange 에서 사용할 콜백함수를 등록한다. 260112 PSU
            ScriptFunctionMenu.procCallBackMenuChange = new ScriptFunctionMenu.CallBackMenuChange(mainform.UserMenuChange);

            // @SetVipScan 에서 사용할 콜백함수를 등록한다.
            ScriptFunctionSet.procSetVipScan = new ScriptFunctionSet.CallBackSetVipScan(PlcScan.SetVipScan);

            SharedViewMain.procSaveTrendRemainAI = new AutoLib.SharedViewMain.DelegateVoidProcAi(CheckEngineMinuteChanged.SaveTrendRemainAI);
            SharedViewMain.procSaveTrendRemainDI = new AutoLib.SharedViewMain.DelegateVoidProcDi(CheckEngineMinuteChanged.SaveTrendRemainDI);

            SharedViewMain.procAlarmDisplayAI = new AutoLib.SharedViewMain.DelegateAlarmDisplayAI(AlarmDisplay.AlarmDisplayAI);
            SharedViewMain.procAlarmDisplayDI = new AutoLib.SharedViewMain.DelegateAlarmDisplayDI(AlarmDisplay.AlarmDisplayDI);

            LibComNetServer.procSendEventProgramToNetwork = new AutoLibLocal.LibComNetServer.DelegateSendEventProgramToNetwork(CheckEngineNetworkToViewMain.SendEventProgramToNetwork);

            SystemValue.Init();     // SystemValueGet, Set을 위한 콜백 함수를 등록한다.

            // 사용자 정의 제어 상자를 위한 콜백 등록
            DialogControl.ControlBoxAnalogInputGo.procUserControl = new DialogControl.ControlBoxAnalogInputGo.DelegateUserControl(GraphicTool.DisplayUserControlBox);

            TerminalClass.Init();

            FormLocalMain.sharedLocalMain = new SharedLocalMain();

            TagStatus.Load(); 	// 태그 버퍼를 초기화(TagPrapare) 한 다음 한다.

            if (TotalConfig.eOemType == EnumOemType.SBAS)
                SmLog.LogInfo(LogCategory.SYSTEM, "태그 불러오기 성공");
            else
                SmLog.LogInfo(LogCategory.SYSTEM, "Tag Load O.K");

            //PrepareDataDirectory(hwnd);			// 데이터 폴더를 준비한다.

            FormConfigFlowView.LoadFlowWatch();     // 순차 감시파일을 불러온다.

            //ProtectShareInit();
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                // SBAS 는 _Public 으로 기본로그인 한다.
            }
            else
            {
                C_protect.ReadProtectLevel(ConfigRunMain.sUserName);	// prepare protect
            }
            // 현재 사용자의 보호 수준을 읽어온다.

            //LoadUserConfig(config.sUserName); 	// 개인 사용자 환경을 불러온다.

            //ViewFontChange(&configUser.logFont);// 반드시 User config file 을 load 한 후에 한다.

            System.Threading.Thread thread = System.Threading.Thread.CurrentThread;

            PlcScanEvent.Init();
            SystemStatusMemory.Init();

            //PrepareAutoDataFileMake();			// 자동으로 Data File을 저장해주는 목록을 설치한다.
            //PrepareForecastDianosis();			//	예측 진단 기능 파일을 읽어서 준비한다.

            C_DdeTag.ClientInit();                  // Dde를 태그로 사용하기 위해 준비
            C_DdeTag.DdeTagLinkAll();               // Dde Tag를 연결해 준다.

            CheckEngineOnOffList.LoadOnOffListDigitalStatus();

            LinePrinter.Init();             // line printer를 찾는다.
            SmsManager.Init();

            Alarm.AlarmConfirm.LoadAlarmConfirmList();

            //LoadAlarmStatusToDigitalOutList();

            DemandControl.FunctionBlockDemandControlLoad(DemandControl.blockDemandControl);
            DemandControl.FunctionBlockDemandControlInit(DemandControl.blockDemandControl);

            CheckEngineDemandNew.Initialize();

            //AlarmRecvEventThreadInit();
            CheckEngineSchedule.ScheduleChangeOnDateChanged();  // 스케쥴 파일을 모두 읽어온 다음 오늘 운전할 목록을 준비한다.

            //WebServerInit(false);
            //DatabaseConnectionListLoad();

            MilliData.LoadMilliData(MilliData.blockMilliData); // CheckEngineMilliData의 생성자에 있었는데 이곳으로 뺏다. 9.5.2
            CheckEngineMilliData.Init();

            // Recipe 초기화
            RecipeManager.Init();

            WatchDogInfo.Init();
            WatchDogInfo.SetTimer(EnumWatchDogInfo.WDI_LocalMain, 0);

            DialogTag.TagEditor.FormTagProperty.procOpcList = new DialogTag.TagEditor.FormTagProperty.DelegateOpcList(OpcTool.SelectItem);
            DialogTag.TagEditor.FormTagProperty.procOpcListUA = new DialogTag.TagEditor.FormTagProperty.DelegateOpcList(OpcTool.SelectItemUA); // OPC UA 24-09-02 추가 hsjeong
            GraphicModule.ScriptFunctionSms.procSmsSend = new GraphicModule.ScriptFunctionSms.DelegateSmsSend(SmsManager.SmsSend);
            ScriptFunctionTag.procTagStatusSave = new GraphicModule.ScriptFunctionTag.DelegateTagStatusSave(TagStatus.Save);
            ScriptFunctionTag.procTagCheckLoop = new GraphicModule.ScriptFunctionTag.DelegateTagCheckLoop(CheckEngineTagChange.CheckSignalChange);

            DialogTag.TagEditor.Editor.procTagPropertyChanged = new DialogTag.TagEditor.Editor.DelegateTagPropertyChanged(OnTagPropertyChanged);

            AlarmUtil.procAlarmDataSave = new AutoLibLocal.AlarmUtil.DelegateAlarmDataSave(AlarmDisplay.AlarmDataSave);

            CheckEngineTagChange.InitSharedTag();
            CheckEngineNetworkToViewMain.Init();
            CheckEngineTagChangeThread.Init();
            PlcScan.OpcInit();

            ScriptFunctionSchedule.procModuleGetTime = new ScriptFunctionSchedule.DelegateModuleGetTime(Schedule.ByScriptModelGetTime);
            ScriptFunctionSchedule.procModuleSetTime = new ScriptFunctionSchedule.DelegateModuleSetTime(Schedule.ByScriptModelSetTime);
            ScriptFunctionSchedule.procReLoad = new ScriptFunctionSchedule.DelegateReLoad(FormSchedule.OnScheduleStructChanged);
            ScriptFunctionSchedule.procModelSave = new ScriptFunctionSchedule.DelegateModelSave(Schedule.ModelSave);
            ScriptFunctionSchedule.procCommonCallBack = new ScriptFunctionScript.DelegateScriptLocalCallBack(Schedule.LocalScriptCallBack);

            // Recipe 스크립트 함수 델리게이트 연결 (기존 하위호환 + 신규 Unit 대응)
            ScriptFunctionRecipe.procRecipeDownload = (name) => CheckEngineRecipe.RecipeDownload(name, null);
            ScriptFunctionRecipe.procRecipeUpload = (name) => CheckEngineRecipe.RecipeUpload(name, null);
            ScriptFunctionRecipe.procRecipeReLoad = new ScriptFunctionRecipe.DelegateRecipeReLoad(RecipeManager.ReLoad);
            ScriptFunctionRecipe.procRecipeDownloadUnit = (name, unit) => CheckEngineRecipe.RecipeDownload(name, unit);
            ScriptFunctionRecipe.procRecipeUploadUnit = (name, unit) => CheckEngineRecipe.RecipeUpload(name, unit);
            ScriptFunctionRecipe.procRecipeIsExecutingCheck = (name, unit) => CheckEngineRecipe.IsExecuting(name, unit);
            ScriptFunctionRecipe.procRecipeIsExecutingAny = () => CheckEngineRecipe.IsExecuting();

            // ISA-88 Batch 제어 delegate 연결
            ScriptFunctionRecipe.procRecipeStart = (name, batchId, user) => CheckEngineRecipe.BatchStart(name, batchId, user);
            ScriptFunctionRecipe.procRecipeStartUnit = (name, batchId, user, unit) => CheckEngineRecipe.BatchStart(name, batchId, user, unit);
            ScriptFunctionRecipe.procRecipeHold = (name) => CheckEngineRecipe.HoldBatch(name);
            ScriptFunctionRecipe.procRecipeRestart = (name) => CheckEngineRecipe.RestartBatch(name);
            ScriptFunctionRecipe.procRecipeAbort = (name) => CheckEngineRecipe.AbortBatch(name);
            ScriptFunctionRecipe.procRecipeGetState = (name) => CheckEngineRecipe.GetBatchState(name);

            // Preset 스크립트 함수 델리게이트 연결 (JSON 파일 기반)
            ScriptFunctionPreset.procPresetApply = PresetScriptBridge.PresetApply;
            ScriptFunctionPreset.procPresetCapture = PresetScriptBridge.PresetCapture;
            ScriptFunctionPreset.procPresetGetList = PresetScriptBridge.PresetGetList;
            ScriptFunctionPreset.procPresetGetVariantCount = PresetScriptBridge.PresetGetVariantCount;
            ScriptFunctionPreset.procPresetGetVariantName = PresetScriptBridge.PresetGetVariantName;

            // FormConfigPreset Apply 기능 연결 (PlcScan.SetTagValue)
            PublicStudioLocalMain.Recipe.FormConfigPreset.SetTagValueFunc = PlcScan.SetTagValue;

            // Barcode / QR Code 서브시스템 초기화
            LocalMain.Barcode.BarcodeManager.Init();

            // Barcode 스크립트 함수 델리게이트 연결
            ScriptFunctionBarcode.procGetLastScan = LocalMain.Barcode.BarcodeManager.GetLastScanValue;
            ScriptFunctionBarcode.procExtractValue = LocalMain.Barcode.BarcodeManager.ExtractBarcodeValue;
            ScriptFunctionBarcode.procSetScannerEnabled = LocalMain.Barcode.BarcodeManager.SetScannerEnabled;
            ScriptFunctionBarcode.procExportImage = (genName, filePath) =>
            {
                var gen = LocalMain.Barcode.BarcodeManager.GetGenerator(genName);
                return gen?.ExportToFile(filePath) ?? false;
            };

            Script.TagEventScript.Init();

            CheckRealTimeTestData.Init();

            ConfigViewMain.SetOpcServerStarting();

            string tag_file = String.Format("{0}\\TAG\\local.tagx", TotalConfig.sDirWorkProject);
            DialogTag.TagEditor.Editor.checkTagFileChanged.Register(tag_file);

            CheckEngineAutoDeleteThread.Init();

            if (NextVersion.bScript11)
            {
                ScriptLibRun.Debugger.DebuggerHostRun.Init();
            }

            LocalServiceMain.Init();

            // CheckEngineAlwaysScript 생성자에 위치했었는데 시작시 스크립트에 해당함수가 있으면 실행이 안된다. 2015-4-30
            GraphicModule.ScriptFunctionScript.procCommonCallBack = new ScriptFunctionScript.DelegateScriptLocalCallBack(CheckEngineAlwaysScript.ProcScriptCallBack);

            //CheckEngineMinuteChanged.InitThread();

            OPCUAServerMain.LoadConfig();
            await OPCUAServerMain.Init();

            OpcUaIpcManager.Start();

            CheckEnginePythonAi.Initialize();
            PythonAiScriptBridge.RegisterCallbacks();

            // REST API Monitor 초기화
            AutobaseRESTAPIMonitor.GlobalSettings.Initialize();
            AutobaseRESTAPIMonitor.ClassClient.Init();
            AutobaseRESTAPIMonitor.ClassClient.StartAllClients();

            // 데이터베이스 초기화
            bool success = await TrendInitializer.InitializeAsync();
            if (!success)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("자료를 저장할 수 없습니다", "TrendInitializer 초기화 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Cannot save data", "TrendInitializer Initialization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        static void OnTagPropertyChanged(TagPublicClass tp)
        {
            C_DdeTag.DdeTagLinkOne(tp);
            Script.TagEventScript.InitOne(tp);
        }

        //------------------------------------------------------------------------------
        //	프로그램을 끝내기 전 할당된 메모리 등을 풀어준다.
        //------------------------------------------------------------------------------

        public static async Task ViewProgrammEnd(Form form)
        {
            // REST API Monitor 종료
            AutobaseRESTAPIMonitor.ClassClient.StopAllClients();

            CheckEngineAlwaysScript.PclProgramEnd();

            //CheckEngineMinuteChanged.UnInitThread();
            LocalMain.Alarm.AlarmMail.UnInit();

            LocalServiceMain.UnInit();

            if (NextVersion.bScript11)
            {
                ScriptLibRun.Debugger.DebuggerHostRun.UnInit();
            }

            CheckEngineAutoDeleteThread.UnInit();

            SystemInfo.SystemInfoStatus.SystemInfoUninit();

            ConfigViewMain.SetOpcServerStartingWhenLocalMainExit();
            ConfigViewMain.SetOpcServerClosing();

            CheckRealTimeTestData.UnInit();

            await OPCUAServerMain.Uninit(releaseLogger: true); //260126 PSU 추가.
            OpcUaIpcManager.Stop(); //260202 PSU 추가.

            PythonAiScriptBridge.UnregisterCallbacks();
            CheckEnginePythonAi.Shutdown();

            // Barcode 서브시스템 종료
            LocalMain.Barcode.BarcodeManager.Shutdown();

            PlcScan.OpcUnInit();
            CheckEngineTagChangeThread.UnInit();
            CheckEngineNetworkToViewMain.UnInit();
            await C_init.PlayScriptWhenProgrammStartEnd(form, "END");
            CheckEngineTagChange.UnInitSharedTag();

            WatchDogInfo.UnInit();

            //AlarmRecvEventThreadUnInit();
            CheckEngineMilliData.UnInit();  //CheckEngineMilliData.SaveRemainMilliData();
            await CheckEngineMinuteChanged.SaveRemainDataBeforeExit();

            Alarm.AlarmConfirm.SaveAlarmConfirmList();

            SmsManager.UnInit();
            LinePrinter.UnInit();               // line printer를 찾는다.

            CheckEngineOnOffList.SaveOnOffListDigitalStatus();

            C_DdeTag.DdeLibClientUnInit();              // Dde를 태그로 사용하기 위해 준비된 버퍼 풀기

            SystemStatusMemory.UnInit();
            PlcScanEvent.UnInit();
            //AutoBaseDDEUninit();

            CheckEngineAlwaysScript.SaveConfig();
            TagStatus.Save();       // 태그 상태 저장
            ConfigRunMain.Save();
            //SaveUserConfig(config.sUserName);		// 개인 사용자 환경을 저장한다.

            if (TotalConfig.eOemType == EnumOemType.SBAS)
                SmLog.Message(EnumEventID.ProgramEnd, "SBAS 감시 프로그램 종료");
            else
                SmLog.Message(EnumEventID.ProgramEnd, "LocalMain Program End");

            // 병렬로 종료 (최대 5초 대기)
            var tasks = new List<Task>
            {
                Task.Run(() => SmLog.ShutdownLogProcessor(5000)),
                Task.Run(() => AlarmProcessor.Shutdown(5000)),
                Task.Run(() => TrendInitializer.ShutdownAsync())
            };

            // 모든 Task 완료 대기 (최대 5초)
            await Task.WhenAll(tasks).ConfigureAwait(false);

            // 프로그램 종료 시 정리
            DatabaseConfigManager.ShutdownDatabase();
            DatabaseHealthMonitor.Shutdown();
            Debug.WriteLine("ViewProgrammEnd 완료");
        }

        public static async Task PlayScriptWhenProgrammStartEnd(Form form, string when)
        {
            ScriptClass control = new ScriptClass();

            string fullpath = TotalConfig.FileOldNew("control\\StartEnd", when + ".CTL", when + ".CTLX");

            if (!File.Exists(fullpath)) return;

            control.LoadFromFile(fullpath);
            await control.RunAsync(form, null);

            if (control.IsError())
            {
                string message;
                message = control.GetError();
                MessageDisplay.Show(fullpath + "\n" + message);
            }

        }
    }
}

/*
//------------------------------------------------------------------
// 데이터를 저장할 폴더를 준비한다.
//------------------------------------------------------------------

void PrepareDataDirectory(HWND hwnd)
{
	CString message;

	ServerMainSetDataDirectory(config.sDirData, config.sDirDataLog, &config.duplexDir);

	if(access(config.sDirData, 0) != 0) {	// 데이터 저장 폴더를 만든다.
		if(MakeDirectory(config.sDirData) == 0) {
#if	defined (COMPILE_HANGUL)
			message.Format("데이터 폴더 {0} 만들기 실패", config.sDirData);
#else
			message.Format("Failed make data directroy({0})", config.sDirData);
#endif
			SmLogMessage(message);
		}
		else {
#if	defined (COMPILE_HANGUL)
			message.Format("데이터 폴더 {0} 만듦", config.sDirData);
#else
			message.Format("Make data directory.({0})", config.sDirData);
#endif
			SmLogMessage(message);
		}
	}

	MakeDirectory(config.sDirDataLog);

	// 데이터를 저장할 폴더의 디스크 용량이 남아 있는가를 검사한다.

	int drive;

	if(config.sDirData[0] >= 'a' && config.sDirData[0] <= 'z')
		drive = config.sDirData[0]-'a'+1;
	else
		drive = config.sDirData[0]-'A'+1;

	hyper size = GetDiskFreeSize(drive);

	if(size < 1000000L) {
#if	defined (COMPILE_HANGUL)
		message.Format("%c: 드라이브 남은 공간이 1M byte 이하.({0})\n자료저장이 안될 수 있음.", drive-1+'A', GetDiskFreeSize(drive));
		MessageBox(hwnd, message, "디스크 공간 부족", MB_OK);
#else
		message.Format("Low free disk space %c: 1M byte.({0})", drive-1+'A', GetDiskFreeSize(drive));
		MessageBox(hwnd, message, "Low disk space", MB_OK);
#endif
		SmLogMessage(message);
	}

	//------------------------------------------------------------------------------
	// TREND 폴더를 준비한다. 만들어 있지 않으면 만든다.
	//------------------------------------------------------------------------------

	char dir[MAXPATH];

	wsprintf(dir, "{0}\\TREND", config.sDirData);
	if(access(dir, 0) != 0) {
		if(MakeDirectory(dir) == 0)
			SmLogMessage("TREND directory make failed.");
		else
			SmLogMessage("TREND directory make.");
	}
}
*/
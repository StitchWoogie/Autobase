using System;
using System.Drawing;
using NetTools;

namespace AutoLibLocal
{
    /// <summary>
    /// ViewMain에서만 사용하는 환경 설정 파일 들
    /// </summary>
    [Serializable]
    public class ConfigViewMain
    {
        public static bool bFitToWindow = true;
        public static Font fontMain = new Font("Gulim", 10);
        public static bool bDisplayToolTip = true;
        public static bool bResponseMouseLeftOnGraphic = true;
        public static bool bResponseMouseRightOnGraphic = true;
        public static bool bResponseMouseRightGraphicContextMenu = true;
        public static bool bResponseMouseRightOnObject = true;
        public static bool bUseMenuButtonOnGraphic = true;
        public static int nMdiCountOnGraphic = 5;
        public static int nMdiCountOnBasicScreen = 2;
        public static int nMdiCountOnReport = 5;
        public static bool bShowMainTitle = true;
        public static bool bShowMainMenu = true;
        public static int nForLoopTimeout = 5;
        public static bool bAllowManualOutputOnClient = true;   // 웹 클라이언트에서 수동 출력만 서버로 태그값을 전송한다.
        public static string PrinterOnScriptPrintModule;
        public static bool bDisplayLogInBoxOnStart = false;

        public static bool bUseNotifyIcon = false;
        public static bool bUseTestModeMessage = false;

        public static bool bNaviSaveFlag;
        public static int nNaviSaveX;
        public static int nNaviSaveY;
        public static int nNaviSaveWidth;
        public static int nNaviSaveHeight;

        public static bool bBasciScreenDataViewWhiteBackground;

        public static bool bUseDeviceQuality;

        public static bool bUserControlBoxUseAnalogModule;
        public static bool bUserControlBoxUseDigitalModule;
        public static bool bUserControlBoxUseStringModule;
        public static string sUserControlBoxAnalogModule;
        public static string sUserControlBoxDigitalModule;
        public static string sUserControlBoxStringModule;

        public static int nTimeoutOfExcelReportRunDirect = 60;   // 웹 클라이언트 엑셀 리포트용 시간초과 30초 이상 걸리는 경우도 있다.

        public static bool bStartOpcServerWhileLocalMainRunning = false;
        public static bool bCloseOpcServerWhenLocalMainExit = false;

        public static bool bOnOffListAddOneSecondToOperationTime;   // OnOffList가동시간 계산시 1을 더한다.

        public static bool bDisplayMouseZoneWhenSameTagSelected;
        public static bool bUseProtectMenu; // 그래픽 context menu에 나오는 protect menu

        // 월보 출력 시 하루의 시작을 0으로 하지 않고 시간을 정할 수 있도록 한다.
        public static int nReportStartHourOfDay;        // 하루의 시작 시간
        public static bool bReportStartHourOfDayMaxSub; // 최대값 차이를 하루의 시작 시간을 사용

        public static bool bUseAlarmServer = true;     // 경보 서버 사용 - DataGateServer 를 사용할 것인가를 체크한다. 11에서는 이 옵션을 삭제하고 항상 ON할 수 있도록 한다.
                                                       //250827 PSU 기본값 true로 변경.

        public static bool bRestoreLocationSizeOnStartup = false;   // 프로그램 시작시 생성 위치 저장

        public static bool bRemoveFlashingWhenActivatingMdiModule = false;  // 그래픽 모듈 창 전환 시 깜박임 제거

        public static bool bUseAutomaticFileRecovery;    // Mirror 폴더에 같은 파일을 만들어 놓고 CRC와 비교해서 맞는 파일을 사용한다.
                                                         // 현재는 태그만 되어 있는데 잘되면 이 변수는 필요없고 항상 ON해도 된다.

        public static bool bScriptErrorMessageShow;         // 스크립트 오류메시지를 보여줌. 2023-8-30 베트남 현장때문에 지원. 통신포트가 비활성화되면 태그가 없는경우가 있어서 어쩔수 없이 지원.
        public static bool bScriptErrorMessageTagNotFound;  // 태그 없음 오류 메시지
        public static bool bScriptErrorMessageElse;         // 기타 오류 메시지

        public static bool bEnableSettingValuePreview;      // ViewMain에서 값을 설정할 때 미리 값을 적용하는 기능.  2023-11-21 값이 미리 적용되는 것을 선택할 수 있게 함.

        public static bool bShowUserManualControl; //수동제어 사용자 보기 옵션 24-01-24

        public static bool bAlwaysOpenNewReport; // 무조건 리포트 모듈을 새로 열기 24-07-01 hsjeong

        public static bool bAlarmConfirmSorting;		// 0 = 오름 차순, 1 = 오름차순. 20241111 PSU

        public static bool bUseEventTimerBatching; // 20251013 PSU 추가
        public static int nEventTimerBatchSize;  //20251013 PSU 추가

        static ConfigViewMain()
        {
            fontMain = MakeDefaultFont();

            ConfigViewMain.Load();
        }

        public static Font MakeDefaultFont()
        {
            Font font;

            if (NetTools.Tools.IsLangJapanese())
                font = new Font("MS UI Gothic", 10);
            else if (NetTools.Tools.IsLangChinese())
                font = new Font("SimSun", 10);
            else if (NetTools.Tools.IsLangKorean())
            {
                // 6 = Vista
                if (Environment.OSVersion.Version.Major >= 6)
                    font = new Font("Malgun Gothic", 10);
                else
                    font = new Font("Gulim", 10);
            }
            else
                font = new Font("Tahoma", 10);

            return font;
        }

        public static void Load()
        {
            bDisplayToolTip = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bDisplayTagInfoOnMouseMove", true);
            bFitToWindow = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bDisplayFitWindowSize", false);
            bResponseMouseLeftOnGraphic = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bLMouseResponseOnGraphic", true);
            bResponseMouseRightOnGraphic = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bRMouseResponseOnGraphic", true);
            bResponseMouseRightGraphicContextMenu = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bResponseMouseRightGraphicContextMenu", true);
            bResponseMouseRightOnObject = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bResponseMouseRightOnObject", true);
            bUseMenuButtonOnGraphic = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bMenuButtonOnGraphic", true);
            nMdiCountOnGraphic = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMaxMdiScreenGraphic", 5);
            nMdiCountOnBasicScreen = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMdiCountOnBasicScreen", 2);
            nMdiCountOnReport = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMaxMdiScreenReport", 5);

            bShowMainTitle = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bShowMainTitle", true);
            bShowMainMenu = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bShowMainMenu", true);

            nForLoopTimeout = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nForLoopTimeout", 5);
            bAllowManualOutputOnClient = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bAllowManualOutputOnClient", true);

            PrinterOnScriptPrintModule = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "PrinterOnScriptPrintModule", "");

            fontMain = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "Font", fontMain);


            /* 폰트 환경을 읽어오는 라이브러리를 만들었다. 
            string font_string = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "Font", "");
            if (font_string.Length != 0)
            {
                CommaBlockString comma = new CommaBlockString();
                comma.Set(font_string);

                int imsi = 0;
                string fname = "arial";
                float fheight = 10;
                FontStyle fstyle = 0;

                comma.GetString(ref fname);
                comma.GetFloat(ref fheight);

                comma.GetInt(ref imsi);
                if (imsi == 1) fstyle |= FontStyle.Bold;
                comma.GetInt(ref imsi);
                if (imsi == 1) fstyle |= FontStyle.Italic;
                comma.GetInt(ref imsi);
                if (imsi == 1) fstyle |= FontStyle.Strikeout;
                comma.GetInt(ref imsi);
                if (imsi == 1) fstyle |= FontStyle.Underline;

                fontMain = new Font(fname, fheight, fstyle);
            }*/

            bNaviSaveFlag = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "bNaviSaveFlag", false);
            nNaviSaveX = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveX", 0);
            nNaviSaveY = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveY", 0);
            nNaviSaveWidth = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveWidth", 100);
            nNaviSaveHeight = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveHeight", 100);

            bDisplayLogInBoxOnStart = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\LogIn", "bDisplayLogInBoxOnStart", false);

            bUseNotifyIcon = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bUseNotifyIcon", false);
            bUseTestModeMessage = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bUseTestModeMessage", true);

            bBasciScreenDataViewWhiteBackground = TotalConfig.LoadRegAutoBaseConfig("RunMain", "BasicScreen", "bBasciScreenDataViewWhiteBackground", false);

            bUseDeviceQuality = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Device", "bUseDeviceQuality", false);

            bUserControlBoxUseAnalogModule = TotalConfigProject.LoadConfig("RunMain", "ControlBox", "bUserControlBoxUseAnalogModule", false);
            bUserControlBoxUseDigitalModule = TotalConfigProject.LoadConfig("RunMain", "ControlBox", "bUserControlBoxUseDigitalModule", false);
            bUserControlBoxUseStringModule = TotalConfigProject.LoadConfig("RunMain", "ControlBox", "bUserControlBoxUseStringModule", false);
            sUserControlBoxAnalogModule = TotalConfigProject.LoadConfig("RunMain", "ControlBox", "sUserControlBoxAnalogModule", "");
            sUserControlBoxDigitalModule = TotalConfigProject.LoadConfig("RunMain", "ControlBox", "sUserControlBoxDigitalModule", "");
            sUserControlBoxStringModule = TotalConfigProject.LoadConfig("RunMain", "ControlBox", "sUserControlBoxStringModule", "");

            nTimeoutOfExcelReportRunDirect = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\WebService", "nTimeoutOfExcelReportRunDirect", 60);

            bStartOpcServerWhileLocalMainRunning = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "bStartOpcServerWhileLocalMainRunning", false);
            bCloseOpcServerWhenLocalMainExit = TotalConfig.LoadRegAutoBaseConfig("Config", "Closing", "bCloseOpcServerWhenLocalMainExit", false);

            bOnOffListAddOneSecondToOperationTime = TotalConfig.LoadRegAutoBaseConfig("Config", "Closing", "bOnOffListAddOneSecondToOperationTime", true);

            bDisplayMouseZoneWhenSameTagSelected = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Graphic", "bDisplayMouseZoneWhenSameTagSelected", false);

            bUseProtectMenu = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Graphic", "bUseProtectMenu", false);

            nReportStartHourOfDay = TotalConfig.LoadRegAutoBaseConfig("Report", "Config", "nReportStartHourOfDay", 0);                  // 하루의 시작 시간
            bReportStartHourOfDayMaxSub = TotalConfig.LoadRegAutoBaseConfig("Report", "Config", "bReportStartHourOfDayMaxSub", false);  // 최대값 차이를 하루의 시작 시간을 사용

            bUseAlarmServer = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Config", "bUseAlarmServer", false);

            bRestoreLocationSizeOnStartup = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Config", "bRestoreLocationSizeOnStartup", false);

            bRemoveFlashingWhenActivatingMdiModule = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Graphic", "bRemoveFlashingWhenActivatingMdiModule", false);

            bUseAutomaticFileRecovery = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bUseAutomaticFileRecovery", false);

            bScriptErrorMessageShow = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bScriptErrorMessageShow", true);
            bScriptErrorMessageTagNotFound = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bScriptErrorMessageTagNotFound", true);
            bScriptErrorMessageElse = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bScriptErrorMessageElse", true);

            bAlwaysOpenNewReport = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Graphic", "bAlwaysOpenNewReport", false); //무조건 리포트 모듈을 새로 열기 24-07-01 hsjeong

            bEnableSettingValuePreview = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bEnableSettingValuePreview", true);

            bShowUserManualControl = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "AlarmFile", "bShowUserManualControl", false);

            bAlarmConfirmSorting = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bAlarmConfirmSorting", false); //20241111 PSU 경보 정렬

            bUseEventTimerBatching = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bUseEventTimerBatching", false); //20251013 PSU 추가
            nEventTimerBatchSize = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nEventTimerBatchSize", 20); //20251013 PSU 추가
        }

        // 감시 프로그램 시작할 때는 OpcServer는 요청시 항상 실행할 수 있도록 한다.
        public static void SetOpcServerStarting()
        {
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServer", 1);
        }

        // 감시 프로그램 종료 시 감시프로그램이 종료한 표시를 한다.
        public static void SetOpcServerStartingWhenLocalMainExit()
        {
            if(bStartOpcServerWhileLocalMainRunning) {
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServer", 0);
            }
        }

        // 감시 프로그램 종료 시 OPC서버도 조건이 되면 종료한다.
        public static void SetOpcServerClosing()
        {
            if (bCloseOpcServerWhenLocalMainExit)
            {
                TotalConfig.SaveRegAutoBaseConfig("Config", "Closing", "OpcServer", 1);
            }
        }

        public static void Save()
        {
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bDisplayTagInfoOnMouseMove", bDisplayToolTip);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bDisplayFitWindowSize", bFitToWindow);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bLMouseResponseOnGraphic", bResponseMouseLeftOnGraphic);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bRMouseResponseOnGraphic", bResponseMouseRightOnGraphic);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bResponseMouseRightGraphicContextMenu", bResponseMouseRightGraphicContextMenu);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bResponseMouseRightOnObject", bResponseMouseRightOnObject);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bMenuButtonOnGraphic", bUseMenuButtonOnGraphic);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nMaxMdiScreenGraphic", nMdiCountOnGraphic);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nMdiCountOnBasicScreen", nMdiCountOnBasicScreen);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nMaxMdiScreenReport", nMdiCountOnReport);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bShowMainTitle", bShowMainTitle);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bShowMainMenu", bShowMainMenu);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nForLoopTimeout", nForLoopTimeout);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bAllowManualOutputOnClient", bAllowManualOutputOnClient);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "PrinterOnScriptPrintModule", PrinterOnScriptPrintModule);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "Font", fontMain);
            /* 폰트를 저장하는 라이브러리를 만들었다.
            string font_string = String.Format("{0},", fontMain.Name);
            font_string += String.Format("{0},", fontMain.Size);
            font_string += String.Format("{0},", (fontMain.Style & FontStyle.Bold) > 0 ? 1 : 0);
            font_string += String.Format("{0},", (fontMain.Style & FontStyle.Italic) > 0 ? 1 : 0);
            font_string += String.Format("{0},", (fontMain.Style & FontStyle.Strikeout) > 0 ? 1 : 0);
            font_string += String.Format("{0},", (fontMain.Style & FontStyle.Underline) > 0 ? 1 : 0);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "Font", font_string);*/

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "bNaviSaveFlag", bNaviSaveFlag);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveX", nNaviSaveX);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveY", nNaviSaveY);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveWidth", nNaviSaveWidth);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveHeight", nNaviSaveHeight);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\LogIn", "bDisplayLogInBoxOnStart", bDisplayLogInBoxOnStart);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bUseNotifyIcon", bUseNotifyIcon);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bUseTestModeMessage", bUseTestModeMessage);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "BasicScreen", "bBasciScreenDataViewWhiteBackground", bBasciScreenDataViewWhiteBackground);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Device", "bUseDeviceQuality", bUseDeviceQuality);

            TotalConfigProject.SaveConfig("RunMain", "ControlBox", "bUserControlBoxUseAnalogModule", bUserControlBoxUseAnalogModule);
            TotalConfigProject.SaveConfig("RunMain", "ControlBox", "bUserControlBoxUseDigitalModule", bUserControlBoxUseDigitalModule);
            TotalConfigProject.SaveConfig("RunMain", "ControlBox", "bUserControlBoxUseStringModule", bUserControlBoxUseStringModule);
            TotalConfigProject.SaveConfig("RunMain", "ControlBox", "sUserControlBoxAnalogModule", sUserControlBoxAnalogModule);
            TotalConfigProject.SaveConfig("RunMain", "ControlBox", "sUserControlBoxDigitalModule", sUserControlBoxDigitalModule);
            TotalConfigProject.SaveConfig("RunMain", "ControlBox", "sUserControlBoxStringModule", sUserControlBoxStringModule);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\WebService", "nTimeoutOfExcelReportRunDirect", nTimeoutOfExcelReportRunDirect);

            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "bStartOpcServerWhileLocalMainRunning", bStartOpcServerWhileLocalMainRunning);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Closing", "bCloseOpcServerWhenLocalMainExit", bCloseOpcServerWhenLocalMainExit);

            TotalConfig.SaveRegAutoBaseConfig("Config", "Closing", "bOnOffListAddOneSecondToOperationTime", bOnOffListAddOneSecondToOperationTime);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Graphic", "bDisplayMouseZoneWhenSameTagSelected", bDisplayMouseZoneWhenSameTagSelected);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Graphic", "bUseProtectMenu", bUseProtectMenu);

            TotalConfig.SaveRegAutoBaseConfig("Report", "Config", "nReportStartHourOfDay", nReportStartHourOfDay);                  // 하루의 시작 시간
            TotalConfig.SaveRegAutoBaseConfig("Report", "Config", "bReportStartHourOfDayMaxSub", bReportStartHourOfDayMaxSub);  // 최대값 차이를 하루의 시작 시간을 사용

            TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Config", "bUseAlarmServer", bUseAlarmServer);

            TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Config", "bRestoreLocationSizeOnStartup", bRestoreLocationSizeOnStartup);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Graphic", "bRemoveFlashingWhenActivatingMdiModule", bRemoveFlashingWhenActivatingMdiModule);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Graphic", "bAlwaysOpenNewReport", bAlwaysOpenNewReport); // 무조건 리포트 모듈을 새로 열기 hsjeong 24-07-01

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bUseAutomaticFileRecovery", bUseAutomaticFileRecovery);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bScriptErrorMessageShow", bScriptErrorMessageShow);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bScriptErrorMessageTagNotFound", bScriptErrorMessageTagNotFound);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bScriptErrorMessageElse", bScriptErrorMessageElse);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bEnableSettingValuePreview", bEnableSettingValuePreview);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "AlarmFile", "bShowUserManualControl", bShowUserManualControl);

            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bAlarmConfirmSorting", bAlarmConfirmSorting); //20241111 PSU 경보정렬

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bUseEventTimerBatching", bEnableSettingValuePreview); //20251013 PSU 추가
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nEventTimerBatchSize", nEventTimerBatchSize); //20251013 PSU 추가
        }
    }
}

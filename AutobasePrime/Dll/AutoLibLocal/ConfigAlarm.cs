using System;
using System.Drawing;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for ConfigAlarm.
	/// </summary>
	public class ConfigAlarm 
	{
		public static bool	bAlarmProtectAll;			// 모든 경보를 중지/시작.
		public static bool 	bAlarmSoundFlag;			// 소리를 울릴것이냐? 
		public static int 	cAlarmSoundType;			// 경보를 울릴 것이냐?
		public static string 	sAlarmWaveFile;			// 사용할 경보 음성 파일
		public static bool	bAlarmScreenFlag;			// 경보가 발생하면 화면을 띄울것이냐.
		public static int	nAlarmScreenTime;		// alarm screen 표시 시간, 0 - 계속 보여준다.
		public static bool	bAlarmAutoMakeConfirmBox;	// 자동으로 경보 확인 상자를 생성시킨다.
		public static uint	dwAlarmFilterEvent;			// Event 필터
		public static uint	dwAlarmFilterLinePrinter;	// Line Printer 필터
		public static uint	dwAlarmFilterSound;			// Sound 필터
		public static uint	dwAlarmFilterFile;			// File 필터
		public static uint	dwAlarmFilterSmsManager;	// SMS mamager 필터
        public static uint  dwAlarmFilterMail;	        // Mail 필터
		public static bool	bAlarmConfirmSorting;		// 0 = 오름 차순, 1 = 오름차순.
		public static bool	bDisplayGraphicFileOnAlarm;	// 경보가 발생했을때 그래픽 파일을 보여줄것인가를 체크?
		public static int	nWaitAlarmAfterHandOper;	// 수동 조작 후 조작이 제대로 되지않았을 때 경보여부 0 = 사용안함, 그외=조작실패라고 판단하는 시간제한.
		public static string sAlarmDigitalOut;			// 경보발생 시 디지털 태그로 출력한다.

        public static bool bAlarmAlsoSaveAsCsvFormat;   // CSV포맷으로도 저장한다.
        public static bool bCsvSaveMillisecond;
        public static bool bCsvSaveTag;
        public static bool bCsvSaveDescription;
        public static bool bCsvSaveMsg;
        public static bool bCsvSaveAlarmType;
        public static bool bCsvSavePriority;
        public static bool bCsvSavePort;
        public static bool bCsvSaveStation;
        public static bool bCsvSaveAddress;
        public static bool bCsvSaveAlarmSubType;    

        public static bool bCsvSpecifyTargetFolder;
        public static string sCsvTargetFolder;

        public static bool bEnableContextMenuOnAlarmEvent;

        public static Font fontAlarmEvent = ConfigViewMain.MakeDefaultFont();

        public static bool bMailActive;
        public static string sMailServer;
        public static string sMailUsername;
        public static string sMailPassword;
        public static string sMailTo;
        public static string sMailFrom;
        public static bool bMailSSL;
        public static int nMailSendingInverval;
        public static int nMailPort; // port인자 추가 20240509 PSU

        public static bool bSaveRemoteControlResult;    // 원격 수동제어의 결과를 경보파일로 저장한다.
        

		static ConfigAlarm()
		{
			LoadConfig();
		}

		static void LoadConfig()
		{
			bAlarmProtectAll = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bAlarmProtectAll", false);
			bAlarmSoundFlag = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bAlarmSoundFlag", true);
			cAlarmSoundType = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "cAlarmSoundType", 0);
			sAlarmWaveFile = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "sAlarmWaveFile", "");
			bAlarmScreenFlag = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bAlarmScreenFlag", true);
			nAlarmScreenTime = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "nAlarmScreenTime", 5);
			bAlarmAutoMakeConfirmBox = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bAlarmAutoMakeConfirmBox", true);
			dwAlarmFilterEvent = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterEvent", 0xFFFFFFFF);
			dwAlarmFilterLinePrinter = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterLinePrinter", 0xFFFFFFFF);
			dwAlarmFilterSound = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterSound", 0xFFFFFFFF);
			dwAlarmFilterFile = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterFile", 0xFFFFFFFF);
			dwAlarmFilterSmsManager = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterSmsManager", 0xFFFFFFFF);
            dwAlarmFilterMail = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterMail", 0xFFFFFFFF);

			bAlarmConfirmSorting = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bAlarmConfirmSorting", false);
			bDisplayGraphicFileOnAlarm = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bDisplayGraphicFileOnAlarm", true);
			nWaitAlarmAfterHandOper = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "nWaitAlarmAfterHandOper", 0);
			sAlarmDigitalOut = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "sAlarmDigitalOut", "");
            bEnableContextMenuOnAlarmEvent = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bEnableContextMenuOnAlarmEvent", true);

            bAlarmAlsoSaveAsCsvFormat = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "bAlarmAlsoSaveAsCsvFormat", false);

            fontAlarmEvent = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", null, "fontAlarmEvent", fontAlarmEvent);

            bCsvSaveMillisecond = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveMillisecond", false);
            bCsvSaveTag = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveTag", true);
            bCsvSaveDescription = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveDescription", true);
            bCsvSaveMsg = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveMsg", true);
            bCsvSaveAlarmType = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveAlarmType", true);
            bCsvSavePriority = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSavePriority", true);
            bCsvSavePort = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSavePort", true);
            bCsvSaveStation = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveStation", false);
            bCsvSaveAddress = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveAddress", false);
            bCsvSaveAlarmSubType = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveAlarmSubType", false);
            bCsvSpecifyTargetFolder = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSpecifyTargetFolder", false);
            sCsvTargetFolder = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "CsvItem", "sCsvTargetFolder", TotalConfig.GetProjectDataDirectory() + "\\ALARM.CSV");

            bMailActive = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "bMailActive", false);
            sMailServer = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailServer", "");
            sMailUsername = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailUsername", "");
            sMailPassword = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailPassword", "");
            sMailFrom = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailFrom", "");
            sMailTo = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailTo", "");
            bMailSSL = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "bMailSSL", false);
            nMailSendingInverval = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "nMailSendingInverval", 10);
            nMailPort = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "Mail", "nMailPort", 25); 

            MessageDisplay.nScreenLifeTime = ConfigAlarm.nAlarmScreenTime;  // 경보 메시지가 떠 있는 시간을 설정한다.

            bSaveRemoteControlResult = TotalConfig.LoadRegAutoBaseConfig("ConfigAlarm", "AlarmFile", "bSaveRemoteControlResult", false);
           
		}

		public static void SaveConfig()
		{
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bAlarmProtectAll", bAlarmProtectAll);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bAlarmSoundFlag", bAlarmSoundFlag);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "cAlarmSoundType", cAlarmSoundType);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "sAlarmWaveFile", sAlarmWaveFile);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bAlarmScreenFlag", bAlarmScreenFlag);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "nAlarmScreenTime", nAlarmScreenTime);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bAlarmAutoMakeConfirmBox", bAlarmAutoMakeConfirmBox);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterEvent", dwAlarmFilterEvent);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterLinePrinter", dwAlarmFilterLinePrinter);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterSound", dwAlarmFilterSound);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterFile", dwAlarmFilterFile);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterSmsManager", dwAlarmFilterSmsManager);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "dwAlarmFilterMail", dwAlarmFilterMail);

			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bAlarmConfirmSorting", bAlarmConfirmSorting);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bDisplayGraphicFileOnAlarm", bDisplayGraphicFileOnAlarm);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "nWaitAlarmAfterHandOper", nWaitAlarmAfterHandOper);
			TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "sAlarmDigitalOut", sAlarmDigitalOut);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bEnableContextMenuOnAlarmEvent", bEnableContextMenuOnAlarmEvent);

            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "bAlarmAlsoSaveAsCsvFormat", bAlarmAlsoSaveAsCsvFormat);

            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", null, "fontAlarmEvent", fontAlarmEvent);

            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveMillisecond", bCsvSaveMillisecond);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveTag", bCsvSaveTag);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveDescription", bCsvSaveDescription);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveMsg", bCsvSaveMsg);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveAlarmType", bCsvSaveAlarmType);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSavePriority", bCsvSavePriority);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSavePort", bCsvSavePort);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveStation", bCsvSaveStation);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveAddress", bCsvSaveAddress);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSaveAlarmSubType", bCsvSaveAlarmSubType);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "bCsvSpecifyTargetFolder", bCsvSpecifyTargetFolder);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "CsvItem", "sCsvTargetFolder", sCsvTargetFolder);

            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "bMailActive", bMailActive);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailServer", sMailServer);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailUsername", sMailUsername);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailPassword", sMailPassword);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailFrom", sMailFrom);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "sMailTo", sMailTo);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "bMailSSL", bMailSSL);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "nMailSendingInverval", nMailSendingInverval);
            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "Mail", "nMailPort", nMailPort);

            TotalConfig.SaveRegAutoBaseConfig("ConfigAlarm", "AlarmFile", "bSaveRemoteControlResult", bSaveRemoteControlResult);
            
		}
	}
}

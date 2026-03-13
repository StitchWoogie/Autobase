using System;
using AutoLibLocal;
using NetTools.OldDefine;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// Summary description for ConfigRunMain.
	/// </summary>
	public class ConfigRunMain
	{
		static ConfigRunMain()
		{
			//
			// TODO: Add constructor logic here
			//
			Load();
		}

		public static bool bAiCalcSameFormat;
		public static bool bMatchAlarmReturnGabAndHiHiLoLoDO;
		public static int  nScanStartTime;
		public static bool bRunScriptIfError;			// 스크립트에 오류가 발생해도 계속 진행
		public static bool bWriteDiSubTagWhenStart;		// 프로그램 시작시 DI Sub Tag 출력.
		public static bool bWriteAiSubTagWhenStart;		// 프로그램 시작시 AI Sub Tag 출력. 
		//public static int  nMilliDataBackColor;		
		public static RECT rAlarmConfirmBox = new RECT();
		public static string sUserName;

		//public static bool bDisplayTagInfoOnMouseMove;
		//public static bool bDisplayFitWindowSize;
		//public static bool bLMouseResponseOnGraphic;
		//public static bool bRMouseResponseOnGraphic;
		//public static bool bMenuButtonOnGraphic;
		//public static int nMaxMdiScreenGraphic;
		public static bool bEndWithPlcScan;
		public static bool bEndPrompt;
		public static bool bEndWithOpcUaClient; //260225 PSU 추가

		public static bool bFlowView;
		public static int  nFlowViewSec = 10;

		public static string sAlarmMsgNEW;
		public static string sAlarmMsgCNF;
		public static string sAlarmMsgRET;

        // 출력 결과 미리보기 기능
        public static bool bPreviewOutputResult;
        public static int nPreviewOutputResultSeconds;    // 출력을 하면 미리 가상 출력해서 보여주고 지정 시간까지 값이 들어오지 않으면 원래대로 표시한다.

        public static bool bShareTagValueBySharedMemory;    // 공유 메모리를 통한 태그값 공유. 항상 true로 사용해도 되지만 버그가 생길 수 있으므로 옵션으로 사용한다.
                                                            // 문제가 없으면 항상 ON으로 사용한다. 즉 이 조건 없이 항상 출력하도록한다.
                                                            // 2016-11-6

        

        // 260226 PSU, 메인타이머 성능 프로파일링. 설정 시간(ms) 이상 지연 시 로그 기록. 0이면 비활성.
		public static bool bMainTimerProfile;
        public static int nMainTimerProfileThresholdMs;

        public static ClassFontsAndColors fac = new ClassFontsAndColors();
        public static FontsAndColorsItemFont facScheduleFont = new FontsAndColorsItemFont();
        
		static void Load()
		{
			bAiCalcSameFormat = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bAiCalcSameFormat", true);
			bMatchAlarmReturnGabAndHiHiLoLoDO = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bMatchAlarmReturnGabAndHiHiLoLoDO", false);
			nScanStartTime = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nScanStartTime", 5);
			bRunScriptIfError = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bRunScriptIfError", false);

			bWriteDiSubTagWhenStart = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bWriteDiSubTagWhenStart", true);
			bWriteAiSubTagWhenStart = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bWriteAiSubTagWhenStart", true);

			//nMilliDataBackColor = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMilliDataBackColor", 0);

			rAlarmConfirmBox.left = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\rAlarmConfirmBox", "left", 0);
			rAlarmConfirmBox.top = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\rAlarmConfirmBox", "top", 0);
			rAlarmConfirmBox.right = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\rAlarmConfirmBox", "right", 500);
			rAlarmConfirmBox.bottom = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\rAlarmConfirmBox", "bottom", 300);

			sUserName = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "sUserName", "admin");
			
			bEndWithPlcScan = TotalConfig.LoadRegOemConfig("RunMain", "Config", "bEndWithPlcScan", true);  // 2015-1-6 부터 기본값을 true로 변경
			bEndPrompt = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bEndPrompt", true);
            bEndWithOpcUaClient = TotalConfig.LoadRegOemConfig("RunMain", "Config", "bEndWithOPCUAClient", true);  // 20260225 PSU 추가

            bFlowView = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bFlowView", false);
			nFlowViewSec = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nFlowViewSec", 10);

			sAlarmMsgNEW = TotalConfig.LoadRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgNEW", "NEW");
			sAlarmMsgCNF = TotalConfig.LoadRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgCNF", "CNF");
			sAlarmMsgRET = TotalConfig.LoadRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgRET", "RET");

            bPreviewOutputResult = TotalConfig.LoadRegAutoBaseConfig("RunMain", "PreviewOutputResult", "bPreviewOutputResult", false);
            nPreviewOutputResultSeconds = TotalConfig.LoadRegAutoBaseConfig("RunMain", "PreviewOutputResult", "nPreviewOutputResultSeconds", 2000);

            bShareTagValueBySharedMemory = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bShareTagValueBySharedMemory", false);

			bMainTimerProfile = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bMainTimerProfile", false); // 260226 PSU
            nMainTimerProfileThresholdMs = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMainTimerProfileThresholdMs", 100); // 260226 PSU
			if(nMainTimerProfileThresholdMs < 20)
			{
				nMainTimerProfileThresholdMs = 20;
            }

            if (Tools.IsLangKorean())
            {
                fac.Add(facScheduleFont, "스케쥴", "달력 글꼴");
            }
            else
            {
                fac.Add(facScheduleFont, "Schedule", "Calendar Font");
            }
		}

		public static void Save()
		{
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bAiCalcSameFormat", bAiCalcSameFormat);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bMatchAlarmReturnGabAndHiHiLoLoDO", bMatchAlarmReturnGabAndHiHiLoLoDO);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nScanStartTime", nScanStartTime);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bRunScriptIfError", bRunScriptIfError);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bWriteDiSubTagWhenStart", bWriteDiSubTagWhenStart);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bWriteAiSubTagWhenStart", bWriteAiSubTagWhenStart);

			//TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nMilliDataBackColor", nMilliDataBackColor);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\rAlarmConfirmBox", "left", rAlarmConfirmBox.left);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\rAlarmConfirmBox", "top", rAlarmConfirmBox.top);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\rAlarmConfirmBox", "right", rAlarmConfirmBox.right);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\rAlarmConfirmBox", "bottom", rAlarmConfirmBox.bottom);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "sUserName", sUserName);
			
			TotalConfig.SaveRegOemConfig("RunMain", "Config", "bEndWithPlcScan", bEndWithPlcScan);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bEndPrompt", bEndPrompt);
            TotalConfig.SaveRegOemConfig("RunMain", "Config", "bEndWithPlcScan", bEndWithOpcUaClient);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bFlowView", bFlowView);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nFlowViewSec", nFlowViewSec);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "PreviewOutputResult", "bPreviewOutputResult", bPreviewOutputResult);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "PreviewOutputResult", "nPreviewOutputResultSeconds", nPreviewOutputResultSeconds);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bShareTagValueBySharedMemory", bShareTagValueBySharedMemory);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bMainTimerProfile", bMainTimerProfile); // 260226 PSU
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nMainTimerProfileThresholdMs", nMainTimerProfileThresholdMs); // 260226 PSU

			/* 이부분은 LocalConfig에서만 저장하자.
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgNEW", sAlarmMsgNEW);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgCNF", sAlarmMsgCNF);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgRET", sAlarmMsgRET);
			*/
			
		}
	}
}

using System;
using AutoLibLocal;
using System.IO;
using NetTools;
using NetCommon;

namespace NetClient
{
	/// <summary>
	/// Summary description for configStruct.
	/// </summary>
	public class configStruct
	{
		public configStruct()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string sNodeName = "Input Node Name";
		public static int  nNodeType = 0;
		//public static int  nServerPort = 7000;  공통환경으로 이동
		public static int  nTcpPort = 7001;

		public static sbyte bChangeConditionPlcScanTimeOut = 1;	// PLC_SCAN의 어느한 포트라도 통신시간 초과이면 절체
		public static sbyte bChangeConditionProgramViewMain = 0;
		public static sbyte bChangeConditionProgramPlcScan = 0;
		public static sbyte bChangeConditionProgramRunMain = 0;
		public static sbyte bChangeConditionProtectChangeTime = 0;	// 15~45초 사이에 절체한다.

		public static sbyte bLatchLinePrinter;		// 라인 경보 프린터 절환여부.
		public static sbyte bLatchReportPrinter;	// 보고서 프린터 절환여부.

		public static sbyte bTimeSyncActive;
		public static int  nTimeSyncHour;
		public static int  nTimeSyncMin;
		public static int  nTimeSyncWhen;

		public static sbyte bSupportPlcScanMemory;	

		public static sbyte bCommunicationValue = 1;		// 현재값 통신
		public static sbyte bExchangeAlarmStatus = 1;		// 경보 상태값 교환 (삭제, 확인 등)

		static void LoadOldConfig()
		{
			string filename;
			const string sSectionTimeSync = "TimeSync";
			const string sSectionSupport = "Support";

			filename = String.Format("{0}\\network\\NetClnt.ini", TotalConfig.sDirWorkProject);

			Profile.GetPrivateProfileStringA("Node", "Name", "Input Node Name", ref configStruct.sNodeName, filename);
			configStruct.nNodeType = Profile.GetPrivateProfileIntA("Node", "Type", 0, filename);
			ConfigNetCommon.nServerPort = Profile.GetPrivateProfileIntA("Node", "Server Port", 7000, filename);

			configStruct.bChangeConditionPlcScanTimeOut = (sbyte)Profile.GetPrivateProfileIntA("ChangeCondition", "PlcScanTimeOut", 1, filename);
			configStruct.bChangeConditionProgramViewMain = (sbyte)Profile.GetPrivateProfileIntA("ChangeCondition", "ProgramViewMain", 0, filename);
			configStruct.bChangeConditionProgramPlcScan = (sbyte)Profile.GetPrivateProfileIntA("ChangeCondition", "ProgramPlcScan", 0, filename);
			configStruct.bChangeConditionProgramRunMain = (sbyte)Profile.GetPrivateProfileIntA("ChangeCondition", "ProgramRunMain", 0, filename);
			configStruct.bChangeConditionProtectChangeTime = (sbyte)Profile.GetPrivateProfileIntA("ChangeCondition", "ProtectChangeTime", 0, filename);

			configStruct.bLatchLinePrinter = (sbyte)Profile.GetPrivateProfileIntA("Latch", "LinePrinter", 0, filename);
			configStruct.bLatchReportPrinter = (sbyte)Profile.GetPrivateProfileIntA("Latch", "ReportPrinter", 0, filename);

			configStruct.bTimeSyncActive = (sbyte)Profile.GetPrivateProfileIntA(sSectionTimeSync, "bTimeSyncActive", 0, filename);
			configStruct.nTimeSyncHour   = Profile.GetPrivateProfileIntA(sSectionTimeSync, "nTimeSyncHour", 0, filename);
			configStruct.nTimeSyncMin    = Profile.GetPrivateProfileIntA(sSectionTimeSync, "nTimeSyncMin", 0, filename);
			configStruct.nTimeSyncWhen   = Profile.GetPrivateProfileIntA(sSectionTimeSync, "nTimeSyncWhen", 0, filename);

			//nUdpPort = configStruct.nServerPort;

			configStruct.bSupportPlcScanMemory = (sbyte)Profile.GetPrivateProfileIntA(sSectionSupport, "PlcScanMemory", 0, filename);

			configStruct.bCommunicationValue = (sbyte)Profile.GetPrivateProfileIntA("Communication", "Value", 1, filename);
			SystemStatusMemory.SetDI(SSMDI.CommunicationValue, configStruct.bCommunicationValue);

			configStruct.bExchangeAlarmStatus = (sbyte)Profile.GetPrivateProfileIntA("Communication", "bExchangeAlarmStatus", 1, filename);
			SystemStatusMemory.SetDI(SSMDI.bExchangeAlarmStatus, configStruct.bExchangeAlarmStatus);
		}

		public static void Load()
		{
			string data;
			CommaBlockString comma = new CommaBlockString();
			TextReader reader;
			string command = "";
			string filename;

			filename = String.Format("{0}\\network\\NetClient.cfgx", TotalConfig.sDirWorkProject);

			if(!File.Exists(filename)) 
			{
				LoadOldConfig();
				return;
			}

            reader = new StreamReader(filename);
			if(reader == null)	return;
			while(true) 
			{
				data = reader.ReadLine();
				if(data == null)	break;
				if(data.Length == 0)	continue;

				comma.Set(data);

				comma.GetString(ref command);

				if(command == "sNodeName")	comma.GetString(ref sNodeName);
				
				else if(command == "nNodeType")	comma.GetInt(ref nNodeType);
				else if(command == "nServerPort")	comma.GetInt(ref ConfigNetCommon.nServerPort);

				else if(command == "bChangeConditionPlcScanTimeOut")	comma.GetChar(ref bChangeConditionPlcScanTimeOut);
				else if(command == "bChangeConditionProgramViewMain")	comma.GetChar(ref bChangeConditionProgramViewMain);
				else if(command == "bChangeConditionProgramPlcScan")	comma.GetChar(ref bChangeConditionProgramPlcScan);
				else if(command == "bChangeConditionProgramRunMain")	comma.GetChar(ref bChangeConditionProgramRunMain);
				else if(command == "bChangeConditionProtectChangeTime")	comma.GetChar(ref bChangeConditionProtectChangeTime);

				else if(command == "bLatchLinePrinter")	comma.GetChar(ref bLatchLinePrinter);
				else if(command == "bLatchReportPrinter")	comma.GetChar(ref bLatchReportPrinter);

				else if(command == "bTimeSyncActive")	comma.GetChar(ref bTimeSyncActive);
				else if(command == "nTimeSyncHour")	comma.GetInt(ref nTimeSyncHour);
				else if(command == "nTimeSyncMin")	comma.GetInt(ref nTimeSyncMin);
				else if(command == "nTimeSyncWhen")	comma.GetInt(ref nTimeSyncWhen);
				else if(command == "bSupportPlcScanMemory")	comma.GetChar(ref bSupportPlcScanMemory);
				else if(command == "bCommunicationValue")	comma.GetChar(ref bCommunicationValue);
				else if(command == "bExchangeAlarmStatus")	comma.GetChar(ref bExchangeAlarmStatus);
				else {}
			}
			reader.Close();

			SystemStatusMemory.SetDI(SSMDI.CommunicationValue, configStruct.bCommunicationValue);
			SystemStatusMemory.SetDI(SSMDI.bExchangeAlarmStatus, configStruct.bExchangeAlarmStatus);
		}

		public static void Save()
		{
			string filename;
			TextWriter writer;

			filename = String.Format("{0}\\network", TotalConfig.sDirWorkProject);
			Directory.CreateDirectory(filename);
	
			filename = String.Format("{0}\\network\\NetClient.cfgx", TotalConfig.sDirWorkProject);

			writer = new StreamWriter(filename);
			if(writer == null)	return;

			writer.WriteLine("sNodeName,{0},", sNodeName);
			writer.WriteLine("nNodeType,{0},", nNodeType);
			writer.WriteLine("nServerPort,{0},", ConfigNetCommon.nServerPort);

			writer.WriteLine("bChangeConditionPlcScanTimeOut,{0},", bChangeConditionPlcScanTimeOut);
			writer.WriteLine("bChangeConditionProgramViewMain,{0},", bChangeConditionProgramViewMain);
			writer.WriteLine("bChangeConditionProgramPlcScan,{0},", bChangeConditionProgramPlcScan);
			writer.WriteLine("bChangeConditionProgramRunMain,{0},", bChangeConditionProgramRunMain);
			writer.WriteLine("bChangeConditionProtectChangeTime,{0},", bChangeConditionProtectChangeTime);

			writer.WriteLine("bLatchLinePrinter,{0},", bLatchLinePrinter);
			writer.WriteLine("bLatchReportPrinter,{0},", bLatchReportPrinter);

			writer.WriteLine("bTimeSyncActive,{0},", bTimeSyncActive);
			writer.WriteLine("nTimeSyncHour,{0},", nTimeSyncHour);
			writer.WriteLine("nTimeSyncMin,{0},", nTimeSyncMin);
			writer.WriteLine("nTimeSyncWhen,{0},", nTimeSyncWhen);

			writer.WriteLine("bSupportPlcScanMemory,{0},", bSupportPlcScanMemory);
			writer.WriteLine("bCommunicationValue,{0},", bCommunicationValue);
			writer.WriteLine("bExchangeAlarmStatus,{0},", bExchangeAlarmStatus);

			writer.Close();
		}
	}
}

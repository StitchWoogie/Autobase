using System;
using System.Runtime.InteropServices;

namespace AutoLibLocal
{
	// 현재 50개(0~49)의 여유가 있다.
	public enum EnumWatchDogInfo
	{
		WDI_LocalMain = 0,	//9.0 이전에는 WDI_AutoBase
		WDI_PlcScan = 1,
		//WDI_Reporter = 2,	이 번호는 그대로 유지하도록 한다.
		WDI_NetServ = 3,
		WDI_NetClnt = 4,
		WDI_RunMain = 5,	
		//WDI_Beeper = 6,	이 번호는 그대로 유지하도록 한다.
		WDI_LinePrinter = 7,
		WDI_SMS = 8,
		WDI_OpcClient = 9,
		WDI_ExportServer = 10,

        WDI_DataSync = 11,  // 종성의 DataSync.exe 폴더에 존재하지 않을 수도 있다.

        WDI_OpcUAClient = 12, // // OPC UA 24-09-02 추가 hsjeong

        WDI_OpcUAServer = 13, // // OPC UA Server 24-11-27 추가 hsjeong
        WDI_RESTAPIClient = 14, //RESTAPIClient 추가 250106 PSU

        // CE용 실행파일?
        WDI_VncServer = 20, // 

        // 30~49 는 사용자 용으로 비워둔다.
	}

	/// <summary>
	/// Summary description for WatchDogInfo.
	/// </summary>
	public class WatchDogInfo
	{
        public delegate void DelegateInit();
        public delegate void DelegateUnInit();
        public delegate int DelegateWatchDogInfoTimerGet(int address);
        public delegate void DelegateWatchDogInfoTimerSet(int address, int val);

        static DelegateInit WatchDogInfoInit = null;
        static DelegateUnInit WatchDogInfoUnInit = null;
        static DelegateWatchDogInfoTimerGet WatchDogInfoTimerGet = null;
        static DelegateWatchDogInfoTimerSet WatchDogInfoTimerSet = null;

        static bool bWatchDogError = false; // 오류가 나는것은 파일이 없는것일것이다.

        static WatchDogInfo()
        {
            //
            // TODO: Add constructor logic here
            //

            if (IntPtr.Size == 8)
            {  // 64bit
                WatchDogInfoInit = WatchDogInfo64.Init;
                WatchDogInfoUnInit = WatchDogInfo64.UnInit;
                WatchDogInfoTimerGet = WatchDogInfo64.WatchDogInfoTimerGet;
                WatchDogInfoTimerSet = WatchDogInfo64.WatchDogInfoTimerSet;
            }
            else
            {
                WatchDogInfoInit = WatchDogInfo32.Init;
                WatchDogInfoUnInit = WatchDogInfo32.UnInit;
                WatchDogInfoTimerGet = WatchDogInfo32.WatchDogInfoTimerGet;
                WatchDogInfoTimerSet = WatchDogInfo32.WatchDogInfoTimerSet;
            }
        }

        public static void Init()
        {
            if (bWatchDogError) return;

            try
            {
                WatchDogInfoInit();
            }
            catch
            {
                bWatchDogError = true;
            }
        }

        public static void UnInit()
        {
            if (bWatchDogError) return;

            try
            {
                WatchDogInfoUnInit();
            }
            catch
            {
                bWatchDogError = true;
            }
        }

		public static int GetTimer(EnumWatchDogInfo address)
		{
            if (bWatchDogError) return 0;

            try
            {
                return WatchDogInfoTimerGet((int)address);
            }
            catch 
            {
                bWatchDogError = true;
                return 0;
            }
		}

		public static void SetTimer(EnumWatchDogInfo address, int val)
		{
            if (bWatchDogError) return;

            try
            {
			    WatchDogInfoTimerSet((int)address, val);
            }
            catch
            {
                bWatchDogError = true;
            }
		}
	}
}

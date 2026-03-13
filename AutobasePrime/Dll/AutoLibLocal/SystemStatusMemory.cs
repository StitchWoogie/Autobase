using System;
using System.Runtime.InteropServices;

namespace AutoLibLocal
{
	public enum SSMDI 
	{
		DuplexConnect		= 0x0000,	//	다중화 서버에서 상대 컴퓨터와의 연결 여부
		DuplexActiveI		= 0x0001,	//	다중화 서버에서 내 컴퓨터 제어권 여부
		DuplexActiveYou		= 0x0002,	//	다중화 서버에서 상대 컴퓨터 제어권 여부

		DuplexAutoWatchI	= 0x0003,	//	내 컴퓨터의 자동 감시 여부
		DuplexAutoWatchYou	= 0x0004,	//	상대 컴퓨터의 자동 감시 여부

		DuplexActivePrimary	= 0x0007,	//	다중화 서버에서 Primary Server의 활성	클라이언트에서 활용
		DuplexActiveSecondary = 0x0008,	//	다중화 서버에서 Secondary Server의 활성	클라이언트에서 활용

		DuplexControlItemLinePrinter	= 0x0010,	//	Line Printer의 동작
		DuplexControlItemReportPrinter	= 0x0011,	//	Report Printer의 동작

		ErrorStatusPlcScanTimeOut		= 0x0020,	// PLC SCAN 메모리의 어느하나라도 통신이 5회 이상 이상하면 Set 해준다.
		ErrorStatusPlcScanTimeOutYou	= 0x0021,	// PLC SCAN 메모리의 어느하나라도 통신이 5회 이상 이상하면 Set 해준다.	상대편의 상태
		AnotherProgramExitingI			= 0x0022,	// 상태편의 부가 프로그램이 종료중인가를 검사한다.
		AnotherProgramExitingYou		= 0x0023,	// 상태편의 부가 프로그램이 종료중인가를 검사한다.

		CommunicationValue				= 0x0030,	// 통신 시 현재값도 통신
		PlcScanEventToLocalMain			= 0x0031,	// 통신 프로그램이 시작되거나 포트의 설정이 바뀌면 감시프로그램이 알수 있도록 한다. 감시프로그램에서 통신 메모리를 다시 Map 해야하므로.
		bExchangeAlarmStatus			= 0x0032,	// 통신 시 경보상태도 교환, (확인,삭제 등)
        PlcScanEventToGateway           = 0x0033,   // 통신 프로그램이 시작되거나 포트의 설정이 바뀌면 Gateway프로그램이 알수 있도록 한다. Gateway프로그램에서 통신 메모리를 다시 Map 해야하므로. 
        

		//  DI에서 0100~011F		resered
		//	0x0100~010F			네트워크 기본 Device의 선로 상태	실제 태그에서는 0160~017F까지 사용된다.
		//	0x0110~011F			네트워크 예비 Device의 선로 상태 
	}
	/// <summary>
	/// Summary description for SystemStatusMemory.
	/// </summary>
	public class SystemStatusMemory
	{
        public delegate void DelegateInit();

        public delegate void DelegateDllInit();
        public delegate void DelegateDllUnInit();
        public delegate int DelegateDllGetDI(ushort address);
        public delegate void DelegateDllSetDI(ushort address, sbyte flag);

        static DelegateDllInit DllInit = null;
        static DelegateDllUnInit DllUnInit = null;
        static DelegateDllGetDI DllGetDI = null;
        static DelegateDllSetDI DllSetDI = null;

		static SystemStatusMemory()
		{
			//
			// TODO: Add constructor logic here
			//
            if (IntPtr.Size == 8)
            {
                DllInit = SystemStatusMemory64.DllInit;
                DllUnInit = SystemStatusMemory64.DllUnInit;
                DllGetDI = SystemStatusMemory64.DllGetDI;
                DllSetDI = SystemStatusMemory64.DllSetDI;
            }
            else
            {
                DllInit = SystemStatusMemory32.DllInit;
                DllUnInit = SystemStatusMemory32.DllUnInit;
                DllGetDI = SystemStatusMemory32.DllGetDI;
                DllSetDI = SystemStatusMemory32.DllSetDI;
            }
		}

		public static int GetDI(int id)
		{
			return DllGetDI((ushort)id);
		}

		public static int GetDI(SSMDI id)
		{
			return DllGetDI((ushort)id);
		}

		public static void SetDI(int id, sbyte flag)
		{
			DllSetDI((ushort)id, flag);
		}

		public static void SetDI(SSMDI id, sbyte flag)
		{
			DllSetDI((ushort)id, flag);
		}

        public static void Init()
        {
            DllInit();
        }

        public static void UnInit()
        {
            DllUnInit();
        }


	}
}


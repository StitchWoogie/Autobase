using System;
using System.Runtime.InteropServices;

namespace NetTools
{
	/// <summary>
	/// Summary description for Win32Function.
	/// </summary>
	public class Win32Function
	{
		public Win32Function()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		// 중복 실행을 방지...
		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		// 이미 실행 중이면 화면 맨앞으로 오게...
		[DllImport("user32.dll")]
		public static extern void BringWindowToTop(IntPtr hWnd);
		// 이미 실행 중이면 포커스...
		[DllImport("user32.dll")]
		public static extern void SetForegroundWindow(IntPtr hWnd);

		/// <summary>
		/// 기존의 32Bit프로그램은 이함수로 실행여부를 찾을 수 있다.
		/// if(hwnd != IntPtr.Zero) 이미 실행 중
		/// </summary>
		/// <param name="ClassName">C++프로그램은 찾을 수 있으나 .NET프로그램은 클래스 이름이 같은 경우가 많다. 클래스 이름은 지정할 수 없다.</param>
		/// <param name="WindowName">윈도우 제목은 활성화된 MDI에 따라서 다르다.</param>
		/// <returns></returns>
		[DllImport("user32.dll")] 
		public static extern IntPtr FindWindow(string ClassName, string WindowName); 

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern int PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		[DllImport("kernel32.dll")] 
		public static extern bool Beep(uint freq, uint dur); 

		[DllImport("kernel32.dll")]
		public static extern uint WinExec(String lpCmdLine, uint uCmdShow);

		[DllImport("winmm.dll")]
		public static extern int PlaySound(String lpszName, int hModule, EnumPlaySound dwFlags); 
		

		public static byte HIBYTE(ushort val)
		{
			return (byte)((val >> 8) & 0xFF);
		}

		public static byte LOBYTE(ushort val)
		{
			return (byte)((val >> 0) & 0xFF);
		}

		public static ushort HIWORD(uint val)
		{
			return (ushort)((val >> 16) & 0xFFFF);
		}

		public static ushort LOWORD(uint val)
		{
			return (ushort)((val >> 0) & 0xFFFF);
		}

	}

	public enum EnumShowWindow 
	{
		SW_HIDE        =     0,
		SW_SHOWNORMAL   =    1,
		SW_NORMAL        =   1,
		SW_SHOWMINIMIZED  =  2,
		SW_SHOWMAXIMIZED   = 3,
		SW_MAXIMIZE         =3,
		SW_SHOWNOACTIVATE   =4,
		SW_SHOW             =5,
		SW_MINIMIZE         =6,
		SW_SHOWMINNOACTIVE  =7,
		SW_SHOWNA           =8,
		SW_RESTORE          =9,
		SW_SHOWDEFAULT      =10,
		SW_FORCEMINIMIZE    =11,
		SW_MAX              =11,
	}

	public enum EnumPlaySound 
	{
		SND_SYNC =          0x0000,  /* play synchronously (default) */
		SND_ASYNC =         0x0001,  /* play asynchronously */
		SND_NODEFAULT      = 0x0002,  /* silence (!default) if sound not found */
		SND_MEMORY =         0x0004,  /* pszSound points to a memory file */
		SND_LOOP     =       0x0008,  /* loop the sound until next sndPlaySound */
		SND_NOSTOP    =      0x0010,  /* don't stop any currently playing sound */

		SND_NOWAIT   =   0x00002000, /* don't wait if the driver is busy */
		SND_ALIAS     =  0x00010000, /* name is a registry alias */
		SND_ALIAS_ID  =  0x00110000, /* alias is a predefined ID */
		SND_FILENAME  =  0x00020000, /* name is file name */
		SND_RESOURCE  =  0x00040004, /* name is resource name or atom */
		//#if(WINVER >= 0x0400)
		SND_PURGE      =     0x0040,  /* purge non-static events for task */
		SND_APPLICATION =    0x0080,  /* look for application specific association */
		//#endif /* WINVER >= 0x0400 */
	}

}

using System;
using System.Runtime.InteropServices;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for WatchDogInfo.
	/// </summary>
	public class WatchDogInfo32
	{
		[DllImport("Win32Common.DLL", EntryPoint="WatchDogInfoInit", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Init();

        [DllImport("Win32Common.DLL", EntryPoint = "WatchDogInfoUnInit", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UnInit();

        [DllImport("Win32Common.DLL", EntryPoint = "WatchDogInfoTimerGet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WatchDogInfoTimerGet(int address);

        [DllImport("Win32Common.DLL", EntryPoint = "WatchDogInfoTimerSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WatchDogInfoTimerSet(int address, int val);
	}
}

using System;
using System.Runtime.InteropServices;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for WatchDogInfo.
	/// </summary>
	public class WatchDogInfo64
	{
        [DllImport("x64\\WatchDogInfo.DLL", EntryPoint = "WatchDogInfoInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init();

        [DllImport("x64\\WatchDogInfo.DLL", EntryPoint = "WatchDogInfoUnInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UnInit();

        [DllImport("x64\\WatchDogInfo.DLL", EntryPoint = "WatchDogInfoTimerGet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WatchDogInfoTimerGet(int address);

        [DllImport("x64\\WatchDogInfo.DLL", EntryPoint = "WatchDogInfoTimerSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WatchDogInfoTimerSet(int address, int val);
	}
}

using System;
using System.Runtime.InteropServices;

namespace AutoLibLocal
{
	
	/// <summary>
	/// Summary description for SystemStatusMemory.
	/// </summary>
	public class SystemStatusMemory64
	{
        [DllImport("Win64Common.DLL", EntryPoint = "SystemStatusInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DllInit();

        [DllImport("Win64Common.DLL", EntryPoint = "SystemStatusUninit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DllUnInit();

        [DllImport("Win64Common.DLL", EntryPoint = "GetDI", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DllGetDI(ushort address);

        [DllImport("Win64Common.DLL", EntryPoint = "SetDI", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DllSetDI(ushort address, sbyte flag);

	}
}


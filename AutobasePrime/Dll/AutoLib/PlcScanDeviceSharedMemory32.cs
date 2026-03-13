using System;
using System.Runtime.InteropServices;	

namespace AutoLib
{
	/// <summary>
	/// Summary description for PlcScanDeviceSharedMemory.
	/// </summary>
	public class PlcScanDeviceSharedMemory32
	{
		[DllImport("Win32Common.DLL", EntryPoint="PlcScanDeviceSharedMemoryInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint DllInit(string name);

        [DllImport("Win32Common.DLL", EntryPoint = "PlcScanDeviceSharedMemoryUnInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DllUnInit(uint id);

        [DllImport("Win32Common.DLL", EntryPoint = "PlcScanDeviceSharedMemoryRead", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DllRead(uint id);

        [DllImport("Win32Common.DLL", EntryPoint = "PlcScanDeviceSharedMemoryWrite", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DllWrite(uint id, byte ch);
	}
}

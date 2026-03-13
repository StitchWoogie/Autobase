using System;
using System.Runtime.InteropServices;	

namespace AutoLib
{
	/// <summary>
	/// Summary description for PlcScanDeviceSharedMemory.
	/// </summary>
	public class PlcScanDeviceSharedMemory64
	{
        [DllImport("x64\\PlcScanDeviceSharedMemory.DLL", EntryPoint = "PlcScanDeviceSharedMemoryInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint DllInit(string name);

        [DllImport("x64\\PlcScanDeviceSharedMemory.DLL", EntryPoint = "PlcScanDeviceSharedMemoryUnInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DllUnInit(uint id);

        [DllImport("x64\\PlcScanDeviceSharedMemory.DLL", EntryPoint = "PlcScanDeviceSharedMemoryRead", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DllRead(uint id);

        [DllImport("x64\\PlcScanDeviceSharedMemory.DLL", EntryPoint = "PlcScanDeviceSharedMemoryWrite", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DllWrite(uint id, byte ch);
	}
}

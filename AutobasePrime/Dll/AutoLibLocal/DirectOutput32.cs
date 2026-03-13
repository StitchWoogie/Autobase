using System;
using System.Runtime.InteropServices;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for DirectOutput.
	/// </summary>
	
    public class DirectOutput32
	{
		[DllImport("Win32Common.DLL", EntryPoint="OpenWriteClose", CallingConvention = CallingConvention.Cdecl)]
		public static extern int OpenWriteClose(sbyte port_type, int port_no, int port_baud, sbyte port_parity, sbyte port_data, sbyte port_stop, String msg, int size);
	}
}

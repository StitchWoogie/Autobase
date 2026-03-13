using System;
using System.Runtime.InteropServices;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for DirectOutput.
	/// </summary>
	public class DirectOutput
	{
        public delegate int DelegateOpenWriteClose(sbyte port_type, int port_no, int port_baud, sbyte port_parity, sbyte port_data, sbyte port_stop, String msg, int size);
        public static DelegateOpenWriteClose OpenWriteClose = null;

		static DirectOutput()
		{
			//
			// TODO: Add constructor logic here
			//
            if (IntPtr.Size == 8)
            {
                OpenWriteClose = DirectOutput64.OpenWriteClose;
            }
            else
            {
                OpenWriteClose = DirectOutput32.OpenWriteClose;
            }
		}

	}
}

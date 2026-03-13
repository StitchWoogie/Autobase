using System;
using System.Runtime.InteropServices;	

namespace AutoLib
{
	/// <summary>
	/// Summary description for PlcScanDeviceSharedMemory.
	/// </summary>
	public class PlcScanDeviceSharedMemory
	{
        public delegate uint DelegateDllInit(string name);
        public delegate void DelegateDllUnInit(uint id);
        public delegate int DelegateDllRead(uint id);
        public delegate void DelegateDllWrite(uint id, byte ch);

        static DelegateDllInit DllInit = null;
        static DelegateDllUnInit DllUnInit = null;
        static DelegateDllRead DllRead = null;
        static DelegateDllWrite DllWrite = null;

		static PlcScanDeviceSharedMemory()
		{
			//
			// TODO: Add constructor logic here
			//
            if (IntPtr.Size == 8)
            {
                DllInit = PlcScanDeviceSharedMemory64.DllInit;
                DllUnInit = PlcScanDeviceSharedMemory64.DllUnInit;
                DllRead = PlcScanDeviceSharedMemory64.DllRead;
                DllWrite = PlcScanDeviceSharedMemory64.DllWrite;
            }
            else
            {
                DllInit = PlcScanDeviceSharedMemory32.DllInit;
                DllUnInit = PlcScanDeviceSharedMemory32.DllUnInit;
                DllRead = PlcScanDeviceSharedMemory32.DllRead;
                DllWrite = PlcScanDeviceSharedMemory32.DllWrite;
            }
		}

		~PlcScanDeviceSharedMemory()
		{
			Close();
		}

		
		

		uint id = 0;

		public void Open(string name)
		{
			id = DllInit(name);
		}

		public void Close()
		{
			if(id == 0)	return;
			DllUnInit(id);
			id = 0;
		}

		public int Read()
		{
			if(id == 0)	return -1;
			return DllRead(id);
		}

		public void Write(byte ch)
		{
			if(id == 0)	return;
			DllWrite(id, ch);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AutoLibLocal
{
    public class RingSharedMemory64
    {
        [DllImport("Win64Common.DLL", EntryPoint = "RingCreate", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RingCreate(string name, int ring_count, int ring_size);

        [DllImport("Win64Common.DLL", EntryPoint = "RingGetItem", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RingGetItem(IntPtr handle, byte[] buf, int size);

        [DllImport("Win64Common.DLL", EntryPoint = "RingAddItem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RingAddItem(IntPtr handle, byte[] buf, int size);

        [DllImport("Win64Common.DLL", EntryPoint = "RingClose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RingClose(IntPtr handle);
    }
}

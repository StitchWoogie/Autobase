using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LocalMain.SystemInfo
{
    class SystemInfoGate
    {
        [DllImport("W83627UHG_GPIO.DLL", EntryPoint = "GpioProtocolInit", CallingConvention = CallingConvention.Cdecl)]
        static extern void GpioProtocolInit(byte cPortInOut, bool bInvert, ushort nWaitTime);

        [DllImport("W83627UHG_GPIO.DLL", EntryPoint = "GpioProtocolUnInit", CallingConvention = CallingConvention.Cdecl)]
        static extern void GpioProtocolUnInit();

        [DllImport("W83627UHG_GPIO.DLL", EntryPoint = "GpioProtocolRead", CallingConvention = CallingConvention.Cdecl)]
        static extern int GpioProtocolRead(string device, out double retn);

        [DllImport("W83627UHG_GPIO.DLL", EntryPoint = "GpioProtocolWrite", CallingConvention = CallingConvention.Cdecl)]
        static extern int GpioProtocolWrite(uint address, string device, double val); 

        public static void GateInit()
        {
            GpioProtocolInit(0xFF, false, 5);
        }

        public static void GateUnInit()
        {
            GpioProtocolUnInit();
        }

        public static bool GateRead(string device, out double val)
        {
            int retn = GpioProtocolRead(device, out val);

            return (retn == 1);
        }

        public static void GateWrite()
        {

        }
    }
}

using System;
using NetTools;
using AutoLibLocal;
using System.Diagnostics;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ScriptFunctionPlcScan.
	/// </summary>
	public class ScriptFunctionPlcScan
	{
        /*
		public ScriptFunctionPlcScan()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public delegate int IntProcIntIntStringIntInt(int port, int station, string type, int address, int delay_value);
		public static IntProcIntIntStringIntInt procReadDelayCommandToPlcScan = null;

        public delegate bool DelegateDigitalOutput(string tag, short port, short station, uint address, string sExtraAddr, ushort wExtraAddr, sbyte bReverse, sbyte flag);
        public static DelegateDigitalOutput procDigitalOutput = null;

        public delegate bool DelegateAnalogOutput(string tag, short port, short station, uint address, string sExtraAddr, ushort wExtraAddr, double out_value);
        public static DelegateAnalogOutput procAnalogOutput = null;

        public delegate bool DelegateBlockOutput(string tag, short port, short station, uint address, string sExtraAddr, ushort wExtraAddr, object out_value, int blocksize);
        public static DelegateBlockOutput procBlockOutput = null;
        
		static int Function_PlcScanSetReadDelay(ScriptClass scriptClass, string command, string argument, out object value)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
	
			int port;
			int station;
			string type;
			int address;
			int delay_value;

			value = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out port))		return -1;

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out station))		return -1;

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out type))		return -1;

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out address))		return -1;

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out delay_value))		return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(procReadDelayCommandToPlcScan != null)
					procReadDelayCommandToPlcScan(port, station, type, address,delay_value);
			}

			return 1;
		}

        static int Function_PlcScanWriteBit(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;

            int port;
            int station;
            int address;
            string extra1;
            int extra2;
            int val;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out port)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out station)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out address)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out extra1)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out extra2)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out val)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procDigitalOutput != null)
                    procDigitalOutput("ScriptOutput", (short)port, (short)station, (uint)address, extra1, (ushort)extra2, 0, (sbyte)val);
            }

            return 1;
        }

        static int Function_PlcScanWriteWord(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;

            int port;
            int station;
            int address;
            string extra1;
            int extra2;
            double val;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out port)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out station)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out address)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out extra1)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out extra2)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out val)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procAnalogOutput != null)
                    procAnalogOutput("ScriptOutput", (short)port, (short)station, (uint)address, extra1, (ushort)extra2, val);
            }

            return 1;
        }

        static int Function_PlcScanWriteBlock(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;

            int port;
            int station;
            int address;
            string extra1;
            int extra2;
            object val;
            int array_size;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out port)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out station)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out address)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out extra1)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out extra2)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out val)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out array_size)) return -1;

            if (array_size < 0) array_size = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procBlockOutput != null)
                {
                    procBlockOutput("ScriptOutput", (short)port, (short)station, (uint)address, extra1, (ushort)extra2, val, array_size);
                }
            }

            return 1;
        }

		public static int Function_PlcScan(ScriptClass scriptClass, string command, string argument, out object value)
		{
			if(String.Compare(command, "PlcScanSetReadDelay") == 0) 
			{
				return Function_PlcScanSetReadDelay(scriptClass, command, argument, out value);
			}
            else if (String.Compare(command, "PlcScanWriteBit") == 0)
            {
                return Function_PlcScanWriteBit(scriptClass, command, argument, out value);
            }
            else if (String.Compare(command, "PlcScanWriteWord") == 0)
            {
                return Function_PlcScanWriteWord(scriptClass, command, argument, out value);
            }
            else if (String.Compare(command, "PlcScanWriteBlock") == 0)
            {
                return Function_PlcScanWriteBlock(scriptClass, command, argument, out value);
            }
			else 
			{
				if(Tools.IsLangKorean()) 
				{
					scriptClass.ErrorMessage(String.Format("지원되지 않는 PlcScan 함수입니다.({0})", command));
				}
				else if(Tools.IsLangChinese()) 
				{
					scriptClass.ErrorMessage(String.Format("不支持的 PlcScan 函数。({0})", command));
				}
				else 
				{
					scriptClass.ErrorMessage(String.Format("Undefined PlcScan function ({0})", command));
				}
				value = 0;
				return -1;
			}
		}*/

        public delegate int IntProcIntIntStringIntInt(int port, int station, string type, int address, int delay_value);
        public static IntProcIntIntStringIntInt procReadDelayCommandToPlcScan = null;

        public delegate bool DelegateDigitalOutput(string tag, short port, short station, uint address, string sExtraAddr, ushort wExtraAddr, sbyte bReverse, sbyte flag);
        public static DelegateDigitalOutput procDigitalOutput = null;

        public delegate bool DelegateAnalogOutput(string tag, short port, short station, uint address, string sExtraAddr, ushort wExtraAddr, double out_value);
        public static DelegateAnalogOutput procAnalogOutput = null;

        public delegate bool DelegateBlockOutput(string tag, short port, short station, uint address, string sExtraAddr, ushort wExtraAddr, object out_value, int blocksize);
        public static DelegateBlockOutput procBlockOutput = null;

        static int Run_PlcScanSetReadDelay(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            int port = (int)args[0];
            int station = (int)args[1];
            string type = (string)args[2];
            int address = (int)args[3];
            int delay_value = (int)args[4];

            value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procReadDelayCommandToPlcScan != null)
                    procReadDelayCommandToPlcScan(port, station, type, address, delay_value);
            }

            return 1;
        }

        static int Run_PlcScanWriteBit(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            int port = (int)args[0];
            int station = (int)args[1];
            int address = (int)args[2];
            string extra1 = (string)args[3];
            int extra2 = (int)args[4];
            int val = (int)args[5];

            value = 0;

            if (SharedLocalMain.bTestMode == true) return 1; //20241010 PSU

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procDigitalOutput != null)
                    procDigitalOutput("ScriptOutput", (short)port, (short)station, (uint)address, extra1, (ushort)extra2, 0, (sbyte)val);
            }

            return 1;
        }

        static int Run_PlcScanWriteWord(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            int port = (int)args[0];
            int station = (int)args[1];
            int address = (int)args[2];
            string extra1 = (string)args[3];
            int extra2 = (int)args[4];
            double val = (double)args[5];

            value = 0;

            if (SharedLocalMain.bTestMode == true) return 1; //20241010 PSU

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procAnalogOutput != null)
                    procAnalogOutput("ScriptOutput", (short)port, (short)station, (uint)address, extra1, (ushort)extra2, val);
            }

            return 1;
        }

        //20250704 PSU 추가. 통신프로그램 WriteWord는 16진수 주소를 사용하여, 사용자가 10진수를 기입할 경우 알맞게 변환하여 전달한다.
        static int Run_PlcScanWriteWordDecimal(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            int port = (int)args[0];
            int station = (int)args[1];
            int address = (int)args[2];
            string extra1 = (string)args[3];
            int extra2 = (int)args[4];
            double val = (double)args[5];

            value = 0;
            try
            {
                // 사용자 입력 63을 "63"으로 변환하고, 이를 16진수로 해석
                string addressStr = address.ToString();
                address = Convert.ToInt32(addressStr, 16);  // "63"을 16진수로 해석 -> 99

                if (SharedLocalMain.bTestMode == true) return 1; 

                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                {

                    if (procAnalogOutput != null)
                        procAnalogOutput("ScriptOutput", (short)port, (short)station, (uint)address, extra1, (ushort)extra2, val);
                }
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(ex.Message);
                return -1;
            }

            return 1;
        }

        static Process SeekProcess(string processname)
        {
            Process[] p;

            p = Process.GetProcessesByName(processname);

            if (p.Length > 0)
            {
                return p[0];
            }

            p = Process.GetProcessesByName(processname + ".vshost");
            if (p.Length > 0)
            {
                return p[0];
            }

            return null;
        }

        static int Run_PlcScanWriteBlock(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            int port = (int)args[0];
            int station = (int)args[1];
            int address = (int)args[2];
            string extra1 = (string)args[3];
            int extra2 = (int)args[4];
            object val = args[5];
            int array_size = (int)args[6];

            value = 0;

            if (array_size < 0) array_size = 0;

            if (SharedLocalMain.bTestMode == true) return 1; //20241010 PSU

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procBlockOutput != null)
                {
                    procBlockOutput("ScriptOutput", (short)port, (short)station, (uint)address, extra1, (ushort)extra2, val, array_size);
                }
            }

            return 1;
        }

        static int Run_PlcScanSetPortActive(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            int port = (int)args[0];
            int flag = (int)args[1];

            value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                // Process p = SeekProcess("PLC_SCAN"); // PLC_SCAN이 트레이로 있을 때는 명령이 전달되지 않는다. 그래서 FindWindow로 수정했음. 2023-7-20
                IntPtr hwnd = Win32Function.FindWindow("PlcScanMainFrame", null);
                if (hwnd != IntPtr.Zero)
                {
                    Win32Function.PostMessage(hwnd, 0x111, (IntPtr)(int)EnumIdmPublic.IDM_PUBLIC_PLC_SCAN_PORT_ACTIVE, (IntPtr)(port * 10000 + flag));
                }

                // 이것은 PLC_SCAN이 트레이로 있을 때는 명령이 전달되지 않는다.
                //Process p = SeekProcess("PLC_SCAN");

                //if (p != null)
                //{
                //    Win32Function.PostMessage(p.MainWindowHandle, 0x111, (int)EnumIdmPublic.IDM_PUBLIC_PLC_SCAN_PORT_ACTIVE, port * 10000 + flag);
                //}
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "PlcScan";

            prepare.AddMethod(prename, "PlcScanSetReadDelay", "void", new ScriptExternalRun.DeleMethod(Run_PlcScanSetReadDelay), "in:int:port", "in:int:station", "in:string:type", "in:int:address", "in:int:delay_count");
            prepare.AddMethod(prename, "PlcScanWriteBit", "void", new ScriptExternalRun.DeleMethod(Run_PlcScanWriteBit), "in:int:port", "in:int:station", "in:int:address", "in:string:extra1", "in:int:extra2", "in:int:value");
            prepare.AddMethod(prename, "PlcScanWriteWord", "void", new ScriptExternalRun.DeleMethod(Run_PlcScanWriteWord), "in:int:port", "in:int:station", "in:int:address", "in:string:extra1", "in:int:extra2", "in:double:value");
            prepare.AddMethod(prename, "PlcScanWriteWordDecimal", "void", new ScriptExternalRun.DeleMethod(Run_PlcScanWriteWordDecimal), "in:int:port", "in:int:station", "in:int:address", "in:string:extra1", "in:int:extra2", "in:double:value");
            prepare.AddMethod(prename, "PlcScanWriteBlock", "void", new ScriptExternalRun.DeleMethod(Run_PlcScanWriteBlock), "in:int:port", "in:int:station", "in:int:address", "in:string:extra1", "in:int:extra2", "in:object:value", "in:int:array_size");

            prepare.AddMethod(prename, "PlcScanSetPortActive", "void", new ScriptExternalRun.DeleMethod(Run_PlcScanSetPortActive), "in:int:port", "in:int:flag");
        }
	}
}


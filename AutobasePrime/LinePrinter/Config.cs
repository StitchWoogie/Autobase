using System;
using AutoLibLocal;

namespace LinePrinter
{
	
	
	
	/// <summary>
	/// Summary description for Config.
	/// </summary>
	/// 
	public struct ESC_COMMAND
	{
		public bool		flag;
		public string	command;
	}
	
	public class Config
	{
		public static bool flag;
		public static bool cEscFlag;
		public static int		port_type;		// 사용할 프린터 포트? 프린터 = 0, serial = 1, Windows Printer 2,
		public static int		lpt_no;		// 프린터
		public static int		com_no;		// serial
		public static int		baud;
		public static int		parity;
		public static int		data;
		public static int		stop;
		public static int		day_type;
		public static int			timer;
		public static int		space_type;		// 각격 조정 0 : 디폴트, 1 : 사용자 정의 (string 에 의해)
		public static string		str;
		public static ESC_COMMAND[] esc = new ESC_COMMAND[11];	// esc command
        public static string sPrinterName;

		public Config()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void Load()
		{
			flag = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "flag", true);
			cEscFlag = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "cEscFlag", false);
			port_type = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "port_type", 0);
			lpt_no = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "lpt_no", 1);
			com_no = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "com_no", 1);
			baud = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "baud", 9600);
			parity = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "parity", 0);
			data = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "data", 8);
			stop = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "stop", 1);
			day_type = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "day_type", 0);
			timer = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "timer", 0);
			space_type = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "space_type", 0);
			str = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "str", "&[Date] &[Time] &[Tag] &[Desc] &[Msg]");
            sPrinterName = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "sPrinterName", "");

			string defaultcommand;
			for(int i = 0; i < 11; i++) 
			{
				InitColorEscCommand(out defaultcommand, i);
				esc[i].command = TotalConfig.LoadRegAutoBaseConfig("LinePrinter", "Config", "esc"+i.ToString(), defaultcommand);
			}
		}

		public static void Save()
		{
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "flag", flag);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "cEscFlag", cEscFlag);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "port_type", port_type);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "lpt_no", lpt_no);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "com_no", com_no);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "baud", baud);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "parity", parity);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "data", data);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "stop", stop);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "day_type", day_type);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "timer", timer);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "space_type", space_type);
			TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "str", str);
            TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "sPrinterName", sPrinterName);

			for(int i = 0; i < 11; i++) 
			{
				TotalConfig.SaveRegAutoBaseConfig("LinePrinter", "Config", "esc"+i.ToString(), esc[i].command);
			}
		}

		static readonly int[] ReadyColorNo = { 5, 5, 3, 3, 6, 6, 0, 4, 1, 2, 0 };

		static void InitColorEscCommand(out string command, int color)
		{
			command = "";
			command += (char)27;
			command += 'r';
			command += (char)((int)'0'+ReadyColorNo[color%11]);
		}

		public static void SetDefaultEsc()
		{
			for(int i = 0; i < 11; i++) 
			{
				InitColorEscCommand(out esc[i].command, i);
			}
		}
	}
}

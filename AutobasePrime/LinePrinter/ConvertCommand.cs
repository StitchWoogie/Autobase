using System;
using AutoLibLocal;

namespace LinePrinter
{
	/// <summary>
	/// Summary description for ConvertCommand.
	/// </summary>
	public class ConvertCommand
	{
		public ConvertCommand()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void Convert(out string buf, string source, ALARM_FILE_STRUCT alarm, bool date_flag)
		{
			buf = "";

			string command = "";
			bool command_mode = false;
			string imsi;
			int  i;

			buf = "";

			for(i = 0; i < source.Length; i++) 
			{
				if(command_mode == true) 
				{
					command += source[i];

					if(source[i] == ']') 
					{	// command close
						command_mode = false;
						if(String.Compare(command, 0, "[Date]", 0, 6, true) == 0 && date_flag == true) 
						{
							imsi = String.Format("{0:0000}-{1:00}-{2:00}", alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay);
							buf += imsi;
						}
						if(String.Compare(command, 0, "[Date/]", 0, 7, true) == 0 && date_flag == true) 
						{
							imsi = String.Format("{0}/{1:00}/{2:00}", alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay);
							buf += imsi;
						}
						else if(String.Compare(command, 0, "[Time]", 0, 6, true) == 0) 
						{					
							imsi = String.Format("{0:00}:{1:00}:{2:00}", alarm.t.wHour, alarm.t.wMinute, alarm.t.wSecond);
							buf += imsi;
						}
						else if(String.Compare(command, 0, "[Tag]", 0, 5, true) == 0) 
						{
							imsi = String.Format("{0,-10}", alarm.tag);
							buf += imsi;
						}
						else if(String.Compare(command, 0, "[Tag", 0, 4, true) == 0 && command[6] == ']') 
						{
							string 	format_string;
							int		num = 10;

							num = (command[4]-'0') * 10 + (command[5]-'0');
							if(num < 5 || num > 99) num = 10;
							format_string = "{0,-" + num.ToString()+"}";
							imsi = String.Format(format_string, alarm.tag);
							buf += imsi;
						}
						else if(String.Compare(command, 0, "[Desc]", 0, 6, true) == 0) 
						{
							imsi = String.Format("{0,-20}", alarm.description);
							buf += imsi;
						}				
						else if(String.Compare(command, 0, "[Desc", 0, 5, true) == 0 && command[7] == ']') 
						{
							string 	format_string;
							int		num = 20;

							num = (command[5]-'0') * 10 + (command[6]-'0');
							if(num < 5 || num > 99) num = 20;

							format_string = "{0,-" + num.ToString()+"}";
							imsi = String.Format(format_string, alarm.description);

							buf += imsi;
						}				
						else if(String.Compare(command, 0, "[Msg]", 0, 5, true) == 0) 
						{
							imsi = String.Format("{0}", alarm.msg);
							buf += imsi;
						}
						else if(String.Compare(command, 0, "[Msg", 0, 4, true) == 0 && command[6] == ']') 
						{
							string 	format_string;
							int		num = 40;

							num = (command[4]-'0') * 10 + (command[5]-'0');
							if(num < 5 || num > 99) num = 40;					
							format_string = "{0,-" + num.ToString()+"}";
							imsi = String.Format(format_string, alarm.msg);
							buf += imsi;
						}				
					}
				}
				else 
				{
					if(source[i] == '&') 
					{	// command
						command_mode = true;
						command = "";
					}
					else 
					{
						buf += source[i];

					}
				}
			}
		}
	}
}

using System;
using NetTools;
using AutoLibLocal;

namespace SMS.SmsFunc
{
	/// <summary>
	/// Summary description for ConvertRawMessageToString.
	/// </summary>
	public class ConvertRawMessageToString
	{
		public ConvertRawMessageToString()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static int getNumericDataToVal(string data, int val)
		{
			try 
			{
				return ConvertTool.ToInt32(data);
			}
			catch {}
			return val;
		}

		static public void ConvertHeadFootCommand(ref string buf, string source, ALARM_FILE_STRUCT alarm)//, ulong val)
		{
			string	command = "";
			bool	command_mode = false;
			int		i;

			buf = "";
			for(i = 0; i < source.Length; i++) 
			{
				if(command_mode == true) 
				{
					command += source[i];
					if(source[i] == ']') 
					{	// command close
						command_mode = false;						
						if(command == "[Date]") 
						{
							buf += string.Format("{0,4:d}-{1,2:d02}-{2,2:d02}", alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay);
						}
						if(command == "[Date/]") 
						{
							buf += string.Format("{0}/{1,2:d02}/{2,2:d02}", alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay);
						}
						else if(command == "[Time]") 
						{					
							buf += string.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}", alarm.t.wHour, alarm.t.wMinute, alarm.t.wSecond);
						}
						else if(command == "[Tag]") 
						{
							buf += string.Format("{0}", alarm.tag);
						}
						else if(command == "[Desc]") 
						{
							buf += string.Format("{0}", alarm.description);
						}
						else if(command == "[ArmNum]") 
						{
							buf += string.Format("{0,3:d03}", alarm.priority);
						}
						else if(command == "[TagType]") 
						{
							if(Tools.IsLangKorean()) 
							{
								if(alarm.enumTagType == EnumTagType.AI) buf += "Analog";
								else								    buf += "Digital";
							}
							else
							{
								if(alarm.enumTagType == EnumTagType.AI) buf += "아날로그";
								else									buf += "디지털"; 
							}
						}
						//else if(command == "[TagNum]") 
						//{
						//	buf += string.Format("{0,4:d04}", val % 0x10000);
						//}			
						else if(command == "[Msg]") 
						{
							buf += string.Format("{0}", alarm.msg);
						}
						else if(string.Compare(command, 0, "[Msg", 0, 4, true) == 0 && command[6] == ']') 
						{
							string 	imsi;
							int		num = getNumericDataToVal(command.Substring(4, 2), 40);
							string format_string;

							if(num < 5 || num > 99) num = 40;
							format_string = "{0,-"+num.ToString()+"}";
							imsi = string.Format(format_string, alarm.msg);
							imsi = imsi.Substring(0, num);
							buf += imsi;
						}
						else 
						{
							command = "";
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

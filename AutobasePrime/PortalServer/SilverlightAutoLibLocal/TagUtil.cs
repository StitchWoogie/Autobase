using System;
using NetTools;
using System.Collections.Generic;
using System.IO;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TagUtil.
	/// </summary>
	public class TagUtil
	{
		

		public TagUtil()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		

		public static bool StringToDisplayFormat(string buf, out float f, out char c)
		{
			if(buf.IndexOf('{') != -1)	// 신형 방식	9.1.2 부터 추가
			{
				try 
				{
					c = '{';
					f = 10.2f;
					buf = String.Format(buf, 123.45);	// 형식이 맞는지 체크한다.
					return true;
				}
				catch	
				{
					c = (char)0;
					f = 10.2f;
					return false;
				}
			}

			f = ConvertTool.ToSingle(buf);
			c = buf.Length > 0 ? buf[buf.Length-1] : (char)0;
			if(c >= '0' && c <= '9') 
			{
				c = (char)0;
			}

			return true;
		}


        // 앞에 빈칸이 붙는 아날로그는 의미가 없는것 같다.
        // 표시형식을 1000같은 값으로 하면 빈칸이 너무 커서 표시가 안되는 경우가 있다.
        // 일단 앞의 형식은 무시하기로 했다. 2007-7-9 

		//10.2 -> "   123.45";
		static public string AiValueToString(TagAiClass ai, double value)
		{
            return AiValueToStringOnlyPoint(ai, value);

            
		}

		//10.2 -> "123.45";
		static public string AiValueToStringOnlyPoint(TagAiClass ai, double value)
		{
			string buf;

			if(ai.cDisplayFormat == '{') 
			{
				buf = String.Format(ai.sDisplayFormat, value);
			}
			else 
			{
				string format_string;

				int high;
				int low;

				buf = ai.fDisplayFormat.ToString("F0");
				high = int.Parse(buf);
				buf = String.Format("{0,0:F0}", ai.fDisplayFormat*10);
				low = int.Parse(buf);
				low %= 10;
			    
				if(ai.cDisplayFormat == 'E')                                                         
					format_string = "{0,0:E"+low.ToString()+"}";
				else if(ai.cDisplayFormat == 'e')
					format_string = "{0,0:e"+low.ToString()+"}";
				else
					format_string = "{0,0:F"+low.ToString()+"}";
			
				buf = String.Format(format_string, value);
			}

			return buf;
		}

		static public string AiValueToStringOnlyPoint(string sFormat, float fFormat, char cFormat, double value)
		{
			string buf;

			if(cFormat == '{') 
			{
				buf = String.Format(sFormat, value);
			}
			else 
			{
				string format_string;

				int high;
				int low;

				buf = fFormat.ToString("F0");
				high = int.Parse(buf);
				buf = String.Format("{0,0:F0}", fFormat*10);
				low = int.Parse(buf);
				low %= 10;
			    
				if(cFormat == 'E')                                                         
					format_string = "{0,0:E"+low.ToString()+"}";
				else if(cFormat == 'e')
					format_string = "{0,0:e"+low.ToString()+"}";
				else
					format_string = "{0,0:F"+low.ToString()+"}";
			
				buf = String.Format(format_string, value);
			}

			return buf;
		}

		static public double GetDisplayValue(TagAiClass ai, double val)
		{
			if(ai.IsPowerFactorTag()) 
			{
				float real_gab = ai.fFull-ai.fBase;
				float view_gab = (float)Math.Abs(ai.view_full);

                if (val < 0)
                {
                    if (view_gab == 0)
                        val = (float)(Math.Abs(val));
                    else
                        val = (float)(real_gab * Math.Abs(val) / view_gab);

                    val = val - ai.fFull;
                }
                else
                {
                    if (view_gab == 0)
                        val = (float)(Math.Abs(val));
                    else
                        val = (float)(real_gab * Math.Abs(val) / view_gab);

                    val = ai.fFull - val;
                }
			}

			return val;
		}

		public static string GetDesON(TagDiClass di)
		{
			if(di.desON.Length == 0)	
			{
				return "ON";
			}
			else 
			{
				return di.desON;
			}
		}

		public static string GetDesOFF(TagDiClass di)
		{
			if(di.desOFF.Length == 0)	
			{
				return "OFF";
			}
			else 
			{
				return di.desOFF;
			}
		}

		static string ConvertTagToFileOld(string tag)
		{
			bool ext_flag = false;	// 확장자를 구분 했느냐?
			int  pos = 0;
			int  count_ascii = 0;
			string buf = "";
			char one;
			
			while(count_ascii < 12) 
			{
				if(pos < tag.Length)	// 태그 읽는중
				{
					one = tag[pos];
					pos++;
				}
				else 
				{
					one = '_';
				}

				if(one <= 32)		one = '_';
				else if(one == '/')	one = '_';
				else if(one == '.')	one = '_';
				else if(one == '\\')one = '_';
				else if(one == '*')	one = '_';
				else if(one == '+')	one = '_';
				else if(one == '|')	one = '_';
				else if(one == ':')	one = '_';
				else if(one == ';')	one = '_';
				else if(one == '"')	one = '_';
				else if(one == '<')	one = '_';
				else if(one == '>')	one = '_';
				else if(one == '?')	one = '_';
				else {}

				buf += one;

				if(one > 255)	count_ascii += 2; 
				else			count_ascii += 1;

				if(ext_flag == false) 
				{
					if(count_ascii == 7) 
					{
						buf += "_.";
						count_ascii += 2;
						ext_flag = true;
					}
					else if(count_ascii == 8) 
					{
						buf += ".";
						count_ascii += 1;
						ext_flag = true;
					}
					else {}
				}

			}

			return buf;
		}

		public static string ConvertTagToFile(string tag)
		{
			int size = tag.Length;
			string buf;
			int size_at_ascii = 0;
			int i;

			for(i = 0; i < size; i++) 
			{
				if(tag[i] > 255) 
					size_at_ascii += 2;
				else
					size_at_ascii += 1;
			}

			if(size_at_ascii <= 10) 
			{	// 열글자 이하일 때는 이전의 방식을 사용한다.
				buf = ConvertTagToFileOld(tag);
				return buf;
			}
	
			buf = "";

			for(i = 0; i < size; i++) 
			{
				if(tag[i] < 32)			buf += "_";
				else if(tag[i] == '/')	buf += "_";
		
				else if(tag[i] == '\\')	buf += "_";
				else if(tag[i] == '*')	buf += "_";
				else if(tag[i] == '+')	buf += "_";
				else if(tag[i] == '|')	buf += "_";
				else if(tag[i] == ':')	buf += "_";
				else if(tag[i] == ';')	buf += "_";
				else if(tag[i] == '"')	buf += "_";
				else if(tag[i] == '<')	buf += "_";
				else if(tag[i] == '>')	buf += "_";
				else if(tag[i] == '?')	buf += "_";
				else buf+=tag[i];
			}

			return buf;
		}

		public static string GetDigitalStatusString(TagDiClass di, sbyte flag)
		{
			if(flag==1)	return GetDesON(di);
			else		return GetDesOFF(di);
		}

		static string[] sTagMember = {  
										 "value", 			
										 "tag", "name",
										 "des", "unit", "desON", "desOFF", 
										 "port",  "station", "address", "extra1", "extra2",
										 "hihi", "high", "low", "lolo", "active",
										 "full", "base", "plcfull", "plcbase", "viewfull", "viewbase",
										 "assign", "NeedAlarmConfirm", 
										 "protectscan", "protectcontrol", "ProtectAlarmEvent", "ProtectAlarmData",
										 "SumTotal", "SumPart", "AlarmLevelStatus", "Format", "alarm"
									 };

		// 태그멤버로 사용하는 문자열인지를 검사한다 (.)은 제외하고

		public static bool IsTagMember(string member)
		{
			for(int i = 0; i < sTagMember.Length; i++) 
			{
				if(String.Compare(member, sTagMember[i], StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					return true;
				}
			}

			return false;
		}

        static void AddTagUsedInformation(MULTI_SELECT_TAG_STRUCT list, string zone, EnumTagUsedType type, object obj, string position)
        {
            /*
            if (list.arrayUsed == null)
            {
                list.arrayUsed = new List<object>();
            }

            TAG_USED_INFOMATION used = new TAG_USED_INFOMATION();

            if (String.Compare(TotalConfig.sDirWorkProject, 0, zone, 0, TotalConfig.sDirWorkProject.Length, StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                used.zone = zone.Substring(TotalConfig.sDirWorkProject.Length + 1);
            }
            else
            {
                used.zone = Path.GetFileName(zone);
            }

            used.type = type;
            used.obj = obj;
            used.position = position;

            list.arrayUsed.Add(used);*/
        }

        public static void AddTagList(List<object> block, string tag, EnumTagType tag_type, string zone, EnumTagUsedType type, object obj, string position)
        {
            if (tag == null) return;
            if (tag.Length == 0) return;

            MULTI_SELECT_TAG_STRUCT list;
            int l;

            for (l = 0; l < block.Count; l++)
            {
                list = (MULTI_SELECT_TAG_STRUCT)block[l];
                if (String.Compare(list.tagSource, tag, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    AddTagUsedInformation(list, zone, type, obj, position);
                    return;	// same tag already exist
                }
            }

            list = new MULTI_SELECT_TAG_STRUCT();
            list.tagSource = tag;
            list.tagTarget = tag;
            list.tag_type = tag_type;
            block.Add(list);

            AddTagUsedInformation(list, zone, type, obj, position);
        }
    }

    public enum EnumTagUsedType
    {
        None,
        GraphicObject,
        String,
    }

    public class TAG_USED_INFOMATION
    {
        public string zone;
        public EnumTagUsedType type;
        public object obj;
        public string position;
    }

    public class MULTI_SELECT_TAG_STRUCT
    {
        public string tagSource;		    //
        public string tagTarget;
        public EnumTagType tag_type;	    // 태그의 종류.
        public List<object> arrayUsed = null;  // 사용된 곳
    }
}


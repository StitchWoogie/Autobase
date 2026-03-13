using System;
using System.Drawing;
using NetTools.OldDefine;
using System.IO;
using NetTools;

namespace AutoLibLocal
{
    public enum EnumAlarmType
    {
        HIHI = 0,
        HIGH = 1,
        LOW = 2,
        LOLO = 3,
        DI_ON = 4,					// 디지털 입력이 ON되었을 때 경보
        DI_OFF = 5,					// 디지털 입력이 OFF되었을 때 경보
        RETURN = 6,					// 경보에서 정상으로 복귀했을 때
        HAND_OPERATION = 7,			// 조작자가 디지털을 수동으로 조작했을 때.
        OVER_RATE_OF_CHANGE_LIMIT = 8,// 변화율 범위 초과.
        HAND_INPUT = 9,				// 수동기입 경보.
        DEMAND_CONTROL = 10,		// Demand Control
        GENERAL = 11,               // 일반 경보
        CNF = 12,                   // 최소값/최대값 확인 2021-2-18 추가
    }
    public enum EnumAlarmSubType
    {
        General_LOGIN = 0,
        General_LOGOUT = 1,
    }

	// total size is 232
	[Serializable]
	public class ALARM_FILE_STRUCT
	{
		public SYSTEMTIME	t = new SYSTEMTIME();	// size 16
		public string 		tag;					// size 40
		public string 		description;			// size 80
		public string 		msg;					// size 80
		public ushort		alarm_type;				
		public ushort		priority;				// 경보우선순위 (0~999)
		public ushort		port;
		public ushort		station;
		public uint			address;
        public ushort alarm_sub_type;
		public ushort		crc;
		public EnumTagType	enumTagType;

        // 아래 user ip computer는 10.2.1 부터 추가됨
        public string user;
        public string ip;
        public string computer;

		public static readonly int			struct_size = 232;	// 9.0이전의 스트럭쳐 크기

		public bool LoadFromFile(BinaryReader reader)
		{
			if(reader.BaseStream.Position+struct_size > reader.BaseStream.Length)	return false;

			System.Text.Decoder d = System.Text.Encoding.Default.GetDecoder();
			
			t.LoadFromFile(reader);
			
			byte[] buf = new byte[10000];
			char[] chars = new char[10000];
			int retn;
			int i;

			buf = reader.ReadBytes(40);
			retn = d.GetChars(buf, 0, buf.Length, chars, 0);
			
			tag = "";
			for(i = 0; i < retn; i++) 
			{
				if(chars[i] == 0)	break;
				tag += Char.ToString(chars[i]);
			}

			buf = reader.ReadBytes(80);
			retn = d.GetChars(buf, 0, buf.Length, chars, 0);
			
			description = "";
			for(i = 0; i < retn; i++) 
			{
				if(chars[i] == 0)	break;
				description += Char.ToString(chars[i]);
			}

			buf = reader.ReadBytes(80);
			retn = d.GetChars(buf, 0, buf.Length, chars, 0);
			
			msg = "";
			for(i = 0; i < retn; i++) 
			{
				if(chars[i] == 0)	break;
				msg += Char.ToString(chars[i]);
			}

			alarm_type = reader.ReadUInt16();
			priority = reader.ReadUInt16();
			port = reader.ReadUInt16();
			station = reader.ReadUInt16();
			address = reader.ReadUInt32();
			alarm_sub_type = reader.ReadUInt16();
			crc = reader.ReadUInt16();

			return true;
		}
	}

	/// <summary>
	/// Summary description for AlarmClass.
	/// </summary>
	public class AlarmClass
	{
		public const int MAX_ALARM_MSG_TYPE = 12;			// 경보 메세지의 유형 개수
		public Color[] colorAlarmText = new Color[MAX_ALARM_MSG_TYPE];
		public Color colorAlarmBack;
		public static readonly string ALARM_FILE_EXT = "ALMX";

		static Color[] defaultAlarmText = new Color[MAX_ALARM_MSG_TYPE] { 
																				   Color.FromArgb(255, 0, 0),
																				   Color.FromArgb(255, 0, 0),
																				   Color.FromArgb(0, 0, 255),
																				   Color.FromArgb(0, 0, 255),
																				   Color.FromArgb(0, 255, 0),
																				   Color.FromArgb(0, 255, 0),
																				   Color.FromArgb(192, 192, 192),
																				   Color.FromArgb(255, 255, 0),
																				   Color.FromArgb(255, 0, 255),
																				   Color.FromArgb(0, 255, 255),
																				   Color.FromArgb(0, 255, 255),
                                                                                   Color.FromArgb(0, 255, 255),
		
		};
		static Color defaultAlarmBack = Color.Black;

        public bool bDisplayColorByPriority = false; // false = By Type, true = By Priority
        public Color[] colorAlarmTextPriority = new Color[1000];

        public void GetDefaultColor(Color[] text_color, Color[] priority_color, out Color back)
        {
            for (int i = 0; i < MAX_ALARM_MSG_TYPE; i++)
            {
                text_color[i] = defaultAlarmText[i];
            }

            for (int i = 0; i < priority_color.Length; i++)
            {
                if (i < MAX_ALARM_MSG_TYPE)
                {
                    priority_color[i] = defaultAlarmText[i];
                }
                else
                {
                    priority_color[i] = Color.FromArgb(192, 192, 192);
                }
            }

            back = defaultAlarmBack;
        }

		public AlarmClass()
		{
			//
			// TODO: Add constructor logic here
			//
            GetDefaultColor(colorAlarmText, colorAlarmTextPriority, out colorAlarmBack);
            /*
			colorAlarmText = new Color[MAX_ALARM_MSG_TYPE];

			for(int i = 0; i < MAX_ALARM_MSG_TYPE; i++) 
			{
				colorAlarmText[i] = defaultAlarmText[i];
			}

            for (int i = 0; i < colorAlarmTextPriority.Length; i++)
            {
                if (i < MAX_ALARM_MSG_TYPE)
                {
                    colorAlarmTextPriority[i] = defaultAlarmText[i];
                }
                else
                {
                    colorAlarmTextPriority[i] = Color.FromArgb(192, 192, 192);
                }
            }

			colorAlarmBack = defaultAlarmBack;*/

			Load();
		}

		void Load()
		{
			string cfg_dir = TotalConfig.AutoBaseIniGetConfigDirectory();
			string filename = String.Format("{0}\\AlarmColor\\AlarmColor.cfg", cfg_dir);

			if(!File.Exists(filename))	return;

			TextReader reader = new StreamReader(filename);

			if(reader == null)	return;

			string one_line;
			CommaBlockString comma = new CommaBlockString();
			string command = "";
			int color = 0;

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				comma.Set(one_line);
				comma.GetString(ref command);

				if(command == "TextColor") 
				{
					int no = 0;
					
					comma.GetInt(ref no);
					if(no >= 0 && no < MAX_ALARM_MSG_TYPE) 
					{
						comma.GetInt(ref color);
						colorAlarmText[no] = Color.FromArgb(color);
					}
				}
				else if(command == "BackColor") 
				{
					comma.GetInt(ref color);
					colorAlarmBack = Color.FromArgb(color);
				}
                else if(command == "DisplayColorMethod") {
                    int type = 0;
                    comma.GetInt(ref type);
                    bDisplayColorByPriority = (type == 1);
                }
                else if (command == "TextColorPriority")
                {
                    int no = 0;

                    comma.GetInt(ref no);
                    if (no >= 0 && no < colorAlarmTextPriority.Length)
                    {
                        comma.GetInt(ref color);
                        colorAlarmTextPriority[no] = Color.FromArgb(color);
                    }
                }
                 
				else {}
			}

			reader.Close();
		}

		public void Save()
		{
			string cfg_dir = TotalConfig.AutoBaseIniGetConfigDirectory();
			string filename = String.Format("{0}\\AlarmColor", cfg_dir);

			Directory.CreateDirectory(filename);

			filename = String.Format("{0}\\AlarmColor\\AlarmColor.cfg", cfg_dir);

			TextWriter writer = new StreamWriter(filename);

			if(writer == null)	return;

            writer.WriteLine("DisplayColorMethod,{0},", bDisplayColorByPriority ? 1 : 0);
            writer.WriteLine("BackColor,{0},", colorAlarmBack.ToArgb());

			for(int i = 0; i < MAX_ALARM_MSG_TYPE; i++) 
			{
				writer.WriteLine("TextColor,{0},{1},", i, colorAlarmText[i].ToArgb());
			}

            for (int i = 0; i < colorAlarmTextPriority.Length; i++)
            {
                writer.WriteLine("TextColorPriority,{0},{1},", i, colorAlarmTextPriority[i].ToArgb());
            }

			writer.Close();
		}

        public static string[] GetAlarmTypeDescription()
        {
            string[] des = new string[MAX_ALARM_MSG_TYPE];

            if (Tools.IsLangKorean())
            {
                des[0] = "HiHi 경보";
                des[1] = "High 경보";
                des[2] = "Low 경보";
                des[3] = "LoLo 경보";
                des[4] = "DI ON 경보";
                des[5] = "DI OFF 경보";
                des[6] = "복귀 경보";
                des[7] = "수동 제어 경보";
                des[8] = "과변화 경보";
                des[9] = "수동 기입 경보";
                des[10] = "디맨드 제어 경보";
                des[11] = "일반 경보";
            }
            else if (Tools.IsLangJapanese())
            {
                des[0] = "HiHi 警報";
                des[1] = "High 警報";
                des[2] = "Low 警報";
                des[3] = "LoLo 警報";
                des[4] = "DI ON 警報";
                des[5] = "DI OFF 警報";
                des[6] = "復帰警報";
                des[7] = "手動制御警報";
                des[8] = "過変化警報";
                des[9] = "手動記入ラーム";
                des[10] = "デマンド制御警報";
                des[11] = "General";
            }
            else if (Tools.IsLangChinese())
            {
                des[0] = "HiHi 警报";
                des[1] = "High 警报";
                des[2] = "Low 警报";
                des[3] = "LoLo 警报";
                des[4] = "DI ON 警报";
                des[5] = "DI OFF 警报";
                des[6] = "回复警报";
                des[7] = "手动操作警报";
                des[8] = "过度变化警报";
                des[9] = "手动输入警报"; 
                des[10] = "需要控制警报";
                des[11] = "General";
            }
            else if (Tools.IsLangVietnamese())
            {
                des[0] = "HiHi";
                des[1] = "High";
                des[2] = "Low";
                des[3] = "LoLo";
                des[4] = "DI ON";
                des[5] = "DI OFF";
                des[6] = "Return";
                des[7] = "Hoạt động thủ công";
                des[8] = "Tỉ lệ thay đổi quá giới hạn";
                des[9] = "Ngõ vào thủ công";
                des[10] = "Điều khiển nhu cầu";
                des[11] = "Chung";
            }
            else
            {
                des[0] = "HiHi";
                des[1] = "High";
                des[2] = "Low";
                des[3] = "LoLo";
                des[4] = "DI ON";
                des[5] = "DI OFF";
                des[6] = "Return";
                des[7] = "Manual Operation";
                des[8] = "OVER_RATE_OF_CHANGE_LIMIT";
                des[9] = "HAND_INPUT";
                des[10] = "DEMAND CONTROL";
                des[11] = "General";
            }

            return des;
        }

        public Color GetAlarmColor(int alarm_type, int priority)
        {
            if (bDisplayColorByPriority)
            {
                if (priority < 0 || priority >= 1000)
                    return Color.Red;
                else
                    return colorAlarmTextPriority[priority];
            }
            else
            {
                if (alarm_type < 0 || alarm_type >= AlarmClass.MAX_ALARM_MSG_TYPE)
                    return Color.Red;
                else
                    return colorAlarmText[alarm_type];
            }
        }

	}
}

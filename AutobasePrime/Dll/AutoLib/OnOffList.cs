using System;
using System.Collections;
using System.IO;
using NetTools;
using AutoLibLocal;

namespace AutoLib
{
	[Serializable]
	public class ON_OFF_LIST_MEMBER 
	{
		public string 	tag;
		public int[]	tag_pos;
		//public int		tag_type;
		public double	curr;		// 아날로그 입력 값일 때 ON 되었을 때 값.
        public string   sCurr;      // 아날로그 입력 값일 때 ON 되었을 때 값.
		public sbyte	bWhen;		// 언제 시점의 값을 취할 것이냐?
	}
	[Serializable]
	public class ON_OFF_LIST
	{
		public string tag;
		public int[] tag_pos;
		//public int   tag_type;
		public sbyte  bOnOff;
		public DateTime stStart;	// 시작 시간.
		public ArrayList blockList;
	}

	public class ON_OFF_LIST_CONFIG 
	{
		public bool	bStartEveryDay;	// 일마다 데이터를 다시 시작한다.
		public bool	bClearOnStart;
		public int	nSaveFileType;	// 0 - 기본자료, 1 - 기본+DB, 2 - DB
		public bool	bSaveTagDescription;
		public bool	bSaveWhenOn;	// 가동시에도 자료를 저장하고 끝날때도 자료를 저장한다.

		public string sDsn;			// DB 저장시 사용할 연결문자열
		public string sTable = "OnOffList";		// DB 저장시 사용할 테이블
	}

	/// <summary>
	/// Summary description for OnOffList.
	/// </summary>
	public class OnOffList
	{
		public static ArrayList blockOnOffList = new ArrayList();
		public static ON_OFF_LIST_CONFIG configOnOffList = new ON_OFF_LIST_CONFIG();

		public OnOffList()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void DigitalOnOffListSave()
		{
			SaveConfigOnOffList();

			string filename;
			TextWriter writer;
			ON_OFF_LIST on_off;
			ON_OFF_LIST_MEMBER item;

			int l, m;

			filename = String.Format("{0}\\tag\\on_off.lstx", TotalConfig.sDirWorkProject);
			writer = new StreamWriter(filename);
			if(writer == null)	return;

			for(l = 0; l < OnOffList.blockOnOffList.Count; l++) 
			{
				on_off = (ON_OFF_LIST)OnOffList.blockOnOffList[l];
				writer.Write("{0},", on_off.tag);
				for(m = 0; m < on_off.blockList.Count; m++) 
				{
					item = (ON_OFF_LIST_MEMBER)on_off.blockList[m];
					writer.Write("{0},{1},", item.tag, item.bWhen);
				}
				writer.WriteLine();
			}

			writer.Close();
		}

		static void SaveConfigOnOffList()
		{
			string filename;
			filename = String.Format("{0}\\tag\\on_off_cfg.txt", TotalConfig.sDirWorkProject);
			TextWriter writer = new StreamWriter(filename);
			if(writer == null)	return;

			writer.WriteLine("bStartEveryDay,{0},", OnOffList.configOnOffList.bStartEveryDay);
			writer.WriteLine("bClearOnStart,{0},", OnOffList.configOnOffList.bClearOnStart);
			writer.WriteLine("nSaveFileType,{0},", OnOffList.configOnOffList.nSaveFileType);
			writer.WriteLine("bSaveTagDescription,{0},", OnOffList.configOnOffList.bSaveTagDescription);
			writer.WriteLine("bSaveWhenOn,{0},", OnOffList.configOnOffList.bSaveWhenOn);
			writer.WriteLine("sDsn,{0},", OnOffList.configOnOffList.sDsn);
			writer.WriteLine("sTable,{0},", OnOffList.configOnOffList.sTable);
			writer.Close();
		}

		public static void DigitalOnOffListLoad()
		{
			blockOnOffList.Clear();
			LoadConfigOnOffList();
	
			string buf = "";
			TextReader reader;
			ON_OFF_LIST on_off;
			ON_OFF_LIST_MEMBER item;
			CommaBlockString comma = new CommaBlockString();

			reader = TotalConfig.OpenOldNew("tag", "on_off.lst", "on_off.lstx");
			if(reader == null)	return;

			TagPublicClass tp;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				if(buf.Length == 0)	continue;

				on_off = new ON_OFF_LIST();

				on_off.stStart = DateTime.Now;
		
				comma.Set(buf);
				comma.GetString(ref on_off.tag);
				on_off.tag = on_off.tag.Trim();

				tp = TagLib.GetStructPublic(on_off.tag, ref on_off.tag_pos);
				if(tp.enumTagType != EnumTagType.DI)	continue;	// not DI tag

				on_off.blockList = new ArrayList();
				while(true) 
				{
					item = new ON_OFF_LIST_MEMBER();
					comma.GetString(ref item.tag);
					item.tag = item.tag.Trim();
					if(item.tag.Length == 0)	break;		// end
					item.curr = 0;

					comma.GetChar(ref item.bWhen);	// 값을 언제 저장할 것인가를 검사한다.
			
					on_off.blockList.Add(item);
				}
				OnOffList.blockOnOffList.Add(on_off);
			}

			reader.Close();

		}

		static void LoadConfigOnOffList()
		{
			string filename;
			filename = String.Format("{0}\\tag\\on_off_cfg.txt", TotalConfig.sDirWorkProject);
			if(!File.Exists(filename))	return;
			TextReader reader = new StreamReader(filename);
			if(reader == null)	return;

			string one_line;
			string command = "";
			CommaBlockString comma = new CommaBlockString();
			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				comma.Set(one_line);
				comma.GetString(ref command);
				if(command == "bStartEveryDay")
					comma.GetBool(ref OnOffList.configOnOffList.bStartEveryDay);
				else if(command == "bClearOnStart")
					comma.GetBool(ref OnOffList.configOnOffList.bClearOnStart);
				else if(command == "nSaveFileType")
					comma.GetInt(ref OnOffList.configOnOffList.nSaveFileType);
				else if(command == "bSaveTagDescription")
					comma.GetBool(ref OnOffList.configOnOffList.bSaveTagDescription);
				else if(command == "bSaveWhenOn")
					comma.GetBool(ref OnOffList.configOnOffList.bSaveWhenOn);
				else if(command == "sDsn")
					comma.GetString(ref OnOffList.configOnOffList.sDsn);
				else if(command == "sTable")
					comma.GetString(ref OnOffList.configOnOffList.sTable);
				else {}
			}
			reader.Close();
		}
	}
}

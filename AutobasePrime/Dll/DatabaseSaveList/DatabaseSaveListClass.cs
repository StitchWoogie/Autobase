using System;
using AutoLibLocal;
using System.Collections;
using NetTools.OldDefine;
using NetTools;
using System.IO;

namespace DatabaseSaveList
{
	public enum EnumSaveType
	{
		Moment,
		Min,
		Max,
		Sum,
		Ave,
		OnTime,
		OffTime,
	}

	[Serializable]
	public class DatabaseMember
	{
		public string tag;
		public string field;
		public EnumSaveType eSaveType;

		// 내부 계산용 변수
		public EnumTagType  tag_type;
		public int[] tag_pos = new int[1];

		public double fCalcSum;
		public double fCalcMin;
		public double fCalcMax;
		public double fCalcOldSum;
		public double nCalcCount;
		public string sValue;
		public double fValue;
		
		public double fOldValue;	// 이전의 값.
		public int    nOnTime;		
		public int	  nOnOffCount;	// On/Off된 횟수
		public int    nLastCalcSec;
		public bool	  bValueReadFlag;	// 실제 태그값은 주기동안 여러번 측정될 수 있으나
										// 한번이라도 실패하면 flag는 OFF 이다.
	}

	[Serializable]
	public class SaveList
	{
        public bool bActive = true;        // 사용 비사용 2015-5-22 추가
		public string title;
		public string con_dsn;
		public string table;
		public int  cycle = 60000;			// 1~1000*60*60*24	// 밀리세크로 계산했을 때 signed로 24일까지는 가능하다.
		public ArrayList arrayMember = new ArrayList();
		public bool bSaveAsNextTime;
		public bool bRelationDuplex = false;
		
		public string sColumnDate = "DataSavedTime";	// 저장시간
		public string sColumnMilli = "MilliSecond";		// 밀리데이터	
		public int nDateType = 0;		// 저장시간 형식
	
		// 내부 계산용 변수.
		public int   remain_millisec;
		public long  old_milli_sec;
		public SYSTEMTIME stOld = new SYSTEMTIME();
		public DateTime dtOld;	// 이것은 초/분/시/일/월/년 이 바뀌 었는가를 검사하기 위해서 존재한다.

		public bool bTableSaveMinute;
		public bool bTableSaveHour;
		public bool bTableSaveDay;
		public bool bTableSaveMonth;
		public bool bTableSaveYear;

		public string sTableSaveMinute;
		public string sTableSaveHour;
		public string sTableSaveDay;
		public string sTableSaveMonth;
		public string sTableSaveYear;

        // 10.3.2 부터 지원
        public bool bUseItemRunTag = false;
        public string sItemRunTag = "";
        public bool bExistItemRunTag;   // 실행중에 계산에 사용한다.
	}

	/// <summary>
	/// Summary description for DatabaseSaveListClass.
	/// </summary>
	public class DatabaseSaveListClass
	{
		public ArrayList arrayDatabaseSaveList = new ArrayList();

        // 10.3.2 부터 지원
        public bool bUseRunTag = false;
        public string sRunTag = "";

		public DatabaseSaveListClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void DatabaseSaveListLoad()
		{
			// TODO: Add extra validation here
			string path;

			string work_dir = TotalConfig.sDirWorkProject;

			path = String.Format("{0}\\Database\\SaveList.lst", work_dir);

			CommaBlockString comma = new CommaBlockString();
			string one_line;
			SaveList list=new SaveList();
			DatabaseMember member;
			bool flag_begin = false;
			string buf="";

			if(!File.Exists(path))	return;

			FileStream fs = File.OpenRead(path);
			
			if(fs == null)	return;

			TextReader reader = new StreamReader(fs);

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;

				if(one_line.Length == 0)	continue;

				comma.Set(one_line);
				comma.GetString(ref buf);
				if(!flag_begin) 
				{
					if(buf == "BEGIN") 
					{
						flag_begin = true;
						list = new SaveList();
					}
                    else if (buf == "RunTag")
                    {
                        bUseRunTag = comma.GetBool();
                        sRunTag = comma.GetString();
                    }
				}
				else 
				{
					if(buf == "END") 
					{
						arrayDatabaseSaveList.Add(list);
						flag_begin = false;
					}
                    else if (buf == "Active")
                    {
                        list.bActive = comma.GetBool();
                    }
					else if(buf == "Title") 
					{
						comma.GetString(ref list.title);
					}
					else if(buf == "Dsn") 
					{
						comma.GetString(ref list.con_dsn);
					}
					else if(buf == "Table") 
					{
						comma.GetString(ref list.table);
					}
					else if(buf == "Cycle") 
					{
						comma.GetInt(ref list.cycle);
					}
					else if(buf == "SaveAsNextTime") 
					{
						comma.GetBool(ref list.bSaveAsNextTime);
					}
					else if(buf == "RelationDuplex") 
					{
						comma.GetBool(ref list.bRelationDuplex);
					}
					else if(buf == "RelationSaveMinute") 
					{
						comma.GetBool(ref list.bTableSaveMinute);
						comma.GetString(ref list.sTableSaveMinute);
					}
					else if(buf == "RelationSaveHour") 
					{
						comma.GetBool(ref list.bTableSaveHour);
						comma.GetString(ref list.sTableSaveHour);
					}
					else if(buf == "RelationSaveDay") 
					{
						comma.GetBool(ref list.bTableSaveDay);
						comma.GetString(ref list.sTableSaveDay);
					}
					else if(buf == "RelationSaveMonth") 
					{
						comma.GetBool(ref list.bTableSaveMonth);
						comma.GetString(ref list.sTableSaveMonth);
					}
					else if(buf == "RelationSaveYear") 
					{
						comma.GetBool(ref list.bTableSaveYear);
						comma.GetString(ref list.sTableSaveYear);
					}
					else if(buf == "sColumnDate") 
					{
						comma.GetString(ref list.sColumnDate);
					}
					else if(buf == "sColumnMilli") 
					{
						comma.GetString(ref list.sColumnMilli);
					}
					else if(buf == "nDateType") 
					{
						comma.GetInt(ref list.nDateType);
					}
					else if(buf == "Member") 
					{
						string imsi = "";

						member = new DatabaseMember();
						comma.GetString(ref member.tag);
						comma.GetString(ref member.field);
						comma.GetString(ref imsi);
						try 
						{
							member.eSaveType = (EnumSaveType)Enum.Parse(typeof(EnumSaveType), imsi);
						}
						catch
						{
							member.eSaveType = EnumSaveType.Moment;
						}

						list.arrayMember.Add(member);
					}
                    else if (buf == "ItemRunTag")
                    {
                        list.bUseItemRunTag = comma.GetBool();
                        list.sItemRunTag = comma.GetString();
                    }
				}
			}

			reader.Close();

			//DatabaseSaveListInit();
		}

		public void DatabaseSaveListSave() 
		{
			// TODO: Add extra validation here
			string path;
			string work_dir = TotalConfig.sDirWorkProject;

			path = String.Format("{0}\\Database", work_dir);

			if(!Directory.Exists(path)) 
			{
				Directory.CreateDirectory(path);
			}

			path = String.Format("{0}\\Database\\SaveList.lst", work_dir);

			Stream fs = File.Open(path, FileMode.Create);
			if(fs == null)	return;
			TextWriter writer = new StreamWriter(fs);

            writer.WriteLine("RunTag,{0},{1},", bUseRunTag, sRunTag);

			int i, j;
			SaveList list;
			DatabaseMember member;

			for(i = 0; i < arrayDatabaseSaveList.Count; i++) 
			{
				list = (SaveList)arrayDatabaseSaveList[i];
				writer.WriteLine("BEGIN,");
                writer.WriteLine("\tActive,{0},", list.bActive);
				writer.WriteLine("\tTitle,{0},", list.title);
				writer.WriteLine("\tDsn,{0},", list.con_dsn);
				writer.WriteLine("\tTable,{0},", list.table);
				writer.WriteLine("\tCycle,{0},", list.cycle);
				writer.WriteLine("\tSaveAsNextTime,{0},", list.bSaveAsNextTime.ToString());
				writer.WriteLine("\tRelationDuplex,{0},", list.bRelationDuplex.ToString());

				writer.WriteLine("\tRelationSaveMinute,{0},{1},", list.bTableSaveMinute.ToString(), list.sTableSaveMinute);
				writer.WriteLine("\tRelationSaveHour,{0},{1},", list.bTableSaveHour.ToString(), list.sTableSaveHour);
				writer.WriteLine("\tRelationSaveDay,{0},{1},", list.bTableSaveDay.ToString(), list.sTableSaveDay);
				writer.WriteLine("\tRelationSaveMonth,{0},{1},", list.bTableSaveMonth.ToString(), list.sTableSaveMonth);
				writer.WriteLine("\tRelationSaveYear,{0},{1},", list.bTableSaveYear.ToString(), list.sTableSaveYear);
				writer.WriteLine("\tsColumnDate,{0},", list.sColumnDate);
				writer.WriteLine("\tsColumnMilli,{0},", list.sColumnMilli);
				writer.WriteLine("\tnDateType,{0},", list.nDateType);

				for(j = 0; j < list.arrayMember.Count; j++) 
				{
					member = (DatabaseMember)list.arrayMember[j];
					writer.WriteLine("\tMember,{0},{1},{2},", member.tag, member.field, member.eSaveType.ToString());
				}

                writer.WriteLine("ItemRunTag,{0},{1},", list.bUseItemRunTag, list.sItemRunTag);

				writer.WriteLine("END,");
			}

			writer.Close();
		}

		public SaveList GetSaveList(string name)
		{
			SaveList save_list;	
			int i;

			for(i = 0; i < arrayDatabaseSaveList.Count; i++) 
			{
				save_list = (SaveList)arrayDatabaseSaveList[i];
				if(save_list.title == name)	return save_list;
			}

			return null;
		}
	}
}

using System;
using AutoLib;
using AutoLibLocal;
using System.Collections;
using System.IO;
using NetTools;
using System.Windows.Forms;
using System.Data;
using System.Threading.Tasks;

namespace LocalMain
{
	/// <summary>
	/// Summary description for OnOffList.
	/// </summary>
	public class CheckEngineOnOffList
	{
		static CheckEngineOnOffList()
		{
			//
			// TODO: Add constructor logic here
			//
			OnOffList.DigitalOnOffListLoad();
		}

		public static void Save(TagDiClass di, bool bOnLoading)
		{
			if(OnOffList.blockOnOffList.Count == 0)	return;	// 저장할 항목이 하나도 없다.

			int l;
			ON_OFF_LIST on_off;

			for(l = 0; l < OnOffList.blockOnOffList.Count; l++) 
			{
				on_off = (ON_OFF_LIST)OnOffList.blockOnOffList[l];
				if(on_off.tag != di.tag) 	continue; // tag mismatched

				if(di.curr == 1) 
				{
					on_off.stStart = DateTime.Now;
					RememberValueAtON(on_off.blockList);
					if(!bOnLoading)
						SaveOneList(on_off, on_off.stStart, on_off.blockList, 1);
				}
				else 
				{
					DateTime st;
					st = DateTime.Now;
					 SaveOneList(on_off, st, on_off.blockList, 0);
				}

				on_off.bOnOff = di.curr;

				break;
			}
		}

		//---------------------------------------------------------------------
		// ON 될 당시의 값을 기억해 둔다.
		//---------------------------------------------------------------------

		static void RememberValueAtON(ArrayList blockList)
		{
			int m;
			ON_OFF_LIST_MEMBER item;
			TagPublicClass tp;

			for(m = 0; m < blockList.Count; m++) 
			{
				item = (ON_OFF_LIST_MEMBER)blockList[m];
				tp = TagLib.GetStructPublic(item.tag, ref item.tag_pos);
				if(tp.enumTagType == EnumTagType.AI) 
				{	// ai
					item.curr = ((TagAiClass)tp).curr;
				}
				else if(tp.enumTagType == EnumTagType.AO) 
				{	// ao
					item.curr = ((TagAoClass)tp).curr;
				}
				else if(tp.enumTagType == EnumTagType.DI) 
				{
					item.curr = ((TagDiClass)tp).curr;
				}
				else if(tp.enumTagType == EnumTagType.DO) 
				{
					item.curr = ((TagDoClass)tp).curr;
				}
                else if (tp.enumTagType == EnumTagType.ST)
                {
                    item.sCurr = ((TagStClass)tp).curr;
                }
				else 
				{
					item.curr = 0;
				}
			}
		}

		static void SaveOneList(ON_OFF_LIST on_off, DateTime stEnd, ArrayList blockList, sbyte bStatusDI)
		{
			if(OnOffList.configOnOffList.bSaveWhenOn) 
			{	// ON일때도 자료저장.
				if(bStatusDI == 1) 
				{	// ON이 되면 일단 데이터를 저장한다.
					if(OnOffList.configOnOffList.nSaveFileType == 1) 
					{
						SaveOneListTextFormat(on_off, stEnd, blockList, 1);
						//SaveOneListDatabaseFormat(on_off, stEnd, blockList, ON);
					}
					else if(OnOffList.configOnOffList.nSaveFileType == 2) 
					{
						//SaveOneListDatabaseFormat(on_off, stEnd, blockList, ON);
					}
					else 
					{
						SaveOneListTextFormat(on_off, stEnd, blockList, 1);
					}
				}
				else 
				{
					if(OnOffList.configOnOffList.nSaveFileType == 1) 
					{
						SaveOneListTextFormatOverwrite(on_off, ref stEnd, blockList);
						stEnd = SaveOneListDatabaseFormat(on_off, stEnd, blockList, 0);
					}
					else if(OnOffList.configOnOffList.nSaveFileType == 2) 
					{
						stEnd = SaveOneListDatabaseFormat(on_off, stEnd, blockList, 0);
					}
					else 
					{
						SaveOneListTextFormatOverwrite(on_off, ref stEnd, blockList);
					}
				}
			}
			else 
			{	// OFF 일때 자료 저장
				if(bStatusDI == 0) 
				{	// OFF 일때만 데이터를 저장하면 된다.
					if(OnOffList.configOnOffList.nSaveFileType == 1) 
					{
						SaveOneListTextFormat(on_off, stEnd, blockList, 0);
						stEnd = SaveOneListDatabaseFormat(on_off, stEnd, blockList, 0);
					}
					else if(OnOffList.configOnOffList.nSaveFileType == 2) 
					{
						stEnd = SaveOneListDatabaseFormat(on_off, stEnd, blockList, 0);
					}
					else 
					{
						SaveOneListTextFormat(on_off, stEnd, blockList, 0);
					}
				}
			}
		}

		static void SaveOneListTextFormat(ON_OFF_LIST on_off, DateTime stEnd, ArrayList blockList, sbyte start_flag)
		{
			string filename;
			string data_dir;

			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
			filename = String.Format("{0}\\database", data_dir);

            try
            {
                Directory.CreateDirectory(filename);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("Can't create folder.\nFolder={0}\nError={1}", filename, exception.Message);
                return;
            }

			filename = String.Format("{0}\\database\\{1:0000}{2:00}.datx", data_dir, stEnd.Year, stEnd.Month);

            TextWriter writer;

            try
            {
                writer = new StreamWriter(filename, true);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("Can't open the file.\nfilename={0}\nError={1}", filename, exception.Message);
                writer = null;
            }

			if(writer == null)	return;

			SaveOneLineTextFormat(writer, on_off, stEnd, blockList, start_flag);
			writer.Close();
		}
		

		static void SaveOneListTextFormatOverwrite(ON_OFF_LIST on_off, ref DateTime stEnd, ArrayList blockList)
		{
			string filename;
			string targetfile;
			string data_dir;
			string buf;
			CommaBlockString comma = new CommaBlockString();

			string tag = "";
			string sDate = "";
			string sTime = "";
			TextReader reader;
			TextWriter writer;

			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			filename = String.Format("{0}\\database\\{1:0000}{2:00}.datx", data_dir, on_off.stStart.Year, on_off.stStart.Month);
			targetfile = String.Format("{0}\\database\\temp.datx", data_dir);

			if(!File.Exists(filename))
			{	// 이전 파일을 찾을 수 없으면 새로운 파일을 만든다.
				SaveOneListTextFormat(on_off, stEnd, blockList, 0);		
				return;
			}
			reader = new StreamReader(filename);

			if(reader == null) 
			{	
				return;
			}

			writer = new StreamWriter(targetfile);

			if(writer == null) 
			{
				reader.Close();
				return;
			}

			string sMakedDate;
			string sMakedTime;

			sMakedTime = String.Format("{0:00}:{1:00}:{2:00}", on_off.stStart.Hour, on_off.stStart.Minute, on_off.stStart.Second);
			sMakedDate = String.Format("{0:0000}-{1:00}-{2:00}", on_off.stStart.Year, on_off.stStart.Month, on_off.stStart.Day);
	
			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				comma.Set(buf);
				comma.GetString(ref tag);
				comma.GetString(ref sTime);
				comma.GetString(ref sDate);
				if(tag == on_off.tag && sTime == sMakedTime && sDate == sMakedDate) 
				{
					SaveOneLineTextFormat(writer, on_off, stEnd, blockList, 0);
				}
				else 
				{
					writer.WriteLine("{0}", buf);
				}
			}

			reader.Close();
			writer.Close();

			File.Copy(targetfile, filename, true);
		}

		static bool bFirstFlagOnDataBase = false;

		static bool MakeTable(CommonDbConnection db, ConnectionString dsn, string table_name)
		{
			CheckTable check = new CheckTable();
			check.AddColumn("TAG", EnumDbDataType.String, 80);
			check.AddColumn("Description", EnumDbDataType.String, 80);
			check.AddColumn("StartTime", EnumDbDataType.DateTime, 0);
			check.AddColumn("EndTime", EnumDbDataType.DateTime, 0);
			check.AddColumn("OperTime", EnumDbDataType.String, 20);

			int l;
			ON_OFF_LIST on_off;
			int member_max = 0;
			string field_name;

			for(l = 0; l < OnOffList.blockOnOffList.Count; l++) 
			{
				on_off = (ON_OFF_LIST)OnOffList.blockOnOffList[l];
				if((int)on_off.blockList.Count > member_max)	member_max = on_off.blockList.Count;
			}

			for(l = 0; (int)l < member_max; l++) 
			{
				field_name = String.Format("Member{0}", l+1);
				check.AddColumn(field_name, EnumDbDataType.String, 40);
			}

            if (!check.Check(db, dsn.dbtype, table_name))
            {
                MessageDisplay.Show("OnOffListMakeTable Error: Table={0}\nMessage={1}", table_name, check.sErrorMessage);
                return false;
            }

            return true;
            
		}

		static DateTime SaveOneListDatabaseFormat(ON_OFF_LIST on_off, DateTime stEnd, ArrayList blockList, sbyte start_flag)
		{
			ConnectionString dsn = DbTool.dsnList.GetConnection(OnOffList.configOnOffList.sDsn);

			if(dsn == null)	return stEnd;	// DSN not found

            CommonDbConnection conn = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);

			try 
			{
				//conn.ConnectionString = dsn.dsn;
				conn.Open();
			}
			catch 
			{
				return stEnd;
			}

			if(conn.State != ConnectionState.Open)	return stEnd;

			if(!bFirstFlagOnDataBase)	// 테이블의 존재 검사와 함께 컬럼도 검사한다.
			{
                bool retn  = MakeTable(conn, dsn, OnOffList.configOnOffList.sTable);

                if (!retn)
                {
                    conn.Close();
                    return stEnd;
                }

                bFirstFlagOnDataBase = true;
			}

			string query;

			query = String.Format("INSERT INTO {0} ({1},{2},{3},{4},{5}", OnOffList.configOnOffList.sTable, DbTool.Field(dsn.dbtype,"TAG"), DbTool.Field(dsn.dbtype,"Description"), DbTool.Field(dsn.dbtype,"StartTime"), DbTool.Field(dsn.dbtype,"EndTime"), DbTool.Field(dsn.dbtype,"OperTime"));

			for(int l = 0, pos=1; l < on_off.blockList.Count; l++, pos++) 
			{
				string column = String.Format("Member{0}", pos);
				query += String.Format(",{0}", DbTool.Field(dsn.dbtype, column));
			}

			query += ") VALUES (";
			
			query += String.Format("'{0}',", on_off.tag);
			if(OnOffList.configOnOffList.bSaveTagDescription) 
			{
				TagDiClass di = TagLib.GetStructDI(on_off.tag, ref on_off.tag_pos);
				query += String.Format("'{0}',", di.description);
			}
			else
				query += String.Format("'',");

			query += String.Format("{0},", DbTool.MakeDateTimeString(dsn.dbtype, on_off.stStart)); 
			query += String.Format("{0},", DbTool.MakeDateTimeString(dsn.dbtype, stEnd)); 

			long curr_sec = TimeUtil.GetMinHap(stEnd)*60+stEnd.Second;
			long old_sec =  TimeUtil.GetMinHap(on_off.stStart)*60+on_off.stStart.Second;

            curr_sec = curr_sec - old_sec;  // 9.3.6 부터 +1을 하다가 10.2.1 부터는 선택적으로 할 수 있게 수정하였다.
            if (ConfigViewMain.bOnOffListAddOneSecondToOperationTime)
                curr_sec++;

			long hour = (curr_sec/3600);
			long min  = (curr_sec%3600)/60;
			long sec  = (curr_sec)%60;
			string imsi;

			imsi = String.Format("{0}:{1:00}:{2:00}", hour, min, sec);

			query += String.Format("'{0}'", imsi);
	
			int m;
			ON_OFF_LIST_MEMBER item;
			string data_result;
			TagPublicClass tp;

			for(m = 0; m < blockList.Count; m++) 
			{
				item = (ON_OFF_LIST_MEMBER)blockList[m];

				tp = TagLib.GetStructPublic(item.tag, ref item.tag_pos);
		
				if(tp.enumTagType == EnumTagType.AI) 
				{	// ai
					TagAiClass ai = (TagAiClass)tp;
					if(item.bWhen == 1) 
					{
						imsi = TagUtil.AiValueToStringOnlyPoint(ai, item.curr);
					}
					else 
					{
						imsi = TagUtil.AiValueToStringOnlyPoint(ai, ai.curr);
					}
					data_result = imsi;
				}
				else if(tp.enumTagType == EnumTagType.AO) 
				{	// ao
					TagAoClass ao = (TagAoClass)tp;
					if(item.bWhen == 1) 
					{
						imsi = String.Format("{0:F2}", item.curr);
					}
					else 
					{
						imsi = String.Format("{0:F2}", ao.curr);
					}
					data_result = imsi;
				}
				else if(tp.enumTagType == EnumTagType.DI) 
				{
					TagDiClass di = (TagDiClass)tp;
			
					sbyte flag;

					if(item.bWhen == 1)		flag = (sbyte)item.curr;
					else					flag = di.curr;

					if(flag == 1) 
					{
						if(di.desON.Length > 0)
							data_result = String.Format("{0}", di.desON);
						else
							data_result = String.Format("{0}", flag);
					}
					else 
					{
						if(di.desOFF.Length > 0)
							data_result = String.Format("{0}", di.desOFF);
						else
							data_result = String.Format("{0}", flag);
					}
				}
				else if(tp.enumTagType == EnumTagType.DO) 
				{	// DO
					TagDoClass dout = (TagDoClass)tp;
			
					sbyte flag;

					if(item.bWhen == 1)		flag = (sbyte)item.curr;
					else					flag = dout.curr;

					if(flag == 1) 
					{
						if(dout.desON.Length > 0)
							data_result = String.Format("{0}", dout.desON);
						else
							data_result = String.Format("{0}", flag);
					}
					else 
					{
						if(dout.desOFF.Length > 0)
							data_result = String.Format("{0}", dout.desOFF);
						else
							data_result = String.Format("{0}", flag);
					}
				}
				else if(tp.enumTagType == EnumTagType.ST) 
				{
					TagStClass st = (TagStClass)tp;

                    if (item.bWhen == 1)
                    {
                        data_result = item.sCurr;
                    }
                    else
                    {
                        data_result = st.curr;
                    }
				}
				else 
				{
					data_result = "?";
				}

				query += String.Format(",'{0}'", data_result);
			}

			query += ")";

			CommonDbCommand command = new CommonDbCommand(dsn.dbConnectionType);
			command.Connection = conn;
			command.CommandText = query;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("OnOffList Save Error Dsn={0}\nMessage={1}\nquery={2}", dsn.title, exception.Message, query);
            }

			conn.Close();
            return stEnd; 
        }

		static void SaveOneLineTextFormat(TextWriter writer, ON_OFF_LIST on_off, DateTime stEnd, ArrayList blockList, sbyte start_flag)
		{
			writer.Write("{0},", on_off.tag);
			writer.Write("{0:00}:{1:00}:{2:00},", on_off.stStart.Hour, on_off.stStart.Minute, on_off.stStart.Second);
			writer.Write("{0:0000}-{1:00}-{2:00},", on_off.stStart.Year, on_off.stStart.Month, on_off.stStart.Day);
			if(start_flag == 1) 
			{
				writer.Write("**:**:**,");
				writer.Write("****-**-**,");
			}
			else 
			{
				writer.Write("{0:00}:{1:00}:{2:00},", stEnd.Hour, stEnd.Minute, stEnd.Second);
				writer.Write("{0:0000}-{1:00}-{2:00},", stEnd.Year, stEnd.Month, stEnd.Day);
			}

			long curr_sec = TimeUtil.GetMinHap(stEnd)*60+stEnd.Second;
			long old_sec =  TimeUtil.GetMinHap(on_off.stStart)*60+on_off.stStart.Second;

            curr_sec = curr_sec - old_sec;  // 9.3.6 부터 +1을 하다가 10.2.1 부터는 선택적으로 할 수 있게 수정하였다.
            if (ConfigViewMain.bOnOffListAddOneSecondToOperationTime)
                curr_sec++;

			long hour = (curr_sec/3600);
			long min  = (curr_sec%3600)/60;
			long sec  = curr_sec%60;

			if(start_flag == 1) 
			{
				writer.Write("***:**:**,");
			}
			else 
			{
				writer.Write("{0:000}:{1:00}:{2:00},", hour, min, sec);
			}

			int m;
			ON_OFF_LIST_MEMBER item;
			TagPublicClass tp;

			for(m = 0; m < blockList.Count; m++) 
			{
				item = (ON_OFF_LIST_MEMBER)blockList[m];
				tp = TagLib.GetStructPublic(item.tag, ref item.tag_pos);
		
				if(tp.enumTagType == EnumTagType.AI) 
				{	// ai
					string imsi;
					TagAiClass ai = (TagAiClass)tp;
					if(item.bWhen == 1) 
					{
						imsi = TagUtil.AiValueToString(ai, item.curr);
					}
					else 
					{
						imsi = TagUtil.AiValueToString(ai, ai.curr);
					}
					writer.Write("{0},", imsi);
				}
				else if(tp.enumTagType == EnumTagType.AO) 
				{	// ao
					string imsi;
					TagAoClass ao = (TagAoClass)tp;
					if(item.bWhen == 1) 
					{
						imsi = String.Format("{0}", item.curr);
					}
					else 
					{
						imsi = String.Format("{0}", ao.curr);
					}
					writer.Write("{0},", imsi);
				}
				else if(tp.enumTagType == EnumTagType.DI) 
				{
					TagDiClass di = (TagDiClass)tp;
			
					sbyte flag;

					if(item.bWhen == 1)	flag = (sbyte)item.curr;
					else					flag = (sbyte)di.curr;

					if(flag == 1) 
					{
						if(di.desON.Length > 0)
							writer.Write("{0},", di.desON);
						else
							writer.Write("{0},", flag);
					}
					else 
					{
						if(di.desOFF.Length > 0)
							writer.Write("{0},", di.desOFF);
						else
							writer.Write("{0},", flag);
					}
				}
				else if(tp.enumTagType == EnumTagType.DO) 
				{	// DO
					TagDoClass dout = (TagDoClass)tp;
			
					sbyte flag;

					if(item.bWhen == 1)	flag = (sbyte)item.curr;
					else					flag = (sbyte)dout.curr;

					if(flag == 1) 
					{
						if(dout.desON.Length > 0)
							writer.Write("{0},", dout.desON);
						else
							writer.Write("{0},", flag);
					}
					else 
					{
						if(dout.desOFF.Length > 0)
							writer.Write("{0},", dout.desOFF);
						else
							writer.Write("{0},", flag);
					}
				}
				else if(tp.enumTagType == EnumTagType.ST) 
				{
					TagStClass st = (TagStClass)tp;

                    if (item.bWhen == 1)
                    {
                        writer.Write("{0},", item.sCurr);
                    }
                    else
                    {
                        writer.Write("{0},", st.curr);
                    }
				}
				else 
				{
					writer.Write("?,");
				}
			}

			writer.WriteLine();
		}

		//---------------------------------------------------------------------------------------
		//	bStartEveryDay가 ON 되어 있으면 모든 자료를 저장한 후 다음날에는 다시 시작한다. 
		//---------------------------------------------------------------------------------------

		public static void DigitalOnOffListSaveOnDayChanged(DateTime d)
		{
			if(OnOffList.configOnOffList.bStartEveryDay == false)	return;
			if(OnOffList.blockOnOffList.Count == 0)			        return;	// 저장할 항목이 하나도 없다.

			int l;
			ON_OFF_LIST on_off;
			DateTime t;

			t = new DateTime(d.Year, d.Month, d.Day, 23, 59, 59);
			
			for(l = 0; l < OnOffList.blockOnOffList.Count; l++) 
			{
				on_off = (ON_OFF_LIST)OnOffList.blockOnOffList[l];
				if(on_off.bOnOff == 1) 
				{
					SaveOneList(on_off, t, on_off.blockList, 0);

					on_off.stStart = DateTime.Now;
					on_off.stStart = new DateTime(on_off.stStart.Year, on_off.stStart.Month, on_off.stStart.Day, 0, 0, 0);

					RememberValueAtON(on_off.blockList);
					SaveOneList(on_off, on_off.stStart, on_off.blockList, 1);
				}
			}
		}

		public static void LoadOnOffListDigitalStatus()
		{
			// 초기에는 아래 부분이 없었으나 Di 태그의 현재값을 저장하는 부분이 추가된 뒤
			// 초기값이 ON이 될 경우가 있으므로 List값을 프로그램 시작할 당시의 시간을 기억해 둔다.

			int l;
			TagDiClass di;
			ON_OFF_LIST on_off;
			TagPublicClass tp;

			for(l = 0; l < OnOffList.blockOnOffList.Count; l++) 
			{
				on_off = (ON_OFF_LIST)OnOffList.blockOnOffList[l];

				tp = TagLib.GetStructPublic(on_off.tag, ref on_off.tag_pos);
				if(tp.enumTagType != EnumTagType.DI)	continue;	// not DI tag

				di = (TagDiClass)tp;

				if(di.curr == 1) 
				{
					Save(di, true);
				}
			}


			if(OnOffList.configOnOffList.bClearOnStart)		return;
	
			string filename;
			string cfg_dir;
	
			string buf;
			TextReader reader;
			string tag = "";
			
			CommaBlockString comma = new CommaBlockString();

			cfg_dir = Application.UserAppDataPath;

			filename = String.Format("{0}\\OnOffListStatus.dat", cfg_dir);

			if(!File.Exists(filename))	return;

			reader = new StreamReader(filename);
			if(reader == null)	return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)		break;
				if(buf.Length == 0)	continue;

				comma.Set(buf);
				comma.GetString(ref tag);

				for(l = 0; l < OnOffList.blockOnOffList.Count; l++) 
				{
					on_off = (ON_OFF_LIST)OnOffList.blockOnOffList[l];

					if(tag == on_off.tag) 
					{
						on_off.stStart = comma.GetDateTime();
						break;
					}
				}
			}
			reader.Close();
		}

		public static void SaveOnOffListDigitalStatus()
		{
			string filename;
			string cfg_dir;

			TextWriter writer;
			ON_OFF_LIST on_off;
			int l;

			cfg_dir = Application.UserAppDataPath;

			filename = String.Format("{0}\\OnOffListStatus.dat", cfg_dir);

			writer = new StreamWriter(filename);
			if(writer == null)	return;

			for(l = 0; l < OnOffList.blockOnOffList.Count; l++) 
			{
				on_off = (ON_OFF_LIST)OnOffList.blockOnOffList[l];

				if(on_off.bOnOff == 1) 
				{
					writer.Write("{0},", on_off.tag);
					writer.Write("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00},",
								on_off.stStart.Year, on_off.stStart.Month, on_off.stStart.Day,
								on_off.stStart.Hour, on_off.stStart.Minute, on_off.stStart.Second);
					writer.WriteLine();
				}
			}
			writer.Close();
		}
	}
}




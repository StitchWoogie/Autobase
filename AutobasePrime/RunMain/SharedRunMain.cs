using System;
using System.Collections;
using System.IO;
using NetTools;
using AutoLibLocal;
using AutoLib;
using NetTools.OldDefine;
using System.Data;
using System.Windows.Forms;
using DatabaseConnection;
using DatabaseSaveList;
using DialogTag;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RunMain
{
	/// <summary>
	/// Summary description for SharedRunMain.
	/// </summary>
	public class SharedRunMain 
	{
		public DatabaseSaveListClass classSaveList = new DatabaseSaveList.DatabaseSaveListClass();
		public ConnectionStringList dsnList = new ConnectionStringList();
		
		public static bool bDebugMode = TotalConfig.GetAutoBaseTestMode();
		public static bool bScanPauseFlag = false;

		public SharedRunMain()
		{
			//
			// TODO: Add constructor logic here
			//
			dsnList.ConnectionStringLoad();
			classSaveList.DatabaseSaveListLoad();
			DatabaseSaveListInit();
		}

		void FitTimeWithCycle(SaveList list, SYSTEMTIME st)
		{
			int day_millisec = st.wHour*60*60*1000+st.wMinute*60*1000+st.wSecond*1000+st.wMilliseconds;

			// millisec check
			if(list.cycle != 0) 
			{
				day_millisec = (day_millisec/list.cycle)*list.cycle;
				st.wHour = (ushort)(day_millisec/(60*60*1000));
				day_millisec %= (60*60*1000);
				st.wMinute = (ushort)(day_millisec/(60*1000));
				day_millisec %= (60*1000);
				st.wSecond = (ushort)(day_millisec/(1000));
				day_millisec %= 1000;
				st.wMilliseconds = (ushort)day_millisec;
			}
		}

		string MakeCommandColumnAdd(ConnectionString dsn, string tablename, DatabaseMember member)
		{
			string var_type;

			TagLib.GetTagTypeAndPos(member.tag, ref member.tag_type, ref member.tag_pos);

			if(member.tag_type == EnumTagType.AI)
				var_type = "FLOAT";
			else if(member.tag_type == EnumTagType.AO)
				var_type = "FLOAT";
			else if(member.tag_type == EnumTagType.DI)
				var_type = "integer";
			else if(member.tag_type == EnumTagType.DO)
				var_type = "integer";
			else if(member.tag_type == EnumTagType.ST) 
			{
                var_type = DbTool.MakeVarType(dsn.dbtype, EnumDbDataType.String, 255);
			}
			else 
			{
                var_type = DbTool.MakeVarType(dsn.dbtype, EnumDbDataType.String, 255);
			}

			string command;

            if (dsn.dbtype == EnumDbType.Oracle || dsn.dbtype == EnumDbType.Tibero)
				command = String.Format("ALTER TABLE {0} ADD (\"{1}\" {2})", tablename, member.field, var_type);
			else if(dsn.dbtype == EnumDbType.MySQL)
				command = String.Format("ALTER TABLE {0} ADD `{1}` {2}", tablename, member.field, var_type);
            else if (dsn.dbtype == EnumDbType.DB2)
                command = String.Format("ALTER TABLE {0} ADD COLUMN \"{1}\" {2}", tablename, member.field, var_type);
            else if (dsn.dbtype == EnumDbType.SQLServerCE)
                command = String.Format("ALTER TABLE {0} ADD COLUMN \"{1}\" {2}", tablename, member.field, var_type);
			else
				command = String.Format("ALTER TABLE {0} ADD {1} {2}", tablename, DbTool.Field(dsn.dbtype, member.field), var_type);

			return command;
		}

		string MakeAddDateColumnCommand(SaveList list, ConnectionString dsn, string table_name)
		{
			string command;

			if(list.nDateType == 1)		// string 19991231235959 형식
			{
                if (dsn.dbtype == EnumDbType.Oracle || dsn.dbtype == EnumDbType.Tibero)
					command = String.Format("ALTER TABLE {0} ADD ({1} VARCHAR2(14))", table_name, list.sColumnDate);
                else if (dsn.dbtype == EnumDbType.DB2)
                    command = String.Format("ALTER TABLE {0} ADD COLUMN {1} VARCHAR(14)", table_name, list.sColumnDate);
                else if (dsn.dbtype == EnumDbType.SQLServerCE)
                    command = String.Format("ALTER TABLE {0} ADD COLUMN {1} nvarchar(14)", table_name, list.sColumnDate);
				else
					command = String.Format("ALTER TABLE {0} ADD {1} VARCHAR(14)", table_name, list.sColumnDate);
			}
			else 
			{
                if (dsn.dbtype == EnumDbType.Oracle || dsn.dbtype == EnumDbType.Tibero)
					command = String.Format("ALTER TABLE {0} ADD ({1} DATE)", table_name, list.sColumnDate);
                else if (dsn.dbtype == EnumDbType.DB2)
                    command = String.Format("ALTER TABLE {0} ADD COLUMN {1} TIMESTAMP", table_name, list.sColumnDate);
                else if (dsn.dbtype == EnumDbType.SQLServerCE)
                    command = String.Format("ALTER TABLE {0} ADD COLUMN {1} DATETIME", table_name, list.sColumnDate);
				else
                    command = String.Format("ALTER TABLE {0} ADD {1} DATETIME", table_name, list.sColumnDate);
			}

			return command;
		}

        void MakeTable(SaveList list, CommonDbConnection db, ConnectionString dsn, string table_name)
		{
            string query;

			CommonDbDataAdapter ad;
			
            /*
			if(dsn.dbtype == EnumDbType.Oracle)
				ad = new CommonDbDataAdapter("SELECT * FROM "+table_name+" WHERE rownum=1", db);
			else
				ad = new CommonDbDataAdapter("SELECT * FROM "+table_name, db);*/

            // Oracle 말고는 모든 데이터베이스가 많이 쌓일수록 늦어지는 현상이 발생했다. 모든 데이터를 SELECT해서 그렇다. 2015-5-27 수정
            if (dsn.dbtype == EnumDbType.Oracle || dsn.dbtype == EnumDbType.Tibero)
                query = String.Format("SELECT * FROM {0} WHERE rownum=1", table_name);
            else if (dsn.dbtype == EnumDbType.SQLServerCE)
                query = String.Format("SELECT TOP(1) * FROM {0}", table_name);
            else if (dsn.dbtype == EnumDbType.MySQL)
                query = String.Format("SELECT * FROM {0} LIMIT 1", table_name); // MySQL은 TOP이 다르다.
            else
                query = String.Format("SELECT TOP 1 * FROM {0}", table_name);

            ad = new CommonDbDataAdapter(query, db);

			DataSet ds = new DataSet();
			try 
			{
				ad.Fill(ds, table_name);
			}
			catch (Exception exception)
			{
                // 테이블이 존재하지 않는경우 오류메시지가 나는데 안보이는것이 좋다.
                string msg = exception.Message;
                //MessageBox.Show(exception.Message, "MakeTable Error:table=" + table_name);	
			}

			CommonDbCommand command = new CommonDbCommand(dsn.dbConnectionType);
			DatabaseSaveList.DatabaseMember member;

            CommonDbTransaction transaction = new CommonDbTransaction();
            if (db.bAddCommitAfterCommand)
            {
                transaction.BeginTransaction(db);
                command.Transaction = transaction;
            }
			
			if(ds.Tables.Count == 0)	// no table
			{
				command.Connection = db;
				command.CommandText = String.Format("CREATE TABLE {0} ({1} Integer)", table_name, list.sColumnMilli);
				try 
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Create Table Error:table="+table_name);
				}

				command.CommandText = MakeAddDateColumnCommand(list, dsn, table_name);

				try 
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Alter Table Error:Column=Date");
				}

				for(int j = 0; j < list.arrayMember.Count; j++) 
				{
					member = (DatabaseSaveList.DatabaseMember)list.arrayMember[j];

					command.CommandText = MakeCommandColumnAdd(dsn, table_name, member);

					try 
					{
						command.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Column Add Error:Field="+member.field);
					}
				}

			}
			else 
			{
				command.Connection = db;

				if(ds.Tables[0].Columns.IndexOf(list.sColumnMilli) == -1) 
				{
                    if (dsn.dbtype == EnumDbType.Oracle || dsn.dbtype == EnumDbType.Tibero)
						command.CommandText = String.Format("ALTER TABLE {0} ADD ({1} Integer)", table_name, list.sColumnMilli);
                    else if (dsn.dbtype == EnumDbType.DB2)
                        command.CommandText = String.Format("ALTER TABLE {0} ADD COLUMN {1} Integer", table_name, list.sColumnMilli);
                    else if (dsn.dbtype == EnumDbType.SQLServerCE)
                        command.CommandText = String.Format("ALTER TABLE {0} ADD COLUMN {1} Integer", table_name, list.sColumnMilli);
					else
						command.CommandText = String.Format("ALTER TABLE {0} ADD {1} Integer", table_name, DbTool.Field(dsn.dbtype, list.sColumnMilli));

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Alter Table Error:Column=" + list.sColumnMilli);
                    }
				}
				if(ds.Tables[0].Columns.IndexOf(list.sColumnDate) == -1) 
				{
					command.CommandText = MakeAddDateColumnCommand(list, dsn, table_name);

					command.ExecuteNonQuery();
				}

				for(int j = 0; j < list.arrayMember.Count; j++) 
				{
					member = (DatabaseSaveList.DatabaseMember)list.arrayMember[j];

					if(ds.Tables[0].Columns.IndexOf(DbTool.ChangeFieldChar(dsn.dbtype, member.field)) != -1) continue; // already exist

					command.CommandText = MakeCommandColumnAdd(dsn, table_name, member);

					try 
					{
						command.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Column Add Error:Field="+member.field);
					}
				}
			}

            if (db.bAddCommitAfterCommand)
            {
                transaction.Commit();
            }
		}


        // 프로그램 중간에 변수의 초기화가 필요할 때 사용한다. RunFlag가 Off되었다가 On이 되었을 때 초기화를 할 필요가 있다. 2019-6-7 함수로 뺌
        void InitOneSaveListVars(SaveList list)
        {
            int j;
            DatabaseSaveList.DatabaseMember member;
            string sval;

            list.stOld.GetLocalTime();
            list.dtOld = list.stOld.ToDateTime();
            long curr_milli = list.stOld.GetMilliSecHap();
            FitTimeWithCycle(list, list.stOld);
            long old_milli = list.stOld.GetMilliSecHap();
            list.remain_millisec = (int)(curr_milli - old_milli);
            list.old_milli_sec = curr_milli;

            for (j = 0; j < list.arrayMember.Count; j++)
            {
                member = (DatabaseSaveList.DatabaseMember)list.arrayMember[j];
                TagLib.GetTagTypeAndPos(member.tag, ref member.tag_type, ref member.tag_pos);
                MakeStringByTag(out sval, member);
                member.nCalcCount = 0;
                member.nOnTime = 0;
                member.nOnOffCount = 0;
                try
                {
                    member.fCalcOldSum = ConvertTool.ToDouble(sval);
                }
                catch
                {
                    member.fCalcOldSum = 0;
                }
                member.bValueReadFlag = true;
            }
        }

		public void DatabaseSaveListInit()
		{
			int i;
			SaveList list;
			//DatabaseSaveList.DatabaseMember member = new DatabaseSaveList.DatabaseMember();
			//string sval;
			
			// 먼저 테이블을 만든다. 테이블 만드는 시간이 오래걸리면 전체시간이 Delay된다.
			CommonDbConnection db;
			ConnectionString dsn;
			
			for(i = 0; i < classSaveList.arrayDatabaseSaveList.Count; i++) 
			{
				list = (SaveList)classSaveList.arrayDatabaseSaveList[i];

				dsn = dsnList.GetConnection(list.con_dsn);
				if(dsn == null)	
				{
					if(Tools.IsLangKorean()) 
						MessageBox.Show("저장목록=["+list.title+"]에서\nDSN="+list.con_dsn, "DSN이 존재하지 않습니다.");
					else
						MessageBox.Show("At Save List=["+list.title+"]\nDSN="+list.con_dsn, "DSN not exist.");

					continue;	// DSN not found
				}
			 
				try 
				{
                    db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
					//db.ConnectionString = dsn.dsn;
				}
				catch (Exception exception)
				{
					if(Tools.IsLangKorean())
					{
						string msg = String.Format("연결문자열 제목={0}\n연결문자열={1}\n오류내용={2}", dsn.title, dsn.dsn, exception.Message);
						MessageBox.Show(msg, "연결문자열 형식 오류");
					}
					else 
					{
						string msg = String.Format("DSN title={0}\nDSN={1}\nError Message={2}", dsn.title, dsn.dsn, exception.Message);
						MessageBox.Show(msg, "DSN Format Error");
					}

					continue;
				}

				try 
				{
					db.Open();
				}
				catch (Exception exception)
				{
					MessageDisplay.Show("Connection Open\n"+dsn.dsn+"\n"+exception.Message);
				}
				if(db.State != ConnectionState.Open) continue;

				MakeTable(list, db, dsn, list.table);

				if(list.bTableSaveMinute)
                    MakeTable(list, db, dsn, list.sTableSaveMinute);
				if(list.bTableSaveHour)
                    MakeTable(list, db, dsn, list.sTableSaveHour);
				if(list.bTableSaveDay)
                    MakeTable(list, db, dsn, list.sTableSaveDay);
				if(list.bTableSaveMonth)
                    MakeTable(list, db, dsn, list.sTableSaveMonth);
				if(list.bTableSaveYear)
                    MakeTable(list, db, dsn, list.sTableSaveYear);

				db.Close();

                if (list.bUseItemRunTag)
                {
                    list.bExistItemRunTag = TagLib.IsTagExist(list.sItemRunTag);

                    if (!list.bExistItemRunTag)
                    {
                        MessageBox.Show("Tag = " + list.sItemRunTag, "Run Tag not found. (" + list.title + ")");
                    }
                }
                else
                {
                    list.bExistItemRunTag = false;
                }
			}

			for(i = 0; i < classSaveList.arrayDatabaseSaveList.Count; i++) 
			{
				list = (SaveList)classSaveList.arrayDatabaseSaveList[i];

                InitOneSaveListVars(list);

                //list.stOld.GetLocalTime();
                //list.dtOld = list.stOld.ToDateTime();
                //long curr_milli = list.stOld.GetMilliSecHap();
                //FitTimeWithCycle(list, list.stOld);
                //long old_milli = list.stOld.GetMilliSecHap();
                //list.remain_millisec = (int)(curr_milli-old_milli);
                //list.old_milli_sec = curr_milli;

                //for(j = 0; j < list.arrayMember.Count; j++) 
                //{
                //    member = (DatabaseSaveList.DatabaseMember)list.arrayMember[j];
                //    TagLib.GetTagTypeAndPos(member.tag, ref member.tag_type, ref member.tag_pos);
                //    MakeStringByTag(out sval, member);
                //    member.nCalcCount = 0;
                //    member.nOnTime = 0;
                //    member.nOnOffCount = 0;
                //    try 
                //    {
                //        member.fCalcOldSum = ConvertTool.ToDouble(sval);
                //    }
                //    catch 
                //    {
                //        member.fCalcOldSum = 0;
                //    }
                //    member.bValueReadFlag = true;
                //}
			}

            if (classSaveList.bUseRunTag)
            {
                bExistRunTag = TagLib.IsTagExist(classSaveList.sRunTag);

                if (!bExistRunTag)
                {
                    MessageBox.Show("Tag = " + classSaveList.sRunTag, "Run Tag not found.");
                }
            }
            else
            {
                bExistRunTag = false;
            }

		}

        bool bExistRunTag = false;

		void MakeStringByTag(out string buf, DatabaseMember member)
		{
			string val="";

			if(SharedTag.GetCurr(member.tag, ref val))
				buf = val;
			else 
			{
				member.bValueReadFlag = false;
				buf = "0";
			}
		}

		string MakeDateString(ConnectionString dsn, SaveList list, int year, int mon, int day, int hour, int min, int sec)
		{
			string command;
			if(list.nDateType == 1) 
			{
				command = String.Format("'{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}'", year, mon, day, hour, min, sec);
			}
			else 
			{
				command = DbTool.MakeDateTimeString(dsn.dbtype, year, mon, day, hour, min, sec);
			}

			return command;
		}

		string MakeInsertUpdateCommand(ConnectionString dsn, SaveList list, SYSTEMTIME st, bool bUpdate)
		{
			string command;
			string imsi;
			int i;
			DatabaseSaveList.DatabaseMember member;

			if(bUpdate == false) 
			{
				command = String.Format("INSERT INTO {0} ({1},{2}", list.table, list.sColumnDate, list.sColumnMilli);
			}
			else 
			{
				command = String.Format("UPDATE {0} ({1},{2}", list.table, list.sColumnDate, list.sColumnMilli);
			}

			for(i = 0; i < list.arrayMember.Count; i++) 
			{
				member = (DatabaseSaveList.DatabaseMember)list.arrayMember[i];

				command += ",";
				command += DbTool.Field(dsn.dbtype, member.field);
			}

			command += ") VALUES(";

			imsi = MakeDateString(dsn, list, st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond);
			command += imsi;

			imsi = String.Format(",{0}", st.wMilliseconds);
			command += imsi;

			for(i = 0; i < list.arrayMember.Count; i++) 
			{
				member = (DatabaseSaveList.DatabaseMember)list.arrayMember[i];

				command += ",";

				if(imsi.Length == 0)
					command += "NULL";
				else 
				{
					command += "'";
					if(member.eSaveType == EnumSaveType.Max) 
						command += member.fCalcMax.ToString();
					else if(member.eSaveType == EnumSaveType.Min) 
						command += member.fCalcMin.ToString();
					else if(member.eSaveType == EnumSaveType.Ave) 
					{
						if(member.nCalcCount == 0) 
							command += member.sValue;
						else 
						{
							command += ((double)(member.fCalcSum/member.nCalcCount)).ToString();
						}
					}
					else if(member.eSaveType == EnumSaveType.Sum) 
					{
						double gab;

						TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
						
						if(member.fValue < member.fCalcOldSum) 
						{
							if(member.tag_type == EnumTagType.AI) 
							{
								gab = member.fValue+ai.fFull-member.fCalcOldSum;
							}
							else 
							{
								gab = 0;
							}
						}
						else 
							gab = member.fValue-member.fCalcOldSum;

						// 지정한 범위보다 크거나 작은 값은 측정 불가
						if(member.fValue < ai.fBase)		gab = 0;
						if(member.fValue > ai.fFull)		gab = 0;
						if(member.fCalcOldSum < ai.fBase)	gab = 0;
						if(member.fCalcOldSum > ai.fFull)	gab = 0;

						if(gab > ai.fFull*0.3)	gab = 0;

						member.fCalcOldSum = member.fValue;

						command += gab.ToString();
					}
					else if(member.eSaveType == EnumSaveType.OnTime) 
					{
						// OnOffCount 가 0이라는 것은 디지털이 저장주기 동안 같은 값이라는 이야기이다.
						if(member.nOnOffCount == 0) 
						{
							if(member.fOldValue == 1) 
								command += String.Format("{0}", list.cycle/1000);								
							else
								command += "0";
						}
						else 
						{
							int time = member.nOnTime;

							if(time > (list.cycle/1000))
								command += String.Format("{0}", list.cycle/1000);
							else
								command += member.nOnTime.ToString();
						}
					}
					else if(member.eSaveType == EnumSaveType.OffTime) 
					{
						// OnOffCount 가 0이라는 것은 디지털이 저장주기 동안 같은 값이라는 이야기이다.
						if(member.nOnOffCount == 0) 
						{
							if(member.fOldValue == 1) 
								command += "0";
							else
								command += String.Format("{0}", list.cycle/1000);
						}
						else 
						{
							int time = (list.cycle/1000)-member.nOnTime;

							if(time < 0)
								command += "0";
							else
								command += time.ToString();
						}
					}
					else 
						command += member.sValue;

					command += "'";
				}
			}

			command += ")";

			if(bUpdate) 
			{
				command += "";
			}

			return command;
		}
		
		void CheckDataBaseSaveListOne(SaveList list, SYSTEMTIME st, DateTime t, bool bDuplexDisable)
		{
            // 실행 태그가 존재할 때만 1인가를 검사한다.
            if (list.bExistItemRunTag)
            {
                string curr = "";

                if (SharedTag.GetCurr(list.sItemRunTag, ref curr))
                {
                    if (ConvertTool.ToInt32(curr) != 1)
                    {
                        if (ShareServerMain.bShowRunTagOffStatus)
                        {
                            // 나중에 메시지는 빼는 것도 좋을 듯...
                            string msg;

                            if (Tools.IsLangKorean())
                                msg = String.Format("목록:{1} - 실행태그({0})가 OFF 상태이므로 저장을 하지 않습니다.", list.sItemRunTag, list.title);
                            else
                                msg = String.Format("List:{1} - Saving skipped. Run Tag ({0}) is OFF status.", list.sItemRunTag, list.title);

                            MessageDisplay.Show(msg);
                        }

                        //Stopwatch sw = new Stopwatch();
                        //sw.Start();
                        InitOneSaveListVars(list);  // 초기화가 필요하다. 2019-6-7 지원. 계속 초기화 할 필요는 없지만 간단해서 일단 적용한다. (1mmsec가 안걸린다.)
                        //sw.Stop();
                        //TimeSpan ts = sw.Elapsed;

                        return;
                    }
                }
            }
            
			ConnectionString dsn = dsnList.GetConnection(list.con_dsn);

			if(dsn == null)	return;	// DSN not found

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			if(!bDuplexDisable) 
			{
				try 
				{
					//db.ConnectionString = dsn.dsn;
					db.Open();
				}
				catch 
				{
					return;
				}

				if(db.State != ConnectionState.Open)	return;
			}

            CommonDbTransaction transaction = new CommonDbTransaction();
            if (db.bAddCommitAfterCommand)
            {
                transaction.BeginTransaction(db);
            }

			string command;

			DatabaseMember member;
			bool save_flag = true;
			string fail_tag = "";

			for(int i = 0; i < list.arrayMember.Count; i++) 
			{
				member = (DatabaseMember)list.arrayMember[i];

				MakeStringByTag(out member.sValue, member);

				CalcMinMaxEtcMember(member, member.sValue, t);

				try 
				{
					member.fValue = ConvertTool.ToDouble(member.sValue);
				}
				catch 
				{
					member.fValue = 0;
				}

				if(!member.bValueReadFlag) 
				{
					save_flag = false;
					fail_tag = member.tag;
				}
			}

            CommonDbCommand cmd = new CommonDbCommand(db.connType);

            cmd.Connection = db;
            if (db.bAddCommitAfterCommand)
            {
                cmd.Transaction = transaction;
            }

			while(list.remain_millisec >= list.cycle) 
			{
				if(list.bSaveAsNextTime)	// 저장시점으로 데이터를 저장하려면 시간을 먼저 더해준다.
				{
					TimeUtil.AddMilliSecond(list.stOld, list.cycle);
				}

				// 속에 특수한 계산이 들어있기 때문에 무조건 실행해야 한다.
				command = MakeInsertUpdateCommand(dsn, list, st, false);

				if(!bDuplexDisable) 
				{
					if(save_flag) 
					{
						cmd.CommandText = command;

						try 
						{
							cmd.ExecuteNonQuery();
						}
						catch (Exception exception)
						{
							if(Tools.IsLangKorean())
								MessageDisplay.Show("["+list.title+"] 저장목록에서 오류\n"+exception.Message);
							else
								MessageDisplay.Show("["+list.title+"] Save List Error\n"+exception.Message);
						}
					}
					else 
					{
						string msg;
						if(Tools.IsLangKorean())
							msg = String.Format("[{0}] 저장목록에서 [{1}] 태그의 값을 읽을 수 없습니다.\n감시 프로그램이 실행중이 아니거나 태그가 없습니다.", list.title, fail_tag);
						else
							msg = String.Format("At savelist [{0}]\n[{1}] Tag not found.\nAutobase.exe program not exist or tag not found.", list.title, fail_tag);

						MessageDisplay.Show(msg);
					}
				}

				// 시간은 나중에 더한다.
				if(!list.bSaveAsNextTime)	// 데이터 시점 저장 방법이다.
				{
					TimeUtil.AddMilliSecond(list.stOld, list.cycle);
				}
				list.remain_millisec -= list.cycle;
			}

			// 사용자가 갑자기 시간을 바꿀 수 있으므로 적절한 시간으로 변경한다.
			list.stOld.Set(t);
			FitTimeWithCycle(list, list.stOld);

			if(!bDuplexDisable) 
			{
                if (db.bAddCommitAfterCommand)
                {
                    transaction.Commit();
                }
				db.Close();
			}

			for(int i = 0; i < list.arrayMember.Count; i++) 
			{
				member = (DatabaseMember)list.arrayMember[i];

				member.nCalcCount = 0;
				if(member.nOnTime > (list.cycle/1000))
					member.nOnTime -= (list.cycle/1000);
				else
					member.nOnTime = 0;

				member.nOnOffCount = 0;
				member.bValueReadFlag = true;
			}
		}

		void CalcMinMaxEtcMember(DatabaseMember member, string getval, DateTime t)
		{
			double val;

			try 
			{
				val = ConvertTool.ToDouble(getval);
			}
			catch 
			{
				val = 0;
			}

			if(member.nCalcCount == 0) 
			{
				member.fCalcMin = val;
				member.fCalcMax = val;
				member.fCalcSum = val;
			}
			else 
			{
				member.fCalcSum += val;
				if(val > member.fCalcMax)	member.fCalcMax = val;
				if(val < member.fCalcMin)	member.fCalcMin = val;
			}

			member.nCalcCount++;

			// 가동 시간
			if(member.eSaveType == EnumSaveType.OnTime || member.eSaveType == EnumSaveType.OffTime) 
			{
				if(val == 1) 
				{
					if(member.fOldValue == 0) 
					{
						member.nLastCalcSec = t.Second;
						member.nOnOffCount++;
					}
					if(member.fOldValue == 1) 
					{
						if(t.Second < member.nLastCalcSec) 
						{
							member.nOnTime += 60+t.Second-member.nLastCalcSec;
						}
						else
							member.nOnTime += t.Second-member.nLastCalcSec;

						member.nLastCalcSec = t.Second;
					}
				}
				else 
				{
					if(member.fOldValue == 1)
						member.nOnOffCount++;
				}

				member.fOldValue =val;
			}
		}

		void CalcMinMaxEtc(SaveList list, DateTime dt)
		{
			string imsi="0";
			DatabaseMember member;

			for(int i = 0; i < list.arrayMember.Count; i++)
			{
				member = (DatabaseMember)list.arrayMember[i];

				MakeStringByTag(out imsi, member);

				CalcMinMaxEtcMember(member, imsi, dt);
			}
		}

		bool GetDateTimeFromRow(int date_type, DataRow row, int time_col_pos, ref DateTime t)
		{
			if(date_type == 1) 
			{
				string s = row[time_col_pos].ToString();
				if(s.Length < 14) 
				{
					return false;
				}
				try 
				{
					int year, mon, day, hour, min, sec;
					year = ConvertTool.ToInt32(s.Substring(0, 4));
					mon = ConvertTool.ToInt32(s.Substring(4, 2));
					day = ConvertTool.ToInt32(s.Substring(6, 2));
					hour = ConvertTool.ToInt32(s.Substring(8, 2));
					min = ConvertTool.ToInt32(s.Substring(10, 2));
					sec = ConvertTool.ToInt32(s.Substring(12, 2));
					
					t = new DateTime(year, mon, day, hour, min, sec);
					return true;
				}
				catch 
				{
					return false;
				}
			}
			else 
			{
				try 
				{
					t = ConvertTool.ToDateTime(row[time_col_pos].ToString());
					return true;
				}
				catch 
				{
					return false;
				}
			}
		}

		void RemoveSameDate(SaveList list, DataTable dt)
		{
			if(dt.Rows.Count == 0)	return;

			int time_col_pos = dt.Columns.IndexOf(list.sColumnDate);

			DateTime t1 = DateTime.Now;
			DateTime t2 = DateTime.Now;
			DataRow row;

			for(int i = 0; i < dt.Rows.Count-1; i++) 
			{
				row = dt.Rows[i];
				
				if(!GetDateTimeFromRow(list.nDateType, row, time_col_pos, ref t1))	continue; 

				for(int j = i+1; j < dt.Rows.Count; j++) 
				{
					row = dt.Rows[j];

					if(!GetDateTimeFromRow(list.nDateType, row, time_col_pos, ref t2))	continue; 

					if( t1.Year == t2.Year && t1.Month == t2.Month && 
						t1.Day == t2.Day && t1.Hour == t2.Hour &&
						t1.Minute == t2.Minute && t1.Second == t2.Second) 
					{
						dt.Rows.RemoveAt(j);
						j--;
					}
				}
			}			
		}

		void CalcWithTable(SaveList list, string target, string source, DateTime dt_org, int type)
		{
			DateTime dt = new DateTime(dt_org.Ticks);

			ConnectionString dsn = dsnList.GetConnection(list.con_dsn);

			if(dsn == null)	return;	// DSN not found

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			try 
			{
				//db.ConnectionString = dsn.dsn;
				db.Open();
			}
			catch 
			{
				return;
			}

			if(db.State != ConnectionState.Open)	return;

			string command_fr;
			string command_to;

			if(type == 0) 
			{
				command_fr = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
				command_to = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 59);
			}
			else if(type == 1) 
			{
				command_fr = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
				command_to = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, 59, 59);
			}
			else if(type == 2) 
			{
				command_fr = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, 0, 0, 0);
				command_to = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, 23, 59, 59);
			}
			else if(type == 3) 
			{
				command_fr = MakeDateString(dsn, list, dt.Year, dt.Month, 1, 0, 0, 0);
				command_to = MakeDateString(dsn, list, dt.Year, dt.Month, 31, 23, 59, 59);
			}
			else 
			{
				command_fr = MakeDateString(dsn, list, dt.Year, 1, 1, 0, 0, 0);
				command_to = MakeDateString(dsn, list, dt.Year, 12, 31, 23, 59, 59);
			}

			// 시간을 ASC로 정렬한 이유는 순시치일 경우 제일 처음의 값을 취하기가 유리하기 때문이다.
			// 초기에는 DESC로 정렬했으나 2003-1-14 ASC로 바꾸었다. (울산 현장 때문에)
			string command = String.Format("SELECT * FROM {0} WHERE {3} >= {1} AND {3} <= {2} ORDER BY {3} ASC", source, command_fr, command_to, list.sColumnDate);

			CommonDbDataAdapter ad = new CommonDbDataAdapter(command, db);
			DataTable dtable = new DataTable();
			try 
			{
				ad.Fill(dtable);
			}
			catch
			{
					
			}
            
			RemoveSameDate(list, dtable);

			// 저장시점의 시간으로 시간을 저장할때는 분자료와 시간자료만 해당된다.
			// 지원의 가장 중요한 이유는 시간이 지났을 때 그 시간의 데이터가 바로 생겼으면 하는 사용자 의견이다.(울산수자원)
			// 순시치는 저장할 시점의 데이터, 기타(평균,최소,최고 값은) 이전 시간의 데이터를 계산해서 현재 시간으로 저장한다.
			// 아래 현재 시간대의 순시치를 가져온다. 일반적으로 하나의 데이터만 들어있을 것이다.
			DataTable dtable_moment = null;
			if(list.bSaveAsNextTime)	
			{
				if(type == 0)	// 분 연관저장
				{
					dt = dt.AddMinutes(1);
					command_fr = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
					command_to = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 59);
				}
				else if(type == 1)	// 시간 연관저장
				{
					dt = dt.AddHours(1);
					command_fr = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
					command_to = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, 59, 59);
				}
				else
				{
					
				}
				
				if(type <= 1) 
				{
					command = String.Format("SELECT * FROM {0} WHERE {3} >= {1} AND {3} <= {2} ORDER BY {3} ASC", source, command_fr, command_to, list.sColumnDate);

					ad = new CommonDbDataAdapter(command, db);
					dtable_moment = new DataTable();
					try 
					{
						ad.Fill(dtable_moment);
					}
					catch
					{
					
					}
				}
			}

			DatabaseMember member;
			DataRow row;
			int col_pos;
			string sval = "";
			string command_member=list.sColumnDate;
			string command_value;
			
			if(type == 0) 
				command_value = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
			else if(type == 1)
				command_value = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
			else if(type == 2)
				command_value = MakeDateString(dsn, list, dt.Year, dt.Month, dt.Day, 0, 0, 0);
			else if(type == 3)
				command_value = MakeDateString(dsn, list, dt.Year, dt.Month, 1, 0, 0, 0);
			else 
				command_value = MakeDateString(dsn, list, dt.Year, 1, 1, 0, 0, 0);

			for(int m = 0; m < list.arrayMember.Count; m++) 
			{
				member = (DatabaseMember)list.arrayMember[m];

				col_pos = dtable.Columns.IndexOf(DbTool.ChangeFieldChar(dsn.dbtype, member.field));
				

				if(member.eSaveType == EnumSaveType.Ave) 
				{
					double sum = 0;

					for(int i = 0; i < dtable.Rows.Count; i++) 
					{
						row = dtable.Rows[i];
						try 
						{
							sum += ConvertTool.ToDouble(row[col_pos].ToString());
						}
						catch 
						{
							
						}
					}

					if(dtable.Rows.Count == 0) 
					{
						sum = 0;
					}
					else 
					{
						sum = sum/dtable.Rows.Count;
					}

					sval = sum.ToString();
				}
				else if(member.eSaveType == EnumSaveType.Max) 
				{
					double read = 0;
					double max=0;

					for(int i = 0; i < dtable.Rows.Count; i++) 
					{
						row = dtable.Rows[i];
						try 
						{
							read = ConvertTool.ToDouble(row[col_pos].ToString());
						}
						catch 
						{
							read = 0;
						}

						if(i == 0)	max = read;
						else 
						{
							if(read > max)	max = read;
						}
					}

					sval = max.ToString();
				}
				else if(member.eSaveType == EnumSaveType.Min) 
				{
					double read = 0;
					double min=0;

					for(int i = 0; i < dtable.Rows.Count; i++) 
					{
						row = dtable.Rows[i];
						try 
						{
							read = ConvertTool.ToDouble(row[col_pos].ToString());
						}
						catch 
						{
							read = 0;
						}

						if(i == 0)	min = read;
						else 
						{
							if(read < min)	min = read;
						}
					}

					sval = min.ToString();
				}
				else if(member.eSaveType == EnumSaveType.Sum ||
					member.eSaveType == EnumSaveType.OnTime ||
					member.eSaveType == EnumSaveType.OffTime) 
				{
					double sum = 0;

					for(int i = 0; i < dtable.Rows.Count; i++) 
					{
						row = dtable.Rows[i];
						try 
						{
							sum += ConvertTool.ToDouble(row[col_pos].ToString());
						}
						catch 
						{
							
						}
					}

					// On/Off Time의 경우 레코드가 중복되면 큰값이 나타날 수 있으므로
					// 최대 범위까지 잘라준다. 하지만 레코드가 빠진 경우는 곤란하다.
					if(member.eSaveType == EnumSaveType.OnTime ||
						member.eSaveType == EnumSaveType.OffTime) 
					{
						if(type == 0 && sum > 60)			sum = 60;
						else if(type == 1 && sum > 3600)	sum = 3600;
						else if(type == 2 && sum > 3600*24)	sum = 3600*24;
						else {}	// 달이나 년 같은 경우는 최대 범위를 계산하기는 곤란하다.
					}

					sval = sum.ToString();
				}
				else if(member.eSaveType == EnumSaveType.Moment)
				{
					if(list.bSaveAsNextTime && type <= 1) 
					{
						if(dtable_moment.Rows.Count == 0) 
						{
							sval = "NULL";
						}
						else 
						{
							row = dtable_moment.Rows[0];
							sval = row[col_pos].ToString();
						}
					}
					else 
					{
						if(dtable.Rows.Count == 0) 
						{
							sval = "NULL";
						}
						else 
						{
							row = dtable.Rows[0];
							sval = row[col_pos].ToString();
						}
					}
				}
				else {}

				command_member += ',';
				command_member += DbTool.Field(dsn.dbtype, member.field);
				command_value += ',';
				command_value += sval;
			}

			command = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", target, command_member, command_value);

			CommonDbCommand cmd = new CommonDbCommand(command, db);

            CommonDbTransaction transaction = new CommonDbTransaction();
            if (db.bAddCommitAfterCommand)
            {
                transaction.BeginTransaction(db);
                cmd.Transaction = transaction;
            }

			try 
			{
				cmd.ExecuteNonQuery();

                if (db.bAddCommitAfterCommand)
                {
                    transaction.Commit();
                }
			}
			catch 
			{
                if (db.bAddCommitAfterCommand)
                {
                    transaction.Rollback();
                }
			}
                                                                          			
			db.Close();
		}

		int nBlockPos;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns>false가 실패만는 아니다. 저장할 시간이 되지 않아서 아무것도 안한 경우도 해당이 된다.</returns>
        bool CheckDataBaseSaveListOne(SaveList list)
        {
            if (!list.bActive) return false;

            DateTime dt = DateTime.Now;

            long curr_milli;
            long curr = TimeUtil.GetMilliSecHap(dt);
            curr_milli = curr - list.old_milli_sec;

            if (curr_milli < 0)    // 시간이 꺼꾸로 갔다.  남아있는 밀리초를 클리어한다.  2013-1-17 추가함 시간이 꺼꾸로 가니 한시간 가량의 데이터가 쌓인다.
            // 이런 경우는 시간이 전체적으로 바뀌었으므로 목록을 다시 초기화 하는 것이 좋을 듯 하다.
            {
                DatabaseSaveListInit();
                return false;
            }

            list.remain_millisec += (int)curr_milli;
            list.old_milli_sec = curr;

            bool bDuplexDisable = (list.bRelationDuplex && SystemStatusMemory.GetDI(SSMDI.DuplexActiveI) == 0);

            // 이중화 연동 모드이고 내 서버가 활성화가 아닐때는 시간만 소비한다.
            if (list.remain_millisec >= list.cycle)
            {	// 조건이 되었을 때만 저장한다.
                CheckDataBaseSaveListOne(list, list.stOld, dt, bDuplexDisable);
            }

            if (dt.Second != list.dtOld.Second) // 초가 바뀌었다.
            {
                CalcMinMaxEtc(list, dt);
            }

            if (!bDuplexDisable) // 이중화 연동 모드이고 활성화거나 단독 모드일 때.
            {
                if (dt.Minute != list.dtOld.Minute) // 분이 바뀌었다.
                {
                    if (list.bTableSaveMinute)
                    {
                        CalcWithTable(list, list.sTableSaveMinute, list.table, list.dtOld, 0);
                    }
                }
                if (dt.Hour != list.dtOld.Hour) // 시간이 바뀌었다.
                {
                    if (list.bTableSaveHour)
                    {
                        if (list.bTableSaveMinute)
                            CalcWithTable(list, list.sTableSaveHour, list.sTableSaveMinute, list.dtOld, 1);
                        else
                            CalcWithTable(list, list.sTableSaveHour, list.table, list.dtOld, 1);
                    }
                }
                if (dt.Day != list.dtOld.Day) // 일이 바뀌었다.
                {
                    if (list.bTableSaveDay)
                    {
                        if (list.bTableSaveHour)
                            CalcWithTable(list, list.sTableSaveDay, list.sTableSaveHour, list.dtOld, 2);
                        else if (list.bTableSaveMinute)
                            CalcWithTable(list, list.sTableSaveDay, list.sTableSaveMinute, list.dtOld, 2);
                        else
                            CalcWithTable(list, list.sTableSaveDay, list.table, list.dtOld, 2);
                    }
                }
                if (dt.Month != list.dtOld.Month) // 월이 바뀌었다.
                {
                    if (list.bTableSaveMonth)
                    {
                        if (list.bTableSaveDay)
                            CalcWithTable(list, list.sTableSaveMonth, list.sTableSaveDay, list.dtOld, 3);
                        else if (list.bTableSaveHour)
                            CalcWithTable(list, list.sTableSaveMonth, list.sTableSaveHour, list.dtOld, 3);
                        else if (list.bTableSaveMinute)
                            CalcWithTable(list, list.sTableSaveMonth, list.sTableSaveMinute, list.dtOld, 3);
                        else
                            CalcWithTable(list, list.sTableSaveMonth, list.table, list.dtOld, 3);
                    }
                }
                if (dt.Year != list.dtOld.Year) // 년이 바뀌었다.
                {
                    if (list.bTableSaveYear)
                    {
                        if (list.bTableSaveMonth)
                            CalcWithTable(list, list.sTableSaveYear, list.sTableSaveMonth, list.dtOld, 4);
                        else if (list.bTableSaveDay)
                            CalcWithTable(list, list.sTableSaveYear, list.sTableSaveDay, list.dtOld, 4);
                        else if (list.bTableSaveHour)
                            CalcWithTable(list, list.sTableSaveYear, list.sTableSaveHour, list.dtOld, 4);
                        else if (list.bTableSaveMinute)
                            CalcWithTable(list, list.sTableSaveYear, list.sTableSaveMinute, list.dtOld, 4);
                        else
                            CalcWithTable(list, list.sTableSaveYear, list.table, list.dtOld, 4);
                    }
                }
            }

            list.dtOld = dt;

            return true;
        }

        // 목록이 많을 때 중간 중간에 저장을 하지 않거나 저장시간 차이가 많이 나는 경우 타이머만 소모하는 경우를 대비해서
        // 목록하나를 저장할 때까지 다음 목록을 계속 체크하도록 수정하였다.  2016-5-17

        public bool bSkippedBecauseRunTagIsOff = false;

		public void CheckDataBaseSaveList()
		{
            if (classSaveList.arrayDatabaseSaveList.Count == 0) return;

            // 실행 태그가 존재할 때만 1인가를 검사한다.
            if (bExistRunTag)
            {
                string curr = "";

                if (SharedTag.GetCurr(classSaveList.sRunTag, ref curr))
                {
                    if (ConvertTool.ToInt32(curr) != 1)
                    {
                        bSkippedBecauseRunTagIsOff = true;
                        return;
                    }
                    else
                    {
                        // 실행태그가 꺼져 있다가 시작되면 리셋해 준다. 2021-7-1 추가
                        if (bSkippedBecauseRunTagIsOff)
                        {
                            bSkippedBecauseRunTagIsOff = false;
                            FormMain.sharedRunMain.DatabaseSaveListInit();
                        }
                    }
                }
            }

            bSkippedBecauseRunTagIsOff = false;
            
            SaveList list;

            for (int i = 0; i < 100; i++)
            {
                nBlockPos++;
                nBlockPos %= classSaveList.arrayDatabaseSaveList.Count;

                list = (SaveList)classSaveList.arrayDatabaseSaveList[nBlockPos];

                if (CheckDataBaseSaveListOne(list)) return;     // 한 목록을 잘 저장했으면 유연하게 하기 위해 돌아간다. 한 타이머마다 하나씩만 저장한다.
            }
		}
	}
}


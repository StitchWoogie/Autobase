using System;
using DatabaseSaveList;
using DatabaseConnection;
using System.Data;
using System.Collections;
using AutoLibLocal;
using System.IO;
using NetTools;
using System.Threading.Tasks;

namespace DialogAddition
{
	/// <summary>
	/// Summary description for Addition.
	/// </summary>
	/// 
	public class AdditionMember
	{
		public string sSaveItem;
		public string sField;
	}

	public class AdditionList
	{
		public ArrayList arrayAddition = new ArrayList();

		public void Load()
		{
			string path;
			string work_dir = TotalConfig.sDirWorkProject;

			path = String.Format("{0}\\Addition\\Addition.lst", work_dir);

			if(!File.Exists(path))	return;

			FileStream fs = File.OpenRead(path);
			
			if(fs == null)	return;

			TextReader reader = new StreamReader(fs);

			CommaBlockString comma = new CommaBlockString();
			string one_line;
			AdditionMember member;

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;

				if(one_line.Length == 0)	continue;

				member = new AdditionMember();
				comma.Set(one_line);
				comma.GetString(ref member.sField);
				comma.GetString(ref member.sSaveItem);
                
				arrayAddition.Add(member);
			}

			reader.Close();
		}

		public void Save()
		{	
			string path;
			string work_dir = TotalConfig.sDirWorkProject;

			path = String.Format("{0}\\Addition", work_dir);

			if(!Directory.Exists(path)) 
			{
				Directory.CreateDirectory(path);
			}

			path = String.Format("{0}\\Addition\\Addition.lst", work_dir);

			Stream fs = File.Open(path, FileMode.Create);
			if(fs == null)	return;
			TextWriter writer = new StreamWriter(fs);

			AdditionMember member;

			for(int i = 0; i < arrayAddition.Count; i++) 
			{
				member = (AdditionMember)arrayAddition[i];
				writer.WriteLine("{0},{1},", member.sField, member.sSaveItem);
			}

			writer.Close();
		}
	}

	public enum EnumAdditionType 
	{
		ThisHour,
		LastHour,		
		ThisDay,
		LastDay,
		ThisMonth,
		LastMonth,
		ThisYear,
		LastYear,		
	}

	public class Addition
	{
		public DatabaseSaveListClass classSaveList = null;
		public ConnectionStringList dsnList = null;

		public Addition()
		{
			//
			// TODO: Add constructor logic here
			//
			classSaveList = new DatabaseSaveListClass();
			dsnList = new ConnectionStringList();

			classSaveList.DatabaseSaveListLoad();
			dsnList.ConnectionStringLoad();
		}

		public static double GetAddedValueFromTable(CommonDbConnection db, string command_fr, string command_to, string table, string field)
		{
			string command = String.Format("SELECT {0} FROM {1} WHERE DataSavedTime >= {2} AND DataSavedTime <= {3} ORDER BY DataSavedTime DESC", 
				field,
				table, 
				command_fr, 
				command_to);

			CommonDbDataAdapter ad = new CommonDbDataAdapter(command, db);
			DataTable dt = new DataTable();

			try 
			{
				ad.Fill(dt);
			}
			catch
			{
					
			}

			double val = 0;

			if(dt.Rows.Count == 0) 
			{
				val = 0;
			}
			else 
			{
				DataRow row;
				for(int i = 0; i < dt.Rows.Count; i++) 
				{
					row = dt.Rows[i];
					try 
					{
						val += ConvertTool.ToDouble(row[0].ToString());
					}
					catch
					{

					}
				}
			}

			return val;
		}

        public static bool GetAddition(ConnectionStringList csl, DatabaseSaveListClass dsl, string sSaveListName, string sColumn, EnumAdditionType eType, out double val)
        {
            val = 0;

            SaveList save_list = dsl.GetSaveList(sSaveListName);

            if (save_list == null) return false;    // save list not found

            ConnectionString dsn = csl.GetConnection(save_list.con_dsn);

            if (dsn == null) return false;	// DSN not found

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
            try
            {
                //db.ConnectionString = dsn.dsn;
                db.Open();
            }
            catch
            {
                return false;
            }

            if (db.State != ConnectionState.Open) return false;

            string command_fr;
            string command_to;

            DateTime t = DateTime.Now;

            string field = DbTool.Field(dsn.dbtype, sColumn);
            //double val_curr = 0;
            //double val_old = 0;
            DateTime ct;
            val = 0;

            // 전시 계산
            if (eType == EnumAdditionType.LastHour)
            {
                ct = t.AddHours(-1);
                command_fr = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, ct.Day, ct.Hour, 0, 0);
                command_to = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, ct.Day, ct.Hour, 59, 59);
                val = GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
            }
            else if (eType == EnumAdditionType.LastDay)
            {
                // 전일 계산
                ct = t.AddDays(-1);
                command_fr = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, ct.Day, 0, 0, 0);
                command_to = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, ct.Day, 23, 59, 59);
                if (save_list.bTableSaveHour)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
                }
                else if (save_list.bTableSaveMinute)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
                }
                else
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
                }
            }
            else if (eType == EnumAdditionType.LastMonth)
            {
                // 전월 계산
                ct = t.AddMonths(-1);
                command_fr = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, 1, 0, 0, 0);
                command_to = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, 31, 23, 59, 59);
                if (save_list.bTableSaveDay)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveDay, field);
                }
                else if (save_list.bTableSaveHour)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
                }
                else if (save_list.bTableSaveMinute)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
                }
                else
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
                }
            }
            else if (eType == EnumAdditionType.LastYear)
            {
                // 전년 계산
                ct = t.AddYears(-1);
                command_fr = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, 1, 1, 0, 0, 0);
                command_to = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, 12, 31, 23, 59, 59);
                if (save_list.bTableSaveMonth)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMonth, field);
                }
                else if (save_list.bTableSaveDay)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveDay, field);
                }
                else if (save_list.bTableSaveHour)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
                }
                else if (save_list.bTableSaveMinute)
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
                }
                else
                {
                    val = GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
                }
            }
            else
            {
                // 금시 계산
                command_fr = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day, t.Hour, 0, 0);
                command_to = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day, t.Hour, 59, 59);
                val = GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);

                if (eType == EnumAdditionType.ThisHour) return true;

                // 금일 계산
                if (t.Hour > 0)
                {   // 1시 이상일 경우
                    command_fr = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day, 0, 0, 0);
                    command_to = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day, t.Hour - 1, 59, 59);
                    if (save_list.bTableSaveHour)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
                    }
                    else if (save_list.bTableSaveMinute)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
                    }
                    else
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
                    }
                }

                if (eType == EnumAdditionType.ThisDay) return true;

                // 금월 계산
                if (t.Day > 1)
                {   // 1시 이상일 경우
                    command_fr = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, 1, 0, 0, 0);
                    command_to = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day - 1, 23, 59, 59);
                    if (save_list.bTableSaveDay)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveDay, field);
                    }
                    else if (save_list.bTableSaveHour)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
                    }
                    else if (save_list.bTableSaveMinute)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
                    }
                    else
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
                    }
                }

                if (eType == EnumAdditionType.ThisMonth) return true;

                // 금년 계산
                if (t.Month > 1)
                {   // 1시 이상일 경우
                    command_fr = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, 1, 1, 0, 0, 0);
                    command_to = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month - 1, 31, 23, 59, 59);
                    if (save_list.bTableSaveMonth)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMonth, field);
                    }
                    else if (save_list.bTableSaveDay)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveDay, field);
                    }
                    else if (save_list.bTableSaveHour)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
                    }
                    else if (save_list.bTableSaveMinute)
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
                    }
                    else
                    {
                        val += GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
                    }
                }
            }

            db.Close();

            return true;
        }

        public bool GetAddition(string sSaveListName, string sColumn, EnumAdditionType eType, out double val)
        {
            return GetAddition(dsnList, classSaveList, sSaveListName, sColumn, eType, out val);
        }
    }
}

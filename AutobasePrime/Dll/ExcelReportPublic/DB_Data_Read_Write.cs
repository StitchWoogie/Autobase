using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;	
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;	
using AutoLibLocal;
using AutoLib;
using NetTools;
using DatabaseConnection;
using System.Threading.Tasks;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for DB_Data_Read_Write.
	/// </summary>
	public class DB_Data_Read_Write
	{
		
		public DB_Data_Read_Write()
		{
			//
			// TODO: Add constructor logic here
			//
			//dsnList.ConnectionStringLoad();
		}

		/*static void getMatchDateTimeString(ConnectionString dsn, DateTime basicDt, ref string command_fr, int dataTypeSort, int index)
		{
			DateTime	dt;

			if(dataTypeSort == 0)			// 분
			{			
				dt = new DateTime(basicDt.Year, basicDt.Month, basicDt.Day, basicDt.Hour, 0, 0, 0);
				dt = dt.AddMinutes(index);
				command_fr = DbTool.MakeDateTimeString(dsn.dbtype, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);				
			}
			else if(dataTypeSort == 1)		// 시간
			{
				dt = new DateTime(basicDt.Year, basicDt.Month, basicDt.Day, 0, 0, 0, 0);
				dt = dt.AddHours(index);
				command_fr = DbTool.MakeDateTimeString(dsn.dbtype, dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
			}
			else if(dataTypeSort == 2)		// 일
			{
				dt = new DateTime(basicDt.Year, basicDt.Month, 1, 0, 0, 0, 0);
				dt = dt.AddDays(index);
				command_fr = DbTool.MakeDateTimeString(dsn.dbtype, dt.Year, dt.Month, dt.Day, 0, 0, 0);
			}
			else if(dataTypeSort == 3)		// 월
			{
				dt = new DateTime(basicDt.Year, 1, 1, 0, 0, 0, 0);
				dt = dt.AddMonths(index);
				command_fr = DbTool.MakeDateTimeString(dsn.dbtype, dt.Year, dt.Month, 1, 0, 0, 0);
			}
			else					// 년
			{
				command_fr = DbTool.MakeDateTimeString(dsn.dbtype, index, 1, 1, 0, 0, 0);
			}		
		}*/

		static string Make14DateString(EnumDbType dbtype, int year, int month, int day, int hour, int minute, int second)
		{
            if(dbtype == EnumDbType.MDB)    // MDB인경우 XP 까지는 '을 양쪽에 붙이지 않아도 되었는데 Vista/7 에서는 붙여야 된다.
                return String.Format("'{0,4:d04}{1,2:d02}{2,2:d02}{3,2:d02}{4,2:d02}{5,2:d02}'", year, month, day, hour, minute, second);
            else
			    return String.Format("{0,4:d04}{1,2:d02}{2,2:d02}{3,2:d02}{4,2:d02}{5,2:d02}", year, month, day, hour, minute, second);
		}

        static string Make12DateString(EnumDbType dbtype, int year, int month, int day, int hour, int minute)
		{
            if (dbtype == EnumDbType.MDB)   // MDB인경우 XP 까지는 '을 양쪽에 붙이지 않아도 되었는데 Vista/7 에서는 붙여야 된다.
			    return String.Format("'{0,4:d04}{1,2:d02}{2,2:d02}{3,2:d02}{4,2:d02}'", year, month, day, hour, minute);
            else
                return String.Format("{0,4:d04}{1,2:d02}{2,2:d02}{3,2:d02}{4,2:d02}", year, month, day, hour, minute);
		}

		static void getMatchDateTimeCalcString(ConnectionString dsn, DateTime basicDt, ref string command, int dataTypeSort, int upTime, int currTime, int nDateFormat, bool toFlag)
		{
			DateTime	dt;

			if(dataTypeSort == 0)			// 분
			{
				dt = new DateTime(basicDt.Year, basicDt.Month, basicDt.Day, basicDt.Hour, 0, 0, 0);
				dt = dt.AddHours(upTime);
				dt = dt.AddMinutes(currTime);
				if(toFlag) dt = dt.AddMinutes(1);

				if(nDateFormat == 1)
				{
					command = Make14DateString(dsn.dbtype, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
				}
				else if(nDateFormat == 2)
				{
                    command = Make12DateString(dsn.dbtype, dt.Year, dt.Month, dt.Day, dt.Hour, 0);
				}
				else 
				{
					command = DbTool.MakeDateTimeString(dsn.dbtype, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
				}
			}
			else if(dataTypeSort == 1)		// 시간
			{
				dt = new DateTime(basicDt.Year, basicDt.Month, basicDt.Day, 0, 0, 0, 0);
				dt = dt.AddDays(upTime);
				dt = dt.AddHours(currTime);
				if(toFlag) dt = dt.AddHours(1);
				if(nDateFormat == 1)
				{
                    command = Make14DateString(dsn.dbtype, dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
				}
				else if(nDateFormat == 2)
				{
                    command = Make12DateString(dsn.dbtype, dt.Year, dt.Month, dt.Day, 0, 0);
				}
				else 
				{
					command = DbTool.MakeDateTimeString(dsn.dbtype, dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
					//command = String.Format("'{0}-{1}-{2} {3}:{4}:{5}'", dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
				}
			}
			else if(dataTypeSort == 2)		// 일
			{
				dt = new DateTime(basicDt.Year, basicDt.Month, 1, 0, 0, 0, 0);
				dt = dt.AddMonths(upTime);
				if(currTime-1 > TimeUtil.getmonthlimit(dt.Year, dt.Month))				
					dt = dt.AddDays(TimeUtil.getmonthlimit(dt.Year, dt.Month)-1);		// 1일 ~				
				else 				
					dt = dt.AddDays(currTime-1);		// 1일 ~				
				if(toFlag) dt = dt.AddDays(1);
				if(nDateFormat == 1)
				{
                    command = Make14DateString(dsn.dbtype, dt.Year, dt.Month, dt.Day, 0, 0, 0);
				}
				else if(nDateFormat == 2)
				{
                    command = Make12DateString(dsn.dbtype, dt.Year, dt.Month, 0, 0, 0);
				}
				else 
				{
					command = DbTool.MakeDateTimeString(dsn.dbtype, dt.Year, dt.Month, dt.Day, 0, 0, 0);
				}
			}
			else if(dataTypeSort == 3)		// 월
			{
				dt = new DateTime(basicDt.Year, 1, 1, 0, 0, 0, 0);
				dt = dt.AddYears(upTime);
				dt = dt.AddMonths(currTime-1);		// 1월 ~
				if(toFlag) dt = dt.AddMonths(1);
				if(nDateFormat == 1)
				{
                    command = Make14DateString(dsn.dbtype, dt.Year, dt.Month, 1, 0, 0, 0);
				}
				else if(nDateFormat == 2)
				{
                    command = Make12DateString(dsn.dbtype, dt.Year, 0, 0, 0, 0);
				}
				else 
				{
					command = DbTool.MakeDateTimeString(dsn.dbtype, dt.Year, dt.Month, 1, 0, 0, 0);
				}
			}
			else					// 년
			{
				if(toFlag) currTime += 1;
				if(nDateFormat == 1)
				{
                    command = Make14DateString(dsn.dbtype, currTime, 1, 1, 0, 0, 0);
				}
				else 
				{
					command = DbTool.MakeDateTimeString(dsn.dbtype, currTime, 1, 1, 0, 0, 0);
				}
			}
		}

		/*static string getMatchColumnTableName(EnumDbType dbtype, string name)
		{
			if(dbtype == EnumDbType.Oracle) return columnName;

			int			i;

			for(i = 0; i < name.Length; i++) 
			{
				if(name[i] == ' ') break;
			}
			if(i >= name.Length) return cname;
			return "[" + name + "]";			
		}*/

		static public DataTable dbDataRead(ConnectionString dsn, string tableName, string columnName, string sDateColumnName, int nDateFormat, string sOptionWhere, DateTime basicDt, int dataTypeSort, int upTime, int currTime)
		{
			if(dsn == null)	return null;	// DSN not found
			if(tableName.Length <= 0 || columnName.Length <= 0) return null;	// Table, Column Name = NULL
			if(BasicRptTool.checkOverDay(basicDt, dataTypeSort, upTime, currTime)) return null;// 일 자료 이고, 지정 날짜가 최대 월의 날짜보다 클때..

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			try 
			{
				//db.ConnectionString = dsn.dsn;
				db.Open();
			}
			catch 
			{
				return null;
			}

			if(db.State != ConnectionState.Open)	return null;
            if (dsn.dbtype != EnumDbType.Oracle && dsn.dbtype != EnumDbType.Tibero) 
			{
				tableName = DbTool.Field(dsn.dbtype, tableName);
			}
			columnName = DbTool.Field(dsn.dbtype, columnName);
			sDateColumnName = DbTool.Field(dsn.dbtype, sDateColumnName);
			//tableName = getMatchColumnTableName(dsn.dbtype, tableName);
			//columnName = getMatchColumnTableName(dsn.dbtype, columnName);
			//sDateColumnName = getMatchColumnTableName(dsn.dbtype, sDateColumnName);
			

			string command_fr =  "", command;

			getMatchDateTimeCalcString(dsn, basicDt, ref command_fr, dataTypeSort, upTime, currTime, nDateFormat, false);	// from
			if(sOptionWhere == null || sOptionWhere.Length <= 0) 
			{
				command = String.Format("SELECT {0} FROM {1} WHERE {2}={3}", columnName, tableName, sDateColumnName, command_fr);
			}
			else 
			{
				command = String.Format("SELECT {0} FROM {1} WHERE {2}={3} AND {4}", columnName, tableName, sDateColumnName, command_fr, sOptionWhere);
			}
			
			CommonDbDataAdapter ad = new CommonDbDataAdapter(command, db);
			DataTable dtable = new DataTable();
			try 
			{
				ad.Fill(dtable);
			}
			catch
			{
					
			}
			db.Close();
			if(dtable.Rows.Count == 0) return null;
			else 					   return dtable;

			/*if(dtable.Rows.Count == 0) 
			{
				return noneDataString;
			}
			else 
			{
				DataRow row = dtable.Rows[0];

				try 
				{
					string	buf = row[columnName].ToString();
					if(buf == null) return "0";
					if(buf.Length <= 0) return "0";
					return buf;
				}
				catch 
				{
					return noneDataString;
				}
			}*/
		}

		static public string dbDataCurrentValueRead(ConnectionString dsn, string tableName, string columnName, string fieldVal, string searchFieldName, string noneDataString)
		{
			if(dsn == null)	return noneDataString;	// DSN not found
			if(tableName.Length <= 0 || columnName.Length <= 0) return noneDataString;	// Table, Column Name = NULL

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			try 
			{
				//db.ConnectionString = dsn.dsn;
				db.Open();
			}
			catch 
			{
				return noneDataString;
			}

			if(db.State != ConnectionState.Open)	return noneDataString;

            if (dsn.dbtype != EnumDbType.Oracle && dsn.dbtype != EnumDbType.Tibero) 
			{
				tableName = DbTool.Field(dsn.dbtype, tableName);
			}
			columnName = DbTool.Field(dsn.dbtype, columnName);
			string	buf;

			//buf = DbTool.Field(dsn.dbtype, columnName);
			string command = String.Format("SELECT {0},CURR FROM {1} WHERE {0}='{2}'", columnName, tableName, fieldVal);

			CommonDbDataAdapter ad = new CommonDbDataAdapter(command, db);
			DataTable dtable = new DataTable();
			try 
			{
				ad.Fill(dtable);
			}
			catch
			{
					
			}
			db.Close();

			if(dtable.Rows.Count == 0) 
			{
				return noneDataString;
			}
			else 
			{
				DataRow row = dtable.Rows[0];

				try 
				{
					buf = row[searchFieldName].ToString();
					if(buf == null) return "0";
					if(buf.Length <= 0) return "0";
					return buf;
				}
				catch 
				{
					return noneDataString;
				}
			}
		}

		static public bool dbDataWrite(ConnectionString dsn, string tableName, string fieldName, string sDateColumnName, int nDateFormat, string sOptionWhere, DateTime basicDt, int dataTypeSort, int upTime, int currTime, string sVal, bool bAddFlag)
		{
			if(dsn == null)	return false;	// DSN not found
			if(tableName.Length <= 0 || fieldName.Length <= 0) return false;	// Table, Column Name = NULL
			if(BasicRptTool.checkOverDay(basicDt, dataTypeSort, upTime, currTime)) return false;// 일 자료 이고, 지정 날짜가 최대 월의 날짜보다 클때..

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

			if(db.State != ConnectionState.Open)	return false;

			string command_value = "";//, command_val = "";
			string command_member = DbTool.Field(dsn.dbtype, sDateColumnName);
			string command;

			command_member += ',';
			command_member += DbTool.Field(dsn.dbtype, fieldName);
			
			getMatchDateTimeCalcString(dsn, basicDt, ref command_value, dataTypeSort, upTime, currTime, nDateFormat, false);	// from

			if(bAddFlag == true) 
			{		// 추가			
				command_value += ',';
				command_value += sVal;
			}
            if (dsn.dbtype != EnumDbType.Oracle && dsn.dbtype != EnumDbType.Tibero) 
			{
				tableName = DbTool.Field(dsn.dbtype, tableName);
			}
			fieldName = DbTool.Field(dsn.dbtype, fieldName);
			sDateColumnName = DbTool.Field(dsn.dbtype, sDateColumnName);
			
			if(bAddFlag == true)		// 추가			
			{
				if(sOptionWhere == null || sOptionWhere.Length <= 0) 
				{
					command = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, command_member, command_value);
				}
				else		// Where 문장이 있을 경우에는 Insert 가 되기는 하지만 Where 문장으로 읽기 때문에 없는 값으로 나온다.
				{
					return false;
					//command = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, command_member, command_value);
				}
			}
			else 
			{ 
				if(sOptionWhere == null || sOptionWhere.Length <= 0) 
				{
					command = String.Format("UPDATE {0} SET {1}={2} WHERE {3}={4}", tableName, fieldName, sVal, sDateColumnName, command_value);
				}
				else 
				{
					command = String.Format("UPDATE {0} SET {1}={2} WHERE {3}={4} AND {5}", tableName, fieldName, sVal, sDateColumnName, command_value, sOptionWhere);
				}
			}

			try 
			{
				CommonDbCommand cmd = new CommonDbCommand(command, db);
				cmd.ExecuteNonQuery();
				return true;
			}
			catch 
			{
				return false;
			}
		}


		static public DataTable dbMultiColumnRowRead(ConnectionString  dsn, string sDateColumnName, int nDateFormat, int nElementNum, string[] fieldData, DateTime basicDt)
		{
			if(dsn == null)	return null;	// DSN not found
			if(nElementNum < 13) return null;
			if(fieldData[1].Length <= 0 || fieldData[2].Length <= 0 || fieldData[4].Length <= 0) return null;	// Table, Time Column, Time sort (분, 시, 일, 월, 년) = NULL

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			try 
			{
				//db.ConnectionString = dsn.dsn;
				db.Open();
			}
			catch 
			{
				return null;
			}

			if(db.State != ConnectionState.Open)	return null;

			string	command_fr =  "", command_to =  "", command;
			int		dataTypeSort, nSorting = 0;
			int[]	currTime = new Int32[2], upTime = new Int32[2];

			dataTypeSort = BasicRptTool.getReadDataSortType(fieldData[4]);
			try 
			{
				currTime[0] = ConvertTool.ToInt32(fieldData[5]);
				currTime[1] = ConvertTool.ToInt32(fieldData[6]);
				upTime[0] = ConvertTool.ToInt32(fieldData[7]);
				upTime[1] = ConvertTool.ToInt32(fieldData[8]);
				nSorting = ConvertTool.ToInt32(fieldData[10]);
			}
			catch
			{
				return null;
			}

			getMatchDateTimeCalcString(dsn, basicDt, ref command_fr, dataTypeSort, upTime[0], currTime[0], nDateFormat, false);
			getMatchDateTimeCalcString(dsn, basicDt, ref command_to, dataTypeSort, upTime[1], currTime[1], nDateFormat, true);
			fieldData[1] = DbTool.Field(dsn.dbtype, fieldData[1]);
			sDateColumnName = DbTool.Field(dsn.dbtype, sDateColumnName);
			
			command = String.Format("SELECT * FROM {0} WHERE {1} >= {2} AND {1} < {3}", fieldData[1], sDateColumnName, command_fr, command_to);

            string where = run_DbDataMain.GetStringOrgOrVar(fieldData[3]);
            if (where.Length > 0)
                command += String.Format(" AND {0}", where);

			if(fieldData[9].Length > 0) 
			{
				switch(nSorting) 
				{
					case 1 : command += String.Format(" ORDER BY {0} ASC", fieldData[9]); break;
					case 2 : command += String.Format(" ORDER BY {0} DESC", fieldData[9]); break;
					default: break;
				}
			}

			
			CommonDbDataAdapter ad = new CommonDbDataAdapter(command, db);
			DataTable dtable = new DataTable();
			try 
			{
				ad.Fill(dtable);
			}
			catch
			{				
			}
			db.Close();

			if(dtable.Rows.Count == 0) return null;
			else 					   return dtable;
		}


		static public DataTable dbMultiColumnRead(ConnectionString dsn, string tableName, string columnName, string sDateColumnName, int nDateFormat, string sOptionWhere, DateTime basicDt, int dataTypeSort, int startCurrTime, int endCurrTime, int startUpTime, int endUpTime)
		{
			if(dsn == null)	return null;	// DSN not found
			if(tableName.Length <= 0 || columnName.Length <= 0) return null;	// Table, Column Name = NULL

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			try 
			{
				//db.ConnectionString = dsn.dsn;
				db.Open();
			}
			catch 
			{
				return null;
			}

			if(db.State != ConnectionState.Open) return null;

			string command_fr =  "", command_to =  "", command;

			getMatchDateTimeCalcString(dsn, basicDt, ref command_fr, dataTypeSort, startUpTime, startCurrTime, nDateFormat, false);	// from
			getMatchDateTimeCalcString(dsn, basicDt, ref command_to, dataTypeSort, endUpTime, endCurrTime, nDateFormat, true);	// to
            if (dsn.dbtype != EnumDbType.Oracle && dsn.dbtype != EnumDbType.Tibero) 
			{
				tableName = DbTool.Field(dsn.dbtype, tableName);
			}
			columnName = DbTool.Field(dsn.dbtype, columnName);
			sDateColumnName = DbTool.Field(dsn.dbtype, sDateColumnName);

			if(sOptionWhere == null || sOptionWhere.Length <= 0) 
			{
				command = String.Format("SELECT {0},{1} FROM {2} WHERE {0} >= {3} AND {0} < {4}", sDateColumnName, columnName, tableName, command_fr, command_to);				
			}
			else 
			{
				command = String.Format("SELECT {0},{1} FROM {2} WHERE {0} >= {3} AND {0} < {4} AND {5}", sDateColumnName, columnName, tableName, command_fr, command_to, sOptionWhere);
			}
			
			CommonDbDataAdapter ad = new CommonDbDataAdapter(command, db);
			DataTable dtable = new DataTable();
			try 
			{
				ad.Fill(dtable);
			}
			catch
			{				
			}
			db.Close();

			if(dtable.Rows.Count == 0) return null;
			else 					   return dtable;			
		}


		static public DataTable dbAiMaxMinMultiColumnRead(ConnectionString dsn, string tableName, string columnName, string sDateColumnName, int nDateFormat, string sOptionWhere, DateTime basicDt, int dataTypeSort, int startCurrTime, int endCurrTime, int startUpTime, int endUpTime)
		{
			if(dsn == null)	return null;	// DSN not found
			if(tableName.Length <= 0 || columnName.Length <= 0) return null;	// Table, Column Name = NULL

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			try 
			{
				//db.ConnectionString = dsn.dsn;
				db.Open();
			}
			catch 
			{
				return null;
			}

			if(db.State != ConnectionState.Open)	return null;

			string	command_fr =  "", command_to =  "", command;
			
			
			getMatchDateTimeCalcString(dsn, basicDt, ref command_fr, dataTypeSort, startUpTime, startCurrTime, nDateFormat, false);	// from
			getMatchDateTimeCalcString(dsn, basicDt, ref command_to, dataTypeSort, endUpTime, endCurrTime, nDateFormat, true);		// to
            if (dsn.dbtype != EnumDbType.Oracle && dsn.dbtype != EnumDbType.Tibero) 
			{
				tableName = DbTool.Field(dsn.dbtype, tableName);
			}
			sDateColumnName = DbTool.Field(dsn.dbtype, sDateColumnName);

			if(sOptionWhere == null || sOptionWhere.Length <= 0) 
			{
				command = String.Format("SELECT * FROM {0} WHERE {1} >= {2} AND {1} < {3}", tableName, sDateColumnName, command_fr, command_to);				
			}
			else 
			{			
				command = String.Format("SELECT * FROM {0} WHERE {1} >= {2} AND {1} < {3} AND {4}", tableName, sDateColumnName, command_fr, command_to, sOptionWhere);
			}
						
			CommonDbDataAdapter ad = new CommonDbDataAdapter(command, db);
			DataTable dtable = new DataTable();
			try 
			{
				ad.Fill(dtable);
			}
			catch
			{				
			}
			db.Close();

			if(dtable.Rows.Count == 0) return null;
			else 					   return dtable;
		}


		static public DataTable dbMultiQueryDataRead(ConnectionString  dsn, string sQueryString)
		{
			if(dsn == null)	return null;	// DSN not found

            string query = run_DbDataMain.GetStringOrgOrVar(sQueryString);

            if (query == null || query.Length <= 0) return null;	// query string = NULL

            CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			try 
			{
				//db.ConnectionString = dsn.dsn;
				db.Open();
			}
			catch 
			{
				return null;
			}

			if(db.State != ConnectionState.Open)	return null;

            CommonDbDataAdapter ad = new CommonDbDataAdapter(query, db);
			DataTable dtable = new DataTable();
			try 
			{
				ad.Fill(dtable);
			}
			catch
			{				
			}
			db.Close();

			if(dtable.Rows.Count == 0) return null;
			else 					   return dtable;			
		}



	}
}

using System;
using System.IO;
using NetTools.OldDefine;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for DbTool.
	/// </summary>
	public class DbTool
	{
		public DbTool()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Db Server 의 종류에 따라서 맞는 필드 스트링을 만든다.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="field"></param>
		/// <returns>좌우로 스트링이 사</returns>
		public static string Field(EnumDbType dbtype, string field)
		{
            if (dbtype == EnumDbType.Oracle || dbtype == EnumDbType.Tibero)
				return "\""+field+"\"";
			else if(dbtype == EnumDbType.MySQL)
				return "`"+field+"`";
			else
				return "["+field+"]";
		}

		public static string MakeDateTimeString(EnumDbType dbtype, int year, int mon, int day, int hour, int min, int sec)
		{
			string imsi;

            if (dbtype == EnumDbType.Oracle || dbtype == EnumDbType.Tibero) 
			{
				imsi = String.Format("TO_DATE('{0}-{1}-{2} {3}:{4}:{5}', 'YYYY-MM-DD HH24:MI:SS')", year, mon, day, hour, min, sec);

				/*
				if(hour >= 12)
					imsi = String.Format("TO_DATE('{0}-{1}-{2} {3}:{4}:{5} 오후', 'yyyy-mm-dd HH:MI:SS PM')", year, mon, day, (hour%12) == 0 ? 12 : (hour%12), min, sec);
				else
					imsi = String.Format("TO_DATE('{0}-{1}-{2} {3}:{4}:{5} 오전', 'yyyy-mm-dd HH:MI:SS AM')", year, mon, day, (hour%12) == 0 ? 12 : (hour%12), min, sec);
				*/

			}
			else if(dbtype == EnumDbType.MDB) 
			{
				imsi = String.Format("#{0}-{1}-{2} {3}:{4}:{5}#", year, mon, day, hour, min, sec);	
			}
			else 
			{
				imsi = String.Format("'{0}-{1}-{2} {3}:{4}:{5}'", year, mon, day, hour, min, sec);
			}

			return imsi;
		}

		public static string MakeDateTimeString(EnumDbType dbtype, DateTime t)
		{
			return MakeDateTimeString(dbtype, t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
		}

		public static string MakeDateTimeString(EnumDbType dbtype, int subtype, SYSTEMTIME t)
		{
			return MakeDateTimeString(dbtype, t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond); 
		}

        /*
		public static void FillComboBox(System.Windows.Forms.ComboBox combo, ConnectionStringList list)
		{
			ConnectionString conn;	
			int i;

			combo.Items.Clear();
			for(i = 0; i < list.arrayConnectionString.Count; i++) 
			{
				conn = (ConnectionString)list.arrayConnectionString[i];
				combo.Items.Add(conn.title);
			}
		}

		static ConnectionStringList connList = null;

		public static ConnectionString GetConnectionString(string dsn)
		{
			if(connList == null) 
			{
				connList = new ConnectionStringList();
				connList.ConnectionStringLoad();
			}

			return connList.GetConnection(dsn);
		}

		public static bool GetConnectionStringDbType(string dsn, out EnumDbType dbtype)
		{
			ConnectionString conn = GetConnectionString(dsn);
			if(conn == null) 
			{
				dbtype = EnumDbType.Normal;
				return false;
			}

			dbtype = conn.dbtype;
			return true;
		}*/

		
	}
}

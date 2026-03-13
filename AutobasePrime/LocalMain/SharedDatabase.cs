using System;
using System.IO;
using AutoLibLocal;
using AutoLib;
using NetTools;
using System.Data;
using System.Threading.Tasks;

namespace LocalMain
{
	public class WEB_SERVER_STRUCT 
	{
		public bool	bActive;
		public bool	bUseDuplex;
		public string sDsn;
		public string sTableTagExchange = "TagExchange";
		public string sTableAlarmFile = "AlarmFile";
		public bool	bTableAlarmFile;
        public bool bTableTagExchange;  // 9.3.9 부터 추가
	}

	/// <summary>
	/// Summary description for SharedDatabase.
	/// </summary>
	public class SharedDatabase
	{
		public static WEB_SERVER_STRUCT configWebServer = new WEB_SERVER_STRUCT();

		static SharedDatabase()
		{
			//
			// TODO: Add constructor logic here
			//
			LoadWebServerConfig();
		}

		static void LoadWebServerConfig()
		{
			string filename;
			filename = String.Format("{0}\\Config\\SharedDatabase.lstx", TotalConfig.sDirWorkProject);
			if(!File.Exists(filename))	return;
			TextReader reader = new StreamReader(filename);
			if(reader == null)	return;

            configWebServer.bTableTagExchange = true;   // 이전에 사용한 프로젝트는 사용함으로 설정

			string one_line;
			string command = "";
			CommaBlockString comma = new CommaBlockString();

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				comma.Set(one_line);
				comma.GetString(ref command);
				if(command == "bActive")
					comma.GetBool(ref configWebServer.bActive);
				else if(command == "bUseDuplex")
					comma.GetBool(ref configWebServer.bUseDuplex);
				else if(command == "sDsn")
					comma.GetString(ref configWebServer.sDsn);
				else if(command == "sTableTagExchange")
					comma.GetString(ref configWebServer.sTableTagExchange);
				else if(command == "sTableAlarmFile")
					comma.GetString(ref configWebServer.sTableAlarmFile);
				else if(command == "bTableAlarmFile")
					comma.GetBool(ref configWebServer.bTableAlarmFile);
                else if (command == "bTableTagExchange")
                    comma.GetBool(ref configWebServer.bTableTagExchange);
				else {}
			}
			reader.Close();
		}

		public static void SaveWebServerConfig()
		{
			string filename;

            filename = String.Format("{0}\\Config", TotalConfig.sDirWorkProject);
            Directory.CreateDirectory(filename);

			filename = String.Format("{0}\\Config\\SharedDatabase.lstx", TotalConfig.sDirWorkProject);
			TextWriter writer = new StreamWriter(filename);
			if(writer == null)	return;

			writer.WriteLine("bActive,{0},", configWebServer.bActive);
			writer.WriteLine("bUseDuplex,{0},", configWebServer.bUseDuplex);
			writer.WriteLine("sDsn,{0},", configWebServer.sDsn);
			writer.WriteLine("sTableTagExchange,{0},", configWebServer.sTableTagExchange);
			writer.WriteLine("sTableAlarmFile,{0},", configWebServer.sTableAlarmFile);
			writer.WriteLine("bTableAlarmFile,{0},", configWebServer.bTableAlarmFile);
            writer.WriteLine("bTableTagExchange,{0},", configWebServer.bTableTagExchange);
			writer.Close();
		}

		static bool IsActive()
		{
			if(!configWebServer.bActive)	return false;
			if(configWebServer.bUseDuplex) 
			{
				if(SystemStatusMemory.GetDI(SSMDI.DuplexActiveI) == 0)	return false;
			}
			return true;
		}

		static bool bFirstFlag = false;

		public static void Reset()
		{
			bFirstFlag = false;
		}

		static void MakeTableTagExchange(CommonDbConnection conn, ConnectionString dsn, string table_name)
		{
			CheckTable check = new CheckTable();

			check.AddColumn("ID", EnumDbDataType.Integer, 0);
			check.AddColumn("TAG", EnumDbDataType.String, 80);
			check.AddColumn("curr", EnumDbDataType.String, 255);

			check.Check(conn, dsn.dbtype, table_name);

			DataTable dt = new DataTable();
			string query = String.Format("SELECT TAG FROM {0}", table_name); 
			CommonDbDataAdapter adapter = new CommonDbDataAdapter(query, conn);
            try
            {
                adapter.Fill(dt);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("ShareaDatabase.MakeTableTagExchange Error\nDsn={0}\nQuery={1}\nMessage={2}", dsn.dsn, query, exception.Message);
                return;
            }

			DataRow row;

			TagListStruct[] list = TagLib.MakeTagList();

			for(int i = 0; i < list.Length; i++) 
			{
				for(int j = 0; j < dt.Rows.Count; j++) 
				{
					row = dt.Rows[j];
					if(String.Compare(list[i].tag, row[0].ToString(), false) == 0)	goto next;
					
				}
				query = String.Format("INSERT INTO {0} (TAG) VALUES ('{1}')", table_name, list[i].tag);
				CommonDbCommand command = new CommonDbCommand(query, conn);
				command.ExecuteNonQuery();
			next:;
			}
		}

		static void MakeTableAlarmFile(CommonDbConnection conn, ConnectionString dsn, string table_name)
		{
			CheckTable check = new CheckTable();

			check.AddColumn("ID", EnumDbDataType.Integer, 0);
			check.AddColumn("TAG", EnumDbDataType.String, 80);
			check.AddColumn("Description", EnumDbDataType.String, 80);
			check.AddColumn("tAlarm", EnumDbDataType.DateTime, 0);
			check.AddColumn("Message", EnumDbDataType.String, 40);
			check.AddColumn("AlarmPriority", EnumDbDataType.Integer, 0);
			check.AddColumn("Port", EnumDbDataType.Integer, 0);
			check.AddColumn("Station", EnumDbDataType.Integer, 0);
			check.AddColumn("Address", EnumDbDataType.Integer, 0);
			check.AddColumn("AlarmType", EnumDbDataType.Integer, 0);

			check.Check(conn, dsn.dbtype, table_name);
		}

		static void CheckFirst(CommonDbConnection conn, ConnectionString dsn)
		{
			if(bFirstFlag)	return;
			bFirstFlag = true;


            if (configWebServer.bTableTagExchange)
			    MakeTableTagExchange(conn, dsn, configWebServer.sTableTagExchange);

			if(configWebServer.bTableAlarmFile)
				MakeTableAlarmFile(conn, dsn, configWebServer.sTableAlarmFile);
		}

		static void EventPublic(string tag, string curr)
		{
			if(!IsActive())					                return;
            if (configWebServer.bTableTagExchange == false) return;	

			ConnectionString dsn = DbTool.dsnList.GetConnection(configWebServer.sDsn);

			if(dsn == null)	return;	// DSN not found

            CommonDbConnection conn = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);

			try 
			{
				//conn.ConnectionString = dsn.dsn;
				conn.Open();
			}
			catch 
			{
				return;
			}

			CheckFirst(conn, dsn);

			string query = String.Format("UPDATE {0} SET curr='{1}' WHERE Tag='{2}'", configWebServer.sTableTagExchange, curr, tag);
			CommonDbCommand command = new CommonDbCommand(dsn.dbConnectionType);
			command.Connection = conn;
			command.CommandText = query;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("Error on CurrentValueSave\nDsn={0}\nTable={1}\nMessage={2}", dsn.dsn, configWebServer.sTableTagExchange, exception.Message);
            }
			conn.Close();
		}

		public static void EventAI(TagAiClass ai)
		{
			EventPublic(ai.tag, ai.curr.ToString("F5"));
		}

		public static void EventAO(TagAoClass ao)
		{
			EventPublic(ao.tag, ao.curr.ToString("F5"));
		}

		public static void EventDI(TagDiClass di)
		{
			EventPublic(di.tag, di.curr.ToString());
		}

		public static void EventDO(TagDoClass dout)
		{
			EventPublic(dout.tag, dout.curr.ToString());
		}

		public static void EventST(TagStClass st)
		{
			EventPublic(st.tag, st.curr);
		}

		public static void WebServerAddAlarm(ALARM_FILE_STRUCT alarm)
		{
			if(configWebServer.bTableAlarmFile == false)	return;	
			if(!IsActive())					return;

			ConnectionString dsn = DbTool.dsnList.GetConnection(configWebServer.sDsn);

			if(dsn == null)	return;	// DSN not found

            CommonDbConnection conn = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);

			try 
			{
				//conn.ConnectionString = dsn.dsn;
				conn.Open();
			}
			catch 
			{
				return;
			}

			CheckFirst(conn, dsn);

			string query = String.Format("INSERT INTO {0} (Tag,Description,tAlarm,Message,AlarmPriority,Port,Station,Address,AlarmType)", configWebServer.sTableAlarmFile);
			query += " VALUES(";
			query += String.Format("'{0}',", alarm.tag);
			query += String.Format("'{0}',", alarm.description);
			query += DbTool.MakeDateTimeString(dsn.dbtype, alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay, alarm.t.wHour, alarm.t.wMinute, alarm.t.wSecond);
			query += ',';
			query += String.Format("'{0}',", alarm.msg);
			query += String.Format("'{0}',", alarm.priority);
			query += String.Format("'{0}',", alarm.port);
			query += String.Format("'{0}',", alarm.station);
			query += String.Format("'{0}',", alarm.address);
			query += String.Format("'{0}'",  alarm.alarm_type);
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
                MessageDisplay.Show("Error on AlarmDataSave\nDsn={0}\nTable={1}\nMessage={2}", dsn.dsn, configWebServer.sTableAlarmFile, exception.Message);
            }
			conn.Close();
		}
	}
}


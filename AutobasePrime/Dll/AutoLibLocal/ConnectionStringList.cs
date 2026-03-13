using System;
using System.Collections;
using System.IO;
using NetTools;

namespace AutoLibLocal  
{
	[Serializable]
	public class ConnectionString
	{
		public string title;
		public string dsn;
		public EnumDbType dbtype;
		public EnumDbConnectionType dbConnectionType;
        public bool bAddCommitAfterCommand;
	}

	/// <summary>
	/// Summary description for ConnectionStringList.
	/// </summary>
	public class ConnectionStringList
	{
		public ArrayList arrayConnectionString = new ArrayList();

		public ConnectionStringList()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void ConnectionStringLoad()
		{	
			string path;

			path = String.Format("{0}\\Database\\ConnectionString.lst", TotalConfig.sDirWorkProject);

			if(!File.Exists(path))	return;

			FileStream fs = File.OpenRead(path);
			
			if(fs == null)	return;

			TextReader reader = new StreamReader(fs);

            CommaTextReader comma = new CommaTextReader();
			string one_line;
			ConnectionString conn;
			string type="";

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;

				if(one_line.Length == 0)	continue;

				conn = new ConnectionString();
				comma.Set(one_line);
				comma.GetString(ref conn.title);
				comma.GetString(ref conn.dsn);
				comma.GetString(ref type);
				try 
				{
					conn.dbtype = (EnumDbType)Enum.Parse(typeof(EnumDbType), type);
				}
				catch
				{
					conn.dbtype = EnumDbType.Normal;
				}
				comma.GetString(ref type);
				if(String.Compare(type, "ODBC", true) == 0)
					conn.dbConnectionType = EnumDbConnectionType.ODBC;
				else
					conn.dbConnectionType = EnumDbConnectionType.OleDb;
                conn.bAddCommitAfterCommand = comma.GetBool();
                
				arrayConnectionString.Add(conn);
			}

			reader.Close();
		}

		public void ConnectionStringSave()
		{	
			string path;
			string work_dir = TotalConfig.sDirWorkProject;

			path = String.Format("{0}\\Database", work_dir);

			if(!Directory.Exists(path)) 
			{
				Directory.CreateDirectory(path);
			}

			path = String.Format("{0}\\Database\\ConnectionString.lst", work_dir);

			Stream fs = File.Open(path, FileMode.Create);
			if(fs == null)	return;
            CommaTextWriter writer = new CommaTextWriter(fs);

			ConnectionString conn;

			for(int i = 0; i < arrayConnectionString.Count; i++) 
			{
				conn = (ConnectionString)arrayConnectionString[i];

                writer.WriteLine("{0},{1},{2},{3},{4}", conn.title, conn.dsn, conn.dbtype.ToString(), conn.dbConnectionType.ToString(), conn.bAddCommitAfterCommand);
			}

			writer.Close();
		}

		public ConnectionString GetConnection(string dsn)
		{
			ConnectionString conn;	
			int i;

			for(i = 0; i < arrayConnectionString.Count; i++) 
			{
				conn = (ConnectionString)arrayConnectionString[i];
				if(conn.title == dsn)	return conn;
			}

            if (dsn.Length > 4)
            {
                string ext = dsn.Substring(dsn.Length - 4);
                if (String.Compare(ext, ".mdb", true) == 0) // mdb file
                {
                    conn = new ConnectionString();
                    conn.dbConnectionType = EnumDbConnectionType.OleDb;
                    conn.dbtype = EnumDbType.MDB;
                    conn.dsn = String.Format("Provider=Microsoft.JET.OLEDB.4.0;Data Source={0};", dsn);
                    conn.title = String.Format("Access file {0}", dsn);
                    return conn;
                }
            }

			return null;
		}

        public void ChangeConnection(string dsn, string text)
        {
            ConnectionString conn;
            int i;

            for (i = 0; i < arrayConnectionString.Count; i++)
            {
                conn = (ConnectionString)arrayConnectionString[i];
                if (conn.title == dsn)
                {
                    conn.dsn = text;
                    ConnectionStringSave();
                    return;
                }
            }
        }

				
		
	}
}

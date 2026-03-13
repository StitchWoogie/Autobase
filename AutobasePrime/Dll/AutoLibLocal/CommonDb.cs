using System;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Threading.Tasks;

namespace AutoLibLocal
{
	public class CommonDbConnection
	{
		public EnumDbConnectionType connType = EnumDbConnectionType.OleDb;

		public OleDbConnection connOleDb;
		public OdbcConnection connOdbc;

        public bool bAddCommitAfterCommand = false; // DB의 설정이 DbCommand 후에 Commit을 해주어야 하는 경우가 있다. 2018-1-9

        /*
		public CommonDbConnection(EnumDbConnectionType type)
		{
			//
			// TODO: Add constructor logic here
			//
			connType = type;

			if(type == EnumDbConnectionType.ODBC) 
				connOdbc = new OdbcConnection();
			else
				connOleDb = new OleDbConnection();
		}	

		public CommonDbConnection(EnumDbConnectionType type, string connectionString)
		{
			//
			// TODO: Add constructor logic here
			//
			connType = type;

			if(type == EnumDbConnectionType.ODBC) 
				connOdbc = new OdbcConnection(connectionString);
			else
				connOleDb = new OleDbConnection(connectionString);
		}*/

        // ExecuteNoneQuery 후에 Commit을 해야 되는 DB설정을 위해 commit을 지원. 2018-1-9
        public CommonDbConnection(EnumDbConnectionType type, string connectionString, bool commit)
        {
            //
            // TODO: Add constructor logic here
            //
            connType = type;

            if (type == EnumDbConnectionType.ODBC)
                connOdbc = new OdbcConnection(connectionString);
            else
                connOleDb = new OleDbConnection(connectionString);

            bAddCommitAfterCommand = commit;
        }

        /*
		public string ConnectionString 
		{
			set
			{
				if(connType == EnumDbConnectionType.ODBC)	
					connOdbc.ConnectionString = value;
				else									
					connOleDb.ConnectionString = value;
			}
		}*/

		public void Open()
		{
			if(connType == EnumDbConnectionType.ODBC)
				connOdbc.Open();
			else
			 	connOleDb.Open();
		}

		public ConnectionState State
		{
			get
			{
				if(connType == EnumDbConnectionType.ODBC)	
					return connOdbc.State;
				else
					return connOleDb.State;
			}
		}

		public void Close()
		{
			if(connType == EnumDbConnectionType.ODBC)
				connOdbc.Close();
			else
				connOleDb.Close();
		}
	}

	public class CommonDbDataAdapter
	{
		OleDbDataAdapter adapterOleDb;
		OdbcDataAdapter adapterOdbc;

		EnumDbConnectionType conn_type = EnumDbConnectionType.OleDb;

		public CommonDbDataAdapter(string selectCommandText, CommonDbConnection selectConnection)
		{
			conn_type = selectConnection.connType;

			if(conn_type == EnumDbConnectionType.ODBC)
				adapterOdbc = new OdbcDataAdapter(selectCommandText, selectConnection.connOdbc);
			else
				adapterOleDb = new OleDbDataAdapter(selectCommandText, selectConnection.connOleDb);
		}

		public void Fill(DataSet dataSet, string srcTable)
		{
			if(conn_type == EnumDbConnectionType.ODBC)
				adapterOdbc.Fill(dataSet, srcTable);
			else
				adapterOleDb.Fill(dataSet, srcTable);
		}

		public void Fill(DataSet dataSet)
		{
			if(conn_type == EnumDbConnectionType.ODBC)
				adapterOdbc.Fill(dataSet);
			else
				adapterOleDb.Fill(dataSet);
		}

		public void Fill(DataTable dataTable)
		{
			if(conn_type == EnumDbConnectionType.ODBC)
				adapterOdbc.Fill(dataTable);
			else
				adapterOleDb.Fill(dataTable);
		}
	}

	public class CommonDbCommand
	{
		OleDbCommand commandOleDb;
		OdbcCommand commandOdbc;

		EnumDbConnectionType conn_type = EnumDbConnectionType.OleDb;

		public CommonDbCommand(EnumDbConnectionType type)
		{
			conn_type = type;

			if(type == EnumDbConnectionType.ODBC) 
				commandOdbc = new OdbcCommand();
			else
				commandOleDb = new OleDbCommand();
		}

		public CommonDbCommand(string cmdText, CommonDbConnection conn)
		{
			conn_type = conn.connType;

			if(conn_type == EnumDbConnectionType.ODBC) 
				commandOdbc = new OdbcCommand(cmdText, conn.connOdbc) ;
			else
				commandOleDb = new OleDbCommand(cmdText, conn.connOleDb);
		}

		public CommonDbConnection Connection
		{
			set 
			{
				if(conn_type == EnumDbConnectionType.ODBC)
					commandOdbc.Connection = value.connOdbc;
				else
					commandOleDb.Connection = value.connOleDb;
			}
		}

		public string CommandText
		{
			set 
			{
				if(conn_type == EnumDbConnectionType.ODBC)
					commandOdbc.CommandText = value;
				else
					commandOleDb.CommandText = value;
			}
			get 
			{
				if(conn_type == EnumDbConnectionType.ODBC)
					return commandOdbc.CommandText;
				else
					return commandOleDb.CommandText;
			}
		}

        public CommonDbTransaction Transaction
        {
            set
            {
                if (conn_type == EnumDbConnectionType.ODBC)
                    commandOdbc.Transaction = value.tranOdbc;
                else
                    commandOleDb.Transaction = value.tranOleDb;
            }
        }

		public void ExecuteNonQuery()
		{
			if(conn_type == EnumDbConnectionType.ODBC)
				commandOdbc.ExecuteNonQuery();
			else
				commandOleDb.ExecuteNonQuery();
		}

        public void AddParameter(string name, object value)
        {
            if (conn_type == EnumDbConnectionType.ODBC)
                commandOdbc.Parameters.AddWithValue(name, value ?? DBNull.Value);
            else
                commandOleDb.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        public CommonDbDataReader ExecuteReader()
        {
            if (conn_type == EnumDbConnectionType.ODBC)
                return new CommonDbDataReader(commandOdbc.ExecuteReader());
            else
                return new CommonDbDataReader(commandOleDb.ExecuteReader());
        }

        public object ExecuteScalar()
        {
            if (conn_type == EnumDbConnectionType.ODBC)
                return commandOdbc.ExecuteScalar();
            else
                return commandOleDb.ExecuteScalar();
        }
    }


    public class CommonDbTransaction
    {
        public OleDbTransaction tranOleDb;
        public OdbcTransaction tranOdbc;

        EnumDbConnectionType conn_type = EnumDbConnectionType.OleDb;

        public void BeginTransaction(CommonDbConnection conn)
        {
            conn_type = conn.connType;

            if (conn_type == EnumDbConnectionType.ODBC)
                tranOdbc = conn.connOdbc.BeginTransaction();
            else
                tranOleDb = conn.connOleDb.BeginTransaction();
        }

        public void Commit()
        {
            if (conn_type == EnumDbConnectionType.ODBC)
                tranOdbc.Commit();
            else
                tranOleDb.Commit();
        }

        public void Rollback()
        {
            if (conn_type == EnumDbConnectionType.ODBC)
                tranOdbc.Rollback();
            else
                tranOleDb.Rollback();
        }
    }

    public class CommonDbDataReader : IDisposable
    {
        private IDataReader _reader;

        public CommonDbDataReader(IDataReader reader)
        {
            _reader = reader;
        }

        public bool Read()
        {
            return _reader.Read();
        }

        public int FieldCount
        {
            get { return _reader.FieldCount; }
        }

        public object this[int i]
        {
            get { return _reader[i]; }
        }

        public object this[string name]
        {
            get { return _reader[name]; }
        }

        public string GetString(int i)
        {
            return _reader.IsDBNull(i) ? "" : _reader.GetString(i);
        }

        public int GetInt32(int i)
        {
            return _reader.IsDBNull(i) ? 0 : _reader.GetInt32(i);
        }

        public double GetDouble(int i)
        {
            return _reader.IsDBNull(i) ? 0.0 : _reader.GetDouble(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _reader.IsDBNull(i) ? DateTime.MinValue : _reader.GetDateTime(i);
        }

        public bool IsDBNull(int i)
        {
            return _reader.IsDBNull(i);
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader.Dispose();
                _reader = null;
            }
        }
    }
}

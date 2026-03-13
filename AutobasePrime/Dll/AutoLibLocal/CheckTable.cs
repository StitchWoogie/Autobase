using System;
using System.Collections;
using System.Data;

namespace AutoLibLocal
{
	class ColumnList
	{
		public string column;
		public EnumDbDataType type;
		public int size;
	}
	/// <summary>
	/// Summary description for CheckTable.
	/// </summary>
	public class CheckTable
	{
		public CheckTable()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		ArrayList listColumn = new ArrayList();

		public void AddColumn(string column_name, EnumDbDataType type, int size)
		{
			ColumnList col = new ColumnList();
			col.column = column_name;
			col.type = type;
			col.size = size;
			this.listColumn.Add(col);
		}

        static void NewTable(CommonDbConnection conn, EnumDbType dbtype, string table_name, ColumnList type)
        {
            CommonDbCommand command = new CommonDbCommand(conn.connType);

            string stype;

            stype = DbTool.MakeVarType(dbtype, type.type, type.size);

            command.Connection = conn;

            // 오라클에서 ""로 싸인 경우 소문자가 포함되면 "" 이 컬럼명으로 같이 포함되므로 컬럼명은 대문자로 보관한다. 9.5.2 부터 수정

            if (dbtype == EnumDbType.Oracle || dbtype == EnumDbType.Tibero)
                command.CommandText = String.Format("CREATE TABLE {0} (\"{1}\" {2})", table_name, type.column.ToUpper(), stype);
            else if (dbtype == EnumDbType.MDB)
                command.CommandText = String.Format("CREATE TABLE {0} ([{1}] {2})", table_name, type.column, stype);
            else
                command.CommandText = String.Format("CREATE TABLE {0} ({1} {2})", table_name, type.column, stype);

            command.ExecuteNonQuery();
        }

        static void MakeCommandColumnAdd(CommonDbConnection conn, EnumDbType dbtype, string tablename, ColumnList col)
        {
            string var_type;

            var_type = DbTool.MakeVarType(dbtype, col.type, col.size);

            string query;

            // 오라클에서 ""로 싸인 경우 소문자가 포함되면 "" 이 컬럼명으로 같이 포함되므로 컬럼명은 대문자로 보관한다. 9.5.2 부터 수정

            if (dbtype == EnumDbType.Oracle || dbtype == EnumDbType.Tibero)
                query = String.Format("ALTER TABLE {0} ADD (\"{1}\" {2})", tablename, col.column.ToUpper(), var_type);
            else if (dbtype == EnumDbType.DB2)
                query = String.Format("ALTER TABLE {0} ADD COLUMN \"{1}\" {2}", tablename, col.column, var_type);
            else if (dbtype == EnumDbType.SQLServerCE)
                query = String.Format("ALTER TABLE {0} ADD COLUMN \"{1}\" {2}", tablename, col.column, var_type);
            else
                query = String.Format("ALTER TABLE {0} ADD {1} {2}", tablename, DbTool.Field(dbtype, col.column), var_type);

            CommonDbCommand command = new CommonDbCommand(conn.connType);

            command.Connection = conn;
            command.CommandText = query;

            command.ExecuteNonQuery();
        }

        public string sErrorMessage = "";

		public bool Check(CommonDbConnection conn, EnumDbType dbtype, string table_name)
		{
			string query;

            if (dbtype == EnumDbType.Oracle || dbtype == EnumDbType.Tibero)
				query = String.Format("SELECT * FROM {0} WHERE rownum=1", table_name);
            else if (dbtype == EnumDbType.SQLServerCE)
                query = String.Format("SELECT TOP(1) * FROM {0}", table_name);
            else if (dbtype == EnumDbType.MySQL)
                query = String.Format("SELECT * FROM {0} LIMIT 1", table_name); // MySQL은 TOP이 다르다.
			else
				query = String.Format("SELECT TOP 1 * FROM {0}", table_name);

			CommonDbDataAdapter ad = new CommonDbDataAdapter(query, conn);

			DataSet ds = new DataSet();

			try 
			{
				ad.Fill(ds, table_name);
			}
			catch {
                
			}

			ColumnList col;

            try
            {
                if (ds.Tables.Count == 0)	// no table
                {
                    col = (ColumnList)this.listColumn[0];
                    NewTable(conn, dbtype, table_name, col);
                    for (int i = 1; i < this.listColumn.Count; i++)
                    {
                        col = (ColumnList)this.listColumn[i];
                        MakeCommandColumnAdd(conn, dbtype, table_name, col);
                    }
                }
                else
                {
                    for (int i = 0; i < this.listColumn.Count; i++)
                    {
                        col = (ColumnList)this.listColumn[i];
                        if (ds.Tables[0].Columns.IndexOf(col.column) == -1)
                        {
                            MakeCommandColumnAdd(conn, dbtype, table_name, col);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.sErrorMessage = exception.Message;
                return false;
            }

            return true;
		}

		public void Create(CommonDbConnection conn, EnumDbType dbtype, string table_name)
		{
			ColumnList col;

			col = (ColumnList)this.listColumn[0];
			NewTable(conn, dbtype, table_name, col);
			for(int i = 1; i < this.listColumn.Count; i++) 
			{
				col = (ColumnList)this.listColumn[i];
				MakeCommandColumnAdd(conn, dbtype, table_name, col);
			}
		}
	}
}

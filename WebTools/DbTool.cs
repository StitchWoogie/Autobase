using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace WebTools
{
    public class DbTool
    {
        string sDsn;

        public DbTool(string dsn)
        {
            sDsn = dsn;
        }

        public DataTable GetDataTable(string query)
        {
            SqlConnection conn = new SqlConnection(sDsn);
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();

            try
            {
                adapter.Fill(dt);
            }
            catch
            {

            }
            conn.Close();

            return dt;
        }

        public DataRow GetDataRow(string query)
        {
            DataTable dt = GetDataTable(query);

            if (dt.Rows.Count == 0) return null;

            return dt.Rows[0];
        }

        /// <summary>
        /// where문에 해킹코드가 삽입될 수 있기 때문에 SqlCommand를 이용하여 Parameter를 더해서 가져오면 해킹을 방지할 수 있다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataRow GetDataRowOnlyOne(SqlCommand cmd)
        {
            SqlConnection conn = new SqlConnection(sDsn);

            conn.Open();
            cmd.Connection = conn;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            conn.Close();

            if (dt.Rows.Count != 1) return null;

            return dt.Rows[0];
        }

        public bool Execute(string query)
        {
            SqlConnection conn = new SqlConnection(sDsn);
            conn.Open();
            SqlCommand command = new SqlCommand(query, conn);
            command.ExecuteNonQuery();
            conn.Close();

            return true;
        }

        public static string AddApostrophe(string source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == '\'')
                {
                    goto change;
                }
            }
            return source;

        change:
            string target = "";
            for (int i = 0; i < source.Length; i++)
            {
                target += source[i];
                if (source[i] == '\'')
                {
                    target += source[i];
                }
            }

            return target;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using NetTools;

namespace WebTools
{
    public class ConfigTable
    {
        string sDsn;
        public string sColumnName = "ConfigItem";   // ConfigName 인곳도 있다.

        public ConfigTable(string dsn)
        {
            sDsn = dsn;
        }

        DataRow GetDataRow(string query)
        {
            try
            {
                SqlConnection conn = new SqlConnection(sDsn);
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();

                adapter.Fill(dt);
                conn.Close();

                if (dt.Rows.Count == 0) return null;

                return dt.Rows[0];
            }
            catch
            {
                return null;
            }
        }

        public string GetValue(string itemname)
        {
            string query = String.Format("SELECT * FROM ConfigTable WHERE {1}='{0}'", itemname, sColumnName);

            DataRow row = GetDataRow(query);
            if (row == null) return "";

            return row["ConfigValue"].ToString();
        }

        public bool IsIncludeInSemicolonItem(string itemname, string compare)
        {
            string value = GetValue(itemname);

            return IsStringIncludeInSemicolonString(value, compare);
        }

        /// <summary>
        /// 세미콜론으로 구분된 문자열에서 해당되는 문자열이 존재하는 가를 검사한다. 대소분자 구분이 없다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool IsStringIncludeInSemicolonString(string source, string val)
        {
            CommaBlockString comma = new CommaBlockString();
            comma.SetBlockCode(';');

            string buf;
            comma.Set(source);
            while (true)
            {
                if (comma.IsEOS()) break;
                buf = comma.GetString();

                if (String.Compare(buf, val, true) == 0) return true;
            }

            return false;
        }

        bool Execute(string query)
        {
            SqlConnection conn = new SqlConnection(sDsn);
            conn.Open();
            SqlCommand command = new SqlCommand(query, conn);
            command.ExecuteNonQuery();
            conn.Close();

            return true;
        }

        public void SetValue(string item_name, string item_value)
        {
            string query = String.Format("SELECT * FROM ConfigTable WHERE {1}='{0}'", item_name, sColumnName);

            DataRow row = GetDataRow(query);
            if (row == null)
            {
                query = String.Format("INSERT INTO ConfigTable ({2},ConfigValue) VALUES ('{0}','{1}')", item_name, item_value, sColumnName);
            }
            else
            {
                query = String.Format("UPDATE ConfigTable SET ConfigValue='{1}' WHERE {2}='{0}'", item_name, item_value, sColumnName);
            }

            Execute(query);
        }
    }
}

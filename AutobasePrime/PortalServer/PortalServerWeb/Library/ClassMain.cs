using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using AutoLibLocal;

namespace PortalServerWeb.Library
{
    public class ClassMain
    {
        public static DataTable GetDataTable(string query)
        {
            if (!ConfigWeb.UseLog()) return null;

            ConnectionString dsn = new ConnectionString();

            dsn.dsn = ConfigWeb.LogDsn();
            dsn.dbConnectionType = EnumDbConnectionType.OleDb;
            dsn.dbtype = EnumDbType.SQLServerCE;

            CommonDbConnection conn = new CommonDbConnection(EnumDbConnectionType.OleDb, dsn.dsn, false);
            //conn.ConnectionString = dsn.dsn;

            try
            {
                conn.Open();
            }
            catch 
            {
                return null;
            }

            DataTable dt = new DataTable();

            CommonDbDataAdapter adapter = new CommonDbDataAdapter(query, conn);
            adapter.Fill(dt);

            return dt;
        }

        static bool MakeTableLog(CommonDbConnection conn, ConnectionString dsn)
        {
            CheckTable check = new CheckTable();

            check.AddColumn("LogTime", EnumDbDataType.DateTime, 0);
            check.AddColumn("LogType", EnumDbDataType.String, 20);
            check.AddColumn("Message", EnumDbDataType.String, 256);

            return check.Check(conn, dsn.dbtype, "ScadaLog");
        }

        static bool MakeTableConfig(CommonDbConnection conn, ConnectionString dsn)
        {
            CheckTable check = new CheckTable();

            check.AddColumn("Item", EnumDbDataType.String, 50);
            check.AddColumn("Value", EnumDbDataType.String, 256);

            return check.Check(conn, dsn.dbtype, "ScadaConfig");
        }

        static bool MakeTableAll(CommonDbConnection conn, ConnectionString dsn)
        {
            bool flag = true;

            if(!MakeTableLog(conn, dsn))
                flag = false;
            if (!MakeTableConfig(conn, dsn))
                flag = false;

            return flag;
        }

        public static bool SaveLog(out string err_msg, EnumLogType log_type, string format, params object[] args)
        {
            err_msg = "";

            if (!ConfigWeb.UseLog()) return true;

            ConnectionString dsn = new ConnectionString();

            dsn.dsn = ConfigWeb.LogDsn();
            dsn.dbConnectionType = EnumDbConnectionType.OleDb;
            dsn.dbtype = EnumDbType.SQLServerCE;

            CommonDbConnection conn = new CommonDbConnection(EnumDbConnectionType.OleDb, dsn.dsn, false);
            //conn.ConnectionString = dsn.dsn;

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                err_msg = String.Format("Database Connection(Web.Config->LogDsn) error.\nMessage={0}", exception.Message);
                return false;
            }

            MakeTableAll(conn, dsn);

            string log_msg = String.Format(format, args);

            string query = String.Format("INSERT INTO ScadaLog (LogTime,LogType,Message) VALUES(getdate(),'{0}','{1}')", log_type.ToString(), log_msg);

            CommonDbCommand cmd = new CommonDbCommand(query, conn);
            cmd.ExecuteNonQuery();

            conn.Close();

            return true;
        }
    }

    public enum EnumLogType
    {
        Normal = 0,
        LogIn = 1,
        Write = 2,
        Error = 3,
    }
}

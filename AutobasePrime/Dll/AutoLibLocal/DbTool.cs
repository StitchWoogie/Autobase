using System;
using System.IO;
using NetTools.OldDefine;
using System.Text;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for DbTool.
	/// </summary>
	public class DbTool
	{
        public static ConnectionStringList dsnList = new ConnectionStringList();

		static DbTool()
		{
			//
			// TODO: Add constructor logic here
			//
            dsnList.ConnectionStringLoad();
		}

        /// <summary>
        /// 알맞는 필드 문자를 만든다.  MDB에서는 .을 _로 바꾸어 준다.  필드 앞뒤부호는 Field() 함수를 사용한다.
        /// </summary>
        /// <param name="dbtype"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string ChangeFieldChar(EnumDbType dbtype, string field)
        {
            // 확인결과 SQL CE는 .을 허용한다.

            if (dbtype == EnumDbType.MDB)
            {
                int index = field.IndexOf('.'); // .은 _로 바꾸어준다.
                if (index != -1)
                {
                    StringBuilder s = new StringBuilder();
                    for (int i = 0; i < field.Length; i++)
                    {
                        if (field[i] == '.')
                            s.Append('_');
                        else
                            s.Append(field[i]);
                    }
                    field = s.ToString();
                }
            }
            
            return field;
        }

		/// <summary>
		/// Db Server 의 종류에 따라서 맞는 필드 스트링을 만든다.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="field"></param>
		/// <returns>좌우로 스트링이 사</returns>
		public static string Field(EnumDbType dbtype, string field)
		{
            field = ChangeFieldChar(dbtype, field);  // 적당한 필드 문자를 만들어 준다.

            if (dbtype == EnumDbType.Oracle || dbtype == EnumDbType.Tibero)
            {
                //return "\"" + field + "\"";
                return "\"" + field.ToUpper() + "\"";       // 컬럼명을 대문자로 만들기 때문에 명령어에서도 대문자로 만들어준다. 10.1.1
            }
            else if (dbtype == EnumDbType.MySQL)
                return "`" + field + "`";
            else if (dbtype == EnumDbType.DB2)
                return field;                       // 스페이스나 특수한 이름은 컬럼명 자체에 " " 가 삽입되므로 ""를 붙일 필요가 없다.
            else
                return "[" + field + "]";   // SQL, SQL CE, MDB는 []로 된다.
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
            else if (dbtype == EnumDbType.DB2)
            {
                imsi = String.Format("'{0}-{1}-{2} {3:00}:{4:00}:{5:00}'", year, mon, day, hour, min, sec); //시간을 꼭 두자리로 채워야 한다.
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

		public static ConnectionString GetConnectionString(string dsn)
		{
			return dsnList.GetConnection(dsn);
		}

        public static void ChangeConnectionString(string dsn, string text)
        {
            dsnList.ChangeConnection(dsn, text);
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
		}

        public static string MakeVarType(EnumDbType dbtype, EnumDbDataType type, int size)
        {
            string var_type;
            if (type == EnumDbDataType.Integer)
                var_type = "Integer";
            else if (type == EnumDbDataType.Float)
                var_type = "FLOAT";
            else if (type == EnumDbDataType.String)
            {
                if (dbtype == EnumDbType.Oracle || dbtype == EnumDbType.Tibero)
                    var_type = String.Format("VARCHAR2({0})", size);   //var_type = String.Format("NVARCHAR2({0})", size);   // 원래는 VARCHAR2였는데 다국어가 섞인 글자는 안되어서 NVARCHAR2로 변경
                else if (dbtype == EnumDbType.SQLServerCE)
                    var_type = String.Format("nvarchar({0})", size);
                else
                    var_type = String.Format("VARCHAR({0})", size);
            }
            else if (type == EnumDbDataType.DateTime)
            {
                if (dbtype == EnumDbType.Oracle || dbtype == EnumDbType.Tibero)
                    var_type = "DATE";
                else if (dbtype == EnumDbType.DB2)
                    var_type = "TIMESTAMP";
                else
                    var_type = "DATETIME";
            }
            else
            {
                var_type = "Integer";
            }

            return var_type;
        }

        /// <summary>
        /// 문자열에 ' 같은 것이 포함되면 저장할 수 없으므로 적당히 바꿔준다.
        /// </summary>
        /// <param name="dbtype"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertStringValue(EnumDbType dbtype, string source)
        {
            if (dbtype == EnumDbType.MDB)
            {
                if (source.IndexOf('\'') == -1)
                {
                    return source;
                }
                else
                {
                    StringBuilder s = new StringBuilder();
                    for (int i = 0; i < source.Length; i++)
                    {
                        if (source[i] == '\'')
                        {
                            s.Append('\'');
                        }
                        s.Append(source[i]);
                    }
                    return s.ToString();
                }
            }
            else
            {
                return source;
            }
        }
		
	}
}

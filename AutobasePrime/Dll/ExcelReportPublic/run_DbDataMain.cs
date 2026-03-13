using System;
using System.Data;	
using DatabaseConnection;
using DialogAddition;
using AutoLibLocal;
using NetTools;
using System.Threading.Tasks;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for run_DbDataMain.
	/// </summary>
	public class run_DbDataMain
	{
		memberConfigStruct	member;
		BasicRptTool.eDbDataType enumDbDataType;
		int nElementNum;
		int nColumn;
		int nRow;
		string[] sFieldData;
		
		public run_DbDataMain(memberConfigStruct mem, int num, int elementNum, int columnCount, int rowCount, string[] sData)
		{
			//
			// TODO: Add constructor logic here
			//
			member = mem;
			enumDbDataType = (BasicRptTool.eDbDataType)num;
			nElementNum = elementNum;
			nColumn = columnCount;
			nRow = rowCount;
			sFieldData = sData;
		}

        public string run(ConnectionStringList dsnList)
        {
			int					upTime = 0, index = 0, dataTypeSort, displayType = 0;
			bool				flag;
			ConnectionString	dsn;
			
			switch(enumDbDataType) 
			{
				case BasicRptTool.eDbDataType.DB_DATA :
					if(nElementNum < 4) return member.noneDataString;
					if(sFieldData[3].Length < 2) return member.noneDataString;					
					return dbOneNormalDataValueRead(dsnList);
					/*dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[3], ref index);
					if(dataTypeSort == -1) return member.noneDataString;

					dsn = dsnList.GetConnection(member.dsnName);
					if(sFieldData[3].Length > 2 || nElementNum <= 4)		// HH00 형태이거나, 데이터요소 수가 3개일 때...
						return DB_Data_Read_Write.dbDataRead(dsn, sFieldData[1], sFieldData[2], member.sDateColumnName, member.nTimeFormat, null, BasicRptTool.reportConfig.dt, dataTypeSort, 0, index, member.noneDataString);
					else 
					{
						try 
						{
							index = ConvertTool.ToInt32(sFieldData[4]);
							if(nElementNum <= 7) // 기존 시간 데이터 형식
							{
								if(nElementNum >= 6) upTime = ConvertTool.ToInt32(sFieldData[5]);
							}
							else				// 현재 시간 데이터 형식
							{
								if(nElementNum >= 7) upTime = ConvertTool.ToInt32(sFieldData[6]);
								if(nElementNum >= 10) displayType = ConvertTool.ToInt32(sFieldData[9]);
								if(nElementNum >= 11) sOptionWhere = sFieldData[10];
							}
						}
						catch 
						{
						}
						if(displayType == 1) // 시간 자료형식
						{
							sData = DB_Data_Read_Write.dbDataRead(dsn, sFieldData[1], sFieldData[2], member.sDateColumnName, member.nTimeFormat, sOptionWhere, BasicRptTool.reportConfig.dt, dataTypeSort, upTime, index, member.noneDataString);
							if(sData == member.noneDataString) return sData;
							try 
							{
								index = (int)ConvertTool.ToDouble(sData);
							}
							catch 
							{
								return sData;
							}
							return String.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}", index/3600, (index/60) % 60, index % 60);
						}
						return DB_Data_Read_Write.dbDataRead(dsn, sFieldData[1], sFieldData[2], member.sDateColumnName, member.nTimeFormat, sOptionWhere, BasicRptTool.reportConfig.dt, dataTypeSort, upTime, index, member.noneDataString);						
					}*/
				case BasicRptTool.eDbDataType.DB_CURRENT :
					if(nElementNum < 3) return member.noneDataString;
					dsn = dsnList.GetConnection(member.dsnName);
					return  DB_Data_Read_Write.dbDataCurrentValueRead(dsn, sFieldData[1], "TAG", sFieldData[2], "CURR", member.noneDataString);
                case BasicRptTool.eDbDataType.DB_PI_ADDITION:
                    if (nElementNum < 4) return member.noneDataString;
                    double val = 0.0;
                    Addition add = new Addition();
                    index = getSaveListPos(sFieldData[3]);
                    flag = add.GetAddition(sFieldData[1], sFieldData[2], getSaveListPosToEnum(index), out val);
                    if (flag == false) return member.noneDataString;
                    return val.ToString();
                case BasicRptTool.eDbDataType.MULTI_COLUMN_ROW :
					if(nElementNum < 13) return member.noneDataString;
					return dbMultiColumnRowDataTableValueRead(nColumn, nRow, member.noneDataString);
				case BasicRptTool.eDbDataType.DB_MULTI_DATA :
					if(nElementNum < 8) return member.noneDataString;
					if(sFieldData[3].Length < 2) return member.noneDataString;
					dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[3], ref index);
					if(dataTypeSort == -1) return member.noneDataString;
					try 
					{
						index = ConvertTool.ToInt32(sFieldData[4]);		// Start Curr Time
						upTime = ConvertTool.ToInt32(sFieldData[6]);	// Start Up Time	
						if(nElementNum >= 10) displayType = ConvertTool.ToInt32(sFieldData[9]);
					}
					catch 
					{
					}
					return dbMultiRowDataTableValueRead(nRow, BasicRptTool.reportConfig.dt, dataTypeSort, index, upTime, displayType, member.noneDataString);
				case BasicRptTool.eDbDataType.DB_AI_MAXTIME :
					if(nElementNum < 10) return member.noneDataString;
					return dbAiMaxMinMultiColumnDataTableValueRead(nColumn, member.noneDataString, true);
				case BasicRptTool.eDbDataType.DB_AI_MINTIME :
					if(nElementNum < 10) return member.noneDataString;
					return dbAiMaxMinMultiColumnDataTableValueRead(nColumn, member.noneDataString, false);
				case BasicRptTool.eDbDataType.DB_MULTI_QUERY :
					if(nElementNum < 2) return member.noneDataString;
					return dbMultiQueryInputValueRead(nColumn, nRow, member.noneDataString);
				default : return member.noneDataString;
			}			
		}

        // 변수를 사용한 문자열인가를 검사한다.
        public static string GetStringOrgOrVar(string org)
        {
            if (org.Length > 0 && org[0] == '$')
            {
                return GlobalVar.GetVarValue(org.Substring(1));
            }
            else
            {
                return org;
            }
        }

        string dbOneNormalDataValueRead(ConnectionStringList dsnList)
        {
            int index = 0, dataTypeSort;

            dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[3], ref index);
            if (dataTypeSort == -1) return member.noneDataString;

            int i, upTime = 0, displayType = 0;
            string sData, sOptionWhere = "", type;
            ConnectionString dsn;

            dsn = dsnList.GetConnection(member.dsnName);
            if (sFieldData[3].Length > 2 || nElementNum <= 4)       // HH00 형태이거나, 데이터요소 수가 3개일 때...
            {
                BasicRptTool.dTableBuf = DB_Data_Read_Write.dbDataRead(dsn, sFieldData[1], sFieldData[2], member.sDateColumnName, member.nTimeFormat, null, BasicRptTool.reportConfig.dt, dataTypeSort, 0, index);
            }
            else
            {
                try
                {
                    index = ConvertTool.ToInt32(sFieldData[4]);
                    if (nElementNum <= 7) // 기존 시간 데이터 형식
                    {
                        if (nElementNum >= 6) upTime = ConvertTool.ToInt32(sFieldData[5]);
                    }
                    else                // 현재 시간 데이터 형식
                    {
                        if (nElementNum >= 7) upTime = ConvertTool.ToInt32(sFieldData[6]);
                        if (nElementNum >= 10) displayType = ConvertTool.ToInt32(sFieldData[9]);
                        if (nElementNum >= 11) sOptionWhere = GetStringOrgOrVar(sFieldData[10]);
                    }
                }
                catch
                {
                }
                BasicRptTool.dTableBuf = DB_Data_Read_Write.dbDataRead(dsn, sFieldData[1], sFieldData[2], member.sDateColumnName, member.nTimeFormat, sOptionWhere, BasicRptTool.reportConfig.dt, dataTypeSort, upTime, index);
            }
            if (BasicRptTool.dTableBuf == null) return member.noneDataString;
            DataRow row = BasicRptTool.dTableBuf.Rows[0];

            try
            {
                sData = row[sFieldData[2]].ToString();
                if (sData == null || sData.Length <= 0) return "0";
            }
            catch
            {
                return member.noneDataString;
            }

            try
            {
                if (displayType == 1) // 시간 자료형식
                {
                    i = (int)ConvertTool.ToDouble(sData);
                    return String.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}", i / 3600, (i / 60) % 60, i % 60);
                }

                type = BasicRptTool.dTableBuf.Columns[sFieldData[2]].DataType.ToString();
                if (type == "System.DateTime")
                {
                    DateTime t = ConvertTool.ToDateTime(sData);
                    type = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
                    return type;
                }
                else
                {
                    return sData;
                }
            }
            catch
            {
            }
            return member.noneDataString;
        }

        string dbMultiColumnRowDataTableValueRead(int nColumn, int nRow, string noneDataString)
		{
			if(BasicRptTool.dTableBuf == null) return noneDataString;
			if(BasicRptTool.dTableBuf.Rows.Count <= 0) return noneDataString;
			if(nRow < 0 || BasicRptTool.dTableBuf.Rows.Count <= nRow) return noneDataString;
			if(nColumn < 0 || 256 <= nColumn) return noneDataString;
			if(nColumn >= BasicRptTool.nMultiLineFieldCount) return noneDataString;

			DataRow row = BasicRptTool.dTableBuf.Rows[nRow];
			try 
			{
				string	buf = row[sFieldData[nColumn+12]].ToString();
				if(buf == null || buf.Length <= 0) return "0";
				string type = BasicRptTool.dTableBuf.Columns[sFieldData[nColumn+12]].DataType.ToString();
				
				if(type == "System.DateTime") 
				{
					DateTime t = ConvertTool.ToDateTime(buf);
					type = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);					
					return type;
				}
				else
				{
					return buf;
				}
			}
			catch 
			{
				return noneDataString;
			}
		}

		bool isMatchedTime(DateTime t, string timeBuf)
		{
			if(member.nTimeFormat == 1) 
			{
				string		buf = String.Format("{0,4:d04}{1,2:d02}{2,2:d02}{3,2:d02}{4,2:d02}{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);

				if(buf == timeBuf) return true;
			}
			else 
			{
				if(t == ConvertTool.ToDateTime(timeBuf)) return true;
			}
			return false;
		}

		string dbMultiRowDataTableValueRead(int nRow, DateTime basicDt, int dataTypeSort, int startCurrTime, int startUpTime, int displayType, string noneDataString)  // DB 여러줄 자료 값 return
		{
			if(BasicRptTool.dTableBuf == null) return noneDataString;
			if(BasicRptTool.dTableBuf.Columns.Count <= 1) return noneDataString;

			int		nRowCount = BasicRptTool.dTableBuf.Rows.Count;
			if(nRowCount <= 0) return noneDataString;

			DataRow		row;
			DateTime	dt;
			string		type, buf;
			int			i;
			
			dt = BasicRptTool.getCurrentMultiRowDateTime(basicDt, dataTypeSort, startUpTime, startCurrTime, nRow);
			for(i = 0; i < nRowCount; i++) 
			{
				row = BasicRptTool.dTableBuf.Rows[i];
				
				try 
				{
					buf = row[member.sDateColumnName].ToString();
					if(isMatchedTime(dt, buf)) 
					{
						if(displayType == 1) // 시간 자료형식
						{
							buf = row[sFieldData[2]].ToString();
							if(buf == member.noneDataString) return member.noneDataString;
							i = (int)ConvertTool.ToDouble(buf);						
							return String.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}", i/3600, (i/60) % 60, i % 60);
						}

						type = BasicRptTool.dTableBuf.Columns[sFieldData[2]].DataType.ToString();
						buf = row[sFieldData[2]].ToString();

						if(type == "System.DateTime") 
						{
							DateTime t = ConvertTool.ToDateTime(buf);
							type = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
							return type;
						}
						else
						{
							if(buf == null || buf.Length <= 0) return " ";
							return buf;
						}
					}
				}
				catch 
				{

				}
			}
			return noneDataString;			
		}

		public UInt32 dbMultiColumnRowDataRead(ConnectionStringList dsnList)
		{
			ConnectionString	dsn;
			
			BasicRptTool.nMultiLineFieldCount = 0;
			if(nElementNum < 13) return 0;
			BasicRptTool.nMultiLineFieldCount = nElementNum-12;
			dsn = dsnList.GetConnection(member.dsnName);
			BasicRptTool.dTableBuf = DB_Data_Read_Write.dbMultiColumnRowRead(dsn, member.sDateColumnName, member.nTimeFormat, nElementNum, sFieldData, BasicRptTool.reportConfig.dt);
			if(BasicRptTool.dTableBuf == null) return 0;
			return (UInt32)BasicRptTool.dTableBuf.Rows.Count;
		}

		public UInt32 dbMultiRowDataRead(ConnectionStringList dsnList)
		{
			ConnectionString	dsn;
			int					dataTypeSort, index = 0, startCurrTime = 0, endCurrTime = 0, startUpTime = 0, endUpTime = 0;
			UInt32				nLineCount;
						
			BasicRptTool.nMultiLineFieldCount = 0;
			if(nElementNum < 8) return 0;
			if(sFieldData[3].Length < 2) return 0;
			dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[3], ref index);
			if(dataTypeSort == -1) return 0;

			string		sOptionWhere = "";			
			try 
			{
				startCurrTime = ConvertTool.ToInt32(sFieldData[4]);
				endCurrTime = ConvertTool.ToInt32(sFieldData[5]);
				startUpTime = ConvertTool.ToInt32(sFieldData[6]);
				endUpTime = ConvertTool.ToInt32(sFieldData[7]);
                if (nElementNum >= 11) sOptionWhere = GetStringOrgOrVar(sFieldData[10]);
			}
			catch 
			{

			}
			BasicRptTool.nMultiLineFieldCount = 1;
			nLineCount = BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, startCurrTime, endCurrTime, startUpTime, endUpTime);
			dsn = dsnList.GetConnection(member.dsnName);
			BasicRptTool.dTableBuf =  DB_Data_Read_Write.dbMultiColumnRead(dsn, sFieldData[1], sFieldData[2], member.sDateColumnName, member.nTimeFormat, sOptionWhere, BasicRptTool.reportConfig.dt, dataTypeSort, startCurrTime, endCurrTime, startUpTime, endUpTime);
			//if(BasicRptTool.dTableBuf == null) return 0;
			return nLineCount;
		}

		public UInt32 dbMultiQueryDataRead(ConnectionStringList dsnList)
		{
			ConnectionString	dsn;
			
			BasicRptTool.nMultiLineFieldCount = 0;
			if(nElementNum < 2) return 0;			
			dsn = dsnList.GetConnection(member.dsnName);
			BasicRptTool.dTableBuf = DB_Data_Read_Write.dbMultiQueryDataRead(dsn, sFieldData[1]);
			if(BasicRptTool.dTableBuf == null) return 0;
			BasicRptTool.nMultiLineFieldCount = BasicRptTool.dTableBuf.Columns.Count;
			return (UInt32)BasicRptTool.dTableBuf.Rows.Count;
		}

		public int getSaveListPos(string data)
		{
			int			i;

			for(i = 0; i < 8; i++) 
			{
				if(data == BasicRptTool.sSaveListRealBuf[i]) return i;				
			}
			return 0;
		}

		public DialogAddition.EnumAdditionType getSaveListPosToEnum(int pos)
		{
			switch(pos) 
			{
				case 1 : return DialogAddition.EnumAdditionType.LastHour;
				case 2 : return DialogAddition.EnumAdditionType.ThisDay;
				case 3 : return DialogAddition.EnumAdditionType.LastDay;
				case 4 : return DialogAddition.EnumAdditionType.ThisMonth;
				case 5 : return DialogAddition.EnumAdditionType.LastMonth;
				case 6 : return DialogAddition.EnumAdditionType.ThisYear;
				case 7 : return DialogAddition.EnumAdditionType.LastYear;
				default: return DialogAddition.EnumAdditionType.ThisHour;
			}
			
		}

		public UInt32 dbAiMaxMinDataRead(ConnectionStringList dsnList)
		{
			ConnectionString	dsn;
			int					dataTypeSort, index = 0, startCurrTime = 0, endCurrTime = 0, startUpTime = 0, endUpTime = 0;
						
			BasicRptTool.nMultiLineFieldCount = 0;
			if(nElementNum < 10) return 0;
			if(sFieldData[3].Length < 2) return 0;
			dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[3], ref index);
			if(dataTypeSort == -1) return 0;

			string		sOptionWhere = "";			
			try 
			{
				startCurrTime = ConvertTool.ToInt32(sFieldData[4]);
				endCurrTime = ConvertTool.ToInt32(sFieldData[5]);
				startUpTime = ConvertTool.ToInt32(sFieldData[6]);
				endUpTime = ConvertTool.ToInt32(sFieldData[7]);
                if (nElementNum >= 10) sOptionWhere = GetStringOrgOrVar(sFieldData[9]);
				
			}
			catch 
			{
			}
			BasicRptTool.nMultiLineFieldCount = nElementNum-10;
			dsn = dsnList.GetConnection(member.dsnName);
			BasicRptTool.dTableBuf = DB_Data_Read_Write.dbAiMaxMinMultiColumnRead(dsn, sFieldData[1], sFieldData[2], member.sDateColumnName, member.nTimeFormat, sOptionWhere, BasicRptTool.reportConfig.dt, dataTypeSort, startCurrTime, endCurrTime, startUpTime, endUpTime);
			if(BasicRptTool.dTableBuf == null || BasicRptTool.dTableBuf.Rows.Count <= 0) return 0;
			return 1;
		}

		int getMaxMinValueColumnPos(string ColumnName, bool bMax)
		{
			int			i, pos = 0;
			DataRow		row;
			string		buf;
			double		val, val2 = 0;
			bool		flag = false;

			for(i = 0; i < BasicRptTool.dTableBuf.Rows.Count; i++) 
			{
				try 
				{
					row = BasicRptTool.dTableBuf.Rows[i];
					buf = row[ColumnName].ToString();
					if(buf == null || buf.Length <= 0) continue;					
					val = ConvertTool.ToDouble(buf);
					if(flag == false) 
					{
						val2 = val;
						flag = true;
						continue;
					}
					if(bMax) 
					{
						if(val > val2) 
						{
							val2 = val;
							pos = i;
						}
					}
					else 
					{
						if(val < val2) 
						{
							val2 = val;
							pos = i;
						}
					}
				}
				catch 
				{					
				}
			}
			return pos;
		}

		int getMaxMinDateTimeColumnPos(string ColumnName, bool bMax)
		{
			int			i, pos = 0;
			DataRow		row;
			string		buf;
			DateTime	t1, t2 = DateTime.Now;
			bool		flag = false;

			for(i = 0; i < BasicRptTool.dTableBuf.Rows.Count; i++) 
			{
				try 
				{
					row = BasicRptTool.dTableBuf.Rows[i];
					buf = row[ColumnName].ToString();
					if(buf == null || buf.Length <= 0) continue;					
					t1 = ConvertTool.ToDateTime(buf);
					if(flag == false) 
					{
						t2 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second, t1.Millisecond);
						flag = true;
						continue;
					}
					if(bMax) 
					{
						if(t1 > t2) 
						{
							t2 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second, t1.Millisecond);
							pos = i;
						}
					}
					else 
					{
						if(t1 < t2) 
						{
							t2 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second, t1.Millisecond);
							pos = i;
						}
					}
				}
				catch 
				{
				}
			}
			return pos;
		}

		int getMaxMinStringColumnPos(string ColumnName, bool bMax)
		{
			int			i, pos = 0;
			DataRow		row;
			string		buf, buf2 = "";
			bool		flag = false;

			for(i = 0; i < BasicRptTool.dTableBuf.Rows.Count; i++) 
			{
				try 
				{
					row = BasicRptTool.dTableBuf.Rows[i];
					buf = row[ColumnName].ToString();
					if(buf == null || buf.Length <= 0) continue;					
					if(flag == false) 
					{
						buf2 = buf;
						flag = true;
						continue;
					}
					if(bMax) 
					{
						if(string.Compare(buf, buf2) > 0) 
						{
							buf2 = buf;
							pos = i;
						}
					}
					else 
					{
						if(string.Compare(buf, buf2) < 0) 
						{
							buf2 = buf;
							pos = i;
						}
					}
				}
				catch 
				{
				}
			}
			return pos;
		}

		int getMaxMinColumnNo(string ColumnName, bool bMax)
		{
			string		type;
			
			type = BasicRptTool.dTableBuf.Columns[ColumnName].DataType.ToString();
			
			if(type == "System.DateTime") 
			{
				return getMaxMinDateTimeColumnPos(ColumnName, bMax);
			}
			else if (type == "System.String")
			{
				return getMaxMinStringColumnPos(ColumnName, bMax);
			}
			else 
			{
				return getMaxMinValueColumnPos(ColumnName, bMax);
			}
		}

		string dbAiMaxMinMultiColumnDataTableValueRead(int nColumn, string noneDataString, bool bMax)
		{
			if(BasicRptTool.dTableBuf == null) return noneDataString;
			if(BasicRptTool.dTableBuf.Rows.Count <= 0) return noneDataString;
			if(nColumn < 0 || 256 <= nColumn) return noneDataString;
			if(nColumn >= BasicRptTool.nMultiLineFieldCount) return noneDataString;

			if(nColumn == 0) 
			{
				BasicRptTool.nDbMaxMinTimeColumnPos = getMaxMinColumnNo(sFieldData[2], bMax);
			}
			if(BasicRptTool.nDbMaxMinTimeColumnPos < 0 ||
				BasicRptTool.nDbMaxMinTimeColumnPos >= BasicRptTool.dTableBuf.Rows.Count) return noneDataString;	// 일치하는 최대값이 없다

			DataRow row = BasicRptTool.dTableBuf.Rows[BasicRptTool.nDbMaxMinTimeColumnPos];

			string type, buf;
			try 
			{
				type = BasicRptTool.dTableBuf.Columns[sFieldData[nColumn+10]].DataType.ToString();
				buf = row[sFieldData[nColumn+10]].ToString();

				if(type == "System.DateTime") 
				{
					DateTime t = ConvertTool.ToDateTime(buf);
					type = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);					
					return type;
				}
				else if(member.nTimeFormat == 1 && sFieldData[nColumn+10] == member.sDateColumnName)
				{
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", ConvertTool.ToInt16(buf.Substring(0, 4)), ConvertTool.ToInt16(buf.Substring(4, 2)), ConvertTool.ToInt16(buf.Substring(6, 2)), ConvertTool.ToInt16(buf.Substring(8, 2)), ConvertTool.ToInt16(buf.Substring(10, 2)), ConvertTool.ToInt16(buf.Substring(12, 2)));
				}
				else 
				{
					if(buf == null || buf.Length <= 0) return "0";
					return buf;
				}
			}
			catch 
			{				
			}
			return noneDataString;
		}

		string dbMultiQueryInputValueRead(int nColumn, int nRow, string noneDataString)
		{
			if(BasicRptTool.dTableBuf == null) return noneDataString;
			if(BasicRptTool.dTableBuf.Rows.Count <= 0) return noneDataString;
			if(nRow < 0 || BasicRptTool.dTableBuf.Rows.Count <= nRow) return noneDataString;
			if(nColumn < 0 || 1024 <= nColumn) return noneDataString;
			if(nColumn >= BasicRptTool.nMultiLineFieldCount) return noneDataString;

			DataRow row = BasicRptTool.dTableBuf.Rows[nRow];
			string	buf, type;
			try 
			{
				type = BasicRptTool.dTableBuf.Columns[nColumn].DataType.ToString();
				buf = row[nColumn].ToString();

				if(type == "System.DateTime") 
				{
					DateTime t = ConvertTool.ToDateTime(buf);
					type = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);					
					return type;
				}
				//else if(member.nTimeFormat == 1 && sFieldData[nColumn+10] == member.sDateColumnName)
				//{
				//	return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", ConvertTool.ToInt16(buf.Substring(0, 4)), ConvertTool.ToInt16(buf.Substring(4, 2)), ConvertTool.ToInt16(buf.Substring(6, 2)), ConvertTool.ToInt16(buf.Substring(8, 2)), ConvertTool.ToInt16(buf.Substring(10, 2)), ConvertTool.ToInt16(buf.Substring(12, 2)));
				//}
				else
				{
					if(buf == null || buf.Length <= 0) return " ";
					return buf;
				}
			}
			catch 
			{
				return noneDataString;
			}
		}






	}
}

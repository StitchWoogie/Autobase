using System;
using System.Data;
using AutoLib;
using AutoLibLocal;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for run_DigitalOnOffListData.
	/// </summary>

	class onOffListSaveStruct
	{
		public string	tag;
		public string	start_time;
		public string	stop_time;
		public string	run_time;
		public string[]	element;
		public int		nAddCount;
	};

	class onOffListSaveSumStruct
	{
		public bool		bExist;
		public string	sum;		
	};

	public class run_DigitalOnOffListData
	{
		memberConfigStruct	member;
		BasicRptTool.eDigitalDataType eDiDataType;
		int nElementNum;
		int nColumn;
		int nRow;
		string[] sFieldData;
		string	tagName;
		onOffListSaveStruct	saveOnOffList;
		onOffListSaveSumStruct saveSumList;

		public run_DigitalOnOffListData(memberConfigStruct mem, int num, int elementNum, int columnCount, int rowCount, string[] sData)
		{
			//
			// TODO: Add constructor logic here
			//
			member = mem;
			eDiDataType = (BasicRptTool.eDigitalDataType)num;
			nElementNum = elementNum;
			nColumn = columnCount;
			nRow = rowCount;
			sFieldData = sData;	
		}


		public string run()
		{
			switch(eDiDataType) 
			{
				case BasicRptTool.eDigitalDataType.ONOFFLIST :
					return readOnOffListRealData();
				case BasicRptTool.eDigitalDataType.ONOFFLIST_SUM :
					if(readOnOffListDataAndRowColumnCount(eDiDataType) != 1) return member.noneDataString;	// 먼저 읽은 데이터가 없으므로
					return readOnOffListSumRealData();
				case BasicRptTool.eDigitalDataType.MULTI_ONOFFLIST_SUM :
					if(nRow == 0) readMultiOnOffListSumData();
					return readMultiOnOffListSumRealData();
			}
			return member.noneDataString;
		}

		string readOnOffListRealData()
		{
			if(BasicRptTool.saveDataArray == null) return member.noneDataString;	// 읽은 List 자료가 없다.
			if(10 + nColumn >= nElementNum) return member.noneDataString;			// 컬럼번호 에러
			if(nRow >= BasicRptTool.saveDataArray.Count) return member.noneDataString;// 읽는 List라인이 저장된라인보다 크다

			string	columnName = sFieldData[10 + nColumn];
			int		index = -1;

			if(String.Compare(columnName, "NO", true) == 0) 
			{
				return (nRow+1).ToString();
			}

			if(isOnOffListDataElement(columnName, ref index) == false) return member.noneDataString;// 일치하는 컬럼이름이 없다
			if(index == -1) return member.noneDataString;					// 컬럼번호 이상
			
			saveOnOffList = (onOffListSaveStruct)BasicRptTool.saveDataArray[nRow];

			if(columnName == BasicRptTool.sOnOffListColumnName[0]) return saveOnOffList.tag;
			if(columnName == BasicRptTool.sOnOffListColumnName[3]) return saveOnOffList.run_time;

			DateTime		t;
			if(columnName == BasicRptTool.sOnOffListColumnName[1]) 
			{
				try 
				{
					t = ConvertTool.ToDateTime(saveOnOffList.start_time);					
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			if(columnName == BasicRptTool.sOnOffListColumnName[2]) 
			{
				try 
				{
					t = ConvertTool.ToDateTime(saveOnOffList.stop_time);
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			index -= 4;
			if(index < 0 || saveOnOffList.nAddCount <= index) return member.noneDataString;
			try 
			{
				if(saveOnOffList.element[index] == null || saveOnOffList.element[index].Length <= 0) return member.noneDataString;
			}
			catch 
			{
				return member.noneDataString;
			}
			return saveOnOffList.element[index];
		}

		string readOnOffListSumRealData()
		{
			if(BasicRptTool.saveDataArray == null) return member.noneDataString;	// 읽은 List 자료가 없다.            			
			int				count = BasicRptTool.saveDataArray.Count;
			if(count <= 0) return member.noneDataString;

			int				i, hour = 0, min = 0, sec = 0;
			string			imsi = "";
			CommaBlockString comma = new CommaBlockString();

			comma.SetBlockCode(':');			
			for(i = 0; i < count; i++) 
			{
				saveOnOffList = (onOffListSaveStruct)BasicRptTool.saveDataArray[i];
				try 
				{
					comma.Set(saveOnOffList.run_time);
					if(comma.IsEOS()) continue;
					comma.GetString(ref imsi);
					hour += ConvertTool.ToInt32(imsi);
					if(comma.IsEOS()) continue;
					comma.GetString(ref imsi);
					min += ConvertTool.ToInt32(imsi);
					if(comma.IsEOS()) continue;
					comma.GetString(ref imsi);
					sec += ConvertTool.ToInt32(imsi);
				}
				catch 
				{					
				}
			}
			min += sec / 60;
			sec = sec % 60;
			hour += min / 60;
			min = min % 60;
			return String.Format("{0}:{1,2:d02}:{2,2:d02}", hour, min, sec);
		}

		bool readMultiOnOffListSumData()
		{
			if(BasicRptTool.saveDataArray == null) return false;	// 읽은 List 자료가 없다.
			if(BasicRptTool.saveDataArray2 == null) return false;	// 저장할 array 가 없다
			if(nElementNum < 8) return false;

			int			i, index = 0, dataTypeSort, hap;
			bool		bStart = true;			
			int[]		currTime = new int[2], upTime = new int[2];
			string		buf = "";

			dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[3], ref index);// 4번째가 시간종류필드
			if(dataTypeSort == -1) return false;
			try 
			{
				bStart = (ConvertTool.ToInt32(sFieldData[2]) == 0) ? true : false;
				currTime[0] = ConvertTool.ToInt32(sFieldData[4]);
				currTime[1] = ConvertTool.ToInt32(sFieldData[5]);
				upTime[0] = ConvertTool.ToInt32(sFieldData[6]);
				upTime[1] = ConvertTool.ToInt32(sFieldData[7]);
			}
			catch
			{
			}

            bool bUsePeriod = ((ConvertTool.ToInt32(sFieldData[8]) & 0x02) > 0);
            DateTime t;

            // 기간자료 범위를 사용할 경우
            if (bUsePeriod)
            {
                t = BasicRptTool.reportConfig.minListStartDt;
                hap = (int)GetTimeCount(BasicRptTool.reportConfig.minListStartDt, BasicRptTool.reportConfig.minListEndDt, dataTypeSort);

            }
            else
            {
                t = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
                hap = (int)BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
            }

			//DateTime t = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			//hap = (int)BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
			if(hap <= 0) return false;

			BasicRptTool.saveDataArray2.Clear();		// 저장할 array 를 클리어
			for(i = 0; i < hap; i++) 
			{
				saveSumList = new onOffListSaveSumStruct();
				saveSumList.bExist = getOnOffListSumData(ref buf, t, i, dataTypeSort, bStart);
				saveSumList.sum = buf;
				BasicRptTool.saveDataArray2.Add(saveSumList);
			}
			return true;
		}

		string readMultiOnOffListSumRealData()
		{
			if(BasicRptTool.saveDataArray2 == null) return member.noneDataString;	// 읽은 array 가 없다			
			if(BasicRptTool.saveDataArray2.Count <= nRow) return member.noneDataString;	// 저장된 array 번지가 없다.
			saveSumList = (onOffListSaveSumStruct)BasicRptTool.saveDataArray2[nRow];
			if(saveSumList.bExist == false) return member.noneDataString;
			return saveSumList.sum;			
		}

        static public int GetTimeCount(DateTime ts, DateTime te, int timetype)
        {
            if (timetype == 0)			// 분
            {
                return (int)(TimeUtil.GetMinHap(te) - TimeUtil.GetMinHap(ts)+1);
            }
            else if (timetype == 1)		// 시간
            {
                return (int)(TimeUtil.GetHourHap(te) - TimeUtil.GetHourHap(ts) + 1);
            }
            else if (timetype == 2)		// 일
            {
                return (int)(TimeUtil.GetDayHap(te) - TimeUtil.GetDayHap(ts) + 1);
            }
            else if (timetype == 3)		// 월
            {
                return (int)(TimeUtil.GetMonHap(te) - TimeUtil.GetMonHap(ts) + 1);
            }
            else					// 년
            {
                return 0;
            }
        }

		public uint readOnOffListDataAndRowColumnCount(BasicRptTool.eDigitalDataType dataType)
		{
			int			i, index = 0, dataTypeSort, nSorting = 0;
			bool		bStart = true, bAllColumnSave;
			long		hap;
			int[]		currTime = new int[2], upTime = new int[2];			
            
			BasicRptTool.saveDataArray.Clear();
			if(dataType == BasicRptTool.eDigitalDataType.ONOFFLIST) 
			{
				bAllColumnSave = true;
				if(nElementNum < 11) return 0;
			}
			else 
			{
				bAllColumnSave = false;
				if(nElementNum < 8) return 0;
			}
			tagName = sFieldData[1];
			tagName = tagName.Trim();
			if(tagName.Length <= 0 || sFieldData[3].Length <= 0) return 0;// Tag, Time Range
			dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[3], ref index);// 4번째가 시간종류필드
			if(dataTypeSort == -1) return 0;
			try 
			{
				bStart = (ConvertTool.ToInt32(sFieldData[2]) == 0) ? true : false;
				currTime[0] = ConvertTool.ToInt32(sFieldData[4]);
				currTime[1] = ConvertTool.ToInt32(sFieldData[5]);
				upTime[0] = ConvertTool.ToInt32(sFieldData[6]);
				upTime[1] = ConvertTool.ToInt32(sFieldData[7]);
				if(dataType == BasicRptTool.eDigitalDataType.ONOFFLIST) 
					nSorting = ConvertTool.ToInt32(sFieldData[9]);
			}
			catch
			{
			}
			if(dataType == BasicRptTool.eDigitalDataType.ONOFFLIST) 
			{
				BasicRptTool.nMultiLineFieldCount = 0;
				BasicRptTool.nMultiLineFieldCount = nElementNum-10;
			}
			
			bool bUsePeriod = ((ConvertTool.ToInt32(sFieldData[8])&0x02) > 0);
            DateTime t;

            // 기간자료 범위를 사용할 경우
            if (bUsePeriod)
            {
                t = BasicRptTool.reportConfig.minListStartDt;
                hap = GetTimeCount(BasicRptTool.reportConfig.minListStartDt, BasicRptTool.reportConfig.minListEndDt, dataTypeSort);

            }
            else
            {
                t = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
                hap = (uint)BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
            }

			if(hap <= 0) return 0;			
			
			DataGate	gate = new DataGate();
			DataSet		ds;
			DateTime	listTime = new DateTime(t.Year, t.Month, 1, 0, 0, 0);

			DataLocal dLocal = new DataLocal();
			ds = dLocal.GetOnOffList(listTime.Year, listTime.Month);
			
			for(i = 0; i < hap; i++) 
			{
				if(BasicRptTool.checkTimeIsEqual(t, listTime, 3) == false) 
				{
					listTime = listTime.AddMonths(1);
					ds = dLocal.GetOnOffList(listTime.Year, listTime.Month);
				}
				if(ds != null) getCurrentOnOffListData(tagName, t, ds, dataTypeSort, bStart, bAllColumnSave);
				switch((BasicRptTool.eTimeRangeType)dataTypeSort)
				{
					case BasicRptTool.eTimeRangeType.MIN : t = t.AddMinutes(1); break;
					case BasicRptTool.eTimeRangeType.HOUR : t = t.AddHours(1); break;
					case BasicRptTool.eTimeRangeType.DAY : t = t.AddDays(1); break;
					case BasicRptTool.eTimeRangeType.MONTH : t = t.AddMonths(1); break;
					default : break;
				}				
			}

			if(dataType == BasicRptTool.eDigitalDataType.ONOFFLIST) 
			{
				SortOnOffList(nSorting, bStart);					// Sorting func
				return (uint)BasicRptTool.saveDataArray.Count;
			}
			else if(dataType == BasicRptTool.eDigitalDataType.MULTI_ONOFFLIST_SUM) 
				return (UInt32)hap;
			else							// One Line ONOFFLIST SUM
				return 1;			
		}

		void getCurrentOnOffListData(string tagName, DateTime t, DataSet ds, int dataTypeSort, bool bStart, bool bAllColumnSave)
		{
			int			i, j, count, columnCount;
			DataRow		row;
			DateTime	t2;
			string		name, dateBuf, timeBuf, buf;

			if(bStart) 
			{
				dateBuf = "start_date";
				timeBuf = "start_time";
			}
			else 
			{
				dateBuf = "end_date";
				timeBuf = "end_time";
			}

			count = ds.Tables[0].Rows.Count;
			columnCount = ds.Tables[0].Columns.Count - 6;
			if(columnCount > 250) columnCount = 250;

			for(i = 0; i < count; i++)
			{
				row = ds.Tables[0].Rows[i];
				try 
				{
					buf = row[dateBuf].ToString();
					buf += " ";
					buf += row[timeBuf].ToString();
					t2 = ConvertTool.ToDateTime(buf);
					if(BasicRptTool.checkTimeIsEqual(t, t2, dataTypeSort) == false) continue;					
				}
				catch
				{
					continue;
				}				

				try
				{					
					name = row["tag"].ToString();
                    if (!BasicRptTool.IsTagInclude(tagName, name)) continue;
				}
				catch
				{
					continue;
				}
				saveOnOffList = new onOffListSaveStruct();
				
				if(bStart) 
				{					
					saveOnOffList.start_time = buf;

					buf = row["end_date"].ToString();
					buf += " ";
					buf += row["end_time"].ToString();
					saveOnOffList.stop_time = buf;
				}
				else 
				{					
					saveOnOffList.stop_time = buf;

					buf = row["start_date"].ToString();
					buf += " ";
					buf += row["start_time"].ToString();
					saveOnOffList.start_time = buf;
				}				
				saveOnOffList.run_time = row["oper_time"].ToString();
				if(bAllColumnSave == false) 
				{
					BasicRptTool.saveDataArray.Add(saveOnOffList);
					continue;
				}

				if(columnCount > 0) 
				{
					saveOnOffList.element = new string[columnCount];
				}
				saveOnOffList.nAddCount = columnCount;
				saveOnOffList.tag = name;
				for(j = 0; j < columnCount; j++) 
				{
					try 
					{
						saveOnOffList.element[j] = row[j+6].ToString();
					}
					catch 
					{
					}
				}
				BasicRptTool.saveDataArray.Add(saveOnOffList);
			}
		}

		bool isOnOffListDataElement(string name, ref int index)
		{
			int		i;

			index = -1;
			for(i = 0; i < 4; i++) 
			{
				if(name == BasicRptTool.sOnOffListColumnName[i]) 
				{
					index = i;
					return true;
				}
			}
			if(String.Compare(name, 0, BasicRptTool.sOnOffListColumnName[4], 0, BasicRptTool.sOnOffListColumnName[4].Length) == 0) {
				try 
				{
					index = ConvertTool.ToInt16(name.Substring(4));
					index += 4;
					return true;
				}
				catch
				{
				}
			}
			return false;
		}


		void SortOnOffList(int nSort, bool bStart)
		{
			if(nSort == 0) return;
			if(BasicRptTool.saveDataArray.Count <= 0)	return;

			int					l, m, count = BasicRptTool.saveDataArray.Count;
			int					big_pos;
			onOffListSaveStruct	item1, item2;
			DateTime			t1, t2;
			

			if(nSort == 1) 
			{
				for(l = 0; l < count-1; l++) 
				{
					item1 = (onOffListSaveStruct)BasicRptTool.saveDataArray[l];
					big_pos = l;
					for(m = l+1; m < count; m++) 
					{
						item2 = (onOffListSaveStruct)BasicRptTool.saveDataArray[m];
						try 
						{
							if(bStart) 
							{
								t1 = ConvertTool.ToDateTime(item1.start_time);
								t2 = ConvertTool.ToDateTime(item2.start_time);
							}
							else 	   
							{
								t1 = ConvertTool.ToDateTime(item1.stop_time);
								t2 = ConvertTool.ToDateTime(item2.stop_time);
							}
						}
						catch 
						{
							continue;
						}
						
						if(t1 > t2) 
						{
							big_pos = m;
							item1 = item2;
						}
					}
					if(big_pos != l) 
					{
						object temp;
						temp = BasicRptTool.saveDataArray[l];
						BasicRptTool.saveDataArray[l] = BasicRptTool.saveDataArray[big_pos];
						BasicRptTool.saveDataArray[big_pos] = temp;
					}
				}
			}
			else if(nSort == 2) 
			{
				for(l = 0; l < count-1; l++) 
				{
					item1 = (onOffListSaveStruct)BasicRptTool.saveDataArray[l];
					big_pos = l;
					for(m = l+1; m < count; m++) 
					{
						item2 = (onOffListSaveStruct)BasicRptTool.saveDataArray[m];
						try 
						{
							if(bStart) 
							{
								t1 = ConvertTool.ToDateTime(item1.start_time);
								t2 = ConvertTool.ToDateTime(item2.start_time);
							}
							else 	   
							{
								t1 = ConvertTool.ToDateTime(item1.stop_time);
								t2 = ConvertTool.ToDateTime(item2.stop_time);
							}
						}
						catch 
						{
							continue;
						}
						if(t1 < t2) 
						{
							big_pos = m;
							item1 = item2;
						}
					}
					if(big_pos != l) 
					{
						object temp;
						temp = BasicRptTool.saveDataArray[l];
						BasicRptTool.saveDataArray[l] = BasicRptTool.saveDataArray[big_pos];
						BasicRptTool.saveDataArray[big_pos] = temp;						
					}
				}
			}			
		}

		bool getOnOffListSumData(ref string buf, DateTime startTime, int nRow, int dataTypeSort, bool bStart)
		{
			int					i, hour = 0, min = 0, sec = 0, count;
			bool				bExist = false;
			DateTime			t, sTime;
			string				imsi = "";
			CommaBlockString comma = new CommaBlockString();

			count = BasicRptTool.saveDataArray.Count;
			sTime = BasicRptTool.getCurrentMultiRowDateTime(startTime, dataTypeSort, 0, 0, nRow);			
			comma.SetBlockCode(':');
			for(i = 0; i < count; i++)
			{
				saveOnOffList = (onOffListSaveStruct)BasicRptTool.saveDataArray[i];
				if(bStart) 
					t = ConvertTool.ToDateTime(saveOnOffList.start_time);				
				else				
					t = ConvertTool.ToDateTime(saveOnOffList.stop_time);
				
				if(BasicRptTool.checkTimeIsEqual(t, sTime, dataTypeSort) == false) continue;
						
				try 
				{
					comma.Set(saveOnOffList.run_time);
					if(comma.IsEOS()) continue;
					comma.GetString(ref imsi);
					hour += ConvertTool.ToInt32(imsi);
					if(comma.IsEOS()) continue;
					comma.GetString(ref imsi);
					min += ConvertTool.ToInt32(imsi);
					if(comma.IsEOS()) continue;
					comma.GetString(ref imsi);
					sec += ConvertTool.ToInt32(imsi);
					bExist = true;
				}
				catch 
				{					
				}
			}
			min += sec / 60;
			sec = sec % 60;
			hour += min / 60;
			min = min % 60;
			buf = String.Format("{0}:{1,2:d02}:{2,2:d02}", hour, min, sec);
			return bExist;


		}




	}
}

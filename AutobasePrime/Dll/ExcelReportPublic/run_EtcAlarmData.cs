using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;	
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Threading.Tasks;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for run_EtcAlarmData.
	/// </summary>

	class alarmSaveStruct
	{
		public string	alarm_datetime;
		public string	tag;
		public string	description;
		public string	message;
		public string	alarm_type;
		public string	priority;
		public string	port;
		public string	station;
		public string	address;
		public string	type;		
	};
	
	public class run_EtcAlarmData
	{
		memberConfigStruct	member;
		BasicRptTool.eAnalogDataType eAiDataType;
		int nElementNum;
		int nColumn;
		int nRow;
		string[] sFieldData;
		string	tagName;
		alarmSaveStruct alarmSave;

		public run_EtcAlarmData(memberConfigStruct mem, int num, int elementNum, int columnCount, int rowCount, string[] sData)
		{
			//
			// TODO: Add constructor logic here
			//
			member = mem;
			eAiDataType = (BasicRptTool.eAnalogDataType)num;
			nElementNum = elementNum;
			nColumn = columnCount;
			nRow = rowCount;
			sFieldData = sData;			
		}

		public string run()
		{			
			if(BasicRptTool.saveDataArray == null) return member.noneDataString;	// 읽은 경보자료가 없다.
			if(8 + nColumn >= nElementNum) return member.noneDataString;			// 컬럼번호 에러

			string	columnName = sFieldData[8 + nColumn];
			if(isAlarmDataElement(columnName) == false) return member.noneDataString;// 일치하는 컬럼이름이 없다			
			if(nRow >= BasicRptTool.saveDataArray.Count) return member.noneDataString;// 읽는 경보라인이 저장된라인보다 크다
			
			alarmSave = (alarmSaveStruct)BasicRptTool.saveDataArray[nRow];
			if(columnName == BasicRptTool.sAlarmColumnName[0]) 
			{
				try 
				{
					DateTime		t = ConvertTool.ToDateTime(alarmSave.alarm_datetime);					
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			if(columnName == BasicRptTool.sAlarmColumnName[1]) return alarmSave.tag;
			if(columnName == BasicRptTool.sAlarmColumnName[2]) return alarmSave.description;
			if(columnName == BasicRptTool.sAlarmColumnName[3]) return alarmSave.message;
			if(columnName == BasicRptTool.sAlarmColumnName[4]) return alarmSave.priority;
			if(columnName == BasicRptTool.sAlarmColumnName[5]) 
			{
				int		order = nRow + 1;
				return order.ToString();	// 순서번호
			}
						
			return member.noneDataString;
		}

		public async Task<uint> readAlarmDataAndRowColumnCount()
		{
			int			i, index = 0, dataTypeSort;
			long		hap;
			int[]		currTime = new int[2], upTime = new int[2];			
            
			BasicRptTool.saveDataArray.Clear();
			if(nElementNum < 9) return 0;
			tagName = sFieldData[1];
			tagName = tagName.Trim();
			if(tagName.Length <= 0 || sFieldData[2].Length <= 0) return 0;// Tag, Time Range
			dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[2], ref index);
			if(dataTypeSort == -1) return 0;
			try 
			{
				currTime[0] = ConvertTool.ToInt32(sFieldData[3]);
				currTime[1] = ConvertTool.ToInt32(sFieldData[4]);
				upTime[0] = ConvertTool.ToInt32(sFieldData[5]);
				upTime[1] = ConvertTool.ToInt32(sFieldData[6]);
			}
			catch
			{
			}
			BasicRptTool.nMultiLineFieldCount = nElementNum-8;
			//for(i = 8; i < nElementNum; i++) 
			//{
			//	if(isAlarmDataElement(sFieldData[i])) 
			//	{
			//		BasicRptTool.nMultiLineFieldCount++;
			//	}
			//}
			//if(BasicRptTool.nMultiLineFieldCount <= 0) return 0;			
			
			DateTime t = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			hap = (uint)BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
			if(hap <= 0) return 0;

			if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MONTH)
			{
				DateTime t2 = new DateTime(t.Year, t.Month, 1, 0, 0, 0);
				t2 = t2.AddMonths((int)hap);
				t2 = t2.AddDays(-1);
				hap = TimeUtil.GetDayHap(t2.Year, t2.Month, t2.Day) - TimeUtil.GetDayHap(t.Year, t.Month, t.Day) + 1;
			}

			DataGate	gate = new DataGate();
			DataSet		ds;

			DateTime alarmFileT = BasicRptTool.getCurrentMultiRowDateTime(t, 1, 0, 0, 0);
			ds = await gate.GetAlarmFile(maketAlarmFileName(alarmFileT)).ConfigureAwait(false);

			
			for(i = 0; i < hap; i++) 
			{
				if(BasicRptTool.checkTimeIsEqual(t, alarmFileT, 2) == false) 
				{
					alarmFileT = alarmFileT.AddDays(1);
					ds = await gate.GetAlarmFile(maketAlarmFileName(alarmFileT)).ConfigureAwait(false);
				}
				if(ds != null) getCurrentAlarmData(tagName, t, ds, dataTypeSort);
				switch((BasicRptTool.eTimeRangeType)dataTypeSort)
				{
					case BasicRptTool.eTimeRangeType.MIN : t = t.AddMinutes(1); break;
					case BasicRptTool.eTimeRangeType.HOUR : t = t.AddHours(1); break;
					case BasicRptTool.eTimeRangeType.DAY : t = t.AddDays(1); break;
					case BasicRptTool.eTimeRangeType.MONTH : t = t.AddDays(1); break;
					default : break;
				}				
			}

			//switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			//{
			//	case BasicRptTool.eTimeRangeType.MONTH :
			//	default : AlarmDataReadMinHourDay(t, hap, dataTypeSort); break;				
			//}
			return (uint)BasicRptTool.saveDataArray.Count;			
		}
		
		void getCurrentAlarmData(string tagName, DateTime t, DataSet ds, int dataTypeSort)
		{
			int			i, count;
			DataRow		row;
			DateTime	t2;
			string		name;

			count = ds.Tables[0].Rows.Count;

			for(i = 0; i < count; i++)
			{
				row = ds.Tables[0].Rows[i];
				try 
				{
					t2 = ConvertTool.ToDateTime(row["alarm_datetime"].ToString());
					if(BasicRptTool.checkTimeIsEqual(t, t2, dataTypeSort) == false) continue;
					//t2 = BasicRptTool.getCurrentMultiRowDateTime(t2, dataTypeSort, 0, 0, 0);					
					//if(t != t2) continue;
				}
				catch
				{
					continue;
				}				

				try 
				{					
					name = row["tag"].ToString();
					if(!BasicRptTool.IsTagInclude(tagName, name)) continue;
				}
				catch
				{
					continue;
				}
				alarmSave = new alarmSaveStruct();				
				alarmSave.alarm_datetime = row["alarm_datetime"].ToString();
				alarmSave.tag = row["tag"].ToString();
				alarmSave.description = row["description"].ToString();
				alarmSave.message = row["message"].ToString();
				alarmSave.alarm_type = row["alarm_type"].ToString();
				alarmSave.priority = row["priority"].ToString();
				alarmSave.port = row["port"].ToString();
				alarmSave.station = row["station"].ToString();
				alarmSave.address = row["address"].ToString();
				alarmSave.type = row["type"].ToString();

				BasicRptTool.saveDataArray.Add(alarmSave);				
			}
		}       
		

		string maketAlarmFileName(DateTime t)
		{
			try 
			{
                // 2007.10.19 .AL3를 ALMX로 수정했으며 DataLocal 에서도 GetAlarmFile 도 두가지 파일을 모두 읽어볼 수 있도록 수정하였다.
				return String.Format("{0,04:d04}{1,02:d02}{2,02:d02}.ALMX", t.Year, t.Month, t.Day);
			}
			catch
			{
				return "";
			}
		}

		DateTime AddDateTime(DateTime t, int dataTypeSort, int count)
		{
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : return t.AddMinutes(count);
				case BasicRptTool.eTimeRangeType.DAY : return t.AddDays(count);
				case BasicRptTool.eTimeRangeType.MONTH : return t.AddMonths(count);
				default : return t.AddHours(count);
			}

		}

		bool isAlarmDataElement(string name)
		{
			int		i;

			for(i = 0; i < BasicRptTool.nAlarmColumnCount; i++) 
			{
				if(name == BasicRptTool.sAlarmColumnName[i]) return true;
			}			
			return false;
		}




	}
}

using System;
using System.Data;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;
using DialogTag;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for run_DigitalDataMain.
	/// </summary>
	public class run_DigitalDataMain
	{
		memberConfigStruct	member;
		BasicRptTool.eDigitalDataType eDiDataType;
		int nElementNum;
		int nColumn;
		int nRow;
		string[] sFieldData;
		string	tagName;

		public run_DigitalDataMain(memberConfigStruct mem, int num, int elementNum, int columnCount, int rowCount, string[] sData)
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

		public async Task<string> run()
		{
			switch(eDiDataType) 
			{
				case BasicRptTool.eDigitalDataType.DI_CURR :
					return getDiCurrTagData();
				case BasicRptTool.eDigitalDataType.ONTIME : 		//ON 시간
				case BasicRptTool.eDigitalDataType.OFFTIME : 		//OFF 시간
				case BasicRptTool.eDigitalDataType.ONCOUNT :		//ON 횟수
					return await DiNormalOneLineData().ConfigureAwait(false);
				case BasicRptTool.eDigitalDataType.DI_MOMENT :		//순시값
					return await DiOneMomentDataCalc().ConfigureAwait(false);
				case BasicRptTool.eDigitalDataType.MULTI_ONTIME :	//여러줄 ON 시간
				case BasicRptTool.eDigitalDataType.MULTI_OFFTIME :	//여러줄 OFF 시간
				case BasicRptTool.eDigitalDataType.MULTI_ONCOUNT :	//여러줄 ON 횟수
				case BasicRptTool.eDigitalDataType.MULTI_DI_MOMENT ://여러줄 순시값
					return getDiMultiLineData();				
			}
			return member.noneDataString;
		}

		string getDiCurrTagData()
		{			
			if(nElementNum < 2) return member.noneDataString;
			tagName = sFieldData[1];
			tagName = tagName.Trim();
			if(tagName.Length <= 0) return member.noneDataString;	// Tag Name
			
            string	curr = "";
            if (ConfigVarTotal.bRunByWebService)
            {
                double fval;
                TagLib.GetTagValue(tagName, out curr, out fval);
                return curr;
            }
            else
            {
                if (SharedTag.GetCurr(tagName, ref curr) == false) return member.noneDataString;
                return curr;
            }
		}


		async Task<string> DiNormalOneLineData()
		{
			DataGate	gate = new DataGate();
			DataSet		ds;
			int			dataTypeSort, index = 0;
			UInt32		hap;
			int[]		currTime = new int[2], upTime = new int[2];
			string		sData;
			EnumDataType dataType;


			if(nElementNum < 7) return member.noneDataString;
			tagName = sFieldData[1];
			tagName = tagName.Trim();
			if(tagName.Length <= 0 || sFieldData[2].Length < 2) return member.noneDataString;	// Tag Name, Time Type error			
			dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[2], ref index);
			if(dataTypeSort == -1) return member.noneDataString;

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
			DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			hap = BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
			if(hap <= 0) return member.noneDataString;

			switch(eDiDataType)
			{
				case BasicRptTool.eDigitalDataType.ONTIME : dataType = EnumDataType.ONTIME; break;
				case BasicRptTool.eDigitalDataType.OFFTIME : dataType = EnumDataType.OFFTIME; break;
				case BasicRptTool.eDigitalDataType.ONCOUNT : dataType = EnumDataType.CountOntime; break;
				default : return member.noneDataString;
			}
			
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : ds = await gate.GetDataDi(tagName, dataType, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.HOUR : ds = await gate.GetDataDi(tagName, dataType, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, 0, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.DAY : ds = await gate.GetDataDi(tagName, dataType, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, 0, 0, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.MONTH : ds = await gate.GetDataDi(tagName, dataType, EnumDataTime.Month, dt.Year, dt.Month, 1, 0, 0, (int)hap, 1).ConfigureAwait(false); break;					
				default : return member.noneDataString;
			}
			switch(eDiDataType) 
			{
				case BasicRptTool.eDigitalDataType.ONTIME : 				
					sData = DiNormalOneLineData(ds, hap, "ONTIME");
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
				case BasicRptTool.eDigitalDataType.OFFTIME : 
					sData = DiNormalOneLineData(ds, hap, "OFFTIME");
					if(sData == member.noneDataString) return sData;
					try 
					{
						index = ConvertTool.ToInt32(sData);
					}
					catch 
					{
						return sData;
					}
					return sData = String.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}", index/3600, (index/60) % 60, index % 60);					
				case BasicRptTool.eDigitalDataType.ONCOUNT : return DiNormalOneLineData(ds, hap, "COUNT");
			}
			return member.noneDataString;
		}

		async Task<string> DiOneMomentDataCalc()
		{
			DataGate	gate = new DataGate();
			DataSet		ds;
			int			dataTypeSort, index = 0, day = 1, hour = 0, min = 0;
			UInt32		hap;
			int[]		currTime = new int[2], upTime = new int[2];
			
			if(nElementNum < 11) return member.noneDataString;
			tagName = sFieldData[1];
			tagName = tagName.Trim();
			if(tagName.Length <= 0 || sFieldData[2].Length < 2) return member.noneDataString;	// Tag Name, Time Type error
			dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[2], ref index);
			if(dataTypeSort == -1) return member.noneDataString;

			try 
			{
				currTime[0] = ConvertTool.ToInt32(sFieldData[3]);
				currTime[1] = ConvertTool.ToInt32(sFieldData[4]);
				upTime[0] = ConvertTool.ToInt32(sFieldData[5]);
				upTime[1] = ConvertTool.ToInt32(sFieldData[6]);
				//bUsePeriodTime = ConvertTool.ToInt32(sFieldData[7]);	// 기간자료 시간사용
				day = ConvertTool.ToInt32(sFieldData[8]);
				hour = ConvertTool.ToInt32(sFieldData[9]);
				min = ConvertTool.ToInt32(sFieldData[10]);				
			}
			catch
			{
			}
			DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			hap = 1;
			
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : ds = await gate.GetDataDi(tagName, EnumDataType.MOMENT, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.HOUR : ds = await gate.GetDataDi(tagName, EnumDataType.MOMENT, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, min, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.DAY : ds = await gate.GetDataDi(tagName, EnumDataType.MOMENT, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, hour, min, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.MONTH : ds = await gate.GetDataDi(tagName, EnumDataType.MOMENT, EnumDataTime.Month, dt.Year, dt.Month, day, hour, min, (int)hap, 1).ConfigureAwait(false); break;					
				default : return member.noneDataString;
			}

			DataRow		row;
			double		val;

			try 
			{
				row = ds.Tables[0].Rows[0];
				if(ConvertTool.ToInt16(row[0].ToString()) != 1) return member.noneDataString;
				val = ConvertTool.ToSingle(row["MOMENT"].ToString());
				return val.ToString();
			}
			catch
			{
			}						
			return member.noneDataString;
		}
		

		string getDiMultiLineData()
		{
			if(BasicRptTool.dReadDs == null) return member.noneDataString;
			if(BasicRptTool.dReadDs.Tables[0].Rows.Count <= nRow) return member.noneDataString;

			string	columnName;
			switch(eDiDataType)
			{
				case BasicRptTool.eDigitalDataType.MULTI_ONTIME : columnName = "ONTIME"; break;
				case BasicRptTool.eDigitalDataType.MULTI_OFFTIME : columnName = "OFFTIME"; break;
				case BasicRptTool.eDigitalDataType.MULTI_ONCOUNT : columnName = "COUNT"; break;
				case BasicRptTool.eDigitalDataType.MULTI_DI_MOMENT : columnName = "MOMENT"; break;
				default : return member.noneDataString;
			}			

			DataRow		row;
			int			index;
			double		val = 0.0;

			try
			{					
				row = BasicRptTool.dReadDs.Tables[0].Rows[nRow];
				if(ConvertTool.ToInt16(row[0].ToString()) != 1) return member.noneDataString;
				switch(eDiDataType)
				{
					case BasicRptTool.eDigitalDataType.MULTI_ONTIME : 
					case BasicRptTool.eDigitalDataType.MULTI_OFFTIME :						
						index = (int)ConvertTool.ToDouble(row[columnName].ToString());						
						return String.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}", index/3600, (index/60) % 60, index % 60);						
					default :				
						val = ConvertTool.ToSingle(row[columnName].ToString());
						return val.ToString();
				}
			}
			catch
			{
			}			
			return member.noneDataString;
		}
		
		public async Task<uint> readDiMultiLineData()
		{
			DataGate	gate = new DataGate();
			int			dataTypeSort, index = 0;
			uint		hap;
			int[]		currTime = new int[2], upTime = new int[2];
			EnumDataType dataType;

			if(nElementNum < 7) return 0;
			tagName = sFieldData[1];
			tagName = tagName.Trim();
			if(tagName.Length <= 0 || sFieldData[2].Length < 2) return 0;	// Tag Name, Time Type error
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
			DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			hap = (uint)BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
			if(hap <= 0) return 0;

			switch(eDiDataType)
			{
				case BasicRptTool.eDigitalDataType.MULTI_ONTIME : dataType = EnumDataType.ONTIME; break;
				case BasicRptTool.eDigitalDataType.MULTI_OFFTIME : dataType = EnumDataType.OFFTIME; break;
				case BasicRptTool.eDigitalDataType.MULTI_ONCOUNT : dataType = EnumDataType.COUNT; break;
				default : return 0;
			}
			
			BasicRptTool.dReadDs = null;
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : BasicRptTool.dReadDs = await gate.GetDataDi(tagName, dataType, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.HOUR : BasicRptTool.dReadDs = await gate.GetDataDi(tagName, dataType, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, 0, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.DAY : BasicRptTool.dReadDs = await gate.GetDataDi(tagName, dataType, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, 0, 0, (int)hap, 1).ConfigureAwait(false); break;					
				case BasicRptTool.eTimeRangeType.MONTH : BasicRptTool.dReadDs = await gate.GetDataDi(tagName, dataType, EnumDataTime.Month, dt.Year, dt.Month, 1, 0, 0, (int)hap, 1).ConfigureAwait(false); break;				
				default : return 0;
			}
			return hap;
		}

		public async Task<uint> readDiMultiLineMomentData()
		{
			DataGate	gate = new DataGate();
			int			dataTypeSort, index = 0, day = 1, hour = 0, min = 0;
			uint		hap;
			int[]		currTime = new int[2], upTime = new int[2];
			
			if(nElementNum < 11) return 0;
			tagName = sFieldData[1];
			tagName = tagName.Trim();
			if(tagName.Length <= 0 || sFieldData[2].Length < 2) return 0;	// Tag Name, Time Type error
			dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[2], ref index);
			if(dataTypeSort == -1) return 0;

			try 
			{
				currTime[0] = ConvertTool.ToInt32(sFieldData[3]);
				currTime[1] = ConvertTool.ToInt32(sFieldData[4]);
				upTime[0] = ConvertTool.ToInt32(sFieldData[5]);
				upTime[1] = ConvertTool.ToInt32(sFieldData[6]);
				//bUsePeriodTime = ConvertTool.ToInt32(sFieldData[7]);	// 기간자료 시간사용
				day = ConvertTool.ToInt32(sFieldData[8]);
				hour = ConvertTool.ToInt32(sFieldData[9]);
				min = ConvertTool.ToInt32(sFieldData[10]);				
			}
			catch
			{
			}
			DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			hap = (uint)BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
			if(hap <= 0) return 0;
			
			BasicRptTool.dReadDs = null;
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : BasicRptTool.dReadDs = await gate.GetDataDi(tagName, EnumDataType.MOMENT, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.HOUR : BasicRptTool.dReadDs = await gate.GetDataDi(tagName, EnumDataType.MOMENT, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, min, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.DAY : BasicRptTool.dReadDs = await gate.GetDataDi(tagName, EnumDataType.MOMENT, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, hour, min, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.MONTH : BasicRptTool.dReadDs = await gate.GetDataDi(tagName, EnumDataType.MOMENT, EnumDataTime.Month, dt.Year, dt.Month, day, hour, min, (int)hap, 1).ConfigureAwait(false); break;					
				default : return 0;
			}
			return hap;
		}

		string DiNormalOneLineData(DataSet ds, UInt32 hap, string columnName)
		{
			bool		bExist = false;
			double		val = 0.0;
			int			i;
			DataRow		row;

			for(i = 0; i < hap; i++)
			{				
				try 
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val += ConvertTool.ToSingle(row[columnName].ToString());						
					bExist = true;
				}
				catch
				{
				}
			}
			if(bExist) 
			{
				try
				{					
					return val.ToString();
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return member.noneDataString;
		}


		




	}
}

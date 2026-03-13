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
	/// Summary description for run_AnalogDataMain.
	/// </summary>
	public class run_AnalogDataMain
	{
		memberConfigStruct	member;
		BasicRptTool.eAnalogDataType eAiDataType;
		int nElementNum;
		int nColumn;
		int nRow;
		string[] sFieldData;
		string	tagName;
					
		public run_AnalogDataMain(memberConfigStruct mem, int num, int elementNum, int columnCount, int rowCount, string[] sData)
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

		public async Task<string> run()
		{
			switch(eAiDataType) 
			{
				case BasicRptTool.eAnalogDataType.AI_CURR : 
					return getAiCurrTagData();
				case BasicRptTool.eAnalogDataType.AI_AVE : 	//평균
				case BasicRptTool.eAnalogDataType.AI_MAX : 	//최대
				case BasicRptTool.eAnalogDataType.AI_MIN :	//최소
				case BasicRptTool.eAnalogDataType.AI_SUM : 	//적산
				case BasicRptTool.eAnalogDataType.AI_MAXSUM : //최대값더하기
				case BasicRptTool.eAnalogDataType.AI_MAXTIME : //최대값발생시점
				case BasicRptTool.eAnalogDataType.AI_MINTIME : //최소값발생시점
					return await AiNormalOneLineData().ConfigureAwait(false);

                case BasicRptTool.eAnalogDataType.AI_MAXSUB: //최대값차이
                    return await AiNormalOneLineDataMaxSub().ConfigureAwait(false);

				case BasicRptTool.eAnalogDataType.AI_MOMENT :	//순시값
					return  await AiOneMomentDataCalc().ConfigureAwait(false);
				case BasicRptTool.eAnalogDataType.MULTI_AVE :	//여러줄 평균값
				case BasicRptTool.eAnalogDataType.MULTI_MAXSUM ://여러줄 최대값더하기
				case BasicRptTool.eAnalogDataType.MULTI_MAX :	//여러줄 최대값
				case BasicRptTool.eAnalogDataType.MULTI_MIN :	//여러줄 최소값
				case BasicRptTool.eAnalogDataType.MULTI_SUM :	//여러줄 적산값
				case BasicRptTool.eAnalogDataType.MULTI_MAXSUB ://여려줄 최대값차이
				case BasicRptTool.eAnalogDataType.MULTI_MOMENT ://여러줄 순시값
					return getAiMultiLineData();
			}
			return member.noneDataString;
		}

		string getAiCurrTagData()
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

		// 최대/최소값 발생시점 일, 월 자료를 시간개수를 얻는 함수 추가, 2005-09-02 by kdy
		UInt32 getAiMinMaxTimeHourHap(ref BasicRptTool.eTimeRangeType eTimeSort, UInt32 hap, DateTime dt)
		{
			if(eAiDataType != BasicRptTool.eAnalogDataType.AI_MAXTIME && eAiDataType != BasicRptTool.eAnalogDataType.AI_MINTIME) return hap;

			if(eTimeSort == BasicRptTool.eTimeRangeType.DAY) 
			{
				eTimeSort = BasicRptTool.eTimeRangeType.HOUR;
				return hap*24;								// day =  24 hour;
			}
			if(eTimeSort == BasicRptTool.eTimeRangeType.MONTH) 
			{
				eTimeSort = BasicRptTool.eTimeRangeType.HOUR;
				UInt32	 day = 0;
				DateTime	dt2 = new DateTime(dt.Year, dt.Month, dt.Day);
				for(int i = 0; i < hap; i++) 
				{
					day += (UInt32)TimeUtil.getmonthlimit(dt2.Year, dt2.Month) * 24; // month = limit day *24 hour
					dt2 = dt2.AddMonths(1);
				}
				return day;
			}
			return hap;			
		}

		async Task<string> AiNormalOneLineData()
		{
			DataGate	gate = new DataGate();
			DataSet		ds;
			int			dataTypeSort, index = 0;
			UInt32		hap;
			int[]		currTime = new int[2], upTime = new int[2];
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
			if(BasicRptTool.checkOverDay(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0])) return member.noneDataString;// 일 자료 이고, 지정 날짜가 최대 월의 날짜보다 클때..

			DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			hap = BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
			if(hap <= 0) return member.noneDataString;

			switch(eAiDataType)
			{
				case BasicRptTool.eAnalogDataType.AI_MAXTIME : //최대값발생시점
				case BasicRptTool.eAnalogDataType.AI_MAXSUM : //최대값더하기
				case BasicRptTool.eAnalogDataType.AI_MAX : dataType = EnumDataType.MAX; break;
				case BasicRptTool.eAnalogDataType.AI_MINTIME : //최소값발생시점
				case BasicRptTool.eAnalogDataType.AI_MIN : dataType = EnumDataType.MIN; break;
				case BasicRptTool.eAnalogDataType.AI_SUM : dataType = EnumDataType.SUM; break;
				case BasicRptTool.eAnalogDataType.AI_MAXSUB : dataType = EnumDataType.SUB; break;
				default : dataType = EnumDataType.AVE; break;
			}			
			
			// 최대/최소값 발생시점 일, 월 자료를 시간까지 얻기 위해 수정, 2005-09-02 by kdy
			BasicRptTool.eTimeRangeType eTimeSort = (BasicRptTool.eTimeRangeType)dataTypeSort;
			if(eAiDataType == BasicRptTool.eAnalogDataType.AI_MAXTIME || eAiDataType == BasicRptTool.eAnalogDataType.AI_MINTIME) 
			{		// Data Local 에서 직접읽는다, 2005-09-02 추가
				ds = new DataSet();			// 필요없는 코드이나 에러가 발생하므로 추가
				hap = getAiMinMaxTimeHourHap(ref eTimeSort, hap, dt);
			}
			else 
			{
				switch(eTimeSort)
				{
					case BasicRptTool.eTimeRangeType.MIN : ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
					case BasicRptTool.eTimeRangeType.HOUR : ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, 0, (int)hap, 1).ConfigureAwait(false); break;
					case BasicRptTool.eTimeRangeType.DAY : ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, 0, 0, (int)hap, 1).ConfigureAwait(false); break;
					case BasicRptTool.eTimeRangeType.MONTH : ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Month, dt.Year, dt.Month, 1, 0, 0, (int)hap, 1).ConfigureAwait(false); break;					
					default : return member.noneDataString;
				}
			}

			switch(eAiDataType) 
			{
				case BasicRptTool.eAnalogDataType.AI_AVE : return run_AnalogDataBasicTools.AiAve(member, ds, (int)hap);
				case BasicRptTool.eAnalogDataType.AI_MAX : return run_AnalogDataBasicTools.AiMax(member, ds, (int)hap);
				case BasicRptTool.eAnalogDataType.AI_MIN : return run_AnalogDataBasicTools.AiMin(member, ds, (int)hap);
				case BasicRptTool.eAnalogDataType.AI_SUM : return run_AnalogDataBasicTools.AiSumAndMaxSub(member, ds, (int)hap, "SUM");
				case BasicRptTool.eAnalogDataType.AI_MAXSUB : return run_AnalogDataBasicTools.AiSumAndMaxSub(member, ds, (int)hap, "SUB");
				case BasicRptTool.eAnalogDataType.AI_MAXSUM : return run_AnalogDataBasicTools.AiSumAndMaxSub(member, ds, (int)hap, "MAX");
				//case BasicRptTool.eAnalogDataType.AI_MAXTIME : return AiNormalOneLineDataMaxTime(ds, hap, dt, (int)eTimeSort);//dataTypeSort); 일, 월 자료를 시간으로 변환했으므로..., 2005-09-02 by kdy
				//case BasicRptTool.eAnalogDataType.AI_MINTIME : return AiNormalOneLineDataMinTime(ds, hap, dt, (int)eTimeSort);//dataTypeSort); 일, 월 자료를 시간으로 변환했으므로..., 2005-09-02 by kdy
				case BasicRptTool.eAnalogDataType.AI_MAXTIME : return await AiNormalOneLineDataMaxMinTime(tagName, hap, dt, (int)eTimeSort, true).ConfigureAwait(false);//dataTypeSort); 일, 월 자료를 시간으로 변환했으므로..., 2005-09-02 by kdy
				case BasicRptTool.eAnalogDataType.AI_MINTIME : return await AiNormalOneLineDataMaxMinTime(tagName, hap, dt, (int)eTimeSort, false).ConfigureAwait(false);//dataTypeSort); 일, 월 자료를 시간으로 변환했으므로..., 2005-09-02 by kdy
			}
			return member.noneDataString;
		}

        async Task<string> AiNormalOneLineDataMaxSub()
        {
            DataGate gate = new DataGate();
            DataSet ds;
            int dataTypeSort, index = 0;
            int hap;
            int[] currTime = new int[2], upTime = new int[2];
            EnumDataType dataType;

            if (nElementNum < 7) return member.noneDataString;
            tagName = sFieldData[1];
            tagName = tagName.Trim();
            if (tagName.Length <= 0 || sFieldData[2].Length < 2) return member.noneDataString;	// Tag Name, Time Type error			
            dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[2], ref index);
            if (dataTypeSort == -1) return member.noneDataString;

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

            if (BasicRptTool.checkOverDay(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0])) return member.noneDataString;// 일 자료 이고, 지정 날짜가 최대 월의 날짜보다 클때..

            DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
            hap = (int)BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
            if (hap <= 0) return member.noneDataString;

            dataType = EnumDataType.MAX; 

            // 최대/최소값 발생시점 일, 월 자료를 시간까지 얻기 위해 수정, 2005-09-02 by kdy
            BasicRptTool.eTimeRangeType eTimeSort = (BasicRptTool.eTimeRangeType)dataTypeSort;

            string svalue1, svalue2;

            if (ConfigViewMain.bReportStartHourOfDayMaxSub && eTimeSort == BasicRptTool.eTimeRangeType.DAY)
            {
                DateTime t = new DateTime(dt.Year, dt.Month, dt.Day);
                t = t.AddHours(ConfigViewMain.nReportStartHourOfDay + 23);
                ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Hour, t.Year, t.Month, t.Day, t.Hour, 0, (int)hap, 1).ConfigureAwait(false);

                svalue1 = run_AnalogDataBasicTools.AiMax(member, ds, (int)hap);

                t = t.AddHours(-24);

                ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Hour, t.Year, t.Month, t.Day, t.Hour, 0, (int)hap, 1).ConfigureAwait(false);

                svalue2 = run_AnalogDataBasicTools.AiMax(member, ds, (int)hap);
            }
            else
            {
                switch (eTimeSort)
                {
                    case BasicRptTool.eTimeRangeType.MIN: ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
                    case BasicRptTool.eTimeRangeType.HOUR: ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, 0, (int)hap, 1).ConfigureAwait(false); break;
                    case BasicRptTool.eTimeRangeType.DAY: ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, 0, 0, (int)hap, 1).ConfigureAwait(false); break;
                    case BasicRptTool.eTimeRangeType.MONTH: ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Month, dt.Year, dt.Month, 1, 0, 0, (int)hap, 1).ConfigureAwait(false); break;
                    default: return member.noneDataString;
                }

                svalue1 = run_AnalogDataBasicTools.AiMax(member, ds, (int)hap);

                switch (eTimeSort)
                {
                    case BasicRptTool.eTimeRangeType.MIN: dt = dt.AddMinutes(-hap); break;
                    case BasicRptTool.eTimeRangeType.HOUR: dt = dt.AddHours(-hap); break;
                    case BasicRptTool.eTimeRangeType.DAY: dt = dt.AddDays(-hap); break;
                    case BasicRptTool.eTimeRangeType.MONTH: dt = dt.AddMonths(-hap); break;
                    default: return member.noneDataString;
                }

                switch (eTimeSort)
                {
                    case BasicRptTool.eTimeRangeType.MIN: ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
                    case BasicRptTool.eTimeRangeType.HOUR: ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, 0, (int)hap, 1).ConfigureAwait(false); break;
                    case BasicRptTool.eTimeRangeType.DAY: ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, 0, 0, (int)hap, 1).ConfigureAwait(false); break;
                    case BasicRptTool.eTimeRangeType.MONTH: ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Month, dt.Year, dt.Month, 1, 0, 0, (int)hap, 1).ConfigureAwait(false); break;
                    default: return member.noneDataString;
                }

                svalue2 = run_AnalogDataBasicTools.AiMax(member, ds, (int)hap);
            }

            if (member.noneDataString == svalue2 || member.noneDataString == svalue1)
                return member.noneDataString;

            double value1 = ConvertTool.ToDouble(svalue1);
            double value2 = ConvertTool.ToDouble(svalue2);
            double val;

            if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                val = TagLib.GetTagMemberFull(tagName) + value1 - value2;
            else
                val = value1 - value2;

            return val.ToString();
        }

		async Task<string> AiOneMomentDataCalc()
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

            string data_type = "";
            int nSharpSharpValue = 15;

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

                data_type = sFieldData[11];
                nSharpSharpValue = ConvertTool.ToInt32(sFieldData[12]);				
			}
			catch
			{
			}
			if(BasicRptTool.checkOverDay(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0])) return member.noneDataString;// 일 자료 이고, 지정 날짜가 최대 월의 날짜보다 클때..
			DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			hap = 1;

            DataLocal.sMomentDataType = data_type;
            DataLocal.nMomentSharpSharpValue = nSharpSharpValue;
			
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : ds =  await gate.GetDataAi(tagName, EnumDataType.MOMENT, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.HOUR : ds = await gate.GetDataAi(tagName, EnumDataType.MOMENT, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, min, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.DAY : ds = await gate.GetDataAi(tagName, EnumDataType.MOMENT, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, hour, min, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.MONTH : ds = await gate.GetDataAi(tagName, EnumDataType.MOMENT, EnumDataTime.Month, dt.Year, dt.Month, day, hour, min, (int)hap, 1).ConfigureAwait(false); break;					
				default : return member.noneDataString;
			}

			DataRow		row;
			float		val;

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
		

		string getAiMultiLineData()
		{
			if(BasicRptTool.dReadDs == null) return member.noneDataString;
			if(BasicRptTool.dReadDs.Tables[0].Rows.Count <= nRow) return member.noneDataString;

			string	columnName;
			switch(eAiDataType)
			{
				case BasicRptTool.eAnalogDataType.MULTI_AVE : columnName = "AVE"; break;
				case BasicRptTool.eAnalogDataType.MULTI_MAXSUM : //여러줄최대값더하기
				case BasicRptTool.eAnalogDataType.MULTI_MAX : columnName = "MAX"; break;
				case BasicRptTool.eAnalogDataType.MULTI_MIN : columnName = "MIN"; break;
				case BasicRptTool.eAnalogDataType.MULTI_SUM : columnName = "SUM"; break;
				case BasicRptTool.eAnalogDataType.MULTI_MAXSUB : columnName = "SUB"; break;
				case BasicRptTool.eAnalogDataType.MULTI_MOMENT : columnName = "MOMENT"; break;
				default : return member.noneDataString;
			}			

			DataRow		row;
			double		val = 0;

			if(eAiDataType == BasicRptTool.eAnalogDataType.MULTI_MAXSUM)
			{
				bool	bExist = false;
				for(int i = 0; i <= nRow; i++) 
				{
					try
					{					
						row = BasicRptTool.dReadDs.Tables[0].Rows[i];
						if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
						val += ConvertTool.ToDouble(row[columnName].ToString());
						bExist = true;
					}
					catch
					{
					}
				}
				try 
				{
					if(bExist) 
					{
						return val.ToString();						
					}
				}
				catch
				{
				}
				return member.noneDataString;
			}
			else 
			{
				try
				{					
					row = BasicRptTool.dReadDs.Tables[0].Rows[nRow];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) return member.noneDataString;
					return row[columnName].ToString();
				}
				catch
				{
				}
			}
			return member.noneDataString;
		}
		
		public async Task<uint> readAiMultiLineData()
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

			switch(eAiDataType)
			{
				case BasicRptTool.eAnalogDataType.MULTI_AVE : dataType = EnumDataType.AVE; break;
				case BasicRptTool.eAnalogDataType.MULTI_MAXSUM : //여러줄최대값더하기
				case BasicRptTool.eAnalogDataType.MULTI_MAX : dataType = EnumDataType.MAX; break;
				case BasicRptTool.eAnalogDataType.MULTI_MIN : dataType = EnumDataType.MIN; break;
				case BasicRptTool.eAnalogDataType.MULTI_SUM : dataType = EnumDataType.SUM; break;
				case BasicRptTool.eAnalogDataType.MULTI_MAXSUB : dataType = EnumDataType.SUB; break;
				default : return 0;
			}
			
			BasicRptTool.dReadDs = null;
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : BasicRptTool.dReadDs = await gate.GetDataAi(tagName, dataType, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.HOUR : BasicRptTool.dReadDs = await gate.GetDataAi(tagName, dataType, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, 0, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.DAY : BasicRptTool.dReadDs = await gate.GetDataAi(tagName, dataType, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, 0, 0, (int)hap, 1).ConfigureAwait(false); break;					
				case BasicRptTool.eTimeRangeType.MONTH : BasicRptTool.dReadDs = await gate.GetDataAi(tagName, dataType, EnumDataTime.Month, dt.Year, dt.Month, 1, 0, 0, (int)hap, 1).ConfigureAwait(false); break;				
				default : return 0;
			}
			return hap;
		}

		public async Task<uint> readAiMultiLineMomentData()
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

            string data_type = "";
            int nSharpSharpValue = 15;

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

                data_type = sFieldData[11];
                nSharpSharpValue = ConvertTool.ToInt32(sFieldData[12]);				
			}
			catch
			{
			}
			DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
			hap = (uint)BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]);
			if(hap <= 0) return 0;
			
			BasicRptTool.dReadDs = null;
            DataLocal.sMomentDataType = data_type;
            DataLocal.nMomentSharpSharpValue = nSharpSharpValue;

			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : BasicRptTool.dReadDs = await gate.GetDataAi(tagName, EnumDataType.MOMENT, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.HOUR : BasicRptTool.dReadDs = await gate.GetDataAi(tagName, EnumDataType.MOMENT, EnumDataTime.Hour, dt.Year, dt.Month, dt.Day, dt.Hour, min, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.DAY : BasicRptTool.dReadDs = await gate.GetDataAi(tagName, EnumDataType.MOMENT, EnumDataTime.Day, dt.Year, dt.Month, dt.Day, hour, min, (int)hap, 1).ConfigureAwait(false); break;
				case BasicRptTool.eTimeRangeType.MONTH : BasicRptTool.dReadDs = await gate.GetDataAi(tagName, EnumDataType.MOMENT, EnumDataTime.Month, dt.Year, dt.Month, day, hour, min, (int)hap, 1).ConfigureAwait(false); break;					
				default : return 0;
			}
			return hap;
		}

		/*string AiNormalOneLineDataMaxTimeMinute(DateTime dt)			// 최대값 발생시점 시간을 얻은후 최대값이 발생한 분자료를 찾아서 리턴, 2005-09-02 추가
		{
			DataGate	gate = new DataGate();
			DataSet		ds = gate.GetDataAi(tagName, EnumDataType.MAX, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 60, 1);
			
			bool		bExist = false;
			float		val, max = 0.0F;
			int			i, pos = 0;
			DataRow		row;

			for(i = 0; i < 60; i++)
			{
				try
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val = ConvertTool.ToSingle(row["MAX"].ToString());						
					if(bExist) 
					{
						if(val > max) 
						{
							max = val;
							pos = i;
						}
					}
					else 
					{
						max = val;
						pos = i;
					}
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
					DateTime dt2 = BasicRptTool.getCurrentMultiRowDateTime(dt, (int)EnumDataTime.Minute, 0, 0, pos);
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, dt2.Second);
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);	// 자료가 없으면 시간까지만 return, 2005-09-02 추가
		}
		
		string AiNormalOneLineDataMaxTime(DataSet ds, UInt32 hap, DateTime startDt, int dataTypeSort)
		{
			bool		bExist = false;
			float		val, max = 0.0F;
			int			i, pos = 0;
			DataRow		row;

			for(i = 0; i < hap; i++)
			{				
				try
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val = ConvertTool.ToSingle(row["MAX"].ToString());						
					if(bExist) 
					{
						if(val > max) 
						{
							max = val;
							pos = i;
						}
					}
					else 
					{
						max = val;
						pos = i;
					}
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
					DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(startDt, dataTypeSort, 0, 0, pos);
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.HOUR)	// 시간 자료이면 분자료를 얻는다.(시간 아니면 분자료이다), 2005-09-02 추가
						return AiNormalOneLineDataMaxTimeMinute(dt);
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);					
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return member.noneDataString;
		}*/

		async Task<string> AiNormalOneLineDataMaxTimeMinute(string tag, UInt32 hap, DateTime startDt, bool bMax, string defaultString)
		{
			DataLocal						dl = new DataLocal();
			AutoLibLocal.TREND_AI_STRUCT	data = new TREND_AI_STRUCT();
			bool							bExist = false;
			float							val, maxMin = 0.0F;
			int								i, pos = 0;
			DateTime						dt = new DateTime(startDt.Year, startDt.Month, startDt.Day, startDt.Hour, startDt.Minute, startDt.Second);

			for(i = 0; i < hap; i++) 
			{
				try 
				{
					if( await dl.LoadMinDataStructAI(tag, dt, data).ConfigureAwait(false)) 
					{
						if(bMax) val = data.fMax;
						else     val = data.fMin;
						if(bExist) 
						{
							if(bMax) 
							{
								if(val > maxMin) 
								{
									maxMin = val;
									pos = i;
								}
							}
							else 
							{
								if(val < maxMin) 
								{
									maxMin = val;
									pos = i;
								}
							}
						}
						else 
						{
							maxMin = val;
							pos = i;
						}
						bExist = true;
					}
					dt = dt.AddMinutes(1);
				}
				catch {}
			}
			if(bExist) 
			{
				try 
				{
					dt = BasicRptTool.getCurrentMultiRowDateTime(startDt, (int)BasicRptTool.eTimeRangeType.MIN, 0, 0, pos);
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return defaultString;
		}

		async Task<string> AiNormalOneLineDataMaxTimeHour(string tag, UInt32 hap, DateTime startDt, bool bMax)
		{
			DataLocal								dl = new DataLocal();
			AutoLibLocal.HOUR_DATA_ANALOG_STRUCT	data = new HOUR_DATA_ANALOG_STRUCT();
			bool									bExist = false;
			float									val, maxMin = 0.0F;
			int										i, pos = 0;
			DateTime								dt = new DateTime(startDt.Year, startDt.Month, startDt.Day, startDt.Hour, startDt.Minute, startDt.Second);

			for(i = 0; i < hap; i++) 
			{
				try 
				{
					if( await dl.LoadHourDataStructAI(tag, dt, data).ConfigureAwait(false)) 
					{
						if(bMax) val = data.fMaxHour;
						else     val = data.fMinHour;						
						if(bExist) 
						{
							if(bMax) 
							{
								if(val > maxMin) 
								{
									maxMin = val;
									pos = i;
								}
							}
							else 
							{
								if(val < maxMin) 
								{
									maxMin = val;
									pos = i;
								}
							}
						}
						else 
						{
							maxMin = val;
							pos = i;
						}
						bExist = true;
					}
					dt = dt.AddHours(1);
				}
				catch {}
			}
			if(bExist) 
			{
				try 
				{
					dt = BasicRptTool.getCurrentMultiRowDateTime(startDt, (int)BasicRptTool.eTimeRangeType.HOUR, 0, 0, pos);
					string	defaultString = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
					return await AiNormalOneLineDataMaxTimeMinute(tag, 60, dt, bMax, defaultString).ConfigureAwait(false);
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return member.noneDataString;
		}

		async Task<string> AiNormalOneLineDataMaxMinTime(string tag, UInt32 hap, DateTime startDt, int dataTypeSort, bool bMax)
		{
			if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) return await AiNormalOneLineDataMaxTimeMinute(tag, hap, startDt, bMax, member.noneDataString).ConfigureAwait(false);
			else return await AiNormalOneLineDataMaxTimeHour(tag, hap, startDt, bMax).ConfigureAwait(false);
		}	
		

		/*string AiNormalOneLineDataMinTimeMinute(DateTime dt)			// 최소값 발생시점 시간을 얻은후 최소값이 발생한 분자료를 찾아서 리턴, 2005-09-02 추가
		{
			DataGate	gate = new DataGate();
			DataSet		ds = gate.GetDataAi(tagName, EnumDataType.MIN, EnumDataTime.Minute, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 60, 1);
			
			bool		bExist = false;
			float		val, min = 0.0F;
			int			i, pos = 0;
			DataRow		row;

			for(i = 0; i < 60; i++)
			{
				try
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val = ConvertTool.ToSingle(row["MIN"].ToString());						
					if(bExist) 
					{
						if(val < min) 
						{
							min = val;
							pos = i;
						}
					}
					else 
					{
						min = val;
						pos = i;
					}
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
					DateTime dt2 = BasicRptTool.getCurrentMultiRowDateTime(dt, (int)EnumDataTime.Minute, 0, 0, pos);
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, dt2.Second);
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);	// 자료가 없으면 시간까지만 return, 2005-09-02 추가
		}

		string AiNormalOneLineDataMinTime(DataSet ds, UInt32 hap, DateTime startDt, int dataTypeSort)
		{
			bool		bExist = false;
			float		val, min = 0.0F;
			int			i, pos = 0;
			DataRow		row;

			for(i = 0; i < hap; i++)
			{				
				try
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val = ConvertTool.ToSingle(row["MIN"].ToString());						
					if(bExist) 
					{
						if(val < min) 
						{
							min = val;
							pos = i;
						}
					}
					else 
					{
						min = val;
						pos = i;
					}
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
					DateTime dt = BasicRptTool.getCurrentMultiRowDateTime(startDt, dataTypeSort, 0, 0, pos);
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.HOUR)	// 시간 자료이면 분자료를 얻는다.(시간 아니면 분자료이다), 2005-09-02 추가
						return AiNormalOneLineDataMinTimeMinute(dt);
					return String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return member.noneDataString;
		}*/






	}
}

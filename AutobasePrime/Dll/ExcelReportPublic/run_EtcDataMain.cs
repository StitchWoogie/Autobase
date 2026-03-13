using System;
using NetTools;
using AutoLibLocal;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for run_EtcDataMain.
	/// </summary>
	public class run_EtcDataMain
	{
		memberConfigStruct	member;
		BasicRptTool.eEtcDataType eDataType;
		int nElementNum;
		int nColumn;
		int nRow;		
		string[] sFieldData;

		public run_EtcDataMain(memberConfigStruct mem, int num, int elementNum, int columnCount, int rowCount, string[] sData)
		{
			//
			// TODO: Add constructor logic here
			//
			member = mem;
			eDataType = (BasicRptTool.eEtcDataType)num;
			nElementNum = elementNum;
			nColumn = columnCount;
			nRow = rowCount;
			sFieldData = sData;
		}

		public string run()
		{
			string		data;
			DateTime	dt;
			int			dataTypeSort, index = 0, startCurrTime = 0, startUpTime = 0;

			switch(eDataType) 
			{
				case BasicRptTool.eEtcDataType.BASIC_DATE :
					data = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:00", BasicRptTool.reportConfig.dt.Year, BasicRptTool.reportConfig.dt.Month, BasicRptTool.reportConfig.dt.Day, BasicRptTool.reportConfig.dt.Hour);
					return data;
				case BasicRptTool.eEtcDataType.BASIC_TIME :
					data = String.Format("{0,2:d02}:00:00", BasicRptTool.reportConfig.dt.Hour);
					return data;
				case BasicRptTool.eEtcDataType.CURRENT_DATE :
					dt = DateTime.Now;					
					data = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
					return data;
				case BasicRptTool.eEtcDataType.CURRENT_TIME :
					dt = DateTime.Now;
					data = String.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}", dt.Hour, dt.Minute, dt.Second);
					return data;
				case BasicRptTool.eEtcDataType.MULTI_ROW_DATE :
				case BasicRptTool.eEtcDataType.MULTI_ROW_TIME :
					if(nElementNum < 6) return member.noneDataString;
					if(sFieldData[1].Length < 2) return member.noneDataString;
					dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[1], ref index);
					if(dataTypeSort == -1) return member.noneDataString;
					try 
					{
						startCurrTime = ConvertTool.ToInt32(sFieldData[2]);
						startUpTime = ConvertTool.ToInt32(sFieldData[4]);
					}
					catch 
					{
					}
					dt = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, startUpTime, startCurrTime, nRow);
					if(eDataType == BasicRptTool.eEtcDataType.MULTI_ROW_DATE)
						data = String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
					else
						data = String.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}", dt.Hour, dt.Minute, dt.Second);
					return data;
                case BasicRptTool.eEtcDataType.GLOBAL_GET_VAR:
                    data = GlobalVar.GetVarValue(sFieldData[1]);
                    return data;
                case BasicRptTool.eEtcDataType.ST_CURR:
                    return getStCurrTagData();
			}
			return member.noneDataString;
		}

        string getStCurrTagData()
        {
            if (nElementNum < 2) return member.noneDataString;

            string tag = sFieldData[1];
            tag = tag.Trim();
            if (tag.Length <= 0) return member.noneDataString;	// Tag Name

            string curr = "";

            if (ConfigVarTotal.bRunByWebService)
            {
                double fval;
                TagLib.GetTagValue(tag, out curr, out fval);
                return curr;
            }
            else
            {
                if (SharedTag.GetCurr(tag, ref curr) == false) return member.noneDataString;
                return curr;
            }
        }


	}
}

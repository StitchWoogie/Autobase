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
	/// Summary description for run_AnalogMinListData.
	/// </summary>
	public class run_AnalogMinListData
	{
		memberConfigStruct	member;
		BasicRptTool.eAnalogDataType eAiDataType;
		int nElementNum;
		int nColumn;
		int nRow;
		string[] sFieldData;
		string	tagName;
		string[] OrderString = { "StartOrder", "EndOrder" };
		ArrayList	saveArray;

		public run_AnalogMinListData(memberConfigStruct mem, int num, int elementNum, int columnCount, int rowCount, string[] sData)
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
			BasicRptTool.checkAndReadMinListTime();		// 기간자료 시간이 스크립트로 바뀌었냐 ?
		}

		public string run()
		{
			
			if(BasicRptTool.saveDataArray == null) return member.noneDataString;
			if(nRow == 0 && nColumn == 0) 
			{
				if(readMinListData() == false)		// 첫줄이면 데이터를 읽는다.
				{
					BasicRptTool.saveDataArray.Clear();
				}
			}

			if(nColumn >= BasicRptTool.saveDataArray.Count) return member.noneDataString;
			saveArray = (ArrayList)BasicRptTool.saveDataArray[nColumn];			
			if(nRow >= saveArray.Count) return member.noneDataString;
			try
			{
				return (string)saveArray[nRow];
			}
			catch
			{
			}
			return member.noneDataString;
		}

        int GetVarValue(string var, int min, int max)
        {
            int val;

            if (var[0] == '$')
            {
                string retn = GlobalVar.GetVarValue(var.Substring(1));

                val = ConvertTool.ToInt32(retn);
            }
            else
            {
                val = ConvertTool.ToInt32(var);
            }

            if (val < min) val = min;
            if (val > max) val = max;

            return val;
        }

		public bool readMinListData()
		{
			int			i, dataTypeSort, dataGap = 0;
			long		hap;
			EnumDataType dataType = EnumDataType.AVE;
			
			if(nElementNum < 5) return false;
			if(sFieldData[1].Length <= 0 || sFieldData[2].Length <= 0) return false;// Time Period, Time Range
			try 
			{
				dataGap = GetVarValue(sFieldData[1], 1, 60);
				dataTypeSort = ConvertTool.ToInt32(sFieldData[2]);
			}
			catch
			{
				return false;
			}
			if(dataGap <= 0) return false;
			if(dataTypeSort < 0 || dataTypeSort > 1) return false;

			hap = 0;
			try
			{				
				if(dataTypeSort == 0)
					hap = TimeUtil.GetMinHap(BasicRptTool.reportConfig.minListEndDt)-TimeUtil.GetMinHap(BasicRptTool.reportConfig.minListStartDt) + 1;
				else
					hap = TimeUtil.GetHourHap(BasicRptTool.reportConfig.minListEndDt)-TimeUtil.GetHourHap(BasicRptTool.reportConfig.minListStartDt) + 1;
			}
			catch
			{
			}
			if(hap <= 0) return false;
			hap = (long)(hap/dataGap);
			if(hap <= 0) return false;

			int			tagPos, tagHap = (nElementNum-3)/2;
			DateTime	t;
			
			BasicRptTool.saveDataArray.Clear();
			for(tagPos = 0; tagPos <tagHap; tagPos++) 
			{
				saveArray = new ArrayList();
				saveArray.Clear();
				tagName = sFieldData[tagPos*2+3];
				tagName = tagName.Trim();

				if(dataTypeSort == 0) t = new DateTime(BasicRptTool.reportConfig.minListStartDt.Year, BasicRptTool.reportConfig.minListStartDt.Month, BasicRptTool.reportConfig.minListStartDt.Day, BasicRptTool.reportConfig.minListStartDt.Hour, BasicRptTool.reportConfig.minListStartDt.Minute, 0);
				else				  t = new DateTime(BasicRptTool.reportConfig.minListStartDt.Year, BasicRptTool.reportConfig.minListStartDt.Month, BasicRptTool.reportConfig.minListStartDt.Day, BasicRptTool.reportConfig.minListStartDt.Hour, 0, 0);
				
				if((tagName == OrderString[0] && sFieldData[tagPos*2+4] == OrderString[0]) ||	//기간자료 순서 시작시간
					tagName == OrderString[1] && sFieldData[tagPos*2+4] == OrderString[1])		//기간자료 순서 끝시간
				{				
					if(tagName == OrderString[1] && sFieldData[tagPos*2+4] == OrderString[1] &&	dataGap >= 2) 
					{
						if(dataTypeSort == 0) t = t.AddMinutes(dataGap-1);
						else				  t = t.AddHours(dataGap-1);
					}
					for(i = 0; i < hap; i++) 
					{
						saveArray.Add(String.Format("{0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second));
						if(dataTypeSort == 0) t = t.AddMinutes(dataGap);
						else				  t = t.AddHours(dataGap);
 
					}
					BasicRptTool.saveDataArray.Add(saveArray);
					continue;
				}
				
				if(tagName.Length <= 0 ||
					(getMinListTagDataType(sFieldData[tagPos*2+4], ref dataType) == false))
				{
					BasicRptTool.saveDataArray.Add(saveArray);
					continue;
				}

				for(i = 0; i < hap; i++) 
				{
					saveArray.Add(readMinListOneValueData(t, dataType, dataTypeSort, dataGap));
					if(dataTypeSort == 0) t = t.AddMinutes(dataGap);
					else				  t = t.AddHours(dataGap);
 
				}
				BasicRptTool.saveDataArray.Add(saveArray);
			}
			return true;
		}

        
		public UInt32 getMinListDataColumnRowCount()
		{
			int			dataTypeSort, dataGap = 0;
			long		hap;
			
			BasicRptTool.nMultiLineFieldCount = 0;
			if(nElementNum < 5) return 0;
			if(sFieldData[1].Length <= 0 || sFieldData[2].Length <= 0) return 0;// Time Period, Time Range
			try 
			{
                dataGap = GetVarValue(sFieldData[1], 1, 60);
				dataTypeSort = ConvertTool.ToInt32(sFieldData[2]);
			}
			catch
			{
				return 0;
			}
			if(dataGap <= 0) return 0;
			if(dataTypeSort < 0 || dataTypeSort > 1) return 0;

			hap = 0;
			try
			{				
				if(dataTypeSort == 0)
					hap = TimeUtil.GetMinHap(BasicRptTool.reportConfig.minListEndDt)-TimeUtil.GetMinHap(BasicRptTool.reportConfig.minListStartDt) + 1;
				else
					hap = TimeUtil.GetHourHap(BasicRptTool.reportConfig.minListEndDt)-TimeUtil.GetHourHap(BasicRptTool.reportConfig.minListStartDt) + 1;
			}
			catch
			{
			}
			if(hap <= 0) return 0;
			hap = (UInt32)(hap/dataGap);
			if(hap <= 0) return 0;
			BasicRptTool.nMultiLineFieldCount = (nElementNum-3)/2;
			return (UInt32)hap;
		}


		async Task<string> readMinListOneValueData(DateTime t, EnumDataType dataType, int dataTypeSort, int dataGap)
		{
			DataGate	gate = new DataGate();
			DataSet		ds;
			string		readData = "";

			if(dataTypeSort == 0)
				ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Minute, t.Year, t.Month, t.Day, t.Hour, t.Minute, dataGap, 1).ConfigureAwait(false);
			else
				ds = await gate.GetDataAi(tagName, dataType, EnumDataTime.Hour, t.Year, t.Month, t.Day, t.Hour, 0, dataGap, 1).ConfigureAwait(false);

			switch(dataType)
			{
				case EnumDataType.AVE : 
					readData = run_AnalogDataBasicTools.AiAve(member, ds, dataGap);
					break;					
				case EnumDataType.MAX : 
					readData = run_AnalogDataBasicTools.AiMax(member, ds, dataGap);
					break;
				case EnumDataType.MIN : 
					readData = run_AnalogDataBasicTools.AiMin(member, ds, dataGap);
					break;
                case EnumDataType.MOMENT:
                    readData = run_AnalogDataBasicTools.AiMoment(member, ds, dataGap);
                    break;
				case EnumDataType.SUM : 
					readData = run_AnalogDataBasicTools.AiSumAndMaxSub(member, ds, dataGap, "SUM");
					break;
				case EnumDataType.SUB : 
					readData = run_AnalogDataBasicTools.AiSumAndMaxSub(member, ds, dataGap, "SUB");
					break;
				default : return member.noneDataString;
				
			}
			if(readData == member.noneDataString || readData == "") return member.noneDataString;
			return readData;
		}
	


		bool getMinListTagDataType(string typeName, ref EnumDataType dataType)
		{
			if(typeName == "AVE") dataType = EnumDataType.AVE;
			else if(typeName == "MAX") dataType = EnumDataType.MAX;
			else if(typeName == "MIN") dataType = EnumDataType.MIN;				
			else if(typeName == "SUM") dataType = EnumDataType.SUM;				
			else if(typeName == "SUB") dataType = EnumDataType.SUB;
            else if (typeName == "MOMENT") dataType = EnumDataType.MOMENT;
			else return false;
			return true;
		}



	}
}

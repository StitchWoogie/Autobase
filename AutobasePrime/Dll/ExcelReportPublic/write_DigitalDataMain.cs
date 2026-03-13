using System;
using System.Data;
using DatabaseConnection;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;
using System.Threading.Tasks;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for write_DigitalDataMain.
	/// </summary>
	public class write_DigitalDataMain
	{
		memberConfigStruct	member;
		BasicRptTool.eDigitalDataType eDiDataType;
		int nElementNum;
		string[] sFieldData;
		string	tagName;
		DateTime save_t;

		int			dataTypeSort;
		int[]		currTime = new int[2], upTime = new int[2];
        
		public write_DigitalDataMain()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public async Task<bool> diDataWriteValue(memberConfigStruct mem, int num, int elementNum, string val, string buf, string val2, string[] sData)
		{
			member = mem;
			eDiDataType = (BasicRptTool.eDigitalDataType)num;
			nElementNum = elementNum;
			sFieldData = sData;

			if(nElementNum < 7) return false;
			if(checkWriteableObject() == false) return false;	// 쓰기가능 오브젝트가 아니다.
			if(checkWriteValue(val) == false) return false;	// 출력값이 범위를 벗어났다.

			string		readData;

			run_DigitalDataMain di = new run_DigitalDataMain(member, num, nElementNum, 0, 0, sFieldData);
			readData = await di.run().ConfigureAwait(false);
			if(readData == val) return false;

			// write code here
			switch(eDiDataType) 
			{
				case BasicRptTool.eDigitalDataType.ONTIME : 		//ON 시간
				case BasicRptTool.eDigitalDataType.OFFTIME : 		//OFF 시간
					return  await writeDiData(val, val2, 1).ConfigureAwait(false);
				default : return await writeDiData(val, val2, 0).ConfigureAwait(false);
			}			
		}

		async Task<bool> writeDiData(string val, string val2, int displayType)		// displayType 0 = 일반, 1 = 시간
		{
			int		curr;

			try 
			{
				if( displayType == 1 ) 
				{ 
					if(val2.IndexOf(':') != -1) 
					{ 
						curr = BasicRptTool.convertTimeStringToInt(val2);
						if(curr == -1) return false;						
					}
					else
						curr = ConvertTool.ToInt32(val2);		// 1,234 와 같이 콤마로 구분된 숫자의 콤마를 없애기 위해
				}
				else
					curr = ConvertTool.ToInt32(val);			// 1,234 와 같이 콤마로 구분된 숫자의 콤마를 없애기 위해
				
			}
			catch
			{
				return false;
			}

			DataLocal dLocal = new DataLocal();
			TREND_DI_STRUCT				data = new TREND_DI_STRUCT();
			HOUR_DATA_DIGITAL_STRUCT	hourData = new HOUR_DATA_DIGITAL_STRUCT();

			if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN)
				await dLocal.LoadMinDataStructDI(tagName, save_t, data).ConfigureAwait(false);			
			else 
				await dLocal.LoadHourDataStructDI(tagName, save_t, hourData).ConfigureAwait(false);


			switch(eDiDataType) 
			{
				case BasicRptTool.eDigitalDataType.DI_CURR : return false;// 나중에 출력가능 ?	
				case BasicRptTool.eDigitalDataType.ONTIME : 		//ON 시간					
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.cOnTime = (byte)curr;
						return await DataSave.SaveMinDataStructDI(tagName, save_t, data, true);						
					}
					else 
					{
						hourData.dwOnTime = (uint)curr;
						return await DataSave.SaveHourDataStructDI(tagName, save_t, hourData);					
					}
				case BasicRptTool.eDigitalDataType.OFFTIME : 		//OFF 시간					
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.cOnTime = (byte)(60-curr);
						return await DataSave.SaveMinDataStructDI(tagName, save_t, data, true);						
					}
					else 
					{
						hourData.dwOnTime = (uint)(3600-curr);
						return await DataSave.SaveHourDataStructDI(tagName, save_t, hourData);					
					}
				case BasicRptTool.eDigitalDataType.ONCOUNT :		//ON 횟수
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.nCountOnOff = (short)curr;
						return await DataSave.SaveMinDataStructDI(tagName, save_t, data, true);						
					}
					else 
					{
						hourData.wCountOnOff = (ushort)curr;
						return await DataSave.SaveHourDataStructDI(tagName, save_t, hourData);					
					}
				case BasicRptTool.eDigitalDataType.DI_MOMENT :		//순시값
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.bOnOff = (byte)curr;
						return await DataSave.SaveMinDataStructDI(tagName, save_t, data, true);						
					}
					return false;
			}
			return false;
		}

		bool checkWriteableObject()
		{
			switch(eDiDataType) 
			{
				case BasicRptTool.eDigitalDataType.DI_CURR : return false;// 나중에 출력가능 ?	
				case BasicRptTool.eDigitalDataType.ONTIME : 		//ON 시간
				case BasicRptTool.eDigitalDataType.OFFTIME : 		//OFF 시간
				case BasicRptTool.eDigitalDataType.ONCOUNT :		//ON 횟수					
				case BasicRptTool.eDigitalDataType.DI_MOMENT :		//순시값
					tagName = sFieldData[1];
					tagName = tagName.Trim();
					if(tagName.Length <= 0 || sFieldData[2].Length < 2) return false;	// Tag Name, Time Type error					
					dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[2]);
					if(dataTypeSort == -1) return false;
					if(checkDataTypeSort(eDiDataType, dataTypeSort) == false) return false;	// 분, 시간 자료가 아닐 때, : 순시값은 분자료만
					try 
					{
						currTime[0] = ConvertTool.ToInt32(sFieldData[3]);
						currTime[1] = ConvertTool.ToInt32(sFieldData[4]);
						upTime[0] = ConvertTool.ToInt32(sFieldData[5]);
						upTime[1] = ConvertTool.ToInt32(sFieldData[6]);
					}
					catch
					{
						return false;
					}
					save_t = BasicRptTool.getCurrentMultiRowDateTime(BasicRptTool.reportConfig.dt, dataTypeSort, upTime[0], currTime[0], 0);
					if(BasicRptTool.getTotalTimeGapCount(BasicRptTool.reportConfig.dt, dataTypeSort, currTime[0], currTime[1], upTime[0], upTime[1]) != 1) return false;	// 여러줄이면
					return true;
			}
			return false;

		}


		bool checkWriteValue(string val)
		{
			int		curr;

			try 
			{
				curr = ConvertTool.ToInt32(val);				
			}
			catch
			{
				return false;
			}

			switch(eDiDataType) 
			{
				case BasicRptTool.eDigitalDataType.DI_CURR : return false;// 나중에 출력가능 ?	
				case BasicRptTool.eDigitalDataType.ONTIME : 		//ON 시간					
				case BasicRptTool.eDigitalDataType.OFFTIME : 		//OFF 시간
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN)
					{
						if(curr >= 0 && curr <= 60) return true;	// 0 ~ 60 초
						return false;
					}
					if(curr >= 0 && curr <= 3600) return true;		// 0 ~ 3600 초
					return false;				
				case BasicRptTool.eDigitalDataType.ONCOUNT :		//ON 횟수
					if(curr < 0) return false;						// - 이면 에러
					return true;
				case BasicRptTool.eDigitalDataType.DI_MOMENT :		//순시값
					if(curr == 0 || curr == 1) return true;			// OFF/ON
					return false;
			}
			return false;
		}



		bool checkDataTypeSort(BasicRptTool.eDigitalDataType eDiDataType, int dataTypeSort)
		{
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : return true;
				case BasicRptTool.eTimeRangeType.HOUR : 
					if(eDiDataType == BasicRptTool.eDigitalDataType.DI_MOMENT) return false;
					return true;
				default : return false;
			}
		}







	}
}

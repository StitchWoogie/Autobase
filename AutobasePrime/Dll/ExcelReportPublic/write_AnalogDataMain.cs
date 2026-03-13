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
	/// Summary description for write_AnalogDataMain.
	/// </summary>
	public class write_AnalogDataMain
	{
		memberConfigStruct	member;
		BasicRptTool.eAnalogDataType eAiDataType;
		int nElementNum;
		string[] sFieldData;
		string	tagName;
		DateTime save_t;

		int			dataTypeSort;		// 데이터종류
		int[]		currTime = new int[2], upTime = new int[2];


		public write_AnalogDataMain()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public async Task<bool> aiDataWriteValue(memberConfigStruct mem, int num, int elementNum, string val, string buf, string[] sData)
		{
			member = mem;
			eAiDataType = (BasicRptTool.eAnalogDataType)num;
			nElementNum = elementNum;
			sFieldData = sData;

			if(nElementNum < 7) return false;
			if(checkWriteableObject() == false) return false;

			string		readData;

			run_AnalogDataMain ai = new run_AnalogDataMain(member, num, nElementNum, 0, 0, sFieldData);
			readData = await ai.run();
			if(readData == val) return false;

			// write code here
			return await WriteAiValue(val);
		}

		
		async Task<bool> WriteAiValue(string val)
		{
			float		curr;

			try 
			{
				curr = ConvertTool.ToSingle(val);
			}
			catch
			{
				return false;
			}

			DataLocal dLocal = new DataLocal();
			TREND_AI_STRUCT				data = new TREND_AI_STRUCT();
			HOUR_DATA_ANALOG_STRUCT		hourData = new HOUR_DATA_ANALOG_STRUCT();

			if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN)
				await dLocal.LoadMinDataStructAI(tagName, save_t, data);			
			else 
				await dLocal.LoadHourDataStructAI(tagName, save_t, hourData);

			switch(eAiDataType) 
			{
				case BasicRptTool.eAnalogDataType.AI_CURR : return false;// 나중에 출력가능 ?					
				case BasicRptTool.eAnalogDataType.AI_AVE : 	//평균
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.fAverage = curr;
						return await DataSave.SaveMinDataStructAI(tagName, save_t, data, true);						
					}
					else 
					{
						hourData.fAveHour = curr;
						return await DataSave.SaveHourDataStructAI(tagName, save_t, hourData);					
					}
				case BasicRptTool.eAnalogDataType.AI_MAX : 	//최대
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.fMax = curr;
						return	 await DataSave.SaveMinDataStructAI(tagName, save_t, data, true);						
					}
					else 
					{
						hourData.fMaxHour = curr;
						return await DataSave.SaveHourDataStructAI(tagName, save_t, hourData);					
					}
				case BasicRptTool.eAnalogDataType.AI_MIN :	//최소
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.fMin = curr;
						return await DataSave.SaveMinDataStructAI(tagName, save_t, data, true);						
					}
					else 
					{
						hourData.fMinHour = curr;
						return await DataSave.SaveHourDataStructAI(tagName, save_t, hourData);					
					}
				case BasicRptTool.eAnalogDataType.AI_SUM :	//적산
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.fSumMin = curr;
						return await DataSave.SaveMinDataStructAI(tagName, save_t, data, true);						
					}
					else 
					{
						hourData.fSumHour = curr;
						return await DataSave.SaveHourDataStructAI(tagName, save_t, hourData);					
					}
				case BasicRptTool.eAnalogDataType.AI_MOMENT :	//순시
					if((BasicRptTool.eTimeRangeType)dataTypeSort == BasicRptTool.eTimeRangeType.MIN) 
					{
						data.fCurr = curr;
						return await DataSave.SaveMinDataStructAI(tagName, save_t, data, true);						
					}
					return false;
			}
			return false;
		}


		bool checkWriteableObject()
		{
			switch(eAiDataType) 
			{
				case BasicRptTool.eAnalogDataType.AI_CURR : return false;// 나중에 출력가능 ?					
				case BasicRptTool.eAnalogDataType.AI_AVE : 	//평균
				case BasicRptTool.eAnalogDataType.AI_MAX : 	//최대
				case BasicRptTool.eAnalogDataType.AI_MIN :	//최소
				case BasicRptTool.eAnalogDataType.AI_SUM :	//적산
				case BasicRptTool.eAnalogDataType.AI_MOMENT ://순시
					tagName = sFieldData[1];
					tagName = tagName.Trim();
					if(tagName.Length <= 0 || sFieldData[2].Length < 2) return false;	// Tag Name, Time Type error					
					dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[2]);
					if(dataTypeSort == -1) return false;
					if(checkDataTypeSort(dataTypeSort) == false) return false;	// 분, 시간 자료가 아닐 때

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

		bool checkDataTypeSort(int dataTypeSort)
		{
			switch((BasicRptTool.eTimeRangeType)dataTypeSort)
			{
				case BasicRptTool.eTimeRangeType.MIN : 
				case BasicRptTool.eTimeRangeType.HOUR : return true;
				default : return false;
			}
		}



	}
}

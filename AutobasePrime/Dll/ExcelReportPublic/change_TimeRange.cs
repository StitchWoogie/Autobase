using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;	
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;	
using AutoLibLocal;
using NetTools;
using DatabaseConnection;
using DialogAddition;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for change_TimeRange.
	/// </summary>
	public class change_TimeRange
	{
		ConnectionStringList	dsnList;
		ArrayList				memberArr;
		string					data;
		int						nElementNum = 0;
		memberConfigStruct		member;
		string[]				sFieldData;

		public change_TimeRange(ConnectionStringList dsn, ArrayList memArr, string buf)
		{
			//
			// TODO: Add constructor logic here
			//
			dsnList = dsn;
			memberArr = memArr;
			data = buf;			
		}

		public string run()
		{
			int					i, nMainType = 0, memPos = -1;
			
			if(memberArr == null) return data;
			memPos = BasicRptTool.getMatchMemberArrCellDataValue(memberArr, data, ref nElementNum);
			if(memPos == -1 || memPos >= memberArr.Count) return data;			

			member = (memberConfigStruct)memberArr[memPos];
			i = BasicRptTool.getMainDataTypeNum(member.sDataTitle, ref nMainType);
			if(nMainType == -1) return data;
			if(i == -1) return data;				// 맞는 인식문자열이 없다

			BasicRptTool.eMainDataType num = (BasicRptTool.eMainDataType)nMainType;
			switch(num)
			{
				case BasicRptTool.eMainDataType.DB_DATA : return changeDbTimeRange((BasicRptTool.eDbDataType)i);
				case BasicRptTool.eMainDataType.ANALOG : return changeAnalogTimeRange((BasicRptTool.eAnalogDataType)i);
				case BasicRptTool.eMainDataType.DIGITAL : return changeDigitalTimeRange((BasicRptTool.eDigitalDataType)i);
				case BasicRptTool.eMainDataType.ETC : return changeEtcTimeRange((BasicRptTool.eEtcDataType)i);
			}
			return data;
		}

		string changeDbTimeRange(BasicRptTool.eDbDataType eDataType)
		{
			switch(eDataType) 
			{
				case BasicRptTool.eDbDataType.DB_DATA :
					return ChangeTimeRange(7, 3);
				case BasicRptTool.eDbDataType.MULTI_COLUMN_ROW :
					return ChangeTimeRange(10, 4);
				case BasicRptTool.eDbDataType.DB_MULTI_DATA :
					return ChangeTimeRange(9, 3);
				case BasicRptTool.eDbDataType.DB_AI_MAXTIME :
				case BasicRptTool.eDbDataType.DB_AI_MINTIME :
					return ChangeTimeRange(7, 3);
			}
			return data;
		}

		string changeAnalogTimeRange(BasicRptTool.eAnalogDataType eAiDataType)
		{
			switch(eAiDataType) 
			{
				case BasicRptTool.eAnalogDataType.AI_AVE : 	//평균
				case BasicRptTool.eAnalogDataType.AI_MAX : 	//최대
				case BasicRptTool.eAnalogDataType.AI_MIN :	//최소
				case BasicRptTool.eAnalogDataType.AI_SUM : 	//적산
				case BasicRptTool.eAnalogDataType.AI_MAXSUB : //최대값차이
				case BasicRptTool.eAnalogDataType.AI_MAXSUM : //최대값더하기
				case BasicRptTool.eAnalogDataType.AI_MAXTIME : //최대값발생시점
				case BasicRptTool.eAnalogDataType.AI_MINTIME : //최소값발생시점					
				case BasicRptTool.eAnalogDataType.AI_MOMENT :	//순시값					
				case BasicRptTool.eAnalogDataType.MULTI_AVE :	//여러줄 평균값
				case BasicRptTool.eAnalogDataType.MULTI_MAXSUM ://여러줄 최대값더하기
				case BasicRptTool.eAnalogDataType.MULTI_MAX :	//여러줄 최대값
				case BasicRptTool.eAnalogDataType.MULTI_MIN :	//여러줄 최소값
				case BasicRptTool.eAnalogDataType.MULTI_SUM :	//여러줄 적산값
				case BasicRptTool.eAnalogDataType.MULTI_MAXSUB ://여려줄 최대값차이
				case BasicRptTool.eAnalogDataType.MULTI_MOMENT ://여러줄 순시값
					return ChangeTimeRange(7, 2);
			}
			return data;
		}

		string changeDigitalTimeRange(BasicRptTool.eDigitalDataType eDiDataType)
		{
			switch(eDiDataType) 
			{
				case BasicRptTool.eDigitalDataType.ONTIME : 		//ON 시간
				case BasicRptTool.eDigitalDataType.OFFTIME : 		//OFF 시간
				case BasicRptTool.eDigitalDataType.ONCOUNT :		//ON 횟수
				case BasicRptTool.eDigitalDataType.DI_MOMENT :		//순시값
				case BasicRptTool.eDigitalDataType.MULTI_ONTIME :	//여러줄 ON 시간
				case BasicRptTool.eDigitalDataType.MULTI_OFFTIME :	//여러줄 OFF 시간
				case BasicRptTool.eDigitalDataType.MULTI_ONCOUNT :	//여러줄 ON 횟수
				case BasicRptTool.eDigitalDataType.MULTI_DI_MOMENT ://여러줄 순시값
					return ChangeTimeRange(7, 2);
				case BasicRptTool.eDigitalDataType.ONOFFLIST :		//ON_OFF LISt
				case BasicRptTool.eDigitalDataType.ONOFFLIST_SUM :	//ON_OFF LISt 가동시간합산
				case BasicRptTool.eDigitalDataType.MULTI_ONOFFLIST_SUM ://여러줄 ON_OFF LISt 가동시간합산
					return ChangeTimeRange(8, 3);
			}
			return data;
		}

		string changeEtcTimeRange(BasicRptTool.eEtcDataType eDataType)
		{
			switch(eDataType) 
			{
				case BasicRptTool.eEtcDataType.MULTI_ROW_DATE :
				case BasicRptTool.eEtcDataType.MULTI_ROW_TIME :
					return ChangeTimeRange(6, 1);
				case BasicRptTool.eEtcDataType.ALARM_DATA :
					return ChangeTimeRange(7, 2);
			}
			return data;
		}

		string ChangeTimeRange(int minCount, int timePos)
		{
			if(nElementNum < minCount) return data;

			int			dataTypeSort;
			int[]		currTime = new int[2], upTime = new int[2];

			sFieldData = BasicRptTool.sFieldData;

			if(BasicRptTool.cSaveChange == 0) 
			{
				dataTypeSort = BasicRptTool.getReadDataSortType(sFieldData[timePos]);
				if(dataTypeSort == -1) dataTypeSort = 1; // 기본시간으로 설정
				try 
				{
					currTime[0] = ConvertTool.ToInt32(sFieldData[timePos+1]);
					currTime[1] = ConvertTool.ToInt32(sFieldData[timePos+2]);
					upTime[0] = ConvertTool.ToInt32(sFieldData[timePos+3]);
					upTime[1] = ConvertTool.ToInt32(sFieldData[timePos+4]);
				}
				catch
				{
				}

				BasicRptTool.cSaveChange = 1;
				if(runDateTimeDlg(ref dataTypeSort, ref currTime, ref upTime)) 
				{
					BasicRptTool.cSaveChange = 255;
					BasicRptTool.nSaveDataTypeSort = dataTypeSort;
					BasicRptTool.nSaveCurrTime[0] = currTime[0];
					BasicRptTool.nSaveCurrTime[1] = currTime[1];
					BasicRptTool.nSaveUpTime[0] = upTime[0];
					BasicRptTool.nSaveUpTime[1] = upTime[1];
					return setTimeRangeData(timePos);
				}
				return data;
			}
			else 
			{
				return setTimeRangeData(timePos);
			}
		}

		string setTimeRangeData(int timePos)
		{
			string		buf = "";
			int			i;

			try 
			{
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
				for(i = 0; i < timePos; i++) 
				{					
					buf += sFieldData[i];
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				}
				buf += BasicRptTool.sTimeTypeRealBuf[BasicRptTool.nSaveDataTypeSort % 5];
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += BasicRptTool.nSaveCurrTime[0];
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += BasicRptTool.nSaveCurrTime[1];
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += BasicRptTool.nSaveUpTime[0];
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += BasicRptTool.nSaveUpTime[1];
				for(i = timePos+5; i < nElementNum; i++) 
				{	
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
					buf += sFieldData[i];					
				}
				return buf;
			}
			catch
			{
			}
			return data;
		}

		bool runDateTimeDlg(ref int timeRange, ref int[] currTime, ref int [] upTime)
		{
			edit_TimeRangeDlg dialog = new edit_TimeRangeDlg();

			dialog.timeRange = (BasicRptTool.eTimeRangeType)timeRange;
			dialog.setCurrentTimeRange();
			dialog.nUpTimeRange[0] = upTime[0];
			dialog.nCurrTimeRange[0] = currTime[0];
			dialog.nUpTimeRange[1] = upTime[1];
			dialog.nCurrTimeRange[1] = currTime[1];			
			
			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				timeRange = (int)dialog.timeRange;
				upTime[0] = dialog.nUpTimeRange[0];
				currTime[0] = dialog.nCurrTimeRange[0];
				upTime[1] = dialog.nUpTimeRange[1];
				currTime[1] = dialog.nCurrTimeRange[1];				
				return true;
			}
			return false;
		}






	}
}

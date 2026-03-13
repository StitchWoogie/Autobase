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
	/// Summary description for write_DbDataMain.
	/// </summary>
	public class write_DbDataMain
	{
		public write_DbDataMain()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public async Task< bool> dbDataWriteValue(ConnectionStringList dsnList, ArrayList memberArr, memberConfigStruct member, int num, int nElementNum, string val, string buf, string val2)
		{
			if(nElementNum < 4) return false;
			if((BasicRptTool.eDbDataType)num != BasicRptTool.eDbDataType.DB_DATA) return false;

			string		readData, sOptionWhere = "";
			int			dataTypeSort, upTime = 0, index = 0, displayType = 0;
			
			readData = await BasicRptTool.getMainCellDataValue(dsnList, memberArr, buf, 0, 0);//column = 0, row = 0
			if(readData == val) return false;

			if(BasicRptTool.sFieldData[3].Length < 2) return false;
			dataTypeSort = BasicRptTool.getReadDataSortType(BasicRptTool.sFieldData[3], ref index);
			if(dataTypeSort == -1) return false;
			ConnectionString dsn = dsnList.GetConnection(member.dsnName);

			if(BasicRptTool.sFieldData[3].Length > 2 || nElementNum <= 4) 
			{		// HH00 형태이거나, 데이터요소 수가 3개일 때...
				upTime = 0;
			}
			else 
			{
				try 
				{
					index = ConvertTool.ToInt32(BasicRptTool.sFieldData[4]);
					if(nElementNum <= 7) // 기존 시간 데이터 형식
					{
						if(nElementNum >= 6) upTime = ConvertTool.ToInt32(BasicRptTool.sFieldData[5]);
					}
					else				// 현재 시간 데이터 형식
					{
						if(nElementNum >= 7) upTime = ConvertTool.ToInt32(BasicRptTool.sFieldData[6]);
						if(nElementNum >= 10) displayType = ConvertTool.ToInt32(BasicRptTool.sFieldData[9]);
                        if (nElementNum >= 11) sOptionWhere = run_DbDataMain.GetStringOrgOrVar(BasicRptTool.sFieldData[10]);
					}					
				}
				catch 
				{
				}
			}

			double		curr;
			string		sVal;

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
						curr = ConvertTool.ToDouble(val2);			// 1,234 와 같이 콤마로 구분된 숫자의 콤마를 없애기 위해
				}
				else
					curr = ConvertTool.ToDouble(val);			// 1,234 와 같이 콤마로 구분된 숫자의 콤마를 없애기 위해
				sVal = curr.ToString();
			}
			catch
			{
				return false;
			}			

			if(readData == member.noneDataString)
				return DB_Data_Read_Write.dbDataWrite(dsn, BasicRptTool.sFieldData[1], BasicRptTool.sFieldData[2], member.sDateColumnName, member.nTimeFormat, sOptionWhere, BasicRptTool.reportConfig.dt, dataTypeSort, upTime, index, sVal, true);		// Add
			else 
				return DB_Data_Read_Write.dbDataWrite(dsn, BasicRptTool.sFieldData[1], BasicRptTool.sFieldData[2], member.sDateColumnName, member.nTimeFormat, sOptionWhere, BasicRptTool.reportConfig.dt, dataTypeSort, upTime, index, sVal, false);	// Modify
		}


	}
}

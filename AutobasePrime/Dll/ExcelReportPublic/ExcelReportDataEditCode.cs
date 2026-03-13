using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;	
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;	
using AutoLibLocal;
using NetTools;
using DatabaseSaveList;
using DatabaseConnection;
using Microsoft.Win32;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for ExcelReportDataEditCode.
	/// </summary>
	public class ExcelReportDataEditCode
	{
		public ArrayList		memberArr;
		memberConfigStruct		member;
		//public DatabaseSaveListClass classSaveList = null;
		//public ConnectionStringList dsnList = null;

		public ExcelReportDataEditCode(ArrayList arr)
		{
			//
			// TODO: Add constructor logic here
			//
			memberArr = arr;			
		}

		public string insertCellInputCommand(string data, bool bInsert)
		{
			int					memPos = -1, mainItemPos = 0, subItemPos = 0;
			bool				bChangeItemFlag = false;
			
			ReportMainDataSortSelectDlg dlg = new ReportMainDataSortSelectDlg();

			dlg.memberArr = memberArr;
			dlg.nMainDataNum = 0;

			if(bInsert == true)	// 원래 삽입 모드일 때 = true, 수정모드에서 불러질 때는 false
			{				
				BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);
				if(memPos != -1 && memPos < memberArr.Count) 
				{
					member = (memberConfigStruct)memberArr[memPos];
					subItemPos = BasicRptTool.getMainDataTypeNum(member.sDataTitle, ref mainItemPos);
					if(mainItemPos == -1) mainItemPos = 0;
					if(subItemPos == -1) subItemPos = 0;
					dlg.nMainDataNum = mainItemPos;
					dlg.nSubDataNum = subItemPos;
				}
			}
			else bChangeItemFlag = true;			
			
			dlg.ShowDialog();
			if(dlg.bOkFlag == true) 
			{
				if(mainItemPos != dlg.nMainDataNum || subItemPos != dlg.m_list.SelectedIndex) bChangeItemFlag = true;
				switch((BasicRptTool.eMainDataType)dlg.nMainDataNum)
				{
					case BasicRptTool.eMainDataType.DB_DATA :
						edit_CodeDbMain dbSub = new edit_CodeDbMain(memberArr);
						return dbSub.mainCellInputCommand(data, (BasicRptTool.eDbDataType)dlg.m_list.SelectedIndex, bChangeItemFlag);
					case BasicRptTool.eMainDataType.ANALOG :
						edit_CodeAnalogMain aiSub = new edit_CodeAnalogMain(memberArr);
						return aiSub.mainCellInputCommand(data, (BasicRptTool.eAnalogDataType)dlg.m_list.SelectedIndex, bChangeItemFlag);
					case BasicRptTool.eMainDataType.DIGITAL : 
						edit_CodeDigitalMain diSub = new edit_CodeDigitalMain(memberArr);
						return diSub.mainCellInputCommand(data, (BasicRptTool.eDigitalDataType)dlg.m_list.SelectedIndex, bChangeItemFlag);
					case BasicRptTool.eMainDataType.ETC :						
						//sDataTitle = BasicRptTool.data[dlg.nMainDataNum % 4].sDataTitle[dlg.m_list.SelectedIndex % BasicRptTool.nTotalDataTypeCount];
						//count = BasicRptTool.getCurrentMainDataItemCount(memberArr, sDataTitle);
						//if(count == 1) 
						//{
						//	buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
							//buf += BasicRptTool.getCurrentMainDataRealFirstBuf(memberArr, sDataTitle);
							//return buf;
						//}
						edit_CodeEtcMain etcSub = new edit_CodeEtcMain(memberArr);
						return etcSub.mainCellInputCommand(data, (BasicRptTool.eEtcDataType)dlg.m_list.SelectedIndex, bChangeItemFlag);
				}
				return data;
			}
			return data;
		}

		public string modifyCellInputCommand(string data)
		{
			int					memPos = -1, mainItemPos = 0, nSubItemNum;
			
			BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);						
			if(memPos != -1 && memPos < memberArr.Count) 
			{
				member = (memberConfigStruct)memberArr[memPos];
				nSubItemNum = BasicRptTool.getMainDataTypeNum(member.sDataTitle, ref mainItemPos);
				if(mainItemPos == -1 || nSubItemNum == -1) return insertCellInputCommand(data, false);		// 읽은 데이터가 무엇인지 모를 때
			}
			else return insertCellInputCommand(data, false);
			
			switch((BasicRptTool.eMainDataType)mainItemPos)
			{
				case BasicRptTool.eMainDataType.DB_DATA :
					edit_CodeDbMain dbSub = new edit_CodeDbMain(memberArr);
					return dbSub.mainCellInputCommand(data, (BasicRptTool.eDbDataType)nSubItemNum, false);
				case BasicRptTool.eMainDataType.ANALOG :
					edit_CodeAnalogMain aiSub = new edit_CodeAnalogMain(memberArr);
					return aiSub.mainCellInputCommand(data, (BasicRptTool.eAnalogDataType)nSubItemNum, false);
				case BasicRptTool.eMainDataType.DIGITAL : 
					edit_CodeDigitalMain diSub = new edit_CodeDigitalMain(memberArr);
					return diSub.mainCellInputCommand(data, (BasicRptTool.eDigitalDataType)nSubItemNum, false);
				case BasicRptTool.eMainDataType.ETC :
					edit_CodeEtcMain etcSub = new edit_CodeEtcMain(memberArr);
					return etcSub.mainCellInputCommand(data, (BasicRptTool.eEtcDataType)nSubItemNum, false);
			}			
			return data;
		}

	}
}

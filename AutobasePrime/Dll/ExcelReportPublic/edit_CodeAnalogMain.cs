using System;
//using Microsoft.Office.Core;
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
	/// Summary description for edit_CodeAnalogMain.
	/// </summary>
	public class edit_CodeAnalogMain
	{
		public ArrayList		memberArr;
		memberConfigStruct		member;

		public edit_CodeAnalogMain(ArrayList arr)
		{
			//
			// TODO: Add constructor logic here
			//
			memberArr = arr;
		}

		public string mainCellInputCommand(string data, BasicRptTool.eAnalogDataType nSubItem, bool bChangeItemFlag)
		{
			switch(nSubItem)
			{
				case BasicRptTool.eAnalogDataType.AI_CURR :
					return CurrCellInputCommand(data, nSubItem, bChangeItemFlag, false, false);
				case BasicRptTool.eAnalogDataType.AI_MOMENT :
				case BasicRptTool.eAnalogDataType.MULTI_MOMENT :
					return AiMomentCellInputCommand(data, nSubItem, bChangeItemFlag);
				case BasicRptTool.eAnalogDataType.AI_MINLIST :
					return AiMinListCellInputCommand(data, nSubItem, bChangeItemFlag);
				default : 
					return CurrCellInputCommand(data, nSubItem, bChangeItemFlag, true, true);
			}			
		}


		public string CurrCellInputCommand(string data, BasicRptTool.eAnalogDataType nSubItem, bool bChangeItemFlag, bool bUseMultiRow, bool bUseRange)
		{
			int					i, memPos = -1;
			string				buf = "";			
			edit_AiDiBasicDlg	dialog = new edit_AiDiBasicDlg();
			
			try 
			{
				dialog.Text += " - " + BasicRptTool.data[1].sMainDataTypeBuf[(int)nSubItem];
			}
			catch
			{
			}

			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[1].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Data_Type.Items.Add(member.dataName);
			}
			if(dialog.comboBox_Data_Type.Items.Count > 0) dialog.comboBox_Data_Type.SelectedIndex = 0;
			dialog.bMultiRow = bUseMultiRow;
			dialog.bUseRange = bUseRange;
			dialog.timeRange = BasicRptTool.eTimeRangeType.HOUR;
			dialog.nCurrTimeRange[0] = 0;
			dialog.nUpTimeRange[0] = 0;
			dialog.nCurrTimeRange[1] = 0;
			dialog.nUpTimeRange[1] = 0;
			dialog.nTagTypePos = 0;			// Analog

			if(bChangeItemFlag == false) 
			{
				i = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);		

				try
				{			
					if(i >= 1) dialog.comboBox_Data_Type.Text = BasicRptTool.sFieldData[0];
					if(i >= 2) dialog.textBox_Tag.Text = BasicRptTool.sFieldData[1];
					if(bUseRange) 
					{
						if(i >= 3) dialog.timeRange = (BasicRptTool.eTimeRangeType)BasicRptTool.getReadDataSortType(BasicRptTool.sFieldData[2]);
						if(i >= 4) dialog.nCurrTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[3]);
						if(i >= 5) dialog.nCurrTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[4]);	
						if(i >= 6) dialog.nUpTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[5]);
						if(i >= 7) dialog.nUpTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[6]);
						if(i >= 8) dialog.checkBox_Seletced_DataTime.Checked = (ConvertTool.ToInt32(BasicRptTool.sFieldData[7]) == 1) ? true : false;
					}
				}
				catch 
				{
				}
			}			
			dialog.ShowDialog();
			if(dialog.bOkFlag == true)
			{
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.comboBox_Data_Type.Text;
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.textBox_Tag.Text;
				if(bUseRange) 
				{					
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
					buf += BasicRptTool.sTimeTypeRealBuf[(int)dialog.timeRange % 5];
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
					i = dialog.nCurrTimeRange[0];
					buf += i.ToString();
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
					i = dialog.nCurrTimeRange[1];
					buf += i.ToString();
					if(bUseMultiRow) 
					{
						buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
						i = dialog.nUpTimeRange[0];
						buf += i.ToString();
						buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
						i = dialog.nUpTimeRange[1];
						buf += i.ToString();
						buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
						i  = ( dialog.checkBox_Seletced_DataTime.Checked ) ? 1 : 0;
						buf += i.ToString();
					}
				}
				return buf;				
			}
			return data;
		}

		public string AiMomentCellInputCommand(string data, BasicRptTool.eAnalogDataType nSubItem, bool bChangeItemFlag)
		{
			int					i, count, memPos = -1;
			string				buf = "";			
			edit_MomentDataDlg	dialog = new edit_MomentDataDlg();

			try 
			{
				dialog.Text += " - " + BasicRptTool.data[1].sMainDataTypeBuf[(int)nSubItem];
			}
			catch
			{
			}
			
			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[1].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Data_Type.Items.Add(member.dataName);
			}

			if(dialog.comboBox_Data_Type.Items.Count > 0) dialog.comboBox_Data_Type.SelectedIndex = 0;
			dialog.timeRange = BasicRptTool.eTimeRangeType.HOUR;
			dialog.nCurrTimeRange[0] = 0;
			dialog.nUpTimeRange[0] = 0;
			dialog.nCurrTimeRange[1] = 0;
			dialog.nUpTimeRange[1] = 0;
			dialog.nTagTypePos = 0;			// Analog
			dialog.numericUpDown_Day.Value = 1;
			dialog.numericUpDown_Hour.Value = 0;
			dialog.numericUpDown_Min.Value = 0;
			
			if(bChangeItemFlag == false) 
			{
				count = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);		

				try
				{			
                    dialog.Set(i);
				}
				catch 
				{
				}
			}			
			dialog.ShowDialog();
			if(dialog.bOkFlag == true)
			{
                buf = dialog.Get();
                /*
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.comboBox_Data_Type.Text;
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.textBox_Tag.Text;
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += BasicRptTool.sTimeTypeRealBuf[(int)dialog.timeRange % 5];
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i = dialog.nCurrTimeRange[0];
				buf += i.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i = dialog.nCurrTimeRange[1];
				buf += i.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i = dialog.nUpTimeRange[0];
				buf += i.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i = dialog.nUpTimeRange[1];
				buf += i.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i  = ( dialog.checkBox_Seletced_DataTime.Checked ) ? 1 : 0;
				buf += i.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.numericUpDown_Day.Value.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.numericUpDown_Hour.Value.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.numericUpDown_Min.Value.ToString();*/
				return buf;
			}
			return data;
		}


		public string AiMinListCellInputCommand(string data, BasicRptTool.eAnalogDataType nSubItem, bool bChangeItemFlag)
		{
			int					i, count, memPos = -1;
			string				buf = "";			
			edit_AiMinListDlg	dialog = new edit_AiMinListDlg();
			ListViewItem		item;
			
			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[1].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Data_Type.Items.Add(member.dataName);
			}
			if(dialog.comboBox_Data_Type.Items.Count > 0) dialog.comboBox_Data_Type.SelectedIndex = 0;
			dialog.nDataTypePos = 0;
			dialog.textBox_DATA_PERIOD.Text = "5";
			
			
			if(bChangeItemFlag == false) 
			{
				count = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);		

				try
				{			
					if(count >= 1) dialog.comboBox_Data_Type.Text = BasicRptTool.sFieldData[0];
					if(count >= 2) dialog.textBox_DATA_PERIOD.Text = BasicRptTool.sFieldData[1];
					if(count >= 3) dialog.nDataTypePos = ConvertTool.ToInt32(BasicRptTool.sFieldData[2]) % 2;
					for(i = 4; i <= count; i += 2) 
					{
						item = new ListViewItem();
						item.Text = BasicRptTool.sFieldData[i-1].ToString();
						item.SubItems.Add(BasicRptTool.sFieldData[i]);
						dialog.listView1.Items.Add(item);						
					}
					
				}
				catch 
				{
				}
			}			
			dialog.ShowDialog();
			if(dialog.bOkFlag == true)
			{
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.comboBox_Data_Type.Text;
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.textBox_DATA_PERIOD.Text;
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i = dialog.nDataTypePos;
				buf += i.ToString();
				for(i = 0; i < dialog.listView1.Items.Count; i++) 
				{
					item = dialog.listView1.Items[i];
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];					
					buf+= item.SubItems[0].Text;
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
					buf+= item.SubItems[1].Text;
				}
				return buf;				
			}
			return data;
		}






	}
}

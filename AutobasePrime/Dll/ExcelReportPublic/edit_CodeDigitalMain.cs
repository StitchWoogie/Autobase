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
	/// Summary description for edit_CodeDigitalMain.
	/// </summary>
	public class edit_CodeDigitalMain
	{
		public ArrayList		memberArr;
		memberConfigStruct		member;

		public edit_CodeDigitalMain(ArrayList arr)
		{
			//
			// TODO: Add constructor logic here
			//
			memberArr = arr;
		}

		public string mainCellInputCommand(string data, BasicRptTool.eDigitalDataType nSubItem, bool bChangeItemFlag)
		{
			switch(nSubItem)
			{
				case BasicRptTool.eDigitalDataType.DI_CURR :
					return BasicCellInputCommand(data, nSubItem, bChangeItemFlag, false, false);
				case BasicRptTool.eDigitalDataType.DI_MOMENT :
				case BasicRptTool.eDigitalDataType.MULTI_DI_MOMENT :
					return DiMomentCellInputCommand(data, nSubItem, bChangeItemFlag);
				case BasicRptTool.eDigitalDataType.ONOFFLIST :
					return onOffListCellInputCommand(data, nSubItem, bChangeItemFlag);
				case BasicRptTool.eDigitalDataType.ONOFFLIST_SUM :
				case BasicRptTool.eDigitalDataType.MULTI_ONOFFLIST_SUM :
					return onOffListOperationSumCellInputCommand(data, nSubItem, bChangeItemFlag);
				default : 
					return BasicCellInputCommand(data, nSubItem, bChangeItemFlag, true, true);
			}			
		}

		public string onOffListCellInputCommand(string data, BasicRptTool.eDigitalDataType nSubItem, bool bChangeItemFlag)
		{
			int			i, count, memPos = 0;
			bool		flag = false;
			string		buf = "";
			edit_OnOffListDlg dialog = new edit_OnOffListDlg();
			
			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[2].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Indetify_Char.Items.Add(member.dataName);
				if(flag == false) dialog.comboBox_Indetify_Char.Text = member.dataName;
				flag = true;
			}
			dialog.textBox_DI_Tag.Text = "*";
			dialog.nBasicTimeNo = 0;
			dialog.nSortingNo = 0;
			dialog.timeRange = BasicRptTool.eTimeRangeType.HOUR;
			dialog.nCurrTimeRange[0] = 0;
			dialog.nCurrTimeRange[1] = 0;
			dialog.nUpTimeRange[0] = 0;
			dialog.nUpTimeRange[0] = 0;
			
			if(bChangeItemFlag == false) 
			{
				memPos = -1;
				count = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);			
				
				try 
				{				
					if(count >= 1) dialog.comboBox_Indetify_Char.Text = BasicRptTool.sFieldData[0];				
					if(count >= 2) dialog.textBox_DI_Tag.Text = BasicRptTool.sFieldData[1];
					if(count >= 3) dialog.nBasicTimeNo = ConvertTool.ToInt32(BasicRptTool.sFieldData[2]) % 2;
					if(count >= 4) dialog.timeRange = (BasicRptTool.eTimeRangeType)BasicRptTool.getReadDataSortType(BasicRptTool.sFieldData[3]);
					if(count >= 5) dialog.nCurrTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[4]);
					if(count >= 6) dialog.nCurrTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[5]);
					if(count >= 7) dialog.nUpTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[6]);
					if(count >= 8) dialog.nUpTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[7]);

                    //if (count >= 9) dialog.checkBox_Seletced_DataTime.Checked = (ConvertTool.ToInt32(BasicRptTool.sFieldData[8]) == 1) ? true : false;
                    if (count >= 9)
                    {
                        int flags = ConvertTool.ToInt32(BasicRptTool.sFieldData[8]);
                        dialog.checkBox_Seletced_DataTime.Checked = ((flags & 0x1) > 0);
                        dialog.checkBoxUsePeriodTime.Checked = ((flags & 0x2) > 0);
                    }

					if(count >= 10) dialog.nSortingNo = ConvertTool.ToInt32(BasicRptTool.sFieldData[9]) % 3;

					for(i = 11; i <= count; i++)
						dialog.listBox_FieldNo.Items.Add(BasicRptTool.sFieldData[i-1]);
				}
				catch 
				{
				}				
			}
			
			dialog.ShowDialog();
			if(dialog.bOkFlag == true) 
			{
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.comboBox_Indetify_Char.Text;
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.textBox_DI_Tag.Text;
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i  = dialog.nBasicTimeNo % 2;
				buf += i.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				try 
				{
					buf += BasicRptTool.sTimeTypeRealBuf[(int)dialog.timeRange % 5];
				}
				catch 
				{
					buf += BasicRptTool.sTimeTypeRealBuf[1];		// 시간
				}
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.nCurrTimeRange[0].ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.nCurrTimeRange[1].ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.nUpTimeRange[0].ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.nUpTimeRange[1].ToString();				
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];

                int flags = 0;
                flags |= ( dialog.checkBox_Seletced_DataTime.Checked ) ? 0x0001 : 0;
                flags |= (dialog.checkBoxUsePeriodTime.Checked) ? 0x0002 : 0;
				buf += flags.ToString();

				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i = dialog.nSortingNo % 3;				
				buf += i.ToString();
				
				for(i = 0; i < dialog.listBox_FieldNo.Items.Count; i++) 
				{
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
					buf+= dialog.listBox_FieldNo.Items[i].ToString();
				}
				return buf;
			}
			return data;
		}


		public string BasicCellInputCommand(string data, BasicRptTool.eDigitalDataType nSubItem, bool bChangeItemFlag, bool bUseMultiRow, bool bUseRange)
		{
			int					i, memPos = -1;
			string				buf = "";			
			edit_AiDiBasicDlg	dialog = new edit_AiDiBasicDlg();

			try 
			{
				dialog.Text += " - " + BasicRptTool.data[2].sMainDataTypeBuf[(int)nSubItem];
			}
			catch
			{
			}

			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[2].sDataTitle[(int)nSubItem]) continue;
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
			dialog.nTagTypePos = 2;			// Digital Input

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


		public string onOffListOperationSumCellInputCommand(string data, BasicRptTool.eDigitalDataType nSubItem, bool bChangeItemFlag)
		{
			int			i, count, memPos = 0;
			bool		flag = false;
			string		buf = "";
			edit_DiOnOffListOperationSumDlg dialog = new edit_DiOnOffListOperationSumDlg();

			try 
			{
				dialog.Text = BasicRptTool.data[2].sMainDataTypeBuf[(int)nSubItem];
			}
			catch
			{
			}
			
			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[2].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Data_Type.Items.Add(member.dataName);
				if(flag == false) dialog.comboBox_Data_Type.Text = member.dataName;
				flag = true;
			}
			dialog.textBox_Tag.Text = "*";
			dialog.nBasicTimeNo = 0;
			dialog.timeRange = BasicRptTool.eTimeRangeType.HOUR;
			dialog.nCurrTimeRange[0] = 0;
			dialog.nCurrTimeRange[1] = 0;
			dialog.nUpTimeRange[0] = 0;
			dialog.nUpTimeRange[0] = 0;
			
			if(bChangeItemFlag == false) 
			{
				memPos = -1;
				count = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);			
				
				try 
				{				
					if(count >= 1) dialog.comboBox_Data_Type.Text = BasicRptTool.sFieldData[0];				
					if(count >= 2) dialog.textBox_Tag.Text = BasicRptTool.sFieldData[1];
					if(count >= 3) dialog.nBasicTimeNo = ConvertTool.ToInt32(BasicRptTool.sFieldData[2]) % 2;
					if(count >= 4) dialog.timeRange = (BasicRptTool.eTimeRangeType)BasicRptTool.getReadDataSortType(BasicRptTool.sFieldData[3]);
					if(count >= 5) dialog.nCurrTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[4]);
					if(count >= 6) dialog.nCurrTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[5]);
					if(count >= 7) dialog.nUpTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[6]);
					if(count >= 8) dialog.nUpTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[7]);

					//if(count >= 9) dialog.checkBox_Seletced_DataTime.Checked = (ConvertTool.ToInt32(BasicRptTool.sFieldData[8]) == 1) ? true : false;					
                    if (count >= 9)
                    {
                        int flags = ConvertTool.ToInt32(BasicRptTool.sFieldData[8]);
                        dialog.checkBox_Seletced_DataTime.Checked = ((flags & 0x1) > 0);
                        dialog.checkBoxUsePeriodTime.Checked = ((flags & 0x2) > 0);
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
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				i  = dialog.nBasicTimeNo % 2;
				buf += i.ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				try 
				{
					buf += BasicRptTool.sTimeTypeRealBuf[(int)dialog.timeRange % 5];
				}
				catch 
				{
					buf += BasicRptTool.sTimeTypeRealBuf[1];		// 시간
				}
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.nCurrTimeRange[0].ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.nCurrTimeRange[1].ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.nUpTimeRange[0].ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.nUpTimeRange[1].ToString();
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];

				//i  = ( dialog.checkBox_Seletced_DataTime.Checked ) ? 1 : 0;				
				//buf += i.ToString();				
                int flags = 0;
                flags |= (dialog.checkBox_Seletced_DataTime.Checked) ? 0x0001 : 0;
                flags |= (dialog.checkBoxUsePeriodTime.Checked) ? 0x0002 : 0;
                buf += flags.ToString();

				return buf;
			}
			return data;
		}


		public string DiMomentCellInputCommand(string data, BasicRptTool.eDigitalDataType nSubItem, bool bChangeItemFlag)
		{
			int					i, count, memPos = -1;
			string				buf = "";			
			edit_MomentDataDlg	dialog = new edit_MomentDataDlg();

			try 
			{
				dialog.Text += " - " + BasicRptTool.data[2].sMainDataTypeBuf[(int)nSubItem];
			}
			catch
			{
			}
			
			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[2].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Data_Type.Items.Add(member.dataName);
			}
			if(dialog.comboBox_Data_Type.Items.Count > 0) dialog.comboBox_Data_Type.SelectedIndex = 0;
			dialog.timeRange = BasicRptTool.eTimeRangeType.HOUR;
			dialog.nCurrTimeRange[0] = 0;
			dialog.nUpTimeRange[0] = 0;
			dialog.nCurrTimeRange[1] = 0;
			dialog.nUpTimeRange[1] = 0;
			dialog.nTagTypePos = 2;			// Digital
			dialog.numericUpDown_Day.Value = 1;
			dialog.numericUpDown_Hour.Value = 0;
			dialog.numericUpDown_Min.Value = 0;
			
			
			if(bChangeItemFlag == false) 
			{
				count = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);		

				try
				{			
					if(i >= 1) dialog.comboBox_Data_Type.Text = BasicRptTool.sFieldData[0];
					if(i >= 2) dialog.textBox_Tag.Text = BasicRptTool.sFieldData[1];
					if(i >= 3) dialog.timeRange = (BasicRptTool.eTimeRangeType)BasicRptTool.getReadDataSortType(BasicRptTool.sFieldData[2]);
					if(i >= 4) dialog.nCurrTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[3]);
					if(i >= 5) dialog.nCurrTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[4]);	
					if(i >= 6) dialog.nUpTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[5]);
					if(i >= 7) dialog.nUpTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[6]);
					if(i >= 8) dialog.checkBox_Seletced_DataTime.Checked = (ConvertTool.ToInt32(BasicRptTool.sFieldData[7]) == 1) ? true : false;
					if(i >= 9) dialog.numericUpDown_Day.Value = ConvertTool.ToInt32(BasicRptTool.sFieldData[8]) % 32;
					if(i >= 10) dialog.numericUpDown_Hour.Value = ConvertTool.ToInt32(BasicRptTool.sFieldData[9]) % 24;
					if(i >= 11) dialog.numericUpDown_Min.Value = ConvertTool.ToInt32(BasicRptTool.sFieldData[10]) % 60;
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
				buf += dialog.numericUpDown_Min.Value.ToString();
				return buf;
			}
			return data;
		}




	}
}

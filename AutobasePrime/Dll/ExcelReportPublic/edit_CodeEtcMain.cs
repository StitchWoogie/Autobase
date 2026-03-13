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
	/// Summary description for edit_CodeEtcMain.
	/// </summary>
	public class edit_CodeEtcMain
	{
		public ArrayList		memberArr;
		memberConfigStruct		member;

		public edit_CodeEtcMain(ArrayList arr)
		{
			//
			// TODO: Add constructor logic here
			//
			memberArr = arr;
		}	

		public string mainCellInputCommand(string data, BasicRptTool.eEtcDataType nSubItem, bool bChangeItemFlag)
		{
			switch(nSubItem)
			{
				case BasicRptTool.eEtcDataType.MULTI_ROW_DATE :
				case BasicRptTool.eEtcDataType.MULTI_ROW_TIME : 
					return CurrentBasicDateTimeInputCommand(data, nSubItem, bChangeItemFlag, true);
				case BasicRptTool.eEtcDataType.ALARM_DATA :
					return AlarmDataCellInputCommand(data, nSubItem, bChangeItemFlag);
                case BasicRptTool.eEtcDataType.GLOBAL_GET_VAR:
                    return ConfigGlobalGetVar(data, nSubItem, bChangeItemFlag);
                case BasicRptTool.eEtcDataType.ST_CURR:
                    return ConfigStCurr(data, nSubItem, bChangeItemFlag);

				default : 
					return CurrentBasicDateTimeInputCommand(data, nSubItem, bChangeItemFlag, false);
			}			
		}


		public string CurrentBasicDateTimeInputCommand(string data, BasicRptTool.eEtcDataType nSubItem, bool bChangeItemFlag, bool bUseRange)
		{
			int					i, count, memPos = -1;
			string				buf = "", sDataTitle;

			if(bChangeItemFlag && bUseRange == false) 
			{
				sDataTitle = BasicRptTool.data[3].sDataTitle[(int)nSubItem % BasicRptTool.nMainItemMaxCount];
				count = BasicRptTool.getCurrentMainDataItemCount(memberArr, sDataTitle);
				if(count == 1) 
				{
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
					buf += BasicRptTool.getCurrentMainDataRealFirstBuf(memberArr, sDataTitle);
					return buf;
				}
			}
			
			edit_EtcDateTimeDlg	dialog = new edit_EtcDateTimeDlg();	
	
			try 
			{
				dialog.Text += " - " + BasicRptTool.data[3].sMainDataTypeBuf[(int)nSubItem];
			}
			catch
			{
			}

			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[3].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Data_Type.Items.Add(member.dataName);
			}
			if(dialog.comboBox_Data_Type.Items.Count > 0) dialog.comboBox_Data_Type.SelectedIndex = 0;
			if(bUseRange) dialog.bMultiRow = true;
			dialog.bUseRange = bUseRange;
			dialog.timeRange = BasicRptTool.eTimeRangeType.HOUR;			
			dialog.nCurrTimeRange[0] = 0;
			dialog.nUpTimeRange[0] = 0;
			dialog.nCurrTimeRange[1] = 0;
			dialog.nUpTimeRange[1] = 0;

			if(bChangeItemFlag == false) 
			{
				i = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);		

				try
				{			
					if(i >= 1) dialog.comboBox_Data_Type.Text = BasicRptTool.sFieldData[0];
					if(bUseRange) 
					{
						if(i >= 2) dialog.timeRange = (BasicRptTool.eTimeRangeType)BasicRptTool.getReadDataSortType(BasicRptTool.sFieldData[1]);
						if(i >= 3) dialog.nCurrTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[2]);
						if(i >= 4) dialog.nCurrTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[3]);	
						if(i >= 5) dialog.nUpTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[4]);
						if(i >= 6) dialog.nUpTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[5]);
						if(i >= 7) dialog.checkBox_Seletced_DataTime.Checked = (ConvertTool.ToInt32(BasicRptTool.sFieldData[6]) == 1) ? true : false;
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
				return buf;				
			}
			return data;
		}

		/*

		public string MultiRowTimeInputCommand(string data, BasicRptTool.eEtcDataType nSubItem, bool bChangeItemFlag)
		{
			int					i, memPos = -1;
			string				buf = "";//, sDataTitle;

			//if(bChangeItemFlag) 
			//{
			//	sDataTitle = BasicRptTool.data[3].sDataTitle[(int)nSubItem % BasicRptTool.nMainItemMaxCount];
			//	count = BasicRptTool.getCurrentMainDataItemCount(memberArr, sDataTitle);				
			//}
			
			edit_EtcDateTimeDlg	dialog = new edit_EtcDateTimeDlg();

			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[3].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Data_Type.Items.Add(member.dataName);
			}
			if(dialog.comboBox_Data_Type.Items.Count > 0) dialog.comboBox_Data_Type.SelectedIndex = 0;

			if(bChangeItemFlag == false) 
			{
				i = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);		

				try
				{			
					if(i >= 1) dialog.comboBox_Data_Type.Text = BasicRptTool.sFieldData[0];
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
				return buf;				
			}
			return data;
		}*/

		public string AlarmDataCellInputCommand(string data, BasicRptTool.eEtcDataType nSubItem, bool bChangeItemFlag)
		{
			int			i, count, memPos = 0;
			bool		flag = false;
			string		buf = "";
			edit_EtcAlarmDataDlg dialog = new edit_EtcAlarmDataDlg();
			
			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				if(member.sDataTitle != BasicRptTool.data[3].sDataTitle[(int)nSubItem]) continue;
				dialog.comboBox_Data_Type.Items.Add(member.dataName);
				if(flag == false) dialog.comboBox_Data_Type.Text = member.dataName;
				flag = true;
			}			
			dialog.textBox_Tag.Text = "*";
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
					if(count >= 3) dialog.timeRange = (BasicRptTool.eTimeRangeType)BasicRptTool.getReadDataSortType(BasicRptTool.sFieldData[2]);
					if(count >= 4) dialog.nCurrTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[3]);
					if(count >= 5) dialog.nCurrTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[4]);
					if(count >= 6) dialog.nUpTimeRange[0] = ConvertTool.ToInt32(BasicRptTool.sFieldData[5]);
					if(count >= 7) dialog.nUpTimeRange[1] = ConvertTool.ToInt32(BasicRptTool.sFieldData[6]);
					if(count >= 8) dialog.checkBox_Seletced_DataTime.Checked = (ConvertTool.ToInt32(BasicRptTool.sFieldData[7]) == 1) ? true : false;
					for(i = 9; i <= count; i++)
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
				buf += dialog.comboBox_Data_Type.Text;
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += dialog.textBox_Tag.Text;				
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				try 
				{
					buf += BasicRptTool.sTimeTypeRealBuf[(int)dialog.timeRange % 5];
				}
				catch 
				{
					buf += BasicRptTool.sTimeTypeRealBuf[1];		// ½Ã°£
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
				i  = ( dialog.checkBox_Seletced_DataTime.Checked ) ? 1 : 0;				
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

        public string ConfigGlobalGetVar(string data, BasicRptTool.eEtcDataType nSubItem, bool bChangeItemFlag)
        {
            int count, memPos = 0;

            string sDataTitle = BasicRptTool.data[3].sDataTitle[(int)nSubItem % BasicRptTool.nMainItemMaxCount];
            
            PropertyEtcGlobalGetVar dialog = new PropertyEtcGlobalGetVar();

            if (bChangeItemFlag == false)
            {
                memPos = -1;
                count = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);

                dialog.Set(BasicRptTool.sFieldData[1]);
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string buf = "";
                string var;
                dialog.Get(out var);

                buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
                buf += sDataTitle;
                buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
                buf += var;

                return buf;
            }

            return data;
        }

        public string ConfigStCurr(string data, BasicRptTool.eEtcDataType nSubItem, bool bChangeItemFlag)
        {
            int count, memPos = 0;

            string sDataTitle = BasicRptTool.data[3].sDataTitle[(int)nSubItem % BasicRptTool.nMainItemMaxCount];

            ExcelReportPublic.PropertyEtcStCurr dialog = new ExcelReportPublic.PropertyEtcStCurr();

            if (bChangeItemFlag == false)
            {
                memPos = -1;
                count = BasicRptTool.setCellDataToMemberValueRead(memberArr, data, ref memPos);

                dialog.Set(BasicRptTool.sFieldData[1]);
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string buf = "";
                string var;
                dialog.Get(out var);

                buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
                buf += sDataTitle;
                buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
                buf += var;

                return buf;
            }

            return data;
        }

	}
}

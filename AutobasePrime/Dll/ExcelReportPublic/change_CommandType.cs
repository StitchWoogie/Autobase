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
	/// Summary description for change_CommandType.
	/// </summary>
	public class change_CommandType
	{
		ConnectionStringList	dsnList;
		ArrayList				memberArr;
		string					data;
		int						nElementNum = 0;
		memberConfigStruct		member;
		string[]				sFieldData;

		public change_CommandType(ConnectionStringList dsn, ArrayList memArr, string buf)
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

			return changeCommand();
		}

		
		
		string changeCommand()
		{
			if(nElementNum < 1) return data;
			sFieldData = BasicRptTool.sFieldData;			
			
			if(BasicRptTool.cSaveChange == 0) 
			{
				string		command = sFieldData[0];

				BasicRptTool.cSaveChange = 1;
				if(runCommandTypeDlg(ref command)) 
				{
					BasicRptTool.sSaveCommand = command;
					BasicRptTool.cSaveChange = 255;
					return setCommandData();
				}
				return data;
			}
			else 
			{
				return setCommandData();
			}			
		}

		string setCommandData()
		{
			int			i;
			string		buf = "";

			try 
			{
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
				buf += BasicRptTool.sSaveCommand;
				for(i = 1; i < nElementNum; i++) 
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

		bool runCommandTypeDlg(ref string command)
		{
			int			i, commPos = -1;
			string		buf;
			change_CommandDlg dialog = new change_CommandDlg();

			for(i = 0; i < memberArr.Count; i++) 
			{
				member = (memberConfigStruct)memberArr[i];
				buf = String.Format("{0} - {1}", member.dataName, member.sMainDataTypeBuf);
				dialog.comboBox_Data_Type.Items.Add(buf);
				if(member.dataName == command) 
				{
					commPos = i;
					dialog.comboBox_Data_Type.Text = buf;
				}
			}
			if(commPos == -1)	// 맞는 명령어가 없다.
			{
				dialog.comboBox_Data_Type.Text = command;
			}
			
			
			dialog.ShowDialog();
			if(dialog.bOkFlag == true)
			{
				try 
				{
					i = dialog.comboBox_Data_Type.SelectedIndex;
					if(i >= memberArr.Count) return false;
					member = (memberConfigStruct)memberArr[i];
					command = member.dataName;
					return true;
				}
				catch 
				{
					return false;
				}				
			}
			return false;
		}




	}
}

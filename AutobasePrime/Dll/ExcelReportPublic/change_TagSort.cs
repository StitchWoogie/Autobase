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
using DialogTag;


namespace ExcelReportData
{
	/// <summary>
	/// Summary description for change_TagSort.
	/// </summary>
	public class change_TagSort
	{
		ConnectionStringList	dsnList;
		ArrayList				memberArr;
		string					data;
		int						nElementNum = 0;
		memberConfigStruct		member;
		string[]				sFieldData;

		public change_TagSort(ConnectionStringList dsn, ArrayList memArr, string buf)
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
				case BasicRptTool.eMainDataType.DB_DATA : return changeDbTagName((BasicRptTool.eDbDataType)i);
				case BasicRptTool.eMainDataType.ANALOG : return changeAnalogTagName((BasicRptTool.eAnalogDataType)i);
				case BasicRptTool.eMainDataType.DIGITAL : return ChangeTagName(1);		// 모든 요소가 태그 필요
				case BasicRptTool.eMainDataType.ETC : return changeEtcTagName((BasicRptTool.eEtcDataType)i);
			}
			return data;
		}

		string changeDbTagName(BasicRptTool.eDbDataType eDbDataType)
		{
			switch(eDbDataType) 
			{				
				case BasicRptTool.eDbDataType.DB_CURRENT : return ChangeTagName(2);	// DB 현재값
				default : return data;
			}			
		}

		string changeAnalogTagName(BasicRptTool.eAnalogDataType eAiDataType)
		{
			switch(eAiDataType) 
			{				
				case BasicRptTool.eAnalogDataType.AI_MINLIST : return data; //AI 기간자료					
				default : return ChangeTagName(1);
			}			
		}
		

		string changeEtcTagName(BasicRptTool.eEtcDataType eDataType)
		{
			switch(eDataType) 
			{
				case BasicRptTool.eEtcDataType.ALARM_DATA :	return ChangeTagName(1);
				default : return data;
			}			
		}


		string ChangeTagName(int pos)
		{
			if(nElementNum < 2) return data;
			if(pos <= 0 || pos >= nElementNum) return data;

			sFieldData = BasicRptTool.sFieldData;

			if(BasicRptTool.cSaveChange == 0) 
			{
				string tagName = sFieldData[pos];

				BasicRptTool.cSaveChange = 1;
				if(runTagNameDlg(ref tagName)) 
				{
					BasicRptTool.sSaveTag = tagName;
					BasicRptTool.cSaveChange = 255;
					return setTagNameData(pos);
				}
				return data;
			}
			else 
			{
				return setTagNameData(pos);
			}			
		}

		string setTagNameData(int pos)
		{
			int			i;
			string		buf = "";

			try 
			{				
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandStartDataCode % BasicRptTool.nSeperateCharCount];
				buf += sFieldData[0];
				for(i = 1; i < pos; i++) 
				{	
					buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
					buf += sFieldData[i];
				}
				buf += BasicRptTool.reportConfig.cStartCode[BasicRptTool.reportConfig.code.nCommandSeperateCode % BasicRptTool.nSeperateCharCount];
				buf += BasicRptTool.sSaveTag;
				for(i = pos+1; i < nElementNum; i++) 
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

		bool runTagNameDlg(ref string tagName)
		{
			SelectTag dialog = new SelectTag();

			dialog.bUseTagDI = true;
			dialog.bUseTagAI = true;
			
			if(dialog.Run(Form.ActiveForm) == DialogResult.OK) 
			{
				tagName = dialog.sTag;
				return true;
			}
			return false;
		}


	}
}

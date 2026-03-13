using System;
using System.IO;
using System.Collections;
using NetTools;
using System.Windows.Forms;
using AutoLibLocal;

namespace AutoLib
{
	/// <summary>
	/// Summary description for DemandControl.
	/// </summary>
	public class DemandControl
	{
		public static ArrayList blockDemandControl = new ArrayList();
		public static ArrayList arrayClassList = new ArrayList();

		public const int MAX_INCLINE_SEC = 60;	// 60초 동안의 기울기를 모아서 계산에 반영한다.

		static DemandControl()
		{
			//
			// TODO: Add constructor logic here
			//
			
		}

		public static void FunctionBlockDemandControlLoad(ArrayList block)
		{
			TextReader reader;
			string filename;
			string buf;
			FUNCTION_BLOCK_DEMAND_CONTROL item;
			CommaBlockString comma = new CommaBlockString();

			filename = String.Format("{0}\\FUNCTION\\Demand.lstx", TotalConfig.sDirWorkProject);
			if(File.Exists(filename)) 
			{
				reader = new StreamReader(filename);
			}
			else 
			{
				// 구버전
				filename = String.Format("{0}\\FUNCTION\\Demand.fbk", TotalConfig.sDirWorkProject);
				if(!File.Exists(filename))	return;
				reader = new StreamReader(filename, System.Text.Encoding.Default);
			}
			if(reader == null)	return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				
				if(buf.Length == 0)	continue;
				item = new FUNCTION_BLOCK_DEMAND_CONTROL();
				
				comma.Set(buf);
				comma.GetString(ref item.title);
				comma.GetString(ref item.tagCurr.tag);
				item.tagCurr.tag = item.tagCurr.tag.Trim();
				comma.GetString(ref item.tagTarget.tag);
				item.tagTarget.tag = item.tagTarget.tag.Trim();
				comma.GetInt(ref item.nControlTime);
				comma.GetInt(ref item.nControlProtectTime);
				comma.GetString(ref item.tagOut.tag);
				item.tagOut.tag = item.tagOut.tag.Trim();
				comma.GetChar(ref item.bUseIsolation);
				comma.GetString(ref item.tagEcho.tag);
				item.tagEcho.tag = item.tagEcho.tag.Trim();
				comma.GetFloat(ref item.fPulseRatio);
				comma.GetChar(ref item.bUseEOI);
				comma.GetString(ref item.tagEOI.tag);
				item.tagEOI.tag = item.tagEOI.tag.Trim();
				comma.GetChar(ref item.cOutputType);
				comma.GetString(ref item.tagOut2.tag);
				item.tagOut2.tag = item.tagOut2.tag.Trim();
				comma.GetString(ref item.tagStartTarget.tag);
				item.tagStartTarget.tag = item.tagStartTarget.tag.Trim();
				comma.GetChar(ref item.bInputAutoReset);
				comma.GetString(ref item.tagPredictionDisplay.tag);
				item.tagPredictionDisplay.tag = item.tagPredictionDisplay.tag.Trim();
		
				if(item.nControlTime <= 0)	item.nControlTime = 15;
				if(item.nControlTime > 60)	item.nControlTime = 60;

				if(item.fPulseRatio <= 0)	item.fPulseRatio = 1;

				//		item.bOutFlag = ON;
				comma.GetInt(ref item.nPercentY);

				if(item.nPercentY < 100 || item.nPercentY > 200)
					item.nPercentY = 120;

				comma.GetString(ref item.tagPredictionInput.tag);
				item.tagPredictionInput.tag = item.tagPredictionInput.tag.Trim();

                comma.GetBool(ref item.bDatabaseSave);
                comma.GetString(ref item.sDatabaseDsn);

				block.Add(item);
			}

			reader.Close();
		}

		public static void FunctionBlockDemandControlInit(ArrayList block)
		{
			int i;
			int l;
			FUNCTION_BLOCK_DEMAND_CONTROL item;

			for(l = 0; l < block.Count; l++) 
			{
				item = (FUNCTION_BLOCK_DEMAND_CONTROL)block[l];
				if(item.nControlTime > 0) 
				{
					item.data = new double[item.nControlTime*60];
					if(item.data != null) 
					{
						for(i = 0; i < item.nControlTime*60; i++) 
						{
							item.data[i] = 0;
						}
					}
				}
		
				item.incline = new double[DemandControl.MAX_INCLINE_SEC];
				item.nInclineRingPos = 0;

				for(i = 0; i < DemandControl.MAX_INCLINE_SEC; i++) 
				{
					item.incline[i] = -1;
				}

				item.nCurrentSec = 0;
				item.tStart = DateTime.Now;

				if(!TagLib.GetTagPosAI(item.tagCurr.tag, ref item.tagCurr.tag_pos)) 
				{
					if(Tools.IsLangKorean()) 
						item.sErrorMsg = String.Format("Demand Control에 설정된 현재전력 태그는 AI가 아닙니다. [{0}]", item.tagCurr.tag);
					else
						item.sErrorMsg = String.Format("Demand Control: Set AI tag to current power. [{0}]", item.tagCurr.tag);

					MessageBox.Show(item.sErrorMsg, "Demand Control : " + item.title);
					item.error_flag = 1;
				}
				if(!TagLib.GetTagPosAI(item.tagTarget.tag, ref item.tagTarget.tag_pos)) 
				{
					if(Tools.IsLangKorean()) 
						item.sErrorMsg = String.Format("Demand Control에 설정된 목표전력 태그는 AI가 아닙니다. [{0}]", item.tagTarget.tag);
					else
						item.sErrorMsg = String.Format("Demand Control: Set AI tag to target power. [{0}]", item.tagTarget.tag);

					MessageBox.Show(item.sErrorMsg, "Demand Control : " + item.title);
					item.error_flag = 1;
				}
				else 
				{
					TagAiClass ai = TagLib.GetStructAI(item.tagTarget.tag, ref item.tagTarget.tag_pos);
					item.fTargetValue = ai.curr;
				}

				if(item.bUseIsolation == 1) 
				{
					if(!TagLib.GetTagTypeAndPos(item.tagOut.tag, ref item.tagOut.tag_type, ref item.tagOut.tag_pos)) 
					{
						if(Tools.IsLangKorean()) 
							item.sErrorMsg = String.Format("Demand Control에 설정된 출력 태그는 존재하지 않는 태그입니다. [{0}]", item.tagOut.tag);
						else
							item.sErrorMsg = String.Format("Demand Control: Output tag is not exist. [{0}]", item.tagOut.tag);

						MessageBox.Show(item.sErrorMsg, item.tagOut.tag);
						item.error_flag = 1;
					}
					else 
					{
						if(item.tagOut.tag_type != EnumTagType.DI && item.tagOut.tag_type != EnumTagType.DO) 
						{
							if(Tools.IsLangKorean()) 
								item.sErrorMsg = String.Format("Demand Control에 설정된 출력 태그는 DI/DO 태그가 아닙니다.[{0}]", item.tagOut.tag);
							else
								item.sErrorMsg = String.Format("Demand Control: Set DI/DO tag to Output. [{0}]", item.tagOut.tag);

							MessageBox.Show(item.sErrorMsg, item.tagOut.tag);
							item.error_flag = 1;
						}
					}
					if(!TagLib.GetTagPosDI(item.tagEcho.tag, ref item.tagEcho.tag_pos)) 
					{
						if(Tools.IsLangKorean()) 
							item.sErrorMsg = String.Format("Demand Control에 설정된 확인 태그는 DI가 아닙니다. [{0}]", item.tagEcho.tag);
						else
							item.sErrorMsg = String.Format("Demand Control: Set DI tag to Confirmation tag. [{0}]", item.tagEcho.tag);

						MessageBox.Show(item.sErrorMsg, item.tagEcho.tag);
						item.error_flag = 1;
					}
					if(item.cOutputType == 1) 
					{
						if(!TagLib.GetTagTypeAndPos(item.tagOut2.tag, ref item.tagOut2.tag_type, ref item.tagOut2.tag_pos)) 
						{
							if(Tools.IsLangKorean()) 
								item.sErrorMsg = String.Format("Demand Control에 설정된 출력 태그2는 존재하지 않는 태그입니다. [{0}]", item.tagOut2.tag);
							else
								item.sErrorMsg = String.Format("Demand Control: Output2 tag is not exist. [{0}]", item.tagOut2.tag);

							MessageBox.Show(item.sErrorMsg, item.tagOut2.tag);
							item.error_flag = 1;
						}
						else 
						{
							if(item.tagOut2.tag_type != EnumTagType.DI && item.tagOut2.tag_type != EnumTagType.DO) 
							{
								if(Tools.IsLangKorean()) 
									item.sErrorMsg = String.Format("Demand Control에 설정된 출력 태그2는 DI/DO 태그가 아닙니다.[{0}]", item.tagOut2.tag);
								else
									item.sErrorMsg = String.Format("Demand Control: Set DI/DO tag to Output2 tag. [{0}]", item.tagOut2.tag);

								MessageBox.Show(item.sErrorMsg, item.tagOut2.tag);
								item.error_flag = 1;
							}
						}
					}

				}

				if(item.bUseEOI == 1) 
				{
					if(!TagLib.GetTagPosDI(item.tagEOI.tag, ref item.tagEOI.tag_pos)) 
					{
						if(Tools.IsLangKorean()) 
							item.sErrorMsg = String.Format("Demand Control에 설정된 EOI 태그는 DI가 아닙니다. [{0}]", item.tagEOI.tag);
						else
							item.sErrorMsg = String.Format("Set DI tag to EOI tag. [{0}]", item.tagEOI.tag);

						MessageBox.Show(item.sErrorMsg, item.tagEOI.tag);
						item.error_flag = 1;
					}
				}

				// 시작점 표시는 태그가 없으면 0으로 처리한다.
				TagLib.GetTagTypeAndPos(item.tagStartTarget.tag, ref item.tagStartTarget.tag_type, ref item.tagStartTarget.tag_pos);

				// 예측 전력을 표시할 태그
				TagLib.GetTagTypeAndPos(item.tagPredictionDisplay.tag, ref item.tagPredictionDisplay.tag_type, ref item.tagPredictionDisplay.tag_pos);
				// 예측 전력을 가져올 태그
				TagLib.GetTagTypeAndPos(item.tagPredictionInput.tag, ref item.tagPredictionInput.tag_type, ref item.tagPredictionInput.tag_pos);
		
				if(item.fTargetValue <= 0)	item.fTargetValue = 10;
			}
		}
	}
}


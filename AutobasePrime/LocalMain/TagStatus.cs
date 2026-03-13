using System;
using System.IO;
using AutoLibLocal;
using AutoLib;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// Summary description for TagStatus.
	/// </summary>
	public class TagStatus
	{
		public TagStatus()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static string TagPosToString(int[] tag_pos)
		{
			string buf = "";
			for(int i = 0; i < tag_pos.Length; i++) 
			{
				if(i != 0) 
				{
					buf += '.';
				}
				buf += tag_pos[i];
			}

			return buf;
		}

		static int[] StringToTagPos(string buf)
		{
			int[] tag_pos;

			int count = 0;
			int i;

			for(i = 0; i < buf.Length; i++) 
			{
				if(buf[i] == '.')	count++;
			}

			CommaBlockString comma = new CommaBlockString();
			comma.SetBlockCode('.');
			comma.Set(buf);

			count++;	// .보다 1개 많다.

			tag_pos = new int[count];

			for(i = 0; i < count; i++) 
			{
				comma.GetInt(ref tag_pos[i]);
			}

			return tag_pos;
		}

		public static void Load()
		{
			TextReader reader;
			string filename;
			string buf;
            CommaTextReader comma = new CommaTextReader();
			string tag = "";

			filename = String.Format("{0}\\Tag\\TagStatus.all", TotalConfig.sDirWorkProject);

			if(!File.Exists(filename))	return;

			reader = new StreamReader(filename);
			
			if(reader == null) 
			{	
				return;
			}

			int[] tag_pos = new int[1];
			TagPublicClass tp;
			int imsi = 0;
			string stagpos = "";

				while(true) 
				{
					buf = reader.ReadLine();
					if(buf == null)	break;

					comma.Set(buf);
					comma.GetString(ref tag);
					comma.GetString(ref stagpos);
					tag_pos = StringToTagPos(stagpos);

					tp = TagLib.GetStructPublic(tag, ref tag_pos);

					if(tp.enumTagType == EnumTagType.AI) 
					{
						TagAiClass ai = (TagAiClass)tp;

						comma.GetDouble(ref ai.curr);
						ai.tSumTotal = comma.GetDateTime();
						ai.tSumPart  = comma.GetDateTime();
						comma.GetDouble(ref ai.fSumTotal);
						comma.GetDouble(ref ai.fSumPart);
						comma.GetBool(ref ai.bCurrentAlarmStatus);
						comma.GetInt(ref imsi);
						ai.cAlarmLevelStatus = (EnumAnalogLevel)imsi;
						comma.GetBool(ref ai.bNeedAlarmConfirm);
                        comma.GetDouble(ref ai.real_curr);              // 2021-8-3 추가 real_curr은 AnalogStatus Bit 연산에서만 사용한다.

                        // 상태값을 읽었으면 분자료를 만드는 최대/최소 초기값도 같이 만들어 주어야 한다. 아래가 없으면 초기시작시 최대값은 항상 0이다.
                        ai.fMinMin = ai.curr;
                        ai.fMinMax = ai.curr;
					}
					else if(tp.enumTagType == EnumTagType.AO) 
					{
						TagAoClass ao = (TagAoClass)tp;

						comma.GetDouble(ref ao.curr);
					}
					else if(tp.enumTagType == EnumTagType.DI) 
					{
						TagDiClass di = (TagDiClass)tp;

						comma.GetChar(ref di.curr);
						comma.GetBool(ref di.bCurrentAlarmStatus);
						comma.GetBool(ref di.bNeedAlarmConfirm);
					}
					else if(tp.enumTagType == EnumTagType.ST) 
					{
						TagStClass st = (TagStClass)tp;

                        comma.GetString(ref st.curr);               // 2019-7-17 수정
						//comma.GetStringTotalRemain(ref st.curr); // 이것은 CommaBlockString 클래스이다 , " 를 수용하지 못한다. 2019-7-17 수정
					}
					else {}
		
				}
			reader.Close();
		}

		public static void Save()
		{
			CommaTextWriter writer;
			string filename;
			TagPublicClass tp;

			filename = String.Format("{0}\\Tag", TotalConfig.sDirWorkProject);
			Directory.CreateDirectory(filename);
			filename = String.Format("{0}\\Tag\\TagStatus.all", TotalConfig.sDirWorkProject);

            writer = new CommaTextWriter(filename);

			if(writer == null)	return;

			for(int i = 0; i < TagLib.tagListAll.Length; i++) 
			{
				tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);

				if(tp.enumTagType == EnumTagType.AI) 
				{
					TagAiClass ai = (TagAiClass)tp;

					writer.Write("{0},", ai.tag);
					writer.Write("{0},", TagPosToString(TagLib.tagListAll[i].tag_pos_save));

					writer.Write("{0},", ai.curr);
					writer.Write("{0},", ConvertTool.ToDateTimeString(ai.tSumTotal));   // 그냥 저장하면 오전 오후가 들어가서 다른 나라와 호환성을 위해서 24시로 저장한다. 2018-3-26
                    writer.Write("{0},", ConvertTool.ToDateTimeString(ai.tSumPart));    // 그냥 저장하면 오전 오후가 들어간다 다른 나라와 호환성을 위해서 24시로 저장한다. 2018-3-26       
					writer.Write("{0},", ai.fSumTotal);
					writer.Write("{0},", ai.fSumPart);
					writer.Write("{0},", ai.bCurrentAlarmStatus);
					writer.Write("{0},", (int)ai.cAlarmLevelStatus);
					writer.Write("{0},", ai.bNeedAlarmConfirm);
                    writer.Write("{0},", ai.real_curr);     // 2021-8-3 추가 real_curr은 AnalogStatus Bit 연산에서만 사용한다.
					writer.WriteLine();
				}
				else if(tp.enumTagType == EnumTagType.AO) 
				{
					TagAoClass ao = (TagAoClass)tp;

					writer.Write("{0},", ao.tag);
					writer.Write("{0},", TagPosToString(TagLib.tagListAll[i].tag_pos_save));

					writer.Write("{0},", ao.curr);
					writer.WriteLine();
				}
				else if(tp.enumTagType == EnumTagType.DI) 
				{
					TagDiClass di = (TagDiClass)tp;

					writer.Write("{0},", di.tag);
					writer.Write("{0},", TagPosToString(TagLib.tagListAll[i].tag_pos_save));

					writer.Write("{0},", di.curr);
					writer.Write("{0},", di.bCurrentAlarmStatus);
					writer.Write("{0},", di.bNeedAlarmConfirm);
					writer.WriteLine();
				}
				else if(tp.enumTagType == EnumTagType.ST) 
				{
					TagStClass st = (TagStClass)tp;

					writer.Write("{0},", st.tag);
					writer.Write("{0},", TagPosToString(TagLib.tagListAll[i].tag_pos_save));

					writer.Write("{0}", st.curr);
					writer.WriteLine();
				}
				else {}
		
			}

            writer.Flush();     // 2017-1-10 추가함.  경일에서 Local.tagx 의 중간이 잘린다고 하여 추가함. 종성도 이전에 비슷한 현상이 있는듯 하여 추가함. 
                                // 여기가 문제가 있는것은 아니나 local.tagx와 특성을 같이 유지하기 위해서 추가함.
			writer.Close();
		}
	}
}



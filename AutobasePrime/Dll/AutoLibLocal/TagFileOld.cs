using System;
using System.Collections;
using NetTools;
using System.IO;
using AutoLibLocal;
using System.Windows.Forms;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TerminalClass.
	/// </summary>
	public class TagFileOld
	{
		public TagFileOld()
		{
			
		}

		void CommaBufToAiFileStruct(ref TagAiClass ai, string buf)
		{
			CommaBlockString commaBuf = new CommaBlockString();
			int number = 0;

			commaBuf.Set(buf);

			commaBuf.GetInt(ref number);
			commaBuf.GetString(ref ai.tag);
			Tools.KillEndSpace(ref ai.tag);
			ai.name = ai.tag;
			commaBuf.GetString(ref ai.description);
			ai.description = ai.description.Trim();

			commaBuf.GetInt(ref ai.port);
			commaBuf.GetInt(ref ai.station);
            commaBuf.GetInt(ref ai.address);
			commaBuf.GetInt(ref ai.fn);
			commaBuf.GetString(ref ai.unit);
			commaBuf.GetDouble(ref ai.fBase);
			float mid = 0, ratio = 0;
			commaBuf.GetFloat(ref mid);
            commaBuf.GetDouble(ref ai.fFull);
			commaBuf.GetFloat(ref ratio);

			if(ratio == 0)	ratio = 1;

			ai.fPlcBase = mid;
			ai.fPlcFull = mid+ratio;
			
			float sp = 0;
			commaBuf.GetFloat(ref sp);
            commaBuf.GetDouble(ref ai.hihi);
            commaBuf.GetDouble(ref ai.high);
            commaBuf.GetDouble(ref ai.low);
            commaBuf.GetDouble(ref ai.lolo);
			commaBuf.GetChar(ref ai.act);
			commaBuf.GetBYTE(ref ai.alarm);
			commaBuf.GetBYTE(ref ai.bFileSave);	// 데이터 파일을 저장할 것이냐?

			commaBuf.GetString(ref ai.sSubOutDigitalHiHi);
			Tools.KillEndSpace(ref ai.sSubOutDigitalHiHi);
			commaBuf.GetString(ref ai.sSubOutDigitalLoLo);
			Tools.KillEndSpace(ref ai.sSubOutDigitalHiHi);
			commaBuf.GetString(ref ai.sSubOutAnalog);
			Tools.KillEndSpace(ref ai.sSubOutDigitalHiHi);

            commaBuf.GetString(ref ai.sDisplayFormat);
            if (!TagUtil.StringToDisplayFormat(ai.sDisplayFormat, out ai.fDisplayFormat, out ai.cDisplayFormat))
            {
                ai.sDisplayFormat = "10.2";
            }
            // 8.x 소스에 이부분이 있어서 일단 그대로 사용함
            if (ai.fDisplayFormat == 0) ai.fDisplayFormat = (float)10.2;

			commaBuf.GetBYTE(ref ai.cAlarmType);			// 알람 조건
			commaBuf.GetString(ref ai.sGraphicFile);		// 이 태그가 있는 그래픽 파일은?
			commaBuf.GetString(ref ai.sAlarmWaveFile);		// 경보 발생시 사용할 그래픽 파일
			commaBuf.GetInt(ref ai.nCalculateFilter);		// 특별한 계산식을 정한다. 0 - 일반

			commaBuf.GetString(ref ai.sSubOutAnalogSP);		// Analog Set Point 값.
			Tools.KillEndSpace(ref ai.sSubOutAnalogSP);

			ushort reserved = 0;
			commaBuf.GetWORD(ref reserved);		// 이전(7.0) 에는 ai.wAlarmBitON로 사용

			commaBuf.GetBYTE(ref ai.bCutOverValue);	// 아날로그 값의 계산치가 RANGE를 벗어났을 때는 값을 최대 최소로 잘라주는 옵션.
            commaBuf.GetDouble(ref ai.view_full);		// 보여주는 범위 high
            commaBuf.GetDouble(ref ai.view_base);		// 보여주는 범위 low
			commaBuf.GetInt(ref ai.nCalcDelay);			// 계산시 지난값과 평균하여 사용할 횟수.
			commaBuf.GetWORD(ref ai.wAlarmPriority); 	// 경보울리는 방법.
			commaBuf.GetBYTE(ref ai.cTagLinkType);		// Dde를 태그로 사용할것인가?
			commaBuf.GetString(ref ai.sDdeService);
			commaBuf.GetString(ref ai.sDdeTopic);
			commaBuf.GetString(ref ai.sDdeItem);
			commaBuf.GetBYTE(ref ai.cConfirmCount);	// 제어시 확인할 count
			commaBuf.GetBYTE(ref ai.bBcdValue);		// 메모리에서 BCD로 읽을 것인가?
			commaBuf.GetFloat(ref ai.fAlarmReturnGab);	// 알람에서 복귀할때 gab만큼 낮추거나 높인 수치에서 복귀한다.
			commaBuf.GetWORD(ref ai.wRateOfChangeLimit);	// 이전값과 비교해서 지정해 놓은 값 이상이 차이나면 경보를 울린다. 0 = 사용안함.
			ushort flags = 0;
			commaBuf.GetHexWORD(ref flags);			// 0 = 자동, 1 = 수동 기입중.
			ai.wProtectFlags = (EnumProtectFlag)flags;
			commaBuf.GetBYTE(ref ai.cAlarmProtectOnBigChangePercent);
			commaBuf.GetInt(ref ai.nAlarmProtectOnBigChangeSecond);
			commaBuf.GetBYTE(ref ai.bDdeRequest);
			commaBuf.GetWORD(ref ai.wScanTime);
			if(ai.wScanTime == 0) 
			{
				ai.wScanTime = TagLib.DEFAULT_SCANTIME;
			}
			commaBuf.GetChar(ref ai.bLocalTag);

			if(ai.view_full <= ai.view_base) 
			{	// 보여주는 range가 이상할때는 다시 초기화 해 준다.
				ai.view_full = ai.fFull;
				ai.view_base = ai.fBase;
			}

			commaBuf.GetFloat(ref ai.fScrollUnit);
			if(ai.fScrollUnit == 0)	ai.fScrollUnit = 1;
		}

		void CommaBufToTagStructAI(ref TagAiClass ai, string buf)
		{
			CommaBufToAiFileStruct(ref ai, buf);
	
			ai.curr = (float)0.0;
			ai.old =  (float)0.0;
			ai.real_curr = 0;
			ai.nScanCount = 0;			// 적산을 하기위해 1분동안 변한 아나로그 횟수
			ai.fMinHap = (float)0;		// 적산을 1분동안 더해 놓은값
			ai.fMinMin = (float)0;		// 1분동안의 최소값
			ai.fMinMax = (float)0;		// 1분동안의 최대값
			ai.fSumTotal = (float)0;	// 현재까지 적산치
			ai.fSumPart = (float)0; 	// 부분적인 적산치
			ai.tSumTotal = DateTime.Now;
			ai.tSumPart = DateTime.Now;

		}

		public void TagLoadAI(string path)
		{
			string buf = "";
			TagAiClass ai;
			TextReader reader;

			if(!File.Exists(path)) 
			{
				return;
			}

			reader = TagFile.TextReaderWithCheck(path, System.Text.Encoding.Default);
			if(reader == null) return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				if(buf.Length == 0)	continue;
				if(buf[0] == ';')	continue;
				
				ai = new TagAiClass();

				ai.enumTagType = EnumTagType.AI;
				
				CommaBufToTagStructAI(ref ai, buf);
				TagLib.groupRoot.AddTag(ai, false);
			}
			reader.Close();
		}

		void CommaBufToAoFileStruct(ref TagAoClass ao, string buf)
		{
			int number = 0;
			CommaBlockString commaBuf = new CommaBlockString();

			commaBuf.Set(buf);

			commaBuf.GetInt(ref number);
			commaBuf.GetString(ref ao.tag);
			Tools.KillEndSpace(ref ao.tag);
			ao.name = ao.tag;
			commaBuf.GetString(ref ao.description);
			ao.description = ao.description.Trim();
			commaBuf.GetInt(ref ao.port);
			commaBuf.GetInt(ref ao.station);
			commaBuf.GetHexDWORD(ref ao.address);
			commaBuf.GetString(ref ao.sExtraAddr);
			commaBuf.GetWORD(ref ao.wExtraAddr);

			commaBuf.GetInt(ref ao.fn);
			commaBuf.GetString(ref ao.unit);
            commaBuf.GetDouble(ref ao.fBase);
            commaBuf.GetDouble(ref ao.fFull);

			commaBuf.GetChar(ref ao.act);

			sbyte reserved=0;
			commaBuf.GetChar(ref reserved);

			commaBuf.GetInt(ref ao.nCalculateFilter);
            commaBuf.GetDouble(ref ao.plc_base);
            commaBuf.GetDouble(ref ao.plc_full);

			commaBuf.GetChar(ref reserved);		// 이전(ref 7.0)에는 wWriteRetryTime

			commaBuf.GetBYTE(ref ao.cTagLinkType);		// Dde를 태그로 사용할것인가?
			commaBuf.GetString(ref ao.sDdeService);
			commaBuf.GetString(ref ao.sDdeTopic);
			commaBuf.GetString(ref ao.sDdeItem);

			commaBuf.GetChar(ref ao.cDdeDataFormat);
			commaBuf.GetChar(ref ao.bBcdValue);
			commaBuf.GetChar(ref ao.bCutOverValue);
			commaBuf.GetChar(ref ao.bLocalTag);
		}

		void CommaBufToTagStructAO(ref TagAoClass ao, string buf)
		{
			CommaBufToAoFileStruct(ref ao, buf);

			ao.curr = (float)0.0;
			ao.old  = (float)0.0;
			ao.real_curr = 0;
		}

		public void TagLoadAO(string path)
		{
			//MakeFilePath mfp = new MakeFilePath();
			//string path;
			string buf = "";
			TagAoClass ao;
			TextReader reader;

			//path = mfp.Tag("AO.TAG");

			if(!File.Exists(path)) 
			{
				return;
			}
			reader = TagFile.TextReaderWithCheck(path, System.Text.Encoding.Default);
			if(reader == null) return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				if(buf.Length == 0)	continue;
				if(buf[0] == ';')	continue;
				

				ao = new TagAoClass();

				ao.enumTagType = EnumTagType.AO;
				
				CommaBufToTagStructAO(ref ao, buf);
				TagLib.groupRoot.AddTag(ao, false);
			}
			reader.Close();
		}

		void CommaBufToDiFileStruct(ref TagDiClass di, string buf)
		{
			int number=0;
			CommaBlockString commaBuf = new CommaBlockString();
			uint address=0;
			string imsi="";

			commaBuf.Set(buf);

			commaBuf.GetInt(ref number);
			commaBuf.GetString(ref di.tag);
			Tools.KillEndSpace(ref di.tag);
			di.name = di.tag;
			commaBuf.GetString(ref di.description);
			di.description = di.description.Trim();
			commaBuf.GetInt(ref di.port);
			commaBuf.GetInt(ref di.station);
			commaBuf.GetHexDWORD(ref address);
			imsi = String.Format("{0:X}", address/16);
			di.address_word = (uint)(ConvertTool.ToInt32(imsi));
			di.address_bit = (sbyte)(address%16);
            // 주소가 0이하인 경우가 있다. 201-10-17
            if (di.address_bit < 0) di.address_bit = 0;

			commaBuf.GetInt(ref di.fn);
			commaBuf.GetString(ref di.desON);
			commaBuf.GetString(ref di.desOFF);
			commaBuf.GetChar(ref di.act);
			commaBuf.GetChar(ref di.alarm);

			commaBuf.GetString(ref di.sSubOutDigital1);
			Tools.KillEndSpace(ref di.sSubOutDigital1);
			commaBuf.GetString(ref di.sSubOutDigital2);
			Tools.KillEndSpace(ref di.sSubOutDigital2);
			commaBuf.GetString(ref di.sSubOutDigitalOnTag);
			Tools.KillEndSpace(ref di.sSubOutDigitalOnTag);
			commaBuf.GetString(ref di.sSubOutDigitalOffTag);
			Tools.KillEndSpace(ref di.sSubOutDigitalOffTag);

			commaBuf.GetChar(ref di.bFileSave);	// 데이터 파일을 저장할 것이냐?
			commaBuf.GetChar(ref di.cAlarmType);	// 데이터 파일을 저장할 것이냐?
			commaBuf.GetString(ref di.sGraphicFile);		// 이 태그가 있는 그래픽 파일은?
			commaBuf.GetString(ref di.sAlarmWaveFile);		// 경보 발생시 사용할 그래픽 파일
			commaBuf.GetWORD(ref di.wAlarmPriority); 	// 경보울리는 방법.
			commaBuf.GetBYTE(ref di.cTagLinkType);		// Dde를 태그로 사용할것인가?
			commaBuf.GetString(ref di.sDdeService);
			commaBuf.GetString(ref di.sDdeTopic);
			commaBuf.GetString(ref di.sDdeItem);	
			commaBuf.GetChar(ref di.cOutLinkMethod);	// out1, out2 선택
			commaBuf.GetChar(ref di.cConfirmCount);	// 제어시 확인할 count
			ushort flags = 0;
			commaBuf.GetHexWORD(ref flags);		// 0 = 자동, 1 = 수동 기입중.
			di.wProtectFlags = (EnumProtectFlag)flags;
			commaBuf.GetChar(ref di.bReverse);			// 0 = 정상, 1 = 반전 태그.
			commaBuf.GetBYTE(ref di.bDdeRequest);
			commaBuf.GetWORD(ref di.wScanTime);
			if(di.wScanTime == 0) 
			{
				di.wScanTime = TagLib.DEFAULT_SCANTIME;
			}
			commaBuf.GetChar(ref di.bLocalTag);
		}

		void CommaBufToTagStructDI(ref TagDiClass di, string buf)
		{
			CommaBufToDiFileStruct(ref di, buf);

			di.curr = 0;
		}

		public void TagLoadDI(string path)
		{
			//MakeFilePath mfp = new MakeFilePath();
			//string path;
			string buf = "";
			TagDiClass di;
			TextReader reader;

			//path = mfp.Tag("DI.TAG");

			if(!File.Exists(path)) 
			{
				return;
			}
			
			reader = TagFile.TextReaderWithCheck(path, System.Text.Encoding.Default);
			if(reader == null) return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				if(buf.Length == 0)	continue;
				if(buf[0] == ';')	continue;
				

				di = new TagDiClass();

				di.enumTagType = EnumTagType.DI;
				
				CommaBufToTagStructDI(ref di, buf);
				TagLib.groupRoot.AddTag(di, false);
			}
			reader.Close();
		}

		void CommaBufToDoFileStruct(ref TagDoClass dout, string buf)
		{
			int number=0;
			//sbyte reserved=0;
			CommaBlockString commaBuf = new CommaBlockString();

			commaBuf.Set(buf);

			commaBuf.GetInt(ref number);
			commaBuf.GetString(ref dout.tag);
			Tools.KillEndSpace(ref dout.tag);
			dout.name = dout.tag;
			commaBuf.GetString(ref dout.description);
			dout.description = dout.description.Trim();
			commaBuf.GetInt(ref dout.port);
			commaBuf.GetInt(ref dout.station);
			commaBuf.GetHexDWORD(ref dout.address);
			commaBuf.GetString(ref dout.sExtraAddr);
			commaBuf.GetWORD(ref dout.wExtraAddr);
			commaBuf.GetString(ref dout.desON);
			commaBuf.GetString(ref dout.desOFF);
			commaBuf.GetChar(ref dout.act);
			commaBuf.GetChar(ref dout.cRelayType);
			commaBuf.GetWORD(ref dout.wRelaySecTarget);

            commaBuf.GetChar(ref dout.bReverse);	    // 2014-6-23 ref reserved 를 dount.bReverse로 수정함 이전에는 commaBuf.GetChar(ref reserved);	더 이전(ref 7.0)에는 wWriteRetryTime

			commaBuf.GetBYTE(ref dout.cTagLinkType);	// Dde를 태그로 사용할것인가?
			commaBuf.GetString(ref dout.sDdeService);
			commaBuf.GetString(ref dout.sDdeTopic);
			commaBuf.GetString(ref dout.sDdeItem);
			commaBuf.GetChar(ref dout.bLocalTag);
			commaBuf.GetInt(ref dout.nDelaySecON);
			commaBuf.GetInt(ref dout.nDelaySecOFF);
		}

		void CommaBufToTagStructDO(ref TagDoClass dout, string buf)
		{
			CommaBufToDoFileStruct(ref dout, buf);
	
			dout.curr = 0;
			//dout.wWriteRetryTimeCurr = 0;
		}

		public void TagLoadDO(string path)
		{
			//MakeFilePath mfp = new MakeFilePath();
			//string path;
			string buf = "";
			TagDoClass dout;
			TextReader reader;

			//path = mfp.Tag("DO.TAG");

			if(!File.Exists(path)) 
			{
				return;
			}

			reader = TagFile.TextReaderWithCheck(path, System.Text.Encoding.Default);
			if(reader == null) return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				if(buf.Length == 0)	continue;
				if(buf[0] == ';')	continue;
				

				dout = new TagDoClass();

				dout.enumTagType = EnumTagType.DO;
				
				CommaBufToTagStructDO(ref dout, buf);
				TagLib.groupRoot.AddTag(dout, false);
			}
			reader.Close();
		}

		void CommaBufToStFileStruct(ref TagStClass st, string buf)
		{
			int number=0;
			CommaBlockString commaBuf = new CommaBlockString();

			commaBuf.Set(buf);

			commaBuf.GetInt(ref number);
			commaBuf.GetString(ref st.tag);
			Tools.KillEndSpace(ref st.tag);
			st.name = st.tag;
			commaBuf.GetString(ref st.description);
			st.description = st.description.Trim();

			commaBuf.GetChar(ref st.act);

			if(commaBuf.IsEOS()) 
			{
				st.cTagLinkType = 2;	// 메모리 태그
			}
			else 
			{
				commaBuf.GetBYTE(ref st.cTagLinkType);		
				commaBuf.GetInt(ref st.port);
				commaBuf.GetWORD(ref st.address);
				commaBuf.GetBYTE(ref st.read_size);
				commaBuf.GetBYTE(ref st.read_method);
				commaBuf.GetChar(ref st.cMemoryType);
				commaBuf.GetString(ref st.sDdeService);
				commaBuf.GetString(ref st.sDdeTopic);
				commaBuf.GetString(ref st.sDdeItem);	
				commaBuf.GetBYTE(ref st.bDdeRequest);
				commaBuf.GetChar(ref st.bLocalTag);
				commaBuf.GetChar(ref st.bUseAsOutput);
			}

			if(st.read_size <= 0)	st.read_size = 1;
		}

		void CommaBufToTagStructST(ref TagStClass st, string buf)
		{
			CommaBufToStFileStruct(ref st, buf);
	
			st.curr = "";
		}

		public void TagLoadST(string path)
		{
			//MakeFilePath mfp = new MakeFilePath();
			//string path;
			string buf = "";
			TagStClass st;
			TextReader reader;

			//path = mfp.Tag("ST.TAG");

			if(!File.Exists(path)) 
			{
				return;
			}
			
			reader = TagFile.TextReaderWithCheck(path, System.Text.Encoding.Default);
			if(reader == null) return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				if(buf.Length == 0)	continue;
				if(buf[0] == ';')	continue;
				

				st = new TagStClass();

				st.enumTagType = EnumTagType.ST;
				
				CommaBufToTagStructST(ref st, buf);
				TagLib.groupRoot.AddTag(st, false);
			}
			reader.Close();
		}

		public void TagLoadDoGroup(string path)
		{
			//MakeFilePath mfp = new MakeFilePath();
			//string path;
			string buf = "";
			TagDoGroupClass gdo;
			TextReader reader;

			//path = mfp.Tag("do-group.TAG");

			if(!File.Exists(path)) 
			{
				return;
			}
			
			reader = new StreamReader(path, System.Text.Encoding.Default);
			if(reader == null) return;

			CommaBlockString comma = new CommaBlockString();

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				if(buf.Length == 0)	continue;
				if(buf[0] == ';')	continue;

				gdo = new TagDoGroupClass();

				gdo.enumTagType = EnumTagType.GDO;
				
				comma.Set(buf);
				comma.Skip();	// number
				comma.GetString(ref gdo.tag);
				gdo.tag = gdo.tag.Trim();

				gdo.name = gdo.tag;

				if(comma.IsEOS())	break;	// file broken or too old version

				comma.GetString(ref gdo.description);
				gdo.description = gdo.description.Trim();
				comma.GetChar(ref gdo.act);

				while(true) 
				{
					DoGroupMember member = new DoGroupMember();
					comma.GetString(ref member.tag);
					member.tag = member.tag.Trim();
					if(member.tag.Length > 0) 
					{
						gdo.member.Add(member);
					}

					if(comma.IsEOS())	break;
				}

				TagLib.groupRoot.AddTag(gdo, false);
			}
			reader.Close();
		}

	}
}

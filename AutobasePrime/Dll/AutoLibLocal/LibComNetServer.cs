using System;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for LibComNetServer.
	/// </summary>
	public class LibComNetServer
	{
		public LibComNetServer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public delegate void DelegateSendEventProgramToNetwork(byte[] data, int size);
		public static DelegateSendEventProgramToNetwork procSendEventProgramToNetwork = null;

		public static void SendEventProgramToNetwork(byte[] data, int size)
		{
			if(procSendEventProgramToNetwork == null)	return;
			if(ConfigVarTotal.bLocalFlag == false)		return;	// 웹클라이언트로 실행중

			procSendEventProgramToNetwork(data, size);
		}

		public static void SendCommandTagValueChanged(string tag, double value)
		{
			NetWorkProtocolSend send = new NetWorkProtocolSend();
			string buf;
			ushort trans = NetWorkProtocolSend.GetTransaction();

			buf = String.Format("Tag={0},Value={1}", tag, value);

			send.MakeBlock(EnumNetworkCommand.TAG_VALUE_CHANGE, trans, buf);
			SendEventProgramToNetwork(send.bufSend, send.nBufCount);
		}

		public static void SendCommandProtectFlagChange(string tag, int value)
		{
			// int SendCommandToAllNet(char *buf, int count);
			NetWorkProtocolSend send = new NetWorkProtocolSend();
			string buf;
			ushort trans = NetWorkProtocolSend.GetTransaction();

			buf = String.Format("Tag={0},Value={1}", tag, value);

			send.MakeBlock(EnumNetworkCommand.TAG_PROTECT_FLAG_CHANGE, trans, buf);
			SendEventProgramToNetwork(send.bufSend, send.nBufCount);
		}

		public static void SendCommandToNetWorkTagMemberChanged(string tag, EnumTagMember member, double value)
		{
			if(ConfigVarTotal.bLocalFlag == false)		return;	// 웹클라이언트로 실행중
			if(SystemStatusMemory.GetDI(SSMDI.CommunicationValue) == 0 && member == EnumTagMember.TAG_MEMBER_curr)	return;

			NetWorkProtocolSend send = new NetWorkProtocolSend();
			string buf;
			ushort trans = NetWorkProtocolSend.GetTransaction();

			buf = String.Format("Tag={0},TagMember={1},Value={2}", tag, (int)member, value);

			send.MakeBlock(EnumNetworkCommand.TAG_MEMBER_CHANGED, trans, buf);
			SendEventProgramToNetwork(send.bufSend, send.nBufCount);
		}

		public static void SendCommandToNetWorkTagMemberChanged(string tag, EnumTagMember member, string value)
		{
			if(ConfigVarTotal.bLocalFlag == false)		return;	// 웹클라이언트로 실행중
			if(SystemStatusMemory.GetDI(SSMDI.CommunicationValue) == 0 && member == EnumTagMember.TAG_MEMBER_curr)	return;

			NetWorkProtocolSend send = new NetWorkProtocolSend();
			string buf;
			ushort trans = NetWorkProtocolSend.GetTransaction();

			buf = String.Format("Tag={0},TagMember={1},String={2}", tag, (int)member, value);

			send.MakeBlock(EnumNetworkCommand.TAG_MEMBER_CHANGED, trans, buf);
			SendEventProgramToNetwork(send.bufSend, send.nBufCount);
		}

		public static void CheckChangedMember(TagPublicClass src, TagPublicClass tar)
		{
			if(src.description != tar.description)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_description, src.description);

			if(src.cTagLinkType != tar.cTagLinkType)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_cTagType, src.cTagLinkType);

			if(src.act != tar.act)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_act, src.act);

			if(src.sDdeItem != tar.sDdeItem)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sDdeItem, src.sDdeItem);

			if(src.sDdeService != tar.sDdeService)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sDdeService, src.sDdeService);

			if(src.sDdeTopic != tar.sDdeTopic)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sDdeTopic, src.sDdeTopic);

			if(src.enumTagType == EnumTagType.AI) 
			{
				CheckChangedMemberLocal((TagAiClass)src, (TagAiClass)tar);
			}
			else if(src.enumTagType == EnumTagType.AO) 
			{
				CheckChangedMemberLocal((TagAoClass)src, (TagAoClass)tar);
			}
			else if(src.enumTagType == EnumTagType.DI) 
			{
				CheckChangedMemberLocal((TagDiClass)src, (TagDiClass)tar);
			}
			else if(src.enumTagType == EnumTagType.DO) 
			{
				CheckChangedMemberLocal((TagDoClass)src, (TagDoClass)tar);
			}
			else if(src.enumTagType == EnumTagType.ST) 
			{
				CheckChangedMemberLocal((TagStClass)src, (TagStClass)tar);
			}
			else {}
		}

		static void CheckChangedMemberLocal(TagAiClass src, TagAiClass tar)
		{
			if(src.cAlarmType != tar.cAlarmType)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_cAlarmType, src.cAlarmType);

			if(src.port != tar.port)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_port, src.port);

			if(src.station != tar.station)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_station, src.station);
	
			if(src.address != tar.address)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_address, src.address);

			if(src.fBase != tar.fBase)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_base, src.fBase);

			if(src.fFull != tar.fFull)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_full, src.fFull);

			if(src.bBcdValue != tar.bBcdValue)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_bBcdValue, src.bBcdValue);

			if(src.bCutOverValue != tar.bCutOverValue)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_bCutOverValue, src.bCutOverValue);

			if(src.cConfirmCount != tar.cConfirmCount)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_cConfirmCount, src.cConfirmCount);

			if(src.fAlarmReturnGab != tar.fAlarmReturnGab)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_fAlarmReturnGab, src.fAlarmReturnGab);

			if(src.fDisplayFormat != tar.fDisplayFormat)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_fDisplayFormat, src.fDisplayFormat);

			if(src.hihi != tar.hihi)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_hihi, src.hihi);

			if(src.high != tar.high)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_high, src.high);

			if(src.low != tar.low)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_low, src.low);

			if(src.lolo != tar.lolo)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_lolo, src.lolo);

			if(src.fPlcBase != tar.fPlcBase)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_plc_base, src.fPlcBase);

			if(src.fPlcFull != tar.fPlcFull)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_plc_full, src.fPlcFull);

			if(src.nCalcDelay != tar.nCalcDelay)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_nCalcDelay, src.nCalcDelay);

			if(src.nCalculateFilter != tar.nCalculateFilter)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_nCalculateFilter, src.nCalculateFilter);

			if(src.sAlarmWaveFile != tar.sAlarmWaveFile)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sAlarmWaveFile, src.sAlarmWaveFile);

			if(String.Compare(src.sGraphicFile, tar.sGraphicFile) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sGraphicFile, src.sGraphicFile);

			if(String.Compare(src.sSubOutAnalog, tar.sSubOutAnalog) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sSubOutAnalog, src.sSubOutAnalog);

			if(src.sSubOutAnalogSP != tar.sSubOutAnalogSP)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sSubOutAnalogSP, src.sSubOutAnalogSP);

			if(src.sSubOutDigitalHiHi != tar.sSubOutDigitalHiHi)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sSubOutDigitalHiHi, src.sSubOutDigitalHiHi);

			if(src.sSubOutDigitalLoLo != tar.sSubOutDigitalLoLo)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sSubOutDigitalLoLo, src.sSubOutDigitalLoLo);

			if(src.unit != tar.unit)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_unit, src.unit);

			if(src.view_base != tar.view_base)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_viewbase, src.view_base);

			if(src.view_full != tar.view_full)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_viewfull, src.view_full);

			if(src.wAlarmPriority != tar.wAlarmPriority)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_wAlarmPriority, src.wAlarmPriority);

			if(src.wProtectFlags != tar.wProtectFlags)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_wProtectFlags, (int)src.wProtectFlags);

			if(src.wRateOfChangeLimit != tar.wRateOfChangeLimit)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_wRateOfChangeLimit, src.wRateOfChangeLimit);
		}

		static void CheckChangedMemberLocal(TagAoClass src, TagAoClass tar)
		{
			if(src.port != tar.port)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_port, src.port);

			if(src.station != tar.station)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_station, src.station);

			if(src.address != tar.address)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_address, src.address);

			if(src.fBase != tar.fBase)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_base, src.fBase);

			if(src.fFull != tar.fFull)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_full, src.fFull);

			if(src.bBcdValue != tar.bBcdValue)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_bBcdValue, src.bBcdValue);

			if(src.bCutOverValue != tar.bCutOverValue)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_bCutOverValue, src.bCutOverValue);

			if(src.nCalculateFilter != tar.nCalculateFilter)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_nCalculateFilter, src.nCalculateFilter);

			if(String.Compare(src.unit, tar.unit) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_unit, src.unit);

			if(String.Compare(src.sExtraAddr, tar.sExtraAddr) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_extra1, src.sExtraAddr);

			if(src.wExtraAddr != tar.wExtraAddr)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_extra2, src.wExtraAddr);

			if(src.cDdeDataFormat != tar.cDdeDataFormat)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_cDdeDataFormat, src.cDdeDataFormat);

			if(src.plc_base != tar.plc_base)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_plc_base, src.plc_base);

			if(src.plc_full != tar.plc_full)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_plc_full, src.plc_full);
		}

		static void CheckChangedMemberLocal(TagDiClass src, TagDiClass tar)
		{
			if(src.cAlarmType != tar.cAlarmType)	
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_cAlarmType, src.cAlarmType);

			if(src.port != tar.port)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_port, src.port);

			if(src.station != tar.station)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_station, src.station);

			if(src.address_word != tar.address_word)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_address, src.address_word);

			if(src.cConfirmCount != tar.cConfirmCount)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_cConfirmCount, src.cConfirmCount);

			if(String.Compare(src.sAlarmWaveFile, tar.sAlarmWaveFile) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sAlarmWaveFile, src.sAlarmWaveFile);

			if(String.Compare(src.sGraphicFile, tar.sGraphicFile) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sGraphicFile, src.sGraphicFile);

			if(src.wAlarmPriority != tar.wAlarmPriority)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_wAlarmPriority, src.wAlarmPriority);

			if(src.wProtectFlags != tar.wProtectFlags)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_wProtectFlags, (int)src.wProtectFlags);

			if(src.bReverse != tar.bReverse)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_bReverse, src.bReverse);

			if(src.cOutLinkMethod != tar.cOutLinkMethod)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_cOutLinkMethod, src.cOutLinkMethod);

			if(String.Compare(src.desOFF, tar.desOFF) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_desOFF, src.desOFF);

			if(String.Compare(src.desON, tar.desON) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_desON, src.desON);

			if(String.Compare(src.sSubOutDigital1, tar.sSubOutDigital1) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sSubOutDigital1, src.sSubOutDigital1);

			if(String.Compare(src.sSubOutDigital2, tar.sSubOutDigital2) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sSubOutDigital2, src.sSubOutDigital2);

			if(String.Compare(src.sSubOutDigitalOffTag, tar.sSubOutDigitalOffTag) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sSubOutDigitalOffTag, src.sSubOutDigitalOffTag);

			if(String.Compare(src.sSubOutDigitalOnTag, tar.sSubOutDigitalOnTag) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_sSubOutDigitalOnTag, src.sSubOutDigitalOnTag);
		}

		static void CheckChangedMemberLocal(TagDoClass src, TagDoClass tar)
		{
			if(src.port != tar.port)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_port, src.port);

			if(src.station != tar.station)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_station, src.station);

			if(src.address != tar.address)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_address, src.address);

			if(String.Compare(src.desOFF, tar.desOFF) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_desOFF, src.desOFF);

			if(String.Compare(src.desON, tar.desON) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_desON, src.desON);

			if(src.cRelayType != tar.cRelayType)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_cRelayType, src.cRelayType);

			if(String.Compare(src.sExtraAddr, tar.sExtraAddr) != 0)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_extra1, src.sExtraAddr);

			if(src.wExtraAddr != tar.wExtraAddr)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_extra2, src.wExtraAddr);

			if(src.wRelaySecTarget != tar.wRelaySecTarget)
				SendCommandToNetWorkTagMemberChanged(src.tag, EnumTagMember.TAG_MEMBER_wRelaySecTarget, src.wRelaySecTarget);
		}

		static void CheckChangedMemberLocal(TagStClass src, TagStClass tar)
		{
		}
	}
}

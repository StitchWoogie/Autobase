using System;
using AutoLibLocal;
using GraphicModule;
using System.Threading;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// Summary description for CheckEngineNetworkToViewMain.
	/// </summary>
	public class CheckEngineNetworkToViewMain
	{
		public CheckEngineNetworkToViewMain()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static void ChangeProtectFlag(string tag, int value)
		{
			int[] tag_pos = new int[1];

			TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				ai.wProtectFlags = (EnumProtectFlag)value;
				TagLib.bChangedByLocalMain = true;
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				TagDiClass di = (TagDiClass)tp;
				di.wProtectFlags = (EnumProtectFlag)value;
				TagLib.bChangedByLocalMain = true;
			}
			else {}
		}

		static bool ChangeTagMember(TagAiClass tag, ushort member, double value, string str)
		{
			switch((EnumTagMember)member) 
			{
				case EnumTagMember.TAG_MEMBER_alarm:				tag.alarm = (byte)value;			break;
				case EnumTagMember.TAG_MEMBER_bFileSave:			tag.bFileSave = (byte)value;
                                                                    TagLib.bNeedFileSaveList = true;   // 파일저장 목록을 새로 만들어야 한다.
                                                                    break;
				case EnumTagMember.TAG_MEMBER_cAlarmType:			tag.cAlarmType = (byte)value;		break;
				case EnumTagMember.TAG_MEMBER_port:				tag.port = (short)value;				break;
				case EnumTagMember.TAG_MEMBER_station:			tag.station = (short)value;			break;
				case EnumTagMember.TAG_MEMBER_address:			tag.address = (ushort)value;			break;
				case EnumTagMember.TAG_MEMBER_base:				tag.fBase = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_full:				tag.fFull = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_bBcdValue:			tag.bBcdValue = (byte)value;		break;
				case EnumTagMember.TAG_MEMBER_bCutOverValue:		tag.bCutOverValue = (byte)value;	break;
				case EnumTagMember.TAG_MEMBER_cConfirmCount:		tag.cConfirmCount = (byte)value;	break;
				case EnumTagMember.TAG_MEMBER_fAlarmReturnGab:	tag.fAlarmReturnGab = (float)value;break;
				case EnumTagMember.TAG_MEMBER_fDisplayFormat:		tag.fDisplayFormat = (float)value;	break;
				case EnumTagMember.TAG_MEMBER_hihi:				tag.hihi = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_high:				tag.high = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_low:				tag.low = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_lolo:				tag.lolo = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_plc_full:				tag.fPlcFull = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_plc_base:				tag.fPlcBase = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_nCalcDelay:			tag.nCalcDelay = (short)value;		break;
				case EnumTagMember.TAG_MEMBER_nCalculateFilter:	tag.nCalculateFilter = (short)value;break;
				case EnumTagMember.TAG_MEMBER_sAlarmWaveFile:		tag.sAlarmWaveFile = str;	break;
				case EnumTagMember.TAG_MEMBER_sGraphicFile:			tag.sGraphicFile = str;	break;
				case EnumTagMember.TAG_MEMBER_sSubOutAnalog:		tag.sSubOutAnalog = str;	break;
				case EnumTagMember.TAG_MEMBER_sSubOutAnalogSP:		tag.sSubOutAnalogSP = str;break;
				case EnumTagMember.TAG_MEMBER_sSubOutDigitalHiHi:	tag.sSubOutDigitalHiHi = str;break;
				case EnumTagMember.TAG_MEMBER_sSubOutDigitalLoLo:	tag.sSubOutDigitalLoLo = str;break;
				case EnumTagMember.TAG_MEMBER_unit:					tag.unit = str;			break;
				case EnumTagMember.TAG_MEMBER_viewbase:				tag.view_base = (float)value;		break;
				case EnumTagMember.TAG_MEMBER_viewfull:				tag.view_full = (float)value;		break;
				case EnumTagMember.TAG_MEMBER_wAlarmPriority:		tag.wAlarmPriority = (ushort)value;	break;
				case EnumTagMember.TAG_MEMBER_wProtectFlags:		tag.wProtectFlags = (EnumProtectFlag)value;	break;
				case EnumTagMember.TAG_MEMBER_wRateOfChangeLimit:	tag.wRateOfChangeLimit = (ushort)value;break;	
				default:
					return false;
			}
			return true;
		}

		static bool ChangeTagMember(TagAoClass tag, ushort member, double value, string str)
		{
			switch((EnumTagMember)member) 
			{
				case EnumTagMember.TAG_MEMBER_port:				tag.port = (short)value;			break;
				case EnumTagMember.TAG_MEMBER_station:			tag.station = (short)value;		break;
				case EnumTagMember.TAG_MEMBER_address:			tag.address = (uint)value;		break;
				case EnumTagMember.TAG_MEMBER_base:				tag.fBase = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_full:				tag.fFull = (float)value;			break;
				case EnumTagMember.TAG_MEMBER_bBcdValue:			tag.bBcdValue = (sbyte)value;		break;
				case EnumTagMember.TAG_MEMBER_bCutOverValue:		tag.bCutOverValue = (sbyte)value;	break;
				case EnumTagMember.TAG_MEMBER_nCalculateFilter:	tag.nCalculateFilter = (short)value;break;
				case EnumTagMember.TAG_MEMBER_unit:				tag.unit = str;			break;

				case EnumTagMember.TAG_MEMBER_extra1:				tag.sExtraAddr = str;		break;
				case EnumTagMember.TAG_MEMBER_extra2:				tag.wExtraAddr = (ushort)value;		break;

				case EnumTagMember.TAG_MEMBER_cDdeDataFormat:		tag.cDdeDataFormat = (sbyte)value;	break;
				case EnumTagMember.TAG_MEMBER_plc_base:			tag.plc_base = (float)value;		break;
				case EnumTagMember.TAG_MEMBER_plc_full:			tag.plc_full = (float)value;		break;
				default:
					return false;
			}
			return true;
		}

		static bool ChangeTagMember(TagDiClass tag, ushort member, double value, string str)
		{
			switch((EnumTagMember)member) 
			{
				case EnumTagMember.TAG_MEMBER_alarm:				tag.alarm = (sbyte)value;			break;
				case EnumTagMember.TAG_MEMBER_bFileSave:			tag.bFileSave = (sbyte)value;
                                                                    TagLib.bNeedFileSaveList = true;   // 파일저장 목록을 새로 만들어야 한다.
                                                                    break;
				case EnumTagMember.TAG_MEMBER_cAlarmType:			tag.cAlarmType = (sbyte)value;		break;
				case EnumTagMember.TAG_MEMBER_port:					tag.port = (short)value;			break;
				case EnumTagMember.TAG_MEMBER_station:				tag.station = (short)value;			break;
				case EnumTagMember.TAG_MEMBER_address:				tag.address_word = (uint)value;			break;
				case EnumTagMember.TAG_MEMBER_cConfirmCount:		tag.cConfirmCount = (sbyte)value;	break;
				case EnumTagMember.TAG_MEMBER_sAlarmWaveFile:		tag.sAlarmWaveFile = str;	break;
				case EnumTagMember.TAG_MEMBER_sGraphicFile:			tag.sGraphicFile = str;	break;
				case EnumTagMember.TAG_MEMBER_wAlarmPriority:		tag.wAlarmPriority = (ushort)value;	break;
				case EnumTagMember.TAG_MEMBER_wProtectFlags:		tag.wProtectFlags = (EnumProtectFlag)value;	break;

				case EnumTagMember.TAG_MEMBER_bReverse:				tag.bReverse = (sbyte)value;		break;
				case EnumTagMember.TAG_MEMBER_cOutLinkMethod:		tag.cOutLinkMethod = (sbyte)value;	break;
				case EnumTagMember.TAG_MEMBER_desOFF:				tag.desOFF = str;			break;
				case EnumTagMember.TAG_MEMBER_desON:				tag.desON = str;			break;
				case EnumTagMember.TAG_MEMBER_sSubOutDigital1:		tag.sSubOutDigital1 = str;break;
				case EnumTagMember.TAG_MEMBER_sSubOutDigital2:		tag.sSubOutDigital2 = str;break;
				case EnumTagMember.TAG_MEMBER_sSubOutDigitalOffTag:	tag.sSubOutDigitalOffTag = str;	break;
				case EnumTagMember.TAG_MEMBER_sSubOutDigitalOnTag:	tag.sSubOutDigitalOnTag = str;	break;
				default:
					return false;
			}
			return true;
		}

		static bool ChangeTagMember(TagDoClass tag, ushort member, double value, string str)
		{
			switch((EnumTagMember)member) 
			{
				case EnumTagMember.TAG_MEMBER_port:				tag.port = (short)value;		break;
				case EnumTagMember.TAG_MEMBER_station:			tag.station = (short)value;		break;
				case EnumTagMember.TAG_MEMBER_address:			tag.address = (uint)value;		break;

				case EnumTagMember.TAG_MEMBER_desOFF:			tag.desOFF = str;			break;
				case EnumTagMember.TAG_MEMBER_desON:			tag.desON = str;			break;

				case EnumTagMember.TAG_MEMBER_cRelayType:		tag.cRelayType = (sbyte)value;		break;
				case EnumTagMember.TAG_MEMBER_extra1:			tag.sExtraAddr = str;		break;
				case EnumTagMember.TAG_MEMBER_extra2:			tag.wExtraAddr = (ushort)value;		break;
				case EnumTagMember.TAG_MEMBER_wRelaySecTarget:	tag.wRelaySecTarget = (ushort)value;	break;
				default:
					return false;
			}
			return true;
		}

		static bool ChangeTagMember(TagStClass tag, ushort member, double value, string str)
		{
			switch((EnumTagMember)member) 
			{
				case EnumTagMember.TAG_MEMBER_curr:		tag.curr = str;			break;
				default:
					return false;
			}
			return true;
		}

		static void ChangeTagMemberChanged(string tag, ushort member, double val, string str)
		{
			TagPublicClass tp;
			int[] tag_pos = new int[1];
			bool change_flag = true;

			tp = TagLib.GetStructPublic(tag, ref tag_pos);

			switch((EnumTagMember)member) 
			{
				case EnumTagMember.TAG_MEMBER_description:	
					tp.description = str;
					break;
				case EnumTagMember.TAG_MEMBER_act:	
					tp.act = (sbyte)val;
					break;
				case EnumTagMember.TAG_MEMBER_cTagType:	
					tp.cTagLinkType = (byte)val;
					break;
				case EnumTagMember.TAG_MEMBER_sDdeService:	
					tp.sDdeService = str;
					break;
				case EnumTagMember.TAG_MEMBER_sDdeTopic:	
					tp.sDdeTopic = str;
					break;
				case EnumTagMember.TAG_MEMBER_sDdeItem:	
					tp.sDdeItem = str;
					break;
				default:
					if(tp.enumTagType == EnumTagType.AI) 
					{
						change_flag = ChangeTagMember((TagAiClass)tp, member, val, str);
					}
					else if(tp.enumTagType == EnumTagType.AO) 
					{
						change_flag = ChangeTagMember((TagAoClass)tp, member, val, str);
					}
					else if(tp.enumTagType == EnumTagType.DI) 
					{
						change_flag = ChangeTagMember((TagDiClass)tp, member, val, str);
					}
					else if(tp.enumTagType == EnumTagType.DO) 
					{
						change_flag = ChangeTagMember((TagDoClass)tp, member, val, str);
					}
					else if(tp.enumTagType == EnumTagType.ST) 
					{
						change_flag = ChangeTagMember((TagStClass)tp, member, val, str);
					}
					else 
					{
						change_flag = false;
					}
					break;
			}

			if(change_flag) 
			{
				SendEventToChild.SendPaintToChild();
			}
		}

		static void ChangeTagValue(string tag, double value)
		{
			int[] tag_pos = new int[1];

			TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				if(ai.curr != value) 
				{
					ai.curr = value;
					SendEventToChild.SendEventAIToChild(ai);
				}

				return;
			}
			if(tp.enumTagType == EnumTagType.DI) 
			{
				TagDiClass di = (TagDiClass)tp;
				if(di.curr != value) 
				{
					di.curr = (sbyte)value;
					SendEventToChild.SendEventDIToChild(di);
				}

				return;
			}

			// 해당되는 태그를 찾을 수가 없다.
		}

		static Thread threadMain;
        static RingSharedMemory smNetworkToViewMain = new RingSharedMemory();
        static RingSharedMemory smViewMainToNetwork = new RingSharedMemory();

		public static void Init()
		{
            smNetworkToViewMain.Create("NetShareNetworkToViewMain", 10, 1000);
            smViewMainToNetwork.Create("NetShareViewMainToNetwork", 10, 1000);
			threadMain = new Thread(new ThreadStart(ThreadLoop));
			threadMain.Start();
		}

		public static void UnInit()
		{
			if(threadMain == null)	return;	// // 초기 로그인에서 실패시 걸림

			bEnd = true;
			threadMain.Join(5000);

            smNetworkToViewMain.Close();
            smViewMainToNetwork.Close();
		}

		static bool bEnd = false;
		static byte[] readyData = null;

		static void ThreadLoop()
		{
			while(!bEnd) 
			{
				Thread.Sleep(10);
                
				if(readyData != null)	continue;

                // Timer에서 COM+를 사용하면 윈도우 최소화 되었다가 활성화가 잘 안되는 경우가 있다.
				// 그래서 Thread에서 값을 읽어오고 Timer에서는 값만 사용한다.

                readyData = (byte[])smNetworkToViewMain.GetItem();
			}
		}

		public static void Check()
		{
			if(readyData == null)	return;

			if(readyData != null) 
			{
				NetWorkProtocolRecv recv = new NetWorkProtocolRecv();

				recv.Split(readyData, readyData.Length);

				if((EnumNetworkCommand)recv.wCommand == EnumNetworkCommand.TAG_PROTECT_FLAG_CHANGE) 
				{
					ChangeProtectFlag(recv.sTag, (int)recv.fValue);
				}
				else if((EnumNetworkCommand)recv.wCommand == EnumNetworkCommand.TAG_VALUE_CHANGE) 
				{
					ChangeTagValue(recv.sTag, recv.fValue);
				} 
				else if((EnumNetworkCommand)recv.wCommand == EnumNetworkCommand.ALARM_LIST_CONFIRM_ONE) 
				{
					FormAlarmEvent.AlarmListConfirmOneByNetWork(recv.sTag, recv.sString);
				}
				else if((EnumNetworkCommand)recv.wCommand == EnumNetworkCommand.ALARM_LIST_DELETE_ONE) 
				{
					FormAlarmEvent.AlarmListDeleteOneByNetWork(recv.sTag, recv.sString);
				}
				else if((EnumNetworkCommand)recv.wCommand == EnumNetworkCommand.TAG_MEMBER_CHANGED) 
				{
					ChangeTagMemberChanged(recv.sTag, recv.wTagMember, recv.fValue, recv.sString);
				}
				else {}
			}

			readyData = null;
		}

		public static void SendEventProgramToNetwork(byte[] data, int size)
		{
            
            /* Open은 Mutex 가 없을 때 try catch가 발생한다.
            System.Threading.Mutex gM1;
            try
            {
                gM1 = System.Threading.Mutex.OpenExisting("AutoBaseNetServerMutex");
            }
            catch
            {
                gM1 = null;
            }

            if (gM1 == null)
            {
                try
                {
                    gM1 = System.Threading.Mutex.OpenExisting("AutoBaseNetClientMutex");
                }
                catch
                {
                    gM1 = null;
                }
                if (gM1 == null) return;
            }*/
            if (!LinePrinter.IsMutexExisting("AutoBaseNetServerMutex"))
            {
                if (!LinePrinter.IsMutexExisting("AutoBaseNetClientMutex"))
                    return;
            }

            if (data.Length == size)
                smViewMainToNetwork.AddItem(data);
            else
            {
                byte[] data2 = new byte[size];
                for (int i = 0; i < size; i++)
                {
                    data2[i] = data[i];
                }
                smViewMainToNetwork.AddItem(data2);
            }
            /*
			using(ComNetServer.ClassServer com = new ComNetServer.ClassServer()) 
			{
				com.AddToNetwork(data, size);
			}*/
		}
    }
}

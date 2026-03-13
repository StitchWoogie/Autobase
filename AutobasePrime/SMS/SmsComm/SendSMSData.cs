using System;
using SMS.SmsFunc;
using NetTools;
using System.Net;
using System.Net.Sockets;
using AutoLibLocal;
using SMS.Display;
using System.Threading;
using DialogCommon;
using System.IO;
using System.Collections.Generic; 


namespace SMS.SmsComm
{
	/// <summary>
	/// Summary description for SendSMSData.
	/// </summary>
	public class SendSMSData
	{
		//static bool bConnectionFlag = false;
		public enum eCommStatus { COMM_OK = 1, COMM_CRLF, COMM_ERROR, COMM_WAITING, };
        public enum eConectionType { SERIAL = 0, UDP_IP = 1, WEB_SERVICE = 2 }; 
        public enum eCdmaType { LG_MOS = 0, ANY_CALL, ANY_CALL_CDMA, ANY_CALL_CDMA2000, ANY_CALL_CDMA2000_SPH, AIR_NCL, TELIT_BSM860S, KTF_MOBICON, GSM_MODEM, EMV_1800K_CDMA, M2M_WM215 };// GSM Modem, add 2012-04-09 for vietnam, EMV-1800K CDMA add 2012-12-18
		public enum eMsgType { NORMAL = 1, FAST = 2, URGENCY = 3, };

		enum eSendCommand { NORMAL, AT_PHONENUM_COMMAND, AT_PING_COMMAND, DATA_SEND_COMMAND, };	// 제어코드 명령어 형태, LG_MOS를 위해

		public static bool	bSmsCommThreadPause = false;
		public static bool	bSmsCommThreadPaused = false;

        private static Dictionary<string, DateTime> lastSendTimeByUser = new Dictionary<string, DateTime>(); //20241111 PSU
        private static DateTime lastSendTime = DateTime.MinValue; //20241111 PSU

		public SendSMSData()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static public bool smsCommThreadPause(bool bPause)
		{
			bSmsCommThreadPause = bPause;
			TimeOutClass timeout = new TimeOutClass();

			timeout.Reset();
			while(true)
			{
				Thread.Sleep(1);
				if(bSmsCommThreadPaused == bPause) return true;
				if(timeout.IsTimeOut(5)) return false;	// 5초 동안만 기다린다
			}
		}

		public static void sendSmsProcThread()
		{
			LanguageTool.ChangeUICulture();
			while(Form1.bSmsCommThreadEnd == false) 
			{
				Thread.Sleep(1);
				bSmsCommThreadPaused = bSmsCommThreadPause;
				if(bSmsCommThreadPaused) continue;
				sendSmsProc();
				SmsServerClientProc.SmsServerClientDataRecvCheck();
			}
		}

        //public static void smsSleepSec(int i)   // add 2007-10-23
        //{
        //    TimeOutClass timeout = new TimeOutClass();

        //    while (true)
        //    {
        //        timeout.Reset();
        //        while (true)
        //        {
        //            Thread.Sleep(1);
        //            if (timeout.IsTimeOut(i)) return;
        //        }
        //    }
        //}


        //public static void sendSmsProc()
        //{			
        //    if(SmsBasic.smsConfig.bStopSmsCall || SmsBasic.arrSmsQueue.Count <= 0) return;

        //    alarmDataMain	data = (alarmDataMain)SmsBasic.arrSmsQueue[0];
			
        //    if(data == null || data.nUserNo < 0 || data.nUserNo > SmsBasic.arrUserSetting.Count) // 사용자번호가 이상인지...
        //    {
        //        if(data.nUserNo == SmsBasic.smsScriptUser.nUserNo)								// 스크립트 사용자 ...
        //        {
        //            SmsBasic.smsScriptUser.phoneNo = data.phoneNo;
        //        }
        //        else 
        //        {
        //            SmsBasic.arrSmsQueue.Remove(data);
        //            return;
        //        }
        //    }

        //    eCommStatus		retn;
        //    retn = WriteSmsMessage(data);

        //    smsUserConfig	user;
        //    if(data.nUserNo == SmsBasic.smsScriptUser.nUserNo) user = SmsBasic.smsScriptUser;
        //    else user = (smsUserConfig)SmsBasic.arrUserSetting[data.nUserNo];
        //    user.eConnectStatus = retn;
        //    if(retn == eCommStatus.COMM_WAITING) return;

        //    if(retn == eCommStatus.COMM_OK) 
        //    {
        //        okOrErrorProcess(data, false);
        //        SmsBasic.arrSmsQueue.Remove(data);				
        //        user.retry_count = 0;
        //        if(SmsMainDisplayForm.formThis != null && data.nUserNo != SmsBasic.smsScriptUser.nUserNo)	// 사용자 표시되고, 스크립트 사용자가 아니면 MDI 화면을 다시 그린다
        //            SmsMainDisplayForm.formThis.CallByThreadReDrawUserInfo(data.nUserNo % 256);
        //        if (data.bManual == false)                              // add 2007-10-23
        //            smsSleepSec(SmsBasic.smsConfig.nDelayDataCall);     // add 2007-10-23
        //        return;
        //    }				
        //    if(data.bManual)			// 수동이면서 에러
        //    {
        //        okOrErrorProcess(data, true);
        //        SmsBasic.arrSmsQueue.Remove(data);
        //        user.retry_count = 0;
        //        if(SmsMainDisplayForm.formThis != null && data.nUserNo != SmsBasic.smsScriptUser.nUserNo)	// 사용자 표시되고, 스크립트 사용자가 아니면 MDI 화면을 다시 그린다
        //            SmsMainDisplayForm.formThis.CallByThreadReDrawUserInfo(data.nUserNo % 256);				
        //    }
        //    else 
        //    {
        //        user.retry_count++;
        //        if(SmsMainDisplayForm.formThis != null && data.nUserNo != SmsBasic.smsScriptUser.nUserNo)	// 사용자 표시되고, 스크립트 사용자가 아니면 MDI 화면을 다시 그린다
        //            SmsMainDisplayForm.formThis.CallByThreadReDrawUserInfo(data.nUserNo % 256);
        //        if(user.retry_count >= SmsBasic.smsConfig.nRetryErrorCount) 
        //        {
        //            okOrErrorProcess(data, true);
        //            SmsBasic.arrSmsQueue.Remove(data);
        //            user.retry_count = 0;
        //        }
        //        if (data.bManual == false)                              // add 2007-10-23
        //            smsSleepSec(SmsBasic.smsConfig.nDelayDataCall);     // add 2007-10-23
        //    }
        //}



        private static void CheckAndSleep(string phoneNo) //20241111 PSU
        {
            DateTime currentTime = DateTime.Now;

            // 동일사용자의마지막전송시간확인
            if (lastSendTimeByUser.ContainsKey(phoneNo))
            {
                TimeSpan diffTime = currentTime - lastSendTimeByUser[phoneNo];
                if (diffTime.TotalSeconds < SmsBasic.smsConfig.nDelayDataCall)
                {
                    Thread.Sleep((int)(SmsBasic.smsConfig.nDelayDataCall - diffTime.TotalSeconds) * 1000);
                }
            }
            // 다른사용자와의간격확인
            else if (lastSendTime != DateTime.MinValue)
            {
                TimeSpan diffTime = currentTime - lastSendTime;
                if (diffTime.TotalSeconds < SmsBasic.smsConfig.nDelaySiteCall)
                {
                    Thread.Sleep((int)(SmsBasic.smsConfig.nDelaySiteCall - diffTime.TotalSeconds) * 1000);
                }
            }

            // 현재시간으로업데이트
            lastSendTimeByUser[phoneNo] = DateTime.Now;
            lastSendTime = DateTime.Now;
        }

        public static void sendSmsProc()  //20241111 PSU
        {
            if (SmsBasic.smsConfig.bStopSmsCall || SmsBasic.arrSmsQueue.Count <= 0) return;

            alarmDataMain data = (alarmDataMain)SmsBasic.arrSmsQueue[0];

            if (data == null || data.nUserNo < 0 || data.nUserNo > SmsBasic.arrUserSetting.Count) // 사용자번호가이상인지...
            {
                if (data.nUserNo == SmsBasic.smsScriptUser.nUserNo)								// 스크립트사용자...
                {
                    SmsBasic.smsScriptUser.phoneNo = data.phoneNo;
                }
                else
                {
                    SmsBasic.arrSmsQueue.Remove(data);
                    return;
                }
            }

            // 수동전송이 아닐경우, 딜레이체크
            if (!data.bManual)
            {
                CheckAndSleep(data.phoneNo);
            }

            eCommStatus retn;
            retn = WriteSmsMessage(data);

            smsUserConfig user;
            if (data.nUserNo == SmsBasic.smsScriptUser.nUserNo) user = SmsBasic.smsScriptUser;
            else user = (smsUserConfig)SmsBasic.arrUserSetting[data.nUserNo];
            user.eConnectStatus = retn;
            if (retn == eCommStatus.COMM_WAITING) return;

            if (retn == eCommStatus.COMM_OK)
            {
                okOrErrorProcess(data, false);
                SmsBasic.arrSmsQueue.Remove(data);
                user.retry_count = 0;
                if (SmsMainDisplayForm.formThis != null && data.nUserNo != SmsBasic.smsScriptUser.nUserNo)	// 사용자표시되고, 스크립트사용자가아니면MDI 화면을다시그린다
                    SmsMainDisplayForm.formThis.CallByThreadReDrawUserInfo(data.nUserNo % 256);
                return;
            }
            if (data.bManual)			// 수동이면서에러
            {
                okOrErrorProcess(data, true);
                SmsBasic.arrSmsQueue.Remove(data);
                user.retry_count = 0;
                if (SmsMainDisplayForm.formThis != null && data.nUserNo != SmsBasic.smsScriptUser.nUserNo)	// 사용자표시되고, 스크립트사용자가아니면MDI 화면을다시그린다
                    SmsMainDisplayForm.formThis.CallByThreadReDrawUserInfo(data.nUserNo % 256);
            }
            else
            {

                user.retry_count++;
                if (SmsMainDisplayForm.formThis != null && data.nUserNo != SmsBasic.smsScriptUser.nUserNo)	// 사용자표시되고, 스크립트사용자가아니면MDI 화면을다시그린다
                    SmsMainDisplayForm.formThis.CallByThreadReDrawUserInfo(data.nUserNo % 256);
                if (user.retry_count >= SmsBasic.smsConfig.nRetryErrorCount)
                {
                    okOrErrorProcess(data, true);
                    SmsBasic.arrSmsQueue.Remove(data);
                    user.retry_count = 0;
                }
            }
        }


		static void okOrErrorProcess(alarmDataMain data, bool bError)
		{
			string			header = "", message = "";
			DateTime		t = DateTime.Now;
			
			SaveCommSendData.getSmsOkErrorMessage(data, t, bError, ref header, ref message);
			SaveCommSendData.saveSendMessageDataToFile(data, t, header, message);
			if(data.bClient)		// 전송결과를 클라이언트에 전송
			{
				SmsServerClientProc.SendSMSDataToClient(data.userName + "," + message, data.dIpAddress, EnumNetworkCommand.SMS_CALL_ERROR);	// 전송결과 전송
			}
		}

		static string getNumOnlyBuf(string telNo)
		{
			if(telNo == null) return "";

			string	buf = "";
			
			for(int i = 0; i < telNo.Length; i++) 
			{
				if(telNo[i] >= '0' && telNo[i] <= '9') buf += telNo[i];				
			}
			return buf;
		}

        public static bool bLimitCountOfDayOver = false;   // 제한을 넘었다.
        static int nLimitCountOfDaySuccessCount = 0;
        static int nLimitCountOfDayOldDay = 0;

        static bool CheckLimitCountOfDay()
        {
            DateTime t = DateTime.Now;
            if (t.Day != nLimitCountOfDayOldDay)
            {
                nLimitCountOfDayOldDay = t.Day;
                bLimitCountOfDayOver = false;
                nLimitCountOfDaySuccessCount = 0;
            }

            if (nLimitCountOfDaySuccessCount >= SmsBasic.smsConfig.nMaxCountOfDay)
            {
                bLimitCountOfDayOver = true;
                return false;
            }

            bLimitCountOfDayOver = false;
            
            return true;
        }
		
        //static eCommStatus WriteSmsMessage(alarmDataMain data)
        //{
        //    if (!CheckLimitCountOfDay()) return eCommStatus.COMM_OK;

        //    string		buf = "";
        //    eCommStatus retn;

        //    if (data.nSmsType == 2)
        //    {
        //        buf = data.data.Trim();
        //        SmsBasic.sLineNotifyErrorCode = "";
        //        if (WebServiceUtil.ExecuteCommand_ToLineNotify(data.phoneNo, data.sendPhoneNo, buf, ref SmsBasic.smsConfig.nWebServiceErrorCode, data.nSmsType))
        //        {
        //            if (SmsBasic.sLineNotifyErrorCode.Length == 0)
        //                retn = eCommStatus.COMM_OK;
        //            else
        //                retn = eCommStatus.COMM_ERROR;
        //        }
        //        else
        //            retn = eCommStatus.COMM_ERROR;

        //    }
        //    else
        //    {
        //        switch (SmsBasic.smsConfig.eConnectType)
        //        {
        //            case SendSMSData.eConectionType.UDP_IP:
        //                buf += data.userName + "," + data.phoneNo + "," + data.sendPhoneNo + "," + data.nSmsType.ToString() + "," + data.data.Trim();
        //                retn = SmsServerClientProc.SendSMSDataToServer(buf, EnumNetworkCommand.SMS_DATA);
        //                break;
        //            case SendSMSData.eConectionType.WEB_SERVICE:
        //                buf = data.data.Trim();
        //                SmsBasic.smsConfig.nWebServiceErrorCode = 0;
        //                if (WebServiceUtil.ExecuteCommand(data.phoneNo, data.sendPhoneNo, buf, ref SmsBasic.smsConfig.nWebServiceErrorCode, data.nSmsType))
        //                {
        //                    if (SmsBasic.smsConfig.nWebServiceErrorCode == 1)
        //                        retn = eCommStatus.COMM_OK;
        //                    else
        //                        retn = eCommStatus.COMM_ERROR;
        //                }
        //                else
        //                    retn = eCommStatus.COMM_ERROR;
        //                break;
        //            case SendSMSData.eConectionType.SERIAL:
        //            default:
        //                if (SmsBasic.smsConfig.eCdma == eCdmaType.LG_MOS)
        //                    makeLgMosSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);
        //                else if (SmsBasic.smsConfig.eCdma == eCdmaType.AIR_NCL)
        //                {
        //                    retn = sendToLocalSmsMessageAirNcl(getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);	// 2006-05-17 추가, 버퍼를 여기서 만들지 않고 makeAirNclSendWriteCode 함수를 이용한다.
        //                    break;
        //                }
        //                else if (SmsBasic.smsConfig.eCdma == eCdmaType.TELIT_BSM860S)
        //                    makeTelitBsm860sSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);// 2007-10-22 추가,
        //                else if (SmsBasic.smsConfig.eCdma == eCdmaType.KTF_MOBICON)
        //                    makeKTFMobiconSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);// 2010-11-18 추가,
        //                else if (SmsBasic.smsConfig.eCdma == eCdmaType.GSM_MODEM)
        //                    makeGSMModemSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);// 2012-04-09 추가,
        //                else if (SmsBasic.smsConfig.eCdma == eCdmaType.EMV_1800K_CDMA)
        //                    makeEMV_1800K_CDMASendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);// 2012-12-18 추가,
        //                else if (SmsBasic.smsConfig.eCdma == eCdmaType.M2M_WM215)
        //                    return sendToLocalSmsMessageM2M_WM215(getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);	// 2016-1-8 추가
        //                else
        //                    makeAnyCallSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);

        //                retn = sendToLocalSmsMessage(buf);
        //                break;
        //        }
        //    }

        //    if (retn == eCommStatus.COMM_OK)
        //    {
        //        nLimitCountOfDaySuccessCount++;
        //    }

        //    return retn;
        //}		


        static eCommStatus WriteSmsMessage(alarmDataMain data)  //20241111 PSU
        {
            if (!CheckLimitCountOfDay()) return eCommStatus.COMM_OK;

            string buf = "";
            eCommStatus retn;

            
            switch (SmsBasic.smsConfig.eConnectType)
            {
                case SendSMSData.eConectionType.UDP_IP:
                    buf += data.userName + "," + data.phoneNo + "," + data.sendPhoneNo + "," + data.nSmsType.ToString() + "," + data.ChatID + "," + data.data.Trim();
                    retn = SmsServerClientProc.SendSMSDataToServer(buf, EnumNetworkCommand.SMS_DATA);
                    break;
                case SendSMSData.eConectionType.WEB_SERVICE:

                    if (data.nSmsType == 2)
                    {
                        buf = data.data.Trim();
                        SmsBasic.sTelegramErrorCode = "";

                        string botToken = data.phoneNo;
                        //string chatId = "-1002494829817";
                        string chatId = data.ChatID;
                        string message = buf;

                        if (WebServiceUtil.ExecuteCommand_ToTelegramBot(botToken, chatId, message, ref SmsBasic.smsConfig.nWebServiceErrorCode, data.nSmsType))
                        {
                            if (SmsBasic.sTelegramErrorCode.Length == 0)
                                retn = eCommStatus.COMM_OK;
                            else
                                retn = eCommStatus.COMM_ERROR;
                        }
                        else
                            retn = eCommStatus.COMM_ERROR;
                        break;
                    }

                    buf = data.data.Trim();
                    SmsBasic.smsConfig.nWebServiceErrorCode = 0;
                    if (WebServiceUtil.ExecuteCommand(data.phoneNo, data.sendPhoneNo, buf, ref SmsBasic.smsConfig.nWebServiceErrorCode, data.nSmsType))
                    {
                        if (SmsBasic.smsConfig.nWebServiceErrorCode == 1)
                            retn = eCommStatus.COMM_OK;
                        else
                            retn = eCommStatus.COMM_ERROR;
                    }
                    else
                        retn = eCommStatus.COMM_ERROR;
                    break;
                case SendSMSData.eConectionType.SERIAL:
                default:
                    if (SmsBasic.smsConfig.eCdma == eCdmaType.LG_MOS)
                        makeLgMosSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);
                    else if (SmsBasic.smsConfig.eCdma == eCdmaType.AIR_NCL)
                    {
                        retn = sendToLocalSmsMessageAirNcl(getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);	// 2006-05-17 추가, 버퍼를 여기서 만들지 않고 makeAirNclSendWriteCode 함수를 이용한다.
                        break;
                    }
                    else if (SmsBasic.smsConfig.eCdma == eCdmaType.TELIT_BSM860S)
                        makeTelitBsm860sSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);// 2007-10-22 추가,
                    else if (SmsBasic.smsConfig.eCdma == eCdmaType.KTF_MOBICON)
                        makeKTFMobiconSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);// 2010-11-18 추가,
                    else if (SmsBasic.smsConfig.eCdma == eCdmaType.GSM_MODEM)
                        makeGSMModemSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);// 2012-04-09 추가,
                    else if (SmsBasic.smsConfig.eCdma == eCdmaType.EMV_1800K_CDMA)
                        makeEMV_1800K_CDMASendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);// 2012-12-18 추가,
                    else if (SmsBasic.smsConfig.eCdma == eCdmaType.M2M_WM215)
                        return sendToLocalSmsMessageM2M_WM215(getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);	// 2016-1-8 추가
                    else
                        makeAnyCallSendDataBuf(ref buf, getNumOnlyBuf(data.phoneNo), getNumOnlyBuf(data.sendPhoneNo), data.data);

                    retn = sendToLocalSmsMessage(buf);
                    break;
            }

            if (retn == eCommStatus.COMM_OK)
            {
                nLimitCountOfDaySuccessCount++;
            }

            return retn;
        }	

		static void makeLgMosSendDataBuf(ref string buf, string telNo, string sendTelNo, string data)
		{
            if (sendTelNo.Length <= 0) buf += string.Format("AT$SMSMO1={0},,4098,", telNo);
            else buf += string.Format("AT$SMSMO1={0},{1},4098,", telNo, sendTelNo);			

			string	imsi;
			if(data.Length > SmsBasic.smsConfig.nMaxSendChar) imsi = data.Substring(0, SmsBasic.smsConfig.nMaxSendChar);
			else imsi = data;
			imsi = imsi.Trim();
			buf += imsi;
		}

        static void makeTelitBsm860sSendDataBuf(ref string buf, string telNo, string sendTelNo, string data)// 2007-10-22 추가,
        {
            if (sendTelNo.Length <= 0) buf += string.Format("AT$SMSMO={0},,4098,", telNo);// 4098 = 메시지 텔레서비스, 4097 = 호출 서비스
            else buf += string.Format("AT$SMSMO={0},{1},4098,", telNo, sendTelNo);
            // 0 = OCTET(8Bits), 2 = ASCII(7Bits), 16 = KOREAN(8Bits) : 아래 첫 인자
            // 0 = not reply option setting,  1 = reply option setting : 아래 두번 째 인자
            if ((int)SmsBasic.smsConfig.eMessageType >= 1 && (int)SmsBasic.smsConfig.eMessageType <= 3)
            {
                buf += string.Format("16,,{0},", ((int)SmsBasic.smsConfig.eMessageType) - 1);// normal = 0, argent = 1, emergency,  삼성기준 normal = 1 ~ 3 이므로 -1 을 한다.
                // ,,{0},을 16,,{0},으로 바꿈   한글이 깨져서 16으로 바꿈 2009.11.3
            }
            else
            {
                buf += string.Format("16,,{0},", 0);// normal = 0, argent = 1, emergency,  삼성기준 normal = 1 ~ 3 이므로 -1 을 한다.
                // ,,{0},을 16,,{0},으로 바꿈   한글이 깨져서 16으로 바꿈 2009.11.3
            }

            string imsi;
            if (data.Length > SmsBasic.smsConfig.nMaxSendChar) imsi = data.Substring(0, SmsBasic.smsConfig.nMaxSendChar);
            else imsi = data;
            imsi = imsi.Trim();

            byte[] data2 = Tools.StringToBytes(imsi);
            int length = data2.Length;
            if(length > SmsBasic.smsConfig.nMaxSendChar) length = SmsBasic.smsConfig.nMaxSendChar;
            for(int i = 0; i < length; i++) buf += string.Format("{0:X00}", (int)data2[i]);
        }

        static void makeKTFMobiconSendDataBuf(ref string buf, string telNo, string sendTelNo, string data)// 2010-11-18 추가,
        {
            buf += string.Format("ASST {0:d02} {1} ", (byte)SmsBasic.smsConfig.cMobiconID, telNo);// slave ID

            string imsi;
            if (data.Length > SmsBasic.smsConfig.nMaxSendChar) imsi = data.Substring(0, SmsBasic.smsConfig.nMaxSendChar);
            else imsi = data;
            imsi = imsi.Trim();
            buf += imsi;

            buf += (char)0x30;//CRC HI
            buf += (char)0x30;//CRC LO
        }

        static void makeGSMModemSendDataBuf(ref string buf, string telNo, string sendTelNo, string data)// 2012-04-09 추가,
        {
            buf += (char)0x07;
            buf += string.Format("AT+CMGS={0}", telNo);
            buf += (char)0x0D;

            string imsi;
            if (data.Length > SmsBasic.smsConfig.nMaxSendChar) imsi = data.Substring(0, SmsBasic.smsConfig.nMaxSendChar);
            else imsi = data;
            imsi = imsi.Trim();
            buf += imsi;
            buf += (char)0x1A;
        }

        static void add11CharPhoneNumberBuf(ref string buf, string telNo)// 2012-12-18 추가,
        {
            
            int length = telNo.Length;
            string      imsi = "";

            if (length >= 11) imsi = telNo.Substring(0, 11);
            else
            {
                for (int i = 0; i < 11; i++)
                {
                    if (i < length) imsi += string.Format("{0:X00}", (char)telNo[i]);
                    else imsi += string.Format(" ");
                }
            }
            buf += imsi;            
        }

        static void makeEMV_1800K_CDMASendDataBuf(ref string buf, string telNo, string sendTelNo, string data)// 2012-12-18 추가,
        {
            string imsi;
            if (data.Length > SmsBasic.smsConfig.nMaxSendChar) imsi = data.Substring(0, SmsBasic.smsConfig.nMaxSendChar);
            else imsi = data;
            imsi = imsi.Trim();

            byte[] byteImsi = Tools.StringToBytes(imsi);
            int length = byteImsi.Length + 23;
            buf += (char)0x07;
            buf += (char)length;
            add11CharPhoneNumberBuf(ref buf, telNo);        // add 11 character telephone number
            add11CharPhoneNumberBuf(ref buf, sendTelNo);    // add 11 character sender telephone number
            buf += imsi;
        }

        static void makeAnyCallSendDataBuf(ref string buf, string telNo, string sendTelNo, string data)
		{
			if(sendTelNo.Length <= 0) buf += string.Format("AT#PSTRM={0},*,{1},", telNo, SmsBasic.smsConfig.nRetryCount);
			else buf += string.Format("AT#PSTRM={0},{1},{2},", telNo, sendTelNo, SmsBasic.smsConfig.nRetryCount);			
			buf += (char)0x22;		// 0x22 = "

			string	imsi;
			if(data.Length > SmsBasic.smsConfig.nMaxSendChar) imsi = data.Substring(0, SmsBasic.smsConfig.nMaxSendChar);
			else imsi = data;
			imsi = imsi.Trim();
			buf += imsi;
			buf += (char)0x22;		// 0x22 = "

			if(SmsBasic.smsConfig.eCdma == eCdmaType.ANY_CALL_CDMA ||
				SmsBasic.smsConfig.eCdma == eCdmaType.ANY_CALL_CDMA2000 ||
				SmsBasic.smsConfig.eCdma == eCdmaType.ANY_CALL_CDMA2000_SPH)
				buf += string.Format(",{0},{1},{2}, 4098", 0, (int)SmsBasic.smsConfig.eMessageType, 0);//configLocal.nCdmaNo);
			if(SmsBasic.smsConfig.eCdma == eCdmaType.ANY_CALL)
				buf += string.Format(",{0},{1},{2}", 0, (int)SmsBasic.smsConfig.eMessageType, 0);
		}



		static eCommStatus sendToLocalSmsMessage(string buf)
		{
			eCommStatus			retn;

            if (SmsBasic.smsConfig.eCdma == eCdmaType.LG_MOS)
                retn = InitSmsMessageLgMos();
            else if (SmsBasic.smsConfig.eCdma == eCdmaType.TELIT_BSM860S)  // 2007-10-22 add
                retn = InitSmsMessageTelitBsm860s();
            else if (SmsBasic.smsConfig.eCdma == eCdmaType.KTF_MOBICON)  // 2010-11-18 add
                retn = eCommStatus.COMM_OK;// 임의로 OK로 설정 먼저 보낼 데이터가 없다
            else if (SmsBasic.smsConfig.eCdma == eCdmaType.GSM_MODEM)   // 2012-04-09 add
                retn = InitSmsMessageGSMModem();
            else if (SmsBasic.smsConfig.eCdma == eCdmaType.EMV_1800K_CDMA)   // 2012-12-18 add
                retn = InitSmsMessageEMV_1800K_CDMA();
            else
                retn = InitSmsMessage();	
			if(retn == eCommStatus.COMM_ERROR) return retn;

			string				imsi;
			if(SmsBasic.smsConfig.eCdma == eCdmaType.ANY_CALL ||	SmsBasic.smsConfig.eCdma == eCdmaType.ANY_CALL_CDMA) 
			{
				imsi = "AT#PSTOP=1";			// old version = 0
				retn = SmsWrite(imsi, eSendCommand.NORMAL, false);
				if(retn == eCommStatus.COMM_ERROR) return retn;
			}
			/*	sprintf(buf, "AT#PCOUT?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;	

				sprintf(buf, "AT#PSSTC?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PSTMC?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;*/

	
			if(SmsBasic.smsConfig.eCdma == eCdmaType.LG_MOS) return SmsWrite(buf, eSendCommand.DATA_SEND_COMMAND, true);//, DATA_SEND_COMMAND);
            else if (SmsBasic.smsConfig.eCdma == eCdmaType.EMV_1800K_CDMA) return SmsWriteEmv1800(buf);// 2012-12-20 add
			else return SmsWrite(buf, eSendCommand.NORMAL, true);
		}

		static eCommStatus sendToLocalSmsMessageAirNcl(string telNo, string sendTelNo, string message)
		{
			byte[]	commSendBuf = new byte[1280];
			int		len;
			len = SendSmsDataAirNcl.makeAirNclSendWriteCode(ref commSendBuf, telNo, sendTelNo, message);
			return SmsWriteByte(commSendBuf, len, eSendCommand.NORMAL, true);
		}

        static eCommStatus sendToLocalSmsMessageM2M_WM215(string telNo, string sendTelNo, string message)
        {
            string commSendBuf = "";

            //buf = "AT+CMGF=0";
            //eCommStatus retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            //if(retn == eCommStatus.COMM_ERROR) return retn;

            int len = SendSmsDataWM215.makeWm215SendWriteCode(ref commSendBuf, telNo, sendTelNo, message);

            string buf = string.Format("AT+CMGS={0}", len - 1);
            buf += (char)0x0D;
            buf += (char)0x0D;
            buf += (char)0x0A;
            SerialPortSetAndOpen.write(buf);
            Thread.Sleep(100);
            commSendBuf += (char)0x1A;									//End Code;
            return SmsWrite(commSendBuf, eSendCommand.NORMAL, true);
        }

		static eCommStatus InitSmsMessage()
		{
			string		buf;
			eCommStatus	retn;
	
			if(SmsBasic.smsConfig.eCdma != eCdmaType.ANY_CALL_CDMA2000) 
			{
				buf =  "at+crm=1";
				//sprintf(buf, "at+crm=135");
				retn = SmsWrite(buf, eSendCommand.NORMAL, false);
				if(retn == eCommStatus.COMM_ERROR) return retn;
			}
			buf = "AT";
			retn = SmsWrite(buf, eSendCommand.NORMAL, false);
			if(retn == eCommStatus.COMM_ERROR) return retn;

			buf = "ATE0V1";
			retn = SmsWrite(buf, eSendCommand.NORMAL, false);
			if(retn == eCommStatus.COMM_ERROR) return retn;

			if(SmsBasic.smsConfig.eCdma != eCdmaType.ANY_CALL_CDMA2000_SPH)
			{
				buf = string.Format("AT+IPR={0,3:d}", SmsBasic.smsConfig.nBaud);
				retn = SmsWrite(buf, eSendCommand.NORMAL, false);
				if(retn == eCommStatus.COMM_ERROR) return retn;
			}

			//sprintf(buf, "AT");	
			//retn = SmsWrite(buf, eSendCommand.NORMAL);
			//if(retn == COMM_ERROR) return retn;

			//sprintf(buf, "AT#PMODE=?");	
			//retn = SmsWrite(buf, eSendCommand.NORMAL);
			//if(retn == COMM_ERROR) return retn;

			buf = "AT#PMODE=1";
			retn = SmsWrite(buf, eSendCommand.NORMAL, false);
			if(retn == eCommStatus.COMM_ERROR) return retn;
			/*
				sprintf(buf, "AT+GMM");	
				retn = SmsWrite(buf, eSendCommand.NORMAL);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PBDYN?");	
				retn = SmsWrite(buf, eSendCommand.NORMAL);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PCOUT?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PMALK?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PMPWD?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PMUNA?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PMSNS?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PMMRS?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PCLNK?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PUMDL=?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PMIDL=?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;

				sprintf(buf, "AT#PCBND?");	
				retn = SmsWrite(pt, buf);
				if(retn == COMM_ERROR) return retn;*/

			return retn;
		}


		static eCommStatus InitSmsMessageLgMos()
		{
			string		buf;
			eCommStatus	retn;
	
			buf = "ATZ";
			retn = SmsWrite(buf, eSendCommand.NORMAL, false);
			if(retn == eCommStatus.COMM_ERROR) return retn;

			buf = "AT+GMR";
			retn = SmsWrite(buf, eSendCommand.NORMAL, false);
			if(retn == eCommStatus.COMM_ERROR) return retn;
			buf = "ATE0";	
			retn = SmsWrite(buf, eSendCommand.NORMAL, false);
			if(retn == eCommStatus.COMM_ERROR) return retn;

			buf = "ATQ1\rAT$PHONENUM?";
			retn = SmsWrite(buf, eSendCommand.AT_PHONENUM_COMMAND, false);
			if(retn == eCommStatus.COMM_ERROR) return retn;

			buf = "AT$PING?";
			retn = SmsWrite(buf, eSendCommand.AT_PING_COMMAND, false);
			if(retn == eCommStatus.COMM_ERROR) return retn;
			return retn;
		}


        static eCommStatus InitSmsMessageTelitBsm860s()
        {
            string buf;
            eCommStatus retn;// = eCommStatus.COMM_OK;

            buf = "ATZ";
            retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            if (retn == eCommStatus.COMM_ERROR) return retn;

            //buf = "AT+GMR";
            //retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            //if (retn == eCommStatus.COMM_ERROR) return retn;
            return retn;
        }

        static eCommStatus InitSmsMessageGSMModem()// 2012-04-09 add
        {
            string buf;
            eCommStatus retn;

            buf = "AT";
            retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            if (retn == eCommStatus.COMM_ERROR) return retn;

            buf = "AT+CMGF=1";
            retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            if (retn == eCommStatus.COMM_ERROR) return retn;

            //buf = "AT+CMGF=1";
            //retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            //if (retn == eCommStatus.COMM_ERROR) return retn;
            return retn;
        }
        

        static eCommStatus InitSmsMessageEMV_1800K_CDMA()// 2012-12-18 add
        {
            string buf;
            eCommStatus retn;

            buf = "AT+STATE";
            retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            if (retn == eCommStatus.COMM_ERROR) return retn;

            buf = "AT+SMSG=1";
            retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            if (retn == eCommStatus.COMM_ERROR) return retn;

            //buf = "AT+SMSS?";
            //retn = SmsWrite(buf, eSendCommand.NORMAL, false);
            //if (retn == eCommStatus.COMM_ERROR) return retn;
            return retn;
        }

        		
		static int PlcDeviceReadContinue(ref string imsi, int count)
		{
			imsi += " ";
			return count;
		}

		public static DialogCommon.CodeViewPush codeViewPush = new CodeViewPush();

		static bool getEndCode(ref string data, ref string buf)
		{
			buf = "";
			if(data.Length < 2) return false;

			for(int i = 0; i < data.Length-1; i++) 
			{
				if(data[i] == (char)0x0D && data[i+1] == (char)0x0A) 
				{
					buf = data.Substring(0, i+2);
					if(data.Length-i-2 <= 0) data = "";
					else					 data = data.Substring(i+2, data.Length-i-2);
					codeViewPush.DisplayRecvNextLine();
					return true;
				}
			}
			return false;
		}
		

		static eCommStatus SmsWrite(string buf, eSendCommand cmd, bool bData)		// bData  -  데이터 = true, 제어코드 등 = false
		{
			//int				val;
			string			imsi = "", data;
			TimeOutClass	timeout = new TimeOutClass();

			buf += (char)0x0D;//CR;
			buf += (char) 0x0A;// LF;
            SerialPortSetAndOpen.write(buf);
            if (cmd == eSendCommand.DATA_SEND_COMMAND && SmsBasic.smsConfig.eCdma == eCdmaType.EMV_1800K_CDMA) return eCommStatus.COMM_OK;    // 2012-12-18 add
            
			timeout.Reset();
			while(true) 
			{
				Thread.Sleep(1);
				if(bSmsCommThreadPause) return eCommStatus.COMM_ERROR;				// thread 정지명령이 들어오면 return
				//if(SmsBasic.smsConfig.eConnectType == eConectionType.SERIAL && SmsBasic.smsConfig.bUseServer) SmsServerCheckRecv();	// SMS서버일 때
				//if(SmsBasic.smsConfig.eConnectType == eConectionType.UDP_IP) SmsServerCheckRecv();								// 클라이언트일 때 ERROR 메시지를 받기위해
				SmsServerClientProc.SmsServerClientDataRecvCheck();					// SMS서버/클라이언트 일때 전송정보, ERROR 메시지를 받기위해
							
				if(bData) 
				{
					if(timeout.IsTimeOut(SmsBasic.smsConfig.nTimeOutMessage)) return eCommStatus.COMM_ERROR;
				}
				else 
				{
					if(timeout.IsTimeOut(SmsBasic.smsConfig.nTimeOutBasic)) return eCommStatus.COMM_ERROR;
				}

                if (SerialPortSetAndOpen.read(ref imsi) == false) continue;
				//if(val == -1) continue;			// open error or not read
				//imsi += (char)val;
				
				if(imsi.Length >= 2)  
				{
					data = "";
					while(getEndCode(ref imsi, ref data))
						//if(imsi[imsi.Length-2] == (char)0x0D && imsi[imsi.Length-1] == (char)0x0A) 
					{
                        if (SmsBasic.smsConfig.eCdma == eCdmaType.TELIT_BSM860S)        // 2007-10-23 add
                        {
                            if (string.Compare(data, 0, "ERROR", 0, 5, true) == 0) return eCommStatus.COMM_ERROR;
                        }
                        else if (SmsBasic.smsConfig.eCdma == eCdmaType.KTF_MOBICON)     // 2010-11-19 add
                        {
                            string bufOkErr = "";
                            bufOkErr = string.Format("ASST {0:d02} OK", (byte)SmsBasic.smsConfig.cMobiconID);// 2010-11-19 add
                            if (string.Compare(data, 0, bufOkErr, 0, 10, true) == 0) return eCommStatus.COMM_OK;// 2010-11-19 add
                            bufOkErr = string.Format("ASST {0:d02} ERROR", (byte)SmsBasic.smsConfig.cMobiconID);// 2010-11-19 add
                            if (string.Compare(data, 0, bufOkErr, 0, 13, true) == 0) return eCommStatus.COMM_ERROR;// 2010-11-19 add                            
                        }
                        else if (SmsBasic.smsConfig.eCdma == eCdmaType.GSM_MODEM || SmsBasic.smsConfig.eCdma == eCdmaType.EMV_1800K_CDMA)     // 2012-04-09 add, 2012-12-20 add = eCdmaType.EMV_1800K_CDMA
                        {
                            if (string.Compare(data, 0, "OK", 0, 2, true) == 0) return eCommStatus.COMM_OK;
                            if (string.Compare(data, 0, "ERROR", 0, 5, true) == 0) return eCommStatus.COMM_ERROR;
                        }                        
                        else
                        {
                            if (string.Compare(data, 0, "ERROR", 0, 5, true) == 0) return eCommStatus.COMM_OK;
                        }
                        
						switch(cmd) 
						{
							case eSendCommand.AT_PHONENUM_COMMAND : 
								if(string.Compare(data, 0, "01", 0, 2, true) == 0) return eCommStatus.COMM_OK;
								break;
							case eSendCommand.AT_PING_COMMAND : 
								return eCommStatus.COMM_OK;
							case eSendCommand.DATA_SEND_COMMAND : 
								if(string.Compare(data, 0, "$SMSMOACK1", 0, 10, true) == 0) return eCommStatus.COMM_OK;
								break;
							default : 
								if(string.Compare(data, 0, "OK", 0, 2, true) == 0) return eCommStatus.COMM_OK;
								break;
						}
						continue;
					}
				}
			}
		}

        static eCommStatus SmsWriteEmv1800(string buf)		// add 2012-12-20
        {
            return SmsWrite(buf, eSendCommand.DATA_SEND_COMMAND, true);

            /*string imsi = "", data, buf2 = "";// 원래 체크해야 하나 5초이상이 지난 후에 체크해야 정상 발신코드가 생성되어서 이 코드를 삽입안함
            TimeOutClass timeout = new TimeOutClass();
            
            Thread.Sleep(5000);
            buf2 = string.Format("AT+SMSS?");               // status request command
            buf2 += (char)0x0D;//CR;
            buf2 += (char)0x0A;// LF;
            SerilaPortSetAndOpen.write(buf2);
            
            timeout.Reset();
            while (true)
            {                
                Thread.Sleep(1);
                if (bSmsCommThreadPause) return eCommStatus.COMM_ERROR;				// thread 정지명령이 들어오면 return
                SmsServerClientProc.SmsServerClientDataRecvCheck();					// SMS서버/클라이언트 일때 전송정보, ERROR 메시지를 받기위해

                if (timeout.IsTimeOut(SmsBasic.smsConfig.nTimeOutBasic)) return eCommStatus.COMM_ERROR;
                if (SerilaPortSetAndOpen.read(ref imsi) == false) continue;
                if (imsi.Length >= 2)
                {
                    data = "";
                    while (getEndCode(ref imsi, ref data))
                    {
                        if (string.Compare(data, 0, "ERROR", 0, 5, true) == 0) return eCommStatus.COMM_ERROR;                        
                        if (string.Compare(data, 0, "+SMSS: 1", 0, 8, true) == 0) return eCommStatus.COMM_OK;
                        continue;
                    }
                }
            }*/
        }

		static eCommStatus SmsWriteByte(byte[] buf, int len, eSendCommand cmd, bool bData)		// bData  -  데이터 = true, 제어코드 등 = false
		{
			string			imsi = "";
			TimeOutClass	timeout = new TimeOutClass();
			eCommStatus		retn;

            SerialPortSetAndOpen.writeByte(buf, len);
			
			timeout.Reset();
			while(true) 
			{
				Thread.Sleep(1);
				if(bSmsCommThreadPause) return eCommStatus.COMM_ERROR;				// thread 정지명령이 들어오면 return
				SmsServerClientProc.SmsServerClientDataRecvCheck();					// SMS서버/클라이언트 일때 전송정보, ERROR 메시지를 받기위해
							
				if(bData) 
				{
					if(timeout.IsTimeOut(SmsBasic.smsConfig.nTimeOutMessage)) return eCommStatus.COMM_ERROR;
				}
				else 
				{
					if(timeout.IsTimeOut(SmsBasic.smsConfig.nTimeOutBasic)) return eCommStatus.COMM_ERROR;
				}
                if (SerialPortSetAndOpen.read(ref imsi) == false) continue;				
				if(imsi.Length >= 8)							// 최소 8 문자 이상
				{
					if(imsi[imsi.Length-1] == (char)EnumAsciiCode.EOT)
					{
						retn = checkAirNclRecvCode(imsi);
						if(retn == eCommStatus.COMM_WAITING) 
						{
							imsi = "";
							continue;
						}
						else return retn;
					}
				}
			}
		}

		static eCommStatus checkAirNclRecvCode(string recvData)
		{
			byte[]	imsiData = new byte[1280];
			byte	val;
			int		pos = 0, currPos = 0, nLen = recvData.Length;
			ushort	tagNo;
			bool	bSave = false;

			while(pos < nLen)
			{
				val = (byte)recvData[pos++];
				if(val == (byte)EnumAsciiCode.SOH) 
				{
					currPos = 0;
					bSave = true;
				}
				else if(val == (byte)EnumAsciiCode.EOT) 
				{
					bSave = false;
					currPos = SendSmsDataAirNcl.EscapeUnprocessing(ref imsiData, currPos);
					//ushort checkSum = SendSmsDataAirNcl.GetCheckSum(imsiData, currPos-2);		// 읽은 bcc 가 맞지 않아서 현재(2006-05-18) 사용안함
					//ushort rCheckSum = (ushort)(imsiData[currPos-2]*256 + imsiData[currPos-1]);
					//if(checkSum != rCheckSum) 
					//{
					//	Tools.bell();									// test Beep
					//	continue;//return eCommStatus.COMM_ERROR;		// BCC 에러, 다음 패킷을 검사한다.
					//}

					tagNo = (ushort)(imsiData[3]*256 + imsiData[4]);
					if(imsiData[0] == 'C' || imsiData[0] == 'c') 
					{
						if((tagNo >= 0x0010) && (tagNo != 0x00FF)) 
						{
							if(imsiData[5] == '1') return eCommStatus.COMM_OK;
							else				   return eCommStatus.COMM_ERROR;
						}
					}
				}
				else 
				{
					if(bSave) imsiData[currPos++] = val;
				}
			}
			return eCommStatus.COMM_WAITING;
		}
		
		

		



		
	}
}

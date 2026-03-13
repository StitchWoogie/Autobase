using System;
using System.IO;
using SMS.SmsFunc;
using SMS.Display;
using System.Net;
using System.Net.Sockets;
using NetTools;
using AutoLib;
using AutoLibLocal;
using SMS.SmsUtil;

namespace SMS.SmsComm
{
	/// <summary>
	/// Summary description for SaveCommSendData.
	/// </summary>
	public class SaveCommSendData
	{
		public SaveCommSendData()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void getSmsOkErrorMessage(alarmDataMain data, DateTime t, bool bError, ref string header, ref string message)
		{
			if(data.bClient == false)
				header = string.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}>{3}>{4}>", t.Hour, t.Minute, t.Second, data.userName, data.phoneNo);
			else
				header = string.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}>{3}>{4}>", t.Hour, t.Minute, t.Second, data.dIpAddress + "-" + data.userName, data.phoneNo);

            if (SendSMSData.bLimitCountOfDayOver)
            {
                if (Tools.IsLangKorean())
                {
                    message += String.Format("하루 최대 전송량({0}) 초과", SmsBasic.smsConfig.nMaxCountOfDay);
                }
                else
                {
                    message += String.Format("Over Max counts({0}) of day", SmsBasic.smsConfig.nMaxCountOfDay);
                }
            }
            else
            {
                if (data.bManual)
                    getCommManualSavingMessage(ref message, data.data, bError, data.nSmsType);
                else if (bError)
                    getCommErrorSavingMessage(ref message, data.nSmsType);
                else
                    getCommOKSavingMessage(ref message, data.nSmsType);
            }

			message += " Data = " + data.data.Trim();
		}

		static public void saveSendMessageDataToFile(alarmDataMain data, DateTime t, string header, string message)
		{
			string			path = TotalConfig.GetProjectDataDirectory() + "\\smsx";

			if(Directory.Exists(path) == false) Directory.CreateDirectory(path);
			if(Directory.Exists(path) == false) return;	// 폴더를 만들지 못했다.

			bool			bCreate;
			Stream			fs;
			
			path += string.Format("\\{0,4:d04}{1,2:d02}{2,2:d02}.smsx", t.Year, t.Month, t.Day);
			if(File.Exists(path)) 
			{
				fs = File.Open(path, FileMode.Append);
				bCreate = false;
			}

			else 
			{
				fs = File.Open(path, FileMode.Create);
				bCreate = true;
			}
			if(fs == null) return;

			TextWriter				writer = new StreamWriter(fs);
			writer.WriteLine("{0}", header + message);
			writer.Close();
			if(SmsSendListDisplayForm.formThis == null) return;
			if(bCreate)
			{
				SmsFileDataLoadSave.LoadSendSmsFileName();					// 자료파일을 새로 읽는다
				SmsSendListDisplayForm.formThis.CallByThreadAllDateSendListDisplay();	// 모든 데이터를 갱신한다
			}
				//else SmsSendListDisplayForm.formThis.oneDateSendListDisplay();	// 선택된 하나의 자료만 갱신한다
			else 
			{
				string	userName;
				if(data.bClient) userName = data.dIpAddress + "-" + data.userName;
				else userName = data.userName;
				SmsSendListDisplayForm.formThis.addOneDateSendList(t, userName, data.phoneNo, message);	// 선택된 하나의 자료만 갱신한다
			}
		}

		static void getCommManualSavingMessage(ref string buf, string data, bool bError, int sms_type)
		{
            //if (sms_type == 2)
            //{
            //    if (bError)
            //    {
            //        if (Tools.IsLangKorean()) buf += String.Format("LINE Notify로 테스트 메시지 전송실패.(ErrorCode={0})", SmsBasic.sLineNotifyErrorCode);
            //        else buf += String.Format("Test SMS Data Send error To Line Notify.(ErrorCode={0})", SmsBasic.sLineNotifyErrorCode);
            //    }
            //    else
            //    {
            //        if (Tools.IsLangKorean()) buf += "Line Notify로 테스트 메시지 전송완료.";
            //        else buf += "Test SMS Data Send To Line Notify.";
            //    }

            //    return;
            //}

            //20241111 PSU 텔레그램
            if (sms_type == 2)
            {
                if (bError)
                {
                    switch (SmsBasic.smsConfig.eConnectType)
                    {
                        case SendSMSData.eConectionType.UDP_IP:
                            if (Tools.IsLangKorean()) buf += "서버로 Telegram 테스트 메시지 전송실패.";
                            else buf += "Test Telegram Data Send Error To Server.";
                            break;

                        default:
                            if (Tools.IsLangKorean()) buf += String.Format("Telegram으로 테스트 메시지 전송실패.(ErrorCode={0}.)", SmsBasic.sTelegramErrorCode);
                            else buf += String.Format("Test Telegram Data Send error To Telegram.(ErrorCode={0}.)", SmsBasic.sTelegramErrorCode);
                            break;
                    }

                }
                else
                {
                    switch (SmsBasic.smsConfig.eConnectType)
                    {
                        case SendSMSData.eConectionType.UDP_IP:
                            if (Tools.IsLangKorean()) buf += "서버로 Telegram 테스트 메시지 전송완료.";
                            else buf += "Test Telegram Data Send To Server.";
                            break;

                        default:
                            if (Tools.IsLangKorean()) buf += "Telegram으로 테스트 메시지 전송완료.";
                            else buf += "Test Telegram Data Send To Telegram.";
                            break;
                    }
                }

                return;
            }

			if(bError) 
			{
				switch(SmsBasic.smsConfig.eConnectType) 
				{
					case SendSMSData.eConectionType.UDP_IP : 
						if(Tools.IsLangKorean()) buf += "서버로 테스트 메시지 전송실패.";
						else					 buf += "Test Sms Data Send Error To Server.";
						break;
					case SendSMSData.eConectionType.WEB_SERVICE : 
						if(Tools.IsLangKorean()) buf += String.Format("웹서비스로 테스트 메시지 전송실패.(ErrorCode={0})", SmsBasic.smsConfig.nWebServiceErrorCode);
                        else buf += String.Format("Test SMS Data Send error To Web Service.(ErrorCode={0})", SmsBasic.smsConfig.nWebServiceErrorCode);
					
						break;
					default : 
						if(Tools.IsLangKorean()) buf += "테스트 문자메시지 전송실패.";
						else					 buf += "Text SMS Data Send error.";
						break;
				}
			}
			else 
			{
				switch(SmsBasic.smsConfig.eConnectType) 
				{
					case SendSMSData.eConectionType.UDP_IP : 
						if(Tools.IsLangKorean()) buf += "서버로 테스트 메시지 전송완료.";
						else					 buf += "Test Sms Data Send To Server.";
						break;

					case SendSMSData.eConectionType.WEB_SERVICE : 
						if(Tools.IsLangKorean()) buf += "웹서비스로 테스트 메시지 전송완료.";
						else					 buf += "Test SMS Data Send To Web Service.";
                        break;

					default : 
						if(Tools.IsLangKorean()) buf += "테스트 문자메시지 전송완료.";
						else					 buf += "Test SMS Data Send.";
						break;
				}
			}
		}

		static void getCommErrorSavingMessage(ref string buf, int sms_type)
		{
            //if (sms_type == 2)
            //{
            //    if (Tools.IsLangKorean())
            //        buf += string.Format("Line Notify 이상({0}회 연결)으로 메시지 전송못함.", SmsBasic.smsConfig.nRetryErrorCount);
            //    else
            //        buf += string.Format("Line Notify Communication Error({0} Retry).", SmsBasic.smsConfig.nRetryErrorCount);

            //    buf += string.Format("(ErrorCode={0})", SmsBasic.sLineNotifyErrorCode);

            //    return;
            //}

            //20241111 PSU 텔레그램
            if (sms_type == 2)
            {
                if (Tools.IsLangKorean())
                    buf += string.Format("Telegram 이상({0}회 연결) 메시지 전송못함.", SmsBasic.smsConfig.nRetryErrorCount);
                else
                    buf += string.Format("Telegram Communication Error({0} Retry).", SmsBasic.smsConfig.nRetryErrorCount);

                buf += string.Format("(ErrorCode={0})", SmsBasic.sTelegramErrorCode);

                return;
            }

			switch(SmsBasic.smsConfig.eConnectType) 
			{
				case SendSMSData.eConectionType.UDP_IP : 
					if(Tools.IsLangKorean())
						buf += string.Format("Network 선로이상({0}회 연결)으로 메시지 전송못함.", SmsBasic.smsConfig.nRetryErrorCount);
					else
						buf += string.Format("Network Communication Error({0} Retry).", SmsBasic.smsConfig.nRetryErrorCount);
					break;
				case SendSMSData.eConectionType.WEB_SERVICE : 
					if(Tools.IsLangKorean())
						buf += string.Format("웹서비스 이상({0}회 연결)으로 메시지 전송못함.", SmsBasic.smsConfig.nRetryErrorCount);
					else
						buf += string.Format("Web Service Communication Error({0} Retry).", SmsBasic.smsConfig.nRetryErrorCount);

					if(SmsBasic.smsConfig.nWebServiceErrorCode == 2) 
						buf += string.Format("Socket Create Error ({0})", SmsBasic.smsConfig.nWebServiceErrorCode);
					else if(SmsBasic.smsConfig.nWebServiceErrorCode == 3) 
						buf += string.Format("Socket Connect Error ({0})", SmsBasic.smsConfig.nWebServiceErrorCode);
					else if(SmsBasic.smsConfig.nWebServiceErrorCode == 5) 
						buf += string.Format("Recv timeouted ({0})", SmsBasic.smsConfig.nWebServiceErrorCode);
					else if(SmsBasic.smsConfig.nWebServiceErrorCode == 100) 
						buf += string.Format("Username or password invalid ({0})", SmsBasic.smsConfig.nWebServiceErrorCode);
					else if(SmsBasic.smsConfig.nWebServiceErrorCode == 101) 
						buf += string.Format("Cash insufficent ({0})", SmsBasic.smsConfig.nWebServiceErrorCode);
                    else if (SmsBasic.smsConfig.nWebServiceErrorCode == 102)
                        buf += string.Format("Invalid Telephone number ({0})", SmsBasic.smsConfig.nWebServiceErrorCode);
                    else if (SmsBasic.smsConfig.nWebServiceErrorCode == 103)    // 같은 전화에 너무 많은 메시지 전송 (10개 이상 밀려있음)
                        buf += string.Format("Too many message ({0})", SmsBasic.smsConfig.nWebServiceErrorCode);
                    else if (SmsBasic.smsConfig.nWebServiceErrorCode == 104)    // 발신번호 형식이 다르거나 발신번호가 등록되지 않았을 때
                        buf += string.Format("Caller ID Invalid ({0})", SmsBasic.smsConfig.nWebServiceErrorCode);
					else
						buf +=string.Format("(ErrorCode={0})", SmsBasic.smsConfig.nWebServiceErrorCode);					
					break;
				default : 
					if(Tools.IsLangKorean())
						buf += string.Format("선로이상({0}회 연결)으로 메시지 전송못함.", SmsBasic.smsConfig.nRetryErrorCount);
					else
						buf += string.Format("Line Error({0} Retry).", SmsBasic.smsConfig.nRetryErrorCount);
					break;
			}
		}

        static void getCommOKSavingMessage(ref string buf, int sms_type)
		{
            //if (sms_type == 2)
            //{
            //    if (Tools.IsLangKorean()) buf += "Line Notify로 메시지 전송완료.";
            //    else buf += "SMS Data Send To Line Notify.";

            //    return;
            //}

            //20241111 PSU 텔레그램
            if (sms_type == 2)
            {
                switch (SmsBasic.smsConfig.eConnectType)
                {
                    case SendSMSData.eConectionType.UDP_IP:
                        if (Tools.IsLangKorean()) buf += "서버로 Telegram 메시지 전송완료.";
                        else buf += "Telegram Data Send To Server.";
                        break;

                    default:
                        if (Tools.IsLangKorean()) buf += "Telegram으로 메시지 전송완료.";
                        else buf += "SMS Data Send To Telegram.";
                        break;
                }

                return;
            }

			switch(SmsBasic.smsConfig.eConnectType) 
			{
				case SendSMSData.eConectionType.UDP_IP : 
					if(Tools.IsLangKorean()) buf += "서버로 메시지 전송완료.";
					else					 buf += "Sms Data Send To Server.";
					break;
				case SendSMSData.eConectionType.WEB_SERVICE : 
					if(Tools.IsLangKorean()) buf += "웹서비스로 메시지 전송완료.";
					else					 buf += "SMS Data Send To Web Service.";
					
					break;
				default : 
					if(Tools.IsLangKorean()) buf += "문자메시지 전송완료.";
					else					 buf += "SMS Data Send.";
					break;
			}
		}

		static void makeSendErrorBufFromServer(ref string buf)
		{
			if(Tools.IsLangKorean()) buf += "SMS서버에서 메시지 전송못함.";
			else					 buf += "Send Message Error form Server.";
		}

		static void getClientUserAndData(string buf, ref string userName, ref string data)
		{
			CommaBlockString	comma = new CommaBlockString();
			comma.Set(buf);
			string			imsi = "";

			comma.GetString(ref imsi);
			userName = imsi;
			if(comma.IsEOS()) return;

			comma.GetStringTotalRemain(ref imsi);
			data = imsi;
		}

		public static void SaveErrorCallBackFromServer(string data, string ipString)
		{
			string			path = TotalConfig.GetProjectDataDirectory() + "\\smsx";

			if(Directory.Exists(path) == false) Directory.CreateDirectory(path);
			if(Directory.Exists(path) == false) return;	// 폴더를 만들지 못했다.

			string 			buf;
			bool			bCreate;
			DateTime		t = DateTime.Now;
			Stream			fs;
			
			path += string.Format("\\{0,4:d04}{1,2:d02}{2,2:d02}.smsx", t.Year, t.Month, t.Day);
			if(File.Exists(path)) 
			{
				fs = File.Open(path, FileMode.Append);
				bCreate = false;
			}
			else 
			{
				fs = File.Open(path, FileMode.Create);
				bCreate = true;
			}
			if(fs == null) return;

			string		userName = "", message = "";
			getClientUserAndData(data, ref userName, ref message);
			buf = string.Format("{0,2:d02}:{1,2:d02}:{2,2:d02}>", t.Hour, t.Minute, t.Second);
			buf += string.Format("{0}>IP={1}>", userName, ipString);
			buf += message;
			
			TextWriter				writer = new StreamWriter(fs);
			writer.WriteLine("{0}", buf);
			writer.Close();
			if(SmsSendListDisplayForm.formThis == null) return;
			if(bCreate)
			{
				SmsFileDataLoadSave.LoadSendSmsFileName();					// 자료파일을 새로 읽는다
				SmsSendListDisplayForm.formThis.CallByThreadAllDateSendListDisplay();	// 모든 데이터를 갱신한다
			}				
			else 
			{
				SmsSendListDisplayForm.formThis.addOneDateSendList(t, userName, "IP="+ipString, message);	// 선택된 하나의 자료만 갱신한다
			}
		}



	



	}
}

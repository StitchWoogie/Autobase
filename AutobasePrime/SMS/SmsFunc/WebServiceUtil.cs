using System;
using System.Net;
using System.Security.Authentication;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Text;
using AutoLibLocal;
using NetTools;
using System.Collections.Specialized;
using System.IO;

namespace SMS.SmsFunc
{
    /*
	public class SocketThreadClass 
	{
		public Thread thread;
		public Socket socket;
	}*/

	public struct COMM_STATUS
	{
		public int nContinueFail;	// 연속 실패 횟수
		public int nCountTotal;		// 총 통신 횟수
		public int nCountFail;		// 총 실패 횟수
	}

	/// <summary>
	/// Summary description for ShareServerMain.
	/// </summary>
	public class WebServiceUtil
	{
		public static string sSiteNamePrimary; 
		public static string sSiteNameSecondary; 
		public static string sUserName;
		public static byte[] aPassCode = new byte[1];
		public static COMM_STATUS[] commStatus = new COMM_STATUS[2];

        static WebServiceUtil()
		{
			//
			// TODO: Add constructor logic here
			//
			LoadConfig();
		}

		public static void LoadConfig()
		{
			sSiteNamePrimary = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "SiteNamePrimary", "www.sms.autobase.biz");
			sSiteNameSecondary = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "SiteNameSecondary", "www.sms2.autobase.biz");
			sUserName = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "Username", "");
			aPassCode = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "PassCode", aPassCode);			
		}

		static bool SendCommand(string sms_recv, string sms_send, string sms_msg, string site, ref COMM_STATUS status, ref int retn, int sms_type)
		{
			status.nCountTotal++;			
			biz.autobase.sms.www.ServiceSend2 send = new biz.autobase.sms.www.ServiceSend2(site);
			
			try 
			{
				retn = send.SendByAutoBase(sUserName, aPassCode, sms_recv, sms_send, sms_msg, sms_type);
			}
			catch 
			{
				status.nContinueFail++;
				status.nCountFail++;
				return false;
			}

			status.nContinueFail = 0;
			return true;
		}

		public static bool ExecuteCommand(string sms_recv, string sms_send, string sms_msg, ref int retn, int sms_type)
		{
			if(commStatus[1].nContinueFail < commStatus[0].nContinueFail)
			{
				if(SendCommand(sms_recv, sms_send, sms_msg, sSiteNameSecondary, ref commStatus[1], ref retn, sms_type))
					return true;
				if(SendCommand(sms_recv, sms_send, sms_msg, sSiteNamePrimary, ref commStatus[0], ref retn, sms_type))	
					return true;
			}
			else 
			{
				if(SendCommand(sms_recv, sms_send, sms_msg, sSiteNamePrimary, ref commStatus[0], ref retn, sms_type))	
					return true;
				if(SendCommand(sms_recv, sms_send, sms_msg, sSiteNameSecondary, ref commStatus[1], ref retn, sms_type))
					return true;
			}
			return false;
		}

        public const SslProtocols _Tls11_12 = (SslProtocols)0x00000F00;//(SslProtocols)0x00000C00=tls12 0x300 = tls11 0x3000=tls13;    // tls 13은 윈도우즈 7에서 안된다. tls12만하면 7에서는 되는데 10에서는 안된다.
        public const SecurityProtocolType Tls11_12 = (SecurityProtocolType)_Tls11_12;


        //public static bool ExecuteCommand_ToLineNotify(string sms_recv, string sms_send, string sms_msg, ref int retn, int sms_type)
        //{
        //    SecurityProtocolType save = ServicePointManager.SecurityProtocol;

        //    // 여기가 오류가 나는 경우가 있다.
        //    try
        //    {
        //        ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | Tls11_12;    // Line의 TLS버전이 변경되면서 2022-8부터 이것을 적용해야 전송이 된다. 2022-8-17추가
        //        // 이전버전은 [HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\.NETFramework\v2.0.50727] [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v2.0.50727] 
        //        // 에 "SystemDefaultTlsVersions"=dword:00000001 "SchUseStrongCrypto"=dword:00000001 두항목을 추가하면 된다.
        //    }
        //    catch
        //    {
        //    }

        //    try
        //    {
        //        WebClient wc4 = new WebClient();

        //        string targetAddress4 = "https://notify-api.line.me/api/notify";
        //        wc4.Headers["Authorization"] = "Bearer " + sms_recv;

        //        NameValueCollection nc4 = new NameValueCollection();
        //        nc4["message"] = sms_msg;

        //        byte[] bResult4 = wc4.UploadValues(targetAddress4, nc4);
        //        string result4 = Encoding.UTF8.GetString(bResult4);

        //        ServicePointManager.SecurityProtocol = save;

        //        return true;

        //        //Display.WriteLineOK("LineSms Msg={0}, result={1}", msg, result4);
        //    }
        //    catch (Exception exception)
        //    {
        //        SmsBasic.sLineNotifyErrorCode = exception.Message;

        //        ServicePointManager.SecurityProtocol = save;

        //        return false;
        //    }
        //}


        //20241111 PSU 텔레그램 봇 API
        public static bool ExecuteCommand_ToTelegramBot(string botToken, string chatId, string message, ref int retn, int sms_type)
        {
            SecurityProtocolType save = ServicePointManager.SecurityProtocol;
            try
            {
                // TLS 1.2 설정
                try
                {
                    ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | Tls11_12;    // Line의 TLS버전이 변경되면서 2022-8부터 이것을 적용해야 전송이 된다. 2022-8-17추가
                    // 이전버전은 [HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\.NETFramework\v2.0.50727] [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v2.0.50727] 
                    // 에 "SystemDefaultTlsVersions"=dword:00000001 "SchUseStrongCrypto"=dword:00000001 두항목을 추가하면 된다.
                }
                catch
                {
                }

                using (WebClient wc = new WebClient())
                {
                    string targetAddress = string.Format("https://api.telegram.org/bot{0}/sendMessage", botToken);
                    NameValueCollection parameters = new NameValueCollection();
                    parameters["chat_id"] = chatId;
                    parameters["text"] = message;
                    parameters["parse_mode"] = "";

                    try
                    {
                        byte[] response = wc.UploadValues(targetAddress, parameters);
                        string result = Encoding.UTF8.GetString(response);

                        if (result.Contains("\"ok\":true"))
                        {
                            return true;
                        }
                        return false;
                    }

                    catch (WebException ex)
                    {
                        string errorMessage = "";

                        // WebException의 응답 확인
                        if (ex.Response != null)
                        {
                            try
                            {
                                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                                {
                                    string result = reader.ReadToEnd();
                                    int errorCode = 0;

                                    // 에러 코드 파싱
                                    int startIndex = result.IndexOf("\"error_code\":");
                                    if (startIndex > -1)
                                    {
                                        startIndex += 13;
                                        int endIndex = result.IndexOf(",", startIndex);
                                        if (endIndex > -1)
                                        {
                                            string errorCodeStr = result.Substring(startIndex, endIndex - startIndex);
                                            errorCode = int.Parse(errorCodeStr);
                                        }
                                    }

                                    // 에러 설명 파싱
                                    string errorDesc = "";

                                    startIndex = result.IndexOf("\"description\":\"");
                                    if (startIndex > -1)
                                    {
                                        startIndex += 15;  // "description":" 길이
                                        int endIndex = result.IndexOf("\"", startIndex);
                                        

                                        if (endIndex > startIndex)
                                        {
                                            errorDesc = result.Substring(startIndex, endIndex - startIndex);
                                            errorDesc = errorDesc.Replace("\"", "");
                                        }
                                    }

                                    errorMessage = errorCode + ", " + errorDesc;

                                }
                            }
                            catch
                            {
                                // 응답 파싱 실패 시 기본 WebException 메시지 사용
                                errorMessage = ex.Message;
                            }
                        }
                        else
                        {
                            errorMessage =  ex.Message;
                        }

                        SmsBasic.sTelegramErrorCode = errorMessage;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        SmsBasic.sTelegramErrorCode = ex.Message;
                        return false;
                    }
                }
            }
            finally
            {
                // 원래의 SecurityProtocol 설정으로 복구
                ServicePointManager.SecurityProtocol = save;
            }
        }

        //// 이것도 같은 현상 - 기본 연결이 닫혔습니다. 보내기에서 예기치 않은 오류가 발생했습니다
        //public static bool ExecuteCommand_ToLineNotify(string sms_recv, string sms_send, string sms_msg, ref int retn, int sms_type)
        //{
        //    try
        //    {
        //        var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
        //        var postData = string.Format("message={0}", sms_msg);
        //        var data = Encoding.UTF8.GetBytes(postData);

        //        request.Method = "POST";
        //        request.ContentType = "application/x-www-form-urlencoded";
        //        request.ContentLength = data.Length;
        //        request.Headers.Add("Authorization", "Bearer " + sms_recv);

        //        using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
        //        var response = (HttpWebResponse)request.GetResponse();
        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        //        //Display.WriteLineOK("LineSms Msg={0}, result={1}", msg, result4);

        //        return true;
        //    }
        //    catch (Exception exception)
        //    {
        //        SmsBasic.sLineNotifyErrorCode = exception.Message;

        //        return false;
        //    }
        //}

        //public static bool ExecuteCommand_ToLineNotify(string sms_recv, string sms_send, string sms_msg, ref int retn, int sms_type)
        //{
        //    try
        //    {
        //        //Step0: regist LINT NOTIFIY from : https://notify-bot.line.me


        //        //Step 1 : 
        //        //Tranfer user to your APP Notification to Allow something : 
        //        //YOUR_CLIENT_ID from Step 0
        //        //YOUR_CALLBACK_URL : after user allow.
        //        //https://notify-bot.line.me/oauth/authorize?response_type=code&client_id=YOUR_CLIENT_ID&redirect_uri=YOUR_CALLBACK_URL&scope=notify&state=state


        //        //Step 2 : 
        //        //After your user authorize LINE will regirect to your redirect_uri from Step1
        //        //in your redirect_uri , you will get querystring value named "code" 
        //        //like thie : http://localhost:62670/risiv.aspx?code=aTONuztevWAOtgDzu8qzc9xxxx&state=checksum


        //        //Step3 : 
        //        //Get Token by code.
        //        //https://notify-bot.line.me/oauth/token

        //        ////sample code
        //        //WebClient wc = new WebClient();
        //        //string targetAddress = "https://notify-bot.line.me/oauth/token";
        //        //wc.Encoding = Encoding.UTF8;
        //        //wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        //        //NameValueCollection nc = new NameValueCollection();
        //        //nc["grant_type"] = "authorization_code";
        //        //nc["code"] = "aTONuztevWAOtgDzu8qzc9xxxx"; //the value from Step 2
        //        //nc["redirect_uri"] = "http://localhost:62670/risiv.aspx"; // the url your fill in Step 1
        //        //nc["client_id"] = "2pfCLBdsaddkILdcXea9UR7q"; // get from https://notify-bot.line.me
        //        //nc["client_secret"] = "ML0uxbg6IGenrhpOFhhHVDze6OnP063Pju6aDkdcbgu"; //get from https://notify-bot.line.me
        //        //byte[] bResult = wc.UploadValues(targetAddress, nc);
        //        //string result = Encoding.UTF8.GetString(bResult);
        //        ////Response.Write(result);

        //        //LINE server response like this : 
        //        //{"status":200,"message":"access_token is issued","access_token":"v8kF7s1qfDyOexIgQIhgUmN6qO56zOAcbOB0EjogWN1M"}


        //        //Step 4 : 
        //        //Send Message to User by token
        //        WebClient wc4 = new WebClient();
        //        string targetAddress4 = "https://notify-api.line.me/api/notify";
        //        //wc4.Encoding = Encoding.UTF8;
        //        wc4.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        //        wc4.Headers["Authorization"] = "Bearer " + sms_recv;
        //        //Bearer TOKEN_FROM_STEP3
        //        NameValueCollection nc4 = new NameValueCollection();
        //        nc4["message"] = sms_msg;

        //        byte[] bResult4 = wc4.UploadValues(targetAddress4, nc4);
        //        string result4 = Encoding.UTF8.GetString(bResult4);
        //        //Response.Write(result4);
        //        //Display.WriteLineOK("LineSms Msg={0}, result={1}", msg, result4);
        //        return true;
        //    }
        //    catch (Exception exception)
        //    {
        //        SmsBasic.sLineNotifyErrorCode = exception.Message;

        //        return false;
        //    }
        //}

	}
}

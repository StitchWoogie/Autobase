using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;

namespace WebTools
{
    public class ServiceUsername
    {
        System.ServiceModel.EndpointAddress GetEndPoint(string service_name, int site)
        {
            string url;

            if (site == 2)  // 보조 사이트
            {
                url = String.Format("http://www.username.kr/Service/{0}", service_name);
            }
            else // 메인 사이트
            {
                url = String.Format("http://www.username.co.kr/Service/{0}", service_name);
            }

            return new System.ServiceModel.EndpointAddress(url);
        }

        // ServiceReferenceUsername.ServiceUsername2SoapClient service = new WebTools.ServiceReferenceUsername.ServiceUsername2SoapClient(); 식으로 하면 Endpoint를 찾지 못한다.
        // 어차피 Web.Config 에서 변경할 수 있는것이 아니므로 수동으로 EndPoint를 지정해 주었다.
        ServiceReferenceUsername.ServiceUsername2SoapClient GetServiceUsername(int site)
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;

            ServiceReferenceUsername.ServiceUsername2SoapClient service = new ServiceReferenceUsername.ServiceUsername2SoapClient(binding, GetEndPoint("ServiceUsername2.asmx", site));

            return service;
        }

        const string sUsernameSiteID = "abFkik08uyYuejd098jJ";  // www.autobase.biz site ID

        public bool GetInformation(string username, string item, out string item_value, out string err_msg)
        {
            string seed = StringHash.MakeRandomSeed();
            string siteid_hash = StringHash.Encode(sUsernameSiteID, seed);
            string username_hash = StringHash.Encode(username, seed);
            string infoitem_hash = StringHash.Encode(item, seed);

            byte[] hash = HashTool.MakeHash(siteid_hash + username_hash + infoitem_hash + seed + "UserInfo");

            ServiceReferenceUsername.ServiceUsername2SoapClient service; // 속성이 app.config에 있는데도 에러나서 강제로 주소를 주었다.  

            try
            {
                service = GetServiceUsername(1);
                return service.GetUserInformation(siteid_hash, username_hash, infoitem_hash, hash, seed, out item_value, out err_msg);
            }
            catch
            {
                // 실패하면 보조 사이트를 연결한다.
                service = GetServiceUsername(2);
                return service.GetUserInformation(siteid_hash, username_hash, infoitem_hash, hash, seed, out item_value, out err_msg);
            }
        }

        public bool LogIn(string username, byte[] passcode, out string err_msg, bool bTrySecondary)
        {
            string seed = StringHash.MakeRandomSeed();
            string siteid_hash = StringHash.Encode(sUsernameSiteID, seed);
            string username_hash = StringHash.Encode(username, seed);
            byte[] hash = HashTool.MakeHash(siteid_hash + username_hash + seed + "LogIn");

            ServiceReferenceUsername.ServiceUsername2SoapClient service; // 속성이 app.config에 있는데도 에러나서 강제로 주소를 주었다.

            // 보조 서버부터 먼저 접속한다.
            if (bTrySecondary)
            {
                try
                {
                    service = GetServiceUsername(2);
                    if(service.LogIn(siteid_hash, username_hash, passcode, "ko", hash, seed, out err_msg))  return true;
                }
                catch
                {
                        
                }

                // 실패하면 보조 사이트를 연결한다.
                service = GetServiceUsername(1);
                return service.LogIn(siteid_hash, username_hash, passcode, "ko", hash, seed, out err_msg);
            }
            else
            {
                try
                {
                    service = GetServiceUsername(1);
                    if(service.LogIn(siteid_hash, username_hash, passcode, "ko", hash, seed, out err_msg))  return true;
                }
                catch
                {
                    
                }

                // 실패하면 보조 사이트를 연결한다.
                service = GetServiceUsername(2);
                return service.LogIn(siteid_hash, username_hash, passcode, "ko", hash, seed, out err_msg);
            }
        }

        // Username 이 Hash 되었다. 앞에서 hash되어 들어오면 시간을 아끼기 위해 hash된 것을 보내준다.
        public bool LogInByHashedUsername(string username_hash, byte[] passcode, string ui_culture, string seed, out string err_msg, bool bTrySecondary)
        {
            string siteid_hash = StringHash.Encode(sUsernameSiteID, seed);
            byte[] hash = HashTool.MakeHash(siteid_hash + username_hash + seed + "LogIn");

            ServiceReferenceUsername.ServiceUsername2SoapClient service; // 속성이 app.config에 있는데도 에러나서 강제로 주소를 주었다.

            // 보조 서버부터 먼저 접속한다.
            if (bTrySecondary)
            {
                try
                {
                    service = GetServiceUsername(2);
                    if(service.LogIn(siteid_hash, username_hash, passcode, ui_culture, hash, seed, out err_msg))    return true;
                }
                catch
                {
                    
                }

                // 실패하면 보조 사이트를 연결한다.
                service = GetServiceUsername(1);
                return service.LogIn(siteid_hash, username_hash, passcode, ui_culture, hash, seed, out err_msg);
            }
            else
            {
                try
                {
                    service = GetServiceUsername(1);
                    if(service.LogIn(siteid_hash, username_hash, passcode, ui_culture, hash, seed, out err_msg))    return true;
                }
                catch
                {
                    
                }

                // 실패하면 보조 사이트를 연결한다.
                service = GetServiceUsername(2);
                return service.LogIn(siteid_hash, username_hash, passcode, ui_culture, hash, seed, out err_msg);
            }
        }

    }
}

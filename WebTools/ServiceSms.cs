using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;

namespace WebTools
{
    public class ServiceSms
    {
        System.ServiceModel.EndpointAddress GetEndPoint(string service_name, int server)
        {
            string url;
            
            if(server == 2)
                url = String.Format("http://www.sms2.autobase.kr/Protocol/{0}", service_name);
            else
                url = String.Format("http://www.sms.autobase.kr/Protocol/{0}", service_name);

            return new System.ServiceModel.EndpointAddress(url);
        }

        // ServiceReferenceSmsServiceSend3.ServiceSend3SoapClient service2 = new WebTools.ServiceReferenceSmsServiceSend3.ServiceSend3SoapClient(); 식으로 하면 Endpoint를 찾지 못한다.
        // 어차피 Web.Config 에서 변경할 수 있는것이 아니므로 수동으로 EndPoint를 지정해 주었다.
        ServiceReferenceSmsServiceSend3.ServiceSend3SoapClient GetServiceSms(int server)
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;

            ServiceReferenceSmsServiceSend3.ServiceSend3SoapClient service = new ServiceReferenceSmsServiceSend3.ServiceSend3SoapClient(binding, GetEndPoint("ServiceSend3.asmx", server));

            return service;
        }

        public bool SendByAdmin(string tel_recv, string tel_send, string msg, int sms_type)
        {
            return SendByAdminWithServer(tel_recv, tel_send, msg, sms_type, 1);
        }

        public bool SendByAdminWithServer(string tel_recv, string tel_send, string msg, int sms_type, int server_no)
        {
            ServiceReferenceSmsServiceSend3.ServiceSend3SoapClient service = GetServiceSms(server_no);

            string seed = StringHash.MakeRandomSeed();
            string tel_recv_hash = StringHash.Encode(tel_recv, seed);
            string tel_send_hash = StringHash.Encode(tel_send, seed);
            string msg_hash = StringHash.Encode(msg, seed);
            string err_msg;

            byte[] hash = HashTool.MakeHash(tel_recv_hash + tel_send_hash + msg_hash + seed + "AdminSend");

            try
            {
                if (!service.SendByAdmin(tel_recv_hash, tel_send_hash, msg_hash, sms_type, seed, hash, out err_msg))
                {
                    return false;
                }
            }
            catch (Exception exception) // 2018-3-27 추가
            {
                err_msg = exception.Message;
            }

            return true;
        }
    }
}

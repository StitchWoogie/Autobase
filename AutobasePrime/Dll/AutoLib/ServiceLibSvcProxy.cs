using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NetTools;
using System.Threading;
using System.Security.Cryptography;
using AutoLibLocal;
using AutoLib.WcfReferenceDataGateServer;

namespace AutoLib
{
    public class ServiceLibSvcProxy
    {
        public static int nWebServerVersionMajor = 0;
        public static int nWebServerVersionMinor = 0;
        public static int nWebServerVersionBuild = 0;
        public static int nWebServerVersionRevision = 0;

        //public static string sSiteName;
        //public static int nServicePort = 8732;
        //public static EnumDataGateBindingType eBindType = EnumDataGateBindingType.NetTcp;

        public static int nConnectionID = 0;

        //static string GetServicePathHttp(string service)
        //{
        //    return String.Format("http://{0}:{1}/AutoWeb/Service/ServiceDataGateServer.svc", ConfigVarTotal.sSiteRootName, ConfigVarTotal.nServicePort);
        //}

        static string GetServicePathTcp(string service)
        {
            //return String.Format("net.tcp://{0}:{1}/AutoWeb/Service/ServiceDataGateServer.svc", ConfigVarTotal.sSiteRootName, ConfigVarTotal.nServicePort);

            //return String.Format("net.tcp://192.168.1.149:8733/Service/ServiceProxyServerMongoDB.svc");//, ConfigVarTotal.sSiteRootName, ConfigVarTotal.nServicePort);
            return String.Format("net.tcp://127.0.0.1:8733/Service/ServiceProxyServerMongoDB.svc");//, ConfigVarTotal.sSiteRootName, ConfigVarTotal.nServicePort);
        }

        //public static System.ServiceModel.EndpointAddress GetEndPointHttp(string service_name)
        //{
        //    string url = GetServicePathHttp(service_name);

        //    return new System.ServiceModel.EndpointAddress(url);
        //}

        public static System.ServiceModel.EndpointAddress GetEndPointTcp(string service_name)
        {
            string url = GetServicePathTcp(service_name);

            return new System.ServiceModel.EndpointAddress(url);
        }

        // WCF인 경우 Windows Form Application에서 호출할 때 서비스를 계속 만들면 조금 되다가 Hang이 걸린다. Windows 8 App에서는 그렇지 않았다.
        // 일단 최초 만들어 지면 그대로 사용하고 
        // 접속이 바뀌는 경우만 null로 바꾸면 될 듯하다. Thread양쪽에서 사용해도 잘 되는 것 같다.
        static WcfReferenceDataGateServer.ServiceDataGateServerClient saveDataGate = null;

        // 사이트가 바뀔때마다 접속을 리셋해야 한다.
        public static void ClearConnection()
        {
            saveDataGate = null;
        }

        public static WcfReferenceDataGateServer.ServiceDataGateServerClient GetServiceDataGate()
        {
            // if (saveDataGate != null) return saveDataGate; 계속 사용은 일단 뺀다. 2019-6-17

            EnumDataGateBindingType bindtype = ConfigVarTotal.eBindType;

            //if (bindtype == EnumDataGateBindingType.BasicHttp)
            //{
            //    System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding();

            //    //binding.MaxBufferSize = int.MaxValue;
            //    binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            //    binding.MaxReceivedMessageSize = int.MaxValue;
            //    binding.AllowCookies = true;

            //    WcfReferenceDataGateServer.ServiceDataGateServerClient service = new WcfReferenceDataGateServer.ServiceDataGateServerClient(binding, GetEndPointHttp("ServiceDataGateServer.svc"));

            //    saveDataGate = service;

            //    return service;

                
            //}
            //else
            //{
                System.ServiceModel.NetTcpBinding binding = new System.ServiceModel.NetTcpBinding();

                binding.Security.Mode = SecurityMode.None;         // Server/Client 모두 None으로 맞추어야 한다.

                binding.MaxBufferSize = int.MaxValue;
                binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                binding.MaxReceivedMessageSize = int.MaxValue;

                WcfReferenceDataGateServer.ServiceDataGateServerClient service = new WcfReferenceDataGateServer.ServiceDataGateServerClient(binding, GetEndPointTcp("ServiceDataGateServer.svc"));

                saveDataGate = service;

                return service;
            //}
        }

        byte[] MakeHash(byte[] org)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(org);

            return result;
        }

        static byte[] KEY = { 0xee, 0xc3, 0x1d, 0x2d, 0x1c, 0xcb, 0x18, 0x53, 0x13, 0x29, 0x19, 0x2c, 0x11, 0x23, 0x36, 0x4b };
        static byte[] IV = { 0xf5, 0x69, 0x73, 0x83, 0x1c, 0xa3, 0x1b, 0x2a, 0xc2, 0x2e, 0x42, 0x20, 0x1f, 0x58, 0x1a, 0x27 };

        bool CheckHash(byte[][] args)
        {
            if (args.Length == 0) return true;
            byte[] hash = args[args.Length - 1];

            int nTotalCount = 0;

            for (int i = 0; i < args.Length - 1; i++)
                nTotalCount += args[i].Length;
            nTotalCount += 4;

            byte[] total = new byte[nTotalCount];

            int pos = 0;
            int length;

            for (int i = 0; i < args.Length - 1; i++)
            {
                length = args[i].Length;
                Array.Copy(args[i], 0, total, pos, length);
                pos += length;
            }

            total[pos++] = (byte)'~';
            total[pos++] = (byte)'$';
            total[pos++] = (byte)'7';
            total[pos++] = (byte)'l';

            byte[] total_hash = MakeHash(total);

            if (!total_hash.SequenceEqual(hash))
            {
                return false;
            }

            return true;
        }

        void AddCrc(List<byte[]> args)
        {
            int nTotalCount = 0;

            for (int i = 0; i < args.Count; i++)
                nTotalCount += args[i].Length;
            nTotalCount += 4;

            byte[] total = new byte[nTotalCount];

            int pos = 0;
            int length;

            for (int i = 0; i < args.Count; i++)
            {
                length = args[i].Length;
                Array.Copy(args[i], 0, total, pos, length);
                pos += length;
            }

            total[pos++] = (byte)'~';
            total[pos++] = (byte)'$';
            total[pos++] = (byte)'7';
            total[pos++] = (byte)'l';

            byte[] total_hash = MakeHash(total);

            args.Add(total_hash);
        }

        void AddString(List<byte[]> recv, string data)
        {
            byte[] baData = Encoding.UTF8.GetBytes(data);
            byte[] baData_Hash = NetTools.Cryptography.CryptoAES.Encrypt(baData, KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            recv.Add(baData_Hash);
        }

        // 오류메시지는 맨처음에 오류메시지를 넣고 -1을 반환한다.
        void AddErrorMessage(List<byte[]> recv, string format, params object[] args)
        {
            string msg = String.Format(format, args);
            byte[] baMessage = Encoding.UTF8.GetBytes(msg);
            byte[] baMessage_Hash = NetTools.Cryptography.CryptoAES.Encrypt(baMessage, KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            recv.Add(baMessage_Hash);

            AddCrc(recv);
        }

        byte[][] ListToByte(List<byte[]> send)
        {
            byte[][] retn = new byte[send.Count][];

            for (int i = 0; i < send.Count; i++)
            {
                retn[i] = send[i];
            }

            return retn;
        }

        List<byte[]> arrayReceive;
        public string sErrorMessage;

        public string GetResultString(int index)
        {
            string data = Encoding.UTF8.GetString(arrayReceive[index]);

            return data;
        }

        public byte[] GetResultBytes(int index)
        {
            if (index >= arrayReceive.Count) return null;
            return arrayReceive[index];
        }

        string sPrepareArg1 = null;

        public void PrepareArg1(string arg1)
        {
            sPrepareArg1 = arg1;
        }

        public int Command(string command, params object[] param)
        {
            AutoLib.WcfReferenceDataGateServer.ServiceDataGateServerClient service = GetServiceDataGate();

            List<byte[]> send = new List<byte[]>();
            byte[][] recv;

            CommaTextMaker ctm = new CommaTextMaker();
            ctm.Write("{0},{1}", ServiceLibSvcDataGate.nConnectionID, command);
            for (int i = 0; i < param.Length; i++)
            {
                ctm.Write(",{0}", param[i]);
            }
            AddString(send, ctm.GetResult());

            if (sPrepareArg1 != null)
            {
                AddString(send, sPrepareArg1);
                sPrepareArg1 = null;    // 클리어해 준다. 클래스를 다시 사용할 수 있으므로
            }
            AddCrc(send);

            int retn;

            try
            {
                CommonMethodResult result = service.CommonMethod(ListToByte(send));
                retn = result.Code;
                recv = result.Recv;
            }
            catch (Exception exception)
            {
                sErrorMessage = String.Format("Command:{0}\nError={1}", command, exception.Message);
                return -1;
            }

            if (!CheckHash(recv))
            {
                sErrorMessage = "Receive Hash Error";
                return -1;
            }

            arrayReceive = new List<byte[]>();
            for(int i = 0; i < recv.Length-1; i++) 
            {
                arrayReceive.Add(NetTools.Cryptography.CryptoAES.Decrypt(recv[i], KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7));
            }

            if (retn == -1)
            {
                sErrorMessage = Encoding.UTF8.GetString(arrayReceive[0]);
                return -1;
            }

            return retn;
        }
    }

    

}

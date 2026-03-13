using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NetTools;
using System.Threading;
using System.Security.Cryptography;

namespace AutoLib
{
    public class ServiceLibWebDataGate
    {
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
            if (args == null || args.Length == 0)
                return true;

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

        /*
        byte[][] ListToByte(List<byte[]> send)
        {
            byte[][] retn = new byte[send.Count][];

            for (int i = 0; i < send.Count; i++)
            {
                retn[i] = send[i];
            }

            return retn;
        }*/

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
            AutoLib.ServiceReferenceDataGateServer.WcfServiceDataGateServerProxyClient service = ServiceLib.GetServiceDataGateServer();

            //ServiceReferenceDataGateServer.ArrayOfBase64Binary send = new ServiceReferenceDataGateServer.ArrayOfBase64Binary();
            //ServiceReferenceDataGateServer.ArrayOfBase64Binary recv;

            // WCF에서는 ArrayOfBase64Binary가 아니라 List<byte[]>
            List<byte[]> sendList = new List<byte[]>();
            byte[][] recv;

            CommaTextMaker ctm = new CommaTextMaker();
            ctm.Write("{0},{1}", ServiceLibSvcDataGate.nConnectionID, command);
            for (int i = 0; i < param.Length; i++)
            {
                ctm.Write(",{0}", param[i]);
            }
            //AddString(send, ctm.GetResult());
            AddString(sendList, ctm.GetResult());

            if (sPrepareArg1 != null)
            {
                // AddString(send, sPrepareArg1);
                AddString(sendList, sPrepareArg1);
                sPrepareArg1 = null;    // 클리어해 준다. 클래스를 다시 사용할 수 있으므로
            }
            // AddCrc(send);
            AddCrc(sendList);


            int retn;

            try
            {
                // retn = service.CommonMethod(send, out recv);
                retn = service.CommonMethod(sendList.ToArray(), out recv);
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
            for(int i = 0; i < recv.Length - 1; i++) 
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

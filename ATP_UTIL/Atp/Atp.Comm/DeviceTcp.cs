using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ats;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using NetTools;
using NetTools.Cryptography;

namespace Ats.Comm
{
    public class DeviceTcp : DeviceCommon
    {
        public Socket socket = null;

        public string sMessage = "";

        string sIP;
        int nTcpPort;
        bool bServerFlag;

        public bool Init(bool server_flag, ProtocolCommon protocol, string ip, int port)
        {
            bServerFlag = server_flag;
            sIP = ip;
            nTcpPort = port;

            pProtocol = protocol;
            pProtocol.SetDevice(this);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(ip);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                
                socket.Connect(remoteEP);
                sMessage = "Port Ready.";

                bConnected = true;
            }
            catch (Exception exception)
            {
                sMessage = exception.Message;
            }

            if(bServerFlag)
                InitThread();

            pProtocol.ProtocolInit();

            return bConnected;
        }

        //bool bInitByListener = false;

        public void InitByListener(ProtocolCommon protocol, Socket s)
        {
            //bInitByListener = true;

            sIP = s.RemoteEndPoint.ToString();

            // IP와 포트를 잘 분리해 놓는다. 
            IPEndPoint remoteIpEndPoint = (IPEndPoint)s.RemoteEndPoint;
            sIP = remoteIpEndPoint.Address.ToString();
            nTcpPort = remoteIpEndPoint.Port;

            pProtocol = protocol;
            pProtocol.SetDevice(this);

            socket = s;
            bConnected = true;

            InitThread();

            pProtocol.ProtocolInit();
        }

        public override void UnInit()
        {
            if (pProtocol != null)
                pProtocol.ProtocolUnInit();

            //if (bServerFlag)
                UnInitThread();

            socket.Close();
            bConnected = false;
        }

        bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 & part2)
            {//connection is closed
                return false;
            }
            return true;
        }

        TimeOutClass timeoutDisconnect = new TimeOutClass();
        
        public override void OnThread()
        {
            if (!bConnected) return;

            if (!socket.Connected)
            {
                bConnected = false;
                return;
            }

            if (socket.Available == 0)
            {
                // 10초에 한번씩 접속이 끊어졌는가를 검사한다.
                if (timeoutDisconnect.IsTimeOut(10))
                {
                    timeoutDisconnect.Reset();
                    if (!SocketConnected(socket))
                    {
                        bConnected = false;
                    }
                }
                return;
            }

            byte[] buffer = new byte[socket.Available];

            int count;

            try
            {
                count = socket.Receive(buffer);     // 클라이언트를 정상 종료하지 않고 윈도우즈 종료를 했을 때 발생했다. 2016-5-26
            }
            catch
            {
                count = 0;
            }

            if (count == 0) return;

            timeoutDisconnect.Reset();

            nRecvBytes += count;

            if (pCC != null && pCC.bUseEncryption)
            {
                for (int i = 0; i < count; i++)
                {
                    pCC.SetEncryptedData(buffer[i]);
                }

                byte[] decoded = new byte[10];
                while(pCC.GetDecryptionData(decoded, 1) > 0) {
                    pProtocol.ExecuteOneChar(decoded[0]);
                }
            }
            else
            {
                // 보낸블럭이 두번에 나누어 오는 경우도 있고 두개의 블럭이 한번에 붙어서 오는 경우도 있다.
                if (pProtocol.bUseBlockReceive)
                {
                    pProtocol.ExecuteOneBlock(buffer, count);
                }
                else
                {
                    int ch = -1;

                    for (int i = 0; i < count; i++)
                    {
                        ch = buffer[i];

                        AddCommCode(EnumCommCode.RecvCode, (byte)ch);

                        pProtocol.ExecuteOneChar(ch);
                    }
                }
            }
        }

        public override void Write(byte[] source, int offset, int count)
        {
            byte[] data;

            if (pCC != null)
            {
                data = pCC.GetEncryptionData(source, ref offset, ref count);
            }
            else
            {
                data = source;
            }

            try
            {
                socket.Send(data, offset, count, SocketFlags.None);
            }
            catch
            {
                bConnected = false;
                return;
            }

            AddCommCode(EnumCommCode.SendNextLine, 0);
            for (int i = 0; i < count; i++)
                AddCommCode(EnumCommCode.SendCode, (byte)(data[i + offset]));

            nSendBytes += count;
        }

        public override string GetInfoString()
        {
            return String.Format("TCP,{0},{1}", sIP, nTcpPort);
        }

        public override void Clear()
        {
            int available = socket.Available;

            if (available <= 0) return;

            byte[] buffer = new byte[available];

            int count = socket.Receive(buffer);            
        }
    }
}

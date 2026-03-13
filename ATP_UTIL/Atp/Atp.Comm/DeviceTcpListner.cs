using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetTools.Cryptography;
using NetTools;

namespace Ats.Comm
{
    public class DeviceTcpListener
    {
        public delegate ProtocolCommon DelegateGetProtocol();
        protected DelegateGetProtocol procGetProtocol = null;

        TcpListener tcpListener;
        Thread threadListner;
        bool bEndListner = false;

        public bool bError = false;
        public string sErrorString;

        public void Init(DelegateGetProtocol proc, int port)
        {
            // Ready Tcp Listener
            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");

            tcpListener = new TcpListener(ipAddress, port);
            threadListner = new Thread(new ThreadStart(ThreadProcListener));
            threadListner.Start();

            procGetProtocol = proc;
        }

        CryptoCommunication pCC = null;

        public void SetCryptoComm(CryptoCommunication cc)
        {
            pCC = cc;
        }

        // 리스너 하나에는 여러개의 소켓이 붙을 수 있으므로 중간에 클래스가 바뀌면 오류가 날 수 있으므로 정보를 복제해서 사용한다.
        CryptoCommunication CloneCryptoCommClass()
        {
            if (pCC == null) return null;

            return (CryptoCommunication) Tools.CopyObject(pCC);
        }

        void RemoveDisconnectedSocket()
        {
            lock (arrayDevice)
            {
                for (int i = 0; i < arrayDevice.Count; i++)
                {
                    if (!arrayDevice[i].Connected)
                    {
                        arrayDevice[i].UnInit();
                        arrayDevice.RemoveAt(i);
                        return; // 하나만 삭제하고 다음에 삭제해도 된다.
                    }
                }
            }
        }

        void ThreadProcListener()
        {
            bEndListner = false;

            try
            {
                tcpListener.Start();
            }
            catch (Exception exception)
            {
                bError = true;
                sErrorString = exception.Message;
                return;
            }

            while (!bEndListner)
            {
                Thread.Sleep(100);  // 자주 접속이 들어오는것이 아니기 때문에 천천히 한다.

                RemoveDisconnectedSocket();

                if (!tcpListener.Pending()) continue;

                Socket socket = tcpListener.AcceptSocket();

                DeviceTcp tcp = new DeviceTcp();

                ProtocolCommon protocol = procGetProtocol();

                tcp.InitByListener(protocol, socket);
                tcp.SetCryptoComm(this.CloneCryptoCommClass()); // 암호화 방법을 복사해준다.

                lock (arrayDevice)
                {
                    arrayDevice.Add(tcp);
                }
            }
            tcpListener.Stop();
        }

        public void UnInit()
        {
            bEndListner = true;
            threadListner.Join(1000);

            lock (arrayDevice)
            {
                for (int i = 0; i < arrayDevice.Count; i++)
                {
                    arrayDevice[i].UnInit();
                }

                arrayDevice.Clear();
            }
        }

        public List<DeviceCommon> arrayDevice = new List<DeviceCommon>();

    }
}

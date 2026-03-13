using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NetTools;
using NetTools.Cryptography;

namespace Ats.Comm
{
    public enum EnumCommCode : byte
    {
        SendCode = 0,
        RecvCode = 1,
        SendNextLine = 10,
        RecvNextLine = 20
    }

    public class DeviceCommon
    {
        protected ProtocolCommon pProtocol = null;

        Thread thread;
        bool bEndThread = false;

        ushort[] ring_code = new ushort[60000];
        int nRingTarget = 0;
        //int nRingCurrent = 0;

        public int nRecvBytes = 0;
        public int nSendBytes = 0;
        public DateTime tConnected = DateTime.Now;

        protected bool bConnected = false;     // 접속 상태. 항상 접속 상태이어야 하며 false가 되면 오류가 나서 끊어지거나 문제가 있는 경우이다.

        protected CryptoCommunication pCC = null;          // null이면 프로그램에서 암호화를 사용하지 않는다.

        public void SetCryptoComm(CryptoCommunication cc)
        {
            pCC = cc;
        }

        public bool Connected
        {
            get
            {
                return bConnected;
            }
        }

        // 통신코드를 스레드 상에서 보여줄 수 없으므로 버퍼에 보관한다.
        protected void AddCommCode(EnumCommCode type, byte code)
        {
            ring_code[nRingTarget] = (ushort)(((byte)type << 8) | code);
            nRingTarget++;
            nRingTarget %= ring_code.Length;
        }

        /*
        // 보관된 통신코드를 Timer발생시에 표시해 준다.
        public void ViewCommCode()
        {
            for (int i = 0; i < 100; i++)
            {
                if (nRingTarget == nRingCurrent) return;

                ushort data = ring_code[nRingCurrent];

                nRingCurrent++;
                nRingCurrent %= ring_code.Length;

                byte type = (byte)((data >> 8) & 0xFF);
                byte code = (byte)((data >> 0) & 0xFF);

                if (type == 0) FormCodeView.DisplaySendCode(code);
                else if (type == 1) FormCodeView.DisplayRecvCode(code);
                else if (type == 10) FormCodeView.DisplaySendNextLine();
                else if (type == 20) FormCodeView.DisplayRecvNextLine();
            }
        }*/

        protected void InitThread()
        {
            thread = new Thread(new ThreadStart(ThreadProc));
            thread.Start();
        }

        void ThreadProc()
        {
            bEndThread = false;

            while (!bEndThread)
            {
                Thread.Sleep(1);
                OnThread();
            }
        }

        /* 이 코드가 예전에 있었는데 무슨코드?  2016-5-3
        int nTimeout = 1000;
        TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();

        public void TimeoutReset()
        {
            timeout.Reset();
        }

        public int Timeout
        {
            get
            {
                return nTimeout;
            }
            set
            {
                nTimeout = value;
            }
        }

        void ThreadProc()
        {
            bEndThread = false;

            bool timeout_old = true;
            bool timeout_curr;

            timeout.SetTime(nTimeout + 1);  // 최초에 Timeout을 만들어 놓는다.

            while (!bEndThread)
            {
                Thread.Sleep(1);
                OnThread();

                timeout_curr = timeout.IsTimeOut(nTimeout);
                if (timeout_curr != timeout_old)
                {
                    if (timeout_curr)
                    {
                        pProtocol.GoCallback(EnumProtocolReturnCode.ErrorTimeOut);
                    }
                    timeout_old = timeout_curr;
                }
            }
        }*/

        protected void UnInitThread()
        {
            bEndThread = true;

            if (thread != null)
            {
                thread.Join(1000);
                thread = null;
            }
        }

        public virtual void UnInit()
        {

        }

        public virtual void OnThread()
        {

        }

        public virtual void Write(byte[] buffer, int pos, int size)
        {

        }

        public virtual string GetInfoString()
        {
            return "Fill Info String";
        }

        public virtual void Clear()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace ScriptLibRun.Debugger
{
    public class MyTcpServer : MyTcpPublic
    {
        public int nTcpPort;

        bool m_Done = false;
        bool m_bEnded = false;
        TcpListener m_Listener = null;
        Thread threadListener = null;

        public void Init(int port)
        {
            m_Done = false;
            m_bEnded = false;

            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");

            m_Listener = new TcpListener(ipAddress, port);

            threadListener = new Thread(new ThreadStart(Listener));
            threadListener.Start();
        }

        public void UnInit()
        {
            m_Done = true;

            TcpClient s = new TcpClient();
            try
            {
                s.Connect("127.0.0.1", nTcpPort);   // 한번 연결해 주어야 m_Listener.AcceptSocket() 을 지나간다.
            }
            catch
            {

            }

            while (!m_bEnded)
            {
                Thread.Sleep(1);	// wait thread ended
            }

            threadListener.Join();
        }

        List<SocketThreadClass> arraySocketThread = new List<SocketThreadClass>(); 
        SocketThreadClass tempSocketThread;

        void Listener()
        {
            m_Listener.Start();
            while (!m_Done)
            {
                Thread.Sleep(1);

                Socket socket = m_Listener.AcceptSocket();

                tempSocketThread = new SocketThreadClass();
                tempSocketThread.socket = socket;
                tempSocketThread.bClose = false;
                tempSocketThread.thread = new Thread(new ThreadStart(ThreadSocket));
                tempSocketThread.thread.Start();
            }
            m_Listener.Stop();

            while (true)
            {
                Thread.Sleep(1);
                if (arraySocketThread.Count == 0) break;
            }

            m_bEnded = true;

            return;
        }

        public virtual void OnReceive(byte[] data)
        {

        }

        void ThreadSocket()
        {
            SocketThreadClass socketThread = tempSocketThread;

            lock (syncLock)
            {
                arraySocketThread.Add(socketThread);
            }

            Socket socket = socketThread.socket;

            byte[] recv_data = new byte[10000];
            int count;
            List<byte> arrayMulti = null;
            int nMultiFrameNo=0;
            bool bMultiFrameStart = false;
            int nMultiFrameTotalSize = 0;
            int nMultiFrameCount = 0;

            while (!m_Done)
            {
                Thread.Sleep(1);

                //if(timeout_nosignal.IsTimeOut(this.nDisconnectTimeOutOnNoSignal))	break;
                if(!socket.Connected)	break;

                count = socket.Available;
                if(count == 0)	continue;

                //timeout_nosignal.Reset();

                //if(count > 10000)	count = 10000;

                try
                {
                    count = socket.Receive(recv_data);
                }
                catch
                {
                    count = 0;
                }

                int trans = recv_data[2] + recv_data[3] * 0x100 + recv_data[4] * 0x10000 + recv_data[5] * 0x1000000;

                if (recv_data[0] == (byte)EnumTcpCommand.MultiFrameStart)
                {
                    nMultiFrameTotalSize = recv_data[6] + recv_data[7] * 0x100 + recv_data[8] * 0x10000 + recv_data[9] * 0x1000000;
                    nMultiFrameNo = recv_data[10] + recv_data[11] * 0x100 + recv_data[12] * 0x10000 + recv_data[13] * 0x1000000;
                    int data_size = recv_data[14] + recv_data[15] * 0x100;

                    if (data_size+16 != count)
                    {
                        SendNAK(socket, trans, EnumNakType.DataSizeMismatched);
                        continue;
                    }

                    if (nMultiFrameNo != 0)
                    {
                        SendNAK(socket, trans, EnumNakType.FirstFrameNoIsNotZero);
                        continue;
                    }

                    SendACK(socket, trans); // ACK를 먼저 보내준다.

                    bMultiFrameStart = true;
                    arrayMulti = new List<byte>();

                    nMultiFrameCount = data_size;
                    for (int i = 0; i < data_size; i++)
                    {
                        arrayMulti.Add(recv_data[16 + i]);
                    }
                }
                else if (recv_data[0] == (byte)EnumTcpCommand.MultiFrameContinue)
                {
                    if (!bMultiFrameStart)
                    {
                        SendNAK(socket, trans, EnumNakType.MultiFrameNotStarted);
                        continue;
                    }

                    int frame_no = recv_data[6] + recv_data[7] * 0x100 + recv_data[8] * 0x10000 + recv_data[9] * 0x1000000;
                    int data_size = recv_data[10] + recv_data[11] * 0x100;

                    if (data_size + 12 != count)
                    {
                        SendNAK(socket, trans, EnumNakType.DataSizeMismatched);
                        continue;
                    }

                    if (frame_no != nMultiFrameNo+1)
                    {
                        SendNAK(socket, trans, EnumNakType.NextFrameNoMismatched);
                        continue;
                    }

                    nMultiFrameNo = frame_no;
                    nMultiFrameCount += data_size;

                    for (int i = 0; i < data_size; i++)
                    {
                        arrayMulti.Add(recv_data[16 + i]);
                    }

                    if (nMultiFrameCount == nMultiFrameTotalSize)
                    {
                        SendACK(socket, trans);
                        OnReceive(arrayMulti.ToArray());
                        bMultiFrameStart = false;
                    }
                    else if (nMultiFrameCount > nMultiFrameTotalSize)
                    {
                        SendNAK(socket, trans, EnumNakType.MultiFrameOverTotalSize);
                        bMultiFrameStart = false;
                    }
                }
                else if (recv_data[0] == (byte)EnumTcpCommand.SingleBlock)
                {
                    int data_size = recv_data[6] + recv_data[7] * 0x100;

                    if (data_size + 8 != count)
                    {
                        SendNAK(socket, trans, EnumNakType.DataSizeMismatched);
                        continue;
                    }

                    arrayMulti = new List<byte>();
                    for (int i = 0; i < data_size; i++)
                    {
                        arrayMulti.Add(recv_data[8 + i]);
                    }

                    OnReceive(arrayMulti.ToArray());
                }
                else if (recv_data[0] == (byte)EnumTcpCommand.Closed)
                {
                    break;
                }
                else 
                {
                    SendNAK(socket, trans, EnumNakType.UnKnownCommand);
                }
            }

            socket.Close();

            lock (syncLock)
            {
                arraySocketThread.Remove(socketThread);
            }
        }

        object syncLock = new object(); // Thread에서 중첩되는 Access를 막기 위해서
    }

    
}

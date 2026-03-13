using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace ScriptLibRun.Debugger
{
    public class MyTcpClient : MyTcpPublic
    {
        string sIP;
        int nPort;
        Thread thread;
        bool m_Done = false;
        bool m_Ended = false;

        public void Init(string ip, int port)
        {
            sIP = ip;
            nPort = port;
            thread = new Thread(new ThreadStart(ThreadSocket));
            thread.Start();
        }

        public void UnInit()
        {
            m_Done = true;

            while (!m_Ended)
            {
                Thread.Sleep(1);
            }
        }

        public virtual void OnReceive(byte[] data)
        {

        }

        int nTransNo = 0;

        int MakeNewTrans()
        {
            nTransNo++;
            return nTransNo;
        }

        void SendFrame(Socket socket)
        {
            /*
            byte[] data;
            
            if (dataToSend.Length < 4000)
            {
                int trans = MakeNewTrans();

                data = new byte[dataToSend.Length+8];
                data[0] = (byte)EnumTcpCommand.SingleBlock;
                data[1] = 0;
                data[2] = 
            }*/
        }

        void ThreadSocket()
        {
            Socket socket = null;

            byte[] recv_data = new byte[10000];
            int count;
            List<byte> arrayMulti = null;
            int nMultiFrameNo = 0;
            bool bMultiFrameStart = false;
            int nMultiFrameTotalSize = 0;
            int nMultiFrameCount = 0;

            while (!m_Done)
            {
                Thread.Sleep(1);

                if (!socket.Connected) break;

                count = socket.Available;
                if (count == 0)
                {
                    if (bDataToSend)
                    {
                        SendFrame(socket);
                    }
                    continue;
                }

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

                    if (data_size + 16 != count)
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

                    if (frame_no != nMultiFrameNo + 1)
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

            m_Ended = true;
        }

        bool bDataToSend = false;
        byte[] dataToSend = null;

        public void Send(byte[] data)
        {
            dataToSend = data;
            bDataToSend = true;
        }
    }
}

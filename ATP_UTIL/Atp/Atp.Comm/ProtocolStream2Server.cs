using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ats;
using System.IO;
using NetTools;
using System.Threading;

namespace Ats.Comm
{
    /// <summary>
    /// ProtocolStreamServer/Client 는 TCP에만 최적화된 것으로 제작되었는데 통신 중 프레임이 잘려서 들어오는 현상때문에 하루에 몇번정도 오류가 발생한다.
    /// ProtocolStream2Server/Client 는 이 현상을 막기위해서 DLE+STX+DATA+DLE+ETX 방식으로 전송해서 RS-232에서도 같이 사용할 수 있도록 만들었다.
    /// 또 버전정보를 사용하지 않기 때문에 최초 속도가 조금 더 빠르다.
    /// 전체적으로는 조금 느릴수도 있음.
    /// 
    /// -사용방법-
    /// ProtocolStream2Server/Client 를 추천한다.
    /// </summary>
    public class ProtocolStream2Server : ProtocolCommon
    {
        enum EnumCommand : byte
        {
            Ack = 6,

            CS_SingleBlock = 9,    // 클라이언트->서버 통신. 한번에 가는 프레임
            CS_MultiBlock = 10,    // 클라이언트->서버 통신. 여러번에 나눠가는 프레임

            SC_SingleBlock = 12,   // 서버->클라이언트 통신. 한번에 가는 프레임
            SC_MultiBlock = 13,    // 서버->클라이언트 통신. 여러번에 나눠가는 프레임
        }

        public ProtocolStream2Server()
        {

        }

        byte[] encodedBuffer = new byte[MAX_SOCKET_BUFFER_SIZE];  // 최종적으로 인코딩된 버퍼이다. 계속 사용하기 때문에 할당해서 사용한다.

        void DeviceWrite(byte[] buf, int count)
        {
            int pos = 0;
            encodedBuffer[pos++] = 0x10;
            encodedBuffer[pos++] = 0x02;
            for (int i = 0; i < count; i++)
            {
                if (buf[i] == 0x10)
                    encodedBuffer[pos++] = 0x10;
                encodedBuffer[pos++] = buf[i];
            }
            encodedBuffer[pos++] = 0x10;
            encodedBuffer[pos++] = 0x03;

            pDevice.Write(encodedBuffer, 0, pos);
        }

        void SendACK(int tns)
        {
            byte[] buf = new byte[5];

            buf[0] = (byte)(buf.Length % 256);
            buf[1] = (byte)(buf.Length / 256);
            buf[2] = (byte)(tns % 256);
            buf[3] = (byte)(tns / 256);
            buf[4] = (byte)EnumCommand.Ack;

            DeviceWrite(buf, buf.Length);

            //Thread.Sleep(0);
        }

        ushort nTransactionNumber = 0;
        ushort GetNewTransactionNumber()
        {
            nTransactionNumber++;
            return nTransactionNumber;
        }

        bool bOrderServerToClient = false;  // 서버가 클라이언트에 데이터를 주는 차례

        byte[] receivedBuffer = null;
        int receivedCount = 0;
        int receivedTotalLength = 0;

        void BlockCopy(byte[] target, int pos_t, byte[] source, int pos_s, int size)
        {
            for (int i = 0; i < size; i++)
            {
                target[pos_t + i] = source[pos_s + i];
            }
        }

        void CheckOneFrameClientToServer(byte[] buf, int count)
        {
            int tns = buf[2] + buf[3] * 256;
            byte command = buf[4];

            if (command == (byte)EnumCommand.CS_SingleBlock)
            {
                int data_size = count - 5;

                receivedBuffer = new byte[data_size];
                BlockCopy(receivedBuffer, 0, buf, 5, data_size);
                ExecuteOneBuffer(receivedBuffer);
            }
            else if (command == (byte)EnumCommand.CS_MultiBlock)
            {
                int total_length = (buf[5] << 0) | (buf[6] << 8) | (buf[7] << 16) | (buf[8] << 24);
                int buf_pos = (buf[9] << 0) | (buf[10] << 8) | (buf[11] << 16) | (buf[12] << 24);

                // 버퍼 위치가 클수는 없다. 이상한 코드일 가능성이 있다.
                if (buf_pos >= total_length)
                {
                    ErrorAndReset("buf_pos >= total_length");
                    return;
                }

                if (buf_pos == 0)
                {
                    bOrderServerToClient = false;

                    receivedTotalLength = total_length;
                    receivedCount = 0;
                    receivedBuffer = new byte[total_length];
                }
                else
                {
                    // 받는 순서가 틀렸다.
                    if (buf_pos != receivedCount)
                    {
                        ErrorAndReset("buf_pos != receivedCount");
                        return;
                    }

                    // 받는 중간에 총 크기가 변경되었다.
                    if (total_length != receivedTotalLength)
                    {
                        ErrorAndReset("total_length != receivedTotalLength");
                        return;
                    }
                }

                int data_size = count - 13;

                // 틀렸다.
                if (receivedCount + data_size > receivedTotalLength)
                {
                    ErrorAndReset("receivedCount + data_size > receivedTotalLength");
                    return;
                }

                BlockCopy(receivedBuffer, receivedCount, buf, 13, data_size);

                receivedCount += data_size;

                // 마지막에는 ACK를 보내지 않는다.
                if (receivedCount >= receivedTotalLength)
                {
                    ExecuteOneBuffer(receivedBuffer);
                }
                else
                {
                    SendACK(tns);
                }
            }
        }

        void CheckOneFrameServerToClient(byte[] buf, int count)
        {
            int tns = buf[2] + buf[3] * 256;
            byte command = buf[4];

            if (tns != nTransactionNumber)
            {
                ErrorAndReset("tns != nTransactionNumber");
                return;
            }

            if (command != (byte)EnumCommand.Ack)
            {
                ErrorAndReset("command != (byte)EnumCommand.Ack");
                return;
            }

            SendNextBlockOfMultiBlock();
        }

        void ErrorAndReset(string msg)
        {
            sErrorMessage = msg;
            bOrderServerToClient = false;

            if (procErrorCallback != null)
                procErrorCallback(msg);
        }

        // 하나의 프레임을 모은다.
        void CheckOneBlock(byte[] buf, int count)
        {
            // 프레임이 너무 작아도 안되고
            if (count < 5)
            {
                string msg = "count < 5  buf=";
                for (int i = 0; i < count; i++)
                    msg += String.Format("{0:X02} ", buf[i]);

                ErrorAndReset(msg);
                return;
            }

            int frame_size = buf[0] + buf[1] * 256;

            if (frame_size < count)
            {
                ErrorAndReset("frame_size < count");
                return;
            }
            else if (frame_size > count)
            {
                ErrorAndReset("frame_size > count");
                return;
            }

            if (bOrderServerToClient)
                CheckOneFrameServerToClient(buf, count);
            else
                CheckOneFrameClientToServer(buf, count);
        }

        byte[] dataGather = new byte[MAX_ONE_FRAME];
        int nGatherData = 0;
        bool dle_flag = false;
        bool bStartFlag = false;

        public override void ExecuteOneChar(int ch)
        {
            if (dle_flag)
            {
                dle_flag = false;

                if (ch == 0x02) // STX 가 들어오면 처음부터 시작한다.
                {
                    bStartFlag = true;
                    nGatherData = 0;
                }
                else if (ch == 0x03)
                {
                    if (bStartFlag) // STX가 있는 경우에만 의미가 있다.
                    {
                        goto ok_oneframe_received;
                    }

                    bStartFlag = false;
                }
                else if (ch == 0x10)
                {
                    if (nGatherData < dataGather.Length)
                        dataGather[nGatherData++] = (byte)ch;
                }
                else
                {

                }
            }
            else
            {
                if (ch == 0x10)
                {
                    dle_flag = true;
                }
                else
                {
                    if (bStartFlag)
                    {
                        if (nGatherData < dataGather.Length)
                            dataGather[nGatherData++] = (byte)ch;
                    }
                }
            }

            return;

        ok_oneframe_received: ;

            CheckOneBlock(dataGather, nGatherData);

            dle_flag = false;
            nGatherData = 0;
            bStartFlag = false;
        }

        // 정상적으로 받은 버퍼를 분석한다.
        void ExecuteOneBuffer(byte[] buf)
        {
            byte[] retn = SetValue(buf);

            ServerToClient(retn);
        }

        const int MAX_SOCKET_BUFFER_SIZE = 8192;
        const int MAX_ONE_FRAME = (MAX_SOCKET_BUFFER_SIZE - 4) / 2; // 실제로 소켓이 사용할 수 있는 크기에서 4를 뺀값의 반만을 사용한다. DLE+STX+DATA+DLE+ETX 형태에서 DATA가 모두 DLE이면 data*2+4 가 최대 나올 수 있는 값이다.
        int sendingCount = 0;
        byte[] sendingBuffer;

        void ServerToClient(byte[] buffer)
        {
            byte[] imsi = new byte[MAX_ONE_FRAME];

            int total_size = buffer.Length;
            int remain = total_size;
            int buf_pos = 0;

            int MAX_ONE_DATA_ZONE = MAX_ONE_FRAME - 5;  // size(2)+tns(2)+command(1)

            int tns = 0;

            // 한번에 다 보낼수 있다.
            if (total_size < MAX_ONE_DATA_ZONE)
            {
                tns = GetNewTransactionNumber();

                imsi[0] = (byte)((total_size + 5) % 256);
                imsi[1] = (byte)((total_size + 5) / 256);
                imsi[2] = (byte)(tns % 256);
                imsi[3] = (byte)(tns / 256);
                imsi[4] = (byte)(EnumCommand.SC_SingleBlock);

                BlockCopy(imsi, 5, buffer, buf_pos, total_size);

                DeviceWrite(imsi, total_size + 5);
                //pDevice.Write(imsi, 0, total_size + 5);

                return;
            }

            sendingBuffer = buffer;
            sendingCount = 0;
            bOrderServerToClient = true;    // 서버에서 데이터를 보내는 중이다.
            SendNextBlockOfMultiBlock();
        }

        void SendNextBlockOfMultiBlock()
        {
            byte[] imsi = new byte[MAX_ONE_FRAME];

            int MAX_ONE_DATA_ZONE_MULTI = MAX_ONE_FRAME - 13;  // size(2)+tns(2)+command(1)+total_size(4)+current_pos(4)
            int size;

            int total_size = sendingBuffer.Length;
            int remain = sendingBuffer.Length - sendingCount;

            if (remain <= MAX_ONE_DATA_ZONE_MULTI)
                size = remain;
            else
                size = MAX_ONE_DATA_ZONE_MULTI;

            int tns = GetNewTransactionNumber();

            imsi[0] = (byte)((size + 13) % 256);
            imsi[1] = (byte)((size + 13) / 256);
            imsi[2] = (byte)(tns % 256);
            imsi[3] = (byte)(tns / 256);
            imsi[4] = (byte)(EnumCommand.SC_MultiBlock);

            imsi[5] = (byte)((total_size >> 0) & 0xFF);
            imsi[6] = (byte)((total_size >> 8) & 0xFF);
            imsi[7] = (byte)((total_size >> 16) & 0xFF);
            imsi[8] = (byte)((total_size >> 24) & 0xFF);

            imsi[9] = (byte)((sendingCount >> 0) & 0xFF);
            imsi[10] = (byte)((sendingCount >> 8) & 0xFF);
            imsi[11] = (byte)((sendingCount >> 16) & 0xFF);
            imsi[12] = (byte)((sendingCount >> 24) & 0xFF);

            BlockCopy(imsi, 13, sendingBuffer, sendingCount, size);

            DeviceWrite(imsi, size + 13);
            //pDevice.Write(imsi, 0, size + 13);

            sendingCount += size;

            if (sendingCount >= total_size)
            {
                bOrderServerToClient = false;
            }
        }

        public virtual byte[] SetValue(byte[] buf)
        {
            byte[] retn = new byte[1];
            retn[0] = 1;

            return retn;
        }

        public delegate void DelegateErrorCallback(string msg);
        DelegateErrorCallback procErrorCallback = null;

        public void SetProcErrorCallback(DelegateErrorCallback proc)
        {
            procErrorCallback = proc;
        }
    }
}

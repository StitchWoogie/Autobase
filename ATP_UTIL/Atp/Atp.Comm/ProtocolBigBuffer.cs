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
    public class ProtocolBigBuffer : ProtocolCommon
    {
        enum EnumCommand : byte
        {
            STX = 2,
            ETX = 3,

            Ack = 6,

            DLE = 0x10,

            SingleBlock = 0x20,    // 한번에 가는 프레임
            MultiBlock = 0x21,     // 여러번에 나눠가는 프레임
        }

        public ProtocolBigBuffer()
        {
            bUseBlockReceive = true;
        }
        
        void SendACK(int tns)
        {
            // ACK는 tns에 대한 응답만 하면 되므로 3바이트로 하는것이 좋다.
            byte[] buf = new byte[3];

            buf[0] = (byte)EnumCommand.Ack;
            buf[1] = (byte)(tns % 256);
            buf[2] = (byte)(tns / 256);

            pDevice.Write(buf, 0, buf.Length);
        }

        ushort nTransactionNumber = 0;
        ushort GetNewTransactionNumber()
        {
            nTransactionNumber++;
            return nTransactionNumber;
        }
       
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

        void CheckOneFrame(byte[] buf, int count)
        {
            byte command = buf[0];
            int tns = buf[1] + buf[2] * 256;

            if (command == (byte)EnumCommand.SingleBlock)
            {
                int data_size = count - 5;

                receivedBuffer = new byte[data_size];
                BlockCopy(receivedBuffer, 0, buf, 5, data_size);
                ExecuteOneBuffer(receivedBuffer);
            }
            else if (command == (byte)EnumCommand.MultiBlock)
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

                SendACK(tns);

                if (receivedCount >= receivedTotalLength)
                {
                    ExecuteOneBuffer(receivedBuffer);
                }
            }
            
        }

        /*
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
        }*/

        int frameCount = 0;
        byte[] frameBuffer = new byte[10000];

        void ErrorAndReset(string msg)
        {
            sErrorMessage = msg;
            //frameCount = 0;

            if (procErrorCallback != null)
                procErrorCallback(msg);
        }

        bool bStartFlag = false;
        bool dle_flag = false;

        // 하나의 프레임을 모은다.
        public override void ExecuteOneBlock(byte[] buf, int count)
        {
            for (int i = 0; i < count; i++)
            {
                ExecuteOneChar(buf[i]);
            }
        }

        public override void ExecuteOneChar(int ch)
        {
            // check buffer over
            if (frameCount >= frameBuffer.Length)
            {
                frameCount = 0;
            }

            if (dle_flag)
            {
                dle_flag = false;

                if (ch == 0x02) // STX 가 들어오면 처음부터 시작한다.
                {
                    bStartFlag = true;
                    frameCount = 0;
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
                    frameBuffer[frameCount++] = (byte)ch;
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
                        frameBuffer[frameCount++] = (byte)ch;
                }
            }

            return;

        ok_oneframe_received: ;

            // 제일 작은 프레임이 3이다. ACK 프레임
            if (frameCount < 3)
            {
                ErrorAndReset("count < 3");
                return;
            }

            byte command = frameBuffer[0];

            if (command == (byte)EnumCommand.Ack)
            {
                if (frameCount != 3)
                {
                    ErrorAndReset("ACK frame_size != 3");
                }
                else
                {
                    int tns = frameBuffer[1] + frameBuffer[2] * 256;

                    if (bWaitingACK)
                    {
                        if (nWaitingTns == tns)
                        {
                            bWaitingACK = false;    // waiting ack를 풀어준다.
                        }
                        else
                        {
                            ErrorAndReset("nWaitingTns != tns");
                        }
                    }
                    else
                    {
                        ErrorAndReset("ACK received on bWaitingACK=false");
                    }
                }
            }
            else
            {
                int frame_size = frameBuffer[3] + frameBuffer[4] * 256;

                if (frame_size < frameCount)
                {
                    ErrorAndReset("frame_size < count");
                }
                else if (frame_size > frameCount)
                {
                    ErrorAndReset("frame_size > frameCount");
                }
                else
                {
                    CheckOneFrame(frameBuffer, frameCount);
                }
            }

            dle_flag = false;
            frameCount = 0;
            bStartFlag = false;
        }

        bool bWaitingACK = true;
        int  nWaitingTns = 0;

        // 정상적으로 받은 버퍼를 분석한다.
        void ExecuteOneBuffer(byte[] buf)
        {
            byte[] retn = SetValue(buf);

            //ServerToClient(retn);
        }

        const int MAX_ONE_FRAME = 4096;
        //int    sendingCount = 0;
        //byte[] sendingBuffer;

        bool ClientToServer(byte[] buffer)
        {
            byte[] imsi = new byte[MAX_ONE_FRAME];

            int total_size = buffer.Length;
            int remain = total_size;
            int buf_pos = 0;

            int MAX_ONE_DATA_ZONE = MAX_ONE_FRAME - 5;  // command(1)+tns(2)+size(2)

            int tns = 0;

            // 한번에 다 보낼수 있다.
            if (total_size <= MAX_ONE_DATA_ZONE)
            {
                tns = GetNewTransactionNumber();

                imsi[0] = (byte)(EnumCommand.SingleBlock);
                imsi[1] = (byte)(tns % 256);
                imsi[2] = (byte)(tns / 256);
                imsi[3] = (byte)((total_size + 5) % 256);
                imsi[4] = (byte)((total_size + 5) / 256);

                BlockCopy(imsi, 5, buffer, buf_pos, total_size);

                pDevice.Write(imsi, 0, total_size + 5);

                return true;
            }

            int MAX_ONE_DATA_ZONE_MULTI = MAX_ONE_FRAME - 13;  // command(1)+tns(2)+size(2)+total_size(4)+current_pos(4)
            int size;
            EnumProtocolReturnCode retn;

            // 여러번에 걸쳐서 보내야 한다.
            while (true)
            {
                if (remain <= 0)
                {
                    break;
                }

                if (remain <= MAX_ONE_DATA_ZONE_MULTI)
                    size = remain;
                else
                    size = MAX_ONE_DATA_ZONE_MULTI;

                tns = GetNewTransactionNumber();

                imsi[0] = (byte)(EnumCommand.MultiBlock);
                imsi[1] = (byte)(tns % 256);
                imsi[2] = (byte)(tns / 256);
                imsi[3] = (byte)((size + 13) % 256);
                imsi[4] = (byte)((size + 13) / 256);
                
                imsi[5] = (byte)((total_size >> 0) & 0xFF);
                imsi[6] = (byte)((total_size >> 8) & 0xFF);
                imsi[7] = (byte)((total_size >> 16) & 0xFF);
                imsi[8] = (byte)((total_size >> 24) & 0xFF);

                imsi[9] = (byte)((buf_pos >> 0) & 0xFF);
                imsi[10] = (byte)((buf_pos >> 8) & 0xFF);
                imsi[11] = (byte)((buf_pos >> 16) & 0xFF);
                imsi[12] = (byte)((buf_pos >> 24) & 0xFF);

                BlockCopy(imsi, 13, buffer, buf_pos, size);

                pDevice.Write(imsi, 0, size + 13);

                buf_pos += size;
                remain -= size;

                // 마지막에는 ACK를 기다리지 않는다.
                if (remain > 0)
                {
                    retn = WaitReceiveFrame();

                    if (retn != EnumProtocolReturnCode.OK) return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
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

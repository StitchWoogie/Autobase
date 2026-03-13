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
    public class ProtocolStreamServer : ProtocolCommon
    {
        enum EnumCommand : byte
        {
            Ack = 1,

            CS_SingleBlock = 2,    // 클라이언트->서버 통신. 한번에 가는 프레임
            CS_MultiBlock = 3,     // 클라이언트->서버 통신. 여러번에 나눠가는 프레임

            SC_SingleBlock = 12,   // 서버->클라이언트 통신. 한번에 가는 프레임
            SC_MultiBlock = 13,    // 서버->클라이언트 통신. 여러번에 나눠가는 프레임

            CS_SetVersion = 0xF1,     // 버전을 주고 받는다. 제일 먼저 할일이다.
            SC_SetVersion = 0xF2,     // 버전을 주고 받는다. 제일 먼저 할일이다.
        }

        int nClientVersionMajor = 1;
        int nClientVersionMinor = 0;
        int nClientVersionBuild = 0;
        int nClientVersionRevision = 0;

        const int nServerVersionMajor = 1;
        const int nServerVersionMinor = 0;
        const int nServerVersionBuild = 0;
        const int nServerVersionRevision = 0;

        public ProtocolStreamServer()
        {
            bUseBlockReceive = true;
        }
        
        void SendACK(int tns)
        {
            byte[] buf = new byte[5];

            buf[0] = (byte)(buf.Length % 256);
            buf[1] = (byte)(buf.Length / 256);
            buf[2] = (byte)(tns % 256);
            buf[3] = (byte)(tns / 256);
            buf[4] = (byte)EnumCommand.Ack;

            pDevice.Write(buf, 0, buf.Length);
            //Thread.Sleep(0);
        }

        void SendVersion(int tns)
        {
            byte[] buf = new byte[13];

            buf[0] = (byte)(buf.Length % 256);
            buf[1] = (byte)(buf.Length / 256);
            buf[2] = (byte)(tns % 256);
            buf[3] = (byte)(tns / 256);
            buf[4] = (byte)EnumCommand.SC_SetVersion;
            buf[5] = (byte)(nServerVersionMajor % 256);
            buf[6] = (byte)(nServerVersionMajor / 256);
            buf[7] = (byte)(nServerVersionMinor % 256);
            buf[8] = (byte)(nServerVersionMinor / 256);
            buf[9] = (byte)(nServerVersionBuild % 256);
            buf[10] = (byte)(nServerVersionBuild / 256);
            buf[11] = (byte)(nServerVersionRevision % 256);
            buf[12] = (byte)(nServerVersionRevision / 256);
            
            pDevice.Write(buf, 0, buf.Length);
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

            // 버전을 받지 않았을 때는 버전 정보만을 명령어로 기다린다. 
            if (!bVersionDownloaded)
            {
                if (command != (byte)EnumCommand.CS_SetVersion)
                {
                    ErrorAndReset("Version not downloaded.");
                    return;
                }
            }

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
            else if (command == (byte)EnumCommand.CS_SetVersion)
            {
                nClientVersionMajor = buf[5] + buf[6] * 256;
                nClientVersionMinor = buf[7] + buf[8] * 256;
                nClientVersionBuild = buf[9] + buf[10] * 256;
                nClientVersionRevision = buf[11] + buf[12] * 256;

                SendVersion(tns);

                bVersionDownloaded = true;
            }
        }

        bool bVersionDownloaded = false;

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

        int frameCount = 0;
        byte[] frameBuffer = new byte[10000];

        void ErrorAndReset(string msg)
        {
            sErrorMessage = msg;
            bOrderServerToClient = false;
            frameCount = 0;

            if (procErrorCallback != null)
                procErrorCallback(msg);
        }

        // 하나의 프레임을 모은다.
        public override void ExecuteOneBlock(byte[] buf, int count)
        {
        retry_same_buf:
            
            // 프레임이 너무 작아도 안되고
            if (count < 5)
            {
                ErrorAndReset("count < 5");
                return;
            }

            if (frameCount == 0)
            {
                int frame_size = buf[0] + buf[1] * 256;

                if (frame_size == count)
                {
                    BlockCopy(frameBuffer, 0, buf, 0, count);
                    frameCount = count;
                }
                else if (frame_size < count)
                {
                    ErrorAndReset("frame_size < count");
                    return;
                }
                else if (frame_size > frameBuffer.Length)
                {
                    ErrorAndReset("frame_size > frameBuffer.Length");
                    return;
                }
                else
                {
                    BlockCopy(frameBuffer, 0, buf, 0, count);
                    frameCount = count;
                    return;
                }
            }
            else
            {
                int frame_size = frameBuffer[0] + frameBuffer[1] * 256;

                if (frame_size == frameCount + count)
                {
                    BlockCopy(frameBuffer, frameCount, buf, 0, count);
                    frameCount += count;
                }
                else if (frame_size < frameCount + count)
                {
                    ErrorAndReset("frame_size < frameCount + count");
                    goto retry_same_buf;
                }
                else if (frameBuffer.Length < frameCount + count)
                {
                    ErrorAndReset("frameBuffer.Length < frameCount + count");
                    goto retry_same_buf;
                }
                else
                {
                    BlockCopy(frameBuffer, frameCount, buf, 0, count);
                    frameCount += count;
                    return;
                }
            }

            if(bOrderServerToClient)
                CheckOneFrameServerToClient(frameBuffer, frameCount);
            else
                CheckOneFrameClientToServer(frameBuffer, frameCount);

            frameCount = 0;
        }

        // 정상적으로 받은 버퍼를 분석한다.
        void ExecuteOneBuffer(byte[] buf)
        {
            byte[] retn = SetValue(buf);

            ServerToClient(retn);
        }

        const int MAX_ONE_FRAME = 4096;
        int    sendingCount = 0;
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

                pDevice.Write(imsi, 0, total_size + 5);

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
            int remain = sendingBuffer.Length-sendingCount;

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

            pDevice.Write(imsi, 0, size + 13);

            sendingCount += size;

            if (sendingCount >= total_size)
            {
                bOrderServerToClient = false;
                frameCount = 0;
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

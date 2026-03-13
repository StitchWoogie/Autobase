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
    public class ProtocolStreamClient : ProtocolCommon
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

        const int nClientVersionMajor = 1;
        const int nClientVersionMinor = 0;
        const int nClientVersionBuild = 0;
        const int nClientVersionRevision = 0;

        int nServerVersionMajor = 1;
        int nServerVersionMinor = 0;
        int nServerVersionBuild = 0;
        int nServerVersionRevision = 0;

        const int MAX_ONE_FRAME = 4096;

        public ProtocolStreamClient()
        {
            bUseBlockReceive = true;
        }

        public override void ProtocolInit()
        {
            bVersionDownloaded = false;
            nVersionDownloadTry = 0;
            bOrderServerToClient = false;
            frameCount = 0;

            base.ProtocolInit();
        }

        void BlockCopy(byte[] target, int pos_t, byte[] source, int pos_s, int size)
        {
            for (int i = 0; i < size; i++)
            {
                target[pos_t + i] = source[pos_s + i];
            }
        }

        bool bOrderServerToClient = false;

        bool ClientToServer(byte[] buffer)
        {
            bOrderServerToClient = false;

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
                imsi[4] = (byte)(EnumCommand.CS_SingleBlock);

                BlockCopy(imsi, 5, buffer, buf_pos, total_size);

                pDevice.Write(imsi, 0, total_size + 5);

                return true;
            }

            int MAX_ONE_DATA_ZONE_MULTI = MAX_ONE_FRAME - 13;  // size(2)+tns(2)+command(1)+total_size(4)+current_pos(4)
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

                imsi[0] = (byte)((size + 13) % 256);
                imsi[1] = (byte)((size + 13) / 256);
                imsi[2] = (byte)(tns % 256);
                imsi[3] = (byte)(tns / 256);
                imsi[4] = (byte)(EnumCommand.CS_MultiBlock);

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

        bool WaitServerToClient()
        {
            bOrderServerToClient = true;

            EnumProtocolReturnCode retn;

            while (true)
            {
                retn = WaitReceiveFrame();

                if (retn == EnumProtocolReturnCode.OK)
                {
                    return true;
                }
                else if (retn == EnumProtocolReturnCode.Waiting)
                {

                }
                else
                {
                    return false;
                }
            }
        }

        int nVersionDownloadTry = 0;
        bool bVersionDownloaded = false;
        // 버전을 제일 먼저 체크 해야한다.
        bool CheckVersion()
        {
            if (bVersionDownloaded) return true;

            if (nVersionDownloadTry >= 5)
            {
                sErrorMessage = "Server not supported Version Command.";
                return false;
            }

            nVersionDownloadTry++;

            int tns = GetNewTransactionNumber();

            int total_size = 13;
            byte[] imsi = new byte[total_size];

            imsi[0] = (byte)((total_size) % 256);
            imsi[1] = (byte)((total_size) / 256);
            imsi[2] = (byte)(tns % 256);
            imsi[3] = (byte)(tns / 256);
            imsi[4] = (byte)(EnumCommand.CS_SetVersion);
            imsi[5] = (byte)(nClientVersionMajor % 256);
            imsi[6] = (byte)(nClientVersionMajor / 256);
            imsi[7] = (byte)(nClientVersionMinor % 256);
            imsi[8] = (byte)(nClientVersionMinor / 256);
            imsi[9] = (byte)(nClientVersionBuild % 256);
            imsi[10] = (byte)(nClientVersionBuild / 256);
            imsi[11] = (byte)(nClientVersionRevision % 256);
            imsi[12] = (byte)(nClientVersionRevision / 256);

            pDevice.Write(imsi, 0, total_size);

            EnumProtocolReturnCode retn = WaitReceiveFrame();

            if (retn != EnumProtocolReturnCode.OK) return false;

            return true;
        }

        protected Object thisLock = new Object();

        public bool SendRecvBlock(byte[] send_buffer, out byte[] recv_buffer)
        {
            lock (thisLock)
            {
                recv_buffer = null;

                if (!CheckVersion()) return false;

                if (!ClientToServer(send_buffer)) return false;
                if (!WaitServerToClient()) return false;

                recv_buffer = receivedBuffer;
            }

            return true;
        }

        ushort nTransactionNumber = 0;
        ushort GetNewTransactionNumber()
        {
            nTransactionNumber++;
            return nTransactionNumber;
        }

        int frameCount = 0;
        byte[] frameBuffer = new byte[10000];

        void ErrorAndReset(string msg)
        {
            sErrorMessage = msg;
            eReturnCode = EnumProtocolReturnCode.Error;
            bReceiveWaiting = false;
            bOrderServerToClient = false;
            frameCount = 0;
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

            if (bOrderServerToClient)
                ExecuteOneBlockServerToClient(frameBuffer, frameCount);
            else
                ExecuteOneBlockClientToServer(frameBuffer, frameCount);

            frameCount = 0;
        }

        void ExecuteOneBlockClientToServer(byte[] buf, int count)
        {
            int tns = buf[2] + buf[3] * 256;
            byte command = buf[4];

            if (tns != nTransactionNumber)
            {
                eReturnCode = EnumProtocolReturnCode.Error;
                sErrorMessage = String.Format("tns != nTransactionNumber");

                bReceiveWaiting = false;
                return;
            }

            if (command == (byte)EnumCommand.SC_SetVersion)
            {
                nServerVersionMajor = buf[5] + buf[6] * 256;
                nServerVersionMinor = buf[7] + buf[8] * 256;
                nServerVersionBuild = buf[9] + buf[10] * 256;
                nServerVersionRevision = buf[11] + buf[12] * 256;
                bVersionDownloaded = true;
            }

            eReturnCode = EnumProtocolReturnCode.OK;
            bReceiveWaiting = false;
        }

        void SendACK(int tns)
        {
            byte[] buf = new byte[5];

            buf[0] = (byte)(buf.Length % 256);
            buf[1] = (byte)(buf.Length / 256);
            buf[2] = (byte)(tns % 256);
            buf[3] = (byte)(tns / 256);
            buf[4] = (byte)EnumCommand.Ack;

            pDevice.Write(buf, 0, 5);
            //Thread.Sleep(0);
        }

        byte[] receivedBuffer = null;
        int receivedCount = 0;
        int receivedTotalLength = 0;

        void ExecuteOneBlockServerToClient(byte[] buf, int count)
        {
            int tns = buf[2] + buf[3] * 256;
            byte command = buf[4];

            if (command == (byte)EnumCommand.SC_SingleBlock)
            {
                int data_size = count - 5;

                receivedBuffer = new byte[data_size];
                BlockCopy(receivedBuffer, 0, buf, 5, data_size);
                receivedTotalLength = data_size;

                eReturnCode = EnumProtocolReturnCode.OK;
                bReceiveWaiting = false;
            }
            else if (command == (byte)EnumCommand.SC_MultiBlock)
            {
                int total_length = (buf[5] << 0) | (buf[6] << 8) | (buf[7] << 16) | (buf[8] << 24);
                int buf_pos = (buf[9] << 0) | (buf[10] << 8) | (buf[11] << 16) | (buf[12] << 24);

                // 버퍼 위치가 클수는 없다. 이상한 코드일 가능성이 있다.
                if (buf_pos >= total_length)
                {
                    ErrorAndReset("buf_pos >= total_length");
                }

                if (buf_pos == 0)
                {
                    //bOrderServerToClient = false;

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
                    }

                    // 받는 중간에 총 크기가 변경되었다.
                    if (total_length != receivedTotalLength)
                    {
                        ErrorAndReset("total_length != receivedTotalLength");
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
                    eReturnCode = EnumProtocolReturnCode.OK;
                    bReceiveWaiting = false;
                }
                else
                {
                    SendACK(tns);
                    eReturnCode = EnumProtocolReturnCode.Waiting;
                    bReceiveWaiting = false;
                }
            }
        }

    }
}

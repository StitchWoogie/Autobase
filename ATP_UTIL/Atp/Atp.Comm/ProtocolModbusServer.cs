using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ats;
using System.IO;
using NetTools;

namespace Ats.Comm
{
    public class ProtocolModbusServer : ProtocolCommon
    {
        public int nStation = 0;
        public bool bUseHeader = false;

        int commCountNeed = 8;
        bool bReadSizeflag = false;

        int nCommand;
        int nStartAddr;
        int nRegistersSize;

        int MAX_TAG_LIST = 10000;

        class CommandProperty
        {
            public byte command;
            public int RequestCount;    // 
            public bool RequestN;       // N 이 있는 명령

            // 485를 사용할 때는 RTU 가 응답하는 코드도 보이게 된다. 이런 경우 해석을 할 필요가 있다. 아직 안되어 있음
            //public int ResponseCount;
            //public bool ResponseN;
        }

        static List<CommandProperty> arrayCommand = new List<CommandProperty>();

        static void AddOneCommand(byte command, int request, bool requestN, int response, bool responseN)
        {
            CommandProperty c = new CommandProperty();

            c.command = command;
            c.RequestCount = request;
            c.RequestN = requestN;
            //c.ResponseCount = response;
            //c.ResponseN = responseN;

            arrayCommand.Add(c);
        }

        static ProtocolModbusServer()
        {
            AddOneCommand(0x01, 5, false, 2, true);
            AddOneCommand(0x02, 5, false, 2, true);
            AddOneCommand(0x03, 5, false, 2, true);
            AddOneCommand(0x04, 5, false, 2, true);

            AddOneCommand(0x05, 5, false, 5, false);
            AddOneCommand(0x06, 5, false, 5, false);

            AddOneCommand(0x10, 6, true, 5, false);
        }

        void RemoveOneCodeFromBuffer()
        {
            for (int i = 0; i < nRecvCount-1; i++)
            {
                recv_buffer[i] = recv_buffer[i+1];
            }

            nRecvCount--;
        }

        bool SeekValidPacketSerial()
        {
            retry:

            if (nRecvCount < 4) return false;           // 제일 작은 길이는 Command7 등과 같이 Command만 있는 코드이다.   Total(4) = ST(1) + Command(1) + CRC(2)

            for (int i = 0; i < arrayCommand.Count; i++)
            {
                if (arrayCommand[i].command != recv_buffer[1]) continue;

                if (arrayCommand[i].RequestN)
                {
                    int npos = arrayCommand[i].RequestCount;

                    if (nRecvCount < arrayCommand[i].RequestCount + 3)
                    {
                        return false;
                    }
                    if (nRecvCount < arrayCommand[i].RequestCount + 3 + recv_buffer[npos])
                    {
                        return false;
                    }
                    if (nRecvCount == arrayCommand[i].RequestCount + 3 + recv_buffer[npos])
                    {
                        ushort crc = ClassMakeCrcData.GetCRC_16_15_2_1(recv_buffer, nRecvCount - 2);
                        if (crc == recv_buffer[nRecvCount - 2] * 256 + recv_buffer[nRecvCount - 1])
                        {
                            // Station mismatched.
                            if (recv_buffer[0] != nStation)
                            {
                                nRecvCount = 0;
                                return false;
                            }

                            return true;
                        }

                        RemoveOneCodeFromBuffer();

                        return false;
                    }
                }
                else
                {
                    if (nRecvCount < arrayCommand[i].RequestCount + 3)
                    {
                        return false;
                    }

                    if (nRecvCount == arrayCommand[i].RequestCount + 3)
                    {
                        ushort crc = ClassMakeCrcData.GetCRC_16_15_2_1(recv_buffer, nRecvCount - 2);
                        if (crc == recv_buffer[nRecvCount - 2] * 256 + recv_buffer[nRecvCount - 1])
                        {
                            // Station mismatched.
                            if (recv_buffer[0] != nStation)
                            {
                                nRecvCount = 0;
                                return false;
                            }

                            return true;
                        }

                        RemoveOneCodeFromBuffer();

                        return false;
                    }
                }

                RemoveOneCodeFromBuffer();
                goto retry;
            }

            RemoveOneCodeFromBuffer();
            goto retry;

            //return false;
        }

        bool SeekValidPacketTcpip()
        {
            int header_size = 6;
        retry:

            if (nRecvCount < header_size + 2) return false;           // 제일 작은 길이는 Command7 등과 같이 Command만 있는 코드이다.   Total(8) = Header(6) + ST(1) + Command(1)

            for (int i = 0; i < arrayCommand.Count; i++)
            {
                if (arrayCommand[i].command != recv_buffer[header_size+1]) continue;

                if (arrayCommand[i].RequestN)
                {
                    int npos = arrayCommand[i].RequestCount+header_size;

                    if (nRecvCount < arrayCommand[i].RequestCount + header_size+1)
                    {
                        return false;
                    }
                    if (nRecvCount < arrayCommand[i].RequestCount + header_size + 1 + recv_buffer[npos])
                    {
                        return false;
                    }
                    if (nRecvCount == arrayCommand[i].RequestCount + header_size + 1 + recv_buffer[npos])
                    {
                        int packet_size = recv_buffer[4] * 256 + recv_buffer[5];

                        if (packet_size == nRecvCount - 6)
                        {
                            // Station mismatched.
                            if (recv_buffer[header_size+0] != nStation)
                            {
                                nRecvCount = 0;
                                return false;
                            }

                            return true;
                        }

                        RemoveOneCodeFromBuffer();

                        return false;
                    }
                }
                else
                {
                    if (nRecvCount < arrayCommand[i].RequestCount + header_size + 1)
                    {
                        return false;
                    }

                    if (nRecvCount == arrayCommand[i].RequestCount + header_size + 1)
                    {
                        int packet_size = recv_buffer[4] * 256 + recv_buffer[5];

                        if (packet_size == nRecvCount - 6)
                        {
                            // Station mismatched.
                            if (recv_buffer[header_size+0] != nStation)
                            {
                                nRecvCount = 0;
                                return false;
                            }

                            return true;
                        }

                        RemoveOneCodeFromBuffer();

                        return false;
                    }
                }

                RemoveOneCodeFromBuffer();
                goto retry;
            }

            RemoveOneCodeFromBuffer();
            goto retry;

            //return false;
        }

        bool SeekValidPacket()
        {
            if (bUseHeader)
                return SeekValidPacketTcpip();
            else
                return SeekValidPacketSerial();
        }

        public override void ExecuteOneChar(int ch)
        {
            if (nRecvCount >= recv_buffer.Length)
            {
                nRecvCount = 0;
            }

            recv_buffer[nRecvCount] = (byte)ch;

            nRecvCount++;

            if (SeekValidPacket())
            {
                if (IsOnePacketCode())
                {
                    readPacketStartAddrAndReadSize();

                    if (isWriteDataPacket())
                    {
                        saveToReceivedDataAndSendAck();
                    }
                    else
                    {
                        MakeAndSendWantedData();
                    }

                    nRecvCount = 0; // 데이터를 전송하고 나면 쓰레기가 있을 수 있으므로 모든 받은 데이터를 클리어한다.

                    return;
                }
            }
        }

        bool IsOnePacketCodeInModbus()
        {
            if (nRecvCount >= 2)
            {
                switch (recv_buffer[1])
                {
                    case 0x10: break;
                    default: commCountNeed = 8; break;
                }
            }

            if (nRecvCount < commCountNeed) return false;

            int nSize;
            ushort crc = 0;

            switch (recv_buffer[1])
            {
                case 5:
                    break;
                case 6:
                    break;
                case 0x10:
                    if (bReadSizeflag == false)
                    {
                        nSize = recv_buffer[6];
                        commCountNeed = nSize + 9;
                        bReadSizeflag = true;
                        if (nRecvCount < commCountNeed) return false;
                        //pt->timeout->Reset();
                    }
                    break;
            }
            bReadSizeflag = false;
            crc = ClassMakeCrcData.GetCRC_16_15_2_1(recv_buffer, commCountNeed - 2);
            if (crc == recv_buffer[commCountNeed - 2] * 256 + recv_buffer[commCountNeed - 1]) return true;
            nRecvCount = 0;
            return false;
        }

        bool IsOnePacketCodeInModbusTcp()
        {
            if (nRecvCount < 8) return false;

            commCountNeed = recv_buffer[4] * 256 + recv_buffer[5] + 6;

            if (nRecvCount < commCountNeed) return false;

            return true;
        }

        bool IsOnePacketCode()
        {
            if (this.bUseHeader) return this.IsOnePacketCodeInModbusTcp();
            else return this.IsOnePacketCodeInModbus();
        }

        bool readPacketStartAddrAndReadSize()
        {
            if (this.bUseHeader)
            {
                switch (recv_buffer[7])
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:	// Single Coil Write
                    case 6:
                    case 0x10: break;
                    default:
                        MakeAndSendErrorCodeData(1);	// Illegal function
                        return false;
                }

                nCommand = recv_buffer[7];
                nStartAddr = recv_buffer[8] * 256 + recv_buffer[9];

                if (recv_buffer[7] == 6)
                    nRegistersSize = 1;
                else
                    nRegistersSize = recv_buffer[10] * 256 + recv_buffer[11];
            }
            else
            {
                switch (recv_buffer[1])
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 0x10: break;
                    default:
                        MakeAndSendErrorCodeData(1);	// Illegal function
                        return false;
                }

                nCommand = recv_buffer[1];
                nStartAddr = recv_buffer[2] * 256 + recv_buffer[3];
                if (recv_buffer[1] == 6)
                    nRegistersSize = 1;
                else
                    nRegistersSize = recv_buffer[4] * 256 + recv_buffer[5];
            }

            return true;
        }

        bool isWriteDataPacket()
        {
            if (this.bUseHeader)
            {
                switch (recv_buffer[7])
                {
                    case 5:
                    case 6:
                    case 0x10: return true;
                    default: return false;
                }
            }
            else
            {
                switch (recv_buffer[1])
                {
                    case 5:
                    case 6:
                    case 0x10: return true;
                    default: return false;
                }
            }
        }

        void floatDataToByte(ref byte[] data, float val)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(val);
            writer.Close();
        }

        void dwordDataToFloat(ref float fVal, uint val)
        {
            byte[] imsi = new Byte[4];

            MemoryStream stream = new MemoryStream(imsi);
            imsi[0] = (byte)(val & 0xFF);
            imsi[1] = (byte)((val >> 8) & 0xFF);
            imsi[2] = (byte)((val >> 16) & 0xFF);
            imsi[3] = (byte)((val >> 24) & 0xFF);
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                fVal = reader.ReadSingle();
            }
            catch
            {
                fVal = 0;
            }
            reader.Close();
        }

        protected ushort[] WORD_MASK = { 0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0080, 0x0100, 0x0200, 0x0400, 0x0800, 0x1000, 0x2000, 0x4000, 0x8000 };

        void MakeAndSendWantedData()
        {
            int i, pos, buf_pos = 0, nSize;
            ushort crc, val;

            byte[] imsi = new byte[50];
            byte[] commSendBuf = new byte[5000];

            if (nStartAddr + nRegistersSize > MAX_TAG_LIST)
            {
                MakeAndSendErrorCodeData(2);
                return;
            }
            if (nRegistersSize <= 0)
            {
                MakeAndSendErrorCodeData(3);
                return;
            }

            byte fn;

            if (this.bUseHeader)
            {
                fn = recv_buffer[7];

                if (fn == 0x01 || fn == 0x02)
                {
                    commSendBuf[buf_pos++] = recv_buffer[0];
                    commSendBuf[buf_pos++] = recv_buffer[1];
                    commSendBuf[buf_pos++] = recv_buffer[2];
                    commSendBuf[buf_pos++] = recv_buffer[3];
                    commSendBuf[buf_pos++] = recv_buffer[4];
                    commSendBuf[buf_pos++] = recv_buffer[5];
                    commSendBuf[buf_pos++] = (byte)nStation;
                    commSendBuf[buf_pos++] = recv_buffer[7];
                    
                    commSendBuf[buf_pos++] = (byte)((nRegistersSize +7)/8);

                    int byte_size = (nRegistersSize + 7) / 8;

                    pos = nStartAddr;
                    byte bit;
                    byte data_byte = 0;

                    for (i = 0; i < byte_size; i++)
                    {
                        data_byte = 0;
                        for (int j = 0; j < 8; j++, pos++)
                        {
                            bit = (byte)GetTagValueDouble(fn, pos);
                            if (bit > 0)
                                data_byte |= (byte)WORD_MASK[j];
                        }
                        commSendBuf[buf_pos++] = data_byte;				
                    }

                    commSendBuf[4] = (byte)((buf_pos - 6) / 256);
                    commSendBuf[5] = (byte)((buf_pos - 6) % 256);

                    SendBytes(commSendBuf, buf_pos);
                    nRecvCount = 0;
                }
                else
                {
                    commSendBuf[buf_pos++] = recv_buffer[0];
                    commSendBuf[buf_pos++] = recv_buffer[1];
                    commSendBuf[buf_pos++] = recv_buffer[2];
                    commSendBuf[buf_pos++] = recv_buffer[3];
                    commSendBuf[buf_pos++] = recv_buffer[4];
                    commSendBuf[buf_pos++] = recv_buffer[5];
                    commSendBuf[buf_pos++] = (byte)nStation;
                    commSendBuf[buf_pos++] = recv_buffer[7];
                    commSendBuf[buf_pos++] = (byte)(nRegistersSize * 2);
                    nSize = nRegistersSize;

                    if (nSize >= 256) nSize = 255;

                    for (i = 0; i < nSize; i++)
                    {
                        pos = (i + nStartAddr) % MAX_TAG_LIST;

                        val = (ushort)GetTagValueDouble(fn, pos);
                        commSendBuf[buf_pos++] = (byte)(val / 256);
                        commSendBuf[buf_pos++] = (byte)(val);
                    }

                    commSendBuf[4] = (byte)((buf_pos - 6) / 256);
                    commSendBuf[5] = (byte)((buf_pos - 6) % 256);

                    SendBytes(commSendBuf, buf_pos);
                    nRecvCount = 0;
                }
            }
            else
            {
                fn = recv_buffer[1];

                if (fn == 0x01 || fn == 0x02)
                {
                    commSendBuf[buf_pos++] = (byte)nStation;
                    commSendBuf[buf_pos++] = recv_buffer[1];

                    commSendBuf[buf_pos++] = (byte)((nRegistersSize + 7) / 8);

                    int byte_size = (nRegistersSize + 7) / 8;

                    pos = nStartAddr;
                    byte bit;
                    byte data_byte = 0;

                    for (i = 0; i < byte_size; i++)
                    {
                        data_byte = 0;
                        for (int j = 0; j < 8; j++, pos++)
                        {
                            bit = (byte)GetTagValueDouble(fn, pos);
                            if (bit > 0)
                                data_byte |= (byte)WORD_MASK[j];
                        }
                        commSendBuf[buf_pos++] = data_byte;
                    }

                    crc = ClassMakeCrcData.GetCRC_16_15_2_1(commSendBuf, buf_pos);
                    commSendBuf[buf_pos++] = (byte)(crc / 256);		// CRC
                    commSendBuf[buf_pos++] = (byte)(crc);

                    SendBytes(commSendBuf, buf_pos);
                    nRecvCount = 0;
                }
                else
                {
                    commSendBuf[buf_pos++] = (byte)nStation;
                    commSendBuf[buf_pos++] = recv_buffer[1];
                    commSendBuf[buf_pos++] = (byte)(nRegistersSize * 2);
                    nSize = nRegistersSize;

                    if (nSize >= 256) nSize = 255;

                    for (i = 0; i < nSize; i++)
                    {
                        pos = (i + nStartAddr) % MAX_TAG_LIST;

                        val = (ushort)GetTagValueDouble(fn, pos);
                        commSendBuf[buf_pos++] = (byte)(val / 256);
                        commSendBuf[buf_pos++] = (byte)(val);

                    }
                    crc = ClassMakeCrcData.GetCRC_16_15_2_1(commSendBuf, buf_pos);
                    commSendBuf[buf_pos++] = (byte)(crc / 256);		// CRC
                    commSendBuf[buf_pos++] = (byte)(crc);

                    SendBytes(commSendBuf, buf_pos);
                    nRecvCount = 0;
                }
            }

        }

        void MakeAndSendErrorCodeData(byte code)
        {
            int buf_pos = 0;
            ushort crc;
            byte[] commSendBuf = new byte[50];

            if (this.bUseHeader)
            {
                commSendBuf[buf_pos++] = recv_buffer[0];
                commSendBuf[buf_pos++] = recv_buffer[1];
                commSendBuf[buf_pos++] = recv_buffer[2];
                commSendBuf[buf_pos++] = recv_buffer[3];
                commSendBuf[buf_pos++] = 0;
                commSendBuf[buf_pos++] = 3;
                commSendBuf[buf_pos++] = (byte)nStation;
                commSendBuf[buf_pos++] = (byte)(recv_buffer[7] | 0x80);
                commSendBuf[buf_pos++] = code;
                SendBytes(commSendBuf, buf_pos);
                nRecvCount = 0;
            }
            else
            {
                commSendBuf[buf_pos++] = (byte)nStation;
                commSendBuf[buf_pos++] = (byte)(recv_buffer[1] | 0x80);
                commSendBuf[buf_pos++] = code;

                crc = ClassMakeCrcData.GetCRC_16_15_2_1(commSendBuf, buf_pos);
                commSendBuf[buf_pos++] = (byte)(crc / 256);		// CRC
                commSendBuf[buf_pos++] = (byte)(crc);
                SendBytes(commSendBuf, buf_pos);
                nRecvCount = 0;
            }
        }

        void MakeWriteAckDataAndSend()
        {
            int buf_pos = 0;
            ushort crc;
            byte[] commSendBuf = new byte[50];

            if (this.bUseHeader)
            {
                commSendBuf[buf_pos++] = recv_buffer[0];
                commSendBuf[buf_pos++] = recv_buffer[1];
                commSendBuf[buf_pos++] = recv_buffer[2];
                commSendBuf[buf_pos++] = recv_buffer[3];
                commSendBuf[buf_pos++] = 0;
                commSendBuf[buf_pos++] = 6;
                commSendBuf[buf_pos++] = (byte)nStation;
                commSendBuf[buf_pos++] = recv_buffer[7];
                commSendBuf[buf_pos++] = (byte)(nStartAddr / 256);
                commSendBuf[buf_pos++] = (byte)(nStartAddr);
                if (nCommand == 0x10)
                {
                    commSendBuf[buf_pos++] = recv_buffer[nRegistersSize / 256];
                    commSendBuf[buf_pos++] = recv_buffer[nRegistersSize % 256];
                }
                else
                {
                    commSendBuf[buf_pos++] = recv_buffer[10];
                    commSendBuf[buf_pos++] = recv_buffer[11];
                }

                SendBytes(commSendBuf, buf_pos);
                nRecvCount = 0;
            }
            else
            {
                commSendBuf[buf_pos++] = (byte)nStation;
                commSendBuf[buf_pos++] = recv_buffer[1];
                commSendBuf[buf_pos++] = (byte)(nStartAddr / 256);
                commSendBuf[buf_pos++] = (byte)(nStartAddr);
                if (nCommand == 0x10)
                {
                    commSendBuf[buf_pos++] = recv_buffer[nRegistersSize / 256];
                    commSendBuf[buf_pos++] = recv_buffer[nRegistersSize % 256];
                }
                else
                {
                    commSendBuf[buf_pos++] = recv_buffer[4];
                    commSendBuf[buf_pos++] = recv_buffer[5];
                }
                crc = ClassMakeCrcData.GetCRC_16_15_2_1(commSendBuf, buf_pos);
                commSendBuf[buf_pos++] = (byte)(crc / 256);		// CRC
                commSendBuf[buf_pos++] = (byte)(crc);

                SendBytes(commSendBuf, buf_pos);
                nRecvCount = 0;
            }
        }

        void saveToReceivedDataAndSendAck()
        {
            int i, nSize;
            int start = this.bUseHeader ? 6 : 0;
            byte fn = recv_buffer[start + 1];

            if (fn == 5) 											// Write Single Coil
            {
                if (recv_buffer[start + 4] == 0 && recv_buffer[start + 5] == 0)
                    SetTagValue(fn, nStartAddr, 0);
                else	// 0xFF00
                    SetTagValue(fn, nStartAddr, 1);
            }
            else if (fn == 6)											// Function 6 = 항상 워드
            {
                SetTagValue(fn, nStartAddr, recv_buffer[start + 4] * 256u + recv_buffer[start + 5]); //PokeWORD(pt, localVars->nStartAddr, pt->recv_buffer[4]*256u+(BYTE)pt->recv_buffer[5]);				
            }
            else if (fn == 16)// 0x10 Write Multiple registers
            {
                // Request
                // Function code 1 Byte 0x10
                // Starting Address 2 Bytes 0x0000 to 0xFFFF
                // Quantity of Registers 2 Bytes 0x0001 to 0x0078
                // Byte Count 1 Byte 2 x N*
                // Registers Value N* x 2 Bytes value

                // Response
                // Function code 1 Byte 0x10
                // Starting Address 2 Bytes 0x0000 to 0xFFFF
                // Quantity of Registers 2 Bytes 1 to 123 (0x7B)

                nSize = nRegistersSize;

                for (i = 0; i < nSize; i++)
                {
                    SetTagValue(fn, nStartAddr + i, recv_buffer[start + 7 + i * 2] * 256u + recv_buffer[start + 8 + i * 2]); //PokeWORD(pt, localVars->nStartAddr+i, pt->recv_buffer[7+i*2]*256u+(BYTE)pt->recv_buffer[8+i*2]);
                }
            }
            MakeWriteAckDataAndSend();
            commCountNeed = 8;
        }

        public virtual void SetValue(byte fn, int address, object value)
        {

        }

        public virtual object GetValue(byte fn, int address)
        {
            return null;
        }

        public double GetTagValueDouble(byte fn, int address)
        {
            object value = GetValue(fn, address);

            if (value == null) return 0;

            if (value.GetType() == typeof(ushort))
                return (ushort)value;
            else if (value.GetType() == typeof(uint))
                return (uint)value;
            else if (value.GetType() == typeof(ulong))
                return (ulong)value;
            else if (value.GetType() == typeof(float))
                return (uint)value;
            else if (value.GetType() == typeof(double))
                return (uint)value;
            else if (value.GetType() == typeof(string))
                return ConvertTool.ToDouble((string)value);
            else
            {
                return Convert.ToDouble(value);
            }
        }

        void SendBytes(byte[] buf, int size)
        {
            pDevice.Write(buf, 0, size);
        }

        bool SetTagValue(byte fn, int address, object val)
        {
            SetValue(fn, address, val);

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;

namespace Ats.Comm
{
    public class ProtocolAobClient : ProtocolCommon
    {
        public static readonly int nVersionMajor = 2;
        public static readonly int nVersionMinor = 0;
        public static readonly int nVersionBuild = 0;
        public static readonly int nVersionRevision = 0;

        ushort nTransactionNumber = 0;
        public bool bBinaryProtocol = false;

        protected Object thisLock = new Object();

        ushort GetNewTransactionNumber()
        {
            nTransactionNumber++;
            return nTransactionNumber;
        }

        ushort GetCRC(byte[] data, int start, int size)
        {
            ushort crc = 0;

            for (int i = 0, pos = start; i < size; i++, pos++)
            {
                crc += data[pos];
            }

            return crc;
        }

        // 데이터중에 DLE가 있으면 하나더 추가한다.
        void AddDataByte(byte[] data, ref int count, ref ushort crc, byte ch)
        {
            if (ch == 0x10)
                data[count++] = 0x10;

            data[count++] = ch;
            crc += ch;
        }

        protected void SendCommon(int station, string command, byte[] block, int block_size)
        {
            pDevice.Clear();
            ClearRecvCount();

            int total_size;
            byte[] data = new byte[block_size*2+18*2];
            
            int count = 0;
            string buf;

            if (!bBinaryProtocol)
            {
                total_size = block_size*2 + 18;

                data[count++] = 0x02;   // STX
                buf = String.Format("{0:X04}", total_size);
                data[count++] = (byte)(buf[0]);
                data[count++] = (byte)(buf[1]);
                data[count++] = (byte)(buf[2]);
                data[count++] = (byte)(buf[3]);

                ushort tns_no = GetNewTransactionNumber();
                buf = String.Format("{0:X04}", tns_no);
                data[count++] = (byte)(buf[0]);
                data[count++] = (byte)(buf[1]);
                data[count++] = (byte)(buf[2]);
                data[count++] = (byte)(buf[3]);

                buf = String.Format("{0:X02}", station);
                data[count++] = (byte)(buf[0]);
                data[count++] = (byte)(buf[1]);

                data[count++] = (byte)(command[0]);
                data[count++] = (byte)(command[1]);

                for (int i = 0; i < block_size; i++)
                {
                    buf = String.Format("{0:X02}", block[i]);
                    data[count++] = (byte)(buf[0]);
                    data[count++] = (byte)(buf[1]);
                }

                ushort crc = GetCRC(data, 1, count - 1);
                buf = String.Format("{0:X04}", crc);
                data[count++] = (byte)(buf[0]);
                data[count++] = (byte)(buf[1]);
                data[count++] = (byte)(buf[2]);
                data[count++] = (byte)(buf[3]);
                data[count++] = 0x03;   // ETX
            }
            else
            {
                total_size = block_size + 11;

                ushort crc = 0;

                // 
                data[count++] = 0x10;   // DLE
                data[count++] = 0x02;   // STX

                AddDataByte(data, ref count, ref crc, (byte)(total_size / 256));
                AddDataByte(data, ref count, ref crc, (byte)(total_size % 256));

                ushort tns_no = GetNewTransactionNumber();

                AddDataByte(data, ref count, ref crc, (byte)(tns_no / 256));
                AddDataByte(data, ref count, ref crc, (byte)(tns_no % 256));

                AddDataByte(data, ref count, ref crc, (byte)station);

                AddDataByte(data, ref count, ref crc, (byte)command[0]);
                AddDataByte(data, ref count, ref crc, (byte)command[1]);

                for (int i = 0; i < block_size; i++)
                {
                    AddDataByte(data, ref count, ref crc, block[i]);
                }

                ushort crc_save = crc;

                AddDataByte(data, ref count, ref crc, (byte)(crc_save / 256));
                AddDataByte(data, ref count, ref crc, (byte)(crc_save % 256));

                data[count++] = 0x10;   // DLE
                data[count++] = 0x03;   // ETX
            }

            pDevice.Write(data, 0, count);
        }

        public EnumProtocolReturnCode SendVersion(int station)
        {
            byte[] data = new byte[16];

            int count = 0;

            data[count++] = (byte)(nVersionMajor / 256);
            data[count++] = (byte)(nVersionMajor % 256);

            data[count++] = (byte)(nVersionMinor / 256);
            data[count++] = (byte)(nVersionMinor % 256);

            data[count++] = (byte)(nVersionBuild / 256);
            data[count++] = (byte)(nVersionBuild % 256);

            data[count++] = (byte)(nVersionRevision / 256);
            data[count++] = (byte)(nVersionRevision % 256);

            lock (thisLock)
            {
                SendCommon(station, "SV", data, count);

                return WaitReceiveFrame();
            }
        }

        public EnumProtocolReturnCode ReadBlock(int station, int table_no, int address, int size, string command)
        {
            byte[] data = new byte[16];

            int count = 0;

            data[count++] = (byte)(table_no);

            data[count++] = (byte)(address / 256);
            data[count++] = (byte)(address % 256);

            data[count++] = (byte)(size / 256);
            data[count++] = (byte)(size % 256);

            lock (thisLock)
            {
                SendCommon(station, command, data, count);

                return WaitReceiveFrame();
            }
        }

        public EnumProtocolReturnCode WriteNumericBlock(int station, int table_no, int address, int size, string[] value, string command)
        {
            //WaitIdle();

            byte[] data = new byte[10 + size * 16];

            int count = 0;

            data[count++] = (byte)(table_no);

            data[count++] = (byte)(address / 256);
            data[count++] = (byte)(address % 256);

            data[count++] = (byte)(size / 256);
            data[count++] = (byte)(size % 256);

            for (int i = 0; i < size; i++)
            {
                if (command == "WD")
                {
                    uint u = ConvertTool.ToUInt32(value[i]);

                    data[count++] = (byte)((u >> 24) & 0xFF);
                    data[count++] = (byte)((u >> 16) & 0xFF);
                    data[count++] = (byte)((u >> 8) & 0xFF);
                    data[count++] = (byte)((u >> 0) & 0xFF);
                }
                else if (command == "WF")
                {
                    float f = ConvertTool.ToSingle(value[i]);

                    uint u = FloatingTool.FloatToUint(f);

                    data[count++] = (byte)((u >> 24) & 0xFF);
                    data[count++] = (byte)((u >> 16) & 0xFF);
                    data[count++] = (byte)((u >> 8) & 0xFF);
                    data[count++] = (byte)((u >> 0) & 0xFF);
                }
                else if (command == "WL")
                {
                    ulong u = ConvertTool.ToUInt64(value[i]);

                    data[count++] = (byte)((u >> 56) & 0xFF);
                    data[count++] = (byte)((u >> 48) & 0xFF);
                    data[count++] = (byte)((u >> 40) & 0xFF);
                    data[count++] = (byte)((u >> 32) & 0xFF);
                    data[count++] = (byte)((u >> 24) & 0xFF);
                    data[count++] = (byte)((u >> 16) & 0xFF);
                    data[count++] = (byte)((u >> 8) & 0xFF);
                    data[count++] = (byte)((u >> 0) & 0xFF);
                }
                else if (command == "WU")
                {
                    double f = ConvertTool.ToDouble(value[i]);

                    ulong u = FloatingTool.DoubleToUlong(f);

                    data[count++] = (byte)((u >> 56) & 0xFF);
                    data[count++] = (byte)((u >> 48) & 0xFF);
                    data[count++] = (byte)((u >> 40) & 0xFF);
                    data[count++] = (byte)((u >> 32) & 0xFF);
                    data[count++] = (byte)((u >> 24) & 0xFF);
                    data[count++] = (byte)((u >> 16) & 0xFF);
                    data[count++] = (byte)((u >> 8) & 0xFF);
                    data[count++] = (byte)((u >> 0) & 0xFF);
                }
                else
                {
                    ushort u = ConvertTool.ToUInt16(value[i]);

                    data[count++] = (byte)((u >> 8) & 0xFF);
                    data[count++] = (byte)((u >> 0) & 0xFF);
                }
            }

            lock (thisLock)
            {
                SendCommon(station, command, data, count);

                return WaitReceiveFrame();
            }
        }

        public EnumProtocolReturnCode WriteStringBlock(int station, int table_no, int address, int size, string[] value, string command)
        {
            //WaitIdle();

            byte[] data = new byte[0x10000];

            int count = 0;

            data[count++] = (byte)(table_no);

            data[count++] = (byte)(address / 256);
            data[count++] = (byte)(address % 256);

            data[count++] = (byte)(size / 256);
            data[count++] = (byte)(size % 256);

            for (int i = 0; i < size; i++)
            {
                if (command == "WT")
                {
                    string s = (string)value[i];

                    byte[] msg_bytes = ConvertString.StringToUtf8Bytes(s);

                    data[count++] = (byte)(msg_bytes.Length / 256);
                    data[count++] = (byte)(msg_bytes.Length % 256);

                    for (int j = 0; j < msg_bytes.Length; j++)
                    {
                        data[count++] = msg_bytes[j];
                    }
                }
            }

            lock (thisLock)
            {
                SendCommon(station, command, data, count);

                return WaitReceiveFrame();
            }
        }

        public EnumProtocolReturnCode WriteBit(int station, int table_no, int address, int bit_pos, int value)
        {
            //WaitIdle();

            byte[] data = new byte[16];

            int count = 0;

            data[count++] = (byte)(table_no);

            data[count++] = (byte)(address / 256);
            data[count++] = (byte)(address % 256);

            data[count++] = (byte)(bit_pos);

            data[count++] = (byte)(value);

            lock (thisLock)
            {
                SendCommon(station, "WB", data, count);

                return WaitReceiveFrame();
            }
        }

        public EnumProtocolReturnCode SystemGetConfiguration(int station, int command)
        {
            byte[] data = new byte[16];

            int count = 0;

            data[count++] = (byte)(command);

            lock (thisLock)
            {
                SendCommon(station, "SG", data, count);

                return WaitReceiveFrame();
            }
        }

        public EnumProtocolReturnCode SystemSetConfiguration(int station, int item, int size, byte[] value)
        {
            byte[] data = new byte[6 + size * 2];

            int count = 0;

            data[count++] = (byte)(item);

            data[count++] = (byte)(size / 256);
            data[count++] = (byte)(size % 256);

            for (int i = 0; i < size; i++)
            {
                data[count++] = value[i];
            }

            lock (thisLock)
            {
                SendCommon(station, "SS", data, count);

                return WaitReceiveFrame();
            }
        }

        public EnumProtocolReturnCode SystemGetInformation(int station, int item)
        {
            byte[] data = new byte[16];

            int count = 0;

            data[count++] = (byte)(item);

            lock (thisLock)
            {
                SendCommon(station, "SI", data, count);

                return WaitReceiveFrame();
            }
        }

        public override void ExecuteOneChar(int ch)
        {
            if(bBinaryProtocol)
                ExecuteOneCharProtocolBinary(ch);
            else
                ExecuteOneCharProtocolAscii(ch);
        }

        void ExecuteOneCharProtocolAscii(int ch)
        {
            if (nRecvCount >= recv_buffer.Length)
            {
                nRecvCount = 0;
            }

            if (ch == 0x02)
            {
                nRecvCount = 0;
            }

            recv_buffer[nRecvCount++] = (byte)ch;

            if (ch == 0x03)
            {
                if (CheckFrameProtocolAscii(recv_buffer, nRecvCount))
                {
                    eReturnCode = EnumProtocolReturnCode.OK;
                }
                else
                {
                    eReturnCode = EnumProtocolReturnCode.Error;
                }

                bReceiveWaiting = false;

                nRecvCount = 0;
            }
        }

        bool dle_flag;

        void ExecuteOneCharProtocolBinary(int ch)
        {
            if (nRecvCount >= recv_buffer.Length)
            {
                nRecvCount = 0;
                dle_flag = false;
            }

            if (ch == 0x10)
            {
                if (!dle_flag)
                {
                    dle_flag = true;
                }
                else
                {
                    dle_flag = false;
                    recv_buffer[nRecvCount++] = (byte)ch;
                }
                return;
            }

            if (ch == 0x02)
            {
                if (dle_flag)
                {
                    dle_flag = false;
                    nRecvCount = 0;
                }

                recv_buffer[nRecvCount++] = (byte)ch;

                return;
            }

            recv_buffer[nRecvCount++] = (byte)ch;

            if (ch == 0x03)
            {
                if (dle_flag)
                {
                    dle_flag = false;

                    if (CheckFrameProtocolBinary(recv_buffer, nRecvCount))
                    {
                        eReturnCode = EnumProtocolReturnCode.OK;
                    }
                    else
                    {
                        eReturnCode = EnumProtocolReturnCode.Error;
                    }

                    bReceiveWaiting = false;

                    nRecvCount = 0;
                }
                else
                {

                }
            }

        }

        string GetMessage(byte[] data, out int error_code)
        {
            error_code = BinBufToUshort(data, 0);
            int msg_size = BinBufToUshort(data, 2);
            string msg = "";

            if (msg_size > 0)
            {
                msg = ConvertString.Utf8BytesToString(data, 4, msg_size);
            }

            return msg;
        }

        string ErrorCodeToString(int code)
        {
            if (code == 0x0001) return "Command not supported.";
            if (code == 0x0002) return "Sub Command not supported.";
            if (code == 0x0003) return "Station not exists.";
            if (code == 0x0004) return "Table Number not exists.";
            if (code == 0x0005) return "Address not exists.";
            if (code == 0x0006) return "CRC mismatched.";

            return "";
        }

        bool CheckFrameProtocolBinary(byte[] recv_data, int count)
        {
            if (recv_data[0] != 0x02)
            {
                sErrorMessage = String.Format("data[0] != 0x02");
                return false;
            }

            if (recv_data[count - 1] != 0x03)
            {
                sErrorMessage = String.Format("data[count-1] != 0x03");
                return false;
            }

            int length = BinBufToUshort(recv_data, 1);
            if (length != count)
            {
                sErrorMessage = String.Format("total length mismatched.");
                return false;
            }

            int frame_crc = BinBufToUshort(recv_data, count - 3);
            int calc_crc = GetCRC(recv_data, 1, count - 4);

            if (frame_crc != calc_crc)
            {
                sErrorMessage = String.Format("CRC mismatched.");
                return false;
            }

            ushort tns = BinBufToUshort(recv_data, 3);
            if (tns != nTransactionNumber)
            {
                sErrorMessage = String.Format("SEND/RECV TNS number mismatched.");
                return false;
            }

            string command = String.Format("{0}{1}", (char)(recv_data[6]), (char)(recv_data[7]));

            int data_count = (length - 11);

            byte[] data = new byte[data_count];

            for (int i = 0; i < data_count; i++)
            {
                data[i] = recv_data[8+i];
            }

            return CheckFrameCommand(command, data, data_count);
        }

        bool CheckFrameProtocolAscii(byte[] recv_data, int count)
        {
            if (recv_data[0] != 0x02)
            {
                sErrorMessage = String.Format("data[0] != 0x02");
                return false;
            }

            if (recv_data[count - 1] != 0x03)
            {
                sErrorMessage = String.Format("data[count-1] != 0x03");
                return false;
            }

            int length = HexBuf.ToUshort(recv_data, 1);
            if (length != count)
            {
                sErrorMessage = String.Format("total length mismatched.");
                return false;
            }

            int frame_crc = HexBuf.ToUshort(recv_data, count - 5);
            int calc_crc = GetCRC(recv_data, 1, count - 6);

            if (frame_crc != calc_crc)
            {
                sErrorMessage = String.Format("CRC mismatched.");
                return false;
            }

            ushort tns = HexBuf.ToUshort(recv_data, 5);
            if (tns != nTransactionNumber)
            {
                sErrorMessage = String.Format("SEND/RECV TNS number mismatched.");
                return false;
            }

            string command = String.Format("{0}{1}", (char)(recv_data[11]), (char)(recv_data[12]));

            int data_count = (length - 18) / 2;

            byte[] data = new byte[data_count];

            for (int i = 0; i < data_count; i++)
            {
                data[i] = HexBuf.ToByte(recv_data, 13 + i * 2);
            }

            return CheckFrameCommand(command, data, data_count);
        }

        protected byte BinBufToByte(byte[] data, int pos)
        {
            return data[pos];
        }

        protected ushort BinBufToUshort(byte[] data, int pos)
        {
            return (ushort)((data[pos] << 8) | (data[pos+1] << 0));
        }

        uint BinBufToUint(byte[] data, int pos)
        {
            return (uint)((data[pos] << 24) | (data[pos+1] << 16) | (data[pos+2] << 8) | (data[pos + 3] << 0));
        }

        ulong BinBufToUlong(byte[] data, int pos)
        {
            return (ulong)((data[pos] << 56) | (data[pos + 1] << 48) | (data[pos + 2] << 40) | (data[pos + 3] << 32) |
                            (data[pos+4] << 24) | (data[pos + 5] << 16) | (data[pos + 6] << 8) | (data[pos + 7] << 0));
        }

        protected virtual bool CheckUserCommand(string command, byte[] recv_data, int count)
        {
            return false;
        }

        bool CheckFrameCommand(string command, byte[] recv_data, int count)
        {
            if (CheckUserCommand(command, recv_data, count))
                return true;

            if (command == "er")
            {
                int error_code;
                string msg = "";

                msg = GetMessage(recv_data, out error_code);

                sErrorMessage = String.Format("ErrorCode=0x{0:X04}({2}), msg={1}", error_code, msg, ErrorCodeToString(error_code));
            }
            else if (command == "rw" || command == "rd" || command == "rf" || command == "rl" || command == "ru")
            {
                int table_no = BinBufToByte(recv_data, 0);
                int address = BinBufToUshort(recv_data, 1);
                int size = BinBufToUshort(recv_data, 3);

                if (command == "rd")
                {
                    uint[] value_data = new uint[size];

                    for (int i = 0; i < size; i++)
                    {
                        value_data[i] = BinBufToUint(recv_data, 5 + i * 4);
                    }

                    pReturnData = value_data;
                }
                else if (command == "rf")
                {
                    uint u;
                    float[] value_data = new float[size];

                    for (int i = 0; i < size; i++)
                    {
                        u = BinBufToUint(recv_data, 5 + i * 4);
                        value_data[i] = FloatingTool.UintToFloat(u);
                    }

                    pReturnData = value_data;
                }
                else if (command == "rl")
                {
                    ulong[] value_data = new ulong[size];

                    for (int i = 0; i < size; i++)
                    {
                        value_data[i] = BinBufToUlong(recv_data, 5 + i * 8);
                    }

                    pReturnData = value_data;
                }
                else if (command == "ru")
                {
                    ulong u;
                    double[] value_data = new double[size];

                    for (int i = 0; i < size; i++)
                    {
                        u = BinBufToUlong(recv_data, 5 + i * 8);
                        value_data[i] = FloatingTool.UlongToDouble(u);
                    }

                    pReturnData = value_data;
                }
                else
                {
                    ushort[] value_data = new ushort[size];

                    for (int i = 0; i < size; i++)
                    {
                        value_data[i] = BinBufToUshort(recv_data, 5 + i * 2);
                    }

                    pReturnData = value_data;
                }
            }
            else if (command == "rt")
            {
                int table_no = BinBufToByte(recv_data, 0);
                int address = BinBufToUshort(recv_data, 1);
                int size = BinBufToUshort(recv_data, 3);
                int msg_size;
                int pos = 5;

                string msg = "";

                string[] value_data = new string[size];

                for (int i = 0; i < size; i++)
                {
                    msg_size = BinBufToUshort(recv_data, pos);

                    if (msg_size > 0)
                    {
                        byte[] utf_bytes = new byte[msg_size];

                        for (int j = 0; j < msg_size; j++)
                        {
                            utf_bytes[j] = BinBufToByte(recv_data, pos + 2 + j);
                        }

                        string unicode = ConvertString.Utf8BytesToString(utf_bytes, 0, msg_size);

                        msg += String.Format("'{0}', ", unicode);

                        value_data[i] = unicode;
                    }
                    else
                    {
                        value_data[i] = "";
                    }

                    pos += msg_size * 1 + 2;
                }

                sErrorMessage = String.Format("O.K Data={0}", msg);

                pReturnData = value_data;
            }
            else if (command == "ww")
            {
                sErrorMessage = String.Format("O.K Write WORD Block.");
            }
            else if (command == "wb")
            {
                sErrorMessage = String.Format("O.K Write Bit.");
            }
            else if (command == "wd")
            {
                sErrorMessage = String.Format("O.K Write DWORD.");
            }
            else if (command == "wf")
            {
                sErrorMessage = String.Format("O.K Write FLOAT.");
            }
            else if (command == "wl")
            {
                sErrorMessage = String.Format("O.K Write LONG.");
            }
            else if (command == "wu")
            {
                sErrorMessage = String.Format("O.K Write Double.");
            }
            else if (command == "wt")
            {
                sErrorMessage = String.Format("O.K Write Text.");
            }
            else if (command == "sv")
            {
                int version_major = BinBufToUshort(recv_data, 0);
                int version_minor = BinBufToUshort(recv_data, 2);
                int version_build = BinBufToUshort(recv_data, 4);
                int version_revision = BinBufToUshort(recv_data, 6);

                sErrorMessage = String.Format("O.K Slave's AOB Version={0}.{1}.{2}.{3}", version_major, version_minor, version_build, version_revision);

                pReturnData = String.Format("{0}.{1}.{2}.{3}", version_major, version_minor, version_build, version_revision);
            }
            else if (command == "sg")
            {
                byte item = BinBufToByte(recv_data, 0);
                int size = BinBufToUshort(recv_data, 1);

                byte[] data = new byte[size];

                for (int i = 0; i < size; i++)
                {
                    data[i] = BinBufToByte(recv_data, 3 + i);
                }

                pReturnData = data;
            }
            else if (command == "ss")
            {
                //FormMainMaster.thisForm.DrawMesage("O.K System Configuration.");
            }
            else if (command == "si")
            {
                byte item = BinBufToByte(recv_data, 0);
                int size = BinBufToUshort(recv_data, 1);

                if (size > 0)
                {
                    byte[] utf_bytes = new byte[size];

                    for (int j = 0; j < size; j++)
                    {
                        utf_bytes[j] = BinBufToByte(recv_data, 3 + j);
                    }

                    string unicode = ConvertString.Utf8BytesToString(utf_bytes, 0, size);

                    pReturnData = unicode;
                }
            }
            else
            {
                sErrorMessage = String.Format("'{0}' command Unknown.", command);
                return false;
            }

            return true;
        }


    }
}


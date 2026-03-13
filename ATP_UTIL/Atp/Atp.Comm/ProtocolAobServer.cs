using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ats;
using NetTools;

namespace Ats.Comm
{
    public class ProtocolAobServer : ProtocolCommon
    {
        public int nStation = 0;

        static readonly int nVersionMajor = 1;
        static readonly int nVersionMinor = 1;
        static readonly int nVersionBuild = 0;
        static readonly int nVersionRevision = 0;

        public override void ExecuteOneChar(int ch)
        {
            if (nRecvCount >= recv_buffer.Length)
            {
                nRecvCount = 0;
            }

            if (ch == 0x02)
            {
                nRecvCount = 0;

            }

            recv_buffer[nRecvCount] = (byte)ch;

            nRecvCount++;

            if (ch == 0x03)
            {
                //AddCommCode(EnumCommCode.RecvNextLine, 0);
                CheckFrame(recv_buffer, nRecvCount);

                // 데이터가 연속해서 들어왔을 수 있으므로 받은 버퍼를 클리어한다.
                nRecvCount = 0;
            }
        }

        void DrawMessage(string format, params object[] arg)
        {

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

        bool CheckFrame(byte[] data, int count)
        {
            if (recv_buffer[0] != 0x02)
            {
                DrawMessage("data[0] != 0x02");
                return false;
            }

            if (recv_buffer[count - 1] != 0x03)
            {
                DrawMessage("data[count-1] != 0x03");
                return false;
            }

            int length = HexBuf.ToUshort(data, 1);
            if (length != count)
            {
                DrawMessage("total length mismatched.");
                return false;
            }

            int frame_crc = HexBuf.ToUshort(data, count - 5);
            int calc_crc = GetCRC(data, 1, count - 6);

            if (frame_crc != calc_crc)
            {
                DrawMessage("CRC mismatched.");
                return false;
            }

            int station = HexBuf.ToByte(data, 9);
            if (station != nStation)
            {
                DrawMessage("Other Station. Command skipped.");
                return false;
            }

            ushort tns_no = HexBuf.ToUshort(data, 5);
            string command = String.Format("{0}{1}", (char)data[11], (char)data[12]);


            if (command == "RW")
            {
                Command_ReadBlock(tns_no, data, command);
            }
            else if (command == "RD")
            {
                Command_ReadBlock(tns_no, data, command);
            }
            else if (command == "RF")
            {
                Command_ReadBlock(tns_no, data, command);
            }
            else if (command == "RL")
            {
                Command_ReadBlock(tns_no, data, command);
            }
            else if (command == "RU")
            {
                Command_ReadBlock(tns_no, data, command);
            }
            else if (command == "RT")
            {
                Command_ReadBlock(tns_no, data, command);
            }
            else if (command == "WW")
            {
                Command_WriteBlock(tns_no, data, command);
            }
            else if (command == "WD")
            {
                Command_WriteBlock(tns_no, data, command);
            }
            else if (command == "WF")
            {
                Command_WriteBlock(tns_no, data, command);
            }
            else if (command == "WL")
            {
                Command_WriteBlock(tns_no, data, command);
            }
            else if (command == "WU")
            {
                Command_WriteBlock(tns_no, data, command);
            }
            else if (command == "WT")
            {
                Command_WriteBlock(tns_no, data, command);
            }
            else if (command == "WB")
            {
                Command_WB(tns_no, data);
            }
            else if (command == "SV")
            {
                Command_SV(tns_no, data);
            }
            else
            {
                string msg = String.Format("Command {0} not supported.", command);
                SendErrorCode(tns_no, 1, msg);
            }

            return true;
        }

        void SendCommon(ushort tns_no, int station, string command, byte[] block, int block_size)
        {
            int total_size = block_size + 18;
            byte[] data = new byte[total_size];

            int count = 0;
            string buf;

            data[count++] = 0x02;   // STX
            buf = String.Format("{0:X04}", total_size);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

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
                data[count++] = block[i];
            }

            ushort crc = GetCRC(data, 1, count - 1);
            buf = String.Format("{0:X04}", crc);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);
            data[count++] = 0x03;   // ETX

            pDevice.Write(data, 0, count);
        }

        void SendErrorCode(ushort tns_no, int code, string msg)
        {
            if (msg == null)
                msg = "";
            byte[] msg_bytes = ConvertString.StringToUtf8Bytes(msg);

            byte[] data = new byte[8 + msg_bytes.Length * 2];

            int count = 0;
            string buf;

            buf = String.Format("{0:X04}", code);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

            buf = String.Format("{0:X04}", msg_bytes.Length);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

            if (msg_bytes.Length > 0)
            {
                for (int i = 0; i < msg_bytes.Length; i++)
                {
                    buf = String.Format("{0:X02}", msg_bytes[i]);
                    data[count++] = (byte)(buf[0]);
                    data[count++] = (byte)(buf[1]);
                }
            }

            SendCommon(tns_no, nStation, "er", data, count);

            DrawMessage("Send Error. Code=0x{0:X04}, Msg={1}", code, msg);
        }

        void Command_ReadBlock(ushort tns_no, byte[] recv_data, string command)
        {
            int table_no = HexBuf.ToByte(recv_data, 13);
            int address = HexBuf.ToUshort(recv_data, 15);
            int size = HexBuf.ToUshort(recv_data, 19);

            byte[] data = new byte[0x10000];

            int count = 0;
            string buf;

            buf = String.Format("{0:X02}", table_no);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);

            buf = String.Format("{0:X04}", address);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

            buf = String.Format("{0:X04}", size);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

            for (int i = 0; i < size; i++)
            {
                if (command == "RD")
                {
                    buf = String.Format("{0:X08}", (uint)GetValueUint64(command, table_no, address + i));
                    data[count++] = (byte)(buf[0]);
                    data[count++] = (byte)(buf[1]);
                    data[count++] = (byte)(buf[2]);
                    data[count++] = (byte)(buf[3]);
                    data[count++] = (byte)(buf[4]);
                    data[count++] = (byte)(buf[5]);
                    data[count++] = (byte)(buf[6]);
                    data[count++] = (byte)(buf[7]);
                }
                else if (command == "RF")
                {
                    float f = (float)GetValueDouble(command, table_no, address + i);

                    uint u = FloatingTool.FloatToUint(f);

                    buf = String.Format("{0:X08}", u);

                    data[count++] = (byte)(buf[0]);
                    data[count++] = (byte)(buf[1]);
                    data[count++] = (byte)(buf[2]);
                    data[count++] = (byte)(buf[3]);
                    data[count++] = (byte)(buf[4]);
                    data[count++] = (byte)(buf[5]);
                    data[count++] = (byte)(buf[6]);
                    data[count++] = (byte)(buf[7]);
                }
                else if (command == "RL")
                {
                    buf = String.Format("{0:X16}", (ulong)GetValueUint64(command, table_no, address + i));
                    data[count++] = (byte)(buf[0]);
                    data[count++] = (byte)(buf[1]);
                    data[count++] = (byte)(buf[2]);
                    data[count++] = (byte)(buf[3]);
                    data[count++] = (byte)(buf[4]);
                    data[count++] = (byte)(buf[5]);
                    data[count++] = (byte)(buf[6]);
                    data[count++] = (byte)(buf[7]);
                    data[count++] = (byte)(buf[8]);
                    data[count++] = (byte)(buf[9]);
                    data[count++] = (byte)(buf[10]);
                    data[count++] = (byte)(buf[11]);
                    data[count++] = (byte)(buf[12]);
                    data[count++] = (byte)(buf[13]);
                    data[count++] = (byte)(buf[14]);
                    data[count++] = (byte)(buf[15]);
                }
                else if (command == "RU")
                {
                    double f = (double)GetValueDouble(command, table_no, address + i);

                    ulong u = FloatingTool.DoubleToUlong(f);

                    buf = String.Format("{0:X16}", u);

                    data[count++] = (byte)(buf[0]);
                    data[count++] = (byte)(buf[1]);
                    data[count++] = (byte)(buf[2]);
                    data[count++] = (byte)(buf[3]);
                    data[count++] = (byte)(buf[4]);
                    data[count++] = (byte)(buf[5]);
                    data[count++] = (byte)(buf[6]);
                    data[count++] = (byte)(buf[7]);
                    data[count++] = (byte)(buf[8]);
                    data[count++] = (byte)(buf[9]);
                    data[count++] = (byte)(buf[10]);
                    data[count++] = (byte)(buf[11]);
                    data[count++] = (byte)(buf[12]);
                    data[count++] = (byte)(buf[13]);
                    data[count++] = (byte)(buf[14]);
                    data[count++] = (byte)(buf[15]);
                }
                else if (command == "RT")
                {
                    string s = GetValueString(command, table_no, address + i);

                    byte[] msg_bytes = ConvertString.StringToUtf8Bytes(s);

                    buf = String.Format("{0:X04}", msg_bytes.Length);
                    data[count++] = (byte)(buf[0]);
                    data[count++] = (byte)(buf[1]);
                    data[count++] = (byte)(buf[2]);
                    data[count++] = (byte)(buf[3]);

                    if (msg_bytes.Length > 0)
                    {
                        for (int j = 0; j < msg_bytes.Length; j++)
                        {
                            buf = String.Format("{0:X02}", msg_bytes[j]);
                            data[count++] = (byte)(buf[0]);
                            data[count++] = (byte)(buf[1]);
                        }
                    }
                }
                else // "RW"
                {
                    buf = String.Format("{0:X04}", (ushort)GetValueUint64(command, table_no, address + i));
                    data[count++] = (byte)(buf[0]);
                    data[count++] = (byte)(buf[1]);
                    data[count++] = (byte)(buf[2]);
                    data[count++] = (byte)(buf[3]);
                }
            }

            SendCommon(tns_no, nStation, command.ToLower(), data, count);

            DrawMessage("Command {3} table={0},address={1},size={2} Send O.K", table_no, address, size, command);
        }

        void Command_WriteBlock(ushort tns_no, byte[] recv_data, string command)
        {
            int table_no = HexBuf.ToByte(recv_data, 13);
            int address = HexBuf.ToUshort(recv_data, 15);
            int size = HexBuf.ToUshort(recv_data, 19);

            int pos = 23;

            for (int i = 0; i < size; i++)
            {
                if (command == "WD")
                {
                    uint value = HexBuf.ToUint(recv_data, 23 + i * 8);
                    SetValue(command, table_no, address + i, 0, value);
                }
                else if (command == "WF")
                {
                    uint value = HexBuf.ToUint(recv_data, 23 + i * 8);
                    float f = FloatingTool.UintToFloat(value);
                    SetValue(command, table_no, address + i, 0, f);
                }
                else if (command == "WL")
                {
                    ulong value = HexBuf.ToUlong(recv_data, 23 + i * 16);
                    SetValue(command, table_no, address + i, 0, value);
                }
                else if (command == "WU")
                {
                    ulong value = NetTools.HexBuf.ToUlong(recv_data, 23 + i * 16);
                    double f = FloatingTool.UlongToDouble(value);
                    SetValue(command, table_no, address + i, 0, f);
                }
                else if (command == "WT")
                {
                    int msg_size = HexBuf.ToUshort(recv_data, pos);

                    if (msg_size > 0)
                    {
                        byte[] utf_bytes = new byte[msg_size];

                        for (int j = 0; j < msg_size; j++)
                        {
                            utf_bytes[j] = HexBuf.ToByte(recv_data, pos + 4 + j * 2);
                        }

                        string unicode = ConvertString.Utf8BytesToString(utf_bytes, 0, msg_size);

                        SetValue(command, table_no, address + i, 0, unicode);
                    }
                    else
                    {
                        SetValue(command, table_no, address + i, 0, "");
                    }

                    pos += msg_size * 2 + 4;
                }
                else // command == "WW"
                {
                    ushort value = HexBuf.ToUshort(recv_data, 23 + i * 4);
                    SetValue(command, table_no, address + i, 0, value);
                }
            }

            SendCommon(tns_no, nStation, command.ToLower(), null, 0);

            DrawMessage("Command {3} table={0},address={1},size={2} O.K", table_no, address, size, command);
        }

        ushort[] WORD_MASK = { 0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0080, 0x0100, 0x0200, 0x0400, 0x0800, 0x1000, 0x2000, 0x4000, 0x8000 };

        // WB Command - Write Bit
        void Command_WB(ushort tns_no, byte[] recv_data)
        {
            int table_no = HexBuf.ToByte(recv_data, 13);
            int address = HexBuf.ToUshort(recv_data, 15);
            int bit_pos = HexBuf.ToByte(recv_data, 19);
            int value = HexBuf.ToByte(recv_data, 21);

            /*
            if (bit_pos > 15)
            {
                SendErrorCode(tns_no, 0, "Bit position > 15");
                return;
            }

            ushort data = (ushort)GetValueUint64("WW", table_no, address);

            if (value == 1)
            {
                data |= WORD_MASK[bit_pos];
            }
            else
            {
                data &= (ushort)((0xFFFF - WORD_MASK[bit_pos]));
            }

            SetValue("WW", table_no, address, data);*/

            SetValue("WB", table_no, address, bit_pos, value);

            SendCommon(tns_no, nStation, "wb", null, 0);

            DrawMessage("Command Write Bit table={0},address={1},bit_pos={2} O.K", table_no, address, bit_pos);
        }

        void Command_SV(ushort tns_no, byte[] recv_data)
        {
            int version_major = HexBuf.ToUshort(recv_data, 13);
            int version_minor = HexBuf.ToUshort(recv_data, 17);
            int version_build = HexBuf.ToUshort(recv_data, 21);
            int version_revision = HexBuf.ToUshort(recv_data, 25);

            byte[] data = new byte[16];

            int count = 0;
            string buf;

            buf = String.Format("{0:X04}", nVersionMajor);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

            buf = String.Format("{0:X04}", nVersionMinor);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

            buf = String.Format("{0:X04}", nVersionBuild);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

            buf = String.Format("{0:X04}", nVersionRevision);
            data[count++] = (byte)(buf[0]);
            data[count++] = (byte)(buf[1]);
            data[count++] = (byte)(buf[2]);
            data[count++] = (byte)(buf[3]);

            SendCommon(tns_no, nStation, "sv", data, count);

            DrawMessage("Master's AOB Version={0}.{1}.{2}.{3}", version_major, version_minor, version_build, version_revision);
        }

        public virtual void SetValue(string command, int table_no, int address, int bit_pos, object value)
        {
            
        }

        public virtual object GetValue(string command, int table_no, int address)
        {
            return null;
        }

        UInt64 GetValueUint64(string command, int table_no, int address)
        {
            object value = GetValue(command, table_no, address);

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
                return ConvertTool.ToUInt64((string)value);
            else
            {
                return Convert.ToUInt64(value);
            }
        }

        double GetValueDouble(string command, int table_no, int address)
        {
            object value = GetValue(command, table_no, address);

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

        string GetValueString(string command, int table_no, int address)
        {
            object value = GetValue(command, table_no, address);

            if (value == null) return "";

            return value.ToString();

            /*
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
            }*/
        }
    }
}

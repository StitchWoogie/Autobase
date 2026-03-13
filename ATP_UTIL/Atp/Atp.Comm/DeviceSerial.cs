using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Ats.Comm
{
    public class DeviceSerial : DeviceCommon
    {
        public string sPortName;
        public SerialPort port = null;
        public string sErrorMessage = null;

        string sMessage = "";
        bool bServerFlag = false;

        public void Init(bool server_flag, ProtocolCommon protocol, string name, int baudrate, Parity parity, int databits, StopBits stopbits)
        {
            Init(server_flag, protocol, name, baudrate, parity, databits, stopbits, false, false);
        }

        public void Init(bool server_flag, ProtocolCommon protocol, string name, int baudrate, Parity parity, int databits, StopBits stopbits, bool bDtrEnable, bool bRtsEnable)
        {
            bServerFlag = server_flag;

            pProtocol = protocol;
            pProtocol.SetDevice(this);

            sPortName = name;
            port = new System.IO.Ports.SerialPort(sPortName, baudrate, parity, databits, stopbits);

            try
            {
                port.DtrEnable = bDtrEnable;
                port.RtsEnable = bRtsEnable;
                port.Open();
                sMessage = "Port Ready.";
                bConnected = true;
            }
            catch (Exception exception)
            {
                sErrorMessage = exception.Message;
                sMessage = sErrorMessage;
            }

            if(bServerFlag)
                InitThread();

            pProtocol.ProtocolInit();
        }

        public void Init(bool server_flag, ProtocolCommon protocol, string name, int baudrate)
        {
            Init(server_flag, protocol, name, baudrate, Parity.None, 8, StopBits.One);
        }

        public override void UnInit()
        {
            if (pProtocol != null)
                pProtocol.ProtocolUnInit();

            if (bServerFlag)
                UnInitThread();

            if (port != null)
            {
                port.Close();
                port = null;
                bConnected = false;
            }
        }

        //TimeOutClass timeout = new TimeOutClass();

        public override void OnThread()
        {
            if (port == null)
                return;

            if (!port.IsOpen)
            {
                sMessage = sErrorMessage;
                return;
            }

            for (int i = 0; i < 256; i++)
            {
                if (port.BytesToRead == 0)
                {
                    /*
                    // 시간이 어느정도 지나면 클리어해준다.
                    if (timeout.IsTimeOut(5))
                    {
                        pProtocol.ClearRecvCount();
                        timeout.Reset();
                    }*/
                    return;
                }

                int ch = port.ReadByte();

                if (ch == -1) return;

                nRecvBytes += 1;

                AddCommCode(EnumCommCode.RecvCode, (byte)ch);

                // timeout.Reset();

                if (pCC != null && pCC.bUseEncryption)
                {
                    pCC.SetEncryptedData((byte)ch);
                    byte[] decoded = new byte[10];
                    while (pCC.GetDecryptionData(decoded, 1) > 0)
                    {
                        pProtocol.ExecuteOneChar(decoded[0]);
                    }
                }
                else
                {
                    pProtocol.ExecuteOneChar(ch);
                }
            }
        }

        public override void Write(byte[] source, int offset, int count)
        {
            byte[] data;

            if (pCC != null)
            {
                data = pCC.GetEncryptionData(source, ref offset, ref count);
            }
            else
            {
                data = source;
            }

            port.Write(data, offset, count);

            AddCommCode(EnumCommCode.SendNextLine, 0);
            for (int i = 0; i < count; i++)
                AddCommCode(EnumCommCode.SendCode, (byte)(data[i]));

            nSendBytes += count;
        }

        public override string GetInfoString()
        {
            return String.Format("{0},{1}", sPortName, port.BaudRate);
        }

        public override void Clear()
        {
            if (bServerFlag) return;    // Server Flag 상태에서는 Thread에서 읽어가 버릴수가 있다.

            // Sync와 Async를 왔다 갔다 하면서 오류가 나는것 같다.
            
            // 혹시 무한루프가 걸릴 수 있으므로 5000번만 돈다.
            for (int i = 0; i < 5000; i++)
            {
                if (port.BytesToRead == 0) return;
                if (port.ReadByte() == -1) return;
            }
        }
    }
}


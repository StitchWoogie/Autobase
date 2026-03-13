using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;

namespace SMS.SmsComm
{
    public class SendSmsDataWM215
    {

        public SendSmsDataWM215()
        {
            //
            // TODO: Add constructor logic here
            //
        }



        static byte getTel2DigitDataToByteCode(byte code0, byte code1)
        {
            return (byte)((code0 % 0x10) + (code1 % 0x10) * 0x10);
        }

        static int stringTelNumToByteCode(ref byte[] commSendBuf, string data, int buf_pos)
        {
            int i = 0, len = data.Length, start = buf_pos;

            if (len <= 0) return 0;
            if (len > 50) len = 50;
            byte[] imsi = new byte[len];

            commSendBuf[buf_pos++] = (byte)len;		// size field
            commSendBuf[buf_pos++] = (byte)0xA1;	// msb = always 1, type number = 010 국내용, ID = 0001
            imsi = Tools.StringToBytes(data);
            for (i = 0; i < len / 2; i++)
            {
                commSendBuf[buf_pos++] = getTel2DigitDataToByteCode(imsi[i * 2], imsi[i * 2 + 1]);
            }
            if (len % 2 == 1)
            {
                commSendBuf[buf_pos++] = getTel2DigitDataToByteCode(imsi[i * 2], 0x0F);
            }
            return buf_pos - start;
        }

        static int stringDataToByteSize(string data)
        {
            int len = data.Length;
            byte[] imsi = new byte[len * 2];

            imsi = Tools.StringToBytes(data);
            return imsi.Length;
            
        }

        static int stringDataToByteCopy(ref byte[] commSendBuf, string data, int buf_pos)
        {
            int i = 0, len = data.Length;
            byte[] imsi = new byte[len * 2];

            imsi = Tools.StringToBytes(data);
            for (i = 0; i < imsi.Length; i++)
            {
                commSendBuf[i + buf_pos] = imsi[i];
            }
            return i;
        }


        static public int makeWm215SendWriteCode(ref string sendBuf, string telNo, string sendTelNo, string message)
        {
            message.Trim();

            int sLen = sendTelNo.Length, buf_pos = 0;
            int len = stringDataToByteSize(message);

            byte[] commSendBuf = new byte[1024];
            commSendBuf[buf_pos++] = (byte)0x00;	// SCA size
            commSendBuf[buf_pos++] = (sLen == 0) ? (byte)0x11 : (byte)0x51;	// TP-UDHI, TP-VPF, ....
            commSendBuf[buf_pos++] = (byte)0xFF;	// TP-MR	자동업데이트되도록 설정
            buf_pos += stringTelNumToByteCode(ref commSendBuf, telNo, buf_pos);// TP-DA
            commSendBuf[buf_pos++] = (byte)0x00;	// TP-DCS : MS 와의 송수신 = 0
            commSendBuf[buf_pos++] = (byte)0x84;	// 84 = KS5601(한글), 00 = 7bit ascii, 04 = 8 bit octet
            commSendBuf[buf_pos++] = (byte)0xA7;	// TP-VP 
            if (sLen == 0)
            {
                commSendBuf[buf_pos++] = (byte)len;	// next field size
            }
            else
            {
                commSendBuf[buf_pos++] = (byte)0x5B;	// next field size
                commSendBuf[buf_pos++] = (byte)0x0A;	// TP-UD 를 제외한 다음 필드의 길이
                commSendBuf[buf_pos++] = (byte)0x22;	// IEI, 22:회신번호
                commSendBuf[buf_pos++] = (byte)0x08;	// IEDL,다음에 오는 IED의 길이

                int len2 = stringTelNumToByteCode(ref commSendBuf, sendTelNo, buf_pos);

                commSendBuf[buf_pos - 1] = (byte)len2;	// IEDL,다음에 오는 IED의 길이
                commSendBuf[buf_pos - 3] = (byte)(len2 + 2);// TP-UD 를 제외한 다음 필드의 길이
                commSendBuf[buf_pos - 4] = (byte)(len + len2 + 3);// next field size
                buf_pos += len2;
            }
            buf_pos += stringDataToByteCopy(ref commSendBuf, message, buf_pos);

            for (int i = 0; i < buf_pos; i++)
            {
                sendBuf += string.Format("{0:X2}", commSendBuf[i]);
            }
            return buf_pos;
        }


    }
}

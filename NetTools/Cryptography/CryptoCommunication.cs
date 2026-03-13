using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace NetTools.Cryptography
{
    enum EnumCryptoEngine
    {
        AES = 0,
        ARIA = 1,
    }

    [Serializable]
    public class CryptoCommunication
    {
        public bool bUseEncryption;
        public string sEngine = "AES";
        public string sCipherMode = "CBC";
        public string sPaddingMode = "PKCS7";
        public string sKey = "000102030405060708090A0B0C0D0E0F";
        public string sIV = "00112233445566778899AABBCCDDEEFF";
        public int nFrameMode = 0;  // 0 = DLE Byte stuffing

        const string sCommand = "CryptoCommunication";

        public CryptoCommunication()
        {
            ConvertStringToVars();
        }

        public bool IsCommand(string command)
        {
            return sCommand == command;
        }

        public void Save(CryptoTextWriter writer)
        {
            writer.WriteLine("{0},BEGIN", sCommand);
            writer.Write("Option,");
            writer.Write("{0},", bUseEncryption);
            writer.Write("{0},", sEngine);
            writer.Write("{0},", sCipherMode);
            writer.Write("{0},", sPaddingMode);
            writer.Write("{0},", sKey);
            writer.Write("{0},", sIV);
            writer.Write("{0},", nFrameMode);
            writer.WriteLine();
            writer.WriteLine("{0},END", sCommand);
        }

        public void Load(CryptoTextReader reader)
        {
            CommaTextReader comma = new CommaTextReader();
            string command;
            string buf;
            string one_line;

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);
                command = comma.GetString();

                if (IsCommand(command))
                {
                    buf = comma.GetString();
                    if (String.Compare(buf, "END", true) == 0) break;
                }
                else if (command == "Option")
                {
                    bUseEncryption = comma.GetBool();
                    sEngine = comma.GetString();
                    sCipherMode = comma.GetString();
                    sPaddingMode = comma.GetString();
                    sKey = comma.GetString();
                    sIV = comma.GetString();
                    nFrameMode = comma.GetInt();
                }
            }

            ConvertStringToVars();
        }

        byte[] pKey;
        byte[] pIV;
        CipherMode eCM;
        PaddingMode ePM;
        EnumCryptoEngine eCE;

        public void ConvertStringToVars()
        {
            int nKeyLength = sKey.Length / 2;
            pKey = new byte[nKeyLength];

            for (int i = 0; i < nKeyLength; i++)
            {
                pKey[i] = HexBuf.ToByte(sKey, i * 2);
            }

            int nIVLength = sIV.Length / 2;
            pIV = new byte[nIVLength];

            for (int i = 0; i < nIVLength; i++)
            {
                pIV[i] = HexBuf.ToByte(sIV, i * 2);
            }

            if (String.Compare(sCipherMode, "CBC") == 0)
                eCM = CipherMode.CBC;
            else if (String.Compare(sCipherMode, "OFB") == 0)
                eCM = CipherMode.OFB;
            else if (String.Compare(sCipherMode, "CFB") == 0)
                eCM = CipherMode.CFB;
            else if (String.Compare(sCipherMode, "CTS") == 0)
                eCM = CipherMode.CTS;
            else
                eCM = CipherMode.ECB;

            if (String.Compare(sPaddingMode, "PKCS7") == 0)
                ePM = PaddingMode.PKCS7;
            else if (String.Compare(sPaddingMode, "Zeros") == 0)
                ePM = PaddingMode.Zeros;
            else if (String.Compare(sPaddingMode, "ANSIX923") == 0)
                ePM = PaddingMode.ANSIX923;
            else if (String.Compare(sPaddingMode, "ISO10126") == 0)
                ePM = PaddingMode.ISO10126;
            else
                ePM = PaddingMode.None;

            if (sEngine == "ARIA")
                eCE = EnumCryptoEngine.ARIA;
            else
                eCE = EnumCryptoEngine.AES;
        }

        public byte[] GetEncryptionData(byte[] source, ref int offset, ref int count)
        {
            if (!bUseEncryption)
            {
                return source;
            }

            byte[] buffer = new byte[count];

            Array.Copy(source, offset, buffer, 0, count);   // offset이 0인 버퍼를 주어야 한다.
            
            byte[] result;

            if (eCE == EnumCryptoEngine.ARIA)
                result = CtyptoARIA.Encrypt(buffer, pKey, pIV, eCM, ePM);
            else
                result = CryptoAES.Encrypt(buffer, pKey, pIV, eCM, ePM);

            if (nFrameMode == 0)
            {
                int dle_count = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == 0x10)
                        dle_count++;
                }

                byte[] new_frame = new byte[4 + dle_count + result.Length];

                int pos = 0;
                new_frame[pos++] = 0x10;
                new_frame[pos++] = 0x02;
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == 0x10)
                        new_frame[pos++] = 0x10;
                    new_frame[pos++] = result[i];
                }
                new_frame[pos++] = 0x10;
                new_frame[pos++] = 0x03;

                count = new_frame.Length;
                return new_frame;
            }
            else
            {
                return null;
            }
        }

        byte[] dataGather = new byte[4096];
        int nGatherData = 0;
        bool dle_flag = false;
        bool bStartFlag = false;

        byte[] pRecvRing = new byte[4096];
        int nRecvRingTarget = 0;
        int nRecvRingCurrent = 0;

        void AddRecvRing(byte ch)
        {
            pRecvRing[nRecvRingTarget] = ch;
            nRecvRingTarget++;
            nRecvRingTarget %= 4096;
        }

        public int GetDecryptionData(byte[] source, int count)
        {
            if (nRecvRingTarget == nRecvRingCurrent) return 0;

            int remain;

            if (nRecvRingTarget > nRecvRingCurrent)
                remain = nRecvRingTarget - nRecvRingCurrent;
            else
                remain = nRecvRingTarget + 4096 - nRecvRingCurrent;

            if (remain < count) count = remain;

            if (count <= 0) return 0;

            for (int i = 0; i < count; i++)
            {
                source[i] = pRecvRing[nRecvRingCurrent];
                nRecvRingCurrent++;
                nRecvRingCurrent %= 4096;
            }

            return count;
        }

        public void SetEncryptedData(byte ch)
        {
            // check buffer over
            if (nGatherData >= dataGather.Length)
            {
                nGatherData = 0;
            }

            if (nFrameMode == 0)
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
                        dataGather[nGatherData++] = ch;
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
                            dataGather[nGatherData++] = ch;
                    }
                }

            }
            else
            {

            }

            return;

        ok_oneframe_received: ;

            if ((nGatherData % 16) != 0)
            {
                dle_flag = false;
                nGatherData = 0;
                bStartFlag = false;
                return;
            }

            byte[] buffer = new byte[nGatherData];

            Array.Copy(dataGather, 0, buffer, 0, nGatherData);   // offset이 0인 버퍼를 주어야 한다.

            byte[] result = null;

            if (eCE == EnumCryptoEngine.ARIA)
                result = CtyptoARIA.Decrypt(buffer, pKey, pIV, eCM, ePM);
            else
            {
                try
                {
                    result = CryptoAES.Decrypt(buffer, pKey, pIV, eCM, ePM);
                }
                catch 
                {
                    result = null;
                }
            }

            if (result != null)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    AddRecvRing(result[i]);
                }
            }

            dle_flag = false;
            nGatherData = 0;
            bStartFlag = false;
        }
    }
}

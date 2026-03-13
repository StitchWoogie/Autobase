using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace NetTools.Cryptography
{
    public class CryptoTextWriter
    {
        MemoryStream ms = new MemoryStream();
        CommaTextWriter ctw;

        string sFilename;

        // 1.0.0 = SHA1.   Compact 3.5에서는 SHA1밖에 없다. 
        byte cVersionMajor = 1;
        byte cVersionMinor = 0;
        byte cVersionBuild = 0;

        // 나중에 SHA256을 위해서 32byte로 준비한다.
        byte[] Make20To32(byte[] source)
        {
            byte[] target = new byte[32];
            for (int i = 0; i < target.Length; i++)
            {
                target[i] = (byte)(i ^ source[i % 20]);
            }

            return target;
        }

        void MakeEmptyPassCode()
        {
            // CE 버전이 SHA256을 지원하기 전까지는 항상 1이다.
            if (cVersionMajor >= 2)
            {
                // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
                // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
                SHA256 sha256 = new SHA256Managed();
                byte[] password_b = Encoding.UTF8.GetBytes("" + "!@#");
                EmptyPassCode = sha256.ComputeHash(password_b);
            }
            else
            {
                // 20바이트이지만 32바이트로 만들어준다.
                SHA1 sha256 = new SHA1CryptoServiceProvider();
                byte[] password_b = Encoding.UTF8.GetBytes("" + "!@#");
                sha256.ComputeHash(password_b);
                
                EmptyPassCode = Make20To32(sha256.ComputeHash(password_b));
            }
        }

        public CryptoTextWriter(string filename)
        {
            MakeEmptyPassCode();

            sFilename = filename;
            ctw = new CommaTextWriter(ms);
        }

        public void Write(string format, params object[] args)
        {
            ctw.Write(format, args);
        }

        public void WriteLine()
        {
            ctw.WriteLine();
        }

        public void WriteLine(string format, params object[] args)
        {
            ctw.WriteLine(format, args);
        }

        byte[] EmptyPassCode;

        public bool bEncryption = false;           // 일단 암호화하지 않는다.

        public void Close()
        {
            ctw.Flush();
            byte[] source = ms.ToArray();

            if (bEncryption)
            {
                byte[] key = { 0x11, 0x63, 0x43, 0x54, 0x64, 0x70, 0x78, 0x89, 0xf9, 0xda, 0xc6, 0xc1, 0x80, 0xb0, 0xfc, 0x1d };
                byte[] IV = { 0x10, 0x62, 0x03, 0x04, 0x54, 0x60, 0x75, 0x83, 0x79, 0x0a, 0xb8, 0xc0, 0xd0, 0xe0, 0xf0, 0x12 };

                byte[] passcode_enc = CryptoAES.Encrypt(EmptyPassCode, key, IV, System.Security.Cryptography.CipherMode.CFB, System.Security.Cryptography.PaddingMode.None);

                byte[] source_enc = CryptoAES.Encrypt(source, EmptyPassCode, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

                byte[] all_bytes = new byte[16 + 32 + source_enc.Length + 32]; // header(EncryptedFile???) + PassCode + buffer + CRC

                all_bytes[0] = (byte)'E';
                all_bytes[1] = (byte)'n';
                all_bytes[2] = (byte)'c';
                all_bytes[3] = (byte)'r';
                all_bytes[4] = (byte)'y';
                all_bytes[5] = (byte)'p';
                all_bytes[6] = (byte)'t';
                all_bytes[7] = (byte)'e';
                all_bytes[8] = (byte)'d';
                all_bytes[9] = (byte)'F';
                all_bytes[10] = (byte)'i';
                all_bytes[11] = (byte)'l';
                all_bytes[12] = (byte)'e';
                all_bytes[13] = cVersionMajor;
                all_bytes[14] = cVersionMinor;
                all_bytes[15] = cVersionBuild;   // Version

                Array.Copy(passcode_enc, 0, all_bytes, 16, 32);
                Array.Copy(source_enc, 0, all_bytes, 48, source_enc.Length);

                byte[] crc;

                if (cVersionMajor >= 2)
                {
                    // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
                    // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
                    SHA256 sha256 = new SHA256Managed();
                    crc = sha256.ComputeHash(all_bytes, 10, source_enc.Length + 48 - 10);
                }
                else
                {
                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    crc = Make20To32(sha1.ComputeHash(all_bytes, 10, source_enc.Length + 48 - 10));
                }

                Array.Copy(crc, 0, all_bytes, source_enc.Length + 48, 32);

                File.WriteAllBytes(sFilename, all_bytes);
            }
            else
            {
                File.WriteAllBytes(sFilename, source);
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using NetTools;

namespace NetTools.Cryptography
{
    public class CryptoTextReader
    {
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

        TextReader reader;
        public CryptoTextReader(string filename)
        {
            byte[] all_bytes = File.ReadAllBytes(filename);

            if (all_bytes.Length >= 96 &&
                all_bytes[0] == 'E' && all_bytes[1] == 'n' && all_bytes[2] == 'c' && all_bytes[3] == 'r' && all_bytes[4] == 'y' &&
                all_bytes[5] == 'p' && all_bytes[6] == 't' && all_bytes[7] == 'e' && all_bytes[8] == 'd' && all_bytes[9] == 'F' &&
                all_bytes[10] == 'i' && all_bytes[11] == 'l' && all_bytes[12] == 'e')
            {
                byte cVersionMajor = (all_bytes[13]);
                byte cVersionMinor = (all_bytes[14]);
                byte cVersionBuild = (all_bytes[15]);

                byte[] crc;
                if (cVersionMajor >= 2)
                {
                    // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
                    // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
                    SHA256 sha256 = new SHA256Managed();

                    crc = sha256.ComputeHash(all_bytes, 10, all_bytes.Length - 32 - 10);
                }
                else
                {
                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    crc = Make20To32(sha1.ComputeHash(all_bytes, 10, all_bytes.Length - 32 - 10));
                }

                if (!CompareTool.CompareBytes(crc, 0, all_bytes, all_bytes.Length - 32, 32))
                {
                    throw new Exception("File CRC mismatched. " + filename);
                }

                byte[] key = { 0x11, 0x63, 0x43, 0x54, 0x64, 0x70, 0x78, 0x89, 0xf9, 0xda, 0xc6, 0xc1, 0x80, 0xb0, 0xfc, 0x1d };
                byte[] IV = { 0x10, 0x62, 0x03, 0x04, 0x54, 0x60, 0x75, 0x83, 0x79, 0x0a, 0xb8, 0xc0, 0xd0, 0xe0, 0xf0, 0x12 };

                byte[] passcode = CryptoAES.Decrypt(all_bytes, 16, 32, key, IV, System.Security.Cryptography.CipherMode.CFB, System.Security.Cryptography.PaddingMode.None);
                byte[] source = CryptoAES.Decrypt(all_bytes, 48, all_bytes.Length - 80, passcode, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

                MemoryStream ms = new MemoryStream(source);

                reader = new StreamReader(ms);
            }
            else
            {
                MemoryStream ms = new MemoryStream(all_bytes);

                reader = new StreamReader(ms);
            }
        }

        public string ReadLine()
        {
            return reader.ReadLine();
        }

        public void Close()
        {
            reader.Close();
        }
    }
}

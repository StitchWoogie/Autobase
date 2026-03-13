using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace NetTools.Cryptography
{
    // KeySize = 128/192/256 세가지가 있다.
    // IV = Block Size 가 128이므로  항상 16이어야 한다.
    public class CryptoAES
    {
        public static byte[] Encrypt(byte[] source, byte[] key, byte[] IV, CipherMode cm, PaddingMode pm)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = key.Length*8;
            aes.BlockSize = 128;
            aes.Mode = cm;
            aes.Padding = pm;
            aes.Key = key;
            aes.IV = IV;

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    cs.Write(source, 0, source.Length);
                }

                xBuff = ms.ToArray();
            }

            return xBuff;
        }

        public static byte[] Decrypt(byte[] source, int offset, int size, byte[] key, byte[] IV, CipherMode cm, PaddingMode pm)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = key.Length * 8;
            aes.BlockSize = 128;
            aes.Mode = cm;
            aes.Padding = pm;
            aes.Key = key;
            aes.IV = IV;

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    cs.Write(source, offset, size);
                }

                xBuff = ms.ToArray();
            }

            return xBuff;
        }

        public static byte[] Decrypt(byte[] source, byte[] key, byte[] IV, CipherMode cm, PaddingMode pm)
        {
            return Decrypt(source, 0, source.Length, key, IV, cm, pm);
        }

    }
}

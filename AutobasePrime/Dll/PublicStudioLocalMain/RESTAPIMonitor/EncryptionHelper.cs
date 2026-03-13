using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace AutobaseRESTAPIMonitor
{
    public static class EncryptionHelper
    {
        // 고정된 키와 IV 값. 필요시 안전한 방식으로 저장/관리해야 함.
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("AutoBaseRESTAPI0"); // 16바이트 키
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("01PATSEResaBotuA");  // 16바이트 IV

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}

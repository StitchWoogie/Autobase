using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using NetTools;

namespace WebTools
{
    public class HashTool
    {
        static byte[] StringToBytes(string buf)
        {
            byte[] b = new byte[buf.Length * 2];

            for (int i = 0; i < buf.Length; i++)
            {
                b[i * 2 + 0] = (byte)(buf[i] / 256);
                b[i * 2 + 1] = (byte)(buf[i] % 256);
            }

            return b;
        }

        public static byte[] MakeHash(string org)
        {
            byte[] b = StringToBytes(org);

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(b);

            return result;
        }

        static byte[] MakeHash256(string org)
        {
            byte[] b = StringToBytes(org);

            SHA256 sha = new SHA256CryptoServiceProvider();
            byte[] result = sha.ComputeHash(b);

            return result;
        }

        public static bool CompareHash(string org, byte[] hash)
        {
            byte[] hash2 = MakeHash(org);

            if (hash2.Length != hash.Length) return false;
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] != hash2[i]) return false;
            }

            return true;
        }

        public static bool CompareHash256(string org, byte[] hash)
        {
            byte[] hash2 = MakeHash256(org);

            if (hash2.Length != hash.Length) return false;
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] != hash2[i]) return false;
            }

            return true;
        }

        public static string ConvertHexaString(byte[] source)
        {
            StringBuilder s = new StringBuilder();

            for(int i = 0; i < source.Length; i++) {
                s.AppendFormat("{0:X02}", source[i]);
            }

            return s.ToString();
        }

        public static byte[] ConvertHexaString(string source)
        {
            int size = source.Length / 2;
            byte[] target = new byte[size];

            for (int i = 0; i < size; i++)
            {
                target[i] = HexBuf.ToByte(source, i * 2);
            }

            return target;
        }
    }
}

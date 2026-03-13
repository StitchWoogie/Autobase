using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using NetTools;

namespace NetTools
{
    public class HashTool
    {
        public static byte[] StringToBytes(string buf)
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

        public static byte[] MakeHash256(string org)
        {
            byte[] b = StringToBytes(org);

            // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
            // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
            SHA256 sha = new SHA256Managed();
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

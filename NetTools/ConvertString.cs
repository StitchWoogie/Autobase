using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace NetTools
{
    public class ConvertString
    {
        public static byte[] StringToUtf8Bytes(string buf)
        {
            System.Text.Encoder d = System.Text.Encoding.UTF8.GetEncoder();

            char[] chars = buf.ToCharArray();
            byte[] bytes = new byte[0x10000];

            int retn = d.GetBytes(chars, 0, chars.Length, bytes, 0, true);

            byte[] result = new byte[retn];

            for (int i = 0; i < retn; i++)
            {
                result[i] = bytes[i];
            }

            return result;
        }

        public static string Utf8BytesToString(byte[] buf, int byte_offset, int byte_count)
        {
            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            char[] chars = new char[byte_count];
            int retn = d.GetChars(buf, byte_offset, byte_count, chars, 0);

            string buf_t = "";
            for (int i = 0; i < retn; i++)
            {
                if (chars[i] == 0) break;	// 실제 문자열 길이와 배열의 길이는 다르다.
                buf_t += Char.ToString(chars[i]);
            }

            return buf_t;
        }
    }
}

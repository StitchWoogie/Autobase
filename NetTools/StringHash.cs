using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace WebTools
{
    public class StringHash
    {
        
        static readonly byte[] table = new byte[64] {    
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9',
            (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z',
            (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h', (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n', (byte)'o', (byte)'p', (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u', (byte)'v', (byte)'w', (byte)'x', (byte)'y', (byte)'z',
            (byte)'_', (byte)'.' };

        static readonly ushort[] xor_hash = new ushort[10] { 0xa020, 0xA158, 0x7fec, 0xaf5f, 0x1C34, 0xe5e7, 0xB7f6, 0xbBcc, 0x8fec, 0x12e4 };

        // 64개의 데이터이므로 각 글자마다 6Bit가 나오므로 적어도 글자 하나는 3Byte가 할당되어야 한다.
        // 처음에는 byte로 했으나 데이터베이스에서 사용하려면 문자열이 사용하기 좋으므로 문자열을 사용할 수 있도록 바꾸었다.
        public static string Encode(string source, string seed)
        {
            StringBuilder target = new StringBuilder();
            ushort ch;
            byte b;
            int table_no;

            for (int i = 0; i < source.Length; i++)
            {
                ch = source[i];
                ch = (ushort)(ch ^ seed[i % seed.Length]);
                ch = (ushort)(ch ^ xor_hash[i % 10]);
                table_no = ((ch >> 10) & 63);
                b = table[table_no];
                target.Append((char)b);
                table_no = ((ch >> 4) & 63);
                b = table[table_no];
                target.Append((char)b);
                table_no = ((ch >> 0) & 15);
                b = table[table_no];
                target.Append((char)b);
            }

            return target.ToString();
        }

        static ushort GetCode(char source)
        {
            ushort value;

            if (source >= (byte)'0' && source <= (byte)'9')
            {
                value = (ushort)(source - '0');
            }
            else if (source >= (byte)'A' && source <= (byte)'Z')
            {
                value = (ushort)(source - 'A' + 10);
            }
            else if (source >= (byte)'a' && source <= (byte)'z')
            {
                value = (ushort)(source - 'a' + 36);
            }
            else if (source == (byte)'_')
            {
                value = (ushort)62;
            }
            else if (source == (byte)'.')
            {
                value = (ushort)63;
            }
            else
            {
                value = 0;  // error
            }

            return value;
        }

        // 64개의 데이터이므로 각 글자마다 6Bit가 나오므로 적어도 글자 하나는 3Byte가 할당되어야 한다.
        public static string Decode(string source, string seed)
        {
            string target = "";
            ushort ch;
            int size = source.Length / 3;

            for (int i = 0; i < size; i++)
            {
                ch = (ushort)((GetCode(source[i * 3 + 0]) << 10) | (GetCode(source[i * 3 + 1]) << 4) | (GetCode(source[i * 3 + 2]) << 0));
                ch = (ushort)(ch ^ xor_hash[i % 10]);
                ch = (ushort)(ch ^ seed[i % seed.Length]);
                target += (char)ch;
            }

            return target;
        }

        // 20 바이트짜리 Seed를 만든다.
        public static string MakeRandomSeed()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            uint r;

            StringBuilder s = new StringBuilder();
            for (int j = 0; j < 20; j++)
            {
                r = (uint)rand.Next();
                s.Append((char)table[r % 64]);
            }

            return s.ToString();
        }

        // 잘됨
        public static bool TestEngine()
        {
            string seed = "dol";
            string source;
            string t;
            string target;

            Random rand = new Random();
            
            for (int i = 0; i < 10000; i++)
            {
                source = MakeRandomSeed();
                
                t = StringHash.Encode(source, seed);
                target = StringHash.Decode(t, seed);
                if (source != target)
                {
                    return false;
                }
            }

            return true;
        }

        /*
        static readonly byte[] table =  new byte[64] {    
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9',
            (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z',
            (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h', (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n', (byte)'o', (byte)'p', (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u', (byte)'v', (byte)'w', (byte)'x', (byte)'y', (byte)'z',
            (byte)'_', (byte)'.' };

        static readonly ushort[] xor_hash = new ushort[10] { 0xa020, 0xA158, 0x7fec, 0xaf5f, 0x1C34, 0xe5e7, 0xB7f6, 0xbBcc, 0x8fec, 0x12e4 };

        // 64개의 데이터이므로 각 글자마다 6Bit가 나오므로 적어도 글자 하나는 3Byte가 할당되어야 한다.
        public static byte[] Encode(string source, string seed)
        {
            byte[] target = new byte[source.Length * 3];
            ushort ch;
            byte b;
            int table_no;

            for (int i = 0; i < source.Length; i++)
            {
                ch = source[i];
                ch = (ushort)(ch ^ seed[i % seed.Length]);
                ch = (ushort)(ch ^ xor_hash[i % 10]);
                table_no = ((ch >> 10) & 63);
                b = table[table_no];
                target[i * 3 + 0] = b;
                table_no = ((ch >> 4) & 63);
                b = table[table_no];
                target[i * 3 + 1] = b;
                table_no = ((ch >> 0) & 15);
                b = table[table_no];
                target[i * 3 + 2] = b;
            }

            return target;
        }

        static ushort GetCode(byte source)
        {
            ushort value;

            if (source >= (byte)'0' && source <= (byte)'9')
            {
                value = (ushort)(source - '0');
            }
            else if (source >= (byte)'A' && source <= (byte)'Z')
            {
                value = (ushort)(source - 'A'+10);
            }
            else if (source >= (byte)'a' && source <= (byte)'z')
            {
                value = (ushort)(source - 'a' + 36);
            }
            else if (source == (byte)'_')
            {
                value = (ushort)62;
            }
            else if (source == (byte)'.')
            {
                value = (ushort)63;
            }
            else
            {
                value = 0;  // error
            }

            return value;
        }

        // 64개의 데이터이므로 각 글자마다 6Bit가 나오므로 적어도 글자 하나는 3Byte가 할당되어야 한다.
        public static string Decode(byte[] source, string seed)
        {
            string target = "";
            ushort ch;
            int size = source.Length / 3;

            for (int i = 0; i < size; i++)
            {
                ch = (ushort)((GetCode(source[i * 3 + 0]) << 10) | (GetCode(source[i * 3 + 1]) << 4) | (GetCode(source[i * 3 + 2]) << 0));
                ch = (ushort)(ch ^ xor_hash[i % 10]);
                ch = (ushort)(ch ^ seed[i % seed.Length]);
                target += (char)ch;
            }

            return target;
        }*/
    }
}

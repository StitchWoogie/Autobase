using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetTools.Hash
{
    public class MakeHashCrc
    {
        uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        uint RotateRight(uint value, int count)
        {
            return (value >> count) | (value << (32 - count));
        }

        public string ComputeHash(string source)
        {
            const int MAX_DATA = 20;

            // a~z A~z 0~9 총 62개의 영문/숫자로 해싱한다.
            uint[] hash = new uint[MAX_DATA];

            // 기본 seed값 설정
            for (int i = 0; i < MAX_DATA; i++)
            {
                hash[i] = (uint)i;
            }

            uint basic_shift = 0;
            for (int i = 0; i < source.Length; i++)
            {
                for (int j = 0; j < MAX_DATA; j++)
                {
                    hash[j] ^= source[i];
                }
                basic_shift += source[i];
            }
            basic_shift %= 32;

            for (int i = 0; i < source.Length; i++)
            {
                hash[i % MAX_DATA] = (hash[i % MAX_DATA] ^ RotateLeft(source[i], (int)((basic_shift + i) % 32)));
            }

            StringBuilder s = new StringBuilder();
            uint b;
            for (int i = 0; i < MAX_DATA; i++)
            {
                b = hash[i] % 62;

                if (b < 10)
                {
                    s.Append((char)('0' + b));
                }
                else if (b < 36)
                {
                    s.Append((char)('a' + (b - 10)));
                }
                else
                {
                    s.Append((char)('A' + (b - 36)));
                }
            }

            return s.ToString();
        }
    }
}

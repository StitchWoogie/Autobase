using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetTools.Hash
{
    public class HashBytes : HashToolRoot
    {
        const int nCountXorHash = 21;
        readonly byte[] xor_hash = new byte[nCountXorHash] { 0xa0, 0x20, 0xA1, 0x58, 0x7f, 0xec, 0xaf, 0x5f, 0x1C, 0x34, 0xe5, 0xe7, 0xB7, 0xf6, 0xbB, 0xcc, 0x8f, 0xec, 0x12, 0xe4, 0x71 };

        public byte[] Encode(byte[] source, byte[] seed)
        {
            if (!IsNormalAccess()) return source;

            byte[] target = new byte[source.Length];

            byte ch = 0;

            for (int i = 0; i < source.Length; i++)
            {
                ch = source[i];    // 앞에것과 연결된다.
                ch ^= xor_hash[i % nCountXorHash];
                ch ^= seed[i % seed.Length];
                target[i] = ch;
            }
            
            return target;
        }

        public byte[] Decode(byte[] source, byte[] seed)
        {
            if (!IsNormalAccess()) return source;

            byte[] target = new byte[source.Length];

            byte ch = 0;

            for (int i = 0; i < source.Length; i++)
            {
                ch = source[i];    // 앞에것과 연결된다.
                ch ^= xor_hash[i % nCountXorHash];
                ch ^= seed[i % seed.Length];
                target[i] = ch;
            }

            return target;
        }

        /*
        // 잘됨
        public bool TestEngine()
        {
            string seed = "dol";
            string source;
            string t;
            string target;

            Random rand = new Random();

            for (int i = 0; i < 10000; i++)
            {
                source = MakeRandomSeedString();

                t = Encode(source, seed);
                target = Decode(t, seed);
                if (source != target)
                {
                    return false;
                }
            }

            return true;
        }*/
    }
}

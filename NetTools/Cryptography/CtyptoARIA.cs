using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace NetTools.Cryptography
{
    public class CtyptoARIA
    {
        static void Xor(byte[] buffer, int ib, byte[] iv, int ii)
        {
            for (int i = 0; i < 16; i++)
            {
                buffer[ib + i] ^= iv[ii + i];
            }
        }

        public static byte[] Encrypt(byte[] source, byte[] key, byte[] IV, CipherMode cm, PaddingMode pm)
        {
            if (pm == PaddingMode.None && (source.Length % 16) != 0)
            {
                throw new Exception("PaddingMode.None 일경우는 source.Length가 16의 배수이어야 합니다.");
            }

            int target_size;
            if (pm == PaddingMode.None)
                target_size = source.Length;    // 같은 크기를 유지한다.
            else
                target_size = (source.Length + 16) / 16 * 16; // 16의 배수를 끊는다.    16의 배수가 맞을 떄는 한개더 블럭을 넣는다.

            byte[] target = new byte[target_size];
            byte[] buffer = new byte[16];
            byte[] vector = new byte[16];

            Array.Copy(source, target, source.Length);

            int remains = target_size - source.Length;

            if (remains > 0)
            {
                if (pm == PaddingMode.PKCS7)
                {
                    for (int i = source.Length; i < target_size; i++)
                    {
                        target[i] = (byte)remains;
                    }
                }
                else if (pm == PaddingMode.ANSIX923)
                {
                    for (int i = source.Length; i < target_size; i++)
                    {
                        if (i == target_size - 1) 
                            target[i] = (byte)remains;
                        else
                            target[i] = 0;
                    }
                }
                else if (pm == PaddingMode.Zeros)
                {
                    // 이것은 복호화가 문제가 생길듯 하다. 원본이 0이 들어가지 않는 경우에 가능하다.
                    for (int i = source.Length; i < target_size; i++)
                    {
                        target[i] = 0;
                    }
                }
                else if (pm == PaddingMode.ISO10126)
                {
                    for (int i = source.Length; i < target_size; i++)
                    {
                        if (i == target_size - 1)
                            target[i] = (byte)remains;
                        else
                            target[i] = (byte)i;        // Random data
                    }
                }
            }

            ARIAEngine crypto;
            crypto = new ARIAEngine();

            crypto.setKeySize(key.Length * 8);
            crypto.setKey(key);
            crypto.setupRoundKeys();

            if (cm == CipherMode.CBC)
            {
                for (int i = 0; i < target_size; i += 16)
                {
                    if (i == 0)
                        Xor(target, 0, IV, 0);
                    else
                        Xor(target, i, target, i - 16);

                    crypto.encrypt(target, i, buffer, 0);
                    Array.Copy(buffer, 0, target, i, 16);
                }
            }
            else if (cm == CipherMode.CFB)
            {
                Array.Copy(IV, 0, vector, 0, 16);

                for (int i = 0; i < target_size; i += 16)
                {
                    crypto.encrypt(vector, 0, buffer, 0);
                    Xor(target, i, buffer, 0);
                    Array.Copy(target, i, vector, 0, 16);
                }
            }
            else if (cm == CipherMode.OFB)
            {
                Array.Copy(IV, 0, vector, 0, 16);

                for (int i = 0; i < target_size; i += 16)
                {
                    crypto.encrypt(vector, 0, buffer, 0);
                    Array.Copy(buffer, 0, vector, 0, 16);
                    Xor(target, i, buffer, 0);
                }
            }
            else if (cm == CipherMode.CTS)
            {
                // CTS가 아니고 CTR로 계산했다.
                int count = 0;
                for (int i = 0; i < target_size; i += 16, count++)
                {
                    //Array.Copy(IV, 0, vector, 0, 16);
                    vector[15] = (byte)(count >> 0 & 0xFF);
                    vector[14] = (byte)(count >> 8 & 0xFF);
                    vector[13] = (byte)(count >> 16 & 0xFF);
                    vector[12] = (byte)(count >> 24 & 0xFF);
                    crypto.encrypt(vector, 0, buffer, 0);
                    Xor(target, i, buffer, 0);
                }
            }
            else
            {
                for (int i = 0; i < target_size; i += 16)
                {
                    crypto.encrypt(target, i, buffer, 0);
                    Array.Copy(buffer, 0, target, i, 16);
                }
            }

            return target;
        }

        public static byte[] Decrypt(byte[] source, byte[] key, byte[] IV, CipherMode cm, PaddingMode pm)
        {
            if(source.Length % 16 != 0) 
                throw new Exception("source.Length 가 16의 배수가 아닙니다.");

            int target_size = source.Length;

            byte[] target = new byte[target_size];
            byte[] buffer = new byte[16];
            byte[] vector = new byte[16];

            Array.Copy(source, target, source.Length);

            ARIAEngine crypto;
            crypto = new ARIAEngine();

            crypto.setKeySize(key.Length * 8);
            crypto.setKey(key);
            crypto.setupRoundKeys();

            if (cm == CipherMode.CBC)
            {
                for (int i = 0; i < target_size; i += 16)
                {
                    crypto.decrypt(target, i, buffer, 0);

                    if (i == 0)
                        Xor(buffer, 0, IV, 0);
                    else
                        Xor(buffer, 0, source, i - 16);

                    Array.Copy(buffer, 0, target, i, 16);
                }
            }
            else if (cm == CipherMode.CFB)
            {
                Array.Copy(IV, 0, vector, 0, 16);

                for (int i = 0; i < target_size; i += 16)
                {
                    crypto.encrypt(vector, 0, buffer, 0);   // CFB는 encryption을 사용한다.
                    Array.Copy(target, i, vector, 0, 16);
                    Xor(target, i, buffer, 0);
                }
            }
            else if (cm == CipherMode.OFB)
            {
                Array.Copy(IV, 0, vector, 0, 16);

                for (int i = 0; i < target_size; i += 16)
                {
                    crypto.encrypt(vector, 0, buffer, 0);   // OFB는 encryption을 사용한다.
                    Array.Copy(buffer, 0, vector, 0, 16);
                    Xor(target, i, buffer, 0);
                }
            }
            else if (cm == CipherMode.CTS)
            {
                // CTS가 아니고 CTR로 계산했다.
                int count = 0;
                for (int i = 0; i < target_size; i += 16, count++)
                {
                    //Array.Copy(IV, 0, vector, 0, 16);
                    vector[15] = (byte)(count >> 0 & 0xFF);
                    vector[14] = (byte)(count >> 8 & 0xFF);
                    vector[13] = (byte)(count >> 16 & 0xFF);
                    vector[12] = (byte)(count >> 24 & 0xFF);
                    crypto.encrypt(vector, 0, buffer, 0);   // CTR는 encryption을 사용한다.
                    Xor(target, i, buffer, 0);
                }
            }
            else
            {
                for (int i = 0; i < target_size; i += 16)
                {
                    crypto.decrypt(target, i, buffer, 0);

                    Array.Copy(buffer, 0, target, i, 16);
                }
            }

            int remains;

            if (pm == PaddingMode.None)
                remains = 0;
            else if (pm == PaddingMode.Zeros)
            {
                int i;
                for (i = target_size - 1; i > 0; i--)
                {
                    if (target[i] != 0) break;
                }

                remains = target_size - i-1;
            }
            else
            {
                remains = target[target_size - 1];
            }

            if (remains > 16) remains = 0;

            byte[] target2 = new byte[target_size - remains];

            Array.Copy(target, target2, target2.Length);

            return target2;
        }

    }
}


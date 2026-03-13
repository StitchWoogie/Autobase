#include "stdafx.h"

#ifdef _ATMEL_CONTROLLER
#include "AtmelLib.h"
#include "string.h"
#else
#include <tools.h>
#endif
#include <Crypto.h>

static void Xor(byte *buffer, int ib, byte *iv, int ii)
{
    for (int i = 0; i < 16; i++)
    {
        buffer[ib + i] ^= iv[ii + i];
    }
}

int EncKeySetup(const byte *w0, byte *e, int keyBits);
int DecKeySetup(const byte *w0, byte *d, int keyBits);
void Crypt(const byte *p, int R, const byte *e, byte *c);

BYTE *CryptoARIA_Encrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size)
{
	//if (pm == PaddingMode.None && (source.Length % 16) != 0)
    //{
    //    throw new Exception("PaddingMode.None 일경우는 source.Length가 16의 배수이어야 합니다.");
    //}

    if (pm == PaddingMode_None)
        *target_size = source_Length;    // 같은 크기를 유지한다.
    else
        *target_size = (source_Length + 16) / 16 * 16; // 16의 배수를 끊는다.    16의 배수가 맞을 떄는 한개더 블럭을 넣는다.

    byte* target = (byte*)malloc(*target_size);
    byte* buffer = (byte*)malloc(16);
    byte* vector = (byte*)malloc(16);

	memcpy(target, source, source_Length);

    int remains = *target_size - source_Length;

    if (remains > 0) 
    {
        if (pm == PaddingMode_PKCS7)
        {
            for (int i = source_Length; i < *target_size; i++)
            {
                target[i] = (byte)remains;
            }
        }
        else if (pm == PaddingMode_ANSIX923)
        {
            for (int i = source_Length; i < *target_size; i++)
            {
                if (i == *target_size - 1) 
                    target[i] = (byte)remains;
                else
                    target[i] = 0;
            }
        }
        else if (pm == PaddingMode_Zeros)
        {
            // 이것은 복호화가 문제가 생길듯 하다. 원본이 0이 들어가지 않는 경우에 가능하다.
            for (int i = source_Length; i < *target_size; i++)
            {
                target[i] = 0;
            }
        }
        else if (pm == PaddingMode_ISO10126)
        {
            for (int i = source_Length; i < *target_size; i++)
            {
                if (i == *target_size - 1)
                    target[i] = (byte)remains;
                else
                    target[i] = (byte)i;        // Random data
            }
        }
    }

	byte rk[16*17];
    int R = EncKeySetup(key, rk, key_Length * 8);

    if (cm == CipherMode_CBC)
    {
        for (int i = 0; i < *target_size; i += 16)
        {
            if (i == 0)
                Xor(target, 0, IV, 0);
            else
                Xor(target, i, target, i - 16);

			memcpy(vector, &target[i], 16);

			Crypt(vector, R, rk, buffer);
			memcpy(&target[i], buffer, 16);
        }
    }
    else if (cm == CipherMode_CFB)
    {
		memcpy(vector, IV, 16);

        for (int i = 0; i < *target_size; i += 16)
        {
            Crypt(vector, R, rk, buffer);
            Xor(target, i, buffer, 0);
			memcpy(vector, &target[i], 16);
        }
    }
    else if (cm == CipherMode_OFB)
    {
        memcpy(vector, IV, 16);

        for (int i = 0; i < *target_size; i += 16)
        {
            Crypt(vector, R, rk, buffer);
            memcpy(vector, buffer, 16);
            Xor(target, i, buffer, 0);
        }
    }
    else if (cm == CipherMode_CTS)
    {
        // CTS가 아니고 CTR로 계산했다.
        int count = 0;
        for (int i = 0; i < *target_size; i += 16, count++)
        {
            memset(vector, 0, 16);	// 메모리 클리어
            vector[15] = (byte)(count >> 0 & 0xFF);
            vector[14] = (byte)(count >> 8 & 0xFF);
            vector[13] = (byte)(count >> 16 & 0xFF);
            vector[12] = (byte)(count >> 24 & 0xFF);
            Crypt(vector, R, rk, buffer);
            Xor(target, i, buffer, 0);
        }
    }
    else
    {
        for (int i = 0; i < *target_size; i += 16)
        {
			memcpy(vector, &target[i], 16);
            Crypt(vector, R, rk, buffer);
            memcpy(&target[i], buffer, 16);
        }
    }

	free(buffer);
	free(vector);

	return target;
}

BYTE *CryptoARIA_Decrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size)
{
	//if(source.Length % 16 != 0) 
      //          throw new Exception("source.Length 가 16의 배수가 아닙니다.");

	*target_size = source_Length;

    byte *target = (byte*)malloc(*target_size);
    byte *buffer = (byte*)malloc(16);
    byte *vector = (byte*)malloc(16);

	memcpy(target, source, source_Length);

	byte rk[16*17];
    int R;// = DecKeySetup(key, rk, key_Length * 8);

	if (cm == CipherMode_CBC)
    {
		R = DecKeySetup(key, rk, key_Length * 8);

        for (int i = 0; i < *target_size; i += 16)
        {
			memcpy(vector, &target[i], 16);

			Crypt(vector, R, rk, buffer);

            if (i == 0)
                Xor(buffer, 0, IV, 0);
            else
                Xor(buffer, 0, source, i - 16);

            memcpy(&target[i], buffer, 16);
        }
    }
    else if (cm == CipherMode_CFB)
    {
		R = EncKeySetup(key, rk, key_Length * 8);

        memcpy(vector, IV, 16);

        for (int i = 0; i < *target_size; i += 16)
        {
            Crypt(vector, R, rk, buffer);
			memcpy(vector, &target[i], 16);
            Xor(target, i, buffer, 0);
        }
    }
    else if (cm == CipherMode_OFB)
    {
		R = EncKeySetup(key, rk, key_Length * 8);

        memcpy(vector, IV, 16);

        for (int i = 0; i < *target_size; i += 16)
        {
            Crypt(vector, R, rk, buffer);
            memcpy(vector, buffer, 16);
            Xor(target, i, buffer, 0);
        }
    }
    else if (cm == CipherMode_CTS)
    {
		R = EncKeySetup(key, rk, key_Length * 8);

        // CTS가 아니고 CTR로 계산했다.
        int count = 0;
        for (int i = 0; i < *target_size; i += 16, count++)
        {
            memset(vector, 0, 16);	// 메모리 클리어
            vector[15] = (byte)(count >> 0 & 0xFF);
            vector[14] = (byte)(count >> 8 & 0xFF);
            vector[13] = (byte)(count >> 16 & 0xFF);
            vector[12] = (byte)(count >> 24 & 0xFF);
            Crypt(vector, R, rk, buffer);
            Xor(target, i, buffer, 0);
        }
    }
    else
    {
		R = DecKeySetup(key, rk, key_Length * 8);

        for (int i = 0; i < *target_size; i += 16)
        {
			memcpy(vector, &target[i], 16);
            Crypt(vector, R, rk, buffer);
            memcpy(&target[i], buffer, 16);
        }
    }

	int remains;

    if (pm == PaddingMode_None)
        remains = 0;
    else if (pm == PaddingMode_Zeros)
    {
        int i;
        for (i = *target_size - 1; i > 0; i--)
        {
            if (target[i] != 0) break;
        }

        remains = *target_size - i-1;
    }
    else
    {
        remains = target[*target_size - 1];
    }

	if(remains > 16)
		remains = 0;

	*target_size -= remains;

	free(buffer);
	free(vector);

    return target;
}


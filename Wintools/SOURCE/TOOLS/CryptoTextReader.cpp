#include "stdafx.h"
#include <dos.h>

#include <tools.h>
#include <Crypto.h>
#include <sha1.h>

static BYTE *ReadAllBytes(const TCHAR *filename, int *file_size)
{
	long size = getfilesize(filename);

	if(size == 0)	return NULL;

	*file_size = size;

	BYTE *buffer = new BYTE[size];

	FILE *in;

	in = fopen(filename, "rb");
	if(in == NULL)	return NULL;
	fread(buffer, 1, size, in);
	fclose(in);

	return buffer;
}

// 나중에 SHA256을 위해서 32byte로 준비한다.
static void Make20To32(byte *source, byte *target)
{
    for (int i = 0; i < 32; i++)
    {
        target[i] = (byte)(i ^ source[i % 20]);
    }
}

static BYTE key[] = { 0x11, 0x63, 0x43, 0x54, 0x64, 0x70, 0x78, 0x89, 0xf9, 0xda, 0xc6, 0xc1, 0x80, 0xb0, 0xfc, 0x1d };
static BYTE IV[] = { 0x10, 0x62, 0x03, 0x04, 0x54, 0x60, 0x75, 0x83, 0x79, 0x0a, 0xb8, 0xc0, 0xd0, 0xe0, 0xf0, 0x12 };

CryptoTextReader :: CryptoTextReader(const TCHAR *filename)
{
	reader = NULL;
	pBuffer = NULL;

	int all_bytes_Length;
	BYTE* all_bytes = ReadAllBytes(filename, &all_bytes_Length);

	if (all_bytes_Length >= 96 &&
        all_bytes[0] == 'E' && all_bytes[1] == 'n' && all_bytes[2] == 'c' && all_bytes[3] == 'r' && all_bytes[4] == 'y' &&
        all_bytes[5] == 'p' && all_bytes[6] == 't' && all_bytes[7] == 'e' && all_bytes[8] == 'd' && all_bytes[9] == 'F' &&
        all_bytes[10] == 'i' && all_bytes[11] == 'l' && all_bytes[12] == 'e')
    {
        byte cVersionMajor = (all_bytes[13]);
        byte cVersionMinor = (all_bytes[14]);
        byte cVersionBuild = (all_bytes[15]);

        byte crc[32];
        if (cVersionMajor >= 2)
        {
            //SHA256 sha256 = new SHA256CryptoServiceProvider();
            //crc = sha256.ComputeHash(all_bytes, 10, all_bytes.Length - 32 - 10);
        }
        else
        {
			SHA1 sha1;
			BYTE result[20];

			sha1.Input(&all_bytes[10], all_bytes_Length - 32 - 10);
			sha1.ResultBytes(result);
			
			Make20To32(result, crc);
        }

		if (memcmp(crc, &all_bytes[all_bytes_Length - 32], 32) != 0)
        {
			MessageBox(NULL, "File CRC mismatched. ", filename, MB_OK);
            return;
        }

		int passcode_size;
        byte *passcode = CryptoAES_Decrypt(&all_bytes[16], 32, key, 16, IV, 16, CipherMode_CFB, PaddingMode_None, &passcode_size);
		int source_size;
        byte *source = CryptoAES_Decrypt(&all_bytes[48], all_bytes_Length - 80, passcode, passcode_size, IV, 16, CipherMode_CBC, PaddingMode_PKCS7, &source_size);

		delete all_bytes;
		delete passcode;

		pBuffer = source;
				
        reader = new MemoryTextReader(pBuffer, source_size);
    }
    else
    {
		pBuffer = all_bytes;
        reader = new MemoryTextReader(pBuffer, all_bytes_Length);
    }

	/*
	int file_size;
	pBuffer = ReadAllBytes(filename, &file_size);

	reader = new MemoryTextReader(pBuffer, file_size);*/
} 

CryptoTextReader::~CryptoTextReader()
{
	if(pBuffer)	{
		delete pBuffer;
		pBuffer = NULL;
	}
}

bool CryptoTextReader :: ReadLine(CString& one_line)
{
	if(reader == NULL) {
		return false;
	}

	return reader->ReadLineByUTF8(one_line);
}

void CryptoTextReader :: Close()
{
	if(reader) {
		delete reader;
		reader = NULL;
	}
}



#include "stdafx.h"

#include <tools.h>
#include <crypto.h>

#include "plc_scan.h"

#define sCommand "CryptoCommunication"

CryptoCommunication :: CryptoCommunication()
{
	pKey = NULL;
	pIV = NULL;
	pResultSend = NULL;
	pRecvRing = new BYTE[4096];

	dataGather = new BYTE[4096];
	nGatherData = 0;
	dle_flag = false;
	bStartFlag = false;


	nRecvRingTarget = 0;
	nRecvRingCurrent = 0;

	bUseEncryption = false;
	sEngine = "AES";
    sCipherMode = "CBC";
    sPaddingMode = "PKCS7";
    sKey = "000102030405060708090A0B0C0D0E0F";
	sIV = "00112233445566778899AABBCCDDEEFF";
    nFrameMode = 0;  // 0 = DLE Byte stuffing

	ConvertStringToVars();
}

CryptoCommunication :: ~CryptoCommunication()
{
	FreeResultSend();	
	FreeKey();
	FreeIV();

	delete dataGather;
	delete pRecvRing;
}

void CryptoCommunication :: FreeResultSend()
{
	if(pResultSend) 
	{
		delete pResultSend;
		pResultSend = NULL;
	}
}

void CryptoCommunication :: FreeKey()
{
	if(pKey) 
	{
		delete pKey;
		pKey = NULL;
	}
}

void CryptoCommunication :: FreeIV()
{
	if(pIV) 
	{
		delete pIV;
		pIV = NULL;
	}
}

void CryptoCommunication :: ConvertStringToVars()
{
	FreeKey();
	FreeIV();

	nKeyLength = strlen(sKey)/2;
	pKey = new BYTE[nKeyLength];
	char imsi[3];
	for(int i = 0; i < nKeyLength; i++) 
	{
		imsi[0] = sKey[i*2];
		imsi[1] = sKey[i*2+1];
		pKey[i] = HexBufToBYTE(imsi);
	}

	nIVLength = strlen(sIV)/2;
	pIV = new BYTE[nIVLength];

	for(int i = 0; i < nIVLength; i++) 
	{
		imsi[0] = sIV[i*2];
		imsi[1] = sIV[i*2+1];
		pIV[i] = HexBufToBYTE(imsi);
	}

	if(strcmp(sCipherMode, "CBC") == 0)
		eCM = CipherMode_CBC;
	else if(strcmp(sCipherMode, "OFB") == 0)
		eCM = CipherMode_OFB;
	else if(strcmp(sCipherMode, "CFB") == 0)
		eCM = CipherMode_CFB;
	else if(strcmp(sCipherMode, "CTS") == 0)
		eCM = CipherMode_CTS;
	else 
		eCM = CipherMode_ECB;

	if(strcmp(sPaddingMode, "PKCS7") == 0)
		ePM = PaddingMode_PKCS7;
	else if(strcmp(sPaddingMode, "Zeros") == 0)
		ePM = PaddingMode_Zeros;
	else if(strcmp(sPaddingMode, "ANSIX923") == 0)
		ePM = PaddingMode_ANSIX923;
	else if(strcmp(sPaddingMode, "ISO10126") == 0)
		ePM = PaddingMode_ISO10126;
	else 
		ePM = PaddingMode_None;

	if (strcmp(sEngine, "ARIA") == 0)
        eCE = CryptoEngine_ARIA;
    else
        eCE = CryptoEngine_AES;
}

bool CryptoCommunication :: IsCommand(const char *command)
{
    if(strcmp(sCommand, command) == 0)	return true; 
	return false;
}

void CryptoCommunication :: Load(CryptoTextReader *reader)
{
	CommaBlockString comma;//CommaTextReader comma = new CommaTextReader();
    CString command;
    CString buf;
    CString one_line;

    while (true)
    {
        if(!reader->ReadLine(one_line))	break;
        
        comma.Set(one_line);
        comma.GetString(command);

        if (IsCommand(command))
        {
            comma.GetString(buf);
            if (stricmp(buf, "END") == 0) break;
        }
        else if (command == "Option")
        {
            comma.GetBool(bUseEncryption);
            comma.GetString(sEngine);
            comma.GetString(sCipherMode);
            comma.GetString(sPaddingMode);
            comma.GetString(sKey);
            comma.GetString(sIV);
            comma.GetInt(nFrameMode);
        }
    }

	ConvertStringToVars();
}

/*
BYTE *CryptoARIA_Encrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, CipherMode cm, PaddingMode pm, int &target_size);
BYTE *CryptoARIA_Decrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, CipherMode cm, PaddingMode pm, int &target_size);

BYTE *CryptoAES_Encrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, CipherMode cm, PaddingMode pm, int &target_size);
BYTE *CryptoAES_Decrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, CipherMode cm, PaddingMode pm, int &target_size);*/



BYTE* CryptoCommunication :: GetEncryptionData(BYTE *source, int source_size, int &target_size)
{
	if(!bUseEncryption) {
		target_size = source_size;
		return source;
	}

	FreeResultSend();

	int result_size;

	BYTE *result;

	if(eCE == CryptoEngine_ARIA)
		result = CryptoARIA_Encrypt(source, source_size, pKey, nKeyLength, pIV, nIVLength, eCM, ePM, &result_size);
	else 
		result = CryptoAES_Encrypt(source, source_size, pKey, nKeyLength, pIV, nIVLength, eCM, ePM, &result_size);

	if (nFrameMode == 0)
    {
        int dle_count = 0;
        for (int i = 0; i < result_size; i++)
        {
            if (result[i] == 0x10)
                dle_count++;
        }

		int new_frame_size = 4 + dle_count + result_size;
        byte *new_frame = new byte[new_frame_size];

        int pos = 0;
        new_frame[pos++] = 0x10;
        new_frame[pos++] = 0x02;
        for (int i = 0; i < result_size; i++)
        {
            if (result[i] == 0x10)
                new_frame[pos++] = 0x10;
            new_frame[pos++] = result[i];
        }
        new_frame[pos++] = 0x10;
        new_frame[pos++] = 0x03;

		delete result;

		target_size = new_frame_size;
        pResultSend = new_frame;
    }
    else
    {
		target_size = result_size;
        pResultSend = result;
    }

	return pResultSend;
}

void CryptoCommunication :: AddRecvRing(BYTE ch)
{
	pRecvRing[nRecvRingTarget] = ch;
	nRecvRingTarget ++;
	nRecvRingTarget %= 4096;
}

int CryptoCommunication :: GetDecryptionData(BYTE *source, int count)
{
	if(nRecvRingTarget == nRecvRingCurrent)	return 0;

	int remain;
	
	if(nRecvRingTarget > nRecvRingCurrent)
		remain = nRecvRingTarget-nRecvRingCurrent;
	else 
		remain = nRecvRingTarget+4096-nRecvRingCurrent;

	if(remain < count)	count = remain;

	if(count <= 0)	return 0;

	for(int i = 0; i < count; i++) {
		source[i] = pRecvRing[nRecvRingCurrent];
		nRecvRingCurrent ++;
		nRecvRingCurrent %= 4096;
	}
		
	return count;
}

void CryptoCommunication :: SetEncryptedData(BYTE ch)
{
	// check buffer over
    if (nGatherData >= 4096)
    {
        nGatherData = 0;
    }

    if (nFrameMode == 0)
    {
        if (dle_flag)
        {
            dle_flag = false;

            if (ch == 0x02) // STX 가 들어오면 처음부터 시작한다.
            {
                bStartFlag = true;
                nGatherData = 0;
            }
            else if (ch == 0x03)
            {
                if (bStartFlag) // STX가 있는 경우에만 의미가 있다.
                {
                    goto ok_oneframe_received;
                }

                bStartFlag = false;
            }
            else if (ch == 0x10)
            {
                dataGather[nGatherData++] = ch;
            }
            else
            {

            }
        }
        else
        {
            if (ch == 0x10)
            {
                dle_flag = true;
            }
            else
            {
                if(bStartFlag) 
                    dataGather[nGatherData++] = ch;
            }
        }
        
    }
    else
    {

    }

    return;	// 아직은 해석할 단계가 아니다.

ok_oneframe_received: ;

	if ((nGatherData % 16) != 0)
    {
        dle_flag = false;
        nGatherData = 0;
        bStartFlag = false;
        return;
    }

    int result_size;

	BYTE *result;
	
	if(eCE == CryptoEngine_ARIA)
		result = CryptoARIA_Decrypt(dataGather, nGatherData, pKey, nKeyLength, pIV, nIVLength, eCM, ePM, &result_size);
	else 
		result = CryptoAES_Decrypt(dataGather, nGatherData, pKey, nKeyLength, pIV, nIVLength, eCM, ePM, &result_size);

	for(int i = 0; i < result_size; i++) {
		AddRecvRing(result[i]);
	}

	delete result;
	
    dle_flag = false;
    nGatherData = 0;
    bStartFlag = false;
}


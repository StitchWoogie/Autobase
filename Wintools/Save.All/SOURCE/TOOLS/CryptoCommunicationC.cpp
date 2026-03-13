#include "stdafx.h"

#ifdef _ATMEL_CONTROLLER
#include "AtmelLib.h"
#include "string.h"
#else
#include <tools.h>
#endif
#include "crypto.h"

//#include "plc_scan.h"

#define sCommand "CryptoCommunication"

void FreeResultSend(CRYPTO_INFO *pInfo)
{
	if(pInfo->pResultSend) 
	{
		free(pInfo->pResultSend);
		pInfo->pResultSend = NULL;
	}
}

void FreeKey(CRYPTO_INFO *pInfo)
{
	if(pInfo->pKey) 
	{
		free(pInfo->pKey);
		pInfo->pKey = NULL;
	}
}

void FreeIV(CRYPTO_INFO *pInfo)
{
	if(pInfo->pIV) 
	{
		free(pInfo->pIV);
		pInfo->pIV = NULL;
	}
}

void InitCryptoSetting(CRYPTO_SETTING *pSetting, BYTE bSettingPort)
{
	pSetting->nKeyLength = 16;
	if(bSettingPort)
	{
		pSetting->bUse = true;
		byte pKey[] = {0x01, 0x25, 0x34, 0x78, 0x95, 0x67, 0x02, 0x71, 0xAB, 0xEF, 0x43, 0x28, 0xBC, 0xDA, 0xDE, 0xFA};
		byte pIV[] = {0xAF, 0xED, 0xAD, 0xCB, 0x82, 0x34, 0xFE, 0xBA, 0x17, 0x20, 0x72, 0x34, 0x5A, 0xDE, 0x28, 0x6F};

		memcpy(pSetting->pKey, pKey, 16);
		memcpy(pSetting->pIV, pIV, 16);
	}
	else
	{
		pSetting->bUse = false;
		byte pKey[] = {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F};
		byte pIV[] = {0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF};

		memcpy(pSetting->pKey, pKey, 16);
		memcpy(pSetting->pIV, pIV, 16);
	}
	pSetting->eCE = CryptoEngine_AES;
    pSetting->eCM = CipherMode_CBC;
    pSetting->ePM = PaddingMode_PKCS7;

	pSetting->nFrameMode = 0;  // 0 = DLE Byte stuffing
}

void InitCryptoInfo(CRYPTO_INFO *pInfo, CRYPTO_SETTING *pSetting)
{
	pInfo->bUse = pSetting->bUse;

	pInfo->nKeyLength = pSetting->nKeyLength;
	pInfo->pKey = (BYTE*)malloc(pInfo->nKeyLength);
	memcpy(pInfo->pKey, pSetting->pKey, pSetting->nKeyLength);

	pInfo->nIVLength = 16;
	pInfo->pIV = (BYTE*)malloc(pInfo->nIVLength);
	memcpy(pInfo->pIV, pSetting->pIV, pInfo->nIVLength);

	pInfo->eCE = pSetting->eCE;
	pInfo->eCM = pSetting->eCM;
	pInfo->ePM = pSetting->ePM;
	pInfo->nFrameMode = pSetting->nFrameMode;

	pInfo->pResultSend = NULL;
	pInfo->pRecvRing = (BYTE*)malloc(MAX_CRYPTO_COMM_BUF);

	pInfo->dataGather = (BYTE*)malloc(MAX_CRYPTO_COMM_BUF);
	pInfo->nGatherData = 0;
	pInfo->dle_flag = false;
	pInfo->bStartFlag = false;


	pInfo->nRecvRingTarget = 0;
	pInfo->nRecvRingCurrent = 0;
}

void DeleteCryptoInfo(CRYPTO_INFO *pInfo)
{
	FreeResultSend(pInfo);	
	FreeKey(pInfo);
	FreeIV(pInfo);

	free(pInfo->dataGather);
	free(pInfo->pRecvRing);
}


bool IsCommand(const char *command)
{
    if(strcmp(sCommand, command) == 0)	return true; 
	return false;
}

/*
BYTE *CryptoARIA_Encrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size);
BYTE *CryptoARIA_Decrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size);

BYTE *CryptoAES_Encrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size);
BYTE *CryptoAES_Decrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size);*/

BYTE* GetEncryptionData(CRYPTO_INFO *pInfo, BYTE *source, int source_size, int *target_size)
{
	if(!pInfo->bUse) {
		*target_size = source_size;
		return source;
	}

	FreeResultSend(pInfo);

	int result_size;

	BYTE *result;

	if(pInfo->eCE == CryptoEngine_ARIA)
		result = CryptoARIA_Encrypt(source, source_size, pInfo->pKey, pInfo->nKeyLength, pInfo->pIV, pInfo->nIVLength, pInfo->eCM, pInfo->ePM, &result_size);
	else 
		result = CryptoAES_Encrypt(source, source_size, pInfo->pKey, pInfo->nKeyLength, pInfo->pIV, pInfo->nIVLength, pInfo->eCM, pInfo->ePM, &result_size);

	if (pInfo->nFrameMode == 0)
    {
        int dle_count = 0;
        for (int i = 0; i < result_size; i++)
        {
            if (result[i] == 0x10)
                dle_count++;
        }

		int new_frame_size = 4 + dle_count + result_size;
        byte *new_frame = (byte*)malloc(new_frame_size);

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

		free(result);

		*target_size = new_frame_size;
        pInfo->pResultSend = new_frame;
    }
    else
    {
		*target_size = result_size;
        pInfo->pResultSend = result;
    }

	return pInfo->pResultSend;
}

void AddRecvRing(CRYPTO_INFO *pInfo, BYTE ch)
{
//	printf("Add [%x], target = %d, current = %d\n", ch, pInfo->nRecvRingTarget, pInfo->nRecvRingCurrent);
	pInfo->pRecvRing[pInfo->nRecvRingTarget] = ch;
	pInfo->nRecvRingTarget ++;
	pInfo->nRecvRingTarget %= MAX_CRYPTO_COMM_BUF;
}

//bool GetDecryptionData(CRYPTO_INFO *pInfo, BYTE *ch)
//{
	//if(pInfo->nRecvRingTarget==pInfo->nRecvRingCurrent)
		//return false;
//
	//*ch = pInfo->pRecvRing[pInfo->nRecvRingCurrent];
	//pInfo->nRecvRingCurrent ++;
	//pInfo->nRecvRingCurrent %= MAX_CRYPTO_COMM_BUF;
	//return true;
//}

int GetDecryptionData(CRYPTO_INFO *pInfo, BYTE *source, int count)
{
	if(pInfo->nRecvRingTarget == pInfo->nRecvRingCurrent)	return 0;

	int remain;
	
	if(pInfo->nRecvRingTarget > pInfo->nRecvRingCurrent)
		remain = pInfo->nRecvRingTarget-pInfo->nRecvRingCurrent;
	else 
		remain = pInfo->nRecvRingTarget+MAX_CRYPTO_COMM_BUF-pInfo->nRecvRingCurrent;

	if(remain < count)	count = remain;

	if(count <= 0)	return 0;

	for(int i = 0; i < count; i++) {
		source[i] = pInfo->pRecvRing[pInfo->nRecvRingCurrent];
//		printf("Get [%x], target = %d, current = %d\n", source[i], pInfo->nRecvRingTarget, pInfo->nRecvRingCurrent);
		pInfo->nRecvRingCurrent ++;
		pInfo->nRecvRingCurrent %= MAX_CRYPTO_COMM_BUF;
	}
		
	return count;
}

void SetEncryptedData(CRYPTO_INFO *pInfo, BYTE ch)
{
	// check buffer over
    if (pInfo->nGatherData >= 4096)
    {
        pInfo->nGatherData = 0;
    }

    if (pInfo->nFrameMode == 0)
    {
        if (pInfo->dle_flag)
        {
            pInfo->dle_flag = false;

            if (ch == 0x02) // STX 가 들어오면 처음부터 시작한다.
            {
                pInfo->bStartFlag = true;
                pInfo->nGatherData = 0;
            }
            else if (ch == 0x03)
            {
                if (pInfo->bStartFlag) // STX가 있는 경우에만 의미가 있다.
                {
                    goto ok_oneframe_received;
                }

                pInfo->bStartFlag = false;
            }
            else if (ch == 0x10)
            {
                pInfo->dataGather[pInfo->nGatherData++] = ch;
            }
            else
            {

            }
        }
        else
        {
            if (ch == 0x10)
            {
                pInfo->dle_flag = true;
            }
            else
            {
                if(pInfo->bStartFlag) 
                    pInfo->dataGather[pInfo->nGatherData++] = ch;
            }
        }
        
    }
    else
    {

    }

    return;	// 아직은 해석할 단계가 아니다.

ok_oneframe_received: ;
	if(pInfo->nGatherData%16!=0)
	{
		pInfo->dle_flag = false;
		pInfo->nGatherData = 0;
		pInfo->bStartFlag = false;
		return;
	}

    int result_size;

	BYTE *result;
	
	if(pInfo->eCE == CryptoEngine_ARIA)
		result = CryptoARIA_Decrypt(pInfo->dataGather, pInfo->nGatherData, pInfo->pKey, pInfo->nKeyLength, pInfo->pIV, pInfo->nIVLength, pInfo->eCM, pInfo->ePM, &result_size);
	else 
		result = CryptoAES_Decrypt(pInfo->dataGather, pInfo->nGatherData, pInfo->pKey, pInfo->nKeyLength, pInfo->pIV, pInfo->nIVLength, pInfo->eCM, pInfo->ePM, &result_size);

	for(int i = 0; i < result_size; i++) {
		AddRecvRing(pInfo, result[i]);
	}

	free(result);
	
    pInfo->dle_flag = false;
    pInfo->nGatherData = 0;
    pInfo->bStartFlag = false;
}


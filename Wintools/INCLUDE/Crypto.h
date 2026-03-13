#if !defined(__Crypto_H)
#define __Crypto_H

#ifndef _ATMEL_CONTROLLER
#include <tools.h>
#endif

#pragma pack(push, 1)

typedef unsigned char BYTE;
#define NULL    0

enum CipherMode
{
        CipherMode_CBC = 0,
        CipherMode_ECB = 1,
        CipherMode_OFB = 2,
        CipherMode_CFB = 3,
        CipherMode_CTS = 4,
};

enum PaddingMode
{
        PaddingMode_None = 0,
        PaddingMode_PKCS7 = 1,
        PaddingMode_Zeros = 2,
        PaddingMode_ANSIX923 = 3,
        PaddingMode_ISO10126 = 4,
};

enum EnumCryptoEngine
{
    CryptoEngine_AES = 0,
    CryptoEngine_ARIA = 1,
};

#define MAX_CRYPTO_COMM_BUF		1052

#ifndef _ATMEL_CONTROLLER
class CryptoTextReader {
		BYTE *pBuffer;
		MemoryTextReader *reader;
		
	public:
		CryptoTextReader(const TCHAR *filename);
		~CryptoTextReader();
		bool ReadLine(CString& one_line);
		void Close();
		
};

class CryptoCommunication 
{
    CString sCipherMode;
    CString sPaddingMode;
    CString sKey;
	CString sIV;
    int nFrameMode;  // 0 = none, 1 = DLE Byte stuffing

	int nKeyLength;
	BYTE *pKey;

	int nIVLength;
	BYTE *pIV;

	CipherMode eCM;
	PaddingMode ePM;
	EnumCryptoEngine eCE;

	BYTE *pResultSend;
	BYTE *pRecvRing;

	BYTE *dataGather;
	int nGatherData;
	bool dle_flag;
	bool bStartFlag;
	int  nRecvRingTarget;
	int  nRecvRingCurrent;

	void ConvertStringToVars();
	void FreeResultSend();
	void FreeKey();
	void FreeIV();
	void AddRecvRing(byte ch);
public:
	bool bUseEncryption;
	CString sEngine;

	CryptoCommunication();
	~CryptoCommunication();
	bool IsCommand(const char *command);
	void Load(CryptoTextReader *reader);
	BYTE *GetEncryptionData(BYTE *source, int source_size, int &target_size);
	int GetDecryptionData(BYTE *source, int count);
	void SetEncryptedData(BYTE ch);
};
#endif

typedef struct
{
	BYTE bUse;
	
	short nKeyLength;
    char pKey[32];
	char pIV[16];

	BYTE eCE;
	BYTE eCM;
	BYTE ePM;
    BYTE nFrameMode;  // 0 = none, 1 = DLE Byte stuffing
}CRYPTO_SETTING;

typedef struct 
{
	BYTE bUse;

	short nKeyLength;
	BYTE *pKey;
	short nIVLength;
	BYTE *pIV;

	BYTE eCE;
	BYTE eCM;
	BYTE ePM;

    BYTE nFrameMode;  // 0 = none, 1 = DLE Byte stuffing

	BYTE *pResultSend;
	BYTE *pRecvRing;

	BYTE *dataGather;
	short nGatherData;
	BYTE dle_flag;
	BYTE bStartFlag;
	short  nRecvRingTarget;
	short  nRecvRingCurrent;
}CRYPTO_INFO;

void InitCryptoSetting(CRYPTO_SETTING *pSetting, BYTE bSettingPort);

void InitCryptoInfo(CRYPTO_INFO *pInfo, CRYPTO_SETTING *pSetting);
void DeleteCryptoInfo(CRYPTO_INFO *pInfo);

//데이터 전송시 호출하여 디바이스로 전송할 암호화된 데이터를 얻는다.
BYTE* GetEncryptionData(CRYPTO_INFO *pInfo, BYTE *source, int source_size, int *target_size);

//디바이스로 부터 수신한 데이터를 1byte씩 넣는다.
void SetEncryptedData(CRYPTO_INFO *pInfo, BYTE ch);
//내부적으로 처리후 암호화 해제된 데이터를 1byte씩 얻는다.
//bool GetDecryptionData(CRYPTO_INFO *pInfo, BYTE *ch);
//내부적으로 처리후 암호화 해제 되면 그 데이터를 한꺼번에 읽는다.
int GetDecryptionData(CRYPTO_INFO *pInfo, BYTE *source, int count);


BYTE *CryptoARIA_Encrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size);
BYTE *CryptoARIA_Decrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size);

BYTE *CryptoAES_Encrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size);
BYTE *CryptoAES_Decrypt(BYTE *source, int source_Length, BYTE *key, int key_Length, BYTE *IV, int IV_Length, int cm, int pm, int *target_size);

#pragma pack(pop)

#endif

#include <compiler.hpp>

#pragma pack(push, 1)

BYTE GetCRC_SumBYTE(void *buf, int length);		// crc 검사 함수
BYTE GetCRC_SUM8(BYTE *buf, int size);			// GetCRC_SumBYTE와 같음 

WORD GetCRC_SumWORD(void *buf, int length);		// crc 검사 함수
WORD GetCRC16(BYTE *buf, int length);			// SumWORD와 같다.

WORD GetCRC_16_15_13_1(unsigned char *buf, int size);
WORD GetCRC_16_12_5_1(unsigned char *bp, int len);
WORD GetCRC_16_15_2_1(unsigned char *buf, int size);

#pragma pack(pop)

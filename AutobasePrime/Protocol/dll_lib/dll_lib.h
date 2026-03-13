/*

int ProcProtocolWriteBlock(LOCAL_PORT_STRUCT* pt, int station, DWORD address, BYTE* value, short byte_size, BYTE array_type, char* device, WORD pannel, char* sAddress)
{
	char buf[160];
	char atype[80];

	if (array_type == 1)		strcpy(atype, "byte");
	else if (array_type == 4)	strcpy(atype, "ushort");
	else if (array_type == 6)	strcpy(atype, "uint");
	else if (array_type == 8)	strcpy(atype, "ulong");
	else if (array_type == 9)	strcpy(atype, "float");
	else if (array_type == 10)	strcpy(atype, "double");
	else if (array_type == 11)	strcpy(atype, "string");
	else						strcpy(atype, "unknown type");

	sprintf(buf, "This protocol is not supported BLOCK Write. (block_size=%d, block_type=%d(%s))", byte_size, array_type, atype);

	PlcScanSetErrorString(pt, buf);

	return COMMUNICATION_ERR_STRING;
}

*/

#if     !defined (__DLL_LIB_H)
#define __DLL_LIB_H

#include <crc.hpp>

#include "..\..\PLC_SCAN\plc_scan.h"


#include "../../CATLIB.SRC/Totalcfg.h"
#include "../../CATLIB.SRC/CatTag9.h"

#pragma pack(push, 1)

void PlcScanSetErrorString(LOCAL_PORT_STRUCT *pt, const char *string, ...);

void PokeValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, WORD address, double value);
void PokeValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, WORD address, __int64 value);
void PokeValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, WORD address, const char *value);

void PokeWORD (LOCAL_PORT_STRUCT *pt, WORD address, WORD value);
void PokeDWORD(LOCAL_PORT_STRUCT *pt, WORD address, DWORD value);
void PokeFLOAT(LOCAL_PORT_STRUCT *pt, WORD address, FLOAT value);
void PokeSTRING(LOCAL_PORT_STRUCT *pt, WORD address, const char *value);
void PokeDOUBLE(LOCAL_PORT_STRUCT *pt, WORD address, double value);
void PokeINT64(LOCAL_PORT_STRUCT *pt, WORD address, __int64 value);

WORD  PeekValueWORD(LOCAL_PORT_STRUCT *pt, WORD address);
float PeekValueFLOAT(LOCAL_PORT_STRUCT *pt, WORD address);
DWORD PeekValueDWORD(LOCAL_PORT_STRUCT *pt, WORD address);
int   PeekValueSTRING(LOCAL_PORT_STRUCT *pt, WORD address, char *value);
double PeekValueDOUBLE(LOCAL_PORT_STRUCT *pt, WORD address);
__int64 PeekValueINT64(LOCAL_PORT_STRUCT *pt, WORD address);

int  AddWaitWriteDigitalOut(int port, int station, WORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag);
int  AddWaitWriteAnalogOut(int port, int station, WORD address, char *sExtraAddr, WORD wExtraAddr, double value);

void GetProtocolIniFile(int port, char *filename);

int PokeValuePortWORD(int port, int address, WORD val);
int PokeValuePortFLOAT(int port, int address, double val);
int PokeValuePortDWORD(int port, int address, DWORD val);
int PokeValuePortSTRING(int port, int address, char *val);
int PokeValuePortDOUBLE(int port, int address, double val);
int PokeValuePortINT64(int port, int address, __int64 val);

WORD   PeekValuePortWORD(int port, int address);
double PeekValuePortFLOAT(int port, int address);
DWORD  PeekValuePortDWORD(int port, int address);
int    PeekValuePortSTRING(int port, int address, char *val);
double PeekValuePortDOUBLE(int port, int address);
__int64  PeekValuePortINT64(int port, int address);

// 8.7 = double/int64 메모리 추가  10.0.4 부터 적용
	// 8.8 = Block write를 추가하고 통신메모리를 short형에서 int형으로 변환하였다. 2010-12-17 10.2.0 부터 정식 지원
	// 8.9 = PlcDeviceGetInfo, PlcDeviceSetInfo 추가  2012-5-23   

	// PokeValuePortUnicodeSTRING, PeekValuePortUnicodeSTRING 을 2015-7-24 (10.3.0) 부터 지원하려고 했으나 기존의 STRING_BUF를 유니코드로 바꾸면 
	// 이전의 드라이버들이 문제가 생길것 같아서 보류했다. UNICODE_BUF 메모리 영역을 새로 만들어서 지원하는 것이 가장 좋을 듯 하다. 
	// 안전하게 하기위해서 이미 지원된 구조체의 중간은 건드리지 말고 필요하면 마지막에 추가하는 구조로 해야 문제가 없을 듯 하다.

// 8.20 = Autobase48 Studio 2022로 컴파일 된 dll 2025-12-17

// 9.0 = Avalonia/Linux 용으로 지원. 2025-4-21

#define PROTOCOL_VERSION_MAJOR  8
#define PROTOCOL_VERSION_MINOR  20

#pragma pack(pop)

#endif




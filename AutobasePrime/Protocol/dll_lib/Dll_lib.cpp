
//------------------------------------------------------------------------------
//	각 프로토콜 드라이브들이 같이 쓸 수 있는 함수들을 모아 놓았다.
//  8.7  Double과 Int64를 지원한다.
//------------------------------------------------------------------------------

#include <afxwin.h>
#include <dos.h>
#include <string.h>
#include <stdarg.h>
#include <stdio.h>

#include <dataswap.h>

#include "dll_lib.h"

char sDirProgramm[MAXPATH];

//static char *sPlcScanErrorString = NULL; 8.4.3부터 LOCAL_PORT_STRUCT로 추가
//CString sPlcScanErrorString;

void PlcScanSetErrorString(LOCAL_PORT_STRUCT *pt, const char *string, ...)
{
	va_list ap;
	StackChar imsi(1000);

	if(pt->sPlcScanErrorString)	delete pt->sPlcScanErrorString;
	pt->sPlcScanErrorString = NULL;

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);

		pt->sPlcScanErrorString = new char[strlen(imsi.data)+1];
		if(pt->sPlcScanErrorString == NULL) 	return;

		strcpy(pt->sPlcScanErrorString, imsi.data);
	}
}

typedef void (* LPFNPROTOCOLPOKEWORD)  (LOCAL_PORT_STRUCT *pt, WORD address, WORD value);
typedef void (* LPFNPROTOCOLPOKEDWORD) (LOCAL_PORT_STRUCT *pt, WORD address, DWORD value);
typedef void (* LPFNPROTOCOLPOKEFLOAT) (LOCAL_PORT_STRUCT *pt, WORD address, float value);
typedef void (* LPFNPROTOCOLPOKESTRING) (LOCAL_PORT_STRUCT *pt, WORD address, const char *value);

LPFNPROTOCOLPOKEWORD   lpfnProtocolPokeWORD;
LPFNPROTOCOLPOKEDWORD  lpfnProtocolPokeDWORD;
LPFNPROTOCOLPOKEFLOAT  lpfnProtocolPokeFLOAT;
LPFNPROTOCOLPOKESTRING  lpfnProtocolPokeSTRING;

extern "C" void DLLEXPORT ProtocolSetProcPokeWORD(FARPROC proc)
{
	lpfnProtocolPokeWORD = (LPFNPROTOCOLPOKEWORD)proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPokeDWORD(FARPROC proc)
{
	lpfnProtocolPokeDWORD = (LPFNPROTOCOLPOKEDWORD)proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPokeFLOAT(FARPROC proc)
{
	lpfnProtocolPokeFLOAT = (LPFNPROTOCOLPOKEFLOAT)proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPokeSTRING(FARPROC proc)
{
	lpfnProtocolPokeSTRING = (LPFNPROTOCOLPOKESTRING)proc;
}

void PokeWORD(LOCAL_PORT_STRUCT *pt, WORD address, WORD value)
{
	lpfnProtocolPokeWORD(pt, address, value);
}

void PokeFLOAT(LOCAL_PORT_STRUCT *pt, WORD address, float value)
{
	lpfnProtocolPokeFLOAT(pt, address, value);
}

void PokeDWORD(LOCAL_PORT_STRUCT *pt, WORD address, DWORD value)
{
	lpfnProtocolPokeDWORD(pt, address, value);
}

void PokeSTRING(LOCAL_PORT_STRUCT *pt, WORD address, const char *value)
{
	if(lpfnProtocolPokeSTRING)
		lpfnProtocolPokeSTRING(pt, address, value);
}

void PokeDOUBLE(LOCAL_PORT_STRUCT *pt, WORD address, double value)
{
	PokeValuePortDOUBLE(pt->no, address, value);
}

void PokeINT64(LOCAL_PORT_STRUCT *pt, WORD address, __int64 value)
{
	PokeValuePortINT64(pt->no, address, value);
}

void PokeValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, WORD address, double value)
{
	if(sm->cVarType == 0) {			// 0 - WORD, 1 - FLOAT
		PokeWORD(pt, address, (WORD)value);
	}
	else if(sm->cVarType == 1) {			// 0 - WORD, 1 - FLOAT
		PokeFLOAT(pt, address, (float)value);
	}
	else if(sm->cVarType == 2) {			// 0 - WORD, 1 - FLOAT
		PokeDWORD(pt, address, (DWORD)value);
	}
	else if(sm->cVarType == 3) {			// 3- STRING
		CString buf;
		buf.Format("%f", value);
		PokeSTRING(pt, address, buf);
	}
	else if(sm->cVarType == 4) {			// 0 - WORD, 1 - FLOAT
		PokeDOUBLE(pt, address, value);
	}
	else if(sm->cVarType == 5) {			// 0 - WORD, 1 - FLOAT
		PokeINT64(pt, address, (__int64)value);
	}
	else {
		PokeWORD(pt, address, (WORD)value);
	}
}

void PokeValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, WORD address, __int64 value)
{
	if(sm->cVarType == 0) {			// 0 - WORD, 1 - FLOAT
		PokeWORD(pt, address, (WORD)value);
	}
	else if(sm->cVarType == 1) {			// 0 - WORD, 1 - FLOAT
		PokeFLOAT(pt, address, (float)value);
	}
	else if(sm->cVarType == 2) {			// 0 - WORD, 1 - FLOAT
		PokeDWORD(pt, address, (DWORD)value);
	}
	else if(sm->cVarType == 3) {			// 3- STRING
		CString buf;
		buf.Format("%f", (float)value);
		PokeSTRING(pt, address, buf);
	}
	else if(sm->cVarType == 4) {			// 0 - WORD, 1 - FLOAT
		PokeDOUBLE(pt, address, (double)value);
	}
	else if(sm->cVarType == 5) {			// 0 - WORD, 1 - FLOAT
		PokeINT64(pt, address, value);
	}
	else {
		PokeWORD(pt, address, (WORD)value);
	}
}

void PokeValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, WORD address, const char *value)
{
	if(sm->cVarType == 3) {			// 3- STRING
		PokeSTRING(pt, address, value);
	}
}

typedef void  (* LPFNDISPLAYSENDCODE)  (int port, BYTE code);
typedef void  (* LPFNDISPLAYRECVCODE)  (int port, BYTE code);
typedef void  (* LPFNDISPLAYSENDCODENEXTLINE)(int port);
typedef void  (* LPFNDISPLAYRECVCODENEXTLINE)(int port);

LPFNDISPLAYSENDCODE lpfnDisplaySendCode;
LPFNDISPLAYRECVCODE lpfnDisplayRecvCode;
LPFNDISPLAYSENDCODENEXTLINE lpfnDisplaySendCodeNextLine;
LPFNDISPLAYRECVCODENEXTLINE lpfnDisplayRecvCodeNextLine;

extern "C" void DLLEXPORT ProtocolGetErrMsg(char *msg)
{
	strcpy(msg, "Protocol error message. Message can display in AUTOBASE Version 8.4.3 or later.");
}

extern "C" void DLLEXPORT ProtocolGetErrMsg2(LOCAL_PORT_STRUCT *pt, char *msg)
{
	strcpy(msg, pt->sPlcScanErrorString);
}

extern "C" void DLLEXPORT ProtocolSetProcDisplaySendCode(FARPROC proc)
{
	lpfnDisplaySendCode = (LPFNDISPLAYSENDCODE) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcDisplaySendCodeNextLine(FARPROC proc)
{
	lpfnDisplaySendCodeNextLine = (LPFNDISPLAYSENDCODENEXTLINE) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcDisplayRecvCode(FARPROC proc)
{
	lpfnDisplayRecvCode = (LPFNDISPLAYRECVCODE) proc;
	//strcpy(msg, sPlcScanErrorString);
}

extern "C" void DLLEXPORT ProtocolSetProcDisplayRecvCodeNextLine(FARPROC proc)
{
	lpfnDisplayRecvCodeNextLine = (LPFNDISPLAYRECVCODENEXTLINE) proc;
}

void DisplaySendCode(int port, BYTE code)
{
	lpfnDisplaySendCode(port, code);
}

void DisplaySendCodeString(int port, const char *string)
{
	unsigned u;
	for(u = 0; u < strlen(string); u++) 
		DisplaySendCode(port, string[u]);
}

void DisplayRecvCode(int port, BYTE code)
{
	lpfnDisplayRecvCode(port, code);
}

void DisplayRecvCodeString(int port, const char *string)
{
	unsigned u;
	for(u = 0; u < strlen(string); u++) 
		DisplayRecvCode(port, string[u]);
}

void DisplaySendCodeNextLine(int port)
{
	lpfnDisplaySendCodeNextLine(port);
}

void DisplayRecvCodeNextLine(int port)
{
	lpfnDisplayRecvCodeNextLine(port);
}

typedef void  (* LPFNPROTOCOLSETCODESTARTEND)  (LOCAL_PORT_STRUCT *pt, int s_code, int e_code);
typedef void  (* LPFNPROTOCOLSETCODEMODE)  (LOCAL_PORT_STRUCT *pt, int mode);

LPFNPROTOCOLSETCODESTARTEND lpfnProtocolSetCodeStartEnd;
LPFNPROTOCOLSETCODEMODE lpfnProtocolSetCodeMode;

void ProtocolSetCodeStartEnd(LOCAL_PORT_STRUCT *pt, int s_code, int e_code)
{
	lpfnProtocolSetCodeStartEnd(pt, s_code, e_code);
}

void ProtocolSetCodeMode(LOCAL_PORT_STRUCT *pt, int mode)
{
	lpfnProtocolSetCodeMode(pt, mode);
}

extern "C" void DLLEXPORT ProtocolSetProcCodeStartEnd(FARPROC proc)
{
	lpfnProtocolSetCodeStartEnd = (LPFNPROTOCOLSETCODESTARTEND) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcCodeMode(FARPROC proc)
{
	lpfnProtocolSetCodeMode = (LPFNPROTOCOLSETCODEMODE) proc;
}

extern "C" int DLLEXPORT ProtocolCheckStruct(WORD port_size, WORD method_size, WORD device_size)
{
	if(port_size != sizeof(LOCAL_PORT_STRUCT)) {
		return 1;		
	}
	if(method_size != sizeof(SCAN_METHOD_STRUCT)) {
		return 2;		
	}
	if(device_size != sizeof(DEVICE_STRUCT)) {
		return 3;		
	}
	return 0;
}

typedef int  (* LPFNPLCDEVICEREADCONTINUE) (DEVICE_STRUCT *device, char *buf, int count);
typedef int  (* LPFNPLCDEVICEWRITECONTINUE) (DEVICE_STRUCT *device, char *buf, int count);
typedef int  (* LPFNPLCDEVICECLEAR) (DEVICE_STRUCT *device);
typedef int  (* LPFNPLCDEVICEENABLE) (DEVICE_STRUCT *device, char flag);
typedef int  (* LPFNPLCDEVICESETCOMMSTATE) (DEVICE_STRUCT *device, DWORD baud, char parity, char data, char stop);
typedef int  (* LPFNPLCDEVICESETCOMMBREAK) (DEVICE_STRUCT *device);
typedef int  (* LPFNPLCDEVICECLEARCOMMBREAK) (DEVICE_STRUCT *device);

typedef int  (* LPFNPLCDEVICEGETINFO) (DEVICE_STRUCT *device, int item, BYTE *buf, int size);
typedef int  (* LPFNPLCDEVICESETINFO) (DEVICE_STRUCT *device, int item, BYTE *buf, int size);

LPFNPLCDEVICEREADCONTINUE  lpfnPlcDeviceReadContinue;
LPFNPLCDEVICEWRITECONTINUE lpfnPlcDeviceWriteContinue;
LPFNPLCDEVICECLEAR         lpfnPlcDeviceClear;
LPFNPLCDEVICEENABLE        lpfnPlcDeviceEnable;
LPFNPLCDEVICESETCOMMSTATE  lpfnPlcDeviceSetCommState;
LPFNPLCDEVICESETCOMMBREAK  lpfnPlcDeviceSetCommBreak;
LPFNPLCDEVICECLEARCOMMBREAK lpfnPlcDeviceClearCommBreak;

LPFNPLCDEVICEGETINFO  lpfnPlcDeviceGetInfo;
LPFNPLCDEVICESETINFO  lpfnPlcDeviceSetInfo;


extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceReadContinue(FARPROC proc)
{
	lpfnPlcDeviceReadContinue = (LPFNPLCDEVICEREADCONTINUE) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceWriteContinue(FARPROC proc)
{
	lpfnPlcDeviceWriteContinue = (LPFNPLCDEVICEWRITECONTINUE) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceClear(FARPROC proc)
{
	lpfnPlcDeviceClear = (LPFNPLCDEVICECLEAR) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceEnable(FARPROC proc)
{
	lpfnPlcDeviceEnable = (LPFNPLCDEVICEENABLE) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceSetCommState(FARPROC proc)
{
	lpfnPlcDeviceSetCommState = (LPFNPLCDEVICESETCOMMSTATE) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceSetCommBreak(FARPROC proc)
{
	lpfnPlcDeviceSetCommBreak = (LPFNPLCDEVICESETCOMMBREAK) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceClearCommBreak(FARPROC proc)
{
	lpfnPlcDeviceClearCommBreak = (LPFNPLCDEVICECLEARCOMMBREAK) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceGetInfo(FARPROC proc)
{
	lpfnPlcDeviceGetInfo = (LPFNPLCDEVICEGETINFO) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPlcDeviceSetInfo(FARPROC proc)
{
	lpfnPlcDeviceSetInfo = (LPFNPLCDEVICESETINFO) proc;
}

int  PlcDeviceReadContinue(DEVICE_STRUCT *device, char *buf, int count)
{
	return lpfnPlcDeviceReadContinue(device, buf, count);
}

int  PlcDeviceWriteContinue(DEVICE_STRUCT *device, char *buf, int count)
{
	return lpfnPlcDeviceWriteContinue(device, buf, count);
}

int  PlcDeviceWrite(DEVICE_STRUCT *device, char ch)
{
	char buf[10];
	buf[0] = ch;
	return lpfnPlcDeviceWriteContinue(device, buf, 1);
}

int  PlcDeviceClear(DEVICE_STRUCT *device)
{
	return lpfnPlcDeviceClear(device);
}

int  PlcDeviceEnable(DEVICE_STRUCT *device, char flag)
{
	if(lpfnPlcDeviceEnable == NULL)	return 1;
	return lpfnPlcDeviceEnable(device, flag);
}

int  PlcDeviceSetCommState(DEVICE_STRUCT *device, DWORD baud, char parity, char data, char stop)
{
	if(lpfnPlcDeviceSetCommState)
		return lpfnPlcDeviceSetCommState(device, baud, parity, data, stop);
	else
		return 0;
}

int  PlcDeviceSetCommBreak(DEVICE_STRUCT *device)
{
	if(lpfnPlcDeviceSetCommBreak)
		return lpfnPlcDeviceSetCommBreak(device);
	else
		return 0;
}

int  PlcDeviceClearCommBreak(DEVICE_STRUCT *device)
{
	if(lpfnPlcDeviceClearCommBreak)
		return lpfnPlcDeviceClearCommBreak(device);
	else
		return 0;
}

int  PlcDeviceGetInfo(DEVICE_STRUCT *device, int item, BYTE *buf, int size)
{
	if(lpfnPlcDeviceGetInfo)
		return lpfnPlcDeviceGetInfo(device, item, buf, size);
	else
		return 0;
}

int  PlcDeviceSetInfo(DEVICE_STRUCT *device, int item, BYTE *buf, int size)
{
	if(lpfnPlcDeviceSetInfo)
		return lpfnPlcDeviceSetInfo(device, item, buf, size);
	else
		return 0;
}

typedef int  (* LPFNADDWAITWRITEDIGITALOUT) (int port, int station, WORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag);
typedef int  (* LPFNADDWAITWRITEANALOGOUT) (int port, int station, WORD address, char *sExtraAddr, WORD wExtraAddr, double value);

LPFNADDWAITWRITEDIGITALOUT lpfnAddWaitWriteDigitalOut;
LPFNADDWAITWRITEANALOGOUT  lpfnAddWaitWriteAnalogOut;

extern "C" void DLLEXPORT ProtocolSetProcAddWaitWriteDigitalOut(FARPROC proc)
{
	lpfnAddWaitWriteDigitalOut = (LPFNADDWAITWRITEDIGITALOUT) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcAddWaitWriteAnalogOut(FARPROC proc)
{
	lpfnAddWaitWriteAnalogOut = (LPFNADDWAITWRITEANALOGOUT) proc;
}

int  AddWaitWriteDigitalOut(int port, int station, WORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag)
{
	return lpfnAddWaitWriteDigitalOut(port, station, address, sExtraAddr, wExtraAddr, flag);
}

int  AddWaitWriteAnalogOut(int port, int station, WORD address, char *sExtraAddr, WORD wExtraAddr, double value)
{
	return lpfnAddWaitWriteAnalogOut(port, station, address, sExtraAddr, wExtraAddr, value);
}

typedef void  (* LPFNMESSAGEDISPLAY) (LPCSTR msg);

LPFNMESSAGEDISPLAY lpfnMessageDisplay;

extern "C" void DLLEXPORT ProtocolSetProcMessageDisplay(FARPROC proc)
{
	lpfnMessageDisplay = (LPFNMESSAGEDISPLAY) proc;
}

void MessageDisplay(LPCSTR msg)
{
	lpfnMessageDisplay(msg);
}

extern "C" WORD DLLEXPORT ProtocolGetDriverVersionMajor()
{
	return PROTOCOL_VERSION_MAJOR;
}

extern "C" WORD DLLEXPORT ProtocolGetDriverVersionMinor()
{
	

	return PROTOCOL_VERSION_MINOR;
}

WORD PeekValueWORD(LOCAL_PORT_STRUCT *pt, WORD address)
{
	if(address >= pt->nBufSizeWORD)	return 0;
	
	return pt->bufWORD[address].value;
}

float PeekValueFLOAT(LOCAL_PORT_STRUCT *pt, WORD address)
{
	if(address >= pt->nBufSizeFLOAT)	return 0;
	
	return pt->bufFLOAT[address].value;
}

DWORD PeekValueDWORD(LOCAL_PORT_STRUCT *pt, WORD address)
{
	if(address >= pt->nBufSizeDWORD)	return 0;
	
	return pt->bufDWORD[address].value;
}

int PeekValueSTRING(LOCAL_PORT_STRUCT *pt, WORD address, char *string)
{
	if(address >= pt->nBufSizeSTRING) {
		string[0] = 0;
		return 0;
	}
	
	strcpy(string, pt->bufSTRING[address].value);
	return 1;
}

double PeekValueDOUBLE(LOCAL_PORT_STRUCT *pt, WORD address)
{
	return PeekValuePortDOUBLE(pt->no, address);
}

__int64 PeekValueINT64(LOCAL_PORT_STRUCT *pt, WORD address)
{
	return PeekValuePortINT64(pt->no, address);
}

typedef int  (* LPFNPOKEVALUEPORTWORD) (int port, int address, WORD val);
typedef int  (* LPFNPOKEVALUEPORTDWORD) (int port, int address, DWORD val);
typedef int  (* LPFNPOKEVALUEPORTFLOAT) (int port, int address, double val);
typedef int  (* LPFNPOKEVALUEPORTSTRING) (int port, int address, char* val);
typedef int  (* LPFNPOKEVALUEPORTDOUBLE) (int port, int address, double val);
typedef int  (* LPFNPOKEVALUEPORTINT64) (int port, int address, __int64 val);

typedef WORD (* LPFNPEEKVALUEPORTWORD) (int port, int address);
typedef DWORD (* LPFNPEEKVALUEPORTDWORD) (int port, int address);
typedef double (* LPFNPEEKVALUEPORTFLOAT) (int port, int address);
typedef int  (* LPFNPEEKVALUEPORTSTRING) (int port, int address, char* val);
typedef double (* LPFNPEEKVALUEPORTDOUBLE) (int port, int address);
typedef _int64 (* LPFNPEEKVALUEPORTINT64) (int port, int address);


LPFNPOKEVALUEPORTWORD  lpfnProtocolPokeValuePortWORD;
LPFNPOKEVALUEPORTDWORD  lpfnProtocolPokeValuePortDWORD;
LPFNPOKEVALUEPORTFLOAT  lpfnProtocolPokeValuePortFLOAT;
LPFNPOKEVALUEPORTSTRING  lpfnProtocolPokeValuePortSTRING;
LPFNPOKEVALUEPORTDOUBLE  lpfnProtocolPokeValuePortDOUBLE;
LPFNPOKEVALUEPORTINT64  lpfnProtocolPokeValuePortINT64;

LPFNPEEKVALUEPORTWORD  lpfnProtocolPeekValuePortWORD;
LPFNPEEKVALUEPORTDWORD  lpfnProtocolPeekValuePortDWORD;
LPFNPEEKVALUEPORTFLOAT  lpfnProtocolPeekValuePortFLOAT;
LPFNPEEKVALUEPORTSTRING  lpfnProtocolPeekValuePortSTRING;
LPFNPEEKVALUEPORTDOUBLE  lpfnProtocolPeekValuePortDOUBLE;
LPFNPEEKVALUEPORTINT64  lpfnProtocolPeekValuePortINT64;

extern "C" void DLLEXPORT ProtocolSetProcPokeValuePortWORD(FARPROC proc)
{
	lpfnProtocolPokeValuePortWORD = (LPFNPOKEVALUEPORTWORD) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPokeValuePortDWORD(FARPROC proc)
{
	lpfnProtocolPokeValuePortDWORD = (LPFNPOKEVALUEPORTDWORD) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPokeValuePortFLOAT(FARPROC proc)
{
	lpfnProtocolPokeValuePortFLOAT = (LPFNPOKEVALUEPORTFLOAT) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPokeValuePortSTRING(FARPROC proc)
{
	lpfnProtocolPokeValuePortSTRING = (LPFNPOKEVALUEPORTSTRING) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPokeValuePortDOUBLE(FARPROC proc)
{
	lpfnProtocolPokeValuePortDOUBLE = (LPFNPOKEVALUEPORTDOUBLE) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPokeValuePortINT64(FARPROC proc)
{
	lpfnProtocolPokeValuePortINT64 = (LPFNPOKEVALUEPORTINT64) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPeekValuePortWORD(FARPROC proc)
{
	lpfnProtocolPeekValuePortWORD = (LPFNPEEKVALUEPORTWORD) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPeekValuePortDWORD(FARPROC proc)
{
	lpfnProtocolPeekValuePortDWORD = (LPFNPEEKVALUEPORTDWORD) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPeekValuePortFLOAT(FARPROC proc)
{
	lpfnProtocolPeekValuePortFLOAT = (LPFNPEEKVALUEPORTFLOAT) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPeekValuePortSTRING(FARPROC proc)
{
	lpfnProtocolPeekValuePortSTRING = (LPFNPEEKVALUEPORTSTRING) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPeekValuePortDOUBLE(FARPROC proc)
{
	lpfnProtocolPeekValuePortDOUBLE = (LPFNPEEKVALUEPORTDOUBLE) proc;
}

extern "C" void DLLEXPORT ProtocolSetProcPeekValuePortINT64(FARPROC proc)
{
	lpfnProtocolPeekValuePortINT64 = (LPFNPEEKVALUEPORTINT64) proc;
}

int PokeValuePortWORD(int port, int address, WORD val)
{
	if(lpfnProtocolPokeValuePortWORD)
		return lpfnProtocolPokeValuePortWORD(port, address, val);
	else
		return 0;
}

int PokeValuePortFLOAT(int port, int address, double val)
{
	if(lpfnProtocolPokeValuePortFLOAT)
		return lpfnProtocolPokeValuePortFLOAT(port, address, val);
	else
		return 0;
}

int PokeValuePortDWORD(int port, int address, DWORD val)
{
	if(lpfnProtocolPokeValuePortDWORD)
		return lpfnProtocolPokeValuePortDWORD(port, address, val);
	else
		return 0;
}

int PokeValuePortSTRING(int port, int address, char *val)
{
	if(lpfnProtocolPokeValuePortSTRING)
		return lpfnProtocolPokeValuePortSTRING(port, address, val);
	else
		return 0;
}

int PokeValuePortDOUBLE(int port, int address, double val)
{
	if(lpfnProtocolPokeValuePortDOUBLE)
		return lpfnProtocolPokeValuePortDOUBLE(port, address, val);
	else
		return 0;
}

int PokeValuePortINT64(int port, int address, __int64 val)
{
	if(lpfnProtocolPokeValuePortINT64)
		return lpfnProtocolPokeValuePortINT64(port, address, val);
	else
		return 0;
}

WORD PeekValuePortWORD(int port, int address)
{
	if(lpfnProtocolPeekValuePortWORD)
		return lpfnProtocolPeekValuePortWORD(port, address);
	else
		return 0;
}

double PeekValuePortFLOAT(int port, int address)
{
	if(lpfnProtocolPeekValuePortFLOAT)
		return lpfnProtocolPeekValuePortFLOAT(port, address);
	else
		return 0;
}

DWORD PeekValuePortDWORD(int port, int address)
{
	if(lpfnProtocolPeekValuePortDWORD)
		return lpfnProtocolPeekValuePortDWORD(port, address);
	else
		return 0;
}

int PeekValuePortSTRING(int port, int address, char *val)
{
	if(lpfnProtocolPeekValuePortSTRING)
		return lpfnProtocolPeekValuePortSTRING(port, address, val);
	else
		return 0;
}

double PeekValuePortDOUBLE(int port, int address)
{
	if(lpfnProtocolPeekValuePortDOUBLE)
		return lpfnProtocolPeekValuePortDOUBLE(port, address);
	else
		return 0;
}

__int64 PeekValuePortINT64(int port, int address)
{
	if(lpfnProtocolPeekValuePortINT64)
		return lpfnProtocolPeekValuePortINT64(port, address);
	else
		return 0;
}

void ProcProtocolGetDriverTitle(char* title);
void ProcProtocolInit(HWND hwnd, LOCAL_PORT_STRUCT *pt);
void ProcProtocolUnInit(LOCAL_PORT_STRUCT *pt);
int ProcProtocolRead(LOCAL_PORT_STRUCT* pt, int pos);

extern "C" void DLLEXPORT ProtocolGetDriverTitle(char* title)
{
	ProcProtocolGetDriverTitle(title);
}

extern "C" void DLLEXPORT ProtocolInit(HWND hwnd, LOCAL_PORT_STRUCT *pt)
{
	pt->sPlcScanErrorString = NULL;

	ProcProtocolInit(hwnd, pt);
}

extern "C" void DLLEXPORT ProtocolUnInit(LOCAL_PORT_STRUCT *pt)
{
	ProcProtocolUnInit(pt);
	
	if(pt->sPlcScanErrorString)	delete pt->sPlcScanErrorString;
	pt->sPlcScanErrorString = NULL;
}

extern "C" int DLLEXPORT ProtocolRead(LOCAL_PORT_STRUCT* pt, int pos)
{
	return ProcProtocolRead(pt, pos);
}

int ProcProtocolWriteBit(LOCAL_PORT_STRUCT *pt, int station, DWORD address, WORD flag, char *device, WORD pannel, char *sAddress);
int ProcProtocolWriteWord(LOCAL_PORT_STRUCT *pt, int station, DWORD address, long double value, char *device, WORD pannel, char *sAddress);

extern "C" int DLLEXPORT ProtocolWriteBit(LOCAL_PORT_STRUCT *pt, int station, DWORD address, WORD flag, char *device, DWORD pannel, char *sAddress)
{
	return ProcProtocolWriteBit(pt, station, address, flag, device, (WORD)pannel, sAddress);
}

extern "C" int DLLEXPORT ProtocolWriteWord(LOCAL_PORT_STRUCT *pt, int station, DWORD address, long double value, char *device, DWORD pannel, char *sAddress)
{
	return ProcProtocolWriteWord(pt, station, address, value, device, (WORD)pannel, sAddress);
}

// 8.0~8.1.9 버전의 통신 프로그램은 이 프로토콜 상자를 사용했다.
extern "C" int DLLEXPORT ProtocolConfigOption(HWND hwnd, HWND hwndEdit, int port, char *option)
{
	CString msg;
	if(IsLangKorean()) {
		msg.Format("통신 프로그램 8.0.0 ~ 8.1.9 버전에서는\n8.2.0 이상에서 개발된 통신 드라이버의 대화 상자 기능은 사용할 수 없습니다.\n프로토콜 옵션을 입력란에 수동으로 입력하시기 바랍니다.\n\n프로토콜 옵션 설정상자 외 모든 통신 기능은 정상적으로 사용할 수 있습니다.\n\n통신 프로그램 8.2.0 이상에서는 모든 기능을 사용할 수 있습니다.");  
		MessageBox(hwnd, msg, "상위 버전용 통신 드라이버", MB_OK);
	}
	else {
		msg.Format("Can't display protocol option DialogBox on this communication program.\nEdit direct to option edit box.\n\nAll function of driver is able except Option Dialog box.\n\nCommunication driver version = %d.%d", ProtocolGetDriverVersionMajor(), ProtocolGetDriverVersionMinor());  
		MessageBox(hwnd, msg, "Driver version is high", MB_OK);
	}
	return 0;
	//return ProcProtocolConfigOption(hwnd, hwndEdit, port, option);
}

int ProcProtocolConfigOption(HWND hwnd, HWND hwndEdit, int port, char *option);
// 8.2.0 이상의 통신 프로그램은 이 함수를 사용한다.
extern "C" int DLLEXPORT ProtocolConfigOption_8_2(HWND hwnd, HWND hwndEdit, int port, char *option)
{
	// NULL이면 AfxGetResourceHandle()에서 오류가 발생한다.
	// 프로토콜 DLL과 PLC_SCAN이 컴파일러 버전이 서로 다를 경우에만 NULL로 나옴
	// 프로토콜 버전 8.6에서는 이부분만 추가 됨 다른것은 변함 없음
	if(afxCurrentResourceHandle == NULL) {	
		HINSTANCE hInstance = ::GetModuleHandle(NULL);
		AfxWinInit(hInstance, NULL, ::GetCommandLine(), 0);
	}

	return ProcProtocolConfigOption(hwnd, hwndEdit, port, option);
}

void ProcProtocolDrawMethodTitle(HDC hdc, int x, int y);

extern "C" void DLLEXPORT ProtocolDrawMethodTitle(HDC hdc, int x, int y)
{
	ProcProtocolDrawMethodTitle(hdc, x, y);
}

void ProcProtocolDrawMethod(LOCAL_PORT_STRUCT *pt, HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);

extern "C" void DLLEXPORT ProtocolDrawMethod(LOCAL_PORT_STRUCT *pt, HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
{
	ProcProtocolDrawMethod(pt, hdc, x, y, sm);
}

// 문자열 출력을 미리 만들어 놓았다. 문자열 출력이 필요할 때 이 함수를 프로토콜에 위치 시키고 이것을 무효화 시킨다.
int ProcProtocolWriteBlock(LOCAL_PORT_STRUCT *pt, int station, DWORD address, BYTE *value, short byte_size, BYTE array_type, char *device, WORD pannel, char *sAddress);
/*
int ProcProtocolWriteBlock(LOCAL_PORT_STRUCT *pt, int station, DWORD address, BYTE *value, short byte_size, BYTE array_type, char *device, WORD pannel, char *sAddress)
{
	CStringA buf;
	CStringA atype;

	if(array_type == 1)			atype = "byte";
	else if(array_type == 4)	atype = "ushort";
	else if(array_type == 6)	atype = "uint";
	else if(array_type == 8)	atype = "ulong";
	else if(array_type == 9)	atype = "float";
	else if(array_type == 10)	atype = "double";
	else if(array_type == 11)	atype = "string";
	else						atype = "unknown type";

	buf.Format("This protocol is not supported BLOCK Write. (block_size=%d, block_type=%d(%s))", byte_size, array_type, atype);

	PlcScanSetErrorString(pt, buf);
	
	return COMMUNICATION_ERR_STRING;
}*/

// Protocol 8.8에서 지원
extern "C" int DLLEXPORT ProtocolWriteBlock(LOCAL_PORT_STRUCT *pt, int station, DWORD address, BYTE *value, char *device, DWORD pannel, char *sAddress, short byte_size, BYTE array_type)
{
	return ProcProtocolWriteBlock(pt, station, address, value, byte_size, array_type, device, (WORD)pannel, sAddress);
}



#include "stdafx.h"

#include <tools.h>
#include <crc.hpp>

#include "..\..\catlib.src\main_scan.h"

#include "Win32Common.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

typedef struct {
	int			nCountWORD;
	int			nCountFLOAT;
	int			nCountDWORD;
	int			nCountSTRING;
	int			nCountSYSTEM;
	int			nCountDOUBLE;
	int			nCountINT64;
	
	WORD_BUF	*buf_word;
	FLOAT_BUF	*buf_float;
	DWORD_BUF	*buf_dword;
	STRING_BUF	*buf_string;
	WORD		*buf_system;
	DOUBLE_BUF	*buf_double;
	INT64_BUF	*buf_int64;

	HANDLE		hWORD;
	HANDLE		hDWORD;
	HANDLE		hFLOAT;
	HANDLE		hSTRING;
	HANDLE		hSYSTEM;
	HANDLE		hDOUBLE;
	HANDLE		hINT64;
	BYTE		hmmfNumber;
} PLC_SCAN_MEMORY;

static PLC_SCAN_MEMORY *plcScanMemory = NULL;
static int nMaxPort = 256;

void InitReadMemoryOne(int i)
{
	char name[80];
	HANDLE hmmf;

	plcScanMemory[i].buf_word = NULL;
	plcScanMemory[i].buf_dword = NULL;
	plcScanMemory[i].buf_float = NULL;
	plcScanMemory[i].buf_string = NULL;
	plcScanMemory[i].buf_system = NULL;
	plcScanMemory[i].buf_double = NULL;
	plcScanMemory[i].buf_int64 = NULL;

	sprintf(name, "PlcScanPort%03d_Information", i);
	hmmf = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(hmmf == NULL)	return;
	else {
		PLC_SCAN_PORT_INFO *info;
		info = (PLC_SCAN_PORT_INFO*)MapViewOfFile(hmmf, FILE_MAP_READ, 0, 0, 0);
		if(GetCRC_SumWORD(info, sizeof(PLC_SCAN_PORT_INFO)-2) != info->crc) {	// crc mismatched
			//MessageDisplay("Port%d의 PLC_SCAN_PORT_INFO 의 crc가 맞지 않습니다.", i);
			return;
		}
		plcScanMemory[i].nCountWORD = info->nCountWORD;
		plcScanMemory[i].nCountFLOAT = info->nCountFLOAT;
		plcScanMemory[i].nCountDWORD = info->nCountDWORD;
		plcScanMemory[i].nCountSTRING = info->nCountSTRING;
		plcScanMemory[i].nCountSYSTEM = info->nCountSYSTEM;
		plcScanMemory[i].nCountDOUBLE = info->nCountDOUBLE;
		plcScanMemory[i].nCountINT64 = info->nCountINT64;
		plcScanMemory[i].hmmfNumber = info->hmmfNumber;
		UnmapViewOfFile(info);
		CloseHandle(hmmf);
	}

	sprintf(name, "PlcScanPort%03d_MemoryWORD%d", i, plcScanMemory[i].hmmfNumber);
	plcScanMemory[i].hWORD = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(plcScanMemory[i].hWORD != NULL) {
		plcScanMemory[i].buf_word = (WORD_BUF*)MapViewOfFile(plcScanMemory[i].hWORD, FILE_MAP_READ, 0, 0, 0);
	}

	sprintf(name, "PlcScanPort%03d_MemoryFLOAT%d", i, plcScanMemory[i].hmmfNumber);
	plcScanMemory[i].hFLOAT = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(plcScanMemory[i].hFLOAT != NULL) {
		plcScanMemory[i].buf_float = (FLOAT_BUF*)MapViewOfFile(plcScanMemory[i].hFLOAT, FILE_MAP_READ, 0, 0, 0);
	}

	sprintf(name, "PlcScanPort%03d_MemoryDWORD%d", i, plcScanMemory[i].hmmfNumber);
	plcScanMemory[i].hDWORD = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(plcScanMemory[i].hDWORD != NULL) {
		plcScanMemory[i].buf_dword = (DWORD_BUF*)MapViewOfFile(plcScanMemory[i].hDWORD, FILE_MAP_READ, 0, 0, 0);
	}

	sprintf(name, "PlcScanPort%03d_MemorySTRING%d", i, plcScanMemory[i].hmmfNumber);
	plcScanMemory[i].hSTRING = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(plcScanMemory[i].hSTRING != NULL) {
		plcScanMemory[i].buf_string = (STRING_BUF*)MapViewOfFile(plcScanMemory[i].hSTRING, FILE_MAP_READ, 0, 0, 0);
	}

	sprintf(name, "PlcScanPort%03d_MemorySYSTEM%d", i, plcScanMemory[i].hmmfNumber);
	plcScanMemory[i].hSYSTEM = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(plcScanMemory[i].hSYSTEM != NULL) {
		plcScanMemory[i].buf_system = (WORD*)MapViewOfFile(plcScanMemory[i].hSYSTEM, FILE_MAP_READ, 0, 0, 0);
	}

	sprintf(name, "PlcScanPort%03d_MemoryDOUBLE%d", i, plcScanMemory[i].hmmfNumber);
	plcScanMemory[i].hDOUBLE = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(plcScanMemory[i].hDOUBLE != NULL) {
		plcScanMemory[i].buf_double = (DOUBLE_BUF*)MapViewOfFile(plcScanMemory[i].hDOUBLE, FILE_MAP_READ, 0, 0, 0);
	}

	sprintf(name, "PlcScanPort%03d_MemoryINT64%d", i, plcScanMemory[i].hmmfNumber);
	plcScanMemory[i].hINT64 = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(plcScanMemory[i].hINT64 != NULL) {
		plcScanMemory[i].buf_int64 = (INT64_BUF*)MapViewOfFile(plcScanMemory[i].hINT64, FILE_MAP_READ, 0, 0, 0);
	}
}

static void InitReadMemory()
{
	// UnInit();
	
	int i;

	for(i = 0; i < nMaxPort; i++) {
		InitReadMemoryOne(i);
	}
}

void UnInitReadMemoryOne(int i)
{
	if(plcScanMemory[i].buf_word != NULL) {	
		UnmapViewOfFile(plcScanMemory[i].buf_word);
		plcScanMemory[i].buf_word = NULL;
		CloseHandle(plcScanMemory[i].hWORD);
	}
	if(plcScanMemory[i].buf_float != NULL) {	
		UnmapViewOfFile(plcScanMemory[i].buf_float);
		plcScanMemory[i].buf_float = NULL;
		CloseHandle(plcScanMemory[i].hFLOAT);
	}
	if(plcScanMemory[i].buf_dword != NULL) {
		UnmapViewOfFile(plcScanMemory[i].buf_dword);
		plcScanMemory[i].buf_dword = NULL;
		CloseHandle(plcScanMemory[i].hDWORD);
	}
	if(plcScanMemory[i].buf_string != NULL) {
		UnmapViewOfFile(plcScanMemory[i].buf_string);
		plcScanMemory[i].buf_string = NULL;
		CloseHandle(plcScanMemory[i].hSTRING);
	}
	if(plcScanMemory[i].buf_system != NULL) {
		UnmapViewOfFile(plcScanMemory[i].buf_system);
		plcScanMemory[i].buf_system = NULL;
		CloseHandle(plcScanMemory[i].hSYSTEM);
	}
	if(plcScanMemory[i].buf_double != NULL) {	
		UnmapViewOfFile(plcScanMemory[i].buf_double);
		plcScanMemory[i].buf_double = NULL;
		CloseHandle(plcScanMemory[i].hDOUBLE);
	}
	if(plcScanMemory[i].buf_int64 != NULL) {
		UnmapViewOfFile(plcScanMemory[i].buf_int64);
		plcScanMemory[i].buf_int64 = NULL;
		CloseHandle(plcScanMemory[i].hINT64);
	}
}

static void UnInitReadMemory()
{
	int i;

	for(i = 0; i < nMaxPort; i++) {
		UnInitReadMemoryOne(i);
	}
}

extern "C" BOOL DLLEXPORT GetWORD(int port, int address, WORD *val)
{
	if(port < 0 || port >= nMaxPort)	return FALSE;
	PLC_SCAN_MEMORY *memory = &plcScanMemory[port];
	if(address < 0 || address >= memory->nCountWORD)	return FALSE;
	*val = memory->buf_word[address].value;
	return memory->buf_word[address].flag;
}

extern "C" BOOL DLLEXPORT GetFLOAT(int port, int address, float *val)
{
	*val = 1;
	if(port < 0 || port >= nMaxPort)	return FALSE;
	PLC_SCAN_MEMORY *memory = &plcScanMemory[port];
	*val = 2;
	if(address < 0 || address >= memory->nCountFLOAT)	return FALSE;
	*val = memory->buf_float[address].value;
	return memory->buf_float[address].flag;
}

extern "C" BOOL DLLEXPORT GetDWORD(int port, int address, DWORD *val)
{
	if(port < 0 || port >= nMaxPort)	return FALSE;
	PLC_SCAN_MEMORY *memory = &plcScanMemory[port];
	if(address < 0 || address >= memory->nCountDWORD)	return FALSE;
	*val = memory->buf_dword[address].value;
	return memory->buf_dword[address].flag;
}

extern "C" BOOL DLLEXPORT GetSTRING(int port, int address, char *val)
{
	if(port < 0 || port >= nMaxPort)	return FALSE;
	PLC_SCAN_MEMORY *memory = &plcScanMemory[port];
	if(address < 0 || address >= memory->nCountSTRING)	return FALSE;
	strcpy(val, memory->buf_string[address].value);
	return memory->buf_string[address].flag;
}

extern "C" BOOL DLLEXPORT GetDOUBLE(int port, int address, double *val)
{
	if(port < 0 || port >= nMaxPort)	return FALSE;
	PLC_SCAN_MEMORY *memory = &plcScanMemory[port];
	if(address < 0 || address >= memory->nCountDOUBLE)	return FALSE;
	*val = memory->buf_double[address].value;
	return memory->buf_double[address].flag;
}

extern "C" BOOL DLLEXPORT GetINT64(int port, int address, __int64 *val)
{
	if(port < 0 || port >= nMaxPort)	return FALSE;
	PLC_SCAN_MEMORY *memory = &plcScanMemory[port];
	if(address < 0 || address >= memory->nCountINT64)	return FALSE;
	*val = memory->buf_int64[address].value;
	return memory->buf_int64[address].flag;
}

extern "C" BOOL DLLEXPORT GetSYSTEM(int port, int address, WORD *val)
{
	if(port < 0 || port >= nMaxPort)	return FALSE;
	PLC_SCAN_MEMORY *memory = &plcScanMemory[port];
	if(address < 0 || address >= memory->nCountSYSTEM)	return FALSE;
	*val = memory->buf_system[address];
	return true;
}

static SCAN_WRITE_EXCHANGE_INFO *share_Main_PlcScan;
static HANDLE hShareMainPlcScan;

void InitWriteMemory()
{
	hShareMainPlcScan = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(SCAN_WRITE_EXCHANGE_INFO), "SHARE_MAIN_PLCSCAN");
	if(hShareMainPlcScan == NULL) {
		
	}
	else {
		share_Main_PlcScan = (SCAN_WRITE_EXCHANGE_INFO*)MapViewOfFile(hShareMainPlcScan, FILE_MAP_WRITE, 0, 0, 0);
		share_Main_PlcScan->struct_size = sizeof(SCAN_WRITE_EXCHANGE_INFO);
	}
}

void UnInitWriteMemory()
{
	if(share_Main_PlcScan != NULL) {
		UnmapViewOfFile(share_Main_PlcScan);
		share_Main_PlcScan = NULL;
		CloseHandle(hShareMainPlcScan);
	}
}

extern "C" void DLLEXPORT Init(int ports)
{
	nMaxPort = ports;
	plcScanMemory = new PLC_SCAN_MEMORY[ports];

	for(int i = 0; i < ports; i++) {
		memset(&plcScanMemory[i], 0, sizeof(PLC_SCAN_MEMORY));
	}

	InitReadMemory();
	InitWriteMemory(); 
}

extern "C" void DLLEXPORT UnInit()
{
	UnInitReadMemory();
	UnInitWriteMemory();
	if(plcScanMemory != NULL) {
		delete plcScanMemory;
		plcScanMemory = NULL;
	}
}

extern "C" BOOL DLLEXPORT IsPortUsing(int port)
{
	if(port >= 0 && port < nMaxPort) 
	{ 
		if(plcScanMemory[port].buf_system == NULL) 
		{
			return FALSE;
		}
		return TRUE;
	}
	return FALSE;
}

extern "C" void DLLEXPORT AddWriteList(SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(share_Main_PlcScan == NULL)	return;

	SCAN_WRITE_EXCHANGE_INFO *exchange = share_Main_PlcScan;

	int next_pos = (exchange->ring_target+1)%MAX_SCAN_WRITE_EXCHANGE_ITEM_COUNT;

	if(next_pos == exchange->ring_current) {
		//MessageDisplay("MAIN->PLC_SCAN 출력 대기 메모리가 %d개를 넘었습니다.", MAX_SCAN_WRITE_ITEM);
		return;
		
		// 아래는 PLC_SCAN에서 꺼내가는 것을 예상해서 기다려 준다. 2000개 정도 꺼내간다. 2021-6-22 추가. PLC_SCAN과 MAIN과 통신은 최대 숫자4000으로 설정해서 사용하는 것이 안전할 듯 하다. 기다리는 것으로 하면 PLC_SCAN이 종료시 MAIN이 무한루프에 들어갈수도 있다.
		//TimeOutMiliSecClass timeout;
		//while(1)
		//{
		//	Sleep(1);
		//	next_pos = (exchange->ring_target+1)%MAX_SCAN_WRITE_EXCHANGE_ITEM_COUNT;
		//	if(next_pos != exchange->ring_current)	break;
		//	if(timeout.IsTimeOut(100))	return;	// 1로 하면 3개정도 빠질때도 있다. 10으로 해도 2개정도 빠질때도 있다. 100으로 하면 모두 되는듯 하다.
		//}
	}

	memcpy(&exchange->item[next_pos], item, sizeof(SCAN_WRITE_EXCHANGE_ITEM));
	exchange->ring_target = next_pos;
}


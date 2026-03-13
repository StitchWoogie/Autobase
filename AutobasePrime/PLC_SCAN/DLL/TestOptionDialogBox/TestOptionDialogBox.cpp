// TestOptionDialogBox.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"

#include <totaldef.h>
#include <glib.h>
#include <crc.hpp>

#include "..\..\plc_scan.h"
#include "..\dll_lib\dll_lib.h"

#include "TestOptionDialogBox.h"
#include "Dialog22.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
// TODO: If this DLL is dynamically linked against the MFC DLLs,
//		any functions exported from this DLL which call into
//		MFC must have the AFX_MANAGE_STATE macro added at the
//		very beginning of the function.
//
//		For example:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// normal function body here
//		}
//
//		It is very important that this macro appear in each
//		function, prior to any calls into MFC.  This means that
//		it must appear as the first statement within the 
//		function, even before any object variable declarations
//		as their constructors may generate calls into the MFC
//		DLL.
//
//		Please see MFC Technical Notes 33 and 58 for additional
//		details.
//


// CTestOptionDialogBoxApp

BEGIN_MESSAGE_MAP(CTestOptionDialogBoxApp, CWinApp)
END_MESSAGE_MAP()


// CTestOptionDialogBoxApp construction

CTestOptionDialogBoxApp::CTestOptionDialogBoxApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CTestOptionDialogBoxApp object

CTestOptionDialogBoxApp theApp;

// CTestOptionDialogBoxApp initialization

BOOL CTestOptionDialogBoxApp::InitInstance()
{
	hInst = AfxGetResourceHandle();

	return CWinApp::InitInstance();
}

typedef struct {
	int	nDelayAfterWrite;
} LOCAL_VARS_STRUCT;

#define localVars ((LOCAL_VARS_STRUCT*)pt->hLocalProtocol)

extern "C" void DLLEXPORT ProtocolGetDriverTitle(char *title)
{
	strcpy(title, "Test Option Dialog Box");
}

void ProcProtocolInit(HWND hwnd, LOCAL_PORT_STRUCT *pt)
{
	/*
	pt->hLocalProtocol = (HGLOBAL) new LOCAL_VARS_STRUCT;

	memset(localVars, 0, sizeof(LOCAL_VARS_STRUCT));

	CommaBlockString comma;

	comma.Set(pt->sScanProtocolOption);
	comma.GetInt(localVars->nDelayAfterWrite);

	if(localVars->nDelayAfterWrite < 0)		localVars->nDelayAfterWrite = 0;
	if(localVars->nDelayAfterWrite > 2000)	localVars->nDelayAfterWrite = 2000;

	for(int i = 0; i < 100; i++) {
		Sleep(1000);
		CString buf;
		buf.Format("%d", i);
		MessageDisplay(buf);
	}*/
}

void ProcProtocolUnInit(LOCAL_PORT_STRUCT *pt)
{
	delete localVars;
}

extern "C" void DLLEXPORT ProtocolDrawMethodTitle(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size";

	TextOut(hdc, x, y, string, strlen(string));
}

void ProcProtocolDrawMethod(LOCAL_PORT_STRUCT *pt, HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
{
	TEXTMETRIC tm;
	char buf[80];
	int cxChar;

	GetTextMetrics(hdc, &tm);
	cxChar = tm.tmAveCharWidth+tm.tmExternalLeading;
	sprintf(buf, "%3d", sm->station);
	TextOut(hdc, x, y, buf, strlen(buf));

	sprintf(buf, "%s", sm->type);
	TextOut(hdc, x+cxChar*4, y, buf, strlen(buf));

	sprintf(buf, "%3d", sm->address);
	TextOut(hdc, x+cxChar*10, y, buf, strlen(buf));

	sprintf(buf, "%3d", sm->target);
	TextOut(hdc, x+cxChar*16, y, buf, strlen(buf));

	sprintf(buf, "%3d", sm->size);         // size word of read
	TextOut(hdc, x+cxChar*20, y, buf, strlen(buf));
}

/*
// CRC를 체크하는 것은 아니고 값을 체크한다.

static int CheckCRC(BYTE *buf, int size)
{
	if(buf[0] != ACK)	return 0;
	if(buf[size-3] != ETX)	return 0;
	
	BYTE crc = HexBufToBYTE((char*)&buf[size-2]);

	if(crc != GetCRC_SUM8(buf, size-2))	return 0;

	return 1;
}

static void MakeErrorString(LOCAL_PORT_STRUCT *pt, BYTE *buf)
{
	WORD value = HexBufToWORD((char*)&buf[6]);
	char msg[160];

	//switch(value) {
	//	default:
			sprintf(msg, "Master-K Returned NAK with code (%04X)", value);
	//		break;
	//}
	PlcScanSetErrorString(pt, msg);
}

enum {
	DATA_TYPE_BIT,
	DATA_TYPE_WORD,
	DATA_TYPE_DWORD,
};

static int GetDataType(char *string, int &data_type)
{
	if(strcmp(string, "PW") == 0)		data_type = DATA_TYPE_WORD;
	else if(strcmp(string, "MW") == 0)	data_type = DATA_TYPE_WORD;
	else if(strcmp(string, "LW") == 0)	data_type = DATA_TYPE_WORD;
	else if(strcmp(string, "KW") == 0)	data_type = DATA_TYPE_WORD;
	else if(strcmp(string, "FW") == 0)	data_type = DATA_TYPE_WORD;
	else if(strcmp(string, "TW") == 0)	data_type = DATA_TYPE_WORD;
	else if(strcmp(string, "CW") == 0)	data_type = DATA_TYPE_WORD;
	else if(strcmp(string, "DW") == 0)	data_type = DATA_TYPE_WORD;
	else if(strcmp(string, "SW") == 0)	data_type = DATA_TYPE_WORD;

	else if(strcmp(string, "PX") == 0)	data_type = DATA_TYPE_BIT;
	else if(strcmp(string, "MX") == 0)	data_type = DATA_TYPE_BIT;
	else if(strcmp(string, "LX") == 0)	data_type = DATA_TYPE_BIT;
	else if(strcmp(string, "KX") == 0)	data_type = DATA_TYPE_BIT;
	else if(strcmp(string, "FX") == 0)	data_type = DATA_TYPE_BIT;
	else if(strcmp(string, "TX") == 0)	data_type = DATA_TYPE_BIT;
	else if(strcmp(string, "CX") == 0)	data_type = DATA_TYPE_BIT;

	else if(strcmp(string, "PD") == 0)	data_type = DATA_TYPE_DWORD;
	else if(strcmp(string, "MD") == 0)	data_type = DATA_TYPE_DWORD;
	else if(strcmp(string, "LD") == 0)	data_type = DATA_TYPE_DWORD;
	else if(strcmp(string, "KD") == 0)	data_type = DATA_TYPE_DWORD;
	else if(strcmp(string, "FD") == 0)	data_type = DATA_TYPE_DWORD;
	else if(strcmp(string, "TD") == 0)	data_type = DATA_TYPE_DWORD;
	else if(strcmp(string, "CD") == 0)	data_type = DATA_TYPE_DWORD;
	else if(strcmp(string, "DD") == 0)	data_type = DATA_TYPE_DWORD;
	else if(strcmp(string, "SD") == 0)	data_type = DATA_TYPE_DWORD;
	else	return 0;

	return 1;
}

static WORD RotateWORD(WORD value)
{
	WORD temp = 0;
	int  i;

	for(i = 0; i < 16; i++) {
		if(WORD_MASK[i] & value)	temp |= WORD_MASK[15-i];
	}

	return temp;
}*/

//------------------------------------------------------------------------------
// Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

extern "C" int DLLEXPORT ProtocolRead(LOCAL_PORT_STRUCT *pt, int pos)
{
	return COMMUNICATION_OK;
	/*
	BYTE crc = 0;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int      count;
	int  data_type;
	char imsi[80];
	
	if(pt->bReadingFlag == OFF) {
		if(!GetDataType(sm->type, data_type)) {
			PlcScanSetErrorString(pt, "읽기에서 MASTER-K S 에서는 없는 영역 (%s)", sm->type);
			return COMMUNICATION_ERR_STRING;
		}
		if(data_type == DATA_TYPE_BIT) {
			PlcScanSetErrorString(pt, "읽기에서 비트 영역은 읽을 수 없습니다. (%s) ?W, ?D 사용할 것", sm->type);
			return COMMUNICATION_ERR_STRING;
		}

		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = ENQ; // STX
		sprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);	// status
		sprintf((char*)&pt->commSendBuf[3], "r");					// status
		sprintf((char*)&pt->commSendBuf[4], "SB");					// mode
		sprintf(imsi, "%%%s%04d", sm->type, sm->address);			// 변수 이름 길이
		sprintf((char*)&pt->commSendBuf[6], "%02X", strlen(imsi));	// 변수 이름 길이
		sprintf((char*)&pt->commSendBuf[8], "%s", imsi);			// 변수 이름

		pt->commCountSend = 8+strlen(imsi);

		sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%02X", sm->size);		// module no
		pt->commCountSend += 2;
		pt->commSendBuf[pt->commCountSend] = EOT; // STX
		pt->commCountSend += 1;

		crc = GetCRC_SUM8(pt->commSendBuf, pt->commCountSend);

		sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%02X", crc);		// module no
		pt->commCountSend += 2;

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

		if(data_type == DATA_TYPE_WORD) {
			pt->commCountNeed = 13+sm->size*4;      // STX+COMMAND+STATION+ADDRESS+SIZE+DATA+CRC+ETX
		}
		else {	// DWORD
			pt->commCountNeed = 13+sm->size*8;      // STX+COMMAND+STATION+ADDRESS+SIZE+DATA+CRC+ETX
		}

		pt->commCountCurr = 0;

		pt->timeout->Reset();

		if(pt->commCountNeed > MAX_RECV_BUF)    return COMMUNICATION_ERR_SIZE_TOO_BIG;

		DisplaySendCodeNextLine(pt->no);

		pt->bReadingFlag = ON;

		return COMMUNICATION_WAITING;
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))               return COMMUNICATION_TIME_OUT;
		
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0)  return COMMUNICATION_WAITING;

		pt->commCountCurr += count;

        

		// 필요한 바이트 수만큼 데이타를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(!CheckCRC(pt->commRecvBuf, pt->commCountNeed))       return COMMUNICATION_CODE_BAD;

			int address = sm->target;
			

			GetDataType(sm->type, data_type);
			
			if(data_type == DATA_TYPE_DWORD) {
				DWORD value;
				for(i = 0; i < sm->size; i++) {
					value = HexBufToDWORD((char*)&pt->commRecvBuf[10+i*8]);
					//PokeValue(pt, sm, address+i, value);
				}
			}
			else {
				WORD value;
				for(i = 0; i < sm->size; i++) {
					value = HexBufToWORD((char*)&pt->commRecvBuf[10+i*4]);
					//PokeValue(pt, sm, address+i, value);
				}
			}

			return COMMUNICATION_OK;
		}
	}*/
}

//------------------------------------------------------------------------------
//      한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int ProcProtocolWriteBit(LOCAL_PORT_STRUCT *pt, int station, DWORD address, WORD flag, char *device, WORD pannel, char *sAddress)
{
	/*
	BYTE            crc = 0;
	int             count;
	TimeOutClass    timeout;
	int  data_type;
	char			imsi[80];

	if(!GetDataType(device, data_type)) {
		PlcScanSetErrorString(pt, "비트 쓰기에서 MASTER-K 에서는 없는 영역 (%s)", device);
		return COMMUNICATION_ERR_STRING;
	}
	if(data_type != DATA_TYPE_BIT) {
		PlcScanSetErrorString(pt, "MASTER-K 비트 쓰기에서 워드 영역은 출력할 수 없음 (%s)", device);
		return COMMUNICATION_ERR_STRING;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ; // STX
	sprintf((char*)&pt->commSendBuf[1], "%02X", station);	// status
	sprintf((char*)&pt->commSendBuf[3], "wSS", station);	// 직접변수 개별 쓰기
	sprintf((char*)&pt->commSendBuf[6], "%02X", 1);			// 블럭수
	sprintf(imsi, "%%%s%04X", device, address);
	sprintf((char*)&pt->commSendBuf[8], "%02X", strlen(imsi));	// 변수길이
	sprintf((char*)&pt->commSendBuf[10], "%s", imsi);		// 변수이름
	pt->commCountSend = 10+strlen(imsi);

	if(data_type == DATA_TYPE_BIT) {
		sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%02X", (BYTE)flag);
		pt->commCountSend += 2;
	}
	else if(data_type == DATA_TYPE_DWORD) {
		sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%08X", (DWORD)flag);
		pt->commCountSend += 8;
	}
	else {	// WORD
		sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%04X", (WORD)flag);		
		pt->commCountSend += 4;
	}
	pt->commSendBuf[pt->commCountSend] = EOT; // STX
	pt->commCountSend += 1;

	crc = GetCRC_SUM8(pt->commSendBuf, pt->commCountSend);
	sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%02X", crc);		// size 1
	pt->commCountSend += 2;
	
	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = 9; // 
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)    return COMMUNICATION_ERR_SIZE_TOO_BIG;

	DisplaySendCodeNextLine(pt->no);

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이타를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed+(pt->commRecvBuf[0] == NAK ? 1 : 0)) {
			if(pt->commRecvBuf[0] == NAK) {
				MakeErrorString(pt, pt->commRecvBuf);
				return COMMUNICATION_ERR_STRING;
			}
			if(!CheckCRC(pt->commRecvBuf, pt->commCountNeed))       return COMMUNICATION_CODE_BAD;

			Sleep(localVars->nDelayAfterWrite);

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))   return COMMUNICATION_TIME_OUT;
	}*/

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//      한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int ProcProtocolWriteWord(LOCAL_PORT_STRUCT *pt, int station, DWORD address, long double value, char *device, WORD pannel, char *sAddress)
{
	/*
	BYTE            crc = 0;
	int             count;
	TimeOutClass    timeout;
	int  data_type;
	char			imsi[80];

	if(!GetDataType(device, data_type)) {
		PlcScanSetErrorString(pt, "워드 쓰기에서 MASTER-K 에서는 없는 영역 (%s)", device);
		return COMMUNICATION_ERR_STRING;
	}
	if(data_type == DATA_TYPE_BIT) {
		PlcScanSetErrorString(pt, "MASTER-K 워드 쓰기에서 비트 영역은 출력할 수 없음 (%s)", device);
		return COMMUNICATION_ERR_STRING;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ; // STX
	sprintf((char*)&pt->commSendBuf[1], "%02X", station);	// status
	sprintf((char*)&pt->commSendBuf[3], "wSB", station);	// 직접변수 개별 쓰기
	
	sprintf(imsi, "%%%s%04X", device, address);
	sprintf((char*)&pt->commSendBuf[6], "%02X", strlen(imsi));	// 변수길이
	sprintf((char*)&pt->commSendBuf[8], "%s", imsi);		// 변수이름
	pt->commCountSend = 8+strlen(imsi);
	sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%02X", 1);	// 데이타 갯수
	pt->commCountSend += 2;

	if(data_type == DATA_TYPE_BIT) {
		sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%02X", (BYTE)value);
		pt->commCountSend += 2;
	}
	else if(data_type == DATA_TYPE_DWORD) {
		sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%08X", (DWORD)value);
		pt->commCountSend += 8;
	}
	else {	// WORD
		sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%04X", (WORD)value);		
		pt->commCountSend += 4;
	}
	pt->commSendBuf[pt->commCountSend] = EOT; // STX
	pt->commCountSend += 1;

	crc = GetCRC_SUM8(pt->commSendBuf, pt->commCountSend);
	sprintf((char*)&pt->commSendBuf[pt->commCountSend], "%02X", crc);		// size 1
	pt->commCountSend += 2;
	
	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = 9; // 
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)    return COMMUNICATION_ERR_SIZE_TOO_BIG;

	DisplaySendCodeNextLine(pt->no);

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이타를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed+(pt->commRecvBuf[0] == NAK ? 1 : 0)) {
			if(pt->commRecvBuf[0] == NAK) {
				MakeErrorString(pt, pt->commRecvBuf);
				return COMMUNICATION_ERR_STRING;
			}
			if(!CheckCRC(pt->commRecvBuf, pt->commCountNeed))       return COMMUNICATION_CODE_BAD;

			Sleep(localVars->nDelayAfterWrite);

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))   return COMMUNICATION_TIME_OUT;
	}*/

	char buf[20];
	sprintf(buf, "%04X", address);

	address = atoi(buf);

	address = (WORD)address;

	return COMMUNICATION_OK;
}

#include "DialogProtocolOption.h"

static HINSTANCE hInst;


// 이 클래스는 2008로 컴파일할 때 필요없다.
class ChangeResource {
	HINSTANCE hInst;
public:
	ChangeResource();
	~ChangeResource();
};

ChangeResource::ChangeResource()
{
	hInst = AfxGetResourceHandle();
	AfxSetResourceHandle(theApp.m_hInstance);
}

ChangeResource::~ChangeResource()
{
	AfxSetResourceHandle(hInst);
}

int ProcProtocolConfigOption(HWND hwnd, HWND hwndEdit, int port, char *option)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());	// 이것을 쓰니 잘된다. 2018-9-12
	//ChangeResource cr;

	CDialog22 dialog;
	CommaBlockString comma;

	comma.Set(option);
//	comma.GetInt(dialog.m_DelayAfterWrite);

	int retn = dialog.DoModal();
	if(retn == IDOK) {
		HWND hwndItemThread = GetDlgItem(hwnd, 1017);
		CheckDlgButton(hwnd, 1017, 0);
		//sprintf(option, "%d,", 
		//				dialog.m_DelayAfterWrite);
	}

	return retn;
}

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
}





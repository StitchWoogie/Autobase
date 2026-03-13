// TestAutobase48.cpp : DLL의 초기화 루틴을 정의합니다.
//

#include "../dll_lib/dll_lib.h"

#include "pch.h"
#include "framework.h"
#include "TestAutobase48.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: 이 DLL이 MFC DLL에 대해 동적으로 링크되어 있는 경우
//		MFC로 호출되는 이 DLL에서 내보내지는 모든 함수의
//		시작 부분에 AFX_MANAGE_STATE 매크로가
//		들어 있어야 합니다.
//
//		예:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// 일반적인 함수 본문은 여기에 옵니다.
//		}
//
//		이 매크로는 MFC로 호출하기 전에
//		각 함수에 반드시 들어 있어야 합니다.
//		즉, 매크로는 함수의 첫 번째 문이어야 하며
//		개체 변수의 생성자가 MFC DLL로
//		호출할 수 있으므로 개체 변수가 선언되기 전에
//		나와야 합니다.
//
//		자세한 내용은
//		MFC Technical Note 33 및 58을 참조하십시오.
//

// CTestAutobase48App

BEGIN_MESSAGE_MAP(CTestAutobase48App, CWinApp)
END_MESSAGE_MAP()


// CTestAutobase48App 생성

CTestAutobase48App::CTestAutobase48App()
{
	// TODO: 여기에 생성 코드를 추가합니다.
	// InitInstance에 모든 중요한 초기화 작업을 배치합니다.
}


// 유일한 CTestAutobase48App 개체입니다.

CTestAutobase48App theApp;


// CTestAutobase48App 초기화

BOOL CTestAutobase48App::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

typedef struct {
	int	nDelayAfterWrite;
} LOCAL_VARS_STRUCT;

#define localVars ((LOCAL_VARS_STRUCT*)pt->hLocalProtocol)

void ProcProtocolGetDriverTitle(char* title)
{
	strcpy(title, "Test Autobase48");
}

void ProcProtocolInit(HWND hwnd, LOCAL_PORT_STRUCT* pt)
{
	
}

void ProcProtocolUnInit(LOCAL_PORT_STRUCT* pt)
{
	delete localVars;
}

void ProcProtocolDrawMethodTitle(HDC hdc, int x, int y)
{
	char* string = "station, type, address, buf address, read size (Test Autobase48)";

	TextOut(hdc, x, y, string, strlen(string));
}

void ProcProtocolDrawMethod(LOCAL_PORT_STRUCT* pt, HDC hdc, int x, int y, SCAN_METHOD_STRUCT* sm)
{
	TEXTMETRIC tm;
	char buf[80];
	int cxChar;

	GetTextMetrics(hdc, &tm);
	cxChar = tm.tmAveCharWidth + tm.tmExternalLeading;
	sprintf(buf, "%3d", sm->station);
	TextOut(hdc, x, y, buf, strlen(buf));

	sprintf(buf, "%s", sm->type);
	TextOut(hdc, x + cxChar * 4, y, buf, strlen(buf));

	sprintf(buf, "%3d", sm->address);
	TextOut(hdc, x + cxChar * 10, y, buf, strlen(buf));

	sprintf(buf, "%3d", sm->target);
	TextOut(hdc, x + cxChar * 16, y, buf, strlen(buf));

	sprintf(buf, "%3d", sm->size);         // size word of read
	TextOut(hdc, x + cxChar * 20, y, buf, strlen(buf));
}

//------------------------------------------------------------------------------
// Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int ProcProtocolRead(LOCAL_PORT_STRUCT* pt, int pos)
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

int ProcProtocolWriteBit(LOCAL_PORT_STRUCT* pt, int station, DWORD address, WORD flag, char* device, WORD pannel, char* sAddress)
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

int ProcProtocolWriteWord(LOCAL_PORT_STRUCT* pt, int station, DWORD address, long double value, char* device, WORD pannel, char* sAddress)
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

int ProcProtocolConfigOption(HWND hwnd, HWND hwndEdit, int port, char* option)
{
	//AFX_MANAGE_STATE(AfxGetStaticModuleState());	// 이것을 쓰니 잘된다. 2018-9-12
	////ChangeResource cr;

	//CDialog22 dialog;
	//CommaBlockString comma;

	//comma.Set(option);
	////	comma.GetInt(dialog.m_DelayAfterWrite);

	//int retn = dialog.DoModal();
	//if (retn == IDOK) {
	//	HWND hwndItemThread = GetDlgItem(hwnd, 1017);
	//	CheckDlgButton(hwnd, 1017, 0);
	//	//sprintf(option, "%d,", 
	//	//				dialog.m_DelayAfterWrite);
	//}

	//return retn;
	return 0;
}

int ProcProtocolWriteBlock(LOCAL_PORT_STRUCT* pt, int station, DWORD address, BYTE* value, short byte_size, BYTE array_type, char* device, WORD pannel, char* sAddress)
{
	CStringA buf;
	CStringA atype;

	if (array_type == 1)			atype = "byte";
	else if (array_type == 4)	atype = "ushort";
	else if (array_type == 6)	atype = "uint";
	else if (array_type == 8)	atype = "ulong";
	else if (array_type == 9)	atype = "float";
	else if (array_type == 10)	atype = "double";
	else if (array_type == 11)	atype = "string";
	else						atype = "unknown type";

	buf.Format("This protocol is not supported BLOCK Write. (block_size=%d, block_type=%d(%s))", byte_size, array_type, (const char*)atype);

	PlcScanSetErrorString(pt, buf);

	return COMMUNICATION_ERR_STRING;
}


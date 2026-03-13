// Test2.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "Test2.h"

#include <tools.h>
#include "..\..\dll_lib\dll_lib.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: If this DLL is dynamically linked against the MFC DLLs,
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

// CTest2App

BEGIN_MESSAGE_MAP(CTest2App, CWinApp)
END_MESSAGE_MAP()


// CTest2App construction

CTest2App::CTest2App()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CTest2App object

CTest2App theApp;


// CTest2App initialization

BOOL CTest2App::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

typedef struct {
	int	nDelayAfterWrite;
} LOCAL_VARS_STRUCT;

#define localVars ((LOCAL_VARS_STRUCT*)pt->hLocalProtocol)

extern "C" void DLLEXPORT ProtocolGetDriverTitle(char *title)
{
	strcpy(title, "Test2");
}

void ProcProtocolInit(HWND hwnd, LOCAL_PORT_STRUCT *pt)
{
	/* 같은 스레드인지 검사
	DWORD dwprocessid;
	DWORD id_main = GetWindowThreadProcessId(hwnd, &dwprocessid);
	DWORD id_current = GetCurrentThreadId();

	if(id_main != id_current) {

	}*/

	pt->hLocalProtocol = (HGLOBAL) new LOCAL_VARS_STRUCT;

	memset(localVars, 0, sizeof(LOCAL_VARS_STRUCT));

	CommaBlockString comma;

	comma.Set(pt->sScanProtocolOption);
	comma.GetInt(localVars->nDelayAfterWrite);

	if(localVars->nDelayAfterWrite < 0)		localVars->nDelayAfterWrite = 0;
	if(localVars->nDelayAfterWrite > 2000)	localVars->nDelayAfterWrite = 2000;
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

//------------------------------------------------------------------------------
// Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

extern "C" int DLLEXPORT ProtocolRead(LOCAL_PORT_STRUCT *pt, int pos)
{
	BYTE crc = 0;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	static double d = 1;

	d+=1234.5678901234567890;

	PokeValuePortDOUBLE(pt->no, 0, d); 
	PokeValuePortINT64(pt->no, 0, (__int64)d); 

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//      한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int ProcProtocolWriteBit(LOCAL_PORT_STRUCT *pt, int station, DWORD address, WORD flag, char *device, WORD pannel, char *sAddress)
{
	BYTE buf[100];

	int retn = PlcDeviceGetInfo(&pt->device, 2, buf, 100);
	if(retn == 9) {
		SYSTEMTIME t;
		t.wYear = buf[0]+buf[1]*256;
		t.wMonth = buf[2];
		t.wDay = buf[3];
		t.wHour = buf[4];
		t.wMinute = buf[5];
		t.wSecond = buf[6];
		t.wMilliseconds = buf[7]+buf[8]*256;
	}

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//      한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int ProcProtocolWriteWord(LOCAL_PORT_STRUCT *pt, int station, DWORD address, long double value, char *device, WORD pannel, char *sAddress)
{
	return COMMUNICATION_OK;
}

static HINSTANCE hInst;

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
	CheckDlgButton(hwnd, 1017, 0);
	return 0;
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



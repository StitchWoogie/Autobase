// english O.K
#include "stdafx.h"

#include <ddeml.h>
#include <math.h>
#include <stdlib.h>
#include <dos.h>

#include <compiler.hpp>
#include <tools.h>
#include <ddelib.hpp>
#include <dataswap.h>

static char  bFlagDDE = OFF;
static DWORD idInst = 0;

typedef struct {
	TCHAR  name[80];
	char  bRegister;			// Server가 등록 되어 있는가 ?
	HSZ   hszService;
	Block *blockTopic;
} CELL_DDE_SERVICE;

Block blockDdeService(sizeof(CELL_DDE_SERVICE));

// 모든 DDE client들이 하나의 idInst를 사용하므로 같은 이름의 Topic은 같은 HSZ를 사용한다..
typedef struct {
	TCHAR  name[80];
	HSZ	hszTopic;
} HSZ_TOPIC_STRUCT;

Block blockHszTopic(sizeof(HSZ_TOPIC_STRUCT));

// 모든 DDE client들이 하나의 idInst를 사용하므로 같은 이름의 item은 같은 item을 사용한다..
typedef struct {
	TCHAR  name[80];
	HSZ	hszItem;
} HSZ_ITEM_STRUCT;
Block blockHszItem(sizeof(HSZ_ITEM_STRUCT));

typedef struct {
	TCHAR  name[80];
	HSZ   hszTopic;
	HCONV hConv;		// service와 topic을 가지고 대화 한다.
	Block *blockItem;
} CELL_DDE_TOPIC;

typedef struct {
	TCHAR  name[80];
	HSZ	  hszItem;
	char  data[256];
	char  advise_flag;		// advise모드이면 ON
	char  bChanged;			// Advice에 의해서 값이 바뀌었는지의 여부 C#을 직접 CallBack하면 얼마후 다운되는 문제점때문에 CallBack을 없애고 값을 읽어가도록 변경
} CELL_DDE_ITEM;

static long FAR PASCAL EXPORT WndProcDebugDdeTag(HWND, UINT, WPARAM, LPARAM);
static TCHAR   szDebugDdeTagClass [] = _T("DdeLibDebugWindow") ;

static HWND hwndDebugDdeTag = NULL;
static HWND hwndListService = NULL;
static HWND hwndListTopic   = NULL;
static HWND hwndListItem    = NULL;

/*
//------------------------------------------------------------------------------
//	Debug Dde Tag 윈도우를 등록한다.
//------------------------------------------------------------------------------

void DdeLibDebugWindowRegister(HINSTANCE hInstance)
{
	WNDCLASS wndclass ;

	// Register the Message window class
	wndclass.style         = CS_HREDRAW | CS_VREDRAW ;
	wndclass.lpfnWndProc   = WndProcDebugDdeTag;
	wndclass.cbClsExtra    = 0 ;
	wndclass.cbWndExtra    = 0 ;
	wndclass.hInstance     = hInstance ;
	wndclass.hIcon         = LoadIcon (NULL, IDI_APPLICATION) ;
	wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW) ;
	wndclass.hbrBackground = (HBRUSH)GetStockObject(NULL_BRUSH);
	wndclass.lpszMenuName  = NULL;
	wndclass.lpszClassName = szDebugDdeTagClass ;

	RegisterClass (&wndclass) ;
}
*/

void DdeLibDebugWindowShow(HWND hwndFrame, HINSTANCE hinst)
{
	if(hwndDebugDdeTag) {
		//UpdateAlarmListBox();
		//SendMessage(hwndAlarmConfirmation, WM_COMMAND, IDM
		return;
	}

	HWND hwnd;

	hwnd = CreateWindow(szDebugDdeTagClass,
							  _T("Debug DDE Service/Topic/Item List"),
							  WS_OVERLAPPEDWINDOW,
							  0, 0,
							  300, 200,
							  hwndFrame, NULL, hinst, NULL);

	ShowWindow(  hwnd, SW_SHOW);
	UpdateWindow(hwnd);
}

static void AddListItem(CELL_DDE_ITEM *blockI, DWORD service, DWORD topic)
{
	if(hwndListItem == NULL)	return;

	LRESULT retn;
	retn  = SendMessage(hwndListService, LB_GETCURSEL, 0, 0);
	if(retn != (LRESULT)service)			return;
	retn  = SendMessage(hwndListTopic, LB_GETCURSEL, 0, 0);
	if(retn != (LRESULT)topic)				return;

	CString buf;

	buf.Format(_T("%s, data:%s"), blockI->name, blockI->data);
	SendMessage(hwndListItem, LB_ADDSTRING, 0, (LPARAM)(LPCTSTR)buf);
}

static void ChangeListItem(CELL_DDE_ITEM *blockI, DWORD service, DWORD topic, DWORD item)
{
	if(hwndListItem == NULL)	return;

	LRESULT retn;
	retn  = SendMessage(hwndListService, LB_GETCURSEL, 0, 0);
	if(retn != (LRESULT)service)			return;
	retn  = SendMessage(hwndListTopic,   LB_GETCURSEL, 0, 0);
	if(retn != (LRESULT)topic)				return;

	retn  = SendMessage(hwndListItem,   LB_DELETESTRING, (WPARAM)item, 0);
	if(retn == LB_ERR)				return;

	CString buf;
	buf.Format(_T("%s, data:%s"), blockI->name, blockI->data);
	SendMessage(hwndListItem, LB_INSERTSTRING, (WPARAM)item, (LPARAM)(LPCTSTR)buf);
}

static void FillListItem()
{
	LRESULT service;
	LRESULT topic;

	SendMessage(hwndListItem, LB_RESETCONTENT, 0, 0L);
	service = SendMessage(hwndListService, LB_GETCURSEL, 0, 0L);
	topic   = SendMessage(hwndListTopic, LB_GETCURSEL, 0, 0L);

	if(service == LB_ERR)	return;
	if(topic   == LB_ERR)	return;

	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	CELL_DDE_ITEM    blockI;
	DWORD i;

	blockDdeService.GetBlock((BYTE*)&blockS, (DWORD)service);
	blockS.blockTopic->GetBlock((BYTE*)&blockT, (DWORD)topic);

	for(i = 0; i < blockT.blockItem->GetBlockCount(); i++) {
		blockT.blockItem->GetBlock((BYTE*)&blockI, i);

		AddListItem(&blockI, (DWORD)service, (DWORD)topic);
	}
}

static void AddListTopic(CELL_DDE_TOPIC *blockT, DWORD service)
{
	if(hwndListTopic == NULL)	return;

	LRESULT retn;
	retn  = SendMessage(hwndListService, LB_GETCURSEL, 0, 0);
	if(retn != (LRESULT)service)			return;

	TCHAR buf[160];

	_stprintf(buf, _T("%s, HSZ:%lu, HCONV:%08lX"), blockT->name, (DWORD)blockT->hszTopic, (DWORD)blockT->hConv);
	SendMessage(hwndListTopic, LB_ADDSTRING, 0, (LPARAM)buf);
}

static void ChangeListTopic(CELL_DDE_TOPIC *blockT, DWORD service, DWORD topic)
{
	if(hwndListTopic == NULL)	return;

	LRESULT retn;

	retn  = SendMessage(hwndListService, LB_GETCURSEL, 0, 0);
	if(retn != (LRESULT)service)			return;

	retn  = SendMessage(hwndListTopic,   LB_DELETESTRING, (WPARAM)topic, 0);
	if(retn == LB_ERR)				return;

	TCHAR buf[160];
	_stprintf(buf, _T("%s, HSZ:%lu, HCONV:%08lX"), blockT->name, (DWORD)blockT->hszTopic, (DWORD)blockT->hConv);
	SendMessage(hwndListTopic, LB_INSERTSTRING, (WPARAM)topic, (LPARAM)buf);

   SendMessage(hwndListTopic, LB_SETCURSEL, (WPARAM)topic, 0L);
}

static void FillListTopic()
{
	LRESULT service;

	SendMessage(hwndListTopic, LB_RESETCONTENT, 0, 0L);
	service = SendMessage(hwndListService, LB_GETCURSEL, 0, 0L);

	if(service == LB_ERR)	return;

	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	DWORD t;

	blockDdeService.GetBlock((BYTE*)&blockS, (DWORD)service);

	for(t = 0; t < blockS.blockTopic->GetBlockCount(); t++) {
		blockS.blockTopic->GetBlock((BYTE*)&blockT, t);

		AddListTopic(&blockT, (DWORD)service);
	}

	SendMessage(hwndListTopic, LB_SETCURSEL, 0, 0L);

	FillListItem();
}

static void AddListService(CELL_DDE_SERVICE *blockS)
{
	if(hwndListService == NULL)	return;

	TCHAR buf[160];

	_stprintf(buf, _T("%s, Register:%d, HSZ:%lu"), blockS->name, blockS->bRegister, blockS->hszService);
	SendMessage(hwndListService, LB_ADDSTRING, 0, (LPARAM)buf);
}

static void ChangeListService(CELL_DDE_SERVICE *blockS, DWORD service)
{
	if(hwndListService == NULL)	return;

	LRESULT retn;

	retn  = SendMessage(hwndListService,   LB_DELETESTRING, (WPARAM)service, 0);
	if(retn == LB_ERR)				return;

	TCHAR buf[160];
	_stprintf(buf, _T("%s, Register:%d, HSZ:%lu"), blockS->name, blockS->bRegister, blockS->hszService);
	SendMessage(hwndListService, LB_INSERTSTRING, (WPARAM)service, (LPARAM)buf);

	SendMessage(hwndListService, LB_SETCURSEL, (WPARAM)service, 0L);
}

static void WmCreate(HWND hwnd, LPARAM lParam)
{
	LPCREATESTRUCT create = (LPCREATESTRUCT) lParam;

	hwndDebugDdeTag = hwnd;

	hwndListService = CreateWindow (_T("listbox"), _T(""),
										WS_CHILD | WS_VISIBLE | WS_VSCROLL | WS_BORDER | LBS_USETABSTOPS | LBS_NOINTEGRALHEIGHT | LBS_NOTIFY,
										0, 0, 0, 0,
										hwnd, (HMENU)1, create->hInstance, NULL);
	hwndListTopic   = CreateWindow (_T("listbox"), _T(""),
										WS_CHILD | WS_VISIBLE | WS_VSCROLL | WS_BORDER | LBS_USETABSTOPS | LBS_NOINTEGRALHEIGHT | LBS_NOTIFY,
										0, 0, 0, 0,
										hwnd, (HMENU)2, create->hInstance, NULL);
	hwndListItem    = CreateWindow (_T("listbox"), _T(""),
										WS_CHILD | WS_VISIBLE | WS_VSCROLL | WS_BORDER | LBS_USETABSTOPS | LBS_NOINTEGRALHEIGHT,
										0, 0, 0, 0,
										hwnd, (HMENU)3, create->hInstance, NULL);

	CELL_DDE_SERVICE blockS;
	DWORD s;

	for(s = 0; s < blockDdeService.GetBlockCount(); s++) {
		blockDdeService.GetBlock((BYTE*)&blockS, s);

		AddListService(&blockS);
	}

	SendMessage(hwndListService, LB_SETCURSEL, 0, 0L);

	FillListTopic();
}

static void WmPaint(HWND hwnd)
{
	PAINTSTRUCT ps;
	RECT rect;
	//HDC hdc;

	GetClientRect(hwnd, &rect);
	BeginPaint(hwnd, &ps);
	EndPaint(hwnd, &ps);
}

static void WmSize(HWND hwnd)
{
	RECT rect;

	GetClientRect(hwnd, &rect);

	MoveWindow(hwndListService, 0, 0,  rect.right, 50, TRUE);
	MoveWindow(hwndListTopic  , 0, 50, rect.right, 50, TRUE);
	MoveWindow(hwndListItem   , 0,100, rect.right, rect.bottom-100, TRUE);
}

/*
LRESULT FAR PASCAL EXPORT WndProcDebugDdeTag(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch(message)
	{
		case WM_CREATE:
			WmCreate(hwnd, lParam);
			return 0;
		case WM_PAINT:
			WmPaint(hwnd);
			return 0;
		case WM_SIZE:
			WmSize(hwnd);
			return 0;
		case WM_DESTROY :
			hwndDebugDdeTag = NULL;
			hwndListService = NULL;
			hwndListTopic = NULL;
			hwndListItem = NULL;
			return 0 ;
		case WM_COMMAND:
			if(HIWORD(wParam) == LBN_SELCHANGE) {
				switch(LOWORD(wParam)) {
					case 1:	// service
						FillListTopic();
						break;
					case 2:
						FillListItem();
						break;
				}
			}
			return (LRESULT)0;
	}
	return DefWindowProc( hwnd, message, wParam, lParam );
}
*/

static HSZ AddOneTopic(TCHAR *topic)
{
	DWORD l;
	HSZ_TOPIC_STRUCT hsz;

	for(l = 0; l < blockHszTopic.GetBlockCount(); l++) {
		blockHszTopic.GetBlock((BYTE*)&hsz, l);
		if(_tcscmp(topic, hsz.name) == 0)	return hsz.hszTopic;
	}

	_tcscpy(hsz.name, topic);
	hsz.hszTopic = DdeCreateStringHandle(idInst, topic, CP_WINANSI);
	if(hsz.hszTopic == 0)	return 0;
	blockHszTopic.AddBlock((BYTE*)&hsz);

	return hsz.hszTopic;
}

static HSZ AddOneItem(TCHAR *item)
{
	DWORD l;
	HSZ_ITEM_STRUCT hsz;

	for(l = 0; l < blockHszItem.GetBlockCount(); l++) {
		blockHszItem.GetBlock((BYTE*)&hsz, l);
		if(_tcscmp(item, hsz.name) == 0)	return hsz.hszItem;
	}

	_tcscpy(hsz.name, item);
	hsz.hszItem = DdeCreateStringHandle(idInst, item, CP_WINANSI);
	if(hsz.hszItem == 0)	return 0;
	blockHszItem.AddBlock((BYTE*)&hsz);

	return hsz.hszItem;
}

//--------------------------------------------------------------
//	Dde Server 쪽에서 data값을 보내왔을 때 일치하는 item이 있으면
// 그 항목의 topic과 item의 실제 스트링을 복사하여 보내준다.
//--------------------------------------------------------------

static int SeekTopicAndItemName(HSZ hsz1, HSZ hsz2, HDDEDATA hData, DWORD &s, DWORD &t, DWORD &i)
{
	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC	  blockT;
	CELL_DDE_ITEM	  blockI;

	for(s = 0; s < blockDdeService.GetBlockCount(); s++) {
		blockDdeService.GetBlock((BYTE*)&blockS, s);

		if(blockS.blockTopic != NULL) {
			for(t = 0; t < blockS.blockTopic->GetBlockCount(); t++) {
				blockS.blockTopic->GetBlock((BYTE*)&blockT, t);
				if(blockT.hszTopic == hsz1) {	// topic corrected
					if(blockT.blockItem != NULL) {
						for(i = 0; i < blockT.blockItem->GetBlockCount(); i++) {
							blockT.blockItem->GetBlock((BYTE*)&blockI, i);

							if(blockI.hszItem == hsz2) {	// yes item corrected
								char data[256];
								DdeGetData(hData, (BYTE*)data, 256, 0);
								strcpy(blockI.data, data);
								blockI.bChanged = 1;
								blockT.blockItem->SetBlock((BYTE*)&blockI, i);

								ChangeListItem(&blockI, s, t, i);
								return 1;
							}
						}
					}
				}// if topic corrected
			}	 // topic seek loop
		}	    // if topic block prepared
	}
	return 0;
}

static void XtypRegister(HSZ hsz1, HSZ /*hsz2*/)
{
	CELL_DDE_SERVICE blockS;
//	CELL_DDE_TOPIC   blockT;
	DWORD s;

//	bell(100);

	for(s = 0; s < blockDdeService.GetBlockCount(); s++) {
		blockDdeService.GetBlock((BYTE*)&blockS, s);

		if(DdeCmpStringHandles(blockS.hszService, hsz1) != 0)	continue;

		blockS.bRegister = ON;
		blockDdeService.SetBlock((BYTE*)&blockS, s);
		ChangeListService(&blockS, s);
		return;
	}
}

static void XtypUnRegister(HSZ hsz1, HSZ /*hsz2*/)
{
	CELL_DDE_SERVICE blockS;
	DWORD s;

//	bell(100);

	for(s = 0; s < blockDdeService.GetBlockCount(); s++) {
		blockDdeService.GetBlock((BYTE*)&blockS, s);

		if(DdeCmpStringHandles(blockS.hszService, hsz1) == 0) {
			blockS.bRegister = OFF;
			blockDdeService.SetBlock((BYTE*)&blockS, s);
			ChangeListService(&blockS, s);
			break;
		}
	}
}

static void XtypDisconnect(HCONV hconv)
{
	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	DWORD s, t;

	for(s = 0; s < blockDdeService.GetBlockCount(); s++) {
		blockDdeService.GetBlock((BYTE*)&blockS, s);

		for(t = 0; t < blockS.blockTopic->GetBlockCount(); t++) {
			blockS.blockTopic->GetBlock((BYTE*)&blockT, t);

			if(blockT.hConv == hconv) {
				blockT.hConv = NULL;
				blockS.blockTopic->SetBlock((BYTE*)&blockT, t);
				ChangeListTopic(&blockT, s, t);
				return;
			}
		}
	}
}



/*
LPFNCALLBACK procCallBackItemChanged;

void DdeLibSetCallBack(LPFNCALLBACK proc)
{
	procCallBackItemChanged = proc;
}
*/

static void DdeDataReceived(HSZ hTopic, HSZ hItem, HDDEDATA hData)
{
	DWORD pos_s;
	DWORD pos_t;
	DWORD pos_i;	
	
	if(SeekTopicAndItemName(hTopic, hItem, hData, pos_s, pos_t, pos_i)) {
		//char data[256];
		//DdeGetData(hData, (BYTE*)data, sizeof(data), 0);

		//procCallBackItemChanged(pos_s, pos_t, pos_i, data);
	}
}

HDDEDATA EXPENTRY EXPORT DdeCallbackClient ( WORD wType, WORD /*wFmt*/, HCONV hconv, HSZ hsz1,
		  HSZ hsz2, HDDEDATA hData, DWORD /*dwData1*/,
		  DWORD /*dwData2*/ )
{
	switch (wType) {
		// server에서 NameService를 실행할때 발생 hsz1 = service name
		case XTYP_REGISTER:
			XtypRegister(hsz1, hsz2);
			return ( (HDDEDATA) NULL);		// return value no need
		case XTYP_UNREGISTER:
			XtypUnRegister(hsz1, hsz2);
			return ( (HDDEDATA) NULL);		// return value no need
		case XTYP_DISCONNECT:
			XtypDisconnect(hconv);
			// hConvWrite = (HCONV)NULL;
			// The Server forced a disconnect.
			return ( (HDDEDATA) NULL);
		case XTYP_ADVDATA:
//		case XTYP_XACT_COMPLETE:
			{
				static char flag = OFF;

				if(flag)	return (HDDEDATA) DDE_FBUSY;
				flag = ON;

				DdeDataReceived(hsz1, hsz2, hData);

				//DdeFreeDataHandle(hData);

				flag = OFF;

				return ( (HDDEDATA) DDE_FACK );
			}
		default:
			break;
	}

	return ( (HDDEDATA) NULL );
}

//---------------------------------------------------------------------------------------------
//	프로그램 실행 시 한번만 불러준다.
//---------------------------------------------------------------------------------------------

void DdeLibClientInit(HWND hwnd)
{
	if(bFlagDDE == ON)	return;		// 이미 설치되었다.

	if(DdeInitialize ( (LPDWORD)&idInst, (PFNCALLBACK)DdeCallbackClient, APPCMD_CLIENTONLY, 0L) != DMLERR_NO_ERROR) {
		MessageBox(hwnd, _T("DdeInitialize() Error"), _T("DdeLibClient Library"), MB_OK);
		return;
	}

	bFlagDDE = ON;
}

//---------------------------------------------------------------------------------------------
//	프로그램 종료시 한번만 불러준다.
//---------------------------------------------------------------------------------------------

void DdeLibClientUninit()
{
	if(bFlagDDE == OFF)	return;

	DWORD s, t;
	DWORD l;
	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	HSZ_TOPIC_STRUCT	hszTopic;
	HSZ_ITEM_STRUCT	hszItem;

	// 모든 할당된 topic HSZ을 풀어준다.
	for(l = 0; l < blockHszTopic.GetBlockCount(); l++) {
		blockHszTopic.GetBlock((BYTE*)&hszTopic, l);
		if(hszTopic.hszTopic != 0) {
			DdeFreeStringHandle(idInst, hszTopic.hszTopic);
		}
	}

	// 모든 할당된 item HSZ을 풀어준다.
	for(l = 0; l < blockHszItem.GetBlockCount(); l++) {
		blockHszItem.GetBlock((BYTE*)&hszItem, l);
		if(hszItem.hszItem != 0) {
			DdeFreeStringHandle(idInst, hszItem.hszItem);
		}
	}

	for(s = 0; s < blockDdeService.GetBlockCount(); s++) {
		blockDdeService.GetBlock((BYTE*)&blockS, s);

		if(blockS.hszService != 0) {
			DdeFreeStringHandle(idInst, blockS.hszService);
		}

		if(blockS.blockTopic != NULL) {
			for(t = 0; t < blockS.blockTopic->GetBlockCount(); t++) {
				blockS.blockTopic->GetBlock((BYTE*)&blockT, t);
				if(blockT.hszTopic != 0) {
					// 실제 할당된 HSZ는 외부에 있으므로 hsz을 풀지 않도록 한다.
					// DdeFreeStringHandle(idInst, blockT.hszTopic);
					blockT.hszTopic = 0;
				}
				if(blockT.hConv) {
					DdeDisconnect(blockT.hConv);
				}

				if(blockT.blockItem)
					delete blockT.blockItem;
			}
			delete blockS.blockTopic;
		}
	}

	DdeUninitialize(idInst);

	bFlagDDE = OFF;
}

static int IsSameServiceExist(TCHAR *service, DWORD &pos)
{
	DWORD l;
	CELL_DDE_SERVICE blockS;

	for(l = 0; l < blockDdeService.GetBlockCount(); l++) {
		blockDdeService.GetBlock((BYTE*)&blockS, l);
		if(_tcsicmp(blockS.name, service) == 0)	{
			pos = l;
			return 1;
		}
	}

	return 0;
}

static int IsSameTopicExist(Block *blockTopic, TCHAR *topic, DWORD &pos)
{
	DWORD l;
	CELL_DDE_TOPIC   blockT;

	for(l = 0; l < blockTopic->GetBlockCount(); l++) {
		blockTopic->GetBlock((BYTE*)&blockT, l);
		if(_tcscmp(blockT.name, topic) == 0)	{
			pos = l;
			return 1;
		}
	}

	return 0;
}

static int IsSameItemExist(Block *blockItem, TCHAR *item, DWORD &pos)
{
	DWORD l;
	CELL_DDE_ITEM   blockI;

	for(l = 0; l < blockItem->GetBlockCount(); l++) {
		blockItem->GetBlock((BYTE*)&blockI, l);
		if(_tcscmp(blockI.name, item) == 0)	{
			pos = l;
			return 1;
		}
	}

	return 0;
}

/*
//----------------------------------------------------
//	이미 등록된 item hsz이 있으면 위치를 알려주고
// 그렇지 않을때는 hsz을 만든다음 위치를 알려준다.
//----------------------------------------------------

DWORD InsertOneItemHSZ(Block *block, char *item, HSZ &hszItem)
{
	CELL_DDE_HSZ hsz;
	DWORD l;

	for(l = 0; l < block->GetBlockCount(); l++) {
		block->GetBlock((BYTE*)&hsz, l);
		if(strcmp(hsz.name, item) == 0) {
			hszItem = hsz.hszItem;
			return l;
		}
	}

	memset(&hsz, 0, sizeof(CELL_DDE_HSZ));
	strcpy(hsz.name, item);
	hsz.hszItem = DdeCreateStringHandle(idInst, item, CP_WINANSI);

	block->AddBlock((BYTE*)&hsz);

	hszItem = hsz.hszItem;

	return block->GetBlockCount()-1;
}
*/

static void ChangeItemCharToComma(TCHAR *item)
{
	static char flag;
	static int  code = -1;

	if(flag == OFF) {
		CString filename;
		StackChar win_dir(MAX_PATH);

		GetWindowsDirectory(win_dir.data, MAX_PATH);

		filename.Format(_T("%s\\DdeTag.ini"), win_dir.data);

		code = GetPrivateProfileInt(_T("Item"), _T("Comma"), -1, filename);

		flag = ON;
	}
	
	if(code == -1)	return;

	for(int i = 0; i < (int)_tcslen(item); i++) {
		if(item[i] == code) {
			item[i] = ',';
		}
	}
}

static int LinkItem(TCHAR *service, TCHAR *topic, TCHAR *item_s, DWORD &pos_s, DWORD &pos_t, DWORD &pos_i, char advise_flag)
{
	if(bFlagDDE == OFF)	return 0;		// Dde가 설치되지 않았다.

	if(service == NULL)	return 0;
	if(service[0] == 0)	return 0;
	if(topic == NULL)	return 0;
	if(topic[0] == 0)	return 0;
	if(item_s == NULL)	return 0;
	if(item_s[0] == 0)	return 0;

	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	CELL_DDE_ITEM    blockI;
	TCHAR item[80];

	_tcscpy(item, item_s);

	ChangeItemCharToComma(item);

	if(IsSameServiceExist(service, pos_s)) {
		blockDdeService.GetBlock((BYTE*)&blockS, pos_s);
	}
	else {
		memset(&blockS, 0, sizeof(CELL_DDE_SERVICE));
		_tcscpy(blockS.name, service);
		blockS.hszService = DdeCreateStringHandle(idInst, service,  CP_WINANSI);
		blockS.blockTopic = new Block(sizeof(CELL_DDE_TOPIC));
		blockDdeService.AddBlock((BYTE*)&blockS);

		pos_s = blockDdeService.GetBlockCount()-1;

		AddListService(&blockS);
	}

	if(IsSameTopicExist(blockS.blockTopic, topic, pos_t)) {
		blockS.blockTopic->GetBlock((BYTE*)&blockT, pos_t);
	}
	else {
		memset(&blockT, 0, sizeof(CELL_DDE_TOPIC));
		_tcscpy(blockT.name, topic);
		blockT.hszTopic = AddOneTopic(topic);
		blockT.hConv = DdeConnect(idInst, blockS.hszService, blockT.hszTopic, NULL);
		blockT.blockItem = new Block(sizeof(CELL_DDE_ITEM));
		blockS.blockTopic->AddBlock((BYTE*)&blockT);

		pos_t = blockS.blockTopic->GetBlockCount()-1;

		AddListTopic(&blockT, pos_s);

		if(blockT.hConv) {	// topic 이 연결 되면 bRegister를 ON 한다.
			blockS.bRegister = ON;
			blockDdeService.SetBlock((BYTE*)&blockS, pos_s);
		}
	}

	if(IsSameItemExist(blockT.blockItem, item, pos_i)) {
		//blockS.blockItem->GetBlock((BYTE*)&blockI, pos_i);
	}
	else {
		HDDEDATA retn;
		DWORD dwResult;

		memset(&blockI, 0, sizeof(CELL_DDE_ITEM));
		_tcscpy(blockI.name, item);
		blockI.hszItem = AddOneItem(item);
		blockI.advise_flag = advise_flag;

		if(advise_flag) {
			if(blockT.hConv == 0) {	// topic이 접속되지 않았을때.
#if	defined (COMPILE_ENGLISH)
				strcpy(blockI.data, "Topic connect unable");
#else
				strcpy(blockI.data, "Topic 접속 불가");
#endif
			}
			else {
				// 2000은 timeout 시간 2sec 바로 돌아오기를 원할 때는 TIMEOUT_ASYNC사용
				retn = DdeClientTransaction(NULL, 0, blockT.hConv, blockI.hszItem, CF_TEXT, XTYP_ADVSTART, TIMEOUT_ASYNC, &dwResult);

				if(retn == FALSE) {
#if	defined (COMPILE_ENGLISH)
					strcpy(blockI.data, "Item connect unable 불가");
#else
					strcpy(blockI.data, "Item 연결 불가");
#endif
				}
				else {
#if	defined (COMPILE_ENGLISH)
					strcpy(blockI.data, "Data connecting");
#else
					strcpy(blockI.data, "데이타 연결중");
#endif
				}
				
			}
			
			// Topic이 접속되어 있지 않더라도 아이템은 만들어서 배열에 추가한다. 2011-5-23
			blockT.blockItem->AddBlock((BYTE*)&blockI);
			pos_i = blockT.blockItem->GetBlockCount()-1;

			AddListItem(&blockI, pos_s, pos_t);

			/*
			if(blockT.hConv == 0) {	// topic이 접속되지 않았을때.
#if	defined (COMPILE_ENGLISH)
				strcpy(blockI.data, "Topic connect unable");
#else
				strcpy(blockI.data, "Topic 접속 불가");
#endif
			}
			else {
				// 2000은 timeout 시간 2sec 바로 돌아오기를 원할 때는 TIMEOUT_ASYNC사용
				retn = DdeClientTransaction(NULL, 0, blockT.hConv, blockI.hszItem, CF_TEXT, XTYP_ADVSTART, TIMEOUT_ASYNC, &dwResult);

				if(retn == FALSE) {
#if	defined (COMPILE_ENGLISH)
					strcpy(blockI.data, "Item connect unable 불가");
#else
					strcpy(blockI.data, "Item 연결 불가");
#endif
				}
				else {
#if	defined (COMPILE_ENGLISH)
					strcpy(blockI.data, "Data connecting");
#else
					strcpy(blockI.data, "데이타 연결중");
#endif
				}
				
				blockT.blockItem->AddBlock((BYTE*)&blockI);
				pos_i = blockT.blockItem->GetBlockCount()-1;

				AddListItem(&blockI, pos_s, pos_t);
			}*/
		}
		else {		// advise하지 않는 item 은
			blockT.blockItem->AddBlock((BYTE*)&blockI);
			pos_i = blockT.blockItem->GetBlockCount()-1;

			AddListItem(&blockI, pos_s, pos_t);
		}
	}

	return 1;
}

int DdeLibRequestItem(DWORD service, DWORD topic, DWORD item, char *data, int data_size)
{
	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	CELL_DDE_ITEM    blockI;

	HDDEDATA retn;
	DWORD dwResult;

	if(service >= blockDdeService.GetBlockCount()) {
#if	defined (COMPILE_ENGLISH)
		strcpy(data, "DDE service not exist");
#else
		strcpy(data, "DDE service 없슴");
#endif
		return 0;
	}

	blockDdeService.GetBlock((BYTE*)&blockS, service);

	if(topic >= blockS.blockTopic->GetBlockCount()) {
#if	defined (COMPILE_ENGLISH)
		strcpy(data, "DDE topic not found");
#else
		strcpy(data, "DDE topic 없슴");
#endif
		return 0;
	}

	blockS.blockTopic->GetBlock((BYTE*)&blockT, topic);

	if(item >= blockT.blockItem->GetBlockCount()) {
#if	defined (COMPILE_ENGLISH)
		strcpy(data, "DDE itemm not found");
#else
		strcpy(data, "DDE item 없슴");
#endif
		return 0;
	}

	blockT.blockItem->GetBlock((BYTE*)&blockI, item);

	// strcpy(data, blockI.data);

	retn = DdeClientTransaction(NULL, 0, blockT.hConv, blockI.hszItem, CF_TEXT, XTYP_REQUEST, 2000, &dwResult);
	if(retn != 0) {
		//char data[80];
		DdeGetData(retn, (BYTE*)data, data_size, 0);
		DdeFreeDataHandle(retn);
	}
	else {
		return 0;
	}

	return 1;
}


int DdeLibChangedItem(DWORD service, DWORD topic, DWORD item, char *data, int data_size)
{
	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	CELL_DDE_ITEM    *blockI;

	if(service >= blockDdeService.GetBlockCount()) {
		return 0;
	}

	blockDdeService.GetBlock((BYTE*)&blockS, service);

	if(topic >= blockS.blockTopic->GetBlockCount()) {
		return 0;
	}

	blockS.blockTopic->GetBlock((BYTE*)&blockT, topic);

	if(item >= blockT.blockItem->GetBlockCount()) {
		return 0;
	}

	blockI = (CELL_DDE_ITEM*)blockT.blockItem->GetPtr(item);

	if(blockI->bChanged == 0)	return 0;

	strcpy(data, blockI->data);
	blockI->bChanged = 0;

	return 1;
}

//------------------------------------------------------------------------------
// Advise받을 Item을 연결하고 등록한다.
//------------------------------------------------------------------------------

int DdeLibLinkItemAdvise(TCHAR *service, TCHAR *topic, TCHAR *item, DWORD &pos_s, DWORD &pos_t, DWORD &pos_i)
{
	return LinkItem(service, topic, item, pos_s, pos_t, pos_i, ON);
}

//------------------------------------------------------------------------------
// Poke할 Item을 연결하고 등록한다.
//------------------------------------------------------------------------------

int DdeLibLinkItemPoke(TCHAR *service, TCHAR *topic, TCHAR *item, DWORD &pos_s, DWORD &pos_t, DWORD &pos_i)
{
	return LinkItem(service, topic, item, pos_s, pos_t, pos_i, OFF);
}

/*
void DdeLibGetData(DWORD service, DWORD topic, DWORD item, char *data)
{
	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	CELL_DDE_ITEM    blockI;

	if(service >= blockDdeService.GetBlockCount()) {
#if	defined (COMPILE_ENGLISH)
		strcpy(data, "DDE service not exist");
#else
		strcpy(data, "DDE service 없슴");
#endif
		return;
	}

	blockDdeService.GetBlock((BYTE*)&blockS, service);

	if(topic >= blockS.blockTopic->GetBlockCount()) {
#if	defined (COMPILE_ENGLISH)
		strcpy(data, "DDE topic not found");
#else
		strcpy(data, "DDE topic 없슴");
#endif
		return;
	}

	blockS.blockTopic->GetBlock((BYTE*)&blockT, topic);

	if(item >= blockT.blockItem->GetBlockCount()) {
#if	defined (COMPILE_ENGLISH)
		strcpy(data, "DDE itemm not found");
#else
		strcpy(data, "DDE item 없슴");
#endif
		return;
	}

	blockT.blockItem->GetBlock((BYTE*)&blockI, item);

	strcpy(data, blockI.data);
}
*/

int DdeLibGetPokeHandle(DWORD pos_s, DWORD pos_t, DWORD pos_i, HCONV &hconv, HSZ &hszTopic, HSZ &hszItem)
{
	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	CELL_DDE_ITEM    blockI;

	if(pos_s >= blockDdeService.GetBlockCount()) {
		//strcpy(data, "DDE service 없슴");
		return 0;
	}

	blockDdeService.GetBlock((BYTE*)&blockS, pos_s);

	if(pos_t >= blockS.blockTopic->GetBlockCount()) {
		//strcpy(data, "DDE topic 없슴");
		return 0;
	}

	blockS.blockTopic->GetBlock((BYTE*)&blockT, pos_t);

	if(pos_i >= blockT.blockItem->GetBlockCount()) {
		//strcpy(data, "DDE item 없슴");
		return 0;
	}

	blockT.blockItem->GetBlock((BYTE*)&blockI, pos_i);

	hconv = blockT.hConv;
	hszTopic = blockT.hszTopic;
	hszItem = blockI.hszItem;

	return 1;
}

//------------------------------------------------------------------------------
//	DDE server가 언제 시작할지 모르기 때문에 시간날 때마다 한번씩 접속을 시도한다.
//------------------------------------------------------------------------------

void DdeLibConnectTry()
{
	if(bFlagDDE == OFF)	return;			// Dde가 설치되지 않았다.
//	if(bDebugMode == ON)	return;		// debug 모드에서는 아무것도 하지 않는다.

	if(blockDdeService.GetBlockCount() == 0) 	return;

	static WORD old_sec;
	SYSTEMTIME t;
	GetLocalTime(&t);
	if(t.wSecond == old_sec)	return;	// 1 sec 마다 한번씩 시도한다.
	old_sec = t.wSecond;

	CELL_DDE_SERVICE blockS;
	CELL_DDE_TOPIC   blockT;
	CELL_DDE_ITEM    blockI;
	static DWORD pos_s = 0;
	static DWORD pos_t = 0;
	DWORD pos_i;
	// HDDEDATA retn;
	DWORD dwResult;

	pos_s %= blockDdeService.GetBlockCount(); 

	blockDdeService.GetBlock((BYTE*)&blockS, pos_s);

	if(pos_t <= blockS.blockTopic->GetBlockCount()-1) {
		blockS.blockTopic->GetBlock((BYTE*)&blockT, pos_t);
		if(blockT.hConv == NULL) {
			blockT.hConv = DdeConnect(idInst, blockS.hszService, blockT.hszTopic, NULL);
			if(blockT.hConv) {	// 접속에 성공했다.
				if(blockT.blockItem != NULL) {
					for(pos_i = 0; pos_i < blockT.blockItem->GetBlockCount(); pos_i++) {
						blockT.blockItem->GetBlock((BYTE*)&blockI, pos_i);
						if(blockI.hszItem) {	// yes item corrected
							// 2000은 timeout 시간 2sec 바로 돌아오기를 원할 때는 TIMEOUT_ASYNC사용
							DdeClientTransaction(NULL, 0, blockT.hConv, blockI.hszItem, CF_TEXT, XTYP_ADVSTART, TIMEOUT_ASYNC, &dwResult);
						}
					}
				}

				blockS.blockTopic->SetBlock((BYTE*)&blockT, pos_t);
				ChangeListTopic(&blockT, pos_s, pos_t);
			}
		}
	}
	pos_t++;

	if(pos_t >= blockS.blockTopic->GetBlockCount()) {
		pos_s ++;
		pos_t = 0;
	}
}

DWORD DdeLibGetInst()
{
	return idInst;
}




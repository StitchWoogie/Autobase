// english O.K
//------------------------------------------------------------------------------
//	컴퓨터 자체에 내장되어 있는 rs-232 의 통신을 담당한다.
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <conio.h>
#include <string.h>
#include <dos.h>
#include <time.h>

#include <tools.h>
#include <dataswap.h>
#include <glib.h>

#include "commmain.h"
#include "..\resource.h"
#include "..\plc_scan.h"

void FillStruct232(DEVICE_STRUCT_RS232 *rs232, CommaBlockString *comma);
void ShowError232(HWND hwnd, int err, char *message, DEVICE_STRUCT_RS232 *rs232);

enum {
	ORDER_WAITING_OK_AFTER_INIT_COMMAND,	// 초기화를 하고 O.K를 기다리는 중
	ORDER_WAITING_CONNECT,					// connect 신호를 기다리는 중이다.
	ORDER_TALKING_PLC,						// PLC와 통신하고 있는 중이다.
};

typedef struct {
	int		user_owner;		// 현재 이모뎀을 사용중인 포트번호 -1 은 사용중이 아님
	int		order;			// 현재 진행중인 명령. 
	TimeOutClass timeout;
	int		read_count;		// 현재 읽은 개수
	char	read_buf[80];	// 읽은 버퍼
} MODEM_LIST;

static MODEM_LIST modemList[256];	// 모뎀을 많이 부착할 수 있으므로 256개를 준비한다.

static void SetErrorMsg(DEVICE_STRUCT_MODEM *modem, char *error)
{
	modem->connect_error_count++;
	strcpy(modem->connect_error_msg, error);
}

//-------------------------------------------------------------------------------------
// 모뎀의 초기화는 스트링을 해석하고 포트가 존재하고 옵션이 이상이 없는가를 검사한 후 
// 포트를 닫고 복귀한다.
//-------------------------------------------------------------------------------------

int PlcDeviceInitModem(HWND hwnd, DEVICE_STRUCT_MODEM *modem, CommaBlockString *comma, TELEPHONE_STRUCT *tel)
{
	FillStruct232(&modem->rs232, comma);	// 스트링을 해석한다.

	GetLocalTime(&modem->tLastCall);

	char buf[80];
	HANDLE id;

	//wsprintf(buf, "COM%d", modem->rs232.port+1);	COM port가 10번이 넘으면 이것으로는 안되는것 같다.
	sprintf(buf, "\\\\.\\COM%d", modem->rs232.port+1);		// 파일명을 232와 같이 \\.\COM10 과 같이 변경했다.  2013-6-11

	if(modem->rs232.port < 0 || modem->rs232.port > 255) {
		if(IsLangKorean()) {
			sprintf(buf, "COM%d: Modem 은 1~256까지 사용할 수 있습니다.", modem->rs232.port+1);
		}else {
			sprintf(buf, "COM%d: Modem port can use 1~256.", modem->rs232.port+1);
		}
		MessageDisplay(buf);
		return 0;
	}

	id = CreateFile(buf, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, NULL); 
	
	if(id == INVALID_HANDLE_VALUE) {
		ShowError232(hwnd, GetLastError(), "Modem OpenComm", &modem->rs232);

		return 0;
	}

	// setup device buffers
	if(SetupComm(id, 4096, 4096 ) == FALSE) {
		ShowError232(hwnd, GetLastError(), "Modem SetupComm", &modem->rs232);
		return 0;
	}

	CloseHandle(id);

	modemList[modem->rs232.port].user_owner = -1;	// 이 모뎀 사용자는 없다.

	PokeBitSYSTEM(modem->port_no, POKE_BIT_MODEM_AUTO_CONNECTION, tel->bAutoConnection);

	return 1;
}

int PlcDeviceOpenRS232(HWND hwnd, DEVICE_STRUCT_RS232 *rs232);

static void WriteString(DEVICE_STRUCT_MODEM *modem, char *string)
{
	PlcDeviceWriteContinueRS232(&modem->rs232, string, strlen(string));
}

static void WriteChar(DEVICE_STRUCT_MODEM *modem, char ch)
{
	char string[10];

	string[0] = ch;
	string[1] = 0;

	PlcDeviceWriteContinueRS232(&modem->rs232, string, strlen(string));
}

static void ClearModemBuf(DEVICE_STRUCT_MODEM *modem)
{
	MODEM_LIST *list = &modemList[modem->rs232.port];
	
	list->read_count = 0;
	PlcDeviceClearRS232(&modem->rs232);
}

static int WaitString(DEVICE_STRUCT_MODEM *modem)
{
	MODEM_LIST *list = &modemList[modem->rs232.port];
	int i;
	char buf[10];

	for(i = 0; i < 10; i++) {
		if(!PlcDeviceReadContinueRS232(&modem->rs232, buf, 1))	break;

		if(buf[0] == '\r' || buf[0] == '\n') {
			if(list->read_count > 0) {
				list->read_buf[list->read_count] = 0;
				list->read_count = 0;
				return 1;	
			}
		}
		else {
			if(list->read_count < 79) {
				list->read_buf[list->read_count] = buf[0];
				list->read_count ++;
			}
		}
	}

	return 0;
}

static int WaitOK(DEVICE_STRUCT_MODEM *modem) 
{
	MODEM_LIST *list = &modemList[modem->rs232.port];

	if(WaitString(modem)) {
		if(strcmp(list->read_buf, "OK") == 0)	return 1;
	}

	return 0;
}

static int WaitConnect(DEVICE_STRUCT_MODEM *modem) 
{
	MODEM_LIST *list = &modemList[modem->rs232.port];

	if(WaitString(modem)) {
		if(strncmp(list->read_buf, "CONNECT", 7) == 0)		return 1;
		if(strcmp(list->read_buf, "BUSY") == 0)	{
			if(IsLangKorean()) {
				SetErrorMsg(modem, "통화 중");
			} else {
				SetErrorMsg(modem, "Line Busy");
			}
			return -1;
		}
		if(stricmp(list->read_buf, "ERROR") == 0)	{
			if(IsLangKorean()) {
						SetErrorMsg(modem, "모뎀이 ERROR 응답");
			} else {
						SetErrorMsg(modem, "Modem ERROR Response");
			}
			WriteString(modem, "ATZ\r");	// 모뎀 reset 명령
			return -1;
		}
		if(stricmp(list->read_buf, "NO DIALTONE") == 0)	{
			if(IsLangKorean()) {
						SetErrorMsg(modem, "발신음 없음");
			} else {
						SetErrorMsg(modem, "No DialTone");
			}
			WriteString(modem, "ATZ\r");	// 모뎀 reset 명령
			return -1;
		}
	}

	return 0;
}

static void ChangeToModemMode(DEVICE_STRUCT_MODEM *modem)
{
	int i;
	TimeOutClass timeout;

	for(i = 0; i < 3; i++) {
		WriteString(modem, "+++");
		timeout.Reset();
		while(1) {
			if(timeout.IsTimeOut(2))	break;
			if(WaitOK(modem))		return;
		}
	}
}

static void HangUp(DEVICE_STRUCT_MODEM *modem)
{
	int i;
	TimeOutClass timeout;

	for(i = 0; i < 3; i++) {
		WriteString(modem, "ATH\r");
		timeout.Reset();
		while(1) {
			if(timeout.IsTimeOut(2))	break;
			if(WaitOK(modem))			return;
		}
	}
}

static void UpdateCountDown(DEVICE_STRUCT_MODEM *modem, SYSTEMTIME *t)
{
	if(t->wSecond == modem->tCountDown.wSecond)	return;

	memcpy(&modem->tCountDown, t, sizeof(SYSTEMTIME));
}

//--------------------------------------------------------------------------------------------
//	전화 걸 시간이 되었는가를 검사한다.
//--------------------------------------------------------------------------------------------

int IsTimeToCall(DEVICE_STRUCT_MODEM *modem, TELEPHONE_STRUCT *tel)
{
	SYSTEMTIME t;
	long  countdown;
	long  gab;
	long  curr;
	long  old;

	GetLocalTime(&t);

	curr = GetMinHap(&t);
	old  = GetMinHap(&modem->tLastCall);

	gab = curr-old;
	countdown = tel->nConnectCicle-gab;
	if(countdown < 0)	countdown = 0;

	if(countdown == 0) {
		t.wHour = 0;
		t.wMinute = 0;
		t.wSecond = 0;
	}
	else {
		t.wHour    = (WORD)((countdown-1)/60);
		t.wMinute  = (WORD)((countdown-1)%60);
		t.wSecond  = 59-t.wSecond;
	}

	UpdateCountDown(modem, &t);

	if(modem->bHandConnection)	return 1;

	if(tel->bAutoConnection == OFF) {	// 자동이 아닐때는 고정 접속시간을 무시한다. 
		return 0;
	}

	if(countdown == 0) {
		return 1;
	}
					
	return 0;	
}

static void UpdateLastCallTime(DEVICE_STRUCT_MODEM *modem)
{
	GetLocalTime(&modem->tLastCall);
}

static void SetMemoryConnectSignal(int port_no, char flag)
{
	if(port_no >= nPortHap)	return;

	GLOBAL_PORT_STRUCT *port = &portBuf[port_no];

	if(flag)	port->bufSYSTEM[1].value |= WORD_MASK[0];
	else		port->bufSYSTEM[1].value &= (0xF-WORD_MASK[0]);
}

static void ResetFlag(DEVICE_STRUCT_MODEM *modem)
{
	MODEM_LIST *list = &modemList[modem->rs232.port];
	
	list->user_owner = -1;
	modem->bHandConnection = OFF;
	SetMemoryConnectSignal(modem->port_no, OFF);
}

void WriteTelNumber(DEVICE_STRUCT_MODEM *modem, char *tel)
{
	unsigned u;

	for(u = 0; u < strlen(tel); u++) {
		if(tel[u] == '.')	WriteChar(modem, ',');
		else				WriteChar(modem, tel[u]);
	}
}

int PlcDeviceCheckConnectingModem(DEVICE_STRUCT_MODEM *modem, TELEPHONE_STRUCT *tel)
{
	MODEM_LIST *list = &modemList[modem->rs232.port];
	int retn;
	SYSTEMTIME t;
	
	if(list->user_owner == -1) {	// 현재 모뎀 사용자는 없다.
		list->user_owner = modem->port_no;
		
		if(modem->connect_error_count >= 10) {	// 오류가 10번이상 발생했으므로 return;
			ResetFlag(modem);
			return 0;
		}

		if(tel->sTelNumber[0] == 0) {
			if(IsLangKorean()) {
			SetErrorMsg(modem, "전화번호 설정안됨(빈칸)");
			}else {
			SetErrorMsg(modem, "Telephone number not defined.");
			}
			list->user_owner = -1;
			return 0;
		}

		if(!IsTimeToCall(modem, tel)) {
			list->user_owner = -1;
			return 0;	// 접속할 시간이 되지 않았다.
		}

		if(!PlcDeviceOpenRS232(NULL, &modem->rs232)) {	// 초기화 실패
			if(IsLangKorean()) {
				SetErrorMsg(modem, "모뎀열기 실패");
			}else {
				SetErrorMsg(modem, "Modem Open Failed.");
			}
			list->user_owner = -1;
			return 0;
		}
		list->order = ORDER_WAITING_OK_AFTER_INIT_COMMAND;
		list->timeout.Reset();

		{
			ClearModemBuf(modem);

			void ModemOptionFileRead(int modem_no, MODEM_FILE_OPTION *option);
			MODEM_FILE_OPTION option;

			ModemOptionFileRead(modem->rs232.port, &option);
			WriteString(modem, option.init_command);
			WriteString(modem, "\r");
		}

		return 0;
	}

	if(list->user_owner != modem->port_no) {
		IsTimeToCall(modem, tel);	// 원래는 이 부분에서 할 필요가 없으나 count down을 계산하기 위해서
		return 0;	// 모뎀을 다른포트에서 사용중이다.
	}

	if(list->order == ORDER_WAITING_OK_AFTER_INIT_COMMAND) {
		if(list->timeout.IsTimeOut(3)) {
			PlcDeviceUnInitRS232(&modem->rs232);
			list->user_owner = -1;
			if(IsLangKorean()) {
				SetErrorMsg(modem, "모뎀 초기화 명령 시간초과");
			}else {
				SetErrorMsg(modem, "Modem initial command time out.");	
			}
			return 0;
		}
		if(WaitOK(modem)) {
			list->order = ORDER_WAITING_CONNECT;
			list->timeout.Reset();
			ClearModemBuf(modem);		
			
			WriteString(modem, "ATDT ");
			WriteTelNumber(modem, tel->sTelNumber);
			WriteString(modem, "\r");
			return 0;
		}
		return 0;
	}
	else if(list->order == ORDER_WAITING_CONNECT) {
		if(list->timeout.IsTimeOut(40)) {	// connect time out
			WriteChar(modem, 27);	// cancel code
			PlcDeviceUnInitRS232(&modem->rs232);
			list->user_owner = -1;
			if(IsLangKorean()) {
				SetErrorMsg(modem, "전화 걸기 시간초과");
			}else {
				SetErrorMsg(modem, "Call Time out");
			}
			return 0;
		}
		else {
			int remain = 40-list->timeout.GetCurrCount();
			t.wHour = 0;
			t.wMinute  = remain/60;
			t.wSecond  = remain%60;
			UpdateCountDown(modem, &t);
		}

		retn = WaitConnect(modem);

		if(retn == -1) {	// error 발생
			PlcDeviceUnInitRS232(&modem->rs232);
			list->user_owner = -1;
			return 0;
		}

		if(retn == 1) {
			list->order = ORDER_TALKING_PLC;
			list->timeout.Reset();
			UpdateLastCallTime(modem);
			SetMemoryConnectSignal(modem->port_no, ON);
			return 1;
		}
	}
	else if(list->order == ORDER_TALKING_PLC) {				// 현재 PLC와 접속 중이다.
		// 수동접속이고 접속시간이 0일때는 자동으로 끊지 않는다.
		if(modem->bHandConnection && tel->nConnectingTimeOnManual == 0) {
			return 1;
		}

		if(list->timeout.IsTimeOut(tel->nConnectingTime)) {	// connect time out	접속을 종료할 시간이 되었다.	
			ChangeToModemMode(modem);
			HangUp(modem);
			PlcDeviceResetErrorCountModem(modem);
			PlcDeviceUnInitRS232(&modem->rs232);
			UpdateLastCallTime(modem);
			ResetFlag(modem);
			return 0;
		}
		else {
			int remain = tel->nConnectingTime-list->timeout.GetCurrCount();
			t.wHour = 0;
			t.wMinute = remain/60;
			t.wSecond = remain%60;
			UpdateCountDown(modem, &t);
		}
		return 1;
	}

	return 0;
}

int PlcDeviceUnInitModem(DEVICE_STRUCT_MODEM *modem)
{
	MODEM_LIST *list = &modemList[modem->rs232.port];	

	if(list->user_owner == modem->port_no) {
		if(list->order == ORDER_TALKING_PLC) {	// 현재 PLC와 접속 중이다.
			ChangeToModemMode(modem);
			HangUp(modem);
		}
		else {
			WriteChar(modem, 27);
		}

		list->user_owner = -1;
		return PlcDeviceUnInitRS232(&modem->rs232);
	}

	return 1;
}

void PlcDeviceResetErrorCountModem(DEVICE_STRUCT_MODEM *modem)
{
	modem->connect_error_count = 0;
	if(IsLangKorean()) {
		strcpy(modem->connect_error_msg, "정상");
	}else {
		strcpy(modem->connect_error_msg, "Normal");
	}
}

int PlcDeviceReadContinueModem(DEVICE_STRUCT_MODEM *modem, char *buf, int count)
{
	return PlcDeviceReadContinueRS232(&modem->rs232, buf, count);
}

int PlcDeviceWriteContinueModem(DEVICE_STRUCT_MODEM *modem, char *buf, int count)
{
	return PlcDeviceWriteContinueRS232(&modem->rs232, buf, count);
}

int PlcDeviceClearModem(DEVICE_STRUCT_MODEM *modem)
{
	return PlcDeviceClearRS232(&modem->rs232);
}

void PlcDeviceSetHandDisConnectModem(DEVICE_STRUCT_MODEM *modem)
{
	MODEM_LIST *list = &modemList[modem->rs232.port];
	
	if(list->user_owner == -1) {	// 현재 모뎀 사용자는 없다.
		return;
	}

	if(list->user_owner != modem->port_no) {
		//IsTimeToCall(modem, tel);	// 원래는 이 부분에서 할 필요가 없으나 count down을 계산하기 위해서
		return;						// 모뎀을 다른포트에서 사용중이다.
	}

	if(list->order == ORDER_WAITING_OK_AFTER_INIT_COMMAND) {
		PlcDeviceUnInitRS232(&modem->rs232);
		ResetFlag(modem);
		return;
	}
	else if(list->order == ORDER_WAITING_CONNECT) {
		WriteChar(modem, 27);	// cancel code
		PlcDeviceUnInitRS232(&modem->rs232);
		ResetFlag(modem);
	}
	else if(list->order == ORDER_TALKING_PLC) {				// 현재 PLC와 접속 중이다.
		ChangeToModemMode(modem);
		HangUp(modem);
		PlcDeviceResetErrorCountModem(modem);
		PlcDeviceUnInitRS232(&modem->rs232);
		UpdateLastCallTime(modem);
		ResetFlag(modem);
	}
	else;
}



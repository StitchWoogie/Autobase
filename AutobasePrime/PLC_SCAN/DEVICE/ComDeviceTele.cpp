// english O.K

//------------------------------------------------------------------------------
//	모뎀-Device ~~~~ Device-모뎀 형식의 Device
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
void ShowError232(HWND hwnd, int err, char *message);

static void WriteString(DEVICE_STRUCT_TELE *tele, char *string)
{
	PlcDeviceWriteContinueRS232(&tele->rs232, string, strlen(string));
}

static int WaitString(DEVICE_STRUCT_TELE *tele, char *wait_string, int time_out)
{
	int  count = 0;
	char buf[80];
	char imsi[80];
	struct time t;
	int  old_sec;
	int  sec_count = 0;
	StackChar msg(1000);

	gettime(&t);
	old_sec = t.ti_sec;

	while(1) {
		gettime(&t);
		if(t.ti_sec != old_sec) {	// time out check
			old_sec = t.ti_sec;
			sec_count ++;
			if(sec_count >= time_out)	return 0;
			sprintf(msg.data, "TeleDevice%d is wait %s signal (wait=%d sec)", tele->rs232.port+1, wait_string, sec_count);
			MessageDisplay(msg.data);
		}
		if(!PlcDeviceReadContinueRS232(&tele->rs232, imsi, 1))	continue;

		if(imsi[0] == '\r' || imsi[0] == '\n') {
			count = 0;
		}
		else {
			if(count < 70) {
				buf[count] = imsi[0];
				count++;
				buf[count] = 0;
				if(strcmp(buf, wait_string) == 0)	return 1;
			}
		}
	}
}

//-------------------------------------------------------------------------------------
// 모뎀의 초기화는 스트링을 해석하고 포트가 존재하고 옵션이 이상이 없는가를 검사한 후 
// 포트를 닫고 복귀한다.
//-------------------------------------------------------------------------------------

int PlcDeviceOpenRS232(HWND hwnd, DEVICE_STRUCT_RS232 *rs232);

int PlcDeviceInitTele(HWND hwnd, DEVICE_STRUCT_TELE *tele, CommaBlockString *comma)
{
	FillStruct232(&tele->rs232, comma);	// 스트링을 해석한다.

	int retn = PlcDeviceOpenRS232(hwnd, &tele->rs232);

	if(retn == 0)	return retn;

	TELE_DEVICE_OPTION opt;
	StackChar msg(1000);

	LoadTeleDeviceOption(tele->rs232.port+1, &opt);

	WriteString(tele, opt.sInitCommand);
	WriteString(tele, "\r");

	if(WaitString(tele, "OK", 3)) {
		sprintf(msg.data, "TeleDevice%d success INIT Command.", tele->rs232.port+1);
	}
	else {
		sprintf(msg.data, "TeleDevice%d failed INIT Command", tele->rs232.port+1);
	}

	MessageDisplay(msg.data);

	Sleep(2000);

	WriteString(tele, opt.sConnectCommand);
	WriteString(tele, "\r");

	if(WaitString(tele, "CONNECT", opt.nTimeOutWaitConnect)) {
		sprintf(msg.data, "TeleDevice%d is CONNECTED.", tele->rs232.port+1);
	}
	else {
		sprintf(msg.data, "TeleDevice%d CONNECTION failed.", tele->rs232.port+1);
	}

	MessageDisplay(msg.data);

	return 1;
}

static void WriteChar(DEVICE_STRUCT_TELE *tele, char ch)
{
	char string[10];

	string[0] = ch;
	string[1] = 0;

	PlcDeviceWriteContinueRS232(&tele->rs232, string, strlen(string));
}

int PlcDeviceUnInitTele(DEVICE_STRUCT_TELE *tele)
{
	WriteString(tele, "+++");

	Sleep(1000);

	//WriteString(tele, "ATZ\r");
	
	return PlcDeviceUnInitRS232(&tele->rs232);
}

int PlcDeviceReadContinueTele(DEVICE_STRUCT_TELE *tele, char *buf, int count)
{
	return PlcDeviceReadContinueRS232(&tele->rs232, buf, count);
}

int PlcDeviceWriteContinueTele(DEVICE_STRUCT_TELE *tele, char *buf, int count)
{
	return PlcDeviceWriteContinueRS232(&tele->rs232, buf, count);
}

int PlcDeviceClearTele(DEVICE_STRUCT_TELE *tele)
{
	return PlcDeviceClearRS232(&tele->rs232);
}


// english O.K
#include "stdafx.h"
#include <compiler.hpp>
#include <stdarg.h>
//#include <winsock.h>

#include <tools.h>
#include <dataswap.h>
#include <gatelib.h>
#include <modemlib.h>
#include <tcpiplib.h>
#include <udpiplib.h>

typedef struct { 			// 현재 서버에 연결되어 있는 Terminal의 정보를 나타낸다.
	int   type;				// 현재 서버에 연결되어 있는 type
	MODEM_STRUCT modem;		// 모뎀일때의 구조체.
	TCPIP_STRUCT tcpip;		// TCPIP일때 구조체.
	UDPIP_STRUCT udpip;		// UDPIP일때 구조체.
	int	nTimeNoKey;			// 키가 입력되지 않았을 때 끊는대기 시간.
} GATE_STRUCT;

static HGATE GateRegister(char *device, int type)
{
	HGATE hGate;

	hGate = GlobalAlloc(GMEM_MOVEABLE | GMEM_ZEROINIT, sizeof(GATE_STRUCT));

	if(hGate == NULL)	return NULL;

	GATE_STRUCT *gate;

	gate = (GATE_STRUCT*)GlobalLock(hGate);

	gate->type = type;
	switch(type) {
		case GATE_TYPE_MODEM:
			memcpy(&gate->modem, device, sizeof(MODEM_STRUCT));
			break;
		case GATE_TYPE_TCPIP:
			memcpy(&gate->tcpip, device, sizeof(TCPIP_STRUCT));
			break;
		case GATE_TYPE_UDPIP:
			memcpy(&gate->udpip, device, sizeof(UDPIP_STRUCT));
			break;
	}

	GlobalUnlock(hGate);

	return hGate;
}

HGATE GateRegister(MODEM_STRUCT *modem)
{
	return GateRegister((char*)modem, GATE_TYPE_MODEM);
}

//------------------------------------------------------------------------------
//	TCP/IP는 서로가 socket접속이 된 상태에서만 등록할 수 있다.
//------------------------------------------------------------------------------

HGATE GateRegister(TCPIP_STRUCT *tcpip)
{
	tcpip->bConnect = ON;

	return GateRegister((char*)tcpip, GATE_TYPE_TCPIP);
}

HGATE GateRegister(UDPIP_STRUCT *device)
{
	return GateRegister((char*)device, GATE_TYPE_UDPIP);
}

int GateGetConnectType(HGATE hGate)
{
	GATE_STRUCT *gate;
	int retn;

	gate = (GATE_STRUCT*)GlobalLock(hGate);
	retn = gate->type;
	GlobalUnlock(hGate);

	return retn;
}

//------------------------------------------------------------------------------
//	232나 tcp/ip 의 모든 입/출력은 여기가 kernal이다.
//------------------------------------------------------------------------------

int GateWrite(HGATE hGate, char ch)
{
	char buf[10];

	buf[0] = ch;

	return GateWriteContinue(hGate, buf, 1);
}

int GateGetStruct(HGATE hGate, TCPIP_STRUCT *tcpip)
{
	if(hGate == NULL)	return 0;

	GATE_STRUCT *gate;
	int retn = 0;
	gate = (GATE_STRUCT*)GlobalLock(hGate);

	switch(gate->type) {
		case GATE_TYPE_NULL:
			break;
		case GATE_TYPE_MODEM:
			break;
		case GATE_TYPE_TCPIP:
			memcpy(tcpip, &gate->tcpip, sizeof(TCPIP_STRUCT));
			break;
		default:
			break;
	}

	GlobalUnlock(hGate);

	return retn;
}

int GateWriteContinue(HGATE hGate, char *buf, int size)
{
	if(hGate == NULL)	return 0;

	GATE_STRUCT *gate;
	int retn = 0;
	gate = (GATE_STRUCT*)GlobalLock(hGate);

	switch(gate->type) {
		case GATE_TYPE_NULL:
			break;
		case GATE_TYPE_MODEM:
			retn = ModemWriteContinue(&gate->modem, buf, size);
			break;
		case GATE_TYPE_TCPIP:
			retn = TcpipWriteContinue(&gate->tcpip, buf, size);
			break;
		case GATE_TYPE_UDPIP:
			retn = UdpipWriteContinue(&gate->udpip, buf, size);
			break;
		default:
			break;
	}

	GlobalUnlock(hGate);

	return retn;
}

int GateWriteString(HGATE hGate, LPSTR string, ...)
{
	va_list ap;
	StackChar imsi(1000);

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);

		return(GateWriteContinue(hGate, imsi.data, strlen(imsi.data)));
	}

	return 0;
}

int GateHangUp(HGATE hGate)
{
	if(hGate == NULL)	return 0;

	GATE_STRUCT *gate;
	int retn = 0;
	gate = (GATE_STRUCT*)GlobalLock(hGate);

	switch(gate->type) {
		case GATE_TYPE_NULL:
			break;
		case GATE_TYPE_MODEM:
			retn = ModemHangUp(&gate->modem);
			break;
		default:
			break;
	}

	GlobalUnlock(hGate);

	return retn;
}

int GateReadContinue(HGATE hGate, char *buf, int size)
{
	if(hGate == NULL)	return 0;

	GATE_STRUCT *gate;
	int retn = 0;
	gate = (GATE_STRUCT*)GlobalLock(hGate);

	switch(gate->type) {
		case GATE_TYPE_NULL:
			break;
		case GATE_TYPE_MODEM:
			retn = ModemReadContinue(&gate->modem, buf, size);
			break;
		case GATE_TYPE_TCPIP:
			retn = TcpipReadContinue(&gate->tcpip, buf, size);
			break;
		case GATE_TYPE_UDPIP:
			retn = UdpipReadContinue(&gate->udpip, buf, size);
			break;
		default:
			break;
	}

	GlobalUnlock(hGate);

	return retn;
}

void GateClear(HGATE hGate)
{
	if(hGate == NULL)	return;

	GATE_STRUCT *gate;
	gate = (GATE_STRUCT*)GlobalLock(hGate);

	switch(gate->type) {
		case GATE_TYPE_NULL:
			break;
		case GATE_TYPE_MODEM:
			ModemClear(&gate->modem);
			break;
		case GATE_TYPE_TCPIP:
			TcpipClear(&gate->tcpip);
			break;
		case GATE_TYPE_UDPIP:
			UdpipClear(&gate->udpip);
			break;
		default:
			break;
	}

	GlobalUnlock(hGate);
}

void GateGetInformationString(HGATE hGate, char *info)
{
	strcpy(info, "hGate Not Alloced");

	if(hGate == NULL)	return;

	GATE_STRUCT *gate;
	gate = (GATE_STRUCT*)GlobalLock(hGate);

	MODEM_STRUCT *modem;
	TCPIP_STRUCT *tcpip;
	UDPIP_STRUCT *udpip;

	switch(gate->type) {
		case GATE_TYPE_MODEM:
			modem = &gate->modem;
			sprintf(info, "COM%d, %lu, %d, %d, %d, %s",
								modem->cPort+1, modem->lBaud, modem->cParity, modem->cData, modem->cStop,
								modem->bNullOrModem ? "Modem" : "NULL Modem");
			break;
		case GATE_TYPE_TCPIP:
			tcpip = &gate->tcpip;
			sprintf(info, "TCPIP, %s, %d,", tcpip->ip, tcpip->port);
			break;
		case GATE_TYPE_UDPIP:
			udpip = &gate->udpip;
			sprintf(info, "UDPIP, %s, %d,", udpip->ip, udpip->port);
			break;
		default:
			sprintf(info, "Unknown Gate device");
	}

	GlobalUnlock(hGate);
}

void GateUninstall(HGATE hGate)
{
	if(hGate == NULL)	return;

	GATE_STRUCT *gate;
	gate = (GATE_STRUCT*)GlobalLock(hGate);

	switch(gate->type) {
		case GATE_TYPE_NULL:
			break;
		case GATE_TYPE_MODEM:
			ModemUninstall(&gate->modem);
			break;
		case GATE_TYPE_TCPIP:
			TcpipUninstall(&gate->tcpip);
			break;
		case GATE_TYPE_UDPIP:
			UdpipUninstall(&gate->udpip);
			break;
		default:
			break;
	}

	GlobalUnlock(hGate);
}

//------------------------------------------------------------------------------
//	주어진 스트링이 들어올 때까지 몇초간 기다린다.
//------------------------------------------------------------------------------

int GateWaitString(HGATE hGate, char *string)
{
	TimeOutClass timeout;
	int  curr = 0;
	char buf[2];
	char recv[100];
	int shap = strlen(string);

	timeout.Reset();
	while(1) {
		if(timeout.IsTimeOut(3))	return 0;
		if(GateReadContinue(hGate, buf, 1)) {
			if(buf[0] == 0x0d || buf[0] == 0x0a) {	// new line
				if(curr < shap) {
					curr = 0;
					continue;
				}
				if(strncmp(recv, string, shap) == 0) {
					return 1;
				}
				curr = 0;
			}
			else {
				if(curr < 99) {
					recv[curr] = buf[0];
					curr++;
				}
			}
		}
	}
}



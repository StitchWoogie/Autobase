#include "stdafx.h"
#include "afxsock.h"
#include "afxmt.h"

#include <dataswap.h>

#include "..\catlib\totalcfg.h"

static void RunSmsServiceServer()
{
	//CMutex mutex(0, "AutoBaseSmsWebServiceServer");
}

// return
// 1 = OK
// 2 = Socket Create Error
// 3 = Socket Connect Error;
// 5 = Recv timeout
// 100 = Username or password invalid
// 101 = 잔고부족
// 102 = 번호오류

int SmsWebSend(const char *tel_recv, const char *tel_send, const char *msg)
{
	RunSmsServiceServer();

	CSocket socket;
	StackChar ip(100);

	DWORD port;
	DWORD nTimeOut;

	LoadRegAutoBaseConfig("SMSService", "Config", "LocalServerPort", 7110, port);
	LoadRegAutoBaseConfig("SMSService", "Config", "ServerIP", "127.0.0.1", ip.data, 100);
	LoadRegAutoBaseConfig("SMSService", "Config", "ServerConnectTimeOut", 10, nTimeOut);

	if(socket.Create() == 0) {
		return 2;	
	}
	if(socket.Connect(ip.data, port) == 0) {
		return 3;
	}
	StackBYTE data(10000);
	int pos = 2;

	data.data[pos++] = 0;
	data.data[pos++] = strlen(tel_recv);
	for(int i = 0; i < (int)strlen(tel_recv); i++) {
		data.data[pos++] = tel_recv[i];
	}
	data.data[pos++] = 1;
	data.data[pos++] = strlen(tel_send);
	for(int i = 0; i < (int)strlen(tel_send); i++) {
		data.data[pos++] = tel_send[i];
	}
	data.data[pos++] = 2;
	data.data[pos++] = strlen(msg);
	for(int i = 0; i < (int)strlen(msg); i++) {
		data.data[pos++] = msg[i];
	}
	data.data[0] = pos%256;
	data.data[1] = pos/256;
	socket.Send(data.data, pos);
	
	TimeOutClass timeout;
	DWORD remain = 0;
	int read;
	int retn;
	while(1) {
		if(timeout.IsTimeOut(nTimeOut)) {
			socket.Close();
			return 5;
		}

		if(socket.IOCtl(FIONREAD, &remain) == 0)	continue;
		if(remain == 0)	continue;

		read = socket.Receive(data.data, remain);
		if(read >= 1) {
			retn = data.data[0];
			break;
		}
	}

	socket.Close();

	return retn;
}


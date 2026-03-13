#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>

#include <totaldef.h>
#include <tools.h>
#include <compiler.hpp>
#include <glib.h>
//#include <windll.h>
#include <gatelib.h>
#include <crc.hpp>
#include <dataswap.h>

#include "..\catlib.src\NetWorkProtocol.h"

#include "plc_scan.h"
#include "ScanServer.h"
#include "resource.h"

//------------------------------------------------------------------------------
//	AutoBase TeleServ 환경을 저장한다.
//------------------------------------------------------------------------------

static void LoadConnectListOne(int connect)
{
	CString filename;
	SCAN_SERVER_LIST *conn;
	char buf[80];

	conn = &scanServerList[connect];

	memset(conn, 0, sizeof(SCAN_SERVER_LIST));
	conn->struct_no = connect;

	filename.Format("%s\\SCAN\\SERVER\\serv%04d.ini", sDirWorkProject, connect);

	// 접속 방법 찾기.
	GetPrivateProfileString("config", "Connect Method", "None", buf, sizeof(buf), filename);

	if(strcmp(buf, "NULL") == 0)			conn->nDeviceType = 1;
	else if(strcmp(buf, "Modem") == 0) 		conn->nDeviceType = 2;
	else if(strcmp(buf, "TCP/IP") == 0)		conn->nDeviceType = 3;
	else if(strcmp(buf, "UDP/IP") == 0)		conn->nDeviceType = 4;
	else									conn->nDeviceType = 0;	// null modem

	GetPrivateProfileString("config", "Thread", "1", buf, sizeof(buf), filename);
	conn->bThreadFlag = atoi(buf);

	conn->nSendDelay = GetPrivateProfileInt("config", "nSendDelay", 0, filename);
	conn->bSendOnlyChange = GetPrivateProfileInt("config", "bSendOnlyChange", 0, filename);
	conn->nSendBlockSize = GetPrivateProfileInt("config", "nSendBlockSize", 500, filename);

	if(conn->nSendBlockSize < 0)	conn->nSendBlockSize = 1;
	if(conn->nSendBlockSize > 500)	conn->nSendBlockSize = 500;

	// 모뎀 설정 찾기
	GetPrivateProfileString("Modem", "port", "COM1", buf, sizeof(buf), filename);
	conn->modem.cPort = atoi(&buf[3])-1;

	GetPrivateProfileString("Modem", "baud", "19200", buf, sizeof(buf), filename);
	conn->modem.lBaud = atol(buf);

	GetPrivateProfileString("Modem", "parity", "0", buf, sizeof(buf), filename);
	conn->modem.cParity = (char)atol(buf);
	GetPrivateProfileString("Modem", "data", "8", buf, sizeof(buf), filename);
	conn->modem.cData = (char)atol(buf);
	GetPrivateProfileString("Modem", "stop", "1", buf, sizeof(buf), filename);
	conn->modem.cStop = (char)atol(buf);

	GetPrivateProfileString("Modem", "Initial Command", "AT &C1 B0", buf, 20, filename);
	strcpy(conn->sModemInitCommand, buf);

	CString default_port;

	default_port.Format("%d", 6000+connect);

	GetPrivateProfileString("tcp/ip", "port", default_port, buf, sizeof(buf), filename);
	conn->tcpipPort = atoi(buf);
}

void ScanServerListLoad()
{
	int i;

	for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
		LoadConnectListOne(i);
	}
}

void ScanServerListSaveOne(int connect)
{
	char filename[MAXPATH];
	SCAN_SERVER_LIST *conn;
	char buf[80];

	sprintf(filename, "%s\\SCAN\\SERVER", sDirWorkProject);
    MakeDirectory(filename);
	sprintf(filename, "%s\\SCAN\\SERVER\\serv%04d.ini", sDirWorkProject, connect);

	conn = &scanServerList[connect];

	// 접속 방법

	if(conn->nDeviceType == 1)		strcpy(buf, "NULL");
	else if(conn->nDeviceType == 2)	strcpy(buf, "Modem");
	else if(conn->nDeviceType == 3)	strcpy(buf, "TCP/IP");
	else if(conn->nDeviceType == 4)	strcpy(buf, "UDP/IP");
	else							strcpy(buf, "None");

	WritePrivateProfileString("config", "Connect Method", buf, filename);

	sprintf(buf, "%d", conn->bThreadFlag);
	WritePrivateProfileString("config", "Thread", buf, filename);

	WritePrivateProfileInt("config", "nSendDelay", conn->nSendDelay, filename);
	WritePrivateProfileInt("config", "bSendOnlyChange", conn->bSendOnlyChange, filename);
	WritePrivateProfileInt("config", "nSendBlockSize", conn->nSendBlockSize, filename);

	// 모뎀 설정
	sprintf(buf, "COM%d", conn->modem.cPort+1);
	WritePrivateProfileString("Modem", "port",      buf,    filename);
	sprintf(buf, "%lu", conn->modem.lBaud);
	WritePrivateProfileString("Modem", "baud",   buf,   filename);
	sprintf(buf, "%d",  conn->modem.cParity);
	WritePrivateProfileString("Modem", "parity", buf, filename);
	sprintf(buf, "%d",  conn->modem.cData);
	WritePrivateProfileString("Modem", "data",   buf, filename);
	sprintf(buf, "%d",  conn->modem.cStop);
	WritePrivateProfileString("Modem", "stop",   buf, filename);

	WritePrivateProfileString("Modem", "Initial Command", conn->sModemInitCommand, filename);

	sprintf(buf, "%d",  conn->tcpipPort);
	WritePrivateProfileString("tcp/ip", "port", buf, filename);
}

int OpenListenSocket(SCAN_SERVER_LIST *conn);

int ATCommandInit(HGATE hGate, char *command);
int ATCommandSetServer(HGATE hGate);

void ResetTimer(SCAN_SERVER_LIST *conn)
{

}


void ConnectStatusChanged();

void SetConnectFlag(SCAN_SERVER_LIST *conn, char flag)
{
	if(conn->bConnectFlag != flag) {
		conn->bConnectFlag = flag;
		ConnectStatusChanged();
	}
}

int  OpenListenSocket(SCAN_SERVER_LIST *conn);
void CloseListenSocket(SCAN_SERVER_LIST *conn);

static void WriteWORD(SCAN_SERVER_LIST *conn, int port, WORD address, WORD value)
{
	char buf[80];

	sprintf(buf, "Port=%d,Address=%d,Value=%d,", port, address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_WORD, trans, buf, strlen(buf));
	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);
}

static void WriteFloat(SCAN_SERVER_LIST *conn, int port, WORD address, float value)
{
	char buf[80];

	sprintf(buf, "Port=%d,Address=%d,Value=%.5E,", port, address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_FLOAT, trans, buf, strlen(buf));
	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);
}

static void WriteDWORD(SCAN_SERVER_LIST *conn, int port, WORD address, DWORD value)
{
	char buf[80];

	sprintf(buf, "Port=%d,Address=%d,Value=%d,", port, address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_DWORD, trans, buf, strlen(buf));
	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);
}

static void WriteSTRING(SCAN_SERVER_LIST *conn, int port, WORD address, char *value)
{
	CString buf;
	char imsi[80];

	buf.Format("Port=%d,Address=%d,BlockSize=%d,Block=", port, address, strlen(value));

	for(int i = 0; i < (int)strlen(value); i++) {
		sprintf(imsi, "%02X", ((BYTE)value[i]));
		buf += imsi;
	}
	buf += ",";

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_STRING, trans, buf, strlen(buf));
	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);
}

static void WriteDOUBLE(SCAN_SERVER_LIST *conn, int port, WORD address, double value)
{
	char buf[80];

	sprintf(buf, "Port=%d,Address=%d,Value=%.5E,", port, address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_DOUBLE, trans, buf, strlen(buf));
	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);
}

static void WriteINT64(SCAN_SERVER_LIST *conn, int port, WORD address, __int64 value)
{
	char buf[80];

	sprintf(buf, "Port=%d,Address=%d,Value=%I64d,", port, address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_INT64, trans, buf, strlen(buf));
	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);
}


// ACK trans가 일치하면 return 1
static int ScanServerStatusReadCommand(SCAN_SERVER_LIST *conn, WORD compare_trans)
{
	char imsi[10];
	int count;
	TimeOutClass timeout;

	while(1) {
		if(timeout.IsTimeOut(3))               return 0;

		count = GateReadContinue(conn->hGate, imsi, 1);

		if(count == 0)  return 0;

		if(imsi[0] == STX) {
			conn->nRecvHap = 0;
		}

		conn->recvBuf[conn->nRecvHap] = imsi[0];

		conn->nRecvHap += count;

		if(conn->nRecvHap >= MAX_RECV_BUF) {
			conn->nRecvHap = 0;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(imsi[0] == ETX && conn->nRecvHap > 6) {
			NetWorkProtocolRecv recv;

			if(!recv.Split(conn->recvBuf, conn->nRecvHap)) {
				conn->nRecvHap = 0;	
				continue;
			}

			if(recv.wCommand == COMMAND_PLCSCAN_WRITE_BIT) {	// Bit Write
				int AddWaitWriteDigitalOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag);
				AddWaitWriteDigitalOut(recv.nPort, recv.nStation, recv.dwAddress, recv.sExtra1, recv.wExtra2, (WORD)recv.fValue);
				//int AddWaitWriteDigitalOut(int port, int station, WORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag)
			}
			else if(recv.wCommand == COMMAND_PLCSCAN_WRITE_WORD) {	// Word Write
				int AddWaitWriteAnalogOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr, double value);
				AddWaitWriteAnalogOut(recv.nPort, recv.nStation, recv.dwAddress, recv.sExtra1, recv.wExtra2, recv.fValue);
				// int AddWaitWriteDigitalOut(int port, int station, WORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag)
			}
			else if(recv.wCommand == COMMAND_PLCSCAN_PORT_LIFE_SIGNAL) {	// Life Signal
				// 10초마다 한 번씩 들어온다.
				//conn->nConnectPort = recv.nBroadCastPort;
				for(int i = 0; i < 16; i++) {
					conn->wCastPort[i] = 0;
				}
				conn->wCastPort[recv.nBroadCastPort/16] = WORD_MASK[recv.nBroadCastPort%16];

				conn->timeoutDisconnect.Reset();

				conn->nVersionMajor = recv.nVersionMajor;
				conn->nVersionMinor = recv.nVersionMinor;

				SetConnectFlag(conn, ON);
			}
			else if(recv.wCommand == COMMAND_PLCSCAN_PORT_LIFE_SIGNAL_MULTI) {	// Life Signal
				// 10초마다 한 번씩 들어온다.
				// conn->nConnectPort = recv.nBroadCastPort;
				for(int i = 0; i < 16; i++) {
					conn->wCastPort[i] = recv.wBroadCastPorts[i];
				}

				conn->timeoutDisconnect.Reset();
				SetConnectFlag(conn, ON);
			}
			else if(recv.wCommand == COMMAND_PLCSCAN_PORT_DISCONNECT_SIGNAL) {	// Disconnect Signal
				// 저 쪽이 종료할 때 넘어온다.
				SetConnectFlag(conn, OFF);
			}
			else if(recv.wCommand == COMMAND_ACK) {
				if(recv.wTransaction == compare_trans) {
					conn->nRecvHap = 0;
					return 1;
				}
			}
			else {

			}

			conn->nRecvHap = 0;
		}
	}

	return 0;
}

static void ScanServerStatusSendWORD(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;

	if(pt->nBufSizeWORD == 0)	return;

	int old_pos = conn->nReadPosWORD[port];
	int pos;
	
	while(1) {
		conn->nReadPosWORD[port]++;
		conn->nReadPosWORD[port] %= pt->nBufSizeWORD;

		pos = conn->nReadPosWORD[port];

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->bufWORD[pos].flag)	break;
	}

	if(pt->bufWORD[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	WORD value = pt->bufWORD[pos].value;

	WriteWORD(conn, port, pos, value);
}

#define	MAX_BLOCK_SEND_WORD	500

static void SendValueBlockWORD(SCAN_SERVER_LIST *conn, int port, NETWORK_PROTOCOL_BLOCK_WORD *block, int block_count)
{
	StackChar buf(5000);
	char imsi[10];
	BYTE *p = (BYTE*)block;
	int  i;
	int  block_size = block_count*sizeof(NETWORK_PROTOCOL_BLOCK_WORD);
	
	sprintf(buf.data, "Port=%d,BlockSize=%d,Block=", port, block_size);

	for(i = 0; i < block_size; i++) {
		sprintf(imsi, "%02X", p[i]);
		strcat(buf.data, imsi);
	}

	NetWorkProtocolSend send;
	WORD trans = GetTransaction();
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_WORD_BLOCK, trans, buf.data, strlen(buf.data));

	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(ScanServerStatusReadCommand(conn, trans))	break;
	}
}

static void ScanServerStatusSendWORD_Block(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;
	NETWORK_PROTOCOL_BLOCK_WORD *block = NULL;

	if(pt->nBufSizeWORD == 0)	return;

	block = new NETWORK_PROTOCOL_BLOCK_WORD[MAX_BLOCK_SEND_WORD];

	int old_pos = -1;
	int pos;
	int i;
	int count = 0;
	
	for(i = 0; i < pt->nBufSizeWORD; i++) {
		conn->nReadPosWORD[port]++;
		conn->nReadPosWORD[port] %= pt->nBufSizeWORD;

		if(old_pos == -1) {
			old_pos = conn->nReadPosWORD[port];
		} 
		else {
			if(conn->nReadPosWORD[port] == old_pos)	break;	// 한바퀴 돌았다.
		}

		pos = conn->nReadPosWORD[port];
		
		if(pt->bufWORD[pos].flag == 0)	continue;

		block[count].address = pos;
		block[count].value = pt->bufWORD[pos].value;
		count++;
		if(count >= MAX_BLOCK_SEND_WORD || count >= conn->nSendBlockSize)	break;
	}

	if(count > 0) {
		SendValueBlockWORD(conn, port, block, count);
	}

	delete block;
}

static void ScanServerStatusSendFLOAT(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;

	if(pt->nBufSizeFLOAT == 0)	return;

	int old_pos = conn->nReadPosFLOAT[port];
	int pos;
	
	while(1) {
		conn->nReadPosFLOAT[port]++;
		conn->nReadPosFLOAT[port] %= pt->nBufSizeFLOAT;

		pos = conn->nReadPosFLOAT[port];

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->bufFLOAT[pos].flag)	break;
	}

	if(pt->bufFLOAT[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	float value = pt->bufFLOAT[pos].value;

	WriteFloat(conn, port, pos, value);
}

#define	MAX_BLOCK_SEND_FLOAT	333	// 2000/6

static void SendValueBlockFLOAT(SCAN_SERVER_LIST *conn, int port, NETWORK_PROTOCOL_BLOCK_FLOAT *block, int block_count)
{
	StackChar buf(5000);
	char imsi[10];
	BYTE *p = (BYTE*)block;
	int  i;
	int  block_size = block_count*sizeof(NETWORK_PROTOCOL_BLOCK_FLOAT);
	
	sprintf(buf.data, "Port=%d,BlockSize=%d,Block=", port, block_size);

	for(i = 0; i < block_size; i++) {
		sprintf(imsi, "%02X", p[i]);
		strcat(buf.data, imsi);
	}

	NetWorkProtocolSend send;
	WORD trans = GetTransaction();
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_FLOAT_BLOCK, trans, buf.data, strlen(buf.data));

	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(ScanServerStatusReadCommand(conn, trans))	break;
	}
}

static void ScanServerStatusSendFLOAT_Block(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;
	NETWORK_PROTOCOL_BLOCK_FLOAT *block = NULL;

	if(pt->nBufSizeFLOAT == 0)	return;

	block = new NETWORK_PROTOCOL_BLOCK_FLOAT[MAX_BLOCK_SEND_FLOAT];

	int old_pos = -1;
	int pos;
	int i;
	int count = 0;
	
	for(i = 0; i < pt->nBufSizeFLOAT; i++) {
		conn->nReadPosFLOAT[port]++;
		conn->nReadPosFLOAT[port] %= pt->nBufSizeFLOAT;

		if(old_pos == -1) {
			old_pos = conn->nReadPosFLOAT[port];
		} 
		else {
			if(conn->nReadPosFLOAT[port] == old_pos)	break;	// 한바퀴 돌았다.
		}

		pos = conn->nReadPosFLOAT[port];
		
		if(pt->bufFLOAT[pos].flag == 0)	continue;

		block[count].address = pos;
		block[count].value = pt->bufFLOAT[pos].value;
		count++;
		if(count >= MAX_BLOCK_SEND_FLOAT || count >= conn->nSendBlockSize)	break;
	}

	if(count > 0) {
		SendValueBlockFLOAT(conn, port, block, count);
	}

	delete block;
}

static void ScanServerStatusSendDWORD(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;

	if(pt->nBufSizeDWORD == 0)	return;

	int old_pos = conn->nReadPosDWORD[port];
	int pos;
	
	while(1) {
		conn->nReadPosDWORD[port]++;
		conn->nReadPosDWORD[port] %= pt->nBufSizeDWORD;

		pos = conn->nReadPosDWORD[port];

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->bufDWORD[pos].flag)	break;
	}
	
	if(pt->bufDWORD[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	DWORD value = pt->bufDWORD[pos].value;

	WriteDWORD(conn, port, pos, value);
}

#define	MAX_BLOCK_SEND_DWORD	333	// 2000/6

static void SendValueBlockDWORD(SCAN_SERVER_LIST *conn, int port, NETWORK_PROTOCOL_BLOCK_DWORD *block, int block_count)
{
	StackChar buf(5000);
	char imsi[10];
	BYTE *p = (BYTE*)block;
	int  i;
	int  block_size = block_count*sizeof(NETWORK_PROTOCOL_BLOCK_DWORD);
	
	sprintf(buf.data, "Port=%d,BlockSize=%d,Block=", port, block_size);

	for(i = 0; i < block_size; i++) {
		sprintf(imsi, "%02X", p[i]);
		strcat(buf.data, imsi);
	}

	NetWorkProtocolSend send;
	WORD trans = GetTransaction();
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_DWORD_BLOCK, trans, buf.data, strlen(buf.data));

	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(ScanServerStatusReadCommand(conn, trans))	break;
	}
}

static void ScanServerStatusSendDWORD_Block(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;
	NETWORK_PROTOCOL_BLOCK_DWORD *block = NULL;

	if(pt->nBufSizeDWORD == 0)	return;

	block = new NETWORK_PROTOCOL_BLOCK_DWORD[MAX_BLOCK_SEND_DWORD];

	int old_pos = -1;
	int pos;
	int i;
	int count = 0;
	
	for(i = 0; i < pt->nBufSizeDWORD; i++) {
		conn->nReadPosDWORD[port]++;
		conn->nReadPosDWORD[port] %= pt->nBufSizeDWORD;

		if(old_pos == -1) {
			old_pos = conn->nReadPosDWORD[port];
		} 
		else {
			if(conn->nReadPosDWORD[port] == old_pos)	break;	// 한바퀴 돌았다.
		}

		pos = conn->nReadPosDWORD[port];
		
		if(pt->bufDWORD[pos].flag == 0)	continue;

		block[count].address = pos;
		block[count].value = pt->bufDWORD[pos].value;
		count++;
		if(count >= MAX_BLOCK_SEND_DWORD || count >= conn->nSendBlockSize)	break;
	}

	if(count > 0) {
		SendValueBlockDWORD(conn, port, block, count);
	}

	delete block;
}

static void ScanServerStatusSendSTRING(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;

	if(pt->nBufSizeSTRING == 0)	return;

	int old_pos = conn->nReadPosSTRING[port];
	int pos;
	
	while(1) {
		conn->nReadPosSTRING[port]++;
		conn->nReadPosSTRING[port] %= pt->nBufSizeSTRING;

		pos = conn->nReadPosSTRING[port];

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->bufSTRING[pos].flag)	break;
	}
	
	if(pt->bufSTRING[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	
	WriteSTRING(conn, port, pos, pt->bufSTRING[pos].value);
}

#define	MAX_BUF_SIZE_SEND_STRING	4000 // 2009-7-14 10.0.2  이것은 버퍼의 사이즈이다.

static void SendValueBlockSTRING(SCAN_SERVER_LIST *conn, int port, const char *block, int block_count)
{
	StackChar buf(5000);
	BYTE *p = (BYTE*)block;
	int  block_size = strlen(block)/2+2; 
	
	sprintf(buf.data, "Port=%d,BlockSize=%d,Block=%04X%s", port, block_size, block_count, block);

	NetWorkProtocolSend send;
	WORD trans = GetTransaction();
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_STRING_BLOCK, trans, buf.data, strlen(buf.data));

	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(ScanServerStatusReadCommand(conn, trans))	break; 
	}
}

static void ScanServerStatusSendSTRING_Block(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;

	if(pt->nBufSizeSTRING == 0)	return;

	CString block;

	int old_pos = -1;
	int pos;
	int i;
	int count = 0;
	char imsi[10];
	int size;
	
	for(i = 0; i < pt->nBufSizeSTRING; i++) {
		conn->nReadPosSTRING[port]++;
		conn->nReadPosSTRING[port] %= pt->nBufSizeSTRING;

		if(old_pos == -1) {
			old_pos = conn->nReadPosSTRING[port];
		} 
		else {
			if(conn->nReadPosSTRING[port] == old_pos)	break;	// 한바퀴 돌았다.
		}

		pos = conn->nReadPosSTRING[port];
		
		if(pt->bufSTRING[pos].flag == 0)	continue;

		size = strlen(pt->bufSTRING[pos].value);
		sprintf(imsi, "%04X%04X", pos, size);
		block += imsi;
		
		for(int j = 0; j < size; j++) {
			sprintf(imsi, "%02X", (BYTE)(pt->bufSTRING[pos].value[j]));
			block += imsi;
		}
		
		count++;

		if(strlen(block) >= MAX_BUF_SIZE_SEND_STRING-520 || count >= conn->nSendBlockSize)	break;
	}

	if(count > 0) {
		SendValueBlockSTRING(conn, port, block, count);
	}
}

// 8.7 이상은 Double과 Int64를 지원한다.
bool IsSupportDoubleInt64(SCAN_SERVER_LIST *conn)
{
	
	if(conn->nVersionMajor < 8)	return false;
	if(conn->nVersionMajor > 8)	return true;

	if(conn->nVersionMinor < 7)	return false;

	return true;
}

static void ScanServerStatusSendDOUBLE(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;

	if(pt->nBufSizeDOUBLE == 0)	return;

	int old_pos = conn->nReadPosDOUBLE[port];
	int pos;
	
	while(1) {
		conn->nReadPosDOUBLE[port]++;
		conn->nReadPosDOUBLE[port] %= pt->nBufSizeDOUBLE;

		pos = conn->nReadPosDOUBLE[port];

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->bufDOUBLE[pos].flag)	break;
	}

	if(pt->bufDOUBLE[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	double value = pt->bufDOUBLE[pos].value;

	WriteDOUBLE(conn, port, pos, value);
}

#define	MAX_BLOCK_SEND_DOUBLE	200	// 2000/10

static void SendValueBlockDOUBLE(SCAN_SERVER_LIST *conn, int port, NETWORK_PROTOCOL_BLOCK_DOUBLE *block, int block_count)
{
	StackChar buf(5000);
	char imsi[10];
	BYTE *p = (BYTE*)block;
	int  i;
	int  block_size = block_count*sizeof(NETWORK_PROTOCOL_BLOCK_DOUBLE);
	
	sprintf(buf.data, "Port=%d,BlockSize=%d,Block=", port, block_size);

	for(i = 0; i < block_size; i++) {
		sprintf(imsi, "%02X", p[i]);
		strcat(buf.data, imsi);
	}

	NetWorkProtocolSend send;
	WORD trans = GetTransaction();
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_DOUBLE_BLOCK, trans, buf.data, strlen(buf.data));

	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(ScanServerStatusReadCommand(conn, trans))	break;
	}
}

static void ScanServerStatusSendDOUBLE_Block(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;
	NETWORK_PROTOCOL_BLOCK_DOUBLE *block = NULL;

	if(pt->nBufSizeDOUBLE == 0)	return;

	block = new NETWORK_PROTOCOL_BLOCK_DOUBLE[MAX_BLOCK_SEND_DOUBLE];

	int old_pos = -1;
	int pos;
	int i;
	int count = 0;
	
	for(i = 0; i < pt->nBufSizeDOUBLE; i++) {
		conn->nReadPosDOUBLE[port]++;
		conn->nReadPosDOUBLE[port] %= pt->nBufSizeDOUBLE;

		if(old_pos == -1) {
			old_pos = conn->nReadPosDOUBLE[port];
		} 
		else {
			if(conn->nReadPosDOUBLE[port] == old_pos)	break;	// 한바퀴 돌았다.
		}

		pos = conn->nReadPosDOUBLE[port];
		
		if(pt->bufDOUBLE[pos].flag == 0)	continue;

		block[count].address = pos;
		block[count].value = pt->bufDOUBLE[pos].value;
		count++;
		if(count >= MAX_BLOCK_SEND_DOUBLE || count >= conn->nSendBlockSize)	break;
	}

	if(count > 0) {
		SendValueBlockDOUBLE(conn, port, block, count);
	}

	delete block;
}

static void ScanServerStatusSendINT64(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;

	if(pt->nBufSizeINT64 == 0)	return;

	int old_pos = conn->nReadPosINT64[port];
	int pos;
	
	while(1) {
		conn->nReadPosINT64[port]++;
		conn->nReadPosINT64[port] %= pt->nBufSizeINT64;

		pos = conn->nReadPosINT64[port];

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->bufINT64[pos].flag)	break;
	}
	
	if(pt->bufINT64[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	__int64 value = pt->bufINT64[pos].value;

	WriteINT64(conn, port, pos, value);
}

#define	MAX_BLOCK_SEND_INT64	200	// 2000/10

static void SendValueBlockINT64(SCAN_SERVER_LIST *conn, int port, NETWORK_PROTOCOL_BLOCK_INT64 *block, int block_count)
{
	StackChar buf(5000);
	char imsi[10];
	BYTE *p = (BYTE*)block;
	int  i;
	int  block_size = block_count*sizeof(NETWORK_PROTOCOL_BLOCK_INT64);
	
	sprintf(buf.data, "Port=%d,BlockSize=%d,Block=", port, block_size);

	for(i = 0; i < block_size; i++) {
		sprintf(imsi, "%02X", p[i]);
		strcat(buf.data, imsi);
	}

	NetWorkProtocolSend send;
	WORD trans = GetTransaction();
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_INT64_BLOCK, trans, buf.data, strlen(buf.data));

	GateWriteContinue(conn->hGate, send.bufSend, send.nBufCount);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(ScanServerStatusReadCommand(conn, trans))	break;
	}
}

static void ScanServerStatusSendINT64_Block(SCAN_SERVER_LIST *conn, int port)
{
	if(conn->bInitialFlag == OFF)			return;	// 접속 초기화 실패
	if(conn->bConnectFlag == OFF)			return;	// 접속중이 아님

	if(port >= nPortHap)	return;

	LOCAL_PORT_STRUCT *pt = &portBuf[port].local;
	NETWORK_PROTOCOL_BLOCK_INT64 *block = NULL;

	if(pt->nBufSizeINT64 == 0)	return;

	block = new NETWORK_PROTOCOL_BLOCK_INT64[MAX_BLOCK_SEND_INT64];

	int old_pos = -1;
	int pos;
	int i;
	int count = 0;
	
	for(i = 0; i < pt->nBufSizeINT64; i++) {
		conn->nReadPosINT64[port]++;
		conn->nReadPosINT64[port] %= pt->nBufSizeINT64;

		if(old_pos == -1) {
			old_pos = conn->nReadPosINT64[port];
		} 
		else {
			if(conn->nReadPosINT64[port] == old_pos)	break;	// 한바퀴 돌았다.
		}

		pos = conn->nReadPosINT64[port];
		
		if(pt->bufINT64[pos].flag == 0)	continue;

		block[count].address = pos;
		block[count].value = pt->bufINT64[pos].value;
		count++;
		if(count >= MAX_BLOCK_SEND_INT64 || count >= conn->nSendBlockSize)	break;
	}

	if(count > 0) {
		SendValueBlockINT64(conn, port, block, count);
	}

	delete block;
}

void ScanServerInitOnlyDevice(SCAN_SERVER_LIST *conn);
void ScanServerUnInitOnlyDevice(SCAN_SERVER_LIST *conn);

static void ScanServerStatusOne(SCAN_SERVER_LIST *conn)
{
	if(conn->nDeviceType == 0)	return;	// 사용 안함으로 선택되어 있음.
 
	if(conn->bSignal_FD_CLOSE) {
		ScanServerUnInitOnlyDevice(conn);
		ScanServerInitOnlyDevice(conn);
		ConnectStatusChanged();
	}

	if(conn->bInitialFlag == OFF) {
		if(conn->nTryInitCount < 5) {
			if(conn->timeoutTryInit.IsTimeOut(10)) {	// 초기화가 되어 있지 않을 때는 10초에 한번씩 5번까지 시도해서 되지 않으면 포기한다.
				conn->timeoutTryInit.Reset();
				conn->nTryInitCount++;
				ScanServerInitOnlyDevice(conn);
				if(conn->bInitialFlag) {
					conn->nTryInitCount = 0;
				}
				ConnectStatusChanged();
				return;
			}
		}
		return;	// 접속 초기화 실패
	}

	if(conn->bConnectFlag) {	// 접속중일 때
		if(conn->timeoutDisconnect.IsTimeOut(20)) {	// 20초동안 아무신호가 들어오지 않으면 초기화를 한다.
			ScanServerUnInitOnlyDevice(conn);
			Sleep(500);
			ScanServerInitOnlyDevice(conn);
			conn->timeoutDisconnect.Reset();
			ConnectStatusChanged();
			return;
		}
	}

	int i;
	GLOBAL_PORT_STRUCT *global;

	if(conn->bInitialFlag) {

		ScanServerStatusReadCommand(conn, 0);

		if(conn->bConnectFlag) {
						
			if(conn->bThreadFlag == 0) {	// thread가 아닐때만 검사한다.
				if(conn->nSendDelay > 0) {
					if(!conn->timeoutSendDelay.IsTimeOut(conn->nSendDelay))	return;
					conn->timeoutSendDelay.Reset();
				}
			}

			for(i = 0; i < 256; i++) {
				if(!(conn->wCastPort[i/16] & WORD_MASK[i%16]))	continue;
				global = &portBuf[i];
				if(global->bActiveFlag == OFF)	continue;

				if(conn->nSendBlockSize == 1) {
					ScanServerStatusSendWORD(conn, i);
					ScanServerStatusSendFLOAT(conn, i);
					ScanServerStatusSendDWORD(conn, i);
					ScanServerStatusSendSTRING(conn, i);
					if(IsSupportDoubleInt64(conn)) {
						ScanServerStatusSendDOUBLE(conn, i);
						ScanServerStatusSendINT64(conn, i);
					}
				}
				else {
					ScanServerStatusSendWORD_Block(conn, i);
					ScanServerStatusSendFLOAT_Block(conn, i);
					ScanServerStatusSendDWORD_Block(conn, i);
					ScanServerStatusSendSTRING_Block(conn, i);
					if(IsSupportDoubleInt64(conn)) {
						ScanServerStatusSendDOUBLE_Block(conn, i);
						ScanServerStatusSendINT64_Block(conn, i);
					}
				}
			}
		}
	}
}

void ScanServerStatus()
{
	int i;
	SCAN_SERVER_LIST *conn;

	for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
		conn = &scanServerList[i];
		if(conn->bThreadFlag == 0) // thread가 아닐때만 검사한다.
			ScanServerStatusOne(conn);
	}
}

static DWORD WINAPI ServerThreadFunc(LPVOID param) 
{
	SCAN_SERVER_LIST *conn = (SCAN_SERVER_LIST*)param;

	conn->bThreadDo = ON;
	conn->bThreadEnd = OFF;

	while(conn->bThreadDo) {
		if(conn->nSendDelay > 0)	Sleep(conn->nSendDelay);
		else						Sleep(1);

		if(conn->bThreadPause == OFF) {
			ScanServerStatusOne(conn);
		}
		else {
			conn->bThreadPauseACK = ON;
		}
	}

	conn->bThreadEnd = ON;

	return 0; 
}

static void ScanServerThreadInit(SCAN_SERVER_LIST *conn)
{
	if(conn->nDeviceType == 0)		return;
	if(conn->bThreadFlag == OFF)	return;

	conn->hThread = CreateThread(NULL, 0, ServerThreadFunc, conn, 0, &conn->idThread);
}

static void ScanServerThreadUnInit(SCAN_SERVER_LIST *conn)
{
	if(conn->nDeviceType == 0)		return;
	if(conn->bThreadFlag == OFF)	return;
	if(conn->hThread == NULL)		return;

	conn->bThreadDo = OFF;

	TimeOutClass timeout;

	while(conn->bThreadEnd == OFF) {
		Sleep(1);
		if(timeout.IsTimeOut(10))	break;
	}

	conn->hThread = NULL;
	conn->idThread = 0;
}

static void ScanServerInitOnlyDevice(SCAN_SERVER_LIST *conn)
{
	conn->sock_listen  = INVALID_SOCKET;		// listen socket을 초기화 함.
	conn->bConnectFlag = OFF;
	conn->bSignal_FD_CLOSE = false;
	conn->bInitialFlag = OFF;

	if(conn->nDeviceType == 1) {		// NULL modem
		if(ModemInstall(NULL, &conn->modem, FALSE)) {	// message box display off
			conn->hGate = GateRegister(&conn->modem);
			conn->bInitialFlag = ON;
		}
		else {
			conn->bInitialFlag = OFF;
		}
	}
	else if(conn->nDeviceType == 2) {		// Real modem

	}
	else if(conn->nDeviceType == 3) {		// TCP/IP
		OpenListenSocket(conn);
	}
	else if(conn->nDeviceType == 4) {		// UDP/IP
		UDPIP_STRUCT udpip;
		memset(&udpip, 0, sizeof(UDPIP_STRUCT));

		udpip.port = conn->tcpipPort;

		if(UdpipInstall(NULL, &udpip, FALSE)) {	// message box display off
			conn->hGate = GateRegister(&udpip);
			conn->bInitialFlag = ON;
		}
		else {
			conn->bInitialFlag = OFF;
		}
	}
	else;

	if(conn->bInitialFlag) {		// 성공적으로 수행되었으면
		ResetTimer(conn);			// 무 응답 시간을 초기화 한다.
	}
}

void ScanServerInitOneDevice(SCAN_SERVER_LIST *conn)
{
	ScanServerInitOnlyDevice(conn);
	ScanServerThreadInit(conn);
}

void ScanServerInitAllDevice(HWND hwnd)
{
	int i;
	SCAN_SERVER_LIST *conn;

	for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
		conn = &scanServerList[i];

		conn->nReadPosWORD = new WORD[MAX_PORT];
		conn->nReadPosDWORD = new WORD[MAX_PORT];
		conn->nReadPosFLOAT = new WORD[MAX_PORT];
		conn->nReadPosSTRING = new WORD[MAX_PORT];
		conn->nReadPosDOUBLE = new WORD[MAX_PORT];
		conn->nReadPosINT64 = new WORD[MAX_PORT];
		
		for(int j = 0; j < MAX_PORT; j++) {	// 초기화를 해야 한다. 
			conn->nReadPosWORD[j] = 0;
			conn->nReadPosDWORD[j] = 0;
			conn->nReadPosFLOAT[j] = 0;
			conn->nReadPosSTRING[j] = 0;
			conn->nReadPosDOUBLE[j] = 0;
			conn->nReadPosINT64[j] = 0;
		}

		ScanServerInitOneDevice(conn);
	}
}

static void ScanServerUnInitOnlyDevice(SCAN_SERVER_LIST *conn)
{
	SetConnectFlag(conn, OFF);
	conn->nRecvHap = 0;
	ResetTimer(conn);				// 무 응답 시간을 초기화 한다.

	if(conn->hGate) {
		GateUninstall(conn->hGate);
		conn->hGate = NULL;
	}

	CloseListenSocket(conn);
}

void ScanServerUnInitOneDevice(SCAN_SERVER_LIST *conn)
{
	ScanServerThreadUnInit(conn);

	ScanServerUnInitOnlyDevice(conn);
}

void ScanServerUnInitAllDevice()
{
	int i;
	SCAN_SERVER_LIST *conn;

	// 모든 포트를 닫기전에 해당 Thread를 미리 종료 시킨다. 2011-11-21. 속도 증가 효과
	for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
		scanServerList[i].bThreadDo = OFF;
	}

	// 모든 포트를 닫는다.
	for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
		conn = &scanServerList[i];
		ScanServerUnInitOneDevice(conn);

		delete conn->nReadPosWORD;
		delete conn->nReadPosDWORD;
		delete conn->nReadPosFLOAT;
		delete conn->nReadPosSTRING;
		delete conn->nReadPosDOUBLE;
		delete conn->nReadPosINT64;
	}
}

void RestartOnScanServer(int no)
{
	SCAN_SERVER_LIST *conn = &scanServerList[no];
	ScanServerUnInitOneDevice(conn);
	LoadConnectListOne(no);
	ScanServerInitOneDevice(conn);
}

void ScanServerPauseOne(SCAN_SERVER_LIST *conn, char flag)
{
	TimeOutClass timeout;

	if(conn->nDeviceType == 0)	return;
	if(conn->bThreadFlag == 0)	return;
	if(conn->hThread == NULL)	return;

	if(flag) {
		conn->bThreadPauseACK = OFF;
		conn->bThreadPause = ON;
		
		timeout.Reset();
		while(conn->bThreadPauseACK == OFF) {
			Sleep(1);
			if(timeout.IsTimeOut(5))	break;
		}
	}
	else {
		conn->bThreadPause = OFF;
	}
}

void ScanServerPause(char flag)
{
	int i;
	SCAN_SERVER_LIST *conn;
	TimeOutClass timeout;

	// 모든 232 포트를 닫는다.
	for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
		conn = &scanServerList[i];
		ScanServerPauseOne(conn, flag);
	}
}








#include "stdafx.h"

#include <dataswap.h>
#include <totaldef.h>

#include "..\catlib.src\NetworkProtocol.h"

#include "plc_scan.h"
#include "protocol\pro_lib.h"

void ComputerDualStatus(GLOBAL_PORT_STRUCT *pt);

enum {
	SYSTEM_PC_DUAL_CONNECTED  = 0x0040,
	SYSTEM_PC_DUAL_ACTIVE_I   = 0x0041,
	SYSTEM_PC_DUAL_ACTIVE_YOU = 0x0042,
	SYSTEM_PC_DUAL_ERROR_I    = 0x0043,
	SYSTEM_PC_DUAL_ERROR_YOU  = 0x0044,
};

static DWORD WINAPI ProcComputerDualThread(LPVOID arg)
{
	GLOBAL_PORT_STRUCT *pt = (GLOBAL_PORT_STRUCT *)arg;

	pt->pcDualRun.bThreadDo  = ON;
	pt->pcDualRun.bThreadEnd = OFF;

	while(pt->pcDualRun.bThreadDo) {
		Sleep(1);
		ComputerDualStatus(pt);
	}
	pt->pcDualRun.bThreadEnd = ON;

	return 0;
}

void ComputerDualThreadInit(int port)
{
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	if(pt->bActiveFlag == 0)			return;	// 2007.9.19 추가
	if(pt->pcDualFile.bActive == 0)		return;	// 2007.9.19 추가
	if(pt->pcDualFile.bThread == OFF)	return;

	pt->pcDualRun.hThread = CreateThread(NULL, 0, ProcComputerDualThread, pt, 0, &pt->pcDualRun.idThread);
}

void ComputerDualThreadUnInit(int port)
{
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	if(pt->pcDualRun.hThread == NULL)	return;

	pt->pcDualRun.bThreadDo = OFF;

	TimeOutClass timeout;
	while(pt->pcDualRun.bThreadEnd == OFF) {
		Sleep(1);
		if(timeout.IsTimeOut(30))	break;
	}

	pt->pcDualRun.hThread = NULL;
	pt->pcDualRun.idThread = 0;	
}

static void SendLifeSignal(GLOBAL_PORT_STRUCT *pt)
{
	CString buf;
	buf.Format("ServerActive=%d,PSTO=%d", pt->pcDualRun.bActiveI, pt->pcDualRun.bErrorI);  // port no

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PC_DUAL_LIFE_SIGNAL, 0, buf, strlen(buf));
	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);

	//DisplaySendCodeNextLine(pt->no);
}

static void SendDisconnectSignal(GLOBAL_PORT_STRUCT *pt)
{
	CString buf;
	buf.Format("Port=%d,BroadCastPort=%d,", pt->local.no, pt->local.no);  // port no

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PC_DUAL_DISCONNECT_SIGNAL, 0, buf, strlen(buf));
	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);

	//DisplaySendCodeNextLine(pt->no);
}

static void SendIAmActivated(GLOBAL_PORT_STRUCT *pt)
{
	CString buf;
	buf.Format("");  // port no

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PC_DUAL_I_AM_ACTIVATED, 0, NULL, 0);
	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);
}

void ComputerDualDeviceInit(int port)
{
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	if(pt->bActiveFlag == 0)		return;	// 2007.9.19 추가
	if(pt->pcDualFile.bActive == 0)	return;	// not used

	pt->pcDualRun.socket = new CSocket();
	if(pt->pcDualRun.socket == NULL)	return;

	pt->pcDualRun.timeoutLifeSignal = new TimeOutClass();
	pt->pcDualRun.timeoutAnotherLife = new TimeOutClass();
	pt->pcDualRun.timeoutChange = new TimeOutClass();

	if(pt->pcDualRun.socket->Create(pt->pcDualFile.nPort, SOCK_DGRAM)) {
		pt->pcDualRun.bSocketCreate = ON;
		SendLifeSignal(pt);
	}
	else {
		pt->pcDualRun.bSocketCreate = OFF;
	}
}

void ComputerDualDeviceUnInit(int port)
{
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	if(pt->pcDualRun.socket) {
		SendDisconnectSignal(pt);
		pt->pcDualRun.socket->Close();
		delete pt->pcDualRun.socket;
		pt->pcDualRun.socket = NULL;
	}

	if(pt->pcDualRun.timeoutLifeSignal) {
		delete pt->pcDualRun.timeoutLifeSignal;
		pt->pcDualRun.timeoutLifeSignal = NULL;
	}

	if(pt->pcDualRun.timeoutAnotherLife) {
		delete pt->pcDualRun.timeoutAnotherLife;
		pt->pcDualRun.timeoutAnotherLife = NULL;
	}

	if(pt->pcDualRun.timeoutChange) {
		delete pt->pcDualRun.timeoutChange;
		pt->pcDualRun.timeoutChange = NULL;
	}
}

static void CheckComputerDualLifeSignal(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->pcDualRun.timeoutLifeSignal) {
		if(pt->pcDualRun.timeoutLifeSignal->IsTimeOut(2)) {
			pt->pcDualRun.timeoutLifeSignal->Reset();
			SendLifeSignal(pt);
		}
	}
}

static void SetConnect(GLOBAL_PORT_STRUCT *pt, char flag)
{
	pt->pcDualRun.bConnect = flag;
	PokeBitSYSTEM(pt, SYSTEM_PC_DUAL_CONNECTED, flag);
}

static void SetActiveI(GLOBAL_PORT_STRUCT *pt, char flag)
{
	pt->pcDualRun.bActiveI = flag;
	PokeBitSYSTEM(pt, SYSTEM_PC_DUAL_ACTIVE_I, flag);
}

static void SetActiveYou(GLOBAL_PORT_STRUCT *pt, char flag)
{
	pt->pcDualRun.bActiveYou = flag;
	PokeBitSYSTEM(pt, SYSTEM_PC_DUAL_ACTIVE_YOU, flag);
}

static void SetErrorI(GLOBAL_PORT_STRUCT *pt, char flag)
{
	pt->pcDualRun.bErrorI = flag;
	PokeBitSYSTEM(pt, SYSTEM_PC_DUAL_ERROR_I, flag);
}

static void SetErrorYou(GLOBAL_PORT_STRUCT *pt, char flag)
{
	pt->pcDualRun.bErrorYou = flag;
	PokeBitSYSTEM(pt, SYSTEM_PC_DUAL_ERROR_YOU, flag);
}

static void CheckComputerDualAnotherLifeSignal(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->pcDualRun.timeoutAnotherLife) {
		if(pt->pcDualRun.timeoutAnotherLife->IsTimeOut(10)) {
			SetConnect(pt, OFF);
		}
	}
}

void SendCodeACK(GLOBAL_PORT_STRUCT *pt, WORD trans)
{
	NetWorkProtocolSend send;

	send.MakeBlock(COMMAND_ACK, trans, NULL, 0);

	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);
	//DisplaySendCodeNextLine(pt->no);
}

static void PokeValueBlockWORD(GLOBAL_PORT_STRUCT *pt, NetWorkProtocolRecv *split)
{
	int j;
	int block_count;

	if(split->nBlockSize == 0)	return;

	block_count = split->nBlockSize/sizeof(NETWORK_PROTOCOL_BLOCK_WORD);

	// if not 0 struct changed
	if((split->nBlockSize%sizeof(NETWORK_PROTOCOL_BLOCK_WORD)) != 0)	return;

	if(block_count == 0)	return;

	NETWORK_PROTOCOL_BLOCK_WORD *block = (NETWORK_PROTOCOL_BLOCK_WORD*)split->Block;

	if(block == NULL) return;

	for(j = 0; j < block_count; j++) {
		PokeWORD(&pt->local, block[j].address,  block[j].value);
	}
}

int NetCheckRecvCommandOne(GLOBAL_PORT_STRUCT *pt, CSocket *socket, WORD &recv_trans, char *recv_data, int retn_size)
{
	NetWorkProtocolRecv split;

	if(split.Split(recv_data, retn_size)) {
		if(split.wCommand == COMMAND_PC_DUAL_LIFE_SIGNAL) {
			SetConnect(pt, ON);
			pt->pcDualRun.timeoutAnotherLife->Reset();
			SetActiveYou(pt, split.bServerActive);
			SetErrorYou( pt, split.bPlcScanTimeOut);
		}
		else if(split.wCommand == COMMAND_PC_DUAL_DISCONNECT_SIGNAL) {
			SetConnect(pt, OFF);
		}
		else if(split.wCommand == COMMAND_PC_DUAL_I_AM_ACTIVATED) {
			SetActiveI(pt, OFF);
			SetActiveYou(pt, ON);
		}
		else if(split.wCommand == COMMAND_PLCSCAN_VALUE_WORD_BLOCK) {
			SendCodeACK(pt, split.wTransaction);	// 코드를 먼저 보내는 것이 서버의 부하를 줄이는데 유리하다.
			PokeValueBlockWORD(pt, &split);
		}
		else if(split.wCommand == COMMAND_PLCSCAN_VALUE_DWORD) {	// DWORD
			PokeDWORD(&pt->local, (WORD)split.dwAddress, (DWORD)split.fValue);
		}
		else if(split.wCommand == COMMAND_PLCSCAN_VALUE_FLOAT) {	// Float
			PokeFLOAT(&pt->local, (WORD)split.dwAddress, (float)split.fValue);
		}
		else if(split.wCommand == COMMAND_PLCSCAN_VALUE_STRING) {	// String
			char buf[256];
			if(split.nBlockSize < 256) {
				memcpy(buf, split.Block, split.nBlockSize);
				buf[split.nBlockSize] = 0;
				PokeSTRING(&pt->local, (WORD)split.dwAddress, buf);
			}
		}
		else if(split.wCommand == COMMAND_PLCSCAN_VALUE_DOUBLE) {	// 
			PokeDOUBLE(&pt->local, (WORD)split.dwAddress, split.fValue);
		}
		else if(split.wCommand == COMMAND_PLCSCAN_VALUE_INT64) {	// 
			PokeINT64(&pt->local, (WORD)split.dwAddress, split.i64Value);
		}
		else if(split.wCommand == COMMAND_ACK) {
			if(split.wTransaction == recv_trans) {
				return 1;
			}
		}
		else if(split.wCommand == COMMAND_PLCSCAN_WRITE_BIT) {	// Bit Write
			SendCodeACK(pt, split.wTransaction);	// 코드를 먼저 보내는 것이 서버의 부하를 줄이는데 유리하다.
			int AddWaitWriteDigitalOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag);
			AddWaitWriteDigitalOut(pt->local.no, split.nStation, split.dwAddress, split.sExtra1, split.wExtra2, (WORD)split.fValue);
		}
		else if(split.wCommand == COMMAND_PLCSCAN_WRITE_WORD) {	// Word Write
			SendCodeACK(pt, split.wTransaction);	// 코드를 먼저 보내는 것이 서버의 부하를 줄이는데 유리하다.
			int AddWaitWriteAnalogOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr, double value);
			AddWaitWriteAnalogOut(pt->local.no, split.nStation, split.dwAddress, split.sExtra1, split.wExtra2, split.fValue);
		}
		else;
	}

	return 0;
}

int NetCheckRecvCommand(GLOBAL_PORT_STRUCT *pt, CSocket *socket, WORD &recv_trans)
{
	CheckComputerDualLifeSignal(pt);	// 여기에서 라이프 시그널을 보내는 이유는 클라이언트에 데이터를 보내고
										// ACK를 기다릴 때 이 함수를 불러주기 때문이다.
										// 클라이언트가 응답이 길어지면 상대서버에서 이 서버의 상태를 인식못하기 때문에 이 부분인 필요하다.
	
	DWORD remain = 0;

	if(socket->IOCtl(FIONREAD, &remain) == 0)	return 0;

	if(remain == 0)	return 0;
	if(remain <  1)	return 0;

	CString ip;
	UINT port;
	
	StackChar recv(remain);
	int retn_size;

	retn_size = socket->ReceiveFrom(recv.data, remain, ip, port);
	if(retn_size == SOCKET_ERROR) {
		return 0;
	}

	//void DisplayRecvCode(const char *ip, const char *buf, int size);
	//DisplayRecvCode(ip, recv.data, retn_size);

	int start_pos = 0;
	int retn = 0;

	for(int i = 0; i < retn_size; i++) {
		if(recv.data[i] == ETX) {
			if(NetCheckRecvCommandOne(pt, socket, recv_trans, &recv.data[start_pos], i-start_pos+1)) {
				retn = 1;
			}
			start_pos = i+1;
		}
	}

	return retn;
}

static void CheckActive(GLOBAL_PORT_STRUCT *pt)
{
	// 둘다 활성화 되었거나 비활성화 되었을 때 한쪽만 활성화 시켜야 한다.
	if( (pt->pcDualRun.bActiveI == ON && pt->pcDualRun.bActiveYou == ON) || (pt->pcDualRun.bActiveI == OFF && pt->pcDualRun.bActiveYou == OFF) ) {
		if(pt->pcDualRun.bChangeStart == OFF) {
			pt->pcDualRun.bChangeStart = ON;
			pt->pcDualRun.timeoutChange->Reset();
		}
		else {
			if(pt->pcDualRun.timeoutChange->IsTimeOut(3)) {
				pt->pcDualRun.bChangeStart = OFF;
				SetActiveI(pt, ON);
				SetActiveYou(pt, OFF);
				SendIAmActivated(pt);
			}
		}
	}
	else if(pt->pcDualRun.bConnect == OFF && pt->pcDualRun.bActiveI == OFF) {
		if(pt->pcDualRun.bChangeStart == OFF) {
			pt->pcDualRun.bChangeStart = ON;
			pt->pcDualRun.timeoutChange->Reset();
		}
		else {
			if(pt->pcDualRun.timeoutChange->IsTimeOut(3)) {
				pt->pcDualRun.bChangeStart = OFF;
				SetActiveI(pt, ON);
				SetActiveYou(pt, OFF);
				SendIAmActivated(pt);
			}
		}
	}
	else {
		//pt->pcDualRun.bChangeStart = OFF;
	}

}

static void WriteFloat(GLOBAL_PORT_STRUCT *pt, WORD address, float value)
{
	char buf[80];

	sprintf(buf, "Address=%d,Value=%.5E,", address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_FLOAT, trans, buf, strlen(buf));
	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);
}

static void WriteDWORD(GLOBAL_PORT_STRUCT *pt, WORD address, DWORD value)
{
	char buf[80];

	sprintf(buf, "Address=%d,Value=%d,", address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_DWORD, trans, buf, strlen(buf));
	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);
}

static void WriteSTRING(GLOBAL_PORT_STRUCT *pt, WORD address, char *value)
{
	CString buf;
	char imsi[80];

	buf.Format("Address=%d,BlockSize=%d,Block=", address, strlen(value));

	for(int i = 0; i < (int)strlen(value); i++) {
		sprintf(imsi, "%02X", (unsigned char)value[i]);
		buf += imsi;
	}
	buf += ",";

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_STRING, trans, buf, strlen(buf));
	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);
}

static void WriteDOUBLE(GLOBAL_PORT_STRUCT *pt, WORD address, double value)
{
	char buf[80];

	sprintf(buf, "Address=%d,Value=%.5E,", address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_DOUBLE, trans, buf, strlen(buf));
	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);
}

static void WriteINT64(GLOBAL_PORT_STRUCT *pt, WORD address, __int64 value)
{
	char buf[80];

	sprintf(buf, "Address=%d,Value=%I64d,", address, value);

	WORD trans = GetTransaction();

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_INT64, trans, buf, strlen(buf));
	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);
}

#define	MAX_BLOCK_SEND_WORD	500

static void SendValueBlockWORD(GLOBAL_PORT_STRUCT *pt, NETWORK_PROTOCOL_BLOCK_WORD *block, int block_count)
{
	StackChar buf(5000);
	char imsi[10];
	BYTE *p = (BYTE*)block;
	int  i;
	int  block_size = block_count*sizeof(NETWORK_PROTOCOL_BLOCK_WORD);
	
	sprintf(buf.data, "BlockSize=%d,Block=", block_size);

	for(i = 0; i < block_size; i++) {
		sprintf(imsi, "%02X", p[i]);
		strcat(buf.data, imsi);
	}

	NetWorkProtocolSend send;
	WORD trans = GetTransaction();
	send.MakeBlock(COMMAND_PLCSCAN_VALUE_WORD_BLOCK, trans, buf.data, strlen(buf.data));

	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(NetCheckRecvCommand(pt, pt->pcDualRun.socket, trans))	break;
	}
}

static void PcDualStatusSendWORD_Block(GLOBAL_PORT_STRUCT *pt)
{
	NETWORK_PROTOCOL_BLOCK_WORD *block = NULL;

	if(pt->local.nBufSizeWORD == 0)	return;

	block = new NETWORK_PROTOCOL_BLOCK_WORD[MAX_BLOCK_SEND_WORD];

	int old_pos = -1;
	int pos;
	int i;
	int count = 0;
	
	for(i = 0; i < pt->local.nBufSizeWORD; i++) {
		pt->pcDualRun.nReadPosWORD++;
		pt->pcDualRun.nReadPosWORD %= pt->local.nBufSizeWORD;

		if(old_pos == -1) {
			old_pos = pt->pcDualRun.nReadPosWORD;
		} 
		else {
			if(pt->pcDualRun.nReadPosWORD == old_pos)	break;	// 한바퀴 돌았다.
		}

		pos = pt->pcDualRun.nReadPosWORD;
		
		if(pt->local.bufWORD[pos].flag == 0)	continue;

		block[count].address = pos;
		block[count].value = pt->local.bufWORD[pos].value;
		count++;
		if(count >= MAX_BLOCK_SEND_WORD)	break;
	}

	if(count > 0) {
		SendValueBlockWORD(pt, block, count);
	}

	delete block;
}

static void PcDualStatusSendFLOAT(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->local.nBufSizeFLOAT == 0)	return;

	int old_pos = pt->pcDualRun.nReadPosFLOAT;
	int pos;
	
	while(1) {
		pt->pcDualRun.nReadPosFLOAT++;
		pt->pcDualRun.nReadPosFLOAT %= pt->local.nBufSizeFLOAT;

		pos = pt->pcDualRun.nReadPosFLOAT;

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->local.bufFLOAT[pos].flag)	break;
	}

	if(pt->local.bufFLOAT[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	float value = pt->local.bufFLOAT[pos].value;

	WriteFloat(pt, pos, value);
}

static void PcDualStatusSendDWORD(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->local.nBufSizeDWORD == 0)	return;

	int old_pos = pt->pcDualRun.nReadPosDWORD;
	int pos;
	
	while(1) {
		pt->pcDualRun.nReadPosDWORD++;
		pt->pcDualRun.nReadPosDWORD %= pt->local.nBufSizeDWORD;

		pos = pt->pcDualRun.nReadPosDWORD;

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->local.bufDWORD[pos].flag)	break;
	}
	
	if(pt->local.bufDWORD[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	DWORD value = pt->local.bufDWORD[pos].value;

	WriteDWORD(pt, pos, value);
}

static void PcDualStatusSendSTRING(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->local.nBufSizeSTRING == 0)	return;

	int old_pos = pt->pcDualRun.nReadPosSTRING;
	int pos;
	
	while(1) {
		pt->pcDualRun.nReadPosSTRING++;
		pt->pcDualRun.nReadPosSTRING %= pt->local.nBufSizeSTRING;

		pos = pt->pcDualRun.nReadPosSTRING;

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->local.bufSTRING[pos].flag)	break;
	}
	
	if(pt->local.bufSTRING[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:

	WriteSTRING(pt, pos, pt->local.bufSTRING[pos].value);
}

static void PcDualStatusSendDOUBLE(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->local.nBufSizeDOUBLE == 0)	return;

	int old_pos = pt->pcDualRun.nReadPosDOUBLE;
	int pos;
	
	while(1) {
		pt->pcDualRun.nReadPosDOUBLE++;
		pt->pcDualRun.nReadPosDOUBLE %= pt->local.nBufSizeDOUBLE;

		pos = pt->pcDualRun.nReadPosDOUBLE;

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->local.bufDOUBLE[pos].flag)	break;
	}

	if(pt->local.bufDOUBLE[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	double value = pt->local.bufDOUBLE[pos].value;

	WriteDOUBLE(pt, pos, value);
}

static void PcDualStatusSendINT64(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->local.nBufSizeINT64 == 0)	return;

	int old_pos = pt->pcDualRun.nReadPosINT64;
	int pos;
	
	while(1) {
		pt->pcDualRun.nReadPosINT64++;
		pt->pcDualRun.nReadPosINT64 %= pt->local.nBufSizeINT64;

		pos = pt->pcDualRun.nReadPosINT64;

		if(pos == old_pos)			goto write_evenif_flag_is_off;
		if(pt->local.bufINT64[pos].flag)	break;
	}
	
	if(pt->local.bufINT64[pos].flag == OFF) {
		return;	// 값이 읽혀지지 않았다.
	}

write_evenif_flag_is_off:
	__int64 value = pt->local.bufINT64[pos].value;

	WriteINT64(pt, pos, value);
}

static void CheckChangeCondition(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->pcDualFile.nTimeOut > 0)	{
		if(pt->nScanDevice == 0) {
			pt->nCountContinueTimeOut = 0;
		}

		if(pt->nCountContinueTimeOut >= pt->pcDualFile.nTimeOut) 
			SetErrorI(pt, ON);
		else
			SetErrorI(pt, OFF);
	}

	if(!pt->pcDualRun.bConnect)	  return; // 연결이 안되면 체크할 필요가 없다.
	if(!pt->pcDualRun.bActiveYou) return; // 상대 서버가 활성화 되어 있지 않으면 체크할 필요가 없다.

	if(!pt->pcDualRun.bErrorI && pt->pcDualRun.bErrorYou) {
		if(pt->pcDualRun.bChangeStart == OFF) {
			pt->pcDualRun.bChangeStart = ON;
			pt->pcDualRun.timeoutChange->Reset();
		}
		else {
			if(pt->pcDualRun.timeoutChange->IsTimeOut(3)) {
				pt->pcDualRun.bChangeStart = OFF;
				SetActiveI(pt, ON);
				SetActiveYou(pt, OFF);
				SendIAmActivated(pt);
				pt->nCountContinueTimeOut = 0;
			}
		}
	}
}

static int PcDualSendBitWrite(GLOBAL_PORT_STRUCT *pt, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	char imsi[80];
	StackChar data(1000);
	WORD trans = GetTransaction();

	sprintf(imsi, "Port=%d,", pt->local.no);
	strcpy(data.data, imsi);
	sprintf(imsi, "Station=%d,", item->station);
	strcat(data.data, imsi);
	sprintf(imsi, "Address=%d,", item->address);
	strcat(data.data, imsi);
	sprintf(imsi, "Value=%d,", (int)item->value);
	strcat(data.data, imsi);
	sprintf(imsi, "Extra1=%s,", item->sExtraAddr);
	strcat(data.data, imsi);
	sprintf(imsi, "Extra2=%d,", item->wExtraAddr);
	strcat(data.data, imsi);

	NetWorkProtocolSend send;

	send.MakeBlock(COMMAND_PLCSCAN_WRITE_BIT, trans, data.data, strlen(data.data));

	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(NetCheckRecvCommand(pt, pt->pcDualRun.socket, trans))	return 1;
	}

	return 0;
}

static int PcDualSendWordWrite(GLOBAL_PORT_STRUCT *pt, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	char imsi[80];
	StackChar data(1000);
	WORD trans = GetTransaction();

	sprintf(imsi, "Port=%d,", pt->local.no);
	strcpy(data.data, imsi);
	sprintf(imsi, "Station=%d,", item->station);
	strcat(data.data, imsi);
	sprintf(imsi, "Address=%d,", item->address);
	strcat(data.data, imsi);
	sprintf(imsi, "Value=%f,", item->value);
	strcat(data.data, imsi);
	sprintf(imsi, "Extra1=%s,", item->sExtraAddr);
	strcat(data.data, imsi);
	sprintf(imsi, "Extra2=%d,", item->wExtraAddr);
	strcat(data.data, imsi);

	NetWorkProtocolSend send;

	send.MakeBlock(COMMAND_PLCSCAN_WRITE_WORD, trans, data.data, strlen(data.data));

	pt->pcDualRun.socket->SendTo(send.bufSend, send.nBufCount, pt->pcDualFile.nPort, pt->pcDualFile.sIP);

	TimeOutClass timeout;

	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
		if(NetCheckRecvCommand(pt, pt->pcDualRun.socket, trans))	return 1;
	}

	return 0;
}

//-----------------------------------------------------------------------------------
//	대기중인 쓰기 명령이 있으면 쓰기를 한다.
//-----------------------------------------------------------------------------------

//void SetWriteBlockRun(int port);
//void ResetWriteBlockRun(int port);

void RunWriteWaitByPcDual(GLOBAL_PORT_STRUCT *pt)
{
	SCAN_WRITE_EXCHANGE_ITEM *wait;
	//DWORD l;
	int retn;
	static int retry_count_on_timeout = 0;
	
	if(pt->blockWriteWait->ring_current == pt->blockWriteWait->ring_target) {
		retry_count_on_timeout = 0;
		return;
	}

	int next_pos;
	
	for(int i = 0; i < MAX_SCAN_WRITE_LOCAL_ITEM_COUNT; i++) {
		if(pt->blockWriteWait->ring_current == pt->blockWriteWait->ring_target) break;

		next_pos = (pt->blockWriteWait->ring_current+1)%MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;

		wait = &pt->blockWriteWait->item[next_pos];

		//ResetWriteBlockRun(pt->local.no);

		if(wait->command == 0) {	// bit write
			retn = PcDualSendBitWrite(pt, wait);
		}
		else {							// word write
			retn = PcDualSendWordWrite(pt, wait);
		}

		if(retn == COMMUNICATION_TIME_OUT) {
			if(retry_count_on_timeout < config.nRetryCountOnWriteTimeOut) {
				retry_count_on_timeout++;
				return;
			}
		}

		retry_count_on_timeout = 0;

		//SetWriteBlockRun(pt->local.no);
		pt->blockWriteWait->ring_current = next_pos;
		//SetWriteWaitCount(pt);
		//ResetWriteBlockRun(pt->local.no);
		return;	// 하나를 썼으면 돌아간다.
				// 계속 하는 경우는 NEXT_WRITE_GO(-1)를 실행할 때이다.
	}


}

void ComputerDualStatus(GLOBAL_PORT_STRUCT *pt)
{
	if(!pt->pcDualFile.bActive)				return;	// 기능 사용 안함

	if(pt->pcDualRun.bSocketCreate == 0)	return;	// socket fail

	CheckComputerDualLifeSignal(pt);
	CheckComputerDualAnotherLifeSignal(pt);
	CheckActive(pt);
	CheckChangeCondition(pt);

	WORD recv_trans = 0;

	NetCheckRecvCommand(pt, pt->pcDualRun.socket, recv_trans);

	if(pt->pcDualRun.bConnect && pt->pcDualRun.bActiveI) {
		PcDualStatusSendWORD_Block(pt);
		PcDualStatusSendFLOAT(pt);
		PcDualStatusSendDWORD(pt);
		PcDualStatusSendSTRING(pt);
		PcDualStatusSendDOUBLE(pt);
		PcDualStatusSendINT64(pt);
	}

	// 연결되어 있고 저쪽이 활성화 되어 있으면 명령을 보내준다.
	if(pt->pcDualRun.bConnect && pt->pcDualRun.bActiveYou) {
		RunWriteWaitByPcDual(pt);
	}
}


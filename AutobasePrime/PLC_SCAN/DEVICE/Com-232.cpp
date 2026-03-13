// english O.K
//------------------------------------------------------------------------------
//	컴퓨터 자체에 내장되어 있는 rs-232 의 통신을 담당한다.
//
//	프로그램 유의사항.
//	NT에서는 OVERLAPP을 사용하면 통신이 잘되지 않아서 OVERLAPP을 뺐다.
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <conio.h> 
#include <string.h>
#include <dos.h>
#include <time.h>

#include <tools.h>
#include <dataswap.h>

#include "commmain.h"
 
static char bReadingOrWriting = 0;


void SetReadingOrWriting(char flag)
{
	bReadingOrWriting = flag;
}

void ShowError232(HWND hwnd, int err, char *message, DEVICE_STRUCT_RS232 *rs232)
{
	StackChar buf(1000);
	char		 imsi[80];
	GetLastErrorClass error;

	sprintf(buf.data, "Port=%d, %s, Device:COM%d\n", rs232->port_no, message, rs232->port+1);

	switch(err) {
		case IE_BADID:	    strcat(buf.data, "IE_BADID\nThe device identifier is invalid or unsupported");
							break;
		case IE_BAUDRATE:
							strcat(buf.data, "IE_BAUDRATE\nThe device's baud is unsupported");
							break;
		case IE_BYTESIZE:
							strcat(buf.data, "IE_BYTESIZE\nThe specified byte size if invalid");
							break;
		case IE_DEFAULT:
							strcat(buf.data, "IE_DEFAULT\nthe default parameters are in error");
							break;
		case IE_HARDWARE:
							strcat(buf.data, "IE_HARDWARE\nthe hardware is not available (is locked by another device).");
							break;
		case IE_MEMORY:
							strcat(buf.data, "IE_MEMORY\nthe function cannot allocate the queues.");
							break;
		case IE_NOPEN:		strcat(buf.data, "IE_NOPEN\nthe device is not open");
							break;
		case IE_OPEN:		strcat(buf.data, "IE_OPEN\nthe device is already open");
							break;
		case ERROR_ACCESS_DENIED:
							strcat(buf.data, "Access Denied(Check used by another program)");
							break;
		default:			strcat(buf.data, error.GetString());
							break;
	}

	sprintf(imsi, "\nErrorCode-%d", err);
	strcat(buf.data, imsi);

	MessageDisplay(buf.data);
}

BOOL NEAR SetupConnection( HWND hwnd, DEVICE_STRUCT_RS232 *rs232)
{
	BOOL       fRetVal;
	BYTE       bSet;
	DCB        dcb;

	dcb.DCBlength = sizeof( DCB ) ;

	GetCommState(rs232->id, &dcb ) ;

	dcb.BaudRate = rs232->baud;
	dcb.ByteSize = rs232->data;
	dcb.Parity =   rs232->parity;
	if(rs232->stop == 1)			// 1 = 1.5
		dcb.StopBits = 0;
	else
		dcb.StopBits = 2;

	// setup hardware flow control

	//bSet = (BYTE) ((FLOWCTRL( npTTYInfo ) & FC_DTRDSR) != 0) ;
	bSet = 0;
	dcb.fOutxDsrFlow = bSet;

	//if(rs232->nRtsMethod) 
	//	dcb.fDtrControl = DTR_CONTROL_DISABLE;
	//else
		dcb.fDtrControl = DTR_CONTROL_ENABLE;

	//bSet = (BYTE) ((FLOWCTRL( npTTYInfo ) & FC_RTSCTS) != 0);
	bSet = 0;
	dcb.fOutxCtsFlow = bSet;

	if(rs232->nRtsMethod) 
		dcb.fRtsControl = RTS_CONTROL_TOGGLE;
	else
		dcb.fRtsControl = RTS_CONTROL_ENABLE;
	//dcb.fRtsControl = RTS_CONTROL_HANDSHAKE;

	// setup software flow control

	//bSet = (BYTE) ((FLOWCTRL( npTTYInfo ) & FC_XONXOFF) != 0) ;

	bSet = 0;
	dcb.fInX = dcb.fOutX = bSet ;
	dcb.XonChar  = 0;
	dcb.XoffChar = 0;
	dcb.XonLim  = 100;
	dcb.XoffLim = 100;

	// other various settings

	dcb.fBinary = TRUE;
	dcb.fParity = TRUE;

	fRetVal = SetCommState(rs232->id, &dcb ) ;

	return ( fRetVal ) ;

} // end of SetupConnection()


int PlcDeviceSetParityRS232( DEVICE_STRUCT_RS232 *rs232, char parity)
{
   BOOL       fRetVal;
   DCB        dcb;

   GetCommState(rs232->id, &dcb ) ;
   dcb.Parity = parity;
   fRetVal = SetCommState(rs232->id, &dcb ) ;

   return ( fRetVal ) ;
}

int PlcDeviceSetCommStateRS232(DEVICE_STRUCT_RS232 *rs232, DWORD baud, char parity, char data, char stop)
{
   BOOL       fRetVal; 
   DCB        dcb;

   GetCommState(rs232->id, &dcb ) ;
   dcb.BaudRate = baud;
   dcb.ByteSize = data;
   dcb.Parity =   parity;
   if(stop == 1)			// 1 = 1.5
      dcb.StopBits = 0;
   else
	  dcb.StopBits = 2;

   fRetVal = SetCommState(rs232->id, &dcb ) ;

   return ( fRetVal ) ;
}

void FillStruct232(DEVICE_STRUCT_RS232 *rs232, CommaBlockString *comma)
{
	char argument[80];
	
	comma->GetLong(rs232->baud);
	comma->GetInt(rs232->parity);
	comma->GetInt(rs232->data);
	comma->GetInt(rs232->stop);

	comma->GetString(argument, sizeof(argument));

	if(strcmp(argument, "TxRTS") == 0) {
		rs232->txMethod = TX_METHOD_RTS;
	}
	else if(strcmp(argument, "TxDTR") == 0) {
		rs232->txMethod = TX_METHOD_DTR;
	}
	else {
		rs232->txMethod = TX_METHOD_ON;
	}

	comma->GetString(argument, sizeof(argument));

	if(strcmp(argument, "RxECHO") == 0) {
		rs232->rxMethod = RX_METHOD_ECHO;
	}
	else {
		rs232->rxMethod = RX_METHOD_ON;
	}

	comma->GetInt(rs232->nEndDelayReadRTS);
	comma->GetString(argument, sizeof(argument));
	if(strlen(argument) == 0) {	// write delay가 없으면 read와 일치
		rs232->nEndDelayWriteRTS = rs232->nEndDelayReadRTS;
	}
	else {
		rs232->nEndDelayWriteRTS = atoi(argument);
	}

	comma->GetInt(rs232->nStartDelayReadRTS);
	comma->GetString(argument, sizeof(argument));
	if(strlen(argument) == 0) {	// write delay가 없으면 read와 일치
		rs232->nStartDelayWriteRTS = rs232->nStartDelayReadRTS;
	}
	else {
		rs232->nStartDelayWriteRTS = atoi(argument);
	}

	comma->GetInt(rs232->nRtsMethod);

	//ComPort = string[3]-'1', Baud=arg1, Parity=arg2, Data=arg3, Stop=arg4
}

int PlcDeviceOpenRS232(HWND hwnd, DEVICE_STRUCT_RS232 *rs232)
{
	char buf[80];
	COMMTIMEOUTS CommTimeOuts;

	//sprintf(buf, "COM%d", rs232->port+1);
	sprintf(buf, "\\\\.\\COM%d", rs232->port+1);

	rs232->id = CreateFile(buf, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0/*FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED*/, NULL); 

	if(rs232->id == INVALID_HANDLE_VALUE) {
		ShowError232(hwnd, GetLastError(), "OpenComm", rs232);
		return 0;
	}

	// get any early notifications

	SetCommMask(rs232->id, EV_TXEMPTY);

	// setup device buffers

	if(SetupComm(rs232->id, 4096, 4096 ) == FALSE) {
	//if(SetupComm(rs232->id, 4096, 0) == FALSE) {
		ShowError232(hwnd, GetLastError(), "SetupComm", rs232);
		return 0;	
	}

	// purge any information in the buffer

	if(PurgeComm(rs232->id, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR ) == FALSE) {
		ShowError232(hwnd, GetLastError(), "PergeComm", rs232);
		return 0;	
	}

	// set up for overlapped I/O
	  
	CommTimeOuts.ReadIntervalTimeout = 0xFFFFFFFF ;
	CommTimeOuts.ReadTotalTimeoutMultiplier = 0 ;
	CommTimeOuts.ReadTotalTimeoutConstant = 1000 ;
	CommTimeOuts.WriteTotalTimeoutMultiplier = 0 ;
	CommTimeOuts.WriteTotalTimeoutConstant = 2000 ;
	SetCommTimeouts(rs232->id, &CommTimeOuts ) ;
	
	SetupConnection(hwnd, rs232);
	
    //rs232->nEchoRemain = 0;
	//EscapeCommFunction(rs232->id, SETDTR ) ;

	return 1;
}

int PlcDeviceInitRS232(HWND hwnd, DEVICE_STRUCT_RS232 *rs232, CommaBlockString *comma)
{
	FillStruct232(rs232, comma);	// 스트링을 해석한다.

	return PlcDeviceOpenRS232(hwnd, rs232);	// ON - message box display
}

static int PlcDeviceReadContinueRS232_Crypto(DEVICE_STRUCT_RS232 *rs232, char *buf, int count)
{
	int retn = rs232->pCC->GetDecryptionData((BYTE*)buf, count);
	if(retn > 0) return retn;	// 남아있는 Decoding 이 있으면 그것을 돌려준다.

	DWORD readed = 0;
	COMSTAT comStat;
	DWORD code = 0;

	retn = ClearCommError(rs232->id, &code, &comStat);

	if(retn == 0 || code != 0) {
		int error = 0;
	}

	int remain = comStat.cbInQue;
	if(remain == 0)	return 0;	// 읽어놓은 것이 없다.
	
	char *data_enc = new char[remain];

	if(ReadFile(rs232->id, data_enc, remain, &readed, NULL)) {
		if(readed) {
			for(int i = 0; i < (int)readed; i++) {
				DisplayRecvCode(rs232->port_no, data_enc[i]);

				rs232->pCC->SetEncryptedData(data_enc[i]);	// 데이터를 넣는다.
			}

			readed = rs232->pCC->GetDecryptionData((BYTE*)buf, count);		
		}
	}
	else {

	}

	delete data_enc;

	return readed;
}

int PlcDeviceReadContinueRS232(DEVICE_STRUCT_RS232 *rs232, char *buf, int count)
{
	// 기존에 잘되는 루틴이 문제가 생길 수 있어 암호화 할때만 새로만든 함수를 호출해서 사용한다. 
	// ??_Crypto 함수를 그대로 대치해도 된다. 2016-10-28
	if(rs232->pCC->bUseEncryption) {
		return PlcDeviceReadContinueRS232_Crypto(rs232, buf, count);
	}

	DWORD readed = 0;
	COMSTAT comStat;
	DWORD code = 0;

	BOOL retn = ClearCommError(rs232->id, &code, &comStat);

	if(retn == 0 || code != 0) {
		int error = 0;
	}

	if(comStat.cbInQue == 0)	return 0;	// 읽어놓은 것이 없다.

	// 갯수가 10개 밖에 없는데 10개 이상 읽으면 다음데이타가 들어올 때까지 기다릴것 같아서
	// 2007.11.9
	if((int)comStat.cbInQue < count)	count = comStat.cbInQue;

	if(ReadFile(rs232->id, buf, count, &readed, NULL)) {
		if(readed) {
			for(int i = 0; i < (int)readed; i++) {
				DisplayRecvCode(rs232->port_no, buf[i]);
			}
		}
	}
	else {

	}

	return readed;
}

//	Data를 다 보내고 난 후 사용자가 지정한 시간만큼 기다린 후 DTR/RTS 를 닫는다.
//	현재는 for 문을 사용했다. 이것은 PC마다 Time이 틀릴 수 있다는 이야기이다.
//	Sleep()을 사용하면 Sleep 함수실행중에 다른 Process가 실행될 수 있으므로 실제 함수를
//	마치는 시간은 지정된 시간보다 훨씬 길 수도 있다.
//	GetLocalTime을 사용하는 MilliSecTimeOutClass 도 Sleep()과 비슷한 현상이 나타나고
//	clock()함수를 사용하는것도 문제가 있다.

static void WaitingBeforeCloseRtsDtr(DEVICE_STRUCT_RS232 *rs232)
{
	int i;
	
	if(bReadingOrWriting == 0) {
		for(i = 0; i < rs232->nEndDelayReadRTS; i++) {
			for(int j = 0; j < 100; j++);
		}
	}
	else {
		for(i = 0; i < rs232->nEndDelayWriteRTS; i++) {
			for(int j = 0; j < 100; j++);
		}
	}

	/*
	if(bReadingOrWriting == 0) {
		Sleep(rs232->nDelayReadRTS);
	}
	else {
		Sleep(rs232->nDelayWriteRTS);
	}
	*/

	/*
	milliTimeOut.Reset();

	if(bReadingOrWriting == 0) {
		while(1) {
			ClearCommError(rs232->id, &code, &comStat);
			if(milliTimeOut.IsTimeOut(rs232->nDelayReadRTS))	break;
		}
	}
	else {
		while(1) {
			ClearCommError(rs232->id, &code, &comStat);
			if(milliTimeOut.IsTimeOut(rs232->nDelayWriteRTS))	break;
		}
	}
	*/

	/*
	clock_t old;
	int curr;

	old = clock();

	if(bReadingOrWriting == 0) {
		while(1) {
			curr = clock()-old;
			if(curr >= rs232->nDelayReadRTS)	break;
			if(curr < 0)						break;
		}
	}
	else {
		while(1) {
			curr = clock()-old;
			if(curr >= rs232->nDelayWriteRTS)	break;
			if(curr < 0)						break;
		}
	}
	*/
}

static void WaitingBeforeSendDataRtsDtr(DEVICE_STRUCT_RS232 *rs232)
{
	int i;
	
	if(bReadingOrWriting == 0) {
		for(i = 0; i < rs232->nStartDelayReadRTS; i++) {
			for(int j = 0; j < 100; j++);
		}
	}
	else {
		for(i = 0; i < rs232->nStartDelayWriteRTS; i++) {
			for(int j = 0; j < 100; j++);
		}
	}
}

static int PlcDeviceWriteContinueRS232_Crypto(DEVICE_STRUCT_RS232 *rs232, char *source_buffer, int source_count)
{
	int count;

	BYTE *buf = rs232->pCC->GetEncryptionData((BYTE*)source_buffer, source_count, count);

	DWORD retn;
	int i;
	COMSTAT comStat;
	TimeOutClass timeout;
	DWORD code;

	if(rs232->txMethod == TX_METHOD_ON) {	// 항상 Tx Port가 열려 있다.

		if(WriteFile(rs232->id, buf, count, &retn, NULL) == FALSE) {
			bell();	
		}

		//------------------------------------------------------------------------
		// 보통 WriteComm 이후 return 해도 되나 통신이 끊어진 후 다시 연계될 때는
		// 통신이 정상으로 살아나지 못하는 경우가 있다. 이 때 다음의 부분을 추가하면
		// 통신이 제대로 살아난다.
		// 중요부분. (통신 OutQue상태가 비었을 때 ReadComm으로 넘어가면 해결되는 것으로 보임)
		//------------------------------------------------------------------------

		ClearCommError(rs232->id, &code, &comStat);
		timeout.Reset();
		while(comStat.cbOutQue != 0) {
			ClearCommError(rs232->id, &code, &comStat);
			if(timeout.IsTimeOut(3)) 	break;
		}
		
		for(i = 0; i < count; i++) DisplaySendCode(rs232->port_no, buf[i]);
	}
	else if(rs232->txMethod == TX_METHOD_RTS) {	// RTS 제어를 사용하고 RX는 항상 ON이다.
		if(!rs232->nRtsMethod)
			EscapeCommFunction(rs232->id, SETRTS);

		WaitingBeforeSendDataRtsDtr(rs232);

		if(!rs232->nRtsMethod) {
			SetCommMask(rs232->id, EV_TXEMPTY);
		}
		
		if(WriteFile(rs232->id, buf, count, &retn, NULL) == FALSE) {
			bell();	
			return 0;
		}

		if(!rs232->nRtsMethod) {
			DWORD mask = 0;

			timeout.Reset();
			while(1) {
				if(timeout.IsTimeOut(2))	break;
				if(WaitCommEvent(rs232->id, &mask, NULL)) {	
					if((mask & EV_TXEMPTY) == EV_TXEMPTY) {
						break;
					}
				}
			} 
		}

		WaitingBeforeCloseRtsDtr(rs232);

		if(!rs232->nRtsMethod)
			EscapeCommFunction(rs232->id, CLRRTS);

		for(i = 0; i < count; i++) DisplaySendCode(rs232->port_no, buf[i]);
	}
	else if(rs232->txMethod == TX_METHOD_DTR) {	// DTR 제어를 사용한다 
		EscapeCommFunction(rs232->id, SETDTR);

		WaitingBeforeSendDataRtsDtr(rs232);
		
		if(WriteFile(rs232->id, buf, count, &retn, NULL) == FALSE) {
			bell();	
			return 0;
		}

		else {
			SetCommMask(rs232->id, EV_TXEMPTY);

			DWORD mask = 0;

			timeout.Reset();
			while(1) {
				if(timeout.IsTimeOut(2))	break;
				if(WaitCommEvent(rs232->id, &mask, NULL)) {	
					if((mask & EV_TXEMPTY) == EV_TXEMPTY) {
						break;
					}
				}
			}

			WaitingBeforeCloseRtsDtr(rs232);
		}

		EscapeCommFunction(rs232->id, CLRDTR);

		for(i = 0; i < count; i++) DisplaySendCode(rs232->port_no, buf[i]);
	}
	else;

	if(rs232->rxMethod == RX_METHOD_ECHO) {
		timeout.Reset();
		while(1) {
			ClearCommError(rs232->id, &code, &comStat);
			if((int)comStat.cbInQue >= count) {
				char imsi[10];
				for(i = 0; i < count; i++)
					PlcDeviceReadContinueRS232(rs232, imsi, 1);
				break;
			}
			if(timeout.IsTimeOut(2)) 		break;
		}
	}

	return 1;
}

int PlcDeviceWriteContinueRS232(DEVICE_STRUCT_RS232 *rs232, char *buf, int count)
{
	// 기존에 잘되는 루틴이 문제가 생길 수 있어 암호화 할때만 새로만든 함수를 호출해서 사용한다.
	// ??_Crypto 함수를 그대로 대치해도 된다. 2016-10-28
	if(rs232->pCC->bUseEncryption) {
		return PlcDeviceWriteContinueRS232_Crypto(rs232, buf, count);
	}

	DWORD retn;
	int i;
	COMSTAT comStat;
	TimeOutClass timeout;
	DWORD code;

	if(rs232->txMethod == TX_METHOD_ON) {	// 항상 Tx Port가 열려 있다.

		if(WriteFile(rs232->id, buf, count, &retn, NULL) == FALSE) {
			bell();	
		}

		//------------------------------------------------------------------------
		// 보통 WriteComm 이후 return 해도 되나 통신이 끊어진 후 다시 연계될 때는
		// 통신이 정상으로 살아나지 못하는 경우가 있다. 이 때 다음의 부분을 추가하면
		// 통신이 제대로 살아난다.
		// 중요부분. (통신 OutQue상태가 비었을 때 ReadComm으로 넘어가면 해결되는 것으로 보임)
		//------------------------------------------------------------------------

		ClearCommError(rs232->id, &code, &comStat);
		timeout.Reset();
		while(comStat.cbOutQue != 0) {
			ClearCommError(rs232->id, &code, &comStat);
			if(timeout.IsTimeOut(3)) 	break;
		}
		
		for(i = 0; i < count; i++) DisplaySendCode(rs232->port_no, buf[i]);
	}
	else if(rs232->txMethod == TX_METHOD_RTS) {	// RTS 제어를 사용하고 RX는 항상 ON이다.
		if(!rs232->nRtsMethod)
			EscapeCommFunction(rs232->id, SETRTS);

		WaitingBeforeSendDataRtsDtr(rs232);

		if(!rs232->nRtsMethod) {
			SetCommMask(rs232->id, EV_TXEMPTY);
		}
		
		if(WriteFile(rs232->id, buf, count, &retn, NULL) == FALSE) {
			bell();	
			return 0;
		}

		if(!rs232->nRtsMethod) {
			DWORD mask = 0;

			timeout.Reset();
			while(1) {
				if(timeout.IsTimeOut(2))	break;
				if(WaitCommEvent(rs232->id, &mask, NULL)) {	
					if((mask & EV_TXEMPTY) == EV_TXEMPTY) {
						break;
					}
				}
			} 
		}

		WaitingBeforeCloseRtsDtr(rs232);

		if(!rs232->nRtsMethod)
			EscapeCommFunction(rs232->id, CLRRTS);

		for(i = 0; i < count; i++) DisplaySendCode(rs232->port_no, buf[i]);
	}
	else if(rs232->txMethod == TX_METHOD_DTR) {	// DTR 제어를 사용한다 
		EscapeCommFunction(rs232->id, SETDTR);

		WaitingBeforeSendDataRtsDtr(rs232);
		
		if(WriteFile(rs232->id, buf, count, &retn, NULL) == FALSE) {
			bell();	
			return 0;
		}

		else {
			SetCommMask(rs232->id, EV_TXEMPTY);

			DWORD mask = 0;

			timeout.Reset();
			while(1) {
				if(timeout.IsTimeOut(2))	break;
				if(WaitCommEvent(rs232->id, &mask, NULL)) {	
					if((mask & EV_TXEMPTY) == EV_TXEMPTY) {
						break;
					}
				}
			}

			WaitingBeforeCloseRtsDtr(rs232);
		}

		EscapeCommFunction(rs232->id, CLRDTR);

		for(i = 0; i < count; i++) DisplaySendCode(rs232->port_no, buf[i]);
	}
	else;

	if(rs232->rxMethod == RX_METHOD_ECHO) {
		timeout.Reset();
		while(1) {
			ClearCommError(rs232->id, &code, &comStat);
			if((int)comStat.cbInQue >= count) {
				char imsi[10];
				for(i = 0; i < count; i++)
					PlcDeviceReadContinueRS232(rs232, imsi, 1);
				break;
			}
			if(timeout.IsTimeOut(2)) 		break;
		}
	}

	return 1;
}

int PlcDeviceClearRS232(DEVICE_STRUCT_RS232 *rs232)
{
	char imsi[10];
	COMSTAT comStat;
	int  size;
	DWORD code;
	int i;
	DWORD retn = 0;

	ClearCommError(rs232->id, &code, &comStat);

	size = comStat.cbInQue;

	for(i = 0; i < size; i++) {
		if(!ReadFile(rs232->id, imsi, 1, &retn, NULL))	break;
		if(retn == 0)	break;
	}

//	PurgeComm(rs232->id, PURGE_TXCLEAR | PURGE_RXCLEAR );
	
	return 1;
}

int PlcDeviceUnInitRS232(DEVICE_STRUCT_RS232 *rs232)
{
	if(rs232->id != INVALID_HANDLE_VALUE) {
		CloseHandle(rs232->id);
		rs232->id = INVALID_HANDLE_VALUE;
	}

	return 1;
}

int PlcDeviceEnableRS232(DEVICE_STRUCT_RS232 *rs232, char flag)
{
	if(flag) {
		if(rs232->id == INVALID_HANDLE_VALUE) {
			return PlcDeviceOpenRS232(NULL, rs232);
		}
	}
	else {
		if(rs232->id != INVALID_HANDLE_VALUE) {
			CloseHandle(rs232->id);
			rs232->id = INVALID_HANDLE_VALUE;
		}
	}

	return 1;
}

int PlcDeviceGetCommErrorRS232(DEVICE_STRUCT_RS232 *rs232, COMSTAT *comStat)
{
	DWORD code;

	ClearCommError(rs232->id, &code, comStat);

	return code;
}


int PlcDeviceSetCommBreakRS232(DEVICE_STRUCT_RS232 *rs232)
{
	return SetCommBreak(rs232->id);
}

int PlcDeviceClearCommBreakRS232(DEVICE_STRUCT_RS232 *rs232)
{
	return ClearCommBreak(rs232->id);
}

void PlcDeviceGetCommModemStatusRS232(DEVICE_STRUCT_RS232 *rs232)
{
	DWORD dwModemStatus;
	BOOL fCTS, fDSR, fRING, fRLSD;

	if(!GetCommModemStatus(rs232->id, &dwModemStatus))
		// error
		return;

	fCTS = MS_CTS_ON & dwModemStatus;
	fDSR = MS_DSR_ON & dwModemStatus;
	fRING = MS_RING_ON & dwModemStatus;
	fRLSD = MS_RLSD_ON & dwModemStatus;

	PokeBitSYSTEM(rs232->port_no, 0x001C, fCTS > 0 ? 1 : 0);		// pin 8
	PokeBitSYSTEM(rs232->port_no, 0x001D, fDSR > 0 ? 1 : 0);		// pin 6
	PokeBitSYSTEM(rs232->port_no, 0x001E, fRING > 0 ? 1 : 0);		// pin 9
	PokeBitSYSTEM(rs232->port_no, 0x001F, fRLSD > 0 ? 1 : 0);		// pin 1
}


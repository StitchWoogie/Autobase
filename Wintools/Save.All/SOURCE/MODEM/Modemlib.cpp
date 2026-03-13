// english O.K
#include "stdafx.h"
#include <stdarg.h>
#include <stdio.h>
#include <string.h>

#include <compiler.hpp>
#include <modemlib.h>
#include <dataswap.h>
#include <tools.h>
#include <stdlib.h>

#define	MAX_BUF_READ	8192
#define	MAX_BUF_WRITE 	8192

#if	defined (_WIN32)
static void ShowError(HWND hwnd, int err, char *message)
{
	StackChar buf(1000);
	char	  imsi[80];

	switch(err) {
		case IE_BADID:	wsprintf(buf.data, "IE_BADID\nThe device identifier is invalid or unsupported");
							break;
		case IE_BAUDRATE:
							wsprintf(buf.data, "IE_BAUDRATE\nThe device's baud is unsupported");
							break;
		case IE_BYTESIZE:
							wsprintf(buf.data, "IE_BYTESIZE\nThe specified byte size if invalid");
							break;
		case IE_DEFAULT:
							wsprintf(buf.data, "IE_DEFAULT\nthe default parameters are in error");
							break;
		case IE_HARDWARE:
							wsprintf(buf.data, "IE_HARDWARE\nthe hardware is not available (is locked by another device).");
							break;
		case IE_MEMORY:
							wsprintf(buf.data, "IE_MEMORY\nthe function cannot allocate the queues.");
							break;
		case IE_NOPEN:	wsprintf(buf.data, "IE_NOPEN\nthe device is not open");
							break;
		case IE_OPEN:	wsprintf(buf.data, "IE_OPEN\nthe device is already open");
							break;
		case ERROR_ACCESS_DENIED:
#if	defined (COMPILE_ENGLISH)
							wsprintf(buf.data, "Access Denied(Check use another program)");
#else
							wsprintf(buf.data, "Access Denied(다른 곳에서 사용 중일 수 있음)");
#endif
							break;
		default:			wsprintf(buf.data, "Unknown Error");
							break;
	}

	sprintf(imsi, "\nErrorCode-%d", err);
	strcat(buf.data, imsi);

	MessageBox(hwnd, buf.data, message, MB_OK);
}

static BOOL NEAR SetupConnection( HWND hwnd, MODEM_STRUCT *rs232)
{
   BOOL       fRetVal;
   DCB        dcb;

   dcb.DCBlength = sizeof( DCB ) ;

   GetCommState(rs232->idDevice, &dcb ) ;

   dcb.BaudRate = rs232->lBaud;
   dcb.ByteSize = rs232->cData;
   dcb.Parity =   rs232->cParity;
	if(rs232->cStop == 1)			// 1 = 1.5
		dcb.StopBits = 0;
	else
		dcb.StopBits = 2;

   fRetVal = SetCommState(rs232->idDevice, &dcb ) ;

   return ( fRetVal ) ;

} // end of SetupConnection()

int ModemInstall(HWND hwnd, MODEM_STRUCT *modem, char message_flag)
{
    char buf[80];
	char message[80];
	COMMTIMEOUTS CommTimeOuts;

	//wsprintf(buf, "COM%d", (BYTE)modem->cPort+1);
	sprintf(buf, "\\\\.\\COM%d", (BYTE)modem->cPort+1);
 
	modem->idDevice = CreateFile(buf, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, /*FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED,*/ NULL); 

	if(modem->idDevice == INVALID_HANDLE_VALUE) {
		if(message_flag) {
			sprintf(message, "OpenComm : COM%d", modem->cPort+1);
			ShowError(hwnd, GetLastError(), message);
		}
		return 0;
	}

	// get any early notifications

	SetCommMask(modem->idDevice, EV_RXCHAR ) ;

	// setup device buffers

	if(SetupComm(modem->idDevice, MAX_BUF_READ, MAX_BUF_WRITE ) == FALSE) {
		if(message_flag) {
			ShowError(hwnd, GetLastError(), "SetupComm");
		}
		return 0;	
	}

	// purge any information in the buffer

	if(PurgeComm(modem->idDevice, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR ) == FALSE) {
		if(message_flag) {
			ShowError(hwnd, GetLastError(), "PergeComm");
		}
		return 0;	
	}

	// set up for overlapped I/O
	  
	CommTimeOuts.ReadIntervalTimeout = 0xFFFFFFFF ;
	CommTimeOuts.ReadTotalTimeoutMultiplier = 0 ;
	CommTimeOuts.ReadTotalTimeoutConstant = 1000 ;
	CommTimeOuts.WriteTotalTimeoutMultiplier = 0 ;
	CommTimeOuts.WriteTotalTimeoutConstant = 1000 ;
	SetCommTimeouts(modem->idDevice, &CommTimeOuts );
	
	SetupConnection(hwnd, modem);
	
	//EscapeCommFunction(rs232->id, SETDTR ) ;

	return 1;
}

int ModemUninstall(MODEM_STRUCT *modem)
{
	if(modem->idDevice != INVALID_HANDLE_VALUE)	CloseHandle(modem->idDevice);
	return 1;
}

int ModemWriteContinue(MODEM_STRUCT *modem, char *buf, int count)
{
	DWORD retn;
	COMSTAT comStat;
	TimeOutClass timeout;
	//OVERLAPPED overlap;
	DWORD code;

	if(WriteFile(modem->idDevice, buf, count, &retn, NULL) == FALSE) {
		bell();	
	}

	//------------------------------------------------------------------------
	// 보통 WriteComm 이후 return 해도 되나 통신이 끊어진 후 다시 연계될 때는
	// 통신이 정상으로 살아나지 못하는 경우가 있다. 이 때 다음의 부분을 추가하면
	// 통신이 제대로 살아난다.
	// 중요부분. (통신 OutQue상태가 비었을 때 ReadComm으로 넘어가면 해결되는 것으로 보임)
	//------------------------------------------------------------------------

	
	ClearCommError(modem->idDevice, &code, &comStat);
	timeout.Reset();
	while(comStat.cbOutQue != 0) {
		ClearCommError(modem->idDevice, &code, &comStat);
		if(timeout.IsTimeOut(3)) 	break;
	}

	return retn;
}

int ModemReadContinue(MODEM_STRUCT *modem, char *buf, int count)
{
	DWORD retn = 0;
	COMSTAT comStat;
	DWORD code;
	//OVERLAPPED overlap;

	ClearCommError(modem->idDevice, &code, &comStat);
	if(comStat.cbInQue == 0)	return 0;	// 읽어놓은 것이 없다.

	if(ReadFile(modem->idDevice, buf, count, &retn, NULL)) {

	}
	else {

	}

	return retn;
}

#else

static void ShowError(HWND hwnd, int err, char *message)
{
	char buf[100];

	switch(err) {
		case IE_BADID:	wsprintf(buf, "%s", "IE_BADID\nThe device identifier is invalid or unsupported");
							break;
		case IE_BAUDRATE:
							wsprintf(buf, "%s", "IE_BAUDRATE\nThe device's baud is unsupported");
							break;
		case IE_BYTESIZE:
							wsprintf(buf, "%s", "IE_BYTESIZE\nThe specified byte size if invalid");
							break;
		case IE_DEFAULT:
							wsprintf(buf, "%s", "IE_DEFAULT\nthe default parameters are in error");
							break;
		case IE_HARDWARE:
							wsprintf(buf, "%s", "IE_HARDWARE\nthe hardware is not available (is locked by another device).");
							break;
		case IE_MEMORY:
							wsprintf(buf, "%s", "IE_MEMORY\nthe function cannot allocate the queues.");
							break;
		case IE_NOPEN:	wsprintf(buf, "%s", "IE_NOPEN\nthe device is not open");
							break;
		case IE_OPEN:	wsprintf(buf, "%s", "IE_OPEN\nthe device is already open");
							break;
	}
	MessageBox(hwnd, buf, message, MB_OK);
}

int ModemInstall(HWND hwnd, MODEM_STRUCT *modem, char message_flag)
{
	int err;
	DCB dcb;
	char buf[80];
	char message[80];

	wsprintf(buf, "COM%d", (BYTE)modem->cPort+1);

	modem->idDevice = OpenComm(buf, MAX_BUF_READ, MAX_BUF_WRITE);

	if(modem->idDevice < 0) {
		if(message_flag) {
			sprintf(message, "OpenComm : COM%d", modem->cPort+1);
			ShowError(hwnd, modem->idDevice, message);
		}
		return 0;
	}

	// "COM1:9600, n, 8, 1"
	sprintf(buf, "COM%d:%lu,%c,%d,%d", (BYTE)modem->cPort+1, 9600L, modem->cParity == 1 ? 'p' : 'n', modem->cData, modem->cStop);

	err = BuildCommDCB(buf, &dcb);
	if(err < 0) {
		if(message_flag) {
			sprintf(message, "BuildCommDCB : COM%d", modem->cPort+1);
			ShowError(hwnd, err, message);
		}
		return 0;
	}

	switch(modem->lBaud) {
		case 2400L:		dcb.BaudRate = CBR_2400;	break;
		case 4800L:		dcb.BaudRate = CBR_4800;	break;
		case 9600L:		dcb.BaudRate = CBR_9600;	break;
		case 19200L:	dcb.BaudRate = CBR_19200;	break;
		case 38400L:	dcb.BaudRate = CBR_38400;	break;
		case 57600L:	dcb.BaudRate = CBR_56000;	break;
		case 115200L:	dcb.BaudRate = CBR_128000; break;
		default:			dcb.BaudRate = CBR_19200;	break;
	}
	dcb.BaudRate |= 0xFF00;

	err = SetCommState(&dcb);
	if(err < 0) {
		if(message_flag) {
			sprintf(message, "SetCommState : COM%d", modem->cPort+1);
			ShowError(hwnd, err, message);
		}
		return 0;
	}

	return 1;
}

int ModemUninstall(MODEM_STRUCT *modem)
{
	if(modem->idDevice >= 0)	CloseComm(modem->idDevice);

	return 1;
}

int ModemWriteContinue(MODEM_STRUCT *modem, char *string, int size)
{
	COMSTAT comStat;
	TimeOutClass timeout;

	if(modem->idDevice < 0)	return 0;

	timeout.Reset();
	while(1) {
		if(timeout.IsTimeOut(2)) 							return 0;
		GetCommError(modem->idDevice, &comStat);
		if(comStat.cbOutQue+size <= MAX_BUF_WRITE)	break;
	}

	return WriteComm(modem->idDevice, string, size);
}

int ModemReadContinue(MODEM_STRUCT *modem, char *string, int size)
{
	COMSTAT comStat;
	GetCommError(modem->idDevice, &comStat);
	return ReadComm(modem->idDevice, string, size);
}
#endif	// 16 bit programm

void ModemClear(MODEM_STRUCT *modem)
{
	char buf[20];
	int i;

	for(i = 0; i < 100; i++) {
		if(ModemReadContinue(modem, buf, 10) != 10)	break;
	}
}

int ModemWrite(MODEM_STRUCT *modem, char ch)
{
	char buf[2];

	buf[0] = ch;

	return ModemWriteContinue(modem, buf, 1);
}

int ModemWriteString(MODEM_STRUCT *modem, LPSTR string, ...)
{
	va_list ap;
	StackChar imsi(1000);

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);

		return(ModemWriteContinue(modem, imsi.data, strlen(imsi.data)));
	}

	return 0;
}

//------------------------------------------------------------------------------
//	주어진 스트링이 들어올 때까지 몇초간 기다린다.
//------------------------------------------------------------------------------

int ModemWaitString(MODEM_STRUCT *modem, char *string)
{
	TimeOutClass timeout;
	int  curr = 0;
	char buf[2];
	char recv[100];
	int shap = strlen(string);

	timeout.Reset();
	while(1) {
		if(timeout.IsTimeOut(3))	return 0;
		if(ModemReadContinue(modem, buf, 1)) {
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

int ModemWaitOK(MODEM_STRUCT *modem)
{
	if(modem->bNullOrModem == 0) {	// NULL_MODEM
		return 1;
	}
	else {
		return ModemWaitString(modem, "OK");
	}
}

//------------------------------------------------------------------------------
//	전화 끊기
//------------------------------------------------------------------------------

int ModemHangUp(MODEM_STRUCT *modem)
{
	int i;

	for(i = 0; i < 3; i++) {
		ModemWriteString(modem, "+++");
		if(ModemWaitOK(modem)) {
			ModemWriteString(modem, "ATH\r");
			if(ModemWaitOK(modem))	return 1;
		}
		ModemWriteString(modem, "ATH\r");
		if(ModemWaitOK(modem))	return 1;
	}

	return 0;
}

void ConvertModemStructToBuf(char *buf, MODEM_STRUCT *modem)
{
	sprintf(buf, "COM%d,%lu,%d,%d,%d,", modem->cPort+1, modem->lBaud, modem->cParity, modem->cData, modem->cStop);
}

void ConvertBufToModemStruct(const char *buf, MODEM_STRUCT *modem)
{
	CommaBlockString comma;
	char imsi[80];

	comma.Set((char*)buf);

	comma.GetString(imsi, sizeof(imsi));
	modem->cPort = atoi(&imsi[3])-1;
	comma.GetString(imsi, sizeof(imsi));
	modem->lBaud = atol(imsi);
	comma.GetString(imsi, sizeof(imsi));
	modem->cParity = atoi(imsi);
	comma.GetString(imsi, sizeof(imsi));
	modem->cData = atoi(imsi);
	comma.GetString(imsi, sizeof(imsi));
	modem->cStop = atoi(imsi);
}








#include "stdafx.h"

#include <tools.h>

#include "Win32Common.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

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
//	MessageBox(hwnd, buf, message, MB_OK);
}

BOOL NEAR SetupLinePrnConnection(HANDLE handle, DWORD baud, char parity, char data, char stop)
{
	BOOL       fRetVal;
	BYTE       bSet;
	DCB        dcb;

	dcb.DCBlength = sizeof( DCB ) ;

	GetCommState(handle, &dcb ) ;

	dcb.BaudRate = baud;
	dcb.ByteSize = data;
	dcb.Parity =   parity;
	if(stop == 1)			// 1 = 1.5
		dcb.StopBits = 0;
	else
		dcb.StopBits = 2;

	bSet = 0;
	dcb.fOutxDsrFlow = bSet;

	dcb.fDtrControl = DTR_CONTROL_ENABLE;

	bSet = 0;
	dcb.fOutxCtsFlow = bSet;

	dcb.fRtsControl = RTS_CONTROL_ENABLE;

	// setup software flow control

	bSet = 0;
	dcb.fInX = dcb.fOutX = bSet ;
	dcb.XonChar  = 0;
	dcb.XoffChar = 0;
	dcb.XonLim  = 100;
	dcb.XoffLim = 100;

	// other various settings

	dcb.fBinary = TRUE;
	dcb.fParity = TRUE;

	fRetVal = SetCommState(handle, &dcb );
	return ( fRetVal );
} 

OVERLAPPED	osWrite, osRead ;
BOOL	connected=FALSE ; // TRUE if connected

HANDLE OpenRS232(HWND hwnd, char type, int port, DWORD baud, char parity, char data, char stop)
{
	HANDLE idComDev;
	
	if(connected == TRUE)
		return INVALID_HANDLE_VALUE;

	char Buffer[20];

	if(type == 0) wsprintf(Buffer,"LPT%d",port);	
	else	      wsprintf(Buffer, "\\\\.\\COM%d", port);

	idComDev = CreateFile (Buffer, GENERIC_READ | GENERIC_WRITE, 0, // Exclusive access
																NULL,	// no Security attrs
																OPEN_EXISTING,
																FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, // overlapped I/O
																NULL);
	if(idComDev==INVALID_HANDLE_VALUE){
		return INVALID_HANDLE_VALUE;
	}		

	SetCommMask (idComDev, EV_RXCHAR) ; // 통신 포트로 들어오는 글자를 잡겠다.
	SetupComm (idComDev, 4096, 0) ; // Rx, Tx Buffer Size
	

	PurgeComm (idComDev, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR) ; // Buffer Clear

	// serial port 일때
	if(type == 1) SetupLinePrnConnection(idComDev, baud, parity, data, stop);

/*	DCB dcb ;

	static DWORD SpeedForm[13] = {110,300,1200,2400,4800,9600,19200,38400,57600,115200,230400,460800,921600};
	static int DataBitForm[2] = {8,7};
	static int StopBitForm[2] = {0,2};

	GetCommState (idComDev, &dcb) ;
	dcb.BaudRate = SpeedForm[m_nBaudRate];
	dcb.ByteSize = DataBitForm[m_nDataBits] ;
	dcb.Parity = m_nParity;
	dcb.StopBits = StopBitForm[m_nStopBits];
	SetCommState (idComDev, &dcb) ;
*/
	osRead.Offset = 0 ;
	osRead.OffsetHigh = 0 ;
	osWrite.Offset = 0 ;
	osWrite.OffsetHigh = 0 ;

	osRead.hEvent = CreateEvent (NULL, TRUE, FALSE, NULL) ;
	osWrite.hEvent = CreateEvent (NULL, TRUE, FALSE, NULL) ;

	connected = TRUE ;
	return idComDev;
}


int WriteRS232(HANDLE idComDev, char far *buf, int count)
{
	DWORD dwBytesWritten;
	COMSTAT comStat;
	TimeOutClass timeout;
	DWORD code;

	if (connected != TRUE)
		return 0;

	WriteFile (idComDev, buf, count, &dwBytesWritten, &osWrite);

	ClearCommError(idComDev, &code, &comStat);
	timeout.Reset();
	while(comStat.cbOutQue != 0) {
		ClearCommError(idComDev, &code, &comStat);
		if(timeout.IsTimeOut(2)) 	break;
	}
	return dwBytesWritten;
}


int CloseRS232(HANDLE idComDev)
{
	if (connected != TRUE)
		return 0;

	connected = FALSE ;
	SetCommMask (idComDev, 0) ;
	EscapeCommFunction (idComDev, CLRDTR) ;
	PurgeComm (idComDev, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR) ;
	CloseHandle (idComDev) ;

	CloseHandle (osRead.hEvent) ;
	CloseHandle (osWrite.hEvent) ;
	return 1;
}


void SetPrinterErrorMessage(HWND hwnd, int nState,char *String)
{
//	char	buf[100];

	if(nState){
		if(nState & CE_OOP) {;
			//MessageBeep(MB_OK);
			//MessageDisplay("프린터 용지가 없습니다.");			
		}
		else;
			//MessageBeep(MB_OK);
			//MessageDisplay("프린터에 이상이 있습니다.");
			//SmLogMessage("현재 에러가 발생했으나 프린터에 이상이 있어 인쇄를 못합니다.");
	}
	//SmLogMessage("인쇄될 메세지: %s",String);
	//MessageBeep(MB_OK);
}

extern "C" int DLLEXPORT OpenWriteClose(char port_type, int port_no, int port_baud, char port_parity, char port_data, char port_stop, char *buffer, int size)
{
	HANDLE CommId;
	int nState=1;

	if(port_type == 0)	CommId = OpenRS232(NULL, port_type, port_no, 9600, 0, 8, 1);
	else				CommId = OpenRS232(NULL, port_type, port_no, port_baud, port_parity, port_data, port_stop);

	if(size == 0)	size = (int)strlen(buffer);

	if(WriteRS232(CommId,buffer,size) <= 0) {
		//SetPrinterErrorMessage(hwnd, nState,String); 
		CloseRS232(CommId);
		return nState;
	}	
	CloseRS232(CommId);	
	return nState;
}

extern "C" HANDLE DLLEXPORT DeviceOpen(char port_type, int port_no, int port_baud, char port_parity, char port_data, char port_stop)
{
	HANDLE CommId;
	int nState=1;

	if(port_type == 0)	CommId = OpenRS232(NULL, port_type, port_no, 9600, 0, 8, 1);
	else				CommId = OpenRS232(NULL, port_type, port_no, port_baud, port_parity, port_data, port_stop);

	return CommId;
}

extern "C" int DLLEXPORT DeviceWrite(HANDLE handle, char *buffer, int size)
{
	if(size == 0)	size = (int)strlen(buffer);

	return WriteRS232(handle,buffer,size);
}

extern "C" void DLLEXPORT DeviceClose(HANDLE handle)
{
	CloseRS232(handle);
}

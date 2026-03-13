// english O.K
#include "stdafx.h"

#include <ddeml.h>

#include <tools.h>
#include <dataswap.h>
#include <glib.h>

#include "..\catlib.src\pub_msg.h"

#include "device\commmain.h"
#include "..\catlib.src\SystemStatusMemory.h"
#include "..\catlib.src\NetWorkProtocol.h"
#include "plc_scan.h"
#include "protocol\pro_lib.h"
#include "resource.h" 
#include "protocol\pro_main.h"
 
void SetReadingOrWriting(char flag);

// CString을 여러군데의 Thread에서 쓰거나 읽거나 하면 CString 오류가 발생한다. 특히 CPU가 2개인 서버 기종에서 발생
/*
class MultiThreadWaiting {
		char bWaiting;
	public:
		MultiThreadWaiting();
		void Set();
		void Reset();
};

MultiThreadWaiting :: MultiThreadWaiting()
{
	bWaiting = OFF;
}

void MultiThreadWaiting :: Set()
{
	TimeOutClass timeout;
	while(bWaiting) {
		if(timeout.IsTimeOut(3))	break;
	}
	bWaiting = ON;
}

void MultiThreadWaiting :: Reset()
{
	bWaiting = OFF;
}

MultiThreadWaiting waitMessage;
static CString sWaitMessage;

void MessageDisplay(const char *string)
{
	if(config.bMessageBoxAllMessage == OFF)	return;
	waitMessage.Set();
	sWaitMessage = string;
	waitMessage.Reset();
}

static void WaitMessageLoad()
{
	waitMessage.Set();
	if(strlen(sWaitMessage) > 0) {
		char buf[80];
		struct time t;

		gettime(&t);

		if(IsLangKorean()) {
			sprintf(buf, "통신 메세지 발생 (%02d:%02d:%02d)", t.ti_hour, t.ti_min, t.ti_sec);
		}else {
			sprintf(buf, "Message (%02d:%02d:%02d)", t.ti_hour, t.ti_min, t.ti_sec);
		}

		MessageScreen(buf, sWaitMessage);
		sWaitMessage = "";
	}
	waitMessage.Reset();
}
*/

static char sWaitMessage[1024];

static ThreadLock tlMessageDisplay;

void MessageDisplay(const char *string)
{
	if(config.bMessageBoxAllMessage == OFF)	return;

	tlMessageDisplay.Lock();
	if(strlen(string) >= 1024) {
		strncpy(sWaitMessage, string, 1023);
		sWaitMessage[1023] = 0;
	}
	else {
		strcpy(sWaitMessage, string);
	}
	tlMessageDisplay.Unlock();
}

static void WaitMessageLoad()
{
	tlMessageDisplay.Lock();
	if(strlen(sWaitMessage) > 0) {
		char buf[80];
		struct time t;

		gettime(&t);

		if(IsLangKorean())
			sprintf(buf, "통신 메세지 발생 (%02d:%02d:%02d)", t.ti_hour, t.ti_min, t.ti_sec);
		else
			sprintf(buf, "Communication Message (%02d:%02d:%02d)", t.ti_hour, t.ti_min, t.ti_sec);

		MessageScreen(buf, sWaitMessage);
		ZeroMemory(sWaitMessage, 1024);
	}
	tlMessageDisplay.Unlock();
}



void CalcSuccessPercent(GLOBAL_PORT_STRUCT *pt, COMM_COUNT_STRUCT *count)
{
	DWORD error = 0;

	error += count->lCountCodeBad;
	error += count->lCountTimeOut;

	if(count->lCountCommTry == 0) 
		count->wSuccessPercent = 100;
	else
		count->wSuccessPercent = (WORD)(((_int64)10000)*(count->lCountCommTry-error)/count->lCountCommTry);
}

static void PokeSuccessPercent(GLOBAL_PORT_STRUCT *pt)
{
	PokeWordSYSTEM(pt, 5, pt->countAll.Total.wSuccessPercent);
	PokeWordSYSTEM(pt, 6, 10000-pt->countAll.Total.wSuccessPercent);
	PokeWordSYSTEM(pt, 7, pt->countDevice[0].Total.wSuccessPercent);
	PokeWordSYSTEM(pt, 8, pt->countDevice[1].Total.wSuccessPercent);
}

static void PokeSystemDWORD(GLOBAL_PORT_STRUCT *pt, int address, DWORD value)
{
	PokeWordSYSTEM(pt, address+0, LOWORD(value));
	PokeWordSYSTEM(pt, address+1, HIWORD(value));
}

void StationInfoPlusCommCountReadTry(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoPlusCommCountReadTimeOut(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoPlusCommCountReadCodeBad(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoPlusCommCountWriteBitTry(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoPlusCommCountWriteBitTimeOut(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoPlusCommCountWriteBitCodeBad(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoPlusCommCountWriteWordTry(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoMinusCommCountWriteWordTry(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoPlusCommCountWriteWordTimeOut(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoPlusCommCountWriteWordCodeBad(GLOBAL_PORT_STRUCT *pt, int station);

static void PlusCommCountReadTry(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->Read.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->Read);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->Read.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->Read);
	CalcSuccessPercent(pt, &device->Total);
	
	PokeSuccessPercent(pt);

	PokeSystemDWORD(pt, 10, device->Total.lCountCommTry);
	PokeSystemDWORD(pt, 16, device->Read.lCountCommTry);

	StationInfoPlusCommCountReadTry(pt, station);
}

static void PlusCommCountReadTimeOut(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->Read.lCountTimeOut++;
	device->Total.lCountTimeOut++;
	CalcSuccessPercent(pt, &device->Read);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->Read.lCountTimeOut++;
	device->Total.lCountTimeOut++;
	CalcSuccessPercent(pt, &device->Read);
	CalcSuccessPercent(pt, &device->Total);

	PokeSystemDWORD(pt, 12, device->Total.lCountTimeOut);
	PokeSystemDWORD(pt, 18, device->Read.lCountTimeOut);

	StationInfoPlusCommCountReadTimeOut(pt, station);
}

static void PlusCommCountReadCodeBad(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->Read.lCountCodeBad++;
	device->Total.lCountCodeBad++;
	CalcSuccessPercent(pt, &device->Read);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->Read.lCountCodeBad++;
	device->Total.lCountCodeBad++;
	CalcSuccessPercent(pt, &device->Read);
	CalcSuccessPercent(pt, &device->Total);

	PokeSystemDWORD(pt, 14, device->Total.lCountCodeBad);
	PokeSystemDWORD(pt, 20, device->Read.lCountCodeBad);

	StationInfoPlusCommCountReadCodeBad(pt, station);
}

static void PlusCommCountWriteBitTry(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->WriteBit.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->WriteBit);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->WriteBit.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->WriteBit);
	CalcSuccessPercent(pt, &device->Total);

	PokeSuccessPercent(pt);

	PokeSystemDWORD(pt, 10, device->Total.lCountCommTry);
	PokeSystemDWORD(pt, 22, device->WriteBit.lCountCommTry);

	StationInfoPlusCommCountWriteBitTry(pt, station);
}

static void PlusCommCountWriteBitTimeOut(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->WriteBit.lCountTimeOut++;
	device->Total.lCountTimeOut++;
	CalcSuccessPercent(pt, &device->WriteBit);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->WriteBit.lCountTimeOut++;
	device->Total.lCountTimeOut++;
	CalcSuccessPercent(pt, &device->WriteBit);
	CalcSuccessPercent(pt, &device->Total);

	PokeSystemDWORD(pt, 12, device->Total.lCountTimeOut);
	PokeSystemDWORD(pt, 24, device->WriteBit.lCountTimeOut);

	StationInfoPlusCommCountWriteBitTimeOut(pt, station);
}

static void PlusCommCountWriteBitCodeBad(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->WriteBit.lCountCodeBad++;
	device->Total.lCountCodeBad++;
	CalcSuccessPercent(pt, &device->WriteBit);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->WriteBit.lCountCodeBad++;
	device->Total.lCountCodeBad++;
	CalcSuccessPercent(pt, &device->WriteBit);
	CalcSuccessPercent(pt, &device->Total);

	PokeSystemDWORD(pt, 14, device->Total.lCountTimeOut);
	PokeSystemDWORD(pt, 26, device->WriteBit.lCountTimeOut);

	StationInfoPlusCommCountWriteBitCodeBad(pt, station);
}

static void PlusCommCountWriteWordTry(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->WriteWord.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->WriteWord.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	PokeSuccessPercent(pt);

	PokeSystemDWORD(pt, 10, device->Total.lCountCommTry);
	PokeSystemDWORD(pt, 28, device->WriteWord.lCountCommTry);

	StationInfoPlusCommCountWriteWordTry(pt, station);
}

static void MinusCommCountWriteWordTry(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->WriteWord.lCountCommTry--;
	device->Total.lCountCommTry--;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->WriteWord.lCountCommTry--;
	device->Total.lCountCommTry--;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	PokeSuccessPercent(pt);

	PokeSystemDWORD(pt, 10, device->Total.lCountCommTry);
	PokeSystemDWORD(pt, 28, device->WriteWord.lCountCommTry);

	StationInfoMinusCommCountWriteWordTry(pt, station);
}

static void PlusCommCountWriteWordTimeOut(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->WriteWord.lCountTimeOut++;
	device->Total.lCountTimeOut++;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->WriteWord.lCountTimeOut++;
	device->Total.lCountTimeOut++;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	PokeSystemDWORD(pt, 12, device->Total.lCountTimeOut);
	PokeSystemDWORD(pt, 30, device->WriteWord.lCountTimeOut);

	StationInfoPlusCommCountWriteWordTimeOut(pt, station);
}

static void PlusCommCountWriteWordCodeBad(GLOBAL_PORT_STRUCT *pt, int station)
{
	DEVICE_COUNT_STRUCT *device;
	
	device = &pt->countDevice[pt->cDualCurrentActiveDevice];
	device->WriteWord.lCountCodeBad++;
	device->Total.lCountCodeBad++;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	device = &pt->countAll;
	device->WriteWord.lCountCodeBad++;
	device->Total.lCountCodeBad++;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	PokeSystemDWORD(pt, 14, device->Total.lCountCodeBad);
	PokeSystemDWORD(pt, 32, device->WriteWord.lCountCodeBad);

	StationInfoPlusCommCountWriteWordCodeBad(pt, station);
}

void ElseCommunicationMessage(int port, int station, char *type, int address, int error)
{
	StackChar message(1000);

	switch(error) {
		case COMMUNICATION_PROGRAMM_NOT_MAKED:
			wsprintf(message.data, "Communication Error- Programm Not maked (Programm Error)");
			break;
		case COMMUNICATION_UNDEFINED_PROTOCOL:
			wsprintf(message.data, "Communication Error- Undefined Protocol (Programm Error)");
			break;
		case COMMUNICATION_UNDEFINED_DEVICE:
			wsprintf(message.data, "Communication Error- Undefined Device (Programm Error)");
			break;

		case COMMUNICATION_ERR_BAD_COMMAND:
			wsprintf(message.data, "Communication Send Error- Bad command");
			break;
		case COMMUNICATION_ERR_BAD_STATION:
			wsprintf(message.data, "Communication Send Error- Bad station");
			break;
		case COMMUNICATION_ERR_BAD_ADDRESS:
			wsprintf(message.data, "Communication Send Error- Bad address");
			break;
		case COMMUNICATION_ERR_SIZE_TOO_BIG:
			wsprintf(message.data, "Communication Send Error- Size Too Big");
			break;
		case COMMUNICATION_ERR_BAD_CRC:
			wsprintf(message.data, "Communication Send Error- Bad CRC");
			break;
		case COMMUNICATION_ERR_RESET:
			wsprintf(message.data, "Communication Error- Controller RESET failed");
			break;
		case COMMUNICATION_ERR_CONTROLLER:
			wsprintf(message.data, "Communication Send Error- Bad Controller");
			break;
		case COMMUNICATION_ERR_STRING:
		case COMMUNICATION_ERR_STRING_AND_CODEBAD:
		case COMMUNICATION_ERR_STRING_AND_TIMEOUT:
			//char *PlcScanGetErrorString();	// 통신 return이 Communication_err_string일 때 스트링의 포인트를 얻을 수 있다.
			//strcpy(message.data, PlcScanGetErrorString());
			void PlcScanGetErrorString(char *buf);	// 통신 return이 Communication_err_string일 때 스트링의 포인트를 얻을 수 있다.
			PlcScanGetErrorString(message.data);
			break;

		case COMMUNICATION_LOCAL_PROTOCOL_MEMORY_NOT_ALLOCATED:	// local 프로토콜 메모리가 할당되지 않았다.
			strcpy(message.data, "Local Protocol Memory Not Allocated");
			break;
		default:
			wsprintf(message.data, "Communication Error- Undefined Error...");
			break;
	}

	int pos = strlen(message.data);
	sprintf(&message.data[pos], "\nPort=%d, station=%d, type=%s, address=%X", port, station, type, address);

	MessageDisplay(message.data);
}

//-------------------------------------------------------------------------
//	하나의 READ항목에 대한 메모리를 깨끗이 한다.
//-------------------------------------------------------------------------

static void OneReadItemMemoryClear(GLOBAL_PORT_STRUCT *pt_curr, SCAN_METHOD_STRUCT *sm)
{
	int i;

	if(config.bBufClearOnTimeOut5 == OFF)	return;

	if(pt_curr->nScanProtocol == PROTOCOL_NETWORK_CLIENT_MULTI) {	// multi로 묶여있는 항목이다.

		WORD act[16];
		void ProtocolNetClientMultiGetOption(WORD *wActPort, char *option);
		ProtocolNetClientMultiGetOption(act, pt_curr->sScanProtocolOption[pt_curr->cDualCurrentActiveDevice]);

		GLOBAL_PORT_STRUCT *global;

		for(int port = 0; port < 256; port++) {
			global = &portBuf[port];

			if((act[port/16] & WORD_MASK[port%16]) == 0)	continue;
			if(global->bActiveFlag == OFF)					continue;

			for(i = 0; i < sm->size; i++) {
				PokeValue(&global->local, sm, sm->target+i, config.fBufClearValueOnTimeOut5);
			}
		}
	}
	else {	// 단독 항목이다.
		for(i = 0; i < sm->size; i++) {
			PokeValue(&pt_curr->local, sm, sm->target+i, config.fBufClearValueOnTimeOut5);
		}
	}
}

void RunWriteWait(GLOBAL_PORT_STRUCT *pt);

int IsTimeLocalReadScan(GLOBAL_PORT_STRUCT *pt)
{
	// 통신을 시작하기 전 통신주기가 되지 않았으면 return 한다.

	if(pt->local.nLocalReadScanTime == 0)		return 1;
		
	struct time t;
	gettime(&t);
	int gab;

	if(t.ti_min != pt->tLocalReadScanTimeOld.ti_min) {	// 분이 넘어감.
		gab = ((60+t.ti_sec)*1000+t.ti_hund*10)
			  -(pt->tLocalReadScanTimeOld.ti_sec*1000+pt->tLocalReadScanTimeOld.ti_hund*10);;
	}
	else {
		gab = (t.ti_sec*1000+t.ti_hund*10)
			  -(pt->tLocalReadScanTimeOld.ti_sec*1000+pt->tLocalReadScanTimeOld.ti_hund*10);
	}
	pt->nLocalReadScanTimeCurr += gab;
	memcpy(&pt->tLocalReadScanTimeOld, &t, sizeof(t));

	// 통신 주기가 아직 되지 않았다.
	if(pt->nLocalReadScanTimeCurr < pt->local.nLocalReadScanTime) 
		return 0;

	pt->nLocalReadScanTimeCurr -= pt->local.nLocalReadScanTime;
	if(pt->nLocalReadScanTimeCurr > 3600000) {	// 숫자가 너무 커지면 -가 되는수가 있으므로 잘라준다.
		pt->nLocalReadScanTimeCurr = 3600000;
	}

	return 1;
}

int IsTimeLocalWriteScan(GLOBAL_PORT_STRUCT *pt)
{
	// 통신을 시작하기 전 통신주기가 되지 않았으면 return 한다.

	if(pt->local.nLocalWriteScanTime == 0)		return 1;
		
	struct time t;
	gettime(&t);
	int gab;

	if(t.ti_min != pt->tLocalWriteScanTimeOld.ti_min) {	// 분이 넘어감.
		gab = ((60+t.ti_sec)*1000+t.ti_hund*10)
			  -(pt->tLocalWriteScanTimeOld.ti_sec*1000+pt->tLocalWriteScanTimeOld.ti_hund*10);;
	}
	else {
		gab = (t.ti_sec*1000+t.ti_hund*10)
			  -(pt->tLocalWriteScanTimeOld.ti_sec*1000+pt->tLocalWriteScanTimeOld.ti_hund*10);
	}
	pt->nLocalWriteScanTimeCurr += gab;
	memcpy(&pt->tLocalWriteScanTimeOld, &t, sizeof(t));
	// 통신 주기가 아직 되지 않았다.
	if(pt->nLocalWriteScanTimeCurr < pt->local.nLocalWriteScanTime) 
		return 0;

	pt->nLocalWriteScanTimeCurr -= pt->local.nLocalWriteScanTime;
	if(pt->nLocalWriteScanTimeCurr > 3600000) {	// 숫자가 너무 커지면 -가 되는수가 있으므로 잘라준다.
		pt->nLocalWriteScanTimeCurr = 3600000;
	}

	return 1;
}

void PlcProtocolInitOne(HWND hwnd, GLOBAL_PORT_STRUCT *port, char bBasicOrDual);
void PlcProtocolUnInitOne(GLOBAL_PORT_STRUCT *port);
void WaitThreadProtocolDrawWorking(GLOBAL_PORT_STRUCT *pt);

void PokeDualSystem(GLOBAL_PORT_STRUCT *pt, char bBasicOrDual)
{
	PokeBitSYSTEM(pt, 0x0020, bBasicOrDual);
	pt->cDualCurrentActiveDevice = bBasicOrDual;
}

//-------------------------------------------------------------------------------------
//	선로 이중화 시스템일 때 통신시간 초과가 발생할 때 체크한다.
//-------------------------------------------------------------------------------------

static int ChangeDualSystem(GLOBAL_PORT_STRUCT *pt, char default_dual)
{
	if(pt->bDualActive == OFF)	return 0;	// 이중화 시스템이 아니다.

	pt->nDualTimeOutCount = 0;	// time out을 Clear한다.
	pt->nDualCodeBadCount = 0;	// code bad을 Clear한다.

	WaitThreadProtocolDrawWorking(pt);
	pt->bThreadProtocolDrawWorking = ON;

	PlcProtocolUnInitOne(pt);
	PlcDeviceUnInit(&pt->local.device);

	if(default_dual == 0) {	// 기본 디바이스로 연결.
		pt->nScanDevice = PlcDeviceInit(hwndMainFrame, &pt->local.device, pt->sScanDevice, pt);	
		PlcProtocolInitOne(hwndMainFrame, pt, default_dual);
		pt->bThreadProtocolDrawWorking = OFF;
		if(!pt->nScanDevice) {
			StackChar msg(1000);
			if(IsLangKorean()) {
				sprintf(msg.data, "Port:%03d의 선로 이중화 절체 중 오류\n알 수 없는 기본 DEVICE 종류입니다.\n%s\n디바이스를 바꿀 수 없습니다.", pt->local.no, pt->sScanDevice);
			}else {
				sprintf(msg.data, "Port:%03d Dual Line Change\nUnknown Primary DEVICE Type.\n%s\nCan't change device.", pt->local.no, pt->sScanDevice);
			}
			MessageDisplay(msg.data);
			return 0;
		}
	}
	else {	// 예비 디바이스로 연결.
		pt->nScanDevice = PlcDeviceInit(hwndMainFrame, &pt->local.device, pt->sDualDevice, pt);
		PlcProtocolInitOne(hwndMainFrame, pt, default_dual);
		pt->bThreadProtocolDrawWorking = OFF;
		if(!pt->nScanDevice) {
			StackChar msg(1000);
			if(IsLangKorean()) {
				sprintf(msg.data, "Port:%03d의 선로 이중화 절체 중 오류\n알 수 없는 예비 DEVICE 종류입니다.\n%s\n디바이스를 바꿀 수 없습니다.", pt->local.no, pt->sDualDevice);
			} else {
				sprintf(msg.data, "Port:%03d Dual Line Change\nUnknown Secondary DEVICE Type.\n%s\nCan't change device.", pt->local.no, pt->sDualDevice);
			}
			MessageDisplay(msg.data);
			return 0;
		}
	}

	StackChar msg(1000);
	if(default_dual == 0) {	// 기본 디바이스로 연결.
		if(IsLangKorean()) {
			sprintf(msg.data, "Port:%03d의 Device를 기본 DEVICE로 절체 했습니다.\n%s", pt->local.no, pt->sScanDevice);
		} else {
			sprintf(msg.data, "Port:%03d Changed to Primary DEVICE.\n%s", pt->local.no, pt->sScanDevice);
		}
	}
	else {
		if(IsLangKorean()) {
			sprintf(msg.data, "Port:%03d의 Device를 예비 DEVICE로 절체 했습니다.\n%s", pt->local.no, pt->sDualDevice);
		} else {
			sprintf(msg.data, "Port:%03d Changed to Secondary DEVICE.\n%s", pt->local.no, pt->sDualDevice);
		}
	}
	MessageDisplay(msg.data);

	PokeDualSystem(pt, default_dual);
	
	return 1;
}

//-------------------------------------------------------------------------------------
//	선로 이중화 시스템일 때 통신시간 초과가 발생할 때 체크한다.
//-------------------------------------------------------------------------------------

static int CheckDualSystem(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->bDualActive == OFF)	return 0;		// 이중화 시스템이 아니다.

	bool must_change = false;

	if(pt->nDualCauseTimeOut > 0 && pt->nDualTimeOutCount >= pt->nDualCauseTimeOut) 
		must_change = true;
	if(pt->nDualCauseCodeBad > 0 && pt->nDualCodeBadCount >= pt->nDualCauseCodeBad) 
		must_change = true;

	if(!must_change)	return 0;
	/*
	if(pt->nDualCauseTimeOut == 0)	return 0;	// 수동모드

	if(pt->nDualTimeOutCount < pt->nDualCauseTimeOut)	return 0;	// TimeOut이 목표치에 도달하지 않았다.*/

	char cDualCurrentActiveDevice = pt->cDualCurrentActiveDevice;
	cDualCurrentActiveDevice ++;
	cDualCurrentActiveDevice %= 2;

	if(ChangeDualSystem(pt, cDualCurrentActiveDevice)) {
		return 1;
	}
	else {
		// 이상이 발생하면 원래대로 복구한다. 이것은 예비디바이스가 열리지 않는다는 의미이다.
		cDualCurrentActiveDevice ++;
		cDualCurrentActiveDevice %= 2;
		ChangeDualSystem(pt, cDualCurrentActiveDevice);
	}
	
	return 0;
}

static void CheckPcDualDevice(GLOBAL_PORT_STRUCT *pt)
{
	// 여기서 컴퓨터 이중화일때 디바이스를 Enable/Disable 한다.

	if(pt->pcDualFile.bActive) {
		if(pt->pcDualRun.bActiveI) {
			if(pt->nScanDevice == 0) {
				if(pt->cDualCurrentActiveDevice == 1)
					pt->nScanDevice = PlcDeviceInit(hwndMainFrame, &pt->local.device, pt->sDualDevice, pt);
				else
					pt->nScanDevice = PlcDeviceInit(hwndMainFrame, &pt->local.device, pt->sScanDevice, pt);

				// 이부분은 추가하지 않으면 최초 통신시도 시 시간초과가 발생한다.
				pt->local.timeout->Reset();
			}
		}
		else {
			if(pt->nScanDevice != 0) {
				PlcDeviceUnInit(&pt->local.device);
				pt->nScanDevice = 0;
			}
		}
	}
}

void CheckPortMemoryClearCondition(GLOBAL_PORT_STRUCT *pt)
{
	if(config.OnPortTimeOut_SetValue_TimeOut < 1)	return;
	
	if(pt->nCountContinueTimeOut >= config.OnPortTimeOut_SetValue_TimeOut) {
		int i;
		for(i = 0; i < pt->local.nBufSizeWORD; i++) {
			PokeWORD(&pt->local, i, config.OnPortTimeOut_SetValue_Value);
		}
		for(i = 0; i < pt->local.nBufSizeDWORD; i++) {
			PokeDWORD(&pt->local, i, config.OnPortTimeOut_SetValue_Value);
		}
		for(i = 0; i < pt->local.nBufSizeFLOAT; i++) {
			PokeFLOAT(&pt->local, i, (float)(config.OnPortTimeOut_SetValue_Value));
		}
	}
}

void StationInfoClearErrorReadComm(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoClearErrorWriteBit(GLOBAL_PORT_STRUCT *pt, int station);
void StationInfoClearErrorWriteWord(GLOBAL_PORT_STRUCT *pt, int station);

//------------------------------------------------------------------------------
//	통신을 해본다.
//------------------------------------------------------------------------------

int CommStatusLocalOne(GLOBAL_PORT_STRUCT *pt)
{
	SCAN_METHOD_STRUCT *sm;
	int retn;
	CString message;

	if(pt->bActiveFlag == 0)	return 0;	// port가 활성화 되어있지 않다.

	if(pt->bDeviceInitialFlag == 0)
	{
		// TCP경우 존재하지 않는 IP의 경우 많은 시간이 걸리므로 각 포트별로 처음 읽을 때 초기화를 하도록 수정 10.0.2

		if(pt->bActiveFlag && !pt->pcDualFile.bActive) {
			// 통신 드라이버 종류를 알아본다.
			pt->nScanDevice = PlcDeviceInit(hwndMainFrame, &pt->local.device, pt->sScanDevice, pt);
			if(!pt->nScanDevice) {
				if(IsLangKorean()) {
					message.Format("알 수 없는 DEVICE 종류입니다.\n%s", pt->sScanDevice);
				}else {
					message.Format("undefined DEVICE type.\n%s", pt->sScanDevice);
				}
				MessageDisplay(message);
			}
		}

		PlcProtocolInitOne(hwndMainFrame, pt, 0);	// 0 = 기본 프로토콜

		pt->bDeviceInitialFlag = 1;
		pt->bInvalidateScreen = 1;	// 화면을 다시 그리도록 한다.
	}

	if(pt->nScanProtocol == 0) {
		return 0;	// Protocol을 모른다.
	}

	// 여기서 컴퓨터 이중화일때 디바이스를 Enable/Disable 한다.
	CheckPcDualDevice(pt);

	if(pt->pcDualFile.bActive) {
		if(!pt->pcDualFile.bThread) {
			void ComputerDualStatus(GLOBAL_PORT_STRUCT *pt);
			ComputerDualStatus(pt);
		}
		if(!pt->pcDualRun.bActiveI)	return 0;	// 내가 활성화가 아닐때는 통신을 하지 않는다.
	}
	
	/*	이 부분을 빼야지 디바이스가 잘못 되었을 때 읽기/쓰기 시간초과가 발생한다.
	if(pt->nScanDevice == 0) {
		return 0;	// Device를 모른다.
	}
	*/

	if(pt->local.bReadingFlag == OFF) {		// 읽기 중이 아닐 때
		if(IsTimeLocalWriteScan(pt)) {
			RunWriteWait(pt);				// 대기중인 write명령이 있으면 실행한다.
		}
	}

	if(pt->bUseDeviceInfo) {	// 디바이스 상태 정보를 체크한다. 2013-6-20일 현재 232의 CTS/DSR/RI/DCD 정보를 읽어온다.
		PlcDeviceGetCommModemStatus(&pt->local.device);
	}

	if(pt->local.nScanMethodHap == 0) {
		return 0;	// READ가 하나도 없다.
	}

	if(pt->local.bReadingFlag == OFF) {		// 읽기 중이 아닐 때 
		if(!PlcDeviceCheckConnecting(&pt->local.device, &pt->tel))	return 0;	// 모뎀이 접속중이 아니다.
	}

	sm = &pt->local.scanMethod[pt->local.nScanPos];

	if(pt->local.bReadingFlag) {	// 읽기가 끝나지 않았으면 계속해서 읽기를 한다.
		retn = PlcProtocolRead(pt, pt->local.nScanPos);
	}
	else {
		// 2007.10.24 지원 통신을 정지하는 기능
		if(pt->bScanPause == 1)			return 0;
		
		// 현재 스캔항목의 통신을 시작한다.
		if(!IsTimeLocalReadScan(pt))	return 0;

		if(sm->active) {	// 유효한 읽기만 실행한다.
			// 통신 시간 초과 발생 시 5회 이상 발생하면 사용자가 설정한 만큼 기다리다 통신을 재개한다.
			if(config.nSkipTimeOnTimeOut > 0 && sm->nTimeOutCount > 4) {
				struct time t;
				gettime(&t);

				if(t.ti_sec == sm->cTimeOutOldSec) {	// check 시간이 변하지 않았으면 돌아간다.
					goto search_next_method;
				}
				
				sm->cTimeOutOldSec = t.ti_sec;
				sm->nTimeOutSkipTime++;
				if(sm->nTimeOutSkipTime <= config.nSkipTimeOnTimeOut) {
					goto search_next_method;
				}
				sm->nTimeOutSkipTime = 0;		// 초기화 하고 통신을 재개한다.
			}			

			if(sm->read_delay_curr < sm->read_delay_target) {	// scan을 쉰다.
				sm->read_delay_curr++;
				goto search_next_method;
			}
			else {
				sm->read_delay_curr = 0;
			}

			if(pt->bCountNextCommTry)
				PlusCommCountReadTry(pt, sm->station);
			else
				pt->bCountNextCommTry = true;

			SetReadingOrWriting(0);	// reading time
			retn = PlcProtocolRead(pt, pt->local.nScanPos);
		} 
		else {
			goto search_next_method;	// 무효화 되어 있는 항목은 실행하지 않고 다음으로 넘어간다.
		}
	}
	
	if(retn == COMMUNICATION_OK) {
		PokeBitSYSTEM(pt, 0x0000, ON);	// 한 순간 timeout이 발생했다.
		PokeBitSYSTEM(pt, 0x0001, ON);	// time out이 5회이상 발생 Flag clear.
		PokeBitSYSTEM(pt, 0x0002, ON);	// time out이 10회이상 발생 Flag clear.
		PokeBitSYSTEM(pt, 0x0003, ON);	// time out이 20회이상 발생 Flag clear.
		PokeBitSYSTEM(pt, 0x0004, ON);	// time out이 30회이상 발생 Flag clear.

		if(pt->nCountContinueTimeOut >= 5) {	// 통신이 재개되면 통신양호 (Duplex System) 2008-6-30 sm->nTimeOutCount에서 pt->nCountContinueTimeOut로 바꿈
		//if(sm->nTimeOutCount >= 5) {	// 통신이 재개되면 통신양호 (Duplex System)
			SystemStatusSetDI(SSMDI_ErrorStatusPlcScanTimeOut, OFF);
		}

		sm->nTimeOutCount = 0;
		pt->nDualTimeOutCount = 0;		// 듀얼 시스템에서 TimeOut을 Clear한다.
		pt->nCountContinueTimeOut = 0;	// 해당포트에서 연속되는 TimeOut

		pt->nDualCodeBadCount = 0;		// 듀얼 시스템에서 Codebad을 Clear한다.

		StationInfoClearErrorReadComm(pt, sm->station);
	}
	else if(retn == COMMUNICATION_OK_NOCOUNT) {
		pt->bCountNextCommTry = false;
	}
	else if(retn == COMMUNICATION_WAITING) {
		
	}
	else if(retn == COMMUNICATION_TIME_OUT) { // 시간 초과 발생.
		PlusCommCountReadTimeOut(pt, sm->station);

		if(pt->nCountContinueTimeOut < 100)	pt->nCountContinueTimeOut++;	// 해당포트에서 연속되는 TimeOut

		if(sm->nTimeOutCount < 100)			sm->nTimeOutCount++;

		PokeBitSYSTEM(pt, 0x0000, OFF);
		
		if(pt->nCountContinueTimeOut > 5)	PokeBitSYSTEM(pt, 0x0001, OFF);	// 1번 bit set
		if(pt->nCountContinueTimeOut > 10)	PokeBitSYSTEM(pt, 0x0002, OFF);	// 2번 bit set
		if(pt->nCountContinueTimeOut > 20)	PokeBitSYSTEM(pt, 0x0003, OFF);	// 3번 bit set
		if(pt->nCountContinueTimeOut > 30)	PokeBitSYSTEM(pt, 0x0004, OFF);	// 4번 bit set
		
		if(config.nItemTimeoutCount > 0) {
			if(sm->nTimeOutCount >= config.nItemTimeoutCount) {
				OneReadItemMemoryClear(pt, sm);
			}
		}

		if(pt->nCountContinueTimeOut >= 5) {
			SystemStatusSetDI(SSMDI_ErrorStatusPlcScanTimeOut, ON);
		}

		if(config.bMessageBoxTimeOut) {
			if(IsLangKorean()) {
				message.Format("읽기 통신시간 초과-port:%d, station:%d, type:%s, address:%d",
					pt->local.no, pt->local.scanMethod[pt->local.nScanPos].station,
					pt->local.scanMethod[pt->local.nScanPos].type,
					pt->local.scanMethod[pt->local.nScanPos].address);
			} else {
				message.Format("Read Communication Time Over-port:%d, station:%d, type:%s, address:%d",
					pt->local.no, pt->local.scanMethod[pt->local.nScanPos].station,
					pt->local.scanMethod[pt->local.nScanPos].type,
					pt->local.scanMethod[pt->local.nScanPos].address);
			}
			MessageDisplay(message);
		}

		if(pt->nDualTimeOutCount < 100)	pt->nDualTimeOutCount++;

		CheckDualSystem(pt);

		CheckPortMemoryClearCondition(pt);
	}
	else if(retn == COMMUNICATION_CODE_BAD) { // 통신 불량 발생.
		PlusCommCountReadCodeBad(pt, sm->station);
		//reading_flag = OFF;
		if(config.bMessageBoxCodeBad) {
			if(IsLangKorean()) {
				message.Format("읽기 통신코드 불량-port:%d, station:%d, type:%s, address:%d",
					pt->local.no, pt->local.scanMethod[pt->local.nScanPos].station,
							pt->local.scanMethod[pt->local.nScanPos].type,
							pt->local.scanMethod[pt->local.nScanPos].address);
			} else {
				message.Format("Read Communication Code Bad-port:%d, station:%d, type:%s, address:%d",
					pt->local.no, pt->local.scanMethod[pt->local.nScanPos].station,
							pt->local.scanMethod[pt->local.nScanPos].type,
							pt->local.scanMethod[pt->local.nScanPos].address);
			}
			MessageDisplay(message);
		}

		if(pt->nDualCodeBadCount < 100)	pt->nDualCodeBadCount++;
		CheckDualSystem(pt);
	}
	else if(retn == COMMUNICATION_ERR_STRING_AND_CODEBAD) {
		PlusCommCountReadCodeBad(pt, sm->station);
		ElseCommunicationMessage(pt->local.no, sm->station, sm->type, sm->address, retn);

		if(pt->nDualCodeBadCount < 100)	pt->nDualCodeBadCount++;
		CheckDualSystem(pt);
	}
	else if(retn == COMMUNICATION_ERR_STRING_AND_TIMEOUT) {
		PlusCommCountReadTimeOut(pt, sm->station);
		ElseCommunicationMessage(pt->local.no, sm->station, sm->type, sm->address, retn);
	}
	else {
		ElseCommunicationMessage(pt->local.no, sm->station, sm->type, sm->address, retn);
	}

	if(retn != COMMUNICATION_WAITING) {	// 통신을 완료하든 에러든 통신이 끝났다.
search_next_method:
		static int nScanPosBeforeVipScan = -1;	// 중간에 VIP scan을 하게되면 현재 Scan을 저장한 후 정상 SCAN일때 사용한다.

		if(pt->nVipScanCount > 0) {	// VIP Scan을 실행한다.
			if(nScanPosBeforeVipScan == -1) {
				nScanPosBeforeVipScan = pt->local.nScanPos;
			}
			pt->local.nScanPos = pt->nVipScanPos;
			pt->nVipScanCount--;
		}
		else {
			if(nScanPosBeforeVipScan != -1) {
				pt->local.nScanPos = nScanPosBeforeVipScan;
				nScanPosBeforeVipScan = -1;
			}

			int save_pos = pt->local.nScanPos;

			while(1) {
				if(pt->local.nScanPos < 0)	pt->local.nScanPos = 0;
				pt->local.nScanPos++;
				pt->local.nScanPos %= pt->local.nScanMethodHap;
				if(save_pos == pt->local.nScanPos)	break;	// 한바퀴 돌았다.

				if(pt->local.scanMethod[pt->local.nScanPos].active)	break; 	// 유효한 읽기만 실행한다.
			}
		}

		pt->local.bReadingFlag = OFF;	// 통신 읽기가 끝났으므로 읽기중 플래그를 없애준다.
	}

	if(retn == COMMUNICATION_OK) {
		pt->nLocalReadScanTimeCurr = 0;	// reading delay count를 off한다.
		return 1;
	}
	else
		return 0;
}

//------------------------------------------------------------------------------
//	통신을 해본다.
//------------------------------------------------------------------------------

static int CommStatusLocal()
{
	WaitMessageLoad();

	if(nPortHap == 0)		return 0;	// Scan.??? 없다.

	static int port = 0;				// 현재 통신을 진행중인 스테이션
	
	GLOBAL_PORT_STRUCT *pt;

	if(nPortHap == 1) {	// 포트가 하나밖에 없을 때는 포트를 바꾸지 않는다.
		port = 0;
	}
	else {
		port %= nPortHap;	// 프로그램 중간에 INIT을 새로하면 port가 nPortHap보다 클 수가 있다.

		if(config.bMultiPortMultiTasking) {	// multi port를 계속해서 읽는다.
			int old_port = port;
			while(1) {
				port++;
				port %= nPortHap;

				if(portBuf[port].bActiveFlag) {
					if(portBuf[port].bActiveThread == OFF)
						break;	// 포트가 살아있고 스레트가 아닌곳만 스캔한다.
				}
				if(port == old_port)			break;	// 한바퀴 돌았다.
			}
		}
		else {	// port 하나가 통신이 끝날 때까지 기다린다. MPMT를 사용하지 않는다.
			if(portBuf[port].local.bReadingFlag == OFF) {	// 읽기가 끝났다.
				port++;
				port %= nPortHap;
			}
		}
	}

	pt = &portBuf[port];

	if(pt->bActiveFlag == 0)	return 0;	// port가 활성화 되어있지 않다.
	if(pt->bActiveThread)		return 0;	// 스레드가 사용중이다.

	return CommStatusLocalOne(pt); 
}

void CommStatus()  
{
	static char flag = OFF; 
	int count;

	if(flag)	return;	// void stack overflow
	flag = ON;
	for(count = 0; count < 2; count++) {	// 통신 정상이면 두번은 시도한다.
		if(CommStatusLocal() == 0)	break;
	}
	flag = OFF;

	return;
}

//------------------------------------------------------------------------------
//	직접 디지털 출력을 살려준다.
//------------------------------------------------------------------------------

static int CommWriteDigitalOutOnOffExtend(GLOBAL_PORT_STRUCT *pt, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(pt->bScanPause == 1)	return COMMUNICATION_OK;

	char message[160];
	int  retn;

	// 실제 통신을 행하여 디지털 출력을 살려주고 또 성공 했을때는 Flag를 살려준다.
	SetReadingOrWriting(1);	// writing time
	retn = PlcProtocolWriteBit(pt, item->port, item->station, item->address, item->sExtraAddr, item->wExtraAddr, (WORD)item->value);

	PlusCommCountWriteBitTry(pt, item->station);

	if(retn == COMMUNICATION_OK) {
		PokeBitSYSTEM(pt, 0x0030, OFF);	// 한 순간 timeout이 발생했다.
		PokeBitSYSTEM(pt, 0x0031, OFF);	// 한 순간 codebad이 발생했다.

		StationInfoClearErrorWriteBit(pt, item->station);
		return retn;
	}
	else if(retn == COMMUNICATION_TIME_OUT) { // 시간초과 발생.
		PokeBitSYSTEM(pt, 0x0030, ON);	// 한 순간 timeout이 발생했다.
		PlusCommCountWriteBitTimeOut(pt, item->station);
		if(IsLangKorean()) {
			wsprintf(message, "Bit쓰기 통신시간 초과-port:%d, station:%d, addr:%04X, extra1:%s",
			item->port, item->station, item->address, item->sExtraAddr);
		} else {
			wsprintf(message, "Write Bit Communication Time Over-port:%d, station:%d, addr:%04X, extra1:%s",
			item->port, item->station, item->address, item->sExtraAddr);
		}
					
		MessageDisplay(message);
	}
	else if(retn == COMMUNICATION_CODE_BAD) { // 통신 불량 발생.
		PokeBitSYSTEM(pt, 0x0031, ON);	// 한 순간 codebad이 발생했다.
		PlusCommCountWriteBitCodeBad(pt, item->station);
		if(IsLangKorean()) {
			wsprintf(message, "Bit쓰기 통신코드 불량-port:%d, station:%d, addr:%04X, extra1:%s",
			item->port, item->station, item->address, item->sExtraAddr);
		} else {
			wsprintf(message, "Write Bit Communication Code Bad-port:%d, station:%d, addr:%04X, extra1:%s",
			item->port, item->station, item->address, item->sExtraAddr);
		}
		MessageDisplay(message);
	}
	else if(retn == COMMUNICATION_NEXT_WRITE_GO) {
		return COMMUNICATION_NEXT_WRITE_GO;
	}
	else if(retn == COMMUNICATION_ERR_STRING_AND_CODEBAD) {
		PlusCommCountWriteBitCodeBad(pt, item->station);
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}
	else if(retn == COMMUNICATION_ERR_STRING_AND_TIMEOUT) {
		PlusCommCountWriteBitTimeOut(pt, item->station);
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}
	else {
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}
	return retn;
}

static int CommWriteAnalogOutputExtend(GLOBAL_PORT_STRUCT *pt, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(pt->bScanPause == 1)	return COMMUNICATION_OK;

	char message[160];
	int  retn;

	// 실제 통신을 행하여 아날로그 값을 보내고 또 성공 했을때는 화면에 값을 보여준다.
	SetReadingOrWriting(1);	// writing time
	retn = PlcProtocolWriteWord(pt, item->port, item->station, item->address, item->sExtraAddr, item->wExtraAddr, item->value);

	PlusCommCountWriteWordTry(pt, item->station);

	if(retn == COMMUNICATION_OK) {
		PokeBitSYSTEM(pt, 0x0030, OFF);	// 한 순간 timeout이 발생했다.
		PokeBitSYSTEM(pt, 0x0031, OFF);	// 한 순간 codebad이 발생했다.

		StationInfoClearErrorWriteWord(pt, item->station);
		return retn;
	}
	else if(retn == COMMUNICATION_TIME_OUT) { // 시간초과 발생.
		PokeBitSYSTEM(pt, 0x0030, ON);	// 한 순간 timeout이 발생했다.
		PlusCommCountWriteWordTimeOut(pt, item->station);
		if(IsLangKorean()) {
			wsprintf(message, "워드 쓰기 통신시간 초과-port:%d, sub station:%d, addr:%X",
			item->port, item->station, item->address);
		} else {
			wsprintf(message, "Write Word Communication Time Over-port:%d, sub station:%d, addr:%X",
			item->port, item->station, item->address);
		}
					
		MessageDisplay(message);
	}
	else if(retn == COMMUNICATION_CODE_BAD) { // 통신 불량 발생.
		PokeBitSYSTEM(pt, 0x0031, ON);	// 한 순간 codebad이 발생했다.
		PlusCommCountWriteWordCodeBad(pt, item->station);
		if(IsLangKorean()) {
			wsprintf(message, "워드 쓰기 통신코드 불량-port:%d, sub station:%d, addr:%03X",
			item->port, item->station, item->address);
		} else {
			wsprintf(message, "Write Word Communication Code Bad-port:%d, sub station:%d, addr:%X",
			item->port, item->station, item->address);
		}
					
		MessageDisplay(message);
	}
	else if(retn == COMMUNICATION_NEXT_WRITE_GO) {
		MinusCommCountWriteWordTry(pt, item->station);
		return COMMUNICATION_NEXT_WRITE_GO;
	}
	else if(retn == COMMUNICATION_ERR_STRING_AND_CODEBAD) {
		PlusCommCountWriteWordCodeBad(pt, item->station);
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}
	else if(retn == COMMUNICATION_ERR_STRING_AND_TIMEOUT) {
		PlusCommCountWriteWordTimeOut(pt, item->station);
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}
	else {
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}

	return retn;
}

// 통신 상황은 문자열 타임아웃이나 코드배드 등이 마련되어 있지 않아서 워드쓰기를 같이 사용한다.
static int CommWriteStringOutputExtend(GLOBAL_PORT_STRUCT *pt, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(pt->bScanPause == 1)	return COMMUNICATION_OK;

	char message[160];
	int  retn;

	// 실제 통신을 행하여 아날로그 값을 보내고 또 성공 했을때는 화면에 값을 보여준다.
	SetReadingOrWriting(1);	// writing time
	retn = PlcProtocolWriteBlock(pt, item->port, item->station, item->address, item->sExtraAddr, item->wExtraAddr, item->arrayValue, (short)item->value, item->array_type);

	PlusCommCountWriteWordTry(pt, item->station);

	if(retn == COMMUNICATION_OK) {
		PokeBitSYSTEM(pt, 0x0030, OFF);	// 한 순간 timeout이 발생했다.
		PokeBitSYSTEM(pt, 0x0031, OFF);	// 한 순간 codebad이 발생했다.

		StationInfoClearErrorWriteWord(pt, item->station);
		return retn;
	}
	else if(retn == COMMUNICATION_TIME_OUT) { // 시간초과 발생.
		PokeBitSYSTEM(pt, 0x0030, ON);	// 한 순간 timeout이 발생했다.
		PlusCommCountWriteWordTimeOut(pt, item->station);
		if(IsLangKorean()) {
			wsprintf(message, "블럭 쓰기 통신시간 초과-port:%d, sub station:%d, addr:%X",
			item->port, item->station, item->address);
		} else {
			wsprintf(message, "Write Block Communication Time Over-port:%d, sub station:%d, addr:%X",
			item->port, item->station, item->address);
		}
					
		MessageDisplay(message);
	}
	else if(retn == COMMUNICATION_CODE_BAD) { // 통신 불량 발생.
		PokeBitSYSTEM(pt, 0x0031, ON);	// 한 순간 codebad이 발생했다.
		PlusCommCountWriteWordCodeBad(pt, item->station);
		if(IsLangKorean()) {
			wsprintf(message, "블럭 쓰기 통신코드 불량-port:%d, sub station:%d, addr:%03X",
			item->port, item->station, item->address);
		} else {
			wsprintf(message, "Write Block Communication Code Bad-port:%d, sub station:%d, addr:%X",
			item->port, item->station, item->address);
		}
					
		MessageDisplay(message);
	}
	else if(retn == COMMUNICATION_NEXT_WRITE_GO) {
		MinusCommCountWriteWordTry(pt, item->station);
		return COMMUNICATION_NEXT_WRITE_GO;
	}
	else if(retn == COMMUNICATION_ERR_STRING_AND_CODEBAD) {
		PlusCommCountWriteWordCodeBad(pt, item->station);
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}
	else if(retn == COMMUNICATION_ERR_STRING_AND_TIMEOUT) {
		PlusCommCountWriteWordTimeOut(pt, item->station);
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}
	else {
		ElseCommunicationMessage(item->port, item->station, item->sExtraAddr, item->address, retn);
	}

	return retn;
}

static int ExecuteLocalDigitalOutCommand(SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(item->port >= nPortHap)	return COMMUNICATION_OK;

	// 실제 통신을 행하여 디지털 출력을 살려주고 또 성공 했을때는 Flag를 살려준다.
	GLOBAL_PORT_STRUCT *pt = &portBuf[item->port];

	PlusCommCountWriteBitTry(pt, item->station);

	if(item->address == 0x0010) {
		if(item->value == ON)	PlcDeviceSetHandConnection(&pt->local.device);
		else					PlcDeviceSetHandDisConnection(&pt->local.device);
	}
	else if(item->address == 0x0011) {
		void PlcDeviceSetAutoConnection(DEVICE_STRUCT *device, TELEPHONE_STRUCT *tel, char type);
		PlcDeviceSetAutoConnection(&pt->local.device, &pt->tel, (char)item->value);
	}
	else if(item->address == 0x0012) {		// 모뎀 오류 클리어  2013-4-23 지원
		void PlcDeviceResetErrorCount(DEVICE_STRUCT *device);
		PlcDeviceResetErrorCount(&pt->local.device);
	}
	else if(item->address == 0x0020) {
		ChangeDualSystem(pt, (char)item->value);
	}
	else if(item->address == 0x0021) {	// 2022-2-10 지원 비활성화시 스레트가 열리지 않으면 이부분은 동작하지 않는다. 그래서 WM_COMMAND에서 직접 받는다.

	}
	else if(item->address == 0x0028) {
		pt->bScanPause = (BYTE)item->value;
		PokeBitSYSTEM(pt, 0x0028, (char)item->value);
	}
	else if(item->address == 0x100100) {
		SCAN_METHOD_STRUCT *sm;
		for(int i = 0; i < pt->local.nScanMethodHap; i++) {
			sm = &pt->local.scanMethod[i];
			if(sm->station == item->station) {
				sm->active = (char)item->value;
			}
		}
	}
	else {
	
	}

	return COMMUNICATION_OK;	
}

static int ExecuteLocalVirtualWriteDigital(SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(item->port >= nPortHap)	return COMMUNICATION_OK;

	// 실제 통신을 행하여 디지털 출력을 살려주고 또 성공 했을때는 Flag를 살려준다.
	GLOBAL_PORT_STRUCT *pt = &portBuf[item->port];

	CString imsi;
	WORD word_pos;
	WORD bit_pos;

	imsi.Format("%X", item->address/16);
	word_pos = atoi(imsi);
	bit_pos = (WORD)(item->address%16);

	WORD value = PeekValueWORD(&pt->local, word_pos);

	if(((WORD)item->value)) {
		value |= WORD_MASK[bit_pos];
	}
	else {
		value &= 0xFFFF-WORD_MASK[bit_pos];
	}
	
	PokeWORD(&pt->local, word_pos, value);
	
	return COMMUNICATION_OK;
}

static int ExecuteLocalVirtualWriteAnalog(SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(item->port >= nPortHap)	return COMMUNICATION_OK;

	// 실제 통신을 행하여 디지털 출력을 살려주고 또 성공 했을때는 Flag를 살려준다.
	GLOBAL_PORT_STRUCT *pt = &portBuf[item->port];

	CString imsi;
	DWORD address;

	imsi.Format("%X", item->address);
	address = atoi(imsi);

	if(item->wExtraAddr == 0) {
		PokeWORD(&pt->local, (WORD)address, MathRound(item->value));
	}
	else if(item->wExtraAddr == 1) {
		PokeFLOAT(&pt->local, (WORD)address, (float)item->value);
	}
	else if(item->wExtraAddr == 2) {
		PokeDWORD(&pt->local, (WORD)address, MathRound(item->value));
	}
	else if(item->wExtraAddr == 3) {
		PokeSTRING(&pt->local, (WORD)address, "");
	}
	else {
		
	}

	return COMMUNICATION_OK;
}

static int ExecuteLocalVirtualWriteString(SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(item->port >= nPortHap)	return COMMUNICATION_OK;

	// 실제 통신을 행하여 디지털 출력을 살려주고 또 성공 했을때는 Flag를 살려준다.
	GLOBAL_PORT_STRUCT *pt = &portBuf[item->port];

	CString imsi;
	DWORD address;

	imsi.Format("%X", item->address);
	address = atoi(imsi);

	if(item->wExtraAddr == 3) {
		if(item->array_type == 11) {
			WCHAR str[256];
			memset(str, 0, sizeof(str));
			int word_count = (int)(item->value/2);
			for(int i = 0; i < word_count; i++) {
				str[i] = item->arrayValue[i*2]+item->arrayValue[i*2+1]*256;
			}
			CString mbs_data = str;
			PokeSTRING(&pt->local, (WORD)address, mbs_data);
		}
	}
	else {
		
	}

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	직접 디지털 출력을 살려준다.
//------------------------------------------------------------------------------

static int CommWriteDigitalOutOnOff(GLOBAL_PORT_STRUCT *pt, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(stricmp(item->sExtraAddr, "#DO#") == 0) {	// 내부 명령어
		ExecuteLocalDigitalOutCommand(item);
		return COMMUNICATION_OK;
	}
	else if(stricmp(item->sExtraAddr, "#MEM#") == 0) {	// 메모리 값만 바꾸기
		ExecuteLocalVirtualWriteDigital(item);
		return COMMUNICATION_OK;
	}
	else;

	return CommWriteDigitalOutOnOffExtend(pt, item);
}

//------------------------------------------------------------------------------
//	직접 아날로그 출력에 값을 쓴다.
//------------------------------------------------------------------------------

static int CommWriteAnalogOutput(GLOBAL_PORT_STRUCT *pt, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(stricmp(item->sExtraAddr, "#MEM#") == 0) {	// 메모리 값만 바꾸기
		ExecuteLocalVirtualWriteAnalog(item);
		return COMMUNICATION_OK;
	}

	return CommWriteAnalogOutputExtend(pt, item);
}

//------------------------------------------------------------------------------
//	직접 블럭 출력에 값을 쓴다.
//------------------------------------------------------------------------------

static int CommWriteStringOutput(GLOBAL_PORT_STRUCT *pt, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	if(stricmp(item->sExtraAddr, "#MEM#") == 0) {	// 메모리 값만 바꾸기
		ExecuteLocalVirtualWriteString(item);
		return COMMUNICATION_OK;
	}

	return CommWriteStringOutputExtend(pt, item);
}

static void ExecuteReadDelayCommand(int port, int station, char *type, int address, int delay_value)
{
	GLOBAL_PORT_STRUCT *pt;
	SCAN_METHOD_STRUCT *sm;
	int p, s;
	int curr_count = 0;
	
	for(p = 0; p < nPortHap; p++) {
		if(port != -1 && port != p)	continue;
		pt = &portBuf[p];

		for(s = 0; s < pt->local.nScanMethodHap; s++) {
			sm = &pt->local.scanMethod[s];
			if(station != -1 && station != sm->station)	continue;
			if(address != -1 && address != (int)sm->address)	continue;
			if(strlen(type) != 0 && strcmp(type, sm->type) != 0)	continue;

			sm->read_delay_target = delay_value;
			
			// 아래는 각각의 SCAN_METHOD의 count를 다르게 설정해서 읽기가 집중되지 않도록 한다.
			sm->read_delay_curr = curr_count;
			if(delay_value > 0) {
				curr_count++;
				curr_count%=(delay_value+1);
			}
		}
	}
}

void SetWriteWaitCount(GLOBAL_PORT_STRUCT *pt)
{
	int hap = pt->blockWriteWait->ring_target-pt->blockWriteWait->ring_current;
	if(hap < 0)	hap += MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;

	PokeWordSYSTEM(pt, 9, hap);	// 남아있는 쓰기 개수
}

// Network Client Virtual 에서 부를 때는 out_port는 Network Client Multi가 되고 item은 자신의 포트가 된다.

static void SetPushing(GLOBAL_PORT_STRUCT *pt)
{
	TimeOutClass timeout;

	while(pt->blockWriteWait->bPoping) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
	}
	pt->blockWriteWait->bPushing = 1;
}

static void ResetPushing(GLOBAL_PORT_STRUCT *pt)
{
	pt->blockWriteWait->bPushing = 0;
}

static void SetPoping(GLOBAL_PORT_STRUCT *pt)
{
	TimeOutClass timeout;

	while(pt->blockWriteWait->bPushing) {
		Sleep(1);
		if(timeout.IsTimeOut(3))	break;
	}
	pt->blockWriteWait->bPoping = 1;
}

static void ResetPoping(GLOBAL_PORT_STRUCT *pt)
{
	pt->blockWriteWait->bPoping = 0;
}

void InsertWriteWaitOne(int out_port, SCAN_WRITE_EXCHANGE_ITEM *item)
{
	SCAN_WRITE_EXCHANGE_ITEM *wait;
	GLOBAL_PORT_STRUCT *pt;

	if(out_port >= nPortHap)	return;	// port number over
	
	pt = &portBuf[out_port];

	if(pt->bInitialFlag == 0)	return;	// 아직 초기화 중이므로 항목을 채워서는 안된다.

	int next_pos;

	if(item->command == 0) {	// Bit write
		if(config.bUseNewValueOnDigitalOut) {
			// 디지털은 반드시 뒤에서부터 검사해야 한다. (중요)
			SetPushing(pt);	// 꺼내기가 끝난다음
			if(pt->blockWriteWait->ring_current != pt->blockWriteWait->ring_target)	{	// 대기 항목이 있을때만
				next_pos = (pt->blockWriteWait->ring_target)%MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;
				while(true) {
					wait = &pt->blockWriteWait->item[next_pos];

					if(	wait->command == 0 &&	// Bit write
						wait->port == item->port &&
						wait->station == item->station &&
						wait->address == item->address &&
						strcmp(wait->sExtraAddr, item->sExtraAddr) == 0 &&
						wait->wExtraAddr == item->wExtraAddr) {
						
						if(wait->value == item->value) {	// 이미 같은 항목이 있다.
							ResetPushing(pt);		
							return;
						}
					}

					next_pos--;
					if(next_pos < 0)	next_pos += MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;
					next_pos %= MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;
					if(next_pos == pt->blockWriteWait->ring_current)	break;
				}
			}
			ResetPushing(pt);
		}
	}
	else if(item->command == 1) {	// Word write
		if(config.bUseNewValueOnAnalogOut) {
			SetPushing(pt);
			if(pt->blockWriteWait->ring_current != pt->blockWriteWait->ring_target) {
				next_pos = (pt->blockWriteWait->ring_current+1)%MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;
				while(true) {
					wait = &pt->blockWriteWait->item[next_pos];

					if(	wait->command == 1 &&	// Word write
						wait->port == item->port &&
						wait->station == item->station &&
						wait->address == item->address &&
						strcmp(wait->sExtraAddr, item->sExtraAddr) == 0 &&
						wait->wExtraAddr == item->wExtraAddr) {
						
						wait->value = item->value;
						
						ResetPushing(pt);

						return;
					}

					if(next_pos == pt->blockWriteWait->ring_target)	break;
					next_pos = (next_pos+1)%MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;
				}
			}
			ResetPushing(pt);
		}
	}
	else if(item->command == 100) {	// read scan delay
		ExecuteReadDelayCommand(item->port, item->station, item->sExtraAddr, item->address, (int)item->value);
		return;
	}
	else;

	next_pos = (pt->blockWriteWait->ring_target+1)%MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;

	if(next_pos == pt->blockWriteWait->ring_current) {
		CString message;
		if(IsLangKorean()) {
			message.Format("쓰기 대기중인 항목이 너무 많습니다.(Port No = %d)", pt->local.no);
		}else {
			message.Format("Write items are too many >= 1000. (Port No = %d)", pt->local.no);
		}
		MessageDisplay(message);
		return;
	}

	memcpy(&pt->blockWriteWait->item[next_pos], item, sizeof(SCAN_WRITE_EXCHANGE_ITEM));

	pt->blockWriteWait->ring_target = next_pos;

	SetWriteWaitCount(pt);
}

static void InsertWriteWaitFromRing(SCAN_WRITE_EXCHANGE_INFO *exchange)
{
	// 같은 write 항목이 발견되면 값을 새로이 교체하고 return 한다.
	if(exchange->struct_size != sizeof(SCAN_WRITE_EXCHANGE_INFO)) {
		return;
	}

	SCAN_WRITE_EXCHANGE_ITEM *item;
	int i;
	int next_pos;

	for(i = 0; i < MAX_SCAN_WRITE_EXCHANGE_ITEM_COUNT; i++) {
		if(exchange->ring_current == exchange->ring_target)	break;

		next_pos = (exchange->ring_current+1)%MAX_SCAN_WRITE_EXCHANGE_ITEM_COUNT;
		
		item = &exchange->item[next_pos];

		if((item->port) >= nPortHap) {
			
		}
		else {
			InsertWriteWaitOne(item->port, item);
		}
		exchange->ring_current = next_pos;
	}
}

int AddWaitWriteDigitalOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag)
{
	SCAN_WRITE_EXCHANGE_ITEM item;

	memset(&item, 0, sizeof(SCAN_WRITE_EXCHANGE_ITEM));

	item.command = 0;	// DO
	item.port = port;
	item.station = station;
	item.address = address;
	strcpy(item.sExtraAddr, sExtraAddr);
	item.wExtraAddr = wExtraAddr;
	item.value = flag;
	item.bVipScanFlag = OFF;

	InsertWriteWaitOne(port, &item);

	return 1;	
}

int AddWaitWriteAnalogOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr, double value)
{
	SCAN_WRITE_EXCHANGE_ITEM item;

	memset(&item, 0, sizeof(SCAN_WRITE_EXCHANGE_ITEM));

	item.command = 1;	// AO
	item.port = port;
	item.station = station;
	item.address = address;
	strcpy(item.sExtraAddr, sExtraAddr);
	item.wExtraAddr = wExtraAddr;
	item.value = value;
	item.bVipScanFlag = OFF;
		
	InsertWriteWaitOne(port, &item);

	return 1;	
}

void VipScanRegister(SCAN_WRITE_EXCHANGE_ITEM *vip)
{
	if(vip->bVipScanFlag == OFF)		return;
	if(config.nVipScanTryCount == 0)	return;	// VIP SCAN을 하지 않는다.

	if(vip->wVipScanMemoryPort >= nPortHap)	return;

	GLOBAL_PORT_STRUCT *pt = &portBuf[vip->wVipScanMemoryPort];
	SCAN_METHOD_STRUCT *sm;
	int i;

	for(i = 0; i < pt->local.nScanMethodHap; i++) {
		sm = &pt->local.scanMethod[i];
		if(sm->cVarType != vip->cVipScanMemoryType)			continue;	// 다른 메모리
		if(vip->wVipScanMemoryPos < sm->target)				continue;
        if(vip->wVipScanMemoryPos >= sm->target+sm->size)	continue;

		if(sm->active == OFF)	break;	// 죽어있는 SCAN

		// 해당 sm을 찾았다.
		pt->nVipScanCount = config.nVipScanTryCount;
		pt->nVipScanPos = i;
		break;
	}
}

//-----------------------------------------------------------------------------------
//	대기중인 쓰기 명령이 있으면 쓰기를 한다.
//-----------------------------------------------------------------------------------

void RunWriteWait(GLOBAL_PORT_STRUCT *pt)
{
	SCAN_WRITE_EXCHANGE_ITEM wait;
	int retn;
	//static int retry_count_on_timeout = 0;
	
next_write:
	
	if(pt->blockWriteWait->ring_current == pt->blockWriteWait->ring_target) {
		pt->retry_count_on_timeout = 0;
		return;
	}

	int next_pos;
	
	for(int i = 0; i < MAX_SCAN_WRITE_LOCAL_ITEM_COUNT; i++) {
		SetPoping(pt);
		if(pt->blockWriteWait->ring_current == pt->blockWriteWait->ring_target) break;
		next_pos = (pt->blockWriteWait->ring_current+1)%MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;
		memcpy(&wait, &pt->blockWriteWait->item[next_pos], sizeof(SCAN_WRITE_EXCHANGE_ITEM));
		ResetPoping(pt);

		if(wait.command == 0) {	// bit write
			retn = CommWriteDigitalOutOnOff(pt, &wait);
		}
		else if(wait.command == 1) {	// word write
			retn = CommWriteAnalogOutput(pt, &wait);
		}
		else if(wait.command == 2) {	// string write
			retn = CommWriteStringOutput(pt, &wait);
		}
		else {
			retn = COMMUNICATION_OK;
		}

		if(retn == COMMUNICATION_TIME_OUT) {
			if(pt->retry_count_on_timeout < config.nRetryCountOnWriteTimeOut) {
				pt->retry_count_on_timeout++;
				return;
			}
		}

		pt->retry_count_on_timeout = 0;

		SetPoping(pt);
		pt->blockWriteWait->ring_current = next_pos;
		ResetPoping(pt);

		SetWriteWaitCount(pt);

		if(retn == COMMUNICATION_NEXT_WRITE_GO)	goto next_write;

		VipScanRegister(&wait);

		return;	// 하나를 썼으면 돌아간다.
				// 계속 하는 경우는 NEXT_WRITE_GO(-1)를 실행할 때이다.
	}

	ResetPoping(pt);
}



static SCAN_WRITE_EXCHANGE_INFO *share_Main_PlcScan;
static HANDLE hThreadInsert = NULL;
static DWORD  idThreadInsert = 0;
static char   bThreadFlag;

static char bThreadingFlag = OFF;
static HANDLE hEventByNetServer;
static SharedMemory shareNetserverToPlcscanClass;

static void InsertWriteCommand(NetWorkProtocolRecv *recv) 
{
	SCAN_WRITE_EXCHANGE_ITEM item;

	memset(&item, 0, sizeof(SCAN_WRITE_EXCHANGE_ITEM));
	item.port = recv->nPort;	
	item.station = recv->nStation;	
	item.address = recv->dwAddress;
	strcpy(item.sExtraAddr, recv->sExtra1);
	item.wExtraAddr = recv->wExtra2;
	item.value = recv->fValue;

	if(recv->wCommand == COMMAND_PLCSCAN_WRITE_BIT) {
		item.command = 0;	// 0 = bit write
		InsertWriteWaitOne(item.port, &item);	
	}
	else if(recv->wCommand == COMMAND_PLCSCAN_WRITE_WORD) {
		item.command = 1;		// 1 = word write
		InsertWriteWaitOne(item.port, &item);	
	}
	else
		return;
}	

static DWORD WINAPI ProcEventRecv(LPVOID)
{
	bThreadFlag = ON;
	bThreadingFlag = ON;
	
	while(bThreadFlag) { 
		Sleep(1);

		if(share_Main_PlcScan) {	// main에서 오는 쓰기 명령을 받는다.
			InsertWriteWaitFromRing(share_Main_PlcScan);
		}

		if(hEventByNetServer && shareNetserverToPlcscanClass.ptr) {
			if(WaitForSingleObject(hEventByNetServer, 1) == WAIT_OBJECT_0) {	// yes signal
				SHARE_PLCSCAN_NETWORK *share;
				NetWorkProtocolRecv recv;

				share = (SHARE_PLCSCAN_NETWORK *)shareNetserverToPlcscanClass.ptr;

				recv.Split(share->buf, share->size);

				InsertWriteCommand(&recv);

				ResetEvent(hEventByNetServer);
			}
		}
	}

	bThreadingFlag = OFF;

	return 0;
}

//---------------------------
//	통신 DDE 서버를 설치한다.
//---------------------------

void PlcScanDdeInit()
{
	HANDLE hmmf = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(SCAN_WRITE_EXCHANGE_INFO), "SHARE_MAIN_PLCSCAN");
	if(hmmf == NULL) {
		
	}
	else {
		share_Main_PlcScan = (SCAN_WRITE_EXCHANGE_INFO*)MapViewOfFile(hmmf, FILE_MAP_WRITE, 0, 0, 0);
		if(share_Main_PlcScan != NULL) {
			share_Main_PlcScan->ring_current = share_Main_PlcScan->ring_target;
			// 감시 프로그램에서 값을 쌓아 놓았을 경우 클리어 효과
		}
	}

	hThreadInsert = CreateThread(NULL, 0, ProcEventRecv, NULL, 0, &idThreadInsert);

	hEventByNetServer = CreateEvent(NULL, TRUE, FALSE, "EventNetServerToPlcScan");
	shareNetserverToPlcscanClass.Init("ShareNetServerPlcScan", sizeof(SHARE_PLCSCAN_NETWORK));
}

//-----------------------
//	통신 DDE를 제거한다.
//-----------------------

void PlcScanDdeUnInit()
{
	if(hEventByNetServer)	CloseHandle(hEventByNetServer);
	shareNetserverToPlcscanClass.Uninit();
	bThreadFlag = OFF;

	TimeOutClass timeout;

	while(bThreadingFlag) {
		if(timeout.IsTimeOut(3))	break;
	}

	if(share_Main_PlcScan != NULL) {
		UnmapViewOfFile(share_Main_PlcScan);
		share_Main_PlcScan = NULL;
	}
}

// english O.K
#include "stdafx.h"
#include <kwl\kdialog.h>
#include <dataswap.h>

#include "plc_scan.h"
#include "resource.h"
#include "protocol\pro_main.h"
#include "..\catlib.src\SystemStatusMemory.h"

#include "DialogPortEdit.h"

void ScanPortInitOne(HWND hwnd, int port);
void ScanPortUnInitOne(int port);

int PortSelect(int &port);

extern int  nSaveViewPort;

void RestartOnePort(int port)
{
	void ScanServerPause(char flag);

	ScanServerPause(ON);

	SendMessage(hwndMainFrame, WM_COMMAND, IDM_CLOSEALL, 0L);
	ScanPortUnInitOne(port);
	ScanPortInitOne(hwndMainFrame, port);

	if(port != nSaveViewPort) {		// 설정한 포트로 바꿔준다.
		GLOBAL_PORT_STRUCT *pt = &portBuf[port];
		if(pt->bActiveFlag) {
			nSaveViewPort = port;
		}
	}

	PostMessage(hwndMainFrame, WM_COMMAND, IDM_VIEW_SCANMEMORY, 0L);

	ScanServerPause(OFF);

	SystemStatusSetDI(SSMDI_PlcScanEventToLocalMain, 1);	// 감시프로그램에 PlcScan의 상황이 바뀐것을 알려준다.
	SystemStatusSetDI(SSMDI_PlcScanEventToGateway, 1);		// Gateway 프로그램에 PlcScan의 상황이 바뀐것을 알려준다.
}

void ScanFileEdit(HWND hwnd)
{
	CDialogPortEdit dialog;
	char buf[160];

	static int  port = 0;

	if(!PortSelect(port))	return;	

	dialog.nPort = port;

	GLOBAL_PORT_STRUCT *pt = &portBuf[port];
	SCAN_METHOD_STRUCT *sm;
	int i;

	dialog.bActiveFlag = pt->bActiveFlag;
	dialog.sDescription = pt->sTitle;
	dialog.wBufLengthWORD = pt->local.nBufSizeWORD;
	dialog.wBufLengthFLOAT = pt->local.nBufSizeFLOAT;
	dialog.wBufLengthDWORD = pt->local.nBufSizeDWORD;
	dialog.wBufLengthSTRING = pt->local.nBufSizeSTRING;
	dialog.wBufLengthDOUBLE = pt->local.nBufSizeDOUBLE;
	dialog.wBufLengthINT64 = pt->local.nBufSizeINT64;
	dialog.sDevice = pt->sScanDevice;
	strcpy(dialog.sProtocol, pt->sScanProtocol);
	dialog.sProtocolOption = pt->sScanProtocolOption[0];
	dialog.sDualProtocolOption = pt->sScanProtocolOption[1];
	
	//10.2부터는 Mili Sec로 바뀌었다.
	//dialog.wTimeOutRead = pt->local.MAX_TIME_OUT_READ;
	//dialog.wTimeOutWrite = pt->local.MAX_TIME_OUT_WRITE;
	dialog.wTimeOutRead = pt->local.TIMEOUT_MILLI_READ;
	dialog.wTimeOutWrite = pt->local.TIMEOUT_MILLI_WRITE;

	dialog.nTelConnectCicle   = pt->tel.nConnectCicle;
	dialog.nTelConnectingTime = pt->tel.nConnectingTime;
	dialog.bTelAutoConnection = pt->tel.bAutoConnection;
	dialog.nTelConnectingTimeOnManual = pt->tel.nConnectingTimeOnManual;

	dialog.bDualActive = pt->bDualActive;
	strcpy(dialog.sDualDevice, pt->sDualDevice);
	dialog.nDualCauseTimeOut = pt->nDualCauseTimeOut;
	dialog.nDualCauseCodeBad = pt->nDualCauseCodeBad;
	dialog.bDualUseProtocol = pt->bDualUseProtocol;
	strcpy(dialog.sDualProtocol, pt->sDualProtocol);

	dialog.m_nReadScanTime = pt->local.nLocalReadScanTime;
	dialog.m_nWriteScanTime = pt->local.nLocalWriteScanTime;

	strcpy(dialog.sTelNumber, pt->tel.sTelNumber);

	dialog.m_bActiveThread = pt->bActiveThread;
	dialog.m_nThreadCycle = pt->nThreadCycle;

	dialog.m_bComputerDualActive = pt->pcDualFile.bActive;
	memcpy(&dialog.pcDual, &pt->pcDualFile, sizeof(COMPUTER_DUAL_FILE));

	dialog.m_bUseStationInfo = pt->stationInfo.bUse;
	dialog.m_bUseDeviceInfo = pt->bUseDeviceInfo;

	dialog.editBuf[0] = 0;

	for(i = 0; i < pt->local.nScanMethodHap; i++) {
		sm = &pt->local.scanMethod[i];
	
		if(i != 0) {
			strcat(dialog.editBuf, "\r\n");
		}
		if(!sm->active) {
			strcat(dialog.editBuf, ";");
		}
		if(sm->cVarType == 0) {
			strcat(dialog.editBuf, "READ,  ");
		}
		else if(sm->cVarType == 1) {
			strcat(dialog.editBuf, "FLOAT, ");
		}
		else if(sm->cVarType == 2) {
			strcat(dialog.editBuf, "DWORD, ");
		}
		else if(sm->cVarType == 3) {
			strcat(dialog.editBuf, "STRING,");
		}
		else if(sm->cVarType == 4) {
			strcat(dialog.editBuf, "DOUBLE,");
		}
		else if(sm->cVarType == 5) {
			strcat(dialog.editBuf, "INT64, ");
		}
		else {
			strcat(dialog.editBuf, "READ,  ");
		}

		sprintf(buf, " %3s, %5s,", sm->sStation, sm->type);
		strcat(dialog.editBuf, buf);

		if(sm->bHexAddress)
			sprintf(buf, " %4Xh,", sm->address);
		else
			sprintf(buf, "  %4d,", sm->address);
		strcat(dialog.editBuf, buf);

		sprintf(buf, " %4d, %3d,", sm->target, sm->size);
		strcat(dialog.editBuf, buf);

		//sprintf(buf, " %3s, %5s, %4d, %4d, %3d,", sm->sStation, sm->type, sm->address, sm->target, sm->size);

		if(sm->extra2 || sm->extra3) {	// SIZE 다음이 0이 아닐 때.
			sprintf(buf, " %3d, %3d,", sm->extra2, sm->extra3);
			strcat(dialog.editBuf, buf);
		}

		if(strlen(dialog.editBuf) >= 30000-160)	break;
	}

	if(dialog.DoModal() == IDOK) {
		RestartOnePort(port);
	}
}
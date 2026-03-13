// english O.K
#include "stdafx.h"
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <io.h>

#include <tools.h>
#include <glib.h>
#include <crc.hpp>

#include "..\catlib.src\SystemStatusMemory.h"
#include "..\catlib.src\pub_msg.h"

#include "plc_scan.h"

GLOBAL_PORT_STRUCT *portBuf = NULL;
int	nPortHap = 0;	

void MapScanMemory(GLOBAL_PORT_STRUCT *pt)
{
	char name[80];
	HANDLE hmmfInfo;
	int i;

	if(pt->local.no > 255) {
		int a = 1;
	}

	pt->bufSYSTEM = NULL;
	pt->local.bufWORD = NULL;
	pt->local.bufFLOAT = NULL;
	pt->local.bufDWORD = NULL;
	pt->local.bufSTRING = NULL;
	pt->local.bufDOUBLE = NULL;
	pt->local.bufINT64 = NULL;
	pt->hmmfNumber++;

	sprintf(name, "PlcScanPort%03d_Information", pt->local.no);
	hmmfInfo = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(PLC_SCAN_PORT_INFO), name);
	if(hmmfInfo == NULL)	return;	//할당할 수 없다.
	else {
		PLC_SCAN_PORT_INFO *info;
		info = (PLC_SCAN_PORT_INFO*)MapViewOfFile(hmmfInfo, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		info->nCountWORD = pt->local.nBufSizeWORD;
		info->nCountFLOAT = pt->local.nBufSizeFLOAT;
		info->nCountDWORD = pt->local.nBufSizeDWORD;
		info->nCountSTRING = pt->local.nBufSizeSTRING;
		info->nCountSYSTEM = MAX_ATTR_WORD;
		info->nCountDOUBLE = pt->local.nBufSizeDOUBLE;
		info->nCountINT64 = pt->local.nBufSizeINT64;
		info->hmmfNumber = pt->hmmfNumber;
		info->crc = GetCRC_SumWORD(info, sizeof(PLC_SCAN_PORT_INFO)-2);
	}

	if(pt->local.nBufSizeWORD > 0) {
		sprintf(name, "PlcScanPort%03d_MemoryWORD%d", pt->local.no, pt->hmmfNumber);
		pt->hmmfWORD = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(WORD_BUF)*pt->local.nBufSizeWORD, name);
		if(pt->hmmfWORD != NULL) {
			pt->local.bufWORD = (WORD_BUF*)MapViewOfFile(pt->hmmfWORD, FILE_MAP_WRITE, 0, 0, 0);
			for(i = 0; i < pt->local.nBufSizeWORD; i++) {
				pt->local.bufWORD[i].flag = OFF;
			}
		}
	}
	if(pt->local.nBufSizeFLOAT > 0) {
		sprintf(name, "PlcScanPort%03d_MemoryFLOAT%d", pt->local.no, pt->hmmfNumber);
		pt->hmmfFLOAT = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(FLOAT_BUF)*pt->local.nBufSizeFLOAT, name);
		if(pt->hmmfFLOAT != NULL) {
			pt->local.bufFLOAT = (FLOAT_BUF*)MapViewOfFile(pt->hmmfFLOAT, FILE_MAP_WRITE, 0, 0, 0);
			for(i = 0; i < pt->local.nBufSizeFLOAT; i++) {
				pt->local.bufFLOAT[i].flag = OFF;
			}
		}
	}
	if(pt->local.nBufSizeDWORD > 0) {
		sprintf(name, "PlcScanPort%03d_MemoryDWORD%d", pt->local.no, pt->hmmfNumber);
		pt->hmmfDWORD = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(DWORD_BUF)*pt->local.nBufSizeDWORD, name);
		if(pt->hmmfDWORD != NULL) {
			pt->local.bufDWORD = (DWORD_BUF*)MapViewOfFile(pt->hmmfDWORD, FILE_MAP_WRITE, 0, 0, 0);
			for(i = 0; i < pt->local.nBufSizeDWORD; i++) {
				pt->local.bufDWORD[i].flag = OFF;
			}
		}
	}
	if(pt->local.nBufSizeSTRING > 0) {
		sprintf(name, "PlcScanPort%03d_MemorySTRING%d", pt->local.no, pt->hmmfNumber);
		pt->hmmfSTRING = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(STRING_BUF)*pt->local.nBufSizeSTRING, name);
		if(pt->hmmfSTRING != NULL) {
			pt->local.bufSTRING = (STRING_BUF*)MapViewOfFile(pt->hmmfSTRING, FILE_MAP_WRITE, 0, 0, 0);
			for(i = 0; i < pt->local.nBufSizeSTRING; i++) {
				pt->local.bufSTRING[i].flag = OFF;
			}
		}
	}
	if(MAX_ATTR_WORD > 0) {
		sprintf(name, "PlcScanPort%03d_MemorySYSTEM%d", pt->local.no, pt->hmmfNumber);
		pt->hmmfSYSTEM = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(SYSTEM_BUF)*MAX_ATTR_WORD, name);
		if(pt->hmmfSYSTEM != NULL) {
			pt->bufSYSTEM = (SYSTEM_BUF*)MapViewOfFile(pt->hmmfSYSTEM, FILE_MAP_WRITE, 0, 0, 0);
		}
	}
	if(pt->local.nBufSizeDOUBLE > 0) {
		sprintf(name, "PlcScanPort%03d_MemoryDOUBLE%d", pt->local.no, pt->hmmfNumber);
		pt->hmmfDOUBLE = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(DOUBLE_BUF)*pt->local.nBufSizeDOUBLE, name);
		if(pt->hmmfDOUBLE != NULL) {
			pt->local.bufDOUBLE = (DOUBLE_BUF*)MapViewOfFile(pt->hmmfDOUBLE, FILE_MAP_WRITE, 0, 0, 0);
			for(i = 0; i < pt->local.nBufSizeDOUBLE; i++) {
				pt->local.bufDOUBLE[i].flag = OFF;
			}
		}
	}
	if(pt->local.nBufSizeINT64 > 0) {
		sprintf(name, "PlcScanPort%03d_MemoryINT64%d", pt->local.no, pt->hmmfNumber);
		pt->hmmfINT64 = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(INT64_BUF)*pt->local.nBufSizeINT64, name);
		if(pt->hmmfINT64 != NULL) {
			pt->local.bufINT64 = (INT64_BUF*)MapViewOfFile(pt->hmmfINT64, FILE_MAP_WRITE, 0, 0, 0);
			for(i = 0; i < pt->local.nBufSizeINT64; i++) {
				pt->local.bufINT64[i].flag = OFF;
			}
		}
	}

	if(pt->bActiveFlag)
		PokeBitSYSTEM(pt, 0x0021, 1);	// 메모리가 활성화 되면 Port (Active)Enable 정보를 해준다. 2022-2-9 추가.
}

void PlcScanMemoryUninit(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->local.bufWORD != NULL) {	
		UnmapViewOfFile(pt->local.bufWORD);
		pt->local.bufWORD = NULL;
		CloseHandle(pt->hmmfWORD);
		pt->hmmfWORD = NULL;
	}
	if(pt->local.bufFLOAT != NULL) {	
		UnmapViewOfFile(pt->local.bufFLOAT);
		pt->local.bufFLOAT = NULL;
		CloseHandle(pt->hmmfFLOAT);
		pt->hmmfFLOAT = NULL;
	}
	if(pt->local.bufDWORD != NULL) {	
		UnmapViewOfFile(pt->local.bufDWORD);
		pt->local.bufDWORD = NULL;
		CloseHandle(pt->hmmfDWORD);
		pt->hmmfDWORD = NULL;
	}
	if(pt->local.bufSTRING != NULL) {	
		UnmapViewOfFile(pt->local.bufSTRING);
		pt->local.bufSTRING = NULL;
		CloseHandle(pt->hmmfSTRING);
		pt->hmmfSTRING = NULL;
	}
	if(pt->bufSYSTEM != NULL) {
		PokeBitSYSTEM(pt, 0x0021, 0);	// 메모리가 비활성화 되면 감시 프로그램이 알수 있도록 Port Disable 정보를 해준다. 2022-2-9 추가.
		UnmapViewOfFile(pt->bufSYSTEM);
		pt->bufSYSTEM = NULL;
		CloseHandle(pt->hmmfSYSTEM);
		pt->hmmfSYSTEM = NULL;
	}

	if(pt->local.bufDOUBLE != NULL) {	
		UnmapViewOfFile(pt->local.bufDOUBLE);
		pt->local.bufDOUBLE = NULL;
		CloseHandle(pt->hmmfDOUBLE);
		pt->hmmfDOUBLE = NULL;
	}
	if(pt->local.bufINT64 != NULL) {	
		UnmapViewOfFile(pt->local.bufINT64);
		pt->local.bufINT64 = NULL;
		CloseHandle(pt->hmmfINT64);
		pt->hmmfINT64 = NULL;
	}
}

static DWORD ConvertStringToStation(char *string, char &bHex)
{
	DWORD value = 0;
	bHex = OFF;

	int size = strlen(string);
	if(size == 0)	return 0;

	if(string[size-1] == 'h' || string[size-1] == 'H') {
		bHex = ON;
		char imsi[80];
		strcpy(imsi, string);
		imsi[size-1] = 0;
		CommaBlockString comma;
		comma.Set(imsi);
		comma.GetHexDWORD(value);
	}
	else {
		bHex = OFF;
		value = atoi(string);
	}

	return value;
}

static bool AddOneScanMethod(HWND hwnd, GLOBAL_PORT_STRUCT *pt, Block& block, CommaBlockString& commaBuf, char *imsi, char vartype)
{
	SCAN_METHOD_STRUCT sm;
	char saddress[100];

	memset(&sm, 0, sizeof(SCAN_METHOD_STRUCT));
	sm.cVarType = vartype;	
	commaBuf.GetString(sm.sStation, sizeof(sm.sStation));
	sm.station = (WORD)ConvertStringToStation(sm.sStation, sm.bHexStation);
	commaBuf.GetString(sm.type, sizeof(sm.type));
	
	commaBuf.GetString(saddress, sizeof(saddress));
	sm.address = ConvertStringToStation(saddress, sm.bHexAddress);
	commaBuf.GetWORD(sm.target);
	commaBuf.GetWORD(sm.size);
	commaBuf.GetDWORD(sm.extra2);
	commaBuf.GetDWORD(sm.extra3);

	if(imsi[0] == ';')	sm.active = OFF;
	else				sm.active = ON;

	if(block.AddBlock((BYTE*)&sm)) {
		pt->local.nScanMethodHap++;
	}
	else {
		MessageBox(hwnd, "Scan Read method add unable because memory insufficent.", "CommDeviceRead", MB_OK);
		return false;
	}

	return true;
}

static void LoadHashedConfig(GLOBAL_PORT_STRUCT *pt)
{
	pt->pCC = new CryptoCommunication();

    CString filename;

	filename.Format("%s\\SCAN\\Config_%03d.cfg", sDirWorkProject, pt->local.no);

    if (!IsFileExists(filename)) return;

    CString one_line;
    CommaBlockString comma;//CommaTextReader comma = new CommaTextReader();
    CString command;
    CryptoTextReader *ctr;

    //try
    //{
        ctr = new CryptoTextReader(filename);
    //}
    //catch (Exception exception)
    //{
        //MessageBox.Show(exception.Message, filename);
        //return;
    //}

    while (true)
    {
        if(!ctr->ReadLine(one_line))	break;

        comma.Set(one_line);
        comma.GetString(command);

		if(pt->pCC->IsCommand(command))
			pt->pCC->Load(ctr);
    }
    ctr->Close();
	delete ctr;
}

static bool IsVersionEqualOrHigher(int file_major, int file_minor, int file_build, int file_revision, int major, int minor, int build, int revision)
{
    if (file_major > major) return true;
    if (file_major < major) return false;
    if (file_minor > minor) return true;
    if (file_minor < minor) return false;
    if (file_build > build) return true;
    if (file_build < build) return false;
    if (file_revision >= revision) return true;
    
    return false;
}

static void CommDeviceRead(HWND hwnd, int port)
{
	char filename[MAXPATH];
	FILE *in;
	char buf[200];
	SCAN_METHOD_STRUCT sm;
	int  line = 0;
	CString message;
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];
	char imsi[100];
	Block block;
	int  i;
	CommaBlockString commaBuf;

	// FileVersion 10.3.3 버전까지는 버전정보가 파일에 없다.  파일 정보가 없는것은 10.3.3 이하 버전이다.
	int FileVersionMajor = 10;
	int FileVersionMinor = 3;
	int FileVersionBuild = 3;

	pt->bActiveFlag = OFF;
	pt->local.MAX_TIME_OUT_READ = 2;				// 통신시 사용하는 MAX_TIME_OUT
	pt->local.MAX_TIME_OUT_WRITE = 2;				// 통신시 사용하는 MAX_TIME_OUT
	pt->local.TIMEOUT_MILLI_READ = 2000;			// 통신시 사용하는 MAX_TIME_OUT
	pt->local.TIMEOUT_MILLI_WRITE = 2000;			// 통신시 사용하는 MAX_TIME_OUT

	pt->local.bufWORD = NULL;
	pt->local.nBufSizeWORD = 100;

	pt->local.bufFLOAT = NULL;
	pt->local.nBufSizeFLOAT = 0;

	pt->local.bufDWORD = NULL;
	pt->local.nBufSizeDWORD = 0;

	pt->local.bufSTRING = NULL;
	pt->local.nBufSizeSTRING = 0;

	pt->local.bufDOUBLE = NULL;
	pt->local.nBufSizeDOUBLE = 0;

	pt->local.bufINT64 = NULL;
	pt->local.nBufSizeINT64 = 0;

	pt->local.scanMethod = NULL;
	pt->local.nScanMethodHap = 0;

	memset(&pt->countAll, 0, sizeof(DEVICE_COUNT_STRUCT));
	memset(&pt->countDevice[0], 0, sizeof(DEVICE_COUNT_STRUCT));
	memset(&pt->countDevice[1], 0, sizeof(DEVICE_COUNT_STRUCT));

	pt->nScanProtocol = 0;					// 프로토콜 종류(0 - debug)
	pt->nScanDevice = 0;            		// 통신 디바이스 종류(0)
	pt->local.nLocalReadScanTime = 0;
	pt->local.nLocalWriteScanTime = 0;
	pt->local.no = port;
	pt->sTitle[0] = 0;
	pt->local.hLocalProtocol = NULL;
	pt->local.timeout = new TimeOutClass;
	pt->tel.sTelNumber[0] = 0;
	pt->tel.nConnectCicle = 60;		// min
	pt->tel.nConnectingTime = 60;	// sec
	pt->tel.nConnectingTimeOnManual = 60;	// sec
	pt->tel.bAutoConnection = ON;	// ON
	pt->nDualCauseTimeOut = 1;
	pt->nDualCauseCodeBad = 0;
	pt->bActiveThread = 1;			// 9.3.0 부터는 기본으로 스레드를 ON한다.
	gettime(&pt->tLocalReadScanTimeOld);
	gettime(&pt->tLocalWriteScanTimeOld);

	ZeroMemory(&pt->pcDualFile, sizeof(COMPUTER_DUAL_FILE));
	pt->pcDualFile.nPort = 6700+port;

	LoadHashedConfig(pt);

	sprintf(filename, "%s\\SCAN\\SCAN.%03d", sDirWorkProject, port);

	in = fopen(filename, "rb");
	if(in == NULL) 	return;

	pt->bActiveFlag = ON;	// default는 port ON
	pt->local.nBufSizeWORD = 200;	// 설정하지 않았으면 200이 default

	pt->bCountNextCommTry = true;

	block.SetBlockSize(sizeof(SCAN_METHOD_STRUCT));

	fseek(in, 0, SEEK_SET);

	while(1) {
		if(!TextGetOneLine(in, buf, 190)) 	break;
		line ++;
		//if(buf[0] == ';')	continue;	// cancel line
		if(buf[0] == 0)	continue;
		if(strlen(buf) == 0)	continue;

		else {
			commaBuf.Set(buf);
			commaBuf.GetString(imsi, sizeof(imsi));

			if(strcmp(imsi, "ACTIVE") == 0) {
				char on_off[20];
				commaBuf.GetString(on_off, sizeof(on_off));
				if(strcmp(on_off, "ON") == 0) {
					pt->bActiveFlag = ON;
				}
				else if(strcmp(on_off, "OFF") == 0) {
					pt->bActiveFlag = OFF;
				}
				else;
			}
			else if(strcmp(imsi, "BUF_LENGTH") == 0) {
				commaBuf.GetInt(pt->local.nBufSizeWORD);
			}
			else if(strcmp(imsi, "BUF_LENGTH_FLOAT") == 0) {
				commaBuf.GetInt(pt->local.nBufSizeFLOAT);
			}
			else if(strcmp(imsi, "BUF_LENGTH_DWORD") == 0) {
				commaBuf.GetInt(pt->local.nBufSizeDWORD);
			}
			else if(strcmp(imsi, "BUF_LENGTH_STRING") == 0) {
				commaBuf.GetInt(pt->local.nBufSizeSTRING);
			}
			else if(strcmp(imsi, "BUF_LENGTH_DOUBLE") == 0) {
				commaBuf.GetInt(pt->local.nBufSizeDOUBLE);
			}
			else if(strcmp(imsi, "BUF_LENGTH_INT64") == 0) {
				commaBuf.GetInt(pt->local.nBufSizeINT64);
			}
			else if(strcmp(imsi, "MAX_TIME_OUT_READ") == 0) {
				commaBuf.GetInt(pt->local.MAX_TIME_OUT_READ);
				pt->local.TIMEOUT_MILLI_READ = pt->local.MAX_TIME_OUT_READ*1000;
			}
			else if(strcmp(imsi, "MAX_TIME_OUT_WRITE") == 0) {
				commaBuf.GetInt(pt->local.MAX_TIME_OUT_WRITE);
				pt->local.TIMEOUT_MILLI_WRITE = pt->local.MAX_TIME_OUT_WRITE*1000;
			}
			else if(strcmp(imsi, "TIMEOUT_MILLI_READ") == 0) {
				commaBuf.GetInt(pt->local.TIMEOUT_MILLI_READ);
			}
			else if(strcmp(imsi, "TIMEOUT_MILLI_WRITE") == 0) {
				commaBuf.GetInt(pt->local.TIMEOUT_MILLI_WRITE);
			}
			else if(strcmp(imsi, "PROTOCOL") == 0) {
				commaBuf.GetString(pt->sScanProtocol, sizeof(pt->sScanProtocol));
				commaBuf.GetStringTotalRemain(pt->sScanProtocolOption[0], sizeof(pt->sScanProtocolOption[0]));
			}
			/*
			else if(stricmp(imsi, "DualProtocol") == 0) {
				commaBuf.GetString(pt->sDualProtocol, sizeof(pt->sDualProtocol));
			}*/
			// 이중화에서도 프로토콜 옵션이 필요해서 수정했다. 2017-8-4
			else if(stricmp(imsi, "DualProtocol") == 0) {
				commaBuf.GetString(pt->sDualProtocol, sizeof(pt->sDualProtocol));

				// 10.3.4 이상 버전의 파일은 이중화용 프로토콜 옵션이 있다. 이전버전은 기본을 복사해서 사용한다.
				if(IsVersionEqualOrHigher(FileVersionMajor, FileVersionMinor, FileVersionBuild, 0, 10, 3, 4, 0))
					commaBuf.GetStringTotalRemain(pt->sScanProtocolOption[1], sizeof(pt->sScanProtocolOption[1]));
				else
					strcpy(pt->sScanProtocolOption[1], pt->sScanProtocolOption[0]);

			}
			else if(strcmp(imsi, "DEVICE") == 0) {
				commaBuf.GetStringTotalRemain(pt->sScanDevice, sizeof(pt->sScanDevice));

				// TCP경우 존재하지 않는 IP의 경우 많은 시간이 걸리므로 각 포트별로 처음 읽을 때 초기화를 하도록 수정 10.0.2
				/*
				if(pt->bActiveFlag && !pt->pcDualFile.bActive) {
					// 통신 드라이버 종류를 알아본다.
					pt->nScanDevice = PlcDeviceInit(hwnd, &pt->local.device, pt->sScanDevice, pt->local.no);
					if(!pt->nScanDevice) {
						if(IsLangKorean()) {
							message.Format("알 수 없는 DEVICE 종류입니다.\n%s", pt->sScanDevice);
						}else {
							message.Format("undefined DEVICE type.\n%s", pt->sScanDevice);
						}
						MessageDisplay(message);
					}
				}*/
			}
			else if(strcmp(imsi, "TITLE") == 0) {
				commaBuf.GetString(pt->sTitle, sizeof(pt->sTitle));
			}
			else if(strcmp(imsi, "TELNUMBER") == 0) {				
				commaBuf.GetString(pt->tel.sTelNumber, sizeof(pt->tel.sTelNumber));
			}
			else if(strcmp(imsi, "TelConnectCicle") == 0) {
				commaBuf.GetInt(pt->tel.nConnectCicle);
			}
			else if(strcmp(imsi, "TelConnectingTime") == 0) {
				commaBuf.GetInt(pt->tel.nConnectingTime);
			}
			else if(strcmp(imsi, "TelAutoConnection") == 0) {
				commaBuf.GetChar(pt->tel.bAutoConnection);
			}
			else if(strcmp(imsi, "TelConnectingTimeOnManual") == 0) {
				commaBuf.GetInt(pt->tel.nConnectingTimeOnManual);
			}
			else if(stricmp(imsi, "DualActive") == 0) {
				commaBuf.GetChar(pt->bDualActive);
			}
			else if(stricmp(imsi, "DualDevice") == 0) {
				commaBuf.GetStringTotalRemain(pt->sDualDevice, sizeof(pt->sDualDevice));
			}
			else if(stricmp(imsi, "DualCauseTimeOut") == 0) {
				commaBuf.GetInt(pt->nDualCauseTimeOut);
			}
			else if(stricmp(imsi, "DualCauseCodeBad") == 0) {
				commaBuf.GetInt(pt->nDualCauseCodeBad);
			}
			
			else if(stricmp(imsi, "DualUseProtocol") == 0) {
				commaBuf.GetChar(pt->bDualUseProtocol);
			}
			else if(strcmp(imsi, "ScanTime") == 0 ||	// 구 버전
					strcmp(imsi, "ScanTimeRead") == 0) {
				commaBuf.GetInt(pt->local.nLocalReadScanTime);
			}
			else if(strcmp(imsi, "ScanTimeWrite") == 0) {
				commaBuf.GetInt(pt->local.nLocalWriteScanTime);
			}
			else if(stricmp(imsi, "ActiveThread") == 0) {
				commaBuf.GetChar(pt->bActiveThread);
			}
			else if(stricmp(imsi, "ThreadCycle") == 0) {
				commaBuf.GetInt(pt->nThreadCycle);
			}
			else if(stricmp(imsi, "ComputerDual") == 0) {
				commaBuf.GetChar(pt->pcDualFile.bActive);
				commaBuf.GetChar(pt->pcDualFile.bThread);
				commaBuf.GetString(pt->pcDualFile.sIP, sizeof(pt->pcDualFile.sIP));
				commaBuf.GetInt(pt->pcDualFile.nPort);
				commaBuf.GetInt(pt->pcDualFile.nTimeOut);
			}
			else if(stricmp(imsi, "UseStationInfo") == 0) {
				commaBuf.GetChar(pt->stationInfo.bUse);   
			}
			else if(stricmp(imsi, "UseDeviceInfo") == 0) {
				commaBuf.GetChar(pt->bUseDeviceInfo);   
			}
			else if(stricmp(imsi, "READ") == 0 ||
					stricmp(imsi, ";READ") == 0) {
				if(!AddOneScanMethod(hwnd, pt, block, commaBuf, imsi, 0))	
					break;
			}
			else if(stricmp(imsi, "FLOAT") == 0 ||
					stricmp(imsi, ";FLOAT") == 0) {
         		if(!AddOneScanMethod(hwnd, pt, block, commaBuf, imsi, 1))	
					break;
			}
			else if(stricmp(imsi, "DWORD") == 0 ||
					stricmp(imsi, ";DWORD") == 0) {
         		if(!AddOneScanMethod(hwnd, pt, block, commaBuf, imsi, 2))	
					break;
			}
			else if(stricmp(imsi, "STRING") == 0 ||
					stricmp(imsi, ";STRING") == 0) {
         		if(!AddOneScanMethod(hwnd, pt, block, commaBuf, imsi, 3))	
					break;
			}
			else if(stricmp(imsi, "DOUBLE") == 0 ||
					stricmp(imsi, ";DOUBLE") == 0) {
         		if(!AddOneScanMethod(hwnd, pt, block, commaBuf, imsi, 4))	
					break;
			}
			else if(stricmp(imsi, "INT64") == 0 ||
					stricmp(imsi, ";INT64") == 0) {
         		if(!AddOneScanMethod(hwnd, pt, block, commaBuf, imsi, 5))	
					break;
			}
			else if(stricmp(imsi, "FileVersion") == 0) {
				CString version;
         		commaBuf.GetString(version);
				CommaBlockString comma_v;
				comma_v.SetBlockCode('.');
				comma_v.Set(version);
				comma_v.GetInt(FileVersionMajor);
				comma_v.GetInt(FileVersionMinor);
				comma_v.GetInt(FileVersionBuild);
			}
		}
	}
	fclose(in);

	pt->local.scanMethod = new SCAN_METHOD_STRUCT[pt->local.nScanMethodHap];
	if(pt->local.scanMethod == NULL) {
		pt->local.nScanMethodHap = 0;
		MsgBoxLocalMemoryLow(hwnd, "at portBuf->scanMethod new");
		return;
	}

	for(i = 0; i < pt->local.nScanMethodHap; i++) {
		block.GetBlock((BYTE*)&sm, i);
		memcpy(&pt->local.scanMethod[i], &sm, sizeof(SCAN_METHOD_STRUCT));
	}

	if(pt->local.nBufSizeWORD > 100000) {
		MessageBox(hwnd, "BUF_LENGTH > 100,000", "buf too big", MB_OK);
		pt->local.nBufSizeWORD = 100000;
	}

	if(pt->local.nBufSizeFLOAT > 100000) {
		MessageBox(hwnd, "BUF_LENGTH_FLOAT > 100,000", "buf too big", MB_OK);
		pt->local.nBufSizeFLOAT = 100000;
	}

	if(pt->local.nBufSizeDWORD > 100000) {
		MessageBox(hwnd, "BUF_LENGTH_DWORD > 100,000", "buf too big", MB_OK);
		pt->local.nBufSizeDWORD = 100000;
	}

	if(pt->local.nBufSizeSTRING > 100000) {
		MessageBox(hwnd, "BUF_LENGTH_STRING > 100,000", "buf too big", MB_OK);
		pt->local.nBufSizeSTRING = 100000;
	}

	if(pt->local.nBufSizeDOUBLE > 100000) {
		MessageBox(hwnd, "BUF_LENGTH_DOUBLE > 100,000", "buf too big", MB_OK);
		pt->local.nBufSizeDOUBLE = 100000;
	}

	if(pt->local.nBufSizeINT64 > 100000) {
		MessageBox(hwnd, "BUF_LENGTH_INT64 > 100,000", "buf too big", MB_OK);
		pt->local.nBufSizeINT64 = 100000;
	}

	MapScanMemory(pt);

	// 파일을 읽고 나면 기본 디바이스로 통신을 한다.
	void PokeDualSystem(GLOBAL_PORT_STRUCT *pt, char bBasicOrDual);
	PokeDualSystem(pt, 0);

	pt->blockWriteWait = new WRITE_WAIT_STRUCT;
	ZeroMemory(pt->blockWriteWait, sizeof(WRITE_WAIT_STRUCT));
	pt->blockWriteWait->item = new SCAN_WRITE_EXCHANGE_ITEM[MAX_SCAN_WRITE_LOCAL_ITEM_COUNT];	// 포인터로 변경되면서 할당해야 한다. 2021-6-23 

	
}

static int CommDeviceReadStationCount()
{
	int  i;

	GLOBAL_PORT_STRUCT *pt;

	for(i = MAX_PORT-1; i >= 0; i--) {
		pt = &portBuf[i];

		if(pt->bActiveFlag) {
			return i+1;
		}
	}

	return 0;
}

void PlcProtocolInitAll(HWND hwnd);

int CheckSquare2(WORD org) 
{
	int i;
	WORD size = 1;

	for(i = 1; i < 16; i++) {
		if(size == org)	return 1;			
		if(size > org)	return 0;
		size *= 2;
	}

	return 0;
}

void ScanStationInfoInit(GLOBAL_PORT_STRUCT *pt);
void ScanStationInfoUnInit(GLOBAL_PORT_STRUCT *pt);

void PortThreadInit(int port);
void PortThreadUnInit(int port);
void ComputerDualThreadInit(int port);
void ComputerDualThreadUnInit(int port);
void ComputerDualDeviceInit(int port);
void ComputerDualDeviceUnInit(int port);

void PlcProtocolInitOne(HWND hwnd, GLOBAL_PORT_STRUCT *port, char bBasicOrDual);

void ScanPortInitOne(HWND hwnd, int port)
{
	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	BYTE savehmmfnumber = pt->hmmfNumber;	// hmmfNumber 는 계속 증가하면서 다른맵으로 구성되므로 초기화시 다음번호로 이동할 수 있도록 보관한다.
	memset(&portBuf[port], 0, sizeof(GLOBAL_PORT_STRUCT));
	pt->hmmfNumber = savehmmfnumber;

	CommDeviceRead(hwnd, port);

	ScanStationInfoInit(&portBuf[port]);

	nPortHap = CommDeviceReadStationCount();

	// TCP경우 존재하지 않는 IP의 경우 많은 시간이 걸리므로 각 포트별로 처음 읽을 때 초기화를 하도록 수정 10.0.2
	//PlcProtocolInitOne(hwndMainFrame, &portBuf[port], 0);	// 0 = 기본 프로토콜
	
	ComputerDualDeviceInit(port);
	ComputerDualThreadInit(port);
 
	PortThreadInit(port);

	portBuf[port].bInitialFlag = 1;

	SystemStatusSetDI(SSMDI_ErrorStatusPlcScanTimeOut, OFF);	// 각 포트를 설정 시 전체 통신 프로그램 시간초과는 초기화 시켜주는 것이 좋다.
}

//------------------------------------------------------------------------------
//	SCAN 디렉토리에 있는 SCAN.??? 파일을 불러온다.
//------------------------------------------------------------------------------

void ScanPortInitAll(HWND hwnd)
{
	int i;

	if(!CheckSquare2(sizeof(GLOBAL_PORT_STRUCT))) {
		char message[80];
		sprintf(message, "PORT_STRUCT must alignment **2 \n(current size-%d Byte)", sizeof(GLOBAL_PORT_STRUCT));
		MessageBox(hwnd, message, "Must change struct size", MB_OK);
	}
	
	portBuf = new GLOBAL_PORT_STRUCT[MAX_PORT];
	if(portBuf == NULL) {
		MsgBoxLocalMemoryLow(hwnd, "At portBuf new");
		nPortHap = 0;
	}
	else {

	}

	for(i = 0; i < MAX_PORT; i++) {
		ScanPortInitOne(hwnd, i);
	}
}

void PlcProtocolUnInitOne(GLOBAL_PORT_STRUCT *port);

void ScanPortUnInitOne(int port)
{
	portBuf[port].bInitialFlag = 0;

	PortThreadUnInit(port);

	ComputerDualThreadUnInit(port);
	ComputerDualDeviceUnInit(port);
	
	GLOBAL_PORT_STRUCT *pt;

	pt = &portBuf[port];

	if(!pt->bActiveThread) {	// 2011-11-18 스레드일때는 스레드 안에서 처리 TCP를 쓰레드 안에서 처리하지 않으니 35번째 위치에서 tcpip->bConnect = OFF; 부분에서 계속 기다리는 듯 하다.
								// 이렇게 하니 포트가 많아도 순식간에 해결된다.
		PlcProtocolUnInitOne(pt);
		PlcDeviceUnInit(&pt->local.device);	
	}

	if(pt->local.timeout)		delete pt->local.timeout;
	if(pt->local.scanMethod)	delete pt->local.scanMethod;

	PlcScanMemoryUninit(pt);

	if(pt->blockWriteWait)		{
		if(pt->blockWriteWait->item) delete pt->blockWriteWait->item;	// item이 포인터로 변경되면서 삭제해야한다. 2021-6-23 추가.
		delete pt->blockWriteWait;
	}

	ScanStationInfoUnInit(pt);

	if(pt->pCC)					delete pt->pCC;
}

void ScanPortUnInitAll()
{
	if(portBuf == NULL)	return;

	int i;
	
	// 모든 쓰레드 명령을 중지시키고 포트를 하나씩 제거하는 것이 빠르다. 2011-11-18 
	for(i = 0; i < MAX_PORT; i++) {
		portBuf[i].threadInfo.bDo = OFF;
	}

	for(i = 0; i < MAX_PORT; i++) {
		ScanPortUnInitOne(i);
	}

	delete portBuf;
	portBuf = NULL;
	nPortHap = 0;
}




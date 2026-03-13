// english O.K
#include "stdafx.h"
#include <stdio.h>

#include <tools.h>
#include <gclass.h>
#include <glib.h>
#include <crc.hpp>

#include "plc_scan.h"
#include "..\catlib.src\totalcfg.h"

HFONT hFontEdit;				// 포트 에디터에서 사용하는 폰트.


void InitConfig()
{
	MakeDefaultLogFont(&config.bin.logFontEdit);	// 포트 에디터에서 사용하는 log font를 만든다.
}

//------------------------------------------------------------------------------
//	AutoBase PlcScan 환경을 저장한다.
//------------------------------------------------------------------------------

char *sSectionPlcScan = "PlcScan";

static int ConfigGetInt(const char *section, const char *item, const int default_value)
{
	DWORD val;
	LoadRegAutoBaseConfig("PlcScan", section, item, default_value, val);
	return val;
}

static void ConfigSetInt(const char *section, const char *item, const DWORD val)
{
	SaveRegAutoBaseConfig("PlcScan", section, item, val);
}

void LoadConfig()
{
	config.pMessageBox.x = ConfigGetInt(sSectionPlcScan, "pMessageBoxX", 100);
	config.pMessageBox.y = ConfigGetInt(sSectionPlcScan, "pMessageBoxY", 100);
	config.nAlarmScreenTime = ConfigGetInt(sSectionPlcScan, "nAlarmScreenTime", 5);
	
	config.bMessageBoxAllMessage = ConfigGetInt(sSectionPlcScan, "bMessageBoxAllMessage", ON);	// 모든 메세지를 표시한다.

	config.bMessageBoxTimeOut = ConfigGetInt(sSectionPlcScan, "bMessageBoxTimeOut", ON);		// TimeOut일 때 메세지 상자를 표시할 것이냐?
	config.bMessageBoxCodeBad = ConfigGetInt(sSectionPlcScan, "bMessageBoxCodeBad", ON);		// CodeBad일 때 메세지 상자를 표시할 것이냐?

    config.nScanTime = ConfigGetInt(sSectionPlcScan, "nScanTime", 0);					// Timer 간격

	config.nSkipTimeOnTimeOut = ConfigGetInt(sSectionPlcScan, "nSkipTimeOnTimeOut", 0);		// timeout이 5회 이상 발생할 때 통신 대기시간.
	config.bBufClearOnTimeOut5 = ConfigGetInt(sSectionPlcScan, "bBufClearOnTimeOut5", 0);	// 시간초과가 5회이상 발행할 때 버퍼 클리어.
	config.fBufClearValueOnTimeOut5 = (float)ConfigGetInt(sSectionPlcScan, "fBufClearValueOnTimeOut5", 0);	// 시간초과가 5회이상 발행할 때 버퍼 클리어 값.
	config.nItemTimeoutCount = ConfigGetInt(sSectionPlcScan, "nItemTimeoutCount", 5);	// 시간초과가 5회이상 발행할 때 버퍼 클리어.

	config.bMultiPortMultiTasking = ConfigGetInt(sSectionPlcScan, "bMultiPortMultiTasking", 1);	// multiport를 번갈아 가면서 읽는다.

	config.nRetryCountOnWriteTimeOut = ConfigGetInt(sSectionPlcScan, "nRetryCountOnWriteTimeOut", 0);	// 쓰기 시 시간초과가 발생할 때 재시도 횟수.

	config.nVipScanTryCount = ConfigGetInt(sSectionPlcScan, "nVipScanTryCount", 1);	// VipScan 횟수
	if(config.nVipScanTryCount < 0 || config.nVipScanTryCount > 5)	config.nVipScanTryCount = 0;

	config.OnPortTimeOut_SetValue_TimeOut = ConfigGetInt("OnPortTimeOut", "OnPortTimeOut_SetValue_TimeOut", 0);	// multiport를 번갈아 가면서 읽는다.
	config.OnPortTimeOut_SetValue_Value = ConfigGetInt("OnPortTimeOut", "OnPortTimeOut_SetValue_Value", 0);	// multiport를 번갈아 가면서 읽는다.

	config.bUseNewValueOnDigitalOut = ConfigGetInt("NewValueOnWrite", "bUseNewValueOnDigitalOut", 1);	// 비트 쓰기시 중복명령이 있으면 마지막 값 사용
	config.bUseNewValueOnAnalogOut = ConfigGetInt("NewValueOnWrite", "bUseNewValueOnAnalogOut", 1);	// 워드 쓰기시 중복명령이 있으면 마지막 값 사용

	FILE *in;
	WORD crc, crccheck;

	CString filename;

	filename.Format("%s\\config\\plc_scan.cfg", sDirProgramm);

	in = fopen(filename, "rb");
	if(in == NULL) {
		InitConfig();
		return;
	}
	fread(&config.bin, 1, sizeof(CONFIG_BIN_STRUCT), in);
	fread(&crccheck, 1, 2, in);
	fclose(in);

	crc = GetCRC16((BYTE*)&config.bin, sizeof(CONFIG_BIN_STRUCT));

	if(crc != crccheck) {
		InitConfig();
		return;
	}
}

//------------------------------------------------------------------------------
//	AutoBase 환경을 불러온다.
//------------------------------------------------------------------------------

void SaveConfig(HWND hwnd)
{
	ConfigSetInt(sSectionPlcScan, "pMessageBoxX", config.pMessageBox.x);
	ConfigSetInt(sSectionPlcScan, "pMessageBoxY", config.pMessageBox.y);
	ConfigSetInt(sSectionPlcScan, "nAlarmScreenTime", config.nAlarmScreenTime);
	
	ConfigSetInt(sSectionPlcScan, "bMessageBoxAllMessage", config.bMessageBoxAllMessage);	// 모든 메세지를 표시한다.

	ConfigSetInt(sSectionPlcScan, "bMessageBoxTimeOut", config.bMessageBoxTimeOut);		// TimeOut일 때 메세지 상자를 표시할 것이냐?
	ConfigSetInt(sSectionPlcScan, "bMessageBoxCodeBad", config.bMessageBoxCodeBad);		// CodeBad일 때 메세지 상자를 표시할 것이냐?

    ConfigSetInt(sSectionPlcScan, "nScanTime", config.nScanTime);					// Timer 간격

	ConfigSetInt(sSectionPlcScan, "nSkipTimeOnTimeOut", config.nSkipTimeOnTimeOut);		// timeout이 5회 이상 발생할 때 통신 대기시간.
	ConfigSetInt(sSectionPlcScan, "bBufClearOnTimeOut5", config.bBufClearOnTimeOut5);	// 시간초과가 5회이상 발행할 때 버퍼 클리어.
	ConfigSetInt(sSectionPlcScan, "fBufClearValueOnTimeOut5", (int)config.fBufClearValueOnTimeOut5);	// 시간초과가 5회이상 발행할 때 버퍼 클리어.
	ConfigSetInt(sSectionPlcScan, "nItemTimeoutCount", config.nItemTimeoutCount);	// 시간초과가 5회이상 발행할 때 버퍼 클리어.
    
	ConfigSetInt(sSectionPlcScan, "bMultiPortMultiTasking", config.bMultiPortMultiTasking);	// multiport를 번갈아 가면서 읽는다.

	ConfigSetInt(sSectionPlcScan, "nRetryCountOnWriteTimeOut", config.nRetryCountOnWriteTimeOut);	// 쓰기 시 시간초과가 발생할 때 재시도 횟수.

	ConfigSetInt(sSectionPlcScan, "nVipScanTryCount", config.nVipScanTryCount);	// 쓰기 시 시간초과가 발생할 때 재시도 횟수.

	ConfigSetInt("OnPortTimeOut", "OnPortTimeOut_SetValue_TimeOut", config.OnPortTimeOut_SetValue_TimeOut);	// multiport를 번갈아 가면서 읽는다.
	ConfigSetInt("OnPortTimeOut", "OnPortTimeOut_SetValue_Value", config.OnPortTimeOut_SetValue_Value);	// multiport를 번갈아 가면서 읽는다.

	ConfigSetInt("NewValueOnWrite", "bUseNewValueOnDigitalOut", config.bUseNewValueOnDigitalOut);	// 비트 쓰기시 중복명령이 있으면 마지막 값 사용
	ConfigSetInt("NewValueOnWrite", "bUseNewValueOnAnalogOut", config.bUseNewValueOnAnalogOut);	// 워드 쓰기시 중복명령이 있으면 마지막 값 사용

	FILE *out;
	WORD crc = 0;

	CString filename;

	filename.Format("%s\\config", sDirProgramm);
	MakeDirectory(filename);

	filename.Format("%s\\config\\plc_scan.cfg", sDirProgramm);

	crc = GetCRC16((BYTE*)&config.bin, sizeof(CONFIG_BIN_STRUCT));

	out = fopen(filename, "wb");
	if(out == NULL) {
		MessageBox(hwnd, filename, "Can't write file", MB_OK);
		return;
	}
	fwrite(&config.bin, 1, sizeof(CONFIG_BIN_STRUCT), out);
	fwrite(&crc, 1, 2, out);
	fclose(out);
}

void EditFontDelete()
{
	if(hFontEdit)	DeleteObject(hFontEdit);
	hFontEdit = NULL;
}

void EditFontMake()
{
	EditFontDelete();
	hFontEdit = CreateFontIndirect(&config.bin.logFontEdit);
}

void ConfigEditFont(HWND hwnd)
{
	GFontDialog dialog(hwnd);

	dialog.SetLogFont(&config.bin.logFontEdit);
	if(dialog.Execute()) {
		dialog.GetLogFont(&config.bin.logFontEdit);
		EditFontMake();
	}
}

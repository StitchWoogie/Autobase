#include "stdafx.h"
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <io.h>

#include <tools.h>
#include <glib.h>
#include <crc.hpp>
#include <dataswap.h>

#include "..\catlib.src\SystemStatusMemory.h"
#include "..\catlib.src\totalcfg.h"

#include "plc_scan.h"

void ScanStationInfoInit(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->stationInfo.bUse == 0)	return;

	SCAN_METHOD_STRUCT *sm;
	StackBYTE ex = StackBYTE(65536);
	STATION_INFO_STRUCT *si = &pt->stationInfo;

	memset(ex.data, 0, 65536);

	for(int i = 0; i < pt->local.nScanMethodHap; i++) {
		sm = &pt->local.scanMethod[i];
		ex.data[sm->station] = 1;
	}

	si->station_count = 0;
	
	for(int i = 0; i < 65536; i++) {
		if(ex.data[i] == 1)
			si->station_count++;
	}

	if(si->station_count > 0) 
		si->stationBuf = new STATION_INFO_ONE_STRUCT[si->station_count];
	else
		si->stationBuf = NULL;

	int pos = 0;
	for(int i = 0; i < 65536; i++) {
		if(ex.data[i] == 1) {
			memset(&si->stationBuf[pos], 0, sizeof(STATION_INFO_ONE_STRUCT));
			si->stationBuf[pos].station_no = i;
			pos++;	
		}
	}
}

void ScanStationInfoUnInit(GLOBAL_PORT_STRUCT *pt)
{
	if(pt->stationInfo.bUse == 0)	return;

	if(pt->stationInfo.stationBuf != NULL)	{
		delete pt->stationInfo.stationBuf;
	}
}

void CalcSuccessPercent(GLOBAL_PORT_STRUCT *pt, COMM_COUNT_STRUCT *count);

STATION_INFO_ONE_STRUCT *SeekStationInfo(GLOBAL_PORT_STRUCT *pt, int station)
{
	STATION_INFO_ONE_STRUCT *sio;

	for(int i = 0; i < pt->stationInfo.station_count; i++) {
		sio = &pt->stationInfo.stationBuf[i];

		if(sio->station_no == station) {
			return sio;
		}
	}

	return NULL;
}

static void SaveInfoToReg(int port, int station, const char *sub_name, COMM_COUNT_STRUCT *count, bool timeout, bool codebad)
{
	CString root_name;
	root_name.Format("PlcScan\\StationInfo\\Port%03d\\Station%03d", port, station);

	SaveRegAutoBaseConfig(root_name, sub_name, "TotalCommTry",	count->lCountCommTry);
	SaveRegAutoBaseConfig(root_name, sub_name, "SeccessPercent", count->wSuccessPercent);

	if(timeout) {	// 속도가 개선될까 해서 필요한 부분만 저장. 속도 체크는 해보지 않음
		SaveRegAutoBaseConfig(root_name, sub_name, "TotalTimeOut",	count->lCountTimeOut);
		SaveRegAutoBaseConfig(root_name, sub_name, "ContinedTimeOut", count->nContinuedTimeOut);
		SaveRegAutoBaseConfig(root_name, sub_name, "FlagsTimeOut", count->flagsTimeOut);
	}
	
	if(codebad) {	// 속도가 개선될까 해서 필요한 부분만 저장. 속도 체크는 해보지 않음
		SaveRegAutoBaseConfig(root_name, sub_name, "TotalCodeBad",	count->lCountCodeBad);
		SaveRegAutoBaseConfig(root_name, sub_name, "ContinedCodeBad", count->nContinuedCodeBad);
		SaveRegAutoBaseConfig(root_name, sub_name, "FlagsCodeBad", count->flagsCodeBad);
	}
}

void StationInfoPlusCommCountReadTry(GLOBAL_PORT_STRUCT *pt, int station) 
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	device->Read.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->Read);
	CalcSuccessPercent(pt, &device->Total);

	SaveInfoToReg(pt->local.no, station, "Total", &device->Total, false, false);
	SaveInfoToReg(pt->local.no, station, "Read", &device->Read, false, false);
}

void CalcTimeOut(GLOBAL_PORT_STRUCT *pt, int station, const char *sub_item, COMM_COUNT_STRUCT *count)
{
	count->lCountTimeOut++;
	if(count->nContinuedTimeOut < 30000)	// 오버를 막기위해서
		count->nContinuedTimeOut++;

	if(count->nContinuedTimeOut >= 30) 
		count->flagsTimeOut |= WORD_MASK[4];
	else if(count->nContinuedTimeOut >= 20) 
		count->flagsTimeOut |= WORD_MASK[3];
	else if(count->nContinuedTimeOut >= 10) 
		count->flagsTimeOut |= WORD_MASK[2];
	else if(count->nContinuedTimeOut >= 5) 
		count->flagsTimeOut |= WORD_MASK[1];
	else	// 1회
		count->flagsTimeOut |= WORD_MASK[0];

	CalcSuccessPercent(pt, count);

	SaveInfoToReg(pt->local.no, station, sub_item, count, true, false);
}

void CalcCodeBad(GLOBAL_PORT_STRUCT *pt, int station, const char *sub_item, COMM_COUNT_STRUCT *count)
{
	count->lCountCodeBad++;
	if(count->nContinuedCodeBad < 30000)	// 오버를 막기위해서
		count->nContinuedCodeBad++;

	if(count->nContinuedCodeBad >= 30) 
		count->flagsCodeBad |= WORD_MASK[4];
	else if(count->nContinuedCodeBad >= 20) 
		count->flagsCodeBad |= WORD_MASK[3];
	else if(count->nContinuedCodeBad >= 10) 
		count->flagsCodeBad |= WORD_MASK[2];
	else if(count->nContinuedCodeBad >= 5) 
		count->flagsCodeBad |= WORD_MASK[1];
	else	// 1회
		count->flagsCodeBad |= WORD_MASK[0];

	CalcSuccessPercent(pt, count);

	SaveInfoToReg(pt->local.no, station, sub_item, count, false, true);
}

void StationInfoPlusCommCountReadTimeOut(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	CalcTimeOut(pt, station, "Total", &device->Total);
	CalcTimeOut(pt, station, "Read", &device->Read);
}

void StationInfoPlusCommCountReadCodeBad(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	CalcCodeBad(pt, station, "Total", &device->Total);
	CalcCodeBad(pt, station, "Read", &device->Read);
}

void StationInfoPlusCommCountWriteBitTry(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	device->WriteBit.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->WriteBit);
	CalcSuccessPercent(pt, &device->Total);

	SaveInfoToReg(pt->local.no, station, "Total", &device->Total, false, false);
	SaveInfoToReg(pt->local.no, station, "WriteBit", &device->WriteBit, false, false);
}

void StationInfoPlusCommCountWriteBitTimeOut(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	CalcTimeOut(pt, station, "Total", &device->Total);
	CalcTimeOut(pt, station, "WriteBit", &device->WriteBit);
}

void StationInfoPlusCommCountWriteBitCodeBad(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	CalcCodeBad(pt, station, "Total", &device->Total);
	CalcCodeBad(pt, station, "WriteBit", &device->WriteBit);
}

void StationInfoPlusCommCountWriteWordTry(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	device->WriteWord.lCountCommTry++;
	device->Total.lCountCommTry++;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	SaveInfoToReg(pt->local.no, station, "Total", &device->Total, false, false);
	SaveInfoToReg(pt->local.no, station, "WriteWord", &device->WriteWord, false, false);
}

void StationInfoMinusCommCountWriteWordTry(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	device->WriteWord.lCountCommTry--;
	device->Total.lCountCommTry--;
	CalcSuccessPercent(pt, &device->WriteWord);
	CalcSuccessPercent(pt, &device->Total);

	SaveInfoToReg(pt->local.no, station, "Total", &device->Total, false, false);
	SaveInfoToReg(pt->local.no, station, "WriteWord", &device->WriteWord, false, false);
}

void StationInfoPlusCommCountWriteWordTimeOut(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	CalcTimeOut(pt, station, "Total", &device->Total);
	CalcTimeOut(pt, station, "WriteWord", &device->WriteWord);
}

void StationInfoPlusCommCountWriteWordCodeBad(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	CalcCodeBad(pt, station, "Total", &device->Total);
	CalcCodeBad(pt, station, "WriteWord", &device->WriteWord);
}

static void ClearError(GLOBAL_PORT_STRUCT *pt, int station, const char *sub_item, COMM_COUNT_STRUCT *count)
{
	count->nContinuedTimeOut = 0;
	count->nContinuedCodeBad = 0;

	count->flagsTimeOut = 0;
	count->flagsCodeBad = 0;

	SaveInfoToReg(pt->local.no, station, sub_item, count, true, true);
}

void StationInfoClearErrorReadComm(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	ClearError(pt, station, "Total", &device->Total);
	ClearError(pt, station, "Read", &device->Read);
}

void StationInfoClearErrorWriteBit(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	ClearError(pt, station, "Total", &device->Total);
	ClearError(pt, station, "WriteBit", &device->WriteBit);
}

void StationInfoClearErrorWriteWord(GLOBAL_PORT_STRUCT *pt, int station)
{
	if(pt->stationInfo.bUse == 0)	return;

	STATION_INFO_ONE_STRUCT *sio = SeekStationInfo(pt, station);
	if(sio == NULL)	return;

	DEVICE_COUNT_STRUCT *device = &sio->count_info;

	ClearError(pt, station, "Total", &device->Total);
	ClearError(pt, station, "WriteWord", &device->WriteWord);
}

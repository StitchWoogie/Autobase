// TAG size O.K
#include "stdafx.h"
#include <stdio.h>
// Version 8과 9의 데이타를 읽어올 수 있는 라이브러리
#include <tools.h>
#include <crc.hpp>

#include "..\catlib\catdata.h" 
#include "..\catlib\totalcfg.h"

static int ReadTrendStructDI(char *filename, char day, char hour, char min, TREND_DI_STRUCT *data)
{
	FILE *in;
	FILE_TREND_DI_STRUCT trend;

	in = fopen(filename, "rb");
	if(in == NULL) 	return 0;	// 자료 없슴

	long start = (long)sizeof(FILE_TREND_DI_STRUCT);
	start = start*((day-1)*1440+hour*60L+min);

	fseek(in, start, SEEK_SET);
	fread(&trend, 1, sizeof(FILE_TREND_DI_STRUCT), in);
	fclose(in);

	if(trend.day != day)						return 0;
	if(trend.hour != hour)						return 0;
	if(trend.min != min)						return 0;
	if(trend.crc != GetCRC16((BYTE*)&trend, sizeof(FILE_TREND_DI_STRUCT)-2))	return 0;

	memcpy(data, &trend.data, sizeof(TREND_DI_STRUCT));

	return 1;
}

void GetDuplexActiveDataDirectory(CString &directory)
{
	/*
	if(!shareData->duplex.bUse) {
		directory = shareData->sDirData;
		return;
	}

	if(SystemStatusGetDI(SSMDI_DuplexActiveSecondary)) {
		directory = shareData->duplex.sSecondary;
		return;
	}
	else {
		directory = shareData->duplex.sPrimary;
		return;
	}
	*/
	CString work_dir;
	char data[MAXPATH];
	AutoBaseIniGetProjectDirectory(work_dir);
	AutoBaseIniGetProjectDataDirectory(work_dir, data);
	directory = data;
}
 
static int SmGetTrendStructDI(int terminal, int year, char mon, char day, char hour, char min, TREND_DI_STRUCT *trend, char *tag)
{
	char path[MAXPATH];
	char tag_file[80];
	CString data_dir;
	GetDuplexActiveDataDirectory(data_dir);

	ConvertTagToFile(tag_file, tag);
	sprintf(path, "%s\\TREND\\%04d\\MON%02d\\DI\\%s", data_dir, year, mon, tag_file);

	return ReadTrendStructDI(path, day, hour, min, trend);
}

//------------------------------------------------------------------------------
//	아날로그 분 데이타를 읽어온다.
//------------------------------------------------------------------------------

int CatDataGetDiMin(int terminal, const char *tag, DWORD *value, int year, int month, int day, int hour, int min, int data_type)
{
	TREND_DI_STRUCT trend;
	//TERMINAL_STRUCT *ter = &terminalStruct[terminal];

	*value = 0;

	if(!SmGetTrendStructDI(terminal, year, month, day, hour, min, &trend, (char*)(LPCSTR)tag)) {
		return 0;
	}

	if(data_type == DI_DATA_TYPE_ONTIME) {
		*value = trend.cOnTime;
	}
	else if(data_type == DI_DATA_TYPE_OFFTIME) {
		*value = 60-trend.cOnTime;
	}
	else if(data_type == DI_DATA_TYPE_COUNT) {
		*value = trend.nCountOnOff;
	}
	else {
		return 0;
	}

	return 1;
}

int SmPrepareFileData(int terminal, LPSTR path, LPCSTR file)
{
	CString data_dir;
	GetDuplexActiveDataDirectory(data_dir);
	sprintf(path, "%s\\%s", data_dir, file);
	return 1;
}

//------------------------------------------------------------------------------
//	아날로그 시간 데이타를 읽어온다.
//------------------------------------------------------------------------------

int CatDataGetDiHour(int terminal, const char *tag, DWORD *value, int year, int month, int day, int hour, int data_type)
{
	char tag_file[80];
	char path[MAXPATH];
	char filename[MAXPATH];
	HOUR_DATA_HEAD head;
	HOUR_DATA_DIGITAL_STRUCT data;
	FILE *in;

	*value = 0;

	ConvertTagToFile(tag_file, tag);
	sprintf(filename, "SUM\\%04d\\MON%02d\\DI\\%s", year, month, tag_file);
	SmPrepareFileData(terminal, path, filename);

	in = fopen(path, "rb");
	if(in == NULL)			return 0;

	fseek(in, 0, SEEK_SET);

	if(fread(&head, 1, sizeof(HOUR_DATA_HEAD), in) != sizeof(HOUR_DATA_HEAD))	{
		fclose(in);
		return 0;
	}

	fseek(in, sizeof(HOUR_DATA_HEAD)+((day-1)*24L+hour)*sizeof(HOUR_DATA_DIGITAL_STRUCT), SEEK_SET);
	if(fread(&data, 1, sizeof(HOUR_DATA_DIGITAL_STRUCT), in) != sizeof(HOUR_DATA_DIGITAL_STRUCT)) {
		fclose(in);
		return 0;
	}

	fclose(in);

	if(data.flag == OFF)	{
		return 0;	// 자료는 있으나 초기화 되어있는 값이다.
	}

	if(data.crc != GetCRC16((BYTE*)&data, sizeof(HOUR_DATA_DIGITAL_STRUCT)-2)) {
    	return 0;
	}

	if(data_type == DI_DATA_TYPE_ONTIME) {
		*value = data.dwOnTime;
	}
	else if(data_type == DI_DATA_TYPE_OFFTIME) {
		*value = 3600-data.dwOnTime;
	}
	else if(data_type == DI_DATA_TYPE_COUNT) {
		*value = data.wCountOnOff;
	}
	else {
		return 0;
	}

	return 1;
}

int CatDataGetDiDay(int terminal, const char *tag, DWORD *value, int year, int month, int day, int data_type)
{
	char tag_file[80];
	char path[MAXPATH];
	char filename[MAXPATH];
	HOUR_DATA_HEAD head;
	HOUR_DATA_DIGITAL_STRUCT data;
	char read_flag = OFF;
	int  read_count = 0;
	int  i;
	FILE *in;

	*value = 0;

	ConvertTagToFile(tag_file, tag);
	sprintf(filename, "SUM\\%04d\\MON%02d\\DI\\%s", year, month, tag_file);
	SmPrepareFileData(terminal, path, filename);

	in = fopen(path, "rb");
	if(in == NULL)			return 0;

	fseek(in, 0, SEEK_SET);

	if(fread(&head, 1, sizeof(HOUR_DATA_HEAD), in) != sizeof(HOUR_DATA_HEAD))	{
   	fclose(in);
		return 0;
	}

	fseek(in, sizeof(HOUR_DATA_HEAD)+((day-1)*24L)*sizeof(HOUR_DATA_DIGITAL_STRUCT), SEEK_SET);

	for(i = 0; i < 24; i++) {
		if(fread(&data, 1, sizeof(HOUR_DATA_DIGITAL_STRUCT), in) != sizeof(HOUR_DATA_DIGITAL_STRUCT)) {
			fclose(in);
			return 0;
		}

		if(data.flag && data.crc == GetCRC16((BYTE*)&data, sizeof(HOUR_DATA_DIGITAL_STRUCT)-2)) {
			read_flag = ON;
			read_count ++;

			if(data_type == DI_DATA_TYPE_ONTIME) {
				*value += data.dwOnTime;
			}
			else if(data_type == DI_DATA_TYPE_OFFTIME) {
				*value += data.dwOnTime;
			}
			else if(data_type == DI_DATA_TYPE_COUNT) {
				*value += data.wCountOnOff;
			}
			else {
				fclose(in);
				return 0;
			}
		}
	}

	fclose(in);

	if(read_flag == OFF) {
		return 0;	// 읽은 데이타가 없다.
	}

	if(data_type == DI_DATA_TYPE_OFFTIME) {
		*value = read_count*3600-*value;
	}

	return 1;
}

int CatDataGetDiMonth(int terminal, const char *tag, DWORD *value, int year, int month, int data_type)
{
	char tag_file[80];
	char path[MAXPATH];
	char filename[MAXPATH];
	HOUR_DATA_HEAD head;
	HOUR_DATA_DIGITAL_STRUCT data;
	char read_flag = OFF;
	int  read_count = 0;
	int  i;
	FILE *in;

	*value = 0;

	ConvertTagToFile(tag_file, tag);
	sprintf(filename, "SUM\\%04d\\MON%02d\\DI\\%s", year, month, tag_file);
	SmPrepareFileData(terminal, path, filename);

	in = fopen(path, "rb");
	if(in == NULL)			return 0;

	fseek(in, 0, SEEK_SET);

	if(fread(&head, 1, sizeof(HOUR_DATA_HEAD), in) != sizeof(HOUR_DATA_HEAD))	{
   		fclose(in);
		return 0;
	}

	//fseek(in, sizeof(HOUR_DATA_HEAD)+((day-1)*24L)*sizeof(HOUR_DATA_DIGITAL_STRUCT), SEEK_SET);

	for(i = 0; i < 24*31; i++) {
		if(fread(&data, 1, sizeof(HOUR_DATA_DIGITAL_STRUCT), in) != sizeof(HOUR_DATA_DIGITAL_STRUCT)) {
			fclose(in);
			return 0;
		}

		if(data.flag && data.crc == GetCRC16((BYTE*)&data, sizeof(HOUR_DATA_DIGITAL_STRUCT)-2)) {
			read_flag = ON;
			read_count ++;

			if(data_type == DI_DATA_TYPE_ONTIME) {
				*value += data.dwOnTime;
			}
			else if(data_type == DI_DATA_TYPE_OFFTIME) {
				*value += data.dwOnTime;
			}
			else if(data_type == DI_DATA_TYPE_COUNT) {
				*value += data.wCountOnOff;
			}
			else {
				fclose(in);
				return 0;
			}
		}
	}

	fclose(in);

	if(read_flag == OFF) {
		return 0;	// 읽은 데이타가 없다.
	}

	if(data_type == DI_DATA_TYPE_OFFTIME) {
		*value = read_count*3600-*value;
	}

	return 1;
}

int CatDataGetDiYearOnTime(int terminal, const char *tag, DWORD *value, int year)
{
	int day;
	int month;
	char read_flag = OFF;
	int read_count = 0;
	DWORD imsi;

	*value = 0;

	for(month = 1; month <= 12; month++) {
		for(day = 1; day <= 31; day++) {
			if(CatDataGetDiDay(terminal, tag, &imsi, year, month, day, DI_DATA_TYPE_ONTIME)) {
				read_flag = ON;
				read_count++;
				*value += imsi;
			}
		}
	}

	if(read_flag == OFF)	return 0;

	return 1;
}

int CatDataGetDiYearCount(int terminal, const char *tag, DWORD *value, int year)
{
	int day;
	int month;
	char read_flag = OFF;
	DWORD imsi;

	*value = 0;

	for(month = 1; month <= 12; month++) {
		for(day = 1; day <= 31; day++) {
			if(CatDataGetDiDay(terminal, tag, &imsi, year, month, day, DI_DATA_TYPE_COUNT)) {
				read_flag = ON;
				*value += imsi;
			}
		}
	}

	if(read_flag == OFF)	return 0;

	return 1;
}

int CatDataGetDiYear(int terminal, const char *tag, DWORD *value, int year, int data_type)
{
	if(data_type == DI_DATA_TYPE_ONTIME)
		return CatDataGetDiYearOnTime(terminal, tag, value, year);
	else if(data_type == DI_DATA_TYPE_COUNT)
		return CatDataGetDiYearCount(terminal, tag, value, year);
	else
		return 0;
}










// TAG size O.K
#include "stdafx.h"
#include <stdio.h>

#include <tools.h>

#include "..\catlib\catdata.h"
#include "..\catlib\cattag.h"
#include "..\catlib\TagShare.h"

#include <crc.hpp>
 
//#include "..\main\servmain.h"
//#include "..\main\main.h"



static double GetTagMemberFull(const char *tag)
{
	/*
	//short pos;

	//if(!GetTagPosAI(&terminalStruct[0], tag, pos))	return 0;

	TAG_AI_STRUCT *ai = TagShareGetAI(tag);
	if(ai == NULL)	return 0;
	//TAG_AI_STRUCT *ai = &terminalStruct[0].analogInput[pos];

	return ai->file.full;
	*/
	return 100;
}

static int ReadTrendStructAI(char *filename, char day, char hour, char min, TREND_AI_STRUCT *data)
{
	FILE *in;

	FILE_TREND_AI_STRUCT trend;

	in = fopen(filename, "rb");
	if(in == NULL) 	return 0;	// 자료 없슴

	long start = (long)sizeof(FILE_TREND_AI_STRUCT);
	start = start*((day-1)*1440+hour*60L+min);

	fseek(in, start, SEEK_SET);
	fread(&trend, 1, sizeof(FILE_TREND_AI_STRUCT), in);
	fclose(in);

	if(trend.day != day)						return 0;
	if(trend.hour != hour)						return 0;
	if(trend.min != min)						return 0;
	if(trend.crc != GetCRC16((BYTE*)&trend, sizeof(FILE_TREND_AI_STRUCT)-2))	return 0;

	memcpy(data, &trend.data, sizeof(TREND_AI_STRUCT));

	return 1;
}

void GetDuplexActiveDataDirectory(CString &directory);

int SmGetTrendStructAI(int terminal, int year, char mon, char day, char hour, char min, TREND_AI_STRUCT *trend, char *tag)
{
	char path[MAXPATH];
	char tag_file[80];
	CString data_dir;
	GetDuplexActiveDataDirectory(data_dir);

	ConvertTagToFile(tag_file, tag);

	sprintf(path, "%s\\TREND\\%04d\\MON%02d\\AI\\%s", data_dir, year, mon, tag_file);

	return ReadTrendStructAI(path, day, hour, min, trend);
}

//------------------------------------------------------------------------------
//	아날로그 분 데이타를 읽어온다.
//------------------------------------------------------------------------------

int CatDataGetAiMin(int terminal, const char *tag, float *value, int year, int month, int day, int hour, int min, int data_type)
{
	TREND_AI_STRUCT trend;
	//TERMINAL_STRUCT *ter = &terminalStruct[terminal];

	*value = 0.0;

	if(!SmGetTrendStructAI(terminal, year, month, day, hour, min, &trend, (char*)tag)) {
		return 0;
	}

	if(data_type == AI_DATA_TYPE_AVE) {
		*value = trend.fAverage;
	}
	else if(data_type == AI_DATA_TYPE_SUM) {
		*value = trend.fSumMin;
	}
	else if(data_type == AI_DATA_TYPE_MIN) {
		*value = trend.fMin;
	}
	else if(data_type == AI_DATA_TYPE_MAX) {
		*value = trend.fMax;
	}
	else if(data_type == AI_DATA_TYPE_SUB) {
		float value1;

		value1 = trend.fMax;

		MinusMin(year, month, day, hour, min);
		if(!SmGetTrendStructAI(terminal, year, month, day, hour, min, &trend, (char*)tag)) {
			return 0;
		}

		if(value1 < trend.fMax)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
			*value = (float)(GetTagMemberFull(tag)+value1-trend.fMax);
		else
			*value = value1-trend.fMax;
	}
	else {
		return 0;
	}

	return 1;
}

int SmPrepareFileData(int terminal, LPSTR path, LPCSTR file);

//------------------------------------------------------------------------------
//	아날로그 시간 데이타를 읽어온다.
//------------------------------------------------------------------------------

static int DataGetAiHourElse(int terminal, const char *tag, float *value, int year, int month, int day, int hour, int data_type)
{
   char tag_file[80];
	char path[MAXPATH];
	char filename[MAXPATH];

	HOUR_DATA_HEAD head;
	HOUR_DATA_ANALOG_STRUCT data;
	FILE *in;

	*value = 0.0;

	ConvertTagToFile(tag_file, tag);
	sprintf(filename, "SUM\\%04d\\MON%02d\\AI\\%s", year, month, tag_file);
	SmPrepareFileData(terminal, path, filename);

	in = fopen(path, "rb");
	if(in == NULL)			return 0;

	fseek(in, 0, SEEK_SET);

	if(fread(&head, 1, sizeof(HOUR_DATA_HEAD), in) != sizeof(HOUR_DATA_HEAD))	{
		fclose(in);
		return 0;
	}

	fseek(in, sizeof(HOUR_DATA_HEAD)+((day-1)*24L+hour)*sizeof(HOUR_DATA_ANALOG_STRUCT), SEEK_SET);
	if(fread(&data, 1, sizeof(HOUR_DATA_ANALOG_STRUCT), in) != sizeof(HOUR_DATA_ANALOG_STRUCT)) {
		fclose(in);
		return 0;
	}

	fclose(in);

	if(data.flag == OFF)	{
		return 0;	// 자료는 있으나 초기화 되어있는 값이다.
	}

	//if(data.crc != GetCRC16((BYTE*)&data, sizeof(HOUR_DATA_ANALOG_STRUCT)-2)) {
	//	return 0;
	//}

	if(data_type == AI_DATA_TYPE_AVE) {
		*value = data.fAveHour;
	}
	else if(data_type == AI_DATA_TYPE_SUM) {
		*value = data.fSumHour;
	}
	else if(data_type == AI_DATA_TYPE_MIN) {
		*value = data.fMinHour;
	}
	else if(data_type == AI_DATA_TYPE_MAX) {
		*value = data.fMaxHour;
	}
	else {
		return 0;
	}

	return 1;
}

int CatDataGetAiHour(int terminal, const char *tag, float *value, int year, int month, int day, int hour, int data_type)
{
	if(data_type == AI_DATA_TYPE_SUB) {
		float value1;
		float value2;

		if(!DataGetAiHourElse(terminal, tag, &value1, year, month, day, hour, AI_DATA_TYPE_MAX))
			return 0;

		MinusHour(year, month, day, hour);

		if(!DataGetAiHourElse(terminal, tag, &value2, year, month, day, hour, AI_DATA_TYPE_MAX))
			return 0;

		if(value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
			*value = (float)(GetTagMemberFull(tag)+value1-value2);
		else
			*value = value1-value2;

		return 1;
	}
	else {
		return DataGetAiHourElse(terminal, tag, value, year, month, day, hour, data_type);
	}
}

static int DataGetAiDayElse(int terminal, const char *tag, float *value, int year, int month, int day, int data_type)
{
	char tag_file[MAXPATH];
	char path[MAXPATH];
	char filename[MAXPATH];
	HOUR_DATA_HEAD head;
	HOUR_DATA_ANALOG_STRUCT data;
	char read_flag = OFF;
	int  read_count = 0;
	int  i;
	FILE *in;
//	float min = 0;
//	float max = 0;

	*value = 0.0;

	ConvertTagToFile(tag_file, tag);
	sprintf(filename, "SUM\\%04d\\MON%02d\\AI\\%s", year, month, tag_file);
	SmPrepareFileData(terminal, path, filename);

	in = fopen(path, "rb");
	if(in == NULL)			return 0;

	fseek(in, 0, SEEK_SET);

	if(fread(&head, 1, sizeof(HOUR_DATA_HEAD), in) != sizeof(HOUR_DATA_HEAD))	{
		fclose(in);
		return 0;
	}

	fseek(in, sizeof(HOUR_DATA_HEAD)+((day-1)*24L)*sizeof(HOUR_DATA_ANALOG_STRUCT), SEEK_SET);

	for(i = 0; i < 24; i++) {
		if(fread(&data, 1, sizeof(HOUR_DATA_ANALOG_STRUCT), in) != sizeof(HOUR_DATA_ANALOG_STRUCT)) {
			fclose(in);
			return 0;
		}
		//if(data.flag && data.crc == GetCRC16((BYTE*)&data, sizeof(HOUR_DATA_ANALOG_STRUCT)-2)) {
		if(data.flag) {
			read_flag = ON;
			read_count ++;

			if(data_type == AI_DATA_TYPE_AVE) {
				*value += data.fAveHour;
			}
			else if(data_type == AI_DATA_TYPE_SUM) {
				*value += data.fSumHour;
			}
			else if(data_type == AI_DATA_TYPE_MIN) {
				if(read_count == 1) {	// 처음으로 읽을때
					*value = data.fMinHour;
				}
				else {
					if(data.fMinHour < *value)	*value = data.fMinHour;
				}
			}
			else if(data_type == AI_DATA_TYPE_MAX) {
				if(read_count == 1) {	// 처음으로 읽을때
					*value = data.fMaxHour;
				}
				else {
					if(data.fMaxHour > *value)	*value = data.fMaxHour;
				}
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

	if(data_type == AI_DATA_TYPE_AVE) {	// 평균치는 평균값으로 계산한다.
		*value = *value/read_count;
	}

	return 1;
}

int CatDataGetAiDay(int terminal, const char *tag, float *value, int year, int month, int day, int data_type)
{
	*value = 0.0;

	if(data_type == AI_DATA_TYPE_SUB) {
		float value1;
		float value2;

		if(!DataGetAiDayElse(terminal, tag, &value1, year, month, day, AI_DATA_TYPE_MAX))
			return 0;
		MinusDay(year, month, day);
		if(!DataGetAiDayElse(terminal, tag, &value2, year, month, day, AI_DATA_TYPE_MAX))
			return 0;

		if(value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
			*value = (float)(GetTagMemberFull(tag)+value1-value2);
		else
			*value = value1-value2;

		return 1;
	}
	else {
		return DataGetAiDayElse(terminal, tag, value, year, month, day, data_type);
	}
}

/*
이 함수는 속도는 빠르다. 이전에 사용했으나 실제 월평균을 모든 시간데이타를 읽어서 시간을 낸다.
이것은 실제 사용자 데이타와 맞지않는 경향이 있다. 
속도는 조금 지연되나 하루하루의 평균을 읽어서 평균을 계산하는것이 사용자가 원하는
데이타와 일치한다.

static int DataGetAiMonthElse(int terminal, char *tag, float *value, int year, int month, int data_type)
{
	char tag_file[MAXPATH];
	char path[MAXPATH];
	char filename[MAXPATH];
	HOUR_DATA_HEAD head;
	HOUR_DATA_ANALOG_STRUCT data;
	char read_flag = OFF;
	int  read_count = 0;
	int  i;
	FILE *in;
	float min = 0;
	float max = 0;

	*value = 0.0;

	ConvertTagToFile(tag_file, tag);
	sprintf(filename, "SUM\\%04d\\MON%02d\\AI\\%s", year, month, tag_file);
	SmPrepareFileData(terminal, path, filename);

	in = fopen(path, "rb");
	if(in == NULL)			return 0;

	fseek(in, 0, SEEK_SET);

	if(fread(&head, 1, sizeof(HOUR_DATA_HEAD), in) != sizeof(HOUR_DATA_HEAD))	{
		fclose(in);
		return 0;
	}

	//fseek(in, sizeof(HOUR_DATA_HEAD)+((day-1)*24L)*sizeof(HOUR_DATA_ANALOG_STRUCT), SEEK_SET);

	for(i = 0; i < 24*31; i++) {	// 한달의 데이타를 모두 읽는다.
		if(fread(&data, 1, sizeof(HOUR_DATA_ANALOG_STRUCT), in) != sizeof(HOUR_DATA_ANALOG_STRUCT)) {
			continue;
		}
		//if(data.flag && data.crc == GetCRC16((BYTE*)&data, sizeof(HOUR_DATA_ANALOG_STRUCT)-2)) {
		if(data.flag) {
			read_flag = ON;
			read_count ++;

			if(data_type == AI_DATA_TYPE_AVE) {
				*value += data.fAveHour;
			}
			else if(data_type == AI_DATA_TYPE_SUM) {
				*value += data.fSumHour;
			}
			else if(data_type == AI_DATA_TYPE_MIN) {
				if(read_count == 1) {	// 처음으로 읽을때
					*value = data.fMinHour;
				}
				else {
					if(data.fMinHour < *value)	*value = data.fMinHour;
				}
			}
			else if(data_type == AI_DATA_TYPE_MAX) {
				if(read_count == 1) {	// 처음으로 읽을때
					*value = data.fMaxHour;
				}
				else {
					if(data.fMaxHour > *value)	*value = data.fMaxHour;
				}
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

	if(data_type == AI_DATA_TYPE_AVE) {	// 평균치는 평균값으로 계산한다.
		*value = *value/read_count;
	}

	return 1;
}

int CatDataGetAiMonth(int terminal, char *tag, float *value, int year, int month, int data_type)
{
	*value = 0.0;

	if(data_type == AI_DATA_TYPE_SUB) {
		float value1;
		float value2;

		if(!DataGetAiMonthElse(terminal, tag, &value1, year, month, AI_DATA_TYPE_MAX))
			return 0;
		MinusMonth(year, month);
		if(!DataGetAiMonthElse(terminal, tag, &value2, year, month, AI_DATA_TYPE_MAX))
			return 0;
		*value = value1-value2;

		return 1;
	}
	else {
		return DataGetAiMonthElse(terminal, tag, value, year, month, data_type);
	}
}
*/

static int DataGetAiMonthElse(int terminal, const char *tag, float *value, int year, int month, int data_type)
{
	char read_flag = OFF;
	int  read_count = 0;
	int  day;
	//float min = 0;
	//float max = 0;
	float day_value = 0;

	*value = 0.0;

	for(day = 1; day <= 31; day++) {	// 한달의 데이타를 모두 읽는다.
		if(!DataGetAiDayElse(terminal, tag, &day_value, year, month, day, data_type))	continue;

		read_flag = ON;
		read_count ++;

		if(data_type == AI_DATA_TYPE_AVE) {
			*value += day_value;
		}
		else if(data_type == AI_DATA_TYPE_SUM) {
			*value += day_value;
		}
		else if(data_type == AI_DATA_TYPE_MIN) {
			if(read_count == 1) {	// 처음으로 읽을때
				*value = day_value;
			}
			else {
				if(day_value < *value)	*value = day_value;
			}
		}
		else if(data_type == AI_DATA_TYPE_MAX) {
			if(read_count == 1) {	// 처음으로 읽을때
				*value = day_value;
			}
			else {
				if(day_value > *value)	*value = day_value;
			}
		}
		else {
			return 0;
		}
	}

	if(read_flag == OFF) {
		return 0;	// 읽은 데이타가 없다.
	}

	if(data_type == AI_DATA_TYPE_AVE) {	// 평균치는 평균값으로 계산한다.
		*value = *value/read_count;
	}

	return 1;
}

int CatDataGetAiMonth(int terminal, const char *tag, float *value, int year, int month, int data_type)
{
	*value = 0.0;

	if(data_type == AI_DATA_TYPE_SUB) {
		float value1;
		float value2;

		if(!DataGetAiMonthElse(terminal, tag, &value1, year, month, AI_DATA_TYPE_MAX))
			return 0;
		MinusMonth(year, month);
		if(!DataGetAiMonthElse(terminal, tag, &value2, year, month, AI_DATA_TYPE_MAX))
			return 0;

		if(value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
			*value = (float)(GetTagMemberFull(tag)+value1-value2);
		else
			*value = value1-value2;

		return 1;
	}
	else {
		return DataGetAiMonthElse(terminal, tag, value, year, month, data_type);
	}
}

static int CatDataGetAiYearAve(int terminal, const char *tag, float *value, int year)
{
	int month;
	char read_flag = OFF;
	int read_count = 0;
	float imsi;

	*value = 0.0;

	for(month = 1; month <= 12; month++) {
		if(CatDataGetAiMonth(terminal, tag, &imsi, year, month, AI_DATA_TYPE_AVE)) {
			read_flag = ON;
			read_count++;
			*value += imsi;
		}
	}

	if(read_flag == OFF)	return 0;

	*value = *value/read_count;

	return 1;
}

static int CatDataGetAiYearSum(int terminal, const char *tag, float *value, int year)
{
	int month;
	char read_flag = OFF;
	float imsi;

	*value = 0.0;

	for(month = 1; month <= 12; month++) {
		if(CatDataGetAiMonth(terminal, tag, &imsi, year, month, AI_DATA_TYPE_SUM)) {
			read_flag = ON;
			*value += imsi;
		}
	}

	if(read_flag == OFF)	return 0;

	return 1;
}

static int CatDataGetAiYearMin(int terminal, const char *tag, float *value, int year)
{
	int month;
	char read_flag = OFF;
	int read_count = 0;
	float imsi;

	*value = 0.0;

	for(month = 1; month <= 12; month++) {
		if(CatDataGetAiMonth(terminal, tag, &imsi, year, month, AI_DATA_TYPE_MIN)) {
			read_flag = ON;
			read_count ++;

			if(read_count == 0) {
				*value = imsi;
			}
			else {
				if(imsi < *value)	*value = imsi;
			}
		}
	}

	if(read_flag == OFF)	return 0;

	return 1;
}

static int CatDataGetAiYearMax(int terminal, const char *tag, float *value, int year)
{
	int month;
	char read_flag = OFF;
	int read_count = 0;
	float imsi;

	*value = 0.0;

	for(month = 1; month <= 12; month++) {
		if(CatDataGetAiMonth(terminal, tag, &imsi, year, month, AI_DATA_TYPE_MAX)) {
			read_flag = ON;
			read_count ++;

			if(read_count == 0) {
				*value = imsi;
			}
			else {
				if(imsi > *value)	*value = imsi;
			}
		}
	}

	if(read_flag == OFF)	return 0;

	return 1;
}

static int CatDataGetAiYearSub(int terminal, const char *tag, float *value, int year)
{
	*value = 0.0;

	float max1, max2;

	if(!CatDataGetAiYearMax(terminal, tag, &max1, year))
		return 0;
	MinusYear(year);
	if(!CatDataGetAiYearMax(terminal, tag, &max2, year))
		return 0;

	*value = max1-max2;

	return 1;
}

int CatDataGetAiYear(int terminal, const char *tag, float *value, int year, int data_type)
{
	if(data_type == AI_DATA_TYPE_AVE)
		return CatDataGetAiYearAve(terminal, tag, value, year);
	else if(data_type == AI_DATA_TYPE_SUM)
		return CatDataGetAiYearSum(terminal, tag, value, year);
	else if(data_type == AI_DATA_TYPE_MIN)
		return CatDataGetAiYearMin(terminal, tag, value, year);
	else if(data_type == AI_DATA_TYPE_MAX)
		return CatDataGetAiYearMax(terminal, tag, value, year);
	else
   	return CatDataGetAiYearSub(terminal, tag, value, year); 
}










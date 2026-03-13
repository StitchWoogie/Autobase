// TAG size O.K
#if	!defined (__CATDATA_H)
#define __CATDATA_H

#pragma pack(push, 1)

enum {	// 아날로그 자료의 형태.
	AI_DATA_TYPE_AVE,
	AI_DATA_TYPE_MIN,
	AI_DATA_TYPE_MAX,
	AI_DATA_TYPE_SUM,
	AI_DATA_TYPE_SUB,
	AI_DATA_TYPE_CURR,
	AI_DATA_TYPE_MOMENT,		// 순시값
};

enum {	// 디지탈 자료의 형태.
	DI_DATA_TYPE_ONTIME,	// On된 시간.
	DI_DATA_TYPE_OFFTIME,	// Off된 시간.
	DI_DATA_TYPE_COUNT,		// on/off count
	DI_DATA_TYPE_CURR,
	DI_DATA_TYPE_MOMENT,	// 순시값
};

//-----------------------------------------------------------------------------------
// 밑의 OLD STRUCT는 6.0 이전의 버전이다. 현재는 LOG702프로그램에서만 사용하고 있다.
//-----------------------------------------------------------------------------------

typedef struct {
	char	version;
	short	year;
	char	month;
	WORD	crc;				// data 의 crc
} OLD_TREND_HEAD_MIN;

typedef struct {
	float		fSumMin;		// 1분 동안에 흘렀을 유량 (실제 적산치)
	float		fAverage;		// 1분 동안에 계측된 값의 평균 (적산 평균 아님)
	float		fMin;			// 1분 동안의 최소값
	float		fMax;			// 1분 동안의 최소값
} OLD_TREND_ANALOG_STRUCT;

typedef struct {
	short	nCountOnOff;		// 1분동안 접점이 ON/OFF 된 횟수.
	char	bOnOff;				// 자료 저장 시의 ON/OFF 상태
	char	cOnTime;			// 1분동안 접점이 ON 된 시간(단위 sec)
} OLD_TREND_DIGITAL_STRUCT;

typedef struct {
	float		fSumMin;	// 1분 동안에 흘렀을 유량 (실제 적산치)
	float		fAverage;	// 1분 동안에 계측된 값의 평균 (적산 평균 아님)
	float		fMin;		// 1분 동안의 최소값
	float		fMax;		// 1분 동안의 최소값
	float		fCurr;		// 저장 당시의 현재값.
} TREND_AI_STRUCT;

typedef struct {
	char		day;			// 일
	char		hour;			// 월
	char		min;			// 분
	TREND_AI_STRUCT data;
	WORD		crc;			// data 의 crc
} FILE_TREND_AI_STRUCT;

typedef struct {
	short		nCountOnOff;	// 1분동안 접점이 ON/OFF 된 횟수.
	char		bOnOff;			// 자료 저장 시의 ON/OFF 상태
	char		cOnTime;		// 1분동안 접점이 ON 된 시간(단위 sec)
} TREND_DI_STRUCT;

typedef struct {
	char		day;		// 일
	char		hour;		// 월
	char		min;		// 분
	TREND_DI_STRUCT data;
	WORD		crc;		// data 의 crc
} FILE_TREND_DI_STRUCT;

typedef struct {
	char	id[5];			// HOUR
	short	version;		// 1
	char	extra[11];		// 이전에는 태그이름으로 사용했으나 40자로 증가되면서 무의미한 배열이 됨.
} HOUR_DATA_HEAD;

typedef struct {
	float		fSumHour;		// 한 시간 동안에 흘렀을 적산치
	float		fAveHour;		// 한 시간 동안의 계측 평균치 (적산평균치 아님)
	float		fMinHour;		// 한 시간 동안의 최소치
	float		fMaxHour;		// 한 시간 동안의 최고치
	float		fCurrSumMeter;	// 시간대 마지막에 계측된 적산 계량기 눈금
	char		flag;			// OFF이면 파일 초기화만 되어 있고 저장되지는 않았다.
	WORD		crc;
} HOUR_DATA_ANALOG_STRUCT;

typedef struct {
	WORD		wCountOnOff;	// 한 시간동안 ON/OFF 된 횟수.
	DWORD		dwOnTime;		// 접점이 ON 된 시간 (단위 sec)
	char		flag;			// OFF이면 파일 초기화만 되어 있고 저장되지는 않았다.
	WORD		crc;
} HOUR_DATA_DIGITAL_STRUCT;

int CatDataGetAiMin(int terminal, const char *tag, float *value, int year, int month, int day, int hour, int min, int data_type);
int CatDataGetAiHour(int terminal, const char *tag, float *value, int year, int month, int day, int hour, int data_type);
int CatDataGetAiDay(int terminal, const char *tag, float *value, int year, int month, int day, int data_type);
int CatDataGetAiMonth(int terminal, const char *tag, float *value, int year, int month, int data_type);
int CatDataGetAiYear(int terminal, const char *tag, float *value, int year, int data_type);

int CatDataGetDiMin(int terminal, const char *tag, DWORD *value, int year, int month, int day, int hour, int min, int data_type);
int CatDataGetDiHour(int terminal, const char *tag, DWORD *value, int year, int month, int day, int hour, int data_type);
int CatDataGetDiDay(int terminal, const char *tag, DWORD *value, int year, int month, int day, int data_type);
int CatDataGetDiMonth(int terminal, const char *tag, DWORD *value, int year, int month, int data_type);
int CatDataGetDiYear(int terminal, const char *tag, DWORD *value, int year, int data_type);

void ConvertTagToFile(char *buf, const char *tag);

/*
int CatDataGetHourAve(int pos, float *value, int year, int month, int day, int hour);
int CatDataGetHourSum(int pos, float *value, int year, int month, int day, int hour);
int CatDataGetHourMin(int pos, float *value, int year, int month, int day, int hour);
int CatDataGetHourMax(int pos, float *value, int year, int month, int day, int hour);

int CatDataGetDayAve(int pos, float *value, int year, int month, int day);
int CatDataGetDaySum(int pos, float *value, int year, int month, int day);
int CatDataGetDayMin(int pos, float *value, int year, int month, int day);
int CatDataGetDayMax(int pos, float *value, int year, int month, int day);

int CatDataGetMonthAve(int pos, float *value, int year, int month);
int CatDataGetMonthSum(int pos, float *value, int year, int month);
int CatDataGetMonthMin(int pos, float *value, int year, int month);
int CatDataGetMonthMax(int pos, float *value, int year, int month);
*/

void AutoBaseDataSaveMinTrendAI(char *tag, struct date *d, struct time *t, float value);
void AutoBaseDataSaveMinTrendAI(char *tag, struct date *d, struct time *t, float val_ave, float val_min, float val_max, float val_sum, float val_curr);

void AutoBaseDataSaveHourTrendAI(char *tag, struct date *d, struct time *t);
void AutoBaseDataSaveHourAIByValue(char *tag, struct date *d, struct time *t, float value);
void AutoBaseDataSaveHourAIByValue(char *tag, struct date *d, struct time *t, float fHap, float fMin, float fMax, float fAve);

#pragma pack(pop)

#endif


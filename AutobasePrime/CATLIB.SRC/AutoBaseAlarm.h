// TAG size O.K
#if	!defined(__ALARM_H)
#define __ALARM_H

#if	!defined(__DOS_H)
#include <dos.h>
#endif

#include "..\catlib\cattag.h"

#pragma pack(push, 1)

typedef struct {
	SYSTEMTIME	t;
	char		tag[40];
	char		description[80];
	char		string[80];
	WORD		alarm_type;
	WORD		priority; // 경보 우선권 (0~999)
	WORD		port;
	WORD		station;
	DWORD		address;
	WORD		type;
	WORD		crc;
} ALARM_FILE_STRUCT;

typedef struct {
	SYSTEMTIME	t;
	char		tag[11];
	char		description[21];
	char		string[80];
	WORD		alarm_type;
	WORD		priority; // 경보 우선권 (0~999)
	WORD		port;
	WORD		station;
	DWORD		address;
	WORD		type;
	WORD		crc;
} OLD_ALARM_FILE_STRUCT2;	// *.AL2

typedef struct {
	ALARM_FILE_STRUCT file;
	char	retn_or_event;		// ON = new 발생 OFF = return alarm
} ALARM_RECV_EVENT_STRUCT;



enum alarmMSGtype{
	ALARM_TYPE_HIHI = 0,
	ALARM_TYPE_HIGH = 1,
	ALARM_TYPE_LOW = 2,
	ALARM_TYPE_LOLO = 3,
	ALARM_TYPE_DI_ON = 4,				// 디지탈 입력이 ON되었을 때 경보
	ALARM_TYPE_DI_OFF = 5,				// 디지탈 입력이 OFF되었을 때 경보
	ALARM_TYPE_RETURN = 6,				// 경보에서 정상으로 복귀했을 때
	ALARM_TYPE_HAND_OPERATION = 7,		// 조작자가 디지탈을 수동으로 조작했을 때.
	ALARM_OVER_RATE_OF_CHANGE_LIMIT = 8,// 변화율 범위 초과.
	ALARM_TYPE_HAND_INPUT = 9,			// 수동기입 경보.
	ALARM_TYPE_DEMAND_CONTROL = 10,		// Demand Control
	ALARM_TYPE_LOG = 11,				// LogIn Out 정보
};

#define	MAX_ALARM_MSG_TYPE	11			// 경보 메세지의 유형 개수
#define ALARM_FILE_EXT					"AL3"	// alarm file extension

void AlarmDisplayAI(TAG_AI_STRUCT *ai, const char *message, WORD msg, char retn_or_event);
void AlarmDisplayDI(TAG_DI_STRUCT *di, const char *message, WORD msg, char retn_or_event);
void AlarmDataSave(ALARM_FILE_STRUCT *alarm);

void AlarmDataSave(TAG_AI_STRUCT *ai, const char *string, WORD msg_type);
void AlarmDataSave(TAG_DI_STRUCT *di, const char *string, WORD msg_type);
void AlarmDataSave(TAG_DO_STRUCT *dout, const char *string, WORD msg_type);

void FillAlarmStruct(ALARM_FILE_STRUCT *alarm, TAG_AI_STRUCT *ai, SYSTEMTIME *t, const char *string, WORD msg_type);
void FillAlarmStruct(ALARM_FILE_STRUCT *alarm, TAG_DI_STRUCT *di, SYSTEMTIME *t, const char *string, WORD msg_type);
void FillAlarmStruct(ALARM_FILE_STRUCT *alarm, TAG_DI_STRUCT *dout, SYSTEMTIME *t, const char *string, WORD msg_type);

void AlarmSound(char *alarm_wave, WORD priority);

#pragma pack(pop)

#endif


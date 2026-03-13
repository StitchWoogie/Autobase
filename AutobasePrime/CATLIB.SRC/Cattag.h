// TAG size O.K
#if	!defined (__CATTAG_H)
#define __CATTAG_H

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#if	!defined (__CATDATA_H)
#include "catdata.h"
#endif

#pragma pack(push, 1)

#define	MAX_TERMINAL	256

#define	MAX_ANALOG_INPUT	30000L
#define	MAX_ANALOG_OUTPUT	30000L
#define	MAX_DIGITAL_INPUT	30000L
#define MAX_DIGITAL_OUTPUT	30000L
#define MAX_STRING_TAG		30000L

#define MAX_TAG_NAME		40
#define MAX_TAG_DES			80

enum {
	TAG_MEMBER_tag = 0,
	TAG_MEMBER_description = 1,
	TAG_MEMBER_port = 2,
	TAG_MEMBER_station = 3,
	TAG_MEMBER_address = 4,
	TAG_MEMBER_fn = 5,
	TAG_MEMBER_unit = 6,
	TAG_MEMBER_desON = 7,
	TAG_MEMBER_desOFF = 8,
	TAG_MEMBER_base = 9,
	TAG_MEMBER_mid = 10,
	TAG_MEMBER_full = 11,
	TAG_MEMBER_ratio = 12,
	TAG_MEMBER_sp = 13,
	TAG_MEMBER_hihi = 14,
	TAG_MEMBER_high = 15,
	TAG_MEMBER_low = 16,
	TAG_MEMBER_lolo = 17,
	TAG_MEMBER_act = 18,
	TAG_MEMBER_alarm = 19,
	TAG_MEMBER_bFileSave = 20,
	TAG_MEMBER_fDisplayFormat = 21,
	TAG_MEMBER_cAlarmType = 22,
	TAG_MEMBER_sGraphicFile = 23,
	TAG_MEMBER_sAlarmWaveFile = 24,
	TAG_MEMBER_nCalculateFilter = 25,
	
	TAG_MEMBER_curr = 26,
	TAG_MEMBER_fSumTotal = 27,		// fSumTotal;	현재까지 세어진 유량.

	TAG_MEMBER_viewbase = 28,
	TAG_MEMBER_viewfull = 29,
	TAG_MEMBER_extra1 = 30,
	TAG_MEMBER_extra2 = 31,

	TAG_MEMBER_assign = 32,				// indirect 태그를 assign해 준다.
	TAG_MEMBER_NeedAlarmConfirm = 33,	// 알람 confirm이 필요하다.
	TAG_MEMBER_ProtectScan = 34,		// 스캔 금지중인가의 여부.
	TAG_MEMBER_ProtectControl = 35,		// 콘트롤 금지중인가의 여부.
	TAG_MEMBER_ProtectAlarmEvent = 36,		// 이벤트 금지중인가의 여부.
	TAG_MEMBER_ProtectAlarmData = 37,		// 경보 금지중인가의 여부.

	TAG_MEMBER_fSumPart = 38,		// fSumTotal;	현재까지 세어진 부분 유량.
	TAG_MEMBER_bBcdValue = 39,		// AI/AO		
	TAG_MEMBER_cConfirmCount = 40,	// AI/DI		
	TAG_MEMBER_bCutOverValue = 41,	// AI/AO
	TAG_MEMBER_cTagType = 42,		// AI/AO/DI/DO
	TAG_MEMBER_fAlarmReturnGab = 43,
	TAG_MEMBER_nCalcDelay = 44,
	TAG_MEMBER_sDdeItem = 45,
	TAG_MEMBER_sDdeService = 46,
	TAG_MEMBER_sDdeTopic = 47,

	TAG_MEMBER_sSubOutAnalog = 48,
	TAG_MEMBER_sSubOutAnalogSP = 49,
	TAG_MEMBER_sSubOutDigitalHiHi = 50,
	TAG_MEMBER_sSubOutDigitalLoLo = 51,

	TAG_MEMBER_wAlarmPriority = 52,
	TAG_MEMBER_wProtectFlags = 53,
	TAG_MEMBER_wRateOfChangeLimit = 54,

	TAG_MEMBER_bReverse = 55,
	TAG_MEMBER_cOutLinkMethod = 56,
	TAG_MEMBER_sSubOutDigital1 = 57,
	TAG_MEMBER_sSubOutDigital2 = 58,
	TAG_MEMBER_sSubOutDigitalOffTag = 59,
	TAG_MEMBER_sSubOutDigitalOnTag = 60,

	TAG_MEMBER_cRelayType = 61,
	TAG_MEMBER_wRelaySecTarget = 62,
	//TAG_MEMBER_wWriteRetryTime = 63,

	TAG_MEMBER_cDdeDataFormat = 64,
	TAG_MEMBER_plc_base = 65,
	TAG_MEMBER_plc_full = 66,

	TAG_MEMBER_cAlarmLevelStatus = 70,

	TAG_MEMBER_cAlarmProtectOnBigChangePercent = 71,
	TAG_MEMBER_cAlarmProtectOnBigChangeSecond = 72,
	TAG_MEMBER_bDdeRequest = 73,
	TAG_MEMBER_wScanTime = 74,
	TAG_MEMBER_ID = 75,
};

// 멤버의 형태
enum {
	MEMBER_VAR_int,
	MEMBER_VAR_float,
	MEMBER_VAR_string,
	MEMBER_VAR_double,
};

// 스크립트에서 상태를 읽어간다.
enum {
	LEVEL_NORMAL = 0,
	LEVEL_LOLO = 1,
	LEVEL_LOW = 2,
	LEVEL_HIGH = 3,
	LEVEL_HIHI = 4,
};

enum {
	TAG_TYPE_AI = 0,
	TAG_TYPE_AO = 1,
	TAG_TYPE_DI = 2,
	TAG_TYPE_DO = 3,
	TAG_TYPE_ST = 9,
	TAG_TYPE_GDO = 10,
	TAG_TYPE_UNKNOWN = 100,	// 태그가 아닌 어떤값일 수 있다.
};

typedef struct {
	char	tag[40];
	short	pos;
} ASSIGN_TAG_STRUCT;

typedef struct {
	struct date d;
	struct time t;
	TREND_AI_STRUCT trend;
} AI_TREND_REMAIN;

typedef struct {
	struct date d;
	struct time t;
	TREND_DI_STRUCT trend;
} DI_TREND_REMAIN;

#define	PROTECT_FLAG_SCAN			0x0001
#define	PROTECT_FLAG_CONTROL		0x0002
#define	PROTECT_FLAG_ALARM_DATA		0x0004
#define	PROTECT_FLAG_ALARM_EVENT	0x0008

typedef struct {
	WORD    struct_size;
	int		tag_type;
	char	bWrite;
	//DWORD   update_flags;				// 각 프로그램에서 현재치를 Update하기 위해서 필요한 플래그
	//									// 값이 바뀌었으면 ON 바뀐값을 적용했으면 OFF가 된다.
} SHARED_TAG_PUBLIC;

typedef struct {
	SHARED_TAG_PUBLIC	pub;
	double	curr;
	double	old_curr;	// 실제 이 변수는 RunMain에서 이벤트를 발생시키기 위해서 필요하다.
	double	fWriteValue;
} SHARED_TAG_AI;

typedef struct {
	SHARED_TAG_PUBLIC	pub;
	double	curr;
	double	fWriteValue;
} SHARED_TAG_AO;

typedef struct {
	SHARED_TAG_PUBLIC	pub;
	char	curr;
	char	old_curr;	// 실제 이 변수는 RunMain에서 이벤트를 발생시키기 위해서 필요하다.
	char	cWriteValue;
} SHARED_TAG_DI;

typedef struct {
	SHARED_TAG_PUBLIC	pub;
	char	curr;
	char	cWriteValue;
	int		nDelaySec;
} SHARED_TAG_DO;

typedef struct {
	SHARED_TAG_PUBLIC	pub;
	char	curr[256];
	char	old_curr[256];// 실제 이 변수는 RunMain에서 이벤트를 발생시키기 위해서 필요하다.
	char	sWriteValue[256];
} SHARED_TAG_ST;

typedef struct {
	WORD 	struct_size;
	char	tag[40];
	char	description[80];
	short	port;
	short	station;				// plc station
	WORD 	address;
	short 	fn;						// 0 - 계산시 WORD 전체값 사용
									// 1 - 계산시 HIBYTE만 사용
									// 2 - 계산시 LOBYTE만 사용
	char 	unit[7];
	float 	base;
	float 	mid;
	float 	full;
	float 	ratio;
	//float 	sp;					// 원래 사용하지 않았다. (7.83부터 뺐다.)
	float 	hihi;
	float 	high;
	float 	low;
	float 	lolo;
	char   	act;
	char	alarm;					// 경보를 울릴것이냐 말것이냐?
	char	bFileSave;				// 데이터 파일을 저장할것이냐???
	char  	sSubOutDigitalHiHi[40];	// HiHi일때 출력할 디지털 태그
	char  	sSubOutDigitalLoLo[40];	// LoLo일때 출력할 디지털 태그
	char  	sSubOutAnalog[40]; 		// 수치가 바뀌면 무조건 결과를 출력하는 아나로그 출력 태그
	float	fDisplayFormat;			// 글자를 표시할 때 방법
									// 0 - default	10.2f
									// 8.2  2.0  등의 수치를 쓸 수 있다.
	char	cAlarmType;				// 경보를 울리면 어떤 조건에서 울리느냐?
	char	sGraphicFile[40];	 	// 주어진 태그를 가장 잘 확인 할 수 있는 그래픽모듈파일?
	char	sAlarmWaveFile[40];		// 경보 발생 시 사용할 음성 파일
	short	nCalculateFilter;		// 계산 방법, 0 - 보통의 계산
	char  	sSubOutAnalogSP[40];	// 아날로그 출력(이 아날로그 출력을 변경하면 아날로그 입력도 바뀐다.
//	WORD	wAlarmBitON;			// 아날로그 값의 Bit경보를 출력한다.
	char	bCutOverValue;			// 아날로그 값의 계산치가 RANGE를 벗어났을 때는 값을 최대 최소로 잘라주는 옵션.
	float 	view_full;				// 보여주는 최고 범위
	float 	view_base;              // 보여주는 최소 범위
	short	nCalcDelay;				// 값을 읽을때 지난값과 평균하여 사용. 0-사용하지 않는다.
	WORD	wAlarmPriority; 		// 경보 우선권
	char	cTagType; 				// 0=PLC_SCAN, 1=DDE, 2=메모리태그, 3=간접태그
	char	sDdeService[21];		// dde service
	char	sDdeTopic[40];			// dde Topic
	char	sDdeItem[40];			// dde item
	char	cConfirmCount;			// confirm을 할 count default=0 confirm을 하지 않는다.
	char	bBcdValue;				// BCD로 읽을 것인가?
	float	fAlarmReturnGab;		// 알람에서 복귀할때 gab만큼 낮추거나 높인 수치에서 복귀한다. 
	WORD	wRateOfChangeLimit;		// 이전값과 비교해서 지정해 놓은 값 이상이 차이나면 경보를 울린다. 0 = 사용안함.
	WORD	wProtectFlags;			// 각종 금지 플래그들
	BYTE	cAlarmProtectOnBigChangePercent;	// 과 변화가 되었을 때 경보를 지연시키는 기능
	BYTE	cAlarmProtectOnBigChangeSecond;		// 과 변화가 되었을 때 경보를 지연시키는 기능
	BYTE	bDdeRequest;
	WORD	wScanTime;				// 0 - 일반 스캔, 그 이외의 값은 지정한 스캔 Time에 따라 움직임.
	char	bLocalTag;				// 로컬태그로 사용
} AI_FILE_STRUCT;

typedef struct {
//	char		type;		// 0, 1, 2, 3

	AI_FILE_STRUCT file;
	ASSIGN_TAG_STRUCT assign;			// 태그가 간접태그일때만 사용한다.

	short		nTerminal;				// terminal 번호
	WORD		buf_pos;				// struct 에서의 위치.
	double		curr;
	double		old;
	unsigned _int64		real_curr;
	unsigned _int64 	real_old;
	char		cAlarmLevelStatus;
	char		cSubCheckLevelStatus;
	char		bNeedAlarmConfirm;		// 경보가 발생하면 이 플래그가 살고 사용자가 확인해 주거나 경보가 복귀되면 이 플래그는 죽는다.

	short  		nSubOutDigitalHiHi;		// HiHi일때 출력할 디지털 태그
	short  		nSubOutDigitalLoLo;		// LoLo일때 출력할 디지털 태그
	short  		nSubOutAnalog; 			// 수치가 바뀌면 무조건 결과를 출력하는 아나로그 출력 태그
	short  		nSubOutAnalogSP;		// 아날로그 출력(이 아날로그 출력을 변경하면 아날로그 입력도 바뀐다.

	short		nScanCount;					// 적산을 하기위해 1분동안 변한 아나로그 횟수
	double		fMinHap;   					// 적산을 1분동안 더해 놓은값
	double		fMinMin;					// 1분동안 측정된값 중에서 최소값
	double		fMinMax;					// 1분동안 측정된값 중에서 최대값
	double		fSumTotal;					// 현재까지 세어진 유량
	double		fSumPart;     				// 부분별로 세어진 유량
	struct date	dSumTotal;					// 적산 시작한 날짜
	struct time	tSumTotal; 					// 적산 시작한 시간
	struct date	dSumPart;  					// 적산 시작한 날짜
	struct time	tSumPart;  					// 적산 시작한 시간
	double		fAlarmMaxValue;				// 알람이 발생하고 난 뒤 최고로 올라간 값, 경보출력시 최고값으로 쓴다.
	double		fAlarmMinValue;				// 알람이 발생하고 난 뒤 최저로 내려간 값, 경보출력시 최저값으로 쓴다.
	char		bTagChangeFlag;				// 태그 속성이 바뀌었다(범위나). 그래서 새로운 계산을 화면에 표시해 줄필요가 있다.
	char		bCurrentAlarmStatus;		// 현재 경보상태

	char		bDdeLinkFlag;				// DdeTag가 접속되었느냐?
	DWORD		dwDdeService;				// dde pos
	DWORD		dwDdeTopic;					// dde pos
	DWORD		dwDdeItem;					// dde pos

	char				cTrendRemainCount;	// 남아있는 trend count
	AI_TREND_REMAIN		remain[10];			// 남아있는 trend 구조체

	char		bRightOperation;			// 사용자가 이 태그를 운전할 수 있는 권한이 있는냐?

	char  		cTypeSubOutDigitalHiHi;		// HiHi일때 출력할 디지털 태그
	char  		cTypeSubOutDigitalLoLo;		// LoLo일때 출력할 디지털 태그

	double		sosu_curr;					// AI 계산시 표시형식과 일치를 사용하게 되면 누적측정이 되지 않는다. 그 문제를 해결하기위한 변수
	double		sosu_old;					// AI 계산시 표시형식과 일치를 사용하게 되면 누적측정이 되지 않는다. 그 문제를 해결하기위한 변수

	char		cAlarmProtectOnBigChangeCount;	// 과 변화가 되었을 때 경보를 지연시키는 기능에서 사용하는 기능
	char		cAlarmProtectOnBigChangeOldSec;	// 과 변화가 되었을 때 경보를 지연시키는 기능에서 사용하는 기능

	WORD		wScanTimeCurr;				// 0 - 일반 스캔, 그 이외의 값은 지정한 스캔 Time에 따라 움직임.

	//char		extra[1];					// 2의 n승을 맞추기 위해 존재한다. 
	// 현재 1024바이트

	HANDLE			hSharedTag;
	SHARED_TAG_AI	*pSharedTag;
} ANALOG_INPUT_STRUCT;

typedef struct {
	WORD		struct_size;
	char		tag[40];
	char		description[80];
	short		port;
	short		station;		// plc station
	DWORD 		address;
	char 		sExtraAddr[40];	// PLC 기종에 따라 틀리다, FUJI(B, M, D...) MELSEC(X, Y, ...)
	WORD		wExtraAddr;		// 어드레스 이외에 통신에 필요할 수 있는 값을 넣을수 있게 했다.
	short 		fn;
	char 		unit[7];
	float 		base;
	float 		full;
	char   		act;
//	char		alarm;
	short		nCalculateFilter;		// 계산 방법, 0 - 계산하지 않고 그대로 출력
	float 		plc_base;				// 출력 최소값
	float 		plc_full;				// 출력 최대값
//	WORD		wWriteRetryTime;		// 지정한 시간이 경과한 후 데이터를 써 준다. 0 - disable , sec
	char		cTagType; 				// 0=PLC_SCAN, 1=DDE,
	char		sDdeService[21];		// dde service
	char		sDdeTopic[40];			// dde Topic
	char		sDdeItem[40];			// dde item
	char		cDdeDataFormat;			// DDE 출력시 데이터 형태
	char		bBcdValue;				// BCD로 출력할 것인가?
	char		bCutOverValue;			// 초과치는 자른다.
	char		bLocalTag;				// 로컬태그로 사용
} AO_FILE_STRUCT;

typedef struct {
//	char		type;		// 0, 1, 2, 3

	AO_FILE_STRUCT file;
	ASSIGN_TAG_STRUCT assign;			// 태그가 간접태그일때만 사용한다.

	short		nTerminal;				// terminal 번호
	WORD		buf_pos;						// struct 에서의 위치.
	double		curr;
	double		old;
	WORD		real_curr;
	WORD  		real_old;
//	WORD		wWriteRetryTimeCurr;		// 지정한 시간이 경과한 후 데이터를 써 준다. 경과한 시간을 말한다.
	char		bDdeLinkFlag;				// DdeTag가 접속되었느냐?
	DWORD		dwDdeService;				// dde pos
	DWORD		dwDdeTopic;					// dde pos
	DWORD		dwDdeItem;					// dde pos

	char		bRightOperation;			// 사용자가 이 태그를 운전할 수 있는 권한이 있는냐?

	HANDLE			hSharedTag;
	SHARED_TAG_AO	*pSharedTag;

//	char		extra[127];					// 2의 n승을 맞추기 위해 존재한다.
	// 현재 512바이트
} ANALOG_OUTPUT_STRUCT;

typedef struct {
	WORD	struct_size;
	char	tag[40];
	char	description[80];
	short	port;						// 컴퓨터 포트
	short	station;					// plc station
	DWORD 	address;
	short 	fn;
	char	desON[7];
	char	desOFF[7];
	char   	act;					// 유효 무효
	char	alarm;
	char	sSubOutDigital1[40];	// 입력태그의 신호가 물려있는 출력태그
	char	sSubOutDigital2[40];
	char	sSubOutDigitalOnTag[40];// 입력이 ON이 되었을때 출력할 디지털 태그
	char	sSubOutDigitalOffTag[40]; // 입력이 OFF가 되었을 때 출력할 디지털 태그
	char	bFileSave;				// 데이터 파일을 저장할것이냐???
	char	cAlarmType;				// 경보를 울리면 어떤 조건에서 울리느냐?
	char	sGraphicFile[40];	 	// 주어진 태그를 가장 잘 확인 할 수 있는 그래픽모듈파일?
	char	sAlarmWaveFile[40];		// 경보 발생 시 사용할 음성 파일
	WORD	wAlarmPriority; 		// 경보 우선권
	char	cTagType; 				// 0=PLC_SCAN, 1=DDE, 2=가상태그.
	char	sDdeService[21];		// dde service
	char	sDdeTopic[40];			// dde Topic
	char	sDdeItem[40];			// dde item
	char	cOutLinkMethod;			// out1, out2로의 출력방법을 정한다.
	char	cConfirmCount;			// confirm을 할 count default=0 confirm을 하지 않는다.
	WORD	wProtectFlags;			// 각종 금지 플래그들 
	char	bReverse;				// I/O 가 반전되어 있다.
	BYTE	bDdeRequest;
	WORD	wScanTime;				// 0 - 일반 스캔, 그 이외의 값은 지정한 스캔 Time에 따라 움직임.
	char	bLocalTag;				// 로컬태그로 사용
} DI_FILE_STRUCT;

typedef struct {
//	char		type;				// 0, 1, 2, 3

	DI_FILE_STRUCT file;
	ASSIGN_TAG_STRUCT assign;		// 태그가 간접태그일때만 사용한다.

	short		nTerminal;				// terminal 번호
	WORD		buf_pos;				// struct 에서의 위치.
	char		curr;					// flag ON/OFF;
	short		count_on_off;			// 1분동안 디지탈이 ON/OFF 된 횟수
	char		startOnSec;				// 디지탈이 ON될 당시의 초
	char		cOnTime;				// 1분동안 총 ON된 시간(초)
	char		bCurrentAlarmStatus;	// 현재 경보상태
	char		bDdeLinkFlag;			// DdeTag가 접속되었느냐?
	DWORD		dwDdeService;			// dde pos
	DWORD		dwDdeTopic;				// dde pos
	DWORD		dwDdeItem;				// dde pos
	struct	date dEvent;				// event 가 발생한 시간
	struct	time tEvent;				// event 가 발생한 시간.
	char		bNeedAlarmConfirm;		// 경보가 발생하면 이 플래그가 살고 사용자가 확인해 주거나 경보가 복귀되면 이 플래그는 죽는다.

	short nSubOutDigital1;		// 입력태그의 신호가 물려있는 출력태그
	short nSubOutDigital2;
	short nSubOutDigitalOnTag;  // 입력이 ON이 되었을때 출력할 디지털 태그
	short nSubOutDigitalOffTag; // 입력이 OFF가 되었을 때 출력할 디지털 태그

	char				cTrendRemainCount;	// 남아있는 trend count
	DI_TREND_REMAIN		remain[10];			// 남아있는 trend 구조체

	char		bRightOperation;			// 사용자가 이 태그를 운전할 수 있는 권한이 있는냐?

	char cTypeSubOutDigital1;		// 입력태그의 신호가 물려있는 출력태그			DO or GDO
	char cTypeSubOutDigital2;		//												DO or GDO
	char cTypeSubOutDigitalOnTag;	// 입력이 ON이 되었을때 출력할 디지털 태그		DO or GDO
	char cTypeSubOutDigitalOffTag;	// 입력이 OFF가 되었을 때 출력할 디지털 태그	DO or GDO
	WORD		wScanTimeCurr;				// 0 - 일반 스캔, 그 이외의 값은 지정한 스캔 Time에 따라 움직임.

	HANDLE			hSharedTag;
	SHARED_TAG_DI	*pSharedTag;

//	char		extra[304];						// 2의 n승을 맞추기 위해 존재한다.
	// 1024 바이트
} DIGITAL_INPUT_STRUCT;

typedef struct {
	WORD		struct_size;
	char		tag[40];
	char		description[80];
	short		port;
	short		station;			// plc station
	DWORD 		address;
	char 		sExtraAddr[40];	// PLC 기종에 따라 틀리다, FUJI(B, M, D...) MELSEC(X, Y, ...)
	WORD		wExtraAddr;		// 어드레스 이외에 통신에 필요할 수 있는 값을 넣을수 있게 했다.
	char		desON[7];
	char		desOFF[7];
	char   		act;

	char 		cRelayType;			// 0 - latch, 1 - pulse,
	WORD		wRelaySecTarget;	// relay type 이 pulse 일때 지연시간.

	char		bReverse;			// 8.10부터추가 반전, 7.0이전에는-wWriteRetryTime으로 사용(지정한 시간이 경과한 후 데이터를 써 준다. 0 - disable , sec)
	char		cTagType; 			// 0=PLC_SCAN, 1=DDE,
	char		sDdeService[21];	// dde service
	char		sDdeTopic[40];		// dde Topic
	char		sDdeItem[40];		// dde item

	char	bLocalTag;				// 로컬태그로 사용
} DO_FILE_STRUCT;

typedef struct {
	DO_FILE_STRUCT file;
	ASSIGN_TAG_STRUCT assign;			// 태그가 간접태그일때만 사용한다.

	short		nTerminal;				// terminal 번호
	WORD		buf_pos;					// struct 에서의 위치.
	char		curr;						// flag ON/OFF;
	char		reserved;					// 이전에는 count_on_off 로 사용했다.

	char		cDelayOutputMethod;			// 지정시간(초)뒤에 출력을 한다. 0 - none, 1 - ON, 2 - OFF.
	short		nDelayOutputSec;			// 지정시간(초)뒤에 출력을 할 시간.

//	WORD		wWriteRetryTimeCurr;		// 지정한 시간이 경과한 후 데이터를 써 준다. 경과한 시간을 말한다.

	char		bDdeLinkFlag;				// DdeTag가 접속되었느냐?
	DWORD		dwDdeService;				// dde pos
	DWORD		dwDdeTopic;					// dde pos
	DWORD		dwDdeItem;					// dde pos

	char		bRightOperation;			// 사용자가 이 태그를 운전할 수 있는 권한이 있는냐?

	HANDLE			hSharedTag;
	SHARED_TAG_DO	*pSharedTag;

//	char		extra[155];					// 2의 n승을 맞추기 위해 존재한다.

	// structure size 512 tag
} DIGITAL_OUTPUT_STRUCT;

typedef struct {
	WORD		struct_size;
	char		tag[40];
	char		description[80];
	char		act;
	char		cTagType;
	short		port;
	short		station;				// plc station
	WORD 		address;
	BYTE		read_size;				//  몇개를 읽을 것이냐 ? 1~255
	BYTE		read_method;			//	어떻게 읽어올 것이냐 ?
	char		cMemoryType;			//  어떤 메모리에서 읽어올 것이냐?

	char		sDdeService[21];		// dde service
	char		sDdeTopic[40];			// dde Topic
	char		sDdeItem[40];			// dde item
	byte 		bDdeRequest;

	char		bLocalTag;				// 로컬태그로 사용

} TAG_ST_FILE_STRUCT;

typedef struct {
	TAG_ST_FILE_STRUCT file;

	short		nTerminal;					// terminal 번호
	WORD		buf_pos;					// struct 에서의 위치.
	char		curr[256];
	char		bRightOperation;			// 사용자가 이 태그를 운전할 수 있는 권한이 있는냐?

	char		bDdeLinkFlag;				// DdeTag가 접속되었느냐?
	DWORD		dwDdeService;				// dde pos
	DWORD		dwDdeTopic;					// dde pos
	DWORD		dwDdeItem;					// dde pos

	//char		extra[3];					// 2의 n승을 맞추기 위해 존재한다.
	HANDLE			hSharedTag;
	SHARED_TAG_ST	*pSharedTag;
} TAG_ST_STRUCT;

typedef ANALOG_INPUT_STRUCT  TAG_AI_STRUCT;
typedef ANALOG_OUTPUT_STRUCT TAG_AO_STRUCT;
typedef DIGITAL_INPUT_STRUCT TAG_DI_STRUCT;
typedef DIGITAL_OUTPUT_STRUCT TAG_DO_STRUCT;

typedef struct {
	WORD struct_size;
	int	nHapAI;
	int	nHapDI;
	int	nHapAO;
	int	nHapDO;
	int	nHapST;
} TAG_MAP_HEADER;

typedef struct {
	int		terminal;		// terminal번호.
	char	name[21];
	char	tag_read_flag;	// tag를 읽었느냐?
	char	bConnect;		// 현재 터미널이 접속중인가?

	ANALOG_INPUT_STRUCT		*analogInput;
	DIGITAL_INPUT_STRUCT	*digitalInput;
	ANALOG_OUTPUT_STRUCT	*analogOutput;
	DIGITAL_OUTPUT_STRUCT	*digitalOutput;
	TAG_ST_STRUCT			*stringTag;

	//HGLOBAL	hGlobalAI;
	//HGLOBAL	hGlobalAO;
	//HGLOBAL	hGlobalDI;
	//HGLOBAL	hGlobalDO;
	//HGLOBAL	hGlobalST;

	int nAnalogInputHap;
	int nAnalogOutputHap;
	int nDigitalInputHap;
	int nDigitalOutputHap;
	int nStringTagHap;

	HANDLE hmmfHeader;
	HANDLE hmmfAI;
	HANDLE hmmfDI;
	HANDLE hmmfAO;
	HANDLE hmmfDO;
	HANDLE hmmfST;

	TAG_MAP_HEADER *tagHeader;

} TERMINAL_STRUCT;

extern TERMINAL_STRUCT *terminalStruct;

void TagSaveAI(int terminal);
void TagSaveAO(int terminal);
void TagSaveDI(int terminal);
void TagSaveDO(int terminal);
void TagSaveST(int terminal);

void PrepareSubTagAI(HWND hwnd, int terminal, int pos);
void PrepareSubTagDI(HWND hwnd, int terminal, int pos);

void TagPrepare(HWND hwnd);
void TagLoadTerminal(HWND hwnd, TERMINAL_STRUCT *ter);
void TagFreeTerminal(TERMINAL_STRUCT *ter);
void TagFree();

int  GetTagPosAI(TERMINAL_STRUCT *ter, const char *tag, short &pos);
int  GetTagPosAO(TERMINAL_STRUCT *ter, const char *tag, short &pos);
int  GetTagPosDI(TERMINAL_STRUCT *ter, const char *tag, short &pos);
int  GetTagPosDO(TERMINAL_STRUCT *ter, const char *tag, short &pos);
int  GetTagPosST(TERMINAL_STRUCT *ter, const char *tag, short &pos);
int  GetTagPosGDO(TERMINAL_STRUCT *ter, const char *tag, short &pos);

int  GetTagTypeAndPos(TERMINAL_STRUCT *ter, const char *tag, int &type, short &pos);
int  GetTagTypeAndPos(int terminal, const char *tag, int &type, short &pos);

int TagCompare(char *str1, char *str2);

double GetDisplayValue(TAG_AI_STRUCT *ai, double value);
void AiValueToString(char *buf, TAG_AI_STRUCT *ai, double value);
void AiValueToString(CString &buf, TAG_AI_STRUCT *ai, double value);
void AiValueToStringOnlyPoint(char *buf, TAG_AI_STRUCT *ai, double value);	// 소수점 이하만 형식에 따른다.
void AiValueToStringOnlyPoint(CString &buf, TAG_AI_STRUCT *ai, double value);	// 소수점 이하만 형식에 따른다.
void AiValueToStringOnlyPoint(char *buf, AI_FILE_STRUCT *ai, double value);	// 소수점 이하만 형식에 따른다.


void ValueToStringByFormat(char *buf, float format, double value);
void GetDesON(TAG_DI_STRUCT *di, char *des);
void GetDesOFF(TAG_DI_STRUCT *di, char *des);
void GetDesON(TAG_DI_STRUCT *di, CString &des);
void GetDesOFF(TAG_DI_STRUCT *di, CString &des);
void GetDigitalStatusString(TAG_DI_STRUCT *di, char *des, char flag);

void GetDoDesON(TAG_DO_STRUCT *dout, char *des);
void GetDoDesOFF(TAG_DO_STRUCT *dout, char *des);
void GetDoStatusString(TAG_DO_STRUCT *dout, char *des, char flag);

void SaveTrendRemainAI(ANALOG_INPUT_STRUCT *ai);
void SaveTrendRemainDI(DIGITAL_INPUT_STRUCT *di);

void SetTagValue(int tag_type, short tag_pos, const char *val_str);
void SetTagValue(int tag_type, short tag_pos, long double val_dbl);
void SetTagValue(const char *tag, const char *val_str, long double val_dbl);
void SetTagValue(const char *tag, const char *value);
void SetTagValue(const char *tag, long double value);

void UpdateSharedValue(TAG_AI_STRUCT *ai);
void UpdateSharedValue(TAG_DI_STRUCT *di);
void UpdateSharedValue(TAG_ST_STRUCT *st);

TAG_AI_STRUCT *GetDirectTag(TAG_AI_STRUCT *ai);
TAG_DI_STRUCT *GetDirectTag(TAG_DI_STRUCT *di);
TAG_AO_STRUCT *GetDirectTag(TAG_AO_STRUCT *ao);
TAG_DO_STRUCT *GetDirectTag(TAG_DO_STRUCT *dout);

#pragma pack(pop)

#endif





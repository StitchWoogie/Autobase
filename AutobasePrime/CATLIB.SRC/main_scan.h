// TAG size O.K
#if	!defined (__MAIN_SCAN_H)
#define __MAIN_SCAN_H
// 감시프로그램과 PLC_SCAN이 공유하는 헤더파일

#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

#pragma pack(push, 1)

typedef struct {
	int			nCountWORD;
	int			nCountFLOAT;
	int			nCountDWORD;
	int			nCountSTRING;
	int			nCountSYSTEM;
	int			nCountDOUBLE;
	int			nCountINT64;
	BYTE        hmmfNumber;
	WORD		crc;
} PLC_SCAN_PORT_INFO;

typedef struct {
	char	flag;
	WORD	value;
} WORD_BUF;

typedef struct {
	char	flag;
	float   value;
} FLOAT_BUF;

typedef struct {
	char	flag;
	DWORD   value;
} DWORD_BUF;

typedef struct {
	WORD   value;
} SYSTEM_BUF;

typedef struct {
	char	flag;
	char	value[256];
} STRING_BUF;

typedef struct {
	char	flag;
	double  value;
} DOUBLE_BUF;

typedef struct {
	char	flag;
	__int64 value;
} INT64_BUF;

typedef struct {
	char	command;				// 0 = DO, 1 = AO, 2 = BYTE array, 100 = Read scan delay.
	char    tag[40];
	short	port;
	short	station;
	DWORD	address;
	char	sExtraAddr[40];
	WORD	wExtraAddr;
	double	value;
	BYTE    arrayValue[512];		// 이때 value = 배열의 size가 된다. 일단 유니코드 256글자를 만들기 위해서 512크기를 확보했다.
	BYTE    array_type;				// 

	char	bVipScanFlag;			// MAIN에서 수동으로 운전했을 때
	WORD    wVipScanMemoryPort;
	char	cVipScanMemoryType;		// 수동으로 운전했을 때 관련 입력의 메모리 종류
	int	    wVipScanMemoryPos;		// 수동으로 운전했을 때 관련 입력의 메모리 위치
} SCAN_WRITE_EXCHANGE_ITEM;

#define MAX_SCAN_WRITE_EXCHANGE_ITEM_COUNT		4000	// 감시 프로그램과 통신프로그램에서 사용하는 최대 갯수. 통신 프로그램에서 설정할 수 있는 최대값으로 설정하는 것이 좋을 듯 하다. MAIN과 PLC_SCAN 한개이므로 크게 무리가 없을 듯 하다.

typedef struct {
	int	struct_size;
	//short	count;
	//char	bEvent;					// Event가 발생했다.
	short	ring_current;
	short	ring_target;
	SCAN_WRITE_EXCHANGE_ITEM item[MAX_SCAN_WRITE_EXCHANGE_ITEM_COUNT];
} SCAN_WRITE_EXCHANGE_INFO;

#pragma pack(pop)

#endif


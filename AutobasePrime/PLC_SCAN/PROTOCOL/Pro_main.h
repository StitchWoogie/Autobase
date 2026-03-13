
#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

#include "..\plc_scan.h"

enum {
	PROTOCOL_NULL,			// attention!!! must be 0
	PROTOCOL_GANTNER,		// Gantner ISK-100 interface
	PROTOCOL_SPC_300,		// samsung brain spc-300
	PROTOCOL_MF2,			// MF2
	PROTOCOL_AOJ2_C214,		// Melsec AOJ2-C214
	PROTOCOL_WR3380,		// National WR3380-82
	PROTOCOL_DPM2,			// S-CON new dpm
	PROTOCOL_GLOFA,			// glofa plc
	PROTOCOL_MASTER_K,		// master k500/1000 series
	PROTOCOL_ABPLC5,		// AB PLC-5 protocol
	PROTOCOL_LA250,			// ASR 의 LA-250CU unit
	PROTOCOL_BL2300,		// Lotte Hotel BL-2300-200A24
	PROTOCOL_PM170E,		// SATEC PM-170E
	PROTOCOL_MJ71E71,		// MJ71E71 ethernet protocol
	PROTOCOL_MODICON,		// Modicon protocol
	PROTOCOL_Reserved,		// DDE client 9.5.3 부터는 없어졌다.
	PROTOCOL_TMTC,			// TMTC protocol
    PROTOCOL_OMRON,			// OMRON plc
	PROTOCOL_PM_B,			// 광성 PM-B Power Meter
	PROTOCOL_PT_L,			// panasonic PT-L???EG series
	PROTOCOL_RLINK,			// RLINK Interface
	PROTOCOL_ADAM,			// ADAM 4000 series 
	PROTOCOL_SP30,
	PROTOCOL_GMPC,			// LG산전 집중계량장치.
	PROTOCOL_PCD,			// SAIA PCD P8 protocol계기
	PROTOCOL_PCD_SBUS,		// SAIA PCD Sbus Protocol
	PROTOCOL_MASTER_K_10,	// master k10/60/200 series
	PROTOCOL_MASTER_K_30,	// master k30/50 series
	PROTOCOL_GESNP,			// GE SNP 프로토콜
	PROTOCOL_DLL,			// DLL로 만들어진 프로토콜
	PROTOCOL_NETWORK_CLIENT_MULTI,		// Network client multi protocol
	PROTOCOL_NETWORK_CLIENT_VIRTUAL,	// Network client multi protocol
};

typedef struct {
	char	filename[80];
	char	title[80];
	char	bProtocol;
	FILETIME	ft;
	DWORD		size;
	short	VersionMajor;
	short	VersionMinor;
} DLL_PROTOCOL_LIST;

extern Block blockProtocolList;

typedef struct {
	char *name;
	int  protocol;
} PROTOCOL_NAME_DEFINE;

#define	MAX_PROTOCOL_NAME_DEFINE	29

extern PROTOCOL_NAME_DEFINE protocolNameDefine[MAX_PROTOCOL_NAME_DEFINE];

//  Gantner ISK-100
void PlcScanDrawMethodTitleGantner(HDC hdc, int x, int y);
void PlcScanDrawMethodGantner(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadGantner(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitGantner(LOCAL_PORT_STRUCT *st, int station, WORD address, WORD flag);
int  PlcScanWriteWordGantner(LOCAL_PORT_STRUCT *st, int station, WORD address, WORD value);

//  SAMSUNG BRAIN SPC-300 Protocol
void PlcScanDrawMethodTitleSpc300(HDC hdc, int x, int y);
void PlcScanDrawMethodSpc300(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadSpc300(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitSpc300(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag);
int  PlcScanWriteWordSpc300(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value);

//  MF2 Protocol
void PlcScanDrawMethodTitleMF2(HDC hdc, int x, int y);
void PlcScanDrawMethodMF2(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadMF2(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitMF2(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag);
int  PlcScanWriteWordMF2(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value);

//  AOJ2-C214 Protocol
void PlcScanDrawMethodTitleAOJ2C214(HDC hdc, int x, int y);
void PlcScanDrawMethodAOJ2C214(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadAOJ2C214(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitAOJ2C214(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordAOJ2C214(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  WR3380 Protocol
void PlcScanDrawMethodTitleWR3380(HDC hdc, int x, int y);
void PlcScanDrawMethodWR3380(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadWR3380(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitWR3380(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag);
int  PlcScanWriteWordWR3380(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value);

//  DPM2 Protocol
void PlcScanDrawMethodTitleDPM2(HDC hdc, int x, int y);
void PlcScanDrawMethodDPM2(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadDPM2(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitDPM2(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag);
int  PlcScanWriteWordDPM2(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *type, WORD channel);

//  GLOFA Protocol
void PlcScanDrawMethodTitleGlofa(HDC hdc, int x, int y);
void PlcScanDrawMethodGlofa(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadGlofa(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitGlofa(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device, WORD pannel);
int  PlcScanWriteWordGlofa(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device, WORD pannel);

//  Master-K500/1000 Protocol
void PlcScanDrawMethodTitleMasterK(HDC hdc, int x, int y);
void PlcScanDrawMethodMasterK(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadMasterK(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitMasterK(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordMasterK(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  AB PLC-5 Protocol
void PlcScanDrawMethodTitleAbplc5(HDC hdc, int x, int y);
void PlcScanDrawMethodAbplc5(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadAbplc5(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitAbplc5(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordAbplc5(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device, WORD file_no);

//  ASR LA-250 Protocol
void PlcScanDrawMethodTitleLA250(HDC hdc, int x, int y);
void PlcScanDrawMethodLA250(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadLA250(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitLA250(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag);
int  PlcScanWriteWordLA250(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value);

//  BL-2300 Protocol
void PlcScanDrawMethodTitleBL2300(HDC hdc, int x, int y);
void PlcScanDrawMethodBL2300(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadBL2300(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitBL2300(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag);
int  PlcScanWriteWordBL2300(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value);

//  SATEC PM-170E Protocol
void PlcScanDrawMethodTitlePM170E(HDC hdc, int x, int y);
void PlcScanDrawMethodPM170E(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadPM170E(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitPM170E(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordPM170E(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *device);

//  Melsec MJ71E71 Protocol
void PlcScanDrawMethodTitleMJ71E71(HDC hdc, int x, int y);
void PlcScanDrawMethodMJ71E71(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadMJ71E71(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitMJ71E71(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordMJ71E71(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  Modicon Protocol
void PlcScanDrawMethodTitleModicon(HDC hdc, int x, int y);
void PlcScanDrawMethodModicon(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadModicon(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitModicon(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordModicon(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *device);

//  DDE Protocol
void PlcScanDrawMethodTitleDDE(HDC hdc, int x, int y);
void PlcScanDrawMethodDDE(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadDDE(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitDDE(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordDDE(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *device);

//  TMTC Protocol
void PlcScanDrawMethodTitleTMTC(HDC hdc, int x, int y);
void PlcScanDrawMethodTMTC(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadTMTC(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitTMTC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordTMTC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  OMRON Protocol
void PlcScanDrawMethodTitleOMRON(HDC hdc, int x, int y);
void PlcScanDrawMethodOMRON(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadOMRON(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitOMRON(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordOMRON(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  PM-B Protocol
void PlcScanDrawMethodTitlePM_B(HDC hdc, int x, int y);
void PlcScanDrawMethodPM_B(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadPM_B(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitPM_B(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordPM_B(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  PT-L Protocol
void PlcScanDrawMethodTitlePT_L(HDC hdc, int x, int y);
void PlcScanDrawMethodPT_L(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadPT_L(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitPT_L(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordPT_L(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  RLINK Protocol
void PlcScanDrawMethodTitleRLINK(HDC hdc, int x, int y);
void PlcScanDrawMethodRLINK(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadRLINK(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitRLINK(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordRLINK(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  ADAM Protocol
void PlcScanDrawMethodTitleADAM(HDC hdc, int x, int y);
void PlcScanDrawMethodADAM(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadADAM(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitADAM(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordADAM(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *device);

//  SP30 Protocol
void PlcScanDrawMethodTitleSP30(HDC hdc, int x, int y);
void PlcScanDrawMethodSP30(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadSP30(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitSP30(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordSP30(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  ABC Protocol
void PlcScanDrawMethodTitleABC(HDC hdc, int x, int y);
void PlcScanDrawMethodABC(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadABC(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitABC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordABC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  GMPC Protocol
void PlcScanDrawMethodTitleGMPC(HDC hdc, int x, int y);
void PlcScanDrawMethodGMPC(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadGMPC(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitGMPC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device, WORD pannel);
int  PlcScanWriteWordGMPC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  PCD Protocol
void PlcScanDrawMethodTitlePCD(HDC hdc, int x, int y);
void PlcScanDrawMethodPCD(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadPCD(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitPCD(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordPCD(LOCAL_PORT_STRUCT *pt, int station, WORD address, double value, char *device);

//  PCD SBUS Protocol
void PlcScanDrawMethodTitlePCD_SBUS(HDC hdc, int x, int y);
void PlcScanDrawMethodPCD_SBUS(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadPCD_SBUS(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitPCD_SBUS(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordPCD_SBUS(LOCAL_PORT_STRUCT *pt, int station, WORD address, double value, char *device);

//  Master-K10/60/200 Protocol
void PlcScanDrawMethodTitleMasterK10(HDC hdc, int x, int y);
void PlcScanDrawMethodMasterK10(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadMasterK10(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitMasterK10(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordMasterK10(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  Master-K30/50 Protocol
void PlcScanDrawMethodTitleMasterK30(HDC hdc, int x, int y);
void PlcScanDrawMethodMasterK30(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadMasterK30(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitMasterK30(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordMasterK30(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  대기 protocol
void PlcScanDrawMethodTitleAir(HDC hdc, int x, int y);
void PlcScanDrawMethodAir(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadAir(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitAir(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordAir(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  GESNP protocol
void PlcScanDrawMethodTitleGESNP(HDC hdc, int x, int y);
void PlcScanDrawMethodGESNP(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadGESNP(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitGESNP(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device);
int  PlcScanWriteWordGESNP(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device);

//  Network Client Multi
void PlcScanDrawMethodTitleNetClientMulti(HDC hdc, int x, int y);
void PlcScanDrawMethodNetClientMulti(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
int  PlcScanReadNetClientMulti(LOCAL_PORT_STRUCT *st, int pos);
int  PlcScanWriteBitNetClientMulti(LOCAL_PORT_STRUCT *pt, int station, DWORD address, WORD flag, char *device, WORD pannel, int write_port);
int  PlcScanWriteWordNetClientMulti(LOCAL_PORT_STRUCT *pt, int station, DWORD address, long double value, char *device, WORD pannel, int write_port);










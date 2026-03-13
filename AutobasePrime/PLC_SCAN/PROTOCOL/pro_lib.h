#if	!defined (__PRO_LIB_H)
#define __PRO_LIB_H

void WaitSec(int sec);
void PlcScanSetErrorString(const char *string, ...);

void PokeWORD(LOCAL_PORT_STRUCT *pt, WORD address,  WORD value);
void PokeDWORD(LOCAL_PORT_STRUCT *pt, WORD address, DWORD value);
void PokeFLOAT(LOCAL_PORT_STRUCT *pt, WORD address, FLOAT value);
void PokeSTRING(LOCAL_PORT_STRUCT *pt, WORD address, const char *value);
void PokeINT64(LOCAL_PORT_STRUCT *pt, WORD address, __int64 value);
void PokeDOUBLE(LOCAL_PORT_STRUCT *pt, WORD address, double value);
void PokeValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, WORD address, double value);


/*
WORD PeekValueWORD(LOCAL_PORT_STRUCT *pt, WORD address);
DWORD PeekValueDWORD(LOCAL_PORT_STRUCT *pt, WORD address);
double PeekValueFLOAT(LOCAL_PORT_STRUCT *pt, WORD address);
int PeekValueSTRING(LOCAL_PORT_STRUCT *pt, WORD address, char *val);
__int64 PeekValueINT64(LOCAL_PORT_STRUCT *pt, WORD address);
double PeekValueDOUBLE(LOCAL_PORT_STRUCT *pt, WORD address);*/

// Peek는 통신 드라이버로 Proc을 전송하지 않기 때문에 WORD address를 int로 바로 변경했다.2010-12-15
WORD PeekValueWORD(LOCAL_PORT_STRUCT *pt, int address);
DWORD PeekValueDWORD(LOCAL_PORT_STRUCT *pt, int address);
double PeekValueFLOAT(LOCAL_PORT_STRUCT *pt, int address);
int PeekValueSTRING(LOCAL_PORT_STRUCT *pt, int address, char *val);
__int64 PeekValueINT64(LOCAL_PORT_STRUCT *pt, int address);
double PeekValueDOUBLE(LOCAL_PORT_STRUCT *pt, int address);

// PokeNew???? 는 65536을 넘는 주소를 위하여 만들어 졌다. 2010-12-15
void PokeNewWORD(LOCAL_PORT_STRUCT *pt, int address,  WORD value);
void PokeNewDWORD(LOCAL_PORT_STRUCT *pt, int address, DWORD value);
void PokeNewFLOAT(LOCAL_PORT_STRUCT *pt, int address, FLOAT value);
void PokeNewSTRING(LOCAL_PORT_STRUCT *pt, int address, const char *value);
void PokeNewINT64(LOCAL_PORT_STRUCT *pt, int address, __int64 value);
void PokeNewDOUBLE(LOCAL_PORT_STRUCT *pt, int address, double value);
void PokeNewValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, int address, double value);

/*
WORD PeekNewWORD(LOCAL_PORT_STRUCT *pt, int address);
DWORD PeekNewDWORD(LOCAL_PORT_STRUCT *pt, int address);
double PeekNewFLOAT(LOCAL_PORT_STRUCT *pt, int address);
int PeekNewSTRING(LOCAL_PORT_STRUCT *pt, int address, char *val);
__int64 PeekNewINT64(LOCAL_PORT_STRUCT *pt, int address);
double PeekNewDOUBLE(LOCAL_PORT_STRUCT *pt, int address);*/

class LocalProtocolClass {
		HGLOBAL hGlobal;
		char    *local;
	public:
		LocalProtocolClass(HGLOBAL hglobal);
		~LocalProtocolClass();
		char *GetPoint() { return local; }
};


#endif





#include <compiler.hpp>
#include "..\catlib.src\ShareProtect.h"

#pragma pack(push, 1)

// DlgTagBx.dll
__declspec( dllexport ) int DllTagSelectTagAll(char *tag, char *des);
__declspec( dllexport ) int DllTagSelectTagAi(char *tag, char *des);
__declspec( dllexport ) int DllTagSelectTagDi(char *tag, char *des);
__declspec( dllexport ) int DllTagSelectTagAiDi(char *tag, char *des);

// DlgTagGr.dll
__declspec( dllexport ) int DllGroupForm();
__declspec( dllexport ) int DllGroupTag();

extern "C" void DLLEXPORT WatchDogInfoInit();
extern "C" void DLLEXPORT WatchDogInfoUnInit();
extern "C" void DLLEXPORT WatchDogInfoTimerReset(int id);

extern "C" BOOL DLLEXPORT SharedTagGetCurr(char *tag, char *curr);
extern "C" BOOL DLLEXPORT SharedTagGetAiCurr(char *tag, double *curr);
extern "C" BOOL DLLEXPORT SharedTagGetDiCurr(char *tag, char *curr);
extern "C" BOOL DLLEXPORT SharedTagGetStCurr(char *tag, char *curr);

enum {
		WDI_AutoBase = 0,
		WDI_PlcScan = 1,
		WDI_Reporter = 2,
		WDI_NetServ = 3,
		WDI_NetClnt = 4,
		WDI_RunMain = 5,
		WDI_Beeper = 6,
		WDI_LinePrinter = 7,
		WDI_SMS = 8,
};

//extern "C" BOOL DLLEXPORT ShareProtectHaveRightsByStruct(SHARE_PROTECT_STRUCT *share, int id);
//extern "C" BOOL DLLEXPORT ShareProtectHaveRights(int id);
//extern "C" BOOL DLLEXPORT ShareProtectHaveAllRights();

#pragma pack(pop)

#include "stdafx.h"

#include <tools.h>

#include "Win32Common.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

#define	MAX_WATCH_DOG	50

typedef struct {
	int struct_size;
	int timer[MAX_WATCH_DOG];
} WATCH_DOG_INFO;

static SharedMemory systemStatus;

extern "C" void DLLEXPORT WatchDogInfoInit()
{
	char first_flag;
	systemStatus.Init("AutoBaseWatchDogInfo", sizeof(WATCH_DOG_INFO), first_flag);

	if(first_flag && systemStatus.ptr != NULL) {	// 처음으로 할당 되었을 때
		memset(systemStatus.ptr, 0, sizeof(WATCH_DOG_INFO));
	}
}

extern "C" void DLLEXPORT WatchDogInfoUnInit()
{
	systemStatus.Uninit();
}

extern "C" void DLLEXPORT WatchDogInfoTimerReset(int id)
{
	if(systemStatus.ptr == NULL)	return;

	if(id < 0 || id >= MAX_WATCH_DOG)	return;	// range over

	WATCH_DOG_INFO *status = (WATCH_DOG_INFO*)systemStatus.ptr;
	
	status->timer[id] = 0;
}

extern "C" int DLLEXPORT WatchDogInfoTimerGet(int id)
{
	if(systemStatus.ptr == NULL)	return 2;

	if(id < 0 || id >= MAX_WATCH_DOG)	return 2;	// range over

	WATCH_DOG_INFO *status = (WATCH_DOG_INFO*)systemStatus.ptr;	
	
	return status->timer[id];
}

extern "C" void DLLEXPORT WatchDogInfoTimerSet(int id, int val)
{
	if(systemStatus.ptr == NULL)	return;

	if(id < 0 || id >= MAX_WATCH_DOG)	return;	// range over

	WATCH_DOG_INFO *status = (WATCH_DOG_INFO*)systemStatus.ptr;	
	
	status->timer[id] = val;
}

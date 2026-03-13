#include "stdafx.h"

#include <tools.h>

#include "Win32Common.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

typedef struct {
	int block_count;
	int block_size;
	int ring_current;
	int ring_target;
	unsigned char ptr[2];
} SHARED_MEMORY;

extern "C" HANDLE DLLEXPORT RingCreate(const char *name, int ring_count, int ring_size)
{
	int hap = 4+4+4+4+ring_count*ring_size;

	HANDLE hmmf = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, hap, name);

	if(hmmf == NULL)	return NULL;

	SHARED_MEMORY *ptr = (SHARED_MEMORY*)MapViewOfFile(hmmf, FILE_MAP_WRITE, 0, 0, 0);

	if (ptr == NULL)	return NULL;

	ptr->block_count = ring_count;
	ptr->block_size = ring_size;
	ptr->ring_current = 0;
	ptr->ring_target = 0;
	UnmapViewOfFile(ptr);

	return hmmf;
}

static int AddItem(HANDLE handle, unsigned char *buf, int size)
{
	SHARED_MEMORY *ptr = (SHARED_MEMORY*)MapViewOfFile(handle, FILE_MAP_WRITE, 0, 0, 0);

	if(ptr == NULL) {
		return -3;
	}
	
	int next_pos;
	TimeOutClass timeout;
	while(1) {
		next_pos = (ptr->ring_target+1)%ptr->block_count;
		if(next_pos != ptr->ring_current)	break;
		Sleep(1);
		if(timeout.IsTimeOut(5)) {
			UnmapViewOfFile(ptr);
			return -1;
		}
	}

	if(size > ptr->block_size) {
		UnmapViewOfFile(ptr);
		return -2;
	}
	memcpy(&ptr->ptr[next_pos*ptr->block_size], buf, size);
	ptr->ring_target = next_pos;
	UnmapViewOfFile(ptr);

	return 1;
}

//-----------------------------------------------------------------------------------------
// return value
// 1 = OK
// 0 = Server not found 
// -1 = Timeouted
// -2 = block_size too small
//-----------------------------------------------------------------------------------------

extern "C" int DLLEXPORT RingAddItem(HANDLE handle, unsigned char *buf, int size)
{
	return AddItem(handle, buf, size);
}

/*
//-----------------------------------------------------------------------------------------
// return value
// 1 = OK
// 0 = Server not found 
// -1 = Timeouted
// -2 = block_size too small
//-----------------------------------------------------------------------------------------
extern "C" int DLLEXPORT RingOpenAndAddItem(const char *name, unsigned char *buf, int size)
{
	HANDLE hmmf = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, name);
	if(hmmf == NULL)	return FALSE;
	BOOL retn = AddItem(hmmf, buf, size);
	CloseHandle(hmmf);
	return retn;
}
*/

static BOOL GetItem(HANDLE hmmf, unsigned char *buf, int size)
{
	SHARED_MEMORY *ptr = (SHARED_MEMORY*)MapViewOfFile(hmmf, FILE_MAP_WRITE, 0, 0, 0);

	if (ptr == NULL)	return FALSE;
	
	if(ptr->ring_current == ptr->ring_target) {
		UnmapViewOfFile(ptr);
		return FALSE;
	}
	int next_pos = (ptr->ring_current+1)%ptr->block_count;
	if(size > ptr->block_size) size = ptr->block_size;
	memcpy(buf, &ptr->ptr[next_pos*ptr->block_size], size);
	ptr->ring_current = next_pos;
	UnmapViewOfFile(ptr);
	
	return TRUE;
}

extern "C" BOOL DLLEXPORT RingGetItem(HANDLE hmmf, unsigned char *buf, int size)
{
	return GetItem(hmmf, buf, size);
}

/*
BOOL RingOpenAndGetItem(const char *name, unsigned char *buf, int size)
{
	HANDLE hmmf = OpenFileMapping(FILE_MAP_WRITE, FALSE, name);
	if(hmmf == NULL)	return FALSE;
	BOOL retn = GetItem(hmmf, buf, size);
	CloseHandle(hmmf);
	return TRUE;
}*/

extern "C" void DLLEXPORT RingClose(HANDLE hmmf)
{
	CloseHandle(hmmf);
}
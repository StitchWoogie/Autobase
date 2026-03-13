// TAG size O.K
// NetServ.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include <tools.h>

SharedMemory::SharedMemory()
{
	hHandleFile = NULL;
	ptr = NULL;
}

SharedMemory::~SharedMemory()
{
	Uninit();
}

void SharedMemory::Init(char *name, DWORD size, char &first_flag)
{
	first_flag = OFF;
	
	hHandleFile = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, size, name);

	if(hHandleFile == NULL)	return;

	if(GetLastError() == ERROR_ALREADY_EXISTS) {
		first_flag = OFF;
	}
	else {
		first_flag = ON;
	}

	ptr = MapViewOfFile(hHandleFile, FILE_MAP_WRITE, 0, 0, 0);
}

void SharedMemory::InitByExist(char *name, DWORD size)
{
	// first_flag = OFF;
	ptr = NULL;
	
	hHandleFile = OpenFileMapping(FILE_MAP_WRITE, FALSE, name);

	if(hHandleFile == NULL)	return;

	//if(GetLastError() == ERROR_ALREADY_EXISTS) {
	//	first_flag = OFF;
	//}
	//else {
	//	first_flag = ON;
	//}

	ptr = MapViewOfFile(hHandleFile, FILE_MAP_WRITE, 0, 0, 0);
}

void SharedMemory::Init(char *name, DWORD size)
{
	char first_flag;

	Init(name, size, first_flag);
}

void SharedMemory::Uninit()
{
	if(ptr != NULL) {
		UnmapViewOfFile(ptr);
		ptr = NULL;
	}
	if(hHandleFile) {
		CloseHandle(hHandleFile);
		hHandleFile = NULL;
	}
}


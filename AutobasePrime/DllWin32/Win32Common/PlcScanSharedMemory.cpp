#include "stdafx.h"

#include "PlcScanSharedMemory.h"

PlcScanSharedMemory::PlcScanSharedMemory() 
{
	hHandleFile = NULL;
	sharedMemory = NULL;
}

PlcScanSharedMemory::~PlcScanSharedMemory() 
{
	Close();
}

void PlcScanSharedMemory::Open(char *name)
{
	hHandleFile = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(SHARED_STRUCT), name);

	if(hHandleFile == NULL)	return;

	bool exist_flag = false;

	if(GetLastError() == ERROR_ALREADY_EXISTS) 
		exist_flag = true;

	sharedMemory = (SHARED_STRUCT*)MapViewOfFile(hHandleFile, FILE_MAP_WRITE, 0, 0, 0);

	if(sharedMemory != NULL) {
		ZeroMemory(sharedMemory, sizeof(SHARED_STRUCT));
	}
}

void PlcScanSharedMemory::Close() 
{
	if(sharedMemory != NULL) {
		UnmapViewOfFile(sharedMemory);
		sharedMemory = NULL;
	}
	if(hHandleFile) {
		CloseHandle(hHandleFile);
		hHandleFile = NULL;
	}
}

void PlcScanSharedMemory::WriteByte(unsigned char b)
{
	if(sharedMemory == NULL)	return;

	sharedMemory->send.buf[sharedMemory->send.target] = b;
	sharedMemory->send.target++;
	sharedMemory->send.target %= MAX_BUF;
}

void PlcScanSharedMemory::WriteBytes(unsigned char *buf, int size)
{
	if(sharedMemory == NULL)	return;

	for(int i = 0; i < size; i++) {
		sharedMemory->send.buf[sharedMemory->send.target] = buf[i];
		sharedMemory->send.target++;
		sharedMemory->send.target %= MAX_BUF;
	}
}

int PlcScanSharedMemory::ReadBytes(unsigned char *buf, int size)
{
	if(sharedMemory == NULL)	return 0;

	int readed_count = 0;

	for(int i = 0; i < size; i++) {
		if(sharedMemory->recv.target == sharedMemory->recv.current)	break;
		buf[readed_count] = sharedMemory->recv.buf[sharedMemory->recv.current];
		sharedMemory->recv.current++;
		sharedMemory->recv.current %= MAX_BUF;
		readed_count++;
	}

	return readed_count;
}

int PlcScanSharedMemory::ReadByte()
{
	if(sharedMemory == NULL)	return -1;

	if(sharedMemory->recv.target == sharedMemory->recv.current)	return -1;

	int ch;

	ch = sharedMemory->recv.buf[sharedMemory->recv.current];
	sharedMemory->recv.current++;
	sharedMemory->recv.current %= MAX_BUF;

	return ch;
}

void PlcScanSharedMemory::ClearRecvBuf()
{
	if(sharedMemory == NULL)	return;

	sharedMemory->recv.current = sharedMemory->recv.target;
}


 

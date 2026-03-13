#include "stdafx.h"

#include <tools.h>

#include "PlcScanSharedMemory.h"

#include "Win32Common.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

extern "C" DWORD DLLEXPORT PlcScanDeviceSharedMemoryInit(char *name)
{
	PlcScanSharedMemory *share = new PlcScanSharedMemory();
	share->Open(name);

	return (DWORD)share;
}

extern "C" void DLLEXPORT PlcScanDeviceSharedMemoryUnInit(DWORD id)
{
	PlcScanSharedMemory *share = (PlcScanSharedMemory*)id;	
	if(share == NULL)	return;
	share->Close();
	delete share;
}

extern "C" int DLLEXPORT PlcScanDeviceSharedMemoryRead(DWORD id)
{
	PlcScanSharedMemory *share = (PlcScanSharedMemory*)id;
	if(share == NULL)	return -1;	

	return share->ReadByte();
}

extern "C" void DLLEXPORT PlcScanDeviceSharedMemoryWrite(DWORD id, BYTE ch)
{
	PlcScanSharedMemory *share = (PlcScanSharedMemory*)id;
	if(share == NULL)	return;	

	share->WriteByte(ch);		
}

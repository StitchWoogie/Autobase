// TAG size O.K
#include "stdafx.h"

#include <tools.h>
#include <glib.h>

#include "..\catlib.src\SystemStatusMemory.h"

//SYSTEM_STATUS_MEMORY *systemStatusMemory;
static SharedMemory systemStatus;

extern char sDirWorkProject[MAXPATH];

// 이 함수를 부를 때는 작업 디렉토리를 먼저 안 다음에 불러준다.

void SystemStatusMemoryInit()
{
	char first_flag;
	systemStatus.Init("AutoBaseSystemStatusMemory", sizeof(SYSTEM_STATUS_MEMORY), first_flag);

	if(first_flag && systemStatus.ptr != NULL) {	// 처음으로 할당 되었을 때
		memset(systemStatus.ptr, 0, sizeof(SYSTEM_STATUS_MEMORY));

		char filename[MAXPATH];
		sprintf(filename, "%s\\network\\NetServ.ini", sDirWorkProject);
		int node_type = GetPrivateProfileInt("Node", "Type", 0, filename);
		if(node_type == 0) {	// Stand-Alone Server
			SystemStatusSetDI(SSMDI_DuplexActiveI, ON);
			SystemStatusSetDI(SSMDI_DuplexControlItemLinePrinter, ON);
			SystemStatusSetDI(SSMDI_DuplexControlItemReportPrinter, ON);
		} 
	}
}

void SystemStatusMemoryUninit()
{
	systemStatus.Uninit();
}

int  SystemStatusGetDI(WORD id)
{
	if(systemStatus.ptr == NULL)	return 0;

	SYSTEM_STATUS_MEMORY *status = (SYSTEM_STATUS_MEMORY*)systemStatus.ptr;	
	
	int word_pos = id/16;
	int bit_pos  = id%16;

	if(word_pos >= MAX_SYSTEM_STATUS_MEMORY_DI)						return 0;

	if(status->digital[word_pos] & WORD_MASK[bit_pos])	return 1;
	else															return 0;
}

void SystemStatusSetDI(WORD id, char flag)
{
	if(systemStatus.ptr == NULL)	return;

	SYSTEM_STATUS_MEMORY *status = (SYSTEM_STATUS_MEMORY*)systemStatus.ptr;	
	
	int word_pos = id/16;
	int bit_pos  = id%16;

	if(word_pos >= MAX_SYSTEM_STATUS_MEMORY_DI)	return;

	if(flag)	status->digital[word_pos] |= WORD_MASK[bit_pos];
	else		status->digital[word_pos] &= 0xFFFF-WORD_MASK[bit_pos];
}

//------------------------------------------------------------------------------
//	각 프로토콜 드라이브들이 같이 쓸 수 있는 함수들을 모아 놓았다.
//------------------------------------------------------------------------------

#include "stdafx.h"

#include "dll_lib.h"
#include "..\..\..\catlib\totalcfg.h"

void GetProtocolIniFile(int port, char *filename)
{
	char work_dir[MAXPATH];
	AutoBaseIniGetProjectDirectory(work_dir);
	sprintf(filename, "%s\\scan\\Port%03d.ini", work_dir, port);
}

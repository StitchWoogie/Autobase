#include "stdafx.h"
#include "afxmt.h"

#include "..\catlib.src\totalcfg.h"
#include "..\catlib.src\ShareProtect.h"

bool ShareProtectHaveRights(int id)
{
	char buf[80];
	
	HANDLE handle = OpenMutex(MUTEX_ALL_ACCESS, FALSE, "AutoBaseLocalMainMutex");
	if(handle == NULL)	return true;	// LocalMain이 실행중이지 않다.
	CloseHandle(handle);

	CString item;
	item.Format("%d", RIGHT_IS_ADMIN);

	LoadRegAutoBaseConfig("Protect", "CurrentRights", item, "true", buf, sizeof(buf));

	if(stricmp(buf, "true") == 0)	return true;	// admin 로그인

	item.Format("%d", id);

	LoadRegAutoBaseConfig("Protect", "CurrentRights", item, "false", buf, sizeof(buf));
	if(stricmp(buf, "true") == 0)	return true;

	return false;		
}

// TAG size O.K
#include "stdafx.h"
//#include <stdio.h>
//#include <string.h>
#include <io.h>
#include <atlbase.h>

#include <compiler.hpp>
#include <tools.h>
#include <glib.h>
#include <dataswap.h>

#include "..\catlib.src\totalcfg.h"
#include "..\catlib.src\cversion.h"

extern char sDirProgramm[MAXPATH];

void ChangeUICulture()
{
	char buf[80];

	CString filename;
	filename.Format("%s\\Config\\Program.ini", sDirProgramm);
	GetPrivateProfileString("Language", "Lang", "Auto", buf, sizeof(buf), filename);

	if(stricmp(buf, "Korean") == 0) {
		WORD lgid = MAKELANGID(LANG_KOREAN, SUBLANG_DEFAULT);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
	}
	else if(stricmp(buf, "Chinese") == 0) {
		WORD lgid = MAKELANGID(LANG_CHINESE, SUBLANG_CHINESE_SIMPLIFIED);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
	}
	else if(stricmp(buf, "Japanese") == 0) {
		WORD lgid = MAKELANGID(LANG_JAPANESE, SUBLANG_DEFAULT);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
	}
	else if(stricmp(buf, "Auto") == 0) {
		
	}
	else {
		WORD lgid = MAKELANGID(LANG_ENGLISH, SUBLANG_DEFAULT);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
		SetThreadUILanguage((LANGID)lcid);	// Vista이상에서는 이것을 하지 않으면 리소스 부분이 적용이 안된다. 특히 중국어
	}
}

// PLCSCAN에서만 사용하는 부분
void ChangeUICultureByPlcScan()
{
	char buf[80];

	CString filename;
	filename.Format("%s\\Config\\Program.ini", sDirProgramm);
	GetPrivateProfileString("Language", "Lang", "Auto", buf, sizeof(buf), filename);

	if(stricmp(buf, "Korean") == 0) {
		WORD lgid = MAKELANGID(LANG_KOREAN, SUBLANG_DEFAULT);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
	}
	else if(stricmp(buf, "Chinese") == 0) {
		WORD lgid = MAKELANGID(LANG_CHINESE, SUBLANG_CHINESE_SIMPLIFIED);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
	}
	else if(stricmp(buf, "Japanese") == 0) {
		WORD lgid = MAKELANGID(LANG_JAPANESE, SUBLANG_DEFAULT);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
	}
	/* 일단 유니코드가 아니라서 언어가 다른 OS에서 베트남 모드로 실행하면 글을 알기가 어렵다. 일단 보류하는 것이 좋겠다.
	else if(stricmp(buf, "Vietnamese") == 0) {
		WORD lgid = MAKELANGID(LANG_VIETNAMESE, SUBLANG_DEFAULT);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
		SetThreadUILanguage((LANGID)lcid);	// 7에서는 이것을 하지 않으면 리소스 부분이 적용이 안된다. 2011-1-28
	}*/
	else if(stricmp(buf, "Auto") == 0) {
		
	}
	else {
		WORD lgid = MAKELANGID(LANG_ENGLISH, SUBLANG_DEFAULT);
		LCID lcid = MAKELCID(lgid, SORT_DEFAULT);

		SetThreadLocale(lcid);
		SetThreadUILanguage((LANGID)lcid);	// Vista이상에서는 이것을 하지 않으면 리소스 부분이 적용이 안된다. 특히 중국어
	}
}




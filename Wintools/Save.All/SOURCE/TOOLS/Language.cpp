#include "stdafx.h"
#include <tools.h>

int IsLangKorean()
{
	static WORD plgid;
	static char flag;

	if(flag == 0) {
		//LCID lcid = GetSystemDefaultLCID();	오토베이스 9.0 이전에는 이것을 사용했다.
		LCID lcid = GetThreadLocale();	// 오토베이스 9.0.0 버전부터는 이것을 사용
		LANGID lgid = LANGIDFROMLCID(lcid);
		plgid = PRIMARYLANGID(lgid);
		flag = 1;
	}

	if(plgid == LANG_KOREAN) {	// korean
		return 1;
	}

	return 0;
}

int IsLangJapanese()
{
	static WORD plgid;
	static char flag;

	if(flag == 0) {
		//LCID lcid = GetSystemDefaultLCID();	오토베이스 9.0 이전에는 이것을 사용했다.
		LCID lcid = GetThreadLocale();	// 오토베이스 9.0.0 버전부터는 이것을 사용
		LANGID lgid = LANGIDFROMLCID(lcid);
		plgid = PRIMARYLANGID(lgid);
		flag = 1;
	}

	if(plgid == LANG_JAPANESE) {	// japanese
		return 1;
	}

	return 0;
}

// 9.0.15부터 추가
int IsLangChinese()
{
	static WORD plgid;
	static char flag;

	if(flag == 0) {
		LCID lcid = GetThreadLocale();	// 오토베이스 9.0.0 버전부터는 이것을 사용
		LANGID lgid = LANGIDFROMLCID(lcid);
		plgid = PRIMARYLANGID(lgid);
		flag = 1;
	}

	if(plgid == LANG_CHINESE) {	// chinese
		return 1;
	}

	return 0;
}

// 10.1.2부터 추가
int IsLangVietnamese()
{
	static WORD plgid;
	static char flag;

	if(flag == 0) {
		LCID lcid = GetThreadLocale();	// 오토베이스 9.0.0 버전부터는 이것을 사용
		LANGID lgid = LANGIDFROMLCID(lcid);
		plgid = PRIMARYLANGID(lgid);
		flag = 1;
	}

	if(plgid == LANG_VIETNAMESE) {	// chinese
		return 1;
	}

	return 0;
}



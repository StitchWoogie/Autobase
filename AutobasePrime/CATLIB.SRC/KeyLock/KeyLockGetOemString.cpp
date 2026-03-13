// TAG size O.K
#include "stdafx.h"

void KeyLockGetOemString(BYTE value, char *s)
{
#if	defined (COMPILE_HANGUL)
	if(value == 0x20)		strcpy(s, "");
	else if(value == 0x00)	strcpy(s, "");
	else if(value == 0x01)	strcpy(s, "학교 교육용");
	else if(value == 0x02)	strcpy(s, "엠알 엔지니어링");
	else if(value == 0x03)	strcpy(s, "중앙제어(주)");
	else if(value == 0x04)	strcpy(s, "(주)코바이오텍");
	else if(value == 0x05)	strcpy(s, "(주)원플러스");
	else if(value == 0x06)	strcpy(s, "보정시엔아이(주)");
	else if(value == 0x07)	strcpy(s, "동영정보통신");
	else if(value == 0x08)	strcpy(s, "광양제철소 화재감시 시스템");
	else if(value == 0x09)	strcpy(s, "명성하이콘 주차시스템");
	else if(value == 0x0A)	strcpy(s, "창원 생활 폐기물 소각장");
	else if(value == 0x0B)	strcpy(s, "마산대학");
	else if(value == 0x0C)	strcpy(s, "(주)대청시스템즈");
	else if(value == 0x0D)	strcpy(s, "(주)모던테크");
	else if(value == 0x0E)	strcpy(s, "충인전장");
	//else if(value == 0x0F)	strcpy(s, "대하제어");
    else if (value == 0x10) strcpy(s, "교육 기관용(캐디언스)");
	
	else					strcpy(s, "Bundle Version");
#else
	if(value == 0x20)		strcpy(s, "");
	else if(value == 0x00)	strcpy(s, "");
	else if(value == 0x01)	strcpy(s, "Academy Version");
	else if(value == 0x02)	strcpy(s, "MR Engineering");
	else if(value == 0x03)	strcpy(s, "JA Control");
	else if(value == 0x04)	strcpy(s, "KOBIO Tech");
	else if(value == 0x05)	strcpy(s, "WONPLUS co.,ltd.");
	else if(value == 0x06)	strcpy(s, "Bo jung C&&I Engineering co.,ltd.");
	else if(value == 0x07)	strcpy(s, "동영정보통신");
	else if(value == 0x08)	strcpy(s, "광양제철소 화재감시 시스템");
	else if(value == 0x09)	strcpy(s, "명성하이콘 주차시스템");
	else if(value == 0x0A)	strcpy(s, "창원 생활 폐기물 소각장");
	else if(value == 0x0B)	strcpy(s, "마산대학");
	else if(value == 0x0C)	strcpy(s, "(주)대청시스템즈");
	else if(value == 0x0D)	strcpy(s, "Modern Tech co.,ltd.");
	else if(value == 0x0E)	strcpy(s, "충인전장");
	//else if(value == 0x0F)	strcpy(s, "대하제어");
	else if (value == 0x10) strcpy(s, "교육 기관용(캐디언스)");
	else					strcpy(s, "Bundle Version");
#endif
}


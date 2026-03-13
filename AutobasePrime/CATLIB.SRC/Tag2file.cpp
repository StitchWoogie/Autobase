// TAG SIZE O.K
// english O.K
#include "stdafx.h"

#include <io.h>

#include <tools.h>

// 실제 98,2000,XP에서는 \ / : * ? " < > | 의 9개 문자만 사용하지 않으면 파일명으로 사용된다.

/*
//------------------------------------------------------------------------------
// 태그를 저장할 수 있는 파일 명으로 바꿔준다.
//------------------------------------------------------------------------------

void ConvertTagToFile(char *buf, char *tag)
{
	char hangul_flag = OFF;
	char ext_flag = OFF;	// 확장자를 구분 했느냐?
	int  i, pos;

	memset(buf, '_', 12);
	buf[8] = '.';

	for(i = 0, pos = 0; i < (int)strlen(tag); i++, pos++) {
		if(ext_flag == OFF) {
			if(i >= 7 && hangul_flag == OFF) {
				pos = 9;
				ext_flag = ON;
			}
		}

		if((BYTE)tag[i] <= 32)			buf[pos] = '_';
		else if((BYTE)tag[i] == '/')	buf[pos] = '_';
		else if((BYTE)tag[i] == '.')	buf[pos] = '_';
		else if((BYTE)tag[i] == '\\')	buf[pos] = '_';
		else if((BYTE)tag[i] == '*')	buf[pos] = '_';
		else if((BYTE)tag[i] == '+')	buf[pos] = '_';
		else if((BYTE)tag[i] == '|')	buf[pos] = '_';
		else if((BYTE)tag[i] == ':')	buf[pos] = '_';
		else if((BYTE)tag[i] == ';')	buf[pos] = '_';
		else if((BYTE)tag[i] == '"')	buf[pos] = '_';
		else if((BYTE)tag[i] == '<')	buf[pos] = '_';
		else if((BYTE)tag[i] == '>')	buf[pos] = '_';
      else if((BYTE)tag[i] == '?')	buf[pos] = '_';
		else									buf[pos] = tag[i];

		if(hangul_flag) {
			hangul_flag = OFF;
		}
		else {
			if((BYTE)tag[i] > 127) {
				hangul_flag = ON;
			}
		}
	}

	buf[12] = 0;
}
*/

//--------------------------------------------------------------------------------------------
//	TAG 이름은 10자로 구성되어야 하는데 10자가 되지 않을 때는 뒤에 space로 채워준다.
//--------------------------------------------------------------------------------------------

static void FillSpaceToTag(char *tag)
{
	int i, j;

	for(i = 0; i < 10; i++) {
		if(tag[i] == NULL) {
			for(j = i; j < 10; j++) {
				tag[j] = 0x20;
			}
			tag[10] = 0;
			return;
		}
	}
}

//------------------------------------------------------------------------------
// 태그를 저장할 수 있는 파일 명으로 바꿔준다.
// 실제 98,2000,XP에서는 \ / : * ? " < > | 의 9개 문자만 사용하지 않으면 파일명으로 사용된다.
//------------------------------------------------------------------------------

static void ConvertTagToFileOld(char *buf, const char *tag_org)
{
	char hangul_flag = OFF;
	char ext_flag = OFF;	// 확장자를 구분 했느냐?
	int  i, pos;

	char tag[80];

	strcpy(tag, tag_org);
	FillSpaceToTag(tag);

	memset(buf, '_', 12);
	buf[8] = '.';

	for(i = 0, pos = 0; i < (int)strlen(tag); i++, pos++) {
		if(ext_flag == OFF) {
			if(i >= 7 && hangul_flag == OFF) {
				pos = 9;
				ext_flag = ON;
			}
		}

		if((BYTE)tag[i] <= 32)			buf[pos] = '_';
		else if((BYTE)tag[i] == '/')	buf[pos] = '_';
		else if((BYTE)tag[i] == '.')	buf[pos] = '_';
		else if((BYTE)tag[i] == '\\')	buf[pos] = '_';
		else if((BYTE)tag[i] == '*')	buf[pos] = '_';
		else if((BYTE)tag[i] == '+')	buf[pos] = '_';
		else if((BYTE)tag[i] == '|')	buf[pos] = '_';
		else if((BYTE)tag[i] == ':')	buf[pos] = '_';
		else if((BYTE)tag[i] == ';')	buf[pos] = '_';
		else if((BYTE)tag[i] == '"')	buf[pos] = '_';
		else if((BYTE)tag[i] == '<')	buf[pos] = '_';
		else if((BYTE)tag[i] == '>')	buf[pos] = '_';
		else if((BYTE)tag[i] == '?')	buf[pos] = '_';
		else									buf[pos] = tag[i];

		if(hangul_flag) {
			hangul_flag = OFF;
		}
		else {
			if((BYTE)tag[i] > 127) {
				hangul_flag = ON;
			}
		}
	}

	buf[12] = 0;
}

void ConvertTagToFile(char *buf, const char *tag)
{
	int size = strlen(tag);

	if(size <= 10) {	// 열글자 이하일 때는 이전의 방식을 사용한다.
		ConvertTagToFileOld(buf, tag);
		return;
	}
	
	int  i;

	strcpy(buf, tag);

	for(i = 0; i < size; i++) {
		if((BYTE)tag[i] < 32)			buf[i] = '_';
		else if((BYTE)tag[i] == '/')	buf[i] = '_';
		//else if((BYTE)tag[i] == '.')	buf[i] = '_';
		else if((BYTE)tag[i] == '\\')	buf[i] = '_';
		else if((BYTE)tag[i] == '*')	buf[i] = '_';
		else if((BYTE)tag[i] == '+')	buf[i] = '_';
		else if((BYTE)tag[i] == '|')	buf[i] = '_';
		else if((BYTE)tag[i] == ':')	buf[i] = '_';
		else if((BYTE)tag[i] == ';')	buf[i] = '_';
		else if((BYTE)tag[i] == '"')	buf[i] = '_';
		else if((BYTE)tag[i] == '<')	buf[i] = '_';
		else if((BYTE)tag[i] == '>')	buf[i] = '_';
		else if((BYTE)tag[i] == '?')	buf[i] = '_';
		else;
	}
}



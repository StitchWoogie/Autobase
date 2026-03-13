// 영문 O.K
#include "stdafx.h"
#include <tools.h>

// window 98, 2000, XP 에서는
// \ / : * ? " < > | 의 9개 문자만 사용하지 않으면 된다.

bool IsFileNameValid(const char *filename)
{
	int i;
	int length = strlen(filename);

	for(i = 0; i < length; i++) {
		if(	filename[i] == '\\' ||
			filename[i] == '/' ||
			filename[i] == ':' ||
			filename[i] == '*' ||
			filename[i] == '?' ||
			filename[i] == '"' ||
			filename[i] == '<' ||
			filename[i] == '>' ||
			filename[i] == '|')
			return false;
	}

	return true;
}
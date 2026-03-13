#include "stdafx.h"

void StringCopy(char *target, int size_t, const char *source, int size_s)
{
	if(size_s == 0) {
		target[0] = 0;
		return;
	}
	if(size_t == 0) {
		target[0] = 0;
		return;
	}

	int i;
	
	for(i = 0; i < size_t && i < size_s; i++) {
		target[i] = source[i];
		if(source[i] == NULL)	return;
	}
	target[size_t-1] = 0;

	if(size_s < size_t) {
		target[size_s] = 0;
	}
}

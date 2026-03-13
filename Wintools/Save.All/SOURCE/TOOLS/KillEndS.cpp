#include "stdafx.h"
#include <string.h>

void KillEndSpace(char *buf)
{
	int hap = strlen(buf);
	int i;

	for(i = hap-1; i >= 0; i--) {
		if(buf[i] == 32 || buf[i] == '\t') {
			buf[i] = 0;
			continue;
		}
		break;
	}
}

// TAG size O.K
#include "stdafx.h"

#include <io.h>

#include <tools.h>

int CompareFile(const char *source, const char *target)
{
	if(access(source, 0) != 0)	return 0;
	if(access(target, 0) != 0)	return 0;

	if(getfilesize(source) != getfilesize(target))	return 0;

	FILE *in1;
	FILE *in2;
	int ch1, ch2;

	in1 = fopen(source, "rb");
	if(in1 == NULL)	return 0;
	in2 = fopen(target, "rb");
	if(in2 == NULL) {
		fclose(in1);
		return 0;
	}

	while(1) {
		ch1 = fgetc(in1);
		ch2 = fgetc(in2);

		if(ch1 == EOF || ch2 == EOF)	break;
		if(ch1 != ch2) {
			fclose(in1);
			fclose(in2);
			return 0;
		}
	}

	fclose(in1);
	fclose(in2);

	return 1;
}

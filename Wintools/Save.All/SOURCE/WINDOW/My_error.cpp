#include "stdafx.h"
#include <dataswap.h>
#include <stdio.h>
#include <io.h>
#include <direct.h>
#include <stdarg.h>
#include <dos.h>

#include <tools.h>
#include <glib.h>
#include <compiler.hpp>
 
void ProgrammError(LPSTR string, ...)
{
#if	defined (_WIN32)
	va_list ap;
	StackChar imsi(1000);
	char filename[MAXPATH];
	FILE *out;
	SYSTEMTIME t;

	bell(100);

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);
	}

	wsprintf(filename, "c:\\MY-ERROR");
	if(access(filename, 0) != 0) {
		mkdir(filename);
	}

	GetSystemTime(&t);

	sprintf(filename, "c:\\MY-ERROR\\%04d%02d%02d.TXT", t.wYear, t.wMonth, t.wDay);
	if(access(filename, 0) == 0) {
		out = fopen(filename, "a");
	}
	else {
		out = fopen(filename, "w");
		fprintf(out, "이 파일은 프로그램에서 발생된 오류로 반드시 고쳐야 한다.\n");
	}
	if(out != NULL) {
		if(imsi.data == NULL) {
			fprintf(out, "%2d:%02d\t오류:메모리 부족으로 ERROR 파일을 저장할 수 없슴(확인 바람)\n", t.wHour, t.wMinute);
		}
		else {
			fprintf(out, "%2d:%02d\t%s\n", t.wHour, t.wMinute, imsi.data);
		}
		fclose(out);
	}
#else
	va_list ap;
	StackChar imsi(1000);
	char filename[MAXPATH];
	FILE *out;
	struct time t;
	struct date d;

	bell(100);

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);
	}

	wsprintf(filename, "c:\\MY-ERROR");
	if(access(filename, 0) != 0) {
		mkdir(filename);
	}

	getdate(&d);
	gettime(&t);

	sprintf(filename, "c:\\MY-ERROR\\%04d%02d%02d.TXT", d.da_year, d.da_mon, d.da_day);
	if(access(filename, 0) == 0) {
		out = fopen(filename, "a");
	}
	else {
		out = fopen(filename, "w");
		fprintf(out, "이 파일은 프로그램에서 발생된 오류로 반드시 고쳐야 한다.\n");
	}
	if(out != NULL) {
		if(imsi.data == NULL) {
			fprintf(out, "%2d:%02d\t오류:메모리 부족으로 ERROR 파일을 저장할 수 없슴(확인 바람)\n", t.ti_hour, t.ti_min);
		}
		else {
			fprintf(out, "%2d:%02d\t%s\n", t.ti_hour, t.ti_min, imsi.data);
		}
		fclose(out);
	}
#endif
}

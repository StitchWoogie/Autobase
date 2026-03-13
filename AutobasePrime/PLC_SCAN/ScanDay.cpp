#include "stdafx.h"

#include <io.h>

#include <dataswap.h>
#include "..\catlib.src\totalcfg.h"

#include "plc_scan.h"

void LogMessage(LPCSTR string, ...)
{
	va_list ap;
	StackChar imsi(1000);
	char data_dir[MAXPATH];
	SYSTEMTIME t;
	FILE *out;
	CString filename;

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);
	}

	AutoBaseIniGetProjectDataLogDirectory(sDirWorkProject, data_dir); 

	filename.Format("%s\\PlcScan", data_dir);
	if(access(filename, 0) != 0) {
		MakeDirectory(filename);
	}

	GetLocalTime(&t);

	filename.Format("%s\\PlcScan\\%04d%02d%02d.log", data_dir, t.wYear, t.wMonth, t.wDay);
	if(access(filename, 0) == 0) {
		out = fopen(filename, "a");
	}
	else {
		out = fopen(filename, "w");
	}
	if(out != NULL) {
		fprintf(out, "%2d:%02d:%02d, %s\n", t.wHour, t.wMinute, t.wSecond, imsi.data);
		fclose(out);
	}
}

static SYSTEMTIME tOld;

void CheckDayChanged()
{
	if(tOld.wYear == 0) {
		GetLocalTime(&tOld);
		return;
	}

	SYSTEMTIME t;

	GetLocalTime(&t);

	if(t.wDay == tOld.wDay)	return;

	CString message;
	int i;
	GLOBAL_PORT_STRUCT *pt;

	for(i = 0; i < nPortHap; i++) {
		pt = &portBuf[i];
		if(IsLangKorean()) {
			message.Format("전날의 Port:%d 통신(총횟수-%ld, 시간초과-%ld, 코드불량-%ld)",
								i,
								pt->countAll.Total.lCountCommTry,
								pt->countAll.Total.lCountTimeOut,
								pt->countAll.Total.lCountCodeBad
								);
		}
		else {
			message.Format("Scan memory status  Port:%d (Total-%ld, TimeOver-%ld, CodeBad-%ld)",
								i,
								pt->countAll.Total.lCountCommTry,
								pt->countAll.Total.lCountTimeOut,
								pt->countAll.Total.lCountCodeBad
								);
		}
		LogMessage(message);
		
		// 통신 상황을 다시 초기화 한다.
		memset(&pt->countAll, 0, sizeof(DEVICE_COUNT_STRUCT));
		memset(&pt->countDevice[0], 0, sizeof(DEVICE_COUNT_STRUCT));
		memset(&pt->countDevice[1], 0, sizeof(DEVICE_COUNT_STRUCT));
	}

	memcpy(&tOld, &t, sizeof(SYSTEMTIME));
}
#include "stdafx.h"

#include <io.h>

#include <dataswap.h>

#include "..\catlib\totalcfg.h"

#include "plc_scan.h"


void ScanLog(const char *string, ...)
{
	va_list ap;
	StackChar imsi(1000);
	CString filename;
	SYSTEMTIME t;
	FILE *out;
	StackChar data_dir(1000);

	AutoBaseIniGetProjectDataLogDirectory(sDirWorkProject, data_dir.data);

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);
	}

	filename.Format("%s\\ScanLog", data_dir.data);
	if(access(filename, 0) != 0) {
		MakeDirectory(filename);
	}

	GetLocalTime(&t);

	filename.Format("%s\\ScanLog\\%04d%02d%02d.log", data_dir.data, t.wYear, t.wMonth, t.wDay);	
	if(access(filename, 0) == 0) {
		out = fopen(filename, "a");
	}
	else {
		out = fopen(filename, "w");
	}
	if(out != NULL) {
		fprintf(out, "%2d:%02d:%02d\t%s\n", t.wHour, t.wMinute, t.wSecond, imsi.data);
		fclose(out);
	}
}

//------------------------------------------------------------------------------
//	하루가 바뀌면 통신상태를 Log 파일에 저장한다.
//------------------------------------------------------------------------------

void SaveScanBufStatus()
{
	char message[160];
	int i;
	int nPortHap = GetScanPortHap();

	for(i = 0; i < nPortHap; i++) {
#if	defined (COMPILE_HANGUL)
		sprintf(message, "전날의 Port:%d 통신(총횟수-%ld, 시간초과-%ld, 코드불량-%ld)",
#else
		sprintf(message, "Scan memory status  Port:%d (Total-%ld, TimeOver-%ld, CodeBad-%ld)",
#endif
							  i,
							  GetScanCommTryCount(i),
							  GetScanCommTimeOutCount(i),
							  GetScanCommCodeBadCount(i));
		SmLogMessage(message);
		// 통신 상황을 다시 초기화 한다.
		ResetScanCommCount(i);
	}
}
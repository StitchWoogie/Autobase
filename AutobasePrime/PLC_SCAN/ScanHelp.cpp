// english O.K
#include "stdafx.h"

#include <htmlhelp.h>
#include <io.h>

#include "..\catlib.src\totalcfg.h"

#include "plc_scan.h"

void PlcScanHelp(HWND hwnd, char *sHtml)
{
	char path[MAXPATH]; 

	AutoBaseMakeHtmlHelpPath(path, "PLC_SCAN.CHM");

	if(access(path, 0) != 0) {
		if(IsLangKorean()) {
			MessageBox(hwnd, path, "도움말 파일을 찾을 수 없습니다.", MB_OK);
		}else {
			MessageBox(hwnd, path, "Can't find Help file.", MB_OK);
		}
		return;
	}

	CString cmd;
	cmd.Format("hh.exe %s", path);

	WinExec(cmd, SW_SHOW);

	//HWND hwndhtml = HtmlHelp(hwnd, path, HH_DISPLAY_TOPIC, (DWORD)sHtml);

	//HWND hwndhtml = HtmlHelp(NULL, path, HH_DISPLAY_TOPIC, (DWORD)sHtml);
} 


#include "stdafx.h"
#include <io.h>

#include <glib.h>
#include <bmptool.h>
#include <gcursor.h>
#include <dataswap.h>
#include <f_format.h>
#include <tools.h>

HPICTURE LoadByImportEngine(HWND hwnd, char *filename)
{
	CString tempfile;
	char temp_dir[_MAX_PATH];
	GetTempPath(_MAX_PATH, temp_dir);

	tempfile.Format("%sTemp_Import_Png.bmp", temp_dir);

	if(access(tempfile, 0) == 0) {
		DeleteFile(tempfile);
	}

	CString command_line;

	command_line.Format("PictureConversion.exe \"%s\" \"%s\"", filename, tempfile);

	STARTUPINFO si;
	PROCESS_INFORMATION pi;

	ZeroMemory( &si, sizeof(si) );
	si.cb = sizeof(si);
	ZeroMemory( &pi, sizeof(pi) );

	char spath[_MAX_PATH];

	int nWaitExit = 1;

	strcpy(spath, command_line);

	BOOL retn_precess = CreateProcess( NULL,   // No module name (use command line)
	spath,			// Command line
    NULL,           // Process handle not inheritable
    NULL,           // Thread handle not inheritable
    FALSE,          // Set handle inheritance to FALSE
    0,              // No creation flags
    NULL,           // Use parent's environment block
    NULL,           // Use parent's starting directory 
    &si,            // Pointer to STARTUPINFO structure
    &pi ) ;          // Pointer to PROCESS_INFORMATION structure

	if(!retn_precess) {
		CString msg;
		msg.Format("Can't execute the file %s\nErrorCode=%d", command_line, retn_precess);
		MessageBox(hwnd, msg, "Execute Error", MB_OK);
		return NULL;
	}
	else {
		// 종료되기를 기다리는 옵션
		if(nWaitExit) {	
			// Wait until child process exits.
			WaitForSingleObject( pi.hProcess, INFINITE );
		}

		CloseHandle( pi.hProcess );
		CloseHandle( pi.hThread );
	}

	if(access(tempfile, 0) != 0) {
		MessageBox(hwnd, tempfile, "Converted file not found", MB_OK);
		return NULL;
	}

	return PictureLoadBmp(hwnd, tempfile);
}

HPICTURE PictureLoad(HWND hwnd, char *filename)
{
	if(access(filename, 0) != 0) {
		if(IsLangKorean()) {
			MessageBox(hwnd, "파일이 존재하지 않습니다.", filename, MB_OK);
		}
		else {
			MessageBox(hwnd, "File not exist.", filename, MB_OK);
		}
		return NULL;
	}
	
	int format = GetFileFormat(filename);

	switch(format) {
		case PCX_FORMAT:
			return PictureLoadPcx(hwnd, filename);
		case BMP_FORMAT:	
			return PictureLoadBmp(hwnd, filename);
		case LBM_FORMAT:
			return PictureLoadLbm(hwnd, filename);
		case SPT_FORMAT:
			return PictureLoadSpt(hwnd, filename);
		case MMP_FORMAT:
			return PictureLoadMmp(hwnd, filename);
		case TIF_FORMAT:
			return PictureLoadTif(hwnd, filename);
		case TGA_FORMAT:
			return PictureLoadTga(hwnd, filename);
		case JPG_FORMAT:
			return LoadByImportEngine(hwnd, filename);	// 오토베이스 10.0 부터는 2.0 라이브러리를 이용한다.
			//return PictureLoadJpg(hwnd, filename);
		case GIF_FORMAT:
			return PictureLoadGif(hwnd, filename);
		case PNG_FORMAT:
			return LoadByImportEngine(hwnd, filename);
			//return PictureLoadPng(hwnd, filename);
		default:					
			MessageBox(hwnd, "지원하지 않는 그림파일입니다.", filename, MB_OK);
			return NULL;
	}
}


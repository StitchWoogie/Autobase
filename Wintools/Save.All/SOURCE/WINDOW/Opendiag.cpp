#include "stdafx.h"
#include <memory.h>

#include <string.h>

#include <cderr.h>

#include <gclass.h>

void GDialog :: ErrorMessage()
{
	DWORD error = CommDlgExtendedError();
	if(error == 0L)	return;
	TCHAR message[80];
	switch(error) {
		//case CDERR_CHOOSECOLORCODES:
		//	strcpy(message, "CDERR_CHOOSECOLORCODES");
		//	break;
		case CDERR_DIALOGFAILURE:
			_tcscpy(message, _T("CDERR_DIALOGFAILURE"));
			break;
		case CDERR_FINDRESFAILURE:
			_tcscpy(message, _T("CDERR_FINDRESFAILURE"));
			break;
		case CDERR_GENERALCODES:
			_tcscpy(message, _T("CDERR_GENERALCODES"));
			break;
		case CDERR_INITIALIZATION:
			_tcscpy(message, _T("CDERR_INITIALIZATION"));
			break;
		case CDERR_LOADRESFAILURE:
			_tcscpy(message, _T("CDERR_LOADRESFAILURE"));
			break;
		case CDERR_LOADSTRFAILURE:
			_tcscpy(message, _T("CDERR_LOADSTRFAILURE"));
			break;
		case CDERR_LOCKRESFAILURE:
			_tcscpy(message, _T("CDERR_LOCKRESFAILURE"));
			break;
		case CDERR_MEMALLOCFAILURE:
			_tcscpy(message, _T("CDERR_MEMALLOCFAILURE"));
			break;
		case CDERR_MEMLOCKFAILURE:
			_tcscpy(message, _T("CDERR_MEMLOCKFAILURE"));
			break;
		case CDERR_NOHINSTANCE:
			_tcscpy(message, _T("CDERR_NOHINSTANCE"));
			break;
		case CDERR_NOHOOK:
			_tcscpy(message, _T("CDERR_NOHOOK"));
			break;
		case CDERR_NOTEMPLATE:
			_tcscpy(message, _T("CDERR_NOTEMPLATE"));
			break;
		case CDERR_REGISTERMSGFAIL:
			_tcscpy(message, _T("CDERR_REGISTERMSGFAIL"));
			break;
		case CDERR_STRUCTSIZE:
			_tcscpy(message, _T("CDERR_STRUCTSIZE"));
			break;
		case CFERR_CHOOSEFONTCODES:
			_tcscpy(message, _T("CFERR_CHOOSEFONTCODES"));
			break;
		case CFERR_MAXLESSTHANMIN:
			_tcscpy(message, _T("CFERR_MAXLESSTHANMIN"));
			break;
		case CFERR_NOFONTS:
			_tcscpy(message, _T("CFERR_NOFONTS"));
			break;
		case FNERR_BUFFERTOOSMALL:
			_tcscpy(message, _T("FNERR_BUFFERTOOSMALL"));
			break;
		case FNERR_FILENAMECODES:
			_tcscpy(message, _T("FNERR_FILENAMECODES"));
			break;
		case FNERR_INVALIDFILENAME:
			_tcscpy(message, _T("FNERR_INVALIDFILENAME"));
			break;
		case FNERR_SUBCLASSFAILURE:
			_tcscpy(message, _T("FNERR_SUBCLASSFAILURE"));
			break;
		case FRERR_BUFFERLENGTHZERO:
			_tcscpy(message, _T("FRERR_BUFFERLENGTHZERO"));
			break;
		case FRERR_FINDREPLACECODES:
			_tcscpy(message, _T("FRERR_FINDREPLACECODES"));
			break;
		case PDERR_CREATEICFAILURE:
			_tcscpy(message, _T("PDERR_CREATEICFAILURE"));
			break;
		case PDERR_DEFAULTDIFFERENT:
			_tcscpy(message, _T("PDERR_DEFAULTDIFFERENT"));
			break;
		case PDERR_DNDMMISMATCH:
			_tcscpy(message, _T("PDERR_DNDMMISMATCH"));
			break;
		case PDERR_GETDEVMODEFAIL:
			_tcscpy(message, _T("PDERR_GETDEVMODEFAIL"));
			break;
		case PDERR_INITFAILURE:
			_tcscpy(message, _T("PDERR_INITFAILURE"));
			break;
		case PDERR_LOADDRVFAILURE:
			_tcscpy(message, _T("PDERR_LOADDRVFAILURE"));
			break;
		case PDERR_NODEFAULTPRN:
			_tcscpy(message, _T("PDERR_NODEFAULTPRN"));
			break;
		case PDERR_NODEVICES:
			_tcscpy(message, _T("PDERR_NODEVICES"));
			break;
		case PDERR_PARSEFAILURE:
			_tcscpy(message, _T("PDERR_PARSEFAILURE"));
			break;
		case PDERR_PRINTERCODES:
			_tcscpy(message, _T("PDERR_PRINTERCODES"));
			break;
		case PDERR_PRINTERNOTFOUND:
			_tcscpy(message, _T("PDERR_PRINTERNOTFOUND"));
			break;
		case PDERR_RETDEFFAILURE:
			_tcscpy(message, _T("PDERR_RETDEFFAILURE"));
			break;
		case PDERR_SETUPFAILURE:
			_tcscpy(message, _T("PDERR_SETUPFAILURE"));
			break;

		default:
			_stprintf(message, _T("errorcode - %lu"), error);
	}
	MessageBox(NULL, message, _T("OpenSaveDialogError"), MB_OK);
}

static bool IsOsVersion4()
{
	OSVERSIONINFO osvi;
	ZeroMemory(&osvi, sizeof(OSVERSIONINFO));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionEx(&osvi);
	if(osvi.dwMajorVersion == 4)	return true;		// 95,98,me,nt4
	return false;
}

GOpenDialog :: GOpenDialog(HWND hwnd)
{
	ofn = new OPENFILENAME[1];
	szDirName = new TCHAR[256];
	szFile = new TCHAR[256];
	szFileTitle = new TCHAR[256];
	szFilter = NULL;
	szTitle = NULL;
	// Set all structure members to zero.
	memset(ofn, 0, sizeof(OPENFILENAME));

	//if(IsOsVersion4()) 
	//	ofn->lStructSize = OPENFILENAME_SIZE_VERSION_400;
	//else
		ofn->lStructSize = sizeof(OPENFILENAME);
	
	ofn->hwndOwner = hwnd;
	ofn->hInstance = 0;
	ofn->lpstrFilter = szFilter;
	ofn->lpstrCustomFilter = NULL;
	ofn->nMaxCustFilter = 0L;
	ofn->nFilterIndex = 1L;
	ofn->lpstrFile= (LPTSTR)szFile;
	ofn->nMaxFile = 256;
	ofn->lpstrFileTitle = szFileTitle;
	ofn->nMaxFileTitle = 256;
	ofn->lpstrInitialDir = szDirName;
	ofn->lpstrTitle = szTitle;
	ofn->Flags = 0;

	SetFilter(_T("All Files(*.*)|*.*|"));
//	strcpy(szDirName, "graphic\\");
	szFile[0] = '\0';
/*
typedef struct tagOPENFILENAME {
	DWORD     lStructSize;
	HWND      hwndOwner;
	HINSTANCE hInstance;
	LPCSTR    lpstrFilter;
	LPSTR     lpstrCustomFilter;
	DWORD     nMaxCustFilter;
	DWORD     nFilterIndex;
	LPSTR     lpstrFile;
	DWORD     nMaxFile;
	LPSTR     lpstrFileTitle;
	DWORD     nMaxFileTitle;
	LPCSTR    lpstrInitialDir;
	LPCSTR    lpstrTitle;
	DWORD     Flags;
	UINT      nFileOffset;
	UINT      nFileExtension;
	LPCSTR    lpstrDefExt;
	LPARAM    lCustData;
	UINT      (CALLBACK* lpfnHook) (HWND, UINT, WPARAM, LPARAM);

	LPCSTR    lpTemplateName;
} OPENFILENAME;
*/
}

GOpenDialog :: ~GOpenDialog()
{
	if(szTitle != NULL) 		delete szTitle;
	if(szDirName != NULL)	delete szDirName;
	if(szFile != NULL)		delete szFile;
	if(szFileTitle != NULL)	delete szFileTitle;
	if(szFilter != NULL)		delete szFilter;
	if(ofn != NULL)			delete ofn;
}

// 0 - 기본 1부터 시작
void GOpenDialog :: SetFilterIndex(int index)
{
	ofn->nFilterIndex = index;
}

int GOpenDialog :: GetFilterIndex()
{
	return ofn->nFilterIndex;
}

void GOpenDialog :: SetFilter(const TCHAR *string)
{
	if(szFilter != NULL) {
		delete szFilter;
		szFilter = NULL;
	}

	szFilter = new TCHAR[_tcslen(string)+1];
	if(szFilter == NULL) {
		MessageBox(NULL, _T("Low memory"), _T("GOpenDialog :: SetFilter"), MB_OK);
	}

	ofn->lpstrFilter = szFilter;
			
	_tcscpy(szFilter, string);

	int i;
	int shap = _tcslen(szFilter);
	
	ofn->nFilterIndex = 0;
	for(i = 0; i < shap; i++) {
		if(szFilter[i] == '|') {
			szFilter[i] = '\0';
			//ofn->nFilterIndex++;
		}
	}

	ofn->nFilterIndex = 50;
}

void GOpenDialog :: SetTitle(const TCHAR *string)
{
	if(szTitle != NULL) {
		delete szTitle;
		szTitle = NULL;
	}
	szTitle = new TCHAR[_tcslen(string)+1];
	if(szTitle == NULL)	return;
	_tcscpy(szTitle, string);
	ofn->lpstrTitle = szTitle;
}

BOOL GOpenDialog :: Execute()
{
	BOOL retn;

	retn = GetOpenFileName(ofn);
	if(retn == FALSE) {	// error
		ErrorMessage();
	}
	return retn;
}

void GOpenDialog :: SetDir(const TCHAR *dir)
{
	if(szDirName != NULL)	_tcscpy(szDirName, dir);
}





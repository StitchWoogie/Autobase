#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>            
#else
#include <memory.h> 
#endif
#include <string.h>

#include <tools.h>
#include <gclass.h>

GFontDialog :: GFontDialog(HWND hwnd)
{
	logFont = new LOGFONT[1];
	memset(logFont, 0, sizeof(LOGFONT));

	choose = new CHOOSEFONT[1];

	if(choose != NULL) {
		memset(choose, 0, sizeof(CHOOSEFONT));

		choose->lStructSize = sizeof(CHOOSEFONT);
		choose->hwndOwner = hwnd;
		choose->lpLogFont = logFont;
		choose->Flags = CF_SCREENFONTS | CF_EFFECTS | CF_TTONLY | CF_INITTOLOGFONTSTRUCT;
		choose->rgbColors = RGB(0, 0, 0);
		choose->nFontType = SCREEN_FONTTYPE;
	}
}

GFontDialog :: ~GFontDialog()
{
	if(choose != NULL) 	delete choose;
	if(logFont != NULL)	delete logFont;
}

BOOL GFontDialog :: Execute()
{
	BOOL retn;

	retn = ChooseFont(choose);
	if(retn == 0L) {	// error
		ErrorMessage();
	}
	return retn;
}

void GFontDialog :: SetLogFont(LOGFONT *lf)
{
	if(logFont == NULL)	return;
	memcpy(logFont, lf, sizeof(LOGFONT));
}

void MakeDefaultLogFont(LOGFONT *lf)
{
	// log font setting
	memset(lf, 0, sizeof(LOGFONT));

	lf->lfHeight = -16;
	lf->lfWidth = 0;
	lf->lfEscapement = 0;
	lf->lfOrientation = 0;
	lf->lfWeight = FW_NORMAL;
	lf->lfItalic = 0;
	lf->lfUnderline = 0;
	lf->lfStrikeOut = 0;
	//lf->lfCharSet = 129;
	lf->lfOutPrecision = 3;
	lf->lfClipPrecision = 2;
	lf->lfQuality = 1;
	lf->lfPitchAndFamily = 17;			// font size
	//strcpy(lf->lfFaceName, "¹ÙÅÁÃ¼");

	if(IsLangKorean()) {	// korean
		lf->lfCharSet = 129;
		_tcscpy(lf->lfFaceName, _T("±¼¸²Ã¼"));
	}
	else if(IsLangJapanese()) {
		lf->lfHeight = -12;
		lf->lfCharSet = 128;
		lf->lfPitchAndFamily = 50;
		_tcscpy(lf->lfFaceName, _T("MS UI Gothic"));
	}
	else if(IsLangChinese()) {
		lf->lfHeight = -12;
		lf->lfCharSet = 134;
		lf->lfPitchAndFamily = 50;
		_tcscpy(lf->lfFaceName, _T("SimSun"));
	}
	else {
		lf->lfCharSet = ANSI_CHARSET;
		_tcscpy(lf->lfFaceName, _T("Arial"));
	}
}


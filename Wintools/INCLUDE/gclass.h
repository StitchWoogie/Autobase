#if	!defined (__GCLASS_H)
#define __GCLASS_H
#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#if	!defined (__COMMDLG_H)
#include <commdlg.h>
#endif

#if	!defined (__STRING_H)
#include <string.h>
#endif

#pragma pack(push, 1)

class GDialog {
	protected:
		void ErrorMessage();
};

class GOpenDialog : public GDialog {
  protected:
	OPENFILENAME *ofn;
	TCHAR *szDirName;
	TCHAR *szFile, *szFileTitle;
	TCHAR *szFilter;
	TCHAR *szTitle;				// 다이어로그 제목
//	void ErrorMessage();
  public:
	GOpenDialog(HWND);
	~GOpenDialog();
	BOOL Execute();
	void SetFilter(const TCHAR *string);
	void SetFilterIndex(int index);
	int GetFilterIndex();
	void SetTitle(const TCHAR *string);
	inline void GetFileName(TCHAR *string) { _tcscpy(string, szFile); }
	void SetDir(const TCHAR *dir);
};

class GSaveDialog : public GOpenDialog {
  public:
	GSaveDialog(HWND);
	~GSaveDialog();
	BOOL Execute();
	void SetOverWritePrompt();
};

class GColorDialog : public GDialog {
		CHOOSECOLOR *choose;
	public:
		GColorDialog(HWND);
		~GColorDialog();
		BOOL Execute(COLORREF init = RGB(0, 0, 0));
		COLORREF rgbResult() {	return choose->rgbResult; }
} ;

class GFontDialog : public GDialog {
		CHOOSEFONT *choose;
		LOGFONT *logFont;
	public:
		GFontDialog(HWND);
		~GFontDialog();
		BOOL Execute();
		void GetLogFont(LOGFONT *lf)	{	memcpy(lf, logFont, sizeof(LOGFONT)); }
		void SetLogFont(LOGFONT *lf);
} ;

void MakeDefaultLogFont(LOGFONT *lf);

class GPrintDialog : public GDialog {
		PRINTDLG *choose;
		HGLOBAL  hDevMode;
	public:
		GPrintDialog(HWND);
		~GPrintDialog();
		void GetStructure(PRINTDLG *st);
		BOOL Execute();
      HDC GetDC();
} ;

#pragma pack(pop)

#endif

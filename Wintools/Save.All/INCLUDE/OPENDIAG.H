#if	!defined (__WINDOWS_H)
#include <windows.h>
#endif

#if	!defined (__COMMDLG_H)
#include <commdlg.h>
#endif

#if	!defined (__STRING_H)
#include <string.h>
#endif

class GOpenDialog {
	OPENFILENAME ofn;
	char szDirName[256];
	char szFile[256], szFileTitle[256];
	char szFilter[256];
	char *szTitle;				// 다이어로그 제목
  public:
	GOpenDialog(HWND);
	~GOpenDialog();
	BOOL Execute();
	void SetFilter(char *string);
	void SetTitle(char *string);
	inline void GetFileName(char *string) { strcpy(string, szFile); }
};


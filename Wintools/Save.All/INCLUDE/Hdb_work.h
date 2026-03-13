// HDB의 편집 도구에서 필요한 부분.

#if	!defined (__HDB_LIB_H)
#include <hdb_lib.h>
#endif

typedef struct {
	RECT	rect;			// window rect size

	DWORD	dwCurrX;		// 현재 셀의 위치
	DWORD	dwCurrY;		// 현재 셀의 위치
	DWORD	dwPageX;		// 현재페이지의 제일 처음 필드
	DWORD	dwPageY;	// 현재페이지의 제일 처음 블럭
	int		cxChar;
	int 	cyChar;
	int		pxStart;		// 실제 셀이 시작하는 위치.
	int		pyStart;		// 실제 셀이 시작하는 위치.
	
	int		limity;

	DWORD	selx1;			// 현재 선택된 cell
	DWORD	sely1;
	DWORD	selx2;
	DWORD	sely2;

	HINSTANCE hInst;

	HDB_SHEET_STRUCT *sheet;	// sheet struct

	HWND hwndEditCell;		// 각 셀을 입력할 수 있는 입력기.
} HDB_WORK_STRUCT;

void HDBWorkInit(HWND hwnd, HDB_WORK_STRUCT *work, HINSTANCE hinst);
void HDBWorkWmSize(HWND hwnd, HDB_WORK_STRUCT *work);
void HDBWorkWmPaint(HWND hwnd, HDC hdc, HDB_WORK_STRUCT *work);	
void HDBWorkWmKeyDown(HWND hwnd, HDB_WORK_STRUCT *work, WPARAM wParam);
void HDBWorkWmLButtonDown(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam);
void HDBWorkWmLButtonDblClk(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam);
void HDBWorkWmMouseMove(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam);
void HDBWorkWmLButtonUp(HWND hwnd, HDB_WORK_STRUCT *work);
void HDBWorkWmHScroll(HWND hwnd, HDB_WORK_STRUCT *work, UINT nSBCode, UINT nPos);
void HDBWorkWmVScroll(HWND hwnd, HDB_WORK_STRUCT *work, UINT nSBCode, UINT nPos);
int  HDBWorkCloseCellEdit(HDB_WORK_STRUCT *work);

void HDBWorkIdmCopy(HWND hwnd, HDB_WORK_STRUCT *work);
void HDBWorkIdmPaste(HWND hwnd, HDB_WORK_STRUCT *work);
void HDBWorkIdmCut(HWND hwnd, HDB_WORK_STRUCT *work);
void HDBWorkIdmInsertOneBlock(HWND hwnd, HDB_WORK_STRUCT *work);
void HDBWorkIdmAddOneBlock(HWND hwnd, HDB_WORK_STRUCT *work);
void HDBWorkIdmDelOneBlock(HWND hwnd, HDB_WORK_STRUCT *work);

int  HDBWorkSetAlwaysViewFieldSize(HDB_WORK_STRUCT *work, int size);


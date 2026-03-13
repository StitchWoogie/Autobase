// TAG size O.K
#include "stdafx.h"
#include <tools.h>
#include <glib.h>
#include <pic_tool.h>
#include <ch_buf.h>

#include <clipview.h>

typedef struct {			// 화면을 디렉토리별로 볼 수 있는 작업화면을 나타낸다.
	int	cxChar;
	int	cyChar;
	char  directory[MAXPATH];	// 현재의 디렉토리

	WORD	ctrl_id;

	HWND  hwndDir;			// 디렉토리 hwnd
	HWND  hwndAlbum;		// 앨범 화면의 윈도우 핸들 (child)
} WORK_DIR_STRUCT;

#define	IDC_CLIPVIEW_WORKDIR_LIST	100
#define	IDC_CLIPVIEW_WORKDIR_ALBUM	101

char *szClassNameClipViewWorkDir = "ClipViewWorkDir";

static int  IsPictureFile(char *filename)
{
	FnSplit fnsplit;
	char	  ext[MAXEXT];
	int i;

	fnsplit.fnsplit(filename);
	fnsplit.GetExt(ext);
	strupr(ext);

#define	MAX_COMPARE	8

	static char *sExt[MAX_COMPARE] = { ".PCX", ".GIF", ".SPT", ".BMP", ".LBM", ".TGA", ".TIF", ".JPG" };

	for(i = 0; i < MAX_COMPARE; i++) {
		if(strcmp(ext, sExt[i]) == 0) {
			return 1;	
		}
	}
	return 0;
}

static void FindFile(WORK_DIR_STRUCT *work)
{
	HCURSOR hCursorOld;
	
	FindFileClass find;
	struct ffblk32 ffblk;
	int done;
	char filename[MAXPATH];

	hCursorOld = SetCursor(LoadCursor(NULL, IDC_WAIT));
	
	done = find.findfirst("*.*", &ffblk, FA_ARCH | FA_HIDDEN);

	while(!done) {
		if(ffblk.ff_name[0] == '.')			goto next;
		if(ffblk.ff_attrib & FA_DIREC)		goto next;
		if(!IsPictureFile(ffblk.ff_name))	goto next;

		sprintf(filename, "%s\\%s", work->directory, ffblk.ff_name);
		
		SendMessage(work->hwndAlbum, LB_ADDSTRING, 0, (LPARAM)filename);
	  next:	
		done = find.findnext(&ffblk);				
	}

	SetCursor(hCursorOld);
}

static void FillListBox(HWND hwndList, char *sActive)
{
	FindFileClass find;
	struct ffblk32 ffblk;
	int done;

	SendMessage(hwndList, LB_ADDSTRING, 0, (LPARAM)"..");
	
	done = find.findfirst("*.*", &ffblk, FA_ARCH);

	while(!done) {
		if(ffblk.ff_name[0] == '.')			goto next;
		if(!(ffblk.ff_attrib & FA_DIREC))	goto next;	// directory가 아니다.
		
		SendMessage(hwndList, LB_ADDSTRING, 0, (LPARAM)ffblk.ff_name);
				
	  next:	
		done = find.findnext(&ffblk);				
	}

	char drive[MAXPATH];
	int  type;
	int  i;

	for(i = 0; i < 26; i++) {
		sprintf(drive, "%c:\\", i+'A');
		type = GetDriveType(drive);
		if(type == DRIVE_REMOVABLE ||
			type == DRIVE_FIXED ||
			type == DRIVE_REMOTE ||
			type == DRIVE_CDROM ||
			type == DRIVE_RAMDISK) {
			sprintf(drive, "[%c:]", i+'A');
			SendMessage(hwndList, LB_ADDSTRING, 0, (LPARAM)drive);	
		}
	}

	if(sActive != NULL && sActive[0] != 0) {
		int index;

		index = SendMessage(hwndList, LB_FINDSTRING, 0, (LPARAM)sActive);	
		SendMessage(hwndList, LB_SETCURSEL, index, 0L);	
	}
}

static void WmCreate(HWND hwnd, LPARAM lParam)
{
	HGLOBAL hGlobal;
	WORK_DIR_STRUCT *work;
	HDC hdc;
	TEXTMETRIC tm;
	LPCREATESTRUCT create = (LPCREATESTRUCT)lParam;

	// Allocate memory for window private data
	hGlobal = GlobalAlloc (GMEM_MOVEABLE | GMEM_ZEROINIT,
								   sizeof (WORK_DIR_STRUCT));

	SetWindowLong(hwnd, 0, (LONG)hGlobal);

	work = (WORK_DIR_STRUCT*) GlobalLock (hGlobal);

	hdc = GetDC(hwnd);
	GetTextMetrics(hdc, &tm);
	ReleaseDC(hwnd, hdc);

	GetCurrentDirectory(MAXPATH, work->directory);
	work->cxChar = tm.tmAveCharWidth;
	work->cyChar = tm.tmExternalLeading+tm.tmHeight;
	work->ctrl_id = (WORD)create->hMenu;

	work->hwndAlbum = CreateWindow(szClassNameClipViewAlbum,  "Clip view",
						   WS_CHILD | WS_VISIBLE,
						   0, 0, 0, 0,
						   hwnd, 0, create->hInstance, NULL);

	work->hwndDir = CreateWindow ("listbox", "Directory",
				  WS_CHILD |
				  WS_VISIBLE |
				  WS_VSCROLL |
				  WS_HSCROLL |
				  LBS_USETABSTOPS |
				  LBS_NOTIFY |
				  LBS_NOINTEGRALHEIGHT | 
				  LBS_SORT, 
				  0, 0, 0, 0,
				  hwnd, (HMENU)1, create->hInstance, NULL);

	FillListBox(work->hwndDir, NULL);

	//SendMessage(work->hwndDir, LB_DIR, DDL_DIRECTORY | DDL_DRIVES, (LPARAM)"*.*");
	
	FindFile(work);

	SetWindowText(hwnd, work->directory);

	GlobalUnlock (hGlobal);
}

static void WmPaint(HWND hwnd)
{
	HDC hdc;
	PAINTSTRUCT ps;
	HGLOBAL hGlobal;
	WORK_DIR_STRUCT *work;
	RECT rect;
	RECT r;

	GetClientRect(hwnd, &rect);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_DIR_STRUCT*)GlobalLock(hGlobal);

	hdc = BeginPaint(hwnd, &ps);
	PopBox2(hdc, 0, 0, rect.right-1, work->cyChar+1, WHITE_GRAY_COLOR);	
	r.left = 5;
	r.top  = 1;
	r.right = rect.right-1;
	r.bottom = work->cyChar+1;
	SetTextColor(hdc, DARK_COLOR);
	SetBkColor(hdc, WHITE_GRAY_COLOR);
	DrawText(hdc, work->directory, strlen(work->directory), &r, 0);
	EndPaint(hwnd, &ps);

	GlobalUnlock(hGlobal);
}

static void WmSize(HWND hwnd)
{
	RECT rect;
	HGLOBAL hGlobal;
	WORK_DIR_STRUCT *work;

	GetClientRect(hwnd, &rect);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_DIR_STRUCT*)GlobalLock(hGlobal);

	MoveWindow(work->hwndDir,   0,   work->cyChar+2, 100, rect.bottom-(work->cyChar+2), TRUE);
	MoveWindow(work->hwndAlbum, 100, work->cyChar+2, rect.right-100, rect.bottom-(work->cyChar+2), TRUE);
	
	GlobalUnlock(hGlobal);
}

static void WmCommand(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	HGLOBAL hGlobal;
	WORK_DIR_STRUCT *work;
	char activename[80];

	if(HIWORD(wParam) == LBN_DBLCLK) {
		char dirname[MAXPATH];
		int  pos;
		int i;

		hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
		work = (WORK_DIR_STRUCT*)GlobalLock(hGlobal);

		if((HWND)lParam == work->hwndAlbum) {
			PostMessage(GetParent(hwnd), WM_COMMAND, MAKELONG(work->ctrl_id, LBN_DBLCLK), (LPARAM)hwnd); 
			GlobalUnlock(hGlobal);
			return;
		}

		pos = SendMessage(work->hwndDir, LB_GETCURSEL, 0, 0L);
		SendMessage(work->hwndDir, LB_GETTEXT, pos, (LPARAM)dirname);

		SendMessage(work->hwndAlbum, LB_RESETCONTENT, 0, 0L);
		SendMessage(work->hwndDir, LB_RESETCONTENT, 0, 0L);

		if(dirname[2] == ':') {
			sprintf(work->directory, "%c:", dirname[1]);
			activename[0] = 0;
		}
		else if(strcmp(dirname, "..") == 0) {
			activename[0] = 0;
			for(i = strlen(work->directory)-1; i > 0; i--) {
				if(work->directory[i] == '\\') {
					strcpy(activename, &work->directory[i+1]);
					work->directory[i] = 0;

					if(work->directory[strlen(work->directory)-1] == ':') {
						strcat(work->directory, "\\");
					}
					break;
				}
			}
			
		}
		else {
			if(work->directory[strlen(work->directory)-1] != '\\') {
				strcat(work->directory, "\\");
			}
			strcat(work->directory, dirname);
			strcpy(activename, "..");
		}

		ChangeDirectory(work->directory);
		GetCurrentDirectory(MAXPATH, work->directory);

		FillListBox(work->hwndDir, activename);
		FindFile(work);
		SetWindowText(hwnd, work->directory);

		GlobalUnlock(hGlobal);

		InvalidateRect(hwnd, NULL, FALSE);

		return;
	}
}

static LRESULT SendMessageToAlbum(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	HGLOBAL hGlobal;
	WORK_DIR_STRUCT *work;
	HWND hwndAlbum;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_DIR_STRUCT*)GlobalLock(hGlobal);
	hwndAlbum = work->hwndAlbum;
	GlobalUnlock(hGlobal);

	return SendMessage(hwndAlbum, message, wParam, lParam);
}

LRESULT CALLBACK WndProcClipViewWorkDir (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	HGLOBAL		    hGlobal;
	WORK_DIR_STRUCT     *work;
	
	switch (message) {
		case WM_CREATE:
			WmCreate(hwnd, lParam);
			return 0;
		case WM_SIZE:
			WmSize(hwnd);
			break;
		case WM_PAINT:
			WmPaint(hwnd);
			return 0;
		case LB_SELECTSTRING:
		case LB_GETCURSEL:
		case LB_GETTEXT:
		case LB_ADDFILE:	// make thumbnail
		case LB_SELITEMRANGE:
		case LB_DELETESTRING:
			return SendMessageToAlbum(hwnd, message, wParam, lParam);
		case WM_COMMAND:
			WmCommand(hwnd, wParam, lParam);
			return 0;
		case WM_DESTROY:
			hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0);
			if(hGlobal) {
				work = (WORK_DIR_STRUCT*) GlobalLock (hGlobal);
							
				GlobalUnlock (hGlobal);
				GlobalFree (hGlobal);
			}
			return 0;
	}
	// Pass unprocessed message to DefMDIChildProc

	return DefWindowProc (hwnd, message, wParam, lParam);
}

void ClipViewRegisterClassWorkDir(HINSTANCE hinstance)
{
	ClipViewRegisterClassAlbum(hinstance);	// 먼저 child로 필요한 album윈도우를 등록한다.
	
	static char install_flag = OFF;
	if(install_flag == ON)	return;		// already register

	WNDCLASS wndclass;
	
	// Register the  Alnum window class

	wndclass.style         = CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS;
	wndclass.lpfnWndProc   = WndProcClipViewWorkDir;
	wndclass.cbClsExtra    = 0 ;
	wndclass.cbWndExtra    = sizeof (HGLOBAL);
	wndclass.hInstance     = hinstance ;
	wndclass.hIcon         = LoadIcon (NULL, IDI_APPLICATION) ;
	wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW) ;
	wndclass.hbrBackground = (HBRUSH)GetStockObject(NULL_BRUSH);
	wndclass.lpszMenuName  = NULL ;
	wndclass.lpszClassName = szClassNameClipViewWorkDir;

	RegisterClass (&wndclass) ;

	install_flag = ON;
}



 
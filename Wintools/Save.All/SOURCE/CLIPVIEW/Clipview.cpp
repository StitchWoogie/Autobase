// 영문 O.K
#include "stdafx.h"
#include <tools.h>
#include <glib.h>
#include <pic_tool.h>
#include <ch_buf.h>
#include <kwl\kdialog.h>
#include <clipview.h>

typedef struct {			// 앨범처럼 나타나는 화면에 대한 struct
	int	limitx;
	int	limity;
	int	cxChar;
	int	cyChar;
	DWORD	cursor;
	WORD	ctrl_id;			// create할때 hMenu값.
	int	nScrollPos;
	int	nScrollHap;
	char  bScrollBar;
	
	DWORD	timerpos;		// timer가 발생할 때마다 하나씩 증가하는데 이것은 디스크에 있는 부분 그림을 시간나는대로 불러오기 위해서 존재한다.

	HPALETTE hPal;			// 사용할 palette

	Block *block;
} ALBUM_WINDOW_STRUCT;

typedef struct {
	char    filename[MAXPATH];		// 전체 이름이 담겨 있다. 
	char	  title[MAXPATH];			// 표시될 이름만 나타낸다.	(디렉토리를 제외한 파일명만)
	char    select;
	char	  keyword[80];

	HGLOBAL hCut;
} FILE_STRUCT;

char *szClassNameClipViewAlbum = "ClipViewAlbum";

static void WmCreate(HWND hwnd, LPARAM lParam)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	HDC hdc;
	TEXTMETRIC tm;
	LPCREATESTRUCT create = (LPCREATESTRUCT)lParam;

	// Allocate memory for window private data
	hGlobal = GlobalAlloc (GMEM_MOVEABLE | GMEM_ZEROINIT,
								   sizeof (ALBUM_WINDOW_STRUCT));

	SetWindowLong(hwnd, 0, (LONG)hGlobal);

	work = (ALBUM_WINDOW_STRUCT*) GlobalLock (hGlobal);

	work->hPal = MakePaletteWithDac(DEFAULT_RGB, 256);

	hdc = GetDC(hwnd);
	GetTextMetrics(hdc, &tm);
	SelectPalette(hdc, work->hPal, FALSE);
	RealizePalette(hdc);
	ReleaseDC(hwnd, hdc);

	work->timerpos = 0;
	work->cursor = 0;
	work->ctrl_id = (WORD)create->hMenu;

	work->cxChar = tm.tmAveCharWidth;
	work->cyChar = tm.tmExternalLeading+tm.tmHeight;
		
	work->block = new Block(sizeof(FILE_STRUCT));
	work->nScrollPos = 0;
	work->nScrollHap = 0;
	work->bScrollBar = OFF;

	GlobalUnlock (hGlobal) ;
}

static void DrawOneImage(HDC hdc, ALBUM_WINDOW_STRUCT *work, int x, int y, DWORD pos)
{
	int cellwidth = 6+100;
	int cellheight = 6+100+work->cyChar;
	FILE_STRUCT block;
	
	if(work->cursor == pos) {
		PushBox2(hdc, x, y,  x+cellwidth, y+cellheight, RGB(0, 0, 255));
		PushBox2(hdc, x+2, y+work->cyChar+3, x+cellwidth-2, y+cellheight-2, WHITE_GRAY_COLOR);
	}
	else {
		PopBox2(hdc, x, y,  x+cellwidth, y+cellheight, WHITE_GRAY_COLOR);
		PushRectangle2(hdc, x+2, y+work->cyChar+3, x+cellwidth-2, y+cellheight-2);
	}

	work->block->GetBlock((BYTE*)&block, pos);
	
	if(block.select) {
		if(work->cursor == pos) {
			SetTextColor(hdc, RGB(255, 0, 0));
			SetBkColor(hdc, RGB(0, 0, 255));
		}
		else {
			SetTextColor(hdc, RGB(255, 0, 0));
			SetBkColor(hdc, WHITE_GRAY_COLOR);
		}
	}
	else {
		if(work->cursor == pos) {
			SetTextColor(hdc, WHITE_COLOR);
			SetBkColor(hdc, RGB(0, 0, 255));
		}
		else {
			SetTextColor(hdc, DARK_COLOR);
			SetBkColor(hdc, WHITE_GRAY_COLOR);
		}
	}
	TextOut(hdc, x+3, y+1, block.title, strlen(block.title));
	
	if(block.hCut) {
		BYTE *lpDib = (BYTE *) GlobalLock(block.hCut);
		BYTE *lpDibBits = GetDibBitsAddr (lpDib);

		int cxDib     = GetDibWidth    (lpDib) ;
		int cyDib     = GetDibHeight   (lpDib) ;

		SetDIBitsToDevice (hdc, x+3, y+work->cyChar+4, cxDib, cyDib, 0, 0,
								 0, cyDib, (LPSTR) lpDibBits,
								 (LPBITMAPINFO) lpDib,
								 DIB_RGB_COLORS);

		GlobalUnlock(block.hCut);
	}
}

static void WmPaint(HWND hwnd)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	HDC hdc;
	PAINTSTRUCT ps;
	RECT rect;
	int i;
	int x, y;
	DWORD pos;
	
	GetClientRect(hwnd, &rect);

	hdc = BeginPaint(hwnd, &ps);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	SelectPalette(hdc, work->hPal, FALSE);
	//RealizePalette(hdc);

	gcls(hdc, 0, 0, rect.right, rect.bottom, WHITE_GRAY_COLOR);

	int cellwidth = 6+100;
	int cellheight = 6+100+work->cyChar;
	int hap = 0;

	pos = work->limitx*work->nScrollPos;

	SetTextColor(hdc, DARK_COLOR);
	SetBkColor(hdc, WHITE_GRAY_COLOR);

	for(y = 5; y < rect.bottom; y+=cellheight+5) {
		for(i = 0, x = 5; i < work->limitx; i++, x+=cellwidth+5) {
			hap = work->block->GetBlockCount();
			if(pos >= work->block->GetBlockCount())	goto out;

			DrawOneImage(hdc, work, x, y, pos);

			pos++;
		}
	}
out:;

	GlobalUnlock(hGlobal);

	EndPaint(hwnd, &ps);
}

static void WmSize(HWND hwnd)
{
	RECT rect;
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	static char flag= OFF;

	if(flag)	return;

	flag = ON;

	GetClientRect(hwnd, &rect);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	work->limitx = rect.right/110;
	work->limity = rect.bottom/(110+work->cyChar);

	if(work->limitx == 0)	work->limitx = 1;
	if(work->limity == 0)	work->limity = 1;

	work->nScrollHap = (work->block->GetBlockCount()+work->limitx-1)/work->limitx;

	if(work->nScrollHap > work->limity) {
		if(work->bScrollBar == OFF) {
			ShowScrollBar(hwnd, SB_VERT, TRUE);
			work->bScrollBar = ON;
		}
		SetScrollRange(hwnd, SB_VERT, 0, work->nScrollHap-1, TRUE);
		SetScrollPos  (hwnd, SB_VERT, work->nScrollPos, TRUE);
	}
	else {
		if(work->bScrollBar) {
			ShowScrollBar(hwnd, SB_VERT, FALSE);
			work->bScrollBar = OFF;
		}
	}

	GlobalUnlock(hGlobal);

	flag = OFF;
}

static void GetAlbumPosition(ALBUM_WINDOW_STRUCT *work, int pos, int &x, int &y)
{
	int cellwidth  = 6+100;
	int cellheight = 6+100+work->cyChar;

	x = 5+(cellwidth +5)*(pos%work->limitx);

	if(abs(work->nScrollPos-(pos/work->limitx)) > work->limity+1) {
		y = 0-(cellheight+100);
	}
	else {
		y = 5+(cellheight+5)*((pos-work->nScrollPos*work->limitx)/work->limitx);
	}
}

static void WmButtonDown(HWND hwnd, LPARAM lParam, char button_flag, char dblclk_flag)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	HDC hdc;
	RECT rect;
	int i;
	int x, y;
	DWORD pos;
	FILE_STRUCT block;
	int mx = LOWORD(lParam), my = HIWORD(lParam);
	int drawx, drawy;
	char selected = OFF;
	WORD id;

	GetClientRect(hwnd, &rect);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	int cellwidth = 6+100;
	int cellheight = 6+100+work->cyChar;

	pos = work->limitx*work->nScrollPos;
	
	for(y = 5; y < rect.bottom; y+=cellheight+5) {
		for(i = 0, x = 5; i < work->limitx; i++, x+=cellwidth+5) {
			if(pos >= work->block->GetBlockCount())	goto out;

			if(mx >= x && my >= y && mx <= x+cellwidth && my <= y+cellheight) {
				if(work->cursor == pos && button_flag == 0) {
					if(dblclk_flag) {
						selected = ON;
						id = work->ctrl_id;
					}
					goto out;
				}

				if(button_flag == 1) {	// right button pressed
					work->block->GetBlock((BYTE*)&block, pos);								
					block.select++;
					block.select %= 2;
					work->block->SetBlock((BYTE*)&block, pos);								
				}
				
				int save_pos;
				
				hdc = GetDC(hwnd);

				SelectPalette(hdc, work->hPal, FALSE);

				save_pos = work->cursor;
				work->cursor = pos;

				GetAlbumPosition(work, save_pos, drawx, drawy);
				DrawOneImage(hdc, work, drawx, drawy, save_pos);

				GetAlbumPosition(work, work->cursor, drawx, drawy);
				DrawOneImage(hdc, work, drawx, drawy, work->cursor);

				ReleaseDC(hwnd, hdc);
				
				GlobalUnlock(hGlobal);
				return;
			}
			
			pos++;
		}
	}
out:;
	GlobalUnlock(hGlobal);

	if(selected) {	// double clik으로 항목이 선택되었다.
		PostMessage(GetParent(hwnd), WM_COMMAND, MAKELONG(work->ctrl_id, LBN_DBLCLK), (LPARAM)hwnd); 
	}
}

static void WmTimer(HWND hwnd)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	FILE_STRUCT block;
	char filename[MAXPATH];
	FILE *in;
	INFO_FILE_HEADER head;
	int x, y;
	HDC hdc;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	if(work->timerpos >= work->block->GetBlockCount())	{
		GlobalUnlock(hGlobal);
		return;
	}

	work->block->GetBlock((BYTE*)&block, work->timerpos);

	if(block.hCut)	{		// 이미 읽혀져 있다.
		work->timerpos++;
		GlobalUnlock(hGlobal);
		return;
	}

	strcpy(filename, block.filename);
	filename[strlen(filename)-1] = '_';

	in = fopen(filename, "rb");

	if(in == NULL) {
		work->timerpos++;
		GlobalUnlock(hGlobal);
		return;
	}

	fread(&head, 1, sizeof(INFO_FILE_HEADER), in);

	strcpy(block.keyword, head.keyword);

	block.hCut = GlobalAlloc(GMEM_MOVEABLE, head.image_size);

	if(block.hCut) {
		BYTE *buf;

		buf = (BYTE*) GlobalLock(block.hCut);
		for(DWORD l = 0; l < head.image_size; l++) {
			buf[l] = fgetc(in);
		}
		GlobalUnlock(block.hCut);
	}
			
	fclose(in);

	work->block->SetBlock((BYTE*)&block, work->timerpos);

	hdc = GetDC(hwnd);
	SelectPalette(hdc, work->hPal, FALSE);
	GetAlbumPosition(work, work->timerpos, x, y);
	DrawOneImage(hdc, work, x, y, work->timerpos);
	ReleaseDC(hwnd, hdc);

	work->timerpos++;
		
	GlobalUnlock(hGlobal);
}

static void SaveInfoFile(HWND hwnd, char *filename, char *keyword, HGLOBAL hCut)
{
	FILE *out;
	INFO_FILE_HEADER head;
	int width, height;
	int color;
	int pal_size;
	BYTE *buf;

	buf = (BYTE*) GlobalLock(hCut);

	width =  GetDibWidth (buf);
	height = GetDibHeight(buf);
	color = ((BITMAPINFOHEADER*)buf)->biBitCount;

	switch(color) {
		case 1:	pal_size = 2;		break;
		case 4:	pal_size = 16;		break;
		case 8:	pal_size = 256;	break;
		default: pal_size = 0;		break;
	}
	
	head.struct_size = sizeof(INFO_FILE_HEADER);
	head.version = 1;
	strcpy(head.keyword, keyword);
	head.image_size = (long)sizeof(BITMAPINFOHEADER)+sizeof(RGBQUAD)*pal_size+(long)BmpWidthToByte(width, color)*height;

	out = fopen(filename, "wb");
	if(out != NULL) {
		fwrite(&head, 1, sizeof(INFO_FILE_HEADER), out);
	
		for(DWORD l = 0; l < head.image_size; l++) {
			fputc(buf[l], out);
		}

		fclose(out);
		GlobalUnlock(hCut);
	}
	else {
		GlobalUnlock(hCut);
#if	defined (COMPILE_ENGLISH)
		MessageBox(hwnd, "Can't make clip information file.\nCheck write protect device", filename, MB_OK);
#else
		MessageBox(hwnd, "요약 파일을 만들 수 없습니다.\n쓰기 금지 되어 있는 장치", filename, MB_OK);
#endif
	}
}

static void GetFitSize(int sx, int sy, int &tx, int &ty)
{
	if(sx <= 100 && sy <= 100) {
		tx = sx;
		ty = sy;
		return;
	}

	if(sx > sy) {
		tx = 100;
		ty = (long)sy*tx/sx;
	}
	else {
		ty = 100;
		tx = (long)sx*ty/sy;
	}
}

static HGLOBAL GetFileToCut(HWND hwnd, char *filename)
{
	HGLOBAL hSource;
	HGLOBAL hTarget;
	int o_width, o_height;
	int t_width, t_height;
	int org_line;
	BYTE *o_buf, *t_buf;
	long o_oneline, t_oneline;
	int color;
	int pal_size;
	int head_size;
	opticBuf optic;
	int y;

	hSource = PictureToolLoadToBufDIB(hwnd, filename);

	if(hSource == NULL) {
		return NULL;
	}

	o_buf = (BYTE*)GlobalLock(hSource);

	o_width =  GetDibWidth (o_buf);
	o_height = GetDibHeight (o_buf);
	color = ((BITMAPINFOHEADER*)o_buf)->biBitCount;

	switch(color) {
		case 1:	pal_size = 2;		break;
		case 4:	pal_size = 16;		break;
		case 8:	pal_size = 256;	break;
		default: pal_size = 0;		break;
	}

	GetFitSize(o_width, o_height, t_width, t_height);

	o_oneline = BmpWidthToByte(o_width, color);
	t_oneline = BmpWidthToByte(t_width, color);

	head_size = sizeof(BITMAPINFOHEADER)+pal_size*sizeof(RGBQUAD);

	hTarget = GlobalAlloc(GMEM_MOVEABLE, head_size+(long)t_oneline*t_height);

	if(hTarget == NULL) {
		GlobalUnlock(hSource);
		GlobalFree(hSource);
#if	defined (COMPILE_ENGLISH)
		MessageBox(hwnd, "Can't read picture because memory insufficent.", filename, MB_OK);
#else
		MessageBox(hwnd, "메모리 부족으로 그림을 읽을 수 없습니다.", filename, MB_OK);
#endif
		return NULL;
	}

	t_buf = (BYTE*) GlobalLock(hTarget);

	memcpy(t_buf, o_buf, head_size);

	((BITMAPINFOHEADER*)t_buf)->biWidth = t_width;
	((BITMAPINFOHEADER*)t_buf)->biHeight = t_height;
	
	optic.Init(t_width, o_width, color);

	for(y = 0; y < t_height; y++) {
		org_line	= (long)o_height*y/t_height;
		
		optic.OneLine(t_buf+head_size+(long)t_oneline*y, o_buf+head_size+(long)o_oneline*org_line);	
	}

	GlobalUnlock(hSource);
	GlobalUnlock(hTarget);

	GlobalFree(hSource);

	return hTarget;
}

static void MakeThumbnailOne(HWND hwnd, FILE_STRUCT *block)
{
	HGLOBAL hCut;
	char infofile[MAXPATH];
			
	hCut = GetFileToCut(hwnd, block->filename);

	if(hCut == NULL)	return;

	if(block->hCut != NULL) {	
		GlobalFree(block->hCut);
		block->hCut = NULL;
	}

	block->hCut = hCut;

	strcpy(infofile, block->filename);
	infofile[strlen(infofile)-1] = '_';

	SaveInfoFile(hwnd, infofile, block->keyword, block->hCut);
}

static void MakeThumbnail(HWND hwnd)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	FILE_STRUCT block;
	DWORD l;
	HDC hdc;
	int drawx, drawy;
		
	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	if(work->block->GetBlockCount() == 0)	{
		GlobalUnlock(hGlobal);
		return;
	}

	int select = 0;

	for(l = 0; l < work->block->GetBlockCount(); l++) {
		work->block->GetBlock((BYTE*)&block, l);
		if(block.select)	select++;
	}

	if(select) {
		for(l = 0; l < work->block->GetBlockCount(); l++) {	
			work->block->GetBlock((BYTE*)&block, l);
			if(block.select)	{
				MakeThumbnailOne(hwnd, &block);
				block.select = OFF;
				work->block->SetBlock((BYTE*)&block, l);

				hdc = GetDC(hwnd);
				SelectPalette(hdc, work->hPal, FALSE);
				GetAlbumPosition(work, l, drawx, drawy);
				DrawOneImage(hdc, work, drawx, drawy, l);			
				ReleaseDC(hwnd, hdc);
			}
		}
	}
	else {
		work->block->GetBlock((BYTE*)&block, work->cursor);
		MakeThumbnailOne(hwnd, &block);
		work->block->SetBlock((BYTE*)&block, work->cursor);
		
		hdc = GetDC(hwnd);
		SelectPalette(hdc, work->hPal, FALSE);
		GetAlbumPosition(work, work->cursor, drawx, drawy);
		DrawOneImage(hdc, work, drawx, drawy, work->cursor);	
		ReleaseDC(hwnd, hdc);
	}
			
	GlobalUnlock(hGlobal);
}

static void AddOneAlbum(HWND hwnd, LPARAM lParam)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	FILE_STRUCT block;
	FnSplit fnsplit;
	int scrollhap;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	memset(&block, 0, sizeof(FILE_STRUCT));
	strcpy(block.filename, (char *)lParam);

	fnsplit.fnsplit((char *)lParam);
	fnsplit.GetNameExt(block.title);

	work->block->AddBlock((BYTE*)&block);

	scrollhap = (work->block->GetBlockCount()+work->limitx-1)/work->limitx;

	if(scrollhap != work->nScrollHap) {
		work->nScrollHap = scrollhap;

		if(work->nScrollHap > work->limity) {
			ShowScrollBar(hwnd, SB_VERT, TRUE);
			work->bScrollBar = ON;
			SetScrollRange(hwnd, SB_VERT, 0, work->nScrollHap-1, TRUE);
			SetScrollPos(hwnd, SB_VERT, work->nScrollPos, TRUE);
		}
	}
	
	GlobalUnlock(hGlobal);
}

static void ResetAlbum(HWND hwnd)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	FILE_STRUCT block;
	DWORD l;
		
	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	for(l = 0; l < work->block->GetBlockCount(); l++) {
		work->block->GetBlock((BYTE*)&block, l);
		if(block.hCut) {
			GlobalFree(block.hCut);
			block.hCut = NULL;
		}
		work->block->SetBlock((BYTE*)&block, l);
	}

	work->block->DeleteAllBlock();
	work->timerpos = 0;
	work->nScrollPos = 0;
	work->nScrollHap = 1;
	work->bScrollBar = OFF;
	work->cursor = 0;
	ShowScrollBar(hwnd, SB_VERT, FALSE);
	
	GlobalUnlock(hGlobal);

	InvalidateRect(hwnd, NULL, FALSE);
}

static void EditSelectAll(HWND hwnd, char flag)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	FILE_STRUCT block;
	DWORD l;
		
	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	for(l = 0; l < work->block->GetBlockCount(); l++) {
		work->block->GetBlock((BYTE*)&block, l);
		if(flag == 0) {
			block.select = OFF;
		}
		else if(flag == 1) {
			block.select = ON;
		}
		else {
			block.select ++;
			block.select %= 2;
		}
		work->block->SetBlock((BYTE*)&block, l);
	}
	
	GlobalUnlock(hGlobal);

	InvalidateRect(hwnd, NULL, TRUE);
}

/*
static char tempKeyWord[80];

class MakeKeyWordDialog : public KDialog {
	public:
		MakeKeyWordDialog()  {}
		~MakeKeyWordDialog() {}
		BOOL WmInitDialog();
		BOOL WmCommand();
};

BOOL MakeKeyWordDialog :: WmInitDialog()
{
	SetWindowText(GetDlgItem(IDC_MakeKeyWord_EDIT_KEYWORD), tempKeyWord);
	SetFocus(GetDlgItem(IDC_MakeKeyWord_EDIT_KEYWORD));

	return FALSE;
}

BOOL MakeKeyWordDialog :: WmCommand()
{
	switch(wParamThis) {
		case IDOK:
			GetWindowText(GetDlgItem(IDC_MakeKeyWord_EDIT_KEYWORD), tempKeyWord, sizeof(tempKeyWord));
			EndDialog(1);
			return TRUE;
		case IDCANCEL:
			EndDialog(0);
	}
	
	return FALSE;
}

static void AddKeyWordToBuf(char *buf, char *keyword)
{
	Block block(80);
	char  string[80];
	char  compare[80];
	CommaBlockString comma;
	DWORD l;

	comma.Set(buf);

	while(1) {
		next:	
		comma.GetString(string, sizeof(string));
		if(string[0] == 0)	break;
		for(l = 0; l < block.GetBlockCount(); l++) {
			block.GetBlock((BYTE*)compare, l);
			if(strcmp(string, compare) == 0)	goto next;		// same word detected
		}
		block.AddBlock((BYTE*)string);
	}

	comma.Set(keyword);

	while(1) {
		next2:	
		comma.GetString(string, sizeof(string));
		if(string[0] == 0)	break;
		for(l = 0; l < block.GetBlockCount(); l++) {
			block.GetBlock((BYTE*)compare, l);
			if(strcmp(string, compare) == 0)	goto next2;		// same word detected
		}
		block.AddBlock((BYTE*)string);
	}

	buf[0] = 0;
	for(l = 0; l < block.GetBlockCount(); l++) {
		block.GetBlock((BYTE*)compare, l);
		strcat(buf, compare);
		strcat(buf, ",");
		if(strlen(buf) >= 80)	break;
	}
	buf[79] = 0;
}

static void MakeKeyWord(HWND hwnd)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	FILE_STRUCT block;
	MakeKeyWordDialog dialog;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);
	work->block->GetBlock((BYTE*)&block, work->cursor);
	strcpy(tempKeyWord, block.keyword);
	GlobalUnlock(hGlobal);

	if(!dialog.run(hwnd, IDD_MAKE_KEYWORD, hInst))	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);
	work->block->GetBlock((BYTE*)&block, work->cursor);
	block.keyword[0] = 0;
	AddKeyWordToBuf(block.keyword, tempKeyWord);
	//strcpy(block.keyword, tempKeyWord);
	MakeThumbnailOne(hwnd, &block);
	work->block->SetBlock((BYTE*)&block, work->cursor);
	GlobalUnlock(hGlobal);
}

class InsertKeyWordDialog : public KDialog {
	public:
		InsertKeyWordDialog()  {}
		~InsertKeyWordDialog() {}
		BOOL WmInitDialog();
		BOOL WmCommand();
};

BOOL InsertKeyWordDialog :: WmInitDialog()
{
	SetFocus(GetDlgItem(IDC_InsertKeyWord_EDIT_KEYWORD));

	return FALSE;
}

BOOL InsertKeyWordDialog :: WmCommand()
{
	switch(wParamThis) {
		case IDOK:
			GetWindowText(GetDlgItem(IDC_InsertKeyWord_EDIT_KEYWORD), tempKeyWord, sizeof(tempKeyWord));
			EndDialog(1);
			return TRUE;
		case IDCANCEL:
			EndDialog(0);
	}
	
	return FALSE;
}


static void InsertKeyWord(HWND hwnd)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	FILE_STRUCT block;
	InsertKeyWordDialog dialog;
	char buf[160];
	DWORD l;

	if(!dialog.run(hwnd, IDD_INSERT_KEYWORD, hInst))	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);
	for(l = 0; l < work->block->GetBlockCount(); l++) {
		work->block->GetBlock((BYTE*)&block, l);
		if(block.select) {
			strcpy(buf, block.keyword);
			AddKeyWordToBuf(buf, tempKeyWord);
			strcpy(block.keyword, buf);
			MakeThumbnailOne(hwnd, &block);
			work->block->SetBlock((BYTE*)&block, l);
		}
	}
	GlobalUnlock(hGlobal);
}

static void IdmCopy(HWND hwnd)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	FILE_STRUCT block;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);

	if(work->block->GetBlockCount() == 0) {	
		GlobalUnlock(hGlobal);
		MessageBox(hwnd, "클립보드로 복사할 수 있는\n그림이 하나도 없습니다.", "복사 오류", MB_OK);
		return;
	}
	work->block->GetBlock((BYTE*)&block, work->cursor);
	GlobalUnlock(hGlobal);

	HGLOBAL hGlobalImage;
	HCURSOR hCursorOld;

	hCursorOld = SetCursor(LoadCursor(NULL, IDC_WAIT));
	hGlobalImage = PictureToolLoadToBufDIB(hwnd, block.filename);
	if(hGlobalImage == NULL) {
		SetCursor(hCursorOld);
		return;
	}

	if(!OpenClipboard(hwnd)) {
		SetCursor(hCursorOld);
		GlobalFree(hGlobalImage);
		MessageBox(hwnd, "클립보드를 열 수 없습니다.", "클립보드 오류", MB_OK);
		return;
	}

	EmptyClipboard();
	SetClipboardData(CF_DIB, hGlobalImage);
	CloseClipboard();

	SetCursor(hCursorOld);
}

static void WmCommand(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	switch(LOWORD(wParam)) {
		
		case IDM_COPY:
			IdmCopy(hwnd);
			break;
		case IDM_EDIT_IMAGE:
			IdmEditImage(hwnd);
			break;
		case IDM_EVENT_TIMER:
			IdmEventTimer(hwnd);
			break;
		case IDM_MAKE_THUMBNAIL:
			MakeThumbnail(hwnd);
			break;
		case IDM_MSG_ALBUM_ADD_ITEM:
			AddOneAlbum(hwnd, lParam);
			break;
		case IDM_MSG_ALBUM_RESET:
			ResetAlbum(hwnd);
			break;
		case IDM_EDIT_SELECT_ALL:
			EditSelectAll(hwnd, 1);
			break;
		case IDM_EDIT_UNSELECT_ALL:
			EditSelectAll(hwnd, 1);
			break;
		case IDM_EDIT_SELECT_CHANGE:
			EditSelectAll(hwnd, 2);
			break;
		case IDM_MAKE_KEYWORD:
			MakeKeyWord(hwnd);
			break;
		case IDM_INSERT_KEYWORD:
			InsertKeyWord(hwnd);
			break;
		
		default:
			break;
	}
}
*/

static void WmKeyDown(HWND hwnd, WPARAM wParam)
{
	switch(wParam) {
		case VK_LEFT:
		case VK_UP:
			bell();
			break;
		case VK_RIGHT:
		case VK_DOWN:
			bell();
			break;
	}
}

static void WmVScroll(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	HGLOBAL hGlobal;
	ALBUM_WINDOW_STRUCT *work;	

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*)GlobalLock(hGlobal);
	
	switch(LOWORD(wParam)) {
		case SB_LINEUP:
			if(work->nScrollPos == 0)	break;
			work->nScrollPos--;
			SetScrollPos(hwnd, SB_VERT, work->nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, FALSE);
			break;
		case SB_LINEDOWN:
			if(work->nScrollPos >= work->nScrollHap-1)	break;
			work->nScrollPos++;
			SetScrollPos(hwnd, SB_VERT, work->nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, FALSE);
			break;
		case SB_PAGEUP:
			if(work->nScrollPos <= 0)	break;
			work->nScrollPos-=work->limity;
			if(work->nScrollPos < 0)	work->nScrollPos = 0;
			SetScrollPos(hwnd, SB_VERT, work->nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, FALSE);
			break;
		case SB_PAGEDOWN:
			if(work->nScrollPos >= work->nScrollHap-1)	break;
			work->nScrollPos+=work->limity;
			if(work->nScrollPos > work->nScrollHap-1) {
				work->nScrollPos = work->nScrollHap-1;	
			}
			SetScrollPos(hwnd, SB_VERT, work->nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, FALSE);
			break;
		case SB_THUMBPOSITION:
			work->nScrollPos = HIWORD(wParam);;
			if(work->nScrollPos > work->nScrollHap-1) {
				work->nScrollPos = work->nScrollHap-1;	
			}
			SetScrollPos(hwnd, SB_VERT, work->nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, FALSE);
			break;
	}

	GlobalUnlock(hGlobal);
}

static void WmDestroy(HWND hwnd) 
{
	HGLOBAL		    hGlobal;
	ALBUM_WINDOW_STRUCT     *work;
	DWORD l;
	FILE_STRUCT block;

	hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0);
	if(hGlobal) {
		work = (ALBUM_WINDOW_STRUCT*) GlobalLock (hGlobal);

		DeleteObject(work->hPal);

		if(work->block) {
			for(l = 0; l < work->block->GetBlockCount(); l++) {
				work->block->GetBlock((BYTE*)&block, l);
				if(block.hCut) {
					GlobalFree(block.hCut);
				}
			}
			delete work->block;
		}
		
		GlobalUnlock (hGlobal);
		GlobalFree (hGlobal);
	}
}

static LRESULT LbGetCurSel(HWND hwnd)
{
	HGLOBAL		    hGlobal;
	ALBUM_WINDOW_STRUCT *work;
	LRESULT retn = LB_ERR;
	
	hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*) GlobalLock (hGlobal);
	if(work->block->GetBlockCount() > 0) {
		retn = work->cursor;
	}
	GlobalUnlock (hGlobal);

	return retn;
}

static LRESULT LbGetText(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	HGLOBAL		    hGlobal;
	ALBUM_WINDOW_STRUCT     *work;
	LRESULT retn = LB_ERR;

	hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*) GlobalLock (hGlobal);
	if(wParam < work->block->GetBlockCount()) {
		FILE_STRUCT block;
		work->block->GetBlock((BYTE*)&block, wParam);
		strcpy((char*)lParam, block.filename);
		retn = 0;
	}
	GlobalUnlock (hGlobal);

	return retn;
}

static LRESULT LbDeleteString(HWND hwnd, WPARAM wParam)
{
	HGLOBAL		    hGlobal;
	ALBUM_WINDOW_STRUCT     *work;
	LRESULT retn = LB_ERR;

	hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*) GlobalLock (hGlobal);
	if(wParam < work->block->GetBlockCount()) {
		work->block->DeleteBlock(wParam);
		retn = 0;
		InvalidateRect(hwnd, NULL, FALSE);
	}
	GlobalUnlock (hGlobal);

	return retn;
}

static LRESULT LbSelectString(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	HGLOBAL		    hGlobal;
	ALBUM_WINDOW_STRUCT     *work;
	LRESULT retn = LB_ERR;
	FILE_STRUCT block;
	DWORD l;

	hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0);
	work = (ALBUM_WINDOW_STRUCT*) GlobalLock (hGlobal);
	for(l = 0; l < work->block->GetBlockCount(); l++) {
		work->block->GetBlock((BYTE*)&block, l);
		if(strcmp((char*)lParam, block.title) == 0) {
			work->cursor = l;
			work->nScrollPos = work->cursor/work->limitx;
			SetScrollPos(hwnd, SB_VERT, work->nScrollPos, TRUE);
			retn = 0;
			break;
		}
	}
	GlobalUnlock (hGlobal);

	if(retn == 0) {
		InvalidateRect(hwnd, NULL, FALSE);
	}

	return retn;
}

LRESULT CALLBACK WndProcClipViewAlbum(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message) {
		case WM_CREATE:
			SetTimer(hwnd, 0, 0, NULL);
			WmCreate(hwnd, lParam);
			return 0;
		case LB_ADDSTRING:
			AddOneAlbum(hwnd, lParam);
			return 0;
		case LB_RESETCONTENT:
			ResetAlbum(hwnd);
			return 0;
		case LB_GETCURSEL:
			return LbGetCurSel(hwnd);
		case LB_GETTEXT:
			return LbGetText(hwnd, wParam, lParam);
		case WM_TIMER:
			WmTimer(hwnd);
			return 0;
		case LB_ADDFILE:
			MakeThumbnail(hwnd);
			return 0;
		case LB_SELITEMRANGE:
			EditSelectAll(hwnd, wParam);
			return 0;		
		case LB_DELETESTRING:
			return LbDeleteString(hwnd, wParam);
		case LB_SELECTSTRING:
			return LbSelectString(hwnd, wParam, lParam);

		case WM_SIZE:
			WmSize(hwnd);
			break;
		case WM_VSCROLL:
			WmVScroll(hwnd, wParam, lParam);
			return 0;
		case WM_PAINT:
			WmPaint(hwnd);
			return 0;
		case WM_LBUTTONDOWN:
			WmButtonDown(hwnd, lParam, 0, 0);
			return 0;
		case WM_LBUTTONDBLCLK:
			WmButtonDown(hwnd, lParam, 0, 1);
			return 0;
		case WM_RBUTTONDOWN:
			WmButtonDown(hwnd, lParam, 1, 0);
			return 0;
		case WM_KEYDOWN:
			WmKeyDown(hwnd, wParam);
			return 0;
		case WM_DESTROY:
			WmDestroy(hwnd);
			KillTimer(hwnd, 0);
			return 0;
	}
	// Pass unprocessed message to DefMDIChildProc

	return DefWindowProc (hwnd, message, wParam, lParam);
}

void ClipViewRegisterClassAlbum(HINSTANCE hinstance)
{
	static char install_flag = OFF;
	if(install_flag == ON)	return;		// already register

	WNDCLASS wndclass;
	
	// Register the  Alnum window class

	wndclass.style         = CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS;
	wndclass.lpfnWndProc   = WndProcClipViewAlbum;
	wndclass.cbClsExtra    = 0 ;
	wndclass.cbWndExtra    = sizeof (HGLOBAL);
	wndclass.hInstance     = hinstance ;
	wndclass.hIcon         = LoadIcon (NULL, IDI_APPLICATION) ;
	wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW) ;
	wndclass.hbrBackground = (HBRUSH)GetStockObject(NULL_BRUSH);
	wndclass.lpszMenuName  = NULL ;
	wndclass.lpszClassName = szClassNameClipViewAlbum;

	RegisterClass (&wndclass) ;

	install_flag = ON;
}



 
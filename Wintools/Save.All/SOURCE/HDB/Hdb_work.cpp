// english O.K
// HDB Edit에 필요한 도구들
#include "stdafx.h"
#include <compiler.hpp>
#include <stdlib.h>
#include <commdlg.h>
#include <string.h>

#include <tools.h>
#include <gclass.h>
#include <glib.h>
#include <dataswap.h>
#include <hdb_work.h>

// static HWND hwndEditCell = NULL;		// 각 셀을 입력할 수 있는 입력기.

static void ScrollBarUpdate(HWND hwnd, HDB_WORK_STRUCT *work)
{
	SetScrollRange(hwnd, SB_HORZ, 0, work->sheet->blockField->GetCount(), FALSE);
	SetScrollRange(hwnd, SB_VERT, 0, work->sheet->dwDataHap, FALSE);

	SetScrollPos(hwnd, SB_HORZ, work->dwPageX, TRUE);
	SetScrollPos(hwnd, SB_VERT, work->dwPageY, TRUE);
}

/*
void GetFontInformation(HWND hwnd, WORK_EDIT_SHEET *work);
void WindowSizeUpdate(HWND hwnd);
void SaveEditedCell(WORK_EDIT_SHEET *work);
void ActiveEditWindow(WORK_EDIT_SHEET *work);

void DeleteOneField(HWND hwnd)
{
	char message[100];
	int  field_pos;
	HDB_FIELD_STRUCT field;
	HLOCAL hWork;
	WORK_EDIT_SHEET *work;
	char sheet_name[11];

	hWork = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_EDIT_SHEET*)LocalLock(hWork);
	field_pos = work->nCurrField;
	strcpy(sheet_name, work->sSheetName);
	LocalUnlock(hWork);

	if(HDBGetFieldHap(sheet_name) == 0) {
		MessageBox(hwnd, "삭제할 수 있는 필드가 하나도 없습니다.", "삭제 오류", MB_OK);
		return;
	}

	if(!HDBGetField(sheet_name, field_pos, &field)) {
		MessageBox(hwnd, "삭제할 필드를 선택한 후\n필드를 삭제할 수 있습니다.", "삭제오류", MB_OK);
		return;
	}

	sprintf(message, "[%s] 필드를 삭제 하겠습니까?", field.sName);
	if(MessageBox(hwnd, message, "필드삭제", MB_YESNO) != IDYES) 	return;

	HDBDeleteField(sheet_name, field_pos);

	InvalidateRect(hwnd, NULL, TRUE);
}

void ChangeFieldFunction(HWND hwnd)
{
	int  field_pos;
	HDB_FIELD_STRUCT field;
	HLOCAL hWork;
	WORK_EDIT_SHEET *work;
	char sheet_name[11];

	hWork = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_EDIT_SHEET*)LocalLock(hWork);
	field_pos = work->nCurrField;
	strcpy(sheet_name, work->sSheetName);
	LocalUnlock(hWork);

	if(HDBGetFieldHap(sheet_name) == 0) {
		MessageBox(hwnd, "수정할 수 있는 필드가 하나도 없습니다.", "수정 오류", MB_OK);
		return;
	}

	if(!HDBGetField(sheet_name, field_pos, &field)) {
		MessageBox(hwnd, "수정할 필드를 선택한 후\n필드를 수정할 수 있습니다.", "수정오류", MB_OK);
		return;
	}

	if(field.wType != HDB_FIELD_TYPE_FUNCTION) {
		MessageBox(hwnd, "계산식 필드가 아니므로 수식을 수정할 수 없습니다.", "수정오류", MB_OK);
		return;
	}

	StackChar stack(1000);

	if(stack.data == NULL) {
		MessageBox(hwnd, "메모리 부족", "수정오류", MB_OK);
		return;
	}

	HDBGetFieldFunction(sheet_name, field_pos, stack.data);


	if(CommDialogInputString(hwnd, "계산식을 입력하세요." , stack.data, 1000)) {
		hWork = (HGLOBAL)GetWindowLong(hwnd, 0);
		work = (WORK_EDIT_SHEET*)LocalLock(hWork);
		SaveEditedCell(work);
		HDBSetFieldFunction(sheet_name, field_pos, stack.data);
		ActiveEditWindow(work);
		LocalUnlock(hWork);
		InvalidateRect(hwnd, NULL, TRUE);
	}

	InvalidateRect(hwnd, NULL, TRUE);
}

void FindField(HWND hwnd)
{
	int  field_pos;
	HLOCAL hWork;
	WORK_EDIT_SHEET *work;

	StackChar stack(1000);

	if(stack.data == NULL) {
		MessageBox(hwnd, "메모리 부족", "찾기오류", MB_OK);
		return;
	}

	memset(stack.data, 0, 1000);

	hWork = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_EDIT_SHEET*)LocalLock(hWork);

  continue_seek:
	if(!CommDialogInputString(hwnd, "찾고 싶은 필드명을 입력하세요." , stack.data, 1000)) {
		LocalUnlock(hWork);
		return;
	}

	field_pos = HDBSearchField(work->sSheetName, stack.data);
	if(field_pos == -1) {
		MessageBox(hwnd, "찾는 필드가 존재하지 않습니다.", stack.data, MB_OK);
		goto continue_seek;
	}

	SaveEditedCell(work);
	work->nCurrField = field_pos;
	work->nPageField = field_pos;
	ActiveEditWindow(work);
	LocalUnlock(hWork);
	InvalidateRect(hwnd, NULL, TRUE);
}

void PasteToRightDirection(HWND hwnd)
{
	if(!IsClipboardFormatAvailable(CF_TEXT))	{
		return;
	}

	HANDLE hClipBoard;
	char huge *text;
	HLOCAL hWork;
	WORK_EDIT_SHEET *work;
	StackChar stack(1000);
	DWORD dw;
	DWORD block_pos;
	int   field_pos;
	int   hap;

	if(stack.data == NULL)	return;

	OpenClipboard(hwnd);
	hClipBoard = GetClipboardData(CF_TEXT);
	if(hClipBoard == NULL) {
		CloseClipboard();
		return;
	}

	text = (char huge*)GlobalLock(hClipBoard);

	hWork = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_EDIT_SHEET*)LocalLock(hWork);
	SaveEditedCell(work);

	field_pos = work->nCurrField;
	block_pos = work->dwCurrBlock;

	hap = 0;
	dw = 0;
	while(1) {
		if(text[dw] == NULL) {
			stack.data[hap] = 0;
			if(hap > 0) {
				HDBSetCell(work->sSheetName, field_pos, block_pos, stack.data);
			}
			break;
		}
		else if(text[dw] == '\r') {
			stack.data[hap] = 0;
			HDBSetCell(work->sSheetName, field_pos, block_pos, stack.data);
			field_pos++;
			hap = 0;
			if(field_pos >= HDBGetFieldHap(work->sSheetName))	break;
		}
		else if(text[dw] == '\n') {

		}
		else {
			if(hap < 900) {
				stack.data[hap] = text[dw];
				hap++;
			}
		}
		dw++;
	}

	ActiveEditWindow(work);
	LocalUnlock(hWork);

	GlobalUnlock(hClipBoard);
	CloseClipboard();

	InvalidateRect(hwnd, NULL, TRUE);
}

void PasteToDownDirection(HWND hwnd)
{
	if(!IsClipboardFormatAvailable(CF_TEXT))	{
		return;
	}

	HANDLE hClipBoard;
	char huge *text;
	HLOCAL hWork;
	WORK_EDIT_SHEET *work;
	StackChar stack(1000);
	DWORD dw;
	DWORD block_pos;
	int   field_pos;
	int   hap;

	if(stack.data == NULL)	return;

	OpenClipboard(hwnd);
	hClipBoard = GetClipboardData(CF_TEXT);
	if(hClipBoard == NULL) {
		CloseClipboard();
		return;
	}

	text = (char huge*)GlobalLock(hClipBoard);

	hWork = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_EDIT_SHEET*)LocalLock(hWork);
	SaveEditedCell(work);

	field_pos = work->nCurrField;
	block_pos = work->dwCurrBlock;

	hap = 0;
	dw = 0;
	while(1) {
		if(text[dw] == NULL) {
			stack.data[hap] = 0;
			if(hap > 0) {
				HDBSetCell(work->sSheetName, field_pos, block_pos, stack.data);
			}
			break;
		}
		else if(text[dw] == '\r') {
			stack.data[hap] = 0;
			HDBSetCell(work->sSheetName, field_pos, block_pos, stack.data);
			block_pos++;
			hap = 0;
			if(block_pos >= HDBGetBlockHap(work->sSheetName))	break;
		}
		else if(text[dw] == '\n') {

		}
		else {
			if(hap < 900) {
				stack.data[hap] = text[dw];
				hap++;
			}
		}
		dw++;
	}

	ActiveEditWindow(work);
	LocalUnlock(hWork);

	GlobalUnlock(hClipBoard);
	CloseClipboard();

	InvalidateRect(hwnd, NULL, TRUE);
}

static void SelectAlwaysViewField(HWND hwnd)
{
	HLOCAL hWork;
	WORK_EDIT_SHEET *work;
	HDB_FIELD_STRUCT field;

	hWork = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_EDIT_SHEET*)LocalLock(hWork);

	if(HDBGetFieldHap(work->sSheetName) == 0) {
		return;
	}

	HDBGetField(work->sSheetName, work->nCurrField, &field);
	HDBSetAlwaysViewField(work->sSheetName, field.sName);

	SaveEditedCell(work);
	ActiveEditWindow(work);
	LocalUnlock(hWork);
	InvalidateRect(hwnd, NULL, TRUE);
}

static int CheckWorkSheetName(HWND hwnd, LPARAM lParam)
{
	HLOCAL hWork;
	WORK_EDIT_SHEET *work;
	int retn;

	hWork = (HGLOBAL)GetWindowLong(hwnd, 0);
	work = (WORK_EDIT_SHEET*)LocalLock(hWork);
	if(strcmp((LPSTR)lParam, work->sSheetName) == 0)
		retn = 1;
	else
		retn = 0;
	LocalUnlock(hWork);

	return retn;
}

*/

static void Not(HDC hdc, HDB_WORK_STRUCT *work) 
{
	RECT r;
	int x, y;
	DWORD dwField, dwBlock;
	DWORD hap_field  = HDBGetFieldHap(work->sheet);
	DWORD hap_block  = HDBGetBlockHap(work->sheet);
	HDB_FIELD_STRUCT field;
	int width;
	DWORD selx1, sely1, selx2, sely2;
	
	selx1 = work->selx1;
	sely1 = work->sely1;
	selx2 = work->selx2;
	sely2 = work->sely2;

	if(selx1 > selx2)	Temp(selx1, selx2);
	if(sely1 > sely2)	Temp(sely1, sely2);

	y = work->pyStart;

	for(dwBlock = work->dwPageY; y < work->rect.bottom; dwBlock++, y+=work->cyChar+1) {
		if(dwBlock < sely1)	continue;
		if(dwBlock > sely2)	break;

		x = work->pxStart;

		for(dwField = work->dwPageX; dwField < hap_field && x < work->rect.right; dwField++) {
			if(dwField > selx2)	break;

			HDBGetField(work->sheet, dwField, &field);
			width = field.wViewSize;

			if(dwField < selx1) {

			}
			else {
				r.left = x;
				r.top  = y;
				r.right = x+width;
				r.bottom = y+work->cyChar;

				InvertRect(hdc, &r);
			}

			x+= width+1;
		}
	}
}

static void Select(HDB_WORK_STRUCT *work, int x1, int y1, int x2, int y2)
{
	work->selx1 = x1;
	work->sely1 = y1;
	work->selx2 = x2;
	work->sely2 = y2;
}

static void Select(HDB_WORK_STRUCT *work, int x, int y)
{	
	Select(work, x, y, x, y);
}

void HDBWorkWmPaint(HWND hwnd, HDC hdc, HDB_WORK_STRUCT *work)	
{
	DWORD hap_field;
	DWORD hap_block;
	DWORD dwField, dwBlock;
	int	x, y;
	HDB_FIELD_STRUCT field;
	int   width;
	TEXTMETRIC tm;
	RECT	rect;
	char	buf[80];
	char  buf2[80];
	HPEN hPen, hPenOld;
	COLORREF tcolor, bcolor;
	RECT  draw_rect;
	int   pos_AlwaysViewField;

	hPen = CreatePen(PS_SOLID, 1, WHITE_GRAY_COLOR);

	GetClientRect(hwnd, &rect);

	hap_field  = HDBGetFieldHap(work->sheet);
	hap_block  = HDBGetBlockHap(work->sheet);

	HDBGetAlwaysViewField(work->sheet, buf);
	pos_AlwaysViewField = HDBSearchField(work->sheet, buf);

	//hOldFont = SelectObject(hdc, hViewFont);

	hPenOld = (HPEN)SelectObject(hdc, hPen);
	GetTextMetrics(hdc, &tm);

	PopBox2(hdc, 0, 0, work->pxStart-2, work->pyStart-2, WHITE_GRAY_COLOR);
	MoveToEx(hdc, 0, work->pyStart-1, NULL);
	LineTo(hdc, rect.right, work->pyStart-1);
	MoveToEx(hdc, work->pxStart-1, 0, NULL);
	LineTo(hdc, work->pxStart-1, rect.bottom);

	SetTextColor(hdc, DARK_COLOR);
	SetBkColor(hdc, WHITE_GRAY_COLOR);

	x = work->pxStart;

	for(dwField = work->dwPageX; dwField < hap_field && x < rect.right; dwField++) {
		HDBGetField(work->sheet, dwField, &field);
		width = field.wViewSize;
		gcls(hdc, x, 0,x+width-1, work->pyStart-2, WHITE_GRAY_COLOR);
		draw_rect.left = x+1;
		draw_rect.top = 0;
		draw_rect.right = x+width;
		draw_rect.bottom = work->pyStart-1;
		DrawText(hdc, field.sName, strlen(field.sName), &draw_rect, DT_CENTER | DT_WORDBREAK);
		//TextOut(hdc, x, 0, field.sName, strlen(field.sName));
		PopRectangle2(hdc, x, 0, x+width-1, work->pyStart-2);
		MoveToEx(hdc, x+width, 0, NULL);
		LineTo(hdc, x+width, rect.bottom);
		x+= width+1;
	}

	if(x < rect.right) {
		gcls(hdc, x, 0, rect.right, work->pyStart-2, DARK_GRAY_COLOR);
	}

	y = work->pyStart;

	for(dwBlock = work->dwPageY; y < rect.bottom && dwBlock < hap_block; dwBlock++) {
		SetTextColor(hdc, DARK_COLOR);
		SetBkColor(hdc, WHITE_GRAY_COLOR);

		gcls(hdc, 0, y, work->pxStart-1, y+work->cyChar-1, WHITE_GRAY_COLOR);

		if(dwBlock < hap_block) {
			if(pos_AlwaysViewField == -1) {
				sprintf(buf, "%5ld", dwBlock+1);
			}
			else {
				HDBGetCell(work->sheet, pos_AlwaysViewField, dwBlock, buf2, sizeof(buf2));
				sprintf(buf, "%5ld (%s)", dwBlock+1, buf2);
			}

			draw_rect.left = work->cxChar;
			draw_rect.top = y+1;
			draw_rect.right = work->pxStart-3;
			draw_rect.bottom = y+work->cyChar;
			DrawText(hdc, buf, strlen(buf), &draw_rect, DT_LEFT | DT_VCENTER | DT_SINGLELINE);
		}

		PopRectangle2(hdc, 0, y, work->pxStart-2, y+work->cyChar-1);

		MoveToEx(hdc, 0, y+work->cyChar, NULL);
		LineTo(hdc, rect.right, y+work->cyChar);
		x = work->pxStart;

		for(dwField = work->dwPageX; dwField < hap_field && x < rect.right; dwField++) {
			HDBGetField(work->sheet, dwField, &field);

			if(field.wType == HDB_FIELD_TYPE_FUNCTION) {
				tcolor = DARK_GRAY_COLOR;
				bcolor = WHITE_COLOR;
			}
			else {
				tcolor = DARK_COLOR;
				bcolor = WHITE_COLOR;
			}

			SetTextColor(hdc, tcolor);
			SetBkColor(hdc, bcolor);
			
			HDBGetCell(work->sheet, dwField, dwBlock, buf, sizeof(buf));

			width = field.wViewSize;

			gcls(hdc, x, y, x+width-1, y+work->cyChar-1, bcolor);

			draw_rect.left = x;
			draw_rect.top = y;
			draw_rect.right = x+width;
			draw_rect.bottom = y+work->cyChar;
			DrawText(hdc, buf, strlen(buf), &draw_rect, DT_LEFT | DT_VCENTER | DT_SINGLELINE);
			//TextOut(hdc, x, y, buf, strlen(buf));
			x+= width+1;
		}
		if(x < rect.right) {
			gcls(hdc, x, y, rect.right, y+work->cyChar-1, DARK_GRAY_COLOR);
		}
		y+=work->cyChar+1;
	}

	if(y < rect.bottom) {
		gcls(hdc, 0, y, rect.right, rect.bottom, DARK_GRAY_COLOR);
	}

	SelectObject(hdc, hPenOld);
//	SelectObject(hdc, hOldFont);

	Not(hdc, work);

	DeleteObject(hPen);
}

int GetCellX(HDB_WORK_STRUCT *work, DWORD pos)
{
	DWORD l;
	HDB_FIELD_STRUCT field;
	DWORD hap_field = HDBGetFieldHap(work->sheet);
	int x;

	x = work->pxStart;

	for(l = work->dwPageX; l < hap_field; l++) {
		if(pos == l)	return x;
		HDBGetField(work->sheet, l, &field);
		x += field.wViewSize+1;
	}
	return work->pxStart;
}

static WNDPROC lpfnOldCellEditProc;

long FAR PASCAL EXPORT EditCellProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	long lvalue;

	switch(message) {
		case WM_KEYDOWN:
			switch(wParam) {
				case VK_RETURN:
				case VK_UP:
				case VK_DOWN:
				case VK_PRIOR:
				case VK_NEXT:
				case VK_ESCAPE:
					SendMessage(GetParent(hwnd), message, wParam, lParam);
					return 0;
				case VK_LEFT:
					lvalue = SendMessage(hwnd, EM_GETSEL, 0, 0L);
					if(HIWORD(lvalue) != LOWORD(lvalue))	break;
					if(LOWORD(lvalue) == 0)	{
						SendMessage(GetParent(hwnd), message, wParam, lParam);
						return 0;
					}
					break;
				case VK_RIGHT:
					lvalue = SendMessage(hwnd, EM_GETSEL, 0, 0L);
					if(HIWORD(lvalue) != LOWORD(lvalue))	break;
					if(LOWORD(lvalue) == SendMessage(hwnd, EM_LINELENGTH, 0, 0))	{
						SendMessage(GetParent(hwnd), message, wParam, lParam);
						return 0;
					}
					break;
			}
			break;
	}

	return CallWindowProc(lpfnOldCellEditProc, hwnd, message, wParam, lParam);
}

//------------------------------------------------------------------------------
//	입력된 자료를 저장한다.
//	return 되는 값은 cursor의 시작 값이다.
//------------------------------------------------------------------------------

int HDBWorkCloseCellEdit(HDB_WORK_STRUCT *work)
{
	StackChar stack(HDB_MAX_STRING_SIZE);
	DWORD count_field;
	DWORD count_block;
	DWORD cursor_start, cursor_end;

	if(work->hwndEditCell == NULL)	return 0;
	if(stack.data == NULL)		return 0;

	count_field = HDBGetFieldHap(work->sheet);
	count_block = HDBGetBlockHap(work->sheet);

	if(work->dwCurrX >= count_field)	return 0;

	GetWindowText(work->hwndEditCell, stack.data, HDB_MAX_STRING_SIZE);

	HDBSetCell(work->sheet, work->dwCurrX, work->dwCurrY, stack.data);

	SendMessage(work->hwndEditCell, EM_GETSEL, (WPARAM)&cursor_start, (LPARAM)&cursor_end);

	DestroyWindow(work->hwndEditCell);
	work->hwndEditCell = NULL;

	return (int)cursor_start;
}

int HDBWorkCloseCellEditWithoutSave(HDB_WORK_STRUCT *work)
{
	if(work->hwndEditCell == NULL)	return 1;
	
	DestroyWindow(work->hwndEditCell);
	work->hwndEditCell = NULL;

	return 1;
}

void ActiveCellEdit(HWND hwnd, HDB_WORK_STRUCT *work, int cursor_start)
{
	int x, y;
	HDB_FIELD_STRUCT field;
	StackChar stack(HDB_MAX_STRING_SIZE);

	if(stack.data == NULL)	return;

	x = GetCellX(work, work->dwCurrX);
	y = (int)(work->pyStart+(work->dwCurrY-work->dwPageY)*(work->cyChar+1));

	HDBGetField(work->sheet, work->dwCurrX, &field);
	HDBGetCell(work->sheet, work->dwCurrX, work->dwCurrY, stack.data, HDB_MAX_STRING_SIZE);

	work->hwndEditCell =  CreateWindow("edit", NULL,
					WS_CHILD | WS_VISIBLE | WS_BORDER | ES_LEFT | ES_AUTOHSCROLL,
					x-2, y-2, field.wViewSize+4, work->cyChar+4, hwnd, (HMENU)1, work->hInst, NULL);

	lpfnOldCellEditProc = (WNDPROC) GetWindowLong(work->hwndEditCell, GWL_WNDPROC);
	SetWindowLong(work->hwndEditCell, GWL_WNDPROC, (LONG)EditCellProc);

	SetWindowText(work->hwndEditCell, stack.data);

	SendMessage(work->hwndEditCell, EM_SETSEL, cursor_start, cursor_start);

	SetFocus(work->hwndEditCell);
}

void ActiveCellEdit(HWND hwnd, HDB_WORK_STRUCT *work)
{
	ActiveCellEdit(hwnd, work, 0);
}

static int GetFieldScreenSize(HDB_SHEET_STRUCT *sheet, DWORD start_field, DWORD end_field)
{
	int size = 0;
	HDB_FIELD_STRUCT field;
	DWORD l;

	if(start_field > end_field)	Temp(start_field, end_field);

	for(l = start_field; l <= end_field; l++) {
		if(HDBGetField(sheet, l, &field)) {
			size += field.wViewSize+1;
		}
	}

	return size;
}

void HDBWorkWmKeyDown(HWND hwnd, HDB_WORK_STRUCT *work, WPARAM wParam)
{
//	RECT    rect;
	HDC hdc;
	DWORD	curr;
	char bEditing = work->hwndEditCell ? ON : OFF;
	int  cursor_pos;

	switch(wParam) {
		case VK_ESCAPE:
			HDBWorkCloseCellEditWithoutSave(work);
			break;
		case VK_RETURN:
		//case VK_TAB:
			if(work->hwndEditCell) {
				HDBWorkCloseCellEdit(work);
			}
			else 
				ActiveCellEdit(hwnd, work);

			break;
		case VK_UP:
			if(work->dwCurrY <= 0) break;

			if(bEditing)	cursor_pos = HDBWorkCloseCellEdit(work);

			if(work->dwCurrY > work->dwPageY) {
				hdc = GetDC(hwnd);
				Not(hdc, work);
				work->dwCurrY --;
				Select(work, work->dwCurrX, work->dwCurrY);
				Not(hdc, work);
				ReleaseDC(hwnd, hdc);
			}
			else {
				work->dwPageY --;
				work->dwCurrY --;
				Select(work, work->dwCurrX, work->dwCurrY);
				InvalidateRect(hwnd, NULL, FALSE);
				ScrollBarUpdate(hwnd, work);
			}

			if(bEditing)	ActiveCellEdit(hwnd, work, cursor_pos);

			break;
		case VK_DOWN:
			if(work->dwCurrY >= HDBGetBlockHap(work->sheet)-1) break;

			if(bEditing)	cursor_pos = HDBWorkCloseCellEdit(work);

			if((int)(work->dwCurrY-work->dwPageY) >= work->limity-1) {
				work->dwPageY ++;
				work->dwCurrY ++;
				
				//GetClientRect(hwnd, &rect);
				//rect.top = work->pyStart;
				Select(work, work->dwCurrX, work->dwCurrY);
				InvalidateRect(hwnd, NULL, FALSE);

				ScrollBarUpdate(hwnd, work);
			}
			else {
				hdc = GetDC(hwnd);
				Not(hdc, work);
				work->dwCurrY++;
				Select(work, work->dwCurrX, work->dwCurrY);
				Not(hdc, work);
				ReleaseDC(hwnd, hdc);
			}

			if(bEditing)	ActiveCellEdit(hwnd, work, cursor_pos);
			break;
		case VK_LEFT:
			if(work->dwCurrX <= 0) break;

			if(bEditing)	HDBWorkCloseCellEdit(work);

			if(work->dwCurrX > work->dwPageX) {
				hdc = GetDC(hwnd);
				Not(hdc, work);
				work->dwCurrX --;
				Select(work, work->dwCurrX, work->dwCurrY);
				Not(hdc, work);
				ReleaseDC(hwnd, hdc);
			}
			else {
				work->dwPageX --;
				work->dwCurrX --;
				Select(work, work->dwCurrX, work->dwCurrY);
				InvalidateRect(hwnd, NULL, FALSE);

				ScrollBarUpdate(hwnd, work);
			}

			if(bEditing)	ActiveCellEdit(hwnd, work);
			break;
		case VK_RIGHT:
			if(work->dwCurrX >= HDBGetFieldHap(work->sheet)-1)	break;

			if(bEditing)	HDBWorkCloseCellEdit(work);

			curr = work->dwCurrX;
			
			curr++;

			if(GetFieldScreenSize(work->sheet, work->dwPageX, curr) < work->rect.right-work->pxStart) {
				hdc = GetDC(hwnd);
				Not(hdc, work);
				work->dwCurrX = curr;
				Select(work, work->dwCurrX, work->dwCurrY);
				Not(hdc, work);
				ReleaseDC(hwnd, hdc);
			}
			else {
				while(1) {
					if(GetFieldScreenSize(work->sheet, work->dwPageX, curr) < work->rect.right-work->pxStart) {
						break;
					}
					work->dwPageX++;
					if(work->dwPageX == curr) {
						break;
					}
				}

				work->dwCurrX = curr;
				Select(work, work->dwCurrX, work->dwCurrY);

				//GetClientRect(hwnd, &rect);
				//rect.left = work->pxStart;
				InvalidateRect(hwnd, NULL, FALSE);

				ScrollBarUpdate(hwnd, work);
			}

			if(bEditing)	ActiveCellEdit(hwnd, work);
			break;

		case VK_PRIOR:
			if(work->dwCurrY <= 0)	break;

			if(bEditing)	HDBWorkCloseCellEdit(work);

			work->dwPageY-=(work->limity-1);
			work->dwCurrY-=(work->limity-1);

			if((long)work->dwPageY < 0)	work->dwPageY = 0;
			if((long)work->dwCurrY < 0)	work->dwCurrY = 0;

			Select(work, work->dwCurrX, work->dwCurrY);
			
			InvalidateRect(hwnd, NULL, TRUE);

			ScrollBarUpdate(hwnd, work);

			if(bEditing)	ActiveCellEdit(hwnd, work);
			
			break;
		case VK_NEXT:
			if(work->dwCurrY >= HDBGetBlockHap(work->sheet)-1)	break;

			if(bEditing)	HDBWorkCloseCellEdit(work);

			work->dwPageY+=(work->limity-1);
			work->dwCurrY+=(work->limity-1);

			if(work->dwPageY >= HDBGetBlockHap(work->sheet))	work->dwPageY = HDBGetBlockHap(work->sheet)-1;
			if(work->dwCurrY >= HDBGetBlockHap(work->sheet))	work->dwCurrY = HDBGetBlockHap(work->sheet)-1;

			Select(work, work->dwCurrX, work->dwCurrY);
			
			InvalidateRect(hwnd, NULL, TRUE);

			ScrollBarUpdate(hwnd, work);

			if(bEditing) ActiveCellEdit(hwnd, work);
			break;
	}
}

enum {
	CAPTURE_METHOD_CELL,
	CAPTURE_METHOD_FIELD_END,
	CAPTURE_METHOD_VIEW_FIELD,
	CAPTURE_METHOD_BLOCK,
};

static char  bMouseCapture = OFF;
static int   nMouseCaptureMethod = CAPTURE_METHOD_CELL;
static DWORD dwMouseCaptureField;  
static DWORD dwMouseCaptureBlock;  
static short nStartMX, nStartMY;
static short nOldMX, nOldMY;

static int GetCellPositionByMouse(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam, DWORD &dwField, DWORD &dwBlock, char move_flag)
{
	short mx, my;
	int x, y;
	DWORD hap_field  = HDBGetFieldHap(work->sheet);
	DWORD hap_block  = HDBGetBlockHap(work->sheet);
	HDB_FIELD_STRUCT field;
	int width;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	y = work->pyStart;

	for(dwBlock = work->dwPageY; dwBlock < hap_block && y < work->rect.bottom; dwBlock++, y+=work->cyChar+1) {
		if(my < y || my > y+work->cyChar)	continue;

		x = work->pxStart;

		for(dwField = work->dwPageX; dwField < hap_field && x < work->rect.right; dwField++) {
			HDBGetField(work->sheet, dwField, &field);
			width = field.wViewSize;
			if(mx >= x && mx <= x+width) {
				return 1;
			}
			x += width+1;
		}
	}

	return 0;
}

static int GetViewFieldByMouse(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam)
{
	short mx, my;
	int x;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	if(my >= work->pyStart)	return 0;

	x = work->pxStart;
	if(mx >= x-2 && mx <= x+2)	return 1;

	return 0;
}

static int GetFieldEndByMouse(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam, DWORD &pos)
{
	short mx, my;
	int x;
	DWORD hap_field  = HDBGetFieldHap(work->sheet);
	HDB_FIELD_STRUCT field;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	if(my >= work->pyStart)	return 0;

	x = work->pxStart;

	for(pos = work->dwPageX; pos < hap_field && x < work->rect.right; pos++) {
		HDBGetField(work->sheet, pos, &field);
		x += field.wViewSize+1;
		if(mx >= x-2 && mx <= x+2) {
			return 1;
		}
	}

	return 0;
}

static int GetBlockByMouse(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam, DWORD &pos, char moving_flag)
{
	short mx, my;
	int y;
	DWORD hap_block  = HDBGetBlockHap(work->sheet);

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	if(moving_flag == OFF)	// 누른상태에서 움직이는 것이 아닐 때 
		if(mx >= work->pxStart)	return 0;

	y = work->pyStart;

	for(pos = work->dwPageY; pos < hap_block && y < work->rect.bottom; pos++) {
		//HDBGetField(work->sheet, pos, &field);
		if(my >= y && my <= y+work->cyChar) {
			return 1;
		}
		y += work->cyChar+1;
	}

	return 0;
}

static void NotLine(HDC hdc, HDB_WORK_STRUCT *work, short mx, short my) 
{
	RECT r;

	r.left = mx-2;
	r.right = mx+3;
	r.top = 0;
	r.bottom = work->rect.bottom;

	InvertRect(hdc, &r);
}

void HDBWorkWmLButtonDown(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam)
{
	DWORD x, y;
	HDC hdc;

	if(GetCellPositionByMouse(hwnd, work, lParam, x, y, 0)) {
		HDBWorkCloseCellEdit(work);

		hdc = GetDC(hwnd);
		Not(hdc, work);
		work->dwCurrX = x;
		work->dwCurrY = y;
		Select(work, x, y);
		Not(hdc, work);
		ReleaseDC(hwnd, hdc);

		bMouseCapture = ON;
		SetCapture(hwnd);
		nMouseCaptureMethod = CAPTURE_METHOD_CELL;
	}
	else if(GetViewFieldByMouse(hwnd, work, lParam)) {
		nStartMX = nOldMX = LOWORD(lParam);
		nStartMY = nOldMY = HIWORD(lParam);

		hdc = GetDC(hwnd);
		NotLine(hdc, work, nOldMX, nOldMY);
		ReleaseDC(hwnd, hdc);

		bMouseCapture = ON;
		SetCapture(hwnd);
		nMouseCaptureMethod = CAPTURE_METHOD_VIEW_FIELD;
	}
	else if(GetFieldEndByMouse(hwnd, work, lParam, dwMouseCaptureField)) {
		nStartMX = nOldMX = LOWORD(lParam);
		nStartMY = nOldMY = HIWORD(lParam);

		hdc = GetDC(hwnd);
		NotLine(hdc, work, nOldMX, nOldMY);
		ReleaseDC(hwnd, hdc);

		bMouseCapture = ON;
		SetCapture(hwnd);
		nMouseCaptureMethod = CAPTURE_METHOD_FIELD_END;
	}
	else if(GetBlockByMouse(hwnd, work, lParam, dwMouseCaptureBlock, OFF)) {
		HDBWorkCloseCellEdit(work);

		hdc = GetDC(hwnd);
		Not(hdc, work);
		work->dwCurrX = 0;
		work->dwCurrY = dwMouseCaptureBlock;
		Select(work, 0, dwMouseCaptureBlock, HDBGetFieldHap(work->sheet)-1, dwMouseCaptureBlock);
		Not(hdc, work);
		ReleaseDC(hwnd, hdc);

		bMouseCapture = ON;
		SetCapture(hwnd);
		nMouseCaptureMethod = CAPTURE_METHOD_BLOCK;
	}
	else;
}

void HDBWorkWmLButtonDblClk(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam)
{
	DWORD x, y;

	if(GetCellPositionByMouse(hwnd, work, lParam, x, y, 0)) {	
		work->dwCurrX = x;
		work->dwCurrY = y;

		ActiveCellEdit(hwnd, work);
	}
}

static void MouseCursorStatus(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam)
{
	DWORD pos;

	if(GetFieldEndByMouse(hwnd, work, lParam, pos)) 
		SetCursor(LoadCursor(NULL, IDC_SIZEWE));
	else if(GetViewFieldByMouse(hwnd, work, lParam))
		SetCursor(LoadCursor(NULL, IDC_SIZEWE));
	else
		SetCursor(LoadCursor(NULL, IDC_ARROW));
}

void HDBWorkWmMouseMove(HWND hwnd, HDB_WORK_STRUCT *work, LPARAM lParam)
{	
	if(bMouseCapture == OFF) {
		MouseCursorStatus(hwnd, work, lParam);
		return;
	}

	DWORD x, y;
	HDC   hdc;

	if(nMouseCaptureMethod == CAPTURE_METHOD_CELL) {
		if(!GetCellPositionByMouse(hwnd, work, lParam, x, y, ON))	return;
		if(work->selx2 == x && work->sely2 == y)				return;	// same position

		hdc = GetDC(hwnd);
		Not(hdc, work);
		Select(work, work->selx1, work->sely1, x, y);
		Not(hdc, work);
		ReleaseDC(hwnd, hdc);
	}
	else if(nMouseCaptureMethod == CAPTURE_METHOD_VIEW_FIELD) {
		hdc = GetDC(hwnd);
		NotLine(hdc, work, nOldMX, nOldMY);
		nOldMX = LOWORD(lParam);
		nOldMY = HIWORD(lParam);
		NotLine(hdc, work, nOldMX, nOldMY);
		ReleaseDC(hwnd, hdc);
	}
	else if(nMouseCaptureMethod == CAPTURE_METHOD_FIELD_END) {
		hdc = GetDC(hwnd);
		NotLine(hdc, work, nOldMX, nOldMY);
		nOldMX = LOWORD(lParam);
		nOldMY = HIWORD(lParam);
		NotLine(hdc, work, nOldMX, nOldMY);
		ReleaseDC(hwnd, hdc);
	}
	else if(nMouseCaptureMethod == CAPTURE_METHOD_BLOCK) {
		DWORD pos;
		if(!GetBlockByMouse(hwnd, work, lParam, pos, ON))	return;

		if(dwMouseCaptureBlock == pos)	return;

		dwMouseCaptureBlock = pos;
		
		hdc = GetDC(hwnd);
		Not(hdc, work);
		Select(work, 0, work->dwCurrY, HDBGetFieldHap(work->sheet)-1, pos);
		Not(hdc, work);
		ReleaseDC(hwnd, hdc);
	}
}

static void ResizeCellEdit(HDB_WORK_STRUCT *work)
{
	if(work->hwndEditCell == NULL)	return;

	int x, y;
	HDB_FIELD_STRUCT field;
	
	x = GetCellX(work, work->dwCurrX);
	y = (int)(work->pyStart+(work->dwCurrY-work->dwPageY)*(work->cyChar+1));

	HDBGetField(work->sheet, work->dwCurrX, &field);

	MoveWindow(work->hwndEditCell, x-2, y-2, field.wViewSize+4, work->cyChar+4, TRUE);
}

void HDBWorkWmLButtonUp(HWND hwnd, HDB_WORK_STRUCT *work) 
{
	if(bMouseCapture == OFF)	return;

	HDC   hdc;

	if(nMouseCaptureMethod == CAPTURE_METHOD_VIEW_FIELD) {
		hdc = GetDC(hwnd);
		NotLine(hdc, work, nOldMX, nOldMY);
		ReleaseDC(hwnd, hdc);

		if(nOldMX == nStartMX) {

		}
		else {
			WORD size;
			
			size = HDBGetAlwaysViewFieldSize(work->sheet);
			size += (nOldMX-nStartMX);
			if(size > 500) {
				size = 500;
			}

			HDBWorkSetAlwaysViewFieldSize(work, size);
			
			ResizeCellEdit(work);

			InvalidateRect(hwnd, NULL, FALSE);
		}
	}
	else if(nMouseCaptureMethod == CAPTURE_METHOD_FIELD_END) {
		hdc = GetDC(hwnd);
		NotLine(hdc, work, nOldMX, nOldMY);
		ReleaseDC(hwnd, hdc);

		if(nOldMX == nStartMX) {

		}
		else {
			HDB_FIELD_STRUCT field;

			HDBGetField(work->sheet, dwMouseCaptureField, &field);
			field.wViewSize += (nOldMX-nStartMX);
			if(field.wViewSize > 500) {
				field.wViewSize = 500;
			}
			HDBSetField(work->sheet, dwMouseCaptureField, &field);

			ResizeCellEdit(work);

			InvalidateRect(hwnd, NULL, FALSE);
		}
	}
	else;

	bMouseCapture = OFF;
	ReleaseCapture();
}

void HDBWorkWmSize(HWND hwnd, HDB_WORK_STRUCT *work)
{
	GetClientRect(hwnd, &work->rect);

	work->limity = (work->rect.bottom-work->pyStart)/(work->cyChar+1);

	ScrollBarUpdate(hwnd, work);
}
/*
//------------------------------------------------------------------------------
// 폰트가 바뀌었을때 폰트의 정보를 얻는다.
//------------------------------------------------------------------------------

void GetFontInformation(HWND hwnd, WORK_EDIT_SHEET *work)
{
	HDC hdc;
	TEXTMETRIC tm;
	HFONT hOldFont;

	hdc = GetDC(hwnd);
	hOldFont = SelectObject(hdc, hViewFont);
	GetTextMetrics(hdc, &tm);
	SelectObject(hdc, hOldFont);
	ReleaseDC(hwnd, hdc);

	work->cxChar = tm.tmAveCharWidth;
	work->cyChar = tm.tmHeight+tm.tmExternalLeading+2;

	work->pxStart = work->cxChar*HDBGetAlwaysViewFieldSize(work->sSheetName);
	work->pyStart = work->cyChar+2;
}
*/

void HDBWorkWmHScroll(HWND hwnd, HDB_WORK_STRUCT *work, UINT nSBCode, UINT nPos)
{
	DWORD curr;
	HDB_FIELD_STRUCT field;
	int width;

	switch(nSBCode) {
		case SB_LINEUP:
			if(work->dwPageX == 0)	return;
			HDBWorkCloseCellEdit(work);

			work->dwPageX--;
			break;
		case SB_LINEDOWN:
			if(work->dwPageX >= HDBGetFieldHap(work->sheet)-1)	return;
			HDBWorkCloseCellEdit(work);
			work->dwPageX++;
			break;
		case SB_PAGEUP:
			if(work->dwPageX == 0)	return;	// 
			HDBWorkCloseCellEdit(work);
			width = 0;
			curr = work->dwPageX;

			curr--;

			while(1) {
				HDBGetField(work->sheet, curr, &field);
				width += field.wViewSize+1;
				if(width >= work->rect.right-work->pxStart)	break;
				if(curr == 0)	break;	//					
				curr--;
			}
			work->dwPageX = curr;
			break;
		case SB_PAGEDOWN:
			if(work->dwPageX >= HDBGetFieldHap(work->sheet)-1)	return;	// last
			HDBWorkCloseCellEdit(work);
			width = 0;
			curr = work->dwPageX;

			while(1) {
				HDBGetField(work->sheet, curr, &field);
				width += field.wViewSize+1;
				if(width >= work->rect.right-work->pxStart)	break;
				if(curr >= HDBGetFieldHap(work->sheet)-1)	break;	//					
				curr++;
			}
			work->dwPageX = curr;
			break;
		case SB_THUMBPOSITION:
			if(work->dwPageX == nPos)	break;
			HDBWorkCloseCellEdit(work);

			work->dwPageX = nPos;			
			if(work->dwPageX >= HDBGetFieldHap(work->sheet)) {
				work->dwPageX = HDBGetFieldHap(work->sheet)-1;	
			}
			break;
		default:
			return;
	}

	InvalidateRect(hwnd, NULL, FALSE);
	ScrollBarUpdate(hwnd, work);
}

void HDBWorkWmVScroll(HWND hwnd, HDB_WORK_STRUCT *work, UINT nSBCode, UINT nPos)
{
	switch(nSBCode) {
		case SB_LINEUP:
			if(work->dwPageY == 0)	return;
			work->dwPageY--;
			break;
		case SB_LINEDOWN:
			if(work->dwPageY >= HDBGetBlockHap(work->sheet)-1)	return;
			work->dwPageY++;
			break;
		case SB_PAGEUP:
			if(work->dwPageY == 0)	return;
			if((long)work->dwPageY-work->limity < 0) {
				work->dwPageY = 0;
			}
			else {
				work->dwPageY -= work->limity;
			}
			break;
		case SB_PAGEDOWN:
			if(work->dwPageY >= HDBGetBlockHap(work->sheet)-1)	return;
			work->dwPageY += work->limity;
			if(work->dwPageY >= HDBGetBlockHap(work->sheet)) {
				work->dwPageY = HDBGetBlockHap(work->sheet)-1;	
			}
			break;
		case SB_THUMBPOSITION:
			work->dwPageY = nPos;			
			if(work->dwPageY >= HDBGetBlockHap(work->sheet)) {
				work->dwPageY = HDBGetBlockHap(work->sheet)-1;	
			}
			break;
		default:
			return;
	}

	InvalidateRect(hwnd, NULL, FALSE);
	ScrollBarUpdate(hwnd, work);
}

static Block blockClipBoard(1000);
static DWORD dwClipBoardX, dwClipBoardY;

void HDBWorkIdmCopy(HWND hwnd, HDB_WORK_STRUCT *work)
{
	DWORD x1, y1, x2, y2;
	DWORD x, y;
	StackChar buf(1000);

	if(buf.data == NULL)	return;
	
	x1 = work->selx1;
	y1 = work->sely1;
	x2 = work->selx2;
	y2 = work->sely2;

	if(x1 > x2)	Temp(x1, x2);
	if(y1 > y2) Temp(y1, y2);

	blockClipBoard.DeleteAllBlock();

	for(y = y1; y <= y2; y++) {
		for(x = x1; x <= x2; x++) {
			HDBGetCell(work->sheet, x, y, buf.data, 1000);
			blockClipBoard.AddBlock(buf.data);		
		}
	}

	dwClipBoardX = x2-x1+1;
	dwClipBoardY = y2-y1+1;
}

void HDBWorkIdmPaste(HWND hwnd, HDB_WORK_STRUCT *work)
{
	DWORD x, y;
	StackChar buf(1000);

	if(buf.data == NULL)	return;
	if(blockClipBoard.GetCount() == 0)	return;
	
	for(y = 0; y < dwClipBoardY; y++) {
		for(x = 0; x < dwClipBoardX; x++) {
			blockClipBoard.GetBlock(buf.data, x+y*dwClipBoardX);		
			HDBSetCell(work->sheet, x+work->dwCurrX, y+work->dwCurrY, buf.data);
		}
	}

	Select(work, work->dwCurrX, work->dwCurrY, work->dwCurrX+dwClipBoardX-1, work->dwCurrY+dwClipBoardY-1);

	InvalidateRect(hwnd, NULL, FALSE);
}

void HDBWorkIdmCut(HWND hwnd, HDB_WORK_STRUCT *work)
{
	DWORD x1, y1, x2, y2;
	DWORD x, y;
	StackChar buf(1000);
	char imsi[10];

	if(buf.data == NULL)	return;
	imsi[0] = 0;
	
	x1 = work->selx1;
	y1 = work->sely1;
	x2 = work->selx2;
	y2 = work->sely2;

	if(x1 > x2)	Temp(x1, x2);
	if(y1 > y2) Temp(y1, y2);

	blockClipBoard.DeleteAllBlock();

	for(y = y1; y <= y2; y++) {
		for(x = x1; x <= x2; x++) {
			HDBGetCell(work->sheet, x, y, buf.data, 1000);
			blockClipBoard.AddBlock(buf.data);		
			HDBSetCell(work->sheet, x, y, imsi);
		}
	}

	dwClipBoardX = x2-x1+1;
	dwClipBoardY = y2-y1+1;

	InvalidateRect(hwnd, NULL, FALSE);
}

void HDBWorkIdmInsertOneBlock(HWND hwnd, HDB_WORK_STRUCT *work)
{
	HDBWorkCloseCellEdit(work);
	
	HDBInsertNewBlock(work->sheet, work->dwCurrY);
	InvalidateRect(hwnd, NULL, FALSE);

	ScrollBarUpdate(hwnd, work);
}

void HDBWorkIdmAddOneBlock(HWND hwnd, HDB_WORK_STRUCT *work)
{
	HDBWorkCloseCellEdit(work);
	
	if(!HDBAddNewBlock(work->sheet))	return;

	work->dwPageX = 0;
	work->dwCurrX = 0;
	work->dwCurrY = HDBGetBlockHap(work->sheet)-1;

	if(work->dwCurrY-work->dwPageY > (DWORD)work->limity) {
		work->dwPageY = work->dwCurrY;
	}

	Select(work, work->dwCurrX, work->dwCurrY);

	InvalidateRect(hwnd, NULL, FALSE);

	ScrollBarUpdate(hwnd, work);
}

void HDBWorkIdmDelOneBlock(HWND hwnd, HDB_WORK_STRUCT *work)
{
	HDBWorkCloseCellEdit(work);
	
	HDBDeleteOneBlock(work->sheet, work->dwCurrY);
	InvalidateRect(hwnd, NULL, FALSE);

	ScrollBarUpdate(hwnd, work);
}

void HDBWorkInit(HWND hwnd, HDB_WORK_STRUCT *work, HINSTANCE hinst)
{
	HDC hdc;
	TEXTMETRIC tm;
	
	memset(work, 0, sizeof(HDB_WORK_STRUCT));
	
	work->dwCurrX  = 0;
	work->dwCurrY = 0;
	work->dwPageX  = 0;
	work->dwPageY = 0;

	work->sheet = HDBMakeNewSheet("ex", "description");

	hdc = GetDC(hwnd);
	GetTextMetrics(hdc, &tm);
	ReleaseDC(hwnd, hdc);

	work->cxChar = tm.tmAveCharWidth;
	work->cyChar = tm.tmHeight+tm.tmExternalLeading;

	work->pxStart = 100;
	work->pyStart = work->cyChar*2+5;

	Select(work, work->dwCurrX, work->dwCurrY);

	work->hInst = hinst;

	ShowScrollBar(hwnd, SB_HORZ, TRUE);
	ShowScrollBar(hwnd, SB_VERT, TRUE);

	//GetFontInformation(hwnd, work);		// 폰트가 바뀌었을 때 폰트의 정보를 얻는다.
}

int  HDBWorkSetAlwaysViewFieldSize(HDB_WORK_STRUCT *work, int size)
{
	int retn = HDBSetAlwaysViewFieldSize(work->sheet, size);

	if(retn)	work->pxStart = HDBGetAlwaysViewFieldSize(work->sheet);

	return retn;
}



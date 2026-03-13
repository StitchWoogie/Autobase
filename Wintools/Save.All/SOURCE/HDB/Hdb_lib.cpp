// english O.K
#include "stdafx.h"
#include <stdio.h>
#include <string.h>
#include <math.h>

#include <glib.h>
#include <tools.h>
#include <dataswap.h>

#include "hdb_lib.h"

long double CalculateFunctionRecurse(char *s, int size);
void RecurseInit(HDB_SHEET_STRUCT *sheet, DWORD block);
int  IsRecurseWrong();
int  IsDataNone();

void HDBFieldMakeDefault(HDB_FIELD_STRUCT *field)
{
	memset(field, 0, sizeof(HDB_FIELD_STRUCT));
	field->wSize = 10;
	field->cDisplayFormMinor = 2;
	field->wType = HDB_FIELD_TYPE_STRING;
	field->blockData = NULL;
}

int HDBFieldAdd(HDB_SHEET_STRUCT *sheet, HDB_FIELD_STRUCT *field, char *function)
{
	field->blockData = new Block(field->wSize+1);
	return sheet->blockField->AddBlock(field);
}

/*
int HDBDeleteField(char *sheet_name, int pos)
{
	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;
	int i;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 0;
	sheet = &sheetAll[sheet_pos];

	if(pos >= sheet->wFieldHap)	return 0;
	if(sheet->wFieldHap == 0)	return 0;

	if(sheet->wFieldHap == 1) {
		if(sheet->fieldAll[0].hBlock) {
			GlobalFree(sheet->fieldAll[0].hBlock);
		}
		GlobalUnlock(sheet->hField);
		GlobalFree(sheet->hField);

		sheet->hField = NULL;
		sheet->wFieldHap = 0;
		bChangeFlag = ON;
		return 1;
	}

	HGLOBAL hFieldImsi;
	HDB_FIELD_STRUCT huge *fieldImsi;

	hFieldImsi = GlobalAlloc(GMEM_MOVEABLE, (long)sizeof(HDB_FIELD_STRUCT)*(sheet->wFieldHap-1));
	if(hFieldImsi == NULL) {
		MessageBox(NULL, "Global memory low", "HDBDeleteField", MB_OK);
		return 0;
	}
	fieldImsi = (HDB_FIELD_STRUCT huge*)GlobalLock(hFieldImsi);

	// pos아래 복사.
	for(i = 0; i <= pos-1; i++) {
		memcpy(&fieldImsi[i], &sheet->fieldAll[i], sizeof(HDB_FIELD_STRUCT));
	}
	// pos 위치 free
	if(sheet->fieldAll[pos].hBlock) {
		GlobalFree(sheet->fieldAll[pos].hBlock);
	}
	// pos 위 복사.
	for(i = pos; i < sheet->wFieldHap-1; i++) {
		memcpy(&fieldImsi[i], &sheet->fieldAll[i+1], sizeof(HDB_FIELD_STRUCT));
	}

   GlobalUnlock(hFieldImsi);
	GlobalUnlock(sheet->hField);
	GlobalFree(sheet->hField);

   sheet->hField = hFieldImsi;
	sheet->fieldAll = (HDB_FIELD_STRUCT huge *) GlobalLock(sheet->hField);
	sheet->wFieldHap --;

	bChangeFlag = ON;

	return 1;
}
*/

//------------------------------------------------------------------------------
//	깨끗한 블럭을 하나 추가한다.
// 모든 필드마다 하나의 셀씩을 추가 해야 하기 때문에 중간에 어떤원인으로 추가하지 못했을 때
// 문제가 발생하게 된다.
// 문제 해결은 임시로 만든 핸들에 모든 필드가 이상이 없는지 확인한 다음
// 핸들을 교환해 준다.
//------------------------------------------------------------------------------

int HDBInsertNewBlock(HDB_SHEET_STRUCT *sheet, DWORD pos)
{
	HDB_FIELD_STRUCT field;
	DWORD l;

	if(sheet->blockField->GetCount() == 0)	return 0;	// 필드가 없을때는 셀을 만들 수 없다.
											// 일단 필드부터 만들것

	if(sheet->dwDataHap >= 0xFFFFFFF0L) {
		// MessageBox(NULL, "데이타  갯수가 많은 이유로 블럭을 새로 만들 수 없습니다.", "HDBAddNewBlock Error", MB_OK);
		return 0;
	}

	for(l = 0; l < sheet->blockField->GetCount(); l++) {
		sheet->blockField->GetBlock(&field, l);
		{
			StackChar buf(field.wSize+1);
			if(buf.data == NULL)	return 0;
			buf.data[0] = 0;
			if(!field.blockData->InsertBlock(buf.data, pos))	return 0;
		}
	}

	sheet->dwDataHap++;

	sheet->bChangeFlag = ON;

	return 1;
}

int HDBAddNewBlock(HDB_SHEET_STRUCT *sheet)
{	
	return HDBInsertNewBlock(sheet, sheet->dwDataHap);
}

int HDBDeleteOneBlock(HDB_SHEET_STRUCT *sheet, DWORD pos)
{
	HDB_FIELD_STRUCT field;
	DWORD l;

	if(sheet->blockField->GetCount() == 0)	return 0;	// 필드가 없을때는 셀을 만들 수 없다.
														// 일단 필드부터 만들것

	if(sheet->dwDataHap == 0) {
		// MessageBox(NULL, "데이타  갯수가 많은 이유로 블럭을 새로 만들 수 없습니다.", "HDBAddNewBlock Error", MB_OK);
		return 0;
	}

	for(l = 0; l < sheet->blockField->GetCount(); l++) {
		sheet->blockField->GetBlock(&field, l);
		if(!field.blockData->DeleteBlock(pos))	return 0;
	}

	sheet->dwDataHap--;

	sheet->bChangeFlag = ON;

	return 1;
}

//------------------------------------------------------------------------------
//	Cell 하나를 갱신 한다.
//------------------------------------------------------------------------------

int HDBSetCell(HDB_SHEET_STRUCT *sheet, DWORD field_pos, DWORD block_pos, char *string)
{
	if(field_pos >= sheet->blockField->GetCount())	 return 0;
	if(block_pos >= sheet->dwDataHap)   return 0;

	HDB_FIELD_STRUCT field;
	int  limit;

	sheet->blockField->GetBlock(&field, field_pos);

	if(field.wType == HDB_FIELD_TYPE_FUNCTION)	return 1;
	if(field.blockData == NULL)	return 0;

	StackChar buf(field.wSize+1);
	if(buf.data == NULL)	return 0;

	limit = strlen(string)+1;
	if(limit > field.wSize)	limit = field.wSize;

	field.blockData->GetBlock(buf.data, block_pos);

	if(string[0] == 0) {
		if(buf.data[0] == 0) {
			
		}
		else {
			buf.data[0] = 0;
			field.blockData->SetBlock(buf.data, block_pos);
			sheet->bChangeFlag = ON;
		}
	}
	else {
		if(strcmp(buf.data, string) == 0) {

		}
		else {
			strncpy(buf.data, string, limit);
			field.blockData->SetBlock(buf.data, block_pos);
			//memcpy(buf+field->wSizeFitBlock*block_pos, string, limit);
			sheet->bChangeFlag = ON;
		}
	}

	return 1;
}

//------------------------------------------------------------------------------
//	Cell 하나를 읽어온다.
//------------------------------------------------------------------------------

int HDBGetCell(HDB_SHEET_STRUCT *sheet, DWORD field_pos, DWORD block_pos, char *string, int limit)
{
	if(field_pos >= sheet->blockField->GetCount())		return 0;
	if(block_pos >= sheet->dwDataHap)   return 0;

	HDB_FIELD_STRUCT field;

	sheet->blockField->GetBlock(&field, field_pos);

	if(limit > field.wSize)	limit = field.wSize;

	if(field.wType == HDB_FIELD_TYPE_FUNCTION) {
		/*
		if(field->hBlock != NULL) {
			StackChar stack(1000);
			if(stack.data != NULL) {
				buf = (BYTE huge *) GlobalLock(field->hBlock);
				memcpy(stack.data, buf, field->wSizeFitBlock);
				stack.data[field->wSizeFitBlock] = 0;
				GlobalUnlock(field->hBlock);

				RecurseInit(sheet, block_pos);
				field->bRecurseFlag = ON;

				long double value = CalculateFunctionRecurse(stack.data, field->wSizeFitBlock);
				if(IsRecurseWrong())
					strcpy(string, stack.data);
				else if(IsDataNone()) {
					string[0] = '#';
					string[1] = 0;
				}
				else {
					char disp_form[15];
					sprintf(disp_form, "%%.%df", field->cDisplayFormMinor);
					sprintf(string, disp_form, (float)value);
				}
			}
			else {
				string[0] = 0;
			}
		}
		else {
			string[0] = 0;
		}
		
		return 1;
		*/
	}

	if(limit > field.wSize)	limit = field.wSize;

	StackChar buf(field.wSize+1);
	if(buf.data == NULL)	return 0;

	field.blockData->GetBlock(buf.data, block_pos);
	strncpy(string, buf.data, limit-1);
	string[limit-1] = 0;

	return 1;
}

/*
int HDBGetCellByName(char *sheet_name, char *field_name, DWORD block_pos, char *string, int limit)
{
	HDB_FIELD_STRUCT huge *field;
	int i;
	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 0;
	sheet = &sheetAll[sheet_pos];

	for(i = 0; i < sheet->wFieldHap; i++) {
		field = &sheet->fieldAll[i];
		if(strcmp(field_name, field->sName) == 0) {
			return HDBGetCell(sheet_name, i, block_pos, string, limit);
		}
	}

	return 0;
}

int HDBSetCellByName(char *sheet_name, char *field_name, DWORD block_pos, char *string)
{
	HDB_FIELD_STRUCT huge *field;
	int i;
	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 0;
	sheet = &sheetAll[sheet_pos];

	for(i = 0; i < sheet->wFieldHap; i++) {
		field = &sheet->fieldAll[i];
		if(strcmp(field_name, field->sName) == 0) {
			return HDBSetCell(sheet_name, i, block_pos, string);
		}
	}

	return 0;
}

void HDBFreeAll()
{
	if(sheetAll == NULL)	return;

	int i, j;

	HDB_FIELD_STRUCT huge *field;
	HDB_SHEET_STRUCT *sheet;

	for(i = 0; i < nSheetHap; i++) {
		sheet = &sheetAll[i];
		if(sheet->hField == NULL)	continue;
		for(j = 0; j < sheet->wFieldHap; j++) {
			field = &sheet->fieldAll[j];
			if(field->hBlock != NULL)	{
				GlobalFree(field->hBlock);
				field->hBlock = NULL;
			}
		}
		GlobalUnlock(sheet->hField);
		GlobalFree(sheet->hField);
		sheet->hField = NULL;
	}

	delete sheetAll;
	sheetAll = NULL;
	nSheetHap = 0;
}

int HDBLoad(HWND hwnd, char *filename)
{
	FILE *in;
	int  size;
	HDB_MAIN_HEADER head;
	int  i;
	BYTE huge *buf;
	DWORD dw;
	WORD crc;
	int j;
	char message[MAXPATH];
	HDB_SHEET_STRUCT *sheet;

	// 아래부분은 태그 구조체의 크기가 2의 승수인가를 검사한다.
	// Global 메모리에서는 구조체의 크기가 2,4,8,16... 의 크기가 되어야 한다.
	int seed = 1;
	for(i = 0; i < 10; i++) {
		seed *= 2;
		if(sizeof(HDB_FIELD_STRUCT) == seed)	goto next_1;
	}
	wsprintf(message, "Field 구조체의 크기는\n2의 n승일것 (현재-%d)", sizeof(HDB_FIELD_STRUCT));
	MessageBox(hwnd, message, "프로그램 오류", MB_OK);
	return 0;

  next_1:

	nSheetHap = 0;

	in = fopen(filename, "rb");
	if(in == NULL)	return 0;
	size = fread(&head, 1, sizeof(HDB_MAIN_HEADER), in);
	if(head.crc != GetCRC16((BYTE*)&head, sizeof(HDB_MAIN_HEADER)-2)) {
		MessageBox(hwnd, "HDB_MAIN_HEADER invalid CRC error", "HDBLoad Error", MB_OK);
		fclose(in);
		return 0;
	}
	if(size != sizeof(HDB_MAIN_HEADER)) {
		MessageBox(hwnd, "HDB_MAIN_HEADER invalid", "HDBLoad Error", MB_OK);
		fclose(in);
		return 0;
	}

	if(head.wSheetHap == 0) {
		fclose(in);
		return 1;
	}

	sheetAll = new HDB_SHEET_STRUCT[head.wSheetHap];

	if(sheetAll == NULL) {
		MessageBox(hwnd, "local memory low", "HDBLoad Error", MB_OK);
		fclose(in);
		nSheetHap = 0;
		return 0;
	}

	memset(sheetAll, 0, (unsigned)sizeof(HDB_SHEET_STRUCT)*head.wSheetHap);

	nSheetHap = head.wSheetHap;

	for(i = 0; i < nSheetHap; i++) {
		sheet = &sheetAll[i];
		fread(sheet, 1, sizeof(HDB_SHEET_STRUCT), in);
		fread(&crc, 1, 2, in);

		if(crc != GetCRC16((BYTE*)sheet, sizeof(HDB_SHEET_STRUCT))) {
			sheet->fieldAll = NULL;
			sheet->hField = NULL;
			fclose(in);
			MessageBox(hwnd, "Sheet struct CRC mismatched", "HDBLoad Error", MB_OK);
			HDBFreeAll();
			return 0;
		}

		sheet->fieldAll = NULL;
		sheet->hField = NULL;
	}

	for(j = 0; j < nSheetHap; j++) {
		sheet = &sheetAll[j];

		if(sheet->wFieldHap == 0) {
			continue;
		}

		sheet->hField = GlobalAlloc(GMEM_MOVEABLE, (long)sizeof(HDB_FIELD_STRUCT)*sheet->wFieldHap);
		if(sheet->hField == NULL) {
			fclose(in);
			sprintf(message, "Global memory low");
			MessageBox(hwnd, message, "HDBLoad Error", MB_OK);
			HDBFreeAll();
			return 0;
		}
		sheet->fieldAll = (HDB_FIELD_STRUCT huge*) GlobalLock(sheet->hField);

		for(i = 0; i < sheet->wFieldHap; i++) {
			memset(&sheet->fieldAll[i], 0, sizeof(HDB_FIELD_STRUCT));
		}

		for(i = 0; i < sheet->wFieldHap; i++) {
			fread(&sheet->fieldAll[i], 1, sizeof(HDB_FIELD_STRUCT), in);
			fread(&crc, 1, 2, in);

			if(crc != GetCRC16((BYTE*)&sheet->fieldAll[i], sizeof(HDB_FIELD_STRUCT))) {
				sheet->fieldAll[i].hBlock = NULL;
				fclose(in);
				MessageBox(hwnd, "field header crc error", "HDBLoad Error", MB_OK);
				HDBFreeAll();
				return 0;
			}
			sheet->fieldAll[i].hBlock = NULL;
		}

		if(sheet->dwBlockHap > 0) {
			for(i = 0; i < sheet->wFieldHap; i++) {
				// 계산식의 필드일 때
				if(sheet->fieldAll[i].wType == HDB_FIELD_TYPE_FUNCTION) {
					if(sheet->fieldAll[i].wSizeFitBlock > 0) {
						sheet->fieldAll[i].hBlock = GlobalAlloc(GMEM_MOVEABLE, sheetAll[j].fieldAll[i].wSizeFitBlock);
						if(sheet->fieldAll[i].hBlock == NULL) {
							MessageBox(hwnd, "Global Memory Low hBlock", "HDBLoad Error", MB_OK);
							fclose(in);
							HDBFreeAll();
							return 0;
						}
						buf = (BYTE huge *) GlobalLock(sheet->fieldAll[i].hBlock);
						fread(buf, 1, sheet->fieldAll[i].wSizeFitBlock, in);
						GlobalUnlock(sheet->fieldAll[i].hBlock);
					}
				}
				else {
					sheet->fieldAll[i].hBlock = GlobalAlloc(GMEM_MOVEABLE, sheet->fieldAll[i].wSizeFitBlock*sheet->dwBlockHap);
					if(sheet->fieldAll[i].hBlock == NULL) {
						MessageBox(hwnd, "Global Memory Low hBlock", "HDBLoad Error", MB_OK);
						fclose(in);
						HDBFreeAll();
						return 0;
					}
					buf = (BYTE huge *) GlobalLock(sheet->fieldAll[i].hBlock);
					for(dw = 0; dw < sheet->dwBlockHap; dw++) {
						fread(buf+sheet->fieldAll[i].wSizeFitBlock*dw, 1, sheet->fieldAll[i].wSizeFitBlock, in);
					}
					GlobalUnlock(sheet->fieldAll[i].hBlock);
				}
			}	// for loop
		}		// block 이 0보다 클때
	}

	fclose(in);

	return 1;
}


int HDBSave(char *filename)
{
	FILE *out;
	HDB_MAIN_HEADER head;
	int  i, j;
	BYTE huge *buf;
	int dw;
	WORD crc;
	HDB_SHEET_STRUCT *sheet;
	HDB_FIELD_STRUCT huge *field;

	out = fopen(filename, "wb");
	if(out == NULL) {
		MessageBox(NULL, "Write Open Error", "HDBSave Error", MB_OK);
		return 0;
	}

	memset(&head, 0, sizeof(HDB_MAIN_HEADER));

	head.dwVersion = 1;
	head.wSheetHap = nSheetHap;
	head.crc = GetCRC16((BYTE*)&head, sizeof(HDB_MAIN_HEADER)-2);
	fwrite(&head, 1, sizeof(HDB_MAIN_HEADER), out);

	for(j = 0; j < nSheetHap; j++) {
		sheet = &sheetAll[j];
		fwrite(sheet, 1, sizeof(HDB_SHEET_STRUCT), out);
		crc = GetCRC16((BYTE*)sheet, sizeof(HDB_SHEET_STRUCT));
		fwrite(&crc, 1, 2, out);
	}

	for(j = 0; j < nSheetHap; j++) {
		sheet = &sheetAll[j];

		for(i = 0; i < sheet->wFieldHap; i++) {
			field = &sheet->fieldAll[i];
			fwrite(field, 1, sizeof(HDB_FIELD_STRUCT), out);
			crc = GetCRC16((BYTE*)field, sizeof(HDB_FIELD_STRUCT));
			fwrite(&crc, 1, 2, out);
		}

		for(i = 0; i < sheet->wFieldHap; i++) {
			field = &sheet->fieldAll[i];
			if(field->wType == HDB_FIELD_TYPE_FUNCTION) {
				if(field->hBlock != NULL) {
					buf = (BYTE huge *) GlobalLock(field->hBlock);
					fwrite(buf, 1, field->wSizeFitBlock, out);
					GlobalUnlock(field->hBlock);
				}
			}
			else {
				if(field->hBlock != NULL) {
					buf = (BYTE huge *) GlobalLock(field->hBlock);
					for(dw = 0; dw < sheet->dwBlockHap; dw++) {
						fwrite(buf+field->wSizeFitBlock*dw, 1, field->wSizeFitBlock, out);
					}
					GlobalUnlock(field->hBlock);
				}
			}
		}
	}

	fclose(out);

	bChangeFlag = OFF;

	return 1;
}
*/

DWORD HDBGetFieldHap(HDB_SHEET_STRUCT *sheet)
{
	return sheet->blockField->GetCount();
}

DWORD HDBGetBlockHap(HDB_SHEET_STRUCT *sheet)
{
	return sheet->dwDataHap;
}

/*

int HDBGetFieldName(char *sheet_name, char *fieldname, int pos)
{
	fieldname[0] = '?';
	fieldname[1] = NULL;

	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 0;
	sheet = &sheetAll[sheet_pos];

	if(pos < 0 || pos >= sheet->wFieldHap)	return 0;

	strcpy(fieldname, sheet->fieldAll[pos].sName);

	return 1;
}
*/

int HDBGetField(HDB_SHEET_STRUCT *sheet, DWORD pos, HDB_FIELD_STRUCT *field)
{
	field->sName[0] = '?';
	field->sName[1] = NULL;

	if(pos >= sheet->blockField->GetCount())	return 0;

	sheet->blockField->GetBlock(field, pos);

	return 1;
}

int HDBSetField(HDB_SHEET_STRUCT *sheet, DWORD pos, HDB_FIELD_STRUCT *field)
{
	if(pos >= sheet->blockField->GetCount())	return 0;

	sheet->blockField->SetBlock(field, pos);

	return 1;
}

/*
int HDBGetFieldFunction(char *sheet_name, int field_pos, char *function)
{
	function[0] = 0;

	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 0;
	sheet = &sheetAll[sheet_pos];

	if(field_pos < 0 || field_pos >= sheet->wFieldHap)	return 0;

	HDB_FIELD_STRUCT huge *field = &sheet->fieldAll[field_pos];

	if(field->wType != HDB_FIELD_TYPE_FUNCTION)	return 0;
	if(field->wSizeFitBlock == 0)	return 0;
	if(field->hBlock == NULL)	return 0;

	BYTE *buf;

	buf = (BYTE*) GlobalLock(field->hBlock);
	memcpy(function, buf, field->wSizeFitBlock);
	function[field->wSizeFitBlock] = 0;
	GlobalUnlock(field->hBlock);

	return 1;
}

int HDBSetFieldFunction(char *sheet_name, int field_pos, char *function)
{
	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 0;
	sheet = &sheetAll[sheet_pos];

	if(field_pos < 0 || field_pos >= sheet->wFieldHap)	return 0;

	HDB_FIELD_STRUCT huge *field = &sheet->fieldAll[field_pos];

	if(field->wType != HDB_FIELD_TYPE_FUNCTION)	return 0;

	HGLOBAL hGlobal = NULL;
	BYTE *buf;

	if(strlen(function) > 0) {
		hGlobal = GlobalAlloc(GMEM_MOVEABLE, strlen(function));
		if(hGlobal == NULL)	return 0;
		buf = (BYTE*) GlobalLock(hGlobal);
		memcpy(buf, function, strlen(function));
		GlobalUnlock(hGlobal);
	}

	GlobalFree(field->hBlock);
	field->hBlock = hGlobal;
	field->wSizeFitBlock = strlen(function);

	bChangeFlag = ON;

	return 1;
}

int HDBIsChanged()
{
	return bChangeFlag;
}

void HDBSetFieldViewSize(char *sheet_name, int field_pos, WORD size)
{
	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return;
	sheet = &sheetAll[sheet_pos];

	if(field_pos < 0 || field_pos >= sheet->wFieldHap)	return;

	HDB_FIELD_STRUCT huge *field = &sheet->fieldAll[field_pos];

	if(size < 1)	size = 1;

	if(field->wViewSize != size) {
		bChangeFlag = ON;
		field->wViewSize = size;
	}

	return;
}
*/

DWORD HDBSearchField(HDB_SHEET_STRUCT *sheet, char *field_name)
{
	HDB_FIELD_STRUCT field;
	DWORD l;

	for(l = 0; l < sheet->blockField->GetCount(); l++) {
		sheet->blockField->GetBlock(&field, l);
		if(strcmp(field_name, field.sName) == 0) {
			return l;
		}
	}
	return -1;
}

/*

int HDBChangeFieldName(char *sheet_name, int field_pos, char *field_name)
{
	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 0;
	sheet = &sheetAll[sheet_pos];

	if(field_pos < 0 || field_pos >= sheet->wFieldHap)	return 0;

	if(strcmp(field_name, sheet->fieldAll[field_pos].sName) != 0)
		bChangeFlag = ON;

	strncpy(sheet->fieldAll[field_pos].sName, field_name, 10);
	sheet->fieldAll[field_pos].sName[10] = 0;

	return 1;
}

int HDBChangeFieldDescription(char *sheet_name, int field_pos, char *field_des)
{
	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 0;
	sheet = &sheetAll[sheet_pos];

	if(field_pos < 0 || field_pos >= sheet->wFieldHap)	return 0;

	if(strcmp(field_des, sheet->fieldAll[field_pos].sDescription) != 0)
		bChangeFlag = ON;

	strncpy(sheet->fieldAll[field_pos].sDescription, field_des, 20);
	sheet->fieldAll[field_pos].sDescription[20] = 0;

	return 1;
}

*/

HDB_SHEET_STRUCT *HDBMakeNewSheet(char *sheet_name, char *description)
{
	if(strlen(sheet_name) > 10)		return NULL;
	if(strlen(description) > 20)	return NULL;

	HDB_SHEET_STRUCT *sheet;

	sheet = new HDB_SHEET_STRUCT[1];

	if(sheet == NULL) {
		MessageBox(NULL, "LocalMemory Low", "HDBMakeNewSheet", MB_OK);
		return NULL;
	}

	memset(sheet, 0, sizeof(HDB_SHEET_STRUCT));

	strcpy(sheet->sName, sheet_name);
	strcpy(sheet->sDescription, description);
	sheet->blockField = new Block(sizeof(HDB_FIELD_STRUCT));

	if(sheet->blockField == NULL) {
		delete sheet;
		return NULL;
	}

	return sheet;
}

// 모든 메모리를 풀어주고 변수를 초기화 한다.

void HDBFreeSheet(HDB_SHEET_STRUCT *sheet)
{
	HDB_FIELD_STRUCT *field;
	
	for(DWORD l = 0; l < sheet->blockField->GetCount(); l++) {
		field = (HDB_FIELD_STRUCT*)sheet->blockField->GetPtr(l);
		field->blockData->DeleteAllBlock();
		delete field->blockData;
		field->blockData = NULL;
	}

	sheet->blockField->DeleteAllBlock();
	delete sheet->blockField;
	sheet->blockField = NULL;
}

/*

//------------------------------------------
// string 을 복사하기 전에 안전한 크기인가를 알기위해서 검사한다.
// source 는 NULL을 포함한 원본의 크기이고
// target 은 NULL을 포함한 타겟의 크기이다.
// return 은 적당한 크기가 돌아온다.
// 복사할때는 다음과 같이 한다.
// strncpy( t, s, return);
// t[return] = 0;
//------------------------------------------

int SeekFitCopyLimit(int target, int source)
{
	if(target <= 0)	return 0;
	if(source <= 0)	return 0;

	if(target < source)	return target-1;
	else						return source-1;
}

int HDBGetSheetName(int pos, char *name, int limit)
{
	name[0] = 0;
	if(pos < 0 || pos >= nSheetHap)	return 0;

	limit = SeekFitCopyLimit(limit, 11);

	strncpy(name, sheetAll[pos].sName, limit);
	name[limit] = 0;

	return 1;
}

int HDBGetAlwaysViewFieldSize(char *sheet_name)
{
	HDB_SHEET_STRUCT *sheet;
	int sheet_pos;

	sheet_pos = HDBSearchSheet(sheet_name);
	if(sheet_pos == -1)	return 7;
	sheet = &sheetAll[sheet_pos];

	if(sheet->wAlwaysViewFieldSize < 3)
		return 7;
	else
		return sheet->wAlwaysViewFieldSize;
}
*/
int HDBSetAlwaysViewFieldSize(HDB_SHEET_STRUCT *sheet, int size)
{
	if(size < 10)		size = 10;
	if(size > 500)		size = 500;

	sheet->wAlwaysViewFieldSize = size;

	return 1;
}

int HDBGetAlwaysViewFieldSize(HDB_SHEET_STRUCT *sheet)
{
	return sheet->wAlwaysViewFieldSize;
}

int HDBSetAlwaysViewField(HDB_SHEET_STRUCT *sheet, char *field_name)
{
	strcpy(sheet->sAlwaysViewField, field_name);

	return 1;
}

int HDBGetAlwaysViewField(HDB_SHEET_STRUCT *sheet, char *field_name)
{
	strcpy(field_name, sheet->sAlwaysViewField);

	return 1;
}




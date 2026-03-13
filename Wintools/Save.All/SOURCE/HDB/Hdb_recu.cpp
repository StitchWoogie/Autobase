#include "stdafx.h"
/*
//------------------------------------------------------------------------------
//	계산식 필드를 계산하는 부분만 모아 놓았다.
//------------------------------------------------------------------------------

#include <stdio.h>
#include <windows.h>
#include <string.h>
#include <math.h>

#include <glib.h>
#include <tools.h>
#include <dataswap.h>

#include "hdb_lib.h"

extern char	 bChangeFlag;

//------------------------------------------------------------------------------
//	스트링 값을 Long Doubld형으로 변환 읽어온다.
//------------------------------------------------------------------------------

static DWORD dwBlockPos4Recurse;		// recurse 함수들이 사용할 수 있도록 블럭의 위치를 알려준다.
static char  bWrongFlag = OFF;		// 함수 작성이 틀렸을 때는 이 플래그를 살려준다.
static WORD  wErrorCode = 0;
static char  bDataNoneFlag = OFF;
static HDB_SHEET_STRUCT *sheetRecurse = NULL;

enum {
	HDB_ERR_FIELD_POS_OVER,
	HDB_ERR_BLOCK_POS_OVER,
	HDB_ERR_RECURSE_RECALL,
	HDB_ERR_LOW_LOCAL_MEMORY,
	HDB_ERR_FIELD_NAME_TO_BIG,
	HDB_ERR_FIELD_NAME_NOT_FOUND,
	HDB_ERR_FUNC_CANT_FIND_END_GALHO_OF_FUNC,
	HDB_ERR_FUNC_CANT_FIND_START_GALHO_OF_FUNC,
};

static void SetErrorCode(WORD code)
{
	bWrongFlag = ON;
	wErrorCode = code;
}

//------------------------------------------------------------------------------
//	데이타를 직접 기록하는 데이타 입력필드만 사용하는 함수
//	데이타가 숫자나 . + - 등이 맞는가 검사한다.
//------------------------------------------------------------------------------

static long double GetCellFloatDataField(HDB_FIELD_STRUCT huge *field)
{
	StackChar stack(1000);

	if(stack.data == NULL) {
		SetErrorCode(HDB_ERR_LOW_LOCAL_MEMORY);
		return 0;
	}

	BYTE huge *buf;

	buf = (BYTE huge *) GlobalLock(field->hBlock);
	memcpy(stack.data, buf+field->wSizeFitBlock*dwBlockPos4Recurse, field->wSize);
	stack.data[field->wSize] = 0;
	GlobalUnlock(field->hBlock);

	if(stack.data[0] == 0) {
		bDataNoneFlag = ON;
		return 0;
	}
	else {
		char number_flag = OFF;
		for(unsigned i = 0; i < strlen(stack.data); i++) {
			if(stack.data[i] >= '0' && stack.data[i] <= '9') {
				number_flag = ON;
				continue;
			}
			if(stack.data[i] == '.')							continue;
			if(stack.data[i] == ',')							continue;
			if(stack.data[i] == '-')							continue;
			if(stack.data[i] == '+')							continue;
			bDataNoneFlag = ON;
			break;
		}
		if(number_flag == OFF) {
			bDataNoneFlag = ON;
		}
	}

	return(atof(stack.data));
}

long double CalculateFunctionRecurse(char *s, int size);

static long double GetCellFloat(int field_pos)
{
	if(field_pos >= sheetRecurse->wFieldHap)	{
		SetErrorCode(HDB_ERR_FIELD_POS_OVER);
		return 0;
	}
	if(dwBlockPos4Recurse >= sheetRecurse->dwBlockHap) {
		SetErrorCode(HDB_ERR_BLOCK_POS_OVER);
		return 0;
	}

	HDB_FIELD_STRUCT huge *field = &sheetRecurse->fieldAll[field_pos];

	if(field->wType == HDB_FIELD_TYPE_FUNCTION) {
		if(field->bRecurseFlag) {
			SetErrorCode(HDB_ERR_RECURSE_RECALL);
			return 0;
		}
		StackChar stack(1000);
		BYTE huge *buf;

		if(stack.data != NULL) {
			buf = (BYTE huge *) GlobalLock(field->hBlock);
			memcpy(stack.data, buf, field->wSizeFitBlock);
			stack.data[field->wSizeFitBlock] = 0;
			GlobalUnlock(field->hBlock);

			return CalculateFunctionRecurse(stack.data, field->wSizeFitBlock);
		}
		return 0;
	}
	else {
		return GetCellFloatDataField(field);
	}
}

static long double GetCellFloatByName(char *field_name, int size)
{
	if(size > 10) {
		SetErrorCode(HDB_ERR_FIELD_NAME_TO_BIG);
		return 0;
	}

	HDB_FIELD_STRUCT huge *field;
	int i;

	for(i = 0; i < sheetRecurse->wFieldHap; i++) {
		field = &sheetRecurse->fieldAll[i];
		if(strncmp(field_name, field->sName, size) == 0) {
			return GetCellFloat(i);
		}
	}

	SetErrorCode(HDB_ERR_FIELD_NAME_NOT_FOUND);
	return 0;
}

static long double StringToFloat(char *string, int size)
{
	StackChar stack(size+1);

	if(stack.data == NULL) {
		SetErrorCode(HDB_ERR_LOW_LOCAL_MEMORY);
		return 0.0;
	}
	strncpy(stack.data, string, size);
	stack.data[size] = 0;

	return atof(stack.data);
}

static long double ElseFunction(char *s, int size);

//------------------------------------------------------------------------------
//	    s는 1234,@cell(),35  식으로 들어온다.
//------------------------------------------------------------------------------

long double GetAverage(char *s, int size)
{
	long double value = 0;
	int  member_count = 0;
	int  hap;
	int  old_pos;
	char flag_save = bDataNoneFlag;
	long double retn;
	int i;

	bDataNoneFlag = OFF;
	hap = 0;
	old_pos = 0;

	for(i = 0; i < size; i++) {
		if(s[i] == ',') {
			if(hap > 0) {
				retn = ElseFunction(&s[old_pos], hap);
				if(bDataNoneFlag == OFF) {	// 알맞은 데이타가 계산되었다.
					member_count++;
					value += retn;
				}
				else {	// 아직 사용자가 입력하지 않은 상태이다. 빈칸도 0으로 치지 않고 입력하지 않은것으로 나옴
					bDataNoneFlag = OFF;
				}
			}
			hap = 0;
			old_pos = i+1;
		}
		else {
			hap++;
		}
	}
	if(hap > 0) {
		retn = ElseFunction(&s[old_pos], hap);
		if(bDataNoneFlag == OFF) {	// 알맞은 데이타가 계산되었다.
			member_count++;
			value += retn;
		}
	}

	bDataNoneFlag = flag_save;

	if(member_count == 0) {
		bDataNoneFlag = ON;
		return 0.0;
	}
	else {
		return value/member_count;
	}
}

//------------------------------------------------------------------------------
//	( 가 시작하는 위치와 ) 가 닫히는 위치를 찾는다.
//------------------------------------------------------------------------------

static int SeekStartEnd(char *s, int size, int &start_pos, int &end_pos)
{
	int i;
	int open = 0;
	int close = 0;

	for(i = 1; i < size; i++) {
		if(s[i] == '(') {
			if(open == 0) {
				start_pos = i;
			}
			open++;
		}
		else if(s[i] == ')') {
			end_pos = i;
			close++;

			if(open < close) {
				SetErrorCode(HDB_ERR_FUNC_CANT_FIND_START_GALHO_OF_FUNC);
				return 0;	// 시작하는 ( 를 찾지 못했다.
			}
			else if(open == close) {
				return 1;
			}
		}
		else;
	}
	SetErrorCode(HDB_ERR_FUNC_CANT_FIND_END_GALHO_OF_FUNC);
	return 0;	// 끝나는 ')'를 찾지 못했다.
}

//------------------------------------------------------------------------------
//	하나의 요소만 남았을 때 숫자이거나 함수일때.
//	(예) @cell(ex), 12345, 12.3, @average 등등
//------------------------------------------------------------------------------

long double ElseFunction(char *s, int size)
{
	if(bWrongFlag == ON)	return 0;		// 함수 이상

	int start_pos, end_pos;
	int name_size;

	if(s[0] == '@') {	// 함수.
		if(!SeekStartEnd(s, size, start_pos, end_pos))	return 0;

		name_size = start_pos-1;
		if(name_size < 1)		return 0;
		if(name_size > 900)	return 0;

		StackChar func_name(1000);
		if(func_name.data == NULL)	return 0;

		strncpy(func_name.data, &s[1], name_size);
		func_name.data[name_size] = 0;

		if(end_pos-start_pos < 2)	return 0;	// 인자값이 하나도 없다.

		if(strcmp(func_name.data, "cell") == 0) {	// cell 값을 없는다.
			return(GetCellFloatByName(&s[start_pos+1], end_pos-start_pos-1));
		}
		else if(strcmp(func_name.data, "average") == 0) {
			return(GetAverage(&s[start_pos+1], end_pos-start_pos-1));
		}
		else if(strcmp(func_name.data, "sqrt") == 0) {
			return sqrt(CalculateFunctionRecurse(&s[start_pos+1], end_pos-start_pos-1));
		}
      else if(strcmp(func_name.data, "abs") == 0) {
			return fabsl(CalculateFunctionRecurse(&s[start_pos+1], end_pos-start_pos-1));
		}
		else {
			return(StringToFloat(s, size));
		}
	}
	else {
		return(StringToFloat(s, size));
	}
}

long double CalculateFunctionRecurse(char *s, int size)
{
	if(bWrongFlag == ON)	return 0;		// 함수 이상

	if(size <= 0)	return 0.0;

	int  open_count = 0;
	int  close_count = 0;
	int  pos = 0;

	//--------------------
	// 명령어 @cell(field)
	//--------------------

	while(1) {
		if(s[pos] == '(') {
			open_count++;
		}
		else if(s[pos] == ')') {
			close_count++;
		}
		else if(s[pos] == '+') {
			if(open_count == close_count) {
				if(pos < 1 || pos >= size-1) {
					return 0.0;
				}
				else {
					return(CalculateFunctionRecurse(s, pos)+CalculateFunctionRecurse(&s[pos+1], size-pos-1));
				}
			}
		}
		else if(s[pos] == '-') {
			if(open_count == close_count) {
				if(pos == 0) {
					return(0-CalculateFunctionRecurse(&s[pos+1], size-pos-1));
				}
				else if(pos >= size-1) {
					return 0.0;
				}
				else {
					return(CalculateFunctionRecurse(s, pos)-CalculateFunctionRecurse(&s[pos+1], size-pos-1));
				}
			}
		}
		else;

		pos ++;

		if(pos >= size) {
			break;
		}
	}

	open_count = 0;
	close_count = 0;
	pos = 0;

	while(1) {
		if(s[pos] == '(') {
			open_count++;
		}
		else if(s[pos] == ')') {
			close_count++;
		}
		else if(s[pos] == '*') {
			if(open_count == close_count) {
				if(pos < 1 || pos >= size-1) {
					return 0.0;
				}
				else {
					return(CalculateFunctionRecurse(s, pos)*CalculateFunctionRecurse(&s[pos+1], size-pos-1));
				}
			}
		}
		else if(s[pos] == '/') {
			if(open_count == close_count) {
				if(pos < 1 || pos >= size-1) {
					return 0.0;
				}
				else {
					long double down = CalculateFunctionRecurse(&s[pos+1], size-pos-1);
					if(down == 0) {	// protected divice by 0
						return 0.0;
					}
					else {
						return(CalculateFunctionRecurse(s, pos)/down);
					}
				}
			}
		}
		else;

		pos ++;

		if(pos >= size) {
      	if(s[0] == '(' && s[size-1] == ')') {
				return CalculateFunctionRecurse(&s[1], size-2);
			}
			else {
				return ElseFunction(s, size);
			}
		}
	}
}

void RecurseInit(HDB_SHEET_STRUCT *sheet, DWORD block)
{
	sheetRecurse = sheet;
	dwBlockPos4Recurse = block;
	bWrongFlag = OFF;
	bDataNoneFlag = OFF;
	int i;
	for(i = 0; i < sheetRecurse->wFieldHap; i++) {
		sheetRecurse->fieldAll[i].bRecurseFlag = OFF;		// 각 필드들이 서로 중복 Recurse가 되지 않도록 한번 들어간 함수는 Flag를 ON 시켜준다.
	}
}

int IsRecurseWrong()
{
	return bWrongFlag;
}

int IsDataNone()
{
	return bDataNoneFlag;			// 데이타를 입력하지 않았다.
}

*/




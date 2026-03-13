#if	!defined (__HDB_LIB_H)
#define	__HDB_LIB_H

#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

typedef struct {
	DWORD	dwVersion;
	WORD	wSheetHap;
	BYTE	extra[120];
	WORD	crc;
} HDB_MAIN_HEADER;		// total 128 byte

typedef struct {
	char 	sName[21];
	WORD	wType;
	WORD	wSize;
	WORD	wViewSize;			// 화면에 보이는 글자수
	char	cDisplayFormMinor;	// 소수점 이하의 자리수.
	char	bRecurseFlag;		// recurse over flow check
	Block	*blockData;			// data가 들어 있는 블럭.
} HDB_FIELD_STRUCT;

typedef struct {
	char	sName[11];
	char	sDescription[21];
	DWORD	dwDataHap;
	WORD	wAlwaysViewFieldSize;
	char    sAlwaysViewField[21];

	Block	*blockField;
	char	bChangeFlag;			// sheet가 갱신되었느냐?
} HDB_SHEET_STRUCT;		// 각 Sheet 의 헤더

typedef struct {
	DWORD	dwVersion;
	WORD	wFieldCount;
	DWORD	dwBlockCount;
	WORD	crc;
} HDB_OLD_MAIN_HEADER;

enum {
	HDB_FIELD_TYPE_STRING 	= 0,
	HDB_FIELD_TYPE_FUNCTION = 1,
	HDB_FIELD_TYPE_INT		= 2,
	HDB_FIELD_TYPE_WORD		= 3,
	HDB_FIELD_TYPE_FLOAT		= 4,
};

#define	HDB_MAX_FIELD_COUNT	200
#define	HDB_MAX_STRING_SIZE	1000

/*

int HDBLoad(HWND hwnd, char *filename);
int HDBSave(char *filename);
void HDBFreeAll();				// 모든 메모리를 풀어주고 변수를 초기화 한다.
int HDBInsertField(char *sheet_name, HDB_FIELD_STRUCT *field, int pos, char *function);
int HDBGetFieldName(char *sheet_name, char *filedname, int pos);


int HDBDeleteField(char *sheet_name, int pos);


int HDBAddNewBlock(char *sheet_name);
int HDBGetCell(char *sheet_name, int field_pos, DWORD block_pos, char *string, int limit);
int HDBSetCell(char *sheet_name, int field_pos, DWORD block_pos, char *string);

int HDBGetCellByName(char *sheet_name, char *field, DWORD block_pos, char *string, int limit);
int HDBSetCellByName(char *sheet_name, char *field, DWORD block_pos, char *string);

int HDBGetFieldFunction(char *sheet_name, int field_pos, char *function);
int HDBSetFieldFunction(char *sheet_name, int field_pos, char *function);

int HDBIsChanged();

void HDBSetFieldViewSize(char *sheet_name, int field_pos, WORD size);
*/

int HDBSetAlwaysViewFieldSize(HDB_SHEET_STRUCT *sheet, int size);
int HDBGetAlwaysViewFieldSize(HDB_SHEET_STRUCT *sheet);
int HDBSetAlwaysViewField(HDB_SHEET_STRUCT *sheet, char *field_name);
int HDBGetAlwaysViewField(HDB_SHEET_STRUCT *sheet, char *field_name);

int   HDBAddNewBlock(HDB_SHEET_STRUCT *sheet);
int   HDBInsertNewBlock(HDB_SHEET_STRUCT *sheet, DWORD pos);
int   HDBDeleteOneBlock(HDB_SHEET_STRUCT *sheet, DWORD pos);

void  HDBFieldMakeDefault(HDB_FIELD_STRUCT *field);
int   HDBFieldAdd(HDB_SHEET_STRUCT *sheet, HDB_FIELD_STRUCT *field, char *function);
DWORD HDBGetBlockHap(HDB_SHEET_STRUCT *sheet);
DWORD HDBGetFieldHap(HDB_SHEET_STRUCT *sheet);

int	HDBGetField(HDB_SHEET_STRUCT *sheet, DWORD pos, HDB_FIELD_STRUCT *filed);
int	HDBSetField(HDB_SHEET_STRUCT *sheet, DWORD pos, HDB_FIELD_STRUCT *filed);
int	HDBGetCell(HDB_SHEET_STRUCT *sheet, DWORD field_pos, DWORD block_pos, char *string, int limit);
int HDBSetCell(HDB_SHEET_STRUCT *sheet, DWORD field_pos, DWORD block_pos, char *string);

DWORD HDBSearchField(HDB_SHEET_STRUCT *sheet, char *field);		// -1 필드가 없다.
//int HDBSearchSheet(char *sheet_name);					// -1 Sheet가 없다.

//int HDBChangeFieldName(char *sheet_name, int field_pos, char *field_name);
//int HDBChangeFieldDescription(char *sheet_name, int field_pos, char *field_des);

HDB_SHEET_STRUCT *HDBMakeNewSheet(char *sheet_name, char *sheet_des);
void HDBFreeSheet(HDB_SHEET_STRUCT *sheet);				// 모든 메모리를 풀어주고 변수를 초기화 한다.

/*
int HDBGetSheetHap();
int HDBGetSheetName(int pos, char *name, int limit);

int HDBGetAlwaysViewFieldSize(char *sheet_name);
void HDBSetAlwaysViewFieldSize(char *sheet_name, int size);

int HDBGetAlwaysViewField(char *sheet_name, char *field_name);
int HDBSetAlwaysViewField(char *sheet_name, char *field_name);
*/


#endif


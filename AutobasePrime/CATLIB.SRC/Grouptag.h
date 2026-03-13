// TAG size O.K
#if	!defined (__GroupTag_H)
#define __GroupTag_H

#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

#pragma pack(push, 1)

typedef struct {
	WORD	struct_size;
	char	tag[40];
	char	description[80];
	char	cTagType;
} GFORM_MEMBER_STRUCT;

// 이 구조체는 정보를 보기 위해서만 존재한다.
typedef struct {
	char title[40];
	char description[80];
} GFORM_INFO_STRUCT;

typedef struct {
	WORD struct_size;
	char title[40];
	char description[80];
	Block *blockMember;
} GFORM_STRUCT;

void GroupTagFormLoad(Block *block);
void GroupTagFormLoadOne(char *file, GFORM_STRUCT *form);
void GroupTagFormInfoLoad(Block *block);
void GroupTagFormFree(GFORM_STRUCT *form);

//------------------------------------------------------------------------------
//	Group Tag에서 에디터와 View에서 공통으로 쓰이는 부분만 모아 놓았다.
//------------------------------------------------------------------------------

#define MAX_GROUP_TAG_DO	100	// 최대 설정할 수 있는 DO의 그룹갯수

typedef struct {		// Group Tag struct
	char  tag[40];
	char  description[80];
	char  act;
	int   hap;
	Block *member;
} TAG_GDO_STRUCT;

typedef struct {
	char tag[40];		// 그룹속에 포함되는 요소.
//	int  pos;			// tag pos
//	char flag;			// if flag is off then this tag not exist
} GROUP_MEMBER_STRUCT;

extern Block blockGroupDO;

void GroupTagLoad(HWND hwnd);
void GroupTagFree();
void GroupTagFillListBox(HWND hwndList);
void GroupTagFillMemberListBox(HWND hwndList, DWORD group_pos);

#pragma pack(pop)

#endif



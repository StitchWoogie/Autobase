// TAG size O.K
//------------------------------------------------------------------------------
//	Group Tag에서 에디터와 View에서 공통으로 쓰이는 부분만 모아 놓았다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <string.h>
#include <stdio.h>
#include <stdlib.h>

#include <glib.h>
#include <tools.h>
#include <dataswap.h>

#include "cattag.h"
#include "grouptag.h"
#include "totalcfg.h"

Block blockGroupDO(sizeof(TAG_GDO_STRUCT));

static void LoadGroupTagDO(HWND hwnd)
{
	char filename[MAXPATH];
	FILE *in;
	int  num;
	FileCommaBlock comma;
	TAG_GDO_STRUCT grptag;
	GROUP_MEMBER_STRUCT member;
	StackChar workdir(MAXPATH);

	AutoBaseIniGetProjectDirectory(workdir.data);

	sprintf(filename, "%s\\tag\\do-group.tag", workdir.data);

	in = fopen(filename, "rb");
	if(in == NULL) {
		return;
	}
	comma.SetFile(in);
	while(1) {
		memset(&grptag, 0, sizeof(TAG_GDO_STRUCT));
		grptag.member = new Block(sizeof(GROUP_MEMBER_STRUCT));
		if(grptag.member == NULL) {
			MessageBox(hwnd, "can't alloc group tag", "memory insuffiecnt", MB_OK);
			break;
		}
		comma.GetInt(num);
		comma.GetString(grptag.tag, sizeof(grptag.tag));
		KillEndSpace(grptag.tag);
		if(comma.IsEOF()) {
			delete grptag.member;
			break;
		}
		comma.GetString(grptag.description, sizeof(grptag.description));
		comma.GetChar(grptag.act);

		while(1) {
			memset(&member, 0, sizeof(GROUP_MEMBER_STRUCT));
			comma.GetString(member.tag, sizeof(member.tag));
			if(member.tag[0] != 0) {
				grptag.member->AddBlock(&member);
			}

			if(comma.IsEOF())	break;			// 파일이 끝남
			if(comma.IsEOL())	break;					// 라인이 끝남
		}

		blockGroupDO.AddBlock(&grptag);
      if(comma.IsEOF())	break;			// 파일이 끝남

		comma.NewLine();	// 다음라인으로 이동한다.
	}
	fclose(in);
}

void GroupTagLoad(HWND hwnd)
{
	GroupTagFree();
	LoadGroupTagDO(hwnd);
}

void GroupTagFree()
{
	DWORD l;
	TAG_GDO_STRUCT grptag;

	for(l = 0; l < blockGroupDO.GetCount(); l++) {
		blockGroupDO.GetBlock(&grptag, l);
		if(grptag.member) {
			delete grptag.member;
		}
		blockGroupDO.SetBlock(&grptag, l);
	}

	blockGroupDO.DeleteAllBlock();
}

//------------------------------------------------------------------------------
//	그룹을 리스트 박스에 집어넣는다.
//------------------------------------------------------------------------------

void GroupTagFillListBox(HWND hwndList)
{
	TAG_GDO_STRUCT grp;
	char buf[80];
	DWORD l;

	SendMessage(hwndList, LB_RESETCONTENT, 0, 0L);

	for(l = 0; l < blockGroupDO.GetCount(); l++) {
		blockGroupDO.GetBlock(&grp, l);
		sprintf(buf, "%s\t%s", grp.tag, grp.description);
		SendMessage(hwndList, LB_ADDSTRING, 0, (LONG)buf);
	}
}

//------------------------------------------------------------------------------
//	그룹에 포함된 멤버를 ListBox에 넣는다.
//------------------------------------------------------------------------------

void GroupTagFillMemberListBox(HWND hwndList, DWORD group_pos)
{
	if(group_pos >= blockGroupDO.GetCount())	return;

	TAG_GDO_STRUCT grp;

	blockGroupDO.GetBlock(&grp, group_pos);
	if(grp.member == NULL)	return;

	SendMessage(hwndList, LB_RESETCONTENT, 0, 0L);

	char buf[80];
	DWORD l;
	GROUP_MEMBER_STRUCT member;

	for(l = 0; l < grp.member->GetCount(); l++) {
		grp.member->GetBlock(&member, l);
		sprintf(buf, "%s", member.tag);
		SendMessage(hwndList, LB_ADDSTRING, 0, (LONG)buf);
	}
}




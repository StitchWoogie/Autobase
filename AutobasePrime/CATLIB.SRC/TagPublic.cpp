// TAG size OK
// 이함수는 TagShare와 TagLoad에서 같이 사용하는 함수이다.

#include "stdafx.h"
#include "cattag.h"
#include "GroupTag.h"

int GetTagPosAI(TERMINAL_STRUCT *ter, const char *tag_org, short &pos)
{
	int i;
	char tag[80];

	strcpy(tag, tag_org);
	KillEndSpace(tag);

	for(i = 0; i < ter->nAnalogInputHap; i++) {
		if(strcmp(tag, ter->analogInput[i].file.tag) == 0) {
			pos = i;
			return 1;
		}
	}
	pos = 0;
	return 0;
}

void GetTagNameDescription(int tag_type, short tag_pos, CString &tag_name, CString &tag_des)
{
	if(tag_pos < 0) {
		tag_name = "TagPos<0";
		tag_des  = "TagPos<0";
	}
	else {
		if(tag_type == TAG_TYPE_AI) {
			TAG_AI_STRUCT *ai = &terminalStruct[0].analogInput[tag_pos];

			tag_name = ai->file.tag;
			tag_des  = ai->file.description;
		}
		else if(tag_type == TAG_TYPE_AO) {
			TAG_AO_STRUCT *ao = &terminalStruct[0].analogOutput[tag_pos];

			tag_name = ao->file.tag;
			tag_des  = ao->file.description;
		}
		else if(tag_type == TAG_TYPE_DI) {
			TAG_DI_STRUCT *di = &terminalStruct[0].digitalInput[tag_pos];

			tag_name = di->file.tag;
			tag_des  = di->file.description;
		}
		else if(tag_type == TAG_TYPE_DO) {
			TAG_DO_STRUCT *dout = &terminalStruct[0].digitalOutput[tag_pos];

			tag_name = dout->file.tag;
			tag_des  = dout->file.description;
		}
		else if(tag_type == TAG_TYPE_ST) {
			TAG_ST_STRUCT *st = &terminalStruct[0].stringTag[tag_pos];

			tag_name = st->file.tag;
			tag_des  = st->file.description;
		}
		else if(tag_type == TAG_TYPE_GDO) {
			TAG_GDO_STRUCT grp;

			blockGroupDO.GetBlock(&grp, tag_pos);

			tag_name = grp.tag;
			tag_des  = grp.description;
		}
		else {
			tag_name = "Error Type";
			tag_des  = "Error Type";
		}
	}
}
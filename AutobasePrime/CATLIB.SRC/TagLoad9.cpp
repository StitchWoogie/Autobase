// TAG size O.K
//#include "stdafx.h"

#include <afxwin.h>
#include <stdio.h>

#include <tools.h>
#include <dataswap.h>

#include "CatTag.h"
#include "CatTag9.h"
#include "totalcfg.h"

Block blockTagList9(sizeof(TagPublicStruct));

//int TextGetOneLineFromUTF8(FILE *in, CString &buf);

void TagLoad9()
{
	CString filename;
	FILE *in;
	CString buf;
	CString work_dir;
	TagPublicStruct tp;
	CommaBlockString comma;

	blockTagList9.DeleteAllBlock();
	
	AutoBaseIniGetProjectDirectory(work_dir);

	filename.Format("%s\\TAG\\Local.tagx", work_dir);

	in = fopen(filename, "rb");
	if(in == NULL) 	return;

	while(1) {
		if(!TextGetOneLineFromUTF8(in, buf)) 	break;
		if(buf[0] == ';')	continue;
		if(strlen(buf) == 0)	continue;

		memset(&tp, 0, sizeof(TagPublicStruct));

		comma.Set(buf);
		comma.GetString(buf);

		if(buf == "AI")			tp.nTagType = TAG_TYPE_AI;
		else if(buf == "AO")	tp.nTagType = TAG_TYPE_AO;
		else if(buf == "DI")	tp.nTagType = TAG_TYPE_DI;
		else if(buf == "DO")	tp.nTagType = TAG_TYPE_DO;
		else if(buf == "ST")	tp.nTagType = TAG_TYPE_ST;
		else					continue;		// Group같은 것은 그냥 Skip 한다.

		comma.GetString(tp.tag, sizeof(tp.tag));
		comma.GetString(tp.description, sizeof(tp.description));
		tp.tag_pos = blockTagList9.GetCount();

		blockTagList9.AddBlock(&tp);
	}
	fclose(in);
}
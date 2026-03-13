// TAG size O.K
#include "stdafx.h"
#include <stdio.h>

#include <tools.h>
#include <dataswap.h>

#include "../../CATLIB.SRC/CatTag.h"
#include "../../CATLIB.SRC/CatTag9.h"
#include "../../CATLIB.SRC/totalcfg.h"

Block blockTagList9(sizeof(TagPublicStruct));



//int TextGetOneLineFromUTF8(FILE *in, CStringW &buf);

int TextGetOneLineFromUTF8(FILE *in, CStringW &buf)// modify CString 을 CStringW로 2012-02-17
{
	int fc;
	buf = "";
	int remain = 0;
	wchar_t val = 0;
	CStringW imsi;

	while( (fc = fgetc(in)) != EOF) {
		if(fc == 0x0d || fc == 0x0A) {                     // 개행 문자
			if(fc == 0x0d)	fgetc(in);
			return 1;
		}
		else {   			
			if(remain) {
				val = (val << 6)|(fc&0x3F);
				remain--;
				if(remain == 0) {
					imsi = val;
					buf += imsi;
				}
			}
			else {
				if(fc < 0x80) {	// 
					buf += (char)fc;
				}
				else if((fc & 0xF0) == 0xF0) {
					remain = 3;
					val = fc & 0x07;
				}
				else if((fc & 0xE0) == 0xE0) {
					remain = 2;
					val = fc & 0x0F;
				}
				else if((fc & 0xC0) == 0xC0) {
					remain = 1;
					val = fc & 0x1F;
				}
				else {
					buf += (char)fc;
				}
			}
		}
	}
	if(wcslen(buf) == 0)	return 0;

	return 1;
}



void TagLoad9()
{
	CString filename;
	FILE *in;
	CString buf;
	CStringW buf2;
	CString work_dir;
	TagPublicStruct tp;
	CommaBlockString comma;
	//tCommaBlockString comma;

	blockTagList9.DeleteAllBlock();
	
	AutoBaseIniGetProjectDirectory(work_dir);

	filename.Format("%s\\TAG\\Local.tagx", (const char*)work_dir);

	in = fopen(filename, "rb");
	if(in == NULL) 	return;

	while(1) {
		if(!TextGetOneLineFromUTF8(in, buf2)) 	break;
		if(buf[0] == ';')	continue;
		if(wcslen(buf2) == 0)	continue;

		memset(&tp, 0, sizeof(TagPublicStruct));

		//wcstombs(buf.GetBuffer(), buf2.GetBuffer(), wcslen(buf2));//LPSTR(LPCTSTR(buf))
		buf = buf2;

		//comma.Set((LPCTSTR)buf2);
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

		blockTagList9.AddBlock(&tp);
	}
	fclose(in);
}

/*void TagLoad9()
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

		blockTagList9.AddBlock(&tp);
	}
	fclose(in);
}*/
#include "stdafx.h"
#include <stdio.h>

#include <tools.h>

MemoryTextReader :: MemoryTextReader(BYTE *buffer, int size)
{
	pBuffer = buffer;
	nSize = size;
	nPos = 0;
}

int MemoryTextReader :: Getc()
{
	if(nPos >= nSize)	return -1;

	return pBuffer[nPos++];
}

bool MemoryTextReader :: ReadLineByUTF8(CString &buf)
{
	int fc;
	buf = "";
	int remain = 0;
	wchar_t val = 0;
	CString imsi;

	while( (fc = Getc()) != EOF) {
		if(fc == 0x0d || fc == 0x0A) {                     // 개행 문자
			if(fc == 0x0d)	Getc();
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
	if(_tcslen(buf) == 0)	return 0;

	return 1;
}
/*
//---------------------------------------------------------------------------
//	Get One Line In Text File
//	must open "rb"
//---------------------------------------------------------------------------

int TextGetOneLine(FILE *in, char *buf, int limit)
{
	int fc;
	int bufpos = 0;

	while( (fc = fgetc(in))  != EOF) {
		if(fc == 0x0d || fc == 0x0A) {                     // 개행 문자
			if(fc == 0x0d)	fgetc(in);
			buf[bufpos] = 0;
			return 1;
		}
		else {   			//
			if(bufpos < limit-1)
			buf[bufpos++] = fc;
		}
	}
	if(bufpos == 0)	return 0;

	buf[bufpos] = 0;

	return 1;
}

// UNICODE로 읽을 때는 fopen을 "r, ccs=UNICODE"로 해야 한다. rb는 안됨
int TextGetOneLineW(FILE *in, WCHAR *buf, int limit)
{
	wint_t fc;
	int bufpos = 0;

	while( (fc = fgetwc(in))  != WEOF) {
		if(fc == 0x0d || fc == 0x0A) {                     // 개행 문자
			//if(fc == 0x0d)	r로 읽으면 이부분은 발생하지 않는다.
				//fgetc(in);
			buf[bufpos] = 0;
			return 1;
		}
		else {   			//
			if(bufpos < limit-1)
			buf[bufpos++] = fc;
		}
	}
	if(bufpos == 0)	return 0;

	buf[bufpos] = 0;

	return 1;
}

int TextGetOneLine(FILE *in, CString &buf)
{
	int fc;
	buf = "";

	while( (fc = fgetc(in))  != EOF) {
		if(fc == 0x0d || fc == 0x0A) {                     // 개행 문자
			if(fc == 0x0d)	fgetc(in);
			return 1;
		}
		else {   			//
			buf += (TCHAR)fc;
		}
	}
	if(_tcslen(buf) == 0)	return 0;

	return 1;
}

// UTF8은 EF BB BF (0xFEFF) 가 첫줄에 있을 수도 있고 없을 수도 있다.
int TextGetOneLineFromUTF8(FILE *in, CString &buf)
{
	int fc;
	buf = "";
	int remain = 0;
	wchar_t val = 0;
	CString imsi;

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
	if(_tcslen(buf) == 0)	return 0;

	return 1;
}*/
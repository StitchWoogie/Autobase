//------------------------------------------------------------------------------
//	아래는 파일에서 콤마로 분리되는 블럭을 읽어오기 위한 클래스다.
// 첫줄이 ; 이면 그줄은 무시한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdlib.h>

#include <tools.h>

FileCommaBlock :: FileCommaBlock()
{
	in = NULL;
	flag_eof = OFF;
	flag_endline = OFF;
	flag_read_first_column = OFF;
}

void FileCommaBlock :: SetFile(FILE *init)
{
	in = init;
	flag_eof = OFF;
	flag_endline = OFF;
	flag_read_first_column = OFF;
}

//------------------------------------------------------------------------------
//	다음 라인으로 넘어갈 수 있게 해준다.
//------------------------------------------------------------------------------

void FileCommaBlock :: NewLine()
{
	if(flag_eof)	return;		// 더이상 읽을 파일이 없다.

	int ch;

	if(flag_endline == OFF) {	// 아직 라인이 끝나지 않았으므로 다음 라인을 계속해서 찾는다.
		while(1) {
			ch = fgetc(in);
			if(ch == EOF) {
				flag_eof = ON;
				break;
			}
			if(ch == '\n')	break;
		}
	}
	flag_endline = OFF;
	flag_read_first_column = OFF;
}

void FileCommaBlock :: GetString(char *string, int max)
{
	memset(string, 0, max);

	if(flag_eof)		return;
	if(flag_endline)	return;

	int ch;
	int count = 0;

	while(1) {
		ch = fgetc(in);
		if(ch == EOF) {
			flag_eof = ON;
			break;
		}
		if(ch == '\n')	{	// new line
			flag_endline = ON;
			break;
		}

		if(flag_read_first_column == OFF && count == 0 && ch == ';') {
			NewLine();
         continue;
		}

		if(ch == 32 && count == 0)		continue;	// 첫번째 빈칸이 있으면 무시한다.
		if(ch == '\t' && count == 0)	continue;	// 첫번째 탭이 있으면 무시한다.

		if(ch == '\r')	continue;	// ignore
		if(ch == ',')	break;		// O.K Get One Block

		if(count < max-1) {			// string zone check
			string[count] = ch;
			count++;
		}
	}
	string[count] = 0;
	flag_read_first_column = ON;
}

void FileCommaBlock :: GetChar(char &value)
{
	char buf[10];

	GetString(buf, sizeof(buf));
	value = atoi(buf);
}

void FileCommaBlock :: GetInt(int &value)
{
	char buf[10];

	GetString(buf, sizeof(buf));
	value = atoi(buf);
}

void FileCommaBlock :: GetLong(long &value)
{
	char buf[10];

	GetString(buf, sizeof(buf));
	value = atol(buf);
}
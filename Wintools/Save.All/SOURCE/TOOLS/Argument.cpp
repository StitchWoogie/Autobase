#include "stdafx.h"
#include <compiler.hpp>
#include <tools.h>

CommandLineClass :: CommandLineClass()
{
	sCmdLine = NULL;
	nStringHap = 0;
	nStringCurr = 0;
}

CommandLineClass :: ~CommandLineClass()
{
	if(sCmdLine != NULL) {
		delete sCmdLine;
		sCmdLine = NULL;
	}
}

void CommandLineClass :: Set(char *buf)
{
	if(sCmdLine != NULL) {
		delete sCmdLine;
		sCmdLine = NULL;
	}

	nStringHap = 0;
	nStringCurr = 0;

	sCmdLine = new char[strlen(buf)+1];
	if(sCmdLine == NULL)	return;

	strncpy(sCmdLine, buf, strlen(buf));
	sCmdLine[strlen(buf)] = 0;
	nStringHap = strlen(sCmdLine);
}

int CommandLineClass :: GetCommand(char *buf, int limit)
{
	int  count = 0;
	bool open_flag = false;

	if(nStringHap == 0)				return 0;
	if(nStringCurr >= nStringHap)	return 0;		// end

	while(1) {
		//for(i = nStringCurr; i < nStringHap; i++) {
		if(count == 0 && sCmdLine[nStringCurr] == '"') {	// space가 포함되는 문자열
			open_flag = true;
		}
		else {
			if(open_flag) {
				if(sCmdLine[nStringCurr] == '"') {
					nStringCurr++;
					buf[count] = 0;
					return 1;
				}
				else {
					if(count < limit-1) {
						buf[count] = sCmdLine[nStringCurr];
						count++;
					}
				}
			}
			else {
				if(sCmdLine[nStringCurr] == NULL ||
					sCmdLine[nStringCurr] == 32 ||
					sCmdLine[nStringCurr] == ',' ||
					sCmdLine[nStringCurr] == '\t') {
					
					if(count > 0) {
						nStringCurr++;
						buf[count] = 0;
						return 1;
					}
				}
				else {
					if(count < limit-1) {
						buf[count] = sCmdLine[nStringCurr];
						count++;
					}
				}
			}
		}

		nStringCurr++;

		if(nStringCurr > nStringHap)	return 0;
	}
}

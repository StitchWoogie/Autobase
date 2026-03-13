// TAG size O.K
// english O.K
#include "stdafx.h"
#include <tools.h>

GetLastErrorClass :: GetLastErrorClass()
{
	lpMsgBuf = NULL;
}

GetLastErrorClass :: ~GetLastErrorClass()
{
	if(lpMsgBuf)	LocalFree( lpMsgBuf );
	lpMsgBuf = NULL;
}

char* GetLastErrorClass :: GetString()
{
	if(lpMsgBuf) LocalFree( lpMsgBuf );
	lpMsgBuf = NULL;

	DWORD error_code = GetLastError();

    FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
    NULL,
    error_code,
    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
    (LPTSTR) &lpMsgBuf,
    0,
    NULL 
	);
	
	return lpMsgBuf;
}

char* GetLastErrorClass :: GetString(DWORD error_code)
{
	if(lpMsgBuf) LocalFree( lpMsgBuf );
	lpMsgBuf = NULL;

    FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
    NULL,
    error_code,
    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
    (LPTSTR) &lpMsgBuf,
    0,
    NULL 
	);
	
	return lpMsgBuf;
}

void GetLastErrorClass :: GetErrorString(char *buf, int limit)
{
	char *msg = GetString();

	if(msg == NULL) {
		buf[0] = 0;
		return;
	}

	if(strlen(msg) >= (unsigned)limit) {
		strncpy(buf, msg, limit-1);
		buf[limit-1] = 0;
	}
	else {
		strcpy(buf, msg);
	}
}

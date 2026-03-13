#include "stdafx.h"

#include <tools.h>
#include <crc.hpp>

#include "SharedTag.h"

static HANDLE hSharedTagMapNumber = NULL;
static SHARED_TAG_MAP_NUMBER *pSharedTagMapNumber = NULL;

static HANDLE hSharedTagInfo = NULL;
static SHARED_TAG_MAP_INFO *infoSharedTag = NULL;

static HANDLE hSharedTag = NULL;
static SHARED_TAG_READ *sharedTag = NULL;

static HANDLE hStringMemory = NULL;
static WCHAR *pStringMemory = NULL;

static HANDLE hWriteTag = NULL;
static SHARED_TAG_WRITE *pWriteTag = NULL;

static BYTE cOldMapNumber;

void SharedTagInit()
{
	int size;

	size = sizeof(SHARED_TAG_MAP_NUMBER);
	if(size != 16) 
	{
		MessageBox(NULL, _T("sizeof(SHARED_TAG_MAP_NUMBER) != 16)"), _T("error"), MB_OK);
		return;
	}

	size = sizeof(SHARED_TAG_MAP_INFO);
	if(size != 32) 
	{
		MessageBox(NULL, _T("sizeof(SHARED_TAG_MAP_INFO) != 32)"), _T("error"), MB_OK);
		return;
	}
	
	size = sizeof(SHARED_TAG_WRITE);
	if(size != 10000) 
	{
		MessageBox(NULL, _T("sizeof(SHARED_TAG_WRITE) != 10000)"), _T("error"), MB_OK);
		return;
	}

	size = sizeof(SHARED_TAG_READ);
	if(size != 600) 
	{
		MessageBox(NULL, _T("sizeof(SHARED_TAG_READ) != 600)"), _T("error"), MB_OK);
		return;
	}

	CString name;

	// 다른 프로그램에서 참조할 수 있는 
	name.Format(_T("TagShare_MapNumber"));
	hSharedTagMapNumber = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(hSharedTagMapNumber == NULL)	return;	//할당할 수 없다.
	else {
		pSharedTagMapNumber = (SHARED_TAG_MAP_NUMBER*)MapViewOfFile(hSharedTagMapNumber, FILE_MAP_READ, 0, 0, 0);
		cOldMapNumber = pSharedTagMapNumber->cMapNumber;
	}
	
	// 다른 프로그램에서 참조할 수 있는 
	name.Format(_T("TagShare_Information%d"), pSharedTagMapNumber->cMapNumber);
	hSharedTagInfo = OpenFileMapping(FILE_MAP_READ, FALSE, name);
	if(hSharedTagInfo == NULL)	return;	//할당할 수 없다.
	else {
		infoSharedTag = (SHARED_TAG_MAP_INFO*)MapViewOfFile(hSharedTagInfo, FILE_MAP_READ, 0, 0, 0);

		if(GetCRC_SumWORD(infoSharedTag, sizeof(SHARED_TAG_MAP_INFO)-2) != infoSharedTag->crc) {	// crc mismatched
			SharedTagUnInit();
			return;
		}
	}

	if(infoSharedTag->nTagTotalCount > 0) {
		name.Format(_T("TagShare_Memory%d"), pSharedTagMapNumber->cMapNumber);
		hSharedTag = OpenFileMapping(FILE_MAP_READ, false, name);
		if(hSharedTag != NULL) {
			sharedTag = (SHARED_TAG_READ*)MapViewOfFile(hSharedTag, FILE_MAP_READ, 0, 0, 0);
		}
	}

	if(infoSharedTag->nCountString > 0) {
		name.Format(_T("TagShare_StringMemory%d"), pSharedTagMapNumber->cMapNumber);
		hStringMemory = OpenFileMapping(FILE_MAP_READ, false, name);
		if(hStringMemory != NULL) {
			pStringMemory = (WCHAR*)MapViewOfFile(hStringMemory, FILE_MAP_READ, 0, 0, 0);
		}
	}

	name.Format(_T("TagShare_WriteTag%d"), pSharedTagMapNumber->cMapNumber);
	hWriteTag = OpenFileMapping(FILE_MAP_ALL_ACCESS, false, name);
	if(hWriteTag != NULL) {
		pWriteTag = (SHARED_TAG_WRITE*)MapViewOfFile(hWriteTag, FILE_MAP_ALL_ACCESS, 0, 0, 0);
	}
}

void SharedTagUnInit()
{
	if(pSharedTagMapNumber != NULL) {
		UnmapViewOfFile(pSharedTagMapNumber);
		pSharedTagMapNumber = NULL;
		CloseHandle(hSharedTagMapNumber);
		hSharedTagMapNumber = NULL;
	}

	if(infoSharedTag != NULL) {
		UnmapViewOfFile(infoSharedTag);
		infoSharedTag = NULL;
		CloseHandle(hSharedTagInfo);
		hSharedTagInfo = NULL;
	}

	if(sharedTag != NULL) {
		UnmapViewOfFile(sharedTag);
		sharedTag = NULL;
		CloseHandle(hSharedTag);
		hSharedTag = NULL;
	}

	if(pStringMemory != NULL) {
		UnmapViewOfFile(pStringMemory);
		pStringMemory = NULL;
		CloseHandle(hStringMemory);
		hStringMemory = NULL;
	}

	if(pWriteTag != NULL) {
		UnmapViewOfFile(pWriteTag);
		pWriteTag = NULL;
		CloseHandle(hWriteTag);
		hWriteTag = NULL;
	}
}

static int GetIndexOfTag(const WCHAR *tag)
{
	if(infoSharedTag == NULL)	return -1;

	SHARED_TAG_READ *st;

	for(int i = 0; i < infoSharedTag->nTagTotalCount; i++) 
	{
		st = &sharedTag[i];

		if(wcscmp(tag, st->tag) == 0)
			return i;
	}

	return -1;
}

bool SharedTagGetDOUBLE(const WCHAR *tag, int *index, double *value)
{
	if(infoSharedTag == NULL)	return false;

	if(*index < 0 || *index >= infoSharedTag->nTagTotalCount)
		*index = -1;
	
	SHARED_TAG_READ *st;

	if(*index != -1) {
		st = &sharedTag[*index];
		if(wcscmp(tag, st->tag) != 0) {
			*index = GetIndexOfTag(tag);		
		}
	}
	else 
	{
		*index = GetIndexOfTag(tag);
	}

	if(*index == -1) {
		*value = 0;
		return false;
	}
	
	st = &sharedTag[*index];

	if(st->value_type == 2)
		memcpy(value, st->value, 8);
	else 
		*value = 0;

	return true;
}

bool SharedTagGetSTRING(const WCHAR *tag, int *index, CString *buf)
{
	if(infoSharedTag == NULL)	return false;

	if(*index < 0 || *index >= infoSharedTag->nTagTotalCount)
		*index = -1;

	SHARED_TAG_READ *st;

	if(*index != -1) {
		st = &sharedTag[*index];
		if(wcscmp(tag, st->tag) != 0) {
			*index = GetIndexOfTag(tag);		
		}
	}
	else 
	{
		*index = GetIndexOfTag(tag);
	}

	if(*index == -1)	return 0;
	
	st = &sharedTag[*index];

	if(st->value_type == 9) 
	{
		int index_s;
		memcpy(&index_s, st->value, 4);	
		int pos = infoSharedTag->MAX_STRING*index_s;
		*buf = &pStringMemory[pos];
		return true;
	}
	else {
		*buf = "";
		return false;
	}
}



// return 이 false 이면 '감시 프로그램이 실행중이 아니거나 아직 대기중인 출력이 있습니다'
bool SharedTagWriteDOUBLE(const WCHAR *tag, double value, const WCHAR *ip, const WCHAR *username, const WCHAR *computername)
{
	// 감시 프로그램이 실행중이 아니다.
	if(pWriteTag == NULL)	return false;

	SHARED_TAG_WRITE *wt = pWriteTag;

	// 아직 출력이 되지 않았으면 1초는 기다린다.
	if(wt->bReadyWrite) 
	{
		TimeOutMiliSecClass timeout;
		
		while(wt->bReadyWrite) 
		{
			Sleep(1);
			if(timeout.IsTimeOut(1000)) return false;
		}
	}

	wcscpy(wt->tag, tag);
	wt->value_type = 2;
	memcpy(wt->value, &value, 8);
	wcscpy(wt->user_name, username);
	wcscpy(wt->computer_name, computername);

	CommaBlockString comma;
	comma.SetBlockCode('.');
	int ip1, ip2, ip3, ip4;
	comma.GetInt(ip1);
	comma.GetInt(ip2);
	comma.GetInt(ip3);
	comma.GetInt(ip4);
	wt->ip = ip1 | (ip2 << 8) | (ip3 << 16) | (ip4 << 24);

	wt->bReadyWrite = true;

	return true;
}

// return 이 false 이면 '감시 프로그램이 실행중이 아니거나 아직 대기중인 출력이 있습니다'
bool SharedTagWriteSTRING(const WCHAR *tag, const WCHAR *value, int size, const WCHAR *ip, const WCHAR *username, const WCHAR *computername)
{
	// 감시 프로그램이 실행중이 아니다.
	if(pWriteTag == NULL)	return false;

	SHARED_TAG_WRITE *wt = pWriteTag;

	// 아직 출력이 되지 않았으면 1초는 기다린다.
	if(wt->bReadyWrite) 
	{
		TimeOutMiliSecClass timeout;
		
		while(wt->bReadyWrite) 
		{
			Sleep(1);
			if(timeout.IsTimeOut(1000)) return false;
		}
	}

	wcscpy(wt->tag, tag);
	wt->value_type = 9;
	
	if(size > 4095)	size = 4095;
	memcpy(wt->value, value, size*2);
	wt->value[size*2+0] = 0;
	wt->value[size*2+1] = 0;

	wcscpy(wt->user_name, username);
	wcscpy(wt->computer_name, computername);

	CommaBlockString comma;
	comma.SetBlockCode('.');
	int ip1, ip2, ip3, ip4;
	comma.GetInt(ip1);
	comma.GetInt(ip2);
	comma.GetInt(ip3);
	comma.GetInt(ip4);
	wt->ip = ip1 | (ip2 << 8) | (ip3 << 16) | (ip4 << 24);

	wt->bReadyWrite = true;

	return true;
}

void SharedTagCheckMapChanged()
{
	if(pSharedTagMapNumber == NULL) 
	{
		SharedTagInit();
		return;
	}

	if(cOldMapNumber != pSharedTagMapNumber->cMapNumber) 
	{
		SharedTagUnInit();
		SharedTagInit();
	}
}


bool SharedTagGetDOUBLE(const char *tag, int *index, double *value)
{
	CStringW tag_w;	// tag_w = tag 로 하면 오류가 난다. 선언만 하고 아래에서 대입한다.

	tag_w = tag;

	return SharedTagGetDOUBLE(tag_w, index, value);
}

bool SharedTagGetSTRING(const char *tag, int *index, CString *value)
{
	CStringW tag_w;

	tag_w = tag;

	return SharedTagGetSTRING(tag_w, index, value);
}

bool SharedTagWriteDOUBLE(const char *tag, double value, const char *ip, const char *username, const char *computername)
{
	CStringW tag_w;
	CStringW ip_w;
	CStringW username_w;
	CStringW computername_w;

	tag_w = tag;
	ip_w = ip;
	username_w = username;
	computername_w = computername;

	return SharedTagWriteDOUBLE(tag_w, value, ip_w, username_w, computername_w);
}

bool SharedTagWriteSTRING(const char *tag, const char *value, int size, const char *ip, const char *username, const char *computername)
{
	CStringW tag_w;
	CStringW value_w;
	CStringW ip_w;
	CStringW username_w;
	CStringW computername_w;

	tag_w = tag;
	value_w = value;
	ip_w = ip;
	username_w = username;
	computername_w = computername;

	return SharedTagWriteSTRING(tag_w, value_w, size, ip_w, username_w, computername_w);
}
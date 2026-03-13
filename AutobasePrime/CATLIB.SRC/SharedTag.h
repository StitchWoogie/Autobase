#pragma once

#pragma pack(push, 1)

// Shared Memory를 이용한 태그값 공유는 2016-11-7  10.3.1.8 부터 지원된다.
// 태그명은 유니코드(256) 512바이트,  문자열 태그의 값은(4096) 8192 byte를 기본으로한다. 

// 16 byte
typedef struct {
	BYTE        cMapVersion;	// 현재는 Version 1
	BYTE		cMapNumber;		// 0 ~ 255
	BYTE		reserved2;
	BYTE		reserved3;
	BYTE		extra[12];
} SHARED_TAG_MAP_NUMBER;

// 정보 struct size=32개 크기로 만든다.
typedef struct {
	int			nTagTotalCount;
	int			nCountString;
	int			MAX_STRING;
	BYTE		extra[16];
	BYTE		extra2[2];
	WORD		crc;
} SHARED_TAG_MAP_INFO;

// 총크기 600바이트  10만개일때 메모리 60MB 소요
typedef struct {
	WCHAR		tag[256];
	BYTE		tag_type;
	BYTE		value_type;		// 
	BYTE		reserved1[2];	// 
	BYTE		value[8];		// double 을 소화하기 위해서 8바이트로 존재 그이상 큰 배열은 Write
	BYTE		extra[76];		//
} SHARED_TAG_READ;

// struct_size = 10000
typedef struct {
	BYTE		bReadyWrite;			// 쓰기가 준비 되었나?
	BYTE		value_type;
	BYTE		reserved1[2];			// 4의 배수에 맞춘다.
	WCHAR		tag[256];
	BYTE		value[8192];
	int			ip;					
	WCHAR		user_name[80];
	WCHAR		computer_name[80];			
	BYTE		reserved2[968];		// 호환성을 휘해 여분을 남겨둔다. 
} SHARED_TAG_WRITE;

void SharedTagInit();
void SharedTagUnInit();
void SharedTagCheckMapChanged();

bool SharedTagGetDOUBLE(const WCHAR *tag, int *index, double *value);
bool SharedTagGetSTRING(const WCHAR *tag, int *index, CString *buf);
bool SharedTagWriteDOUBLE(const WCHAR *tag, double value, const WCHAR *ip, const WCHAR *username, const WCHAR *computername);
bool SharedTagWriteSTRING(const WCHAR *tag, const WCHAR *value, int size, const WCHAR *ip, const WCHAR *username, const WCHAR *computername);

// 아래 함수는 유니 코드로 컴파일 하지 않을 때 사용할 수 있도록 지원하였다.
// 항상 문자열이 유니코드 변환 과정을 거치므로 속도가 WCHAR 를 사용하는 위의 함수보다는 속도가 느리다.
bool SharedTagGetDOUBLE(const char *tag, int *index, double *value);
bool SharedTagGetSTRING(const char *tag, int *index, CString *buf);
bool SharedTagWriteDOUBLE(const char *tag, double value, const char *ip, const char *username, const char *computername);
bool SharedTagWriteSTRING(const char *tag, const char *value, int size, const char *ip, const char *username, const char *computername);

#pragma pack(pop)
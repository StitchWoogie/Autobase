#if !defined(__TOOLS_H)
#define __TOOLS_H

#if	!defined(__COMPILER_HPP)
#include <compiler.hpp>
#endif

#if	!defined(_INC_STRING)
#include <string.h>
#endif

#if !defined(_INC_STDIO)
#include <stdio.h>
#endif

#pragma pack(push, 1)

#define PI 3.1415926535
//#define PI 3.141592

#define MIN(a, b) ((a) < (b) ? (a) : (b))
#define MAX(a, b) ((a) > (b) ? (a) : (b))

enum BIT{  OFF,   ON   };

int sort_function( const void *a, const void *b);

void Temp(int& temp1, int& temp2);
void Temp(char& temp1, char& temp2);
void Temp(float& temp1, float& temp2);
void Temp(long& temp1, long& temp2);
void Temp(double& temp1, double& temp2);
void Temp(DWORD& temp1, DWORD& temp2);
void NumToHangul( char *Fvalue, char *Svalue);
void AddComma( char *Source,  char *Target );

void blankfill(char *buf);
inline void bell(void) {		MessageBeep(0xFFFF); }
void bell(int count);
int  square(int x, int y);
int  IsSquare(DWORD number);
bool IsFileNameValid(const char *filename);

//-------------------------------------------------------------------------
// 디렉토리 조작함수
//-------------------------------------------------------------------------
int  minusdirectory(char *directory);
int  plusdirectory(char *directory, char *path);
long getfilesize(const char *filename);
int  GetFileDate(char *filename, struct date *d);
int  GetFileTime(char *filename, struct time *t);
int  makeextension(char *filename, char *ext);
unsigned _int64 GetDiskFreeSize(int disknum);
int ChangeDirectory(const TCHAR *directory);			// 디렉토리를 바꿔준다.
													// 드라이브까지 바꿔주므로 편리하다.
int MakeDirectory(LPCTSTR path);
DWORD DeleteDirAndFile(char *path);				// 주어진 디렉토리와 sub file 모두를 지운다.

int  getlabel(char drive, char *label);
int  setlabel(char drive, char *label);

void GetTempDirectory(char *directory);

int GetFileHap(char *filename);  // 주어진 파일종류의 갯수를 알아본다.

// 파일명 분해 클래스
class FnSplit {
		TCHAR *drive;
		TCHAR *dir;
		TCHAR *name;
		TCHAR *ext;
	public:
		FnSplit();
		~FnSplit();
		int  fnsplit(const TCHAR *path);
		void GetName(TCHAR *string) { _tcscpy(string, name); }	// 이름만 알아본다.
		void GetExt (TCHAR *string) { _tcscpy(string, ext);	} 	// 확장자를 구한다.
		void GetNameExt(TCHAR *string);							// 파일명을 구한다.
		void GetDirNameExt(TCHAR *string);						// 디렉토리와 파일명을 구한다.
		void GetDriveDir(TCHAR *directory);						// 드라이브와 디렉토리명을 구한다.
		void GetDrive(TCHAR *string) { _tcscpy(string, drive); }	// 드라이브 이름을 구한다.
		void GetDir(TCHAR *string) { _tcscpy(string, dir); }		// 디렉토리 이름만 구한다.
};

//----------------------------------------------------------------------------
// 날짜 관련함수
//----------------------------------------------------------------------------

int  getmonthlimit(int year, int month);		// 주어진 달의 마지막 날을 구한다.
void MinusYear(int &year);
void MinusYear(struct date *d);
void MinusMonth(int &year, int &month);
void MinusMonth(struct date *d);               // 주어진 날짜 구조체에서 하루를 뺀다.
void MinusMonth(SYSTEMTIME *t);               // 주어진 날짜 구조체에서 하루를 뺀다.
void MinusDay(struct date *d);               // 주어진 날짜 구조체에서 하루를 뺀다.
void MinusDay(int &year, int &mon, int &day);// 주어진 날짜 구조체에서 하루를 뺀다.
void MinusHour(int &year, int &mon, int &day, int &hour);
void MinusHour(struct date *d, struct time *t);
void MinusMin(int &year, int &mon, int &day, int &hour, int &min);
void MinusMin(struct date *d, struct time *t);
void MinusDay(SYSTEMTIME *t);
void MinusHour(SYSTEMTIME *t);
void MinusMinute(SYSTEMTIME *t);
void MinusSecond(int &year, int &mon, int &day, int &hour, int &min, int &sec);
void MinusSecond(SYSTEMTIME *t);
void MinusMilliSeconds(SYSTEMTIME *t, int milli);

void PlusDay(struct date *d);           		// 주어진 날짜 구조체에서 하루를 더한다.
void PlusDay(int &year, int &mon, int &day);	// 주어진 날짜 구조체에서 하루를 뺀다.
void PlusDay(SYSTEMTIME *t);
void PlusHour(struct date *d, struct time *t);
void PlusHour(int &year, int &min, int &day, int &hour);
void PlusHour(SYSTEMTIME *t);
void PlusMin(struct date *d, struct time *t);
void PlusMin(int &year, int &mon, int &day, int &hour, int &min);
void PlusMin(SYSTEMTIME *t);
void PlusSecond(int &year, int &mon, int &day, int &hour, int &min, int &sec);
void PlusSecond(SYSTEMTIME *t);
void PlusMonth(struct date *d);
void PlusMonth(int &year, int &mon);
void PlusMonth(SYSTEMTIME *t);
void PlusYear(int &year);

void AddMilliSecond(SYSTEMTIME *t, int milli);

DWORD GetDayHap(int year,   int month, int day);	// 주어진 날을 원년부터 합한다.
DWORD GetMonHap(int year, int month);				// 주어진 달을 원년부터 합한다.
int   GetWeekDay(int year,  int month, int day);	// 요일을 알아본다. 0-일 ~ 6-토
DWORD GetMinHap(int year,  int month, int day, int hour, int min);
DWORD GetHourHap(int year,  int month, int day, int hour);
DWORD GetDayHap(struct date *d);
DWORD GetHourHap(struct date *d, struct time *t);
DWORD GetMinHap(struct date *d, struct time *t);
DWORD GetMinHap(SYSTEMTIME *st);

int IsDayExist(int year, int mon, int day);	// 지정한 날짜가 존재하는지를 검사한다.

typedef struct {
	short syear;
	char  smon;
	char  sday;
	short lyear;
	char  lmon;
	char  lday;
	char  leap;	// 윤달.
} DATE_SOLAR_LUNAR;

int ConvertSolarToLunar(DATE_SOLAR_LUNAR *date);
//int ConvertSolarToLunar(int s_year, int s_mon, int s_day, int &l_year, int &l_mon, int &l_day, int &leap);

int TextGetOneLine(FILE *in, char *buf, int limit);	// 텍스트 파일에서 원하는 길이만큼 읽어온다.
int TextGetOneLine(FILE *in, CString &buf);			// 텍스트 파일에서 원하는 길이만큼 읽어온다.
//int tTextGetOneLine(FILE *in, TCHAR *buf, int limit);	// 텍스트 파일에서 원하는 길이만큼 읽어온다.
int TextGetOneLineFromUTF8(FILE *in, CString &buf);

int TextGetOneLineW(FILE *in, WCHAR *buf, int limit); // UNICODE용 선언 읽을 때는 in = _tfopen(filename, L"r, ccs=UNICODE"); 로 해야 한다.

void GetProgrammDirectory(HINSTANCE hInstance, LPTSTR dir, int limit);
void GetWindowsRootDirectory(TCHAR *directory, int size);

// 버퍼 조작 함수

DWORD GetFitGlobalBlockSize(DWORD one_block_size);		// Windows 3.1의 Global 메모리는 65536byte 가 넘을때는 블럭을 2의 승수로 맞춘다.

BYTE HexBufToBYTE(char *buf);
WORD HexBufToWORD(char *buf);
DWORD HexBufToDWORD(char *buf);
DWORD HexBufToValue(char *buf, int count);

int MathRound(double f);

void CommaToPoint(char *string);		// string 속에 있는 콤마를 Point로 바꿔준다.

void KillEndSpace(char *buf);		// 뒤에 붙은 빈칸이나 TAB을 잘라준다.

void StringCopy(char *target, int size_t, const char *source, int size_s);


// 지정된 시간이 초과 되었는지를 검사한다. 주로 통신에서 사용한다.
class TimeOutClass {
		char old_sec;
		int  nCurrCount;
	public:
		TimeOutClass();
		void Reset();
		void SetTime(int sec);
		int  IsTimeOut(int sec);
		int  GetCurrCount() { return nCurrCount; }
};

// 지정된 시간이 초과 되었는지를 검사한다. 주로 통신에서 사용한다.
class TimeOutMiliSecClass {
		DWORD  dwCurrCount;
		SYSTEMTIME old_t;
	public:
		TimeOutMiliSecClass();
		void   Reset();
		void   SetTime(DWORD milisec);
		int    IsTimeOut(DWORD milisec);
		DWORD  GetCurrCount() { return dwCurrCount; }
};

void WaitSec(int sec);	// 지정된 시간동안 기다린다.

// Motolola 구조를 IBM 구조로 바꾼다.
DWORD MOTO2IBM(DWORD value);
USHORT MOTO2IBM(USHORT value);
SHORT MOTO2IBM(SHORT value);
int MOTO2IBM(int value);

class FileCommaBlock {	// 콤마로 된 파일 스트링을 분해한다.
		FILE *in;
		char flag_eof;
		char flag_endline;
		char flag_read_first_column;	// 그 줄에서 첫번째로 스캔 하는가를 검사한다.
	  public:
		FileCommaBlock();
		void SetFile(FILE *set);
		void GetString(char *string, int max);
		void GetInt(int &value);
		void GetChar(char &value);
		void GetLong(long &value);
		int IsEOF() { return flag_eof; }
		int IsEOL() { return flag_endline; }
		void NewLine();
};

class CommaBlockString {
		int  scanBufPos;
		int  scanBufHap;
		//HGLOBAL hGlobal;
		TCHAR *scanBuf;
		char cBlockCode;	// 블럭을 구분하는 단위 default ,
	public:
		CommaBlockString(TCHAR *string=NULL);
		~CommaBlockString();

		void Set(const TCHAR *string);
		void SetBlockCode(char code) { cBlockCode = code; }
		void GetString(TCHAR *string, int limit);
		void GetString(CString &str);
		void GetInt(int &val);
		void GetWORD(WORD &val);
		void GetHexDWORD(DWORD &val);
		void GetHexUINT64(unsigned __int64 &val);
		void GetHexWORD(WORD &val);
		void GetInt(short &val);
		void GetChar(char &val);
		void GetBYTE(BYTE &val);
		void GetLong(long &val);
		void GetDWORD(DWORD &val);
		void GetAddressDI(WORD &val);
		void GetFloat(float &val);
		void GetDouble(double &val);
		void GetLongDouble(long double &val);
		void GetStringTotalRemain(TCHAR *string, int limit);
		void GetStringTotalRemain(CString &str);
		int  IsEOS() { return scanBufPos >= scanBufHap; }
		void GetDate(struct date *d);
		void GetTime(struct time *t);
		void GetCOLORREF(COLORREF &color);
		void GetDateTime(SYSTEMTIME *t);
		void GetBool(bool &val);
		void Skip();
};

class tCommaBlockString {
		int  scanBufPos;
		int  scanBufHap;
		TCHAR *scanBuf;
		TCHAR cBlockCode;	// 블럭을 구분하는 단위 default ,
	public:
		tCommaBlockString(TCHAR *string=NULL);
		~tCommaBlockString();

		void Set(LPCTSTR string);
		void SetBlockCode(TCHAR code) { cBlockCode = code; }
		void GetString(TCHAR *string, int limit);
		void GetInt(int &val);
		void GetWORD(WORD &val);
		void GetHexDWORD(DWORD &val);
		void GetHexWORD(WORD &val);
		void GetInt(short &val);
		void GetChar(char &val);
		void GetBYTE(BYTE &val);
		void GetLong(long &val);
		void GetDWORD(DWORD &val);
		void GetAddressDI(WORD &val);
		void GetFloat(float &val);
		void GetDouble(double &val);
		void GetLongDouble(long double &val);
		void GetStringTotalRemain(TCHAR *string, int limit);
		int  IsEOS() { return scanBufPos >= scanBufHap; }
		void GetDate(struct date *d);
		void GetTime(struct time *t);
		void GetCOLORREF(COLORREF &color);
};

class Block {
		BYTE  *pBlock;
		DWORD dwBlockSizeOriginal;
		DWORD dwBlockSizeReal;
		DWORD dwBlockCount;
		DWORD dwAllocedCount;
	public:
		Block(DWORD size = 0);
		~Block();
		void SetBlockSize(DWORD size);
		DWORD GetBlockSize() { return dwBlockSizeOriginal; }
		void SetBlock(void *block, DWORD pos);
		int  GetBlock(void *block, DWORD pos);
		int  InsertBlock(void *block, DWORD pos);
		int  AddBlock(void *block);
		int  DeleteBlock(DWORD pos);
		void DeleteAllBlock();
		DWORD  GetBlockCount() { return dwBlockCount; }
		DWORD  GetCount() 	  { return dwBlockCount; }
		void TempBlock(DWORD t1, DWORD t2);
		void *GetPtr(DWORD pos);
};

// Block Sort시 사용.
typedef int (WINAPI* LPFNBLOCKSORTFUNCTION) (BYTE *data1, BYTE *data2);
void BlockSort(Block *block, LPFNBLOCKSORTFUNCTION function);
void BlockCopy(Block *target, Block *source);

class FindFileClass {
		long handle;
	public:
		FindFileClass();
		~FindFileClass();
		int findfirst(const char *pathname, struct ffblk32 *ffblk, int attrib);
		int findnext(struct ffblk32 *ffblk);
};

class CommandLineClass {
		int  nStringHap;
		int  nStringCurr;
		char *sCmdLine;
	public:
		CommandLineClass();
		~CommandLineClass();
		void Set(char *buf);
		int GetCommand(char *buf, int limit);
};

bool IsFileExists(const TCHAR *filename);
int CompareFile(const char *source, const char *target);

int IsLangKorean();	// Is Korean Window
int IsLangJapanese();	// Is Japanese Window
int IsLangChinese();
int IsLangVietnamese();

class GetLastErrorClass {
	public:
		char *lpMsgBuf;
		GetLastErrorClass();
		~GetLastErrorClass();
		char *GetString();
		char *GetString(DWORD error_code);
		void GetErrorString(char *string, int limit);
};

class SharedMemory {
		HANDLE hHandleFile;
		
	public:
		void   *ptr;

		SharedMemory();
		~SharedMemory();
		void Init(char *name, DWORD size);
		void InitByExist(char *name, DWORD size);
		void Init(char *name, DWORD size, char &first_flag);
		void Uninit();
};

int GetSunRiseSunSetTime(int org_year, int org_mon, int day, double longitude, double latitude, int gh, struct time *tRise, struct time *tSet);

bool IsOsVersionNtPlatform();
bool IsOsVersion98();

// GDI 에서 필요한 A속성
#define ARGB(a, r,g,b)          ((COLORREF)(((BYTE)(r)|((WORD)((BYTE)(g))<<8))|(( (DWORD)(BYTE)(b) )<<16)|(( (DWORD)(BYTE)(a) )<<24)))
#define GetAValue(rgb)      (LOBYTE((rgb)>>24))

// 배열을 크기를 구한다. sizeof 는 배열의 전체 바이트수를 말하고 GetArrLength 는 배열의 갯수를 얻어온다.
template<typename T, int size>		// 
int GetArrLength(T(&)[size]){return size;}

class ThreadLock
{
public:
	ThreadLock(void) { try { InitializeCriticalSection(&cs); }
					 catch(...) {}
					 }
   ~ThreadLock( ) { DeleteCriticalSection(&cs); }
   
   void Lock( void ) { EnterCriticalSection(&cs); }
   void Unlock( void ) { LeaveCriticalSection(&cs); }
private:
   CRITICAL_SECTION cs;
};


class MemoryTextReader {
		BYTE *pBuffer;
		int nSize;
		int nPos;

		int Getc();

	public:
		MemoryTextReader(BYTE *buffer, int size);
		bool ReadLineByUTF8(CString &buf);
};

#pragma pack(pop)

#endif

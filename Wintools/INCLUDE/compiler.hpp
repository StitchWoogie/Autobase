#if !defined(__COMPILER_HPP)
#define __COMPILER_HPP

#ifndef __AFXWIN_H__
#include <afxwin.h>
#endif

#pragma pack(push, 1)

#pragma warning(disable:4996)

#define	MAXFILE	_MAX_FNAME
#define	MAXPATH  _MAX_PATH
#define	MAXEXT	_MAX_EXT
#define	MAXDRIVE	_MAX_DRIVE
#define	MAXDIR	_MAX_DIR

// DLLEXPORT define
#if	!defined (DLLEXPORT)
#define	DLLEXPORT	__declspec(dllexport)
#define	DLLIMPORT	__declspec(dllimport)
#endif

#define	EXPORT

struct time
{
	unsigned char   ti_min;		// Minutes
    unsigned char   ti_hour;	// Hours 
    unsigned char   ti_hund;	// Hundredths of seconds 
    unsigned char   ti_sec;		// Seconds 
};
struct date
{
    short   da_year;					// Year - 1980 
    char    da_day;					// Day of the month 
    char    da_mon;					// Month (1 = Jan) 
};
void getdate(struct date *d);
void gettime(struct time *t);

#define FA_NORMAL   0x00        // Normal file, no attributes 
#define FA_RDONLY   0x01        // Read only attribute 
#define FA_HIDDEN   0x02        // Hidden file 
#define FA_SYSTEM   0x04        // System file 
#define FA_LABEL    0x08        // Volume label 
#define FA_DIREC    0x10        // Directory 
#define FA_ARCH     0x20        // Archive 

struct ffblk32 {
	unsigned	ff_attrib;           // attribute found
	WORD		ff_ftime;             // file time
	WORD		ff_fdate;             // file date
	long		ff_fsize;            // file size
	char		ff_name[260];         // found file name
};

#if	!defined (SHORT)
typedef	short	SHORT;
#endif

#if	!defined (USHORT)
typedef	unsigned short	USHORT;
#endif

typedef char sbyte;
typedef unsigned char byte;

#define GetWindowGlobal(hwnd) GetWindowLong(hwnd, 0)
#define SetWindowGlobal(hwnd, hGlobal) SetWindowLong(hwnd, 0, (LONG)hGlobal)

#pragma pack(pop)

#endif


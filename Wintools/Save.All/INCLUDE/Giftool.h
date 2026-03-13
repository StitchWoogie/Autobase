#if !defined(__GIFTOOL_H)
#define __GIFTOOL_H

#if !defined(__STDIO_H)
#include <stdio.h>
#endif

#if !defined(__GLIB_H)
#include <glib.h>
#endif

#if	!defined (__PICTURE_H)
#include <picture.h>
#endif

#if	!defined (__LZWTOOL_H)
#include <lzwtool.h>
#endif


//*********************************************************
// 	colorBit      | 7 | 6   5   4 | 3 |  2   1   0 |
//                  |   |           |   |            |
//                  | a |      b    | c |      d     |
//
//	a :  1 - 전역 칼라 테이블 있음
//           0 - 전역 칼라 테이블 없음
//	b :  명도수를 나타낸다.
//	     ex>  011b 이면 3이다. 여기에 1을 더해서 2의 4승으로 계산하면
//		  16이 나오므로 16가지의 명도가 있다.
//	c :  몰라
//      d :  색상수를 나타낸다. 계산을 명도수와 같다.
//********************************************************


typedef struct tagGIFHEADER {
	BYTE	maker[3];   		// always - "GIF"
	BYTE	version[3];             // almost - "87a"
	short	xres;   		// screen resolution
	short	yres;
	BYTE	colorBit;               // 컬러 비트 ( 위에서 설명 )
	BYTE	backColor;              // 그림을 뿌리고 난후에 배경색으로
					// 쓰게될 색깔 번호
	BYTE	etc;                    // 몰라
} GIFHEADER;

typedef struct tagGIFHEADER_LOCAL {
	//BYTE	tag;			// always - 0x2C (',')
	short	start_x;			// start x
	short	start_y;			// start y
	short	width;			// image width
	short height;			// image depth
	BYTE	colorBit;		//
} GIFHEADER_LOCAL;

class gifClass : public pictureFileClass {
		int nInterlace;
		lzwReadClass *lzwRead;
		lzwWriteClass *lzwWrite;

		int ReadByte();
		int ReadOK(char *buf, int len);
		int GetDataBlock (char *buf);
		void ReadColorMap (int cmaplen);
		void DoExtension ();
		int  WriteHeader();
	public:
		gifClass(void);
		~gifClass(void);
		int  ReadOpen(HWND hwnd, char *filename);
		int  WriteOpen(HWND hwnd, char *filename);
		int  GetOneLine(BYTE *buf, int limitx);
		int  PutOneLine(BYTE *buf);
		int GetInterlace() { return nInterlace; }

		virtual void WriteClosePrepare();
};

extern const char interlace_start[4];	// 비월주사 방식 시작위치
extern const char interlace_plus[4];	// 비원주사 방식 건너뛰기.



#endif

//***************************************************************************
//	end of giftool.h
//***************************************************************************
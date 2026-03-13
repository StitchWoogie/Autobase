#if !defined(__PCXTOOL_H)
#define __PCXTOOL_H

#if !defined(__STDIO_H)
#include <stdio.h>
#endif

#if !defined(__GLIB_H)
#include <glib.h>
#endif

#if !defined (__PICTURE_H)
#include <picture.h>
#endif

typedef struct tagPCXHEADER{
       char maker;					// allways 10
       char version;					//
       char code;
       char bitperpixel;			//	1 - mono, 16color, 8 - 256 color
       WORD  x1;						// 그림의 시작점 x
       WORD  y1;                 // 그림의 시작점 y
       WORD  x2;                 //	그림의 끝점   x
       WORD  y2;                 //	그림의 끝점   y
       WORD  hres;               //	그림그릴때의 수평해상도
       WORD  vres;               //	그림그릴때의 수직해상도
       char rgb[48];					//	RGB [16][3]
       char vmode;					//
       char nplanes;					//	4 - 16 color, 1 - mono, 256
       WORD  byteperline;        //	bytes per line
       WORD  palinfo;
       WORD  shres;
       WORD  svres;
       char extra[54];
} PCXHEADER;

class pcxClass : public pictureFileClass {
      PCXHEADER head;
		int GetOneLine2To2(BYTE *buf, int limitx);
		int GetOneLine16To16(BYTE *buf, int limitx);
		int GetOneLine256To256(BYTE *buf, int limitx);
		int GetOneLine1600To1600(BYTE *buf, int limitx);

		int FillPcxHeader(PCXHEADER *head);

		void PutRGB();
		int  PutOneLine16(BYTE *buf);
		int  PutOneLineElse(BYTE *buf);
	 public:
		pcxClass(void);
		~pcxClass(void);
		
		int  ReadOpen(HWND hwnd, const TCHAR *filename);
		int  GetOneLine(BYTE *buf, int limitx);

		int  WriteOpen(HWND hwnd, TCHAR *filename);
		int  PutOneLine(BYTE *buf);
		void WriteClosePrepare();
};

int  PcxGetSize(char *filename, int& sizex, int& sizey);
void PcxGetRGB( FILE *in, PCXHEADER *head, BYTE *dac);
int  PcxGetOneLine(FILE *in, PCXHEADER *head, BYTE *buf, int maxx);
int  PcxGetMaxColor( PCXHEADER *head );

void PcxPutRGB(FILE *out, PCXHEADER *head, BYTE *dac);
int  PcxPutOneLine(FILE *out, PCXHEADER *head, BYTE *buf);

#endif
#if	!defined (__SPTTOOL_H)
#define	__SPTTOOL_H

#if	!defined (__GLIB_H)
#include <glib.h>
#endif

#if	!defined (__STDIO_H)
#include <stdio.h>
#endif

#if	!defined (__PICTURE_H)
#include <picture.h>
#endif

typedef struct {
	char 	id[3];			// always "SPT"
	BYTE 	version;			// 1
	WORD    width;       // 그림 가로
	WORD	height;        // 그림 세로
	WORD	startx;        // 시작위치
	WORD	starty;        //
	BYTE	bitsperpixel;  // 팩셀당 차지하는 비트수
	BYTE	nplanes;       // 플랜수
	long	backcolor;     // 배경칼라
	BYTE    interlace;   // 인터레이스 방식
	WORD	hres;          // 화면 수평 해상도
	WORD	vres;          // 화면 수직 해상도
	BYTE	compression;   // 압축
	char	extra[104];    // 여분
} SPTHEADER;

class sptClass : public pictureFileClass {
		int  nBitsPerPixelOriginal;	

		void PutRGB(BYTE *dac);	
		void MakeHeader(SPTHEADER *head);
		int  PutOneLine16(BYTE *buf);
   public:
		sptClass(void);
		~sptClass(void);
		int  ReadOpen(char *filename);
		int  GetOneLine(BYTE *buf, int limitx);
		int  WriteOpen(char *filename);
		int  PutOneLine(BYTE *buf);
};

/*
int  SptGetHeader( FILE *in, SPTHEADER &head);
void SptGetRGB( FILE *in, SPTHEADER *head, BYTE *dac);
int  SptGetOneLine(FILE *in, SPTHEADER *head, BYTE *buf, int maxx);

void SptPutRGB(FILE *out, SPTHEADER *head, BYTE *dac);
int  SptPutOneLine(FILE *out, SPTHEADER *head, BYTE *buf);
*/

#endif

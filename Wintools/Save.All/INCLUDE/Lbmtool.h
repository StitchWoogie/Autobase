//---------------------------------------------------------------------------
//	LBM, BBM, IFF, 화일의 형식은 모두 여기에 속한다.
//
//      화일의 구조와 형식은 다음과 같다.
//
//			   PICTURE FILE FORMATS
//			 by Bob Montgomery 9-21-90
//
//Deluxe Paint II LBM & IFF files
//
//The Deluxe Paint LBM (and IFF) file header (40 bytes) has the following
//content:
//     struct dp2
//     {   char msg1[4];               "FORM"
//	 BYTE a3, a2, a1, a0;        File length - 8  (read left to right)
//	 char msg2[8];               "ILBMBMHD"
//	 BYTE b3, b2, b1, b0;        Length of header (read left to right)
//	 int  width, length, xoff, yoff;
//	 BYTE planes, masking, compression, pad;
//	 int  tansparent;
//	 BYTE x_aspect, y_aspect;
//	 int  screenwidth, screenheight;
//     } ;
//   There may be a color map following a string "CMAP" in the file. After CMAP
//	is the length of the color map (4 bytes, read left to right). The color
//	map is byte triples (R, G, B) for each colors. The number of colors is
//	1 shifted left by planes (1 << planes).
//   The actual picture data follows a string "BODY" and length of the picture
//	data (4 bytes read left to right). The picture data is organized on a
//	color plane basis for DP2, and on a pixel basis for DP2e (enhanced).
//	Thus, for DP2:
//	    There are (width / 8) bytes per row.
//	    The data stream for each row consists of all the bytes for plane 0,
//		followed by all the bytes for plane 1, etc.
//	and for DP2e:
//	    There are (width) bytes/row, where each byte is a pixel color.
//   If the data is uncomperessed (compression flag = 0), the data stream bytes
//	are fed to the output unmodified. If it is compressed, it is run length
//	encoded as follows:
//	    Get a code byte from the data stream.
//	    If the msb is 1, the 'count' is (1 - code), max = 127. Get the next
//		byte from the data stream, and repeat it 'count' times.
//	    If the msb is 0, the 'count' is (1 + code), max = 128. Get the next
//		'count' bytes from the data stream.
//
//---------------------------------------------------------------------------

#ifndef	__GLIB_H
#include <glib.h>
#endif

#ifndef __STDIO_H
#include <stdio.h>
#endif

#if	!defined(__PICTURE_H)
#include <picture.h>
#endif

typedef struct {
	char 	msg1[4];  	// "FORM"
	DWORD 	fileSize; 	// file length - 8;
	char	msg2[8];	// "ILBMBMHD"  "PBM BMHD"
	DWORD	headSize;       // header size
	short	width;
	short	height;
	short	xoff;           // 좌표상에서 그림의 시작위치 x
	short	yoff;           // 좌표상에서 그림의 시작위치 y
	BYTE	planes;         // 사용플랜수
	BYTE	masking;
	BYTE	compression;    // 0/1 (비압축/압축)
	BYTE	pad;
	short	tansparent;
	BYTE	x_aspect;
	BYTE	y_aspect;
	short	screenWidth;    // 스크린 해상도
	short	screenHeight;
} LBMHEADER;			// 이헤더는 IFF 화일과 같다.

class lbmClass : public pictureFileClass {
      LBMHEADER head;
		int GetOneLinePlaneTo256(BYTE *buf, int maxx);
		int GetOneLinePixelTo256(BYTE *buf, int maxx);
		int GetOneLinePlaneTo16( BYTE *buf, int maxx);
		int GetOneLinePlaneTo2(  BYTE *buf, int maxx);
		int GetOneLinePixelTo1600(BYTE *buf, int maxx);

		void HeaderConvertToIBM();
		void ReadRGB();

		//void PutRGB();
	public:
		lbmClass(void);
		~lbmClass(void);
		
		int  ReadOpen(HWND hwnd, char *filename);
		int  GetOneLine(BYTE *buf, int limitx);

		//int  WriteOpen(char *filename);
		//int  PutOneLine(BYTE *buf);
		//void WriteClosePrepare();
};
//void LbmHeaderConvertToIBM(LBMHEADER& head);
//void LbmGetRGB( FILE *in, LBMHEADER *head, BYTE *dac);
//int  LbmGetOneLine(FILE *in, LBMHEADER *head, BYTE *buf, int maxx);

#if !defined(__BMPTOOL_H)
#define __BMPTOOL_H

#if !defined(__COMPILER_H)
#include <compiler.hpp>
#endif

#if !defined(__STDIO_H)
#include <stdio.h>
#endif

#if !defined(__PICTURE_H)
#include <picture.h>
#endif

class bmpClass : public pictureFileClass {
enum {	COMPRESSION_0,
	COMPRESSION_RLE8,
	COMPRESSION_RLE4
};
	int compression;
	int  GetOneLineCompression0(BYTE *buf, int limitx);
	int  GetOneLineCompressionRLE8(BYTE *buf, int limitx);
	int  GetOneLineCompressionRLE4(BYTE *buf, int limitx);
	void MakeHeader(BITMAPFILEHEADER *bf, BITMAPINFOHEADER *bi);
	int  PutRGB();
	FILE *out_alpha;
 public:
	bmpClass(void);
	~bmpClass(void);
	int  ReadOpen(HWND hwnd, const char *filename);
	int  WriteOpen(const char *filename);
	int  GetOneLine(BYTE *buf, int limitx);
	int  PutOneLine(BYTE *buf);

};

//void BmpPutRGB(FILE *out, BITMAPINFOHEADER *bi, BYTE *dac);
//int  BmpPutOneLine(FILE *out, BITMAPINFOHEADER *bi, BYTE *buf);
#endif

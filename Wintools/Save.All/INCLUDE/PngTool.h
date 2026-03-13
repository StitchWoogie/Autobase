#if !defined(__PNGTOOL_H)
#define __PNGTOOL_H

#if !defined(__COMPILER_H)
#include <compiler.hpp>
#endif

#if !defined(__STDIO_H)
#include <stdio.h>
#endif

#if !defined(__PICTURE_H)
#include <picture.h>
#endif

typedef struct tagPNGHEADER{
       BYTE signature[8];	// 137 80 78 71 13 10 26 10
} PNGHEADER;

typedef struct tagPNGCHUNK{
		DWORD Length;
		BYTE ChunkType[4];
		BYTE *ChunkData;
		unsigned int CRC;
} PNGCHUNK;

typedef struct {
	int Width;
	int Height;
	BYTE BitDepth;
	BYTE ColorType;
	BYTE CompressionMethod;
	BYTE FilterMethod;
	BYTE InterlaceMethod;
} CHUNK_IHDR;

typedef struct {
	DWORD PixelsPerUnitX;
	DWORD PixelsPerUnitY;
	BYTE  UnitSpecifier;
} CHUNK_PHYS;

class pngClass : public pictureFileClass {
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
	CHUNK_IHDR chunkIHDR;
 public:
	pngClass(void);
	~pngClass(void);
	int  ReadOpen(HWND hwnd, const char *filename);
	int  WriteOpen(const char *filename);
	int  GetOneLine(BYTE *buf, int limitx);
	int  PutOneLine(BYTE *buf);

};
#endif

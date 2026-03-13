#include "stdafx.h"
#include <math.h>
//#include <iostream.h>
#include <conio.h>
#include <memory.h>

#include <bmptool.h>
#include <glib.h>

#define	COLOR_2			1
#define	COLOR_16			4
#define	COLOR_256		8
#define	COLOR_32768		15
#define	COLOR_65536		16
#define	COLOR_1600		24

bmpClass :: bmpClass() : pictureFileClass()
{
	out_alpha = NULL;
}

bmpClass :: ~bmpClass()
{
	if(out_alpha != NULL) {
		fclose(out_alpha);
		out_alpha = NULL;
	}
}

int bmpClass :: ReadOpen(HWND hwnd, const char *filename)
{
	BITMAPFILEHEADER *bf;
	BITMAPINFOHEADER *bi;

	if(dac == NULL)	return 0;

	bf = new BITMAPFILEHEADER[1];
	if(bf == NULL)	return 0;
	bi = new BITMAPINFOHEADER[1];
	if(bi == NULL) {
		delete bf;
	}

	in = fopen(filename, "rb");
	if(in == NULL) {
		ReadOpenError(hwnd, filename);
		delete bf;
		delete bi;
		return 0;
	}
	fread(bf, 1, sizeof(BITMAPFILEHEADER), in);
	fread(bi, 1, sizeof(BITMAPINFOHEADER), in);

	if(bf->bfType != 0x4D42) {	// "BM"
		MessageBox(NULL, "BMP 파일이 아닙니다.", "파일 형식 틀림", MB_OK);
		fclose(in);
		delete bf;
		delete bi;
		return 0;
	}

	nWidth = (int)bi->biWidth;
	nHeight = (int)bi->biHeight;
	nBitsPerPixel = bi->biBitCount;

	compression = (int) bi->biCompression;

	image_order = IMAGE_ORDER_UP;

	switch(bi->biBitCount) {
		case 1:
		case 4:
		case 8:
		case 24:
		case 32:
			nBytesPerLine = BmpWidthToByte((int)bi->biWidth, bi->biBitCount);
			break;
		default:
			char message[256];
			wsprintf(message, "지원되지 않는 색상수를 가진 그림입니다.\nbi.biBitCount=%d",
													 bi->biBitCount);
			MessageBox(NULL, message, "BMP 색상수 이상", MB_OK);
			fclose(in);
			delete bf;
			delete bi;
			return 0;
	}

	switch(bi->biBitCount) {
		case COLOR_16:
		case COLOR_256:
			int color = (int)pow((double)2, (double)bi->biBitCount);	// get max color
			RGBQUAD	 brgb[256];  			// bitmap rgb
			int i;

			fread(&brgb[0], 1, (size_t)color*4, in);
				// mono = 2*4byte
				// 16color = 16*4byte
				// 256color = 256*4 byte

			for(i = 0; i < color; i++) {   	// change RGB
				dac[i*3+0]  = brgb[i].rgbRed;
				dac[i*3+1]  = brgb[i].rgbGreen;
				dac[i*3+2]  = brgb[i].rgbBlue;
			}
			break;
	}

	fseek(in, bf->bfOffBits, SEEK_SET);

	delete bf;
	delete bi;

	return 1;
}

int bmpClass :: GetOneLineCompression0(BYTE *buf, int /*maxx*/)
{
	fread(buf, 1, nBytesPerLine, in);

	return 1;
}

int bmpClass :: GetOneLineCompressionRLE8(BYTE *buf, int /*maxx*/)
{
	int ch1, ch2, x = 0, i;

	switch(nBitsPerPixel) {
		case COLOR_1600:
		case COLOR_32768:
		case COLOR_65536:
		case COLOR_256:
		case COLOR_2:
			while(1) {
				ch1 = fgetc(in);
				ch2 = fgetc(in);
				if(ch1 == 0) {	// absolute or escape
					switch(ch2) {
						case 0:		return 1;	// end of line
						case 1:		return 1;	// end of image
						case 2:		break;		// delta
						default:		// nonrepeating run
							for(i = 0; i < ch2; i++) {
								buf[x] = fgetc(in);
								x++;
							}
							if(ch2%2)	fgetc(in);
							break;
					}
				}
				else {	// encoded
					for(i = 0; i < ch1; i++) {
						buf[x] = ch2;
						x++;
					}
				}
				//if(x >= nBytesPerLine)	break;
			}
	}
	return 1;
}

int bmpClass :: GetOneLine(BYTE *buf, int maxx)
{
	switch(compression) {
		case COMPRESSION_0:	return (GetOneLineCompression0(buf, maxx));
		case COMPRESSION_RLE8:	return (GetOneLineCompressionRLE8(buf, maxx));
	}
	return 1;
}

int bmpClass :: PutRGB()
{
	RGBQUAD win;
	register int color;
	register int i;

	if(nBitsPerPixel == 1)	return 0;
	if(nBitsPerPixel == 24)	return 0;

	color = (int)pow((double)2, (double)nBitsPerPixel);

	for(i = 0; i < color; i++) {
		win.rgbRed = dac[i*3];
		win.rgbGreen = dac[i*3+1];
		win.rgbBlue = dac[i*3+2];
		win.rgbReserved = 0;
		fwrite(&win, 1, sizeof(RGBQUAD), out);
	}

	return 1;
}


//----------------------------------------------------------------------------
//	BMP 
//
//	
//----------------------------------------------------------------------------

int bmpClass :: PutOneLine(BYTE *buf)
{
	unsigned byteperline;

	byteperline = BmpWidthToByte(nWidth, nBitsPerPixel);
	fwrite(buf, 1, byteperline, out);

	if(nBitsPerPixel == 32 && out_alpha != NULL) {
		for(int i = 0; i < nWidth; i++) {
			fputc(buf[i*4+3], out_alpha);
		}
	}

	return 1;
}

void bmpClass :: MakeHeader(BITMAPFILEHEADER *bf, BITMAPINFOHEADER *bi)
{
	int  palsize = 0;
	DWORD datasize;

	switch(nBitsPerPixel) {
		case COLOR_1600:	palsize = 0;			break;
		case COLOR_256:	palsize = 4*256;     break;
		case COLOR_16:    palsize = 4*16;		break;
		case COLOR_2:		palsize = 0;			break;
	}
	datasize = BmpWidthToByte(nWidth, nBitsPerPixel);
	datasize = datasize*nHeight;

	bf->bfType = 0x4d42;   		// "BM"
	bf->bfSize = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER) +
			 palsize + datasize;      // all file size
	bf->bfReserved1 = 0;
	bf->bfReserved2 = 0;
	bf->bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER) +
		palsize;                // start offset of image data

	bi->biSize = sizeof (BITMAPINFOHEADER); 	// info header size
	bi->biWidth =  nWidth;							// image width
	bi->biHeight = nHeight;							// image height
	bi->biPlanes = 1;                         // n planes
	bi->biBitCount = (WORD)nBitsPerPixel;
	bi->biCompression = BI_RGB;	// 0        // compression method
	bi->biSizeImage = datasize;
	bi->biXPelsPerMeter = 0;
	bi->biYPelsPerMeter = 0;
	bi->biClrUsed = 0;
	bi->biClrImportant = 0;
}

int bmpClass :: WriteOpen(const char *filename)
{
	BITMAPFILEHEADER bf;                    // bitmap file header
	BITMAPINFOHEADER bi;                    // bitmap info header

	out = fopen(filename, "wb");

	if(out == NULL) {
		return 0;
	}

	MakeHeader(&bf, &bi);

	fwrite(&bf, sizeof(BITMAPFILEHEADER), 1, out);
	fwrite(&bi, sizeof(BITMAPINFOHEADER), 1, out);

	PutRGB();

	if(nBitsPerPixel == 32) {
		CString filename_alpha;
		filename_alpha.Format("%s.alpha", filename);
		out_alpha = fopen(filename_alpha, "wb");
	}

	return 1;
}




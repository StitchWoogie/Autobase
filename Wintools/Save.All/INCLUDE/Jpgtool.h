#if !defined(__JPGTOOL_H)
#define __JPGTOOL_H

#if !defined(__WINDOWS_H)
#include <windows.h>
#endif

#if !defined(__STDIO_H)
#include <stdio.h>
#endif

#if !defined(__PICTURE_H)
#include <picture.h>
#endif

class jpgClass : public pictureFileClass {
 public:
	jpgClass(void);
	~jpgClass(void);
	int  ReadOpen(char *filename);
	int  WriteOpen(char *filename);
	int  GetOneLine(BYTE *buf, int limitx);
	int  PutOneLine(BYTE *buf);
};

typedef	struct {
	BYTE	ff1;	// 0xFF;
	BYTE	soi;	// 0xD8;

	BYTE	ff2;	// 0xFF;
	BYTE	APPO;	// 0xE0;

	short	length;	// total APPO field byte count, including the byte
			// count value(2 bytes), but excluding the APPO
			// maker itself.
	char	identifier[5];	// "JFIF\0"
	BYTE	majorVersion;
	BYTE	minorVersion;
	BYTE	units;	// units for the X and Y densities.
			// 0 - no units, X and Y specify the pixel aspect ratio
			// 1 - X and Y are dots per inch
			// 2 - X and Y are dots per cm
	short	xDensity;	// Horizontal pixel density
	short	yDensity;	// Vertical pixel density
	BYTE	xThumbnail;	// Thumbnail horizontal pixel count
	BYTE	yThumbnail;	// Thumbnail vertical pixel count
} JPGHEADER;

void JpgHeaderConvertToIBM(JPGHEADER& head);

#endif







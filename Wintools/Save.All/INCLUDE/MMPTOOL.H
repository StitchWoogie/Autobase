#if !defined (__MMPTOOL_H)
#define __MMPTOOL_H

#if !defined (__STDIO_H)
#include <stdio.h>
#endif

#if !defined (__GLIB_H)
#include <glib.h>
#endif

#if !defined (__PICTURE_H)
#include <picture.h>
#endif

#define	MMP_HDR_STRING_12	"YUV12C"	/* Header string for MMotion file */
#define	MMP_HDR_STRING_16	"YUV16C"	/* Header string for MMotion file */


typedef struct MMPHEADER {
    char	mhIDString[6]; 	// "YUV12C"
    WORD	mhReserved[3];  // 0
    WORD	mhWidth;
    WORD	mhHeight;
} MMPHEADER;

enum mmpHEADERform {
	YUV_411,
	YUV_422
};

/*

int  MmpGetSize(char *filename, int& sizex, int& sizey);
void MmpGetRGB(FILE *in, MMPHEADER head, BYTE *dac);
int  MmpGetOneLine(FILE *in, MMPHEADER head, BYTE *buf, int maxx);
int  MmpGetMaxColor( MMPHEADER head );
void MmpPutRGB(FILE *out, MMPHEADER head, BYTE *dac);
int  MmpPutOneLine(FILE *out, MMPHEADER head, BYTE *buf);
*/

class mmpClass : public pictureFileClass {
	int yuvForm;
	int mmpClass :: GetOneLine411(BYTE *buf, int mamx);
	int mmpClass :: GetOneLine422(BYTE *buf, int mamx);
    public:
	mmpClass(void);
	~mmpClass(void);
	int  ReadOpen(char *filename);
	int  GetOneLine(BYTE *buf, int limitx);
};

#endif
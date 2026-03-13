#if	!defined (__TIFFTOOL_H)
#define		__TIFFTOOL_H

#if	!defined (__STUDIO_H)
#include	<stdio.h>
#endif

#if	!defined (__PICTURE_H)
#include	<picture.h>
#endif

#if	!defined (LONG)
#define	LONG	unsigned long
#endif

#define TF_NEW_SUB_FILE_TYPE	0xFE
#define TF_OLD_SUB_FILE_TYPE	0xFF

#define TF_IMAGE_WIDTH		0x100
#define TF_IMAGE_LENGTH		0x101
#define TF_BITS_PER_SAMPLE	0x102
#define TF_COMPRESSION		0x103
#define TF_PHOTOMETRIC_INTERPRETATION	0x106
#define TF_THRESHOLDING		0x107
#define TF_CELL_WIDTH		0x108
#define TF_CELL_LENGTH		0x109
#define TF_FILL_ORDER		0x10A
#define TF_DOCUMENT_CONTEXT	0x10D
#define TF_IMAGE_DESCRIPTION	0x10E
#define TF_MAKE			0x10F

#define TF_MODEL		0x110
#define TF_STRIP_OFFSETS	0x111
#define TF_ORIENTATION		0x112
#define TF_SAMPLES_PER_PIXEL	0x115
#define TF_ROWS_PER_STRIP	0x116
#define TF_STRIP_BYTE_COUNTS	0x117
#define TF_MIN_SAMPLE_VALUE	0x118
#define TF_MAX_SAMPLE_VALUE	0x119
#define TF_X_RESOLUTION		0x11A
#define TF_Y_RESOLUTION		0x11B
#define TF_PLANAR_CONFIGURATION 0x11C
#define TF_PAGE_NAME		0x11D
#define TF_X_POSITION  		0x11E
#define TF_Y_POSITION		0x11F

#define TF_FREE_OFFSETS		0x120
#define TF_FREE_BYTE_COUNT	0x121
#define TF_GRAY_RESPONSE_UNIT	0x122
#define TF_GRAY_RESPONSE_CURVE	0x123
#define TF_GROUP_3_OPTIONS	0x124				// T4 Encoding Option 이라고도 한다.
#define TF_GROUP_4_OPTIONS	0x125				// T5 Encoding Option 이라고도 한다.
#define TF_RESOLUTION_UNIT	0x128
#define TF_PAGE_NUMBER		0x129
#define TF_COLOR_RESPONSE_UNIT	0x12C
#define TF_COLOR_RESPONSE_CURVE 0x12D

#define TF_SOFTWARE		0x131
#define TF_PREDICTOR		0x13D


#define TF_COLOR_MAP		0x140
#define TF_HALFTONE_HINTS	0x141

typedef struct tagTIFFimageFILEheader {
	WORD	hardware;	// 0-1 	II or MM
	WORD	version;	// 2-3	version
	long	offset;		// 4-7  offset of first IFD
} TIFF_FILE_HEADER;

typedef	struct tagTIFFheader {
	WORD	wHardWare;	// 0x4949 - "II", 0x4d4d - "MM"
	WORD  wVersion;
	LONG  lNewSubfileType;
	WORD	wOldSubfileType; 	// 0xff 1 -
	LONG	lImageWidth;     	// short or long
	LONG	lImageLength;    	// short or long
	LONG	lRowsPerStrip;		// short or long
	LONG	*lpStripByteCounts; 	// strip byte count
	LONG	*lpStripOffsets;        // short or long
	WORD	wSamplesPerPixel;       //
	WORD	*wpBitsPerSample;  		//
	WORD	wPlanarConfiguration;   //
	WORD	wCompression;           // short
											// 1 - no compression
											// 2 - CCITT 31 - Dimensional Modified Huffman run length encoding
											// 3 - Fax compatible CCITT Group 3
											// 4 - Fax compatible CCITT Group 4
											// 5 - LZW compression
											// 32773 - 
	LONG	lGroup3Options;			// 
	LONG	rXResolution[2];                 // rational
	LONG	rYResolution[2];
	WORD	wResolutionUnit;		// resolution unit
	WORD	wPhotometricInterpretation; 	// 0 - white is zero
						// 1 - black is zero
						// 2 - RGB
						// 3 - RGB palette
						// 4 - transparency mask
						// 5 - CMYK
						// 6 - YCbCr
						// 8 - CIELab
	WORD	wThresholding;			// short 0x107
	char	*spSoftware;			// name and version number of the software package(s) used to create the image
	WORD	wGrayResponseUnit;		// the precision of the information contained in the GrayResponseCurve
	WORD	*wpGrayResponseCurve;		// for grayscale data, the optical density of each possible pixel value.
						// 2**BitsPerSample
	WORD	wHalftoneHints[2];		// [0] - highlight, [1] - shadow
	WORD	wPredictor;			// 1 - no prediction scheme used before coding
						// 2 - Horizontal differencing
	WORD	*wpMinSampleValue;		// the minimum component value used
	WORD	*wpMaxSampleValue;		// the maximum component value used
	char	*spMake;			// the scanner manufacturer
}	TIFHEADER;

typedef	struct tagtiffIFDfield {
	WORD	fieldTag;       // field tag
	WORD	fieldType;		// 1 - BYTE
								// 2 - ASCII
								// 3 - SHORT
								// 4 - LONG
								// 5 - 2LONG
	long	fieldLength; 	// field length
	long	fieldOffset;    // field offset
}	TIFF_FIELD_HEADER;

enum fieldTYPEenum{
	TYPE_BYTE = 1,		// 8 bit unsigned char
	TYPE_ASCII = 2,       	// 8 bit ascii string, should be last byte is NULL
	TYPE_WORD = 3,    	// 16 bit unsigned int
	TYPE_LONG = 4,    	// 32 bit unsigned long
	TYPE_RATIONAL = 5      	// 2 LONG data
};

typedef struct {
	WORD	value;
	char	*code;
} T4_RUN_LENGTH_CODE_WORD;

void	TifGetHeader(FILE *in, TIFHEADER& head);	// tif 의 헤더를 만든다.
//int TifGetMaxColor(TIFHEADER head);
//void TifGetRGB(FILE *in, TIFHEADER head, BYTE *dac);
//int TifGetOneLine(FILE *in, TIFHEADER head, BYTE *buf, int size);

class tifClass : public pictureFileClass {
		TIFHEADER head;
		long lCurrentPosY;

		int  ReadPrepare();
		void ReadColorMap(TIFF_FIELD_HEADER);
		void ReadGrayResponseCurve(TIFF_FIELD_HEADER field);
		void ReadRational(TIFF_FIELD_HEADER field, LONG *val);
		LONG Value(TIFF_FIELD_HEADER field);
		char *ReadAsciiString(TIFF_FIELD_HEADER field);
		WORD *ReadWordPointer(TIFF_FIELD_HEADER field);
		LONG *ReadLongPointer(TIFF_FIELD_HEADER field);
		WORD *MakeWordDefault(WORD val);
		void MakePaletteDefault();
		int  GetOneLineComp1(BYTE *buf, int limitx);
		int  GetOneLineComp32773(BYTE *buf, int limitx);
		int  GetOneLineComp32773_16(BYTE *buf, int limitx);
		int  GetOneLineComp32773_2(BYTE *buf, int limitx);
		int  GetOneLineComp3_Extend(BYTE *buf, int maxx, T4_RUN_LENGTH_CODE_WORD *code);
		int  GetOneLineComp3(BYTE *buf, int maxx);
    public:
		tifClass(void);
		~tifClass(void);
		int  ReadOpen(char *filename);
		int  GetOneLine(BYTE *buf, int limitx);
		void GetHeader(TIFHEADER& h);

		int  WriteOpen(char *filename, int width, int height, int bitsperpixel, BYTE *dac);
		void WriteClose();
		int  PutOneLine(BYTE *buf);
};

#endif

/*--------------------------------------------------------------------------

	Image File Header
       ---------------------------------
	0-1 	II or MM
	2-3	version
	4-7     offset of first IFD
       ---------------------------------

	Image File Directory (IFD)
       ---------------------------------
	2 byte  number of directory entries
			 +
	0-1	The tag that identified the field                   +
	2-3	The field type                                      |
	4-7	The number of values, Count of the indicated Type   |  12byte
	8-11	The Value Offset                                    +
       ---------------------------------
			 +
			...   entries
			 +
	4 byte	next IFD offset (0 means none)
---------------------------------------------------------------------------*/
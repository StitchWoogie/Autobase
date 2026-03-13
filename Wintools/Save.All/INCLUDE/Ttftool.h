
typedef	short 				SHORT;
typedef	unsigned short 	USHORT;
typedef  DWORD					Fixed;
typedef	DWORD					ULONG;
typedef	short					FWord;
typedef	unsigned short		uFWord;
typedef	char					CHAR;

typedef struct {			// 'head'
	Fixed		version;
	Fixed 	fontRevision;
	ULONG		checkSumAdjustment;
	ULONG 	magicNumber;
	USHORT   flags;
	USHORT	unitsPerEm;
	DWORD		created[2];				//longDateTime created;
	DWORD 	modified[2];			// longDateTime modified;
	FWord		xMin;
	FWord		yMin;
	FWord		xMax;
	FWord		yMax;
	USHORT	macStyle;
	USHORT	lowestRecPPEM;
	SHORT		fontDirectionHint;
	SHORT		indexToLocFormat;
	SHORT		glyphDataFormat;
} FONT_HEADER;

typedef struct {
	SHORT	numberOfContours;
	SHORT	xMin;
	SHORT yMin;
	SHORT	xMax;
	SHORT yMax;
} GLYPH_HEADER;

typedef struct {
	Fixed	version;			// 0x00010000 for version 1.0
	FWord	Ascender;		// Typographic ascent.
	FWord Descender;		// Typographic descent.
	FWord	LineGap;			// Typographic line gap. Negative LineGap values are reaated as zero in windows 3.1, System 6, and System 7.
	uFWord advanceWidthMax;
	FWord minLeftSideBearing;
	FWord minRightSideBearing;
	FWord xMaxExtent;
	SHORT caretSlopeRise;
	SHORT caretSlopeRun;
	SHORT reserved1;
	SHORT reserved2;
	SHORT reserved3;
	SHORT reserved4;
	SHORT reserved5;
	SHORT metricDataFormat;
   USHORT numberOfHMetrics;
} HORIZONTAL_HEADER;	// 'hhea' header

typedef struct {
	USHORT	version;
	SHORT		xAvgCharWidth;
	USHORT	usWeightClass;
	USHORT	usWidthClass;
	SHORT		fsType;
	SHORT		ySubscriptXSize;
	SHORT		ySubscriptYSize;
	SHORT		ySubscriptXOffset;
	SHORT		ySubscriptYOffset;
	SHORT		ySuperscriptXSize;
	SHORT		ySuperscriptYSize;
	SHORT		ySuperscriptXOffset;
	SHORT		ySuperscriptYOffset;
	SHORT		yStrikeoutSize;
	SHORT		yStrikeoutPosition;
	SHORT		sFamilyClass;
	PANOSE	panose;
	ULONG		ulCharRange[4];
	char		achVendID[4];
	USHORT	fsSelection;
	USHORT	usFirstCharIndex;
	USHORT	usLastCharIncex;
	USHORT	sTypoAscender;
	USHORT	sTypoDescender;
	USHORT	sTypoLineGap;
	USHORT	usWinAscent;
	USHORT	usWinDescent;
} OS2_HEADER;

typedef struct {
	Fixed		version;
	USHORT	numGlyphs;
	// ... etc
} MAXP_HEADER;

#define	MAX_COMPOSITE_GLYPH	10

class ttfClass {
		HDC 	 hDC;
		MAXP_HEADER	 headMaxp;
		GLYPH_HEADER headGlyph;
		FONT_HEADER	headFont;
		OS2_HEADER	headOS2;
		USHORT usIndexMappingSegCount;
		ULONG  ulIndexOffset;
		char	 bUniCode;	// unicode를 사용한 폰트인가?

		void FreeAllBuf();
		int GetFontOffsetAndSize(int font_number, ULONG &offset, DWORD &size);
		int PrepareIndexMappingTable();
		void PrepareTableNAME(HDC hdc);
		int nCompositeGlyphHap;

	public:
		SHORT *xCoordinates;
		SHORT *yCoordinates;
		USHORT *endPtsOfContours;
		BYTE	 *flags;
		int    numCoordinates;
		HORIZONTAL_HEADER headHorizontal;	// 'hhea' header
		USHORT compositeGlyphIndex[MAX_COMPOSITE_GLYPH];

		ttfClass();
		~ttfClass();
		int SetHDC(HDC hdc);
		int GetData(USHORT font_number);
//		int GetHeight() {   return (headHorizontal.Ascender-headHorizontal.Descender+headHorizontal.LineGap); }
//		int GetBaseLine() { return headHorizontal.Ascender; }
//		int GetHeight()   { return (headFont.yMax-headFont.yMin+headFont.lineGap); }
//		int GetBaseLine() { return (headFont.yMax); }
		int GetHeight()   { return (headOS2.usWinAscent+headOS2.usWinDescent/*+headOS2.sTypoLineGap*/); }
		int GetBaseLine() { return (headOS2.usWinAscent); }
		int GetWidth(USHORT font_number);
		USHORT GetGlyphPosition(USHORT font_number);
};




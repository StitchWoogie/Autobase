#if	!defined (__GLIB_H)
#include	<glib.h>
#endif

#if	!defined (__COMPILEER_HPP)
#include <compiler.hpp>
#endif

void ConvertBufInit(int tBit, int sBit, int xsize, int Convertform, BYTE *dac);
void ConvertBufPrnInit(int tBit, int sBit, int xsize, int Convertform, BYTE *dac);
void ConvertBufOneLine(BYTE *target, BYTE *source);
void ConvertBufGetDac(BYTE *dac);
int  GetColorAllBuf16(unsigned char *buf, int x);
void PutColorAllBuf16(unsigned char *buf, int x, int color);

extern BYTE cbSourceBit, cbTargetBit, cbConvertFlag;
extern int  cbByteX, cbSizeX;
extern int  cbLineY;	// 디더링 할때 현재의 라인수가 필요하다.
extern BYTE dacConvertBuf[768];	// convert buf 할때 임시로 보관하는 dac

class convertBuf {
	BYTE	sourceBit, targetBit;
	int	ditherMethod;
	int	byteX, sizeX;
	int	currY;
	BYTE	*table;
	BYTE	*gammaVector;
	float	gammavalue;
	int	*err1, *err2;

	int	*cerr1, *cerr2;
	int	*yerr1, *yerr2;
	int	*merr1, *merr2;
	int	*kerr1, *kerr2;

	int	threshold;

	void 	ConvertSame(BYTE *tbuf, BYTE *sbuf);

	void 	Convert1600ToCYMPrintLoss(BYTE *tbuf, BYTE *sbuf);
	void 	Convert32768ToCYMPrintLoss(BYTE *tbuf, BYTE *sbuf);
	void 	Convert256ToCYMPrintLoss(BYTE *tbuf, BYTE *sbuf);
	void 	Convert16ToCYMPrintLoss(BYTE *tbuf, BYTE *sbuf);

	void 	Convert1600ToCYMKPrintLoss(BYTE *tbuf, BYTE *sbuf);
	void 	Convert32768ToCYMKPrintLoss(BYTE *tbuf, BYTE *sbuf);
	void 	Convert256ToCYMKPrintLoss(BYTE *tbuf, BYTE *sbuf);
	void 	Convert16ToCYMKPrintLoss(BYTE *tbuf, BYTE *sbuf);

	void 	Convert1600ToCYMKPrint(BYTE *tbuf, BYTE *sbuf);
	void 	Convert32768ToCYMKPrint(BYTE *tbuf, BYTE *sbuf);
	void 	Convert256ToCYMKPrint(BYTE *tbuf, BYTE *sbuf);
	void 	Convert16ToCYMKPrint(BYTE *tbuf, BYTE *sbuf);
	void 	Convert2ToCYMKPrint(BYTE *tbuf, BYTE *sbuf);

	void 	Convert1600To2Print(BYTE *tbuf, BYTE *sbuf);
	void 	Convert32768To2Print(BYTE *tbuf, BYTE *sbuf);
	void 	Convert256To2Print(BYTE *tbuf, BYTE *sbuf);
	void 	Convert16To2Print(BYTE *tbuf, BYTE *sbuf);
	void 	Convert2To2Print(BYTE *tbuf, BYTE *sbuf);

	void    ConvertCYMKPrintDiffusionExpand(BYTE *tbuf);
	void 	ConvertCYMPrintDiffusionExpand(BYTE *tbuf);

	void 	Convert1600ToCYMKPrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert32768ToCYMKPrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert256ToCYMKPrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert16ToCYMKPrintDiffusion(BYTE *tbuf, BYTE *sbuf);

	void 	Convert1600ToCYMPrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert32768ToCYMPrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert256ToCYMPrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert16ToCYMPrintDiffusion(BYTE *tbuf, BYTE *sbuf);

	void    Convert2PrintDiffusionExpand(BYTE *tbuf);
	void 	Convert1600To2PrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert32768To2PrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert256To2PrintDiffusion(BYTE *tbuf, BYTE *sbuf);
	void 	Convert16To2PrintDiffusion(BYTE *tbuf, BYTE *sbuf);

       public:
	convertBuf();
	~convertBuf();
	int	Init(int tBit, int sBit, int xsize, int dither, BYTE *dac, float gamma);
	void	OneLine(BYTE *tbuf, BYTE *sbuf);
};

class opticBuf {
	int 	*table;
	int 	nTargetX, nSourceX;	// 버퍼의 xsize;
	int 	nBit;	// 확대 축소할 버퍼의 색상수

	void 	OpticBufSame(BYTE *target, BYTE *source);
	void 	OpticBuf1600(BYTE *target, BYTE *source);
	void 	OpticBuf32768(BYTE *target, BYTE *source);
	void 	OpticBuf256(BYTE  *target, BYTE  *source);
	void 	OpticBuf16(BYTE  *target, BYTE  *source);
	void 	OpticBuf2(BYTE  *target, BYTE  *source);
	void  OpticMakeTable(void);
		 public:
	opticBuf();
	~opticBuf();
	int	Init(int targetx, int sourcex, int bufBit);
	void	Free(void);
	void	OneLine(BYTE  *tbuf, BYTE  *sbuf);
};

//void OpticBufInit(int targetx, int sourcex, int bufBit, int *table);
//extern void (*OpticBufOneLine)(BYTE *target, BYTE *source);

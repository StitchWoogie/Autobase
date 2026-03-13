#if	!defined (__PICTURE_H)
#define	__PICTURE_H

#if	!defined (__STDLIB_H)
#include <stdlib.h>
#endif

#if	!defined (__STDIO_H)
#include	<stdio.h>
#endif

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

class pictureFileClass {
 protected:
	FILE *in;
	FILE *out;
	BYTE *dac;
	int  nBitsPerPixel;
	int  nWidth;
	int  nHeight;
	int  nBytesPerLine;
	char image_order;

	void ReadOpenError(HWND hwnd, const TCHAR *filename);
	void ReadSizeError(HWND hwnd, const TCHAR *filename);
	void WriteOpenError(HWND hwnd, const TCHAR *filename);
 public:
enum tagImageORDER {			// image order
		IMAGE_ORDER_DOWN,			// up~down image
		IMAGE_ORDER_UP			// donw~up image
};
	pictureFileClass();
	~pictureFileClass();
	void GetDac(BYTE *dac);
	void GetSize(int &x, int &y);
	int  GetColor();
	void GetInformation(int &x, int &y, int &color);
	
	void SetDac(BYTE *dac);
	void SetSize(int x, int y);
	void SetColor(int color);
	
	char ImageOrder();

	void ReadClose();
	void WriteClose();
	virtual void WriteClosePrepare();
};

typedef	struct {
	int 		nWidth;
	int 		nHeight;
	int 		nBitsPerPixel;
	LONG		lBytesPerLine;
	HGLOBAL		hData;
	BYTE		*data;
	HGLOBAL		hInfo;
	HPALETTE	hPalette;
	BYTE		dac[768];

	int			nDrawZoneX1;
	int			nDrawZoneY1;
	int			nDrawZoneX2;
	int			nDrawZoneY2;
} PICTURE_BITMAP_STRUCT;

// 패턴 채우기에서 쓰는 각종 변수들
// 주의 메모리를 할당해서 쓰는 핸들은 선언을 삼가하자. Pic_line에서 구조체를 임시로
// 보관했다가 다시 복구한다.
typedef struct {
	int		nForm;		// 0 - 보통, 1 - 투명, 2 - 혼합
	BYTE		pattern[8];
	COLORREF	lColorL;		// 색을 혼합 할때 두가지 색상이 필요하다.
	COLORREF	lColorR;
} FILL_PATTERN_STRUCT;

// 그라데이션에서 쓰는 각종 변수들
typedef struct {
	int 		x1, y1, x2, y2;	// 그라데이션 영역
	int		nMethod;				// 위에서 아래 방향
	int		nCount;				// 반복 횟수
	COLORREF	lColorL;
	COLORREF	lColorR;
	BYTE 		sign_flag[3];
	int 		nSizeX, nSizeY;
	int		r_sub, g_sub, b_sub;
	int		r1, g1, b1;
	HLOCAL	hLocalImsi;			// 그라데이션에서 쓰는 임시 버퍼
} FILL_GRADATION_STRUCT;

typedef struct {
	PICTURE_BITMAP_STRUCT	bmp;	
	char	filename[MAXPATH];

	// 아래 Dac로 시작되는 것들은 그림 패턴을 배경에 뿌릴때 팔레트를 미리 일치시키기 위해서 필요하다.
	// 예를 들어서 16색상을 256배경에 뿌릴때 dacTable에 테이블을 미리 만들어 놓고 한다.
	// 만약에 파레트가 일치하면 그냥 계속한다.
	BYTE	dacSource[768];
	BYTE	dacTarget[768];
	BYTE	dacTable[256];
	int	colorSource;
	int	colorTarget;
} FILL_BITMAP_STRUCT;

//------------------------------------------------------------------------------
//	비트맵 버퍼에 그림을 그릴 수 있도록 하는 구조체
//------------------------------------------------------------------------------

typedef struct {
	PICTURE_BITMAP_STRUCT	back;
	int						nFillStyle;		// 채우는 방법, 0-pattern, 1-gradation, 2-bitmap
	char					bFillOutLine;	// 채울때 테두리선의 표시 여부
	COLORREF				lColorLine;
	int						nLineThick;
	int						nStartX;
	int						nStartY;

	FILL_PATTERN_STRUCT 	fillPat;
	FILL_GRADATION_STRUCT 	fillGrad;
	FILL_BITMAP_STRUCT		fillBmp;
} PICTURE_STRUCT;

typedef struct {
	char form;
	char writeform;
	char gradform;
} VECTOR_FORM;

typedef	HGLOBAL	HPICTURE;

HPICTURE PictureLoadNew(HWND hwnd, int width, int height, int bitsperpixel, BYTE *dac);
void PictureClear(HPICTURE hPic, COLORREF color);
PICTURE_STRUCT *PictureLock(HPICTURE hPic);
void PictureUnlock(HPICTURE hPic, PICTURE_STRUCT *pic);
void PictureDelete(HPICTURE hPic);
void PictureGetSize(PICTURE_STRUCT *pic, int &width, int &height);
int  PictureGetColor(PICTURE_STRUCT *pic);
void PicturePutOneLine(PICTURE_STRUCT *pic, int y, BYTE *buf);
void PicturePutBuf(PICTURE_STRUCT *pic, BYTE *buf, int x, int y, int width);
void PictureGetOneLine(PICTURE_STRUCT *pic, int y, BYTE *buf);
void PictureGetBuf(PICTURE_STRUCT *pic, BYTE *buf, int x, int y, int width);
void PictureGetOneLineVertical(PICTURE_STRUCT *pic, int x, BYTE *buf);
HPALETTE PictureMakePalette(PICTURE_STRUCT *pic);
void PictureCopy(PICTURE_STRUCT *target, PICTURE_STRUCT *source);
void PictureSetDac(PICTURE_STRUCT *pic, BYTE *dac);
void PictureSetColor(PICTURE_STRUCT *pic, COLORREF color);
void PictureSetLineThick(PICTURE_STRUCT *pic, int thick);

void PictureSetFillStyle(PICTURE_STRUCT *pic, int num);	// 0 - pattern, 1 - gradation, 2 - bitmap
void PictureSetFillOutLine(PICTURE_STRUCT *pic, char flag);
void PictureSetFillPattern(PICTURE_STRUCT *pic, int pattern, COLORREF color);
void PictureSetFillPattern(PICTURE_STRUCT *pic, BYTE *pattern, COLORREF color);
void PictureSetFillPattern(PICTURE_STRUCT *pic, int pattern, COLORREF colorl, COLORREF colorr);
void PictureSetFillPattern(PICTURE_STRUCT *pic, BYTE *pattern, COLORREF colorl, COLORREF colorr);
void PictureSetFillPatternForm(PICTURE_STRUCT *pic, int form);

void PictureSetFillGrad(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2, COLORREF l, COLORREF r, int form, int num);

void PictureSetFillBitmap(PICTURE_STRUCT *pic, char *filename);

void PicturePutImage(HDC hdc, PICTURE_STRUCT *pic, int x, int y);
void PicturePutImage(HDC hdc, PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);
void PicturePutImageViewRect(HDC hdc, PICTURE_STRUCT *pic, int x, int y, RECT *rect);
void PicturePutImageViewRect(HDC hdc, PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2, RECT *rect);
void PictureRestoreBack(HDC hdc, PICTURE_STRUCT *pic, int x, int y, int x1, int y1, int x2, int y2);

void PicturePutPixel(PICTURE_STRUCT *pic, int x, int y, COLORREF color);
COLORREF PictureGetPixel(PICTURE_STRUCT *pic, int x, int y);
void PictureLine(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);
void PictureMoveTo(PICTURE_STRUCT *pic, int x, int y);
void PictureLineTo(PICTURE_STRUCT *pic, int x, int y);
void PictureCircle(PICTURE_STRUCT *pic, int x, int y, int rx, int ry);
void PictureCircleFill(PICTURE_STRUCT *pic, int x, int y, int rx, int ry);
void PictureRectangle(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);
void PictureRoundRectangle(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);
void PictureRoundRectangleFill(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);
void PictureBar(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);
void PictureBezier(PICTURE_STRUCT *pic, void *p, int npts, int segments);
void PictureFillPoly(PICTURE_STRUCT *pic, int num, int *point);
void PictureFloodFill(PICTURE_STRUCT *pic, int x, int y);

void PictureVectorPoly(HDC hdc, PICTURE_STRUCT *pic,
							  int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, 
							  char *sTextString, VECTOR_FORM *form);
void PictureVectorCircle(HDC hdc, PICTURE_STRUCT *pic, int cx, int cy, int rx1, int ry1, int rx2, int ry2,
	    int startangle, int endangle, int angledirection, int rightform, char *s, VECTOR_FORM *form);

// bitmap 에 관련된 함수
void PictureBitmapSetDac(PICTURE_BITMAP_STRUCT *bmp, BYTE *dac);
int  PictureBitmapLoadNew(HWND hwnd, PICTURE_BITMAP_STRUCT *bmp, int width, int height, int bitsperpixel, BYTE *dac);
HPALETTE PictureBitmapMakePalette(PICTURE_BITMAP_STRUCT *bmp);
void PictureBitmapLock(PICTURE_BITMAP_STRUCT *bmp);
void PictureBitmapUnlock(PICTURE_BITMAP_STRUCT *bmp);
void PictureBitmapDelete(PICTURE_BITMAP_STRUCT *bmp);
void PictureBitmapGetSize(PICTURE_BITMAP_STRUCT *bmp, int &width, int &height);
int PictureBitmapGetColor(PICTURE_BITMAP_STRUCT *bmp);
void PictureBitmapPutOneLine(PICTURE_BITMAP_STRUCT *bmp, int y, BYTE *buf);
void PictureBitmapPutBuf(PICTURE_BITMAP_STRUCT *bmp, BYTE *buf, int x, int y, int width);
void PictureBitmapGetOneLine(PICTURE_BITMAP_STRUCT *bmp, int y, BYTE *buf);
void PictureBitmapGetBuf(PICTURE_BITMAP_STRUCT *bmp, BYTE *buf, int x, int y, int width);
void PictureBitmapGetOneLineVertical(PICTURE_BITMAP_STRUCT *bmp, int x, BYTE *buf);
void PictureBitmapPutImage(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x, int y);
void PictureBitmapPutImageViewRect(HDC hdc, PICTURE_BITMAP_STRUCT *pic, int x, int y, RECT *rect);
void PictureBitmapPutImage(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x1, int y1, int x2, int y2);
void PictureBitmapPutImageViewRect(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x1, int y1, int x2, int y2, RECT *rect);
void PictureBitmapRestoreBack(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x, int y, int x1, int y1, int x2, int y2);
void PictureBitmapPutPixel(PICTURE_BITMAP_STRUCT *bmp, int x, int y, COLORREF color);
COLORREF PictureBitmapGetPixel(PICTURE_BITMAP_STRUCT *bmp, int x, int y);
void PictureBitmapCopy(PICTURE_BITMAP_STRUCT *target, PICTURE_BITMAP_STRUCT *source);

HPICTURE PictureLoad(HWND hwnd, char *filename);
HPICTURE PictureLoadPcx(HWND hwnd, char *filename);
HPICTURE PictureLoadBmp(HWND hwnd, const char *filename);
HPICTURE PictureLoadLbm(HWND hwnd, char *filename);
HPICTURE PictureLoadSpt(HWND hwnd, char *filename);
HPICTURE PictureLoadMmp(HWND hwnd, char *filename);
HPICTURE PictureLoadTif(HWND hwnd, char *filename);
HPICTURE PictureLoadTga(HWND hwnd, char *filename);
HPICTURE PictureLoadJpg(HWND hwnd, char *filename);
HPICTURE PictureLoadGif(HWND hwnd, char *filename);
HPICTURE PictureLoadPng(HWND hwnd, const char *filename);

void PictureSetLock(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);
int  PictureIsPaintableZone(PICTURE_STRUCT *pic, int x, int y);
#endif
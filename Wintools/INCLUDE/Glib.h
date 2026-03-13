#if	!defined (__GLIB_H)
#define __GLIB_H

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#if	!defined (__AFXCMN_H__)
#include <afxcmn.h>
#endif

#pragma pack(push, 1)

#define	COLOR_2				1
#define	COLOR_16			4
#define	COLOR_256			8
#define	COLOR_32768			15
#define	COLOR_65536			16
#define	COLOR_1600			24
#define	COLOR_32BIT			32

#define	WHITE_COLOR			RGB(255, 255, 255)
#define	DARK_COLOR  		RGB(0, 0, 0)
#define WHITE_GRAY_COLOR	RGB(192, 192, 192)
#define	DARK_GRAY_COLOR	RGB(128, 128, 128)

void DrawBitmapTile(HDC hdc, int x1, int y1, int x2, int y2, HBITMAP hBitmap);

void PopRectangle(HDC hdc, int x1, int y1, int x2, int y2);
void PopRectangle(HDC hdc, RECT *rect);
void PopBox(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color);
void PopBox(HDC hdc, RECT *rect, COLORREF color);

void PushRectangle(HDC hdc, int x1, int y1, int x2, int y2);
void PushRectangle(HDC hdc, RECT *rect);
void PushBox(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color);
void PushBox(HDC hdc, RECT *rect, COLORREF color);

void PopRectangle2(HDC hdc, int x1, int y1, int x2, int y2);
void PopRectangle2(HDC hdc, RECT *rect);
void PopBox2(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color);
void PopBox2(HDC hdc, RECT *rect, COLORREF color);

void PushBox3(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color);
void PushRectangle3(HDC hdc, int x1, int y1, int x2, int y2);


void PushRectangle2(HDC hdc, int x1, int y1, int x2, int y2);
void PushRectangle2(HDC hdc, RECT *rect);
void PushBox2(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color);
void PushBox2(HDC hdc, RECT *rect, COLORREF color);

void PopBoxBitmap(HDC hdc, int x1, int y1, int x2, int y2, HBITMAP hBitmap);
void PopBoxBitmap2(HDC hdc, int x1, int y1, int x2, int y2, HBITMAP hBitmap);

void gcls(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color);
void grect(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color);
void gcls(HDC hdc, RECT *rect, COLORREF color);

void gbezier(HDC hdc, void *p, int npts, int segments);

int  ButtonCheckDown2(HWND hwnd, LPARAM lParam, int x1, int y1, int x2, int y2, int id);
int  ButtonCheckMove2(HWND hwnd, LPARAM lParam);	// return 1 = ButtonCheck가 진행중이다.
int  ButtonCheckUp2(HWND hwnd);

void FloatBoxPaint(HWND hwnd, HDC hdc);
void FloatBoxLButtonDown(HWND hwnd, LPARAM lParam, HMENU hMenu);
LRESULT FloatBoxMouseMove(HWND hwnd, LPARAM lParam);
void FloatBoxLButtonUp(HWND hwnd);

void MoveWindowCenter(HWND hwnd);

int ExecuteDialogBox(HINSTANCE hinst, LPCTSTR lpszDlg, HWND hwnd, DLGPROC dlgproc);

int BmpWidthToByte(int width, int bitsperpixel);

void MsgBoxLocalMemoryLow(HWND hwnd,  char *string);
void MsgBoxGlobalMemoryLow(HWND hwnd, char *string);

// 0부터 시작하는 연속되는 radio button에서 현재 선택된 값을 읽어온다. 
int  GetRadioPosition(HWND hDlg, int start_id, int count);
void SetRadioPosition(HWND hDlg, int start_id, int count, int value);

//	주어진 DAC에서 RGB에 가장 접근한 값을 읽어온다.
int SeekFitRGB(BYTE *dac, int r, int g, int b, int colorhap);
HPALETTE MakePaletteWithDac(BYTE *dac, int pal_count);	// DAC로 팔레트를 만든다.

// 프로그램 중간에 메세지 박스를 뿌리기도 곤란하고 나중에라도 알아야 할 필요가 있을때 이에러 메세지를 발생해 준다.
void ProgrammError(LPSTR string, ...);

// Profile tools (profile.cpp)
void WritePrivateProfileInt(const TCHAR *section, const TCHAR *entry, int value, const TCHAR *filename);
void GetPrivateProfileDate(const TCHAR *section, TCHAR *entry, struct date *t, TCHAR *filename);
void GetPrivateProfileTime(const TCHAR *section, TCHAR *entry, struct time *t, TCHAR *filename);
void WritePrivateProfileDate(const TCHAR *section, TCHAR *entry, struct date *t, TCHAR *filename);
void WritePrivateProfileTime(const TCHAR *section, TCHAR *entry, struct time *t, TCHAR *filename);
void WritePrivateProfileFloat(const TCHAR *section, TCHAR *entry, float value, TCHAR *filename);
float GetPrivateProfileFloat(const TCHAR *section, TCHAR *entry, float default_value, TCHAR *filename);

// Profile UTF8
void GetProfileStringFromUTF8(const char *section, const char *item, const char *default_value, CString &buf, const char *path);

extern	unsigned char DEFAULT_RGB[768];
extern	BYTE FILL_PATTERN_SYSTEM[30][8];
extern	BYTE BIT_MASK[8];
extern	WORD WORD_MASK[16];
extern	BYTE DITHER_TABLE[64][8];
extern  DWORD DWORD_MASK[32]; 

inline	WORD 	rgb(BYTE r, BYTE g, BYTE b)
{ return ((WORD)( (r << 2) | (g >> 3) ) << 8) | ((WORD) ((g << 5) | b)); }

inline  BYTE getrvalue(WORD value) { return (((BYTE) (value >> 10)) &0x1F); }
inline  BYTE getgvalue(WORD value) { return (((BYTE) (value >> 5)) & 0x1F); }
inline  BYTE getbvalue(WORD value) { return (((BYTE) (value)) & 0x1F); }

void RGBQUAD2DAC(RGBQUAD *quad, BYTE *dac, int count);

// DIB 정보를 얻는데 사용하는 함수.
WORD GetDibWidth (BYTE * lpDib);
WORD GetDibHeight (BYTE * lpDib);
BYTE * GetDibBitsAddr (BYTE * lpDib);

// Icon에 string을 나타나게 하는 함수집합.
void IconInfoStringRegisterClass(HINSTANCE hInstance);
void IconInfoStringClose();
int  IconInfoStringCheck(HWND hwnd, int x1, int y1, int x2, int y2, LPARAM lParam, char *text);
void IconInfoSetParentHWND(HWND hwnd);

class WaitCursorClass {
		HCURSOR hCursorOld;
	public:
		WaitCursorClass();
		~WaitCursorClass();
		void Restore();
};

void MessageScreen(const char *title, const char *string, ...);
void MessageScreenRegisterClass(HWND hwnd, HINSTANCE hInstance);
void MessageScreenHide();
void MessageScreenSetLifeTime(int time);
void MessageScreenSetParentHWND(HWND hwnd);
void MessageScreenSetShowMethod(int method, char bTopMost);

void LogoScreen(const char *filename);
void LogoScreenRegisterClass(HWND hwnd, HINSTANCE hInstance);
void LogoScreenHide();
void LogoScreenSetLifeTime(int time);

// 이 함수를 WM_PAINT메세지에서 Global Unlock 한 후에 불러주면 미처 Unlock 하지 못한 부분의 에러를 잡아준다.
void DrawGlobalLockCount(HDC hdc, HGLOBAL hGlobal); 

void DrawUserListCtrl(LPDRAWITEMSTRUCT lpDrawItemStruct, CListCtrl &m_list);

#pragma pack(pop)

#endif

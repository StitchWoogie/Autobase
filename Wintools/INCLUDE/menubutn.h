#if	!defined (__WINDOWS_H)
#include <windows.h>
#endif

#pragma pack(push, 1)

class MenuButtonClass {
typedef struct {
		HWND  	hwnd; 	// 각 버턴의 핸들
		unsigned  id;   	// 고유 ID
		int      sHap;		// 스트링의 길이
} BUTTON_STRUCT;
#define	MAX_BUTTON	20
		BUTTON_STRUCT button[MAX_BUTTON];
		HWND  hWnd;
		HINSTANCE hInst;
		int   nButtonHap;
		int   cxChar;
		int   cyChar;
		int   nWidth;
		int   nHeight;
		int   nStartX;
		int	nStartY;
	  public:
		void Init(HWND hwnd, HINSTANCE hInstance);
		~MenuButtonClass();
		void Insert(const char *title, unsigned id);
		void SetFont(HFONT hFont);
		void SetFontSize(int x, int y) { cxChar = x, cyChar = y; }
		int  GetNeedSizeY(int width);
		void Move(int x, int y);
		void Paint(HDC hdc);
		int  GetHeight() { return nHeight; }
		void DisableAll();
		BOOL Enable(unsigned id, int flag);
      void SetText(unsigned id, char *text);
};

#pragma pack(pop)


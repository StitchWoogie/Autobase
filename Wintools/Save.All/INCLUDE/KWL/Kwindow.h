#if	!defined (__KWINDOW_H)
#define __KWINDOW_H

#if	!defined (__AFX_H)
#include <afx.h>
#endif

class KWindow {
		int nInitWidth, nInitHeight;
		DWORD dwWindowStyle;
		char *szClassName;

		void WindowRegister(HINSTANCE hPrevInstance, HINSTANCE hInstance);
		
	protected:
		int nInitX, nInitY;
	   HWND hwndThis;
		HWND hwndParent;
		WPARAM wParamThis;
		LPARAM lParamThis;
		int nFontWidth, nFontHeight;		// cxChar, cyChar WM_CREATE시 자동으로 계산되어진다.

		virtual void OnCreate();
		virtual void OnSize();
		virtual void OnMove() {}
		virtual void WmCreate();
		virtual void WmMove() {}
		virtual void WmLButtonDown() {}
		virtual void WmMouseMove() {}
		virtual void WmLButtonUp() {}
		virtual void WmRButtonDown() {}
		virtual void WmRButtonUp() {}
		virtual int  WmCommand() { return 0; }
		virtual void WmSize() {}
		virtual void WmInitMenuPopup() {}
		virtual void WmDestroy() {}
		virtual void WmPaint(HDC hdc);
		virtual int  WmQueryEndSession();
		virtual void WmHScroll() {}
		virtual void WmVScroll() {}
		virtual LRESULT ElseWindowProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

		BOOL GetClientRect(LPRECT lpRect) { return ::GetClientRect(hwndThis, lpRect); }
	public:
		KWindow();
		~KWindow();

		LRESULT ProcFunction(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam);
		
		HWND Create(HWND parent, char *szClassName, char *title, HMENU hMenuInit, HINSTANCE hInstance);
		
		BOOL ShowWindow(int nCmdShow) { return ::ShowWindow(hwndThis, nCmdShow); }
		BOOL UpdateWindow()				{ return ::UpdateWindow(hwndThis); }
		HWND GetThisHWND()						{ return hwndThis; }
		BOOL MoveWindow(int x, int y, int width, int height, BOOL repaint);

		void SetWindowStyle(DWORD dwStyle);
		void SetThisHWND(HWND hwnd)			{ hwndThis = hwnd; }
		void GetClassName(char *classname) { strcpy(classname, szClassName); }
		HWND GetParent() { return hwndParent; }
};

#endif

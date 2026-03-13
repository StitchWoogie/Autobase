#if	!defined (__KMFRMWND_H)
#define	__KMFRMWND_H

#if	!defined (__KFRMWND_H)
#include "kfrmwnd.h"
#endif

class KMdiFrameWindow : public KFrameWindow {
	protected:
		HWND hMenuWindow;				// child윈도우를 배열할 메뉴 핸들
		UINT idFirstChild;

		LRESULT ElseWindowProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
	public:
		KMdiFrameWindow();
		~KMdiFrameWindow();
		HWND GetClientWindow() { return hwndClientThis; }
		void OnCreate();
		void MdiSetMenu(HMENU hMenu, HMENU hmenuwindow);
		void SetFirstChildID(UINT id);
};		


#endif

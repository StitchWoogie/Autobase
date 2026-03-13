#if	!defined (__KTOOLWND_H)
#define __KTOOLWND_H

#if	!defined (__KWINDOW_H)
#include "kwindow.h"
#endif

enum {
		TOOL_ZONE_FLOAT,
		TOOL_ZONE_TOP,
		TOOL_ZONE_BOTTOM,
		TOOL_ZONE_LEFT,
		TOOL_ZONE_RIGHT,
};

typedef struct {			// ToolBox환경을 저장할 필요가 있을때 이 구조체를 통채로 저장한다.
		WORD	struct_size;
		short	zone;			// tool box가 존재하는 위치.
		int   x;
		int   y;
} TOOL_BOX_CONFIG_STRUCT;

class KToolWindow : public KWindow {
		TOOL_BOX_CONFIG_STRUCT toolBoxConfig;
		int nGabX;
		int nGabY;
		char bMouseLeftFlag;
		RECT rOld;

		int GetMouseZone(short mx, short my);
		int  GetBoxSize(int zone, int *width, int *height);
		void InitConfigStruct(TOOL_BOX_CONFIG_STRUCT *config);
		void OnMove(LPARAM lParam);
		//int GetZone(RECT *r, int zone);
	public:
		KToolWindow();
		~KToolWindow();
		virtual void WmLButtonDown(WPARAM wParam, LPARAM lParam);
		virtual void WmMouseMove(WPARAM wParam, LPARAM lParam);
		virtual void WmLButtonUp(WPARAM wParam, LPARAM lParam);
		virtual void GetSizeBoxHorz(int *x, int *y);
		virtual void GetSizeBoxVert(int *x, int *y);
		virtual void GetSizeBoxFloat(int *x, int *y);
		void GetConfigStruct(TOOL_BOX_CONFIG_STRUCT *config);
		void SetConfigStruct(TOOL_BOX_CONFIG_STRUCT *config);	
		HWND Register(HWND parent, char *szClassName, char *title, HMENU hMenuInit, HINSTANCE hInstance);
};

#endif

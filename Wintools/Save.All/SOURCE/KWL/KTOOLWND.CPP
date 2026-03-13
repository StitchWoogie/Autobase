#include <tools.h>

#include "ktoolwnd.h"
#include "kfrmwnd.h"

KToolWindow :: KToolWindow() : KWindow()
{
	bMouseLeftFlag = OFF;
	InitConfigStruct(&toolBoxConfig);
}

KToolWindow :: ~KToolWindow()
{

}

void KToolWindow :: GetSizeBoxHorz(int *x, int *y)
{
	*x = 50;
	*y = 200;
}

void KToolWindow :: GetSizeBoxVert(int *x, int *y)
{
	*x = 200;
	*y = 50;
}


void KToolWindow :: GetSizeBoxFloat(int *x, int *y)
{
	*x = 100;
	*y = 100;
}

static void ClientRectToScreen(HWND hwnd, RECT *r)
{
	POINT pt;
	
	pt.x = r->left;
	pt.y = r->top;
	ClientToScreen(hwnd, &pt);
	r->left = pt.x;
	r->top = pt.y;

	pt.x = r->right;
	pt.y = r->bottom;
	ClientToScreen(hwnd, &pt);
	r->right = pt.x;
	r->bottom = pt.y;
}

static void ScreenRectToClient(HWND hwnd, RECT *r)
{
	POINT pt;
	
	pt.x = r->left;
	pt.y = r->top;
	ScreenToClient(hwnd, &pt);
	r->left = pt.x;
	r->top = pt.y;

	pt.x = r->right;
	pt.y = r->bottom;
	ScreenToClient(hwnd, &pt);
	r->right = pt.x;
	r->bottom = pt.y;
}

void KToolWindow :: WmLButtonDown(WPARAM wParam, LPARAM lParam)
{
	RECT r;
//	POINT p;
	short mx, my;
	mx = LOWORD(lParam), my = HIWORD(lParam);

	/*
	if(hMenu != NULL) {
		if(mx >= 3 && my >= 3 &&
			mx <= 20 && my <= 13) {
			p.x = 0;
			p.y = 13;
			ClientToScreen(hwnd, &p);
			TrackPopupMenu(hMenu, TPM_LEFTALIGN, p.x, p.y, 0, hwnd, NULL);
			return;
		}
	}
	*/

	GetClientRect(hwndThis, &r);

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	bMouseLeftFlag = ON;
	SetCapture(hwndThis);

	HDC hdc;

	hdc = GetDC(NULL);

	ClientRectToScreen(hwndThis, &r);
	memcpy(&rOld, &r, sizeof(RECT));

	InvertRect(hdc, &r);
	ReleaseDC(hwndThis, hdc);

	nGabX = mx;
	nGabY = my;
}

int KToolWindow :: GetMouseZone(short mx, short my)
{
	RECT r, rParent;
	POINT p;

	p.x = mx;
	p.y = my;
	ClientToScreen(hwndThis, &p);
	mx = (int)p.x;
	my = (int)p.y;

	KFrameWindow *window = (KFrameWindow*) GetWindowLong(hwndParent, GWL_USERDATA);
	
	window->GetWorkRect(&r);
	GetClientRect(hwndParent, &rParent);
	ClientRectToScreen(hwndParent, &r);
	ClientRectToScreen(hwndParent, &rParent);
	//ScreenRectToClient(hwndThis, &r);

	if(mx < rParent.left || mx > rParent.right || my < rParent.top || my > rParent.bottom)	return TOOL_ZONE_FLOAT;

	if(my <= r.top+20)		return TOOL_ZONE_TOP;
	if(my >= r.bottom-20)	return TOOL_ZONE_BOTTOM;
	if(mx <= r.left+20)	{
		return TOOL_ZONE_LEFT;
	}
	if(mx >= r.right-20)		return TOOL_ZONE_RIGHT;

	return TOOL_ZONE_FLOAT;
}

int KToolWindow :: GetBoxSize(int zone, int *width, int *height)
{
	switch(zone) {
		case TOOL_ZONE_LEFT:
			GetSizeBoxVert(width, height);
			break;
		case TOOL_ZONE_TOP:
			GetSizeBoxHorz(width, height);
			break;
		case TOOL_ZONE_RIGHT:
			GetSizeBoxVert(width, height);
			break;
		case TOOL_ZONE_BOTTOM:
			GetSizeBoxHorz(width, height);
			break;
		case TOOL_ZONE_FLOAT:
			GetSizeBoxFloat(width, height);
			break;
	}

	return 1;
}

void KToolWindow :: WmMouseMove(WPARAM wParam, LPARAM lParam)
{
	if(bMouseLeftFlag == OFF)	return;	// 마우스가 눌러져 있지 않다.

	short mx, my;
	RECT r;
	HDC hdc;
	int zone;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	zone = GetMouseZone((short)mx, (short)my);

	hdc = GetDC(NULL);
	InvertRect(hdc, &rOld);

	if(zone == toolBoxConfig.zone) {
		GetClientRect(hwndThis, &r);
		r.left  += mx-nGabX;
		r.top   += my-nGabY;
		r.right += mx-nGabX;
		r.bottom+= my-nGabY;
		ClientRectToScreen(hwndThis, &r);
	}
	else {
		int width, height;
		POINT p;

		GetClientRect(hwndParent, &r);
		ClientRectToScreen(hwndParent, &r);

		p.x = mx;
		p.y = my;
		ClientToScreen(hwndThis, &p);
		mx = (short)p.x;
		my = (short)p.y;

		GetBoxSize(zone, &width, &height);

		r.left  = mx-width/2;
		r.top   = my-height/2;
		r.right = r.left+width;
		r.bottom= r.top +height;

		//ClientRectToScreen(hwndParent, &r);
	}
	memcpy(&rOld, &r, sizeof(RECT));
	InvertRect(hdc, &r);
	ReleaseDC(hwndThis, hdc);

	//MoveWindow(hwndThis, pt.x, pt.y, rect.right, rect.bottom, TRUE);

	return;
}

void KToolWindow :: WmLButtonUp(WPARAM wParam, LPARAM lParam)
{
	if(bMouseLeftFlag == OFF)	return;
	bMouseLeftFlag = OFF;
	ReleaseCapture();

	HDC hdc;

	hdc = GetDC(NULL);
	InvertRect(hdc, &rOld);
	ReleaseDC(hwndThis, hdc);

	short mx, my;
	POINT pt;
	RECT r;
	int zone;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	if(mx == nGabX && my == nGabY)	return;

	zone = GetMouseZone(mx, my);

	if(zone == TOOL_ZONE_FLOAT && toolBoxConfig.zone != TOOL_ZONE_FLOAT) {	// Box가 바깥에서 중간으로 빠져 나왔을 때
		int width, height;
		POINT p;

		toolBoxConfig.zone = zone;

		p.x = mx;
		p.y = my;
		ClientToScreen(hwndThis, &p);
		mx = (short)p.x;
		my = (short)p.y;

		GetBoxSize(zone, &width, &height);

		r.left  = mx-width/2;
		r.top   = my-height/2;

		//ScreenRectToClient(hwndParent, &r);
		
		KFrameWindow *window = (KFrameWindow*)GetWindowLong(hwndParent, GWL_USERDATA);
		window->DeleteFloatBox(hwndThis, r.left, r.top);
	}
	else if(zone != TOOL_ZONE_FLOAT && toolBoxConfig.zone == TOOL_ZONE_FLOAT) {	// 떠있는 Box를 바깥으로 배치
		int width, height;
		POINT p;

		toolBoxConfig.zone = zone;

		GetClientRect(hwndParent, &r);
		ClientRectToScreen(hwndParent, &r);

		p.x = mx;
		p.y = my;
		ClientToScreen(hwndThis, &p);
		mx = (short)p.x;
		my = (short)p.y;

		GetBoxSize(zone, &width, &height);

		r.left  = mx-width/2;
		r.top   = my-height/2;
		r.right = r.left+width;
		r.bottom= r.top +height;

		ScreenRectToClient(hwndParent, &r);

		KFrameWindow *window = (KFrameWindow*) GetWindowLong(hwndParent, GWL_USERDATA);
		window->AddFloatBox(hwndThis, r.left, r.top, width, height, zone);
	}
	else if(zone == TOOL_ZONE_FLOAT && toolBoxConfig.zone == TOOL_ZONE_FLOAT) {	// box가 여전히 떠있는 상태이다.
		GetClientRect(hwndThis, &r);

		pt.x = r.left+mx-nGabX;
		pt.y = r.top +my-nGabY;

		ClientToScreen(hwndThis, &pt);

		MoveWindow(hwndThis, pt.x, pt.y, r.right, r.bottom, TRUE);
	}
	else if(zone != toolBoxConfig.zone) {		// 역시 바깥 방향에 있으나 다른곳에 위치하였을 때
		int width, height;
		POINT p;

		toolBoxConfig.zone = zone;

		GetClientRect(hwndParent, &r);
		ClientRectToScreen(hwndParent, &r);

		p.x = mx;
		p.y = my;
		ClientToScreen(hwndThis, &p);
		mx = (short)p.x;
		my = (short)p.y;

		GetBoxSize(zone, &width, &height);

		r.left  = mx-width/2;
		r.top   = my-height/2;
		r.right = r.left+width;
		r.bottom= r.top +height;

		ScreenRectToClient(hwndParent, &r);

		KFrameWindow *window = (KFrameWindow*) GetWindowLong(hwndParent, GWL_USERDATA);
		window->ChangeFloatBox(hwndThis, r.left, r.top, width, height, zone);
	}
	else {		// 같은 위치에서 박스가 조금 이동 했을 경우.
		int width, height;

		GetClientRect(hwndThis, &r);
		GetBoxSize(zone, &width, &height);

		r.left  += mx-nGabX;
		r.top   += my-nGabY;
		r.right = r.left+width;
		r.bottom= r.top +height;

		ClientRectToScreen(hwndThis, &r);
		ScreenRectToClient(hwndParent, &r);

		KFrameWindow *window = (KFrameWindow*) GetWindowLong(hwndParent, GWL_USERDATA);
		window->ChangeFloatBox(hwndThis, r.left, r.top, width, height, zone);
	}
}

void KToolWindow :: GetConfigStruct(TOOL_BOX_CONFIG_STRUCT *config)
{
	RECT r;

	GetWindowRect(hwndThis, &r);

	config->struct_size = sizeof(TOOL_BOX_CONFIG_STRUCT);
	config->zone = toolBoxConfig.zone;
	config->x = r.left;
	config->y = r.right;
}

void KToolWindow :: InitConfigStruct(TOOL_BOX_CONFIG_STRUCT *config)
{
	config->struct_size = sizeof(TOOL_BOX_CONFIG_STRUCT);
	config->zone = TOOL_ZONE_FLOAT;
	config->x = nInitX;
	config->y = nInitY;
}

void KToolWindow :: SetConfigStruct(TOOL_BOX_CONFIG_STRUCT *config)
{
	memcpy(&toolBoxConfig, config, sizeof(TOOL_BOX_CONFIG_STRUCT));
}

HWND KToolWindow :: Register(HWND parent, char *szClassName, char *title, HMENU hMenuInit, HINSTANCE hInstance)
{
	if(toolBoxConfig.struct_size != sizeof(TOOL_BOX_CONFIG_STRUCT)) {	// 구조체가 깨져 있다.
		InitConfigStruct(&toolBoxConfig);	
	}

	int width, height;
	
	GetBoxSize(toolBoxConfig.zone, &width, &height);

	if(toolBoxConfig.zone == TOOL_ZONE_FLOAT) {
		SetWindowStyle(WS_POPUP);
	}
	else {
		SetWindowStyle(WS_CHILD);
	}

	SetWindowSize(toolBoxConfig.x, toolBoxConfig.y, width, height);
	Create(parent, szClassName, title, hMenuInit, hInstance);

	if(toolBoxConfig.zone == TOOL_ZONE_FLOAT) {
		ShowWindow(SW_SHOWNA);
		UpdateWindow();
		return hwndThis;
	}

	KFrameWindow *window = (KFrameWindow*) GetWindowLong(parent, GWL_USERDATA);

	window->PrepareToolBox(hwndThis, &toolBoxConfig, width, height);

	return hwndThis;
}

void KToolWindow :: OnMove(LPARAM lParam)
{
	toolBoxConfig.x = LOWORD(lParam);
	toolBoxConfig.y = HIWORD(lParam);
}
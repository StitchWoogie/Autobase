#include <kwl\kwindow.h>

static KWindow *windowCreate = NULL;
HINSTANCE hPrevInstApplication;
 
LRESULT CALLBACK StandardWndProc (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	if(windowCreate != NULL) {
		SetWindowLong(hwnd, GWL_USERDATA, (LONG)windowCreate);
		windowCreate->SetThisHWND(hwnd);
		windowCreate = NULL;
	}

	KWindow *window = (KWindow*) GetWindowLong(hwnd, GWL_USERDATA);

	return window->ProcFunction(hwnd, message, wParam, lParam);
}

KWindow :: KWindow()
{
	szClassName = NULL;
	hwndThis = NULL;
	hwndParent = NULL;

	nInitX = CW_USEDEFAULT;
	nInitY = CW_USEDEFAULT;
	nInitWidth  = CW_USEDEFAULT;
	nInitHeight = CW_USEDEFAULT;

	dwWindowStyle = WS_OVERLAPPEDWINDOW | WS_CLIPCHILDREN;
}

KWindow :: ~KWindow()
{
	if(szClassName != NULL) {
		delete szClassName;
		szClassName = NULL;
	}
}

int KWindow :: WmQueryEndSession()
{
	return 1;
}

void KWindow :: OnSize()
{

}

LRESULT KWindow :: ProcFunction(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	wParamThis = wParam;
	lParamThis = lParam;

	switch(message) {
		case WM_CREATE:
			{
				TEXTMETRIC tm;
				HDC hdc;

				hdc = GetDC(hwnd);
				GetTextMetrics(hdc, &tm);
				ReleaseDC(hwnd, hdc);

				nFontWidth = tm.tmAveCharWidth;
				nFontHeight = tm.tmExternalLeading+tm.tmHeight;
			}
			OnCreate();
			WmCreate();
			return 0;
		case WM_COMMAND:
			WmCommand();
			//break;
			return 0;
		case WM_SIZE:
			OnSize();
			WmSize();
			return 0;
		case WM_PAINT:
			{
				HDC hdc;
				PAINTSTRUCT ps;
				
				hdc = BeginPaint(hwnd, &ps);
				WmPaint(hdc);
				EndPaint(hwnd, &ps);
			}
			return 0;
		case WM_LBUTTONDOWN:			WmLButtonDown();			return 0;
		case WM_MOUSEMOVE:			WmMouseMove();				return 0;
		case WM_LBUTTONUP:			WmLButtonUp();				return 0;
		case WM_RBUTTONDOWN:			WmRButtonDown();			return 0;
		case WM_RBUTTONUP:			WmRButtonUp();				return 0;
		case WM_MOVE:
			OnMove();
			WmMove();
			return 0;
		case WM_VSCROLL:
			WmVScroll();
			return 0;
		case WM_HSCROLL:
			WmHScroll();
			return 0;
		case WM_CLOSE:
		case WM_QUERYENDSESSION:
			if(WmQueryEndSession() == 0)	return 0;
			break;
		case WM_DESTROY:
			WmDestroy();
			hwndThis = NULL;
			return 0;
		default:	
			break;
	}
	return ElseWindowProc (hwnd, message, wParam, lParam);
}


LRESULT KWindow :: ElseWindowProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	return DefWindowProc (hwnd, message, wParam, lParam);
}

void KWindow :: WindowRegister(HINSTANCE hPrevInstance, HINSTANCE hInstance)
{
	if(hPrevInstance)		return;

	WNDCLASS wndclass ;

	// Register the frame window class

	wndclass.style         = CS_HREDRAW | CS_VREDRAW ;
	wndclass.lpfnWndProc   = StandardWndProc ;
	wndclass.cbClsExtra    = 0;
	wndclass.cbWndExtra    = sizeof (LONG);
	wndclass.hInstance     = hInstance ;
	wndclass.hIcon         = NULL;// LoadIcon (hInstance, MAKEINTRESOURCE(ICON_SUCHEWHA_MAIN));
	wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW) ;
	wndclass.hbrBackground = (HBRUSH)GetStockObject (WHITE_BRUSH);
	wndclass.lpszMenuName  = NULL ;
	wndclass.lpszClassName = szClassName;

	RegisterClass (&wndclass) ;
}

HWND KWindow :: Create(HWND hwndparent, char *classname, char *title, HMENU hMenuInit, HINSTANCE hInstance)
{
	if(szClassName != NULL) {
		delete szClassName;
		szClassName = NULL;
	}
	szClassName = new char[strlen(classname)+1];
	strcpy_s(szClassName, strlen(classname)+1, classname);
	
	hwndParent = hwndparent;
	windowCreate = this;
	
	WindowRegister(hPrevInstApplication, hInstance);

	hwndThis = CreateWindow (szClassName,
								title, 
								dwWindowStyle,
  								nInitX, nInitY, nInitWidth, nInitHeight,
								hwndparent, hMenuInit, hInstance, NULL);

	return hwndThis;
}

void KWindow :: OnCreate()
{

}

void KWindow :: WmCreate()
{

}

void KWindow :: WmPaint(HDC hdc)
{

}

BOOL KWindow :: MoveWindow(int x, int y, int width, int height, BOOL repaint)
{
	nInitX = x;
	nInitY = y;
	nInitWidth = width;
	nInitHeight= height;

	if(hwndThis != NULL) {
		return ::MoveWindow(hwndThis, x, y, width, height, repaint);
	}

	return TRUE;
}

void KWindow :: SetWindowStyle(DWORD dwStyle)
{
	dwWindowStyle = dwStyle;
}




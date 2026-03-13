#include "kfrmwnd.h"
#include "ktoolwnd.h"

extern HANDLE hInstApplication;
extern HANDLE hPrevInstApplication;

KFrameWindow :: KFrameWindow() : KWindow()
{
	hwndClientThis = NULL;

	blockFloatBox = new Block(sizeof(FLOAT_BOX_STRUCT));

	nLeftCount = 0;
	nRightCount = 0;
	nTopCount = 0;
	nBottomCount = 0;

	memset(&rLeft, 0, sizeof(RECT));
	memset(&rRight, 0, sizeof(RECT));
	memset(&rTop, 0, sizeof(RECT));
	memset(&rBottom, 0, sizeof(RECT));
}

KFrameWindow :: ~KFrameWindow()
{
	if(blockFloatBox   != NULL)		delete blockFloatBox;
}

void KFrameWindow :: DisplayFloatBox()
{
	RECT r;
	int  i;
	FLOAT_BOX_STRUCT item;

	GetClientRect(hwndThis, &r);

	MoveWindow(hwndClientThis, rLeft.right, rTop.bottom, 
						r.right-(rLeft.right-rLeft.left)-(rRight.right-rRight.left), 
						r.bottom-(rTop.bottom-rTop.top)-(rBottom.bottom-rBottom.top), 
						TRUE);

	for(i = 0; i < blockFloatBox->GetBlockCount(); i++) {
		blockFloatBox->GetBlock((BYTE*)&item, i);

		if(item.form == TOOL_ZONE_LEFT)
			MoveWindow(item.hwnd, rLeft.left+item.x, rLeft.top+item.y, item.width, item.height, TRUE);
		else if(item.form == TOOL_ZONE_TOP)
			MoveWindow(item.hwnd, rTop.left+item.x, rTop.top+item.y, item.width, item.height, TRUE);
		else if(item.form == TOOL_ZONE_RIGHT)
			MoveWindow(item.hwnd, rRight.left+item.x, rRight.top+item.y, item.width, item.height, TRUE);
		else
			MoveWindow(item.hwnd, rBottom.left+item.x, rBottom.top+item.y, item.width, item.height, TRUE);

		if(item.show == OFF) {
			item.show = ON;
			::ShowWindow(item.hwnd, SW_SHOW);
			blockFloatBox->SetBlock((BYTE*)&item, i);
		}
	}
}

//-----------------------------------------------------------------------------
//	ToolBox가 바깥에서 중앙(Float)으로 빠져나왔을 때 이 함수가 불러진다. 
//-----------------------------------------------------------------------------

void KFrameWindow :: DeleteFloatBox(HWND hwnd, int x, int y)
{
	FLOAT_BOX_STRUCT item;
	int i;

	for(i = 0; i < blockFloatBox->GetBlockCount(); i++) {
		blockFloatBox->GetBlock((BYTE*)&item, i);
		if(item.hwnd == hwnd) {
			DeleteAtRectBox(i, &item);
			blockFloatBox->DeleteBlock(i);
	
			KToolWindow *window = (KToolWindow*)GetWindowLong(hwnd, GWL_USERDATA);
			char classname[MAXPATH];
			HWND parent;
			int width, height;

			window->GetClassName(classname);
			parent = window->GetParent();
			window->GetSizeBoxFloat(&width, &height);

			window->ShowWindow(SW_HIDE);
			DestroyWindow(hwnd);
			window->SetWindowStyle(WS_POPUP);
			window->SetWindowSize(x, y, width, height);
			window->Create(parent, classname, "", NULL, hInstApplication);
			window->ShowWindow(SW_SHOW);
			window->UpdateWindow();

			DisplayFloatBox();
			
			return;
		}
	}
}

void KFrameWindow :: AddFloatBox(HWND hwnd, int x, int y, int width, int height, int form)
{
	FLOAT_BOX_STRUCT item;
	RECT r;

	GetClientRect(hwndThis, &r);

	item.hwnd = hwnd;
	item.x = x;
	item.y = y;
	item.width  = width;
	item.height = height;
	item.form = form;
	item.show = OFF;

	blockFloatBox->InsertBlock((BYTE*)&item, 0);
	InsertAtRectBox(0, &item, x, y, width, height);
	
	KToolWindow *window = (KToolWindow*)GetWindowLong(hwnd, GWL_USERDATA);
	char classname[MAXPATH];
	HWND parent;

	window->GetClassName(classname);
	parent = window->GetParent();

	window->ShowWindow(SW_HIDE);
	DestroyWindow(hwnd);
	window->SetWindowStyle(WS_CHILD);
	window->SetWindowSize(item.x, item.y, item.width, item.height);
	window->Create(parent, classname, "", NULL, hInstApplication);
	window->ShowWindow(SW_SHOW);

	item.hwnd = window->GetThisHWND();

	blockFloatBox->SetBlock((BYTE*)&item, 0);

	DisplayFloatBox();
}

void KFrameWindow :: OnSize()
{
	RECT r;

	GetClientRect(hwndThis, &r);

	if(nLeftCount) {
		rLeft.bottom = r.bottom;
	}
	if(nTopCount) {
		rTop.right = r.right;
	}
	if(nRightCount) {
		int size = rRight.right-rRight.left;
		
		rRight.left = r.right-size;
		rRight.right = r.right;
		rRight.bottom = r.bottom;
	}
	if(nBottomCount) {
		int size = rBottom.bottom-rBottom.top;
		
		rBottom.top = r.bottom-size;
		rBottom.bottom = r.bottom;
		rBottom.right = r.right;
	}

	DisplayFloatBox();
}

//----------------------------------------------
//	해당 박스에 포함시킨다.
//----------------------------------------------

void KFrameWindow :: InsertAtRectBox(int pos, FLOAT_BOX_STRUCT *item, int x, int y, int width, int height)
{
	RECT r;

	GetClientRect(hwndThis, &r);

	if(item->form == TOOL_ZONE_LEFT) {
		if(x < 0)	x = 0;
		if(y < 0)	y = 0;
		if(nLeftCount == 0) {
			rLeft.top = 0;
			rLeft.left = 0;
			rLeft.right = width;
			rLeft.bottom = r.bottom;
			x = 0;
		}
		else {
			if(x+width > rLeft.right)	rLeft.right = x+width;
		}
		nLeftCount++;
	}
	else if(item->form == TOOL_ZONE_RIGHT) {
		if(nRightCount == 0) {
			rRight.top =  0;
			rRight.left = r.right-width;
			rRight.right = r.right;
			rRight.bottom = r.bottom;
			x = 0;
		}
		else {
			if(x < rRight.left)	rRight.left = x;
			x -= rRight.left;
		}

		nRightCount++;
	}
	else if(item->form == TOOL_ZONE_TOP) {
		if(x < 0)	x = 0;
		if(y < 0)	y = 0;

		if(nTopCount == 0) {
			rTop.top = 0;
			rTop.left = 0;
			rTop.right = r.right;
			rTop.bottom = height;
			y = 0;
		}
		else {
			rTop.top = 0;
			rTop.left = 0;
			rTop.right = r.right;
			if(y+height > rTop.bottom)	rTop.bottom = y+height;
		}			
		nTopCount++;
	}
	else {
		if(nBottomCount == 0) {
			rBottom.top =  r.bottom-height;
			rBottom.left = 0;
			rBottom.right =  r.right;
			rBottom.bottom = r.bottom;
			y = 0;
		}
		else {
			if(y < rBottom.top)	rBottom.top = y;

			y -= rBottom.top;
		}
		nBottomCount++;
	}

	item->x = x;
	item->y = y;
	item->width  = width;
	item->height = height;
}

//----------------------------------------------
//	해당 박스에서 제외시킨다.
//----------------------------------------------

void KFrameWindow :: DeleteAtRectBox(int pos, FLOAT_BOX_STRUCT *item)
{
	if(item->form == TOOL_ZONE_LEFT) {
		nLeftCount--;
		if(nLeftCount == 0)	{
			memset(&rLeft, 0, sizeof(RECT));
		}
	}
	else if(item->form == TOOL_ZONE_RIGHT) {
		nRightCount--;
		if(nRightCount == 0)	{
			memset(&rRight, 0, sizeof(RECT));
		}
	}
	else if(item->form == TOOL_ZONE_TOP) {
		nTopCount--;
		if(nTopCount == 0)	{
			memset(&rTop, 0, sizeof(RECT));
		}
	}
	else {
		nBottomCount--;
		if(nTopCount == 0)	{
			memset(&rBottom, 0, sizeof(RECT));
		}
	}
}

//-----------------------------------------------------------------------------------------
//	해당 영역에서 움직이거나 다른 영역(Float Box는 제외)으로 옮겼을 때 이 함수가 불려진다.
//-----------------------------------------------------------------------------------------

void KFrameWindow :: ChangeFloatBox(HWND hwnd, int x, int y, int width, int height, int form)
{
	FLOAT_BOX_STRUCT item;
	int i;

	for(i = 0; i < blockFloatBox->GetBlockCount(); i++) {
		blockFloatBox->GetBlock((BYTE*)&item, i);
		if(item.hwnd == hwnd) {
			DeleteAtRectBox(i, &item);
			item.form = form;
			InsertAtRectBox(i, &item, x, y, width, height);
			blockFloatBox->SetBlock((BYTE*)&item, i);

			DisplayFloatBox();
			return;
		}
	}
}

void KFrameWindow :: GetWorkRect(RECT *r)
{
	GetClientRect(hwndThis, r);

	if(nLeftCount) {
		r->left = rLeft.right+1;
	}
	if(nTopCount) {
		r->top =  rTop.bottom+1;
	}
	if(nRightCount) {
		r->right = rRight.left-1;
	}
	if(nBottomCount) {
		r->bottom = rBottom.top-1;
	}
}

void KFrameWindow :: PrepareToolBox(HWND hwnd, TOOL_BOX_CONFIG_STRUCT *config, int width, int height)
{
	if(config->zone == TOOL_ZONE_FLOAT)	return;

	FLOAT_BOX_STRUCT item;

	item.hwnd = hwnd;
	item.x = config->x;
	item.y = config->y;
	item.width  = width;
	item.height = height;
	item.form = config->zone;
	item.show = OFF;

	blockFloatBox->InsertBlock((BYTE*)&item, 0);

	InsertAtRectBox(0, &item, config->x, config->y, width, height);

	blockFloatBox->SetBlock((BYTE*)&item, 0);

	DisplayFloatBox();
/*
	RECT r;
	FLOAT_BOX_STRUCT item;

	item.hwnd = hwnd;
	item.x = config->x;
	item.y = config->y;
	item.width  = width;
	item.height = height;
	item.form = config->zone;
	item.show = OFF;

	GetClientRect(hwndThis, &r);

	if(config->zone == TOOL_ZONE_LEFT) {
		if(config->x < rLeft.left) {
			rLeft.left = config->x;
		}
		if(config->x+width > rLeft.right) {
			rLeft.right = config->x+width;
		}
		nLeftCount++;
	}
	else if(config->zone == TOOL_ZONE_RIGHT) {
		if(config->x < rRight.left) {
			rRight.left = config->x;
		}
		if(config->x+width > rRight.right) {
			rRight.right = config->x+width;
		}
		nRightCount++;
	}
	else if(config->zone == TOOL_ZONE_TOP) {
		if(config->y < rTop.top) {
			rTop.top = config->y;
		}
		if(config->y+height > rTop.bottom) {
			rTop.bottom = config->y+height;
		}

		nTopCount++;
	}
	else {
		if(config->y < rBottom.top) {
			rBottom.top = config->y;
		}
		if(config->y+height > rBottom.bottom) {
			rBottom.bottom = config->y+height;
		}

		nBottomCount++;
	}

	blockFloatBox->AddBlock((BYTE*)&item);
*/
}



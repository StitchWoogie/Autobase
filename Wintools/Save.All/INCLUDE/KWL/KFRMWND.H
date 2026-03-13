#if	!defined (__KFRMWND_H)
#define __KFRMWND_H

#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

#if	!defined (__KWINDOW_H)
#include "kwindow.h"
#endif

#if	!defined (__KTOOLWND_H)
#include "ktoolwnd.h"
#endif

class KFrameWindow : public KWindow {
		typedef struct {
			HWND hwnd;
			int x;
			int y;
			int width;
			int height;
			int form;
			char show;
		} FLOAT_BOX_STRUCT;

		Block *blockFloatBox;
		int nLeftCount;
		int nRightCount;
		int nTopCount;
		int nBottomCount;

		RECT rLeft;
		RECT rRight;
		RECT rTop;
		RECT rBottom;

		void DeleteAtRectBox(int pos, FLOAT_BOX_STRUCT *item);
		void InsertAtRectBox(int pos, FLOAT_BOX_STRUCT *item, int x, int y, int width, int height);

	protected:
		HWND hwndClientThis;

		void OnSize();
	public:
		KFrameWindow();
		~KFrameWindow();
		void DeleteFloatBox(HWND hwnd, int x, int y);
		void AddFloatBox(HWND hwnd, int x, int y, int width, int height, int form);
		void ChangeFloatBox(HWND hwnd, int x, int y, int width, int height, int form);
		void DisplayFloatBox();
		void GetWorkRect(RECT *r);
		void PrepareToolBox(HWND hwnd, TOOL_BOX_CONFIG_STRUCT *config, int width, int height);
};

#endif

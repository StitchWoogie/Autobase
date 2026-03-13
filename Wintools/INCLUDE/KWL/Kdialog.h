#if	!defined (__KDIALOG_H)
#define	__KDIALOG_H

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#if	!defined (__PRINT_HPP)
#include <print.hpp>
#endif

#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

#pragma pack(push, 1)

class KDialog {
		char bCenterFlag;
	protected:
		HWND hwndDlg;
		HWND hwndOwner;
		WPARAM wParamThis;
		LPARAM lParamThis;
		HWND GetDlgItem(int id) { return :: GetDlgItem(hwndDlg, id); }
#if	defined (_WIN32)
		BOOL EndDialog(int result) { return ::EndDialog(hwndDlg, result); }
#else
		void EndDialog(int result) { ::EndDialog(hwndDlg, result); }
#endif
	public:
		KDialog();
		int run(HWND hwnd, WORD id, HINSTANCE hInstance);
		BOOL DialogProc( HWND hdlg, UINT message, WPARAM wParam, LPARAM lParam );
		virtual BOOL WmInitDialog();
		virtual BOOL WmCommand();
		virtual BOOL WmDrawItem();
		virtual BOOL WmTimer();
		virtual BOOL WmHScroll();
		virtual BOOL WmVScroll();
		void SetWindowCenter() { bCenterFlag = ON; }
};

class KDialogPrint : public KDialog {
		Block *block;
	protected:
		void FillPrintDriverComboBox(HWND hwndCombo, PRINT_DRIVER_LIST *defaultDriver);
		void GetSelectedDriver(HWND hwndCombo, PRINT_DRIVER_LIST *driver);
	public:
		KDialogPrint();
		~KDialogPrint();

		DWORD GetDriverCount() { return block->GetBlockCount(); }
};

#pragma pack(pop)

#endif



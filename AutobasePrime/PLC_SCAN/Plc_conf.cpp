// english O.K
#include "stdafx.h"
#include <stdlib.h>

#include <kwl\kdialog.h>

#include "plc_scan.h"
#include "resource.h"

class configDialog : public KDialog {
	public:
		configDialog() {}
		BOOL WmInitDialog();
		BOOL WmCommand();
		void EnableDisable();
};

void configDialog :: EnableDisable()
{
	char flag = IsDlgButtonChecked(hwndDlg, IDC_ConfigAll_CHKBOX_ALL_MSG);

	EnableWindow(GetDlgItem(IDC_ConfigAll_CHKBOX_TIMEOUT), flag);
	EnableWindow(GetDlgItem(IDC_ConfigAll_CHKBOX_CODEBAD), flag);
}

BOOL configDialog :: WmInitDialog()
{
	char buf[80];

	CheckDlgButton(hwndDlg, IDC_ConfigAll_CHKBOX_TIMEOUT, config.bMessageBoxTimeOut);
	CheckDlgButton(hwndDlg, IDC_ConfigAll_CHKBOX_CODEBAD, config.bMessageBoxCodeBad);
    CheckDlgButton(hwndDlg, IDC_ConfigAll_CHKBOX_ALL_MSG, config.bMessageBoxAllMessage);
	sprintf(buf, "%d", config.nScanTime);
    SetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_SCANTIME), buf);
	sprintf(buf, "%d", config.nSkipTimeOnTimeOut);
    SetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_TIMEOUT_SKIP_SEC), buf);

    CheckDlgButton(hwndDlg, IDC_ConfigAll_CHKBOX_MULTIPORT_MULTITASKING, config.bMultiPortMultiTasking);

	CheckDlgButton(hwndDlg, IDC_ConfigAll_CHKBOX_ON_TIMEOUT5_BUF_CLEAR, config.bBufClearOnTimeOut5);
	sprintf(buf, "%d", (int)config.fBufClearValueOnTimeOut5);
    SetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_ON_TIMEOUT5_BUF_CLEAR_VALUE), buf);

	sprintf(buf, "%d", config.nRetryCountOnWriteTimeOut);
    SetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_RETRY_COUNT_ON_WRITE_TIMEOUT), buf);

	sprintf(buf, "%d", config.nVipScanTryCount);
    SetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_VIP_SCAN_TRY_COUNT), buf);

	sprintf(buf, "%d", config.nItemTimeoutCount);
    SetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_ITEM_TIMEOUT_COUNT), buf);

	sprintf(buf, "%d", config.OnPortTimeOut_SetValue_TimeOut);
    SetWindowText(GetDlgItem(IDC_EDIT_OnPortTimeOut_SetValue_TimeOut), buf);

	sprintf(buf, "%d", config.OnPortTimeOut_SetValue_Value);
    SetWindowText(GetDlgItem(IDC_EDIT_OnPortTimeOut_SetValue_Value), buf);

	CheckDlgButton(hwndDlg, IDC_CHECK_USE_LAST_VALUE_DO, config.bUseNewValueOnDigitalOut);
	CheckDlgButton(hwndDlg, IDC_CHECK_USE_LAST_VALUE_AO, config.bUseNewValueOnAnalogOut);

	EnableDisable();

	return TRUE;
}

BOOL configDialog :: WmCommand()
{
	char buf[80];
	int  scantime;
	int  skiptime;

	switch(wParamThis) {
		case IDOK:
			GetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_SCANTIME), buf, sizeof(buf));
			scantime = atoi(buf);
			if(scantime < 0 || scantime > 30000)	{
				if(IsLangKorean()) {
					MessageBox(hwndDlg, "지연시간은 0~30000까지입니다.", "설정오류", MB_OK);
				} else {
					MessageBox(hwndDlg, "0 <= Delay time <= 30000.", "Delay time range error", MB_OK);
				}
				return TRUE;
			}
			GetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_TIMEOUT_SKIP_SEC), buf, sizeof(buf));
			skiptime = atoi(buf);
			if(skiptime < 0 || skiptime > 60)	{
				if(IsLangKorean()) {
					MessageBox(hwndDlg, "TIMEOUT 설정 시간은 0~60까지입니다.", "설정오류", MB_OK);
				}else {
					MessageBox(hwndDlg, "0 <= TIMEOUT <= 60", "TimeOut range error", MB_OK);
				}
				return TRUE;
			}	

			config.nScanTime = scantime;
			config.nSkipTimeOnTimeOut = skiptime;
			config.bMessageBoxTimeOut = IsDlgButtonChecked(hwndDlg, IDC_ConfigAll_CHKBOX_TIMEOUT);
			config.bMessageBoxCodeBad = IsDlgButtonChecked(hwndDlg, IDC_ConfigAll_CHKBOX_CODEBAD);
            config.bMessageBoxAllMessage = IsDlgButtonChecked(hwndDlg, IDC_ConfigAll_CHKBOX_ALL_MSG);

			config.bMultiPortMultiTasking = IsDlgButtonChecked(hwndDlg, IDC_ConfigAll_CHKBOX_MULTIPORT_MULTITASKING);

			config.bBufClearOnTimeOut5 = IsDlgButtonChecked(hwndDlg, IDC_ConfigAll_CHKBOX_ON_TIMEOUT5_BUF_CLEAR);

			GetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_ON_TIMEOUT5_BUF_CLEAR_VALUE), buf, sizeof(buf));
			config.fBufClearValueOnTimeOut5 = (float)atoi(buf);

			GetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_RETRY_COUNT_ON_WRITE_TIMEOUT), buf, sizeof(buf));
			config.nRetryCountOnWriteTimeOut = atoi(buf);

			GetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_VIP_SCAN_TRY_COUNT), buf, sizeof(buf));
			config.nVipScanTryCount = atoi(buf);

			if(config.nVipScanTryCount < 0 || config.nVipScanTryCount > 5) {
				config.nVipScanTryCount = 0;
			}

			GetWindowText(GetDlgItem(IDC_ConfigAll_EDIT_ITEM_TIMEOUT_COUNT), buf, sizeof(buf));
			config.nItemTimeoutCount = atoi(buf);

			if(config.nItemTimeoutCount < 0 || config.nItemTimeoutCount > 10) {
				config.nItemTimeoutCount = 5;
			}

			GetWindowText(GetDlgItem(IDC_EDIT_OnPortTimeOut_SetValue_TimeOut), buf, sizeof(buf));
			config.OnPortTimeOut_SetValue_TimeOut = atoi(buf);

			if(config.OnPortTimeOut_SetValue_TimeOut < 0 || config.OnPortTimeOut_SetValue_TimeOut > 10) {
				config.OnPortTimeOut_SetValue_TimeOut = 0;
			}

			GetWindowText(GetDlgItem(IDC_EDIT_OnPortTimeOut_SetValue_Value), buf, sizeof(buf));
			config.OnPortTimeOut_SetValue_Value = atoi(buf);

			config.bUseNewValueOnDigitalOut = IsDlgButtonChecked(hwndDlg, IDC_CHECK_USE_LAST_VALUE_DO);
			config.bUseNewValueOnAnalogOut = IsDlgButtonChecked(hwndDlg, IDC_CHECK_USE_LAST_VALUE_AO);
			
			KillTimer(hwndMainFrame, 1);

			if(SetTimer (hwndMainFrame, 1, config.nScanTime, NULL) == 0) {
				MessageBox(hwndMainFrame, "too many timer used...", "Timer Error", MB_OK);
			};

			void SaveConfig(HWND hwnd);
			SaveConfig(hwndDlg);

			EndDialog(1);
			return TRUE;
		case IDC_ConfigAll_CHKBOX_ALL_MSG:
			EnableDisable();
			return TRUE;
		case IDCANCEL:
			EndDialog(0);
			return TRUE;
		case IDHELP:
			PlcScanHelp(hwndDlg, "PLC_SCANWholeCfg.htm");
			return TRUE;
	}

	return FALSE;
}

void ConfigAll(HWND hwnd)
{
	configDialog dialog;

	dialog.run(hwnd, IDD_CONFIG_ALL, hInst);
}



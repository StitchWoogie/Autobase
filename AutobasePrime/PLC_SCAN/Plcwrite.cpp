// english O.K
#include "stdafx.h"
#include <stdlib.h>

#include <kwl\kdialog.h>
#include <glib.h>
#include "..\catlib.src\totalcfg.h"

#include "plc_scan.h"                     
#include "resource.h"

int AddWaitWriteDigitalOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag);
int AddWaitWriteAnalogOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr,  double value);

static int  bitPort = 0;
static int  bitStation = 0;
static DWORD bitAddress = 0;
static char bitExtra1[40];
static WORD bitExtra2 = 0;

static int  wordPort = 0;
static int  wordStation = 0;
static DWORD wordAddress = 0;
static char wordExtra1[40];
static WORD wordExtra2 = 0;
static long double doubleValue = 0;

class dialogWriteBit : public KDialog
{

	public:
		dialogWriteBit() {}
		~dialogWriteBit() {}
		BOOL WmInitDialog();
		BOOL WmCommand();
};

BOOL dialogWriteBit :: WmInitDialog()
{
	char buf[80];

	sprintf(buf, "%d", bitPort);
	SetWindowText(GetDlgItem(IDC_WriteBit_EDIT_PORT), buf);
	sprintf(buf, "%d", bitStation);
	SetWindowText(GetDlgItem(IDC_WriteBit_EDIT_STATION), buf);
	sprintf(buf, "%04X", bitAddress);
	SetWindowText(GetDlgItem(IDC_WriteBit_EDIT_ADDRESS), buf);
	sprintf(buf, "%s", bitExtra1);
	SetWindowText(GetDlgItem(IDC_WriteBit_EDIT_EXTRA1), buf);
	sprintf(buf, "%d", bitExtra2);
	SetWindowText(GetDlgItem(IDC_WriteBit_EDIT_EXTRA2), buf);

	CheckRadioButton(hwndDlg, IDC_WriteBit_RADIO_OFF, IDC_WriteBit_RADIO_ON, IDC_WriteBit_RADIO_OFF);

	return TRUE;
}

static int CheckRightWrite(HWND hwnd)
{
	char bEnableWrite;
//	char autobase_ini[MAXPATH];

	bEnableWrite = AutoBaseIniGetPlcScanWriteTest();

	/*
	AutoBaseIniGetIniPath(autobase_ini, sizeof(autobase_ini));
	bEnableWrite = GetPrivateProfileInt("PlcScan", "bPlcScanWriteTest", 1, autobase_ini);
	*/

	if(bEnableWrite == OFF) {
		if(IsLangKorean()) {
			MessageBox(hwnd, "출력 테스트 권한이 없습니다.", "권한 없음", MB_OK);
		}else {
			MessageBox(hwnd, "You have not right to write.", "User right", MB_OK);
		}
		return 0;
	}

	return 1;
}

bool CheckPortNumber(int port)
{
	if(eOemType == OEM_TYPE_SBAS) {
		if(port < 0 || port > 255) {
			MessageBox(hwndMainFrame, "포트번호는 0~255 까지 사용할 수 있습니다.", "포트 번호 오류", MB_OK);
			return false;
		}
	}

	return true;
}

bool CheckStationNumber(int station)
{
	if(eOemType == OEM_TYPE_SBAS) {
		if(station < 0 || station > 255) {
			MessageBox(hwndMainFrame, "스테이션 번호는 0~255 까지 사용할 수 있습니다.", "스테이션 번호 오류", MB_OK);
			return false;
		}
	}

	return true;
}

bool CheckAddressNumber(int address)
{
	if(eOemType == OEM_TYPE_SBAS) {
		if(address < 0 || address > 100000) {
			MessageBox(hwndMainFrame, "주소는 0~100000 까지 사용할 수 있습니다.", "주소 오류", MB_OK);
			return false;
		}
	}

	return true;
}

bool CheckExtra1(char *buf)
{
	if(eOemType == OEM_TYPE_SBAS) {
		if(strlen(buf) > 10) {
			MessageBox(hwndMainFrame, "Extra1은 10자리까지 사용할 수 있습니다.", "Extra1 오류", MB_OK);
			return false;
		}
	}

	return true;
}

bool CheckExtra2(char *buf)
{
	if(eOemType == OEM_TYPE_SBAS) {
		if(strlen(buf) > 10) {
			MessageBox(hwndMainFrame, "Extra2는 10자리까지 사용할 수 있습니다.", "Extra2 오류", MB_OK);
			return false;
		}
	}

	return true;
}

bool CheckWordValue(double value)
{
	if(eOemType == OEM_TYPE_SBAS) {
		if(value > 100000) {
			MessageBox(hwndMainFrame, "워드쓰기 값은 100000까지 사용할 수 있습니다.", "워드쓰기 값 오류", MB_OK);
			return false;
		}
	}

	return true;
}

BOOL dialogWriteBit :: WmCommand()
{
	char buf[80];
	char flag;

	switch(wParamThis) {
		case IDOK:
			if(!CheckRightWrite(hwndDlg))	return TRUE;

			GetWindowText(GetDlgItem(IDC_WriteBit_EDIT_PORT), buf, sizeof(buf));
			bitPort = atoi(buf);

			if(!CheckPortNumber(bitPort))	return TRUE;

			if(bitPort < 0 || bitPort >= nPortHap) {
				if(IsLangKorean()) {
					MessageBox(hwndDlg, "설치되지 않은 포트에 쓰기를 시도했습니다.", "포트 오류", MB_OK);
				}else {
					MessageBox(hwndDlg, "Port not exist.", "Port number error", MB_OK);
				}
				SetFocus(GetDlgItem(IDC_WriteBit_EDIT_PORT));
				return TRUE;
			}
			GetWindowText(GetDlgItem(IDC_WriteBit_EDIT_STATION), buf, sizeof(buf));
			bitStation = atoi(buf);

			if(!CheckStationNumber(bitStation))	return TRUE;

			GetWindowText(GetDlgItem(IDC_WriteBit_EDIT_ADDRESS), buf, sizeof(buf));
			if(strlen(buf) == 0) {
				bitAddress = 0;
			}
			else {
				CommaBlockString comma;
				comma.Set(buf);
				
				comma.GetHexDWORD(bitAddress);
			}

			if(!CheckAddressNumber(bitAddress))	return TRUE;

			GetWindowText(GetDlgItem(IDC_WriteBit_EDIT_EXTRA1), bitExtra1, sizeof(bitExtra1));

			if(!CheckExtra1(bitExtra1))	return TRUE;

			GetWindowText(GetDlgItem(IDC_WriteBit_EDIT_EXTRA2), buf, sizeof(buf));
			bitExtra2 = atoi(buf);

			if(!CheckExtra2(buf))	return TRUE;

			flag = GetRadioPosition(hwndDlg, IDC_WriteBit_RADIO_OFF, 2);

			AddWaitWriteDigitalOut(bitPort, bitStation, bitAddress, bitExtra1, bitExtra2, flag);

			return TRUE;
		case IDCANCEL:
			EndDialog(0);
	}

	return FALSE;
}

void ExecuteWriteBitDialog(HWND hwnd)
{
	dialogWriteBit dialog;

	dialog.run(hwnd, IDD_WRITE_BIT, hInst);
}


class dialogWriteWord : public KDialog
{

	public:
		dialogWriteWord() {}
		~dialogWriteWord() {}
		BOOL WmInitDialog();
		BOOL WmCommand();
};

BOOL dialogWriteWord :: WmInitDialog()
{
	char buf[80];

	sprintf(buf, "%d", wordPort);
	SetWindowText(GetDlgItem(IDC_WriteWord_EDIT_PORT), buf);
	sprintf(buf, "%d", wordStation);
	SetWindowText(GetDlgItem(IDC_WriteWord_EDIT_STATION), buf);
	sprintf(buf, "%04X", wordAddress);
	SetWindowText(GetDlgItem(IDC_WriteWord_EDIT_ADDRESS), buf);
	sprintf(buf, "%s", wordExtra1);
	SetWindowText(GetDlgItem(IDC_WriteWord_EDIT_EXTRA1), buf);
	sprintf(buf, "%d", wordExtra2);
	SetWindowText(GetDlgItem(IDC_WriteWord_EDIT_EXTRA2), buf);
	sprintf(buf, "%f", doubleValue);
	SetWindowText(GetDlgItem(IDC_WriteWord_EDIT_VALUE), buf);
	return TRUE;
}

BOOL dialogWriteWord :: WmCommand()
{
	char buf[80];

	switch(wParamThis) {
		case IDOK:
			if(!CheckRightWrite(hwndDlg))	return TRUE;

			GetWindowText(GetDlgItem(IDC_WriteWord_EDIT_PORT), buf, sizeof(buf));
			wordPort = atoi(buf);

			if(!CheckPortNumber(wordPort))	return TRUE;

			if(wordPort < 0 || wordPort >= nPortHap) {
				if(IsLangKorean()) {
					MessageBox(hwndDlg, "설치되지 않은 포트에 쓰기를 시도했습니다.", "포트 오류", MB_OK);
				}else {
					MessageBox(hwndDlg, "Port not exist.", "Port number error", MB_OK);
				}
				SetFocus(GetDlgItem(IDC_WriteWord_EDIT_PORT));
				return TRUE;
			}
			GetWindowText(GetDlgItem(IDC_WriteWord_EDIT_STATION), buf, sizeof(buf));
			wordStation = atoi(buf);

			if(!CheckStationNumber(wordStation))	return TRUE;

			GetWindowText(GetDlgItem(IDC_WriteWord_EDIT_ADDRESS), buf, sizeof(buf));
			{
				CommaBlockString comma;
				comma.Set(buf);
					
				comma.GetHexDWORD(wordAddress); 
				//wordAddress = HexBufToWORD(buf);
			}

			if(!CheckAddressNumber(wordAddress))	return TRUE;
			
			GetWindowText(GetDlgItem(IDC_WriteWord_EDIT_EXTRA1), wordExtra1, sizeof(wordExtra1));

			if(!CheckExtra1(wordExtra1))	return TRUE;

			GetWindowText(GetDlgItem(IDC_WriteWord_EDIT_EXTRA2), buf, sizeof(buf));
			wordExtra2 = atoi(buf);

			if(!CheckExtra2(buf))	return TRUE;
                            
			GetWindowText(GetDlgItem(IDC_WriteWord_EDIT_VALUE), buf, sizeof(buf));
			if(IsDlgButtonChecked(hwndDlg, IDC_CHECK_TO_HEX)) {
				CString imsi;
				DWORD dword_value;
				CommaBlockString comma;
				comma.Set(buf);
				comma.GetHexDWORD(dword_value);
				doubleValue = dword_value;
			}
			else {
				doubleValue = atof(buf);
			}

			if(!CheckWordValue(doubleValue))	return TRUE;

			AddWaitWriteAnalogOut(wordPort, wordStation, wordAddress, wordExtra1, wordExtra2, doubleValue);
			return TRUE;
		case IDCANCEL:
			EndDialog(0);
	}

	return FALSE;
}

void ExecuteWriteWordDialog(HWND hwnd)
{
	dialogWriteWord dialog;

	dialog.run(hwnd, IDD_WRITE_WORD, hInst);
}



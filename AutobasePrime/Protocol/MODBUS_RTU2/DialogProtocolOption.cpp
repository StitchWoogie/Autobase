// DialogProtocolOption.cpp : implementation file
//

#include "stdafx.h"
#include "MODBUS_RTU2.h"
#include "DialogProtocolOption.h"
#include "DialogSelectTag.h"

#include "..\..\catlib.src\totalcfg.h"
#include "..\..\catdll\catdll.hpp"
#include ".\dialogprotocoloption.h"

extern char sWorkDir[MAXPATH];


// CDialogProtocolOption dialog

IMPLEMENT_DYNAMIC(CDialogProtocolOption, CDialog)
CDialogProtocolOption::CDialogProtocolOption(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogProtocolOption::IDD, pParent)
	, m_station(0)
	, m_bFloatData(FALSE)
	, m_bReadValueFloat(FALSE)
	, m_bVersion9(FALSE)
	, m_nSendStartPos(0)
	, m_bUseTcp(FALSE)
	, m_bUseDigitalMemory(FALSE)
	, m_nSendStartPosCoil(0)
{
}

CDialogProtocolOption::~CDialogProtocolOption()
{
}

void CDialogProtocolOption::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_READ_SCHEDULE, m_list);
	DDX_Text(pDX, IDC_EDIT_STATION, m_station);
	DDV_MinMaxByte(pDX, m_station, 0, 255);
	DDX_Control(pDX, IDC_SPIN1, m_spinStation);
	DDX_Check(pDX, IDC_CHECK1, m_bFloatData);
	DDX_Check(pDX, IDC_CHECK2, m_bReadValueFloat);
	DDX_Check(pDX, IDC_CHECK_VERSION9, m_bVersion9);
	DDX_Control(pDX, IDC_SPIN_MODBUS_START_ADDR, m_spinSendStartAddr);
	DDX_Text(pDX, IDC_EDIT_MODBUS_START_ADDR, m_nSendStartPos);
	DDV_MinMaxUInt(pDX, m_nSendStartPos, 0, 65535);
	DDX_Check(pDX, IDC_CHECK_MODBUS_IP_PROTOCOL, m_bUseTcp);
	DDX_Check(pDX, IDC_CHECK_USE_COIL_INPUT_MEMORY, m_bUseDigitalMemory);
	DDX_Text(pDX, IDC_EDIT_MODBUS_START_ADDR_COIL, m_nSendStartPosCoil);
	DDV_MinMaxUInt(pDX, m_nSendStartPosCoil, 0, 65535);
	DDX_Control(pDX, IDC_SPIN_MODBUS_START_ADDR_COIL, m_spinSendStartPosCoil);
}


BEGIN_MESSAGE_MAP(CDialogProtocolOption, CDialog)
	ON_BN_CLICKED(IDC_BUTTON_INSERT, OnBnClickedButtonInsert)
	ON_BN_CLICKED(IDC_BUTTON_ADD, OnBnClickedButtonAddData)
	ON_BN_CLICKED(IDC_BUTTON_DELETE, OnBnClickedButtonDelete)
	ON_BN_CLICKED(IDC_CHECK_USE_COIL_INPUT_MEMORY, OnBnClickedCheckUseCoilInputMemory)
	ON_BN_CLICKED(IDC_RADIO_ANALOG, OnBnClickedRadioAnalog)
	ON_BN_CLICKED(IDC_RADIO_DIGITAL, OnBnClickedRadioDigital)
END_MESSAGE_MAP()


// CDialogProtocolOption message handlers


void CDialogProtocolOption::GetRegisteredTag()
{
	FILE				*in;
	CommaBlockString	comma;
	int					pos;
	char				buf[200], imsi[81], imsi2[50], filename[MAXPATH];
	
	if(m_memoryType == 1) // add 2015-03-20 for Coil/Input status 0 = Analog, 1 = digital
		sprintf(filename, "%s\\SCAN\\READTAG_COIL%03d.txt", sWorkDir, m_port);
	else
		sprintf(filename, "%s\\SCAN\\READTAG%03d.txt", sWorkDir, m_port);
	in = fopen(filename, "rb");
	if(in == NULL) return;

	pos = 0;

	while(TextGetOneLine(in, buf, sizeof(buf))) {
		comma.Set(buf);
		comma.GetString(imsi, sizeof(imsi));
		comma.GetString(imsi, sizeof(imsi));
		if(strlen(imsi) <= 0) continue;		// 태그명이 없으면 ....
		sprintf(imsi2, "%03d", pos);
		m_list.InsertItem(pos, imsi2);
		m_list.SetItemText(pos, 1, imsi);		
		
		comma.GetString(imsi, sizeof(imsi));
		m_list.SetItemText(pos, 2, imsi);		

		pos++;
		if(pos > MAX_SEND_TAG_COUNT) break;	// 1000 -> MAX_SEND_TAG_COUNT 로 수정, 2005-09-13
	}
	fclose(in);	
}

void CDialogProtocolOption::saveRegisteredTag()
{
	if(m_bChangeData == false) return;// add 2015-03-20

	FILE				*out;
	CommaBlockString	comma;
	int					i, hap;
	char				tag[80], desc[80], imsi[80], filename[MAXPATH];
	
	if(m_memoryType == 1) // add 2015-03-20 for Coil/Input status 0 = Analog, 1 = digital
		sprintf(filename, "%s\\SCAN\\READTAG_COIL%03d.txt", sWorkDir, m_port);
	else
		sprintf(filename, "%s\\SCAN\\READTAG%03d.txt", sWorkDir, m_port);
	out = fopen(filename, "wb");
	if(out == NULL) return;

	hap = m_list.GetItemCount();

	for(i = 0; i < hap; i++) {
		m_list.GetItemText(i, 0, imsi, sizeof(imsi));
		m_list.GetItemText(i, 1, tag, sizeof(tag));
		m_list.GetItemText(i, 2, desc, sizeof(desc));
		fprintf(out, "%s,%s,%s,\r\n", imsi, tag, desc);
	}	
	fclose(out);
	m_bChangeData = false;	// add 2015-03-20
}

int CDialogProtocolOption::GetSelectPosition()
{
	int		i;

	for(i = 0; i < m_list.GetItemCount(); i++) {
		if(m_list.GetItemState(i, LVIS_SELECTED)) {
			return i;
		}
	}

	return -1;
}


BOOL CDialogProtocolOption::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  Add extra initialization here
	AutoBaseIniGetProjectDirectory(sWorkDir);			// Modus ASCII RTU 모드로 변경하면 안되기 때문에 다시 읽는다
	m_spinStation.SetRange(1, 16);
	m_spinSendStartAddr.SetRange32(0, 65535);
	m_spinSendStartPosCoil.SetRange32(0, 65535);
	m_currPos = 0;		// add 2007-08-30
	m_tagType = 0;		// AI, add 2007-08-30
	m_memoryType = 0;	// add 2015-03-20 for Coil/Input status 0 = Analog, 1 = digital
	m_bChangeData = false;// add 2015-03-20

	m_list.InsertColumn(0, "No", LVCFMT_CENTER, 60);
	m_list.InsertColumn(1, "Tag", LVCFMT_CENTER, 250);
	m_list.InsertColumn(2, "Description", LVCFMT_CENTER, 300);
	CheckRadioButton(IDC_RADIO_ANALOG, IDC_RADIO_DIGITAL, IDC_RADIO_ANALOG+m_memoryType);

	GetRegisteredTag();
	UpdateData(false);
	OnBnClickedCheckUseCoilInputMemory();
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogProtocolOption::OnOK()
{
	// TODO: Add your specialized code here and/or call the base class

	if(!UpdateData()) return;

	saveRegisteredTag();
	CDialog::OnOK();
}

void CDialogProtocolOption::OnBnClickedButtonAddData()
{
	// TODO: Add your control notification handler code here
	if(tagCountIsMaxCount()) return;			// 등록된 태그가 최대 버퍼수를 넘었다. 10000개, 2005-09-13 추가

	//char		tag[80], desc[80];
	CString		buf;
	int			pos;

	pos = m_list.GetItemCount();

	UpdateData(true);				// add 2007-08-30
	if(m_bVersion9) {				// add 2007-08-30
		CDialogSelectTag	dialog;

		dialog.m_currPos = m_currPos;
		dialog.m_tagType = m_tagType;
		if(dialog.DoModal() != IDOK) return;
		m_currPos = dialog.m_currPos;
		m_tagType = dialog.m_tagType;

		buf.Format("%03d", pos);
		m_list.InsertItem(pos, buf);
		m_list.SetItemText(pos, 1, dialog.mTagName);
		m_list.SetItemText(pos, 2, dialog.mTagDesc);
		checkAndSetTagPos();
		m_bChangeData = true;// add 2015-03-20
		return;
	}

	MessageBox("Check Tag Version is 9.xx or Later.", "Check Error");// add 2010-02-03
	return;
	/*if(DllTagSelectTagAiDi(tag, desc)) {// deleted 2010-02-03
		buf.Format("%03d", pos);
		m_list.InsertItem(pos, buf);
		m_list.SetItemText(pos, 1, tag);
		m_list.SetItemText(pos, 2, desc);		
		checkAndSetTagPos();
	}*/
}

void CDialogProtocolOption::checkAndSetTagPos()
{
	int		i, hap = m_list.GetItemCount();
	char	imsi[50];
	CString	buf;

	for(i = 0; i < hap; i++) {
		m_list.GetItemText(i, 0, imsi, sizeof(imsi));
		if(atoi(imsi) != i) {
			buf.Format("%03d", i);
			m_list.SetItemText(i, 0, buf);
		}
	}
}

bool CDialogProtocolOption::tagCountIsMaxCount()// 등록된 태그확인, 2005-09-13 추가
{
	if(m_list.GetItemCount() >= MAX_SEND_TAG_COUNT) {
		CString	buf;

		buf.Format("Regersted Tag is >= %d.", m_list.GetItemCount());
		MessageBox(buf, "Tag Count Overflow");
		return true;
	}
	return false;
}

void CDialogProtocolOption::OnBnClickedButtonInsert()
{
	// TODO: Add your control notification handler code here
	if(tagCountIsMaxCount()) return;			// 등록된 태그가 최대 버퍼수를 넘었다. 10000개, 2005-09-13 추가

	//char		tag[80], desc[80];
	CString		buf;
	int			pos = GetSelectPosition();

	if(pos == -1) {
		MessageBox("Select One Tag Item.", "Select Error");
		return;
	}

	UpdateData(true);				// add 2007-08-30
	if(m_bVersion9) {				// add 2007-08-30
		CDialogSelectTag	dialog;

		dialog.m_currPos = m_currPos;
		dialog.m_tagType = m_tagType;
		if(dialog.DoModal() != IDOK) return;
		m_currPos = dialog.m_currPos;
		m_tagType = dialog.m_tagType;

		buf.Format("%03d", pos);
		m_list.InsertItem(pos, buf);
		m_list.SetItemText(pos, 1, dialog.mTagName);
		m_list.SetItemText(pos, 2, dialog.mTagDesc);
		m_bChangeData = true;// add 2015-03-20
		checkAndSetTagPos();
		return;
	}

	MessageBox("Check Tag Version is 9.xx or Later.", "Check Error");// add 2010-02-03
	return;
	/*if(DllTagSelectTagAiDi(tag, desc)) {// deleted 2010-02-03
		buf.Format("%03d", pos);
		m_list.InsertItem(pos, buf);
		m_list.SetItemText(pos, 1, tag);
		m_list.SetItemText(pos, 2, desc);

		checkAndSetTagPos();
	}*/	
}


void CDialogProtocolOption::OnBnClickedButtonDelete()
{
	// TODO: Add your control notification handler code here

	int			pos = GetSelectPosition();

	if(pos == -1) {
		MessageBox("Select One Tag Item.", "Select Error");
		return;
	}
	m_list.DeleteItem(pos);

	m_bChangeData = true;// add 2015-03-20
	if(m_list.GetItemCount() <= pos) pos -= 1;// add 2015-03-20
	if(pos < 0) return;// add 2015-03-20
	m_list.SetItemState(pos, LVIS_SELECTED, LVIS_SELECTED);// add 2015-03-20
	m_list.EnsureVisible(pos, false);// add 2015-03-20

	checkAndSetTagPos();
}

void CDialogProtocolOption::OnBnClickedCheckUseCoilInputMemory()
{
	// TODO: Add your control notification handler code here

	UpdateData(true);
	bool		flag = (m_bUseDigitalMemory == TRUE) ? true : false;
	
	GetDlgItem(IDC_EDIT_MODBUS_START_ADDR_COIL)->EnableWindow(flag);
	GetDlgItem(IDC_SPIN_MODBUS_START_ADDR_COIL)->EnableWindow(flag);
	//GetDlgItem(IDC_RADIO_DIGITAL)->EnableWindow(flag);
}

void CDialogProtocolOption::OnBnClickedRadioAnalog()// add 2015-03-20
{
	// TODO: Add your control notification handler code here
	if(m_memoryType == 0) return;

	saveRegisteredTag();
	m_memoryType = 0;
	m_list.DeleteAllItems();
	GetRegisteredTag();
}

void CDialogProtocolOption::OnBnClickedRadioDigital()// add 2015-03-20
{
	// TODO: Add your control notification handler code here
	if(m_memoryType == 1) return;

	saveRegisteredTag();
	m_memoryType = 1;
	m_list.DeleteAllItems();
	GetRegisteredTag();
}

// DialogSelectTag.cpp : implementation file
//

#include "stdafx.h"
#include "MODBUS_RTU2.h"
#include "DialogSelectTag.h"
#include "..\..\catlib.src\CatTag9.h"
#include "..\..\catlib.src\CatTag.h"


// CDialogSelectTag dialog

IMPLEMENT_DYNAMIC(CDialogSelectTag, CDialog)
CDialogSelectTag::CDialogSelectTag(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogSelectTag::IDD, pParent)
{
}

CDialogSelectTag::~CDialogSelectTag()
{
}

void CDialogSelectTag::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_SELECT_TAG, m_list);
}


BEGIN_MESSAGE_MAP(CDialogSelectTag, CDialog)
	ON_BN_CLICKED(IDC_RADIO_ANALOG_INPUT, OnBnClickedRadioAnalogInput)
	ON_BN_CLICKED(IDC_RADIO_DIGITAL_INPUT, OnBnClickedRadioDigitalInput)
	ON_NOTIFY(NM_DBLCLK, IDC_LIST_SELECT_TAG, OnNMDblclkListSelectTag)
END_MESSAGE_MAP()


void CDialogSelectTag::checkCurrPos()
{
	if(m_currPos < 0 || m_currPos >= m_list.GetItemCount()) m_currPos = 0;
}


bool CDialogSelectTag::isTagTypeMached(int nTagType)
{
	if(m_tagType == 0 && nTagType == TAG_TYPE_AI) return true;
	if(m_tagType == 2 && nTagType == TAG_TYPE_DI) return true;
	return false;
}

void CDialogSelectTag::GetCurrentTag()
{
	TagPublicStruct		*tp;
	char				buf[81];
	DWORD				i, pos = 0;
	
	m_list.DeleteAllItems();
	for(i = 0; i < blockTagList9.GetCount(); i++) {
		tp = (TagPublicStruct*)blockTagList9.GetPtr(i);
		if(isTagTypeMached(tp->nTagType)) {
		//if(tp->nTagType == TAG_TYPE_AI) {
			sprintf(buf, "%05d", pos);
			m_list.InsertItem(pos, buf);
			m_list.SetItemText(pos, 1, tp->tag);
			m_list.SetItemText(pos, 2, tp->description);
			pos++;			
		}
	}
}

int CDialogSelectTag::GetSelectPosition()
{
	int		i;

	for(i = 0; i < m_list.GetItemCount(); i++) {
		if(m_list.GetItemState(i, LVIS_SELECTED)) {
			return i;
		}
	}
	return -1;
}

// CDialogSelectTag message handlers

BOOL CDialogSelectTag::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  Add extra initialization here
	m_list.InsertColumn(0, "No", LVCFMT_CENTER, 60);
	m_list.InsertColumn(1, "Tag Name", LVCFMT_LEFT, 200);
	m_list.InsertColumn(2, "Description", LVCFMT_LEFT, 200);
	if(m_tagType != 2) m_tagType = 0;
	CheckRadioButton(IDC_RADIO_ANALOG_INPUT, IDC_RADIO_STRING, IDC_RADIO_ANALOG_INPUT+m_tagType);
	GetCurrentTag();
	
	checkCurrPos();
	m_list.SetItemState(m_currPos, LVIS_SELECTED, LVIS_SELECTED);
	m_list.EnsureVisible(m_currPos, false);
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogSelectTag::OnOK()
{
	// TODO: Add your specialized code here and/or call the base class
	UpdateData(true);

	int			pos = GetSelectPosition();

	m_currPos = pos;
	m_tagType = (IsDlgButtonChecked(IDC_RADIO_DIGITAL_INPUT)) ? 2 : 0;
	if(blockTagList9.GetCount() <= 0 || pos == -1) {
		MessageBox("Tag Selection Error.", "Selection Error");
		return;
	}
	m_list.GetItemText(pos, 1, mTagName, sizeof(mTagName));
	if((int)strlen(mTagName) <= 0) {
		MessageBox("Tag Name is Empty or Null.", "Tag Error");
		return;
	}
	m_list.GetItemText(pos, 2, mTagDesc, sizeof(mTagDesc));
	CDialog::OnOK();
}

void CDialogSelectTag::OnBnClickedRadioAnalogInput()
{
	// TODO: Add your control notification handler code here
	if(m_tagType == 0) return;

	m_tagType = 0;
	GetCurrentTag();
	checkCurrPos();
	m_list.SetItemState(m_currPos, LVIS_SELECTED, LVIS_SELECTED);
	m_list.EnsureVisible(m_currPos, false);
}

void CDialogSelectTag::OnBnClickedRadioDigitalInput()
{
	// TODO: Add your control notification handler code here
	if(m_tagType == 2) return;

	m_tagType = 2;
	GetCurrentTag();
	checkCurrPos();
	m_list.SetItemState(m_currPos, LVIS_SELECTED, LVIS_SELECTED);
	m_list.EnsureVisible(m_currPos, false);
}

void CDialogSelectTag::OnNMDblclkListSelectTag(NMHDR *pNMHDR, LRESULT *pResult)
{
	// TODO: Add your control notification handler code here
	OnOK();
	*pResult = 0;
}

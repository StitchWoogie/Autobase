// TAG size O.K
// DialogSelectProject.cpp : implementation file
//

#include "stdafx.h"

#include <glib.h>

static LPCTSTR MakeShortString(CDC* pDC, LPCTSTR lpszLong, int nColumnLen, int nOffset)
{
	static const _TCHAR szThreeDots[] = _T("...");

	int nStringLen = lstrlen(lpszLong);

	if(nStringLen == 0 ||
		(pDC->GetTextExtent(lpszLong, nStringLen).cx + nOffset) <= nColumnLen)
	{
		return(lpszLong);
	}

	static _TCHAR szShort[MAX_PATH];

	lstrcpy(szShort,lpszLong);
	int nAddLen = pDC->GetTextExtent(szThreeDots,sizeof(szThreeDots)).cx;

	for(int i = nStringLen-1; i > 0; i--)
	{
		szShort[i] = 0;
		if((pDC->GetTextExtent(szShort, i).cx + nOffset + nAddLen)
			<= nColumnLen)
		{
			break;
		}
	}

	lstrcat(szShort, szThreeDots);
	return(szShort);
}

void DrawUserListCtrl(LPDRAWITEMSTRUCT lpDrawItemStruct, CListCtrl &m_list)
{
#define OFFSET_FIRST	2
#define OFFSET_OTHER    6

#define	m_clrBkgnd	(::GetSysColor(COLOR_WINDOW))
#define m_clrTextBk (::GetSysColor(COLOR_WINDOW))

#define m_cxStateImageOffset	0

	CDC* pDC = CDC::FromHandle(lpDrawItemStruct->hDC);
	CRect rcItem(lpDrawItemStruct->rcItem);
	UINT uiFlags = ILD_TRANSPARENT;
	CImageList* pImageList;
	int nItem = lpDrawItemStruct->itemID;
	BOOL bFocus = (::GetFocus() == m_list.m_hWnd);
	COLORREF clrTextSave, clrBkSave;
	COLORREF clrImage = m_clrBkgnd;
	static _TCHAR szBuff[MAX_PATH];
	LPCTSTR pszText;

// get item data

	LV_ITEM lvi;
	lvi.mask = LVIF_TEXT | LVIF_IMAGE | LVIF_STATE;
	lvi.iItem = nItem;
	lvi.iSubItem = 0;
	lvi.pszText = szBuff;
	lvi.cchTextMax = sizeof(szBuff);
	lvi.stateMask = 0xFFFF;     // get all state flags
	m_list.GetItem(&lvi);

	BOOL bSelected = (bFocus || (m_list.GetStyle() & LVS_SHOWSELALWAYS)) && lvi.state & LVIS_SELECTED;
	bSelected = bSelected || (lvi.state & LVIS_DROPHILITED);

// set colors if item is selected

	CRect rcAllLabels;
	m_list.GetItemRect(nItem, rcAllLabels, LVIR_BOUNDS);

	CRect rcLabel;
	m_list.GetItemRect(nItem, rcLabel, LVIR_LABEL);

	rcAllLabels.left = rcLabel.left;

	//if (m_bClientWidthSel && rcAllLabels.right<m_cxClient)
	//	rcAllLabels.right = m_cxClient;

	if (bSelected)
	{
		clrTextSave = pDC->SetTextColor(::GetSysColor(COLOR_HIGHLIGHTTEXT));
		clrBkSave = pDC->SetBkColor(::GetSysColor(COLOR_HIGHLIGHT));

		pDC->FillRect(rcAllLabels, &CBrush(::GetSysColor(COLOR_HIGHLIGHT)));
	}
	else
		pDC->FillRect(rcAllLabels, &CBrush(m_clrTextBk));

// set color and mask for the icon

	if (lvi.state & LVIS_CUT)
	{
		clrImage = m_clrBkgnd;
		uiFlags |= ILD_BLEND50;
	}
	else if (bSelected)
	{
		clrImage = ::GetSysColor(COLOR_HIGHLIGHT);
		uiFlags |= ILD_BLEND50;
	}

// draw state icon

	UINT nStateImageMask = lvi.state & LVIS_STATEIMAGEMASK;
	if (nStateImageMask)
	{
		int nImage = (nStateImageMask>>12) - 1;
		pImageList = m_list.GetImageList(LVSIL_STATE);
		if (pImageList)
		{
			pImageList->Draw(pDC, nImage,
				CPoint(rcItem.left, rcItem.top), ILD_TRANSPARENT);
		}
	}

// draw normal and overlay icon

	CRect rcIcon;
	m_list.GetItemRect(nItem, rcIcon, LVIR_ICON);

	pImageList = m_list.GetImageList(LVSIL_SMALL);
	if (pImageList)
	{
		UINT nOvlImageMask=lvi.state & LVIS_OVERLAYMASK;
		if (rcItem.left<rcItem.right-1)
		{
			ImageList_DrawEx(pImageList->m_hImageList, lvi.iImage,
					pDC->m_hDC,rcIcon.left,rcIcon.top, 16, 16,
					m_clrBkgnd, clrImage, uiFlags | nOvlImageMask);
		}
	}

// draw item label

	m_list.GetItemRect(nItem, rcItem, LVIR_LABEL);
	rcItem.right -= m_cxStateImageOffset;

	pszText = MakeShortString(pDC, szBuff,
				rcItem.right-rcItem.left, 2*OFFSET_FIRST);

	rcLabel = rcItem;
	rcLabel.left += OFFSET_FIRST;
	rcLabel.right -= OFFSET_FIRST;

	pDC->DrawText(pszText,-1,rcLabel,DT_LEFT | DT_SINGLELINE | DT_NOPREFIX | DT_NOCLIP | DT_VCENTER);

// draw labels for extra columns

	LV_COLUMN lvc;
	lvc.mask = LVCF_FMT | LVCF_WIDTH;

	for(int nColumn = 1; m_list.GetColumn(nColumn, &lvc); nColumn++)
	{
		rcItem.left = rcItem.right;
		rcItem.right += lvc.cx;

		int nRetLen = m_list.GetItemText(nItem, nColumn,
						szBuff, sizeof(szBuff));
		if (nRetLen == 0)
			continue;

		pszText = MakeShortString(pDC, szBuff,
			rcItem.right - rcItem.left, 2*OFFSET_OTHER);

		UINT nJustify = DT_LEFT;

		if(pszText == szBuff)
		{
			switch(lvc.fmt & LVCFMT_JUSTIFYMASK)
			{
			case LVCFMT_RIGHT:
				nJustify = DT_RIGHT;
				break;
			case LVCFMT_CENTER:
				nJustify = DT_CENTER;
				break;
			default:
				break;
			}
		}

		rcLabel = rcItem;
		rcLabel.left += OFFSET_OTHER;
		rcLabel.right -= OFFSET_OTHER;

		pDC->DrawText(pszText, -1, rcLabel,
			nJustify | DT_SINGLELINE | DT_NOPREFIX | DT_NOCLIP | DT_VCENTER);
	}

// draw focus rectangle if item has focus

	if (lvi.state & LVIS_FOCUSED && bFocus)
		pDC->DrawFocusRect(rcAllLabels);

// set original colors if item was selected

	if (bSelected)
	{
		pDC->SetTextColor(clrTextSave);
		pDC->SetBkColor(clrBkSave);
	}
}



using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools.OldDefine;
using NetTools;
using AutoLibLocal;
using System.IO;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for WndUserList.
	/// </summary>
	public class WndUserList : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		class WORK_BLOCK_STRUCT
		{
			public ArrayList	block = new ArrayList();
			public WNDPROC		drawProc = null;
			public int			cxChar=10;
			public int			cyChar=10;
			public int			nScrollVerPos;
			public int			nScrollVerHap;
			//public bool			bScrollVer;
			public int			ylimit;					// 한 화면에 보일수 있는 라인수
			public Color		textColor;
			public Color		backColor;
		}

		protected class COLUMN_STRUCT 
		{
			public int size;
			public int default_size;
			public string title;
			public int align;
		}

		protected class DRAW_STRUCT 
		{
			public int	  x;
			public int    y;
			public int	  line;
			public int	  cxChar;
			public int	  cyChar;
			public RECT   rClient = new RECT();	// 윈도우의 전체영역
			public object block;
			public Graphics g;
			public ArrayList blockColumn;
			public WndUserList  wnd;
			public int   nCursor;
		}

		bool	bCursorUsed;
		WNDPROC	ProcDrawTitle;
		protected bool	bUseColumnHeader;

		protected delegate void WNDPROC(Form form, DRAW_STRUCT obj);

		public WndUserList()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			blockColumn = new ArrayList();
			bConfigChange = false;
			bCursorUsed = false;
			//MakeDefaultLogFont(&lfUserList);
			ProcDrawTitle = null;
			bUseColumnHeader = true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WndUserList));
            this.panelMain = new System.Windows.Forms.Panel();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.AccessibleDescription = null;
            this.panelMain.AccessibleName = null;
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.BackgroundImage = null;
            this.panelMain.Font = null;
            this.panelMain.Name = "panelMain";
            this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDown);
            this.panelMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseMove);
            this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
            this.panelMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseUp);
            // 
            // vScrollBar
            // 
            this.vScrollBar.AccessibleDescription = null;
            this.vScrollBar.AccessibleName = null;
            resources.ApplyResources(this.vScrollBar, "vScrollBar");
            this.vScrollBar.BackgroundImage = null;
            this.vScrollBar.Font = null;
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            // 
            // hScrollBar
            // 
            this.hScrollBar.AccessibleDescription = null;
            this.hScrollBar.AccessibleName = null;
            resources.ApplyResources(this.hScrollBar, "hScrollBar");
            this.hScrollBar.BackgroundImage = null;
            this.hScrollBar.Font = null;
            this.hScrollBar.LargeChange = 20;
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.SmallChange = 10;
            this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
            // 
            // WndUserList
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.panelMain);
            this.Icon = null;
            this.Name = "WndUserList";
            this.SizeChanged += new System.EventHandler(this.WndUserList_SizeChanged);
            this.Load += new System.EventHandler(this.WndUserList_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.VScrollBar vScrollBar;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private System.Windows.Forms.Panel panelMain;
		WORK_BLOCK_STRUCT workStruct = new WORK_BLOCK_STRUCT();

		protected void DeleteAll()
		{
			WORK_BLOCK_STRUCT work = workStruct;
	
			work.block.Clear();
			ScrollUpdate(work);
			Invalidate();
		}

		protected void AddListBlock(object item)
		{
			WORK_BLOCK_STRUCT work = workStruct;

			if(work.block.Count < 32000) 
			{
				work.block.Add(item); 
				
				if(work.nScrollVerPos+work.ylimit+1 >= (int)work.block.Count) 
				{
					ScrollUpdate(work);
					Invalidate();
				}
				else if(work.nScrollVerPos+work.ylimit == (int)work.block.Count) 
				{
					work.nScrollVerPos++;
					ScrollUpdate(work);
					Invalidate();
				}
				else 
				{
					ScrollUpdate(work);
				}
				
			}
		}

		ArrayList blockColumn = new ArrayList();
		int		nScrollHorPos;
		int		nScrollHorHap;
		//bool	bScrollHor;
		bool	bConfigChange;
		int nStartY = 0;

		void ScrollUpdate(WORK_BLOCK_STRUCT work)
		{
			RECT rect = new RECT();
			rect.Set(this.ClientRectangle);

			int hap_x = 0;
			int i;
			COLUMN_STRUCT col;

			for(i = 0; i < (int)blockColumn.Count; i++) 
			{
				col = (COLUMN_STRUCT)blockColumn[i];
				hap_x += col.size;
			}

			if(hap_x > rect.right) 
			{
				nScrollHorHap = hap_x-rect.right;
				if(hScrollBar.Visible == false) 
				{
					hScrollBar.Visible = true;
				}

				hScrollBar.Minimum = 0;
				hScrollBar.Maximum = nScrollHorHap+9;
				hScrollBar.Value = nScrollHorPos;
			}
			else 
			{
				if(hScrollBar.Visible == true) 
				{
					hScrollBar.Visible = false;
				}
				nScrollHorPos = 0;
				nScrollHorHap = 0;
			}

			if(hScrollBar.Visible)
				rect.bottom-=hScrollBar.Height;

			work.ylimit = (rect.bottom-nStartY)/work.cyChar;
			if(work.ylimit <= 0)	work.ylimit = 0;

			work.nScrollVerHap = (int)work.block.Count-work.ylimit;
			if(work.nScrollVerHap < 0)	work.nScrollVerHap = 0;

			if(work.nScrollVerPos+work.ylimit >= (int)work.block.Count) 
			{
				work.nScrollVerPos = (int)work.block.Count-work.ylimit;
				if(work.nScrollVerPos < 0)	work.nScrollVerPos = 0;
			}

			if((int)work.block.Count > work.ylimit) 
			{
				if(vScrollBar.Visible == false) 
				{
					vScrollBar.Visible = true;
					
				}
				vScrollBar.Minimum = 0;
				vScrollBar.Maximum = work.nScrollVerHap+9;
				vScrollBar.Value = work.nScrollVerPos;
			}
			else 
			{
				if(vScrollBar.Visible == true) 
				{
					vScrollBar.Visible = false;
				}
				work.nScrollVerPos = 0;
				work.nScrollVerHap = 0;
			}
		}

		int		nCursorY = 0;

		private void panelMain_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
	
			// TODO: Add your message handler code here
			RECT rect = new RECT();
			WORK_BLOCK_STRUCT work = workStruct;
			int i, pos, x;
			DRAW_STRUCT blb = new DRAW_STRUCT();
			COLUMN_STRUCT col;
			RECT r = new RECT();
			//CFont font;
			//CFont *old_font;
			int y_pos = 0;

			rect.Set(this.panelMain.ClientRectangle);

			object stack;

			//font.CreateFontIndirect(&lfUserList);

			//old_font = (CFont*)dc.SelectObject(&font);

			if(ProcDrawTitle != null) 
			{
				blb = new DRAW_STRUCT();
				blb.rClient.Set(this.panelMain.ClientRectangle);
				blb.x = 0;
				blb.y = 0;
				blb.cxChar = work.cxChar;
				blb.cyChar = work.cyChar;
				blb.block = null;
				blb.g = g;
				blb.wnd = this;
				ProcDrawTitle(this.ParentForm, blb);
				y_pos = work.cyChar;
			}

			if(bUseColumnHeader) 
			{
				//dc.SetTextColor(DARK_COLOR);
				//dc.SetBkColor(WHITE_GRAY_COLOR);

				for(i = 0, x = -nScrollHorPos; i < (int)blockColumn.Count && x < rect.right; i++) 
				{
					col = (COLUMN_STRUCT)blockColumn[i];
					DrawClass.PopBox2(g, x, y_pos, x+col.size-1, nStartY-1, Color.LightGray);
					r.left = x+3;
					r.top = y_pos+1;
					r.right = x+col.size-3;
					r.bottom = nStartY-1;
					DrawClass.DrawText(g, col.title, this.Font, Brushes.Black, r, DrawClass.StringFormatLeft);
					x += col.size;
				}
				if(x < rect.right) 
				{
					DrawClass.PopBox2(g, x, y_pos, rect.right-1, nStartY-1, Color.LightGray);
				}
			}

			DrawClass.gcls(g, 0, nStartY, rect.right-1, rect.bottom-1, work.backColor);
	
			//dc.SetTextColor(work.textColor);
			//dc.SetBkColor(work.backColor);
			//dc.SetBkMode(TRANSPARENT);

			blb.rClient.Set(this.panelMain.ClientRectangle);
			blb.cxChar = work.cxChar;
			blb.cyChar = work.cyChar;
			blb.g = g;
			blb.wnd = this;
			blb.nCursor = nCursorY;
			blb.blockColumn = blockColumn;

			for(i = 0, pos = work.nScrollVerPos; i <= work.ylimit && pos < (int)work.block.Count; pos++, i++) 
			{
				stack = work.block[pos];
	
				blb.x = -nScrollHorPos;
				blb.y = i*work.cyChar+nStartY;
				blb.block = stack;
				blb.line = pos;
						
				work.drawProc(this.ParentForm, blb);
			}

			DrawNot(g, nOldMX);
			//dc.SelectObject(old_font);		
		}

		protected void GetCharSize(out int cx, out int cy)
		{
			cx = workStruct.cxChar;
			cy = workStruct.cyChar;
		}

		protected void InsertColumn(string text, int size)
		{
			COLUMN_STRUCT col = new COLUMN_STRUCT();
	
			col.title = text;
			col.size = size;
			col.default_size = size;

			string filename = Path.GetFileName(Application.ExecutablePath);

			string section;
	
			section = this.Text;

			col.size = UserListConfigLoad(filename, section, blockColumn.Count, col.default_size);
	
			blockColumn.Add(col);
		}

		protected void SetDrawProc(WNDPROC proc)
		{
			workStruct.drawProc = proc;
		}

		protected void SetTextColor(Color color)
		{
			workStruct.textColor = color;
		}

		protected void SetBackColor(Color color)
		{
			workStruct.backColor = color;
		}

		protected void SetCursorUsed(bool flag) 
		{
			bCursorUsed = flag; 
		}

		int UserListConfigLoad(string filename, string section, int item_no, int default_value)
		{
			string win_dir;
			string filepath;
			int value;
			string item;
	
			win_dir = TotalConfig.AutoBaseIniGetConfigDirectory();
			filepath = String.Format("{0}\\HansolTechConfig\\UserList\\{1}.ini", win_dir, filename);
	
			item = String.Format("Item{0:00}", item_no);
			value = Profile.GetPrivateProfileIntW(section, item, default_value, filepath);
			if(value != 0) 
			{
				if(value > 1000)	value = 50;
				if(value < 10)		value = 10;
			}

			return value;
		}

		void UserListConfigSave()
		{
			if(bConfigChange == false)	return;

			string filename = Path.GetFileName(Application.ExecutablePath);
			string win_dir;
			string filepath;
			string section;
	
			section = this.Text;
	
			win_dir = TotalConfig.AutoBaseIniGetConfigDirectory();
			filepath = String.Format("{0}\\HansolTechConfig\\UserList", win_dir);
			Directory.CreateDirectory(filepath);
			filepath = String.Format("{0}\\HansolTechConfig\\UserList\\{1}.ini", win_dir, filename);
	
			int i;
			string item;
			COLUMN_STRUCT col;

			for(i = 0; i < (int)blockColumn.Count; i++) 
			{
				col = (COLUMN_STRUCT)blockColumn[i];
				item = String.Format("Item{0:00}", i);
				Profile.WritePrivateProfileIntW(section, item, col.size, filepath);
			}
		}

		private void WndUserList_Load(object sender, System.EventArgs e)
		{
			WORK_BLOCK_STRUCT work = workStruct;

			FillFontSize(work);
			work.block = new ArrayList();
			work.drawProc = null;
			work.textColor = Color.Black;
			work.backColor = Color.White;
			work.nScrollVerPos = 0;
			work.nScrollVerHap = 0;

			nScrollHorPos = 0;
			nScrollHorHap = 0;
			nCursorY = 0;
		}

		void FillFontSize(WORK_BLOCK_STRUCT work)
		{
			//TEXTMETRIC tm;
			Graphics g = CreateGraphics();
			//CFont font;
			//CFont *old_font;

			//font.CreateFontIndirect(&lfUserList);

			//old_font = dc.SelectObject(&font);

			//dc.GetTextMetrics(&tm);
			work.cxChar = this.Font.Height/2;
			work.cyChar = this.Font.Height;

			//dc.SelectObject(old_font);

			nStartY = 0;
			if(ProcDrawTitle != null) 
			{
				nStartY += work.cyChar;
			}
			if(bUseColumnHeader) 
			{
				nStartY += work.cyChar+4;
			}
		}

		private void WndUserList_SizeChanged(object sender, System.EventArgs e)
		{
			ScrollUpdate(workStruct);
			if(this.vScrollBar.Visible) 
			{
				this.vScrollBar.Left = this.ClientRectangle.Right-this.vScrollBar.Width;
				this.vScrollBar.Top = 0;
				if(this.hScrollBar.Visible) 
				{
					this.vScrollBar.Height = this.ClientRectangle.Height-this.hScrollBar.Height;
				}
				else 
				{
					this.vScrollBar.Height = this.ClientRectangle.Height;
				}
			}
			if(this.hScrollBar.Visible) 
			{
				this.hScrollBar.Top = this.ClientRectangle.Bottom-this.hScrollBar.Height;
				this.hScrollBar.Left = 0;
				if(this.vScrollBar.Visible) 
				{
					this.hScrollBar.Width = this.ClientRectangle.Width-this.vScrollBar.Width;
				}
				else 
				{
					this.hScrollBar.Width = this.ClientRectangle.Width;
				}
			}

			this.panelMain.Left = 0;
			this.panelMain.Top = 0;
			this.panelMain.Width = this.vScrollBar.Visible ?  this.ClientRectangle.Width-this.vScrollBar.Width : this.ClientRectangle.Width;
			this.panelMain.Height = this.hScrollBar.Visible ?  this.ClientRectangle.Height-this.hScrollBar.Height : this.ClientRectangle.Height;

			this.panelMain.Invalidate();
		}

		public virtual void OnClosed()
		{
			UserListConfigSave();
		}

		protected int GetListCount() 
		{
			return workStruct.block.Count; 
		}

		protected void InvalidateItem(int pos, int column_from, int column_to)
		{
			WORK_BLOCK_STRUCT work = workStruct;

			if(pos < work.nScrollVerPos)	return;	// zone over
			if(pos > work.nScrollVerPos+work.ylimit)	return;

			RECT r = new RECT();
			RECT rect = new RECT();

			rect.Set(this.panelMain.ClientRectangle);
			r.Set(this.panelMain.ClientRectangle);

			COLUMN_STRUCT col;
			int i, x;

			for(i = 0, x = -nScrollHorPos; i < (int)blockColumn.Count && x < rect.right; i++) 
			{
				col = (COLUMN_STRUCT)blockColumn[i];
				if(column_from == i)	r.left = x;
				if(column_to == i)		r.right = x+col.size;

				x += col.size;
			}

			if(r.left > r.right) 
			{
				r.left = rect.left;
				r.right = rect.right;
			}

			if(r.right < 0)	return;
			if(r.left > rect.right)	return;

			r.top = nStartY+(pos-work.nScrollVerPos)*work.cyChar;
			r.bottom = r.top+work.cyChar;

			Rectangle lr = new Rectangle(r.left, r.top, r.right-r.left, r.bottom-r.top);
			this.panelMain.Invalidate(lr);
		}

		protected object GetListBlock(int i)
		{
			WORK_BLOCK_STRUCT work = workStruct;

			if(i >= (int)work.block.Count) 
			{
				return null;
			}
			else 
			{
				return work.block[i];
			}
		}

		int		nOldMX, nStartMX, nCapturePos;
		bool bMouseCapture = false;

		protected virtual void OnMouseDownRight(System.Windows.Forms.MouseEventArgs e)
		{
			
		}
		
		private void panelMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(bMouseCapture)	return;

			if(e.Button == MouseButtons.Right) 
			{
				OnMouseDownRight(e);
				return;
			}

			WORK_BLOCK_STRUCT work = workStruct;

			if(bUseColumnHeader && e.Y < nStartY) 
			{
				COLUMN_STRUCT col;
				int i, x;
				RECT rect = new RECT();

				rect.Set(this.panelMain.ClientRectangle);

				for(i = 0, x = -nScrollHorPos; i < (int)blockColumn.Count && x < rect.right; i++) 
				{
					col = (COLUMN_STRUCT)blockColumn[i];
					if(e.X >= (x+col.size-1) && e.X <= (x+col.size)) 
					{
						this.panelMain.Capture = true;
						bMouseCapture = true;
						nStartMX = nOldMX = e.X;
						this.panelMain.Invalidate();
						nCapturePos = i;
						return;
					}
					x += col.size;
				}
			}

			if(bCursorUsed) 
			{
				int y, i, pos;

				for(i = 0, pos = work.nScrollVerPos, y = nStartY; i <= work.ylimit && pos < (int)work.block.Count; pos++, i++, y+=work.cyChar) 
				{
					if(e.Y >= y && e.Y < y+work.cyChar) 
					{
						if(nCursorY != pos) 
						{
							InvalidateItem(nCursorY, -1, -1);
							nCursorY = pos;
							InvalidateItem(nCursorY, -1, -1);
						}
						return;
					}
				}
			}	
		}

		private void panelMain_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(bMouseCapture == false) 
			{
				if(bUseColumnHeader && e.Y < nStartY) 
				{
					COLUMN_STRUCT col;
					int i, x;
					RECT rect = new RECT();

					rect.Set(this.panelMain.ClientRectangle);

					for(i = 0, x = -nScrollHorPos; i < (int)blockColumn.Count && x < rect.right; i++) 
					{
						col = (COLUMN_STRUCT)blockColumn[i];
						if(e.X >= (x+col.size-1) && e.X <= (x+col.size)) 
						{
							this.panelMain.Cursor = Cursors.SizeWE;
							return;
						}
						x += col.size;
					}
				}
				this.panelMain.Cursor = Cursors.Arrow;
				return;
			}

			nOldMX = e.X;
			this.panelMain.Invalidate();
		}

		private void panelMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(bMouseCapture == false)	return;

			this.panelMain.Capture = false;
			bMouseCapture = false;

			if(nOldMX == nStartMX)	return;

			bConfigChange = true;

			COLUMN_STRUCT col;
			col = (COLUMN_STRUCT)blockColumn[nCapturePos];
			col.size += (nOldMX-nStartMX);

			if(col.size < 10)	col.size = 10;
			if(col.size > 1000)	col.size = 1000;

			this.panelMain.Invalidate();
			ScrollUpdate(workStruct);	
		}

		void DrawNot(Graphics g, int x)
		{
			if(bMouseCapture == false)	return;

			Brush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
			g.FillRectangle(brush, x, 0, 2, nStartY);
		}

		protected void GetItemZone(int pos, int column_from, int column_to, RECT r)
		{
			WORK_BLOCK_STRUCT work = workStruct;

			r.Set(this.panelMain.ClientRectangle);

			r.top = nStartY+(pos-work.nScrollVerPos)*work.cyChar;
			r.bottom = r.top+work.cyChar;

			COLUMN_STRUCT col;
			int i, x;

			for(i = 0, x = -nScrollHorPos; i < (int)blockColumn.Count; i++) 
			{
				col = (COLUMN_STRUCT)blockColumn[i];
				if(column_from == i)	r.left = x;
				if(column_to == i)		r.right = x+col.size;

				x += col.size;
			}
		}

		private void vScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			WORK_BLOCK_STRUCT wk = workStruct;

			wk.nScrollVerPos = e.NewValue;
			this.panelMain.Invalidate();
		}

		private void hScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			WORK_BLOCK_STRUCT wk = workStruct;

			nScrollHorPos = e.NewValue;
			this.panelMain.Invalidate();
		}

		public int  GetCursorPos() { return nCursorY; }

		protected int  GetPagePos() { return workStruct.nScrollVerPos; }

		protected int  GetPageLimitY() { return workStruct.ylimit; }

		public void SetCursorPos(int cursor)
		{
			if(nCursorY == cursor)	return;

			InvalidateItem(nCursorY, -1, -1);
			nCursorY = cursor;
			InvalidateItem(nCursorY, -1, -1);
		}

		public void SetFont(Font font)
		{
			this.Font = font;
			FillFontSize(workStruct);
		}
	}
}

/*
int CWndUserList::SetListBlock(void *block, int i)
{
	WORK_BLOCK_STRUCT work = workStruct;

	if(i >= (int)work.block.Count) {
		return LB_ERR;
	}
	else {
		work.block.SetBlock(block, i);
		return 1;
	}
}

void CWndUserList::SetToLastPage()
{
	WORK_BLOCK_STRUCT work = workStruct;

	work.nScrollVerPos = work.block.Count-work.ylimit;
	if(work.nScrollVerPos < 0)	work.nScrollVerPos = 0;
	SetScrollPos(SB_VERT, work.nScrollVerPos, TRUE);
}

void CWndUserList::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags) 
{
	// TODO: Add your message handler code here and/or call default
	switch(nChar) {
		case VK_LEFT:
			break;
		case VK_RIGHT:
			break;
		case VK_UP:
			SendMessage(WM_VSCROLL, SB_LINEUP, 0L);	break;
		case VK_DOWN:
			SendMessage(WM_VSCROLL, SB_LINEDOWN, 0L);	break;
		case VK_PRIOR:
			SendMessage(WM_VSCROLL, SB_PAGEUP, 0L);	break;
		case VK_NEXT:
			SendMessage(WM_VSCROLL, SB_PAGEDOWN, 0L);	break;
	}
	
	CWnd::OnKeyDown(nChar, nRepCnt, nFlags);
}

BOOL CWndUserList::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt) 
{
	// TODO: Add your message handler code here and/or call default
	WORK_BLOCK_STRUCT *wk = &workStruct;

	int gab = zDelta/WHEEL_DELTA*2;

	wk->nScrollVerPos += (-gab);

	if(wk->nScrollVerPos < 0)
		wk->nScrollVerPos = 0;
	if(wk->nScrollVerPos > wk->nScrollVerHap)	
		wk->nScrollVerPos = wk->nScrollVerHap;

	SetScrollPos(SB_VERT, wk->nScrollVerPos, TRUE);

	InvalidateRect(NULL);
	
	return CWnd::OnMouseWheel(nFlags, zDelta, pt);
}





void CWndUserList::GetColumnPosWidth(int column, int &pos, int &width)
{
	WORK_BLOCK_STRUCT work = workStruct;

	COLUMN_STRUCT col;
	int i, x=0;

	for(i = 0; i < (int)blockColumn.Count; i++) {
		col = (COLUMN_STRUCT*)blockColumn->GetPtr(i);
		if(column == i)	{
			pos = x;
			width = col.size;
			return;
		}
		x += col.size;
	}

	pos = 0;
	width = 0;
}
*/
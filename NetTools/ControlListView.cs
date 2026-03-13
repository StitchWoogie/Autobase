using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace NetTools
{
	/// <summary>
	/// Summary description for ControlListView.
	/// </summary>
	public class ControlListView : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel1;
		public  System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		
		public		Font		font;
		public		int			fontX, fontY;
		public		ArrayList	header = new ArrayList();
		public		ArrayList	showData = new ArrayList();
		public		ArrayList	selectIndices = new ArrayList();
		public		int			listHap = 0;
		Color		frameColor = Color.FromArgb(236, 233, 216);			// 캡쳐하여 확인한 color
		public		Color		backColor = Color.White, textColor = Color.White;
		public		int			startPos = 0, currPos = 0, pageLineCount = 0, headHeight = 10;
		public		int			startX = 0;
		int			nDisplayWidth = 100, nDisplayHeight = 100;
		public		bool bSmallImage = false;				// 이미지 리스트를 사용할 것인가
		public		bool bOwnerDraw = false;				// 외부에서 그릴 것인지?
		public		bool bHscroll = false, bVscroll = false;// 스크롤 표시여부
		bool		bMouseCapture = false;					// header 부분 사이즈 변경을 선택했다?
		public		bool bMultiRowSelect = false;			// 멀티 선택을 할 것인가를 설정
		//bool		bShiftKey = false, bCtrlKey = false;	// Shift, Control, Alt 키가 눌러졌는가?
		int			nCaptureX = 0, nCaptureMouseX = 0;		// header 사이즈 변경을 위한 X 위치
		int			nCaptureHeaderPos = 0;					// header 사이즈 변경을 capture pos
		public		ImageList	SmallImageList;
		public		ContextMenu	contextMenu;				// 마우스 오른쪽 버턴을 눌렀을 때 표시되는 메뉴 바
		public		int	nScrollThick = 17;					// 스크롤의 굵기
        public Color colorCursorFocus = Color.FromArgb(100, Color.LightSkyBlue);
        public Color colorCursor = Color.FromArgb(100, Color.DarkGray);
        public Color colorCursorBorder = Color.FromArgb(63, Color.Black);

        public bool bOptionScrollHorz = true;   // 수평 스크롤을 사용한다. 2011-11-23
        public bool bOptionScrollVert = true;   // 수직 스크롤을 사용한다. 2011-11-23
				
		public ControlListView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//			
			this.TopLevel = false;
			this.Dock = System.Windows.Forms.DockStyle.Fill;
			
			this.panel1.Height = this.hScrollBar1.Height;
			this.panel2.Width = this.vScrollBar1.Width;
			hScrollBar1.Top = 0;
			hScrollBar1.Left = 0;
			vScrollBar1.Top = 0;
			vScrollBar1.Left = 0;
			//this.BackColor = SharedData.colorTotal.BACK;
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
				GC.Collect();
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.panel2 = new System.Windows.Forms.Panel();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.hScrollBar1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 336);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(504, 24);
			this.panel1.TabIndex = 0;
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Location = new System.Drawing.Point(8, 8);
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(416, 17);
			this.hScrollBar1.TabIndex = 0;
			this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
			this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.vScrollBar1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(480, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(24, 336);
			this.panel2.TabIndex = 1;
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Location = new System.Drawing.Point(8, 8);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 272);
			this.vScrollBar1.TabIndex = 0;
			this.vScrollBar1.ValueChanged += new System.EventHandler(this.vScrollBar1_ValueChanged);
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// ControlListView
			// 
			this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(504, 360);
			this.ControlBox = false;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ControlListView";
			this.ShowInTaskbar = false;
			this.Text = "ControlListView";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ControlListView_MouseDown);
			this.SizeChanged += new System.EventHandler(this.ControlListView_SizeChanged);
			this.Load += new System.EventHandler(this.ControlListView_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ControlListView_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ControlListView_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ControlListView_MouseMove);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ControlListView_Load(object sender, System.EventArgs e)
		{
			getFontXYAndHeaderHeight();
			getPageLineCount();
			this.LostFocus +=new EventHandler(ControlListView_LostFocus);
			this.GotFocus +=new EventHandler(ControlListView_GotFocus);
			this.MouseWheel +=new MouseEventHandler(ControlListView_MouseWheel);

            // 2007.6.14 OnPaintBitmap 대신 사용할 수 있다. 이 함수로 화면 떨림을 예방할 수 있다. OnPaintBitmap(Memory dc)을 사용하면 화면위에 투명한 윈도우가 오면 화면 떨림이 발생한다.
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
		}
		
		void getFontXYAndHeaderHeight()
		{
			fontY = (int)font.GetHeight();
			if(fontY < 2) fontY = 2;
			if(bSmallImage && this.SmallImageList != null) 
				if(fontY < SmallImageList.Images[0].Height) fontY = SmallImageList.Images[0].Height;			

			fontX = fontY/2;

			if(columnHeaderStyle == ColumnHeaderStyle.None)
				headHeight = 0;
			else
				headHeight = (int)(fontY*1.3);
		}

		//bool	bStart = true;			// 처음 currPos = StartPos 에 맞추기위해

		void fitValidStartPos()
		{
			//if(bStart == false) return;
			//bStart = false;
			if(bVscroll == false) return;
			if(currPos >= startPos && currPos < startPos + pageLineCount) return;

			if(pageLineCount <= 1) startPos = currPos;
			else startPos = currPos-pageLineCount+2;
			if(startPos > this.vScrollBar1.Maximum) startPos = vScrollBar1.Maximum;
			if(startPos < this.vScrollBar1.Minimum) startPos = vScrollBar1.Minimum;
			if(startPos < 0) startPos = 0;			// 2005/3/10 추가 startPos 가 -1 이하이면 다운, 혹시 문제가 있을 수 있으므로
		}

		public void listItemChanged()
		{
			if(currPos >= listHap) 
			{
				int			pos = currPos;
				currPos = (listHap <= 0) ? 0 : listHap-1;
				if(pos != currPos) releaseSelectedIndex(); // 멀티선택 시에는 선택된 위치를 지워야...
				EventGoOnEventSelectedIndexChanged();
			}
			scrollEnableDisable();
			fitValidStartPos();				// 시작위치를 설정하기 위해
			if(startPos < 0 || startPos >= listHap) startPos = (listHap <= 0) ? 0 : listHap-1;
			this.vScrollBar1.Value = startPos;			
			dataAreaInvalidate();
		}

		public void fontChanged()
		{
			getFontXYAndHeaderHeight();
			scrollEnableDisable();
			getPageLineCount();
			Invalidate();
		}

		public void listSelectedPosChange(int pos)
		{
			if(pos == currPos || pos < 0 || pos >= listHap) return;
			releaseSelectedIndex(); // 멀티선택 시에는 선택된 위치를 지워야...
			currPos = pos;
			EventGoOnEventSelectedIndexChanged();
			if(bVscroll) 
			{
				startPos = currPos;
				this.vScrollBar1.Value = startPos;
			}
			dataAreaInvalidate();
		}
		
		void DrawHeader(Graphics g)
		{
			if(columnHeaderStyle == ColumnHeaderStyle.None)	return;	// header가 없음

			if(fontY <= 0 || headHeight <= 1) return;
			DrawClass.PopBox2(g, Left, Top, Width+2, headHeight-1, frameColor);	// width +2 을 하여 마지막이 보이지 않게

			int x = startX, y = (int)(headHeight - fontY)/2, xGap = (int)(fontX*0.25);
			ControlListViewHeader	head;
			
			for(int i = 0; i < header.Count; i++) 
			{				
				head = (ControlListViewHeader)header[i];
                if (!head.bVisible) continue;

				if(x+head.width > 0)		// 유효한 영역만 그린다
				{
					DrawClass.PopBox2(g, x, Top, x+head.width-1, headHeight-1, frameColor);
					DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, fontY, head.text, Color.Black, frameColor, font, head.format);
                    if (this.nMouseMovePosOnColumnClikable == i)
                        DrawClass.grect(g, x, Top, x + head.width - 1, headHeight - 2, Color.DarkBlue);
				}
				x += head.width;
			}
		}

		int drawStartIcon(Graphics g, int x, int y, int index)
		{
			int			gap = fontY-SmallImageList.Images[index].Height;

			gap = (gap <= 0) ? 0 : gap/2;
			g.DrawImageUnscaled(SmallImageList.Images[index], x, y+gap);
			return SmallImageList.Images[index].Width;
		}

		void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < headHeight || pageLineCount <= 0 || fontY <= 0 || listHap <= 0) return;

			if(r.Top >= headHeight) DrawClass.gcls(g, r, backColor);
			else				    DrawClass.gcls(g, 0, headHeight, Width, r.Bottom, backColor);

			int						j, pos, x, y = headHeight, xGap = (int)(fontX*0.25), endPos;
			ControlListViewData		data;
			ControlListViewHeader	head;
			Color					color;

			endPos = pageLineCount+startPos+1;		// 1줄 더 그린다
			if(endPos > listHap) endPos = listHap;

			for(pos = startPos; pos < endPos; pos++, y += fontY) 
			{
				if(y > r.Bottom) break;
				if(y+fontY < r.Top) continue;				
				data = (ControlListViewData)showData[pos];
				x = startX;				
				
				for(j = 0; j < data.text.Length; j++, x += head.width) 
				{
					head = (ControlListViewHeader)header[j];
					color = (data.bUseTextColor) ? textColor :head.color;
					if(x+head.width > 0) 
					{		// 유효한 영역만 그린다
						if(j == 0) 
						{
							if(bSmallImage && this.SmallImageList != null) 
							{
								if(data.ImageIndex < SmallImageList.Images.Count) 
								{
									int		width = drawStartIcon(g, x, y, data.ImageIndex);
									if(head.width-width > 0)
										DrawClass.WinDrawText(g, x+xGap+width, y, head.width-xGap*2-width, (int)fontY, data.text[j], color, backColor, font, head.format);
								}
							}
							else
								DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)fontY, data.text[j], color, backColor, font, head.format);
							continue;
						}
						//if(pos == currPos) 
						//{
						//	DrawClass.gcls(g, x, y, x+head.width, y+fontY-1, textColor);
						//	DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)fontY, data.text[j], backColor, textColor, font, head.format);
						//}
						//else
						DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)fontY, data.text[j], color, backColor, font, head.format);
					}
				}
			}
		}

		void drawCurrentOneRowPos(Graphics g)
		{
			int		pos = currPos-startPos;
			if(pos > pageLineCount + 1 || pos < 0) return;
			
			int		y = headHeight + pos*fontY;

			Color		color;
			if(Focused) color = colorCursorFocus;
			else		color = colorCursor;

			DrawClass.gcls(g, 0, y, getTotalColumnWidth(), y+fontY-1, color);
			DrawClass.grect(g, 0, y, getTotalColumnWidth(), y+fontY-1, colorCursorBorder);
		}

		void drawCurrentMultiRowPos(Graphics g)
		{
			if(selectIndices.Count <= 0) 
			{
				selectIndices.Add(currPos);
				drawCurrentOneRowPos(g);
				return;
			}

			int				pos, y;
			Color			color;
			if(Focused) color = colorCursorFocus;
			else		color = colorCursor;

			for(int i = 0; i < selectIndices.Count; i++) 
			{
				pos = (int)selectIndices[i] - startPos;
				if(pos > pageLineCount + 1 || pos < 0) continue;
				y = headHeight + pos*fontY;
				DrawClass.gcls(g, 0, y, getTotalColumnWidth(), y+fontY-1, color);
				DrawClass.grect(g, 0, y, getTotalColumnWidth(), y+fontY-1, Color.FromArgb(63, Color.Black));			
			}
		}

		void drawCurrentPos(Graphics g)
		{
			if(bMultiRowSelect == false) drawCurrentOneRowPos(g);
			else					     drawCurrentMultiRowPos(g);
		}
		
	
        private void ControlListView_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (this.ClientRectangle.Height <= 0 || this.ClientRectangle.Width <= 0) return;		// 가로, 세로의 길이가 0 보다 적을 때는 그리지 않는다, 이 코드가 없으면 다운... 2005/3/10 추가

            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.CompositingMode = CompositingMode.SourceOver;

            if (e.ClipRectangle.Top < headHeight) DrawHeader(g);

            if (bOwnerDraw) EventGoOnEventPaintMessage(g, e.ClipRectangle);
            else            DrawData(g, e.ClipRectangle);

            drawCurrentPos(g);
            if (bMouseCapture) headerCaptureLineDraw(g);
        }

        void setFocusToThis()
		{
			if(Focused) return;
			Focus();
			oneLineInvalidate(currPos);		// 선택된 색상을 바꾼다
		}

		bool ArrowKeyOperation(Keys key)
		{
			if(pageLineCount <= 0 || listHap <= 0 || fontY <= 0) return true;
			
			int				hap;			
			switch(key)
			{
				case Keys.Up :
					hap = startPos+pageLineCount;
					if(currPos < startPos || currPos >= hap)	// 현재위치가 화면표시밖에 있다
					{
						releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
						currPos = startPos;
						EventGoOnEventSelectedIndexChanged();
						oneLineInvalidate(currPos);
						return true;
					}
					if(currPos <= 0) return true;
					
					if(startPos == currPos) 
					{
						releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
						currPos--;
						startPos--;
						EventGoOnEventSelectedIndexChanged();
						this.vScrollBar1.Value = startPos;
						dataAreaInvalidate();
					}
					else 
					{	
						releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
						currPos--;
						EventGoOnEventSelectedIndexChanged();
						oneLineInvalidate(currPos+1);
						oneLineInvalidate(currPos);
					}
					return true;
				case Keys.Down :
					hap = startPos+pageLineCount;
					if(currPos < startPos || currPos >= hap)	 // 현재위치가 화면표시밖에 있다 
					{
						releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
						currPos = startPos+pageLineCount-1;
						if(currPos >= listHap) currPos = listHap-1;
						EventGoOnEventSelectedIndexChanged();
						oneLineInvalidate(currPos);
						return true;
					}
					if(currPos >= listHap-1) return true;

					if(startPos+pageLineCount-1 == currPos) 
					{
						releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
						currPos++;
						startPos++;
						EventGoOnEventSelectedIndexChanged();
						this.vScrollBar1.Value = startPos;
						dataAreaInvalidate();
					}
					else 
					{
						releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
						currPos++;
						EventGoOnEventSelectedIndexChanged();
						oneLineInvalidate(currPos-1);
						oneLineInvalidate(currPos);
					}					
					return true;
				case Keys.PageUp :
					hap = startPos+pageLineCount;
					if(currPos < startPos || currPos >= hap)	// 현재위치가 화면표시밖에 있다
					{
						releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
						currPos = startPos;
						EventGoOnEventSelectedIndexChanged();
						oneLineInvalidate(currPos);
						return true;
					}
					if(startPos <= 0 && currPos == 0) return true;

					startPos = (startPos-pageLineCount < 0) ? 0 : startPos-pageLineCount;
					releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
					currPos = startPos;
					EventGoOnEventSelectedIndexChanged();
					this.vScrollBar1.Value = startPos;
					dataAreaInvalidate();
					return true;
				case Keys.PageDown :
					hap = startPos+pageLineCount;
					if(currPos < startPos || currPos >= hap)	 // 현재위치가 화면표시밖에 있다 
					{
						releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
						currPos = startPos+pageLineCount-1;
						if(currPos >= listHap) currPos = listHap-1;
						EventGoOnEventSelectedIndexChanged();
						oneLineInvalidate(currPos);
						return true;
					}
					if(currPos >= listHap-1 && currPos >= startPos && currPos < startPos+pageLineCount) return true;
					
					if(pageLineCount >= listHap) startPos = 0;	// 한 화면에 모든 것이 표시될 경우 start = 항상 0
					else						 startPos = (startPos+pageLineCount >= listHap) ? listHap-1 : startPos+pageLineCount;
					
					releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
					currPos = (startPos+pageLineCount-1 >= listHap) ? listHap-1 : startPos+pageLineCount-1;
					EventGoOnEventSelectedIndexChanged();
					this.vScrollBar1.Value = startPos;
					dataAreaInvalidate();
					return true;
				case Keys.Left :
					if(!bHscroll || hScrollBar1.Value <= 0) return true;
					hScrollBar1.Value = (hScrollBar1.Value-30 < 0) ? 0 : hScrollBar1.Value - 30;
					this.Invalidate();
					return true;
				case Keys.Right :
					if(!bHscroll || hScrollBar1.Value >= hScrollBar1.Maximum-1) return true;
					hScrollBar1.Value = (hScrollBar1.Value+30 >= hScrollBar1.Maximum) ? hScrollBar1.Maximum-1 : hScrollBar1.Value + 30;
					this.Invalidate();
					return true;
				case Keys.Home :
					if(currPos == 0 && startPos == 0) return true;

					releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
					currPos = 0;
					startPos = 0;
					EventGoOnEventSelectedIndexChanged();
					this.vScrollBar1.Value = startPos;
					dataAreaInvalidate();
					return true;
				case Keys.End :
					if(currPos >= listHap-1 && currPos >= startPos && currPos < startPos+pageLineCount) return true;
					
					startPos = (pageLineCount >= listHap) ? 0 : listHap-pageLineCount+1;
					releaseSelectedIndex();					// 멀티선택 시에는 선택된 위치를 지워야...
					currPos = listHap-1;
					EventGoOnEventSelectedIndexChanged();
					this.vScrollBar1.Value = startPos;
					dataAreaInvalidate();
					return true;
			}
			return false;
		}

		Shortcut getRealShortCutKey(Keys key, bool bAltKey)
		{
			int		keyValue = (int)key;

			if((Control.ModifierKeys & Keys.Control) == Keys.Control) keyValue += (int)Keys.Control;
			if((Control.ModifierKeys & Keys.Shift) == Keys.Shift) keyValue += (int)Keys.Shift;
			if((Control.ModifierKeys & Keys.Alt) == Keys.Alt) keyValue += (int)Keys.Alt;

			//if(bCtrlKey) keyValue += (int)Keys.Control;
			//if(bShiftKey) keyValue += (int)Keys.Shift;
			//if(bAltKey) keyValue += (int)Keys.Alt;

			return (Shortcut)keyValue;
		}

		void checkMenuShortKey(Keys key, bool bAltKey)
		{
			if(contextMenu == null) return;

			MenuItem		menu;
			for(int i = 0; i < contextMenu.MenuItems.Count; i++) 
			{
				menu = contextMenu.MenuItems[i];
				if(menu.Shortcut == Shortcut.None) continue;
				if(menu.Shortcut == getRealShortCutKey(key, bAltKey))
				{
					//bShiftKey = false;// 실행되기 때문에 Shift, Ctrl 키 플래그를 클리어해야 다음에 정상적인 키 조작이 가능
					//bCtrlKey = false;
					menu.PerformClick();
					return;
				}
			}
		}
				
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(msg.Msg == 0x100)	// key Down, 방향키는 ProcessCmdKey 에서만 들어온다
			{
				if(msg.WParam.ToInt32() == 0x21) ArrowKeyOperation(Keys.PageUp);
				if(msg.WParam.ToInt32() == 0x22) ArrowKeyOperation(Keys.PageDown);
				if(msg.WParam.ToInt32() == 0x25) ArrowKeyOperation(Keys.Left);
				if(msg.WParam.ToInt32() == 0x26) ArrowKeyOperation(Keys.Up);
				if(msg.WParam.ToInt32() == 0x27) ArrowKeyOperation(Keys.Right);
				if(msg.WParam.ToInt32() == 0x28) ArrowKeyOperation(Keys.Down);
				if(msg.WParam.ToInt32() == (int)Keys.Home) ArrowKeyOperation(Keys.Home);
				if(msg.WParam.ToInt32() == (int)Keys.End) ArrowKeyOperation(Keys.End);
				checkMenuShortKey((Keys)msg.WParam.ToInt32(), false);
			}
			if(msg.Msg == 0x104)	// Sys KeyDown ( Alt 키 일 때만 사용 )
			{
				checkMenuShortKey((Keys)msg.WParam.ToInt32(), true);
			}
			return base.ProcessCmdKey (ref msg, keyData);
		}

		protected override bool ProcessKeyEventArgs(ref Message m)
		{
			//if(bMultiRowSelect == false) return base.ProcessKeyEventArgs (ref m);

			if(m.Msg == 0x100)	// key Down	, 방향키가 들어오지 않으므로 ProcessCmdKey가 필요
			{
				//if(m.WParam.ToInt32() == (Int32)Keys.ShiftKey) bShiftKey = true;
				//if(m.WParam.ToInt32() == (Int32)Keys.ControlKey) bCtrlKey = true;
			}
			if(m.Msg == 0x101)	// key Up
			{
				//if(m.WParam.ToInt32() == (Int32)Keys.ShiftKey) bShiftKey = false;
				//if(m.WParam.ToInt32() == (Int32)Keys.ControlKey) bCtrlKey = false;
			}
			return base.ProcessKeyEventArgs (ref m);
		}


		int getTotalColumnWidth()
		{
			if(header == null) return 0;

			ControlListViewHeader	head;
			int						hap = 0;
			
			for(int i = 0; i < header.Count; i++) 
			{
				head = (ControlListViewHeader)header[i];
                if (!head.bVisible) continue;

				hap += head.width;
			}
			return hap;
		}

		void scrollEnableDisable()
		{
            bool old_bHscroll = bHscroll;
            bool old_bVscroll = bVscroll;

			int			width = this.Width, height = Height-headHeight;

			int			xHap = getTotalColumnWidth(), yHap = listHap*fontY;

            if (bOptionScrollHorz)
            {
                bHscroll = (xHap > width) ? true : false;
            }
            else
            {
                bHscroll = false;
            }

			if(bHscroll) height -= nScrollThick;//panel1.Height;

            if (bOptionScrollVert)
            {
                bVscroll = (yHap > height) ? true : false;
            }
            else
            {
                bVscroll = false;
            }

			if(bVscroll) width -= nScrollThick;//panel2.Width;
            if (bVscroll && bHscroll == false)
            {
                if (bOptionScrollHorz)
                {
                    bHscroll = (xHap > width) ? true : false;
                }
            }

			if(bHscroll) 
			{
				this.panel1.Height = nScrollThick;
			}
			else this.panel1.Height = 0;
			if(bVscroll) 
			{
				this.panel2.Width = nScrollThick;
			}
			else this.panel2.Width = 0;

			hScrollBar1.Width = (bVscroll) ? Width-panel2.Width : Width;		// 스크롤 화면 위치 조절
			vScrollBar1.Height = (bHscroll) ? Height-panel1.Height : Height;	// 스크롤 화면 위치 조절
			nDisplayWidth = hScrollBar1.Width;

			nDisplayHeight = vScrollBar1.Height-headHeight;
			
			//this.panel1.Visible = bHscroll;
			//this.hScrollBar1.Visible = bHscroll;
			//this.panel2.Visible = bVscroll;
			//this.vScrollBar1.Visible = bVscroll;

			getPageLineCount();
            if (bHscroll)
            {
                hScrollBar1.Minimum = 0;
                hScrollBar1.Maximum = (fontX <= 0) ? 0 : (xHap - nDisplayWidth) / fontX + 25 + 2;	// 2 칸 더 있어야 사이즈 조절을 할 수 있다
                if (hScrollBar1.Maximum < 0) hScrollBar1.Maximum = 0;
                hScrollBar1.LargeChange = 25;
            }
            else
                startX = 0;

			if(bVscroll)
			{
				this.vScrollBar1.Minimum = 0;
				vScrollBar1.Maximum = listHap+pageLineCount-2;							// listHap-1, pageLineCount-1
				if(vScrollBar1.Maximum < 0) vScrollBar1.Maximum = 0;
				vScrollBar1.LargeChange = (pageLineCount <= 0) ? 1 : pageLineCount;
			}
			else startPos = 0;			// 수직 스크롤이 없을 때는 스타트는 항상 0
		}

		void getPageLineCount()
		{
			if(fontY <= 0) pageLineCount = 0;
			else 
			{
				pageLineCount = (int)(nDisplayHeight / fontY);	// 표시할 라인 수를 얻는다
				if((nDisplayHeight % fontY) > fontY*3/4) pageLineCount++;
			}
		}

		private void ControlListView_SizeChanged(object sender, System.EventArgs e)
		{
			scrollEnableDisable();
			getPageLineCount();
			this.Invalidate();
		}


		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void vScrollBar1_ValueChanged(object sender, System.EventArgs e)
		{
			if(pageLineCount <= 0 || vScrollBar1.Value < 0) return;				// 스크롤 바가 -1 일 경우 다운되므로 ... 2005/3/10 수정
			startPos = this.vScrollBar1.Value;
			if(startPos >= listHap) startPos = (listHap <= 0) ? 0 : listHap-1;	// 스크롤 위치를 변경하면 이벤트가 발생하는데 listHap보다 클 수가 있으므로..
			dataAreaInvalidate();
		}

		private void hScrollBar1_ValueChanged(object sender, System.EventArgs e)
		{
			if(pageLineCount <= 0 || hScrollBar1.Value < 0) return;				// 스크롤 바가 -1 일 경우 다운되므로 ... 2005/3/10 수정
			startX = -hScrollBar1.Value*fontX;
			this.Invalidate();
			//dataAreaInvalidate();
		}

		public void dataAreaInvalidate()
		{
			this.Invalidate(new Rectangle(0, headHeight, Width, (pageLineCount+1)*fontY));
		}

		public void oneLineInvalidate(int pos)
		{
			int				y = headHeight+(pos-startPos)*fontY;

			if(y < headHeight || y >= headHeight+(pageLineCount+1)*fontY) return;

			Rectangle		r = new Rectangle(0, y, Width, fontY);
			this.Invalidate(r);
		}

        int nCaptureType = 0;   // 0 = 컬럼크기 변경, 1 = 컬럼헤더 누르는 경우 (소팅 등)

		bool checkPointHeaderEdge(int x)
		{
			int						i, px = startX;
			ControlListViewHeader	head;

			for(i = 0; i < header.Count; i++) 
			{
				head = (ControlListViewHeader)header[i];
                if (!head.bVisible) continue;

				px += head.width;

				if(x >= px-fontX && x <= px+fontX) 
				{
					nCaptureHeaderPos = i;
					nCaptureX = x;
					nCaptureMouseX = x;
                    nCaptureType = 0;
					return true;
				}
                // 컬럼헤더를 누르는 스타일
                if (this.columnHeaderStyle == ColumnHeaderStyle.Clickable)
                {
                    if (x <= px)
                    {
                        if (head.bClickable)
                        {
                            nCaptureHeaderPos = i;
                            nCaptureX = x;
                            nCaptureMouseX = x;
                            nCaptureType = 1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
			}

			return false;
		}

		bool menuItemShow(MouseEventArgs e, int curr)
		{
			if(e.Button != MouseButtons.Right || contextMenu == null) return false;

			if(curr != -1 && selectIndices.Count >= 1) 
			{
				int			i = 0;
				for( ; i < selectIndices.Count; i++) 
				{
					if(curr == (int)selectIndices[i]) break;
				}
				if(i >= selectIndices.Count) return false;
			}

			Point		p = new Point(e.X, e.Y);
            try
            {
                this.contextMenu.Show(this, p);
            }
            catch
            {
                // 보이지 않는 컨트롤에 ContextMenu를 표시할 수 없습니다.라는 메시지가 나타나서 오류처리함 어떤동작에서 발생하는 지는 잘 모름
            }
			return true;
		}

		void addSelectedIndex(int start, int end)
		{
			//selectIndices.Clear();
			int				i;
			if(start < end) 
			{
				for(i = start; i <= end; i++) 
				{
					selectIndices.Add(i);
					if(i >= startPos && i <= startPos+pageLineCount) oneLineInvalidate(i);
				}
			}
			else 
			{
				for(i = end; i <= start; i++) 
				{
					selectIndices.Add(i);
					if(i >= startPos && i <= startPos+pageLineCount) oneLineInvalidate(i);
				}
			}
		}

		// Control키를 눌렀을 때
		void addSelectedIndex(int pos)
		{
			if(pos >= startPos && pos <= startPos+pageLineCount) oneLineInvalidate(pos);
			if(this.selectIndices.Count <= 0) selectIndices.Add(pos);
			else 
			{				
				int		curr;
				for(int i = 0; i < selectIndices.Count; i++) // 위치를 정렬하기 위해
				{
					curr = (int)selectIndices[i];
					if(curr == pos)		// 이미 존재하면 뺀다.
					{
						this.selectIndices.RemoveAt(i);
						return;
					}
					if(curr > pos) 
					{
						this.selectIndices.Insert(i, pos);
						return;
					}
				}
				selectIndices.Add(pos);
			}
		}

		void releaseSelectedIndex()
		{
			if(bMultiRowSelect == false) return;

			int			pos;
			for(int i = 0; i < this.selectIndices.Count; i ++) 
			{
				pos = (int)selectIndices[i];
				if(pos >= startPos && pos <= startPos+pageLineCount) oneLineInvalidate(pos);
			}
			selectIndices.Clear();
		}

		private void ControlListView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			setFocusToThis();								// 포커스를 이동
			if(fontY <= 0) return;

			if(columnHeaderStyle == ColumnHeaderStyle.Nonclickable && e.Y >= 0 && e.Y < headHeight) 
			{		// header area checked
				if(e.Button != MouseButtons.Left) return;
				if(bMouseCapture) return;
				if(!checkPointHeaderEdge(e.X)) return;
				
				Cursor = Cursors.SizeWE;
				bMouseCapture = true;
				headerCaptureLineInvalidate();
				return;
			}

            if (columnHeaderStyle == ColumnHeaderStyle.Clickable && e.Y >= 0 && e.Y < headHeight)
            {		// header area checked
                if (e.Button != MouseButtons.Left) return;
                if (bMouseCapture) return;
                if (!checkPointHeaderEdge(e.X)) return;

                if (this.nCaptureType == 1)
                {
                    this.EventGoOnEventColumnHeaderClick(this.nCaptureHeaderPos);
                    return;
                }

                //Cursor = Cursors.SizeWE;
                bMouseCapture = true;
                //headerCaptureLineInvalidate();
                return;
            }

			int	pos = (e.Y-headHeight)/fontY;
			if(pos >= pageLineCount) return;

			pos += startPos;
			if(pos >= listHap) return;
			if(menuItemShow(e, pos)) return;
			if(pos == currPos) 
			{
				if(EventGoDoubleClick(e)) return;
				return;
			}

			if(bMultiRowSelect) 
			{
				//if(this.bShiftKey)
				if((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
				{
					releaseSelectedIndex();
					addSelectedIndex(currPos, pos);
					return;
				}
				if((Control.ModifierKeys & Keys.Control) == Keys.Control)
				{
					addSelectedIndex(pos);
					return;
				}
				releaseSelectedIndex();
				selectIndices.Add(pos);
			}
			
			
			int		oldPos = currPos;			
			
			currPos = pos;
			EventGoOnEventSelectedIndexChanged();
			if(oldPos >= startPos && oldPos <= startPos+pageLineCount)
				oneLineInvalidate(oldPos);
			oneLineInvalidate(currPos);			
			
			if(EventGoDoubleClick(e)) return;
			menuItemShow(e, -1);
			
			
		}	

		public delegate void OnEventDoubleClick();
		public OnEventDoubleClick doubleClick = null;

		public bool EventGoDoubleClick(System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Clicks < 2 || e.Button != MouseButtons.Left) return false;
			if(doubleClick == null)	return false;
			doubleClick();
			return true;
		}

		public delegate void OnEventSelectedIndexChanged();
		public OnEventSelectedIndexChanged selectedIndexChanged = null;

		public void EventGoOnEventSelectedIndexChanged()
		{
			if(selectedIndexChanged == null)	return;
			selectedIndexChanged();
		}

		public delegate void OnEventPaintMessage(Graphics g, Rectangle r);
		public OnEventPaintMessage paintMessage = null;

		public void EventGoOnEventPaintMessage(Graphics g, Rectangle r)
		{
			if(paintMessage == null)	return;
			if(r.Width <= 1 || r.Height <= 1) return;
			paintMessage(g, r);
		}

		public void headerCaptureLineDraw(Graphics g)
		{
			DrawClass.gline(g, nCaptureMouseX, 0, nCaptureMouseX, headHeight-1, Color.Black);
		}

		public void headerCaptureLineInvalidate()
		{
			Rectangle	r = new Rectangle(nCaptureMouseX, 0, 1, headHeight-1);
			this.Invalidate(r);
		}

        int nMouseMovePosOnColumnClikable = -1;

        void SetMouseMovePosOnColumnClikable(int pos)
        {
            if (nMouseMovePosOnColumnClikable == pos) return;

            nMouseMovePosOnColumnClikable = pos;

            Rectangle r = new Rectangle(0, 0, this.ClientRectangle.Width, headHeight - 1);
            this.Invalidate(r);
        }

        void MouseMoveCheckHeader(int x)
		{
			int						i, px = startX;
			ControlListViewHeader	head;

			for(i = 0; i < header.Count; i++) 
			{
				head = (ControlListViewHeader)header[i];
                if (!head.bVisible) continue;

				px += head.width;
                if (x >= px - fontX && x <= px + fontX)
                {
                    Cursor = Cursors.SizeWE;
                    return;
                }
				
                Cursor = Cursors.Arrow;

                if (this.columnHeaderStyle == ColumnHeaderStyle.Clickable)
                {
                    if (x <= px)
                    {
                        if (head.bClickable)
                        {
                            SetMouseMovePosOnColumnClikable(i);
                            return;
                        }
                        else
                        {
                            SetMouseMovePosOnColumnClikable(-1);
                            return;
                        }
                    }
                }
			}

            SetMouseMovePosOnColumnClikable(-1);

			return;
		}

		void mouseMove(int x, int y)
		{
			if(bMouseCapture) 
			{
				if(nCaptureMouseX == x) return;
		
				Cursor = Cursors.SizeWE;
				headerCaptureLineInvalidate();
				nCaptureMouseX = x;
				headerCaptureLineInvalidate();
				return;
			}

			if(y < headHeight) 
			{
				MouseMoveCheckHeader(x); 

				return;
			}

            SetMouseMovePosOnColumnClikable(-1);

			//if(!bMouseCapture) 
			//{
			Cursor = Cursors.Arrow;				
			//	return;
			//}
		}

		private void ControlListView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mouseMove(e.X, e.Y);
		}

		bool mouseUp(int x)
		{
			if(!bMouseCapture) return false;
	
			Cursor = Cursors.Arrow;
			
			headerCaptureLineInvalidate();
			//AdDisplayCaptureLineDraw(g);
			bMouseCapture = false;
			if(x == nCaptureX || nCaptureHeaderPos >= header.Count) return false;
	
			ControlListViewHeader	head = (ControlListViewHeader)header[nCaptureHeaderPos];
			int						width = head.width;
			
			width += x-nCaptureX;
			if(width < 32) width = 32;
			head.width = width;
			return true;	
		}


		private void ControlListView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left) return;
			
			if(mouseUp(e.X)) 
			{
				scrollEnableDisable();			// 스크롤 위치가 바뀔 수도 있으므로
				if(bHscroll == false) this.hScrollBar1.Value = 0;
				else if(hScrollBar1.Value >= hScrollBar1.Maximum) hScrollBar1.Value = (hScrollBar1.Maximum <= 0) ? 0 : hScrollBar1.Maximum-1;				
				Invalidate();
			}
		}
		
		private void hScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			setFocusToThis();								// 포커스를 이동
		}

		private void vScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			setFocusToThis();								// 포커스를 이동
		}

		private void ControlListView_LostFocus(object sender, System.EventArgs e)
		{
			oneLineInvalidate(currPos);		// 선택된 색상을 바꾼다
		}

		private void ControlListView_GotFocus(object sender, System.EventArgs e)
		{
			oneLineInvalidate(currPos);		// 선택된 색상을 바꾼다
		}

		private void ControlListView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(bVscroll == false) return;
			int		gap = -e.Delta/fontY;
			if(gap == 0) return;

			startPos += gap;
			if(startPos < 0) startPos = 0;
			if(startPos >= listHap) startPos = listHap-1;

            if (startPos < this.vScrollBar1.Minimum) startPos = this.vScrollBar1.Minimum;
            if (startPos > this.vScrollBar1.Maximum) startPos = this.vScrollBar1.Maximum;   // 화면이 아주 작은 경우는 안 맞는 경우가 있다. 2011-11-25

			this.vScrollBar1.Value = startPos;
			dataAreaInvalidate();
		}

		ColumnHeaderStyle columnHeaderStyle = ColumnHeaderStyle.Nonclickable;

		public ColumnHeaderStyle HeaderStyle 
		{
			set 
			{
				columnHeaderStyle = value;
			}
			get 
			{
				return columnHeaderStyle;
			}
		}

        public delegate void OnEventColumnHeaderClick(int pos);
        public OnEventColumnHeaderClick onEventColumnHeaderClick = null;

        public void EventGoOnEventColumnHeaderClick(int pos)
        {
            if (onEventColumnHeaderClick == null) return;
            onEventColumnHeaderClick(pos);
        }

        ControlListViewHeader SeekColumnByID(int id)
        {
            ControlListViewHeader clvh;

            for (int i = 0; i < header.Count; i++)
            {
                clvh = (ControlListViewHeader)header[i];
                if (id == clvh.nID)
                {
                    return clvh;
                }
            }

            return null;
        }

        public void ConfigLoad(string cfg_dir, string filename, string section)
        {
            string filepath;

            CommaBlockString comma = new CommaBlockString();

            filepath = String.Format("{0}\\ListViewConfig\\{1}_{2}", cfg_dir, filename, section);

            if (!File.Exists(filepath)) return;

            string buf;

            TextReader reader = new StreamReader(filepath);
            if (reader == null) return;

            int id;
            ControlListViewHeader clvh;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                id = comma.GetInt();

                clvh = SeekColumnByID(id);

                if (clvh != null)
                {
                    clvh.width = comma.GetInt();
                    clvh.bVisible = comma.GetBool();

                    if (clvh.width != 0)
                    {
                        if (clvh.width > 1000) clvh.width = 50;
                        if (clvh.width < 10) clvh.width = 10;
                    }
                }
            }
            reader.Close();
        }

        public void ConfigSave(string cfg_dir, string filename, string section)
        {
            string filepath;
            int i;
            CommaBlockString comma = new CommaBlockString();

            filepath = String.Format("{0}\\ListViewConfig", cfg_dir);
            Directory.CreateDirectory(filepath);
            filepath = String.Format("{0}\\ListViewConfig\\{1}_{2}", cfg_dir, filename, section);

            TextWriter writer = new StreamWriter(filepath);
            if (writer == null) return;

            ControlListViewHeader clvh;

            for (i = 0; i < header.Count; i++)
            {
                clvh = (ControlListViewHeader)header[i];
                writer.WriteLine("{0},{1},{2}", clvh.nID, clvh.width, clvh.bVisible);
            }

            writer.Close();

        }

        public bool DialogConfigColumn()
        {
            ControlListViewConfigColumn dialog = new ControlListViewConfigColumn();

            dialog.Set(header);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.Get(header);
                this.Invalidate();
            }

            return true;
        }
	}

	public class ControlListViewHeader
	{
		public int			width;
		public string		text;
		public StringFormat	format;
		public Color		color;
        public bool         bClickable = true;
        public bool         bVisible = true;    // 컬럼을 안보이도록 설정할 수 있다.                 2016-4-30
        public int          nID;                // 환경을 저장할 때 이 번호를 가지고 구분할 수 있다. 컬럼의 고유번호를 부여하여 사용한다. 2016-4-30
	};

	public class ControlListViewData
	{
		public bool						bUseTextColor;			// 현재 라인은 textColor로 뿌린다
		public int						ImageIndex;
		public string[]					text;
	};


}

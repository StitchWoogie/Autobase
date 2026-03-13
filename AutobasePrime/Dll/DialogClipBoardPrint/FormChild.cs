using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Printing;
using NetTools;
using System.Drawing.Imaging;

namespace DialogClipBoardPrint
{
	/// <summary>
	/// Summary description for FormChild.
	/// </summary>
	public class FormChild : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		Bitmap bitmapSource;
		Bitmap bitmapReverse;
		int px1, py1, px2, py2;
		bool bReverse = false;
		public PageSettings savePageSetting = new PageSettings();

		public void SetPx(int x1, int y1, int x2, int y2)
		{
			if(x1 == 0 && y1 == 0 && x2 == 0 && y2 == 0) 
			{
				CalcDefaultPX();
				Invalidate();
				return;
			}

			px1 = x1;
			py1 = y1;
			px2 = x2;
			py2 = y2;

			if(px1 > px2)	Tools.Temp(ref px1, ref px2);
			if(py1 > py2)	Tools.Temp(ref py1, ref py2);

			if(px2 <= 0 || px1 >= ClientRectangle.Right-1 ||
				py2 <= 0 || py1 >= ClientRectangle.Bottom-1) 
			{
				CalcDefaultPX();
			}

			Invalidate();
		}

		public void GetPx(out int x1, out int y1, out int x2, out int y2)
		{
			x1 = px1;
			y1 = py1;
			x2 = px2;
			y2 = py2;
		}

		void MakeReverseBitmap()
		{
			if(bitmapSource == null)	return;
			if(bitmapReverse == null) 
			{
				//if(bitmapSource.PixelFormat == PixelFormat.Format32bppRgb ||
				//	bitmapSource.PixelFormat == PixelFormat.Format24bppRgb ||
				//	bitmapSource.PixelFormat == PixelFormat.Format32bppArgb) 
				{
					bitmapReverse = new Bitmap(bitmapSource);
					int x, y;
					Color color;

					for(y = 0; y < bitmapReverse.Height; y++) 
					{
						for(x = 0; x < bitmapReverse.Width; x++) 
						{
							color = bitmapReverse.GetPixel(x, y);
							color = Color.FromArgb(0xFF-color.R, 0xFF-color.G, 0xFF-color.B);
							bitmapReverse.SetPixel(x, y, color);
						}
					}
				}
			}
		}

		public FormChild(Bitmap prepare)
		{
			//
			// Required for Windows Form Designer support
			//

			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			if(prepare != null) 
			{
				bitmapSource = prepare;
			}
			else 
			{
				DataObject data = (DataObject)Clipboard.GetDataObject();

				if(data.GetDataPresent(DataFormats.Bitmap)) 
				{
					bitmapSource = (Bitmap)data.GetData(DataFormats.Bitmap);
				}
				else if(data.GetDataPresent(DataFormats.Dib))	// 98일때는 클립보드에 DIB로 저장된다.
				{
					bitmapSource = (Bitmap)data.GetData(DataFormats.Bitmap);
				}
				else {}
			}

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
			// 
			// FormChild
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(328, 266);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormChild";
			this.Text = "FormChild";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormChild_MouseDown);
			this.SizeChanged += new System.EventHandler(this.FormChild_SizeChanged);
			this.Load += new System.EventHandler(this.FormChild_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormChild_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormChild_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormChild_MouseMove);

		}
		#endregion

		void DrawNotRectangle(Graphics g, int x, int y)
		{
			g.FillRectangle(new SolidBrush(Color.White), x-2, y-2, 5, 5);
			g.DrawRectangle(new Pen(Color.Black), x-2, y-2, 5, 5);
		}
		
		private void FormChild_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(bReverse) MakeReverseBitmap();			

			Bitmap bitmap;

			if(bReverse && bitmapReverse != null)	bitmap = bitmapReverse;
			else									bitmap = bitmapSource;

			if(bitmap != null) 
			{
				e.Graphics.DrawImage(bitmap, px1, py1, px2-px1, py2-py1);
			}

			DrawNotRectangle(e.Graphics, px1, py1);
			DrawNotRectangle(e.Graphics, px2, py1);
			DrawNotRectangle(e.Graphics, px2, py2);
			DrawNotRectangle(e.Graphics, px1, py2);

			DrawNotRectangle(e.Graphics, px1+(px2-px1)/2, py1);
			DrawNotRectangle(e.Graphics, px1+(px2-px1)/2, py2);
			DrawNotRectangle(e.Graphics, px1, py1+(py2-py1)/2);
			DrawNotRectangle(e.Graphics, px2, py1+(py2-py1)/2);
		}

		void CalcDefaultPX()
		{
			int width;
			int height;

			if(bitmapSource == null) 
			{
				width  = (int)(ClientRectangle.Width*0.7);	
				height = (int)(ClientRectangle.Height*0.7);
			}
			else 
			{
				width = ClientRectangle.Width;
				height = bitmapSource.Height*width/bitmapSource.Width;

				if(height > ClientRectangle.Height) 
				{
					height = ClientRectangle.Height;
					width = bitmapSource.Width*height/bitmapSource.Height;
				}
				else 
				{
					width = ClientRectangle.Width;
					height = bitmapSource.Height*width/bitmapSource.Width;
				}

				width = (int)(width*0.7);
				height = (int)(height*0.7);
			}

			px1 = ClientRectangle.Width/2-width/2;
			py1 = ClientRectangle.Height/2-height/2;

			px2 = px1+width;
			py2 = py1+height;
		}

		private void FormChild_Load(object sender, System.EventArgs e)
		{
			CalcDefaultPX();
		}

		private void FormChild_SizeChanged(object sender, System.EventArgs e)
		{
			CalcDefaultPX();
		}

		public void PrintGo(PrinterSettings prnSetting, PageSettings pageSetting)
		{
			PrintDocument pd = new PrintDocument();

			pd.PrinterSettings     = prnSetting;
			pd.DefaultPageSettings = pageSetting;

			pd.PrintPage += new PrintPageEventHandler
				(this.pd_PrintPage);

			pd.Print();
		}

		private void pd_PrintPage(object sender, PrintPageEventArgs ev) 
		{
			Bitmap bitmap;

			if(bReverse) MakeReverseBitmap();

			if(bReverse && bitmapReverse != null)	bitmap = bitmapReverse;
			else									bitmap = bitmapSource;

			if(bitmap != null) 
			{
				int x1, y1, x2, y2;

				x1 = px1*ev.PageSettings.Bounds.Width/ClientRectangle.Width;
				x2 = px2*ev.PageSettings.Bounds.Width/ClientRectangle.Width;
				y1 = py1*ev.PageSettings.Bounds.Height/ClientRectangle.Height;
				y2 = py2*ev.PageSettings.Bounds.Height/ClientRectangle.Height;
				
				ev.Graphics.DrawImage(bitmap, x1, y1, x2-x1, y2-y1);
			}

			ev.HasMorePages = false;
            /*
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height / 
                printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while(count < linesPerPage && 
                ((line=streamToPrint.ReadLine()) != null)) 
            {
                yPos = topMargin + (count * 
                    printFont.GetHeight(ev.Graphics));
                SafeException.SafeDrawString(ev.Graphics, line, printFont, Brushes.Black, 
                    leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if(line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
            */

        }

		bool IsMouseInclude(MouseEventArgs e, int x, int y)
		{
			if(e.X >= x-2 && e.X <= x+2 && e.Y >= y-2 && e.Y <= y+2) 
				return true;

			return false;
		}

		enum EnumSelectPosition 
		{
			none,
			LeftTop,
			RightTop,
			LeftBottom,
			RightBottom,
			LeftMid,
			RightMid,
			TopMid,
			BottomMid,
			Center,
		}

		EnumSelectPosition GetSelectPosition(MouseEventArgs e)
		{
			EnumSelectPosition pos;

			if(IsMouseInclude(e, px1, py1)) 
				pos = EnumSelectPosition.LeftTop;
			else if(IsMouseInclude(e, px2, py1)) 
				pos = EnumSelectPosition.RightTop;
			else if(IsMouseInclude(e, px2, py2)) 
				pos = EnumSelectPosition.RightBottom;
			else if(IsMouseInclude(e, px1, py2)) 
				pos = EnumSelectPosition.LeftBottom;
			else if(IsMouseInclude(e, px1+(px2-px1)/2, py1))
				pos = EnumSelectPosition.TopMid;
			else if(IsMouseInclude(e, px1+(px2-px1)/2, py2))
				pos = EnumSelectPosition.BottomMid;
			else if(IsMouseInclude(e, px1, py1+(py2-py1)/2))
				pos = EnumSelectPosition.LeftMid;
			else if(IsMouseInclude(e, px2, py1+(py2-py1)/2))
				pos = EnumSelectPosition.RightMid;
			else if(e.X >= px1 && e.X <= px2 && e.Y >= py1 && e.Y <= py2) 
				pos = EnumSelectPosition.Center;
			else
				pos = EnumSelectPosition.none;

			return pos;
		}

		EnumSelectPosition enumSelectPosition;
		int nStartX,nStartY;
		int nOldX1, nOldY1, nOldX2, nOldY2;

        private void FormChild_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(Capture) 
			{
				int gabx = e.X-nStartX;
				int gaby = e.Y-nStartY;
				px1 = nOldX1;
				py1 = nOldY1;
				px2 = nOldX2;
				py2 = nOldY2;

				if(enumSelectPosition == EnumSelectPosition.LeftTop) 
				{
					px1 += gabx;
					py1 += gaby;
				}
				else if(enumSelectPosition == EnumSelectPosition.RightTop)
				{
					px2 += gabx;
					py1 += gaby;	
				}
				else if(enumSelectPosition == EnumSelectPosition.RightBottom)
				{
					px2 += gabx;
					py2 += gaby;	
				}
				else if(enumSelectPosition == EnumSelectPosition.LeftBottom)
				{
					px1 += gabx;
					py2 += gaby;	
				}
				else if(enumSelectPosition == EnumSelectPosition.TopMid)
				{
					py1 += gaby;	
				}
				else if(enumSelectPosition == EnumSelectPosition.BottomMid)
				{
					py2 += gaby;	
				}
				else if(enumSelectPosition == EnumSelectPosition.LeftMid)
				{
					px1 += gabx;	
				}
				else if(enumSelectPosition == EnumSelectPosition.RightMid)
				{
					px2 += gabx;	
				}
				else if(enumSelectPosition == EnumSelectPosition.Center)
				{
					px1 += gabx;
					px2 += gabx;
					py1 += gaby;
					py2 += gaby;
				}

				if(px1 > px2)	Tools.Temp(ref px1, ref px2);
				if(py1 > py2)	Tools.Temp(ref py1, ref py2);

				Invalidate();
			}
			else 
			{
				EnumSelectPosition pos = GetSelectPosition(e);

				if(pos == EnumSelectPosition.LeftTop) 
					Cursor = Cursors.SizeNWSE;
				else if(pos == EnumSelectPosition.RightTop) 
					Cursor = Cursors.SizeNESW;
				else if(pos == EnumSelectPosition.RightBottom) 
					Cursor = Cursors.SizeNWSE;
				else if(pos == EnumSelectPosition.LeftBottom) 
					Cursor = Cursors.SizeNESW;
				else if(pos == EnumSelectPosition.TopMid)
					Cursor = Cursors.SizeNS;
				else if(pos == EnumSelectPosition.BottomMid)
					Cursor = Cursors.SizeNS;
				else if(pos == EnumSelectPosition.LeftMid)
					Cursor = Cursors.SizeWE;
				else if(pos == EnumSelectPosition.RightMid)
					Cursor = Cursors.SizeWE;
				else if(pos == EnumSelectPosition.Center) 
					Cursor = Cursors.SizeAll;
				else
					Cursor = Cursors.Default;
			}
		}

		private void FormChild_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			EnumSelectPosition pos = GetSelectPosition(e);
			
			if(pos == EnumSelectPosition.none)	return;

			this.Capture = true;
			enumSelectPosition = pos;
			nStartX = e.X;
			nStartY = e.Y;
			nOldX1 = px1;
			nOldY1 = py1;
			nOldX2 = px2;
			nOldY2 = py2;
		}

		private void FormChild_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(this.Capture == false)	return;

			Capture = false;

			SetPx(px1, py1, px2, py2);
		}

		public void SetReverse(bool reverse)
		{
			bReverse = reverse;
			
			Invalidate();
		}
	}
}

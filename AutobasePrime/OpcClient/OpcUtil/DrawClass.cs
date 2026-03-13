using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using NetTools.OldDefine;


namespace NetTools
{
	/// <summary>
	/// Summary description for DrawClass.
	/// </summary>
	public class DrawClass
	{
		public static StringFormat StringFormatCenter;
		public static StringFormat StringFormatRight;
		public static StringFormat StringFormatLeft;

		static DrawClass()
		{
			//
			// TODO: Add constructor logic here
			//
			StringFormatCenter = new StringFormat();
			StringFormatCenter.Alignment = StringAlignment.Center;
			StringFormatCenter.LineAlignment = StringAlignment.Center;

			StringFormatRight = new StringFormat();
			StringFormatRight.Alignment = StringAlignment.Far;

			StringFormatLeft = new StringFormat();
			StringFormatLeft.Alignment = StringAlignment.Near;
		}

		public static void gline(Graphics g, int x1, int y1, int x2, int y2, Color color)
		{
			Pen	pen = new Pen(color);
			
			g.DrawLine(pen, x1, y1, x2, y2);			
		}

		public static void grectangle(Graphics g, int x1, int y1, int width, int height, Color color)
		{
			Pen	pen = new Pen(color);
			
			g.DrawRectangle(pen, x1, y1, width, height);
		}

		public static void grect(Graphics g, int x1, int y1, int x2, int y2, Color color)
		{
			Pen	pen = new Pen(color);
			g.DrawLine(pen, x1, y1, x2, y1);
			g.DrawLine(pen, x2, y1, x2, y2);
			g.DrawLine(pen, x2, y2, x1, y2);
			g.DrawLine(pen, x1, y2, x1, y1);
		}

		public static void PopRectangle(Graphics g, int x1, int y1, int x2, int y2)
		{
			Pen	penBlack = Pens.Black;
			Pen	penWhite = Pens.White;
			Pen	penGray = new Pen(Color.FromArgb(0x80, 0x80, 0x80));

			g.DrawLine(penWhite, x1+1, y2-1, x1+1, y1+1);
			g.DrawLine(penWhite, x1+1, y1+1, x2-1, y1+1);
			
			g.DrawLine(penGray, x2-1, y1+1, x2-1, y2-1);
			g.DrawLine(penGray, x2-1, y2-1, x1+1, y2-1);

			g.DrawLine(penWhite, x1+2, y2-2, x1+2, y1+2);
			g.DrawLine(penWhite, x1+2, y1+2, x2-2, y1+2);
						
			g.DrawLine(penGray, x2-2, y1+2, x2-2, y2-2);
			g.DrawLine(penGray, x2-2, y2-2, x1+2, y2-2);

			g.DrawLine(penBlack, x1, y1, x2, y1);
			g.DrawLine(penBlack, x2, y1, x2, y2);
			g.DrawLine(penBlack, x2, y2, x1, y2);
			g.DrawLine(penBlack, x1, y2, x1, y1);			
		}

		public static void PopRectangle(Graphics g, Rectangle rect)
		{
			PopRectangle(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1);
		}

		public static void PopRectangle2(Graphics g, int x1, int y1, int x2, int y2)
		{
			Pen	penWhite = Pens.White;
			Pen	penGray = new Pen(Color.FromArgb(0x80, 0x80, 0x80));

			g.DrawLine(penWhite, x1, y2, x1, y1);
			g.DrawLine(penWhite, x1, y1, x2, y1);			

			g.DrawLine(penGray, x2, y1, x2, y2);
			g.DrawLine(penGray, x2, y2, x1, y2);			
		}

		public static void PopRectangle2(Graphics g, Rectangle rect)
		{
			PopRectangle2(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1);
		}

		public static void gcls(Graphics g, int x1, int y1, int x2, int y2, Color color)
		{
			Brush	brush = new SolidBrush(color);

			g.FillRectangle(brush, x1, y1, x2-x1+1, y2-y1+1);
		}

		public static void gcls(Graphics g, Rectangle rect, Color color)
		{
			gcls(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1, color);
		}

		public static void gcls(Graphics g, RectangleF rect, Color color)
		{
			gcls(g, (int)rect.X, (int)rect.Y, (int)(rect.X+rect.Width-1), (int)(rect.Y+rect.Height-1), color);
		}

		public static void PopBox(Graphics g, int x1, int y1, int x2, int y2, Color color)
		{			
			gcls(g, x1, y1, x2, y2, color);
			PopRectangle(g, x1, y1, x2, y2);
		}

		public static void PopBox(Graphics g, Rectangle rect, Color color)
		{			
			PopBox(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1, color);
		}

		public static void PopBox2(Graphics g, int x1, int y1, int x2, int y2, Color color)
		{
			gcls(g, x1+1, y1+1, x2-1, y2-1, color);
			PopRectangle2(g, x1, y1, x2, y2);
		}

		public static void PopBox2(Graphics g, Rectangle rect, Color color)
		{
			PopBox2(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1, color);
		}

		public static void PushRectangle(Graphics g, int x1, int y1, int x2, int y2)
		{
			Pen	penBlack = Pens.Black;
			Pen	penWhite = Pens.White;
			Pen	penGray = new Pen(Color.FromArgb(0x80, 0x80, 0x80));
			
			g.DrawLine(penGray, x1, y2, x1, y1);
			g.DrawLine(penGray, x1, y1, x2, y1);

			g.DrawLine(penWhite, x2, y1, x2, y2);
			g.DrawLine(penWhite, x2, y2, x1, y2);

			g.DrawLine(penGray, x1+1,y2-1, x1+1,y1+1);
			g.DrawLine(penGray, x1+1,y1+1, x2-1,y1+1);

			g.DrawLine(penWhite, x2-1,y1+1, x2-1,y2-1);
			g.DrawLine(penWhite, x2-1,y2-1, x1+1,y2-1);			

			g.DrawLine(penBlack, x1+2, y1+2, x2-2, y1+2);
			g.DrawLine(penBlack, x2-2, y1+2, x2-2, y2-2);
			g.DrawLine(penBlack, x2-2, y2-2, x1+2, y2-2);
			g.DrawLine(penBlack, x1+2, y2-2, x1+2, y1+2);
		}

		public static void PushRectangle(Graphics g, Rectangle rect)
		{
			PushRectangle(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1);
		}

		public static void PushRectangle2(Graphics g, int x1, int y1, int x2, int y2)
		{
			Pen	penWhite = Pens.White;
			Pen	penGray = new Pen(Color.FromArgb(0x80, 0x80, 0x80));
			
			g.DrawLine(penGray, x1, y2, x1, y1);
			g.DrawLine(penGray, x1, y1, x2, y1);
			
			g.DrawLine(penWhite, x2, y1, x2, y2);
			g.DrawLine(penWhite, x2, y2, x1, y2);
		}


		public static void PushBox(Graphics g, int x1, int y1, int x2, int y2, Color color)
		{
			gcls(g, x1, y1, x2, y2, color);
			PushRectangle(g, x1, y1, x2, y2);
		}

		public static void PushBox(Graphics g, Rectangle rect, Color color)
		{
			PushBox(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1, color);
		}

		public static void PushBox2(Graphics g, int x1, int y1, int x2, int y2, Color color)
		{
			gcls(g, x1, y1, x2, y2, color);
			PushRectangle2(g, x1, y1, x2, y2);
		}

		public static void PushBox2(Graphics g, Rectangle rect, Color color)
		{
			PushBox2(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1, color);
		}

		public static void PushRectangle3(Graphics g, int x1, int y1, int x2, int y2)
		{
			Pen hWhitePen = Pens.White;
			Pen hDarkGrayPen = new Pen(Color.FromArgb(0x80, 0x80, 0x80), 1);
			Pen hWhiteGrayPen = new Pen(Color.FromArgb(0xC0, 0xC0, 0xC0), 1);
			Pen hDarkPen = Pens.Black;

			g.DrawLine(hDarkGrayPen, x1, y2, x1, y1);
			g.DrawLine(hDarkGrayPen, x1, y1, x2, y1);

			g.DrawLine(hWhitePen, x2, y1, x2, y2);
			g.DrawLine(hWhitePen, x2, y2, x1, y2);

			g.DrawLine(hDarkPen, x1+1, y2-1, x1+1, y1+1);
			g.DrawLine(hDarkPen, x1+1, y1+1, x2-1, y1+1);

			g.DrawLine(hWhiteGrayPen, x2-1, y1+1, x2-1, y2-1);
			g.DrawLine(hWhiteGrayPen, x2-1, y2-1, x1+1, y2-1);
		}

		public static void PushBox3(Graphics g, int x1, int y1, int x2, int y2, Color color)
		{
			gcls(g, x1, y1, x2, y2, color);
			PushRectangle3(g, x1, y1, x2, y2);
		}

		public static void GrayDrawText(Graphics g, int x, int y, int width, int height, String buf, Color bcolor, Font f, StringFormat format)
		{
			Rectangle	rect = new Rectangle(x+1, y+1, width-1, height-1);
			Brush		brush = Brushes.White;

			//SetTextColor(hdc, WHITE);
			//SetBkColor(hdc, bcolor);
			g.DrawString(buf, f, brush, rect, format);
					
			brush = Brushes.Black;
			rect = new Rectangle(x, y, width-1, height-1);

			g.DrawString(buf, f, brush, rect, format);
			//DrawText(hdc, buf, strlen(buf), &r, type);		
		}

		public static void WinDrawText(Graphics g, int x, int y, int width, int height, String buf, Color tcolor, Color bcolor, Font f, StringFormat format)
		{
			Rectangle	rect = new Rectangle(x, y, width, height);
			Brush		brush = new SolidBrush(tcolor);
	
			//SetTextColor(hdc, tcolor);
			//SetBkColor(hdc, bcolor);	
			g.DrawString(buf, f, brush, rect, format);
		}

		public static void DrawText(Graphics g, string buf, Font f, Brush brush, RECT r, StringFormat format)
		{
			Rectangle	rect = new Rectangle(r.left, r.top, r.right-r.left+1, r.bottom-r.top+1);
	
			g.DrawString(buf, f, brush, rect, format);
		}

		public static void DrawTextClip(Graphics g, string buf, Font f, Brush brush, Rectangle r, StringFormat format)
		{
			g.SetClip(r);
			int x, y;
			if(format.Alignment == StringAlignment.Near)		x = r.Left;
			else if(format.Alignment == StringAlignment.Center)	x = r.Left+(r.Width+1)/2;
			else												x = r.Right;

			if(format.LineAlignment == StringAlignment.Near)		y = r.Top;
			else if(format.LineAlignment == StringAlignment.Center)	y = r.Top+(r.Height+1)/2;
			else													y = r.Bottom;

			g.DrawString(buf, f, brush, x, y, format);
			g.ResetClip();
		}

		public static void gnot(Graphics g, int x1, int y1, int x2, int y2, Color color, int alpha)
		{
			Color	color2 = Color.FromArgb(alpha%255, color);
			Brush	brush = new SolidBrush(color2);			
			//Rectangle rect = new Rectangle(x1, y1, x2-x1+1, y2-y1+1);
			
			g.FillRectangle(brush, x1, y1, x2-x1+1, y2-y1+1);
			
			//System.Windows.Forms.ControlPaint.FillReversibleRectangle(rect, color);			
		}

		public static void InvertRect(System.Drawing.Graphics g, int x1, int y1, int x2, int y2)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			int width = x2-x1+1;
			int height = y2-y1+1;

			Color	color = Color.FromArgb(128, Color.White);
			Brush	brush = new SolidBrush(color);

			g.FillRectangle(brush, x1, y1, width, height);
		}

		public static void InvertRect(System.Drawing.Graphics g, RECT r)
		{
			InvertRect(g, r.left, r.top, r.right, r.bottom);
		}

		
	}
}

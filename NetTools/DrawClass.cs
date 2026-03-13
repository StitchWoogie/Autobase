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

        public static void gcls(Graphics g, int x1, int y1, int x2, int y2, Brush brush)
        {
            g.FillRectangle(brush, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
        }

		public static void gcls(Graphics g, Rectangle rect, Color color)
		{
			gcls(g, rect.X, rect.Y, rect.X+rect.Width-1, rect.Y+rect.Height-1, color);
		}

		public static void gcls(Graphics g, RectangleF rect, Color color)
		{
			gcls(g, (int)rect.X, (int)rect.Y, (int)(rect.X+rect.Width-1), (int)(rect.Y+rect.Height-1), color);
		}

        //Analog Gauge용 25-02-04 hsjeong
        public static void gcls3(Graphics g, int x1, int y1, int width, int height, float fStartAngle, float fSweepAngle, double thickness, Brush brush)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (thickness >= 1.0)
            {


                g.FillPie(brush, x1, y1, width, height, fStartAngle, fSweepAngle);


                //using (Pen pen = new Pen(Color.Black, borderThickness))
                //{
                //    g.DrawPie(pen, x1, y1, width, height, startAngle, sweepAngle);
                //}
                return;
            }

            if (thickness <= 0) thickness = 0.01;

            // 내부 원 크기 계산
            int innerWidth = (int)(width * (1.0 - thickness));
            int innerHeight = (int)(height * (1.0 - thickness));


            // 1차로 내부 원, 2차로 외부 원을 계산할 때, 두께가 84이상이 되면 w,h가 0이 되는 경우 발생. 이 때, 0으로 나눠지게 되어 스튜디오 꺼짐.
            // 이를 방지하기 위하여 0이 되면 1로 고정시킴 25-02-07 hsjeong
            if (innerWidth == 0) innerWidth = 1;
            if (innerHeight == 0) innerHeight = 1;

            int marginX = (width - innerWidth) / 2;
            int marginY = (height - innerHeight) / 2;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.FillMode = FillMode.Winding;

                // 외부 호
                path.AddArc(x1, y1, width, height, fStartAngle, fSweepAngle);

                // 내부 호 (반대 방향)
                path.AddArc(x1 + marginX, y1 + marginY, innerWidth, innerHeight,
                           fStartAngle + fSweepAngle, -fSweepAngle);

                path.CloseFigure();

                g.FillPath(brush, path);


            }
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

        public static void PopBox2(Graphics g, int x1, int y1, int x2, int y2, Brush brush)
        {
            gcls(g, x1 + 1, y1 + 1, x2 - 1, y2 - 1, brush);
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

        public static void PushBox2(Graphics g, int x1, int y1, int x2, int y2, Brush brush)
        {
            gcls(g, x1, y1, x2, y2, brush);
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

            SafeException.SafeDrawString(g, buf, f, brush, rect, format);
					
			brush = Brushes.Black;
			rect = new Rectangle(x, y, width-1, height-1);

            SafeException.SafeDrawString(g, buf, f, brush, rect, format);
		}

		public static void WinDrawText(Graphics g, int x, int y, int width, int height, String buf, Color tcolor, Color bcolor, Font f, StringFormat format)
		{
			Rectangle	rect = new Rectangle(x, y, width, height);
			Brush		brush = new SolidBrush(tcolor);

            SafeException.SafeDrawString(g, buf, f, brush, rect, format);
		}

		public static void DrawText(Graphics g, string buf, Font f, Brush brush, RECT r, StringFormat format)
		{
			Rectangle	rect = new Rectangle(r.left, r.top, r.right-r.left+1, r.bottom-r.top+1);

            SafeException.SafeDrawString(g, buf, f, brush, rect, format);
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

            SafeException.SafeDrawString(g, buf, f, brush, x, y, format);

			g.ResetClip();
		}

		public static void gnot(Graphics g, int x1, int y1, int x2, int y2, Color color, int alpha)
		{
			Color	color2 = Color.FromArgb(alpha%255, color);
			Brush	brush = new SolidBrush(color2);			
			//Rectangle rect = new Rectangle(x1, y1, x2-x1+1, y2-y1+1);
			
			g.FillRectangle(brush, x1, y1, x2-x1+1, y2-y1+1);
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

using System;
using AutoLib;
using NetTools.OldDefine;
using System.Drawing;
using NetTools;
using System.Drawing.Drawing2D;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsRoundRectangle
	{
		public int round_x;
		public int round_y;
	}

	/// <summary>
	/// Summary description for ObjectRoundRectangle.
	/// </summary>
	[Serializable]
	public class ObjectRoundRectangle : ObjectExpand
	{
		public ObjectArgsRoundRectangle objArgs;

		public ObjectArgsRoundRectangle ObjectArgs 
		{
			set 
			{
				objArgs = value;
			}
			get 
			{
				return objArgs;
			}
		}

        public ObjectRoundRectangle(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick, ObjectArgsRoundRectangle args)
			: base(ocp, rect, eid, null, general)
		{
			enumObjectType = EnumObjectType.RoundRectangle;
			objArgs = args;

			SetLineColor(lcolor);
			SetFillColor(fcolor);		
			nLineOption = loption;
			SetBorderThick(lthick);
		}

        /*
		void RoundRectangle(Graphics g,Pen pen,int x1, int y1, int x2, int y2, int width, int height)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			if(x1 == x2)	return;
			if(y1 == y2)	return;


			if(width > (x2-x1+1))	width = (x2-x1+1);
			if(height > (y2-y1+1))	height = (y2-y1+1);

			int rx = width/2;
			int ry = height/2;

			width = rx*2;
			height = ry*2;

			if(width == 0)	 width = 1;
			if(height == 0)	 height = 1;

			g.DrawLine(pen,x1+rx,y1,x2-rx,y1);
			g.DrawArc(pen,x2-width,y1,width,height,270,90);
			g.DrawLine(pen,x2,y1+ry,x2,y2-ry);
			g.DrawArc(pen,x2-width,y2-height,width,height,0,90);
			g.DrawLine(pen,x1+rx,y2,x2-rx,y2);
			g.DrawArc(pen,x1, y2-height, width,height,90,90);
			g.DrawLine(pen,x1,y1+ry,x1,y2-ry);
			g.DrawArc(pen,x1,y1,width,height,180,90);
		}

		void FillRoundRectangle(Graphics g,Brush brush,int x1, int y1, int x2, int y2, int width, int height)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			if(x1 == x2)	return;
			if(y1 == y2)	return;


			if(width > (x2-x1+1))	width = (x2-x1+1);
			if(height > (y2-y1+1))	height = (y2-y1+1);

			int rx = width/2;
			int ry = height/2;

			width = rx*2;
			height = ry*2;

			if(width == 0)	 width = 1;
			if(height == 0)	 height = 1;
			
			g.FillPie(brush,x2-width,y1,width,height,270,90);
			g.FillPie(brush,x2-width,y2-height,width,height,0,90);
			g.FillPie(brush,x1, y2-height, width,height,90,90);
			g.FillPie(brush,x1,y1,width,height,180,90);
			g.FillRectangle(brush, x1+rx, y1, (x2-x1+1)-width, y2-y1+1);
			g.FillRectangle(brush, x1, y1+ry, x2-x1+1, (y2-y1+1)-height);
		}*/

        void ex(int x1, int y1, int x2, int y2, out Point[] polygon, out byte[] polyatr, int round_x, int round_y)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            bool rect_flag = false;

            int width = x2 - x1;
            int height = y2 - y1;

            // 라운드가 크기보다 클때는 적당히 잘라준다.
            if (round_x > width) round_x = width;
            if (round_y > height) round_y = height;

            round_x /= 2;
            round_y /= 2;

            int xg = (int)(round_x * 0.56);
            int yg = (int)(round_y * 0.56);
            int cx = (x2 - x1) / 2 + x1;
            int cy = (y2 - y1) / 2 + y1;

            int hap = 17;
            polygon = new Point[hap];
            polyatr = new byte[hap];

            hap = 0;
            polyatr[hap] = (byte)PathPointType.Start;
            polygon[hap].X = x1 + round_x;
            polygon[hap].Y = y1;
            hap++;

            polyatr[hap] = (byte)PathPointType.Line;
            polygon[hap].X = x2 - round_x;
            polygon[hap].Y = y1;
            hap++;

            if (!rect_flag)
            {
                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x2 - round_x + xg;
                polygon[hap].Y = y1;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x2;
                polygon[hap].Y = y1 + round_y - yg;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x2;
                polygon[hap].Y = y1 + round_y;
                hap++;
            }

            polyatr[hap] = (byte)PathPointType.Line;
            polygon[hap].X = x2;
            polygon[hap].Y = y2 - round_y;
            hap++;

            if (!rect_flag)
            {
                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x2;
                polygon[hap].Y = y2 - round_y + yg;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x2 - round_x + xg;
                polygon[hap].Y = y2;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x2 - round_x;
                polygon[hap].Y = y2;
                hap++;
            }

            polyatr[hap] = (byte)PathPointType.Line;
            polygon[hap].X = x1 + round_x;
            polygon[hap].Y = y2;
            hap++;

            if (!rect_flag)
            {
                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x1 + round_x - xg;
                polygon[hap].Y = y2;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x1;
                polygon[hap].Y = y2 - round_y + yg;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x1;
                polygon[hap].Y = y2 - round_y;
                hap++;
            }

            polyatr[hap] = (byte)PathPointType.Line;
            polygon[hap].X = x1;
                polygon[hap].Y = y1 + round_y;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x1;
                polygon[hap].Y = y1 + round_y - yg;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier;
                polygon[hap].X = x1 + round_x - xg;
                polygon[hap].Y = y1;
                hap++;

                polyatr[hap] = (byte)PathPointType.Bezier | (byte)PathPointType.CloseSubpath;
                polygon[hap].X = x1 + round_x;
                polygon[hap].Y = y1;
                hap++;
        }

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            Point[] polygon;
            byte[] polyatr;
            int round_x, round_y;

            round_x = GetViewSizeX(objArgs.round_x);    // 100%가 아닐때 round가 맞지 않아서 GetViewSizeX로 보정했다. 2010-11-11
            round_y = GetViewSizeY(objArgs.round_y);

            ex(x1, y1, x2, y2, out polygon, out polyatr, round_x, round_y);
            //ex(x1, y1, x2, y2, out polygon, out polyatr, objArgs.round_x, objArgs.round_y);

            GraphicsPath path = new GraphicsPath(polygon, polyatr);

            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            if (nFillOption != 0)
            {
                Brush brush = ObjectRectangle.MakePublicBrush(RunColorFill, x1, y1, x2, y2);
                g.FillPath(brush, path);
            }

            if (nLineOption != 0)
            {
                Pen pen = new Pen(RunColorLine, bthick);
                pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);

                try
                {
                    g.DrawPath(pen, path);          // Start부터 Close까지 포인트의 이동이 전혀 없는 경우 메모리 부족 Exception이 발생한다. (FillPath는 상관없다.) 레이어에서 작은 Preview이미지를 그릴 때 발생한다.
                }
                catch
                {

                }
            }
            
            /*
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);
	
			if(nLineOption == 0 && nFillOption == 0) 
			{
				nLineOption = 1;
				nFillOption = 1;
			}

			Brush brush;
			Pen pen;

			if(nFillOption == 1)
				brush = new SolidBrush(RunColorFill);
			else 
			{
				brush = null;
			}

			if(nLineOption == 0) 
			{
				pen = null;
			}
			else 
			{ 
				pen = new Pen(RunColorLine, bthick);
				pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;	// 이 부분이 없으면 이음새가 깨진다.
			}

			int view_sizex = this.GetViewSizeX(objArgs.round_x);
			int view_sizey = this.GetViewSizeY(objArgs.round_y);
	
			if(nFillOption != 0) 
				FillRoundRectangle(g, brush, x1, y1, x2, y2, view_sizex, view_sizey);
			if(nLineOption != 0) 
			{
				pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);
				RoundRectangle(g, pen, x1, y1, x2, y2, view_sizex, view_sizey);
			}*/
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			SaveObjectItem.LineColor(writer, GetLineColor());
			SaveObjectItem.FillColor(writer, GetFillColor());
			SaveObjectItem.LineThick(writer, GetBorderThick());
			SaveObjectItem.LineOption(writer, nLineOption);
			SaveObjectItem.FillOption(writer, nFillOption);
			writer.WriteLine("\tStringOption,{0},{1},", objArgs.round_x, objArgs.round_y);
		}

        public override void EditRotateRight(int nx1, int ny1, int nx2, int ny2)
        {
            Tools.Temp(ref objArgs.round_x, ref objArgs.round_y);

            base.EditRotateRight(nx1, ny1, nx2, ny2);
        }

        public override void EditRotateLeft(int nx1, int ny1, int nx2, int ny2)
        {
            Tools.Temp(ref objArgs.round_x, ref objArgs.round_y);

            base.EditRotateLeft(nx1, ny1, nx2, ny2);
        }
	}
}



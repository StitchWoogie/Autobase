using System;
using System.Drawing;
using NetTools.OldDefine;
using NetTools;
using AutoLib;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsRectangle
	{
		public int nBorderStyle = 0;	// 9.0.3 부터 생긴 요소 0=일반선(직선,점선,일점쇄선 등...), 1 = Pop, 2 = Push, ...
										// 이전에 wLinrOption에서 사용하는 것을 사각형의 경우만 pop push만 존재하므로 border스타일을 따로 만들었다.
	}

	/// <summary>
	/// Summary description for ObjectRectangle.
	/// </summary>
	[Serializable]
	public class ObjectRectangle : ObjectExpand
	{
		public ObjectArgsRectangle objArgs;

		public ObjectArgsRectangle ObjectArgs 
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
 
		public ObjectRectangle(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick, ObjectArgsRectangle args)
			: base(ocp, rect, eid, null, general)
		{
			enumObjectType = EnumObjectType.Rectangle;
			objArgs = args;

			SetLineColor(lcolor);
			SetFillColor(fcolor);		
			nLineOption = loption;
			SetBorderThick(lthick);
		}

		public static System.Drawing.Drawing2D.DashStyle GetDashStyle(int option)
		{
			if(option == 1)		return System.Drawing.Drawing2D.DashStyle.Solid;
			if(option == 2)		return System.Drawing.Drawing2D.DashStyle.Dot;
			if(option == 3)		return System.Drawing.Drawing2D.DashStyle.Dash;
			if(option == 4)		return System.Drawing.Drawing2D.DashStyle.DashDot;
			if(option == 5)		return System.Drawing.Drawing2D.DashStyle.DashDotDot;
			return System.Drawing.Drawing2D.DashStyle.Solid;
		}

		void DrawRectLine(Graphics g, int x1, int y1, int x2, int y2, int thick, Color color)
		{
			/*
			 * 
			int minus;
			minus = thick/2;

			x1 -= minus;
			y1 -= minus;
			x2 -= minus;
			y2 -= minus;

			if(thick > 0)	thick--;

			DrawClass.gcls(g, x1, y1, x1+thick, y2, color);
			DrawClass.gcls(g, x1, y1, x2, y1+thick, color);
			DrawClass.gcls(g, x1, y2, x2, y2+thick, color);
			DrawClass.gcls(g, x2, y1, x2+thick, y2+thick, color);
			*/

			using (Pen pen = new Pen(color, thick))
			{
				pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);
				g.DrawRectangle(pen, x1, y1, x2-x1, y2-y1);
			}
		}

        // 기본 컬러 블랜드를 만든다.
        public static ColorBlend MakeColorBlend(List<BrushGradientStop> stops)
        {
            bool add_first = false; // offset이 0 으로 시작하지 않는 경우 0을 추가한다.
            bool add_last = false;  // offset이 1 로 끝나지 않는 경우 1을 추가한다.
            int pos_small = 0;
            int pos_big = 0;

            for (int i = 0; i < stops.Count; i++)
            {
                if (stops[i].offset < stops[pos_small].offset) pos_small = i;
                if (stops[i].offset > stops[pos_big].offset)   pos_big = i;
            }

            if (stops.Count < 1)
            {
                add_first = true;
                add_last = true;
            }
            else
            {
                if (stops[pos_small].offset != 0)   add_first = true;
                if (stops[pos_big].offset != 1)     add_last = true;
            }

            int count = stops.Count;
            if (add_first) count++;
            if (add_last) count++;

            ColorBlend blend = new ColorBlend(count);
            blend.Colors = new Color[count];
            blend.Positions = new float[count];

            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                if (i == 0 && add_first)
                {
                    if (pos_small < stops.Count)
                    {
                        blend.Colors[i] = stops[pos_small].color;
                    }
                    else
                    {
                        blend.Colors[i] = Color.White;
                    }

                    blend.Positions[i] = 0;
                }
                else if (i == count - 1 && add_last)
                {
                    if (pos_big < stops.Count)
                    {
                        blend.Colors[i] = stops[pos_big].color;
                    }
                    else
                    {
                        blend.Colors[i] = Color.Black;
                    }

                    blend.Positions[i] = 1;
                }
                else
                {
                    blend.Colors[i] = stops[pos].color;
                    blend.Positions[i] = stops[pos].offset;
                    pos++;
                }
            }

            return blend;
        }

        // Pad 모드는 사각형을 지나서 모두 포함하는 위치의 좌표값을 찾는다.
        public static ColorBlend MakeColorBlendPadMode(List<BrushGradientStop> stops, PointF pStart, PointF pEnd, out float sx, out float sy, out float ex, out float ey)
        {
            bool add_first = false;  // offset이 0 으로 시작하지 않는 경우 0을 추가한다.
            bool add_last = false;   // offset이 1 로 끝나지 않는 경우 1을 추가한다.
            int pos_small = 0;
            int pos_big = 0;

            float w = pEnd.X - pStart.X;
            float h = pEnd.Y - pStart.Y;

            float gabs = 0;
            float gabe = 0;
            float gabm = 1;

            sx = 0;
            sy = 0;
            ex = 0;
            ey = 0;

            if (h == 0) // 가로 방향
            {
                gabm = Math.Abs(w);
                if (w > 0)
                {
                    gabs = pStart.X;
                    gabe = 1-pEnd.X;
                    sx = 0;
                    sy = 0;
                    ex = 1;
                    ey = 0;
                }
                else 
                {
                    gabe = pEnd.X;
                    gabs = 1 - pStart.X;
                    sx = 1;
                    sy = 0;
                    ex = 0;
                    ey = 0;
                }
            }
            else if (w == 0)
            {
                gabm = Math.Abs(h);
                if (h > 0)
                {
                    gabs = pStart.Y;
                    gabe = 1 - pEnd.Y;
                    sx = 0;
                    sy = 0;
                    ex = 0;
                    ey = 1;
                }
                else
                {
                    gabe = pEnd.Y;
                    gabs = 1 - pStart.Y;
                    sx = 0;
                    sy = 1;
                    ex = 0;
                    ey = 0;
                }
            }
            else // 기울기가 있는 경우
            {
                float xx = 0;
                float yy = 0;

                // 계산하기 좋게 바른 오른쪽 아래 방향 좌표로 바꾼다.
                float psx, psy, pex, pey;

                if (w < 0)
                {
                    psx = 1 - pStart.X;
                    pex = 1 - pEnd.X;
                }
                else
                {
                    psx = pStart.X;
                    pex = pEnd.X;
                }

                if (h < 0)
                {
                    psy = 1 - pStart.Y;
                    pey = 1 - pEnd.Y;
                }
                else
                {
                    psy = pStart.Y;
                    pey = pEnd.Y;
                }

                xx = Math.Abs(h) * psx / Math.Abs(w);
                yy = Math.Abs(w) * psy / Math.Abs(h);

                if (psx - yy < psy - xx)
                {
                    xx = psx - yy;
                    yy = 0;
                }
                else
                {
                    yy = psy - xx;
                    xx = 0;
                }
                sx = xx;
                sy = yy;

                xx = Math.Abs(h) * (1 - pex) / Math.Abs(w);
                yy = Math.Abs(w) * (1 - pey) / Math.Abs(h);

                if (pex + yy > pey + xx)
                {
                    xx = pex + yy;
                    yy = 1;
                }
                else
                {
                    yy = pey + xx;
                    xx = 1;
                }
                ex = xx;
                ey = yy;

                if (Math.Abs(ex - sx) > Math.Abs(ey - sy))
                {
                    gabs = psx - sx;
                    gabm = Math.Abs(w);
                    gabe = ex - pex;
                }
                else
                {
                    gabs = psy - sy;
                    gabm = Math.Abs(h);
                    gabe = ey - pey;
                }

                // 계산한 좌표를 원래의 좌표로 바꾼다.

                if (w < 0)
                {
                    sx = sx+1;
                    ex = 1-ex;
                }
                if (h < 0)
                {
                    sy = sy+1;
                    ey = 1-ey;
                }

            }
            
            for (int i = 0; i < stops.Count; i++)
            {
                if (stops[i].offset < stops[pos_small].offset) pos_small = i;
                if (stops[i].offset > stops[pos_big].offset) pos_big = i;
            }

            if (stops.Count < 1)
            {
                add_first = true;
                add_last = true;
            }
            else
            {
                if (stops[pos_small].offset != 0) add_first = true;
                if (stops[pos_big].offset != 1)   add_last = true;
            }

            int count = stops.Count+2;  // 처음과 끝을 항상 하나씩 붙여준다. 
            if (add_first) count++;
            if (add_last) count++;

            ColorBlend blend = new ColorBlend(count);
            blend.Colors = new Color[count];
            blend.Positions = new float[count];

            int pos = 0;
            float total_gab = gabs + gabm + gabe;
            float ratio = gabm/total_gab;

            for (int i = 0; i < count; i++)
            {
                if (i == 0) // 맨처음
                {
                    if (pos_small < stops.Count)
                    {
                        blend.Colors[i] = stops[pos_small].color;
                    }
                    else
                    {
                        blend.Colors[i] = Color.White;
                    }

                    blend.Positions[i] = 0;
                }
                else if (i == 1 && add_first)
                {
                    if (pos_small < stops.Count)
                    {
                        blend.Colors[i] = stops[pos_small].color;
                    }
                    else
                    {
                        blend.Colors[i] = Color.White;
                    }

                    blend.Positions[i] = (gabs) / total_gab;
                }
                else if (i == count - 2 && add_last)
                {
                    if (pos_big < stops.Count)
                    {
                        blend.Colors[i] = stops[pos_big].color;
                    }
                    else
                    {
                        blend.Colors[i] = Color.Black;
                    }

                    blend.Positions[i] = (gabs + gabm) / total_gab;
                }
                else if (i == count - 1)    // 마지막 버퍼
                {
                    if (pos_big < stops.Count)
                    {
                        blend.Colors[i] = stops[pos_big].color;
                    }
                    else
                    {
                        blend.Colors[i] = Color.Black;
                    }

                    blend.Positions[i] = 1;
                }
                else
                {
                    blend.Colors[i] = stops[pos].color;
                    blend.Positions[i] = (gabs + stops[pos].offset*gabm) / total_gab;
                    pos++;
                }
            }

            return blend;
        }

        public static Brush MakePublicBrush(BrushPublic pub, int x1, int y1, int x2, int y2)
        {
            if (pub.brush_type == 1)
            {
                return new SolidBrush(pub.basic_color);
            }
            else if(pub.brush_type == 2) {
                BrushLinearGradient bp = (BrushLinearGradient)pub;

                if (bp.spread_method == 0)  // pad mod
                {
                    float sx, sy, ex, ey;

                    ColorBlend blend = MakeColorBlendPadMode(bp.stops, bp.pStart, bp.pEnd, out sx, out sy, out ex, out ey);

                    int w = x2 - x1;
                    int h = y2 - y1;

                    int xx1 = (int)(x1 + w * sx);
                    int yy1 = (int)(y1 + h * sy);
                    int xx2 = (int)(x1 + w * ex);
                    int yy2 = (int)(y1 + h * ey);

                    if (bp.pStart.X == bp.pEnd.X)   // Y1, Y2 그라데이션인 경우 시작과 끝에 Tile이라서 줄이 하나 생기는 경우가 있으므로 범위를 1칸씩 크게 잡는다.
                    {
                        yy1--;
                        yy2++;
                    }
                    if (bp.pStart.Y == bp.pEnd.Y)   // X1, X2 그라데이션인 경우 시작과 끝에 Tile이라서 줄이 하나 생기는 경우가 있으므로 범위를 1칸씩 크게 잡는다.
                    {
                        xx1--;
                        xx2++;
                    }

                    if (xx1 == xx2 && yy1 == yy2)
                    {
                        xx2 = xx1 + 1;
                    }

                    // Pad 모드가 없으므로 사이즈는 전체로 하고 ColorBlend에서 조절한다.
                    LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new PointF(xx1, yy1), new PointF(xx2, yy2), Color.Blue, Color.Red);
                    
                    brush.InterpolationColors = blend;

                    brush.WrapMode = WrapMode.Tile; 

                    return brush;
                }
                else
                {
                    int w = x2 - x1 + 1;
                    int h = y2 - y1 + 1;

                    int xx1 = (int)(x1 + w * bp.pStart.X);
                    int yy1 = (int)(y1 + h * bp.pStart.Y);
                    int xx2 = (int)(x1 + w * bp.pEnd.X);
                    int yy2 = (int)(y1 + h * bp.pEnd.Y);

                    if (xx1 == xx2 && yy1 == yy2)
                    {
                        xx2 = xx1 + 1;
                    }

                    LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new PointF(xx1, yy1), new PointF(xx2, yy2), Color.Blue, Color.Red);
                    ColorBlend blend = MakeColorBlend(bp.stops);
                    brush.InterpolationColors = blend;

                    if (bp.spread_method == 1)
                    {
                        brush.WrapMode = WrapMode.TileFlipXY;
                    }
                    else
                    {
                        brush.WrapMode = WrapMode.Tile; // Repeat mode
                    }

                    return brush;
                }
            }
            else {
                //
                return new SolidBrush(Color.Transparent);  // 새 인스턴스 → Dispose 안전
                //return Brushes.Transparent; // ← static 공유 인스턴스 using 사용 시 위험.
            }
        }

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);
	
			if(nLineOption == 0 && nFillOption == 0) 
			{
				nLineOption = 1;
				nFillOption = 1;
			}

			if(nFillOption > 0) 
			{
                Brush brush = ObjectRectangle.MakePublicBrush(RunColorFill, x1, y1, x2, y2);

                if(nLineOption == 0)
                    DrawClass.gcls(g, x1, y1, x2, y2, brush);
                else
                    g.FillRectangle(brush, x1, y1, x2-x1, y2-y1);   // 고품질 모드일 때는 정확한 크기로 그리면 오른쪽/아래쪽으로 더 그리므로 테두리가 있는 경우는 -1 해서 그린다. 2010.11-11
			}

			if(nLineOption == 0) 
			{

			}
			else 
			{
				if(objArgs.nBorderStyle == 0) 
				{
					DrawRectLine(g, x1, y1, x2, y2, bthick, RunColorLine);
				}
				else if(objArgs.nBorderStyle == 1) 
				{
					DrawClass.PopRectangle(g, x1, y1, x2, y2);
				}
				else if(objArgs.nBorderStyle == 2) 
				{
					DrawClass.PopRectangle2(g, x1, y1, x2, y2);
				}
				else if(objArgs.nBorderStyle == 3) 
				{
					DrawClass.PushRectangle(g, x1, y1, x2, y2);
				}
				else if(objArgs.nBorderStyle == 4) 
				{
					DrawClass.PushRectangle2(g, x1, y1, x2, y2);
				}
				else 
				{
					DrawRectLine(g, x1, y1, x2, y2, bthick, RunColorLine);
				}
			}
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			SaveObjectItem.LineColor(writer, GetLineColor());
			SaveObjectItem.FillColor(writer, GetFillColor());
			SaveObjectItem.LineThick(writer, GetBorderThick());
			SaveObjectItem.LineOption(writer, nLineOption);
			SaveObjectItem.FillOption(writer, nFillOption);
			writer.WriteLine("\tStringOption,{0},", objArgs.nBorderStyle);
		}

        
	}
}


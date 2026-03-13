using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NetTools.OldDefine;
using System.Collections.Generic;

namespace SilverlightGraphicModule
{
    //[Serializable]
    public class ObjectArgsRectangle
    {
        public int nBorderStyle = 0;	// 9.0.3 부터 생긴 요소 0=일반선(직선,점선,일점쇄선 등...), 1 = Pop, 2 = Push, ...
        // 이전에 wLineOption에서 사용하는 것을 사각형의 경우만 pop push만 존재하므로 border스타일을 따로 만들었다.
    }

    /// <summary>
    /// Summary description for ObjectRectangle.
    /// </summary>
    //[Serializable]
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

        // 기본 컬러 블랜드를 만든다.
        public static GradientStopCollection MakeColorBlend(List<BrushGradientStop> stops)
        {
            bool add_first = false; // offset이 0 으로 시작하지 않는 경우 0을 추가한다.
            bool add_last = false;  // offset이 1 로 끝나지 않는 경우 1을 추가한다.
            int pos_small = 0;
            int pos_big = 0;

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
                if (stops[pos_big].offset != 1) add_last = true;
            }

            int count = stops.Count;

            GradientStopCollection blend = new GradientStopCollection();
            GradientStop stop;

            if (add_first)
            {
                stop = new GradientStop();
                
                stop.Offset = 0;

                if (pos_small < stops.Count)
                {
                    stop.Color = stops[pos_small].color;
                }
                else
                {
                    stop.Color = Colors.White;
                }
                blend.Add(stop);
            }
            

            for (int i = 0; i < count; i++)
            {
                stop = new GradientStop();
                stop.Color = stops[i].color;
                stop.Offset = stops[i].offset;

                blend.Add(stop);
            }

            if (add_last)
            {
                stop = new GradientStop();

                stop.Offset = 1;

                if (pos_big < stops.Count)
                {
                    stop.Color = stops[pos_big].color;
                }
                else
                {
                    stop.Color = Colors.Black;
                }
                blend.Add(stop);
            }

            return blend;
        }

        public static Brush MakePublicBrush(BrushPublic pub/*, int x1, int y1, int x2, int y2*/)
        {
            if (pub.brush_type == 1)
            {
                return new SolidColorBrush(pub.basic_color);
            }
            else if (pub.brush_type == 2)
            {
                BrushLinearGradient bp = (BrushLinearGradient)pub;

                LinearGradientBrush brush = new LinearGradientBrush();
                brush.StartPoint = bp.pStart;
                brush.EndPoint = bp.pEnd;
                brush.GradientStops = MakeColorBlend(bp.stops);
                if (bp.spread_method == 1)      brush.SpreadMethod = GradientSpreadMethod.Reflect;
                else if (bp.spread_method == 2) brush.SpreadMethod = GradientSpreadMethod.Repeat;
                else                            brush.SpreadMethod = GradientSpreadMethod.Pad;

                return brush;
                /*
                
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
                }*/
            }
            else
            {
                return null;// Brushes.Transparent;
            }
        }

        public ObjectRectangle(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick, ObjectArgsRectangle args)
            : base(ocp, rect, eid, null, general)
        {
            enumObjectType = EnumObjectType.Rectangle;
            objArgs = args;

            SetLineColor(lcolor);
            SetFillColor(fcolor);
            nLineOption = loption;
            SetBorderThick(lthick);

            FrameworkElement child;
            Rectangle r = new Rectangle();

            child = r;

            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            if (nLineOption == 0)
            {
                r.Stroke = null;
            }
            else
            {
                r.Stroke = new SolidColorBrush(RunColorLine);
                r.StrokeThickness = nRunThickLine;

                if (objArgs.nBorderStyle == 1)
                {
                    Grid grid = new Grid();
                    ObjectAnalogString.AddLinePopBox2(grid);

                    if (nFillOption == 0)
                    {
                        grid.Background = null;
                    }
                    else
                    {
                        grid.Background = ObjectRectangle.MakePublicBrush(RunColorFill);
                    }

                    child = grid;
                }
                else if (objArgs.nBorderStyle == 2)
                {
                    Grid grid = new Grid();
                    ObjectAnalogString.AddLinePopBox2(grid);

                    if (nFillOption == 0)
                    {
                        grid.Background = null;
                    }
                    else
                    {
                        grid.Background = ObjectRectangle.MakePublicBrush(RunColorFill);
                    }

                    child = grid;
                }
                else if (objArgs.nBorderStyle == 3)
                {
                    //DrawClass.PushRectangle(g, x1, y1, x2, y2);
                    Grid grid = new Grid();
                    ObjectAnalogString.AddLinePushBox2(grid);

                    if (nFillOption == 0)
                    {
                        grid.Background = null;
                    }
                    else
                    {
                        grid.Background = ObjectRectangle.MakePublicBrush(RunColorFill);
                    }

                    child = grid;
                }
                else if (objArgs.nBorderStyle == 4)
                {
                    Grid grid = new Grid();
                    ObjectAnalogString.AddLinePushBox2(grid);

                    if (nFillOption == 0)
                    {
                        grid.Background = null;
                    }
                    else
                    {
                        grid.Background = ObjectRectangle.MakePublicBrush(RunColorFill);
                    }
                    
                    child = grid;
                }
                else
                {
                    r.StrokeDashArray = ObjectRectangle.GetDashStyle(nLineOption);
                }
            }

            if (nFillOption == 0)
            {
                r.Fill = null;
            }
            else
            {
                r.Fill = ObjectRectangle.MakePublicBrush(RunColorFill);
            }

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);

            MoveShape();
        }

        public static  DoubleCollection GetDashStyle(int option)
        {
            if (option == 2)
            {
                DoubleCollection col = new DoubleCollection();
                col.Add(1);
                return col;
            }
            else if (option == 3)
            {
                DoubleCollection col = new DoubleCollection();
                col.Add(3);
                col.Add(1);
                return col;
            }
            else if (option == 4)
            {
                DoubleCollection col = new DoubleCollection();
                col.Add(3);
                col.Add(1);
                col.Add(1);
                col.Add(1);
                return col;
            }
            else if (option == 5)
            {
                DoubleCollection col = new DoubleCollection();
                col.Add(3);
                col.Add(1);
                col.Add(1);
                col.Add(1);
                col.Add(1);
                col.Add(1);
                return col;
            }
            return null;
            /*
            if (option == 1) return System.Drawing.Drawing2D.DashStyle.Solid;
            if (option == 2) return System.Drawing.Drawing2D.DashStyle.Dot;
            if (option == 3) return System.Drawing.Drawing2D.DashStyle.Dash;
            if (option == 4) return System.Drawing.Drawing2D.DashStyle.DashDot;
            if (option == 5) return System.Drawing.Drawing2D.DashStyle.DashDotDot;
            return System.Drawing.Drawing2D.DashStyle.Solid;
             */
        }

        protected override void LineColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            if (child.GetType() == typeof(Rectangle))
                ((Rectangle)child).Stroke = new SolidColorBrush(color);

        }

        protected override void FillColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            if(child.GetType() == typeof(Grid))
                ((Grid)child).Background = new SolidColorBrush(color);
            else if (child.GetType() == typeof(Rectangle))
                ((Rectangle)child).Fill = new SolidColorBrush(color);

        }

        /*
        void DrawRectLine(Graphics g, int x1, int y1, int x2, int y2, int thick, Color color)
        {
            Pen pen = new Pen(color, thick);

            pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);

            g.DrawRectangle(pen, x1, y1, x2 - x1, y2 - y1);
        }

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            if (nFillOption > 0)
            {
                DrawClass.gcls(g, x1, y1, x2, y2, RunColorFill);
            }

            if (nLineOption == 0)
            {

            }
            else
            {
                if (objArgs.nBorderStyle == 0)
                {
                    DrawRectLine(g, x1, y1, x2, y2, bthick, RunColorLine);
                }
                else if (objArgs.nBorderStyle == 1)
                {
                    DrawClass.PopRectangle(g, x1, y1, x2, y2);
                }
                else if (objArgs.nBorderStyle == 2)
                {
                    DrawClass.PopRectangle2(g, x1, y1, x2, y2);
                }
                else if (objArgs.nBorderStyle == 3)
                {
                    DrawClass.PushRectangle(g, x1, y1, x2, y2);
                }
                else if (objArgs.nBorderStyle == 4)
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
        }*/

        protected override int ApplyGabByThick(int thick)
        {
            return thick / 2;
        }

        protected override void LineThickChanged(double thick)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            if (child.GetType() == typeof(Rectangle))
                ((Rectangle)child).StrokeThickness = thick;
        }
    }
}

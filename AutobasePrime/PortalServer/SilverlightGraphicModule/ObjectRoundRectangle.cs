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

namespace SilverlightGraphicModule
{
    //[Serializable]
    public class ObjectArgsRoundRectangle
    {
        public int round_x;
        public int round_y;
    }

    /// <summary>
    /// Summary description for ObjectRoundRectangle.
    /// </summary>
    //[Serializable]
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

        public ObjectRoundRectangle(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick, ObjectArgsRoundRectangle args)
            : base(ocp, rect, eid, null, general)
        {
            enumObjectType = EnumObjectType.RoundRectangle;
            objArgs = args;

            SetLineColor(lcolor);
            SetFillColor(fcolor);
            nLineOption = loption;
            SetBorderThick(lthick);

            Rectangle child = new Rectangle();

            child.RadiusX = args.round_x/2.0;   
            child.RadiusY = args.round_y/2.0;

            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            if (nLineOption == 0)
            {
                child.Stroke = null;
            }
            else
            {
                child.Stroke = new SolidColorBrush(RunColorLine);
                child.StrokeThickness = nRunThickLine;
                child.StrokeDashArray = ObjectRectangle.GetDashStyle(nLineOption);
            }

            if (nFillOption == 0)
            {
                child.Fill = null;
            }
            else
            {
                child.Fill = ObjectRectangle.MakePublicBrush(RunColorFill);
            }

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);

            MoveShape();
        }

        /*
        void RoundRectangle(Graphics g, Pen pen, int x1, int y1, int x2, int y2, int width, int height)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (x1 == x2) return;
            if (y1 == y2) return;


            if (width > (x2 - x1 + 1)) width = (x2 - x1 + 1);
            if (height > (y2 - y1 + 1)) height = (y2 - y1 + 1);

            int rx = width / 2;
            int ry = height / 2;

            width = rx * 2;
            height = ry * 2;

            if (width == 0) width = 1;
            if (height == 0) height = 1;

            g.DrawLine(pen, x1 + rx, y1, x2 - rx, y1);
            g.DrawArc(pen, x2 - width, y1, width, height, 270, 90);
            g.DrawLine(pen, x2, y1 + ry, x2, y2 - ry);
            g.DrawArc(pen, x2 - width, y2 - height, width, height, 0, 90);
            g.DrawLine(pen, x1 + rx, y2, x2 - rx, y2);
            g.DrawArc(pen, x1, y2 - height, width, height, 90, 90);
            g.DrawLine(pen, x1, y1 + ry, x1, y2 - ry);
            g.DrawArc(pen, x1, y1, width, height, 180, 90);
        }

        void FillRoundRectangle(Graphics g, Brush brush, int x1, int y1, int x2, int y2, int width, int height)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (x1 == x2) return;
            if (y1 == y2) return;


            if (width > (x2 - x1 + 1)) width = (x2 - x1 + 1);
            if (height > (y2 - y1 + 1)) height = (y2 - y1 + 1);

            int rx = width / 2;
            int ry = height / 2;

            width = rx * 2;
            height = ry * 2;

            if (width == 0) width = 1;
            if (height == 0) height = 1;

            g.FillPie(brush, x2 - width, y1, width, height, 270, 90);
            g.FillPie(brush, x2 - width, y2 - height, width, height, 0, 90);
            g.FillPie(brush, x1, y2 - height, width, height, 90, 90);
            g.FillPie(brush, x1, y1, width, height, 180, 90);
            g.FillRectangle(brush, x1 + rx, y1, (x2 - x1 + 1) - width, y2 - y1 + 1);
            g.FillRectangle(brush, x1, y1 + ry, x2 - x1 + 1, (y2 - y1 + 1) - height);
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

            Brush brush;
            Pen pen;

            if (nFillOption == 1)
                brush = new SolidBrush(RunColorFill);
            else
            {
                brush = null;
            }

            if (nLineOption == 0)
            {
                pen = null;
            }
            else
            {
                pen = new Pen(RunColorLine, bthick);
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;	// 이 부분이 없으면 이음새가 깨진다.
            }

            //RectangleF r = new RectangleF(x1, y1, x2-x1+1, y2-y1+1);
            //r.
            int view_sizex = this.GetViewSizeX(objArgs.round_x);
            int view_sizey = this.GetViewSizeY(objArgs.round_y);

            if (nFillOption != 0)
                FillRoundRectangle(g, brush, x1, y1, x2, y2, view_sizex, view_sizey);
            if (nLineOption != 0)
            {
                pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);
                RoundRectangle(g, pen, x1, y1, x2, y2, view_sizex, view_sizey);
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.LineColor(writer, GetLineColor());
            SaveObjectItem.FillColor(writer, GetFillColor());
            SaveObjectItem.LineThick(writer, GetBorderThick());
            SaveObjectItem.LineOption(writer, nLineOption);
            SaveObjectItem.FillOption(writer, nFillOption);
            writer.WriteLine("\tStringOption,{0},{1},", objArgs.round_x, objArgs.round_y);
        }*/

        protected override int ApplyGabByThick(int thick)
        {
            return thick / 2;
        }

        protected override void LineThickChanged(double thick)
        {
            Rectangle child = (Rectangle)GetShapeOriginal();

            if (child == null) return;

            child.StrokeThickness = thick;
        }

        protected override void LineColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            if (child.GetType() == typeof(Rectangle))
                ((Rectangle)child).Stroke = new SolidColorBrush(color);

        }
    }
}

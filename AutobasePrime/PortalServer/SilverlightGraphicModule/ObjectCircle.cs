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
using AutoLibLocal;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectArgsCircle
    {
        public int type = 0;
        public float fStartAngle = 45;
        public float fSweepAngle = 270;
    }

    public class ObjectCircle : ObjectExpand
    {
        public ObjectArgsCircle objArgs;

        // 등록된 클래스 리스트
        //[NonSerialized]
        static public List<object> arrayClassList = new List<object>();

        public ObjectCircle(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick, ObjectArgsCircle args)
            : base(ocp, rect, eid, null, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Circle;

            if (args == null)
                objArgs = new ObjectArgsCircle();
            else
                objArgs = args;

            SetLineColor(lcolor);
            SetFillColor(fcolor);
            nLineOption = loption;
            SetBorderThick(lthick);

            Shape child = MakeChildObject();
            
            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);
            }
        }

        Shape MakeChildObject()
        {
            Shape child;

            if (objArgs.type == 0)  // 
            {
                child = new Ellipse();
            }
            else
            {

                Path path = new Path();

                int x1 = 0;
                int y1 = 0;
                int x2 = 0;
                int y2 = 0;

                GetViewZone(ref x1, ref y1, ref x2, ref y2);

                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                double cx = (x2 - x1) / 2.0;
                double cy = (y2 - y1) / 2.0;
                double rx = (x2 - x1) / 2.0;
                double ry = (y2 - y1) / 2.0;

                float startangle = objArgs.fStartAngle;
                float sweepangle = objArgs.fSweepAngle;

                double pointx, pointy;

                PathGeometry pg = new PathGeometry();
                PathFigure pf = new PathFigure();

                if (sweepangle < 0)
                    NetTools.MathLib.MathGetEllipsePoint(0, 0, rx, ry, startangle + Math.Abs(sweepangle), out pointx, out pointy);
                else
                    NetTools.MathLib.MathGetEllipsePoint(0, 0, rx, ry, startangle, out pointx, out pointy);

                if (objArgs.type == 1)
                {
                    pf.StartPoint = new Point(cx, cy);
                    LineSegment ls = new LineSegment();
                    ls.Point = new Point(pointx + cx, pointy + cy);
                    pf.Segments.Add(ls);
                }
                else
                {
                    pf.StartPoint = new Point(pointx + cx, pointy + cy);
                }

                ArcSegment arc = new ArcSegment();
                //if (objArgs.fSweepAngle < 0)
                //    arc.SweepDirection = SweepDirection.Counterclockwise;
                //else
                arc.SweepDirection = SweepDirection.Clockwise;
                arc.RotationAngle = 0;
                arc.Size = new Size(rx, ry);  // 반지름 역할을 한다.

                if (Math.Abs(sweepangle) > 180)
                    arc.IsLargeArc = true;
                else
                    arc.IsLargeArc = false;

                if (sweepangle < 0)
                    NetTools.MathLib.MathGetEllipsePoint(0, 0, rx, ry, startangle, out pointx, out pointy);
                else
                    NetTools.MathLib.MathGetEllipsePoint(0, 0, rx, ry, startangle - Math.Abs(sweepangle), out pointx, out pointy);

                arc.Point = new Point(pointx + cx, pointy + cy);

                pf.Segments.Add(arc);

                if (objArgs.type == 1)
                {
                    LineSegment ls = new LineSegment();
                    ls.Point = new Point(cx, cy);
                    pf.Segments.Add(ls);
                }

                pg.Figures.Add(pf);
                path.Data = pg;

                child = path;
            }

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

            return child;
        }

        protected override void LineColorChanged(Color color)
        {
            Shape child = (Shape)GetShapeOriginal();

            if (child == null) return;

            child.Stroke = new SolidColorBrush(color);
        }

        protected override void FillColorChanged(Color color)
        {
            Shape child = (Shape)GetShapeOriginal();

            if (child == null) return;

            child.Fill = new SolidColorBrush(color);
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            int width = x2 - x1;
            int height = y2 - y1;

            if (nFillOption > 0)
            {
                Brush brush = new SolidBrush(RunColorFill);
                g.FillEllipse(brush, x1, y1, width, height);
            }

            if (nLineOption > 0)
            {
                Pen pen = new Pen(RunColorLine, bthick);
                pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);
                g.DrawEllipse(pen, x1, y1, width, height);
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.LineColor(writer, GetLineColor());
            SaveObjectItem.FillColor(writer, GetFillColor());
            SaveObjectItem.LineThick(writer, GetBorderThick());
            SaveObjectItem.LineOption(writer, nLineOption);
            SaveObjectItem.FillOption(writer, nFillOption);
        }*/

        protected override int ApplyGabByThick(int thick)
        {
            return thick / 2;
        }

        protected override void LineThickChanged(double thick)
        {
            //Ellipse child = (Ellipse)GetShapeOriginal();
            Shape child = (Shape)GetShapeOriginal();

            if (child == null) return;

            child.StrokeThickness = thick;
        }

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "CircleSetType")
            {
                int savetype = objArgs.type;

                objArgs.type = ((int)args[0]);

                if (savetype != objArgs.type)
                {
                    Shape child = (Shape)GetShapeOriginal();
                    Canvas parent_canvas = (Canvas)child.Parent;

                    int index = parent_canvas.Children.IndexOf(child);

                    child = MakeChildObject();

                    parent_canvas.Children[index] = child;
                    SetShapeOriginal(child);
                    MoveShape();
                }
                
                return 1;
            }
            else if (command == "CircleSetAngle")
            {
                float savestart = objArgs.fStartAngle;
                float savesweep = objArgs.fSweepAngle;

                objArgs.fStartAngle = ((float)args[0]);
                objArgs.fSweepAngle = ((float)args[1]);

                // 값이 변경되었을 때에만
                if (savestart != objArgs.fStartAngle ||
                    savesweep != objArgs.fSweepAngle)
                {

                    Shape child = (Shape)GetShapeOriginal();
                    Canvas parent_canvas = (Canvas)child.Parent;

                    int index = parent_canvas.Children.IndexOf(child);

                    child = MakeChildObject();

                    parent_canvas.Children[index] = child;
                    SetShapeOriginal(child);
                    MoveShape();
                }
                
                return 1;
            }

            return 0;
        }
    }
}

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
using System.Collections.Generic;
using NetTools.OldDefine;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectPoly : ObjectExpand
    {
        List<object> blockPoint;
        RECT rPolySize = new RECT();

        public ObjectPoly(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick, List<object> block)
            : base(ocp, rect, eid, null, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Poly;
            SetLineColor(lcolor);
            SetFillColor(fcolor);
            nLineOption = loption;
            SetBorderThick(lthick);

            SetPointBlock(form, block);

            Polygon child = new Polygon();

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

            MakePath(child);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        public void SetPointBlock(UserControl form, List<object> block)
        {
            //blockPoint = (ArrayList)Tools.CopyObject(block);
            blockPoint = block;

            CalcPolyRect(ref rPolySize);
            UpdateZone(rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
        }

        void CalcPolyRect(ref RECT rect)
        {
            Point poly;
            int i;

            poly = (Point)blockPoint[0];

            rect.left = (int)poly.X;
            rect.top = (int)poly.Y;
            rect.right = (int)poly.X;
            rect.bottom = (int)poly.Y;

            for (i = 0; i < blockPoint.Count; i++)
            {
                poly = (Point)blockPoint[i];
                if (poly.X < rect.left) rect.left = (int)poly.X;
                if (poly.Y < rect.top) rect.top = (int)poly.Y;
                if (poly.X > rect.right) rect.right = (int)poly.X;
                if (poly.Y > rect.bottom) rect.bottom = (int)poly.Y;
            }
        }

        void MakePath(Polygon polygon)
        {
            //Point[] polygon = new Point[blockPoint.Count];
            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;

            GetViewZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            int org_sizex = rPolySize.right - rPolySize.left;
            int org_sizey = rPolySize.bottom - rPolySize.top;
            int tar_sizex = x2 - x1;
            int tar_sizey = y2 - y1;
            int l;
            Point p;
            Point p2;

            for (l = 0; l < blockPoint.Count; l++)
            {
                p = (Point)blockPoint[l];
                
                p2 = new Point();
                
                if (org_sizex == 0)
                    p2.X = 0;
                else
                    p2.X = ((p.X - rPolySize.left) * (tar_sizex) / (org_sizex));

                if (org_sizey == 0)
                    p2.Y = 0;
                else
                    p2.Y = ((p.Y - rPolySize.top) * (tar_sizey) / (org_sizey));

                polygon.Points.Add(p2);
            }


            /*
            if (nFillOption > 0)
            {
                Brush brush = new SolidBrush(RunColorFill);
                g.FillPolygon(brush, polygon);
            }

            if (nLineOption > 0)
            {
                Pen pen = new Pen(RunColorLine, bthick);
                pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);
                g.DrawPolygon(pen, polygon);
            }*/
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            Point[] polygon = new Point[blockPoint.Count];

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            int org_sizex = rPolySize.right - rPolySize.left;
            int org_sizey = rPolySize.bottom - rPolySize.top;
            int tar_sizex = x2 - x1;
            int tar_sizey = y2 - y1;
            int l;
            Point p;

            for (l = 0; l < blockPoint.Count; l++)
            {
                p = (Point)blockPoint[l];
                polygon[l].X = p.X;
                polygon[l].Y = p.Y;

                if (org_sizex == 0)
                    polygon[l].X = x1;
                else
                    polygon[l].X = x1 + ((polygon[l].X - rPolySize.left) * (tar_sizex) / (org_sizex));

                if (org_sizey == 0)
                    polygon[l].Y = y1;
                else
                    polygon[l].Y = y1 + ((polygon[l].Y - rPolySize.top) * (tar_sizey) / (org_sizey));
            }

            if (nFillOption > 0)
            {
                Brush brush = new SolidBrush(RunColorFill);
                g.FillPolygon(brush, polygon);
            }

            if (nLineOption > 0)
            {
                Pen pen = new Pen(RunColorLine, bthick);
                pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);
                g.DrawPolygon(pen, polygon);
            }
        }*/

        /*
        public void GetPolyBlock(List<object> block)
        {
            Point poly = new Point();
            Point copy = new Point();

            block.Clear();

            for (int l = 0; l < blockPoint.Count; l++)
            {
                poly = (Point)blockPoint[l];
                copy = new Point();
                copy.X = poly.X;
                copy.Y = poly.Y;
                block.Add(poly);
            }
        }*/

        public override void UpdateZone(int x1, int y1, int x2, int y2)
        {
            int org_sizex = rPolySize.right - rPolySize.left;
            int org_sizey = rPolySize.bottom - rPolySize.top;
            int tar_sizex = x2 - x1;
            int tar_sizey = y2 - y1;
            int l;
            Point p;
            double x, y;

            for (l = 0; l < blockPoint.Count; l++)
            {
                p = (Point)blockPoint[l];

                if (org_sizex == 0)
                    x = x1;
                else
                    x = x1 + ((p.X - rPolySize.left) * (tar_sizex) / (org_sizex));

                if (org_sizey == 0)
                    y = y1;
                else
                    y = y1 + ((p.Y - rPolySize.top) * (tar_sizey) / (org_sizey));

                // p.X, p.Y 에 바로 대입하면 struct라서 그런지 대입해도 이전값을 유지한다.
                blockPoint[l] = new Point(x, y);
            }

            CalcPolyRect(ref rPolySize);
            base.UpdateZone(rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
        }

        /*
        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.LineColor(writer, GetLineColor());
            SaveObjectItem.FillColor(writer, GetFillColor());
            SaveObjectItem.LineThick(writer, GetBorderThick());
            SaveObjectItem.LineOption(writer, nLineOption);
            SaveObjectItem.FillOption(writer, nFillOption);

            Point poly;

            for (int l = 0; l < blockPoint.Count; l++)
            {
                poly = (Point)blockPoint[l];
                writer.WriteLine("\tPoint,{0},{1},", poly.X, poly.Y);
            }
        }*/
    }
}

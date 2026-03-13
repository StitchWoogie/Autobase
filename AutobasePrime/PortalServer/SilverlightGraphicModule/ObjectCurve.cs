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
using NetTools;
using System.Collections.Generic;

namespace SilverlightGraphicModule
{
    [Flags]
    public enum EnumCurveType : ushort
    {
        START = 0x0001,
        LINE = 0x0002,
        BEZIER = 0x0004,
        CLOSE = 0x0008,

        SYMMETRICAL = 0x8000,
        SMOOTH = 0x4000,
    }

    //[Serializable]
    public class CURVE_STRUCT
    {
        public EnumCurveType type;
        public int[] x = new int[3];
        public int[] y = new int[3];
    }
    /// <summary>
    /// Summary description for ObjectCurve.
    /// </summary>
    /// 
    //[Serializable]
    public class ObjectCurve : ObjectExpand
    {
        List<object> blockPoint = new List<object>();
        //RECT rPolySize = new RECT();

        // 그룹에서 Scale과 Rotate가 함께 쓰일 수 있기 때문에 Expand에 Rotate와 Scale을 함께 쓸 수 있도록 한다.

        public ObjectCurve(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick, List<object> block)
            : base(ocp, rect, eid, null, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Curve;
            SetLineColor(lcolor);
            SetFillColor(fcolor);
            nLineOption = loption;
            SetBorderThick(lthick);

            blockPoint = block;
            //CalcPolyRect(ref rPolySize);
            //SetCurveBlock(form, block);

            Path child = new Path();

            bool close_flag = MakePath(child);

            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            if (close_flag == false)
            {	// 채움이 없다면 선은 반드시 있어야 한다.

                nFillOption = 0;

                if (nLineOption == 0)
                    nLineOption = 1;
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
        void CalcPolyRect(ref RECT rect)
        {
            CURVE_STRUCT poly;
            int l;
            int j;

            if (blockPoint == null) return;
            if (blockPoint.Count == 0) return;

            poly = (CURVE_STRUCT)blockPoint[0];

            rect.left = poly.x[0];
            rect.top = poly.y[0];
            rect.right = poly.x[0];
            rect.bottom = poly.y[0];

            for (l = 0; l < blockPoint.Count; l++)
            {
                poly = (CURVE_STRUCT)blockPoint[l];

                if ((poly.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)
                {
                    for (j = 0; j < 3; j++)
                    {
                        if (poly.x[j] < rect.left) rect.left = poly.x[j];
                        if (poly.y[j] < rect.top) rect.top = poly.y[j];
                        if (poly.x[j] > rect.right) rect.right = poly.x[j];
                        if (poly.y[j] > rect.bottom) rect.bottom = poly.y[j];
                    }
                }
                else
                {
                    if (poly.x[0] < rect.left) rect.left = poly.x[0];
                    if (poly.y[0] < rect.top) rect.top = poly.y[0];
                    if (poly.x[0] > rect.right) rect.right = poly.x[0];
                    if (poly.y[0] > rect.bottom) rect.bottom = poly.y[0];
                }
            }
        }*/

        bool MakePath(Path path)
        {
            if (blockPoint.Count < 1) return true;
            //int hap = 0;
            CURVE_STRUCT curve;
            int l;

            /*
            for (l = 0; l < blockPoint.Count; l++)
            {
                curve = (CURVE_STRUCT)blockPoint[l];
                if ((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)
                    hap += 3;
                else
                    hap += 1;
            }*/

            bool close_flag = false;

            
            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;

            GetViewZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);
            /*
            
            int org_sizex = rPolySize.right - rPolySize.left;
            int org_sizey = rPolySize.bottom - rPolySize.top;
            int tar_sizex = x2 - x1;
            int tar_sizey = y2 - y1;*/

            //hap = 0;

            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();
            Point p;

            for (l = 0; l < blockPoint.Count; l++)
            {
                curve = (CURVE_STRUCT)blockPoint[l];

                if ((curve.type & EnumCurveType.START) == EnumCurveType.START)
                {
                    if (pf.Segments.Count > 0)
                    {
                        pg.Figures.Add(pf);
                        pf = new PathFigure();
                    }

                    p = new Point();

                    
                        p.X = curve.x[0]-x1;

                        p.Y = curve.y[0]-y1;

                    pf.StartPoint = p;
                }

                else if ((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)
                {
                    BezierSegment bs = new BezierSegment();

                    for (int j = 0; j < 3; j++)
                    {
                        p = new Point();


                        p.X = curve.x[j]-x1;


                        p.Y = curve.y[j]-y1;


                        if (j == 0) bs.Point1 = p;
                        else if (j == 1) bs.Point2 = p;
                        else if (j == 2) bs.Point3 = p;
                    }

                    pf.Segments.Add(bs);

                    if ((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)
                    {
                        pf.IsClosed = true;
                        pg.Figures.Add(pf);
                        pf = new PathFigure();

                        close_flag = true;
                    }
                }

                else
                {
                    LineSegment ls = new LineSegment();

                    p = new Point();

                    p.X = curve.x[0]-x1;


                    p.Y = curve.y[0]-y1;

                    ls.Point = p;
                    pf.Segments.Add(ls);

                    if ((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)
                    {
                        pf.IsClosed = true;
                        pg.Figures.Add(pf);
                        pf = new PathFigure();

                        close_flag = true;
                    }

                    //hap++;
                }
            }

            // 남아 있는 요소를 등록
            if (pf.Segments.Count > 0)
            {
                pg.Figures.Add(pf);
            }

            path.Data = pg;

            return close_flag;
        }

        /*
        bool MakePath(Path path)
        {
            if (blockPoint.Count < 1) return true;
            int hap = 0;
            CURVE_STRUCT curve;
            int l;

            for (l = 0; l < blockPoint.Count; l++)
            {
                curve = (CURVE_STRUCT)blockPoint[l];
                if ((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)
                    hap += 3;
                else
                    hap += 1;
            }

            bool close_flag = false;

            int x1=0;
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

            hap = 0;

            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();
            Point p;

            for (l = 0; l < blockPoint.Count; l++)
            {
                curve = (CURVE_STRUCT)blockPoint[l];

                if ((curve.type & EnumCurveType.START) == EnumCurveType.START)
                {
                    if (pf.Segments.Count > 0)
                    {
                        pg.Figures.Add(pf);
                        pf = new PathFigure();
                    }

                    p = new Point();

                    if (org_sizex == 0)
                        p.X = 0;
                    else
                        p.X = ((curve.x[0] - rPolySize.left) * (tar_sizex) / (org_sizex));

                    if (org_sizey == 0)
                        p.Y = 0;
                    else
                        p.Y = ((curve.y[0] - rPolySize.top) * (tar_sizey) / (org_sizey));

                    pf.StartPoint = p;
                }

                else if ((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)
                {
                    BezierSegment bs = new BezierSegment();

                    for (int j = 0; j < 3; j++)
                    {
                        p = new Point();

                        if (org_sizex == 0)
                            p.X = 0;
                        else
                            p.X = ((curve.x[j] - rPolySize.left) * (tar_sizex) / (org_sizex));

                        if (org_sizey == 0)
                            p.Y = 0;
                        else
                            p.Y = ((curve.y[j] - rPolySize.top) * (tar_sizey) / (org_sizey));

                        //polyatr[hap] = (byte)PathPointType.Bezier;//PT_BEZIERTO;

                        if (j == 0) bs.Point1 = p;
                        else if (j == 1) bs.Point2 = p;
                        else if (j == 2) bs.Point3 = p;
                    }

                    pf.Segments.Add(bs);

                    if ((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)
                    {
                        pf.IsClosed = true;
                        pg.Figures.Add(pf);
                        pf = new PathFigure();

                        close_flag = true;
                    }
                }

                else
                {
                    LineSegment ls = new LineSegment();

                    p = new Point();
                    if (org_sizex == 0)
                        p.X = 0;
                    else
                        p.X = ((curve.x[0] - rPolySize.left) * (tar_sizex) / (org_sizex));

                    if (org_sizey == 0)
                        p.Y = 0;
                    else
                        p.Y = ((curve.y[0] - rPolySize.top) * (tar_sizey) / (org_sizey));

                    ls.Point = p;
                    pf.Segments.Add(ls);

                    if ((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)
                    {
                        pf.IsClosed = true;
                        pg.Figures.Add(pf);
                        pf = new PathFigure();

                        close_flag = true;
                    }

                    hap++;
                }
            }

            // 남아 있는 요소를 등록
            if (pf.Segments.Count > 0)
            {
                pg.Figures.Add(pf);
            }

            path.Data = pg;

            return close_flag;
        }*/

        /*
        public void GetCurveBlock(List<object> block)
        {
            CURVE_STRUCT poly;
            CURVE_STRUCT poly_copy;

            block.Clear();

            for (int l = 0; l < blockPoint.Count; l++)
            {
                poly = (CURVE_STRUCT)blockPoint[l];
                poly_copy = new CURVE_STRUCT();
                poly_copy.type = poly.type;
                for (int j = 0; j < 3; j++)
                {
                    poly_copy.x[j] = poly.x[j];
                    poly_copy.y[j] = poly.y[j];
                }
                block.Add(poly_copy);
            }
        }*/

        /*
        public void SetCurveBlock(UserControl form, List<object> block)
        {
            CURVE_STRUCT poly;
            CURVE_STRUCT poly_copy;

            blockPoint.Clear();

            for (int l = 0; l < block.Count; l++)
            {
                poly = (CURVE_STRUCT)block[l];
                poly_copy = new CURVE_STRUCT();
                poly_copy.type = poly.type;
                for (int j = 0; j < 3; j++)
                {
                    poly_copy.x[j] = poly.x[j];
                    poly_copy.y[j] = poly.y[j];
                }
                blockPoint.Add(poly_copy);
            }

            CalcPolyRect(ref rPolySize);
            UpdateZone(rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
        }

        public override void UpdateZone(int x1, int y1, int x2, int y2)
        {
            int org_sizex = rPolySize.right - rPolySize.left;
            int org_sizey = rPolySize.bottom - rPolySize.top;
            int tar_sizex = x2 - x1;
            int tar_sizey = y2 - y1;
            int l;
            int j;
            CURVE_STRUCT p;

            for (l = 0; l < blockPoint.Count; l++)
            {
                p = (CURVE_STRUCT)blockPoint[l];

                for (j = 0; j < 3; j++)
                {
                    if (org_sizex == 0)
                        p.x[j] = x1;
                    else
                        p.x[j] = x1 + ((p.x[j] - rPolySize.left) * (tar_sizex) / (org_sizex));

                    if (org_sizey == 0)
                        p.y[j] = y1;
                    else
                        p.y[j] = y1 + ((p.y[j] - rPolySize.top) * (tar_sizey) / (org_sizey));
                }
            }

            CalcPolyRect(ref rPolySize);
            base.UpdateZone(rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
        }
        */
    }
}

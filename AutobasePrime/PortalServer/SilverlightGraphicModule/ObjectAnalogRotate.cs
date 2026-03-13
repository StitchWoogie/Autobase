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
using AutoLibLocal;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectArgsAnalogRotate
    {
        public BrushPublic lBackColor = new BrushPublic();
        public int nStartAngle;
        public int nEndAngle;
        public sbyte bAngleDirection;
        public int nMethod;
        public string sFileName;
    }

    public class POLY_STRUCT
    {
        public Color lLineColor;
        public Color lFillColor;
        public Point[] p;
        public int nPointCount;
        public Polygon polygon;
    }

    /// <summary>
    /// Summary description for ObjectAnalogString.
    /// </summary>
    public class ObjectAnalogRotate : ObjectTag
    {
        ObjectArgsAnalogRotate objArgs;
        List<object> blockPoly = new List<object>();
        int nMaxPolyUnit;

        public ObjectArgsAnalogRotate ObjectArgs
        {
            get
            {
                return objArgs;
            }
            set
            {
                objArgs = value;
                SetFileName(value.sFileName);
            }
        }

        public ObjectAnalogRotate(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsAnalogRotate args)
            : base(ocp, rect, eid, general, null, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //

            enumObjectType = EnumObjectType.AnalogRotate;
            objArgs = args;
        
            SetFileName(args.sFileName);
            BackColor = args.lBackColor;

            PrepareObject(parent_canvas);
        }

        void PrepareObject(Canvas parent_canvas)
        {
            Grid child = new Grid();

            /*
            if (objArgs.nMethod == 1)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = new SolidColorBrush(RunColorBack);
                child.Children.Add(ellipse);
            }
            else if (objArgs.nMethod == 2)
            {
                Rectangle r = new Rectangle();
                r.Fill = new SolidColorBrush(RunColorBack);
                child.Children.Add(r);
            }*/

            child.SizeChanged += new SizeChangedEventHandler(child_SizeChanged);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();

            MakePath(child);
        }

        void ReCalcPosition()
        {
            Grid child = (Grid)GetShapeOriginal();

            int cx = (int)child.ActualWidth / 2;
            int cy = (int)child.ActualHeight / 2;
            int width = (int)child.ActualWidth;
            int height = (int)child.ActualHeight;

            float fBase, fFull;
            double curr;
            float radius, r;
            double getx;
            double gety;
            int l;
            double value_x;
            double value_y;
            float angle, degree;
            double radian;
            int nAngleGab;	// 변하는 각
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (objArgs.bAngleDirection == 0)
            {	// 시계방향.
                nAngleGab = objArgs.nStartAngle - objArgs.nEndAngle;
                while (nAngleGab <= 0) nAngleGab += 360;
            }
            else
            {	// 반시계방향.
                nAngleGab = objArgs.nEndAngle - objArgs.nStartAngle;
                while (nAngleGab <= 0) nAngleGab += 360;
            }

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {
                fBase = ai.fBase;
                fFull = ai.fFull;
                curr = ai.curr;
                if (fFull - fBase == 0)
                {
                    fFull = 100;
                    fBase = 0;
                }
            }
            else
            {
                fBase = 0;
                fFull = 100;
                curr = 50;
            }

            if (width > height) radius = (float)height;
            else radius = (float)width;

            radius = radius / 2;

            Point p;
            POLY_STRUCT poly;

            for (l = 0; l < blockPoly.Count; l++)
            {
                poly = (POLY_STRUCT)blockPoly[l];

                for (int i = 0; i < poly.nPointCount; i++)
                {
                    value_x = (poly.p[i].X * radius) / nMaxPolyUnit;
                    value_y = (poly.p[i].Y * radius) / nMaxPolyUnit;

                    degree = MathLib.MathGradientToDegree((int)poly.p[i].X, (int)poly.p[i].Y);
                    radian = MathLib.MathDegreeToRadian(degree);

                    // 점의 실제 반지름을 구한다.
                    if (degree == 0 || degree == 180)
                    {
                        r = (float)value_x;
                    }
                    else
                    {
                        r = (float)(value_y / Math.Sin(radian));
                    }

                    // 점이 실제로 위치할 각을 찾는다.
                    if (fFull - fBase == 0)
                    {
                        angle = objArgs.nStartAngle + degree;
                    }
                    else
                    {
                        if (objArgs.bAngleDirection == 0)
                        {
                            angle = (float)(objArgs.nStartAngle + degree - (nAngleGab) * curr / (fFull - fBase));
                        }
                        else
                        {
                            angle = (float)(objArgs.nStartAngle + degree + (nAngleGab) * curr / (fFull - fBase));
                        }
                    }

                    MathLib.MathGetEllipsePoint(cx, cy, r, r, angle, out getx, out gety);

                    p = poly.polygon.Points[i];

                    p.X = getx;
                    p.Y = gety;
                    
                    poly.polygon.Points[i] = p; // point 가 struct이다.
                }

                poly.polygon.Fill = new SolidColorBrush(poly.lFillColor);
                poly.polygon.Stroke = new SolidColorBrush(RunColorLine);    // 이부분이 호출되지 않으니 폴리곤의 크기변경이 적용되지 않는다.
                poly.polygon.StrokeThickness = 1;
            }
        }

        void child_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReCalcPosition();
        }

        void MakePath(Grid child)
        {
            child.Children.Clear();

            if (objArgs.nMethod == 1)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = ObjectRectangle.MakePublicBrush(RunColorBack);
                child.Children.Add(ellipse);
            }
            else if (objArgs.nMethod == 2)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Fill = ObjectRectangle.MakePublicBrush(RunColorBack);
                child.Children.Add(rectangle);
            }

            int cx = (int)child.ActualWidth / 2;
            int cy = (int)child.ActualHeight / 2;
            int width = (int)child.ActualWidth;
            int height = (int)child.ActualHeight;    

            float fBase, fFull;
            double curr;
            float radius, r;
            double getx;
            double gety;
            int l;
            double value_x;
            double value_y;
            float angle, degree;
            double radian;
            int nAngleGab;	// 변하는 각
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (objArgs.bAngleDirection == 0)
            {	// 시계방향.
                nAngleGab = objArgs.nStartAngle - objArgs.nEndAngle;
                while (nAngleGab <= 0) nAngleGab += 360;
            }
            else
            {	// 반시계방향.
                nAngleGab = objArgs.nEndAngle - objArgs.nStartAngle;
                while (nAngleGab <= 0) nAngleGab += 360;
            }

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {
                fBase = ai.fBase;
                fFull = ai.fFull;
                curr = ai.curr;
                if (fFull - fBase == 0)
                {
                    fFull = 100;
                    fBase = 0;
                }
            }
            else
            {
                fBase = 0;
                fFull = 100;
                curr = 50;
            }

            if (width > height) radius = (float)height;
            else radius = (float)width;

            radius = radius / 2;

            Point p;
            POLY_STRUCT poly;

            for (l = 0; l < blockPoly.Count; l++)
            {
                poly = (POLY_STRUCT)blockPoly[l];
                poly.polygon = new Polygon();
                

                for (int i = 0; i < poly.nPointCount; i++)
                {
                    value_x = (poly.p[i].X * radius) / nMaxPolyUnit;
                    value_y = (poly.p[i].Y * radius) / nMaxPolyUnit;

                    degree = MathLib.MathGradientToDegree((int)poly.p[i].X, (int)poly.p[i].Y);
                    radian = MathLib.MathDegreeToRadian(degree);

                    // 점의 실제 반지름을 구한다.
                    if (degree == 0 || degree == 180)
                    {
                        r = (float)value_x;
                    }
                    else
                    {
                        r = (float)(value_y / Math.Sin(radian));
                    }

                    // 점이 실제로 위치할 각을 찾는다.
                    if (fFull - fBase == 0)
                    {
                        angle = objArgs.nStartAngle + degree;
                    }
                    else
                    {
                        if (objArgs.bAngleDirection == 0)
                        {
                            angle = (float)(objArgs.nStartAngle + degree - (nAngleGab) * curr / (fFull - fBase));
                        }
                        else
                        {
                            angle = (float)(objArgs.nStartAngle + degree + (nAngleGab) * curr / (fFull - fBase));
                        }
                    }

                    MathLib.MathGetEllipsePoint(cx, cy, r, r, angle, out getx, out gety);

                    p = new Point();
                    p.X = getx;
                    p.Y = gety;

                    poly.polygon.Points.Add(p);
                }

                poly.polygon.Fill = ObjectRectangle.MakePublicBrush(RunColorFill);
                poly.polygon.Stroke = new SolidColorBrush(RunColorLine);
                poly.polygon.StrokeThickness = 1;

                child.Children.Add(poly.polygon);
            }
        }

        double fOldCurr;

        public override void EventTimerObject(UserControl form)
        {
            base.EventTimerObject(form);

            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (fOldCurr != ai.curr)
            {
                fOldCurr = ai.curr;
                ReCalcPosition();
            }
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            x1++;
            y1++;
            x2--;
            y2--;

            int cx, cy;
            int width;
            int height;

            width = x2 - x1;
            height = y2 - y1;
            cx = x1 + (width) / 2;
            cy = y1 + (height) / 2;

            //	배경뿌리기.
            if (objArgs.nMethod == 1)
            {
                Pen pen = new Pen(objArgs.lBackColor, 1);
                Brush brush = new SolidBrush(RunColorBack);
                g.FillEllipse(brush, x1, y1, width, height);
                //g.DrawEllipse(pen, x1, y1, width, height);
            }
            else if (objArgs.nMethod == 2)
            {
                Pen pen = new Pen(objArgs.lBackColor, 1);
                Brush brush = new SolidBrush(RunColorBack);
                g.FillRectangle(brush, x1, y1, width, height);
                //g.DrawRectangle(pen, x1, y1, width, height);
            }

            DisplayAllPoly(g, cx, cy, width, height);

            Brush brush_black = new SolidBrush(Color.Black);
            g.FillRectangle(brush_black, cx, cy, 1, 1);
        }*/

        void SetFileName(string filename)
        {
            LoadDefaultRotate();
            /*
            string path;

            if (filename.Length == 0)
            {
                LoadDefaultRotate();
                return;
            }

            path = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(objCommonProperty, filename);

            if (!File.Exists(path))
            {
                LoadDefaultRotate();
                return;
            }

            blockPoly.Clear();

            FileStream fs;
            string buf = "";
            bool block_start = false;
            bool unit_read = false;
            CommaBlockString comma = new CommaBlockString();
            ArrayList blockImsi = new ArrayList();

            Color linecolor = Color.Black, fillcolor = Color.Blue;
            int l;
            int r = 0, g = 0, b = 0;

            fs = File.OpenRead(path);
            if (fs == null)
            {
                LoadDefaultRotate();
                return;
            }

            while (true)
            {
                if (!Tools.TextGetOneLine(fs, ref buf)) break;

                comma.Set(buf);

                comma.GetString(ref buf);
                if (block_start == false)
                {
                    if (String.Compare(buf, "Block", true) == 0)
                    {	// block 인식자.
                        comma.GetString(ref buf);
                        if (String.Compare(buf, "BEGIN", true) == 0)
                        {
                            blockImsi.Clear();
                            block_start = true;
                        }
                    }
                    else if (String.Compare(buf, "Unit", true) == 0)
                    {
                        comma.GetInt(ref nMaxPolyUnit);
                        unit_read = true;
                    }
                    else { }
                }
                else
                {	// block 속에 있다.
                    if (String.Compare(buf, "Block", true) == 0)
                    {	// block 인식자.
                        comma.GetString(ref buf);
                        if (String.Compare(buf, "END", true) == 0)
                        {
                            if (blockImsi.Count > 0)
                            {
                                POLY_STRUCT poly = new POLY_STRUCT();
                                poly.nPointCount = (int)blockImsi.Count;
                                poly.lLineColor = linecolor;
                                poly.lFillColor = fillcolor;
                                poly.p = new Point[(int)blockImsi.Count];

                                Point p;
                                for (l = 0; l < blockImsi.Count; l++)
                                {
                                    p = (Point)blockImsi[l];
                                    poly.p[(int)l].X = p.X;
                                    poly.p[(int)l].Y = p.Y;
                                }
                                blockPoly.Add(poly);
                            }
                            block_start = false;
                        }
                    }
                    else if (String.Compare(buf, "Point", true) == 0)
                    {	// Point 인식자.

                        int x = 0, y = 0;

                        comma.GetInt(ref x);
                        comma.GetInt(ref y);

                        Point p = new Point(x, y);

                        blockImsi.Add(p);
                    }
                    else if (String.Compare(buf, "LineColor", true) == 0)
                    {	// Point 인식자.
                        comma.GetInt(ref r);
                        comma.GetInt(ref g);
                        comma.GetInt(ref b);
                        linecolor = Color.FromArgb(r, g, b);
                    }
                    else if (String.Compare(buf, "FillColor", true) == 0)
                    {	// Point 인식자.
                        comma.GetInt(ref r);
                        comma.GetInt(ref g);
                        comma.GetInt(ref b);
                        fillcolor = Color.FromArgb(r, g, b);
                    }
                }
            }

            fs.Close();

            if (unit_read == false) nMaxPolyUnit = 100;

            if (blockPoly.Count == 0)
            {	// 파일이 고장 났다.
                LoadDefaultRotate();
            }*/
        }

        //--------------------------------------------------------------------------
        //	파일이 없을 때는 화살표를 기본값으로 취한다.
        //--------------------------------------------------------------------------

        void LoadDefaultRotate()
        {
            blockPoly.Clear();

            POLY_STRUCT poly = new POLY_STRUCT();

            poly.nPointCount = 8;
            poly.lLineColor = Colors.Black;
            poly.lFillColor = Color.FromArgb(255, 0, 255, 0);

            nMaxPolyUnit = 100;

            poly.p = new Point[8];
            poly.p[0].X = -95; poly.p[0].Y = -10;
            poly.p[1].X = 60; poly.p[1].Y = -10;
            poly.p[2].X = 60; poly.p[2].Y = -20;
            poly.p[3].X = 95; poly.p[3].Y = 0;
            poly.p[4].X = 60; poly.p[4].Y = 20;
            poly.p[5].X = 60; poly.p[5].Y = 10;
            poly.p[6].X = -95; poly.p[6].Y = 10;
            poly.p[7].X = -95; poly.p[7].Y = -10;

            blockPoly.Add(poly);
        }

        /*
        void DisplayAllPoly(Graphics g, int cx, int cy, int width, int height)
        {
            float fBase, fFull;
            double curr;
            float radius, r;
            int getx;
            int gety;
            int l;
            double value_x;
            double value_y;
            float angle, degree;
            double radian;
            int nAngleGab;	// 변하는 각
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (objArgs.bAngleDirection == 0)
            {	// 시계방향.
                nAngleGab = objArgs.nStartAngle - objArgs.nEndAngle;
                while (nAngleGab <= 0) nAngleGab += 360;
            }
            else
            {	// 반시계방향.
                nAngleGab = objArgs.nEndAngle - objArgs.nStartAngle;
                while (nAngleGab <= 0) nAngleGab += 360;
            }

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {
                fBase = ai.fBase;
                fFull = ai.fFull;
                curr = ai.curr;
                if (fFull - fBase == 0)
                {
                    fFull = 100;
                    fBase = 0;
                }
            }
            else
            {
                fBase = 0;
                fFull = 100;
                curr = 50;
            }

            if (width > height) radius = (float)height;
            else radius = (float)width;

            radius = radius / 2;

            Point[] p;
            POLY_STRUCT poly;

            for (l = 0; l < blockPoly.Count; l++)
            {
                poly = (POLY_STRUCT)blockPoly[l];

                p = new Point[poly.nPointCount];

                if (p == null) continue;

                for (int i = 0; i < poly.nPointCount; i++)
                {
                    value_x = (poly.p[i].X * radius) / nMaxPolyUnit;
                    value_y = (poly.p[i].Y * radius) / nMaxPolyUnit;

                    degree = MathLib.MathGradientToDegree(poly.p[i].X, poly.p[i].Y);
                    radian = MathLib.MathDegreeToRadian(degree);

                    // 점의 실제 반지름을 구한다.
                    if (degree == 0 || degree == 180)
                    {
                        r = (float)value_x;
                    }
                    else
                    {
                        r = (float)(value_y / Math.Sin(radian));
                    }

                    // 점이 실제로 위치할 각을 찾는다.
                    if (fFull - fBase == 0)
                    {
                        angle = objArgs.nStartAngle + degree;
                    }
                    else
                    {
                        if (objArgs.bAngleDirection == 0)
                        {
                            angle = (float)(objArgs.nStartAngle + degree - (nAngleGab) * curr / (fFull - fBase));
                        }
                        else
                        {
                            angle = (float)(objArgs.nStartAngle + degree + (nAngleGab) * curr / (fFull - fBase));
                        }
                    }

                    MathLib.MathGetEllipsePoint(cx, cy, r, r, angle, out getx, out gety);

                    p[i].X = getx;
                    p[i].Y = gety;
                }

                Pen pen = new Pen(poly.lLineColor);
                Brush brush = new SolidBrush(poly.lFillColor);
                g.FillPolygon(brush, p);
                g.DrawPolygon(pen, p);
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            ObjectSaveTag(writer);
            SaveObjectItem.FileName(writer, objArgs.sFileName);
            SaveObjectItem.BackColor(writer, GetBackColor());
            SaveObjectItem.LocalMethod(writer, objArgs.nMethod);
            SaveObjectItem.StartAngle(writer, objArgs.nStartAngle);
            SaveObjectItem.EndAngle(writer, objArgs.nEndAngle);
            SaveObjectItem.AngleDirection(writer, objArgs.bAngleDirection);
        }

        public override void GetFamilyFile(ArrayList block)
        {
            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

            family.filename = objArgs.sFileName;
            block.Add(family);
        }

        public override void ChangeFamilyFile(ArrayList block)
        {
            FAMILY_FILE_STRUCT family;
            family = ObjectGroup.GetMatchFamilyFile(objArgs.sFileName, block);
            if (family == null) return;
            objArgs.sFileName = family.change;
        }*/

    }
}

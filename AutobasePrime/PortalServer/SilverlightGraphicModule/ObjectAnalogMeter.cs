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
using NetTools;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public class ObjectArgsAnalogMeter
    {
        public BrushPublic colorBack = new BrushPublic();
        public Color colorGuide;
        public Color colorHand;
        public int thickHand;
        public BrushPublic colorBorder = new BrushPublic();
        public Color colorText;
    }

    /// <summary>
    /// Summary description for ObjectAnalogString.
    /// </summary> 

    public class ObjectAnalogMeter : ObjectTag
    {
        ObjectArgsAnalogMeter objArgs;

        public Color GuideColor
        {
            get
            {
                return objArgs.colorGuide;
            }
            set
            {
                objArgs.colorGuide = value;
            }
        }

        public ObjectAnalogMeter(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsAnalogMeter args)
            : base(ocp, rect, eid, general, lf, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //

            enumObjectType = EnumObjectType.AnalogMeter;
            objArgs = args;

            TextColor = args.colorText;
            BackColor = args.colorBack;
            LineColor = args.colorHand;
            FillColor = args.colorBorder;
            LineThick = args.thickHand;

            PrepareObject(parent_canvas);
        }

        TextBlock textUnit;

        void PrepareObject(Canvas parent_canvas)
        {
            Grid child = new Grid();

            child.Background = ObjectRectangle.MakePublicBrush(RunColorFill);
            child.SizeChanged += new SizeChangedEventHandler(child_SizeChanged); 

            ObjectAnalogString.AddLinePopBox2(child);

            //Grid grid = new Grid();
            gridIn.Background = ObjectRectangle.MakePublicBrush(RunColorBack);
            gridIn.SizeChanged += new SizeChangedEventHandler(gridIn_SizeChanged);
            gridIn.Margin = new Thickness(3, 3, 3, child.ActualHeight * 0.3);
            child.Children.Add(gridIn);
            ObjectAnalogString.AddLinePushBox2(gridIn);

            for (int i = 0, angle = 170; i < 17; i++, angle -= 10)
            {
                if (angle == 170 || angle == 90 || angle == 10 || angle == 130 || angle == 50)
                {
                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 2;
                    lineIn.Add(line);
                    gridIn.Children.Add(line);
                }
                else
                {
                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1;
                    lineIn.Add(line);
                    gridIn.Children.Add(line);
                }
            }

            lineArrow = new Line();
            lineArrow.Stroke = new SolidColorBrush(RunColorLine);
            lineArrow.StrokeThickness = 3;
            gridIn.Children.Add(lineArrow);

            textUnit = new TextBlock();
            textUnit.Text = MakeUnitString();

            MakeFont(textUnit);

            textUnit.HorizontalAlignment = HorizontalAlignment.Center;
            textUnit.VerticalAlignment = VerticalAlignment.Center;
            gridIn.Children.Add(textUnit);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        string MakeUnitString()
        {
            string unit;
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {	// AI
                unit = ai.unit;
            }
            else
            {
                unit = "No Tag";
            }

            if (unit == null) unit = "";

            unit = unit.Trim();
            return unit;
        }

        public override void OnTagFileReaded()
        {
            base.OnTagFileReaded();

            textUnit.Text = MakeUnitString();
        }

        Line lineArrow;

        void ReCalcLines()
        {
            int x1, y1, x2, y2;
            int radius1, radius2;
            int midx;
            double px1, py1, px2, py2;
            int bottom;

            x1 = 0;
            y1 = 0;
            x2 = (int)gridIn.ActualWidth - 1;
            y2 = (int)gridIn.ActualHeight - 1;

            bottom = y2;

            midx = (x2 - x1) / 2 + x1;				// 중심이 위치하는 X 좌표

            if ((x2 - x1) / 2 > (bottom - y1))
            {				// 가로가 세로보다 2배 이상이 크다.
                radius1 = bottom - (y1 + 3);		// 눈금의 바깥 반지름
                radius2 = (int)(radius1 * 0.8);		// 큰 눈금의 안쪽 반지름
            }
            else
            {
                radius1 = (x2 - x1) / 2 - 1;		// 눈금의 바깥 반지름
                radius2 = (int)(radius1 * 0.8);		// 큰 눈금의 안쪽 반지름
            }

            int angle = 170;

            for (int i = 0; i < 17; i++, angle -= 10)
            {
                MathLib.MathGetEllipsePoint(midx, bottom, (float)radius1, (float)radius1, (float)angle, out px1, out py1);
                MathLib.MathGetEllipsePoint(midx, bottom, (float)radius2, (float)radius2, (float)angle, out px2, out py2);

                lineIn[i].X1 = px1;
                lineIn[i].Y1 = py1;
                lineIn[i].X2 = px2;
                lineIn[i].Y2 = py2;
            }

            float fBase, fFull;
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);
            double curr;

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {	// AI
                fBase = ai.view_base;
                fFull = ai.view_full;
                curr = ai.curr;
            }
            else
            {
                fBase = (float)0;
                fFull = (float)100;
                curr = (float)5.0;
                if (curr < fBase) curr = fBase;
                if (curr > fFull) curr = fFull;
            }

            if (fFull - fBase == 0)
                angle = 0;
            else
            {
                if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
                {	// AI
                    angle = (int)(160.0 * (TagUtil.GetDisplayValue(ai, (float)curr) - fBase) / (fFull - fBase));
                }
                else
                {
                    angle = (int)(160.0 * (curr - fBase) / (fFull - fBase));
                }
            }

            angle = 170 - angle;

            MathLib.MathGetEllipsePoint(midx, bottom, (float)radius1, (float)radius1, (float)angle, out px1, out py1);

            lineArrow.X1 = px1;
            lineArrow.Y1 = py1;
            lineArrow.X2 = midx;
            lineArrow.Y2 = bottom;
        }

        void gridIn_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReCalcLines();
        }

        Grid gridIn = new Grid();
        List<Line> lineIn = new List<Line>();

        void child_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid child = (Grid)GetShapeOriginal();
            gridIn.Margin = new Thickness(3, 3, 3, child.ActualHeight * 0.3);
        }

        double fOldCurr;

        public override void EventTimerObject(UserControl form)
        {
            base.EventTimerObject(form);

            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (fOldCurr != ai.curr)
            {
                fOldCurr = ai.curr;
                ReCalcLines();
            }
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            DrawClass.PopBox2(g, x1, y1, x2, y2, RunColorFill);

            float fBase, fFull;
            double curr;
            int bottom;
            int radius1, radius2;
            int midx;
            int angle;
            int px1, py1, px2, py2;
            int i;
            RECT rect = new RECT();
            string unit;
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            Pen penBlack1 = new Pen(objArgs.colorGuide, 1);
            Pen penBlack2 = new Pen(objArgs.colorGuide, 2);
            Pen penRed = new Pen(RunColorLine, bthick);

            x1 = x1 + 3;
            y1 = y1 + 3;
            x2 = x2 - 3;
            bottom = y2 - (y2 - y1) / 3;

            midx = (x2 - x1) / 2 + x1;				// 중심이 위치하는 X 좌표

            if ((x2 - x1) / 2 > (bottom - y1))
            {				// 가로가 세로보다 2배 이상이 크다.
                radius1 = bottom - (y1 + 3);		// 눈금의 바깥 반지름
                radius2 = (int)(radius1 * 0.8);		// 큰 눈금의 안쪽 반지름
            }
            else
            {
                radius1 = (x2 - x1) / 2 - 1;				// 눈금의 바깥 반지름
                radius2 = (int)(radius1 * 0.8);		// 큰 눈금의 안쪽 반지름
            }

            DrawClass.PushBox2(g, x1, y1, x2, bottom, RunColorBack);

            rect.left = x1;
            rect.top = y1;
            rect.right = x2;
            rect.bottom = y2 - 3;

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {	// AI
                unit = ai.unit;
            }
            else
            {
                unit = "No Tag";
            }

            if (unit == null) unit = "";

            unit = unit.Trim();

            Font font = MakeFont();

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            Brush brush = new SolidBrush(RunColorText);

            DrawClass.DrawText(g, unit, font, brush, rect, format);

            for (i = 0, angle = 170; i < 17; i++, angle -= 10)
            {
                MathLib.MathGetEllipsePoint(midx, bottom, (float)radius1, (float)radius1, (float)angle, out px1, out py1);
                if (angle == 170 || angle == 90 || angle == 10 || angle == 130 || angle == 50)
                {
                    MathLib.MathGetEllipsePoint(midx, bottom, (float)radius2, (float)radius2, (float)angle, out px2, out py2);
                    g.DrawLine(penBlack2, px1, py1, px2, py2);
                }
                else
                {
                    MathLib.MathGetEllipsePoint(midx, bottom, (float)radius2, (float)radius2, (float)angle, out px2, out py2);
                    g.DrawLine(penBlack1, px1, py1, px2, py2);
                }
            }

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {	// AI
                fBase = ai.view_base;
                fFull = ai.view_full;
                curr = ai.curr;
            }
            else
            {
                fBase = (float)0;
                fFull = (float)100;
                curr = (float)5.0;
                if (curr < fBase) curr = fBase;
                if (curr > fFull) curr = fFull;
            }

            if (fFull - fBase == 0)
                angle = 0;
            else
            {
                if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
                {	// AI
                    angle = (int)(160.0 * (TagUtil.GetDisplayValue(ai, (float)curr) - fBase) / (fFull - fBase));
                }
                else
                {
                    angle = (int)(160.0 * (curr - fBase) / (fFull - fBase));
                }
            }

            angle = 170 - angle;

            MathLib.MathGetEllipsePoint(midx, bottom, (float)radius1, (float)radius1, (float)angle, out px1, out py1);

            g.DrawLine(penRed, px1, py1, midx, bottom);
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            ObjectSaveTag(writer);
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            SaveObjectItem.GuideLineColor(writer, objArgs.colorGuide);
            SaveObjectItem.LineColor(writer, GetLineColor());
            SaveObjectItem.LineThick(writer, this.GetBorderThick());
            SaveObjectItem.FillColor(writer, GetFillColor());
            ObjectSaveFont(writer);
        }*/


    }
}

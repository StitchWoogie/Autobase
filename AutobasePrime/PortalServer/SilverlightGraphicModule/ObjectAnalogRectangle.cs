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
using AutoLibLocal;
using System.Collections.Generic;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectArgsAnalogRectangle
    {
        public BrushPublic colorOff = new BrushPublic();
        public BrushPublic colorOn = new BrushPublic();
        public int nBarDir;
    }

    public class VIEW_RANGE_STRUCT
    {
        public sbyte flag;
        public double fBase;
        public double fFull;
    }

    public class GUIDE_LINE_STRUCT
    {
        public sbyte method;
        public int devideBig;
        public int devideSmall;
        public int line_length;
        public Color colorBig;
        public Color colorSmall;
        public sbyte bLevelString;	// 레벨표시에 문자가 표시한다.
    }

    /// <summary>
    /// Summary description for ObjectAnalogString.
    /// </summary>

    public class ObjectAnalogRectangle : ObjectTag
    {
        ObjectArgsAnalogRectangle objArgs;
        VIEW_RANGE_STRUCT viewRange;
        GUIDE_LINE_STRUCT guideLine;

        public ObjectArgsAnalogRectangle ObjectArgs
        {
            get
            {
                return objArgs;
            }
            set
            {
                objArgs = value;
            }
        }

        public GUIDE_LINE_STRUCT GuideLine
        {
            get
            {
                return guideLine;
            }
            set
            {
                guideLine = value;
            }
        }

        public VIEW_RANGE_STRUCT ViewRange
        {
            get
            {
                return viewRange;
            }
            set
            {
                viewRange = value;
            }
        }

        public ObjectAnalogRectangle(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsAnalogRectangle args, VIEW_RANGE_STRUCT view, GUIDE_LINE_STRUCT guide)
            : base(ocp, rect, eid, general, lf, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.AnalogRectangle;
            objArgs = args;
            viewRange = view;
            guideLine = guide;

            FillColor = args.colorOn;
            BackColor = args.colorOff;

            PrepareObject(parent_canvas);
        }

        Grid childBack;
        Grid childFill;

        List<Line> lineSmall = new List<Line>();
        List<Line> lineBig = new List<Line>();
        List<TextBlock> textBig = new List<TextBlock>();

        void PrepareObject(Canvas parent_canvas)
        {
            Canvas child = new Canvas();

            childBack = new Grid();
            childBack.Background = ObjectRectangle.MakePublicBrush(RunColorBack);
            //childBack.Margin = GetBarThickness();
            child.Children.Add(childBack);

            childFill = new Grid();
            childFill.Background = ObjectRectangle.MakePublicBrush(RunColorFill);
            //childBack.Margin = GetBarThickness();
            childBack.Children.Add(childFill);

            for (int i = 0; i <= guideLine.devideSmall; i++)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(guideLine.colorSmall);
                line.StrokeThickness = 1;
                lineSmall.Add(line);

                child.Children.Add(line);
            }

            for (int i = 0; i <= guideLine.devideBig; i++)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(guideLine.colorBig);
                line.StrokeThickness = 1;
                lineBig.Add(line);

                child.Children.Add(line);

                if (guideLine.bLevelString == 1)
                {
                    TextBlock text = new TextBlock();
                    text.Foreground = new SolidColorBrush(guideLine.colorBig);
                    textBig.Add(text);

                    child.Children.Add(text);
                }
            }

            child.SizeChanged += new SizeChangedEventHandler(child_SizeChanged);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        void OnSizeChanged()
        {
            FrameworkElement child = GetShapeOriginal();

            if (child.ActualHeight == 0 || child.ActualWidth == 0)  // 아직 윈도우가 초기화가 되지 않았는데 태그를 다 읽은 경우
            {
                return;
            }

            int bx1, by1, bx2, by2;
            GetBarZone(out bx1, out by1, out bx2, out by2);

            Canvas.SetLeft(childBack, bx1);
            Canvas.SetTop(childBack, by1);

            childBack.Width = bx2 - bx1 + 1;
            childBack.Height = by2 - by1 + 1;

            if (guideLine.method > 0)
            {
                int x1 = 0, y1 = 0, x2 = (int)(child.Width - 1), y2 = (int)(child.Height - 1);

                RECT r = new RECT();

                double fBase = 0, fFull = 0;

                GetFullBase(ref fFull, ref fBase);
                string buf;

                if (guideLine.method == 1)
                {
                    if (objArgs.nBarDir == 0 || objArgs.nBarDir == 1)
                    {	// default : 아래에서 위로
                        x2 = bx1 - 1;
                    }
                    else
                    {
                        y2 = by1 - 1;
                    }
                }
                else
                {
                    if (objArgs.nBarDir == 0 || objArgs.nBarDir == 1)
                    {	// default : 아래에서 위로
                        x1 = bx2 + 1;
                    }
                    else
                    {
                        y1 = by2 + 1;
                    }
                }

                int i;
                int pos;

                if (objArgs.nBarDir == 0 || objArgs.nBarDir == 1)
                {	// default : 아래에서 위로
                    for (i = 0; i <= guideLine.devideSmall; i++)
                    {
                        pos = y2 - (y2 - y1) * i / guideLine.devideSmall;

                        lineSmall[i].X1 = x1 + 1;
                        lineSmall[i].Y1 = pos;
                        lineSmall[i].X2 = x2;
                        lineSmall[i].Y2 = pos;
                        //g.DrawLine(hPenSmall, x1 + 1, pos, x2, pos);
                    }
                }
                else
                {
                    for (i = 0; i <= guideLine.devideSmall; i++)
                    {
                        pos = x2 - (x2 - x1) * i / guideLine.devideSmall;

                        lineSmall[i].X1 = pos;
                        lineSmall[i].Y1 = y1 + 1;
                        lineSmall[i].X2 = pos;
                        lineSmall[i].Y2 = y2;
                        //g.DrawLine(hPenSmall, pos, y1 + 1, pos, y2);
                    }
                }

                //-------------------
                // 큰 눈금을 그린다.
                //-------------------

                if (objArgs.nBarDir == 0 || objArgs.nBarDir == 1)
                {	// default : 아래에서 위로
                    for (i = 0; i <= guideLine.devideBig; i++)
                    {
                        if (objArgs.nBarDir == 1)
                            pos = y1 + (y2 - y1) * i / guideLine.devideBig;
                        else
                            pos = y2 - (y2 - y1) * i / guideLine.devideBig;

                        lineBig[i].X1 = x1;
                        lineBig[i].Y1 = pos;
                        lineBig[i].X2 = x2 + 1;
                        lineBig[i].Y2 = pos;
                        // g.DrawLine(hPenBig, x1, pos, x2 + 1, pos);

                        if (guideLine.bLevelString == 1)
                        {
                            TagAiClass ai = null;
                            float fTagDisplayFormat;

                            if (nTagType == 0)
                            {	// AI
                                ai = TagLib.GetStructAI(sTagName, ref nTagPos);
                            }

                            if (ai != null) fTagDisplayFormat = ai.fDisplayFormat;
                            else fTagDisplayFormat = (float)10.2;

                            if (guideLine.method == 1)
                            {
                                r.right = x1;
                            }
                            else
                            {
                                r.left = x2 + 1;
                            }

                            if (ai != null)
                            {
                                double val = (i * (fFull - fBase)) / guideLine.devideBig + fBase;
                                val = TagUtil.GetDisplayValue(ai, val);
                                buf = TagUtil.AiValueToStringOnlyPoint(ai, val);
                            }
                            else
                            {
                                buf = "?";
                            }

                            textBig[i].Text = buf;

                            if (guideLine.method == 1)
                            {
                                Canvas.SetLeft(textBig[i], r.right - textBig[i].ActualWidth);
                                Canvas.SetTop(textBig[i], pos - textBig[i].ActualHeight / 2);
                            }
                            else
                            {
                                Canvas.SetLeft(textBig[i], r.left);
                                Canvas.SetTop(textBig[i], pos - textBig[i].ActualHeight / 2);
                            }
                        }
                    }
                }
                else
                {
                    for (i = 0; i <= guideLine.devideBig; i++)
                    {

                        if (objArgs.nBarDir == 2)
                            pos = x1 + (x2 - x1) * i / guideLine.devideBig;
                        else
                            pos = x2 - (x2 - x1) * i / guideLine.devideBig;

                        lineBig[i].X1 = pos;
                        lineBig[i].Y1 = y1;
                        lineBig[i].X2 = pos;
                        lineBig[i].Y2 = y2 + 1;
                        //g.DrawLine(hPenBig, pos, y1, pos, y2 + 1);

                        if (guideLine.bLevelString == 1)
                        {

                            if (guideLine.method == 1)
                            {
                                r.bottom = y1;
                            }
                            else
                            {
                                r.top = y2 + 1;
                            }

                            TagAiClass ai = null;

                            if (nTagType == 0)
                            {	// AI
                                ai = TagLib.GetStructAI(sTagName, ref nTagPos);
                            }

                            if (ai != null)
                            {
                                double val = (i * (fFull - fBase)) / guideLine.devideBig + fBase;
                                val = TagUtil.GetDisplayValue(ai, val);
                                buf = TagUtil.AiValueToStringOnlyPoint(ai, val);
                            }
                            else
                            {
                                buf = "?";
                            }

                            textBig[i].Text = buf;

                            if (guideLine.method == 1)
                            {
                                Canvas.SetLeft(textBig[i], pos - textBig[i].ActualWidth / 2);
                                Canvas.SetTop(textBig[i], r.bottom - textBig[i].ActualHeight);
                            }
                            else
                            {
                                Canvas.SetLeft(textBig[i], pos - textBig[i].ActualWidth / 2);
                                Canvas.SetTop(textBig[i], r.top);
                            }
                        }
                    }
                }
            }
        }

        void child_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnSizeChanged();
        }

        public override void OnTagFileReaded()
        {
            base.OnTagFileReaded();

            OnSizeChanged();
        }

        // 채우는 사각형의 크기를 다시 결정한다.
        void ReCalcLevelBar()
        {
            double fBase = 0, fFull = 0;
            double curr;
            int mid;
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            GetFullBase(ref fFull, ref fBase);

            if (nTagType == EnumTagType.AI)
            {	// AI
                curr = TagUtil.GetDisplayValue(ai, ai.curr);
            }
            else
            {
                curr = (float)50;
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                curr = (fFull - fBase) / 2 + fBase;
            }

            int x1, y1, x2, y2;
            x1 = 0;
            y1 = 0;
            x2 = (int)childBack.ActualWidth - 1;
            y2 = (int)childBack.ActualHeight - 1;

            if (objArgs.nBarDir == 0)
            {	// default : 아래에서 위로
                mid = (int)((y2 -y1) * (curr - fBase) / (fFull - fBase));
                mid = y2 - mid;
                if (mid > y2) mid = y2;
                if (mid < y1) mid = y1;

                childFill.Margin = new Thickness(0, GetViewPosY(mid), 0, 0);
            }
            else if (objArgs.nBarDir == 1)
            {	// 위에서 아래로
                mid = (int)((y2 - y1) * (curr - fBase) / (fFull - fBase));
                mid = y2 - mid;
                if (mid > y2) mid = y2;
                if (mid < y1) mid = y1;

                childFill.Margin = new Thickness(0, 0, 0, GetViewPosY(mid));
            }
            else if (objArgs.nBarDir == 2)
            {	// 왼쪽에서 오른쪽으로
                mid = (int)((x2 - x1) * (curr - fBase) / (fFull - fBase));
                mid = x2 - mid;
                if (mid > x2) mid = x2;
                if (mid < x1) mid = x1;

                childFill.Margin = new Thickness(0, 0, GetViewPosX(mid), 0);
            }
            else
            {	// 오른쪽에서 왼쪽으로
                mid = (int)((x2 - x1) * (curr - fBase) / (fFull - fBase));
                mid = x2 - mid;
                if (mid > x2) mid = x2;
                if (mid < x1) mid = x1;

                childFill.Margin = new Thickness(GetViewPosX(mid), 0, 0, 0);
            }

        }

        double fOldCurr = 1.2345678901234567890;  // 기본적으로 채워져 있으므로 다시 계산할 수 있도록 초기값을 존재하지 않는 값을 사용한다.

        public override void EventTimerObject(UserControl form)
        {
            base.EventTimerObject(form);

            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (fOldCurr != ai.curr)
            {
                fOldCurr = ai.curr;
                ReCalcLevelBar();
            }
        }

        void GetFullBase(ref double fFull, ref double fBase)
        {
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (viewRange.flag == 1)
            {
                fBase = viewRange.fBase;
                fFull = viewRange.fFull;
            }
            else
            {
                if (nTagType == 0)
                {	// AI
                    fBase = ai.view_base;
                    fFull = ai.view_full;
                }
                else
                {
                    fBase = (float)0;
                    fFull = (float)100;
                }
            }

            if (fFull - fBase == 0)
            {	// void divide by zero
                fFull = (float)100;
                fBase = (float)0;
            }
        }

        Thickness GetBarThickness()
        {
            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;

            if (guideLine.method == 0) return new Thickness();	// 전체를 막대로 사용

            if (objArgs.nBarDir == 0 || objArgs.nBarDir == 1)
            {	// default : 아래에서 위로
                if (guideLine.method == 1)
                {
                    x1 = guideLine.line_length;
                }
                else
                {
                    x2 = guideLine.line_length;
                }
            }
            else
            {	// 오른쪽에서 왼쪽으로
                if (guideLine.method == 1)
                {
                    y1 = guideLine.line_length;
                }
                else
                {
                    y2 = guideLine.line_length;
                }
            }

            return new Thickness(x1, y1, x2, y2);
        }

        void GetBarZone(out int x1, out int y1, out int x2, out int y2)
        {
            FrameworkElement child = GetShapeOriginal();

            x1 = 0;
            y1 = 0;
            x2 = (int)(child.ActualWidth - 1);
            y2 = (int)(child.ActualHeight - 1);

            if (guideLine.method == 0) return;	// 전체를 막대로 사용

            if (objArgs.nBarDir == 0 || objArgs.nBarDir == 1)
            {	// default : 아래에서 위로
                if (guideLine.method == 1)
                {
                    x1 = guideLine.line_length;
                }
                else
                {
                    x2 = x2 - guideLine.line_length;
                }
            }
            else
            {	// 오른쪽에서 왼쪽으로
                if (guideLine.method == 1)
                {
                    y1 = guideLine.line_length;
                }
                else
                {
                    y2 = y2 - guideLine.line_length;
                }
            }
        }

    }
}

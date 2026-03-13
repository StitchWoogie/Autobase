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

namespace SilverlightGraphicModule
{
    //[Serializable]
    public class ObjectArgsAnalogString
    {
        public int nDisplayValue;
        public int nBoxUse;
        public Color colorText;
        public BrushPublic colorBack = new BrushPublic();
        public string sDisplayFormat = "";	// 없으면 태그의 기본, {0} 은 자동 등
    }

    /// <summary>
    /// Summary description for ObjectAnalogString.
    /// </summary>
    //[Serializable]
    public class ObjectAnalogString : ObjectTag
    {
        ObjectArgsAnalogString objArgs;

        public ObjectArgsAnalogString ObjectArgs
        {
            set
            {
                objArgs.nBoxUse = value.nBoxUse;
                objArgs.nDisplayValue = value.nDisplayValue;
                objArgs.sDisplayFormat = value.sDisplayFormat;
            }
            get
            {
                ObjectArgsAnalogString args = new ObjectArgsAnalogString();
                args.nBoxUse = objArgs.nBoxUse;
                args.nDisplayValue = objArgs.nDisplayValue;
                args.sDisplayFormat = objArgs.sDisplayFormat;
                return args;
            }
        }

        TextBlock childTextBlock;

        public ObjectAnalogString(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsAnalogString args)
            : base(ocp, rect, eid, general, lf, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.AnalogString;
            objArgs = args;
            SetTextColor(args.colorText);
            SetBackColor(args.colorBack);

            Grid child = new Grid();

            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            if(objArgs.nBoxUse != 3)
                child.Background = ObjectRectangle.MakePublicBrush(RunColorBack);

            childTextBlock = new TextBlock();
            MakeFont(childTextBlock);
            childTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
            childTextBlock.VerticalAlignment = VerticalAlignment.Center;
            childTextBlock.Foreground = new SolidColorBrush(RunColorText);

            child.Children.Add(childTextBlock);

            if (objArgs.nBoxUse == 1)
            {
                AddLinePopBox2(child);
                childTextBlock.Margin = new Thickness(1, 1, 1, 1);
            }
            else if (objArgs.nBoxUse == 2)
            {
                AddLinePushBox2(child);
                childTextBlock.Margin = new Thickness(1, 1, 1, 1);
            }
            
            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        public static void AddLinePopBox2(Grid child)
        {
            Rectangle r;

            r = new Rectangle();
            r.Height = 0.5;
            r.VerticalAlignment = VerticalAlignment.Top;
            r.Stroke = new SolidColorBrush(Colors.White);
            child.Children.Add(r);

            r = new Rectangle();
            r.Width = 0.5;
            r.HorizontalAlignment = HorizontalAlignment.Left;
            r.Stroke = new SolidColorBrush(Colors.White);
            child.Children.Add(r);

            r = new Rectangle();
            r.Height = 0.5;
            r.VerticalAlignment = VerticalAlignment.Bottom;
            r.Stroke = new SolidColorBrush(Colors.DarkGray);
            child.Children.Add(r);

            r = new Rectangle();
            r.Width = 0.5;
            r.HorizontalAlignment = HorizontalAlignment.Right;
            r.Stroke = new SolidColorBrush(Colors.DarkGray);
            child.Children.Add(r);
        }

        public static void AddLinePushBox2(Grid child)
        {
            Rectangle r;

            r = new Rectangle();
            r.Height = 0.5;
            r.VerticalAlignment = VerticalAlignment.Top;
            r.Stroke = new SolidColorBrush(Colors.DarkGray);
            child.Children.Add(r);

            r = new Rectangle();
            r.Width = 0.5;
            r.HorizontalAlignment = HorizontalAlignment.Left;
            r.Stroke = new SolidColorBrush(Colors.DarkGray);
            child.Children.Add(r);

            r = new Rectangle();
            r.Height = 0.5;
            r.VerticalAlignment = VerticalAlignment.Bottom;
            r.Stroke = new SolidColorBrush(Colors.White);
            child.Children.Add(r);

            r = new Rectangle();
            r.Width = 0.5;
            r.HorizontalAlignment = HorizontalAlignment.Right;
            r.Stroke = new SolidColorBrush(Colors.White);
            child.Children.Add(r);
        }

        public override void EventTimerObject(UserControl form)
        {
            base.EventTimerObject(form);

            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            double curr;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (objArgs.nDisplayValue == 1)
                    curr = ai.fSumTotal;
                else
                    curr = ai.curr;
            }
            else
            {
                curr = 12345678.12345678;
            }

            string buf;
            if (objArgs.sDisplayFormat.Length > 0)	// 사용자 형식
            {
                try
                {
                    buf = String.Format(objArgs.sDisplayFormat, curr);
                }
                catch
                {
                    objArgs.sDisplayFormat = "";	// 오류가 생기면 형식을 제거한다. (계속 문제를 발생 시킬 수 있다.)
                    buf = TagUtil.AiValueToStringOnlyPoint(ai, curr);
                }
            }
            else // 기본 형식
                buf = TagUtil.AiValueToStringOnlyPoint(ai, curr);

            if (buf != childTextBlock.Text)
            {
                childTextBlock.Text = buf;
            }
        }

        protected override void TextColorChanged(Color color)
        {
            if (childTextBlock == null) return;

            childTextBlock.Foreground = new SolidColorBrush(color);
        }

        protected override void BackColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            if (child.GetType() == typeof(Grid))
                ((Grid)child).Background = new SolidColorBrush(color);

        }

    }
}

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
    public class ObjectArgsText
    {
        public string text;
        public Color textColor;
        public TEXT_ALIGN align = new TEXT_ALIGN();
        public bool formatFlagDirectionVertical;
        public bool formatFlagNoWrap;
    }
    /// <summary>
    /// Summary description for ObjectText.
    /// </summary>
    
    public class ObjectText : ObjectExpand
    {
        ObjectArgsText objArgs;

        public ObjectArgsText ObjectArgs
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

        public ObjectText(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsText args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Text;
            objArgs = args;
            TextColor = args.textColor;
            SetText(args.text);

            // Text Block 에는 수직 정렬이 없으므로 Border 를 안에 넣고 수직/중앙으로 정렬한다.

            Border child = new Border();

            TextBlock tb = new TextBlock();
            MakeFont(tb);
            
            //tb.HorizontalAlignment = HorizontalAlignment.Center;
            //tb.VerticalAlignment = VerticalAlignment.Top;

            // 2015-9-8 수정
            if (objArgs.align.x == 1) tb.TextAlignment = TextAlignment.Center;
            else if (objArgs.align.x == 2) tb.TextAlignment = TextAlignment.Right;
            else tb.TextAlignment = TextAlignment.Left;

            if (objArgs.align.y == 1) tb.VerticalAlignment = VerticalAlignment.Center;
            else if (objArgs.align.y == 2) tb.VerticalAlignment = VerticalAlignment.Bottom;
            else tb.VerticalAlignment = VerticalAlignment.Top;

            tb.Foreground = new SolidColorBrush(RunColorText);
            //child.TextWrapping = TextWrapping.NoWrap;
            tb.Text = objArgs.text;

            child.Child = tb;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        void SetText(string str)
        {
            objArgs.text = str;
        }

        protected override void TextColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            TextBlock tb = (TextBlock)((Border)child).Child;
            tb.Foreground = new SolidColorBrush(color);
        }

        protected override void OnObjectSetText(string text)
        {
            objArgs.text = text;

            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            TextBlock tb = (TextBlock)((Border)child).Child;
            tb.Text = text;
        }
    }
}

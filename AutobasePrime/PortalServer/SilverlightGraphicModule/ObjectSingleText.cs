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
    public class ObjectArgsSingleText
    {
        public string text;
        public Color textColor;
    }

    /// <summary>
    /// Summary description for ObjectSingleText.
    /// </summary>
    public class ObjectSingleText : ObjectExpand
    {
        ObjectArgsSingleText objArgs;

        public ObjectArgsSingleText ObjectArgs
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

        UserControl formParent;
        TextBlock childControl = new TextBlock();

        public ObjectSingleText(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsSingleText args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.SingleText;
            objArgs = args;
            TextColor = args.textColor;
            //SetText(args.text);
            formParent = form;

            TextBlock child = childControl;
            MakeFont(child);

            child.HorizontalAlignment = HorizontalAlignment.Left;
            child.VerticalAlignment = VerticalAlignment.Top;
            child.Foreground = new SolidColorBrush(RunColorText);
            child.TextWrapping = TextWrapping.NoWrap;
            child.Text = objArgs.text;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            DrawClass draw = new DrawClass();

            Font font = MakeFont();
            StringFormat format = new StringFormat();
            Brush brush = new SolidBrush(RunColorText);

            format.Alignment = StringAlignment.Near;
            //format.LineAlignment = StringAlignment.Center;

            g.DrawString(objArgs.text, font, brush, x1, y1, format);
            //draw.WinDrawText(g, x1, y1, x2-x1+1, y2-y1+1, objArgs.text, lRunColorLine, lRunColorFill, font, format);
        }

        
        // Text의 값을 바꾸었거나 폰트를 바꾸었을때는 사각형의 크기를 다시 계산하여야 한다.
        public void RecalcRectSize(Graphics g)
        {
            Font font = MakeFont100();
            SizeF size = g.MeasureString(objArgs.text, font);
            nRight = (int)(nLeft + size.Width - 1);
            nBottom = (int)(nTop + size.Height - 1);
        }

        
        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.String(writer, objArgs.text);
            ObjectSaveFont(writer);
        }

        public override void UpdateZone(int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (Math.Abs(x2 - x1) == Math.Abs(nRight - nLeft))
            {
                base.UpdateZone(x1, y1, x2, y2);
                return;
            }

            float size = logFont.lfHeight;

            float gab = 1.0f;
            Font font = new Font(logFont.lfFaceName, size, logFont.style);

            Graphics g = formParent.CreateGraphics();

            if (Math.Abs(x2 - x1) < Math.Abs(nRight - nLeft)) // 작아졌다.
            {
                while (true)
                {
                    size -= gab;
                    if (size < 1)
                    {
                        size += gab;
                        break;
                    }

                    font = new Font(logFont.lfFaceName, size, logFont.style);
                    SizeF sizef = g.MeasureString(objArgs.text, font);

                    if (sizef.Width <= Math.Abs(x2 - x1)) break;
                }
            }
            else	// 커졌다.
            {
                while (true)
                {
                    size += gab;
                    if (size > 1000)
                    {
                        size -= gab;
                        break;
                    }

                    font = new Font(logFont.lfFaceName, size, logFont.style);
                    SizeF sizef = g.MeasureString(objArgs.text, font);

                    if (sizef.Width >= Math.Abs(x2 - x1)) break;
                }
            }

            logFont.lfHeight = size;

            nLeft = x1;
            nTop = y1;

            RecalcRectSize(g);
        }*/

        protected override void OnObjectSetText(string text)
        {
            objArgs.text = text;
            childControl.Text = text;

            //Graphics g = form.CreateGraphics();
            //RecalcRectSize(g);
        }

        protected override void TextColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            ((TextBlock)child).Foreground = new SolidColorBrush(color);
        }

    }
}

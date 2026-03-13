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
    public class TEXT_ALIGN
    {
        public byte x;	// 0 - left, 1 - center, 2 - right
        public byte y;	// 0 - top,  1 - vcenter, 2 - bottom
    }

    public class ObjectArgsStringString
    {
        public int nBoxUse;
        public Color colorText;
        public BrushPublic colorBack = new BrushPublic();
    }

    /// <summary>
    /// Summary description for ObjectAnalogString.
    /// </summary>
    public class ObjectStringString : ObjectTag
    {
        ObjectArgsStringString objArgs;
        TEXT_ALIGN textAlign;

        public ObjectArgsStringString ObjectArgs
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

        public TEXT_ALIGN TextAlign
        {
            get
            {
                return textAlign;
            }
            set
            {
                textAlign = value;
            }
        }

        TextBlock childTextBlock;

        public ObjectStringString(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsStringString args, TEXT_ALIGN align)
            : base(ocp, rect, eid, general, lf, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.StringString;
            objArgs = args;
            textAlign = align;
            SetTextColor(args.colorText);
            SetBackColor(args.colorBack);

            Grid child = new Grid();

            if (nLineOption == 0 && nFillOption == 0)
            {
                nLineOption = 1;
                nFillOption = 1;
            }

            if (objArgs.nBoxUse != 3)
                child.Background = ObjectRectangle.MakePublicBrush(RunColorBack);

            childTextBlock = new TextBlock();
            MakeFont(childTextBlock);

            if (textAlign.x == 0)
                childTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            else if (textAlign.x == 1)
                childTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            else
                childTextBlock.HorizontalAlignment = HorizontalAlignment.Right;

            if (textAlign.y == 0)
                childTextBlock.VerticalAlignment = VerticalAlignment.Top;
            else if (textAlign.y == 1)
                childTextBlock.VerticalAlignment = VerticalAlignment.Center;
            else
                childTextBlock.VerticalAlignment = VerticalAlignment.Bottom;

            childTextBlock.Foreground = new SolidColorBrush(RunColorText);

            child.Children.Add(childTextBlock);

            if (objArgs.nBoxUse == 1)
            {
                ObjectAnalogString.AddLinePopBox2(child);
                childTextBlock.Margin = new Thickness(1, 1, 1, 1);
            }
            else if (objArgs.nBoxUse == 2)
            {
                ObjectAnalogString.AddLinePushBox2(child);
                childTextBlock.Margin = new Thickness(1, 1, 1, 1);
            }

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }


        public override void EventTimerObject(UserControl form)
        {
            base.EventTimerObject(form);

            TagStClass st = TagLib.GetStructST(sTagName, ref nTagPos);

            if (st.curr != childTextBlock.Text)
            {
                childTextBlock.Text = st.curr;
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

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            string buf;

            TagStClass st = TagLib.GetStructST(sTagName, ref nTagPos);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
                buf = sTagName;
            else
                buf = st.curr;

            if (objArgs.nBoxUse == 0)
                DrawClass.gcls(g, x1, y1, x2, y2, RunColorBack);
            else if (objArgs.nBoxUse == 1)
                DrawClass.PopBox2(g, x1, y1, x2, y2, RunColorBack);
            else if (objArgs.nBoxUse == 2)
                DrawClass.PushBox2(g, x1, y1, x2, y2, RunColorBack);
            else { }

            Font font = MakeFont();
            StringFormat format = new StringFormat();

            if (textAlign.x == 0)
                format.Alignment = StringAlignment.Near;
            else if (textAlign.x == 1)
                format.Alignment = StringAlignment.Center;
            else
                format.Alignment = StringAlignment.Far;

            if (textAlign.y == 0)
                format.LineAlignment = StringAlignment.Near;
            else if (textAlign.y == 1)
                format.LineAlignment = StringAlignment.Center;
            else
                format.LineAlignment = StringAlignment.Far;

            format.FormatFlags |= StringFormatFlags.NoWrap;

            DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, buf, RunColorText, RunColorBack, font, format);

            DisplayInactive(g, x1, y1, x2, y2);
        }

        public override void EventTimerObject(System.Windows.Forms.Form form)
        {
            TagStClass st = TagLib.GetStructST(sTagName, ref nTagPos);

            st.bNeedDataCurr = true;
        }

        public override void EventTag(System.Windows.Forms.Form form, COMM_EVENT_STRUCT tagevent)
        {
            if (tagevent.tag_type != EnumTagType.ST) return;
            if (String.Compare(tagevent.tag, sTagName) != 0) return;

            InvalidateObject(form);
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            ObjectSaveTag(writer);
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            SaveObjectItem.BackBox(writer, objArgs.nBoxUse);
            SaveObjectItem.TextAlign(writer, textAlign);
            ObjectSaveFont(writer);
        }
        */

    }
}

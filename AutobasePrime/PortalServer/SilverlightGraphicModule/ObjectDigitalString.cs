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
    public class ObjectArgsDigitalString
    {
        public Color colorOff;
        public Color colorOn;
        public BrushPublic colorBack = new BrushPublic();
    }

    /// <summary>
    /// Summary description for ObjectAnalogString. 
    /// </summary>
    public class ObjectDigitalString : ObjectTag
    {
        public ObjectArgsDigitalString objArgs;

        TextBlock childTextBlock;

        public ObjectDigitalString(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsDigitalString args)
            : base(ocp, rect, eid, general, lf, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.DigitalString;
            objArgs = args;

            Grid child = new Grid();

            child.Background = ObjectRectangle.MakePublicBrush(objArgs.colorBack);

            childTextBlock = new TextBlock();
            MakeFont(childTextBlock);
            childTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            childTextBlock.VerticalAlignment = VerticalAlignment.Center;
            childTextBlock.Foreground = new SolidColorBrush(objArgs.colorOff);

            child.Children.Add(childTextBlock);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        int nOldValue = 0;

        public override void EventTimerObject(UserControl form)
        {
            base.EventTimerObject(form);

            TagDiClass di = TagLib.GetStructDI(sTagName, ref nTagPos);

            int curr;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                curr = di.curr;
            }
            else
            {
                curr = 1;
            }

            string buf;
            if (curr == 1)
            {
                buf = di.desON;
            }
            else
            {
                buf = di.desOFF;
            }

            if (buf != childTextBlock.Text)
            {
                childTextBlock.Text = buf;
            }

            if (curr != nOldValue)
            {
                if(curr == 1)
                    childTextBlock.Foreground = new SolidColorBrush(objArgs.colorOn);
                else
                    childTextBlock.Foreground = new SolidColorBrush(objArgs.colorOff);

                nOldValue = curr;
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

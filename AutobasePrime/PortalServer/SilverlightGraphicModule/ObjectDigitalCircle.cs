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
    public class ObjectArgsDigitalCircle
    {
        public Color colorOff;
        public Color colorOn;
    }

    /// <summary>
    /// Summary description for ObjectAnalogString.
    /// </summary>

    //[Serializable]
    public class ObjectDigitalCircle : ObjectTag
    {
        public ObjectArgsDigitalCircle objArgs;

        public ObjectDigitalCircle(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsDigitalCircle args)
            : base(ocp, rect, eid, general, null, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.DigitalCircle;
            objArgs = args;

            Ellipse child = new Ellipse();

            child.Stroke = new SolidColorBrush(Colors.Black);
            child.StrokeThickness = 1;

            child.Fill = new SolidColorBrush(objArgs.colorOff);

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

            if (curr != nOldValue)
            {
                Ellipse child = (Ellipse)GetShapeOriginal();

                if (curr == 1)
                    child.Fill = new SolidColorBrush(objArgs.colorOn);
                else
                    child.Fill = new SolidColorBrush(objArgs.colorOff);

                nOldValue = curr;
            }
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            int width = x2 - x1;
            int height = y2 - y1;
            int curr;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                TagDiClass di = TagLib.GetStructDI(sTagName, ref nTagPos);
                curr = di.curr;
            }
            else
            {
                curr = 1;
            }

            Brush brush = new SolidBrush(curr == 1 ? objArgs.colorOn : objArgs.colorOff);
            g.FillEllipse(brush, x1, y1, width, height);

            Pen pen = new Pen(Color.Black, 1);
            g.DrawEllipse(pen, x1, y1, width, height);
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            ObjectSaveTag(writer);
            SaveObjectItem.OnColor(writer, objArgs.colorOn);
            SaveObjectItem.OffColor(writer, objArgs.colorOff);
        }*/

    }
}

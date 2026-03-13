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
    //[Serializable]
    public class ObjectLine : ObjectExpand
    {
        public ObjectLine(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick)
            : base(ocp, rect, eid, null, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Line;
            SetLineColor(lcolor);
            SetFillColor(fcolor);
            nLineOption = loption;
            SetBorderThick(lthick);

            Line child = new Line();

            child.Stroke = new SolidColorBrush(RunColorLine);
            child.StrokeThickness = nRunThickLine;
            child.StrokeDashArray = ObjectRectangle.GetDashStyle(nLineOption);

            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;

            GetViewZone(ref x1, ref y1, ref x2, ref y2);

            //child.X1 = x1;
            //child.Y1 = y1;
            //child.X2 = x2;
            //child.Y2 = y2;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);

            //MoveShape();
            OnMove(x1, y1, x2, y2);
        }

        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            FrameworkElement shape = GetShapeOriginal();
            if (shape == null) return;
            Line child = (Line)shape;
            child.X1 = x1;
            child.Y1 = y1;
            child.X2 = x2;
            child.Y2 = y2;
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            DrawClass draw = new DrawClass();

            Pen pen = new Pen(RunColorLine, bthick);

            pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);

            g.DrawLine(pen, x1, y1, x2, y2);
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.LineColor(writer, GetLineColor());
            //SaveObjectItem.FillColor(writer, GetBackColor());
            SaveObjectItem.LineThick(writer, GetBorderThick());
            SaveObjectItem.LineOption(writer, nLineOption);
            //SaveObjectItem.FillOption(writer, nFillOption);
        }*/
    }
}

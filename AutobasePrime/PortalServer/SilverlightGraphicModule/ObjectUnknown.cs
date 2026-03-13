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
    public class ObjectUnknown : ObjectExpand
    {
        string sCommand;

        public ObjectUnknown(ObjectCommonProperty ocp, Canvas parent_canvas, string command, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general)
            : base(ocp, rect, eid, null, general)
        {
            enumObjectType = EnumObjectType.Unknown;
            sCommand = command;

            TextBox child = new TextBox();

            //Canvas.SetLeft(child, rect.left);
            //Canvas.SetTop(child, rect.top);
            //child.Width = rect.right - rect.left + 1;
            //child.Height = rect.bottom - rect.top + 1;

            //myEllipse.Stroke = new SolidColorBrush(Colors.Black);
            //myEllipse.StrokeThickness = 1;

            child.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
            child.Text = sCommand;// "Unknown object : " + sCommand;
            child.TextWrapping = TextWrapping.Wrap;
            child.TextAlignment = TextAlignment.Center;
            child.IsReadOnly = true;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);

            MoveShape();
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            DrawClass.PopBox2(g, x1, y1, x2, y2, Color.Red);

            Font font = this.MakeFont();
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            //align.
            DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, sCommand, Color.White, Color.Red, font, format);
        }

        public override void ObjectSave(CommaTextWriter writer)
        {

        }*/
    }
}

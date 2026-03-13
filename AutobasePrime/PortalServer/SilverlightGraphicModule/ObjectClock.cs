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
using SilverlightPublicControls;

namespace SilverlightGraphicModule
{
    public class ObjectArgsClock
    {
        public int type;
        public Color textColor;
        public BrushPublic backColor = new BrushPublic();
    }
    /// <summary>
    /// Summary description for ObjectClock.
    /// </summary>
    public class ObjectClock : ObjectExpand
    {
        ObjectArgsClock objArgs;
        string sDateString;
        DateTime dtOld = DateTime.Now;

        public ObjectArgsClock ObjectArgs
        {
            set
            {
                objArgs = value;
                MakeDateString();
            }
            get
            {
                return objArgs;
            }
        }

        public ObjectClock(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsClock args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Clock;
            objArgs = args;
            TextColor = args.textColor;
            BackColor = args.backColor;
            MakeDateString();

            if (args.type == 1)
            {
                SilverlightControlClock3 child = new SilverlightControlClock3();
                parent_canvas.Children.Add(child);
                SetShapeOriginal(child);
            }
            else
            {
                SilverlightControlDigitalClock child = new SilverlightControlDigitalClock();
                parent_canvas.Children.Add(child);
                SetShapeOriginal(child);
            }
            
            MoveShape();
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            //DrawClass draw = new DrawClass();

            if (objArgs.type == 0)
            {
                Font font = MakeFont();
                StringFormat format = new StringFormat();
                Brush brush = new SolidBrush(RunColorText);

                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                RectangleF rect = new RectangleF(x1, y1, x2 - x1 + 1, y2 - y1 + 1);

                DrawClass.gcls(g, x1, y1, x2, y2, RunColorBack);
                g.DrawString(sDateString, font, brush, rect, format);
            }
            else
            {
                int radius;
                int angle;
                int px, py;
                int cx, cy;
                Point[] p = new Point[4];
                //POINT p[4];

                if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1))
                {
                    radius = (int)((y2 - y1 - 6) * 0.9 / 2);
                }
                else
                {
                    radius = (int)((x2 - x1 - 6) * 0.9 / 2);
                }

                DrawClass.PopBox2(g, x1, y1, x2, y2, Color.FromArgb(0xC0, 0xc0, 0xc0));
                DrawClass.PushRectangle2(g, x1 + 3, y1 + 3, x2 - 3, y2 - 3);

                cx = x1 + (x2 - x1) / 2;
                cy = y1 + (y2 - y1) / 2;

                Brush brush = new SolidBrush(Color.White);
                Brush brushRed = new SolidBrush(Color.Red);
                Pen pen = new Pen(Color.Black);

                for (angle = 0; angle < 360; angle += 30)
                {		// 숫자판을 그린다.
                    MathLib.MathGetEllipsePoint(cx, cy, (float)radius, (float)radius, (float)angle, out px, out py);
                    if ((angle % 90) == 0)
                    {
                        g.FillEllipse(brush, px - 3, py - 3, 6, 6);
                        g.DrawEllipse(pen, px - 3, py - 3, 6, 6);
                    }
                    else
                    {
                        g.FillEllipse(brush, px - 2, py - 2, 4, 4);
                        g.DrawEllipse(pen, px - 2, py - 2, 4, 4);
                    }
                    g.FillRectangle(brushRed, px, py, 1, 1);
                }

                // 시침을 그린다.
                angle = (int)(360 - ((dtOld.Hour % 12) * 30 + dtOld.Minute * 0.5) + 270) % 360;
                MathLib.MathGetEllipsePoint(cx, cy, (float)(radius * 0.2), (float)(radius * 0.2), (float)angle, out px, out py);
                p[0].X = px;
                p[0].Y = py;
                angle = (int)(360 - ((dtOld.Hour % 12) * 30 + dtOld.Minute * 0.5) + 90) % 360 + 80;
                MathLib.MathGetEllipsePoint(cx, cy, (float)(radius * 0.16), (float)(radius * 0.16), (float)angle, out px, out py);
                p[1].X = px;
                p[1].Y = py;
                angle = (int)(360 - ((dtOld.Hour % 12) * 30 + dtOld.Minute * 0.5) + 90) % 360;
                MathLib.MathGetEllipsePoint(cx, cy, (float)(radius * 0.75), (float)(radius * 0.75), (float)angle, out px, out py);
                p[2].X = px;
                p[2].Y = py;
                angle = (int)(360 - ((dtOld.Hour % 12) * 30 + dtOld.Minute * 0.5) + 90) % 360 - 80;
                MathLib.MathGetEllipsePoint(cx, cy, (float)(radius * 0.16), (float)(radius * 0.16), (float)angle, out px, out py);
                p[3].X = px;
                p[3].Y = py;

                g.FillPolygon(brush, p);
                g.DrawPolygon(pen, p);

                // 분침을 그린다.
                angle = (360 - (dtOld.Minute * 6) + 270) % 360;
                MathLib.MathGetEllipsePoint(cx, cy, (float)(radius * 0.2), (float)(radius * 0.2), (float)angle, out px, out py);
                p[0].X = px;
                p[0].Y = py;
                angle = (360 - (dtOld.Minute * 6) + 90) % 360 + 80;
                MathLib.MathGetEllipsePoint(cx, cy, (float)(radius * 0.14), (float)(radius * 0.14), (float)angle, out px, out py);
                p[1].X = px;
                p[1].Y = py;
                angle = (360 - (dtOld.Minute * 6) + 90) % 360;
                MathLib.MathGetEllipsePoint(cx, cy, (float)(radius * 0.9), (float)(radius * 0.9), (float)angle, out px, out py);
                p[2].X = px;
                p[2].Y = py;
                angle = (360 - (dtOld.Minute * 6) + 90) % 360 - 80;
                MathLib.MathGetEllipsePoint(cx, cy, (float)(radius * 0.14), (float)(radius * 0.14), (float)angle, out px, out py);
                p[3].X = px;
                p[3].Y = py;

                g.FillPolygon(brush, p);
                g.DrawPolygon(pen, p);
            }
        }*/

        void MakeDateString()
        {
            dtOld = DateTime.Now;

            sDateString = String.Format("{0,00}:{1:00}:{2:00}", dtOld.Hour, dtOld.Minute, dtOld.Second);
        }
        /*
        public override void EventTimerObject(System.Windows.Forms.Form form)
        {
            DateTime dt = DateTime.Now;
            if (dt.Second != dtOld.Second)
            {
                MakeDateString();
                InvalidateObject(form);
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            writer.WriteLine("\tStringOption,{0},", objArgs.type);
            ObjectSaveFont(writer);
        }*/
    }
}

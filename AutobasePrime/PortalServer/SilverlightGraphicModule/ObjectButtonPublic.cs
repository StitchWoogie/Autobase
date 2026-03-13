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
    public class ObjectArgsButtonPublic
    {
        public string sText;
        public Color tcolor;
        public BrushPublic bcolor = new BrushPublic();
    }

    /// <summary>
    /// Summary description for ObjectButtonPublic.
    /// </summary>
    /// 
    public class ObjectButtonPublic : ObjectExpand
    {
        ObjectArgsButtonPublic objArgs;
        //bool bCaptureFlag = false;	// 마우스가 눌러져있는가?
        //bool bMouseInFlag = false;	// 마우스가 버턴영역 안에 있는가를 검사.

        public string Text
        {
            set
            {
                objArgs.sText = value;
            }
            get
            {
                return objArgs.sText;
            }
        }

        public ObjectButtonPublic(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general, ObjectArgsButtonPublic args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            objArgs = args;

            SetTextColor(args.tcolor);
            SetBackColor(args.bcolor);
            //SetTextFormat(DT_CENTER | DT_VCENTER | DT_SINGLELINE);

            Button child = new Button();
            child.Background = ObjectRectangle.MakePublicBrush(RunColorBack);
            child.Foreground = new SolidColorBrush(RunColorText);
            child.Content = objArgs.sText;
            child.Style = (Style)ocp.rootPage.Resources["ButtonSolidStyle"];
            child.Click += new RoutedEventHandler(child_Click);

            /*
            Style style = new Style(typeof(Button));
            style.Setters.Add(new Setter(Button..BackgroundProperty, new SolidColorBrush(Colors.Blue)));
            child.Style = style;*/

            MakeFont(child);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        protected virtual void OnClicked()
        {

        }

        void child_Click(object sender, RoutedEventArgs e)
        {
            OnClicked();
        }

        protected override bool IsNeedMouseHitTest()
        {
            return true;    // Button종류는 마우스 응답을 무조건 한다.
        }

        protected override void TextColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            ((Button)child).Foreground = new SolidColorBrush(color);
        }

        protected override void BackColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            ((Button)child).Background = new SolidColorBrush(color);
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            Font font = MakeFont();
            StringFormat format = new StringFormat();

            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.NoWrap;

            Rectangle r;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (bMouseInFlag == true)
                {	// 마우스가 버턴 바깥에서 안으로 들어왔다.
                    DrawClass.PushBox(g, x1, y1, x2, y2, RunColorBack);
                    r = new Rectangle(x1 + 1, y1 + 1, x2 - x1, y2 - y1);
                    DrawClass.DrawTextClip(g, objArgs.sText, font, new SolidBrush(RunColorText), r, format);
                }
                else
                {			// 마우스가 버턴 안에서 바깥으로 나갔다.
                    DrawClass.PopBox(g, x1, y1, x2, y2, RunColorBack);
                    r = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                    DrawClass.DrawTextClip(g, objArgs.sText, font, new SolidBrush(RunColorText), r, format);
                }
            }
            else
            {
                DrawClass.PopBox(g, x1, y1, x2, y2, RunColorBack);
                r = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                DrawClass.DrawTextClip(g, objArgs.sText, font, new SolidBrush(RunColorText), r, format);
            }
        }

        public override bool WmLeftButtonDown(UserControl form, MouseEventArgs e)
        {
         
            if (!CheckResponseOnVisible()) return false;

            int sx, sy;
            int x1 = 0, x2 = 0, y1 = 0, y2 = 0;

            sx = e.X;
            sy = e.Y;

            GetViewZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (sx < x1 || sy < y1 || sx > x2 || sy > y2) return false;

            InvalidateObject(form);
            bCaptureFlag = true;
            bMouseInFlag = true;		// 마우스가 버턴속에 있다.
            form.Capture = true;

            return true;
        }

        public override bool WmLeftButtonUp(UserControl form, MouseEventArgs e)
        {
            
            if (bCaptureFlag == false) return false;
            bCaptureFlag = false;
            form.Capture = false;

            if (bMouseInFlag == false) return false;

            bCaptureFlag = false;
            bMouseInFlag = false;

            InvalidateObject(form);

            return true;
        }

        public override bool WmMouseMove(UserControl form, MouseEventArgs e)
        {
            
            base.WmMouseMove(form, e);	// Button 도 MouseZone Display를  사용한다.

            if (bCaptureFlag == false) return false;	// 마우스가 눌러져 있지 않다.

            int sx, sy;
            int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
            bool mousein;

            sx = e.X;
            sy = e.Y;

            GetViewZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (sx >= x1 && sy >= y1 && sx <= x2 && sy <= y2) mousein = true;
            else mousein = false;

            if (bMouseInFlag == mousein) return true;	// 마우스 움직임에 변화를 줄필요가 없다.

            bMouseInFlag = mousein;

            InvalidateObject(form);

            return true;
        }*/
    }
}

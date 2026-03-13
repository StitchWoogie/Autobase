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
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectArgsDate
    {
        public int type;
        public Color textColor;
        public BrushPublic backColor = new BrushPublic();
    }
    /// <summary>
    /// Summary description for ObjectText.
    /// </summary>
    
    public class ObjectDate : ObjectExpand
    {
        ObjectArgsDate objArgs;
        string sDateString;
        DateTime dtOld = DateTime.Now; 

        public ObjectArgsDate ObjectArgs
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

        public ObjectDate(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsDate args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Text;
            objArgs = args;
            
            TextColor = args.textColor;
            SetBackColor(args.backColor);

            MakeDateString();
            //SetText(args.text);

            // Text Block 에는 수직 정렬이 없으므로 Border 를 안에 넣고 수직/중앙으로 정렬한다.

            Border child = new Border();

            TextBlock tb = new TextBlock();
            MakeFont(tb);

            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;

            tb.Foreground = new SolidColorBrush(RunColorText);
            tb.Text = sDateString;

            child.Child = tb;
            child.Background = ObjectRectangle.MakePublicBrush(RunColorBack);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        static string[] sWeekEnglish = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        static string[] sWeekHangul = { "일", "월", "화", "수", "목", "금", "토" };
        static string[] sWeekJapanese = { "日", "月", "火", "水", "木", "金", "土" };
        static string[] sWeekChinese = { "星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };

        void MakeDateString()
        {
            string buf;

            // 단위는 표시 형식
            dtOld = DateTime.Now;

            int val = objArgs.type % 10;

            if (val == 1)
            {
                buf = String.Format("{0,4:d}/{1,2:d}/{2,2:d}", dtOld.Year, dtOld.Month, dtOld.Day);
            }
            else
            {
                buf = String.Format("{0,4:d}-{1,2:d}-{2,2:d}", dtOld.Year, dtOld.Month, dtOld.Day);
            }

            // 10단위는 요일 표시
            val = (objArgs.type / 10) % 10;
            if (val == 1)
            {
                buf += "(";
                buf += sWeekEnglish[(int)dtOld.DayOfWeek];
                buf += ")";
            }
            else if (val == 2)
            {
                buf += "(";
                if (Tools.IsLangJapanese())
                    buf += sWeekJapanese[(int)dtOld.DayOfWeek];
                else if (Tools.IsLangChinese())
                    buf += sWeekChinese[(int)dtOld.DayOfWeek];
                else if (Tools.IsLangKorean())
                    buf += sWeekHangul[(int)dtOld.DayOfWeek];
                else
                    buf += sWeekEnglish[(int)dtOld.DayOfWeek];

                buf += ")";
            }
            else { }			// no display

            sDateString = buf;
        }

        protected override void TextColorChanged(Color color)
        {
            FrameworkElement child = (FrameworkElement)GetShapeOriginal();

            if (child == null) return;

            TextBlock tb = (TextBlock)((Border)child).Child;
            tb.Foreground = new SolidColorBrush(color);
        }


        public override void EventTimerObject(UserControl form)
        {
            DateTime dt = DateTime.Now;
            if (dt.Day != dtOld.Day)
            {
                MakeDateString();

                FrameworkElement child = (FrameworkElement)GetShapeOriginal();

                if (child == null) return;

                TextBlock tb = (TextBlock)((Border)child).Child;
                tb.Text = sDateString;
            }
        }
    }
}

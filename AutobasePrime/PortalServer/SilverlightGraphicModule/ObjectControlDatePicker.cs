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
using System.Collections.Generic;
using NetTools.OldDefine;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public class ObjectArgsControlDatePicker
    {
        //public string sTag;
        public EnumWindowStyleFlags dwWindowStyle;
        public Color textColor = Colors.Black;
        public BrushPublic backColor = new BrushSolid(Colors.White);
        public string sFormat;
    }

    public class ObjectControlDatePicker : ObjectExpand
    {
        ObjectArgsControlDatePicker objArgs;

        DatePicker childControl = new DatePicker();

        // 등록된 클래스 리스트
        static public List<object> arrayClassList = new List<object>();

        public ObjectArgsControlDatePicker ObjectArgs
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

        public ObjectControlDatePicker(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlDatePicker args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlDatePicker;
            objArgs = args;

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                /*
                form.Controls.Add(childControl);


                arrayClassList.Add(this);

                //editBox.TextChanged += new EventHandler(this.Event_TextChanged);
                childControl.ForeColor = this.RunColorText;
                childControl.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22
                childControl.Format = DateTimePickerFormat.Custom;
                childControl.CustomFormat = args.sFormat;

                base.SetToolTipOnChildWindow(childControl);

                OnVisible(ExpandCalcVisible());*/
            }

            DatePicker child = new DatePicker();

            childControl = child;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();

            arrayClassList.Add(this);
        }

        public override void Close()
        {
            arrayClassList.Remove(this);
        }

        /*
        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (childControl != null)
                {
                    Font font = MakeFont();
                    childControl.Font = font;

                    childControl.Left = x1;
                    childControl.Top = y1;
                    childControl.Width = x2 - x1;
                    childControl.Height = y2 - y1;
                }
            }
        }*/

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "DatePickerGetDate")
            {
                return childControl.SelectedDate;
            }
            else if (command == "DatePickerSetDate")
            {
                DateTime t;

                try
                {
                    t = new DateTime((int)args[0], (int)args[1], (int)args[2]);
                    childControl.SelectedDate = t;
                }
                catch
                {

                }

                return 1;
            }

            return 0;
        }

        /*
        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                childControl.Visible = flag;
            }

        }*/

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }
    }
}

/*
namespace GraphicModule
{
    [Serializable]
    public class ObjectArgsControlDatePicker
    {
        //public string sTag;
        public EnumWindowStyleFlags dwWindowStyle;
        public Color textColor = Color.Black;
        public BrushPublic backColor = new BrushSolid(Color.White);
        public string sFormat;
    }

    [Serializable]
    public class ObjectControlDatePicker : ObjectExpand
    {
        ObjectArgsControlDatePicker objArgs;

        [NonSerialized]
        DateTimePicker childControl = new DateTimePicker();

        // 등록된 클래스 리스트
        [NonSerialized]
        static public ArrayList arrayClassList = new ArrayList();

        public ObjectArgsControlDatePicker ObjectArgs
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

        public ObjectControlDatePicker(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlDatePicker args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlDatePicker;
            objArgs = args;

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                form.Controls.Add(childControl);


                arrayClassList.Add(this);

                //editBox.TextChanged += new EventHandler(this.Event_TextChanged);
                childControl.ForeColor = this.RunColorText;
                childControl.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22
                childControl.Format = DateTimePickerFormat.Custom;
                childControl.CustomFormat = args.sFormat;

                base.SetToolTipOnChildWindow(childControl);

                OnVisible(ExpandCalcVisible());
            }
        }

        public override void Close()
        {
            arrayClassList.Remove(this);
        }

        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (childControl != null)
                {
                    Font font = MakeFont();
                    childControl.Font = font;

                    childControl.Left = x1;
                    childControl.Top = y1;
                    childControl.Width = x2 - x1;
                    childControl.Height = y2 - y1;
                }
            }
        }

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "DatePickerGetDate")
            {
                return childControl.Value;
            }
            else if (command == "DatePickerSetDate")
            {
                DateTime t;

                try
                {
                    t = new DateTime((int)args[0], (int)args[1], (int)args[2]);
                    childControl.Value = t;
                }
                catch
                {

                }
                
                return 1;
            }

            return 0;
        }

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                Font font = MakeFont();

                int cyChar = (int)font.GetHeight() + 1;
                int cxChar = (int)font.SizeInPoints;

                RECT r = new RECT();

                //if ((objArgs.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
                //{
                //    g.DrawRectangle(Pens.Black, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                //}
                //else
                //{
                    DrawClass.PushRectangle2(g, x1, y1, x2, y2);
                //}

                Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);
                DrawClass.gcls(g, x1 + 1, y1 + 1, x2 - 1, y2 - 1, brushback);

                r.left = x1 + 1;
                r.top = y1 + 1;
                r.right = x2;
                r.bottom = y2;

                string str;

                str = String.Format("{0}", objArgs.sFormat);

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                Brush brush = new SolidBrush(this.RunColorText);

                DrawClass.DrawText(g, str, font, brush, r, format);
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            //SaveObjectItem.TagName(writer, objArgs.sTag);
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            ObjectSaveFont(writer);
            SaveObjectItem.WindowStyle(writer, objArgs.dwWindowStyle);
            writer.WriteLine("\tStringOption,{0},", objArgs.sFormat);
        }

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                childControl.Visible = flag;
            }

        }
    }
}
*/
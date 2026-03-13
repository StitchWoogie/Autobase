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
using System.Collections.Generic;
using AutoLib;

namespace SilverlightGraphicModule
{
    public class ObjectArgsControlCheckBox
    {
        public string sTag;
        public Color rgbColor;
        public string sTitle;
    }

    /// <summary>
    /// Summary description for ObjectControlCheckBox.
    /// </summary>
    public class ObjectControlCheckBox : ObjectExpand
    {
        ObjectArgsControlCheckBox objArgs;

        bool bSelected = false;
        //int cxChar;
        //int cyChar;

        UserControl formParent;
        CheckBox childCheckBox = new CheckBox();

        // 등록된 클래스 리스트
        static public List<object> arrayClassList = new List<object>();

        int[] nTagPos;

        public ObjectArgsControlCheckBox ObjectArgs
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

        public ObjectControlCheckBox(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlCheckBox args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlCheckBox;
            formParent = form;
            objArgs = args;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
                bSelected = true;
            else
                bSelected = false;

            TextColor = args.rgbColor;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);

                string sVal;
                double fVal;

                TagLib.GetTagValue(objArgs.sTag, out sVal, out fVal);
                if (fVal != 0) bSelected = true;
                else bSelected = false;
            }

            CheckBox child = new CheckBox();

            childCheckBox = child;

            child.Content = objArgs.sTitle;
            child.Click += new RoutedEventHandler(child_Click);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        bool bLoading = false;

        public override void EventTimerObject(UserControl form)
        {
            if (objArgs.sTag.Length == 0) return;

            TagPublicClass tp = TagLib.GetStructPublic(objArgs.sTag, ref nTagPos);

            if (nTagPos[0] == TagLib.TAG_NOT_FOUND) return;

            string sVal;
            double fVal;

            TagLib.GetTagValue(objArgs.sTag, out sVal, out fVal);
            if (fVal != 0)  bSelected = true;
            else            bSelected = false;

            if (childCheckBox.IsChecked != bSelected)
            {
                bLoading = true;
                childCheckBox.IsChecked = bSelected;
                bLoading = false;
            }

            tp.bNeedDataCurr = true;
        }

        void child_Click(object sender, RoutedEventArgs e)
        {
            if (bLoading) return;
            /*
            //if (!CheckResponseOnVisible()) return false;

            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            GetViewZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (e.X < x1) return false;
            if (e.X > x2) return false;

            if (e.Y < y1 || e.Y >= y1 + cyChar) return false;

            int val = bSelected ? 0 : 1;

            ChangeTag(val, true);

            base.OnEventSelChange();

            return true;*/

            int flag = childCheckBox.IsChecked == true ? 1 : 0;

            ChangeTag(flag, true);
        }

        public override void Close()
        {
            arrayClassList.Remove(this);
        }

        /*
        void DrawCheck(Graphics g, int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            float x, y;
            float px, py;
            int width, height;

            width = x2 - x1 + 1;
            height = y2 - y1 + 1;

            Pen pen = new Pen(Color.Black, 2);

            px = x1 + width / 5f;
            py = y1 + height / 2f;
            x = x1 + width / 3f;
            y = y2 - height / 5f;
            g.DrawLine(pen, px, py, x, y);
            px = x;
            py = y;
            x = x2 - width / 7f;
            y = y1 + height / 7f;
            g.DrawLine(pen, px, py, x, y);
        }

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            Font font = MakeFont();

            cyChar = (int)font.GetHeight() + 1;
            cxChar = (int)font.SizeInPoints;

            RECT r = new RECT();

            DrawClass.PushBox3(g, x1, y1 + 1, x1 + cyChar - 2, y1 + cyChar - 2, Color.White);

            if (bSelected)
            {
                DrawCheck(g, x1 + 2, y1 + 1 + 2, x1 + cyChar - 2 - 2, y1 + cyChar - 2 - 2);
            }

            r.left = x1 + cyChar + cxChar;
            r.top = y1;
            r.right = x2;
            r.bottom = y1 + cyChar;

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            Brush brush = new SolidBrush(RunColorText);
            DrawClass.DrawText(g, objArgs.sTitle, font, brush, r, format);
        }

        public override bool WmLeftButtonDown(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)
        {
            if (!CheckResponseOnVisible()) return false;

            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            GetViewZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (e.X < x1) return false;
            if (e.X > x2) return false;

            if (e.Y < y1 || e.Y >= y1 + cyChar) return false;

            int val = bSelected ? 0 : 1;

            ChangeTag(val, true);

            base.OnEventSelChange();

            return true;
        }

        public override bool WmLeftButtonUp(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }
        */

        void ChangeTag(int flag, bool bHandOperation)
        {
            if (flag == 0) bSelected = false;
            else bSelected = true;

            // InvalidateObject(formParent);

            if (objArgs.sTag.Length == 0) return;

            string text = "";

            text = flag.ToString();

            TagWrite.SetTagValue(objArgs.sTag, text, flag, bHandOperation);
        }

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "CheckBoxSetCheck")
            {
                int flag = (int)args[0];

                bLoading = true;
                this.childCheckBox.IsChecked = (flag == 1);
                bLoading = false;

                ChangeTag(flag, bHandOperation);
            }
            else if (command == "CheckBoxGetCheck")
            {
                return bSelected ? 1 : 0;
            }

            return 0;
        }

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }


        /*
        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.TagName(writer, objArgs.sTag);
            SaveObjectItem.TextColor(writer, GetTextColor());
            ObjectSaveFont(writer);
            writer.WriteLine("\tStringOption,{0},", objArgs.sTitle);
        }*/
    }
}

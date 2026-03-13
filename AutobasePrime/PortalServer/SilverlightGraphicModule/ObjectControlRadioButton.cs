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
using System.Collections.Generic;
using AutoLibLocal;
using AutoLib;

namespace SilverlightGraphicModule
{
    public class ObjectArgsControlRadioButton
    {
        public string sTag;
        public Color rgbColor;
        public List<object> arrayListData;
    }
    /// <summary>
    /// Summary description for ObjectControlRadioButton.
    /// </summary>
    
    public class ObjectControlRadioButton : ObjectExpand
    {
        ObjectArgsControlRadioButton objArgs;
        //int cxChar;
        //int cyChar;

        UserControl formParent;
        int nSelectPos;

        // 등록된 클래스 리스트

        static public List<object> arrayClassList = new List<object>();

        public ObjectArgsControlRadioButton ObjectArgs
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

        public ObjectControlRadioButton(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlRadioButton args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlRadioButton;
            formParent = form;
            objArgs = args;
            //cxChar = 1;
            //cyChar = 1;

            nSelectPos = -1;

            TextColor = objArgs.rgbColor;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);

                string sVal;
                double fVal;

                TagLib.GetTagValue(objArgs.sTag, out sVal, out fVal);
                nSelectPos = (int)fVal;
            }

            StackPanel child = new StackPanel();

            //child.Background = new SolidColorBrush(Colors.Green);

            RadioButton radio = new RadioButton();

            for (int i = 0; i < objArgs.arrayListData.Count; i++)
            {
                radio = new RadioButton();
                radio.Content = (string)objArgs.arrayListData[i];
                radio.Click += new RoutedEventHandler(radio_Click);
                child.Children.Add(radio);
            }
            
            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();

            SetRadio(nSelectPos);
        }

        bool bLoading = false;

        void SetRadio(int pos)
        {
            StackPanel child = (StackPanel)GetShapeOriginal();
            for (int i = 0; i < child.Children.Count; i++)
            {
                if (i == pos)
                {
                    bLoading = true;
                    ((RadioButton)(child.Children[i])).IsChecked = true;
                    bLoading = false;
                    break;
                }
            }
        }

        void radio_Click(object sender, RoutedEventArgs e)
        {
            if (bLoading) return;

            StackPanel child = (StackPanel)GetShapeOriginal();
            for (int i = 0; i < child.Children.Count; i++)
            {
                if (sender == child.Children[i])
                {
                    nSelectPos = i;
                    ChangeTag(nSelectPos, true);
                    break;
                }
            }
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

            int width, height;

            width = x2 - x1 + 1;
            height = y2 - y1 + 1;

            DrawClass.gcls(g, x1 + width / 5, y1 + height / 5, x2 - width / 5, y2 - height / 5, Color.Black);
        }

        void DrawOneRadio(Graphics g, int x1, int y1, int x2, int y2, string text, int cxChar, int cyChar, int radio_pos, Font font)
        {
            RECT r = new RECT();

            DrawClass.PushBox3(g, x1, y1 + 1, x1 + cyChar - 2, y1 + cyChar - 2, Color.White);

            if (nSelectPos == radio_pos)
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
            DrawClass.DrawText(g, text, font, brush, r, format);
        }

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            Font font = MakeFont();

            cyChar = (int)font.GetHeight() + 1;
            cxChar = (int)font.SizeInPoints;

            if (objArgs.arrayListData == null || objArgs.arrayListData.Count < 1)
            {
                DrawOneRadio(g, x1, y1, x2, y2, objGeneral.GetClassName(), cxChar, cyChar, 0, font);
            }
            else
            {
                int l;
                string buf;
                int y;

                for (l = 0; l < objArgs.arrayListData.Count; l++)
                {
                    buf = (string)objArgs.arrayListData[l];
                    y = y1 + (y2 - y1 + 1) * l / (objArgs.arrayListData.Count);
                    DrawOneRadio(g, x1, y, x2, y2, buf, cxChar, cyChar, l, font);
                }
            }
        }
        */

        void ChangeTag(int radio_pos, bool bHandOperation)
        {
            nSelectPos = radio_pos;

            //InvalidateObject(formParent);

            if (objArgs.sTag.Length == 0) return;

            TagWrite.SetTagValue(objArgs.sTag, nSelectPos.ToString(), nSelectPos, bHandOperation);
        }
        /*
        public override bool WmLeftButtonDown(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)
        {
            if (!CheckResponseOnVisible()) return false;

            int radio_pos = nSelectPos;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            GetViewZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (e.X < x1 || e.X > x2) return false;

            int l;
            int y;

            for (l = 0; l < objArgs.arrayListData.Count; l++)
            {
                y = y1 + (y2 - y1 + 1) * l / (objArgs.arrayListData.Count);
                if (e.Y >= y && e.Y < y + cyChar)
                {
                    radio_pos = l;
                    ChangeTag(radio_pos, true);
                    OnEventSelChange();
                    return true;
                }
            }

            return false;
        }

        public override bool WmLeftButtonUp(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }
        */
        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "RadioButtonSetPos")
            {
                int pos = (int)args[0];
                bLoading = true;
                SetRadio(pos);
                bLoading = false;
                ChangeTag(pos, bHandOperation);
            }
            else if (command == "RadioButtonGetPos")
            {
                return nSelectPos;
            }

            return 0;
        }
        /*
        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.TagName(writer, objArgs.sTag);
            SaveObjectItem.TextColor(writer, GetTextColor());
            ObjectSaveFont(writer);
            //SaveObjectItem.WindowStyle(writer, objArgs.dwWindowStyle);
            //writer.WriteLine("\tStringOption,{0},", objArgs.nValueConvert);

            if (objArgs.arrayListData != null && objArgs.arrayListData.Count > 0)
            {
                int l;
                string buf;
                for (l = 0; l < objArgs.arrayListData.Count; l++)
                {
                    buf = (string)objArgs.arrayListData[l];
                    writer.WriteLine("\tListData,{0},", buf);
                }
            }
        }*/

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }
    }
}

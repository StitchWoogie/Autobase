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
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectArgsControlComboBox
    {
        public string sTag;
        public List<object> arrayListData;
        public EnumWindowStyleFlags dwWindowStyle;
        public int nValueConvert;
        public Color textColor = Colors.Black;
        public BrushPublic backColor = new BrushSolid(Colors.White);
    }

    /// <summary>
    /// Summary description for ObjectControlComboBox. 
    /// </summary>
    public class ObjectControlComboBox : ObjectExpand
    {
        ObjectArgsControlComboBox objArgs;

        ComboBox comboBox = new ComboBox();

        // 등록된 클래스 리스트
        static public List<object> arrayClassList = new List<object>();

        public ObjectArgsControlComboBox ObjectArgs
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

        public ObjectControlComboBox(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlComboBox args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlComboBox;
            objArgs = args;

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

            ComboBox child = new ComboBox();
            comboBox = child;

            /*
            if ((args.dwWindowStyle & EnumWindowStyleFlags.CBS_SORT) == EnumWindowStyleFlags.CBS_SORT)
                comboBoxSorted = true;
            else
                comboBox.Sorted = false;
            

            if ((args.dwWindowStyle & EnumWindowStyleFlags.CBS_NOINTEGRALHEIGHT) == EnumWindowStyleFlags.CBS_NOINTEGRALHEIGHT)
                comboBox.IntegralHeight = false;
            else
                comboBox.IntegralHeight = true;*/

            if ((args.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWNLIST) == EnumWindowStyleFlags.CBS_DROPDOWNLIST)
            {
                //comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            else if ((args.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWN) == EnumWindowStyleFlags.CBS_DROPDOWN)
            {
                //comboBox.IsEditable = true;
            }
            else
            {
                //comboBox..IsEditable = true;
                comboBox.IsDropDownOpen = true;
            }

            MakeFont(child);

            comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);

            comboBox.Foreground = new SolidColorBrush(this.RunColorText);
            comboBox.Background = ObjectRectangle.MakePublicBrush(this.RunColorBack);
            
            string sVal;
            double fVal;
            TagLib.GetTagValue(objArgs.sTag, out sVal, out fVal);

            if (objArgs.arrayListData != null)
            {
                int l;
                string buf;
                for (l = 0; l < objArgs.arrayListData.Count; l++)
                {
                    buf = (string)objArgs.arrayListData[l];
                    comboBox.Items.Add(buf);
                }

                if (objArgs.nValueConvert == 1)
                {
                    if ((int)fVal >= 0 && (int)fVal < comboBox.Items.Count)
                        comboBox.SelectedIndex = (int)fVal;
                }
            }

            if (objArgs.nValueConvert == 0)
            {
                comboBox.SelectedItem = sVal;
            }

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();

            arrayClassList.Add(this);
        }

        void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeTag(true);
            OnEventSelChange();
        }

        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            base.OnMove(x1, y1, x2, y2);

            FrameworkElement child = GetShapeOriginal();
            if(child != null)
                child.Height = 30;
        }

        /*
        private void Event_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ChangeTag(true);
            OnEventSelChange();
        }
        */
        public override void Close()
        {
            arrayClassList.Remove(this);
        }
        /*
        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (comboBox != null)
                {
                    Font font = MakeFont();
                    comboBox.Font = font;

                    comboBox.Left = x1;
                    comboBox.Top = y1;
                    comboBox.Width = x2 - x1;
                    comboBox.Height = y2 - y1;
                }
                //MoveWindow(hwndCtrl, x1, y1, x2-x1, y2-y1, TRUE);
                //InvalidateRect(hwndCtrl, NULL, FALSE);
                //InvalidateRect(GetParent(hwndCtrl), NULL, FALSE);	// 배경 화면을 Update한다.
            }
        }*/

        void ChangeTag(bool bHandOperation)
        {
            if (objArgs.sTag.Length == 0) return;

            string text = "";
            EnumTagType tag_type = 0;
            int[] tag_pos = new int[1];

            if (!TagLib.GetTagTypeAndPos(objArgs.sTag, ref tag_type, ref tag_pos)) return;

            int retn = comboBox.SelectedIndex;

            if (retn == -1) return;

            if (tag_type == EnumTagType.ST)
            {
                text = (string)comboBox.SelectedItem;
                TagWrite.SetTagValue(objArgs.sTag, text, 0, bHandOperation);
            }
            else
            {
                if (objArgs.nValueConvert == 0)
                {
                    text = (string)comboBox.SelectedItem;
                    TagWrite.SetTagValue(objArgs.sTag, text, ConvertTool.ToDouble(text), bHandOperation);
                }
                else
                {
                    TagWrite.SetTagValue(objArgs.sTag, retn.ToString(), retn, bHandOperation);
                }
            }
        }
        /*
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

                DrawClass.PushBox2(g, x1, y1, x2, y1 + cyChar + 1, this.RunColorBack);

                r.left = x1 + 1;
                r.top = y1 + 1;
                r.right = x2 - cyChar;
                r.bottom = y1 + cyChar + 1;

                string str;

                str = objGeneral.GetClassName();

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                Brush brush = new SolidBrush(this.RunColorText);

                DrawClass.DrawText(g, str, font, brush, r, format);

                if ((objArgs.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWNLIST) > 0)
                {
                    DrawClass.PopBox2(g, x2 - cyChar, y1 + 1, x2 - 1, y1 + cyChar, Color.LightGray);
                }
                else if ((objArgs.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWN) > 0)
                {
                    DrawClass.PopBox2(g, x2 - cyChar, y1 + 1, x2 - 1, y1 + cyChar, Color.LightGray);
                }
                else
                {
                    DrawClass.PushBox2(g, x1, y1 + cyChar + 2, x2, y2, Color.LightGray);
                }
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.TagName(writer, objArgs.sTag);
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            ObjectSaveFont(writer);
            SaveObjectItem.WindowStyle(writer, objArgs.dwWindowStyle);
            writer.WriteLine("\tStringOption,{0},", objArgs.nValueConvert);

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
        }

        */

        protected override void TextColorChanged(Color color)
        {
            if (comboBox == null) return;

            comboBox.Foreground = new SolidColorBrush(color);
        }

        protected override void BackColorChanged(Color color)
        {
            if (comboBox == null) return;

            comboBox.Background = new SolidColorBrush(color);
        }

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "ComboBoxAddString")
            {
                this.comboBox.Items.Add(args[0]);
                return 1;
            }
            else if (command == "ComboBoxSetCurSel")
            {
                int index = (int)args[0];
                if (index >= comboBox.Items.Count || index < -1) index = -1;	// overflow -1=cursor unselect
                this.comboBox.SelectedIndex = index;

                return index;
            }
            else if (command == "ComboBoxGetCurSel")
            {
                return this.comboBox.SelectedIndex;
            }
            else if (command == "ComboBoxResetContent")
            {
                this.comboBox.Items.Clear();
                return 1;
            }
            else if (command == "ComboBoxDeleteString")
            {
                int index = (int)args[0];
                if (index >= comboBox.Items.Count || index < 0) return 0;	// overflow
                comboBox.Items.RemoveAt(index);
                return 1;
            }
            else if (command == "ComboBoxSetText")
            {
                string text = (string)args[0];
                for (int i = 0; i < this.comboBox.Items.Count; i++)
                {
                    if ((string)(this.comboBox.Items[i]) == text)
                    {
                        this.comboBox.SelectedIndex = i;
                        return 1;
                    }
                }
                
                return 0;
            }
            else if (command == "ComboBoxGetText")
            {
                if (this.comboBox.SelectedItem == null) return "";

                return this.comboBox.SelectedItem;     
            }
            else if (command == "ComboBoxGetItemCount")
            {
                return this.comboBox.Items.Count;
            }
            else if (command == "ComboBoxGetItemText")
            {
                int index = (int)args[0];
                if (index >= comboBox.Items.Count || index < 0) return "";	// overflow
                return (string)this.comboBox.Items[index];
            }
            else
            {
                return 0;
            }
        }
        
        /*
        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                comboBox.Visible = flag;
            }

        }*/

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }
    }
}

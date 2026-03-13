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
using AutoLib;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectArgsControlListBox
    {
        public string sTag;
        public List<object> arrayListData;
        public EnumWindowStyleFlags dwWindowStyle;
        public int nValueConvert;
        public Color textColor = Colors.Black;
        public BrushPublic backColor = new BrushSolid(Colors.White);
        public int nSelectionMode = 1;  // default - ONE select
    }

    /// <summary>
    /// Summary description for ObjectControlListBox.
    /// </summary>
    public class ObjectControlListBox : ObjectExpand
    {
        ObjectArgsControlListBox objArgs;

        ListBox listBox = new ListBox();

        // 등록된 클래스 리스트
        static public List<object> arrayClassList = new List<object>();

        public ObjectArgsControlListBox ObjectArgs
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

        public ObjectControlListBox(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlListBox args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlListBox;
            objArgs = args;

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

            /*
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                form.Controls.Add(listBox);

                if ((args.dwWindowStyle & EnumWindowStyleFlags.LBS_SORT) == 0)
                    listBox.Sorted = false;
                else
                    listBox.Sorted = true;

                if ((args.dwWindowStyle & EnumWindowStyleFlags.LBS_NOINTEGRALHEIGHT) == 0)
                    listBox.IntegralHeight = true;
                else
                    listBox.IntegralHeight = false;

                if ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
                    listBox.BorderStyle = BorderStyle.FixedSingle;
                else
                    listBox.BorderStyle = BorderStyle.None;

                if ((args.dwWindowStyle & EnumWindowStyleFlags.WS_VSCROLL) > 0)
                    listBox.ScrollAlwaysVisible = true;
                else
                    listBox.ScrollAlwaysVisible = false;

                arrayClassList.Add(this);

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
                        listBox.Items.Add(buf);
                    }

                    if (objArgs.nValueConvert == 1)
                    {
                        if ((int)fVal >= 0 && (int)fVal < listBox.Items.Count)
                            listBox.SelectedIndex = (int)fVal;
                    }
                }

                if (objArgs.nValueConvert == 0)
                {
                    listBox.Text = sVal;
                }

                listBox.SelectedIndexChanged += new System.EventHandler(this.Event_SelectedIndexChanged);

                listBox.ForeColor = this.RunColorText;
                listBox.BackColor = this.RunColorBack;

                if (args.nSelectionMode == 0)
                {
                    listBox.SelectionMode = SelectionMode.None;
                }
                else if (args.nSelectionMode == 2)
                {
                    listBox.SelectionMode = SelectionMode.MultiSimple;
                }
                else if (args.nSelectionMode == 3)
                {
                    listBox.SelectionMode = SelectionMode.MultiExtended;
                }
                else
                {
                    listBox.SelectionMode = SelectionMode.One;
                }

                base.SetToolTipOnChildWindow(listBox);
                OnVisible(ExpandCalcVisible());
            }*/

            ListBox child = new ListBox();
            listBox = child;

            MakeFont(child);

            listBox.SelectionChanged += new SelectionChangedEventHandler(listBox_SelectionChanged);

            listBox.Foreground = new SolidColorBrush(this.RunColorText);
            listBox.Background = ObjectRectangle.MakePublicBrush(this.RunColorBack);

            //if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0)
            //    child.editBox.PasswordChar = '*';

            /*
            
            if ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
            {
                child.BorderThickness = new Thickness(1);
            }
            else
                child.BorderThickness = new Thickness(0);*/

            /*
            if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0)
                editBox.CharacterCasing = CharacterCasing.Upper;
            else if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0)
                editBox.CharacterCasing = CharacterCasing.Lower;
            else
                editBox.CharacterCasing = CharacterCasing.Normal;*/

            //child.Source = animation.GetImageSource(0);
            //child.Stretch = Stretch.Fill;

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
                    listBox.Items.Add(buf);
                }

                if (objArgs.nValueConvert == 1)
                {
                    if ((int)fVal >= 0 && (int)fVal < listBox.Items.Count)
                        listBox.SelectedIndex = (int)fVal;
                }
            }

            if (objArgs.nValueConvert == 0)
            {
                listBox.SelectedItem = sVal;
            }

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();

            arrayClassList.Add(this);
        }

        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeTag(true);
            OnEventSelChange();
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
                if (listBox != null)
                {
                    Font font = MakeFont();
                    listBox.Font = font;

                    listBox.Left = x1;
                    listBox.Top = y1;
                    listBox.Width = x2 - x1;
                    listBox.Height = y2 - y1;
                }
            }
        }
        */
        void ChangeTag(bool bHandOperation)
        {
            if (objArgs.sTag.Length == 0) return;

            string text = "";
            EnumTagType tag_type = 0;
            int[] tag_pos = new int[1];

            if (!TagLib.GetTagTypeAndPos(objArgs.sTag, ref tag_type, ref tag_pos)) return;

            int retn = listBox.SelectedIndex;

            if (retn == -1) return;

            if (tag_type == EnumTagType.ST)
            {
                text = (string)listBox.Items[retn];
                TagWrite.SetTagValue(objArgs.sTag, text, 0, bHandOperation);
            }
            else
            {
                if (objArgs.nValueConvert == 0)
                {
                    text = (string)listBox.Items[retn];
                    TagWrite.SetTagValue(objArgs.sTag, text, ConvertTool.ToDouble(text), bHandOperation);
                }
                else
                {
                    TagWrite.SetTagValue(objArgs.sTag, retn.ToString(), retn, bHandOperation);
                }
            }
        }
        /*
        void FillDirFile(string dir, int method)
        {
            string dir_name;

            try
            {
                dir_name = Path.GetDirectoryName(dir);
            }
            catch
            {
                return;
            }

            if (!Directory.Exists(dir_name)) return;

            DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(dir));

            if (method == 1)
            {
                foreach (DirectoryInfo di in info.GetDirectories("*.*"))
                {
                    listBox.Items.Add(di.Name);
                }
            }
            else if (method == 2)
            {
                foreach (FileInfo fi in info.GetFiles(Path.GetFileName(dir)))
                {
                    listBox.Items.Add(fi.Name);
                }
            }
        }
        */
        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "ListBoxAddString")
            {
                listBox.Items.Add(args[0]);
            }
            else if (command == "ListBoxGetCurSel")
            {
                return listBox.SelectedIndex;
            }
            else if (command == "ListBoxSetCurSel")
            {
                int index = (int)args[0];
                if (index >= listBox.Items.Count || index < -1) index = -1;	// overflow -1=cursor unselect
                listBox.SelectedIndex = index;
                return index;
            }
            else if (command == "ListBoxResetContent")
            {
                listBox.Items.Clear();
            }
            else if (command == "ListBoxFillDir")
            {
                //FillDirFile((string)args[0], 0x0001);
            }
            else if (command == "ListBoxFillFile")
            {
                //FillDirFile((string)args[0], 0x0002);
            }
            else if (command == "ListBoxDeleteString")
            {
                int index = (int)args[0];
                if (index >= listBox.Items.Count || index < 0) return 0;	// overflow
                listBox.Items.RemoveAt(index);
            }
            else if (command == "ListBoxGetItemCount")
            {
                return this.listBox.Items.Count;
            }
            else if (command == "ListBoxGetItemText")
            {
                int index = (int)args[0];
                if (index >= listBox.Items.Count || index < 0) return "";	// overflow
                return (string)this.listBox.Items[index];
            }
            else if (command == "ListBoxGetSel")
            {
                int index = (int)args[0];

                /*
                if (index >= listBox.Items.Count || index < 0) return 0;	// overflow

                for (int i = 0; i < listBox.SelectedIndices.Count; i++)
                {
                    if (listBox.SelectedIndices[i] == index) return 1;
                }*/
                if (index == listBox.SelectedIndex) return 1;

                return 0;
            }
            else if (command == "ListBoxGetSelCount")
            {
                //return this.listBox.se.SelectedItems.Count;
                return (this.listBox.SelectedIndex == -1) ? 0 : 1;  // 다중 선택이 없다.
            }
            else
            {

            }

            return 0;
        }

        public override string ExecuteClassNameStringReturn(string command, params object[] args)
        {
            if (command == "ListBoxGetText")
            {
                int index = (int)args[0];
                if (index >= this.listBox.Items.Count)
                    return "";
                else if (index < 0)	// -1
                    return "";
                else
                    return (string)this.listBox.Items[index];
            }

            return "";
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

                if ((objArgs.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
                {
                    g.DrawRectangle(Pens.Black, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                }
                else
                {
                    DrawClass.PushRectangle2(g, x1, y1, x2, y2);
                }

                DrawClass.gcls(g, x1 + 1, y1 + 1, x2 - 1, y2 - 1, this.RunColorBack);

                if (objArgs.arrayListData != null && objArgs.arrayListData.Count > 0)
                {
                    int l;
                    string buf;
                    int y;
                    for (l = 0, y = y1 + 1; l < objArgs.arrayListData.Count && y < y2 - cyChar; l++, y += cyChar)
                    {
                        buf = (string)objArgs.arrayListData[l];
                        r.left = x1 + 1;
                        r.top = y;
                        r.right = x2 - 1;
                        r.bottom = y + cyChar;

                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Center;

                        DrawClass.DrawText(g, buf, font, new SolidBrush(this.RunColorText), r, format);
                    }
                }
                else
                {
                    r.left = x1 + 1;
                    r.top = y1 + 1;
                    r.right = x2;
                    r.bottom = y2;

                    string str;

                    str = objGeneral.GetClassName();

                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    DrawClass.DrawText(g, str, font, new SolidBrush(this.RunColorText), r, format);
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

            writer.Write("\tStringOption,{0},", objArgs.nValueConvert);
            writer.Write("{0},", objArgs.nSelectionMode);
            writer.WriteLine();

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

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                listBox.Visible = flag;
            }

        }*/

        protected override void TextColorChanged(Color color)
        {
            if (listBox == null) return;

            listBox.Foreground = new SolidColorBrush(color);
        }

        protected override void BackColorChanged(Color color)
        {
            if (listBox == null) return;

            listBox.Background = new SolidColorBrush(color);
        }

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }
    }
}

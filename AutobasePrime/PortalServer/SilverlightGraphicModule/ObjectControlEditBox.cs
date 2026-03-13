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
    public class ObjectArgsControlEditBox
    {
        public string sTag;
        public EnumWindowStyleFlags dwWindowStyle;
        public Color textColor = Colors.Black;
        public BrushPublic backColor = new BrushSolid(Colors.White);
    }


    /// <summary>
    /// Summary description for ObjectControlEditBox.
    /// </summary>
    public class ObjectControlEditBox : ObjectExpand
    {
        ObjectArgsControlEditBox objArgs;

        Control editBox = new TextBox();

        // 등록된 클래스 리스트
        static public List<object> arrayClassList = new List<object>();

        public ObjectArgsControlEditBox ObjectArgs
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

        public ObjectControlEditBox(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlEditBox args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlEditBox;
            objArgs = args;

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                /*
                form.Controls.Add(editBox);

                if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0)
                    editBox.PasswordChar = '*';

                if ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
                    editBox.BorderStyle = BorderStyle.FixedSingle;
                else
                    editBox.BorderStyle = BorderStyle.None;

                if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0)
                    editBox.CharacterCasing = CharacterCasing.Upper;
                else if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0)
                    editBox.CharacterCasing = CharacterCasing.Lower;
                else
                    editBox.CharacterCasing = CharacterCasing.Normal;

                arrayClassList.Add(this);

                string sVal;
                double fVal;
                TagLib.GetTagValue(objArgs.sTag, out sVal, out fVal);

                editBox.Text = sVal;

                editBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Event_KeyDown);
                editBox.TextChanged += new EventHandler(this.Event_TextChanged);
                editBox.ForeColor = this.RunColorText;
                editBox.BackColor = this.RunColorBack;

                base.SetToolTipOnChildWindow(editBox);

                OnVisible(ExpandCalcVisible());
                 */
            }

            Control child;

            if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0)
            {
                PasswordBox pbox = new PasswordBox();
                child = pbox;
                //MakeFont((PasswordBox)child);
                pbox.PasswordChanged += new RoutedEventHandler(pbox_PasswordChanged);
            }
            else
            {
                TextBox tbox = new TextBox();
                child = tbox;
                MakeFont(tbox);
                tbox.TextChanged += new TextChangedEventHandler(child_TextChanged);
            }

            editBox = child;

            // 두께도 적용이 안됨
            if ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
            {
                child.BorderThickness = new Thickness(1);
            }
            else
                child.BorderThickness = new Thickness(0);

            child.VerticalContentAlignment = VerticalAlignment.Center;  // 적용이 안됨

            ChangeEditorTextAsTagValue();

            /*
            if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0)
                editBox.CharacterCasing = CharacterCasing.Upper;
            else if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0)
                editBox.CharacterCasing = CharacterCasing.Lower;
            else
                editBox.CharacterCasing = CharacterCasing.Normal;*/

            //child.Source = animation.GetImageSource(0);
            //child.Stretch = Stretch.Fill;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();

            arrayClassList.Add(this);
        }

        void pbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (objArgs.sTag.Length != 0)
            {
                string text = ((PasswordBox)editBox).Password;

                TagWrite.SetTagValue(objArgs.sTag, text, ConvertTool.ToDouble(text), true);
            }
        }

        void child_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (objArgs.sTag.Length != 0)
            {
                string text = ((TextBox)editBox).Text;

                TagWrite.SetTagValue(objArgs.sTag, text, ConvertTool.ToDouble(text), true);
            }

            
        }

        void ChangeEditorTextAsTagValue()
        {
            if (objArgs.sTag.Length == 0) return;

            string sVal;
            double fVal;
            TagLib.GetTagValue(objArgs.sTag, out sVal, out fVal);

            if(editBox.GetType() == typeof(TextBox)) 
                ((TextBox)editBox).Text = sVal;
            else if (editBox.GetType() == typeof(PasswordBox))
                ((PasswordBox)editBox).Password = sVal;
        }

        /*
        private void Event_KeyDown(object sender, KeyEventArgs e)
        {
            OnEventKeyDown(e.KeyValue);
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
                if (editBox != null)
                {
                    Font font = MakeFont();
                    editBox.Font = font;

                    editBox.Left = x1;
                    editBox.Top = y1;
                    editBox.Width = x2 - x1;
                    editBox.Height = y2 - y1;
                }
            }
        }

        void ChangeTag(string text, bool bHandOperation)
        {
            editBox.Text = text;

            if (objArgs.sTag.Length == 0) return;

            TagWrite.SetTagValue(objArgs.sTag, text, 0, bHandOperation);
        }*/

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "EditBoxSetText")
            {
                if (editBox.GetType() == typeof(TextBox))
                    ((TextBox)editBox).Text = (string)args[0];
                else if (editBox.GetType() == typeof(PasswordBox))
                    ((PasswordBox)editBox).Password = (string)args[0];
            }
            else if (command == "EditBoxSetFocus")
            {
                editBox.Focus();
            }
            else if (command == "EditBoxSelectAll")
            {
                if (editBox.GetType() == typeof(TextBox))
                    ((TextBox)editBox).SelectAll();
                else if (editBox.GetType() == typeof(PasswordBox))
                    ((PasswordBox)editBox).SelectAll();
            }

            return 0;
        }

        public override string ExecuteClassNameStringReturn(string command, params object[] args)
        {
            if (command == "EditBoxGetText")
            {
                if (editBox.GetType() == typeof(TextBox))
                    return ((TextBox)editBox).Text;
                else if (editBox.GetType() == typeof(PasswordBox))
                    return ((PasswordBox)editBox).Password;
                else 
                    return "";
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

                r.left = x1 + 1;
                r.top = y1 + 1;
                r.right = x2;
                r.bottom = y2;

                string str;

                str = objGeneral.GetClassName();

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                Brush brush = new SolidBrush(this.RunColorText);

                DrawClass.DrawText(g, str, font, brush, r, format);
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.TagName(writer, objArgs.sTag);
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            ObjectSaveFont(writer);
            SaveObjectItem.WindowStyle(writer, objArgs.dwWindowStyle);
        }*/

        protected override void TextColorChanged(Color color)
        {
            if (editBox == null) return;

            if (editBox.GetType() == typeof(TextBox))
                ((TextBox)editBox).Foreground = new SolidColorBrush(color);
            else if (editBox.GetType() == typeof(PasswordBox))
                ((PasswordBox)editBox).Foreground = new SolidColorBrush(color);
        }

        protected override void BackColorChanged(Color color)
        {
            if (editBox == null) return;

            if (editBox.GetType() == typeof(TextBox))
                ((TextBox)editBox).Background = new SolidColorBrush(color);
            else if (editBox.GetType() == typeof(PasswordBox))
                ((PasswordBox)editBox).Background = new SolidColorBrush(color);
        }

        /*
        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                editBox.Visible = flag;
            }

        }*/

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }
    }
}

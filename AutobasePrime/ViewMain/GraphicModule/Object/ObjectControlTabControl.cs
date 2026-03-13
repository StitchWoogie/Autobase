using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using AutoLibLocal;
using NetTools.OldDefine;
using NetTools;
using AutoLib;
using System.Threading.Tasks;

namespace GraphicModule
{
    [Serializable]
    public class ObjectArgsControlTabControl
    {
        //public string sTag;
        public EnumWindowStyleFlags dwWindowStyle;
        public Color textColor = Color.Black;
        public BrushPublic backColor = new BrushSolid(Color.White);
        public string sFormat;
    }

    [Serializable]
    public class ObjectControlTabControl : ObjectExpand
    {
        ObjectArgsControlTabControl objArgs;

        [NonSerialized]
        TabControl childControl = new TabControl();

        [NonSerialized]
        Form formParent; // 20250312 PSU

        // 등록된 클래스 리스트
        [NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        public ObjectArgsControlTabControl ObjectArgs
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

        public ObjectControlTabControl(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlTabControl args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlTabControl;
            objArgs = args;

            formParent = form; //20250312 PSU

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                form.Controls.Add(childControl);

                /*
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
                    editBox.CharacterCasing = CharacterCasing.Normal;*/

                arrayClassList.Add(this);

                //editBox.TextChanged += new EventHandler(this.Event_TextChanged);
                childControl.ForeColor = this.RunColorText;
                childControl.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22
                //childControl.Format = DateTimePickerFormat.Custom;
                //childControl.CustomFormat = args.sFormat;

                _ = base.SetToolTipOnChildWindow(childControl);

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
                    // 컨트롤 업데이트 일시 중지  //20250312 PSU 추가.
                    childControl.SuspendLayout();
                    try
                    {
                        Font font = MakeFont();
                        childControl.Font = font;

                        childControl.Left = x1;
                        childControl.Top = y1;
                        childControl.Width = x2 - x1;
                        childControl.Height = y2 - y1;
                    }
                    finally
                    {
                        // 컨트롤 업데이트 재개
                        childControl.ResumeLayout(false);
                    }
                }
            }
        }

        void InsertTabPage(int pos, string page_title, string module_name)
        {
            TabPage page = new TabPage(page_title);

            string filename = MakeFilePath.Graphic(module_name);

            /*                
            if (String.Compare(filename,  .sModuleName, true) == 0)
            {
                bModuleSameFlag = true;
            }
            else
            {
                bModuleSameFlag = false;
            }*/

            FormGraphicChild formModule = new FormGraphicChild(filename);
            //
            formModule.FormBorderStyle = FormBorderStyle.None;
            formModule.TopLevel = false;
            formModule.Dock = DockStyle.Fill;

            formModule.WindowState = FormWindowState.Normal;
            formModule.Size = new Size(0, 0);

            page.Controls.Add(formModule);

            if (pos > childControl.TabPages.Count)
                pos = childControl.TabPages.Count;
            if (pos < 0)
                pos = 0;

            childControl.TabPages.Insert(pos, page);

            formModule.Show();
        }

        public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            // UI 스레드에서 실행해야 하는 작업을 위한 헬퍼 메서드
            //T InvokeIfRequiredWithReturn<T>(Func<T> action)
            //{
            //    if (this.childControl.InvokeRequired)
            //    {
            //        return (T)this.childControl.Invoke(action);
            //    }
            //    else
            //    {
            //        return action();
            //    }
            //}

            void InvokeIfRequiredVoid(Action action)
            {
                if (this.childControl.InvokeRequired)
                {
                    this.childControl.Invoke(action);
                }
                else
                {
                    action();
                }
            }

            if (command == "TabControlAddPage")
            {
                string page_title = (string)args[1];        // 
                string module_name = (string)args[2];

                InvokeIfRequiredVoid(() => {
                    InsertTabPage(childControl.TabPages.Count, page_title, module_name);
                });

                return 1;
            }
            else if (command == "TabControlInsertPage")
            {
                int pos = (int)args[1];
                string page_title = (string)args[2];        
                string module_name = (string)args[3];

                InvokeIfRequiredVoid(() => {
                    InsertTabPage(pos, page_title, module_name);
                });
            }
            else if (command == "TabControlDeletePage")
            {
                int pos = (int)args[1];

                InvokeIfRequiredVoid(() => {
                    if (pos >= 0 && pos < childControl.TabPages.Count)
                        childControl.TabPages.RemoveAt(pos);
                });
            }

            await Task.CompletedTask;
            return 0;
        }

        /* 사용하지 않아서 뺐다. 2016-7-1
        public override string ExecuteClassNameStringReturn(string command, params object[] args)
        {
            

            return "";
        }*/

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

                str = String.Format("{0}", "TabControl");

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                Brush brush = new SolidBrush(this.RunColorText);

                DrawClass.DrawText(g, str, font, brush, r, format);
            }

            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {

                UpdateControlState(childControl,formParent);//20250312 PSU
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

        /*
        protected override void TextColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            childControl.ForeColor = this.RunColorText;
        }

        protected override void BackColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            childControl.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22
        }*/

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                childControl.Visible = flag;
            }
        }

        // 탭 컨트롤 안에도 그래픽 모듈이 있으므로 함수를 실행해 준다.
        public bool ExecuteClassNameOnlyObject(Form form, string classname, string command, out object retn_value, params object[] args)
        {
            bool retn = false;
            retn_value = 0;

            for (int i = 0; i < childControl.TabPages.Count; i++)
            {
                TabPage tb = childControl.TabPages[i];

                if (tb.Controls.Count < 1) continue;
                if (tb.Controls[0].GetType() != typeof(FormGraphicChild)) continue;

                FormGraphicChild fgc = (FormGraphicChild)tb.Controls[0];

                retn = fgc.objectGraphic.ExecuteClassNameOnlyObject(form, classname, command, out retn_value, args);
                if (retn)
                    return true;
            }

            return retn;
        }
    }
}

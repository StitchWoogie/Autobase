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
    public class ObjectArgsControlTreeView
    {
        //public string sTag;
        public EnumWindowStyleFlags dwWindowStyle;
        public Color textColor = Color.Black;
        public BrushPublic backColor = new BrushSolid(Color.White);
        public string sFormat;

        public ScriptClass scriptEventDoubleClick;

        //public ScriptClass scriptEventCellClick;
        //public ScriptClass scriptEventCellPainting;
        //public ScriptClass scriptEventCellValueChanged;
    }

    [Serializable]
    public class ObjectControlTreeView : ObjectExpand
    {
        ObjectArgsControlTreeView objArgs;

        [NonSerialized]
        TreeView childControl = new TreeView();

        [NonSerialized]
        Form formParent; // 20250312 PSU

        // 등록된 클래스 리스트
        [NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        public ObjectArgsControlTreeView ObjectArgs
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

        public ObjectControlTreeView(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlTreeView args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ControlTreeView;
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
                */
                if ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
                    childControl.BorderStyle = BorderStyle.FixedSingle;
                else
                    childControl.BorderStyle = BorderStyle.None;

                /*
                if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0)
                    editBox.CharacterCasing = CharacterCasing.Upper;
                else if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0)
                    editBox.CharacterCasing = CharacterCasing.Lower;
                else
                    editBox.CharacterCasing = CharacterCasing.Normal;*/

                arrayClassList.Add(this);

                childControl.DoubleClick += new EventHandler(wndChild_DoubleClick);
                //childControl.KeyDown += new KeyEventHandler(Event_KeyDown);
                childControl.KeyDown += async (sender, e) => await Event_KeyDown(sender, e); // 20250724 PSU
                //childControl.AfterSelect += new TreeViewEventHandler(childControl_AfterSelect);
                childControl.AfterSelect += async (sender, e) => await childControl_AfterSelect(sender, e); // 20250724 PSU

                //editBox.TextChanged += new EventHandler(this.Event_TextChanged);
                childControl.ForeColor = this.RunColorText;
                childControl.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22
                //childControl.Format = DateTimePickerFormat.Custom;
                //childControl.CustomFormat = args.sFormat;

                _ = base.SetToolTipOnChildWindow(childControl);

                OnVisible(ExpandCalcVisible());
            }
        }

        async Task childControl_AfterSelect(object sender, TreeViewEventArgs e)
        {
            await OnEventSelChange();
        }

        private async Task Event_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            await OnEventKeyDown(e.KeyValue);
        }

        async Task RunScript(ScriptClass script, string scriptname)
        {
            if (script != null)
            {
                script.SetHandOperation();	// 수동으로 출력한다.
                await script.RunAsync(objCommonProperty.form, this);

                if (script.IsError())
                {
                    string message;
                    message = script.GetError();

                    if (Tools.IsLangKorean())
                        MessageBox.Show("스크립트 오류\n\n" + message, scriptname);
                    else if (Tools.IsLangChinese())
                        MessageBox.Show("脚本错误\n\n" + message, scriptname);
                    else
                        MessageBox.Show("Script Error\n\n" + message, scriptname);
                }
            }
        }

        async void wndChild_DoubleClick(object sender, EventArgs e)
        {
            await RunScript(objArgs.scriptEventDoubleClick, "DoubleClick Script");
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
                    // 컨트롤 업데이트 일시 중지
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

        public override  async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            // UI 스레드에서 실행해야 하는 작업을 위한 헬퍼 메서드
            T InvokeIfRequiredWithReturn<T>(Func<T> action)
            {
                if (this.childControl.InvokeRequired)
                {
                    return (T)this.childControl.Invoke(action);
                }
                else
                {
                    return action();
                }
            }

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

            if (command == "TreeViewNodeAdd")
            {
                TreeNode node = (TreeNode)args[1];        // 

                return InvokeIfRequiredWithReturn<int>(() => {
                    try
                    {
                        childControl.Nodes.Add(node);
                        return 1;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("TreeViewNodeAdd() Error\n\n" + exception.Message, "Error");
                        return 0;
                    }
                });
            }
            else if (command == "TreeViewNodeClear")
            {
                InvokeIfRequiredVoid(() => childControl.Nodes.Clear());

                return 1;
            }
            else if (command == "TreeViewSetImageList")
            {
                ImageList im = (ImageList)args[1];        // 

                InvokeIfRequiredVoid(() => childControl.ImageList = im);

                return 1;
            }
            else if (command == "TreeViewGetSelectedNode")
            {
                return InvokeIfRequiredWithReturn<TreeNode>(() => childControl.SelectedNode);
            }
            else if (command == "TreeViewExpandAll")
            {
                InvokeIfRequiredVoid(() => childControl.ExpandAll());

                return 1;
            }
            else if (command == "TreeViewSetSelectedNode")
            {
                object tn = args[1];

                InvokeIfRequiredVoid(() => {
                    if (tn == null)
                        childControl.SelectedNode = null;
                    else if (tn.GetType() == typeof(TreeNode))
                        childControl.SelectedNode = (TreeNode)tn;
                });

                return 1;
            }
            else if (command == "TreeViewSeekNode")
            {
                string fullpath = (string)args[1];

                return InvokeIfRequiredWithReturn<TreeNode>(() => {
                    return SeekNodeRecurse(childControl.Nodes, fullpath);
                });
            }

            await Task.CompletedTask;
            return 0;
        }

        public TreeNode SeekNodeRecurse(TreeNodeCollection nodes, string fullpath)
        {
            TreeNode node;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (String.Compare(nodes[i].FullPath, fullpath, true) == 0)
                    return nodes[i];

                node = SeekNodeRecurse(nodes[i].Nodes, fullpath);

                if (node != null)
                    return node;
            }

            return null;
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

                if ((objArgs.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
                {
                    g.DrawRectangle(Pens.Black, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                }
                else
                {
                    DrawClass.PushRectangle2(g, x1, y1, x2, y2);
                }

                Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);
                DrawClass.gcls(g, x1 + 1, y1 + 1, x2 - 1, y2 - 1, brushback);

                r.left = x1 + 1;
                r.top = y1 + 1;
                r.right = x2;
                r.bottom = y2;

                string str;

                str = String.Format("{0}", "TreeView");

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

            if (objArgs.scriptEventDoubleClick != null)
            {
                objArgs.scriptEventDoubleClick.SaveScript(writer, "ScriptEventDoubleClick");
            }
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

        /*
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
        }*/
    }
}

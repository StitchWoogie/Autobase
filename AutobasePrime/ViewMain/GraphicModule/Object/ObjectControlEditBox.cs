using AutoLib;
using AutoLibLocal;
using NetTools;
using NetTools.OldDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsControlEditBox
	{
		public string sTag;
		public EnumWindowStyleFlags dwWindowStyle;
        public Color textColor = Color.Black;
        public BrushPublic backColor = new BrushSolid(Color.White);
        public int nHorzAlign = 0;      // 2012-6-14 10.2.4.3 부터 지원 , 이전버전의 기본은 Left이다.
	}

	/// <summary>
	/// Summary description for ObjectControlEditBox.
	/// </summary>
	[Serializable]
	public class ObjectControlEditBox : ObjectExpand
	{ 
		ObjectArgsControlEditBox objArgs;

		[NonSerialized]
		TextBox editBox = new TextBox();

        [NonSerialized]
        Form formParent; // 20250312 PSU

		// 등록된 클래스 리스트
		[NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

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

		public ObjectControlEditBox(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlEditBox args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.ControlEditBox;
			objArgs = args;

            formParent = form; //20250312 PSU

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{

				form.Controls.Add(editBox);

				if((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0)
                    editBox.PasswordChar = '*';

				if((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0)
                    editBox.BorderStyle = BorderStyle.FixedSingle;
				else
                    editBox.BorderStyle = BorderStyle.None;

				if((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0)
                    editBox.CharacterCasing = CharacterCasing.Upper;
				else if((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0)
                    editBox.CharacterCasing = CharacterCasing.Lower;
				else
                    editBox.CharacterCasing = CharacterCasing.Normal;

                if ((args.dwWindowStyle & EnumWindowStyleFlags.ES_READONLY) > 0)
                    editBox.ReadOnly = true;

				arrayClassList.Add(this);

				string sVal;
				double fVal;
				TagLib.GetTagValue(objArgs.sTag, out sVal, out fVal);

                editBox.Text = sVal;

                editBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Event_KeyDown);
                editBox.TextChanged += new EventHandler(this.Event_TextChanged);
                editBox.ForeColor = this.RunColorText;
                editBox.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22

                if (objArgs.nHorzAlign == 1)
                    editBox.TextAlign = HorizontalAlignment.Center;
                else if (objArgs.nHorzAlign == 2)
                    editBox.TextAlign = HorizontalAlignment.Right;
                else
                    editBox.TextAlign = HorizontalAlignment.Left;

                _ = base.SetToolTipOnChildWindow(editBox);

                OnVisible(ExpandCalcVisible());
			}
		}

		private async void Event_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			await OnEventKeyDown(e.KeyValue);
		}

		private void Event_TextChanged(object sender, EventArgs e)
		{
			if(objArgs.sTag.Length != 0) 
			{
                string text = editBox.Text;

				TagWrite.SetTagValue(objArgs.sTag, text, ConvertTool.ToDouble(text), true).GetAwaiter().GetResult();
			}
		}

		public override void Close()
		{
			arrayClassList.Remove(this);
		}

		public override void OnMove(int x1, int y1, int x2, int y2)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
                if (editBox != null) 
				{
                    // 컨트롤 업데이트 일시 중지 //20250312 PSU 추가, editbox는 모듈 스크롤 관계없음.
                    editBox.SuspendLayout();
                    try
                    {
                        Font font = MakeFont();
                        editBox.Font = font;

                        editBox.Left = x1;
                        editBox.Top = y1;
                        editBox.Width = x2 - x1;
                        editBox.Height = y2 - y1;
                    }
                    finally
                    {
                        // 컨트롤 업데이트 재개
                        editBox.ResumeLayout(false);
                    }
				}
			}
		}

		//void ChangeTag(string text, bool bHandOperation)
		//{
  //          editBox.Text = text;

		//	if(objArgs.sTag.Length == 0)	return;

		//	TagWrite.SetTagValue(objArgs.sTag, text, 0, bHandOperation);
		//}

        public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "EditBoxSetText")
            {
                if (editBox.InvokeRequired)  // 백그라운드 스레드인 경우
                {
                    editBox.Invoke(new Action(() => {
                        editBox.Text = (string)args[1];
                        editBox.Select(editBox.Text.Length, 0);
                    }));
                }
                else  // 이미 UI 스레드인 경우
                {
                    editBox.Text = (string)args[1];
                    editBox.Select(editBox.Text.Length, 0);
                }
            }
            else if (command == "EditBoxGetText")
            {
                args[1] = editBox.Text;
                return 1;
            }
            else if (command == "EditBoxSetFocus")
            {
                //editBox.Focus();
                if (editBox.InvokeRequired)  // 백그라운드 스레드인 경우
                {
                    editBox.Invoke(new Action(() => {
                        editBox.Focus();
                    }));
                }
                else  // 이미 UI 스레드인 경우
                {
                    editBox.Focus();
                }
            }
            else if (command == "EditBoxSelectAll")
            {
                //editBox.SelectAll();
                if (editBox.InvokeRequired)  // 백그라운드 스레드인 경우
                {
                    editBox.Invoke(new Action(() => {
                        editBox.SelectAll();
                    }));
                }
                else  // 이미 UI 스레드인 경우
                {
                    editBox.SelectAll();
                }
            }
            else if (command == "EditBoxGetImeMode")
            {
                return (int)editBox.ImeMode;
            }
            else if (command == "EditBoxSetImeMode")
            {
                int mode = (int)args[1];

                try
                {
                    editBox.ImeMode = (ImeMode)(mode);
                }
                catch
                {

                }
            }
            else if (command == "EditBoxGetReadOnly")
            {
                return editBox.ReadOnly ? 1 : 0;
            }
            else if (command == "EditBoxSetReadOnly")
            {
                //int flag = (int)args[1];
                //editBox.ReadOnly = (flag == 1);
                int flag = (int)args[1];
                if (editBox.InvokeRequired)  // 백그라운드 스레드인 경우
                {
                    editBox.Invoke(new Action(() => {
                        editBox.ReadOnly = (flag == 1);
                    }));
                }
                else  // 이미 UI 스레드인 경우
                {
                    editBox.ReadOnly = (flag == 1);
                }
            }

            await Task.CompletedTask;
            return 0;
        }

        /*
		public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
			if(command == "EditBoxSetText")
			{
                editBox.Text = (string)args[0];
                editBox.Select(editBox.Text.Length, 0);
			}
			else if(command == "EditBoxSetFocus")
			{
                editBox.Focus();
			}
            else if (command == "EditBoxSelectAll")
            {
                editBox.SelectAll();
            }
            else if (command == "EditBoxGetImeMode")
            {
                return (int)editBox.ImeMode;
            }
            else if (command == "EditBoxSetImeMode")
            {
                try
                {
                    editBox.ImeMode = (ImeMode)((int)args[0]);
                }
                catch
                {

                }
            }

			return 0;
		}

		public override string ExecuteClassNameStringReturn(string command, params object[] args)
		{
			if(command == "EditBoxGetText")
			{
                return editBox.Text;
			}

			return "";
		}*/

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);

				Font font = MakeFont();

				int cyChar = (int)font.GetHeight()+1;
				int cxChar = (int)font.SizeInPoints;

				RECT r = new RECT();

				if((objArgs.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) 
				{
					g.DrawRectangle(Pens.Black, x1, y1, x2-x1+1, y2-y1+1);
				}
				else 
				{
                    g.DrawRectangle(Pens.LightGray, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
				}

                Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);
				DrawClass.gcls(g, x1+1, y1+1, x2-1, y2-1, brushback);

				r.left = x1+1;
				r.top =  y1+1;
				r.right = x2;
				r.bottom = y2;

				string str;

				str = objGeneral.GetClassName();

				StringFormat format = new StringFormat();

                if(objArgs.nHorzAlign == 1)
				    format.Alignment = StringAlignment.Center;
                else if (objArgs.nHorzAlign == 2)
                    format.Alignment = StringAlignment.Far;
                else
                    format.Alignment = StringAlignment.Near;

				format.LineAlignment = StringAlignment.Center;
				Brush brush = new SolidBrush(this.RunColorText);

				DrawClass.DrawText(g, str, font, brush, r, format);
			}

            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                UpdateControlState(editBox, formParent);//20250312 PSU
            }
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			SaveObjectItem.TagName(writer, objArgs.sTag);
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
			ObjectSaveFont(writer);
			SaveObjectItem.WindowStyle(writer, objArgs.dwWindowStyle);

            writer.Write("\tStringOption,");
            writer.Write("{0},", objArgs.nHorzAlign);
            writer.WriteLine("");
		}

        protected override void TextColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            editBox.ForeColor = this.RunColorText;
        }

        protected override void BackColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            editBox.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22
        }

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                editBox.Visible = flag;
            }

        }
	}
}


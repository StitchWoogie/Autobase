using AutoLib;
using AutoLibLocal;
using GraphicModule;
using NetTools;
using NetTools.OldDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsControlComboBox
	{
		public string sTag;
		public ArrayList arrayListData;
		public EnumWindowStyleFlags dwWindowStyle;
		public int nValueConvert;
        public Color textColor = Color.Black;
        public BrushPublic backColor = new BrushSolid(Color.White);
	}

	/// <summary>
	/// Summary description for ObjectControlComboBox. 
	/// </summary>
	[Serializable]
	public class ObjectControlComboBox : ObjectExpand
	{
		ObjectArgsControlComboBox objArgs;

		[NonSerialized]
		ComboBox comboBox = new ComboBox();


        [NonSerialized]
        Form formParent; // 20250312 PSU

		// 등록된 클래스 리스트
		[NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

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

		public ObjectControlComboBox(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlComboBox args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.ControlComboBox;
			objArgs = args;

            formParent = form; //20250312 PSU

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				//comboBox = new ComboBox();
				form.Controls.Add(comboBox);

				if((args.dwWindowStyle & EnumWindowStyleFlags.CBS_SORT) == EnumWindowStyleFlags.CBS_SORT)
                    comboBox.Sorted = true;
				else
                    comboBox.Sorted = false;

				if((args.dwWindowStyle & EnumWindowStyleFlags.CBS_NOINTEGRALHEIGHT) == EnumWindowStyleFlags.CBS_NOINTEGRALHEIGHT)
                    comboBox.IntegralHeight = false;
				else
                    comboBox.IntegralHeight = true;

				if((args.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWNLIST) == EnumWindowStyleFlags.CBS_DROPDOWNLIST)
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				else if((args.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWN) == EnumWindowStyleFlags.CBS_DROPDOWN)
                    comboBox.DropDownStyle = ComboBoxStyle.DropDown;
				else
                    comboBox.DropDownStyle = ComboBoxStyle.Simple;

				arrayClassList.Add(this);

				string sVal;
				double fVal;
				TagLib.GetTagValue(objArgs.sTag, out sVal, out fVal);

				if(objArgs.arrayListData != null) 
				{
					int l;
					string buf;
					for(l = 0; l < objArgs.arrayListData.Count; l++) 
					{
						buf = (string)objArgs.arrayListData[l];
                        comboBox.Items.Add(buf);
					}

					if(objArgs.nValueConvert == 1) 
					{
						if(objArgs.arrayListData.Count > (int)fVal)
                            comboBox.SelectedIndex = (int)fVal;
						//SendMessage(hwndCtrl, CB_SETCURSEL, (WPARAM)fVal, 0L);
					}
				}

				if(objArgs.nValueConvert == 0) 
				{
                    comboBox.Text = sVal;
				}

                //comboBox.SelectedIndexChanged += new System.EventHandler(this.Event_SelectedIndexChanged);
                comboBox.SelectedIndexChanged += (sender, e) =>
                {
                    Event_SelectedIndexChanged(sender, e).GetAwaiter().GetResult();
                };

                comboBox.ForeColor = this.RunColorText;
                comboBox.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22

                _ = base.SetToolTipOnChildWindow(comboBox);

                OnVisible(ExpandCalcVisible());
			}
		}

		private async Task Event_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			await ChangeTag(true);
			await OnEventSelChange();
		}

		public override void Close()
		{
			arrayClassList.Remove(this);
		}

		public override void OnMove(int x1, int y1, int x2, int y2)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if (comboBox != null)  //20250312 PSU
				{
					// 컨트롤 업데이트 일시 중지
					comboBox.SuspendLayout();
					try
					{
						Font font = MakeFont();
						comboBox.Font = font;

						comboBox.Left = x1;
						comboBox.Top = y1;
						comboBox.Width = x2 - x1;
						comboBox.Height = y2 - y1;
					}
					finally
					{
						// 컨트롤 업데이트 재개
						comboBox.ResumeLayout(false);
					}
				}
				//MoveWindow(hwndCtrl, x1, y1, x2-x1, y2-y1, TRUE);
				//InvalidateRect(hwndCtrl, NULL, FALSE);
				//InvalidateRect(GetParent(hwndCtrl), NULL, FALSE);	// 배경 화면을 Update한다.
			}
		}

		async Task ChangeTag(bool bHandOperation)
		{
			if(objArgs.sTag.Length == 0)	return;

			string text = "";
			EnumTagType tag_type = 0;
			int[] tag_pos = new int[1];

			if(!TagLib.GetTagTypeAndPos(objArgs.sTag, ref tag_type, ref tag_pos))	return;

            int retn = comboBox.SelectedIndex;

			if(retn == -1)	return;

			if(tag_type == EnumTagType.ST) 
			{
                text = (string)comboBox.SelectedItem;
				await TagWrite.SetTagValue(objArgs.sTag, text, 0, bHandOperation);
			}
			else 
			{
				if(objArgs.nValueConvert == 0) 
				{
                    text = (string)comboBox.SelectedItem;
					await TagWrite.SetTagValue(objArgs.sTag, text, ConvertTool.ToDouble(text), bHandOperation);
				}
				else 
				{
					await TagWrite.SetTagValue(objArgs.sTag, retn.ToString(), retn, bHandOperation);
				}
			}
		}

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

                using (Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2))
                    DrawClass.PushBox2(g, x1, y1, x2, y1 + cyChar + 1, brushback);

				r.left = x1+1;
				r.top =  y1+1;
				r.right = x2-cyChar;
				r.bottom = y1+cyChar+1;

				string str;

				str = objGeneral.GetClassName();

				using (StringFormat format = new StringFormat())
				using (Brush brush = new SolidBrush(this.RunColorText))
				{
					format.Alignment = StringAlignment.Near;
					format.LineAlignment = StringAlignment.Center;
					DrawClass.DrawText(g, str, font, brush, r, format);
				}

				if((objArgs.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWNLIST) > 0) 
				{
					DrawClass.PopBox2(g, x2-cyChar, y1+1, x2-1, y1+cyChar, Color.LightGray);
				}
				else if((objArgs.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWN) > 0) 
				{
					DrawClass.PopBox2(g, x2-cyChar, y1+1, x2-1, y1+cyChar, Color.LightGray);
				}
				else 
				{
					DrawClass.PushBox2(g, x1, y1+cyChar+2, x2, y2, Color.LightGray);
				}
			}
            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                UpdateControlState(comboBox, formParent);//20250312 PSU
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

			if(objArgs.arrayListData != null && objArgs.arrayListData.Count > 0) {
				int l;
				string buf;
				for(l = 0; l < objArgs.arrayListData.Count; l++) {
					buf = (string)objArgs.arrayListData[l];
					writer.WriteLine("\tListData,{0},", buf);
				}
			}
		}

        protected override void TextColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            comboBox.ForeColor = this.RunColorText;
        }

        protected override void BackColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            comboBox.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22
        }

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
            // UI 스레드에서 실행해야 하는 작업을 위한 헬퍼 메서드
            T InvokeIfRequiredWithReturn<T>(Func<T> action)
            {
                if (this.comboBox.InvokeRequired)
                {
                    return (T)this.comboBox.Invoke(action);
                }
                else
                {
                    return action();
                }
            }

            void InvokeIfRequired(Action action)
            {
                if (this.comboBox.InvokeRequired)
                {
                    this.comboBox.Invoke(action);
                }
                else
                {
                    action();
                }
            }

            if (command == "ComboBoxAddString") 
			{
                InvokeIfRequired(() => this.comboBox.Items.Add(args[1]));
                return 1;
			}
			else if(command == "ComboBoxSetCurSel") 
			{
                int index = (int)args[1];
                return InvokeIfRequiredWithReturn<int>(() => {
                    if (index >= comboBox.Items.Count || index < 0) return 0;
                    comboBox.Items.RemoveAt(index);
                    return 1;
                });
            }
			else if(command == "ComboBoxGetCurSel") 
			{
                return InvokeIfRequiredWithReturn<int>(() => this.comboBox.SelectedIndex);
            }
			else if(command == "ComboBoxResetContent") 
			{
                InvokeIfRequired(() => this.comboBox.Items.Clear());
                return 1;
			}
			else if(command == "ComboBoxDeleteString") 
			{
                int index = (int)args[1];
                return InvokeIfRequiredWithReturn<int>(() => {
                    if (index >= comboBox.Items.Count || index < 0) return 0;
                    comboBox.Items.RemoveAt(index);
                    return 1;
                });
            }
			else if(command == "ComboBoxSetText") 
			{
                InvokeIfRequired(() => this.comboBox.Text = (string)args[1]);
                return 1;
            }
			else if(command == "ComboBoxGetText") 
			{
                return InvokeIfRequiredWithReturn<string>(() => this.comboBox.Text);
            }
			else if(command == "ComboBoxGetItemCount") 
			{
                return InvokeIfRequiredWithReturn<int>(() => this.comboBox.Items.Count);
            }
			else if(command == "ComboBoxGetItemText") 
			{
                int index = (int)args[1];
                return InvokeIfRequiredWithReturn<string>(() => {
                    if (index >= comboBox.Items.Count || index < 0) return "";
                    return this.comboBox.Items[index].ToString();
                });
            }
            else if (command == "ComboBoxGetImeMode")
            {
                return InvokeIfRequiredWithReturn<int>(() => (int)comboBox.ImeMode);
            }
            else if (command == "ComboBoxSetImeMode")
            {
                InvokeIfRequired(() => {
                    try
                    {
                        comboBox.ImeMode = (ImeMode)((int)args[1]);
                    }
                    catch
                    {
                    }
                });
                return 1;
            }
			else 
			{
                await Task.CompletedTask;
                return 0;
			}
		}

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                comboBox.Visible = flag;
            }
            
        }
	}
}







using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using NetTools;
using NetTools.OldDefine;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsControlListBox
	{
		public string sTag;
		public ArrayList arrayListData;
		public EnumWindowStyleFlags dwWindowStyle;
		public int nValueConvert;
        public Color textColor = Color.Black;
        public BrushPublic backColor = new BrushSolid(Color.White);
        public int nSelectionMode = 1;  // default - ONE select
	}

	/// <summary>
	/// Summary description for ObjectControlListBox.
	/// </summary>
	[Serializable]
	public class ObjectControlListBox : ObjectExpand 
	{
		ObjectArgsControlListBox objArgs;

		[NonSerialized]
		ListBox listBox = new ListBox();

        [NonSerialized]
        Form formParent; // 20250312 PSU

		// 등록된 클래스 리스트
		[NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

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

		public ObjectControlListBox(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsControlListBox args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.ControlListBox;
			objArgs = args;

            formParent = form; //20250312 PSU

            SetTextColor(args.textColor);
            SetBackColor(args.backColor);

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				form.Controls.Add(listBox);

				if((args.dwWindowStyle & EnumWindowStyleFlags.LBS_SORT) == 0) 
					listBox.Sorted = false;
				else
					listBox.Sorted = true;

				if((args.dwWindowStyle & EnumWindowStyleFlags.LBS_NOINTEGRALHEIGHT) == 0) 
					listBox.IntegralHeight = true;
				else
					listBox.IntegralHeight = false;

				if((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) 
					listBox.BorderStyle = BorderStyle.FixedSingle;
				else
					listBox.BorderStyle = BorderStyle.None;

				if((args.dwWindowStyle & EnumWindowStyleFlags.WS_VSCROLL) > 0) 
					listBox.ScrollAlwaysVisible = true;
				else
					listBox.ScrollAlwaysVisible = false;

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
						listBox.Items.Add(buf);
					}

					if(objArgs.nValueConvert == 1) 
					{
						if((int)fVal >= 0 && (int)fVal < listBox.Items.Count)
							listBox.SelectedIndex = (int)fVal;
					}
				}

				if(objArgs.nValueConvert == 0) 
				{
					listBox.Text = sVal;
				}

				//listBox.SelectedIndexChanged += new System.EventHandler(this.Event_SelectedIndexChanged);
				listBox.SelectedIndexChanged += async (sender, e) => await Event_SelectedIndexChanged(sender, e);

                listBox.ForeColor = this.RunColorText;
                listBox.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22

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
                
				_ = base.SetToolTipOnChildWindow(listBox);
                OnVisible(ExpandCalcVisible());
			}
		}

		private async Task Event_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ChangeTag(true);
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
				if(listBox != null)    //20250312 PSU 
				{
					// 컨트롤 업데이트 일시 중지
					listBox.SuspendLayout();
					try
					{
						Font font = MakeFont();
						listBox.Font = font;

						listBox.Left = x1;
						listBox.Top = y1;
						listBox.Width = x2 - x1;
						listBox.Height = y2 - y1;
					}
					finally
					{
						// 컨트롤 업데이트 재개
						listBox.ResumeLayout(false);
					}
				}
			}
		}

		void ChangeTag(bool bHandOperation)
		{
			if(objArgs.sTag.Length == 0)	return;

			string text = "";
			EnumTagType tag_type = 0;
			int[] tag_pos = new int[1];

			if(!TagLib.GetTagTypeAndPos(objArgs.sTag, ref tag_type, ref tag_pos))	return;

			int retn = listBox.SelectedIndex;

			if(retn == -1)	return;

			if(tag_type == EnumTagType.ST) 
			{
				text = (string)listBox.Items[retn];
				TagWrite.SetTagValue(objArgs.sTag, text, 0, bHandOperation);
			}
			else 
			{
				if(objArgs.nValueConvert == 0) 
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

			if(!Directory.Exists(dir_name))	return;

			DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(dir));

			if(method == 1)
			{
				foreach(DirectoryInfo di in info.GetDirectories("*.*"))
				{
					listBox.Items.Add(di.Name);
				}
			}
			else if(method == 2)
			{
				foreach(FileInfo fi in info.GetFiles(Path.GetFileName(dir)))
				{
					listBox.Items.Add(fi.Name);
				}
			}
		}

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
            // UI 스레드에서 실행해야 하는 작업을 위한 헬퍼 메서드
            T InvokeIfRequiredWithReturn<T>(Func<T> action)
            {
                if (this.listBox.InvokeRequired)
                {
                    return (T)this.listBox.Invoke(action);
                }
                else
                {
                    return action();
                }
            }

            void InvokeIfRequiredVoid(Action action)
            {
                if (this.listBox.InvokeRequired)
                {
                    this.listBox.Invoke(action);
                }
                else
                {
                    action();
                }
            }

            if (command == "ListBoxAddString") 
			{
                InvokeIfRequiredVoid(() => listBox.Items.Add(args[1]));
            }
            else if (command == "ListBoxInsertString")
            {
                int index = (int)args[1];
                string text = (string)args[2];
                InvokeIfRequiredVoid(() => {
                    if (index > listBox.Items.Count)
                    {
                        index = listBox.Items.Count;
                    }
                    listBox.Items.Insert(index, text);
                });
                return 0;
            }
			else if(command == "ListBoxGetCurSel") 
			{
                return InvokeIfRequiredWithReturn<int>(() => listBox.SelectedIndex);
            }
			else if(command == "ListBoxSetCurSel") 
			{
				int index = (int)args[1];
                return InvokeIfRequiredWithReturn<int>(() => {
                    if (index >= listBox.Items.Count || index < -1) index = -1;
                    listBox.SelectedIndex = index;
                    return index;
                });
            }
			else if(command == "ListBoxResetContent") 
			{
                InvokeIfRequiredVoid(() => listBox.Items.Clear());
                return 0;
            }
			else if(command == "ListBoxFillDir") 
			{
                InvokeIfRequiredVoid(() => FillDirFile((string)args[1], 0x0001));
            }
			else if(command == "ListBoxFillFile") 
			{
				FillDirFile((string)args[1], 0x0002);
			}
			else if(command == "ListBoxDeleteString") 
			{
				int index = (int)args[1];
                return InvokeIfRequiredWithReturn<int>(() => {
                    if (index >= listBox.Items.Count || index < 0) return 0;
                    listBox.Items.RemoveAt(index);
                    return 0;
                });
            }
			else if(command == "ListBoxGetItemCount") 
			{
                return InvokeIfRequiredWithReturn<int>(() => this.listBox.Items.Count);
            }
			else if(command == "ListBoxGetItemText") 
			{
				int index = (int)args[1];
                return InvokeIfRequiredWithReturn<string>(() => {
                    if (index >= listBox.Items.Count || index < 0) return "";
                    return this.listBox.Items[index].ToString();
                });
            }
            else if (command == "ListBoxGetSel")
            {
                int index = (int)args[1];
                return InvokeIfRequiredWithReturn<int>(() => {
                    if (index >= listBox.Items.Count || index < 0) return 0;
                    for (int i = 0; i < listBox.SelectedIndices.Count; i++)
                    {
                        if (listBox.SelectedIndices[i] == index) return 1;
                    }
                    return 0;
                });
            }
            else if (command == "ListBoxGetSelCount")
            {
                return InvokeIfRequiredWithReturn<int>(() => this.listBox.SelectedItems.Count);
            }
            else if (command == "ListBoxGetText")
            {
                int index = (int)args[1];

                InvokeIfRequiredVoid(() => {
                    if (index >= this.listBox.Items.Count)
                        args[2] = "";
                    else if (index < 0)
                        args[2] = "";
                    else
                        args[2] = this.listBox.Items[index].ToString();
                });
                return 1;
            }
			else 
			{

			}

			await Task.CompletedTask;
			return 0;
		}

        /*
        public override string ExecuteClassNameStringReturn(string command, params object[] args)
        {
            if(command == "ListBoxGetText") 
			{
				int index = (int)args[0];
				if(index >= this.listBox.Items.Count)
					return "";
				else if(index < 0)	// -1
					return "";
				else
					return (string)this.listBox.Items[index];
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
					DrawClass.PushRectangle2(g, x1, y1, x2, y2);
				}

                Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);
                DrawClass.gcls(g, x1 + 1, y1 + 1, x2 - 1, y2 - 1, brushback);

				if(objArgs.arrayListData != null && objArgs.arrayListData.Count > 0) 
				{
					int l;
					string buf;
					int  y;
					for(l = 0, y = y1+1; l < objArgs.arrayListData.Count && y < y2-cyChar; l++, y+=cyChar) 
					{
						buf = (string)objArgs.arrayListData[l];
						r.left = x1+1;
						r.top =  y;
						r.right = x2-1;
						r.bottom = y+cyChar;

						StringFormat format = new StringFormat();
						format.Alignment = StringAlignment.Near;
						format.LineAlignment = StringAlignment.Center;

						DrawClass.DrawText(g, buf, font, new SolidBrush(this.RunColorText), r, format);
					}
				}
				else 
				{
					r.left = x1+1;
					r.top =  y1+1;
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
            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                  UpdateControlState(listBox, formParent); //20250312 PSU
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

			if(objArgs.arrayListData != null && objArgs.arrayListData.Count > 0) 
			{
				int l;
				string buf;
				for(l = 0; l < objArgs.arrayListData.Count; l++) 
				{
					buf = (string)objArgs.arrayListData[l];
					writer.WriteLine("\tListData,{0},", buf);
				}
			}
		}

        protected override void TextColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            listBox.ForeColor = this.RunColorText;
        }

        protected override void BackColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            listBox.BackColor = Color.FromArgb(RunColorBack.basic_color.R, RunColorBack.basic_color.G, RunColorBack.basic_color.B); // 윈도우 컨트롤은 투명한 배경색을 지정하면 안됨 2009.7.22
        }

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                listBox.Visible = flag;
            }

        }
	}
}




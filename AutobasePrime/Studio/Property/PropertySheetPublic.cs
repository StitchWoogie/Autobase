using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools.OldDefine;
using GraphicModule;
using NetTools;
using System.Linq;

namespace Studio
{

	public delegate void Property_Recv(object obj, Form prop);
	/// <summary>
	/// Summary description for PropertySheetPublic.
	/// </summary>
	public class PropertySheetPublic : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl m_tab;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel; 
		private System.Windows.Forms.Button buttonApply;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertySheetPublic()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			this.Left = nTempStartX;
			this.Top = nTempStartY;

			sizeCalc = new Size(this.m_tab.ClientRectangle.Width, this.m_tab.ClientRectangle.Height);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertySheetPublic));
            this.m_tab = new System.Windows.Forms.TabControl();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_tab
            // 
            this.m_tab.AccessibleDescription = null;
            this.m_tab.AccessibleName = null;
            resources.ApplyResources(this.m_tab, "m_tab");
            this.m_tab.BackgroundImage = null;
            this.m_tab.Font = null;
            this.m_tab.Multiline = true;
            this.m_tab.Name = "m_tab";
            this.m_tab.SelectedIndex = 0;
            this.m_tab.SelectedIndexChanged += new System.EventHandler(this.m_tab_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.AccessibleDescription = null;
            this.buttonApply.AccessibleName = null;
            resources.ApplyResources(this.buttonApply, "buttonApply");
            this.buttonApply.BackgroundImage = null;
            this.buttonApply.Font = null;
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // PropertySheetPublic
            // 
            this.AcceptButton = this.buttonApply;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.m_tab);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertySheetPublic";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PropertySheetPublic_Load);
            this.Closed += new System.EventHandler(this.PropertySheetPublic_Closed);
            this.Move += new System.EventHandler(this.PropertySheetPublic_Move);
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			SharedStudio.formMain.Select();	// 이게 없으면 Focus가 다른 프로그램으로 갈때가 많다.

			Close();
		}

		Size sizeCalc;  // 탭의 클라이언트 크기

		ArrayList listPages = new ArrayList();

		void AddOnlyPageTitle(Form form)
		{
			listPages.Add(form);
			TabPage page = new TabPage(form.Text);
			m_tab.TabPages.Add(page);
		}

		/// <summary>
		/// 이전에 등록되어 있는 Form을 반납해 주어야 새로운 오브젝트에서 알수 있다.
        /// use_at_multi가 true일때는 반환된 값을 사용하여 form을 바꾸어야 한다.
		/// </summary>
		/// <param name="form"></param>
		/// <param name="use_at_multi"></param>
		/// <param name="multi_select"></param>
		/// <returns></returns>

		public Form AddPage(Form form, bool use_at_multi, bool multi_select)
		{
			if(use_at_multi == false && multi_select == true) 
			{	// 멀티에서는 사용하지 않는 특성
				return form;
			}

			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Text == save.Text) 
				{
					return save;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			return form;
		}

		public void AddPageFont(object obj)
		{
			PropertyPageFont form = new PropertyPageFont();

			LOGFONT lf = ((ObjectFont)obj).GetLogFont();

			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Text == save.Text) 
				{
					((PropertyPageFont)save).SetLogFont(lf, false);
					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			form.SetLogFont(lf, true);
		}

        public void AddPageControlBox(object obj)
        {
            PropertyPageControlBox form = new PropertyPageControlBox();

            //LOGFONT lf = ((ObjectFont)obj).GetLogFont();

            Form save;
            for (int i = 0; i < listPages.Count; i++)
            {
                save = (Form)listPages[i];
                if (form.Text == save.Text)
                {
                    ((PropertyPageControlBox)save).SetProp(((ObjectTag)obj).MouseResponse);
                    return;	// 이미 등록되어 있다.
                }
            }

            AddOnlyPageTitle(form);

            form.SetProp(((ObjectTag)obj).MouseResponse);
        }

		public void AddPageClassName(object obj, bool multi_select)
		{
			PropertyPageClassName form = new PropertyPageClassName(); 

			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Text == save.Text) 
				{
					form = (PropertyPageClassName)save;
					goto next;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			next:;

			// 클래스 명은 1개일때만 정상적으로 기입한다.
			if(multi_select)	// 두 개 이상 선택되어 있을 때
			{
				form.textBoxClassName.ReadOnly = true;		// 두개 이상일 때는 편집할 수 없다.
				string class_name = ((ObjectExpand)obj).objGeneral.GetClassName();
				form.textBoxClassName.Text += class_name+"; ";
			}
			else 
			{
				string class_name = ((ObjectExpand)obj).objGeneral.GetClassName();
				form.textBoxClassName.Text = class_name;
			}

			form.Set((ObjectExpand)obj);
		}

		public void AddPageExpand(PropertyPageExpandOption form, object obj, bool multi_select)
		{
			if(multi_select)	return;	

			//string class_name = ((ObjectExpand)obj).GetClassName();

			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Text == save.Text) 
				{
					//((PropertyPageClassName)save)..SetLogFont(lf, false);
					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			form.ObjectToDialog((ObjectExpand)obj);
			

			//form.textBoxClassName.Text = class_name;
		}

		public void AddPageColor(PropertyPageColor form, Color color)
		{
			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Tag == save.Tag)
				{
					((PropertyPageColor)save).SetSelectedColor(color, false);
					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			form.SetSelectedColor(color, true);
		}

        public void AddPageBrush(PropertyPageBrush form, BrushPublic color)
        {
            Form save;
            for (int i = 0; i < listPages.Count; i++)
            {
                save = (Form)listPages[i];
                if (form.Tag == save.Tag)
                {
                    ((PropertyPageBrush)save).SetSelectedBrush(color, false);
                    return;	// 이미 등록되어 있다.
                }
            }

            AddOnlyPageTitle(form);

            form.SetSelectedBrush(color, true);
        }

		public void AddPageBackColor(object obj)
		{
			string title;
			if(Tools.IsLangKorean())		title = "배경색";
			else if(Tools.IsLangJapanese())	title = "背景色";
			else if(Tools.IsLangChinese())	title = "背景颜色";
            else if (Tools.IsLangVietnamese()) title = "Màu nền";
			else							title = "BackColor";

            PropertyPageBrush form = new PropertyPageBrush("BackColor", title);

            BrushPublic color = ((ObjectExpand)obj).GetBackColor();

			AddPageBrush(form, color);
		}

		public void AddPageTextColor(object obj)
		{
			string title;
			if(Tools.IsLangKorean())		title = "글자색";
			else if(Tools.IsLangJapanese())	title = "テキスト色";
			else if(Tools.IsLangChinese())	title = "文字颜色";
            else if (Tools.IsLangVietnamese()) title = "Màu chữ";
			else							title = "TextColor";

			PropertyPageColor form = new PropertyPageColor("TextColor", title);	

			Color color = ((ObjectExpand)obj).GetTextColor();

			AddPageColor(form, color);
		}

		public void AddPageLineColor(object obj)
		{
			PropertyPageColorLine form = new PropertyPageColorLine();

			Color color = ((ObjectExpand)obj).GetLineColor();
			int thick = ((ObjectExpand)obj).GetBorderThick();
			int option = ((ObjectExpand)obj).nLineOption;

			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Name == save.Name)
				{
					((PropertyPageColorLine)save).SetSelectedColor(color, false);
					((PropertyPageColorLine)save).SetSelectedThick(thick, false);
					((PropertyPageColorLine)save).SetSelectedOption(option, false);

					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			form.SetSelectedColor(color, true);
			form.SetSelectedThick(thick, true);
			form.SetSelectedOption(option, true);
		}

        public void AddPageButtonThick(object obj)
        {
            PropertyPageButtonThick form = new PropertyPageButtonThick();

            Color color = ((ObjectExpand)obj).GetLineColor();
            int thick = ((ObjectExpand)obj).GetBorderThick();
            int option = ((ObjectExpand)obj).nLineOption;

            int designType = 0;
            int radius = 0;
            if (obj is GraphicModule.ObjectButtonPublic)
            {
                designType = (int)((GraphicModule.ObjectButtonPublic)obj).DesignType;
                radius = ((GraphicModule.ObjectButtonPublic)obj).Radius;
            }

            Form save;
            for (int i = 0; i < listPages.Count; i++)
            {
                save = (Form)listPages[i];
                if (form.Name == save.Name)
                {
                    //((PropertyPageButtonThick)save).SetSelectedColor(color, false);
                    ((PropertyPageButtonThick)save).SetSelectedThick(thick, false);
                    ((PropertyPageButtonThick)save).SetSelectedOption(option, false);
                    ((PropertyPageButtonThick)save).SetSelectedDesignType(designType, false);
                    ((PropertyPageButtonThick)save).SetSelectedRadius(radius, false);
                    ((PropertyPageButtonThick)save).SetSelectedLineColor(color, false);

                    return;	// 이미 등록되어 있다.
                }
            }

            AddOnlyPageTitle(form);

            //form.SetSelectedColor(color, true);
            form.SetSelectedThick(thick, true);
            form.SetSelectedOption(option, true);
            form.SetSelectedDesignType(designType, true);
            form.SetSelectedRadius(radius, true);
            form.SetSelectedLineColor(color, true);
        }

		public void AddPageFillColor(object obj)
		{
            string title;
            if (Tools.IsLangKorean()) title = "채움색";
            else if (Tools.IsLangJapanese()) title = "塗りつぶし色";
            else if (Tools.IsLangChinese()) title = "填充颜色";
            else if (Tools.IsLangVietnamese()) title = "Màu tô";
            else title = "FillColor";

            PropertyPageBrush form = new PropertyPageBrush("FillColor", title);

            BrushPublic color = ((ObjectExpand)obj).GetFillColor();

            AddPageBrush(form, color);

            /*
            string title;
            if (Tools.IsLangKorean()) title = "채움색";
            else if (Tools.IsLangJapanese()) title = "塗りつぶし色";
            else if (Tools.IsLangChinese()) title = "填充颜色";
            else title = "FillColor";

            PropertyPageBrush form = new PropertyPageBrush("FillColor", title); 

			BrushPublic color = ((ObjectExpand)obj).GetFillColor();
			int option = ((ObjectExpand)obj).nFillOption;

			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Name == save.Name)
				{
                    ((PropertyPageBrush)save).SetSelectedBrush(color, false);
                    //((PropertyPageBrush)save).SetSelectedOption(option, false);

					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

            form.SetSelectedBrush(color, true);
			//form.SetSelectedOption(option, true);
            */
		}

		void AddPageTag(PropertyPageTag form, object obj)
		{
			Form save;
			string tag = ((ObjectTag)obj).sTagName;

			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Name == save.Name)
				{
					((PropertyPageTag)save).SetTag(tag);
					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			form.SetTag(tag);
		}

		public void AddPageTagAI(object obj)
		{
			PropertyPageTag form = new PropertyPageTag();
			form.bUseAI = true;
			AddPageTag(form, obj);
		}

		public void AddPageTagDI(object obj)
		{
			PropertyPageTag form = new PropertyPageTag();
			form.bUseDI = true;
			AddPageTag(form, obj);
		}

		public void AddPageTagST(object obj)
		{
			PropertyPageTag form = new PropertyPageTag();
			form.bUseST = true;
			AddPageTag(form, obj);
		}

		// 콘트롤 오브젝트에서 사용한다.
		public void AddPageTagLocalAll(object obj, string tag)
		{
			PropertyPageTag form = new PropertyPageTag();
			form.bUseAI = true;
			form.bUseAO = true;
			form.bUseDI = true;
			form.bUseDO = true;
			form.bUseST = true;
			
			Form save;

			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Name == save.Name)
				{
					((PropertyPageTag)save).SetTag(tag);
					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			form.SetTag(tag);
		}

		public void AddPageMouseResponseDigital(object obj, bool multi_select)
		{
            PropertyPageMouseResponseDigital form = new PropertyPageMouseResponseDigital();

            Form save;
            for (int i = 0; i < listPages.Count; i++)
            {
                save = (Form)listPages[i];
                if (save.GetType() == typeof(PropertyPageMouseResponseDigital))
                {
                    ((PropertyPageMouseResponseDigital)save).SetMouseResponse(((ObjectTag)obj).MouseResponse, multi_select);
                    return;	// 이미 등록되어 있다.
                }
            }

            AddOnlyPageTitle(form);

            form.SetMouseResponse(((ObjectTag)obj).MouseResponse, multi_select);

            /*
			if(multi_select)	return;

			PropertyPageMouseResponseDigital form = new PropertyPageMouseResponseDigital(); 

			AddOnlyPageTitle(form);

			form.SetMouseResponse(((ObjectTag)obj).MouseResponse);
             */
		}

		public void AddPageMouseResponseAnalog(object obj, bool multi_select)
		{
			PropertyPageMouseResponseAnalog form = new PropertyPageMouseResponseAnalog();

            Form save;
            for (int i = 0; i < listPages.Count; i++)
            {
                save = (Form)listPages[i];
                if (save.GetType() == typeof(PropertyPageMouseResponseAnalog))
                {
                    ((PropertyPageMouseResponseAnalog)save).SetMouseResponse(((ObjectTag)obj).MouseResponse, multi_select);
                    return;	// 이미 등록되어 있다.
                }
            }

			AddOnlyPageTitle(form);

			form.SetMouseResponse(((ObjectTag)obj).MouseResponse, multi_select);
		}

		public void AddPageMouseResponseString(object obj, bool multi_select)
		{
            PropertyPageMouseResponseString form = new PropertyPageMouseResponseString();

            Form save;
            for (int i = 0; i < listPages.Count; i++)
            {
                save = (Form)listPages[i];
                if (save.GetType() == typeof(PropertyPageMouseResponseString))
                {
                    ((PropertyPageMouseResponseString)save).SetMouseResponse(((ObjectTag)obj).MouseResponse, multi_select);
                    return;	// 이미 등록되어 있다.
                }
            }

            AddOnlyPageTitle(form);

            form.SetMouseResponse(((ObjectTag)obj).MouseResponse, multi_select);
            /*
			if(multi_select)	return;

			PropertyPageMouseResponseString form = new PropertyPageMouseResponseString();

			AddOnlyPageTitle(form);

			form.SetMouseResponse(((ObjectTag)obj).MouseResponse);*/
		}

        public void AddPageMouseResponse(object obj, bool multi_select)
        {
            PropertyPageMouseResponse form = new PropertyPageMouseResponse();

            Form save;
            for (int i = 0; i < listPages.Count; i++)
            {
                save = (Form)listPages[i];
                if (save.GetType() == typeof(PropertyPageMouseResponse))
                {
                    ((PropertyPageMouseResponse)save).SetMouseResponse(((ObjectTag)obj).MouseResponse, multi_select);
                    return;	// 이미 등록되어 있다.
                }
            }

            AddOnlyPageTitle(form);

            form.SetMouseResponse(((ObjectTag)obj).MouseResponse, multi_select);

            /*
            if (multi_select) return;

            PropertyPageMouseResponse form = new PropertyPageMouseResponse();

            AddOnlyPageTitle(form);

            form.SetMouseResponse(((ObjectTag)obj).MouseResponse);*/
        }

		// 콘트롤 오브젝트에서 사용한다.
		public void AddPageListData(object obj, ArrayList array, bool multi_select)
		{
			if(multi_select)	return;

			PropertyPageListData form = new PropertyPageListData();

			AddOnlyPageTitle(form);

			form.ListData = array;
		}

		bool bFlagEnable = false;
		FormEditGraphic formParent = null;

		public void Prepare(FormEditGraphic form)
		{
			formParent = form;
			bFlagEnable = false;
			m_tab.TabPages.Clear();
			listObject.Clear();
			listPages.Clear();
		}

		bool bShowFlag = false;

        public void Run()
        {
            DebugSpeed speed = new DebugSpeed();
            speed.Start();

            for (int i = 0; i < m_tab.TabPages.Count; i++)
            {
                if (sSelectedTabText == m_tab.TabPages[i].Text)
                {
                    m_tab.SelectedIndex = i;
                    break;
                }
            }

            if (listPages.Count == 0)
            {
                PropertyPageNotObject not = new PropertyPageNotObject();
                AddOnlyPageTitle(not);
                this.buttonApply.Enabled = false;
                this.buttonOK.Enabled = false;
            }
            else
            {
                this.buttonApply.Enabled = true;
                this.buttonOK.Enabled = true;
            }

            bool flag_size_changed = false;

            for (int i = 0; i < listPages.Count; i++)
            {
                Form form = (Form)listPages[i];

                if (form.ClientSize.Width > sizeCalc.Width ||
                    form.ClientSize.Height > sizeCalc.Height)
                {
                    int width = form.ClientSize.Width > sizeCalc.Width ? form.ClientSize.Width : sizeCalc.Width;
                    int height = form.ClientSize.Height > sizeCalc.Height ? form.ClientSize.Height : sizeCalc.Height;
                    if (width != sizeCalc.Width || height != sizeCalc.Height)
                    {
                        flag_size_changed = true;
                        sizeCalc = new Size(width, height);
                    }
                }

                form.FormBorderStyle = FormBorderStyle.None;
                form.TopLevel = false;
                form.Parent = m_tab.TabPages[i];
                m_tab.TabPages[i].Controls.Add(form);
                form.Show();
            }

            // 탭 사이즈 보다 큰것이 발견되면 Sheet를 확장해 준다. 
            if (flag_size_changed)
            {
                m_tab.Size = new Size(sizeCalc.Width + 8, sizeCalc.Height + m_tab.RowCount * m_tab.ItemSize.Height + 8);

                Size size = new Size(m_tab.Size.Width + 16, m_tab.Size.Height + 50);
                this.ClientSize = size;

                this.buttonCancel.Left = this.ClientRectangle.Right - this.buttonCancel.Size.Width - 8;
                this.buttonCancel.Top = this.ClientRectangle.Bottom - this.buttonCancel.Size.Height - 8;

                this.buttonOK.Left = this.buttonCancel.Left - this.buttonOK.Size.Width - 8;
                this.buttonOK.Top = this.ClientRectangle.Bottom - this.buttonOK.Size.Height - 8;

                this.buttonApply.Left = this.buttonOK.Left - this.buttonApply.Size.Width - 8;
                this.buttonApply.Top = this.ClientRectangle.Bottom - this.buttonApply.Size.Height - 8;
            }


            
            this.Owner = SharedStudio.formMain;

            //if(!this.Visible)	this.Visible = true;
            if (!bShowFlag)
            {
                this.Show();
                bShowFlag = true;
            }
            else
            {
                //this.Visible = true;
            }

            bFlagEnable = true;

            if (listObject.Count == 1)
            {
                OBJECT_LIST list = (OBJECT_LIST)listObject[0];
                string name = list.obj.ToString();
                name = name.Substring(name.IndexOf('.') + 1);
                if (Tools.IsLangKorean())
                    this.Text = String.Format("요소 속성 ({0})", name);
                else if (Tools.IsLangJapanese())
                    this.Text = String.Format("オブジェクトのプロパティ ({0})", name);
                else if (Tools.IsLangChinese())
                    this.Text = String.Format("对象属性 ({0})", name);
                else if (Tools.IsLangVietnamese())
                    this.Text = String.Format("Thuộc tính đối tượng ({0})", name);
                else
                    this.Text = String.Format("Object Properties ({0})", name);
            }
            else
            {
                if (Tools.IsLangKorean())
                    this.Text = String.Format("요소 속성");
                else if (Tools.IsLangJapanese())
                    this.Text = String.Format("オブジェクトのプロパティ");
                else if (Tools.IsLangChinese())
                    this.Text = String.Format("对象属性");
                else if (Tools.IsLangVietnamese())
                    this.Text = String.Format("Thuộc tính đối tượng");
                else
                    this.Text = String.Format("Object Properties");
            }

            speed.Stop("ProperySheet Loading");

            for (int i = 0; i < listPages.Count; i++)
            {
                Form form = (Form)listPages[i];

                if (form.GetType() == typeof(PropertyPageObjectAnalogString))
                    ((PropertyPageObjectAnalogString)form).AfterSheetRun();
            }
        }

        /*
		public void Run()
		{
            DebugSpeed speed = new DebugSpeed();
            speed.Start();

			for(int i = 0; i < m_tab.TabPages.Count; i++) 
			{
				if(sSelectedTabText == m_tab.TabPages[i].Text) 
				{
					m_tab.SelectedIndex = i;
					break;
				}
			}

			if(listPages.Count == 0) 
			{
				PropertyPageNotObject not = new PropertyPageNotObject();
				AddOnlyPageTitle(not);
			}

			for(int i = 0; i < listPages.Count; i++) 
			{
				Form form = (Form)listPages[i];
				
				if(form.ClientSize.Width > sizeCalc.Width ||
					form.ClientSize.Height > sizeCalc.Height) 
				{
					int width = form.ClientSize.Width > sizeCalc.Width ? form.ClientSize.Width : sizeCalc.Width;
					int height = form.ClientSize.Height > sizeCalc.Height ? form.ClientSize.Height : sizeCalc.Height;
					sizeCalc = new Size(width, height);

					m_tab.Size = new Size(width+8, height+m_tab.RowCount*m_tab.ItemSize.Height+8);

					Size size = new Size(m_tab.Size.Width+16, m_tab.Size.Height+50);
					this.ClientSize = size;

					this.buttonCancel.Left = this.ClientRectangle.Right-this.buttonCancel.Size.Width-8;
					this.buttonCancel.Top = this.ClientRectangle.Bottom-this.buttonCancel.Size.Height-8;

					this.buttonOK.Left = this.buttonCancel.Left-this.buttonOK.Size.Width-8;
					this.buttonOK.Top = this.ClientRectangle.Bottom-this.buttonOK.Size.Height-8;

					this.buttonApply.Left = this.buttonOK.Left-this.buttonApply.Size.Width-8;
					this.buttonApply.Top = this.ClientRectangle.Bottom-this.buttonApply.Size.Height-8;
				}

				form.FormBorderStyle = FormBorderStyle.None;
				form.TopLevel = false;
				form.Parent = m_tab.TabPages[i];
				m_tab.TabPages[i].Controls.Add(form);
				form.Show();
			}

			this.Owner = SharedStudio.formMain;

			//if(!this.Visible)	this.Visible = true;
			if(!bShowFlag) 
			{
				this.Show();
				bShowFlag = true;
			}
			else 
			{
				//this.Visible = true;
			}

			bFlagEnable = true;

			if(listObject.Count == 1) 
			{
				OBJECT_LIST list = (OBJECT_LIST)listObject[0];
				string name = list.obj.ToString();
				name = name.Substring(name.IndexOf('.')+1);
				if(Tools.IsLangKorean()) 
					this.Text = String.Format("요소 속성 ({0})", name);
				else if(Tools.IsLangJapanese()) 
					this.Text = String.Format("オブジェクトのプロパティ ({0})", name);
				else if(Tools.IsLangChinese()) 
					this.Text = String.Format("对象属性 ({0})", name);
                else if (Tools.IsLangVietnamese())
                    this.Text = String.Format("Thuộc tính đối tượng ({0})", name);
				else 
					this.Text = String.Format("Object Properties ({0})", name);
			}
			else 
			{
				if(Tools.IsLangKorean()) 
					this.Text = String.Format("요소 속성");
				else if(Tools.IsLangJapanese()) 
					this.Text = String.Format("オブジェクトのプロパティ");
				else if(Tools.IsLangChinese()) 
					this.Text = String.Format("对象属性");
                else if (Tools.IsLangVietnamese())
                    this.Text = String.Format("Thuộc tính đối tượng");
				else 
					this.Text = String.Format("Object Properties");
			}

            speed.Stop("ProperySheet Loading");

		}*/

		static string sSelectedTabText;

		private void m_tab_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(!bFlagEnable)	return;

			if(m_tab.SelectedTab == null)	return;
			sSelectedTabText = m_tab.SelectedTab.Text;	
		}

		class OBJECT_LIST 
		{
			public object obj;
			public Property_Recv recv;
		}

		ArrayList listObject = new ArrayList();

		public void AddObject(object obj, Property_Recv recv)
		{
			OBJECT_LIST list = new OBJECT_LIST();
			list.obj = obj;
			list.recv = recv;
			listObject.Add(list);
		}

		public void SendPropertyToRecv()
		{
			OBJECT_LIST list;

			for(int i = 0; i < listObject.Count; i++) 
			{
				list = (OBJECT_LIST)listObject[i];

				for(int j = 0; j < listPages.Count; j++) 
				{
					list.recv(list.obj, (Form)listPages[j]);
				}

                ((ObjectType)list.obj).previewOnStudio = null;  // 미리보기를 다시 만들 수 있도록 Layer Preview를 삭제한다.
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ClassStudioEditUndo.UndoSave_Selected(formParent, "Object Properties");
			SendPropertyToRecv();
			formParent.SetChangeFlag();
			formParent.Invalidate();
            
            // 편집용 제목이 바뀔 수 있으므로 레이어를 Invalidate한다.
            Layer.FormLayer.InvalidateDisplay();

			SharedStudio.formMain.Select();	// 이게 없으면 Focus가 다른 프로그램으로 갈때가 많다.

			Close();
		}

		private void buttonApply_Click(object sender, System.EventArgs e)
		{
			ClassStudioEditUndo.UndoSave_Selected(formParent, "Object Properties");
			SendPropertyToRecv();
			formParent.SetChangeFlag();
			formParent.Invalidate();

            // 편집용 제목이 바뀔 수 있으므로 레이어를 Invalidate 한다.
            Layer.FormLayer.InvalidateDisplay();

            SharedStudio.formMain.Select();
		}

        //20241010 PSU 요소속성 창 위치 수정.
        private static Screen lastScreen = null;  // 마지막으로 사용된 스크린
        private static Point lastPosition = new Point(50, 50);  // 기본 위치

		private void PropertySheetPublic_Closed(object sender, System.EventArgs e)
		{
			ClassEditProperty.propertySheet = null;

			nTempStartX = this.Left;
			nTempStartY = this.Top;

            lastPosition = this.Location;
            lastScreen = Screen.FromControl(this);  // 현재 스크린 저장
		}

		static int nTempStartX = 50, nTempStartY = 50;

		private void PropertySheetPublic_Load(object sender, System.EventArgs e)
		{
            if (SharedStudio.formMain != null)
            {
                Screen mainScreen = Screen.FromControl(SharedStudio.formMain);

                // 마지막으로 사용된 스크린이 없거나 현재 사용 불가능한 경우 메인 스크린 사용
                if (lastScreen == null || !Screen.AllScreens.Any(s => s.DeviceName == lastScreen.DeviceName))
                {
                    lastScreen = mainScreen;
                }

                // 마지막 위치를 스크린 상대 좌표로 변환
                Point relativePosition = new Point(
                    lastPosition.X - lastScreen.Bounds.Left,
                    lastPosition.Y - lastScreen.Bounds.Top
                );

                // 상대 좌표를 현재 메인 스크린의 좌표로 변환
                Point adjustedPosition = new Point(
                    mainScreen.Bounds.Left + relativePosition.X,
                    mainScreen.Bounds.Top + relativePosition.Y
                );

                // 위치가 현재 스크린 내에 있는지 확인하고 조정
                Rectangle screenBounds = mainScreen.WorkingArea;  // 작업 영역 사용 (작업 표시줄 제외)
                adjustedPosition.X = Math.Max(screenBounds.Left, Math.Min(adjustedPosition.X, screenBounds.Right - this.Width));
                adjustedPosition.Y = Math.Max(screenBounds.Top, Math.Min(adjustedPosition.Y, screenBounds.Bottom - this.Height));

                // 폼의 위치를 설정
                this.StartPosition = FormStartPosition.Manual;
                this.Location = adjustedPosition;
            }
		}

		private void PropertySheetPublic_Move(object sender, System.EventArgs e)
		{
			
		}

        public void ObjectPosSizeChanged()
        {
            OBJECT_LIST list;
            Form form;
            bool first_flag = false;

            for (int i = 0; i < listObject.Count; i++)
            {
                list = (OBJECT_LIST)listObject[i];

                for (int j = 0; j < listPages.Count; j++)
                {
                    form = (Form)listPages[j];

                    if (form.Name == "PropertyPageClassName")
                    {
                        PropertyPageClassName prop = (PropertyPageClassName)form;
                        if (first_flag == false)
                        {
                            prop.InitPositionSize();
                            first_flag = true;
                        }

                        prop.Set((ObjectExpand)list.obj);
                    }
                }
            }
        }

        public void EnableDisableTabPage(string tag, bool flag)
        {
            Form save;
            for (int i = 0; i < listPages.Count; i++)
            {
                save = (Form)listPages[i];
                if ((string)save.Tag == tag)
                {
                    //Control control = save.Parent;
                    save.Visible = flag;
                    return;	// 이미 등록되어 있다.
                }
            }
        }
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools.OldDefine;
//using GraphicModule;
using NetTools;

namespace ReportModule
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
            this.SuspendLayout();
            // 
            // m_tab
            // 
            resources.ApplyResources(this.m_tab, "m_tab");
            this.m_tab.Multiline = true;
            this.m_tab.Name = "m_tab";
            this.m_tab.SelectedIndex = 0;
            this.m_tab.SelectedIndexChanged += new System.EventHandler(this.m_tab_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // PropertySheetPublic
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.m_tab);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertySheetPublic";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PropertySheetPublic_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
		}

		Size sizeCalc;

		ArrayList listPages = new ArrayList();

		

		void AddOnlyPageTitle(Form form)
		{
			listPages.Add(form);
			TabPage page = new TabPage(form.Text);
			m_tab.TabPages.Add(page);			
		}

		public void AddPage(Form form)
		{
			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Text == save.Text)	return;	// 이미 등록되어 있다.
			}

			AddOnlyPageTitle(form);
		}

		/*
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

		public void AddPageClassName(object obj, bool multi_select)
		{
			PropertyPageClassName form = new PropertyPageClassName(); 

			if(multi_select)	return;	

			string class_name = ((ObjectExpand)obj).GetClassName();

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


			form.textBoxClassName.Text = class_name;
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

		public void AddPageBackColor(object obj)
		{
			PropertyPageColor form = new PropertyPageColor("BackColor", Tools.IsLangKorean() ? "배경색" : "BackColor");

			Color color = ((ObjectExpand)obj).BackColor;

			AddPageColor(form, color);
		}

		public void AddPageTextColor(object obj)
		{
			PropertyPageColor form = new PropertyPageColor("TextColor", Tools.IsLangKorean() ? "글자색" : "TextColor");	

			Color color = ((ObjectExpand)obj).TextColor;

			AddPageColor(form, color);
		}

		public void AddPageLineColor(object obj)
		{
			PropertyPageColorLine form = new PropertyPageColorLine();

			Color color = ((ObjectExpand)obj).LineColor;
			int thick = ((ObjectExpand)obj).LineThick;
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

		public void AddPageFillColor(object obj)
		{
			PropertyPageColorFill form = new PropertyPageColorFill(); 

			Color color = ((ObjectExpand)obj).FillColor;
			int option = ((ObjectExpand)obj).nFillOption;

			Form save;
			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Name == save.Name)
				{
					((PropertyPageColorFill)save).SetSelectedColor(color, false);
					((PropertyPageColorFill)save).SetSelectedOption(option, false);

					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			form.SetSelectedColor(color, true);
			form.SetSelectedOption(option, true);
		}
		*/

		void AddPageTag(PropertyPageTag form, string tag)
		{
			Form save;

			for(int i = 0; i < listPages.Count; i++) 
			{
				save = (Form)listPages[i];
				if(form.Name == save.Name)
				{
					((PropertyPageTag)save).SetTag(tag, false);
					return;	// 이미 등록되어 있다.
				}
			}

			AddOnlyPageTitle(form);

			form.SetTag(tag, true);
		}

		public void AddPageTagAI(PropertyPageTag form, string tag)
		{
			form.bUseAI = true;
			AddPageTag(form, tag);
		}

		public void AddPageTagDI(PropertyPageTag form, string tag)
		{
			form.bUseDI = true;
			AddPageTag(form, tag);
		}

		public void AddPageTagST(PropertyPageTag form, string tag)
		{
			form.bUseST = true;
			AddPageTag(form, tag);
		}

		/*

		

		public void AddPageMouseResponseDigital(object obj, bool multi_select)
		{
			if(multi_select)	return;

			PropertyPageMouseResponseDigital form = new PropertyPageMouseResponseDigital(); 

			AddOnlyPageTitle(form);

			form.SetMouseResponse(((ObjectTag)obj).MouseResponse);
		}

		public void AddPageMouseResponseAnalog(object obj, bool multi_select)
		{
			if(multi_select)	return;

			PropertyPageMouseResponseAnalog form = new PropertyPageMouseResponseAnalog(); 

			AddOnlyPageTitle(form);

			form.SetMouseResponse(((ObjectTag)obj).MouseResponse);
		}

		// 콘트롤 오브젝트에서 사용한다.
		public void AddPageListData(object obj, ArrayList array, bool multi_select)
		{
			if(multi_select)	return;

			PropertyPageListData form = new PropertyPageListData();

			AddOnlyPageTitle(form);

			form.ListData = array;

			//form.SetTag(tag, true);
		}
		*/

		//bool bFlagEnable = false;

		//FormReportChild formParent = null;

		/*
		public void Prepare(FormReportChild form)
		{
			formParent = form;
			//bFlagEnable = false;
			m_tab.TabPages.Clear();
			listObject.Clear();
			listPages.Clear();
		}
		

		bool bShowFlag = false;
		*/
		

		/*
		public void Run()
		{
			for(int i = 0; i < m_tab.TabPages.Count; i++) 
			{
				if(sSelectedTabText == m_tab.TabPages[i].Text) 
				{
					m_tab.SelectedIndex = i;
					break;
				}
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

			this.TopMost = true;

			//if(!this.Visible)	this.Visible = true;
			if(!bShowFlag) 
			{
				this.Show();
				bShowFlag = true;
			}
			else 
			{
				this.Visible = true;
			}

			bFlagEnable = true;
		}
		*/

		static string sSelectedTabText;

		private void m_tab_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//if(!bFlagEnable)	return;

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
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			//ClassStudioEditUndo.UndoSave_Selected(formParent, "Object Properties");
			//SendPropertyToRecv();
			//formParent.SetChangeFlag();
			//formParent.Invalidate();
			
			//this.Visible = false;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonApply_Click(object sender, System.EventArgs e)
		{
			//ClassStudioEditUndo.UndoSave_Selected(formParent, "Object Properties");
			//SendPropertyToRecv();
			//formParent.SetChangeFlag();
			//formParent.Invalidate();
		}

		private void PropertySheetPublic_Load(object sender, System.EventArgs e)
		{
			for(int i = 0; i < m_tab.TabPages.Count; i++) 
			{
				if(sSelectedTabText == m_tab.TabPages[i].Text) 
				{
					m_tab.SelectedIndex = i;
					break;
				}
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

					//this.buttonApply.Left = this.buttonOK.Left-this.buttonApply.Size.Width-8;
					//this.buttonApply.Top = this.ClientRectangle.Bottom-this.buttonApply.Size.Height-8;
				}

				form.FormBorderStyle = FormBorderStyle.None;
				form.TopLevel = false;
				form.Parent = m_tab.TabPages[i];
				m_tab.TabPages[i].Controls.Add(form);
				form.Show();
			}

			/*
			this.TopMost = true;

			//if(!this.Visible)	this.Visible = true;
			if(!bShowFlag) 
			{
				this.Show();
				bShowFlag = true;
			}
			else 
			{
				this.Visible = true;
			}

			bFlagEnable = true;
			*/
		}
	}
}

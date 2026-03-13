using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ReportModule
{
	/// <summary>
	/// Summary description for FormDialogInsertFunction.
	/// </summary>
	public class FormDialogInsertFunction : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private Label labelPreviewTitle;
        private TextBox textBoxPreview;
        public Func<string, string> FunctionPreviewResolver;

		public FormDialogInsertFunction()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDialogInsertFunction));
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelPreviewTitle = new System.Windows.Forms.Label();
            this.textBoxPreview = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_list
            // 
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            this.m_list.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
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
            // 
            // labelPreviewTitle
            // 
            resources.ApplyResources(this.labelPreviewTitle, "labelPreviewTitle");
            this.labelPreviewTitle.Name = "labelPreviewTitle";
            // 
            // textBoxPreview
            // 
            resources.ApplyResources(this.textBoxPreview, "textBoxPreview");
            this.textBoxPreview.Name = "textBoxPreview";
            this.textBoxPreview.ReadOnly = true;
            // 
            // FormDialogInsertFunction
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxPreview);
            this.Controls.Add(this.labelPreviewTitle);
            this.Controls.Add(this.m_list);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDialogInsertFunction";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormDialogInsertFunction_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ColumnHeader columnHeader1;

		//static int MAX_FUNCTION = 16;
		public string sFunction;

		static string[] sSampleFunction =  
		{ 
			"@ave(A0:A3)",
			"@max(A0:A3)",
			"@min(A0:A3)",
			"@sum(A0:A3)",
			"@LineAve(A:C)",
			"@LineMax(A:C)",
			"@LineMin(A:C)",
			"@LineSum(A:C)",
			"@LineSub(A:C)",
			"@AbsAve(A0:A3)",
			"@AbsMax(A0:A3)",
			"@AbsMin(A0:A3)",
			"@abs(A0)",
			"@MinCellText(A, B0:B3)",
			"@MaxCellText(A, B0:B3)",
			"@GetLineCount(B1:B1)",
		};

		private void FormDialogInsertFunction_Load(object sender, System.EventArgs e)
		{
			ListViewItem item;
			for(int i = 0; i < sSampleFunction.Length; i++) 
			{
				item = new ListViewItem(sSampleFunction[i]);
				m_list.Items.Add(item);
			}

            if (m_list.Items.Count > 0)
            {
                m_list.Items[0].Selected = true;
            }
            UpdatePreview();
		}

		void OK()
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean()) 
					MessageBox.Show("삽입할 항목을 선택하세요.", "선택오류");
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请选择要插入的项。", "选择错误");
				}
				else
					MessageBox.Show("Select the item to insert.", "Selection error"); 
				return;
			}

			sFunction = m_list.SelectedItems[0].SubItems[0].Text;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			OK();
		}

		private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            UpdatePreview();
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			OK();
		}

        void UpdatePreview()
        {
            if (textBoxPreview == null)
            {
                return;
            }

            if (m_list.SelectedItems.Count == 0)
            {
                textBoxPreview.Text = "";
                return;
            }

            string sample = m_list.SelectedItems[0].SubItems[0].Text;
            if (FunctionPreviewResolver != null)
            {
                textBoxPreview.Text = FunctionPreviewResolver(sample);
            }
            else
            {
                textBoxPreview.Text = sample;
            }
        }
	}
}

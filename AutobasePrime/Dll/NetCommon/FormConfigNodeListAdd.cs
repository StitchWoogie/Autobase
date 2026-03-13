using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NetCommon
{
	/// <summary>
	/// Summary description for FormConfigNodeListAdd.
	/// </summary>
	public class FormConfigNodeListAdd : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox textBoxTitle;
		public System.Windows.Forms.TextBox textBoxIP1;
		public System.Windows.Forms.TextBox textBoxIP2;
		public System.Windows.Forms.CheckBox checkBoxPlcScan;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigNodeListAdd()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigNodeListAdd));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.textBoxIP1 = new System.Windows.Forms.TextBox();
            this.textBoxIP2 = new System.Windows.Forms.TextBox();
            this.checkBoxPlcScan = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
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
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.AccessibleDescription = null;
            this.textBoxTitle.AccessibleName = null;
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.BackgroundImage = null;
            this.textBoxTitle.Font = null;
            this.textBoxTitle.Name = "textBoxTitle";
            // 
            // textBoxIP1
            // 
            this.textBoxIP1.AccessibleDescription = null;
            this.textBoxIP1.AccessibleName = null;
            resources.ApplyResources(this.textBoxIP1, "textBoxIP1");
            this.textBoxIP1.BackgroundImage = null;
            this.textBoxIP1.Font = null;
            this.textBoxIP1.Name = "textBoxIP1";
            // 
            // textBoxIP2
            // 
            this.textBoxIP2.AccessibleDescription = null;
            this.textBoxIP2.AccessibleName = null;
            resources.ApplyResources(this.textBoxIP2, "textBoxIP2");
            this.textBoxIP2.BackgroundImage = null;
            this.textBoxIP2.Font = null;
            this.textBoxIP2.Name = "textBoxIP2";
            // 
            // checkBoxPlcScan
            // 
            this.checkBoxPlcScan.AccessibleDescription = null;
            this.checkBoxPlcScan.AccessibleName = null;
            resources.ApplyResources(this.checkBoxPlcScan, "checkBoxPlcScan");
            this.checkBoxPlcScan.BackgroundImage = null;
            this.checkBoxPlcScan.Font = null;
            this.checkBoxPlcScan.Name = "checkBoxPlcScan";
            // 
            // FormConfigNodeListAdd
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.checkBoxPlcScan);
            this.Controls.Add(this.textBoxIP2);
            this.Controls.Add(this.textBoxIP1);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigNodeListAdd";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if(this.textBoxTitle.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("제목을 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese()) 
					MessageBox.Show("请输入标题。", "输入错误");
				else
					MessageBox.Show("You must input the title.", "Title Error");
				return;
			}

			if(this.textBoxIP1.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("기본 IP를 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入基本IP", "输入错误");
				else
					MessageBox.Show("You must input the primary IP.", "IP Error");
				return;
			}

			if(this.textBoxIP2.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("예비 IP를 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入预备IP", "输入错误");
				else
					MessageBox.Show("You must input the secondary IP.", "IP Error");
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for edit_DBMultiQueryInputDlg.
	/// </summary>
	public class edit_DBMultiQueryInputDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.ComboBox comboBox_Indetify_Char;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox7;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public System.Windows.Forms.TextBox textBox_QueryInput;
		public System.Windows.Forms.CheckBox checkBox_InsertLine;
		public bool bOkFlag;

		public edit_DBMultiQueryInputDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit_DBMultiQueryInputDlg));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox_Indetify_Char = new System.Windows.Forms.ComboBox();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.textBox_QueryInput = new System.Windows.Forms.TextBox();
            this.checkBox_InsertLine = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_Indetify_Char);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // comboBox_Indetify_Char
            // 
            resources.ApplyResources(this.comboBox_Indetify_Char, "comboBox_Indetify_Char");
            this.comboBox_Indetify_Char.Name = "comboBox_Indetify_Char";
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.textBox_QueryInput);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // textBox_QueryInput
            // 
            resources.ApplyResources(this.textBox_QueryInput, "textBox_QueryInput");
            this.textBox_QueryInput.Name = "textBox_QueryInput";
            // 
            // checkBox_InsertLine
            // 
            resources.ApplyResources(this.checkBox_InsertLine, "checkBox_InsertLine");
            this.checkBox_InsertLine.Name = "checkBox_InsertLine";
            // 
            // edit_DBMultiQueryInputDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.checkBox_InsertLine);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "edit_DBMultiQueryInputDlg";
            this.groupBox2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(comboBox_Indetify_Char.Text.Length <= 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("명렁인식 문장을 설정해야 합니다.", "설정오류");
				else
					MessageBox.Show("Please Select Command Name.", "Select Error");
				return;
			}
			if(this.textBox_QueryInput.Text.Length == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("Query 문장을 입력하여야 합니다.", "입력오류");
				else
					MessageBox.Show("Please Input Query String.", "Input Error");
				return;
			}
			bOkFlag = true;
			Close();
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormSelectMilliDataOption.
	/// </summary>
	public class FormSelectMilliDataOption : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonViewType0;
		private System.Windows.Forms.RadioButton radioButtonViewType1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonBackColor1;
		private System.Windows.Forms.RadioButton radioButtonBackColor0;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public int m_nTimeType;
        public CheckBox checkBoxDisplayTagDescription;
		public int m_nBackColor;

		public FormSelectMilliDataOption()
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
		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectMilliDataOption));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonViewType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonViewType0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonBackColor1 = new System.Windows.Forms.RadioButton();
            this.radioButtonBackColor0 = new System.Windows.Forms.RadioButton();
            this.checkBoxDisplayTagDescription = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonViewType1);
            this.groupBox1.Controls.Add(this.radioButtonViewType0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonViewType1
            // 
            this.radioButtonViewType1.AccessibleDescription = null;
            this.radioButtonViewType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonViewType1, "radioButtonViewType1");
            this.radioButtonViewType1.BackgroundImage = null;
            this.radioButtonViewType1.Font = null;
            this.radioButtonViewType1.Name = "radioButtonViewType1";
            // 
            // radioButtonViewType0
            // 
            this.radioButtonViewType0.AccessibleDescription = null;
            this.radioButtonViewType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonViewType0, "radioButtonViewType0");
            this.radioButtonViewType0.BackgroundImage = null;
            this.radioButtonViewType0.Font = null;
            this.radioButtonViewType0.Name = "radioButtonViewType0";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.radioButtonBackColor1);
            this.groupBox2.Controls.Add(this.radioButtonBackColor0);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonBackColor1
            // 
            this.radioButtonBackColor1.AccessibleDescription = null;
            this.radioButtonBackColor1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBackColor1, "radioButtonBackColor1");
            this.radioButtonBackColor1.BackgroundImage = null;
            this.radioButtonBackColor1.Font = null;
            this.radioButtonBackColor1.Name = "radioButtonBackColor1";
            // 
            // radioButtonBackColor0
            // 
            this.radioButtonBackColor0.AccessibleDescription = null;
            this.radioButtonBackColor0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBackColor0, "radioButtonBackColor0");
            this.radioButtonBackColor0.BackgroundImage = null;
            this.radioButtonBackColor0.Font = null;
            this.radioButtonBackColor0.Name = "radioButtonBackColor0";
            // 
            // checkBoxDisplayTagDescription
            // 
            this.checkBoxDisplayTagDescription.AccessibleDescription = null;
            this.checkBoxDisplayTagDescription.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDisplayTagDescription, "checkBoxDisplayTagDescription");
            this.checkBoxDisplayTagDescription.BackgroundImage = null;
            this.checkBoxDisplayTagDescription.Font = null;
            this.checkBoxDisplayTagDescription.Name = "checkBoxDisplayTagDescription";
            this.checkBoxDisplayTagDescription.UseVisualStyleBackColor = true;
            // 
            // FormSelectMilliDataOption
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.checkBoxDisplayTagDescription);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectMilliDataOption";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormSelectMilliDataOption_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FormSelectMilliDataOption_Load(object sender, System.EventArgs e)
		{
			this.radioButtonViewType0.Checked = (m_nTimeType == 0);
			this.radioButtonViewType1.Checked = (m_nTimeType == 1);

			this.radioButtonBackColor0.Checked = (m_nBackColor == 0);
			this.radioButtonBackColor1.Checked = (m_nBackColor == 1);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if(this.radioButtonViewType0.Checked)	m_nTimeType = 0;
			else									m_nTimeType = 1;

			if(this.radioButtonBackColor0.Checked)	m_nBackColor = 0;
			else									m_nBackColor = 1;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

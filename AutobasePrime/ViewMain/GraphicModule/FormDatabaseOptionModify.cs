using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for FormDatabaseOptionModify.
	/// </summary>
	public class FormDatabaseOptionModify : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.TextBox textBoxColumn;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.TextBox textBoxDisplay;
		public System.Windows.Forms.CheckBox checkBoxActive;
		private System.Windows.Forms.GroupBox groupBox3;
		public System.Windows.Forms.RadioButton radioButtonAlign0;
		public System.Windows.Forms.RadioButton radioButtonAlign1;
		public System.Windows.Forms.RadioButton radioButtonAlign2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormDatabaseOptionModify()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDatabaseOptionModify));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxColumn = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxDisplay = new System.Windows.Forms.TextBox();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonAlign2 = new System.Windows.Forms.RadioButton();
            this.radioButtonAlign1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAlign0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            this.groupBox1.Controls.Add(this.textBoxColumn);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxColumn
            // 
            this.textBoxColumn.AccessibleDescription = null;
            this.textBoxColumn.AccessibleName = null;
            resources.ApplyResources(this.textBoxColumn, "textBoxColumn");
            this.textBoxColumn.BackgroundImage = null;
            this.textBoxColumn.Font = null;
            this.textBoxColumn.Name = "textBoxColumn";
            this.textBoxColumn.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.textBoxDisplay);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.AccessibleDescription = null;
            this.textBoxDisplay.AccessibleName = null;
            resources.ApplyResources(this.textBoxDisplay, "textBoxDisplay");
            this.textBoxDisplay.BackgroundImage = null;
            this.textBoxDisplay.Font = null;
            this.textBoxDisplay.Name = "textBoxDisplay";
            // 
            // checkBoxActive
            // 
            this.checkBoxActive.AccessibleDescription = null;
            this.checkBoxActive.AccessibleName = null;
            resources.ApplyResources(this.checkBoxActive, "checkBoxActive");
            this.checkBoxActive.BackgroundImage = null;
            this.checkBoxActive.Font = null;
            this.checkBoxActive.Name = "checkBoxActive";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.radioButtonAlign2);
            this.groupBox3.Controls.Add(this.radioButtonAlign1);
            this.groupBox3.Controls.Add(this.radioButtonAlign0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonAlign2
            // 
            this.radioButtonAlign2.AccessibleDescription = null;
            this.radioButtonAlign2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAlign2, "radioButtonAlign2");
            this.radioButtonAlign2.BackgroundImage = null;
            this.radioButtonAlign2.Font = null;
            this.radioButtonAlign2.Name = "radioButtonAlign2";
            // 
            // radioButtonAlign1
            // 
            this.radioButtonAlign1.AccessibleDescription = null;
            this.radioButtonAlign1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAlign1, "radioButtonAlign1");
            this.radioButtonAlign1.BackgroundImage = null;
            this.radioButtonAlign1.Font = null;
            this.radioButtonAlign1.Name = "radioButtonAlign1";
            // 
            // radioButtonAlign0
            // 
            this.radioButtonAlign0.AccessibleDescription = null;
            this.radioButtonAlign0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAlign0, "radioButtonAlign0");
            this.radioButtonAlign0.BackgroundImage = null;
            this.radioButtonAlign0.Font = null;
            this.radioButtonAlign0.Name = "radioButtonAlign0";
            // 
            // FormDatabaseOptionModify
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.checkBoxActive);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDatabaseOptionModify";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace WatchDog
{
	/// <summary>
	/// Summary description for FormModify.
	/// </summary>
	public class FormModify : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		public System.Windows.Forms.CheckBox checkBoxActive;
		public System.Windows.Forms.NumericUpDown numericUpDownTimer;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormModify()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			this.numericUpDownTimer.Minimum = 10;
			this.numericUpDownTimer.Maximum = 3600;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModify));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.numericUpDownTimer = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimer)).BeginInit();
            this.groupBox1.SuspendLayout();
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
            // checkBoxActive
            // 
            this.checkBoxActive.AccessibleDescription = null;
            this.checkBoxActive.AccessibleName = null;
            resources.ApplyResources(this.checkBoxActive, "checkBoxActive");
            this.checkBoxActive.BackgroundImage = null;
            this.checkBoxActive.Font = null;
            this.checkBoxActive.Name = "checkBoxActive";
            // 
            // numericUpDownTimer
            // 
            this.numericUpDownTimer.AccessibleDescription = null;
            this.numericUpDownTimer.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTimer, "numericUpDownTimer");
            this.numericUpDownTimer.Font = null;
            this.numericUpDownTimer.Name = "numericUpDownTimer";
            this.numericUpDownTimer.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownTimer);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // FormModify
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxActive);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModify";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormModify_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimer)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
		{
		
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void FormModify_Load(object sender, System.EventArgs e)
		{
			
		}
	}
}

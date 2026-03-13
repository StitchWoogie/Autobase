using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Studio
{
	/// <summary>
	/// Summary description for FormExpandOptionMouseZone.
	/// </summary>
	public class FormExpandOptionMouseZone : System.Windows.Forms.Form
	{
		public System.Windows.Forms.CheckBox checkBoxLock;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.NumericUpDown numericUpDownX1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		public System.Windows.Forms.NumericUpDown numericUpDownY1;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.NumericUpDown numericUpDownX2;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.NumericUpDown numericUpDownY2;
		private System.Windows.Forms.Label label4;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormExpandOptionMouseZone()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExpandOptionMouseZone));
            this.checkBoxLock = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownX1 = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.numericUpDownY1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownX2 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownY2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownY1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownY2)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxLock
            // 
            this.checkBoxLock.AccessibleDescription = null;
            this.checkBoxLock.AccessibleName = null;
            resources.ApplyResources(this.checkBoxLock, "checkBoxLock");
            this.checkBoxLock.BackgroundImage = null;
            this.checkBoxLock.Checked = true;
            this.checkBoxLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLock.Font = null;
            this.checkBoxLock.Name = "checkBoxLock";
            this.checkBoxLock.CheckedChanged += new System.EventHandler(this.checkBoxLock_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // numericUpDownX1
            // 
            this.numericUpDownX1.AccessibleDescription = null;
            this.numericUpDownX1.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownX1, "numericUpDownX1");
            this.numericUpDownX1.Font = null;
            this.numericUpDownX1.Name = "numericUpDownX1";
            this.numericUpDownX1.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownX1.ValueChanged += new System.EventHandler(this.numericUpDownX1_ValueChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
            // numericUpDownY1
            // 
            this.numericUpDownY1.AccessibleDescription = null;
            this.numericUpDownY1.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownY1, "numericUpDownY1");
            this.numericUpDownY1.Font = null;
            this.numericUpDownY1.Name = "numericUpDownY1";
            this.numericUpDownY1.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // numericUpDownX2
            // 
            this.numericUpDownX2.AccessibleDescription = null;
            this.numericUpDownX2.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownX2, "numericUpDownX2");
            this.numericUpDownX2.Font = null;
            this.numericUpDownX2.Name = "numericUpDownX2";
            this.numericUpDownX2.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownY2
            // 
            this.numericUpDownY2.AccessibleDescription = null;
            this.numericUpDownY2.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownY2, "numericUpDownY2");
            this.numericUpDownY2.Font = null;
            this.numericUpDownY2.Name = "numericUpDownY2";
            this.numericUpDownY2.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // FormExpandOptionMouseZone
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.numericUpDownY2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownX2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownY1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.numericUpDownX1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxLock);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExpandOptionMouseZone";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormExpandOptionMouseZone_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownY1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownY2)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		void EnableDisable()
		{
			bool check = this.checkBoxLock.Checked; 
			
			this.numericUpDownX2.Enabled = !check;
			this.numericUpDownY1.Enabled = !check;
			this.numericUpDownY2.Enabled = !check;
		}

		private void FormExpandOptionMouseZone_Load(object sender, System.EventArgs e)
		{
			EnableDisable();			
		}

		private void checkBoxLock_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void numericUpDownX1_ValueChanged(object sender, System.EventArgs e)
		{
			if(!this.checkBoxLock.Checked)	return;

			decimal val = this.numericUpDownX1.Value;
			this.numericUpDownX2.Value = val;
			this.numericUpDownY1.Value = val;
			this.numericUpDownY2.Value = val;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void label1_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}

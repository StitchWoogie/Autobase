using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace AniEditLib
{
	/// <summary>
	/// Summary description for FormConfigBitmapEditor.
	/// </summary>
	public class FormConfigBitmapEditor : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBoxFilename;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonFilename;
		private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigBitmapEditor()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigBitmapEditor));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonFilename = new System.Windows.Forms.Button();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
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
            // buttonFilename
            // 
            this.buttonFilename.AccessibleDescription = null;
            this.buttonFilename.AccessibleName = null;
            resources.ApplyResources(this.buttonFilename, "buttonFilename");
            this.buttonFilename.BackgroundImage = null;
            this.buttonFilename.Font = null;
            this.buttonFilename.Name = "buttonFilename";
            this.buttonFilename.Click += new System.EventHandler(this.buttonFilename_Click);
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.AccessibleDescription = null;
            this.textBoxFilename.AccessibleName = null;
            resources.ApplyResources(this.textBoxFilename, "textBoxFilename");
            this.textBoxFilename.BackgroundImage = null;
            this.textBoxFilename.Font = null;
            this.textBoxFilename.Name = "textBoxFilename";
            // 
            // tabControl1
            // 
            this.tabControl1.AccessibleDescription = null;
            this.tabControl1.AccessibleName = null;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.BackgroundImage = null;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Font = null;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.AccessibleDescription = null;
            this.tabPage1.AccessibleName = null;
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.BackgroundImage = null;
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.buttonFilename);
            this.tabPage1.Controls.Add(this.textBoxFilename);
            this.tabPage1.Font = null;
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // FormConfigBitmapEditor
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigBitmapEditor";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigBitmapEditor_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigBitmapEditor_Load(object sender, System.EventArgs e)
		{
			textBoxFilename.Text = Config.sBitmapEditor;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            Config.sBitmapEditor = textBoxFilename.Text;
            Config.Save();
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonFilename_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Bitmap Editor (*.exe)|*.exe";

			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				textBoxFilename.Text = dialog.FileName;
			}
		}


	}
}

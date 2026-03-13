using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Studio
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
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonScriptEditor;
		private System.Windows.Forms.TextBox textBoxScriptEditor;
		private System.Windows.Forms.Label label3;
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonScriptEditor = new System.Windows.Forms.Button();
            this.textBoxScriptEditor = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tabPage2);
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
            // tabPage2
            // 
            this.tabPage2.AccessibleDescription = null;
            this.tabPage2.AccessibleName = null;
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.BackgroundImage = null;
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.buttonScriptEditor);
            this.tabPage2.Controls.Add(this.textBoxScriptEditor);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Font = null;
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // buttonScriptEditor
            // 
            this.buttonScriptEditor.AccessibleDescription = null;
            this.buttonScriptEditor.AccessibleName = null;
            resources.ApplyResources(this.buttonScriptEditor, "buttonScriptEditor");
            this.buttonScriptEditor.BackgroundImage = null;
            this.buttonScriptEditor.Font = null;
            this.buttonScriptEditor.Name = "buttonScriptEditor";
            this.buttonScriptEditor.Click += new System.EventHandler(this.buttonScriptEditor_Click);
            // 
            // textBoxScriptEditor
            // 
            this.textBoxScriptEditor.AccessibleDescription = null;
            this.textBoxScriptEditor.AccessibleName = null;
            resources.ApplyResources(this.textBoxScriptEditor, "textBoxScriptEditor");
            this.textBoxScriptEditor.BackgroundImage = null;
            this.textBoxScriptEditor.Font = null;
            this.textBoxScriptEditor.Name = "textBoxScriptEditor";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
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
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public static string ExternalScriptEditor
		{
			get 
			{
				return AutoLibLocal.TotalConfig.LoadRegAutoBaseConfig("Studio", "Config", "External Script Editor", "NotePad.exe");
			}
			set 
			{
				AutoLibLocal.TotalConfig.SaveRegAutoBaseConfig("Studio", "Config", "External Script Editor", value);
			}
		}

		private void FormConfigBitmapEditor_Load(object sender, System.EventArgs e)
		{
			textBoxFilename.Text = AutoLib.ConfigStudio.sEditFileName;
			textBoxScriptEditor.Text = ExternalScriptEditor;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            AutoLib.ConfigStudio.sEditFileName = textBoxFilename.Text;
			ExternalScriptEditor = textBoxScriptEditor.Text;
            AutoLib.ConfigStudio.Save();
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonFilename_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Bitmap Editor (*.exe)|*.exe";

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				textBoxFilename.Text = dialog.FileName;
			}
		}

		private void buttonScriptEditor_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Script Editor (*.exe)|*.exe";

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				textBoxScriptEditor.Text = dialog.FileName;
			}
		}
	}
}

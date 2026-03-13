using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using System.IO;
using AutoLibLocal;
using Studio.Script;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageButtonModule3D.
	/// </summary>
	public class PropertyPageObjectButtonProgram : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.TextBox textBoxText;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonEditScript; 
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.RichTextBox richTextBoxScript;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButtonScriptType0;
		private System.Windows.Forms.RadioButton radioButtonScriptType1;
		public System.Windows.Forms.TextBox textBoxFilename;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button buttonEditFile;
		public ScriptClass scriptTemp;
		

		public PropertyPageObjectButtonProgram()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectButtonProgram));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBoxScript = new System.Windows.Forms.RichTextBox();
            this.buttonEditScript = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonScriptType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonScriptType0 = new System.Windows.Forms.RadioButton();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonEditFile = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxText);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxText
            // 
            this.textBoxText.AccessibleDescription = null;
            this.textBoxText.AccessibleName = null;
            resources.ApplyResources(this.textBoxText, "textBoxText");
            this.textBoxText.BackgroundImage = null;
            this.textBoxText.Font = null;
            this.textBoxText.Name = "textBoxText";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.richTextBoxScript);
            this.groupBox2.Controls.Add(this.buttonEditScript);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // richTextBoxScript
            // 
            this.richTextBoxScript.AccessibleDescription = null;
            this.richTextBoxScript.AccessibleName = null;
            resources.ApplyResources(this.richTextBoxScript, "richTextBoxScript");
            this.richTextBoxScript.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxScript.BackgroundImage = null;
            this.richTextBoxScript.Font = null;
            this.richTextBoxScript.Name = "richTextBoxScript";
            this.richTextBoxScript.ReadOnly = true;
            // 
            // buttonEditScript
            // 
            this.buttonEditScript.AccessibleDescription = null;
            this.buttonEditScript.AccessibleName = null;
            resources.ApplyResources(this.buttonEditScript, "buttonEditScript");
            this.buttonEditScript.BackgroundImage = null;
            this.buttonEditScript.Font = null;
            this.buttonEditScript.Name = "buttonEditScript";
            this.buttonEditScript.Click += new System.EventHandler(this.buttonEditScript_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.radioButtonScriptType1);
            this.groupBox3.Controls.Add(this.radioButtonScriptType0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonScriptType1
            // 
            this.radioButtonScriptType1.AccessibleDescription = null;
            this.radioButtonScriptType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonScriptType1, "radioButtonScriptType1");
            this.radioButtonScriptType1.BackgroundImage = null;
            this.radioButtonScriptType1.Font = null;
            this.radioButtonScriptType1.Name = "radioButtonScriptType1";
            this.radioButtonScriptType1.CheckedChanged += new System.EventHandler(this.radioButtonScriptType1_CheckedChanged);
            // 
            // radioButtonScriptType0
            // 
            this.radioButtonScriptType0.AccessibleDescription = null;
            this.radioButtonScriptType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonScriptType0, "radioButtonScriptType0");
            this.radioButtonScriptType0.BackgroundImage = null;
            this.radioButtonScriptType0.Font = null;
            this.radioButtonScriptType0.Name = "radioButtonScriptType0";
            this.radioButtonScriptType0.CheckedChanged += new System.EventHandler(this.radioButtonScriptType0_CheckedChanged);
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
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.textBoxFilename);
            this.groupBox4.Controls.Add(this.buttonEditFile);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // buttonEditFile
            // 
            this.buttonEditFile.AccessibleDescription = null;
            this.buttonEditFile.AccessibleName = null;
            resources.ApplyResources(this.buttonEditFile, "buttonEditFile");
            this.buttonEditFile.BackgroundImage = null;
            this.buttonEditFile.Font = null;
            this.buttonEditFile.Name = "buttonEditFile";
            this.buttonEditFile.Click += new System.EventHandler(this.buttonEditFile_Click);
            // 
            // PropertyPageObjectButtonProgram
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Icon = null;
            this.Name = "PropertyPageObjectButtonProgram";
            this.Load += new System.EventHandler(this.PropertyPageButtonProgram_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageButtonProgram_Load(object sender, System.EventArgs e)
		{
			this.richTextBoxScript.Text = scriptTemp.Script;
		}

		public int ScriptType 
		{
			get 
			{
				int type = 0;
				if(this.radioButtonScriptType0.Checked)			type = 0;
				else if (this.radioButtonScriptType1.Checked)	type = 1;
				else											type = 0;
				return type;
			}
			set 
			{
				this.radioButtonScriptType0.Checked = (value == 0);
				this.radioButtonScriptType1.Checked = (value == 1);
			}
		}

		private void buttonEditScript_Click(object sender, System.EventArgs e)
		{
			FormScriptEditor dialog = new FormScriptEditor();

			if(NetTools.Tools.IsLangKorean()) 
				dialog.SetScript("스크립트 실행 버튼의 스크립트 편집", scriptTemp);
			else if(NetTools.Tools.IsLangJapanese()) 
				dialog.SetScript("スクリプト実行ボタンのスクリプト編集", scriptTemp);  
			else if(NetTools.Tools.IsLangChinese()) 
				dialog.SetScript("编辑[脚本运行按钮]中的脚本", scriptTemp);  
			else
				dialog.SetScript("Button Script", scriptTemp);

			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				scriptTemp = dialog.GetScript();
				this.richTextBoxScript.Text = scriptTemp.Script;
			}
		}

		private void buttonEditFile_Click(object sender, System.EventArgs e)
		{
			if(this.textBoxFilename.Text.Length == 0) 
			{
				MessageBox.Show("Input script filename to Edit", "Error");
				return;
			}
			
			string path = String.Format("{0}\\Control\\Button\\{1}", TotalConfig.sDirWorkProject, this.textBoxFilename.Text);

			if(FormScriptAlways.IsExternalScriptEditorKey()) 
			{
				if(!File.Exists(path)) 
				{
					ScriptClass sample = new ScriptClass();
					sample.SaveFileToMODX(path);
				}
				FormScriptAlways.RunExternalScriptEditor(path);
				return;				
			}

            FormScriptEditor dialog = new FormScriptEditor();
			dialog.SetScript(path);

			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				// 혹시 CTL파일일 경우는 CTLX로 이름을 바꾸어야 한다.
				textBoxFilename.Text = Path.GetFileNameWithoutExtension(textBoxFilename.Text)+".ctlx";
			}
		}

		void EnableDisable()
		{
			bool flag = this.radioButtonScriptType0.Checked;
			this.textBoxFilename.Enabled = flag;
			this.buttonEditFile.Enabled = flag;

			flag = this.radioButtonScriptType1.Checked;
			this.richTextBoxScript.Enabled = flag;
			this.buttonEditScript.Enabled = flag;
		}

		private void radioButtonScriptType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonScriptType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}
	}
}

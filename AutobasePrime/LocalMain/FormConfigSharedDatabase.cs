using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormConfigSharedDatabase.
	/// </summary>
	public class FormConfigSharedDatabase : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxActive;
		private System.Windows.Forms.CheckBox checkBoxUseDuplexServer;
		private System.Windows.Forms.ComboBox comboBoxDsn;
		private System.Windows.Forms.TextBox textBoxTableTagExchange;
		private System.Windows.Forms.TextBox textBoxTableAlarmFile;
		private System.Windows.Forms.CheckBox checkBoxUseSaveAlarmFile;
        private CheckBox checkBoxUseSaveTagExchange;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigSharedDatabase()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigSharedDatabase));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.checkBoxUseDuplexServer = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDsn = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseSaveTagExchange = new System.Windows.Forms.CheckBox();
            this.textBoxTableAlarmFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTableTagExchange = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxUseSaveAlarmFile = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            // checkBoxActive
            // 
            resources.ApplyResources(this.checkBoxActive, "checkBoxActive");
            this.checkBoxActive.Name = "checkBoxActive";
            // 
            // checkBoxUseDuplexServer
            // 
            resources.ApplyResources(this.checkBoxUseDuplexServer, "checkBoxUseDuplexServer");
            this.checkBoxUseDuplexServer.Name = "checkBoxUseDuplexServer";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxDsn
            // 
            this.comboBoxDsn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxDsn, "comboBoxDsn");
            this.comboBoxDsn.Name = "comboBoxDsn";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxUseSaveTagExchange);
            this.groupBox1.Controls.Add(this.textBoxTableAlarmFile);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxTableTagExchange);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.checkBoxUseSaveAlarmFile);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBoxUseSaveTagExchange
            // 
            resources.ApplyResources(this.checkBoxUseSaveTagExchange, "checkBoxUseSaveTagExchange");
            this.checkBoxUseSaveTagExchange.Name = "checkBoxUseSaveTagExchange";
            // 
            // textBoxTableAlarmFile
            // 
            resources.ApplyResources(this.textBoxTableAlarmFile, "textBoxTableAlarmFile");
            this.textBoxTableAlarmFile.Name = "textBoxTableAlarmFile";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxTableTagExchange
            // 
            resources.ApplyResources(this.textBoxTableTagExchange, "textBoxTableTagExchange");
            this.textBoxTableTagExchange.Name = "textBoxTableTagExchange";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // checkBoxUseSaveAlarmFile
            // 
            resources.ApplyResources(this.checkBoxUseSaveAlarmFile, "checkBoxUseSaveAlarmFile");
            this.checkBoxUseSaveAlarmFile.Name = "checkBoxUseSaveAlarmFile";
            // 
            // FormConfigSharedDatabase
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxDsn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxUseDuplexServer);
            this.Controls.Add(this.checkBoxActive);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigSharedDatabase";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigSharedDatabase_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigSharedDatabase_Load(object sender, System.EventArgs e)
		{
			DbTool.FillComboBox(this.comboBoxDsn, DbTool.dsnList);

			this.checkBoxActive.Checked = SharedDatabase.configWebServer.bActive;
			this.checkBoxUseDuplexServer.Checked = SharedDatabase.configWebServer.bUseDuplex;
			this.comboBoxDsn.Text = SharedDatabase.configWebServer.sDsn;
			this.textBoxTableTagExchange.Text = SharedDatabase.configWebServer.sTableTagExchange;
			this.textBoxTableAlarmFile.Text = SharedDatabase.configWebServer.sTableAlarmFile;
			this.checkBoxUseSaveAlarmFile.Checked = SharedDatabase.configWebServer.bTableAlarmFile;
            this.checkBoxUseSaveTagExchange.Checked = SharedDatabase.configWebServer.bTableTagExchange;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_CONFIG_ETC))
                this.buttonOK.Enabled = false;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			SharedDatabase.configWebServer.bActive = this.checkBoxActive.Checked;
			SharedDatabase.configWebServer.bUseDuplex = this.checkBoxUseDuplexServer.Checked;
			SharedDatabase.configWebServer.sDsn = this.comboBoxDsn.Text;
			SharedDatabase.configWebServer.sTableTagExchange = this.textBoxTableTagExchange.Text;
			SharedDatabase.configWebServer.sTableAlarmFile = this.textBoxTableAlarmFile.Text;
			SharedDatabase.configWebServer.bTableAlarmFile = this.checkBoxUseSaveAlarmFile.Checked;
            SharedDatabase.configWebServer.bTableTagExchange = this.checkBoxUseSaveTagExchange.Checked;

			SharedDatabase.SaveWebServerConfig();
			SharedDatabase.Reset();

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SMS.SmsFunc;
using NetTools;

namespace SMS.Display
{
	/// <summary>
	/// Summary description for SmsUserSettingUserSelectDialog.
	/// </summary>
	public class SmsUserSettingUserSelectDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox1; 
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SmsUserSettingUserSelectDialog()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SmsUserSettingUserSelectDialog));
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // SmsUserSettingUserSelectDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SmsUserSettingUserSelectDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.SmsUserSettingUserSelectDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void SmsUserSettingUserSelectDialog_Load(object sender, System.EventArgs e)
		{
			ListViewItem	item;
			smsUserConfig	user;
			for(int i = 0; i < 256; i++) 
			{
				if(SmsBasic.arrUserSetting.Count <= i) break;
				user = (smsUserConfig)SmsBasic.arrUserSetting[i];
				item = new ListViewItem();
				item.Text = user.nUserNo.ToString();				
				if(user.bActive) item.SubItems.Add(user.userName);
				else 
				{
					if(Tools.IsLangKorean()) 
						item.SubItems.Add("¼³Á¤¾ÈµÊ(" + user.userName + ")");
					else if(Tools.IsLangChinese()) 
						item.SubItems.Add("Ð×éÄ(" + user.userName + ")");
					else					 
						item.SubItems.Add("Not used(" + user.userName + ")");					
				}
				this.listView1.Items.Add(item);
			}
		}

		void currentUserDisplay(int nPos)
		{			
			if(SmsBasic.arrUserSetting.Count <= nPos || this.listView1.Items.Count <= nPos) return;
			
			smsUserConfig	user = (smsUserConfig)SmsBasic.arrUserSetting[nPos];
			ListViewItem	item = this.listView1.Items[nPos];
			if(user.bActive) item.SubItems[1].Text = user.userName;
			else 
			{
				if(Tools.IsLangKorean()) 
					item.SubItems[1].Text = "¼³Á¤¾ÈµÊ(" + user.userName + ")";
				else if(Tools.IsLangChinese()) 
					item.SubItems[1].Text = "Ð×éÄ(" + user.userName + ")";
				else					 
					item.SubItems[1].Text = "Not used(" + user.userName + ")";
			}
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(this.listView1.SelectedIndices.Count <= 0) return;
			int		nUserPos = this.listView1.SelectedIndices[0];
			//Close();

			SmsUserSettingDialog dialog = new SmsUserSettingDialog();
			dialog.nUserPos = nUserPos;
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			if(dialog.DialogResult == DialogResult.OK) 
			{
				//if(SmsMainDisplayForm.formThis != null)
				//	SmsMainDisplayForm.formThis.reDrawUserInfo(dialog.nUserPos);
				currentUserDisplay(nUserPos);
			}		
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			button_OK_Click(sender, e);
		}

		private void button_Cancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		
	}
}

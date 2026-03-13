using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using System.IO;
using NetTools;
//using NetTools;

namespace DatabaseConnection
{
	
	/// <summary>
	/// Summary description for FormDatabaseConnection.
	/// </summary>
	public class FormDatabaseConnection : System.Windows.Forms.Form 
	{
		private System.Windows.Forms.Button buttonOK; 
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ListView listViewConnection;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonModify;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;

		ConnectionStringList connectionStringList;

		public FormDatabaseConnection(ConnectionStringList connection_string_list)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			connectionStringList = connection_string_list;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDatabaseConnection));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listViewConnection = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
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
            // listViewConnection
            // 
            this.listViewConnection.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewConnection.FullRowSelect = true;
            this.listViewConnection.HideSelection = false;
            resources.ApplyResources(this.listViewConnection, "listViewConnection");
            this.listViewConnection.MultiSelect = false;
            this.listViewConnection.Name = "listViewConnection";
            this.listViewConnection.UseCompatibleStateImageBehavior = false;
            this.listViewConnection.View = System.Windows.Forms.View.Details;
            this.listViewConnection.SelectedIndexChanged += new System.EventHandler(this.listViewConnection_SelectedIndexChanged);
            this.listViewConnection.DoubleClick += new System.EventHandler(this.listViewConnection_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // buttonAdd
            // 
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonModify
            // 
            resources.ApplyResources(this.buttonModify, "buttonModify");
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // FormDatabaseConnection
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonModify);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listViewConnection);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDatabaseConnection";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormDatabaseConnection_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			FormDatabaseConnectionModify dialog = new FormDatabaseConnectionModify();

			if(NetTools.Tools.IsLangKorean())
				dialog.Text = "연결문자열 추가";
			else if(NetTools.Tools.IsLangJapanese()) 
				dialog.Text = "接続文字列の追加";
			else if(NetTools.Tools.IsLangChinese()) 
				dialog.Text = "添加连接字符串";
            else if (NetTools.Tools.IsLangVietnamese())
                dialog.Text = "Thêm chuỗi kết nối cơ sở dữ liệu";
			else
				dialog.Text = "Add Database Connection String";

            dialog.StartPosition = FormStartPosition.CenterParent;
			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				ListViewItem item = new ListViewItem();
				item.Text = dialog.textBoxTitle.Text;
				item.SubItems.Add(dialog.textBoxDsn.Text);
				item.SubItems.Add(dialog.m_dbType);
				item.SubItems.Add(dialog.m_dbConnectionType);
                item.SubItems.Add(dialog.add_commit.ToString());

				listViewConnection.Items.Add(item);
			}
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			if(listViewConnection.SelectedItems.Count == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("삭제하고 싶은 항목을 선택한 후 다시하세요.", "선택오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else
					MessageBox.Show("Select item to delete.", "Selection error");

				return;
			}

			int index = listViewConnection.SelectedItems[0].Index;

			listViewConnection.Items.RemoveAt(index);
		}

		void Modify()
		{
			if(listViewConnection.SelectedItems.Count == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("수정하고 싶은 항목을 선택한 후 다시하세요.", "선택오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请选择要修改的项。", "选择错误");
				else
					MessageBox.Show("Select item to modify.", "Selection error");

				return;
			}

			ListViewItem item = listViewConnection.SelectedItems[0];

			FormDatabaseConnectionModify dialog = new FormDatabaseConnectionModify();

			dialog.TopMost = this.TopMost;

			dialog.textBoxTitle.Text = item.SubItems[0].Text;
			dialog.textBoxDsn.Text = item.SubItems[1].Text;
			dialog.m_dbType = item.SubItems[2].Text;
			dialog.m_dbConnectionType = item.SubItems[3].Text;
            dialog.add_commit = ConvertTool.ToBoolean(item.SubItems[4].Text);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				item.SubItems[0].Text = dialog.textBoxTitle.Text;
				item.SubItems[1].Text = dialog.textBoxDsn.Text;
				item.SubItems[2].Text = dialog.m_dbType;
				item.SubItems[3].Text = dialog.m_dbConnectionType;
                item.SubItems[4].Text = dialog.add_commit.ToString();
			}
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void listViewConnection_DoubleClick(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void FormDatabaseConnection_Load(object sender, System.EventArgs e)
		{
			ListViewItem item;
			ConnectionString conn;
			for(int i = 0; i < connectionStringList.arrayConnectionString.Count; i++) 
			{
				conn = (ConnectionString)connectionStringList.arrayConnectionString[i];
				item = new ListViewItem();

				item.Text = conn.title; 
				item.SubItems.Add(conn.dsn);
				item.SubItems.Add(conn.dbtype.ToString());
				item.SubItems.Add(conn.dbConnectionType.ToString());
                item.SubItems.Add(conn.bAddCommitAfterCommand.ToString());
				listViewConnection.Items.Add(item);
			}
		}

        public void EnableButtonOK(bool flag)
        {
            this.buttonOK.Enabled = false;
        }

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			connectionStringList.arrayConnectionString.Clear();

			ConnectionString conn;
			ListViewItem item;

			for(int i = 0; i < listViewConnection.Items.Count; i++) 
			{
				item = listViewConnection.Items[i];
				conn = new ConnectionString();

				conn.title = item.SubItems[0].Text;
				conn.dsn = item.SubItems[1].Text;
				conn.dbtype = (EnumDbType)Enum.Parse(typeof(EnumDbType), item.SubItems[2].Text);
				conn.dbConnectionType = (EnumDbConnectionType)Enum.Parse(typeof(EnumDbConnectionType), item.SubItems[3].Text);
                conn.bAddCommitAfterCommand = ConvertTool.ToBoolean(item.SubItems[4].Text);

				connectionStringList.arrayConnectionString.Add(conn);
			}

			connectionStringList.ConnectionStringSave();
			

			DialogResult = DialogResult.OK;
			Close();
		}

		private void listViewConnection_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}

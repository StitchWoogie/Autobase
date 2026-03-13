using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DatabaseSaveList;
using DatabaseConnection;
using System.Data;
using AutoLibLocal;
using System.Threading.Tasks;

namespace DialogAddition
{
	/// <summary>
	/// Summary description for FormAddition.
	/// </summary>
	public class FormMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemConfigAddition;
		private System.ComponentModel.IContainer components;

		AdditionList additionList = new AdditionList();
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button buttonClose;
		Addition addition = new Addition();

		public FormMain()
		{
			LanguageTool.ChangeUICulture();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			additionList.Load();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemConfigAddition = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_list
            // 
            this.m_list.AccessibleDescription = null;
            this.m_list.AccessibleName = null;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.BackgroundImage = null;
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.m_list.Font = null;
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            resources.ApplyResources(this.columnHeader9, "columnHeader9");
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
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // columnHeader6
            // 
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // columnHeader8
            // 
            resources.ApplyResources(this.columnHeader8, "columnHeader8");
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            resources.ApplyResources(this.mainMenu1, "mainMenu1");
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemConfigAddition});
            // 
            // menuItemConfigAddition
            // 
            resources.ApplyResources(this.menuItemConfigAddition, "menuItemConfigAddition");
            this.menuItemConfigAddition.Index = 0;
            this.menuItemConfigAddition.Click += new System.EventHandler(this.menuItemConfigAddition_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonClose
            // 
            this.buttonClose.AccessibleDescription = null;
            this.buttonClose.AccessibleName = null;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.BackgroundImage = null;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Font = null;
            this.buttonClose.Name = "buttonClose";
            // 
            // FormMain
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.m_list);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void menuItemConfigAddition_Click(object sender, System.EventArgs e)
		{
			FormAddition dialog = new FormAddition(additionList, addition.classSaveList);

			dialog.TopMost = this.TopMost;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				UpdateListView();				
			}
		}

		void UpdateListView()
		{
			AdditionMember member;
			ListViewItem item;

			m_list.Items.Clear();

			for(int i = 0; i < additionList.arrayAddition.Count; i++) 
			{
				member = (AdditionMember)additionList.arrayAddition[i];

				item = new ListViewItem(member.sField);
				item.SubItems.Add("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				item.SubItems.Add("");

				m_list.Items.Add(item);
			}
		}

		private void FormMain_Load(object sender, System.EventArgs e)
		{
			UpdateListView();
			CalcAllItem();

			timer1.Enabled = true;
		}

		void UpdateOneItem(AdditionMember member, ListViewItem item)
		{
			SaveList save_list = addition.classSaveList.GetSaveList(member.sSaveItem);

			if(save_list == null)	return;	// save list not found
			
			ConnectionString dsn = addition.dsnList.GetConnection(save_list.con_dsn);

			if(dsn == null)	return;	// DSN not found

			CommonDbConnection db = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);
			try 
			{
				//db.ConnectionString = dsn.dsn;
			   db.Open();
			}
			catch 
			{
				return;
			}

			if(db.State != ConnectionState.Open)	return;

			string command_fr;
			string command_to;

			DateTime t = DateTime.Now;

			string field = DbTool.Field(dsn.dbtype, member.sField);
			double val_curr = 0;
			double val_old = 0;
			DateTime ct;

			// 금시 계산
			command_fr = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day, t.Hour, 0, 0);
			command_to = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day, t.Hour, 59, 59);
			val_curr = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
			item.SubItems[8].Text = val_curr.ToString();

			// 전시 계산
			ct = t.AddHours(-1);
			command_fr = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, ct.Day, ct.Hour, 0, 0);
			command_to = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, ct.Day, ct.Hour, 59, 59);
			val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
			item.SubItems[7].Text = val_old.ToString();

			// 금일 계산
			if(t.Hour > 0) 
			{	// 1시 이상일 경우
				command_fr = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day, 0, 0, 0);
				command_to = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day, t.Hour-1, 59, 59);
				if(save_list.bTableSaveHour) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
				}
				else if(save_list.bTableSaveMinute) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);					
				}
				else 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
				}
			}
			item.SubItems[6].Text = val_curr.ToString();

			// 전일 계산
			ct = t.AddDays(-1);
			command_fr = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, ct.Day, 0, 0, 0);
			command_to = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, ct.Day, 23, 59, 59);
			if(save_list.bTableSaveHour) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
			}
			else if(save_list.bTableSaveMinute) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
			}
			else 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
			}
			item.SubItems[5].Text = val_old.ToString();

			// 금월 계산
			if(t.Day > 1) 
			{	// 1시 이상일 경우
				command_fr = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, 1, 0, 0, 0);
				command_to = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month, t.Day-1, 23, 59, 59);
				if(save_list.bTableSaveDay) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveDay, field);
				}
				else if(save_list.bTableSaveHour) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
				}
				else if(save_list.bTableSaveMinute) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);					
				}
				else 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
				}
			}
			item.SubItems[4].Text = val_curr.ToString();

			// 전월 계산
			ct = t.AddMonths(-1);
			command_fr = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, 1, 0, 0, 0);
			command_to = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, ct.Month, 31, 23, 59, 59);
			if(save_list.bTableSaveDay) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveDay, field);
			}
			else if(save_list.bTableSaveHour) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
			}
			else if(save_list.bTableSaveMinute) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
			}
			else 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
			}
			item.SubItems[3].Text = val_old.ToString();


			// 금년 계산
			if(t.Month > 1) 
			{	// 1시 이상일 경우
				command_fr = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, 1, 1, 0, 0, 0);
				command_to = DbTool.MakeDateTimeString(dsn.dbtype, t.Year, t.Month-1, 31, 23, 59, 59);
				if(save_list.bTableSaveMonth) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMonth, field);
				}
				else if(save_list.bTableSaveDay) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveDay, field);
				}
				else if(save_list.bTableSaveHour) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
				}
				else if(save_list.bTableSaveMinute) 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);					
				}
				else 
				{
					val_curr += Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
				}
			}
			item.SubItems[2].Text = val_curr.ToString();

			// 전년 계산
			ct = t.AddYears(-1);
			command_fr = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, 1, 1, 0, 0, 0);
			command_to = DbTool.MakeDateTimeString(dsn.dbtype, ct.Year, 12, 31, 23, 59, 59);
			if(save_list.bTableSaveMonth) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMonth, field);
			}
			else if(save_list.bTableSaveDay) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveDay, field);
			}
			else if(save_list.bTableSaveHour) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveHour, field);
			}
			else if(save_list.bTableSaveMinute) 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.sTableSaveMinute, field);
			}
			else 
			{
				val_old = Addition.GetAddedValueFromTable(db, command_fr, command_to, save_list.table, field);
			}
			item.SubItems[1].Text = val_old.ToString();

			db.Close();
		}

		int old_min;
		bool refresh_flag = false;

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			DateTime t = DateTime.Now;

			if(!refresh_flag) 
			{
				if(t.Second >= 10) 
				{
					refresh_flag = true;
					CalcAllItem();
				}
			}
			else 
			{
				if(old_min != t.Minute) 
				{
					old_min = t.Minute;
					refresh_flag = false;
				}
			}
		}

		void CalcAllItem()
		{
			AdditionMember member;
			ListViewItem item;

			for(int i = 0; i < additionList.arrayAddition.Count; i++) 
			{
				member = (AdditionMember)additionList.arrayAddition[i];
				item = m_list.Items[i];
				
				UpdateOneItem(member, item);
			}

			refresh_flag = true;
		}
	}
}

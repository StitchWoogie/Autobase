using System;
using System.Drawing;
using System.Windows.Forms;

namespace Studio
{
	partial class FormConfigDemandNew
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
				components.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.m_list = new ListView();
			this.buttonAdd = new Button();
			this.buttonModify = new Button();
			this.buttonDelete = new Button();
			this.buttonClose = new Button();
			this.SuspendLayout();
			//
			// m_list
			//
			this.m_list.FullRowSelect = true;
			this.m_list.GridLines = true;
			this.m_list.View = View.Details;
			this.m_list.Location = new Point(12, 12);
			this.m_list.Size = new Size(560, 300);
			this.m_list.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.m_list.Columns.Add("Block ID", 100);
			this.m_list.Columns.Add("Title", 150);
			this.m_list.Columns.Add("Mode", 70);
			this.m_list.Columns.Add("Interval", 60);
			this.m_list.Columns.Add("Contract kW", 90);
			this.m_list.Columns.Add("Loads", 60);
			this.m_list.DoubleClick += new EventHandler(this.m_list_DoubleClick);
			//
			// buttonAdd
			//
			this.buttonAdd.Text = "Add";
			this.buttonAdd.Location = new Point(580, 12);
			this.buttonAdd.Size = new Size(90, 28);
			this.buttonAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.buttonAdd.Click += new EventHandler(this.buttonAdd_Click);
			//
			// buttonModify
			//
			this.buttonModify.Text = "Modify";
			this.buttonModify.Location = new Point(580, 46);
			this.buttonModify.Size = new Size(90, 28);
			this.buttonModify.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.buttonModify.Click += new EventHandler(this.buttonModify_Click);
			//
			// buttonDelete
			//
			this.buttonDelete.Text = "Delete";
			this.buttonDelete.Location = new Point(580, 80);
			this.buttonDelete.Size = new Size(90, 28);
			this.buttonDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
			//
			// buttonClose
			//
			this.buttonClose.Text = "Close";
			this.buttonClose.Location = new Point(580, 280);
			this.buttonClose.Size = new Size(90, 28);
			this.buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.buttonClose.Click += new EventHandler(this.buttonClose_Click);
			//
			// FormConfigDemandNew
			//
			this.Text = "Demand Control (New) Settings";
			this.ClientSize = new Size(682, 324);
			this.Controls.AddRange(new Control[] { this.m_list, this.buttonAdd, this.buttonModify, this.buttonDelete, this.buttonClose });
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.ResumeLayout(false);
		}

		#endregion

		private ListView m_list;
		private Button buttonAdd;
		private Button buttonModify;
		private Button buttonDelete;
		private Button buttonClose;
	}
}

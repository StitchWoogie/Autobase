using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for PropertyPageObjectEtcDatabase.
	/// </summary>
	public class PropertyPageObjectEtcDatabase : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxFilename;
		private System.Windows.Forms.Button buttonFindFilename;
		private System.Windows.Forms.Button buttonVarFilename;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonVarTable;
		private System.Windows.Forms.Button buttonFindTable;
		private System.Windows.Forms.TextBox textBoxTable;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button buttonVarColumn;
		private System.Windows.Forms.Button buttonFileColumn;
		private System.Windows.Forms.TextBox textBoxColumn;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button buttonVarWhere;
		private System.Windows.Forms.TextBox textBoxWhere;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button buttonVarOrderBy;
		private System.Windows.Forms.TextBox textBoxOrderBy;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectEtcDatabase()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectEtcDatabase));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonVarFilename = new System.Windows.Forms.Button();
            this.buttonFindFilename = new System.Windows.Forms.Button();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonVarTable = new System.Windows.Forms.Button();
            this.buttonFindTable = new System.Windows.Forms.Button();
            this.textBoxTable = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonVarColumn = new System.Windows.Forms.Button();
            this.buttonFileColumn = new System.Windows.Forms.Button();
            this.textBoxColumn = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonVarWhere = new System.Windows.Forms.Button();
            this.textBoxWhere = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonVarOrderBy = new System.Windows.Forms.Button();
            this.textBoxOrderBy = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonVarFilename);
            this.groupBox1.Controls.Add(this.buttonFindFilename);
            this.groupBox1.Controls.Add(this.textBoxFilename);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonVarFilename
            // 
            resources.ApplyResources(this.buttonVarFilename, "buttonVarFilename");
            this.buttonVarFilename.Name = "buttonVarFilename";
            this.buttonVarFilename.Click += new System.EventHandler(this.buttonVarFilename_Click);
            // 
            // buttonFindFilename
            // 
            resources.ApplyResources(this.buttonFindFilename, "buttonFindFilename");
            this.buttonFindFilename.Name = "buttonFindFilename";
            this.buttonFindFilename.Click += new System.EventHandler(this.buttonFindFilename_Click);
            // 
            // textBoxFilename
            // 
            resources.ApplyResources(this.textBoxFilename, "textBoxFilename");
            this.textBoxFilename.Name = "textBoxFilename";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonVarTable);
            this.groupBox2.Controls.Add(this.buttonFindTable);
            this.groupBox2.Controls.Add(this.textBoxTable);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // buttonVarTable
            // 
            resources.ApplyResources(this.buttonVarTable, "buttonVarTable");
            this.buttonVarTable.Name = "buttonVarTable";
            this.buttonVarTable.Click += new System.EventHandler(this.buttonVarTable_Click);
            // 
            // buttonFindTable
            // 
            resources.ApplyResources(this.buttonFindTable, "buttonFindTable");
            this.buttonFindTable.Name = "buttonFindTable";
            // 
            // textBoxTable
            // 
            resources.ApplyResources(this.textBoxTable, "textBoxTable");
            this.textBoxTable.Name = "textBoxTable";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonVarColumn);
            this.groupBox3.Controls.Add(this.buttonFileColumn);
            this.groupBox3.Controls.Add(this.textBoxColumn);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // buttonVarColumn
            // 
            resources.ApplyResources(this.buttonVarColumn, "buttonVarColumn");
            this.buttonVarColumn.Name = "buttonVarColumn";
            this.buttonVarColumn.Click += new System.EventHandler(this.buttonVarColumn_Click);
            // 
            // buttonFileColumn
            // 
            resources.ApplyResources(this.buttonFileColumn, "buttonFileColumn");
            this.buttonFileColumn.Name = "buttonFileColumn";
            // 
            // textBoxColumn
            // 
            resources.ApplyResources(this.textBoxColumn, "textBoxColumn");
            this.textBoxColumn.Name = "textBoxColumn";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonVarWhere);
            this.groupBox4.Controls.Add(this.textBoxWhere);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // buttonVarWhere
            // 
            resources.ApplyResources(this.buttonVarWhere, "buttonVarWhere");
            this.buttonVarWhere.Name = "buttonVarWhere";
            this.buttonVarWhere.Click += new System.EventHandler(this.buttonVarWhere_Click);
            // 
            // textBoxWhere
            // 
            resources.ApplyResources(this.textBoxWhere, "textBoxWhere");
            this.textBoxWhere.Name = "textBoxWhere";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonVarOrderBy);
            this.groupBox5.Controls.Add(this.textBoxOrderBy);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // buttonVarOrderBy
            // 
            resources.ApplyResources(this.buttonVarOrderBy, "buttonVarOrderBy");
            this.buttonVarOrderBy.Name = "buttonVarOrderBy";
            this.buttonVarOrderBy.Click += new System.EventHandler(this.buttonVarOrderBy_Click);
            // 
            // textBoxOrderBy
            // 
            resources.ApplyResources(this.textBoxOrderBy, "textBoxOrderBy");
            this.textBoxOrderBy.Name = "textBoxOrderBy";
            // 
            // PropertyPageObjectEtcDatabase
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Name = "PropertyPageObjectEtcDatabase";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetItem(OBJECT_ETC_DATABASE obj)
		{
			this.textBoxFilename.Text = obj.filename;
			this.textBoxTable.Text = obj.table;
			this.textBoxColumn.Text = obj.field;
			this.textBoxWhere.Text = obj.where;
			this.textBoxOrderBy.Text = obj.orderby;
		}

		public void GetItem(OBJECT_ETC_DATABASE obj)
		{
			obj.filename = this.textBoxFilename.Text;
			obj.table = this.textBoxTable.Text;
			obj.field = this.textBoxColumn.Text;
			obj.where = this.textBoxWhere.Text;
			obj.orderby = this.textBoxOrderBy.Text;
		}

		private void buttonVarFilename_Click(object sender, System.EventArgs e)
		{
			FormDialogSelectVars dialog = new FormDialogSelectVars();

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxFilename.Text = "$"+dialog.sSelectedItem;
			}
		}

		private void buttonVarTable_Click(object sender, System.EventArgs e)
		{
			FormDialogSelectVars dialog = new FormDialogSelectVars();

			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				this.textBoxTable.Text = "$"+dialog.sSelectedItem;
			}
		}

		private void buttonVarColumn_Click(object sender, System.EventArgs e)
		{
			FormDialogSelectVars dialog = new FormDialogSelectVars();

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxColumn.Text = "$"+dialog.sSelectedItem;
			}
		}

		private void buttonVarWhere_Click(object sender, System.EventArgs e)
		{
			FormDialogSelectVars dialog = new FormDialogSelectVars();

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxWhere.Text = "$"+dialog.sSelectedItem;
			}
		}

		private void buttonVarOrderBy_Click(object sender, System.EventArgs e)
		{
			FormDialogSelectVars dialog = new FormDialogSelectVars();

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxOrderBy.Text = "$"+dialog.sSelectedItem;
			}
		}

		private void buttonFindFilename_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Access Files (*.mdb)|*.mdb";
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxFilename.Text = dialog.FileName;
			}
		}
	}
}

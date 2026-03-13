using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using DatabaseConnection;
using AutoLibLocal;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectCircle.
	/// </summary>
	public class PropertyPageObjectDatabase : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboBoxDsn;
		private System.Windows.Forms.TextBox textBoxTable;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonDsn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxUseRowCount;
		private System.Windows.Forms.CheckBox checkBoxAutoUpdate;
		private System.Windows.Forms.CheckBox checkBoxUseFullCursor;
		private System.Windows.Forms.NumericUpDown numericUpDownUpdateTime;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.RadioButton radioButtonConnType0;
		private System.Windows.Forms.RadioButton radioButtonConnType1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxFilename;
		private System.Windows.Forms.Button buttonFilename;
		private System.Windows.Forms.CheckBox checkBoxUseGrid;
        private CheckBox checkBoxReverseRowCount;
		private System.Windows.Forms.RadioButton radioButtonConnType2;
		private System.Windows.Forms.GroupBox groupBoxSql;
		private System.Windows.Forms.Label labelSqlWhere;
		private System.Windows.Forms.TextBox textBoxSqlWhere;
		private System.Windows.Forms.Label labelSqlOrderBy;
		private System.Windows.Forms.TextBox textBoxSqlOrderBy;
		private System.Windows.Forms.GroupBox groupBoxRecordLimit;
		private System.Windows.Forms.NumericUpDown numericUpDownRecordLimit;
		private System.Windows.Forms.Label labelRecordLimit;
		private System.Windows.Forms.GroupBox groupBoxDisplay;
		private System.Windows.Forms.CheckBox checkBoxUseAlternateRowColor;
		private System.Windows.Forms.CheckBox checkBoxUsePagination;
		private System.Windows.Forms.NumericUpDown numericUpDownPageSize;
		private System.Windows.Forms.Label labelPageSize;

		ConnectionStringList listDsn = new ConnectionStringList();

		public PropertyPageObjectDatabase()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectDatabase));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonFilename = new System.Windows.Forms.Button();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButtonConnType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonConnType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonConnType0 = new System.Windows.Forms.RadioButton();
            this.comboBoxDsn = new System.Windows.Forms.ComboBox();
            this.textBoxTable = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDsn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxUseRowCount = new System.Windows.Forms.CheckBox();
            this.checkBoxUseGrid = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoUpdate = new System.Windows.Forms.CheckBox();
            this.checkBoxUseFullCursor = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownUpdateTime = new System.Windows.Forms.NumericUpDown();
            this.checkBoxReverseRowCount = new System.Windows.Forms.CheckBox();
            this.groupBoxSql = new System.Windows.Forms.GroupBox();
            this.labelSqlWhere = new System.Windows.Forms.Label();
            this.textBoxSqlWhere = new System.Windows.Forms.TextBox();
            this.labelSqlOrderBy = new System.Windows.Forms.Label();
            this.textBoxSqlOrderBy = new System.Windows.Forms.TextBox();
            this.groupBoxRecordLimit = new System.Windows.Forms.GroupBox();
            this.labelRecordLimit = new System.Windows.Forms.Label();
            this.numericUpDownRecordLimit = new System.Windows.Forms.NumericUpDown();
            this.groupBoxDisplay = new System.Windows.Forms.GroupBox();
            this.checkBoxUseAlternateRowColor = new System.Windows.Forms.CheckBox();
            this.checkBoxUsePagination = new System.Windows.Forms.CheckBox();
            this.labelPageSize = new System.Windows.Forms.Label();
            this.numericUpDownPageSize = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpdateTime)).BeginInit();
            this.groupBoxSql.SuspendLayout();
            this.groupBoxRecordLimit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecordLimit)).BeginInit();
            this.groupBoxDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPageSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonFilename);
            this.groupBox1.Controls.Add(this.textBoxFilename);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.radioButtonConnType2);
            this.groupBox1.Controls.Add(this.radioButtonConnType1);
            this.groupBox1.Controls.Add(this.radioButtonConnType0);
            this.groupBox1.Controls.Add(this.comboBoxDsn);
            this.groupBox1.Controls.Add(this.textBoxTable);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.buttonDsn);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonFilename
            // 
            resources.ApplyResources(this.buttonFilename, "buttonFilename");
            this.buttonFilename.Name = "buttonFilename";
            this.buttonFilename.Click += new System.EventHandler(this.buttonFilename_Click);
            // 
            // textBoxFilename
            // 
            resources.ApplyResources(this.textBoxFilename, "textBoxFilename");
            this.textBoxFilename.Name = "textBoxFilename";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // radioButtonConnType2
            // 
            resources.ApplyResources(this.radioButtonConnType2, "radioButtonConnType2");
            this.radioButtonConnType2.Name = "radioButtonConnType2";
            this.radioButtonConnType2.CheckedChanged += new System.EventHandler(this.radioButtonConnType2_CheckedChanged);
            // 
            // radioButtonConnType1
            // 
            resources.ApplyResources(this.radioButtonConnType1, "radioButtonConnType1");
            this.radioButtonConnType1.Name = "radioButtonConnType1";
            this.radioButtonConnType1.CheckedChanged += new System.EventHandler(this.radioButtonConnType1_CheckedChanged);
            // 
            // radioButtonConnType0
            // 
            resources.ApplyResources(this.radioButtonConnType0, "radioButtonConnType0");
            this.radioButtonConnType0.Name = "radioButtonConnType0";
            this.radioButtonConnType0.CheckedChanged += new System.EventHandler(this.radioButtonConnType0_CheckedChanged);
            // 
            // comboBoxDsn
            // 
            resources.ApplyResources(this.comboBoxDsn, "comboBoxDsn");
            this.comboBoxDsn.Name = "comboBoxDsn";
            // 
            // textBoxTable
            // 
            resources.ApplyResources(this.textBoxTable, "textBoxTable");
            this.textBoxTable.Name = "textBoxTable";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // buttonDsn
            // 
            resources.ApplyResources(this.buttonDsn, "buttonDsn");
            this.buttonDsn.Name = "buttonDsn";
            this.buttonDsn.Click += new System.EventHandler(this.buttonDsn_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkBoxUseRowCount
            // 
            resources.ApplyResources(this.checkBoxUseRowCount, "checkBoxUseRowCount");
            this.checkBoxUseRowCount.Name = "checkBoxUseRowCount";
            // 
            // checkBoxUseGrid
            // 
            resources.ApplyResources(this.checkBoxUseGrid, "checkBoxUseGrid");
            this.checkBoxUseGrid.Name = "checkBoxUseGrid";
            // 
            // checkBoxAutoUpdate
            // 
            resources.ApplyResources(this.checkBoxAutoUpdate, "checkBoxAutoUpdate");
            this.checkBoxAutoUpdate.Name = "checkBoxAutoUpdate";
            this.checkBoxAutoUpdate.CheckedChanged += new System.EventHandler(this.checkBoxAutoUpdate_CheckedChanged);
            // 
            // checkBoxUseFullCursor
            // 
            resources.ApplyResources(this.checkBoxUseFullCursor, "checkBoxUseFullCursor");
            this.checkBoxUseFullCursor.Name = "checkBoxUseFullCursor";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numericUpDownUpdateTime);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // numericUpDownUpdateTime
            // 
            resources.ApplyResources(this.numericUpDownUpdateTime, "numericUpDownUpdateTime");
            this.numericUpDownUpdateTime.Name = "numericUpDownUpdateTime";
            // 
            // checkBoxReverseRowCount
            // 
            resources.ApplyResources(this.checkBoxReverseRowCount, "checkBoxReverseRowCount");
            this.checkBoxReverseRowCount.Name = "checkBoxReverseRowCount";
            // 
            // groupBoxSql
            // 
            this.groupBoxSql.Controls.Add(this.labelSqlWhere);
            this.groupBoxSql.Controls.Add(this.textBoxSqlWhere);
            this.groupBoxSql.Controls.Add(this.labelSqlOrderBy);
            this.groupBoxSql.Controls.Add(this.textBoxSqlOrderBy);
            resources.ApplyResources(this.groupBoxSql, "groupBoxSql");
            this.groupBoxSql.Name = "groupBoxSql";
            this.groupBoxSql.TabStop = false;
            // 
            // labelSqlWhere
            // 
            resources.ApplyResources(this.labelSqlWhere, "labelSqlWhere");
            this.labelSqlWhere.Name = "labelSqlWhere";
            // 
            // textBoxSqlWhere
            // 
            resources.ApplyResources(this.textBoxSqlWhere, "textBoxSqlWhere");
            this.textBoxSqlWhere.Name = "textBoxSqlWhere";
            // 
            // labelSqlOrderBy
            // 
            resources.ApplyResources(this.labelSqlOrderBy, "labelSqlOrderBy");
            this.labelSqlOrderBy.Name = "labelSqlOrderBy";
            // 
            // textBoxSqlOrderBy
            // 
            resources.ApplyResources(this.textBoxSqlOrderBy, "textBoxSqlOrderBy");
            this.textBoxSqlOrderBy.Name = "textBoxSqlOrderBy";
            // 
            // groupBoxRecordLimit
            // 
            this.groupBoxRecordLimit.Controls.Add(this.labelRecordLimit);
            this.groupBoxRecordLimit.Controls.Add(this.numericUpDownRecordLimit);
            resources.ApplyResources(this.groupBoxRecordLimit, "groupBoxRecordLimit");
            this.groupBoxRecordLimit.Name = "groupBoxRecordLimit";
            this.groupBoxRecordLimit.TabStop = false;
            // 
            // labelRecordLimit
            // 
            resources.ApplyResources(this.labelRecordLimit, "labelRecordLimit");
            this.labelRecordLimit.Name = "labelRecordLimit";
            // 
            // numericUpDownRecordLimit
            // 
            resources.ApplyResources(this.numericUpDownRecordLimit, "numericUpDownRecordLimit");
            this.numericUpDownRecordLimit.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownRecordLimit.Name = "numericUpDownRecordLimit";
            // 
            // groupBoxDisplay
            // 
            this.groupBoxDisplay.Controls.Add(this.checkBoxUseAlternateRowColor);
            this.groupBoxDisplay.Controls.Add(this.checkBoxUsePagination);
            this.groupBoxDisplay.Controls.Add(this.labelPageSize);
            this.groupBoxDisplay.Controls.Add(this.numericUpDownPageSize);
            resources.ApplyResources(this.groupBoxDisplay, "groupBoxDisplay");
            this.groupBoxDisplay.Name = "groupBoxDisplay";
            this.groupBoxDisplay.TabStop = false;
            // 
            // checkBoxUseAlternateRowColor
            // 
            resources.ApplyResources(this.checkBoxUseAlternateRowColor, "checkBoxUseAlternateRowColor");
            this.checkBoxUseAlternateRowColor.Name = "checkBoxUseAlternateRowColor";
            // 
            // checkBoxUsePagination
            // 
            resources.ApplyResources(this.checkBoxUsePagination, "checkBoxUsePagination");
            this.checkBoxUsePagination.Name = "checkBoxUsePagination";
            this.checkBoxUsePagination.CheckedChanged += new System.EventHandler(this.checkBoxUsePagination_CheckedChanged);
            // 
            // labelPageSize
            // 
            resources.ApplyResources(this.labelPageSize, "labelPageSize");
            this.labelPageSize.Name = "labelPageSize";
            // 
            // numericUpDownPageSize
            // 
            resources.ApplyResources(this.numericUpDownPageSize, "numericUpDownPageSize");
            this.numericUpDownPageSize.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownPageSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPageSize.Name = "numericUpDownPageSize";
            this.numericUpDownPageSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // PropertyPageObjectDatabase
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBoxDisplay);
            this.Controls.Add(this.groupBoxRecordLimit);
            this.Controls.Add(this.groupBoxSql);
            this.Controls.Add(this.checkBoxReverseRowCount);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxUseFullCursor);
            this.Controls.Add(this.checkBoxAutoUpdate);
            this.Controls.Add(this.checkBoxUseGrid);
            this.Controls.Add(this.checkBoxUseRowCount);
            this.Controls.Add(this.groupBox1);
            this.Name = "PropertyPageObjectDatabase";
            this.Load += new System.EventHandler(this.PropertyPageObjectDatabase_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpdateTime)).EndInit();
            this.groupBoxSql.ResumeLayout(false);
            this.groupBoxSql.PerformLayout();
            this.groupBoxRecordLimit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecordLimit)).EndInit();
            this.groupBoxDisplay.ResumeLayout(false);
            this.groupBoxDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPageSize)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void label3_Click(object sender, System.EventArgs e)
		{
		
		}

		void EnableConnectionType()
		{
			bool flag_mdb = this.radioButtonConnType0.Checked;
			bool flag_dsn = this.radioButtonConnType1.Checked;
			bool flag_default = this.radioButtonConnType2.Checked;

			this.textBoxFilename.Enabled = flag_mdb;
			this.buttonFilename.Enabled = flag_mdb;
			this.comboBoxDsn.Enabled = flag_dsn;
			this.buttonDsn.Enabled = flag_dsn;
		}

		private void PropertyPageObjectDatabase_Load(object sender, System.EventArgs e)
		{
			listDsn.ConnectionStringLoad();
			DbTool.FillComboBox(this.comboBoxDsn, listDsn); 

			EnableConnectionType();
			EnableUpdateTime();
		}

		private void buttonDsn_Click(object sender, System.EventArgs e)
		{
			FormDatabaseConnection dialog = new FormDatabaseConnection(listDsn);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				DbTool.FillComboBox(this.comboBoxDsn, listDsn);
			}
		}

		public ObjectArgsDatabase ObjectArgs 
		{
			set 
			{
				this.comboBoxDsn.Text = value.dsn;
				this.textBoxTable.Text = value.table;
				this.checkBoxAutoUpdate.Checked = (value.bAutoUpdate == 1);
				this.checkBoxUseGrid.Checked = value.bUseGrid;
				this.checkBoxUseFullCursor.Checked = (value.bUseFullCursor == 1);
				this.checkBoxUseRowCount.Checked = (value.bUseNo == 1);
				this.numericUpDownUpdateTime.Value = value.nUpdateTime;
				this.textBoxFilename.Text = value.filename;

				this.radioButtonConnType0.Checked = (value.nConnectionType == 0);
				this.radioButtonConnType1.Checked = (value.nConnectionType == 1);
				this.radioButtonConnType2.Checked = (value.nConnectionType == 2);

                this.checkBoxReverseRowCount.Checked = value.bReverseNo;

				this.textBoxSqlWhere.Text = value.sSqlTextWhere ?? "";
				this.textBoxSqlOrderBy.Text = value.sSqlTextOrderBy ?? "";
				this.numericUpDownRecordLimit.Value = Math.Max(0, Math.Min(value.nRecordLimit, 1000000));

				this.checkBoxUseAlternateRowColor.Checked = value.bUseAlternateRowColor;
				this.checkBoxUsePagination.Checked = value.bUsePagination;
				this.numericUpDownPageSize.Value = Math.Max(10, Math.Min(value.nPageSize > 0 ? value.nPageSize : 100, 100000));
				this.numericUpDownPageSize.Enabled = value.bUsePagination;
			}
			get 
			{
				ObjectArgsDatabase args = new ObjectArgsDatabase();

				args.dsn = this.comboBoxDsn.Text;
				args.table = this.textBoxTable.Text;
				args.bAutoUpdate = this.checkBoxAutoUpdate.Checked ? (sbyte)1 : (sbyte)0;
				args.bUseGrid = this.checkBoxUseGrid.Checked;
				args.bUseFullCursor = this.checkBoxUseFullCursor.Checked ? (sbyte)1 : (sbyte)0;
				args.bUseNo = this.checkBoxUseRowCount.Checked ? (sbyte)1 : (sbyte)0;
				args.nUpdateTime = ConvertTool.ToInt32(this.numericUpDownUpdateTime.Value);

				args.filename = this.textBoxFilename.Text;

				if(this.radioButtonConnType0.Checked)		args.nConnectionType = 0;
				else if(this.radioButtonConnType1.Checked)	args.nConnectionType = 1;
				else if(this.radioButtonConnType2.Checked)	args.nConnectionType = 2;
				else										args.nConnectionType = 0;

                args.bReverseNo = this.checkBoxReverseRowCount.Checked;

				args.sSqlTextWhere = this.textBoxSqlWhere.Text;
				args.sSqlTextOrderBy = this.textBoxSqlOrderBy.Text;
				args.nRecordLimit = ConvertTool.ToInt32(this.numericUpDownRecordLimit.Value);

				args.bUseAlternateRowColor = this.checkBoxUseAlternateRowColor.Checked;
				args.bUsePagination = this.checkBoxUsePagination.Checked;
				args.nPageSize = ConvertTool.ToInt32(this.numericUpDownPageSize.Value);

				return args;
			}
		}

		void EnableUpdateTime()
		{
			this.numericUpDownUpdateTime.Enabled = this.checkBoxAutoUpdate.Checked;
		}

		private void checkBoxAutoUpdate_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableUpdateTime();
		}

		private void radioButtonConnType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableConnectionType();			
		}

		private void radioButtonConnType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableConnectionType();
		}

		private void radioButtonConnType2_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableConnectionType();
		}

		private void checkBoxUsePagination_CheckedChanged(object sender, System.EventArgs e)
		{
			this.numericUpDownPageSize.Enabled = this.checkBoxUsePagination.Checked;
		}

		private void buttonFilename_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Access files (*.mdb)|*.mdb";

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxFilename.Text = dialog.FileName;
			}
		}
	}
}

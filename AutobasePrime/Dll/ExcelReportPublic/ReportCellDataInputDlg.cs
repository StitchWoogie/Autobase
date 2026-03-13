using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for ReportCellDataInputDlg.
	/// </summary>
	public class ReportCellDataInputDlg : System.Windows.Forms.Form
	{
		public System.Windows.Forms.ComboBox comboBox_Indetify_Char;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox textBox_Table_Name;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox textBox_Field_Name;
		public System.Windows.Forms.ComboBox comboBox_Time_Type;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.ComboBox comboBox_Time_Range;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WorkDataStruct work;
		public enum eDataType { DB_DATA = 0, BASIC_TIME, CURRENT_TIME, BASIC_DATA };
		public eDataType dataTypeNum;

		public ReportCellDataInputDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			work = new WorkDataStruct();
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
			this.comboBox_Indetify_Char = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox_Table_Name = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox_Field_Name = new System.Windows.Forms.TextBox();
			this.comboBox_Time_Type = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBox_Time_Range = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.button_CANCEL = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBox_Indetify_Char
			// 
			this.comboBox_Indetify_Char.Location = new System.Drawing.Point(8, 48);
			this.comboBox_Indetify_Char.Name = "comboBox_Indetify_Char";
			this.comboBox_Indetify_Char.Size = new System.Drawing.Size(104, 20);
			this.comboBox_Indetify_Char.TabIndex = 11;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 10;
			this.label1.Text = "데이터 종류";
			// 
			// textBox_Table_Name
			// 
			this.textBox_Table_Name.Location = new System.Drawing.Point(128, 48);
			this.textBox_Table_Name.Name = "textBox_Table_Name";
			this.textBox_Table_Name.Size = new System.Drawing.Size(136, 21);
			this.textBox_Table_Name.TabIndex = 12;
			this.textBox_Table_Name.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(160, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 13;
			this.label2.Text = "테이블 명";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(320, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 15;
			this.label3.Text = "필드 명";
			// 
			// textBox_Field_Name
			// 
			this.textBox_Field_Name.Location = new System.Drawing.Point(288, 48);
			this.textBox_Field_Name.Name = "textBox_Field_Name";
			this.textBox_Field_Name.Size = new System.Drawing.Size(112, 21);
			this.textBox_Field_Name.TabIndex = 14;
			this.textBox_Field_Name.Text = "";
			// 
			// comboBox_Time_Type
			// 
			this.comboBox_Time_Type.Location = new System.Drawing.Point(8, 104);
			this.comboBox_Time_Type.Name = "comboBox_Time_Type";
			this.comboBox_Time_Type.Size = new System.Drawing.Size(104, 20);
			this.comboBox_Time_Type.TabIndex = 17;
			this.comboBox_Time_Type.SelectedIndexChanged += new System.EventHandler(this.comboBox_Time_Type_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 16);
			this.label4.TabIndex = 16;
			this.label4.Text = "자료시간 구분";
			// 
			// comboBox_Time_Range
			// 
			this.comboBox_Time_Range.Location = new System.Drawing.Point(128, 104);
			this.comboBox_Time_Range.Name = "comboBox_Time_Range";
			this.comboBox_Time_Range.Size = new System.Drawing.Size(104, 20);
			this.comboBox_Time_Range.TabIndex = 19;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(136, 88);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(88, 16);
			this.label5.TabIndex = 18;
			this.label5.Text = "자료시간 범위";
			// 
			// button_CANCEL
			// 
			this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_CANCEL.Location = new System.Drawing.Point(264, 168);
			this.button_CANCEL.Name = "button_CANCEL";
			this.button_CANCEL.TabIndex = 21;
			this.button_CANCEL.Text = "취소";
			// 
			// button_OK
			// 
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(88, 168);
			this.button_OK.Name = "button_OK";
			this.button_OK.TabIndex = 20;
			this.button_OK.Text = "확인";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label4,
																					this.textBox_Table_Name,
																					this.label3,
																					this.label2,
																					this.textBox_Field_Name,
																					this.label5,
																					this.comboBox_Indetify_Char,
																					this.comboBox_Time_Type,
																					this.label1,
																					this.comboBox_Time_Range});
			this.groupBox1.Location = new System.Drawing.Point(16, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(416, 144);
			this.groupBox1.TabIndex = 22;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Cell 자료편집";
			// 
			// ReportCellDataInputDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(448, 198);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1,
																		  this.button_CANCEL,
																		  this.button_OK});
			this.Name = "ReportCellDataInputDlg";
			this.Text = "Cell자료 입력";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void makeAndFillTimeRangeBuf()
		{
			string		buf;
			int			i, pos, start, count;

			comboBox_Time_Range.Items.Clear();
			pos = comboBox_Time_Type.SelectedIndex;

			start = 0;
			switch(pos)
			{
				case 4 : count = 100;	break;	// year
				case 3 : start = 1; count = 12;	break;	// month
				case 2 : start = 1; count = 31;	break;	// day
				case 1 : count = 24;	break;	// hour
				default : count = 60;	break;	// min

			}
			for(i = start; i < count; i++) 
			{
				buf = String.Format("{0,2:d02}", i);
				this.comboBox_Time_Range.Items.Add(buf);
			}
		}

		private void comboBox_Time_Type_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			makeAndFillTimeRangeBuf();
		}		
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for edit_OnOffListFieldNoDlg.
	/// </summary>
	public class edit_OnOffListFieldNoDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public System.Windows.Forms.NumericUpDown numericUpDown_addItemNo;
		private System.Windows.Forms.RadioButton radioButton_addSaveItem;
		private System.Windows.Forms.RadioButton radioButton_operationTime;
		private System.Windows.Forms.RadioButton radioButton_endTime;
		private System.Windows.Forms.RadioButton radioButton_startTime;
		private System.Windows.Forms.RadioButton radioButton_tag;

		public bool	bAddItemCheck;
		private System.Windows.Forms.RadioButton radioButton_No;
		public int	nSelectedPos;

		public edit_OnOffListFieldNoDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit_OnOffListFieldNoDlg));
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_No = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_addItemNo = new System.Windows.Forms.NumericUpDown();
            this.radioButton_addSaveItem = new System.Windows.Forms.RadioButton();
            this.radioButton_operationTime = new System.Windows.Forms.RadioButton();
            this.radioButton_endTime = new System.Windows.Forms.RadioButton();
            this.radioButton_startTime = new System.Windows.Forms.RadioButton();
            this.radioButton_tag = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_addItemNo)).BeginInit();
            this.SuspendLayout();
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_No);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDown_addItemNo);
            this.groupBox1.Controls.Add(this.radioButton_addSaveItem);
            this.groupBox1.Controls.Add(this.radioButton_operationTime);
            this.groupBox1.Controls.Add(this.radioButton_endTime);
            this.groupBox1.Controls.Add(this.radioButton_startTime);
            this.groupBox1.Controls.Add(this.radioButton_tag);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButton_No
            // 
            resources.ApplyResources(this.radioButton_No, "radioButton_No");
            this.radioButton_No.Name = "radioButton_No";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // numericUpDown_addItemNo
            // 
            resources.ApplyResources(this.numericUpDown_addItemNo, "numericUpDown_addItemNo");
            this.numericUpDown_addItemNo.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown_addItemNo.Name = "numericUpDown_addItemNo";
            // 
            // radioButton_addSaveItem
            // 
            resources.ApplyResources(this.radioButton_addSaveItem, "radioButton_addSaveItem");
            this.radioButton_addSaveItem.Name = "radioButton_addSaveItem";
            this.radioButton_addSaveItem.CheckedChanged += new System.EventHandler(this.radioButton_Priority_CheckedChanged);
            // 
            // radioButton_operationTime
            // 
            resources.ApplyResources(this.radioButton_operationTime, "radioButton_operationTime");
            this.radioButton_operationTime.Name = "radioButton_operationTime";
            // 
            // radioButton_endTime
            // 
            resources.ApplyResources(this.radioButton_endTime, "radioButton_endTime");
            this.radioButton_endTime.Name = "radioButton_endTime";
            // 
            // radioButton_startTime
            // 
            resources.ApplyResources(this.radioButton_startTime, "radioButton_startTime");
            this.radioButton_startTime.Name = "radioButton_startTime";
            // 
            // radioButton_tag
            // 
            resources.ApplyResources(this.radioButton_tag, "radioButton_tag");
            this.radioButton_tag.Name = "radioButton_tag";
            // 
            // edit_OnOffListFieldNoDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "edit_OnOffListFieldNoDlg";
            this.Load += new System.EventHandler(this.edit_OnOffListFieldNoDlg_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_addItemNo)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		void setSelectPos()
		{
			if(this.nSelectedPos == 1) this.radioButton_startTime.Checked = true;
			else if(this.nSelectedPos == 2) this.radioButton_endTime.Checked = true;
			else if(this.nSelectedPos == 3) this.radioButton_operationTime.Checked = true;
			else if(this.nSelectedPos == 4) this.radioButton_addSaveItem.Checked = true;
			else if(this.nSelectedPos == 5) this.radioButton_No.Checked = true;
			else this.radioButton_tag.Checked = true;
		}
		
		void EnableDisableItem()
		{
			if(bAddItemCheck)
				this.numericUpDown_addItemNo.Enabled = true;
			else
				this.numericUpDown_addItemNo.Enabled = false;
		}

		private void radioButton_Priority_CheckedChanged(object sender, System.EventArgs e)
		{
			bAddItemCheck = radioButton_addSaveItem.Checked;
			EnableDisableItem();
		}

		private void edit_OnOffListFieldNoDlg_Load(object sender, System.EventArgs e)
		{
			bAddItemCheck = radioButton_addSaveItem.Checked;
			setSelectPos();
			EnableDisableItem();
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(radioButton_startTime.Checked) nSelectedPos = 1;
			else if(radioButton_endTime.Checked) nSelectedPos = 2;
			else if(radioButton_operationTime.Checked) nSelectedPos = 3;
			else if(radioButton_addSaveItem.Checked) nSelectedPos = 4;
			else if(radioButton_No.Checked) nSelectedPos = 5;
			else nSelectedPos = 0;
		}


	}
}

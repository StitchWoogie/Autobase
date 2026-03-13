using System;
using System.Drawing;
using System.Windows.Forms;

namespace LocalMain.DemandNew
{
	partial class FormEditLoadModel
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
			this.lblLoadId = new Label();
			this.lblDisplayName = new Label();
			this.lblPriority = new Label();
			this.lblGroup = new Label();
			this.lblEstimatedKW = new Label();
			this.lblCommandTag = new Label();
			this.lblFeedbackTag = new Label();
			this.lblInterlockTag = new Label();
			this.lblMinOff = new Label();
			this.lblMinOn = new Label();
			this.lblBlockTime = new Label();
			this.txtLoadId = new TextBox();
			this.txtDisplayName = new TextBox();
			this.txtGroup = new TextBox();
			this.txtCommandTag = new TextBox();
			this.txtFeedbackTag = new TextBox();
			this.txtInterlockTag = new TextBox();
			this.btnCommandTag = new Button();
			this.btnFeedbackTag = new Button();
			this.btnInterlockTag = new Button();
			this.numPriority = new NumericUpDown();
			this.numEstimatedKW = new NumericUpDown();
			this.numMinOff = new NumericUpDown();
			this.numMinOn = new NumericUpDown();
			this.numReShedBlock = new NumericUpDown();
			this.chkReShedBlock = new CheckBox();
			this.btnOK = new Button();
			this.btnCancel = new Button();
			this.SuspendLayout();
			//
			// lblLoadId
			//
			this.lblLoadId.Text = "Load ID:";
			this.lblLoadId.Location = new Point(12, 15);
			this.lblLoadId.AutoSize = true;
			//
			// txtLoadId
			//
			this.txtLoadId.Location = new Point(140, 12);
			this.txtLoadId.Width = 200;
			//
			// lblDisplayName
			//
			this.lblDisplayName.Text = "Display Name:";
			this.lblDisplayName.Location = new Point(12, 43);
			this.lblDisplayName.AutoSize = true;
			//
			// txtDisplayName
			//
			this.txtDisplayName.Location = new Point(140, 40);
			this.txtDisplayName.Width = 200;
			//
			// lblPriority
			//
			this.lblPriority.Text = "Priority:";
			this.lblPriority.Location = new Point(12, 71);
			this.lblPriority.AutoSize = true;
			//
			// numPriority
			//
			this.numPriority.Location = new Point(140, 68);
			this.numPriority.Width = 60;
			this.numPriority.Minimum = 1;
			this.numPriority.Maximum = 100;
			this.numPriority.Value = 1;
			//
			// lblGroup
			//
			this.lblGroup.Text = "Group:";
			this.lblGroup.Location = new Point(12, 99);
			this.lblGroup.AutoSize = true;
			//
			// txtGroup
			//
			this.txtGroup.Location = new Point(140, 96);
			this.txtGroup.Width = 200;
			//
			// lblEstimatedKW
			//
			this.lblEstimatedKW.Text = "Estimated KW:";
			this.lblEstimatedKW.Location = new Point(12, 127);
			this.lblEstimatedKW.AutoSize = true;
			//
			// numEstimatedKW
			//
			this.numEstimatedKW.Location = new Point(140, 124);
			this.numEstimatedKW.Width = 100;
			this.numEstimatedKW.Minimum = 0;
			this.numEstimatedKW.Maximum = 99999;
			this.numEstimatedKW.DecimalPlaces = 1;
			//
			// lblCommandTag
			//
			this.lblCommandTag.Text = "Command Tag:";
			this.lblCommandTag.Location = new Point(12, 155);
			this.lblCommandTag.AutoSize = true;
			//
			// txtCommandTag
			//
			this.txtCommandTag.Location = new Point(140, 152);
			this.txtCommandTag.Width = 166;
			//
			// btnCommandTag
			//
			this.btnCommandTag.Text = "...";
			this.btnCommandTag.Location = new Point(310, 152);
			this.btnCommandTag.Width = 30;
			this.btnCommandTag.Click += new EventHandler(this.btnCommandTag_Click);
			//
			// lblFeedbackTag
			//
			this.lblFeedbackTag.Text = "Feedback Tag:";
			this.lblFeedbackTag.Location = new Point(12, 183);
			this.lblFeedbackTag.AutoSize = true;
			//
			// txtFeedbackTag
			//
			this.txtFeedbackTag.Location = new Point(140, 180);
			this.txtFeedbackTag.Width = 166;
			//
			// btnFeedbackTag
			//
			this.btnFeedbackTag.Text = "...";
			this.btnFeedbackTag.Location = new Point(310, 180);
			this.btnFeedbackTag.Width = 30;
			this.btnFeedbackTag.Click += new EventHandler(this.btnFeedbackTag_Click);
			//
			// lblInterlockTag
			//
			this.lblInterlockTag.Text = "Interlock Tag:";
			this.lblInterlockTag.Location = new Point(12, 211);
			this.lblInterlockTag.AutoSize = true;
			//
			// txtInterlockTag
			//
			this.txtInterlockTag.Location = new Point(140, 208);
			this.txtInterlockTag.Width = 166;
			//
			// btnInterlockTag
			//
			this.btnInterlockTag.Text = "...";
			this.btnInterlockTag.Location = new Point(310, 208);
			this.btnInterlockTag.Width = 30;
			this.btnInterlockTag.Click += new EventHandler(this.btnInterlockTag_Click);
			//
			// lblMinOff
			//
			this.lblMinOff.Text = "Min Off Time(s):";
			this.lblMinOff.Location = new Point(12, 239);
			this.lblMinOff.AutoSize = true;
			//
			// numMinOff
			//
			this.numMinOff.Location = new Point(140, 236);
			this.numMinOff.Width = 80;
			this.numMinOff.Minimum = 0;
			this.numMinOff.Maximum = 7200;
			this.numMinOff.Value = 300;
			//
			// lblMinOn
			//
			this.lblMinOn.Text = "Min On Time(s):";
			this.lblMinOn.Location = new Point(12, 267);
			this.lblMinOn.AutoSize = true;
			//
			// numMinOn
			//
			this.numMinOn.Location = new Point(140, 264);
			this.numMinOn.Width = 80;
			this.numMinOn.Minimum = 0;
			this.numMinOn.Maximum = 7200;
			this.numMinOn.Value = 300;
			//
			// chkReShedBlock
			//
			this.chkReShedBlock.Text = "Re-Shed Block";
			this.chkReShedBlock.Location = new Point(140, 292);
			this.chkReShedBlock.AutoSize = true;
			//
			// lblBlockTime
			//
			this.lblBlockTime.Text = "Block Time(s):";
			this.lblBlockTime.Location = new Point(12, 319);
			this.lblBlockTime.AutoSize = true;
			//
			// numReShedBlock
			//
			this.numReShedBlock.Location = new Point(140, 316);
			this.numReShedBlock.Width = 80;
			this.numReShedBlock.Minimum = 0;
			this.numReShedBlock.Maximum = 7200;
			this.numReShedBlock.Value = 600;
			//
			// btnOK
			//
			this.btnOK.Text = "OK";
			this.btnOK.Location = new Point(220, 351);
			this.btnOK.Size = new Size(90, 28);
			this.btnOK.Click += new EventHandler(this.BtnOK_Click);
			//
			// btnCancel
			//
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Location = new Point(316, 351);
			this.btnCancel.Size = new Size(90, 28);
			this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
			//
			// FormEditLoadModel
			//
			this.Text = "Edit Load";
			this.ClientSize = new Size(420, 391);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.AcceptButton = this.btnOK;
			this.CancelButton = this.btnCancel;
			this.Controls.AddRange(new Control[] {
				this.lblLoadId, this.txtLoadId,
				this.lblDisplayName, this.txtDisplayName,
				this.lblPriority, this.numPriority,
				this.lblGroup, this.txtGroup,
				this.lblEstimatedKW, this.numEstimatedKW,
				this.lblCommandTag, this.txtCommandTag, this.btnCommandTag,
				this.lblFeedbackTag, this.txtFeedbackTag, this.btnFeedbackTag,
				this.lblInterlockTag, this.txtInterlockTag, this.btnInterlockTag,
				this.lblMinOff, this.numMinOff,
				this.lblMinOn, this.numMinOn,
				this.chkReShedBlock,
				this.lblBlockTime, this.numReShedBlock,
				this.btnOK, this.btnCancel
			});
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private Label lblLoadId;
		private Label lblDisplayName;
		private Label lblPriority;
		private Label lblGroup;
		private Label lblEstimatedKW;
		private Label lblCommandTag;
		private Label lblFeedbackTag;
		private Label lblInterlockTag;
		private Label lblMinOff;
		private Label lblMinOn;
		private Label lblBlockTime;
		private TextBox txtLoadId;
		private TextBox txtDisplayName;
		private TextBox txtGroup;
		private TextBox txtCommandTag;
		private TextBox txtFeedbackTag;
		private TextBox txtInterlockTag;
		private Button btnCommandTag;
		private Button btnFeedbackTag;
		private Button btnInterlockTag;
		private NumericUpDown numPriority;
		private NumericUpDown numEstimatedKW;
		private NumericUpDown numMinOff;
		private NumericUpDown numMinOn;
		private NumericUpDown numReShedBlock;
		private CheckBox chkReShedBlock;
		private Button btnOK;
		private Button btnCancel;
	}
}

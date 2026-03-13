using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExportModBus
{
	/// <summary>
	/// Summary description for FormProtocolOption.
	/// </summary>
	public class FormProtocolOption : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		public	System.Windows.Forms.NumericUpDown numericUpDownStation;
		private System.Windows.Forms.Label label1;
		public  System.Windows.Forms.CheckBox checkBoxReadFloat;
		public  System.Windows.Forms.CheckBox checkBoxWriteFloat;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.CheckBox checkBoxUseHeader;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormProtocolOption()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormProtocolOption));
			this.button1 = new System.Windows.Forms.Button();
			this.numericUpDownStation = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBoxReadFloat = new System.Windows.Forms.CheckBox();
			this.checkBoxWriteFloat = new System.Windows.Forms.CheckBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBoxUseHeader = new System.Windows.Forms.CheckBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStation)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.AccessibleDescription = resources.GetString("button1.AccessibleDescription");
			this.button1.AccessibleName = resources.GetString("button1.AccessibleName");
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button1.Anchor")));
			this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
			this.button1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button1.Dock")));
			this.button1.Enabled = ((bool)(resources.GetObject("button1.Enabled")));
			this.button1.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button1.FlatStyle")));
			this.button1.Font = ((System.Drawing.Font)(resources.GetObject("button1.Font")));
			this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
			this.button1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button1.ImageAlign")));
			this.button1.ImageIndex = ((int)(resources.GetObject("button1.ImageIndex")));
			this.button1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button1.ImeMode")));
			this.button1.Location = ((System.Drawing.Point)(resources.GetObject("button1.Location")));
			this.button1.Name = "button1";
			this.button1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button1.RightToLeft")));
			this.button1.Size = ((System.Drawing.Size)(resources.GetObject("button1.Size")));
			this.button1.TabIndex = ((int)(resources.GetObject("button1.TabIndex")));
			this.button1.Text = resources.GetString("button1.Text");
			this.button1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button1.TextAlign")));
			this.button1.Visible = ((bool)(resources.GetObject("button1.Visible")));
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// numericUpDownStation
			// 
			this.numericUpDownStation.AccessibleDescription = resources.GetString("numericUpDownStation.AccessibleDescription");
			this.numericUpDownStation.AccessibleName = resources.GetString("numericUpDownStation.AccessibleName");
			this.numericUpDownStation.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("numericUpDownStation.Anchor")));
			this.numericUpDownStation.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("numericUpDownStation.Dock")));
			this.numericUpDownStation.Enabled = ((bool)(resources.GetObject("numericUpDownStation.Enabled")));
			this.numericUpDownStation.Font = ((System.Drawing.Font)(resources.GetObject("numericUpDownStation.Font")));
			this.numericUpDownStation.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("numericUpDownStation.ImeMode")));
			this.numericUpDownStation.Location = ((System.Drawing.Point)(resources.GetObject("numericUpDownStation.Location")));
			this.numericUpDownStation.Maximum = new System.Decimal(new int[] {
																				 255,
																				 0,
																				 0,
																				 0});
			this.numericUpDownStation.Name = "numericUpDownStation";
			this.numericUpDownStation.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("numericUpDownStation.RightToLeft")));
			this.numericUpDownStation.Size = ((System.Drawing.Size)(resources.GetObject("numericUpDownStation.Size")));
			this.numericUpDownStation.TabIndex = ((int)(resources.GetObject("numericUpDownStation.TabIndex")));
			this.numericUpDownStation.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("numericUpDownStation.TextAlign")));
			this.numericUpDownStation.ThousandsSeparator = ((bool)(resources.GetObject("numericUpDownStation.ThousandsSeparator")));
			this.numericUpDownStation.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("numericUpDownStation.UpDownAlign")));
			this.numericUpDownStation.Visible = ((bool)(resources.GetObject("numericUpDownStation.Visible")));
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// checkBoxReadFloat
			// 
			this.checkBoxReadFloat.AccessibleDescription = resources.GetString("checkBoxReadFloat.AccessibleDescription");
			this.checkBoxReadFloat.AccessibleName = resources.GetString("checkBoxReadFloat.AccessibleName");
			this.checkBoxReadFloat.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("checkBoxReadFloat.Anchor")));
			this.checkBoxReadFloat.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("checkBoxReadFloat.Appearance")));
			this.checkBoxReadFloat.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBoxReadFloat.BackgroundImage")));
			this.checkBoxReadFloat.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxReadFloat.CheckAlign")));
			this.checkBoxReadFloat.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("checkBoxReadFloat.Dock")));
			this.checkBoxReadFloat.Enabled = ((bool)(resources.GetObject("checkBoxReadFloat.Enabled")));
			this.checkBoxReadFloat.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("checkBoxReadFloat.FlatStyle")));
			this.checkBoxReadFloat.Font = ((System.Drawing.Font)(resources.GetObject("checkBoxReadFloat.Font")));
			this.checkBoxReadFloat.Image = ((System.Drawing.Image)(resources.GetObject("checkBoxReadFloat.Image")));
			this.checkBoxReadFloat.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxReadFloat.ImageAlign")));
			this.checkBoxReadFloat.ImageIndex = ((int)(resources.GetObject("checkBoxReadFloat.ImageIndex")));
			this.checkBoxReadFloat.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("checkBoxReadFloat.ImeMode")));
			this.checkBoxReadFloat.Location = ((System.Drawing.Point)(resources.GetObject("checkBoxReadFloat.Location")));
			this.checkBoxReadFloat.Name = "checkBoxReadFloat";
			this.checkBoxReadFloat.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("checkBoxReadFloat.RightToLeft")));
			this.checkBoxReadFloat.Size = ((System.Drawing.Size)(resources.GetObject("checkBoxReadFloat.Size")));
			this.checkBoxReadFloat.TabIndex = ((int)(resources.GetObject("checkBoxReadFloat.TabIndex")));
			this.checkBoxReadFloat.Text = resources.GetString("checkBoxReadFloat.Text");
			this.checkBoxReadFloat.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxReadFloat.TextAlign")));
			this.checkBoxReadFloat.Visible = ((bool)(resources.GetObject("checkBoxReadFloat.Visible")));
			// 
			// checkBoxWriteFloat
			// 
			this.checkBoxWriteFloat.AccessibleDescription = resources.GetString("checkBoxWriteFloat.AccessibleDescription");
			this.checkBoxWriteFloat.AccessibleName = resources.GetString("checkBoxWriteFloat.AccessibleName");
			this.checkBoxWriteFloat.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("checkBoxWriteFloat.Anchor")));
			this.checkBoxWriteFloat.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("checkBoxWriteFloat.Appearance")));
			this.checkBoxWriteFloat.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBoxWriteFloat.BackgroundImage")));
			this.checkBoxWriteFloat.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxWriteFloat.CheckAlign")));
			this.checkBoxWriteFloat.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("checkBoxWriteFloat.Dock")));
			this.checkBoxWriteFloat.Enabled = ((bool)(resources.GetObject("checkBoxWriteFloat.Enabled")));
			this.checkBoxWriteFloat.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("checkBoxWriteFloat.FlatStyle")));
			this.checkBoxWriteFloat.Font = ((System.Drawing.Font)(resources.GetObject("checkBoxWriteFloat.Font")));
			this.checkBoxWriteFloat.Image = ((System.Drawing.Image)(resources.GetObject("checkBoxWriteFloat.Image")));
			this.checkBoxWriteFloat.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxWriteFloat.ImageAlign")));
			this.checkBoxWriteFloat.ImageIndex = ((int)(resources.GetObject("checkBoxWriteFloat.ImageIndex")));
			this.checkBoxWriteFloat.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("checkBoxWriteFloat.ImeMode")));
			this.checkBoxWriteFloat.Location = ((System.Drawing.Point)(resources.GetObject("checkBoxWriteFloat.Location")));
			this.checkBoxWriteFloat.Name = "checkBoxWriteFloat";
			this.checkBoxWriteFloat.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("checkBoxWriteFloat.RightToLeft")));
			this.checkBoxWriteFloat.Size = ((System.Drawing.Size)(resources.GetObject("checkBoxWriteFloat.Size")));
			this.checkBoxWriteFloat.TabIndex = ((int)(resources.GetObject("checkBoxWriteFloat.TabIndex")));
			this.checkBoxWriteFloat.Text = resources.GetString("checkBoxWriteFloat.Text");
			this.checkBoxWriteFloat.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxWriteFloat.TextAlign")));
			this.checkBoxWriteFloat.Visible = ((bool)(resources.GetObject("checkBoxWriteFloat.Visible")));
			// 
			// buttonCancel
			// 
			this.buttonCancel.AccessibleDescription = resources.GetString("buttonCancel.AccessibleDescription");
			this.buttonCancel.AccessibleName = resources.GetString("buttonCancel.AccessibleName");
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonCancel.Anchor")));
			this.buttonCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonCancel.BackgroundImage")));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonCancel.Dock")));
			this.buttonCancel.Enabled = ((bool)(resources.GetObject("buttonCancel.Enabled")));
			this.buttonCancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonCancel.FlatStyle")));
			this.buttonCancel.Font = ((System.Drawing.Font)(resources.GetObject("buttonCancel.Font")));
			this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
			this.buttonCancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonCancel.ImageAlign")));
			this.buttonCancel.ImageIndex = ((int)(resources.GetObject("buttonCancel.ImageIndex")));
			this.buttonCancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonCancel.ImeMode")));
			this.buttonCancel.Location = ((System.Drawing.Point)(resources.GetObject("buttonCancel.Location")));
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonCancel.RightToLeft")));
			this.buttonCancel.Size = ((System.Drawing.Size)(resources.GetObject("buttonCancel.Size")));
			this.buttonCancel.TabIndex = ((int)(resources.GetObject("buttonCancel.TabIndex")));
			this.buttonCancel.Text = resources.GetString("buttonCancel.Text");
			this.buttonCancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonCancel.TextAlign")));
			this.buttonCancel.Visible = ((bool)(resources.GetObject("buttonCancel.Visible")));
			// 
			// groupBox1
			// 
			this.groupBox1.AccessibleDescription = resources.GetString("groupBox1.AccessibleDescription");
			this.groupBox1.AccessibleName = resources.GetString("groupBox1.AccessibleName");
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox1.Anchor")));
			this.groupBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox1.BackgroundImage")));
			this.groupBox1.Controls.Add(this.checkBoxUseHeader);
			this.groupBox1.Controls.Add(this.numericUpDownStation);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.checkBoxReadFloat);
			this.groupBox1.Controls.Add(this.checkBoxWriteFloat);
			this.groupBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox1.Dock")));
			this.groupBox1.Enabled = ((bool)(resources.GetObject("groupBox1.Enabled")));
			this.groupBox1.Font = ((System.Drawing.Font)(resources.GetObject("groupBox1.Font")));
			this.groupBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox1.ImeMode")));
			this.groupBox1.Location = ((System.Drawing.Point)(resources.GetObject("groupBox1.Location")));
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox1.RightToLeft")));
			this.groupBox1.Size = ((System.Drawing.Size)(resources.GetObject("groupBox1.Size")));
			this.groupBox1.TabIndex = ((int)(resources.GetObject("groupBox1.TabIndex")));
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = resources.GetString("groupBox1.Text");
			this.groupBox1.Visible = ((bool)(resources.GetObject("groupBox1.Visible")));
			// 
			// checkBoxUseHeader
			// 
			this.checkBoxUseHeader.AccessibleDescription = resources.GetString("checkBoxUseHeader.AccessibleDescription");
			this.checkBoxUseHeader.AccessibleName = resources.GetString("checkBoxUseHeader.AccessibleName");
			this.checkBoxUseHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("checkBoxUseHeader.Anchor")));
			this.checkBoxUseHeader.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("checkBoxUseHeader.Appearance")));
			this.checkBoxUseHeader.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBoxUseHeader.BackgroundImage")));
			this.checkBoxUseHeader.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxUseHeader.CheckAlign")));
			this.checkBoxUseHeader.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("checkBoxUseHeader.Dock")));
			this.checkBoxUseHeader.Enabled = ((bool)(resources.GetObject("checkBoxUseHeader.Enabled")));
			this.checkBoxUseHeader.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("checkBoxUseHeader.FlatStyle")));
			this.checkBoxUseHeader.Font = ((System.Drawing.Font)(resources.GetObject("checkBoxUseHeader.Font")));
			this.checkBoxUseHeader.Image = ((System.Drawing.Image)(resources.GetObject("checkBoxUseHeader.Image")));
			this.checkBoxUseHeader.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxUseHeader.ImageAlign")));
			this.checkBoxUseHeader.ImageIndex = ((int)(resources.GetObject("checkBoxUseHeader.ImageIndex")));
			this.checkBoxUseHeader.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("checkBoxUseHeader.ImeMode")));
			this.checkBoxUseHeader.Location = ((System.Drawing.Point)(resources.GetObject("checkBoxUseHeader.Location")));
			this.checkBoxUseHeader.Name = "checkBoxUseHeader";
			this.checkBoxUseHeader.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("checkBoxUseHeader.RightToLeft")));
			this.checkBoxUseHeader.Size = ((System.Drawing.Size)(resources.GetObject("checkBoxUseHeader.Size")));
			this.checkBoxUseHeader.TabIndex = ((int)(resources.GetObject("checkBoxUseHeader.TabIndex")));
			this.checkBoxUseHeader.Text = resources.GetString("checkBoxUseHeader.Text");
			this.checkBoxUseHeader.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxUseHeader.TextAlign")));
			this.checkBoxUseHeader.Visible = ((bool)(resources.GetObject("checkBoxUseHeader.Visible")));
			// 
			// tabControl1
			// 
			this.tabControl1.AccessibleDescription = resources.GetString("tabControl1.AccessibleDescription");
			this.tabControl1.AccessibleName = resources.GetString("tabControl1.AccessibleName");
			this.tabControl1.Alignment = ((System.Windows.Forms.TabAlignment)(resources.GetObject("tabControl1.Alignment")));
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabControl1.Anchor")));
			this.tabControl1.Appearance = ((System.Windows.Forms.TabAppearance)(resources.GetObject("tabControl1.Appearance")));
			this.tabControl1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabControl1.BackgroundImage")));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabControl1.Dock")));
			this.tabControl1.Enabled = ((bool)(resources.GetObject("tabControl1.Enabled")));
			this.tabControl1.Font = ((System.Drawing.Font)(resources.GetObject("tabControl1.Font")));
			this.tabControl1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabControl1.ImeMode")));
			this.tabControl1.ItemSize = ((System.Drawing.Size)(resources.GetObject("tabControl1.ItemSize")));
			this.tabControl1.Location = ((System.Drawing.Point)(resources.GetObject("tabControl1.Location")));
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.Padding = ((System.Drawing.Point)(resources.GetObject("tabControl1.Padding")));
			this.tabControl1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabControl1.RightToLeft")));
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.ShowToolTips = ((bool)(resources.GetObject("tabControl1.ShowToolTips")));
			this.tabControl1.Size = ((System.Drawing.Size)(resources.GetObject("tabControl1.Size")));
			this.tabControl1.TabIndex = ((int)(resources.GetObject("tabControl1.TabIndex")));
			this.tabControl1.Text = resources.GetString("tabControl1.Text");
			this.tabControl1.Visible = ((bool)(resources.GetObject("tabControl1.Visible")));
			// 
			// tabPage1
			// 
			this.tabPage1.AccessibleDescription = resources.GetString("tabPage1.AccessibleDescription");
			this.tabPage1.AccessibleName = resources.GetString("tabPage1.AccessibleName");
			this.tabPage1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPage1.Anchor")));
			this.tabPage1.AutoScroll = ((bool)(resources.GetObject("tabPage1.AutoScroll")));
			this.tabPage1.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPage1.AutoScrollMargin")));
			this.tabPage1.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPage1.AutoScrollMinSize")));
			this.tabPage1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage1.BackgroundImage")));
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPage1.Dock")));
			this.tabPage1.Enabled = ((bool)(resources.GetObject("tabPage1.Enabled")));
			this.tabPage1.Font = ((System.Drawing.Font)(resources.GetObject("tabPage1.Font")));
			this.tabPage1.ImageIndex = ((int)(resources.GetObject("tabPage1.ImageIndex")));
			this.tabPage1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPage1.ImeMode")));
			this.tabPage1.Location = ((System.Drawing.Point)(resources.GetObject("tabPage1.Location")));
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPage1.RightToLeft")));
			this.tabPage1.Size = ((System.Drawing.Size)(resources.GetObject("tabPage1.Size")));
			this.tabPage1.TabIndex = ((int)(resources.GetObject("tabPage1.TabIndex")));
			this.tabPage1.Text = resources.GetString("tabPage1.Text");
			this.tabPage1.ToolTipText = resources.GetString("tabPage1.ToolTipText");
			this.tabPage1.Visible = ((bool)(resources.GetObject("tabPage1.Visible")));
			// 
			// tabPage2
			// 
			this.tabPage2.AccessibleDescription = resources.GetString("tabPage2.AccessibleDescription");
			this.tabPage2.AccessibleName = resources.GetString("tabPage2.AccessibleName");
			this.tabPage2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPage2.Anchor")));
			this.tabPage2.AutoScroll = ((bool)(resources.GetObject("tabPage2.AutoScroll")));
			this.tabPage2.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPage2.AutoScrollMargin")));
			this.tabPage2.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPage2.AutoScrollMinSize")));
			this.tabPage2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage2.BackgroundImage")));
			this.tabPage2.Controls.Add(this.label7);
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Controls.Add(this.label4);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPage2.Dock")));
			this.tabPage2.Enabled = ((bool)(resources.GetObject("tabPage2.Enabled")));
			this.tabPage2.Font = ((System.Drawing.Font)(resources.GetObject("tabPage2.Font")));
			this.tabPage2.ImageIndex = ((int)(resources.GetObject("tabPage2.ImageIndex")));
			this.tabPage2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPage2.ImeMode")));
			this.tabPage2.Location = ((System.Drawing.Point)(resources.GetObject("tabPage2.Location")));
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPage2.RightToLeft")));
			this.tabPage2.Size = ((System.Drawing.Size)(resources.GetObject("tabPage2.Size")));
			this.tabPage2.TabIndex = ((int)(resources.GetObject("tabPage2.TabIndex")));
			this.tabPage2.Text = resources.GetString("tabPage2.Text");
			this.tabPage2.ToolTipText = resources.GetString("tabPage2.ToolTipText");
			this.tabPage2.Visible = ((bool)(resources.GetObject("tabPage2.Visible")));
			// 
			// label2
			// 
			this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
			this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
			this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
			this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
			this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
			this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
			this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
			this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
			this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
			this.label2.Name = "label2";
			this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
			this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
			this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
			this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
			// 
			// label3
			// 
			this.label3.AccessibleDescription = resources.GetString("label3.AccessibleDescription");
			this.label3.AccessibleName = resources.GetString("label3.AccessibleName");
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
			this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
			this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
			this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
			this.label3.Font = ((System.Drawing.Font)(resources.GetObject("label3.Font")));
			this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
			this.label3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.ImageAlign")));
			this.label3.ImageIndex = ((int)(resources.GetObject("label3.ImageIndex")));
			this.label3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label3.ImeMode")));
			this.label3.Location = ((System.Drawing.Point)(resources.GetObject("label3.Location")));
			this.label3.Name = "label3";
			this.label3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label3.RightToLeft")));
			this.label3.Size = ((System.Drawing.Size)(resources.GetObject("label3.Size")));
			this.label3.TabIndex = ((int)(resources.GetObject("label3.TabIndex")));
			this.label3.Text = resources.GetString("label3.Text");
			this.label3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.TextAlign")));
			this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
			this.label3.Click += new System.EventHandler(this.label3_Click);
			// 
			// label4
			// 
			this.label4.AccessibleDescription = resources.GetString("label4.AccessibleDescription");
			this.label4.AccessibleName = resources.GetString("label4.AccessibleName");
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label4.Anchor")));
			this.label4.AutoSize = ((bool)(resources.GetObject("label4.AutoSize")));
			this.label4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label4.Dock")));
			this.label4.Enabled = ((bool)(resources.GetObject("label4.Enabled")));
			this.label4.Font = ((System.Drawing.Font)(resources.GetObject("label4.Font")));
			this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
			this.label4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.ImageAlign")));
			this.label4.ImageIndex = ((int)(resources.GetObject("label4.ImageIndex")));
			this.label4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label4.ImeMode")));
			this.label4.Location = ((System.Drawing.Point)(resources.GetObject("label4.Location")));
			this.label4.Name = "label4";
			this.label4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label4.RightToLeft")));
			this.label4.Size = ((System.Drawing.Size)(resources.GetObject("label4.Size")));
			this.label4.TabIndex = ((int)(resources.GetObject("label4.TabIndex")));
			this.label4.Text = resources.GetString("label4.Text");
			this.label4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.TextAlign")));
			this.label4.Visible = ((bool)(resources.GetObject("label4.Visible")));
			// 
			// label5
			// 
			this.label5.AccessibleDescription = resources.GetString("label5.AccessibleDescription");
			this.label5.AccessibleName = resources.GetString("label5.AccessibleName");
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label5.Anchor")));
			this.label5.AutoSize = ((bool)(resources.GetObject("label5.AutoSize")));
			this.label5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label5.Dock")));
			this.label5.Enabled = ((bool)(resources.GetObject("label5.Enabled")));
			this.label5.Font = ((System.Drawing.Font)(resources.GetObject("label5.Font")));
			this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
			this.label5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.ImageAlign")));
			this.label5.ImageIndex = ((int)(resources.GetObject("label5.ImageIndex")));
			this.label5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label5.ImeMode")));
			this.label5.Location = ((System.Drawing.Point)(resources.GetObject("label5.Location")));
			this.label5.Name = "label5";
			this.label5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label5.RightToLeft")));
			this.label5.Size = ((System.Drawing.Size)(resources.GetObject("label5.Size")));
			this.label5.TabIndex = ((int)(resources.GetObject("label5.TabIndex")));
			this.label5.Text = resources.GetString("label5.Text");
			this.label5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.TextAlign")));
			this.label5.Visible = ((bool)(resources.GetObject("label5.Visible")));
			// 
			// label6
			// 
			this.label6.AccessibleDescription = resources.GetString("label6.AccessibleDescription");
			this.label6.AccessibleName = resources.GetString("label6.AccessibleName");
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label6.Anchor")));
			this.label6.AutoSize = ((bool)(resources.GetObject("label6.AutoSize")));
			this.label6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label6.Dock")));
			this.label6.Enabled = ((bool)(resources.GetObject("label6.Enabled")));
			this.label6.Font = ((System.Drawing.Font)(resources.GetObject("label6.Font")));
			this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
			this.label6.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label6.ImageAlign")));
			this.label6.ImageIndex = ((int)(resources.GetObject("label6.ImageIndex")));
			this.label6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label6.ImeMode")));
			this.label6.Location = ((System.Drawing.Point)(resources.GetObject("label6.Location")));
			this.label6.Name = "label6";
			this.label6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label6.RightToLeft")));
			this.label6.Size = ((System.Drawing.Size)(resources.GetObject("label6.Size")));
			this.label6.TabIndex = ((int)(resources.GetObject("label6.TabIndex")));
			this.label6.Text = resources.GetString("label6.Text");
			this.label6.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label6.TextAlign")));
			this.label6.Visible = ((bool)(resources.GetObject("label6.Visible")));
			// 
			// label7
			// 
			this.label7.AccessibleDescription = resources.GetString("label7.AccessibleDescription");
			this.label7.AccessibleName = resources.GetString("label7.AccessibleName");
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label7.Anchor")));
			this.label7.AutoSize = ((bool)(resources.GetObject("label7.AutoSize")));
			this.label7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label7.Dock")));
			this.label7.Enabled = ((bool)(resources.GetObject("label7.Enabled")));
			this.label7.Font = ((System.Drawing.Font)(resources.GetObject("label7.Font")));
			this.label7.Image = ((System.Drawing.Image)(resources.GetObject("label7.Image")));
			this.label7.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label7.ImageAlign")));
			this.label7.ImageIndex = ((int)(resources.GetObject("label7.ImageIndex")));
			this.label7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label7.ImeMode")));
			this.label7.Location = ((System.Drawing.Point)(resources.GetObject("label7.Location")));
			this.label7.Name = "label7";
			this.label7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label7.RightToLeft")));
			this.label7.Size = ((System.Drawing.Size)(resources.GetObject("label7.Size")));
			this.label7.TabIndex = ((int)(resources.GetObject("label7.TabIndex")));
			this.label7.Text = resources.GetString("label7.Text");
			this.label7.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label7.TextAlign")));
			this.label7.Visible = ((bool)(resources.GetObject("label7.Visible")));
			// 
			// FormProtocolOption
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.button1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "FormProtocolOption";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.ShowInTaskbar = false;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStation)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{			
			DialogResult = DialogResult.OK;			
			Close();
		}

		private void label3_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}

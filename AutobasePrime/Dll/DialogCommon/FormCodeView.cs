using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DialogCommon
{
	/// <summary>
	/// Summary description for FormCodeView.
	/// </summary>
	public class FormCodeView : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panelSend;
		private System.Windows.Forms.Panel panelRecv;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioButtonHex;
		private System.Windows.Forms.RadioButton radioButtonAscii;
		private System.Windows.Forms.RadioButton radioButtonDecimal;
		private System.Windows.Forms.CheckBox checkBoxPause;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormCodeView()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCodeView));
            this.panelSend = new System.Windows.Forms.Panel();
            this.panelRecv = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxPause = new System.Windows.Forms.CheckBox();
            this.radioButtonDecimal = new System.Windows.Forms.RadioButton();
            this.radioButtonAscii = new System.Windows.Forms.RadioButton();
            this.radioButtonHex = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSend
            // 
            this.panelSend.AccessibleDescription = null;
            this.panelSend.AccessibleName = null;
            resources.ApplyResources(this.panelSend, "panelSend");
            this.panelSend.BackgroundImage = null;
            this.panelSend.Font = null;
            this.panelSend.Name = "panelSend";
            // 
            // panelRecv
            // 
            this.panelRecv.AccessibleDescription = null;
            this.panelRecv.AccessibleName = null;
            resources.ApplyResources(this.panelRecv, "panelRecv");
            this.panelRecv.BackgroundImage = null;
            this.panelRecv.Font = null;
            this.panelRecv.Name = "panelRecv";
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.checkBoxPause);
            this.panel1.Controls.Add(this.radioButtonDecimal);
            this.panel1.Controls.Add(this.radioButtonAscii);
            this.panel1.Controls.Add(this.radioButtonHex);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // checkBoxPause
            // 
            this.checkBoxPause.AccessibleDescription = null;
            this.checkBoxPause.AccessibleName = null;
            resources.ApplyResources(this.checkBoxPause, "checkBoxPause");
            this.checkBoxPause.BackgroundImage = null;
            this.checkBoxPause.Font = null;
            this.checkBoxPause.Name = "checkBoxPause";
            this.checkBoxPause.CheckedChanged += new System.EventHandler(this.checkBoxPause_CheckedChanged);
            // 
            // radioButtonDecimal
            // 
            this.radioButtonDecimal.AccessibleDescription = null;
            this.radioButtonDecimal.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDecimal, "radioButtonDecimal");
            this.radioButtonDecimal.BackgroundImage = null;
            this.radioButtonDecimal.Font = null;
            this.radioButtonDecimal.Name = "radioButtonDecimal";
            this.radioButtonDecimal.CheckedChanged += new System.EventHandler(this.radioButtonDecimal_CheckedChanged);
            // 
            // radioButtonAscii
            // 
            this.radioButtonAscii.AccessibleDescription = null;
            this.radioButtonAscii.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAscii, "radioButtonAscii");
            this.radioButtonAscii.BackgroundImage = null;
            this.radioButtonAscii.Font = null;
            this.radioButtonAscii.Name = "radioButtonAscii";
            this.radioButtonAscii.CheckedChanged += new System.EventHandler(this.radioButtonAscii_CheckedChanged);
            // 
            // radioButtonHex
            // 
            this.radioButtonHex.AccessibleDescription = null;
            this.radioButtonHex.AccessibleName = null;
            resources.ApplyResources(this.radioButtonHex, "radioButtonHex");
            this.radioButtonHex.BackgroundImage = null;
            this.radioButtonHex.Font = null;
            this.radioButtonHex.Name = "radioButtonHex";
            this.radioButtonHex.CheckedChanged += new System.EventHandler(this.radioButtonHex_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Controls.Add(this.panelSend);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // splitter1
            // 
            this.splitter1.AccessibleDescription = null;
            this.splitter1.AccessibleName = null;
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.BackgroundImage = null;
            this.splitter1.Font = null;
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.AccessibleDescription = null;
            this.panel3.AccessibleName = null;
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.BackgroundImage = null;
            this.panel3.Controls.Add(this.panelRecv);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Font = null;
            this.panel3.Name = "panel3";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // FormCodeView
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = null;
            this.Name = "FormCodeView";
            this.Load += new System.EventHandler(this.FormCodeView_Load);
            this.Closed += new System.EventHandler(this.FormCodeView_Closed);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public FormCommunicationCode wndSend;
		public FormCommunicationCode wndRecv;

		private void FormCodeView_Load(object sender, System.EventArgs e)
		{
			wndSend = new FormCommunicationCode();
			wndSend.FormBorderStyle = FormBorderStyle.None;
			wndSend.TopLevel = false;
			this.panelSend.Controls.Add(wndSend);
			wndSend.Dock = DockStyle.Fill;
			wndSend.Show();

			wndRecv = new FormCommunicationCode();
			wndRecv.TopLevel = false;
			wndRecv.FormBorderStyle = FormBorderStyle.None;
			this.panelRecv.Controls.Add(wndRecv);
			wndRecv.Dock = DockStyle.Fill;
			wndRecv.Show();

			thisForm = this;

			this.radioButtonHex.Checked = wndSend.cDisplayMethod == 0;
			this.radioButtonAscii.Checked = wndSend.cDisplayMethod == 1;
			this.radioButtonDecimal.Checked = wndSend.cDisplayMethod == 2;
			this.checkBoxPause.Checked = wndSend.bPause;
		}

		static FormCodeView thisForm = null;

		private void FormCodeView_Closed(object sender, System.EventArgs e)
		{
			thisForm = null;
		}

		public static void DisplaySendCode(byte[] codes, int length)
		{
			if(thisForm == null)	return;	
			thisForm.wndSend.DisplayCode(codes, length);
		}

		public static void DisplaySendCode(string codes, int length)
		{
			if(thisForm == null)	return;	
			thisForm.wndSend.DisplayCode(codes, length);
		}

		public static void DisplaySendCode(int code)
		{
			if(thisForm == null)	return;	
			thisForm.wndSend.DisplayOneChar(code);
		}

		public static void DisplaySendNextLine()
		{
			if(thisForm == null)	return;	
			thisForm.wndSend.DisplayNextLine();
		}

		public static void DisplayRecvCode(byte[] codes, int length)
		{
			if(thisForm == null)	return;	
			thisForm.wndRecv.DisplayCode(codes, length);
		}

		public static void DisplayRecvCode(string codes, int length)
		{
			if(thisForm == null)	return;	
			thisForm.wndRecv.DisplayCode(codes, length);
		}

		public static void DisplayRecvCode(int code)
		{
			if(thisForm == null)	return;	
			thisForm.wndRecv.DisplayOneChar(code);
		}

		public static void DisplayRecvNextLine()
		{
			if(thisForm == null)	return;	
			thisForm.wndRecv.DisplayNextLine();
		}

		void SetDisplayMethod(sbyte pos)
		{
			this.wndSend.cDisplayMethod = pos;
			this.wndRecv.cDisplayMethod = pos;
		}

		private void radioButtonHex_CheckedChanged(object sender, System.EventArgs e)
		{
			SetDisplayMethod(0);
		}

		private void radioButtonAscii_CheckedChanged(object sender, System.EventArgs e)
		{
			SetDisplayMethod(1);
		}

		private void radioButtonDecimal_CheckedChanged(object sender, System.EventArgs e)
		{
			SetDisplayMethod(2);
		}

		private void checkBoxPause_CheckedChanged(object sender, System.EventArgs e)
		{
			this.wndSend.bPause = this.checkBoxPause.Checked; 
			this.wndRecv.bPause = this.checkBoxPause.Checked;
		}
        
		/*
		public static void DisplaySendCode(string ip, byte[] codes, int length)
		{
			if(thisForm == null)	return;

			thisForm.wndSend.DisplayCode(ip, ip.Length);
			thisForm.wndSend.DisplayCode(codes, length);
			thisForm.wndSend.DisplayNextLine();
			
		}

		public static void DisplayRecvCode(string ip, byte[] codes, int length)
		{
			if(thisForm == null)	return;	

			thisForm.wndRecv.DisplayCode(ip, ip.Length);
			thisForm.wndRecv.DisplayCode(codes, length);
			thisForm.wndRecv.DisplayNextLine();
		}
		*/
	}
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace LinePrinter
{
	/// <summary>
	/// Summary description for FormConfigEscCommand.
	/// </summary>
	public class FormConfigEscCommand : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxActive;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button buttonDefault;
		private System.Windows.Forms.TextBox textBoxCommand0;
		private System.Windows.Forms.TextBox textBoxCommand1;
		private System.Windows.Forms.TextBox textBoxCommand2;
		private System.Windows.Forms.TextBox textBoxCommand3;
		private System.Windows.Forms.TextBox textBoxCommand4;
		private System.Windows.Forms.TextBox textBoxCommand5;
		private System.Windows.Forms.TextBox textBoxCommand6;
		private System.Windows.Forms.TextBox textBoxCommand7;
		private System.Windows.Forms.TextBox textBoxCommand8;
		private System.Windows.Forms.TextBox textBoxCommand9;
		private System.Windows.Forms.TextBox textBoxCommand10;
		private System.Windows.Forms.Label label1Command0;
		private System.Windows.Forms.Label label1Command1;
		private System.Windows.Forms.Label label1Command2;
		private System.Windows.Forms.Label label1Command3;
		private System.Windows.Forms.Label label1Command4;
		private System.Windows.Forms.Label label1Command5;
		private System.Windows.Forms.Label label1Command6;
		private System.Windows.Forms.Label label1Command7;
		private System.Windows.Forms.Label label1Command8;
		private System.Windows.Forms.Label label1Command9;
		private System.Windows.Forms.Label label1Command10;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigEscCommand()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigEscCommand));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1Command10 = new System.Windows.Forms.Label();
            this.label1Command9 = new System.Windows.Forms.Label();
            this.label1Command8 = new System.Windows.Forms.Label();
            this.label1Command7 = new System.Windows.Forms.Label();
            this.label1Command6 = new System.Windows.Forms.Label();
            this.label1Command5 = new System.Windows.Forms.Label();
            this.label1Command4 = new System.Windows.Forms.Label();
            this.label1Command3 = new System.Windows.Forms.Label();
            this.label1Command2 = new System.Windows.Forms.Label();
            this.label1Command1 = new System.Windows.Forms.Label();
            this.label1Command0 = new System.Windows.Forms.Label();
            this.textBoxCommand10 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxCommand9 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxCommand8 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxCommand7 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxCommand6 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCommand5 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxCommand4 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxCommand3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxCommand2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCommand1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCommand0 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // checkBoxActive
            // 
            this.checkBoxActive.AccessibleDescription = null;
            this.checkBoxActive.AccessibleName = null;
            resources.ApplyResources(this.checkBoxActive, "checkBoxActive");
            this.checkBoxActive.BackgroundImage = null;
            this.checkBoxActive.Font = null;
            this.checkBoxActive.Name = "checkBoxActive";
            this.checkBoxActive.CheckedChanged += new System.EventHandler(this.checkBoxActive_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label1Command10);
            this.groupBox1.Controls.Add(this.label1Command9);
            this.groupBox1.Controls.Add(this.label1Command8);
            this.groupBox1.Controls.Add(this.label1Command7);
            this.groupBox1.Controls.Add(this.label1Command6);
            this.groupBox1.Controls.Add(this.label1Command5);
            this.groupBox1.Controls.Add(this.label1Command4);
            this.groupBox1.Controls.Add(this.label1Command3);
            this.groupBox1.Controls.Add(this.label1Command2);
            this.groupBox1.Controls.Add(this.label1Command1);
            this.groupBox1.Controls.Add(this.label1Command0);
            this.groupBox1.Controls.Add(this.textBoxCommand10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBoxCommand9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.textBoxCommand8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBoxCommand7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBoxCommand6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxCommand5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBoxCommand4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxCommand3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxCommand2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxCommand1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxCommand0);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1Command10
            // 
            this.label1Command10.AccessibleDescription = null;
            this.label1Command10.AccessibleName = null;
            resources.ApplyResources(this.label1Command10, "label1Command10");
            this.label1Command10.Font = null;
            this.label1Command10.Name = "label1Command10";
            // 
            // label1Command9
            // 
            this.label1Command9.AccessibleDescription = null;
            this.label1Command9.AccessibleName = null;
            resources.ApplyResources(this.label1Command9, "label1Command9");
            this.label1Command9.Font = null;
            this.label1Command9.Name = "label1Command9";
            // 
            // label1Command8
            // 
            this.label1Command8.AccessibleDescription = null;
            this.label1Command8.AccessibleName = null;
            resources.ApplyResources(this.label1Command8, "label1Command8");
            this.label1Command8.Font = null;
            this.label1Command8.Name = "label1Command8";
            // 
            // label1Command7
            // 
            this.label1Command7.AccessibleDescription = null;
            this.label1Command7.AccessibleName = null;
            resources.ApplyResources(this.label1Command7, "label1Command7");
            this.label1Command7.Font = null;
            this.label1Command7.Name = "label1Command7";
            // 
            // label1Command6
            // 
            this.label1Command6.AccessibleDescription = null;
            this.label1Command6.AccessibleName = null;
            resources.ApplyResources(this.label1Command6, "label1Command6");
            this.label1Command6.Font = null;
            this.label1Command6.Name = "label1Command6";
            // 
            // label1Command5
            // 
            this.label1Command5.AccessibleDescription = null;
            this.label1Command5.AccessibleName = null;
            resources.ApplyResources(this.label1Command5, "label1Command5");
            this.label1Command5.Font = null;
            this.label1Command5.Name = "label1Command5";
            // 
            // label1Command4
            // 
            this.label1Command4.AccessibleDescription = null;
            this.label1Command4.AccessibleName = null;
            resources.ApplyResources(this.label1Command4, "label1Command4");
            this.label1Command4.Font = null;
            this.label1Command4.Name = "label1Command4";
            // 
            // label1Command3
            // 
            this.label1Command3.AccessibleDescription = null;
            this.label1Command3.AccessibleName = null;
            resources.ApplyResources(this.label1Command3, "label1Command3");
            this.label1Command3.Font = null;
            this.label1Command3.Name = "label1Command3";
            // 
            // label1Command2
            // 
            this.label1Command2.AccessibleDescription = null;
            this.label1Command2.AccessibleName = null;
            resources.ApplyResources(this.label1Command2, "label1Command2");
            this.label1Command2.Font = null;
            this.label1Command2.Name = "label1Command2";
            // 
            // label1Command1
            // 
            this.label1Command1.AccessibleDescription = null;
            this.label1Command1.AccessibleName = null;
            resources.ApplyResources(this.label1Command1, "label1Command1");
            this.label1Command1.Font = null;
            this.label1Command1.Name = "label1Command1";
            // 
            // label1Command0
            // 
            this.label1Command0.AccessibleDescription = null;
            this.label1Command0.AccessibleName = null;
            resources.ApplyResources(this.label1Command0, "label1Command0");
            this.label1Command0.Font = null;
            this.label1Command0.Name = "label1Command0";
            // 
            // textBoxCommand10
            // 
            this.textBoxCommand10.AccessibleDescription = null;
            this.textBoxCommand10.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand10, "textBoxCommand10");
            this.textBoxCommand10.BackgroundImage = null;
            this.textBoxCommand10.Font = null;
            this.textBoxCommand10.Name = "textBoxCommand10";
            this.textBoxCommand10.TextChanged += new System.EventHandler(this.textBoxCommand10_TextChanged);
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // textBoxCommand9
            // 
            this.textBoxCommand9.AccessibleDescription = null;
            this.textBoxCommand9.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand9, "textBoxCommand9");
            this.textBoxCommand9.BackgroundImage = null;
            this.textBoxCommand9.Font = null;
            this.textBoxCommand9.Name = "textBoxCommand9";
            this.textBoxCommand9.TextChanged += new System.EventHandler(this.textBoxCommand9_TextChanged);
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Font = null;
            this.label10.Name = "label10";
            // 
            // textBoxCommand8
            // 
            this.textBoxCommand8.AccessibleDescription = null;
            this.textBoxCommand8.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand8, "textBoxCommand8");
            this.textBoxCommand8.BackgroundImage = null;
            this.textBoxCommand8.Font = null;
            this.textBoxCommand8.Name = "textBoxCommand8";
            this.textBoxCommand8.TextChanged += new System.EventHandler(this.textBoxCommand8_TextChanged);
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
            // 
            // textBoxCommand7
            // 
            this.textBoxCommand7.AccessibleDescription = null;
            this.textBoxCommand7.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand7, "textBoxCommand7");
            this.textBoxCommand7.BackgroundImage = null;
            this.textBoxCommand7.Font = null;
            this.textBoxCommand7.Name = "textBoxCommand7";
            this.textBoxCommand7.TextChanged += new System.EventHandler(this.textBoxCommand7_TextChanged);
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // textBoxCommand6
            // 
            this.textBoxCommand6.AccessibleDescription = null;
            this.textBoxCommand6.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand6, "textBoxCommand6");
            this.textBoxCommand6.BackgroundImage = null;
            this.textBoxCommand6.Font = null;
            this.textBoxCommand6.Name = "textBoxCommand6";
            this.textBoxCommand6.TextChanged += new System.EventHandler(this.textBoxCommand6_TextChanged);
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // textBoxCommand5
            // 
            this.textBoxCommand5.AccessibleDescription = null;
            this.textBoxCommand5.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand5, "textBoxCommand5");
            this.textBoxCommand5.BackgroundImage = null;
            this.textBoxCommand5.Font = null;
            this.textBoxCommand5.Name = "textBoxCommand5";
            this.textBoxCommand5.TextChanged += new System.EventHandler(this.textBoxCommand5_TextChanged);
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // textBoxCommand4
            // 
            this.textBoxCommand4.AccessibleDescription = null;
            this.textBoxCommand4.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand4, "textBoxCommand4");
            this.textBoxCommand4.BackgroundImage = null;
            this.textBoxCommand4.Font = null;
            this.textBoxCommand4.Name = "textBoxCommand4";
            this.textBoxCommand4.TextChanged += new System.EventHandler(this.textBoxCommand4_TextChanged);
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // textBoxCommand3
            // 
            this.textBoxCommand3.AccessibleDescription = null;
            this.textBoxCommand3.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand3, "textBoxCommand3");
            this.textBoxCommand3.BackgroundImage = null;
            this.textBoxCommand3.Font = null;
            this.textBoxCommand3.Name = "textBoxCommand3";
            this.textBoxCommand3.TextChanged += new System.EventHandler(this.textBoxCommand3_TextChanged);
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // textBoxCommand2
            // 
            this.textBoxCommand2.AccessibleDescription = null;
            this.textBoxCommand2.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand2, "textBoxCommand2");
            this.textBoxCommand2.BackgroundImage = null;
            this.textBoxCommand2.Font = null;
            this.textBoxCommand2.Name = "textBoxCommand2";
            this.textBoxCommand2.TextChanged += new System.EventHandler(this.textBoxCommand2_TextChanged);
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // textBoxCommand1
            // 
            this.textBoxCommand1.AccessibleDescription = null;
            this.textBoxCommand1.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand1, "textBoxCommand1");
            this.textBoxCommand1.BackgroundImage = null;
            this.textBoxCommand1.Font = null;
            this.textBoxCommand1.Name = "textBoxCommand1";
            this.textBoxCommand1.TextChanged += new System.EventHandler(this.textBoxCommand1_TextChanged);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // textBoxCommand0
            // 
            this.textBoxCommand0.AccessibleDescription = null;
            this.textBoxCommand0.AccessibleName = null;
            resources.ApplyResources(this.textBoxCommand0, "textBoxCommand0");
            this.textBoxCommand0.BackgroundImage = null;
            this.textBoxCommand0.Font = null;
            this.textBoxCommand0.Name = "textBoxCommand0";
            this.textBoxCommand0.TextChanged += new System.EventHandler(this.textBoxCommand0_TextChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // buttonDefault
            // 
            this.buttonDefault.AccessibleDescription = null;
            this.buttonDefault.AccessibleName = null;
            resources.ApplyResources(this.buttonDefault, "buttonDefault");
            this.buttonDefault.BackgroundImage = null;
            this.buttonDefault.Font = null;
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // FormConfigEscCommand
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonDefault);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxActive);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigEscCommand";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigEscCommand_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		

		void EnableDisable()
		{
			bool flag = this.checkBoxActive.Checked;

			this.textBoxCommand0.Enabled = flag;
			this.textBoxCommand1.Enabled = flag;
			this.textBoxCommand2.Enabled = flag;
			this.textBoxCommand3.Enabled = flag;
			this.textBoxCommand4.Enabled = flag;
			this.textBoxCommand5.Enabled = flag;
			this.textBoxCommand6.Enabled = flag;
			this.textBoxCommand7.Enabled = flag;
			this.textBoxCommand8.Enabled = flag;
			this.textBoxCommand9.Enabled = flag;
			this.textBoxCommand10.Enabled = flag;
		}

		void FillEditBox()
		{
			CommandToString(this.textBoxCommand0, this.label1Command0, Config.esc[0].command);
			CommandToString(this.textBoxCommand1, this.label1Command1, Config.esc[1].command);
			CommandToString(this.textBoxCommand2, this.label1Command2, Config.esc[2].command);
			CommandToString(this.textBoxCommand3, this.label1Command3, Config.esc[3].command);
			CommandToString(this.textBoxCommand4, this.label1Command4, Config.esc[4].command);
			CommandToString(this.textBoxCommand5, this.label1Command5, Config.esc[5].command);
			CommandToString(this.textBoxCommand6, this.label1Command6, Config.esc[6].command);
			CommandToString(this.textBoxCommand7, this.label1Command7, Config.esc[7].command);
			CommandToString(this.textBoxCommand8, this.label1Command8, Config.esc[8].command);
			CommandToString(this.textBoxCommand9, this.label1Command9, Config.esc[9].command);
			CommandToString(this.textBoxCommand10,this.label1Command10,Config.esc[10].command);
		}

		private void FormConfigEscCommand_Load(object sender, System.EventArgs e)
		{
			this.checkBoxActive.Checked = Config.cEscFlag;
	
			FillEditBox();

			EnableDisable();
		}

		private void checkBoxActive_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		void CommandToString(TextBox textbox, Label label, string command)
		{
			int		i, len;
			string 	buf = "";

			len = command.Length;
			for(i = 0; i < len; i++) 
			{
				buf += String.Format("{0}, ", (int)command[i]);
			}

			textbox.Text = buf;
			label.Text = command;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			Config.esc[0].command = EditToBuf(this.textBoxCommand0);
			Config.esc[1].command = EditToBuf(this.textBoxCommand1);
			Config.esc[2].command = EditToBuf(this.textBoxCommand2);
			Config.esc[3].command = EditToBuf(this.textBoxCommand3);
			Config.esc[4].command = EditToBuf(this.textBoxCommand4);
			Config.esc[5].command = EditToBuf(this.textBoxCommand5);
			Config.esc[6].command = EditToBuf(this.textBoxCommand6);
			Config.esc[7].command = EditToBuf(this.textBoxCommand7);
			Config.esc[8].command = EditToBuf(this.textBoxCommand8);
			Config.esc[9].command = EditToBuf(this.textBoxCommand9);
			Config.esc[10].command = EditToBuf(this.textBoxCommand10);

			Config.cEscFlag = this.checkBoxActive.Checked;
			
			Config.Save();

			DialogResult = DialogResult.OK;
			Close();
		}

		string EditToBuf(TextBox textbox)
		{
			CommaBlockString comma = new CommaBlockString();
			comma.Set(textbox.Text);

			string imsi = "";
			string val = "";
			while(true) 
			{
				if(comma.IsEOS())	break;

				comma.GetString(ref val);
				val = val.Trim();
				if(val.Length == 0)	break;
				imsi += (char)ConvertTool.ToInt32(val);
			}

			return imsi;
		}

		void EditToLabel(TextBox textbox, Label label)
		{
			label.Text = EditToBuf(textbox);
		}

		private void textBoxCommand0_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand0, this.label1Command0);
		}

		private void textBoxCommand1_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand1, this.label1Command1);
		}

		private void textBoxCommand2_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand2, this.label1Command2);
		}

		private void textBoxCommand3_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand3, this.label1Command3);
		}

		private void textBoxCommand4_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand4, this.label1Command4);
		}

		private void textBoxCommand5_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand5, this.label1Command5);
		}

		private void textBoxCommand6_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand6, this.label1Command6);
		}

		private void textBoxCommand7_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand7, this.label1Command7);
		}

		private void textBoxCommand8_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand8, this.label1Command8);
		}

		private void textBoxCommand9_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand9, this.label1Command9);
		}

		private void textBoxCommand10_TextChanged(object sender, System.EventArgs e)
		{
			EditToLabel(this.textBoxCommand10, this.label1Command10);
		}

		private void buttonDefault_Click(object sender, System.EventArgs e)
		{
			Config.SetDefaultEsc();
			FillEditBox();
		}

		private void label2_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}

/*


void DialogLinePrintEscSetting :: StringToCommand(int pos)
{
	int				i;
	char			buf[100], imsi[20];
	CommaBlockString commaBuf;

	GetWindowText(GetDlgItem(IDC_EDIT_ESC1+pos), buf, sizeof(buf));
	if(strlen(buf) == 0) {
		print_config.esc[pos].command[0] = 0;
		return;
	}	

	commaBuf.Set(buf);	
	for(i = 0; i < 20; i++) {
		commaBuf.GetString(imsi, sizeof(imsi));
		if(imsi[0] == 0) {
			print_config.esc[pos].command[i] = 0;
			return;
		}
		print_config.esc[pos].command[i] = atoi(imsi) % 256;		
	}	
}

*/ 

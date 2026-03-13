using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ExportLib;

namespace ExportServer
{
	/// <summary>
	/// Summary description for FormCodeView.
	/// </summary>
	public class FormCodeView : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.CheckBox checkBoxPause;
		private System.Windows.Forms.RadioButton radioButtonModeAscii;
		private System.Windows.Forms.RadioButton radioButtonModeHex;
		private System.Windows.Forms.RadioButton radioButtonModeDecimal;
		private System.Windows.Forms.Panel panel1;

		SocketThreadClass socketThread;

		public FormCodeView(SocketThreadClass st)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			socketThread = st;

			nMode = socketThread.obj.ProtocolGetCodeMode();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCodeView));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.checkBoxPause = new System.Windows.Forms.CheckBox();
            this.radioButtonModeAscii = new System.Windows.Forms.RadioButton();
            this.radioButtonModeHex = new System.Windows.Forms.RadioButton();
            this.radioButtonModeDecimal = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // listBox1
            // 
            this.listBox1.AccessibleDescription = null;
            this.listBox1.AccessibleName = null;
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.BackgroundImage = null;
            this.listBox1.Font = null;
            this.listBox1.Name = "listBox1";
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
            // radioButtonModeAscii
            // 
            this.radioButtonModeAscii.AccessibleDescription = null;
            this.radioButtonModeAscii.AccessibleName = null;
            resources.ApplyResources(this.radioButtonModeAscii, "radioButtonModeAscii");
            this.radioButtonModeAscii.BackgroundImage = null;
            this.radioButtonModeAscii.Font = null;
            this.radioButtonModeAscii.Name = "radioButtonModeAscii";
            this.radioButtonModeAscii.CheckedChanged += new System.EventHandler(this.radioButtonModeAscii_CheckedChanged);
            // 
            // radioButtonModeHex
            // 
            this.radioButtonModeHex.AccessibleDescription = null;
            this.radioButtonModeHex.AccessibleName = null;
            resources.ApplyResources(this.radioButtonModeHex, "radioButtonModeHex");
            this.radioButtonModeHex.BackgroundImage = null;
            this.radioButtonModeHex.Font = null;
            this.radioButtonModeHex.Name = "radioButtonModeHex";
            this.radioButtonModeHex.CheckedChanged += new System.EventHandler(this.radioButtonModeHex_CheckedChanged);
            // 
            // radioButtonModeDecimal
            // 
            this.radioButtonModeDecimal.AccessibleDescription = null;
            this.radioButtonModeDecimal.AccessibleName = null;
            resources.ApplyResources(this.radioButtonModeDecimal, "radioButtonModeDecimal");
            this.radioButtonModeDecimal.BackgroundImage = null;
            this.radioButtonModeDecimal.Font = null;
            this.radioButtonModeDecimal.Name = "radioButtonModeDecimal";
            this.radioButtonModeDecimal.CheckedChanged += new System.EventHandler(this.radioButtonModeDecimal_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.checkBoxPause);
            this.panel1.Controls.Add(this.radioButtonModeHex);
            this.panel1.Controls.Add(this.radioButtonModeDecimal);
            this.panel1.Controls.Add(this.radioButtonModeAscii);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // FormCodeView
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.panel1);
            this.Icon = null;
            this.Name = "FormCodeView";
            this.Load += new System.EventHandler(this.FormCodeView_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		void DrawOneLine(string buf)
		{
			if(buf.Length == 0)	return;
			if(this.listBox1.Items.Count > 100)	
				this.listBox1.Items.RemoveAt(0);
			this.listBox1.Items.Add(buf);

			this.listBox1.Update();
		}

		bool DisplayCodeModeAscii()
		{
			ushort code = 0;
			string buf = "";
			int old_s_r = -1;
			int s_r;
			byte ch;

			while(true) 
			{
				if(!socketThread.PeekCode(ref code))	break;

				s_r = (code >> 8) & 0xFF;

				if(old_s_r == -1) // first read
				{
					if(s_r == 0) 
					{
						buf += "Send:";
					}
					else 
					{
						buf += "Recv:";
					}
				}
				else 
				{
					if(s_r != old_s_r) 
					{
						DrawOneLine(buf);
						return true;
					}
				}

				old_s_r = s_r;

				ch = (byte)(code & 0xFF);

				switch(ch) 
				{
					case 2: buf += "<STX>";	break;
					case 3: buf += "<ETX>";	break;
					case 4: buf += "<EOT>";	break;
					case 5: buf += "<ENQ>";	break;
					case 6: buf += "<ACK>"; break;
					default:buf += String.Format("{0}", (char)ch); break;
				}
                
				socketThread.GetCode(ref code);
			}

			DrawOneLine(buf);
			return false;
		}

		bool DisplayCodeModeHex()
		{
			ushort code = 0;
			string buf = "";
			int old_s_r = -1;
			int s_r;
			while(true) 
			{
				if(!socketThread.PeekCode(ref code))	break;

				s_r = (code >> 8) & 0xFF;

				if(old_s_r == -1) // first read
				{
					if(s_r == 0) 
					{
						buf += "Send:";
					}
					else 
					{
						buf += "Recv:";
					}
				}
				else 
				{
					if(s_r != old_s_r) 
					{
						DrawOneLine(buf);
						return true;
					}
				}

				old_s_r = s_r;

				buf += String.Format(" {0:X02}", code & 0xFF);
                
				socketThread.GetCode(ref code);
			}

			DrawOneLine(buf);
			return false;
		}

		bool DisplayCodeModeDecimal()
		{
			ushort code = 0;
			string buf = "";
			int old_s_r = -1;
			int s_r;
			while(true) 
			{
				if(!socketThread.PeekCode(ref code))	break;

				s_r = (code >> 8) & 0xFF;

				if(old_s_r == -1) // first read
				{
					if(s_r == 0) 
					{
						buf += "Send:";
					}
					else 
					{
						buf += "Recv:";
					}
				}
				else 
				{
					if(s_r != old_s_r) 
					{
						DrawOneLine(buf);
						return true;
					}
				}

				old_s_r = s_r;

				buf += String.Format(" {0:000}", code & 0xFF);
                
				socketThread.GetCode(ref code);
			}

			DrawOneLine(buf);
			return false;
		}

		bool DisplayCode()
		{
			if(nMode == EnumCodeMode.ASCII) 
			{
				return DisplayCodeModeAscii();
			}
			else if(nMode == EnumCodeMode.HEX) 
			{
				return DisplayCodeModeHex();
			}
			else 
			{
				return DisplayCodeModeDecimal();
			}
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(this.checkBoxPause.Checked)	return;

			for(int i = 0; i < 10; i++) 
			{
				if(!DisplayCode())	break;
			}
		}

		EnumCodeMode nMode = 0;	// ascii

		private void FormCodeView_Load(object sender, System.EventArgs e)
		{
			this.radioButtonModeAscii.Checked = (nMode==EnumCodeMode.ASCII);
			this.radioButtonModeHex.Checked = (nMode==EnumCodeMode.HEX);
			this.radioButtonModeDecimal.Checked = (nMode==EnumCodeMode.DECIMAL);
		}

		private void radioButtonModeAscii_CheckedChanged(object sender, System.EventArgs e)
		{
			nMode = EnumCodeMode.ASCII;
		}

		private void radioButtonModeHex_CheckedChanged(object sender, System.EventArgs e)
		{
			nMode = EnumCodeMode.HEX;
		}

		private void radioButtonModeDecimal_CheckedChanged(object sender, System.EventArgs e)
		{
			nMode = EnumCodeMode.DECIMAL;
		}

		private void checkBoxPause_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}

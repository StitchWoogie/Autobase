using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;

namespace NetServer
{
	/// <summary>
	/// Summary description for FormViewSystemStatusMemory.
	/// </summary>
	public class FormViewSystemStatusMemory : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		public FormViewSystemStatusMemory()
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormViewSystemStatusMemory));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormViewSystemStatusMemory
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = null;
            this.Icon = null;
            this.Name = "FormViewSystemStatusMemory";
            this.Load += new System.EventHandler(this.FormViewSystemStatusMemory_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormViewSystemStatusMemory_Paint);
            this.ResumeLayout(false);

		}
		#endregion

		void Add(string title, int address)
		{
			DEFINE_MEMORY define = new DEFINE_MEMORY();
			define.title = title;
			define.address = address;
			defineMemory.Add(define);
		}

		private void FormViewSystemStatusMemory_Load(object sender, System.EventArgs e)
		{
			Add("Duplex Connect", (int)SSMDI.DuplexConnect);

			Add("Duplex Active This Server", (int)SSMDI.DuplexActiveI);
			Add("Duplex Active Another Server", (int)SSMDI.DuplexActiveYou);

			Add("AutoWatch This Server",    (int)SSMDI.DuplexAutoWatchI);
			Add("AutoWatch Another Server", (int)SSMDI.DuplexAutoWatchYou);

			Add("ErrorStatusPlcScanTimeOut This", (int)SSMDI.ErrorStatusPlcScanTimeOut);
			Add("ErrorStatusPlcScanTimeOut Another", (int)SSMDI.ErrorStatusPlcScanTimeOutYou); 	// PLC SCAN 메모리의 어느하나라도 통신이 5회 이상 이상하면 Set 해준다.	상대편의 상태
			Add("AnotherProgramExited This", (int)SSMDI.AnotherProgramExitingI);// 상태편의 부가 프로그램이 종료중인가를 검사한다.
			Add("AnotherProgramExited Another",   (int)SSMDI.AnotherProgramExitingYou);	// 상태편의 부가 프로그램이 종료중인가를 검사한다.

			Add("Communication Current Value", (int)SSMDI.CommunicationValue);

			Add("Node 0 Primary IP", 0x0100);
			Add("Node 1 Primary IP", 0x0101);
			Add("Node 2 Primary IP", 0x0102);
			Add("Node 3 Primary IP", 0x0103);
			Add("Node 4 Primary IP", 0x0104);
			Add("Node 5 Primary IP", 0x0105);
			Add("Node 6 Primary IP", 0x0106);
			Add("Node 7 Primary IP", 0x0107);
			Add("Node 8 Primary IP", 0x0108);
			Add("Node 9 Primary IP", 0x0109);
			Add("Node A Primary IP", 0x010A);
			Add("Node B Primary IP", 0x010B);
			Add("Node C Primary IP", 0x010C);
			Add("Node D Primary IP", 0x010D);
			Add("Node E Primary IP", 0x010E);
			Add("Node F Primary IP", 0x010F);
			Add("Node 0 Secondary IP", 0x0110);
			Add("Node 1 Secondary IP", 0x0111);
			Add("Node 2 Secondary IP", 0x0112);
			Add("Node 3 Secondary IP", 0x0113);
			Add("Node 4 Secondary IP", 0x0114);
			Add("Node 5 Secondary IP", 0x0115);
			Add("Node 6 Secondary IP", 0x0116);
			Add("Node 7 Secondary IP", 0x0117);
			Add("Node 8 Secondary IP", 0x0118);
			Add("Node 9 Secondary IP", 0x0119);
			Add("Node A Secondary IP", 0x011A);
			Add("Node B Secondary IP", 0x011B);
			Add("Node C Secondary IP", 0x011C);
			Add("Node D Secondary IP", 0x011D);
			Add("Node E Secondary IP", 0x011E);
			Add("Node F Secondary IP", 0x011F);
		}

		private System.Windows.Forms.Timer timer1;

		class DEFINE_MEMORY
		{
			public string title;
			public int address;
		}

		ArrayList defineMemory = new ArrayList();

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			Invalidate();
		}

		int nScrollPos = 0;

		private void FormViewSystemStatusMemory_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Rectangle rect = this.ClientRectangle;

			int y;
			int i;
			string buf;
			int cyChar = this.Font.Height;
			int cxChar = this.Font.Height/2;;
			DEFINE_MEMORY define;
			Brush tbrush;

			for(i = nScrollPos, y = 0; i < defineMemory.Count && y < rect.Bottom; y+=cyChar, i++) 
			{
				define = (DEFINE_MEMORY)defineMemory[i];
				buf = String.Format("{0:000}{1:X01}", define.address/16, define.address%16);
				if(SystemStatusMemory.GetDI(define.address) == 1)
					tbrush = Brushes.Blue;
				else
					tbrush = Brushes.DarkGray;

				g.DrawString(buf, this.Font, tbrush, 0, y);

				//pDC->SetTextColor(RGB(0, 0, 0));
				g.DrawString(define.title, this.Font, tbrush, cxChar*10, y);
			}	
		}


	}
}


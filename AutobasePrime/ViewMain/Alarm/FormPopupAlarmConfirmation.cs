using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using AutoLibLocal;

namespace LocalMain.Alarm
{
	/// <summary>
	/// Summary description for FormPopupAlarmConfirmation.
	/// </summary>
	public class FormPopupAlarmConfirmation : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormPopupAlarmConfirmation()
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
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonClose;

		public static FormPopupAlarmConfirmation hwndPopupAlarmConfirmation = null;

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPopupAlarmConfirmation));
            this.buttonClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // FormPopupAlarmConfirmation
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonClose);
            this.Name = "FormPopupAlarmConfirmation";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormPopupAlarmConfirmation_Load);
            this.SizeChanged += new System.EventHandler(this.FormPopupAlarmConfirmation_SizeChanged);
            this.Closed += new System.EventHandler(this.FormPopupAlarmConfirmation_Closed);
            this.ResumeLayout(false);

		}
		#endregion

        GraphicModule.FormAlarmEvent wndChild = new FormAlarmEvent();

		private void FormPopupAlarmConfirmation_Load(object sender, System.EventArgs e)
		{
			hwndPopupAlarmConfirmation = this;

			//formAlarmEvent = new FormAlarmEvent();

            wndChild.Text = "DefaultAlarmEventWindow";
            wndChild.SetFont(ConfigAlarm.fontAlarmEvent);
            wndChild.TopLevel = false;
            wndChild.FormBorderStyle = FormBorderStyle.None;
            wndChild.Parent = this.panel1;
            
            wndChild.IncludeMethod = 1;
			this.panel1.Controls.Add(wndChild);
            
            wndChild.Show();

            wndChild.Dock = DockStyle.Fill; // 이 부분이 Show 앞에 있으면 child의 초기 윈도우의 사이즈를 못받아서 위치가 초기 크기로 발생된다. Show 뒤로 옮겼다. 2010-9-1

            wndChild.Select();

            /*
            wndChild = new FormAlarmEvent();
            wndChild.Text = "DefaultAlarmEventWindow";
            wndChild.IncludeMethod = 1;
            wndChild.TopLevel = false;
            wndChild.FormBorderStyle = FormBorderStyle.None;

            wndChild.Show();        // form.Coltrols.Add 후에 Show하면 초기 크기가 맞지 않는다.

            this.panel1.Controls.Add(wndChild);

            wndChild.Dock = DockStyle.Fill;
            wndChild.SetFont(ConfigAlarm.fontAlarmEvent);

            wndChild.Select();*/
		}

		private void FormPopupAlarmConfirmation_Closed(object sender, System.EventArgs e)
		{
            wndChild.OnMyClosed();
			hwndPopupAlarmConfirmation = null;
			//ConfigRunMain.rAlarmConfirmBox.left = this.Left;
			//ConfigRunMain.rAlarmConfirmBox.top  = this.Top;
			//ConfigRunMain.rAlarmConfirmBox.right = this.Right;
			//ConfigRunMain.rAlarmConfirmBox.bottom = this.Bottom;
		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}

        private void FormPopupAlarmConfirmation_SizeChanged(object sender, EventArgs e)
        {
            
        }
	}
}

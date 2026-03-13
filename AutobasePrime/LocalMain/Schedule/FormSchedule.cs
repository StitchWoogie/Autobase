using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using PublicStudioLocalMain.Schedule;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormSchedule.
	/// </summary>
	public class FormSchedule : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panelChild;
		private System.Windows.Forms.Panel panelDay;
		private System.Windows.Forms.Panel panelToday;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
        private ToolBarButton toolBarButton6;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormSchedule()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSchedule));
            this.panelChild = new System.Windows.Forms.Panel();
            this.panelDay = new System.Windows.Forms.Panel();
            this.panelToday = new System.Windows.Forms.Panel();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton6 = new System.Windows.Forms.ToolBarButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelChild
            // 
            this.panelChild.AccessibleDescription = null;
            this.panelChild.AccessibleName = null;
            resources.ApplyResources(this.panelChild, "panelChild");
            this.panelChild.BackgroundImage = null;
            this.panelChild.Font = null;
            this.panelChild.Name = "panelChild";
            // 
            // panelDay
            // 
            this.panelDay.AccessibleDescription = null;
            this.panelDay.AccessibleName = null;
            resources.ApplyResources(this.panelDay, "panelDay");
            this.panelDay.BackgroundImage = null;
            this.panelDay.Font = null;
            this.panelDay.Name = "panelDay";
            // 
            // panelToday
            // 
            this.panelToday.AccessibleDescription = null;
            this.panelToday.AccessibleName = null;
            resources.ApplyResources(this.panelToday, "panelToday");
            this.panelToday.BackgroundImage = null;
            this.panelToday.Font = null;
            this.panelToday.Name = "panelToday";
            // 
            // toolBar1
            // 
            this.toolBar1.AccessibleDescription = null;
            this.toolBar1.AccessibleName = null;
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.BackgroundImage = null;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2,
            this.toolBarButton3,
            this.toolBarButton4,
            this.toolBarButton5,
            this.toolBarButton6});
            this.toolBar1.Font = null;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButton1
            // 
            resources.ApplyResources(this.toolBarButton1, "toolBarButton1");
            this.toolBarButton1.Name = "toolBarButton1";
            // 
            // toolBarButton2
            // 
            resources.ApplyResources(this.toolBarButton2, "toolBarButton2");
            this.toolBarButton2.Name = "toolBarButton2";
            // 
            // toolBarButton3
            // 
            resources.ApplyResources(this.toolBarButton3, "toolBarButton3");
            this.toolBarButton3.Name = "toolBarButton3";
            // 
            // toolBarButton4
            // 
            resources.ApplyResources(this.toolBarButton4, "toolBarButton4");
            this.toolBarButton4.Name = "toolBarButton4";
            // 
            // toolBarButton5
            // 
            resources.ApplyResources(this.toolBarButton5, "toolBarButton5");
            this.toolBarButton5.Name = "toolBarButton5";
            // 
            // toolBarButton6
            // 
            resources.ApplyResources(this.toolBarButton6, "toolBarButton6");
            this.toolBarButton6.Name = "toolBarButton6";
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.panelToday);
            this.panel1.Controls.Add(this.panelDay);
            this.panel1.Controls.Add(this.panelChild);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            // 
            // FormSchedule
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolBar1);
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "FormSchedule";
            this.Load += new System.EventHandler(this.FormSchedule_Load);
            this.SizeChanged += new System.EventHandler(this.FormSchedule_SizeChanged);
            this.Closed += new System.EventHandler(this.FormSchedule_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormSchedule_KeyDown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public FormScheduleChild formChild;
		FormScheduleDay   formDay;
		FormScheduleToday formToday;

		private void FormSchedule_Load(object sender, System.EventArgs e)
		{
			formChild = new FormScheduleChild();
			formDay = new FormScheduleDay();
			formToday = new FormScheduleToday();

			formChild.TopLevel = false;
			formChild.Dock = DockStyle.Fill;
			this.panelChild.Controls.Add(formChild);

			formDay.TopLevel = false;
			formDay.Dock = DockStyle.Fill;
			this.panelDay.Controls.Add(formDay);

			formToday.TopLevel = false;
			formToday.Dock = DockStyle.Fill;
			this.panelToday.Controls.Add(formToday);

			formChild.Show();
			formDay.Show();
			formToday.Show();

			formChild.SetScheduleDayWnd(formDay);

			ringSchedule.push(this);

			ReCalcSize();
		}

		public static CatWindowRing ringSchedule = new CatWindowRing();

		public static void OnScheduleStructChanged()
		{
			CheckEngineSchedule.ScheduleGoPrepare();	// 오늘의 운전 항목 갱신.

			Form hwnd = ringSchedule.GetFirstHWND();
			if(hwnd != null) 
			{
				FormSchedule form = (FormSchedule)hwnd;
				form.formChild.ReLoad();
				form.formDay.ReLoad();
			}
		}

		private void FormSchedule_Closed(object sender, System.EventArgs e)
		{
			ringSchedule.pop(this);
		}

		void ReCalcSize()
		{
			this.panelChild.Height = panel1.ClientRectangle.Height/2;
			this.panelDay.Width = panel1.ClientRectangle.Width/2;
		}

		private void panel1_SizeChanged(object sender, System.EventArgs e)
		{
			ReCalcSize();
		}

        public static bool ConfigFixedSchedule()
        {
            FormConfigSchedule dialog = new FormConfigSchedule();

            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(FormLocalMain.formMain) == DialogResult.OK)
            {
                Schedule.blockScheduleModel = ScheduleLib.ModelLoad();      // 고정 스케쥴 변경 시 모델이 변경될 수 있다.
                Schedule.blockScheduleFixed = ScheduleLib.ScheduleLoadFixed();
                Schedule.FindFixedScheduleAtWeek();   // 고정스케쥴 변경 후 주간 스케쥴의 block_pos 등을 다시 계산해야 한다.
                FormSchedule.OnScheduleStructChanged();

                return true;
            }

            return false;
        }

        public static void ConfigScheduleAdditional()
        {
            FormConfigScheduleAdditional dialog = new FormConfigScheduleAdditional();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(FormLocalMain.formMain) == DialogResult.OK)
            {
                Schedule.blockScheduleModel = ScheduleLib.ModelLoad();      // 추가 스케쥴 변경 시 모델이 변경될 수 있다.
                Schedule.blockScheduleAdditional = ScheduleLib.ScheduleLoadAdditional();
                
                FormSchedule.OnScheduleStructChanged();
            }
        }

        public static void ConfigScheduleWeek()
        {
            FormConfigScheduleWeek dialog = new FormConfigScheduleWeek();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(FormLocalMain.formMain) == DialogResult.OK)
            {
                Schedule.scheduleWeek = ScheduleLib.ScheduleLoadWeek();
                Schedule.FindFixedScheduleAtWeek();
                FormSchedule.OnScheduleStructChanged();
            }
        }

        public static bool ScheduleModelConfig(ref ArrayList block)
        {
            FormConfigModel dialog = new FormConfigModel();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(FormLocalMain.formMain) == DialogResult.OK)
            {
                block = ScheduleLib.ModelLoad();
                FormSchedule.OnScheduleStructChanged();
                return true;
            }

            return false;
        }

        public static void ConfigScheduleDesigner() //ScheduleDesigner 추가 hsjeong 25-03-11
        {

            FormConfigScheduleDesigner dialog = new FormConfigScheduleDesigner();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(FormLocalMain.formMain) == DialogResult.OK)
            {
                Schedule.blockScheduleModel = ScheduleLib.ModelLoad();      // 고정 스케쥴 변경 시 모델이 변경될 수 있다.
                Schedule.blockScheduleFixed = ScheduleLib.ScheduleLoadFixed();
                Schedule.FindFixedScheduleAtWeek();   // 고정스케쥴 변경 후 주간 스케쥴의 block_pos 등을 다시 계산해야 한다.
                Schedule.blockScheduleAdditional = ScheduleLib.ScheduleLoadAdditional();
                

                Schedule.scheduleWeek = ScheduleLib.ScheduleLoadWeek();
                Schedule.FindFixedScheduleAtWeek();

                
                FormSchedule.OnScheduleStructChanged();
            }
        }

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == this.toolBarButton1) 
			{
				Close();
			}
            else if (e.Button == this.toolBarButton2) 
			{
                if (ConfigFixedSchedule())
                {
                    Invalidate();
                }
			}
            else if (e.Button == this.toolBarButton3) 
			{
                FormSchedule.ConfigScheduleAdditional();
			}
            else if (e.Button == this.toolBarButton4) 
			{
				FormSchedule.ScheduleModelConfig(ref Schedule.blockScheduleModel);
			}
            else if (e.Button == this.toolBarButton5) 
			{
                FormSchedule.ConfigScheduleWeek();
			}
            else if (e.Button == this.toolBarButton6) // designer 호출 추가. hsjeong 25-03-11
            {
                FormSchedule.ConfigScheduleDesigner();
            }
            else { }
		}

		private void FormSchedule_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape)	Close();
			else if(e.KeyCode == Keys.F3) 
			{
                if (ConfigFixedSchedule())
                {
                    Invalidate();
                }
			}
			else if(e.KeyCode == Keys.F4) 
			{
                FormSchedule.ConfigScheduleAdditional();
			}
			else if(e.KeyCode == Keys.F6) 
			{
                FormSchedule.ScheduleModelConfig(ref Schedule.blockScheduleModel);
			}
			else if(e.KeyCode == Keys.F7) 
			{
                FormSchedule.ConfigScheduleWeek();
			}
            else if (e.KeyCode == Keys.F8) //designer 단축키 추가. hsjeong 25-03-11
            {
                FormSchedule.ConfigScheduleDesigner();
            }
		}

		private void FormSchedule_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Height = this.ClientRectangle.Height-this.toolBar1.Height;
		}
	}
}


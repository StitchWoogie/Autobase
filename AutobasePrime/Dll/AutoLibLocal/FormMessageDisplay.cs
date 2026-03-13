using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using System.Collections.Generic;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for FormMessageDisplay.
	/// </summary>
	public class FormMessageDisplay : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		private System.Windows.Forms.Timer timer1;
		public System.Windows.Forms.Label labelMsg;
		public TimeOutClass timeout = new TimeOutClass();
        

		public FormMessageDisplay()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			timeout.Reset();
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 900;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labelMsg
            // 
            this.labelMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMsg.Location = new System.Drawing.Point(0, 0);
            this.labelMsg.Name = "labelMsg";
            this.labelMsg.Size = new System.Drawing.Size(592, 54);
            this.labelMsg.TabIndex = 0;
            this.labelMsg.Text = "label1";
            // 
            // FormMessageDisplay
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(592, 54);
            this.Controls.Add(this.labelMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormMessageDisplay";
            this.ShowInTaskbar = false;
            this.Text = "FormMessageDisplay";
            this.Load += new System.EventHandler(this.FormMessageDisplay_Load);
            this.Closed += new System.EventHandler(this.FormMessageDisplay_Closed);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormMessageDisplay_Load(object sender, System.EventArgs e)
		{
			MessageDisplay.formMessageDisplay = this;
		}

		private void FormMessageDisplay_Closed(object sender, System.EventArgs e)
		{
			MessageDisplay.formMessageDisplay = null;
		}

        bool bTimerRunning = false;

		private void timer1_Tick(object sender, System.EventArgs e)
		{
            if (bTimerRunning) return;
            bTimerRunning = true;

			if(timeout.IsTimeOut(MessageDisplay.nScreenLifeTime)) 
			{
				timer1.Enabled = false;
				Close();
			}

            bTimerRunning = false;
		}
	}

    public class MessageDisplay
    {
        public static FormMessageDisplay formMessageDisplay = null;
        public static int nScreenLifeTime = 5;

        public MessageDisplay()
        {

        }

        //static void ShowDialog(string msg)
        //{
        //    if (formMessageDisplay == null)
        //    {
        //        FormMessageDisplay form = new FormMessageDisplay();
        //        //form.TopMost = true;
        //        form.labelMsg.Text = msg;
        //        form.Owner = TotalConfig.formMain;

        //        if (NetTools.Tools.IsLangKorean())
        //            form.Text = "메시지 발생:" + DateTimeServer.Now.ToString();
        //        else if (NetTools.Tools.IsLangChinese())
        //            form.Text = "消息发生:" + DateTimeServer.Now.ToString();
        //        else
        //            form.Text = "Message:" + DateTimeServer.Now.ToString();

        //        form.Show();
        //    }
        //    else
        //    {
        //        formMessageDisplay.labelMsg.Text = msg;
        //        formMessageDisplay.Invalidate();
        //        formMessageDisplay.timeout.Reset();

        //        if (NetTools.Tools.IsLangKorean())
        //            formMessageDisplay.Text = "메시지 발생:" + DateTimeServer.Now.ToString();
        //        else if (NetTools.Tools.IsLangChinese())
        //            formMessageDisplay.Text = "消息发生:" + DateTimeServer.Now.ToString();
        //        else
        //            formMessageDisplay.Text = "Message:" + DateTimeServer.Now.ToString();
        //    }
        //}

        //메시지 위치 수정 20241010 PSU
        //static void ShowDialog(string msg)
        //{
        //    const int margin = 50; // 화면 가장자리로부터의 여백

        //    if (formMessageDisplay == null)
        //    {
        //        FormMessageDisplay form = new FormMessageDisplay();
        //        form.labelMsg.Text = msg;
        //        form.Owner = TotalConfig.formMain;

        //        // FormLocalMain.formMain의 위치를 기준으로 메시지 창 위치 설정
        //        Screen ownerScreen = Screen.FromControl(TotalConfig.formMain);

        //        form.StartPosition = FormStartPosition.Manual;
        //        form.Left = ownerScreen.WorkingArea.Left + margin;
        //        form.Top = ownerScreen.WorkingArea.Top + margin;

        //        if (NetTools.Tools.IsLangKorean())
        //            form.Text = "메시지 발생:" + DateTimeServer.Now.ToString();
        //        else if (NetTools.Tools.IsLangChinese())
        //            form.Text = "消息发生:" + DateTimeServer.Now.ToString();
        //        else
        //            form.Text = "Message:" + DateTimeServer.Now.ToString();

        //        form.Show();
        //    }
        //    else
        //    {
        //        formMessageDisplay.labelMsg.Text = msg;

        //        // 현재 ownerScreen 가져오기
        //        Screen currentOwnerScreen = Screen.FromControl(TotalConfig.formMain);

        //        // 현재 formMessageDisplay의 Screen 가져오기
        //        Screen formScreen = Screen.FromControl(formMessageDisplay);

        //        // devicename으로 Screen이 변경되었는지 확인. screen만 비교하면, 좌표값이 달라질 경우 실행된다.
        //        if (currentOwnerScreen.DeviceName != formScreen.DeviceName)
        //        {
        //            // Screen이 변경되었다면 위치 업데이트
        //            formMessageDisplay.Left = currentOwnerScreen.WorkingArea.Left + margin;
        //            formMessageDisplay.Top = currentOwnerScreen.WorkingArea.Top + margin;
        //        }

        //        formMessageDisplay.Invalidate();
        //        formMessageDisplay.timeout.Reset();

        //        if (NetTools.Tools.IsLangKorean())
        //            formMessageDisplay.Text = "메시지 발생:" + DateTimeServer.Now.ToString();
        //        else if (NetTools.Tools.IsLangChinese())
        //            formMessageDisplay.Text = "消息发生:" + DateTimeServer.Now.ToString();
        //        else
        //            formMessageDisplay.Text = "Message:" + DateTimeServer.Now.ToString();
        //    }
        //}

        // Screen.FromControl(TotalConfig.formMain) null 오류 수정 20250205 PSU
        static void ShowDialog(string msg)
        {
            const int margin = 50; // 화면 가장자리로부터의 여백
            Screen targetScreen;

            // TotalConfig.formMain이 null이 아니고 유효한 경우
            if (TotalConfig.formMain != null && !TotalConfig.formMain.IsDisposed)
            {
                targetScreen = Screen.FromControl(TotalConfig.formMain);
            }
            else
            {
                // 기본 화면 사용
                targetScreen = Screen.PrimaryScreen;
            }

            if (formMessageDisplay == null)
            {
                FormMessageDisplay form = new FormMessageDisplay();
                form.labelMsg.Text = msg;
                form.Owner = TotalConfig.formMain;

                form.StartPosition = FormStartPosition.Manual;
                form.Left = targetScreen.WorkingArea.Left + margin;
                form.Top = targetScreen.WorkingArea.Top + margin;

                if (NetTools.Tools.IsLangKorean())
                    form.Text = "메시지 발생:" + DateTimeServer.Now.ToString();
                else if (NetTools.Tools.IsLangChinese())
                    form.Text = "消息发生:" + DateTimeServer.Now.ToString();
                else
                    form.Text = "Message:" + DateTimeServer.Now.ToString();

                form.Show();
            }
            else
            {
                formMessageDisplay.labelMsg.Text = msg;

                // 현재 formMessageDisplay의 Screen 확인
                Screen formScreen = Screen.FromControl(formMessageDisplay);

                // devicename으로 Screen이 변경되었는지 확인. screen만 비교하면, 좌표값이 달라질 경우 실행된다.
                if (targetScreen.DeviceName != formScreen.DeviceName)
                {
                    formMessageDisplay.Left = targetScreen.WorkingArea.Left + margin;
                    formMessageDisplay.Top = targetScreen.WorkingArea.Top + margin;
                }

                formMessageDisplay.Invalidate();
                formMessageDisplay.timeout.Reset();

                if (NetTools.Tools.IsLangKorean())
                    formMessageDisplay.Text = "메시지 발생:" + DateTimeServer.Now.ToString();
                else if (NetTools.Tools.IsLangChinese())
                    formMessageDisplay.Text = "消息发生:" + DateTimeServer.Now.ToString();
                else
                    formMessageDisplay.Text = "Message:" + DateTimeServer.Now.ToString();
            }
        }


        /* 일단 메시지는 모두 ShowInThread를 사용한다.
        /// <summary>
        /// 메인 UI에서 사용할 때 이함수를 사용한다. 쓰레드 안에서 사용할 때는 ShowInThread를 사용한다.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
		public static void Show(string format, params object[] args)
		{
			string msg;

			if(args.Length == 0)	// 이 부분이 없으면 Tag Description에서 {} 문자를 사용했을 때 다운된다.	
				msg = format;
			else
				msg = String.Format(format, args);

            ShowDialog(msg);
		}*/

        // 일단 메시지는 모두 ShowInThread를 사용한다. 2016-3-31
        public static void Show(string format, params object[] args)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ShowInThread(format, args);
            }
            else
            {
                string msg;

                if (args.Length == 0)	// 이 부분이 없으면 Tag Description에서 {} 문자를 사용했을 때 다운된다.	
                    msg = format;
                else
                    msg = String.Format(format, args);

                ShowDialog(msg);
            }
        }

        /// <summary>
        /// 쓰레드 안에서 사용할 때 이 함수를 불러준다. 이 함수를 사용하는 경우 Main의 타이머에서 CheckMessage()를 수시로 호출해 주어야 한다.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void ShowInThread(string format, params object[] args)
        {
            string msg;

            if (args.Length == 0)	// 이 부분이 없으면 Tag Description에서 {} 문자를 사용했을 때 다운된다.	
                msg = format;
            else
                msg = String.Format(format, args);

            //lock (sWaitMessage)
            //{
            sWaitMessage = msg;
            //}
        }

        // static List<string> arrayMsg = new List<string>();
        static string sWaitMessage = null;

        public static void CheckMessage()
        {
            if (sWaitMessage == null) return;

            string msg;

            //lock (sWaitMessage)
            //{
            msg = sWaitMessage;
            sWaitMessage = null;
            //}

            ShowDialog(msg);
        }
    }

    //20241010 PSU
    public class MonitorIndexDisplay
    {
        private class MonitorForm : Form
        {
            public MonitorForm(int index, Rectangle bounds)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = bounds.Location;
                this.Size = new Size(300, 150); // 크기를 조금 늘렸습니다
                this.FormBorderStyle = FormBorderStyle.None;
                this.TopMost = true;
                this.BackColor = Color.Black;
                this.Opacity = 0.7;

                Label label = new Label();
                label.Text = string.Format("Monitor Index: {0}\nResolution: {1}x{2}",
                    index, bounds.Width, bounds.Height);
                label.ForeColor = Color.White;
                label.Font = new Font("Arial", 16, FontStyle.Bold);
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleCenter;

                this.Controls.Add(label);
            }
        }

        private static MonitorForm[] monitorForms;
        private static TimeOutClass timeout = new TimeOutClass();
        private static Timer checkTimer;

        public static void ShowMonitorIndexes(int durationSeconds)
        {
            if (monitorForms != null && monitorForms.Length > 0)
            {
                // 이미 창이 표시 중이면 타이머만 리셋
                timeout.Reset();
                return;
            }

            Screen[] screens = Screen.AllScreens;
            monitorForms = new MonitorForm[screens.Length];

            for (int i = 0; i < screens.Length; i++)
            {
                MonitorForm form = new MonitorForm(i, screens[i].Bounds);
                monitorForms[i] = form;
                form.Show();
            }

            // TimeOutClass 설정
            timeout.Reset();
            SetupCheckTimer(durationSeconds);
        }

        public static void ShowMonitorIndexes()
        {
            ShowMonitorIndexes(5); // 기본 5초 동안 표시
        }

        private static void SetupCheckTimer(int durationSeconds)
        {
            if (checkTimer == null)
            {
                checkTimer = new Timer();
                checkTimer.Tick += new EventHandler(CheckTimeout);
            }
            checkTimer.Interval = 1000; // 1초마다 체크
            checkTimer.Start();
        }

        private static void CheckTimeout(object sender, EventArgs e)
        {
            if (timeout.IsTimeOut(MessageDisplay.nScreenLifeTime))
            {
                CloseMonitorForms();
            }
        }

        private static void CloseMonitorForms()
        {
            if (checkTimer != null)
            {
                checkTimer.Stop();
            }

            if (monitorForms != null)
            {
                foreach (MonitorForm form in monitorForms)
                {
                    if (form != null && !form.IsDisposed)
                    {
                        form.Close();
                        form.Dispose();
                    }
                }
                monitorForms = null;
            }
        }
	}
}

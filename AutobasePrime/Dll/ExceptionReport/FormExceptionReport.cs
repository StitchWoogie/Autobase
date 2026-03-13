using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using AutoLibLocal;
using System.IO;
using NetTools;

namespace ExceptionReport
{
	/// <summary>
	/// Summary description for FormExceptionReport.
	/// </summary>
	public class FormExceptionReport : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textBoxMsg;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonSendMail;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxEtc;
		private System.Windows.Forms.Label label5;

		Exception exceptionMsg;

		static bool bRunning = false;
		public static void Go(Exception exception)
		{
			if(bRunning)	return;	// 이미 실행 중

			bRunning = true;

			FormExceptionReport dialog = new FormExceptionReport(exception);

			dialog.ShowDialog();

			bRunning = false;
		}

		public FormExceptionReport(Exception exception)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			exceptionMsg = exception;
			
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExceptionReport));
            this.textBoxMsg = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSendMail = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxEtc = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxMsg
            // 
            this.textBoxMsg.AccessibleDescription = null;
            this.textBoxMsg.AccessibleName = null;
            resources.ApplyResources(this.textBoxMsg, "textBoxMsg");
            this.textBoxMsg.BackgroundImage = null;
            this.textBoxMsg.Font = null;
            this.textBoxMsg.Name = "textBoxMsg";
            this.textBoxMsg.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // buttonClose
            // 
            this.buttonClose.AccessibleDescription = null;
            this.buttonClose.AccessibleName = null;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.BackgroundImage = null;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Font = null;
            this.buttonClose.Name = "buttonClose";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // buttonSendMail
            // 
            this.buttonSendMail.AccessibleDescription = null;
            this.buttonSendMail.AccessibleName = null;
            resources.ApplyResources(this.buttonSendMail, "buttonSendMail");
            this.buttonSendMail.BackgroundImage = null;
            this.buttonSendMail.Font = null;
            this.buttonSendMail.Name = "buttonSendMail";
            this.buttonSendMail.Click += new System.EventHandler(this.buttonSendMail_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.AccessibleDescription = null;
            this.buttonSave.AccessibleName = null;
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.BackgroundImage = null;
            this.buttonSave.Font = null;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Name = "label4";
            // 
            // textBoxEtc
            // 
            this.textBoxEtc.AccessibleDescription = null;
            this.textBoxEtc.AccessibleName = null;
            resources.ApplyResources(this.textBoxEtc, "textBoxEtc");
            this.textBoxEtc.BackgroundImage = null;
            this.textBoxEtc.Font = null;
            this.textBoxEtc.Name = "textBoxEtc";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Name = "label5";
            // 
            // FormExceptionReport
            // 
            this.AcceptButton = this.buttonClose;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxEtc);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonSendMail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExceptionReport";
            this.Load += new System.EventHandler(this.FormExceptionReport_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		DateTime GetTimeFromVersion(string sver)
		{
			Version version = new Version(sver);
			DateTime t = new DateTime(2000, 1, 1);
			t = t.AddDays(version.Build);
			t = t.AddSeconds(version.Revision*2);
			return t;
		}

		void ExceptionToEdit()
		{
			string msg = "";
            msg += "DateTime : " + DateTimeServer.Now.ToString() + "\r\n\r\n";

			//msg += String.Format("Main Version : {0}.{1}.{2}\r\n", TotalConfig.nVersionMajor, TotalConfig.nVersionMinor, TotalConfig.nVersionBuild);
			try		// 엑셀에서 이 부분을 사용하면 Exception이 발생한다.
			{
				//DateTime t = GetTimeFromVersion(Application.ProductVersion);
				//msg += String.Format("Program : {0} ({1}) ({2})\r\n", Application.ProductName, Application.ProductVersion, t);
				msg += String.Format("Program : {0} ({1})\r\n", Application.ProductName, Application.ProductVersion);
			}
			catch 
			{
				msg += String.Format("Program : ???? (??.??.??.??)\r\n");
			}
			System.OperatingSystem veros = Environment.OSVersion;
			msg += String.Format("OS Version : {0} ({1})\r\n", veros.Platform.ToString(), veros.Version.ToString());
			msg += String.Format("CLR Version : {0}\r\n", Environment.Version.ToString());

			msg += "\r\n";
			
			msg += "Source : "+exceptionMsg.Source+"\r\n";
			msg += "Message : "+exceptionMsg.Message+"\r\n";
			msg += "\r\n"+exceptionMsg.ToString()+"\r\n";
			
			/* 아래 부분을 사용하면 Souce의 Version 8.0.2.0 과 같은 버전 정보만 추가된다.
			msg += "\r\n"+"Class information"+"\r\n";
			
			string s;

			Stream stream = new MemoryStream(100000);
			SoapFormatter format = new SoapFormatter();
			format.Serialize(stream, exceptionMsg);
			stream.Seek(0, SeekOrigin.Begin);
			TextReader reader = new StreamReader(stream);
			s = reader.ReadToEnd();
			reader.Close();
			stream.Close();

			msg += s;
			*/

			textBoxMsg.Text = msg;
		}

		private void FormExceptionReport_Load(object sender, System.EventArgs e)
		{
			ExceptionToEdit();

			buttonClose.Select();
		}

		private void buttonSendMail_Click(object sender, System.EventArgs e)
		{
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage("user@hansolt.com", "error@autobase.biz");

			try 
			{
				mail.Subject = String.Format("Program Error ({0}, {1})", Application.ProductName, Application.ProductVersion);
			}
			catch 
			{
				mail.Subject = String.Format("Program Error (????, ??.??.??.??)");
			}
			
			mail.Body = textBoxMsg.Text;
			if(Tools.IsLangKorean())
				mail.Body += "\n참고사항\n";
			else
				mail.Body += "\nUser message\n";

			mail.Body += textBoxEtc.Text;
			//mail.BodyFormat = MailFormat.Text;
			
			try 
			{
                SmtpClient client = new SmtpClient("autobase.biz");
                // Add credentials if the SMTP server requires them.
                client.Credentials = CredentialCache.DefaultNetworkCredentials;
                client.Send(mail);
			}
			catch (Exception ex)
			{
				string msg;
			
				if(Tools.IsLangKorean()) 
				{
					msg = String.Format("메일 발송 중 오류 발생\n메일이 발송되지 않을 경우 파일로 저장해서 보내주시기 바랍니다.\nMessage={0}", ex.Message);
					MessageBox.Show(msg, "메일 발송 오류");
				}
				else 
				{
					msg = String.Format("Can't send mail to A/S center\n\nMessage={0}", ex.Message);
					MessageBox.Show(msg, "Send mail error");
				}
				return;
			}

			if(Tools.IsLangKorean())
				MessageBox.Show("A/S 센타로 메일을 보냈습니다.", "발송 완료");
			else
				MessageBox.Show("Mail sended to A/S Center", "Mail send O.K");
		}

		private void buttonSave_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();

			dialog.Filter = "Text Files (*.txt)|*.txt";

			dialog.FileName = "ErrorMsg.txt";

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				TextWriter writer = new StreamWriter(dialog.FileName);
	
				if(writer == null) 
				{
					if(Tools.IsLangKorean()) 
						MessageBox.Show(dialog.FileName, "파일을 쓸 수 없습니다.");
					else
						MessageBox.Show(dialog.FileName, "Can't write the file.");
					return;
				}
				writer.Write(textBoxMsg.Text);
				writer.WriteLine();
				writer.WriteLine("User message");
				writer.Write(textBoxEtc.Text);
				writer.Close();

				string msg;
				
				if(Tools.IsLangKorean()) 
				{
					msg = String.Format("파일이 다음이름으로 저장되었습니다.\n{0}", dialog.FileName);
					MessageBox.Show(msg, "저장 완료"); 
				}
				else 
				{
					msg = String.Format("The file is successfully saved.\n{0}", dialog.FileName);
					MessageBox.Show(msg, "Save O.K"); 
				}
			}
		}
	}
}



/*
 * 
 * 
Framework 1.0 오류 메시지 원본

See the end of this message for details on invoking 
just-in-time (JIT) debugging instead of this dialog box.

************** Exception Text **************
System.DivideByZeroException: Attempted to divide by zero.
   at RunMain.FormMain.FormMain_Load(Object sender, EventArgs e) in d:\net\autobase\runmain\formmain.cs:line 337
   at System.Windows.Forms.Form.OnLoad(EventArgs e)
   at System.Windows.Forms.Form.OnCreateControl()
   at System.Windows.Forms.Control.CreateControl(Boolean fIgnoreVisible)
   at System.Windows.Forms.Control.CreateControl()
   at System.Windows.Forms.Control.WmShowWindow(Message& m)
   at System.Windows.Forms.Control.WndProc(Message& m)
   at System.Windows.Forms.ScrollableControl.WndProc(Message& m)
   at System.Windows.Forms.ContainerControl.WndProc(Message& m)
   at System.Windows.Forms.Form.WmShowWindow(Message& m)
   at System.Windows.Forms.Form.WndProc(Message& m)
   at System.Windows.Forms.ControlNativeWindow.OnMessage(Message& m)
   at System.Windows.Forms.ControlNativeWindow.WndProc(Message& m)
   at System.Windows.Forms.NativeWindow.Callback(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)


************** Loaded Assemblies **************
mscorlib
    Assembly Version: 1.0.3300.0
    Win32 Version: 1.0.3705.288
    CodeBase: file:///c:/windows/microsoft.net/framework/v1.0.3705/mscorlib.dll
----------------------------------------
RunMain
    Assembly Version: 8.0.2.0
    Win32 Version: 8.0.2.0
    CodeBase: file:///D:/EXE/CAT32/HANGUL/RunMain.exe
----------------------------------------
System.Windows.Forms
    Assembly Version: 1.0.3300.0
    Win32 Version: 1.0.3705.288
    CodeBase: file:///c:/windows/assembly/gac/system.windows.forms/1.0.3300.0__b77a5c561934e089/system.windows.forms.dll
----------------------------------------
System
    Assembly Version: 1.0.3300.0
    Win32 Version: 1.0.3705.288
    CodeBase: file:///c:/windows/assembly/gac/system/1.0.3300.0__b77a5c561934e089/system.dll
----------------------------------------
AutoLibLocal
    Assembly Version: 8.5.1.10
    Win32 Version: 8.5.1.10
    CodeBase: file:///D:/EXE/CAT32/HANGUL/AutoLibLocal.DLL
----------------------------------------
System.Xml
    Assembly Version: 1.0.3300.0
    Win32 Version: 1.0.3705.288
    CodeBase: file:///c:/windows/assembly/gac/system.xml/1.0.3300.0__b77a5c561934e089/system.xml.dll
----------------------------------------
System.Drawing
    Assembly Version: 1.0.3300.0
    Win32 Version: 1.0.3705.288
    CodeBase: file:///c:/windows/assembly/gac/system.drawing/1.0.3300.0__b03f5f7f11d50a3a/system.drawing.dll
----------------------------------------
RunMain.resources
    Assembly Version: 8.0.2.0
    Win32 Version: 8.0.2.0
    CodeBase: file:///D:/EXE/CAT32/HANGUL/ko/RunMain.resources.DLL
----------------------------------------
DatabaseSaveList
    Assembly Version: 8.0.3.0
    Win32 Version: 8.0.3.0
    CodeBase: file:///D:/EXE/CAT32/HANGUL/DatabaseSaveList.DLL
----------------------------------------
DatabaseConnection
    Assembly Version: 8.0.4.2
    Win32 Version: 8.0.4.2
    CodeBase: file:///D:/exe/cat32/hangul/DatabaseConnection.DLL
----------------------------------------
AutoLib
    Assembly Version: 8.0.3.0
    Win32 Version: 8.0.3.0
    CodeBase: file:///D:/EXE/CAT32/HANGUL/AutoLib.DLL
----------------------------------------
System.Runtime.Serialization.Formatters.Soap
    Assembly Version: 1.0.3300.0
    Win32 Version: 1.0.3705.288
    CodeBase: file:///c:/windows/assembly/gac/system.runtime.serialization.formatters.soap/1.0.3300.0__b03f5f7f11d50a3a/system.runtime.serialization.formatters.soap.dll
----------------------------------------
NetTools
    Assembly Version: 8.0.0.2
    Win32 Version: 8.0.0.2
    CodeBase: file:///D:/EXE/CAT32/HANGUL/NetTools.DLL
----------------------------------------
System.Data
    Assembly Version: 1.0.3300.0
    Win32 Version: 1.0.3705.288
    CodeBase: file:///c:/windows/assembly/gac/system.data/1.0.3300.0__b77a5c561934e089/system.data.dll
----------------------------------------

************** JIT Debugging **************
To enable just in time (JIT) debugging, the config file for this
application or machine (machine.config) must have the
jitDebugging value set in the system.windows.forms section.
The application must also be compiled with debugging
enabled.

For example:

<configuration>
    <system.windows.forms jitDebugging="true" />
</configuration>

When JIT debugging is enabled, any unhandled exception
will be sent to the JIT debugger registered on the machine
rather than being handled by this dialog.
*/
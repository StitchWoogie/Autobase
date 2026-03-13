using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using WebTools;
using NetTools;
using System.Threading;

namespace UsernameLibrary
{
    /// <summary>
    /// DialogResult가 OK이면 로그인에 성공한 것이다.
    /// </summary>
    public partial class FormLogIn : Form
    {
        public FormLogIn()
        {
            InitializeComponent();
        }

        private void FormLogIn_Load(object sender, EventArgs e)
        {
            this.checkBoxSaveUsername.Checked = Config.bSaveUsername;
            this.checkBoxSavePassword.Checked = Config.bSavePassword;
            this.checkBoxAutoLogIn.Checked = Config.bAutoLogin;

            if (Config.bSaveUsername)
                this.textBoxUsername.Text = Config.sUserName;

            if (Config.bSavePassword)
            {
                PassCode = Config.aPassCode;

                this.textBoxPassword.Text = init_password;
            }

            if (sRegisterInfoText != null)
            {
                this.linkLabel1.Text = sRegisterInfoText;
            }

            if (bDisplayRegisterInfoUrl == false)
                this.linkLabel1.Visible = false;
        }

        string init_password = "_P_a_S_W_O_r_D_m_u_s_";
        byte[] PassCode = null;

        byte[] StringToBytes(string buf)
        {
            byte[] b = new byte[buf.Length * 2];

            for (int i = 0; i < buf.Length; i++)
            {
                b[i * 2 + 0] = (byte)(buf[i] / 256);
                b[i * 2 + 1] = (byte)(buf[i] % 256);
            }

            return b;
        }

        byte[] hash(string org)
        {
            byte[] b = StringToBytes(org);

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(b);

            return result;
        }

        public static bool LogInAuto(string sRegisterInfoUrl, string sRegisterInfoText, bool bDisplayRegisterInfoUrl)
        {
            if (Config.bAutoLogin)
            {
                if(LogIn(Config.sUserName, Config.aPassCode))   return true;
            }

            UsernameLibrary.FormLogIn dialog = new UsernameLibrary.FormLogIn();
            dialog.sRegisterInfoUrl = sRegisterInfoUrl;
            dialog.sRegisterInfoText = sRegisterInfoText;
            dialog.bDisplayRegisterInfoUrl = bDisplayRegisterInfoUrl;
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(Form.ActiveForm) != System.Windows.Forms.DialogResult.OK)
            {
                return false;
            }

            return true;
        }

        static int LogInToServer(string username, byte[] passcode, bool bSecondary, out string err_msg)
        {
            err_msg = "";

            try
            {
                bool retn;

                ServiceUsername2.ServiceUsername2 service = new UsernameLibrary.ServiceUsername2.ServiceUsername2();

                if (bSecondary)
                    service.Url = String.Format("http://www.username.kr/Service/ServiceUsername2.asmx");
                else
                    service.Url = String.Format("http://www.username.co.kr/Service/ServiceUsername2.asmx");

                string seed = Guid.NewGuid().ToString();

                string siteid_hash = StringHash.Encode("UsernameLibrary.dll", seed);
                string username_hash = StringHash.Encode(username, seed);
                byte[] hash = HashTool.MakeHash(siteid_hash + username_hash + seed + "LogIn");

                retn = service.LogIn(siteid_hash, username_hash, passcode, Thread.CurrentThread.CurrentUICulture.Name, hash, seed, out err_msg);

                return retn ? 1 : 0;    

                /*
                int retn;

                kr.co.username.www.ServiceUsername service = new kr.co.username.www.ServiceUsername();

                if(bSecondary)
                    service.Url = String.Format("http://www.username.kr/Service/ServiceUsername.asmx");
                else
                    service.Url = String.Format("http://www.username.co.kr/Service/ServiceUsername.asmx");

                retn = service.LogIn(username, passcode);

                if (retn == 99998)
                {
                    return true;
                }
                else if (retn == 1)
                {
                    if (NetTools.Tools.IsLangKorean())
                        err_msg = "사용자명이나 암호가 틀립니다.";
                    else
                        err_msg = "Error:Username or password is invalid.";
                }
                else if (retn == 2)
                {
                    if (NetTools.Tools.IsLangKorean())
                        err_msg = "암호 연속 오류로 접속이 일시 제한되어 있습니다.";
                    else
                        err_msg = "Error:Bacause you inputed continuing wrong password, Access is temporarily denied.";
                }
                else
                {
                    err_msg = "Connection Failed";
                }*/
            }
            catch (Exception exception)
            {
                err_msg = String.Format("Connection Failed\nMessage={0}", exception.Message);
            }

            return -1;  // 
        }

        static bool LogIn(string username, byte[] passcode)
        {
            string err_msg;

            int retn = LogInToServer(username, passcode, false, out err_msg);

            if (retn == 1) return true;
            // 0은 접속은 정상적으로 되고 로그인 조건이 안되는 것이므로 보조서버를 호출할 필요가 없다.
            if (retn == 0)
            {
                goto go_err_msg;
            }
            
            // -1인 경우는 접속이 안되는 경우이므로 보조 서버와 통신해 본다.

            retn = LogInToServer(username, passcode, true, out err_msg); 
            if (retn == 1) return true;

        go_err_msg:

            err_msg = err_msg.Replace("\\r\\n", "\n");  // JAVA용 메시지이므로 윈도우즈에 맞게 수정한다.

            if (NetTools.Tools.IsLangKorean())
                MessageBox.Show(err_msg, "접속 결과");
            else
                MessageBox.Show(err_msg, "Connection Result");

            return false;

            /*
            try
            {
                int retn;

                kr.co.username.www.ServiceUsername service = new kr.co.username.www.ServiceUsername();

                service.Url = service.Url;

                retn = service.LogIn(username, passcode);

                if (retn == 99998)
                {
                    return true;
                }
                else if (retn == 1)
                {
                    if(NetTools.Tools.IsLangKorean())
                        MessageBox.Show("사용자명이나 암호가 틀립니다.", "접속 결과");
                    else
                        MessageBox.Show("Error:Username or password is invalid.", "Connection Result");
                }
                else if (retn == 2)
                {
                    if (NetTools.Tools.IsLangKorean())
                        MessageBox.Show("암호 연속 오류로 접속이 일시 제한되어 있습니다.", "접속 결과");
                    else
                        MessageBox.Show("Error:Bacause you inputed continuing wrong password, Access is temporarily denied.", "Connection Result");
                }
                else
                {
                    MessageBox.Show("Connection Failed", "Connection Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exception)
            {
                string msg;
                msg = String.Format("Connection Failed\nMessage={0}", exception.Message);
                MessageBox.Show(msg, "Test Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;*/
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //ClassStatus.UnInit();

            if (this.textBoxPassword.Text != init_password)
            {
                PassCode = hash(this.textBoxPassword.Text);
            }

            if (LogIn(this.textBoxUsername.Text, PassCode))
            {
                Config.bSaveUsername = this.checkBoxSaveUsername.Checked;
                Config.bSavePassword = this.checkBoxSavePassword.Checked;
                Config.bAutoLogin = this.checkBoxAutoLogIn.Checked;
                Config.sUserName = this.textBoxUsername.Text;
                if (this.textBoxPassword.Text != init_password)
                {
                    byte[] result = hash(this.textBoxPassword.Text);
                    Config.aPassCode = result;
                }
                Config.SaveConfig();

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public bool bDisplayRegisterInfoUrl = true;
        public string sRegisterInfoUrl;
        public string sRegisterInfoText;

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url;

            if (sRegisterInfoUrl == null || sRegisterInfoUrl.Length == 0)
            {
                url = "http://www.username.co.kr";
            }
            else
            {
                url = sRegisterInfoUrl;
            }

            System.Diagnostics.Process.Start(url); // 2022-4-12 변경. Edge와 IExplore가 두개가 뜬다는 이야기가 있어서... // System.Diagnostics.Process.Start("IExplore", url);
        }

        private void checkBoxSavePassword_CheckedChanged(object sender, EventArgs e)
        {
            if(!this.checkBoxSavePassword.Checked) {
                this.textBoxPassword.Text = "";
            }
        }
    }
}
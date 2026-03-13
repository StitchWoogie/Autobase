using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoLibLocal;
using PortalServerWeb.Library;
using NetTools;
using System.Text;
using System.Security.Cryptography;

namespace PortalServerWeb.AutoWeb.WebPages
{
    public partial class LogOn : System.Web.UI.Page
    {
        public static bool CheckLogOn(System.Web.UI.Page page)
        {
            object obj = page.Session["bWebAdminLogOn"];

            if (obj != null)
            {
                bool flag = (bool)obj;

                if (flag) return true;
            }

            string url = String.Format("LogOn.aspx?RedirectPage={0}", page.Request.Url.PathAndQuery);
            page.Response.Redirect(url);
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HttpCookie modulesCookie;

                modulesCookie = Request.Cookies["WebAdminUsername"];
                if (modulesCookie != null)
                {
                    this.TextBoxUsername.Text = modulesCookie.Value;
                }

                /*
                HttpCookie modulesCookie = Request.Cookies["saveid"];
                if (modulesCookie != null && String.Compare(modulesCookie.Value, "true", true) == 0)
                {
                    this.CheckBoxSaveID.Checked = true;
                    modulesCookie = Request.Cookies["username"];
                    if (modulesCookie != null)
                    {
                        this.TextBoxUsername.Text = modulesCookie.Value;
                    }
                }*/
            }
        }

        protected void CommandOK_Click(object sender, EventArgs e)
        {
            if (!IsValid) return;

            Session["bWebAdminLogOn"] = true;


            DateTime tExpire = DateTime.Now.AddDays(365);

            Response.Cookies["WebAdminUsername"].Value = this.TextBoxUsername.Text;
            Response.Cookies["WebAdminUsername"].Expires = tExpire;

            /*
            DateTime tExpire = DateTime.Now.AddDays(365);

            Response.Cookies["username"].Value = this.TextBoxUsername.Text;
            Response.Cookies["username"].Expires = tExpire;

            Response.Cookies["saveid"].Value = this.CheckBoxSaveID.Checked.ToString();
            Response.Cookies["saveid"].Expires = tExpire;*/

            Response.Redirect(Request["RedirectPage"]);
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string username = this.TextBoxUsername.Text.Trim();
            if (username.Length == 0)
            {
                this.CustomValidator1.Text = "사용자 명을 입력하세요.";
                args.IsValid = false;
                return;
            }

            string admin_username = ConfigWeb.WebAdminUsername();
            string admin_password = ConfigWeb.WebAdminPassword();

            if (admin_username == null || admin_username.Length == 0)
            {
                this.CustomValidator1.Text = "웹서버 설정에서 웹관리자 명을 지정하지 않았습니다.";
                args.IsValid = false;
                return;
            }

            if (admin_password == null || admin_password.Length == 0)
            {
                this.CustomValidator1.Text = "웹서버 설정에서 웹관리자 암호를 지정하지 않았습니다.";
                args.IsValid = false;
                return;
            }

            string item_count = "FailCount_" + username;
            string item_time = "FailTime_" + username;

            int fail_count = ConfigApplication.GetInt32(this.Application, item_count);

            if (fail_count >= 5)
            {
                DateTime tLast = ConfigApplication.GetDateTime(this.Application, item_time);
                DateTime t = DateTime.Now;

                TimeSpan ts = t - tLast;

                int MAX_WAIT = 60;

                if (ts.TotalSeconds < MAX_WAIT)
                {
                    this.CustomValidator1.Text = String.Format("로그인을 연속으로 실패({0}회)해서 {1}초 후에 다시 시도할 수 있습니다.", fail_count, (int)(MAX_WAIT - ts.TotalSeconds));
                    args.IsValid = false;
                    return;
                }
            }

            byte[] hash_s = Encoding.UTF8.GetBytes(admin_password);
            // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
            // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
            SHA256 sha = new SHA256Managed();
            byte[] result = sha.ComputeHash(hash_s);
            StringBuilder result_s = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                result_s.AppendFormat("{0:x02}", result[i]);
            }

            string err_msg;
                                   
            if (String.Compare(username, admin_username, true) != 0 ||
                String.Compare(this.TextBoxPassword.Text, "Hashed_"+result_s.ToString()) != 0)
            {
                fail_count++;

                ConfigApplication.SetValue(Application, item_count, fail_count);
                ConfigApplication.SetValue(Application, item_time, DateTime.Now);

                if(Tools.IsLangKorean())
                    this.CustomValidator1.Text = "웹관리자명이 다르거나 암호가 다릅니다.";
                else
                    this.CustomValidator1.Text = "Web administrator's username or password is mismatched.";

                args.IsValid = false;

                string msg = String.Format("Username:{0} {1} FailCount={2} IP={3}", username, this.CustomValidator1.Text, fail_count, Context.Request.UserHostAddress);

                ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, msg);

                return;
            }

            ConfigApplication.SetValue(Application, item_count, 0);    // Fail Count를 클리어한다.

            if(Tools.IsLangKorean())
                ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, "웹관리자 로그인 Username={0} IP={1}", username, Context.Request.UserHostAddress);
            else
                ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, "Web administrator logged in. Username={0} IP={1}", username, Context.Request.UserHostAddress);
        }

        protected void TextBoxPasswordHide_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

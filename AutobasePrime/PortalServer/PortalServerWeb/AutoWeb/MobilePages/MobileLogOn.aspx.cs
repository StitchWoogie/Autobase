using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Mobile;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AutoLibLocal;

namespace PortalServerWeb.AutoWeb.MobilePages
{
    public partial class MobileLogOn : System.Web.UI.Page
    {
        public static bool CheckLogOn(System.Web.UI.Page page)
        {
            object obj = page.Session["bMobileLogOn"];

            if (obj != null)
            {
                bool flag = (bool)obj;

                if (flag) return true;
            }

            string url = String.Format("MobileLogOn.aspx?RedirectPage={0}", page.Request.Url.PathAndQuery);
            page.Response.Redirect(url);
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HttpCookie modulesCookie;

                modulesCookie = Request.Cookies["username"];
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

            Session["bMobileLogOn"] = true;


            DateTime tExpire = DateTime.Now.AddDays(365);

            Response.Cookies["username"].Value = this.TextBoxUsername.Text;
            Response.Cookies["username"].Expires = tExpire;

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

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            UserInfoStruct user = new UserInfoStruct();
            string err_msg;
            string filename = String.Format("{0}\\Users\\{1}.user", work_dir, username);

            if (!user.LoadUser(out err_msg, filename, username))
            {
                this.CustomValidator1.Text = "이름이나 암호가 틀립니다.";
                args.IsValid = false;
                return;
            }

            if (user.sPassCode != UserInfoStruct.ZipPassword(username, this.TextBoxPassword.Text))
            {
                this.CustomValidator1.Text = "이름이나 암호가 틀립니다.";
                args.IsValid = false;
                return;
            }
        }
        protected void TextBoxUsername_TextChanged(object sender, EventArgs e)
        {

        }
}
}

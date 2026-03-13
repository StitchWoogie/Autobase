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

namespace PortalServerWeb.AutoWeb.MobilePages
{

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PortalServerWeb.AutoWeb.MobilePages.MobileLogOn.CheckLogOn(this))
            {
                return;
            }


        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PortalServerWeb.Library;

namespace PortalServerWeb.AutoWeb.WebPages
{
    public partial class ViewLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!LogOn.CheckLogOn(this))
            {
                return;
            }
            //if (!IsPostBack)
            //{
                this.GridView1.DataSource = ClassMain.GetDataTable("SELECT * FROM ScadaLog ORDER BY LogTime DESC");
                this.GridView1.DataBind();
            //}
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            this.GridView1.DataBind();
        }
    }
}

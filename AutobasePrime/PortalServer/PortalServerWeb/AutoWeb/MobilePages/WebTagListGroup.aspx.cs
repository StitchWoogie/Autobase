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
using System.Collections.Generic;

namespace PortalServerWeb.AutoWeb.MobilePages
{
    public partial class WebTagListGroup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PortalServerWeb.AutoWeb.MobilePages.MobileLogOn.CheckLogOn(this))
            {
                return;
            }

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            TotalConfig.sDirWorkProject = work_dir;

            List<WebTagList> array = WebGroupList.Load();
            WebTagList list;
            ListItem item;

            for (int i = 0; i < array.Count; i++)
            {
                list = (WebTagList)array[i];
                item = new ListItem();

                if (list.description.Length > 0)
                    item.Text = String.Format("{0}  ({1})", list.name, list.description);
                else
                    item.Text = String.Format("{0}", list.name);

                item.Value = String.Format("WebTagListItem.aspx?name={0}&des={1}", Server.UrlEncode(list.name), Server.UrlEncode(list.description));

                this.List1.Items.Add(item);
            }

        }
        protected void List1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.List1.SelectedIndex == -1)  return;

            Response.Redirect(this.List1.SelectedItem.Value);
        }
}
}
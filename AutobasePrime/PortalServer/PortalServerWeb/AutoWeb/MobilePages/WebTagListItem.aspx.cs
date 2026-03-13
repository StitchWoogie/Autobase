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
    public partial class WebTagListItem : System.Web.UI.Page {


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
            string group_name = Request["name"];
            string group_des = Request["des"];

            this.LabelGroupName.Text = String.Format("그룹명:{0}", group_name);
            this.LabelDescription.Text = String.Format("설명:{0}", group_des);

            for (int i = 0; i < array.Count; i++)
            {
                list = (WebTagList)array[i];

                if (list.name == group_name)
                {
                    ListItem item;
                    TagPublicClass tp;
                    string tag;
                    int[] tag_pos = new int[1];

                    TagFile file = new TagFile();
                    string filename = String.Format("{0}\\Tag\\local.tagx", work_dir);
                    TagGrClass gr = new TagGrClass();
                    file.LoadTag(filename, gr, System.Text.Encoding.UTF8, false);

                    for (int j = 0; j < list.member.Count; j++)
                    {
                        tag = (string)list.member[j];
                        item = new ListItem();
                        tp = TagLib.GetStructPublic(gr, tag, ref tag_pos);
                        item.Text = String.Format("{0}  ({1})", tag, tp.description);

                        item.Value = String.Format("TagView.aspx?tag={0}&des={1}", Server.UrlEncode(tag), Server.UrlEncode(tp.description));

                        this.List1.Items.Add(item);
                    }

                    break;
                }
            }

        }
        protected void List1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.List1.SelectedIndex == -1) return;

            Response.Redirect(this.List1.SelectedItem.Value);
        }
}

}
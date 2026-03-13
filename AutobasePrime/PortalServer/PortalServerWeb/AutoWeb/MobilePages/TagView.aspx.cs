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
using PortalServerWeb.AutoWeb.Service;

namespace PortalServerWeb.AutoWeb.MobilePages
{

    public partial class TagView : System.Web.UI.Page
    {



        public static string GetTagValue(string tag)
        {
            WcfServiceDataTag service = new WcfServiceDataTag();

            ArrayList array = new ArrayList();

            array.Add(tag);
            DataSet ds;
            string curr = null;

            try
            {
                ds = service.GetTagValueList(array);
            }
            catch
            {
                ds = null;
            }

            if (ds != null)
            {
                DataRow row;
                row = ds.Tables[0].Rows[0];
                curr = row["curr"].ToString();
            }

            return curr;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PortalServerWeb.AutoWeb.MobilePages.MobileLogOn.CheckLogOn(this))
            {
                return;
            }

            if (!IsPostBack)
            {
                string tag = Request["Tag"];

                string work_dir = ProjectLib.GetWorkDir(Context.Request);

                TotalConfig.sDirWorkProject = work_dir;
                TagFile file = new TagFile();
                string filename = String.Format("{0}\\Tag\\local.tagx", work_dir);
                TagPublicClass tp = file.LoadTagOne(filename, tag);

                this.LabelTag.Text = String.Format("태그:{0}", tag);
                this.LabelDescription.Text = String.Format("설명:{0}", tp == null ? "" : tp.description);

                string curr = GetTagValue(tag);
                double value = 0;

                if (curr != null)
                {
                    value = NetTools.ConvertTool.ToDouble(curr);
                }

                this.LabelValue.Text = String.Format("현재값:{0}", value);

                if (tp != null && tp.enumTagType == EnumTagType.DI)
                {
                    this.LinkWrite.NavigateUrl = String.Format("TagWriteBit.aspx?tag={0}", Server.UrlEncode(tag));
                }
                else
                {
                    this.LinkWrite.NavigateUrl = String.Format("TagWriteWord.aspx?tag={0}", Server.UrlEncode(tag));
                }

                this.LinkList.NavigateUrl = "default.aspx";
            }
        }

        protected void CommandOK_Click(object sender, EventArgs e)
        {

        }
    }

}
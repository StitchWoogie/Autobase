using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using System.IO;
using AutoLib;
using NetTools;

namespace ViewMain
{
    public partial class FormSelectSite : Form
    {
        public static List<SiteItem> arraySites = new List<SiteItem>();

        public FormSelectSite()
        {
            InitializeComponent();
        }

        void FillDefault()
        {
            SiteItem si;

            si = new SiteItem();
            si.sSiteName = "Local";
            si.sSiteUrl = "Local";
            arraySites.Add(si);

            si = new SiteItem();
            si.sSiteName = "Demo1 Site";
            si.sSiteUrl = "http://demo1.autobase.biz";
            arraySites.Add(si);

            si = new SiteItem();
            si.sSiteName = "Demo2 Site";
            si.sSiteUrl = "http://demo2.autobase.biz";
            arraySites.Add(si);

            si = new SiteItem();
            si.sSiteName = "Demo1 Site SSL";
            si.sSiteUrl = "https://demo1.autobase.biz";
            arraySites.Add(si);


            /*
            this.comboBoxSite.Items.Add("http://demo1.autobase.biz");
            this.comboBoxSite.Items.Add("http://demo2.autobase.biz");
            this.comboBoxSite.Items.Add("https://demo1.autobase.biz");
            */
        }

        void SiteListLoad()
        {
            arraySites.Clear();

            string folder = String.Format("{0}\\ViewMain", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            string filename = String.Format("{0}\\{1}", folder, "\\SiteLists.lst");

            if (!File.Exists(filename))
            {
                FillDefault();
                return;
            }

            TextReader reader = new StreamReader(filename);

            if (reader == null)
            {
                FillDefault();
                return;
            }

            string buf;
            CommaTextReader comma = new CommaTextReader();
            SiteItem si;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);

                si = new SiteItem();
                si.sSiteName = comma.GetString();
                si.sSiteUrl = comma.GetString();

                arraySites.Add(si);
            }

            reader.Close();

            if(arraySites.Count == 0)
                FillDefault();
        }

        void SiteListSave()
        {
            string folder = String.Format("{0}\\ViewMain", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filename = String.Format("{0}\\{1}", folder, "\\SiteLists.lst");

            CommaTextWriter writer = new CommaTextWriter(filename);

            if (writer == null) return;

            SiteItem si;

            for (int i = 0; i < arraySites.Count; i++)
            {
                si = arraySites[i];

                writer.Write("{0},", si.sSiteName);
                writer.Write("{0},", si.sSiteUrl);
                writer.WriteLine();
            }

            writer.Close();
        }


        void ChangeItem(ListViewItem lvi, SiteItem si)
        {
            lvi.SubItems[0].Text = si.sSiteName;
            lvi.SubItems[1].Text = si.sSiteUrl;
        }

        void FillListBox()
        {
            SiteItem si;
            ListViewItem lvi;

            for (int i = 0; i < arraySites.Count; i++)
            {
                si = arraySites[i];

                lvi = new ListViewItem(si.sSiteName);

                lvi.SubItems.Add(si.sSiteUrl);

                listView1.Items.Add(lvi);

                ChangeItem(lvi, si);

                if (ConfigVarTotal.bLocalFlag)
                {
                    if (String.Compare(si.sSiteUrl, "Local", true) == 0)
                        lvi.Selected = true;
                }
                else
                {
                    if (String.Compare(si.sSiteUrl, ConfigVarTotal.MakeRootUrl(), true) == 0)
                        lvi.Selected = true;
                }
            }
        }

        private void FormSelectSite_Load(object sender, EventArgs e)
        {
            SiteListLoad();
            FillListBox();

            /*
            if (ConfigVarTotal.bLocalFlag)
                this.comboBoxSite.Text = "Local";
            else
            {
                string text = ConfigVarTotal.MakeRootUrl();

                this.comboBoxSite.Text = text;
            }*/

            this.checkBoxCheckProject.Checked = ConfigWebView.bCheckProjectEveryConnection;
        }

        public string sSelectedSite = "";

        void OnOK()
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a site to connection.", "Site");
                return;
            }

            string url = listView1.SelectedItems[0].SubItems[1].Text;

            // 여기서 ConfigVarTotal.eServiceType 을 WcfService인지 WebService 인지를 결정해야 한다.
            // 일단 http 이면 WebService net.tcp 이면 WcfService로 연결하도록 했다.
            // 나중에 Service종류를 선택할 수 있도록 해야한다. 2016-12-22
            if (String.Compare(url, 0, "net.tcp", 0, 7, true) == 0)
            {
                ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
            }
            else
            {
                ConfigVarTotal.eServiceType = EnumServiceType.WebService;
            }

            if (String.Compare("Local", url, true) == 0)
                sSelectedSite = url;
            else
                sSelectedSite = "Site=" + url;

            /*
            for (int i = 0; i < this.comboBoxSite.Items.Count; i++)
            {
                if (String.Compare(this.comboBoxSite.Text, (string)this.comboBoxSite.Items[i], true) == 0) 
                    goto ok_same_found;
            }
            this.comboBoxSite.Items.Add(this.comboBoxSite.Text);

        ok_same_found:*/

            ConfigWebView.bCheckProjectEveryConnection = this.checkBoxCheckProject.Checked;
            ConfigWebView.Save();

            

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OnOK();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnOK();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormSelectSiteAdd dialog = new FormSelectSiteAdd();

            SiteItem si = new SiteItem();

            dialog.Set(si);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.Get(si);

                ListViewItem lvi = new ListViewItem();
                lvi.SubItems.Add("");
                ChangeItem(lvi, si);
                lvi.Selected = true;
                listView1.Items.Add(lvi);
                arraySites.Add(si);

                SiteListSave();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a site to delete.", "Site");
                return;
            }

            ListViewItem lvi = listView1.SelectedItems[0];

            string msg = String.Format("Are you sure you want to delete the selected site?");

            if (MessageBox.Show(msg, lvi.SubItems[0].Text, MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            arraySites.RemoveAt(lvi.Index);
            listView1.Items.RemoveAt(lvi.Index);

            SiteListSave();
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a site to modify.", "Site");
                return;
            }

            ListViewItem lvi = listView1.SelectedItems[0];

            FormSelectSiteAdd dialog = new FormSelectSiteAdd();

            if (Tools.IsLangKorean())
                dialog.Text = "사이트 수정";
            else
                dialog.Text = "Site Modify";

            SiteItem si = arraySites[lvi.Index];

            dialog.Set(si);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.Get(si);

                ChangeItem(lvi, si);

                SiteListSave();
            }
        }
    }

    public class SiteItem
    {
        public string sSiteName;
        public string sSiteUrl;
    }
}

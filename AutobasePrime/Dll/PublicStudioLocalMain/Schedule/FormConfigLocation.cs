using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using NetTools;

namespace PublicStudioLocalMain.Schedule
{
    public partial class FormConfigLocation : Form
    {
        public FormConfigLocation()
        {
            InitializeComponent();

            arrayCities = LocationLib.LoadProjectLocationList();
        }

        bool IsContryCodeExist(List<ContryCodeItem> array, string code)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (code == array[i].code) return true;
            }

            return false;
        }

        int SortContry(ContryCodeItem array1, ContryCodeItem array2)
        {
            return String.Compare(array1.name, array2.name);
        }

        List<ContryCodeItem> GetContryList()
        {
            List<ContryCodeItem> array = new List<ContryCodeItem>();

            CultureInfo[] _cultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in _cultureInfo)
            {
                RegionInfo ri = new RegionInfo(culture.LCID);

                if (!IsContryCodeExist(array, ri.Name))
                {
                    ContryCodeItem cci = new ContryCodeItem();
                    cci.name = ri.DisplayName;
                    cci.code = ri.Name;
                    array.Add(cci);
                }
            }

            array.Sort(SortContry);

            return array;
        }

        string GetSelectedContryCode()
        {
            int index = this.comboBoxNation.SelectedIndex;

            if (index != -1) return arrayContries[index].code;

            return "KR";
        }

        void UpdateLocationList()
        {
            this.listViewLocations.Items.Clear();

            string country_code = GetSelectedContryCode();

            ListViewItem lvi;
            LocationItem item;

            for (int i = 0; i < arrayCities.Count; i++)
            {
                item = arrayCities[i];

                if (item.sNationalCode != country_code) continue;    // 정확한 나라 정보가 없으면 모두 보여준다.

                lvi = new ListViewItem(item.sCity);
                lvi.SubItems.Add(item.sLatitude);
                lvi.SubItems.Add(item.sLongitude);

                this.listViewLocations.Items.Add(lvi);
            }
        }

        List<ContryCodeItem> arrayContries;
        List<LocationItem> arrayCities;

        private void FormConfigLocation_Load(object sender, EventArgs e)
        {
            RegionInfo ri = RegionInfo.CurrentRegion;
            arrayContries = GetContryList();

            for (int i = 0; i < arrayContries.Count; i++)
            {
                this.comboBoxNation.Items.Add(arrayContries[i].name);
                if (ri.Name == arrayContries[i].code)
                    this.comboBoxNation.SelectedIndex = i;
            }
        }

        private void listViewLocations_DoubleClick(object sender, EventArgs e)
        {
            OK();
        }

        public string sSelectedCity;

        void OK()
        {
            if(this.listViewLocations.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the item.", "Selection Error");
                return;
            }

            ListViewItem lvi = this.listViewLocations.SelectedItems[0];

            int index = GetLocationPositionByCityName(arrayCities, lvi.SubItems[0].Text);
            if (index == -1) return;

            LocationItem item = arrayCities[index];

            sSelectedCity = item.sCity;

            DialogResult = DialogResult.OK;
            Close();

            if (bModified)
            {
                LocationLib.arrayLocation = arrayCities;
                LocationLib.SaveLocationList();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK();
        }

        bool bModified = false;

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormConfigLocationAdd dialog = new FormConfigLocationAdd();

            LocationItem item = new LocationItem();

            item.sNationalCode = GetSelectedContryCode();
            dialog.SetStruct(item);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.GetStruct(item);

                ListViewItem lvi = new ListViewItem(item.sCity);
                lvi.SubItems.Add(item.sLatitude);
                lvi.SubItems.Add(item.sLongitude);
                lvi.Selected = true;
                lvi.EnsureVisible();

                this.listViewLocations.Items.Add(lvi);
                arrayCities.Add(item);

                bModified = true;
            }
        }

        int GetLocationPositionByCityName(List<LocationItem> array, string city)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].sCity == city) return i;
            }

            return -1;
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            if (this.listViewLocations.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the item to modify.", "Selection error");
                return;
            }

            ListViewItem lvi = this.listViewLocations.SelectedItems[0];

            int index = GetLocationPositionByCityName(arrayCities, lvi.SubItems[0].Text);
            if(index == -1) return;

            LocationItem item = arrayCities[index];

            FormConfigLocationAdd dialog = new FormConfigLocationAdd();

            dialog.SetStruct(item);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.GetStruct(item);

                lvi.SubItems[0].Text = item.sCity;
                lvi.SubItems[1].Text = item.sLatitude;
                lvi.SubItems[2].Text = item.sLongitude;

                bModified = true;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (this.listViewLocations.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the item to delete.", "Selection error");
                return;
            }

            ListViewItem lvi = this.listViewLocations.SelectedItems[0];

            int index = GetLocationPositionByCityName(arrayCities, lvi.SubItems[0].Text);
            if (index == -1) return;

            arrayCities.RemoveAt(index);
            this.listViewLocations.Items.RemoveAt(lvi.Index);

            bModified = true;
        }

        private void comboBoxNation_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLocationList();
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            
        }

        private void FormConfigLocation_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void buttonGoogleMap_Click(object sender, EventArgs e)
        {
            if (this.listViewLocations.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a item to view.", "Selection error");
                return;
            }

            ListViewItem lvi = this.listViewLocations.SelectedItems[0];

            int index = GetLocationPositionByCityName(arrayCities, lvi.SubItems[0].Text);
            if(index == -1) return;

            LocationItem item = arrayCities[index];

            double latitude, longitude;
            LocationLib.CalcLocationInfomationByTitle(arrayCities, lvi.SubItems[0].Text, out latitude, out longitude);

            // https://maps.google.com/?ll=37.020098,-95.625&z=5
            //string argument = String.Format("https://maps.google.com/?ll={0},{1}&t=m&z=6", latitude, longitude); 

            //https://maps.google.com/maps?q=37.5,127&hl=vi&sll=37.5,127&sspn=14.141349,33.815918&t=m&z=12
            string argument = String.Format("https://maps.google.com/maps?q={0},{1}&hl=vi&z=8", latitude, longitude);

            System.Diagnostics.Process.Start(argument); // 2022-4-12 변경.Edge와 IExplore가 두개가 뜬다는 이야기가 있어서...//Process.Start("IExplore.exe", argument);
        }

        private void buttonSetDefault_Click(object sender, EventArgs e)
        {
            if (Tools.IsLangKorean())
            {
                if (MessageBox.Show("위치 리스트를 기본값으로 초기화 할까요?", "기본 목록", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            }
            else
            {
                if (MessageBox.Show("Restore to default location list?", "Dafault List", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            }

            arrayCities = LocationLib.LoadDefaultLocationList();

            UpdateLocationList();

            bModified = true;
        }
    }

    class ContryCodeItem
    {
        public string name;
        public string code;
    }
}

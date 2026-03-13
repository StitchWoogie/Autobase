using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BasicScreen.kdymain
{
    public partial class ViewListAlarmDetailItemDetail : Form
    {
        public ViewListAlarmDetailItemDetail()
        {
            InitializeComponent();
        }

        private void ViewListAlarmDetailItemDetail_Load(object sender, EventArgs e)
        {

        }

        public void Set(DataSet ds, DataRow row)
        {
            this.textBoxTime.Text = row["alarm_datetime"].ToString();
            this.textBoxTag.Text = row["tag_name"].ToString();
            this.textBoxDescription.Text = row["description"].ToString();
            this.textBoxMessage.Text = row["message"].ToString();
            this.textBoxType.Text = row["alarm_type"].ToString();
            this.textBoxPriority.Text = row["priority"].ToString();
            this.textBoxPort.Text = row["port"].ToString();
            this.textBoxStation.Text = row["station"].ToString();
            this.textBoxAddress.Text = row["address"].ToString();
            this.textBoxSubType.Text = row["sub_type"].ToString();

            // User/IP/Computer 는 10.2.1 부터 추가되었다.
            int index = ds.Tables[0].Columns.IndexOf("username");
            if(index != -1) 
                this.textBoxUser.Text = row[index].ToString();
            index = ds.Tables[0].Columns.IndexOf("ip_address");
            if (index != -1)
                this.textBoxIP.Text = row[index].ToString();
            index = ds.Tables[0].Columns.IndexOf("computer_name");
            if (index != -1)
                this.textBoxComputer.Text = row[index].ToString();
        }
    }
}

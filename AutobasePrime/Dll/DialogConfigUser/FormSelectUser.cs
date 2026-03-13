using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;

namespace DialogConfigUser
{
    public partial class FormSelectUser : Form
    {
        public string sSelectedUser;
        List<UserInfoStruct> arrayUsers;

        public FormSelectUser(List<UserInfoStruct> array)
        {
            InitializeComponent();

            arrayUsers = array;
        }

        void OK()
        {
            if (this.listViewUser.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select user to add.", "Selection error");
                return;
            }

            sSelectedUser = this.listViewUser.SelectedItems[0].SubItems[0].Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK();
        }

        private void listViewUser_DoubleClick(object sender, EventArgs e)
        {
            OK();
        }

        private void FormSelectUser_Load(object sender, EventArgs e)
        {
            ListViewItem lvi;
            for (int i = 0; i < arrayUsers.Count; i++)
            {
                lvi = new ListViewItem(arrayUsers[i].sUsername);
                lvi.SubItems.Add(arrayUsers[i].sDescription);
                this.listViewUser.Items.Add(lvi);
            }
        }
    }
}

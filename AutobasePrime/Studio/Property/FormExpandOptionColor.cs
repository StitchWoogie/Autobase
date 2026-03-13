using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using System.Collections;
using GraphicModule;

namespace Studio
{
    public partial class FormExpandOptionColor : Form
    {
        public FormExpandOptionColor()
        {
            InitializeComponent();

            PropertyPageObjectTagAnimation.TagConditionPlayFillToCombo(this.comboBoxCondition);
        }

        public void SetExpand(object obj)
        {
            

            if (obj == null) return;

            GraphicModule.ExpandBasicColor basic = (GraphicModule.ExpandBasicColor)obj;

            tempArray = (ArrayList)Tools.CopyObject(basic.member);

            this.listView1.Items.Clear();

            ExpandBasicColorMember member;
            for (int i = 0; i < tempArray.Count; i++)
            {
                member = (ExpandBasicColorMember)tempArray[i];

                AddNewItem(member);
            }

            if (tempArray.Count > 0)
            {
                this.listView1.Items[0].Selected = true;
            }
        }

        public object GetExpand()
        {
            GraphicModule.ExpandBasicColor basic = new GraphicModule.ExpandBasicColor();

            basic.member = tempArray;

            // 원래 form_closed에서 해야하는데 closing closed 모두 발생안하다.
            TotalConfig.AutoBaseListCtrlConfigSave(this.listView1, "Studio", "ExpandOptionColor");

            return basic;
        }

        private void buttonTag_Click(object sender, EventArgs e)
        {
            string tag;
            string des;

            if (DialogTag.SelectTag.SelectAiDiSt(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxTag.Text = tag;
            }
        }

        private void FormExpandOptionColor_Load(object sender, EventArgs e)
        {
            TotalConfig.AutoBaseListCtrlConfigLoad(this.listView1, "Studio", "ExpandOptionColor");

            EnableDisable();
        }

        ArrayList tempArray = new ArrayList();

        ListViewItem AddNewItem(ExpandBasicColorMember member)
        {
            ListViewItem lvi = new ListViewItem("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.UseItemStyleForSubItems = false;
            
            ChangeItem(lvi, member);

            

            this.listView1.Items.Add(lvi);

            return lvi;
        }

        void ChangeItem(ListViewItem lvi, ExpandBasicColorMember member)
        {
            //lvi.UseItemStyleForSubItems = true;

            if (Tools.IsLangKorean())
                lvi.SubItems[0].Text = member.active ? "사용" : "사용안함";
            else
                lvi.SubItems[0].Text = member.active ? "Use" : "Not Used";


            string tag = member.sTag;
            if (member.sTag == null || member.sTag.Length == 0)
            {
                tag = "???";
            }

            if (member.condition <= 4)
            {
                lvi.SubItems[1].Text = tag + " " + PropertyPageObjectTagAnimation.sCommand[member.condition] + " " + member.value.ToString();
            }
            else if (member.condition == 5 || member.condition == 6)
            {
                lvi.SubItems[1].Text = tag + " " + "=" + " " + PropertyPageObjectTagAnimation.sCommand[member.condition];
            }
            else if (member.condition == 7) // hihi
            {
                lvi.SubItems[1].Text = tag + " " + ">=" + " " + PropertyPageObjectTagAnimation.sCommand[member.condition];
            }
            else if (member.condition == 8) // high
            {
                lvi.SubItems[1].Text = tag + " " + ">=" + " " + PropertyPageObjectTagAnimation.sCommand[member.condition];
            }
            else if (member.condition == 9) // low
            {
                lvi.SubItems[1].Text = tag + " " + "<=" + " " + PropertyPageObjectTagAnimation.sCommand[member.condition];
            }
            else if (member.condition == 10)    // lolo
            {
                lvi.SubItems[1].Text = tag + " " + "<=" + " " + PropertyPageObjectTagAnimation.sCommand[member.condition];
            }
            else if (member.condition == 11)    // alarm
            {
                lvi.SubItems[1].Text = tag + " " + "=" + " " + PropertyPageObjectTagAnimation.sCommand[member.condition];
            }
            else if (member.condition == 12)
            {
                lvi.SubItems[1].Text = tag + " " + PropertyPageObjectTagAnimation.sCommand[member.condition] + " " + member.value.ToString();
            }
            else
            {
                lvi.SubItems[1].Text = tag + " " + "?" + " " + member.value.ToString();
            }

            //lvi.SubItems[2].ForeColor = member.color;
            lvi.SubItems[2].Text = member.color.Name;
            lvi.SubItems[2].BackColor = member.color;
        }

        bool bInitial = false;

        void DialogToArray()
        {
            if (bInitial) return;   // 초기화 중.

            if (this.listView1.SelectedItems.Count == 0) return;

            ListViewItem lvi = this.listView1.SelectedItems[0];
            int index = lvi.Index;

            ExpandBasicColorMember member;
            member = (ExpandBasicColorMember)tempArray[index];

            member.active = this.checkBoxActive.Checked;
            member.sTag = this.textBoxTag.Text;
            member.condition = this.comboBoxCondition.SelectedIndex;
            member.value = this.textBoxValue.Text;
            member.color = this.buttonColor.BackColor;

            ChangeItem(lvi, member);
        }

        void ArrayToDialog()
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                this.checkBoxActive.Checked = false;
                this.textBoxTag.Text = "";
                this.buttonColor.BackColor = Color.White;
                this.textBoxValue.Text = "";
                this.comboBoxCondition.SelectedIndex = -1;
                return;
            }

            bInitial = true;

            int index = this.listView1.SelectedItems[0].Index;

            ExpandBasicColorMember member;
            member = (ExpandBasicColorMember)tempArray[index];

            this.checkBoxActive.Checked = member.active;
            this.textBoxTag.Text = member.sTag;
            this.comboBoxCondition.SelectedIndex = member.condition;
            this.textBoxValue.Text = member.value;
            this.buttonColor.BackColor = member.color;

            bInitial = false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayToDialog();
        }

        void EnableDisable()
        {
            bool flag_tag;
            bool flag;
            bool flag_value;

            if (!this.checkBoxActive.Checked)
            {
                flag_tag = false;
                flag = false;
                flag_value = false;
            }
            else
            {
                flag_tag = true;
                if (this.comboBoxCondition.SelectedIndex >= 0 && this.comboBoxCondition.SelectedIndex <= 4)
                {
                    flag_value = true;
                }
                else if (this.comboBoxCondition.SelectedIndex == 12)
                    flag_value = true;
                else
                    flag_value = false;

                flag = true;
            }

            this.textBoxTag.Enabled = flag_tag;
            this.buttonTag.Enabled = flag_tag;
            this.comboBoxCondition.Enabled = flag_tag;
            this.textBoxValue.Enabled = flag_value;

            this.buttonColor.Enabled = flag;
        }

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            DialogToArray();
            EnableDisable();
        }

        private void comboBoxCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            DialogToArray();
            EnableDisable();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            ExpandBasicColorMember member = new ExpandBasicColorMember();

            member.active = true;

            tempArray.Add(member);

            ListViewItem lvi = AddNewItem(member);

            lvi.Selected = true;
            lvi.EnsureVisible();

            //ArrayToDialog();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("삭제할 멤버를 선택하세요.", "삭제 오류");
                }
                else
                {
                    MessageBox.Show("Selete the member to delete.", "Delete error");
                }

                return;
            }

            ListViewItem lvi = this.listView1.SelectedItems[0];
            int index = lvi.Index;

            tempArray.RemoveAt(index);
            this.listView1.Items.RemoveAt(index);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0) return;

            ListViewItem lvi = this.listView1.SelectedItems[0];
            int index = lvi.Index;

            if (index <= 0) return;

            object temp = tempArray[index];
            tempArray[index] = tempArray[index - 1];
            tempArray[index - 1] = temp;

            ChangeItem(this.listView1.Items[index], (ExpandBasicColorMember)tempArray[index]);
            ChangeItem(this.listView1.Items[index - 1], (ExpandBasicColorMember)tempArray[index - 1]);

            this.listView1.Items[index - 1].Selected = true;
            this.listView1.Items[index - 1].EnsureVisible();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0) return;

            ListViewItem lvi = this.listView1.SelectedItems[0];
            int index = lvi.Index;

            if (index >= this.listView1.Items.Count - 1) return;

            object temp = tempArray[index];
            tempArray[index] = tempArray[index + 1];
            tempArray[index + 1] = temp;

            ChangeItem(this.listView1.Items[index], (ExpandBasicColorMember)tempArray[index]);
            ChangeItem(this.listView1.Items[index + 1], (ExpandBasicColorMember)tempArray[index + 1]);

            this.listView1.Items[index + 1].Selected = true;
            this.listView1.Items[index + 1].EnsureVisible();
        }

        private void textBoxTag_TextChanged(object sender, EventArgs e)
        {
            DialogToArray();
        }

        private void textBoxValue_TextChanged(object sender, EventArgs e)
        {
            DialogToArray();
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonColor.BackColor = dialog.GetSelectedColor();
                DialogToArray();
            }
        }

    }
}

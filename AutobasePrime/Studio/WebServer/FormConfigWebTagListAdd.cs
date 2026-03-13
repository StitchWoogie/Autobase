using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using DialogTag;
using NetTools;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AutoLibLocal
{
    public partial class FormConfigWebTagListAdd : Form
    {
        public FormConfigWebTagListAdd()
        {
            InitializeComponent();
        }

        public void Set(WebTagList list)
        {
            this.textBoxName.Text = list.name;
            this.textBoxDescription.Text = list.description;

            TagPublicClass tp;
            int[] tag_pos = new int[1];

            for (int i = 0; i < list.member.Count; i++)
            {
                tp = TagLib.GetStructPublic((string)list.member[i], ref tag_pos);

                ListViewItem lvi = new ListViewItem(tp.tag);
                lvi.SubItems.Add(tp.description);

                this.listViewMember.Items.Add(lvi);
            }
        }

        public WebTagList Get()
        {
            WebTagList list = new WebTagList();

            list.name = this.textBoxName.Text;
            list.description = this.textBoxDescription.Text;

            for (int i = 0; i < this.listViewMember.Items.Count; i++)
            {
                ListViewItem lvi = this.listViewMember.Items[i];

                list.member.Add(lvi.SubItems[0].Text);
            }

            return list;
        }

        private void FormConfigWebTagListAdd_Load(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.textBoxName.Text.Length == 0)
            {
                MessageBox.Show("Input the name.", "Input error");
                return;
            }

            for (int i = listViewMember.Items.Count - 1; i >= 0; i--) // 역순으로 순회 (삭제를 위해)
            {
                MILLI_DATA_TAG tag = new MILLI_DATA_TAG();
                tag.tag = listViewMember.Items[i].Text.Trim(); // i번째 항목 사용

                if (!TagLib.GetTagTypeAndPos(tag.tag, ref tag.tag_type, ref tag.tag_pos))
                {
                    DialogResult result;

                    if (Tools.IsLangKorean())
                    {
                        result = MessageBox.Show(
                            String.Format("{0}\n존재하지 않는 태그입니다.\n목록에서 삭제하시겠습니까?", tag.tag),
                            "태그 없음",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                    }
                    else
                    {
                        result = MessageBox.Show(
                             String.Format("{0}\nThis tag does not exist.\nDo you want to remove it from the list?", tag.tag),
                            "Tag Not Found",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                    }

                    if (result == DialogResult.Yes)
                    {
                        listViewMember.Items.RemoveAt(i);
                    }
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        //private void buttonAdd_Click(object sender, EventArgs e)
        //{
        //    string tag;
        //    string des;

        //    if (DialogTag.SelectTag.SelectAll(this, out tag, out des) == DialogResult.OK)
        //    {
        //        ListViewItem lvi = new ListViewItem(tag);
        //        lvi.SubItems.Add(des);
        //        listViewMember.Items.Add(lvi);

        //        lvi.Selected = true;
        //        lvi.EnsureVisible();
        //    }
        //}

        //20250903 PSU 다중태그 기능 추가, 중복태그 및 존재하지않는태그 체크
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagAI = true;
            dialog.bUseTagAO = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagDO = true;
            dialog.bUseTagST = true;
            dialog.bUseTagGDO= true;
            dialog.bUseMultiSelection = true;
            dialog.checkBoxMultiSelect.Checked = bTagMultiSelection;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (dialog.checkBoxMultiSelect.Checked == false || dialog.m_list.SelectedItems.Count <= 1)
                {
                    AddOneTag(dialog.sTag, dialog.sDes);
                }
                else
                {
                    ListViewItem lvi;
                    string tag;
                    int[] tag_pos = new int[1];
                    TagPublicClass tp;

                    for (int i = 0; i < dialog.m_list.SelectedItems.Count; i++)
                    {
                        lvi = dialog.m_list.SelectedItems[i];

                        tag = dialog.GetFullTagName(lvi.SubItems[0].Text);

                        tp = TagLib.GetStructPublic(tag, ref tag_pos);

                        AddOneTag(tag, tp.description);
                    }
                }

                bTagMultiSelection = dialog.checkBoxMultiSelect.Checked;
            }

        }

        static bool bTagMultiSelection = true;

        void AddOneTag(string tag, string des)
        {
            ListViewItem lvi;

            for (int i = 0; i < this.listViewMember.Items.Count; i++)
            {
                lvi = this.listViewMember.Items[i];
                if (lvi.SubItems[0].Text == tag)
                {
                    // 중복되는 경우 기존 항목을 선택하고 보이게 함
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                    return;
                }
            }

            lvi = new ListViewItem(tag);

            lvi.SubItems.Add(des);
            this.listViewMember.Items.Add(lvi);
            lvi.Selected = true;
            lvi.EnsureVisible();
        }


        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (listViewMember.SelectedItems.Count == 0)
                return;

            int selectedIndex = listViewMember.SelectedItems[0].Index;

            // 첫 번째 항목이면 이동할 수 없음
            if (selectedIndex <= 0)
                return;

            ListViewItem item = listViewMember.SelectedItems[0];

            // ListView에서 항목 이동
            listViewMember.Items.RemoveAt(selectedIndex);
            listViewMember.Items.Insert(selectedIndex - 1, item);

            // 선택 상태 유지
            listViewMember.Items[selectedIndex - 1].Selected = true;
            listViewMember.Items[selectedIndex - 1].EnsureVisible();
            listViewMember.Select();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (listViewMember.SelectedItems.Count == 0)
                return;

            int selectedIndex = listViewMember.SelectedItems[0].Index;

            // 마지막 항목이면 이동할 수 없음
            if (selectedIndex >= listViewMember.Items.Count - 1)
                return;

            ListViewItem item = listViewMember.SelectedItems[0];

            // ListView에서 항목 이동
            listViewMember.Items.RemoveAt(selectedIndex);
            listViewMember.Items.Insert(selectedIndex + 1, item);

            // 선택 상태 유지
            listViewMember.Items[selectedIndex + 1].Selected = true;
            listViewMember.Items[selectedIndex + 1].EnsureVisible();
            listViewMember.Select();
        }


        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listViewMember.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the item to delete.", "Delete error");
                return;
            }

            int index = listViewMember.SelectedItems[0].Index;
            listViewMember.Items.RemoveAt(index);
        }
    }
}
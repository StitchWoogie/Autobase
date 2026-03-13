using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using System.Collections;
using AutoLibLocal;

namespace Studio
{
    public partial class PropertyPageObjectTagAnimation : Form
    {
        MultiSelectCheckBox multiSelectOverlay = new MultiSelectCheckBox();

        public PropertyPageObjectTagAnimation()
        {
            InitializeComponent();

            TagConditionPlayFillToCombo(this.comboBoxCondition);
            
            multiSelectOverlay.Add(this.checkBoxOverlay);
        }

        private void PropertyPageObjectTagAnimation_Load(object sender, EventArgs e)
        {
            TotalConfig.AutoBaseListCtrlConfigLoad(this.listView1, "Studio", "TagAnimationMember");

            aniOn.TopLevel = false;
            aniOn.FormBorderStyle = FormBorderStyle.None;
            this.panelPreview.Controls.Add(aniOn);
            aniOn.Show();

            EnableDisable();
        }

        ArrayList tempArray;

        ListViewItem AddNewItem(TagAnimationMember member)
        {
            ListViewItem lvi = new ListViewItem("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");

            ChangeItem(lvi, member);

            this.listView1.Items.Add(lvi);

            return lvi;
        }

        bool bMultiSelect = false;

        public void SetObjectArgs(ObjectArgsTagAnimation args, bool multi_select)
        {
            //multiSelectOnFile.Set(args.sFileOn);
            //multiSelectOffFile.Set(args.sFileOff);
            multiSelectOverlay.Set(args.nOverlayMethod);

            //aniOn.SetFileName(args.sFileOn);
            //aniOff.SetFileName(args.sFileOff);
            tempArray = (ArrayList)Tools.CopyObject(args.member);

            this.listView1.Items.Clear();

            TagAnimationMember member;
            for (int i = 0; i < tempArray.Count; i++)
            {
                member = (TagAnimationMember)tempArray[i];

                AddNewItem(member);
            }

            if (tempArray.Count > 0)
            {
                this.listView1.Items[0].Selected = true;
            }

            // 다중선택일 때 모든 오브젝트가 마지막 오브젝트로 변경되어서 적용되지 않도록 수정했다. 2019-12-30
            bMultiSelect = multi_select;

            if (bMultiSelect)
            {
                this.listView1.Enabled = false;
                this.buttonAdd.Enabled = false;
                this.buttonDelete.Enabled = false;
                this.buttonDown.Enabled = false;
                this.buttonUp.Enabled = false;
                this.groupBox1.Enabled = false;
            }
        }

        public ObjectArgsTagAnimation GetObjectArgs(ObjectArgsTagAnimation org)
        {
            ObjectArgsTagAnimation args = (ObjectArgsTagAnimation)Tools.CopyObject(org);

            // 다중선택일 때 모든 오브젝트가 마지막 오브젝트로 변경되어서 적용되지 않도록 수정했다. 2019-12-30
            if(!bMultiSelect)
                args.member = tempArray;

            // 태그 애니메이션에서는 nOverlayMethod 하나만 다중선택해서 적용할 수 있다.
            multiSelectOverlay.Get(ref args.nOverlayMethod);

            // 원래 form_closed에서 해야하는데 closing closed 모두 발생안하다.
            TotalConfig.AutoBaseListCtrlConfigSave(this.listView1, "Studio", "TagAnimationMember");

            return args;
        }

        public static string[] sCommand = { "=", ">=", "<=", ">", "<" , "On", "Off", "HiHi", "High", "Low", "LoLo", "Alarm", "!=" };

        public static void TagConditionPlayFillToCombo(ComboBox combo)
        {
            for (int i = 0; i < sCommand.Length; i++)
            {
                combo.Items.Add(sCommand[i]);
            }
        }

        void ChangeItem(ListViewItem lvi, TagAnimationMember member)
        {
            if(Tools.IsLangKorean())
                lvi.SubItems[0].Text = member.active ? "사용" : "사용안함";
            else
                lvi.SubItems[0].Text = member.active ? "Use" : "Not Used";

            if (member.bDefault)
            {
                if(Tools.IsLangKorean()) 
                    lvi.SubItems[1].Text = "기본 그림";
                else
                    lvi.SubItems[1].Text = "default";
            }
            else
            {
                string tag = member.tag;
                if (member.tag == null || member.tag.Length == 0)
                {
                    tag = "???";
                }

                if (member.condition <= 4)
                {
                    lvi.SubItems[1].Text = tag + " " + sCommand[member.condition] + " " + member.value.ToString();
                }
                else if (member.condition == 5 || member.condition == 6)
                {
                    lvi.SubItems[1].Text = tag + " " + "=" + " " + sCommand[member.condition];
                }
                else if (member.condition == 7) // hihi
                {
                    lvi.SubItems[1].Text = tag + " " + ">=" + " " + sCommand[member.condition];
                }
                else if (member.condition == 8) // high
                {
                    lvi.SubItems[1].Text = tag + " " + ">=" + " " + sCommand[member.condition];
                }
                else if (member.condition == 9) // low
                {
                    lvi.SubItems[1].Text = tag + " " + "<=" + " " + sCommand[member.condition];
                }
                else if (member.condition == 10)    // lolo
                {
                    lvi.SubItems[1].Text = tag + " " + "<=" + " " + sCommand[member.condition];
                }
                else if (member.condition == 11)    // alarm
                {
                    lvi.SubItems[1].Text = tag + " " + "=" + " " + sCommand[member.condition];
                }
                else if (member.condition == 12)    // !=
                {
                    lvi.SubItems[1].Text = tag + " " + sCommand[member.condition] + " " + member.value.ToString();
                }
                else
                {
                    lvi.SubItems[1].Text = tag + " " + "?" + " " + member.value.ToString();
                }

            }

            lvi.SubItems[2].Text = member.filename;
        }

        bool bInitial = false;

        void DialogToArray()
        {
            if (bInitial) return;   // 초기화 중.

            if (this.listView1.SelectedItems.Count == 0) return;

            ListViewItem lvi = this.listView1.SelectedItems[0];
            int index = lvi.Index;

            TagAnimationMember member;
            member = (TagAnimationMember)tempArray[index];

            member.active = this.checkBoxActive.Checked;
            member.bDefault = this.checkBoxDefault.Checked;
            member.tag = this.textBoxTag.Text;
            member.condition = this.comboBoxCondition.SelectedIndex;
            member.value = this.textBoxValue.Text;
            member.filename = this.textBoxFilename.Text;

            ChangeItem(lvi, member);
        }

        void ArrayToDialog()
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                this.checkBoxActive.Checked = false;
                this.textBoxTag.Text = "";
                this.textBoxFilename.Text = "";
                this.textBoxValue.Text = "";
                this.comboBoxCondition.SelectedIndex = -1;
                return;
            }

            bInitial = true;

            int index = this.listView1.SelectedItems[0].Index;

            TagAnimationMember member;
            member = (TagAnimationMember)tempArray[index];

            this.checkBoxActive.Checked = member.active;
            this.checkBoxDefault.Checked = member.bDefault;
            this.textBoxTag.Text = member.tag;
            this.comboBoxCondition.SelectedIndex = member.condition;
            this.textBoxValue.Text = member.value;
            this.textBoxFilename.Text = member.filename;

            aniOn.SetFileName(member.filename);

            bInitial = false;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayToDialog();
        }

        void EnableDisable()
        {
            bool flag_tag;
            bool flag;
            bool flag_default = true;
            bool flag_value;

            if (!this.checkBoxActive.Checked)
            {
                flag_tag = false;
                flag = false;
                flag_default = false;
                flag_value = false;
            }
            else {
                if(this.checkBoxDefault.Checked) {
                    flag_tag = false;
                    flag_value = false;
                }
                else {
                    flag_tag = true;
                    if (this.comboBoxCondition.SelectedIndex >= 0 && this.comboBoxCondition.SelectedIndex <= 4)
                    {
                        flag_value = true;
                    }
                    else if (this.comboBoxCondition.SelectedIndex == 12)
                        flag_value = true;
                    else
                        flag_value = false;
                }
                flag = true;
            }

            this.checkBoxDefault.Enabled = flag_default;

            this.textBoxTag.Enabled = flag_tag;
            this.buttonTag.Enabled = flag_tag;
            this.comboBoxCondition.Enabled = flag_tag;
            this.textBoxValue.Enabled = flag_value;

            this.textBoxFilename.Enabled = flag;
        }

        private void checkBoxDefault_CheckedChanged(object sender, EventArgs e)
        {
            DialogToArray();
            EnableDisable();

            // default 그림으로 설정하면 다른 멤버는 default를 없앤다.
            if (!bInitial && this.checkBoxDefault.Checked)
            {   
                if (this.listView1.SelectedItems.Count == 0) return;

                ListViewItem lvi = this.listView1.SelectedItems[0];
                int index = lvi.Index;

                for (int i = 0; i < tempArray.Count; i++)
                {
                    if (i == index) continue;

                    TagAnimationMember member;
                    member = (TagAnimationMember)tempArray[i];

                    member.bDefault = false;

                    ChangeItem(this.listView1.Items[i], member);
                }
            }
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

        private void textBoxValue_TextChanged(object sender, EventArgs e)
        {
            DialogToArray();
        }

        private void buttonTag_Click(object sender, EventArgs e)
        {
            string tag, des;
            if (DialogTag.SelectTag.SelectAiDiSt(this, out tag, out des) != DialogResult.OK) return;

            this.textBoxTag.Text = tag;

            //DialogToArray();
        }

        AnimationPlay aniOn = new AnimationPlay();

        private void buttonFilename_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.InitialDirectory = TotalConfig.sDirWorkProject + "\\graphic";
            dialog.Filter = PropertyPageObjectBitmap.sFilterAnimation + "|" + PropertyPageObjectBitmap.sFilterBitmap;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string change;
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(ClassEditProperty.formEditor, dialog.FileName, out change);
                this.textBoxFilename.Text = change;
                aniOn.SetFileName(change);
            }

            //DialogToArray();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            TagAnimationMember member = new TagAnimationMember();

            member.active = true;

            tempArray.Add(member);

            ListViewItem lvi = AddNewItem(member);

            lvi.Selected = true;
            lvi.EnsureVisible();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0) {
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
            
            ChangeItem(this.listView1.Items[index], (TagAnimationMember)tempArray[index]);
            ChangeItem(this.listView1.Items[index - 1], (TagAnimationMember)tempArray[index - 1]);

            this.listView1.Items[index - 1].Selected = true;
            this.listView1.Items[index - 1].EnsureVisible();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0) return;

            ListViewItem lvi = this.listView1.SelectedItems[0];
            int index = lvi.Index;

            if (index >= this.listView1.Items.Count-1) return;

            object temp = tempArray[index];
            tempArray[index] = tempArray[index + 1];
            tempArray[index + 1] = temp;

            ChangeItem(this.listView1.Items[index], (TagAnimationMember)tempArray[index]);
            ChangeItem(this.listView1.Items[index + 1], (TagAnimationMember)tempArray[index + 1]);

            this.listView1.Items[index + 1].Selected = true;
            this.listView1.Items[index + 1].EnsureVisible();
        }

        private void PropertyPageObjectTagAnimation_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void PropertyPageObjectTagAnimation_FormClosing(object sender, FormClosingEventArgs e)
        {
 
        }

        private void textBoxTag_TextChanged(object sender, EventArgs e)
        {
            DialogToArray();
        }

        private void textBoxFilename_TextChanged(object sender, EventArgs e)
        {
            DialogToArray();
        }
    }
}
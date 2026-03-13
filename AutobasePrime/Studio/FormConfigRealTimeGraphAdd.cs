using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using AutoLibLocal;
using DialogTag;
using NetTools;
using System.Collections;

namespace Studio
{
    public partial class FormConfigRealTimeGraphAdd : Form
    {
        public FormConfigRealTimeGraphAdd()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.textBoxTitle.Text.Length == 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("제목을 입력해야 합니다.", "제목");
                }
                else if (Tools.IsLangChinese())
                {
                    MessageBox.Show("请输入标题。", "输入错误");
                }
                else
                {
                    MessageBox.Show("You must input Title.", "Title Error");
                }
                this.textBoxTitle.Select();
                return;
            }

            if (m_list.Items.Count == 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("저장할 태그를 하나 이상 추가하여야 합니다.", "태그 없음");
                }
                else if (Tools.IsLangChinese())
                {
                    MessageBox.Show("请添加一个以上的标记。", "没有标记");
                }
                else
                {
                    MessageBox.Show("Register tag to save.", "Tag not registered.");
                }
                return;
            }

            
            if (this.textBoxTagStart.Text.Length == 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("시작 DI 태그를 입력하여야 합니다.", "태그 추가");
                }
                else if (Tools.IsLangChinese())
                {
                    MessageBox.Show("请输入[Start DI标记]。", "输入错误");
                }
                else
                {
                    MessageBox.Show("Must Input Start DI Tag.", "DI Tag.");
                }
                this.textBoxTagStart.Select();
                return;
            }
            

            DialogResult = DialogResult.OK;
            Close();
        }

        public void GetStruct(ref RealTimeTestStruct item)
        {
            /*
            item.nCondition = GetRadioCondition();
            item.nCutMethod = GetRadioCutMethod();
            item.nSaveFileType = GetRadioSaveType();
            item.nGab = ConvertTool.ToInt32(this.numericUpDownDataUnit.Value);
            item.nSizeCut = ConvertTool.ToInt32(this.textBoxSizeCut.Text);
            item.tagCheckDI = this.textBoxCheckDI.Text;*/
            item.title = this.textBoxTitle.Text;
            item.nSize = ConvertTool.ToInt32(this.numericUpDownShowUnit.Value);
            item.nGab = ConvertTool.ToInt32(this.numericUpDownDataTime.Value);
            item.tagCheckDI = this.textBoxTagStart.Text;
            item.tagRunDI = this.textBoxTagRun.Text;
            /*
            item.bDateTimeMatch = this.checkBoxMatchToDateTime.Checked;

            item.bAutoDelete = this.checkBoxUseAutoDelete.Checked;
            item.nDaysOfAutoDelete = ConvertTool.ToInt32(this.numericUpDownDaysOfAutoDelete.Value);

            item.bUseTargetFolder = this.checkBoxSpecifyTargetFolder.Checked;
            item.sTargetFolder = this.textBoxCsvTargetFolder.Text;

            item.bUseCsvDateFolder = this.checkBoxUseCsvDateFolder.Checked;
            */
            RealTimeTestTag tag;
            int i;
            ListViewItem lvi;

            item.blockTag = new List<RealTimeTestTag>();

            for (i = 0; i < m_list.Items.Count; i++)
            {
                lvi = m_list.Items[i];
                tag = new RealTimeTestTag();
                tag.tag = lvi.SubItems[0].Text;
                item.blockTag.Add(tag);
            }
        }

        List<RealTimeTestTag> blockTag = new List<RealTimeTestTag>();

        public void SetStruct(RealTimeTestStruct item)
        {
            /*
            this.m_nCondition = item.nCondition;
            this.m_nCutMethod = item.nCutMethod;
            this.m_nSaveType = item.nSaveFileType;

            this.numericUpDownDataUnit.Value = item.nGab;
            this.textBoxSizeCut.Text = item.nSizeCut.ToString();
            this.textBoxCheckDI.Text = item.tagCheckDI;*/
            this.textBoxTitle.Text = item.title;
            Tools.SetNumericUpDownValue(this.numericUpDownShowUnit, item.nSize);
            Tools.SetNumericUpDownValue(this.numericUpDownDataTime, item.nGab);
            this.textBoxTagStart.Text = item.tagCheckDI;
            this.textBoxTagRun.Text = item.tagRunDI;
            /*
            this.checkBoxMatchToDateTime.Checked = item.bDateTimeMatch;

            this.checkBoxUseAutoDelete.Checked = item.bAutoDelete;
            Tools.SetNumericUpDownValue(this.numericUpDownDaysOfAutoDelete, item.nDaysOfAutoDelete);
            */
            this.blockTag = (List < RealTimeTestTag > )Tools.CopyObject(item.blockTag);
            /*
            this.checkBoxSpecifyTargetFolder.Checked = item.bUseTargetFolder;
            this.textBoxCsvTargetFolder.Text = item.sTargetFolder;

            this.checkBoxUseCsvDateFolder.Checked = item.bUseCsvDateFolder;
            */
            FillListBox();

            //EnableDisableTargetFolder();
        }

        void FillListBox()
        {
            m_list.Items.Clear();

            RealTimeTestTag tag;
            int l;
            ListViewItem lvi;

            for (l = 0; l < blockTag.Count; l++)
            {
                tag = blockTag[l];
                lvi = new ListViewItem(tag.tag);
                m_list.Items.Add(lvi);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string tag;
            string des;
            int i;

            if (SelectTag.SelectAiDi(this, out tag, out des) == DialogResult.OK)
            {
                ListViewItem lvi;
                for (i = 0; i < m_list.Items.Count; i++)
                {
                    lvi = m_list.Items[i];
                    if (lvi.SubItems[0].Text == tag)
                    {
                        lvi.Selected = true;
                        return;
                    }
                }

                lvi = new ListViewItem(tag);
                m_list.Items.Add(lvi);
                lvi.Selected = true;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("삭제하고 싶은 항목을 선택한 후 다시 하세요.", "삭제 오류");
                }
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请选择要删除的项。", "选择错误");
                else
                {
                    MessageBox.Show("Select tag to delete.", "Delete Error");
                }
                return;
            }

            int retn = m_list.SelectedItems[0].Index;

            m_list.Items.RemoveAt(retn);
        }

        private void buttonTagStart_Click(object sender, EventArgs e)
        {
            string tag;
            string des;

            if (SelectTag.SelectDi(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxTagStart.Text = tag;
            }		
        }

        private void buttonTagRun_Click(object sender, EventArgs e)
        {
            string tag;
            string des;

            if (SelectTag.SelectDi(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxTagRun.Text = tag;
            }
        }
    }
}

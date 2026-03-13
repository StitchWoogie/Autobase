using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;

namespace LocalMain.FontsAndColors
{
    public partial class UserControlFontsAndColors : UserControl
    {
        public UserControlFontsAndColors()
        {
            InitializeComponent();
        }

        ClassFontsAndColors tempFac;

        static string[] font_names = null;

        void FillList()
        {
            //if (fill_flag) return;

            for (int i = 0; i < font_names.Length; i++)
            {
                comboBoxFontsAndColorsFontName.Items.Add(font_names[i]);
            }

            for (int i = 2; i < 100; i++)
            {
                comboBoxFontsAndColorsFontSize.Items.Add(i.ToString());
            }

            //fill_flag = true;
        }

        public void Set(ClassFontsAndColors fac)
        {
            // 이부분이 속도가 좀 지연된다 그래서 static으로 잡았음 2007.10.30
            if (font_names == null)
            {
                font_names = new string[FontFamily.Families.Length];
                FontFamily family;
                for (int i = 0; i < FontFamily.Families.Length; i++)
                {
                    family = FontFamily.Families[i];
                    font_names[i] = family.Name;
                }
            }

            FillList();

            tempFac = (ClassFontsAndColors)Tools.CopyObject(fac);

            for (int i = 0; i < tempFac.arrayGroup.Count; i++)
            {
                this.comboBoxFontsAndColorsGroup.Items.Add(tempFac.arrayGroup[i].group_name);
            }

            this.comboBoxFontsAndColorsGroup.SelectedIndex = 0; // 맨처음의 항목을 선택해 준다.
        }

        public void Get(ClassFontsAndColors fac)
        {
            DialogToStruct();   // 마지막으로 설정한 것을 버퍼에 넣는다.

            //return tempFac;
            for (int i = 0; i < tempFac.arrayGroup.Count; i++)
            {
                //this.comboBoxFontsAndColorsGroup.Items.Add(tempFac.arrayGroup[i].group_name);
                for (int j = 0; j < tempFac.arrayGroup[i].arrayItem.Count; j++)
                {
                    fac.arrayGroup[i].arrayItem[j].Copy(tempFac.arrayGroup[i].arrayItem[j]);
                }
            }
        }

        private void comboBoxFontsAndColorsGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listBoxFontsAndColorsItem.Items.Clear();

            int index = this.comboBoxFontsAndColorsGroup.SelectedIndex;

            if (index == -1) return;

            FontsAndColorsGroup group = tempFac.GetGroup(this.comboBoxFontsAndColorsGroup.Text);

            if (group == null) return;

            for (int i = 0; i < group.arrayItem.Count; i++)
            {
                this.listBoxFontsAndColorsItem.Items.Add(group.arrayItem[i].item_name);
            }

            this.listBoxFontsAndColorsItem.SelectedIndex = 0;   // 맨처음의 항목을 자동으로 선택해 준다.
        }

        int nOldIndex = -1;

        private void listBoxFontsAndColorsItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            DialogToStruct();

            StructToDialog();
        }

        void DialogToStruct()
        {
            if (nOldIndex == -1) return;

            int index = nOldIndex;

            if (index == -1) return;

            FontsAndColorsGroup group = tempFac.GetGroup(this.comboBoxFontsAndColorsGroup.Text);

            if (group == null) return;

            FontsAndColorsItemPublic item = group.GetItem(this.listBoxFontsAndColorsItem.Text);

            if (item == null) return;

            FontsAndColorsItemFont facif = (FontsAndColorsItemFont)item;

            facif.FontName = this.comboBoxFontsAndColorsFontName.Text;
            facif.FontSize = ConvertTool.ToSingle(this.comboBoxFontsAndColorsFontSize.Text);
            facif.FontBold = this.checkBoxFontsAndColorsFontBold.Checked;
        }

        void StructToDialog()
        {
            int index = this.listBoxFontsAndColorsItem.SelectedIndex;

            if (index == -1) return;

            FontsAndColorsGroup group = tempFac.GetGroup(this.comboBoxFontsAndColorsGroup.Text);

            if (group == null) return;

            FontsAndColorsItemPublic item = group.GetItem(this.listBoxFontsAndColorsItem.Text);

            if (item == null) return;

            FontsAndColorsItemFont facif = (FontsAndColorsItemFont)item;

            this.comboBoxFontsAndColorsFontName.Text = facif.FontName;
            this.comboBoxFontsAndColorsFontSize.Text = facif.FontSize.ToString();
            this.checkBoxFontsAndColorsFontBold.Checked = facif.FontBold;

            nOldIndex = index;
        }

        bool IsFontExist(string name)
        {
            if (font_names == null) return false;
            for (int i = 0; i < font_names.Length; i++)
            {
                if (String.Compare(font_names[i], name, true) == 0) return true;
            }

            return false;
        }

        void PreviewFontChange()
        {
            int size;

            size = ConvertTool.ToInt32(comboBoxFontsAndColorsFontSize.Text);
            if (size == 0) return;

            FontStyle style = 0;
            //if (checkBoxItalic.Checked) style |= FontStyle.Italic;
            if (checkBoxFontsAndColorsFontBold.Checked) style |= FontStyle.Bold;
            //if (checkBoxUnderline.Checked) style |= FontStyle.Underline;
            //if (checkBoxStrikeout.Checked) style |= FontStyle.Strikeout;

            Font font = null;
            string font_name = this.comboBoxFontsAndColorsFontName.Text;

            if (!this.IsFontExist(font_name))
            {
                font_name = "Arial";
            }

            try
            {
                font = new Font(font_name, size, style);
                labelPreview.BackColor = Color.White;
            }
            catch	// 폰트가 해당 스타일을 지원하지 않을 때 발생한다. 
            {
                font = new Font("Arial", size, style);
                labelPreview.BackColor = Color.Red;
            }

            labelPreview.Font = font;
        }

        private void checkBoxFontsAndColorsFontBold_CheckedChanged(object sender, EventArgs e)
        {
            PreviewFontChange();
        }

        void ActiveStyle()
        {
            string font_name = this.comboBoxFontsAndColorsFontName.Text;
            FontFamily family;

            if (!this.IsFontExist(font_name))
            {
                font_name = "Arial";
            }

            family = new FontFamily(font_name);

            //bool flag = family.IsStyleAvailable(FontStyle.Italic);
            //this.checkBoxItalic.Enabled = flag;
            //if (!flag) this.checkBoxItalic.Checked = false;

            bool flag = family.IsStyleAvailable(FontStyle.Bold);
            this.checkBoxFontsAndColorsFontBold.Enabled = flag;
            if (!flag) this.checkBoxFontsAndColorsFontBold.Checked = false;
        }

        private void comboBoxFontsAndColorsFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveStyle();
            PreviewFontChange();
        }

        private void comboBoxFontsAndColorsFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            PreviewFontChange();
        }

        private void buttonFontsAndColorsForegroundColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = buttonFontsAndColorsForegroundColor.BackColor;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                buttonFontsAndColorsForegroundColor.BackColor = dialog.Color;
                PreviewFontChange();
            }
        }

        private void buttonFontsAndColorsBackgroundColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = buttonFontsAndColorsBackgroundColor.BackColor;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                buttonFontsAndColorsBackgroundColor.BackColor = dialog.Color;
                PreviewFontChange();
            }
        }
    }
}

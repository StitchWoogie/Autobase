using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using DatabaseConnection;
using AutoLibLocal;
using Studio.Script;

namespace Studio.Property
{
    public partial class PropertyPageObjectDataGridView : Form
    {
        ScriptClass scriptEventCellClick;
        ScriptClass scriptEventCellPainting;
        ScriptClass scriptEventCellValueChanged;

        ConnectionStringList listDsn = new ConnectionStringList();

        public PropertyPageObjectDataGridView()
        {
            InitializeComponent();
        }

        public void SetObjectArgs(ObjectArgsDataGridView args)
        {
            this.comboBoxDsn.Text = args.dsn;

            scriptEventCellClick = args.scriptEventCellClick;
            scriptEventCellPainting = args.scriptEventCellPainting;
            scriptEventCellValueChanged = args.scriptEventCellValueChanged;

            this.checkBoxAllowUserToAddRows.Checked = args.bAllowUserToAddRows;
            this.checkBoxAllowUserToDeleteRows.Checked = args.bAllowUserToDeleteRows;


            //20250304 PSU 추가
            this.checkBoxHideBorder.Checked = args.bHideBorder;
            this.checkBoxAlternatingRowColor.Checked = args.bUseAlternatingRowColors;
            this.checkBoxHideGridLines.Checked = args.bHideGridLines;
            this.checkBoxHideRowHeader.Checked = args.bHideRowHeaders;
            this.checkBoxColumnsAutoSize.Checked = (args.nAutoSizeColumnsMode == (int)(DataGridViewAutoSizeColumnsMode.AllCells));
            this.checkBoxRowsAutoSize.Checked = (args.nAutoSizeRowsMode == (int)(DataGridViewAutoSizeRowsMode.AllCells));
            this.buttonAlternatingRowTextColor.BackColor = args.lAlternatingRowForeColor;
            this.buttonAlternatingRowBackColor.BackColor = args.lAlternatingRowBackColor;
            this.buttonSelectionForeColor.BackColor = args.lSelectionForeColor;
            this.buttonSelectionBackColor.BackColor = args.lSelectionBackColor;
            this.buttonColHeadersForeColor.BackColor = args.lColumnHeadersForeColor;
            this.buttonColHeadersBackColor.BackColor = args.lColumnHeadersDefaultBackColor;
            this.buttonRowHeadersForeColor.BackColor = args.lRowHeadersDefaultForeColor;
            this.buttonRowHeadersBackColor.BackColor = args.lRowHeadersDefaultBackColor;
            //this.buttonCellForeColor.BackColor = args.lCellForeColor; 
            this.buttonCellBackColor.BackColor = args.lCellBackColor;
            this.numericUpDownColumnHeaderFontSize.Value = args.nColumnHeaderFontSize; 
        }

        public ObjectArgsDataGridView GetObjectArgs(ObjectArgsDataGridView org)
        {
            ObjectArgsDataGridView args = (ObjectArgsDataGridView)Tools.CopyObject(org);

            args.dsn = this.comboBoxDsn.Text;

            args.bAllowUserToAddRows = this.checkBoxAllowUserToAddRows.Checked;
            args.bAllowUserToDeleteRows = this.checkBoxAllowUserToDeleteRows.Checked;

            args.scriptEventCellClick = scriptEventCellClick;
            args.scriptEventCellPainting = scriptEventCellPainting;
            args.scriptEventCellValueChanged = scriptEventCellValueChanged;

            // 20250304 PSU 추가
            args.bHideBorder = this.checkBoxHideBorder.Checked;
            args.bUseAlternatingRowColors = this.checkBoxAlternatingRowColor.Checked;
            args.bHideRowHeaders = this.checkBoxHideRowHeader.Checked;
            args.bHideGridLines = this.checkBoxHideGridLines.Checked;

            if (this.checkBoxColumnsAutoSize.Checked)
                args.nAutoSizeColumnsMode = (int)DataGridViewAutoSizeColumnsMode.AllCells;
            else
                args.nAutoSizeColumnsMode = (int)DataGridViewAutoSizeColumnsMode.None;

            if (this.checkBoxRowsAutoSize.Checked)
                args.nAutoSizeRowsMode = (int)DataGridViewAutoSizeRowsMode.AllCells;
            else
                args.nAutoSizeRowsMode = (int)DataGridViewAutoSizeRowsMode.None;

            args.lSelectionForeColor = this.buttonSelectionForeColor.BackColor;
            args.lSelectionBackColor = this.buttonSelectionBackColor.BackColor;
            args.lColumnHeadersForeColor = this.buttonColHeadersForeColor.BackColor;
            args.lColumnHeadersDefaultBackColor = this.buttonColHeadersBackColor.BackColor;
            args.lRowHeadersDefaultForeColor = this.buttonRowHeadersForeColor.BackColor;
            args.lRowHeadersDefaultBackColor = this.buttonRowHeadersBackColor.BackColor;
            //args.lCellForeColor = this.buttonCellForeColor.BackColor;
            args.lCellBackColor = this.buttonCellBackColor.BackColor;
            args.lAlternatingRowBackColor = this.buttonAlternatingRowBackColor.BackColor;
            args.lAlternatingRowForeColor = this.buttonAlternatingRowTextColor.BackColor;
            args.nColumnHeaderFontSize = ConvertTool.ToInt32(this.numericUpDownColumnHeaderFontSize.Value); 

            return args;
        }

        private void buttonEventCellClick_Click(object sender, EventArgs e)
        {
            FormScriptEditor dialog = new FormScriptEditor();

            if (NetTools.Tools.IsLangKorean())
                dialog.SetScript("CellClick 스크립트 편집", scriptEventCellClick);
            else
                dialog.SetScript("CellClick Script", scriptEventCellClick);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                scriptEventCellClick = dialog.GetScript();
            }
        }

        private void buttonEventCellPainting_Click(object sender, EventArgs e)
        {
            FormScriptEditor dialog = new FormScriptEditor();

            if (NetTools.Tools.IsLangKorean())
                dialog.SetScript("CellPainting 스크립트 편집", scriptEventCellPainting);
            else
                dialog.SetScript("CellPainting Script", scriptEventCellPainting);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                scriptEventCellPainting = dialog.GetScript();
            }
        }

        private void buttonEventCellValueChanged_Click(object sender, EventArgs e)
        {
            FormScriptEditor dialog = new FormScriptEditor();

            if (NetTools.Tools.IsLangKorean())
                dialog.SetScript("CellValueChanged 스크립트 편집", scriptEventCellValueChanged);
            else
                dialog.SetScript("CellValueChanged Script", scriptEventCellValueChanged);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                scriptEventCellValueChanged = dialog.GetScript();
            }
        }

        private void buttonDsn_Click(object sender, EventArgs e)
        {
            FormDatabaseConnection dialog = new FormDatabaseConnection(listDsn);
            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                DbTool.FillComboBox(this.comboBoxDsn, listDsn);
            }
        }

        private void PropertyPageObjectDataGridView_Load(object sender, EventArgs e)
        {
            listDsn.ConnectionStringLoad();
            DbTool.FillComboBox(this.comboBoxDsn, listDsn); 
        }

        private void buttonSelectionForeColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonSelectionForeColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonSelectionForeColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonSelectionBackColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonSelectionBackColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonSelectionBackColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonColHeadersForeColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonColHeadersForeColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonColHeadersForeColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonColHeadersBackColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonColHeadersBackColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonColHeadersBackColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonRowHeadersForeColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonRowHeadersForeColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonRowHeadersForeColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonRowHeadersBackColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonRowHeadersBackColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonRowHeadersBackColor.BackColor = dialog.GetSelectedColor();
            }

        }


        private void buttonCellBackColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonCellBackColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonCellBackColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonAlternatingRowBackColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonAlternatingRowBackColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonAlternatingRowBackColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonAlternatingRowTextColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonAlternatingRowTextColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonAlternatingRowTextColor.BackColor = dialog.GetSelectedColor();
            }
        }

        
    }
}

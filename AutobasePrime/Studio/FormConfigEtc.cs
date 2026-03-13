using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;

namespace Studio
{
    public partial class FormConfigEtc : Form
    {
        //20241010 PSU
        public class MonitorItem
        {
            public int Index { get; private set; }
            public string Text { get; private set; }

            public MonitorItem(int index, string text)
            {
                this.Index = index;
                this.Text = text;
            }

            public override string ToString()
            {
                return this.Text;
            }
        }

        public FormConfigEtc()
        {
            InitializeComponent();
        }

        private void FormConfigEtc_Load(object sender, EventArgs e)
        {
            this.numericUpDownUndoCount.Value = AutoLib.ConfigStudio.nUndoCount;
            this.checkBoxSaveLockStatus.Checked = AutoLib.ConfigStudio.bSaveLayerLockStatus;
            this.checkBoxSaveShowStatus.Checked = AutoLib.ConfigStudio.bSaveLayerShowStatus;

            Tools.SetNumericUpDownValue(this.numericUpDownPasteX, AutoLib.ConfigStudio.nPasteX);
            Tools.SetNumericUpDownValue(this.numericUpDownPasteY, AutoLib.ConfigStudio.nPasteY);

            this.checkBoxUserNewScriptEditor.Checked = AutoLib.ConfigStudio.bUseNewScriptEditor;

            this.chkUseSmartScriptEditor.Checked = AutoLib.ConfigStudio.bUseMonacoEditor;

            checkBoxUserNewScriptEditor.Enabled = !chkUseSmartScriptEditor.Checked;

            this.textBoxAutoBackupFolder.Text = BackUp.GetAutoBackupFolder();

            //20241010 PSU
            PopulateMonitorComboBox(); 
            LoadSavedMonitorIndex(); 
        }

        private void PopulateMonitorComboBox()
        {
            comboBoxStartMonitorIndex.Items.Clear();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Screen screen = Screen.AllScreens[i];
                string itemText = string.Format(" {0} ({1}x{2})", i, screen.Bounds.Width, screen.Bounds.Height);
                if (screen.Primary)
                {
                    itemText += " - Main";
                }
                comboBoxStartMonitorIndex.Items.Add(new MonitorItem(i, itemText));
            }

            comboBoxStartMonitorIndex.DisplayMember = "Text";
            comboBoxStartMonitorIndex.ValueMember = "Index";

            if (comboBoxStartMonitorIndex.Items.Count > 0)
            {
                comboBoxStartMonitorIndex.SelectedIndex = 0;
            }

            comboBoxStudioMonitorIndex.Items.Clear();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Screen screen = Screen.AllScreens[i];
                string itemText = string.Format(" {0} ({1}x{2})", i, screen.Bounds.Width, screen.Bounds.Height);
                if (screen.Primary)
                {
                    itemText += " - Main";
                }
                comboBoxStudioMonitorIndex.Items.Add(new MonitorItem(i, itemText));
            }

            comboBoxStudioMonitorIndex.DisplayMember = "Text";
            comboBoxStudioMonitorIndex.ValueMember = "Index";

            if (comboBoxStudioMonitorIndex.Items.Count > 0)
            {
                comboBoxStudioMonitorIndex.SelectedIndex = 0;
            }
        }

        private void LoadSavedMonitorIndex()
        {
            int savedIndex = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "StartMonitorIndex", 0);
            for (int i = 0; i < comboBoxStartMonitorIndex.Items.Count; i++)
            {
                if (((MonitorItem)comboBoxStartMonitorIndex.Items[i]).Index == savedIndex)
                {
                    comboBoxStartMonitorIndex.SelectedIndex = i;
                    break;
                }
            }

            savedIndex = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "StudioMonitorIndex", 0);
            for (int i = 0; i < comboBoxStudioMonitorIndex.Items.Count; i++)
            {
                if (((MonitorItem)comboBoxStudioMonitorIndex.Items[i]).Index == savedIndex)
                {
                    comboBoxStudioMonitorIndex.SelectedIndex = i;
                    break;
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            AutoLib.ConfigStudio.nUndoCount = Convert.ToInt32(this.numericUpDownUndoCount.Value);
            AutoLib.ConfigStudio.bSaveLayerLockStatus = this.checkBoxSaveLockStatus.Checked;
            AutoLib.ConfigStudio.bSaveLayerShowStatus = this.checkBoxSaveShowStatus.Checked;

            AutoLib.ConfigStudio.nPasteX = Convert.ToInt32(this.numericUpDownPasteX.Value);
            AutoLib.ConfigStudio.nPasteY = Convert.ToInt32(this.numericUpDownPasteY.Value);

            AutoLib.ConfigStudio.bUseNewScriptEditor = this.checkBoxUserNewScriptEditor.Checked;

            AutoLib.ConfigStudio.bUseMonacoEditor = this.chkUseSmartScriptEditor.Checked;

            BackUp.SetAutoBackupFolder(this.textBoxAutoBackupFolder.Text);

            AutoLib.ConfigStudio.Save();

            //20241010 PSU
            if (comboBoxStartMonitorIndex.SelectedItem != null)
            {
                int selectedMonitorIndex = ((MonitorItem)comboBoxStartMonitorIndex.SelectedItem).Index;
                TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "StartMonitorIndex", selectedMonitorIndex);
            }

            if (comboBoxStudioMonitorIndex.SelectedItem != null)
            {
                int selectedMonitorIndex = ((MonitorItem)comboBoxStudioMonitorIndex.SelectedItem).Index;
                TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "StudioMonitorIndex", selectedMonitorIndex);
            }


            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonAutoBackupFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.SelectedPath = this.textBoxAutoBackupFolder.Text;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxAutoBackupFolder.Text = dialog.SelectedPath;
            }
        }

        private void buttonCheckMonitor_Click(object sender, EventArgs e)
        {
            MonitorIndexDisplay.ShowMonitorIndexes();
        }

        private void chkUseSmartScriptEditor_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxUserNewScriptEditor.Checked) checkBoxUserNewScriptEditor.Checked = chkUseSmartScriptEditor.Checked;
            checkBoxUserNewScriptEditor.Enabled = !chkUseSmartScriptEditor.Checked;
        }

    }
}
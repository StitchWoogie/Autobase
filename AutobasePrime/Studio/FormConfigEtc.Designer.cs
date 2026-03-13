namespace Studio
{
    partial class FormConfigEtc
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigEtc));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownUndoCount = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxSaveShowStatus = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveLockStatus = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkUseSmartScriptEditor = new System.Windows.Forms.CheckBox();
            this.buttonCheckMonitor = new System.Windows.Forms.Button();
            this.comboBoxStartMonitorIndex = new System.Windows.Forms.ComboBox();
            this.comboBoxStudioMonitorIndex = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxUserNewScriptEditor = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownPasteY = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownPasteX = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonAutoBackupFolder = new System.Windows.Forms.Button();
            this.textBoxAutoBackupFolder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUndoCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPasteY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPasteX)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownUndoCount);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // numericUpDownUndoCount
            // 
            resources.ApplyResources(this.numericUpDownUndoCount, "numericUpDownUndoCount");
            this.numericUpDownUndoCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownUndoCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownUndoCount.Name = "numericUpDownUndoCount";
            this.numericUpDownUndoCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.checkBoxSaveShowStatus);
            this.groupBox2.Controls.Add(this.checkBoxSaveLockStatus);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxSaveShowStatus
            // 
            resources.ApplyResources(this.checkBoxSaveShowStatus, "checkBoxSaveShowStatus");
            this.checkBoxSaveShowStatus.Name = "checkBoxSaveShowStatus";
            this.checkBoxSaveShowStatus.UseVisualStyleBackColor = true;
            // 
            // checkBoxSaveLockStatus
            // 
            resources.ApplyResources(this.checkBoxSaveLockStatus, "checkBoxSaveLockStatus");
            this.checkBoxSaveLockStatus.Name = "checkBoxSaveLockStatus";
            this.checkBoxSaveLockStatus.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.chkUseSmartScriptEditor);
            this.tabPage1.Controls.Add(this.buttonCheckMonitor);
            this.tabPage1.Controls.Add(this.comboBoxStartMonitorIndex);
            this.tabPage1.Controls.Add(this.comboBoxStudioMonitorIndex);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.checkBoxUserNewScriptEditor);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkUseSmartScriptEditor
            // 
            resources.ApplyResources(this.chkUseSmartScriptEditor, "chkUseSmartScriptEditor");
            this.chkUseSmartScriptEditor.Name = "chkUseSmartScriptEditor";
            this.chkUseSmartScriptEditor.UseVisualStyleBackColor = true;
            this.chkUseSmartScriptEditor.CheckedChanged += new System.EventHandler(this.chkUseSmartScriptEditor_CheckedChanged);
            // 
            // buttonCheckMonitor
            // 
            resources.ApplyResources(this.buttonCheckMonitor, "buttonCheckMonitor");
            this.buttonCheckMonitor.Name = "buttonCheckMonitor";
            this.buttonCheckMonitor.UseVisualStyleBackColor = true;
            this.buttonCheckMonitor.Click += new System.EventHandler(this.buttonCheckMonitor_Click);
            // 
            // comboBoxStartMonitorIndex
            // 
            resources.ApplyResources(this.comboBoxStartMonitorIndex, "comboBoxStartMonitorIndex");
            this.comboBoxStartMonitorIndex.FormattingEnabled = true;
            this.comboBoxStartMonitorIndex.Name = "comboBoxStartMonitorIndex";
            // 
            // comboBoxStudioMonitorIndex
            // 
            resources.ApplyResources(this.comboBoxStudioMonitorIndex, "comboBoxStudioMonitorIndex");
            this.comboBoxStudioMonitorIndex.FormattingEnabled = true;
            this.comboBoxStudioMonitorIndex.Name = "comboBoxStudioMonitorIndex";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // checkBoxUserNewScriptEditor
            // 
            resources.ApplyResources(this.checkBoxUserNewScriptEditor, "checkBoxUserNewScriptEditor");
            this.checkBoxUserNewScriptEditor.Name = "checkBoxUserNewScriptEditor";
            this.checkBoxUserNewScriptEditor.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.numericUpDownPasteY);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownPasteX);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // numericUpDownPasteY
            // 
            resources.ApplyResources(this.numericUpDownPasteY, "numericUpDownPasteY");
            this.numericUpDownPasteY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDownPasteY.Name = "numericUpDownPasteY";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownPasteX
            // 
            resources.ApplyResources(this.numericUpDownPasteX, "numericUpDownPasteX");
            this.numericUpDownPasteX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDownPasteX.Name = "numericUpDownPasteX";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Controls.Add(this.buttonAutoBackupFolder);
            this.tabPage3.Controls.Add(this.textBoxAutoBackupFolder);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // buttonAutoBackupFolder
            // 
            resources.ApplyResources(this.buttonAutoBackupFolder, "buttonAutoBackupFolder");
            this.buttonAutoBackupFolder.Name = "buttonAutoBackupFolder";
            this.buttonAutoBackupFolder.UseVisualStyleBackColor = true;
            this.buttonAutoBackupFolder.Click += new System.EventHandler(this.buttonAutoBackupFolder_Click);
            // 
            // textBoxAutoBackupFolder
            // 
            resources.ApplyResources(this.textBoxAutoBackupFolder, "textBoxAutoBackupFolder");
            this.textBoxAutoBackupFolder.Name = "textBoxAutoBackupFolder";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // FormConfigEtc
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigEtc";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigEtc_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUndoCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPasteY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPasteX)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownUndoCount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxSaveShowStatus;
        private System.Windows.Forms.CheckBox checkBoxSaveLockStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numericUpDownPasteY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownPasteX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxUserNewScriptEditor;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button buttonAutoBackupFolder;
        private System.Windows.Forms.TextBox textBoxAutoBackupFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonCheckMonitor;
        private System.Windows.Forms.ComboBox comboBoxStartMonitorIndex;
        private System.Windows.Forms.ComboBox comboBoxStudioMonitorIndex;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkUseSmartScriptEditor;
    }
}
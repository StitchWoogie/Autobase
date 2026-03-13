namespace Reporting.Designer
{
    partial class FormReportDesigner
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this._menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this._menuFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this._menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this._menuEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this._menuEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFormat = new System.Windows.Forms.ToolStripMenuItem();
            this._menuFormatCells = new System.Windows.Forms.ToolStripMenuItem();
            this._toolBar = new System.Windows.Forms.ToolStrip();
            this._btnBold = new System.Windows.Forms.ToolStripButton();
            this._btnItalic = new System.Windows.Forms.ToolStripButton();
            this._btnUnderline = new System.Windows.Forms.ToolStripButton();
            this._toolBarSep1 = new System.Windows.Forms.ToolStripSeparator();
            this._btnAlignLeft = new System.Windows.Forms.ToolStripButton();
            this._btnAlignCenter = new System.Windows.Forms.ToolStripButton();
            this._btnAlignRight = new System.Windows.Forms.ToolStripButton();
            this._toolBarSep2 = new System.Windows.Forms.ToolStripSeparator();
            this._btnBorderAll = new System.Windows.Forms.ToolStripButton();
            this._cmbFontName = new System.Windows.Forms.ToolStripComboBox();
            this._cmbFontSize = new System.Windows.Forms.ToolStripComboBox();
            this._formulaBar = new Reporting.Designer.Controls.FormulaBar();
            this._spreadsheet = new Reporting.Designer.Controls.SpreadsheetControl();
            this._sheetTab = new Reporting.Designer.Controls.SheetTabControl();
            this._statusBar = new System.Windows.Forms.StatusStrip();
            this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._menuStrip.SuspendLayout();
            this._toolBar.SuspendLayout();
            this._statusBar.SuspendLayout();
            this.SuspendLayout();
            //
            // _menuStrip
            //
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._menuFile,
                this._menuEdit,
                this._menuFormat
            });
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Size = new System.Drawing.Size(900, 24);
            this._menuStrip.TabIndex = 0;
            //
            // _menuFile
            //
            this._menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._menuFileNew,
                this._menuFileOpen,
                this._menuFileSave,
                this._menuFileSaveAs,
                this._menuFileSep1,
                this._menuFileClose
            });
            this._menuFile.Name = "_menuFile";
            this._menuFile.Text = "파일(&F)";
            //
            // _menuFileNew
            //
            this._menuFileNew.Name = "_menuFileNew";
            this._menuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this._menuFileNew.Text = "새로 만들기(&N)";
            this._menuFileNew.Click += new System.EventHandler(this.MenuFileNew_Click);
            //
            // _menuFileOpen
            //
            this._menuFileOpen.Name = "_menuFileOpen";
            this._menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this._menuFileOpen.Text = "열기(&O)...";
            this._menuFileOpen.Click += new System.EventHandler(this.MenuFileOpen_Click);
            //
            // _menuFileSave
            //
            this._menuFileSave.Name = "_menuFileSave";
            this._menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._menuFileSave.Text = "저장(&S)";
            this._menuFileSave.Click += new System.EventHandler(this.MenuFileSave_Click);
            //
            // _menuFileSaveAs
            //
            this._menuFileSaveAs.Name = "_menuFileSaveAs";
            this._menuFileSaveAs.Text = "다른 이름으로 저장(&A)...";
            this._menuFileSaveAs.Click += new System.EventHandler(this.MenuFileSaveAs_Click);
            //
            // _menuFileSep1
            //
            this._menuFileSep1.Name = "_menuFileSep1";
            //
            // _menuFileClose
            //
            this._menuFileClose.Name = "_menuFileClose";
            this._menuFileClose.Text = "닫기(&C)";
            this._menuFileClose.Click += new System.EventHandler(delegate { this.Close(); });
            //
            // _menuEdit
            //
            this._menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._menuEditUndo,
                this._menuEditRedo
            });
            this._menuEdit.Name = "_menuEdit";
            this._menuEdit.Text = "편집(&E)";
            //
            // _menuEditUndo
            //
            this._menuEditUndo.Name = "_menuEditUndo";
            this._menuEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this._menuEditUndo.Text = "실행 취소(&U)";
            this._menuEditUndo.Enabled = false;
            //
            // _menuEditRedo
            //
            this._menuEditRedo.Name = "_menuEditRedo";
            this._menuEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this._menuEditRedo.Text = "다시 실행(&R)";
            this._menuEditRedo.Enabled = false;
            //
            // _menuFormat
            //
            this._menuFormat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._menuFormatCells
            });
            this._menuFormat.Name = "_menuFormat";
            this._menuFormat.Text = "서식(&O)";
            //
            // _menuFormatCells
            //
            this._menuFormatCells.Name = "_menuFormatCells";
            this._menuFormatCells.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this._menuFormatCells.Text = "셀 서식(&E)...";
            //
            // _toolBar
            //
            this._toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._cmbFontName,
                this._cmbFontSize,
                this._btnBold,
                this._btnItalic,
                this._btnUnderline,
                this._toolBarSep1,
                this._btnAlignLeft,
                this._btnAlignCenter,
                this._btnAlignRight,
                this._toolBarSep2,
                this._btnBorderAll
            });
            this._toolBar.Location = new System.Drawing.Point(0, 24);
            this._toolBar.Name = "_toolBar";
            this._toolBar.Size = new System.Drawing.Size(900, 25);
            this._toolBar.TabIndex = 1;
            //
            // _cmbFontName
            //
            this._cmbFontName.Name = "_cmbFontName";
            this._cmbFontName.Size = new System.Drawing.Size(120, 25);
            this._cmbFontName.Text = "맑은 고딕";
            this._cmbFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            //
            // _cmbFontSize
            //
            this._cmbFontSize.Name = "_cmbFontSize";
            this._cmbFontSize.Size = new System.Drawing.Size(45, 25);
            this._cmbFontSize.Text = "11";
            this._cmbFontSize.Items.AddRange(new object[] { "8", "9", "10", "11", "12", "14", "16", "18", "20", "24", "28", "36" });
            //
            // _btnBold
            //
            this._btnBold.Name = "_btnBold";
            this._btnBold.Text = "B";
            this._btnBold.Font = new System.Drawing.Font("맑은 고딕", 9f, System.Drawing.FontStyle.Bold);
            this._btnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnBold.ToolTipText = "굵게";
            //
            // _btnItalic
            //
            this._btnItalic.Name = "_btnItalic";
            this._btnItalic.Text = "I";
            this._btnItalic.Font = new System.Drawing.Font("맑은 고딕", 9f, System.Drawing.FontStyle.Italic);
            this._btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnItalic.ToolTipText = "기울임꼴";
            //
            // _btnUnderline
            //
            this._btnUnderline.Name = "_btnUnderline";
            this._btnUnderline.Text = "U";
            this._btnUnderline.Font = new System.Drawing.Font("맑은 고딕", 9f, System.Drawing.FontStyle.Underline);
            this._btnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnUnderline.ToolTipText = "밑줄";
            //
            // _toolBarSep1
            //
            this._toolBarSep1.Name = "_toolBarSep1";
            //
            // _btnAlignLeft
            //
            this._btnAlignLeft.Name = "_btnAlignLeft";
            this._btnAlignLeft.Text = "좌";
            this._btnAlignLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnAlignLeft.ToolTipText = "왼쪽 맞춤";
            //
            // _btnAlignCenter
            //
            this._btnAlignCenter.Name = "_btnAlignCenter";
            this._btnAlignCenter.Text = "중";
            this._btnAlignCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnAlignCenter.ToolTipText = "가운데 맞춤";
            //
            // _btnAlignRight
            //
            this._btnAlignRight.Name = "_btnAlignRight";
            this._btnAlignRight.Text = "우";
            this._btnAlignRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnAlignRight.ToolTipText = "오른쪽 맞춤";
            //
            // _toolBarSep2
            //
            this._toolBarSep2.Name = "_toolBarSep2";
            //
            // _btnBorderAll
            //
            this._btnBorderAll.Name = "_btnBorderAll";
            this._btnBorderAll.Text = "田";
            this._btnBorderAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnBorderAll.ToolTipText = "모든 테두리";
            //
            // _formulaBar
            //
            this._formulaBar.Dock = System.Windows.Forms.DockStyle.Top;
            this._formulaBar.Location = new System.Drawing.Point(0, 49);
            this._formulaBar.Name = "_formulaBar";
            this._formulaBar.Size = new System.Drawing.Size(900, 26);
            this._formulaBar.TabIndex = 2;
            //
            // _sheetTab
            //
            this._sheetTab.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._sheetTab.Location = new System.Drawing.Point(0, 554);
            this._sheetTab.Name = "_sheetTab";
            this._sheetTab.Size = new System.Drawing.Size(900, 24);
            this._sheetTab.TabIndex = 4;
            //
            // _statusBar
            //
            this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._statusLabel
            });
            this._statusBar.Location = new System.Drawing.Point(0, 578);
            this._statusBar.Name = "_statusBar";
            this._statusBar.Size = new System.Drawing.Size(900, 22);
            this._statusBar.TabIndex = 5;
            //
            // _statusLabel
            //
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Text = "준비";
            //
            // _spreadsheet
            //
            this._spreadsheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this._spreadsheet.Location = new System.Drawing.Point(0, 75);
            this._spreadsheet.Name = "_spreadsheet";
            this._spreadsheet.Size = new System.Drawing.Size(900, 479);
            this._spreadsheet.TabIndex = 3;
            //
            // FormReportDesigner
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this._spreadsheet);
            this.Controls.Add(this._sheetTab);
            this.Controls.Add(this._statusBar);
            this.Controls.Add(this._formulaBar);
            this.Controls.Add(this._toolBar);
            this.Controls.Add(this._menuStrip);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.MainMenuStrip = this._menuStrip;
            this.Name = "FormReportDesigner";
            this.Text = "새 리포트";
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this._toolBar.ResumeLayout(false);
            this._toolBar.PerformLayout();
            this._statusBar.ResumeLayout(false);
            this._statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem _menuFile;
        private System.Windows.Forms.ToolStripMenuItem _menuFileNew;
        private System.Windows.Forms.ToolStripMenuItem _menuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem _menuFileSave;
        private System.Windows.Forms.ToolStripMenuItem _menuFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator _menuFileSep1;
        private System.Windows.Forms.ToolStripMenuItem _menuFileClose;
        private System.Windows.Forms.ToolStripMenuItem _menuEdit;
        private System.Windows.Forms.ToolStripMenuItem _menuEditUndo;
        private System.Windows.Forms.ToolStripMenuItem _menuEditRedo;
        private System.Windows.Forms.ToolStripMenuItem _menuFormat;
        private System.Windows.Forms.ToolStripMenuItem _menuFormatCells;
        private System.Windows.Forms.ToolStrip _toolBar;
        private System.Windows.Forms.ToolStripComboBox _cmbFontName;
        private System.Windows.Forms.ToolStripComboBox _cmbFontSize;
        private System.Windows.Forms.ToolStripButton _btnBold;
        private System.Windows.Forms.ToolStripButton _btnItalic;
        private System.Windows.Forms.ToolStripButton _btnUnderline;
        private System.Windows.Forms.ToolStripSeparator _toolBarSep1;
        private System.Windows.Forms.ToolStripButton _btnAlignLeft;
        private System.Windows.Forms.ToolStripButton _btnAlignCenter;
        private System.Windows.Forms.ToolStripButton _btnAlignRight;
        private System.Windows.Forms.ToolStripSeparator _toolBarSep2;
        private System.Windows.Forms.ToolStripButton _btnBorderAll;
        private Reporting.Designer.Controls.FormulaBar _formulaBar;
        private Reporting.Designer.Controls.SpreadsheetControl _spreadsheet;
        private Reporting.Designer.Controls.SheetTabControl _sheetTab;
        private System.Windows.Forms.StatusStrip _statusBar;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
    }
}

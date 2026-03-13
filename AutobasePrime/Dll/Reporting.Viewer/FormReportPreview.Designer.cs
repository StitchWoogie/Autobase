namespace Reporting.Viewer
{
    partial class FormReportPreview
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
            this._toolBar = new System.Windows.Forms.ToolStrip();
            this._btnPrint = new System.Windows.Forms.ToolStripButton();
            this._btnClose = new System.Windows.Forms.ToolStripButton();
            this._toolBarSep1 = new System.Windows.Forms.ToolStripSeparator();
            this._btnPrevPage = new System.Windows.Forms.ToolStripButton();
            this._lblPage = new System.Windows.Forms.ToolStripLabel();
            this._btnNextPage = new System.Windows.Forms.ToolStripButton();
            this._panelPreview = new System.Windows.Forms.Panel();
            this._toolBar.SuspendLayout();
            this.SuspendLayout();
            //
            // _toolBar
            //
            this._toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._btnPrint,
                this._btnClose,
                this._toolBarSep1,
                this._btnPrevPage,
                this._lblPage,
                this._btnNextPage
            });
            this._toolBar.Location = new System.Drawing.Point(0, 0);
            this._toolBar.Name = "_toolBar";
            this._toolBar.Size = new System.Drawing.Size(800, 25);
            this._toolBar.TabIndex = 0;
            //
            // _btnPrint
            //
            this._btnPrint.Name = "_btnPrint";
            this._btnPrint.Text = "인쇄";
            this._btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnPrint.Enabled = false;
            //
            // _btnClose
            //
            this._btnClose.Name = "_btnClose";
            this._btnClose.Text = "닫기";
            this._btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnClose.Click += new System.EventHandler(delegate { this.Close(); });
            //
            // _toolBarSep1
            //
            this._toolBarSep1.Name = "_toolBarSep1";
            //
            // _btnPrevPage
            //
            this._btnPrevPage.Name = "_btnPrevPage";
            this._btnPrevPage.Text = "<";
            this._btnPrevPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnPrevPage.ToolTipText = "이전 페이지";
            this._btnPrevPage.Enabled = false;
            //
            // _lblPage
            //
            this._lblPage.Name = "_lblPage";
            this._lblPage.Text = "1 / 1";
            //
            // _btnNextPage
            //
            this._btnNextPage.Name = "_btnNextPage";
            this._btnNextPage.Text = ">";
            this._btnNextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnNextPage.ToolTipText = "다음 페이지";
            this._btnNextPage.Enabled = false;
            //
            // _panelPreview
            //
            this._panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelPreview.Location = new System.Drawing.Point(0, 25);
            this._panelPreview.Name = "_panelPreview";
            this._panelPreview.Size = new System.Drawing.Size(800, 575);
            this._panelPreview.TabIndex = 1;
            this._panelPreview.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this._panelPreview.AutoScroll = true;
            //
            // FormReportPreview
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this._panelPreview);
            this.Controls.Add(this._toolBar);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.Name = "FormReportPreview";
            this.Text = "인쇄 미리보기";
            this._toolBar.ResumeLayout(false);
            this._toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip _toolBar;
        private System.Windows.Forms.ToolStripButton _btnPrint;
        private System.Windows.Forms.ToolStripButton _btnClose;
        private System.Windows.Forms.ToolStripSeparator _toolBarSep1;
        private System.Windows.Forms.ToolStripButton _btnPrevPage;
        private System.Windows.Forms.ToolStripLabel _lblPage;
        private System.Windows.Forms.ToolStripButton _btnNextPage;
        private System.Windows.Forms.Panel _panelPreview;
    }
}

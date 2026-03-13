namespace Reporting.Viewer
{
    partial class FormReportViewer
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
            this._btnExportXlsx = new System.Windows.Forms.ToolStripButton();
            this._btnExportCsv = new System.Windows.Forms.ToolStripButton();
            this._btnExportPdf = new System.Windows.Forms.ToolStripButton();
            this._toolBarSep1 = new System.Windows.Forms.ToolStripSeparator();
            this._btnPrint = new System.Windows.Forms.ToolStripButton();
            this._btnPrintPreview = new System.Windows.Forms.ToolStripButton();
            this._toolBarSep2 = new System.Windows.Forms.ToolStripSeparator();
            this._btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this._btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this._lblZoom = new System.Windows.Forms.ToolStripLabel();
            this._sheetView = new Reporting.Viewer.Controls.SheetViewControl();
            this._sheetTab = new Reporting.Viewer.Controls.ViewerSheetTabControl();
            this._statusBar = new System.Windows.Forms.StatusStrip();
            this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._statusCellInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this._toolBar.SuspendLayout();
            this._statusBar.SuspendLayout();
            this.SuspendLayout();
            //
            // _toolBar
            //
            this._toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._btnExportXlsx,
                this._btnExportCsv,
                this._btnExportPdf,
                this._toolBarSep1,
                this._btnPrint,
                this._btnPrintPreview,
                this._toolBarSep2,
                this._btnZoomIn,
                this._btnZoomOut,
                this._lblZoom
            });
            this._toolBar.Location = new System.Drawing.Point(0, 0);
            this._toolBar.Name = "_toolBar";
            this._toolBar.Size = new System.Drawing.Size(800, 25);
            this._toolBar.TabIndex = 0;
            //
            // _btnExportXlsx
            //
            this._btnExportXlsx.Name = "_btnExportXlsx";
            this._btnExportXlsx.Text = "XLSX";
            this._btnExportXlsx.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnExportXlsx.ToolTipText = "Excel 파일로 내보내기";
            this._btnExportXlsx.Click += new System.EventHandler(this.BtnExportXlsx_Click);
            //
            // _btnExportCsv
            //
            this._btnExportCsv.Name = "_btnExportCsv";
            this._btnExportCsv.Text = "CSV";
            this._btnExportCsv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnExportCsv.ToolTipText = "CSV 파일로 내보내기";
            this._btnExportCsv.Click += new System.EventHandler(this.BtnExportCsv_Click);
            //
            // _btnExportPdf
            //
            this._btnExportPdf.Name = "_btnExportPdf";
            this._btnExportPdf.Text = "PDF";
            this._btnExportPdf.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnExportPdf.ToolTipText = "PDF 파일로 내보내기";
            this._btnExportPdf.Enabled = false;
            //
            // _toolBarSep1
            //
            this._toolBarSep1.Name = "_toolBarSep1";
            //
            // _btnPrint
            //
            this._btnPrint.Name = "_btnPrint";
            this._btnPrint.Text = "인쇄";
            this._btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnPrint.ToolTipText = "인쇄";
            this._btnPrint.Enabled = false;
            //
            // _btnPrintPreview
            //
            this._btnPrintPreview.Name = "_btnPrintPreview";
            this._btnPrintPreview.Text = "미리보기";
            this._btnPrintPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnPrintPreview.ToolTipText = "인쇄 미리보기";
            this._btnPrintPreview.Enabled = false;
            //
            // _toolBarSep2
            //
            this._toolBarSep2.Name = "_toolBarSep2";
            //
            // _btnZoomIn
            //
            this._btnZoomIn.Name = "_btnZoomIn";
            this._btnZoomIn.Text = "+";
            this._btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnZoomIn.ToolTipText = "확대";
            //
            // _btnZoomOut
            //
            this._btnZoomOut.Name = "_btnZoomOut";
            this._btnZoomOut.Text = "-";
            this._btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._btnZoomOut.ToolTipText = "축소";
            //
            // _lblZoom
            //
            this._lblZoom.Name = "_lblZoom";
            this._lblZoom.Text = "100%";
            //
            // _sheetView
            //
            this._sheetView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._sheetView.Location = new System.Drawing.Point(0, 25);
            this._sheetView.Name = "_sheetView";
            this._sheetView.Size = new System.Drawing.Size(800, 431);
            this._sheetView.TabIndex = 1;
            //
            // _sheetTab
            //
            this._sheetTab.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._sheetTab.Location = new System.Drawing.Point(0, 456);
            this._sheetTab.Name = "_sheetTab";
            this._sheetTab.Size = new System.Drawing.Size(800, 24);
            this._sheetTab.TabIndex = 2;
            //
            // _statusBar
            //
            this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._statusLabel,
                this._statusCellInfo
            });
            this._statusBar.Location = new System.Drawing.Point(0, 480);
            this._statusBar.Name = "_statusBar";
            this._statusBar.Size = new System.Drawing.Size(800, 22);
            this._statusBar.TabIndex = 3;
            //
            // _statusLabel
            //
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Text = "준비";
            this._statusLabel.Spring = true;
            this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _statusCellInfo
            //
            this._statusCellInfo.Name = "_statusCellInfo";
            this._statusCellInfo.Text = "";
            this._statusCellInfo.AutoSize = false;
            this._statusCellInfo.Size = new System.Drawing.Size(200, 17);
            this._statusCellInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // FormReportViewer
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 502);
            this.Controls.Add(this._sheetView);
            this.Controls.Add(this._sheetTab);
            this.Controls.Add(this._statusBar);
            this.Controls.Add(this._toolBar);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.Name = "FormReportViewer";
            this.Text = "리포트 뷰어";
            this._toolBar.ResumeLayout(false);
            this._toolBar.PerformLayout();
            this._statusBar.ResumeLayout(false);
            this._statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip _toolBar;
        private System.Windows.Forms.ToolStripButton _btnExportXlsx;
        private System.Windows.Forms.ToolStripButton _btnExportCsv;
        private System.Windows.Forms.ToolStripButton _btnExportPdf;
        private System.Windows.Forms.ToolStripSeparator _toolBarSep1;
        private System.Windows.Forms.ToolStripButton _btnPrint;
        private System.Windows.Forms.ToolStripButton _btnPrintPreview;
        private System.Windows.Forms.ToolStripSeparator _toolBarSep2;
        private System.Windows.Forms.ToolStripButton _btnZoomIn;
        private System.Windows.Forms.ToolStripButton _btnZoomOut;
        private System.Windows.Forms.ToolStripLabel _lblZoom;
        private Reporting.Viewer.Controls.SheetViewControl _sheetView;
        private Reporting.Viewer.Controls.ViewerSheetTabControl _sheetTab;
        private System.Windows.Forms.StatusStrip _statusBar;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel _statusCellInfo;
    }
}

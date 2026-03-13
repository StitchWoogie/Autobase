using System;
using System.Drawing;
using System.Windows.Forms;
using GraphicModule;
using DialogTag;
using NetTools;

namespace Studio
{
	public class PropertyPageObjectBarcodeScanner : Form
	{
		private Label lblScannerName;
		private TextBox txtScannerName;
		private Label lblResultTag;
		private TextBox txtResultTag;
		private Button btnResultTagSelect;
		private CheckBox chkShowLastScan;
		private CheckBox chkShowStatus;
		private Label lblValidColor;
		private Button btnValidColor;
		private Label lblInvalidColor;
		private Button btnInvalidColor;

		private ObjectArgsBarcodeScanner _objectArgs;

		public ObjectArgsBarcodeScanner ObjectArgs
		{
			get
			{
				_objectArgs.sScannerName = txtScannerName.Text;
				_objectArgs.sResultTagName = txtResultTag.Text;
				_objectArgs.bShowLastScan = chkShowLastScan.Checked;
				_objectArgs.bShowStatus = chkShowStatus.Checked;
				_objectArgs.nValidColor = btnValidColor.BackColor.ToArgb();
				_objectArgs.nInvalidColor = btnInvalidColor.BackColor.ToArgb();
				return _objectArgs;
			}
			set
			{
				_objectArgs = value ?? new ObjectArgsBarcodeScanner();
				txtScannerName.Text = _objectArgs.sScannerName ?? "";
				txtResultTag.Text = _objectArgs.sResultTagName ?? "";
				chkShowLastScan.Checked = _objectArgs.bShowLastScan;
				chkShowStatus.Checked = _objectArgs.bShowStatus;
				btnValidColor.BackColor = _objectArgs.nValidColor != 0
					? Color.FromArgb(_objectArgs.nValidColor) : Color.Green;
				btnInvalidColor.BackColor = _objectArgs.nInvalidColor != 0
					? Color.FromArgb(_objectArgs.nInvalidColor) : Color.Red;
			}
		}

		public PropertyPageObjectBarcodeScanner()
		{
			InitializeComponent();
			InitializeData();
			ApplyLanguage();
		}

        #region Designer Code

        private void InitializeComponent()
        {
            this.lblScannerName = new Label();
            this.txtScannerName = new TextBox();
            this.lblResultTag = new Label();
            this.txtResultTag = new TextBox();
            this.btnResultTagSelect = new Button();
            this.chkShowLastScan = new CheckBox();
            this.chkShowStatus = new CheckBox();
            this.lblValidColor = new Label();
            this.btnValidColor = new Button();
            this.lblInvalidColor = new Label();
            this.btnInvalidColor = new Button();
            this.SuspendLayout();

            // lblScannerName
            this.lblScannerName.Location = new Point(12, 15);
            this.lblScannerName.Size = new Size(80, 20);

            // txtScannerName
            this.txtScannerName.Location = new Point(100, 12);
            this.txtScannerName.Size = new Size(180, 21);

            // lblResultTag
            this.lblResultTag.Location = new Point(12, 45);
            this.lblResultTag.Size = new Size(80, 20);

            // txtResultTag
            this.txtResultTag.Location = new Point(100, 42);
            this.txtResultTag.Size = new Size(150, 21);

            // btnResultTagSelect
            this.btnResultTagSelect.Location = new Point(255, 41);
            this.btnResultTagSelect.Size = new Size(26, 23);
            this.btnResultTagSelect.Text = "...";

            // chkShowLastScan
            this.chkShowLastScan.Location = new Point(100, 72);
            this.chkShowLastScan.Size = new Size(180, 20);

            // chkShowStatus
            this.chkShowStatus.Location = new Point(100, 95);
            this.chkShowStatus.Size = new Size(180, 20);

            // lblValidColor
            this.lblValidColor.Location = new Point(12, 125);
            this.lblValidColor.Size = new Size(80, 20);

            // btnValidColor
            this.btnValidColor.Location = new Point(100, 123);
            this.btnValidColor.Size = new Size(60, 22);
            this.btnValidColor.FlatStyle = FlatStyle.Flat;
            this.btnValidColor.BackColor = Color.Green;

            // lblInvalidColor
            this.lblInvalidColor.Location = new Point(12, 155);
            this.lblInvalidColor.Size = new Size(80, 20);

            // btnInvalidColor
            this.btnInvalidColor.Location = new Point(100, 153);
            this.btnInvalidColor.Size = new Size(60, 22);
            this.btnInvalidColor.FlatStyle = FlatStyle.Flat;
            this.btnInvalidColor.BackColor = Color.Red;

            // PropertyPageObjectBarcodeScanner
            this.ClientSize = new Size(300, 185);
            this.Controls.Add(this.lblScannerName);
            this.Controls.Add(this.txtScannerName);
            this.Controls.Add(this.lblResultTag);
            this.Controls.Add(this.txtResultTag);
            this.Controls.Add(this.btnResultTagSelect);
            this.Controls.Add(this.chkShowLastScan);
            this.Controls.Add(this.chkShowStatus);
            this.Controls.Add(this.lblValidColor);
            this.Controls.Add(this.btnValidColor);
            this.Controls.Add(this.lblInvalidColor);
            this.Controls.Add(this.btnInvalidColor);
            this.Name = "PropertyPageObjectBarcodeScanner";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        /// <summary>
        /// 이벤트 핸들러 연결
        /// </summary>
        private void InitializeData()
		{
			btnResultTagSelect.Click += BtnResultTagSelect_Click;
			btnValidColor.Click += BtnColor_Click;
			btnInvalidColor.Click += BtnColor_Click;
		}

		/// <summary>
		/// 다국어 텍스트 적용 (한글/영어)
		/// </summary>
		private void ApplyLanguage()
		{
			bool kor = Tools.IsLangKorean();

			this.Text = kor ? "바코드 스캐너" : "Barcode Scanner";
			lblScannerName.Text = kor ? "스캐너" : "Scanner";
			lblResultTag.Text = kor ? "결과 태그" : "Result Tag";
			chkShowLastScan.Text = kor ? "최근 스캔 표시" : "Show Last Scan";
			chkShowStatus.Text = kor ? "상태 표시" : "Show Status";
			lblValidColor.Text = kor ? "정상 색상" : "Valid Color";
			lblInvalidColor.Text = kor ? "오류 색상" : "Invalid Color";
		}

		private void BtnResultTagSelect_Click(object sender, EventArgs e)
		{
			string tag, des;
			if (DialogTag.SelectTag.SelectAll(this, out tag, out des) != DialogResult.OK) return;
			txtResultTag.Text = tag;
		}

		private void BtnColor_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			using (var dlg = new FormColorDialog())
			{
				dlg.SetSelectedColor( btn.BackColor, true);
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					btn.BackColor = dlg.GetSelectedColor();
                }
			}
        }
	}
}

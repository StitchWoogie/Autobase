using System;
using System.Drawing;
using System.Windows.Forms;
using GraphicModule;
using DialogTag;
using NetTools;

namespace Studio
{
	public class PropertyPageObjectBarcodeDisplay : Form
	{
		private Label lblFormat;
		private ComboBox comboFormat;
		private Label lblTextSource;
		private TextBox txtTextSource;
		private Label lblTagTemplate;
		private TextBox txtTagTemplate;
		private Button btnTagSelect;
		private CheckBox chkAutoUpdate;
		private Label lblErrorCorrection;
		private ComboBox comboErrorCorrection;
		private CheckBox chkAutoSave;
		private Label lblSavePath;
		private TextBox txtSavePath;
		private Button btnBrowseSavePath;
		private CheckBox chkShowHumanReadableText;

		private ObjectArgsBarcodeDisplay _objectArgs;

		public ObjectArgsBarcodeDisplay ObjectArgs
		{
			get
			{
				_objectArgs.nBarcodeFormat = comboFormat.SelectedIndex;
				_objectArgs.sTextSource = txtTextSource.Text;
				_objectArgs.sTagBindingTemplate = txtTagTemplate.Text;
				_objectArgs.bAutoUpdate = chkAutoUpdate.Checked;
				_objectArgs.nErrorCorrectionLevel = comboErrorCorrection.SelectedIndex;
				_objectArgs.bAutoSave = chkAutoSave.Checked;
				_objectArgs.sSavePath = txtSavePath.Text;
				_objectArgs.bShowHumanReadableText = chkShowHumanReadableText.Checked;
				return _objectArgs;
			}
			set
			{
				_objectArgs = value ?? new ObjectArgsBarcodeDisplay();
				comboFormat.SelectedIndex = Math.Min(_objectArgs.nBarcodeFormat, comboFormat.Items.Count - 1);
				txtTextSource.Text = _objectArgs.sTextSource ?? "";
				txtTagTemplate.Text = _objectArgs.sTagBindingTemplate ?? "";
				chkAutoUpdate.Checked = _objectArgs.bAutoUpdate;
				comboErrorCorrection.SelectedIndex = Math.Min(_objectArgs.nErrorCorrectionLevel, comboErrorCorrection.Items.Count - 1);
				chkAutoSave.Checked = _objectArgs.bAutoSave;
				txtSavePath.Text = _objectArgs.sSavePath ?? "";
				chkShowHumanReadableText.Checked = _objectArgs.bShowHumanReadableText;
				UpdateFormatDependentUI();
			}
		}

		public PropertyPageObjectBarcodeDisplay()
		{
			InitializeComponent();
			InitializeData();
			ApplyLanguage();
		}

        #region Designer Code

        private void InitializeComponent()
        {
            this.lblFormat = new Label();
            this.comboFormat = new ComboBox();
            this.lblTextSource = new Label();
            this.txtTextSource = new TextBox();
            this.lblTagTemplate = new Label();
            this.txtTagTemplate = new TextBox();
            this.btnTagSelect = new Button();
            this.chkAutoUpdate = new CheckBox();
            this.lblErrorCorrection = new Label();
            this.comboErrorCorrection = new ComboBox();
            this.chkAutoSave = new CheckBox();
            this.lblSavePath = new Label();
            this.txtSavePath = new TextBox();
            this.btnBrowseSavePath = new Button();
            this.chkShowHumanReadableText = new CheckBox();
            this.SuspendLayout();

            // lblFormat
            this.lblFormat.Location = new Point(12, 15);
            this.lblFormat.Size = new Size(80, 20);

            // comboFormat
            this.comboFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboFormat.Location = new Point(100, 12);
            this.comboFormat.Size = new Size(140, 21);

            // lblTextSource
            this.lblTextSource.Location = new Point(12, 45);
            this.lblTextSource.Size = new Size(80, 20);

            // txtTextSource
            this.txtTextSource.Location = new Point(100, 42);
            this.txtTextSource.Size = new Size(220, 21);

            // lblTagTemplate
            this.lblTagTemplate.Location = new Point(12, 75);
            this.lblTagTemplate.Size = new Size(80, 20);

            // txtTagTemplate
            this.txtTagTemplate.Location = new Point(100, 72);
            this.txtTagTemplate.Size = new Size(190, 21);

            // btnTagSelect
            this.btnTagSelect.Location = new Point(295, 71);
            this.btnTagSelect.Size = new Size(26, 23);
            this.btnTagSelect.Text = "...";

            // chkAutoUpdate
            this.chkAutoUpdate.Location = new Point(100, 102);
            this.chkAutoUpdate.Size = new Size(200, 20);

            // lblErrorCorrection
            this.lblErrorCorrection.Location = new Point(12, 130);
            this.lblErrorCorrection.Size = new Size(80, 20);

            // comboErrorCorrection
            this.comboErrorCorrection.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboErrorCorrection.Location = new Point(100, 127);
            this.comboErrorCorrection.Size = new Size(140, 21);

            // chkShowHumanReadableText
            this.chkShowHumanReadableText.Location = new Point(100, 155);
            this.chkShowHumanReadableText.Size = new Size(200, 20);

            // chkAutoSave
            this.chkAutoSave.Location = new Point(100, 183);
            this.chkAutoSave.Size = new Size(200, 20);

            // lblSavePath
            this.lblSavePath.Location = new Point(12, 211);
            this.lblSavePath.Size = new Size(80, 20);

            // txtSavePath
            this.txtSavePath.Location = new Point(100, 208);
            this.txtSavePath.Size = new Size(190, 21);

            // btnBrowseSavePath
            this.btnBrowseSavePath.Location = new Point(295, 207);
            this.btnBrowseSavePath.Size = new Size(26, 23);
            this.btnBrowseSavePath.Text = "...";

            // PropertyPageObjectBarcodeDisplay
            this.ClientSize = new Size(340, 243);
            this.Controls.Add(this.lblFormat);
            this.Controls.Add(this.comboFormat);
            this.Controls.Add(this.lblTextSource);
            this.Controls.Add(this.txtTextSource);
            this.Controls.Add(this.lblTagTemplate);
            this.Controls.Add(this.txtTagTemplate);
            this.Controls.Add(this.btnTagSelect);
            this.Controls.Add(this.chkAutoUpdate);
            this.Controls.Add(this.lblErrorCorrection);
            this.Controls.Add(this.comboErrorCorrection);
            this.Controls.Add(this.chkShowHumanReadableText);
            this.Controls.Add(this.chkAutoSave);
            this.Controls.Add(this.lblSavePath);
            this.Controls.Add(this.txtSavePath);
            this.Controls.Add(this.btnBrowseSavePath);
            this.Name = "PropertyPageObjectBarcodeDisplay";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        /// <summary>
        /// ComboBox 데이터, 기본값, 이벤트 핸들러 연결
        /// </summary>
        private void InitializeData()
		{
			comboFormat.Items.AddRange(new object[] {
				"QR Code", "DataMatrix", "Code128", "Code39", "EAN-13",
				"EAN-8", "UPC-A", "ITF-14", "PDF417"
			});
			comboFormat.SelectedIndex = 0;

			comboErrorCorrection.Items.AddRange(new object[] {
				"L (7%)", "M (15%)", "Q (25%)", "H (30%)"
			});
			comboErrorCorrection.SelectedIndex = 2;

			chkShowHumanReadableText.Checked = true;

			// 이벤트 핸들러
			comboFormat.SelectedIndexChanged += ComboFormat_SelectedIndexChanged;
			btnTagSelect.Click += BtnTagSelect_Click;
			btnBrowseSavePath.Click += BtnBrowseSavePath_Click;
		}

		/// <summary>
		/// 다국어 텍스트 적용 (한글/영어)
		/// </summary>
		private void ApplyLanguage()
		{
			bool kor = Tools.IsLangKorean();

			this.Text = kor ? "바코드 디스플레이" : "Barcode Display";
			lblFormat.Text = kor ? "포맷" : "Format";
			lblTextSource.Text = kor ? "텍스트" : "Text";
			lblTagTemplate.Text = kor ? "태그 템플릿" : "Tag Template";
			chkAutoUpdate.Text = kor ? "자동 업데이트" : "Auto Update";
			lblErrorCorrection.Text = kor ? "오류 정정" : "EC Level";
			chkShowHumanReadableText.Text = kor ? "하단 텍스트 표시" : "Show Text";
			chkAutoSave.Text = kor ? "이미지 자동 저장" : "Auto Save Image";
			lblSavePath.Text = kor ? "저장 경로" : "Save Path";
		}

		/// <summary>
		/// 포맷 변경 시 포맷 의존적 UI를 업데이트한다.
		/// QR/DataMatrix: EC Level 표시, Human Readable Text 숨김
		/// 1D 바코드: EC Level 숨김, Human Readable Text 표시
		/// </summary>
		private void ComboFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateFormatDependentUI();
		}

		private void UpdateFormatDependentUI()
		{
			int idx = comboFormat.SelectedIndex;
			bool is2D = (idx == 0 || idx == 1 || idx == 8); // QR, DataMatrix, PDF417

			// EC Level은 QR Code에서만 의미 있음
			lblErrorCorrection.Visible = (idx == 0);
			comboErrorCorrection.Visible = (idx == 0);

			// Human Readable Text는 1D 바코드에서만 의미 있음
			chkShowHumanReadableText.Visible = !is2D;
		}

		private void BtnTagSelect_Click(object sender, EventArgs e)
		{
			string tag, des;
			if (DialogTag.SelectTag.SelectAll(this, out tag, out des) != DialogResult.OK) return;

			// 커서 위치에 {Tag.xxx} 형태로 삽입
			string insert = "{Tag." + tag + "}";
			int pos = txtTagTemplate.SelectionStart;
			txtTagTemplate.Text = txtTagTemplate.Text.Insert(pos, insert);
			txtTagTemplate.SelectionStart = pos + insert.Length;
			txtTagTemplate.Focus();
		}

		private void BtnBrowseSavePath_Click(object sender, EventArgs e)
		{
			using (var dlg = new SaveFileDialog())
			{
				dlg.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap|*.bmp|All Files|*.*";
				dlg.DefaultExt = "png";
				dlg.FileName = txtSavePath.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					txtSavePath.Text = dlg.FileName;
				}
			}
		}
	}
}

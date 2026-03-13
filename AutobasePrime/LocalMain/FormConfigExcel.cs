using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;
using System.IO;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormConfigExcel.
	/// </summary>
	public class FormConfigExcel : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxPath;
		private System.Windows.Forms.Button buttonFind;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
        private Button btnExcelFind;
        private Button buttonXLStart;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

		public FormConfigExcel()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigExcel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.btnExcelFind = new System.Windows.Forms.Button();
            this.buttonXLStart = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonFind);
            this.groupBox1.Controls.Add(this.textBoxPath);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonFind
            // 
            resources.ApplyResources(this.buttonFind, "buttonFind");
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // textBoxPath
            // 
            resources.ApplyResources(this.textBoxPath, "textBoxPath");
            this.textBoxPath.Name = "textBoxPath";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // btnExcelFind
            // 
            resources.ApplyResources(this.btnExcelFind, "btnExcelFind");
            this.btnExcelFind.Name = "btnExcelFind";
            this.btnExcelFind.UseVisualStyleBackColor = true;
            this.btnExcelFind.Click += new System.EventHandler(this.btnExcelFind_Click);
            // 
            // buttonXLStart
            // 
            resources.ApplyResources(this.buttonXLStart, "buttonXLStart");
            this.buttonXLStart.Name = "buttonXLStart";
            this.buttonXLStart.UseVisualStyleBackColor = true;
            this.buttonXLStart.Click += new System.EventHandler(this.buttonXLStart_Click);
            // 
            // FormConfigExcel
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonXLStart);
            this.Controls.Add(this.btnExcelFind);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigExcel";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigExcel_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigExcel_Load(object sender, System.EventArgs e)
		{
			textBoxPath.Text = TotalConfig.LoadRegAutoBaseConfig("ExcelReport", null, "ExcelPath", "C:\\Program Files\\Microsoft Office\\Office\\EXCEL.EXE");

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.textBoxPath.ReadOnly = true;
                this.btnExcelFind.Visible = false; //260121 PSU 추가.
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_CONFIG_ETC))
                this.buttonOK.Enabled = false;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			TotalConfig.SaveRegAutoBaseConfig("ExcelReport", null, "ExcelPath", textBoxPath.Text); 
			Close();
		}

		private void buttonFind_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "EXE files (*.exe) |*.exe";

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				textBoxPath.Text = dialog.FileName;
			}
		}


        // ========================= Excel Add-in XLStart 복사 기능 및 엑셀 경로 자동 찾기 기능 추가  260121 PSU =========================
        private void btnExcelFind_Click(object sender, EventArgs e)
        {
            using (var dlg = new FormSelectExcelPath())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string excelPath = dlg.SelectedExcelPath;

                    textBoxPath.Text = excelPath;
                }
            }
        }

        private void buttonXLStart_Click(object sender, EventArgs e)
        {
            try
            {
                string excelExePath = textBoxPath.Text;
                if (string.IsNullOrEmpty(excelExePath))
                {
                    if (Tools.IsLangKorean())
                        throw new ArgumentException("excelExePath가 비어 있습니다.");
                    else
                        throw new ArgumentException("excelExePath is empty.");
                }

                DialogResult dr;
                if (Tools.IsLangKorean())
                {
                    dr = MessageBox.Show(
                        this,
                        "Excel 추가기능(ExcelReport.xla)을 XLStart 폴더에 새로 복사하시겠습니까?\n" +
                        "기존 파일이 있으면 삭제 후 다시 설치됩니다.",
                        "Excel Add-in 설치",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                }
                else
                {
                    dr = MessageBox.Show(
                        this,
                        "Do you want to copy the Excel add-in (ExcelReport.xla) to the XLStart folder?\n" +
                        "If an existing file is found, it will be deleted and reinstalled.",
                        "Excel Add-in Installation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                }

                if (dr != DialogResult.Yes)
                    return;

                CopyAddinToXLStart(excelExePath);

                if (Tools.IsLangKorean())
                    MessageBox.Show("Excel Add-in 설치 완료");
                else 
                    MessageBox.Show("Excel Add-in installation completed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public void CopyAddinToXLStart(string excelExePath)
        {      
            // EXCEL.EXE가 있는 폴더
            string excelDir = Path.GetDirectoryName(excelExePath);
            if (string.IsNullOrEmpty(excelDir))
                if (Tools.IsLangKorean())
                    throw new InvalidOperationException("잘못된 Excel 디렉토리입니다.");
                else
                    throw new InvalidOperationException("Invalid Excel directory.");

            // 대상 XLStart 폴더
            string targetXLStart = Path.Combine(excelDir, "XLStart");

            // 소스 파일
            string sourceFile = Path.Combine(
                Application.StartupPath,
                @"XLStart\ExcelReport.xla"
            );

            if (!File.Exists(sourceFile))
                if (Tools.IsLangKorean())
                    throw new FileNotFoundException("ExcelReport.xla 추가 기능 파일을 찾을 수 없습니다.", sourceFile);
                else
                    throw new FileNotFoundException("ExcelReport.xla add-in not found.", sourceFile);

            // XLStart 폴더 생성
            if (!Directory.Exists(targetXLStart))
                Directory.CreateDirectory(targetXLStart);

            string targetFile = Path.Combine(targetXLStart, "ExcelReport.xla");

            // 기존 파일 삭제
            if (File.Exists(targetFile))
            {
                File.SetAttributes(targetFile, FileAttributes.Normal);
                File.Delete(targetFile);
            }

            // 새로 복사
            File.Copy(sourceFile, targetFile, true);
        }
    }
}

using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AutobaseRESTAPIMonitor.FormRestApiMonitor;

namespace AutobaseRESTAPIMonitor
{
    public partial class FormErrorReport : Form
    {
        private TextBox txtErrorDetails;
        private Button btnCopy;
        private Button btnClose;

        public FormErrorReport(Exception ex)
        {
            InitializeErrorForm(ex);
        }

        private void InitializeErrorForm(Exception ex)
        {
            this.Text = GlobalSettings.IsKorean ? "오류 보고" : "Error Report";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 텍스트박스 설정
            txtErrorDetails = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Top,
                Height = 300
            };

            // 오류 정보 구성
            string errorInfo = $"{(GlobalSettings.IsKorean ? "발생 시간" : "Time")}: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n" +
                              $"{(GlobalSettings.IsKorean ? "오류 유형" : "Error Type")}: {ex.GetType().FullName}\r\n" +
                              $"{(GlobalSettings.IsKorean ? "오류 메시지" : "Error Message")}: {ex.Message}\r\n\r\n" +
                              $"{(GlobalSettings.IsKorean ? "스택 추적" : "Stack Trace")}:\r\n{ex.StackTrace}";

            txtErrorDetails.Text = errorInfo;

            // 버튼 패널
            Panel buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom
            };

            // 복사 버튼
            btnCopy = new Button
            {
                Text = GlobalSettings.IsKorean ? "클립보드에 복사" : "Copy to Clipboard",
                Width = 150,
                Location = new Point(150, 10)
            };
            btnCopy.Click += (s, e) => {
                try
                {
                    Clipboard.SetText(txtErrorDetails.Text);
                    MessageBox.Show(
                        GlobalSettings.IsKorean ? "클립보드에 복사되었습니다." : "Copied to clipboard.",
                        GlobalSettings.IsKorean ? "알림" : "Notice",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show(
                        GlobalSettings.IsKorean ? "클립보드 복사 실패" : "Failed to copy to clipboard",
                        GlobalSettings.IsKorean ? "오류" : "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };

            this.FormClosing += (s, e) =>
            {
                // 폼이 닫힐 때 프로세스 종료
                Application.Exit();
            };


            // 닫기 버튼
            btnClose = new Button
            {
                Text = GlobalSettings.IsKorean ? "닫기" : "Close",
                Width = 100,
                Location = new Point(320, 10)
            };
            btnClose.Click += (s, e) =>
            {
                this.Close(); // 이것이 FormClosing 이벤트를 트리거하여 Application.Exit()가 호출됨
            };

            // 컨트롤 추가
            buttonPanel.Controls.Add(btnCopy);
            buttonPanel.Controls.Add(btnClose);

            this.Controls.Add(txtErrorDetails);
            this.Controls.Add(buttonPanel);

            // 로그 파일에도 기록
            try
            {
                string logPath = Path.Combine("C:/Autobase/RESTAPIMonitor_LOG", "error_log.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, errorInfo + "\r\n----------------------------------------\r\n");
            }
            catch
            {
                // 로그 저장 실패는 무시
            }
        }
    }
}

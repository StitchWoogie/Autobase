using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reporting.Designer.Controls
{
    /// <summary>
    /// 수식 입력바 (셀 주소 + 수식/값 편집)
    /// </summary>
    public class FormulaBar : UserControl
    {
        private TextBox _txtAddress;
        private TextBox _txtFormula;
        private Label _lblSeparator;

        /// <summary>수식/값이 확정될 때 발생</summary>
        public event EventHandler<string> FormulaCommitted;

        /// <summary>셀 주소 입력 후 Enter 시 발생</summary>
        public event EventHandler<string> AddressNavigated;

        public FormulaBar()
        {
            Height = 26;
            Dock = DockStyle.Top;
            BackColor = Color.White;
            InitializeControls();
        }

        private void InitializeControls()
        {
            _txtAddress = new TextBox();
            _txtAddress.Location = new Point(2, 2);
            _txtAddress.Size = new Size(80, 22);
            _txtAddress.Font = new Font("맑은 고딕", 9f);
            _txtAddress.TextAlign = HorizontalAlignment.Center;
            _txtAddress.KeyDown += TxtAddress_KeyDown;
            Controls.Add(_txtAddress);

            _lblSeparator = new Label();
            _lblSeparator.Location = new Point(84, 0);
            _lblSeparator.Size = new Size(2, 26);
            _lblSeparator.BackColor = Color.FromArgb(208, 215, 229);
            Controls.Add(_lblSeparator);

            _txtFormula = new TextBox();
            _txtFormula.Location = new Point(88, 2);
            _txtFormula.Size = new Size(Width - 90, 22);
            _txtFormula.Font = new Font("맑은 고딕", 9f);
            _txtFormula.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            _txtFormula.KeyDown += TxtFormula_KeyDown;
            Controls.Add(_txtFormula);
        }

        /// <summary>셀 주소 표시 업데이트</summary>
        public void UpdateAddress(string address)
        {
            _txtAddress.Text = address;
        }

        /// <summary>수식/값 표시 업데이트</summary>
        public void UpdateFormula(string text)
        {
            _txtFormula.Text = text;
        }

        /// <summary>수식바에 포커스</summary>
        public void FocusFormula()
        {
            _txtFormula.Focus();
            _txtFormula.SelectAll();
        }

        private void TxtFormula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FormulaCommitted?.Invoke(this, _txtFormula.Text);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                // 포커스를 스프레드시트로 되돌림
                Parent?.SelectNextControl(_txtFormula, true, true, true, true);
                e.Handled = true;
            }
        }

        private void TxtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddressNavigated?.Invoke(this, _txtAddress.Text.Trim());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // 하단 구분선
            using (var pen = new Pen(Color.FromArgb(208, 215, 229)))
                e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
        }
    }
}

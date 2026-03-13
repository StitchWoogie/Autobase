using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Reporting.Engine.Model;

namespace Reporting.Viewer.Controls
{
    /// <summary>
    /// 뷰어용 시트 탭 컨트롤 (하단 시트 전환)
    /// </summary>
    public class ViewerSheetTabControl : UserControl
    {
        private List<string> _sheetNames = new List<string>();
        private int _activeIndex;
        private const int TabHeight = 24;
        private const int TabPadding = 20;
        private const int MinTabWidth = 60;

        /// <summary>활성 시트 변경 이벤트</summary>
        public event EventHandler<int> ActiveSheetChanged;

        public ViewerSheetTabControl()
        {
            Height = TabHeight;
            Dock = DockStyle.Bottom;
            BackColor = Color.FromArgb(242, 242, 242);

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint, true);
        }

        /// <summary>시트 이름 목록 설정</summary>
        public void SetSheets(IList<string> names, int activeIndex)
        {
            _sheetNames.Clear();
            if (names != null)
                _sheetNames.AddRange(names);
            _activeIndex = activeIndex;
            Invalidate();
        }

        /// <summary>활성 시트 인덱스</summary>
        public int ActiveIndex
        {
            get => _activeIndex;
            set
            {
                if (value >= 0 && value < _sheetNames.Count && value != _activeIndex)
                {
                    _activeIndex = value;
                    Invalidate();
                    ActiveSheetChanged?.Invoke(this, _activeIndex);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (var pen = new Pen(Color.FromArgb(208, 215, 229)))
                g.DrawLine(pen, 0, 0, Width, 0);

            var font = new Font("맑은 고딕", 8.5f);
            int x = 4;

            for (int i = 0; i < _sheetNames.Count; i++)
            {
                string name = _sheetNames[i];
                int textWidth = (int)g.MeasureString(name, font).Width;
                int tabWidth = Math.Max(textWidth + TabPadding, MinTabWidth);

                var tabRect = new Rectangle(x, 2, tabWidth, TabHeight - 2);

                if (i == _activeIndex)
                {
                    using (var brush = new SolidBrush(Color.White))
                        g.FillRectangle(brush, tabRect);
                    using (var pen = new Pen(Color.FromArgb(180, 180, 180)))
                    {
                        g.DrawLine(pen, tabRect.Left, tabRect.Top, tabRect.Right, tabRect.Top);
                        g.DrawLine(pen, tabRect.Left, tabRect.Top, tabRect.Left, tabRect.Bottom);
                        g.DrawLine(pen, tabRect.Right, tabRect.Top, tabRect.Right, tabRect.Bottom);
                    }
                }
                else
                {
                    using (var pen = new Pen(Color.FromArgb(200, 200, 200)))
                        g.DrawLine(pen, tabRect.Right, tabRect.Top + 4, tabRect.Right, tabRect.Bottom - 2);
                }

                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using (var brush = new SolidBrush(i == _activeIndex ? Color.Black : Color.Gray))
                    g.DrawString(name, font, brush, tabRect, sf);
                sf.Dispose();

                x += tabWidth + 2;
            }

            font.Dispose();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button != MouseButtons.Left) return;

            using (var g = CreateGraphics())
            {
                var font = new Font("맑은 고딕", 8.5f);
                int x = 4;

                for (int i = 0; i < _sheetNames.Count; i++)
                {
                    int textWidth = (int)g.MeasureString(_sheetNames[i], font).Width;
                    int tabWidth = Math.Max(textWidth + TabPadding, MinTabWidth);

                    if (e.X >= x && e.X < x + tabWidth + 2)
                    {
                        ActiveIndex = i;
                        break;
                    }
                    x += tabWidth + 2;
                }
                font.Dispose();
            }
        }
    }
}

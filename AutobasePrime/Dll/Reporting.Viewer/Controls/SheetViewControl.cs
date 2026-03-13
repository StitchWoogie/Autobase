using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Reporting.Engine.Model;

namespace Reporting.Viewer.Controls
{
    /// <summary>
    /// 읽기 전용 스프레드시트 뷰어 컨트롤
    /// 리포트 실행 결과를 표시한다. 편집 불가, 스크롤/선택만 가능.
    /// </summary>
    public class SheetViewControl : UserControl
    {
        private SheetModel _sheet;
        private int _scrollX;
        private int _scrollY;
        private int _selectedRow = -1;
        private int _selectedCol = -1;

        // 렌더링 상수
        private const int HeaderHeight = 22;
        private const int HeaderWidth = 50;
        private const double PixelsPerCharWidth = 7.5;
        private const double PixelsPerPoint = 1.333;

        // 그리드 색상
        private static readonly Color GridColor = Color.FromArgb(208, 215, 229);
        private static readonly Color HeaderBackColor = Color.FromArgb(242, 242, 242);
        private static readonly Color HeaderTextColor = Color.FromArgb(51, 51, 51);
        private static readonly Color SelectionColor = Color.FromArgb(40, 0, 120, 215);

        // 폰트 캐시
        private readonly Dictionary<string, Font> _fontCache = new Dictionary<string, Font>();

        public SheetViewControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            BackColor = Color.White;
        }

        /// <summary>표시할 시트 설정</summary>
        public SheetModel Sheet
        {
            get => _sheet;
            set
            {
                _sheet = value;
                _scrollX = 0;
                _scrollY = 0;
                _selectedRow = -1;
                _selectedCol = -1;
                Invalidate();
            }
        }

        /// <summary>현재 선택된 셀 주소</summary>
        public string SelectedCellAddress
        {
            get
            {
                if (_selectedRow < 1 || _selectedCol < 1) return "";
                return CellAddress.ColumnToLetter(_selectedCol) + _selectedRow;
            }
        }

        /// <summary>현재 선택된 셀 텍스트</summary>
        public string SelectedCellText
        {
            get
            {
                if (_sheet == null || _selectedRow < 1 || _selectedCol < 1) return "";
                var cell = _sheet.GetCell(_selectedRow, _selectedCol);
                return cell?.GetDisplayText() ?? "";
            }
        }

        /// <summary>선택 변경 이벤트</summary>
        public event EventHandler SelectionChanged;

        #region 렌더링

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_sheet == null) return;

            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int maxRow = Math.Max(_sheet.UsedRowCount, 30);
            int maxCol = Math.Max(_sheet.UsedColumnCount, 20);

            // 열 좌표 계산
            var colPositions = CalcColumnPositions(maxCol);
            var rowPositions = CalcRowPositions(maxRow);

            // 헤더 배경
            using (var headerBrush = new SolidBrush(HeaderBackColor))
            {
                g.FillRectangle(headerBrush, 0, 0, Width, HeaderHeight);
                g.FillRectangle(headerBrush, 0, 0, HeaderWidth, Height);
            }

            // 셀 렌더링
            RenderCells(g, colPositions, rowPositions, maxRow, maxCol);

            // 격자선
            RenderGridLines(g, colPositions, rowPositions, maxRow, maxCol);

            // 헤더
            RenderColumnHeaders(g, colPositions, maxCol);
            RenderRowHeaders(g, rowPositions, maxRow);

            // 선택 하이라이트
            if (_selectedRow >= 1 && _selectedCol >= 1)
            {
                RenderSelection(g, colPositions, rowPositions);
            }

            // 좌상단 코너
            using (var pen = new Pen(GridColor))
            {
                g.DrawRectangle(pen, 0, 0, HeaderWidth - 1, HeaderHeight - 1);
            }
        }

        private double[] CalcColumnPositions(int maxCol)
        {
            var positions = new double[maxCol + 2];
            positions[0] = HeaderWidth - _scrollX;
            for (int c = 1; c <= maxCol + 1; c++)
            {
                double w = _sheet.GetColumnWidth(c) * PixelsPerCharWidth;
                if (w < 10) w = 10;
                positions[c] = positions[c - 1] + w;
            }
            return positions;
        }

        private double[] CalcRowPositions(int maxRow)
        {
            var positions = new double[maxRow + 2];
            positions[0] = HeaderHeight - _scrollY;
            for (int r = 1; r <= maxRow + 1; r++)
            {
                double h = _sheet.GetRowHeight(r) * PixelsPerPoint;
                if (h < 5) h = 5;
                positions[r] = positions[r - 1] + h;
            }
            return positions;
        }

        private void RenderCells(Graphics g, double[] colPos, double[] rowPos, int maxRow, int maxCol)
        {
            for (int row = 1; row <= maxRow; row++)
            {
                int y = (int)rowPos[row - 1];
                int h = (int)(rowPos[row] - rowPos[row - 1]);
                if (y + h < HeaderHeight) continue;
                if (y > Height) break;

                for (int col = 1; col <= maxCol; col++)
                {
                    int x = (int)colPos[col - 1];
                    int w = (int)(colPos[col] - colPos[col - 1]);
                    if (x + w < HeaderWidth) continue;
                    if (x > Width) break;

                    // 병합 셀 체크
                    var merged = _sheet.FindMergedRange(row, col);
                    if (merged != null && !merged.IsOrigin(row, col))
                        continue;

                    var cellRect = new Rectangle(x, y, w, h);
                    if (merged != null)
                    {
                        int mw = (int)(colPos[merged.LastCol] - colPos[merged.FirstCol - 1]);
                        int mh = (int)(rowPos[merged.LastRow] - rowPos[merged.FirstRow - 1]);
                        cellRect = new Rectangle((int)colPos[merged.FirstCol - 1], (int)rowPos[merged.FirstRow - 1], mw, mh);
                    }

                    var cell = _sheet.GetCell(row, col);
                    if (cell == null) continue;

                    // 배경색
                    if (cell.Style?.Fill != null && cell.Style.Fill.HasFill)
                    {
                        using (var brush = new SolidBrush(cell.Style.Fill.BackgroundColor))
                            g.FillRectangle(brush, cellRect);
                    }

                    // 텍스트
                    string text = cell.GetDisplayText();
                    if (!string.IsNullOrEmpty(text))
                    {
                        var font = GetFont(cell.Style?.Font);
                        var color = cell.Style?.Font?.Color ?? Color.Black;
                        var sf = GetStringFormat(cell.Style?.Alignment);

                        var textRect = new Rectangle(cellRect.X + 2, cellRect.Y + 1,
                            cellRect.Width - 4, cellRect.Height - 2);
                        using (var brush = new SolidBrush(color))
                            g.DrawString(text, font, brush, textRect, sf);
                        sf.Dispose();
                    }

                    // 테두리
                    if (cell.Style?.Border != null && cell.Style.Border.HasAnyBorder)
                    {
                        DrawCellBorder(g, cellRect, cell.Style.Border);
                    }
                }
            }
        }

        private void RenderGridLines(Graphics g, double[] colPos, double[] rowPos, int maxRow, int maxCol)
        {
            using (var pen = new Pen(GridColor))
            {
                for (int r = 0; r <= maxRow; r++)
                {
                    int y = (int)rowPos[r];
                    if (y >= HeaderHeight && y <= Height)
                        g.DrawLine(pen, HeaderWidth, y, Width, y);
                }
                for (int c = 0; c <= maxCol; c++)
                {
                    int x = (int)colPos[c];
                    if (x >= HeaderWidth && x <= Width)
                        g.DrawLine(pen, x, HeaderHeight, x, Height);
                }
            }
        }

        private void RenderColumnHeaders(Graphics g, double[] colPos, int maxCol)
        {
            using (var font = new Font("맑은 고딕", 8.5f))
            using (var brush = new SolidBrush(HeaderTextColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                for (int c = 1; c <= maxCol; c++)
                {
                    int x = (int)colPos[c - 1];
                    int w = (int)(colPos[c] - colPos[c - 1]);
                    if (x + w < HeaderWidth || x > Width) continue;
                    var rect = new Rectangle(x, 0, w, HeaderHeight);
                    g.DrawString(CellAddress.ColumnToLetter(c), font, brush, rect, sf);
                }
                sf.Dispose();
            }
        }

        private void RenderRowHeaders(Graphics g, double[] rowPos, int maxRow)
        {
            using (var font = new Font("맑은 고딕", 8.5f))
            using (var brush = new SolidBrush(HeaderTextColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                for (int r = 1; r <= maxRow; r++)
                {
                    int y = (int)rowPos[r - 1];
                    int h = (int)(rowPos[r] - rowPos[r - 1]);
                    if (y + h < HeaderHeight || y > Height) continue;
                    var rect = new Rectangle(0, y, HeaderWidth, h);
                    g.DrawString(r.ToString(), font, brush, rect, sf);
                }
                sf.Dispose();
            }
        }

        private void RenderSelection(Graphics g, double[] colPos, double[] rowPos)
        {
            if (_selectedCol < 1 || _selectedRow < 1) return;
            int maxCol = colPos.Length - 2;
            int maxRow = rowPos.Length - 2;
            if (_selectedCol > maxCol || _selectedRow > maxRow) return;

            int x = (int)colPos[_selectedCol - 1];
            int y = (int)rowPos[_selectedRow - 1];
            int w = (int)(colPos[_selectedCol] - colPos[_selectedCol - 1]);
            int h = (int)(rowPos[_selectedRow] - rowPos[_selectedRow - 1]);

            using (var brush = new SolidBrush(SelectionColor))
                g.FillRectangle(brush, x, y, w, h);

            using (var pen = new Pen(Color.FromArgb(0, 120, 215), 2))
                g.DrawRectangle(pen, x, y, w, h);
        }

        private void DrawCellBorder(Graphics g, Rectangle rect, BorderStyleModel border)
        {
            if (border.Left.Style != BorderLineStyle.None)
                using (var pen = CreateBorderPen(border.Left))
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
            if (border.Top.Style != BorderLineStyle.None)
                using (var pen = CreateBorderPen(border.Top))
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            if (border.Right.Style != BorderLineStyle.None)
                using (var pen = CreateBorderPen(border.Right))
                    g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
            if (border.Bottom.Style != BorderLineStyle.None)
                using (var pen = CreateBorderPen(border.Bottom))
                    g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
        }

        private Pen CreateBorderPen(BorderEdge edge)
        {
            float width = 1;
            switch (edge.Style)
            {
                case BorderLineStyle.Medium: width = 2; break;
                case BorderLineStyle.Thick: width = 3; break;
            }
            var pen = new Pen(edge.Color, width);
            if (edge.Style == BorderLineStyle.Dashed)
                pen.DashStyle = DashStyle.Dash;
            else if (edge.Style == BorderLineStyle.Dotted)
                pen.DashStyle = DashStyle.Dot;
            return pen;
        }

        #endregion

        #region 입력 처리

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (_sheet == null || e.Button != MouseButtons.Left) return;

            int maxRow = Math.Max(_sheet.UsedRowCount, 30);
            int maxCol = Math.Max(_sheet.UsedColumnCount, 20);
            var colPos = CalcColumnPositions(maxCol);
            var rowPos = CalcRowPositions(maxRow);

            int clickedCol = -1, clickedRow = -1;
            for (int c = 1; c <= maxCol; c++)
            {
                if (e.X >= colPos[c - 1] && e.X < colPos[c])
                { clickedCol = c; break; }
            }
            for (int r = 1; r <= maxRow; r++)
            {
                if (e.Y >= rowPos[r - 1] && e.Y < rowPos[r])
                { clickedRow = r; break; }
            }

            if (clickedRow >= 1 && clickedCol >= 1)
            {
                _selectedRow = clickedRow;
                _selectedCol = clickedCol;
                Invalidate();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int delta = e.Delta > 0 ? -40 : 40;
            _scrollY = Math.Max(0, _scrollY + delta);
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_sheet == null) return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (_selectedRow > 1) { _selectedRow--; Invalidate(); SelectionChanged?.Invoke(this, EventArgs.Empty); }
                    e.Handled = true; break;
                case Keys.Down:
                    _selectedRow++; Invalidate(); SelectionChanged?.Invoke(this, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.Left:
                    if (_selectedCol > 1) { _selectedCol--; Invalidate(); SelectionChanged?.Invoke(this, EventArgs.Empty); }
                    e.Handled = true; break;
                case Keys.Right:
                    _selectedCol++; Invalidate(); SelectionChanged?.Invoke(this, EventArgs.Empty);
                    e.Handled = true; break;
            }
        }

        #endregion

        #region 유틸리티

        private Font GetFont(FontStyleModel fm)
        {
            if (fm == null) fm = new FontStyleModel();
            string key = $"{fm.Name}|{fm.Size}|{(fm.Bold ? "B" : "")}{(fm.Italic ? "I" : "")}{(fm.Underline ? "U" : "")}";

            if (!_fontCache.TryGetValue(key, out var font))
            {
                FontStyle fs = FontStyle.Regular;
                if (fm.Bold) fs |= FontStyle.Bold;
                if (fm.Italic) fs |= FontStyle.Italic;
                if (fm.Underline) fs |= FontStyle.Underline;
                font = new Font(fm.Name, (float)fm.Size, fs);
                _fontCache[key] = font;
            }
            return font;
        }

        private StringFormat GetStringFormat(AlignmentStyleModel align)
        {
            var sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Trimming = StringTrimming.EllipsisCharacter;

            if (align == null)
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                return sf;
            }

            switch (align.Horizontal)
            {
                case HorizontalAlign.Left: sf.Alignment = StringAlignment.Near; break;
                case HorizontalAlign.Center: sf.Alignment = StringAlignment.Center; break;
                case HorizontalAlign.Right: sf.Alignment = StringAlignment.Far; break;
                default: sf.Alignment = StringAlignment.Near; break;
            }
            switch (align.Vertical)
            {
                case VerticalAlign.Top: sf.LineAlignment = StringAlignment.Near; break;
                case VerticalAlign.Middle: sf.LineAlignment = StringAlignment.Center; break;
                case VerticalAlign.Bottom: sf.LineAlignment = StringAlignment.Far; break;
                default: sf.LineAlignment = StringAlignment.Center; break;
            }
            return sf;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var f in _fontCache.Values)
                    f.Dispose();
                _fontCache.Clear();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}

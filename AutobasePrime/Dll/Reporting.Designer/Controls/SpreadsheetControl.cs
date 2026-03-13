using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Reporting.Engine.Model;

namespace Reporting.Designer.Controls
{
    /// <summary>
    /// 엑셀식 스프레드시트 편집 컨트롤 (커스텀 렌더링)
    /// </summary>
    public class SpreadsheetControl : UserControl
    {
        #region 상수

        private const int HeaderHeight = 22;
        private const int HeaderWidth = 50;
        private const double PixelsPerCharWidth = 7.5;
        private const double PixelsPerPoint = 1.333;
        private const int MinColPixelWidth = 10;
        private const int MinRowPixelHeight = 5;
        private const int DefaultVisibleRows = 100;
        private const int DefaultVisibleCols = 26;

        #endregion

        #region 필드

        private SheetModel _sheet;
        private int _scrollRow = 1;
        private int _scrollCol = 1;
        private int _currentRow = 1;
        private int _currentCol = 1;
        private int _selStartRow = 1;
        private int _selStartCol = 1;
        private int _selEndRow = 1;
        private int _selEndCol = 1;
        private bool _selecting;
        private bool _editing;

        private VScrollBar _vScroll;
        private HScrollBar _hScroll;
        private TextBox _editBox;

        // 캐시
        private int[] _colPixelPositions;
        private int[] _rowPixelPositions;
        private int _cachedTotalRows;
        private int _cachedTotalCols;

        // 열/행 리사이즈
        private bool _resizingCol;
        private bool _resizingRow;
        private int _resizeIndex;
        private int _resizeStart;

        #endregion

        #region 이벤트

        /// <summary>선택 셀 변경 이벤트</summary>
        public event EventHandler SelectionChanged;

        /// <summary>셀 값 변경 이벤트</summary>
        public event EventHandler<CellValueChangedEventArgs> CellValueChanged;

        /// <summary>시트 데이터 변경(dirty) 이벤트</summary>
        public event EventHandler SheetModified;

        #endregion

        #region 속성

        /// <summary>현재 편집 중인 시트</summary>
        public SheetModel Sheet
        {
            get => _sheet;
            set
            {
                _sheet = value;
                _currentRow = 1;
                _currentCol = 1;
                _scrollRow = 1;
                _scrollCol = 1;
                InvalidateLayout();
                Invalidate();
            }
        }

        /// <summary>현재 선택 셀 행 (1-based)</summary>
        public int CurrentRow => _currentRow;

        /// <summary>현재 선택 셀 열 (1-based)</summary>
        public int CurrentCol => _currentCol;

        /// <summary>현재 셀 주소 문자열</summary>
        public string CurrentCellAddress => CellAddress.ColumnToLetter(_currentCol) + _currentRow;

        /// <summary>현재 셀의 수식 또는 값 텍스트</summary>
        public string CurrentCellText
        {
            get
            {
                if (_sheet == null) return "";
                var cell = _sheet.GetCell(_currentRow, _currentCol);
                if (cell == null) return "";
                return cell.HasFormula ? cell.Formula : (cell.Value?.ToString() ?? "");
            }
        }

        #endregion

        #region 생성자

        public SpreadsheetControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.Selectable, true);

            BackColor = Color.White;
            _sheet = new SheetModel("Sheet1");

            InitializeScrollBars();
            InitializeEditBox();
        }

        private void InitializeScrollBars()
        {
            _vScroll = new VScrollBar();
            _vScroll.Dock = DockStyle.Right;
            _vScroll.Minimum = 1;
            _vScroll.Maximum = DefaultVisibleRows + 10;
            _vScroll.SmallChange = 1;
            _vScroll.LargeChange = 10;
            _vScroll.Scroll += (s, e) =>
            {
                _scrollRow = e.NewValue;
                InvalidateLayout();
                Invalidate();
            };
            Controls.Add(_vScroll);

            _hScroll = new HScrollBar();
            _hScroll.Dock = DockStyle.Bottom;
            _hScroll.Minimum = 1;
            _hScroll.Maximum = DefaultVisibleCols + 5;
            _hScroll.SmallChange = 1;
            _hScroll.LargeChange = 3;
            _hScroll.Scroll += (s, e) =>
            {
                _scrollCol = e.NewValue;
                InvalidateLayout();
                Invalidate();
            };
            Controls.Add(_hScroll);
        }

        private void InitializeEditBox()
        {
            _editBox = new TextBox();
            _editBox.Visible = false;
            _editBox.BorderStyle = BorderStyle.FixedSingle;
            _editBox.Font = new Font("맑은 고딕", 9f);
            _editBox.KeyDown += EditBox_KeyDown;
            _editBox.LostFocus += EditBox_LostFocus;
            Controls.Add(_editBox);
        }

        #endregion

        #region 레이아웃 계산

        private int GetTotalRows()
        {
            int used = _sheet?.UsedRowCount ?? 0;
            return Math.Max(used + 50, DefaultVisibleRows);
        }

        private int GetTotalCols()
        {
            int used = _sheet?.UsedColumnCount ?? 0;
            return Math.Max(used + 10, DefaultVisibleCols);
        }

        private int GetColumnPixelWidth(int col)
        {
            if (_sheet == null) return (int)(8.43 * PixelsPerCharWidth);
            double charWidth = _sheet.GetColumnWidth(col);
            return Math.Max((int)(charWidth * PixelsPerCharWidth), MinColPixelWidth);
        }

        private int GetRowPixelHeight(int row)
        {
            if (_sheet == null) return (int)(15.0 * PixelsPerPoint);
            double ptHeight = _sheet.GetRowHeight(row);
            return Math.Max((int)(ptHeight * PixelsPerPoint), MinRowPixelHeight);
        }

        private void InvalidateLayout()
        {
            _colPixelPositions = null;
            _rowPixelPositions = null;
        }

        private void EnsureLayout()
        {
            if (_colPixelPositions != null && _rowPixelPositions != null) return;

            _cachedTotalCols = GetTotalCols();
            _cachedTotalRows = GetTotalRows();

            _colPixelPositions = new int[_cachedTotalCols + 2];
            _colPixelPositions[0] = HeaderWidth;
            for (int c = 1; c <= _cachedTotalCols; c++)
            {
                _colPixelPositions[c] = _colPixelPositions[c - 1] + GetColumnPixelWidth(c);
            }

            _rowPixelPositions = new int[_cachedTotalRows + 2];
            _rowPixelPositions[0] = HeaderHeight;
            for (int r = 1; r <= _cachedTotalRows; r++)
            {
                _rowPixelPositions[r] = _rowPixelPositions[r - 1] + GetRowPixelHeight(r);
            }

            // 스크롤바 범위 업데이트
            _vScroll.Maximum = Math.Max(_cachedTotalRows, 1);
            _hScroll.Maximum = Math.Max(_cachedTotalCols, 1);
        }

        /// <summary>행/열 인덱스로부터 화면상 사각형 계산 (스크롤 반영)</summary>
        private Rectangle GetCellRect(int row, int col)
        {
            EnsureLayout();
            if (row < 1 || col < 1) return Rectangle.Empty;

            int x = HeaderWidth;
            for (int c = _scrollCol; c < col && c <= _cachedTotalCols; c++)
                x += GetColumnPixelWidth(c);
            if (col < _scrollCol) return Rectangle.Empty;

            int y = HeaderHeight;
            for (int r = _scrollRow; r < row && r <= _cachedTotalRows; r++)
                y += GetRowPixelHeight(r);
            if (row < _scrollRow) return Rectangle.Empty;

            int w = GetColumnPixelWidth(col);
            int h = GetRowPixelHeight(row);

            return new Rectangle(x, y, w, h);
        }

        /// <summary>화면 좌표 → 셀 행/열 (1-based). 헤더이면 0</summary>
        private void HitTest(int px, int py, out int row, out int col)
        {
            row = 0;
            col = 0;

            if (px < HeaderWidth && py < HeaderHeight) return;

            // 열 찾기
            if (px >= HeaderWidth)
            {
                int x = HeaderWidth;
                for (int c = _scrollCol; c <= _cachedTotalCols; c++)
                {
                    int w = GetColumnPixelWidth(c);
                    if (px < x + w)
                    {
                        col = c;
                        break;
                    }
                    x += w;
                }
            }

            // 행 찾기
            if (py >= HeaderHeight)
            {
                int y = HeaderHeight;
                for (int r = _scrollRow; r <= _cachedTotalRows; r++)
                {
                    int h = GetRowPixelHeight(r);
                    if (py < y + h)
                    {
                        row = r;
                        break;
                    }
                    y += h;
                }
            }
        }

        #endregion

        #region 렌더링

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_sheet == null) return;

            EnsureLayout();

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int clientW = ClientSize.Width - _vScroll.Width;
            int clientH = ClientSize.Height - _hScroll.Height;

            // 클리핑
            g.SetClip(new Rectangle(0, 0, clientW, clientH));

            DrawCells(g, clientW, clientH);
            DrawGridLines(g, clientW, clientH);
            DrawSelection(g);
            DrawHeaders(g, clientW, clientH);
        }

        private void DrawCells(Graphics g, int clientW, int clientH)
        {
            int y = HeaderHeight;
            for (int r = _scrollRow; r <= _cachedTotalRows && y < clientH; r++)
            {
                int rowH = GetRowPixelHeight(r);
                int x = HeaderWidth;

                for (int c = _scrollCol; c <= _cachedTotalCols && x < clientW; c++)
                {
                    int colW = GetColumnPixelWidth(c);

                    // 병합 셀 처리
                    var merged = _sheet.FindMergedRange(r, c);
                    if (merged != null && !merged.IsOrigin(r, c))
                    {
                        x += colW;
                        continue;
                    }

                    Rectangle cellRect;
                    if (merged != null)
                    {
                        cellRect = GetMergedCellRect(merged);
                    }
                    else
                    {
                        cellRect = new Rectangle(x, y, colW, rowH);
                    }

                    var cell = _sheet.GetCell(r, c);
                    if (cell != null)
                    {
                        // 배경
                        if (cell.Style?.Fill != null && cell.Style.Fill.HasFill)
                        {
                            using (var brush = new SolidBrush(cell.Style.Fill.BackgroundColor))
                                g.FillRectangle(brush, cellRect);
                        }

                        // 텍스트
                        string text = cell.GetDisplayText();
                        if (!string.IsNullOrEmpty(text))
                        {
                            DrawCellText(g, text, cellRect, cell.Style);
                        }

                        // 보더
                        if (cell.Style?.Border != null && cell.Style.Border.HasAnyBorder)
                        {
                            DrawCellBorders(g, cellRect, cell.Style.Border);
                        }
                    }

                    x += colW;
                }
                y += rowH;
            }
        }

        private Rectangle GetMergedCellRect(MergedRange merged)
        {
            var topLeft = GetCellRect(merged.FirstRow, merged.FirstCol);
            int w = 0;
            for (int c = merged.FirstCol; c <= merged.LastCol; c++)
                w += GetColumnPixelWidth(c);
            int h = 0;
            for (int r = merged.FirstRow; r <= merged.LastRow; r++)
                h += GetRowPixelHeight(r);
            return new Rectangle(topLeft.X, topLeft.Y, w, h);
        }

        private void DrawCellText(Graphics g, string text, Rectangle rect, StyleModel style)
        {
            var font = GetFont(style?.Font);
            Color color = style?.Font?.Color ?? Color.Black;

            var sf = new StringFormat();
            sf.Trimming = StringTrimming.EllipsisCharacter;

            if (style?.Alignment != null)
            {
                switch (style.Alignment.Horizontal)
                {
                    case HorizontalAlign.Left: sf.Alignment = StringAlignment.Near; break;
                    case HorizontalAlign.Center: sf.Alignment = StringAlignment.Center; break;
                    case HorizontalAlign.Right: sf.Alignment = StringAlignment.Far; break;
                    default: sf.Alignment = StringAlignment.Near; break;
                }
                switch (style.Alignment.Vertical)
                {
                    case VerticalAlign.Top: sf.LineAlignment = StringAlignment.Near; break;
                    case VerticalAlign.Middle: sf.LineAlignment = StringAlignment.Center; break;
                    case VerticalAlign.Bottom: sf.LineAlignment = StringAlignment.Far; break;
                }
                if (!style.Alignment.WrapText)
                    sf.FormatFlags = StringFormatFlags.NoWrap;
            }
            else
            {
                sf.FormatFlags = StringFormatFlags.NoWrap;
            }

            var textRect = new RectangleF(rect.X + 2, rect.Y + 1, rect.Width - 4, rect.Height - 2);
            using (var brush = new SolidBrush(color))
            {
                g.DrawString(text, font, brush, textRect, sf);
            }
            sf.Dispose();
        }

        private void DrawCellBorders(Graphics g, Rectangle rect, BorderStyleModel border)
        {
            if (border.Left.Style != BorderLineStyle.None)
            {
                using (var pen = CreateBorderPen(border.Left))
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
            }
            if (border.Top.Style != BorderLineStyle.None)
            {
                using (var pen = CreateBorderPen(border.Top))
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }
            if (border.Right.Style != BorderLineStyle.None)
            {
                using (var pen = CreateBorderPen(border.Right))
                    g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
            }
            if (border.Bottom.Style != BorderLineStyle.None)
            {
                using (var pen = CreateBorderPen(border.Bottom))
                    g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            }
        }

        private Pen CreateBorderPen(BorderEdge edge)
        {
            float width = 1f;
            switch (edge.Style)
            {
                case BorderLineStyle.Medium: width = 2f; break;
                case BorderLineStyle.Thick: width = 3f; break;
            }
            var pen = new Pen(edge.Color, width);
            if (edge.Style == BorderLineStyle.Dashed)
                pen.DashStyle = DashStyle.Dash;
            else if (edge.Style == BorderLineStyle.Dotted)
                pen.DashStyle = DashStyle.Dot;
            return pen;
        }

        private void DrawGridLines(Graphics g, int clientW, int clientH)
        {
            using (var pen = new Pen(Color.FromArgb(208, 215, 229)))
            {
                // 수평선
                int y = HeaderHeight;
                for (int r = _scrollRow; r <= _cachedTotalRows && y < clientH; r++)
                {
                    y += GetRowPixelHeight(r);
                    g.DrawLine(pen, HeaderWidth, y, clientW, y);
                }

                // 수직선
                int x = HeaderWidth;
                for (int c = _scrollCol; c <= _cachedTotalCols && x < clientW; c++)
                {
                    x += GetColumnPixelWidth(c);
                    g.DrawLine(pen, x, HeaderHeight, x, clientH);
                }
            }
        }

        private void DrawSelection(Graphics g)
        {
            int minRow = Math.Min(_selStartRow, _selEndRow);
            int maxRow = Math.Max(_selStartRow, _selEndRow);
            int minCol = Math.Min(_selStartCol, _selEndCol);
            int maxCol = Math.Max(_selStartCol, _selEndCol);

            // 선택 범위 하이라이트
            if (minRow != maxRow || minCol != maxCol)
            {
                for (int r = minRow; r <= maxRow; r++)
                {
                    for (int c = minCol; c <= maxCol; c++)
                    {
                        if (r == _currentRow && c == _currentCol) continue;
                        var rect = GetCellRect(r, c);
                        if (rect != Rectangle.Empty)
                        {
                            using (var brush = new SolidBrush(Color.FromArgb(48, 49, 106, 197)))
                                g.FillRectangle(brush, rect);
                        }
                    }
                }
            }

            // 현재 셀 강조
            var currentRect = GetCellRect(_currentRow, _currentCol);
            if (currentRect != Rectangle.Empty)
            {
                using (var pen = new Pen(Color.FromArgb(49, 106, 197), 2f))
                    g.DrawRectangle(pen, currentRect);
            }
        }

        private void DrawHeaders(Graphics g, int clientW, int clientH)
        {
            var headerBg = Color.FromArgb(242, 242, 242);
            var headerBorder = Color.FromArgb(208, 215, 229);
            var headerFont = new Font("맑은 고딕", 8.5f);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // 좌상단 코너
            using (var brush = new SolidBrush(headerBg))
                g.FillRectangle(brush, 0, 0, HeaderWidth, HeaderHeight);
            using (var pen = new Pen(headerBorder))
                g.DrawRectangle(pen, 0, 0, HeaderWidth, HeaderHeight);

            // 열 헤더 (A, B, C...)
            int x = HeaderWidth;
            for (int c = _scrollCol; c <= _cachedTotalCols && x < clientW; c++)
            {
                int w = GetColumnPixelWidth(c);
                var rect = new Rectangle(x, 0, w, HeaderHeight);
                bool isSelected = c >= Math.Min(_selStartCol, _selEndCol) && c <= Math.Max(_selStartCol, _selEndCol);

                using (var brush = new SolidBrush(isSelected ? Color.FromArgb(200, 213, 236) : headerBg))
                    g.FillRectangle(brush, rect);
                using (var pen = new Pen(headerBorder))
                    g.DrawRectangle(pen, rect);

                string colLetter = CellAddress.ColumnToLetter(c);
                using (var brush = new SolidBrush(Color.Black))
                    g.DrawString(colLetter, headerFont, brush, rect, sf);

                x += w;
            }

            // 행 헤더 (1, 2, 3...)
            int y = HeaderHeight;
            for (int r = _scrollRow; r <= _cachedTotalRows && y < clientH; r++)
            {
                int h = GetRowPixelHeight(r);
                var rect = new Rectangle(0, y, HeaderWidth, h);
                bool isSelected = r >= Math.Min(_selStartRow, _selEndRow) && r <= Math.Max(_selStartRow, _selEndRow);

                using (var brush = new SolidBrush(isSelected ? Color.FromArgb(200, 213, 236) : headerBg))
                    g.FillRectangle(brush, rect);
                using (var pen = new Pen(headerBorder))
                    g.DrawRectangle(pen, rect);

                using (var brush = new SolidBrush(Color.Black))
                    g.DrawString(r.ToString(), headerFont, brush, rect, sf);

                y += h;
            }

            headerFont.Dispose();
            sf.Dispose();
        }

        private static readonly Dictionary<string, Font> _fontCache = new Dictionary<string, Font>();

        private Font GetFont(FontStyleModel fontStyle)
        {
            if (fontStyle == null)
                return Font;

            string key = $"{fontStyle.Name}|{fontStyle.Size}|{fontStyle.Bold}|{fontStyle.Italic}|{fontStyle.Underline}";
            if (_fontCache.TryGetValue(key, out var cached))
                return cached;

            FontStyle fs = FontStyle.Regular;
            if (fontStyle.Bold) fs |= FontStyle.Bold;
            if (fontStyle.Italic) fs |= FontStyle.Italic;
            if (fontStyle.Underline) fs |= FontStyle.Underline;

            try
            {
                var font = new Font(fontStyle.Name, (float)fontStyle.Size, fs);
                _fontCache[key] = font;
                return font;
            }
            catch
            {
                return Font;
            }
        }

        #endregion

        #region 마우스 처리

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            EnsureLayout();

            if (e.Button != MouseButtons.Left) return;

            // 열 리사이즈 체크
            if (e.Y < HeaderHeight && e.X > HeaderWidth)
            {
                int x = HeaderWidth;
                for (int c = _scrollCol; c <= _cachedTotalCols; c++)
                {
                    x += GetColumnPixelWidth(c);
                    if (Math.Abs(e.X - x) <= 3)
                    {
                        _resizingCol = true;
                        _resizeIndex = c;
                        _resizeStart = e.X;
                        Cursor = Cursors.VSplit;
                        return;
                    }
                }
            }

            // 행 리사이즈 체크
            if (e.X < HeaderWidth && e.Y > HeaderHeight)
            {
                int y = HeaderHeight;
                for (int r = _scrollRow; r <= _cachedTotalRows; r++)
                {
                    y += GetRowPixelHeight(r);
                    if (Math.Abs(e.Y - y) <= 3)
                    {
                        _resizingRow = true;
                        _resizeIndex = r;
                        _resizeStart = e.Y;
                        Cursor = Cursors.HSplit;
                        return;
                    }
                }
            }

            // 셀 클릭
            CommitEdit();
            HitTest(e.X, e.Y, out int row, out int col);
            if (row > 0 && col > 0)
            {
                _currentRow = row;
                _currentCol = col;
                _selStartRow = row;
                _selStartCol = col;
                _selEndRow = row;
                _selEndCol = col;
                _selecting = true;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            EnsureLayout();

            // 열 리사이즈 드래그
            if (_resizingCol)
            {
                int delta = e.X - _resizeStart;
                int newW = GetColumnPixelWidth(_resizeIndex) + delta;
                if (newW >= MinColPixelWidth)
                {
                    _sheet.ColumnWidths[_resizeIndex] = newW / PixelsPerCharWidth;
                    _resizeStart = e.X;
                    InvalidateLayout();
                    Invalidate();
                    SheetModified?.Invoke(this, EventArgs.Empty);
                }
                return;
            }

            // 행 리사이즈 드래그
            if (_resizingRow)
            {
                int delta = e.Y - _resizeStart;
                int newH = GetRowPixelHeight(_resizeIndex) + delta;
                if (newH >= MinRowPixelHeight)
                {
                    _sheet.RowHeights[_resizeIndex] = newH / PixelsPerPoint;
                    _resizeStart = e.Y;
                    InvalidateLayout();
                    Invalidate();
                    SheetModified?.Invoke(this, EventArgs.Empty);
                }
                return;
            }

            // 커서 변경
            if (e.Y < HeaderHeight && e.X > HeaderWidth)
            {
                int x = HeaderWidth;
                for (int c = _scrollCol; c <= _cachedTotalCols; c++)
                {
                    x += GetColumnPixelWidth(c);
                    if (Math.Abs(e.X - x) <= 3)
                    {
                        Cursor = Cursors.VSplit;
                        return;
                    }
                }
            }
            if (e.X < HeaderWidth && e.Y > HeaderHeight)
            {
                int y = HeaderHeight;
                for (int r = _scrollRow; r <= _cachedTotalRows; r++)
                {
                    y += GetRowPixelHeight(r);
                    if (Math.Abs(e.Y - y) <= 3)
                    {
                        Cursor = Cursors.HSplit;
                        return;
                    }
                }
            }
            Cursor = Cursors.Default;

            // 범위 선택
            if (_selecting)
            {
                HitTest(e.X, e.Y, out int row, out int col);
                if (row > 0 && col > 0)
                {
                    _selEndRow = row;
                    _selEndCol = col;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _selecting = false;
            _resizingCol = false;
            _resizingRow = false;
            Cursor = Cursors.Default;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left)
            {
                HitTest(e.X, e.Y, out int row, out int col);
                if (row > 0 && col > 0)
                {
                    BeginEdit();
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int delta = e.Delta > 0 ? -3 : 3;
            _scrollRow = Math.Max(1, Math.Min(_scrollRow + delta, _cachedTotalRows - 5));
            _vScroll.Value = Math.Min(_scrollRow, _vScroll.Maximum);
            InvalidateLayout();
            Invalidate();
        }

        #endregion

        #region 키보드 처리

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Tab:
                case Keys.Enter:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_editing) return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveCursor(Math.Max(1, _currentRow - 1), _currentCol);
                    e.Handled = true;
                    break;
                case Keys.Down:
                case Keys.Enter:
                    MoveCursor(_currentRow + 1, _currentCol);
                    e.Handled = true;
                    break;
                case Keys.Left:
                    MoveCursor(_currentRow, Math.Max(1, _currentCol - 1));
                    e.Handled = true;
                    break;
                case Keys.Right:
                case Keys.Tab:
                    MoveCursor(_currentRow, _currentCol + 1);
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    DeleteCurrentCell();
                    e.Handled = true;
                    break;
                case Keys.F2:
                    BeginEdit();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    CancelEdit();
                    e.Handled = true;
                    break;
                default:
                    // 일반 문자 입력 시 편집 시작
                    if (!e.Control && !e.Alt && e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.Z ||
                        e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 ||
                        e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.OemPeriod ||
                        e.KeyCode == Keys.Oemplus)
                    {
                        BeginEdit(clear: true);
                    }
                    break;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!_editing && !char.IsControl(e.KeyChar))
            {
                BeginEdit(clear: true);
                _editBox.Text = e.KeyChar.ToString();
                _editBox.SelectionStart = 1;
                e.Handled = true;
            }
        }

        #endregion

        #region 셀 편집

        /// <summary>인라인 편집 시작</summary>
        public void BeginEdit(bool clear = false)
        {
            if (_sheet == null) return;

            var rect = GetCellRect(_currentRow, _currentCol);
            if (rect == Rectangle.Empty) return;

            _editing = true;
            _editBox.Location = new Point(rect.X + 1, rect.Y + 1);
            _editBox.Size = new Size(Math.Max(rect.Width - 2, 50), rect.Height - 2);

            if (clear)
            {
                _editBox.Text = "";
            }
            else
            {
                _editBox.Text = CurrentCellText;
            }

            _editBox.Visible = true;
            _editBox.Focus();
            _editBox.SelectAll();
        }

        /// <summary>편집 확정</summary>
        public void CommitEdit()
        {
            if (!_editing) return;
            _editing = false;

            string text = _editBox.Text;
            _editBox.Visible = false;

            if (_sheet == null) return;

            var cell = _sheet.GetOrCreateCell(_currentRow, _currentCol);
            string oldValue = cell.HasFormula ? cell.Formula : (cell.Value?.ToString() ?? "");

            if (text == oldValue) return;

            if (text.StartsWith("="))
            {
                cell.Formula = text;
                cell.DataType = CellDataType.Formula;
                cell.Value = text;
            }
            else if (double.TryParse(text, out double d))
            {
                cell.Value = d;
                cell.Formula = null;
                cell.DataType = CellDataType.Number;
            }
            else
            {
                cell.Value = text;
                cell.Formula = null;
                cell.DataType = CellDataType.String;
            }

            CellValueChanged?.Invoke(this, new CellValueChangedEventArgs(_currentRow, _currentCol, oldValue, text));
            SheetModified?.Invoke(this, EventArgs.Empty);
            SelectionChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>편집 취소</summary>
        public void CancelEdit()
        {
            if (!_editing) return;
            _editing = false;
            _editBox.Visible = false;
            Focus();
        }

        private void DeleteCurrentCell()
        {
            if (_sheet == null) return;

            int minRow = Math.Min(_selStartRow, _selEndRow);
            int maxRow = Math.Max(_selStartRow, _selEndRow);
            int minCol = Math.Min(_selStartCol, _selEndCol);
            int maxCol = Math.Max(_selStartCol, _selEndCol);

            for (int r = minRow; r <= maxRow; r++)
            {
                for (int c = minCol; c <= maxCol; c++)
                {
                    var cell = _sheet.GetCell(r, c);
                    if (cell != null)
                    {
                        cell.Value = null;
                        cell.Formula = null;
                    }
                }
            }
            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        private void EditBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitEdit();
                MoveCursor(_currentRow + 1, _currentCol);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Tab)
            {
                CommitEdit();
                MoveCursor(_currentRow, _currentCol + 1);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CancelEdit();
                e.Handled = true;
            }
        }

        private void EditBox_LostFocus(object sender, EventArgs e)
        {
            CommitEdit();
        }

        #endregion

        #region 네비게이션

        private void MoveCursor(int row, int col)
        {
            if (row < 1) row = 1;
            if (col < 1) col = 1;

            _currentRow = row;
            _currentCol = col;
            _selStartRow = row;
            _selStartCol = col;
            _selEndRow = row;
            _selEndCol = col;

            EnsureVisible(row, col);
            SelectionChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>셀이 보이도록 스크롤 조정</summary>
        public void EnsureVisible(int row, int col)
        {
            if (row < _scrollRow)
                _scrollRow = row;
            if (col < _scrollCol)
                _scrollCol = col;

            // 오른쪽/아래로 벗어나면 스크롤
            int clientW = ClientSize.Width - _vScroll.Width;
            int clientH = ClientSize.Height - _hScroll.Height;

            int x = HeaderWidth;
            for (int c = _scrollCol; c <= col; c++)
                x += GetColumnPixelWidth(c);
            while (x > clientW && _scrollCol < col)
            {
                x -= GetColumnPixelWidth(_scrollCol);
                _scrollCol++;
            }

            int y = HeaderHeight;
            for (int r = _scrollRow; r <= row; r++)
                y += GetRowPixelHeight(r);
            while (y > clientH && _scrollRow < row)
            {
                y -= GetRowPixelHeight(_scrollRow);
                _scrollRow++;
            }

            _vScroll.Value = Math.Min(_scrollRow, _vScroll.Maximum);
            _hScroll.Value = Math.Min(_scrollCol, _hScroll.Maximum);
            InvalidateLayout();
        }

        /// <summary>지정 셀로 이동 (외부에서 호출)</summary>
        public void GoToCell(int row, int col)
        {
            CommitEdit();
            MoveCursor(row, col);
        }

        /// <summary>셀 주소 문자열로 이동</summary>
        public void GoToCell(string address)
        {
            if (CellAddress.TryParse(address, out var addr))
                GoToCell(addr.Row, addr.Col);
        }

        #endregion

        #region 외부 API

        /// <summary>수식바에서 값 설정 (외부 호출)</summary>
        public void SetCurrentCellText(string text)
        {
            if (_sheet == null) return;

            var cell = _sheet.GetOrCreateCell(_currentRow, _currentCol);
            if (text.StartsWith("="))
            {
                cell.Formula = text;
                cell.DataType = CellDataType.Formula;
                cell.Value = text;
            }
            else if (double.TryParse(text, out double d))
            {
                cell.Value = d;
                cell.Formula = null;
                cell.DataType = CellDataType.Number;
            }
            else
            {
                cell.Value = text;
                cell.Formula = null;
                cell.DataType = CellDataType.String;
            }

            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>선택 범위 반환 (minRow, minCol, maxRow, maxCol, 모두 1-based)</summary>
        public void GetSelectionRange(out int minRow, out int minCol, out int maxRow, out int maxCol)
        {
            minRow = Math.Min(_selStartRow, _selEndRow);
            minCol = Math.Min(_selStartCol, _selEndCol);
            maxRow = Math.Max(_selStartRow, _selEndRow);
            maxCol = Math.Max(_selStartCol, _selEndCol);
        }

        #endregion

        #region 클립보드 (Copy/Cut/Paste)

        /// <summary>선택 범위 복사</summary>
        public void CopySelection()
        {
            if (_sheet == null) return;
            GetSelectionRange(out int r1, out int c1, out int r2, out int c2);

            var sb = new System.Text.StringBuilder();
            for (int r = r1; r <= r2; r++)
            {
                for (int c = c1; c <= c2; c++)
                {
                    if (c > c1) sb.Append('\t');
                    var cell = _sheet.GetCell(r, c);
                    sb.Append(cell?.GetDisplayText() ?? "");
                }
                sb.AppendLine();
            }
            if (sb.Length > 0)
                Clipboard.SetText(sb.ToString());
        }

        /// <summary>선택 범위 잘라내기 (복사 후 삭제)</summary>
        public void CutSelection()
        {
            CopySelection();
            DeleteSelection();
        }

        /// <summary>클립보드에서 붙여넣기</summary>
        public void PasteFromClipboard()
        {
            if (_sheet == null || !Clipboard.ContainsText()) return;

            string text = Clipboard.GetText();
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            int startRow = _currentRow;
            int startCol = _currentCol;

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]) && i == lines.Length - 1) break;
                string[] cells = lines[i].Split('\t');
                for (int j = 0; j < cells.Length; j++)
                {
                    int r = startRow + i;
                    int c = startCol + j;
                    var cell = _sheet.GetOrCreateCell(r, c);
                    string val = cells[j];
                    if (double.TryParse(val, out double d))
                    {
                        cell.Value = d;
                        cell.DataType = CellDataType.Number;
                    }
                    else
                    {
                        cell.Value = val;
                        cell.DataType = CellDataType.String;
                    }
                    cell.Formula = null;
                }
            }
            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>선택 범위 삭제</summary>
        public void DeleteSelection()
        {
            DeleteCurrentCell();
        }

        #endregion

        #region 서식 적용

        /// <summary>선택 범위의 각 셀에 대해 스타일 변경 액션 적용</summary>
        public void ApplyStyleToSelection(Action<StyleModel> applyAction)
        {
            if (_sheet == null) return;
            GetSelectionRange(out int r1, out int c1, out int r2, out int c2);

            for (int r = r1; r <= r2; r++)
            {
                for (int c = c1; c <= c2; c++)
                {
                    var cell = _sheet.GetOrCreateCell(r, c);
                    if (cell.Style == null)
                        cell.Style = new StyleModel();
                    applyAction(cell.Style);
                }
            }
            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>선택 범위에 Bold 토글</summary>
        public void ToggleBold()
        {
            bool newVal = !GetCurrentCellStyle()?.Font?.Bold ?? true;
            ApplyStyleToSelection(s => s.Font.Bold = newVal);
        }

        /// <summary>선택 범위에 Italic 토글</summary>
        public void ToggleItalic()
        {
            bool newVal = !GetCurrentCellStyle()?.Font?.Italic ?? true;
            ApplyStyleToSelection(s => s.Font.Italic = newVal);
        }

        /// <summary>선택 범위에 Underline 토글</summary>
        public void ToggleUnderline()
        {
            bool newVal = !GetCurrentCellStyle()?.Font?.Underline ?? true;
            ApplyStyleToSelection(s => s.Font.Underline = newVal);
        }

        /// <summary>선택 범위 수평 정렬 설정</summary>
        public void SetHorizontalAlign(HorizontalAlign align)
        {
            ApplyStyleToSelection(s => s.Alignment.Horizontal = align);
        }

        /// <summary>선택 범위 글꼴색 설정</summary>
        public void SetFontColor(Color color)
        {
            ApplyStyleToSelection(s => s.Font.Color = color);
        }

        /// <summary>선택 범위 배경색 설정</summary>
        public void SetBackColor(Color color)
        {
            ApplyStyleToSelection(s =>
            {
                s.Fill.BackgroundColor = color;
                s.Fill.PatternType = FillPattern.Solid;
            });
        }

        /// <summary>선택 범위 글꼴 설정</summary>
        public void SetFont(string fontName, double fontSize)
        {
            ApplyStyleToSelection(s =>
            {
                s.Font.Name = fontName;
                s.Font.Size = fontSize;
            });
        }

        /// <summary>선택 범위 전체 테두리 설정</summary>
        public void SetAllBorders(BorderLineStyle style, Color color)
        {
            ApplyStyleToSelection(s =>
            {
                s.Border.Left = new BorderEdge { Style = style, Color = color };
                s.Border.Top = new BorderEdge { Style = style, Color = color };
                s.Border.Right = new BorderEdge { Style = style, Color = color };
                s.Border.Bottom = new BorderEdge { Style = style, Color = color };
            });
        }

        /// <summary>현재 셀의 스타일 반환 (null 가능)</summary>
        public StyleModel GetCurrentCellStyle()
        {
            return _sheet?.GetCell(_currentRow, _currentCol)?.Style;
        }

        #endregion

        #region 행/열 조작

        /// <summary>현재 행 위에 새 행 삽입</summary>
        public void InsertRowAbove()
        {
            if (_sheet == null) return;
            int insertRow = _currentRow;
            int maxRow = _sheet.UsedRowCount;

            // 아래로 밀기 (역순)
            for (int r = maxRow; r >= insertRow; r--)
            {
                for (int c = 1; c <= _sheet.UsedColumnCount; c++)
                {
                    var cell = _sheet.GetCell(r, c);
                    if (cell != null)
                    {
                        _sheet.SetCell(r + 1, c, cell);
                        _sheet.RemoveCell(r, c);
                    }
                }
                if (_sheet.RowHeights.TryGetValue(r, out double h))
                {
                    _sheet.RowHeights[r + 1] = h;
                    _sheet.RowHeights.Remove(r);
                }
            }
            InvalidateLayout();
            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>현재 행 삭제</summary>
        public void DeleteCurrentRow()
        {
            if (_sheet == null) return;
            int delRow = _currentRow;
            int maxRow = _sheet.UsedRowCount;

            for (int c = 1; c <= _sheet.UsedColumnCount; c++)
                _sheet.RemoveCell(delRow, c);

            // 위로 당기기
            for (int r = delRow + 1; r <= maxRow; r++)
            {
                for (int c = 1; c <= _sheet.UsedColumnCount + 10; c++)
                {
                    var cell = _sheet.GetCell(r, c);
                    if (cell != null)
                    {
                        _sheet.SetCell(r - 1, c, cell);
                        _sheet.RemoveCell(r, c);
                    }
                }
            }
            InvalidateLayout();
            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>현재 열 왼쪽에 새 열 삽입</summary>
        public void InsertColumnLeft()
        {
            if (_sheet == null) return;
            int insertCol = _currentCol;
            int maxCol = _sheet.UsedColumnCount;

            for (int c = maxCol; c >= insertCol; c--)
            {
                for (int r = 1; r <= _sheet.UsedRowCount; r++)
                {
                    var cell = _sheet.GetCell(r, c);
                    if (cell != null)
                    {
                        _sheet.SetCell(r, c + 1, cell);
                        _sheet.RemoveCell(r, c);
                    }
                }
                if (_sheet.ColumnWidths.TryGetValue(c, out double w))
                {
                    _sheet.ColumnWidths[c + 1] = w;
                    _sheet.ColumnWidths.Remove(c);
                }
            }
            InvalidateLayout();
            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>현재 열 삭제</summary>
        public void DeleteCurrentColumn()
        {
            if (_sheet == null) return;
            int delCol = _currentCol;
            int maxCol = _sheet.UsedColumnCount;

            for (int r = 1; r <= _sheet.UsedRowCount; r++)
                _sheet.RemoveCell(r, delCol);

            for (int c = delCol + 1; c <= maxCol; c++)
            {
                for (int r = 1; r <= _sheet.UsedRowCount + 10; r++)
                {
                    var cell = _sheet.GetCell(r, c);
                    if (cell != null)
                    {
                        _sheet.SetCell(r, c - 1, cell);
                        _sheet.RemoveCell(r, c);
                    }
                }
            }
            InvalidateLayout();
            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        #endregion

        #region 셀 병합

        /// <summary>선택 범위 셀 병합</summary>
        public void MergeSelection()
        {
            if (_sheet == null) return;
            GetSelectionRange(out int r1, out int c1, out int r2, out int c2);
            if (r1 == r2 && c1 == c2) return;

            // 기존 병합과 겹치는지 확인
            var existing = _sheet.FindMergedRange(r1, c1);
            if (existing != null) return;

            _sheet.MergedRanges.Add(new MergedRange(r1, c1, r2, c2));
            SheetModified?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>현재 셀의 병합 해제</summary>
        public void UnmergeSelection()
        {
            if (_sheet == null) return;
            var merged = _sheet.FindMergedRange(_currentRow, _currentCol);
            if (merged != null)
            {
                _sheet.MergedRanges.Remove(merged);
                SheetModified?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        #endregion
    }

    /// <summary>셀 값 변경 이벤트 인수</summary>
    public class CellValueChangedEventArgs : EventArgs
    {
        public int Row { get; }
        public int Col { get; }
        public string OldValue { get; }
        public string NewValue { get; }

        public CellValueChangedEventArgs(int row, int col, string oldValue, string newValue)
        {
            Row = row;
            Col = col;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using GraphicModule;
using NetTools;

namespace Studio
{
	public class PropertyPageObjectTable : Form
	{
		private Label lblRowCount;
		private NumericUpDown numRowCount;
		private Label lblColCount;
		private NumericUpDown numColCount;
		private Label lblGridColor;
		private Button btnGridColor;
		private Label lblHeaderBackColor;
		private Button btnHeaderBackColor;
		private Label lblHeaderForeColor;
		private Button btnHeaderForeColor;
		private Label lblCellBackColor;
		private Button btnCellBackColor;
		private Label lblCellForeColor;
		private Button btnCellForeColor;
		private Label lblGridLineWidth;
		private NumericUpDown numGridLineWidth;
		private CheckBox chkShowHeader;
		private CheckBox chkAutoSizeColumns;
		private GroupBox grpTableData;
		private DataGridView dgvCellData;
		private Button btnPasteExcel;
		private Button btnPasteHtml;
		private Button btnClearTable;
		private Button btnApplySize;

		private ObjectArgsTable _objectArgs;

		public ObjectArgsTable ObjectArgs
		{
			get
			{
				_objectArgs.nRowCount = (int)numRowCount.Value;
				_objectArgs.nColCount = (int)numColCount.Value;
				_objectArgs.nGridLineWidth = (int)numGridLineWidth.Value;
				_objectArgs.bShowHeader = chkShowHeader.Checked;
				_objectArgs.bAutoSizeColumns = chkAutoSizeColumns.Checked;

				// 셀 데이터 수집
				CollectCellDataFromGrid();

				return _objectArgs;
			}
			set
			{
				_objectArgs = value ?? new ObjectArgsTable();
				numRowCount.Value = Math.Max(1, Math.Min(100, _objectArgs.nRowCount));
				numColCount.Value = Math.Max(1, Math.Min(100, _objectArgs.nColCount));
				numGridLineWidth.Value = Math.Max(1, Math.Min(5, _objectArgs.nGridLineWidth));
				chkShowHeader.Checked = _objectArgs.bShowHeader;
				chkAutoSizeColumns.Checked = _objectArgs.bAutoSizeColumns;
				btnGridColor.BackColor = _objectArgs.lGridColor;
				btnHeaderBackColor.BackColor = _objectArgs.lHeaderBackColor;
				btnHeaderForeColor.BackColor = _objectArgs.lHeaderForeColor;
				btnCellBackColor.BackColor = _objectArgs.lCellBackColor;
				btnCellForeColor.BackColor = _objectArgs.lCellForeColor;
				PopulateCellDataGrid();
			}
		}

		public PropertyPageObjectTable()
		{
			InitializeComponent();
			ApplyLanguage();
		}

		private void InitializeComponent()
		{
			this.lblRowCount = new Label();
			this.numRowCount = new NumericUpDown();
			this.lblColCount = new Label();
			this.numColCount = new NumericUpDown();
			this.lblGridColor = new Label();
			this.btnGridColor = new Button();
			this.lblHeaderBackColor = new Label();
			this.btnHeaderBackColor = new Button();
			this.lblHeaderForeColor = new Label();
			this.btnHeaderForeColor = new Button();
			this.lblCellBackColor = new Label();
			this.btnCellBackColor = new Button();
			this.lblCellForeColor = new Label();
			this.btnCellForeColor = new Button();
			this.lblGridLineWidth = new Label();
			this.numGridLineWidth = new NumericUpDown();
			this.chkShowHeader = new CheckBox();
			this.chkAutoSizeColumns = new CheckBox();
			this.grpTableData = new GroupBox();
			this.dgvCellData = new DataGridView();
			this.btnPasteExcel = new Button();
			this.btnPasteHtml = new Button();
			this.btnClearTable = new Button();
			this.btnApplySize = new Button();
			this.SuspendLayout();

			int y = 12;
			int labelX = 12;
			int controlX = 130;
			int rowGap = 28;

			// Row Count
			this.lblRowCount.Location = new Point(labelX, y + 3);
			this.lblRowCount.Size = new Size(110, 20);
			this.numRowCount.Location = new Point(controlX, y);
			this.numRowCount.Size = new Size(60, 21);
			this.numRowCount.Minimum = 1;
			this.numRowCount.Maximum = 200;
			y += rowGap;

			// Col Count
			this.lblColCount.Location = new Point(labelX, y + 3);
			this.lblColCount.Size = new Size(110, 20);
			this.numColCount.Location = new Point(controlX, y);
			this.numColCount.Size = new Size(60, 21);
			this.numColCount.Minimum = 1;
			this.numColCount.Maximum = 50;
			y += rowGap;

			// Apply Size button
			this.btnApplySize.Location = new Point(controlX + 70, y - rowGap);
			this.btnApplySize.Size = new Size(80, 23);
			this.btnApplySize.Click += new EventHandler(btnApplySize_Click);

			// Grid Line Width
			this.lblGridLineWidth.Location = new Point(labelX, y + 3);
			this.lblGridLineWidth.Size = new Size(110, 20);
			this.numGridLineWidth.Location = new Point(controlX, y);
			this.numGridLineWidth.Size = new Size(60, 21);
			this.numGridLineWidth.Minimum = 1;
			this.numGridLineWidth.Maximum = 5;
			y += rowGap;

			// Grid Color
			this.lblGridColor.Location = new Point(labelX, y + 3);
			this.lblGridColor.Size = new Size(110, 20);
			this.btnGridColor.Location = new Point(controlX, y);
			this.btnGridColor.Size = new Size(60, 23);
			this.btnGridColor.FlatStyle = FlatStyle.Flat;
			this.btnGridColor.Click += new EventHandler(btnColor_Click);
			y += rowGap;

			// Header Back Color
			this.lblHeaderBackColor.Location = new Point(labelX, y + 3);
			this.lblHeaderBackColor.Size = new Size(110, 20);
			this.btnHeaderBackColor.Location = new Point(controlX, y);
			this.btnHeaderBackColor.Size = new Size(60, 23);
			this.btnHeaderBackColor.FlatStyle = FlatStyle.Flat;
			this.btnHeaderBackColor.Click += new EventHandler(btnColor_Click);
			y += rowGap;

			// Header Fore Color
			this.lblHeaderForeColor.Location = new Point(labelX, y + 3);
			this.lblHeaderForeColor.Size = new Size(110, 20);
			this.btnHeaderForeColor.Location = new Point(controlX, y);
			this.btnHeaderForeColor.Size = new Size(60, 23);
			this.btnHeaderForeColor.FlatStyle = FlatStyle.Flat;
			this.btnHeaderForeColor.Click += new EventHandler(btnColor_Click);
			y += rowGap;

			// Cell Back Color
			this.lblCellBackColor.Location = new Point(labelX, y + 3);
			this.lblCellBackColor.Size = new Size(110, 20);
			this.btnCellBackColor.Location = new Point(controlX, y);
			this.btnCellBackColor.Size = new Size(60, 23);
			this.btnCellBackColor.FlatStyle = FlatStyle.Flat;
			this.btnCellBackColor.Click += new EventHandler(btnColor_Click);
			y += rowGap;

			// Cell Fore Color
			this.lblCellForeColor.Location = new Point(labelX, y + 3);
			this.lblCellForeColor.Size = new Size(110, 20);
			this.btnCellForeColor.Location = new Point(controlX, y);
			this.btnCellForeColor.Size = new Size(60, 23);
			this.btnCellForeColor.FlatStyle = FlatStyle.Flat;
			this.btnCellForeColor.Click += new EventHandler(btnColor_Click);
			y += rowGap;

			// Show Header
			this.chkShowHeader.Location = new Point(labelX, y);
			this.chkShowHeader.Size = new Size(200, 20);
			y += 24;

			// Auto Size Columns
			this.chkAutoSizeColumns.Location = new Point(labelX, y);
			this.chkAutoSizeColumns.Size = new Size(200, 20);
			y += 28;

			// Table Data Group
			this.grpTableData.Location = new Point(labelX, y);
			this.grpTableData.Size = new Size(360, 200);

			this.dgvCellData.Location = new Point(6, 18);
			this.dgvCellData.Size = new Size(348, 140);
			this.dgvCellData.AllowUserToAddRows = false;
			this.dgvCellData.AllowUserToDeleteRows = false;
			this.dgvCellData.ScrollBars = ScrollBars.Both;
			this.dgvCellData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

			this.btnPasteExcel.Location = new Point(6, 165);
			this.btnPasteExcel.Size = new Size(110, 25);
			this.btnPasteExcel.Click += new EventHandler(btnPasteExcel_Click);

			this.btnPasteHtml.Location = new Point(122, 165);
			this.btnPasteHtml.Size = new Size(110, 25);
			this.btnPasteHtml.Click += new EventHandler(btnPasteHtml_Click);

			this.btnClearTable.Location = new Point(238, 165);
			this.btnClearTable.Size = new Size(110, 25);
			this.btnClearTable.Click += new EventHandler(btnClearTable_Click);

			this.grpTableData.Controls.Add(this.dgvCellData);
			this.grpTableData.Controls.Add(this.btnPasteExcel);
			this.grpTableData.Controls.Add(this.btnPasteHtml);
			this.grpTableData.Controls.Add(this.btnClearTable);

			this.ClientSize = new Size(390, y + 210);
			this.FormBorderStyle = FormBorderStyle.None;

			this.Controls.Add(this.lblRowCount);
			this.Controls.Add(this.numRowCount);
			this.Controls.Add(this.lblColCount);
			this.Controls.Add(this.numColCount);
			this.Controls.Add(this.btnApplySize);
			this.Controls.Add(this.lblGridLineWidth);
			this.Controls.Add(this.numGridLineWidth);
			this.Controls.Add(this.lblGridColor);
			this.Controls.Add(this.btnGridColor);
			this.Controls.Add(this.lblHeaderBackColor);
			this.Controls.Add(this.btnHeaderBackColor);
			this.Controls.Add(this.lblHeaderForeColor);
			this.Controls.Add(this.btnHeaderForeColor);
			this.Controls.Add(this.lblCellBackColor);
			this.Controls.Add(this.btnCellBackColor);
			this.Controls.Add(this.lblCellForeColor);
			this.Controls.Add(this.btnCellForeColor);
			this.Controls.Add(this.chkShowHeader);
			this.Controls.Add(this.chkAutoSizeColumns);
			this.Controls.Add(this.grpTableData);

			this.ResumeLayout(false);
		}

		private void ApplyLanguage()
		{
			if (Tools.IsLangKorean())
			{
				this.Text = "테이블";
				this.lblRowCount.Text = "행 수";
				this.lblColCount.Text = "열 수";
				this.lblGridLineWidth.Text = "격자선 두께";
				this.lblGridColor.Text = "격자선 색상";
				this.lblHeaderBackColor.Text = "헤더 배경색";
				this.lblHeaderForeColor.Text = "헤더 글자색";
				this.lblCellBackColor.Text = "셀 배경색";
				this.lblCellForeColor.Text = "셀 글자색";
				this.chkShowHeader.Text = "첫 행을 헤더로 표시";
				this.chkAutoSizeColumns.Text = "열 자동 크기";
				this.grpTableData.Text = "테이블 데이터";
				this.btnPasteExcel.Text = "Excel 붙여넣기";
				this.btnPasteHtml.Text = "HTML 붙여넣기";
				this.btnClearTable.Text = "지우기";
				this.btnApplySize.Text = "크기 적용";
			}
			else
			{
				this.Text = "Table";
				this.lblRowCount.Text = "Rows";
				this.lblColCount.Text = "Columns";
				this.lblGridLineWidth.Text = "Grid Width";
				this.lblGridColor.Text = "Grid Color";
				this.lblHeaderBackColor.Text = "Header BG";
				this.lblHeaderForeColor.Text = "Header FG";
				this.lblCellBackColor.Text = "Cell BG";
				this.lblCellForeColor.Text = "Cell FG";
				this.chkShowHeader.Text = "Show first row as header";
				this.chkAutoSizeColumns.Text = "Auto-size columns";
				this.grpTableData.Text = "Table Data";
				this.btnPasteExcel.Text = "Paste Excel";
				this.btnPasteHtml.Text = "Paste HTML";
				this.btnClearTable.Text = "Clear";
				this.btnApplySize.Text = "Apply Size";
			}
		}

		void PopulateCellDataGrid()
		{
			dgvCellData.Columns.Clear();
			dgvCellData.Rows.Clear();

			if (_objectArgs == null || _objectArgs.cells.Count == 0) return;

			for (int c = 0; c < _objectArgs.nColCount; c++)
			{
				dgvCellData.Columns.Add("Col" + c, "C" + (c + 1));
				dgvCellData.Columns[c].Width = 80;
			}

			for (int r = 0; r < _objectArgs.cells.Count && r < _objectArgs.nRowCount; r++)
			{
				int rowIndex = dgvCellData.Rows.Add();
				for (int c = 0; c < _objectArgs.cells[r].Count && c < _objectArgs.nColCount; c++)
				{
					string text = _objectArgs.cells[r][c].sText;
					if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(_objectArgs.cells[r][c].sTagName))
						text = "{" + _objectArgs.cells[r][c].sTagName + "}";
					dgvCellData.Rows[rowIndex].Cells[c].Value = text;
				}
			}
		}

		void CollectCellDataFromGrid()
		{
			if (_objectArgs == null) return;

			int rows = dgvCellData.Rows.Count;
			int cols = dgvCellData.Columns.Count;

			// 기존 셀 보존하면서 텍스트만 업데이트
			for (int r = 0; r < rows && r < _objectArgs.cells.Count; r++)
			{
				for (int c = 0; c < cols && c < _objectArgs.cells[r].Count; c++)
				{
					object val = dgvCellData.Rows[r].Cells[c].Value;
					string text = val != null ? val.ToString() : "";

					// 태그 바인딩 구문 처리
					if (text.StartsWith("{") && text.EndsWith("}") && !text.Contains(" "))
					{
						_objectArgs.cells[r][c].sTagName = text.Substring(1, text.Length - 2);
						_objectArgs.cells[r][c].sText = "";
					}
					else
					{
						_objectArgs.cells[r][c].sText = text;
					}
				}
			}
		}

		void btnApplySize_Click(object sender, EventArgs e)
		{
			int newRows = (int)numRowCount.Value;
			int newCols = (int)numColCount.Value;

			if (_objectArgs == null)
			{
				_objectArgs = new ObjectArgsTable();
			}

			_objectArgs.nRowCount = newRows;
			_objectArgs.nColCount = newCols;

			// 기존 데이터 보존하면서 크기 변경
			List<System.Collections.Generic.List<TableCellData>> newCells = new System.Collections.Generic.List<System.Collections.Generic.List<TableCellData>>();
			for (int r = 0; r < newRows; r++)
			{
				System.Collections.Generic.List<TableCellData> row = new System.Collections.Generic.List<TableCellData>();
				for (int c = 0; c < newCols; c++)
				{
					if (r < _objectArgs.cells.Count && c < _objectArgs.cells[r].Count)
						row.Add(_objectArgs.cells[r][c]);
					else
						row.Add(new TableCellData());
				}
				newCells.Add(row);
			}
			_objectArgs.cells = newCells;

			// 비율 재계산
			_objectArgs.columnWidths.Clear();
			_objectArgs.rowHeights.Clear();
			float colW = 1.0f / newCols;
			float rowH = 1.0f / newRows;
			for (int c = 0; c < newCols; c++) _objectArgs.columnWidths.Add(colW);
			for (int r = 0; r < newRows; r++) _objectArgs.rowHeights.Add(rowH);

			PopulateCellDataGrid();
		}

		void btnPasteExcel_Click(object sender, EventArgs e)
		{
			string clipText = Clipboard.GetText();
			if (string.IsNullOrEmpty(clipText)) return;

			ObjectTable tempTable = null;
			// 직접 파싱
			ParseExcelClipboardText(clipText);
			PopulateCellDataGrid();
			numRowCount.Value = _objectArgs.nRowCount;
			numColCount.Value = _objectArgs.nColCount;
		}

		void ParseExcelClipboardText(string text)
		{
			string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			System.Collections.Generic.List<string[]> rows = new System.Collections.Generic.List<string[]>();
			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line) && rows.Count > 0) continue;
				rows.Add(line.Split('\t'));
			}

			if (rows.Count == 0) return;

			int maxCols = 0;
			foreach (string[] row in rows)
			{
				if (row.Length > maxCols) maxCols = row.Length;
			}

			_objectArgs.nRowCount = rows.Count;
			_objectArgs.nColCount = maxCols;
			_objectArgs.cells.Clear();
			_objectArgs.columnWidths.Clear();
			_objectArgs.rowHeights.Clear();

			float colW = 1.0f / maxCols;
			float rowH = 1.0f / rows.Count;

			for (int c = 0; c < maxCols; c++) _objectArgs.columnWidths.Add(colW);
			for (int r = 0; r < rows.Count; r++) _objectArgs.rowHeights.Add(rowH);

			for (int r = 0; r < rows.Count; r++)
			{
				System.Collections.Generic.List<TableCellData> row = new System.Collections.Generic.List<TableCellData>();
				for (int c = 0; c < maxCols; c++)
				{
					TableCellData cell = new TableCellData();
					if (c < rows[r].Length)
						cell.sText = rows[r][c];
					row.Add(cell);
				}
				_objectArgs.cells.Add(row);
			}
		}

		void btnPasteHtml_Click(object sender, EventArgs e)
		{
			string htmlText = Clipboard.GetText(TextDataFormat.Html);
			if (string.IsNullOrEmpty(htmlText))
				htmlText = Clipboard.GetText();
			if (string.IsNullOrEmpty(htmlText)) return;

			// 간단한 HTML 테이블 파싱
			ParseHtmlTable(htmlText);
			PopulateCellDataGrid();
			numRowCount.Value = _objectArgs.nRowCount;
			numColCount.Value = _objectArgs.nColCount;
		}

		void ParseHtmlTable(string html)
		{
			System.Text.RegularExpressions.Regex regRow = new System.Text.RegularExpressions.Regex(@"<tr[^>]*>(.*?)</tr>", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
			System.Text.RegularExpressions.Regex regCell = new System.Text.RegularExpressions.Regex(@"<t[dh][^>]*>(.*?)</t[dh]>", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
			System.Text.RegularExpressions.Regex regStripTags = new System.Text.RegularExpressions.Regex(@"<[^>]+>");

			System.Text.RegularExpressions.MatchCollection rowMatches = regRow.Matches(html);
			if (rowMatches.Count == 0) return;

			_objectArgs.cells.Clear();
			_objectArgs.columnWidths.Clear();
			_objectArgs.rowHeights.Clear();
			int maxCols = 0;

			foreach (System.Text.RegularExpressions.Match rowMatch in rowMatches)
			{
				System.Text.RegularExpressions.MatchCollection cellMatches = regCell.Matches(rowMatch.Groups[1].Value);
				System.Collections.Generic.List<TableCellData> row = new System.Collections.Generic.List<TableCellData>();

				foreach (System.Text.RegularExpressions.Match cellMatch in cellMatches)
				{
					TableCellData cell = new TableCellData();
					cell.sText = regStripTags.Replace(cellMatch.Groups[1].Value, "").Trim();
					cell.sText = System.Net.WebUtility.HtmlDecode(cell.sText);

					if (cellMatch.Value.StartsWith("<th", StringComparison.OrdinalIgnoreCase))
					{
						cell.bBold = true;
						cell.nTextAlign = 1;
					}

					row.Add(cell);
				}

				if (row.Count > maxCols) maxCols = row.Count;
				_objectArgs.cells.Add(row);
			}

			if (maxCols == 0) maxCols = 1;

			_objectArgs.nRowCount = _objectArgs.cells.Count;
			_objectArgs.nColCount = maxCols;

			float colW = 1.0f / maxCols;
			float rowH = 1.0f / _objectArgs.cells.Count;
			for (int c = 0; c < maxCols; c++) _objectArgs.columnWidths.Add(colW);
			for (int r = 0; r < _objectArgs.cells.Count; r++) _objectArgs.rowHeights.Add(rowH);

			// 짧은 행 패딩
			foreach (var row in _objectArgs.cells)
			{
				while (row.Count < maxCols) row.Add(new TableCellData());
			}
		}

		void btnClearTable_Click(object sender, EventArgs e)
		{
			if (_objectArgs != null)
			{
				_objectArgs.InitializeCells();
				numRowCount.Value = _objectArgs.nRowCount;
				numColCount.Value = _objectArgs.nColCount;
				PopulateCellDataGrid();
			}
		}

		void btnColor_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			ColorDialog dlg = new ColorDialog();
			dlg.Color = btn.BackColor;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				btn.BackColor = dlg.Color;

				if (btn == btnGridColor) _objectArgs.lGridColor = dlg.Color;
				else if (btn == btnHeaderBackColor) _objectArgs.lHeaderBackColor = dlg.Color;
				else if (btn == btnHeaderForeColor) _objectArgs.lHeaderForeColor = dlg.Color;
				else if (btn == btnCellBackColor) _objectArgs.lCellBackColor = dlg.Color;
				else if (btn == btnCellForeColor) _objectArgs.lCellForeColor = dlg.Color;
			}
		}
	}
}

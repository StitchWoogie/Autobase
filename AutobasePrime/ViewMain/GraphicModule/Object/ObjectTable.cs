using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetTools;
using NetTools.OldDefine;
using AutoLib;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// 테이블 오브젝트 셀 데이터.
	/// </summary>
	[Serializable]
	public class TableCellData
	{
		public string sText = "";
		public string sTagName = "";          // 태그 바인딩 이름
		public int nColSpan = 1;
		public int nRowSpan = 1;
		public int nTextAlign = 0;            // 0=Left, 1=Center, 2=Right
		public Color lForeColor = Color.Black;
		public Color lBackColor = Color.White;
		public bool bBold = false;
		public bool bMergedChild = false;      // 병합 영역의 자식 셀인 경우 true
	}

	/// <summary>
	/// 테이블 오브젝트 설정 클래스.
	/// </summary>
	[Serializable]
	public class ObjectArgsTable
	{
		public int nRowCount = 3;
		public int nColCount = 3;
		public Color lGridColor = Color.FromArgb(180, 180, 180);
		public Color lHeaderBackColor = Color.FromArgb(220, 220, 220);
		public Color lHeaderForeColor = Color.Black;
		public Color lCellBackColor = Color.White;
		public Color lCellForeColor = Color.Black;
		public int nGridLineWidth = 1;
		public bool bShowHeader = true;
		public bool bAutoSizeColumns = false;

		// 열 너비 비율 (합이 1.0)
		public List<float> columnWidths = new List<float>();
		// 행 높이 비율 (합이 1.0)
		public List<float> rowHeights = new List<float>();

		// 셀 데이터 [row][col]
		public List<List<TableCellData>> cells = new List<List<TableCellData>>();

		// 이벤트 스크립트
		public ScriptClass scriptEventCellClick;

		public void InitializeCells()
		{
			cells.Clear();
			columnWidths.Clear();
			rowHeights.Clear();

			float colWidth = 1.0f / nColCount;
			float rowHeight = 1.0f / nRowCount;

			for (int c = 0; c < nColCount; c++)
				columnWidths.Add(colWidth);

			for (int r = 0; r < nRowCount; r++)
				rowHeights.Add(rowHeight);

			for (int r = 0; r < nRowCount; r++)
			{
				List<TableCellData> row = new List<TableCellData>();
				for (int c = 0; c < nColCount; c++)
				{
					row.Add(new TableCellData());
				}
				cells.Add(row);
			}
		}
	}

	/// <summary>
	/// HMI 화면에 테이블을 표시하는 오브젝트.
	/// Excel 복사/붙여넣기, HTML 붙여넣기, 태그 데이터 바인딩, 셀 선택/편집 기능 지원.
	///
	/// 스크립트 사용:
	///   @ObjectCommand("Table1", "TableSetCell", "0,0,Hello")
	///   @ObjectCommand("Table1", "TableGetCell", "0,0")
	///   @ObjectCommand("Table1", "TableSetSize", "5,3")
	///   @ObjectCommand("Table1", "TableClear")
	///   @ObjectCommand("Table1", "TablePasteExcel", "A1\tB1\nA2\tB2")
	///   @ObjectCommand("Table1", "TablePasteHtml", "&lt;table&gt;...&lt;/table&gt;")
	///   @ObjectCommand("Table1", "TableSetCellColor", "0,0,#FF0000,#FFFFFF")
	///   @ObjectCommand("Table1", "TableMergeCells", "0,0,2,2")
	/// </summary>
	[Serializable]
	public class ObjectTable : ObjectExpand
	{
		ObjectArgsTable objArgs;

		[NonSerialized]
		static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

		[NonSerialized]
		Form formParent;

		// 런타임에서 셀 선택 상태
		[NonSerialized]
		int nSelectedRow = -1;
		[NonSerialized]
		int nSelectedCol = -1;

		// 런타임 셀 편집
		[NonSerialized]
		TextBox editBox;
		[NonSerialized]
		bool bEditing = false;

		// 태그 바인딩 캐시
		[NonSerialized]
		Dictionary<string, int[]> tagBindingCache;

		// 시스템 변수 (스크립트에서 접근)
		public static int nSystemValueColumnIndex;
		public static int nSystemValueRowIndex;

		static readonly Regex RegexTag =
			new Regex(@"\{Tag\.([^}]+)\}", RegexOptions.Compiled);

		public ObjectArgsTable ObjectArgs
		{
			set { objArgs = value; }
			get { return objArgs; }
		}

		public ObjectTable(ObjectCommonProperty ocp, Form form, RECT rect,
			EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsTable args)
			: base(ocp, rect, eid, lf, general)
		{
			enumObjectType = EnumObjectType.Table;
			objArgs = args;
			formParent = form;

			if (objArgs.cells.Count == 0)
				objArgs.InitializeCells();

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				arrayClassList.Add(this);
				BuildTagBindingCache();
			}
		}

		public override void Close()
		{
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				arrayClassList.Remove(this);
			}

			if (editBox != null)
			{
				editBox.Dispose();
				editBox = null;
			}
		}

		// ─── 태그 바인딩 ────────────────────────────────

		void BuildTagBindingCache()
		{
			tagBindingCache = new Dictionary<string, int[]>();
			for (int r = 0; r < objArgs.cells.Count; r++)
			{
				for (int c = 0; c < objArgs.cells[r].Count; c++)
				{
					string tag = objArgs.cells[r][c].sTagName;
					if (!string.IsNullOrEmpty(tag))
					{
						tagBindingCache[tag] = new int[] { r, c };
					}
				}
			}
		}

		public override async Task EventTimerObject(Form form)
		{
			// 태그 바인딩 갱신
			if (tagBindingCache == null) return;

			bool needInvalidate = false;
			foreach (var kvp in tagBindingCache)
			{
				string tagName = kvp.Key;
				int r = kvp.Value[0];
				int c = kvp.Value[1];

				string newValue = GetTagValue(tagName);
				if (newValue != null && objArgs.cells[r][c].sText != newValue)
				{
					objArgs.cells[r][c].sText = newValue;
					needInvalidate = true;
				}
			}

			if (needInvalidate)
			{
				InvalidateObject(formParent);
			}

			await Task.CompletedTask;
		}

		string GetTagValue(string tagName)
		{
			try
			{
				int[] pos = new int[1];
				pos[0] = objCommonProperty.tagServer.SearchTag(tagName);
				if (pos[0] < 0) return null;

				return objCommonProperty.tagServer.ReadTagString(pos[0]);
			}
			catch
			{
				return null;
			}
		}

		// ─── 렌더링 ────────────────────────────────────

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if (x1 > x2) Tools.Temp(ref x1, ref x2);
			if (y1 > y2) Tools.Temp(ref y1, ref y2);

			int tableWidth = x2 - x1;
			int tableHeight = y2 - y1;

			// 배경 그리기
			using (SolidBrush bgBrush = new SolidBrush(objArgs.lCellBackColor))
			{
				g.FillRectangle(bgBrush, x1, y1, tableWidth, tableHeight);
			}

			int rowCount = objArgs.nRowCount;
			int colCount = objArgs.nColCount;

			// 열 위치 계산
			int[] colPositions = new int[colCount + 1];
			colPositions[0] = x1;
			for (int c = 0; c < colCount; c++)
			{
				float ratio = (c < objArgs.columnWidths.Count) ? objArgs.columnWidths[c] : (1.0f / colCount);
				colPositions[c + 1] = colPositions[c] + (int)(tableWidth * ratio);
			}
			colPositions[colCount] = x2;

			// 행 위치 계산
			int[] rowPositions = new int[rowCount + 1];
			rowPositions[0] = y1;
			for (int r = 0; r < rowCount; r++)
			{
				float ratio = (r < objArgs.rowHeights.Count) ? objArgs.rowHeights[r] : (1.0f / rowCount);
				rowPositions[r + 1] = rowPositions[r] + (int)(tableHeight * ratio);
			}
			rowPositions[rowCount] = y2;

			Font font = MakeFont();
			Font boldFont = new Font(font, FontStyle.Bold);

			// 셀 그리기
			for (int r = 0; r < rowCount && r < objArgs.cells.Count; r++)
			{
				for (int c = 0; c < colCount && c < objArgs.cells[r].Count; c++)
				{
					TableCellData cell = objArgs.cells[r][c];
					if (cell.bMergedChild) continue;

					int cx1 = colPositions[c];
					int cy1 = rowPositions[r];
					int cx2 = (c + cell.nColSpan <= colCount) ? colPositions[c + cell.nColSpan] : x2;
					int cy2 = (r + cell.nRowSpan <= rowCount) ? rowPositions[r + cell.nRowSpan] : y2;

					// 셀 배경
					Color bgColor = cell.lBackColor;
					if (r == 0 && objArgs.bShowHeader)
						bgColor = objArgs.lHeaderBackColor;
					if (r == nSelectedRow && c == nSelectedCol && TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
						bgColor = Color.FromArgb(200, 220, 255);

					using (SolidBrush cellBrush = new SolidBrush(bgColor))
					{
						g.FillRectangle(cellBrush, cx1, cy1, cx2 - cx1, cy2 - cy1);
					}

					// 셀 텍스트
					string text = cell.sText;
					if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(cell.sTagName))
						text = "{" + cell.sTagName + "}";

					if (!string.IsNullOrEmpty(text))
					{
						Color fgColor = cell.lForeColor;
						if (r == 0 && objArgs.bShowHeader)
							fgColor = objArgs.lHeaderForeColor;

						Font drawFont = cell.bBold ? boldFont : font;

						StringFormat sf = new StringFormat();
						sf.Trimming = StringTrimming.EllipsisCharacter;
						sf.FormatFlags = StringFormatFlags.NoWrap;
						sf.LineAlignment = StringAlignment.Center;

						switch (cell.nTextAlign)
						{
							case 0: sf.Alignment = StringAlignment.Near; break;
							case 1: sf.Alignment = StringAlignment.Center; break;
							case 2: sf.Alignment = StringAlignment.Far; break;
						}

						Rectangle textRect = new Rectangle(cx1 + 3, cy1 + 1, cx2 - cx1 - 6, cy2 - cy1 - 2);

						using (SolidBrush textBrush = new SolidBrush(fgColor))
						{
							SafeException.SafeDrawString(g, text, drawFont, textBrush, textRect, sf);
						}
					}
				}
			}

			// 그리드 선 그리기
			using (Pen gridPen = new Pen(objArgs.lGridColor, objArgs.nGridLineWidth))
			{
				// 가로선
				for (int r = 0; r <= rowCount; r++)
				{
					g.DrawLine(gridPen, x1, rowPositions[r], x2, rowPositions[r]);
				}

				// 세로선
				for (int c = 0; c <= colCount; c++)
				{
					g.DrawLine(gridPen, colPositions[c], y1, colPositions[c], y2);
				}
			}

			// 편집 모드 표시 (스튜디오)
			if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
			{
				StringFormat infoFormat = new StringFormat();
				infoFormat.Alignment = StringAlignment.Center;
				infoFormat.LineAlignment = StringAlignment.Center;

				// 빈 셀에 위치 표시
				if (objArgs.cells.Count == 0 || (rowCount <= 0 || colCount <= 0))
				{
					Rectangle rect = new Rectangle(x1, y1, tableWidth, tableHeight);
					SafeException.SafeDrawString(g, "Table", font, Brushes.Gray, rect, infoFormat);
				}
			}

			boldFont.Dispose();
		}

		// ─── 마우스 이벤트 (런타임) ─────────────────────────

		public override async Task<bool> WmLeftButtonDown(Form form, MouseEventArgs e)
		{
			if (!CheckResponseOnVisible()) return false;

			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
				return await base.WmLeftButtonDown(form, e);

			int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
			GetViewZone(ref x1, ref y1, ref x2, ref y2);

			int mx = e.X;
			int my = e.Y;

			if (mx < x1 || mx > x2 || my < y1 || my > y2)
				return false;

			// 클릭한 셀 찾기
			int tableWidth = x2 - x1;
			int tableHeight = y2 - y1;

			int clickedRow = -1, clickedCol = -1;

			float cumY = 0;
			for (int r = 0; r < objArgs.nRowCount; r++)
			{
				float ratio = (r < objArgs.rowHeights.Count) ? objArgs.rowHeights[r] : (1.0f / objArgs.nRowCount);
				float nextY = cumY + tableHeight * ratio;
				if (my - y1 >= cumY && my - y1 < nextY)
				{
					clickedRow = r;
					break;
				}
				cumY = nextY;
			}

			float cumX = 0;
			for (int c = 0; c < objArgs.nColCount; c++)
			{
				float ratio = (c < objArgs.columnWidths.Count) ? objArgs.columnWidths[c] : (1.0f / objArgs.nColCount);
				float nextX = cumX + tableWidth * ratio;
				if (mx - x1 >= cumX && mx - x1 < nextX)
				{
					clickedCol = c;
					break;
				}
				cumX = nextX;
			}

			if (clickedRow >= 0 && clickedCol >= 0)
			{
				nSelectedRow = clickedRow;
				nSelectedCol = clickedCol;

				nSystemValueRowIndex = clickedRow;
				nSystemValueColumnIndex = clickedCol;

				InvalidateObject(form);

				// 셀 클릭 스크립트 실행
				if (objArgs.scriptEventCellClick != null)
				{
					objArgs.scriptEventCellClick.SetHandOperation();
					await objArgs.scriptEventCellClick.RunAsync(objCommonProperty.form, this);
				}
			}

			return true;
		}

		public override async Task<bool> WmLeftButtonUp(Form form, MouseEventArgs e)
		{
			return await base.WmLeftButtonUp(form, e);
		}

		// ─── Excel/HTML 붙여넣기 ────────────────────────────

		/// <summary>
		/// 탭/줄바꿈 구분 텍스트(Excel 복사 형식)를 테이블에 붙여넣기.
		/// </summary>
		public void PasteFromExcelText(string text)
		{
			if (string.IsNullOrEmpty(text)) return;

			string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			// 빈 줄 제거 (마지막)
			List<string[]> rows = new List<string[]>();
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

			objArgs.nRowCount = rows.Count;
			objArgs.nColCount = maxCols;
			objArgs.cells.Clear();
			objArgs.columnWidths.Clear();
			objArgs.rowHeights.Clear();

			float colW = 1.0f / maxCols;
			float rowH = 1.0f / rows.Count;

			for (int c = 0; c < maxCols; c++)
				objArgs.columnWidths.Add(colW);
			for (int r = 0; r < rows.Count; r++)
				objArgs.rowHeights.Add(rowH);

			for (int r = 0; r < rows.Count; r++)
			{
				List<TableCellData> row = new List<TableCellData>();
				for (int c = 0; c < maxCols; c++)
				{
					TableCellData cell = new TableCellData();
					if (c < rows[r].Length)
						cell.sText = rows[r][c];
					row.Add(cell);
				}
				objArgs.cells.Add(row);
			}

			BuildTagBindingCache();
			InvalidateObject(formParent);
		}

		/// <summary>
		/// HTML 테이블을 파싱하여 데이터 붙여넣기.
		/// </summary>
		public void PasteFromHtml(string html)
		{
			if (string.IsNullOrEmpty(html)) return;

			// 간단한 HTML 테이블 파서
			Regex regRow = new Regex(@"<tr[^>]*>(.*?)</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Regex regCell = new Regex(@"<t[dh][^>]*>(.*?)</t[dh]>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Regex regColSpan = new Regex(@"colspan\s*=\s*[""']?(\d+)", RegexOptions.IgnoreCase);
			Regex regRowSpan = new Regex(@"rowspan\s*=\s*[""']?(\d+)", RegexOptions.IgnoreCase);
			Regex regStripTags = new Regex(@"<[^>]+>");

			MatchCollection rowMatches = regRow.Matches(html);
			if (rowMatches.Count == 0) return;

			List<List<TableCellData>> parsedRows = new List<List<TableCellData>>();
			int maxCols = 0;

			foreach (Match rowMatch in rowMatches)
			{
				MatchCollection cellMatches = regCell.Matches(rowMatch.Groups[1].Value);
				List<TableCellData> row = new List<TableCellData>();

				foreach (Match cellMatch in cellMatches)
				{
					TableCellData cell = new TableCellData();
					cell.sText = regStripTags.Replace(cellMatch.Groups[1].Value, "").Trim();
					cell.sText = System.Net.WebUtility.HtmlDecode(cell.sText);

					// colspan
					Match csMatch = regColSpan.Match(cellMatch.Value);
					if (csMatch.Success)
						cell.nColSpan = int.Parse(csMatch.Groups[1].Value);

					// rowspan
					Match rsMatch = regRowSpan.Match(cellMatch.Value);
					if (rsMatch.Success)
						cell.nRowSpan = int.Parse(rsMatch.Groups[1].Value);

					// th는 헤더 → 볼드 + 가운데 정렬
					if (cellMatch.Value.StartsWith("<th", StringComparison.OrdinalIgnoreCase))
					{
						cell.bBold = true;
						cell.nTextAlign = 1;
					}

					row.Add(cell);
				}

				if (row.Count > maxCols) maxCols = row.Count;
				parsedRows.Add(row);
			}

			if (parsedRows.Count == 0) return;

			objArgs.nRowCount = parsedRows.Count;
			objArgs.nColCount = maxCols;
			objArgs.cells.Clear();
			objArgs.columnWidths.Clear();
			objArgs.rowHeights.Clear();

			float colW = 1.0f / maxCols;
			float rowH = 1.0f / parsedRows.Count;

			for (int c = 0; c < maxCols; c++)
				objArgs.columnWidths.Add(colW);
			for (int r = 0; r < parsedRows.Count; r++)
				objArgs.rowHeights.Add(rowH);

			// 패딩: 짧은 행에 빈 셀 채우기
			foreach (var row in parsedRows)
			{
				while (row.Count < maxCols)
					row.Add(new TableCellData());
			}

			objArgs.cells = parsedRows;

			// 병합 영역 처리
			ApplyMergedCells();
			BuildTagBindingCache();
			InvalidateObject(formParent);
		}

		void ApplyMergedCells()
		{
			for (int r = 0; r < objArgs.cells.Count; r++)
			{
				for (int c = 0; c < objArgs.cells[r].Count; c++)
				{
					TableCellData cell = objArgs.cells[r][c];
					if (cell.nColSpan > 1 || cell.nRowSpan > 1)
					{
						for (int dr = 0; dr < cell.nRowSpan; dr++)
						{
							for (int dc = 0; dc < cell.nColSpan; dc++)
							{
								if (dr == 0 && dc == 0) continue;
								int tr = r + dr;
								int tc = c + dc;
								if (tr < objArgs.cells.Count && tc < objArgs.cells[tr].Count)
								{
									objArgs.cells[tr][tc].bMergedChild = true;
								}
							}
						}
					}
				}
			}
		}

		// ─── 커맨드 ────────────────────────────────────

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
			if (command == "TableSetCell")
			{
				string param = (string)args[1];
				string[] parts = param.Split(new[] { ',' }, 3);
				if (parts.Length >= 3)
				{
					int r = int.Parse(parts[0]);
					int c = int.Parse(parts[1]);
					string val = parts[2];
					SetCellText(r, c, val);
				}
				return 1;
			}
			else if (command == "TableGetCell")
			{
				string param = (string)args[1];
				string[] parts = param.Split(',');
				if (parts.Length >= 2)
				{
					int r = int.Parse(parts[0]);
					int c = int.Parse(parts[1]);
					return GetCellText(r, c);
				}
				return "";
			}
			else if (command == "TableSetSize")
			{
				string param = (string)args[1];
				string[] parts = param.Split(',');
				if (parts.Length >= 2)
				{
					int rows = int.Parse(parts[0]);
					int cols = int.Parse(parts[1]);
					ResizeTable(rows, cols);
				}
				return 1;
			}
			else if (command == "TableClear")
			{
				ClearTable();
				return 1;
			}
			else if (command == "TablePasteExcel")
			{
				PasteFromExcelText((string)args[1]);
				return 1;
			}
			else if (command == "TablePasteHtml")
			{
				PasteFromHtml((string)args[1]);
				return 1;
			}
			else if (command == "TableSetCellColor")
			{
				// "row,col,#foreColor,#backColor"
				string param = (string)args[1];
				string[] parts = param.Split(',');
				if (parts.Length >= 4)
				{
					int r = int.Parse(parts[0]);
					int c = int.Parse(parts[1]);
					SetCellColors(r, c, ColorTranslator.FromHtml(parts[2]), ColorTranslator.FromHtml(parts[3]));
				}
				return 1;
			}
			else if (command == "TableMergeCells")
			{
				// "row,col,rowSpan,colSpan"
				string param = (string)args[1];
				string[] parts = param.Split(',');
				if (parts.Length >= 4)
				{
					int r = int.Parse(parts[0]);
					int c = int.Parse(parts[1]);
					int rs = int.Parse(parts[2]);
					int cs = int.Parse(parts[3]);
					MergeCells(r, c, rs, cs);
				}
				return 1;
			}
			else if (command == "TableSetTag")
			{
				// "row,col,tagName"
				string param = (string)args[1];
				string[] parts = param.Split(new[] { ',' }, 3);
				if (parts.Length >= 3)
				{
					int r = int.Parse(parts[0]);
					int c = int.Parse(parts[1]);
					SetCellTag(r, c, parts[2]);
				}
				return 1;
			}
			else if (command == "TableGetSelectedCell")
			{
				return string.Format("{0},{1}", nSelectedRow, nSelectedCol);
			}

			return await base.ExecuteClassName(bHandOperation, command, args);
		}

		// ─── 셀 조작 메서드 ──────────────────────────────

		public void SetCellText(int row, int col, string text)
		{
			if (row < 0 || row >= objArgs.cells.Count) return;
			if (col < 0 || col >= objArgs.cells[row].Count) return;
			objArgs.cells[row][col].sText = text;
			InvalidateObject(formParent);
		}

		public string GetCellText(int row, int col)
		{
			if (row < 0 || row >= objArgs.cells.Count) return "";
			if (col < 0 || col >= objArgs.cells[row].Count) return "";
			return objArgs.cells[row][col].sText ?? "";
		}

		public void SetCellColors(int row, int col, Color foreColor, Color backColor)
		{
			if (row < 0 || row >= objArgs.cells.Count) return;
			if (col < 0 || col >= objArgs.cells[row].Count) return;
			objArgs.cells[row][col].lForeColor = foreColor;
			objArgs.cells[row][col].lBackColor = backColor;
			InvalidateObject(formParent);
		}

		public void MergeCells(int row, int col, int rowSpan, int colSpan)
		{
			if (row < 0 || row >= objArgs.cells.Count) return;
			if (col < 0 || col >= objArgs.cells[row].Count) return;

			objArgs.cells[row][col].nRowSpan = rowSpan;
			objArgs.cells[row][col].nColSpan = colSpan;

			for (int dr = 0; dr < rowSpan; dr++)
			{
				for (int dc = 0; dc < colSpan; dc++)
				{
					if (dr == 0 && dc == 0) continue;
					int tr = row + dr;
					int tc = col + dc;
					if (tr < objArgs.cells.Count && tc < objArgs.cells[tr].Count)
					{
						objArgs.cells[tr][tc].bMergedChild = true;
					}
				}
			}

			InvalidateObject(formParent);
		}

		public void SetCellTag(int row, int col, string tagName)
		{
			if (row < 0 || row >= objArgs.cells.Count) return;
			if (col < 0 || col >= objArgs.cells[row].Count) return;
			objArgs.cells[row][col].sTagName = tagName;
			BuildTagBindingCache();
		}

		public void ResizeTable(int newRows, int newCols)
		{
			if (newRows <= 0 || newCols <= 0) return;

			// 기존 데이터 보존하면서 크기 변경
			List<List<TableCellData>> newCells = new List<List<TableCellData>>();
			for (int r = 0; r < newRows; r++)
			{
				List<TableCellData> row = new List<TableCellData>();
				for (int c = 0; c < newCols; c++)
				{
					if (r < objArgs.cells.Count && c < objArgs.cells[r].Count)
						row.Add(objArgs.cells[r][c]);
					else
						row.Add(new TableCellData());
				}
				newCells.Add(row);
			}

			objArgs.nRowCount = newRows;
			objArgs.nColCount = newCols;
			objArgs.cells = newCells;

			// 비율 재계산
			objArgs.columnWidths.Clear();
			objArgs.rowHeights.Clear();
			float colW = 1.0f / newCols;
			float rowH = 1.0f / newRows;
			for (int c = 0; c < newCols; c++) objArgs.columnWidths.Add(colW);
			for (int r = 0; r < newRows; r++) objArgs.rowHeights.Add(rowH);

			BuildTagBindingCache();
			InvalidateObject(formParent);
		}

		public void ClearTable()
		{
			for (int r = 0; r < objArgs.cells.Count; r++)
			{
				for (int c = 0; c < objArgs.cells[r].Count; c++)
				{
					objArgs.cells[r][c].sText = "";
					objArgs.cells[r][c].sTagName = "";
					objArgs.cells[r][c].bMergedChild = false;
					objArgs.cells[r][c].nColSpan = 1;
					objArgs.cells[r][c].nRowSpan = 1;
				}
			}

			tagBindingCache?.Clear();
			InvalidateObject(formParent);
		}

		// ─── 저장/로드 ──────────────────────────────────

		public override void ObjectSave(CommaTextWriter writer)
		{
			ObjectSaveFont(writer);

			writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.nRowCount);
			writer.Write("{0},", objArgs.nColCount);
			writer.Write("{0},", objArgs.lGridColor.ToArgb());
			writer.Write("{0},", objArgs.lHeaderBackColor.ToArgb());
			writer.Write("{0},", objArgs.lHeaderForeColor.ToArgb());
			writer.Write("{0},", objArgs.lCellBackColor.ToArgb());
			writer.Write("{0},", objArgs.lCellForeColor.ToArgb());
			writer.Write("{0},", objArgs.nGridLineWidth);
			writer.Write("{0},", objArgs.bShowHeader ? 1 : 0);
			writer.Write("{0},", objArgs.bAutoSizeColumns ? 1 : 0);
			writer.WriteLine();

			// 열 너비 저장
			writer.Write("\tTableColumnWidths,");
			for (int c = 0; c < objArgs.columnWidths.Count; c++)
			{
				writer.Write(string.Format(CultureTool.ciKR, "{0},", objArgs.columnWidths[c]));
			}
			writer.WriteLine();

			// 행 높이 저장
			writer.Write("\tTableRowHeights,");
			for (int r = 0; r < objArgs.rowHeights.Count; r++)
			{
				writer.Write(string.Format(CultureTool.ciKR, "{0},", objArgs.rowHeights[r]));
			}
			writer.WriteLine();

			// 셀 데이터 저장
			for (int r = 0; r < objArgs.cells.Count; r++)
			{
				for (int c = 0; c < objArgs.cells[r].Count; c++)
				{
					TableCellData cell = objArgs.cells[r][c];
					writer.Write("\tTableCell,");
					writer.Write("{0},{1},", r, c);
					writer.Write("{0},", EscapeCellText(cell.sText));
					writer.Write("{0},", EscapeCellText(cell.sTagName));
					writer.Write("{0},{1},", cell.nColSpan, cell.nRowSpan);
					writer.Write("{0},", cell.nTextAlign);
					writer.Write("{0},{1},", cell.lForeColor.ToArgb(), cell.lBackColor.ToArgb());
					writer.Write("{0},", cell.bBold ? 1 : 0);
					writer.Write("{0},", cell.bMergedChild ? 1 : 0);
					writer.WriteLine();
				}
			}

			// 스크립트 저장
			if (objArgs.scriptEventCellClick != null)
			{
				objArgs.scriptEventCellClick.SaveScript(writer, "ScriptEventCellClick");
			}
		}

		static string EscapeCellText(string text)
		{
			if (string.IsNullOrEmpty(text)) return "";
			// 콤마와 줄바꿈 이스케이프
			return text.Replace("\\", "\\\\").Replace(",", "\\c").Replace("\r\n", "\\n").Replace("\n", "\\n");
		}

		public static string UnescapeCellText(string text)
		{
			if (string.IsNullOrEmpty(text)) return "";
			return text.Replace("\\n", "\n").Replace("\\c", ",").Replace("\\\\", "\\");
		}

		// ─── 트리뷰 표시 ────────────────────────────────

		public override void AddObjectInfo(TreeNode parent)
		{
			string info = Path.GetExtension(this.ToString()).Substring(1);
			info += string.Format(", {0}x{1}", objArgs.nRowCount, objArgs.nColCount);
			parent.Nodes.Add(info);
		}

		public override string GetObjectMainTitle()
		{
			return string.Format("Table {0}x{1}", objArgs.nRowCount, objArgs.nColCount);
		}

		public override void OnVisible(bool flag)
		{
			// 테이블은 자체 렌더링이므로 특별한 처리 불필요
		}
	}
}

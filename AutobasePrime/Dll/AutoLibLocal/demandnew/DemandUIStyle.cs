using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoLibLocal.DemandNew
{
	/// <summary>
	/// 신형 디맨드 UI 공통 스타일 헬퍼.
	/// 모든 DemandNew 관련 폼에서 일관된 모던 UI를 적용한다.
	/// </summary>
	public static class DemandUIStyle
	{
		// ── Color Palette ──
		public static readonly Color Primary      = Color.FromArgb(24, 78, 196);     // #184EC4 진한 블루
		public static readonly Color PrimaryDark   = Color.FromArgb(18, 58, 148);     // #123A94
		public static readonly Color PrimaryLight  = Color.FromArgb(210, 226, 252);   // #D2E2FC
		public static readonly Color Accent        = Color.FromArgb(0, 128, 80);      // #008050
		public static readonly Color Danger        = Color.FromArgb(200, 30, 30);     // #C81E1E
		public static readonly Color DangerDark    = Color.FromArgb(160, 20, 20);     // #A01414
		public static readonly Color Background    = Color.FromArgb(235, 238, 242);   // #EBEEF2 눈에 띄는 회색
		public static readonly Color Surface       = Color.White;
		public static readonly Color Border        = Color.FromArgb(200, 206, 214);   // #C8CED6 진한 테두리
		public static readonly Color TextPrimary   = Color.FromArgb(34, 34, 34);      // #222222 거의 검정
		public static readonly Color TextSecondary = Color.FromArgb(80, 80, 80);      // #505050 진한 회색
		public static readonly Color GridAltRow    = Color.FromArgb(245, 247, 250);   // #F5F7FA
		public static readonly Color GridHeader    = Color.FromArgb(230, 234, 240);   // #E6EAF0 눈에 띄는 헤더

		// ── Fonts ──
		private const string FontFamily = "맑은 고딕";  // Malgun Gothic
		public static readonly Font FontTitle    = new Font(FontFamily, 12f, FontStyle.Bold);
		public static readonly Font FontSubtitle = new Font(FontFamily, 10f, FontStyle.Bold);
		public static readonly Font FontNormal   = new Font(FontFamily, 10f);
		public static readonly Font FontSmall    = new Font(FontFamily, 9f);
		public static readonly Font FontHeader   = new Font(FontFamily, 10.5f, FontStyle.Bold);

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		//  Form
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		public static void StyleForm(Form form)
		{
			form.BackColor = Background;
			form.Font = FontNormal;
			form.ForeColor = TextPrimary;
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		//  Buttons
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		public static void StyleButtonPrimary(Button btn)
		{
			btn.FlatStyle = FlatStyle.Flat;
			btn.FlatAppearance.BorderSize = 0;
			btn.BackColor = Primary;
			btn.ForeColor = Color.White;
			btn.Font = FontHeader;
			btn.Cursor = Cursors.Hand;
			btn.FlatAppearance.MouseOverBackColor = PrimaryDark;
		}

		public static void StyleButtonSecondary(Button btn)
		{
			btn.FlatStyle = FlatStyle.Flat;
			btn.FlatAppearance.BorderColor = Border;
			btn.FlatAppearance.BorderSize = 1;
			btn.BackColor = Surface;
			btn.ForeColor = TextPrimary;
			btn.Font = FontNormal;
			btn.Cursor = Cursors.Hand;
			btn.FlatAppearance.MouseOverBackColor = PrimaryLight;
		}

		public static void StyleButtonDanger(Button btn)
		{
			btn.FlatStyle = FlatStyle.Flat;
			btn.FlatAppearance.BorderSize = 0;
			btn.BackColor = Danger;
			btn.ForeColor = Color.White;
			btn.Font = FontHeader;
			btn.Cursor = Cursors.Hand;
			btn.FlatAppearance.MouseOverBackColor = DangerDark;
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		//  TabControl (owner-draw)
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		public static void StyleTabControl(TabControl tab)
		{
			tab.DrawMode = TabDrawMode.OwnerDrawFixed;
			tab.SizeMode = TabSizeMode.Fixed;
			tab.ItemSize = new Size(100, 38);
			tab.DrawItem += TabControl_DrawItem;

			foreach (TabPage page in tab.TabPages)
				page.BackColor = Surface;
		}

		private static void TabControl_DrawItem(object sender, DrawItemEventArgs e)
		{
			var tab = (TabControl)sender;
			var page = tab.TabPages[e.Index];
			var bounds = tab.GetTabRect(e.Index);
			bool sel = (e.Index == tab.SelectedIndex);

			// 배경
			using (var bg = new SolidBrush(sel ? Surface : Background))
				e.Graphics.FillRectangle(bg, bounds);

			// 선택된 탭 하단 악센트 라인
			if (sel)
			{
				using (var pen = new Pen(Primary, 3))
					e.Graphics.DrawLine(pen,
						bounds.Left + 2, bounds.Bottom - 1,
						bounds.Right - 2, bounds.Bottom - 1);
			}

			// 탭 텍스트
			var textColor = sel ? Primary : TextSecondary;
			var font = sel ? FontHeader : FontNormal;
			using (var brush = new SolidBrush(textColor))
			{
				var sf = new StringFormat
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center
				};
				e.Graphics.DrawString(page.Text, font, brush, bounds, sf);
			}
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		//  DataGridView
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		public static void StyleDataGridView(DataGridView dgv)
		{
			dgv.BackgroundColor = Surface;
			dgv.BorderStyle = BorderStyle.None;
			dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
			dgv.GridColor = Border;
			dgv.EnableHeadersVisualStyles = false;

			dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeader;
			dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
			dgv.ColumnHeadersDefaultCellStyle.Font = FontHeader;
			dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = GridHeader;
			dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextPrimary;
			dgv.ColumnHeadersHeight = 36;
			dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

			dgv.RowTemplate.Height = 32;
			dgv.DefaultCellStyle.SelectionBackColor = PrimaryLight;
			dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
			dgv.DefaultCellStyle.Font = FontSmall;
			dgv.AlternatingRowsDefaultCellStyle.BackColor = GridAltRow;
			dgv.RowHeadersVisible = false;
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		//  ListView (owner-draw)
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		public static void StyleListView(ListView lv)
		{
			lv.FullRowSelect = true;
			lv.GridLines = false;
			lv.BorderStyle = BorderStyle.None;
			lv.BackColor = Surface;
			lv.ForeColor = TextPrimary;
			lv.Font = FontNormal;
			lv.OwnerDraw = true;

			// DoubleBuffered 활성화 (protected 속성이므로 리플렉션 사용)
			var prop = typeof(ListView).GetProperty("DoubleBuffered",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (prop != null) prop.SetValue(lv, true, null);

			lv.DrawColumnHeader += ListView_DrawColumnHeader;
			lv.DrawItem += ListView_DrawItem;
			lv.DrawSubItem += ListView_DrawSubItem;
		}

		private static void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			using (var bg = new SolidBrush(GridHeader))
				e.Graphics.FillRectangle(bg, e.Bounds);

			using (var brush = new SolidBrush(TextPrimary))
			{
				var sf = new StringFormat { LineAlignment = StringAlignment.Center };
				var r = new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 12, e.Bounds.Height);
				e.Graphics.DrawString(e.Header.Text, FontHeader, brush, r, sf);
			}

			using (var pen = new Pen(Border))
				e.Graphics.DrawLine(pen,
					e.Bounds.Left, e.Bounds.Bottom - 1,
					e.Bounds.Right, e.Bounds.Bottom - 1);
		}

		private static Color GetListViewRowBackColor(ListViewItem item, int itemIndex)
		{
			if (item.Selected)
				return PrimaryLight;
			if (itemIndex % 2 == 1)
				return GridAltRow;
			return Surface;
		}

		private static void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			var lv = (ListView)sender;
			Color bg = GetListViewRowBackColor(e.Item, e.ItemIndex);

			// 전체 행 배경 그리기
			using (var brush = new SolidBrush(bg))
				e.Graphics.FillRectangle(brush, e.Bounds);

			// 모든 컬럼 텍스트를 여기서 그린다 (DrawSubItem 호버 누락 방지)
			var textColor = e.Item.Selected ? Primary : TextPrimary;
			using (var textBrush = new SolidBrush(textColor))
			using (var sf = new StringFormat())
			{
				sf.LineAlignment = StringAlignment.Center;
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags = StringFormatFlags.NoWrap;

				int x = e.Bounds.X;
				for (int i = 0; i < lv.Columns.Count; i++)
				{
					int colW = lv.Columns[i].Width;
					string text = (i < e.Item.SubItems.Count) ? e.Item.SubItems[i].Text : "";
					var r = new Rectangle(x + 6, e.Bounds.Y, Math.Max(1, colW - 12), e.Bounds.Height);
					e.Graphics.DrawString(text, FontNormal, textBrush, r, sf);
					x += colW;
				}
			}
		}

		private static void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			// WinForms 알려진 버그: column 0 의 e.Bounds 가 전체 행 너비를 반환
			// → column 0 배경 그리기 시 다른 컬럼 텍스트를 덮어씀
			// 실제 컬럼 너비로 제한하여 처리한다
			var lv = (ListView)sender;
			Rectangle bounds = e.Bounds;
			if (e.ColumnIndex == 0 && lv.Columns.Count > 0)
				bounds = new Rectangle(e.Bounds.X, e.Bounds.Y, lv.Columns[0].Width, e.Bounds.Height);

			// 서브아이템 배경
			Color bg = GetListViewRowBackColor(e.Item, e.ItemIndex);
			using (var bgBrush = new SolidBrush(bg))
				e.Graphics.FillRectangle(bgBrush, bounds);

			// 서브아이템 텍스트
			var color = e.Item.Selected ? Primary : TextPrimary;
			using (var brush = new SolidBrush(color))
			using (var sf = new StringFormat())
			{
				sf.LineAlignment = StringAlignment.Center;
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags = StringFormatFlags.NoWrap;
				var r = new Rectangle(bounds.X + 6, bounds.Y, Math.Max(1, bounds.Width - 12), bounds.Height);
				e.Graphics.DrawString(e.SubItem.Text, FontNormal, brush, r, sf);
			}
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		//  Section Header Label (■ 일반, ■ 계약 등)
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		public static void StyleSectionLabel(Label lbl)
		{
			lbl.Font = new Font(FontFamily, 11f, FontStyle.Bold);
			lbl.ForeColor = Primary;
		}

		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		//  Recursive: 모든 자식 컨트롤 스타일 적용
		// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
		/// <summary>
		/// parent 아래 모든 자식 컨트롤에 현대적 스타일을 재귀 적용한다.
		/// Button.Tag = "color_swatch" 설정 시 버튼 BackColor를 보존한다.
		/// </summary>
		public static void StyleAllControls(Control parent)
		{
			foreach (Control c in parent.Controls)
			{
				if (c is Button btn)
				{
					// 색상 스와치 버튼 (BackColor가 데이터)
					if (btn.Tag as string == "color_swatch")
					{
						btn.FlatStyle = FlatStyle.Flat;
						btn.FlatAppearance.BorderColor = Border;
						btn.FlatAppearance.BorderSize = 1;
						// BackColor 유지
					}
					// 태그 브라우즈 버튼
					else if (btn.Text == "...")
					{
						btn.FlatStyle = FlatStyle.Flat;
						btn.FlatAppearance.BorderColor = Border;
						btn.FlatAppearance.BorderSize = 1;
						btn.BackColor = Surface;
						btn.ForeColor = TextSecondary;
						btn.FlatAppearance.MouseOverBackColor = PrimaryLight;
					}
					else
					{
						string txt = (btn.Text ?? "").Trim().ToLower();
						if (txt == "ok" || txt == "확인" || txt == "apply" || txt == "적용"
							|| txt == "save" || txt == "저장")
							StyleButtonPrimary(btn);
						else if (txt == "delete" || txt == "삭제" || txt == "remove" || txt == "제거")
							StyleButtonDanger(btn);
						else
							StyleButtonSecondary(btn);
					}
				}
				else if (c is TextBox tb)
				{
					tb.BackColor = Surface;
					tb.BorderStyle = BorderStyle.FixedSingle;
				}
				else if (c is ComboBox cb)
				{
					cb.FlatStyle = FlatStyle.Flat;
					cb.BackColor = Surface;
				}
				else if (c is NumericUpDown nud)
				{
					nud.BorderStyle = BorderStyle.FixedSingle;
					nud.BackColor = Surface;
				}
				else if (c is DataGridView dgv)
				{
					StyleDataGridView(dgv);
				}
				else if (c is ListView lv)
				{
					StyleListView(lv);
				}
				else if (c is GroupBox gb)
				{
					gb.ForeColor = TextPrimary;
					gb.Font = FontSubtitle;
					gb.BackColor = Surface;
				}
				else if (c is TabControl tc)
				{
					StyleTabControl(tc);
				}
				else if (c is TabPage tp)
				{
					tp.BackColor = Surface;
				}
				else if (c is Panel pnl)
				{
					// 하단 버튼 패널은 Background, 나머지는 Surface
					pnl.BackColor = (pnl.Dock == DockStyle.Bottom) ? Background : Surface;
				}

				// 자식으로 재귀 (DataGridView/ListView/NumericUpDown/ComboBox 내부는 스킵)
				if (c.HasChildren
					&& !(c is DataGridView) && !(c is ListView)
					&& !(c is NumericUpDown) && !(c is ComboBox))
				{
					StyleAllControls(c);
				}
			}
		}
	}
}

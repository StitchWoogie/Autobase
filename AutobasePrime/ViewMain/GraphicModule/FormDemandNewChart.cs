using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using NetTools;

namespace GraphicModule
{
	public class FormDemandNewChart : Form
	{
		private System.ComponentModel.Container components = null;

		private Font _font;
		private DemandSnapshotBus _bus;
		private DemandSnapshot _currentSnapshot;
		private string _blockId = "";
		private int _percentY = 120;

		private Rectangle _plotRect;
		private Rectangle _kpiRect;
		private int _statusBarPos;

		private Color _colorPrediction = Color.FromArgb(0, 122, 204);
		private Color _colorExcess = Color.FromArgb(217, 72, 15);
		private Color _colorTarget = Color.FromArgb(32, 167, 86);
		private Color _colorGuideLine = Color.FromArgb(208, 215, 224);
		private Color _colorText = Color.FromArgb(36, 44, 58);
		private BrushPublic _colorFill = new BrushPublic();
		private BrushPublic _colorBack = new BrushPublic();
		private Color _colorStatusFill = Color.FromArgb(51, 78, 104);
		private Color _colorStatusBack = Color.FromArgb(236, 241, 246);
		private Color _colorStatusTitle = Color.FromArgb(36, 44, 58);
		private Color _colorStatusValue = Color.FromArgb(18, 33, 48);
		private Color _colorTargetValue = Color.FromArgb(32, 167, 86);
		private Color _colorPreValue = Color.FromArgb(0, 122, 204);
		private Color _colorExValue = Color.FromArgb(217, 72, 15);
		private int _lineThickTarget = 1;

		private int _uiTheme = 0;
		private bool _showAreaFill = true;
		private bool _showPeakLine = true;
		private bool _showForecastBand = true;
		private bool _showGridLabels = true;
		private bool _showStepLines = true;
		private bool _showTimeZoneTarget = true;
		private bool _showDetailedAxis = true;
		private int _gridIntensity = 45;
		private int _lineThicknessTrend = 2;
		private int _lineThicknessForecast = 1;
		private int _kpiDensity = 0;
		private int _cornerRadius = 8;
		private int _kpiOpacity = 82;
		private Color _colorCardBack = Color.FromArgb(245, 248, 252);
		private Color _colorCardBorder = Color.FromArgb(206, 216, 227);
		private Color _colorAreaFill = Color.FromArgb(48, 255, 141, 64);
		private Color _colorForecastBand = Color.FromArgb(48, 30, 136, 229);
		private Color _colorPeakLine = Color.FromArgb(255, 255, 152, 0);

		private DemandNewConfig _config;

		public FormDemandNewChart()
		{
			InitializeComponent();
			_font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			AutoScaleMode = AutoScaleMode.None;
			ClientSize = new Size(400, 300);
			FormBorderStyle = FormBorderStyle.None;
			Name = "FormDemandNewChart";
			Paint += new PaintEventHandler(OnPaint);
			Resize += new EventHandler(OnResize);
			Load += new EventHandler(OnLoad);
			FormClosed += new FormClosedEventHandler(OnFormClosed);
			ResumeLayout(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null) components.Dispose();
			base.Dispose(disposing);
		}

		public void SetBlockId(string blockId)
		{
			_blockId = blockId ?? "";
			if (ConfigVarTotal.bLocalFlag)
				LoadConfigByBlockId();
			// 원격 모드: ObjectDemandChart.PollRemoteData()에서 SetConfig() 호출
		}
		public void SetPercentY(int percentY) { _percentY = Math.Max(100, Math.Min(200, percentY)); }
		public void SetLogFont(Font font) { if (font != null) _font = font; }
		public void SetLineThickTarget(int thick) { _lineThickTarget = Math.Max(1, thick); }

		public void SetColors(Color prediction, Color excess, Color target, Color guideLine,
			Color text, BrushPublic fill, BrushPublic back, Color statusBack, Color statusFill,
			Color statusValue, Color targetValue, Color preValue, Color exValue, int statusBarPos)
		{
			_colorPrediction = prediction;
			_colorExcess = excess;
			_colorTarget = target;
			_colorGuideLine = guideLine;
			_colorText = text;
			_colorFill = fill;
			_colorBack = back;
			_colorStatusBack = statusBack;
			_colorStatusFill = statusFill;
			_colorStatusValue = statusValue;
			_colorTargetValue = targetValue;
			_colorPreValue = preValue;
			_colorExValue = exValue;
			_statusBarPos = statusBarPos;
		}

		public void SetVisualOptions(ObjectArgsDemandChart args)
		{
			if (args == null) return;
			_uiTheme = DemandChartThemeHelper.ClampTheme(args.uiTheme);
			_showAreaFill = args.showAreaFill;
			_showPeakLine = args.showPeakLine;
			_showForecastBand = args.showForecastBand;
			_showGridLabels = args.showGridLabels;
			_showStepLines = args.showStepLines;
			_showTimeZoneTarget = args.showTimeZoneTarget;
			_showDetailedAxis = args.showDetailedAxis;
			_gridIntensity = Math.Max(0, Math.Min(100, args.gridIntensity));
			_lineThicknessTrend = Math.Max(1, Math.Min(5, args.lineThicknessTrend));
			_lineThicknessForecast = Math.Max(1, Math.Min(5, args.lineThicknessForecast));
			_kpiDensity = Math.Max(0, Math.Min(1, args.kpiDensity));
			_cornerRadius = Math.Max(0, Math.Min(16, args.cornerRadius));
			_kpiOpacity = Math.Max(30, Math.Min(100, args.kpiOpacity));
			_colorCardBack = args.lColorCardBack;
			_colorCardBorder = args.lColorCardBorder;
			_colorAreaFill = args.lColorAreaFill;
			_colorForecastBand = args.lColorForecastBand;
			_colorPeakLine = args.lColorPeakLine;
			_colorStatusTitle = args.lColorStatusTitle;
		}

		private Color ThemeText(Color c) { return DemandChartThemeHelper.ApplyText(c, _uiTheme); }
		private Color ThemeGuide(Color c) { return DemandChartThemeHelper.ApplyGuideLine(c, _uiTheme); }
		private Color ThemeAccent(Color c) { return DemandChartThemeHelper.ApplyAccent(c, _uiTheme); }
		private Color ThemeDanger(Color c) { return DemandChartThemeHelper.ApplyDanger(c, _uiTheme); }
		private Color ThemeStatus(Color c) { return DemandChartThemeHelper.ApplyStatus(c, _uiTheme); }
		private Color ThemeCardBack(Color c) { return DemandChartThemeHelper.ApplyCardBack(c, _uiTheme); }
		private Color ThemeCardBorder(Color c) { return DemandChartThemeHelper.ApplyCardBorder(c, _uiTheme); }

		public void SubscribeToBus(DemandSnapshotBus bus)
		{
			if (_bus != null) _bus.SnapshotPublished -= OnSnapshotPublished;
			_bus = bus;
			if (_bus != null) _bus.SnapshotPublished += OnSnapshotPublished;
		}

		/// <summary>
		/// 원격 모드에서 스냅샷을 직접 업데이트 (DataGate 폴링용)
		/// </summary>
		public void UpdateSnapshot(DemandSnapshot snapshot)
		{
			_currentSnapshot = snapshot;
			if (InvokeRequired) BeginInvoke(new Action(() => Invalidate()));
			else Invalidate();
		}

		/// <summary>
		/// 원격 모드에서 Config를 직접 설정 (LoadConfigByBlockId 대체)
		/// </summary>
		public void SetConfig(DemandNewConfig config)
		{
			_config = config;
		}

		private void OnLoad(object sender, EventArgs e)
		{
			CalcLayout();
		}

		private void OnResize(object sender, EventArgs e)
		{
			CalcLayout();
			Invalidate();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			if (_bus != null) _bus.SnapshotPublished -= OnSnapshotPublished;
		}

		private void OnSnapshotPublished(object sender, DemandSnapshot snapshot)
		{
			_currentSnapshot = snapshot;
			if (InvokeRequired) BeginInvoke(new Action(() => Invalidate()));
			else Invalidate();
		}

		private void LoadConfigByBlockId()
		{
			_config = null;
			if (string.IsNullOrEmpty(_blockId)) return;

			try
			{
				List<DemandNewConfig> list = DemandNewConfigLoader.LoadAll();
				for (int i = 0; i < list.Count; i++)
				{
					if (string.Equals(list[i].BlockId, _blockId, StringComparison.OrdinalIgnoreCase))
					{
						_config = list[i];
						break;
					}
				}
			}
			catch
			{
			}
		}

		public void CalcLayout()
		{
			int W = ClientSize.Width;
			int H = ClientSize.Height;
			if (W < 20 || H < 20)
			{
				_plotRect = new Rectangle(0, 0, Math.Max(8, W), Math.Max(8, H));
				_kpiRect = Rectangle.Empty;
				return;
			}

			int fontH = Math.Max(8, (_font != null) ? _font.Height : 10);

			// 기본 스케일: 기준 크기(400×300) 대비 비례 계산
			float scaleX = W / 400f;
			float scaleY = H / 300f;
			float scale = Math.Max(0.4f, Math.Min(2.5f, Math.Min(scaleX, scaleY)));

			int margin = Math.Max(2, (int)(8 * scale));

			// ── KPI 영역: 폼 크기에 비례 ──
			int kpiPct = (_kpiDensity == 0) ? 12 : 18;   // 높이의 12% 또는 18%
			int kpiMinH = Math.Max(20, fontH * 2 + 6);
			int kpiHeight = (_statusBarPos == 2) ? 0 : Math.Max(kpiMinH, H * kpiPct / 100);

			int leftPanelW = 0;
			if (_statusBarPos == 1)
			{
				leftPanelW = Math.Max(fontH * 6, W * 22 / 100);
				int textW = TextRenderer.MeasureText("Forecast 9999.9kW", _font).Width + 24;
				leftPanelW = Math.Min(W * 35 / 100, Math.Max(leftPanelW, textW));
			}

			// ── 축 여백: 폼 크기에 비례 ──
			int axisLeftPad = _showGridLabels ? Math.Max(8, (int)((_showDetailedAxis ? 46 : 34) * scale)) : Math.Max(4, margin);
			int axisBottomPad = _showGridLabels ? Math.Max(8, (int)((_showDetailedAxis ? 26 : 20) * scale)) : Math.Max(4, margin);
			int axisTopPad = Math.Max(2, (int)(8 * scale));

			// 범례 우측 여백: 폼 너비의 최대 28%로 제한
			int axisRightPad;
			if (_showStepLines)
			{
				int legendNeeded = EstimateLegendWidth() + 14;
				axisRightPad = Math.Max(margin, Math.Min(legendNeeded, W * 28 / 100));
			}
			else
			{
				axisRightPad = Math.Max(4, margin);
			}

			// ── 최종 레이아웃 ──
			if (_statusBarPos == 1)
			{
				_kpiRect = new Rectangle(margin, margin, Math.Max(20, leftPanelW - margin), H - margin * 2);

				int px = leftPanelW + margin + axisLeftPad;
				int py = margin + axisTopPad;
				int pw = W - px - margin - axisRightPad;
				int ph = H - py - margin - axisBottomPad;
				_plotRect = new Rectangle(px, py, pw, ph);
			}
			else
			{
				_kpiRect = new Rectangle(margin, margin, W - margin * 2, kpiHeight);

				int topBase = margin + kpiHeight + ((_statusBarPos == 2) ? 0 : (int)(4 * scale));
				int px = margin + axisLeftPad;
				int py = topBase + axisTopPad;
				int pw = W - px - margin - axisRightPad;
				int ph = H - py - margin - axisBottomPad;
				_plotRect = new Rectangle(px, py, pw, ph);
			}

			if (_plotRect.Width < 8) _plotRect.Width = 8;
			if (_plotRect.Height < 8) _plotRect.Height = 8;
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

			DemandSnapshot snap = _currentSnapshot;

			DrawCanvasBackground(g);
			DrawGridAndAxis(g, snap);
			DrawTargetAndBands(g, snap);
			DrawTrendAndForecast(g, snap);
			DrawKpiCards(g, snap);

			if (snap == null)
			{
				Font baseFont = GetSafeFont();
				string msg = Tools.IsLangKorean() ? "데이터 없음" : "No Data";
				using (Brush b = new SolidBrush(Color.FromArgb(130, ThemeText(_colorText))))
					g.DrawString(msg, baseFont, b, _plotRect.Left + 10, _plotRect.Top + 8);
			}
		}

		private void DrawCanvasBackground(Graphics g)
		{
			Brush backBrush = ObjectRectangle.MakePublicBrush(_colorBack, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
			DrawClass.gcls(g, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1, backBrush);

			if (_uiTheme == 1)
			{
				using (Brush shade = new SolidBrush(Color.FromArgb(52, 12, 18, 28)))
					g.FillRectangle(shade, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
			}
			else if (_uiTheme == 2)
			{
				using (Brush shade = new SolidBrush(Color.FromArgb(72, 255, 255, 255)))
					g.FillRectangle(shade, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
			}

			Color frameColor = DemandChartThemeHelper.GetFrameColor(_uiTheme);
			using (Pen border = new Pen(frameColor, 1))
				g.DrawRectangle(border, 0, 0, Math.Max(1, ClientSize.Width - 1), Math.Max(1, ClientSize.Height - 1));
		}

		private void DrawGridAndAxis(Graphics g, DemandSnapshot snap)
		{
			int graphW = _plotRect.Width;
			int graphH = _plotRect.Height;
			if (graphW <= 2 || graphH <= 2) return;

			int alpha = 30 + (int)(_gridIntensity * 1.7);
			Font baseFont = GetSafeFont();
			Color gridColor = Color.FromArgb(Math.Max(20, Math.Min(200, alpha)), ThemeGuide(_colorGuideLine));
			double maxKW = (snap != null) ? GetMaxKw(snap) : 100.0;
			int yStep = GetYGridStep(maxKW, graphH, _showDetailedAxis);
			int maxAxisKw = Math.Max(yStep, (int)(Math.Ceiling(maxKW / yStep) * yStep));
			int totalMin = (snap != null) ? Math.Max(1, snap.IntervalMinutes) : 15;
			int xStepMin = GetXGridMinuteStep(totalMin, graphW, _showDetailedAxis);

			using (Pen gridPen = new Pen(gridColor, 1f))
			{
				gridPen.DashStyle = DashStyle.Dot;
				for (int kw = 0; kw <= maxAxisKw; kw += yStep)
				{
					int y = _plotRect.Bottom - (int)(_plotRect.Height * (kw / (double)maxAxisKw));
					g.DrawLine(gridPen, _plotRect.Left, y, _plotRect.Right, y);
				}

				for (int m = 0; m <= totalMin; m += xStepMin)
				{
					int x = _plotRect.Left + (int)(_plotRect.Width * (m / (double)totalMin));
					g.DrawLine(gridPen, x, _plotRect.Top, x, _plotRect.Bottom);
				}
			}

			using (Pen axis = new Pen(Color.FromArgb(140, ThemeGuide(_colorGuideLine)), 1.2f))
			{
				g.DrawLine(axis, _plotRect.Left, _plotRect.Bottom, _plotRect.Right, _plotRect.Bottom);
				g.DrawLine(axis, _plotRect.Left, _plotRect.Top, _plotRect.Left, _plotRect.Bottom);
			}

			if (!_showGridLabels) return;

			using (Brush textBrush = new SolidBrush(ThemeText(_colorText)))
			{
				for (int kw = 0; kw <= maxAxisKw; kw += yStep)
				{
					int y = _plotRect.Bottom - (int)(_plotRect.Height * (kw / (double)maxAxisKw));
					string yText = kw.ToString();
					SizeF sy = g.MeasureString(yText, baseFont);
					g.DrawString(yText, baseFont, textBrush, _plotRect.Left - sy.Width - 6, y - sy.Height / 2);
				}

				for (int m = 0; m <= totalMin; m += xStepMin)
				{
					int x = _plotRect.Left + (int)(_plotRect.Width * (m / (double)totalMin));
					string xText = String.Format("{0}:00", m);
					SizeF sx = g.MeasureString(xText, baseFont);
					float tx = x - sx.Width / 2;
					float minX = _plotRect.Left - 2;
					float maxX = _plotRect.Right - sx.Width + 2;
					if (tx < minX) tx = minX;
					if (tx > maxX) tx = maxX;
					g.DrawString(xText, baseFont, textBrush, tx, _plotRect.Bottom + 4);
				}
			}
		}

		private void DrawTargetAndBands(Graphics g, DemandSnapshot snap)
		{
			if (snap == null) return;
			double maxKW = GetMaxKw(snap);
			int targetY = ToY(snap.EffectiveTargetKW, maxKW);

			if (_showForecastBand)
			{
				int bandTop = Math.Max(_plotRect.Top, targetY - 6);
				int bandBottom = Math.Min(_plotRect.Bottom, targetY + 6);
				using (Brush b = new SolidBrush(ThemeAccent(_colorForecastBand)))
					g.FillRectangle(b, _plotRect.Left, bandTop, _plotRect.Width, Math.Max(1, bandBottom - bandTop));
			}

			using (Pen targetPen = new Pen(ThemeAccent(_colorTarget), Math.Max(1, _lineThickTarget + 1)))
			{
				targetPen.StartCap = LineCap.Round;
				targetPen.EndCap = LineCap.Round;
				g.DrawLine(targetPen, _plotRect.Left, targetY, _plotRect.Right, targetY);
			}

			DrawStepLines(g, snap, maxKW);
			DrawTimeZoneTargetLine(g, snap, maxKW);

			if (_showPeakLine && snap.MonthlyPeakKW > 0)
			{
				int peakY = ToY(snap.MonthlyPeakKW, maxKW);
				using (Pen peakPen = new Pen(ThemeDanger(_colorPeakLine), 1f))
				{
					peakPen.DashStyle = DashStyle.DashDot;
					g.DrawLine(peakPen, _plotRect.Left, peakY, _plotRect.Right, peakY);
				}
			}
		}

		private void DrawStepLines(Graphics g, DemandSnapshot snap, double maxKW)
		{
			if (!_showStepLines || _config == null) return;

			double[] stepCum = GetStepCumulativeReductions();
			if (stepCum == null || stepCum.Length == 0) return;

			Color[] stepColors = new Color[]
			{
				Color.FromArgb(140, 64, 138, 196),
				Color.FromArgb(150, 94, 164, 51),
				Color.FromArgb(150, 230, 126, 34)
			};
			List<string> legendTexts = new List<string>();
			List<Color> legendColors = new List<Color>();
			legendTexts.Add(String.Format("Target {0:F0}kW", snap.EffectiveTargetKW));
			legendColors.Add(ThemeAccent(_colorTarget));

			for (int i = 0; i < stepCum.Length && i < 3; i++)
			{
				double lineKw = snap.EffectiveTargetKW + stepCum[i];
				int y = ToY(lineKw, maxKW);
				using (Pen p = new Pen(stepColors[i], 1f))
				{
					p.DashStyle = DashStyle.Dash;
					g.DrawLine(p, _plotRect.Left, y, _plotRect.Right, y);
				}

				legendTexts.Add(String.Format("S{0} +{1:F0}kW", i + 1, stepCum[i]));
				legendColors.Add(stepColors[i]);
			}

			DrawStepLegend(g, legendTexts, legendColors);
		}

		private void DrawStepLegend(Graphics g, List<string> texts, List<Color> colors)
		{
			if (texts == null || colors == null || texts.Count == 0) return;

			Font baseFont = GetSafeFont();
			int lineH = (int)Math.Ceiling(baseFont.GetHeight(g)) + 2;
			int maxTextW = 0;
			Font legendFont = new Font(baseFont, FontStyle.Bold);
			for (int i = 0; i < texts.Count; i++)
			{
				Size sz = TextRenderer.MeasureText(texts[i], legendFont);
				if (sz.Width > maxTextW) maxTextW = sz.Width;
			}

			int pad = 6;
			int markerW = 14;
			int legendW = pad + markerW + 6 + maxTextW + pad;
			int legendH = pad + texts.Count * lineH + pad;
			int lx = _plotRect.Right + 6;
			int ly = _plotRect.Top + 6;

			int maxAvailW = ClientSize.Width - lx - 6;
			if (maxAvailW < 72) return;
			if (legendW > maxAvailW) legendW = maxAvailW;

			Rectangle panel = new Rectangle(lx, ly, legendW, legendH);
			using (Brush back = new SolidBrush(Color.FromArgb(182, ThemeCardBack(_colorCardBack))))
				g.FillRectangle(back, panel);
			using (Pen border = new Pen(Color.FromArgb(165, ThemeCardBorder(_colorCardBorder))))
				g.DrawRectangle(border, panel);

			for (int i = 0; i < texts.Count && i < colors.Count; i++)
			{
				int y = ly + pad + i * lineH + lineH / 2;
				using (Pen p = new Pen(colors[i], 2.2f))
				{
					if (i > 0) p.DashStyle = DashStyle.Dash;
					g.DrawLine(p, lx + pad, y, lx + pad + markerW, y);
				}
				using (Brush b = new SolidBrush(Color.Black))
					g.DrawString(texts[i], legendFont, b, lx + pad + markerW + 6, ly + pad + i * lineH - 1);
			}
			legendFont.Dispose();
		}

		private void DrawTimeZoneTargetLine(Graphics g, DemandSnapshot snap, double maxKW)
		{
			if (!_showTimeZoneTarget || _config == null || _config.TimeZoneTargets == null) return;

			double tzTargetKw = 0;
			for (int i = 0; i < _config.TimeZoneTargets.Count; i++)
			{
				TimeZoneTarget tz = _config.TimeZoneTargets[i];
				if (tz.IsActiveAt(DateTime.Now) && tz.TargetKW > 0)
				{
					tzTargetKw = tz.TargetKW * _config.SafetyFactor;
					break;
				}
			}
			if (tzTargetKw <= 0) return;

			int y = ToY(tzTargetKw, maxKW);
			using (Pen p = new Pen(Color.FromArgb(180, 156, 39, 176), 1f))
			{
				p.DashStyle = DashStyle.DashDotDot;
				g.DrawLine(p, _plotRect.Left, y, _plotRect.Right, y);
			}
			Font baseFont = GetSafeFont();
			using (Brush b = new SolidBrush(Color.FromArgb(220, 156, 39, 176)))
				g.DrawString(String.Format("TZ {0:F0}kW", tzTargetKw), baseFont, b, _plotRect.Left + 92, y - 14);
		}

		private void DrawTrendAndForecast(Graphics g, DemandSnapshot snap)
		{
			if (snap == null || snap.DemandCurve == null || snap.DemandCurveLength <= 1) return;

			double maxKW = GetMaxKw(snap);
			int totalSec = Math.Max(1, snap.IntervalMinutes * 60);

			Point[] pts = new Point[snap.DemandCurveLength];
			for (int i = 0; i < snap.DemandCurveLength; i++)
			{
				int x = _plotRect.Left + (_plotRect.Width * i / totalSec);
				int y = ToY(snap.DemandCurve[i], maxKW);
				pts[i] = new Point(x, y);
			}

			if (_showAreaFill)
			{
				Point[] poly = new Point[pts.Length + 2];
				Array.Copy(pts, poly, pts.Length);
				poly[poly.Length - 2] = new Point(pts[pts.Length - 1].X, _plotRect.Bottom);
				poly[poly.Length - 1] = new Point(pts[0].X, _plotRect.Bottom);
				using (Brush b = new SolidBrush(ThemeAccent(_colorAreaFill)))
					g.FillPolygon(b, poly);
			}

			using (Pen trendPen = new Pen(ThemeAccent(_colorPrediction), _lineThicknessTrend))
			{
				trendPen.StartCap = LineCap.Round;
				trendPen.EndCap = LineCap.Round;
				trendPen.LineJoin = LineJoin.Round;
				g.DrawLines(trendPen, pts);
			}

			if (snap.RemainingSeconds > 0)
			{
				Point last = pts[pts.Length - 1];
				int forecastY = ToY(snap.ForecastDemandEndKW, maxKW);
				Color forecastColor = (snap.ForecastDemandEndKW > snap.EffectiveTargetKW)
					? ThemeDanger(_colorExcess)
					: ThemeAccent(_colorPrediction);
				using (Pen p = new Pen(forecastColor, _lineThicknessForecast))
				{
					p.DashStyle = DashStyle.Dash;
					p.StartCap = LineCap.Round;
					p.EndCap = LineCap.Round;
					g.DrawLine(p, last.X, last.Y, _plotRect.Right, forecastY);
				}
			}
		}

		private void DrawKpiCards(Graphics g, DemandSnapshot snap)
		{
			if (_statusBarPos == 2 || _kpiRect.Width <= 0 || _kpiRect.Height <= 0 || snap == null) return;
			Font baseFont = GetSafeFont();

			int shedCount = 0;
			if (snap.Loads != null)
			{
				for (int i = 0; i < snap.Loads.Count; i++)
					if (snap.Loads[i].IsShed) shedCount++;
			}

			string[] labels = Tools.IsLangKorean()
				? new[] { "목표", "예측", "현재", "경과", "차단", "모드" }
				: new[] { "Target", "Forecast", "Current", "Elapsed", "Shed", "Mode" };
			string[] values = new string[]
			{
				String.Format("{0:F0}kW", snap.EffectiveTargetKW),
				String.Format("{0:F0}kW", snap.ForecastDemandEndKW),
				String.Format("{0:F1}kW", snap.CurrentKW),
				String.Format("{0}:{1:D2}", snap.ElapsedSeconds / 60, snap.ElapsedSeconds % 60),
				shedCount.ToString(),
				(snap.Mode == EngineMode.Active) ? "Active" : "Shadow"
			};
			Color[] valueColors = new Color[]
			{
				ThemeAccent(_colorTargetValue),
				(snap.ForecastDemandEndKW > snap.EffectiveTargetKW) ? ThemeDanger(_colorExValue) : ThemeAccent(_colorPreValue),
				ThemeStatus(_colorStatusValue),
				ThemeStatus(_colorStatusValue),
				(shedCount > 0) ? ThemeDanger(_colorExValue) : ThemeStatus(_colorStatusValue),
				(snap.Mode == EngineMode.Active) ? ThemeDanger(Color.Orange) : ThemeAccent(Color.SteelBlue)
			};

			int count = 6;
			if (_statusBarPos == 1)
			{
				int gap = Math.Max(2, _kpiRect.Height / 80);
				int cardH = Math.Max(14, (_kpiRect.Height - gap * (count - 1)) / count);
				for (int i = 0; i < count; i++)
				{
					Rectangle rc = new Rectangle(_kpiRect.Left, _kpiRect.Top + i * (cardH + gap), _kpiRect.Width, cardH);
					if (rc.Bottom > _kpiRect.Bottom) break;
					DrawCard(g, baseFont, rc, labels[i], values[i], valueColors[i]);
				}
			}
			else
			{
				int gap = Math.Max(2, _kpiRect.Width / 120);
				int cardW = Math.Max(16, (_kpiRect.Width - gap * (count - 1)) / count);
				for (int i = 0; i < count; i++)
				{
					Rectangle rc = new Rectangle(_kpiRect.Left + i * (cardW + gap), _kpiRect.Top, cardW, _kpiRect.Height);
					if (rc.Right > _kpiRect.Right) break;
					DrawCard(g, baseFont, rc, labels[i], values[i], valueColors[i]);
				}
			}
		}

		private void DrawCard(Graphics g, Font baseFont, Rectangle rc, string label, string value, Color valueColor)
		{
			Color cardBack = Color.FromArgb((int)(255.0 * _kpiOpacity / 100.0), ThemeCardBack(_colorCardBack));
			using (GraphicsPath path = CreateRoundRect(rc, _cornerRadius))
			{
				using (Brush b = new SolidBrush(cardBack))
					g.FillPath(b, path);
				using (Pen p = new Pen(ThemeCardBorder(_colorCardBorder), 1f))
					g.DrawPath(p, path);
			}

			// 실제 폰트 높이 기반 레이아웃 (고정 rc.Height/2 분할 대신)
			int fH = (int)Math.Ceiling(baseFont.GetHeight(g));
			int totalTextH = fH * 2;
			int gap = Math.Max(0, Math.Min(2, (rc.Height - totalTextH) / 3));
			int topPad = Math.Max(1, (rc.Height - totalTextH - gap) / 2);
			int hPad = Math.Max(2, Math.Min(6, rc.Width / 16));
			int textW = Math.Max(1, rc.Width - hPad * 2);

			Rectangle labelRect = new Rectangle(rc.Left + hPad, rc.Top + topPad, textW, fH);
			Rectangle valueRect = new Rectangle(rc.Left + hPad, rc.Top + topPad + fH + gap, textW, fH);

			using (Brush lb = new SolidBrush(ThemeText(_colorStatusTitle)))
			using (Brush vb = new SolidBrush(valueColor))
			using (StringFormat sf = new StringFormat())
			{
				sf.Alignment = StringAlignment.Near;
				sf.LineAlignment = StringAlignment.Center;
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags = StringFormatFlags.NoWrap;
				g.DrawString(label, baseFont, lb, labelRect, sf);
				g.DrawString(value, baseFont, vb, valueRect, sf);
			}
		}

		private GraphicsPath CreateRoundRect(Rectangle rc, int radius)
		{
			GraphicsPath p = new GraphicsPath();
			int r = Math.Max(0, Math.Min(16, radius));
			if (r <= 0)
			{
				p.AddRectangle(rc);
				return p;
			}

			int d = r * 2;
			p.AddArc(rc.Left, rc.Top, d, d, 180, 90);
			p.AddArc(rc.Right - d, rc.Top, d, d, 270, 90);
			p.AddArc(rc.Right - d, rc.Bottom - d, d, d, 0, 90);
			p.AddArc(rc.Left, rc.Bottom - d, d, d, 90, 90);
			p.CloseFigure();
			return p;
		}

		private int ToY(double kw, double maxKw)
		{
			if (maxKw <= 0) maxKw = 1;
			double ratio = kw / maxKw;
			if (ratio < 0) ratio = 0;
			if (ratio > 1.25) ratio = 1.25;
			int y = _plotRect.Bottom - (int)(_plotRect.Height * ratio);
			if (y < _plotRect.Top) y = _plotRect.Top;
			if (y > _plotRect.Bottom) y = _plotRect.Bottom;
			return y;
		}

		private double GetMaxKw(DemandSnapshot snap)
		{
			double max = Math.Max(100, snap.EffectiveTargetKW * _percentY / 100.0);
			if (snap.ForecastDemandEndKW > max) max = snap.ForecastDemandEndKW * 1.1;
			if (snap.CurrentKW > max) max = snap.CurrentKW * 1.1;
			if (snap.MonthlyPeakKW > max) max = snap.MonthlyPeakKW * 1.1;

			double[] stepCum = GetStepCumulativeReductions();
			if (stepCum != null && stepCum.Length > 0)
			{
				for (int i = 0; i < stepCum.Length; i++)
				{
					double v = snap.EffectiveTargetKW + stepCum[i];
					if (v > max) max = v * 1.05;
				}
			}
			return Math.Ceiling(max / 10.0) * 10.0;
		}

		private int GetYGridStep(double maxKw, int graphHeight, bool detailedAxis)
		{
			int maxLabels = Math.Max(3, graphHeight / (detailedAxis ? 20 : 26));
			double raw = Math.Max(1.0, maxKw / maxLabels);
			double exponent = Math.Pow(10.0, Math.Floor(Math.Log10(raw)));
			double fraction = raw / exponent;
			double niceFraction;
			if (fraction <= 1.0) niceFraction = 1.0;
			else if (fraction <= 2.0) niceFraction = 2.0;
			else if (fraction <= 5.0) niceFraction = 5.0;
			else niceFraction = 10.0;
			int step = (int)Math.Round(niceFraction * exponent);
			if (step < 10) step = 10;
			return step;
		}

		private int GetXGridMinuteStep(int totalMin, int graphWidth, bool detailedAxis)
		{
			int maxLabels = Math.Max(3, graphWidth / (detailedAxis ? 44 : 64));
			double raw = Math.Max(1.0, totalMin / (double)maxLabels);
			int[] choices = new int[] { 1, 2, 5, 10, 15, 30, 60 };
			for (int i = 0; i < choices.Length; i++)
			{
				if (raw <= choices[i]) return choices[i];
			}
			return Math.Max(1, (int)Math.Ceiling(raw / 60.0) * 60);
		}

		private int EstimateLegendWidth()
		{
			string[] texts = new[] { "Target 9999kW", "S1 +999kW", "S2 +999kW", "S3 +999kW" };
			Font baseFont = GetSafeFont();
			Font legendFont = new Font(baseFont, FontStyle.Bold);
			int maxTextW = 0;
			for (int i = 0; i < texts.Length; i++)
			{
				int w = TextRenderer.MeasureText(texts[i], legendFont).Width;
				if (w > maxTextW) maxTextW = w;
			}
			legendFont.Dispose();
			return 6 + 14 + 6 + maxTextW + 6;
		}

		private Font GetSafeFont()
		{
			if (_font != null) return _font;
			return SystemFonts.DefaultFont;
		}

		private double[] GetStepCumulativeReductions()
		{
			if (_config == null) return null;
			double[] step = new double[3];

			if (_config.Steps != null && _config.Steps.Count > 0)
			{
				double acc = 0;
				for (int i = 0; i < 3 && i < _config.Steps.Count; i++)
				{
					acc += Math.Max(0, _config.Steps[i].ReductionKW);
					step[i] = acc;
				}
				return step;
			}

			if (_config.Loads != null && _config.Loads.Count > 0)
			{
				double s1 = 0, s2 = 0, s3 = 0;
				for (int i = 0; i < _config.Loads.Count; i++)
				{
					LoadModel l = _config.Loads[i];
					if (l == null) continue;
					if (l.Priority <= 3) s1 += Math.Max(0, l.EstimatedKW);
					else if (l.Priority <= 6) s2 += Math.Max(0, l.EstimatedKW);
					else s3 += Math.Max(0, l.EstimatedKW);
				}
				step[0] = s1;
				step[1] = s1 + s2;
				step[2] = s1 + s2 + s3;
				return step;
			}

			return null;
		}
	}
}

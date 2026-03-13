using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using NetTools;
using NetTools.OldDefine;

namespace GraphicModule
{
	[Serializable]
	public class ObjectDemandChart : ObjectExpand
	{
		ObjectArgsDemandChart objArgs;

		public ObjectArgsDemandChart ObjectArgs
		{
			get { return objArgs; }
			set { objArgs = value; }
		}

		[NonSerialized] FormDemandNewChart wndChild;
		[NonSerialized] Form formParent;
		[NonSerialized] static public ArrayList arrayClassList = new ArrayList();
		[NonSerialized] Timer _pollTimer;
		[NonSerialized] bool _configFetched;

		/// <summary>
		/// 외부에서 SnapshotBus를 조회하는 콜백 (LocalMain 초기화 시 설정)
		/// </summary>
		[NonSerialized]
		public static Func<string, DemandSnapshotBus> GetBusCallback;

		public ObjectDemandChart(ObjectCommonProperty ocp, Form form, RECT rect,
			EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsDemandChart args)
			: base(ocp, rect, eid, lf, general)
		{
			enumObjectType = EnumObjectType.DemandChart;
			bSupportObjectOnCE = false;
			objArgs = args;
			formParent = form;

			TextColor = args.lColorText;
			BackColor = args.lColorBack;
			FillColor = args.lColorFill;

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				CreateChildForm(form, rect);
			}
		}

		private void CreateChildForm(Form form, RECT rect)
		{
			wndChild = new FormDemandNewChart();
			wndChild.SetBlockId(objArgs.demand_block_id);
			wndChild.SetPercentY(objArgs.nPercentY);
			wndChild.SetLineThickTarget(objArgs.thick_target);
			wndChild.SetLogFont(MakeFont());

			wndChild.SetColors(
				objArgs.lColorPrediction,
				objArgs.lColorExcess,
				objArgs.lColorTarget,
				objArgs.lColorGuideLine,
				GetTextColor(),
				GetFillColor(),
				GetBackColor(),
				objArgs.lColorStatusBack,
				objArgs.lColorStatusFill,
				objArgs.lColorStatusValue,
				objArgs.lColorTargetValue,
				objArgs.lColorPreValue,
				objArgs.lColorExValue,
				objArgs.nStatusBarPos
			);
			wndChild.SetVisualOptions(objArgs);

			if (ConfigVarTotal.bLocalFlag)
			{
				// 로컬 모드: 스냅샷 버스 직접 구독
				if (GetBusCallback != null)
				{
					var bus = GetBusCallback(objArgs.demand_block_id);
					if (bus != null)
						wndChild.SubscribeToBus(bus);
				}
			}
			else
			{
				// 원격 모드: DataGate 통한 폴링
				_configFetched = false;
				_pollTimer = new Timer();
				_pollTimer.Interval = 1000;
				_pollTimer.Tick += PollRemoteData;
				_pollTimer.Start();
			}

			wndChild.TopLevel = false;
			wndChild.FormBorderStyle = FormBorderStyle.None;
			form.Controls.Add(wndChild);

			int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
			GetViewZone(ref x1, ref y1, ref x2, ref y2);
			wndChild.Left = x1;
			wndChild.Top = y1;
			wndChild.Width = x2 - x1;
			wndChild.Height = y2 - y1;

			wndChild.Show();
			arrayClassList.Add(this);
			SetToolTipOnChildWindow(wndChild);
			OnVisible(ExpandCalcVisible());
		}

		public override void Close()
		{
			if (_pollTimer != null)
			{
				_pollTimer.Stop();
				_pollTimer.Dispose();
				_pollTimer = null;
			}

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (wndChild != null)
					wndChild.Close();
				arrayClassList.Remove(this);
			}
		}

		/// <summary>
		/// 원격 모드: 매 1초마다 DataGate를 통해 서버에서 스냅샷/설정을 폴링
		/// </summary>
		private void PollRemoteData(object sender, EventArgs e)
		{
			if (wndChild == null) return;
			string blockId = objArgs.demand_block_id;
			if (string.IsNullOrEmpty(blockId)) return;

			try
			{
				// 1회: Config 가져오기
				if (!_configFetched)
				{
					string configData = DataGate.GetDemandNewConfig(blockId);
					if (configData != null)
					{
						DemandNewConfig config = DemandNewConfigLoader.ParseConfigLine(configData);
						if (config != null)
						{
							wndChild.SetConfig(config);
							_configFetched = true;
						}
					}
				}

				// 매초: Snapshot 가져오기
				string snapData = DataGate.GetDemandNewSnapshot(blockId);
				if (snapData != null)
				{
					DemandSnapshot snapshot = DemandSnapshotSerializer.Deserialize(snapData);
					if (snapshot != null)
						wndChild.UpdateSnapshot(snapshot);
				}
			}
			catch
			{
				// 통신 오류 → 다음 폴링에서 재시도 (크래시 방지)
			}
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
			{
				DisplayObjectEdit(g, x1, y1, x2, y2);
			}
			else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (wndChild != null)
					UpdateControlState(wndChild, formParent);
			}
		}

		public override void OnMove(int x1, int y1, int x2, int y2)
		{
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && wndChild != null)
			{
				wndChild.SuspendLayout();
				wndChild.SetLogFont(MakeFont());
				wndChild.SetBounds(x1, y1, x2 - x1, y2 - y1);
				wndChild.CalcLayout();
				wndChild.ResumeLayout(false);
				wndChild.Invalidate();
			}
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			ObjectSaveFont(writer);
			SaveObjectItem.BackColor(writer, GetBackColor());
			SaveObjectItem.TextColor(writer, GetTextColor());
			SaveObjectItem.FillColor(writer, GetFillColor());
			SaveObjectItem.GuideLineColor(writer, objArgs.lColorGuideLine);

			writer.Write("\tStringOption,");
			writer.Write("v4,");
			writer.Write("{0},", objArgs.demand_block_id);
			writer.Write("{0},", objArgs.thick_target);
			writer.Write("{0},", objArgs.nStatusBarPos);
			writer.Write("{0},", objArgs.nPercentY);
			writer.Write("{0},", objArgs.uiTheme);
			writer.Write("{0},", objArgs.showAreaFill ? 1 : 0);
			writer.Write("{0},", objArgs.showPeakLine ? 1 : 0);
			writer.Write("{0},", objArgs.showForecastBand ? 1 : 0);
			writer.Write("{0},", objArgs.showGridLabels ? 1 : 0);
			writer.Write("{0},", objArgs.showStepLines ? 1 : 0);
			writer.Write("{0},", objArgs.showTimeZoneTarget ? 1 : 0);
			writer.Write("{0},", objArgs.showDetailedAxis ? 1 : 0);
			writer.Write("{0},", objArgs.gridIntensity);
			writer.Write("{0},", objArgs.lineThicknessTrend);
			writer.Write("{0},", objArgs.lineThicknessForecast);
			writer.Write("{0},", objArgs.kpiDensity);
			writer.Write("{0},", objArgs.cornerRadius);
			writer.Write("{0},", objArgs.kpiOpacity);

			// 색상 RGBA
			WriteColorRGBA(writer, objArgs.lColorPrediction);
			WriteColorRGBA(writer, objArgs.lColorExcess);
			WriteColorRGBA(writer, objArgs.lColorTarget);
			WriteColorRGBA(writer, objArgs.lColorStatusFill);
			WriteColorRGBA(writer, objArgs.lColorStatusBack);
			WriteColorRGBA(writer, objArgs.lColorStatusTitle);
			WriteColorRGBA(writer, objArgs.lColorStatusValue);
			WriteColorRGBA(writer, objArgs.lColorTargetValue);
			WriteColorRGBA(writer, objArgs.lColorPreValue);
			WriteColorRGBA(writer, objArgs.lColorExValue);
			WriteColorRGBA(writer, objArgs.lColorCardBack);
			WriteColorRGBA(writer, objArgs.lColorCardBorder);
			WriteColorRGBA(writer, objArgs.lColorAreaFill);
			WriteColorRGBA(writer, objArgs.lColorForecastBand);
			WriteColorRGBA(writer, objArgs.lColorPeakLine);

			writer.WriteLine();
		}

		private void WriteColorRGBA(CommaTextWriter writer, Color c)
		{
			writer.Write("{0},{1},{2},{3},", c.R, c.G, c.B, c.A);
		}

		/// <summary>
		/// Studio 편집 모드에서의 미리보기 렌더링
		/// </summary>
		private void DisplayObjectEdit(Graphics g, int x1, int y1, int x2, int y2)
		{
			int theme = DemandChartThemeHelper.ClampTheme(objArgs.uiTheme);
			Color textColor = DemandChartThemeHelper.ApplyText(GetTextColor(), theme);
			Color titleColor = DemandChartThemeHelper.ApplyText(objArgs.lColorStatusTitle, theme);
			Color guideColor = DemandChartThemeHelper.ApplyGuideLine(objArgs.lColorGuideLine, theme);
			Color predictionColor = DemandChartThemeHelper.ApplyAccent(objArgs.lColorPrediction, theme);
			Color excessColor = DemandChartThemeHelper.ApplyDanger(objArgs.lColorExcess, theme);
			Color targetColor = DemandChartThemeHelper.ApplyAccent(objArgs.lColorTarget, theme);
			Color targetValueColor = DemandChartThemeHelper.ApplyAccent(objArgs.lColorTargetValue, theme);
			Color forecastBandColor = DemandChartThemeHelper.ApplyAccent(objArgs.lColorForecastBand, theme);
			Color areaFillColor = DemandChartThemeHelper.ApplyAccent(objArgs.lColorAreaFill, theme);
			Color peakColor = DemandChartThemeHelper.ApplyDanger(objArgs.lColorPeakLine, theme);
			Color cardBackColor = DemandChartThemeHelper.ApplyCardBack(objArgs.lColorCardBack, theme);
			Color cardBorderColor = DemandChartThemeHelper.ApplyCardBorder(objArgs.lColorCardBorder, theme);

			Region oldClip = g.Clip;
			g.Clip = new Region(new Rectangle(x1, y1, x2 - x1, y2 - y1));

			Brush backBrush = ObjectRectangle.MakePublicBrush(GetBackColor(), x1, y1, x2, y2);
			DrawClass.gcls(g, x1, y1, x2, y2, backBrush);
			if (theme == 1)
			{
				using (Brush shade = new SolidBrush(Color.FromArgb(52, 12, 18, 28)))
					g.FillRectangle(shade, x1, y1, x2 - x1, y2 - y1);
			}
			else if (theme == 2)
			{
				using (Brush shade = new SolidBrush(Color.FromArgb(72, 255, 255, 255)))
					g.FillRectangle(shade, x1, y1, x2 - x1, y2 - y1);
			}

			using (Pen border = new Pen(DemandChartThemeHelper.GetFrameColor(theme), 1))
				g.DrawRectangle(border, x1, y1, x2 - x1 - 1, y2 - y1 - 1);

			Font font = MakeFont() ?? new Font("Tahoma", 9);
			int fontH = Math.Max(10, (int)Math.Ceiling(font.GetHeight(g)));
			string title = string.IsNullOrEmpty(objArgs.demand_block_id)
				? "DemandChart (New)"
				: String.Format("DemandChart [{0}]", objArgs.demand_block_id);
			using (Brush titleBrush = new SolidBrush(textColor))
				g.DrawString(title, font, titleBrush, x1 + 4, y1 + 4);

			int totalW = Math.Max(10, x2 - x1);
			int totalH = Math.Max(10, y2 - y1);

			// 런타임 CalcLayout()과 동일한 비례 계산
			float scaleX = totalW / 400f;
			float scaleY = totalH / 300f;
			float scale = Math.Max(0.4f, Math.Min(2.5f, Math.Min(scaleX, scaleY)));

			int margin = Math.Max(2, (int)(8 * scale));
			int kpiPct = (objArgs.kpiDensity == 0) ? 12 : 18;
			int kpiMinH = Math.Max(20, fontH * 2 + 6);
			int kpiHeight = (objArgs.nStatusBarPos == 2) ? 0 : Math.Max(kpiMinH, totalH * kpiPct / 100);
			int leftPanelW = 0;
			if (objArgs.nStatusBarPos == 1)
			{
				leftPanelW = Math.Max(fontH * 6, totalW * 22 / 100);
				float sampleTextW = g.MeasureString("Forecast 9999.9kW", font).Width;
				leftPanelW = Math.Min(totalW * 35 / 100, Math.Max(leftPanelW, (int)Math.Ceiling(sampleTextW) + 24));
			}
			int axisLeftPad = objArgs.showGridLabels ? Math.Max(8, (int)((objArgs.showDetailedAxis ? 46 : 34) * scale)) : Math.Max(4, margin);
			int axisBottomPad = objArgs.showGridLabels ? Math.Max(8, (int)((objArgs.showDetailedAxis ? 26 : 20) * scale)) : Math.Max(4, margin);
			int axisTopPad = Math.Max(2, (int)(8 * scale));
			Size legendSize = GetPreviewLegendSize(g, font, objArgs.showStepLines);
			int axisRightPad;
			if (objArgs.showStepLines)
			{
				int legendNeeded = legendSize.Width + 14;
				axisRightPad = Math.Max(margin, Math.Min(legendNeeded, totalW * 28 / 100));
			}
			else
			{
				axisRightPad = Math.Max(4, margin);
			}

			Rectangle kpiRect;
			Rectangle plotRect;
			if (objArgs.nStatusBarPos == 1)
			{
				kpiRect = new Rectangle(x1 + margin, y1 + margin, Math.Max(20, leftPanelW - margin), totalH - margin * 2);
				int px = x1 + leftPanelW + margin + axisLeftPad;
				int py = y1 + margin + axisTopPad;
				int pw = x2 - px - margin - axisRightPad;
				int ph = y2 - py - margin - axisBottomPad;
				plotRect = new Rectangle(px, py, pw, ph);
			}
			else
			{
				kpiRect = new Rectangle(x1 + margin, y1 + margin, totalW - margin * 2, kpiHeight);
				int topBase = y1 + margin + kpiHeight + ((objArgs.nStatusBarPos == 2) ? 0 : (int)(4 * scale));
				int px = x1 + margin + axisLeftPad;
				int py = topBase + axisTopPad;
				int pw = x2 - px - margin - axisRightPad;
				int ph = y2 - py - margin - axisBottomPad;
				plotRect = new Rectangle(px, py, pw, ph);
			}
			if (plotRect.Width < 8) plotRect.Width = 8;
			if (plotRect.Height < 8) plotRect.Height = 8;

			Brush fillBrush = ObjectRectangle.MakePublicBrush(GetFillColor(), plotRect.Left, plotRect.Top, plotRect.Right, plotRect.Bottom);
			DrawClass.gcls(g, plotRect.Left, plotRect.Top, plotRect.Right, plotRect.Bottom, fillBrush);

			double maxKWRaw = Math.Max(100, ((objArgs.nPercentY + 9) / 10) * 10);
			int yStep = GetPreviewYStep(maxKWRaw, plotRect.Height, objArgs.showDetailedAxis);
			int maxKW = Math.Max(yStep, (int)(Math.Ceiling(maxKWRaw / yStep) * yStep));
			int xTotalMin = 15;
			int xStepMin = GetPreviewXMinuteStep(xTotalMin, plotRect.Width, objArgs.showDetailedAxis);
			int gridAlpha = 30 + (int)(Math.Max(0, Math.Min(100, objArgs.gridIntensity)) * 1.7);

			using (Pen gridPen = new Pen(Color.FromArgb(Math.Max(20, Math.Min(200, gridAlpha)), guideColor), 1f))
			{
				gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
				for (int kw = 0; kw <= maxKW; kw += yStep)
				{
					int y = plotRect.Bottom - (int)(plotRect.Height * (kw / (double)maxKW));
					g.DrawLine(gridPen, plotRect.Left, y, plotRect.Right, y);
				}
				for (int m = 0; m <= xTotalMin; m += xStepMin)
				{
					int x = plotRect.Left + (int)(plotRect.Width * (m / (double)xTotalMin));
					g.DrawLine(gridPen, x, plotRect.Top, x, plotRect.Bottom);
				}
			}
			using (Pen axis = new Pen(Color.FromArgb(180, guideColor), 1.2f))
			{
				g.DrawLine(axis, plotRect.Left, plotRect.Bottom, plotRect.Right, plotRect.Bottom);
				g.DrawLine(axis, plotRect.Left, plotRect.Top, plotRect.Left, plotRect.Bottom);
			}
			if (objArgs.showGridLabels)
			{
				using (Brush lb = new SolidBrush(textColor))
				{
					for (int kw = 0; kw <= maxKW; kw += yStep)
					{
						int y = plotRect.Bottom - (int)(plotRect.Height * (kw / (double)maxKW));
						string t = kw.ToString();
						SizeF s = g.MeasureString(t, font);
						g.DrawString(t, font, lb, plotRect.Left - s.Width - 6, y - s.Height / 2);
					}
					for (int m = 0; m <= xTotalMin; m += xStepMin)
					{
						int x = plotRect.Left + (int)(plotRect.Width * (m / (double)xTotalMin));
						string t = String.Format("{0}:00", m);
						SizeF s = g.MeasureString(t, font);
						float tx = x - s.Width / 2;
						if (tx < plotRect.Left - 2) tx = plotRect.Left - 2;
						if (tx > plotRect.Right - s.Width + 2) tx = plotRect.Right - s.Width + 2;
						g.DrawString(t, font, lb, tx, plotRect.Bottom + 3);
					}
				}
			}

			int targetY = plotRect.Top + (int)(plotRect.Height * 0.38);
			int midX = plotRect.Left + plotRect.Width / 2;
			int trendEndX = plotRect.Right - plotRect.Width / 6;
			if (objArgs.showAreaFill)
			{
				// 트렌드 라인과 동일한 좌표 사용 (런타임과 일치)
				Point[] fillPoly = new Point[]
				{
					new Point(plotRect.Left, plotRect.Bottom - 8),
					new Point(midX, targetY + 20),
					new Point(trendEndX, targetY + 3),
					new Point(trendEndX, plotRect.Bottom),
					new Point(plotRect.Left, plotRect.Bottom)
				};
				using (Brush area = new SolidBrush(areaFillColor))
					g.FillPolygon(area, fillPoly);
			}
			if (objArgs.showForecastBand)
			{
				using (Brush b = new SolidBrush(forecastBandColor))
					g.FillRectangle(b, plotRect.Left, targetY - 4, plotRect.Width, 8);
			}

			using (Pen targetPen = new Pen(targetColor, Math.Max(1, objArgs.thick_target)))
				g.DrawLine(targetPen, plotRect.Left, targetY, plotRect.Right, targetY);

			if (objArgs.showStepLines)
			{
				// 런타임과 동일한 3색 (파랑/초록/주황) + 목표 위(높은 kW) 방향
				Color[] stepColors = new Color[]
				{
					Color.FromArgb(140, 64, 138, 196),
					Color.FromArgb(150, 94, 164, 51),
					Color.FromArgb(150, 230, 126, 34)
				};
				int stepSpacing = Math.Max(6, plotRect.Height / 14);
				for (int si = 0; si < 3; si++)
				{
					int stepY = targetY - stepSpacing * (si + 1);
					if (stepY < plotRect.Top) break;
					using (Pen sp = new Pen(stepColors[si], 1f))
					{
						sp.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
						g.DrawLine(sp, plotRect.Left, stepY, plotRect.Right, stepY);
					}
				}
			}
			if (objArgs.showTimeZoneTarget)
			{
				using (Pen tz = new Pen(Color.FromArgb(180, 156, 39, 176), 1f))
				{
					tz.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
					g.DrawLine(tz, plotRect.Left, targetY - 14, plotRect.Right, targetY - 14);
				}
			}

			using (Pen trendPen = new Pen(predictionColor, Math.Max(1, objArgs.lineThicknessTrend)))
			{
				g.DrawLine(trendPen, plotRect.Left, plotRect.Bottom - 8, midX, targetY + 20);
				g.DrawLine(trendPen, midX, targetY + 20, trendEndX, targetY + 3);
			}
			using (Pen forecastPen = new Pen(excessColor, Math.Max(1, objArgs.lineThicknessForecast)))
			{
				forecastPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
				g.DrawLine(forecastPen, trendEndX, targetY + 3, plotRect.Right, targetY - 12);
			}
			if (objArgs.showPeakLine)
			{
				using (Pen peakPen = new Pen(peakColor, 1f))
				{
					peakPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
					g.DrawLine(peakPen, plotRect.Left, targetY - 24, plotRect.Right, targetY - 24);
				}
			}

			DrawPreviewLegend(g, font, plotRect, x2 - margin, textColor, targetValueColor, targetColor, cardBackColor, cardBorderColor, objArgs.showStepLines);

			if (objArgs.nStatusBarPos != 2)
			{
				string[] labels = Tools.IsLangKorean()
					? new[] { "목표", "예측", "현재", "경과", "차단", "모드" }
					: new[] { "Target", "Forecast", "Current", "Elapsed", "Shed", "Mode" };
				string[] values = new[]
				{
					String.Format("{0}kW", maxKW - 20),
					String.Format("{0}kW", maxKW - 8),
					String.Format("{0:F1}kW", (maxKW - 28.5)),
					"06:20",
					"2",
					"Active"
				};
				Color[] valueColors = new[]
				{
					targetValueColor, excessColor, DemandChartThemeHelper.ApplyStatus(objArgs.lColorStatusValue, theme),
					DemandChartThemeHelper.ApplyStatus(objArgs.lColorStatusValue, theme), excessColor, targetColor
				};

				int count = 6;
				if (objArgs.nStatusBarPos == 1)
				{
					// 런타임 DrawKpiCards와 동일한 비례 간격
					int gap = Math.Max(2, kpiRect.Height / 80);
					int cardH = Math.Max(14, (kpiRect.Height - gap * (count - 1)) / count);
					for (int i = 0; i < count; i++)
					{
						Rectangle rc = new Rectangle(kpiRect.Left, kpiRect.Top + i * (cardH + gap), kpiRect.Width, cardH);
						if (rc.Bottom > kpiRect.Bottom) break;
						DrawPreviewKpiCard(g, font, rc, labels[i], values[i], titleColor, valueColors[i], cardBackColor, cardBorderColor);
					}
				}
				else
				{
					int gap = Math.Max(2, kpiRect.Width / 120);
					int cardW = Math.Max(16, (kpiRect.Width - gap * (count - 1)) / count);
					for (int i = 0; i < count; i++)
					{
						Rectangle rc = new Rectangle(kpiRect.Left + i * (cardW + gap), kpiRect.Top, cardW, kpiRect.Height);
						if (rc.Right > kpiRect.Right) break;
						DrawPreviewKpiCard(g, font, rc, labels[i], values[i], titleColor, valueColors[i], cardBackColor, cardBorderColor);
					}
				}
			}

			g.Clip = oldClip;
		}

		private int GetPreviewYStep(double maxKw, int graphHeight, bool detailedAxis)
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

		private int GetPreviewXMinuteStep(int totalMin, int graphWidth, bool detailedAxis)
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

		private Size GetPreviewLegendSize(Graphics g, Font font, bool showStepLines)
		{
			string[] texts = showStepLines
				? new[] { "Target", "S1 +10kW", "S2 +20kW", "S3 +30kW" }
				: new[] { "Target" };

			Font legendFont = new Font(font, FontStyle.Bold);
			int lineH = (int)Math.Ceiling(legendFont.GetHeight(g)) + 2;
			int maxTextW = 0;
			for (int i = 0; i < texts.Length; i++)
			{
				Size s = TextRenderer.MeasureText(texts[i], legendFont);
				if (s.Width > maxTextW) maxTextW = s.Width;
			}
			legendFont.Dispose();
			int pad = 6;
			int markerW = 14;
			int legendW = pad + markerW + 6 + maxTextW + pad;
			int legendH = pad + texts.Length * lineH + pad;
			return new Size(Math.Max(48, legendW), Math.Max(24, legendH));
		}

		private void DrawPreviewLegend(Graphics g, Font font, Rectangle plotRect, int rightLimit, Color textColor, Color targetValueColor, Color targetColor, Color cardBackColor, Color cardBorderColor, bool showStepLines)
		{
			string[] texts = showStepLines
				? new[] { "Target", "S1 +10kW", "S2 +20kW", "S3 +30kW" }
				: new[] { "Target" };
			Color[] colors = showStepLines
				? new[] { targetValueColor, Color.FromArgb(140, 64, 138, 196), Color.FromArgb(150, 94, 164, 51), Color.FromArgb(150, 230, 126, 34) }
				: new[] { targetColor };

			Font legendFont = new Font(font, FontStyle.Bold);
			Size legendSize = GetPreviewLegendSize(g, font, showStepLines);
			int lineH = (int)Math.Ceiling(legendFont.GetHeight(g)) + 2;
			int lx = plotRect.Right + 6;
			int ly = plotRect.Top + 6;
			if (lx + legendSize.Width > rightLimit)
				lx = rightLimit - legendSize.Width;

			Rectangle panel = new Rectangle(lx, ly, legendSize.Width, legendSize.Height);
			using (Brush back = new SolidBrush(Color.FromArgb(190, cardBackColor)))
				g.FillRectangle(back, panel);
			using (Pen border = new Pen(cardBorderColor))
				g.DrawRectangle(border, panel);

			int pad = 6;
			int markerW = 14;
			for (int i = 0; i < texts.Length; i++)
			{
				int y = ly + pad + i * lineH + lineH / 2;
				using (Pen p = new Pen(colors[i], 2.2f))
				{
					if (i > 0) p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
					g.DrawLine(p, lx + pad, y, lx + pad + markerW, y);
				}
				using (Brush b = new SolidBrush(Color.Black))
					g.DrawString(texts[i], legendFont, b, lx + pad + markerW + 6, ly + pad + i * lineH - 1);
			}
			legendFont.Dispose();
		}

		private void DrawPreviewKpiCard(Graphics g, Font font, Rectangle rc, string label, string value, Color titleColor, Color valueColor, Color cardBackColor, Color cardBorderColor)
		{
			Color cardBack = Color.FromArgb((int)(255.0 * objArgs.kpiOpacity / 100.0), cardBackColor);
			using (GraphicsPath path = CreatePreviewRoundRect(rc, Math.Max(0, Math.Min(16, objArgs.cornerRadius))))
			{
				using (Brush b = new SolidBrush(cardBack))
					g.FillPath(b, path);
				using (Pen p = new Pen(cardBorderColor, 1f))
					g.DrawPath(p, path);
			}

			// 실제 폰트 높이 기반 레이아웃 (런타임 DrawCard와 동일)
			int fH = (int)Math.Ceiling(font.GetHeight(g));
			int totalTextH = fH * 2;
			int gap = Math.Max(0, Math.Min(2, (rc.Height - totalTextH) / 3));
			int topPad = Math.Max(1, (rc.Height - totalTextH - gap) / 2);
			int hPad = Math.Max(2, Math.Min(6, rc.Width / 16));
			int textW = Math.Max(1, rc.Width - hPad * 2);

			Rectangle labelRect = new Rectangle(rc.Left + hPad, rc.Top + topPad, textW, fH);
			Rectangle valueRect = new Rectangle(rc.Left + hPad, rc.Top + topPad + fH + gap, textW, fH);

			using (Brush lb = new SolidBrush(titleColor))
			using (Brush vb = new SolidBrush(valueColor))
			using (StringFormat sf = new StringFormat())
			{
				sf.Alignment = StringAlignment.Near;
				sf.LineAlignment = StringAlignment.Center;
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags = StringFormatFlags.NoWrap;
				g.DrawString(label, font, lb, labelRect, sf);
				g.DrawString(value, font, vb, valueRect, sf);
			}
		}

		private GraphicsPath CreatePreviewRoundRect(Rectangle rc, int radius)
		{
			GraphicsPath p = new GraphicsPath();
			if (radius <= 0)
			{
				p.AddRectangle(rc);
				return p;
			}

			int d = radius * 2;
			p.AddArc(rc.Left, rc.Top, d, d, 180, 90);
			p.AddArc(rc.Right - d, rc.Top, d, d, 270, 90);
			p.AddArc(rc.Right - d, rc.Bottom - d, d, d, 0, 90);
			p.AddArc(rc.Left, rc.Bottom - d, d, d, 90, 90);
			p.CloseFigure();
			return p;
		}
	}
}


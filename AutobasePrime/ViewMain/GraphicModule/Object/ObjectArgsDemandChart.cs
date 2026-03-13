using System;
using System.Drawing;
using NetTools;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsDemandChart
	{
		public string demand_block_id = "";
		public int thick_target = 1;
		public int nStatusBarPos = 0;      // 0=상단, 1=좌측, 2=숨김
		public int nPercentY = 120;
		public int uiTheme = 0;             // 0=Light Minimal, 1=Dark Ops, 2=High Contrast
		public bool showAreaFill = true;
		public bool showPeakLine = true;
		public bool showForecastBand = true;
		public bool showGridLabels = true;
		public bool showStepLines = true;
		public bool showTimeZoneTarget = true;
		public bool showDetailedAxis = true;
		public int gridIntensity = 45;      // 0~100
		public int lineThicknessTrend = 2;  // 1~5
		public int lineThicknessForecast = 1; // 1~5
		public int kpiDensity = 0;          // 0=compact, 1=detailed
		public int cornerRadius = 8;        // 0~16
		public int kpiOpacity = 82;         // 30~100

		public BrushPublic lColorFill = new BrushPublic();
		public Color lColorText = Color.FromArgb(36, 44, 58);
		public BrushPublic lColorBack = new BrushPublic();
		public Color lColorGuideLine = Color.FromArgb(208, 215, 224);
		public Color lColorPrediction = Color.FromArgb(0, 122, 204);
		public Color lColorExcess = Color.FromArgb(217, 72, 15);
		public Color lColorTarget = Color.FromArgb(32, 167, 86);
		public Color lColorStatusFill = Color.FromArgb(51, 78, 104);
		public Color lColorStatusBack = Color.FromArgb(236, 241, 246);
		public Color lColorStatusTitle = Color.FromArgb(36, 44, 58);
		public Color lColorStatusValue = Color.FromArgb(18, 33, 48);
		public Color lColorTargetValue = Color.FromArgb(32, 167, 86);
		public Color lColorPreValue = Color.FromArgb(0, 122, 204);
		public Color lColorExValue = Color.FromArgb(217, 72, 15);

		public Color lColorCardBack = Color.FromArgb(245, 248, 252);
		public Color lColorCardBorder = Color.FromArgb(206, 216, 227);
		public Color lColorAreaFill = Color.FromArgb(48, 255, 141, 64);
		public Color lColorForecastBand = Color.FromArgb(48, 30, 136, 229);
		public Color lColorPeakLine = Color.FromArgb(255, 255, 152, 0);
	}
}

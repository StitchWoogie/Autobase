using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using AutoLibLocal;
using NetTools.OldDefine;
using NetTools;

namespace GraphicModule
{
    #region Enums

    /// <summary>
    /// Chart 시리즈 표시 타입
    /// </summary>
    public enum EnumChartSeriesType
    {
        Line = 0,
        FastLine = 1,
        Spline = 2,
        Area = 3,
        Column = 4,
        StepLine = 5,
        Pie = 6,         // CustomTrend 전용
        Doughnut = 7,    // CustomTrend 전용
        Radar = 8        // CustomTrend 전용
    }

    #endregion

    #region Member Structs

    /// <summary>
    /// Chart 태그 멤버 구조체 (ANALOG_GRAPH_MEMBER 패턴 기반)
    /// </summary>
    [Serializable]
    public class CHART_TAG_MEMBER
    {
        public string tag;
        public EnumTagType nType;
        public int[] nPos;
        public Color color;
        public int nSeriesType;        // EnumChartSeriesType
        public int nLineThick = 2;
        public int nPointType;
        public int nAxisPosition;      // 0=없음, 1=좌측축, 2=우측축
        public int nLevelFrom;         // Y축 범위 시작 (0-100%)
        public int nLevelTo = 100;     // Y축 범위 끝 (0-100%)

        public double min_value;
        public double max_value;
        public double view_full;
        public double view_base;
        public sbyte visible = 1;      // ON=표시, OFF=숨김
        public sbyte bReverseY;
        public int nValueType;         // 0=AVE, 1=MIN, 2=MAX, 3=SUM, 4=SUB, 5=MOMENT

        // 비직렬화 런타임 데이터
        [NonSerialized]
        public ONE_POINT_STRUCT[] point;

        // MilliData용 컬럼 정보
        public string column;
        [NonSerialized]
        public int column_pos = -1;

        public int nTagDisplaySize = 10;
        public ushort wFlags = 0;      // Bit 0=hihi, 1=high, 2=low, 3=lolo
        public int nTimeShift = 0;

        /// <summary>
        /// ANALOG_GRAPH_MEMBER → CHART_TAG_MEMBER 변환
        /// PropertyPageGraphMember가 ANALOG_GRAPH_MEMBER를 반환하므로 변환 필요
        /// </summary>
        public static CHART_TAG_MEMBER FromAnalogGraphMember(ANALOG_GRAPH_MEMBER src)
        {
            if (src == null) return null;
            var dst = new CHART_TAG_MEMBER();
            dst.tag = src.tag;
            dst.nType = src.nType;
            dst.nPos = src.nPos;
            dst.color = src.color;
            dst.nSeriesType = src.nGraphType;   // nGraphType → nSeriesType
            dst.nLineThick = src.nLineThick;
            dst.nPointType = src.nPointType;
            dst.nAxisPosition = src.nAxisPosition;
            dst.nLevelFrom = src.nLevelFrom;
            dst.nLevelTo = src.nLevelTo;
            dst.min_value = src.min_value;
            dst.max_value = src.max_value;
            dst.view_full = src.view_full;
            dst.view_base = src.view_base;
            dst.visible = src.visible;
            dst.bReverseY = src.bReverseY;
            dst.nValueType = src.nValueType;
            dst.nTagDisplaySize = src.nTagDisplaySize;
            dst.wFlags = src.wFlags;
            dst.nTimeShift = src.nTimeShift;
            return dst;
        }

        /// <summary>
        /// CHART_TAG_MEMBER → ANALOG_GRAPH_MEMBER 변환
        /// PropertyPageGraphMember에 전달하기 위한 변환
        /// </summary>
        public ANALOG_GRAPH_MEMBER ToAnalogGraphMember()
        {
            var dst = new ANALOG_GRAPH_MEMBER();
            dst.tag = this.tag;
            dst.nType = this.nType;
            dst.nPos = this.nPos;
            dst.color = this.color;
            dst.nGraphType = this.nSeriesType;  // nSeriesType → nGraphType
            dst.nLineThick = this.nLineThick;
            dst.nPointType = this.nPointType;
            dst.nAxisPosition = this.nAxisPosition;
            dst.nLevelFrom = this.nLevelFrom;
            dst.nLevelTo = this.nLevelTo;
            dst.min_value = this.min_value;
            dst.max_value = this.max_value;
            dst.view_full = this.view_full;
            dst.view_base = this.view_base;
            dst.visible = this.visible;
            dst.bReverseY = this.bReverseY;
            dst.nValueType = this.nValueType;
            dst.nTagDisplaySize = this.nTagDisplaySize;
            dst.wFlags = this.wFlags;
            dst.nTimeShift = this.nTimeShift;
            return dst;
        }
    }

    /// <summary>
    /// CustomTrend 사용자 지정 데이터 포인트
    /// </summary>
    [Serializable]
    public class CHART_CUSTOM_POINT
    {
        public string label;
        public double value;
        public Color color;
        public bool visible = true;
    }

    #endregion

    #region Args Classes

    /// <summary>
    /// Chart 공통 설정 (모든 Chart 타입에서 공유)
    /// </summary>
    [Serializable]
    public class ObjectArgsChartCommon
    {
        public int nDefaultSeriesType;  // EnumChartSeriesType 기본값

        public bool bShowLegend = true;
        public bool bShowGrid = true;
        public bool bAntiAlias = true;
        public bool b3DStyle = false;
        public string sTitle = "";

        public Color colorChartBack = Color.White;
        public Color colorPlotBack = Color.White;
        public Color colorGrid = Color.FromArgb(200, 200, 200);
        public Color colorTitle = Color.Black;
        public Color colorLegendText = Color.Black;
        public Color colorLegendBack = Color.FromArgb(245, 245, 245);

        public int nTitleSize = 12;
        public int nLegendPosition;    // 0=Bottom, 1=Right, 2=Top, 3=Left
        public int nAxisXLabelAngle;   // X축 라벨 각도

        public bool bUseToolBar;
        public int nToolBarPos = 0;
        public int nToolBarBtnColor = 0;
        public int nToolBarButtonSize = 16;
        public int nToolBarTextSize = 9;
        public bool bHideLabelDataRange;

        public bool bDontUseConfigDialog;
        public bool bUseMouseButtonAsZoom = true;
        public bool bUseCrosshair = true;            // 크로스헤어 활성화
        public bool bUseTagDescription = false;      // 태그 설명으로 범례 표시

        public ScriptClass scriptEventAfterSettings;
    }

    /// <summary>
    /// ChartRealTimeTrend 설정
    /// </summary>
    [Serializable]
    public class ObjectArgsChartRealTime
    {
        public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();
        public ObjectArgsChartCommon chart = new ObjectArgsChartCommon();

        public BrushPublic lColorBack = new BrushPublic();
        public Color lColorText;
        public BrushPublic lColorFill = new BrushPublic();
        public Color lColorGuideLine;

        private int _wShowUnit;
        public int wShowUnit
        {
            get => _wShowUnit;
            set => _wShowUnit = Math.Max(10, Math.Min(36000, value));
        }

        private int _nDataTime;
        public int nDataTime
        {
            get => _nDataTime;
            set => _nDataTime = Math.Max(100, Math.Min(60000, value));
        }

        private int _wLevelDevide;
        public int wLevelDevide
        {
            get => _wLevelDevide;
            set => _wLevelDevide = Math.Max(2, Math.Min(100, value));
        }

        private int _wTimeDevide;
        public int wTimeDevide
        {
            get => _wTimeDevide;
            set => _wTimeDevide = Math.Max(1, Math.Min(10000, value));
        }

        public sbyte bTimeDirToLeft;
        public sbyte bDisplayByTime = 1;

        public LogarithmicScale logarithmicScale = new LogarithmicScale();
    }

    /// <summary>
    /// ChartMinuteTrend 설정
    /// </summary>
    [Serializable]
    public class ObjectArgsChartMinute
    {
        public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();
        public ObjectArgsChartCommon chart = new ObjectArgsChartCommon();

        public BrushPublic lColorBack = new BrushPublic();
        public Color lColorText;
        public BrushPublic lColorFill = new BrushPublic();
        public Color lColorGuideLine;

        public int wTimeSelectOption;  // 0=분, 1=시, 2=일, 3=월

        private int _wShowUnit;
        public int wShowUnit
        {
            get => _wShowUnit;
            set => _wShowUnit = Math.Max(2, Math.Min(44640, value));
        }

        private int _nDataCycle;
        public int nDataCycle
        {
            get => _nDataCycle;
            set => _nDataCycle = Math.Max(1, Math.Min(1000, value));
        }

        private int _wLevelDevide;
        public int wLevelDevide
        {
            get => _wLevelDevide;
            set => _wLevelDevide = Math.Max(2, Math.Min(100, value));
        }

        private int _wTimeDivide;
        public int wTimeDivide
        {
            get => _wTimeDivide;
            set => _wTimeDivide = Math.Max(1, Math.Min(10000, value));
        }

        public LogarithmicScale logarithmicScale = new LogarithmicScale();
    }

    /// <summary>
    /// ChartMilliTrend 설정
    /// </summary>
    [Serializable]
    public class ObjectArgsChartMilli
    {
        public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();
        public ObjectArgsChartCommon chart = new ObjectArgsChartCommon();

        public BrushPublic lColorBack = new BrushPublic();
        public Color lColorText;
        public BrushPublic lColorFill = new BrushPublic();
        public Color lColorGuideLine;

        public string sDsn;  // MilliData 데이터 소스명

        public int wTimeSelectOption;  // 0=ms, 1=sec, 2=min, 3=hr, 4=day, 5=month, 6=year

        private int _wShowUnit;
        public int wShowUnit
        {
            get => _wShowUnit;
            set => _wShowUnit = Math.Max(2, Math.Min(44640, value));
        }

        private int _nDataCycle;
        public int nDataCycle
        {
            get => _nDataCycle;
            set => _nDataCycle = Math.Max(1, Math.Min(1000, value));
        }

        private int _wLevelDevide;
        public int wLevelDevide
        {
            get => _wLevelDevide;
            set => _wLevelDevide = Math.Max(2, Math.Min(100, value));
        }

        private int _wTimeDivide;
        public int wTimeDivide
        {
            get => _wTimeDivide;
            set => _wTimeDivide = Math.Max(1, Math.Min(10000, value));
        }

        public bool bAutoUpdate = false;
        public LogarithmicScale logarithmicScale = new LogarithmicScale();
    }

    /// <summary>
    /// CustomChart 설정
    /// </summary>
    [Serializable]
    public class ObjectArgsChartCustom
    {
        public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();
        public ObjectArgsChartCommon chart = new ObjectArgsChartCommon();

        public BrushPublic lColorBack = new BrushPublic();
        public Color lColorText;
        public BrushPublic lColorFill = new BrushPublic();
        public Color lColorGuideLine;

        private int _wShowUnit;
        public int wShowUnit
        {
            get => _wShowUnit;
            set => _wShowUnit = Math.Max(2, Math.Min(44640, value));
        }

        private int _wLevelDevide;
        public int wLevelDevide
        {
            get => _wLevelDevide;
            set => _wLevelDevide = Math.Max(2, Math.Min(100, value));
        }

        private int _wTimeDivide;
        public int wTimeDivide
        {
            get => _wTimeDivide;
            set => _wTimeDivide = Math.Max(1, Math.Min(10000, value));
        }

        private int _nDataTime = 1000;
        public int nDataTime
        {
            get => _nDataTime;
            set => _nDataTime = Math.Max(100, Math.Min(60000, value));
        }

        public bool bSupportPie = true;
        public bool bSupportDoughnut = true;
        public bool bSupportRadar = true;

        public LogarithmicScale logarithmicScale = new LogarithmicScale();
    }

    #endregion

    #region ObjectChartBase

    /// <summary>
    /// Chart 컨트롤 기반 그래프 오브젝트 공통 기반 클래스.
    /// System.Windows.Forms.DataVisualization.Charting.Chart를 사용하여 렌더링.
    /// </summary>
    [Serializable]
    public class ObjectChartBase : ObjectExpand
    {
        //=== 정적 필드 ===
        // (각 서브 클래스에서 자체 arrayClassList를 정의)

        //=== 인스턴스 필드 ===
        [NonSerialized]
        protected Form formParent;

        [NonSerialized]
        protected Chart chartControl;

        [NonSerialized]
        protected Bitmap cachedChartBitmap;

        [NonSerialized]
        protected bool needsChartRedraw = true;

        [NonSerialized]
        protected DateTime lastCacheTime = DateTime.MinValue;

        [NonSerialized]
        protected Size lastCacheSize = Size.Empty;

        // Lock 객체: NonSerialized + readonly 조합은 역직렬화 후 null이 되므로
        // 프로퍼티로 접근하여 지연 초기화
        [NonSerialized]
        private object _chartLockField;
        protected object _chartLock
        {
            get
            {
                if (_chartLockField == null)
                    _chartLockField = new object();
                return _chartLockField;
            }
        }

        [NonSerialized]
        private object _dataLockField;
        protected object _dataLock
        {
            get
            {
                if (_dataLockField == null)
                    _dataLockField = new object();
                return _dataLockField;
            }
        }

        [NonSerialized]
        private object _loadingLockField;
        protected object _loadingLock
        {
            get
            {
                if (_loadingLockField == null)
                    _loadingLockField = new object();
                return _loadingLockField;
            }
        }

        [NonSerialized]
        protected volatile bool _isLoading = false;

        [NonSerialized]
        protected int _loadProgress = 0;

        protected ArrayList blockMember;

        // 공통 접근 속성
        public ArrayList ChartMember
        {
            get => blockMember;
            set
            {
                if (value == null)
                {
                    blockMember = new ArrayList();
                    return;
                }

                // PropertyPageGraphMember가 ANALOG_GRAPH_MEMBER를 반환하므로 변환 필요
                blockMember = ConvertMemberList(value);
                SetBlock(blockMember);

                // Studio 편집 모드에서 속성 변경 시 차트 미리보기 갱신
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                {
                    OnChartMemberChanged();
                }
            }
        }

        /// <summary>
        /// Studio 편집 모드에서 ChartMember가 변경되었을 때 호출.
        /// 서브클래스에서 override하여 Pie/Doughnut/Radar 등 특수 타입 처리 가능.
        /// </summary>
        protected virtual void OnChartMemberChanged()
        {
            if (chartControl == null) InitializeChart();
            RebuildSeries();
            FillSampleData(GetShowUnit());
            InvalidateChartCache();
        }

        /// <summary>
        /// ArrayList의 ANALOG_GRAPH_MEMBER 항목을 CHART_TAG_MEMBER로 변환.
        /// 이미 CHART_TAG_MEMBER인 항목은 그대로 유지.
        /// </summary>
        protected ArrayList ConvertMemberList(ArrayList source)
        {
            if (source == null) return new ArrayList();

            ArrayList result = new ArrayList(source.Count);
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] is CHART_TAG_MEMBER chartMember)
                {
                    result.Add(chartMember);
                }
                else if (source[i] is ANALOG_GRAPH_MEMBER analogMember)
                {
                    result.Add(CHART_TAG_MEMBER.FromAnalogGraphMember(analogMember));
                }
            }
            return result;
        }

        /// <summary>
        /// ChartMember를 PropertyPageGraphMember용 ANALOG_GRAPH_MEMBER ArrayList로 변환
        /// </summary>
        public ArrayList ChartMemberAsAnalog
        {
            get
            {
                ArrayList result = new ArrayList(blockMember.Count);
                for (int i = 0; i < blockMember.Count; i++)
                {
                    if (blockMember[i] is CHART_TAG_MEMBER chartMember)
                    {
                        result.Add(chartMember.ToAnalogGraphMember());
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 현재 ShowUnit 값 반환 (각 서브클래스에서 오버라이드 가능)
        /// </summary>
        protected virtual int GetShowUnit()
        {
            return 100;
        }

        //=== 생성자 ===
        public ObjectChartBase(ObjectCommonProperty ocp, RECT rect,
            EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general)
            : base(ocp, rect, eid, lf, general)
        {
            blockMember = new ArrayList();
        }

        //=== Chart 초기화 ===
        protected virtual void InitializeChart()
        {
            try
            {
                if (chartControl != null)
                {
                    chartControl.Dispose();
                    chartControl = null;
                }

                chartControl = new Chart();
                chartControl.Size = new Size(
                    Math.Max(100, nRight - nLeft),
                    Math.Max(100, nBottom - nTop));

                // 기본 ChartArea 설정
                var chartArea = new ChartArea("MainArea");
                chartArea.BackColor = Color.White;
                chartArea.BorderColor = Color.Transparent;
                chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(200, 200, 200);
                chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(200, 200, 200);
                chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                chartArea.AxisX.LabelStyle.Font = new Font("맑은 고딕", 8f);
                chartArea.AxisY.LabelStyle.Font = new Font("맑은 고딕", 8f);
                chartArea.AxisX.LineColor = Color.DimGray;
                chartArea.AxisY.LineColor = Color.DimGray;
                chartArea.AxisX.TitleFont = new Font("맑은 고딕", 9f);
                chartArea.AxisY.TitleFont = new Font("맑은 고딕", 9f);
                chartArea.Position = new ElementPosition(3, 5, 94, 80);

                chartControl.ChartAreas.Add(chartArea);

                // 범례 설정
                var legend = new Legend("MainLegend");
                legend.Docking = Docking.Bottom;
                legend.Alignment = StringAlignment.Center;
                legend.Font = new Font("맑은 고딕", 8f);
                legend.BackColor = Color.Transparent;
                chartControl.Legends.Add(legend);

                chartControl.AntiAliasing = AntiAliasingStyles.All;
                chartControl.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObjectChartBase] InitializeChart error: {ex.Message}");
            }
        }

        //=== Chart 공통 설정 적용 ===
        protected virtual void ApplyChartCommonSettings(ObjectArgsChartCommon chartArgs)
        {
            if (chartControl == null || chartArgs == null) return;

            try
            {
                chartControl.BackColor = chartArgs.colorChartBack;

                if (chartControl.ChartAreas.Count > 0)
                {
                    var area = chartControl.ChartAreas[0];
                    area.BackColor = chartArgs.colorPlotBack;
                    area.AxisX.MajorGrid.LineColor = chartArgs.colorGrid;
                    area.AxisY.MajorGrid.LineColor = chartArgs.colorGrid;
                    area.AxisX.MajorGrid.Enabled = chartArgs.bShowGrid;
                    area.AxisY.MajorGrid.Enabled = chartArgs.bShowGrid;
                    // X축/Y축 라벨·눈금 글자색 = 제목색과 동일
                    area.AxisX.LabelStyle.ForeColor = chartArgs.colorTitle;
                    area.AxisY.LabelStyle.ForeColor = chartArgs.colorTitle;
                    area.AxisX.TitleForeColor = chartArgs.colorTitle;
                    area.AxisY.TitleForeColor = chartArgs.colorTitle;
                    area.AxisX.LineColor = chartArgs.colorGrid;
                    area.AxisY.LineColor = chartArgs.colorGrid;
                }

                // 제목
                chartControl.Titles.Clear();
                if (!string.IsNullOrEmpty(chartArgs.sTitle))
                {
                    var title = new Title(chartArgs.sTitle);
                    title.Font = new Font("맑은 고딕", chartArgs.nTitleSize, FontStyle.Bold);
                    title.ForeColor = chartArgs.colorTitle;
                    title.Docking = Docking.Top;
                    chartControl.Titles.Add(title);
                }

                // 범례
                if (chartControl.Legends.Count > 0)
                {
                    var legend = chartControl.Legends[0];
                    legend.Enabled = chartArgs.bShowLegend;
                    legend.ForeColor = chartArgs.colorLegendText;
                    legend.BackColor = chartArgs.colorLegendBack;

                    switch (chartArgs.nLegendPosition)
                    {
                        case 0: legend.Docking = Docking.Bottom; break;
                        case 1: legend.Docking = Docking.Right; break;
                        case 2: legend.Docking = Docking.Top; break;
                        case 3: legend.Docking = Docking.Left; break;
                    }
                }

                // 3D 스타일
                if (chartControl.ChartAreas.Count > 0)
                {
                    chartControl.ChartAreas[0].Area3DStyle.Enable3D = chartArgs.b3DStyle;
                    if (chartArgs.b3DStyle)
                    {
                        chartControl.ChartAreas[0].Area3DStyle.Inclination = 15;
                        chartControl.ChartAreas[0].Area3DStyle.Rotation = 10;
                        chartControl.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Realistic;
                    }
                }

                // 안티앨리어싱
                chartControl.AntiAliasing = chartArgs.bAntiAlias
                    ? AntiAliasingStyles.All
                    : AntiAliasingStyles.None;

                // 기본 시리즈 타입을 모든 시리즈에 적용
                SeriesChartType defaultType = ConvertSeriesType(chartArgs.nDefaultSeriesType);
                foreach (Series s in chartControl.Series)
                {
                    s.ChartType = defaultType;

                    // Area 타입의 경우 반투명 채우기
                    if (chartArgs.nDefaultSeriesType == (int)EnumChartSeriesType.Area)
                    {
                        s.Color = Color.FromArgb(128, s.Color.R, s.Color.G, s.Color.B);
                        s.BorderColor = Color.FromArgb(255, s.Color.R, s.Color.G, s.Color.B);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObjectChartBase] ApplyChartCommonSettings error: {ex.Message}");
            }
        }

        //=== Chart 시리즈 타입 변환 ===
        protected static SeriesChartType ConvertSeriesType(int seriesType)
        {
            switch (seriesType)
            {
                case 0: return SeriesChartType.Line;
                case 1: return SeriesChartType.FastLine;
                case 2: return SeriesChartType.Spline;
                case 3: return SeriesChartType.Area;
                case 4: return SeriesChartType.Column;
                case 5: return SeriesChartType.StepLine;
                case 6: return SeriesChartType.Pie;
                case 7: return SeriesChartType.Doughnut;
                case 8: return SeriesChartType.Radar;
                default: return SeriesChartType.Line;
            }
        }

        protected static SeriesChartType ConvertSeriesType(EnumChartSeriesType type)
        {
            return ConvertSeriesType((int)type);
        }

        //=== 시리즈 생성 유틸리티 ===
        protected Series CreateSeries(string name, Color color, int seriesType, int lineThick)
        {
            var series = new Series(name);
            series.ChartType = ConvertSeriesType(seriesType);
            series.Color = color;
            series.BorderWidth = Math.Max(1, lineThick);
            series.ChartArea = "MainArea";
            series.Legend = "MainLegend";

            // Area 타입의 경우 반투명 채우기
            if (seriesType == (int)EnumChartSeriesType.Area)
            {
                series.Color = Color.FromArgb(128, color);
                series.BorderColor = color;
            }

            return series;
        }

        //=== Chart 비트맵 렌더링 ===
        protected Bitmap RenderChartToBitmap(int width, int height)
        {
            if (chartControl == null || width <= 0 || height <= 0) return null;

            Bitmap bmp = null;
            try
            {
                lock (_chartLock)
                {
                    chartControl.Size = new Size(width, height);

                    // 배경색이 투명이면 불투명 흰색으로 교체 (투명이면 비트맵이 안 보임)
                    if (chartControl.BackColor.A == 0)
                        chartControl.BackColor = Color.White;

                    // Chart 컨트롤에 윈도우 핸들 강제 생성 (DrawToBitmap 필수)
                    if (!chartControl.IsHandleCreated)
                    {
                        chartControl.CreateControl();
                    }

                    // 속성 변경 후 차트 레이아웃 강제 재계산
                    chartControl.PerformLayout();
                    chartControl.Invalidate();
                    chartControl.Update();

                    bmp = new Bitmap(width, height);

                    // DrawToBitmap 사용 (PrintPaint보다 오프스크린 컨트롤에서 안정적)
                    chartControl.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));

                    // DrawToBitmap 결과 확인: 비트맵이 완전히 비어있으면 PrintPaint 폴백
                    if (IsBitmapBlank(bmp, chartControl.BackColor))
                    {
                        using (Graphics gBmp = Graphics.FromImage(bmp))
                        {
                            gBmp.Clear(chartControl.BackColor);
                            chartControl.Printing.PrintPaint(gBmp, new Rectangle(0, 0, width, height));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObjectChartBase] RenderChartToBitmap error: {ex.Message}");
                bmp?.Dispose();
                bmp = null;
            }
            return bmp;
        }

        /// <summary>
        /// 비트맵이 단일 색상(배경색)으로만 채워져 있는지 검사.
        /// 차트 렌더링이 실패했는지 빠르게 판단하기 위함.
        /// </summary>
        private static bool IsBitmapBlank(Bitmap bmp, Color bgColor)
        {
            if (bmp == null) return true;
            // 모서리와 중앙 5개 픽셀만 빠르게 검사
            int w = bmp.Width, h = bmp.Height;
            Point[] checkPoints = new Point[]
            {
                new Point(w / 4, h / 4),
                new Point(3 * w / 4, h / 4),
                new Point(w / 2, h / 2),
                new Point(w / 4, 3 * h / 4),
                new Point(3 * w / 4, 3 * h / 4)
            };
            foreach (var pt in checkPoints)
            {
                if (pt.X < 0 || pt.X >= w || pt.Y < 0 || pt.Y >= h) continue;
                Color pixel = bmp.GetPixel(pt.X, pt.Y);
                if (pixel.ToArgb() != bgColor.ToArgb())
                    return false; // 배경색이 아닌 픽셀 발견 → 차트 내용 있음
            }
            return true; // 모든 검사 픽셀이 배경색 → 차트 내용 없음 (비어있음)
        }

        //=== 캐시 유효성 확인 ===
        protected bool IsCacheValid(int width, int height)
        {
            if (needsChartRedraw) return false;
            if (cachedChartBitmap == null) return false;
            if (lastCacheSize.Width != width || lastCacheSize.Height != height) return false;
            // 60초 후 캐시 자동 만료
            if (DateTime.Now.Subtract(lastCacheTime).TotalMilliseconds > 60000) return false;
            return true;
        }

        //=== 캐시 무효화 ===
        protected void InvalidateChartCache()
        {
            needsChartRedraw = true;
            lock (_chartLock)
            {
                cachedChartBitmap?.Dispose();
                cachedChartBitmap = null;
            }
        }

        //=== DisplayObject 공통 패턴 ===
        protected void DisplayChartObject(Graphics g, int x1, int y1, int x2, int y2)
        {
            int width = x2 - x1;
            int height = y2 - y1;
            if (width <= 0 || height <= 0) return;

            // 편집 모드에서 멤버가 없으면 안내 텍스트 표시
            if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN && !HasAnyData())
            {
                DrawEmptyGuide(g, x1, y1, x2, y2);
                return;
            }

            try
            {
                // chartControl이 null이면 초기화 시도
                if (chartControl == null)
                {
                    InitializeChart();
                    RebuildSeries();
                    if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                    {
                        FillSampleData(GetShowUnit());
                    }
                }

                if (!IsCacheValid(width, height))
                {
                    lock (_chartLock)
                    {
                        cachedChartBitmap?.Dispose();
                        cachedChartBitmap = RenderChartToBitmap(width, height);
                        needsChartRedraw = false;
                        lastCacheTime = DateTime.Now;
                        lastCacheSize = new Size(width, height);
                    }
                }

                Bitmap bmpToDraw;
                lock (_chartLock)
                {
                    bmpToDraw = cachedChartBitmap;
                }

                if (bmpToDraw != null)
                {
                    g.DrawImage(bmpToDraw, x1, y1);
                }
                else
                {
                    DrawEmptyGuide(g, x1, y1, x2, y2);
                }

                // 로딩 표시
                if (_isLoading)
                {
                    DrawLoadingIndicator(g, x1, y1, x2, y2);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObjectChartBase] DisplayChartObject error: {ex.Message}");
                DrawEmptyGuide(g, x1, y1, x2, y2);
            }
        }

        /// <summary>
        /// 데이터(멤버 또는 커스텀포인트)가 하나라도 있는지 확인
        /// </summary>
        protected virtual bool HasAnyData()
        {
            return blockMember != null && blockMember.Count > 0;
        }

        //=== 설정 전 안내 표시 ===
        protected void DrawEmptyGuide(Graphics g, int x1, int y1, int x2, int y2)
        {
            int width = x2 - x1;
            int height = y2 - y1;

            // 배경
            using (var bgBrush = new SolidBrush(Color.FromArgb(248, 249, 250)))
            {
                g.FillRectangle(bgBrush, x1, y1, width, height);
            }

            // 점선 테두리
            using (var borderPen = new Pen(Color.FromArgb(160, 160, 160), 1))
            {
                borderPen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(borderPen, x1 + 1, y1 + 1, width - 3, height - 3);
            }

            int cx = (x1 + x2) / 2;
            int cy = (y1 + y2) / 2;

            // 차트 아이콘 (간단한 막대 모양)
            int barWidth = Math.Max(3, width / 40);
            int barGap = Math.Max(2, barWidth / 2);
            int barCount = 5;
            int totalBarWidth = barCount * barWidth + (barCount - 1) * barGap;
            int bx = cx - totalBarWidth / 2;
            int maxBarH = Math.Max(15, height / 6);
            int[] barH = { 40, 70, 50, 90, 60 };

            using (var barBrush = new SolidBrush(Color.FromArgb(100, 70, 130, 180)))
            {
                for (int i = 0; i < barCount; i++)
                {
                    int bh = maxBarH * barH[i] / 100;
                    g.FillRectangle(barBrush, bx + i * (barWidth + barGap),
                        cy - maxBarH / 2 - 10 + (maxBarH - bh), barWidth, bh);
                }
            }

            // 타입명
            string typeName = enumObjectType.ToString();
            float titleSize = Math.Max(9, Math.Min(14, width / 35f));
            using (var titleBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
            using (var titleFont = new Font("맑은 고딕", titleSize, FontStyle.Bold))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(typeName, titleFont, titleBrush, cx, cy + maxBarH / 2, sf);
            }

            // 안내 메시지
            float guideSize = Math.Max(8, Math.Min(11, width / 45f));
            using (var guideBrush = new SolidBrush(Color.FromArgb(130, 130, 130)))
            using (var guideFont = new Font("맑은 고딕", guideSize, FontStyle.Regular))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString("더블클릭하여 태그/속성을 설정하세요", guideFont, guideBrush,
                    cx, cy + maxBarH / 2 + titleSize + 8, sf);
            }
        }

        //=== 로딩 인디케이터 ===
        protected void DrawLoadingIndicator(Graphics g, int x1, int y1, int x2, int y2)
        {
            int cx = (x1 + x2) / 2;
            int cy = (y1 + y2) / 2;

            // 반투명 오버레이
            using (var overlayBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255)))
            {
                g.FillRectangle(overlayBrush, x1, y1, x2 - x1, y2 - y1);
            }

            // 프로그레스 텍스트
            string loadText = _loadProgress > 0
                ? $"데이터 로딩중... {_loadProgress}%"
                : "데이터 로딩중...";

            using (var textBrush = new SolidBrush(Color.FromArgb(64, 64, 64)))
            using (var font = new Font("맑은 고딕", 10f, FontStyle.Bold))
            {
                var textSize = g.MeasureString(loadText, font);
                g.DrawString(loadText, font, textBrush,
                    cx - textSize.Width / 2, cy - textSize.Height / 2);
            }
        }

        //=== 태그 해석 유틸리티 (ObjectMultiTrend 패턴) ===
        protected TagAiClass GetRealTagAI(string tag, ref int[] tag_pos)
        {
            try
            {
                TagAiClass ai = TagLib.GetStructAI(tag, ref tag_pos);
                if (ai.cTagLinkType == 3)
                {
                    if (ai.assign != null && ai.assign.pos[0] != TagLib.TAG_NOT_FOUND)
                    {
                        TagAiClass ai2 = TagLib.GetStructAI(ai.assign.tag, ref ai.assign.pos);
                        return ai2;
                    }
                }
                return ai;
            }
            catch
            {
                return TagLib.GetStructAI(tag, ref tag_pos);
            }
        }

        protected TagDiClass GetRealTagDI(string tag, ref int[] tag_pos)
        {
            try
            {
                TagDiClass di = TagLib.GetStructDI(tag, ref tag_pos);
                if (di.cTagLinkType == 3)
                {
                    if (di.assign != null && di.assign.pos[0] != TagLib.TAG_NOT_FOUND)
                    {
                        TagDiClass di2 = TagLib.GetStructDI(di.assign.tag, ref di.assign.pos);
                        return di2;
                    }
                }
                return di;
            }
            catch
            {
                return TagLib.GetStructDI(tag, ref tag_pos);
            }
        }

        //=== SetBlock: 멤버 초기화 (ObjectMultiTrend 패턴) ===
        protected virtual void SetBlock(ArrayList block)
        {
            if (block == null)
            {
                blockMember = new ArrayList();
                return;
            }

            for (int i = 0; i < block.Count; i++)
            {
                CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)block[i];

                try
                {
                    TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos);
                }
                catch { }

                if (member.nLevelFrom < 0) member.nLevelFrom = 0;
                if (member.nLevelTo > 100) member.nLevelTo = 100;
                if (member.nLevelFrom >= member.nLevelTo)
                {
                    member.nLevelFrom = 0;
                    member.nLevelTo = 100;
                }

                if (member.visible == 0) member.visible = 1;

                if (member.nType == EnumTagType.AI)
                {
                    try
                    {
                        TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);
                        member.max_value = ai.view_full;
                        member.min_value = ai.view_base;
                        member.view_base = ai.view_base;
                        member.view_full = ai.view_full;
                    }
                    catch { }
                }
                else
                {
                    member.min_value = 0;
                    member.max_value = 100;
                }
            }
        }

        //=== 포인트 버퍼 할당 ===
        protected void MallocAllBuf(int showUnit)
        {
            for (int i = 0; i < blockMember.Count; i++)
            {
                CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                MallocOneBuf(member, showUnit);
            }
        }

        protected void MallocOneBuf(CHART_TAG_MEMBER member, int showUnit)
        {
            if (showUnit <= 0) showUnit = 100;
            member.point = new ONE_POINT_STRUCT[showUnit];
            for (int j = 0; j < showUnit; j++)
            {
                member.point[j].val = 0;
                member.point[j].read_flag = false;
            }
        }

        //=== 태그 추가/삭제 ===
        public virtual void AddTag(CHART_TAG_MEMBER member)
        {
            if (member == null) return;

            try
            {
                TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos);
            }
            catch { }

            if (member.nLevelFrom < 0) member.nLevelFrom = 0;
            if (member.nLevelTo > 100) member.nLevelTo = 100;
            if (member.nLevelFrom >= member.nLevelTo)
            {
                member.nLevelFrom = 0;
                member.nLevelTo = 100;
            }

            if (member.visible == 0) member.visible = 1;

            if (member.nType == EnumTagType.AI)
            {
                try
                {
                    TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);
                    member.max_value = ai.view_full;
                    member.min_value = ai.view_base;
                    member.view_base = ai.view_base;
                    member.view_full = ai.view_full;
                }
                catch { }
            }
            else
            {
                if (member.min_value == 0 && member.max_value == 0)
                {
                    member.min_value = 0;
                    member.max_value = 100;
                }
            }

            blockMember.Add(member);

            // 포인트 버퍼 할당
            int showUnit = GetShowUnit();
            MallocOneBuf(member, showUnit);

            // Chart Series 생성
            if (chartControl != null)
            {
                int defaultSeriesType = GetDefaultSeriesType();
                int idx = blockMember.Count - 1;
                string seriesName = !string.IsNullOrEmpty(member.tag)
                    ? member.tag : $"Series{idx}";

                lock (_chartLock)
                {
                    if (chartControl.Series.FindByName(seriesName) != null)
                    {
                        seriesName = $"{seriesName}_{idx}";
                    }

                    var series = CreateSeries(seriesName, member.color,
                        defaultSeriesType, member.nLineThick);
                    series.Enabled = member.visible == 1;
                    chartControl.Series.Add(series);
                }
            }

            // 편집 모드 샘플 데이터
            if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
            {
                FillSampleData(showUnit);
            }

            InvalidateChartCache();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        public virtual void DeleteTag(string tagName)
        {
            for (int i = blockMember.Count - 1; i >= 0; i--)
            {
                CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                if (member.tag == tagName)
                {
                    blockMember.RemoveAt(i);

                    // 대응하는 Chart Series 제거
                    if (chartControl != null)
                    {
                        lock (_chartLock)
                        {
                            if (i < chartControl.Series.Count)
                            {
                                chartControl.Series.RemoveAt(i);
                            }
                        }
                    }
                    break;
                }
            }
            InvalidateChartCache();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        //=== 시리즈 타입 변경 ===
        public virtual void SetSeriesType(int seriesType)
        {
            if (chartControl == null) return;
            lock (_chartLock)
            {
                foreach (var series in chartControl.Series)
                {
                    series.ChartType = ConvertSeriesType(seriesType);
                }
            }
            InvalidateChartCache();
        }

        public virtual void SetSeriesType(string tagName, int seriesType)
        {
            for (int i = 0; i < blockMember.Count; i++)
            {
                CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                if (member.tag == tagName)
                {
                    member.nSeriesType = seriesType;
                    if (chartControl != null && i < chartControl.Series.Count)
                    {
                        lock (_chartLock)
                        {
                            chartControl.Series[i].ChartType = ConvertSeriesType(seriesType);
                        }
                    }
                    break;
                }
            }
            InvalidateChartCache();
        }

        //=== 멤버 가시성 설정 ===
        public virtual void SetVisible(string tagName, bool visible)
        {
            for (int i = 0; i < blockMember.Count; i++)
            {
                CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                if (member.tag == tagName)
                {
                    member.visible = visible ? (sbyte)1 : (sbyte)0;
                    if (chartControl != null && i < chartControl.Series.Count)
                    {
                        lock (_chartLock)
                        {
                            chartControl.Series[i].Enabled = visible;
                        }
                    }
                    break;
                }
            }
            InvalidateChartCache();
        }

        //=== 배경색 설정 ===
        public virtual void SetBackColor(Color color)
        {
            if (chartControl != null)
            {
                chartControl.BackColor = color;
            }
            InvalidateChartCache();
        }

        //=== Y축 범위 설정 ===
        public virtual void SetBasicLevel(string tagName, double min, double max)
        {
            for (int i = 0; i < blockMember.Count; i++)
            {
                CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                if (member.tag == tagName)
                {
                    member.view_base = min;
                    member.view_full = max;
                    member.min_value = min;
                    member.max_value = max;
                    break;
                }
            }
            InvalidateChartCache();
        }

        //=== 로그 스케일 설정 ===
        public virtual void SetLogarithmicScale(bool use, double logBase)
        {
            if (chartControl == null || chartControl.ChartAreas.Count == 0) return;
            lock (_chartLock)
            {
                chartControl.ChartAreas[0].AxisY.IsLogarithmic = use;
                if (use && logBase >= 2)
                {
                    chartControl.ChartAreas[0].AxisY.LogarithmBase = logBase;
                }
            }
            InvalidateChartCache();
        }

        //=== 그리드라인 설정 ===
        public virtual void SetGridLine(bool show)
        {
            if (chartControl == null || chartControl.ChartAreas.Count == 0) return;
            lock (_chartLock)
            {
                chartControl.ChartAreas[0].AxisX.MajorGrid.Enabled = show;
                chartControl.ChartAreas[0].AxisY.MajorGrid.Enabled = show;
            }
            InvalidateChartCache();
        }

        /// <summary>
        /// 기본 시리즈 타입 반환 (서브클래스에서 오버라이드)
        /// </summary>
        protected virtual int GetDefaultSeriesType()
        {
            return 0; // Line
        }

        //=== Chart 시리즈 업데이트 유틸리티 ===
        protected void RebuildSeries()
        {
            if (chartControl == null) return;

            int defaultSeriesType = GetDefaultSeriesType();

            lock (_chartLock)
            {
                chartControl.Series.Clear();

                for (int i = 0; i < blockMember.Count; i++)
                {
                    CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];

                    string seriesName = !string.IsNullOrEmpty(member.tag)
                        ? member.tag : $"Series{i}";

                    // 같은 이름이 이미 존재하면 인덱스 추가
                    if (chartControl.Series.FindByName(seriesName) != null)
                    {
                        seriesName = $"{seriesName}_{i}";
                    }

                    // 기본 시리즈 타입 사용 (PropertyPage에서 설정한 값)
                    var series = CreateSeries(seriesName, member.color,
                        defaultSeriesType, member.nLineThick);

                    series.Enabled = member.visible == 1;
                    chartControl.Series.Add(series);
                }
            }
        }

        //=== EnumDataType/EnumDataTime 변환 유틸리티 (ObjectMultiTrend 패턴) ===
        protected static EnumDataType GetDataType(int valueType)
        {
            switch (valueType)
            {
                case 0: return EnumDataType.AVE;
                case 1: return EnumDataType.MIN;
                case 2: return EnumDataType.MAX;
                case 3: return EnumDataType.SUM;
                case 4: return EnumDataType.SUB;
                case 5: return EnumDataType.MOMENT;
                default: return EnumDataType.AVE;
            }
        }

        protected static EnumDataTime GetDataTime(int timeSelectOption)
        {
            switch (timeSelectOption)
            {
                case 0: return EnumDataTime.Minute;
                case 1: return EnumDataTime.Hour;
                case 2: return EnumDataTime.Day;
                case 3: return EnumDataTime.Month;
                default: return EnumDataTime.Minute;
            }
        }

        //=== 리소스 해제 ===
        public virtual void Close()
        {
            try
            {
                lock (_chartLock)
                {
                    cachedChartBitmap?.Dispose();
                    cachedChartBitmap = null;
                    chartControl?.Dispose();
                    chartControl = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObjectChartBase] Close error: {ex.Message}");
            }
        }

        //=== ObjectSave 공통 패턴 ===
        protected void SaveChartCommon(CommaTextWriter writer, ObjectArgsChartCommon chart)
        {
            if (chart == null) return;

            writer.Write("\tChartOption,");
            writer.Write("{0},", chart.nDefaultSeriesType);
            writer.Write("{0},", chart.bShowLegend);
            writer.Write("{0},", chart.bShowGrid);
            writer.Write("{0},", chart.bAntiAlias);
            writer.Write("{0},", chart.b3DStyle);
            writer.Write("{0},", chart.nTitleSize);
            writer.Write("{0},", chart.nLegendPosition);
            writer.Write("{0},", chart.nAxisXLabelAngle);
            writer.Write("{0},", chart.bUseToolBar);
            writer.Write("{0},", chart.nToolBarPos);
            writer.Write("{0},", chart.nToolBarBtnColor);
            writer.Write("{0},", chart.nToolBarButtonSize);
            writer.Write("{0},", chart.nToolBarTextSize);
            writer.Write("{0},", chart.bHideLabelDataRange);
            writer.Write("{0},", chart.bDontUseConfigDialog);
            writer.Write("{0},", chart.bUseMouseButtonAsZoom);
            writer.Write("{0},", chart.bUseCrosshair);
            writer.Write("{0},", chart.bUseTagDescription);
            writer.WriteLine();

            // 색상 정보
            writer.Write("\tChartColors,");
            writer.Write("{0},", chart.colorChartBack.ToArgb());
            writer.Write("{0},", chart.colorPlotBack.ToArgb());
            writer.Write("{0},", chart.colorGrid.ToArgb());
            writer.Write("{0},", chart.colorTitle.ToArgb());
            writer.Write("{0},", chart.colorLegendText.ToArgb());
            writer.Write("{0},", chart.colorLegendBack.ToArgb());
            writer.WriteLine();

            // 제목
            if (!string.IsNullOrEmpty(chart.sTitle))
            {
                writer.WriteLine("\tChartTitle,{0},", chart.sTitle);
            }

            // 스크립트
            if (chart.scriptEventAfterSettings != null)
            {
                chart.scriptEventAfterSettings.SaveScript(writer, "ScriptEventAfterSettings");
            }
        }

        //=== Chart 멤버 저장 ===
        protected void SaveChartMembers(CommaTextWriter writer)
        {
            for (int i = 0; i < blockMember.Count; i++)
            {
                CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];

                writer.Write("\tChartMember,");
                writer.Write("{0},", member.tag);
                writer.Write("{0},", (int)member.nType);
                writer.Write("{0},", member.color.ToArgb());
                writer.Write("{0},", member.nSeriesType);
                writer.Write("{0},", member.nLineThick);
                writer.Write("{0},", member.nPointType);
                writer.Write("{0},", member.nAxisPosition);
                writer.Write("{0},", member.nLevelFrom);
                writer.Write("{0},", member.nLevelTo);
                writer.Write("{0},", member.visible);
                writer.Write("{0},", member.bReverseY);
                writer.Write("{0},", member.nValueType);
                writer.Write("{0},", member.nTagDisplaySize);
                writer.Write("{0},", member.wFlags);
                writer.Write("{0},", member.nTimeShift);
                writer.Write("{0},", member.column ?? "");
                writer.WriteLine();
            }
        }

        //=== 편집모드 미리보기 (샘플 데이터) ===
        protected void FillSampleData(int showUnit)
        {
            if (chartControl == null) return;

            lock (_chartLock)
            {
                for (int i = 0; i < chartControl.Series.Count && i < blockMember.Count; i++)
                {
                    var series = chartControl.Series[i];
                    series.Points.Clear();

                    CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                    double amplitude = (member.view_full - member.view_base) / 2;
                    double center = (member.view_full + member.view_base) / 2;

                    for (int j = 0; j < Math.Min(showUnit, 100); j++)
                    {
                        double val = center + amplitude * Math.Sin(2.0 * Math.PI * j / 50.0 + i * 0.5);
                        series.Points.AddXY(j, val);
                    }
                }
            }
        }

        //=== GetViewFullBase 유틸리티 ===
        protected void GetViewFullBase(TagAiClass ai, CHART_TAG_MEMBER member,
            out double maxVal, out double minVal)
        {
            if (ai != null)
            {
                maxVal = ai.view_full;
                minVal = ai.view_base;
                if (member.view_full != 0 || member.view_base != 0)
                {
                    maxVal = member.view_full;
                    minVal = member.view_base;
                }
            }
            else
            {
                maxVal = member.view_full;
                minVal = member.view_base;
            }
        }
    }

    #endregion
}

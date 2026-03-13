using AutoLibLocal;
using NetTools;
using NetTools.OldDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GraphicModule
{
    /// <summary>
    /// CustomChart: 사용자가 지정한 데이터 포인트를 표시하는 Chart.
    /// Line, FastLine, Spline, Area, Column, StepLine 외에
    /// Pie, Doughnut, Radar 차트도 지원.
    /// 스크립트를 통한 동적 데이터 추가/삭제/갱신 가능.
    /// </summary>
    [Serializable]
    public class CustomChart : ObjectChartBase
    {
        //=== 정적 필드 (스크립트 접근용) ===
        [NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        //=== 인스턴스 필드 ===
        public ObjectArgsChartCustom objArgs;

        // 사용자 지정 데이터 포인트 (태그 기반이 아닌 직접 값 입력)
        public ArrayList customPoints;  // CHART_CUSTOM_POINT 리스트

        // Pie/Doughnut/Radar용 추가 설정
        [NonSerialized]
        private bool _isPieType = false;

        [NonSerialized]
        private bool _isRadarType = false;

        // 태그 기반 순환 버퍼 + 시간 제어 (ChartRealTimeTrend 패턴)
        [NonSerialized]
        private int nBufPos = -1;

        [NonSerialized]
        private int nOldMilliSec = -1;

        [NonSerialized]
        private int nRemainMilliSec = 0;

        // 크로스헤어 상태 (비트맵 위 오버레이로 그림)
        [NonSerialized]
        private bool _showCrosshair = false;

        [NonSerialized]
        private double _crosshairRelX = -1;  // 0.0~1.0 오브젝트 상대 좌표

        [NonSerialized]
        private double _crosshairRelY = -1;

        [NonSerialized]
        private string _crosshairText = "";

        //=== 속성 ===
        public ObjectArgsChartCustom ObjectArgs
        {
            get => objArgs;
            set
            {
                objArgs = value;
                // objArgs.lColor* 를 현재 ObjectExpand 색상으로 동기화
                objArgs.lColorText = GetTextColor();
                objArgs.lColorBack = GetBackColor();
                objArgs.lColorFill = GetFillColor();
                if (chartControl == null) InitializeChart();
                ApplySettings();
                // Studio 편집 모드에서 속성 변경 시 차트 미리보기 갱신
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                {
                    int seriesType = objArgs.chart.nDefaultSeriesType;
                    bool isPieOrRadar = (seriesType == (int)EnumChartSeriesType.Pie ||
                                         seriesType == (int)EnumChartSeriesType.Doughnut ||
                                         seriesType == (int)EnumChartSeriesType.Radar);

                    RebuildSeriesForSpecialType();
                    ApplyTagDescriptionToSeries();

                    if (isPieOrRadar && blockMember.Count > 0)
                    {
                        // Pie/Doughnut/Radar + 태그 멤버: 멤버별 카테고리 데이터
                        FillSamplePieRadarFromMembers();
                    }
                    else if (isPieOrRadar)
                    {
                        // Pie/Doughnut/Radar + 멤버 없음: 샘플 카테고리 데이터
                        FillSampleCustomPoints();
                    }
                    else if (blockMember.Count > 0)
                    {
                        // 태그 멤버 기반 Line/Bar 등: 시계열 샘플 데이터
                        FillSampleData(objArgs.wShowUnit);
                    }
                    else
                    {
                        // 빈 상태: 기본 샘플 커스텀 포인트
                        FillSampleCustomPoints();
                    }
                    InvalidateChartCache();
                }
            }
        }

        protected override int GetShowUnit()
        {
            return objArgs != null ? objArgs.wShowUnit : 100;
        }

        protected override int GetDefaultSeriesType()
        {
            return objArgs?.chart != null ? objArgs.chart.nDefaultSeriesType : 0;
        }

        public ArrayList CustomPoints
        {
            get => customPoints;
            set
            {
                customPoints = value;
                UpdateChartFromCustomPoints();
                InvalidateChartCache();
            }
        }

        /// <summary>
        /// CustomTrend는 멤버 또는 커스텀포인트 중 하나라도 있으면 데이터 있음
        /// </summary>
        protected override bool HasAnyData()
        {
            return (blockMember != null && blockMember.Count > 0) ||
                   (customPoints != null && customPoints.Count > 0);
        }

        //=== 생성자 ===
        public CustomChart(ObjectCommonProperty ocp, Form form, RECT rect,
            EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf,
            ObjectArgsChartCustom args, ArrayList block, ArrayList points)
            : base(ocp, rect, eid, lf, general)
        {
            enumObjectType = EnumObjectType.CustomChart;
            objArgs = args ?? new ObjectArgsChartCustom();
            formParent = form;

            if (block == null)
                blockMember = new ArrayList();
            else
                blockMember = block;

            if (points == null)
                customPoints = new ArrayList();
            else
                customPoints = points;

            // 기본값 보정
            if (objArgs.wShowUnit < 2) objArgs.wShowUnit = 100;
            if (objArgs.wLevelDevide < 2) objArgs.wLevelDevide = 5;
            if (objArgs.wTimeDivide < 1) objArgs.wTimeDivide = 10;

            SetBlock(blockMember);

            InitializeChart();
            ApplySettings();

            // 태그 기반 시리즈 구성
            if (blockMember.Count > 0)
            {
                RebuildSeries();
                ApplyTagDescriptionToSeries();
                MallocAllBuf(objArgs.wShowUnit);
            }

            // 커스텀 포인트 기반 시리즈 구성
            if (customPoints.Count > 0)
            {
                UpdateChartFromCustomPoints();
            }

            // 편집 모드 샘플 데이터
            if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
            {
                if (blockMember.Count > 0)
                    FillSampleData(objArgs.wShowUnit);
                else if (customPoints.Count == 0)
                    FillSampleCustomPoints();
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);
            }

            TextColor = args.lColorText;
            BackColor = args.lColorBack;
            FillColor = args.lColorFill;

            // 트렌드 색상이 설정된 후 차트에 매핑 재적용
            MapTrendColorsToChart();
            if (chartControl != null)
                ApplyChartCommonSettings(objArgs.chart);
        }

        // 간편 생성자 (커스텀 포인트 없이)
        public CustomChart(ObjectCommonProperty ocp, Form form, RECT rect,
            EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf,
            ObjectArgsChartCustom args, ArrayList block)
            : this(ocp, form, rect, eid, general, lf, args, block, null)
        {
        }

        //=== 태그 설명 표시: 태그 이름 대신 태그 설명을 시리즈 이름으로 사용 ===
        private string GetTagDescription(CHART_TAG_MEMBER member, int index)
        {
            if (objArgs?.chart?.bUseTagDescription == true && !string.IsNullOrEmpty(member.tag))
            {
                try
                {
                    if (member.nType == EnumTagType.AI)
                    {
                        TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);
                        if (ai != null && !string.IsNullOrEmpty(ai.description))
                            return ai.description;
                    }
                    else if (member.nType == EnumTagType.DI)
                    {
                        TagDiClass di = TagLib.GetStructDI(member.tag, ref member.nPos);
                        if (di != null && !string.IsNullOrEmpty(di.description))
                            return di.description;
                    }
                }
                catch { }
            }

            return !string.IsNullOrEmpty(member.tag) ? member.tag : $"Series{index}";
        }

        /// <summary>
        /// bUseTagDescription 옵션이 켜져 있으면 모든 시리즈 이름을 태그 설명으로 변경
        /// RebuildSeries() 호출 후 실행
        /// </summary>
        private void ApplyTagDescriptionToSeries()
        {
            if (objArgs?.chart?.bUseTagDescription != true) return;
            if (chartControl == null) return;

            lock (_chartLock)
            {
                for (int i = 0; i < blockMember.Count && i < chartControl.Series.Count; i++)
                {
                    CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                    string displayName = GetTagDescription(member, i);

                    try
                    {
                        // Series.Name 변경 시 중복 이름 체크
                        bool nameExists = false;
                        foreach (var s in chartControl.Series)
                        {
                            if (s != chartControl.Series[i] && s.Name == displayName)
                            {
                                nameExists = true;
                                break;
                            }
                        }

                        if (nameExists)
                            displayName = $"{displayName}_{i}";

                        chartControl.Series[i].Name = displayName;
                        chartControl.Series[i].LegendText = displayName;
                    }
                    catch { }
                }
            }
        }

        //=== 트렌드 색상 → 차트 색상 매핑 ===
        // 채움색 = Plot 배경, 배경색 = Chart 배경, 눈금색 = 그리드 색상, 글자색 = 제목 색상
        private void MapTrendColorsToChart()
        {
            if (objArgs == null || objArgs.chart == null) return;

            BrushPublic fill = GetFillColor();
            if (fill != null)
                objArgs.chart.colorPlotBack = fill.basic_color;

            BrushPublic back = GetBackColor();
            if (back != null)
                objArgs.chart.colorChartBack = back.basic_color;

            objArgs.chart.colorGrid = objArgs.lColorGuideLine;
            objArgs.chart.colorTitle = GetTextColor();
        }

        /// <summary>
        /// 외부에서 색상 변경 후 차트에 반영 (PropertyRecv에서 호출)
        /// </summary>
        public void ApplyColorToChart()
        {
            if (chartControl == null || objArgs == null) return;
            MapTrendColorsToChart();
            ApplyChartCommonSettings(objArgs.chart);
            InvalidateChartCache();
        }

        /// <summary>
        /// 렌더링 직전 차트 컨트롤 색상이 ObjectExpand 색상과 동기화되어 있는지 확인.
        /// 속성 변경 후 차트 컨트롤 색상이 누락될 수 있으므로 매 페인트 시 확인.
        /// </summary>
        private void EnsureChartColors()
        {
            if (chartControl == null || objArgs == null) return;

            BrushPublic back = GetBackColor();
            BrushPublic fill = GetFillColor();
            Color expectedChartBack = back != null ? back.basic_color : Color.White;
            Color expectedPlotBack = fill != null ? fill.basic_color : Color.White;

            // 알파가 0이면 불투명 흰색으로 교체 (투명하면 차트가 안 보임)
            if (expectedChartBack.A == 0) expectedChartBack = Color.White;
            if (expectedPlotBack.A == 0) expectedPlotBack = Color.White;

            bool needUpdate = false;
            if (chartControl.BackColor != expectedChartBack)
            {
                objArgs.chart.colorChartBack = expectedChartBack;
                chartControl.BackColor = expectedChartBack;
                needUpdate = true;
            }

            if (chartControl.ChartAreas.Count > 0 &&
                chartControl.ChartAreas[0].BackColor != expectedPlotBack)
            {
                objArgs.chart.colorPlotBack = expectedPlotBack;
                chartControl.ChartAreas[0].BackColor = expectedPlotBack;
                needUpdate = true;
            }

            if (needUpdate)
                InvalidateChartCache();
        }

        //=== 설정 적용 ===
        private void ApplySettings()
        {
            if (chartControl == null || objArgs == null) return;

            MapTrendColorsToChart();
            ApplyChartCommonSettings(objArgs.chart);

            int seriesType = objArgs.chart.nDefaultSeriesType;
            _isPieType = (seriesType == (int)EnumChartSeriesType.Pie ||
                          seriesType == (int)EnumChartSeriesType.Doughnut);
            _isRadarType = (seriesType == (int)EnumChartSeriesType.Radar);

            lock (_chartLock)
            {
                if (chartControl.ChartAreas.Count > 0)
                {
                    var area = chartControl.ChartAreas[0];

                    // Pie/Doughnut은 축이 필요 없음
                    if (_isPieType)
                    {
                        area.AxisX.Enabled = AxisEnabled.False;
                        area.AxisY.Enabled = AxisEnabled.False;
                        area.AxisX.MajorGrid.Enabled = false;
                        area.AxisY.MajorGrid.Enabled = false;
                    }
                    else
                    {
                        area.AxisX.Enabled = AxisEnabled.Auto;
                        area.AxisY.Enabled = AxisEnabled.Auto;

                        // X축 DateTime 설정 (태그 기반 시계열)
                        area.AxisX.LabelStyle.Format = GetTimeAxisFormat();
                        area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;

                        // 로그 스케일
                        if (objArgs.logarithmicScale != null && objArgs.logarithmicScale.bUse)
                        {
                            area.AxisY.IsLogarithmic = true;
                            area.AxisY.LogarithmBase = Math.Max(2, objArgs.logarithmicScale.fBase);
                        }
                        else
                        {
                            area.AxisY.IsLogarithmic = false;
                        }
                    }

                    // Radar 설정
                    if (_isRadarType)
                    {
                        area.Area3DStyle.Enable3D = false; // Radar는 2D만 지원
                    }

                    // 줌 (Pie/Doughnut/Radar 제외)
                    if (!_isPieType && !_isRadarType && objArgs.chart.bUseMouseButtonAsZoom)
                    {
                        area.CursorX.IsUserEnabled = true;
                        area.CursorX.IsUserSelectionEnabled = true;
                        area.CursorY.IsUserEnabled = true;
                        area.CursorY.IsUserSelectionEnabled = true;
                        area.AxisX.ScaleView.Zoomable = true;
                        area.AxisY.ScaleView.Zoomable = true;
                    }
                }
            }
        }

        //=== X축 시간 포맷 결정 (nDataTime 기반) ===
        private string GetTimeAxisFormat()
        {
            if (objArgs == null) return "HH:mm:ss";
            int totalSpanMs = objArgs.nDataTime * objArgs.wShowUnit;
            if (totalSpanMs < 60000)           // < 1분: 시:분:초
                return "HH:mm:ss";
            else if (totalSpanMs < 3600000)    // < 1시간: 시:분:초
                return "HH:mm:ss";
            else if (totalSpanMs < 86400000)   // < 1일: 시:분
                return "HH:mm";
            else                                // >= 1일: 월/일 시:분
                return "MM/dd HH:mm";
        }

        //=== Pie/Doughnut/Radar 시리즈 재구성 ===
        private void RebuildSeriesForSpecialType()
        {
            if (chartControl == null) return;

            int seriesType = objArgs.chart.nDefaultSeriesType;
            _isPieType = (seriesType == (int)EnumChartSeriesType.Pie ||
                          seriesType == (int)EnumChartSeriesType.Doughnut);
            _isRadarType = (seriesType == (int)EnumChartSeriesType.Radar);

            lock (_chartLock)
            {
                chartControl.Series.Clear();

                if (_isPieType || _isRadarType)
                {
                    // Pie/Doughnut/Radar: 단일 시리즈에 모든 데이터 포인트
                    var series = new Series("CustomData");
                    series.ChartType = ConvertSeriesType(seriesType);
                    series.ChartArea = "MainArea";
                    series.Legend = "MainLegend";

                    if (seriesType == (int)EnumChartSeriesType.Doughnut)
                    {
                        series["PieDrawingStyle"] = "SoftEdge";
                        series["DoughnutRadius"] = "60";
                    }

                    if (seriesType == (int)EnumChartSeriesType.Pie)
                    {
                        series["PieLabelStyle"] = "Outside";
                        series["PieDrawingStyle"] = "SoftEdge";
                    }

                    if (_isRadarType)
                    {
                        series["RadarDrawingStyle"] = "Area";
                        series.BorderWidth = 2;
                        series.Color = Color.FromArgb(128, 0, 120, 215);
                        series.BorderColor = Color.FromArgb(0, 120, 215);
                    }

                    chartControl.Series.Add(series);
                }
                else
                {
                    // 일반 트렌드: 태그 멤버별 시리즈
                    RebuildSeries();
                }
            }
        }

        //=== 커스텀 포인트 → Chart 데이터 변환 ===
        private void UpdateChartFromCustomPoints()
        {
            if (chartControl == null) return;

            int seriesType = objArgs.chart.nDefaultSeriesType;
            _isPieType = (seriesType == (int)EnumChartSeriesType.Pie ||
                          seriesType == (int)EnumChartSeriesType.Doughnut);
            _isRadarType = (seriesType == (int)EnumChartSeriesType.Radar);

            RebuildSeriesForSpecialType();

            lock (_chartLock)
            {
                if (_isPieType || _isRadarType)
                {
                    // 단일 시리즈에 모든 커스텀 포인트 추가
                    if (chartControl.Series.Count > 0)
                    {
                        var series = chartControl.Series[0];
                        series.Points.Clear();

                        for (int i = 0; i < customPoints.Count; i++)
                        {
                            CHART_CUSTOM_POINT cp = (CHART_CUSTOM_POINT)customPoints[i];
                            if (!cp.visible) continue;

                            int idx = series.Points.AddXY(
                                !string.IsNullOrEmpty(cp.label) ? cp.label : $"P{i}",
                                cp.value);

                            if (idx >= 0 && idx < series.Points.Count)
                            {
                                series.Points[idx].Color = cp.color;
                                series.Points[idx].Label = cp.label;

                                if (_isPieType)
                                {
                                    series.Points[idx].LegendText = cp.label;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 일반 트렌드: 각 커스텀 포인트를 별도 시리즈로 표시하거나
                    // 단일 시리즈에 순차적으로 추가
                    if (chartControl.Series.Count == 0)
                    {
                        var series = CreateSeries("CustomData", Color.Blue,
                            seriesType, 2);
                        chartControl.Series.Add(series);
                    }

                    if (chartControl.Series.Count > 0)
                    {
                        var series = chartControl.Series[0];
                        series.Points.Clear();

                        for (int i = 0; i < customPoints.Count; i++)
                        {
                            CHART_CUSTOM_POINT cp = (CHART_CUSTOM_POINT)customPoints[i];
                            if (!cp.visible) continue;

                            int idx = series.Points.AddXY(
                                !string.IsNullOrEmpty(cp.label) ? cp.label : i.ToString(),
                                cp.value);

                            if (idx >= 0 && idx < series.Points.Count)
                            {
                                series.Points[idx].Color = cp.color;
                            }
                        }
                    }
                }
            }

            InvalidateChartCache();
        }

        //=== 태그 기반 데이터 → Chart 시리즈 변환 ===
        private void UpdateChartFromTagData()
        {
            if (chartControl == null || nBufPos < 0) return;

            int seriesType = objArgs.chart.nDefaultSeriesType;
            bool isSingleSeriesType =
                seriesType == (int)EnumChartSeriesType.Pie ||
                seriesType == (int)EnumChartSeriesType.Doughnut ||
                seriesType == (int)EnumChartSeriesType.Radar;

            lock (_chartLock)
            {
                if (isSingleSeriesType)
                {
                    // Radar/Pie/Doughnut: 단일 Series, 태그별 현재값을 카테고리 포인트로
                    if (chartControl.Series.Count == 0) return;

                    var series = chartControl.Series[0];
                    series.Points.Clear();

                    for (int i = 0; i < blockMember.Count; i++)
                    {
                        CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                        if (member.point == null || member.visible != 1) continue;

                        double val = member.point[nBufPos].read_flag
                            ? member.point[nBufPos].val : 0;

                        // bUseTagDescription: 태그 설명으로 라벨 표시
                        string label = GetTagDescription(member, i);

                        int idx = series.Points.AddXY(label, val);
                        if (idx >= 0 && idx < series.Points.Count)
                        {
                            series.Points[idx].Color = member.color;
                            series.Points[idx].Label = label;
                            if (seriesType == (int)EnumChartSeriesType.Pie ||
                                seriesType == (int)EnumChartSeriesType.Doughnut)
                            {
                                series.Points[idx].LegendText = label;
                            }
                        }
                    }
                }
                else
                {
                    // Line/Area/Spline/StepLine/Bar: 태그당 시리즈, 순환 버퍼
                    // X축 = DateTime (맨 오른쪽 = 현재시간, 왼쪽 = 과거)
                    DateTime now = DateTimeServer.Now;
                    int showUnit = objArgs.wShowUnit;
                    if (showUnit <= 0) showUnit = 100;
                    int dataTimeMs = objArgs.nDataTime;
                    if (dataTimeMs <= 0) dataTimeMs = 1000;

                    // 유효 데이터 개수 계산 (첫 번째 멤버 기준)
                    int validCount = 0;
                    int startPos = (nBufPos + 1) % showUnit;
                    if (blockMember.Count > 0)
                    {
                        CHART_TAG_MEMBER firstMember = (CHART_TAG_MEMBER)blockMember[0];
                        if (firstMember.point != null)
                        {
                            for (int j = 0; j < showUnit; j++)
                            {
                                int pos = (startPos + j) % showUnit;
                                if (firstMember.point[pos].read_flag)
                                    validCount++;
                            }
                        }
                    }

                    for (int i = 0; i < blockMember.Count && i < chartControl.Series.Count; i++)
                    {
                        CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                        if (member.point == null) continue;

                        var series = chartControl.Series[i];
                        series.Points.Clear();
                        series.XValueType = ChartValueType.DateTime;

                        // 가장 오래된 데이터부터 순서대로 출력
                        int dataIdx = 0;
                        for (int j = 0; j < showUnit; j++)
                        {
                            int pos = (startPos + j) % showUnit;
                            if (!member.point[pos].read_flag) continue;

                            // 맨 오른쪽(최신) = now, 왼쪽으로 갈수록 과거
                            int backSteps = validCount - 1 - dataIdx;
                            DateTime xTime = now.AddMilliseconds(-(long)backSteps * dataTimeMs);
                            series.Points.AddXY(xTime, member.point[pos].val);
                            dataIdx++;
                        }
                    }
                }
            }

            InvalidateChartCache();
        }

        //=== 편집 모드용 샘플 커스텀 포인트 ===
        private void FillSampleCustomPoints()
        {
            int seriesType = objArgs.chart.nDefaultSeriesType;

            if (seriesType == (int)EnumChartSeriesType.Pie ||
                seriesType == (int)EnumChartSeriesType.Doughnut)
            {
                customPoints.Clear();
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "Category A", value = 35, color = Color.FromArgb(0, 120, 215) });
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "Category B", value = 25, color = Color.FromArgb(255, 140, 0) });
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "Category C", value = 20, color = Color.FromArgb(40, 167, 69) });
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "Category D", value = 15, color = Color.FromArgb(220, 53, 69) });
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "Category E", value = 5, color = Color.FromArgb(108, 117, 125) });
            }
            else if (seriesType == (int)EnumChartSeriesType.Radar)
            {
                customPoints.Clear();
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "항목1", value = 80, color = Color.FromArgb(0, 120, 215) });
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "항목2", value = 65, color = Color.FromArgb(0, 120, 215) });
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "항목3", value = 90, color = Color.FromArgb(0, 120, 215) });
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "항목4", value = 45, color = Color.FromArgb(0, 120, 215) });
                customPoints.Add(new CHART_CUSTOM_POINT
                    { label = "항목5", value = 70, color = Color.FromArgb(0, 120, 215) });
            }
            else
            {
                customPoints.Clear();
                for (int i = 0; i < 10; i++)
                {
                    customPoints.Add(new CHART_CUSTOM_POINT
                    {
                        label = $"X{i}",
                        value = 50 + 30 * Math.Sin(i * 0.7),
                        color = Color.FromArgb(0, 120, 215)
                    });
                }
            }

            UpdateChartFromCustomPoints();
        }

        //=== DisplayObject ===
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            // Chart 컨트롤은 자체 배경색을 렌더링하므로 PopBox2 배경 그리기 불필요

            // 역직렬화(복사/붙여넣기) 후 chartControl이 null인 경우 전체 재초기화
            if (chartControl == null && objArgs != null)
            {
                InitializeChart();
                MapTrendColorsToChart();
                ApplySettings();

                int seriesType = objArgs.chart.nDefaultSeriesType;
                bool isPieOrRadar = (seriesType == (int)EnumChartSeriesType.Pie ||
                                     seriesType == (int)EnumChartSeriesType.Doughnut ||
                                     seriesType == (int)EnumChartSeriesType.Radar);

                if (blockMember.Count > 0)
                {
                    if (isPieOrRadar)
                    {
                        RebuildSeriesForSpecialType();
                        ApplyTagDescriptionToSeries();
                        if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                            FillSamplePieRadarFromMembers();
                    }
                    else
                    {
                        RebuildSeries();
                        ApplyTagDescriptionToSeries();
                        MallocAllBuf(objArgs.wShowUnit);
                        if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                            FillSampleData(objArgs.wShowUnit);
                    }
                }
                else if (customPoints.Count > 0)
                {
                    UpdateChartFromCustomPoints();
                }
                else if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                {
                    FillSampleCustomPoints();
                }
                InvalidateChartCache();
            }

            // 렌더링 전 차트 컨트롤 색상이 ObjectExpand 색상과 동기화되어 있는지 확인
            EnsureChartColors();

            DisplayChartObject(g, x1, y1, x2, y2);

            // 크로스헤어 오버레이 (비트맵 캐시 위에 그림)
            if (_showCrosshair && objArgs?.chart?.bUseCrosshair == true)
            {
                DrawCrosshairOverlay(g, x1, y1, x2, y2);
            }
        }

        //=== EventTimerObject (태그 기반: nDataTime 간격으로 순환 버퍼에 데이터 축적) ===
        public override async Task EventTimerObject(Form form)
        {
            // 역직렬화 후 formParent 복원
            if (formParent == null && form != null)
                formParent = form;

            if (blockMember.Count == 0)
            {
                await Task.CompletedTask;
                return;
            }

            // 1단계: 모든 멤버 태그에 현재값 요청
            for (int i = 0; i < blockMember.Count; i++)
            {
                try
                {
                    CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                    TagPublicClass pub = TagLib.GetStructPublic(member.tag, ref member.nPos);
                    pub.NeedDataCurr = true;
                }
                catch { }
            }

            DateTime dt = DateTimeServer.Now;

            // 2단계: 최초 초기화
            if (nOldMilliSec == -1)
            {
                nOldMilliSec = dt.Second * 1000 + dt.Millisecond;
                nRemainMilliSec = 0;
                nBufPos = 0;
                FillData();
                UpdateChartFromTagData();
                InvalidateObject(form);
                await Task.CompletedTask;
                return;
            }

            // 3단계: 경과 시간 계산
            int curMilliSec = dt.Second * 1000 + dt.Millisecond;
            if (curMilliSec < nOldMilliSec)
                nRemainMilliSec += (curMilliSec + 60000) - nOldMilliSec;
            else
                nRemainMilliSec += curMilliSec - nOldMilliSec;
            nOldMilliSec = curMilliSec;

            // 4단계: nDataTime 간격마다 데이터 수집
            bool dataFlag = false;
            while (nRemainMilliSec >= objArgs.nDataTime)
            {
                FillData();
                nRemainMilliSec -= objArgs.nDataTime;
                dataFlag = true;
            }

            // 5단계: 새 데이터가 있으면 Chart 갱신
            if (dataFlag)
            {
                UpdateChartFromTagData();
                InvalidateObject(form);
            }

            await Task.CompletedTask;
        }

        //=== FillData: 현재 태그값을 순환 버퍼에 저장 ===
        private void FillData()
        {
            int showUnit = objArgs.wShowUnit;
            if (showUnit <= 0) showUnit = 100;

            nBufPos++;
            nBufPos %= showUnit;

            for (int i = 0; i < blockMember.Count; i++)
            {
                CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                if (member.point == null) continue;

                try
                {
                    if (member.nType == EnumTagType.AI)
                    {
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                        GetViewFullBase(ai, member, out member.max_value, out member.min_value);
                        member.point[nBufPos].read_flag = true;
                        member.point[nBufPos].val = ai.curr;
                    }
                    else if (member.nType == EnumTagType.DI)
                    {
                        TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                        member.min_value = 0;
                        member.max_value = 100;
                        member.point[nBufPos].read_flag = true;
                        member.point[nBufPos].val = di.curr;
                    }
                }
                catch
                {
                    member.point[nBufPos].read_flag = false;
                    member.point[nBufPos].val = 0;
                }
            }
        }

        //=== 스크립트 API: 커스텀 포인트 추가 ===
        public void AddCustomPoint(string label, double value, Color color)
        {
            // 태그 기반 멤버가 있으면 클리어 (혼용 방지)
            if (blockMember.Count > 0)
            {
                blockMember.Clear();
                if (chartControl != null)
                {
                    lock (_chartLock) { chartControl.Series.Clear(); }
                }
            }

            customPoints.Add(new CHART_CUSTOM_POINT
            {
                label = label,
                value = value,
                color = color,
                visible = true
            });

            UpdateChartFromCustomPoints();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        //=== 스크립트 API: 커스텀 포인트 삭제 ===
        public void RemoveCustomPoint(string label)
        {
            for (int i = customPoints.Count - 1; i >= 0; i--)
            {
                CHART_CUSTOM_POINT cp = (CHART_CUSTOM_POINT)customPoints[i];
                if (cp.label == label)
                {
                    customPoints.RemoveAt(i);
                    break;
                }
            }

            UpdateChartFromCustomPoints();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        //=== 스크립트 API: 커스텀 포인트 값 갱신 ===
        public void SetCustomPointValue(string label, double value)
        {
            for (int i = 0; i < customPoints.Count; i++)
            {
                CHART_CUSTOM_POINT cp = (CHART_CUSTOM_POINT)customPoints[i];
                if (cp.label == label)
                {
                    cp.value = value;
                    break;
                }
            }

            UpdateChartFromCustomPoints();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        //=== 스크립트 API: 커스텀 포인트 색상 변경 ===
        public void SetCustomPointColor(string label, Color color)
        {
            for (int i = 0; i < customPoints.Count; i++)
            {
                CHART_CUSTOM_POINT cp = (CHART_CUSTOM_POINT)customPoints[i];
                if (cp.label == label)
                {
                    cp.color = color;
                    break;
                }
            }

            UpdateChartFromCustomPoints();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        //=== 스크립트 API: 모든 커스텀 포인트 초기화 ===
        public void ClearCustomPoints()
        {
            customPoints.Clear();

            if (chartControl != null)
            {
                lock (_chartLock)
                {
                    foreach (var series in chartControl.Series)
                        series.Points.Clear();
                }
            }

            InvalidateChartCache();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        //=== 스크립트 API: Chart 타입 변경 (Pie/Doughnut/Radar 포함) ===
        public override void SetSeriesType(int seriesType)
        {
            objArgs.chart.nDefaultSeriesType = seriesType;
            ApplySettings();
            RebuildSeriesForSpecialType();

            if (customPoints.Count > 0)
                UpdateChartFromCustomPoints();
            else if (blockMember.Count > 0)
                UpdateChartFromTagData();

            InvalidateChartCache();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        //=== ChartMember 변경 시 Pie/Doughnut/Radar 특수 처리 ===
        protected override void OnChartMemberChanged()
        {
            if (chartControl == null) InitializeChart();

            int seriesType = objArgs.chart.nDefaultSeriesType;
            bool isPieOrRadar = (seriesType == (int)EnumChartSeriesType.Pie ||
                                 seriesType == (int)EnumChartSeriesType.Doughnut ||
                                 seriesType == (int)EnumChartSeriesType.Radar);

            if (blockMember.Count > 0)
            {
                if (isPieOrRadar)
                {
                    // Pie/Doughnut/Radar: 단일 시리즈 + 멤버 개수만큼 카테고리 데이터
                    RebuildSeriesForSpecialType();
                    ApplyTagDescriptionToSeries();
                    FillSamplePieRadarFromMembers();
                }
                else
                {
                    // 일반 트렌드: 멤버별 시리즈 + 시계열 샘플 데이터
                    RebuildSeries();
                    ApplyTagDescriptionToSeries();
                    MallocAllBuf(objArgs.wShowUnit);
                    FillSampleData(objArgs.wShowUnit);
                }
            }
            else if (customPoints != null && customPoints.Count > 0)
            {
                // 멤버 없음 + 커스텀 포인트 존재: 커스텀 포인트로 렌더링
                UpdateChartFromCustomPoints();
            }
            else
            {
                // 멤버도 커스텀 포인트도 없음: 기본 샘플 데이터 표시
                FillSampleCustomPoints();
            }
            InvalidateChartCache();
        }

        /// <summary>
        /// Pie/Doughnut/Radar용: blockMember 개수만큼 단일 시리즈에 샘플 데이터 추가
        /// </summary>
        private void FillSamplePieRadarFromMembers()
        {
            if (chartControl == null || blockMember.Count == 0) return;

            lock (_chartLock)
            {
                if (chartControl.Series.Count == 0) return;
                var series = chartControl.Series[0];
                series.Points.Clear();

                for (int i = 0; i < blockMember.Count; i++)
                {
                    CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                    string label = !string.IsNullOrEmpty(member.tag) ? member.tag : $"Member{i}";
                    double sampleValue = 20 + 15 * Math.Sin(i * 1.2); // 의미있는 샘플 값
                    if (sampleValue < 5) sampleValue = 5;

                    int idx = series.Points.AddXY(label, sampleValue);
                    if (idx >= 0 && idx < series.Points.Count)
                    {
                        series.Points[idx].Color = member.color;
                        series.Points[idx].Label = label;
                        if (_isPieType)
                            series.Points[idx].LegendText = label;
                    }
                }
            }
        }

        //=== 스크립트 API: 커스텀 포인트 개수 ===
        public int GetCustomPointCount()
        {
            return customPoints.Count;
        }

        //=== 스크립트 API: 커스텀 포인트 값 조회 ===
        public double GetCustomPointValue(string label)
        {
            for (int i = 0; i < customPoints.Count; i++)
            {
                CHART_CUSTOM_POINT cp = (CHART_CUSTOM_POINT)customPoints[i];
                if (cp.label == label) return cp.value;
            }
            return 0;
        }

        //=== 전체 데이터 초기화 ===
        public void ClearData()
        {
            blockMember.Clear();
            customPoints.Clear();
            nBufPos = -1;
            nOldMilliSec = -1;
            nRemainMilliSec = 0;

            if (chartControl != null)
            {
                lock (_chartLock)
                {
                    chartControl.Series.Clear();
                }
            }

            InvalidateChartCache();
            if (formParent != null)
                InvalidateObject(formParent);
        }

        //=== DeleteTag override: Radar/Pie/Doughnut 단일 Series 보호 ===
        public override void DeleteTag(string tagName)
        {
            int seriesType = objArgs.chart.nDefaultSeriesType;
            bool isSingleSeriesType =
                seriesType == (int)EnumChartSeriesType.Pie ||
                seriesType == (int)EnumChartSeriesType.Doughnut ||
                seriesType == (int)EnumChartSeriesType.Radar;

            if (isSingleSeriesType)
            {
                // 단일 Series는 유지하고 blockMember만 제거
                for (int i = blockMember.Count - 1; i >= 0; i--)
                {
                    CHART_TAG_MEMBER member = (CHART_TAG_MEMBER)blockMember[i];
                    if (member.tag == tagName)
                    {
                        blockMember.RemoveAt(i);
                        break;
                    }
                }

                // blockMember가 비면 단일 Series도 제거
                if (blockMember.Count == 0 && chartControl != null)
                {
                    lock (_chartLock) { chartControl.Series.Clear(); }
                }

                InvalidateChartCache();
                if (formParent != null)
                    InvalidateObject(formParent);
            }
            else
            {
                base.DeleteTag(tagName);
            }
        }

        //=== ObjectSave ===
        public override void ObjectSave(CommaTextWriter writer)
        {
            ObjectSaveFont(writer);

            SaveObjectItem.BackColor(writer, GetBackColor());
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.FillColor(writer, GetFillColor());
            SaveObjectItem.GuideLineColor(writer, objArgs.lColorGuideLine);
            SaveObjectItem.LogarithmicScale(writer, objArgs.logarithmicScale);

            writer.WriteLine("\tShowUnit,{0},", objArgs.wShowUnit);
            writer.WriteLine("\twTimeDevide,{0},", objArgs.wTimeDivide);
            writer.WriteLine("\twLevelDevide,{0},", objArgs.wLevelDevide);
            writer.WriteLine("\tnDataTime,{0},", objArgs.nDataTime);

            SaveObjectItem.GraphPublicArgs(writer, objArgs.pub);
            SaveChartCommon(writer, objArgs.chart);
            SaveChartMembers(writer);

            // 커스텀 포인트 저장
            for (int i = 0; i < customPoints.Count; i++)
            {
                CHART_CUSTOM_POINT cp = (CHART_CUSTOM_POINT)customPoints[i];
                writer.Write("\tCustomPoint,");
                writer.Write("{0},", cp.label ?? "");
                writer.Write("{0},", cp.value);
                writer.Write("{0},", cp.color.ToArgb());
                writer.Write("{0},", cp.visible);
                writer.WriteLine();
            }

            writer.Write("\tStringOption,");
            writer.Write("{0},", objArgs.bSupportPie);
            writer.Write("{0},", objArgs.bSupportDoughnut);
            writer.Write("{0},", objArgs.bSupportRadar);
            writer.WriteLine();
        }

        //=== AddTag override: customPoints 혼용 방지 + Radar/Pie/Doughnut 단일 Series ===
        public override void AddTag(CHART_TAG_MEMBER member)
        {
            // 커스텀 포인트가 있으면 클리어 (혼용 방지)
            if (customPoints.Count > 0)
            {
                customPoints.Clear();
                nBufPos = -1;
                nOldMilliSec = -1;
                nRemainMilliSec = 0;
                if (chartControl != null)
                {
                    lock (_chartLock) { chartControl.Series.Clear(); }
                }
            }

            int seriesType = objArgs.chart.nDefaultSeriesType;
            bool isSingleSeriesType =
                seriesType == (int)EnumChartSeriesType.Pie ||
                seriesType == (int)EnumChartSeriesType.Doughnut ||
                seriesType == (int)EnumChartSeriesType.Radar;

            if (isSingleSeriesType)
            {
                // Radar/Pie/Doughnut: 단일 Series에 태그별 카테고리 포인트
                // base.AddTag의 멤버 초기화만 수행하고 Series 생성은 직접 관리
                if (member == null) return;

                try { TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos); }
                catch { }

                if (member.nLevelFrom < 0) member.nLevelFrom = 0;
                if (member.nLevelTo > 100) member.nLevelTo = 100;
                if (member.nLevelFrom >= member.nLevelTo)
                { member.nLevelFrom = 0; member.nLevelTo = 100; }

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
                    { member.min_value = 0; member.max_value = 100; }
                }

                blockMember.Add(member);
                MallocOneBuf(member, GetShowUnit());

                // 단일 Series 관리: 없으면 생성
                if (chartControl != null)
                {
                    lock (_chartLock)
                    {
                        if (chartControl.Series.Count == 0)
                        {
                            var series = new Series("TagData");
                            series.ChartType = ConvertSeriesType(seriesType);
                            series.ChartArea = "MainArea";
                            series.Legend = "MainLegend";

                            if (seriesType == (int)EnumChartSeriesType.Doughnut)
                            {
                                series["PieDrawingStyle"] = "SoftEdge";
                                series["DoughnutRadius"] = "60";
                            }
                            else if (seriesType == (int)EnumChartSeriesType.Pie)
                            {
                                series["PieLabelStyle"] = "Outside";
                                series["PieDrawingStyle"] = "SoftEdge";
                            }
                            else if (seriesType == (int)EnumChartSeriesType.Radar)
                            {
                                series["RadarDrawingStyle"] = "Area";
                                series.BorderWidth = 2;
                                series.Color = Color.FromArgb(128, 0, 120, 215);
                                series.BorderColor = Color.FromArgb(0, 120, 215);
                            }

                            chartControl.Series.Add(series);
                        }
                    }
                }

                InvalidateChartCache();
                if (formParent != null)
                    InvalidateObject(formParent);
            }
            else
            {
                // Line/Area/Spline/StepLine/Bar: 태그당 별도 Series (순환 버퍼)
                base.AddTag(member);
                ApplyTagDescriptionToSeries();
            }
        }

        //=== ExecuteClassName: 스크립트 명령 디스패치 ===
        public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            await Task.CompletedTask;

            if (command == "CustomChartClear")
            {
                ClearData();
            }
            else if (command == "CustomChartAddTag")
            {
                CHART_TAG_MEMBER member = new CHART_TAG_MEMBER();
                member.tag = (string)args[1];
                member.color = Color.FromArgb((int)args[2]);
                AddTag(member);
            }
            else if (command == "CustomChartDeleteTag")
            {
                DeleteTag((string)args[1]);
            }
            else if (command == "CustomChartAddPoint")
            {
                AddCustomPoint((string)args[1], (double)args[2], Color.FromArgb((int)args[3]));
            }
            else if (command == "CustomChartRemovePoint")
            {
                RemoveCustomPoint((string)args[1]);
            }
            else if (command == "CustomChartSetPointValue")
            {
                SetCustomPointValue((string)args[1], (double)args[2]);
            }
            else if (command == "CustomChartGetPointValue")
            {
                return GetCustomPointValue((string)args[1]);
            }
            else if (command == "CustomChartSetPointColor")
            {
                SetCustomPointColor((string)args[1], Color.FromArgb((int)args[2]));
            }
            else if (command == "CustomChartGetPointCount")
            {
                return GetCustomPointCount();
            }
            else if (command == "CustomChartClearPoints")
            {
                ClearCustomPoints();
            }
            else if (command == "CustomChartSetSeriesType")
            {
                SetSeriesType((int)args[1]);
            }
            else if (command == "CustomChartSetVisible")
            {
                SetVisible((string)args[1], (int)args[2] != 0);
            }
            else if (command == "CustomChartSetBackColor")
            {
                GetBackColor().basic_color = Color.FromArgb((int)args[1]);
                InvalidateChartCache();
                if (formParent != null) InvalidateObject(formParent);
            }
            else if (command == "CustomChartSetBasicLevel")
            {
                SetBasicLevel((string)args[1], (double)args[2], (double)args[3]);
            }
            else if (command == "CustomChartSetLogarithmicScale")
            {
                SetLogarithmicScale((int)args[1] != 0, (double)args[2]);
            }
            else if (command == "CustomChartSetGridLine")
            {
                SetGridLine((int)args[1] != 0);
            }

            return 0;
        }

        //=== 크로스헤어: plotArea 경계 계산 ===
        private Rectangle GetPlotAreaBounds(int x1, int y1, int x2, int y2)
        {
            if (chartControl == null || chartControl.ChartAreas.Count == 0)
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);

            var ca = chartControl.ChartAreas[0];
            var pos = ca.Position;              // ChartArea 위치 (% of chart)
            var inner = ca.InnerPlotPosition;   // PlotArea 위치 (% of ChartArea)

            // InnerPlotPosition이 Auto이고 아직 계산되지 않은 경우 fallback
            if (inner.Width <= 0 || inner.Height <= 0)
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);

            int chartW = x2 - x1, chartH = y2 - y1;

            // ChartArea → 픽셀
            float caX = x1 + pos.X / 100f * chartW;
            float caY = y1 + pos.Y / 100f * chartH;
            float caW = pos.Width / 100f * chartW;
            float caH = pos.Height / 100f * chartH;

            // InnerPlotPosition → 픽셀 (ChartArea 기준)
            int plotX = (int)(caX + inner.X / 100f * caW);
            int plotY = (int)(caY + inner.Y / 100f * caH);
            int plotW = (int)(inner.Width / 100f * caW);
            int plotH = (int)(inner.Height / 100f * caH);

            return new Rectangle(plotX, plotY, plotW, plotH);
        }

        //=== 크로스헤어: 마우스 클릭 처리 ===
        public override async Task<bool> WmLeftButtonDown(Form form, MouseEventArgs e)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN &&
                objArgs?.chart?.bUseCrosshair == true)
            {
                int mx, my;
                GetMousePositionByRotate(e, out mx, out my);

                int objWidth = nRight - nLeft;
                int objHeight = nBottom - nTop;

                if (objWidth > 0 && objHeight > 0 &&
                    mx >= nLeft && mx < nRight && my >= nTop && my < nBottom)
                {
                    // plotArea 내부 클릭인지 확인
                    var plotRect = GetPlotAreaBounds(nLeft, nTop, nRight, nBottom);
                    if (!plotRect.Contains(mx, my))
                    {
                        // plotArea 밖 클릭 → 크로스헤어 숨김
                        if (_showCrosshair)
                        {
                            _showCrosshair = false;
                            _crosshairRelX = -1;
                            _crosshairRelY = -1;
                            _crosshairText = "";
                            InvalidateObject(form);
                        }
                        return await base.WmLeftButtonDown(form, e);
                    }

                    _crosshairRelX = (double)(mx - nLeft) / objWidth;
                    _crosshairRelY = (double)(my - nTop) / objHeight;
                    _showCrosshair = true;

                    // 클릭 위치의 데이터 값을 계산
                    UpdateCrosshairTooltip(
                        (int)(_crosshairRelX * objWidth),
                        (int)(_crosshairRelY * objHeight),
                        objWidth, objHeight);

                    InvalidateObject(form);
                }
            }

            return await base.WmLeftButtonDown(form, e);
        }

        //=== 크로스헤어: 우클릭으로 해제 ===
        public override async Task<bool> WmRightButtonDown(Form form, MouseEventArgs e)
        {
            if (_showCrosshair)
            {
                _showCrosshair = false;
                _crosshairRelX = -1;
                _crosshairRelY = -1;
                _crosshairText = "";
                InvalidateObject(form);
            }

            return await base.WmRightButtonDown(form, e);
        }

        //=== 크로스헤어: HitTest로 가장 가까운 데이터 포인트의 값을 찾아 툴팁 텍스트 생성 ===
        private void UpdateCrosshairTooltip(int pixelX, int pixelY, int chartWidth, int chartHeight)
        {
            _crosshairText = "";
            if (chartControl == null) return;

            lock (_chartLock)
            {
                try
                {
                    // HitTest를 위해 차트 크기를 맞춤
                    chartControl.Size = new Size(chartWidth, chartHeight);
                    if (!chartControl.IsHandleCreated)
                        chartControl.CreateControl();

                    // Chart.HitTest로 클릭 위치의 데이터 포인트 검색
                    HitTestResult result = chartControl.HitTest(pixelX, pixelY);

                    if (result.ChartElementType == ChartElementType.DataPoint &&
                        result.Series != null && result.PointIndex >= 0 &&
                        result.PointIndex < result.Series.Points.Count)
                    {
                        var pt = result.Series.Points[result.PointIndex];
                        string seriesName = result.Series.Name;
                        double yVal = pt.YValues[0];

                        // X 값 포맷
                        string xStr = "";
                        if (result.Series.XValueType == ChartValueType.DateTime && pt.XValue != 0)
                        {
                            xStr = DateTime.FromOADate(pt.XValue).ToString("HH:mm:ss");
                        }
                        else if (!string.IsNullOrEmpty(pt.AxisLabel))
                        {
                            xStr = pt.AxisLabel;
                        }

                        // 툴팁 텍스트 구성
                        var sb = new System.Text.StringBuilder();
                        if (!string.IsNullOrEmpty(xStr))
                            sb.AppendFormat("X: {0}", xStr);
                        sb.AppendFormat("{0}{1}: {2:F2}",
                            sb.Length > 0 ? "\n" : "",
                            seriesName, yVal);

                        _crosshairText = sb.ToString();
                    }
                    else
                    {
                        // 데이터 포인트가 아닌 영역: 축 값으로 표시 시도
                        if (chartControl.ChartAreas.Count > 0)
                        {
                            var area = chartControl.ChartAreas[0];
                            try
                            {
                                double xVal = area.AxisX.PixelPositionToValue(pixelX);
                                double yVal = area.AxisY.PixelPositionToValue(pixelY);
                                _crosshairText = string.Format("Y: {0:F2}", yVal);
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[CustomChart] UpdateCrosshairTooltip error: {ex.Message}");
                }
            }
        }

        //=== 크로스헤어: 오버레이 렌더링 (비트맵 캐시 위에 그림, plotArea 내부로 제한) ===
        private void DrawCrosshairOverlay(Graphics g, int x1, int y1, int x2, int y2)
        {
            int chartWidth = x2 - x1;
            int chartHeight = y2 - y1;
            if (chartWidth <= 0 || chartHeight <= 0) return;
            if (_crosshairRelX < 0 || _crosshairRelY < 0) return;

            int cx = x1 + (int)(_crosshairRelX * chartWidth);
            int cy = y1 + (int)(_crosshairRelY * chartHeight);

            // plotArea 경계 계산
            var plotRect = GetPlotAreaBounds(x1, y1, x2, y2);

            // 크로스헤어 좌표가 plotArea 밖이면 그리지 않음
            if (!plotRect.Contains(cx, cy)) return;

            // 크로스헤어 라인 (대시 스타일, plotArea 경계로 제한)
            using (var pen = new Pen(Color.FromArgb(160, 255, 80, 80), 1))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawLine(pen, cx, plotRect.Top, cx, plotRect.Bottom);    // 수직선 (plotArea 내)
                g.DrawLine(pen, plotRect.Left, cy, plotRect.Right, cy);    // 수평선 (plotArea 내)
            }

            // 교차점에 작은 원
            using (var brush = new SolidBrush(Color.FromArgb(200, 255, 80, 80)))
            {
                g.FillEllipse(brush, cx - 4, cy - 4, 8, 8);
            }

            // 툴팁 표시
            if (!string.IsNullOrEmpty(_crosshairText))
            {
                using (var font = new Font("맑은 고딕", 9f))
                using (var textBrush = new SolidBrush(Color.FromArgb(240, 40, 40, 40)))
                using (var bgBrush = new SolidBrush(Color.FromArgb(230, 255, 255, 235)))
                using (var borderPen = new Pen(Color.FromArgb(180, 160, 160, 120), 1))
                {
                    SizeF textSize = g.MeasureString(_crosshairText, font);
                    int padding = 5;
                    float tooltipW = textSize.Width + padding * 2;
                    float tooltipH = textSize.Height + padding * 2;

                    // 툴팁 위치 (plotArea 기준으로 조정)
                    float tx = cx + 12;
                    float ty = cy - tooltipH - 8;
                    if (tx + tooltipW > plotRect.Right) tx = cx - tooltipW - 12;
                    if (ty < plotRect.Top) ty = cy + 12;
                    if (tx < plotRect.Left) tx = plotRect.Left + 2;
                    if (ty + tooltipH > plotRect.Bottom) ty = plotRect.Bottom - tooltipH - 2;

                    RectangleF tooltipRect = new RectangleF(tx, ty, tooltipW, tooltipH);

                    // 그림자
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                    {
                        g.FillRectangle(shadowBrush,
                            tooltipRect.X + 2, tooltipRect.Y + 2,
                            tooltipRect.Width, tooltipRect.Height);
                    }

                    g.FillRectangle(bgBrush, tooltipRect);
                    g.DrawRectangle(borderPen, tx, ty, tooltipW, tooltipH);
                    g.DrawString(_crosshairText, font, textBrush, tx + padding, ty + padding);
                }
            }
        }

        //=== 리소스 해제 ===
        public override void Close()
        {
            arrayClassList.Remove(this);
            base.Close();
        }
    }
}

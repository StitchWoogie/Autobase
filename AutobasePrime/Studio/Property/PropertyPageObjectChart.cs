using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GraphicModule;
using Studio.Script;

namespace Studio
{
    /// <summary>
    /// Chart 오브젝트 속성 설정 폼.
    /// PropertyPageObjectMultiTrend 패턴 기반.
    /// 4개 Chart 타입 (RealTime, Minute, Milli, Custom) 공통 속성 페이지.
    /// </summary>
    public class PropertyPageObjectChart : Form
    {
        //=== 멤버 ===
        private int _chartTypeMode;  // 0=RealTime, 1=Minute, 2=Milli, 3=Custom
        private ScriptClass scriptEventAfterSettings;

        //=== UI 컨트롤 ===
        private TabControl tabMain;

        // 탭1: 기본 설정
        private TabPage tabBasic;
        private GroupBox groupDataSettings;
        private Label labelShowUnit;
        private NumericUpDown numShowUnit;
        private Label labelDataCycle;
        private NumericUpDown numDataCycle;
        private Label labelDataCycleUnit;
        private GroupBox groupTimeOption;
        private RadioButton radioMinute;
        private RadioButton radioHour;
        private RadioButton radioDay;
        private RadioButton radioMonth;
        // MilliTrend 전용 추가 옵션
        private RadioButton radioMs;
        private RadioButton radioSec;
        private RadioButton radioYear;

        // 탭2: 차트 설정
        private TabPage tabChart;
        private GroupBox groupChartType;
        private Label labelSeriesType;
        private ComboBox comboSeriesType;
        private CheckBox checkShowLegend;
        private CheckBox checkShowGrid;
        private CheckBox checkAntiAlias;
        private CheckBox check3DStyle;
        private Label labelTitle;
        private TextBox textTitle;
        private Label labelTitleSize;
        private NumericUpDown numTitleSize;
        private Label labelLegendPos;
        private ComboBox comboLegendPos;

        // 탭3: 색상 설정
        private TabPage tabColors;
        private Label labelChartBack;
        private Panel panelChartBack;
        private Button btnChartBack;
        private Label labelPlotBack;
        private Panel panelPlotBack;
        private Button btnPlotBack;
        private Label labelGridColor;
        private Panel panelGridColor;
        private Button btnGridColor;
        private Label labelTitleColor;
        private Panel panelTitleColor;
        private Button btnTitleColor;

        // 탭4: 고급 설정
        private TabPage tabAdvanced;
        private GroupBox groupLogarithmic;
        private CheckBox checkLogScale;
        private Label labelLogBase;
        private NumericUpDown numLogBase;
        private GroupBox groupToolbar;
        private CheckBox checkUseToolBar;
        private Label labelToolBarPos;
        private ComboBox comboToolBarPos;
        private Label labelBtnSize;
        private NumericUpDown numBtnSize;
        private CheckBox checkUseZoom;
        private CheckBox checkDontUseConfigDialog;
        private GroupBox groupChartFeatures;
        private CheckBox checkUseCrosshair;
        private CheckBox checkUseTagDescription;

        // (CustomTrend은 numDataCycle을 nDataTime으로 사용)

        // MilliTrend 전용
        private GroupBox groupMilliOptions;
        private Label labelDsn;
        private TextBox textDsn;
        private CheckBox checkAutoUpdate;

        // 스크립트 버튼
        private Button btnEventScript;

        // (확인/취소 버튼은 부모 PropertySheetPublic에서 처리)

        //=== 생성자 ===
        public PropertyPageObjectChart(int chartTypeMode)
        {
            _chartTypeMode = chartTypeMode;
            InitializeComponents();
            ConfigureForChartType();
        }

        //=== Chart 타입별 모드 문자열 ===
        private string GetChartTypeName()
        {
            switch (_chartTypeMode)
            {
                case 0: return "ChartRealTimeTrend";
                case 1: return "ChartMinuteTrend";
                case 2: return "ChartMilliTrend";
                case 3: return "CustomChart";
                default: return "Chart";
            }
        }

        //=== UI 초기화 ===
        private void InitializeComponents()
        {
            this.Text = GetChartTypeName() + " 속성";
            this.Name = "PropertyPageObjectChart";
            this.Size = new Size(520, 530);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ControlBox = false;

            tabMain = new TabControl();
            tabMain.Dock = DockStyle.Fill;
            tabMain.Padding = new Point(8, 4);

            //====================================
            // 탭1: 기본 설정
            //====================================
            tabBasic = new TabPage("기본 설정");
            int y = 10;

            // 데이터 설정 그룹
            groupDataSettings = new GroupBox();
            groupDataSettings.Text = "데이터 설정";
            groupDataSettings.Location = new Point(10, y);
            groupDataSettings.Size = new Size(470, 80);

            labelShowUnit = new Label();
            labelShowUnit.Text = "표시 개수:";
            labelShowUnit.Location = new Point(15, 25);
            labelShowUnit.AutoSize = true;

            numShowUnit = new NumericUpDown();
            numShowUnit.Location = new Point(100, 22);
            numShowUnit.Size = new Size(100, 22);
            numShowUnit.Minimum = 2;
            numShowUnit.Maximum = 44640;
            numShowUnit.Value = 100;

            labelDataCycle = new Label();
            labelDataCycle.Text = "데이터 주기:";
            labelDataCycle.Location = new Point(220, 25);
            labelDataCycle.AutoSize = true;

            numDataCycle = new NumericUpDown();
            numDataCycle.Location = new Point(310, 22);
            numDataCycle.Size = new Size(80, 22);
            numDataCycle.Minimum = 1;
            numDataCycle.Maximum = 60000;
            numDataCycle.Value = 1;

            labelDataCycleUnit = new Label();
            labelDataCycleUnit.Text = "";
            labelDataCycleUnit.Location = new Point(395, 25);
            labelDataCycleUnit.AutoSize = true;

            groupDataSettings.Controls.AddRange(new Control[] {
                labelShowUnit, numShowUnit, labelDataCycle, numDataCycle, labelDataCycleUnit
            });

            tabBasic.Controls.Add(groupDataSettings);
            y += 90;

            // 시간 옵션 그룹
            groupTimeOption = new GroupBox();
            groupTimeOption.Text = "시간 단위";
            groupTimeOption.Location = new Point(10, y);
            groupTimeOption.Size = new Size(470, 60);

            radioMs = new RadioButton();
            radioMs.Text = "ms";
            radioMs.Location = new Point(15, 25);
            radioMs.AutoSize = true;

            radioSec = new RadioButton();
            radioSec.Text = "초";
            radioSec.Location = new Point(75, 25);
            radioSec.AutoSize = true;

            radioMinute = new RadioButton();
            radioMinute.Text = "분";
            radioMinute.Location = new Point(130, 25);
            radioMinute.AutoSize = true;
            radioMinute.Checked = true;

            radioHour = new RadioButton();
            radioHour.Text = "시";
            radioHour.Location = new Point(185, 25);
            radioHour.AutoSize = true;

            radioDay = new RadioButton();
            radioDay.Text = "일";
            radioDay.Location = new Point(240, 25);
            radioDay.AutoSize = true;

            radioMonth = new RadioButton();
            radioMonth.Text = "월";
            radioMonth.Location = new Point(295, 25);
            radioMonth.AutoSize = true;

            radioYear = new RadioButton();
            radioYear.Text = "년";
            radioYear.Location = new Point(350, 25);
            radioYear.AutoSize = true;

            groupTimeOption.Controls.AddRange(new Control[] {
                radioMs, radioSec, radioMinute, radioHour, radioDay, radioMonth, radioYear
            });

            tabBasic.Controls.Add(groupTimeOption);
            y += 70;

            // MilliTrend 전용 옵션
            groupMilliOptions = new GroupBox();
            groupMilliOptions.Text = "MilliData 설정";
            groupMilliOptions.Location = new Point(10, y);
            groupMilliOptions.Size = new Size(470, 70);

            labelDsn = new Label();
            labelDsn.Text = "데이터 소스:";
            labelDsn.Location = new Point(15, 25);
            labelDsn.AutoSize = true;

            textDsn = new TextBox();
            textDsn.Location = new Point(100, 22);
            textDsn.Size = new Size(250, 22);

            checkAutoUpdate = new CheckBox();
            checkAutoUpdate.Text = "자동 갱신";
            checkAutoUpdate.Location = new Point(370, 24);
            checkAutoUpdate.AutoSize = true;

            groupMilliOptions.Controls.AddRange(new Control[] {
                labelDsn, textDsn, checkAutoUpdate
            });

            tabBasic.Controls.Add(groupMilliOptions);

            //====================================
            // 탭2: 차트 설정
            //====================================
            tabChart = new TabPage("차트 설정");

            groupChartType = new GroupBox();
            groupChartType.Text = "차트 타입";
            groupChartType.Location = new Point(10, 10);
            groupChartType.Size = new Size(470, 180);

            labelSeriesType = new Label();
            labelSeriesType.Text = "시리즈 타입:";
            labelSeriesType.Location = new Point(15, 25);
            labelSeriesType.AutoSize = true;

            comboSeriesType = new ComboBox();
            comboSeriesType.Location = new Point(100, 22);
            comboSeriesType.Size = new Size(150, 22);
            comboSeriesType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboSeriesType.Items.AddRange(new object[] {
                "Line", "FastLine", "Spline", "Area", "Column", "StepLine"
            });
            comboSeriesType.SelectedIndex = 0;

            checkShowLegend = new CheckBox();
            checkShowLegend.Text = "범례 표시";
            checkShowLegend.Location = new Point(15, 55);
            checkShowLegend.AutoSize = true;
            checkShowLegend.Checked = true;

            checkShowGrid = new CheckBox();
            checkShowGrid.Text = "그리드 표시";
            checkShowGrid.Location = new Point(130, 55);
            checkShowGrid.AutoSize = true;
            checkShowGrid.Checked = true;

            checkAntiAlias = new CheckBox();
            checkAntiAlias.Text = "안티앨리어싱";
            checkAntiAlias.Location = new Point(260, 55);
            checkAntiAlias.AutoSize = true;
            checkAntiAlias.Checked = true;

            check3DStyle = new CheckBox();
            check3DStyle.Text = "3D 스타일";
            check3DStyle.Location = new Point(380, 55);
            check3DStyle.AutoSize = true;

            labelTitle = new Label();
            labelTitle.Text = "제목:";
            labelTitle.Location = new Point(15, 85);
            labelTitle.AutoSize = true;

            textTitle = new TextBox();
            textTitle.Location = new Point(60, 82);
            textTitle.Size = new Size(250, 22);

            labelTitleSize = new Label();
            labelTitleSize.Text = "크기:";
            labelTitleSize.Location = new Point(320, 85);
            labelTitleSize.AutoSize = true;

            numTitleSize = new NumericUpDown();
            numTitleSize.Location = new Point(360, 82);
            numTitleSize.Size = new Size(60, 22);
            numTitleSize.Minimum = 6;
            numTitleSize.Maximum = 48;
            numTitleSize.Value = 12;

            labelLegendPos = new Label();
            labelLegendPos.Text = "범례 위치:";
            labelLegendPos.Location = new Point(15, 115);
            labelLegendPos.AutoSize = true;

            comboLegendPos = new ComboBox();
            comboLegendPos.Location = new Point(100, 112);
            comboLegendPos.Size = new Size(100, 22);
            comboLegendPos.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLegendPos.Items.AddRange(new object[] {
                "아래", "오른쪽", "위", "왼쪽"
            });
            comboLegendPos.SelectedIndex = 0;

            groupChartType.Controls.AddRange(new Control[] {
                labelSeriesType, comboSeriesType,
                checkShowLegend, checkShowGrid, checkAntiAlias, check3DStyle,
                labelTitle, textTitle, labelTitleSize, numTitleSize,
                labelLegendPos, comboLegendPos
            });

            tabChart.Controls.Add(groupChartType);

            //====================================
            // 탭3: 색상 설정
            //====================================
            tabColors = new TabPage("색상 설정");
            int cy = 15;

            // Chart 배경
            labelChartBack = new Label();
            labelChartBack.Text = "Chart 배경:";
            labelChartBack.Location = new Point(15, cy + 3);
            labelChartBack.AutoSize = true;

            panelChartBack = new Panel();
            panelChartBack.Location = new Point(120, cy);
            panelChartBack.Size = new Size(40, 25);
            panelChartBack.BackColor = Color.White;
            panelChartBack.BorderStyle = BorderStyle.FixedSingle;

            btnChartBack = new Button();
            btnChartBack.Text = "선택";
            btnChartBack.Location = new Point(170, cy);
            btnChartBack.Size = new Size(60, 25);
            btnChartBack.Click += (s, e) => SelectColor(panelChartBack);

            tabColors.Controls.AddRange(new Control[] {
                labelChartBack, panelChartBack, btnChartBack
            });
            cy += 35;

            // Plot 배경
            labelPlotBack = new Label();
            labelPlotBack.Text = "Plot 배경:";
            labelPlotBack.Location = new Point(15, cy + 3);
            labelPlotBack.AutoSize = true;

            panelPlotBack = new Panel();
            panelPlotBack.Location = new Point(120, cy);
            panelPlotBack.Size = new Size(40, 25);
            panelPlotBack.BackColor = Color.White;
            panelPlotBack.BorderStyle = BorderStyle.FixedSingle;

            btnPlotBack = new Button();
            btnPlotBack.Text = "선택";
            btnPlotBack.Location = new Point(170, cy);
            btnPlotBack.Size = new Size(60, 25);
            btnPlotBack.Click += (s, e) => SelectColor(panelPlotBack);

            tabColors.Controls.AddRange(new Control[] {
                labelPlotBack, panelPlotBack, btnPlotBack
            });
            cy += 35;

            // 그리드 색상
            labelGridColor = new Label();
            labelGridColor.Text = "그리드 색상:";
            labelGridColor.Location = new Point(15, cy + 3);
            labelGridColor.AutoSize = true;

            panelGridColor = new Panel();
            panelGridColor.Location = new Point(120, cy);
            panelGridColor.Size = new Size(40, 25);
            panelGridColor.BackColor = Color.FromArgb(200, 200, 200);
            panelGridColor.BorderStyle = BorderStyle.FixedSingle;

            btnGridColor = new Button();
            btnGridColor.Text = "선택";
            btnGridColor.Location = new Point(170, cy);
            btnGridColor.Size = new Size(60, 25);
            btnGridColor.Click += (s, e) => SelectColor(panelGridColor);

            tabColors.Controls.AddRange(new Control[] {
                labelGridColor, panelGridColor, btnGridColor
            });
            cy += 35;

            // 제목 색상
            labelTitleColor = new Label();
            labelTitleColor.Text = "제목 색상:";
            labelTitleColor.Location = new Point(15, cy + 3);
            labelTitleColor.AutoSize = true;

            panelTitleColor = new Panel();
            panelTitleColor.Location = new Point(120, cy);
            panelTitleColor.Size = new Size(40, 25);
            panelTitleColor.BackColor = Color.Black;
            panelTitleColor.BorderStyle = BorderStyle.FixedSingle;

            btnTitleColor = new Button();
            btnTitleColor.Text = "선택";
            btnTitleColor.Location = new Point(170, cy);
            btnTitleColor.Size = new Size(60, 25);
            btnTitleColor.Click += (s, e) => SelectColor(panelTitleColor);

            tabColors.Controls.AddRange(new Control[] {
                labelTitleColor, panelTitleColor, btnTitleColor
            });

            //====================================
            // 탭4: 고급 설정
            //====================================
            tabAdvanced = new TabPage("고급 설정");

            // 로그 스케일
            groupLogarithmic = new GroupBox();
            groupLogarithmic.Text = "로그 스케일";
            groupLogarithmic.Location = new Point(10, 10);
            groupLogarithmic.Size = new Size(470, 60);

            checkLogScale = new CheckBox();
            checkLogScale.Text = "로그 스케일 사용";
            checkLogScale.Location = new Point(15, 25);
            checkLogScale.AutoSize = true;
            checkLogScale.CheckedChanged += (s, e) =>
            {
                numLogBase.Enabled = checkLogScale.Checked;
            };

            labelLogBase = new Label();
            labelLogBase.Text = "Base:";
            labelLogBase.Location = new Point(200, 27);
            labelLogBase.AutoSize = true;

            numLogBase = new NumericUpDown();
            numLogBase.Location = new Point(250, 24);
            numLogBase.Size = new Size(80, 22);
            numLogBase.Minimum = 2;
            numLogBase.Maximum = 1000;
            numLogBase.Value = 10;
            numLogBase.Enabled = false;

            groupLogarithmic.Controls.AddRange(new Control[] {
                checkLogScale, labelLogBase, numLogBase
            });

            // 툴바
            groupToolbar = new GroupBox();
            groupToolbar.Text = "툴바 설정";
            groupToolbar.Location = new Point(10, 80);
            groupToolbar.Size = new Size(470, 90);

            checkUseToolBar = new CheckBox();
            checkUseToolBar.Text = "툴바 사용";
            checkUseToolBar.Location = new Point(15, 25);
            checkUseToolBar.AutoSize = true;

            labelToolBarPos = new Label();
            labelToolBarPos.Text = "위치:";
            labelToolBarPos.Location = new Point(150, 27);
            labelToolBarPos.AutoSize = true;

            comboToolBarPos = new ComboBox();
            comboToolBarPos.Location = new Point(190, 24);
            comboToolBarPos.Size = new Size(80, 22);
            comboToolBarPos.DropDownStyle = ComboBoxStyle.DropDownList;
            comboToolBarPos.Items.AddRange(new object[] { "상단", "하단", "좌측", "우측" });
            comboToolBarPos.SelectedIndex = 0;

            labelBtnSize = new Label();
            labelBtnSize.Text = "버튼 크기:";
            labelBtnSize.Location = new Point(290, 27);
            labelBtnSize.AutoSize = true;

            numBtnSize = new NumericUpDown();
            numBtnSize.Location = new Point(370, 24);
            numBtnSize.Size = new Size(60, 22);
            numBtnSize.Minimum = 16;
            numBtnSize.Maximum = 64;
            numBtnSize.Value = 16;

            checkUseZoom = new CheckBox();
            checkUseZoom.Text = "마우스 줌 사용";
            checkUseZoom.Location = new Point(15, 55);
            checkUseZoom.AutoSize = true;
            checkUseZoom.Checked = true;

            checkDontUseConfigDialog = new CheckBox();
            checkDontUseConfigDialog.Text = "설정 대화상자 비활성화";
            checkDontUseConfigDialog.Location = new Point(200, 55);
            checkDontUseConfigDialog.AutoSize = true;

            groupToolbar.Controls.AddRange(new Control[] {
                checkUseToolBar, labelToolBarPos, comboToolBarPos,
                labelBtnSize, numBtnSize, checkUseZoom, checkDontUseConfigDialog
            });

            // 차트 기능 그룹 (크로스헤어/태그설명 - 모든 차트 타입에서 사용)
            groupChartFeatures = new GroupBox();
            groupChartFeatures.Text = "차트 기능";
            groupChartFeatures.Location = new Point(10, 180);
            groupChartFeatures.Size = new Size(470, 55);

            checkUseCrosshair = new CheckBox();
            checkUseCrosshair.Text = "크로스헤어 사용";
            checkUseCrosshair.Location = new Point(15, 22);
            checkUseCrosshair.AutoSize = true;
            checkUseCrosshair.Checked = true;

            checkUseTagDescription = new CheckBox();
            checkUseTagDescription.Text = "태그 설명으로 표시";
            checkUseTagDescription.Location = new Point(200, 22);
            checkUseTagDescription.AutoSize = true;
            checkUseTagDescription.Visible = false; // Custom 모드에서만 표시

            groupChartFeatures.Controls.AddRange(new Control[] {
                checkUseCrosshair, checkUseTagDescription
            });

            // 스크립트 버튼
            btnEventScript = new Button();
            btnEventScript.Text = "설정 후 이벤트 스크립트...";
            btnEventScript.Location = new Point(10, 245);
            btnEventScript.Size = new Size(200, 30);
            btnEventScript.Click += BtnEventScript_Click;

            tabAdvanced.Controls.Add(groupLogarithmic);
            tabAdvanced.Controls.Add(groupToolbar);
            tabAdvanced.Controls.Add(groupChartFeatures);
            tabAdvanced.Controls.Add(btnEventScript);

            //====================================
            // 탭 구성
            //====================================
            tabMain.TabPages.Add(tabBasic);
            tabMain.TabPages.Add(tabChart);
            tabMain.TabPages.Add(tabColors);
            tabMain.TabPages.Add(tabAdvanced);

            // 탭을 전체 영역에 배치 (버튼은 부모 PropertySheetPublic에서 관리)
            this.Controls.Add(tabMain);
        }

        //=== 차트 타입에 따른 UI 가시성 설정 ===
        private void ConfigureForChartType()
        {
            switch (_chartTypeMode)
            {
                case 0: // RealTime
                    groupTimeOption.Visible = false;
                    groupMilliOptions.Visible = false;
                    labelDataCycleUnit.Text = "ms";
                    numDataCycle.Minimum = 100;
                    numDataCycle.Maximum = 60000;
                    numDataCycle.Value = 1000;
                    numShowUnit.Minimum = 10;
                    numShowUnit.Maximum = 36000;
                    break;

                case 1: // Minute
                    radioMs.Visible = false;
                    radioSec.Visible = false;
                    radioYear.Visible = false;
                    groupMilliOptions.Visible = false;
                    labelDataCycleUnit.Text = "";
                    numDataCycle.Minimum = 1;
                    numDataCycle.Maximum = 1000;
                    break;

                case 2: // Milli
                    numDataCycle.Minimum = 1;
                    numDataCycle.Maximum = 1000;
                    break;

                case 3: // Custom
                    groupTimeOption.Visible = false;
                    groupMilliOptions.Visible = false;
                    // CustomTrend: 데이터 수집 간격 (nDataTime)
                    labelDataCycle.Text = "수집 간격:";
                    labelDataCycleUnit.Text = "ms";
                    numDataCycle.Minimum = 100;
                    numDataCycle.Maximum = 60000;
                    numDataCycle.Value = 1000;
                    // Pie/Doughnut/Radar 시리즈 타입 추가
                    comboSeriesType.Items.Add("Pie");
                    comboSeriesType.Items.Add("Doughnut");
                    comboSeriesType.Items.Add("Radar");
                    // CustomTrend: 색상 탭 제거 (기존 트렌드 색상 속성 사용)
                    tabMain.TabPages.Remove(tabColors);
                    // CustomTrend: 툴바/스크립트 제거 (사용 안함)
                    groupToolbar.Visible = false;
                    btnEventScript.Visible = false;
                    // 차트 기능 그룹을 툴바 위치로 이동 (툴바가 숨겨졌으므로)
                    groupChartFeatures.Location = new Point(10, 80);
                    // 태그 설명 옵션 활성화 (Custom 모드에서만 유효)
                    checkUseTagDescription.Visible = true;
                    break;
            }
        }

        //=== 색상 선택 대화상자 ===
        private void SelectColor(Panel colorPanel)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.Color = colorPanel.BackColor;
                dlg.FullOpen = true;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    colorPanel.BackColor = dlg.Color;
                }
            }
        }

        //=== 스크립트 편집 ===
        private void BtnEventScript_Click(object sender, EventArgs e)
        {
            try
            {
                FormScriptEditor dlg = new FormScriptEditor();
                dlg.SetScript("EventAfterSettings", scriptEventAfterSettings);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    scriptEventAfterSettings = dlg.GetScript();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[PropertyPageObjectChart] Script editor error: {ex.Message}");
            }
        }

        //===================================================================
        // ObjectArgs 속성: UI ↔ 데이터 양방향 바인딩
        // 원본 Args를 저장한 후, getter에서 UI 값만 갱신하여 반환.
        // 이렇게 하면 PropertyPage에서 관리하지 않는 필드(pub, lColor*, bDisplayByTime 등)도 보존됨.
        //===================================================================

        private ObjectArgsChartRealTime _origRealTime;
        private ObjectArgsChartMinute _origMinute;
        private ObjectArgsChartMilli _origMilli;
        private ObjectArgsChartCustom _origCustom;

        #region ChartRealTime Args

        public ObjectArgsChartRealTime ArgsRealTime
        {
            get
            {
                var args = _origRealTime ?? new ObjectArgsChartRealTime();
                args.wShowUnit = (int)numShowUnit.Value;
                args.nDataTime = (int)numDataCycle.Value;
                FillChartCommon(args.chart);
                FillColors(args.chart);
                args.logarithmicScale = GetLogarithmicScale();
                args.chart.scriptEventAfterSettings = scriptEventAfterSettings;
                return args;
            }
            set
            {
                if (value == null) return;
                _origRealTime = value;
                numShowUnit.Value = ClampDecimal(value.wShowUnit, numShowUnit.Minimum, numShowUnit.Maximum);
                numDataCycle.Value = ClampDecimal(value.nDataTime, numDataCycle.Minimum, numDataCycle.Maximum);
                LoadChartCommon(value.chart);
                LoadColors(value.chart);
                LoadLogarithmicScale(value.logarithmicScale);
                scriptEventAfterSettings = value.chart?.scriptEventAfterSettings;
            }
        }

        #endregion

        #region ChartMinute Args

        public ObjectArgsChartMinute ArgsMinute
        {
            get
            {
                var args = _origMinute ?? new ObjectArgsChartMinute();
                args.wShowUnit = (int)numShowUnit.Value;
                args.nDataCycle = (int)numDataCycle.Value;
                args.wTimeSelectOption = GetSelectedTimeOption();
                FillChartCommon(args.chart);
                FillColors(args.chart);
                args.logarithmicScale = GetLogarithmicScale();
                args.chart.scriptEventAfterSettings = scriptEventAfterSettings;
                return args;
            }
            set
            {
                if (value == null) return;
                _origMinute = value;
                numShowUnit.Value = ClampDecimal(value.wShowUnit, numShowUnit.Minimum, numShowUnit.Maximum);
                numDataCycle.Value = ClampDecimal(value.nDataCycle, numDataCycle.Minimum, numDataCycle.Maximum);
                SetSelectedTimeOption(value.wTimeSelectOption);
                LoadChartCommon(value.chart);
                LoadColors(value.chart);
                LoadLogarithmicScale(value.logarithmicScale);
                scriptEventAfterSettings = value.chart?.scriptEventAfterSettings;
            }
        }

        #endregion

        #region ChartMilli Args

        public ObjectArgsChartMilli ArgsMilli
        {
            get
            {
                var args = _origMilli ?? new ObjectArgsChartMilli();
                args.wShowUnit = (int)numShowUnit.Value;
                args.nDataCycle = (int)numDataCycle.Value;
                args.wTimeSelectOption = GetSelectedTimeOptionMilli();
                args.sDsn = textDsn.Text;
                args.bAutoUpdate = checkAutoUpdate.Checked;
                FillChartCommon(args.chart);
                FillColors(args.chart);
                args.logarithmicScale = GetLogarithmicScale();
                args.chart.scriptEventAfterSettings = scriptEventAfterSettings;
                return args;
            }
            set
            {
                if (value == null) return;
                _origMilli = value;
                numShowUnit.Value = ClampDecimal(value.wShowUnit, numShowUnit.Minimum, numShowUnit.Maximum);
                numDataCycle.Value = ClampDecimal(value.nDataCycle, numDataCycle.Minimum, numDataCycle.Maximum);
                SetSelectedTimeOptionMilli(value.wTimeSelectOption);
                textDsn.Text = value.sDsn ?? "";
                checkAutoUpdate.Checked = value.bAutoUpdate;
                LoadChartCommon(value.chart);
                LoadColors(value.chart);
                LoadLogarithmicScale(value.logarithmicScale);
                scriptEventAfterSettings = value.chart?.scriptEventAfterSettings;
            }
        }

        #endregion

        #region ChartCustom Args

        public ObjectArgsChartCustom ArgsCustom
        {
            get
            {
                var args = _origCustom ?? new ObjectArgsChartCustom();
                args.wShowUnit = (int)numShowUnit.Value;
                args.nDataTime = (int)numDataCycle.Value;
                FillChartCommon(args.chart);
                // CustomTrend: 색상은 기존 트렌드 색상 속성(채움색/배경색/눈금색/글자색)에서 매핑
                // FillColors 호출 안함 (색상 탭 제거됨)
                args.logarithmicScale = GetLogarithmicScale();
                // CustomTrend: 스크립트 사용 안함
                return args;
            }
            set
            {
                if (value == null) return;
                _origCustom = value;
                numShowUnit.Value = ClampDecimal(value.wShowUnit, numShowUnit.Minimum, numShowUnit.Maximum);
                numDataCycle.Value = ClampDecimal(value.nDataTime, numDataCycle.Minimum, numDataCycle.Maximum);
                LoadChartCommon(value.chart);
                // CustomTrend: 색상은 기존 트렌드 색상 속성에서 매핑 (LoadColors 호출 안함)
                LoadLogarithmicScale(value.logarithmicScale);
            }
        }

        #endregion

        //=== 공통 헬퍼 메서드 ===

        private void FillChartCommon(ObjectArgsChartCommon chart)
        {
            if (chart == null) return;
            chart.nDefaultSeriesType = comboSeriesType.SelectedIndex;
            chart.bShowLegend = checkShowLegend.Checked;
            chart.bShowGrid = checkShowGrid.Checked;
            chart.bAntiAlias = checkAntiAlias.Checked;
            chart.b3DStyle = check3DStyle.Checked;
            chart.sTitle = textTitle.Text;
            chart.nTitleSize = (int)numTitleSize.Value;
            chart.nLegendPosition = comboLegendPos.SelectedIndex;
            chart.bUseToolBar = checkUseToolBar.Checked;
            chart.nToolBarPos = comboToolBarPos.SelectedIndex;
            chart.nToolBarButtonSize = (int)numBtnSize.Value;
            chart.bUseMouseButtonAsZoom = checkUseZoom.Checked;
            chart.bDontUseConfigDialog = checkDontUseConfigDialog.Checked;
            chart.bUseCrosshair = checkUseCrosshair.Checked;
            chart.bUseTagDescription = checkUseTagDescription.Checked;
        }

        private void LoadChartCommon(ObjectArgsChartCommon chart)
        {
            if (chart == null) return;
            comboSeriesType.SelectedIndex = Math.Min(chart.nDefaultSeriesType,
                comboSeriesType.Items.Count - 1);
            checkShowLegend.Checked = chart.bShowLegend;
            checkShowGrid.Checked = chart.bShowGrid;
            checkAntiAlias.Checked = chart.bAntiAlias;
            check3DStyle.Checked = chart.b3DStyle;
            textTitle.Text = chart.sTitle ?? "";
            numTitleSize.Value = ClampDecimal(chart.nTitleSize, 6, 48);
            comboLegendPos.SelectedIndex = Math.Min(chart.nLegendPosition,
                comboLegendPos.Items.Count - 1);
            checkUseToolBar.Checked = chart.bUseToolBar;
            comboToolBarPos.SelectedIndex = Math.Min(chart.nToolBarPos,
                comboToolBarPos.Items.Count - 1);
            numBtnSize.Value = ClampDecimal(chart.nToolBarButtonSize, 16, 64);
            checkUseZoom.Checked = chart.bUseMouseButtonAsZoom;
            checkDontUseConfigDialog.Checked = chart.bDontUseConfigDialog;
            checkUseCrosshair.Checked = chart.bUseCrosshair;
            checkUseTagDescription.Checked = chart.bUseTagDescription;
        }

        private void FillColors(ObjectArgsChartCommon chart)
        {
            if (chart == null) return;
            chart.colorChartBack = panelChartBack.BackColor;
            chart.colorPlotBack = panelPlotBack.BackColor;
            chart.colorGrid = panelGridColor.BackColor;
            chart.colorTitle = panelTitleColor.BackColor;
        }

        private void LoadColors(ObjectArgsChartCommon chart)
        {
            if (chart == null) return;
            panelChartBack.BackColor = chart.colorChartBack;
            panelPlotBack.BackColor = chart.colorPlotBack;
            panelGridColor.BackColor = chart.colorGrid;
            panelTitleColor.BackColor = chart.colorTitle;
        }

        private LogarithmicScale GetLogarithmicScale()
        {
            return new LogarithmicScale
            {
                bUse = checkLogScale.Checked,
                fBase = (double)numLogBase.Value
            };
        }

        private void LoadLogarithmicScale(LogarithmicScale log)
        {
            if (log == null) return;
            checkLogScale.Checked = log.bUse;
            numLogBase.Value = ClampDecimal((decimal)log.fBase, 2, 1000);
            numLogBase.Enabled = log.bUse;
        }

        private int GetSelectedTimeOption()
        {
            if (radioMinute.Checked) return 0;
            if (radioHour.Checked) return 1;
            if (radioDay.Checked) return 2;
            if (radioMonth.Checked) return 3;
            return 0;
        }

        private void SetSelectedTimeOption(int option)
        {
            switch (option)
            {
                case 0: radioMinute.Checked = true; break;
                case 1: radioHour.Checked = true; break;
                case 2: radioDay.Checked = true; break;
                case 3: radioMonth.Checked = true; break;
            }
        }

        private int GetSelectedTimeOptionMilli()
        {
            if (radioMs.Checked) return 0;
            if (radioSec.Checked) return 1;
            if (radioMinute.Checked) return 2;
            if (radioHour.Checked) return 3;
            if (radioDay.Checked) return 4;
            if (radioMonth.Checked) return 5;
            if (radioYear.Checked) return 6;
            return 0;
        }

        private void SetSelectedTimeOptionMilli(int option)
        {
            switch (option)
            {
                case 0: radioMs.Checked = true; break;
                case 1: radioSec.Checked = true; break;
                case 2: radioMinute.Checked = true; break;
                case 3: radioHour.Checked = true; break;
                case 4: radioDay.Checked = true; break;
                case 5: radioMonth.Checked = true; break;
                case 6: radioYear.Checked = true; break;
            }
        }

        private decimal ClampDecimal(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}

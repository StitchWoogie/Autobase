using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System.Diagnostics;
using NetTools.OldDefine;
using NetTools;
using AutoLibLocal;
using AutoLib;

namespace GraphicModule
{
    /// <summary>
    /// PostgreSQL 기반 밀리초 데이터 뷰어 (현대적 UI 적용)
    /// </summary>
    public partial class MilliDataWnd : Form
    {
        #region 상수 및 설정
        private const string HISTORY_SCHEMA = "history";
        private const int MAX_VIEW_ACTIVE = 1000;
        private const int MAX_DEFINED_COLOR = 6;

        public static int nMilliDataBackColor = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMilliDataBackColor", 0);
        public static bool bMilliDataTagDescription = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bMilliDataTagDescription", false);
        #endregion

        #region 필드
        private int m_nTimeType;
        private int cyChar;
        private int cxChar;
        private SYSTEMTIME stStart = new SYSTEMTIME();
        private SYSTEMTIME stEnd = new SYSTEMTIME();
        private int nTimeInterval;
        private DataTable dtData = new DataTable();
        private DataTable dtMember = new DataTable();

        private Font _safeFont; //Font dispose 방지 캐시.

        // 그래프 영역
        private int gx1, gy1, gx2, gy2;
        private int tx1, ty1, tx2, ty2;

        // 스크롤 관련
        private int nLimitX;
        private int nScrollPosX;
        private int nScrollMaxX;

        // 커서 및 선택
        private int nCursorPos;
        private bool bCursorFlag;
        private int nDesTag;
        private int nSelectX1, nSelectX2;

        // 더블 버퍼링 (깜빡임 방지)
        private Bitmap bmTotal;
        private Bitmap backBuffer;
        public bool bBitmapFill;

        // 태그 활성화 상태
        private sbyte[] bActive = new sbyte[MAX_VIEW_ACTIVE];
        private int nRecordCount;

        // 마우스 이벤트
        private static int nStartX, nEndX;
        private static bool bMouseCapture;

        // PostgreSQL 연결
        private string connectionString;
        private string currentTableName;

        // 현대적 UI 컨트롤
        private HScrollBar scrollHorz;
        private Panel controlPanel;
        private DateTimePicker dateTimeEnd;
        private DateTimePicker dateTimeStart;
        private Label lblEnd;
        private Label lblTable;
        private Label lblStart;
        private ComboBox comboTableSelect;
        private Button btnLoad;
        private Button buttonConfig;
        private Button buttonClose;
        #endregion

        #region 색상 정의
        private Color[] definedColor = {
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(0, 255, 255),
            Color.FromArgb(255, 0, 255),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(255, 0, 0),
        };
        #endregion

        #region 태그바 관련 필드 추가
        private int tagScrollOffset = 0;  // 태그 스크롤 오프셋
        private int visibleTagCount = 0;  // 화면에 보이는 태그 개수
        private bool isTagScrollMode = false; // 태그 스크롤 모드 여부
        private const int TAG_WIDTH = 120;    // 태그 박스 폭 (기존 cxChar * 15 대신)
        private const int TAG_HEIGHT = 20;    // 태그 박스 높이
        private VScrollBar tagVerticalScroll; // 세로 스크롤바 (옵션 3용)
        #endregion

        #region 생성자 및 초기화
        public MilliDataWnd()
        {
            // 더블 버퍼링 활성화 (깜빡임 방지)
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            InitializeComponent();

            _safeFont = new Font(this.Font.FontFamily, this.Font.Size, this.Font.Style);

            nTimeInterval = 0;
            nRecordCount = 0;
            nLimitX = 50;
            nScrollPosX = 0;
            nScrollMaxX = 0;
            nCursorPos = 0;
            bCursorFlag = false;
            nDesTag = 0;
            nSelectX1 = 0;
            nSelectX2 = 0;
            bBitmapFill = false;
            m_nTimeType = 1; // 절대시간 기본

            // 태그 스크롤 관련 초기화
            tagScrollOffset = 0;
            visibleTagCount = 0;
            isTagScrollMode = false;

            for (int i = 0; i < MAX_VIEW_ACTIVE; i++)
                bActive[i] = 1;

            // PostgreSQL 연결 문자열 설정
            connectionString = GetConnectionString();

            dateTimeStart.Value = DateTime.Today;
            dateTimeEnd.Value = DateTime.Now;

            if (Tools.IsLangKorean())
            {
                buttonConfig.Text = "설정";
                btnLoad.Text = "불러오기";
                lblStart.Text = "시작시간";
                lblEnd.Text = "종료시간";
                lblTable.Text = "미세자료";
                buttonClose.Text = "닫기";
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MilliDataWnd));
            this.scrollHorz = new System.Windows.Forms.HScrollBar();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.comboTableSelect = new System.Windows.Forms.ComboBox();
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimeStart = new System.Windows.Forms.DateTimePicker();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.lblTable = new System.Windows.Forms.Label();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrollHorz
            // 
            resources.ApplyResources(this.scrollHorz, "scrollHorz");
            this.scrollHorz.Name = "scrollHorz";
            this.scrollHorz.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollHorz_Scroll);
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.controlPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controlPanel.Controls.Add(this.buttonClose);
            this.controlPanel.Controls.Add(this.btnLoad);
            this.controlPanel.Controls.Add(this.comboTableSelect);
            this.controlPanel.Controls.Add(this.dateTimeEnd);
            this.controlPanel.Controls.Add(this.dateTimeStart);
            this.controlPanel.Controls.Add(this.lblEnd);
            this.controlPanel.Controls.Add(this.lblStart);
            this.controlPanel.Controls.Add(this.lblTable);
            resources.ApplyResources(this.controlPanel, "controlPanel");
            this.controlPanel.Name = "controlPanel";
            // 
            // buttonClose
            // 
            this.buttonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.buttonClose.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.ForeColor = System.Drawing.Color.White;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnLoad.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // comboTableSelect
            // 
            this.comboTableSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboTableSelect, "comboTableSelect");
            this.comboTableSelect.FormattingEnabled = true;
            this.comboTableSelect.Name = "comboTableSelect";
            this.comboTableSelect.SelectedIndexChanged += new System.EventHandler(this.ComboTableSelect_SelectedIndexChanged);
            // 
            // dateTimeEnd
            // 
            resources.ApplyResources(this.dateTimeEnd, "dateTimeEnd");
            this.dateTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Value = new System.DateTime(2025, 9, 30, 14, 31, 19, 0);
            // 
            // dateTimeStart
            // 
            resources.ApplyResources(this.dateTimeStart, "dateTimeStart");
            this.dateTimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeStart.Name = "dateTimeStart";
            this.dateTimeStart.Value = new System.DateTime(2025, 9, 30, 14, 43, 17, 0);
            // 
            // lblEnd
            // 
            resources.ApplyResources(this.lblEnd, "lblEnd");
            this.lblEnd.Name = "lblEnd";
            // 
            // lblStart
            // 
            resources.ApplyResources(this.lblStart, "lblStart");
            this.lblStart.Name = "lblStart";
            // 
            // lblTable
            // 
            resources.ApplyResources(this.lblTable, "lblTable");
            this.lblTable.Name = "lblTable";
            // 
            // buttonConfig
            // 
            this.buttonConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.buttonConfig.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.buttonConfig, "buttonConfig");
            this.buttonConfig.ForeColor = System.Drawing.Color.White;
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.UseVisualStyleBackColor = false;
            this.buttonConfig.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // MilliDataWnd
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.buttonConfig);
            this.Controls.Add(this.scrollHorz);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MilliDataWnd";
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.ResumeLayout(false);

        }

      

        private string GetConnectionString()
        {
            // 실제 환경에서는 설정 파일에서 읽어오도록 구현
            return ConfigDataDB.sConnectionString;
        }
        #endregion

        #region PostgreSQL 데이터 로드
        private async void BtnLoad_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentTableName))
            {
                MessageBox.Show("테이블을 선택해주세요.");
                return;
            }

            await LoadDataFromPostgreSQL();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadTableList();
        }

        private async Task LoadTableList()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = $@"
                        SELECT table_name 
                        FROM information_schema.tables 
                        WHERE table_schema = '{HISTORY_SCHEMA}' 
                        AND table_name LIKE 'millidata_%'
                        AND table_name != 'millidata_tag_metadata'
                        AND table_name != 'millidata_metadata'
                        ORDER BY table_name";

                    using (var command = new NpgsqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        comboTableSelect.Items.Clear();

                        while (await reader.ReadAsync())
                        {
                            comboTableSelect.Items.Add(reader.GetString(0));
                        }

                        if (comboTableSelect.Items.Count > 0)
                        {
                            comboTableSelect.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"테이블 목록 로드 오류: {ex.Message}");
            }
        }

        private void ComboTableSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentTableName = comboTableSelect.SelectedItem?.ToString();
        }

        private async Task LoadDataFromPostgreSQL()
        {
            if (string.IsNullOrEmpty(currentTableName)) return;

            try
            {
                DataClose();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // 1. 태그 메타데이터 로드
                    await LoadTagMetadata(connection);

                    // 2. 데이터 로드
                    await LoadHistoryData(connection);
                }

                // UI 업데이트
                if (nRecordCount < 50) nLimitX = nRecordCount;
                else nLimitX = 50;

                nScrollPosX = 0;
                ScrollUpdate();
                SetSelectZone(nScrollPosX, nScrollPosX + nLimitX - 1);

                bBitmapFill = false;
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 오류: {ex.Message}");
            }
        }

        private async Task LoadTagMetadata(NpgsqlConnection connection)
        {
            // 테이블명은 파라미터로 바인딩할 수 없으므로 문자열 보간 사용
            // SQL 인젝션 방지를 위해 테이블명 검증 필요
            if (string.IsNullOrEmpty(currentTableName) || !IsValidTableName(currentTableName))
            {
                throw new ArgumentException("유효하지 않은 테이블명입니다.");
            }

            string query = $@"
                SELECT tag_name, tag_type, full_scale, base_value, column_name
                FROM {HISTORY_SCHEMA}.millidata_tag_metadata 
                WHERE table_name = @tableName
                ORDER BY tag_name";

            dtMember = new DataTable();
            dtMember.Columns.Add("Tag", typeof(string));
            dtMember.Columns.Add("TagType", typeof(int));
            dtMember.Columns.Add("Full", typeof(double));
            dtMember.Columns.Add("Base", typeof(double));
            dtMember.Columns.Add("ColumnName", typeof(string));

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tableName", currentTableName);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = dtMember.NewRow();
                        row["Tag"] = reader.GetString(reader.GetOrdinal("tag_name"));
                        row["TagType"] = reader.GetInt32(reader.GetOrdinal("tag_type"));
                        row["Full"] = reader.GetDouble(reader.GetOrdinal("full_scale"));
                        row["Base"] = reader.GetDouble(reader.GetOrdinal("base_value"));
                        row["ColumnName"] = reader.GetString(reader.GetOrdinal("column_name"));
                        dtMember.Rows.Add(row);
                    }
                }
            }
        }


        private bool IsValidTableName(string tableName)
        {
            // 테이블명 검증: 영문자, 숫자, 언더스코어만 허용
            return System.Text.RegularExpressions.Regex.IsMatch(tableName, @"^[a-zA-Z][a-zA-Z0-9_]*$");
        }

        private async Task LoadHistoryData(NpgsqlConnection connection)
        {
            if (string.IsNullOrEmpty(currentTableName) || !IsValidTableName(currentTableName))
            {
                throw new ArgumentException("유효하지 않은 테이블명입니다.");
            }

            DateTime startTime = dateTimeStart.Value;
            DateTime endTime = dateTimeEnd.Value;

            string baseQuery = $@"
                SELECT data_time, interval_ms";

            // 동적으로 컬럼 추가
            foreach (DataRow row in dtMember.Rows)
            {
                string columnName = row["ColumnName"].ToString();
                // 컬럼명도 검증 필요
                if (!IsValidColumnName(columnName))
                    continue;
                baseQuery += $", {columnName}";
            }

            baseQuery += $@"
                FROM {HISTORY_SCHEMA}.{currentTableName}
                WHERE data_time >= @startTime AND data_time <= @endTime
                ORDER BY data_time";

            dtData = new DataTable();
            dtData.Columns.Add("data_time", typeof(DateTime));
            dtData.Columns.Add("interval_ms", typeof(int));

            // 태그별 컬럼 추가
            foreach (DataRow row in dtMember.Rows)
            {
                string tagName = row["Tag"].ToString();
                int tagType = Convert.ToInt32(row["TagType"]);

                if (tagType == 0) // AI
                    dtData.Columns.Add(tagName, typeof(double));
                else if (tagType == 1) // DI
                    dtData.Columns.Add(tagName, typeof(int));
                else // ST
                    dtData.Columns.Add(tagName, typeof(string));
            }

            using (var command = new NpgsqlCommand(baseQuery, connection))
            {
                command.Parameters.AddWithValue("@startTime", startTime);
                command.Parameters.AddWithValue("@endTime", endTime);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    bool firstRecord = true;

                    while (await reader.ReadAsync())
                    {
                        var dataRow = dtData.NewRow();

                        // DateTime을 로컬 시간으로 변환
                        DateTime recordTime = reader.GetDateTime(reader.GetOrdinal("data_time"));

                        // UTC로 저장된 경우 로컬 시간으로 변환
                        if (recordTime.Kind == DateTimeKind.Utc)
                        {
                            recordTime = recordTime.ToLocalTime();
                        }
                        else if (recordTime.Kind == DateTimeKind.Unspecified)
                        {
                            // Unspecified인 경우 UTC로 간주하고 로컬 시간으로 변환
                            recordTime = DateTime.SpecifyKind(recordTime, DateTimeKind.Utc).ToLocalTime();
                        }

                        int intervalMs = reader.GetInt32(reader.GetOrdinal("interval_ms"));

                        dataRow["data_time"] = recordTime;
                        dataRow["interval_ms"] = intervalMs;

                        if (firstRecord)
                        {
                            stStart.Set(recordTime);
                            nTimeInterval = intervalMs;
                            firstRecord = false;
                        }

                        // 태그 데이터 읽기
                        foreach (DataRow memberRow in dtMember.Rows)
                        {
                            string tagName = memberRow["Tag"].ToString();
                            string columnName = memberRow["ColumnName"].ToString();
                            int tagType = Convert.ToInt32(memberRow["TagType"]);

                            try
                            {
                                int columnOrdinal = reader.GetOrdinal(columnName);

                                if (!reader.IsDBNull(columnOrdinal))
                                {
                                    if (tagType == 0) // AI
                                        dataRow[tagName] = reader.GetDouble(columnOrdinal);
                                    else if (tagType == 2) // DI
                                        dataRow[tagName] = reader.GetInt32(columnOrdinal);
                                    else // ST
                                        dataRow[tagName] = reader.GetString(columnOrdinal);
                                }
                                else
                                {
                                    if (tagType == 0) // AI
                                        dataRow[tagName] = 0.0;
                                    else if (tagType == 2) // DI
                                        dataRow[tagName] = 0;
                                    else // ST
                                        dataRow[tagName] = "";
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"컬럼 {columnName} 읽기 오류: {ex.Message}");
                                // 기본값 설정
                                if (tagType == 0) // AI
                                    dataRow[tagName] = 0.0;
                                else if (tagType == 2) // DI
                                    dataRow[tagName] = 0;
                                else // ST
                                    dataRow[tagName] = "";
                            }
                        }

                        dtData.Rows.Add(dataRow);
                    }
                }
            }

            nRecordCount = dtData.Rows.Count;

            if (nRecordCount > 0)
            {
                stEnd.Set((DateTime)dtData.Rows[nRecordCount - 1]["data_time"]);
            }
        }

        private bool IsValidColumnName(string columnName)
        {
            // 컬럼명 검증: 영문자, 숫자, 언더스코어만 허용
            return !string.IsNullOrEmpty(columnName) &&
                   System.Text.RegularExpressions.Regex.IsMatch(columnName, @"^[a-zA-Z][a-zA-Z0-9_]*$");
        }

        #endregion

        #region 기존 그래프 그리기 메소드들 (UI 개선)
        private Color GetMilliDataBackColor()
        {
            return nMilliDataBackColor == 1 ? Color.White : Color.FromArgb(20, 20, 20); // 다크 테마 적용
        }

        private Color GetDefinedColor(int no)
        {
            no %= MAX_DEFINED_COLOR;
            Color color = definedColor[no];

            if (nMilliDataBackColor == 1)
            {
                int r = color.R > 0 ? 0x80 : 0;
                int g = color.G > 0 ? 0x80 : 0;
                int b = color.B > 0 ? 0x80 : 0;
                return Color.FromArgb(r, g, b);
            }

            return color;
        }

       protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                // 백버퍼에 그리기
                if (backBuffer == null || backBuffer.Width != this.ClientRectangle.Width ||
                    backBuffer.Height != this.ClientRectangle.Height)
                {
                    backBuffer?.Dispose();
                    backBuffer = new Bitmap(Math.Max(1, this.ClientRectangle.Width),
                                           Math.Max(1, this.ClientRectangle.Height));
                }

                using (Graphics g = Graphics.FromImage(backBuffer))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    RECT rect = new RECT();
                    rect.Set(this.ClientRectangle);

                    // 배경 그리기
                    using (var brush = new SolidBrush(GetMilliDataBackColor()))
                    {
                        g.FillRectangle(brush, rect.left, 0,
                                       rect.right, rect.bottom);
                    }

                    DrawModernHeader(g);
                    UpdateGraphBounds();

                    using (var brush = new SolidBrush(GetMilliDataBackColor()))
                    {
                        g.FillRectangle(brush, gx1, gy1, gx2 - gx1, gy2 - gy1);
                    }

                    DrawBarY(g);
                    DrawBarX(g);

                    if (!bBitmapFill && bmTotal != null)
                    {
                        using (Graphics gb = Graphics.FromImage(bmTotal))
                        {
                            gb.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            using (var brush = new SolidBrush(GetMilliDataBackColor()))
                            {
                                gb.FillRectangle(brush, 0, 0, tx2 - tx1, ty2 - ty1);
                            }
                            DrawGraph(gb, 0, 0, tx2 - tx1 + 1, ty2 - ty1 + 1, 0, nRecordCount, false);
                        }
                        bBitmapFill = true;
                    }

                    if (bmTotal != null)
                        g.DrawImageUnscaled(bmTotal, tx1, ty1);

                    DrawGraph(g, gx1, gy1, gx2, gy2, nScrollPosX, nLimitX, true);
                    //DrawTagBar(g);
                    DrawTagBarWithHorizontalScroll(g);
                    DrawSelectBar(g);
                    DrawCursorPos(g);
                }

                // 백버퍼를 화면에 복사
                e.Graphics.DrawImageUnscaled(backBuffer, 0, 0);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"[MilliDataWnd] error: {ex.Message}");
            }
            
        }


        private void DrawModernHeader(Graphics g)
        {
            int yPos = 10;
            
            var headerFont = new Font(_safeFont.FontFamily, _safeFont.Size + 1, FontStyle.Bold);
            var brush = new SolidBrush(nMilliDataBackColor == 1 ? Color.Black : Color.White);

            string timeInfo;
            if (Tools.IsLangKorean())
            {
                timeInfo = $"조회기간: {dateTimeStart.Value:yyyy-MM-dd HH:mm:ss} ~ {dateTimeEnd.Value:yyyy-MM-dd HH:mm:ss}";
            }
            else
            {
                timeInfo = $"Query Period: {dateTimeStart.Value:yyyy-MM-dd HH:mm:ss} ~ {dateTimeEnd.Value:yyyy-MM-dd HH:mm:ss}";
            }

            SafeException.SafeDrawString(g, timeInfo, headerFont, brush, 10 + buttonConfig.Width +10, yPos);

            string dataInfo;
            if (Tools.IsLangKorean())
            {
                dataInfo = $"데이터: {nRecordCount:N0}건 | 간격: {nTimeInterval}ms | 테이블: {currentTableName ?? "선택안됨"}";
            }
            else
            {
                dataInfo = $"Records: {nRecordCount:N0} | Interval: {nTimeInterval}ms | Table: {currentTableName ?? "Not Selected"}";
            }

            SafeException.SafeDrawString(g, dataInfo, _safeFont, brush, 10 + buttonConfig.Width + 10, yPos + cyChar + 5);

            headerFont.Dispose();
            brush.Dispose();
        }

        private void UpdateGraphBounds()
        {
            int cx = this.ClientRectangle.Width;
            int cy = this.ClientRectangle.Height;

            //if (controlPanel == null) return;

            // 위쪽 선택 그래프 영역
            ty1 =  cyChar * 3 + 20;
            tx1 = 10;
            tx2 = cx - 10;
            ty2 = ty1 + (cy - ty1) / 4;

            // 스크롤바는 위쪽 그래프 바로 아래
            scrollHorz.Location = new Point(tx1, ty2 + 5);
            scrollHorz.Width = tx2 - tx1;

            // 메인 그래프 영역은 스크롤바 아래
            gy1 = ty2 + scrollHorz.Height + 25;
            gx1 = cxChar * 20;
            gx2 = cx - 10;
            gy2 = cy - cyChar * 5 - 10;

            // 비트맵 캐시 재생성
            if (tx2 > tx1 && ty2 > ty1)
            {
                bmTotal?.Dispose();
                bmTotal = new Bitmap(Math.Max(1, tx2 - tx1), Math.Max(1, ty2 - ty1));
                bBitmapFill = false;
            }
        }

        // 기존 DrawGraph, DrawBarX, DrawBarY 등의 메소드들은 유지하되 UI 개선 적용
        // ... (기존 그래프 그리기 로직들)
        #endregion

        #region 스크롤 및 이벤트 처리
        private void ScrollUpdate()
        {
            if (nRecordCount > nLimitX)
            {
                scrollHorz.Enabled = true;
                nScrollMaxX = nRecordCount - nLimitX;
                scrollHorz.Minimum = 0;
                scrollHorz.Maximum = nScrollMaxX + 9;
                scrollHorz.Value = nScrollPosX;
            }
            else
            {
                scrollHorz.Enabled = false;
            }
        }

        private void ScrollHorz_Scroll(object sender, ScrollEventArgs e)
        {
            if (nScrollPosX == e.NewValue) return;

            nScrollPosX = e.NewValue;
            if (nScrollPosX < 0) nScrollPosX = 0;
            if (nScrollPosX > nScrollMaxX) nScrollPosX = nScrollMaxX;

            SetSelectZone(nScrollPosX, nScrollPosX + nLimitX - 1);
            Invalidate();
        }

        public void SetFont(Font font)
        {
            //this.Font = font;  //this.Font는 자동 dispose 되어 예외발생 가능성 높음.
            _safeFont = font;
            CalcCharSize();
            this.ScrollUpdate();
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            controlPanel.Visible = !controlPanel.Visible;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateGraphBounds();
            CalcCharSize();
            this.buttonConfig.Size = new Size(cyChar * 2+15, cyChar * 2);
            this.buttonConfig.Font = _safeFont;
            Invalidate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CalcCharSize();
            _ = LoadTableList(); // 비동기 실행
        }

        private void CalcCharSize()
        {
            //DPI 오류, 화면 크기를 변경하면 예외발생하는 경우.
            try
            {
                if(_safeFont ==  null) _safeFont = new Font(this.Font.FontFamily, this.Font.Size, this.Font.Style);

                cxChar = _safeFont.Height / 2;
                cyChar = _safeFont.Height;
            }
            catch { }
        }
        #endregion

        #region 유틸리티 메소드
        private void DataClose()
        {
            nTimeInterval = 0;
            nRecordCount = 0;
            bCursorFlag = false;
            stStart = new SYSTEMTIME();
            stEnd = new SYSTEMTIME();
            scrollHorz.Enabled = false;

            dtData?.Clear();
            dtMember?.Clear();

            // 태그 스크롤 리셋
            tagScrollOffset = 0;
            visibleTagCount = 0;
        }

        private void SetSelectZone(int x1, int x2)
        {
            nSelectX1 = x1;
            nSelectX2 = x2;

            if (nSelectX1 > nSelectX2)
            {
                int temp = nSelectX1;
                nSelectX1 = nSelectX2;
                nSelectX2 = temp;
            }

            if (nSelectX1 < 0) nSelectX1 = 0;
            if (nSelectX2 >= nRecordCount) nSelectX2 = nRecordCount - 1;
        }

        public void SetTimeType(int type)
        {
            m_nTimeType = type;
            Invalidate();
        }

        public int GetTimeType()
        {
            return m_nTimeType;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                bmTotal?.Dispose();
                dtData?.Dispose();
                dtMember?.Dispose();
                backBuffer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        // TODO: 기존 그래프 그리기 메소드들 (DrawGraph, DrawBarX, DrawBarY, DrawTagBar, DrawSelectBar, DrawCursorPos 등)
        // 및 마우스 이벤트 처리 메소드들을 PostgreSQL 데이터 구조에 맞게 수정하여 구현
    }

    /// <summary>
    /// PostgreSQL 기반 밀리데이터 관리 클래스
    /// </summary>
    public class MilliDataPostgreSQLManager
    {
        private const string HISTORY_SCHEMA = "history";
        private readonly string connectionString;

        public MilliDataPostgreSQLManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// 사용 가능한 테이블 목록 조회
        /// </summary>
        public async Task<List<string>> GetTableListAsync()
        {
            var tables = new List<string>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = $@"
                    SELECT table_name 
                    FROM information_schema.tables 
                    WHERE table_schema = '{HISTORY_SCHEMA}' 
                    AND table_name LIKE 'millidata_%'
                    AND table_name != 'millidata_tag_metadata'
                    ORDER BY table_name";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }

            return tables;
        }

        /// <summary>
        /// 데이터 기간 조회
        /// </summary>
        public async Task<(DateTime? start, DateTime? end)> GetDataRangeAsync(string tableName)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = $@"
                    SELECT MIN(data_time) as start_time, MAX(data_time) as end_time
                    FROM {HISTORY_SCHEMA}.{tableName}";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var start = reader.IsDBNull(reader.GetOrdinal("start_time")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("start_time"));
                        var end = reader.IsDBNull(reader.GetOrdinal("end_time")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("end_time"));
                        return (start, end);
                    }
                }
            }

            return (null, null);
        }

        /// <summary>
        /// 자동 삭제 기능 (보존 기간 설정)
        /// </summary>
        public async Task AutoDeleteOldDataAsync(string tableName, TimeSpan retentionPeriod)
        {
            DateTime cutoffDate = DateTime.Now - retentionPeriod;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = $@"
                    DELETE FROM {HISTORY_SCHEMA}.{tableName} 
                    WHERE data_time < @cutoffDate";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("cutoffDate", cutoffDate);
                    int deletedRows = await command.ExecuteNonQueryAsync();

                    Debug.WriteLine($"자동삭제: {tableName}에서 {deletedRows}개 레코드 삭제됨 (기준: {cutoffDate})");
                }
            }
        }

        /// <summary>
        /// 이중화 서버 동기화 (기본 구조)
        /// </summary>
        public async Task<bool> SyncToBackupServerAsync(string backupConnectionString, string tableName, DateTime fromTime)
        {
            try
            {
                using (var sourceConnection = new NpgsqlConnection(connectionString))
                using (var backupConnection = new NpgsqlConnection(backupConnectionString))
                {
                    await sourceConnection.OpenAsync();
                    await backupConnection.OpenAsync();

                    // 소스에서 데이터 조회
                    string selectQuery = $@"
                        SELECT * FROM {HISTORY_SCHEMA}.{tableName} 
                        WHERE data_time >= @fromTime 
                        ORDER BY data_time";

                    using (var selectCommand = new NpgsqlCommand(selectQuery, sourceConnection))
                    {
                        selectCommand.Parameters.AddWithValue("fromTime", fromTime);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);

                            // 백업 서버로 데이터 복사
                            using (var writer = backupConnection.BeginBinaryImport($"COPY {HISTORY_SCHEMA}.{tableName} FROM STDIN (FORMAT BINARY)"))
                            {
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    writer.StartRow();
                                    for (int i = 0; i < dataTable.Columns.Count; i++)
                                    {
                                        writer.Write(row[i]);
                                    }
                                }
                                await writer.CompleteAsync();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"이중화 동기화 오류: {ex.Message}");
                return false;
            }
        }
    }
}

// 기존 메소드들의 구현 부분
namespace GraphicModule
{
    public partial class MilliDataWnd
    {
        #region 그래프 그리기 메소드들 (PostgreSQL 데이터 구조에 맞게 수정)

        private void DrawGraph(Graphics g, int x1, int y1, int x2, int y2, int scroll_posx, int limitx, bool point_flag)
        {
            if (nRecordCount <= 1) return;

            bool move_flag = false;
            int x, y;
            int screen_width = x2 - x1;
            int screen_height = y2 - y1;
            int nGab = limitx > screen_width ? limitx / screen_width : 1;

            for (int f = 0; f < dtMember.Rows.Count; f++)
            {
                DataRow memberRow = dtMember.Rows[f];
                string tagName = memberRow["Tag"].ToString();

                if (!dtData.Columns.Contains(tagName)) continue;

                using (var pen = new Pen(GetDefinedColor(f), 2)) // 선 두께 증가
                {
                    double full = Convert.ToDouble(memberRow["Full"]);
                    double fBase = Convert.ToDouble(memberRow["Base"]);
                    int varTagType = Convert.ToInt32(memberRow["TagType"]);

                    if (f >= MAX_VIEW_ACTIVE) continue;
                    if (bActive[f] == 0) continue;

                    move_flag = false;
                    int di_height = dtMember.Rows.Count == 0 ? screen_height : screen_height / dtMember.Rows.Count;
                    int di_pos = y2 - di_height * f;
                    int movex = 0, movey = 0;

                    for (int i = 0, pos = scroll_posx; i < limitx && pos < nRecordCount; i += nGab, pos += nGab)
                    {
                        if (pos >= dtData.Rows.Count) break;

                        DataRow datarow = dtData.Rows[pos];

                        x = limitx <= 1 ? x1 : x1 + screen_width * i / (limitx - 1);

                        double var = 0;
                        if (datarow[tagName] != DBNull.Value)
                        {
                            if (varTagType == 0) // AI 태그
                                var = Convert.ToDouble(datarow[tagName].ToString());
                            else if (varTagType == 9) // ST 태그
                                return;
                            else // DI 태그
                                var = Convert.ToDouble(datarow[tagName].ToString());
                        }

                        if (varTagType == 0) // AI 태그
                        {
                            if (Math.Abs(full - fBase) < double.Epsilon)
                                y = y2;
                            else
                                y = (int)(y2 - ((var - fBase) * screen_height / (full - fBase)));

                            y = Math.Max(y1, Math.Min(y2, y));
                        }
                        else // DI 태그
                        {
                            y = var == 1 ? (int)(di_pos - di_height * 0.9) : (int)(di_pos - di_height * 0.1);
                        }

                        if (!move_flag)
                        {
                            move_flag = true;
                            movex = x;
                            movey = y;
                        }
                        else
                        {
                            g.DrawLine(pen, movex, movey, x, y);
                            movex = x;
                            movey = y;
                        }

                        if (point_flag)
                        {
                            using (var brush = new SolidBrush(GetDefinedColor(f)))
                            {
                                g.FillEllipse(brush, x - 2, y - 2, 4, 4); // 원형 포인트
                            }
                        }
                    }
                }
            }
        }

        private void DrawBarX(Graphics g)
        {
            var pen = new Pen(Color.Gray, 1);
            int screen_width = gx2 - gx1;
            int old_x = -100;
            int old_textx = 0;
            int old_date_textx = 0;
            int gab = 1;
            var brush = new SolidBrush(nMilliDataBackColor == 1 ? Color.Black : Color.White);

            for (int i = 0, pos = nScrollPosX; i < nLimitX && pos < nRecordCount; i += gab, pos += gab)
            {
                if (pos >= dtData.Rows.Count) break;

                int x = nLimitX <= 1 ? gx1 : gx1 + screen_width * i / (nLimitX - 1);

                // 적어도 안내라인은 15칸 정도로 한다 (MilliDataWnd_mdb와 동일)
                if (x < old_x + 15) continue;
                old_x = x;

                g.DrawLine(pen, x, gy1, x, gy2);

                // ChangePosToString 메서드 사용하여 시간 문자열 생성
                string timeStr, dateStr;
                ChangePosToString(out timeStr, out dateStr, pos);

                var timeSize = g.MeasureString(timeStr, _safeFont);

                // 시간 텍스트 영역 계산
                var timeRect = new RectangleF(x - timeSize.Width / 2, gy2 + cyChar + 1, timeSize.Width, cyChar);

                // 이전 텍스트와 겹치지 않도록 체크 (cxChar만큼 여유 공간)
                if (timeRect.Left > old_textx + cxChar)
                {
                    SafeException.SafeDrawString(g, timeStr, _safeFont, brush, timeRect);
                    old_textx = (int)timeRect.Right;

                    // 날짜 표시 (절대시간 모드일 때만)
                    if (!string.IsNullOrEmpty(dateStr))
                    {
                        var dateSize = g.MeasureString(dateStr, _safeFont);
                        var dateRect = new RectangleF(x - dateSize.Width / 2, gy2 + cyChar * 2 + 1, dateSize.Width, cyChar);

                        // 이전 날짜 텍스트와 겹치지 않도록 체크
                        if (dateRect.Left > old_date_textx + cxChar)
                        {
                            SafeException.SafeDrawString(g, dateStr, _safeFont, brush, dateRect);
                            old_date_textx = (int)dateRect.Right;
                        }
                    }
                }
            }

            pen.Dispose();
            brush.Dispose();
        }

        #region 시간 문자열 변환 메서드 
        /// <summary>
        /// 위치(인덱스)를 시간 문자열로 변환
        /// m_nTimeType에 따라 상대시간(0) 또는 절대시간(1) 표시
        /// </summary>
        /// <param name="timeString">시간 문자열 (HH:mm:ss 또는 HH:mm:ss.fff)</param>
        /// <param name="dateString">날짜 문자열 (yyyy-MM-dd 또는 빈 문자열)</param>
        /// <param name="pos">데이터 위치 인덱스</param>
        private void ChangePosToString(out string timeString, out string dateString, int pos)
        {
            if (m_nTimeType == 0) // 상대시간 (시작시간 기준)
            {
                int msec = pos * nTimeInterval;
                int hour = msec / (1000 * 3600);
                msec %= (1000 * 3600);
                int minute = msec / (1000 * 60);
                msec %= (1000 * 60);
                int sec = msec / 1000;
                msec %= 1000;

                if (nTimeInterval < 1000)
                {
                    timeString = $"{hour:00}:{minute:00}:{sec:00}.{msec:000}";
                }
                else
                {
                    timeString = $"{hour:00}:{minute:00}:{sec:00}";
                }
                dateString = "";
            }
            else // 절대시간 (실제 시간)
            {
                DateTime tm = stStart.ToDateTime();
                int msec = pos * nTimeInterval;
                tm = tm.AddMilliseconds(msec);

                if (nTimeInterval < 1000)
                {
                    timeString = $"{tm.Hour:00}:{tm.Minute:00}:{tm.Second:00}.{tm.Millisecond:000}";
                }
                else
                {
                    timeString = $"{tm.Hour:00}:{tm.Minute:00}:{tm.Second:00}";
                }

                dateString = $"{tm.Year:0000}-{tm.Month:00}-{tm.Day:00}";
            }
        }
        #endregion

        private void DrawBarY(Graphics g)
        {
            var bgBrush = new SolidBrush(GetMilliDataBackColor());
            g.FillRectangle(bgBrush, 10, gy1, gx1 - 20, gy2 - gy1);
            bgBrush.Dispose();

            if (dtMember.Rows.Count == 0) return;

            var pen = new Pen(Color.Gray, 1);
            int screen_height = gy2 - gy1;
            int devide = 11;

            if (nDesTag >= dtMember.Rows.Count) nDesTag = 0;
            DataRow row = dtMember.Rows[nDesTag];

            double full = Convert.ToDouble(row["Full"]);
            double fBase = Convert.ToDouble(row["Base"]);
            int tag_type = Convert.ToInt32(row["TagType"]);

            var brush = new SolidBrush(GetDefinedColor(nDesTag));

            if (tag_type == 0) // AI 태그
            {
                for (int i = 0; i < devide; i++)
                {
                    int y = gy2 - (i * screen_height / (devide - 1));
                    g.DrawLine(pen, gx1, y, gx2, y);

                    string valueStr = (fBase + i * (full - fBase) / (devide - 1)).ToString("F2");
                    var size = g.MeasureString(valueStr, _safeFont);
                    var rect = new RectangleF(gx1 - size.Width - 5, y - size.Height / 2, size.Width, size.Height);
                    SafeException.SafeDrawString(g, valueStr, _safeFont, brush, rect);
                }
            }
            else if(tag_type == 9) //ST 태그
            {

            }
            else // DI 태그
            {
                int di_height = screen_height / dtMember.Rows.Count;
                int di_pos = gy2 - di_height * nDesTag;

                SafeException.SafeDrawString(g, "OFF", _safeFont, brush, new PointF(10, di_pos - cyChar));
                SafeException.SafeDrawString(g, "ON", _safeFont, brush, new PointF(10, di_pos - di_height));
            }

            pen.Dispose();
            brush.Dispose();
        }

        private void DrawTagBar(Graphics g)
        {
            int y = gy2 + cyChar * 2 + 2;
            var darkBrush = new SolidBrush(nMilliDataBackColor == 1 ? Color.Black : Color.White);
            var grayBrush = new SolidBrush(Color.Gray);
            var whiteBrush = new SolidBrush(Color.White);

            for (int f = 0, x = 0; f < dtMember.Rows.Count && x < this.ClientRectangle.Width; f++, x += cxChar * 15)
            {
                DataRow row = dtMember.Rows[f];
                string tagName = row["Tag"].ToString();
                int tag_type = Convert.ToInt32(row["TagType"]);

                // 태그 박스 배경
                var boxRect = new Rectangle(x, y + cyChar, cxChar * 15 - 1, cyChar);
                g.FillRectangle(new SolidBrush(Color.LightGray), boxRect);
                g.DrawRectangle(Pens.DarkGray, boxRect);

                // 색상 표시기
                var colorRect = new Rectangle(x + 2, y + 2 + cyChar, 4, cyChar - 4);
                g.FillRectangle(new SolidBrush(GetDefinedColor(f)), colorRect);
                g.DrawRectangle(Pens.Black, colorRect);

                // 태그명 표시
                var textRect = new RectangleF(x + 9, y + 1 + cyChar, cxChar * 15 - 10, cyChar);
                var textBrush = f < MAX_VIEW_ACTIVE && bActive[f] == 0 ? grayBrush :
                               nDesTag == f ? whiteBrush : darkBrush;

                string displayText = tagName;
                if (bMilliDataTagDescription)
                {
                    // 태그 설명으로 표시하는 로직 (필요시 구현)
                }

                SafeException.SafeDrawString(g, displayText, _safeFont, textBrush, textRect);

                // 현재 값 표시
                var valueRect = new Rectangle(x, y + cyChar * 2 + 1, cxChar * 15 - 1, cyChar);
                g.FillRectangle(new SolidBrush(Color.LightGray), valueRect);
                g.DrawRectangle(Pens.DarkGray, valueRect);

                if (bCursorFlag && nCursorPos < dtData.Rows.Count)
                {
                    DataRow dataRow = dtData.Rows[nCursorPos];
                    string valueStr = "";

                    if (dataRow[tagName] != DBNull.Value)
                    {
                        if (tag_type == 0) // AI
                        {
                            double val = Convert.ToDouble(dataRow[tagName]);
                            valueStr = val.ToString("F2");
                        }
                        else if(tag_type == 9) //ST
                        {
                            valueStr = "";
                        }
                        else // DI
                        {
                            int val = Convert.ToInt32(dataRow[tagName]);
                            valueStr = val == 1 ? "ON" : "OFF";
                        }
                    }

                    var valueTextRect = new RectangleF(x + 9, y + cyChar * 2 + 2, cxChar * 15 - 10, cyChar);
                    SafeException.SafeDrawString(g, valueStr, _safeFont, darkBrush, valueTextRect);
                }
            }

            darkBrush.Dispose();
            grayBrush.Dispose();
            whiteBrush.Dispose();
        }

        private void DrawCursorPos(Graphics g)
        {
            if (!bCursorFlag) return;
            if (nCursorPos < nScrollPosX || nCursorPos >= nScrollPosX + nLimitX) return;

            int screen_width = gx2 - gx1;
            int x = nLimitX <= 1 ? gx1 : gx1 + screen_width * (nCursorPos - nScrollPosX) / (nLimitX - 1);

            // 커서 라인 그리기
            using (var pen = new Pen(Color.Red, 2))
            {
                g.DrawLine(pen, x, gy1, x, gy2);
            }

            // 시간 정보 표시
            if (nCursorPos < dtData.Rows.Count)
            {
                DataRow dataRow = dtData.Rows[nCursorPos];
                DateTime recordTime = Convert.ToDateTime(dataRow["data_time"]);

                string timeInfo = m_nTimeType == 1 ?
                    recordTime.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                    recordTime.ToString("HH:mm:ss.fff");

                var size = g.MeasureString(timeInfo, _safeFont);
                var rect = new RectangleF(
                    Math.Max(0, Math.Min(this.ClientRectangle.Width - size.Width, x - size.Width / 2)),
                    gy1 - cyChar - 2,
                    size.Width,
                    size.Height);
                var rect2 = new RectangleF(
                  Math.Max(0, Math.Min(this.ClientRectangle.Width - size.Width, x - size.Width / 2)),
                  gy1 - cyChar,
                  size.Width,
                  size.Height);

                using (var brush = new SolidBrush(Color.Yellow))
                {
                    g.FillRectangle(brush, rect);
                }
                g.DrawRectangle(Pens.Black, Rectangle.Round(rect));
                SafeException.SafeDrawString(g, timeInfo, _safeFont, Brushes.Black, rect2);
            }
        }

        private void DrawSelectBar(Graphics g)
        {
            if (bmTotal != null)
                g.DrawImageUnscaled(bmTotal, tx1, ty1);

            int dx1 = bMouseCapture ? nStartX : nSelectX1;
            int dx2 = bMouseCapture ? nEndX : nSelectX2;

            if (dx1 > dx2)
            {
                int temp = dx1;
                dx1 = dx2;
                dx2 = temp;
            }

            int screen_width = tx2 - tx1;
            int x1, x2;

            if (nRecordCount <= 1)
            {
                x1 = x2 = tx1;
            }
            else
            {
                x1 = tx1 + screen_width * dx1 / (nRecordCount - 1);
                x2 = tx1 + screen_width * dx2 / (nRecordCount - 1);
            }

            var selectRect = new Rectangle(x1, ty1, x2 - x1 + 1, ty2 - ty1);
            using (var brush = new SolidBrush(Color.FromArgb(100, Color.LightGray)))
            {
                g.FillRectangle(brush, selectRect);
            }
            g.DrawRectangle(Pens.Blue, selectRect);
        }
        #endregion

        #region 마우스 이벤트 처리
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnLButtonDown(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                OnRButtonDown(e);
            }
        }

        private void OnLButtonDown(MouseEventArgs e)
        {
            // 태그 스크롤 화살표 클릭 체크 (먼저 처리)
            if (HandleTagScrollClick(e))
                return;

            int tag_pos = 0;
            if (CheckTagSelect(ref tag_pos, e))
            {
                nDesTag = tag_pos;
                Invalidate();
                return;
            }

            // 선택 영역에서 클릭한 경우
            if (e.X >= tx1 && e.X <= tx2 && e.Y >= ty1 && e.Y <= ty2)
            {
                this.Capture = true;
                bMouseCapture = true;
                int screen_width = tx2 - tx1;
                nStartX = (int)((double)(e.X - tx1) * (nRecordCount - 1) / screen_width + 0.5);
                nEndX = nStartX;
                Invalidate();
                return;
            }

            // 그래프 영역에서 클릭한 경우 (커서 설정)
            if (e.X >= gx1 && e.X <= gx2 && e.Y >= gy1 && e.Y <= gy2)
            {
                int screen_width = gx2 - gx1;
                int pos = (int)(nScrollPosX + (double)(e.X - gx1) * (nLimitX - 1) / screen_width + 0.5);

                if (pos >= nScrollPosX && pos < nScrollPosX + nLimitX && pos < nRecordCount)
                {
                    nCursorPos = pos;
                    bCursorFlag = true;
                    Invalidate();
                }
            }
        }

        private void OnRButtonDown(MouseEventArgs e)
        {
            int tag_pos = 0;
            if (!CheckTagSelect(ref tag_pos, e)) return;
            if (tag_pos >= MAX_VIEW_ACTIVE) return;

            bActive[tag_pos] = bActive[tag_pos] == 1 ? (sbyte)0 : (sbyte)1;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!bMouseCapture) return;

            int screen_width = tx2 - tx1;
            int pos = (int)((double)(e.X - tx1) * (nRecordCount - 1) / screen_width + 0.5);

            pos = Math.Max(0, Math.Min(nRecordCount - 1, pos));
            if (pos == nEndX) return;

            nEndX = pos;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!bMouseCapture) return;

            bMouseCapture = false;
            this.Capture = false;

            SetSelectZone(nStartX, nEndX);
            nScrollPosX = nSelectX1;
            nLimitX = nSelectX2 - nSelectX1 + 1;

            ScrollUpdate();
            Invalidate();
        }

        /// <summary>
        /// 마우스 클릭으로 태그 스크롤 처리 (가로 스크롤용)
        /// 기존 OnMouseDown을 확장하여 태그 스크롤 기능 추가
        /// </summary>
        private bool HandleTagScrollClick(MouseEventArgs e)
        {
            int y = gy2 + cyChar * 2 + 2;
            int arrowWidth = 30;

            // 왼쪽 화살표 클릭
            if (e.X >= 0 && e.X <= arrowWidth && e.Y >= y + cyChar && e.Y <= y + cyChar + TAG_HEIGHT)
            {
                if (tagScrollOffset > 0)
                {
                    tagScrollOffset--;
                    Invalidate();
                }
                return true;
            }

            // 오른쪽 화살표 클릭
            int rightArrowX = this.ClientRectangle.Width - arrowWidth;
            if (e.X >= rightArrowX && e.X <= this.ClientRectangle.Width &&
                e.Y >= y + cyChar && e.Y <= y + cyChar + TAG_HEIGHT)
            {
                int totalTags = dtMember.Rows.Count;
                if (tagScrollOffset + visibleTagCount < totalTags)
                {
                    tagScrollOffset++;
                    Invalidate();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 가로 스크롤을 위한 태그 선택 체크 메서드 (CheckTagSelect 대체)
        /// </summary>
        private bool CheckTagSelect(ref int pos, MouseEventArgs e)
        {
            int y = gy2 + cyChar * 2 + 2;
            int totalTags = dtMember.Rows.Count;
            int clientWidth = this.ClientRectangle.Width;

            // 화살표 영역 체크
            visibleTagCount = Math.Max(1, clientWidth / TAG_WIDTH);
            bool showLeftArrow = tagScrollOffset > 0;
            bool showRightArrow = tagScrollOffset + visibleTagCount < totalTags;

            int arrowWidth = 30;
            int tagAreaWidth = clientWidth - (showLeftArrow ? arrowWidth : 0) - (showRightArrow ? arrowWidth : 0);
            int actualVisibleCount = Math.Min(visibleTagCount, tagAreaWidth / TAG_WIDTH);
            int startX = showLeftArrow ? arrowWidth : 0;

            // 태그 영역에서 클릭 체크
            for (int i = 0; i < actualVisibleCount && (tagScrollOffset + i) < totalTags; i++)
            {
                int tagIndex = tagScrollOffset + i;
                int x = startX + i * TAG_WIDTH;

                if (e.X >= x && e.X <= x + TAG_WIDTH && e.Y > y)
                {
                    pos = tagIndex;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 방법 1: 가로 스크롤 가능한 태그바
        /// </summary>
        private void DrawTagBarWithHorizontalScroll(Graphics g)
        {
            int y = gy2 + cyChar * 2 + 2;
            int clientWidth = this.ClientRectangle.Width;

            // 한 줄에 표시 가능한 태그 개수 계산
            visibleTagCount = Math.Max(1, clientWidth / TAG_WIDTH);
            int totalTags = dtMember.Rows.Count;

            var darkBrush = new SolidBrush(nMilliDataBackColor == 1 ? Color.Black : Color.White);
            var grayBrush = new SolidBrush(Color.Gray);
            var whiteBrush = new SolidBrush(Color.White);

            // 스크롤 화살표 버튼 영역
            int arrowWidth = 25;
            bool showLeftArrow = tagScrollOffset > 0;
            bool showRightArrow = tagScrollOffset + visibleTagCount < totalTags;

            int tagAreaWidth = clientWidth - (showLeftArrow ? arrowWidth : 0) - (showRightArrow ? arrowWidth : 0);
            int actualVisibleCount = Math.Min(visibleTagCount, tagAreaWidth / TAG_WIDTH);

            // 왼쪽 스크롤 화살표
            if (showLeftArrow)
            {
                var leftArrowRect = new Rectangle(0, y + cyChar, arrowWidth-2, TAG_HEIGHT);
                g.FillRectangle(Brushes.LightGray, leftArrowRect);
                g.DrawRectangle(Pens.Gray, leftArrowRect);
                SafeException.SafeDrawString(g, "◀", _safeFont, Brushes.Black,
                    leftArrowRect.X + 4, leftArrowRect.Y );
            }

            // 오른쪽 스크롤 화살표
            if (showRightArrow)
            {
                var rightArrowRect = new Rectangle(clientWidth - arrowWidth, y + cyChar, arrowWidth-2, TAG_HEIGHT);
                g.FillRectangle(Brushes.LightGray, rightArrowRect);
                g.DrawRectangle(Pens.Gray, rightArrowRect);
                SafeException.SafeDrawString(g, "▶", _safeFont, Brushes.Black,
                    rightArrowRect.X + 4, rightArrowRect.Y );
            }

            // 태그 표시
            int startX = showLeftArrow ? arrowWidth : 0;
            for (int i = 0; i < actualVisibleCount && (tagScrollOffset + i) < totalTags; i++)
            {
                int tagIndex = tagScrollOffset + i;
                int x = startX + i * TAG_WIDTH;

                DrawSingleTag(g, tagIndex, x, y, darkBrush, grayBrush, whiteBrush);
            }

            // 스크롤 정보 표시
            if (totalTags > actualVisibleCount)
            {
                string scrollInfo = $"{tagScrollOffset + 1}-{Math.Min(tagScrollOffset + actualVisibleCount, totalTags)} / {totalTags}";
                SafeException.SafeDrawString(g, scrollInfo, _safeFont, darkBrush, 10, y - cyChar);
            }

            darkBrush.Dispose();
            grayBrush.Dispose();
            whiteBrush.Dispose();
        }

        /// <summary>
        /// 개별 태그를 그리는 공통 메서드
        /// </summary>
        private void DrawSingleTag(Graphics g, int tagIndex, int x, int y,
            SolidBrush darkBrush, SolidBrush grayBrush, SolidBrush whiteBrush)
        {
            DataRow memberRow = dtMember.Rows[tagIndex];
            string tagName = memberRow["Tag"].ToString();
            int tag_type = Convert.ToInt32(memberRow["TagType"]);

            // 태그 박스 배경
            var boxRect = new Rectangle(x, y + cyChar, TAG_WIDTH - 1, TAG_HEIGHT);
            g.FillRectangle(new SolidBrush(Color.LightGray), boxRect);
            g.DrawRectangle(Pens.DarkGray, boxRect);

            // 색상 표시기
            var colorRect = new Rectangle(x + 2, y + 2 + cyChar, 4, TAG_HEIGHT - 4);
            g.FillRectangle(new SolidBrush(GetDefinedColor(tagIndex)), colorRect);
            g.DrawRectangle(Pens.Black, colorRect);

            // 태그명 표시
            var textRect = new RectangleF(x + 9, y + 1 + cyChar, TAG_WIDTH - 10, cyChar);
            var textBrush = tagIndex < MAX_VIEW_ACTIVE && bActive[tagIndex] == 0 ? grayBrush :
                           nDesTag == tagIndex ? whiteBrush : darkBrush;

            string displayText = bMilliDataTagDescription ? GetTagDescription(tagName) : tagName;
            if (displayText.Length > 12)
                displayText = displayText.Substring(0, 12) + "...";

            SafeException.SafeDrawString(g, displayText, _safeFont, textBrush, textRect);

            // 현재 값 표시
            var valueRect = new Rectangle(x, y + TAG_HEIGHT + cyChar + 1, TAG_WIDTH - 1, TAG_HEIGHT);
            g.FillRectangle(new SolidBrush(Color.LightGray), valueRect);
            g.DrawRectangle(Pens.DarkGray, valueRect);

            if (bCursorFlag && nCursorPos < dtData.Rows.Count)
            {
                DataRow dataRow = dtData.Rows[nCursorPos];
                string valueStr = GetTagValueString(dataRow, tagName, tag_type);

                var valueTextRect = new RectangleF(x + 9, y + TAG_HEIGHT + cyChar + 2, TAG_WIDTH - 10, cyChar);
                SafeException.SafeDrawString(g, valueStr, _safeFont, darkBrush, valueTextRect);
            }
        }

        /// <summary>
        /// 태그 값을 문자열로 변환
        /// </summary>
        private string GetTagValueString(DataRow dataRow, string tagName, int tagType)
        {
            if (dataRow[tagName] == DBNull.Value) return "N/A";

            if (tagType == 0) // AI
            {
                double val = Convert.ToDouble(dataRow[tagName]);
                return val.ToString("F2");
            }
            else if(tagType == 9 ) //ST
            {
                return Convert.ToString(dataRow[tagName]);
            }
            else // DI
            {
                int val = Convert.ToInt32(dataRow[tagName]);
                return val == 1 ? "ON" : "OFF";
            }
        }

        /// <summary>
        /// 태그 설명 반환 
        /// </summary>
        private string GetTagDescription(string tagName)
        {
            TagPublicClass tp;
            int[] tag_pos = new int[1];
            tp = TagLib.GetStructPublic(tagName, ref tag_pos);
            return tp.description; 
        }
        #endregion


        /// <summary>
        /// 시작 시간 설정 (개별 파라미터)
        /// </summary>
        public void SetStartTime(int year, int mon, int day, int hour, int min, int sec)
        {
            dateTimeStart.Value = new DateTime(year, mon, day, hour, min, sec);
        }

         /// <summary>
        /// 종료 시간 설정 (개별 파라미터)
        /// </summary>
        public void SetEndTime(int year, int mon, int day, int hour, int min, int sec)
        {
            dateTimeEnd.Value = new DateTime(year, mon, day, hour, min, sec);
        }

        /// <summary>
        /// 시작/종료 시간 동시 설정 (개별 파라미터)
        /// </summary>
        public void SetTimeRange(
            int startYear, int startMon, int startDay, int startHour, int startMin, int startSec,
            int endYear, int endMon, int endDay, int endHour, int endMin, int endSec)
        {
            DateTime startTime = new DateTime(startYear, startMon, startDay, startHour, startMin, startSec);
            DateTime endTime = new DateTime(endYear, endMon, endDay, endHour, endMin, endSec);

            if (startTime >= endTime)
            {
                throw new ArgumentException("시작 시간은 종료 시간보다 이전이어야 합니다.");
            }

            dateTimeStart.Value = startTime;
            dateTimeEnd.Value = endTime;
        }


        /// <summary>
        /// 테이블명 설정
        /// </summary>
        /// <param name="tableName">조회할 테이블명 (millidata_xxx)</param>
        public void SetTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("The TableName is empty.");
            }

            if (!IsValidTableName(tableName))
            {
                throw new ArgumentException("Invalid TableName");
            }

            currentTableName = tableName;

            // 콤보박스에 해당 테이블이 있으면 선택
            if (comboTableSelect.Items.Contains(tableName))
            {
                comboTableSelect.SelectedItem = tableName;
            }
        }

        /// <summary>
        /// 데이터 로드 (외부 호출용 비동기 메서드)
        /// </summary>
        public async Task LoadDataAsync()
        {
            if (string.IsNullOrEmpty(currentTableName))
            {
                throw new InvalidOperationException("테이블이 설정되지 않았습니다. SetTableName()을 먼저 호출하세요.");
            }

            await LoadDataFromPostgreSQL();
        }

    }
}
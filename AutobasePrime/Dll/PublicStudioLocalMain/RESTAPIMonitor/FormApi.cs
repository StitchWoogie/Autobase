using AutoLib;
using AutoLibLocal;
using DialogTag;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace AutobaseRESTAPIMonitor
{
       public partial class FormApi : Form
    {
        public string CurrentValue { get; set; }
        public string ModifiedTitle { get; private set; }

        private bool isRunning;
        private BindingSource bindingSource;
        private BindingList<ParseRule> parseRules = new BindingList<ParseRule>();
        private HttpClient client = new HttpClient();
        private int selectedRowIndex = -1;  // 선택된 행 인덱스 저장
        private Dictionary<string, string> previousValues = new Dictionary<string, string>();
        private Dictionary<string, string> currentValues = new Dictionary<string, string>();
        private string originalTitle;
        private readonly string apiUrl;
        private int dragIndex = -1;  // 드래그 시작 인덱스
        private string notFoundText = "Not Found"; // 기본값 설정
        private bool useNotFoundText = false;
        private List<(string Name, string Value, bool Encrypt)> headers = new List<(string Name, string Value, bool Encrypt)>();


        public FormApi(string title, string url, int IntervalSeconds)
        {
            InitializeComponent();

            originalTitle = title;
            apiUrl = url;
            txtUrl.Text = apiUrl;
            txtTitle.Text = title;
            if (IntervalSeconds <= 0) IntervalSeconds = 1;
                numericInterval.Value = IntervalSeconds;

            // 멤버 변수 초기화
            bindingSource = new BindingSource();
            parseRules = new BindingList<ParseRule>();
            client = new HttpClient();

            // 기존 설정 로드시 체크박스 상태 설정
            if (!string.IsNullOrEmpty(title))
            {
                var configs = RestApiMonitorService.LoadAllConfigs();
                var clientInfo = configs?.FirstOrDefault(c => c.Title == title);
                if (clientInfo != null)
                {
                    checkBoxUse.Checked = clientInfo.IsRunning;
                    isRunning = clientInfo.IsRunning;
                    notFoundText = clientInfo.NotFoundText ?? "Not Found";
                    txtNotFoundText.Text = notFoundText;
                    chkUseNotFound.Checked = clientInfo.UseNotFoundText;
                    txtNotFoundText.Enabled = chkUseNotFound.Checked;
                    headers = clientInfo.Headers ?? new List<(string Name, string Value, bool Encrypt)>();
                }
            }
            else
            {
                checkBoxUse.Checked = true; // 새로운 클라이언트의 경우 기본값
                isRunning = true;
            }

            InitializeDataGridView();

            InitializeContextMenu();

            // 기존 ParseRule 데이터 로드
            if (!string.IsNullOrEmpty(title))
            {
                LoadParseRules(title);
            }
        }

        private void LoadParseRules(string title)
        {
            try
            {
                string configPath = RestApiMonitorService.ConfigFile;
                if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
                {
                    // 전체 설정 로드
                    var configs = RestApiMonitorService.LoadAllConfigs();
                    var clientInfo = configs?.FirstOrDefault(c => c.Title == title);

                    if (clientInfo != null)
                    {
                        // 기존 ParseRules를 데이터그리드뷰에 로드
                        parseRules.Clear();
                        foreach (var rule in clientInfo.ParseRules)
                        {
                            parseRules.Add(new ParseRule
                            {
                                JsonPath = rule.JsonPath,
                                Tag = rule.Tag
                            });
                        }

                        // 주기값 설정
                        numericInterval.Value = clientInfo.IntervalSeconds;
                        UpdateGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                if (GlobalSettings.IsKorean)
                    MessageBox.Show($"규칙 로드 중 오류 발생: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    MessageBox.Show($"Error loading rules: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async void btnTestParse_Click(object sender, EventArgs e)
        {
            
            btnTestParse.Enabled = false;
            btnTestParse.Text = GlobalSettings.IsKorean ? "파싱 중..." : "Parsing...";
            try
            {
                // URL 포맷 유효성 검사
                string invalidFormat;
                if (!UrlFormatter.ValidateUrlFormat(txtUrl.Text, out invalidFormat))
                {
                    MessageBox.Show(GlobalSettings.IsKorean ? $"잘못된 URL 포맷이 사용되었습니다: {invalidFormat}": $"Invalid URL format used: {invalidFormat}",
                        GlobalSettings.IsKorean ? "오류":"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // URL에 포맷 적용
                string formattedUrl = UrlFormatter.FormatUrl(txtUrl.Text);

                client.DefaultRequestHeaders.Clear();
                foreach (var header in headers)
                {
                    string headerValue = header.Encrypt ? EncryptionHelper.Decrypt(header.Value) : header.Value;
                    client.DefaultRequestHeaders.Add(header.Name, headerValue);
                }

                string response = await client.GetStringAsync(formattedUrl);

               // string response = await client.GetStringAsync(txtUrl.Text);


                //JToken jsonToken = JToken.Parse(response);
                //currentValues.Clear();

                // JSON 시작 위치 찾기 (배열 또는 객체)
                int jsonStartIndex = response.IndexOfAny(new char[] { '[', '{' });
                if (jsonStartIndex == -1)
                {
                    MessageBox.Show( GlobalSettings.IsKorean? "유효한 JSON 데이터를 찾을 수 없습니다.": "No valid JSON data found", GlobalSettings.IsKorean ? "오류":"Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 시작 문자에 따른 종료 문자 결정
                char endChar = response[jsonStartIndex] == '[' ? ']' : '}';

                // JSON 끝 위치 찾기
                int jsonEndIndex = response.LastIndexOf(endChar);
                if (jsonEndIndex == -1)
                {
                    MessageBox.Show(GlobalSettings.IsKorean ? "유효한 JSON 데이터를 찾을 수 없습니다." : "No valid JSON data found", GlobalSettings.IsKorean ? "오류" : "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 주석을 제외한 순수 JSON 문자열 추출
                string jsonStr = response.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1);

                /// JSON 파싱
                JToken jsonToken = JToken.Parse(jsonStr);

                foreach (var rule in parseRules)
                {
                    try
                    {
                        var tokens = jsonToken.SelectTokens(rule.JsonPath);
                        if (tokens.Any())
                        {
                            if (tokens.Count() == 1)
                            {
                                var token = tokens.First();
                                switch (token.Type)
                                {
                                    case JTokenType.Integer:
                                    case JTokenType.Float:
                                    case JTokenType.Boolean:
                                        currentValues[rule.Tag] = token.ToString();
                                        break;
                                    case JTokenType.String:
                                        string processedString = ProcessStringValue(token.Value<string>());
                                        currentValues[rule.Tag] = processedString;
                                        break;
                                    case JTokenType.Array:
                                    case JTokenType.Object:
                                        currentValues[rule.Tag] = ProcessTokenValue(token).ToString(Formatting.None);
                                        break;
                                    default:
                                        currentValues[rule.Tag] = ProcessStringValue(token.ToString());
                                        break;
                                }
                            }
                            else
                            {
                                var tokenArray = new JArray();
                                foreach (var token in tokens)
                                {
                                    switch (token.Type)
                                    {
                                        case JTokenType.Integer:
                                        case JTokenType.Float:
                                        case JTokenType.Boolean:
                                            tokenArray.Add(token);
                                            break;
                                        case JTokenType.String:
                                            tokenArray.Add(ProcessStringValue(token.Value<string>()));
                                            break;
                                        case JTokenType.Array:
                                        case JTokenType.Object:
                                            tokenArray.Add(ProcessTokenValue(token));
                                            break;
                                        default:
                                            tokenArray.Add(ProcessStringValue(token.ToString()));
                                            break;
                                    }
                                }
                                currentValues[rule.Tag] = tokenArray.ToString(Formatting.None);
                            }
                        }
                        else
                        {
                            currentValues[rule.Tag] = "Not Found";
                        }
                    }
                    catch (JsonException pathEx)
                    {
                        currentValues[rule.Tag] = $"Invalid Path: {pathEx.Message}";
                    }
                }
                UpdateGrid();
            }
            catch (Exception ex)
            {
                if (GlobalSettings.IsKorean)
                {
                    MessageBox.Show($"파싱 중 오류 발생: {ex.Message}", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Error parsing: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                // 작업 완료 후 버튼 다시 활성화
                btnTestParse.Enabled = true;
                btnTestParse.Text = GlobalSettings.IsKorean ? "테스트 파싱" : "Test Parse";
            }
        }

        // JToken 처리를 위한 헬퍼 메서드 추가
        private static JToken ProcessTokenValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var newObj = new JObject();
                    foreach (var prop in token.Children<JProperty>())
                    {
                        newObj[prop.Name] = ProcessTokenValue(prop.Value);
                    }
                    return newObj;

                case JTokenType.Array:
                    var newArr = new JArray();
                    foreach (var item in token.Children())
                    {
                        newArr.Add(ProcessTokenValue(item));
                    }
                    return newArr;

                case JTokenType.String:
                    return JToken.FromObject(ProcessStringValue(token.Value<string>()));

                default:
                    return token;
            }
        }

        private static string ProcessStringValue(string value) //html 태그 제거
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // HTML 태그 제거
           string processed = System.Text.RegularExpressions.Regex.Replace(value, "<.*?>", string.Empty);
            // HTML 엔티티 디코딩
            processed = System.Web.HttpUtility.HtmlDecode(processed);
            // 연속된 공백 제거  //20250212 제거
            //processed = System.Text.RegularExpressions.Regex.Replace(processed, @"\s+", " ");

            return processed.Trim();
        }

        private void FormApi_Click(object sender, EventArgs e)
        {
            try
            {
                SaveToTextFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] FormApi_Click error: {ex.Message}");
            }
        }
        private void SaveToTextFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("parsed_data.txt", true))
                {
                    foreach (var rule in parseRules)
                    {
                        writer.WriteLine($"{rule.Tag}: {CurrentValue}");
                    }
                    writer.WriteLine("-------------------");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] SaveToTextFile error: {ex.Message}");
            }
        }

        private void InitializeDataGridView()
        {
            // DataGridView 기본 설정
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            //dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.ReadOnly = false;  // 셀 직접 수정 가능하도록 변경
            dataGridView1.ScrollBars = ScrollBars.Both;  // 스크롤바 표시 설정
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;  // 행 높이 자동 조정
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersWidth = 25;

            dataGridView1.RowHeaderMouseClick += DataGridView1_RowHeaderMouseClick;

            // CurrentValue 컬럼만 읽기 전용으로 설정
            DataGridViewTextBoxColumn colJsonPath = new DataGridViewTextBoxColumn
            {
                Name = "JsonPath",
                HeaderText = GlobalSettings.IsKorean?"JSON 경로":"JSON Path",
                DataPropertyName = "JsonPath",
                Width = 200,
                ReadOnly = false  // 수정 가능
            };

            DataGridViewTextBoxColumn colTag = new DataGridViewTextBoxColumn
            {
                Name = "Tag",
                HeaderText = GlobalSettings.IsKorean ? "태그":"Tag",
                DataPropertyName = "Tag",
                Width = 100,
                ReadOnly = false
            };

            DataGridViewTextBoxColumn colValue = new DataGridViewTextBoxColumn
            {
                Name = "CurrentValue",
                HeaderText = GlobalSettings.IsKorean ? "현재 값":"Current Value",
                // DataPropertyName 제거: ParseRule에 CurrentValue 속성이 없으므로
                // 존재하지 않는 속성 바인딩 시 CurrencyManager -1 인덱스 오류 발생
                // CellFormatting 이벤트에서 currentValues 딕셔너리로 값을 채움
                Width = 100,
                ReadOnly = true
            };

            // 컬럼 추가
            dataGridView1.Columns.AddRange(new DataGridViewColumn[]
            {
        colJsonPath,
        colTag,
        colValue
            });

            // 데이터 바인딩
            bindingSource.DataSource = parseRules;
            dataGridView1.DataSource = bindingSource;

            // DataError 핸들러: 빈 BindingSource 클릭 시 CurrencyManager -1 인덱스 예외 방지
            dataGridView1.DataError += (s, e) =>
            {
                e.ThrowException = false;
                MessageBox.Show($"[FormApi] DataError: Row={e.RowIndex}, Col={e.ColumnIndex}, Exception={e.Exception?.Message}");
            };

            dataGridView1.CellFormatting += (s, e) =>
            {
                try
                {
                    if (e.ColumnIndex == colValue.Index && e.RowIndex >= 0 && e.RowIndex < parseRules.Count)
                    {
                        var rule = parseRules[e.RowIndex];
                        if (rule != null && !string.IsNullOrEmpty(rule.Tag) && currentValues.ContainsKey(rule.Tag))
                        {
                            e.Value = currentValues[rule.Tag];
                            e.FormattingApplied = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[FormApi] CellFormatting error: {ex.Message}");
                }
            };

            dataGridView1.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.F3)
                {
                    e.Handled = true; // F3 키 이벤트 처리 중단
                }
            };

            // 드래그 앤 드롭 설정 추가
            dataGridView1.AllowDrop = true;
            dataGridView1.MouseMove += DataGridView1_MouseMove;
            dataGridView1.MouseDown += DataGridView1_MouseDown;
            dataGridView1.DragOver += DataGridView1_DragOver;
            dataGridView1.DragDrop += DataGridView1_DragDrop;
        }

        private void DataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    // 현재 편집 모드인 경우 해제
                    if (dataGridView1.IsCurrentCellInEditMode)
                    {
                        dataGridView1.EndEdit();
                    }
                    // 임시로 SelectionMode를 FullRowSelect로 변경
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                    // 현재 선택된 행 갱신
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                    selectedRowIndex = e.RowIndex;

                    // TextBox에 선택된 값 표시
                    if (selectedRowIndex >= 0 && selectedRowIndex < parseRules.Count)
                    {
                        txtJsonPath.Text = parseRules[selectedRowIndex].JsonPath;
                        txtTag.Text = parseRules[selectedRowIndex].Tag;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] RowHeaderMouseClick error: {ex.Message}");
            }
        }

        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                // 드래그 시작 행 인덱스 저장
                dragIndex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] MouseDown error: {ex.Message}");
            }
        }

        private void DataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // 마우스 왼쪽 버튼이 눌린 상태에서 이동할 때만 드래그 시작
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left && dragIndex >= 0 && dragIndex < dataGridView1.Rows.Count)
                {
                    // DragDrop 작업 시작
                    dataGridView1.DoDragDrop(dataGridView1.Rows[dragIndex], DragDropEffects.Move);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] MouseMove error: {ex.Message}");
            }
        }

        private void DataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void DataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // 마우스 포인터의 현재 화면 좌표를 클라이언트 좌표로 변환
                Point clientPoint = dataGridView1.PointToClient(new Point(e.X, e.Y));

                // 드롭 위치의 행 인덱스
                int dropIndex = dataGridView1.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

                // 유효한 위치에 드롭된 경우만 처리
                if (dropIndex >= 0 && dropIndex < dataGridView1.Rows.Count && dragIndex >= 0 && dragIndex < parseRules.Count && dropIndex != dragIndex)
                {
                    // 이동할 ParseRule 객체
                    ParseRule movingRule = parseRules[dragIndex];

                    // parseRules 리스트에서 항목 이동
                    parseRules.RemoveAt(dragIndex);
                    parseRules.Insert(dropIndex, movingRule);

                    // 그리드 업데이트
                    UpdateGrid();

                    // 이동된 행 선택
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[dropIndex].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[dropIndex].Cells[0];
                }

                // 드래그 인덱스 초기화
                dragIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] DragDrop error: {ex.Message}");
                dragIndex = -1;
            }
        }

        private void UpdateGrid()
        {
            try
            {
                if (_closing || IsDisposed || !IsHandleCreated)
                    return;

                if (dataGridView1.InvokeRequired)
                {
                    dataGridView1.Invoke(new Action(() => UpdateGrid()));
                    return;
                }

                dataGridView1.SuspendLayout();
                bindingSource.SuspendBinding();

                try
                {
                    bindingSource.ResetBindings(false);

                    // 마지막 행이 보이도록 스크롤
                    if (dataGridView1.Rows.Count > 0)
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
                    }
                }
                finally
                {
                    bindingSource.ResumeBinding();
                    dataGridView1.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(GlobalSettings.IsKorean? $"그리드 업데이트 중 오류가 발생했습니다: {ex.Message}": $"Grid Update Error: {ex.Message}", GlobalSettings.IsKorean ? "오류":"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddRule_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtJsonPath.Text) ||
                string.IsNullOrWhiteSpace(txtTag.Text))
                {
                    if (GlobalSettings.IsKorean) MessageBox.Show("JSON Path와 Tag를 모두 입력해주세요.");
                    else { MessageBox.Show("Please enter both JSON Path and Tag."); }
                    return;
                }

                ParseRule newRule = new ParseRule
                {
                    JsonPath = txtJsonPath.Text,
                    Tag = txtTag.Text
                };

                parseRules.Add(newRule);
                UpdateGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] btnAddRule_Click error: {ex.Message}");
            }
        }

        private void btnDeleteRule_Click(object sender, EventArgs e)
        {
            // 현재 선택된 행이 없는 경우
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show((GlobalSettings.IsKorean)?"삭제할 항목을 선택해주세요.": "Please select the item to delete",
                    (GlobalSettings.IsKorean) ? "알림" :"Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selectedIndex = dataGridView1.CurrentRow.Index;
            if (selectedIndex < 0 || selectedIndex >= parseRules.Count)
            {
                MessageBox.Show((GlobalSettings.IsKorean) ? "잘못된 항목이 선택되었습니다.": "Invalid item selected.", (GlobalSettings.IsKorean) ? "오류":"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 삭제 확인 메시지
            ParseRule selectedRule = dataGridView1.CurrentRow.DataBoundItem as ParseRule;
            if (selectedRule != null)
            {
                DialogResult result;
                if (GlobalSettings.IsKorean)
                {
                   result = MessageBox.Show(
                        $"선택한 규칙을 삭제하시겠습니까?\nJSON Path: {selectedRule.JsonPath}\nTag: {selectedRule.Tag}",
                        "삭제 확인",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                }
                else
                {
                    result = MessageBox.Show(
                    $"Do you want to delete the selected rule?\nJSON Path: {selectedRule.JsonPath}\nTag: {selectedRule.Tag}",
                    "Check Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                }

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        parseRules.Remove(selectedRule);
                        UpdateGrid();
                        ClearInputFields(); // 입력 필드 초기화

                        // 삭제 후 이전 인덱스로 포커스 이동
                        int newIndex = Math.Min(selectedIndex, parseRules.Count - 1);
                        if (newIndex >= 0 && dataGridView1.Rows.Count > 0)
                        {
                            dataGridView1.ClearSelection();
                            dataGridView1.Rows[newIndex].Selected = true;
                            dataGridView1.CurrentCell = dataGridView1.Rows[newIndex].Cells[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(GlobalSettings.IsKorean?$"삭제 중 오류가 발생했습니다: {ex.Message}": $"Error deleting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private FormHelp helpForm = null; // 클래스 멤버로 선언
        private void btnJsonPathHelp_Click(object sender, EventArgs e)
        {
            try
            {
                if (FormHelp.GetInstance(1).Visible)
                {
                    FormHelp.GetInstance(1).Close();
                }
                FormHelp.GetInstance(1).Show(this.Owner);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] btnJsonPathHelp_Click error: {ex.Message}");
            }
        }

        private bool ValidateJsonPath(string jsonPath)
        {
            try
            {
                // Newtonsoft.Json.Linq 사용
                JObject testObj = new JObject();
                JToken.Parse("{}").SelectToken(jsonPath);
                return true;
            }
            catch (Exception)
            {
                if (GlobalSettings.IsKorean)
                    MessageBox.Show("잘못된 JSON Path 형식입니다.", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else MessageBox.Show("Invalid JSON Path format.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    // 현재 편집 모드인 경우 해제
                    if (dataGridView1.IsCurrentCellInEditMode)
                    {
                        dataGridView1.EndEdit();
                    }
                    // 임시로 SelectionMode를 FullRowSelect로 변경
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                    // 현재 선택된 행 갱신
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                    selectedRowIndex = e.RowIndex;

                    // TextBox에 선택된 값 표시
                    if (selectedRowIndex >= 0 && selectedRowIndex < parseRules.Count)
                    {
                        txtJsonPath.Text = parseRules[selectedRowIndex].JsonPath;
                        txtTag.Text = parseRules[selectedRowIndex].Tag;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] CellClick error: {ex.Message}");
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // RowHeader 클릭인 경우 (ColumnIndex가 -1) 처리하지 않음
            if (e.ColumnIndex == -1) return;

            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                // SelectionMode를 다시 CellSelect로 복원
                dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;

                selectedRowIndex = e.RowIndex;
                var row = dataGridView1.Rows[e.RowIndex];

                // 셀 선택 및 편집 모드 설정
                try
                {
                    string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                    DataGridViewCell cell = row.Cells[columnName];

                    // CurrentValue 컬럼이 아닌 경우에만 편집 허용
                    if (columnName != "CurrentValue")
                    {
                        dataGridView1.CurrentCell = cell;
                        dataGridView1.BeginEdit(true);
                    }

                    // TextBox에 선택된 행의 값을 표시
                    txtJsonPath.Text = row.Cells["JsonPath"].Value?.ToString() ?? "";
                    txtTag.Text = row.Cells["Tag"].Value?.ToString() ?? "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[FormApi] 셀 선택 오류: {ex.Message}");
                }

            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {

        }

        private void ClearInputFields()
        {
            txtJsonPath.Clear();
            txtTag.Clear();
            selectedRowIndex = -1;
        }

        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            dataGridView1.ContextMenuStrip = contextMenu;

            // 메뉴 아이템들 추가
            ToolStripMenuItem deleteItem = new ToolStripMenuItem(GlobalSettings.IsKorean ?"삭제":"Delete");
            contextMenu.Items.AddRange(new ToolStripItem[] { deleteItem });

            // 컨텍스트 메뉴 열리기 전
            contextMenu.Opening += (sender, e) =>
            {
                try
                {
                    // 헤더나 빈 영역 클릭시 메뉴 비활성화
                    var hit = dataGridView1.HitTest(
                        dataGridView1.PointToClient(Control.MousePosition).X,
                        dataGridView1.PointToClient(Control.MousePosition).Y);

                    if (hit.RowIndex < 0 || hit.RowIndex >= parseRules.Count)
                    {
                        e.Cancel = true;
                        return;
                    }

                    // 선택된 행 갱신
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[hit.RowIndex].Selected = true;
                    selectedRowIndex = hit.RowIndex;

                    // TextBox에 선택된 값 표시
                    txtJsonPath.Text = parseRules[selectedRowIndex].JsonPath;
                    txtTag.Text = parseRules[selectedRowIndex].Tag;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[FormApi] ContextMenu.Opening error: {ex.Message}");
                    e.Cancel = true;
                }
            };

            // 삭제 메뉴 클릭
            deleteItem.Click += (sender, e) =>
            {
                try
                {
                    if (selectedRowIndex >= 0 && selectedRowIndex < parseRules.Count)
                    {
                        DialogResult result = MessageBox.Show(
                             GlobalSettings.IsKorean ? "선택한 항목을 삭제하시겠습니까?" : "Are you sure you want to delete the selected item?",
                             GlobalSettings.IsKorean ? "삭제 확인" : "Confirm deletion",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            int currentIndex = selectedRowIndex;
                            parseRules.RemoveAt(selectedRowIndex);
                            UpdateGrid();

                            ClearInputFields();
                            // 삭제 후 이전 인덱스로 포커스 이동
                            int newIndex = Math.Min(currentIndex, parseRules.Count - 1);
                            if (newIndex >= 0 && dataGridView1.Rows.Count > 0)
                            {
                                dataGridView1.ClearSelection();
                                dataGridView1.Rows[newIndex].Selected = true;
                                dataGridView1.CurrentCell = dataGridView1.Rows[newIndex].Cells[0];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[FormApi] ContextMenu.Delete error: {ex.Message}");
                }
            };
        }


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    string cellKey = $"{e.RowIndex},{e.ColumnIndex}";

                    // 수정된 값 검증
                    if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                    {
                        MessageBox.Show(GlobalSettings.IsKorean ? "빈 값은 입력할 수 없습니다." : "Empty values cannot be entered.");
                        // 이전 값으로 복원
                        if (previousValues.ContainsKey(cellKey))
                        {
                            cell.Value = previousValues[cellKey];
                        }
                        return;
                    }

                    // JSON Path 검증 (JsonPath 컬럼인 경우)
                    if (e.ColumnIndex == dataGridView1.Columns["JsonPath"].Index)
                    {
                        if (!ValidateJsonPath(cell.Value.ToString()))
                        {
                            // 이전 값으로 복원
                            if (previousValues.ContainsKey(cellKey))
                            {
                                cell.Value = previousValues[cellKey];
                            }
                            return;
                        }
                    }

                    // 검증 통과 후 이전 값 제거
                    previousValues.Remove(cellKey);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] CellEndEdit error: {ex.Message}");
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    string cellKey = $"{e.RowIndex},{e.ColumnIndex}";
                    string currentValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
                    previousValues[cellKey] = currentValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] CellBeginEdit error: {ex.Message}");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                    string.IsNullOrWhiteSpace(txtUrl.Text) ||
                    numericInterval.Value <= 0 ||
                    parseRules.Count == 0)
                {
                    MessageBox.Show(GlobalSettings.IsKorean ? "모든 필수 정보를 입력해주세요." : "Please enter all required information.");
                    return;
                }

                // Title 중복 체크 - 원래 제목이 아닌 다른 제목과의 중복만 체크
                var configs = RestApiMonitorService.LoadAllConfigs();
                bool isDuplicate = configs.Any(c =>
                    c.Title.Equals(txtTitle.Text, StringComparison.OrdinalIgnoreCase) &&
                    !c.Title.Equals(originalTitle, StringComparison.OrdinalIgnoreCase)  // 원래 제목과 다른 경우만 체크
                );

                if (isDuplicate)
                {
                    MessageBox.Show(GlobalSettings.IsKorean ? "이미 존재하는 제목입니다. 다른 제목을 입력해주세요." : "This title already exists. Please enter a different title.",
                        GlobalSettings.IsKorean ? "중복 오류" : "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitle.Focus();
                    return;
                }

                // 제목이 변경되었는지 확인
                bool isTitleChanged = !string.IsNullOrEmpty(originalTitle) &&
                                     !originalTitle.Equals(txtTitle.Text, StringComparison.OrdinalIgnoreCase);

                // 기존 클라이언트 중지
                if (!string.IsNullOrEmpty(originalTitle))
                {
                    RestApiMonitorService.StopClient?.Invoke(originalTitle);
                }

                // ParseRule 목록을 ApiClientInfo 형식으로 변환
                var clientInfo = new ApiClientInfo
                {
                    Title = txtTitle.Text,
                    Url = txtUrl.Text,
                    IntervalSeconds = (int)numericInterval.Value,
                    IsRunning = checkBoxUse.Checked, // 체크박스 상태 저장
                    ParseRules = parseRules.Select(r => new ParseRule
                    {
                        JsonPath = r.JsonPath,
                        Tag = r.Tag,
                    }).ToList(),
                    NotFoundText = txtNotFoundText.Text,
                    UseNotFoundText = chkUseNotFound.Checked,
                    Headers = headers
                };

                // 저장 및 처리
                if (isTitleChanged)
                {
                    // 기존 설정 삭제 후 새 설정 저장
                    var allConfigs = RestApiMonitorService.LoadAllConfigs();
                    allConfigs.RemoveAll(x => x.Title.Equals(originalTitle, StringComparison.OrdinalIgnoreCase));
                    allConfigs.Add(clientInfo);
                    RestApiMonitorService.SaveAllConfigs(allConfigs);
                }
                else
                {
                    RestApiMonitorService.SaveApiClient(clientInfo);
                }

                ModifiedTitle = txtTitle.Text;  // 수정된 제목 저장
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] btnOK_Click error: {ex.Message}");
                MessageBox.Show(GlobalSettings.IsKorean ? $"저장 중 오류가 발생했습니다: {ex.Message}" : $"Error saving: {ex.Message}",
                    GlobalSettings.IsKorean ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelectTag_Click(object sender, EventArgs e)
        {
            try
            {
                DialogTag.SelectTag dialog = new DialogTag.SelectTag();

                dialog.bUseTagAI = true;
                dialog.bUseTagAO = true;
                dialog.bUseTagDI = true;
                dialog.bUseTagDO = true;
                dialog.bUseTagST = true;

                if (dialog.Run() == DialogResult.OK)
                {
                    txtTag.Text = dialog.sTag;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] btnSelectTag_Click error: {ex.Message}");
            }
        }

        private void chkUseNotFound_CheckedChanged(object sender, EventArgs e)
        {
            txtNotFoundText.Enabled = chkUseNotFound.Checked;
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {
            UpdateUrlPreview();
        }

        private void UpdateUrlPreview()
        {
            try
            {
                string previewUrl = UrlFormatter.FormatUrl(txtUrl.Text);
                var controls = this.Controls.Find("txtUrlPreview", true);
                if (controls != null && controls.Length > 0)
                {
                    var txtPreview = controls[0] as TextBox;
                    if (txtPreview != null)
                    {
                        txtPreview.Text = previewUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] UpdateUrlPreview error: {ex.Message}");
            }
        }

        private void txtTitle_MouseHover(object sender, EventArgs e)
        {
            try
            {
                this.toolTip1.ToolTipTitle = GlobalSettings.IsKorean ? "제목" : "Title";
                string invalidChars = "\\ / : * ? \" < > |";
                if (GlobalSettings.IsKorean)
                {
                    this.toolTip1.SetToolTip(txtTitle,
                        $"로그 저장 시 다음 문자들은 자동으로 '_' (언더바)로 치환됩니다.\n {invalidChars}");
                }
                else
                {
                    this.toolTip1.SetToolTip(txtTitle,
                        $"When saving logs, the following characters will be automatically replaced with '_' (underscore).\n {invalidChars}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] txtTitle_MouseHover error: {ex.Message}");
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSetHeaders_Click(object sender, EventArgs e)
        {
            try
            {
                var clientInfo = (RestApiMonitorService.LoadAllConfigs()).FirstOrDefault(c => c.Title == txtTitle.Text);

                if (clientInfo == null)
                {
                    // clientInfo가 없으면 새로 생성
                    clientInfo = new ApiClientInfo
                    {
                        Headers = headers
                    };

                    headers = clientInfo.Headers;
                }

                using (FormRequestHeader formRequestHeader = new FormRequestHeader(headers))
                {
                    if (formRequestHeader.ShowDialog() == DialogResult.OK)
                    {
                        headers = formRequestHeader.GetHeaders();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] btnSetHeaders_Click error: {ex.Message}");
            }
        }

        private void SaveHeadersToConfig(List<(string Name, string Value, bool Encrypt)> headers)
        {
            var clientInfo = (RestApiMonitorService.LoadAllConfigs()).FirstOrDefault(c => c.Title == txtTitle.Text);
            if (clientInfo != null)
            {
                clientInfo.Headers = headers;
                RestApiMonitorService.SaveApiClient(clientInfo);
            }
        }

        private void btnUrlCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.txtUrlPreview.Text))
                {
                    Clipboard.SetText(this.txtUrlPreview.Text);
                    MessageBox.Show(GlobalSettings.IsKorean ? "URL이 복사되었습니다." : "URL has been copied.",
                        GlobalSettings.IsKorean ? "알림" : "Notification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] btnUrlCopy_Click error: {ex.Message}");
            }
        }

        private volatile bool _closing;

        private void FormApi_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closing = true;

            try
            {
                // 1. 포커스 제거
                this.ActiveControl = null;

                // 2. 편집 상태 종료
                if (dataGridView1.IsCurrentCellInEditMode)
                {
                    dataGridView1.EndEdit();
                }

                // 3. 접근성 차단
                dataGridView1.AccessibleRole = AccessibleRole.None;

                // 2. DataGridView 선택 해제
                dataGridView1.ClearSelection();

                // 4. 선택 / 현재 셀 제거
                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;

                // 5. DataSource 분리 (CurrencyManager 해제)
                dataGridView1.DataSource = null;
                bindingSource.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormApi] FormClosing error: {ex.Message}");
            }
        }
    }
}

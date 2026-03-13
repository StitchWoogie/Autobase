using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using GraphicModule;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoLibLocal;
using DialogTag;
using NetTools;


namespace Studio
{
    public partial class FormConfigJsonTemplate : Form
    {

        // 추가된 변수들
        private int modifiedFlag = 0;
        private string currentFilePath = string.Empty;
        private string originalTitle;
        private bool _isLangKorean = false;

        public FormConfigJsonTemplate()
        {
            InitializeComponent();
            _isLangKorean = Tools.IsLangKorean();
            InitializeCustomComponents();
            originalTitle = this.Text;
        }

        private void InitializeCustomComponents()
        {
            // 기본 템플릿 텍스트 설정
            string exampleTemplate = @"{
  ""TemplateName"":""ExampleTemplate1"",
  ""deviceId"": ""#DEVICE_ID#"",
  ""timestamp"": ""#TIME#"",
  ""measurement"": {
    ""temperature"": ""#$AI_0000#"",
    ""pressure"": ""#$AI_0001#"",
    ""flow_rate"": ""#$AI_0002#"",
    ""level"": ""#$AI_0003#""
  },
  ""control"": {
    ""pump_status"": ""#$DI_0000#"",
    ""valve_position"": ""#$AI_0000#"",
    ""motor_speed"": ""#$AO_0000#"",
    ""emergency_stop"": ""#DI_0001#""
  },
  ""settings"": {
    ""high_alarm"": ""#$AI_0000.hihi#"",
    ""low_alarm"": ""#$AI_0000.lolo#"",
    ""pid_setpoint"": ""#$ST_0000#""
  },
  ""status"": {
    ""error_code"": ""#$ST_0001#"",
    ""comment1"": ""#comment1#"",
    ""#comment2#"": ""#comment3#""
  }
}";
            rtbEditor.Text = exampleTemplate;

            if (_isLangKorean)
            {
                lbTemplateID.Text = "템플릿 ID";
                btnFormat.Text = "정렬";
                btnLoad.Text = "불러오기";
                btnSave.Text = "저장";
                btnValidate.Text = "유효성 검사";
                btnTag.Text = "태그";
            }
        }

        private void RtbEditor_TextChanged(object sender, EventArgs e)
        {
            if (modifiedFlag == 0)
            {
                modifiedFlag = 1;
                this.Text = this.Text + " *";
            }
        }

        private void BtnExample_Click(object sender, EventArgs e)
        {
            string exampleTemplate = @"{
    ""deviceId"": ""#DEVICE_ID#"",
    ""timestamp"": ""#TIME#"",
    ""measurement"": {
        ""temperature"": ""#ST001#"",
        ""pressure"": ""#ST002#"",
        ""flow_rate"": ""#ST003#"",
        ""level"": ""#ST004#""
    },
    ""control"": {
        ""pump_status"": ""#DI001#"",
        ""valve_position"": ""#AI001#"",
        ""motor_speed"": ""#AO001#"",
        ""emergency_stop"": ""#DO001#""
    },
    ""settings"": {
        ""high_alarm"": ""#ST005#"",
        ""low_alarm"": ""#ST006#"",
        ""pid_setpoint"": ""#ST007#""
    },
    ""status"": {
        ""is_running"": ""#ST008#"",
        ""error_code"": ""#ST009#"",
        ""maintenance_required"": ""#ST010#""
    }
}

// ===== 사용 예시 =====
{
    ""deviceId"": ""PUMP_01"",               // 문자열
    ""timestamp"": ""2024-01-22 14:30:00"",  // 날짜시간
    ""measurement"": {
        ""temperature"": 85.5,              // ST001: 실수형
        ""pressure"": 3.2,                  // ST002: 실수형
        ""flow_rate"": 120.7,              // ST003: 실수형
        ""level"": 75.2                    // ST004: 실수형
    },
    ""control"": {
        ""pump_status"": true,             // DI001: 부울형
        ""valve_position"": 65.5,          // AI001: 실수형
        ""motor_speed"": 1750,             // AO001: 정수형
        ""emergency_stop"": false          // DO001: 부울형
    },
    ""settings"": {
        ""high_alarm"": 90.0,              // ST005: 실수형
        ""low_alarm"": 20.0,               // ST006: 실수형
        ""pid_setpoint"": 50.0             // ST007: 실수형
    },
    ""status"": {
        ""is_running"": true,              // ST008: 부울형
        ""error_code"": 0,                 // ST009: 정수형
        ""maintenance_required"": false     // ST010: 부울형
    }
}";

            rtbEditor.Text = exampleTemplate;
            txtTemplateId.Text = "exampleTemplate";
            this.Text = originalTitle;

        }


        private void BtnFormat_Click(object sender, EventArgs e)
        {
            try
            {
                // JSON 파싱 후 포맷팅된 문자열로 변환
                string jsonText = rtbEditor.Text;
                JToken parsedJson = JToken.Parse(jsonText);
                string formattedJson = parsedJson.ToString(Formatting.Indented);
                rtbEditor.Text = formattedJson;
            }
            catch (Exception ex)
            {
                string message = _isLangKorean ? "JSON 포맷팅 중 오류가 발생했습니다." : "An error occurred while formatting JSON.";
                MessageBox.Show(message + "\n" + ex.Message,
                    _isLangKorean ? "오류" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertTag()
        {
            FormSelectTag dialog = new FormSelectTag();
            dialog.bUseTagAI = true;
            dialog.bUseTagAO = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagDO = true;
            dialog.bUseTagST = true;
            dialog.bUseTagGDO = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                // 현재 커서 위치에 태그 삽입
                string tagText = "#$" + dialog.sTag + "#";
                rtbEditor.SelectedText = tagText;

                // 커서를 삽입된 텍스트 뒤로 이동
                rtbEditor.SelectionStart = rtbEditor.SelectionStart + tagText.Length;
                rtbEditor.SelectionLength = 0;
                rtbEditor.Focus();
            }
        }

        private void ButtonTag_Click(object sender, System.EventArgs e)
        {
            InsertTag();
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtTemplateId.Text))
                {
                    MessageBox.Show(
                       _isLangKorean ? "템플릿 ID를 입력하세요." : "Please enter a template ID.",
                       _isLangKorean ? "알림" : "Notice",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 저장 확인 메시지 박스
                var result = MessageBox.Show(
                     _isLangKorean ? "이 템플릿을 저장하시겠습니까?" : "Do you want to save this template?",
                     _isLangKorean ? "저장 확인" : "Save Confirmation",
                     MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Cancel) return;

                // JSON 유효성 검사
                JToken.Parse(rtbEditor.Text);

                // 템플릿 저장
                GraphicModule.ScriptFunctionJsonTemplate.JsonTemplateManager.SaveTemplate(txtTemplateId.Text, rtbEditor.Text);

                MessageBox.Show(
                    _isLangKorean ? "템플릿이 저장되었습니다." : "Template has been saved.",
                    _isLangKorean ? "성공" : "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);


                string folderPath = Path.Combine(TotalConfig.sDirWorkProject, "JSON");

                string fullpath = Path.Combine(folderPath, txtTemplateId.Text + ".json");
                currentFilePath = fullpath;

                modifiedFlag = 0; // 저장 후 수정 플래그 초기화
                this.Text = originalTitle + " - " + currentFilePath; // * 제거
            }
            catch (Exception ex)
            {
                string message = _isLangKorean ? "저장 중 오류가 발생했습니다." : "An error occurred while saving.";
                MessageBox.Show(message + "\n" + ex.Message,
                    _isLangKorean ? "오류" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = Path.Combine(TotalConfig.sDirWorkProject, "JSON");
                    openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 파일 경로 저장
                        currentFilePath = openFileDialog.FileName;

                        // 파일 경로에서 파일명을 가져와 템플릿 ID로 설정 (확장자 제외)
                        string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        txtTemplateId.Text = fileName;

                        // 파일 내용 읽기
                        string jsonContent = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);

                        // JSON 유효성 검사
                        JToken.Parse(jsonContent);

                        // 에디터에 내용 표시
                        rtbEditor.Text = jsonContent;

                        // 폼 제목 업데이트
                        this.Text = originalTitle + " - " + currentFilePath;
                        modifiedFlag = 0; // 수정 플래그 초기화
                    }
                }
            }
            catch (Exception ex)
            {
                string message = _isLangKorean ? "템플릿을 불러오는 중 오류가 발생했습니다." : "An error occurred while loading the template.";
                MessageBox.Show(message + "\n" + ex.Message,
                    _isLangKorean ? "오류" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                JToken.Parse(rtbEditor.Text);
                MessageBox.Show(
                    _isLangKorean ? "유효한 JSON 형식입니다." : "Valid JSON format.",
                    _isLangKorean ? "검증 성공" : "Validation Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string message = _isLangKorean ? "JSON 형식이 올바르지 않습니다." : "Invalid JSON format.";
                MessageBox.Show(message + "\n" + ex.Message,
                    _isLangKorean ? "검증 실패" : "Validation Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormConfigJson_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (modifiedFlag == 1)
            {
                string message = _isLangKorean ? "변경된 내용을 저장하시겠습니까?" : "Do you want to save the changes?";
                string caption = _isLangKorean ? "저장 확인" : "Save Confirmation";

                DialogResult result = MessageBox.Show(
                    message,
                    caption,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    BtnSave_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // 폼 닫기 취소
                }
            }
        }

        private void rtbEditor_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int line = rtbEditor.GetLineFromCharIndex(rtbEditor.SelectionStart) + 1;
                int column = rtbEditor.SelectionStart - rtbEditor.GetFirstCharIndexOfCurrentLine() + 1;

                // StatusBar에 현재 커서 위치 표시
                if (_isLangKorean)
                {
                    statusBar.Text = String.Format("    줄: {0}, 열: {1}", line, column);
                }
                else statusBar.Text = String.Format("    Line: {0}, Column: {1}", line, column);
            }
            catch (Exception)
            {

            }
        }

    }



}

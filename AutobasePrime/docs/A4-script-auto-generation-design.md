# A4. 스크립트 자동 생성 & 어시스턴트 - 상세 구현 설계

## 1. 개요

SCADA 운전원/엔지니어가 자연어로 동작을 설명하면 AI가 Autobase 스크립트 코드를 자동 생성하는 기능.
기존 100+개 ScriptFunction 클래스의 1000+ 메서드 시그니처를 LLM 컨텍스트로 활용.

**핵심 가치:** 비전문가도 복잡한 SCADA 스크립트 작성 가능, 개발 시간 90% 단축

---

## 2. 아키텍처

```
┌─────────────────────────────────────────────────────────┐
│ Studio Script Editor (FormNewScriptEditor)               │
│  ┌─────────────────────────────────────────────────┐    │
│  │ AI 어시스턴트 패널 (FormScriptAiAssistant)       │    │
│  │  ┌───────────────────────────────────────────┐  │    │
│  │  │ 자연어 입력                                │  │    │
│  │  │ "온도가 80도 넘으면 펌프 정지하고 알람"     │  │    │
│  │  └───────────────────────────────────────────┘  │    │
│  │  [생성] [설명] [수정] [삽입]                     │    │
│  │  ┌───────────────────────────────────────────┐  │    │
│  │  │ 생성된 코드 미리보기                       │  │    │
│  │  │ if (Tag.GetAnalog("Temp_01") > 80.0)      │  │    │
│  │  │ {                                          │  │    │
│  │  │     Tag.SetDigital("Pump_01", 0);          │  │    │
│  │  │     Alarm.SetAlarm("Temp_01", "HiHi");     │  │    │
│  │  │ }                                          │  │    │
│  │  └───────────────────────────────────────────┘  │    │
│  └─────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────┐    │
│  │ 기존 코드 에디터 (UserControlScriptEditor)       │    │
│  └─────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────┘
         │
         ▼
┌──────────────────────┐     ┌──────────────────────────┐
│ ScriptAiService      │────▶│ python_ai_engine          │
│ (C# 클라이언트)       │     │ script_codegen_service    │
│                      │     │ (신규 서비스)              │
│ - BuildPrompt()      │     │                          │
│ - ParseResponse()    │     │ - LLM API 호출            │
│ - ValidateCode()     │     │ - 함수 시그니처 DB        │
└──────────────────────┘     │ - 코드 검증               │
                             └──────────────────────────┘
```

---

## 3. C# 측 구현

### 3.1 FormScriptAiAssistant (신규 Form)

**파일:** `Studio/Script/FormScriptAiAssistant.cs`

```csharp
public class FormScriptAiAssistant : UserControl
{
    // UI 컨트롤
    private TextBox txtNaturalLanguage;        // 자연어 입력
    private RichTextBox rtbGeneratedCode;       // 생성 코드 미리보기
    private Button btnGenerate;                 // 생성 버튼
    private Button btnInsert;                   // 에디터에 삽입
    private Button btnExplain;                  // 코드 설명 요청
    private Button btnModify;                   // 수정 요청
    private ComboBox cmbMode;                   // 모드: 신규생성/수정/설명
    private ListBox lstHistory;                 // 대화 이력
    private CheckBox chkIncludeTagList;         // 태그 목록 포함 여부
    private Label lblStatus;                    // 상태 표시

    // 콜백
    public Action<string> OnInsertCode;         // 에디터에 코드 삽입 콜백
    public Func<string> OnGetSelectedCode;      // 에디터 선택 코드 가져오기

    // 태그 참조 (자동완성용)
    private TagGrClass tagRoot;

    // 코드 생성 요청
    private async void btnGenerate_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "AI 코드 생성 중...";
        btnGenerate.Enabled = false;

        try
        {
            string userInput = txtNaturalLanguage.Text;
            string existingCode = OnGetSelectedCode?.Invoke() ?? "";
            string mode = cmbMode.SelectedItem.ToString(); // "generate" | "modify" | "explain"

            // 태그 목록 수집 (선택 시)
            string tagContext = "";
            if (chkIncludeTagList.Checked)
                tagContext = BuildTagContext();

            var request = new ScriptAiRequest
            {
                UserPrompt = userInput,
                Mode = mode,
                ExistingCode = existingCode,
                TagContext = tagContext,
                ProjectContext = BuildProjectContext()
            };

            string result = await ScriptAiService.GenerateAsync(request);
            var response = ScriptAiService.ParseResponse(result);

            rtbGeneratedCode.Text = response.Code;
            HighlightCode(response.Code);

            // 이력 추가
            lstHistory.Items.Add($"[{DateTime.Now:HH:mm}] {userInput.Substring(0, Math.Min(50, userInput.Length))}...");
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"오류: {ex.Message}";
        }
        finally
        {
            btnGenerate.Enabled = true;
        }
    }

    // 생성된 코드를 에디터에 삽입
    private void btnInsert_Click(object sender, EventArgs e)
    {
        string code = rtbGeneratedCode.Text;
        if (!string.IsNullOrEmpty(code))
        {
            OnInsertCode?.Invoke(code);
        }
    }

    // 프로젝트 태그 컨텍스트 구축
    private string BuildTagContext()
    {
        var sb = new StringBuilder();
        sb.AppendLine("// 사용 가능한 태그 목록:");
        tagRoot = TagLib.groupRoot;
        CollectTags(tagRoot, sb, 0);
        return sb.ToString();
    }

    private void CollectTags(TagGrClass group, StringBuilder sb, int depth)
    {
        if (depth > 3) return; // 깊이 제한
        foreach (var item in group.arrayTag)
        {
            if (item is TagAiClass ai)
                sb.AppendLine($"// AI: {ai.tag} - {ai.description} ({ai.unit}, {ai.fBase}~{ai.fFull})");
            else if (item is TagDiClass di)
                sb.AppendLine($"// DI: {di.tag} - {di.description}");
            else if (item is TagAoClass ao)
                sb.AppendLine($"// AO: {ao.tag} - {ao.description} ({ao.unit})");
            else if (item is TagDoClass dobj)
                sb.AppendLine($"// DO: {dobj.tag} - {dobj.description}");
            else if (item is TagStClass st)
                sb.AppendLine($"// ST: {st.tag} - {st.description}");
            else if (item is TagGrClass subGroup)
                CollectTags(subGroup, sb, depth + 1);
        }
    }

    // 프로젝트 컨텍스트 (현재 화면 정보 등)
    private string BuildProjectContext()
    {
        return $"프로젝트: {TotalConfig.sProjectName}, " +
               $"현재 모듈: {TotalConfig.sCurrentModule}";
    }
}
```

### 3.2 FormNewScriptEditor 통합

**파일:** `Studio/Script/FormNewScriptEditor.cs` (기존 수정)

```csharp
// FormNewScriptEditor.cs에 추가할 코드

// 필드 추가
private FormScriptAiAssistant aiAssistant;
private SplitContainer splitMain;
private ToolStripButton btnToggleAi;

// InitializeComponent() 또는 생성자에서
private void SetupAiAssistant()
{
    // AI 어시스턴트 패널 생성
    aiAssistant = new FormScriptAiAssistant();
    aiAssistant.Dock = DockStyle.Fill;
    aiAssistant.OnInsertCode = InsertCodeToEditor;
    aiAssistant.OnGetSelectedCode = GetSelectedCode;

    // SplitContainer로 에디터/AI 패널 분할
    splitMain = new SplitContainer();
    splitMain.Dock = DockStyle.Fill;
    splitMain.Orientation = Orientation.Horizontal;
    splitMain.SplitterDistance = (int)(this.Height * 0.65);
    splitMain.Panel1.Controls.Add(formChild);      // 기존 에디터
    splitMain.Panel2.Controls.Add(aiAssistant);    // AI 패널

    this.Controls.Add(splitMain);

    // 툴바에 AI 토글 버튼 추가
    btnToggleAi = new ToolStripButton("AI 어시스턴트");
    btnToggleAi.Click += (s, e) => {
        splitMain.Panel2Collapsed = !splitMain.Panel2Collapsed;
    };
}

private void InsertCodeToEditor(string code)
{
    // 현재 커서 위치에 코드 삽입
    formChild.panelEditor.InsertText(code);
}

private string GetSelectedCode()
{
    return formChild.panelEditor.GetSelectedText();
}
```

### 3.3 ScriptAiService (신규 서비스 클래스)

**파일:** `Studio/Script/ScriptAiService.cs`

```csharp
public static class ScriptAiService
{
    // 스크립트 함수 시그니처 캐시
    private static string _functionSignaturesCache = null;

    public static async Task<string> GenerateAsync(ScriptAiRequest request)
    {
        // 함수 시그니처 로드 (최초 1회)
        if (_functionSignaturesCache == null)
            _functionSignaturesCache = BuildFunctionSignatures();

        var payload = new JObject
        {
            ["user_prompt"] = request.UserPrompt,
            ["mode"] = request.Mode,
            ["existing_code"] = request.ExistingCode,
            ["tag_context"] = request.TagContext,
            ["project_context"] = request.ProjectContext,
            ["function_signatures"] = _functionSignaturesCache
        };

        // PythonAiScriptBridge를 통해 호출
        string result = await PythonAiManager.CallAsync(
            "codegen/script", payload.ToString());

        return result;
    }

    // ScriptExternalRun에서 등록된 모든 함수 시그니처 추출
    private static string BuildFunctionSignatures()
    {
        var sb = new StringBuilder();
        var externalRun = new ScriptExternalRun();

        // 그룹별 함수 시그니처 수집
        foreach (var group in externalRun.arrayGroup)
        {
            sb.AppendLine($"\n// === {group.name} ===");
            foreach (var method in group.arrayMethod)
            {
                sb.Append($"{method.returnType} {method.name}(");
                for (int i = 0; i < method.args.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append($"{method.args[i].type} {method.args[i].name}");
                }
                sb.AppendLine(")");
            }
        }

        return sb.ToString();
    }

    public static ScriptAiResponse ParseResponse(string json)
    {
        var obj = JObject.Parse(json);
        return new ScriptAiResponse
        {
            Code = obj["Result"]?["code"]?.ToString() ?? "",
            Explanation = obj["Result"]?["explanation"]?.ToString() ?? "",
            Warnings = obj["Result"]?["warnings"]?.ToObject<List<string>>()
                       ?? new List<string>()
        };
    }
}

public class ScriptAiRequest
{
    public string UserPrompt { get; set; }
    public string Mode { get; set; }        // "generate" | "modify" | "explain"
    public string ExistingCode { get; set; }
    public string TagContext { get; set; }
    public string ProjectContext { get; set; }
}

public class ScriptAiResponse
{
    public string Code { get; set; }
    public string Explanation { get; set; }
    public List<string> Warnings { get; set; }
}
```

---

## 4. Python 측 구현

### 4.1 codegen_service.py (신규 서비스)

**파일:** `python_ai_engine/services/codegen_service.py`

```python
"""SCADA 스크립트 코드 자동 생성 서비스"""

import json
import re
from typing import Any

# LLM 호출 (OpenAI 호환 API 또는 로컬 모델)
_llm_client = None
_function_db = None

SYSTEM_PROMPT = """당신은 Autobase SCADA 스크립트 전문가입니다.
사용자의 자연어 요청을 Autobase 스크립트 코드로 변환합니다.

규칙:
1. Autobase 스크립트 문법만 사용 (C# 유사 문법)
2. 제공된 함수 시그니처만 사용
3. 태그 이름은 사용자가 제공한 태그 목록에서 선택
4. 존재하지 않는 함수를 만들어내지 않기
5. 코드는 즉시 실행 가능해야 함
6. 한글 주석으로 각 블록 설명 추가

응답 형식 (JSON):
{
    "code": "생성된 코드",
    "explanation": "코드 동작 설명",
    "warnings": ["주의사항 목록"],
    "used_functions": ["사용된 함수 목록"],
    "used_tags": ["사용된 태그 목록"]
}
"""

async def register(router, **deps):
    """서비스 등록"""
    global _llm_client, _function_db
    config = deps.get("config")

    # LLM 클라이언트 초기화
    _llm_client = _create_llm_client(config)

    router.add_handler("codegen/script", handle_script_codegen)
    router.add_handler("codegen/explain", handle_script_explain)
    router.add_handler("codegen/modify", handle_script_modify)


async def handle_script_codegen(payload: dict) -> dict:
    """자연어 → 스크립트 코드 생성"""
    user_prompt = payload.get("user_prompt", "")
    function_signatures = payload.get("function_signatures", "")
    tag_context = payload.get("tag_context", "")
    project_context = payload.get("project_context", "")

    # 프롬프트 구성
    messages = [
        {"role": "system", "content": SYSTEM_PROMPT},
        {"role": "system", "content": f"사용 가능한 함수 시그니처:\n{function_signatures}"},
    ]

    if tag_context:
        messages.append({
            "role": "system",
            "content": f"프로젝트 태그 목록:\n{tag_context}"
        })

    if project_context:
        messages.append({
            "role": "system",
            "content": f"프로젝트 정보: {project_context}"
        })

    messages.append({"role": "user", "content": user_prompt})

    # LLM 호출
    response = await _call_llm(messages)

    # 응답 파싱 및 검증
    result = _parse_and_validate(response, function_signatures)

    return result


async def handle_script_explain(payload: dict) -> dict:
    """기존 코드 설명"""
    code = payload.get("existing_code", "")
    function_signatures = payload.get("function_signatures", "")

    messages = [
        {"role": "system", "content": "Autobase SCADA 스크립트를 분석하여 한글로 설명합니다."},
        {"role": "system", "content": f"함수 레퍼런스:\n{function_signatures}"},
        {"role": "user", "content": f"다음 코드를 설명해주세요:\n```\n{code}\n```"}
    ]

    response = await _call_llm(messages)
    return {"explanation": response}


async def handle_script_modify(payload: dict) -> dict:
    """기존 코드 수정"""
    code = payload.get("existing_code", "")
    modification = payload.get("user_prompt", "")
    function_signatures = payload.get("function_signatures", "")
    tag_context = payload.get("tag_context", "")

    messages = [
        {"role": "system", "content": SYSTEM_PROMPT},
        {"role": "system", "content": f"함수 시그니처:\n{function_signatures}"},
    ]

    if tag_context:
        messages.append({"role": "system", "content": f"태그:\n{tag_context}"})

    messages.append({
        "role": "user",
        "content": f"기존 코드:\n```\n{code}\n```\n\n수정 요청: {modification}"
    })

    response = await _call_llm(messages)
    result = _parse_and_validate(response, function_signatures)
    return result


def _parse_and_validate(llm_response: str, function_signatures: str) -> dict:
    """LLM 응답 파싱 및 코드 검증"""
    try:
        # JSON 블록 추출
        json_match = re.search(r'\{[\s\S]*\}', llm_response)
        if json_match:
            result = json.loads(json_match.group())
        else:
            result = {"code": llm_response, "explanation": "", "warnings": []}
    except json.JSONDecodeError:
        result = {"code": llm_response, "explanation": "", "warnings": []}

    # 코드 검증
    code = result.get("code", "")
    warnings = result.get("warnings", [])

    # 위험한 패턴 검사
    dangerous_patterns = [
        (r'System\.IO\.File\.Delete', "파일 삭제 함수 사용"),
        (r'Process\.Start', "외부 프로세스 실행"),
        (r'System\.Diagnostics', "시스템 진단 접근"),
        (r'DROP\s+TABLE', "SQL 테이블 삭제"),
        (r'DELETE\s+FROM.*WHERE\s+1\s*=\s*1', "전체 데이터 삭제"),
    ]
    for pattern, warning in dangerous_patterns:
        if re.search(pattern, code, re.IGNORECASE):
            warnings.append(f"⚠ 주의: {warning}")

    result["warnings"] = warnings
    return result


async def _call_llm(messages: list) -> str:
    """LLM API 호출 (OpenAI 호환)"""
    if _llm_client is None:
        return _fallback_template_generation(messages)

    response = await _llm_client.chat.completions.create(
        model="gpt-4o",  # 또는 로컬 모델
        messages=messages,
        temperature=0.2,
        max_tokens=2000,
        response_format={"type": "json_object"}
    )
    return response.choices[0].message.content


def _fallback_template_generation(messages: list) -> str:
    """LLM 미사용 시 템플릿 기반 생성 (폴백)"""
    user_msg = next((m["content"] for m in messages if m["role"] == "user"), "")

    # 키워드 기반 간단한 템플릿 매칭
    templates = {
        "온도.*초과|온도.*넘으면": '''
{
    "code": "// 온도 초과 시 처리\\nfloat temp = (float)@Tag.GetAnalog(\\"Temperature_01\\");\\nif (temp > {threshold})\\n{\\n    @Tag.SetDigital(\\"Pump_01_Run\\", 0);\\n    @Alarm.SetAlarm(\\"Temperature_01\\", \\"HiHi\\");\\n}",
    "explanation": "온도 태그값이 임계값을 초과하면 펌프를 정지하고 알람을 발생시킵니다.",
    "warnings": ["태그 이름을 실제 프로젝트 태그로 변경하세요"],
    "used_functions": ["Tag.GetAnalog", "Tag.SetDigital", "Alarm.SetAlarm"]
}''',
        "펌프.*기동|펌프.*시작": '''
{
    "code": "// 펌프 기동\\n@Tag.SetDigital(\\"Pump_01_Run\\", 1);\\n@Log.Write(\\"펌프 기동 명령 전송\\");",
    "explanation": "지정된 펌프의 운전 태그를 1로 설정하여 기동합니다.",
    "warnings": [],
    "used_functions": ["Tag.SetDigital", "Log.Write"]
}'''
    }

    for pattern, template in templates.items():
        if re.search(pattern, user_msg):
            return template

    return json.dumps({
        "code": "// TODO: 수동으로 코드를 작성하세요\n// 요청: " + user_msg,
        "explanation": "자동 생성을 위해 LLM 연동이 필요합니다.",
        "warnings": ["LLM이 설정되지 않아 템플릿만 제공됩니다"]
    })


def _create_llm_client(config):
    """LLM 클라이언트 생성"""
    if config and hasattr(config, 'llm_api_key') and config.llm_api_key:
        try:
            from openai import AsyncOpenAI
            return AsyncOpenAI(
                api_key=config.llm_api_key,
                base_url=getattr(config, 'llm_base_url', None)
            )
        except ImportError:
            return None
    return None
```

### 4.2 main.py 등록

```python
# main.py에 추가
from services import codegen_service

async def main(host, port, config_path):
    # ... 기존 서비스 등록 ...
    await codegen_service.register(router, config=config)
```

### 4.3 Router 권한 설정

```python
# router/auth.py DEFAULT_PERMISSIONS에 추가
"codegen/*": "codegen.execute"
```

---

## 5. 함수 시그니처 DB 구축

### 5.1 자동 추출 방식

ScriptExternalRun 생성자에서 PrepareMethod()를 통해 등록되는 모든 메서드를 JSON으로 직렬화:

```csharp
// ScriptAiService.cs 내부
public static string ExportFunctionSignaturesAsJson()
{
    var externalRun = new ScriptExternalRun();
    var groups = new JArray();

    foreach (var group in externalRun.arrayGroup)
    {
        var gObj = new JObject
        {
            ["group"] = group.name,
            ["methods"] = new JArray()
        };

        foreach (var method in group.arrayMethod)
        {
            var mObj = new JObject
            {
                ["name"] = method.name,
                ["returnType"] = method.returnType,
                ["description"] = method.description ?? "",
                ["args"] = new JArray(method.args.Select(a =>
                    new JObject
                    {
                        ["direction"] = a.direction,    // "in", "out", "ref"
                        ["type"] = a.type,              // "int", "string", "double"
                        ["name"] = a.name
                    }))
            };
            ((JArray)gObj["methods"]).Add(mObj);
        }
        groups.Add(gObj);
    }

    return groups.ToString(Formatting.Indented);
}
```

### 5.2 시그니처 카테고리 (LLM 프롬프트용 요약)

```
주요 카테고리 (LLM에 전달할 핵심 함수):

[태그 읽기/쓰기]
- float Tag.GetAnalog(string tagName) - AI 태그 현재값
- int Tag.GetDigital(string tagName) - DI 태그 현재값
- string Tag.GetString(string tagName) - ST 태그 현재값
- void Tag.SetAnalog(string tagName, float value) - AO 태그 쓰기
- void Tag.SetDigital(string tagName, int value) - DO 태그 쓰기

[알람]
- void Alarm.SetAlarm(string tagName, string level) - 알람 발생
- int Alarm.GetAlarmCount() - 미확인 알람 수
- void Alarm.AckAll() - 전체 알람 확인

[애니메이션]
- void Animation.SetVisible(string className, int visible)
- void Animation.SetColor(string className, int r, int g, int b)
- void Animation.SetText(string className, string text)

[데이터/SQL]
- object Sql.Execute(string query) - SQL 실행
- DataTable Sql.Select(string query) - SQL 조회
- void Data.SaveTrend(string tagName) - 트렌드 저장

[시스템]
- void System.Sleep(int ms) - 대기
- string System.GetDateTime(string format) - 현재시각
- void Screen.Open(string moduleName) - 화면 전환
- void Message.Show(string msg) - 메시지 표시

[HTTP/통신]
- string Http.Get(string url) - HTTP GET
- string Http.Post(string url, string body) - HTTP POST

[파일]
- string File.ReadText(string path) - 파일 읽기
- void File.WriteText(string path, string text) - 파일 쓰기

[리포트]
- void Report.Execute(string reportName) - 리포트 실행
- void Report.Print(string reportName) - 리포트 인쇄
```

---

## 6. 모드별 동작

### 6.1 Generate (신규 생성)

```
사용자: "5분마다 온도 태그 10개의 평균을 계산해서 로그 파일에 저장"
→ LLM이 함수 시그니처에서 적절한 함수 선택
→ 변수 선언 + 루프 + 태그 읽기 + 평균 계산 + 파일 쓰기 코드 생성
```

### 6.2 Modify (기존 코드 수정)

```
사용자: (기존 코드 선택 후) "이 코드에 에러 처리 추가해줘"
→ 기존 코드 컨텍스트 + 수정 요청 전달
→ try-catch 래핑, null 체크 등 추가
```

### 6.3 Explain (코드 설명)

```
사용자: (복잡한 기존 코드 선택 후) "이 코드가 뭐 하는 건지 설명해줘"
→ 코드 분석 → 한글 설명 반환
→ 사용된 태그, 함수, 제어 흐름 설명
```

---

## 7. 보안 고려사항

1. **생성 코드 실행 제한**: 사용자가 반드시 미리보기 후 수동 삽입
2. **위험 패턴 감지**: File.Delete, Process.Start, DROP TABLE 등 경고
3. **태그 쓰기 확인**: SetDigital/SetAnalog 사용 시 대상 태그 확인 메시지
4. **코드 감사 로그**: 생성된 모든 코드와 원본 요청 로깅
5. **LLM 응답 검증**: JSON 파싱 실패 시 원본 텍스트 표시 (실행 불가)

---

## 8. 구현 단계

### Phase 1 (2주): 기본 프레임워크
- FormScriptAiAssistant UI 구현
- ScriptAiService 기본 구조
- 함수 시그니처 자동 추출
- 템플릿 기반 폴백 생성기

### Phase 2 (2주): LLM 연동
- python_ai_engine에 codegen_service 추가
- LLM API 연동 (OpenAI 호환)
- 프롬프트 엔지니어링 및 최적화
- 코드 검증 로직

### Phase 3 (1주): 통합 및 고도화
- FormNewScriptEditor와 통합
- 태그 컨텍스트 자동 수집
- 대화 이력 관리
- 코드 하이라이팅

---

## 9. 의존성

- **기존 코드**: ScriptExternalRun.cs (함수 시그니처), FormNewScriptEditor.cs (에디터 통합)
- **Python 의존성**: openai>=1.0 (LLM 클라이언트, 선택적)
- **신규 파일**:
  - `Studio/Script/FormScriptAiAssistant.cs`
  - `Studio/Script/ScriptAiService.cs`
  - `python_ai_engine/services/codegen_service.py`

# B5. 자연어 데이터 조회 (Conversational HMI) - 상세 구현 설계

## 1. 개요

운전원이 자연어로 태그 데이터 조회, 트렌드 표시, 알람 확인, 제어 명령을 수행.
웹 채팅 인터페이스 + NLP-to-Tag 매핑.

**핵심 가치:** 모바일/현장 사용 편의, 비전문 운전원 접근성 향상

---

## 2. 아키텍처

```
┌──────────────────────────────────────────────────────┐
│ 웹 클라이언트 (PortalServerWeb/AutoWeb)               │
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │ 채팅 UI (ChatWidget)                           │  │
│  │                                                │  │
│  │ 운전원: "지금 1번 보일러 온도 얼마야?"          │  │
│  │ AI: "1번 보일러 출구온도(TT-101)는 185.3°C     │  │
│  │     정상 범위(170~190°C) 내입니다."             │  │
│  │                                                │  │
│  │ 운전원: "어제 3시~5시 압력 트렌드 보여줘"       │  │
│  │ AI: [트렌드 차트 인라인 표시]                   │  │
│  │                                                │  │
│  │ 운전원: "2번 펌프 기동해줘"                     │  │
│  │ AI: "P-002 기동 명령 전송할까요? [확인][취소]"  │  │
│  └────────────────────────────────────────────────┘  │
└──────────┬───────────────────────────────────────────┘
           │ WebSocket / HTTP API
           ▼
┌──────────────────────────────────────────────────────┐
│ ConversationalHMI API Controller (신규)                │
│                                                      │
│  POST /api/chat/query                                │
│  ├─ 의도 분류 (Intent Classification)                │
│  ├─ 태그 매핑 (Entity Extraction)                    │
│  ├─ 실행 (Query/Command)                             │
│  └─ 응답 생성 (Response Formatting)                  │
└──────────┬───────────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────────────────────┐
│ python_ai_engine                                      │
│                                                      │
│  nlp_service (신규)                                   │
│  ├─ nlp/parse         자연어 파싱                     │
│  ├─ nlp/tag_resolve   태그 이름 해석                  │
│  └─ nlp/response      응답 생성                      │
└──────────────────────────────────────────────────────┘
```

---

## 3. 의도 분류 체계

```
Intent (의도)           예시                          실행 액션
─────────────────────────────────────────────────────────
TAG_READ              "온도 얼마야?"               Tag.GetAnalog()
TAG_READ_MULTI        "모든 펌프 상태 보여줘"      Tag.GetDigital() × N
TREND_QUERY           "어제 압력 트렌드"           DB 이력 조회 + 차트
ALARM_QUERY           "미확인 알람 몇 개?"         AlarmProcessor 조회
ALARM_HISTORY         "오늘 알람 이력"             DB 알람 이력
TAG_WRITE             "펌프 기동해줘"              Tag.SetDigital() (승인 필요)
SETPOINT_CHANGE       "온도 셋포인트 25도로"       Tag.SetAnalog() (승인 필요)
SCREEN_NAVIGATE       "냉각수 화면 열어줘"         Screen.Open()
REPORT_RUN            "일간 보고서 실행"           Report.Execute()
SYSTEM_STATUS         "시스템 상태 알려줘"         시스템 정보 조회
UNKNOWN               인식 불가                    "다시 말씀해주세요"
```

---

## 4. C# 구현

### 4.1 ConversationalHMI Controller

**파일:** `PortalServerWeb/AutoWeb/Api/ChatController.cs`

```csharp
[Route("api/chat")]
public class ChatController : ApiController
{
    [HttpPost("query")]
    public async Task<ChatResponse> Query([FromBody] ChatRequest request)
    {
        // 1. Python NLP 서비스로 의도 분류 + 엔티티 추출
        var parseResult = await ParseNaturalLanguage(request.Message);

        // 2. 의도별 실행
        object result = null;
        switch (parseResult.Intent)
        {
            case "TAG_READ":
                result = await HandleTagRead(parseResult);
                break;
            case "TREND_QUERY":
                result = await HandleTrendQuery(parseResult);
                break;
            case "ALARM_QUERY":
                result = await HandleAlarmQuery(parseResult);
                break;
            case "TAG_WRITE":
                result = HandleTagWrite(parseResult, request.SessionId);
                break;
            case "SCREEN_NAVIGATE":
                result = HandleScreenNavigate(parseResult);
                break;
            default:
                result = new { message = "죄송합니다. 이해하지 못했습니다." };
                break;
        }

        // 3. 응답 포맷팅
        return FormatResponse(parseResult, result);
    }

    private async Task<object> HandleTagRead(NlpParseResult parsed)
    {
        var responses = new List<TagReadResponse>();

        foreach (var tagRef in parsed.Tags)
        {
            // 태그 이름 해석 (자연어 → 실제 태그명)
            string actualTag = ResolveTagName(tagRef);
            if (actualTag == null)
            {
                responses.Add(new TagReadResponse
                {
                    Query = tagRef,
                    Error = $"'{tagRef}'에 해당하는 태그를 찾을 수 없습니다"
                });
                continue;
            }

            int[] pos = null;
            var tp = TagLib.GetStructPublic(actualTag, ref pos);
            if (tp == null) continue;

            double value = 0;
            string unit = "";
            string status = "정상";

            if (tp is TagAiClass ai)
            {
                value = ai.curr;
                unit = ai.unit;
                if (ai.alarm > 0)
                {
                    if (value >= ai.hihi) status = "HiHi 알람";
                    else if (value >= ai.high) status = "Hi 경고";
                    else if (value <= ai.lolo) status = "LoLo 알람";
                    else if (value <= ai.low) status = "Lo 경고";
                }
                status += $" (범위: {ai.fBase}~{ai.fFull}{unit})";
            }
            else if (tp is TagDiClass di)
            {
                value = di.curr;
                unit = di.curr == 1 ? di.desc1 ?? "ON" : di.desc0 ?? "OFF";
            }

            responses.Add(new TagReadResponse
            {
                TagName = actualTag,
                Description = tp.description,
                Value = value,
                Unit = unit,
                Status = status
            });
        }

        return responses;
    }

    private async Task<object> HandleTrendQuery(NlpParseResult parsed)
    {
        // 시간 범위 파싱
        DateTime from = parsed.TimeRange?.From ?? DateTime.Now.AddHours(-1);
        DateTime to = parsed.TimeRange?.To ?? DateTime.Now;

        var trendData = new List<TrendDataResponse>();
        foreach (var tagRef in parsed.Tags)
        {
            string actualTag = ResolveTagName(tagRef);
            if (actualTag == null) continue;

            var data = await DataPostgres.GetMinDataAI(actualTag, from, to);
            trendData.Add(new TrendDataResponse
            {
                TagName = actualTag,
                From = from, To = to,
                DataPoints = data.Select(d => new { d.Timestamp, d.Value }).ToList()
            });
        }

        return new { type = "trend_chart", data = trendData };
    }

    private object HandleTagWrite(NlpParseResult parsed, string sessionId)
    {
        // 제어 명령은 승인 필요
        return new
        {
            type = "confirmation_required",
            message = $"{parsed.Tags[0]} 제어 명령을 실행할까요?",
            action = "TAG_WRITE",
            tag = parsed.Tags[0],
            value = parsed.Value,
            confirmationId = Guid.NewGuid().ToString()
        };
    }

    // 태그 이름 해석 (자연어 → 실제 태그)
    private string ResolveTagName(string naturalRef)
    {
        // "1번 보일러 온도" → "Boiler1_Temp"
        // "P-002" → 태그에서 검색
        // "펌프1 전류" → "Pump1_Current"

        // 1. 정확한 태그명이면 그대로 반환
        int[] pos = null;
        if (TagLib.GetStructPublic(naturalRef, ref pos) != null)
            return naturalRef;

        // 2. 설명(Description) 검색
        var allTags = TagLib.GetAllTags();
        var matched = allTags
            .Where(t => t.description != null &&
                       t.description.Contains(naturalRef))
            .OrderBy(t => t.description.Length)
            .FirstOrDefault();

        if (matched != null) return matched.tag;

        // 3. 유사도 매칭
        matched = allTags
            .Select(t => new { Tag = t, Score = CalculateSimilarity(naturalRef, t) })
            .Where(x => x.Score > 0.4)
            .OrderByDescending(x => x.Score)
            .FirstOrDefault()?.Tag;

        return matched?.tag;
    }
}
```

---

## 5. Python NLP 서비스

### 5.1 nlp_service.py

**파일:** `python_ai_engine/services/nlp_service.py`

```python
"""자연어 SCADA 조회 서비스"""

import re
import json
from datetime import datetime, timedelta

async def register(router, **deps):
    router.add_handler("nlp/parse", handle_parse)
    router.add_handler("nlp/tag_resolve", handle_tag_resolve)
    router.add_handler("nlp/response", handle_response)


async def handle_parse(payload: dict) -> dict:
    """자연어 의도 분류 + 엔티티 추출"""
    message = payload.get("message", "")
    tag_list = payload.get("tag_list", [])  # 사용 가능한 태그 목록

    # 의도 분류 (키워드 기반 + LLM 보조)
    intent = _classify_intent(message)

    # 엔티티 추출
    tags = _extract_tags(message, tag_list)
    time_range = _extract_time_range(message)
    value = _extract_value(message)

    return {
        "intent": intent,
        "tags": tags,
        "time_range": time_range,
        "value": value,
        "original": message
    }


def _classify_intent(message: str) -> str:
    """키워드 기반 의도 분류"""
    patterns = {
        "TAG_WRITE": [
            r"(기동|정지|시작|멈춰|켜|꺼|열어|닫아|설정|변경)",
            r"(세?트?포인트|SP|셋포인트).*(변경|바꿔|설정)"
        ],
        "TREND_QUERY": [
            r"(트렌드|추이|변화|그래프|차트)",
            r"(어제|오늘|지난|최근|시간|분).*보여"
        ],
        "ALARM_QUERY": [
            r"(알람|경보|이상)",
            r"미확인.*알람"
        ],
        "SCREEN_NAVIGATE": [
            r"(화면|페이지|스크린).*(열어|가|보여|이동)"
        ],
        "REPORT_RUN": [
            r"(보고서|리포트).*(실행|생성|만들어)"
        ],
        "TAG_READ": [
            r"(얼마|몇도|몇|상태|현재|값|읽어)",
            r"(온도|압력|유량|전류|속도|레벨|위치)"
        ],
    }

    for intent, pats in patterns.items():
        for pat in pats:
            if re.search(pat, message):
                return intent

    return "TAG_READ"  # 기본: 태그 읽기


def _extract_tags(message: str, tag_list: list) -> list:
    """태그 참조 추출"""
    refs = []

    # 패턴: "1번 보일러 온도", "P-002", "TT-101"
    # 장비 번호 패턴
    equip_pattern = r'(\d+)\s*번\s*(보일러|펌프|밸브|탱크|모터|압축기)'
    matches = re.findall(equip_pattern, message)
    for num, equip_type in matches:
        refs.append(f"{num}번 {equip_type}")

    # 태그 ID 패턴 (예: TT-101, P-002)
    tag_id_pattern = r'[A-Z]{1,3}-?\d{2,4}'
    for m in re.findall(tag_id_pattern, message):
        refs.append(m)

    # 파라미터 추출
    param_pattern = r'(온도|압력|유량|전류|레벨|속도|진동|출력)'
    params = re.findall(param_pattern, message)
    if params and refs:
        # 조합: "1번 보일러" + "온도"
        refs = [f"{r} {p}" for r in refs for p in params]
    elif params and not refs:
        refs = params

    return refs if refs else [message]  # 폴백: 전체 메시지


def _extract_time_range(message: str) -> dict:
    """시간 범위 추출"""
    now = datetime.now()

    # "어제 오후 3시부터 5시까지"
    m = re.search(r'어제\s*(오전|오후)?\s*(\d+)시.*?(\d+)시', message)
    if m:
        yesterday = now - timedelta(days=1)
        hour_start = int(m.group(2))
        hour_end = int(m.group(3))
        if m.group(1) == "오후":
            hour_start += 12
            hour_end += 12
        return {
            "from": yesterday.replace(hour=hour_start, minute=0).isoformat(),
            "to": yesterday.replace(hour=hour_end, minute=0).isoformat()
        }

    # "최근 1시간", "지난 30분"
    m = re.search(r'(최근|지난)\s*(\d+)\s*(시간|분|일)', message)
    if m:
        amount = int(m.group(2))
        unit = m.group(3)
        delta = {"시간": timedelta(hours=amount),
                 "분": timedelta(minutes=amount),
                 "일": timedelta(days=amount)}.get(unit, timedelta(hours=1))
        return {
            "from": (now - delta).isoformat(),
            "to": now.isoformat()
        }

    # "오늘"
    if "오늘" in message:
        return {
            "from": now.replace(hour=0, minute=0).isoformat(),
            "to": now.isoformat()
        }

    return None


def _extract_value(message: str) -> float:
    """수치값 추출"""
    m = re.search(r'(\d+\.?\d*)\s*(도|℃|%|bar|MPa|A|V|Hz|rpm)', message)
    if m:
        return float(m.group(1))
    return None
```

---

## 6. 웹 채팅 UI

### 6.1 ChatWidget (HTML/JS)

```javascript
// AutoWeb/static/js/chat-widget.js
class ChatWidget {
    constructor(container) {
        this.container = container;
        this.ws = new WebSocket(`ws://${location.host}/api/chat/ws`);
        this.render();
    }

    async sendMessage(text) {
        const response = await fetch('/api/chat/query', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                message: text,
                sessionId: this.sessionId
            })
        });
        const data = await response.json();
        this.displayResponse(data);
    }

    displayResponse(data) {
        if (data.type === 'trend_chart') {
            this.renderTrendChart(data.data);
        } else if (data.type === 'confirmation_required') {
            this.renderConfirmation(data);
        } else {
            this.renderTextResponse(data.message);
        }
    }
}
```

---

## 7. 구현 단계

### Phase 1 (2주): NLP 파싱 + 태그 읽기
- 의도 분류 (키워드 기반)
- 태그 이름 해석
- TAG_READ 구현

### Phase 2 (2주): 트렌드 + 알람
- TREND_QUERY (DB 조회 + 차트)
- ALARM_QUERY
- 웹 채팅 UI

### Phase 3 (2주): 제어 + 고도화
- TAG_WRITE (승인 플로우)
- LLM 기반 고도화 (선택)
- WebSocket 실시간 통신

---

## 8. 의존성

- **C# 측**: PortalServerWeb, TagLib, DataPostgres, PythonAiManager
- **Python 측**: python_ai_engine (nlp_service)
- **웹**: HTML/JS 채팅 위젯
- **신규 파일**:
  - `PortalServerWeb/AutoWeb/Api/ChatController.cs`
  - `python_ai_engine/services/nlp_service.py`
  - `AutoWeb/static/js/chat-widget.js`

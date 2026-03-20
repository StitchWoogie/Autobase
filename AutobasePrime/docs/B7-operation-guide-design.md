# B7. 운전 가이드 & 의사결정 지원 - 상세 구현 설계

## 1. 개요

비정상 상황(알람) 발생 시 AI가 유사 과거 사례 검색 + SOP 문서 매칭으로
단계별 대응 가이드를 자동 제시. RAG(Retrieval-Augmented Generation) 기반.

**핵심 가치:** 신입 운전원 대응 시간 60% 단축, 경험 지식 체계화

---

## 2. 아키텍처

```
┌──────────────────────────────────────────────────────────┐
│ LocalMain - 알람 발생 시 트리거                            │
│                                                          │
│  AlarmProcessor → 알람 발생                               │
│       │                                                  │
│       ▼                                                  │
│  OperationGuideBridge (신규)                              │
│  ├─ 알람 컨텍스트 수집                                    │
│  │  ├─ 관련 태그 현재값                                  │
│  │  ├─ 최근 태그 변화 이력                                │
│  │  ├─ 동시 발생 알람 목록                                │
│  │  └─ 장비 정보                                         │
│  ├─ Python guide_service 호출                            │
│  └─ 가이드 결과 → 알람 UI에 표시                          │
│                                                          │
└──────────────┬───────────────────────────────────────────┘
               │ TCP
               ▼
┌──────────────────────────────────────────────────────────┐
│ python_ai_engine                                          │
│                                                          │
│  guide_service (신규)                                     │
│  ├─ guide/alarm_response   알람 대응 가이드 생성          │
│  ├─ guide/sop_search       SOP 문서 검색                 │
│  ├─ guide/history_match    유사 이력 검색                 │
│  └─ guide/index            SOP 문서 인덱싱               │
│                                                          │
│  RAG 파이프라인:                                          │
│  ├─ 벡터 DB (ChromaDB/FAISS)                             │
│  │  └─ SOP 문서 임베딩                                   │
│  ├─ 유사도 검색 → Top-K 문서 조각                         │
│  └─ LLM → 컨텍스트 기반 가이드 생성                       │
└──────────────────────────────────────────────────────────┘
```

---

## 3. C# 구현

### 3.1 OperationGuideBridge

**파일:** `LocalMain/Guide/OperationGuideBridge.cs`

```csharp
public class OperationGuideBridge
{
    private static OperationGuideBridge _instance;
    public static OperationGuideBridge Instance => _instance;

    // 알람 발생 시 호출 (AlarmProcessor 또는 IntelligentAlarmEngine에서)
    public async Task<OperationGuide> GetGuideForAlarm(
        ALARM_FILE_STRUCT alarm,
        List<ALARM_FILE_STRUCT> concurrentAlarms = null)
    {
        // 1. 알람 컨텍스트 수집
        var context = BuildAlarmContext(alarm, concurrentAlarms);

        // 2. Python guide_service 호출
        var payload = new JObject
        {
            ["alarm_tag"] = alarm.tagName,
            ["alarm_message"] = alarm.message,
            ["alarm_priority"] = alarm.priority,
            ["context"] = JObject.FromObject(context),
            ["concurrent_alarms"] = new JArray(
                (concurrentAlarms ?? new List<ALARM_FILE_STRUCT>())
                .Select(a => a.tagName))
        };

        string result = await PythonAiManager.CallAsync(
            "guide/alarm_response", payload.ToString());

        var response = JObject.Parse(result);
        if (response["Ok"]?.Value<bool>() != true)
            return null;

        return ParseGuideResponse(response["Result"]);
    }

    private AlarmContext BuildAlarmContext(
        ALARM_FILE_STRUCT alarm,
        List<ALARM_FILE_STRUCT> concurrent)
    {
        var context = new AlarmContext();

        // 관련 태그 현재값
        int[] pos = null;
        var tp = TagLib.GetStructPublic(alarm.tagName, ref pos);
        if (tp is TagAiClass ai)
        {
            context.CurrentValue = ai.curr;
            context.Unit = ai.unit;
            context.NormalRange = $"{ai.fBase}~{ai.fFull}";
            context.AlarmLimits = new Dictionary<string, double>
            {
                ["hihi"] = ai.hihi, ["high"] = ai.high,
                ["low"] = ai.low, ["lolo"] = ai.lolo
            };
        }

        // 태그 그룹의 다른 태그들 (같은 장비)
        string tagGroup = GetTagGroup(alarm.tagName);
        context.RelatedTags = GetGroupTagValues(tagGroup);

        // 최근 변화 트렌드 (10분)
        context.RecentTrend = GetRecentTrend(alarm.tagName, 10);

        // 동시 알람
        context.ConcurrentAlarms = concurrent?.Select(a => new
        {
            Tag = a.tagName,
            Message = a.message,
            Priority = a.priority
        }).ToList();

        return context;
    }

    private OperationGuide ParseGuideResponse(JToken result)
    {
        return new OperationGuide
        {
            Summary = result["summary"]?.ToString(),
            Steps = result["steps"]?.ToObject<List<GuideStep>>()
                    ?? new List<GuideStep>(),
            RelatedScreens = result["related_screens"]?.ToObject<List<string>>()
                             ?? new List<string>(),
            SimilarCases = result["similar_cases"]?.ToObject<List<SimilarCase>>()
                           ?? new List<SimilarCase>(),
            SopReference = result["sop_reference"]?.ToString(),
            Confidence = result["confidence"]?.Value<double>() ?? 0,
            EmergencyContact = result["emergency_contact"]?.ToString()
        };
    }
}

public class OperationGuide
{
    public string Summary { get; set; }
    public List<GuideStep> Steps { get; set; }
    public List<string> RelatedScreens { get; set; }
    public List<SimilarCase> SimilarCases { get; set; }
    public string SopReference { get; set; }
    public double Confidence { get; set; }
    public string EmergencyContact { get; set; }
}

public class GuideStep
{
    public int StepNumber { get; set; }
    public string Action { get; set; }        // "냉각수 밸브 확인"
    public string Detail { get; set; }        // 상세 설명
    public string ScreenName { get; set; }    // 관련 SCADA 화면
    public string TagToCheck { get; set; }    // 확인할 태그
    public bool IsEmergency { get; set; }     // 긴급 여부
}

public class SimilarCase
{
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public string Resolution { get; set; }
    public double Similarity { get; set; }
}
```

### 3.2 알람 UI 연동

```csharp
// ObjectWindowAlarm.cs 또는 FormPopupAlarmConfirmation.cs에서
// 알람 선택 시 가이드 패널 표시

private async void ShowOperationGuide(ALARM_FILE_STRUCT alarm)
{
    var guide = await OperationGuideBridge.Instance.GetGuideForAlarm(alarm);
    if (guide == null) return;

    // 가이드 패널 표시
    panelGuide.Visible = true;
    lblGuideSummary.Text = guide.Summary;

    // 단계별 가이드 표시
    lstGuideSteps.Items.Clear();
    foreach (var step in guide.Steps)
    {
        string icon = step.IsEmergency ? "🔴" : "📋";
        lstGuideSteps.Items.Add(
            $"{icon} {step.StepNumber}. {step.Action}");
    }

    // 관련 화면 버튼
    foreach (var screen in guide.RelatedScreens)
    {
        var btn = new Button { Text = $"화면: {screen}" };
        btn.Click += (s, e) => Screen.Open(screen);
        panelGuideButtons.Controls.Add(btn);
    }

    // 유사 사례 표시
    if (guide.SimilarCases.Any())
    {
        lblSimilarCase.Text =
            $"유사 사례: {guide.SimilarCases[0].Date:g} - " +
            $"{guide.SimilarCases[0].Resolution}";
    }
}
```

---

## 4. Python 구현

### 4.1 guide_service.py

**파일:** `python_ai_engine/services/guide_service.py`

```python
"""운전 가이드 서비스 (RAG 기반)"""

import json
import os
from typing import Any, List

# 벡터 DB
_vector_store = None
_sop_index = None
_alarm_history = []

async def register(router, **deps):
    global _vector_store
    router.add_handler("guide/alarm_response", handle_alarm_response)
    router.add_handler("guide/sop_search", handle_sop_search)
    router.add_handler("guide/history_match", handle_history_match)
    router.add_handler("guide/index", handle_index)

    # 벡터 DB 초기화
    _vector_store = _init_vector_store()


async def handle_alarm_response(payload: dict) -> dict:
    """알람 대응 가이드 생성"""
    alarm_tag = payload.get("alarm_tag", "")
    alarm_message = payload.get("alarm_message", "")
    context = payload.get("context", {})
    concurrent = payload.get("concurrent_alarms", [])

    # 1. SOP 문서 검색 (RAG)
    sop_results = _search_sop(alarm_tag, alarm_message)

    # 2. 유사 알람 이력 검색
    similar_cases = _search_alarm_history(alarm_tag, context)

    # 3. 규칙 기반 가이드 생성 (SOP + 이력 + 컨텍스트)
    guide = _generate_guide(
        alarm_tag, alarm_message, context,
        concurrent, sop_results, similar_cases)

    return guide


def _search_sop(alarm_tag: str, alarm_message: str) -> list:
    """SOP 문서 벡터 검색"""
    if _vector_store is None:
        return _fallback_sop_search(alarm_tag)

    query = f"{alarm_tag} {alarm_message}"

    try:
        results = _vector_store.similarity_search(query, k=3)
        return [{"content": r.page_content, "source": r.metadata.get("source", "")}
                for r in results]
    except Exception:
        return _fallback_sop_search(alarm_tag)


def _fallback_sop_search(alarm_tag: str) -> list:
    """벡터 DB 없을 때 키워드 검색"""
    sop_dir = os.path.join("data", "sop")
    if not os.path.exists(sop_dir):
        return []

    results = []
    tag_prefix = alarm_tag.split("_")[0] if "_" in alarm_tag else alarm_tag

    for f in os.listdir(sop_dir):
        if f.endswith(('.txt', '.md', '.json')):
            path = os.path.join(sop_dir, f)
            with open(path, 'r', encoding='utf-8') as fh:
                content = fh.read()
                if tag_prefix.lower() in content.lower() or \
                   alarm_tag.lower() in content.lower():
                    results.append({"content": content[:500], "source": f})

    return results[:3]


def _search_alarm_history(alarm_tag: str, context: dict) -> list:
    """유사 알람 이력 검색"""
    similar = []
    for case in _alarm_history:
        if case.get("alarm_tag") == alarm_tag:
            similar.append({
                "date": case.get("date"),
                "description": case.get("description"),
                "resolution": case.get("resolution"),
                "similarity": 0.9
            })
        elif _tag_group(case.get("alarm_tag", "")) == _tag_group(alarm_tag):
            similar.append({
                "date": case.get("date"),
                "description": case.get("description"),
                "resolution": case.get("resolution"),
                "similarity": 0.6
            })

    return sorted(similar, key=lambda x: x["similarity"], reverse=True)[:5]


def _generate_guide(
    alarm_tag, alarm_message, context,
    concurrent, sop_results, similar_cases
) -> dict:
    """대응 가이드 생성"""

    # 알람 타입별 기본 대응 템플릿
    templates = {
        "HiHi": {
            "summary": "고고 알람 발생 - 즉시 확인 필요",
            "steps": [
                {"step": 1, "action": "현장 확인", "detail": "해당 기기의 실제 상태 확인",
                 "is_emergency": True},
                {"step": 2, "action": "관련 밸브/스위치 확인",
                 "detail": "입출구 밸브 개도, 관련 스위치 상태 점검"},
                {"step": 3, "action": "부하 감소", "detail": "필요 시 부하 감소 조치"},
                {"step": 4, "action": "정비팀 연락", "detail": "상태 지속 시 정비팀 호출"}
            ]
        },
        "LoLo": {
            "summary": "저저 알람 발생 - 공급 확인 필요",
            "steps": [
                {"step": 1, "action": "공급원 확인",
                 "detail": "원료/유체 공급 상태 확인"},
                {"step": 2, "action": "배관 누설 점검",
                 "detail": "배관, 이음부 누설 여부 확인"},
                {"step": 3, "action": "센서 정상 여부",
                 "detail": "계측기 동작 상태 확인"}
            ]
        }
    }

    # 알람 레벨 추출
    alarm_level = "HiHi" if "HiHi" in alarm_message else \
                  "LoLo" if "LoLo" in alarm_message else \
                  "Hi" if "Hi" in alarm_message else \
                  "Lo" if "Lo" in alarm_message else "General"

    template = templates.get(alarm_level, templates.get("HiHi"))

    # SOP 결과로 보강
    sop_ref = sop_results[0]["source"] if sop_results else None

    # 컨텍스트 기반 커스터마이즈
    steps = []
    for s in template["steps"]:
        step = {
            "step_number": s["step"],
            "action": s["action"],
            "detail": s["detail"],
            "screen_name": _guess_related_screen(alarm_tag),
            "tag_to_check": alarm_tag,
            "is_emergency": s.get("is_emergency", False)
        }
        steps.append(step)

    # 동시 알람이 있으면 추가 단계
    if concurrent and len(concurrent) > 1:
        steps.insert(0, {
            "step_number": 0,
            "action": "연쇄 알람 확인",
            "detail": f"동시 발생 알람 {len(concurrent)}건 확인. " +
                     f"관련: {', '.join(concurrent[:3])}",
            "is_emergency": True
        })

    # 관련 화면 추출
    screens = [_guess_related_screen(alarm_tag)]
    for tag in concurrent[:2]:
        s = _guess_related_screen(tag)
        if s and s not in screens:
            screens.append(s)

    return {
        "summary": f"{alarm_tag}: {template['summary']}",
        "steps": steps,
        "related_screens": [s for s in screens if s],
        "similar_cases": similar_cases[:3],
        "sop_reference": sop_ref,
        "confidence": 0.6 if sop_results else 0.3,
        "emergency_contact": "정비팀 내선 1234"
    }


def _guess_related_screen(alarm_tag: str) -> str:
    """태그 이름에서 관련 화면 추측"""
    # "Boiler1_Temp_HiHi" → "Boiler1.modx"
    parts = alarm_tag.split("_")
    if parts:
        return f"{parts[0]}.modx"
    return None


def _tag_group(tag_name: str) -> str:
    parts = tag_name.split("_")
    return parts[0] if parts else tag_name


def _init_vector_store():
    """벡터 DB 초기화 (ChromaDB)"""
    try:
        import chromadb
        client = chromadb.PersistentClient(path="data/vectordb")
        collection = client.get_or_create_collection("sop_documents")
        return collection
    except ImportError:
        return None


async def handle_sop_search(payload: dict) -> dict:
    query = payload.get("query", "")
    results = _search_sop("", query)
    return {"results": results}


async def handle_history_match(payload: dict) -> dict:
    alarm_tag = payload.get("alarm_tag", "")
    results = _search_alarm_history(alarm_tag, {})
    return {"similar_cases": results}


async def handle_index(payload: dict) -> dict:
    """SOP 문서 인덱싱"""
    doc_path = payload.get("document_path")
    if not doc_path or not os.path.exists(doc_path):
        return {"status": "error", "message": "문서 경로 없음"}

    if _vector_store is None:
        return {"status": "error", "message": "벡터 DB 미설정"}

    # 문서 읽기 + 청킹 + 임베딩 + 저장
    with open(doc_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # 간단한 청킹 (500자 단위)
    chunks = [content[i:i+500] for i in range(0, len(content), 400)]

    _vector_store.add(
        documents=chunks,
        metadatas=[{"source": doc_path, "chunk": i} for i in range(len(chunks))],
        ids=[f"{doc_path}_{i}" for i in range(len(chunks))]
    )

    return {"status": "indexed", "chunks": len(chunks), "source": doc_path}
```

---

## 5. 구현 단계

### Phase 1 (2주): 규칙 기반 가이드
- OperationGuideBridge 기본 구조
- 알람 레벨별 템플릿 가이드
- 알람 UI 가이드 패널

### Phase 2 (2주): SOP 검색 (RAG)
- SOP 문서 인덱싱 (키워드 기반)
- 유사 이력 검색
- 가이드 보강

### Phase 3 (2주): 벡터 DB + LLM
- ChromaDB 벡터 검색
- LLM 기반 가이드 생성 (선택)
- 관련 화면 자동 연결

---

## 6. 의존성

- **C# 측**: AlarmProcessor, TagLib, PythonAiManager
- **Python 측**: chromadb (선택), openai (선택)
- **데이터**: SOP 문서 (txt/md/json), 알람 이력
- **신규 파일**:
  - `LocalMain/Guide/OperationGuideBridge.cs`
  - `python_ai_engine/services/guide_service.py`
  - `python_ai_engine/data/sop/` (SOP 문서 디렉토리)

# A3. SCADA 화면 자동 생성 (Template AI) - 상세 구현 설계

## 1. 개요

프로세스 설명(텍스트) 또는 P&ID 이미지를 입력하면 SCADA 화면을 자동 생성.
라이브러리 컴포넌트 자동 선택 + 태그 바인딩 + 레이아웃 배치.

**핵심 가치:** 신규 프로젝트 화면 구축 시간 70% 단축

---

## 2. 아키텍처

```
┌────────────────────────────────────────────────────────────┐
│ FormScreenGenerator (신규 대화상자)                          │
│                                                            │
│  입력 모드 1: 텍스트 기반                                   │
│  ┌──────────────────────────────────────────────────┐      │
│  │ "3개 탱크, 2개 펌프, 배관 연결, 온도/압력 모니터링" │      │
│  └──────────────────────────────────────────────────┘      │
│                                                            │
│  입력 모드 2: P&ID 이미지                                   │
│  ┌──────────────────────────────────────────────────┐      │
│  │ [이미지 업로드] → vision_service OCR/detect       │      │
│  └──────────────────────────────────────────────────┘      │
│                                                            │
│  [생성] → ScreenGenerationPipeline                          │
│                                                            │
│  미리보기 ──→ [적용] → ObjectGroup으로 변환                 │
└────────────────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ ScreenGenerationPipeline                                    │
│                                                            │
│ Stage 1: 텍스트/이미지 파싱                                 │
│   └─ LLM 또는 vision_service → 구조화 데이터               │
│   {equipment: [{type:"Tank", count:3, name:"TK-101~103"}]} │
│                                                            │
│ Stage 2: 컴포넌트 매핑                                      │
│   └─ equipment_type → Library 컴포넌트 선택                 │
│   Tank → Library/Tank_Vertical.lib                         │
│   Pump → Library/Pump_Centrifugal.lib                      │
│                                                            │
│ Stage 3: 레이아웃 배치                                      │
│   └─ LayoutSuggestionEngine + P&ID 토폴로지                │
│   좌→우 공정 흐름 배치                                      │
│                                                            │
│ Stage 4: 태그 바인딩                                        │
│   └─ TagBindingSuggester 활용                              │
│   Tank_01_Level → TK-101 레벨 게이지                       │
│                                                            │
│ Stage 5: 배관/라인 연결                                     │
│   └─ 장비 간 연결선 자동 생성                               │
└────────────────────────────────────────────────────────────┘
```

---

## 3. 핵심 구현

### 3.1 ScreenGenerationPipeline

**파일:** `Studio/ScreenGen/ScreenGenerationPipeline.cs`

```csharp
public class ScreenGenerationPipeline
{
    public async Task<ObjectGroup> GenerateFromText(string description)
    {
        // Stage 1: LLM 파싱 → 구조화 데이터
        var spec = await ParseDescription(description);

        // Stage 2: 컴포넌트 매핑
        var components = MapToComponents(spec);

        // Stage 3: 레이아웃 배치
        var layout = CalculateLayout(components, spec);

        // Stage 4: ObjectGroup 생성
        var group = BuildObjectGroup(layout);

        // Stage 5: 태그 바인딩 제안
        await SuggestTagBindings(group);

        return group;
    }

    public async Task<ObjectGroup> GenerateFromImage(byte[] imageData)
    {
        // vision_service로 P&ID 심볼 인식
        var payload = new JObject
        {
            ["image"] = Convert.ToBase64String(imageData)
        };

        string result = await PythonAiManager.CallAsync(
            "vision/detect", payload.ToString());

        var detections = JObject.Parse(result)["Result"]["detections"];

        // 인식된 심볼 → 장비 목록 변환
        var spec = ConvertDetectionsToSpec(detections);

        return await GenerateFromSpec(spec);
    }

    private async Task<ScreenSpec> ParseDescription(string text)
    {
        // LLM으로 자연어 → 구조화 데이터
        var payload = new JObject
        {
            ["user_prompt"] = text,
            ["mode"] = "screen_spec"
        };

        string result = await PythonAiManager.CallAsync(
            "codegen/screen_parse", payload.ToString());

        return JsonConvert.DeserializeObject<ScreenSpec>(
            JObject.Parse(result)["Result"].ToString());
    }

    private List<PlacedComponent> CalculateLayout(
        List<ComponentMapping> components, ScreenSpec spec)
    {
        int screenWidth = 1920;
        int screenHeight = 1080;
        int margin = 50;

        // 공정 흐름: 좌 → 우 배치
        int x = margin;
        int y = screenHeight / 3; // 중앙 정렬
        int spacing = 100;

        var placed = new List<PlacedComponent>();
        foreach (var comp in components)
        {
            placed.Add(new PlacedComponent
            {
                Component = comp,
                X = x, Y = y,
                Width = comp.DefaultWidth,
                Height = comp.DefaultHeight
            });

            x += comp.DefaultWidth + spacing;
            if (x > screenWidth - margin)
            {
                x = margin;
                y += comp.DefaultHeight + spacing;
            }
        }

        // 배관 연결선 추가
        for (int i = 0; i < placed.Count - 1; i++)
        {
            placed[i].ConnectionTo = placed[i + 1];
        }

        return placed;
    }
}

public class ScreenSpec
{
    public List<EquipmentSpec> Equipment { get; set; }
    public List<ConnectionSpec> Connections { get; set; }
    public List<MonitoringPoint> Monitoring { get; set; }
    public string ProcessFlow { get; set; } // "left-to-right", "top-to-bottom"
}

public class EquipmentSpec
{
    public string Type { get; set; }    // "Tank", "Pump", "Valve", "HeatExchanger"
    public int Count { get; set; }
    public string NamePattern { get; set; }  // "TK-{001}"
    public List<string> Parameters { get; set; } // ["Level", "Temperature", "Pressure"]
}

public class ComponentMapping
{
    public EquipmentSpec Spec { get; set; }
    public string LibraryPath { get; set; }  // "Library/Tank_Vertical.lib"
    public int DefaultWidth { get; set; }
    public int DefaultHeight { get; set; }
    public List<string> TagSlots { get; set; } // 바인딩할 태그 슬롯
}
```

### 3.2 장비 → 라이브러리 매핑 테이블

```csharp
private static readonly Dictionary<string, ComponentTemplate> EquipmentTemplates = new()
{
    ["Tank"] = new ComponentTemplate
    {
        LibraryName = "Tank_Vertical",
        DefaultSize = new Size(80, 120),
        TagSlots = new[] { "Level", "Temperature", "Pressure" },
        SubComponents = new[] { "LevelGauge", "TempIndicator" }
    },
    ["Pump"] = new ComponentTemplate
    {
        LibraryName = "Pump_Centrifugal",
        DefaultSize = new Size(60, 60),
        TagSlots = new[] { "Run", "Speed", "Current", "Vibration" },
        SubComponents = new[] { "StatusIndicator", "SpeedDisplay" }
    },
    ["Valve"] = new ComponentTemplate
    {
        LibraryName = "Valve_Control",
        DefaultSize = new Size(40, 40),
        TagSlots = new[] { "Open", "Position" },
        SubComponents = new[] { "PositionBar" }
    },
    ["HeatExchanger"] = new ComponentTemplate
    {
        LibraryName = "HeatExchanger",
        DefaultSize = new Size(100, 80),
        TagSlots = new[] { "InletTemp", "OutletTemp", "Flow" },
        SubComponents = new[] { "TempDiff" }
    },
    ["Motor"] = new ComponentTemplate
    {
        LibraryName = "Motor_3Phase",
        DefaultSize = new Size(50, 50),
        TagSlots = new[] { "Run", "Speed", "Current" },
        SubComponents = new[] { "StatusLamp" }
    },
    ["Compressor"] = new ComponentTemplate
    {
        LibraryName = "Compressor",
        DefaultSize = new Size(80, 80),
        TagSlots = new[] { "Run", "SuctionPress", "DischargePress", "Vibration" },
        SubComponents = new[] { "PressGauge" }
    }
};
```

---

## 4. Python 측: 이미지 인식 보강

### 4.1 P&ID 심볼 인식 학습

```python
# python_ai_engine/services/vision_service.py에 추가

async def handle_pid_detect(payload: dict) -> dict:
    """P&ID 도면 심볼 인식"""
    image_data = payload.get("image")  # base64

    # YOLO v8 + P&ID 커스텀 모델
    # 클래스: Tank, Pump, Valve, HeatExchanger, Motor, Pipe, Instrument
    detections = await _detect_pid_symbols(image_data)

    # 심볼 간 연결 관계 추론 (배관 라인 추적)
    connections = _trace_pipe_connections(detections)

    return {
        "equipment": detections,
        "connections": connections,
        "confidence": _avg_confidence(detections)
    }
```

---

## 5. 구현 단계

### Phase 1 (2주): 텍스트 기반 생성
- LLM 파싱 → ScreenSpec
- 장비 → 라이브러리 매핑
- 좌→우 레이아웃 배치

### Phase 2 (2주): 태그 + 배관
- TagBindingSuggester 연동
- 배관 연결선 자동 생성
- FormScreenGenerator UI

### Phase 3 (4주): P&ID 이미지 인식 (장기)
- vision_service P&ID 커스텀 모델 학습
- 심볼 인식 → 장비 매핑
- 배관 추적 알고리즘

---

## 6. 의존성

- **기존 코드**: Library/, ClassEditInsert, ObjectGroup, TagBindingSuggester(A2-2)
- **Python**: codegen_service(A4), vision_service
- **신규 파일**:
  - `Studio/ScreenGen/ScreenGenerationPipeline.cs`
  - `Studio/ScreenGen/FormScreenGenerator.cs`
  - `Studio/ScreenGen/ComponentMapping.cs`

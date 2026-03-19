# SCADA Studio 편집기 & LocalMain Runtime AI 기능 탐색

## 현재 AI 인프라 현황

Autobase에는 이미 Python AI Engine(`python_ai_engine/`)이 존재하며 다음 서비스를 제공합니다:
- `predict_service` - 예측 모델
- `analysis_service` - 데이터 분석
- `vision_service` - 이미지 처리
- `train_service` - 모델 학습
- `script_service` - Python 스크립트 실행

연동 경로: `PortalServerWeb → LocalMain → PythonAiScriptBridge → python_ai_engine (port 5678)`

---

## Part A: Studio SCADA 편집기 AI 기능

### A1. AI 기반 자동 레이아웃 & 배치

**기능:** SCADA 객체(밸브, 펌프, 탱크, 배관 등)를 드래그&드롭 시 AI가 자동 정렬/배치 제안

**적용 포인트:**
- `Studio/ClassStudioEdit.cs` - 편집 로직에 배치 제안 통합
- `Studio/ClassEditObjectMove.cs` - 객체 이동 시 스냅/정렬 제안

**구현 아이디어:**
```
사용자가 펌프 객체 배치 → AI가 기존 배관/탱크와의 최적 위치 제안
기존 P&ID 패턴 학습 → 유사 배치 자동 추천
```

**실용성:** ★★★★☆ (편집 효율 대폭 향상)
**구현 난이도:** ★★★☆☆

---

### A2. AI 기반 태그 대량 자동 생성 & 바인딩

#### A2-1. 태그 대량 자동 생성 (핵심 기능)

**현재 워크플로우의 문제점:**

PLC 주소 M1000~M2000에 대해 AI 태그 1000개를 생성하려면:
```
현재 절차 (5단계, 수작업 집약적):
① FormTagEditor에서 CSV 내보내기 (exportToCSVFileToolStripMenuItem)
② Excel에서 CSV 열기
③ 1000행 수작업 편집 (태그명, port, station, address, fn 등 컬럼별 입력)
④ CSV 저장
⑤ FormTagEditor에서 CSV 불러오기 (importFromCSVFileToolStripMenuItem)
```

CSV 컬럼 구조 (`TagFile.cs` 기준):
```
TagType, Tag, Description, Active, LinkType, LocalTag,
OpcServer, OpcGroup, OpcItem, ...,
port, station, address, extra1, extra2, ...,
fn, unit, fBase, fFull, fPlcBase, fPlcFull, hihi, high, low, lolo, ...
```

**AI 자동 생성 기능 제안:**

FormTagEditor에 "AI 태그 생성" 버튼/대화상자를 추가하여,
자연어 또는 간단한 양식으로 대량 태그를 자동 생성.

**입력 방식 1: 간단 양식 (Form-based)**

```
┌─────────────────────────────────────────────────┐
│  AI 태그 자동 생성                               │
├─────────────────────────────────────────────────┤
│                                                  │
│  태그 타입:    [AI ▼]                            │
│  이름 패턴:    [Boiler1_Temp_####]               │
│  시작 번호:    [0]      끝 번호: [999]            │
│  설명 패턴:    [보일러1 온도센서 ####]             │
│                                                  │
│  ── PLC SCAN 설정 ──                             │
│  Port:         [1]                               │
│  Station:      [1]                               │
│  시작 주소:    [1000]                             │
│  주소 증분:    [1]                                │
│  Extra Addr:   [D]  (MELSEC 등)                  │
│  Function:     [0] (16bit whole)                 │
│                                                  │
│  ── 엔지니어링 단위 ──                           │
│  Unit:         [℃]                               │
│  Base(min):    [0]      Full(max): [100]          │
│  PLC Base:     [0]      PLC Full:  [4095]         │
│  Display:      [10.1]                             │
│                                                  │
│  ── 알람 설정 (선택) ──                          │
│  □ 알람 사용   HiHi:[90] Hi:[80] Lo:[10] LoLo:[5]│
│                                                  │
│  [미리보기]  [생성]  [취소]                        │
└─────────────────────────────────────────────────┘
```

**입력 방식 2: 자연어 (LLM 기반)**

```
사용자 입력:
"Port 1, Station 1, MELSEC D레지스터 1000번부터 1999번까지
 AI 태그 1000개 생성해줘.
 이름은 Boiler1_Temp_0000~0999,
 단위는 ℃, 범위 0~200, PLC 0~4095,
 80도 이상 Hi알람, 150도 이상 HiHi알람"

AI 파싱 결과:
→ 타입: AI
→ 이름: Boiler1_Temp_{0000..0999}
→ port: 1, station: 1
→ address: 1000~1999 (증분 1)
→ sExtraAddr: "D"
→ unit: "℃", fBase: 0, fFull: 200
→ fPlcBase: 0, fPlcFull: 4095
→ high: 80, hihi: 150, alarm: 1
```

**구현 위치 및 방법:**

```csharp
// FormTagEditor.cs에 메뉴 항목 추가
// 기존 importFromCSVFileToolStripMenuItem 옆에 배치
private void aiGenerateTagsToolStripMenuItem_Click(object sender, EventArgs e)
{
    using (var dialog = new FormAiTagGenerator())
    {
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            TagGrClass generatedTags = dialog.GeneratedTags;

            // 기존 TagFile/TagGrClass 구조 그대로 활용
            // importFromCSV와 동일한 병합 로직 사용
            MergeGeneratedTags(generatedTags);
        }
    }
}
```

```csharp
// FormAiTagGenerator.cs (신규)
public class FormAiTagGenerator : Form
{
    public TagGrClass GeneratedTags { get; private set; }

    // 방식 1: 양식 기반 생성
    private TagGrClass GenerateFromForm()
    {
        var gr = new TagGrClass();
        string namePattern = txtNamePattern.Text;   // "Boiler1_Temp_####"
        int startNum = (int)nudStartNum.Value;      // 0
        int endNum = (int)nudEndNum.Value;           // 999
        int startAddr = (int)nudStartAddr.Value;     // 1000
        int addrStep = (int)nudAddrStep.Value;       // 1

        for (int i = startNum; i <= endNum; i++)
        {
            var tag = new TagAiClass();
            tag.name = namePattern.Replace("####", i.ToString("D4"));
            tag.port = (short)nudPort.Value;
            tag.station = (short)nudStation.Value;
            tag.address = startAddr + (i - startNum) * addrStep;
            tag.fn = (short)cmbFunction.SelectedIndex;
            tag.cTagLinkType = 0; // PLC_SCAN
            tag.unit = txtUnit.Text;
            tag.fBase = (double)nudBase.Value;
            tag.fFull = (double)nudFull.Value;
            tag.fPlcBase = (double)nudPlcBase.Value;
            tag.fPlcFull = (double)nudPlcFull.Value;

            if (chkAlarm.Checked)
            {
                tag.alarm = 1;
                tag.hihi = (double)nudHiHi.Value;
                tag.high = (double)nudHigh.Value;
                tag.low = (double)nudLow.Value;
                tag.lolo = (double)nudLoLo.Value;
            }

            gr.aiList.Add(tag);
        }
        return gr;
    }

    // 방식 2: 자연어 → LLM 파싱 → 양식 자동 채움
    private async Task ParseNaturalLanguage(string userInput)
    {
        // PythonAiScriptBridge 또는 직접 LLM API 호출
        // 파싱 결과로 양식 필드 자동 채움
        var parsed = await AiService.ParseTagGenerationRequest(userInput);
        nudPort.Value = parsed.Port;
        nudStation.Value = parsed.Station;
        nudStartAddr.Value = parsed.StartAddress;
        // ... 나머지 필드 자동 채움
        // 사용자가 확인 후 [생성] 클릭
    }
}
```

**미리보기 기능:**
```
┌──────────────────────────────────────────────────────────────┐
│  미리보기 (처음 5개 / 총 1000개)                              │
├──────┬──────────────────┬───┬───┬──────┬────┬───────────────┤
│ Type │ Tag              │Prt│Stn│ Addr │ Fn │ Unit/Range    │
├──────┼──────────────────┼───┼───┼──────┼────┼───────────────┤
│ AI   │ Boiler1_Temp_0000│ 1 │ 1 │ D1000│ 0  │ ℃ 0~200      │
│ AI   │ Boiler1_Temp_0001│ 1 │ 1 │ D1001│ 0  │ ℃ 0~200      │
│ AI   │ Boiler1_Temp_0002│ 1 │ 1 │ D1002│ 0  │ ℃ 0~200      │
│ AI   │ Boiler1_Temp_0003│ 1 │ 1 │ D1003│ 0  │ ℃ 0~200      │
│ AI   │ Boiler1_Temp_0004│ 1 │ 1 │ D1004│ 0  │ ℃ 0~200      │
│ ...  │ ...              │...│...│ ...  │... │ ...           │
│ AI   │ Boiler1_Temp_0999│ 1 │ 1 │ D1999│ 0  │ ℃ 0~200      │
├──────┴──────────────────┴───┴───┴──────┴────┴───────────────┤
│ 총 1000개 태그 생성 예정        [CSV로 내보내기] [바로 적용]   │
└──────────────────────────────────────────────────────────────┘
```

**DI 태그의 비트 주소 자동 계산 예:**
```
"Port 1, Station 1, M1000~M1999까지 DI 태그 16000개 생성"
→ DI_M1000_B00 (address_word=1000, address_bit=0)
→ DI_M1000_B01 (address_word=1000, address_bit=1)
→ ...
→ DI_M1000_B15 (address_word=1000, address_bit=15)
→ DI_M1001_B00 (address_word=1001, address_bit=0)
→ ...
→ DI_M1999_B15 (address_word=1999, address_bit=15)
```

**고급 기능: PLC 메모리맵 기반 자동 생성**
```
입력: PLC 메모리맵 문서 (Excel/PDF)
┌────────┬──────────────┬──────┬───────┐
│ 주소   │ 설명          │ 타입 │ 범위  │
├────────┼──────────────┼──────┼───────┤
│ D1000  │ 보일러 온도   │ INT  │ 0~200 │
│ D1001  │ 보일러 압력   │ INT  │ 0~50  │
│ M2000  │ 펌프1 운전    │ BIT  │ 0/1   │
│ M2001  │ 밸브1 열림    │ BIT  │ 0/1   │
└────────┴──────────────┴──────┴───────┘

AI 파싱 → 혼합 태그 자동 생성:
- AI: Boiler_Temp (D1000, ℃, 0~200)
- AI: Boiler_Press (D1001, bar, 0~50)
- DI: Pump1_Run (M2000, ON/OFF)
- DI: Valve1_Open (M2001, 열림/닫힘)
```

**실용성:** ★★★★★ (현장 최고 빈도 작업의 자동화)
**구현 난이도:** ★★☆☆☆ (양식 기반) / ★★★☆☆ (자연어 + LLM)

---

#### A2-2. 태그 자동 바인딩 제안

**기능:** SCADA 객체를 배치하면 AI가 적절한 태그를 자동 추천

**적용 포인트:**
- `Studio/Property/PropertyPageObjectTag.cs` - 태그 속성 편집 시
- `Studio/ClassEditProperty.cs` - 속성 편집 전반
- `Dll/DialogTag/TagEditor/FormTagEditor.cs` - 태그 선택 대화상자

**구현 아이디어:**
```
ObjectAnalogGauge 배치 → AI가 "Temperature" 관련 AI 태그 목록 추천
ObjectDigitalAnimation 배치 → AI가 관련 DO 태그 추천
객체 이름/설명 기반 → NLP로 적합한 태그 매칭
```

**실용성:** ★★★★★ (태그 수천 개인 대형 프로젝트에서 핵심)
**구현 난이도:** ★★☆☆☆ (태그 이름 기반 유사도 매칭으로 시작 가능)

---

### A3. SCADA 화면 자동 생성 (Template AI)

**기능:** 프로세스 설명(텍스트) 또는 P&ID 이미지를 입력하면 SCADA 화면을 자동 생성

**적용 포인트:**
- `Studio/StudioMain.cs` - 신규 메뉴 "AI 화면 생성"
- `Studio/ClassEditInsert.cs` - 객체 자동 삽입
- `Studio/Library/` - 라이브러리 컴포넌트 자동 선택

**구현 아이디어:**
```
입력: "3개 탱크, 2개 펌프, 배관 연결, 온도/압력 모니터링"
출력: SCADA 화면 초안 자동 생성 (탱크+펌프+배관+게이지 배치)

입력: P&ID 도면 이미지 (vision_service 활용)
출력: 인식된 심볼 → SCADA 객체 자동 매핑 및 배치
```

**실용성:** ★★★★★ (신규 프로젝트 구축 시간 대폭 단축)
**구현 난이도:** ★★★★★ (높음, LLM + vision 필요)

---

### A4. 스크립트 자동 생성 & 어시스턴트

**기능:** 자연어로 동작 설명 시 SCADA 스크립트 코드 자동 생성

**적용 포인트:**
- `ViewMain/GraphicModule/Script/ScriptClass.cs` - 319KB 스크립트 엔진
- 100+ ScriptFunction 클래스 (Tag, Animation, Data, Alarm, SQL 등)

**구현 아이디어:**
```
입력: "온도가 80도 넘으면 펌프1 정지하고 알람 발생"
출력:
  if (Tag.GetAnalog("Temperature_01") > 80.0)
  {
      Tag.SetDigital("Pump_01_Run", 0);
      Alarm.SetAlarm("Temperature_01", "HiHi");
  }
```

**활용 가능한 스크립트 함수 카테고리:**
- `ScriptFunctionTag.cs` - 태그 읽기/쓰기
- `ScriptFunctionAlarm.cs` - 알람 제어
- `ScriptFunctionAnimation.cs` - 애니메이션
- `ScriptFunctionData.cs` - 데이터 연산
- `ScriptFunctionSql.cs` - SQL 쿼리
- `ScriptFunctionHttp.cs` - HTTP 통신
- `ScriptFunctionFile.cs` - 파일 I/O
- `ScriptFunctionReport.cs` - 리포트

**실용성:** ★★★★★ (비전문가도 스크립트 작성 가능)
**구현 난이도:** ★★★☆☆ (LLM API + 함수 시그니처 프롬프트로 구현 가능)

---

### A5. 디자인 일관성 검사

**기능:** SCADA 화면의 디자인 품질을 AI가 분석하고 개선 제안

**적용 포인트:**
- `Studio/StudioMain.cs` - "AI 디자인 검사" 메뉴
- 각 Object 클래스의 속성 분석

**검사 항목:**
```
- 폰트 크기/스타일 일관성
- 색상 팔레트 통일성
- 객체 정렬/간격 균일성
- 태그 바인딩 누락 검출
- 접근성 (색약 대응, 대비율)
- 업계 표준 (ISA-101 HMI 가이드라인) 준수 여부
```

**실용성:** ★★★★☆ (HMI 표준 준수 자동화)
**구현 난이도:** ★★☆☆☆ (규칙 기반 + AI 보조)

---

## Part B: LocalMain SCADA Runtime AI 기능

### B1. 예측 정비 (Predictive Maintenance)

**기능:** 태그 데이터 패턴 분석으로 장비 고장/이상 사전 예측

**적용 포인트:**
- `LocalMain/CheckEngineTagChange.cs` - 태그 변화 감지 엔진
- `LocalMain/DataSave/TrendSaveManager.cs` - 이력 데이터 활용
- `python_ai_engine/predict_service` - 예측 모델 서비스

**구현 아이디어:**
```
이력 데이터 학습 → 이상 패턴 모델 생성
실시간 태그 데이터 → 모델 추론 → 이상 점수 계산
이상 점수 임계값 초과 → 예측 알람 발생

적용 예:
- 진동 센서 데이터 → 베어링 교체 시기 예측
- 온도/압력 트렌드 → 이상 가열 사전 감지
- 전류값 패턴 → 모터 고장 예측
```

**실용성:** ★★★★★ (산업현장 최우선 니즈)
**구현 난이도:** ★★★☆☆ (python_ai_engine 인프라 활용)

---

### B2. 지능형 알람 관리

**기능:** 알람 홍수 억제, 근본 원인 분석, 알람 우선순위 자동 조정

**적용 포인트:**
- `LocalMain/Alarm/AlarmProcessor.cs` - 알람 처리 핵심
- `LocalMain/Alarm/AlarmDisplay.cs` - 알람 표시
- `LocalMain/Alarm/AlarmConfirm.cs` - 알람 확인

**구현 아이디어:**
```
(1) 알람 홍수 억제 (Alarm Flood Suppression)
    연관 알람 그룹핑 → 근본 원인 1개만 표시
    예: 탱크 레벨 Low → 펌프 과열 → 배관 압력 Low
        → "탱크 레벨 Low" 1건만 우선 표시

(2) 근본 원인 분석 (Root Cause Analysis)
    동시 발생 알람 패턴 → 인과관계 추론
    이력 데이터 기반 유사 사례 검색

(3) 알람 우선순위 동적 조정
    운전 상태(가동/정지/전환) → 알람 임계값 자동 조정
    시간대/계절별 정상 범위 학습
```

**실용성:** ★★★★★ (운전원 피로도 감소, 대응 속도 향상)
**구현 난이도:** ★★★☆☆

---

### B3. 자동 셋포인트 최적화

**기능:** 공정 조건에 따라 최적의 셋포인트/운전 조건 AI 추천

**적용 포인트:**
- 태그 시스템의 Setpoint 속성 (`cattag.h` - `m_fAoSetpoint`)
- `LocalMain/CheckEngineTagChange.cs` - 태그 변경 시 최적값 제안
- `python_ai_engine/analysis_service` - 분석 서비스

**구현 아이디어:**
```
이력 데이터 분석 → 최적 운전 조건 학습
현재 공정 상태 입력 → 최적 셋포인트 추천
운전원 승인 → 자동 적용 (Semi-Auto)

예: 외기온도 35°C, 생산량 80% → 냉각수 온도 셋포인트 22°C 추천
```

**실용성:** ★★★★☆ (에너지 절감, 품질 최적화)
**구현 난이도:** ★★★★☆

---

### B4. 이상 탐지 (Anomaly Detection)

**기능:** 정상 운전 패턴을 학습하고 이상 상태를 실시간 탐지

**적용 포인트:**
- `LocalMain/CheckEngineTagChange.cs` - 태그 변화 모니터링
- `LocalMain/DataSave/TrendSaveManager.cs` - 학습 데이터
- `ViewMain/GraphicModule/Object/ObjectMultiTrend.cs` - 트렌드 시각화

**구현 아이디어:**
```
(1) 통계 기반: 이동 평균, 표준편차, CUSUM
(2) ML 기반: Isolation Forest, Autoencoder
(3) 다변량 분석: 태그 간 상관관계 이탈 감지

시각화: 트렌드 차트에 "정상 범위 밴드" 오버레이
        이상 구간 하이라이트 표시
```

**실용성:** ★★★★★
**구현 난이도:** ★★★☆☆ (python_ai_engine 활용)

---

### B5. 자연어 데이터 조회 (Conversational HMI)

**기능:** 운전원이 자연어로 데이터 조회/제어 명령

**적용 포인트:**
- `PortalServerWeb/AutoWeb/` - 웹 채팅 인터페이스 추가
- `LocalMain/PythonAi/PythonAiScriptBridge.cs` - NLP 처리
- `ViewMain/GraphicModule/Script/ScriptFunctionTag.cs` - 태그 조회/제어

**구현 아이디어:**
```
운전원: "지금 1번 보일러 온도 얼마야?"
AI: "1번 보일러 출구 온도(TT-101)는 현재 185.3°C입니다.
     정상 범위(170~190°C) 내에 있습니다."

운전원: "어제 오후 3시부터 5시까지 압력 트렌드 보여줘"
AI: [트렌드 차트 자동 생성 및 표시]

운전원: "2번 펌프 기동해줘"
AI: "2번 펌프(P-002) 기동 명령을 전송할까요? [확인/취소]"
```

**실용성:** ★★★★☆ (모바일/현장 사용 시 편리)
**구현 난이도:** ★★★★☆ (LLM 통합 + 태그 매핑)

---

### B6. 에너지 최적화 분석

**기능:** 에너지 사용 패턴 분석 및 절감 방안 자동 제안

**적용 포인트:**
- `LocalMain/Demand/` - 수요 관리 모듈 (이미 존재)
- `python_ai_engine/analysis_service`

**구현 아이디어:**
```
전력/가스/스팀 사용량 패턴 분석
피크 시간대 부하 분산 제안
장비별 에너지 효율 비교
비가동 시간 에너지 낭비 탐지
```

**실용성:** ★★★★☆ (직접적 비용 절감)
**구현 난이도:** ★★★☆☆

---

### B7. 운전 가이드 & 의사결정 지원

**기능:** 비정상 상황 발생 시 AI가 운전 가이드(SOP) 자동 제시

**적용 포인트:**
- `LocalMain/Alarm/AlarmProcessor.cs` - 알람 발생 시 트리거
- `ViewMain/GraphicModule/Object/ObjectWindowAlarm.cs` - 알람 창에 가이드 표시

**구현 아이디어:**
```
알람 발생 → AI가 유사 과거 사례 검색
          → 조치 이력 분석
          → 단계별 대응 가이드 제시
          → 관련 SCADA 화면 자동 열기

예: "TT-101 HiHi 알람"
→ "1. 냉각수 밸브 확인 (화면: Cooling-01 열기)"
   "2. 보일러 부하 감소 (현재 85% → 70% 권장)"
   "3. 정비팀 호출 필요 시 내선 1234"
```

**실용성:** ★★★★★ (신입 운전원 교육/지원)
**구현 난이도:** ★★★☆☆ (RAG 기반 문서 검색)

---

## 우선순위 종합 매트릭스

| 순위 | 기능 | 적용 영역 | 실용성 | 난이도 | 비고 |
|------|------|----------|--------|--------|------|
| **1** | **태그 대량 자동 생성 (A2-1)** | **Studio** | **★★★★★** | **★★☆☆☆** | **현장 최고빈도 작업, 양식 기반으로 빠르게 구현** |
| 2 | 스크립트 자동 생성 (A4) | Studio | ★★★★★ | ★★★☆☆ | LLM API로 빠르게 구현 가능 |
| 3 | 태그 자동 바인딩 (A2-2) | Studio | ★★★★★ | ★★☆☆☆ | 텍스트 유사도로 시작 |
| 3 | 이상 탐지 (B4) | Runtime | ★★★★★ | ★★★☆☆ | python_ai_engine 활용 |
| 4 | 예측 정비 (B1) | Runtime | ★★★★★ | ★★★☆☆ | 산업 현장 최우선 니즈 |
| 5 | 지능형 알람 (B2) | Runtime | ★★★★★ | ★★★☆☆ | 운전원 피로도 감소 |
| 6 | 운전 가이드 (B7) | Runtime | ★★★★★ | ★★★☆☆ | RAG 기반 |
| 7 | 디자인 검사 (A5) | Studio | ★★★★☆ | ★★☆☆☆ | 규칙 기반으로 시작 |
| 8 | 에너지 최적화 (B6) | Runtime | ★★★★☆ | ★★★☆☆ | Demand 모듈 확장 |
| 9 | 자연어 조회 (B5) | Both | ★★★★☆ | ★★★★☆ | LLM + 태그 매핑 |
| 10 | 셋포인트 최적화 (B3) | Runtime | ★★★★☆ | ★★★★☆ | 공정 지식 필요 |
| 11 | 자동 레이아웃 (A1) | Studio | ★★★★☆ | ★★★☆☆ | UX 향상 |
| 12 | 화면 자동 생성 (A3) | Studio | ★★★★★ | ★★★★★ | 장기 과제 |

---

## 권장 구현 순서

### Phase 1: Quick Win (1~2개월)
- **A2-1 태그 대량 자동 생성**: FormAiTagGenerator 양식 기반 (LLM 없이도 즉시 구현 가능)
- **A4 스크립트 자동 생성**: LLM API 연동 + ScriptFunction 시그니처 프롬프트
- **A2-2 태그 자동 바인딩**: 태그명 유사도 매칭 (Levenshtein / TF-IDF)
- **A5 디자인 검사**: 규칙 기반 일관성 체크

### Phase 2: Core AI (2~4개월)
- **B4 이상 탐지**: python_ai_engine에 Isolation Forest / Autoencoder 모델 추가
- **B1 예측 정비**: 이력 데이터 기반 예측 모델 학습
- **B2 지능형 알람**: AlarmProcessor에 알람 그룹핑/RCA 로직 추가

### Phase 3: Advanced (4~6개월)
- **B7 운전 가이드**: RAG 파이프라인 구축 (SOP 문서 벡터DB)
- **B5 자연어 조회**: 웹 채팅 UI + NLP-to-Tag 매핑
- **B6 에너지 최적화**: Demand 모듈 AI 확장

### Phase 4: Next-Gen (6개월+)
- **A3 화면 자동 생성**: Vision + LLM 기반 P&ID→SCADA 변환
- **B3 셋포인트 최적화**: 강화학습 기반 최적 제어

# SCADA Studio UX 개선 설계안

> **작성일**: 2026-03-15
> **대상**: Autobase Studio (SCADA 편집기)
> **범위**: 작화(모듈 편집), 스크립트 편집, AI 기능 접목, 전반적 사용자 편의성

---

## 목차

1. [현행 분석 요약](#1-현행-분석-요약)
2. [개선 영역 총괄](#2-개선-영역-총괄)
3. [Phase 1 — 스크립트 에디터 스마트화](#3-phase-1--스크립트-에디터-스마트화)
4. [Phase 2 — 작화(모듈 편집) AI 지원](#4-phase-2--작화모듈-편집-ai-지원)
5. [Phase 3 — 속성 편집 및 일괄 작업 UX](#5-phase-3--속성-편집-및-일괄-작업-ux)
6. [Phase 4 — Python AI Engine 확장 서비스](#6-phase-4--python-ai-engine-확장-서비스)
7. [구현 우선순위 및 로드맵](#7-구현-우선순위-및-로드맵)

---

## 1. 현행 분석 요약

### 1.1 Studio 아키텍처

```
┌─────────────────────────────────────────────────────────────┐
│                     STUDIO (WinForms)                        │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ 그래픽 에디터  │  │ 스크립트 에디터│  │  속성 패널    │      │
│  │ (Canvas)     │  │ (Monaco)     │  │ (Property)   │      │
│  │              │  │              │  │              │      │
│  │ 77개 오브젝트 │  │ C# DSL 편집  │  │ 120+ 속성페이지│      │
│  │ 64개 삽입메서드│  │ WebView2기반  │  │ 오브젝트별 설정│      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│           │                 │                │               │
│           └─────────────────┼────────────────┘               │
│                             │ TCP                            │
│                    ┌────────▼────────┐                       │
│                    │ Python AI Engine │                       │
│                    │ (127.0.0.1:5678)│                       │
│                    │ script/predict/ │                       │
│                    │ vision/analysis │                       │
│                    └─────────────────┘                       │
└─────────────────────────────────────────────────────────────┘
```

### 1.2 현행 강점
- Monaco Editor + WebView2 기반 스크립트 편집 (pre-warming pool로 빠른 로딩)
- `$Tag`, `@Method` 자동완성 기본 지원
- Python AI Engine 인프라 구축 완료 (script/predict/vision/analysis 서비스)
- 77종 그래픽 오브젝트, 64개 삽입 메서드로 충분한 작화 요소
- ZIP 기반 컴포넌트 라이브러리 시스템

### 1.3 현행 주요 한계점

| 영역 | 한계점 | 영향도 |
|------|--------|--------|
| 스크립트 편집 | 수동 트리거 자동완성만 지원 (`.`, `$`, `@`) | 높음 |
| 스크립트 편집 | 실시간 구문 오류 검출 없음 (컴파일 시에만) | 높음 |
| 스크립트 편집 | 파라미터 힌트, 호버 문서화 미지원 | 높음 |
| 스크립트 편집 | 코드 아웃라인/구조 탐색 패널 없음 | 중간 |
| 작화 편집 | AI 기반 자동 배치/레이아웃 없음 | 높음 |
| 작화 편집 | 태그 자동 바인딩 및 스마트 삽입 없음 | 높음 |
| 작화 편집 | 데이터 기반 반복 배치 기능 없음 | 중간 |
| 속성 편집 | 다중 선택 시 일괄 속성 편집 미지원 | 중간 |
| 속성 편집 | 스타일 복사/붙여넣기 (포맷 브러시) 없음 | 중간 |
| 전체 | 전역 검색(스크립트/태그/오브젝트) 없음 | 높음 |

---

## 2. 개선 영역 총괄

```
                         UX 개선 4대 영역
    ┌──────────────────────────────────────────────────┐
    │                                                    │
    │   ① 스크립트 에디터      ② 작화(모듈 편집)        │
    │      스마트화                AI 지원               │
    │   ┌─────────────┐      ┌─────────────┐           │
    │   │ AI 자동완성  │      │ 스마트 삽입  │           │
    │   │ 실시간 검증  │      │ AI 레이아웃  │           │
    │   │ 코드 탐색    │      │ 태그 자동연결│           │
    │   │ 실행 미리보기│      │ 템플릿 시스템│           │
    │   └─────────────┘      └─────────────┘           │
    │                                                    │
    │   ③ 속성/일괄 작업       ④ AI Engine 확장         │
    │      UX 개선                                      │
    │   ┌─────────────┐      ┌─────────────┐           │
    │   │ 일괄 편집    │      │ 코드 생성    │           │
    │   │ 포맷 브러시  │      │ 레이아웃 추천│           │
    │   │ 전역 검색    │      │ 태그 분석    │           │
    │   │ 즐겨찾기     │      │ 이상 감지    │           │
    │   └─────────────┘      └─────────────┘           │
    └──────────────────────────────────────────────────┘
```

---

## 3. Phase 1 — 스크립트 에디터 스마트화

### 3.1 AI 기반 코드 자동완성 강화

**현행**: `autobase-completions.js`에서 `.`, `$`, `@` 트리거 시에만 정적 완성 목록 제공

**개선안**: 문맥 인식 자동완성 시스템

#### 3.1.1 컨텍스트 인식 자동완성

```
[변경 파일] ScriptLib/ScriptLibEdit/Editor/MonacoFiles/autobase-completions.js

현행 트리거: '.', '$', '@'
추가 트리거: '=', '(', '<', ',', ' ' (공백 후 키워드)

동작 방식:
┌─────────────────────────────────────────────┐
│ 사용자 입력: $Temperature.value =           │
│                                    ↑ 커서    │
│                                              │
│ ┌─ AI 완성 제안 ─────────────────────────┐  │
│ │ ★ $Temperature.hihi     (상한값: 100)  │  │
│ │   $Temperature.lolo     (하한값: 0)    │  │
│ │   $Pressure.value       (연관 태그)    │  │
│ │   Math.Max(a, b)        (수학 함수)    │  │
│ │   ────────────────────────────────────  │  │
│ │   💡 AI 제안: 이전 패턴 기반 추천      │  │
│ └────────────────────────────────────────┘  │
└─────────────────────────────────────────────┘
```

**구현 상세**:

```javascript
// autobase-completions.js 확장

// 1. 확장된 트리거 문자
triggerCharacters: ['.', '$', '@', '=', '(', ','],

// 2. 컨텍스트 분석 함수 추가
function analyzeContext(model, position) {
    const lineContent = model.getLineContent(position.lineNumber);
    const textBeforeCursor = lineContent.substring(0, position.column - 1);

    return {
        isAssignment: /=\s*$/.test(textBeforeCursor),
        isFunctionArg: /\(\s*$/.test(textBeforeCursor) || /,\s*$/.test(textBeforeCursor),
        isComparison: /(==|!=|>=|<=|>|<)\s*$/.test(textBeforeCursor),
        currentTag: extractCurrentTag(textBeforeCursor),
        currentScope: detectScope(model, position),
    };
}

// 3. 태그 연관성 기반 추천
function getRelatedTagSuggestions(currentTag, allTags) {
    // 같은 그룹의 태그 우선 추천
    // 이전 사용 패턴 기반 추천
    // 태그 타입 호환성 체크
}
```

#### 3.1.2 파라미터 힌트 (Signature Help)

```
[변경 파일] ScriptLib/ScriptLibEdit/Editor/MonacoFiles/autobase-completions.js
[신규 파일] ScriptLib/ScriptLibEdit/Editor/MonacoFiles/autobase-signature-help.js

동작 방식:
┌─────────────────────────────────────────────┐
│ 사용자 입력: @SetTagValue(                  │
│                           ↑ 커서             │
│                                              │
│ ┌─ 파라미터 힌트 ─────────────────────────┐ │
│ │ @SetTagValue(tagName, value, timestamp) │ │
│ │              ▲                           │ │
│ │ tagName: string — 대상 태그 이름        │ │
│ │ value: double — 설정할 값               │ │
│ │ timestamp: DateTime — (선택) 타임스탬프  │ │
│ └─────────────────────────────────────────┘ │
└─────────────────────────────────────────────┘
```

**구현**: Monaco `SignatureHelpProvider` 등록

```javascript
// autobase-signature-help.js
monaco.languages.registerSignatureHelpProvider('autobase-script', {
    signatureHelpTriggerCharacters: ['(', ','],
    provideSignatureHelp(model, position) {
        const methodName = extractMethodName(model, position);
        const signatures = methodSignatureDB[methodName];
        if (!signatures) return null;

        return {
            signatures: signatures.map(sig => ({
                label: sig.label,
                documentation: { value: sig.description },
                parameters: sig.params.map(p => ({
                    label: p.name,
                    documentation: { value: `**${p.type}** — ${p.description}` }
                }))
            })),
            activeSignature: 0,
            activeParameter: countCommasBeforeCursor(model, position)
        };
    }
});
```

#### 3.1.3 호버 문서화 (Hover Provider)

```
[변경 파일] ScriptLib/ScriptLibEdit/Editor/MonacoFiles/autobase-completions.js
[신규 파일] ScriptLib/ScriptLibEdit/Editor/MonacoFiles/autobase-hover.js

동작 방식:
┌─────────────────────────────────────────────┐
│ 코드: if ($Temperature.value > $Temp.hihi)  │
│              ▲ 마우스 호버                   │
│                                              │
│ ┌─ 호버 정보 ─────────────────────────────┐ │
│ │ 📋 $Temperature                         │ │
│ │ ─────────────────────────────────────── │ │
│ │ 타입: Analog Input (AI)                 │ │
│ │ 설명: 반응기 온도 센서                   │ │
│ │ 단위: ℃                                 │ │
│ │ 범위: 0.0 ~ 500.0                       │ │
│ │ 현재값: 127.5 (마지막 갱신: 10:32:15)   │ │
│ │ ─────────────────────────────────────── │ │
│ │ 멤버: .value .tag .hihi .lolo .des      │ │
│ └─────────────────────────────────────────┘ │
└─────────────────────────────────────────────┘
```

### 3.2 실시간 구문 검증 및 진단

**현행**: 저장/컴파일 시에만 오류 확인 가능

**개선안**: 타이핑 중 실시간 오류 표시

#### 3.2.1 인라인 오류 표시 (Diagnostics)

```
[변경 파일] ScriptLib/ScriptLibEdit/Editor/MonacoEditorBridge.cs
[신규 파일] ScriptLib/ScriptLibEdit/Editor/MonacoFiles/autobase-diagnostics.js

동작 방식:
┌─────────────────────────────────────────────┐
│  1 │ int count = 0;                         │
│  2 │ $Temperature.value = count             │
│    │ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ❌  │
│    │  ⚠ 세미콜론(;) 누락                    │
│  3 │                                         │
│  4 │ if (count >> 10) {                      │
│    │          ~~ ⚠                           │
│    │  ⚠ 비교 연산자 '>'를 의도하셨나요?       │
│    │     ('>>': 비트 시프트 연산자)            │
│  5 │     $UnknownTag.value = 1;              │
│    │     ~~~~~~~~~~~ ❌                       │
│    │  ❌ 존재하지 않는 태그: $UnknownTag      │
│  6 │ }                                       │
└─────────────────────────────────────────────┘
```

**구현 상세**:

```javascript
// autobase-diagnostics.js

// 디바운스된 검증 (타이핑 후 500ms 대기)
let diagnosticTimer = null;

editor.onDidChangeModelContent(() => {
    clearTimeout(diagnosticTimer);
    diagnosticTimer = setTimeout(() => validateScript(), 500);
});

function validateScript() {
    const code = editor.getValue();
    const markers = [];

    // 1단계: 로컬 구문 검사 (즉시)
    markers.push(...checkSemicolons(code));
    markers.push(...checkBracketBalance(code));
    markers.push(...checkTagReferences(code, knownTags));
    markers.push(...checkCommonMistakes(code));

    // 2단계: Python AI Engine 검증 (비동기, 선택적)
    if (aiValidationEnabled) {
        requestAiValidation(code).then(aiMarkers => {
            markers.push(...aiMarkers);
            monaco.editor.setModelMarkers(editor.getModel(), 'autobase', markers);
        });
    } else {
        monaco.editor.setModelMarkers(editor.getModel(), 'autobase', markers);
    }
}

// 일반적 실수 패턴 검출
function checkCommonMistakes(code) {
    const patterns = [
        { regex: />>(?!=)/g, message: "'>'(비교)를 의도하셨나요? ('>>'는 비트 시프트)", severity: Warning },
        { regex: /=(?!=)/g,  check: isInsideCondition, message: "'=='(비교)를 의도하셨나요? ('='는 대입)", severity: Warning },
        { regex: /\$\w+(?!\.\w)/g, message: "태그 멤버(.value 등)를 지정하세요", severity: Info },
    ];
    // ...
}
```

#### 3.2.2 Quick Fix (빠른 수정) 액션

```
사용자가 오류 squiggle 위에 커서를 놓으면:
┌─────────────────────────────────────────────┐
│  💡 빠른 수정                                │
│  ├── ✏️ 세미콜론 추가                        │
│  ├── 🔧 '>>'를 '>'로 변경                   │
│  └── 📋 '$UnknownTag'를 가장 유사한          │
│         '$Temperature'로 교체                 │
└─────────────────────────────────────────────┘
```

### 3.3 코드 탐색 및 구조화

#### 3.3.1 코드 아웃라인 패널

```
[변경 파일] Studio/Script/FormNewScriptEditor.cs
[신규 파일] Studio/Script/UserControlCodeOutline.cs

UI 레이아웃:
┌─────────────────────────────────────────────────────┐
│ ┌──────────┐ ┌─────────────────────────────────────┐│
│ │ 아웃라인  │ │          코드 에디터                ││
│ │          │ │                                     ││
│ │ ▼ 변수   │ │  1 │ int count = 0;                ││
│ │   count  │ │  2 │ double sum = 0.0;             ││
│ │   sum    │ │  3 │                                ││
│ │          │ │  4 │ // 온도 체크                    ││
│ │ ▼ 태그참조│ │  5 │ if ($Temp.value > 100) {      ││
│ │   $Temp  │ │  6 │   @SetAlarm("HIGH_TEMP");     ││
│ │   $Press │ │  7 │ }                              ││
│ │          │ │  8 │                                ││
│ │ ▼ 메서드  │ │  9 │ for (int i = 0; ...) {       ││
│ │   @Set.. │ │ 10 │   sum += $Press.value;        ││
│ │   @Log.. │ │ 11 │ }                              ││
│ └──────────┘ └─────────────────────────────────────┘│
└─────────────────────────────────────────────────────┘

기능:
- 항목 클릭 → 해당 라인으로 점프
- 실시간 갱신 (편집 시 자동 반영)
- 태그 참조 섹션: 사용된 모든 태그 목록
- 접기/펼치기 지원
```

#### 3.3.2 전역 심볼 검색 (Go to Symbol)

```
단축키: Ctrl+Shift+O (현재 파일) / Ctrl+T (전체 프로젝트)

┌───────────────────────────────────────────────┐
│ 🔍 심볼 검색: temp                            │
│ ─────────────────────────────────────────────  │
│ 📋 $Temperature       Analog Input    AI_0001 │
│ 📋 $TempSetpoint      Analog Output   AO_0005 │
│ 📝 @SetTempAlarm()    Script Method           │
│ 📄 온도 모니터링.mod   Graphic Module          │
│ 📄 온도 제어.script    Script File             │
└───────────────────────────────────────────────┘
```

### 3.4 스크립트 실행 미리보기 패널

```
[신규 파일] Studio/Script/UserControlScriptPreview.cs

UI 레이아웃 (에디터 하단 패널):
┌─────────────────────────────────────────────────────┐
│                   코드 에디터                         │
│  1 │ double temp = $Temperature.value;               │
│  2 │ if (temp > $Temperature.hihi) {                 │
│  3 │     @SetAlarm("HIGH_TEMP");                     │
│  4 │     print("경고: 온도 초과 " + temp);            │
│  5 │ }                                               │
├─────────────────────────────────────────────────────┤
│ 실행 미리보기          [▶ 실행] [⚙ 태그 설정] [📋]  │
│ ─────────────────────────────────────────────────── │
│ ┌─ 태그 시뮬레이션 ──┐ ┌─ 출력 결과 ──────────────┐│
│ │ $Temperature       │ │ [10:32:15] 실행 시작     ││
│ │  .value: [  150.0] │ │ [10:32:15] 경고: 온도    ││
│ │  .hihi:  [  100.0] │ │  초과 150                ││
│ │  .lolo:  [    0.0] │ │ [10:32:15] @SetAlarm     ││
│ │                    │ │  호출: "HIGH_TEMP"       ││
│ │ [+ 태그 추가]      │ │ ─────────────────────── ││
│ │                    │ │ ✅ 정상 완료 (12ms)      ││
│ └────────────────────┘ └──────────────────────────┘│
└─────────────────────────────────────────────────────┘

기능:
- 태그 값 수동 입력으로 시뮬레이션
- Python AI Engine의 script/execute 서비스 활용
- print() 출력 실시간 표시
- @Method 호출 추적 (실제 실행 없이 호출 로그)
- 실행 시간 및 메모리 사용량 표시
- 이전 실행 이력 저장
```

### 3.5 AI 코드 생성 (자연어 → 코드)

```
[변경 파일] ScriptLib/ScriptLibEdit/Editor/MonacoEditorBridge.cs
[신규 파일] ScriptLib/ScriptLibEdit/Editor/MonacoFiles/autobase-ai-assist.js
[신규 파일] python_ai_engine/services/codegen_service.py

단축키: Ctrl+I (AI 어시스트)

동작 방식:
┌─────────────────────────────────────────────────────┐
│  1 │ // 여기에 코드 작성                             │
│  2 │ ▊                                              │
│    │                                                 │
│    │  ┌─ AI 어시스트 ─────────────────────────────┐ │
│    │  │ 💬 무엇을 구현하시겠습니까?                │ │
│    │  │                                           │ │
│    │  │ [온도가 상한값 초과 시 알람 발생하고      ] │ │
│    │  │ [해당 값을 DB에 기록하는 코드 작성해줘    ] │ │
│    │  │                                           │ │
│    │  │ 사용 가능 태그: $Temperature, $Pressure   │ │
│    │  │                                           │ │
│    │  │              [생성] [취소]                 │ │
│    │  └───────────────────────────────────────────┘ │
│    │                                                 │
│    │  ▼ AI 생성 결과 (Diff 미리보기)                 │
│    │  ┌───────────────────────────────────────────┐ │
│    │  │ + double temp = $Temperature.value;       │ │
│    │  │ + double limit = $Temperature.hihi;       │ │
│    │  │ +                                         │ │
│    │  │ + if (temp > limit) {                     │ │
│    │  │ +     @SetAlarm("TEMP_HIGH");             │ │
│    │  │ +     @LogToDatabase("Temperature",       │ │
│    │  │ +         temp, DateTime.Now);            │ │
│    │  │ + }                                       │ │
│    │  │                                           │ │
│    │  │          [적용] [수정 요청] [취소]         │ │
│    │  └───────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘

구현 흐름:
1. 사용자가 자연어로 요구사항 입력
2. 현재 스크립트 컨텍스트 + 사용 가능 태그 목록 수집
3. Python AI Engine의 codegen/generate 서비스 호출
4. 생성된 코드를 Diff 형태로 미리보기
5. 사용자 확인 후 에디터에 삽입
```

---

## 4. Phase 2 — 작화(모듈 편집) AI 지원

### 4.1 스마트 오브젝트 삽입

**현행**: 툴바에서 오브젝트 선택 → 캔버스에 수동 배치 (64개 삽입 메서드)

**개선안**: 태그 기반 자동 오브젝트 추천 + 드래그&드롭 강화

#### 4.1.1 태그 드래그 → 자동 오브젝트 생성

```
[변경 파일] Studio/FormEditGraphicFrame.cs
[변경 파일] Studio/ClassEditInsert.cs
[신규 파일] Studio/SmartInsert/SmartObjectResolver.cs

동작 방식:
┌──────────────────────────────────────────────────────────┐
│ 태그 브라우저                    그래픽 에디터            │
│ ┌──────────────┐                ┌──────────────────────┐│
│ │ ▼ Reactor    │                │                      ││
│ │   $Temp (AI) │───드래그──→   │    ┌──────────┐      ││
│ │   $Press(AI) │                │    │ 온도 표시 │      ││
│ │   $Valve(DO) │                │    │  127.5℃  │      ││
│ │   $Pump (DO) │                │    └──────────┘      ││
│ │              │                │                      ││
│ └──────────────┘                └──────────────────────┘│
│                                                          │
│  태그 타입별 자동 매핑:                                   │
│  ┌─────────────────────────────────────────────────────┐ │
│  │ AI (Analog Input)  → 숫자 표시기 / 트렌드 차트      │ │
│  │ AO (Analog Output) → 설정값 입력 + 숫자 표시기      │ │
│  │ DI (Digital Input)  → 상태 표시 램프 (ON/OFF)       │ │
│  │ DO (Digital Output) → 토글 버튼 + 상태 표시         │ │
│  │ ST (String)         → 텍스트 표시기                  │ │
│  └─────────────────────────────────────────────────────┘ │
│                                                          │
│  드롭 시 선택 다이얼로그:                                 │
│  ┌─────────────────────────────────────────────────────┐ │
│  │ $Temperature (AI) 오브젝트 선택:                    │ │
│  │                                                     │ │
│  │  [숫자 표시]  [아날로그 게이지]  [트렌드 차트]       │ │
│  │  [바 그래프]  [상태 텍스트]      [사용자 정의...]    │ │
│  │                                                     │ │
│  │  ☑ 라벨 자동 추가 ($Temperature.des)                │ │
│  │  ☑ 단위 자동 표시 ($Temperature.unit)               │ │
│  │  ☑ 알람 범위 표시 (hihi/lolo)                       │ │
│  └─────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────┘
```

**구현 상세**:

```csharp
// SmartObjectResolver.cs
public class SmartObjectResolver
{
    // 태그 타입 → 추천 오브젝트 매핑
    private static readonly Dictionary<EnumTagType, List<SmartObjectTemplate>> TagTypeMapping
        = new Dictionary<EnumTagType, List<SmartObjectTemplate>>
    {
        [EnumTagType.AnalogInput] = new List<SmartObjectTemplate> {
            new SmartObjectTemplate("숫자 표시", EnumObjectType.TEXT,
                autoBindValue: true, autoBindUnit: true, autoBindLabel: true),
            new SmartObjectTemplate("아날로그 게이지", EnumObjectType.ANALOG_GAUGE,
                autoBindValue: true, autoBindRange: true),
            new SmartObjectTemplate("트렌드 차트", EnumObjectType.MULTI_TREND,
                autoBindValue: true, autoBindHistory: true),
        },
        [EnumTagType.DigitalOutput] = new List<SmartObjectTemplate> {
            new SmartObjectTemplate("토글 버튼", EnumObjectType.BUTTON_3D,
                autoBindValue: true, scriptTemplate: "toggle"),
            new SmartObjectTemplate("상태 램프", EnumObjectType.DIGITAL_ANIMATION,
                autoBindValue: true, onOffColors: true),
        },
        // ... 기타 태그 타입
    };

    public SmartInsertResult ResolveSmartInsert(TagInfo tag, Point dropPosition)
    {
        var templates = TagTypeMapping[tag.TagType];
        // 다이얼로그 표시 또는 기본 템플릿 사용
        return new SmartInsertResult {
            Objects = GenerateObjects(tag, selectedTemplate, dropPosition),
            TagBindings = GenerateBindings(tag, selectedTemplate),
            Scripts = GenerateScripts(tag, selectedTemplate),
        };
    }
}
```

#### 4.1.2 다중 태그 일괄 배치

```
[신규 파일] Studio/SmartInsert/FormBatchInsert.cs

동작: 여러 태그를 한번에 선택하여 그리드 형태로 자동 배치

┌───────────────────────────────────────────────────────────┐
│ 일괄 배치 마법사                                           │
│ ─────────────────────────────────────────────────────────  │
│                                                            │
│ 1. 태그 선택                     2. 레이아웃 설정          │
│ ┌────────────────────────┐      ┌──────────────────────┐  │
│ │ ☑ $Reactor1.Temp       │      │ 배치 형식:           │  │
│ │ ☑ $Reactor1.Press      │      │ ○ 수평 그리드        │  │
│ │ ☑ $Reactor1.Level      │      │ ● 수직 리스트        │  │
│ │ ☑ $Reactor1.Flow       │      │ ○ 자유 배치          │  │
│ │ ☐ $Reactor1.Valve      │      │                      │  │
│ │                        │      │ 열 수: [2]           │  │
│ │ [전체 선택] [해제]     │      │ 간격:  [20] px       │  │
│ └────────────────────────┘      │ 오브젝트: [숫자표시▼]│  │
│                                  └──────────────────────┘  │
│                                                            │
│ 3. 미리보기                                                │
│ ┌──────────────────────────────────────────────────────┐  │
│ │  ┌──────────┐  ┌──────────┐                          │  │
│ │  │ Temp     │  │ Press    │                          │  │
│ │  │ 127.5 ℃ │  │ 3.2 MPa  │                          │  │
│ │  └──────────┘  └──────────┘                          │  │
│ │  ┌──────────┐  ┌──────────┐                          │  │
│ │  │ Level    │  │ Flow     │                          │  │
│ │  │ 75.2 %   │  │ 120 L/m  │                          │  │
│ │  └──────────┘  └──────────┘                          │  │
│ └──────────────────────────────────────────────────────┘  │
│                                                            │
│                              [배치] [취소]                  │
└───────────────────────────────────────────────────────────┘
```

### 4.2 AI 기반 자동 레이아웃

#### 4.2.1 레이아웃 추천 엔진

```
[신규 파일] python_ai_engine/services/layout_service.py
[신규 파일] Studio/SmartInsert/AiLayoutAdvisor.cs

서비스: layout/suggest

입력:
{
    "service": "layout/suggest",
    "payload": {
        "module_size": { "width": 1920, "height": 1080 },
        "existing_objects": [...],    // 이미 배치된 오브젝트
        "new_objects": [...],          // 새로 배치할 오브젝트
        "constraints": {
            "avoid_overlap": true,
            "maintain_alignment": true,
            "group_related_tags": true
        }
    }
}

출력:
{
    "layout": [
        { "object_id": "obj_1", "x": 100, "y": 50, "width": 200, "height": 80 },
        { "object_id": "obj_2", "x": 100, "y": 150, "width": 200, "height": 80 },
        ...
    ],
    "guidelines": [
        { "type": "alignment", "axis": "x", "value": 100 },
        { "type": "spacing", "direction": "vertical", "value": 20 }
    ],
    "confidence": 0.85
}
```

#### 4.2.2 스마트 정렬 가이드

```
[변경 파일] Studio/FormEditGraphic.cs (캔버스 렌더링)

동작: 오브젝트 이동/크기 조절 시 스마트 가이드 표시

┌────────────────────────────────────────────────┐
│                                                 │
│     ┌──────────┐                                │
│     │  Temp    │                                │
│     │  127.5℃  │                                │
│     └──────────┘                                │
│     ·                                           │
│     · (정렬 가이드 - 파란 점선)                   │
│     ·                                           │
│     ┌──────────┐  ← 20px →  ┌──────────┐      │
│     │  Press   │·············│  Level   │      │
│     │  3.2 MPa │   간격 표시  │  75.2%   │      │
│     └──────────┘             └──────────┘      │
│                                                 │
│  ℹ️ "Press"가 "Temp" 아래에 정렬됩니다 (20px)   │
└────────────────────────────────────────────────┘

기능:
- 인접 오브젝트와의 정렬 가이드 (파란 점선)
- 등간격 스냅 (가장 가까운 일정 간격으로 스냅)
- 간격 수치 표시
- 중앙 정렬 가이드
- Ctrl 키로 스냅 일시 해제
```

### 4.3 모듈 템플릿 시스템

#### 4.3.1 파라메트릭 컴포넌트 템플릿

```
[신규 파일] Studio/Template/ModuleTemplateManager.cs
[신규 파일] Studio/Template/FormTemplateWizard.cs

개념: 재사용 가능한 파라미터화된 모듈 컴포넌트

┌───────────────────────────────────────────────────────────┐
│ 템플릿 라이브러리                                          │
│ ─────────────────────────────────────────────────────────  │
│                                                            │
│ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐         │
│ │ 밸브     │ │ 펌프     │ │ 반응기   │ │ 탱크    │         │
│ │ 제어패널 │ │ 제어패널 │ │ 모니터링 │ │ 레벨    │         │
│ │         │ │         │ │         │ │ 모니터  │         │
│ │ [미리보기]│ │ [미리보기]│ │ [미리보기]│ │ [미리보기]│        │
│ └─────────┘ └─────────┘ └─────────┘ └─────────┘         │
│                                                            │
│ 템플릿 적용 시 파라미터 입력:                               │
│ ┌──────────────────────────────────────────────────────┐  │
│ │ "밸브 제어패널" 템플릿                                │  │
│ │                                                      │  │
│ │ 태그 매핑:                                           │  │
│ │   밸브 출력 태그:  [$Reactor1.Valve      ▼]          │  │
│ │   밸브 상태 태그:  [$Reactor1.ValveSts   ▼]          │  │
│ │   인터록 태그:     [$Reactor1.Interlock  ▼]          │  │
│ │                                                      │  │
│ │ 스타일:                                              │  │
│ │   크기: ○ 소  ● 중  ○ 대                            │  │
│ │   테마: [기본 ▼]                                     │  │
│ │                                                      │  │
│ │                        [삽입] [취소]                  │  │
│ └──────────────────────────────────────────────────────┘  │
└───────────────────────────────────────────────────────────┘
```

#### 4.3.2 템플릿 저장 ("이 구성을 템플릿으로 저장")

```
동작 흐름:
1. 사용자가 캔버스에서 오브젝트 그룹 선택
2. 우클릭 → "템플릿으로 저장"
3. 태그 참조를 파라미터로 자동 추출
4. 이름/카테고리/설명 입력
5. 라이브러리에 저장

데이터 구조:
{
    "templateId": "valve-control-panel-001",
    "name": "밸브 제어패널",
    "category": "제어",
    "description": "밸브 ON/OFF 제어 + 상태 표시 + 인터록",
    "parameters": [
        { "name": "valve_output", "type": "DO", "label": "밸브 출력 태그" },
        { "name": "valve_status", "type": "DI", "label": "밸브 상태 태그" },
        { "name": "interlock",    "type": "DI", "label": "인터록 태그" }
    ],
    "objects": [ /* 직렬화된 오브젝트 트리 (태그→파라미터 치환) */ ],
    "preview": "base64_thumbnail",
    "version": 1
}
```

### 4.4 자연어 기반 모듈 생성

```
[신규 파일] python_ai_engine/services/module_gen_service.py
[신규 파일] Studio/SmartInsert/FormAiModuleGenerator.cs

서비스: module/generate

동작:
┌───────────────────────────────────────────────────────────┐
│ AI 모듈 생성기                                             │
│ ─────────────────────────────────────────────────────────  │
│                                                            │
│ 💬 어떤 화면을 만들고 싶으신가요?                           │
│                                                            │
│ ┌──────────────────────────────────────────────────────┐  │
│ │ 반응기 1번의 온도, 압력, 레벨을 모니터링하는 화면을    │  │
│ │ 만들어줘. 각 값의 트렌드 차트도 아래에 배치하고,       │  │
│ │ 알람 상태를 표시하는 영역도 포함해줘.                   │  │
│ └──────────────────────────────────────────────────────┘  │
│                                                            │
│ 🔍 감지된 태그:                                            │
│   $Reactor1.Temp, $Reactor1.Press, $Reactor1.Level        │
│                                                            │
│ ┌─ 생성된 레이아웃 미리보기 ──────────────────────────┐   │
│ │  ┌──────── 반응기 1 모니터링 ────────┐              │   │
│ │  │  ┌────┐  ┌────┐  ┌────┐          │              │   │
│ │  │  │Temp│  │Press│  │Level│         │              │   │
│ │  │  │127℃│  │3.2M│  │75% │          │              │   │
│ │  │  └────┘  └────┘  └────┘          │              │   │
│ │  │  ┌────────────────────────────┐  │              │   │
│ │  │  │      트렌드 차트           │  │              │   │
│ │  │  │  ~~~\  /~~~  ~~~~/\~~~~    │  │              │   │
│ │  │  └────────────────────────────┘  │              │   │
│ │  │  ┌────────────────────────────┐  │              │   │
│ │  │  │ ⚠ 알람 영역                │  │              │   │
│ │  │  └────────────────────────────┘  │              │   │
│ │  └───────────────────────────────┘  │              │   │
│ └──────────────────────────────────────────────────────┘  │
│                                                            │
│           [적용] [재생성] [수정 요청] [취소]                │
└───────────────────────────────────────────────────────────┘
```

---

## 5. Phase 3 — 속성 편집 및 일괄 작업 UX

### 5.1 다중 선택 일괄 속성 편집

```
[변경 파일] Studio/Property/ClassEditProperty.cs
[신규 파일] Studio/Property/FormBatchPropertyEdit.cs

현행: 한 번에 하나의 오브젝트만 속성 편집 가능
개선: 여러 오브젝트 선택 시 공통 속성 일괄 편집

┌──────────────────────────────────────────┐
│ 속성 (3개 선택됨)                         │
│ ─────────────────────────────────────── │
│                                          │
│ ▼ 공통 속성                              │
│   배경색:    [■ 혼합 ▼]  ← 값이 다를 때  │
│   전경색:    [■ #000  ▼]  ← 값이 같을 때 │
│   폰트:      [Gulim 12 ▼]               │
│   표시 형식:  [#.## ▼]                   │
│                                          │
│ ▼ 위치/크기                              │
│   X:     [혼합]   Y:     [200]           │
│   너비:  [200]    높이:  [80]            │
│                                          │
│ ▼ 태그 바인딩                            │
│   값 태그:  [각각 다름 — 클릭하여 확인]   │
│                                          │
│  [일괄 적용] [선택 해제]                  │
└──────────────────────────────────────────┘

규칙:
- 동일한 값: 해당 값 표시
- 다른 값: "혼합" 표시, 수정 시 전체 적용
- 빈 필드: 선택적 적용 체크박스
```

### 5.2 포맷 브러시 (스타일 복사)

```
[변경 파일] Studio/FormEditGraphicFrame.cs (툴바)
[신규 파일] Studio/Edit/FormatBrush.cs

단축키: Ctrl+Shift+C (스타일 복사) / Ctrl+Shift+V (스타일 붙여넣기)
또는 툴바 🖌 버튼 → 원본 클릭 → 대상 클릭

복사되는 속성:
┌─────────────────────────────┐
│ 포맷 브러시 옵션             │
│                              │
│ ☑ 색상 (배경/전경/테두리)    │
│ ☑ 폰트 (서체/크기/스타일)   │
│ ☑ 크기 (너비/높이)          │
│ ☐ 위치                      │
│ ☑ 표시 형식                  │
│ ☑ 애니메이션 설정            │
│ ☐ 태그 바인딩               │
│ ☐ 스크립트                  │
│                              │
│ 모드: ● 단일  ○ 연속 (더블클릭) │
└─────────────────────────────┘
```

### 5.3 전역 검색 (Command Palette)

```
[신규 파일] Studio/Search/FormGlobalSearch.cs
[신규 파일] Studio/Search/SearchIndex.cs

단축키: Ctrl+Shift+P (Command Palette) 또는 Ctrl+F3 (전역 검색)

┌───────────────────────────────────────────────────┐
│ 🔍 검색:  reactor temp                            │
│ ─────────────────────────────────────────────────  │
│                                                    │
│ 📋 태그 (3)                                       │
│   $Reactor1.Temp        AI    반응기1 온도 센서    │
│   $Reactor2.Temp        AI    반응기2 온도 센서    │
│   $ReactorTemp.Setpoint AO    온도 설정값          │
│                                                    │
│ 📄 모듈 (2)                                       │
│   반응기_온도_모니터링.mod     800x600             │
│   Reactor_Control.mod          1920x1080           │
│                                                    │
│ 📝 스크립트 (1)                                    │
│   온도_알람_처리.script:15     "if ($Reactor..."   │
│                                                    │
│ 🎨 오브젝트 (4)                                   │
│   Text "Reactor Temp" in 반응기_모니터링.mod:L2    │
│   AnalogGauge "Temp" in Reactor_Control.mod:L1     │
│   ...                                              │
│                                                    │
│ ⚡ 명령                                            │
│   새 모듈 생성...                                  │
│   새 스크립트 생성...                               │
│   빌드 및 배포...                                   │
└───────────────────────────────────────────────────┘
```

### 5.4 즐겨찾기 및 최근 사용 패널

```
[신규 파일] Studio/Navigation/FormFavorites.cs

┌──────────────────────────────┐
│ ⭐ 즐겨찾기                   │
│ ─────────────────────────── │
│ 📄 반응기_모니터링.mod       │
│ 📝 알람_처리.script          │
│ 📋 $Reactor1.Temp           │
│                              │
│ 🕐 최근 열기                 │
│ ─────────────────────────── │
│ 📄 펌프_제어.mod       10분전│
│ 📝 데이터_로깅.script  1시간 │
│ 📄 개요_화면.mod       2시간 │
│                              │
│ 📊 최근 편집                 │
│ ─────────────────────────── │
│ $Reactor1.Temp 값 변경       │
│ 펌프_제어.mod 버튼 추가      │
│ 알람_처리.script 조건 수정   │
└──────────────────────────────┘
```

### 5.5 키보드 단축키 강화

```
[변경 파일] Studio/StudioMain.cs
[신규 파일] Studio/Config/ShortcutManager.cs

신규 단축키:
┌─────────────────────────────────────────────────────────┐
│ 범주          단축키              기능                    │
│ ──────────────────────────────────────────────────────  │
│ 전역          Ctrl+Shift+P       Command Palette        │
│ 전역          Ctrl+T             전역 심볼 검색          │
│ 전역          Ctrl+Tab           열린 문서 간 전환       │
│ 전역          Ctrl+`             스크립트 미리보기 토글   │
│ ──────────────────────────────────────────────────────  │
│ 작화          Ctrl+D             선택 복제 (Duplicate)   │
│ 작화          Ctrl+Shift+C/V     포맷 복사/붙여넣기      │
│ 작화          Ctrl+G             그룹화                  │
│ 작화          Ctrl+Shift+G       그룹 해제               │
│ 작화          Alt+Arrow          1px 이동                │
│ 작화          Shift+Alt+Arrow    10px 이동               │
│ 작화          Ctrl+Shift+A       모두 정렬 (AI 정렬)     │
│ ──────────────────────────────────────────────────────  │
│ 스크립트      Ctrl+I             AI 어시스트             │
│ 스크립트      Ctrl+Shift+F       코드 포맷팅             │
│ 스크립트      F5                 스크립트 실행 미리보기   │
│ 스크립트      Ctrl+Shift+O       코드 아웃라인           │
│ 스크립트      Ctrl+.             Quick Fix               │
│ 스크립트      F12                정의로 이동 (태그/메서드) │
│ 스크립트      Shift+F12          모든 참조 찾기          │
└─────────────────────────────────────────────────────────┘
```

---

## 6. Phase 4 — Python AI Engine 확장 서비스

### 6.1 신규 서비스 설계

#### 6.1.1 코드 생성 서비스

```python
# python_ai_engine/services/codegen_service.py

서비스 경로: codegen/generate, codegen/explain, codegen/fix

# codegen/generate - 자연어 → 코드 생성
입력:
{
    "service": "codegen/generate",
    "payload": {
        "prompt": "온도가 상한값 초과 시 알람 발생",
        "context": {
            "available_tags": ["$Temperature", "$Pressure"],
            "available_methods": ["@SetAlarm", "@LogToDatabase"],
            "existing_code": "// 기존 코드...",
            "cursor_position": { "line": 5, "column": 1 }
        },
        "language": "autobase-script"  // C# DSL
    }
}

출력:
{
    "status": "ok",
    "result": {
        "code": "double temp = $Temperature.value;\nif (temp > $Temperature.hihi) {\n    @SetAlarm(\"TEMP_HIGH\");\n}",
        "explanation": "온도 태그 값을 읽어 상한값(hihi) 초과 시 알람을 발생시킵니다.",
        "confidence": 0.92
    }
}

# codegen/explain - 코드 설명
입력:
{
    "service": "codegen/explain",
    "payload": {
        "code": "if ($Temperature.value > $Temperature.hihi) { @SetAlarm(\"TEMP_HIGH\"); }",
        "language": "autobase-script"
    }
}

# codegen/fix - 코드 오류 수정 제안
입력:
{
    "service": "codegen/fix",
    "payload": {
        "code": "if ($Temperature.value > $Temperature.hihi)\n    @SetAlarm(TEMP_HIGH);",
        "errors": [
            { "line": 2, "message": "Undefined identifier: TEMP_HIGH" }
        ]
    }
}
```

#### 6.1.2 레이아웃 추천 서비스

```python
# python_ai_engine/services/layout_service.py

서비스 경로: layout/suggest, layout/optimize, layout/validate

# layout/suggest - 오브젝트 배치 추천
입력:
{
    "service": "layout/suggest",
    "payload": {
        "canvas_size": { "width": 1920, "height": 1080 },
        "objects_to_place": [
            { "type": "TEXT", "tag": "$Temp", "label": "온도" },
            { "type": "TEXT", "tag": "$Press", "label": "압력" },
            { "type": "MULTI_TREND", "tags": ["$Temp", "$Press"] },
            { "type": "ALARM_LIST", "scope": "Reactor1" }
        ],
        "existing_objects": [],
        "style_preference": "industrial-modern"
    }
}

출력:
{
    "status": "ok",
    "result": {
        "placements": [
            { "id": 0, "rect": { "x": 50, "y": 50, "w": 200, "h": 80 } },
            { "id": 1, "rect": { "x": 270, "y": 50, "w": 200, "h": 80 } },
            { "id": 2, "rect": { "x": 50, "y": 150, "w": 420, "h": 300 } },
            { "id": 3, "rect": { "x": 50, "y": 470, "w": 420, "h": 200 } }
        ],
        "layout_type": "dashboard-vertical",
        "score": 0.88
    }
}

# layout/optimize - 기존 레이아웃 최적화
# layout/validate - 레이아웃 검증 (겹침, 여백, 접근성)
```

#### 6.1.3 태그 분석 서비스

```python
# python_ai_engine/services/tag_analysis_service.py

서비스 경로: tag/analyze, tag/suggest-bindings, tag/find-related

# tag/suggest-bindings - 오브젝트에 적합한 태그 추천
입력:
{
    "service": "tag/suggest-bindings",
    "payload": {
        "object_type": "ANALOG_GAUGE",
        "context": {
            "module_name": "반응기_모니터링",
            "nearby_objects": ["$Reactor1.Temp", "$Reactor1.Press"],
            "all_tags": [...]
        }
    }
}

출력:
{
    "status": "ok",
    "result": {
        "suggestions": [
            { "tag": "$Reactor1.Level", "confidence": 0.95, "reason": "같은 그룹의 미사용 아날로그 태그" },
            { "tag": "$Reactor1.Flow", "confidence": 0.80, "reason": "반응기 관련 아날로그 태그" }
        ]
    }
}

# tag/find-related - 연관 태그 탐색
# tag/analyze - 태그 사용 패턴 분석
```

### 6.2 서비스 통합 아키텍처

```
┌──────────────────────────────────────────────────────────┐
│                    Studio (C# WinForms)                    │
│                                                            │
│  MonacoEditorBridge ──→ autobase-ai-assist.js             │
│       │                        │                           │
│  SmartObjectResolver           │                           │
│       │                        │                           │
│  AiLayoutAdvisor               │                           │
│       │                        │                           │
│       └────────┬───────────────┘                           │
│                │                                           │
│         PythonAiManager.CallAsync()                        │
│                │ TCP (127.0.0.1:5678)                      │
└────────────────┼───────────────────────────────────────────┘
                 │
┌────────────────▼───────────────────────────────────────────┐
│              Python AI Engine                               │
│                                                             │
│  ┌─────────┐  ┌─────────┐  ┌──────────┐  ┌────────────┐  │
│  │ codegen │  │ layout  │  │ tag      │  │ 기존 서비스 │  │
│  │ service │  │ service │  │ analysis │  │ (script,   │  │
│  │         │  │         │  │ service  │  │  predict,  │  │
│  │ generate│  │ suggest │  │          │  │  vision,   │  │
│  │ explain │  │ optimize│  │ analyze  │  │  analysis) │  │
│  │ fix     │  │ validate│  │ suggest  │  │            │  │
│  └─────────┘  └─────────┘  └──────────┘  └────────────┘  │
│                                                             │
│  구현 방식:                                                 │
│  - 규칙 기반(Rule-based) 로직 우선 구현                     │
│  - 선택적 LLM API 연동 (codegen 서비스)                     │
│  - 로컬 ONNX 모델 활용 (layout 최적화)                      │
│  - 통계 기반 추천 (tag analysis)                            │
└─────────────────────────────────────────────────────────────┘
```

---

## 7. 구현 우선순위 및 로드맵

### 7.1 우선순위 매트릭스

```
                  구현 난이도
           낮음 ◄──────────► 높음
     ┌────────────────────────────┐
높   │  ★ 파라미터 힌트    │  AI 코드 생성   │
음   │  ★ 호버 문서화     │  AI 레이아웃    │
     │  ★ 실시간 진단     │  모듈 자동생성   │
사   │  ★ Quick Fix       │                │
용   ├─────────────────────┤────────────────┤
자   │  포맷 브러시        │  템플릿 시스템   │
가   │  ★ 태그 드래그 삽입 │  코드 아웃라인   │
치   │  다중 속성 편집     │  전역 검색       │
     │  키보드 단축키      │                 │
낮   │                    │  즐겨찾기 패널   │
음   │                    │  실행 미리보기    │
     └────────────────────────────────────────┘

★ = Phase 1 권장 (Quick Win)
```

### 7.2 단계별 로드맵

#### Phase 1: Quick Win (2-3주) — 스크립트 에디터 핵심 개선

| # | 항목 | 변경 파일 | 예상 공수 |
|---|------|----------|----------|
| 1 | SignatureHelpProvider (파라미터 힌트) | `autobase-completions.js` + 신규 js | 3일 |
| 2 | HoverProvider (태그/메서드 호버 문서) | 신규 `autobase-hover.js` | 3일 |
| 3 | 실시간 구문 진단 (DiagnosticsProvider) | 신규 `autobase-diagnostics.js` | 4일 |
| 4 | Quick Fix (CodeActionProvider) | 진단 시스템 확장 | 2일 |
| 5 | 컨텍스트 인식 자동완성 확장 | `autobase-completions.js` 수정 | 3일 |

#### Phase 2: 작화 UX 혁신 (3-4주) — AI 기반 모듈 편집

| # | 항목 | 변경 파일 | 예상 공수 |
|---|------|----------|----------|
| 1 | 태그 드래그 → 자동 오브젝트 생성 | `ClassEditInsert.cs` + 신규 | 5일 |
| 2 | 다중 태그 일괄 배치 마법사 | 신규 `FormBatchInsert.cs` | 4일 |
| 3 | 스마트 정렬 가이드 | `FormEditGraphic.cs` | 3일 |
| 4 | 포맷 브러시 | 신규 `FormatBrush.cs` + 툴바 | 3일 |
| 5 | 다중 선택 일괄 속성 편집 | `ClassEditProperty.cs` + 신규 | 5일 |

#### Phase 3: AI Engine 확장 (4-5주) — 고급 AI 기능

| # | 항목 | 변경 파일 | 예상 공수 |
|---|------|----------|----------|
| 1 | codegen 서비스 (코드 생성/설명/수정) | 신규 Python 서비스 | 7일 |
| 2 | layout 서비스 (레이아웃 추천) | 신규 Python 서비스 | 5일 |
| 3 | tag_analysis 서비스 (태그 분석) | 신규 Python 서비스 | 3일 |
| 4 | AI 어시스트 UI (Ctrl+I) | `MonacoEditorBridge.cs` + js | 5일 |
| 5 | AI 모듈 자동생성 | 신규 Form + 서비스 연동 | 5일 |

#### Phase 4: 워크플로우 완성 (2-3주) — 생산성 도구

| # | 항목 | 변경 파일 | 예상 공수 |
|---|------|----------|----------|
| 1 | 전역 검색 (Command Palette) | 신규 `FormGlobalSearch.cs` | 5일 |
| 2 | 코드 아웃라인 패널 | 신규 `UserControlCodeOutline.cs` | 4일 |
| 3 | 스크립트 실행 미리보기 | 신규 `UserControlScriptPreview.cs` | 4일 |
| 4 | 모듈 템플릿 시스템 | 신규 Template 폴더 | 5일 |
| 5 | 키보드 단축키 체계화 | `StudioMain.cs` + 신규 | 2일 |

### 7.3 기대 효과

```
┌────────────────────────────────────────────────────────────┐
│                     기대 효과 요약                          │
│                                                             │
│  📈 생산성 향상                                             │
│  ├── 스크립트 작성 시간 40-50% 단축 (AI 완성 + 실시간 검증) │
│  ├── 작화 시간 30-40% 단축 (스마트 삽입 + 일괄 배치)        │
│  └── 디버깅 시간 50% 단축 (실시간 진단 + Quick Fix)         │
│                                                             │
│  🎯 품질 향상                                               │
│  ├── 구문 오류 사전 방지 (실시간 검증)                       │
│  ├── 일관된 레이아웃 (AI 정렬 + 포맷 브러시)                 │
│  └── 코드 가독성 향상 (포맷팅 + 아웃라인)                    │
│                                                             │
│  🧑‍💻 학습 곡선 완화                                         │
│  ├── 호버 문서로 API 즉시 확인                               │
│  ├── AI 코드 생성으로 초보자 지원                             │
│  └── 전역 검색으로 프로젝트 탐색 용이                         │
└────────────────────────────────────────────────────────────┘
```

---

## 부록: 파일 변경 요약

### 변경 대상 기존 파일

| 파일 | 변경 내용 |
|------|----------|
| `ScriptLib/.../MonacoFiles/autobase-completions.js` | 컨텍스트 인식 완성, 확장 트리거 |
| `ScriptLib/.../Editor/MonacoEditorBridge.cs` | AI 어시스트 통신, 진단 연동 |
| `Studio/FormEditGraphicFrame.cs` | 스마트 삽입 툴바, 포맷 브러시 |
| `Studio/FormEditGraphic.cs` | 스마트 정렬 가이드 렌더링 |
| `Studio/ClassEditInsert.cs` | 스마트 삽입 메서드 |
| `Studio/Property/ClassEditProperty.cs` | 다중 선택 속성 편집 |
| `Studio/StudioMain.cs` | 전역 검색, 단축키, 메뉴 |
| `Studio/Script/FormNewScriptEditor.cs` | 아웃라인 패널, 실행 미리보기 |

### 신규 파일

| 파일 | 용도 |
|------|------|
| `ScriptLib/.../MonacoFiles/autobase-signature-help.js` | 파라미터 힌트 |
| `ScriptLib/.../MonacoFiles/autobase-hover.js` | 호버 문서화 |
| `ScriptLib/.../MonacoFiles/autobase-diagnostics.js` | 실시간 진단 |
| `ScriptLib/.../MonacoFiles/autobase-ai-assist.js` | AI 코드 생성 UI |
| `Studio/SmartInsert/SmartObjectResolver.cs` | 태그→오브젝트 자동 매핑 |
| `Studio/SmartInsert/FormBatchInsert.cs` | 일괄 배치 마법사 |
| `Studio/SmartInsert/FormAiModuleGenerator.cs` | AI 모듈 생성기 |
| `Studio/SmartInsert/AiLayoutAdvisor.cs` | 레이아웃 추천 |
| `Studio/Edit/FormatBrush.cs` | 포맷 브러시 |
| `Studio/Property/FormBatchPropertyEdit.cs` | 다중 속성 편집 |
| `Studio/Search/FormGlobalSearch.cs` | 전역 검색 |
| `Studio/Search/SearchIndex.cs` | 검색 인덱스 |
| `Studio/Navigation/FormFavorites.cs` | 즐겨찾기 |
| `Studio/Config/ShortcutManager.cs` | 단축키 관리 |
| `Studio/Script/UserControlCodeOutline.cs` | 코드 아웃라인 |
| `Studio/Script/UserControlScriptPreview.cs` | 실행 미리보기 |
| `Studio/Template/ModuleTemplateManager.cs` | 템플릿 관리 |
| `Studio/Template/FormTemplateWizard.cs` | 템플릿 마법사 |
| `python_ai_engine/services/codegen_service.py` | 코드 생성 서비스 |
| `python_ai_engine/services/layout_service.py` | 레이아웃 서비스 |
| `python_ai_engine/services/tag_analysis_service.py` | 태그 분석 서비스 |

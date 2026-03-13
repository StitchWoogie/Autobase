================================================================================
  AutobasePrime Recipe & Preset 기술 참조 문서 (개발팀용)
  작성일: 2026-03-08
================================================================================


1. 개요
================================================================================

이 문서는 AutobasePrime의 Recipe(레시피) 및 Preset(프리셋) 서브시스템의
내부 아키텍처, 데이터 모델, 실행 엔진, 서비스 계층을 다룹니다.

대상 독자: 개발팀, 코드 리뷰어, 유지보수 엔지니어
프레임워크: .NET Framework 4.8, C# 7.3
표준: ISA-88 Batch Control (Lite 구현)


2. 시스템 아키텍처
================================================================================

2.1 계층 구조
─────────────

  ┌─────────────────────────────────────────────────────────────────┐
  │  UI Layer (WinForms)                                            │
  │  FormConfigRecipe, FormConfigPreset, FormConfigRecipeStep       │
  │  FormElectronicSignature, FormRecipe                            │
  └───────────────────────────┬─────────────────────────────────────┘
                              │
  ┌───────────────────────────┼─────────────────────────────────────┐
  │  Manager Layer            │                                     │
  │  RecipeManager (DB)       PresetManager (JSON)                  │
  └───────────────────────────┬─────────────────────────────────────┘
                              │
  ┌───────────────────────────┼─────────────────────────────────────┐
  │  Execution Layer          │                                     │
  │  CheckEngineRecipe        BatchStateMachine                     │
  │  RecipeExpressionEvaluator                                      │
  │  PresetScriptBridge                                             │
  └───────────────────────────┬─────────────────────────────────────┘
                              │
  ┌───────────────────────────┼─────────────────────────────────────┐
  │  Service Layer (WCF)      │                                     │
  │  IServiceRecipe / WcfServiceRecipe                              │
  │  IServicePreset / WcfServicePreset                              │
  │  ClassDataGateServer (V2_Preset*, V2_Recipe*)                   │
  │  ServiceLibSvcDataGate (net.tcp proxy)                          │
  └───────────────────────────┬─────────────────────────────────────┘
                              │
  ┌───────────────────────────┼─────────────────────────────────────┐
  │  Data Layer               │                                     │
  │  DataPostgres (PostgreSQL async CRUD)                           │
  │  RecipeJsonHelper (JSON 직렬화)                                 │
  │  RecipeCsvHelper (CSV import/export)                            │
  │  JSON 파일 (Presets/*.json, RECIPE/recipes.json)                │
  └─────────────────────────────────────────────────────────────────┘


2.2 "단일 경유점" 패턴 (Preset)
───────────────────────────────

모든 Preset 파일 I/O는 LocalMain 프로세스가 전담합니다.
PortalServerWeb(IIS)은 파일 직접 접근 대신 WCF를 통해 위임합니다.

  [ViewMain Client]                [PortalServerWeb (IIS)]            [LocalMain]
  FormConfigPreset                 WcfServicePreset.svc               ClassDataGateServer
    │ bLocalFlag?                    │ Init()                           │
    ├ true → PresetManager         ServiceLibSvcDataGate              V2_Preset handlers
    ├ WcfService → ServiceLib..     .Command("V2_PresetXxx")            │
    │           → net.tcp:8732        ─── net.tcp ──→               PresetManager
    └ else → ServiceReferencePreset                                  (파일 I/O 전담)
             → WcfServicePreset
             → ServiceLibSvcDataGate ──→

이유: IIS가 다른 PC에 있을 수 있어 Named Mutex 불가 → 단일 경유점 채택


2.3 ConfigVarTotal.bLocalFlag 분기
──────────────────────────────────

FormConfigPreset, FormConfigRecipe 등 UI 폼에서 데이터 접근 시:

  if (ConfigVarTotal.bLocalFlag)
  {
      // 로컬: 직접 파일 시스템 접근 (PresetManager, File.ReadAllText)
  }
  else
  {
      // 원격: DataGate 웹서비스 경유
      DataGate gate = new DataGate();
      gate.PresetGetList() / gate.PresetGet() / gate.PresetSave() / ...
  }


3. 데이터 모델
================================================================================

3.1 Recipe 데이터 모델 (DataPostgres.cs)
────────────────────────────────────────

  RecipeData : RecipeInfo
  ├─ recipe_id          int         DB PK
  ├─ recipe_name        string      레시피 이름
  ├─ description        string      설명
  ├─ recipe_guid        string      크로스시스템 불변 식별자 (GUID "N" 형식)
  ├─ recipe_code        string      (deprecated, 항상 빈 문자열)
  ├─ version            int         자동 증가 (저장 시 version++)
  ├─ is_active          bool        소프트 삭제 플래그
  ├─ status             string      "draft" | "approved" | "obsolete"
  ├─ approved_by        string      승인자 이름
  ├─ approved_at        DateTime?   승인 일시
  ├─ recipe_mode        string      "standard" | "quick"
  ├─ created_at         DateTime
  ├─ updated_at         DateTime
  ├─ steps              ArrayList   RecipeStepData (recipe 직속, unit_id=0)
  └─ units              ArrayList   RecipeUnitData (ISA-88 Unit 계층)

  RecipeUnitData
  ├─ unit_id            int
  ├─ recipe_id          int
  ├─ unit_name          string
  ├─ unit_order         int         순서 (1-based)
  ├─ description        string
  └─ steps              ArrayList   RecipeStepData

  RecipeStepData
  ├─ step_id            int
  ├─ unit_id            int         0 = recipe 직속
  ├─ step_order         int         실행 순서 (0-based)
  ├─ step_name          string
  ├─ wait_time_ms       int         최소 대기시간
  ├─ timeout_ms         int         종료조건 타임아웃 (기본 30000)
  ├─ condition_tag      string      (legacy) 종료 조건 태그
  ├─ condition_value    string      (legacy) 종료 조건 비교값
  ├─ condition_type     string      (legacy) "none"|"equal"|"greater"|"less"
  ├─ entry_condition_tag    string  (legacy) 시작 조건 태그
  ├─ entry_condition_value  string  (legacy) 시작 조건 비교값
  ├─ entry_condition_type   string  (legacy) "none"|"equal"|"greater"|"less"
  ├─ entry_timeout_ms       int     시작 조건 타임아웃
  ├─ entry_expression       string  시작 조건 표현식 (예: "$AI_0000 > 10")
  ├─ exit_expression        string  (legacy) 종료 조건 표현식
  ├─ running_expression     string  (legacy) 실행 중 감시 표현식
  ├─ exit_actions_json      string  JSON: 정상종료 시 액션
  ├─ abort_actions_json     string  JSON: 비정상종료 시 액션
  ├─ transitions_json       string  JSON: List<StepTransition>
  └─ items              ArrayList   RecipeItemData

  RecipeItemData
  ├─ item_id            int
  ├─ tag_name           string      PLC 태그명
  ├─ set_value          string      설정값
  ├─ value_type         string      "double"|"int"|"string"|"bool"
  └─ item_order         int

  StepTransition
  ├─ priority           int         평가 우선순위 (낮을수록 먼저)
  ├─ expression         string      조건 표현식
  ├─ type               TransitionType  전이 유형
  ├─ target_step_order  int         Loop 시 점프 대상 (-1 = 현재 반복)
  ├─ max_loop_count     int         최대 반복 (기본 10, 0 = 무제한)
  ├─ description        string      설명 (UI 표시용)
  └─ timeout_ms         int         자동 전이 타임아웃 (0 = expression만)

  TransitionType enum
  ├─ Complete   = 0     정상 완료 → 다음 Step
  ├─ Exception  = 1     예외 → abort_actions → 배치 실패
  ├─ Abort      = 2     즉시 중단 → abort_actions → 배치 중단
  ├─ Loop       = 3     target_step_order로 점프
  └─ End        = 4     레시피 전체 종료 (남은 Step/Unit 건너뛰기)


3.2 Preset 데이터 모델 (PresetManager.cs)
─────────────────────────────────────────

  PresetData
  ├─ preset_name        string
  ├─ alias_map          List<AliasEntry>    별칭 ↔ 태그 매핑
  └─ variants           List<PresetVariant> 다중 변형

  AliasEntry
  ├─ alias              string      사용자 친화적 이름
  └─ tag                string      실제 PLC 태그명

  PresetVariant
  ├─ name               string      변형 이름
  └─ items              List<PresetItem>

  PresetItem
  ├─ alias              string      별칭 (AliasMap의 alias와 매칭)
  └─ value              string      설정값

AliasMap 역할:
  - 사용자는 alias로 작업, 내부적으로 tag로 변환
  - 태그 변경 시 AliasMap만 수정하면 모든 Variant에 반영
  - FindTagByAlias(aliasMap, alias) → tag 반환

JSON 파일 위치: {ProjectPath}/Presets/{PresetName}.json
  - UTF-8 BOM 인코딩
  - Newtonsoft.Json Formatting.Indented


3.3 JSON 포맷 마이그레이션 (3단계 자동 감지)
─────────────────────────────────────────────

  DeserializePreset(json):
    ① alias_map 필드 존재 → 최신 포맷 (직접 역직렬화)
    ② preset_name 존재, alias_map 없음 → 중간 포맷 (MigrateFromMidFormat)
    ③ name + items 존재 → 구 포맷 (MigrateFromOldFormat)

  중간 포맷 마이그레이션:
    - tag 필드에서 AliasMap 자동 생성 (alias = tag = 동일)
    - variants의 items를 alias 기반으로 변환

  구 포맷 마이그레이션:
    - name → PresetName
    - items → AliasMap (alias = tag) + "Default" variant 1개 생성


4. 실행 엔진 (CheckEngineRecipe)
================================================================================

4.1 실행 모드
─────────────

  ┌──────────────────┬──────────────────────┬───────────────────────────┐
  │ 항목              │ Quick 모드            │ Standard 모드              │
  ├──────────────────┼──────────────────────┼───────────────────────────┤
  │ 실행 함수         │ RecipeQuickDownload() │ BatchStart()               │
  │ Entry 조건        │ 무시                  │ entry_expression 평가      │
  │ Transition        │ 무시                  │ Type+Priority 순 평가      │
  │ Exit Actions      │ 없음                  │ exit_actions_json 실행     │
  │ Abort Actions     │ 없음                  │ abort_actions_json 실행    │
  │ Wait Time         │ 무시                  │ wait_time_ms 대기          │
  │ Timeout           │ 없음                  │ timeout_ms (기본 30s)      │
  │ Unit 병렬실행     │ 미지원                │ Task.WhenAll 병렬          │
  │ 배치 기록         │ 없음                  │ Control Recipe 스냅샷      │
  │ 상태 머신         │ 없음                  │ BatchStateMachine (ISA-88) │
  │ Hold/Resume/Abort │ 미지원                │ 지원                       │
  │ 전자서명          │ 미지원                │ 필요                       │
  └──────────────────┴──────────────────────┴───────────────────────────┘


4.2 Standard 모드 배치 실행 흐름 (BatchStart)
─────────────────────────────────────────────

  ① 전자서명 확인 (비밀번호 + 사유)
  ② BatchId 생성: "yyyyMMdd-HHmmss-NNN"
  ③ 상태 머신: Idle → Running
  ④ Master Recipe 로드 (DB), 승인 상태 확인
  ⑤ Control Recipe 스냅샷 (JSON → DB)
  ⑥ Phase A: Recipe 직속 Step 순차 실행
       while (s < DirectSteps.Count):
         Hold/Abort 체크
         ExecuteStepDownloadV2(step[s])
         switch (result.Type):
           Complete  → s++
           Loop      → s = stepOrderIndex[targetStepOrder]
           End       → 루프 탈출 + Unit 스킵
           Exception → 배치 실패
           Abort     → 배치 실패
  ⑦ Phase B: Unit 병렬 실행 (Task.WhenAll)
       각 Unit 내부는 Phase A와 동일한 while 루프
  ⑧ 완료: SetComplete(), 로그 기록


4.3 단일 Step 실행 순서 (ExecuteStepDownloadV2)
────────────────────────────────────────────────

  ⓪ 안전 제한 체크 (TotalStepExecutions > 10,000 → Exception)
  ① Entry 조건 (StepState = WaitingEntry)
     - entry_expression 정의 시: 100ms 폴링 루프
     - entry_timeout_ms 초과 → abort_actions → Exception
     - expression 평가 예외 → abort_actions → Exception (보수적)
  ② 태그 쓰기 (StepState = Running)
     - step.items 순회 → PlcScan.SetTagValue()
     - 실패 시: abort_actions → Exception
  ③ 최소 대기 (wait_time_ms > 0 → Task.Delay)
  ④ Transition 목록 로드
     - transitions_json 있으면 JSON 파싱
     - 없으면 BuildTransitionsFromLegacy() 자동 변환
     - 0개이면: exit_actions → Complete
  ⑤ Transition 평가 루프 (100ms 간격)
     - 평가 순서: Abort(0) > Exception(1) > Complete/Loop/End(2)
     - 같은 카테고리 내 priority 순
     - 충족 시 type별 처리 (exit_actions/abort_actions 실행)


4.4 Transition 평가 순서 규칙
─────────────────────────────

  ★ 산업 안전 핵심 — Type 기반 카테고리 우선:

  GetTypeCategory(TransitionType):
    Abort     → 카테고리 0 (최우선 — 비상 정지)
    Exception → 카테고리 1 (감시 조건)
    Complete  → 카테고리 2 (정상 완료)
    Loop      → 카테고리 2
    End       → 카테고리 2

  CompareTransitions(a, b):
    1차: GetTypeCategory(a.type) vs GetTypeCategory(b.type)
    2차: a.priority vs b.priority (같은 카테고리 내)

  → 사용자가 priority를 잘못 설정해도 안전 감시가 보장됨


4.5 무한루프 다중 보호 (4단계)
──────────────────────────────

  ┌─────────────────────────┬────────────────┬──────────────────────────┐
  │ 보호 계층                │ 제한값          │ 적용 범위                 │
  ├─────────────────────────┼────────────────┼──────────────────────────┤
  │ ① Step별 Loop 카운터    │ max_loop_count │ 개별 전이 (source→target)│
  │                         │ (기본 10)      │                          │
  │ ② Step 실행 총 횟수     │ 10,000회       │ 배치 전체                 │
  │ ③ 전이 평가 총 횟수     │ 1,000,000회    │ 배치 전체                 │
  │ ④ 배치 총 실행 시간     │ 24시간         │ 배치 전체                 │
  └─────────────────────────┴────────────────┴──────────────────────────┘

  모든 보호 계층 초과 시: abort_actions 실행 → Exception


4.6 Expression 문법 (RecipeExpressionEvaluator)
───────────────────────────────────────────────

  expression := or_expr
  or_expr    := and_expr ('||' and_expr)*
  and_expr   := not_expr ('&&' not_expr)*
  not_expr   := '!' not_expr | comparison
  comparison := value (('==' | '!=' | '>' | '<' | '>=' | '<=') value)?
  value      := '$TAG_NAME' | NUMBER | STRING | '(' expression ')'

  예시:
    $AI_TEMP > 80 && $DI_READY == 1
    !($DO_RUNNING == 1) || $AI_PRESSURE >= 2.5
    $ST_MODE == 'AUTO'

  태그 참조: $TAG_NAME → TagLib.GetStructPublic()로 현재값 읽기
  부동소수점 비교: epsilon = 0.0001 허용 오차
  문자열 비교: 해시 기반
  평가 예외: abort_actions 실행 후 Exception (보수적 정책)


4.7 BatchStateMachine (ISA-88 상태 전이)
────────────────────────────────────────

  BatchState enum:
    Idle → Running → Holding → Held → Restarting → Running
                  → Complete
    Running/Holding/Held/Restarting → Aborting → Aborted

  StepState enum:
    Pending → WaitingEntry → Running → WaitingExit → Completed
                                    → Aborted / Exception

  Thread Safety: private readonly object _lock
  Event: StateChanged(BatchState oldState, BatchState newState)


5. 서비스 계층 (WCF)
================================================================================

5.1 Preset 서비스
─────────────────

  인터페이스: IServicePreset
  ┌──────────────────┬─────────────────────────────────────────────────┐
  │ 메서드            │ 시그니처                                        │
  ├──────────────────┼─────────────────────────────────────────────────┤
  │ GetPresetList    │ string GetPresetList(out string error)          │
  │ GetPreset        │ string GetPreset(string name, out string error) │
  │ SavePreset       │ bool SavePreset(name, json, out string error)   │
  │ DeletePreset     │ bool DeletePreset(name, out string error)       │
  └──────────────────┴─────────────────────────────────────────────────┘

  구현: WcfServicePreset (PerCall, AspNetCompatibility)
  패턴: Init() → ServiceLibSvcDataGate → Command("V2_PresetXxx")
  전송: net.tcp://LocalMainIP:8732

  Init() 패턴:
    ConfigVarTotal.sSiteRootName = ConfigWeb.GetLocalIP();
    ConfigVarTotal.eServiceType  = EnumServiceType.WcfService;
    ConfigVarTotal.eBindType     = EnumDataGateBindingType.NetTcp;
    ConfigVarTotal.nServicePort  = 8732;


5.2 Recipe 서비스
─────────────────

  인터페이스: IServiceRecipe
  ┌──────────────────────┬─────────────────────────────────────────────┐
  │ 메서드                │ 시그니처                                    │
  ├──────────────────────┼─────────────────────────────────────────────┤
  │ GetRecipeList        │ DataSet GetRecipeList(out string error)     │
  │ GetRecipe            │ DataSet GetRecipe(int id, out string error) │
  │ RecipeDownload       │ bool (name, clientGuid, out error)          │
  │ RecipeUpload         │ bool (name, clientGuid, out error)          │
  │ RecipeDownloadUnit   │ bool (name, unitName, clientGuid, out err)  │
  │ RecipeUploadUnit     │ bool (name, unitName, clientGuid, out err)  │
  └──────────────────────┴─────────────────────────────────────────────┘

  구현: WcfServiceRecipe (PerCall)
  데이터 접근: DataPostgres.Instance 직접 호출 (DB 직접 접근)
  반환: DataSet (Recipe/Unit/Step/Item 다중 테이블)


5.3 ClassDataGateServer V2_Preset 핸들러
────────────────────────────────────────

  LocalMain 측 (변경 불필요):
    V2_PresetGetList → PresetManager.GetPresetNameList()
    V2_PresetGet     → PresetManager.LoadPresetRaw(name)
    V2_PresetSave    → PresetManager.SavePresetRaw(name, json)
    V2_PresetDelete  → PresetManager.DeletePreset(name)

  PortalServerWeb 측 (프록시):
    V2_PresetGetList → ServiceLibSvcDataGate → LocalMain → PresetManager
    V2_PresetGet     → ServiceLibSvcDataGate → LocalMain → PresetManager
    (모든 경로가 LocalMain의 PresetManager를 경유 → 파일 충돌 없음)


6. Helper 클래스
================================================================================

6.1 RecipeJsonHelper (AutoLibLocal)
───────────────────────────────────

  용도: Recipe ↔ JSON 직렬화/역직렬화, 파일 I/O

  주요 메서드:
    SaveToJsonFile(List<RecipeData>, filePath, out error) → bool
    LoadFromJsonFile(filePath, out error) → List<RecipeData>
    ToJsonString(RecipeData) → string        // Control Recipe 스냅샷
    FromJsonString(json) → RecipeData        // 스냅샷 복원
    ValidateRecipes(List<RecipeData>) → string (null=OK, else=에러)
    IsContentEqual(RecipeData a, RecipeData b) → bool

  DTO 클래스 (DB ID 제외, GUID/Code/Version 포함):
    RecipeJsonDto, RecipeUnitJsonDto, RecipeStepJsonDto, RecipeItemJsonDto

  하위호환:
    - recipe_code 없으면 빈 문자열 기본값
    - recipe_guid 없으면 자동 생성
    - transitions_json/exit_actions_json/abort_actions_json 없으면 빈 문자열


6.2 RecipeCsvHelper (DataPostgres.cs 내)
────────────────────────────────────────

  용도: Recipe ↔ CSV 변환

  주요 메서드:
    ExportToCsv(RecipeData, filePath, out error) → bool
    ImportFromCsv(filePath, out error) → RecipeData

  CSV 섹션:
    [RECIPE]          레시피 메타데이터
    [UNIT]            Unit 정의
    [STEP]            Step 정의 (조건 포함)
    [STEP_ITEM]       Tag-Value 쌍
    [STEP_TRANSITION] ISA-88 전이 정의

  내부 헬퍼: CsvEscape(), ParseCsvLine(), IsCsvHeaderLine()


6.3 PresetManager (LocalMain)
─────────────────────────────

  용도: JSON 파일 기반 Preset CRUD (DB 미사용)

  주요 메서드:
    LoadPresetList() → List<PresetData>
    LoadPreset(name) → PresetData
    LoadPresetRaw(name) → string (JSON)
    SavePreset(preset, out error) → bool
    SavePresetRaw(name, json, out error) → bool
    DeletePreset(name, out error) → bool
    ExportToCsv(preset, filePath, out error) → bool
    ImportFromCsv(filePath, out error) → PresetData
    CaptureAsPreset(template, variantName, saveName) → Task<(bool, string)>
    GetPresetNameList() → List<(string name, string date)>
    FindVariant(preset, variantName) → PresetVariant
    FindTagByAlias(aliasMap, alias) → string

  동시성: private readonly object _lock (파일 I/O 보호)
  파일 경로: {TotalConfig.sDirWorkProject}/Presets/{name}.json


6.4 PresetScriptBridge (LocalMain)
──────────────────────────────────

  용도: 스크립트 엔진에서 Preset 기능 호출

  주요 메서드:
    PresetApply(presetName, variantName) → Task<(bool, string)>
    PresetCapture(presetName, variantName, saveName) → Task<(bool, string)>
    PresetGetList() → List<string>
    PresetGetVariantCount(presetName) → int
    PresetGetVariantName(presetName, index) → string

  동작: AliasMap으로 alias→tag 변환 후 PlcScan.SetTagValue() 호출
  에러 처리: 개별 태그 실패 시 계속 진행 (best-effort)


7. UI 폼
================================================================================

7.1 FormConfigRecipe (Studio)
─────────────────────────────

  파일: PublicStudioLocalMain/Recipe/FormConfigRecipe.cs + .Designer.cs
  용도: 레시피 목록/편집/저장/CSV Import-Export
  데이터: JSON 파일 기반 (RECIPE/recipes.json)
  분기: bLocalFlag → 직접 파일 / DataGate 웹서비스

  주요 기능:
    - Recipe CRUD (Add/Delete/Save)
    - Unit CRUD (ISA-88 계층)
    - Step CRUD (FormConfigRecipeStep 다이얼로그)
    - TreeView 구조: (Direct) → Step / Unit → Step
    - Step 미리보기 패널 (전이 요약 표시)
    - CSV Export/Import (RecipeCsvHelper 위임)
    - 상태 표시: draft(주황) / approved(초록) / obsolete(회색)
    - 모드 전환: Standard(Unit 표시) / Quick(Unit 숨김)
    - 권한: RIGHT_RECIPE_VIEW, RIGHT_RECIPE_MODIFY


7.2 FormConfigRecipeStep (Studio)
─────────────────────────────────

  파일: PublicStudioLocalMain/Recipe/FormConfigRecipeStep.cs
  용도: 단일 Step 편집 다이얼로그
  모드별 UI:
    Standard: Entry 조건, Transition 그리드, Exit/Abort Actions
    Quick: Tag-Value Items만 표시 (조건/액션 UI 숨김)

  Transition DataGridView 열:
    Priority, Expression, Type(ComboBox), TargetStep, MaxLoop, Timeout, Description


7.3 FormConfigPreset (Studio/LocalMain)
───────────────────────────────────────

  파일: PublicStudioLocalMain/Recipe/FormConfigPreset.cs + .Designer.cs
  용도: Preset 설정 (AliasMap + Variant + Items)
  데이터: JSON 파일 (Presets/*.json)
  분기: bLocalFlag → 직접 파일 / DataGate 웹서비스

  주요 기능:
    - Preset CRUD (New/Delete/Save)
    - AliasMap 그리드 (Alias, Tag, SelectTag 버튼)
    - Variant TabControl (Add/Delete/선택 시 Items 전환)
    - Items DataGridView (Alias, Value)
    - 행 순서 변경 (Up/Down 버튼 + Drag & Drop)
    - AliasMap 접기/펼치기 (Toggle)
    - 검색 필터 (txtSearch)
    - CSV Export/Import
    - Apply (RUN 모드: 태그값 쓰기)
    - Capture (RUN 모드: 현재 태그값 읽기)
    - 태그 검증 경고 (TagLib.IsTagExist)
    - MDI 지원 (CatWindowRing)
    - 권한: RIGHT_RECIPE_VIEW


7.4 FormElectronicSignature
───────────────────────────

  파일: PublicStudioLocalMain/Recipe/FormElectronicSignature.cs + .Designer.cs
  용도: 전자서명 (ISA-88 Phase 6 감사 추적)

  서명 결과: "username|yyyy-MM-dd HH:mm:ss|action|reason"
  검증: DataGate.CheckUserName → SHA-256 해시 비교
  최대 실패: 5회 → 자동 취소
  정적 헬퍼: ShowSignature(owner, actionName, username, objectType, objectName, objectVersion)
            → (bool success, string signature, string reason)


8. 파일 목록
================================================================================

  LocalMain/Recipe/
  ├─ CheckEngineRecipe.cs         ISA-88 레시피 실행 엔진
  ├─ BatchStateMachine.cs         배치 상태 머신
  ├─ RecipeManager.cs             레시피 DB CRUD + 캐시
  ├─ PresetManager.cs             프리셋 JSON CRUD
  ├─ PresetScriptBridge.cs        프리셋 스크립트 브리지
  ├─ FormRecipe.cs                LocalMain 레시피 실행 UI
  ├─ FormRecipeImport.cs          JSON Import 도구

  Dll/PublicStudioLocalMain/Recipe/
  ├─ FormConfigRecipe.cs          Studio 레시피 설정 폼
  ├─ FormConfigRecipe.Designer.cs
  ├─ FormConfigRecipeStep.cs      Step 편집 다이얼로그
  ├─ FormConfigPreset.cs          Studio Preset 설정 폼
  ├─ FormConfigPreset.Designer.cs
  ├─ FormElectronicSignature.cs   전자서명 다이얼로그
  ├─ FormElectronicSignature.Designer.cs
  ├─ RecipeExecutionFlow.txt      실행 흐름 설명서

  Dll/AutoLibLocal/
  ├─ RecipeJsonHelper.cs          JSON 직렬화 유틸리티
  ├─ RecipeExpressionEvaluator.cs Expression 파서/평가기
  ├─ PostgresSQL/DataPostgres.cs  DB CRUD + RecipeCsvHelper + 데이터 모델

  PortalServer/PortalServerWeb/AutoWeb/Service/
  ├─ IServicePreset.cs            Preset WCF 인터페이스
  ├─ ServicePreset.svc.cs         Preset WCF 구현 (프록시)
  ├─ IServiceRecipe.cs            Recipe WCF 인터페이스
  ├─ ServiceRecipe.svc.cs         Recipe WCF 구현 (DB 직접)
  ├─ ClassDataGateServer.cs       V2_Preset/V2_Recipe 핸들러

  Dll/AutoLib/
  ├─ DataGate.cs                  클라이언트 DataGate
  ├─ ServiceLibSvcDataGate.cs     net.tcp 프록시
  ├─ Service References/          WCF 자동생성 프록시


9. 감사 로그 (Audit Trail)
================================================================================

9.1 테이블: history.transition_execution_log
────────────────────────────────────────────

  필드:
    batch_id, step_order, step_name, transition_index, transition_type,
    expression, evaluated_result, action_taken, unit_name, error_message,
    username, machine_name, created_at

  보호: UPDATE/DELETE 금지 RULE (변경 불가능한 감사 추적)
  기록: fire-and-forget (InsertTransitionLogFireAndForget)

  기록 대상:
    Complete/Exception/Abort/Loop/End  전이 평가 true
    ENTRY_MET                          시작 조건 충족
    ENTRY_TIMEOUT                      시작 조건 타임아웃
    ENTRY_EXPRESSION_ERROR             시작 조건 표현식 오류
    TIMEOUT                            전이 조건 타임아웃
    EXPRESSION_ERROR                   전이 표현식 오류
    LOOP_LIMIT                         반복 카운터 초과
    ACTION_FAILURE                     개별 태그 SetTagValue 실패
    UNIT_FAILURE                       Unit 실행 실패


9.2 테이블: history.recipe_execution_log
────────────────────────────────────────

  주요 action:
    BATCH_START                        배치 시작
    QUICK_DOWNLOAD                     Quick 모드 실행
    COMPLETED                          정상 완료
    FAILED                             실패
    ABORTED                            중단


10. 빌드 및 종속성
================================================================================

  빌드 도구:
    MSBuild (VS 2022 Professional)
    "C:/Program Files/Microsoft Visual Studio/2022/Professional/MSBuild/Current/Bin/MSBuild.exe"

  빌드 순서 (Recipe/Preset 관련):
    1. AutoLib          (DataGate, ServiceLibSvcDataGate, Service References)
    2. AutoLibLocal     (RecipeJsonHelper, RecipeExpressionEvaluator, DataPostgres)
    3. PublicStudioLocalMain (FormConfig*, FormElectronicSignature)
    4. LocalMain        (CheckEngineRecipe, BatchStateMachine, PresetManager, RecipeManager)
    5. PortalServerWeb  (WcfServicePreset, WcfServiceRecipe, ClassDataGateServer)

  NuGet 종속:
    Newtonsoft.Json (JSON 직렬화)

================================================================================
  끝
================================================================================

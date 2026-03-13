# 신형 디맨드 제어 시스템 (DemandNew)

> Autobase48 BEMS 전력 수요관리 엔진
> 최종 업데이트: 2026-03-04

---

## 1. 개요

신형 디맨드 제어 시스템은 건물 에너지 관리(BEMS)를 위한 **전력 수요 예측·제어 엔진**이다.
기존 Demand Control(레거시)의 한계를 극복하고, 다중 블록·다단계 부하 차단·EWMA 예측·시간대별 목표 등을 지원한다.

### 핵심 기능

| 기능 | 설명 |
|------|------|
| 다중 엔진 | 블록(Block)별 독립 엔진, 동시 다수 운영 |
| 유연한 계측 | DirectKW / DeltaKWH / Pulse 3종 입력 + 스파이크 검출 |
| 시간대 요금제 | TariffZone별 시간대 목표(kW) 설정 |
| 다단계 차단 | Priority 기반 단계적 부하 Shed/Restore |
| EWMA 예측 | 지수가중이동평균 트렌드 예측 + 신뢰도 |
| 실시간 차트 | GDI+ 기반 수요곡선·목표선·상태바 시각화 |
| 이력 저장 | PostgreSQL/ODBC DB 로깅 + JSON 백업 + CSV 내보내기 |
| Studio 편집 | 설계 시 블록 CRUD + 그래픽 오브젝트 프로퍼티 |
| 다국어 | 한국어·영어·일본어·중국어·베트남어·러시아어 |

---

## 2. 아키텍처

```
┌─────────────────────────────────────────────────────┐
│                    AutoLibLocal                      │
│  DemandNewEnums  DemandNewConfig  DemandSnapshot     │
│  DemandSnapshotBus  DemandNewConfigLoader             │
└──────────────────────┬──────────────────────────────┘
                       │ 참조
┌──────────────────────▼──────────────────────────────┐
│                     LocalMain                        │
│                                                      │
│  ┌─────────────────────────────────────────────┐    │
│  │          DemandNewEngineManager              │    │
│  │   ┌───────────────────────────────────┐     │    │
│  │   │        DemandNewEngine            │     │    │
│  │   │                                   │     │    │
│  │   │  IMeasurementSource ──► Normalizer │     │    │
│  │   │         │                         │     │    │
│  │   │    DemandWindow (구간 계산)        │     │    │
│  │   │         │                         │     │    │
│  │   │    DemandForecaster (EWMA 예측)   │     │    │
│  │   │         │                         │     │    │
│  │   │    ITargetProvider (목표 결정)     │     │    │
│  │   │         │                         │     │    │
│  │   │    IPolicyEngine (제어 판단)       │     │    │
│  │   │    └─ LoadGroupManager            │     │    │
│  │   │         │                         │     │    │
│  │   │    IActuator (부하 제어 실행)      │     │    │
│  │   │         │                         │     │    │
│  │   │    DemandHistorian (DB/백업)       │     │    │
│  │   │         │                         │     │    │
│  │   │    DemandSnapshotBus ─────────────┼──┐  │    │
│  │   └───────────────────────────────────┘  │  │    │
│  └──────────────────────────────────────────┘  │    │
│                                                │    │
│  FormDemandNewSettings (런타임 설정 UI)         │    │
└────────────────────────────────────────────────┼────┘
                                                 │
┌────────────────────────────────────────────────▼────┐
│                   GraphicModule                      │
│                                                      │
│  ObjectDemandChart ──► FormDemandNewChart (실시간)    │
│  ObjectArgsDemandChart (색상·레이아웃)                │
└─────────────────────────────────────────────────────┘
                       ▲ 참조
┌──────────────────────┴──────────────────────────────┐
│                      Studio                          │
│                                                      │
│  FormConfigDemandNew (블록 CRUD)                     │
│  FormConfigDemandNewAdd (블록 설정 다이얼로그)        │
│  PropertyPageObjectDemandChart (그래픽 프로퍼티)      │
│  ClassEditInsert.EditInsertDemandChart()              │
└─────────────────────────────────────────────────────┘
```

---

## 3. 프로젝트별 파일 목록

### 3.1 AutoLibLocal — 공유 데이터 모델 (5개)

| 파일 | 설명 |
|------|------|
| `DemandNew/DemandNewEnums.cs` | EngineMode, MeterType, MeasurementQuality, ControlDecision, TargetReason, ActuationResult, TariffZone |
| `DemandNew/DemandNewConfig.cs` | 블록 설정 클래스 + TimeZoneTarget, DrTarget, StepConfig, LoadModel |
| `DemandNew/DemandSnapshot.cs` | 런타임 상태 스냅샷 + LoadStatus |
| `DemandNew/DemandSnapshotBus.cs` | 이벤트 버스 (SnapshotPublished / IntervalCompleted) |
| `DemandNew/DemandNewConfigLoader.cs` | lstx 파일 읽기/쓰기 (CommaBlockString 직렬화) |

### 3.2 LocalMain — 엔진 런타임 (20개)

| 하위폴더 | 파일 | 설명 |
|----------|------|------|
| **Engine/** | `DemandNewEngine.cs` | 핵심 엔진 (초단위 실행 루프) |
| | `DemandNewEngineManager.cs` | 다중 엔진 관리 + GetBus(blockId) |
| | `CheckEngineDemandNew.cs` | LocalMain 기동 시 초기화 진입점 |
| **Metering/** | `IMeasurementSource.cs` | 계측 인터페이스 (MeasurementReading) |
| | `TagMeasurementSource.cs` | 태그 기반 kW 읽기 (TagLib.GetStructAI) |
| | `MeasurementNormalizer.cs` | 단위 변환 + 스파이크 검출 |
| **Calculator/** | `DemandWindow.cs` | 슬라이딩 윈도우 (구간 전력·에너지 계산) |
| **Forecast/** | `DemandForecaster.cs` | EWMA 예측 + 트렌드 분석 + 신뢰도 |
| **Target/** | `ITargetProvider.cs` | 목표 인터페이스 (TargetResult) |
| | `ContractTargetProvider.cs` | 계약전력·시간대·DR·수동 목표 결정 |
| **Policy/** | `IPolicyEngine.cs` | 정책 인터페이스 (PolicyResult) |
| | `PolicyEngine.cs` | Shed/Restore/Hold 판단 + 보호시간 |
| | `LoadGroupManager.cs` | 부하 그룹 관리 + 우선순위 정렬 |
| **Actuation/** | `IActuator.cs` | 제어 실행 인터페이스 |
| | `TagActuator.cs` | DO 태그 출력 + 피드백 검증 |
| **Historian/** | `DemandHistorian.cs` | DB 로깅 (demand_interval, demand_control_event) |
| | `DemandBackupManager.cs` | JSON 파일 백업 (Data/Backup/) |
| | `DemandCsvExporter.cs` | CSV 내보내기 유틸리티 |
| **Config/** | `DemandNewConfigValidator.cs` | 설정값 유효성 검증 |
| **Form/** | `FormDemandNewSettings.cs` | 런타임 설정 다이얼로그 (7탭) |

### 3.3 Studio — 설계도구 (3개 신규 + 기존 수정)

| 파일 | 설명 |
|------|------|
| `FormConfigDemandNew.cs` | 블록 목록 CRUD (Config 메뉴) |
| `FormConfigDemandNewAdd.cs` | 블록 추가/수정 다이얼로그 |
| `Property/PropertyPageObjectDemandChart.cs` | DemandChart 그래픽 오브젝트 프로퍼티 페이지 |

**기존 파일 수정:**

| 파일 | 수정 내용 |
|------|----------|
| `StudioMain.cs` | Config 메뉴에 "신형 디맨드 제어" 항목 추가 |
| `ClassEditInsert.cs` | `EditInsertDemandChart()` 메서드 추가 |
| `FormEditGraphicFrame.cs` | 그래프/트렌드 메뉴에 "Demand Chart" 항목 추가 |
| `Property/ClassEditProperty.cs` | ObjectDemandChart → PropertyPageObjectDemandChart 연결 |

### 3.4 GraphicModule — 시각화 (3개 신규 + 기존 수정)

| 파일 | 설명 |
|------|------|
| `FormDemandNewChart.cs` | 실시간 차트 폼 (GDI+ 렌더링) |
| `Object/ObjectDemandChart.cs` | 그래픽 오브젝트 (ObjectExpand 상속) |
| `Object/ObjectArgsDemandChart.cs` | 오브젝트 인자 (색상 15개, 레이아웃) |

**기존 파일 수정:**

| 파일 | 수정 내용 |
|------|----------|
| `Object/ObjectType.cs` | EnumObjectType.DemandChart 추가 |
| `Object/ObjectGroupLoadModX.cs` | LoadDemandChart() 디스패치 + 로드 함수 |

---

## 4. 설정 파일 형식

### 4.1 DemandNew.lstx (메인 설정)

경로: `{프로젝트}/FUNCTION/DemandNew.lstx`

```
BlockId,Title,Mode,IntervalMin,ClockAligned,
ContractKW,SafetyFactor,
MeterTag,MeterType,PulseRatio,TargetTag,UseTarget,UseEOI,EOITag,AutoReset,PredictionTag,
ShedMargin,RestoreMargin,ProtectionTime,MultiStep,StepCount,QualityBadBlock,
TrendWindow,EwmaAlpha,
DbSave,DbDsn,UsePostgres,Failover,
PercentY,
TimeZoneCount,[Zone,StartMin,EndMin,TargetKW]...
```

CommaBlockString 직렬화 형식. 한 줄 = 한 블록.

### 4.2 DemandNewLoads_{BlockId}.lstx (부하 목록)

경로: `{프로젝트}/FUNCTION/DemandNewLoads_{BlockId}.lstx`

```
LoadId,DisplayName,Priority,Group,EstimatedKW,
CommandTag,CommandTagType,FeedbackTag,
MinOffTime,MinOnTime,ReShedBlock,ReShedBlockTime,InterlockTag
```

한 줄 = 한 부하.

---

## 5. 데이터 모델

### 5.1 DemandNewConfig (설정)

```csharp
public class DemandNewConfig
{
    // 일반
    string BlockId;             // 블록 고유 ID
    string Title;               // 표시명
    EngineMode Mode;            // Shadow(모니터링) / Active(실제 제어)
    int IntervalMinutes;        // 수요구간 (5/10/15/30/60분)
    bool ClockAligned;          // 정시 정렬 여부

    // 계약
    double ContractKW;          // 계약전력 (kW)
    double SafetyFactor;        // 안전계수 (0.5~1.0)
    List<TimeZoneTarget> TimeZoneTargets;  // 시간대별 목표

    // 계측
    string MeterTagName;        // 전력 측정 태그
    MeterType MeterType;        // DirectKW / DeltaKWH / Pulse
    double PulseRatio;          // 펄스 환산비

    // 정책
    double ShedMarginKW;        // 차단 여유 kW
    double RestoreMarginKW;     // 복귀 여유 kW
    int ProtectionTimeSec;      // 보호시간 (초)
    bool MultiStepEnabled;      // 다단계 제어
    int StepCount;              // 단계 수

    // 예측
    int TrendWindowSec;         // 트렌드 윈도우 (30~300초)
    double EwmaAlpha;           // EWMA 계수 (0.05~0.5)

    // 저장
    bool DatabaseSaveEnabled;   // DB 저장 활성화
    string DatabaseDsn;         // DSN 이름
    bool UsePostgres;           // PostgreSQL 사용
    bool FailoverEnabled;       // 장애복구 활성화

    // 부하 목록
    List<LoadModel> Loads;
}
```

### 5.2 DemandSnapshot (런타임 스냅샷)

```csharp
public class DemandSnapshot
{
    string BlockId;
    DateTime Timestamp;
    EngineMode Mode;

    // 계측
    double CurrentKW;
    MeasurementQuality Quality;

    // 구간
    int IntervalMinutes;
    int ElapsedSeconds, RemainingSeconds;
    DateTime IntervalStart;
    double BlockDemandKW;       // 구간 평균전력
    double BlockEnergyKWH;      // 구간 에너지

    // 예측
    double ForecastDemandEndKW;
    double Confidence;

    // 목표
    double TargetKW, EffectiveTargetKW;
    TargetReason TargetReason;

    // 정책
    ControlDecision LastDecision;
    int CurrentStep;
    string PolicyReason;

    // 부하 상태
    List<LoadStatus> Loads;

    // 차트용
    double[] DemandCurve;       // 초단위 kW 배열
    bool IsIntervalEnd;

    // 산출값
    double MarginKW => EffectiveTargetKW - ForecastDemandEndKW;
}
```

### 5.3 LoadModel (부하 설정)

```csharp
public class LoadModel
{
    string LoadId;              // 부하 고유 ID
    string DisplayName;         // 표시명
    int Priority;               // 우선순위 (1=최우선)
    string Group;               // 그룹명
    double EstimatedKW;         // 추정 소비전력
    string CommandTagName;      // 출력 DO 태그
    sbyte CommandTagType;       // 태그 타입
    string FeedbackTagName;     // 피드백 태그
    int MinOffTimeSec;          // 최소 차단시간
    int MinOnTimeSec;           // 최소 복귀시간
    bool ReShedBlockEnabled;    // 재차단 방지
    int ReShedBlockTimeSec;     // 재차단 방지시간
    string InterlockTagName;    // 인터록 태그
}
```

---

## 6. 엔진 실행 흐름

```
CheckEngineDemandNew.Initialize()
    │
    ├── DemandNewConfigLoader.LoadAll()       ← lstx 파일 로드
    ├── DemandNewEngineManager 생성
    │   └── 블록마다 DemandNewEngine 생성
    │       ├── TagMeasurementSource          ← 태그 바인딩
    │       ├── MeasurementNormalizer         ← 단위 변환기
    │       ├── DemandWindow                  ← 구간 계산기
    │       ├── DemandForecaster              ← EWMA 예측기
    │       ├── ContractTargetProvider        ← 목표 공급자
    │       ├── PolicyEngine + LoadGroupManager ← 제어 정책
    │       ├── TagActuator                   ← DO 출력기
    │       └── DemandHistorian               ← DB/백업
    │
    └── ObjectDemandChart.GetBusCallback 등록
        (GraphicModule → LocalMain 역참조 콜백)
```

### 매 초 실행 사이클

```
1. 계측  │ TagMeasurementSource.Read()
         │ MeasurementNormalizer.NormalizeToKW()
         │
2. 누적  │ DemandWindow.SampleAdd(kW)
         │ → BlockDemandKW, BlockEnergyKWH 갱신
         │
3. 예측  │ DemandForecaster.Update(kW)
         │ → ForecastDemandEndKW, Confidence
         │
4. 목표  │ ContractTargetProvider.GetCurrentTarget(now)
         │ → TargetKW × SafetyFactor = EffectiveTargetKW
         │
5. 판단  │ PolicyEngine.Evaluate(snapshot, target)
         │ → Shed / Restore / Hold + 대상 LoadIds
         │
6. 실행  │ TagActuator.ExecuteAsync(loadId, decision)
         │ → DO 출력 + 피드백 검증
         │
7. 기록  │ DemandHistorian.LogInterval(snapshot)
         │ → DB + JSON 백업
         │
8. 배포  │ DemandSnapshotBus.Publish(snapshot)
         │ → FormDemandNewChart 실시간 갱신
```

---

## 7. 제어 정책 로직

### 판단 기준

```
Forecast = ForecastDemandEndKW (구간 말 예측 전력)
Target   = EffectiveTargetKW   (계약전력 × 안전계수)

if Quality == Bad && QualityBadBlockControl:
    → Hold (계측 불량 시 제어 중지)

if Forecast > Target + ShedMargin:
    → Shed (차단 - 예측이 목표+여유 초과)

if Forecast < Target - RestoreMargin:
    → Restore (복귀 - 예측이 목표-여유 미만)

else:
    → Hold (유지)
```

### 다단계 차단

```
Step 1: Priority 1~3 부하 차단 (소형)
Step 2: Priority 4~6 부하 추가 차단 (중형)
Step 3: Priority 7~  부하 추가 차단 (대형)

복귀: 역순 (높은 Priority부터)
```

### 보호 타이머

- `ProtectionTimeSec`: 차단/복귀 후 최소 대기시간
- `MinOffTimeSec`: 부하별 최소 차단 유지시간
- `MinOnTimeSec`: 부하별 최소 가동 유지시간
- `ReShedBlockTimeSec`: 재차단 방지시간

---

## 8. 그래픽 오브젝트 (ObjectDemandChart)

### 8.1 저장 형식 (ObjectSave)

```
Font                        ← ObjectSaveFont
BackColor                   ← SaveObjectItem.BackColor
TextColor                   ← SaveObjectItem.TextColor
FillColor                   ← SaveObjectItem.FillColor
GuideLineColor              ← SaveObjectItem.GuideLineColor
StringOption:
    demand_block_id          (string)
    thick_target             (int, 목표선 두께)
    nStatusBarPos            (int, 0=상단/1=좌측/2=숨김)
    nPercentY                (int, Y축 퍼센트, 기본120)
    lColorPrediction  RGBA   (예측선 색상)
    lColorExcess      RGBA   (초과 색상)
    lColorTarget      RGBA   (목표선 색상)
    lColorStatusFill  RGBA   (상태바 채움)
    lColorStatusBack  RGBA   (상태바 배경)
    lColorStatusValue RGBA   (상태바 값)
    lColorTargetValue RGBA   (목표값 색상)
    lColorPreValue    RGBA   (예측값 색상)
    lColorExValue     RGBA   (초과값 색상)
```

### 8.2 색상 목록 (15개)

| 색상 | 용도 | 기본값 |
|------|------|--------|
| lColorBack | 차트 배경 | White |
| lColorFill | 채움 | LightGray |
| lColorText | 텍스트 | Black |
| lColorGuideLine | 가이드라인 | LightGray |
| lColorPrediction | 예측선 | Blue |
| lColorExcess | 초과 영역 | Red |
| lColorTarget | 목표선 | LightGreen |
| lColorStatusFill | 상태바 채움 | DodgerBlue |
| lColorStatusBack | 상태바 배경 | DarkGray |
| lColorStatusValue | 현재값 텍스트 | White |
| lColorTargetValue | 목표값 텍스트 | LimeGreen |
| lColorPreValue | 예측값 텍스트 | DeepSkyBlue |
| lColorExValue | 초과값 텍스트 | OrangeRed |

### 8.3 LocalMain ↔ GraphicModule 연결

GraphicModule은 LocalMain을 참조할 수 없으므로 **정적 콜백 패턴** 사용:

```csharp
// ObjectDemandChart.cs (GraphicModule)
public static Func<string, DemandSnapshotBus> GetBusCallback;

// CheckEngineDemandNew.cs (LocalMain) - 초기화 시 등록
ObjectDemandChart.GetBusCallback = (blockId) =>
    DemandNewEngineManager.Instance.GetBus(blockId);
```

---

## 9. 메뉴 구조

### LocalMain

```
Config
  └─ Demand Control (New)  →  FormDemandNewSettings
       신형 디맨드 제어(&N)

View
  └─ Demand Controls        →  FormDemandControl (레거시)
```

매크로 명령: `CONFIG_DEMAND_NEW`

### Studio

```
Config
  ├─ Demand Control          →  FormConfigDemandControl (레거시)
  └─ Demand Control (New)    →  FormConfigDemandNew
       신형 디맨드 제어(&N)

Insert > Graph & Trend
  └─ Demand Chart            →  ClassEditInsert.EditInsertDemandChart()
```

### 다국어 메뉴 텍스트

| 언어 | 텍스트 |
|------|--------|
| English | `Demand Control (&New)` |
| 한국어 | `신형 디맨드 제어(&N)` |
| 日本語 | `デマンド制御(新型)(&N)` |
| 中文 | `新型需求控制(&N)` |
| Tiếng Việt | `Điều khiển nhu cầu (Mới)(&N)` |
| Русский | `Управление спросом (Новое)(&N)` |

---

## 10. DB 테이블 구조

### demand_interval (구간 이력)

| 컬럼 | 타입 | 설명 |
|------|------|------|
| block_id | VARCHAR | 블록 ID |
| interval_start | TIMESTAMP | 구간 시작 시각 |
| interval_end | TIMESTAMP | 구간 종료 시각 |
| interval_minutes | INT | 구간 길이 (분) |
| demand_kw | DOUBLE | 구간 평균전력 (kW) |
| energy_kwh | DOUBLE | 구간 에너지 (kWh) |
| forecast_kw | DOUBLE | 예측 전력 |
| target_kw | DOUBLE | 목표 전력 |
| effective_target_kw | DOUBLE | 실효 목표 (×안전계수) |
| decision | INT | 최종 판단 |
| engine_mode | INT | 엔진 모드 |

### demand_control_event (제어 이벤트)

| 컬럼 | 타입 | 설명 |
|------|------|------|
| block_id | VARCHAR | 블록 ID |
| event_time | TIMESTAMP | 이벤트 시각 |
| decision | INT | Shed/Restore |
| step | INT | 제어 단계 |
| load_id | VARCHAR | 부하 ID |
| load_name | VARCHAR | 부하명 |
| estimated_kw | DOUBLE | 추정 소비전력 |
| forecast_kw | DOUBLE | 예측 전력 |
| target_kw | DOUBLE | 목표 전력 |
| result | INT | 실행 결과 |
| engine_mode | INT | 엔진 모드 |

---

## 11. csproj 등록 현황

### AutoLibLocal.csproj

```xml
<Compile Include="DemandNew\DemandNewEnums.cs" />
<Compile Include="DemandNew\DemandNewConfig.cs" />
<Compile Include="DemandNew\DemandSnapshot.cs" />
<Compile Include="DemandNew\DemandSnapshotBus.cs" />
<Compile Include="DemandNew\DemandNewConfigLoader.cs" />
```

### LocalMain.csproj

```xml
<Compile Include="DemandNew\Metering\IMeasurementSource.cs" />
<Compile Include="DemandNew\Metering\TagMeasurementSource.cs" />
<Compile Include="DemandNew\Metering\MeasurementNormalizer.cs" />
<Compile Include="DemandNew\Calculator\DemandWindow.cs" />
<Compile Include="DemandNew\Forecast\DemandForecaster.cs" />
<Compile Include="DemandNew\Target\ITargetProvider.cs" />
<Compile Include="DemandNew\Target\ContractTargetProvider.cs" />
<Compile Include="DemandNew\Policy\IPolicyEngine.cs" />
<Compile Include="DemandNew\Policy\PolicyEngine.cs" />
<Compile Include="DemandNew\Policy\LoadGroupManager.cs" />
<Compile Include="DemandNew\Actuation\IActuator.cs" />
<Compile Include="DemandNew\Actuation\TagActuator.cs" />
<Compile Include="DemandNew\Engine\DemandNewEngine.cs" />
<Compile Include="DemandNew\Engine\DemandNewEngineManager.cs" />
<Compile Include="DemandNew\Engine\CheckEngineDemandNew.cs" />
<Compile Include="DemandNew\Config\DemandNewConfigValidator.cs" />
<Compile Include="DemandNew\Historian\DemandHistorian.cs" />
<Compile Include="DemandNew\Historian\DemandBackupManager.cs" />
<Compile Include="DemandNew\Historian\DemandCsvExporter.cs" />
<Compile Include="DemandNew\Form\FormDemandNewSettings.cs" />
```

### Studio.csproj

```xml
<Compile Include="FormConfigDemandNew.cs" />
<Compile Include="FormConfigDemandNewAdd.cs" />
<Compile Include="Property\PropertyPageObjectDemandChart.cs" />
```

### GraphicModule.csproj

```xml
<Compile Include="FormDemandNewChart.cs" />
<Compile Include="Object\ObjectArgsDemandChart.cs" />
<Compile Include="Object\ObjectDemandChart.cs" />
```

---

## 12. 레거시 비교

| 항목 | 레거시 (Demand) | 신형 (DemandNew) |
|------|----------------|-----------------|
| 설정 파일 | `Demand.lstx` | `DemandNew.lstx` + `DemandNewLoads_*.lstx` |
| 설정 위치 | `FUNCTION/` | `FUNCTION/` |
| 엔진 클래스 | `CheckEngineDemandControl` | `CheckEngineDemandNew` |
| Studio 폼 | `FormConfigDemandControl` | `FormConfigDemandNew` |
| 런타임 뷰 | `FormDemandControl` (MDI) | `FormDemandNewChart` (ObjectDemandChart 내장) |
| 계측 | 단일 태그 | 3종 (Direct/Delta/Pulse) + 스파이크 검출 |
| 예측 | 없음 | EWMA + 트렌드 윈도우 |
| 다단계 | 없음 | Priority 기반 다단계 |
| 시간대 | 없음 | TariffZone별 목표 |
| DB 저장 | 없음 | PostgreSQL/ODBC + JSON 백업 |
| 그래픽 | `ObjectDemandWindow` | `ObjectDemandChart` |

---

## 13. 인코딩 주의사항

- 모든 신규 파일: **UTF-8 with BOM** (`utf-8-sig`)
- 기존 파일 수정 시: EUC-KR(CP949) → UTF-8 BOM 변환 필요
- resx XML에서 `&` 문자는 반드시 `&amp;`로 이스케이프
  - 예: `신형 디맨드 제어(&amp;N)` ← `(&N)` 안 됨
- ObjectGroupLoadModX.cs, ObjectType.cs, ClassEditInsert.cs: 이번 작업에서 UTF-8 BOM으로 변환 완료

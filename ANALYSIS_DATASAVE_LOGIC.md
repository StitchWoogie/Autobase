# LocalMain DataSave 로직 분석

## 1. 전체 아키텍처 개요

```
[매초 스캔] → [1분 누적 계산] → [Remain 버퍼] → [TrendSaveManager 큐] → [PostgreSQL DB]
                                                                      ↘ [JSON 백업 (장애시)]
```

### 핵심 파일

| 파일 | 역할 |
|------|------|
| `LocalMain/CheckEngineMinuteChanged.cs` | 분/시간 자료 계산 및 메모리 저장 |
| `LocalMain/DataSave/TrendSaveManager.cs` | 큐 기반 비동기 DB 저장 매니저 |
| `LocalMain/DataSave/TrendBackupManager.cs` | DB 장애시 JSON 백업/복구 |
| `Dll/AutoLibLocal/DataLocal.cs` | 데이터 구조체 정의 |
| `Dll/AutoLibLocal/PostgresSQL/DataPostgres.cs` | PostgreSQL 저장 쿼리 |

---

## 2. 데이터 구조체

### TREND_AI_STRUCT (분 아날로그 데이터)
```csharp
public class TREND_AI_STRUCT
{
    public float fSumMin;   // 1분 동안의 적산치 (유량 등)
    public float fAverage;  // 1분 동안의 평균값
    public float fMin;      // 1분 동안의 최소값
    public float fMax;      // 1분 동안의 최대값
    public float fCurr;     // 저장 당시 순시값(현재값)
}
```

### TREND_DI_STRUCT (분 디지털 데이터)
```csharp
public class TREND_DI_STRUCT
{
    public short nCountOnOff;  // 1분 동안 ON/OFF 전환 횟수
    public byte bOnOff;        // 현재 ON/OFF 상태
    public byte cOnTime;       // 1분 동안 ON 된 시간 (초)
}
```

### HOUR_DATA_ANALOG_STRUCT (시간 아날로그 데이터)
```csharp
public class HOUR_DATA_ANALOG_STRUCT
{
    public float fSumHour;      // 1시간 적산치
    public float fAveHour;      // 1시간 평균치
    public float fMinHour;      // 1시간 최소치
    public float fMaxHour;      // 1시간 최대치
    public float fCurrSumMeter; // 누적 적산 계량기 눈금
}
```

### HOUR_DATA_DIGITAL_STRUCT (시간 디지털 데이터)
```csharp
public class HOUR_DATA_DIGITAL_STRUCT
{
    public ushort wCountOnOff;  // 1시간 ON/OFF 전환 횟수 합계
    public uint dwOnTime;       // 1시간 ON 시간 합계 (초)
    public byte flag;           // 데이터 유효 플래그
}
```

---

## 3. 분자료 계산 로직 (SaveMinTrendAI)

### 3.1 호출 흐름
```
FormMain 타이머 (1분마다)
  → WorkOnBeforeChangeMin(DateTime t)
    → SaveMinTrendAI(t, save_flag, ...)   // AI 분 데이터 계산
    → SaveMinTrendDI(t, save_flag, ...)   // DI 분 데이터 계산
```

### 3.2 저장 조건 판단
```csharp
// save_flag 결정 로직:
if (ConfigData.bDataSaveAll)                         // 전체 저장 활성화?
  if (테스트모드 && 테스트모드저장 비활성)            // 테스트모드 체크
    save_flag = false;
  else if (count >= ConfigData.nDataSaveDelay)        // 지연시간 경과?
    save_flag = true;
  else
    save_flag = false; count++;                       // 통신 안정화 대기
```

### 3.3 AI 평균값 (fAverage) 계산

**일반 태그:**
```
fAverage = fMinHap / nScanCount
```
- `fMinHap`: 매 스캔마다 `ai.curr` 값을 누적한 합계
- `nScanCount`: 1분 동안의 스캔 횟수
- 분이 바뀌면 `fMinHap = ai.curr`, `nScanCount = 1`로 리셋

**역률(Power Factor) 태그:**
```
fAverage = fMinHap / nScanCount    (fMinHap은 Math.Abs(curr)로 누적)
if (ai.curr < 0) fAverage *= -1;  // 부호 복원
```

### 3.4 적산치 (fSumMin) 계산

**일반 태그 (비누적):**
```
fSumMin = fAverage / 60.0
```
- 1분 평균을 60으로 나눈 값 = 1분 동안의 적산 기여분

**누적값 태그 (bAccumulatedValue == 1):** (전력량계 등)
```
정상 케이스:
  fSumMin = fMinMax - fMinMin         // 최대값 - 최소값

리셋 감지 케이스 (fMinMax > curr 또는 fMinInitial > fMinMin):
  fSumMin = fMinMax - fMinInitial + curr  // 최대 - 초기 + 현재값
```
- `fMinMax`: 1분 동안 관측된 최대값 (누적 카운터의 최대 도달점)
- `fMinMin`: 1분 동안 관측된 최소값
- `fMinInitial`: 분 시작 시점의 값
- `bHasSavedOnce`: 최초 저장 전 리셋 오감지 방지 플래그 (20250717 추가)

### 3.5 최소/최대값 (fMin, fMax)
```
fMin = ai.fMinMin   // 1분 동안 관측된 최소값
fMax = ai.fMinMax   // 1분 동안 관측된 최대값
```
- 매 스캔마다 `if (curr < fMinMin) fMinMin = curr` 방식으로 갱신
- 분이 바뀌면 `fMinMin = curr`, `fMinMax = curr`로 리셋

### 3.6 순시값 (fCurr)
```
fCurr = (float)ai.curr   // 분 데이터 저장 시점의 현재값
```

### 3.7 누적 적산치 (fSumTotal, fSumPart)
```csharp
// 소수점 정밀도 문제 방지를 위해 정수 연산 사용
long_sum   = (long)(fSumMin * 1000)
long_total = (long)(fSumTotal * 1000)
long_part  = (long)(fSumPart * 1000)

long_total += long_sum
long_part  += long_sum

// 오버플로 방지: 10억(1,000,000,000,000) 초과시 리셋
if (long_total >= LIMIT_SUM_VALUE) long_total = 0
if (long_part  >= LIMIT_SUM_VALUE) long_part  = 0

fSumTotal = long_total / 1000.0
fSumPart  = long_part  / 1000.0
```

---

## 4. DI 분자료 계산 로직 (SaveMinTrendDI)

### 4.1 ON/OFF 횟수
```
trend.nCountOnOff = di.count_on_off  // 1분 동안 ON 전환 횟수
```

### 4.2 ON 시간 계산
```csharp
if (di.curr == 1)  // 현재 ON 상태
    cOnTime = (byte)((60 - di.startOnSec) + di.cOnTime / 1000);
    // 60초 - 마지막 ON 시작시간 + 이전 누적 ON시간
else               // 현재 OFF 상태
    cOnTime = (byte)(di.cOnTime / 1000);
    // 누적된 ON 시간만 반영
```

### 4.3 리셋
```
di.count_on_off = 0;   // ON/OFF 카운트 리셋
di.cOnTime = 0;        // ON 시간 리셋
di.startOnSec = 0;     // ON 시작 시각 리셋
```

---

## 5. 시간자료 계산 로직

### 5.1 트리거 조건
59분 데이터가 저장 완료되면 `TrendSaveManager.CheckAndPrepareHourData()`에서 감지:
```csharp
if (item.dataTime.Minute == 59)
{
    string hourKey = $"{tagName}_{dataTime:yyyyMMddHH}";
    // 중복 방지 후 hourProcessQueue에 추가
}
```

### 5.2 AI 시간 데이터 (SaveHourTrendAIAsync)

DB에서 해당 시간의 60개 분 데이터를 로드하여 집계:

```
fAve (시간평균) = Σ(분별 fAverage) / 분데이터 개수
fHap (시간적산) = Σ(분별 fSumMin)
fMin (시간최소) = MIN(분별 fMin)
fMax (시간최대) = MAX(분별 fMax)
```

**역률 태그 특수 처리:**
- 절대값 기준으로 min/max 비교: `Math.Abs(trend.fMin) < Math.Abs(fMin)`
- 합계도 절대값: `fHap += Math.Abs(trend.fSumMin)`
- 평균 부호 결정: 음수 평균 분 데이터가 과반수이면 `fAve *= -1`

**저장 구조:**
```
fSumHour      = fHap                    // 시간 적산 합계
fAveHour      = fAve                    // 시간 평균
fMinHour      = fMin                    // 시간 최소
fMaxHour      = fMax                    // 시간 최대
fCurrSumMeter = (float)ai.fSumTotal     // 현재 누적 적산 계량기 눈금
```

### 5.3 DI 시간 데이터 (SaveHourTrendDIAsync)

DB에서 60개 분 데이터를 각각 로드하여 집계:

```
count_on_off = Σ(분별 nCountOnOff)   // 1시간 ON/OFF 횟수 합계
on_time      = Σ(분별 cOnTime)       // 1시간 ON 시간 합계(초)
```

---

## 6. 데이터 저장 파이프라인

### 6.1 단계별 흐름

```
[1단계] 분 계산 (SaveMinTrendAI/DI)
   ↓ 계산된 TREND_*_STRUCT를 remain 버퍼에 저장
   ↓ ai.remain[ai.nTrendRemainCount] = {t, trend}

[2단계] 큐 전송 (StatusRemainTrendSave)
   ↓ FormMain 타이머에서 반복 호출
   ↓ Round-robin으로 nSaveCountAtOnce개씩 처리
   ↓ remain 버퍼 → TrendSaveManager 큐로 이동

[3단계] 비동기 DB 저장 (TrendSaveManager.SaveThreadLoop)
   ↓ 백그라운드 스레드에서 1초 간격으로 처리
   ↓ 우선순위 큐 → 일반 큐 순서로 배치 저장
   ↓ PostgreSQL operational.minute_analog_data / minute_digital_data

[4단계] 시간 집계 (59분 데이터 감지시)
   ↓ DB에서 60개 분 데이터 로드
   ↓ 평균/최소/최대/적산 계산
   ↓ PostgreSQL operational.hour_analog_data / hour_digital_data
```

### 6.2 우선순위 처리
트렌드 화면에서 특정 태그를 조회할 때:
- `SaveTrendRemainAI()` / `SaveTrendRemainDI()` 호출
- 해당 태그의 remain 데이터를 **우선순위 큐**(`_aiPriorityQueue`)에 추가
- 일반 큐보다 먼저 처리되어 화면에 즉시 반영

### 6.3 과부하 제어
```csharp
// remain 버퍼 초과시
if (ai.nTrendRemainCount >= ConfigData.nDataSaveTrendRemain)
{
    if (ConfigData.bSkipDataWhenSavingDelayed)
        continue;  // 데이터 건너뜀 (스킵 모드)
    // else: 계속 저장 시도 (지연 모드)
}
```

### 6.4 장애 복구
```
DB 연결 끊김 → 데이터를 JSON 파일로 백업
                경로: ConfigData.sDirData/Backup/{ProjectName}/TrendData/
DB 연결 복구 → OnDbStateChanged 이벤트 → AutoRecoverFromBackupsAsync()
프로그램 종료 → 잔여 큐 데이터 백업 (ShutdownAsync)
```

---

## 7. 계산 값 요약 표

| 항목 | 분 자료 필드 | 계산 방법 | 시간 자료 필드 | 집계 방법 |
|------|-------------|-----------|---------------|-----------|
| **평균** | fAverage | fMinHap / nScanCount | fAveHour | Σ(분 fAverage) / 60 |
| **최소** | fMin | 매 스캔 min 갱신 | fMinHour | MIN(분 fMin) |
| **최대** | fMax | 매 스캔 max 갱신 | fMaxHour | MAX(분 fMax) |
| **적산** | fSumMin | 일반: fAverage/60, 누적: fMax-fMin | fSumHour | Σ(분 fSumMin) |
| **순시값** | fCurr | 저장 시점 ai.curr | - | - |
| **누적 적산** | - | fSumTotal += fSumMin (오버플로 보호) | fCurrSumMeter | ai.fSumTotal |
| **ON횟수** | nCountOnOff | 1분간 ON 전환 카운트 | wCountOnOff | Σ(분 nCountOnOff) |
| **ON시간** | cOnTime | ON상태: 60-startSec+누적 | dwOnTime | Σ(분 cOnTime) |

---

## 8. 특이사항 및 주의점

1. **소수점 정밀도**: 적산 누적 시 `float→long(×1000)→long 덧셈→double(÷1000)` 방식으로 부동소수점 오차 방지
2. **오버플로 보호**: `LIMIT_SUM_VALUE = 1,000,000,000,000` 초과 시 적산치 0으로 리셋
3. **역률 태그**: `IsPowerFactorTag()` 여부에 따라 절대값 기반 계산, 부호 별도 처리
4. **누적값 리셋 감지**: 전력량계 등에서 카운터 리셋 시 `fMinMax - fMinInitial + curr`로 보정
5. **bHasSavedOnce 플래그**: 최초 저장 전 리셋 오감지 방지 (2025.07.17 추가)
6. **DB 타임아웃**: 저장 시 10초 타임아웃, 초과 시 JSON 백업으로 전환
7. **시간 데이터 중복 방지**: `_processedHourKeys` HashSet으로 동일 시간대 중복 처리 차단

# B1. 예측 정비 (Predictive Maintenance) - 상세 구현 설계

## 1. 개요

장비별 태그 데이터(진동, 온도, 전류 등) 이력을 분석하여 고장/열화 패턴을 학습하고,
잔여수명(RUL) 및 정비 시기를 사전 예측.

**핵심 가치:** 비계획 정지 50% 감소, 정비 비용 30% 절감, 장비 가동률 향상

---

## 2. 아키텍처

```
┌─────────────────────────────────────────────────────────────┐
│ LocalMain                                                   │
│                                                             │
│  TrendSaveManager                                           │
│       │ (이력 데이터)                                        │
│       ▼                                                     │
│  PredictiveMaintenanceBridge (신규)                           │
│  ├─ 장비 등록/관리                                           │
│  ├─ 주기적 건전성 평가 요청 (cron)                           │
│  ├─ RUL 결과 → 가상 태그 저장                                │
│  └─ 정비 일정 알림 → AlarmProcessor                          │
│                                                             │
└──────────────┬──────────────────────────────────────────────┘
               │ TCP
               ▼
┌─────────────────────────────────────────────────────────────┐
│ python_ai_engine                                             │
│                                                             │
│  pdm_service (Predictive Maintenance, 신규)                  │
│  ├─ pdm/health       건전성 지수 (HI) 계산                   │
│  ├─ pdm/rul          잔여수명 (RUL) 예측                     │
│  ├─ pdm/train        고장 패턴 학습                          │
│  ├─ pdm/equipment    장비 등록/조회                           │
│  └─ pdm/report       정비 보고서 생성                        │
│                                                             │
│  알고리즘:                                                   │
│  ├─ Tier-1: 통계적 열화 지표 (이동 RMS, 크레스트 팩터)      │
│  ├─ Tier-2: Random Forest / LSTM 기반 RUL                   │
│  └─ Tier-3: 생존 분석 (Weibull)                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. 장비 모델

### 3.1 Equipment 데이터 구조

```csharp
public class Equipment
{
    public string Id { get; set; }              // "EQ-001"
    public string Name { get; set; }            // "1번 냉각수 펌프"
    public string Type { get; set; }            // "Pump", "Motor", "Compressor"
    public DateTime InstalledDate { get; set; } // 설치일
    public int DesignLifeHours { get; set; }    // 설계 수명 (시간)
    public double RunningHours { get; set; }    // 누적 운전 시간

    // 감시 태그 매핑
    public List<MonitoredTag> MonitoredTags { get; set; }

    // 정비 이력
    public List<MaintenanceRecord> MaintenanceHistory { get; set; }
}

public class MonitoredTag
{
    public string TagName { get; set; }         // "Pump1_Vibration"
    public string Role { get; set; }            // "vibration", "temperature", "current", "pressure"
    public double NormalMin { get; set; }        // 정상 범위 하한
    public double NormalMax { get; set; }        // 정상 범위 상한
    public double WarningThreshold { get; set; } // 경고 임계값
    public double CriticalThreshold { get; set; }// 위험 임계값
}

public class MaintenanceRecord
{
    public DateTime Date { get; set; }
    public string Type { get; set; }            // "예방정비", "긴급정비", "교체"
    public string Description { get; set; }
    public string Technician { get; set; }
}
```

---

## 4. C# 측 구현

### 4.1 PredictiveMaintenanceBridge

**파일:** `LocalMain/PredictiveMaintenance/PredictiveMaintenanceBridge.cs`

```csharp
public class PredictiveMaintenanceBridge
{
    private List<Equipment> _equipments;
    private Timer _evaluationTimer;
    private int _evaluationIntervalMinutes = 60; // 1시간마다 평가

    public void Initialize(List<Equipment> equipments)
    {
        _equipments = equipments;

        // 주기적 건전성 평가 타이머
        _evaluationTimer = new Timer();
        _evaluationTimer.Interval = _evaluationIntervalMinutes * 60 * 1000;
        _evaluationTimer.Elapsed += async (s, e) => await EvaluateAllEquipments();
        _evaluationTimer.Start();
    }

    private async Task EvaluateAllEquipments()
    {
        foreach (var equipment in _equipments)
        {
            try
            {
                var result = await EvaluateEquipment(equipment);
                ProcessResult(equipment, result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"PDM evaluation failed for {equipment.Id}: {ex.Message}");
            }
        }
    }

    private async Task<PdmResult> EvaluateEquipment(Equipment equipment)
    {
        // 태그 이력 데이터 수집 (최근 24시간)
        var tagHistory = new JObject();
        foreach (var mt in equipment.MonitoredTags)
        {
            var history = await GetTagHistory(mt.TagName,
                DateTime.Now.AddHours(-24), DateTime.Now);
            tagHistory[mt.TagName] = new JObject
            {
                ["values"] = new JArray(history.Select(h => h.Value)),
                ["timestamps"] = new JArray(history.Select(h =>
                    h.Timestamp.ToString("o"))),
                ["role"] = mt.Role
            };
        }

        // 장비 정보
        var equipmentInfo = new JObject
        {
            ["id"] = equipment.Id,
            ["type"] = equipment.Type,
            ["running_hours"] = equipment.RunningHours,
            ["design_life_hours"] = equipment.DesignLifeHours,
            ["installed_date"] = equipment.InstalledDate.ToString("o"),
            ["maintenance_count"] = equipment.MaintenanceHistory.Count
        };

        var payload = new JObject
        {
            ["equipment"] = equipmentInfo,
            ["tag_history"] = tagHistory
        };

        // 건전성 지수 요청
        string hiResult = await PythonAiManager.CallAsync(
            "pdm/health", payload.ToString());

        // RUL 예측 요청
        string rulResult = await PythonAiManager.CallAsync(
            "pdm/rul", payload.ToString());

        var hiResponse = JObject.Parse(hiResult);
        var rulResponse = JObject.Parse(rulResult);

        return new PdmResult
        {
            EquipmentId = equipment.Id,
            HealthIndex = hiResponse["Result"]?["health_index"]?.Value<double>() ?? 1.0,
            HealthStatus = hiResponse["Result"]?["status"]?.ToString() ?? "unknown",
            RulHours = rulResponse["Result"]?["rul_hours"]?.Value<double>() ?? -1,
            RulConfidence = rulResponse["Result"]?["confidence"]?.Value<double>() ?? 0,
            RecommendedAction = rulResponse["Result"]?["recommendation"]?.ToString() ?? "",
            NextMaintenanceDate = ParseNullableDate(
                rulResponse["Result"]?["next_maintenance"]?.ToString()),
            Details = rulResponse["Result"]?["details"]?.ToString() ?? ""
        };
    }

    private void ProcessResult(Equipment equipment, PdmResult result)
    {
        // 1. 가상 태그에 건전성 지수 저장
        string hiTagName = $"PDM_{equipment.Id}_HealthIndex";
        SetVirtualTagValue(hiTagName, result.HealthIndex);

        string rulTagName = $"PDM_{equipment.Id}_RUL";
        if (result.RulHours >= 0)
            SetVirtualTagValue(rulTagName, result.RulHours);

        // 2. 건전성 상태에 따른 알람
        if (result.HealthIndex < 0.3) // Critical
        {
            AlarmProcessor.Instance.AddAlarm(new ALARM_FILE_STRUCT
            {
                tagName = hiTagName,
                message = $"[예측정비] {equipment.Name} 건전성 위험 " +
                         $"(HI={result.HealthIndex:P0}). {result.RecommendedAction}",
                priority = 1
            });
        }
        else if (result.HealthIndex < 0.6) // Warning
        {
            AlarmProcessor.Instance.AddAlarm(new ALARM_FILE_STRUCT
            {
                tagName = hiTagName,
                message = $"[예측정비] {equipment.Name} 건전성 주의 " +
                         $"(HI={result.HealthIndex:P0}). " +
                         $"예상 잔여수명: {result.RulHours:F0}시간",
                priority = 2
            });
        }

        // 3. 정비 일정 알림 (잔여수명 72시간 이내)
        if (result.RulHours >= 0 && result.RulHours < 72)
        {
            AlarmProcessor.Instance.AddAlarm(new ALARM_FILE_STRUCT
            {
                tagName = rulTagName,
                message = $"[정비예고] {equipment.Name} 정비 필요 " +
                         $"(잔여수명 약 {result.RulHours:F0}시간). " +
                         $"권장: {result.RecommendedAction}",
                priority = 1
            });
        }
    }

    private void SetVirtualTagValue(string tagName, double value)
    {
        // 가상 태그(Memory 타입)에 값 쓰기
        int[] pos = null;
        var tag = TagLib.GetStructAI(tagName, ref pos);
        if (tag != null)
        {
            tag.curr = value;
        }
    }

    private async Task<List<TimestampedValue>> GetTagHistory(
        string tagName, DateTime from, DateTime to)
    {
        // TrendSaveManager 또는 DB에서 이력 조회
        return await DataPostgres.GetMinDataAI(tagName, from, to);
    }
}

public class PdmResult
{
    public string EquipmentId { get; set; }
    public double HealthIndex { get; set; }     // 0.0 (위험) ~ 1.0 (건전)
    public string HealthStatus { get; set; }    // "healthy", "warning", "critical"
    public double RulHours { get; set; }        // 잔여수명 (시간), -1 = 예측 불가
    public double RulConfidence { get; set; }   // 예측 신뢰도
    public string RecommendedAction { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string Details { get; set; }
}
```

---

## 5. Python 측 구현

### 5.1 pdm_service.py

**파일:** `python_ai_engine/services/pdm_service.py`

```python
"""예측 정비 서비스"""

import numpy as np
import time
from typing import Any

async def register(router, **deps):
    router.add_handler("pdm/health", handle_health)
    router.add_handler("pdm/rul", handle_rul)
    router.add_handler("pdm/train", handle_train)
    router.add_handler("pdm/equipment", handle_equipment)
    router.add_handler("pdm/report", handle_report)


async def handle_health(payload: dict) -> dict:
    """건전성 지수 (Health Index) 계산"""
    tag_history = payload.get("tag_history", {})
    equipment = payload.get("equipment", {})

    if not tag_history:
        return {"health_index": 1.0, "status": "unknown", "message": "데이터 없음"}

    indicators = {}
    health_scores = []

    for tag_name, data in tag_history.items():
        values = np.array(data.get("values", []))
        role = data.get("role", "unknown")

        if len(values) < 10:
            continue

        # 역할별 건전성 지표 계산
        if role == "vibration":
            hi = _vibration_health(values)
        elif role == "temperature":
            hi = _temperature_health(values, equipment)
        elif role == "current":
            hi = _current_health(values)
        elif role == "pressure":
            hi = _pressure_health(values)
        else:
            hi = _generic_health(values)

        indicators[tag_name] = {
            "health": hi["score"],
            "trend": hi["trend"],
            "description": hi["description"]
        }
        health_scores.append(hi["score"])

    # 종합 건전성 = 가중 최소값 (가장 나쁜 지표가 전체를 지배)
    if health_scores:
        overall = min(health_scores) * 0.6 + np.mean(health_scores) * 0.4
    else:
        overall = 1.0

    status = "healthy" if overall > 0.7 else "warning" if overall > 0.3 else "critical"

    return {
        "health_index": float(overall),
        "status": status,
        "indicators": indicators,
        "evaluated_at": time.time()
    }


def _vibration_health(values: np.ndarray) -> dict:
    """진동 건전성 (RMS, Crest Factor, Kurtosis)"""
    rms = np.sqrt(np.mean(values ** 2))
    peak = np.max(np.abs(values))
    crest_factor = peak / rms if rms > 0 else 0
    kurtosis = float(np.mean(((values - np.mean(values)) / np.std(values)) ** 4)) \
               if np.std(values) > 0 else 3.0

    # 정상: RMS < 4.5 mm/s (ISO 10816 Class I)
    rms_score = max(0, 1.0 - rms / 11.2)  # 11.2 = Danger threshold

    # 정상: Crest Factor 2~6, 높으면 충격성 결함
    cf_score = 1.0 if 2 <= crest_factor <= 6 else \
               max(0, 1.0 - abs(crest_factor - 4) / 10)

    # 정상: Kurtosis ≈ 3, 높으면 임펄스 결함
    kurt_score = 1.0 if kurtosis < 4 else max(0, 1.0 - (kurtosis - 3) / 10)

    score = rms_score * 0.5 + cf_score * 0.3 + kurt_score * 0.2

    # 트렌드 (최근 1/3 vs 이전 2/3)
    n = len(values)
    split = n * 2 // 3
    recent_rms = np.sqrt(np.mean(values[split:] ** 2))
    prev_rms = np.sqrt(np.mean(values[:split] ** 2))
    trend = "increasing" if recent_rms > prev_rms * 1.1 else \
            "decreasing" if recent_rms < prev_rms * 0.9 else "stable"

    return {
        "score": float(score),
        "trend": trend,
        "description": f"RMS={rms:.2f}mm/s, CF={crest_factor:.1f}, Kurt={kurtosis:.1f}"
    }


def _temperature_health(values: np.ndarray, equipment: dict) -> dict:
    """온도 건전성"""
    current = values[-1]
    mean_val = np.mean(values)
    std_val = np.std(values)

    # 온도 상승 트렌드
    n = len(values)
    split = n * 2 // 3
    recent_mean = np.mean(values[split:])
    prev_mean = np.mean(values[:split])
    drift = recent_mean - prev_mean

    # 정상 범위 대비 점수
    if std_val > 0:
        z = abs(current - mean_val) / std_val
        score = max(0, 1.0 - z / 5.0)
    else:
        score = 1.0

    # 지속 상승 시 감점
    if drift > std_val:
        score *= 0.8

    trend = "increasing" if drift > std_val * 0.5 else \
            "decreasing" if drift < -std_val * 0.5 else "stable"

    return {
        "score": float(score),
        "trend": trend,
        "description": f"현재={current:.1f}, 평균={mean_val:.1f}, 편차={drift:+.1f}"
    }


def _current_health(values: np.ndarray) -> dict:
    """전류 건전성 (불균형, 과전류)"""
    mean_val = np.mean(values)
    std_val = np.std(values)
    current = values[-1]

    # 전류 변동 계수 (높으면 불안정)
    cv = std_val / mean_val if mean_val > 0 else 0
    cv_score = max(0, 1.0 - cv / 0.3)

    score = cv_score

    n = len(values)
    split = n * 2 // 3
    trend = "increasing" if np.mean(values[split:]) > np.mean(values[:split]) * 1.05 \
            else "stable"

    return {
        "score": float(score),
        "trend": trend,
        "description": f"전류={current:.1f}A, CV={cv:.3f}"
    }


def _pressure_health(values: np.ndarray) -> dict:
    """압력 건전성"""
    return _generic_health(values)


def _generic_health(values: np.ndarray) -> dict:
    """범용 건전성 (Z-score 기반)"""
    mean_val = np.mean(values)
    std_val = np.std(values)
    current = values[-1]

    if std_val > 0:
        z = abs(current - mean_val) / std_val
        score = max(0, 1.0 - z / 5.0)
    else:
        score = 1.0

    return {"score": float(score), "trend": "stable",
            "description": f"현재={current:.2f}, 평균={mean_val:.2f}"}


async def handle_rul(payload: dict) -> dict:
    """잔여수명 (RUL) 예측"""
    equipment = payload.get("equipment", {})
    tag_history = payload.get("tag_history", {})

    running_hours = equipment.get("running_hours", 0)
    design_life = equipment.get("design_life_hours", 50000)

    # Tier-1: 설계수명 기반 단순 추정
    simple_rul = max(0, design_life - running_hours)

    # Tier-2: 건전성 트렌드 기반 보정
    # 건전성 지수의 추세선으로 HI=0.3 도달 시점 예측
    health_result = await handle_health(payload)
    current_hi = health_result.get("health_index", 1.0)

    if current_hi < 0.3:
        # 이미 위험 수준
        rul_hours = 0
        recommendation = "즉시 정비 필요"
        confidence = 0.9
    elif current_hi < 0.6:
        # 열화 진행 중 - 트렌드 외삽
        degradation_rate = (1.0 - current_hi) / max(running_hours, 1)
        hours_to_critical = (current_hi - 0.3) / degradation_rate if degradation_rate > 0 \
                           else simple_rul
        rul_hours = min(hours_to_critical, simple_rul)
        recommendation = f"{rul_hours:.0f}시간 내 정비 계획 수립"
        confidence = 0.6
    else:
        rul_hours = simple_rul * current_hi  # 건전성으로 보정
        recommendation = "정상 운전 유지"
        confidence = 0.4

    # 다음 정비 예상 시점
    next_maintenance = None
    if rul_hours < simple_rul:
        import datetime
        next_maintenance = (
            datetime.datetime.now() + datetime.timedelta(hours=rul_hours)
        ).isoformat()

    return {
        "rul_hours": float(rul_hours),
        "confidence": confidence,
        "recommendation": recommendation,
        "next_maintenance": next_maintenance,
        "health_index": current_hi,
        "details": {
            "design_rul": simple_rul,
            "adjusted_rul": float(rul_hours),
            "running_hours": running_hours,
            "health_index": current_hi
        }
    }


async def handle_train(payload: dict) -> dict:
    """고장 패턴 학습 (이력 데이터 기반)"""
    # 향후 구현: 고장 이력 + 센서 데이터로 지도학습 모델 훈련
    return {"status": "not_implemented",
            "message": "Tier-2 ML 모델 학습은 향후 구현 예정"}


async def handle_equipment(payload: dict) -> dict:
    """장비 등록/조회"""
    action = payload.get("action", "list")
    # 장비 관리 CRUD
    return {"status": "ok", "action": action}


async def handle_report(payload: dict) -> dict:
    """정비 보고서 생성"""
    equipment_id = payload.get("equipment_id")
    # 건전성 이력 + RUL 추이 + 정비 이력을 종합한 보고서
    return {"status": "ok", "report": "보고서 생성 기능 구현 예정"}
```

---

## 6. 구현 단계

### Phase 1 (2주): 기본 프레임워크
- Equipment 모델 + 설정 UI
- PredictiveMaintenanceBridge 기본 구조
- 통계적 건전성 지수 (Tier-1)

### Phase 2 (2주): Python 서비스
- pdm_service (health, rul)
- 진동/온도/전류 건전성 계산
- RUL 예측 (설계수명 + 트렌드 보정)

### Phase 3 (1주): 알람 + 대시보드
- AlarmProcessor 연동
- 가상 태그 (HI, RUL) 연동
- 정비 일정 관리

---

## 7. 의존성

- **C# 측**: TrendSaveManager, AlarmProcessor, PythonAiManager, TagLib, DataPostgres
- **Python 측**: numpy, scipy (선택)
- **신규 파일**:
  - `LocalMain/PredictiveMaintenance/PredictiveMaintenanceBridge.cs`
  - `LocalMain/PredictiveMaintenance/Equipment.cs`
  - `python_ai_engine/services/pdm_service.py`

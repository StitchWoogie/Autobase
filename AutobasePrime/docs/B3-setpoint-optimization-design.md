# B3. 자동 셋포인트 최적화 - 상세 구현 설계

## 1. 개요

이력 데이터와 현재 공정 상태를 분석하여 최적 셋포인트/운전 조건을 AI가 추천.
운전원 승인 후 Semi-Auto 적용.

**핵심 가치:** 에너지 10~15% 절감, 품질 편차 감소, 운전 최적화

---

## 2. 아키텍처

```
┌─────────────────────────────────────────────────────────────┐
│ LocalMain                                                   │
│                                                             │
│  SetpointOptimizer (신규)                                    │
│  ├─ 현재 공정 상태 수집 (태그 스냅샷)                         │
│  ├─ Python 최적화 서비스 호출                                │
│  ├─ 추천 결과 → 운전원 승인 UI                              │
│  └─ 승인 시 → Tag.SetAnalog() / AO 태그 쓰기               │
│                                                             │
└──────────────┬──────────────────────────────────────────────┘
               │ TCP
               ▼
┌─────────────────────────────────────────────────────────────┐
│ python_ai_engine                                             │
│                                                             │
│  setpoint_service (신규)                                     │
│  ├─ setpoint/recommend    현재 상태 → 최적 셋포인트 추천     │
│  ├─ setpoint/evaluate     추천 결과 시뮬레이션               │
│  ├─ setpoint/history      적용 이력 조회                    │
│  └─ setpoint/train        최적화 모델 학습                   │
│                                                             │
│  알고리즘:                                                   │
│  ├─ Tier-1: 룩업 테이블 (조건별 최적값 매핑)                 │
│  ├─ Tier-2: Random Forest 회귀 (이력 학습)                   │
│  └─ Tier-3: 베이지안 최적화                                  │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. C# 구현

### 3.1 SetpointOptimizer

**파일:** `LocalMain/Setpoint/SetpointOptimizer.cs`

```csharp
public class SetpointOptimizer
{
    private List<SetpointGroup> _groups;
    private Timer _evaluationTimer;

    public void Initialize(List<SetpointGroup> groups)
    {
        _groups = groups;
        _evaluationTimer = new Timer();
        _evaluationTimer.Interval = 300000; // 5분마다
        _evaluationTimer.Elapsed += async (s, e) => await EvaluateAll();
        _evaluationTimer.Start();
    }

    private async Task EvaluateAll()
    {
        foreach (var group in _groups)
        {
            // 현재 공정 상태 수집
            var state = CollectProcessState(group);

            // Python 서비스 호출
            var payload = new JObject
            {
                ["group_name"] = group.Name,
                ["current_state"] = JObject.FromObject(state),
                ["setpoint_tags"] = new JArray(
                    group.SetpointTags.Select(t => t.TagName)),
                ["constraint_tags"] = new JArray(
                    group.ConstraintTags.Select(t => t.TagName)),
                ["objective"] = group.Objective // "minimize_energy", "maximize_quality"
            };

            string result = await PythonAiManager.CallAsync(
                "setpoint/recommend", payload.ToString());

            var response = JObject.Parse(result);
            if (response["Ok"]?.Value<bool>() == true)
            {
                var recommendations = response["Result"]["recommendations"]
                    .ToObject<List<SetpointRecommendation>>();

                // 가상 태그에 추천값 저장 (대시보드 표시용)
                foreach (var rec in recommendations)
                {
                    SetVirtualTag($"SP_REC_{rec.TagName}", rec.RecommendedValue);
                    SetVirtualTag($"SP_SAVE_{rec.TagName}", rec.EstimatedSaving);
                }

                // 변화량이 임계값 이상이면 알람으로 알림
                if (recommendations.Any(r =>
                    Math.Abs(r.CurrentValue - r.RecommendedValue) > r.ChangeThreshold))
                {
                    NotifyOperator(group, recommendations);
                }
            }
        }
    }

    /// <summary>
    /// 운전원 승인 후 셋포인트 적용
    /// </summary>
    public async Task ApplyRecommendation(
        string tagName, double newValue, string approvedBy)
    {
        // 안전 검사
        var constraint = ValidateConstraint(tagName, newValue);
        if (!constraint.IsValid)
            throw new InvalidOperationException(constraint.Reason);

        // AO/DO 태그 쓰기
        await PlcScan.SetTagValue(tagName, newValue.ToString(), false);

        // 적용 이력 기록
        LogSetpointChange(tagName, newValue, approvedBy);
    }
}

public class SetpointGroup
{
    public string Name { get; set; }              // "냉각수 시스템"
    public string Objective { get; set; }         // "minimize_energy"
    public List<SetpointTag> SetpointTags { get; set; }    // 조정 대상
    public List<ConstraintTag> ConstraintTags { get; set; } // 제약 조건
    public List<InputTag> InputTags { get; set; }           // 입력 조건
}

public class SetpointTag
{
    public string TagName { get; set; }    // "CoolWater_TempSP"
    public double MinValue { get; set; }   // 15.0
    public double MaxValue { get; set; }   // 30.0
    public double ChangeThreshold { get; set; } // 0.5 (변화 알림 기준)
}

public class SetpointRecommendation
{
    public string TagName { get; set; }
    public double CurrentValue { get; set; }
    public double RecommendedValue { get; set; }
    public double EstimatedSaving { get; set; }  // 예상 절감율 (%)
    public double Confidence { get; set; }
    public string Reason { get; set; }
    public double ChangeThreshold { get; set; }
}
```

---

## 4. Python 구현

### 4.1 setpoint_service.py

**파일:** `python_ai_engine/services/setpoint_service.py`

```python
"""셋포인트 최적화 서비스"""

import numpy as np
from typing import Any

async def register(router, **deps):
    router.add_handler("setpoint/recommend", handle_recommend)
    router.add_handler("setpoint/evaluate", handle_evaluate)
    router.add_handler("setpoint/history", handle_history)
    router.add_handler("setpoint/train", handle_train)


async def handle_recommend(payload: dict) -> dict:
    """최적 셋포인트 추천"""
    current_state = payload.get("current_state", {})
    setpoint_tags = payload.get("setpoint_tags", [])
    objective = payload.get("objective", "minimize_energy")

    recommendations = []

    for sp_tag in setpoint_tags:
        current_val = current_state.get(sp_tag, {}).get("value", 0)

        # Tier-1: 룩업 테이블 기반 추천
        rec = _lookup_table_recommend(sp_tag, current_state, objective)

        if rec is None:
            # Tier-2: 통계적 최적값 (이력 데이터 기반)
            rec = _statistical_recommend(sp_tag, current_state, objective)

        recommendations.append({
            "tag_name": sp_tag,
            "current_value": current_val,
            "recommended_value": rec["value"],
            "estimated_saving": rec.get("saving", 0),
            "confidence": rec.get("confidence", 0.5),
            "reason": rec.get("reason", ""),
            "change_threshold": rec.get("threshold", 0.5)
        })

    return {"recommendations": recommendations}


def _lookup_table_recommend(sp_tag, state, objective):
    """조건별 최적값 룩업 테이블"""
    # 예: 외기온도별 냉각수 셋포인트
    outdoor_temp = state.get("OutdoorTemp", {}).get("value")
    if outdoor_temp is None:
        return None

    # 간단한 선형 보간 테이블
    table = {
        0: 18.0,   # 외기 0°C → SP 18°C
        10: 20.0,
        20: 22.0,
        30: 25.0,
        35: 27.0,
        40: 28.0,
    }

    # 보간
    temps = sorted(table.keys())
    for i in range(len(temps) - 1):
        if temps[i] <= outdoor_temp <= temps[i + 1]:
            ratio = (outdoor_temp - temps[i]) / (temps[i + 1] - temps[i])
            optimal = table[temps[i]] + ratio * (table[temps[i + 1]] - table[temps[i]])

            return {
                "value": round(optimal, 1),
                "saving": 5.0,
                "confidence": 0.7,
                "reason": f"외기온도 {outdoor_temp:.1f}°C 기준 최적값",
                "threshold": 0.5
            }

    return None


def _statistical_recommend(sp_tag, state, objective):
    """통계적 추천 (이력 데이터 분석)"""
    # 현재값에서 소폭 조정 (보수적)
    current = state.get(sp_tag, {}).get("value", 0)
    return {
        "value": current,
        "saving": 0,
        "confidence": 0.3,
        "reason": "통계 데이터 부족 (현재값 유지)",
        "threshold": 1.0
    }


async def handle_evaluate(payload: dict) -> dict:
    """추천 결과 시뮬레이션"""
    return {"status": "not_implemented",
            "message": "공정 시뮬레이션 모델 필요"}


async def handle_history(payload: dict) -> dict:
    """적용 이력 조회"""
    return {"history": []}


async def handle_train(payload: dict) -> dict:
    """최적화 모델 학습"""
    return {"status": "not_implemented",
            "message": "충분한 운전 이력 수집 후 학습 가능"}
```

---

## 5. 구현 단계

### Phase 1 (2주): 룩업 테이블 기반
- SetpointOptimizer 기본 프레임워크
- 조건별 룩업 테이블 설정
- 운전원 승인 UI

### Phase 2 (3주): 통계/ML 기반
- 이력 데이터 분석 → 최적 조건 학습
- Random Forest 회귀 모델
- 절감 효과 추정

### Phase 3 (장기): 베이지안 최적화
- 다변량 최적화
- 공정 시뮬레이션 연동

---

## 6. 의존성

- **C# 측**: PythonAiManager, PlcScan, TagLib, AlarmProcessor
- **Python 측**: numpy, scikit-learn (선택)
- **신규 파일**:
  - `LocalMain/Setpoint/SetpointOptimizer.cs`
  - `python_ai_engine/services/setpoint_service.py`

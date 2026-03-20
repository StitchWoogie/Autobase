# B6. 에너지 최적화 분석 - 상세 구현 설계

## 1. 개요

전력/가스/스팀 등 에너지 사용 패턴을 분석하여 절감 방안을 자동 제안.
기존 Demand 모듈과 연계하여 피크 관리 + AI 기반 비효율 탐지.

**핵심 가치:** 에너지 비용 10~20% 절감, 피크 관리 최적화

---

## 2. 아키텍처

```
┌──────────────────────────────────────────────────────────┐
│ LocalMain                                                │
│                                                          │
│  기존 Demand 모듈 (LocalMain/Demand/)                     │
│  ├─ DemandManager - 수요 관리                             │
│  ├─ DemandCalculation - 계산                             │
│  └─ DemandSchedule - 스케줄                              │
│       │                                                  │
│       ▼                                                  │
│  EnergyOptimizationBridge (신규)                          │
│  ├─ 주기적 에너지 분석 요청                               │
│  ├─ 패턴 분석 결과 → 대시보드 태그                        │
│  └─ 절감 제안 → 운전원 알림                               │
└──────────────┬───────────────────────────────────────────┘
               │ TCP
               ▼
┌──────────────────────────────────────────────────────────┐
│ python_ai_engine                                          │
│                                                          │
│  energy_service (신규)                                    │
│  ├─ energy/analyze     사용 패턴 분석                     │
│  ├─ energy/peak        피크 예측/관리                     │
│  ├─ energy/waste       낭비 탐지                          │
│  ├─ energy/benchmark   장비별 효율 비교                   │
│  └─ energy/report      에너지 보고서 생성                 │
└──────────────────────────────────────────────────────────┘
```

---

## 3. C# 구현

### 3.1 EnergyOptimizationBridge

**파일:** `LocalMain/Energy/EnergyOptimizationBridge.cs`

```csharp
public class EnergyOptimizationBridge
{
    private EnergyConfig _config;
    private Timer _analysisTimer;

    public void Initialize(EnergyConfig config)
    {
        _config = config;
        _analysisTimer = new Timer();
        _analysisTimer.Interval = config.AnalysisIntervalMinutes * 60000;
        _analysisTimer.Elapsed += async (s, e) => await RunAnalysis();
        _analysisTimer.Start();
    }

    private async Task RunAnalysis()
    {
        // 에너지 태그 데이터 수집 (최근 24시간)
        var energyData = new JObject();
        foreach (var meter in _config.EnergyMeters)
        {
            var history = await DataPostgres.GetMinDataAI(
                meter.TagName, DateTime.Now.AddHours(-24), DateTime.Now);

            energyData[meter.TagName] = new JObject
            {
                ["values"] = new JArray(history.Select(h => h.Value)),
                ["timestamps"] = new JArray(history.Select(h => h.Timestamp.ToString("o"))),
                ["unit"] = meter.Unit,    // "kW", "m3/h", "ton/h"
                ["cost_rate"] = meter.CostRate  // 원/kWh
            };
        }

        // 장비 가동 상태
        var equipmentStatus = new JObject();
        foreach (var eq in _config.MonitoredEquipment)
        {
            int[] pos = null;
            var tag = TagLib.GetStructDI(eq.RunTagName, ref pos);
            equipmentStatus[eq.Name] = new JObject
            {
                ["running"] = tag?.curr == 1,
                ["power_tag"] = eq.PowerTagName,
                ["rated_power"] = eq.RatedPower
            };
        }

        var payload = new JObject
        {
            ["energy_data"] = energyData,
            ["equipment_status"] = equipmentStatus,
            ["outdoor_temp"] = GetCurrentOutdoorTemp(),
            ["production_rate"] = GetProductionRate()
        };

        // 분석 요청
        string result = await PythonAiManager.CallAsync(
            "energy/analyze", payload.ToString());

        var response = JObject.Parse(result);
        if (response["Ok"]?.Value<bool>() == true)
        {
            ProcessAnalysisResult(response["Result"]);
        }

        // 피크 예측
        string peakResult = await PythonAiManager.CallAsync(
            "energy/peak", payload.ToString());

        var peakResponse = JObject.Parse(peakResult);
        if (peakResponse["Ok"]?.Value<bool>() == true)
        {
            ProcessPeakPrediction(peakResponse["Result"]);
        }
    }

    private void ProcessAnalysisResult(JToken result)
    {
        // 에너지 효율 지표 → 가상 태그
        double efficiency = result["overall_efficiency"]?.Value<double>() ?? 0;
        SetVirtualTag("ENERGY_EFFICIENCY", efficiency);

        // 낭비 탐지 결과
        var wastes = result["waste_detections"]?.ToObject<List<WasteDetection>>();
        if (wastes != null)
        {
            foreach (var waste in wastes.Where(w => w.EstimatedLoss > _config.WasteAlertThreshold))
            {
                AlarmProcessor.Instance.AddAlarm(new ALARM_FILE_STRUCT
                {
                    tagName = waste.TagName,
                    message = $"[에너지낭비] {waste.Description} " +
                             $"(예상 손실: {waste.EstimatedLoss:N0}원/일)",
                    priority = 2
                });
            }
        }
    }

    private void ProcessPeakPrediction(JToken result)
    {
        double peakPredicted = result["predicted_peak_kw"]?.Value<double>() ?? 0;
        double contractPeak = _config.ContractPeakKw;

        SetVirtualTag("ENERGY_PEAK_PREDICT", peakPredicted);

        if (peakPredicted > contractPeak * 0.9)
        {
            var shedList = result["recommended_shedding"]?.ToObject<List<string>>();
            string shedMsg = shedList != null ? string.Join(", ", shedList) : "";

            AlarmProcessor.Instance.AddAlarm(new ALARM_FILE_STRUCT
            {
                tagName = "ENERGY_PEAK_PREDICT",
                message = $"[피크경고] 예상 피크 {peakPredicted:N0}kW " +
                         $"(계약: {contractPeak:N0}kW). " +
                         $"감축 추천: {shedMsg}",
                priority = 1
            });
        }
    }
}

public class EnergyConfig
{
    public int AnalysisIntervalMinutes { get; set; } = 15;
    public double ContractPeakKw { get; set; }          // 계약 전력
    public double WasteAlertThreshold { get; set; } = 10000; // 원/일
    public List<EnergyMeter> EnergyMeters { get; set; }
    public List<MonitoredEquipment> MonitoredEquipment { get; set; }
}

public class EnergyMeter
{
    public string TagName { get; set; }    // "Total_Power_kW"
    public string Unit { get; set; }       // "kW"
    public double CostRate { get; set; }   // 원/kWh
}
```

---

## 4. Python 구현

### 4.1 energy_service.py

**파일:** `python_ai_engine/services/energy_service.py`

```python
"""에너지 최적화 서비스"""

import numpy as np
from datetime import datetime

async def register(router, **deps):
    router.add_handler("energy/analyze", handle_analyze)
    router.add_handler("energy/peak", handle_peak)
    router.add_handler("energy/waste", handle_waste)
    router.add_handler("energy/benchmark", handle_benchmark)
    router.add_handler("energy/report", handle_report)


async def handle_analyze(payload: dict) -> dict:
    """에너지 사용 패턴 분석"""
    energy_data = payload.get("energy_data", {})
    equipment_status = payload.get("equipment_status", {})

    total_consumption = 0
    meter_analysis = {}

    for tag, data in energy_data.items():
        values = np.array(data.get("values", []))
        if len(values) == 0:
            continue

        cost_rate = data.get("cost_rate", 100)

        avg_power = float(np.mean(values))
        peak_power = float(np.max(values))
        min_power = float(np.min(values))
        total_kwh = float(np.sum(values) / 60)  # 분데이터 → kWh

        # 부하율 (Load Factor)
        load_factor = avg_power / peak_power if peak_power > 0 else 0

        meter_analysis[tag] = {
            "avg_power": avg_power,
            "peak_power": peak_power,
            "min_power": min_power,
            "total_kwh": total_kwh,
            "estimated_cost": total_kwh * cost_rate,
            "load_factor": load_factor
        }
        total_consumption += total_kwh * cost_rate

    # 낭비 탐지
    waste_detections = _detect_waste(energy_data, equipment_status)

    # 전체 효율
    overall_efficiency = np.mean([
        m["load_factor"] for m in meter_analysis.values()
    ]) if meter_analysis else 0

    return {
        "overall_efficiency": float(overall_efficiency),
        "total_cost_24h": total_consumption,
        "meter_analysis": meter_analysis,
        "waste_detections": waste_detections,
        "suggestions": _generate_suggestions(meter_analysis, waste_detections)
    }


def _detect_waste(energy_data, equipment_status):
    """에너지 낭비 탐지"""
    wastes = []

    for eq_name, eq_info in equipment_status.items():
        running = eq_info.get("running", False)
        power_tag = eq_info.get("power_tag")
        rated_power = eq_info.get("rated_power", 0)

        if power_tag and power_tag in energy_data:
            values = np.array(energy_data[power_tag].get("values", []))
            if len(values) == 0:
                continue

            current_power = values[-1] if len(values) > 0 else 0

            # 비가동 중 전력 소모 (대기전력 낭비)
            if not running and current_power > rated_power * 0.05:
                daily_loss = current_power * 24 * energy_data[power_tag].get("cost_rate", 100)
                wastes.append({
                    "tag_name": power_tag,
                    "type": "standby_waste",
                    "description": f"{eq_name} 정지 중 대기전력 {current_power:.1f}kW",
                    "estimated_loss": daily_loss
                })

            # 과부하 운전 (정격 대비 110% 초과)
            if running and rated_power > 0 and current_power > rated_power * 1.1:
                wastes.append({
                    "tag_name": power_tag,
                    "type": "overload",
                    "description": f"{eq_name} 과부하 운전 " +
                                  f"({current_power:.1f}/{rated_power:.1f}kW)",
                    "estimated_loss": (current_power - rated_power) * 24 * \
                                     energy_data[power_tag].get("cost_rate", 100)
                })

            # 저효율 운전 (정격 대비 30% 미만)
            if running and rated_power > 0 and current_power < rated_power * 0.3:
                wastes.append({
                    "tag_name": power_tag,
                    "type": "low_efficiency",
                    "description": f"{eq_name} 저부하 운전 " +
                                  f"({current_power/rated_power*100:.0f}%)",
                    "estimated_loss": 0  # 직접 손실보다 효율 문제
                })

    return wastes


async def handle_peak(payload: dict) -> dict:
    """피크 전력 예측"""
    energy_data = payload.get("energy_data", {})

    # 총 전력 데이터 (합산 또는 주 전력계)
    total_values = None
    for tag, data in energy_data.items():
        values = np.array(data.get("values", []))
        if total_values is None:
            total_values = values
        else:
            total_values = total_values + values[:len(total_values)]

    if total_values is None or len(total_values) < 60:
        return {"predicted_peak_kw": 0, "confidence": 0}

    # 최근 1시간 트렌드로 다음 15분 피크 예측
    recent = total_values[-60:]  # 최근 60분
    trend = np.polyfit(range(len(recent)), recent, 1)  # 선형 추세
    predicted_15min = np.polyval(trend, len(recent) + 15)

    # 피크 시간대 보정 (오전 10-12시, 오후 2-5시 가중)
    hour = datetime.now().hour
    peak_hours = {10: 1.1, 11: 1.15, 14: 1.1, 15: 1.15, 16: 1.1}
    multiplier = peak_hours.get(hour, 1.0)
    predicted = float(max(predicted_15min, np.max(recent)) * multiplier)

    # 감축 추천
    equipment = payload.get("equipment_status", {})
    shedding = []
    for eq_name, eq_info in equipment.items():
        if eq_info.get("running") and eq_info.get("rated_power", 0) > 0:
            # 우선순위 낮은 장비 (향후 설정으로 관리)
            shedding.append(eq_name)

    return {
        "predicted_peak_kw": predicted,
        "current_peak_kw": float(np.max(recent)),
        "confidence": 0.6,
        "recommended_shedding": shedding[:3]  # 상위 3개
    }


def _generate_suggestions(meter_analysis, wastes):
    """절감 제안 생성"""
    suggestions = []

    for tag, info in meter_analysis.items():
        if info["load_factor"] < 0.4:
            suggestions.append(
                f"{tag}: 부하율 {info['load_factor']:.0%}로 낮음. "
                f"운전 스케줄 최적화 검토")

    for waste in wastes:
        if waste["estimated_loss"] > 0:
            suggestions.append(
                f"{waste['description']} → "
                f"일 {waste['estimated_loss']:,.0f}원 절감 가능")

    return suggestions


async def handle_benchmark(payload: dict) -> dict:
    """장비별 효율 비교"""
    return {"status": "not_implemented"}


async def handle_report(payload: dict) -> dict:
    """에너지 보고서"""
    return {"status": "not_implemented"}
```

---

## 5. 구현 단계

### Phase 1 (2주): 패턴 분석 + 낭비 탐지
- EnergyOptimizationBridge 기본 구조
- 에너지 사용 통계 분석
- 낭비 탐지 (대기전력, 과부하, 저효율)

### Phase 2 (2주): 피크 관리
- 피크 전력 예측
- 감축 추천
- Demand 모듈 연동

### Phase 3 (1주): 보고서 + 대시보드
- 에너지 보고서 생성
- 대시보드 가상 태그 연동

---

## 6. 의존성

- **C# 측**: Demand 모듈, PythonAiManager, AlarmProcessor, DataPostgres
- **Python 측**: numpy
- **신규 파일**:
  - `LocalMain/Energy/EnergyOptimizationBridge.cs`
  - `python_ai_engine/services/energy_service.py`

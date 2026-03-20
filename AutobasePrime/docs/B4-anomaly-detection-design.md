# B4. 이상 탐지 (Anomaly Detection) - 상세 구현 설계

## 1. 개요

정상 운전 패턴을 학습하고 실시간 태그 데이터에서 이상 상태를 탐지.
통계 기반(Tier-1) → ML 기반(Tier-2) → 다변량 분석(Tier-3) 3단계 접근.

**핵심 가치:** 장비 이상 조기 감지, 비계획 정지 예방, 운전원 부담 감소

---

## 2. 아키텍처

```
┌─────────────────────────────────────────────────────────────┐
│ LocalMain Runtime                                           │
│                                                             │
│  CheckEngineTagChange                                       │
│       │ (태그 변화 감지)                                     │
│       ▼                                                     │
│  AnomalyDetectionBridge (신규)                               │
│       │                                                     │
│       ├─ 실시간 데이터 버퍼 (슬라이딩 윈도우)                │
│       │  ┌──────────────────────────────────┐               │
│       │  │ Tag1: [v1, v2, v3, ..., vN]      │               │
│       │  │ Tag2: [v1, v2, v3, ..., vN]      │               │
│       │  │ ...                              │               │
│       │  └──────────────────────────────────┘               │
│       │                                                     │
│       ├─ Tier-1: 통계 기반 (C# 내장, 항시 동작)             │
│       │  ├─ 이동 평균/표준편차                               │
│       │  ├─ CUSUM (누적합 관리도)                            │
│       │  └─ 변화율 감지                                     │
│       │                                                     │
│       ├─ Tier-2: ML 기반 (Python AI Engine)                  │
│       │  ├─ Isolation Forest                                │
│       │  └─ Autoencoder                                     │
│       │                                                     │
│       └─ 이상 점수 → AlarmProcessor                         │
│                                                             │
└──────────────┬──────────────────────────────────────────────┘
               │ TCP (port 5678)
               ▼
┌─────────────────────────────────────────────────────────────┐
│ python_ai_engine                                             │
│                                                             │
│  anomaly_service (신규)                                      │
│  ├─ anomaly/detect     - 실시간 이상 탐지                    │
│  ├─ anomaly/train      - 정상 패턴 학습                      │
│  ├─ anomaly/status     - 모델 상태 조회                      │
│  └─ anomaly/config     - 감시 설정                           │
│                                                             │
│  models/                                                     │
│  ├─ anomaly_iforest_{tag_group}.joblib                       │
│  └─ anomaly_autoencoder_{tag_group}.onnx                     │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. C# 측 구현

### 3.1 AnomalyDetectionBridge

**파일:** `LocalMain/AnomalyDetection/AnomalyDetectionBridge.cs`

```csharp
public class AnomalyDetectionBridge
{
    // 감시 대상 태그 그룹
    private List<AnomalyWatchGroup> _watchGroups;

    // 실시간 데이터 버퍼 (태그별 슬라이딩 윈도우)
    private Dictionary<string, CircularBuffer<TimestampedValue>> _dataBuffers;

    // 통계 엔진 (Tier-1)
    private Dictionary<string, StatisticalDetector> _statDetectors;

    // ML 호출 주기 (Tier-2, 초 단위)
    private int _mlDetectionIntervalSec = 60;
    private DateTime _lastMlDetection = DateTime.MinValue;

    // 콜백 등록 (C_init에서 호출)
    public static void RegisterCallbacks()
    {
        // CheckEngineTagChange에서 태그 변화 시 호출될 콜백
        CheckEngineTagChange.OnAITagChanged += OnTagValueChanged;
    }

    public void Initialize(List<AnomalyWatchGroup> groups)
    {
        _watchGroups = groups;
        _dataBuffers = new Dictionary<string, CircularBuffer<TimestampedValue>>();
        _statDetectors = new Dictionary<string, StatisticalDetector>();

        foreach (var group in groups)
        {
            foreach (var tagName in group.Tags)
            {
                _dataBuffers[tagName] = new CircularBuffer<TimestampedValue>(
                    group.WindowSize);
                _statDetectors[tagName] = new StatisticalDetector(
                    group.StatConfig);
            }
        }
    }

    // 태그 값 변화 시 호출
    private static void OnTagValueChanged(string tagName, double value, DateTime timestamp)
    {
        if (!_instance._dataBuffers.ContainsKey(tagName)) return;

        // 버퍼에 추가
        _instance._dataBuffers[tagName].Add(new TimestampedValue(timestamp, value));

        // Tier-1: 통계 기반 즉시 검사
        var statResult = _instance._statDetectors[tagName].Detect(value);
        if (statResult.IsAnomaly)
        {
            _instance.RaiseAnomalyAlarm(tagName, statResult);
        }

        // Tier-2: ML 기반 주기 검사
        if ((DateTime.Now - _instance._lastMlDetection).TotalSeconds
            >= _instance._mlDetectionIntervalSec)
        {
            _instance._lastMlDetection = DateTime.Now;
            _ = _instance.RunMlDetectionAsync();
        }
    }

    // ML 기반 이상 탐지 (비동기)
    private async Task RunMlDetectionAsync()
    {
        foreach (var group in _watchGroups)
        {
            if (!group.EnableMlDetection) continue;

            // 버퍼 데이터를 JSON으로 구성
            var payload = new JObject();
            var tagData = new JObject();

            foreach (var tagName in group.Tags)
            {
                var buffer = _dataBuffers[tagName];
                var values = buffer.ToArray().Select(v => v.Value).ToArray();
                tagData[tagName] = new JArray(values);
            }

            payload["tag_data"] = tagData;
            payload["group_name"] = group.Name;
            payload["model_name"] = group.ModelName;

            try
            {
                string result = await PythonAiManager.CallAsync(
                    "anomaly/detect", payload.ToString());

                var response = JObject.Parse(result);
                if (response["Ok"]?.Value<bool>() == true)
                {
                    ProcessMlResult(group, response["Result"]);
                }
            }
            catch (Exception ex)
            {
                // ML 실패 시 Tier-1만 동작 (graceful degradation)
                System.Diagnostics.Debug.WriteLine(
                    $"ML anomaly detection failed: {ex.Message}");
            }
        }
    }

    private void ProcessMlResult(AnomalyWatchGroup group, JToken result)
    {
        double anomalyScore = result["anomaly_score"]?.Value<double>() ?? 0;
        bool isAnomaly = result["is_anomaly"]?.Value<bool>() ?? false;
        string[] anomalousTags = result["anomalous_tags"]?
            .ToObject<string[]>() ?? Array.Empty<string>();

        if (isAnomaly)
        {
            foreach (var tagName in anomalousTags)
            {
                RaiseAnomalyAlarm(tagName, new AnomalyResult
                {
                    IsAnomaly = true,
                    Score = anomalyScore,
                    Method = "ML",
                    Description = result["description"]?.ToString()
                                  ?? "ML 모델 이상 탐지"
                });
            }
        }
    }

    private void RaiseAnomalyAlarm(string tagName, AnomalyResult result)
    {
        // AlarmProcessor를 통한 알람 발생
        var alarm = new ALARM_FILE_STRUCT
        {
            tagName = tagName,
            message = $"[이상탐지] {result.Description} (점수: {result.Score:F2}, 방법: {result.Method})",
            priority = result.Score > 0.9 ? 1 : 2,
            alarmType = "ANOMALY"
        };

        AlarmProcessor.Instance.AddAlarm(alarm);
    }
}
```

### 3.2 StatisticalDetector (Tier-1)

**파일:** `LocalMain/AnomalyDetection/StatisticalDetector.cs`

```csharp
public class StatisticalDetector
{
    private StatConfig _config;

    // 이동 통계량
    private double _movingMean;
    private double _movingVariance;
    private int _count;
    private double _cusumPos;    // CUSUM 양방향
    private double _cusumNeg;
    private double _prevValue;

    public StatisticalDetector(StatConfig config)
    {
        _config = config;
        Reset();
    }

    public AnomalyResult Detect(double value)
    {
        _count++;

        // 1. Welford 온라인 평균/분산 갱신
        double delta = value - _movingMean;
        _movingMean += delta / _count;
        double delta2 = value - _movingMean;
        _movingVariance += delta * delta2;

        double stdDev = _count > 1
            ? Math.Sqrt(_movingVariance / (_count - 1)) : 0;

        // 학습 기간 (최소 데이터 수집)
        if (_count < _config.MinSamples)
        {
            _prevValue = value;
            return AnomalyResult.Normal;
        }

        // 2. Z-Score 검사
        double zScore = stdDev > 0 ? Math.Abs(value - _movingMean) / stdDev : 0;
        if (zScore > _config.ZScoreThreshold)
        {
            return new AnomalyResult
            {
                IsAnomaly = true,
                Score = Math.Min(zScore / _config.ZScoreThreshold, 1.0),
                Method = "Z-Score",
                Description = $"Z-Score {zScore:F2} > 임계값 {_config.ZScoreThreshold}"
            };
        }

        // 3. CUSUM (누적합 관리도)
        double target = _movingMean;
        double slack = _config.CusumSlack * stdDev;

        _cusumPos = Math.Max(0, _cusumPos + (value - target) - slack);
        _cusumNeg = Math.Max(0, _cusumNeg - (value - target) - slack);

        double cusumThreshold = _config.CusumThreshold * stdDev;
        if (_cusumPos > cusumThreshold || _cusumNeg > cusumThreshold)
        {
            var result = new AnomalyResult
            {
                IsAnomaly = true,
                Score = Math.Max(_cusumPos, _cusumNeg) / cusumThreshold,
                Method = "CUSUM",
                Description = $"누적 편차 감지 (CUSUM+ {_cusumPos:F2}, CUSUM- {_cusumNeg:F2})"
            };
            _cusumPos = 0;
            _cusumNeg = 0;
            return result;
        }

        // 4. 변화율 검사
        if (_prevValue != 0)
        {
            double changeRate = Math.Abs(value - _prevValue) / Math.Abs(_prevValue);
            if (changeRate > _config.MaxChangeRate)
            {
                _prevValue = value;
                return new AnomalyResult
                {
                    IsAnomaly = true,
                    Score = Math.Min(changeRate / _config.MaxChangeRate, 1.0),
                    Method = "ChangeRate",
                    Description = $"급격한 변화: {changeRate * 100:F1}% " +
                                 $"(임계: {_config.MaxChangeRate * 100:F1}%)"
                };
            }
        }

        _prevValue = value;
        return AnomalyResult.Normal;
    }

    public void Reset()
    {
        _movingMean = 0;
        _movingVariance = 0;
        _count = 0;
        _cusumPos = 0;
        _cusumNeg = 0;
        _prevValue = 0;
    }
}

public class StatConfig
{
    public int MinSamples { get; set; } = 100;
    public double ZScoreThreshold { get; set; } = 3.0;
    public double CusumSlack { get; set; } = 0.5;
    public double CusumThreshold { get; set; } = 5.0;
    public double MaxChangeRate { get; set; } = 0.3; // 30%
}
```

### 3.3 AnomalyWatchGroup 설정

```csharp
public class AnomalyWatchGroup
{
    public string Name { get; set; }           // "보일러1 온도계통"
    public List<string> Tags { get; set; }     // ["Boiler1_Temp", "Boiler1_Press"]
    public int WindowSize { get; set; } = 300; // 슬라이딩 윈도우 크기
    public bool EnableMlDetection { get; set; } = true;
    public string ModelName { get; set; }      // "anomaly_boiler1"
    public StatConfig StatConfig { get; set; } = new StatConfig();
}

public class CircularBuffer<T>
{
    private T[] _buffer;
    private int _head;
    private int _count;

    public CircularBuffer(int capacity)
    {
        _buffer = new T[capacity];
    }

    public void Add(T item)
    {
        _buffer[_head] = item;
        _head = (_head + 1) % _buffer.Length;
        if (_count < _buffer.Length) _count++;
    }

    public T[] ToArray()
    {
        var result = new T[_count];
        for (int i = 0; i < _count; i++)
        {
            int idx = (_head - _count + i + _buffer.Length) % _buffer.Length;
            result[i] = _buffer[idx];
        }
        return result;
    }
}
```

---

## 4. Python 측 구현

### 4.1 anomaly_service.py

**파일:** `python_ai_engine/services/anomaly_service.py`

```python
"""이상 탐지 서비스 - Isolation Forest / Autoencoder"""

import numpy as np
import json
import os
import time
from typing import Any

_model_registry = None
_model_cache = None

async def register(router, **deps):
    global _model_registry, _model_cache
    _model_registry = deps.get("model_registry")
    _model_cache = deps.get("model_cache")

    router.add_handler("anomaly/detect", handle_detect)
    router.add_handler("anomaly/train", handle_train)
    router.add_handler("anomaly/status", handle_status)
    router.add_handler("anomaly/config", handle_config)


async def handle_detect(payload: dict) -> dict:
    """실시간 이상 탐지"""
    tag_data = payload.get("tag_data", {})
    group_name = payload.get("group_name", "default")
    model_name = payload.get("model_name", f"anomaly_{group_name}")

    if not tag_data:
        return {"is_anomaly": False, "anomaly_score": 0.0, "method": "none"}

    # 데이터를 2D 배열로 변환 (태그 × 시간)
    tag_names = sorted(tag_data.keys())
    values_matrix = []
    for tag in tag_names:
        values = tag_data[tag]
        if isinstance(values, list):
            values_matrix.append(values)

    if not values_matrix:
        return {"is_anomaly": False, "anomaly_score": 0.0}

    data = np.array(values_matrix).T  # (time_steps, n_tags)

    # Tier-2a: Isolation Forest
    result = await _detect_isolation_forest(data, tag_names, model_name)

    return result


async def _detect_isolation_forest(
    data: np.ndarray,
    tag_names: list,
    model_name: str
) -> dict:
    """Isolation Forest 기반 이상 탐지"""
    start = time.time()

    # 모델 로드 (캐시 우선)
    model = None
    if _model_cache:
        model = _model_cache.get(model_name)

    if model is None:
        # 학습된 모델 없으면 즉석 학습 (데이터가 충분할 때)
        if data.shape[0] < 50:
            return {
                "is_anomaly": False,
                "anomaly_score": 0.0,
                "method": "insufficient_data",
                "description": f"데이터 부족 ({data.shape[0]}개, 최소 50개 필요)"
            }

        try:
            from sklearn.ensemble import IsolationForest
            model = IsolationForest(
                n_estimators=100,
                contamination=0.05,
                random_state=42
            )
            model.fit(data)

            if _model_cache:
                _model_cache.put(model_name, model)
        except ImportError:
            return _fallback_statistical_detect(data, tag_names)

    # 최신 데이터 포인트로 이상 판정
    latest = data[-1:, :]  # 마지막 행
    score = model.decision_function(latest)[0]  # 음수 = 이상
    prediction = model.predict(latest)[0]       # -1 = 이상, 1 = 정상

    # 점수 정규화 (0~1, 1이 가장 이상)
    normalized_score = max(0, min(1, -score))

    # 이상 태그 식별
    anomalous_tags = []
    if prediction == -1:
        # 각 태그별 기여도 계산 (간단: Z-score 기반)
        means = np.mean(data, axis=0)
        stds = np.std(data, axis=0)
        stds[stds == 0] = 1  # 0 나누기 방지

        z_scores = np.abs((latest[0] - means) / stds)
        for i, z in enumerate(z_scores):
            if z > 2.0:
                anomalous_tags.append(tag_names[i])

    elapsed = (time.time() - start) * 1000

    return {
        "is_anomaly": prediction == -1,
        "anomaly_score": float(normalized_score),
        "method": "isolation_forest",
        "anomalous_tags": anomalous_tags,
        "description": _build_description(prediction, normalized_score, anomalous_tags),
        "inference_ms": elapsed
    }


async def handle_train(payload: dict) -> dict:
    """정상 패턴 학습"""
    tag_data = payload.get("tag_data", {})
    group_name = payload.get("group_name", "default")
    model_name = payload.get("model_name", f"anomaly_{group_name}")
    contamination = payload.get("contamination", 0.05)

    tag_names = sorted(tag_data.keys())
    values_matrix = [tag_data[tag] for tag in tag_names]
    data = np.array(values_matrix).T

    if data.shape[0] < 100:
        return {
            "status": "error",
            "message": f"학습 데이터 부족: {data.shape[0]}개 (최소 100개)"
        }

    try:
        from sklearn.ensemble import IsolationForest
        import joblib

        model = IsolationForest(
            n_estimators=200,
            contamination=contamination,
            random_state=42
        )
        model.fit(data)

        # 모델 저장
        model_path = os.path.join("models", f"{model_name}.joblib")
        joblib.dump({
            "model": model,
            "tag_names": tag_names,
            "trained_at": time.time(),
            "data_shape": data.shape,
            "contamination": contamination
        }, model_path)

        # 캐시 갱신
        if _model_cache:
            _model_cache.put(model_name, model)

        return {
            "status": "success",
            "model_name": model_name,
            "data_points": data.shape[0],
            "tags": tag_names,
            "model_path": model_path
        }
    except ImportError:
        return {"status": "error", "message": "scikit-learn not installed"}


async def handle_status(payload: dict) -> dict:
    """모델 상태 조회"""
    model_name = payload.get("model_name")

    if model_name:
        model_path = os.path.join("models", f"{model_name}.joblib")
        if os.path.exists(model_path):
            import joblib
            info = joblib.load(model_path)
            return {
                "exists": True,
                "model_name": model_name,
                "tag_names": info.get("tag_names", []),
                "trained_at": info.get("trained_at"),
                "data_shape": info.get("data_shape")
            }
        return {"exists": False, "model_name": model_name}

    # 전체 모델 목록
    models = []
    model_dir = "models"
    if os.path.exists(model_dir):
        for f in os.listdir(model_dir):
            if f.startswith("anomaly_") and f.endswith(".joblib"):
                models.append(f.replace(".joblib", ""))
    return {"models": models}


async def handle_config(payload: dict) -> dict:
    """감시 설정 관리"""
    action = payload.get("action", "get")

    if action == "get":
        return {"config": _get_default_config()}
    elif action == "set":
        new_config = payload.get("config", {})
        # 설정 저장 로직
        return {"status": "updated", "config": new_config}

    return {"error": f"Unknown action: {action}"}


def _fallback_statistical_detect(data: np.ndarray, tag_names: list) -> dict:
    """ML 모델 없을 때 통계적 폴백"""
    latest = data[-1, :]
    means = np.mean(data, axis=0)
    stds = np.std(data, axis=0)
    stds[stds == 0] = 1

    z_scores = np.abs((latest - means) / stds)
    max_z = float(np.max(z_scores))
    is_anomaly = max_z > 3.0

    anomalous = [tag_names[i] for i, z in enumerate(z_scores) if z > 3.0]

    return {
        "is_anomaly": is_anomaly,
        "anomaly_score": min(max_z / 5.0, 1.0),
        "method": "statistical_fallback",
        "anomalous_tags": anomalous,
        "description": f"통계적 이상: Z-score {max_z:.2f}"
    }


def _build_description(prediction, score, anomalous_tags):
    if prediction == 1:
        return "정상 범위 내"
    tags_str = ", ".join(anomalous_tags[:3])
    if len(anomalous_tags) > 3:
        tags_str += f" 외 {len(anomalous_tags)-3}개"
    return f"이상 감지 (점수: {score:.2f}) - 관련 태그: {tags_str}"


def _get_default_config():
    return {
        "detection_interval_sec": 60,
        "min_training_samples": 100,
        "contamination": 0.05,
        "z_score_threshold": 3.0,
        "cusum_slack": 0.5,
        "cusum_threshold": 5.0
    }
```

---

## 5. 트렌드 시각화 연동

### 5.1 ObjectMultiTrend에 이상 구간 오버레이

```csharp
// ObjectMultiTrend 확장 (기존 클래스에 추가)
// 이상 구간 하이라이트를 위한 데이터
public List<AnomalyHighlight> AnomalyHighlights { get; set; }

public class AnomalyHighlight
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double Score { get; set; }
    public string Description { get; set; }
    public Color HighlightColor { get; set; } = Color.FromArgb(60, 255, 0, 0);
}

// DrawTrend() 메서드 내에서 이상 구간 렌더링
private void DrawAnomalyHighlights(Graphics g, Rectangle chartArea)
{
    if (AnomalyHighlights == null) return;

    foreach (var highlight in AnomalyHighlights)
    {
        int x1 = TimeToX(highlight.StartTime, chartArea);
        int x2 = TimeToX(highlight.EndTime, chartArea);

        using (var brush = new SolidBrush(highlight.HighlightColor))
        {
            g.FillRectangle(brush, x1, chartArea.Top,
                           x2 - x1, chartArea.Height);
        }

        // 이상 점수 표시
        using (var font = new Font("맑은 고딕", 8))
        {
            g.DrawString($"이상:{highlight.Score:P0}",
                font, Brushes.Red, x1, chartArea.Top + 2);
        }
    }
}
```

---

## 6. 구현 단계

### Phase 1 (2주): Tier-1 통계 기반
- StatisticalDetector 구현 (Z-Score, CUSUM, 변화율)
- AnomalyDetectionBridge + CheckEngineTagChange 연동
- CircularBuffer 구현

### Phase 2 (2주): Tier-2 ML 기반
- python_ai_engine에 anomaly_service 추가
- Isolation Forest 학습/추론
- 모델 저장/로드

### Phase 3 (1주): 시각화 + 통합
- ObjectMultiTrend 이상 구간 오버레이
- 이상 탐지 설정 UI
- AlarmProcessor 연동

---

## 7. 의존성

- **C# 측**: CheckEngineTagChange, AlarmProcessor, ObjectMultiTrend, PythonAiManager
- **Python 측**: scikit-learn (Isolation Forest), numpy
- **신규 파일**:
  - `LocalMain/AnomalyDetection/AnomalyDetectionBridge.cs`
  - `LocalMain/AnomalyDetection/StatisticalDetector.cs`
  - `python_ai_engine/services/anomaly_service.py`

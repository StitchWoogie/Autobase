# B2. 지능형 알람 관리 - 상세 구현 설계

## 1. 개요

알람 홍수(Alarm Flood) 억제, 근본 원인 분석(RCA), 동적 우선순위 조정을 통해
운전원의 알람 피로도를 감소시키고 대응 효율을 향상.

**핵심 가치:** 알람 홍수 70% 감소, 근본 원인 식별 시간 80% 단축

---

## 2. 아키텍처

```
┌─────────────────────────────────────────────────────────────┐
│ LocalMain - 알람 파이프라인                                    │
│                                                             │
│  CheckEngineTagChange                                       │
│       │ (알람 조건 감지)                                     │
│       ▼                                                     │
│  AlarmDisplay.AlarmDisplayAI/DI()                            │
│       │                                                     │
│       ▼                                                     │
│  ┌─────────────────────────────────────┐                    │
│  │ IntelligentAlarmEngine (신규)        │                    │
│  │                                     │                    │
│  │ Stage 1: 알람 수집 + 버퍼링         │                    │
│  │   └─ 3초 윈도우 내 알람 그룹핑      │                    │
│  │                                     │                    │
│  │ Stage 2: 알람 홍수 억제             │                    │
│  │   ├─ 시간 상관 그룹핑              │                    │
│  │   ├─ 인과관계 분석                 │                    │
│  │   └─ 대표 알람 선정                │                    │
│  │                                     │                    │
│  │ Stage 3: 근본 원인 분석             │                    │
│  │   ├─ 규칙 기반 RCA                 │                    │
│  │   ├─ 이력 패턴 매칭                │                    │
│  │   └─ (선택) Python AI 호출          │                    │
│  │                                     │                    │
│  │ Stage 4: 우선순위 동적 조정         │                    │
│  │   ├─ 운전 상태별 임계값 변경        │                    │
│  │   └─ 반복 알람 감쇠                │                    │
│  └──────────┬──────────────────────────┘                    │
│             ▼                                               │
│  AlarmProcessor.AddAlarm() (기존)                            │
│       │                                                     │
│       ▼                                                     │
│  FormPopupAlarmConfirmation (수정: RCA 정보 표시)             │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. C# 핵심 구현

### 3.1 IntelligentAlarmEngine

**파일:** `LocalMain/Alarm/IntelligentAlarmEngine.cs`

```csharp
public class IntelligentAlarmEngine
{
    // 알람 버퍼 (시간 윈도우 내 수집)
    private List<AlarmEvent> _alarmBuffer = new List<AlarmEvent>();
    private Timer _flushTimer;
    private readonly object _bufferLock = new object();

    // 인과관계 규칙 DB
    private List<CausalRule> _causalRules;

    // 알람 이력 (패턴 매칭용)
    private CircularBuffer<AlarmCluster> _historyBuffer;

    // 운전 상태
    private OperationMode _currentMode = OperationMode.Normal;

    // 설정
    private IntelligentAlarmConfig _config;

    private static IntelligentAlarmEngine _instance;
    public static IntelligentAlarmEngine Instance => _instance;

    public void Initialize(IntelligentAlarmConfig config)
    {
        _config = config;
        _instance = this;
        _causalRules = LoadCausalRules();
        _historyBuffer = new CircularBuffer<AlarmCluster>(1000);

        // 버퍼 플러시 타이머 (알람 그룹핑 윈도우)
        _flushTimer = new Timer();
        _flushTimer.Interval = _config.GroupingWindowMs; // 3000ms
        _flushTimer.Elapsed += (s, e) => FlushAlarmBuffer();
        _flushTimer.Start();
    }

    /// <summary>
    /// 원본 알람을 수신 (AlarmDisplay에서 호출)
    /// 즉시 전달하지 않고 버퍼에 수집
    /// </summary>
    public void ReceiveAlarm(ALARM_FILE_STRUCT alarm)
    {
        lock (_bufferLock)
        {
            _alarmBuffer.Add(new AlarmEvent
            {
                Alarm = alarm,
                Timestamp = DateTime.Now,
                TagName = alarm.tagName,
                Priority = alarm.priority
            });
        }

        // 버퍼가 임계값 초과 시 즉시 플러시
        if (_alarmBuffer.Count > _config.MaxBufferSize)
            FlushAlarmBuffer();
    }

    private void FlushAlarmBuffer()
    {
        List<AlarmEvent> batch;
        lock (_bufferLock)
        {
            if (_alarmBuffer.Count == 0) return;
            batch = new List<AlarmEvent>(_alarmBuffer);
            _alarmBuffer.Clear();
        }

        // Stage 1: 시간 상관 그룹핑
        var clusters = GroupByTimeCorrelation(batch);

        foreach (var cluster in clusters)
        {
            // Stage 2: 알람 홍수 억제
            var suppressed = SuppressAlarmFlood(cluster);

            // Stage 3: 근본 원인 분석
            var rcaResult = AnalyzeRootCause(suppressed);

            // Stage 4: 우선순위 동적 조정
            var adjusted = AdjustPriority(suppressed, rcaResult);

            // 최종 알람 전달
            foreach (var alarmEvent in adjusted)
            {
                var enriched = EnrichAlarm(alarmEvent, rcaResult);
                AlarmProcessor.Instance.AddAlarm(enriched);
            }

            // 이력 저장
            _historyBuffer.Add(new AlarmCluster
            {
                Events = cluster,
                RootCause = rcaResult,
                Timestamp = DateTime.Now
            });
        }
    }

    // ═══════════════════════════════════════════
    // Stage 1: 시간 상관 그룹핑
    // ═══════════════════════════════════════════

    private List<List<AlarmEvent>> GroupByTimeCorrelation(List<AlarmEvent> alarms)
    {
        if (alarms.Count <= 1)
            return new List<List<AlarmEvent>> { alarms };

        var sorted = alarms.OrderBy(a => a.Timestamp).ToList();
        var clusters = new List<List<AlarmEvent>>();
        var current = new List<AlarmEvent> { sorted[0] };

        for (int i = 1; i < sorted.Count; i++)
        {
            // 인접 알람 간 시간 차이가 임계값 이내면 같은 그룹
            if ((sorted[i].Timestamp - sorted[i - 1].Timestamp).TotalMilliseconds
                <= _config.CorrelationWindowMs)
            {
                current.Add(sorted[i]);
            }
            else
            {
                clusters.Add(current);
                current = new List<AlarmEvent> { sorted[i] };
            }
        }
        clusters.Add(current);

        return clusters;
    }

    // ═══════════════════════════════════════════
    // Stage 2: 알람 홍수 억제
    // ═══════════════════════════════════════════

    private List<AlarmEvent> SuppressAlarmFlood(List<AlarmEvent> cluster)
    {
        if (cluster.Count <= _config.FloodThreshold)
            return cluster; // 홍수 아님, 그대로 전달

        // 인과관계 규칙으로 대표 알람 선정
        var rootAlarms = new List<AlarmEvent>();
        var suppressedAlarms = new HashSet<string>();

        foreach (var rule in _causalRules)
        {
            var causeAlarm = cluster.FirstOrDefault(a =>
                MatchesPattern(a.TagName, rule.CauseTag));
            var effectAlarms = cluster.Where(a =>
                rule.EffectTags.Any(et => MatchesPattern(a.TagName, et)));

            if (causeAlarm != null && effectAlarms.Any())
            {
                rootAlarms.Add(causeAlarm);
                foreach (var effect in effectAlarms)
                {
                    suppressedAlarms.Add(effect.TagName);
                }
            }
        }

        // 규칙 매칭 안 된 알람 + 근본 원인 알람만 전달
        var result = cluster.Where(a =>
            !suppressedAlarms.Contains(a.TagName) ||
            rootAlarms.Contains(a)).ToList();

        // 억제 정보 기록
        int suppressedCount = cluster.Count - result.Count;
        if (suppressedCount > 0 && result.Count > 0)
        {
            result[0].SuppressionInfo =
                $"관련 알람 {suppressedCount}건 그룹화됨";
        }

        return result;
    }

    // ═══════════════════════════════════════════
    // Stage 3: 근본 원인 분석 (RCA)
    // ═══════════════════════════════════════════

    private RcaResult AnalyzeRootCause(List<AlarmEvent> alarms)
    {
        if (alarms.Count <= 1)
            return new RcaResult { ProbableRootCause = alarms.FirstOrDefault()?.TagName };

        // 방법 1: 규칙 기반 (인과관계 테이블 역방향 검색)
        foreach (var rule in _causalRules)
        {
            bool hasCause = alarms.Any(a =>
                MatchesPattern(a.TagName, rule.CauseTag));
            int effectCount = alarms.Count(a =>
                rule.EffectTags.Any(et => MatchesPattern(a.TagName, et)));

            if (hasCause && effectCount >= 1)
            {
                return new RcaResult
                {
                    ProbableRootCause = rule.CauseTag,
                    Confidence = 0.8 + 0.2 * Math.Min(effectCount / 3.0, 1.0),
                    Method = "rule_based",
                    Description = rule.Description,
                    CausalChain = BuildCausalChain(rule, alarms)
                };
            }
        }

        // 방법 2: 이력 패턴 매칭
        var historyMatch = FindSimilarHistoricalCluster(alarms);
        if (historyMatch != null)
        {
            return new RcaResult
            {
                ProbableRootCause = historyMatch.RootCause?.ProbableRootCause,
                Confidence = 0.5,
                Method = "history_pattern",
                Description = $"과거 유사 패턴 발견 ({historyMatch.Timestamp:g})"
            };
        }

        // 방법 3: 시간순 최초 알람 = 추정 원인
        var first = alarms.OrderBy(a => a.Timestamp).First();
        return new RcaResult
        {
            ProbableRootCause = first.TagName,
            Confidence = 0.3,
            Method = "temporal_first",
            Description = "시간순 최초 발생 알람 (추정)"
        };
    }

    private AlarmCluster FindSimilarHistoricalCluster(List<AlarmEvent> current)
    {
        var currentTags = new HashSet<string>(current.Select(a => a.TagName));

        foreach (var historical in _historyBuffer.ToArray().Reverse())
        {
            var histTags = new HashSet<string>(
                historical.Events.Select(a => a.TagName));

            // Jaccard 유사도
            int intersection = currentTags.Intersect(histTags).Count();
            int union = currentTags.Union(histTags).Count();
            double similarity = union > 0 ? (double)intersection / union : 0;

            if (similarity >= 0.7) // 70% 이상 유사
                return historical;
        }

        return null;
    }

    // ═══════════════════════════════════════════
    // Stage 4: 우선순위 동적 조정
    // ═══════════════════════════════════════════

    private List<AlarmEvent> AdjustPriority(
        List<AlarmEvent> alarms, RcaResult rca)
    {
        foreach (var alarm in alarms)
        {
            // 4-1. 운전 상태별 조정
            if (_currentMode == OperationMode.Startup ||
                _currentMode == OperationMode.Shutdown)
            {
                // 시운전/정지 중에는 특정 알람 우선순위 하향
                if (IsTransientAlarm(alarm.TagName))
                    alarm.Priority = Math.Min(alarm.Priority + 1, 4); // 낮춤
            }

            // 4-2. 반복 알람 감쇠 (chattering)
            if (IsChatteringAlarm(alarm.TagName))
            {
                alarm.Priority = Math.Min(alarm.Priority + 1, 4);
                alarm.Alarm.message += " [반복 알람]";
            }

            // 4-3. 근본 원인 알람 우선순위 상향
            if (rca.ProbableRootCause == alarm.TagName)
            {
                alarm.Priority = Math.Max(alarm.Priority - 1, 1); // 높임
            }
        }

        return alarms;
    }

    // 채터링 감지 (최근 N분 내 동일 태그 반복)
    private Dictionary<string, List<DateTime>> _chatterTracker
        = new Dictionary<string, List<DateTime>>();

    private bool IsChatteringAlarm(string tagName)
    {
        var now = DateTime.Now;
        if (!_chatterTracker.ContainsKey(tagName))
            _chatterTracker[tagName] = new List<DateTime>();

        _chatterTracker[tagName].Add(now);

        // 5분 이전 기록 제거
        _chatterTracker[tagName].RemoveAll(t =>
            (now - t).TotalMinutes > _config.ChatterWindowMinutes);

        return _chatterTracker[tagName].Count > _config.ChatterThreshold;
    }

    // ═══════════════════════════════════════════
    // 알람 보강 (RCA 정보 추가)
    // ═══════════════════════════════════════════

    private ALARM_FILE_STRUCT EnrichAlarm(AlarmEvent alarmEvent, RcaResult rca)
    {
        var alarm = alarmEvent.Alarm;

        // 억제 정보 추가
        if (!string.IsNullOrEmpty(alarmEvent.SuppressionInfo))
            alarm.message += $" ({alarmEvent.SuppressionInfo})";

        // RCA 정보 추가
        if (rca != null && rca.Confidence > 0.5)
        {
            alarm.message += $" [원인추정: {rca.ProbableRootCause}]";
        }

        alarm.priority = alarmEvent.Priority;

        return alarm;
    }

    // ═══════════════════════════════════════════
    // 운전 상태 관리
    // ═══════════════════════════════════════════

    public void SetOperationMode(OperationMode mode)
    {
        _currentMode = mode;
    }

    private bool MatchesPattern(string tagName, string pattern)
    {
        if (pattern.Contains("*"))
        {
            string regex = "^" + pattern.Replace("*", ".*") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(tagName, regex);
        }
        return tagName == pattern;
    }
}
```

### 3.2 인과관계 규칙 DB

```csharp
public class CausalRule
{
    public string CauseTag { get; set; }        // "Tank1_Level" (와일드카드 지원)
    public List<string> EffectTags { get; set; } // ["Pump1_*", "Pipe1_Pressure"]
    public string Description { get; set; }      // "탱크 레벨 저하 → 펌프/배관 연쇄"
    public double Confidence { get; set; }
}

// 규칙 예시 (JSON 파일에서 로드)
/*
[
  {
    "causeTag": "Tank1_Level",
    "effectTags": ["Pump1_Overheat", "Pump1_Current_Hi", "Pipe1_Press_Lo"],
    "description": "탱크 레벨 저하 시 펌프 과부하 및 배관 압력 저하",
    "confidence": 0.9
  },
  {
    "causeTag": "CoolWater_Temp_Hi",
    "effectTags": ["Boiler*_Temp_HiHi", "HeatExch*_Temp_Hi"],
    "description": "냉각수 온도 상승 시 관련 장비 온도 연쇄 상승",
    "confidence": 0.85
  }
]
*/
```

### 3.3 데이터 구조

```csharp
public class AlarmEvent
{
    public ALARM_FILE_STRUCT Alarm { get; set; }
    public DateTime Timestamp { get; set; }
    public string TagName { get; set; }
    public int Priority { get; set; }
    public string SuppressionInfo { get; set; }
}

public class AlarmCluster
{
    public List<AlarmEvent> Events { get; set; }
    public RcaResult RootCause { get; set; }
    public DateTime Timestamp { get; set; }
}

public class RcaResult
{
    public string ProbableRootCause { get; set; }
    public double Confidence { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }
    public List<string> CausalChain { get; set; }
}

public enum OperationMode
{
    Normal,     // 정상 운전
    Startup,    // 기동 중
    Shutdown,   // 정지 중
    Emergency   // 비상
}

public class IntelligentAlarmConfig
{
    public int GroupingWindowMs { get; set; } = 3000;
    public int CorrelationWindowMs { get; set; } = 5000;
    public int FloodThreshold { get; set; } = 5;
    public int MaxBufferSize { get; set; } = 50;
    public int ChatterWindowMinutes { get; set; } = 5;
    public int ChatterThreshold { get; set; } = 3;
    public string CausalRulesPath { get; set; } = "alarm_causal_rules.json";
}
```

---

## 4. AlarmDisplay 통합

**파일:** `LocalMain/Alarm/AlarmDisplay.cs` (기존 수정)

```csharp
// AlarmDisplay.AlarmDisplayAI() 또는 WaitAlarm()에서
// 기존: AlarmProcessor.Instance.AddAlarm(alarm);
// 수정: IntelligentAlarmEngine 경유

private void ProcessAlarm(ALARM_FILE_STRUCT alarm)
{
    if (IntelligentAlarmEngine.Instance != null &&
        TotalConfig.bUseIntelligentAlarm)
    {
        // 지능형 알람 엔진 경유
        IntelligentAlarmEngine.Instance.ReceiveAlarm(alarm);
    }
    else
    {
        // 기존 방식 (직접 전달)
        AlarmProcessor.Instance.AddAlarm(alarm);
    }
}
```

---

## 5. 알람 확인 UI 보강

### FormPopupAlarmConfirmation 수정

```csharp
// 알람 확인 팝업에 RCA 정보 표시
// 기존 알람 메시지에 추가된 [원인추정: xxx] 정보를 파싱하여
// 별도 패널에 시각적으로 표시

// 인과관계 체인 표시 예:
// ┌─────────────────────────────────────────┐
// │ 근본 원인 분석                            │
// │                                         │
// │ Tank1_Level (Low) ← 추정 원인           │
// │   └→ Pump1_Current (HiHi)              │
// │   └→ Pipe1_Pressure (Low)              │
// │   └→ Pump1_Overheat (HiHi) ← 현재     │
// │                                         │
// │ 신뢰도: 85%  방법: 규칙 기반            │
// │ [관련 화면 열기] [인과관계 편집]          │
// └─────────────────────────────────────────┘
```

---

## 6. 구현 단계

### Phase 1 (2주): 기본 엔진
- IntelligentAlarmEngine 구현 (버퍼링 + 그룹핑)
- 알람 홍수 억제 (시간 상관 + 임계값)
- AlarmDisplay 통합

### Phase 2 (2주): RCA + 규칙
- 인과관계 규칙 DB (JSON)
- 근본 원인 분석 (규칙 기반 + 이력 패턴)
- 인과관계 편집 UI

### Phase 3 (1주): 동적 조정 + UI
- 우선순위 동적 조정
- 채터링 감지
- 운전 상태별 모드
- 알람 확인 UI 보강

---

## 7. 의존성

- **기존 코드**: AlarmDisplay, AlarmProcessor, AlarmConfirm, CheckEngineTagChange
- **신규 파일**:
  - `LocalMain/Alarm/IntelligentAlarmEngine.cs`
  - `LocalMain/Alarm/IntelligentAlarmConfig.cs`
  - `LocalMain/Alarm/CausalRule.cs`
  - `alarm_causal_rules.json` (프로젝트별 설정)
- **외부 의존성 없음** (규칙 기반, Python AI는 선택적 확장)

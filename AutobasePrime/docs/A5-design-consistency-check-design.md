# A5. 디자인 일관성 검사 - 상세 구현 설계

## 1. 개요

SCADA 화면의 디자인 품질을 규칙 기반 + AI 보조로 분석하여 일관성 문제, HMI 표준 위반,
접근성 문제를 자동 감지하고 개선안을 제시.

**핵심 가치:** ISA-101 HMI 가이드라인 준수 자동화, 디자인 리뷰 시간 90% 단축

---

## 2. 아키텍처

```
┌──────────────────────────────────────────────────────┐
│ StudioMain 메뉴: [도구] → [AI 디자인 검사]            │
└──────────┬───────────────────────────────────────────┘
           ▼
┌──────────────────────────────────────────────────────┐
│ FormDesignChecker (검사 결과 대화상자)                 │
│  ┌─────────────────────────────────────────────────┐ │
│  │ [전체 검사] [폰트] [색상] [정렬] [태그] [접근성] │ │
│  ├─────────────────────────────────────────────────┤ │
│  │ 검사 결과 (TreeView)                            │ │
│  │ ├─ ⚠ 폰트 일관성 (3건)                         │ │
│  │ │  ├─ Text1: 맑은고딕 9pt (다수 객체: 10pt)     │ │
│  │ │  ├─ Label3: 굴림 10pt (표준: 맑은고딕)        │ │
│  │ │  └─ Meter2: 8pt (최소 권장: 9pt)              │ │
│  │ ├─ ✕ 색상 팔레트 (2건)                          │ │
│  │ │  ├─ Valve1: #FF0000 (표준 위험색: #C81E1E)    │ │
│  │ │  └─ Pump3: 배경 대비율 2.1:1 (최소: 3:1)     │ │
│  │ ├─ ℹ 정렬/간격 (5건)                            │ │
│  │ │  ├─ Tank1~Tank3: X축 불균일 (12,14,11px)     │ │
│  │ │  └─ ...                                       │ │
│  │ └─ ✓ 태그 바인딩 (문제 없음)                    │ │
│  ├─────────────────────────────────────────────────┤ │
│  │ 상세 정보 + [자동 수정] [무시] [규칙 설정]       │ │
│  └─────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────┐
│ DesignCheckEngine                    │
│                                      │
│ - FontConsistencyChecker             │
│ - ColorPaletteChecker                │
│ - AlignmentChecker                   │
│ - TagBindingChecker                  │
│ - AccessibilityChecker               │
│ - NamingConventionChecker            │
│ - ISA101ComplianceChecker            │
└──────────────────────────────────────┘
```

---

## 3. 검사 규칙 엔진

### 3.1 DesignCheckEngine (핵심)

**파일:** `Studio/DesignCheck/DesignCheckEngine.cs`

```csharp
public class DesignCheckEngine
{
    private List<IDesignChecker> _checkers;
    private DesignCheckConfig _config;

    public DesignCheckEngine(DesignCheckConfig config = null)
    {
        _config = config ?? DesignCheckConfig.Default;
        _checkers = new List<IDesignChecker>
        {
            new FontConsistencyChecker(_config),
            new ColorPaletteChecker(_config),
            new AlignmentChecker(_config),
            new TagBindingChecker(_config),
            new AccessibilityChecker(_config),
            new NamingConventionChecker(_config),
            new ISA101ComplianceChecker(_config),
        };
    }

    /// <summary>
    /// 화면 전체 검사
    /// </summary>
    public DesignCheckResult CheckModule(ObjectGroup objectGroup)
    {
        var result = new DesignCheckResult();
        var allObjects = CollectAllObjects(objectGroup);

        foreach (var checker in _checkers)
        {
            var issues = checker.Check(allObjects, objectGroup);
            result.Issues.AddRange(issues);
        }

        result.Summary = BuildSummary(result.Issues);
        return result;
    }

    private List<ObjectExpand> CollectAllObjects(ObjectGroup group)
    {
        var objects = new List<ObjectExpand>();
        // ObjectGroup의 모든 자식 객체를 재귀적으로 수집
        foreach (var obj in group.arrayObject)
        {
            if (obj is ObjectExpand expand)
                objects.Add(expand);
            if (obj is ObjectGroup subGroup)
                objects.AddRange(CollectAllObjects(subGroup));
        }
        return objects;
    }
}

public interface IDesignChecker
{
    string Category { get; }
    List<DesignIssue> Check(List<ObjectExpand> objects, ObjectGroup rootGroup);
}

public class DesignIssue
{
    public string Category { get; set; }        // "Font", "Color", "Alignment"
    public IssueSeverity Severity { get; set; }  // Error, Warning, Info
    public string ObjectName { get; set; }       // 대상 객체
    public string Description { get; set; }      // 문제 설명
    public string Suggestion { get; set; }       // 개선 제안
    public Action AutoFix { get; set; }          // 자동 수정 액션 (nullable)
    public Rectangle ObjectBounds { get; set; }  // 객체 위치 (화면 하이라이트용)
}

public enum IssueSeverity
{
    Error,      // ISA-101 위반, 기능 문제
    Warning,    // 일관성 위반, 사용성 문제
    Info        // 개선 제안
}
```

### 3.2 FontConsistencyChecker

```csharp
public class FontConsistencyChecker : IDesignChecker
{
    public string Category => "폰트 일관성";

    public List<DesignIssue> Check(List<ObjectExpand> objects, ObjectGroup root)
    {
        var issues = new List<DesignIssue>();

        // 폰트 사용 통계 수집
        var fontStats = new Dictionary<string, int>();     // "맑은 고딕/10pt" → count
        var fontSizeStats = new Dictionary<int, int>();    // 10 → count

        foreach (var obj in objects)
        {
            if (obj is ObjectFont fontObj && fontObj.logFont != null)
            {
                string fontKey = $"{fontObj.logFont.lfFaceName}/{Math.Abs(fontObj.logFont.lfHeight)}pt";
                fontStats[fontKey] = fontStats.GetValueOrDefault(fontKey, 0) + 1;
                int size = Math.Abs(fontObj.logFont.lfHeight);
                fontSizeStats[size] = fontSizeStats.GetValueOrDefault(size, 0) + 1;
            }
        }

        // 최다 사용 폰트 = 표준 폰트
        string standardFont = fontStats.OrderByDescending(kv => kv.Value)
                                       .FirstOrDefault().Key;
        string standardFace = standardFont?.Split('/')[0];
        int standardSize = fontSizeStats.OrderByDescending(kv => kv.Value)
                                        .FirstOrDefault().Key;

        // 일탈 검사
        foreach (var obj in objects)
        {
            if (obj is ObjectFont fontObj && fontObj.logFont != null)
            {
                string face = fontObj.logFont.lfFaceName;
                int size = Math.Abs(fontObj.logFont.lfHeight);

                // 폰트 패밀리 불일치
                if (!string.IsNullOrEmpty(standardFace) && face != standardFace)
                {
                    issues.Add(new DesignIssue
                    {
                        Category = Category,
                        Severity = IssueSeverity.Warning,
                        ObjectName = obj.sClassName,
                        Description = $"폰트 '{face}' 사용 (표준: {standardFace})",
                        Suggestion = $"폰트를 '{standardFace}'로 변경 권장",
                        AutoFix = () => { fontObj.logFont.lfFaceName = standardFace; },
                        ObjectBounds = GetBounds(obj)
                    });
                }

                // 최소 폰트 크기 위반
                if (size < _config.MinFontSize)
                {
                    issues.Add(new DesignIssue
                    {
                        Category = Category,
                        Severity = IssueSeverity.Warning,
                        ObjectName = obj.sClassName,
                        Description = $"폰트 크기 {size}pt (최소 권장: {_config.MinFontSize}pt)",
                        Suggestion = $"가독성을 위해 {_config.MinFontSize}pt 이상 권장"
                    });
                }
            }
        }

        return issues;
    }
}
```

### 3.3 ColorPaletteChecker

```csharp
public class ColorPaletteChecker : IDesignChecker
{
    public string Category => "색상 팔레트";

    // ISA-101 표준 색상 팔레트
    private static readonly Dictionary<string, Color[]> ISA101Colors = new()
    {
        ["위험/정지"] = new[] { Color.FromArgb(200, 30, 30) },      // 빨강
        ["경고"] = new[] { Color.FromArgb(255, 165, 0) },           // 주황
        ["정상/운전"] = new[] { Color.FromArgb(0, 128, 80) },       // 녹색
        ["비활성"] = new[] { Color.FromArgb(128, 128, 128) },       // 회색
        ["배경"] = new[] { Color.FromArgb(50, 50, 60),              // 어두운 배경
                          Color.FromArgb(235, 238, 242) },          // 밝은 배경
    };

    public List<DesignIssue> Check(List<ObjectExpand> objects, ObjectGroup root)
    {
        var issues = new List<DesignIssue>();
        var usedColors = new Dictionary<Color, int>();

        foreach (var obj in objects)
        {
            // 전경색/배경색 수집
            Color lineColor = obj.lLineColor;
            Color textColor = obj.lTextColor;

            TrackColor(usedColors, lineColor);
            TrackColor(usedColors, textColor);

            // 순수 빨강(#FF0000) 같은 과도한 채도 검사
            if (IsPureSaturated(lineColor))
            {
                issues.Add(new DesignIssue
                {
                    Category = Category,
                    Severity = IssueSeverity.Warning,
                    ObjectName = obj.sClassName,
                    Description = $"과도한 채도 색상 #{ColorToHex(lineColor)} 사용",
                    Suggestion = "ISA-101 권장 색상 팔레트 사용 권장"
                });
            }
        }

        // 색상 다양성 경고 (너무 많은 고유 색상)
        if (usedColors.Count > _config.MaxUniqueColors)
        {
            issues.Add(new DesignIssue
            {
                Category = Category,
                Severity = IssueSeverity.Info,
                ObjectName = "(전체)",
                Description = $"고유 색상 {usedColors.Count}개 사용 (권장: {_config.MaxUniqueColors}개 이하)",
                Suggestion = "색상 팔레트를 단순화하여 일관성 향상"
            });
        }

        return issues;
    }

    private bool IsPureSaturated(Color c)
    {
        // R, G, B 중 하나만 255이고 나머지가 0인 경우
        return (c.R == 255 && c.G == 0 && c.B == 0) ||
               (c.R == 0 && c.G == 255 && c.B == 0) ||
               (c.R == 0 && c.G == 0 && c.B == 255);
    }
}
```

### 3.4 AlignmentChecker

```csharp
public class AlignmentChecker : IDesignChecker
{
    public string Category => "정렬/간격";

    public List<DesignIssue> Check(List<ObjectExpand> objects, ObjectGroup root)
    {
        var issues = new List<DesignIssue>();

        // 같은 Y좌표 근처의 객체들 (수평 행 감지)
        var rows = GroupByProximity(objects, o => o.nTop, _config.AlignmentTolerance);
        foreach (var row in rows)
        {
            if (row.Count < 2) continue;

            // Y 좌표 불일치 검사
            var tops = row.Select(o => o.nTop).Distinct().ToList();
            if (tops.Count > 1)
            {
                int median = tops.OrderBy(t => t).ElementAt(tops.Count / 2);
                foreach (var obj in row.Where(o => o.nTop != median))
                {
                    int diff = Math.Abs(obj.nTop - median);
                    if (diff > 0 && diff <= _config.AlignmentTolerance)
                    {
                        issues.Add(new DesignIssue
                        {
                            Category = Category,
                            Severity = IssueSeverity.Info,
                            ObjectName = obj.sClassName,
                            Description = $"수평 정렬 불일치: Y={obj.nTop} (행 기준: {median}, 차이: {diff}px)",
                            Suggestion = $"Y좌표를 {median}으로 정렬",
                            AutoFix = () => { obj.nTop = median; }
                        });
                    }
                }
            }

            // 간격 균일성 검사 (3개 이상 객체)
            if (row.Count >= 3)
            {
                var sorted = row.OrderBy(o => o.nLeft).ToList();
                var gaps = new List<int>();
                for (int i = 1; i < sorted.Count; i++)
                {
                    gaps.Add(sorted[i].nLeft - sorted[i - 1].nRight);
                }

                double avgGap = gaps.Average();
                double maxDeviation = gaps.Max(g => Math.Abs(g - avgGap));

                if (maxDeviation > _config.SpacingTolerance)
                {
                    issues.Add(new DesignIssue
                    {
                        Category = Category,
                        Severity = IssueSeverity.Info,
                        ObjectName = $"{sorted.First().sClassName}~{sorted.Last().sClassName}",
                        Description = $"간격 불균일: {string.Join(",", gaps)}px (평균: {avgGap:F0}px)",
                        Suggestion = $"균일 간격 {avgGap:F0}px로 재배치"
                    });
                }
            }
        }

        // 객체 크기 불일치 (같은 타입 객체)
        var typeGroups = objects.GroupBy(o => o.GetType().Name);
        foreach (var group in typeGroups)
        {
            if (group.Count() < 2) continue;
            var sizes = group.Select(o => new { o.sClassName, W = o.nRight - o.nLeft, H = o.nBottom - o.nTop }).ToList();
            var avgW = sizes.Average(s => s.W);
            var avgH = sizes.Average(s => s.H);

            foreach (var s in sizes)
            {
                if (Math.Abs(s.W - avgW) > _config.SizeTolerance ||
                    Math.Abs(s.H - avgH) > _config.SizeTolerance)
                {
                    issues.Add(new DesignIssue
                    {
                        Category = Category,
                        Severity = IssueSeverity.Info,
                        ObjectName = s.sClassName,
                        Description = $"크기 불일치: {s.W}x{s.H} (같은 타입 평균: {avgW:F0}x{avgH:F0})",
                        Suggestion = "같은 타입 객체는 동일 크기 권장"
                    });
                }
            }
        }

        return issues;
    }

    private List<List<ObjectExpand>> GroupByProximity(
        List<ObjectExpand> objects, Func<ObjectExpand, int> selector, int tolerance)
    {
        var sorted = objects.OrderBy(selector).ToList();
        var groups = new List<List<ObjectExpand>>();
        var current = new List<ObjectExpand> { sorted[0] };

        for (int i = 1; i < sorted.Count; i++)
        {
            if (Math.Abs(selector(sorted[i]) - selector(sorted[i - 1])) <= tolerance)
                current.Add(sorted[i]);
            else
            {
                if (current.Count >= 2) groups.Add(current);
                current = new List<ObjectExpand> { sorted[i] };
            }
        }
        if (current.Count >= 2) groups.Add(current);
        return groups;
    }
}
```

### 3.5 TagBindingChecker

```csharp
public class TagBindingChecker : IDesignChecker
{
    public string Category => "태그 바인딩";

    public List<DesignIssue> Check(List<ObjectExpand> objects, ObjectGroup root)
    {
        var issues = new List<DesignIssue>();

        foreach (var obj in objects)
        {
            if (obj is ObjectTag tagObj)
            {
                // 태그 미바인딩 검사
                if (string.IsNullOrEmpty(tagObj.sTagName))
                {
                    issues.Add(new DesignIssue
                    {
                        Category = Category,
                        Severity = IssueSeverity.Warning,
                        ObjectName = obj.sClassName,
                        Description = "태그가 바인딩되지 않음",
                        Suggestion = "태그를 바인딩하거나 장식용 객체로 변경"
                    });
                }
                else
                {
                    // 존재하지 않는 태그 참조
                    int[] pos = null;
                    var tp = TagLib.GetStructPublic(tagObj.sTagName, ref pos);
                    if (pos != null && pos[0] == TagLib.TAG_NOT_FOUND)
                    {
                        issues.Add(new DesignIssue
                        {
                            Category = Category,
                            Severity = IssueSeverity.Error,
                            ObjectName = obj.sClassName,
                            Description = $"존재하지 않는 태그 참조: '{tagObj.sTagName}'",
                            Suggestion = "태그를 확인하고 올바른 태그로 변경"
                        });
                    }

                    // 태그 타입 불일치 (예: AnalogMeter에 DI 태그)
                    if (tp != null && pos[0] != TagLib.TAG_NOT_FOUND)
                    {
                        var expectedType = GetExpectedTagType(obj.GetType().Name);
                        if (expectedType != EnumTagType.GR && tp.enumTagType != expectedType)
                        {
                            issues.Add(new DesignIssue
                            {
                                Category = Category,
                                Severity = IssueSeverity.Warning,
                                ObjectName = obj.sClassName,
                                Description = $"태그 타입 불일치: {tp.enumTagType} " +
                                             $"(객체 권장: {expectedType})",
                                Suggestion = $"{expectedType} 타입 태그로 변경 권장"
                            });
                        }
                    }
                }
            }
        }

        // 중복 태그 바인딩 검사
        var tagUsage = objects.OfType<ObjectTag>()
            .Where(o => !string.IsNullOrEmpty(o.sTagName))
            .GroupBy(o => o.sTagName)
            .Where(g => g.Count() > 1);

        foreach (var dup in tagUsage)
        {
            // 같은 태그를 여러 출력 객체에서 사용 (의도적일 수 있지만 경고)
            var outputObjects = dup.Where(o =>
                o.GetType().Name.Contains("Button") ||
                o.GetType().Name.Contains("DigitalOut"));

            if (outputObjects.Count() > 1)
            {
                issues.Add(new DesignIssue
                {
                    Category = Category,
                    Severity = IssueSeverity.Warning,
                    ObjectName = string.Join(", ", dup.Select(o => o.sClassName)),
                    Description = $"출력 태그 '{dup.Key}'가 복수 객체에서 사용",
                    Suggestion = "의도하지 않은 중복인 경우 확인 필요"
                });
            }
        }

        return issues;
    }
}
```

### 3.6 AccessibilityChecker

```csharp
public class AccessibilityChecker : IDesignChecker
{
    public string Category => "접근성";

    public List<DesignIssue> Check(List<ObjectExpand> objects, ObjectGroup root)
    {
        var issues = new List<DesignIssue>();
        Color bgColor = root.BackgroundColor;

        foreach (var obj in objects)
        {
            // 텍스트-배경 대비율 검사 (WCAG AA 기준: 4.5:1 이상)
            if (obj is ObjectFont fontObj)
            {
                Color textColor = obj.lTextColor;
                Color objBgColor = obj.IsUseBackColor ? obj.lBackColor.basic_color : bgColor;

                double ratio = CalculateContrastRatio(textColor, objBgColor);

                if (ratio < 3.0)
                {
                    issues.Add(new DesignIssue
                    {
                        Category = Category,
                        Severity = IssueSeverity.Error,
                        ObjectName = obj.sClassName,
                        Description = $"대비율 {ratio:F1}:1 (최소 권장: 3:1)",
                        Suggestion = "텍스트 색상 또는 배경색 조정 필요"
                    });
                }
                else if (ratio < 4.5)
                {
                    issues.Add(new DesignIssue
                    {
                        Category = Category,
                        Severity = IssueSeverity.Warning,
                        ObjectName = obj.sClassName,
                        Description = $"대비율 {ratio:F1}:1 (WCAG AA 기준: 4.5:1)",
                        Suggestion = "대비율 향상 권장"
                    });
                }
            }

            // 클릭 영역 최소 크기 (터치스크린 대응)
            int width = obj.nRight - obj.nLeft;
            int height = obj.nBottom - obj.nTop;
            if (obj is ObjectTag && (width < 30 || height < 30))
            {
                issues.Add(new DesignIssue
                {
                    Category = Category,
                    Severity = IssueSeverity.Info,
                    ObjectName = obj.sClassName,
                    Description = $"터치 영역 {width}x{height}px (권장: 44x44px 이상)",
                    Suggestion = "터치스크린 사용 시 크기 확대 권장"
                });
            }
        }

        // 색약 대응: 빨강-녹색만으로 상태 구분하는 경우
        CheckColorBlindSafety(objects, issues);

        return issues;
    }

    private double CalculateContrastRatio(Color fg, Color bg)
    {
        double l1 = RelativeLuminance(fg);
        double l2 = RelativeLuminance(bg);
        double lighter = Math.Max(l1, l2);
        double darker = Math.Min(l1, l2);
        return (lighter + 0.05) / (darker + 0.05);
    }

    private double RelativeLuminance(Color c)
    {
        double r = SRGBToLinear(c.R / 255.0);
        double g = SRGBToLinear(c.G / 255.0);
        double b = SRGBToLinear(c.B / 255.0);
        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private double SRGBToLinear(double v)
    {
        return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
    }

    private void CheckColorBlindSafety(List<ObjectExpand> objects, List<DesignIssue> issues)
    {
        // 같은 위치 근처에서 빨강/녹색만으로 상태를 구분하는 객체 쌍 감지
        var redObjects = objects.Where(o => IsRedish(o.lLineColor) || IsRedish(o.lFillColor?.basic_color ?? Color.Empty));
        var greenObjects = objects.Where(o => IsGreenish(o.lLineColor) || IsGreenish(o.lFillColor?.basic_color ?? Color.Empty));

        // 근접한 빨강-녹색 쌍 존재 시 경고
        foreach (var red in redObjects)
        {
            foreach (var green in greenObjects)
            {
                if (AreNearby(red, green, 100))
                {
                    issues.Add(new DesignIssue
                    {
                        Category = Category,
                        Severity = IssueSeverity.Info,
                        ObjectName = $"{red.sClassName}, {green.sClassName}",
                        Description = "빨강-녹색 색상 조합으로 상태 구분",
                        Suggestion = "색약 사용자를 위해 형태/텍스트로 보조 구분 추가 권장"
                    });
                    break;
                }
            }
        }
    }
}
```

---

## 4. 설정 구조

```csharp
public class DesignCheckConfig
{
    // 폰트
    public int MinFontSize { get; set; } = 9;
    public string StandardFontFamily { get; set; } = null; // null = 자동 감지

    // 색상
    public int MaxUniqueColors { get; set; } = 15;
    public bool CheckISA101Colors { get; set; } = true;

    // 정렬
    public int AlignmentTolerance { get; set; } = 5;   // px
    public int SpacingTolerance { get; set; } = 8;     // px
    public int SizeTolerance { get; set; } = 10;       // px

    // 접근성
    public double MinContrastRatio { get; set; } = 3.0;
    public int MinTouchTarget { get; set; } = 44;      // px

    // 규칙 활성화
    public bool EnableFontCheck { get; set; } = true;
    public bool EnableColorCheck { get; set; } = true;
    public bool EnableAlignmentCheck { get; set; } = true;
    public bool EnableTagCheck { get; set; } = true;
    public bool EnableAccessibilityCheck { get; set; } = true;

    public static DesignCheckConfig Default => new DesignCheckConfig();
}
```

---

## 5. UI: FormDesignChecker

**파일:** `Studio/DesignCheck/FormDesignChecker.cs`

```csharp
public class FormDesignChecker : Form
{
    private TreeView treeResults;
    private RichTextBox rtbDetail;
    private Button btnAutoFix;
    private Button btnIgnore;
    private Button btnConfig;
    private ToolStrip toolFilter;
    private StatusStrip statusBar;
    private ProgressBar progressBar;

    private DesignCheckResult _result;
    private DesignCheckEngine _engine;

    public FormDesignChecker(ObjectGroup objectGroup)
    {
        InitializeComponent();
        _engine = new DesignCheckEngine();

        // 비동기 검사 실행
        RunCheckAsync(objectGroup);
    }

    private async void RunCheckAsync(ObjectGroup objectGroup)
    {
        progressBar.Visible = true;
        statusBar.Text = "검사 중...";

        _result = await Task.Run(() => _engine.CheckModule(objectGroup));

        // 결과를 TreeView에 표시
        PopulateTree(_result);

        progressBar.Visible = false;
        statusBar.Text = $"검사 완료: 오류 {_result.ErrorCount}건, " +
                         $"경고 {_result.WarningCount}건, 정보 {_result.InfoCount}건";
    }

    private void PopulateTree(DesignCheckResult result)
    {
        treeResults.Nodes.Clear();

        var categories = result.Issues.GroupBy(i => i.Category);
        foreach (var category in categories)
        {
            var icon = GetCategoryIcon(category.Max(i => i.Severity));
            var node = treeResults.Nodes.Add(
                $"{icon} {category.Key} ({category.Count()}건)");

            foreach (var issue in category)
            {
                var issueIcon = issue.Severity switch
                {
                    IssueSeverity.Error => "✕",
                    IssueSeverity.Warning => "⚠",
                    _ => "ℹ"
                };
                var child = node.Nodes.Add(
                    $"{issueIcon} {issue.ObjectName}: {issue.Description}");
                child.Tag = issue;
            }
        }

        treeResults.ExpandAll();
    }

    // 자동 수정 실행
    private void btnAutoFix_Click(object sender, EventArgs e)
    {
        var selected = treeResults.SelectedNode?.Tag as DesignIssue;
        if (selected?.AutoFix != null)
        {
            selected.AutoFix();
            // 재검사
            RunCheckAsync(_currentObjectGroup);
        }
    }
}
```

### 5.1 StudioMain 통합

```csharp
// StudioMain.cs 메뉴 추가
private void designCheckToolStripMenuItem_Click(object sender, EventArgs e)
{
    var currentModule = GetCurrentObjectGroup();
    if (currentModule != null)
    {
        var checker = new FormDesignChecker(currentModule);
        checker.ShowDialog();
    }
}
```

---

## 6. 구현 단계

### Phase 1 (1주): 규칙 기반 검사기
- FontConsistencyChecker, ColorPaletteChecker 구현
- TagBindingChecker 구현
- FormDesignChecker 기본 UI

### Phase 2 (1주): 정렬/접근성
- AlignmentChecker 구현
- AccessibilityChecker (대비율, 터치 영역)
- 자동 수정 기능

### Phase 3 (0.5주): 통합
- StudioMain 메뉴 통합
- 설정 대화상자
- 검사 결과 객체 하이라이트

---

## 7. 의존성

- **기존 코드**: ObjectExpand, ObjectFont, ObjectTag, TagLib, StudioMain
- **신규 파일**:
  - `Studio/DesignCheck/DesignCheckEngine.cs`
  - `Studio/DesignCheck/FontConsistencyChecker.cs`
  - `Studio/DesignCheck/ColorPaletteChecker.cs`
  - `Studio/DesignCheck/AlignmentChecker.cs`
  - `Studio/DesignCheck/TagBindingChecker.cs`
  - `Studio/DesignCheck/AccessibilityChecker.cs`
  - `Studio/DesignCheck/FormDesignChecker.cs`
  - `Studio/DesignCheck/DesignCheckConfig.cs`
- **외부 의존성 없음** (순수 C# 규칙 기반)

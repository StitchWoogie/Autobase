# A1. AI 기반 자동 레이아웃 & 배치 - 상세 구현 설계

## 1. 개요

SCADA 객체를 드래그&드롭 시 AI가 기존 객체 배치 패턴을 분석하여
최적 위치/정렬/간격을 제안하고, 스냅/자동 정렬 기능 제공.

**핵심 가치:** 편집 효율 50% 향상, 화면 레이아웃 일관성 확보

---

## 2. 아키텍처

```
┌─────────────────────────────────────────────────────────┐
│ Studio 편집기                                            │
│                                                         │
│  ClassEditObjectMove (객체 이동)                         │
│       │                                                 │
│       ▼                                                 │
│  LayoutSuggestionEngine (신규)                           │
│  ├─ SnapGuide        - 스냅 가이드 라인 표시             │
│  ├─ SmartAlign       - 인접 객체 기준 자동 정렬          │
│  ├─ PatternDetector  - 행/열/그리드 패턴 감지            │
│  ├─ SpacingHelper    - 균일 간격 제안                    │
│  └─ LayoutTemplate   - P&ID 배치 템플릿 (규칙 기반)     │
│       │                                                 │
│       ▼                                                 │
│  렌더링: 가이드 라인 오버레이 (ClassDraw)                │
└─────────────────────────────────────────────────────────┘
```

---

## 3. 핵심 엔진

### 3.1 LayoutSuggestionEngine

**파일:** `Studio/Layout/LayoutSuggestionEngine.cs`

```csharp
public class LayoutSuggestionEngine
{
    private List<ObjectExpand> _allObjects;
    private LayoutConfig _config;

    // 가이드 라인 결과 (렌더링용)
    public List<GuideLine> ActiveGuideLines { get; private set; }
    public Point? SuggestedPosition { get; private set; }

    public void UpdateContext(List<ObjectExpand> objects)
    {
        _allObjects = objects;
    }

    /// <summary>
    /// 객체 이동 중 실시간 호출 → 가이드 라인 + 스냅 위치 반환
    /// </summary>
    public LayoutSuggestion GetSuggestion(ObjectExpand movingObject, Point currentPos)
    {
        var suggestion = new LayoutSuggestion();
        var objRect = new Rectangle(currentPos.X, currentPos.Y,
            movingObject.nRight - movingObject.nLeft,
            movingObject.nBottom - movingObject.nTop);

        // 1. 정렬 가이드 (다른 객체의 좌/우/상/하/중심과 정렬)
        suggestion.GuideLines.AddRange(
            FindAlignmentGuides(objRect, movingObject));

        // 2. 균일 간격 스냅
        var spacingSnap = FindEqualSpacingSnap(objRect, movingObject);
        if (spacingSnap.HasValue)
        {
            suggestion.SnapPosition = spacingSnap.Value;
            suggestion.GuideLines.AddRange(
                CreateSpacingGuides(objRect, spacingSnap.Value));
        }

        // 3. 그리드 패턴 감지 (행/열 패턴이 있으면 다음 셀 제안)
        var patternSnap = DetectGridPattern(movingObject);
        if (patternSnap != null)
            suggestion.PatternSuggestion = patternSnap;

        // 최종 스냅 위치 결정 (가장 가까운 가이드)
        suggestion.FinalSnapPosition = CalculateBestSnap(
            currentPos, suggestion);

        ActiveGuideLines = suggestion.GuideLines;
        SuggestedPosition = suggestion.FinalSnapPosition;

        return suggestion;
    }

    // ═══════════════════════════════════════════
    // 정렬 가이드
    // ═══════════════════════════════════════════

    private List<GuideLine> FindAlignmentGuides(
        Rectangle moving, ObjectExpand movingObj)
    {
        var guides = new List<GuideLine>();
        int snapDist = _config.SnapDistance; // 기본 8px

        foreach (var obj in _allObjects)
        {
            if (obj == movingObj) continue;

            int oLeft = obj.nLeft, oRight = obj.nRight;
            int oTop = obj.nTop, oBottom = obj.nBottom;
            int oCenterX = (oLeft + oRight) / 2;
            int oCenterY = (oTop + oBottom) / 2;

            int mCenterX = moving.X + moving.Width / 2;
            int mCenterY = moving.Y + moving.Height / 2;

            // 좌변 정렬
            if (Math.Abs(moving.X - oLeft) <= snapDist)
                guides.Add(new GuideLine(oLeft, GuideType.VerticalLeft));

            // 우변 정렬
            if (Math.Abs(moving.Right - oRight) <= snapDist)
                guides.Add(new GuideLine(oRight, GuideType.VerticalRight));

            // 상변 정렬
            if (Math.Abs(moving.Y - oTop) <= snapDist)
                guides.Add(new GuideLine(oTop, GuideType.HorizontalTop));

            // 하변 정렬
            if (Math.Abs(moving.Bottom - oBottom) <= snapDist)
                guides.Add(new GuideLine(oBottom, GuideType.HorizontalBottom));

            // 수평 중심 정렬
            if (Math.Abs(mCenterX - oCenterX) <= snapDist)
                guides.Add(new GuideLine(oCenterX, GuideType.VerticalCenter));

            // 수직 중심 정렬
            if (Math.Abs(mCenterY - oCenterY) <= snapDist)
                guides.Add(new GuideLine(oCenterY, GuideType.HorizontalCenter));
        }

        return guides;
    }

    // ═══════════════════════════════════════════
    // 균일 간격
    // ═══════════════════════════════════════════

    private Point? FindEqualSpacingSnap(
        Rectangle moving, ObjectExpand movingObj)
    {
        // 수평 방향: 좌측 인접 객체와 우측 인접 객체 사이 균일 간격
        var leftNeighbor = _allObjects
            .Where(o => o != movingObj && o.nRight < moving.X)
            .OrderByDescending(o => o.nRight)
            .FirstOrDefault();

        var rightNeighbor = _allObjects
            .Where(o => o != movingObj && o.nLeft > moving.Right)
            .OrderBy(o => o.nLeft)
            .FirstOrDefault();

        if (leftNeighbor != null && rightNeighbor != null)
        {
            // 두 이웃 사이에서 균일 간격 위치 계산
            int leftGap = moving.X - leftNeighbor.nRight;
            int rightGap = rightNeighbor.nLeft - moving.Right;
            int avgGap = (leftGap + rightGap) / 2;

            int snapX = leftNeighbor.nRight + avgGap;
            if (Math.Abs(snapX - moving.X) <= _config.SnapDistance * 2)
            {
                return new Point(snapX, moving.Y);
            }
        }

        return null;
    }

    // ═══════════════════════════════════════════
    // 그리드 패턴 감지
    // ═══════════════════════════════════════════

    private PatternSuggestion DetectGridPattern(ObjectExpand movingObj)
    {
        // 같은 타입 객체들의 배치 패턴 분석
        var sameType = _allObjects
            .Where(o => o.GetType() == movingObj.GetType() && o != movingObj)
            .OrderBy(o => o.nTop).ThenBy(o => o.nLeft)
            .ToList();

        if (sameType.Count < 2) return null;

        // 행 패턴 감지
        var rows = GroupByY(sameType, _config.SnapDistance);
        if (rows.Count >= 1 && rows[0].Count >= 2)
        {
            var row = rows[0];
            int stepX = row.Count >= 2
                ? row[1].nLeft - row[0].nLeft : 0;
            int stepY = rows.Count >= 2
                ? rows[1][0].nTop - rows[0][0].nTop : 0;

            if (stepX > 0)
            {
                // 다음 위치 제안
                var lastInRow = row.Last();
                return new PatternSuggestion
                {
                    NextPosition = new Point(
                        lastInRow.nLeft + stepX, lastInRow.nTop),
                    PatternType = "grid",
                    Description = $"그리드 패턴 감지 (간격: {stepX}x{stepY}px)"
                };
            }
        }

        return null;
    }
}

public class LayoutSuggestion
{
    public List<GuideLine> GuideLines { get; set; } = new List<GuideLine>();
    public Point? SnapPosition { get; set; }
    public Point? FinalSnapPosition { get; set; }
    public PatternSuggestion PatternSuggestion { get; set; }
}

public class GuideLine
{
    public int Position { get; set; }
    public GuideType Type { get; set; }
    public Color Color { get; set; } = Color.FromArgb(100, 0, 120, 255);

    public GuideLine(int pos, GuideType type) { Position = pos; Type = type; }
}

public enum GuideType
{
    VerticalLeft, VerticalRight, VerticalCenter,
    HorizontalTop, HorizontalBottom, HorizontalCenter,
    Spacing
}

public class PatternSuggestion
{
    public Point NextPosition { get; set; }
    public string PatternType { get; set; }
    public string Description { get; set; }
}

public class LayoutConfig
{
    public int SnapDistance { get; set; } = 8;
    public bool EnableSnapGuides { get; set; } = true;
    public bool EnableEqualSpacing { get; set; } = true;
    public bool EnablePatternDetection { get; set; } = true;
    public Color GuideColor { get; set; } = Color.FromArgb(100, 0, 120, 255);
}
```

### 3.2 ClassEditObjectMove 통합

```csharp
// ClassEditObjectMove.cs에 추가
private LayoutSuggestionEngine _layoutEngine;

// 객체 이동 중 (마우스 이동 이벤트)
private void OnObjectMoving(ObjectExpand obj, Point mousePos)
{
    if (_layoutEngine != null && TotalConfig.bUseSmartLayout)
    {
        var suggestion = _layoutEngine.GetSuggestion(obj, mousePos);

        // 스냅 적용
        if (suggestion.FinalSnapPosition.HasValue)
        {
            obj.nLeft = suggestion.FinalSnapPosition.Value.X;
            obj.nTop = suggestion.FinalSnapPosition.Value.Y;
        }

        // 가이드 라인 렌더링 요청
        Invalidate(); // 다시 그리기
    }
}

// 가이드 라인 렌더링 (OnPaint에서 호출)
private void DrawGuideLines(Graphics g)
{
    if (_layoutEngine?.ActiveGuideLines == null) return;

    using (var pen = new Pen(Color.FromArgb(100, 0, 120, 255), 1))
    {
        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

        foreach (var guide in _layoutEngine.ActiveGuideLines)
        {
            switch (guide.Type)
            {
                case GuideType.VerticalLeft:
                case GuideType.VerticalRight:
                case GuideType.VerticalCenter:
                    g.DrawLine(pen, guide.Position, 0,
                              guide.Position, ClientSize.Height);
                    break;
                case GuideType.HorizontalTop:
                case GuideType.HorizontalBottom:
                case GuideType.HorizontalCenter:
                    g.DrawLine(pen, 0, guide.Position,
                              ClientSize.Width, guide.Position);
                    break;
            }
        }
    }
}
```

---

## 4. 구현 단계

### Phase 1 (1주): 스냅 가이드
- 정렬 가이드 라인 (좌/우/상/하/중심)
- ClassEditObjectMove 통합
- 가이드 라인 렌더링

### Phase 2 (1주): 균일 간격 + 패턴
- 균일 간격 스냅
- 그리드 패턴 감지
- 설정 UI

---

## 5. 의존성

- **기존 코드**: ClassEditObjectMove, ClassDraw, ObjectExpand
- **신규 파일**:
  - `Studio/Layout/LayoutSuggestionEngine.cs`
  - `Studio/Layout/LayoutConfig.cs`
- **외부 의존성 없음** (순수 C# 기하학 계산)

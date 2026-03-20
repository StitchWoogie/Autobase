# A2-2. 태그 자동 바인딩 제안 - 상세 구현 설계

## 1. 개요

SCADA 화면에 객체를 배치하면 AI가 객체의 타입/이름/설명을 분석하여 적합한 태그를 자동 추천.
수천 개 태그가 있는 대형 프로젝트에서 태그 검색/선택 시간을 대폭 단축.

**핵심 가치:** 태그 바인딩 작업 시간 80% 단축, 바인딩 오류 감소

---

## 2. 아키텍처

```
┌────────────────────────────────────────────────────────────┐
│ Studio 편집기                                               │
│                                                            │
│  객체 배치/속성 편집                                        │
│       │                                                    │
│       ▼                                                    │
│  PropertyPageTag (태그 속성 페이지)                          │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ 태그: [________________] [찾기] [AI 추천 ▼]          │  │
│  │ 설명: [보일러1 출구 온도]                              │  │
│  │                                                      │  │
│  │ ┌── AI 추천 태그 ──────────────────────────────────┐ │  │
│  │ │ ★★★ Boiler1_OutTemp     보일러1 출구온도 (AI)   │ │  │
│  │ │ ★★☆ Boiler1_InTemp      보일러1 입구온도 (AI)   │ │  │
│  │ │ ★★☆ Boiler2_OutTemp     보일러2 출구온도 (AI)   │ │  │
│  │ │ ★☆☆ HeatExch_Temp       열교환기 온도 (AI)      │ │  │
│  │ └──────────────────────────────────────────────────┘ │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────┘
         │
         ▼
┌──────────────────────────────────────┐
│ TagBindingSuggester (C# 엔진)        │
│                                      │
│ - 객체 타입 → 태그 타입 매핑         │
│ - 텍스트 유사도 (Levenshtein/TF-IDF) │
│ - 컨텍스트 분석 (같은 화면 태그)     │
│ - 이력 학습 (사용자 선택 패턴)       │
└──────────────────────────────────────┘
```

---

## 3. 태그 추천 알고리즘

### 3.1 점수 계산 모델

```
최종 점수 = W1×타입매칭 + W2×이름유사도 + W3×설명유사도 + W4×컨텍스트 + W5×이력보너스

가중치:
  W1 = 0.30 (타입 매칭)
  W2 = 0.25 (이름 유사도)
  W3 = 0.25 (설명 유사도)
  W4 = 0.10 (컨텍스트)
  W5 = 0.10 (이력)
```

### 3.2 타입 매칭 (W1)

```
객체 타입 → 허용 태그 타입 매핑:

ObjectAnalogRectangle    → AI (1.0)
ObjectAnalogRotate       → AI (1.0)
ObjectAnalogMeter        → AI (1.0)
ObjectAnalogGauge        → AI (1.0)
ObjectAnalogString       → AI (1.0), ST (0.5)
ObjectAnalogStatus       → AI (1.0)

ObjectDigitalRectangle   → DI (1.0)
ObjectDigitalCircle      → DI (1.0)
ObjectDigitalAnimation   → DI (1.0)
ObjectDigitalString      → DI (1.0), ST (0.5)

ObjectButtonDigitalOut   → DO (1.0)

ObjectChangeValueDisplay → AI (0.8), AO (0.8), DI (0.5), DO (0.5), ST (0.5)

ObjectControlEditBox     → AI (0.7), AO (0.7), DI (0.5), DO (0.5), ST (0.7)
ObjectControlComboBox    → DI (0.8), DO (0.8), ST (0.5)
ObjectControlCheckBox    → DI (0.9), DO (0.9)

ObjectStringString       → ST (1.0)
```

### 3.3 이름 유사도 (W2)

```csharp
// 다단계 유사도 계산
private double CalculateNameSimilarity(string objectName, string tagName)
{
    // 1. 정규화: 대소문자, 구분자 통일
    string normObj = Normalize(objectName);   // "AnalogMeter_Boiler1Temp" → "boiler1 temp"
    string normTag = Normalize(tagName);       // "Boiler1_OutTemp" → "boiler1 outtemp"

    // 2. 토큰 분리
    var objTokens = Tokenize(normObj);  // ["boiler1", "temp"]
    var tagTokens = Tokenize(normTag);  // ["boiler1", "outtemp"]

    // 3. 토큰 매칭 점수 (Jaccard + 부분 매칭)
    double tokenScore = CalculateTokenOverlap(objTokens, tagTokens);

    // 4. 편집 거리 기반 보정
    double editScore = 1.0 - (double)LevenshteinDistance(normObj, normTag)
                        / Math.Max(normObj.Length, normTag.Length);

    return Math.Max(tokenScore, editScore);
}

private string Normalize(string input)
{
    // CamelCase 분리: "Boiler1OutTemp" → "Boiler1 Out Temp"
    string result = Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    // 구분자 통일: "_", "-", "." → " "
    result = Regex.Replace(result, "[_\\-\\.]", " ");
    return result.ToLower().Trim();
}

private string[] Tokenize(string normalized)
{
    return normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
}

private double CalculateTokenOverlap(string[] tokens1, string[] tokens2)
{
    int matched = 0;
    foreach (var t1 in tokens1)
    {
        foreach (var t2 in tokens2)
        {
            if (t2.Contains(t1) || t1.Contains(t2))
            {
                matched++;
                break;
            }
        }
    }
    int total = Math.Max(tokens1.Length, tokens2.Length);
    return total > 0 ? (double)matched / total : 0;
}
```

### 3.4 설명 유사도 (W3)

```csharp
private double CalculateDescriptionSimilarity(string objDescription, string tagDescription)
{
    if (string.IsNullOrEmpty(objDescription) || string.IsNullOrEmpty(tagDescription))
        return 0;

    // 한글 형태소 단위 토큰 비교 (간단 구현)
    var objWords = ExtractKoreanWords(objDescription);
    var tagWords = ExtractKoreanWords(tagDescription);

    // 공통 단어 비율
    var intersection = objWords.Intersect(tagWords).Count();
    var union = objWords.Union(tagWords).Count();

    return union > 0 ? (double)intersection / union : 0;
}

private HashSet<string> ExtractKoreanWords(string text)
{
    // 2자 이상 한글 단어 추출
    var matches = Regex.Matches(text, "[가-힣]{2,}");
    var words = new HashSet<string>();
    foreach (Match m in matches)
        words.Add(m.Value);
    return words;
}
```

### 3.5 컨텍스트 점수 (W4)

```csharp
private double CalculateContextScore(string tagName, List<string> screenTags,
                                     string currentModule)
{
    double score = 0;

    // 같은 화면에 있는 다른 객체의 태그와 같은 그룹인지
    foreach (var existingTag in screenTags)
    {
        string existingGroup = GetTagGroup(existingTag);  // "Boiler1/"
        string candidateGroup = GetTagGroup(tagName);      // "Boiler1/"
        if (existingGroup == candidateGroup)
        {
            score += 0.3;
            break;
        }
    }

    // 모듈 이름과 태그 이름의 연관성
    string moduleName = Path.GetFileNameWithoutExtension(currentModule);
    if (Normalize(tagName).Contains(Normalize(moduleName)))
        score += 0.2;

    return Math.Min(score, 1.0);
}
```

### 3.6 이력 보너스 (W5)

```csharp
// 사용자가 과거에 선택한 패턴 학습
private static Dictionary<string, List<string>> _bindingHistory
    = new Dictionary<string, List<string>>();

private double CalculateHistoryBonus(string objectType, string tagName)
{
    string key = objectType; // 예: "ObjectAnalogMeter"
    if (_bindingHistory.TryGetValue(key, out var history))
    {
        // 같은 타입 객체에 이전에 바인딩된 태그와의 패턴 유사도
        foreach (var prevTag in history.TakeLast(20))
        {
            string prevGroup = GetTagGroup(prevTag);
            string candGroup = GetTagGroup(tagName);
            if (prevGroup == candGroup)
                return 0.5;
        }
    }
    return 0;
}

// 사용자가 태그 선택 시 이력 기록
public static void RecordBinding(string objectType, string tagName)
{
    if (!_bindingHistory.ContainsKey(objectType))
        _bindingHistory[objectType] = new List<string>();
    _bindingHistory[objectType].Add(tagName);

    // 최대 100개 이력 유지
    if (_bindingHistory[objectType].Count > 100)
        _bindingHistory[objectType].RemoveAt(0);
}
```

---

## 4. C# 구현

### 4.1 TagBindingSuggester (핵심 엔진)

**파일:** `Studio/Property/TagBindingSuggester.cs`

```csharp
public class TagBindingSuggester
{
    // 가중치 상수
    private const double W_TYPE = 0.30;
    private const double W_NAME = 0.25;
    private const double W_DESC = 0.25;
    private const double W_CONTEXT = 0.10;
    private const double W_HISTORY = 0.10;

    // 최대 추천 수
    private const int MAX_SUGGESTIONS = 10;

    // 타입 매칭 테이블
    private static readonly Dictionary<string, Dictionary<EnumTagType, double>> TypeMatchTable
        = new Dictionary<string, Dictionary<EnumTagType, double>>
    {
        ["ObjectAnalogRectangle"] = new Dictionary<EnumTagType, double>
            { { EnumTagType.AI, 1.0 } },
        ["ObjectAnalogMeter"] = new Dictionary<EnumTagType, double>
            { { EnumTagType.AI, 1.0 } },
        ["ObjectAnalogGauge"] = new Dictionary<EnumTagType, double>
            { { EnumTagType.AI, 1.0 } },
        ["ObjectDigitalAnimation"] = new Dictionary<EnumTagType, double>
            { { EnumTagType.DI, 1.0 } },
        ["ObjectDigitalRectangle"] = new Dictionary<EnumTagType, double>
            { { EnumTagType.DI, 1.0 } },
        ["ObjectButtonDigitalOut"] = new Dictionary<EnumTagType, double>
            { { EnumTagType.DO, 1.0 } },
        ["ObjectStringString"] = new Dictionary<EnumTagType, double>
            { { EnumTagType.ST, 1.0 } },
        ["ObjectChangeValueDisplay"] = new Dictionary<EnumTagType, double>
            { { EnumTagType.AI, 0.8 }, { EnumTagType.AO, 0.8 },
              { EnumTagType.DI, 0.5 }, { EnumTagType.DO, 0.5 },
              { EnumTagType.ST, 0.5 } },
    };

    // 바인딩 이력
    private static Dictionary<string, List<string>> _bindingHistory
        = new Dictionary<string, List<string>>();

    /// <summary>
    /// 객체에 적합한 태그 목록을 점수순으로 반환
    /// </summary>
    public List<TagSuggestion> Suggest(
        ObjectTag targetObject,
        TagGrClass tagRoot,
        List<string> screenTags,
        string currentModule)
    {
        string objectTypeName = targetObject.GetType().Name;
        string objectClassName = targetObject.sClassName ?? "";
        string objectDescription = targetObject.sObjectDescription ?? "";

        // 모든 태그를 플랫 리스트로 수집
        var allTags = new List<TagPublicClass>();
        CollectAllTags(tagRoot, allTags);

        // 각 태그에 대해 점수 계산
        var scored = new List<TagSuggestion>();

        foreach (var tag in allTags)
        {
            // 그룹 태그 제외
            if (tag.enumTagType == EnumTagType.GR) continue;

            double score = CalculateScore(
                objectTypeName, objectClassName, objectDescription,
                tag, screenTags, currentModule);

            if (score > 0.1) // 최소 임계값
            {
                scored.Add(new TagSuggestion
                {
                    TagName = tag.tag,
                    TagDescription = tag.description,
                    TagType = tag.enumTagType,
                    Score = score,
                    Stars = ScoreToStars(score)
                });
            }
        }

        // 점수 내림차순 정렬, 상위 N개 반환
        return scored
            .OrderByDescending(s => s.Score)
            .Take(MAX_SUGGESTIONS)
            .ToList();
    }

    private double CalculateScore(
        string objectType, string objectName, string objectDesc,
        TagPublicClass tag, List<string> screenTags, string module)
    {
        double typeScore = GetTypeMatchScore(objectType, tag.enumTagType);
        double nameScore = CalculateNameSimilarity(objectName, tag.name);
        double descScore = CalculateDescriptionSimilarity(objectDesc, tag.description);
        double ctxScore = CalculateContextScore(tag.tag, screenTags, module);
        double histScore = CalculateHistoryBonus(objectType, tag.tag);

        return W_TYPE * typeScore
             + W_NAME * nameScore
             + W_DESC * descScore
             + W_CONTEXT * ctxScore
             + W_HISTORY * histScore;
    }

    private double GetTypeMatchScore(string objectType, EnumTagType tagType)
    {
        if (TypeMatchTable.TryGetValue(objectType, out var typeMap))
        {
            if (typeMap.TryGetValue(tagType, out double score))
                return score;
            return 0;
        }
        // 알 수 없는 객체 타입은 모든 태그 타입 허용 (낮은 점수)
        return 0.3;
    }

    private int ScoreToStars(double score)
    {
        if (score >= 0.7) return 3;
        if (score >= 0.4) return 2;
        return 1;
    }

    private void CollectAllTags(TagGrClass group, List<TagPublicClass> result)
    {
        foreach (var item in group.arrayTag)
        {
            if (item is TagGrClass subGroup)
            {
                CollectAllTags(subGroup, result);
            }
            else if (item is TagPublicClass tag)
            {
                result.Add(tag);
            }
        }
    }

    // 사용자 선택 기록
    public static void RecordUserSelection(string objectType, string tagName)
    {
        if (!_bindingHistory.ContainsKey(objectType))
            _bindingHistory[objectType] = new List<string>();

        _bindingHistory[objectType].Add(tagName);

        if (_bindingHistory[objectType].Count > 100)
            _bindingHistory[objectType].RemoveAt(0);
    }

    // ... CalculateNameSimilarity, CalculateDescriptionSimilarity 등
    // (섹션 3.3~3.6의 구현 재사용)
}

public class TagSuggestion
{
    public string TagName { get; set; }
    public string TagDescription { get; set; }
    public EnumTagType TagType { get; set; }
    public double Score { get; set; }
    public int Stars { get; set; }  // 1~3
}
```

### 4.2 PropertyPageTag 수정

**파일:** `Studio/Property/PropertyPageTag.cs` (기존 수정)

```csharp
// PropertyPageTag에 추가할 필드/메서드

private Button btnAiSuggest;
private ListBox lstSuggestions;
private TagBindingSuggester _suggester;
private ObjectTag _targetObject;

// 생성자 또는 초기화에서
private void InitAiSuggestion()
{
    _suggester = new TagBindingSuggester();

    btnAiSuggest = new Button();
    btnAiSuggest.Text = "AI 추천";
    btnAiSuggest.Size = new Size(70, 23);
    btnAiSuggest.Click += btnAiSuggest_Click;

    lstSuggestions = new ListBox();
    lstSuggestions.Size = new Size(380, 120);
    lstSuggestions.DrawMode = DrawMode.OwnerDrawFixed;
    lstSuggestions.ItemHeight = 22;
    lstSuggestions.DrawItem += lstSuggestions_DrawItem;
    lstSuggestions.DoubleClick += lstSuggestions_DoubleClick;
    lstSuggestions.Visible = false;

    this.Controls.Add(btnAiSuggest);
    this.Controls.Add(lstSuggestions);
}

// 대상 객체 설정
public void SetTargetObject(ObjectTag obj)
{
    _targetObject = obj;
}

// AI 추천 버튼 클릭
private void btnAiSuggest_Click(object sender, EventArgs e)
{
    if (_targetObject == null) return;

    // 현재 화면의 기존 태그 목록 수집
    var screenTags = CollectScreenTags();
    string currentModule = TotalConfig.sCurrentModule;

    // 추천 실행
    var suggestions = _suggester.Suggest(
        _targetObject,
        TagLib.groupRoot,
        screenTags,
        currentModule);

    // 결과 표시
    lstSuggestions.Items.Clear();
    foreach (var s in suggestions)
    {
        lstSuggestions.Items.Add(s);
    }
    lstSuggestions.Visible = true;
}

// 추천 항목 커스텀 렌더링 (별점 표시)
private void lstSuggestions_DrawItem(object sender, DrawItemEventArgs e)
{
    if (e.Index < 0) return;
    e.DrawBackground();

    var suggestion = (TagSuggestion)lstSuggestions.Items[e.Index];
    string stars = new string('★', suggestion.Stars) +
                   new string('☆', 3 - suggestion.Stars);
    string display = $"{stars} {suggestion.TagName}  ({suggestion.TagType}) " +
                     $"- {suggestion.TagDescription}";

    var brush = (e.State & DrawItemState.Selected) != 0
        ? Brushes.White : Brushes.Black;
    e.Graphics.DrawString(display, e.Font, brush, e.Bounds.Location);
    e.DrawFocusRectangle();
}

// 추천 항목 더블클릭 → 태그 적용
private void lstSuggestions_DoubleClick(object sender, EventArgs e)
{
    if (lstSuggestions.SelectedItem is TagSuggestion selected)
    {
        SetTag(selected.TagName);
        TagBindingSuggester.RecordUserSelection(
            _targetObject.GetType().Name, selected.TagName);
        lstSuggestions.Visible = false;
    }
}

// 현재 화면의 태그 목록 수집
private List<string> CollectScreenTags()
{
    var tags = new List<string>();
    // ObjectGroup에서 모든 ObjectTag의 sTagName 수집
    // (ClassEditProperty에서 접근 가능한 현재 모듈의 객체 목록)
    return tags;
}
```

### 4.3 ClassEditProperty 통합

**파일:** `Studio/Property/ClassEditProperty.cs` (기존 수정)

```csharp
// PropertyPageTag 생성 시 대상 객체 전달
private static void AddPageTagWithSuggestion(
    PropertySheetPublic propertySheet,
    PropertyPageTag tagPage,
    ObjectTag obj)
{
    tagPage.SetTargetObject(obj);
    // 기존 AddPageTag 로직 유지
    propertySheet.AddPageTag(tagPage, obj);
}
```

---

## 5. 성능 최적화

### 5.1 태그 인덱스 캐싱

```csharp
public class TagSearchIndex
{
    // 토큰 → 태그 매핑 (역 인덱스)
    private Dictionary<string, List<TagPublicClass>> _tokenIndex;

    public void Build(TagGrClass tagRoot)
    {
        _tokenIndex = new Dictionary<string, List<TagPublicClass>>();
        var allTags = new List<TagPublicClass>();
        CollectAllTags(tagRoot, allTags);

        foreach (var tag in allTags)
        {
            var tokens = Tokenize(Normalize(tag.name));
            foreach (var token in tokens)
            {
                if (!_tokenIndex.ContainsKey(token))
                    _tokenIndex[token] = new List<TagPublicClass>();
                _tokenIndex[token].Add(tag);
            }

            // 설명도 인덱싱
            if (!string.IsNullOrEmpty(tag.description))
            {
                var descTokens = ExtractKoreanWords(tag.description);
                foreach (var dt in descTokens)
                {
                    if (!_tokenIndex.ContainsKey(dt))
                        _tokenIndex[dt] = new List<TagPublicClass>();
                    _tokenIndex[dt].Add(tag);
                }
            }
        }
    }

    // 후보 태그 빠른 필터링 (전체 스캔 대신)
    public List<TagPublicClass> GetCandidates(string objectName, string objectDesc)
    {
        var candidates = new HashSet<TagPublicClass>();
        var queryTokens = Tokenize(Normalize(objectName));

        foreach (var token in queryTokens)
        {
            if (_tokenIndex.TryGetValue(token, out var tags))
                foreach (var t in tags) candidates.Add(t);

            // 부분 매칭
            foreach (var key in _tokenIndex.Keys)
            {
                if (key.Contains(token) || token.Contains(key))
                    foreach (var t in _tokenIndex[key]) candidates.Add(t);
            }
        }

        return candidates.ToList();
    }
}
```

### 5.2 비동기 추천 (대규모 태그)

```csharp
// 태그가 10,000개 이상인 경우 백그라운드 계산
private async void btnAiSuggest_Click(object sender, EventArgs e)
{
    btnAiSuggest.Enabled = false;
    lblStatus.Text = "추천 태그 검색 중...";

    var suggestions = await Task.Run(() =>
        _suggester.Suggest(_targetObject, TagLib.groupRoot,
                          CollectScreenTags(), TotalConfig.sCurrentModule));

    lstSuggestions.Items.Clear();
    foreach (var s in suggestions)
        lstSuggestions.Items.Add(s);
    lstSuggestions.Visible = true;

    btnAiSuggest.Enabled = true;
    lblStatus.Text = $"{suggestions.Count}개 태그 추천됨";
}
```

---

## 6. 자동 추천 트리거

### 6.1 객체 배치 시 자동 팝업 (선택적)

```csharp
// ClassEditInsert.cs에서 객체 배치 완료 시
private void OnObjectInserted(ObjectTag newObject)
{
    // 설정에서 자동 추천 활성화 확인
    if (TotalConfig.bAutoTagSuggestion)
    {
        // 속성 대화상자 열기 + AI 추천 자동 표시
        ClassEditProperty.PropertyWithAutoSuggest(newObject);
    }
}
```

### 6.2 태그 입력 필드에서 실시간 필터링

```csharp
// PropertyPageTag의 텍스트 입력 시 실시간 추천
private void textBoxTag_TextChanged(object sender, EventArgs e)
{
    string partial = textBoxTag.Text;
    if (partial.Length < 2) return;

    // 디바운싱 (300ms)
    _debounceTimer?.Stop();
    _debounceTimer = new Timer();
    _debounceTimer.Interval = 300;
    _debounceTimer.Tick += (s, ev) =>
    {
        _debounceTimer.Stop();
        ShowFilteredSuggestions(partial);
    };
    _debounceTimer.Start();
}
```

---

## 7. 구현 단계

### Phase 1 (1주): 기본 추천 엔진
- TagBindingSuggester 클래스 구현
- 타입 매칭 + 이름 유사도
- PropertyPageTag에 AI 추천 버튼/리스트 추가

### Phase 2 (1주): 고도화
- 설명 유사도 (한글 토큰 매칭)
- 컨텍스트 점수 (같은 화면 태그 그룹)
- 이력 학습
- TagSearchIndex 캐싱

### Phase 3 (0.5주): UX 개선
- 자동 추천 트리거
- 실시간 필터링
- 별점 시각화

---

## 8. 의존성

- **기존 코드**: PropertyPageTag.cs, ClassEditProperty.cs, TagLib, TagGrClass
- **신규 파일**:
  - `Studio/Property/TagBindingSuggester.cs`
  - `Studio/Property/TagSearchIndex.cs`
- **외부 의존성 없음** (LLM 불필요, 순수 C# 텍스트 매칭)

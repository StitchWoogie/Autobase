# Autobase SCADA UI 렌더링 분리 설계안

## 현재 아키텍처 분석

### 상속 구조
```
ObjectPublic → ObjectFont → ObjectExpand → 각 Object (ObjectRectangle, ObjectLine, ObjectBitmap 등 76개)
                                         → ObjectPublicGroupLayer (그룹 컨테이너)
```

### 현재 문제점 - 강결합 구조
`ObjectExpand` 클래스(약 3500줄)가 아래 기능을 **모두** 한 곳에서 처리:

1. **위치/크기 계산** (`ExpandCalcObjectRect`) - nViewX1/Y1/X2/Y2 계산
2. **확장기능 실행** (`ExpandRunAsync`) - 스크립트 실행, 색상변경, 크기변경, 위치변경, 회전, 점멸, Visible 등
3. **UI 표시** (`Display` → `DisplayObject`) - GDI+ Graphics로 그리기
4. **이벤트 처리** - 마우스 이벤트, 슬라이더

### 타이머 기반 업데이트 흐름
```
FormGraphicChild.FormGraphic_EventTimer()
  → ObjectRoot.EventTimerAsync()
    → ObjectPublicGroupLayer.EventTimerAsync()  ← 모든 오브젝트를 순회
      → 각 ObjectExpand.EventTimerAsync()
        → ExpandRunAsync()         ← 스크립트 실행 + 값 계산 (async)
        → ExpandCalcObjectRect()   ← 위치/크기 재계산
        → form.Invalidate()       ← UI 갱신 트리거
```

**Paint 흐름:**
```
FormGraphicChild.FormGraphic_Paint()
  → ObjectRoot.Display()
    → groupRoot.Display()
      → 각 ObjectExpand.Display()   ← 매번 모든 오브젝트가 GDI+ 그리기 수행
```

---

## 방법 1: 정적 요소 Bitmap 캡처 (Static Layer Caching)

### 개념
확장기능을 사용하지 않는 오브젝트들(`eID.active == 0`)을 하나의 Bitmap으로 캡처하여 배경처럼 사용.

### 구현 계획

#### 1단계: ObjectRoot에 Static/Dynamic 레이어 분리
- `ObjectRoot`에 `Bitmap staticLayerBitmap` 필드 추가
- `bool staticLayerDirty` 플래그로 재캡처 필요 여부 관리

#### 2단계: 오브젝트 분류 로직
`ObjectPublicGroupLayer`에서 오브젝트를 분류:
```csharp
bool IsStaticObject(ObjectExpand obj)
{
    return obj.eID.active == 0;  // 확장기능 미사용
}
```

#### 3단계: Static Layer 렌더링
```csharp
// ObjectRoot.Display() 수정
void RenderStaticLayer(Graphics gScreen, Rectangle rcScreen)
{
    if (staticLayerBitmap == null || staticLayerDirty)
    {
        // Bitmap 생성 (모듈 크기 기준)
        staticLayerBitmap = new Bitmap(moduleWidth, moduleHeight);
        using (var g = Graphics.FromImage(staticLayerBitmap))
        {
            // 정적 오브젝트만 그리기
            groupRoot.DisplayStaticObjects(g, rcPaint, 0, 0);
        }
        staticLayerDirty = false;
    }
    // 스케일 적용하여 그리기
    gScreen.DrawImage(staticLayerBitmap, destRect, srcRect, GraphicsUnit.Pixel);
}
```

#### 4단계: Display 흐름 변경
```
1. 배경색 칠하기
2. 배경 비트맵 그리기 (기존)
3. ★ staticLayerBitmap 그리기 (NEW - 정적 오브젝트 캐시)
4. 동적 오브젝트만 순회하여 그리기
```

#### 5단계: 확대/축소 지원
- `SetScreenSize()`, `SetOpticRate()`, `SetBasePoint()` 호출 시 `staticLayerDirty = true` 설정
- 스크롤 시에는 Bitmap의 그리기 offset만 변경 (재캡처 불필요)

### 장점
- 정적 오브젝트 100개 → DrawImage 1회로 축소
- 기존 코드 변경 최소화
- 모듈 확대/축소 시 dirty 플래그로 재캡처

### 단점/리스크
- 메모리 사용 증가 (큰 모듈의 경우 Bitmap 크기)
- 확대/축소 시 재캡처 비용
- 정적/동적 오브젝트의 Z-order 혼재 시 처리 복잡

---

## 방법 2: ObjectExpand 계산 로직과 UI 갱신 분리

### 개념
현재 `EventTimerAsync()` 안에서 계산 + Invalidate가 동기적으로 일어나는 것을 분리하여:
- **계산 단계**: 모든 오브젝트의 스크립트 실행 및 값 계산 (별도 스레드 가능)
- **UI 갱신 단계**: 변경된 오브젝트만 Invalidate

### 구현 계획

#### 1단계: ExpandState 데이터 클래스 도입
```csharp
// 새 파일: ExpandState.cs
public class ExpandState
{
    public int LocationX;
    public int LocationY;
    public float PercentWidth;
    public float PercentHeight;
    public bool Visible;
    public int BlinkingCycle;
    public int AnimationSpeed;
    public int ThickLine;
    public Color ColorLine;
    public Color ColorFill;
    public Color ColorText;
    public Color ColorBack;
    public float RotationAngle;
    public bool HasChanged;
    public EnumRunChangedType ChangedType;
}
```

#### 2단계: ExpandRunAsync를 순수 계산으로 분리
```csharp
// ObjectExpand에 추가
public async Task<ExpandState> CalculateExpandStateAsync()
{
    // ExpandRunAsync의 계산 로직만 추출
    // form 참조 없이 순수 데이터 계산
    // UI thread 호출 (form.Invoke) 제거
    var state = new ExpandState();
    // ... 스크립트 실행 및 값 계산 ...
    return state;
}
```

#### 3단계: UI 적용 메서드 분리
```csharp
// ObjectExpand에 추가
public void ApplyExpandState(ExpandState state, Form form)
{
    // UI thread에서 실행
    if (!state.HasChanged) return;

    // 값 적용
    nRunLocationX = state.LocationX;
    // ...

    // Invalidate 처리
    if ((state.ChangedType & EnumRunChangedType.Rotation) > 0)
        form.Invalidate();
    else
        InvalidateObject(form);
}
```

#### 4단계: EventTimerAsync 리팩터링
```csharp
// ObjectPublicGroupLayer.EventTimerAsync 수정
public override async Task EventTimerAsync(Form form)
{
    // Phase 1: 모든 오브젝트의 상태 계산 (병렬 가능)
    var tasks = new List<Task<(ObjectExpand obj, ExpandState state)>>();
    for (int l = 0; l < objectList.Count; l++)
    {
        var obj = (ObjectExpand)objectList[l];
        tasks.Add(Task.Run(async () => (obj, await obj.CalculateExpandStateAsync())));
    }
    var results = await Task.WhenAll(tasks);

    // Phase 2: UI 갱신 (UI thread에서 순차 적용)
    foreach (var (obj, state) in results)
    {
        obj.ApplyExpandState(state, form);
    }
}
```

#### 5단계: Dirty Region 최적화
```csharp
// 변경된 오브젝트 영역만 합산하여 한 번에 Invalidate
Rectangle dirtyRegion = Rectangle.Empty;
foreach (var (obj, state) in results)
{
    if (state.HasChanged)
    {
        dirtyRegion = Rectangle.Union(dirtyRegion, obj.GetViewRect());
        obj.ApplyExpandState(state, form);
    }
}
if (!dirtyRegion.IsEmpty)
    form.Invalidate(dirtyRegion);
```

### 장점
- 계산을 별도 스레드에서 병렬 수행 가능 → CPU 활용도 극대화
- UI thread 점유 시간 대폭 감소
- 변경된 오브젝트만 Invalidate → 불필요한 다시그리기 제거
- 기존 기능 100% 유지 (모든 확장기능: 크기, 위치, 색상, 회전, 점멸, 슬라이더 등)
- 확대/축소 동작 기존과 동일 (SetScreenSize/SetOpticRate → ExpandCalcObjectRect)

### 단점/리스크
- TagLib 태그 읽기가 thread-safe한지 확인 필요
- ScriptClass.RunAsync의 thread-safety 확인 필요
- form.Invoke 호출이 필요한 부분 (TextColorChanged, BackColorChanged) 별도 처리

---

## 권장 접근: 방법 2 우선 적용

### 이유
1. **성능 개선 폭이 더 큼**: 계산 로직을 병렬화하면 오브젝트 수에 비례하여 성능 향상
2. **메모리 부담 없음**: Bitmap 캐시 없이 CPU 최적화만으로 개선
3. **Z-order 문제 없음**: 기존 그리기 순서 유지
4. **점진적 적용 가능**: 한 번에 모든 오브젝트를 변경할 필요 없음

### 구현 파일 목록
1. `ObjectExpand.cs` - CalculateExpandStateAsync(), ApplyExpandState() 추가, EventTimerAsync() 수정
2. `ExpandState.cs` (신규) - 상태 데이터 클래스
3. `ObjectPublicGroupLayer.cs` - EventTimerAsync() 수정 (병렬 계산 + 순차 적용)

### 확대/축소 관련
- `SetScreenSize()`, `SetOpticRate()`, `SetBasePoint()`, `SetZoneAtPercent100Expand()` 는 기존과 동일하게 `ExpandCalcObjectRect()` 호출
- 이 메서드들은 계산 분리와 무관하게 동일하게 동작

---

## 구현 순서

1. `ExpandState` 데이터 클래스 생성
2. `ExpandRunAsync`에서 순수 계산 부분을 `CalculateExpandStateAsync`로 추출
3. UI 적용 부분을 `ApplyExpandState`로 추출
4. `EventTimerAsync` 수정 (계산/적용 분리)
5. Dirty Region 합산 최적화
6. (선택) 방법 1의 Static Layer 캐싱을 추가 적용

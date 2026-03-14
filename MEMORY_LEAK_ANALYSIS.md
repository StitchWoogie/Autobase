# Memory Leak Analysis Report

## 1. LocalMain Timer/Thread 메모리 누수 분석

### 1.1 timerMain (Windows.Forms.Timer) - `FormLocalMain.cs`

**설정:** `timerMain.Interval = 1` (1ms 간격, Designer.cs:833)

#### 문제점 1: `async void` 타이머 핸들러 (심각도: 중간)

```
위치: FormLocalMain.cs:1331
private async void timerMain_Tick(object sender, EventArgs e)
```

- `async void`는 예외가 발생하면 호출자에게 전달되지 않고 SynchronizationContext로 전파됨
- `_excetpionFlag = true`로 타이머를 영구 중지하는 방어 코드가 있으나, catch 블록에서 `Program.HandleFatalException(ex)`를 호출 후 타이머 자체를 명시적으로 Stop하지 않음
- **위험:** 예외 발생 시 `_excetpionFlag`에 의해 return되지만, 타이머 Tick 이벤트 자체는 계속 발생하여 불필요한 CPU 사이클 소모

#### 문제점 2: `bTimerRunning` 재진입 방지 - 타이머 누적 (심각도: 낮음)

```
위치: FormLocalMain.cs:1341-1344
if (bTimerRunning || _excetpionFlag) return;
bTimerRunning = true;
```

- Interval=1ms에서 `async void` 내부의 `await SharedViewMain.EventGoTimerAsync()`가 완료되기 전에 다음 Tick이 발생
- `bTimerRunning` 가드로 인해 중복 실행은 방지되나, 매 1ms마다 Tick 이벤트 객체가 생성되고 delegate 호출이 이루어짐
- **영향:** 직접적 메모리 누수는 아니지만, 불필요한 이벤트 할당으로 GC 부담 증가

#### 문제점 3: SharedViewMain.EventListTimer static 이벤트 (심각도: 높음)

```
위치: SharedViewMain.cs:51
public static event OnEventTimer EventListTimer;
```

- `FormGraphicChild`에서 `EventListTimer += timerHandler` (Line 1004)로 구독
- `FormGraphicChild_FormClosed`에서 `-= timerHandler`로 해제 (Line 2550)
- **그러나** `EventListTagChanged`는 `new SharedViewMain.OnEventTagChanged(FormGraphic_EventTag)`로 구독하고, 해제 시에도 `new`로 생성한 새 delegate로 해제 시도 (Line 2549)
- **C# delegate 특성상 `new` delegate로 `-=` 하면 동일 메서드를 가리키므로 정상 해제됨** - 이 부분은 안전
- **위험:** `FormGraphicChild.LoadFile()`(Line 332-333)에서 `objectGraphic.Close()` 후 새 `ObjectRoot`를 생성하지만, **이벤트 핸들러를 해제하지 않고 재등록하지도 않음** → 기존 핸들러가 닫힌 ObjectRoot를 참조할 수 있음

### 1.2 CheckEngineTagChangeThread - 레거시 Thread (심각도: 중간)

```
위치: CheckEngineTagChangeThread.cs
```

#### 문제점 1: Thread 참조 미해제

```csharp
static Thread threadSharedTag;   // Line 20
// UnInit() 에서:
bEnd = true;
threadSharedTag.Join(5000);      // Line 44
// threadSharedTag = null; ← 없음!
```

- `Join(5000)` 후 타임아웃 시 Thread가 아직 살아있을 수 있음
- `threadSharedTag` 정적 참조가 null로 설정되지 않아 Thread 객체와 연관된 ThreadStart delegate가 GC되지 않음
- `bEnd` 플래그가 `volatile`이 아님 → 이론적으로 다른 스레드에서 변경 사항이 즉시 보이지 않을 수 있음

#### 문제점 2: TimeOutMiliSecClass 반복 생성

```csharp
TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();  // Line 165
while (!bEnd) {
    Thread.Sleep(100);
    timeout.Reset();  // 매 반복마다
```

- 루프 외부에서 1회 생성이므로 누수는 아님. 양호.

### 1.3 CheckEngineNetworkToViewMain - 레거시 Thread (심각도: 중간)

```
위치: CheckEngineNetworkToViewMain.cs
```

#### 문제점 1: RingSharedMemory 정적 인스턴스

```csharp
static RingSharedMemory smNetworkToViewMain = new RingSharedMemory();  // Line 272
static RingSharedMemory smViewMainToNetwork = new RingSharedMemory();  // Line 273
```

- `UnInit()`에서 `Close()` 호출하지만, null로 설정하지 않음
- RingSharedMemory 내부의 공유 메모리 핸들이 `Close()`에서 완전히 해제되는지 확인 필요

#### 문제점 2: readyData 배열 참조

```csharp
static byte[] readyData = null;  // Line 295
readyData = (byte[])smNetworkToViewMain.GetItem();  // Line 308
```

- ThreadLoop에서 할당한 `readyData`가 `Check()` 호출 전까지 유지
- `Check()`에서 `readyData = null`로 해제 (Line 345)
- **위험:** `Check()`가 호출되지 않으면(타이머 오류 등) readyData가 계속 참조를 유지

#### 문제점 3: NetWorkProtocolRecv 반복 생성

```csharp
NetWorkProtocolRecv recv = new NetWorkProtocolRecv();  // Line 318
recv.Split(readyData, readyData.Length);
```

- 매 Check() 호출마다 새로운 객체 생성 후 버려짐
- 타이머 간격(1ms)이 매우 짧아 GC 부담 가중
- **권장:** object pool 패턴 또는 static 인스턴스 재사용

### 1.4 CheckEngineAutoDeleteThread - 최신 Task 패턴 (심각도: 낮음)

```
위치: CheckEngineAutoDeleteThread.cs
```

#### 문제점: CancellationTokenSource 미해제

```csharp
public static void UnInit() {
    if (_cts != null) {
        _cts.Cancel();
        _workerTask?.Wait(5000);
        // _cts.Dispose(); ← 없음!
        // _cts = null; ← 없음!
    }
}
```

- `CancellationTokenSource`는 `IDisposable`이며, 내부적으로 `WaitHandle` 등의 OS 리소스를 사용할 수 있음
- `Cancel()` 후 `Dispose()`를 호출하지 않음
- `_workerTask`도 null로 설정되지 않아 Task 객체 참조가 유지됨

### 1.5 LocalMain Timer/Thread 종합 위험도

| 구분 | 위치 | 심각도 | 유형 |
|------|------|--------|------|
| async void + 1ms 타이머 | FormLocalMain.cs:1331 | 중간 | GC 부담/이벤트 누적 |
| EventListTimer static event 미해제 | SharedViewMain.cs:51 | 높음 | 이벤트 핸들러 누수 |
| Thread 참조 미해제 | CheckEngineTagChangeThread.cs:20 | 중간 | 참조 유지 |
| bEnd non-volatile | CheckEngineTagChangeThread.cs:60 | 낮음 | 가시성 문제 |
| RingSharedMemory 미null처리 | CheckEngineNetworkToViewMain.cs:272 | 중간 | 참조 유지 |
| CancellationTokenSource 미Dispose | CheckEngineAutoDeleteThread.cs:74 | 낮음 | OS 리소스 |
| NetWorkProtocolRecv 반복 할당 | CheckEngineNetworkToViewMain.cs:318 | 낮음 | GC 부담 |

---

## 2. GraphicModule Object/ScriptFunction 메모리 누수 및 위험성 분석

### 2.1 ObjectRoot 및 전체 Object 계층 - GDI+ 리소스 누수 (심각도: 치명적)

```
위치: ObjectRoot.cs 및 57개 Object*.cs 파일
```

#### 주석/활성 코드 구분 (수정된 분석)

| 코드 | 위치 | 상태 |
|------|------|------|
| `Graphics.FromImage(bitmap)` | ObjectRoot.cs:209 | **주석 처리됨** (2007년 DoubleBuffer 전환 시) |
| `OnPaintBitmap.CreateGraphics(...)` | ObjectRoot.cs:236 | **주석 처리됨** |
| `new SolidBrush(backcolor)` | ObjectRoot.cs:280 | **활성 코드** — 매 Paint마다 누수 |
| `new SolidBrush(lBackGroundColor)` | ObjectRoot.cs:301 | **활성 코드** (DisplayPrint) |

#### 전체 Object 파일 GDI 누수 통계

```
전체 57개 Object*.cs 파일 분석 결과:
- GDI 리소스 할당 총 횟수: 256개소
- using 처리된 것: 107개소 (42%)
- using 없이 누수되는 것: 149개소 (58%)
```

#### 파일별 누수 순위 (상위 9개, using 미적용 개수)

| 파일 | 누수 개소 | 주요 누수 리소스 |
|------|-----------|-----------------|
| ObjectMilliDataTrend.cs | 65개 | Pen, SolidBrush, Font (DrawPattern, DisplayGraph 등) |
| ObjectMultiGraph.cs | 14개 | Pen (DisplayGraph, DisplayGraphGuideLine) |
| ObjectDataGridView.cs | 13개 | Brush, Font, StringFormat (DisplayObject) |
| ObjectText.cs | 7개 | SolidBrush (Paint 사이클) |
| ObjectSingleText.cs | 7개 | StringFormat, SolidBrush, Font (DisplayObject) |
| ObjectChart.cs | 6개 | Pen, SolidBrush |
| ObjectFont.cs | 5개 | Font |
| ObjectSVG.cs | 4개 | Pen, Brush |
| ObjectButtonPublic.cs | 4개 | Pen, Brush |

#### 주요 누수 코드 상세 (Paint 사이클마다 호출)

```csharp
// ObjectLine.cs:38 - DisplayObject() 매 Paint마다 호출
Pen pen = new Pen(RunColorLine, bthick);          // Pen 누수

// ObjectCircle.cs:94 - DisplayObject()
Pen pen = new Pen(RunColorLine, bthick);          // Pen 누수

// ObjectRoundRectangle.cs:263 - DisplayObject()
Pen pen = new Pen(RunColorLine, bthick);          // Pen 누수

// ObjectRectangle.cs:80 - DrawRectLine()
Pen pen = new Pen(color, thick);                  // Pen 누수

// ObjectMilliDataTrend.cs:7851 - DrawPatternLineOne() 루프 내
Pen pen = new Pen(pli.color, pli.thick);          // 루프 내 Pen 누수

// ObjectMilliDataTrend.cs:7983 - 루프 내
g.DrawLine(new Pen(pli.color, pli.thick * 2f), ...); // 인라인 Pen 누수

// ObjectMilliDataTrend.cs:8019 - 루프 내
g.DrawString(buf, font, new SolidBrush(pli.color), x, dy1); // 인라인 Brush 누수

// ObjectControlComboBox.cs:238,241 - DisplayObject()
StringFormat format = new StringFormat();          // StringFormat 누수
Brush brush = new SolidBrush(this.RunColorText);   // Brush 누수

// ObjectControlEditBox.cs:346,356 - DisplayObject()
StringFormat format = new StringFormat();          // StringFormat 누수
Brush brush = new SolidBrush(this.RunColorText);   // Brush 누수

// ObjectSingleText.cs:110,111,149 - DisplayObject()
StringFormat format = new StringFormat();          // StringFormat 누수
Brush brush = new SolidBrush(RunColorText);        // Brush 누수
Font font = new Font(...);                         // Font 누수

// ObjectExpand.cs:2207-2208 - DrawMouseZone()
Pen hPenWhite = new Pen(Color.White, 1);           // Pen 누수
Pen hPenBlack = new Pen(Color.Black, 1);           // Pen 누수
```

#### Paint 호출 체인과 승수 효과

```
FormGraphic_Paint() → ObjectRoot.Display()
  → new SolidBrush() × 1                              ← ObjectRoot 자체
  → ObjectGroup.Display() → objectList 순회
    → 각 ObjectExpand.Display() → DisplayObject()
      → ObjectMilliDataTrend: new Pen/Brush × 최대 65개소
      → ObjectMultiGraph:     new Pen/Brush × 최대 14개소
      → ObjectText:           new Brush × 7개소
      → ObjectSingleText:     new Brush/Font/StringFormat × 7개소
      → ObjectRectangle:      new Pen × 3개소
      → ...
```

**Object 1,000개 모듈 예시 (Paint 1회당 GDI 핸들 누수):**

```
ObjectRectangle 200개     → 200 × 3 = 600 핸들
ObjectText 300개           → 300 × 7 = 2,100 핸들
ObjectSingleText 200개    → 200 × 7 = 1,400 핸들
ObjectMilliDataTrend 50개 → 50 × 65 = 3,250 핸들
ObjectButtonPublic 100개  → 100 × 4 = 400 핸들
기타 150개                 → ~450 핸들
────────────────────────────────────────────
합계:                       ~8,200 핸들/Paint

Paint 호출 빈도: 초당 15~30회 (Invalidate → WM_PAINT)
초당 누수: 8,200 × 20 = ~164,000 핸들/초
```

#### GDI 핸들 고갈 시 발생하는 예외 (단계별)

```
[1단계] 핸들 8,000~9,000개 — 시각적 이상
  증상: 컨트롤이 안 그려짐, 빈 화면, 폰트 깨짐
  예외: 없음 (GDI+가 조용히 실패)

[2단계] 핸들 ~10,000개 (프로세스 기본 한계) — GDI+ 예외
  System.Runtime.InteropServices.ExternalException
    "A generic error occurred in GDI+."

  System.ArgumentException
    "Parameter is not valid."

  System.OutOfMemoryException  ← 실제 RAM 부족이 아닌 GDI 핸들 부족!
    at System.Drawing.Graphics.FromHdcInternal()

[3단계] 핸들 고갈 — Win32 예외
  System.ComponentModel.Win32Exception
    "Error creating window handle"
    → 새 Form/Control 생성 불가, MessageBox도 표시 불가

[4단계] 시스템 전역 (65,536 한계)
  → 다른 프로세스도 창 생성 불가
  → OS 재시작 외 복구 불가
```

#### Finalizer 의존의 위험성 — 왜 특정 PC에서만 터지는가

SolidBrush/Pen/Font은 Finalizer(`~Brush()`)를 가지고 있어 GC가 최종적으로
핸들을 회수하지만, 이는 "보장"이 아닌 "최선의 노력":

```
핸들 해제 = GC 수집 + Finalizer 스레드 실행 (2단계 필요)
실패 조건: 핸들 생성 속도 > GC 수집 빈도 × Finalizer 처리 속도
```

**동일 코드인데 특정 PC에서만 발생하는 원인:**

1. **GC 모드 차이 (가장 흔함)**
   - Workstation GC: Gen0 수집 빈번 → 핸들 빨리 회수 → 문제 안 터질 수 있음
   - Server GC: 메모리 압박 시에만 수집 → 핸들 오래 생존 → 고갈 발생

2. **RAM 크기 역설**
   - RAM 8GB PC: GC 자주 작동 → Finalizer 자주 실행 → 안전
   - RAM 32GB PC: GC가 "메모리 충분"으로 판단 → 수집 안 함 → 핸들 고갈!

3. **CPU 코어 수** — Server GC가 코어별 힙 생성 → 수집 지연

4. **동시 모듈 수/해상도** — 모니터 3개, 모듈 10개+ → Paint 호출 폭증

5. **.NET Runtime 미세 차이** — 동일 4.8이라도 KB 패치에 따라 GC 동작 상이

6. **백그라운드 프로세스** — 백신/모니터링이 Finalizer 스레드 CPU 선점

**Object 1,000개 규모에서 Finalizer 의존은 위험합니다. using 처리는 필수 조치입니다.**

#### Dispose() 불완전

```csharp
public void Dispose() {              // Line 167
    backBitmap.Dispose();             // Line 170
    groupRoot.Dispose();              // Line 171
    // scriptModuleStart = null;  ← 없음
    // scriptModuleEnd = null;    ← 없음
    // scriptModuleAlways = null; ← 없음
}
```

- `IDisposable` 인터페이스를 구현하지 않음 (표준 패턴 미준수)
- Finalizer(`~ObjectRoot()`) 없어 GDI 리소스 누수 시 최종 방어선 부재
- ScriptClass 참조들이 Dispose 시 null 처리되지 않아 GC 사이클 발생 가능

### 2.2 ScriptClass 순환 참조 (심각도: 높음)

```
위치: ScriptClass.cs (ScriptLibRun)
```

#### 문제점: Form ↔ Script ↔ Object 순환 참조

```
ScriptClass.cs:
  Line 61: [NonSerialized] public System.Windows.Forms.Form formParent;
  Line 147: public ObjectExpand parentObject = null;
```

참조 체인:
```
FormGraphicChild → ObjectRoot → ScriptClass (scriptModuleStart 등)
                                    ↓
                              formParent → FormGraphicChild (순환!)
                              parentObject → ObjectExpand → ObjectRoot (순환!)
```

- `ObjectRoot.Close()`에서 ScriptClass의 `formParent`, `parentObject`를 null로 설정하지 않음
- FormGraphicChild가 닫혀도 ScriptClass가 Form을 참조하므로 **Form이 GC되지 않음**
- 화면 전환이 빈번한 SCADA 시스템에서 **모듈 열기/닫기마다 Form 객체가 누적**

### 2.3 ScriptExternalRun 정적 싱글턴 (심각도: 높음)

```
위치: ScriptExternalRun.cs
```

#### 문제점 1: 정적 싱글턴의 delegate 보유

```csharp
public static ScriptExternalRun scriptExternal = new ScriptExternalRun();  // Line 16
List<LocalMethodGroup> arrayGroup = new List<LocalMethodGroup>();           // Line 38

class LocalMethod {
    public DeleMethod pMethod;           // delegate 참조
    public AsyncDeleMethod pAsyncMethod; // async delegate 참조
}
```

- 생성자에서 100+ 개의 `PrepareMethod()`를 호출하여 delegate 등록
- 이 delegate들은 대부분 `static` 메서드를 가리키므로 직접적 누수는 아님
- **그러나** ScriptFunctionObject.cs 등에서 static Form 참조를 보유:

```csharp
// ScriptFunctionObject.cs:15
public static Form formParentScriptAction = null;
```

- 이 static 필드가 할당 후 null로 해제되지 않아 **마지막으로 스크립트를 실행한 Form이 영구 참조됨**

#### 문제점 2: ScriptClassRunNewEngine 정적 캐시

```csharp
// ScriptClassRunNewEngine.cs:142
public static EditScriptLibMain slmLocalSave = null;
```

- 스크립트 엔진 컴파일 결과가 정적 변수에 캐싱되어 해제되지 않음
- 스크립트 변경 시 이전 컴파일 결과가 남아있을 수 있음

### 2.4 FormGraphicChild 생명주기 (심각도: 높음)

```
위치: FormGraphicChild.cs
```

#### 문제점 1: LoadFile()에서 이전 ObjectRoot 미해제

```csharp
public void LoadFile(string filename) {     // Line 330
    objectGraphic.Close();                   // Close만 호출
    objectGraphic = new ObjectRoot();        // 새 객체 생성
    // 이전 objectGraphic.Dispose() 호출 없음!
}
```

- `Close()`는 논리적 종료이고, `Dispose()`는 리소스 해제
- Close만 호출하고 Dispose를 호출하지 않아 **GDI 리소스(backBitmap 등)가 해제되지 않음**
- 모듈 전환 시마다 리소스 누수 발생

#### 문제점 2: Dispose 순서 문제

```csharp
protected override void Dispose(bool disposing) {  // Line 341
    if (disposing) {
        if (components != null) {
            components.Dispose();
        }
    }
    base.Dispose(disposing);           // base 먼저 호출
    objectGraphic.Dispose();           // base.Dispose 이후에 호출 - 위험!
}
```

- `base.Dispose()` 이후에 `objectGraphic.Dispose()`를 호출하면 이미 Form이 해제된 상태에서 ObjectRoot의 리소스를 해제하게 됨
- ObjectRoot 내부에서 Form 참조를 사용하면 `ObjectDisposedException` 발생 가능

#### 문제점 3: LanguageManager 이벤트 미해제

```csharp
// 생성자 또는 Load에서:
LanguageManager.EventLanguageChanged += OnLanguageChanged;  // 구독
// Dispose나 FormClosed에서 -= 호출 없음!
```

- 정적 이벤트에 인스턴스 메서드를 구독하면 **Form 인스턴스가 GC되지 않음**
- Form을 닫아도 LanguageManager가 Form 참조를 유지

### 2.5 ScriptFunction 정적 컬렉션 (심각도: 중간)

여러 ScriptFunction 파일에서 정적 컬렉션이 정리되지 않음:

| 파일 | 변수 | 설명 |
|------|------|------|
| ScriptFunctionDb.cs:345 | `static ArrayList arrayDataSet` | DataSet 누적 |
| ScriptFunctionFile.cs:862 | `static ArrayList blockOpenList` | 파일 핸들 누적 |
| ScriptFunctionSql.cs:1501 | `static ArrayList blockSQL` | SQL 연결 누적 |
| ScriptFunctionAlarm.cs:489 | `static ArrayList arrayFilter` | 필터 누적 |
| ScriptFunction_MU.cs:17 | `static ArrayList arrayMultiRegister` | 레지스터 누적 |
| ScriptFunctionJson.cs:18 | `static Dictionary<string, JToken> jsonObjects` | JSON 객체 누적 |

- 이 컬렉션들은 스크립트 함수 호출 시 항목이 추가되지만, 명시적으로 정리하는 코드가 없음
- 애플리케이션 수명 동안 **지속적으로 메모리가 증가**

### 2.6 ObjectPublicGroupLayer 객체 관리 (심각도: 중간)

```
위치: ObjectPublicGroupLayer.cs
```

#### 문제점: 양방향 참조 미해제

```csharp
public void AddObject(object p) {
    ((ObjectType)p).parentGroupLayer = this;  // 양방향 참조 설정
    objectList.Add(p);
}
```

- 객체 삭제 시 `RemoveAt()`만 호출하고 `parentGroupLayer = null` 처리를 하지 않음
- 삭제된 객체가 부모 그룹 참조를 유지하여 GC 방해

---

## 3. 종합 권장 사항

### 즉시 조치 필요 (치명적/높음)

1. **ObjectRoot GDI 리소스**: 모든 Graphics, Brush, Pen 생성에 `using` 문 적용
2. **ScriptClass 순환 참조**: `ObjectRoot.Close()`에서 ScriptClass 필드의 `formParent`, `parentObject`를 null로 설정
3. **FormGraphicChild.LoadFile()**: `Close()` 후 `Dispose()` 추가 호출
4. **FormGraphicChild.Dispose()**: `objectGraphic.Dispose()`를 `base.Dispose()` 이전으로 이동
5. **LanguageManager 이벤트**: FormClosed에서 `EventLanguageChanged -= OnLanguageChanged` 추가
6. **ScriptFunctionObject.formParentScriptAction**: 스크립트 실행 완료 후 null 설정

### 중기 개선 (중간)

7. **CheckEngineAutoDeleteThread**: `UnInit()`에서 `_cts.Dispose()` 추가
8. **Thread 참조 정리**: `Join()` 후 `threadSharedTag = null` 설정
9. **`bEnd` volatile 키워드**: 멀티스레드 가시성 보장
10. **정적 컬렉션 정리**: 모듈 종료 시 또는 주기적으로 Clear() 호출

### 장기 개선 (낮음/구조적)

11. **ObjectRoot IDisposable 구현**: 표준 Dispose 패턴 + Finalizer 적용
12. **타이머 간격 조정**: 1ms → 적절한 간격(50~100ms)으로 변경하여 GC 부담 감소
13. **Object Pool 적용**: NetWorkProtocolRecv 등 빈번하게 생성/파괴되는 객체에 Pool 패턴 적용
14. **WeakReference 도입**: ScriptExternalRun의 delegate에서 Form 참조 시 WeakReference 사용 검토

---

## 부록: 특정 PC 문제 진단 방법

```
1. 작업관리자 → 세부정보 → 열 추가 → "GDI 개체"
   → 시간에 따라 증가하면 누수 확정

2. 문제 PC에서 확인할 것:
   > [System.Runtime.GCSettings]::IsServerGC    # PowerShell
   > Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' -Name Release

3. Performance Monitor → .NET CLR Memory
   → "# Gen 2 Collections" 카운터 비교
   → 문제 PC에서 이 값이 낮으면 GC가 덜 돌아가는 것
```

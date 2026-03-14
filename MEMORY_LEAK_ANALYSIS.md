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

### 2.1 ObjectRoot - GDI+ 리소스 누수 (심각도: 치명적)

```
위치: ObjectRoot.cs
```

#### 문제점 1: Graphics 객체 미해제

```
Line 209: Graphics g = Graphics.FromImage(bitmap);   // using 없음
Line 236: Graphics g = OnPaintBitmap.CreateGraphics(...)  // using 없음
```

- `Display()` 메서드에서 Graphics 객체를 생성하지만 `using` 또는 `Dispose()` 없이 사용
- 타이머에 의해 반복 호출되므로 **GDI 핸들이 지속적으로 증가**
- Windows의 GDI 핸들 제한(기본 10,000개)에 도달하면 애플리케이션 크래시

#### 문제점 2: SolidBrush 인라인 생성 미해제

```
Line 214: g.FillRectangle(new SolidBrush(lBackGroundColor), rcScreen);
Line 241: g.FillRectangle(new SolidBrush(lBackGroundColor), rcScreen);
Line 280: new SolidBrush(backcolor)
Line 301: new SolidBrush(lBackGroundColor)
```

- 모든 렌더링 호출마다 Brush GDI 핸들 누수
- **영향:** 장시간 운영 시 GDI 핸들 고갈로 인한 시스템 불안정

#### 문제점 3: Dispose() 불완전

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

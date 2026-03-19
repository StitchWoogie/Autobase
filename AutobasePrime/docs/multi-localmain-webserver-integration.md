# Multi-LocalMain WebServer 통합 모니터링 아키텍처 설계

## 1. 현재 구조 분석

### 1.1 현재 아키텍처 (1:1 구조)

```
[Web Browser] → [PortalServerWeb (IIS)] → [LocalMain 1대]
                     port 80/443              port 7200/8732
```

**현재 한계점:**
- `Web.config`에 `LocalMainIP`/`LocalMainPort`가 단일 값으로 고정 (`127.0.0.1:7200`)
- `ServiceLibSvcDataGate.cs`에서 WCF 프록시가 단일 endpoint(`net.tcp://localhost:8732`)만 지원
- `ServiceDataTagStatic.cs`의 소켓 풀(`arrayClient`)이 단일 서버만 대상
- 서버 디스커버리, 로드밸런싱, 페일오버 메커니즘 없음

### 1.2 핵심 통신 경로

```
Web Client
  → PortalServerWeb.ServiceDataTag (HTTP/WCF)
    → ServiceDataTagStatic.SendAndGetData() (TCP socket to port 7200)
      → LocalMain.DataGateServer (WCF on port 8732)
        → Tag Engine → 값 반환
```

---

## 2. ViewMain/OpenSilver에 Multi-Site를 넣지 않는 이유

### 2.1 현재 클라이언트 구조 분석

| 클라이언트 | 연결 방식 | Multi-Site 제약 |
|-----------|----------|----------------|
| **ViewMain** (데스크톱) | `InitSite(site)` → `nConnectionID` 발급, TCP/WCF 1:1 | `CallBackWebClientChangeSite()`로 전환 가능하나 동시 연결 불가 |
| **OpenSilver** (웹) | URL `?site=IP`로 1대 지정 | 브라우저 탭 당 1사이트 |
| **PortalServerWeb** (서버) | `Web.config`에 `LocalMainIP` 단일값 | 1:1 고정 |

### 2.2 기술적 제약 사항

**1) 태그 네임스페이스 충돌**
- `TagLib`는 단일 네임스페이스: `TagAiClass[]` 배열 인덱스 기반 (MAX 30,000개)
- Site-A의 `TT-001`과 Site-B의 `TT-001`이 동일 이름 → 충돌
- 다중 사이트 태그를 한 프로젝트에 합치려면 태그 시스템 전면 재설계 필요

**2) ThreadDataChange 단일 서버 폴링**
- `CheckCurrentValue()`가 단일 `DataGate` 통해 태그 폴링
- 다중 서버 동시 폴링은 대규모 리팩토링 필요

**3) 프로젝트 재구성 비용**
- 각 사이트의 태그(최대 30,000×5종), 통신설정, 화면을 하나의 프로젝트로 합치는 것은 사실상 새 프로젝트

**4) 역할 분리**
- ViewMain/OpenSilver = **운전 화면 뷰어** (운전원이 자기 사이트에 집중)
- Multi-Site Dashboard = **관제 개요 화면** (관제자가 전체 사이트 파악)
- 목적이 다르므로 분리가 적절

### 2.3 권장 접근: PortalServerWeb 확장 + 기존 뷰어 드릴다운

```
┌──────────────────────────────────────────────────────────────┐
│           Multi-Site Monitor (PortalServerWeb 내 신규 페이지)  │
│           /AutoWeb/MultiSite/Dashboard.html                   │
│                                                              │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐                     │
│  │ Site-A  │  │ Site-B  │  │ Site-C  │  ← 요약 카드        │
│  │ ●Online │  │ ●Online │  │ ○Offline│                     │
│  │ Alm: 2  │  │ Alm: 0  │  │         │                     │
│  │ [Open]  │  │ [Open]  │  │         │  ← 클릭 시 드릴다운  │
│  └────┬────┘  └────┬────┘  └─────────┘                     │
│       │            │                                         │
│       ▼            ▼                                         │
│  OpenSilver    OpenSilver   ← 기존 1:1 뷰어 그대로 활용     │
│  ?site=.10    ?site=.20    (새 탭 또는 iframe)               │
└──────────────────────────────────────────────────────────────┘
```

**별도 프로그램이 아닌 PortalServerWeb 확장인 이유:**
1. 완전 별도 프로그램은 인증/배포/운영 관리 포인트 증가
2. PortalServerWeb에 이미 WCF/TCP 통신 인프라 존재
3. 웹 페이지 추가만으로 구현 가능 (기존 코드 수정 제로)
4. 드릴다운 시 OpenSilver `?site=IP` 파라미터로 기존 뷰어 바로 활용

### 2.4 변경 영향도

| 구성요소 | 변경 내용 | 영향도 |
|----------|----------|--------|
| **LocalMain** | 변경 없음 (기존 DataGateServer API 사용) | 없음 |
| **ViewMain** | 변경 없음 | 없음 |
| **OpenSilver** | 변경 없음 (`?site=` 그대로) | 없음 |
| **PortalServerWeb** | `Web.config` 사이트목록 + `ServiceMultiSite.svc` + `/MultiSite/` 페이지 | 추가만 |

---

## 3. Multi-LocalMain 통합 아키텍처 제안

### 3.1 방안 A: Gateway Router 패턴 (권장)

PortalServerWeb 내부에 **LocalMain Router/Registry** 레이어를 추가하여, 여러 LocalMain 인스턴스를 등록하고 라우팅하는 방식.

```
[Web Browser]
     │
     ▼
[PortalServerWeb (IIS)]
     │
     ├── LocalMain Router (신규)
     │      │
     │      ├── LocalMain Registry (인스턴스 목록 관리)
     │      ├── Health Checker (상태 모니터링)
     │      └── Request Router (요청 라우팅)
     │
     ├───→ [LocalMain-A] site-A (192.168.1.10:8732)
     ├───→ [LocalMain-B] site-B (192.168.1.20:8732)
     └───→ [LocalMain-C] site-C (192.168.1.30:8732)
```

#### 구현 방법

**Step 1: LocalMain Registry 구성 (`Web.config` 확장)**

```xml
<appSettings>
  <!-- 기존 단일 설정 유지 (하위 호환) -->
  <add key="LocalMainIP" value="127.0.0.1" />
  <add key="LocalMainPort" value="7200" />

  <!-- Multi-LocalMain 설정 (신규) -->
  <add key="MultiLocalMainEnabled" value="true" />
  <add key="LocalMainInstances" value="
    site-a|192.168.1.10|8732|Plant-A;
    site-b|192.168.1.20|8732|Plant-B;
    site-c|192.168.1.30|8732|Plant-C
  " />
  <add key="HealthCheckIntervalSec" value="30" />
</appSettings>
```

**Step 2: LocalMain Registry 클래스 (신규)**

```csharp
// PortalServerWeb/Library/LocalMainRegistry.cs
public class LocalMainInstance
{
    public string SiteId { get; set; }        // "site-a"
    public string DisplayName { get; set; }   // "Plant-A"
    public string IP { get; set; }            // "192.168.1.10"
    public int Port { get; set; }             // 8732
    public bool IsOnline { get; set; }
    public DateTime LastHealthCheck { get; set; }
    public int TagCount { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
}

public static class LocalMainRegistry
{
    private static ConcurrentDictionary<string, LocalMainInstance> _instances;
    private static Timer _healthCheckTimer;

    public static void Initialize() { /* Web.config에서 인스턴스 목록 로드 */ }
    public static LocalMainInstance GetInstance(string siteId) { ... }
    public static List<LocalMainInstance> GetAllInstances() { ... }
    public static List<LocalMainInstance> GetOnlineInstances() { ... }
}
```

**Step 3: Multi-Site ServiceLibSvcDataGate 확장**

```csharp
// 기존 ServiceLibSvcDataGate의 단일 연결을 사이트별 연결 풀로 확장
public class MultiSiteDataGate
{
    // siteId → WCF proxy 매핑
    private static ConcurrentDictionary<string, ServiceDataGate> _gatePool;

    public static ServiceDataGate GetGate(string siteId)
    {
        var instance = LocalMainRegistry.GetInstance(siteId);
        return _gatePool.GetOrAdd(siteId, id => CreateGate(instance));
    }

    public static byte[] Command(string siteId, string command, params object[] args)
    {
        var gate = GetGate(siteId);
        return gate.CommonMethod(EncryptArgs(command, args));
    }

    // 모든 사이트에 동시 요청 (대시보드용)
    public static Dictionary<string, byte[]> BroadcastCommand(string command, params object[] args)
    {
        var results = new ConcurrentDictionary<string, byte[]>();
        Parallel.ForEach(LocalMainRegistry.GetOnlineInstances(), instance =>
        {
            results[instance.SiteId] = Command(instance.SiteId, command, args);
        });
        return results;
    }
}
```

**Step 4: Web API 엔드포인트 (신규)**

```csharp
// PortalServerWeb/AutoWeb/Service/ServiceMultiSite.svc.cs

// 사이트 목록 조회
[WebMethod] public string GetSiteList()
// 특정 사이트의 태그 데이터 조회
[WebMethod] public string GetSiteTagData(string siteId, string tagNames)
// 전체 사이트 요약 대시보드
[WebMethod] public string GetDashboardSummary()
// 특정 사이트의 알람 조회
[WebMethod] public string GetSiteAlarms(string siteId)
// 크로스사이트 태그 비교
[WebMethod] public string CompareTags(string tagName, string siteIds)
```

**Step 5: 웹 대시보드 UI (신규)**

```
/AutoWeb/MultiSite/
  ├── Dashboard.aspx          ← 전체 사이트 개요 (카드 형태)
  ├── SiteDetail.aspx         ← 개별 사이트 상세 (기존 화면 재활용)
  ├── CrossSiteAlarm.aspx     ← 통합 알람 뷰
  ├── CrossSiteTrend.aspx     ← 크로스사이트 트렌드 비교
  └── SiteMap.aspx            ← 지리적 사이트 맵 (선택)
```

#### 대시보드 화면 구성

```
┌─────────────────────────────────────────────────────────┐
│  Multi-Site Dashboard                        [Settings] │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐             │
│  │ Plant-A  │  │ Plant-B  │  │ Plant-C  │             │
│  │ ● Online │  │ ● Online │  │ ○ Offline│             │
│  │ Tags: 5K │  │ Tags: 3K │  │ Tags: -- │             │
│  │ Alm: 2   │  │ Alm: 0   │  │ Alm: -- │             │
│  │ CPU: 45% │  │ CPU: 32% │  │ CPU: -- │             │
│  │ [Detail] │  │ [Detail] │  │ [Detail] │             │
│  └──────────┘  └──────────┘  └──────────┘             │
│                                                         │
│  ── 통합 알람 ──────────────────────────────            │
│  [!] Plant-A | T-001 HiHi | 95.2°C | 10:23:45          │
│  [!] Plant-A | P-003 Low  | 1.2bar | 10:22:11          │
│                                                         │
│  ── 크로스사이트 트렌드 ────────────────────            │
│  [온도 비교 차트: Plant-A vs Plant-B]                    │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 3.2 방안 B: Aggregator 서비스 (별도 프로세스)

별도의 Aggregator 프로세스를 두어 여러 LocalMain의 데이터를 수집/캐싱하는 방식.

```
                    [Web Browser]
                         │
                         ▼
                  [PortalServerWeb]
                         │
                         ▼
              [Multi-Site Aggregator] ← 신규 프로세스
                    │    │    │
                    ▼    ▼    ▼
            [LM-A] [LM-B] [LM-C]
```

**장점:** PortalServerWeb 수정 최소화, 데이터 캐싱/버퍼링 가능
**단점:** 추가 프로세스 관리 필요, 레이턴시 증가

### 3.3 방안 비교

| 항목 | 방안 A (Gateway Router) | 방안 B (Aggregator) |
|------|----------------------|-------------------|
| 구현 복잡도 | 중간 | 높음 |
| 기존 코드 변경 | PortalServerWeb 확장 | 최소 변경 + 신규 프로세스 |
| 실시간 성능 | 직접 연결 (빠름) | 중간 레이어 (느림) |
| 캐싱 | 선택적 | 필수/용이 |
| 운영 복잡도 | 낮음 | 높음 (프로세스 관리) |
| 확장성 | 10대 이하 적합 | 대규모 적합 |
| **권장 시나리오** | **3~10대 통합** | **10대 이상 대규모** |

---

### 3.4 대시보드 구현 구조

```
PortalServerWeb (기존 IIS)
│
├── /AutoWeb/WebPages/         ← 기존 (변경 없음)
├── /AutoWeb/MobilePages/      ← 기존 (변경 없음)
├── /AutoWeb/Service/          ← 기존 + ServiceMultiSite.svc 추가
│
├── /AutoWeb/MultiSite/        ← 신규 (관제 대시보드)
│   ├── Dashboard.html         ← SPA (순수 HTML+JS)
│   ├── js/
│   │   ├── multi-site-app.js  ← 대시보드 로직
│   │   ├── site-card.js       ← 사이트 카드 컴포넌트
│   │   └── alarm-feed.js      ← 통합 알람 피드
│   └── css/
│       └── dashboard.css
│
└── /AutoWeb/Service/
    └── ServiceMultiSite.svc   ← 신규 (다중 LocalMain 연결)
```

**드릴다운 연동:**
```javascript
function openSiteDetail(siteIP) {
    // 방법 1: 새 탭에서 OpenSilver 열기
    window.open(`https://views.autobase.biz/?site=${siteIP}`, '_blank');

    // 방법 2: iframe으로 현재 페이지 내 임베드
    document.getElementById('detail-frame').src =
        `https://views.autobase.biz/?site=${siteIP}`;

    // 방법 3: ViewMain 실행 (데스크톱 환경)
    // URL scheme: autobase://viewmain?site=${siteIP}
}
```

---

## 4. 단계별 구현 로드맵

### Phase 1: 기반 구축 (핵심)
1. `LocalMainRegistry` 클래스 구현 및 `Web.config` multi-instance 설정
2. `MultiSiteDataGate` 연결 풀 구현
3. Health Check 타이머로 LocalMain 상태 모니터링
4. `ServiceMultiSite.svc` API 엔드포인트 생성

### Phase 2: 대시보드 UI
1. 사이트 카드 대시보드 페이지
2. 통합 알람 뷰 (전체 사이트 알람 통합)
3. 사이트별 Detail 화면 (기존 뷰 재활용)

### Phase 3: 고급 기능
1. 크로스사이트 트렌드 비교
2. 사이트 맵 (지리적 위치 표시)
3. 통합 리포트 (다중 사이트 데이터 병합)
4. 사이트 그룹핑 및 권한 관리

### Phase 4: 안정화
1. 연결 페일오버 (자동 재연결)
2. 데이터 캐싱 레이어 (선택)
3. WebSocket 기반 실시간 push (SignalR 등)

---

## 5. 주요 변경 대상 파일

| 파일 | 변경 내용 |
|------|----------|
| `PortalServerWeb/Web.config` | Multi-instance 설정 추가 |
| `PortalServerWeb/Library/ConfigWeb.cs` | 다중 인스턴스 설정 파싱 |
| `Dll/AutoLib/ServiceLibSvcDataGate.cs` | 사이트별 연결 풀 지원 |
| `PortalServerWeb/AutoWeb/Service/` | ServiceMultiSite.svc 신규 |
| `PortalServerWeb/AutoWeb/MultiSite/` | 대시보드 UI 페이지 신규 |

---

## 6. LocalMain 측 변경 사항

LocalMain 자체는 최소한의 변경만 필요합니다:

1. **사이트 식별자 반환**: `V2_GetSiteInfo` 커맨드 추가 (siteId, displayName 반환)
2. **Health Check 응답**: `V2_HealthCheck` 커맨드 추가 (상태, 태그 수, 리소스 사용량)
3. **기존 `IServiceDataGateServer` 인터페이스에 메서드 추가** (하위 호환 유지)

```csharp
// IServiceDataGateServer.cs 확장
[OperationContract]
byte[] GetSiteInfo();      // siteId, name, version 반환

[OperationContract]
byte[] HealthCheck();       // online, tagCount, cpu, memory 반환
```

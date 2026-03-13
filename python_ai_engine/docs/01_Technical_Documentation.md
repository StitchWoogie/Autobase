# Python AI Engine - 기술 문서 (Technical Documentation)

> 대상: 개발자, 시스템 엔지니어, 기술 관리자
> 버전: Phase 5 (2026-03-09)
> 플랫폼: Autobase48 SCADA + Python 3.10+

---

## 1. 시스템 아키텍처

### 1.1 전체 구조

```
┌─────────────────────────────────────────────────────────┐
│  Autobase48 LocalMain (.NET 4.8.1 / WinForms)          │
│  ┌──────────────┐  ┌──────────────┐  ┌───────────────┐ │
│  │ PythonAi     │  │ ScriptBridge │  │ FormDashboard │ │
│  │ Manager      │──│ (Delegate)   │  │ (WinForms UI) │ │
│  │ (TCP Client) │  │ Tag R/W      │  │ 5-Tab Monitor │ │
│  └──────┬───────┘  └──────────────┘  └───────────────┘ │
│         │ TCP localhost:5678                             │
│         │ Length-prefixed JSON (4-byte LE + UTF-8)       │
├─────────┼───────────────────────────────────────────────┤
│         ▼                                                │
│  ┌─────────────────────────────────────────────────┐    │
│  │  Python AI Engine (독립 프로세스)                │    │
│  │  ┌──────────┐  ┌────────┐  ┌──────────────┐    │    │
│  │  │ Gateway  │→ │ Router │→ │ Services(16) │    │    │
│  │  │ (TCP)    │  │ +Auth  │  │ predict/     │    │    │
│  │  │          │  │ +Valid │  │ analysis/    │    │    │
│  │  │          │  │ +Audit │  │ script/      │    │    │
│  │  └──────────┘  └────────┘  │ vision/train │    │    │
│  │                             └──────┬───────┘    │    │
│  │  ┌──────────┐  ┌────────┐  ┌──────┴───────┐    │    │
│  │  │ Model    │  │ Worker │  │ Sandbox      │    │    │
│  │  │ Registry │  │ Pool   │  │ (Restricted) │    │    │
│  │  │ +Cache   │  │ RT/B/S │  │ +ResourceLim │    │    │
│  │  └──────────┘  └────────┘  └──────────────┘    │    │
│  └─────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────┘
```

### 1.2 프로토콜 사양

| 항목 | 값 |
|------|-----|
| 전송 프로토콜 | TCP (localhost only) |
| 포트 | 5678 (설정 가능) |
| 메시지 포맷 | 4-byte Little-Endian 길이 + UTF-8 JSON |
| 최대 메시지 크기 | 10MB |
| 타임아웃 (기본) | 30초 |
| 프로토콜 버전 | 2 |

### 1.3 요청/응답 메시지 스키마

**요청:**
```json
{
  "Id": "a1b2c3d4...",
  "Service": "predict.power",
  "Payload": { "current_kw": 150.5 },
  "protocolVersion": 2,
  "context": { "user": "operator1", "station": "S1", "permissions": ["predict.execute"] },
  "priority": "high"
}
```

**응답:**
```json
{
  "Id": "a1b2c3d4...",
  "Ok": true,
  "Result": { "predicted_kw": 155.2, "confidence": 0.87, "method": "model" },
  "durationMs": 12.3
}
```

---

## 2. 등록 서비스 (16개)

### 2.1 서비스 목록

| 서비스 | 필요 권한 | 설명 |
|--------|-----------|------|
| `system.ping` | 없음 | 연결 확인 |
| `system.status` | 없음 | 엔진 상태 조회 |
| `system.metrics` | 없음 | 성능 메트릭 |
| `system.models` | 없음 | 모델 목록/캐시 상태 |
| `system.health` | 없음 | 종합 건강 상태 |
| `system.shutdown` | python.admin | 엔진 종료 |
| `predict.power` | predict.execute | 전력 부하 예측 |
| `analysis.trend` | analysis.execute | 시계열 추세 분석 (OLS) |
| `analysis.correlation` | analysis.execute | 상관관계 분석 (Pearson) |
| `analysis.report` | analysis.execute | 분석 리포트 생성 |
| `script.execute` | script.execute | 사용자 스크립트 실행 |
| `vision.detect` | vision.execute | 이미지 객체 검출 |
| `vision.ocr` | vision.execute | 문자 인식 (OCR) |
| `train.start` | train.execute | 모델 학습 시작 |
| `train.status` | train.execute | 학습 상태 조회 |
| `train.cancel` | train.execute | 학습 취소 |

### 2.2 predict.power (3-Tier 추론)

```
요청 → Tier 1: 모델 레지스트리 (ONNX/sklearn)
          ↓ (모델 없으면)
       Tier 2: 통계 기반 OLS 선형회귀
          ↓ (히스토리 부족하면)
       Tier 3: Fallback (현재값 반환, confidence=0.0)
```

**입력:**
```json
{
  "current_kw": 150.5,
  "history": [140.0, 142.5, 145.0, 148.0, 150.5],
  "model_name": "power_predict",
  "horizon_sec": 300
}
```

**출력:**
```json
{
  "predicted_kw": 155.2,
  "confidence": 0.87,
  "method": "model",
  "model": "power_predict (onnx)",
  "horizon_sec": 300,
  "inference_ms": 3.2
}
```

### 2.3 script.execute (SCADA 통합 샌드박스)

**Preload/Snapshot 패턴:**
```
C# PythonAiScriptBridge
  ├── Step 1: payload._read_tags 파싱
  ├── Step 2: TagLib.GetStructPublic() → _tag_snapshot 생성
  ├── Step 3: Python Engine에 전송
  │     └── SandboxRunner.execute()
  │           ├── tag_read() → snapshot에서 읽기
  │           ├── tag_write() → _pending_writes에 저장
  │           └── emit_result() → 결과 반환
  ├── Step 4: 응답._pending_writes 수신
  └── Step 5: PlcScan.SetTagValue() → 실제 SCADA 태그 쓰기
```

**샌드박스 보안 5계층:**

| 계층 | 내용 |
|------|------|
| 1. Safe Builtins | exec/eval/open/compile 등 16개 함수 제거 |
| 2. Import Whitelist | math/numpy/pandas 등 25개 모듈만 허용 |
| 3. Code Validation | __subclasses__/os.system 등 15개 패턴 사전 차단 |
| 4. Resource Limiter | 타임아웃 5초 + 메모리 256MB 상한 |
| 5. Restricted Globals | SCADA API 9개 함수만 주입 |

**스크립트에서 사용 가능한 API:**

| 함수 | 설명 |
|------|------|
| `tag_read(tag_name)` | SCADA 태그 현재값 읽기 |
| `tag_write(tag_name, value)` | 태그 쓰기 (pending, 권한 필요) |
| `history_query(tag, start, end)` | 이력 조회 (향후 확장) |
| `log_info(msg)` | 정보 로그 |
| `log_warning(msg)` | 경고 로그 |
| `log_error(msg)` | 에러 로그 |
| `emit_result(data)` | 결과 데이터 설정 |
| `print(...)` | 출력 캡처 |
| `payload` | 입력 데이터 dict |

---

## 3. 인증/권한 모델

### 3.1 AuthChecker 동작

```
요청 수신 → 서비스별 필요 권한 조회
         → context.permissions 확인
         → strict_mode=True: 권한 없으면 거부
            strict_mode=False: 권한 없으면 허용 (개발 모드)
         → 'python.admin'은 모든 서비스 접근 가능
```

### 3.2 설정

```json
{
  "auth": {
    "strictMode": true
  }
}
```

- **운영 환경:** `strictMode: true` (기본값) — permissions 없으면 거부
- **개발 환경:** `strictMode: false` — permissions 없으면 허용

### 3.3 script.execute 권한 분리

- `script.execute`: 스크립트 실행 + 태그 읽기 (기본)
- `script.tag_write`: 태그 쓰기 (payload에 `_allow_tag_write: true` 명시 시에만 부여)

---

## 4. 모델 관리

### 4.1 ModelRegistry

```
models/
├── registry.json          # 모델 메타데이터
├── power_predict.onnx     # ONNX 모델
├── anomaly_detect.pkl     # sklearn 모델
└── custom_model/
    └── model.py           # load_model() + predict() 구현
```

**registry.json 예시:**
```json
{
  "power_predict": {
    "framework": "onnx",
    "path": "models/power_predict.onnx",
    "version": "1.0",
    "input_shape": [1, 11]
  }
}
```

### 4.2 지원 프레임워크

| 프레임워크 | 확장자 | 로더 |
|-----------|--------|------|
| ONNX Runtime | .onnx | onnxruntime.InferenceSession |
| scikit-learn | .pkl, .joblib | joblib.load |
| Custom | .py | importlib (load_model + predict) |

### 4.3 ModelCache

- LRU 방식, 최대 5개 모델 / 512MB
- Hot Reload: 30초 주기 파일 변경 감지 (설정 가능)

---

## 5. Worker Pool

| 풀 | 기본 수 | 용도 |
|----|---------|------|
| Realtime | 4 | predict.*, system.* |
| Batch | 2 | analysis.*, train.* |
| Script | 2 | script.execute |

---

## 6. C# 연동 계층

### 6.1 PythonAiManager (TCP 클라이언트)

```csharp
// 비동기 서비스 호출
PythonAiMessage response = await PythonAiManager.CallAsync(
    "predict.power",
    new { current_kw = 150.5 },
    PythonCallOptions.RealtimePredict
);

if (response.IsSuccess)
{
    double predicted = response.GetResultField<double>("predicted_kw");
}
```

### 6.2 ScriptFunction Bridge (Autobase 스크립트용)

```csharp
// GraphicModule (delegate 선언) → LocalMain (구현 등록)
ScriptFunctionPythonAi.procCallAsync = CallAsync;  // C_init에서 등록
```

**등록된 스크립트 함수 10개:**

| 함수 | 반환 | 설명 |
|------|------|------|
| PythonAiIsConnected() | int | 연결 상태 |
| PythonAiPing() | int | Ping 결과 |
| PythonAiStatus() | string | 상태 JSON |
| PythonAiPredictPower(double) | double | 전력 예측값 |
| PythonAiTrend(string) | string | 추세 분석 JSON |
| PythonAiCorrelation(string, string) | double | 상관계수 |
| PythonAiScript(string) | string | 스크립트 실행 |
| PythonAiScriptEx(string, string) | string | 스크립트+payload |
| PythonAiCall(string, string) | string | 범용 호출 |
| PythonAiCallTimeout(string, string, int) | string | 타임아웃 지정 호출 |

### 6.3 Dashboard UI (FormPythonAiDashboard)

5-Tab WinForms 대시보드:
- Tab 1: 연결 상태 / Ping / 엔진 시작/중지
- Tab 2: 서비스 테스트 (predict, analysis, script)
- Tab 3: 모델 관리 (목록, 캐시 상태)
- Tab 4: 로그 뷰어 (실시간 로그 스트림)
- Tab 5: 설정 (엔진 설정 편집)

---

## 7. 설정 파일

### engine_config.json

```json
{
  "host": "127.0.0.1",
  "port": 5678,
  "auth": { "strictMode": true },
  "workers": {
    "realtime": { "count": 4 },
    "batch": { "count": 2 },
    "script": { "count": 2 }
  },
  "limits": {
    "scriptTimeoutMs": 30000,
    "scriptMemoryMb": 256,
    "payloadMaxKb": 1024
  },
  "models": {
    "registryPath": "models",
    "cacheMaxCount": 5,
    "hotReloadEnabled": true,
    "hotReloadIntervalSec": 30
  },
  "logging": {
    "level": "INFO",
    "auditEnabled": true
  }
}
```

---

## 8. 파일 구조

```
python_ai_engine/
├── main.py                    # 엔트리포인트
├── config.py                  # 설정 클래스
├── engine_config.json         # 런타임 설정
├── requirements.txt           # Python 의존성
├── gateway/
│   └── server.py              # TCP 게이트웨이
├── router/
│   ├── router.py              # 서비스 라우터
│   ├── auth.py                # 인증/권한
│   └── validator.py           # 요청 검증
├── services/
│   ├── system_service.py      # system.* 서비스
│   ├── predict_service.py     # predict.* 서비스 (3-Tier)
│   ├── analysis_service.py    # analysis.* 서비스
│   ├── script_service.py      # script.execute
│   ├── vision_service.py      # vision.* 서비스
│   └── train_service.py       # train.* 서비스
├── sandbox/
│   ├── runner.py              # 스크립트 실행 엔진
│   ├── api_bridge.py          # SCADA API (tag_read/write)
│   ├── policy.py              # 보안 정책
│   └── resource_limiter.py    # 리소스 제한
├── models/
│   ├── registry.py            # 모델 레지스트리
│   ├── cache.py               # LRU 모델 캐시
│   ├── hot_reload.py          # 핫 리로드
│   └── loaders/               # ONNX/sklearn 로더
├── monitoring/
│   ├── health.py              # 건강 상태
│   ├── metrics.py             # 성능 메트릭
│   └── resource_guard.py      # 리소스 감시
├── workers/
│   └── worker_pool.py         # 워커 풀 (RT/Batch/Script)
├── plugins/
│   ├── loader.py              # 플러그인 로더
│   └── interface.py           # 플러그인 인터페이스
└── logs/
    ├── audit_logger.py        # 감사 로거
    └── structured_logger.py   # 구조화 로거
```

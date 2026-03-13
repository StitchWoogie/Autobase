# Python AI Engine - 사용자 가이드

> 대상: Autobase 스크립트 개발자, SCADA 운영자, 현장 엔지니어
> 버전: Phase 5 (2026-03-09)

---

## 1. 개요

Python AI Engine은 Autobase48 SCADA 시스템에 AI/머신러닝 기능을 추가하는 확장 모듈입니다.
기존 Autobase 스크립트에서 간단한 함수 호출만으로 전력 예측, 데이터 분석,
사용자 정의 Python 스크립트 실행이 가능합니다.

### 할 수 있는 것

- SCADA 태그 데이터로 **전력 부하 예측**
- 시계열 데이터의 **추세 분석** 및 **상관관계 분석**
- SCADA 태그를 읽고 쓰는 **사용자 Python 스크립트** 실행
- ONNX/sklearn **AI 모델** 로드 및 추론
- **대시보드**에서 실시간 상태 모니터링

---

## 2. 시작하기

### 2.1 대시보드 열기

LocalMain 메뉴에서: **설정 → Python AI Dashboard**

대시보드에서 확인할 수 있는 정보:
- 엔진 연결 상태 (연결됨/끊어짐)
- 서비스별 호출 테스트
- 모델 목록 및 캐시 상태
- 실시간 로그

### 2.2 연결 확인

Autobase 스크립트에서:
```
int connected = PythonAiIsConnected()
// 1 = 연결됨, 0 = 미연결
```

---

## 3. 스크립트 함수 사용법

### 3.1 전력 예측

현재 전력값을 입력하면 예측값을 반환합니다.

```
// 현재 전력 150.5 kW → 예측값 반환
double predicted = PythonAiPredictPower(150.5)
```

히스토리 데이터를 함께 보내면 더 정확한 예측이 가능합니다:
```
string result = PythonAiCall("predict.power",
    "{\"current_kw\": 150.5, \"history\": [140, 142, 145, 148, 150.5]}")
// result: {"ok":true,"result":{"predicted_kw":155.2,"confidence":0.87,"method":"statistics"}}
```

**예측 방식 (자동 선택):**
| method | 설명 | confidence |
|--------|------|-----------|
| model | AI 모델 추론 (ONNX/sklearn) | 0.8~0.95 |
| statistics | 통계 회귀분석 (OLS) | R-squared 값 |
| fallback | 현재값 반환 (모델/이력 없음) | 0.0 |

### 3.2 추세 분석

시계열 데이터의 추세를 분석합니다.

```
// 값 배열을 JSON으로 전달
string trend = PythonAiTrend("[100, 105, 110, 108, 115, 120]")
// 결과: 기울기, 방향(상승/하락/안정), R-squared 등
```

### 3.3 상관관계 분석

두 변수 간 상관계수를 계산합니다.

```
double corr = PythonAiCorrelation(
    "[100, 105, 110, 115, 120]",   // X: 온도
    "[200, 210, 215, 225, 230]")   // Y: 전력
// corr: 0.98 (강한 양의 상관관계)
```

### 3.4 시스템 상태 조회

```
string status = PythonAiStatus()
// 결과: 엔진 버전, 가동시간, 서비스 목록, 모델 목록 등
```

### 3.5 범용 서비스 호출

모든 서비스를 직접 호출할 수 있습니다.

```
// 서비스명과 JSON payload로 호출
string result = PythonAiCall("analysis.trend",
    "{\"values\": [100, 105, 110, 108, 115]}")

// 타임아웃 지정 (5000ms = 5초)
string result = PythonAiCallTimeout("train.start",
    "{\"model_name\": \"my_model\", \"data_path\": \"data.csv\"}", 60000)
```

---

## 4. 사용자 Python 스크립트

### 4.1 기본 실행

Autobase에서 Python 코드를 직접 실행할 수 있습니다.

```
// 간단한 계산
string result = PythonAiScript("emit_result(2 + 3)")
// result: {"ok":true,"result":{"result":5}}

// 여러 줄 코드
string result = PythonAiScript(
    "import math\n" +
    "area = math.pi * 5 ** 2\n" +
    "emit_result({'area': round(area, 2)})")
```

### 4.2 SCADA 태그 읽기 (Preload 방식)

payload에 `_read_tags`를 지정하면 C#이 태그값을 미리 읽어서 Python에 전달합니다.

```
string result = PythonAiScriptEx(
    "# tag_read()로 프리로드된 태그값 읽기\n" +
    "temp = tag_read('Station1.Temperature')\n" +
    "power = tag_read('Station1.Power.kW')\n" +
    "log_info('온도: ' + str(temp) + ', 전력: ' + str(power))\n" +
    "if temp > 35:\n" +
    "    emit_result({'alarm': True, 'message': '고온 경보'})\n" +
    "else:\n" +
    "    emit_result({'alarm': False, 'status': 'normal'})",
    "{\"_read_tags\": [\"Station1.Temperature\", \"Station1.Power.kW\"]}")
```

### 4.3 SCADA 태그 쓰기

태그 쓰기에는 명시적 권한이 필요합니다.

```
string result = PythonAiScriptEx(
    "temp = tag_read('Station1.Temperature')\n" +
    "if temp > 40:\n" +
    "    tag_write('Station1.Alarm.HighTemp', 1)\n" +
    "    tag_write('Station1.Fan.Speed', 100)\n" +
    "    log_warning('고온 감지: 팬 속도 100% 설정')\n" +
    "emit_result({'temp': temp, 'action_taken': temp > 40})",
    "{\"_read_tags\": [\"Station1.Temperature\"], \"_allow_tag_write\": true}")
```

**중요:** `_allow_tag_write: true`가 없으면 tag_write()는 실패합니다.

### 4.4 데이터 분석 스크립트

numpy, pandas, scipy가 사용 가능합니다.

```
string result = PythonAiScriptEx(
    "import numpy as np\n" +
    "import statistics\n" +
    "values = payload['measurements']\n" +
    "emit_result({\n" +
    "    'mean': round(np.mean(values), 2),\n" +
    "    'std': round(np.std(values), 2),\n" +
    "    'median': statistics.median(values),\n" +
    "    'min': min(values),\n" +
    "    'max': max(values),\n" +
    "    'count': len(values)\n" +
    "})",
    "{\"measurements\": [23.1, 24.5, 22.8, 25.0, 23.7, 24.2]}")
```

### 4.5 스크립트에서 사용 가능한 함수

| 함수 | 설명 | 예시 |
|------|------|------|
| `tag_read(name)` | 태그 현재값 읽기 | `tag_read('S1.Temp')` |
| `tag_write(name, value)` | 태그 쓰기 (권한 필요) | `tag_write('S1.Alarm', 1)` |
| `emit_result(data)` | 결과 반환 | `emit_result({'value': 42})` |
| `print(...)` | 출력 (캡처됨) | `print('디버그:', x)` |
| `log_info(msg)` | 정보 로그 | `log_info('처리 완료')` |
| `log_warning(msg)` | 경고 로그 | `log_warning('임계값 초과')` |
| `log_error(msg)` | 에러 로그 | `log_error('계산 실패')` |
| `payload` | 입력 데이터 | `payload['my_param']` |

### 4.6 사용 가능한 Python 모듈

`math`, `statistics`, `datetime`, `json`, `collections`, `itertools`,
`functools`, `operator`, `decimal`, `fractions`, `random`, `string`,
`re`, `time`, `copy`, **`numpy`**, **`pandas`**, **`scipy.stats`**

### 4.7 제한사항

- 실행 시간: 최대 30초
- 메모리: 최대 256MB
- 코드 크기: 최대 256KB
- 파일/네트워크/프로세스 접근: 불가
- os, sys, subprocess 등 시스템 모듈: 사용 불가

---

## 5. 응답 구조 이해하기

모든 함수의 JSON 응답은 동일한 구조입니다:

```json
{
  "ok": true,           // 성공 여부
  "result": { ... },    // 결과 데이터
  "error": "...",       // 에러 시 메시지
  "elapsed_ms": 12.3    // 처리 시간 (ms)
}
```

**스크립트 실행 응답 (추가 필드):**
```json
{
  "ok": true,
  "result": {
    "status": "ok",
    "result": { ... },         // emit_result() 값
    "output": ["디버그: 42"],  // print() 출력
    "logs": [                  // log_xxx() 기록
      {"level": "info", "message": "처리 완료", "time": "..."}
    ],
    "duration_ms": 5.2
  }
}
```

---

## 6. 에러 처리

### 일반적인 에러

| 에러 | 원인 | 해결 |
|------|------|------|
| `NULL_RESPONSE` | 엔진 미연결 또는 타임아웃 | PythonAiIsConnected() 확인 |
| `PERMISSION_DENIED` | 권한 부족 | context.permissions 확인 |
| `VALIDATION_ERROR` | 잘못된 입력 형식 | payload 형식 확인 |
| `Script timeout` | 스크립트 시간 초과 | 코드 최적화 또는 타임아웃 증가 |
| `BLOCKED_PATTERN` | 보안 차단 패턴 사용 | 금지된 코드 패턴 제거 |
| `Module not allowed` | 허용되지 않은 모듈 import | 허용 모듈 목록 확인 |

### 스크립트에서 에러 확인

```
string result = PythonAiScript("1 / 0")  // ZeroDivisionError
// result: {"ok":true,"result":{"status":"error","error":"division by zero"}}
```

---

## 7. 실전 활용 예제

### 7.1 전력 수요 예측 + 알람

```
// 5분마다 실행되는 주기 스크립트에서:
double currentKw = TagRead("Station1.Power.kW")
double predicted = PythonAiPredictPower(currentKw)

if predicted > 500.0
    TagWrite("Station1.Alarm.OverloadWarning", 1)
    LogWrite("전력 초과 예측: " + predicted + " kW")
end if
```

### 7.2 온도-전력 상관분석 자동화

```
// 최근 24시간 데이터로 상관분석
string temps = HistoryToJson("Station1.Temperature", -24h, now)
string powers = HistoryToJson("Station1.Power.kW", -24h, now)
double correlation = PythonAiCorrelation(temps, powers)

TagWrite("Station1.Analysis.TempPowerCorr", correlation)
```

### 7.3 이상 감지 스크립트

```
string result = PythonAiScriptEx(
    "import numpy as np\n" +
    "values = payload['recent_values']\n" +
    "mean = np.mean(values)\n" +
    "std = np.std(values)\n" +
    "latest = values[-1]\n" +
    "z_score = abs(latest - mean) / std if std > 0 else 0\n" +
    "emit_result({\n" +
    "    'is_anomaly': z_score > 3,\n" +
    "    'z_score': round(z_score, 2),\n" +
    "    'threshold': 3.0\n" +
    "})",
    "{\"recent_values\": [100,102,98,101,99,103,100,150]}")
```

# Autobase 특허 가능 기술 분석 보고서

> 작성일: 2026-04-13
> 대상: Autobase48 SCADA/HMI + Python AI Engine
> 목적: 잠재적 특허 출원 후보 기술 식별

---

## 요약

Autobase는 .NET Framework 4.8 기반 SCADA/HMI에 Python AI 엔진을 결합한 하이브리드 산업 자동화 플랫폼으로, 다수의 기술적 독창성을 보유하고 있습니다. 본 문서는 특허 출원 가능성이 높은 10개 핵심 기술을 정리합니다.

---

## 1. SCADA 태그 Preload/Snapshot 기반 .NET-Python 브리지

**위치:** `python_ai_engine/docs/01_Technical_Documentation.md`, `AutobasePrime/LocalMain/PythonAiScriptBridge.cs`

**특징:**
- C# 측에서 스크립트가 사용할 태그를 사전 파싱(`payload._read_tags`)하여 `TagLib.GetStructPublic()`으로 메모리 스냅샷 생성
- Python 샌드박스에 스냅샷을 JSON으로 주입 → 실행 중 추가 IPC 없이 `tag_read()` 처리
- `tag_write()`는 지연 큐(`_pending_writes`)에 적재 후 응답으로 일괄 반환 → C#이 `PlcScan.SetTagValue()`로 원자적 반영

**특허성:**
- 프로세스 경계를 가로지르는 실시간 태그 접근을 "선읽기 + 지연쓰기" 2단계로 압축해 왕복 레이턴시 제거
- 트랜잭션적 일관성을 샌드박스와 무관하게 .NET 측에서 보장하는 구조

---

## 2. 5계층 Python 스크립트 샌드박스 (산업 제어 특화)

**위치:** `python_ai_engine/sandbox/policy.py`, `sandbox/runner.py`, `sandbox/resource_limiter.py`

**5계층 구성:**
| 계층 | 메커니즘 |
|------|---------|
| Safe Builtins | `exec/eval/open/compile` 등 16개 제거 |
| Import Whitelist | math/numpy/pandas 등 25개만 허용 |
| Code Validation | `__subclasses__`, `os.system` 등 15개 AST 패턴 사전 차단 |
| Resource Limiter | 타임아웃 5초, 메모리 256MB 상한 |
| Restricted Globals | SCADA API 9개 함수만 주입 |

**특허성:**
- 일반 Python 샌드박스와 달리 "SCADA API 최소 주입 + 태그 쓰기 권한 분리(`script.tag_write`)" 모델
- payload에 `_allow_tag_write: true` 명시 시에만 런타임 권한 부여 → 이중 잠금

---

## 3. DemandNew — EWMA 예측 + 우선순위 기반 다단계 부하 제어

**위치:** `AutobasePrime/docs/DemandNew-System.md`, `AutobasePrime/LocalMain/DemandNew/*`

**핵심 구성:**
- 3종 계측 입력(DirectKW / DeltaKWH / Pulse) + 스파이크 자동 검출 및 보정
- EWMA 트렌드 윈도우로 구간말(15분) 전력 예측
- 시간대별 TariffZone(경부하/중간/최대) 목표 계약전력 동적 적용
- LoadGroupManager: Priority 기반 단계적 부하 차단/복귀, 최소 on/off 시간 보장
- PostgreSQL 이력 + JSON 스냅샷 백업 + CSV 내보내기

**특허성:**
- 시간대별 계약전력 변동과 다단계 우선순위 기반 자동 부하 스케줄링의 조합
- 예측-목표-부하 3요소를 단일 블록 단위로 독립 엔진화한 다중 블록 병렬 제어 구조

---

## 4. Recipe/Preset — ISA-88 경량 구현 + 다층 무한루프 방어

**위치:** `AutobasePrime/docs/DEV_RecipePreset_TechnicalReference.md`

**Recipe (DB 기반 배치 제어):**
- Step → Unit → Expression → Transition → Action 계층
- RecipeExpressionEvaluator: 재귀 내림차순 파서 (`or/and/not/comparison`)
- 부동소수점 epsilon(0.0001) 비교, 평가 예외 시 `abort_actions` 폴백

**Preset (JSON 기반 태그 변형):**
- 태그 별칭-값 매핑으로 템플릿 Variant 자동 생성
- 분산환경 파일 동기화: PortalServerWeb(IIS) → WCF net.tcp:8732 → LocalMain 단일 경유 I/O

**4단계 루프 방어:**
1. Step 루프 카운터
2. 배치 총 실행 횟수 상한
3. 전이(transition) 평가 횟수 제한
4. 24시간 타임아웃

**특허성:**
- ISA-88 상태 머신 위에 Abort/Exception 전이 우선순위 + 다층 루프 안전장치
- 분산 IIS/클라이언트/로컬 환경에서 JSON 일관성을 메시지 기반 락으로 보장하는 단일 경유점 패턴

---

## 5. OPC UA Client — Runtime/Host 정책 위임 아키텍처

**위치:** `AutobasePrime/OPCUA.Client.Core/`, `AutobasePrime/OpcUa.Client.Host/`

**특징:**
- `OpcUaRuntime`(Core)는 순수 비동기 통신만 담당 (UI/설정 저장 없음)
- `Host`가 재연결 정책, 설정 로드/저장, SynchronizationContext 선택을 위임
- 다중 서버 레지스트리 + 런타임 동적 추가(`ServerAdded` 이벤트)
- 콘솔/WinForms/WPF/WebService 어느 호스트에서든 동일 Core 재사용

**특허성:**
- 산업용 통신 런타임과 호스트 정책의 엄격한 계층 분리 → 커널 모드 재사용성과 감사(audit) 가능성 향상

---

## 6. AI 모델 Hot Reload + 자동 롤백

**위치:** `python_ai_engine/models/hot_reload.py`, `models/registry.py`, `models/cache.py`

**동작:**
- 30초 주기로 `registry.json` 및 파일 변경 감지
- 새 모델 로드 실패 시 이전 버전으로 즉시 롤백
- LRU 캐시(5개, 512MB)로 버전 공존, 추론 중단 없이 스와핑

**특허성:**
- 운영 중단 없는 ML 모델 배포 + 장애 자동 복구를 SCADA 실시간 루프와 결합한 드문 조합

---

## 7. 3-Tier 추론 자동 폴백 (predict.power)

**위치:** `python_ai_engine/services/predict_service.py`

**계층:**
1. ONNX/sklearn 모델 (고정밀, ~3ms)
2. OLS 선형회귀(history 기반 통계)
3. 현재값 반환, `confidence=0.0`

**특허성:**
- 모델 부재/로드 실패/히스토리 부족 등 각 실패 모드에 대응하는 자동 다운그레이드
- 신뢰도(`confidence`) 필드를 API 계약에 포함시켜 호출측이 결정 가능하게 한 설계

---

## 8. Excel Worker 분산 리포트 아키텍처

**위치:** `AutobasePrime/docs/excel-worker-architecture-plan.md`, `ExcelReportData/`

**특징:**
- IIS 내 Excel DCOM을 금지하고 Windows 인터랙티브 세션의 독립 Worker로 분리
- 요청 API → 큐 → Excel Worker → 결과 저장소 파이프라인
- 작업당 타임아웃, 고아 Excel 프로세스 정리, 복구 정책
- 기존 VBA/템플릿 호환 유지

**특허성:**
- 웹/서비스 환경에서 Excel 의존성을 제거하면서도 레거시 템플릿 자산을 재사용하는 중계 아키텍처

---

## 9. RecipeExpressionEvaluator — 안전한 산업용 조건식 DSL

**위치:** `AutobasePrime/docs/DEV_RecipePreset_TechnicalReference.md` §4.6

**문법 예:** `$TAG_NAME > 80 && $DI_READY == 1`

**특성:**
- 재귀 하향 파서, 연산자 우선순위 고정
- `$TAG` 참조는 `TagLib.GetStructPublic()`로 실시간 평가
- 부동소수 비교 epsilon 허용, 예외 → 안전 폴백

**특허성:**
- 일반 표현식 엔진(NCalc 등)과 달리 SCADA 태그 직접 바인딩 + 공정 안전 폴백을 포함한 DSL

---

## 10. Python AI 스크립트-SCADA Tag 권한 이중화 모델

**위치:** `python_ai_engine/router/auth.py`, `sandbox/api_bridge.py`

**메커니즘:**
- `script.execute`: 읽기 + 실행만 허용(기본)
- `script.tag_write`: payload에 `_allow_tag_write: true` 동시 존재 시에만 활성
- `python.admin`은 모든 서비스 우회 가능
- strict/non-strict 모드 → 개발/운영 분리

**특허성:**
- 스크립트 실행 권한과 쓰기 권한을 "정적 권한 + 런타임 플래그" 이중 조건으로 결합한 세분화 모델

---

## 특허 출원 우선순위 (권장)

| 순위 | 기술 | 사유 |
|------|------|------|
| 1 | DemandNew (EWMA + 다단계 부하) | 독립 가치, BEMS 시장 직결, 구현 완성도 높음 |
| 2 | Preload/Snapshot .NET-Python 브리지 | 아키텍처 원천 기술, 타사 회피 어려움 |
| 3 | Recipe/Preset + 다층 루프 방어 | ISA-88 확장, 안전성 차별화 |
| 4 | 5계층 샌드박스 + 권한 이중화 | SCADA 결합형 샌드박스는 유사 선행기술 적음 |
| 5 | 모델 Hot Reload + 자동 롤백 | ML + 산업제어 조합의 희소성 |
| 6 | Excel Worker 분산 | 구현 완결 시 방어 특허로 유용 |

---

## 참고

- 국내 출원 전 선행기술 조사(KIPRIS, Google Patents, USPTO) 필수
- SCADA/BEMS 도메인은 Siemens, Schneider, Rockwell, GE Digital 등 기존 특허 다수 존재 → 청구항 범위 설계 시 회피설계 필요
- Python/ONNX/OPC UA 등 오픈소스 의존성과 분리되는 "조합 발명" 관점에서 청구항 작성 권장

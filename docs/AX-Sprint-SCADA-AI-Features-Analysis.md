# AX-Sprint 사업 기반 Autobase SCADA AI 접목 기능 및 과제영역 분석

> 분석일: 2026-03-18
> 대상 사업: AI 응용제품 신속 상용화 지원사업 (AX-Sprint)
> 사업 규모: 총 7,540억원 (2026~2027)
> 대상 시스템: Autobase SCADA/HMI 플랫폼

---

## 1. Autobase 현재 역량 분석 (코드베이스 기반)

### 1.1 핵심 보유 기술 스택

| 영역 | 현재 상태 | 과제 활용 가치 |
|------|----------|--------------|
| **SCADA/HMI 플랫폼** | .NET 4.8.1 WinForms 기반 성숙 플랫폼 | ★★★★★ 즉시 활용 |
| **Python AI 엔진** | TCP 기반 독립 AI 서비스 (4-Phase 구현 완료) | ★★★★★ 핵심 차별화 |
| **DemandNew 전력관리** | EWMA 예측, 다단계 부하차단, 시간대별 요금 | ★★★★★ BEMS 핵심 |
| **OPC-UA 통신** | 풀스택 클라이언트 (인증, 인증서 관리) | ★★★★☆ 산업 호환성 |
| **Modbus/커스텀 프로토콜** | RTU/TCP, GlofaCnet, CDTP 등 | ★★★★☆ 현장 연결성 |
| **TimescaleDB 시계열 DB** | PostgreSQL + Hypertable, 밀리초 데이터 수집 | ★★★★★ AI 학습 데이터 |
| **Excel 리포트 엔진** | OpenXML/ClosedXML 자동 보고서 생성 | ★★★★☆ 운영 리포트 |
| **Avalonia Launcher** | 모던 크로스플랫폼 UI | ★★★☆☆ 확장성 |

### 1.2 AI 엔진 현재 구현 상태 (python_ai_engine)

```
서비스 구조:
├── predict/power    → 3-Tier 전력 예측 (ONNX → OLS → Fallback)
├── analysis/trend   → OLS 선형회귀 (기울기, R²)
├── analysis/correlation → 피어슨 상관계수
├── analysis/report  → 다중 태그 통계 요약
├── vision/detect    → YOLOv8 객체 탐지 (선택)
├── vision/ocr       → 문자 인식 (선택)
├── train/start|status|cancel → 모델 학습 관리
├── script/execute   → 5계층 보안 샌드박스 스크립트 실행
└── system/ping|status|metrics|health → 시스템 모니터링
```

### 1.3 하드웨어 포트폴리오

| 제품 | 과제 내 역할 |
|------|------------|
| **리눅스 터치패널** | Edge AI 디바이스 (현장 1차 분석) |
| **윈도우 터치패널** | HMI/SCADA UI 단말 (운영자 인터페이스) |
| **SPD (서지보호기)** | 전력 품질 보호 + 이벤트 데이터 발생 장비 |
| **SBB (Surge Black Box)** | 서지 이벤트 로깅 + AI 학습 데이터 생성 |

---

## 2. AX-Sprint 5대 분야별 접목 가능 기능 분석

### 2.1 제조 (중기부/산업부) — ★★★★★ 최적합

#### 접목 가능 기능

| 기능 | Autobase 연계 모듈 | 구현 난이도 | 상용화 기간 |
|------|-------------------|-----------|-----------|
| **AI 설비 이상 감지** | python_ai_engine → analysis/trend + predict/power | 낮음 | 6개월 |
| **공정 최적화 시스템** | DemandNew + AI 예측 엔진 | 중간 | 12개월 |
| **비전 검사 통합** | vision/detect (YOLOv8) + SCADA 알람 | 중간 | 12개월 |
| **작업자 안전 모니터링** | vision/detect + 알람 시스템 | 중간 | 12개월 |
| **예측 유지보수** | TimescaleDB 시계열 + AI trend 분석 | 낮음 | 6개월 |

#### 구체적 과제 설계

**과제명: AI 기반 제조 설비 이상탐지 및 예측유지보수 SCADA 플랫폼**

```
[PLC/센서] → [Modbus/OPC-UA] → [Edge 리눅스패널 (1차 이상감지)]
                                        ↓
                              [Autobase SCADA 서버]
                                        ↓
                              [Python AI Engine]
                              ├── predict/power (부하 예측)
                              ├── analysis/trend (설비 열화 추세)
                              ├── analysis/correlation (설비간 상관)
                              └── script/execute (Z-score 이상탐지)
                                        ↓
                              [알람/리포트/대시보드]
```

**활용 기존 코드:**
- `LocalMain/PythonAi/PythonAiManager.cs` — AI 엔진 TCP 통신
- `LocalMain/PythonAi/PythonAiHealthMonitor.cs` — 연결 상태 모니터링
- `LocalMain/demandnew/` — 전력 데이터 수집/분석 로직
- `python_ai_engine/services/predict_service.py` — 3-Tier 예측
- `python_ai_engine/services/analysis_service.py` — 추세/상관 분석

---

### 2.2 국토·교통 (국토부) — ★★★★☆ 매우 적합

#### 접목 가능 기능

| 기능 | Autobase 연계 모듈 | 구현 난이도 | 상용화 기간 |
|------|-------------------|-----------|-----------|
| **건설현장 안전 SCADA** | SCADA + vision/detect (CCTV 연동) | 중간 | 12개월 |
| **도로 유지보수 AI 경고** | Edge AI + 알람 시스템 | 중간 | 12개월 |
| **인프라 설비 예측 관리** | TimescaleDB + AI trend 분석 | 낮음 | 6개월 |

#### 구체적 과제 설계

**과제명: AI 기반 건설현장 안전 모니터링 통합 시스템**

```
[CCTV/IoT센서] → [Edge 리눅스패널]
                     ├── vision/detect (위험 상황 탐지)
                     ├── vision/ocr (안전장비 미착용 감지)
                     └── 1차 이상 감지
                           ↓
                  [Autobase SCADA 중앙 서버]
                     ├── 실시간 위험도 대시보드
                     ├── 다단계 알람 (음성/문자/경광등)
                     └── 일일 안전 리포트 자동 생성
```

---

### 2.3 보건·복지·환경 — 기후부 (★★★★★ 핵심 타겟)

#### BEMS/ZEB 접목 기능 (가장 강력한 포지션)

| 기능 | Autobase 연계 모듈 | 구현 난이도 | 상용화 기간 |
|------|-------------------|-----------|-----------|
| **전력 수급 예측** | DemandNew EWMA + AI predict/power | 매우 낮음 | 3개월 |
| **피크 전력 관리** | DemandNew 다단계 부하차단 | 이미 구현됨 | 즉시 |
| **에너지 최적화** | DemandNew + AI 분석 | 낮음 | 6개월 |
| **전력 품질 분석** | SPD/SBB + TimescaleDB | 낮음 | 6개월 |
| **서지 예측/예방** | SBB 데이터 + AI predict | 중간 | 12개월 |
| **탄소 배출 분석** | 에너지 데이터 + 리포트 엔진 | 낮음 | 6개월 |
| **ZEB 에너지 자립률** | BEMS 데이터 통합 분석 | 중간 | 12개월 |

#### 구체적 과제 설계 (★ 최우선 추천)

**과제명: AI 기반 전력품질·에너지 통합 BEMS 플랫폼 개발 (ZEB 대응형)**

```
[현장 설비/건물]
     ↓
[SPD/SBB] ──→ 서지 이벤트 + 전력 품질 데이터
[계측기]  ──→ 전압/전류/전력/역률 데이터
[IoT센서] ──→ 온습도/CO2/조도 데이터
     ↓
[Edge Layer: 리눅스 터치패널]
     ├── 실시간 데이터 수집 (Modbus RTU/TCP)
     ├── 1차 이상 감지 (룰 기반)
     └── Edge AI 추론 (경량 ONNX 모델)
     ↓
[AI Layer: Python AI Engine]
     ├── predict/power → 피크 전력 예측 (3-Tier)
     ├── analysis/trend → 에너지 사용 추세 분석
     ├── analysis/correlation → 설비간 에너지 상관
     ├── script/execute → Z-score 이상 탐지
     └── (신규) 서지 패턴 분석 모델
     ↓
[Application Layer: SCADA/BEMS 서버]
     ├── DemandNew 엔진 → 실시간 수요 관리
     ├── TimescaleDB → 시계열 데이터 저장
     ├── 알람 시스템 → 다단계 경보
     └── 리포트 엔진 → 자동 에너지 보고서
     ↓
[UI Layer]
     ├── 윈도우 터치패널 → 현장 운영 HMI
     ├── 웹 포털 → 원격 모니터링
     └── Excel 리포트 → 관리자 보고서
```

**기존 코드 직접 활용:**
- `LocalMain/demandnew/DemandEngine.cs` — EWMA 수요 예측, 부하차단 로직
- `LocalMain/demandnew/DemandDataCollector.cs` — 전력 데이터 수집
- `LocalMain/demandnew/DemandCsvExporter.cs` — 데이터 내보내기
- `python_ai_engine/services/predict_service.py` — 3-Tier 전력 예측
- `python_ai_engine/services/analysis_service.py` — 추세/상관 분석
- `python_ai_engine/models/model_cache.py` — 모델 캐시/핫리로드
- `LocalMain/DataSave/TrendSaveManager.cs` — 시계열 데이터 저장
- `Dll/ReportModule/` — 리포트 생성 엔진

---

### 2.4 생활·보안·방산 — 과기정통부 (★★★☆☆ 틈새)

#### 접목 가능 기능

| 기능 | Autobase 연계 모듈 | 구현 난이도 |
|------|-------------------|-----------|
| **AI 기반 물리 보안** | SCADA + vision/detect | 중간 |
| **시설 통합 안전 관리** | SCADA + 알람 + 리포트 | 낮음 |
| **사이버 보안 모니터링** | OPC-UA 인증 + 시스템 모니터링 | 높음 |

---

## 3. 신규 개발 필요 기능 (과제 수행 시)

### 3.1 AI 엔진 확장 (python_ai_engine)

```python
# 신규 서비스 추가 필요
services/
├── predict_service.py      # 기존: 전력 예측
├── (신규) surge_service.py  # 서지 패턴 분석/예측
├── (신규) energy_optimizer.py  # 에너지 최적화 추천
├── (신규) zeb_calculator.py   # ZEB 에너지 자립률 계산
├── (신규) carbon_service.py   # 탄소 배출량 산정
└── (신규) anomaly_service.py  # 고급 이상탐지 (Isolation Forest, AutoEncoder)
```

### 3.2 SCADA/HMI 확장 (AutobasePrime)

```
신규 모듈:
├── BEMSModule/           # BEMS 전용 대시보드
│   ├── EnergyDashboard   # 에너지 현황 종합
│   ├── ZEBStatus         # ZEB 자립률 현황
│   ├── CarbonTracker     # 탄소 배출 추적
│   └── SurgeAnalyzer     # 서지 분석 뷰
├── EdgeManager/          # Edge 디바이스 관리
│   ├── EdgeDeployment    # 모델 배포
│   └── EdgeMonitor       # Edge 상태 모니터링
└── AIReport/             # AI 분석 리포트
    ├── PredictReport     # 예측 결과 보고서
    ├── AnomalyReport     # 이상 탐지 보고서
    └── EnergyReport      # 에너지 분석 보고서
```

### 3.3 Edge AI 구현 (리눅스 터치패널)

```
신규 개발:
├── ONNX Runtime 경량 모델 배포
├── 실시간 1차 이상 감지 (로컬)
├── 네트워크 단절 시 독립 운영
└── SCADA 서버 자동 동기화
```

---

## 4. 과제 트랙별 최종 추천

### 4.1 1순위: 기후부 — BEMS/ZEB (★★★★★)

**과제명:** AI 기반 전력품질·에너지 통합 BEMS 플랫폼 개발 (ZEB 대응형)

**선정 확률 높은 이유:**
- DemandNew 시스템이 이미 BEMS 핵심 기능 구현 완료
- SPD/SBB → "전력 품질 + 에너지 관리 통합"은 유니크 포지션
- AI 엔진 3-Tier 예측이 이미 동작 중
- 1~2년 내 상용화 조건 충족 (기존 코드 활용)

**정량 KPI:**
- 에너지 사용량 15% 이상 절감
- 설비 고장률 20% 감소
- 전력 피크 10% 감소
- AI 이상 탐지 정확도 90% 이상

**예산 규모:** 10~15억원 (2년)

---

### 4.2 2순위: 중기부/산업부 — 제조 (★★★★☆)

**과제명:** AI 기반 제조 설비 이상탐지·예측유지보수 스마트 SCADA 플랫폼

**선정 확률 높은 이유:**
- SCADA/HMI가 핵심 역량
- 제조 현장 PLC/센서 통신 경험 풍부
- "중대재해 대응" 키워드 활용 가능

**예산 규모:** 8~12억원 (2년)

---

### 4.3 3순위: 국토부 — 건설/인프라 안전 (★★★☆☆)

**과제명:** AI 기반 건설현장·인프라 안전 모니터링 통합 시스템

**예산 규모:** 6~10억원 (2년)

---

## 5. 컨소시엄 구성 전략

```
[주관기관: 자사]
├── 역할: 총괄 / SCADA 플랫폼 / HW 디바이스 / 시스템 통합
├── 기여: Autobase SCADA, 터치패널, SPD/SBB, DemandNew
│
[참여기관 1: AI 전문기업]
├── 역할: AI 모델 개발 / 고급 분석 알고리즘
├── 기여: 이상탐지 모델, 예측 모델, Edge AI 최적화
│
[참여기관 2: 대학/연구기관]
├── 역할: 알고리즘 검증 / 논문 / 기술 자문
├── 기여: 학술적 근거, 성능 평가 방법론
│
[실증기업: 빌딩/공장 운영사]
├── 역할: 실증 환경 제공 / 운영 데이터
├── 기여: 현장 적용 검증, 사용자 피드백
```

---

## 6. 기술 아키텍처 상세 (제안서용)

### 6.1 전체 시스템 구성도

```
┌─────────────────────────────────────────────────────────┐
│                    클라우드/웹 계층                        │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐              │
│  │ 웹 포털   │  │ 모바일앱  │  │ API 서버  │              │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘              │
└───────┼──────────────┼─────────────┼────────────────────┘
        └──────────────┼─────────────┘
                       ↓
┌─────────────────────────────────────────────────────────┐
│                 SCADA/BEMS 서버 계층                      │
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌───────────────┐ │
│  │ Autobase     │  │ Python AI    │  │ PostgreSQL    │ │
│  │ SCADA Core   │  │ Engine       │  │ + TimescaleDB │ │
│  │              │  │              │  │               │ │
│  │ · DemandNew  │←→│ · predict    │←→│ · minute_data │ │
│  │ · 알람관리    │  │ · analysis   │  │ · hour_data   │ │
│  │ · 스케줄      │  │ · vision     │  │ · millidata   │ │
│  │ · 리포트      │  │ · script     │  │ · alarms      │ │
│  │ · OPC-UA     │  │ · train      │  │ · logs        │ │
│  └──────┬───────┘  └──────────────┘  └───────────────┘ │
└─────────┼───────────────────────────────────────────────┘
          ↓
┌─────────────────────────────────────────────────────────┐
│                    Edge 계층                              │
│  ┌──────────────┐  ┌──────────────┐  ┌───────────────┐ │
│  │ 리눅스 패널   │  │ 윈도우 패널   │  │ Gateway       │ │
│  │ (Edge AI)    │  │ (현장 HMI)   │  │ (프로토콜변환) │ │
│  └──────┬───────┘  └──────┬───────┘  └───────┬───────┘ │
└─────────┼────────────────┼────────────────────┼─────────┘
          ↓                ↓                    ↓
┌─────────────────────────────────────────────────────────┐
│                   현장 설비 계층                           │
│  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐ ┌──────┐ ┌────────┐ │
│  │ PLC │ │센서  │ │계측기│ │ SPD │ │ SBB  │ │ CCTV   │ │
│  └─────┘ └─────┘ └─────┘ └─────┘ └──────┘ └────────┘ │
└─────────────────────────────────────────────────────────┘
```

### 6.2 데이터 흐름

```
[수집] SPD/SBB 서지 이벤트 → TimescaleDB millidata (ms단위)
       계측기 전력 데이터   → TimescaleDB minute_data (분단위)
       IoT 환경 데이터     → TimescaleDB minute_data (분단위)
                ↓
[분석] Python AI Engine
       ├── 실시간: predict/power (피크 예측, 신뢰도 포함)
       ├── 배치: analysis/trend (추세 분석, R² 판단)
       ├── 배치: analysis/correlation (설비간 상관)
       └── 실시간: script/execute (Z-score 이상 탐지)
                ↓
[제어] DemandNew Engine
       ├── EWMA 기반 수요 예측
       ├── 다단계 부하차단 (우선순위별)
       ├── 시간대별 타깃 관리
       └── 안전 마진 자동 조절
                ↓
[출력] 알람 → 단계별 경보 (주의/경고/위험)
       리포트 → Excel 자동 생성 (일/주/월)
       대시보드 → 실시간 에너지 현황
       API → 외부 시스템 연동
```

---

## 7. 예산 구조 (10억 기준 시나리오)

### 7.1 직접비

| 항목 | 금액 (백만원) | 비율 | 세부 내용 |
|------|-------------|------|----------|
| **인건비** | 450 | 45% | PM 1, SW개발 3, AI개발 2, HW 1, 품질 1 |
| **장비비** | 200 | 20% | Edge 패널, 서버, SPD/SBB, 계측기, 네트워크 |
| **AI 개발비** | 150 | 15% | 모델 개발, 학습 인프라, GPU 서버 |
| **실증비** | 100 | 10% | 현장 설치, 데이터 수집, 성능 검증 |
| **위탁연구비** | 50 | 5% | 대학 알고리즘 검증 |
| **기타** | 50 | 5% | 인증, 특허, 출장, 회의 |
| **합계** | **1,000** | **100%** | |

### 7.2 재원 구성

| 구분 | 금액 | 비율 |
|------|------|------|
| 정부 출연금 | 700 | 70% |
| 민간 부담금 (현금) | 150 | 15% |
| 민간 부담금 (현물) | 150 | 15% |

---

## 8. 개발 로드맵 (2년)

### 1차년도 (2026.06 ~ 2027.05)

| 분기 | 주요 과업 |
|------|----------|
| **Q1** | 시스템 설계, AI 모델 아키텍처, 실증 현장 확보 |
| **Q2** | DemandNew → BEMS 확장, 서지 분석 모듈 개발 |
| **Q3** | AI 모델 개발 (이상탐지, 에너지 예측), Edge AI 구현 |
| **Q4** | 1차 실증 설치, 데이터 수집 시작, 중간 평가 |

### 2차년도 (2027.06 ~ 2028.05)

| 분기 | 주요 과업 |
|------|----------|
| **Q1** | AI 모델 고도화, ZEB 자립률 계산 모듈 |
| **Q2** | 2차 실증 (다중 현장), 성능 최적화 |
| **Q3** | 탄소 배출 분석, 리포트 고도화, 인증 |
| **Q4** | 최종 검증, 제품 패키징, 상용 출시 |

---

## 9. 차별화 전략 요약

### 기존 BEMS vs Autobase AI BEMS

| 항목 | 기존 BEMS | Autobase AI BEMS |
|------|----------|-----------------|
| 에너지 관리 | 모니터링 위주 | AI 예측 + 자동 최적화 |
| 전력 품질 | 미지원 | SPD/SBB 연동 통합 분석 |
| 이상 탐지 | 임계치 기반 | AI 패턴 분석 (Z-score, Isolation Forest) |
| 예측 기능 | 없음 | 3-Tier 전력 예측 (ONNX/OLS/Fallback) |
| Edge 대응 | 없음 | 리눅스 패널 기반 Edge AI |
| 설비 보호 | 별도 시스템 | SCADA + SPD 통합 |
| 데이터 분석 | 기본 통계 | 추세/상관/이상탐지 AI 분석 |
| 리포트 | 수동 | Excel/PDF 자동 생성 |

### 핵심 차별화 문장 (제안서용)

> "기존 BEMS가 에너지 사용량의 단순 모니터링에 머물러 있는 것과 달리, 본 시스템은 전력 품질 분석(SPD/SBB)과 에너지 관리(BEMS)를 통합하고, 3-Tier AI 예측 엔진과 Edge AI를 적용하여 설비 보호·에너지 최적화·사고 예방을 동시에 달성하는 ZEB 대응형 스마트 에너지 플랫폼이다."

---

## 10. 즉시 실행 액션 플랜

| 순서 | 항목 | 기한 | 비고 |
|------|------|------|------|
| 1 | 과제 공고 확인 (기후부/국토부) | 2026.03.19~ | 공고 시작일 |
| 2 | 아이템 최종 선택 (BEMS/ZEB 추천) | 3월 내 | 1순위 확정 |
| 3 | 실증 기업 컨택 (빌딩/공장) | 3월 내 | 필수 |
| 4 | AI 파트너 컨택 | 3월 내 | 모델 개발 담당 |
| 5 | 대학 연구실 컨택 | 4월 초 | 알고리즘 검증 |
| 6 | 제안서 초안 작성 | 공고 후 2주 내 | 본 문서 기반 |
| 7 | 컨소시엄 MOU 체결 | 제출 2주 전 | 필수 서류 |
| 8 | 제안서 최종본 + 발표 준비 | 제출 1주 전 | 리허설 필수 |

---

## 부록: Autobase 핵심 소스 코드 맵 (과제 수행 시 활용)

```
AutobasePrime/
├── LocalMain/
│   ├── PythonAi/                    # AI 엔진 통합 (16개 파일)
│   │   ├── PythonAiManager.cs       # TCP 통신 관리
│   │   ├── PythonAiHealthMonitor.cs # 헬스체크
│   │   ├── FormPythonAiDashboard.cs # AI 대시보드 UI
│   │   └── PythonAiScriptBridge.cs  # 태그 데이터 교환
│   ├── demandnew/                   # 전력 수요 관리 (20개 파일)
│   │   ├── DemandEngine.cs          # EWMA 예측 + 부하차단
│   │   ├── DemandDataCollector.cs   # 전력 데이터 수집
│   │   └── DemandCsvExporter.cs     # 데이터 내보내기
│   ├── DataSave/                    # 데이터 저장
│   │   ├── TrendSaveManager.cs      # 시계열 저장
│   │   └── DatabaseHealthMonitor.cs # DB 상태 감시
│   ├── OPCUA/                       # OPC-UA 통신 (20+ 파일)
│   └── Schedule/                    # 스케줄 엔진
├── ExportServer/                    # 프로토콜 브릿지
├── ExportDll/                       # 프로토콜 드라이버
├── Dll/ReportModule/                # 리포트 엔진
└── Studio/                          # 설계 도구

python_ai_engine/
├── services/
│   ├── predict_service.py           # 3-Tier 전력 예측
│   ├── analysis_service.py          # 추세/상관 분석
│   ├── vision_service.py            # 객체탐지/OCR
│   ├── train_service.py             # 모델 학습
│   └── script_service.py            # 스크립트 샌드박스
├── models/
│   ├── model_cache.py               # 모델 캐시/핫리로드
│   └── model_registry.py            # 모델 레지스트리
└── sandbox/                         # 5계층 보안 샌드박스
```

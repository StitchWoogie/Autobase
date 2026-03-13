# DemandNew 사용자 운영문서

## 1. 개요
- DemandNew는 계약전력 초과를 방지하기 위해 예측 기반으로 부하를 자동 차단/복귀하는 디맨드제어 기능이다.
- 운영 모드:
  - `Shadow`: 판단만 수행(실제 제어 없음)
  - `Active`: 실제 부하 차단/복귀 수행

## 2. 시작 전 준비
- 블록별 필수 설정:
  - 블록 ID, 계약전력(`ContractKW`), 계측 태그(`MeterTagName`)
  - 제어할 부하 목록(Active 모드인 경우)
- 권장 설정:
  - 통신상태 DI 태그(`CommStatusTagName`)
  - 보호시간(`ProtectionTimeSec`) 및 부하별 최소 On/Off 시간
  - Failover 사용 시 DB DSN 확인

## 3. 주요 설정 항목 설명
- `IntervalMinutes`: 수요 구간(5~60분)
- `ClockAligned`: 구간 시작을 정시 슬롯(예: 15분 단위)으로 맞춤
- `SafetyFactor`: 목표 여유 계수(보통 0.90~0.98)
- `ShedMarginKW` / `RestoreMarginKW`: 차단/복귀 민감도
- `EwmaAlpha`, `TrendWindowSec`: 예측 반응속도/안정성
- `QualityBadBlockControl`: 계측 품질 불량 시 제어 차단 여부

## 4. 동작 원리(운영 관점)
1. 계측값을 읽어 현재 kW를 계산한다.
2. 남은 구간의 수요를 예측한다.
3. 목표전력(계약/시간대/DR/수동)을 계산한다.
4. 예측이 목표를 넘으면 우선순위 부하를 차단한다.
5. 예측이 충분히 낮아지면 차단된 부하를 복귀한다.

## 5. 차트 보는 법
- `Target`: 유효 목표(kW)
- `Forecast`: 구간 종료 시점 예측 수요(kW)
- `Current`: 현재 계측 kW
- `Elapsed`: 현재 구간 경과시간
- `Shed`: 현재 차단된 부하 수
- 예측선이 목표선 위에 계속 있으면 차단이 발생할 가능성이 높다.

## 6. 알람/이벤트
- 차단 시: 초과 예측 알람 발생
- 복귀 시: 여유 예측 상태로 복귀 이벤트 발생
- 제어 결과(성공/실패/타임아웃)는 제어 이벤트 이력으로 확인 가능

## 7. 이력/백업
- DB 저장 테이블:
  - `demand_interval`
  - `demand_control_event`
  - `demand_monthly_peak`
- DB 장애 시 백업(JSONL)으로 임시 저장 후 복구 시 자동 재적재 시도
- 설정 화면에서 수동 복구 버튼도 사용 가능

## 8. 운영 점검 체크리스트
- 차트 `Current` 값이 실계측과 일치하는지
- `Shadow`에서 판단만 되고 실제 출력 변화가 없는지
- `Active`에서 보호시간/최소시간이 의도대로 지켜지는지
- 통신 불량 시 품질 상태와 제어 차단 동작이 맞는지
- 월최대수요와 계약전력 이력이 정상 기록되는지

## 9. 자주 발생하는 증상과 조치
- 증상: 그래프가 바닥에 고정됨
  - 조치: 계측 태그/통신상태 태그/품질 상태 확인
- 증상: 차단이 너무 늦음
  - 조치: `SafetyFactor` 낮춤, `ShedMarginKW` 축소, 예측 설정 재조정
- 증상: 차단/복귀가 잦음
  - 조치: `ProtectionTimeSec` 증가, margin 확대, 부하 최소시간 확인
- 증상: 원격 차트 미갱신
  - 조치: DataGate 통신 상태 및 블록 ID 매칭 확인

## 10. 운영 권장값(초기 튜닝 예시)
- `SafetyFactor`: 0.95
- `ShedMarginKW`: 계약전력의 1~3%
- `RestoreMarginKW`: ShedMargin보다 약간 크게 설정
- `ProtectionTimeSec`: 60~180초
- `TrendWindowSec`: 60~180초
- `EwmaAlpha`: 0.2~0.35


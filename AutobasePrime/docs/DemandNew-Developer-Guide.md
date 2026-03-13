# DemandNew 개발팀 기술문서

## 1. 문서 목적
- 본 문서는 `DemandNew` 디맨드제어 엔진의 실제 구현(코드 기준)을 개발팀이 빠르게 이해하고 유지보수/확장할 수 있도록 정리한 기술문서다.
- 기준 소스: `AutobasePrime/LocalMain/DemandNew`, `AutobasePrime/Dll/AutoLibLocal/DemandNew`, `AutobasePrime/ViewMain/GraphicModule/*DemandNew*`

## 2. 아키텍처 요약
- 핵심 실행체:
  - `CheckEngineDemandNew` → 초기화/재로딩 진입점
  - `DemandNewEngineManager` → 블록별 엔진 로드/틱 처리
  - `DemandNewEngine` → 블록 1개에 대한 계측/예측/정책/제어/히스토리/스냅샷 발행
- 데이터 모델:
  - `DemandNewConfig`, `LoadModel`, `StepConfig`, `TimeZoneTarget`, `DrTarget`
  - `DemandSnapshot`, `LoadStatus`, `DemandSnapshotBus`
- 표시/연동:
  - 로컬: `DemandSnapshotBus` 구독 기반 차트 갱신
  - 원격: `DemandSnapshotSerializer` + DataGate 폴링

## 3. 실행 사이클(초당)
1. 계측 읽기: `TagMeasurementSource.Read()`
2. 단위/품질 처리: `MeasurementNormalizer.NormalizeToKW()`
3. 수요 윈도우 반영: `DemandWindow.AddSample()`
4. 예측 갱신: `DemandForecaster.AddSample()` + `Predict()`
5. 목표 계산: `ContractTargetProvider.GetCurrentTarget()`
6. 정책 판단: `PolicyEngine.Evaluate()`
7. 제어 실행: `TagActuator.ExecuteAsync()`
8. 이력 저장: `DemandHistorian` (interval/event/monthly peak)
9. 스냅샷 발행: `DemandSnapshotBus.Publish()`

## 4. 수요/예측/시간 정렬 구현 포인트
- `DemandWindow`
  - `ClockAligned=true`일 때 시작시각은 정시 슬롯으로 정렬됨.
  - `ElapsedSeconds`, `RemainingSeconds`, `IsIntervalComplete()`는 시각 기준으로 동작.
  - 과거 경과분을 0kW로 선채움하지 않음(초기 과소평가 방지).
- `DemandForecaster`
  - 기본 예측: 현재 kW가 남은 구간 유지된다고 가정.
  - 보정: `deltaKW` EWMA를 잔여시간에 대해 적분해 추가 보정.
  - 결과는 0 미만으로 내려가지 않도록 클램프.

## 5. 품질 처리 구현 포인트
- `TagMeasurementSource`
  - 우선순위:
    1. `CommStatusTagName`(DI) 설정 시 해당 DI로 품질 판정
    2. Memory/Indirect/SYSTEM 태그는 Good
    3. PLC/DDE/OPC는 `DeviceQuality` 매핑
- `MeasurementNormalizer`
  - `Bad`만 0 처리, `Stale`는 값 사용.
- `DemandNewEngine`
  - `Bad` 품질에서는 마지막 유효 kW(`_lastKW`)를 윈도우에 넣어 시간 진행 유지.
  - 정책은 `QualityBadBlockControl=true`이면 `Good`이 아닐 때 제어 차단.

## 6. 정책/제어 로직 요약
- 정책 조건:
  - `forecast > effectiveTarget + shedMargin` → Shed
  - `forecast < effectiveTarget - restoreMargin` → Restore
  - 그 외 Hold
- 보호 로직:
  - `ProtectionTimeSec`, `MinOffTimeSec`, `MinOnTimeSec`, `ReShedBlockTimeSec`, Interlock
- 다단계:
  - `StepConfig` 기준 우선선정 + 부족 시 priority fallback
- 제어 실행:
  - `TagActuator`는 DI/DO 타입만 허용
  - 피드백 태그가 있으면 타임아웃 내 검증

## 7. 이력/복구
- `DemandHistorian`
  - `demand_interval`, `demand_control_event`, `demand_monthly_peak` 저장
  - monthly peak의 `contract_kw`는 `config.ContractKW` 저장
- Failover
  - 저장 실패 시 `DemandBackupManager` JSONL 백업
  - 엔진 로드 시 백업 복구를 비동기로 시도

## 8. 차트 표시 구현 요약
- `FormDemandNewChart`
  - KPI(목표/예측/현재/경과/차단/모드), 목표선, 예측선, 스텝선, 월최대선 렌더링
  - `DemandCurveLength`만큼 실제 곡선 사용
- `ObjectDemandChart`
  - 로컬은 bus 구독, 원격은 DataGate 폴링으로 snapshot/config 반영

## 9. 현재 코드 기준 점검 체크리스트
- 설정 저장 후 엔진 재로드 시 블록 수/태그 바인딩 정상 여부
- `ClockAligned` 블록에서 구간 종료 시각이 정시 슬롯과 일치하는지
- 품질 Bad 연속 발생 시 `ElapsedSeconds`가 계속 진행되는지
- Failover 백업 파일 존재 시 재기동 후 자동 복구되는지
- 원격 차트에서 snapshot deserialize 실패 로그 유무

## 10. 확장 권장 포인트
- 정책 인터페이스 분리 유지(`IPolicyEngine`) 후 tariff/가격기반 정책 추가
- 예측기 대체(`DemandForecaster`) 시 동일 시그니처 유지
- Historian에 배치 insert 옵션 추가(블록 수 증가 대비)
- SnapshotBus subscriber 예외 로그에 블록ID/핸들러 타입 추가


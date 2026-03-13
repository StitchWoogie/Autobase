using System;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using NetTools;

namespace LocalMain.DemandNew
{
	public class DemandNewEngine
	{
		private readonly DemandNewConfig _config;
		private readonly IMeasurementSource _meter;
		private readonly MeasurementNormalizer _normalizer;
		private readonly DemandWindow _window;
		private readonly DemandForecaster _forecaster;
		private readonly ITargetProvider _targetProvider;
		private readonly IPolicyEngine _policyEngine;
		private readonly IActuator _actuator;
		private readonly LoadGroupManager _loadManager;
		private readonly DemandSnapshotBus _bus;

		private DemandHistorian _historian;

		private sbyte _oldSec = -1;
		private double _lastKW;
		private bool _isRunning;
		private bool _initialized;
		private DateTime _lastTickTime = DateTime.MinValue;

		// EOI 상태
		private sbyte _oldEoiValue;
		private int[] _eoiTagPos;

		// 월별 최대수요 추적
		private double _monthlyPeakKW;
		private DateTime _monthlyPeakTime;
		private string _currentMonth = "";

		public DemandNewEngine(DemandNewConfig config)
		{
			_config = config;

			// 계측
			if (config.MeterTagPos == null) config.MeterTagPos = new int[1];
			if (!string.IsNullOrEmpty(config.CommStatusTagName))
			{
				if (config.CommStatusTagPos == null) config.CommStatusTagPos = new int[1];
			}
			_meter = new TagMeasurementSource(config.MeterTagName, config.MeterTagPos,
				config.CommStatusTagName, config.CommStatusTagPos);
			_normalizer = new MeasurementNormalizer(config.MeterType, config.PulseRatio, config.InputAutoReset, config.ContractKW);

			// 수요 계산
			_window = new DemandWindow(config.IntervalMinutes, config.ClockAligned);

			// 예측
			_forecaster = new DemandForecaster(config.EwmaAlpha, config.TrendWindowSec);

			// 목표
			_targetProvider = new ContractTargetProvider(config);

			// 부하 관리
			_loadManager = new LoadGroupManager(config.Loads);
			_loadManager.InitializeTags();
			if (config.MultiStepEnabled && config.Steps != null)
				_loadManager.SetSteps(config.Steps);

			// 정책
			_policyEngine = new PolicyEngine(config, _loadManager);

			// 실행기
			_actuator = new TagActuator(_loadManager);

			// 스냅샷 버스
			_bus = new DemandSnapshotBus();

			// EOI
			if (config.UseEOI && !string.IsNullOrEmpty(config.EOITagName))
			{
				_eoiTagPos = config.EOITagPos ?? new int[1];
			}

			_initialized = true;
		}

		public string BlockId { get { return _config.BlockId; } }
		public DemandSnapshotBus Bus { get { return _bus; } }
		public DemandNewConfig Config { get { return _config; } }

		public void SetHistorian(DemandHistorian historian)
		{
			_historian = historian;
		}

		public async Task TickAsync()
		{
			if (!_initialized) return;
			if (_isRunning) return;  // 중복 실행 방지

			DateTime now = DateTime.Now;
			if (now.Second == _oldSec) return;  // 초당 1회
			_oldSec = (sbyte)now.Second;

			_isRunning = true;

			try
			{
				await ProcessTick(now);
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					String.Format("DemandNewEngine[{0}].TickAsync: {1}", _config.BlockId, ex.Message));
			}
			finally
			{
				_isRunning = false;
			}
		}

		private async Task ProcessTick(DateTime now)
		{
			// 실제 경과 시간 계산 (Windows Timer drift 보상)
			double deltaTimeSec = 1.0;
			if (_lastTickTime != DateTime.MinValue)
			{
				deltaTimeSec = (now - _lastTickTime).TotalSeconds;
				if (deltaTimeSec <= 0) deltaTimeSec = 1.0;
				if (deltaTimeSec > 30.0) deltaTimeSec = 1.0;  // 비정상 간격 방어 (30초 초과만)
			}
			_lastTickTime = now;

			// 1. 계측 읽기
			MeasurementReading reading = _meter.Read();

			// 2. kW 정규화 (실제 delta 사용)
			double kw = _normalizer.NormalizeToKW(reading, deltaTimeSec);
			double sampleKW = _lastKW;

			// 3. 품질 양호 시에만 윈도우/예측기 업데이트
			//    품질 불량(Quality == Bad) → NormalizeToKW가 0 반환
			//    이 0을 윈도우에 넣으면 평균수요를 끌어내리고,
			//    deltaKW = 0 - lastKW 로 급감 인식하여 예측 오류 발생
			if (reading.Quality != MeasurementQuality.Bad)
			{
				sampleKW = kw;
				double deltaKW = sampleKW - _lastKW;
				_lastKW = sampleKW;

				// 수요 윈도우 업데이트
				_window.AddSample(sampleKW, now);

				// 예측기 업데이트
				_forecaster.AddSample(deltaKW);
			}
			else
			{
				_window.AddSample(_lastKW, now);
			}

			// 4. 예측 (품질 불량 시에도 기존 데이터 기반으로 예측 계속)
			double forecastKW = _forecaster.Predict(
				_window.BlockEnergyKWH,
				_lastKW,
				_window.RemainingSeconds,
				_window.IntervalSeconds);

			// 5. 목표 산출
			TargetResult target = _targetProvider.GetCurrentTarget(now);

			// 6. 스냅샷 생성
			DemandSnapshot snapshot = BuildSnapshot(now, sampleKW, reading.Quality, forecastKW, target);

			// 7. 정책 평가
			PolicyResult policy = _policyEngine.Evaluate(snapshot, target);
			snapshot.LastDecision = policy.Decision;
			snapshot.CurrentStep = policy.Step;
			snapshot.PolicyReason = policy.Reason;

			// 8. 제어 실행 (Active 모드만)
			if (_config.Mode == EngineMode.Active && policy.Decision != ControlDecision.Hold)
			{
				foreach (var loadId in policy.LoadIdsToControl)
				{
					ActuationResult result = await _actuator.ExecuteAsync(loadId, policy.Decision);

					// 제어 이벤트 기록
					if (_historian != null)
					{
						_historian.OnControlEvent(BuildControlEvent(now, policy, loadId, result, forecastKW, target.EffectiveTargetKW));
					}

					// 알람 표시
					RaiseAlarm(policy, loadId, forecastKW, target.EffectiveTargetKW);
				}
			}

			// 9. 예측값 표시 태그 업데이트
			await WritePredictionDisplay(forecastKW);

			// 10. 월별 최대수요 갱신
			UpdateMonthlyPeak(now, _window.BlockDemandKW);

			// 11. EOI 체크
			CheckEOI();

			// 12. 구간 종료 체크
			if (_window.IsIntervalComplete(now))
			{
				snapshot.IsIntervalEnd = true;

				if (_historian != null)
					_historian.OnIntervalComplete(snapshot);

				_window.Reset(now);
				_forecaster.Reset();
				_normalizer.Reset();
			}

			// 13. 스냅샷 발행
			snapshot.MonthlyPeakKW = _monthlyPeakKW;
			snapshot.MonthlyPeakTime = _monthlyPeakTime;
			_bus.Publish(snapshot);
		}

		private DemandSnapshot BuildSnapshot(DateTime now, double currentKW,
			MeasurementQuality quality, double forecastKW, TargetResult target)
		{
			double[] curve = _window.GetCurveSnapshot();

			return new DemandSnapshot
			{
				BlockId = _config.BlockId,
				Timestamp = now,
				Mode = _config.Mode,

				CurrentKW = currentKW,
				Quality = quality,

				IntervalMinutes = _config.IntervalMinutes,
				ElapsedSeconds = _window.ElapsedSeconds,
				RemainingSeconds = _window.RemainingSeconds,
				IntervalStart = _window.IntervalStart,
				BlockDemandKW = _window.BlockDemandKW,
				BlockEnergyKWH = _window.BlockEnergyKWH,

				ForecastDemandEndKW = forecastKW,
				ForecastSlope = _forecaster.ForecastSlope,
				Confidence = _forecaster.Confidence,

				TargetKW = target.TargetKW,
				EffectiveTargetKW = target.EffectiveTargetKW,
				TargetReason = target.Reason,

				Loads = _loadManager.GetLoadStatuses(),

				DemandCurve = curve,
				DemandCurveLength = (curve != null) ? curve.Length : 0
			};
		}

		private ControlEventRecord BuildControlEvent(DateTime now, PolicyResult policy,
			string loadId, ActuationResult result, double forecastKW, double targetKW)
		{
			LoadModel load = _loadManager.GetLoad(loadId);
			return new ControlEventRecord
			{
				BlockId = _config.BlockId,
				EventTime = now,
				Decision = policy.Decision,
				Step = policy.Step,
				LoadId = loadId,
				LoadName = (load != null) ? load.DisplayName : "",
				EstimatedKW = (load != null) ? load.EstimatedKW : 0,
				ForecastKW = forecastKW,
				TargetKW = targetKW,
				Result = result,
				EngineMode = _config.Mode
			};
		}

		private void RaiseAlarm(PolicyResult policy, string loadId,
			double forecastKW, double targetKW)
		{
			if (string.IsNullOrEmpty(_config.MeterTagName)) return;

			try
			{
				TagAiClass ai = TagLib.GetStructAI(_config.MeterTagName, ref _config.MeterTagPos);

				string msg;
				bool isAlarm;

				if (policy.Decision == ControlDecision.Shed)
				{
					isAlarm = true;
					if (Tools.IsLangKorean())
						msg = String.Format("[신형DC] 예측({0:F0}kW) 초과 → 부하 차단: {1}", forecastKW, loadId);
					else
						msg = String.Format("[NewDC] Forecast({0:F0}kW) over target → Shed: {1}", forecastKW, loadId);
				}
				else
				{
					isAlarm = false;
					if (Tools.IsLangKorean())
						msg = String.Format("[신형DC] 예측({0:F0}kW) 여유 → 부하 복귀: {1}", forecastKW, loadId);
					else
						msg = String.Format("[NewDC] Forecast({0:F0}kW) below target → Restore: {1}", forecastKW, loadId);
				}

				AlarmDisplay.AlarmDisplayAI(ai, msg, EnumAlarmType.DEMAND_CONTROL,
					isAlarm, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
			catch { }
		}

		private async Task WritePredictionDisplay(double forecastKW)
		{
			if (string.IsNullOrEmpty(_config.PredictionDisplayTagName)) return;

			try
			{
				if (_config.PredictionDisplayTagPos == null)
					_config.PredictionDisplayTagPos = new int[1];

				TagAiClass ai = TagLib.GetStructAI(_config.PredictionDisplayTagName, ref _config.PredictionDisplayTagPos);
				await PlcScan.WriteAnalogInput(ai, forecastKW, false);
			}
			catch { }
		}

		private void UpdateMonthlyPeak(DateTime now, double currentDemandKW)
		{
			string month = now.ToString("yyyy-MM");
			if (month != _currentMonth)
			{
				_currentMonth = month;
				_monthlyPeakKW = 0;
				_monthlyPeakTime = now;
			}

			if (currentDemandKW > _monthlyPeakKW)
			{
				_monthlyPeakKW = currentDemandKW;
				_monthlyPeakTime = now;
			}
		}

		private void CheckEOI()
		{
			if (!_config.UseEOI || string.IsNullOrEmpty(_config.EOITagName)) return;

			try
			{
				TagDiClass di = TagLib.GetStructDI(_config.EOITagName, ref _eoiTagPos);

				if (di.curr != _oldEoiValue)
				{
					if (di.curr == 1)  // OFF→ON 전환
					{
						_window.Reset(DateTime.Now);
						_forecaster.Reset();
						_normalizer.Reset();
					}
					_oldEoiValue = di.curr;
				}
			}
			catch { }
		}
	}
}

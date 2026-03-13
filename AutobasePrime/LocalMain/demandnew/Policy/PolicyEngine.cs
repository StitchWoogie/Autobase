using System;
using System.Collections.Generic;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public class PolicyEngine : IPolicyEngine
	{
		private readonly DemandNewConfig _config;
		private readonly LoadGroupManager _loadManager;
		private DateTime _lastControlTime = DateTime.MinValue;

		public PolicyEngine(DemandNewConfig config, LoadGroupManager loadManager)
		{
			_config = config;
			_loadManager = loadManager;
		}

		public PolicyResult Evaluate(DemandSnapshot snapshot, TargetResult target)
		{
			var result = new PolicyResult();
			result.Decision = ControlDecision.Hold;
			result.Step = 0;
			result.LoadIdsToControl = new List<string>();

			// 품질 Bad 시 제어 금지
			if (_config.QualityBadBlockControl && snapshot.Quality != MeasurementQuality.Good)
			{
				result.Reason = "Quality Bad - control blocked";
				return result;
			}

			// ProtectionTime: 마지막 제어 후 보호 시간 내에는 추가 제어 금지
			if (_config.ProtectionTimeSec > 0 && _lastControlTime != DateTime.MinValue)
			{
				double elapsed = (snapshot.Timestamp - _lastControlTime).TotalSeconds;
				if (elapsed < _config.ProtectionTimeSec)
				{
					result.Reason = String.Format("Protection time active ({0:F0}s of {1}s)",
						elapsed, _config.ProtectionTimeSec);
					return result;
				}
			}

			double forecast = snapshot.ForecastDemandEndKW;
			double effective = target.EffectiveTargetKW;
			double shedMargin = _config.ShedMarginKW;
			double restoreMargin = _config.RestoreMarginKW;

			// 초과 위험 → Shed
			if (forecast > effective + shedMargin)
			{
				double excessKW = forecast - effective;

				// ForecastSlope 보호: slope이 음수(감소 추세)이면
				// 남은 시간 동안 자연 감소로 초과분을 해소할 수 있는지 확인
				if (snapshot.ForecastSlope < 0 && snapshot.RemainingSeconds > 0)
				{
					double expectedReduction = Math.Abs(snapshot.ForecastSlope) * snapshot.RemainingSeconds;
					if (expectedReduction >= excessKW)
					{
						result.Reason = String.Format(
							"Forecast {0:F1}kW exceeds target but declining (slope={1:F3}), expected natural reduction {2:F1}kW >= excess {3:F1}kW",
							forecast, snapshot.ForecastSlope, expectedReduction, excessKW);
						return result;
					}
				}

				int step = DetermineStep(excessKW);

				var loadsToShed = _loadManager.GetNextLoadsToShed(step, excessKW);

				if (loadsToShed.Count > 0)
				{
					result.Decision = ControlDecision.Shed;
					result.Step = step;
					result.LoadIdsToControl = loadsToShed;
					result.Reason = String.Format("Forecast {0:F1}kW exceeds target {1:F1}kW by {2:F1}kW (slope={3:F3})",
						forecast, effective, excessKW, snapshot.ForecastSlope);
				}
				else
				{
					result.Reason = "Shed needed but no eligible loads available";
				}
			}
			// 여유 → Restore
			else if (forecast < effective - restoreMargin)
			{
				var loadsToRestore = _loadManager.GetNextLoadsToRestore();

				if (loadsToRestore.Count > 0)
				{
					result.Decision = ControlDecision.Restore;
					result.Step = 0;
					result.LoadIdsToControl = loadsToRestore;
					result.Reason = String.Format("Forecast {0:F1}kW below target {1:F1}kW, restoring loads",
						forecast, effective);
				}
			}
			else
			{
				result.Reason = "Within margin - holding";
			}

			// 제어 결정이 발생하면 보호 타이머 갱신
			if (result.Decision != ControlDecision.Hold)
				_lastControlTime = snapshot.Timestamp;

			return result;
		}

		private int DetermineStep(double excessKW)
		{
			if (!_config.MultiStepEnabled || _config.StepCount <= 1)
				return 1;

			// 단계별 감축량 기준으로 필요한 단계 결정
			if (_config.Steps == null || _config.Steps.Count == 0)
				return 1;

			double accumulated = 0;
			for (int i = 0; i < _config.Steps.Count && i < _config.StepCount; i++)
			{
				accumulated += _config.Steps[i].ReductionKW;
				if (accumulated >= excessKW)
					return i + 1;
			}

			return _config.StepCount;  // 최대 단계
		}
	}
}

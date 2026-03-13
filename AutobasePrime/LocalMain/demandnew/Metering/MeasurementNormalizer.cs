using System;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public class MeasurementNormalizer
	{
		private readonly MeterType _meterType;
		private readonly double _pulseRatio;
		private readonly bool _inputAutoReset;
		private readonly double _contractKW;  // 절대값 스파이크 기준

		private double _previousRawValue;
		private double _previousKWH;
		private bool _hasPrevious;

		// 스파이크 감지용 이동평균
		private double _movingAvg;
		private int _avgCount;
		private const int SPIKE_FACTOR = 3;  // 이동평균 대비 3배 초과 시 스파이크

		public MeasurementNormalizer(MeterType meterType, double pulseRatio, bool inputAutoReset, double contractKW = 0)
		{
			_meterType = meterType;
			_pulseRatio = (pulseRatio <= 0) ? 1.0 : pulseRatio;
			_inputAutoReset = inputAutoReset;
			_contractKW = contractKW;
		}

		public double NormalizeToKW(MeasurementReading reading, double deltaTimeSec)
		{
			// Stale 값은 표시/추세 계산에는 활용하고, Bad만 무효로 처리한다.
			if (reading.Quality == MeasurementQuality.Bad)
				return 0;

			switch (_meterType)
			{
				case MeterType.DirectKW:
					return NormalizeDirectKW(reading.Value);

				case MeterType.DeltaKWH:
					return NormalizeDeltaKWH(reading.Value, deltaTimeSec);

				case MeterType.Pulse:
					return NormalizePulse(reading.Value, deltaTimeSec);

				default:
					return reading.Value;
			}
		}

		public void Reset()
		{
			_hasPrevious = false;
			_previousRawValue = 0;
			_previousKWH = 0;
			_movingAvg = 0;
			_avgCount = 0;
		}

		private double NormalizeDirectKW(double value)
		{
			double kw = value;
			kw = ClipSpike(kw);
			return Math.Max(0, kw);
		}

		private double NormalizeDeltaKWH(double currentKWH, double deltaTimeSec)
		{
			if (!_hasPrevious)
			{
				_previousKWH = currentKWH;
				_hasPrevious = true;
				return 0;
			}

			double deltaKWH = currentKWH - _previousKWH;
			_previousKWH = currentKWH;

			// 롤오버 감지
			if (deltaKWH < 0)
			{
				if (_inputAutoReset)
				{
					// 카운터 리셋으로 판단 → deltaKWH = currentKWH (0부터 재시작)
					deltaKWH = currentKWH;
				}
				else
				{
					deltaKWH = 0;  // 무시
				}
			}

			if (deltaTimeSec <= 0) return 0;

			// kWh → kW: (ΔkWh / Δt_hours)
			double hours = deltaTimeSec / 3600.0;
			double kw = deltaKWH / hours;

			kw = ClipSpike(kw);
			return Math.Max(0, kw);
		}

		private double NormalizePulse(double pulseCount, double deltaTimeSec)
		{
			if (!_hasPrevious)
			{
				_previousRawValue = pulseCount;
				_hasPrevious = true;
				return 0;
			}

			double deltaPulse = pulseCount - _previousRawValue;
			_previousRawValue = pulseCount;

			// 롤오버 감지
			if (deltaPulse < 0)
			{
				if (_inputAutoReset)
					deltaPulse = pulseCount;
				else
					deltaPulse = 0;
			}

			if (deltaTimeSec <= 0) return 0;

			// pulse → kWh → kW
			double deltaKWH = deltaPulse / _pulseRatio;
			double hours = deltaTimeSec / 3600.0;
			double kw = deltaKWH / hours;

			kw = ClipSpike(kw);
			return Math.Max(0, kw);
		}

		private double ClipSpike(double value)
		{
			// 1. 절대값 제한: ContractKW의 150% 초과는 비현실적 → 즉시 클램프
			if (_contractKW > 0 && value > _contractKW * 1.5)
			{
				return (_avgCount > 0) ? _movingAvg : 0;
			}

			if (_avgCount < 5)
			{
				// 초기 데이터 수집 단계
				_movingAvg = (_movingAvg * _avgCount + value) / (_avgCount + 1);
				_avgCount++;
				return value;
			}

			// 2. 편차 기반 스파이크 감지
			double deviation = Math.Abs(value - _movingAvg);
			bool isSpike = false;

			// 기준 A: 이동평균의 SPIKE_FACTOR(3)배 편차
			if (_movingAvg > 1.0 && deviation > _movingAvg * SPIKE_FACTOR)
				isSpike = true;

			// 기준 B: ContractKW의 30% 초과 편차 (절대값 기준)
			if (_contractKW > 0 && deviation > _contractKW * 0.3)
				isSpike = true;

			if (isSpike)
			{
				// 스파이크 감지 → 이전 이동평균으로 대체
				value = _movingAvg;
			}
			else
			{
				// 이동평균 업데이트 (지수평활)
				_movingAvg = 0.9 * _movingAvg + 0.1 * value;
			}

			return value;
		}
	}
}

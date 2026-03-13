using System;

namespace LocalMain.DemandNew
{
	public class DemandForecaster
	{
		private readonly double _alpha;
		private readonly int _trendWindowSec;
		private readonly double[] _trendRing;

		private int _trendPos;
		private int _trendCount;
		private double _ewmaValue;
		private bool _ewmaInitialized;

		// 이상치 감지용
		private double _sumSquared;
		private double _sum;

		public DemandForecaster(double ewmaAlpha, int trendWindowSec)
		{
			_alpha = Math.Max(0.05, Math.Min(0.5, ewmaAlpha));
			_trendWindowSec = Math.Max(30, Math.Min(300, trendWindowSec));
			_trendRing = new double[_trendWindowSec];
			Reset();
		}

		public double ForecastDemandEndKW { get; private set; }
		public double Confidence { get; private set; }

		/// <summary>
		/// 예측 기울기(EWMA kW 변화율). 양수=증가추세, 음수=감소추세.
		/// PolicyEngine에서 slope 음수일 때 불필요한 Shed 방지에 사용.
		/// </summary>
		public double ForecastSlope { get { return _ewmaValue; } }

		public void Reset()
		{
			_trendPos = 0;
			_trendCount = 0;
			_ewmaValue = 0;
			_ewmaInitialized = false;
			_sumSquared = 0;
			_sum = 0;
			ForecastDemandEndKW = 0;
			Confidence = 0;
			Array.Clear(_trendRing, 0, _trendRing.Length);
		}

		/// <summary>
		/// 초당 kW 변화량(delta)을 추가
		/// </summary>
		public void AddSample(double deltaKW)
		{
			// 이상치 클리핑 (2σ)
			double clipped = ClipOutlier(deltaKW);

			// EWMA 업데이트
			if (!_ewmaInitialized)
			{
				_ewmaValue = clipped;
				_ewmaInitialized = true;
			}
			else
			{
				_ewmaValue = _alpha * clipped + (1.0 - _alpha) * _ewmaValue;
			}

			// 트렌드 링버퍼에 삽입
			// 오래된 값 제거 (통계 업데이트)
			if (_trendCount >= _trendWindowSec)
			{
				double oldVal = _trendRing[_trendPos];
				_sum -= oldVal;
				_sumSquared -= oldVal * oldVal;
			}

			_trendRing[_trendPos] = clipped;
			_sum += clipped;
			_sumSquared += clipped * clipped;

			_trendPos = (_trendPos + 1) % _trendWindowSec;
			if (_trendCount < _trendWindowSec) _trendCount++;
		}

		/// <summary>
		/// 현재 블록 에너지(kWh), 현재 kW, 남은 초를 기반으로 구간 종료 시 수요(kW) 예측.
		/// 기본: 현재 kW가 남은 시간 동안 지속된다고 가정.
		/// 보정: deltaKW 추세(EWMA)에 의한 추가 에너지 (적분 = 삼각형 면적).
		/// </summary>
		public double Predict(double currentBlockEnergyKWH, double currentKW,
			int remainingSeconds, int intervalSeconds)
		{
			if (remainingSeconds <= 0 || intervalSeconds <= 0)
			{
				ForecastDemandEndKW = 0;
				Confidence = 0;
				return 0;
			}

			// 1. 기본 예측: 현재 kW가 남은 시간 동안 지속
			//    추가 에너지(kWh) = currentKW × remainingSeconds / 3600
			double baseAdditionalKWH = currentKW * remainingSeconds / 3600.0;

			// 2. 추세 보정: kW 변화율(EWMA)에 의한 추가 에너지
			//    kW(t) = currentKW + _ewmaValue × t 로 선형 근사
			//    추가 에너지 = ∫₀ᴿ _ewmaValue×t dt / 3600 = _ewmaValue × R² / (2×3600)
			double trendAdjustmentKWH = 0;
			if (_trendCount > 0)
			{
				trendAdjustmentKWH = _ewmaValue * (double)remainingSeconds * remainingSeconds / (2.0 * 3600.0);
			}

			// 3. 총 예상 에너지(kWh)
			double forecastTotalKWH = currentBlockEnergyKWH + baseAdditionalKWH + trendAdjustmentKWH;

			// 4. 수요전력(kW) = 총 에너지(kWh) / 구간 시간(h)
			double intervalHours = intervalSeconds / 3600.0;
			ForecastDemandEndKW = Math.Max(0, forecastTotalKWH / intervalHours);

			// 신뢰도 계산
			Confidence = CalculateConfidence();

			return ForecastDemandEndKW;
		}

		private double ClipOutlier(double value)
		{
			if (_trendCount < 10) return value;  // 초기 데이터 부족

			double mean = _sum / _trendCount;
			double variance = (_sumSquared / _trendCount) - (mean * mean);
			if (variance < 0) variance = 0;
			double stddev = Math.Sqrt(variance);

			if (stddev < 0.001) return value;  // 변동 없음

			double deviation = Math.Abs(value - mean);
			if (deviation > 2.0 * stddev)
			{
				// 2σ 초과 → 클램프
				return (value > mean) ? mean + 2.0 * stddev : mean - 2.0 * stddev;
			}

			return value;
		}

		private double CalculateConfidence()
		{
			if (_trendCount < 5) return 0;

			double mean = _sum / _trendCount;
			double variance = (_sumSquared / _trendCount) - (mean * mean);
			if (variance < 0) variance = 0;
			double stddev = Math.Sqrt(variance);

			double absEwma = Math.Abs(_ewmaValue);
			if (absEwma < 0.001) absEwma = 0.001;

			// 변동계수(CV)가 낮을수록 신뢰도 높음
			double cv = stddev / absEwma;
			double conf = 1.0 - Math.Min(1.0, cv);

			// 샘플 수 보정: 트렌드 윈도우가 절반 미만이면 감점
			double fillRatio = (double)_trendCount / _trendWindowSec;
			if (fillRatio < 0.5)
				conf *= fillRatio * 2.0;

			return Math.Max(0, Math.Min(1.0, conf));
		}
	}
}

using System;

namespace LocalMain.DemandNew
{
	public class DemandWindow
	{
		private readonly int _intervalMinutes;
		private readonly int _intervalSeconds;
		private readonly bool _clockAligned;
		private readonly double[] _kwSamples;

		private int _sampleCount;
		private double _sumKW;
		private DateTime _intervalStart;
		private DateTime _lastTimestamp;

		public DemandWindow(int intervalMinutes, bool clockAligned)
		{
			_intervalMinutes = Math.Max(5, Math.Min(60, intervalMinutes));
			_intervalSeconds = _intervalMinutes * 60;
			_clockAligned = clockAligned;
			_kwSamples = new double[_intervalSeconds];
			Reset(DateTime.Now);
		}

		public int IntervalMinutes { get { return _intervalMinutes; } }
		public int IntervalSeconds { get { return _intervalSeconds; } }
		public int ElapsedSeconds
		{
			get { return _clockAligned ? GetElapsedSecondsFromTime(_lastTimestamp) : _sampleCount; }
		}
		public int RemainingSeconds { get { return Math.Max(0, _intervalSeconds - ElapsedSeconds); } }
		public DateTime IntervalStart { get { return _intervalStart; } }

		/// <summary>
		/// 블록 수요(kW): 구간 내 평균 전력
		/// 수요전력 산정: 구간 내 총 에너지(kWh) / 구간 시간(h)
		/// </summary>
		public double BlockDemandKW
		{
			get
			{
				if (_sampleCount == 0) return 0;
				// 평균 kW = 구간 내 kW 합계 / 샘플 수 (1초 간격이므로 샘플 수 = 경과 초)
				return _sumKW / _sampleCount;
			}
		}

		/// <summary>
		/// 블록 누적 에너지(kWh): 구간 내 누적 에너지
		/// </summary>
		public double BlockEnergyKWH
		{
			get
			{
				// 총 kW 합계를 시간으로 변환
				return _sumKW / 3600.0;
			}
		}

		/// <summary>
		/// 슬라이딩 수요(kW): 최근 N초의 평균 전력
		/// N = min(intervalSeconds, sampleCount)
		/// </summary>
		public double SlidingDemandKW
		{
			get
			{
				if (_sampleCount == 0) return 0;
				int windowSize = Math.Min(_sampleCount, _intervalSeconds);
				double sum = 0;
				int start = _sampleCount - windowSize;
				for (int i = start; i < _sampleCount; i++)
				{
					sum += _kwSamples[i];
				}
				return sum / windowSize;
			}
		}

		/// <summary>
		/// 현재 시점 kW (마지막 샘플)
		/// </summary>
		public double CurrentKW
		{
			get
			{
				if (_sampleCount == 0) return 0;
				return _kwSamples[_sampleCount - 1];
			}
		}

		public void AddSample(double kw, DateTime timestamp)
		{
			_lastTimestamp = timestamp;

			if (_sampleCount >= _intervalSeconds)
				return;  // 구간 초과 방지

			_kwSamples[_sampleCount] = kw;
			_sumKW += kw;
			_sampleCount++;
		}

		public bool IsIntervalComplete(DateTime now)
		{
			if (_clockAligned)
				return now >= _intervalStart.AddSeconds(_intervalSeconds);

			return _sampleCount >= _intervalSeconds;
		}

		public void Reset(DateTime newStart)
		{
			if (_clockAligned)
			{
				// 시계 기준 정렬: 예) 15분 간격이면 :00, :15, :30, :45
				int alignedMinute = (newStart.Minute / _intervalMinutes) * _intervalMinutes;
				_intervalStart = new DateTime(
					newStart.Year, newStart.Month, newStart.Day,
					newStart.Hour, alignedMinute, 0);
			}
			else
			{
				_intervalStart = newStart;
			}

			_sampleCount = 0;
			_sumKW = 0;
			Array.Clear(_kwSamples, 0, _kwSamples.Length);
			_lastTimestamp = newStart;
			// ClockAligned라도 경과구간을 0kW로 선채움하지 않는다.
			// (선채움 시 평균수요/예측/차트가 체계적으로 과소평가됨)
		}

		/// <summary>
		/// 차트 렌더링용 수요 곡선 배열의 독립 복사본을 반환한다.
		/// 주의: 기존 A/B 더블버퍼링 방식과 달리 3번째 엔진에서 1번째 엔진의
		/// 배열을 공유하므로, 비동기 누적과 차트 렌더러가 이전 엔진을
		/// 보정하면 데이터 경합 문제가 발생하였음.
		/// 매 인터벌마다 새 배열을 할당하여 각 엔진이 독립적 데이터를 보유.
		/// (15분 구간 기준 최대 900 doubles = ~7KB/초, GC 부담 무시 가능)
		/// </summary>
		public double[] GetCurveSnapshot()
		{
			if (_sampleCount <= 0) return new double[0];

			double[] copy = new double[_sampleCount];
			Array.Copy(_kwSamples, copy, _sampleCount);
			return copy;
		}

		private int GetElapsedSecondsFromTime(DateTime time)
		{
			if (time <= _intervalStart) return 0;
			int elapsed = (int)(time - _intervalStart).TotalSeconds;
			if (elapsed < 0) return 0;
			if (elapsed > _intervalSeconds) return _intervalSeconds;
			return elapsed;
		}
	}
}

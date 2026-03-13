using System;
using System.Collections.Generic;

namespace AutoLibLocal.DemandNew
{
	public class LoadStatus
	{
		public string LoadId;
		public string DisplayName;
		public int Priority;
		public double EstimatedKW;
		public bool IsShed;
		public DateTime LastShedTime;
		public DateTime LastRestoreTime;
	}

	public class DemandSnapshot
	{
		// 식별
		public string BlockId;
		public DateTime Timestamp;
		public EngineMode Mode;

		// 계측
		public double CurrentKW;
		public MeasurementQuality Quality;

		// 수요 윈도우
		public int IntervalMinutes;
		public int ElapsedSeconds;
		public int RemainingSeconds;
		public DateTime IntervalStart;
		public double BlockDemandKW;
		public double BlockEnergyKWH;

		// 예측
		public double ForecastDemandEndKW;
		public double ForecastSlope;   // EWMA kW 변화율 (양수=증가, 음수=감소)
		public double Confidence;

		// 목표
		public double TargetKW;
		public double EffectiveTargetKW;
		public TargetReason TargetReason;

		// 정책
		public ControlDecision LastDecision;
		public int CurrentStep;
		public string PolicyReason;

		// 부하 상태
		public List<LoadStatus> Loads;

		// 구간 종료 플래그
		public bool IsIntervalEnd;

		// 차트 데이터 (초별 kW 배열)
		public double[] DemandCurve;
		public int DemandCurveLength;

		// 월 최대수요
		public double MonthlyPeakKW;
		public DateTime MonthlyPeakTime;

		// 마진 (target - forecast, 양수 = 안전)
		public double MarginKW
		{
			get { return EffectiveTargetKW - ForecastDemandEndKW; }
		}
	}
}

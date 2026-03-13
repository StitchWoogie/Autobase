using System;
using System.Collections.Generic;

namespace AutoLibLocal.DemandNew
{
	[Serializable]
	public class TimeZoneTarget
	{
		public TariffZone Zone;
		public TimeSpan StartTime;
		public TimeSpan EndTime;
		public double TargetKW;

		public bool IsActiveAt(DateTime now)
		{
			TimeSpan tod = now.TimeOfDay;
			if (StartTime <= EndTime)
				return tod >= StartTime && tod < EndTime;
			// 자정 넘김 (예: 22:00~06:00)
			return tod >= StartTime || tod < EndTime;
		}
	}

	[Serializable]
	public class DrTarget
	{
		public bool IsEnabled;
		public double TargetKW;
		public DateTime StartTime;
		public DateTime EndTime;

		public bool IsActiveAt(DateTime now)
		{
			return IsEnabled && now >= StartTime && now < EndTime;
		}
	}

	[Serializable]
	public class StepConfig
	{
		public double ReductionKW;
		public List<string> LoadIds = new List<string>();
	}

	[Serializable]
	public class LoadModel
	{
		public string LoadId = "";
		public string DisplayName = "";
		public int Priority = 1;
		public string Group = "";
		public double EstimatedKW;
		public string CommandTagName = "";
		public sbyte CommandTagType;       // EnumTagType 값
		public string FeedbackTagName = "";
		public int MinOffTimeSec = 300;
		public int MinOnTimeSec = 300;
		public bool ReShedBlockEnabled;
		public int ReShedBlockTimeSec = 600;
		public string InterlockTagName = "";

		// 런타임 상태 (비직렬화)
		[NonSerialized] public DateTime LastShedTime;
		[NonSerialized] public DateTime LastRestoreTime;
		[NonSerialized] public bool IsCurrentlyShed;
		[NonSerialized] public int[] CommandTagPos;
		[NonSerialized] public int[] FeedbackTagPos;
		[NonSerialized] public int[] InterlockTagPos;
	}

	[Serializable]
	public class DemandNewConfig
	{
		// 일반
		public string BlockId = "";
		public string Title = "";
		public EngineMode Mode = EngineMode.Shadow;
		public int IntervalMinutes = 15;
		public bool ClockAligned = true;

		// 계약/요금
		public double ContractKW = 1000;
		public double SafetyFactor = 0.95;
		public List<TimeZoneTarget> TimeZoneTargets = new List<TimeZoneTarget>();
		public DrTarget DrTarget = new DrTarget();

		// 계측
		public string MeterTagName = "";
		public MeterType MeterType = MeterType.DirectKW;
		public double PulseRatio = 1.0;
		public string TargetTagName = "";      // 선택: AI 태그에서 목표값 읽기 (레거시 호환)
		public bool UseTargetTag;
		public bool UseEOI;
		public string EOITagName = "";
		public bool InputAutoReset;            // 펄스 리셋 감지
		public string PredictionDisplayTagName = "";  // 예측값 표시 AI 태그
		public string CommStatusTagName = "";          // DI 통신상태 태그 (0=통신이상, 1=정상)

		// 부하 (다중)
		public List<LoadModel> Loads = new List<LoadModel>();

		// 정책
		public double ShedMarginKW;
		public double RestoreMarginKW;
		public int ProtectionTimeSec = 60;
		public bool MultiStepEnabled;
		public int StepCount = 1;
		public List<StepConfig> Steps = new List<StepConfig>();
		public bool QualityBadBlockControl = true;  // Bad 품질 시 제어 금지

		// 예측
		public int TrendWindowSec = 120;
		public double EwmaAlpha = 0.3;

		// 저장
		public bool DatabaseSaveEnabled;
		public string DatabaseDsn = "";
		public bool UsePostgres;
		public bool FailoverEnabled = true;

		// 표시
		public int PercentY = 120;

		// 런타임 전용 (비직렬화)
		[NonSerialized] public int[] MeterTagPos;
		[NonSerialized] public int[] TargetTagPos;
		[NonSerialized] public int[] EOITagPos;
		[NonSerialized] public int[] PredictionDisplayTagPos;
		[NonSerialized] public int[] CommStatusTagPos;
		[NonSerialized] public bool Initialized;
		[NonSerialized] public string ErrorMessage;

		public int IntervalSeconds
		{
			get { return IntervalMinutes * 60; }
		}
	}
}

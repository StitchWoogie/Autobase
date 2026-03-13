using System;

namespace AutoLibLocal.DemandNew
{
	public enum EngineMode
	{
		Shadow = 0,
		Active = 1
	}

	public enum MeterType
	{
		DirectKW = 0,
		DeltaKWH = 1,
		Pulse = 2
	}

    /// <summary>
    /// 품질	조건	의미
    /// Bad		태그 미존재, 미해석, 예외 발생		값 자체가 신뢰 불가
    /// Good	태그 읽기 성공 + 통신 정상		값이 유효하고 최신
    /// Stale	태그 읽기 성공 + 통신 끊김		값은 읽히지만 갱신이 멈춘 상태
    /// </summary>
    public enum MeasurementQuality
	{
		Good = 0,
		Bad = 1,
		Stale = 2
	}

	public enum ControlDecision
	{
		Hold = 0,
		Shed = 1,
		Restore = 2
	}

	public enum TargetReason
	{
		Contract = 0,
		TimeZone = 1,
		DemandResponse = 2,
		Manual = 3
	}

	public enum ActuationResult
	{
		Success = 0,
		Timeout = 1,
		Failure = 2,
		Skipped = 3
	}

	public enum TariffZone
	{
		OffPeak = 0,
		MidPeak = 1,
		OnPeak = 2
	}
}

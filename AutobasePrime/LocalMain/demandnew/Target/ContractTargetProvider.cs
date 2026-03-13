using System;
using AutoLib;
using AutoLibLocal;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public class ContractTargetProvider : ITargetProvider
	{
		private readonly DemandNewConfig _config;

		public ContractTargetProvider(DemandNewConfig config)
		{
			_config = config;
		}

		public TargetResult GetCurrentTarget(DateTime now)
		{
			double baseKW = _config.ContractKW;
			double safety = _config.SafetyFactor;
			TargetReason reason = TargetReason.Contract;

			// AI 태그에서 목표값 읽기 (레거시 호환)
			if (_config.UseTargetTag && !string.IsNullOrEmpty(_config.TargetTagName))
			{
				try
				{
					TagAiClass ai = TagLib.GetStructAI(_config.TargetTagName, ref _config.TargetTagPos);
					if (ai.curr > 0)
					{
						baseKW = ai.curr;
						reason = TargetReason.Manual;
					}
				}
				catch { }
			}

			// 시간대별 목표 확인 (더 낮은 값이면 적용)
			foreach (var tz in _config.TimeZoneTargets)
			{
				if (tz.IsActiveAt(now) && tz.TargetKW > 0)
				{
					if (tz.TargetKW < baseKW)
					{
						baseKW = tz.TargetKW;
						reason = TargetReason.TimeZone;
					}
				}
			}

			// DR 이벤트 확인 (최우선)
			if (_config.DrTarget != null && _config.DrTarget.IsActiveAt(now))
			{
				if (_config.DrTarget.TargetKW > 0 && _config.DrTarget.TargetKW < baseKW)
				{
					baseKW = _config.DrTarget.TargetKW;
					reason = TargetReason.DemandResponse;
				}
			}

			// 안전계수 적용
			double effective = baseKW * safety;

			// 목표값이 0 이하이면 최소값 설정 (레거시: 10)
			if (effective <= 0) effective = 10;

			return new TargetResult
			{
				TargetKW = baseKW,
				SafetyFactor = safety,
				EffectiveTargetKW = effective,
				Reason = reason
			};
		}
	}
}

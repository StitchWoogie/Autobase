using System;
using System.Collections.Generic;
using AutoLib;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using NetTools;

namespace LocalMain.DemandNew
{
	public static class DemandNewConfigValidator
	{
		/// <summary>
		/// 설정 유효성 검증. 오류가 있으면 오류 메시지 목록 반환.
		/// </summary>
		public static List<string> Validate(DemandNewConfig config)
		{
			var errors = new List<string>();

			if (string.IsNullOrEmpty(config.BlockId))
				errors.Add("BlockId is required");

			if (string.IsNullOrEmpty(config.MeterTagName))
				errors.Add("MeterTagName is required");

			if (config.IntervalMinutes < 5 || config.IntervalMinutes > 60)
				errors.Add("IntervalMinutes must be 5~60");

			if (config.SafetyFactor < 0.5 || config.SafetyFactor > 1.0)
				errors.Add("SafetyFactor must be 0.50~1.00");

			if (config.ContractKW <= 0)
				errors.Add("ContractKW must be > 0");

			if (config.EwmaAlpha < 0.05 || config.EwmaAlpha > 0.5)
				errors.Add("EwmaAlpha must be 0.05~0.50");

			if (config.TrendWindowSec < 30 || config.TrendWindowSec > 300)
				errors.Add("TrendWindowSec must be 30~300");

			if (config.Mode == EngineMode.Active && config.Loads.Count == 0)
				errors.Add("Active mode requires at least one load");

			// 부하별 검증
			foreach (var load in config.Loads)
			{
				if (string.IsNullOrEmpty(load.LoadId))
					errors.Add("Load must have a LoadId");
				if (string.IsNullOrEmpty(load.CommandTagName))
					errors.Add(String.Format("Load '{0}' must have a CommandTagName", load.LoadId));
			}

			return errors;
		}

		/// <summary>
		/// 레거시 엔진과의 DO 태그 충돌 진단
		/// </summary>
		public static void ValidateConflicts(List<DemandNewEngine> engines)
		{
			// 1. 신형 엔진 간 DO 충돌 체크
			var usedTags = new Dictionary<string, string>();  // tagName -> blockId

			foreach (var engine in engines)
			{
				foreach (var load in engine.Config.Loads)
				{
					if (engine.Config.Mode == EngineMode.Shadow) continue;
                    if (string.IsNullOrEmpty(load.CommandTagName)) continue;

                    string tag = load.CommandTagName.ToLower();
					if (usedTags.ContainsKey(tag))
					{
						string msg;
						if (Tools.IsLangKorean())
							msg = String.Format("경고: DO 태그 '{0}'가 블록 '{1}'과 '{2}'에서 중복 사용됩니다.",
								load.CommandTagName, usedTags[tag], engine.BlockId);
						else
							msg = String.Format("Warning: DO tag '{0}' is used in both block '{1}' and '{2}'.",
								load.CommandTagName, usedTags[tag], engine.BlockId);

						SmLog.Message(LogLevel.WARNING, LogCategory.DATA_SAVE, msg);
						MessageDisplay.Show(msg);
					}
					else
					{
						usedTags[tag] = engine.BlockId;
					}
				}
			}

			// 2. 레거시 엔진과의 충돌 체크
			if (DemandControl.blockDemandControl == null) return;

			for (int i = 0; i < DemandControl.blockDemandControl.Count; i++)
			{
				var legacy = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[i];
				string legacyTag = "";

				if (legacy.tagOut.tag != null)
					legacyTag = legacy.tagOut.tag.ToLower();

				if (string.IsNullOrEmpty(legacyTag)) continue;

				if (usedTags.ContainsKey(legacyTag))
				{
					string msg;
					if (Tools.IsLangKorean())
						msg = String.Format("경고: 레거시 Demand Control '{0}'의 출력 태그 '{1}'가 신형 블록 '{2}'에서도 사용됩니다.",
							legacy.title, legacy.tagOut.tag, usedTags[legacyTag]);
					else
						msg = String.Format("Warning: Legacy DemandControl '{0}' output tag '{1}' conflicts with new block '{2}'.",
							legacy.title, legacy.tagOut.tag, usedTags[legacyTag]);

					SmLog.Message(LogLevel.WARNING, LogCategory.DATA_SAVE, msg);
					MessageDisplay.Show(msg);
				}
			}
		}
	}
}

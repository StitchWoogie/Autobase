using System;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public class TagActuator : IActuator
	{
		private readonly LoadGroupManager _loadManager;
		private readonly int _verifyTimeoutMs;
		private readonly int _retryCount;

		public TagActuator(LoadGroupManager loadManager, int verifyTimeoutMs = 5000, int retryCount = 1)
		{
			_loadManager = loadManager;
			_verifyTimeoutMs = verifyTimeoutMs;
			_retryCount = retryCount;
		}

		public async Task<ActuationResult> ExecuteAsync(string loadId, ControlDecision decision)
		{
			if (decision == ControlDecision.Hold)
				return ActuationResult.Skipped;

			LoadModel load = _loadManager.GetLoad(loadId);
			if (load == null) return ActuationResult.Failure;
			if (string.IsNullOrEmpty(load.CommandTagName)) return ActuationResult.Failure;

			// Shed → 차단(출력 OFF=0 또는 ON=1, 사이트마다 다름)
			// 레거시 패턴: Shed 시 echo==1이면 이미 차단, 차단 = tagOut OFF(0)
			// 여기서는 Shed=0(차단), Restore=1(복귀) 로 통일
			sbyte value = (decision == ControlDecision.Shed) ? (sbyte)0 : (sbyte)1;

			for (int attempt = 0; attempt <= _retryCount; attempt++)
			{
				try
				{
					bool written = await WriteDigitalOut(load, value);
					if (!written) return ActuationResult.Failure;  // DI/DO 외 태그 → 즉시 실패
				}
				catch (Exception)
				{
					if (attempt >= _retryCount) return ActuationResult.Failure;
					continue;
				}

				// 피드백 검증
				if (!string.IsNullOrEmpty(load.FeedbackTagName))
				{
					bool verified = await WaitForFeedback(load, decision);
					if (verified)
					{
						UpdateLoadState(load, decision);
						return ActuationResult.Success;
					}
					if (attempt >= _retryCount) return ActuationResult.Timeout;
				}
				else
				{
					// 피드백 태그 없으면 성공으로 간주
					UpdateLoadState(load, decision);
					return ActuationResult.Success;
				}
			}

			return ActuationResult.Failure;
		}

		public bool VerifyFeedback(string loadId, ControlDecision expectedState)
		{
			LoadModel load = _loadManager.GetLoad(loadId);
			if (load == null || string.IsNullOrEmpty(load.FeedbackTagName))
				return true;  // 피드백 없으면 항상 OK

			try
			{
				TagDiClass di = TagLib.GetStructDI(load.FeedbackTagName, ref load.FeedbackTagPos);
				// Shed 상태: feedback == 1 (차단 확인)
				// Restore 상태: feedback == 0
				sbyte expected = (expectedState == ControlDecision.Shed) ? (sbyte)1 : (sbyte)0;
				return (di.curr == expected);
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 디지털 출력 쓰기. DI/DO 이외 타입은 실패(false) 반환.
		/// </summary>
		private async Task<bool> WriteDigitalOut(LoadModel load, sbyte value)
		{
			EnumTagType tagType = (EnumTagType)load.CommandTagType;

			if (tagType == EnumTagType.DI)
			{
				TagDiClass di = TagLib.GetStructDI(load.CommandTagName, ref load.CommandTagPos);
				await PlcScan.CommWriteDigitalInputDelaySec(di, value, 0, false);
				return true;
			}
			else if (tagType == EnumTagType.DO)
			{
				TagDoClass dout = TagLib.GetStructDO(load.CommandTagName, ref load.CommandTagPos);
				PlcScan.CommWriteDigitalOutputDelaySec(dout, value, 0, false);
				return true;
			}

			// DI/DO 이외 태그 타입은 제어 불가
			return false;
		}

		private async Task<bool> WaitForFeedback(LoadModel load, ControlDecision expectedState)
		{
			int elapsed = 0;
			int checkInterval = 500;  // 500ms마다 확인

			while (elapsed < _verifyTimeoutMs)
			{
				await Task.Delay(checkInterval);
				elapsed += checkInterval;

				if (VerifyFeedback(load.LoadId, expectedState))
					return true;
			}

			return false;
		}

		private void UpdateLoadState(LoadModel load, ControlDecision decision)
		{
			if (decision == ControlDecision.Shed)
				_loadManager.MarkShed(load.LoadId);
			else if (decision == ControlDecision.Restore)
				_loadManager.MarkRestored(load.LoadId);
		}
	}
}

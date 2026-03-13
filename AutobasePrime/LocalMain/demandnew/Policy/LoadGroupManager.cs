using System;
using System.Collections.Generic;
using AutoLib;
using AutoLibLocal;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public class LoadGroupManager
	{
		private readonly List<LoadModel> _loads;
		private readonly List<LoadModel> _sortedByPriority;
		private List<StepConfig> _steps;

		public LoadGroupManager(List<LoadModel> loads)
		{
			_loads = loads ?? new List<LoadModel>();
			_sortedByPriority = new List<LoadModel>(_loads);
			_sortedByPriority.Sort((a, b) => a.Priority.CompareTo(b.Priority));
		}

		/// <summary>
		/// 다단계 제어 설정 바인딩 (DemandNewEngine 생성 시 호출)
		/// </summary>
		public void SetSteps(List<StepConfig> steps)
		{
			_steps = steps;
		}

		public LoadModel GetLoad(string loadId)
		{
			for (int i = 0; i < _loads.Count; i++)
			{
				if (_loads[i].LoadId == loadId)
					return _loads[i];
			}
			return null;
		}

		public List<LoadModel> AllLoads { get { return _loads; } }

		/// <summary>
		/// 다음에 차단할 부하 목록을 반환
		/// 다단계 모드: StepConfig에 지정된 부하만 해당 단계까지 선별
		/// 단일 단계: 우선순위 순으로 필요 감축량까지 선별
		/// </summary>
		public List<string> GetNextLoadsToShed(int step, double neededReductionKW)
		{
			var result = new List<string>();
			double accumulated = 0;
			DateTime now = DateTime.Now;

			// 다단계 모드: step 설정에 등록된 부하 우선 선별
			if (_steps != null && _steps.Count > 0 && step > 0)
			{
				// step 1 ~ step N까지 누적 선별
				for (int s = 0; s < step && s < _steps.Count; s++)
				{
					foreach (string loadId in _steps[s].LoadIds)
					{
						LoadModel load = GetLoad(loadId);
						if (load == null) continue;
						if (load.IsCurrentlyShed) continue;
						if (!CanShed(load, now)) continue;
						if (IsInterlocked(load)) continue;
						if (result.Contains(loadId)) continue;

						result.Add(loadId);
						accumulated += load.EstimatedKW;
					}
				}

				// step 내 부하로 충분하면 반환
				if (accumulated >= neededReductionKW)
					return result;
			}

			// 부족분은 우선순위 기반으로 추가 선별
			foreach (var load in _sortedByPriority)
			{
				if (load.IsCurrentlyShed) continue;
				if (!CanShed(load, now)) continue;
				if (IsInterlocked(load)) continue;
				if (result.Contains(load.LoadId)) continue;

				result.Add(load.LoadId);
				accumulated += load.EstimatedKW;

				if (accumulated >= neededReductionKW)
					break;
			}

			return result;
		}

		/// <summary>
		/// 복귀할 부하 목록을 반환 (역순: 낮은 우선순위부터 복귀)
		/// </summary>
		public List<string> GetNextLoadsToRestore()
		{
			var result = new List<string>();
			DateTime now = DateTime.Now;

			// 역순: 우선순위가 낮은(숫자가 큰) 부하부터 복귀
			for (int i = _sortedByPriority.Count - 1; i >= 0; i--)
			{
				var load = _sortedByPriority[i];
				if (!load.IsCurrentlyShed) continue;
				if (!CanRestore(load, now)) continue;

				result.Add(load.LoadId);
			}

			return result;
		}

		/// <summary>
		/// 부하 상태 업데이트 (차단)
		/// </summary>
		public void MarkShed(string loadId)
		{
			var load = GetLoad(loadId);
			if (load == null) return;
			load.IsCurrentlyShed = true;
			load.LastShedTime = DateTime.Now;
		}

		/// <summary>
		/// 부하 상태 업데이트 (복귀)
		/// </summary>
		public void MarkRestored(string loadId)
		{
			var load = GetLoad(loadId);
			if (load == null) return;
			load.IsCurrentlyShed = false;
			load.LastRestoreTime = DateTime.Now;
		}

		/// <summary>
		/// 현재 차단 중인 부하 수
		/// </summary>
		public int ShedCount
		{
			get
			{
				int count = 0;
				foreach (var load in _loads)
				{
					if (load.IsCurrentlyShed) count++;
				}
				return count;
			}
		}

		/// <summary>
		/// 현재 차단 중인 부하의 총 추정 kW
		/// </summary>
		public double TotalShedKW
		{
			get
			{
				double total = 0;
				foreach (var load in _loads)
				{
					if (load.IsCurrentlyShed) total += load.EstimatedKW;
				}
				return total;
			}
		}

		/// <summary>
		/// 부하 상태 목록 (스냅샷용)
		/// </summary>
		public List<LoadStatus> GetLoadStatuses()
		{
			var statuses = new List<LoadStatus>();
			foreach (var load in _loads)
			{
				statuses.Add(new LoadStatus
				{
					LoadId = load.LoadId,
					DisplayName = load.DisplayName,
					Priority = load.Priority,
					EstimatedKW = load.EstimatedKW,
					IsShed = load.IsCurrentlyShed,
					LastShedTime = load.LastShedTime,
					LastRestoreTime = load.LastRestoreTime
				});
			}
			return statuses;
		}

		public void InitializeTags()
		{
			foreach (var load in _loads)
			{
				if (!string.IsNullOrEmpty(load.CommandTagName))
				{
					if (load.CommandTagPos == null)
						load.CommandTagPos = new int[1];
				}
				if (!string.IsNullOrEmpty(load.FeedbackTagName))
				{
					if (load.FeedbackTagPos == null)
						load.FeedbackTagPos = new int[1];
				}
				if (!string.IsNullOrEmpty(load.InterlockTagName))
				{
					if (load.InterlockTagPos == null)
						load.InterlockTagPos = new int[1];
				}
			}
		}

		private bool CanShed(LoadModel load, DateTime now)
		{
			// MinOnTime 체크: 복귀 후 최소 유지 시간 경과 여부
			if (load.LastRestoreTime != DateTime.MinValue)
			{
				double elapsed = (now - load.LastRestoreTime).TotalSeconds;
				if (elapsed < load.MinOnTimeSec) return false;
			}

			// ReShedBlock 체크: 최근 복귀 후 재차단 금지
			if (load.ReShedBlockEnabled && load.LastRestoreTime != DateTime.MinValue)
			{
				double elapsed = (now - load.LastRestoreTime).TotalSeconds;
				if (elapsed < load.ReShedBlockTimeSec) return false;
			}

			return true;
		}

		private bool CanRestore(LoadModel load, DateTime now)
		{
			// MinOffTime 체크: 차단 후 최소 유지 시간 경과 여부
			if (load.LastShedTime != DateTime.MinValue)
			{
				double elapsed = (now - load.LastShedTime).TotalSeconds;
				if (elapsed < load.MinOffTimeSec) return false;
			}

			return true;
		}

		private bool IsInterlocked(LoadModel load)
		{
			if (string.IsNullOrEmpty(load.InterlockTagName)) return false;

			try
			{
				TagDiClass di = TagLib.GetStructDI(load.InterlockTagName, ref load.InterlockTagPos);
				return (di.curr == 1);  // 인터락 ON이면 차단 불가
			}
			catch
			{
				return false;
			}
		}
	}
}

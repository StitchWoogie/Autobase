using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoLibLocal.DemandNew;
using NetTools;
using AutoLibLocal;

namespace LocalMain.DemandNew
{
	public class DemandNewEngineManager
	{
		private readonly List<DemandNewEngine> _engines = new List<DemandNewEngine>();

		public int EngineCount { get { return _engines.Count; } }

		public void Load()
		{
			_engines.Clear();

			List<DemandNewConfig> configs = DemandNewConfigLoader.LoadAll();

			foreach (var config in configs)
			{
				try
				{
					var engine = new DemandNewEngine(config);

					// Historian 연결
					if (config.DatabaseSaveEnabled || config.FailoverEnabled)
					{
						var historian = new DemandHistorian(config);
						engine.SetHistorian(historian);
						if (config.FailoverEnabled && config.DatabaseSaveEnabled)
						{
							Task.Run(() =>
							{
								try { historian.RecoverFromBackups(); }
								catch (Exception ex)
								{
									SmLog.LogError(LogCategory.DATA_SAVE,
										String.Format("DemandNewEngineManager.RecoverFromBackups[{0}]", config.BlockId), ex.Message);
								}
							});
						}
					}

					_engines.Add(engine);
				}
				catch (Exception ex)
				{
					SmLog.LogError(LogCategory.DATA_SAVE,
						String.Format("DemandNewEngineManager.Load[{0}]", config.BlockId), ex.Message);
				}
			}

			// DO 충돌 진단
			DemandNewConfigValidator.ValidateConflicts(_engines);
		}

		/// <summary>
		/// 매 tick마다 모든 엔진을 처리한다.
		/// 주의: round-robin 패턴을 제거함.
		///   - round-robin 시 각 엔진이 N초 간격으로 실행 → deltaTimeSec 클램프(>5s→1s)에 걸려
		///     DeltaKWH/Pulse 환산에서 수배 오차 발생.
		///   - DemandWindow가 1초 간격 샘플을 전제로 설계되어
		///     ElapsedSeconds/RemainingSeconds 계산이 부정확해짐.
		///   - 각 엔진의 TickAsync는 태그 1회 읽기 + 연산으로 경량이므로
		///     수십 블록도 전체 순차 처리에 무리 없음.
		/// </summary>
		public async Task TickAsync()
		{
			if (_engines.Count == 0) return;

			for (int i = 0; i < _engines.Count; i++)
			{
				await _engines[i].TickAsync();
			}
		}

		public DemandNewEngine GetEngine(int index)
		{
			if (index < 0 || index >= _engines.Count) return null;
			return _engines[index];
		}

		public DemandNewEngine GetEngine(string blockId)
		{
			for (int i = 0; i < _engines.Count; i++)
			{
				if (_engines[i].BlockId == blockId)
					return _engines[i];
			}
			return null;
		}

		public DemandSnapshotBus GetBus(string blockId)
		{
			var engine = GetEngine(blockId);
			return engine?.Bus;
		}

		public List<string> GetBlockIds()
		{
			var ids = new List<string>();
			foreach (var engine in _engines)
			{
				ids.Add(engine.BlockId);
			}
			return ids;
		}
	}
}

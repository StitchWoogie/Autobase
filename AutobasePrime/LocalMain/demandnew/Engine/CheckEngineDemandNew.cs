using System;
using System.Threading.Tasks;
using AutoLibLocal.DemandNew;
using GraphicModule;
using NetTools;
using AutoLibLocal;


namespace LocalMain.DemandNew
{
	public class CheckEngineDemandNew
	{
		private static DemandNewEngineManager _manager;

		static CheckEngineDemandNew()
		{
		}

		public static void Initialize()
		{
			try
			{
				_manager = new DemandNewEngineManager();
				_manager.Load();

				// GraphicModule의 ObjectDemandChart에 SnapshotBus 조회 콜백 등록
				ObjectDemandChart.GetBusCallback = (blockId) =>
				{
					return _manager?.GetBus(blockId);
				};

				if (_manager.EngineCount > 0)
				{
					if (Tools.IsLangKorean())
						SmLog.Message(LogLevel.INFO, LogCategory.DATA_SAVE,
							String.Format("신형 Demand Control 엔진 초기화: {0}개 블록", _manager.EngineCount));
					else
						SmLog.Message(LogLevel.INFO, LogCategory.DATA_SAVE,
							String.Format("New Demand Control engine initialized: {0} blocks", _manager.EngineCount));
				}
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					"CheckEngineDemandNew.Initialize", ex.Message);
			}
		}

		public static async Task TickAsync()
		{
			if (_manager == null) return;
			await _manager.TickAsync();
		}

		/// <summary>
		/// 설정 변경 후 엔진을 다시 로드한다.
		/// FormDemandNewSettings에서 저장 후 호출.
		/// </summary>
		public static void ReloadConfig()
		{
			if (_manager == null) return;
			try
			{
				_manager.Load();

				// 콜백 재등록
				ObjectDemandChart.GetBusCallback = (blockId) =>
				{
					return _manager?.GetBus(blockId);
				};

				SmLog.Message(LogLevel.INFO, LogCategory.DATA_SAVE,
					String.Format("DemandNew engine reloaded: {0} blocks", _manager.EngineCount));
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					"CheckEngineDemandNew.ReloadConfig", ex.Message);
			}
		}

		public static DemandNewEngineManager Manager
		{
			get { return _manager; }
		}
	}
}

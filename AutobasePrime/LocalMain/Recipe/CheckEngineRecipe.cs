using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;
using NetTools;
using Newtonsoft.Json;

namespace LocalMain
{
	/// <summary>
	/// Step 실행 결과 (ISA-88 Full Transition Model)
	/// </summary>
	public class TransitionResult
	{
		public TransitionType Type;
		public int TargetStepOrder = -1;  // Loop 시 점프 대상
		public string Error;              // Exception/Abort 메시지

		public static TransitionResult Completed()
		{
			return new TransitionResult { Type = TransitionType.Complete };
		}
		public static TransitionResult Excepted(string error)
		{
			return new TransitionResult { Type = TransitionType.Exception, Error = error };
		}
		public static TransitionResult Aborted(string error)
		{
			return new TransitionResult { Type = TransitionType.Abort, Error = error };
		}
		public static TransitionResult Looped(int target)
		{
			return new TransitionResult { Type = TransitionType.Loop, TargetStepOrder = target };
		}
		public static TransitionResult Ended()
		{
			return new TransitionResult { Type = TransitionType.End };
		}
	}

	/// <summary>
	/// 액션 실행 실패 정보 (태그별 개별 추적)
	/// </summary>
	public class ActionFailureInfo
	{
		public string TagName;
		public string SetValue;
		public string ErrorMessage;
	}

	/// <summary>
	/// ISA-88 Lite 레시피 실행 엔진: 다중 동시 실행 지원
	/// Download (태그 쓰기) / Upload (태그 읽기)
	/// </summary>
	public class CheckEngineRecipe
	{
		/// <summary>
		/// 레시피 실행 컨텍스트
		/// </summary>
		public class RecipeExecutionContext
		{
			public string RecipeName;
			public string UnitName;       // null = 전체 레시피
			public bool IsExecuting;
			public int CurrentStepIndex;
			public int TotalSteps;
			public DateTime StartTime;
			public CancellationTokenSource CTS;
			public string LastError = "";

			// ISA-88 Phase 2: Batch 관련
			public string BatchId = "";
			public int ControlRecipeId;

			// ISA-88 Phase 3: 상태 머신
			public BatchStateMachine StateMachine;
			public ManualResetEventSlim HoldEvent = new ManualResetEventSlim(true); // 초기: signaled (통과)

			// Step 레벨 상태 (C-6: Item 12)
			public ConcurrentDictionary<int, StepState> StepStates =
				new ConcurrentDictionary<int, StepState>();

			// 무한루프 보호: 전역 안전 제한
			public int TotalStepExecutions;          // Step 실행 총 횟수
			public long TotalTransitionEvaluations;  // 전이 평가 총 횟수

			// 사용자 추적
			public string Username = "";

			// 현재 Step 시작 시각 (UI 타임아웃 카운트다운용)
			public DateTime CurrentStepStartTime;
		}

		/// <summary>
		/// 배치 안전 제한 상수 (산업 안전)
		/// </summary>
		const int MaxBatchDurationHours = 24;                 // 배치 최대 실행 시간
		const int MaxTotalStepExecutions = 10000;             // Step 실행 총 횟수 제한
		const long MaxTotalTransitionEvaluations = 1000000;   // 전이 평가 총 횟수 제한

		/// <summary>
		/// 실행 키: 충돌 방지용 struct (recipe|unit 문자열 대신)
		/// </summary>
		struct ExecutionKey : IEquatable<ExecutionKey>
		{
			public readonly string Recipe;
			public readonly string Unit;

			public ExecutionKey(string recipe, string unit)
			{
				Recipe = recipe ?? "";
				Unit = unit ?? "";
			}

			public bool Equals(ExecutionKey other)
			{
				return string.Equals(Recipe, other.Recipe, StringComparison.Ordinal)
					&& string.Equals(Unit, other.Unit, StringComparison.Ordinal);
			}

			public override bool Equals(object obj)
			{
				return obj is ExecutionKey other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked { return (Recipe.GetHashCode() * 397) ^ Unit.GetHashCode(); }
			}

			public override string ToString()
			{
				return string.IsNullOrEmpty(Unit) ? Recipe : Recipe + " [" + Unit + "]";
			}
		}

		static ConcurrentDictionary<ExecutionKey, RecipeExecutionContext> _executions =
			new ConcurrentDictionary<ExecutionKey, RecipeExecutionContext>();

		#region Background Log Queue (Single Writer Worker)

		/// <summary>
		/// 로그 항목 기본 인터페이스
		/// </summary>
		abstract class LogEntry
		{
			public abstract Task WriteAsync(DataPostgres db);
		}

		class ExecutionLogEntry : LogEntry
		{
			public int RecipeId;
			public string RecipeName, UnitName, Action, Status, ErrorMessage, Username;
			public int StepIndex, TotalSteps;
			public DateTime? ExecStart, ExecEnd;

			public override async Task WriteAsync(DataPostgres db)
			{
				await db.InsertRecipeExecutionLogAsync(
					RecipeId, RecipeName, UnitName, Action, Status,
					StepIndex, TotalSteps, ErrorMessage,
					Username, Environment.MachineName,
					ExecStart, ExecEnd);
			}
		}

		class TransitionLogEntry : LogEntry
		{
			public string BatchId, StepName, TransitionType, Expression, ActionTaken, UnitName, ErrorMessage, Username;
			public int StepOrder, TransitionIndex;
			public bool EvaluatedResult;

			public override async Task WriteAsync(DataPostgres db)
			{
				await db.InsertTransitionLogAsync(
					BatchId, StepOrder, StepName,
					TransitionIndex, TransitionType, Expression,
					EvaluatedResult, ActionTaken, UnitName,
					ErrorMessage, Username, Environment.MachineName);
			}
		}

		static readonly BlockingCollection<LogEntry> _logQueue = new BlockingCollection<LogEntry>(10000);
		static readonly Thread _logWorker;

		static CheckEngineRecipe()
		{
			_logWorker = new Thread(LogWriterLoop)
			{
				Name = "RecipeLogWriter",
				IsBackground = true
			};
			_logWorker.Start();
		}

		static void LogWriterLoop()
		{
			foreach (var entry in _logQueue.GetConsumingEnumerable())
			{
				try
				{
					var db = DataPostgres.Instance;
					if (db == null) continue;
					entry.WriteAsync(db).GetAwaiter().GetResult();
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"LogWriter: {ex.Message}");
				}
			}
		}

		/// <summary>
		/// 로그를 큐에 추가 (호출자 차단 없음, 단일 Writer가 순차 기록)
		/// </summary>
		static void EnqueueExecutionLog(int recipeId, string recipeName, string unitName,
			string action, string status, int stepIndex, int totalSteps, string errorMessage,
			DateTime? execStart, DateTime? execEnd)
		{
			string username = null;
			try { username = AutoLib.SharedData.userInfo?.sUsername; } catch { }

			_logQueue.TryAdd(new ExecutionLogEntry
			{
				RecipeId = recipeId, RecipeName = recipeName, UnitName = unitName,
				Action = action, Status = status,
				StepIndex = stepIndex, TotalSteps = totalSteps, ErrorMessage = errorMessage,
				Username = username, ExecStart = execStart, ExecEnd = execEnd
			});
		}

		static void EnqueueTransitionLog(
			string batchId, int stepOrder, string stepName,
			int transitionIndex, string transitionType, string expression,
			bool evaluatedResult, string actionTaken, string unitName,
			string errorMessage)
		{
			string username = null;
			try { username = AutoLib.SharedData.userInfo?.sUsername; } catch { }

			_logQueue.TryAdd(new TransitionLogEntry
			{
				BatchId = batchId, StepOrder = stepOrder, StepName = stepName,
				TransitionIndex = transitionIndex, TransitionType = transitionType, Expression = expression,
				EvaluatedResult = evaluatedResult, ActionTaken = actionTaken, UnitName = unitName,
				ErrorMessage = errorMessage, Username = username
			});
		}

		#endregion

		/// <summary>
		/// 실행 키 생성 (struct 기반 — 충돌 방지)
		/// </summary>
		static ExecutionKey MakeKey(string recipeName, string unitName)
		{
			return new ExecutionKey(recipeName, unitName);
		}

		/// <summary>
		/// 하나라도 실행 중 여부 (하위호환)
		/// </summary>
		public static bool IsExecuting()
		{
			foreach (var kv in _executions)
			{
				if (kv.Value.IsExecuting) return true;
			}
			return false;
		}

		/// <summary>
		/// 특정 키 실행 중 여부
		/// </summary>
		public static bool IsExecuting(string recipeName, string unitName)
		{
			var key = MakeKey(recipeName, unitName);
			RecipeExecutionContext ctx;
			if (_executions.TryGetValue(key, out ctx))
				return ctx.IsExecuting;
			return false;
		}

		/// <summary>
		/// 현재 실행 중인 레시피 이름 (하위호환: 첫 번째 실행 중인 것 반환)
		/// </summary>
		public static string GetExecutingRecipeName()
		{
			foreach (var kv in _executions)
			{
				if (kv.Value.IsExecuting) return kv.Value.RecipeName;
			}
			return "";
		}

		/// <summary>
		/// 전체 실행 목록 스냅샷
		/// </summary>
		public static RecipeExecutionContext[] GetAllExecutions()
		{
			var list = new ArrayList();
			foreach (var kv in _executions)
			{
				if (kv.Value.IsExecuting)
					list.Add(kv.Value);
			}
			var result = new RecipeExecutionContext[list.Count];
			list.CopyTo(result);
			return result;
		}

		/// <summary>
		/// 레시피 Download: DB에서 로드 → 각 Step 순차 실행 (태그에 값 쓰기)
		/// unitName이 지정되면 해당 Unit의 step만 실행
		/// </summary>
		public static async Task<(bool success, string error)> RecipeDownload(string recipeName, string unitName = null, string username = null)
		{
			if (string.IsNullOrEmpty(username))
				try { username = AutoLib.SharedData.userInfo?.sUsername; } catch { }

			var key = MakeKey(recipeName, unitName);
			var ctx = new RecipeExecutionContext
			{
				RecipeName = recipeName,
				UnitName = unitName,
				IsExecuting = true,
				CurrentStepIndex = 0,
				TotalSteps = 0,
				StartTime = DateTime.Now,
				CTS = new CancellationTokenSource(),
				Username = username ?? ""
			};

			if (!_executions.TryAdd(key, ctx))
			{
				string msg = Tools.IsLangKorean()
					? "동일 레시피가 이미 실행 중입니다: " + key
					: "Same recipe is already executing: " + key;
				return (false, msg);
			}

			DateTime execStart = DateTime.Now;

			try
			{
				// DB에서 레시피 로드
				RecipeData recipe = await RecipeManager.LoadRecipeByNameAsync(recipeName);
				if (recipe == null)
				{
					ctx.LastError = Tools.IsLangKorean()
						? "레시피를 찾을 수 없습니다: " + recipeName
						: "Recipe not found: " + recipeName;
					return (false, ctx.LastError);
				}

				// 승인 상태 검증 (draft 레시피 실행 차단)
				if (recipe.status != "approved")
				{
					ctx.LastError = Tools.IsLangKorean()
						? "승인되지 않은 레시피는 실행할 수 없습니다 (status=" + recipe.status + ")"
						: "Cannot execute unapproved recipe (status=" + recipe.status + ")";
					return (false, ctx.LastError);
				}

				// 실행할 step 목록 결정
				ArrayList stepsToExecute = GetStepsForExecution(recipe, unitName);
				if (stepsToExecute == null)
				{
					ctx.LastError = Tools.IsLangKorean()
						? "Unit을 찾을 수 없습니다: " + unitName
						: "Unit not found: " + unitName;
					return (false, ctx.LastError);
				}

				ctx.TotalSteps = stepsToExecute.Count;

				// 실행 로그: STARTED
				await InsertExecutionLog(recipe.recipe_id, recipeName, unitName,
					"DOWNLOAD", "STARTED", 0, ctx.TotalSteps, null, execStart, null);

				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean() ? "레시피 Download 시작: {0}{1}" : "Recipe Download started: {0}{1}",
					recipeName, !string.IsNullOrEmpty(unitName) ? " [" + unitName + "]" : ""));

				// 각 Step 순차 실행
				for (int s = 0; s < stepsToExecute.Count; s++)
				{
					ctx.CurrentStepIndex = s;
					var step = (RecipeStepData)stepsToExecute[s];

					string stepResult = await ExecuteStepDownload(step, ctx.CTS.Token);
					if (stepResult != null)
					{
						ctx.LastError = String.Format(
							Tools.IsLangKorean()
								? "Step {0}({1}) 실패: {2}"
								: "Step {0}({1}) failed: {2}",
							step.step_order, step.step_name, stepResult);

						await InsertExecutionLog(recipe.recipe_id, recipeName, unitName,
							"DOWNLOAD", "FAILED", s, ctx.TotalSteps, ctx.LastError, execStart, DateTime.Now);

						SmLog.LogError(LogCategory.SYSTEM, String.Format(
							Tools.IsLangKorean() ? "레시피 Download 실패: {0} - {1}" : "Recipe Download failed: {0} - {1}",
							recipeName, ctx.LastError));

						return (false, ctx.LastError);
					}
				}

				// 실행 로그: COMPLETED
				await InsertExecutionLog(recipe.recipe_id, recipeName, unitName,
					"DOWNLOAD", "COMPLETED", ctx.TotalSteps, ctx.TotalSteps, null, execStart, DateTime.Now);

				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean() ? "레시피 Download 완료: {0}" : "Recipe Download completed: {0}", recipeName));

				return (true, null);
			}
			catch (Exception ex)
			{
				ctx.LastError = ex.Message;

				await InsertExecutionLog(0, recipeName, unitName,
					"DOWNLOAD", "FAILED", ctx.CurrentStepIndex, ctx.TotalSteps, ex.Message, execStart, DateTime.Now);

				SmLog.LogError(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean() ? "레시피 Download 예외: {0} - {1}" : "Recipe Download exception: {0} - {1}",
					recipeName, ex.Message));

				return (false, ctx.LastError);
			}
			finally
			{
				ctx.IsExecuting = false;
				RecipeExecutionContext removed;
				_executions.TryRemove(key, out removed);
			}
		}

		/// <summary>
		/// 레시피 Upload: 현재 태그값을 읽어 새 버전 레시피로 저장
		/// </summary>
		public static async Task<(bool success, string error)> RecipeUpload(string recipeName, string unitName = null)
		{
			var key = MakeKey(recipeName, unitName);
			var ctx = new RecipeExecutionContext
			{
				RecipeName = recipeName,
				UnitName = unitName,
				IsExecuting = true,
				StartTime = DateTime.Now,
				CTS = new CancellationTokenSource()
			};

			if (!_executions.TryAdd(key, ctx))
			{
				string msg = Tools.IsLangKorean()
					? "동일 레시피가 이미 실행 중입니다: " + key
					: "Same recipe is already executing: " + key;
				return (false, msg);
			}

			DateTime execStart = DateTime.Now;

			try
			{
				// DB에서 기존 레시피 구조 로드
				RecipeData sourceRecipe = await RecipeManager.LoadRecipeByNameAsync(recipeName);
				if (sourceRecipe == null)
				{
					ctx.LastError = Tools.IsLangKorean()
						? "레시피를 찾을 수 없습니다: " + recipeName
						: "Recipe not found: " + recipeName;
					return (false, ctx.LastError);
				}

				// 실행할 step 목록 결정
				ArrayList stepsToRead = GetStepsForExecution(sourceRecipe, unitName);
				if (stepsToRead == null)
				{
					ctx.LastError = Tools.IsLangKorean()
						? "Unit을 찾을 수 없습니다: " + unitName
						: "Unit not found: " + unitName;
					return (false, ctx.LastError);
				}

				ctx.TotalSteps = stepsToRead.Count;

				// 실행 로그: STARTED
				await InsertExecutionLog(sourceRecipe.recipe_id, recipeName, unitName,
					"UPLOAD", "STARTED", 0, ctx.TotalSteps, null, execStart, null);

				// ISA-88: Upload 시 Control Recipe Snapshot 기반
				// unitName 지정 시 해당 Unit만 포함 (전체 복사 방지)
				RecipeData newRecipe = new RecipeData();
				newRecipe.recipe_id = 0;
				newRecipe.recipe_guid = Guid.NewGuid().ToString("N");
				newRecipe.recipe_name = String.Format("{0}@{1}", recipeName, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
				newRecipe.description = String.Format("Upload snapshot from {0}{1} at {2}",
					recipeName,
					!string.IsNullOrEmpty(unitName) ? " [" + unitName + "]" : "",
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				newRecipe.recipe_mode = sourceRecipe.recipe_mode;
				newRecipe.version = 1;
				newRecipe.status = "draft";

				// 실행 대상 step만 스냅샷 복사
				ArrayList allSteps = new ArrayList();
				if (!string.IsNullOrEmpty(unitName))
				{
					// 지정 Unit만 복사
					for (int u = 0; u < sourceRecipe.units.Count; u++)
					{
						var srcUnit = (RecipeUnitData)sourceRecipe.units[u];
						if (string.Equals(srcUnit.unit_name, unitName, StringComparison.OrdinalIgnoreCase))
						{
							var clonedUnit = srcUnit.Clone();
							clonedUnit.unit_id = 0;
							for (int s = 0; s < clonedUnit.steps.Count; s++)
							{
								var st = (RecipeStepData)clonedUnit.steps[s];
								st.step_id = 0;
								for (int t = 0; t < st.items.Count; t++)
									((RecipeItemData)st.items[t]).item_id = 0;
								allSteps.Add(st);
							}
							newRecipe.units.Add(clonedUnit);
							break;
						}
					}
				}
				else
				{
					// 전체 복사
					for (int s = 0; s < sourceRecipe.steps.Count; s++)
					{
						var clonedStep = ((RecipeStepData)sourceRecipe.steps[s]).Clone();
						clonedStep.step_id = 0;
						for (int t = 0; t < clonedStep.items.Count; t++)
							((RecipeItemData)clonedStep.items[t]).item_id = 0;
						newRecipe.steps.Add(clonedStep);
						allSteps.Add(clonedStep);
					}
					for (int u = 0; u < sourceRecipe.units.Count; u++)
					{
						var clonedUnit = ((RecipeUnitData)sourceRecipe.units[u]).Clone();
						clonedUnit.unit_id = 0;
						for (int s = 0; s < clonedUnit.steps.Count; s++)
						{
							var st = (RecipeStepData)clonedUnit.steps[s];
							st.step_id = 0;
							for (int t = 0; t < st.items.Count; t++)
								((RecipeItemData)st.items[t]).item_id = 0;
							allSteps.Add(st);
						}
						newRecipe.units.Add(clonedUnit);
					}
				}

				for (int s = 0; s < allSteps.Count; s++)
				{
					var step = (RecipeStepData)allSteps[s];
					for (int i = 0; i < step.items.Count; i++)
					{
						var item = (RecipeItemData)step.items[i];
						try
						{
							if (TagLib.IsTagExist(item.tag_name))
							{
								int[] tagPos = new int[1];
								TagPublicClass tp = TagLib.GetStructPublic(item.tag_name, ref tagPos);
								object currObj = tp.GetCurr();
								if (currObj != null)
									item.set_value = currObj.ToString();
							}
						}
						catch (Exception ex)
						{
							Debug.WriteLine($"Upload 태그 읽기 실패 [{item.tag_name}]: {ex.Message}");
						}
					}
				}

				// 새 레시피로 저장
				var result = await RecipeManager.SaveRecipeAsync(newRecipe);
				if (result.error != null)
				{
					ctx.LastError = Tools.IsLangKorean()
						? "레시피 저장 실패: " + result.error
						: "Recipe save failed: " + result.error;

					await InsertExecutionLog(sourceRecipe.recipe_id, recipeName, unitName,
						"UPLOAD", "FAILED", 0, ctx.TotalSteps, ctx.LastError, execStart, DateTime.Now);

					return (false, ctx.LastError);
				}

				// 실행 로그: COMPLETED
				await InsertExecutionLog(sourceRecipe.recipe_id, recipeName, unitName,
					"UPLOAD", "COMPLETED", ctx.TotalSteps, ctx.TotalSteps, null, execStart, DateTime.Now);

				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean()
						? "레시피 Upload 완료: {0} → {1}"
						: "Recipe Upload completed: {0} → {1}",
					recipeName, newRecipe.recipe_name));

				return (true, null);
			}
			catch (Exception ex)
			{
				ctx.LastError = ex.Message;

				await InsertExecutionLog(0, recipeName, unitName,
					"UPLOAD", "FAILED", 0, ctx.TotalSteps, ex.Message, execStart, DateTime.Now);

				SmLog.LogError(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean() ? "레시피 Upload 예외: {0} - {1}" : "Recipe Upload exception: {0} - {1}",
					recipeName, ex.Message));

				return (false, ctx.LastError);
			}
			finally
			{
				ctx.IsExecuting = false;
				RecipeExecutionContext removed;
				_executions.TryRemove(key, out removed);
			}
		}

		/// <summary>
		/// 실행할 Step 목록 결정: unitName 지정 시 해당 Unit의 steps, 아니면 전체
		/// </summary>
		static ArrayList GetStepsForExecution(RecipeData recipe, string unitName)
		{
			if (string.IsNullOrEmpty(unitName))
			{
				// 전체: recipe 직속 + 모든 unit의 step을 step_order 순으로 합침
				ArrayList all = new ArrayList();
				for (int s = 0; s < recipe.steps.Count; s++)
					all.Add(recipe.steps[s]);
				for (int u = 0; u < recipe.units.Count; u++)
				{
					var unit = (RecipeUnitData)recipe.units[u];
					for (int s = 0; s < unit.steps.Count; s++)
						all.Add(unit.steps[s]);
				}
				// step_order 기준 정렬
				all.Sort(new StepOrderComparer());
				return all;
			}
			else
			{
				// 특정 Unit의 step만
				for (int u = 0; u < recipe.units.Count; u++)
				{
					var unit = (RecipeUnitData)recipe.units[u];
					if (unit.unit_name == unitName)
						return unit.steps;
				}
				return null; // Unit 없음
			}
		}

		class StepOrderComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				return ((RecipeStepData)x).step_order.CompareTo(((RecipeStepData)y).step_order);
			}
		}

		/// <summary>
		/// 실행 계획: Direct steps (순차) + Unit groups (병렬) (C-4: Item 7)
		/// </summary>
		class ExecutionPlan
		{
			public ArrayList DirectSteps = new ArrayList();
			public List<(string unitName, ArrayList steps)> UnitGroups = new List<(string, ArrayList)>();
		}

		static ExecutionPlan GetExecutionPlan(RecipeData recipe)
		{
			var plan = new ExecutionPlan();
			plan.DirectSteps = new ArrayList(recipe.steps);
			plan.DirectSteps.Sort(new StepOrderComparer());

			for (int u = 0; u < recipe.units.Count; u++)
			{
				var unit = (RecipeUnitData)recipe.units[u];
				var steps = new ArrayList(unit.steps);
				steps.Sort(new StepOrderComparer());
				plan.UnitGroups.Add((unit.unit_name, steps));
			}
			return plan;
		}

		/// <summary>
		/// Unit 내 Step들을 순차 실행 (병렬 Unit 실행 시 사용)
		/// </summary>
		static async Task<string> ExecuteUnitStepsAsync(string unitName, ArrayList steps,
			RecipeExecutionContext ctx)
		{
			// step_order → 인덱스 매핑 (Unit 내 전이용)
			var stepOrderIndex = new Dictionary<int, int>();
			for (int i = 0; i < steps.Count; i++)
				stepOrderIndex[((RecipeStepData)steps[i]).step_order] = i;

			var loopCounters = new Dictionary<string, int>();
			int s = 0;

			while (s < steps.Count)
			{
				ctx.CTS.Token.ThrowIfCancellationRequested();

				// Hold/Abort 체크
				string holdResult = await CheckHoldAbort(ctx, ctx.CTS.Token);
				if (holdResult != null) return holdResult;

				var step = (RecipeStepData)steps[s];
				var result = await ExecuteStepDownloadV2(step, ctx.CTS.Token, ctx, loopCounters);

				switch (result.Type)
				{
					case TransitionType.Complete:
						s++;
						break;

					case TransitionType.Loop:
						int targetIdx;
						if (stepOrderIndex.TryGetValue(result.TargetStepOrder, out targetIdx))
							s = targetIdx;
						else
							return String.Format("Unit '{0}': Invalid loop target step_order: {1}",
								unitName, result.TargetStepOrder);
						break;

					case TransitionType.End:
						s = steps.Count; // Unit 내 실행 종료
						break;

					case TransitionType.Exception:
					case TransitionType.Abort:
						return result.Error ?? "Step failed";
				}
			}
			return null;
		}

		/// <summary>
		/// Hold/Abort 체크 헬퍼 — Step 내부 while 루프에서 호출 (C-3: Item 8)
		/// </summary>
		static async Task<string> CheckHoldAbort(RecipeExecutionContext ctx, CancellationToken ct)
		{
			if (ctx?.StateMachine == null) return null;

			if (ctx.StateMachine.CurrentState == BatchState.Holding)
			{
				ctx.StateMachine.TransitionAuto(BatchState.Held);
				ctx.HoldEvent.Reset();
				await Task.Run(() => ctx.HoldEvent.Wait(ct));
				if (ctx.StateMachine.CurrentState == BatchState.Restarting)
					ctx.StateMachine.TransitionAuto(BatchState.Running);
			}
			if (ctx.StateMachine.CurrentState == BatchState.Aborting)
			{
				ctx.StateMachine.TransitionAuto(BatchState.Aborted);
				return "Aborted";
			}
			return null;
		}

		/// <summary>
		/// Exit/Abort Actions 실행 헬퍼 (best-effort: 개별 태그 실패해도 나머지 계속 실행)
		/// 실패한 태그를 List&lt;ActionFailureInfo&gt;로 반환하고 감사 로그에 기록
		/// </summary>
		static async Task<List<ActionFailureInfo>> ExecuteActions(string actionsJson, CancellationToken ct,
			string batchId = null, int stepOrder = 0, string stepName = null, string unitName = null)
		{
			if (string.IsNullOrEmpty(actionsJson)) return null;

			List<ActionFailureInfo> failures = null;

			try
			{
				var items = JsonConvert.DeserializeObject<List<RecipeJsonHelper.RecipeItemJsonDto>>(actionsJson);
				if (items == null) return null;
				for (int i = 0; i < items.Count; i++)
				{
					ct.ThrowIfCancellationRequested();
					try
					{
						await PlcScan.SetTagValue(items[i].tag_name, items[i].set_value, false);
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						if (failures == null) failures = new List<ActionFailureInfo>();
						failures.Add(new ActionFailureInfo
						{
							TagName = items[i].tag_name,
							SetValue = items[i].set_value,
							ErrorMessage = ex.Message
						});

						// 개별 태그 실패 감사 로그 (fire-and-forget)
						InsertTransitionLogFireAndForget(
							batchId, stepOrder, stepName,
							i, "ACTION_FAILURE", null,
							false,
							String.Format("tag={0}, value={1}", items[i].tag_name, items[i].set_value),
							unitName, ex.Message);

						Debug.WriteLine(String.Format("Action tag failure [{0}={1}]: {2}",
							items[i].tag_name, items[i].set_value, ex.Message));
					}
				}
			}
			catch (OperationCanceledException) { throw; }
			catch (Exception ex)
			{
				Debug.WriteLine($"Action JSON parse error: {ex.Message}");
			}

			return failures;
		}

		#region ISA-88 Full Transition Model Helpers

		/// <summary>
		/// legacy 필드 → Transition 자동 생성 (하위호환)
		/// running_expression → Exception (priority 0, 표현식 반전)
		/// exit_expression → Complete (priority 10)
		/// legacy tag/compare/value → Complete (priority 10)
		/// </summary>
		static List<StepTransition> BuildTransitionsFromLegacy(RecipeStepData step)
		{
			var list = new List<StepTransition>();

			if (!string.IsNullOrEmpty(step.running_expression))
			{
				list.Add(new StepTransition
				{
					priority = 0,
					expression = "!(" + step.running_expression + ")",
					type = TransitionType.Exception,
					description = "Auto: Running condition"
				});
			}

			if (!string.IsNullOrEmpty(step.exit_expression))
			{
				list.Add(new StepTransition
				{
					priority = 10,
					expression = step.exit_expression,
					type = TransitionType.Complete,
					description = "Auto: Exit condition"
				});
			}
			else if (!string.IsNullOrEmpty(step.condition_tag) && step.condition_type != "none")
			{
				string op;
				switch (step.condition_type)
				{
					case "greater": op = ">"; break;
					case "less": op = "<"; break;
					default: op = "=="; break;
				}
				list.Add(new StepTransition
				{
					priority = 10,
					expression = "$" + step.condition_tag + " " + op + " " + (step.condition_value ?? "0"),
					type = TransitionType.Complete,
					description = "Auto: Legacy exit"
				});
			}

			return list;
		}

		/// <summary>
		/// Transition 타입별 평가 카테고리 (낮을수록 먼저 평가)
		/// 산업 안전 규칙: Abort > Exception > Complete/Loop/End
		/// </summary>
		static int GetTypeCategory(TransitionType t)
		{
			switch (t)
			{
				case TransitionType.Abort: return 0;      // 최우선: 즉시 중단
				case TransitionType.Exception: return 1;   // 차순위: 감시 조건 (RunningCondition)
				default: return 2;                         // Complete, Loop, End
			}
		}

		/// <summary>
		/// Transition 정렬: ① 타입 카테고리 (Abort→Exception→나머지)  ② 같은 카테고리 내 priority
		/// </summary>
		static int CompareTransitions(StepTransition a, StepTransition b)
		{
			int catA = GetTypeCategory(a.type);
			int catB = GetTypeCategory(b.type);
			if (catA != catB) return catA.CompareTo(catB);
			return a.priority.CompareTo(b.priority);
		}

		/// <summary>
		/// Step에서 transitions 목록 가져오기 (JSON 우선, 없으면 legacy 자동 생성)
		/// 정렬: Abort → Exception → Complete/Loop/End (같은 타입 내 priority 순)
		/// </summary>
		static List<StepTransition> GetStepTransitions(RecipeStepData step)
		{
			if (!string.IsNullOrEmpty(step.transitions_json))
			{
				try
				{
					var list = JsonConvert.DeserializeObject<List<StepTransition>>(step.transitions_json);
					if (list != null && list.Count > 0)
					{
						list.Sort(CompareTransitions);
						return list;
					}
				}
				catch { }
			}
			return BuildTransitionsFromLegacy(step);
		}

		/// <summary>
		/// 단일 Step Download 실행 (ISA-88 Full Transition Model)
		/// Entry → Items Write → Wait → Transition Evaluation Loop
		/// </summary>
		static async Task<TransitionResult> ExecuteStepDownloadV2(
			RecipeStepData step, CancellationToken ct,
			RecipeExecutionContext ctx = null,
			Dictionary<string, int> loopCounters = null)
		{
			// Step 상태: WaitingEntry
			ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.WaitingEntry, (k, v) => StepState.WaitingEntry);

			// 전역 안전 제한: Step 실행 총 횟수
			if (ctx != null)
			{
				ctx.TotalStepExecutions++;
				if (ctx.TotalStepExecutions > MaxTotalStepExecutions)
				{
					ctx.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
					await ExecuteActions(step.abort_actions_json, ct,
						ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
						ctx != null ? ctx.UnitName : null);
					return TransitionResult.Excepted(String.Format(
						Tools.IsLangKorean()
							? "Step 실행 횟수 한계 초과 ({0}회, 안전 제한)"
							: "Step execution limit exceeded ({0}, safety)",
						MaxTotalStepExecutions));
				}
			}

			// ① Entry Expression 대기
			bool hasEntryExpr = !string.IsNullOrEmpty(step.entry_expression);
			bool hasEntryTag = !string.IsNullOrEmpty(step.entry_condition_tag) && step.entry_condition_type != "none";

			if (hasEntryExpr || hasEntryTag)
			{
				DateTime entryStart = DateTime.Now;
				int entryTimeoutMs = step.entry_timeout_ms;

				while (true)
				{
					ct.ThrowIfCancellationRequested();

					string holdResult = await CheckHoldAbort(ctx, ct);
					if (holdResult != null)
					{
						ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Aborted, (k, v) => StepState.Aborted);
						await ExecuteActions(step.abort_actions_json, ct,
						ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
						ctx != null ? ctx.UnitName : null);
						return TransitionResult.Aborted(holdResult);
					}

					if (entryTimeoutMs > 0 && (DateTime.Now - entryStart).TotalMilliseconds >= entryTimeoutMs)
					{
						InsertTransitionLogFireAndForget(
							ctx != null ? ctx.BatchId : null,
							step.step_order, step.step_name,
							-1, "ENTRY_TIMEOUT",
							hasEntryExpr ? step.entry_expression : step.entry_condition_tag,
							false, "abort_actions",
							ctx != null ? ctx.UnitName : null,
							String.Format("Entry timeout after {0}ms", entryTimeoutMs));

						ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
						await ExecuteActions(step.abort_actions_json, ct,
							ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
							ctx != null ? ctx.UnitName : null);
						return TransitionResult.Excepted(String.Format(
							Tools.IsLangKorean()
								? "시작 조건 타임아웃 ({0}ms)"
								: "Entry condition timeout ({0}ms)", entryTimeoutMs));
					}

					bool entryMet;
					try
					{
						entryMet = hasEntryExpr
							? RecipeExpressionEvaluator.Evaluate(step.entry_expression)
							: CheckCondition(step.entry_condition_tag, step.entry_condition_value, step.entry_condition_type);
					}
					catch (Exception exEntry)
					{
						// 보수적 정책: 잘못된 표현식 → abort_actions → Exception
						string entryEvalError = String.Format(
							Tools.IsLangKorean()
								? "시작 조건 표현식 평가 오류 [Step {0}]: {1} — 표현식: {2}"
								: "Entry expression error [Step {0}]: {1} — expr: {2}",
							step.step_order, exEntry.Message,
							hasEntryExpr ? step.entry_expression : step.entry_condition_tag);

						InsertTransitionLogFireAndForget(
							ctx != null ? ctx.BatchId : null,
							step.step_order, step.step_name,
							-1, "ENTRY_EXPRESSION_ERROR",
							hasEntryExpr ? step.entry_expression : step.entry_condition_tag,
							false, "abort_actions",
							ctx != null ? ctx.UnitName : null,
							exEntry.Message);

						ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
						await ExecuteActions(step.abort_actions_json, ct,
							ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
							ctx != null ? ctx.UnitName : null);
						return TransitionResult.Excepted(entryEvalError);
					}

					if (entryMet)
					{
						// Entry 조건 충족 감사 로그
						InsertTransitionLogFireAndForget(
							ctx != null ? ctx.BatchId : null,
							step.step_order, step.step_name,
							-1, "ENTRY_MET",
							hasEntryExpr ? step.entry_expression : step.entry_condition_tag,
							true, "entry_passed",
							ctx != null ? ctx.UnitName : null, null);
						break;
					}
					// Adaptive Delay: timeout 기반 (50~200ms)
					int entryPoll = Math.Max(50, Math.Min(entryTimeoutMs / 20, 200));
					await Task.Delay(entryPoll, ct);
				}
			}

			// Step 상태: Running
			ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Running, (k, v) => StepState.Running);

			// Step 이력 로깅: STEP_START
			EnqueueExecutionLog(
				ctx?.ControlRecipeId ?? 0,
				ctx?.RecipeName ?? "", ctx?.UnitName,
				"STEP_START", "RUNNING",
				step.step_order, ctx?.TotalSteps ?? 0,
				null, DateTime.Now, null);

			// ② Step의 모든 Item에 태그값 쓰기
			for (int i = 0; i < step.items.Count; i++)
			{
				ct.ThrowIfCancellationRequested();
				var item = (RecipeItemData)step.items[i];
				try
				{
					await PlcScan.SetTagValue(item.tag_name, item.set_value, false);
				}
				catch (Exception ex)
				{
					ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
					await ExecuteActions(step.abort_actions_json, ct,
						ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
						ctx != null ? ctx.UnitName : null);
					string tagErr = String.Format(
						Tools.IsLangKorean()
							? "태그 쓰기 실패 [{0}={1}]: {2}"
							: "Tag write failed [{0}={1}]: {2}",
						item.tag_name, item.set_value, ex.Message);
					EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
						"STEP_COMPLETE", "EXCEPTION", step.step_order, ctx?.TotalSteps ?? 0, tagErr, null, DateTime.Now);
					return TransitionResult.Excepted(tagErr);
				}
			}

			// ③ 최소 대기시간
			if (step.wait_time_ms > 0)
				await Task.Delay(step.wait_time_ms, ct);

			// ④ Transition 목록 가져오기
			var transitions = GetStepTransitions(step);

			// 전이 없으면 즉시 Complete (조건 없는 Step)
			if (transitions.Count == 0)
			{
				await ExecuteActions(step.exit_actions_json, ct,
						ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
						ctx != null ? ctx.UnitName : null);
				ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Completed, (k, v) => StepState.Completed);
				EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
					"STEP_COMPLETE", "COMPLETED", step.step_order, ctx?.TotalSteps ?? 0, null, null, DateTime.Now);
				return TransitionResult.Completed();
			}

			// ⑤ Transition Evaluation Loop (Running 상태에서 조건 평가)
			DateTime startTime = DateTime.Now;
			DateTime transitionTimerStart = startTime;  // Per-transition timeout 기준 시각

			// UI 카운트다운용 현재 Step 시작 시각 기록
			if (ctx != null)
				ctx.CurrentStepStartTime = startTime;
			int timeoutMs = step.timeout_ms > 0 ? step.timeout_ms : 30000;

			while (true)
			{
				ct.ThrowIfCancellationRequested();

				// 전역 안전 제한: 전이 평가 횟수
				if (ctx != null)
				{
					ctx.TotalTransitionEvaluations++;
					if (ctx.TotalTransitionEvaluations > MaxTotalTransitionEvaluations)
					{
						ctx.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
						await ExecuteActions(step.abort_actions_json, ct,
						ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
						ctx != null ? ctx.UnitName : null);
						string safetyMsg = Tools.IsLangKorean()
							? "전이 평가 횟수 한계 초과 (안전 제한)"
							: "Transition evaluation limit exceeded (safety)";
						EnqueueExecutionLog(ctx.ControlRecipeId, ctx.RecipeName, ctx.UnitName,
							"STEP_COMPLETE", "EXCEPTION", step.step_order, ctx.TotalSteps, safetyMsg, null, DateTime.Now);
						return TransitionResult.Excepted(safetyMsg);
					}

					// 전역 안전 제한: 배치 총 실행 시간
					if ((DateTime.Now - ctx.StartTime).TotalHours >= MaxBatchDurationHours)
					{
						ctx.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
						await ExecuteActions(step.abort_actions_json, ct,
						ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
						ctx != null ? ctx.UnitName : null);
						string durMsg = String.Format(
							Tools.IsLangKorean()
								? "배치 최대 실행 시간 초과 ({0}시간)"
								: "Batch max duration exceeded ({0}h)",
							MaxBatchDurationHours);
						EnqueueExecutionLog(ctx.ControlRecipeId, ctx.RecipeName, ctx.UnitName,
							"STEP_COMPLETE", "EXCEPTION", step.step_order, ctx.TotalSteps, durMsg, null, DateTime.Now);
						return TransitionResult.Excepted(durMsg);
					}
				}

				// Hold/Abort 즉시 반응
				string holdRes = await CheckHoldAbort(ctx, ct);
				if (holdRes != null)
				{
					ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Aborted, (k, v) => StepState.Aborted);
					await ExecuteActions(step.abort_actions_json, ct,
						ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
						ctx != null ? ctx.UnitName : null);
					EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
						"STEP_COMPLETE", "ABORTED", step.step_order, ctx?.TotalSteps ?? 0, holdRes, null, DateTime.Now);
					return TransitionResult.Aborted(holdRes);
				}

				// Timeout 체크
				if ((DateTime.Now - startTime).TotalMilliseconds >= timeoutMs)
				{
					InsertTransitionLogFireAndForget(
						ctx != null ? ctx.BatchId : null,
						step.step_order, step.step_name,
						-1, "TIMEOUT", null,
						false, "abort_actions",
						ctx != null ? ctx.UnitName : null,
						String.Format("Transition timeout after {0}ms", timeoutMs));

					ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
					await ExecuteActions(step.abort_actions_json, ct,
						ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
						ctx != null ? ctx.UnitName : null);
					string toMsg = String.Format(
						Tools.IsLangKorean()
							? "전이 조건 타임아웃 ({0}ms)"
							: "Transition timeout ({0}ms)", timeoutMs);
					EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
						"STEP_COMPLETE", "TIMEOUT", step.step_order, ctx?.TotalSteps ?? 0, toMsg, null, DateTime.Now);
					return TransitionResult.Excepted(toMsg);
				}

				// 우선순위 순으로 전이 평가
				for (int t = 0; t < transitions.Count; t++)
				{
					var tr = transitions[t];
					bool exprResult;

					// Per-transition timeout: timeout_ms > 0이면 경과시간 체크 후 자동 발동
					bool timedOut = (tr.timeout_ms > 0 &&
						(DateTime.Now - transitionTimerStart).TotalMilliseconds >= tr.timeout_ms);

					if (timedOut)
					{
						exprResult = true;
						InsertTransitionLogFireAndForget(
							ctx != null ? ctx.BatchId : null,
							step.step_order, step.step_name,
							t, tr.type.ToString() + "_TIMEOUT", tr.expression,
							true, "auto_trigger_timeout",
							ctx != null ? ctx.UnitName : null,
							String.Format("Per-transition timeout after {0}ms", tr.timeout_ms));
					}
					else if (string.IsNullOrEmpty(tr.expression))
						exprResult = true;
					else
					{
						try
						{
							exprResult = RecipeExpressionEvaluator.Evaluate(tr.expression);
						}
						catch (Exception exEval)
						{
							// 보수적 정책: 잘못된 표현식 → abort_actions → Exception
							string evalError = String.Format(
								Tools.IsLangKorean()
									? "전이 표현식 평가 오류 [Step {0}, T{1}]: {2} — 표현식: {3}"
									: "Transition expression error [Step {0}, T{1}]: {2} — expr: {3}",
								step.step_order, t, exEval.Message, tr.expression);

							InsertTransitionLogFireAndForget(
								ctx != null ? ctx.BatchId : null,
								step.step_order, step.step_name,
								t, "EXPRESSION_ERROR", tr.expression,
								false, "abort_actions",
								ctx != null ? ctx.UnitName : null,
								exEval.Message);

							ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
							await ExecuteActions(step.abort_actions_json, ct,
								ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
								ctx != null ? ctx.UnitName : null);
							EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
								"STEP_COMPLETE", "EXCEPTION", step.step_order, ctx?.TotalSteps ?? 0, evalError, null, DateTime.Now);
							return TransitionResult.Excepted(evalError);
						}
					}

					if (!exprResult) continue;

					// 전이 조건 충족 감사 로그 (fire-and-forget)
					string actionDesc;
					switch (tr.type)
					{
						case TransitionType.Complete: actionDesc = "exit_actions"; break;
						case TransitionType.End: actionDesc = "exit_actions"; break;
						case TransitionType.Loop:
							actionDesc = "jump->" + (tr.target_step_order >= 0 ? tr.target_step_order : step.step_order);
							break;
						default: actionDesc = "abort_actions"; break;
					}
					InsertTransitionLogFireAndForget(
						ctx != null ? ctx.BatchId : null,
						step.step_order, step.step_name,
						t, tr.type.ToString(), tr.expression,
						true, actionDesc,
						ctx != null ? ctx.UnitName : null, null);

					// 전이 조건 충족 — 타입별 처리
					switch (tr.type)
					{
						case TransitionType.Complete:
							await ExecuteActions(step.exit_actions_json, ct,
								ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
								ctx != null ? ctx.UnitName : null);
							ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Completed, (k, v) => StepState.Completed);
							EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
								"STEP_COMPLETE", "COMPLETED", step.step_order, ctx?.TotalSteps ?? 0, null, null, DateTime.Now);
							return TransitionResult.Completed();

						case TransitionType.Exception:
							ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
							await ExecuteActions(step.abort_actions_json, ct,
								ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
								ctx != null ? ctx.UnitName : null);
							EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
								"STEP_COMPLETE", "EXCEPTION", step.step_order, ctx?.TotalSteps ?? 0,
								(Tools.IsLangKorean() ? "전이 예외: " : "Transition exception: ") + tr.expression, null, DateTime.Now);
							return TransitionResult.Excepted(
								(Tools.IsLangKorean() ? "전이 예외: " : "Transition exception: ") + tr.expression);

						case TransitionType.Abort:
							ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Aborted, (k, v) => StepState.Aborted);
							await ExecuteActions(step.abort_actions_json, ct,
								ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
								ctx != null ? ctx.UnitName : null);
							EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
								"STEP_COMPLETE", "ABORTED", step.step_order, ctx?.TotalSteps ?? 0,
								(Tools.IsLangKorean() ? "전이 중단: " : "Transition abort: ") + tr.expression, null, DateTime.Now);
							return TransitionResult.Aborted(
								(Tools.IsLangKorean() ? "전이 중단: " : "Transition abort: ") + tr.expression);

						case TransitionType.Loop:
							int target = tr.target_step_order >= 0 ? tr.target_step_order : step.step_order;
							string loopKey = step.step_order + "->" + target;
							if (loopCounters != null)
							{
								int count;
								loopCounters.TryGetValue(loopKey, out count);
								count++;
								if (tr.max_loop_count > 0 && count > tr.max_loop_count)
								{
									InsertTransitionLogFireAndForget(
										ctx != null ? ctx.BatchId : null,
										step.step_order, step.step_name,
										t, "LOOP_LIMIT", tr.expression,
										false, "abort_actions",
										ctx != null ? ctx.UnitName : null,
										String.Format("Loop limit exceeded: {0} (max={1})", loopKey, tr.max_loop_count));

									ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Exception, (k, v) => StepState.Exception);
									await ExecuteActions(step.abort_actions_json, ct,
										ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
										ctx != null ? ctx.UnitName : null);
									string loopLimitMsg = String.Format(
										Tools.IsLangKorean()
											? "반복 제한 초과: {0} (최대={1})"
											: "Loop limit exceeded: {0} (max={1})",
										loopKey, tr.max_loop_count);
									EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
										"STEP_COMPLETE", "EXCEPTION", step.step_order, ctx?.TotalSteps ?? 0, loopLimitMsg, null, DateTime.Now);
									return TransitionResult.Excepted(loopLimitMsg);
								}
								loopCounters[loopKey] = count;
							}
							// Loop: exit_actions 실행 안 함 (Loop는 정상 완료가 아님)
							EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
								"STEP_COMPLETE", "LOOPED", step.step_order, ctx?.TotalSteps ?? 0, null, null, DateTime.Now);
							return TransitionResult.Looped(target);

						case TransitionType.End:
							await ExecuteActions(step.exit_actions_json, ct,
								ctx != null ? ctx.BatchId : null, step.step_order, step.step_name,
								ctx != null ? ctx.UnitName : null);
							ctx?.StepStates?.AddOrUpdate(step.step_order, StepState.Completed, (k, v) => StepState.Completed);
							EnqueueExecutionLog(ctx?.ControlRecipeId ?? 0, ctx?.RecipeName ?? "", ctx?.UnitName,
								"STEP_COMPLETE", "ENDED", step.step_order, ctx?.TotalSteps ?? 0, null, null, DateTime.Now);
							return TransitionResult.Ended();
					}
				}

				// Adaptive Delay: step timeout 기반 (50~200ms)
				int exitPoll = Math.Max(50, Math.Min(step.timeout_ms / 20, 200));
				await Task.Delay(exitPoll, ct);
			}
		}

		#endregion

		/// <summary>
		/// 단일 Step Download 실행 — 하위호환 래퍼 (RecipeDownload용)
		/// </summary>
		static async Task<string> ExecuteStepDownload(RecipeStepData step, CancellationToken ct,
			RecipeExecutionContext ctx = null)
		{
			var result = await ExecuteStepDownloadV2(step, ct, ctx, null);
			return (result.Type == TransitionType.Complete || result.Type == TransitionType.End)
				? null : (result.Error ?? "Step failed");
		}

		/// <summary>
		/// 조건 체크: 태그의 현재 값과 조건값 비교
		/// </summary>
		static bool CheckCondition(string conditionTag, string conditionValue, string conditionType)
		{
			try
			{
				if (!TagLib.IsTagExist(conditionTag)) return false;

				int[] tagPos = new int[1];
				TagPublicClass tp = TagLib.GetStructPublic(conditionTag, ref tagPos);
				object currObj = tp.GetCurr();
				if (currObj == null) return false;

				double currVal;
				double condVal;

				if (!double.TryParse(currObj.ToString(), out currVal))
					return false;

				if (conditionType == "equal")
					return currObj.ToString() == conditionValue;

				if (!double.TryParse(conditionValue, out condVal))
					return false;

				if (conditionType == "greater")
					return currVal > condVal;
				else if (conditionType == "less")
					return currVal < condVal;

				return false;
			}
			catch (Exception)
			{
				throw;  // 보수적 정책: caller에게 예외 위임 → abort_actions → Exception
			}
		}

		#region ISA-88 Phase 2: Batch Start (Standard 모드)

		/// <summary>
		/// ISA-88 Standard 배치 시작: Master → Control Recipe 스냅샷 → 배치 실행
		/// 기존 RecipeDownload()를 포함하되, 배치 기록을 남기는 확장 버전
		/// </summary>
		public static async Task<(bool success, string error, string batchId)> BatchStart(
			string recipeName, string batchId = null, string username = null, string unitName = null)
		{
			// batch_id 자동 생성: "yyyyMMdd-HHmmss-NNN"
			if (string.IsNullOrEmpty(batchId))
				batchId = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + (++_batchCounter).ToString("D3");

			var key = MakeKey(recipeName, unitName);
			var ctx = new RecipeExecutionContext
			{
				RecipeName = recipeName,
				UnitName = unitName,
				IsExecuting = true,
				CurrentStepIndex = 0,
				TotalSteps = 0,
				StartTime = DateTime.Now,
				CTS = new CancellationTokenSource(),
				BatchId = batchId
			};

			// 상태 머신 초기화
			ctx.StateMachine = new BatchStateMachine();
			ctx.StateMachine.Start(); // Idle → Running

			if (!_executions.TryAdd(key, ctx))
			{
				string msg = Tools.IsLangKorean()
					? "동일 레시피가 이미 실행 중입니다: " + key
					: "Same recipe is already executing: " + key;
				return (false, msg, null);
			}

			DateTime execStart = DateTime.Now;
			var db = DataPostgres.Instance;

			try
			{
				if (string.IsNullOrEmpty(username))
					try { username = AutoLib.SharedData.userInfo?.sUsername; } catch { }

				// 1. Master 로드
				RecipeData master = await RecipeManager.LoadRecipeByNameAsync(recipeName);
				if (master == null)
				{
					string err = Tools.IsLangKorean()
						? "레시피를 찾을 수 없습니다: " + recipeName
						: "Recipe not found: " + recipeName;
					return (false, err, null);
				}

				// 2. 승인 상태 확인
				if (master.status != "approved")
				{
					string err = Tools.IsLangKorean()
						? "승인되지 않은 레시피는 실행할 수 없습니다 (status=" + master.status + ")"
						: "Cannot execute unapproved recipe (status=" + master.status + ")";
					return (false, err, null);
				}

				// 3. Control Recipe 생성 (Master 스냅샷)
				string snapshotJson = RecipeJsonHelper.ToJsonString(master);
				int controlRecipeId = -1;
				if (db != null)
				{
					controlRecipeId = await db.CreateControlRecipeAsync(
						master.recipe_id, master.version, batchId, snapshotJson, username);
				}
				ctx.ControlRecipeId = controlRecipeId;

				// 4. Batch Execution 레코드 생성
				if (db != null)
				{
					var batchRecord = new BatchExecutionRecord
					{
						batch_id = batchId,
						control_recipe_id = controlRecipeId,
						master_recipe_id = master.recipe_id,
						master_recipe_name = master.recipe_name,
						master_version = master.version,
						operator_id = username ?? "",
						start_time = execStart,
						status = "running"
					};
					await db.CreateBatchExecutionAsync(batchRecord);
				}

				// Control Recipe 상태 → running
				if (db != null && controlRecipeId > 0)
					await db.UpdateControlRecipeStatusAsync(controlRecipeId, "running");

				// 5. Control Recipe 스냅샷에서 실행 (C-2: Item 10 — 진정한 ISA-88 스냅샷 실행)
				RecipeData snapshot = RecipeJsonHelper.FromJsonString(snapshotJson);
				if (snapshot == null)
				{
					string err = Tools.IsLangKorean()
						? "스냅샷 역직렬화 실패" : "Snapshot deserialization failed";
					return (false, err, batchId);
				}

				// 실행 계획 생성 (C-4: Item 7 — 병렬 Unit 실행)
				var plan = GetExecutionPlan(snapshot);

				// unitName 지정 시: 해당 Unit만 실행 (직속 Step 건너뛰기)
				if (!string.IsNullOrEmpty(unitName))
				{
					plan.DirectSteps.Clear();
					for (int u = plan.UnitGroups.Count - 1; u >= 0; u--)
					{
						if (!string.Equals(plan.UnitGroups[u].unitName, unitName, StringComparison.OrdinalIgnoreCase))
							plan.UnitGroups.RemoveAt(u);
					}

					if (plan.UnitGroups.Count == 0)
					{
						string err = Tools.IsLangKorean()
							? "Unit을 찾을 수 없습니다: " + unitName
							: "Unit not found: " + unitName;
						return (false, err, null);
					}
				}

				int totalSteps = plan.DirectSteps.Count;
				for (int u = 0; u < plan.UnitGroups.Count; u++)
					totalSteps += plan.UnitGroups[u].steps.Count;
				ctx.TotalSteps = totalSteps;

				// 실행 로그: STARTED
				await InsertExecutionLog(master.recipe_id, recipeName, unitName,
					"BATCH_START", "STARTED", 0, ctx.TotalSteps, null, execStart, null);

				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean()
						? "배치 시작: {0} (BatchID={1}, Ver={2})"
						: "Batch started: {0} (BatchID={1}, Ver={2})",
					recipeName, batchId, master.version));

				// 6a. Recipe 직속 Step — Transition-based while loop
				var stepOrderIndex = new Dictionary<int, int>();
				for (int i = 0; i < plan.DirectSteps.Count; i++)
					stepOrderIndex[((RecipeStepData)plan.DirectSteps[i]).step_order] = i;

				var loopCounters = new Dictionary<string, int>();
				int s = 0;
				bool directStepsFailed = false;
				bool recipeEnded = false;  // End 전이 시 Unit 실행도 건너뛰기

				while (s < plan.DirectSteps.Count)
				{
					// Hold/Abort 체크 (Step 간)
					string holdResult = await CheckHoldAbort(ctx, ctx.CTS.Token);
					if (holdResult != null)
					{
						if (db != null)
						{
							await db.UpdateBatchStatusAsync(batchId, "aborted", "aborted", DateTime.Now);
							if (controlRecipeId > 0)
								await db.UpdateControlRecipeStatusAsync(controlRecipeId, "aborted");
						}
						await InsertExecutionLog(master.recipe_id, recipeName, null,
							"BATCH_START", "ABORTED", s, ctx.TotalSteps, holdResult, execStart, DateTime.Now);
						return (false, holdResult, batchId);
					}

					ctx.CurrentStepIndex = s;
					var step = (RecipeStepData)plan.DirectSteps[s];

					var stepResult = await ExecuteStepDownloadV2(step, ctx.CTS.Token, ctx, loopCounters);

					switch (stepResult.Type)
					{
						case TransitionType.Complete:
							s++;
							break;

						case TransitionType.Loop:
							int targetIdx;
							if (stepOrderIndex.TryGetValue(stepResult.TargetStepOrder, out targetIdx))
								s = targetIdx;
							else
							{
								ctx.LastError = String.Format(
									Tools.IsLangKorean()
										? "잘못된 Loop 대상 step_order: {0}"
										: "Invalid loop target step_order: {0}",
									stepResult.TargetStepOrder);
								directStepsFailed = true;
								s = plan.DirectSteps.Count;
							}
							break;

						case TransitionType.End:
							s = plan.DirectSteps.Count; // 직속 Step 루프 탈출
							recipeEnded = true;          // Unit 실행도 건너뛰기
							break;

						case TransitionType.Exception:
						case TransitionType.Abort:
							ctx.LastError = String.Format(
								Tools.IsLangKorean()
									? "Step {0}({1}) {2}: {3}"
									: "Step {0}({1}) {2}: {3}",
								step.step_order, step.step_name,
								stepResult.Type.ToString(), stepResult.Error);
							directStepsFailed = true;
							s = plan.DirectSteps.Count;
							break;
					}
				}

				if (directStepsFailed)
				{
					await InsertExecutionLog(master.recipe_id, recipeName, null,
						"BATCH_START", "FAILED", ctx.CurrentStepIndex, ctx.TotalSteps, ctx.LastError, execStart, DateTime.Now);

					if (db != null)
					{
						await db.UpdateBatchStatusAsync(batchId, "failed", "failed", DateTime.Now);
						if (controlRecipeId > 0)
							await db.UpdateControlRecipeStatusAsync(controlRecipeId, "aborted");
					}
					return (false, ctx.LastError, batchId);
				}

				// 6b. Unit들 병렬 실행 (C-4: Item 7) — End 전이 시 건너뛰기
				if (plan.UnitGroups.Count > 0 && !recipeEnded)
				{
					var unitTasks = new Task<string>[plan.UnitGroups.Count];
					for (int u = 0; u < plan.UnitGroups.Count; u++)
					{
						int idx = u;
						unitTasks[u] = ExecuteUnitStepsAsync(
							plan.UnitGroups[idx].unitName, plan.UnitGroups[idx].steps, ctx);
					}
					await Task.WhenAll(unitTasks);

					// 모든 Unit 실패 수집 (첫 번째만이 아닌 전체 실패 집계)
					var unitFailures = new List<string>();
					for (int u = 0; u < unitTasks.Length; u++)
					{
						string unitResult = unitTasks[u].Result;
						if (unitResult != null)
						{
							string unitError = String.Format(
								Tools.IsLangKorean()
									? "Unit '{0}' 실패: {1}"
									: "Unit '{0}' failed: {1}",
								plan.UnitGroups[u].unitName, unitResult);
							unitFailures.Add(unitError);

							// 개별 Unit 실패 감사 로그
							InsertTransitionLogFireAndForget(
								batchId, -1, null,
								u, "UNIT_FAILURE", null,
								false, "unit_execution",
								plan.UnitGroups[u].unitName,
								unitResult);
						}
					}

					if (unitFailures.Count > 0)
					{
						// 모든 실패 집계
						if (unitFailures.Count == 1)
						{
							ctx.LastError = unitFailures[0];
						}
						else
						{
							var sb = new System.Text.StringBuilder();
							sb.AppendFormat(
								Tools.IsLangKorean()
									? "{0}개 Unit 실패:"
									: "{0} unit(s) failed:",
								unitFailures.Count);
							for (int f = 0; f < unitFailures.Count; f++)
							{
								sb.Append(" [");
								sb.Append(unitFailures[f]);
								sb.Append("]");
							}
							ctx.LastError = sb.ToString();
						}

						await InsertExecutionLog(master.recipe_id, recipeName, null,
							"BATCH_START", "FAILED", 0, ctx.TotalSteps, ctx.LastError, execStart, DateTime.Now);

						if (db != null)
						{
							await db.UpdateBatchStatusAsync(batchId, "failed", "failed", DateTime.Now);
							if (controlRecipeId > 0)
								await db.UpdateControlRecipeStatusAsync(controlRecipeId, "aborted");
						}
						return (false, ctx.LastError, batchId);
					}
				}

				// 7. 완료 기록
				if (ctx.StateMachine != null)
					ctx.StateMachine.SetComplete();
				await InsertExecutionLog(master.recipe_id, recipeName, null,
					"BATCH_START", "COMPLETED", ctx.TotalSteps, ctx.TotalSteps, null, execStart, DateTime.Now);

				if (db != null)
				{
					await db.UpdateBatchStatusAsync(batchId, "complete", "completed", DateTime.Now);
					if (controlRecipeId > 0)
						await db.UpdateControlRecipeStatusAsync(controlRecipeId, "completed");
				}

				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean()
						? "배치 완료: {0} (BatchID={1})"
						: "Batch completed: {0} (BatchID={1})",
					recipeName, batchId));

				return (true, null, batchId);
			}
			catch (OperationCanceledException)
			{
				// 사용자에 의한 중단
				if (db != null)
				{
					await db.UpdateBatchStatusAsync(batchId, "aborted", "aborted", DateTime.Now);
					if (ctx.ControlRecipeId > 0)
						await db.UpdateControlRecipeStatusAsync(ctx.ControlRecipeId, "aborted");
				}

				await InsertExecutionLog(0, recipeName, null,
					"BATCH_START", "ABORTED", ctx.CurrentStepIndex, ctx.TotalSteps, "Cancelled", execStart, DateTime.Now);

				return (false, "Batch cancelled", batchId);
			}
			catch (Exception ex)
			{
				ctx.LastError = ex.Message;

				if (db != null)
				{
					await db.UpdateBatchStatusAsync(batchId, "failed", "failed", DateTime.Now);
					if (ctx.ControlRecipeId > 0)
						await db.UpdateControlRecipeStatusAsync(ctx.ControlRecipeId, "aborted");
				}

				await InsertExecutionLog(0, recipeName, null,
					"BATCH_START", "FAILED", ctx.CurrentStepIndex, ctx.TotalSteps, ex.Message, execStart, DateTime.Now);

				SmLog.LogError(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean()
						? "배치 예외: {0} - {1}"
						: "Batch exception: {0} - {1}",
					recipeName, ex.Message));

				return (false, ctx.LastError, batchId);
			}
			finally
			{
				ctx.IsExecuting = false;
				RecipeExecutionContext removed;
				_executions.TryRemove(key, out removed);
			}
		}

		static int _batchCounter = 0;

		/// <summary>
		/// 배치 Hold: Running → Holding (실행 중인 배치를 일시정지)
		/// </summary>
		public static bool HoldBatch(string recipeName)
		{
			var key = MakeKey(recipeName, null);
			RecipeExecutionContext ctx;
			if (!_executions.TryGetValue(key, out ctx)) return false;
			if (ctx.StateMachine == null) return false;

			bool result = ctx.StateMachine.Hold();
			if (result)
			{
				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean()
						? "배치 Hold 요청: {0}"
						: "Batch Hold requested: {0}",
					recipeName));
			}
			return result;
		}

		/// <summary>
		/// 배치 Restart: Held → Restarting → Running (일시정지된 배치를 재개)
		/// </summary>
		public static bool RestartBatch(string recipeName)
		{
			var key = MakeKey(recipeName, null);
			RecipeExecutionContext ctx;
			if (!_executions.TryGetValue(key, out ctx)) return false;
			if (ctx.StateMachine == null) return false;

			bool result = ctx.StateMachine.Restart();
			if (result)
			{
				ctx.HoldEvent.Set(); // 대기 중인 스레드 풀기

				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean()
						? "배치 Restart 요청: {0}"
						: "Batch Restart requested: {0}",
					recipeName));
			}
			return result;
		}

		/// <summary>
		/// 배치 Abort: 실행 중/일시정지 배치를 즉시 중단
		/// </summary>
		public static bool AbortBatch(string recipeName)
		{
			var key = MakeKey(recipeName, null);
			RecipeExecutionContext ctx;
			if (!_executions.TryGetValue(key, out ctx)) return false;
			if (ctx.StateMachine == null) return false;

			bool result = ctx.StateMachine.Abort();
			if (result)
			{
				// Held 상태 대기를 풀어 Abort 처리 가능하도록
				ctx.HoldEvent.Set();
				// CancellationToken으로 진행 중인 Delay/Wait도 중단
				try { ctx.CTS.Cancel(); } catch { }

				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean()
						? "배치 Abort 요청: {0}"
						: "Batch Abort requested: {0}",
					recipeName));
			}
			return result;
		}

		/// <summary>
		/// 배치 상태 조회
		/// </summary>
		public static string GetBatchState(string recipeName)
		{
			var key = MakeKey(recipeName, null);
			RecipeExecutionContext ctx;
			if (!_executions.TryGetValue(key, out ctx)) return "idle";
			if (ctx.StateMachine == null) return ctx.IsExecuting ? "running" : "idle";
			return ctx.StateMachine.GetStateString();
		}

		#endregion

		#region ISA-88 Phase 4: Quick 모드 실행

		/// <summary>
		/// Quick Download: 모든 Step의 모든 Item 태그에 값을 한 번에 쓰기 (조건/대기 무시)
		/// 배치 기록 없음, 상태 머신 없음 — 단순 태그값 출력 전용
		/// </summary>
		public static async Task<(bool success, string error)> RecipeQuickDownload(string recipeName)
		{
			var key = MakeKey(recipeName, null);
			var ctx = new RecipeExecutionContext
			{
				RecipeName = recipeName,
				IsExecuting = true,
				StartTime = DateTime.Now,
				CTS = new CancellationTokenSource()
			};

			if (!_executions.TryAdd(key, ctx))
			{
				string msg = Tools.IsLangKorean()
					? "동일 레시피가 이미 실행 중입니다: " + key
					: "Same recipe is already executing: " + key;
				return (false, msg);
			}

			DateTime execStart = DateTime.Now;

			try
			{
				// DB에서 레시피 로드
				RecipeData recipe = await RecipeManager.LoadRecipeByNameAsync(recipeName);
				if (recipe == null)
				{
					return (false, Tools.IsLangKorean()
						? "레시피를 찾을 수 없습니다: " + recipeName
						: "Recipe not found: " + recipeName);
				}

				// 승인 상태 검증
				if (recipe.status != "approved")
				{
					return (false, Tools.IsLangKorean()
						? "승인되지 않은 레시피는 실행할 수 없습니다 (status=" + recipe.status + ")"
						: "Cannot execute unapproved recipe (status=" + recipe.status + ")");
				}

				// 모든 Step의 모든 Item 수집 (조건/대기 무시)
				ArrayList allSteps = GetStepsForExecution(recipe, null);
				int tagCount = 0;

				if (allSteps != null)
				{
					for (int s = 0; s < allSteps.Count; s++)
					{
						var step = (RecipeStepData)allSteps[s];
						for (int i = 0; i < step.items.Count; i++)
						{
							var item = (RecipeItemData)step.items[i];
							try
							{
								await PlcScan.SetTagValue(item.tag_name, item.set_value, false);
								tagCount++;
							}
							catch (Exception ex)
							{
								return (false, String.Format(
									Tools.IsLangKorean()
										? "태그 쓰기 실패 [{0}={1}]: {2}"
										: "Tag write failed [{0}={1}]: {2}",
									item.tag_name, item.set_value, ex.Message));
							}
						}
					}
				}

				// 실행 로그
				await InsertExecutionLog(recipe.recipe_id, recipeName, null,
					"QUICK_DOWNLOAD", "COMPLETED", tagCount, tagCount, null, execStart, DateTime.Now);

				SmLog.LogInfo(LogCategory.SYSTEM, String.Format(
					Tools.IsLangKorean()
						? "Quick Download 완료: {0} ({1}개 태그)"
						: "Quick Download completed: {0} ({1} tags)",
					recipeName, tagCount));

				return (true, null);
			}
			catch (Exception ex)
			{
				await InsertExecutionLog(0, recipeName, null,
					"QUICK_DOWNLOAD", "FAILED", 0, 0, ex.Message, execStart, DateTime.Now);
				return (false, ex.Message);
			}
			finally
			{
				ctx.IsExecuting = false;
				RecipeExecutionContext removed;
				_executions.TryRemove(key, out removed);
			}
		}

		#endregion

		/// <summary>
		/// 실행 로그 INSERT — Background Log Queue 사용 (Single Writer, 비차단)
		/// await는 하위호환을 위해 유지하지만 실제로는 큐에 넣고 즉시 반환
		/// </summary>
		static Task InsertExecutionLog(int recipeId, string recipeName, string unitName,
			string action, string status, int stepIndex, int totalSteps, string errorMessage,
			DateTime? execStart, DateTime? execEnd)
		{
			EnqueueExecutionLog(recipeId, recipeName, unitName,
				action, status, stepIndex, totalSteps, errorMessage,
				execStart, execEnd);
			return Task.CompletedTask;
		}

		/// <summary>
		/// 전이 로그 INSERT — Background Log Queue 사용 (Single Writer, 비차단)
		/// </summary>
		static void InsertTransitionLogFireAndForget(
			string batchId, int stepOrder, string stepName,
			int transitionIndex, string transitionType, string expression,
			bool evaluatedResult, string actionTaken, string unitName,
			string errorMessage)
		{
			EnqueueTransitionLog(batchId, stepOrder, stepName,
				transitionIndex, transitionType, expression,
				evaluatedResult, actionTaken, unitName,
				errorMessage);
		}
	}
}

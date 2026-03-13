using System;
using System.Diagnostics;

namespace LocalMain
{
	/// <summary>
	/// ISA-88 Batch State Machine
	/// 상태 전이:
	///   Idle       → Running    (Start)
	///   Running    → Holding    (Hold)    → Held (자동 전이)
	///   Running    → Aborting   (Abort)   → Aborted (자동 전이)
	///   Running    → Complete   (모든 Step 완료)
	///   Held       → Restarting (Restart) → Running (자동 전이)
	///   Held       → Aborting   (Abort)   → Aborted (자동 전이)
	/// </summary>
	public enum BatchState
	{
		Idle,
		Running,
		Holding,
		Held,
		Restarting,
		Aborting,
		Aborted,
		Complete
	}

	/// <summary>
	/// Step 레벨 상태 (ISA-88 Step 상태 머신)
	/// </summary>
	public enum StepState
	{
		Pending,
		WaitingEntry,
		Running,
		WaitingExit,
		Completed,
		Aborted,
		Exception
	}

	public class BatchStateMachine
	{
		public BatchState CurrentState { get; private set; } = BatchState.Idle;

		/// <summary>
		/// 상태 전이 이벤트: (이전상태, 새상태)
		/// </summary>
		public event Action<BatchState, BatchState> StateChanged;

		private readonly object _lock = new object();

		/// <summary>
		/// Start: Idle → Running
		/// </summary>
		public bool Start()
		{
			lock (_lock)
			{
				if (CurrentState != BatchState.Idle && CurrentState != BatchState.Complete
					&& CurrentState != BatchState.Aborted)
					return false;

				return Transition(BatchState.Running);
			}
		}

		/// <summary>
		/// Hold: Running → Holding (엔진 루프에서 Held로 자동 전이)
		/// </summary>
		public bool Hold()
		{
			lock (_lock)
			{
				if (CurrentState != BatchState.Running)
					return false;

				return Transition(BatchState.Holding);
			}
		}

		/// <summary>
		/// Restart: Held → Restarting (엔진 루프에서 Running으로 자동 전이)
		/// </summary>
		public bool Restart()
		{
			lock (_lock)
			{
				if (CurrentState != BatchState.Held)
					return false;

				return Transition(BatchState.Restarting);
			}
		}

		/// <summary>
		/// Abort: Running/Holding/Held/Restarting → Aborting (엔진 루프에서 Aborted로 자동 전이)
		/// </summary>
		public bool Abort()
		{
			lock (_lock)
			{
				if (CurrentState != BatchState.Running &&
					CurrentState != BatchState.Holding &&
					CurrentState != BatchState.Held &&
					CurrentState != BatchState.Restarting)
					return false;

				return Transition(BatchState.Aborting);
			}
		}

		/// <summary>
		/// Complete: Running → Complete (모든 Step 정상 완료 시 엔진에서 호출)
		/// </summary>
		public bool SetComplete()
		{
			lock (_lock)
			{
				if (CurrentState != BatchState.Running)
					return false;

				return Transition(BatchState.Complete);
			}
		}

		/// <summary>
		/// 엔진 내부에서 자동 전이 시 사용: Holding → Held, Restarting → Running, Aborting → Aborted
		/// </summary>
		public bool TransitionAuto(BatchState target)
		{
			lock (_lock)
			{
				// Holding → Held
				if (CurrentState == BatchState.Holding && target == BatchState.Held)
					return Transition(target);
				// Restarting → Running
				if (CurrentState == BatchState.Restarting && target == BatchState.Running)
					return Transition(target);
				// Aborting → Aborted
				if (CurrentState == BatchState.Aborting && target == BatchState.Aborted)
					return Transition(target);

				return false;
			}
		}

		/// <summary>
		/// 특정 상태로 전이 가능 여부
		/// </summary>
		public bool CanTransition(BatchState target)
		{
			lock (_lock)
			{
				switch (target)
				{
					case BatchState.Running:
						return CurrentState == BatchState.Idle || CurrentState == BatchState.Restarting
							|| CurrentState == BatchState.Complete || CurrentState == BatchState.Aborted;
					case BatchState.Holding:
						return CurrentState == BatchState.Running;
					case BatchState.Held:
						return CurrentState == BatchState.Holding;
					case BatchState.Restarting:
						return CurrentState == BatchState.Held;
					case BatchState.Aborting:
						return CurrentState == BatchState.Running || CurrentState == BatchState.Holding
							|| CurrentState == BatchState.Held || CurrentState == BatchState.Restarting;
					case BatchState.Aborted:
						return CurrentState == BatchState.Aborting;
					case BatchState.Complete:
						return CurrentState == BatchState.Running;
					default:
						return false;
				}
			}
		}

		/// <summary>
		/// 내부 전이 실행
		/// </summary>
		bool Transition(BatchState newState)
		{
			BatchState oldState = CurrentState;
			CurrentState = newState;

			Debug.WriteLine($"[BatchStateMachine] {oldState} → {newState}");

			try
			{
				StateChanged?.Invoke(oldState, newState);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BatchStateMachine] StateChanged 이벤트 오류: {ex.Message}");
			}

			return true;
		}

		/// <summary>
		/// 상태 문자열 반환 (DB/로그용)
		/// </summary>
		public string GetStateString()
		{
			return CurrentState.ToString().ToLower();
		}
	}
}

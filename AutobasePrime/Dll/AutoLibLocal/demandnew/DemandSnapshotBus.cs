using System;
using NetTools;

namespace AutoLibLocal.DemandNew
{
	public class DemandSnapshotBus
	{
		public event EventHandler<DemandSnapshot> SnapshotPublished;

		private readonly object _lock = new object();
		private DemandSnapshot _latestSnapshot;

		public DemandSnapshot LatestSnapshot
		{
			get { lock (_lock) { return _latestSnapshot; } }
		}

		public void Publish(DemandSnapshot snapshot)
		{
			lock (_lock)
			{
				_latestSnapshot = snapshot;
			}

			InvokeSafely(SnapshotPublished, this, snapshot);
		}

		private static void InvokeSafely(EventHandler<DemandSnapshot> handler, object sender, DemandSnapshot snapshot)
		{
			if (handler == null) return;

			Delegate[] subscribers = handler.GetInvocationList();
			for (int i = 0; i < subscribers.Length; i++)
			{
				try
				{
					((EventHandler<DemandSnapshot>)subscribers[i]).Invoke(sender, snapshot);
				}
				catch (Exception ex)
				{
					// Prevent a single subscriber failure from breaking engine tick flow.
					Log.Write(LogLevel.ERROR, LogCategory.DATA_SAVE,
						"DemandSnapshotBus subscriber failed: {0}", ex.Message);
				}
			}
		}
	}
}

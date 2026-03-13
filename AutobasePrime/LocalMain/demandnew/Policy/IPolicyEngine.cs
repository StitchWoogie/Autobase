using System;
using System.Collections.Generic;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public struct PolicyResult
	{
		public ControlDecision Decision;
		public int Step;
		public List<string> LoadIdsToControl;
		public string Reason;
	}

	public interface IPolicyEngine
	{
		PolicyResult Evaluate(DemandSnapshot snapshot, TargetResult target);
	}
}

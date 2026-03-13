using System;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public struct TargetResult
	{
		public double TargetKW;
		public double SafetyFactor;
		public double EffectiveTargetKW;
		public TargetReason Reason;
	}

	public interface ITargetProvider
	{
		TargetResult GetCurrentTarget(DateTime now);
	}
}

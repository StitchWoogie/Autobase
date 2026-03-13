using System;
using System.Threading.Tasks;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public interface IActuator
	{
		Task<ActuationResult> ExecuteAsync(string loadId, ControlDecision decision);
		bool VerifyFeedback(string loadId, ControlDecision expectedState);
	}
}

using System;
using System.Collections.Generic;
using NetTools;

namespace AutoLibLocal.DemandNew
{
	/// <summary>
	/// DemandSnapshot ↔ CommaBlockString 직렬화/역직렬화.
	/// AutoLibLocal에 배치하여 서버(LocalMain)와 클라이언트(ViewMain/AutoLib) 양쪽에서 참조.
	/// </summary>
	public static class DemandSnapshotSerializer
	{
		public static string Serialize(DemandSnapshot snap)
		{
			if (snap == null) return "";

			var c = new CommaBlockString();

			// 식별
			c.AddString(snap.BlockId ?? "");
			c.AddString(snap.Timestamp.ToString("o"));
			c.AddInt((int)snap.Mode);

			// 계측
			c.AddDouble(snap.CurrentKW);
			c.AddInt((int)snap.Quality);

			// 수요 윈도우
			c.AddInt(snap.IntervalMinutes);
			c.AddInt(snap.ElapsedSeconds);
			c.AddInt(snap.RemainingSeconds);
			c.AddString(snap.IntervalStart.ToString("o"));
			c.AddDouble(snap.BlockDemandKW);
			c.AddDouble(snap.BlockEnergyKWH);

			// 예측
			c.AddDouble(snap.ForecastDemandEndKW);
			c.AddDouble(snap.ForecastSlope);
			c.AddDouble(snap.Confidence);

			// 목표
			c.AddDouble(snap.TargetKW);
			c.AddDouble(snap.EffectiveTargetKW);
			c.AddInt((int)snap.TargetReason);

			// 정책
			c.AddInt((int)snap.LastDecision);
			c.AddInt(snap.CurrentStep);
			c.AddString(snap.PolicyReason ?? "");
			c.AddInt(snap.IsIntervalEnd ? 1 : 0);

			// 월 최대수요
			c.AddDouble(snap.MonthlyPeakKW);
			c.AddString(snap.MonthlyPeakTime.ToString("o"));

			// 부하 상태 리스트
			int loadCount = (snap.Loads != null) ? snap.Loads.Count : 0;
			c.AddInt(loadCount);
			for (int i = 0; i < loadCount; i++)
			{
				var ls = snap.Loads[i];
				c.AddString(ls.LoadId ?? "");
				c.AddString(ls.DisplayName ?? "");
				c.AddInt(ls.Priority);
				c.AddDouble(ls.EstimatedKW);
				c.AddInt(ls.IsShed ? 1 : 0);
				c.AddString(ls.LastShedTime.ToString("o"));
				c.AddString(ls.LastRestoreTime.ToString("o"));
			}

			// DemandCurve 배열
			int curveLen = snap.DemandCurveLength;
			c.AddInt(curveLen);
			if (snap.DemandCurve != null)
			{
				for (int i = 0; i < curveLen && i < snap.DemandCurve.Length; i++)
					c.AddDouble(snap.DemandCurve[i]);
			}

			return c.Get();
		}

		public static DemandSnapshot Deserialize(string data)
		{
			if (string.IsNullOrEmpty(data)) return null;

			var c = new CommaBlockString();
			c.Set(data);
			var snap = new DemandSnapshot();
			string tmp = "";
			double d = 0;
			int n = 0;

			// 식별
			c.GetString(ref tmp); snap.BlockId = tmp;
			c.GetString(ref tmp);
			DateTime dt;
			if (DateTime.TryParse(tmp, out dt)) snap.Timestamp = dt;
			c.GetInt(ref n); snap.Mode = (EngineMode)n;

			// 계측
			c.GetDouble(ref d); snap.CurrentKW = d;
			c.GetInt(ref n); snap.Quality = (MeasurementQuality)n;

			// 수요 윈도우
			c.GetInt(ref n); snap.IntervalMinutes = n;
			c.GetInt(ref n); snap.ElapsedSeconds = n;
			c.GetInt(ref n); snap.RemainingSeconds = n;
			c.GetString(ref tmp);
			if (DateTime.TryParse(tmp, out dt)) snap.IntervalStart = dt;
			c.GetDouble(ref d); snap.BlockDemandKW = d;
			c.GetDouble(ref d); snap.BlockEnergyKWH = d;

			// 예측
			c.GetDouble(ref d); snap.ForecastDemandEndKW = d;
			c.GetDouble(ref d); snap.ForecastSlope = d;
			c.GetDouble(ref d); snap.Confidence = d;

			// 목표
			c.GetDouble(ref d); snap.TargetKW = d;
			c.GetDouble(ref d); snap.EffectiveTargetKW = d;
			c.GetInt(ref n); snap.TargetReason = (TargetReason)n;

			// 정책
			c.GetInt(ref n); snap.LastDecision = (ControlDecision)n;
			c.GetInt(ref n); snap.CurrentStep = n;
			c.GetString(ref tmp); snap.PolicyReason = tmp;
			c.GetInt(ref n); snap.IsIntervalEnd = (n == 1);

			// 월 최대수요
			c.GetDouble(ref d); snap.MonthlyPeakKW = d;
			c.GetString(ref tmp);
			if (DateTime.TryParse(tmp, out dt)) snap.MonthlyPeakTime = dt;

			// 부하 상태 리스트
			int loadCount = 0;
			c.GetInt(ref loadCount);
			snap.Loads = new List<LoadStatus>();
			for (int i = 0; i < loadCount; i++)
			{
				var ls = new LoadStatus();
				c.GetString(ref tmp); ls.LoadId = tmp;
				c.GetString(ref tmp); ls.DisplayName = tmp;
				c.GetInt(ref n); ls.Priority = n;
				c.GetDouble(ref d); ls.EstimatedKW = d;
				c.GetInt(ref n); ls.IsShed = (n == 1);
				c.GetString(ref tmp);
				if (DateTime.TryParse(tmp, out dt)) ls.LastShedTime = dt;
				c.GetString(ref tmp);
				if (DateTime.TryParse(tmp, out dt)) ls.LastRestoreTime = dt;
				snap.Loads.Add(ls);
			}

			// DemandCurve 배열
			int curveLen = 0;
			c.GetInt(ref curveLen);
			snap.DemandCurveLength = curveLen;
			snap.DemandCurve = new double[Math.Max(curveLen, 1)];
			for (int i = 0; i < curveLen; i++)
			{
				c.GetDouble(ref d);
				snap.DemandCurve[i] = d;
			}

			return snap;
		}
	}
}

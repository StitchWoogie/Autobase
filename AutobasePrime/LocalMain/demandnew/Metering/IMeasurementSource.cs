using System;
using AutoLibLocal.DemandNew;

namespace LocalMain.DemandNew
{
	public struct MeasurementReading
	{
		public double Value;
		public MeasurementQuality Quality;
		public DateTime Timestamp;
	}

	public interface IMeasurementSource
	{
		MeasurementReading Read();
		string TagName { get; }
		bool IsAvailable { get; }
	}
}

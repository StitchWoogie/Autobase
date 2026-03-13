using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NetTools;

namespace AutoLibLocal.DemandNew
{
	public static class DemandNewConfigLoader
	{
		private const string CONFIG_FILE = "DemandNew.lstx";
		private const string LOADS_PREFIX = "DemandNewLoads_";

		public static string GetConfigFilePath()
		{
			return String.Format("{0}\\FUNCTION\\{1}", TotalConfig.sDirWorkProject, CONFIG_FILE);
		}

		public static string GetLoadsFilePath(string blockId)
		{
			return String.Format("{0}\\FUNCTION\\{1}{2}.lstx", TotalConfig.sDirWorkProject, LOADS_PREFIX, blockId);
		}

		public static List<DemandNewConfig> LoadAll()
		{
			var configs = new List<DemandNewConfig>();
			string path = GetConfigFilePath();

			if (!File.Exists(path)) return configs;

			try
			{
				using (var reader = new StreamReader(path, Encoding.UTF8))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						line = line.Trim();
						if (string.IsNullOrEmpty(line) || line.StartsWith("//")) continue;

						var config = ParseConfigLine(line);
						if (config != null)
						{
							config.Loads = LoadLoads(config.BlockId);
							configs.Add(config);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Write(LogLevel.ERROR, LogCategory.DATA_SAVE, "DemandNewConfigLoader.LoadAll: {0}", ex.Message);
			}

			return configs;
		}

		public static void SaveAll(List<DemandNewConfig> configs)
		{
			string path = GetConfigFilePath();
			string dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

			try
			{
				using (var writer = new StreamWriter(path, false, Encoding.UTF8))
				{
					foreach (var config in configs)
					{
						string line = FormatConfigLine(config);
						writer.WriteLine(line);
						SaveLoads(config.BlockId, config.Loads);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Write(LogLevel.ERROR, LogCategory.DATA_SAVE, "DemandNewConfigLoader.SaveAll: {0}", ex.Message);
			}
		}

		public static DemandNewConfig ParseConfigLine(string line)
		{
			var c = new CommaBlockString();
			c.Set(line);

			var config = new DemandNewConfig();
			string tmp = "";

			c.GetString(ref tmp); config.BlockId = tmp;
			c.GetString(ref tmp); config.Title = tmp;
			int mode = 0; c.GetInt(ref mode); config.Mode = (EngineMode)mode;
			int interval = 15; c.GetInt(ref interval); config.IntervalMinutes = interval;
			int aligned = 1; c.GetInt(ref aligned); config.ClockAligned = (aligned == 1);

			double d = 0;
			c.GetDouble(ref d); config.ContractKW = d;
			c.GetDouble(ref d); config.SafetyFactor = d;

			c.GetString(ref tmp); config.MeterTagName = tmp;
			int mtype = 0; c.GetInt(ref mtype); config.MeterType = (MeterType)mtype;
			c.GetDouble(ref d); config.PulseRatio = d;
			c.GetString(ref tmp); config.TargetTagName = tmp;
			int useTarget = 0; c.GetInt(ref useTarget); config.UseTargetTag = (useTarget == 1);
			int useEOI = 0; c.GetInt(ref useEOI); config.UseEOI = (useEOI == 1);
			c.GetString(ref tmp); config.EOITagName = tmp;
			int autoReset = 0; c.GetInt(ref autoReset); config.InputAutoReset = (autoReset == 1);
			c.GetString(ref tmp); config.PredictionDisplayTagName = tmp;

			c.GetDouble(ref d); config.ShedMarginKW = d;
			c.GetDouble(ref d); config.RestoreMarginKW = d;
			int protTime = 60; c.GetInt(ref protTime); config.ProtectionTimeSec = protTime;
			int multiStep = 0; c.GetInt(ref multiStep); config.MultiStepEnabled = (multiStep == 1);
			int stepCount = 1; c.GetInt(ref stepCount); config.StepCount = stepCount;
			int qualBad = 1; c.GetInt(ref qualBad); config.QualityBadBlockControl = (qualBad == 1);

			int trendWin = 120; c.GetInt(ref trendWin); config.TrendWindowSec = trendWin;
			c.GetDouble(ref d); config.EwmaAlpha = d;

			int dbSave = 0; c.GetInt(ref dbSave); config.DatabaseSaveEnabled = (dbSave == 1);
			c.GetString(ref tmp); config.DatabaseDsn = tmp;
			int usePg = 0; c.GetInt(ref usePg); config.UsePostgres = (usePg == 1);
			int failover = 1; c.GetInt(ref failover); config.FailoverEnabled = (failover == 1);

			int pctY = 120; c.GetInt(ref pctY); config.PercentY = pctY;

			// 시간대 목표 개수
			int tzCount = 0; c.GetInt(ref tzCount);
			for (int i = 0; i < tzCount; i++)
			{
				var tz = new TimeZoneTarget();
				int zone = 0; c.GetInt(ref zone); tz.Zone = (TariffZone)zone;
				int startMin = 0; c.GetInt(ref startMin); tz.StartTime = TimeSpan.FromMinutes(startMin);
				int endMin = 0; c.GetInt(ref endMin); tz.EndTime = TimeSpan.FromMinutes(endMin);
				c.GetDouble(ref d); tz.TargetKW = d;
				config.TimeZoneTargets.Add(tz);
			}

			// 다단계 제어 Steps
			int stepsCount = 0; c.GetInt(ref stepsCount);
			for (int i = 0; i < stepsCount; i++)
			{
				var step = new StepConfig();
				c.GetDouble(ref d); step.ReductionKW = d;
				int loadCount = 0; c.GetInt(ref loadCount);
				for (int j = 0; j < loadCount; j++)
				{
					c.GetString(ref tmp);
					if (!string.IsNullOrEmpty(tmp)) step.LoadIds.Add(tmp);
				}
				config.Steps.Add(step);
			}

			// DR(수요반응) 목표
			int drEnabled = 0; c.GetInt(ref drEnabled); config.DrTarget.IsEnabled = (drEnabled == 1);
			c.GetDouble(ref d); config.DrTarget.TargetKW = d;
			c.GetString(ref tmp);
			if (!string.IsNullOrEmpty(tmp))
			{
				DateTime dt;
				if (DateTime.TryParse(tmp, out dt)) config.DrTarget.StartTime = dt;
			}
			c.GetString(ref tmp);
			if (!string.IsNullOrEmpty(tmp))
			{
				DateTime dt;
				if (DateTime.TryParse(tmp, out dt)) config.DrTarget.EndTime = dt;
			}

			// 통신상태 DI 태그 (v2 추가 - 하위 호환: 필드 없으면 빈 문자열)
			c.GetString(ref tmp); config.CommStatusTagName = tmp;

			return config;
		}

		public static string FormatConfigLine(DemandNewConfig config)
		{
			var c = new CommaBlockString();

			c.AddString(config.BlockId);
			c.AddString(config.Title);
			c.AddInt((int)config.Mode);
			c.AddInt(config.IntervalMinutes);
			c.AddInt(config.ClockAligned ? 1 : 0);

			c.AddDouble(config.ContractKW);
			c.AddDouble(config.SafetyFactor);

			c.AddString(config.MeterTagName);
			c.AddInt((int)config.MeterType);
			c.AddDouble(config.PulseRatio);
			c.AddString(config.TargetTagName);
			c.AddInt(config.UseTargetTag ? 1 : 0);
			c.AddInt(config.UseEOI ? 1 : 0);
			c.AddString(config.EOITagName);
			c.AddInt(config.InputAutoReset ? 1 : 0);
			c.AddString(config.PredictionDisplayTagName);

			c.AddDouble(config.ShedMarginKW);
			c.AddDouble(config.RestoreMarginKW);
			c.AddInt(config.ProtectionTimeSec);
			c.AddInt(config.MultiStepEnabled ? 1 : 0);
			c.AddInt(config.StepCount);
			c.AddInt(config.QualityBadBlockControl ? 1 : 0);

			c.AddInt(config.TrendWindowSec);
			c.AddDouble(config.EwmaAlpha);

			c.AddInt(config.DatabaseSaveEnabled ? 1 : 0);
			c.AddString(config.DatabaseDsn);
			c.AddInt(config.UsePostgres ? 1 : 0);
			c.AddInt(config.FailoverEnabled ? 1 : 0);

			c.AddInt(config.PercentY);

			// 시간대 목표
			c.AddInt(config.TimeZoneTargets.Count);
			foreach (var tz in config.TimeZoneTargets)
			{
				c.AddInt((int)tz.Zone);
				c.AddInt((int)tz.StartTime.TotalMinutes);
				c.AddInt((int)tz.EndTime.TotalMinutes);
				c.AddDouble(tz.TargetKW);
			}

			// 다단계 제어 Steps
			c.AddInt(config.Steps.Count);
			foreach (var step in config.Steps)
			{
				c.AddDouble(step.ReductionKW);
				c.AddInt(step.LoadIds.Count);
				foreach (string loadId in step.LoadIds)
				{
					c.AddString(loadId);
				}
			}

			// DR(수요반응) 목표
			c.AddInt(config.DrTarget.IsEnabled ? 1 : 0);
			c.AddDouble(config.DrTarget.TargetKW);
			c.AddString(config.DrTarget.StartTime.ToString("o"));
			c.AddString(config.DrTarget.EndTime.ToString("o"));

			// 통신상태 DI 태그 (v2 추가)
			c.AddString(config.CommStatusTagName);

			return c.Get();
		}

		private static List<LoadModel> LoadLoads(string blockId)
		{
			var loads = new List<LoadModel>();
			string path = GetLoadsFilePath(blockId);

			if (!File.Exists(path)) return loads;

			try
			{
				using (var reader = new StreamReader(path, Encoding.UTF8))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						line = line.Trim();
						if (string.IsNullOrEmpty(line) || line.StartsWith("//")) continue;

						var load = ParseLoadLine(line);
						if (load != null) loads.Add(load);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Write(LogLevel.ERROR, LogCategory.DATA_SAVE, "DemandNewConfigLoader.LoadLoads: {0}", ex.Message);
			}

			return loads;
		}

		public static void SaveLoads(string blockId, List<LoadModel> loads)
		{
			string path = GetLoadsFilePath(blockId);
			string dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

			try
			{
				using (var writer = new StreamWriter(path, false, Encoding.UTF8))
				{
					foreach (var load in loads)
					{
						writer.WriteLine(FormatLoadLine(load));
					}
				}
			}
			catch (Exception ex)
			{
				Log.Write(LogLevel.ERROR, LogCategory.DATA_SAVE, "DemandNewConfigLoader.SaveLoads: {0}", ex.Message);
			}
		}

		private static LoadModel ParseLoadLine(string line)
		{
			var c = new CommaBlockString();
			c.Set(line);

			var load = new LoadModel();
			string tmp = "";

			c.GetString(ref tmp); load.LoadId = tmp;
			c.GetString(ref tmp); load.DisplayName = tmp;
			int priority = 1; c.GetInt(ref priority); load.Priority = priority;
			c.GetString(ref tmp); load.Group = tmp;
			double d = 0; c.GetDouble(ref d); load.EstimatedKW = d;
			c.GetString(ref tmp); load.CommandTagName = tmp;
			int tagType = 0; c.GetInt(ref tagType); load.CommandTagType = (sbyte)tagType;
			c.GetString(ref tmp); load.FeedbackTagName = tmp;
			int minOff = 300; c.GetInt(ref minOff); load.MinOffTimeSec = minOff;
			int minOn = 300; c.GetInt(ref minOn); load.MinOnTimeSec = minOn;
			int reShed = 0; c.GetInt(ref reShed); load.ReShedBlockEnabled = (reShed == 1);
			int reShedTime = 600; c.GetInt(ref reShedTime); load.ReShedBlockTimeSec = reShedTime;
			c.GetString(ref tmp); load.InterlockTagName = tmp;

			return load;
		}

		private static string FormatLoadLine(LoadModel load)
		{
			var c = new CommaBlockString();

			c.AddString(load.LoadId);
			c.AddString(load.DisplayName);
			c.AddInt(load.Priority);
			c.AddString(load.Group);
			c.AddDouble(load.EstimatedKW);
			c.AddString(load.CommandTagName);
			c.AddInt(load.CommandTagType);
			c.AddString(load.FeedbackTagName);
			c.AddInt(load.MinOffTimeSec);
			c.AddInt(load.MinOnTimeSec);
			c.AddInt(load.ReShedBlockEnabled ? 1 : 0);
			c.AddInt(load.ReShedBlockTimeSec);
			c.AddString(load.InterlockTagName);

			return c.Get();
		}
	}
}

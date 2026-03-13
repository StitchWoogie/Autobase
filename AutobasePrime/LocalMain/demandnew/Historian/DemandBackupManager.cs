using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using NetTools;

namespace LocalMain.DemandNew
{
	public class DemandBackupManager
	{
		private readonly string _blockId;
		private readonly string _backupPath;
		private readonly object _fileLock = new object();
		private const int MAX_BACKUP_FILES = 1000;

		public DemandBackupManager(string blockId)
		{
			_blockId = blockId;
			_backupPath = Path.Combine(TotalConfig.sDirWorkProject, "Data", "Backup", "DemandData", blockId);

			if (!Directory.Exists(_backupPath))
				Directory.CreateDirectory(_backupPath);
		}

		/// <summary>
		/// 구간 완료 데이터를 날짜별 JSONL 파일에 추가
		/// 파일명: YYYYMMDD_interval.jsonl
		/// </summary>
		public void BackupInterval(DemandSnapshot snapshot)
		{
			try
			{
				string json = FormatIntervalJson(snapshot);
				string filename = String.Format("{0:yyyyMMdd}_interval.jsonl", DateTime.Now);
				string filepath = Path.Combine(_backupPath, filename);

				lock (_fileLock)
				{
					File.AppendAllText(filepath, json + Environment.NewLine, Encoding.UTF8);
				}

				CleanupOldFiles();
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					"DemandBackupManager.BackupInterval", ex.Message);
			}
		}

		/// <summary>
		/// 제어 이벤트를 날짜별 JSONL 파일에 추가
		/// 파일명: YYYYMMDD_event.jsonl
		/// </summary>
		public void BackupControlEvent(ControlEventRecord record)
		{
			try
			{
				string json = FormatControlEventJson(record);
				string filename = String.Format("{0:yyyyMMdd}_event.jsonl", DateTime.Now);
				string filepath = Path.Combine(_backupPath, filename);

				lock (_fileLock)
				{
					File.AppendAllText(filepath, json + Environment.NewLine, Encoding.UTF8);
				}

				CleanupOldFiles();
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					"DemandBackupManager.BackupControlEvent", ex.Message);
			}
		}

		public void RecoverIntervals(Func<DemandSnapshot, bool> insertFunc)
		{
			if (!Directory.Exists(_backupPath)) return;

			string[] files = Directory.GetFiles(_backupPath, "*_interval.jsonl");
			foreach (string file in files)
			{
				try
				{
					string[] lines;
					lock (_fileLock)
					{
						lines = File.ReadAllLines(file, Encoding.UTF8);
					}

					bool allInserted = true;
					foreach (string line in lines)
					{
						if (string.IsNullOrWhiteSpace(line)) continue;
						DemandSnapshot snapshot = ParseIntervalJson(line);
						if (snapshot != null)
						{
							if (!insertFunc(snapshot))
								allInserted = false;
						}
					}

					if (allInserted)
					{
						lock (_fileLock) { File.Delete(file); }
					}
				}
				catch (Exception ex)
				{
					SmLog.LogError(LogCategory.DATA_SAVE,
						"DemandBackupManager.RecoverIntervals", ex.Message);
				}
			}
		}

		public void RecoverControlEvents(Func<ControlEventRecord, bool> insertFunc)
		{
			if (!Directory.Exists(_backupPath)) return;

			string[] files = Directory.GetFiles(_backupPath, "*_event.jsonl");
			foreach (string file in files)
			{
				try
				{
					string[] lines;
					lock (_fileLock)
					{
						lines = File.ReadAllLines(file, Encoding.UTF8);
					}

					bool allInserted = true;
					foreach (string line in lines)
					{
						if (string.IsNullOrWhiteSpace(line)) continue;
						ControlEventRecord record = ParseControlEventJson(line);
						if (!insertFunc(record))
							allInserted = false;
					}

					if (allInserted)
					{
						lock (_fileLock) { File.Delete(file); }
					}
				}
				catch (Exception ex)
				{
					SmLog.LogError(LogCategory.DATA_SAVE,
						"DemandBackupManager.RecoverControlEvents", ex.Message);
				}
			}
		}

		private void CleanupOldFiles()
		{
			try
			{
				string[] files = Directory.GetFiles(_backupPath, "*.jsonl");
				if (files.Length > MAX_BACKUP_FILES)
				{
					Array.Sort(files);
					int deleteCount = files.Length - MAX_BACKUP_FILES;
					for (int i = 0; i < deleteCount; i++)
					{
						lock (_fileLock) { File.Delete(files[i]); }
					}
				}
			}
			catch { }
		}

		// 간이 JSON 직렬화 (.NET 4.8 호환, System.Text.Json 미사용)
		private string FormatIntervalJson(DemandSnapshot s)
		{
			return String.Format(
				"{{\"block_id\":\"{0}\",\"interval_start\":\"{1:o}\",\"interval_end\":\"{2:o}\"," +
				"\"interval_min\":{3},\"demand_kw\":{4},\"forecast_kw\":{5}," +
				"\"target_kw\":{6},\"target_reason\":{7},\"peak_kw\":{8},\"engine_mode\":{9}}}",
				EscapeJson(s.BlockId), s.IntervalStart, s.Timestamp,
				s.IntervalMinutes, s.BlockDemandKW, s.ForecastDemandEndKW,
				s.EffectiveTargetKW, (int)s.TargetReason, s.BlockDemandKW, (int)s.Mode);
		}

		private string FormatControlEventJson(ControlEventRecord r)
		{
			return String.Format(
				"{{\"block_id\":\"{0}\",\"event_time\":\"{1:o}\",\"decision\":{2}," +
				"\"step\":{3},\"load_id\":\"{4}\",\"load_name\":\"{5}\"," +
				"\"estimated_kw\":{6},\"forecast_kw\":{7},\"target_kw\":{8}," +
				"\"result\":{9},\"engine_mode\":{10}}}",
				EscapeJson(r.BlockId), r.EventTime, (int)r.Decision,
				r.Step, EscapeJson(r.LoadId), EscapeJson(r.LoadName),
				r.EstimatedKW, r.ForecastKW, r.TargetKW,
				(int)r.Result, (int)r.EngineMode);
		}

		private DemandSnapshot ParseIntervalJson(string json)
		{
			try
			{
				// 간이 파서
				var s = new DemandSnapshot();
				s.BlockId = ExtractJsonString(json, "block_id");
				s.IntervalStart = DateTime.Parse(ExtractJsonString(json, "interval_start"));
				s.Timestamp = DateTime.Parse(ExtractJsonString(json, "interval_end"));
				s.IntervalMinutes = ExtractJsonInt(json, "interval_min");
				s.BlockDemandKW = ExtractJsonDouble(json, "demand_kw");
				s.ForecastDemandEndKW = ExtractJsonDouble(json, "forecast_kw");
				s.EffectiveTargetKW = ExtractJsonDouble(json, "target_kw");
				s.TargetReason = (TargetReason)ExtractJsonInt(json, "target_reason");
				s.Mode = (EngineMode)ExtractJsonInt(json, "engine_mode");
				return s;
			}
			catch { return null; }
		}

		private ControlEventRecord ParseControlEventJson(string json)
		{
			var r = new ControlEventRecord();
			r.BlockId = ExtractJsonString(json, "block_id");
			r.EventTime = DateTime.Parse(ExtractJsonString(json, "event_time"));
			r.Decision = (ControlDecision)ExtractJsonInt(json, "decision");
			r.Step = ExtractJsonInt(json, "step");
			r.LoadId = ExtractJsonString(json, "load_id");
			r.LoadName = ExtractJsonString(json, "load_name");
			r.EstimatedKW = ExtractJsonDouble(json, "estimated_kw");
			r.ForecastKW = ExtractJsonDouble(json, "forecast_kw");
			r.TargetKW = ExtractJsonDouble(json, "target_kw");
			r.Result = (ActuationResult)ExtractJsonInt(json, "result");
			r.EngineMode = (EngineMode)ExtractJsonInt(json, "engine_mode");
			return r;
		}

		private static string EscapeJson(string s)
		{
			if (s == null) return "";
			return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

		private static string ExtractJsonString(string json, string key)
		{
			string search = "\"" + key + "\":\"";
			int start = json.IndexOf(search);
			if (start < 0) return "";
			start += search.Length;
			int end = json.IndexOf("\"", start);
			if (end < 0) return "";
			return json.Substring(start, end - start);
		}

		private static int ExtractJsonInt(string json, string key)
		{
			string search = "\"" + key + "\":";
			int start = json.IndexOf(search);
			if (start < 0) return 0;
			start += search.Length;
			int end = start;
			while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '-'))
				end++;
			int result;
			int.TryParse(json.Substring(start, end - start), out result);
			return result;
		}

		private static double ExtractJsonDouble(string json, string key)
		{
			string search = "\"" + key + "\":";
			int start = json.IndexOf(search);
			if (start < 0) return 0;
			start += search.Length;
			int end = start;
			while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '.' || json[end] == '-' || json[end] == 'E' || json[end] == 'e' || json[end] == '+'))
				end++;
			double result;
			double.TryParse(json.Substring(start, end - start),
				System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result);
			return result;
		}
	}
}

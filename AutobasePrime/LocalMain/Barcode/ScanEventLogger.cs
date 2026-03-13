using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain.Barcode
{
	/// <summary>
	/// 스캔 이벤트 로거.
	/// 모든 바코드 스캔 이벤트를 CSV 파일로 기록하여 추적성(Traceability)을 보장한다.
	///
	/// 산업 환경에서 바코드 스캔 이력은 품질 관리, 감사(Audit),
	/// 리콜 추적, 규정 준수를 위해 필수적이다.
	///
	/// 로그 파일 위치: {프로젝트폴더}\BarcodeLog\ScanLog_YYYYMMDD.csv
	/// </summary>
	public class ScanEventLogger : IDisposable
	{
		private readonly string _logDirectory;
		private readonly int _retentionDays;
		private readonly ConcurrentQueue<ScanLogRecord> _queue = new ConcurrentQueue<ScanLogRecord>();
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly AutoResetEvent _signal = new AutoResetEvent(false);
		private Task _writerTask;
		private bool _disposed;

		public ScanEventLogger(string logDirectory, int retentionDays = 90)
		{
			_logDirectory = logDirectory;
			_retentionDays = retentionDays;
		}

		/// <summary>
		/// 로거를 시작한다.
		/// </summary>
		public void Start()
		{
			try
			{
				if (!Directory.Exists(_logDirectory))
					Directory.CreateDirectory(_logDirectory);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[ScanEventLogger] Failed to create log dir: {ex.Message}");
			}

			_writerTask = Task.Run(() => WriterLoop(_cts.Token));

			// 오래된 로그 정리 (비동기)
			if (_retentionDays > 0)
			{
				Task.Run(() => CleanupOldLogs());
			}
		}

		/// <summary>
		/// 스캔 로그 레코드를 큐에 추가한다 (비차단).
		/// </summary>
		public void Log(ScanLogRecord record)
		{
			if (record == null) return;
			_queue.Enqueue(record);
			_signal.Set();
		}

		/// <summary>
		/// BarcodeScannedEventArgs에서 로그 레코드를 생성하여 큐에 추가한다.
		/// </summary>
		public void Log(BarcodeScannedEventArgs e, string operatorName,
			string resultTagName = null, string resultTagValue = null,
			string scriptExecuted = null)
		{
			Log(new ScanLogRecord
			{
				Timestamp = e.Timestamp,
				Operator = operatorName ?? "",
				BarcodeValue = e.CleanCode ?? e.RawCode,
				ValidationResult = e.ValidationResult,
				ScannerId = e.ScannerId ?? "",
				SourceType = e.SourceType,
				ResultTagName = resultTagName ?? "",
				ResultTagValue = resultTagValue ?? "",
				ScriptExecuted = scriptExecuted ?? "",
			});
		}

		/// <summary>
		/// 특정 날짜의 스캔 로그를 읽는다.
		/// </summary>
		public List<ScanLogRecord> ReadLogs(DateTime date)
		{
			var records = new List<ScanLogRecord>();
			string filePath = GetLogFilePath(date);

			if (!File.Exists(filePath))
				return records;

			try
			{
				using (var reader = new StreamReader(filePath, Encoding.UTF8))
				{
					string header = reader.ReadLine(); // 헤더 건너뛰기
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						var record = ParseCsvLine(line);
						if (record != null)
							records.Add(record);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[ScanEventLogger] Read error: {ex.Message}");
			}

			return records;
		}

		#region Writer Loop

		private void WriterLoop(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				try
				{
					_signal.WaitOne(5000); // 5초마다 또는 신호 시 깨어남

					FlushQueue();
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"[ScanEventLogger] Writer error: {ex.Message}");
				}
			}

			// 종료 전 남은 레코드 플러시
			FlushQueue();
		}

		private void FlushQueue()
		{
			var byDate = new Dictionary<string, List<ScanLogRecord>>();

			while (_queue.TryDequeue(out var record))
			{
				string dateKey = record.Timestamp.ToString("yyyyMMdd");
				if (!byDate.ContainsKey(dateKey))
					byDate[dateKey] = new List<ScanLogRecord>();
				byDate[dateKey].Add(record);
			}

			foreach (var kvp in byDate)
			{
				try
				{
					string filePath = Path.Combine(_logDirectory, $"ScanLog_{kvp.Key}.csv");
					bool isNew = !File.Exists(filePath);

					using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
					{
						if (isNew)
						{
							writer.WriteLine("Timestamp,Operator,BarcodeValue,ValidationResult,ScannerId,SourceType,ResultTagName,ResultTagValue,ScriptExecuted");
						}

						foreach (var record in kvp.Value)
						{
							writer.WriteLine(FormatCsvLine(record));
						}
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"[ScanEventLogger] Write error for {kvp.Key}: {ex.Message}");
				}
			}
		}

		#endregion

		#region CSV 포맷

		private static string FormatCsvLine(ScanLogRecord r)
		{
			return string.Join(",",
				CsvEscape(r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")),
				CsvEscape(r.Operator),
				CsvEscape(r.BarcodeValue),
				r.ValidationResult.ToString(),
				CsvEscape(r.ScannerId),
				r.SourceType.ToString(),
				CsvEscape(r.ResultTagName),
				CsvEscape(r.ResultTagValue),
				CsvEscape(r.ScriptExecuted)
			);
		}

		private static string CsvEscape(string value)
		{
			if (string.IsNullOrEmpty(value)) return "";
			if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
			{
				return "\"" + value.Replace("\"", "\"\"") + "\"";
			}
			return value;
		}

		private static ScanLogRecord ParseCsvLine(string line)
		{
			if (string.IsNullOrEmpty(line)) return null;

			try
			{
				string[] fields = SplitCsvLine(line);
				if (fields.Length < 9) return null;

				return new ScanLogRecord
				{
					Timestamp = DateTime.Parse(fields[0]),
					Operator = fields[1],
					BarcodeValue = fields[2],
					ValidationResult = (ScanValidationResult)Enum.Parse(typeof(ScanValidationResult), fields[3]),
					ScannerId = fields[4],
					SourceType = (ScannerInputType)Enum.Parse(typeof(ScannerInputType), fields[5]),
					ResultTagName = fields[6],
					ResultTagValue = fields[7],
					ScriptExecuted = fields[8],
				};
			}
			catch
			{
				return null;
			}
		}

		private static string[] SplitCsvLine(string line)
		{
			var fields = new List<string>();
			var sb = new StringBuilder();
			bool inQuotes = false;

			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];
				if (inQuotes)
				{
					if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
					{
						sb.Append('"');
						i++;
					}
					else if (c == '"')
					{
						inQuotes = false;
					}
					else
					{
						sb.Append(c);
					}
				}
				else
				{
					if (c == '"')
					{
						inQuotes = true;
					}
					else if (c == ',')
					{
						fields.Add(sb.ToString());
						sb.Clear();
					}
					else
					{
						sb.Append(c);
					}
				}
			}
			fields.Add(sb.ToString());
			return fields.ToArray();
		}

		#endregion

		#region 오래된 로그 정리

		private void CleanupOldLogs()
		{
			try
			{
				if (!Directory.Exists(_logDirectory)) return;

				DateTime cutoff = DateTime.Today.AddDays(-_retentionDays);
				string[] files = Directory.GetFiles(_logDirectory, "ScanLog_*.csv");

				foreach (string file in files)
				{
					try
					{
						string fileName = Path.GetFileNameWithoutExtension(file);
						string datePart = fileName.Replace("ScanLog_", "");
						if (DateTime.TryParseExact(datePart, "yyyyMMdd", null,
							System.Globalization.DateTimeStyles.None, out DateTime fileDate))
						{
							if (fileDate < cutoff)
							{
								File.Delete(file);
								Debug.WriteLine($"[ScanEventLogger] Deleted old log: {file}");
							}
						}
					}
					catch { }
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[ScanEventLogger] Cleanup error: {ex.Message}");
			}
		}

		#endregion

		private string GetLogFilePath(DateTime date)
		{
			return Path.Combine(_logDirectory, $"ScanLog_{date:yyyyMMdd}.csv");
		}

		public void Dispose()
		{
			if (_disposed) return;
			_disposed = true;

			_cts.Cancel();
			_signal.Set();

			try { _writerTask?.Wait(3000); } catch { }

			_cts.Dispose();
			_signal.Dispose();
		}
	}
}

using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoLibLocal;
using NetTools;

namespace LocalMain.DemandNew
{
	public class DemandCsvExporter
	{
		private readonly string _dsnName;

		public DemandCsvExporter(string dsnName)
		{
			_dsnName = dsnName;
		}

		/// <summary>
		/// demand_interval 테이블을 CSV로 내보내기
		/// </summary>
		public bool ExportIntervals(string blockId, DateTime from, DateTime to, string outputPath)
		{
			return ExportTable(
				"demand_interval",
				"block_id, interval_start, interval_end, interval_min, demand_kw, forecast_kw, target_kw, target_reason, peak_kw, engine_mode",
				blockId, from, to, "interval_start", outputPath);
		}

		/// <summary>
		/// demand_control_event 테이블을 CSV로 내보내기
		/// </summary>
		public bool ExportControlEvents(string blockId, DateTime from, DateTime to, string outputPath)
		{
			return ExportTable(
				"demand_control_event",
				"block_id, event_time, decision, step, load_id, load_name, estimated_kw, forecast_kw, target_kw, result, engine_mode",
				blockId, from, to, "event_time", outputPath);
		}

		/// <summary>
		/// demand_monthly_peak 테이블을 CSV로 내보내기
		/// </summary>
		public bool ExportMonthlyPeaks(string blockId, string outputPath)
		{
			ConnectionString dsn = DbTool.dsnList.GetConnection(_dsnName);
			if (dsn == null) return false;

			CommonDbConnection conn = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);

			try
			{
				conn.Open();
				if (conn.State != ConnectionState.Open) return false;

				string query = "SELECT block_id, year_month, peak_kw, peak_time, contract_kw, updated_at " +
					"FROM demand_monthly_peak WHERE block_id = @block_id ORDER BY year_month";

				CommonDbCommand cmd = new CommonDbCommand(dsn.dbConnectionType);
				cmd.Connection = conn;
				cmd.CommandText = query;
				cmd.AddParameter("@block_id", blockId);

				string dir = Path.GetDirectoryName(outputPath);
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

				using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
				{
					writer.WriteLine("block_id,year_month,peak_kw,peak_time,contract_kw,updated_at");

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							writer.WriteLine("{0},{1},{2},{3},{4},{5}",
								CsvEscape(reader[0]),
								CsvEscape(reader[1]),
								reader[2],
								CsvEscape(reader[3]),
								reader[4],
								CsvEscape(reader[5]));
						}
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					"DemandCsvExporter.ExportMonthlyPeaks: " + ex.Message);
				return false;
			}
			finally
			{
				try { conn.Close(); } catch { }
			}
		}

		private bool ExportTable(string tableName, string columns, string blockId,
			DateTime from, DateTime to, string timeColumn, string outputPath)
		{
			ConnectionString dsn = DbTool.dsnList.GetConnection(_dsnName);
			if (dsn == null) return false;

			CommonDbConnection conn = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);

			try
			{
				conn.Open();
				if (conn.State != ConnectionState.Open) return false;

				string query = String.Format(
					"SELECT {0} FROM {1} WHERE block_id = @block_id AND {2} >= @from AND {2} <= @to ORDER BY {2}",
					columns, tableName, timeColumn);

				CommonDbCommand cmd = new CommonDbCommand(dsn.dbConnectionType);
				cmd.Connection = conn;
				cmd.CommandText = query;
				cmd.AddParameter("@block_id", blockId);
				cmd.AddParameter("@from", from);
				cmd.AddParameter("@to", to);

				string dir = Path.GetDirectoryName(outputPath);
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

				using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
				{
					// 헤더
					writer.WriteLine(columns.Replace(" ", ""));

					using (var reader = cmd.ExecuteReader())
					{
						int fieldCount = reader.FieldCount;
						while (reader.Read())
						{
							var sb = new StringBuilder();
							for (int i = 0; i < fieldCount; i++)
							{
								if (i > 0) sb.Append(",");
								sb.Append(CsvEscape(reader[i]));
							}
							writer.WriteLine(sb.ToString());
						}
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					String.Format("DemandCsvExporter.ExportTable[{0}] {1}", tableName, ex.Message));
				return false;
			}
			finally
			{
				try { conn.Close(); } catch { }
			}
		}

		#region 비동기 래퍼 (대형 현장에서 UI thread 멈춤 방지)

		/// <summary>
		/// 백그라운드 Thread에서 demand_interval CSV 내보내기
		/// </summary>
		public void ExportIntervalsAsync(string blockId, DateTime from, DateTime to, string outputPath,
			Action<bool, string> callback)
		{
			Task.Run(() =>
			{
				try
				{
					bool ok = ExportIntervals(blockId, from, to, outputPath);
					callback?.Invoke(ok, ok ? outputPath : "Export failed");
				}
				catch (Exception ex)
				{
					callback?.Invoke(false, ex.Message);
				}
			});
		}

		/// <summary>
		/// 백그라운드 Thread에서 demand_control_event CSV 내보내기
		/// </summary>
		public void ExportControlEventsAsync(string blockId, DateTime from, DateTime to, string outputPath,
			Action<bool, string> callback)
		{
			Task.Run(() =>
			{
				try
				{
					bool ok = ExportControlEvents(blockId, from, to, outputPath);
					callback?.Invoke(ok, ok ? outputPath : "Export failed");
				}
				catch (Exception ex)
				{
					callback?.Invoke(false, ex.Message);
				}
			});
		}

		/// <summary>
		/// 백그라운드 Thread에서 demand_monthly_peak CSV 내보내기
		/// </summary>
		public void ExportMonthlyPeaksAsync(string blockId, string outputPath,
			Action<bool, string> callback)
		{
			Task.Run(() =>
			{
				try
				{
					bool ok = ExportMonthlyPeaks(blockId, outputPath);
					callback?.Invoke(ok, ok ? outputPath : "Export failed");
				}
				catch (Exception ex)
				{
					callback?.Invoke(false, ex.Message);
				}
			});
		}

		#endregion

		private static string CsvEscape(object value)
		{
			if (value == null || value == DBNull.Value)
				return "";

			string str = value.ToString();
			if (str.Contains(",") || str.Contains("\"") || str.Contains("\n"))
				return "\"" + str.Replace("\"", "\"\"") + "\"";

			return str;
		}
	}
}

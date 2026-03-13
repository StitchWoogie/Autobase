using System;
using System.Data;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using NetTools;

namespace LocalMain.DemandNew
{
	public struct ControlEventRecord
	{
		public string BlockId;
		public DateTime EventTime;
		public ControlDecision Decision;
		public int Step;
		public string LoadId;
		public string LoadName;
		public double EstimatedKW;
		public double ForecastKW;
		public double TargetKW;
		public ActuationResult Result;
		public EngineMode EngineMode;
	}

	public class DemandHistorian
	{
		private readonly DemandNewConfig _config;
		private readonly DemandBackupManager _backup;
		private bool _tablesChecked;

		public DemandHistorian(DemandNewConfig config)
		{
			_config = config;
			_backup = new DemandBackupManager(config.BlockId);
		}

		public void OnIntervalComplete(DemandSnapshot snapshot)
		{
			if (!_config.DatabaseSaveEnabled) return;

			if (!TryInsertInterval(snapshot))
			{
				if (_config.FailoverEnabled)
					_backup.BackupInterval(snapshot);
			}

			TryUpdateMonthlyPeak(snapshot);
		}

		public void OnControlEvent(ControlEventRecord record)
		{
			if (!_config.DatabaseSaveEnabled) return;

			if (!TryInsertControlEvent(record))
			{
				if (_config.FailoverEnabled)
					_backup.BackupControlEvent(record);
			}
		}

		public void RecoverFromBackups()
		{
			_backup.RecoverIntervals((snapshot) => TryInsertInterval(snapshot));
			_backup.RecoverControlEvents((record) => TryInsertControlEvent(record));
		}

		private bool TryInsertInterval(DemandSnapshot snapshot)
		{
			ConnectionString dsn = DbTool.dsnList.GetConnection(_config.DatabaseDsn);
			if (dsn == null) return false;

			CommonDbConnection conn = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);

			try
			{
				conn.Open();
				if (conn.State != ConnectionState.Open) return false;

				if (!_tablesChecked)
				{
					if (!EnsureIntervalTable(conn, dsn)) { conn.Close(); return false; }
					if (!EnsureControlEventTable(conn, dsn)) { conn.Close(); return false; }
					if (!EnsureMonthlyPeakTable(conn, dsn)) { conn.Close(); return false; }
					_tablesChecked = true;
				}

				string query = "INSERT INTO demand_interval " +
					"(block_id, interval_start, interval_end, interval_min, demand_kw, forecast_kw, target_kw, target_reason, peak_kw, engine_mode) " +
					"VALUES (@block_id, @interval_start, @interval_end, @interval_min, @demand_kw, @forecast_kw, @target_kw, @target_reason, @peak_kw, @engine_mode)";

				CommonDbCommand cmd = new CommonDbCommand(dsn.dbConnectionType);
				cmd.Connection = conn;
				cmd.CommandText = query;

				cmd.AddParameter("@block_id", snapshot.BlockId);
				cmd.AddParameter("@interval_start", snapshot.IntervalStart);
				cmd.AddParameter("@interval_end", snapshot.Timestamp);
				cmd.AddParameter("@interval_min", snapshot.IntervalMinutes);
				cmd.AddParameter("@demand_kw", snapshot.BlockDemandKW);
				cmd.AddParameter("@forecast_kw", snapshot.ForecastDemandEndKW);
				cmd.AddParameter("@target_kw", snapshot.EffectiveTargetKW);
				cmd.AddParameter("@target_reason", (int)snapshot.TargetReason);
				cmd.AddParameter("@peak_kw", snapshot.BlockDemandKW);
				cmd.AddParameter("@engine_mode", (int)snapshot.Mode);

				cmd.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					String.Format("DemandHistorian.InsertInterval[{0}] {1}", _config.BlockId, ex.Message));
				return false;
			}
			finally
			{
				try { conn.Close(); } catch { }
			}
		}

		private bool TryInsertControlEvent(ControlEventRecord record)
		{
			ConnectionString dsn = DbTool.dsnList.GetConnection(_config.DatabaseDsn);
			if (dsn == null) return false;

			CommonDbConnection conn = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);

			try
			{
				conn.Open();
				if (conn.State != ConnectionState.Open) return false;

				string query = "INSERT INTO demand_control_event " +
					"(block_id, event_time, decision, step, load_id, load_name, estimated_kw, forecast_kw, target_kw, result, engine_mode) " +
					"VALUES (@block_id, @event_time, @decision, @step, @load_id, @load_name, @estimated_kw, @forecast_kw, @target_kw, @result, @engine_mode)";

				CommonDbCommand cmd = new CommonDbCommand(dsn.dbConnectionType);
				cmd.Connection = conn;
				cmd.CommandText = query;

				cmd.AddParameter("@block_id", record.BlockId);
				cmd.AddParameter("@event_time", record.EventTime);
				cmd.AddParameter("@decision", (int)record.Decision);
				cmd.AddParameter("@step", record.Step);
				cmd.AddParameter("@load_id", record.LoadId ?? "");
				cmd.AddParameter("@load_name", record.LoadName ?? "");
				cmd.AddParameter("@estimated_kw", record.EstimatedKW);
				cmd.AddParameter("@forecast_kw", record.ForecastKW);
				cmd.AddParameter("@target_kw", record.TargetKW);
				cmd.AddParameter("@result", (int)record.Result);
				cmd.AddParameter("@engine_mode", (int)record.EngineMode);

				cmd.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					String.Format("DemandHistorian.InsertControlEvent[{0}] {1}", _config.BlockId, ex.Message));
				return false;
			}
			finally
			{
				try { conn.Close(); } catch { }
			}
		}

		private void TryUpdateMonthlyPeak(DemandSnapshot snapshot)
		{
			ConnectionString dsn = DbTool.dsnList.GetConnection(_config.DatabaseDsn);
			if (dsn == null) return;

			CommonDbConnection conn = new CommonDbConnection(dsn.dbConnectionType, dsn.dsn, dsn.bAddCommitAfterCommand);

			try
			{
				conn.Open();
				if (conn.State != ConnectionState.Open) return;

				string yearMonth = snapshot.Timestamp.ToString("yyyy-MM");

				// 기존 피크 조회
				string selectQuery = "SELECT peak_kw FROM demand_monthly_peak WHERE block_id = @block_id AND year_month = @year_month";
				CommonDbCommand selectCmd = new CommonDbCommand(dsn.dbConnectionType);
				selectCmd.Connection = conn;
				selectCmd.CommandText = selectQuery;
				selectCmd.AddParameter("@block_id", snapshot.BlockId);
				selectCmd.AddParameter("@year_month", yearMonth);

				object existing = selectCmd.ExecuteScalar();
				double existingPeak = (existing != null && existing != DBNull.Value)
					? Convert.ToDouble(existing) : 0;

				if (snapshot.BlockDemandKW > existingPeak)
				{
					string upsertQuery;
					if (existing != null && existing != DBNull.Value)
					{
						upsertQuery = "UPDATE demand_monthly_peak SET peak_kw = @peak_kw, peak_time = @peak_time, contract_kw = @contract_kw, updated_at = @updated_at " +
							"WHERE block_id = @block_id AND year_month = @year_month";
					}
					else
					{
						upsertQuery = "INSERT INTO demand_monthly_peak (block_id, year_month, peak_kw, peak_time, contract_kw, updated_at) " +
							"VALUES (@block_id, @year_month, @peak_kw, @peak_time, @contract_kw, @updated_at)";
					}

					CommonDbCommand upsertCmd = new CommonDbCommand(dsn.dbConnectionType);
					upsertCmd.Connection = conn;
					upsertCmd.CommandText = upsertQuery;
					upsertCmd.AddParameter("@block_id", snapshot.BlockId);
					upsertCmd.AddParameter("@year_month", yearMonth);
					upsertCmd.AddParameter("@peak_kw", snapshot.BlockDemandKW);
					upsertCmd.AddParameter("@peak_time", snapshot.Timestamp);
					upsertCmd.AddParameter("@contract_kw", _config.ContractKW);
					upsertCmd.AddParameter("@updated_at", DateTime.Now);

					upsertCmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				SmLog.LogError(LogCategory.DATA_SAVE,
					String.Format("DemandHistorian.UpdateMonthlyPeak[{0}] {1}", _config.BlockId, ex.Message));
			}
			finally
			{
				try { conn.Close(); } catch { }
			}
		}

		private bool EnsureIntervalTable(CommonDbConnection conn, ConnectionString dsn)
		{
			CheckTable check = new CheckTable();
			check.AddColumn("block_id", EnumDbDataType.String, 64);
			check.AddColumn("interval_start", EnumDbDataType.DateTime, 0);
			check.AddColumn("interval_end", EnumDbDataType.DateTime, 0);
			check.AddColumn("interval_min", EnumDbDataType.Integer, 0);
			check.AddColumn("demand_kw", EnumDbDataType.Float, 0);
			check.AddColumn("forecast_kw", EnumDbDataType.Float, 0);
			check.AddColumn("target_kw", EnumDbDataType.Float, 0);
			check.AddColumn("target_reason", EnumDbDataType.Integer, 0);
			check.AddColumn("peak_kw", EnumDbDataType.Float, 0);
			check.AddColumn("engine_mode", EnumDbDataType.Integer, 0);
			return check.Check(conn, dsn.dbtype, "demand_interval");
		}

		private bool EnsureControlEventTable(CommonDbConnection conn, ConnectionString dsn)
		{
			CheckTable check = new CheckTable();
			check.AddColumn("block_id", EnumDbDataType.String, 64);
			check.AddColumn("event_time", EnumDbDataType.DateTime, 0);
			check.AddColumn("decision", EnumDbDataType.Integer, 0);
			check.AddColumn("step", EnumDbDataType.Integer, 0);
			check.AddColumn("load_id", EnumDbDataType.String, 64);
			check.AddColumn("load_name", EnumDbDataType.String, 128);
			check.AddColumn("estimated_kw", EnumDbDataType.Float, 0);
			check.AddColumn("forecast_kw", EnumDbDataType.Float, 0);
			check.AddColumn("target_kw", EnumDbDataType.Float, 0);
			check.AddColumn("result", EnumDbDataType.Integer, 0);
			check.AddColumn("engine_mode", EnumDbDataType.Integer, 0);
			return check.Check(conn, dsn.dbtype, "demand_control_event");
		}

		private bool EnsureMonthlyPeakTable(CommonDbConnection conn, ConnectionString dsn)
		{
			CheckTable check = new CheckTable();
			check.AddColumn("block_id", EnumDbDataType.String, 64);
			check.AddColumn("year_month", EnumDbDataType.String, 7);
			check.AddColumn("peak_kw", EnumDbDataType.Float, 0);
			check.AddColumn("peak_time", EnumDbDataType.DateTime, 0);
			check.AddColumn("contract_kw", EnumDbDataType.Float, 0);
			check.AddColumn("updated_at", EnumDbDataType.DateTime, 0);
			return check.Check(conn, dsn.dbtype, "demand_monthly_peak");
		}
	}
}

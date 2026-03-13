using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using DialogTag;
using NetTools;

namespace LocalMain.DemandNew
{
	public partial class FormDemandNewSettings : Form
	{
		// 스텝 배열 (InitializeComponent 후 매핑)
		private GroupBox[] grpSteps;
		private NumericUpDown[] numStepReductionKW;
		private CheckedListBox[] clbStepLoads;

		private DemandNewConfig _config;
		private bool _isNew;
		private bool _loadingConfig;

		public FormDemandNewSettings() : this(null, false) { }

		public FormDemandNewSettings(DemandNewConfig config, bool isNew)
		{
			_config = config ?? new DemandNewConfig();
			_isNew = isNew;
			InitializeComponent();

			// 스텝 배열 매핑 (Designer 개별 필드 → 배열)
			grpSteps = new GroupBox[] { grpStep1, grpStep2, grpStep3 };
			numStepReductionKW = new NumericUpDown[] { numStepReductionKW1, numStepReductionKW2, numStepReductionKW3 };
			clbStepLoads = new CheckedListBox[] { clbStepLoads1, clbStepLoads2, clbStepLoads3 };

			// 런타임 초기화 (DSN 로드, 날짜 기본값 등)
			InitializeRuntimeDefaults();

			ApplyModernStyle();
			ApplyLocalization();
			LoadConfigToUI();
		}

		/// <summary>
		/// 런타임 데이터 초기화 (InitializeComponent에 넣을 수 없는 동적 데이터)
		/// </summary>
		private void InitializeRuntimeDefaults()
		{
			// DSN 목록 로드
			try
			{
				for (int i = 0; i < DbTool.dsnList.arrayConnectionString.Count; i++)
				{
					ConnectionString cs = (ConnectionString)DbTool.dsnList.arrayConnectionString[i];
					if (cs != null) cboDsn.Items.Add(cs.title);
				}
			}
			catch { }

			// 날짜 기본값
			dtpFrom.Value = DateTime.Now.AddDays(-7);

			// 내보내기 경로 기본값
			txtExportPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}

		#region Modern Style

		private void ApplyModernStyle()
		{
			DemandUIStyle.StyleForm(this);
			DemandUIStyle.StyleAllControls(this);

			// 부하 탭: Add 버튼 Primary 스타일 적용
			DemandUIStyle.StyleButtonPrimary(btnAddLoad);
		}

		#endregion

		#region Localization

		private void ApplyLocalization()
		{
			if (!Tools.IsLangKorean()) return;

			this.Text = "디맨드제어 설정 (신형)";

			// 탭 제목
			tabControl.TabPages[0].Text = "일반";
			tabControl.TabPages[1].Text = "계약/요금";
			tabControl.TabPages[2].Text = "계측";
			tabControl.TabPages[3].Text = "부하";
			tabControl.TabPages[4].Text = "정책";
			tabControl.TabPages[5].Text = "예측";
			tabControl.TabPages[6].Text = "저장/내보내기";

			// 하단 버튼
			btnOK.Text = "확인";
			btnCancel.Text = "취소";
			btnApply.Text = "적용";

			// 전체 컨트롤 재귀 번역 (Label, CheckBox, RadioButton, Button, GroupBox)
			var map = new System.Collections.Generic.Dictionary<string, string>
			{
				// 일반 탭
				{ "Block ID:", "블록 ID:" },
				{ "Title:", "제목:" },
				{ "Mode:", "모드:" },
				{ "Interval (min):", "수요구간(분):" },
				{ "Shadow", "감시(Shadow)" },
				{ "Active", "제어(Active)" },
				{ "Clock Aligned", "시간 정렬" },

				// 계약/요금 탭
				{ "Contract KW:", "계약전력(kW):" },
				{ "Safety Factor:", "안전계수:" },
				{ "TimeZone Targets:", "시간대 목표:" },

				// 계측 탭
				{ "Meter Tag:", "계측 태그:" },
				{ "Meter Type:", "계측 타입:" },
				{ "Pulse Ratio:", "펄스 비율:" },
				{ "Auto Reset on Pulse Drop", "펄스 감소 시 자동 리셋" },
				{ "Use Target Tag:", "목표 태그 사용:" },
				{ "Use EOI Tag:", "EOI 태그 사용:" },
				{ "Prediction Display:", "예측값 표시 태그:" },
				{ "Comm Status Tag:", "통신상태 태그:" },

				// 부하 탭
				{ "Add", "추가" },
				{ "Edit", "편집" },
				{ "Delete", "삭제" },

				// 정책 탭
				{ "Shed Margin KW:", "차단 여유(kW):" },
				{ "Restore Margin KW:", "복귀 여유(kW):" },
				{ "Protection Time (sec):", "보호시간(초):" },
				{ "Multi-Step Control", "다단계 제어" },
				{ "Step Count:", "단계 수:" },
				{ "Reduction(kW):", "감축량(kW):" },
				{ "Loads:", "부하:" },

				// 예측 탭
				{ "Trend Window (sec):", "트렌드 윈도우(초):" },
				{ "EWMA Alpha:", "EWMA 계수:" },

				// 저장/내보내기 탭
				{ "Enable DB Save", "DB 저장 활성화" },
				{ "DSN:", "DSN:" },
				{ "Enable Failover (JSON backup)", "Failover 활성화 (JSON 백업)" },
				{ "── CSV Export ──", "── CSV 내보내기 ──" },
				{ "From:", "시작:" },
				{ "To:", "종료:" },
				{ "Export Path:", "내보내기 경로:" },
				{ "Export Intervals", "구간 내보내기" },
				{ "Export Events", "이벤트 내보내기" },
				{ "Export Peaks", "피크 내보내기" },
				{ "Recover from Backup", "백업 복구" },
			};
			LocalizeControls(this, map);

			// 스텝 그룹박스
			for (int i = 0; i < 3; i++)
				grpSteps[i].Text = String.Format("단계 {0}", i + 1);

			// DataGridView 컬럼 헤더
			if (gridTimeZone.Columns.Count >= 4)
			{
				gridTimeZone.Columns[0].HeaderText = "영역";
				gridTimeZone.Columns[1].HeaderText = "시작 (HH:MM)";
				gridTimeZone.Columns[2].HeaderText = "종료 (HH:MM)";
				gridTimeZone.Columns[3].HeaderText = "목표(kW)";
			}

			// ListView 폰트 축소 (한국어 헤더 잘림 방지)
			lvLoads.Font = new Font(lvLoads.Font.FontFamily, 8f);

			// ListView 컬럼 헤더 + 한국어 너비 조정
			if (lvLoads.Columns.Count >= 9)
			{
				lvLoads.Columns[0].Text = "ID";          lvLoads.Columns[0].Width = 50;
				lvLoads.Columns[1].Text = "이름";         lvLoads.Columns[1].Width = 80;
				lvLoads.Columns[2].Text = "우선순위";      lvLoads.Columns[2].Width = 75;
				lvLoads.Columns[3].Text = "그룹";         lvLoads.Columns[3].Width = 50;
				lvLoads.Columns[4].Text = "예상(kW)";     lvLoads.Columns[4].Width = 80;
				lvLoads.Columns[5].Text = "제어 태그";     lvLoads.Columns[5].Width = 88;
				lvLoads.Columns[6].Text = "피드백 태그";    lvLoads.Columns[6].Width = 98;
				lvLoads.Columns[7].Text = "최소차단(초)";   lvLoads.Columns[7].Width = 100;
				lvLoads.Columns[8].Text = "최소복귀(초)";   lvLoads.Columns[8].Width = 100;
			}

			// 예측 탭 설명
			lblForecastDesc.Text = "EWMA Alpha: 높을수록 최근 데이터에 민감 (0.1=안정, 0.5=민감)\nTrend Window: 예측에 사용하는 최근 데이터 범위 (초)";
		}

		/// <summary>
		/// 컨트롤 트리를 재귀 순회하며 텍스트 번역 적용
		/// </summary>
		private static void LocalizeControls(Control parent, System.Collections.Generic.Dictionary<string, string> map)
		{
			foreach (Control c in parent.Controls)
			{
				if (c is Label || c is CheckBox || c is RadioButton || c is Button || c is GroupBox)
				{
					string val;
					if (map.TryGetValue(c.Text, out val))
						c.Text = val;
				}
				if (c.HasChildren)
					LocalizeControls(c, map);
			}
		}

		#endregion

		#region UI ↔ Config

		private void LoadConfigToUI()
		{
			_loadingConfig = true;
			FillBlockIdCombo();

			// 블록이 비어있으면 첫 번째 블록 자동 선택 및 로드
			if (string.IsNullOrEmpty(_config.BlockId) && cboBlockId.Items.Count > 0)
			{
				string firstBlock = cboBlockId.Items[0].ToString();
				try
				{
					var configs = DemandNewConfigLoader.LoadAll();
					var found = configs.Find(c => c.BlockId == firstBlock);
					if (found != null)
					{
						_config = found;
						_isNew = false;
					}
				}
				catch { }
			}

			cboBlockId.Text = _config.BlockId;
			txtTitle.Text = _config.Title;
			rdoShadow.Checked = (_config.Mode == EngineMode.Shadow);
			rdoActive.Checked = (_config.Mode == EngineMode.Active);
			cboInterval.SelectedItem = _config.IntervalMinutes;
			if (cboInterval.SelectedIndex < 0) cboInterval.SelectedIndex = 2;  // default 15
			chkClockAligned.Checked = _config.ClockAligned;

			numContractKW.Value = (decimal)_config.ContractKW;
			numSafetyFactor.Value = (decimal)Math.Max(0.50, Math.Min(1.00, _config.SafetyFactor));

			foreach (var tz in _config.TimeZoneTargets)
			{
				gridTimeZone.Rows.Add(tz.Zone.ToString(), tz.StartTime.ToString(@"hh\:mm"),
					tz.EndTime.ToString(@"hh\:mm"), tz.TargetKW);
			}

			txtMeterTag.Text = _config.MeterTagName;
			cboMeterType.SelectedIndex = (int)_config.MeterType;
			numPulseRatio.Value = (decimal)Math.Max(0.001, _config.PulseRatio);
			chkAutoReset.Checked = _config.InputAutoReset;
			chkUseTargetTag.Checked = _config.UseTargetTag;
			txtTargetTag.Text = _config.TargetTagName;
			chkUseEOI.Checked = _config.UseEOI;
			txtEOITag.Text = _config.EOITagName;
			txtPredictionTag.Text = _config.PredictionDisplayTagName;
			txtCommStatusTag.Text = _config.CommStatusTagName;

			RefreshLoadsListView();

			numShedMargin.Value = (decimal)_config.ShedMarginKW;
			numRestoreMargin.Value = (decimal)_config.RestoreMarginKW;
			numProtectionTime.Value = _config.ProtectionTimeSec;
			chkMultiStep.Checked = _config.MultiStepEnabled;
			cboStepCount.SelectedItem = _config.StepCount;
			if (cboStepCount.SelectedIndex < 0) cboStepCount.SelectedIndex = 0;

			// 스텝 상세 로드
			for (int i = 0; i < 3 && i < _config.Steps.Count; i++)
				numStepReductionKW[i].Value = (decimal)_config.Steps[i].ReductionKW;
			RefreshStepLoadLists();
			UpdateStepPanelVisibility();

			trkTrendWindow.Value = Math.Max(30, Math.Min(300, _config.TrendWindowSec));
			trkEwmaAlpha.Value = (int)(_config.EwmaAlpha * 100);
			lblTrendValue.Text = trkTrendWindow.Value.ToString();
			lblAlphaValue.Text = (_config.EwmaAlpha).ToString("F2");

			chkDbSave.Checked = _config.DatabaseSaveEnabled;
			chkFailover.Checked = _config.FailoverEnabled;
			for (int i = 0; i < cboDsn.Items.Count; i++)
			{
				if (cboDsn.Items[i].ToString() == _config.DatabaseDsn)
					cboDsn.SelectedIndex = i;
			}

			_loadingConfig = false;
		}

		private void SaveUIToConfig()
		{
			_config.BlockId = cboBlockId.Text.Trim();
			_config.Title = txtTitle.Text.Trim();
			_config.Mode = rdoActive.Checked ? EngineMode.Active : EngineMode.Shadow;
			_config.IntervalMinutes = (int)cboInterval.SelectedItem;
			_config.ClockAligned = chkClockAligned.Checked;

			_config.ContractKW = (double)numContractKW.Value;
			_config.SafetyFactor = (double)numSafetyFactor.Value;

			_config.TimeZoneTargets.Clear();
			foreach (DataGridViewRow row in gridTimeZone.Rows)
			{
				if (row.IsNewRow) continue;
				try
				{
					var tz = new TimeZoneTarget();
					Enum.TryParse(row.Cells[0].Value?.ToString() ?? "OffPeak", out tz.Zone);
					tz.StartTime = TimeSpan.Parse(row.Cells[1].Value?.ToString() ?? "0:00");
					tz.EndTime = TimeSpan.Parse(row.Cells[2].Value?.ToString() ?? "0:00");
					double tkw = 0;
					double.TryParse(row.Cells[3].Value?.ToString() ?? "0", out tkw);
					tz.TargetKW = tkw;
					_config.TimeZoneTargets.Add(tz);
				}
				catch { }
			}

			_config.MeterTagName = txtMeterTag.Text.Trim();
			_config.MeterType = (MeterType)cboMeterType.SelectedIndex;
			_config.PulseRatio = (double)numPulseRatio.Value;
			_config.InputAutoReset = chkAutoReset.Checked;
			_config.UseTargetTag = chkUseTargetTag.Checked;
			_config.TargetTagName = txtTargetTag.Text.Trim();
			_config.UseEOI = chkUseEOI.Checked;
			_config.EOITagName = txtEOITag.Text.Trim();
			_config.PredictionDisplayTagName = txtPredictionTag.Text.Trim();
			_config.CommStatusTagName = txtCommStatusTag.Text.Trim();

			_config.ShedMarginKW = (double)numShedMargin.Value;
			_config.RestoreMarginKW = (double)numRestoreMargin.Value;
			_config.ProtectionTimeSec = (int)numProtectionTime.Value;
			_config.MultiStepEnabled = chkMultiStep.Checked;
			_config.StepCount = (int)(cboStepCount.SelectedItem ?? 1);
			SaveStepDetails();

			_config.TrendWindowSec = trkTrendWindow.Value;
			_config.EwmaAlpha = trkEwmaAlpha.Value / 100.0;

			_config.DatabaseSaveEnabled = chkDbSave.Checked;
			_config.DatabaseDsn = cboDsn.SelectedItem?.ToString() ?? "";
			_config.FailoverEnabled = chkFailover.Checked;
		}

		#endregion

		#region Event Handlers

		private void BtnOK_Click(object sender, EventArgs e)
		{
			if (ValidateAndSave())
				this.DialogResult = DialogResult.OK;
		}

		private void BtnApply_Click(object sender, EventArgs e)
		{
			ValidateAndSave();
		}

		private bool ValidateAndSave()
		{
			SaveUIToConfig();
			var errors = DemandNewConfigValidator.Validate(_config);
			if (errors.Count > 0)
			{
				MessageBox.Show(string.Join("\n", errors),
					Tools.IsLangKorean() ? "검증 오류" : "Validation Error",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			// Active 모드 전환 확인
			if (_config.Mode == EngineMode.Active)
			{
				string msg = Tools.IsLangKorean()
					? "Active 모드로 설정하면 실제 부하 제어가 수행됩니다. 계속하시겠습니까?"
					: "Active mode will perform actual load control. Continue?";
				if (MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
					return false;
			}

			return SaveConfig();
		}

		/// <summary>
		/// 설정을 파일에 저장하고 엔진을 다시 로드한다.
		/// </summary>
		private bool SaveConfig()
		{
			try
			{
				var configs = DemandNewConfigLoader.LoadAll();
				int idx = configs.FindIndex(c => c.BlockId == _config.BlockId);
				if (idx >= 0)
					configs[idx] = _config;
				else
					configs.Add(_config);

				DemandNewConfigLoader.SaveAll(configs);

				// 런타임 엔진 다시 로드
				CheckEngineDemandNew.ReloadConfig();

				MessageBox.Show(
					Tools.IsLangKorean() ? "저장 완료. 엔진이 다시 로드되었습니다." : "Saved. Engine reloaded.",
					"OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					Tools.IsLangKorean()
						? String.Format("저장 실패: {0}", ex.Message)
						: String.Format("Save failed: {0}", ex.Message),
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		/// <summary>
		/// 블록 선택 변경 시 해당 블록 설정 로드
		/// </summary>
		private void CboBlockId_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_loadingConfig) return;
			if (cboBlockId.SelectedIndex < 0) return;

			string selectedId = cboBlockId.SelectedItem.ToString();
			if (string.IsNullOrEmpty(selectedId)) return;

			try
			{
				var configs = DemandNewConfigLoader.LoadAll();
				var found = configs.Find(c => c.BlockId == selectedId);
				if (found != null)
				{
					_config = found;
					_isNew = false;
					LoadConfigToUI();
				}
			}
			catch { }
		}

		private void BtnAddLoad_Click(object sender, EventArgs e)
		{
			var load = new LoadModel();
			load.LoadId = "L" + (_config.Loads.Count + 1).ToString("D3");
			var dlg = new FormEditLoadModel(load, true);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				_config.Loads.Add(load);
				RefreshLoadsListView();
			}
		}

		private void BtnEditLoad_Click(object sender, EventArgs e)
		{
			if (lvLoads.SelectedItems.Count == 0) return;
			int idx = lvLoads.SelectedItems[0].Index;
			if (idx < 0 || idx >= _config.Loads.Count) return;

			var dlg = new FormEditLoadModel(_config.Loads[idx], false);
			if (dlg.ShowDialog(this) == DialogResult.OK)
				RefreshLoadsListView();
		}

		private void BtnDeleteLoad_Click(object sender, EventArgs e)
		{
			if (lvLoads.SelectedItems.Count == 0) return;
			int idx = lvLoads.SelectedItems[0].Index;
			if (idx >= 0 && idx < _config.Loads.Count)
			{
				_config.Loads.RemoveAt(idx);
				RefreshLoadsListView();
			}
		}

		private void BtnExportInterval_Click(object sender, EventArgs e)
		{
			var exporter = new DemandCsvExporter(_config.DatabaseDsn);
			string path = System.IO.Path.Combine(txtExportPath.Text, "demand_interval.csv");
			bool ok = exporter.ExportIntervals(_config.BlockId, dtpFrom.Value, dtpTo.Value, path);
			ShowExportResult(ok, path);
		}

		private void BtnExportEvent_Click(object sender, EventArgs e)
		{
			var exporter = new DemandCsvExporter(_config.DatabaseDsn);
			string path = System.IO.Path.Combine(txtExportPath.Text, "demand_control_event.csv");
			bool ok = exporter.ExportControlEvents(_config.BlockId, dtpFrom.Value, dtpTo.Value, path);
			ShowExportResult(ok, path);
		}

		private void BtnExportPeak_Click(object sender, EventArgs e)
		{
			var exporter = new DemandCsvExporter(_config.DatabaseDsn);
			string path = System.IO.Path.Combine(txtExportPath.Text, "demand_monthly_peak.csv");
			bool ok = exporter.ExportMonthlyPeaks(_config.BlockId, path);
			ShowExportResult(ok, path);
		}

		private void BtnRecoverBackup_Click(object sender, EventArgs e)
		{
			var historian = new DemandHistorian(_config);
			historian.RecoverFromBackups();
			MessageBox.Show(Tools.IsLangKorean() ? "백업 복구 완료" : "Backup recovery complete");
		}

		// Designer 이벤트 핸들러 (InitializeComponent에서 연결)
		private void btnMeterTag_Click(object sender, EventArgs e) { BrowseTag(txtMeterTag, "AI"); }
		private void btnTargetTag_Click(object sender, EventArgs e) { BrowseTag(txtTargetTag, "AI"); }
		private void btnEOITag_Click(object sender, EventArgs e) { BrowseTag(txtEOITag, "DI"); }
		private void btnPredictionTag_Click(object sender, EventArgs e) { BrowseTag(txtPredictionTag, "AO"); }
		private void btnCommStatusTag_Click(object sender, EventArgs e) { BrowseTag(txtCommStatusTag, "DI"); }
		private void chkMultiStep_CheckedChanged(object sender, EventArgs e) { UpdateStepPanelVisibility(); }
		private void cboStepCount_SelectedIndexChanged(object sender, EventArgs e) { UpdateStepPanelVisibility(); }
		private void trkTrendWindow_ValueChanged(object sender, EventArgs e) { lblTrendValue.Text = trkTrendWindow.Value.ToString(); }
		private void trkEwmaAlpha_ValueChanged(object sender, EventArgs e) { lblAlphaValue.Text = (trkEwmaAlpha.Value / 100.0).ToString("F2"); }
		private void gridTimeZone_DataError(object sender, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl.SelectedIndex == 3)  // Loads tab
				lvLoads.Refresh();
			if (tabControl.SelectedIndex == 4)  // Policy tab
				RefreshStepLoadLists();
		}

		#endregion

		#region Helpers

		private void UpdateStepPanelVisibility()
		{
			bool enabled = chkMultiStep.Checked;
			cboStepCount.Enabled = enabled;
			pnlStepDetail.Visible = enabled;

			if (!enabled) return;

			int count = 1;
			if (cboStepCount.SelectedItem != null)
				count = (int)cboStepCount.SelectedItem;

			for (int i = 0; i < 3; i++)
				grpSteps[i].Visible = (i < count);
		}

		private void RefreshStepLoadLists()
		{
			for (int i = 0; i < 3; i++)
			{
				clbStepLoads[i].Items.Clear();
				foreach (var load in _config.Loads)
				{
					bool isChecked = false;
					if (i < _config.Steps.Count)
						isChecked = _config.Steps[i].LoadIds.Contains(load.LoadId);

					string display = string.IsNullOrEmpty(load.DisplayName)
						? load.LoadId
						: String.Format("{0} ({1})", load.LoadId, load.DisplayName);
					clbStepLoads[i].Items.Add(display, isChecked);
				}
			}
		}

		private void SaveStepDetails()
		{
			_config.Steps.Clear();
			if (!_config.MultiStepEnabled) return;

			int count = _config.StepCount;
			for (int i = 0; i < count && i < 3; i++)
			{
				var step = new StepConfig();
				step.ReductionKW = (double)numStepReductionKW[i].Value;

				for (int j = 0; j < clbStepLoads[i].Items.Count; j++)
				{
					if (clbStepLoads[i].GetItemChecked(j))
					{
						if (j < _config.Loads.Count)
							step.LoadIds.Add(_config.Loads[j].LoadId);
					}
				}
				_config.Steps.Add(step);
			}
		}

		private void RefreshLoadsListView()
		{
			lvLoads.Items.Clear();
			foreach (var load in _config.Loads)
			{
				var item = new ListViewItem(load.LoadId);
				item.SubItems.Add(load.DisplayName);
				item.SubItems.Add(load.Priority.ToString());
				item.SubItems.Add(load.Group);
				item.SubItems.Add(load.EstimatedKW.ToString("F0"));
				item.SubItems.Add(load.CommandTagName);
				item.SubItems.Add(load.FeedbackTagName);
				item.SubItems.Add(load.MinOffTimeSec.ToString());
				item.SubItems.Add(load.MinOnTimeSec.ToString());
				lvLoads.Items.Add(item);
			}
		}

		private void BrowseTag(TextBox target, string tagType)
		{
			try
			{
				string tag = "";
				string des = "";
				DialogResult result = DialogResult.Cancel;
				switch (tagType)
				{
					case "AI": result = DialogTag.SelectTag.SelectAi(this, out tag, out des); break;
					case "AO": result = DialogTag.SelectTag.SelectAo(this, out tag, out des); break;
					case "DI": result = DialogTag.SelectTag.SelectDi(this, out tag, out des); break;
					case "DO": result = DialogTag.SelectTag.SelectDo(this, out tag, out des); break;
				}
				if (result == DialogResult.OK && !string.IsNullOrEmpty(tag))
					target.Text = tag;
			}
			catch { }
		}

		private void FillBlockIdCombo()
		{
			cboBlockId.Items.Clear();
			try
			{
				var configs = DemandNewConfigLoader.LoadAll();
				foreach (var c in configs)
				{
					if (!string.IsNullOrEmpty(c.BlockId))
						cboBlockId.Items.Add(c.BlockId);
				}
			}
			catch { }
		}

		private void ShowExportResult(bool ok, string path)
		{
			if (ok)
				MessageBox.Show(Tools.IsLangKorean()
					? String.Format("내보내기 완료: {0}", path)
					: String.Format("Export complete: {0}", path));
			else
				MessageBox.Show(Tools.IsLangKorean() ? "내보내기 실패" : "Export failed",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		#endregion

		public DemandNewConfig GetConfig() { return _config; }
	}
}

using System;
using System.Drawing;
using System.Windows.Forms;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using NetTools;

namespace Studio
{
	/// <summary>
	/// 신형 디맨드 제어 블록 추가/수정 다이얼로그
	/// </summary>
	public partial class FormConfigDemandNewAdd : Form
	{
		private DemandNewConfig _config;

		public FormConfigDemandNewAdd(DemandNewConfig config)
		{
			_config = config;
			InitializeComponent();
			ApplyModernStyle();
			ApplyLocalization();
			ConfigToDialog();
		}

		private void ApplyModernStyle()
		{
			DemandUIStyle.StyleForm(this);
			DemandUIStyle.StyleAllControls(this);

			// 섹션 헤더 라벨에 악센트 스타일 적용
			DemandUIStyle.StyleSectionLabel(grpGeneral);
			DemandUIStyle.StyleSectionLabel(grpContract);
			DemandUIStyle.StyleSectionLabel(grpMetering);
			DemandUIStyle.StyleSectionLabel(grpPolicy);
			DemandUIStyle.StyleSectionLabel(grpForecast);
			DemandUIStyle.StyleSectionLabel(grpStorage);
			DemandUIStyle.StyleSectionLabel(grpDR);
		}

		/// <summary>
		/// 다국어 텍스트 적용 (InitializeComponent 외부 분리)
		/// </summary>
		private void ApplyLocalization()
		{
			if (!Tools.IsLangKorean()) return;

			this.Text = "디맨드 블록 설정";

			// 그룹 라벨
			grpGeneral.Text = "■ 일반";
			grpContract.Text = "■ 계약/목표";
			grpMetering.Text = "■ 계측";
			grpPolicy.Text = "■ 정책";
			grpForecast.Text = "■ 예측";
			grpStorage.Text = "■ 저장";
			grpDR.Text = "■ DR(수요반응)";

			// 필드 라벨
			lblBlockId.Text = "블록 ID:";
			lblTitle.Text = "제목:";
			lblMode.Text = "모드:";
			lblInterval.Text = "수요구간(분):";
			lblContractKW.Text = "계약전력(kW):";
			lblSafetyFactor.Text = "안전계수:";
			lblMeterTag.Text = "계측 태그:";
			lblMeterType.Text = "계측 타입:";
			lblPulseRatio.Text = "펄스 비율:";
			lblCommStatusTag.Text = "통신상태 태그:";
			lblShedMargin.Text = "차단 여유(kW):";
			lblRestoreMargin.Text = "복귀 여유(kW):";
			lblProtectionTime.Text = "보호시간(초):";
			lblTrendWindow.Text = "트렌드 윈도우(초):";
			lblEwmaAlpha.Text = "EWMA 계수:";
			lblDrTargetKW.Text = "DR 목표(kW):";
			lblDrStart.Text = "DR 시작:";
			lblDrEnd.Text = "DR 종료:";

			// 체크박스
			chkClockAligned.Text = "시간 정렬";
			chkDbSave.Text = "DB 저장 활성화";
			chkFailover.Text = "Failover 활성화";
			chkDrEnabled.Text = "DR 활성화";

			// 버튼
			buttonCancel.Text = "취소";
		}

		private void ConfigToDialog()
		{
			txtBlockId.Text = _config.BlockId ?? "";
			txtTitle.Text = _config.Title ?? "";
			cmbMode.SelectedIndex = (int)_config.Mode;
			cmbInterval.Text = _config.IntervalMinutes.ToString();
			chkClockAligned.Checked = _config.ClockAligned;

			numContractKW.Value = (decimal)_config.ContractKW;
			numSafetyFactor.Value = (decimal)_config.SafetyFactor;

			txtMeterTag.Text = _config.MeterTagName ?? "";
			cmbMeterType.SelectedIndex = (int)_config.MeterType;
			numPulseRatio.Value = (decimal)_config.PulseRatio;
			txtCommStatusTag.Text = _config.CommStatusTagName ?? "";

			numShedMargin.Value = (decimal)_config.ShedMarginKW;
			numRestoreMargin.Value = (decimal)_config.RestoreMarginKW;
			numProtectionTime.Value = _config.ProtectionTimeSec;

			numTrendWindow.Value = _config.TrendWindowSec;
			numEwmaAlpha.Value = (decimal)_config.EwmaAlpha;

			chkDbSave.Checked = _config.DatabaseSaveEnabled;
			txtDbDsn.Text = _config.DatabaseDsn ?? "";
			chkFailover.Checked = _config.FailoverEnabled;

			// DR
			if (_config.DrTarget != null)
			{
				chkDrEnabled.Checked = _config.DrTarget.IsEnabled;
				numDrTargetKW.Value = (decimal)_config.DrTarget.TargetKW;
				if (_config.DrTarget.StartTime > DateTime.MinValue)
					dtpDrStart.Value = _config.DrTarget.StartTime;
				if (_config.DrTarget.EndTime > DateTime.MinValue)
					dtpDrEnd.Value = _config.DrTarget.EndTime;
			}
		}

		private void DialogToConfig()
		{
			_config.BlockId = txtBlockId.Text.Trim();
			_config.Title = txtTitle.Text.Trim();
			_config.Mode = (EngineMode)cmbMode.SelectedIndex;
			int interval;
			if (int.TryParse(cmbInterval.Text, out interval))
				_config.IntervalMinutes = interval;
			_config.ClockAligned = chkClockAligned.Checked;

			_config.ContractKW = (double)numContractKW.Value;
			_config.SafetyFactor = (double)numSafetyFactor.Value;

			_config.MeterTagName = txtMeterTag.Text.Trim();
			_config.MeterType = (MeterType)cmbMeterType.SelectedIndex;
			_config.PulseRatio = (double)numPulseRatio.Value;
			_config.CommStatusTagName = txtCommStatusTag.Text.Trim();

			_config.ShedMarginKW = (double)numShedMargin.Value;
			_config.RestoreMarginKW = (double)numRestoreMargin.Value;
			_config.ProtectionTimeSec = (int)numProtectionTime.Value;

			_config.TrendWindowSec = (int)numTrendWindow.Value;
			_config.EwmaAlpha = (double)numEwmaAlpha.Value;

			_config.DatabaseSaveEnabled = chkDbSave.Checked;
			_config.DatabaseDsn = txtDbDsn.Text.Trim();
			_config.FailoverEnabled = chkFailover.Checked;

			// DR
			if (_config.DrTarget == null) _config.DrTarget = new DrTarget();
			_config.DrTarget.IsEnabled = chkDrEnabled.Checked;
			_config.DrTarget.TargetKW = (double)numDrTargetKW.Value;
			_config.DrTarget.StartTime = dtpDrStart.Value;
			_config.DrTarget.EndTime = dtpDrEnd.Value;
		}

		private void btnMeterTag_Click(object sender, EventArgs e)
		{
			string tag = "";
			string des = "";
			if (DialogTag.SelectTag.SelectAi(this, out tag, out des) == DialogResult.OK)
			{
				txtMeterTag.Text = tag;
			}
		}

		private void btnCommStatusTag_Click(object sender, EventArgs e)
		{
			string tag = "";
			string des = "";
			if (DialogTag.SelectTag.SelectDi(this, out tag, out des) == DialogResult.OK)
			{
				txtCommStatusTag.Text = tag;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtBlockId.Text.Trim()))
			{
				string msg = Tools.IsLangKorean() ? "블록 ID를 입력하세요." : "Please enter Block ID.";
				MessageBox.Show(msg);
				return;
			}
			DialogToConfig();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}

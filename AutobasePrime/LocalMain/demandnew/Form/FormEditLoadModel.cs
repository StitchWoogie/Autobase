using System;
using System.Drawing;
using System.Windows.Forms;
using AutoLibLocal.DemandNew;
using DialogTag;
using NetTools;

namespace LocalMain.DemandNew
{
	/// <summary>
	/// 부하(LoadModel) 추가/편집 다이얼로그
	/// </summary>
	public partial class FormEditLoadModel : Form
	{
		private LoadModel _load;
		private bool _isNew;

		public FormEditLoadModel(LoadModel load, bool isNew)
		{
			_load = load;
			_isNew = isNew;
			InitializeComponent();
			txtLoadId.Enabled = _isNew;
			ApplyModernStyle();
			ApplyLocalization();
			LoadToUI();
		}

		private void ApplyModernStyle()
		{
			DemandUIStyle.StyleForm(this);
			DemandUIStyle.StyleAllControls(this);
		}

		private void ApplyLocalization()
		{
			if (!Tools.IsLangKorean()) return;

			this.Text = _isNew ? "부하 추가" : "부하 편집";
			btnCancel.Text = "취소";

			// 전체 레이블/체크박스 번역
			var map = new System.Collections.Generic.Dictionary<string, string>
			{
				{ "Load ID:", "부하 ID:" },
				{ "Display Name:", "표시 이름:" },
				{ "Priority:", "우선순위:" },
				{ "Group:", "그룹:" },
				{ "Estimated KW:", "예상 전력(kW):" },
				{ "Command Tag:", "제어 태그:" },
				{ "Feedback Tag:", "피드백 태그:" },
				{ "Interlock Tag:", "인터락 태그:" },
				{ "Min Off Time(s):", "최소 차단시간(초):" },
				{ "Min On Time(s):", "최소 복귀시간(초):" },
				{ "Re-Shed Block", "재차단 방지" },
				{ "Block Time(s):", "방지시간(초):" },
			};
			LocalizeControls(this, map);
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

		private void LoadToUI()
		{
			txtLoadId.Text = _load.LoadId;
			txtDisplayName.Text = _load.DisplayName;
			numPriority.Value = Math.Max(1, Math.Min(100, _load.Priority));
			txtGroup.Text = _load.Group;
			numEstimatedKW.Value = (decimal)_load.EstimatedKW;
			txtCommandTag.Text = _load.CommandTagName;
			txtFeedbackTag.Text = _load.FeedbackTagName;
			txtInterlockTag.Text = _load.InterlockTagName;
			numMinOff.Value = Math.Max(0, Math.Min(7200, _load.MinOffTimeSec));
			numMinOn.Value = Math.Max(0, Math.Min(7200, _load.MinOnTimeSec));
			chkReShedBlock.Checked = _load.ReShedBlockEnabled;
			numReShedBlock.Value = Math.Max(0, Math.Min(7200, _load.ReShedBlockTimeSec));
		}

		private void SaveUIToLoad()
		{
			_load.LoadId = txtLoadId.Text.Trim();
			_load.DisplayName = txtDisplayName.Text.Trim();
			_load.Priority = (int)numPriority.Value;
			_load.Group = txtGroup.Text.Trim();
			_load.EstimatedKW = (double)numEstimatedKW.Value;
			_load.CommandTagName = txtCommandTag.Text.Trim();
			_load.FeedbackTagName = txtFeedbackTag.Text.Trim();
			_load.InterlockTagName = txtInterlockTag.Text.Trim();
			_load.MinOffTimeSec = (int)numMinOff.Value;
			_load.MinOnTimeSec = (int)numMinOn.Value;
			_load.ReShedBlockEnabled = chkReShedBlock.Checked;
			_load.ReShedBlockTimeSec = (int)numReShedBlock.Value;
		}

		private void BtnOK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtLoadId.Text.Trim()))
			{
				string msg = Tools.IsLangKorean() ? "부하 ID를 입력하세요." : "Please enter Load ID.";
				MessageBox.Show(msg);
				return;
			}
			SaveUIToLoad();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnCommandTag_Click(object sender, EventArgs e) { BrowseTag(txtCommandTag, "DO"); }
		private void btnFeedbackTag_Click(object sender, EventArgs e) { BrowseTag(txtFeedbackTag, "DI"); }
		private void btnInterlockTag_Click(object sender, EventArgs e) { BrowseTag(txtInterlockTag, "DI"); }

		private void BrowseTag(TextBox target, string tagType)
		{
			try
			{
				string tag = "";
				string des = "";
				DialogResult result = DialogResult.Cancel;
				if (tagType == "DO") result = SelectTag.SelectDo(this, out tag, out des);
				else if (tagType == "DI") result = SelectTag.SelectDi(this, out tag, out des);
				if (result == DialogResult.OK && !string.IsNullOrEmpty(tag))
					target.Text = tag;
			}
			catch { }
		}
	}
}

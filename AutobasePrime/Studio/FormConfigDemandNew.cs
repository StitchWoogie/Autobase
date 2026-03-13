using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AutoLibLocal;
using AutoLibLocal.DemandNew;
using NetTools;

namespace Studio
{
	/// <summary>
	/// 신형 디맨드 제어 블록 설정 폼 (Studio용)
	/// </summary>
	public partial class FormConfigDemandNew : Form
	{
		private List<DemandNewConfig> blockTemp;

		public FormConfigDemandNew()
		{
			InitializeComponent();
			ApplyModernStyle();
			ApplyLocalization();
			LoadBlocks();
		}

		/// <summary>
		/// Studio Config 메뉴에서 호출하는 진입점
		/// </summary>
		public static bool FunctionBlockDemandNew(Form parentForm)
		{
			FormConfigDemandNew frm = new FormConfigDemandNew();
			frm.StartPosition = FormStartPosition.CenterParent;
			frm.ShowDialog(parentForm);
			return true;
		}

		private void ApplyModernStyle()
		{
			DemandUIStyle.StyleForm(this);
			DemandUIStyle.StyleAllControls(this);

			// Add 버튼 Primary 스타일
			DemandUIStyle.StyleButtonPrimary(buttonAdd);
		}

		/// <summary>
		/// 다국어 텍스트 적용 (InitializeComponent 외부 분리)
		/// </summary>
		private void ApplyLocalization()
		{
			if (!Tools.IsLangKorean()) return;

			this.Text = "신형 디맨드 제어 설정";
			buttonAdd.Text = "추가";
			buttonModify.Text = "수정";
			buttonDelete.Text = "삭제";
			buttonClose.Text = "닫기";
		}

		private void LoadBlocks()
		{
			blockTemp = DemandNewConfigLoader.LoadAll();
			RefreshListView();
		}

		private void RefreshListView()
		{
			m_list.Items.Clear();
			foreach (var config in blockTemp)
			{
				ListViewItem item = new ListViewItem(config.BlockId);
				item.SubItems.Add(config.Title);
				item.SubItems.Add(config.Mode.ToString());
				item.SubItems.Add(config.IntervalMinutes.ToString() + " min");
				item.SubItems.Add(config.ContractKW.ToString("F0"));
				item.SubItems.Add(config.Loads.Count.ToString());
				m_list.Items.Add(item);
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			DemandNewConfig newConfig = new DemandNewConfig();
			newConfig.BlockId = "Block" + (blockTemp.Count + 1);
			newConfig.Title = "New Demand Block";

			FormConfigDemandNewAdd dlg = new FormConfigDemandNewAdd(newConfig);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				blockTemp.Add(newConfig);
				DemandNewConfigLoader.SaveAll(blockTemp);
				CheckDOConflicts();
				RefreshListView();
			}
		}

		private void buttonModify_Click(object sender, EventArgs e)
		{
			ModifySelected();
		}

		private void m_list_DoubleClick(object sender, EventArgs e)
		{
			ModifySelected();
		}

		private void ModifySelected()
		{
			if (m_list.SelectedIndices.Count == 0) return;
			int idx = m_list.SelectedIndices[0];
			if (idx < 0 || idx >= blockTemp.Count) return;

			DemandNewConfig config = blockTemp[idx];
			FormConfigDemandNewAdd dlg = new FormConfigDemandNewAdd(config);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				DemandNewConfigLoader.SaveAll(blockTemp);
				CheckDOConflicts();
				RefreshListView();
			}
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (m_list.SelectedIndices.Count == 0) return;
			int idx = m_list.SelectedIndices[0];
			if (idx < 0 || idx >= blockTemp.Count) return;

			string blockId = blockTemp[idx].BlockId;
			string msg = Tools.IsLangKorean()
				? String.Format("블록 '{0}'을(를) 삭제하시겠습니까?", blockId)
				: String.Format("Delete block '{0}'?", blockId);
			string title = Tools.IsLangKorean() ? "확인" : "Confirm";

			if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				blockTemp.RemoveAt(idx);
				DemandNewConfigLoader.SaveAll(blockTemp);
				RefreshListView();
			}
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// 저장 후 DO 태그 충돌 진단.
		/// 블록 간 동일 CommandTag 사용 시 경고 표시.
		/// </summary>
		private void CheckDOConflicts()
		{
			var usedTags = new Dictionary<string, string>();  // tagName(lower) -> blockId

			foreach (var config in blockTemp)
			{
				foreach (var load in config.Loads)
				{
					if (string.IsNullOrEmpty(load.CommandTagName)) continue;

					string tag = load.CommandTagName.ToLower();
					if (usedTags.ContainsKey(tag))
					{
						string msg = Tools.IsLangKorean()
							? String.Format("경고: DO 태그 '{0}'가 블록 '{1}'과 '{2}'에서 중복 사용됩니다.",
								load.CommandTagName, usedTags[tag], config.BlockId)
							: String.Format("Warning: DO tag '{0}' is used in both block '{1}' and '{2}'.",
								load.CommandTagName, usedTags[tag], config.BlockId);

						string title = Tools.IsLangKorean() ? "DO 충돌 경고" : "DO Conflict Warning";
						MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
					{
						usedTags[tag] = config.BlockId;
					}
				}
			}
		}
	}
}

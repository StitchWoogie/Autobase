using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using DialogTag;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PublicStudioLocalMain.Recipe
{
	/// <summary>
	/// Studio/LocalMain용 Preset 설정 폼
	/// JSON 파일 기반 Preset 관리 (DB 미사용)
	/// Preset → AliasMap + Variant → Items(Alias, Value) 구조
	/// </summary>
	public partial class FormConfigPreset : System.Windows.Forms.Form
	{
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
		private const int EM_SETCUEBANNER = 0x1501;


		// MDI support (CatWindowRing — LocalMain에서 MDI 자식폼으로 사용)
		public static CatWindowRing ringPreset = new CatWindowRing();

		// Delegate for tag write (PlcScan.SetTagValue — LocalMain에서 연결)
		public delegate Task DelegateSetTagValue(string tag, string value, bool bForce);
		public static DelegateSetTagValue SetTagValueFunc = null;

		// Data
		private PresetData _current;
		private PresetVariant _currentVariant;
		private List<PresetData> _allPresets = new List<PresetData>();
		private Dictionary<string, string> _presetDates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private string _presetDir;
		private bool _isDirty = false;
		private int _dragRowIndex = -1;
		private bool _aliasMapCollapsed = false;

		public FormConfigPreset()
		{
			InitializeComponent();
			PostInitializeComponent();
			ApplyLocalization();
			InitPresetDir();
			LoadPresetList();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ringPreset.push(this);
		}

		protected override void OnClosed(EventArgs e)
		{
			ringPreset.pop(this);
			base.OnClosed(e);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			bool isKor = NetTools.Tools.IsLangKorean();
			SendMessage(txtSearch.Handle, EM_SETCUEBANNER, IntPtr.Zero,
				isKor ? "검색..." : "Search...");
		}

		#region PostInitializeComponent

		private void PostInitializeComponent()
		{
			// DataError handlers
			this.dgvItems.DataError += DataGrid_DataError;
			this.dgvAliasMap.DataError += DataGrid_DataError;

			// MODE_RUN에서만 Apply/Capture 버튼 표시
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				this.toolStripSep3.Visible = true;
				this.btnApply.Visible = true;
				this.btnCapture.Visible = true;
			}

			SetupGridStyle(dgvAliasMap);
			SetupGridStyle(dgvItems);

			// AliasMap 접기/펼치기 — 라벨 클릭으로 토글
			this.lblAliasCaption.Cursor = Cursors.Hand;
			this.lblAliasCaption.Click += lblAliasCaption_Click;
		}

		private void SetupGridStyle(DataGridView dgv)
		{
			// Modern flat grid styling
			dgv.EnableHeadersVisualStyles = false;
			DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
			headerStyle.BackColor = Color.FromArgb(240, 240, 240);
			headerStyle.ForeColor = Color.Black;
			headerStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
			headerStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			dgv.ColumnHeadersDefaultCellStyle = headerStyle;
			dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dgv.ColumnHeadersHeight = 28;

			dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 240);
			dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
			dgv.GridColor = Color.FromArgb(220, 220, 220);

			DataGridViewCellStyle altStyle = new DataGridViewCellStyle();
			altStyle.BackColor = Color.FromArgb(248, 248, 248);
			dgv.AlternatingRowsDefaultCellStyle = altStyle;
			dgv.RowTemplate.Height = 28;
		}

		private void DataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			e.ThrowException = false;
		}

		#endregion

		#region Localization

		private void ApplyLocalization()
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			this.Text = isKor ? "프리셋 설정" : "Preset Configuration";
			this.btnNew.Text = isKor ? "✚ 새로만들기" : "✚ New";
			this.btnDelete.Text = isKor ? "✕ 삭제" : "✕ Delete";
			this.btnSave.Text = isKor ? "■ 저장" : "■ Save";
			this.btnExportCsv.Text = isKor ? "↑ 내보내기" : "↑ Export";
			this.btnImportCsv.Text = isKor ? "↓ 가져오기" : "↓ Import";
			this.btnApply.Text = isKor ? "▶ 적용" : "▶ Apply";
			this.btnCapture.Text = isKor ? "◎ 캡처" : "◎ Capture";
			this.labelName.Text = isKor ? "이름:" : "Name:";
			this.colName.Text = isKor ? "이름" : "Name";
			this.colVariantCount.Text = isKor ? "변형" : "Variants";
			this.colUpdated.Text = isKor ? "수정일" : "Updated";
			UpdateAliasCaptionText();
			this.lblVariantCaption.Text = "Variants";
			this.btnAddVariant.Text = isKor ? "+ 추가" : "+ Add";
			this.btnDeleteVariant.Text = isKor ? "- 삭제" : "- Del";
			this.btnAddAlias.Text = isKor ? "+ 추가" : "+ Add";
			this.btnRemoveAlias.Text = isKor ? "- 삭제" : "- Del";

			this.dgvAliasMap.Columns["AliasCol"].HeaderText = isKor ? "별칭" : "Alias";
			this.dgvAliasMap.Columns["TagCol"].HeaderText = isKor ? "태그" : "Tag";
			this.dgvItems.Columns["AliasName"].HeaderText = isKor ? "별칭" : "Alias";
			this.dgvItems.Columns["SetValue"].HeaderText = isKor ? "값" : "Value";
		}

		#endregion

		#region Preset Directory

		private void InitPresetDir()
		{
			string projectPath = TotalConfig.sDirWorkProject;
			if (string.IsNullOrEmpty(projectPath))
				projectPath = AppDomain.CurrentDomain.BaseDirectory;
			_presetDir = Path.Combine(projectPath, "Presets");
			if (!Directory.Exists(_presetDir))
				Directory.CreateDirectory(_presetDir);
		}

		#endregion

		#region Data Model (matches PresetManager in LocalMain)

		public class AliasEntry
		{
			[JsonProperty("alias")] public string Alias = "";
			[JsonProperty("tag")]   public string Tag = "";
		}

		public class PresetData
		{
			[JsonProperty("preset_name")] public string PresetName = "";
			[JsonProperty("alias_map")]   public List<AliasEntry> AliasMap = new List<AliasEntry>();
			[JsonProperty("variants")]    public List<PresetVariant> Variants = new List<PresetVariant>();
		}

		public class PresetVariant
		{
			[JsonProperty("name")] public string Name = "";
			[JsonProperty("items")] public List<PresetItem> Items = new List<PresetItem>();
		}

		public class PresetItem
		{
			[JsonProperty("alias")] public string Alias = "";
			[JsonProperty("value")] public string Value = "";
		}

		/// <summary>
		/// 중간 포맷 역직렬화용 (preset_name + variants, tag 필드 사용)
		/// </summary>
		class MidPresetData
		{
			[JsonProperty("preset_name")] public string PresetName = "";
			[JsonProperty("variants")] public List<MidPresetVariant> Variants = new List<MidPresetVariant>();
		}

		class MidPresetVariant
		{
			[JsonProperty("name")] public string Name = "";
			[JsonProperty("items")] public List<MidPresetItem> Items = new List<MidPresetItem>();
		}

		class MidPresetItem
		{
			[JsonProperty("tag")] public string Tag = "";
			[JsonProperty("value")] public string Value = "";
		}

		/// <summary>
		/// 구 포맷 역직렬화용 (마이그레이션)
		/// </summary>
		class OldPresetData
		{
			[JsonProperty("name")] public string Name = "";
			[JsonProperty("description")] public string Description = "";
			[JsonProperty("items")] public List<MidPresetItem> Items = new List<MidPresetItem>();
		}

		/// <summary>
		/// JSON 문자열에서 PresetData 로드 (3단계 포맷 자동 마이그레이션)
		/// </summary>
		private static PresetData DeserializePreset(string json)
		{
			var jobj = JObject.Parse(json);

			// 최신 포맷: alias_map 필드 존재
			if (jobj["alias_map"] != null)
				return jobj.ToObject<PresetData>();

			// 중간 포맷: preset_name 존재, alias_map 없음
			if (jobj["preset_name"] != null)
			{
				var mid = jobj.ToObject<MidPresetData>();
				return MigrateFromMidFormat(mid);
			}

			// 구 포맷: name + items 필드 존재
			if (jobj["name"] != null && jobj["items"] != null)
			{
				var old = jobj.ToObject<OldPresetData>();
				return MigrateFromOldFormat(old);
			}

			return null;
		}

		private static PresetData MigrateFromMidFormat(MidPresetData mid)
		{
			var preset = new PresetData { PresetName = mid.PresetName };

			var tagSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var tagOrder = new List<string>();
			for (int v = 0; v < mid.Variants.Count; v++)
			{
				for (int i = 0; i < mid.Variants[v].Items.Count; i++)
				{
					string tag = mid.Variants[v].Items[i].Tag;
					if (!string.IsNullOrEmpty(tag) && tagSet.Add(tag))
						tagOrder.Add(tag);
				}
			}

			for (int i = 0; i < tagOrder.Count; i++)
				preset.AliasMap.Add(new AliasEntry { Alias = tagOrder[i], Tag = tagOrder[i] });

			for (int v = 0; v < mid.Variants.Count; v++)
			{
				var newVariant = new PresetVariant { Name = mid.Variants[v].Name };
				for (int i = 0; i < mid.Variants[v].Items.Count; i++)
				{
					newVariant.Items.Add(new PresetItem
					{
						Alias = mid.Variants[v].Items[i].Tag,
						Value = mid.Variants[v].Items[i].Value
					});
				}
				preset.Variants.Add(newVariant);
			}

			return preset;
		}

		private static PresetData MigrateFromOldFormat(OldPresetData old)
		{
			var preset = new PresetData { PresetName = old.Name };

			for (int i = 0; i < old.Items.Count; i++)
			{
				string tag = old.Items[i].Tag;
				if (!string.IsNullOrEmpty(tag))
					preset.AliasMap.Add(new AliasEntry { Alias = tag, Tag = tag });
			}

			var variant = new PresetVariant { Name = "Default" };
			for (int i = 0; i < old.Items.Count; i++)
			{
				variant.Items.Add(new PresetItem
				{
					Alias = old.Items[i].Tag,
					Value = old.Items[i].Value
				});
			}
			preset.Variants.Add(variant);

			return preset;
		}

		/// <summary>
		/// Preset 내에서 이름으로 Variant 찾기
		/// </summary>
		private static PresetVariant FindVariant(PresetData preset, string variantName)
		{
			if (preset == null || preset.Variants == null) return null;
			for (int i = 0; i < preset.Variants.Count; i++)
			{
				if (string.Equals(preset.Variants[i].Name, variantName, StringComparison.OrdinalIgnoreCase))
					return preset.Variants[i];
			}
			return null;
		}

		/// <summary>
		/// AliasMap에서 Alias로 Tag 찾기
		/// </summary>
		private static string FindTagByAlias(List<AliasEntry> aliasMap, string alias)
		{
			if (aliasMap == null || string.IsNullOrEmpty(alias)) return null;
			for (int i = 0; i < aliasMap.Count; i++)
			{
				if (string.Equals(aliasMap[i].Alias, alias, StringComparison.OrdinalIgnoreCase))
					return aliasMap[i].Tag;
			}
			return null;
		}

		#endregion

		#region Load / Save

		private void LoadPresetList()
		{
			listViewPresets.Items.Clear();
			_allPresets.Clear();
			_presetDates.Clear();
			_current = null;
			_currentVariant = null;

			try
			{
				if (ConfigVarTotal.bLocalFlag)
				{
					// 로컬: 직접 파일 시스템 접근
					string[] files = Directory.GetFiles(_presetDir, "*.json");
					foreach (string f in files)
					{
						try
						{
							string json = File.ReadAllText(f, System.Text.Encoding.UTF8);
							PresetData p = DeserializePreset(json);
							if (p != null)
							{
								_allPresets.Add(p);
								_presetDates[p.PresetName] = File.GetLastWriteTime(f).ToString("yyyy-MM-dd HH:mm");
							}
						}
						catch { }
					}
				}
				else
				{
					// 원격: DataGate 웹서비스
					DataGate gate = new DataGate();
					var list = gate.PresetGetList();
					for (int i = 0; i < list.Count; i++)
					{
						string json = gate.PresetGet(list[i].name);
						if (json != null)
						{
							PresetData p = DeserializePreset(json);
							if (p != null)
							{
								_allPresets.Add(p);
								_presetDates[p.PresetName] = list[i].date;
							}
						}
					}
				}
			}
			catch { }

			ApplySearchFilter();
			ClearDetailPanel();
		}

		private void ApplySearchFilter()
		{
			string filter = txtSearch.Text.Trim().ToLowerInvariant();
			listViewPresets.BeginUpdate();
			listViewPresets.Items.Clear();

			for (int i = 0; i < _allPresets.Count; i++)
			{
				PresetData p = _allPresets[i];
				if (!string.IsNullOrEmpty(filter) &&
					p.PresetName.ToLowerInvariant().IndexOf(filter) < 0)
					continue;

				ListViewItem lvi = new ListViewItem(p.PresetName);
				lvi.SubItems.Add(p.Variants.Count.ToString());
				string dateStr = "";
				if (_presetDates.ContainsKey(p.PresetName))
					dateStr = _presetDates[p.PresetName];
				lvi.SubItems.Add(dateStr);
				lvi.Tag = p;
				listViewPresets.Items.Add(lvi);
			}

			listViewPresets.EndUpdate();
		}

		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			ApplySearchFilter();
		}

		private bool SaveCurrentPreset()
		{
			if (_current == null) return false;

			bool isKor = NetTools.Tools.IsLangKorean();

			string name = textBoxName.Text.Trim();
			if (string.IsNullOrEmpty(name))
			{
				MessageBox.Show(isKor ? "프리셋 이름을 입력하세요." : "Please enter preset name.",
					isKor ? "입력 오류" : "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			// Update current from UI
			_current.PresetName = name;

			// Save AliasMap from grid
			SaveAliasMap();

			// Save current variant items from grid
			SaveGridItems();

			// Tag validation — AliasMap의 태그 존재 검사
			var invalidTags = new List<string>();
			for (int i = 0; i < _current.AliasMap.Count; i++)
			{
				var ae = _current.AliasMap[i];
				if (string.IsNullOrEmpty(ae.Tag)) continue;
				if (!TagLib.IsTagExist(ae.Tag))
					invalidTags.Add(ae.Alias + " \u2192 " + ae.Tag);
			}
			if (invalidTags.Count > 0)
			{
				string tagList = string.Join("\n", invalidTags.ToArray());
				string msg = isKor
					? string.Format("다음 태그가 존재하지 않습니다 ({0}개):\n{1}\n\n계속 저장하시겠습니까?", invalidTags.Count, tagList)
					: string.Format("The following tags do not exist ({0}):\n{1}\n\nContinue saving?", invalidTags.Count, tagList);
				var drTag = MessageBox.Show(msg,
					isKor ? "태그 검증 경고" : "Tag Validation Warning",
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (drTag != DialogResult.Yes) return false;
			}

			// Write JSON
			try
			{
				string json = JsonConvert.SerializeObject(_current, Formatting.Indented);
				if (ConfigVarTotal.bLocalFlag)
				{
					string fileName = SanitizeFileName(_current.PresetName) + ".json";
					string filePath = Path.Combine(_presetDir, fileName);
					File.WriteAllText(filePath, json, new System.Text.UTF8Encoding(true));
				}
				else
				{
					DataGate gate = new DataGate();
					var (success, error) = gate.PresetSave(_current.PresetName, json);
					if (!success)
					{
						MessageBox.Show(error ?? "Save failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
				_isDirty = false;
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		private static string SanitizeFileName(string name)
		{
			char[] invalid = Path.GetInvalidFileNameChars();
			string result = name;
			foreach (char c in invalid)
				result = result.Replace(c, '_');
			return result;
		}

		#endregion

		#region Event Handlers

		private void listViewPresets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_isDirty && _current != null)
			{
				bool isKor = NetTools.Tools.IsLangKorean();
				var dr = MessageBox.Show(
					isKor ? "변경사항을 저장하시겠습니까?" : "Save changes?",
					isKor ? "확인" : "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dr == DialogResult.Yes)
					SaveCurrentPreset();
				else if (dr == DialogResult.Cancel)
					return;
			}

			if (listViewPresets.SelectedItems.Count > 0)
			{
				_current = listViewPresets.SelectedItems[0].Tag as PresetData;
				ShowDetail(_current);
			}
			else
			{
				_current = null;
				_currentVariant = null;
				ClearDetailPanel();
			}
		}

		private void btnNew_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			string name = ShowInputDialog(
				isKor ? "프리셋 이름을 입력하세요:" : "Enter preset name:",
				isKor ? "새 프리셋" : "New Preset", "");
			if (string.IsNullOrWhiteSpace(name)) return;

			// Check duplicate
			foreach (var p in _allPresets)
			{
				if (string.Equals(p.PresetName, name, StringComparison.OrdinalIgnoreCase))
				{
					MessageBox.Show(isKor ? "이미 존재하는 이름입니다." : "Name already exists.",
						isKor ? "중복" : "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
			}

			var preset = new PresetData { PresetName = name };

			_allPresets.Add(preset);
			_current = preset;

			// Save immediately
			string json = JsonConvert.SerializeObject(preset, Formatting.Indented);
			if (ConfigVarTotal.bLocalFlag)
			{
				string fileName = SanitizeFileName(preset.PresetName) + ".json";
				string filePath = Path.Combine(_presetDir, fileName);
				File.WriteAllText(filePath, json, new System.Text.UTF8Encoding(true));
			}
			else
			{
				DataGate gate = new DataGate();
				gate.PresetSave(preset.PresetName, json);
			}

			// Refresh
			LoadPresetList();
			SelectPresetByName(name);
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (_current == null) return;
			bool isKor = NetTools.Tools.IsLangKorean();

			var dr = MessageBox.Show(
				string.Format(isKor ? "'{0}' 프리셋을 삭제하시겠습니까?" : "Delete preset '{0}'?", _current.PresetName),
				isKor ? "삭제 확인" : "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dr != DialogResult.Yes) return;

			try
			{
				if (ConfigVarTotal.bLocalFlag)
				{
					string fileName = SanitizeFileName(_current.PresetName) + ".json";
					string filePath = Path.Combine(_presetDir, fileName);
					if (File.Exists(filePath))
						File.Delete(filePath);
				}
				else
				{
					DataGate gate = new DataGate();
					var (success, error) = gate.PresetDelete(_current.PresetName);
					if (!success)
					{
						MessageBox.Show(error ?? "Delete failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			_isDirty = false;
			_current = null;
			_currentVariant = null;
			LoadPresetList();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (SaveCurrentPreset())
			{
				string name = _current.PresetName;
				LoadPresetList();
				SelectPresetByName(name);
			}
		}

		private void btnExportCsv_Click(object sender, EventArgs e)
		{
			if (_current == null) return;

			// Save current state to model
			SaveAliasMap();
			SaveGridItems();

			using (var sfd = new SaveFileDialog())
			{
				sfd.Filter = "CSV files (*.csv)|*.csv";
				sfd.FileName = SanitizeFileName(_current.PresetName) + ".csv";
				if (sfd.ShowDialog() != DialogResult.OK) return;

				try
				{
					using (var sw = new StreamWriter(sfd.FileName, false, new System.Text.UTF8Encoding(true)))
					{
						sw.WriteLine("Preset,Variant,Alias,Tag,Value");
						for (int v = 0; v < _current.Variants.Count; v++)
						{
							var variant = _current.Variants[v];
							for (int i = 0; i < variant.Items.Count; i++)
							{
								var item = variant.Items[i];
								string tag = FindTagByAlias(_current.AliasMap, item.Alias) ?? "";
								sw.WriteLine("{0},{1},{2},{3},{4}",
									EscapeCsv(_current.PresetName),
									EscapeCsv(variant.Name),
									EscapeCsv(item.Alias),
									EscapeCsv(tag),
									EscapeCsv(item.Value));
							}
						}
					}
					bool isKor = NetTools.Tools.IsLangKorean();
					MessageBox.Show(isKor ? "CSV 내보내기 완료." : "CSV exported.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void btnImportCsv_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "CSV files (*.csv)|*.csv";
				if (ofd.ShowDialog() != DialogResult.OK) return;

				try
				{
					var lines = File.ReadAllLines(ofd.FileName, System.Text.Encoding.UTF8);
					if (lines.Length < 2)
					{
						MessageBox.Show(isKor ? "CSV 파일이 비어있습니다." : "CSV file is empty.",
							isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}

					// CSV 파싱 — Variant별 그룹핑 + AliasMap 구성
					var variantMap = new Dictionary<string, List<PresetItem>>(StringComparer.OrdinalIgnoreCase);
					var aliasTagMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					string csvPresetName = "";

					for (int i = 0; i < lines.Length; i++)
					{
						string line = lines[i].Trim();
						if (string.IsNullOrEmpty(line)) continue;
						if (line.StartsWith("[")) continue;

						string[] cols = ParseCsvLine(line);
						if (IsCsvHeaderLine(cols)) continue;

						if (cols.Length >= 5)
						{
							// 최신 포맷: Preset,Variant,Alias,Tag,Value
							if (string.IsNullOrEmpty(csvPresetName))
								csvPresetName = cols[0].Trim();

							string vName = cols[1].Trim();
							string alias = cols[2].Trim();
							string tag = cols[3].Trim();
							string val = cols[4].Trim();

							if (!string.IsNullOrEmpty(alias) && !aliasTagMap.ContainsKey(alias))
								aliasTagMap[alias] = tag;

							if (!variantMap.ContainsKey(vName))
								variantMap[vName] = new List<PresetItem>();

							variantMap[vName].Add(new PresetItem { Alias = alias, Value = val });
						}
						else if (cols.Length >= 4)
						{
							// 중간 포맷: Preset,Variant,Tag,Value
							if (string.IsNullOrEmpty(csvPresetName))
								csvPresetName = cols[0].Trim();

							string vName = cols[1].Trim();
							string tag = cols[2].Trim();
							string val = cols[3].Trim();

							if (!string.IsNullOrEmpty(tag) && !aliasTagMap.ContainsKey(tag))
								aliasTagMap[tag] = tag;

							if (!variantMap.ContainsKey(vName))
								variantMap[vName] = new List<PresetItem>();

							variantMap[vName].Add(new PresetItem { Alias = tag, Value = val });
						}
						else if (cols.Length >= 2)
						{
							// 구 포맷: TagName,Value → Default variant
							string vName = "Default";
							string tag = cols[0].Trim();

							if (!string.IsNullOrEmpty(tag) && !aliasTagMap.ContainsKey(tag))
								aliasTagMap[tag] = tag;

							if (!variantMap.ContainsKey(vName))
								variantMap[vName] = new List<PresetItem>();

							variantMap[vName].Add(new PresetItem
							{
								Alias = tag,
								Value = cols[1].Trim()
							});
						}
					}

					if (variantMap.Count == 0)
					{
						MessageBox.Show(isKor ? "CSV 파일에 항목이 없습니다." : "No items found in CSV file.",
							isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}

					// 새 프리셋 이름 입력
					string defaultName = Path.GetFileNameWithoutExtension(ofd.FileName);
					if (!string.IsNullOrEmpty(csvPresetName))
						defaultName = csvPresetName;

					string newName = ShowInputDialog(
						isKor ? "새 프리셋 이름을 입력하세요:" : "Enter new preset name:",
						isKor ? "CSV 가져오기" : "CSV Import", defaultName);
					if (string.IsNullOrWhiteSpace(newName)) return;

					// 이름 중복 검사
					foreach (var p in _allPresets)
					{
						if (string.Equals(p.PresetName, newName, StringComparison.OrdinalIgnoreCase))
						{
							MessageBox.Show(isKor ? "이미 존재하는 이름입니다." : "Name already exists.",
								isKor ? "중복" : "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
					}

					// 새 프리셋 생성
					var newPreset = new PresetData { PresetName = newName };
					foreach (var kvp in aliasTagMap)
						newPreset.AliasMap.Add(new AliasEntry { Alias = kvp.Key, Tag = kvp.Value });

					foreach (var kvp in variantMap)
					{
						newPreset.Variants.Add(new PresetVariant
						{
							Name = kvp.Key,
							Items = kvp.Value
						});
					}

					// 저장
					string fileName = SanitizeFileName(newPreset.PresetName) + ".json";
					string filePath = Path.Combine(_presetDir, fileName);
					string json = JsonConvert.SerializeObject(newPreset, Formatting.Indented);
					File.WriteAllText(filePath, json, new System.Text.UTF8Encoding(true));

					// 목록 갱신 및 새 프리셋 선택
					_isDirty = false;
					LoadPresetList();
					SelectPresetByName(newName);

					MessageBox.Show(
						string.Format(isKor ? "CSV 가져오기 완료. 프리셋 '{0}' 생성됨." : "CSV imported. Preset '{0}' created.", newName),
						"OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private async void btnApply_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			if (_current == null || _currentVariant == null)
			{
				MessageBox.Show(
					isKor ? "Preset과 Variant를 선택하세요." : "Select a preset and variant.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
			{
				MessageBox.Show(
					isKor ? "RUN 모드에서만 Apply 가능합니다." : "Apply only available in RUN mode.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (SetTagValueFunc == null)
			{
				MessageBox.Show(
					isKor ? "태그 쓰기 기능이 연결되지 않았습니다." : "Tag write function not connected.",
					isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// Save current state to model
			SaveAliasMap();
			SaveGridItems();

			if (MessageBox.Show(
				string.Format(isKor
					? "Preset '{0}' / Variant '{1}'의 값을 태그에 쓰시겠습니까?"
					: "Write preset '{0}' / variant '{1}' values to tags?",
					_current.PresetName, _currentVariant.Name),
				isKor ? "확인" : "Confirm",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			int successCount = 0;
			int failCount = 0;

			for (int i = 0; i < _currentVariant.Items.Count; i++)
			{
				var item = _currentVariant.Items[i];
				try
				{
					// AliasMap에서 태그 resolve
					string tag = FindTagByAlias(_current.AliasMap, item.Alias);
					if (string.IsNullOrEmpty(tag))
					{
						failCount++;
						System.Diagnostics.Debug.WriteLine("PresetApply: Alias not in AliasMap - " + item.Alias);
						continue;
					}

					if (TagLib.IsTagExist(tag))
					{
						await SetTagValueFunc(tag, item.Value, false);
						successCount++;
					}
					else
					{
						failCount++;
						System.Diagnostics.Debug.WriteLine("PresetApply: Tag not found - " + tag + " (alias: " + item.Alias + ")");
					}
				}
				catch (Exception ex)
				{
					failCount++;
					System.Diagnostics.Debug.WriteLine("PresetApply error [" + item.Alias + "]: " + ex.Message);
				}
			}

			MessageBox.Show(
				string.Format(isKor
					? "Apply 완료: 성공 {0}, 실패 {1}"
					: "Apply done: OK {0}, Fail {1}", successCount, failCount),
				"Apply", MessageBoxButtons.OK,
				failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
		}

		private void btnCapture_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			if (_current == null || _currentVariant == null)
			{
				MessageBox.Show(
					isKor ? "Preset과 Variant를 선택하세요." : "Select a preset and variant.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
			{
				MessageBox.Show(
					isKor ? "RUN 모드에서만 Capture 가능합니다." : "Capture only available in RUN mode.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// Save current state to model
			SaveAliasMap();
			SaveGridItems();

			string saveName = ShowInputDialog(
				isKor ? "저장할 Preset 이름을 입력하세요:" : "Enter preset name to save:",
				"Capture", _current.PresetName);
			if (string.IsNullOrWhiteSpace(saveName)) return;

			try
			{
				// saveName 프리셋이 이미 있으면 로드, 없으면 새로 생성
				PresetData targetPreset = null;
				string fileName = SanitizeFileName(saveName) + ".json";
				string filePath = Path.Combine(_presetDir, fileName);
				if (File.Exists(filePath))
				{
					string existJson = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
					targetPreset = DeserializePreset(existJson);
				}
				if (targetPreset == null)
				{
					targetPreset = new PresetData { PresetName = saveName };
					// AliasMap 복사
					for (int i = 0; i < _current.AliasMap.Count; i++)
					{
						targetPreset.AliasMap.Add(new AliasEntry
						{
							Alias = _current.AliasMap[i].Alias,
							Tag = _current.AliasMap[i].Tag
						});
					}
				}

				// 대상 variant 찾기 또는 생성
				PresetVariant targetVariant = FindVariant(targetPreset, _currentVariant.Name);
				if (targetVariant == null)
				{
					targetVariant = new PresetVariant { Name = _currentVariant.Name };
					targetPreset.Variants.Add(targetVariant);
				}

				// 현재 태그값 읽기 (AliasMap 기반)
				targetVariant.Items.Clear();
				int readCount = 0;
				for (int i = 0; i < _currentVariant.Items.Count; i++)
				{
					var srcItem = _currentVariant.Items[i];
					string tag = FindTagByAlias(_current.AliasMap, srcItem.Alias);
					var newItem = new PresetItem { Alias = srcItem.Alias };

					try
					{
						if (!string.IsNullOrEmpty(tag) && TagLib.IsTagExist(tag))
						{
							int[] tagPos = new int[1];
							var tp = TagLib.GetStructPublic(tag, ref tagPos);
							object currObj = tp.GetCurr();
							newItem.Value = currObj != null ? currObj.ToString() : srcItem.Value;
							readCount++;
						}
						else
						{
							newItem.Value = srcItem.Value;
						}
					}
					catch
					{
						newItem.Value = srcItem.Value;
					}

					targetVariant.Items.Add(newItem);
				}

				// JSON 저장
				string json = JsonConvert.SerializeObject(targetPreset, Formatting.Indented);
				File.WriteAllText(filePath, json, new System.Text.UTF8Encoding(true));

				MessageBox.Show(
					string.Format(isKor
						? "Capture 완료. {0}개 태그 값을 '{1}'에 저장했습니다."
						: "Capture done. {0} tag values saved to '{1}'.", readCount, saveName),
					"Capture", MessageBoxButtons.OK, MessageBoxIcon.Information);

				// 목록 갱신
				_isDirty = false;
				LoadPresetList();
				SelectPresetByName(saveName);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					(isKor ? "Capture 실패: " : "Capture failed: ") + ex.Message,
					isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			_isDirty = true;
		}

		private void FormConfigPreset_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_isDirty && _current != null)
			{
				bool isKor = NetTools.Tools.IsLangKorean();
				var dr = MessageBox.Show(
					isKor ? "변경사항을 저장하시겠습니까?" : "Save changes before closing?",
					isKor ? "확인" : "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dr == DialogResult.Yes) SaveCurrentPreset();
				else if (dr == DialogResult.Cancel) { e.Cancel = true; return; }
			}
		}

		#endregion

		#region AliasMap Grid

		private void lblAliasCaption_Click(object sender, EventArgs e)
		{
			ToggleAliasMap();
		}

		private void ToggleAliasMap()
		{
			_aliasMapCollapsed = !_aliasMapCollapsed;
			dgvAliasMap.Visible = !_aliasMapCollapsed;
			btnAddAlias.Visible = !_aliasMapCollapsed;
			btnRemoveAlias.Visible = !_aliasMapCollapsed;
			UpdateAliasCaptionText();
		}

		private void UpdateAliasCaptionText()
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			string arrow = _aliasMapCollapsed ? "▶ " : "▼ ";
			lblAliasCaption.Text = arrow + (isKor ? "별칭 매핑" : "Alias Map");
		}

		/// <summary>
		/// AliasMap 그리드에서 "..." 버튼 클릭 → FormSelectTag 열기
		/// </summary>
		private void dgvAliasMap_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			if (e.ColumnIndex != dgvAliasMap.Columns["SelectTagCol"].Index) return;

			FormSelectTag dialog = new FormSelectTag();
			dialog.bUseTagAI = true;
			dialog.bUseTagAO = true;
			dialog.bUseTagDI = true;
			dialog.bUseTagDO = true;
			dialog.bUseTagST = true;
			dialog.bUseTagGDO = true;

			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				var row = dgvAliasMap.Rows[e.RowIndex];
				row.Cells["TagCol"].Value = dialog.sTag;

				// Alias가 비어있으면 Tag명으로 자동 채움
				string currentAlias = (row.Cells["AliasCol"].Value ?? "").ToString().Trim();
				if (string.IsNullOrEmpty(currentAlias))
					row.Cells["AliasCol"].Value = dialog.sTag;

				_isDirty = true;
			}
		}

		private void dgvAliasMap_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			_isDirty = true;
		}

		private void btnAddAlias_Click(object sender, EventArgs e)
		{
			if (_current == null) return;
			dgvAliasMap.Rows.Add();
			_isDirty = true;
		}

		private void btnRemoveAlias_Click(object sender, EventArgs e)
		{
			if (_current == null) return;
			if (dgvAliasMap.CurrentRow == null) return;

			string alias = (dgvAliasMap.CurrentRow.Cells["AliasCol"].Value ?? "").ToString().Trim();
			dgvAliasMap.Rows.RemoveAt(dgvAliasMap.CurrentRow.Index);

			// 모든 Variant에서 해당 Alias 항목 삭제
			if (!string.IsNullOrEmpty(alias))
			{
				for (int v = 0; v < _current.Variants.Count; v++)
				{
					_current.Variants[v].Items.RemoveAll(
						item => string.Equals(item.Alias, alias, StringComparison.OrdinalIgnoreCase));
				}

				// 현재 variant의 그리드도 갱신
				if (_currentVariant != null)
					LoadItemsToGrid(_currentVariant, GetAliasMapFromGrid());
			}

			_isDirty = true;
		}

		/// <summary>
		/// AliasMap 그리드 → 모델 저장
		/// </summary>
		private void SaveAliasMap()
		{
			if (_current == null) return;
			_current.AliasMap.Clear();
			foreach (DataGridViewRow row in dgvAliasMap.Rows)
			{
				string alias = (row.Cells["AliasCol"].Value ?? "").ToString().Trim();
				if (string.IsNullOrEmpty(alias)) continue;
				_current.AliasMap.Add(new AliasEntry
				{
					Alias = alias,
					Tag = (row.Cells["TagCol"].Value ?? "").ToString().Trim()
				});
			}
		}

		/// <summary>
		/// AliasMap 그리드에서 현재 AliasMap 읽기 (모델 저장 없이)
		/// </summary>
		private List<AliasEntry> GetAliasMapFromGrid()
		{
			var list = new List<AliasEntry>();
			foreach (DataGridViewRow row in dgvAliasMap.Rows)
			{
				string alias = (row.Cells["AliasCol"].Value ?? "").ToString().Trim();
				if (string.IsNullOrEmpty(alias)) continue;
				list.Add(new AliasEntry
				{
					Alias = alias,
					Tag = (row.Cells["TagCol"].Value ?? "").ToString().Trim()
				});
			}
			return list;
		}

		/// <summary>
		/// AliasMap 모델 → 그리드 로드
		/// </summary>
		private void LoadAliasMapToGrid(List<AliasEntry> aliasMap)
		{
			dgvAliasMap.Rows.Clear();
			for (int i = 0; i < aliasMap.Count; i++)
			{
				int rowIdx = dgvAliasMap.Rows.Add();
				dgvAliasMap.Rows[rowIdx].Cells["AliasCol"].Value = aliasMap[i].Alias;
				dgvAliasMap.Rows[rowIdx].Cells["TagCol"].Value = aliasMap[i].Tag;
			}
		}

		#endregion

		#region Variant Selection

		private void tabVariants_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_current == null) return;

			// 현재 variant의 변경사항 반영
			if (_currentVariant != null)
				SaveGridItems();

			int idx = tabVariants.SelectedIndex;
			if (idx < 0 || idx >= _current.Variants.Count)
			{
				_currentVariant = null;
				dgvItems.Rows.Clear();
				return;
			}

			_currentVariant = _current.Variants[idx];
			LoadItemsToGrid(_currentVariant, _current.AliasMap);
		}

		private void btnAddVariant_Click(object sender, EventArgs e)
		{
			if (_current == null)
			{
				bool isKor = NetTools.Tools.IsLangKorean();
				MessageBox.Show(
					isKor ? "먼저 Preset을 선택하세요." : "Select a preset first.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			bool kr = NetTools.Tools.IsLangKorean();
			string name = ShowInputDialog(
				kr ? "Variant 이름을 입력하세요:" : "Enter variant name:",
				kr ? "새 Variant" : "New Variant", "");
			if (string.IsNullOrWhiteSpace(name)) return;

			// 중복 검사
			for (int i = 0; i < _current.Variants.Count; i++)
			{
				if (string.Equals(_current.Variants[i].Name, name, StringComparison.OrdinalIgnoreCase))
				{
					MessageBox.Show(kr ? "이미 존재하는 이름입니다." : "Name already exists.",
						kr ? "중복" : "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
			}

			// 현재 variant 저장
			SaveGridItems();
			SaveAliasMap();

			// 새 Variant 생성 — AliasMap의 모든 alias를 항목으로 추가
			var newVariant = new PresetVariant { Name = name };
			for (int i = 0; i < _current.AliasMap.Count; i++)
			{
				newVariant.Items.Add(new PresetItem
				{
					Alias = _current.AliasMap[i].Alias,
					Value = ""
				});
			}
			_current.Variants.Add(newVariant);

			// UI 갱신
			tabVariants.SelectedIndexChanged -= tabVariants_SelectedIndexChanged;
			tabVariants.TabPages.Add(new TabPage(name));
			tabVariants.SelectedIndexChanged += tabVariants_SelectedIndexChanged;

			tabVariants.SelectedIndex = tabVariants.TabPages.Count - 1;
			_currentVariant = newVariant;
			LoadItemsToGrid(_currentVariant, _current.AliasMap);
			_isDirty = true;
		}

		private void btnDeleteVariant_Click(object sender, EventArgs e)
		{
			if (_current == null || tabVariants.SelectedIndex < 0) return;

			bool isKor = NetTools.Tools.IsLangKorean();
			int idx = tabVariants.SelectedIndex;
			string vName = _current.Variants[idx].Name;

			var dr = MessageBox.Show(
				string.Format(isKor ? "Variant '{0}'을(를) 삭제하시겠습니까?" : "Delete variant '{0}'?", vName),
				isKor ? "삭제 확인" : "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dr != DialogResult.Yes) return;

			_current.Variants.RemoveAt(idx);

			tabVariants.SelectedIndexChanged -= tabVariants_SelectedIndexChanged;
			tabVariants.TabPages.RemoveAt(idx);
			tabVariants.SelectedIndexChanged += tabVariants_SelectedIndexChanged;

			_currentVariant = null;
			dgvItems.Rows.Clear();

			if (tabVariants.TabPages.Count > 0)
			{
				tabVariants.SelectedIndex = 0;
				_currentVariant = _current.Variants[0];
				LoadItemsToGrid(_currentVariant, _current.AliasMap);
			}

			_isDirty = true;
		}

		#endregion

		#region Row Reorder (Up/Down + Drag & Drop)

		private void btnItemUp_Click(object sender, EventArgs e)
		{
			if (dgvItems.CurrentRow == null) return;
			int idx = dgvItems.CurrentRow.Index;
			if (idx <= 0) return;
			int dataRowCount = dgvItems.Rows.Count;
			if (idx >= dataRowCount) return;

			SwapRowValues(idx, idx - 1);
			dgvItems.ClearSelection();
			dgvItems.Rows[idx - 1].Selected = true;
			dgvItems.CurrentCell = dgvItems.Rows[idx - 1].Cells[0];
			_isDirty = true;
		}

		private void btnItemDown_Click(object sender, EventArgs e)
		{
			if (dgvItems.CurrentRow == null) return;
			int idx = dgvItems.CurrentRow.Index;
			int dataRowCount = dgvItems.Rows.Count;
			if (idx < 0 || idx >= dataRowCount - 1) return;

			SwapRowValues(idx, idx + 1);
			dgvItems.ClearSelection();
			dgvItems.Rows[idx + 1].Selected = true;
			dgvItems.CurrentCell = dgvItems.Rows[idx + 1].Cells[0];
			_isDirty = true;
		}

		private void dgvItems_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				var hitTest = dgvItems.HitTest(e.X, e.Y);
				if (hitTest.RowIndex >= 0)
				{
					_dragRowIndex = hitTest.RowIndex;
				}
				else
				{
					_dragRowIndex = -1;
				}
			}
		}

		private void dgvItems_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || _dragRowIndex < 0) return;

			var rect = dgvItems.GetRowDisplayRectangle(_dragRowIndex, false);
			if (rect.IsEmpty) return;

			int dy = Math.Abs(e.Y - (rect.Top + rect.Height / 2));
			if (dy > rect.Height / 2)
			{
				dgvItems.DoDragDrop(_dragRowIndex, DragDropEffects.Move);
			}
		}

		private void dgvItems_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void dgvItems_DragDrop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(typeof(int))) return;

			int fromIndex = (int)e.Data.GetData(typeof(int));
			Point clientPoint = dgvItems.PointToClient(new Point(e.X, e.Y));
			int toIndex = dgvItems.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

			int dataRowCount = dgvItems.Rows.Count;
			if (toIndex < 0 || toIndex >= dataRowCount || toIndex == fromIndex)
			{
				_dragRowIndex = -1;
				return;
			}

			MoveRow(fromIndex, toIndex);
			_dragRowIndex = -1;
		}

		/// <summary>
		/// 행을 fromIndex에서 toIndex로 이동 (인접 행 swap 반복)
		/// </summary>
		private void MoveRow(int fromIndex, int toIndex)
		{
			if (fromIndex == toIndex) return;

			if (fromIndex < toIndex)
			{
				for (int i = fromIndex; i < toIndex; i++)
					SwapRowValues(i, i + 1);
			}
			else
			{
				for (int i = fromIndex; i > toIndex; i--)
					SwapRowValues(i, i - 1);
			}

			dgvItems.ClearSelection();
			dgvItems.Rows[toIndex].Selected = true;
			dgvItems.CurrentCell = dgvItems.Rows[toIndex].Cells[0];
			_isDirty = true;
		}

		/// <summary>
		/// 두 행의 셀 값을 교환
		/// </summary>
		private void SwapRowValues(int idx1, int idx2)
		{
			string[] colNames = new string[] { "AliasName", "SetValue" };
			foreach (string colName in colNames)
			{
				object temp = dgvItems.Rows[idx1].Cells[colName].Value;
				dgvItems.Rows[idx1].Cells[colName].Value = dgvItems.Rows[idx2].Cells[colName].Value;
				dgvItems.Rows[idx2].Cells[colName].Value = temp;
			}
		}

		#endregion

		#region Detail Panel Helpers

		private void ShowDetail(PresetData p)
		{
			if (p == null) { ClearDetailPanel(); return; }

			textBoxName.Text = p.PresetName;
			_currentVariant = null;

			// Load AliasMap grid
			LoadAliasMapToGrid(p.AliasMap);

			// Suppress event during tab rebuild
			tabVariants.SelectedIndexChanged -= tabVariants_SelectedIndexChanged;
			tabVariants.TabPages.Clear();
			dgvItems.Rows.Clear();
			for (int i = 0; i < p.Variants.Count; i++)
			{
				TabPage tp = new TabPage(p.Variants[i].Name);
				tabVariants.TabPages.Add(tp);
			}
			tabVariants.SelectedIndexChanged += tabVariants_SelectedIndexChanged;

			// Auto-select first variant and load items
			if (p.Variants.Count > 0)
			{
				tabVariants.SelectedIndex = 0;
				_currentVariant = p.Variants[0];
				LoadItemsToGrid(_currentVariant, p.AliasMap);
			}

			_isDirty = false;
		}

		private void ClearDetailPanel()
		{
			textBoxName.Text = "";
			dgvAliasMap.Rows.Clear();
			tabVariants.TabPages.Clear();
			dgvItems.Rows.Clear();
			_currentVariant = null;
			_isDirty = false;
		}

		/// <summary>
		/// AliasMap 기준으로 Values 그리드 로드
		/// AliasMap에 정의된 모든 Alias를 표시하고, Variant에 해당 값이 있으면 채움
		/// </summary>
		private void LoadItemsToGrid(PresetVariant variant, List<AliasEntry> aliasMap)
		{
			dgvItems.Rows.Clear();
			for (int i = 0; i < aliasMap.Count; i++)
			{
				string alias = aliasMap[i].Alias;
				string value = "";

				// Variant Items에서 해당 alias의 값 찾기
				for (int j = 0; j < variant.Items.Count; j++)
				{
					if (string.Equals(variant.Items[j].Alias, alias, StringComparison.OrdinalIgnoreCase))
					{
						value = variant.Items[j].Value;
						break;
					}
				}

				int rowIdx = dgvItems.Rows.Add();
				dgvItems.Rows[rowIdx].Cells["AliasName"].Value = alias;
				dgvItems.Rows[rowIdx].Cells["SetValue"].Value = value;
			}
		}

		/// <summary>
		/// Grid의 항목을 현재 variant의 Items에 저장
		/// </summary>
		private void SaveGridItems()
		{
			if (_currentVariant == null) return;

			_currentVariant.Items.Clear();
			foreach (DataGridViewRow row in dgvItems.Rows)
			{
				string alias = (row.Cells["AliasName"].Value ?? "").ToString().Trim();
				if (string.IsNullOrEmpty(alias)) continue;

				_currentVariant.Items.Add(new PresetItem
				{
					Alias = alias,
					Value = (row.Cells["SetValue"].Value ?? "").ToString()
				});
			}
		}

		private void SelectPresetByName(string name)
		{
			foreach (ListViewItem lvi in listViewPresets.Items)
			{
				if (string.Equals(lvi.Text, name, StringComparison.OrdinalIgnoreCase))
				{
					lvi.Selected = true;
					lvi.EnsureVisible();
					break;
				}
			}
		}

		#endregion

		#region CSV Helpers

		/// <summary>
		/// CSV 헤더 행 감지 (파싱된 필드 값 기반)
		/// </summary>
		private static bool IsCsvHeaderLine(string[] cols)
		{
			if (cols.Length >= 4)
			{
				string f0 = cols[0].Trim();
				string f1 = cols[1].Trim();
				if (f0.Equals("Preset", StringComparison.OrdinalIgnoreCase)
					&& f1.Equals("Variant", StringComparison.OrdinalIgnoreCase))
					return true;
			}
			if (cols.Length >= 2)
			{
				string f0 = cols[0].Trim();
				string f1 = cols[1].Trim();
				if (f0.Equals("TagName", StringComparison.OrdinalIgnoreCase)
					&& f1.Equals("Value", StringComparison.OrdinalIgnoreCase))
					return true;
				if (f0.Equals("Name", StringComparison.OrdinalIgnoreCase)
					&& f1.Equals("Description", StringComparison.OrdinalIgnoreCase))
					return true;
				if (f0.Equals("StepOrder", StringComparison.OrdinalIgnoreCase)
					|| f0.Equals("StepName", StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

		private static string EscapeCsv(string val)
		{
			if (string.IsNullOrEmpty(val)) return "";
			if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
				return "\"" + val.Replace("\"", "\"\"") + "\"";
			return val;
		}

		private static string[] ParseCsvLine(string line)
		{
			var fields = new List<string>();
			bool inQuotes = false;
			string field = "";
			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];
				if (inQuotes)
				{
					if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
					{ field += '"'; i++; }
					else if (c == '"') inQuotes = false;
					else field += c;
				}
				else
				{
					if (c == '"') inQuotes = true;
					else if (c == ',') { fields.Add(field); field = ""; }
					else field += c;
				}
			}
			fields.Add(field);
			return fields.ToArray();
		}

		#endregion

		#region Input Dialog

		private static string ShowInputDialog(string prompt, string title, string defaultValue)
		{
			Form frm = new Form
			{
				Width = 350, Height = 150,
				Text = title,
				StartPosition = FormStartPosition.CenterParent,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				MinimizeBox = false, MaximizeBox = false
			};
			Label lbl = new Label { Left = 10, Top = 12, AutoSize = true, Text = prompt };
			TextBox txt = new TextBox { Left = 10, Top = 35, Width = 310, Text = defaultValue };
			Button ok = new Button { Text = "OK", Left = 160, Width = 75, Top = 70, DialogResult = DialogResult.OK };
			Button cancel = new Button { Text = "Cancel", Left = 245, Width = 75, Top = 70, DialogResult = DialogResult.Cancel };
			frm.Controls.AddRange(new Control[] { lbl, txt, ok, cancel });
			frm.AcceptButton = ok;
			frm.CancelButton = cancel;
			return frm.ShowDialog() == DialogResult.OK ? txt.Text : null;
		}

		#endregion

		#region Static Entry Point

		/// <summary>
		/// Studio/LocalMain에서 호출하는 정적 진입점
		/// </summary>
		public static void ConfigPreset()
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			// 권한 체크
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN &&
				!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_VIEW))
			{
				MessageBox.Show(
					isKor ? "프리셋 설정 열람 권한이 없습니다." : "No permission to view preset configuration.",
					isKor ? "권한 오류" : "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			FormConfigPreset dialog = new FormConfigPreset();
			dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog();
		}

		#endregion
	}
}

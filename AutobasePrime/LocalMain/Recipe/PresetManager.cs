using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AutoLibLocal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// Preset 관리자 — JSON 파일 기반, DB 미사용
	/// JSON 포맷: { preset_name, alias_map: [{ alias, tag }], variants: [{ name, items: [{ alias, value }] }] }
	/// </summary>
	public static class PresetManager
	{
		static string _presetDir;
		static readonly object _lock = new object();

		/// <summary>
		/// Preset 저장 디렉토리 (프로젝트 경로 하위)
		/// </summary>
		public static string PresetDirectory
		{
			get
			{
				if (_presetDir == null)
				{
					string projectPath = TotalConfig.sDirWorkProject;
					if (string.IsNullOrEmpty(projectPath))
						projectPath = AppDomain.CurrentDomain.BaseDirectory;
					_presetDir = Path.Combine(projectPath, "Presets");
				}
				if (!Directory.Exists(_presetDir))
					Directory.CreateDirectory(_presetDir);
				return _presetDir;
			}
		}

		#region Preset Data Model

		/// <summary>
		/// Alias ↔ Tag 매핑 항목
		/// </summary>
		public class AliasEntry
		{
			[JsonProperty("alias")] public string Alias = "";
			[JsonProperty("tag")]   public string Tag = "";
		}

		/// <summary>
		/// Preset 데이터 모델
		/// </summary>
		public class PresetData
		{
			[JsonProperty("preset_name")]
			public string PresetName = "";

			[JsonProperty("alias_map")]
			public List<AliasEntry> AliasMap = new List<AliasEntry>();

			[JsonProperty("variants")]
			public List<PresetVariant> Variants = new List<PresetVariant>();
		}

		/// <summary>
		/// Variant — 하나의 프리셋 내 변형
		/// </summary>
		public class PresetVariant
		{
			[JsonProperty("name")]
			public string Name = "";

			[JsonProperty("items")]
			public List<PresetItem> Items = new List<PresetItem>();
		}

		/// <summary>
		/// Preset Item — Alias 기반 값 항목
		/// </summary>
		public class PresetItem
		{
			[JsonProperty("alias")]
			public string Alias = "";

			[JsonProperty("value")]
			public string Value = "";
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

		#endregion

		#region File I/O

		/// <summary>
		/// JSON 문자열에서 PresetData 로드 (3단계 포맷 자동 마이그레이션)
		/// 1. 최신 포맷: alias_map 존재 → 그대로 사용
		/// 2. 중간 포맷: preset_name + variants (tag 필드) → AliasMap 자동 생성
		/// 3. 구 포맷: name + items → Default variant + AliasMap 생성
		/// </summary>
		public static PresetData DeserializePreset(string json)
		{
			var jobj = JObject.Parse(json);

			// 최신 포맷: alias_map 필드 존재
			if (jobj["alias_map"] != null)
			{
				return jobj.ToObject<PresetData>();
			}

			// 중간 포맷: preset_name 존재, alias_map 없음
			if (jobj["preset_name"] != null)
			{
				var mid = jobj.ToObject<MidPresetData>();
				return MigrateFromMidFormat(mid);
			}

			// 구 포맷: name + items 필드 존재 (variants 없음)
			if (jobj["name"] != null && jobj["items"] != null)
			{
				var old = jobj.ToObject<OldPresetData>();
				return MigrateFromOldFormat(old);
			}

			return null;
		}

		/// <summary>
		/// 중간 포맷 → 최신 포맷 마이그레이션
		/// tag 값을 alias와 tag 양쪽에 설정하여 AliasMap 자동 생성
		/// </summary>
		static PresetData MigrateFromMidFormat(MidPresetData mid)
		{
			var preset = new PresetData { PresetName = mid.PresetName };

			// 모든 variant의 tag를 수집하여 AliasMap 생성
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

			// AliasMap: tag → alias (초기값은 tag명과 동일)
			for (int i = 0; i < tagOrder.Count; i++)
			{
				preset.AliasMap.Add(new AliasEntry { Alias = tagOrder[i], Tag = tagOrder[i] });
			}

			// Variants: tag → alias 변환
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

		/// <summary>
		/// 구 포맷 → 최신 포맷 마이그레이션
		/// name+items → Default variant + AliasMap 생성
		/// </summary>
		static PresetData MigrateFromOldFormat(OldPresetData old)
		{
			var preset = new PresetData { PresetName = old.Name };

			// AliasMap 생성
			for (int i = 0; i < old.Items.Count; i++)
			{
				string tag = old.Items[i].Tag;
				if (!string.IsNullOrEmpty(tag))
				{
					preset.AliasMap.Add(new AliasEntry { Alias = tag, Tag = tag });
				}
			}

			// Default variant 생성
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
		/// Preset 목록 로드 (디렉토리의 모든 .json 파일)
		/// </summary>
		public static List<PresetData> LoadPresetList()
		{
			var list = new List<PresetData>();
			try
			{
				string dir = PresetDirectory;
				string[] files = Directory.GetFiles(dir, "*.json");
				for (int i = 0; i < files.Length; i++)
				{
					try
					{
						string json = File.ReadAllText(files[i], System.Text.Encoding.UTF8);
						var preset = DeserializePreset(json);
						if (preset != null)
							list.Add(preset);
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Preset load failed [{files[i]}]: {ex.Message}");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"PresetManager.LoadPresetList error: {ex.Message}");
			}
			return list;
		}

		/// <summary>
		/// Preset 저장 (PresetName 기반 파일명)
		/// </summary>
		public static bool SavePreset(PresetData preset, out string error)
		{
			error = null;
			try
			{
				if (string.IsNullOrWhiteSpace(preset.PresetName))
				{
					error = "Preset name is required.";
					return false;
				}

				string fileName = SanitizeFileName(preset.PresetName) + ".json";
				string filePath = Path.Combine(PresetDirectory, fileName);

				string json = JsonConvert.SerializeObject(preset, Formatting.Indented);
				lock (_lock)
				{
					File.WriteAllText(filePath, json, new System.Text.UTF8Encoding(true));
				}
				return true;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				return false;
			}
		}

		/// <summary>
		/// Preset 삭제
		/// </summary>
		public static bool DeletePreset(string presetName, out string error)
		{
			error = null;
			try
			{
				string fileName = SanitizeFileName(presetName) + ".json";
				string filePath = Path.Combine(PresetDirectory, fileName);
				if (File.Exists(filePath))
				{
					lock (_lock)
					{
						File.Delete(filePath);
					}
					return true;
				}
				error = "Preset file not found.";
				return false;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				return false;
			}
		}

		/// <summary>
		/// Preset 로드 (이름으로, 구 포맷 자동 마이그레이션)
		/// </summary>
		public static PresetData LoadPreset(string presetName)
		{
			try
			{
				string fileName = SanitizeFileName(presetName) + ".json";
				string filePath = Path.Combine(PresetDirectory, fileName);
				if (!File.Exists(filePath)) return null;

				string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
				return DeserializePreset(json);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"PresetManager.LoadPreset error: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// JSON 원문 반환 (웹서비스용)
		/// </summary>
		public static string LoadPresetRaw(string presetName)
		{
			try
			{
				string fileName = SanitizeFileName(presetName) + ".json";
				string filePath = Path.Combine(PresetDirectory, fileName);
				if (!File.Exists(filePath)) return null;
				return File.ReadAllText(filePath, System.Text.Encoding.UTF8);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"PresetManager.LoadPresetRaw error: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// JSON 원문 저장 (웹서비스용)
		/// </summary>
		public static bool SavePresetRaw(string presetName, string json, out string error)
		{
			error = null;
			try
			{
				string fileName = SanitizeFileName(presetName) + ".json";
				string filePath = Path.Combine(PresetDirectory, fileName);
				lock (_lock)
				{
					File.WriteAllText(filePath, json, new System.Text.UTF8Encoding(true));
				}
				return true;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				return false;
			}
		}

		/// <summary>
		/// 프리셋 이름+날짜 목록 반환 (웹서비스용)
		/// </summary>
		public static List<(string name, string date)> GetPresetNameList()
		{
			var result = new List<(string, string)>();
			try
			{
				string dir = PresetDirectory;
				string[] files = Directory.GetFiles(dir, "*.json");
				for (int i = 0; i < files.Length; i++)
				{
					string name = Path.GetFileNameWithoutExtension(files[i]);
					string date = File.GetLastWriteTime(files[i]).ToString("yyyy-MM-dd HH:mm");
					result.Add((name, date));
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"PresetManager.GetPresetNameList error: {ex.Message}");
			}
			return result;
		}

		#endregion

		#region Variant / Alias Helpers

		/// <summary>
		/// Preset 내에서 이름으로 Variant 찾기
		/// </summary>
		public static PresetVariant FindVariant(PresetData preset, string variantName)
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
		public static string FindTagByAlias(List<AliasEntry> aliasMap, string alias)
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

		#region CSV Export/Import

		/// <summary>
		/// Preset을 CSV로 내보내기 (전체 Variant 포함)
		/// 포맷: Preset,Variant,Alias,Tag,Value (5컬럼)
		/// </summary>
		public static bool ExportToCsv(PresetData preset, string filePath, out string error)
		{
			error = null;
			try
			{
				using (var writer = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(true)))
				{
					writer.WriteLine("Preset,Variant,Alias,Tag,Value");
					for (int v = 0; v < preset.Variants.Count; v++)
					{
						var variant = preset.Variants[v];
						for (int i = 0; i < variant.Items.Count; i++)
						{
							var item = variant.Items[i];
							string tag = FindTagByAlias(preset.AliasMap, item.Alias) ?? "";
							writer.WriteLine("{0},{1},{2},{3},{4}",
								CsvEscape(preset.PresetName),
								CsvEscape(variant.Name),
								CsvEscape(item.Alias),
								CsvEscape(tag),
								CsvEscape(item.Value));
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				return false;
			}
		}

		/// <summary>
		/// CSV에서 Preset 가져오기
		/// 포맷: Preset,Variant,Alias,Tag,Value (5컬럼)
		/// 또는 Preset,Variant,Tag,Value (4컬럼, 중간 포맷)
		/// 또는 구 포맷: TagName,Value (2컬럼) → "Default" variant로 변환
		/// </summary>
		public static PresetData ImportFromCsv(string filePath, out string error)
		{
			error = null;
			try
			{
				if (!File.Exists(filePath))
				{
					error = "File not found.";
					return null;
				}

				string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);

				// Variant별 아이템 그룹핑
				var variantMap = new Dictionary<string, List<PresetItem>>(StringComparer.OrdinalIgnoreCase);
				var aliasTagMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				string presetName = "";

				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i].Trim();
					if (string.IsNullOrEmpty(line)) continue;

					// 섹션 구분자 스킵
					if (line.StartsWith("[")) continue;

					string[] fields = CsvParseLine(line);

					// 헤더 행 감지 (파싱 후 필드 값 기반 검사)
					if (IsCsvHeaderLine(fields)) continue;

					if (fields.Length >= 5)
					{
						// 최신 포맷: Preset,Variant,Alias,Tag,Value
						if (string.IsNullOrEmpty(presetName))
							presetName = fields[0].Trim();

						string vName = fields[1].Trim();
						string alias = fields[2].Trim();
						string tag = fields[3].Trim();
						string val = fields[4].Trim();

						if (!string.IsNullOrEmpty(alias) && !aliasTagMap.ContainsKey(alias))
							aliasTagMap[alias] = tag;

						if (!variantMap.ContainsKey(vName))
							variantMap[vName] = new List<PresetItem>();

						variantMap[vName].Add(new PresetItem { Alias = alias, Value = val });
					}
					else if (fields.Length >= 4)
					{
						// 중간 포맷: Preset,Variant,Tag,Value
						if (string.IsNullOrEmpty(presetName))
							presetName = fields[0].Trim();

						string vName = fields[1].Trim();
						string tag = fields[2].Trim();
						string val = fields[3].Trim();

						if (!string.IsNullOrEmpty(tag) && !aliasTagMap.ContainsKey(tag))
							aliasTagMap[tag] = tag;

						if (!variantMap.ContainsKey(vName))
							variantMap[vName] = new List<PresetItem>();

						variantMap[vName].Add(new PresetItem { Alias = tag, Value = val });
					}
					else if (fields.Length >= 2)
					{
						// 구 포맷: TagName,Value → Default variant
						string vName = "Default";
						string tag = fields[0].Trim();

						if (!string.IsNullOrEmpty(tag) && !aliasTagMap.ContainsKey(tag))
							aliasTagMap[tag] = tag;

						if (!variantMap.ContainsKey(vName))
							variantMap[vName] = new List<PresetItem>();

						variantMap[vName].Add(new PresetItem
						{
							Alias = tag,
							Value = fields[1].Trim()
						});
					}
				}

				var preset = new PresetData { PresetName = presetName };

				// AliasMap 생성
				foreach (var kvp in aliasTagMap)
				{
					preset.AliasMap.Add(new AliasEntry { Alias = kvp.Key, Tag = kvp.Value });
				}

				// Variants 생성
				foreach (var kvp in variantMap)
				{
					preset.Variants.Add(new PresetVariant
					{
						Name = kvp.Key,
						Items = kvp.Value
					});
				}

				return preset;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				return null;
			}
		}

		#endregion

		#region Capture (현재 태그값 → Preset 저장)

		/// <summary>
		/// 지정한 Variant의 Alias 목록을 기준으로 현재 태그값을 읽어서 저장 (Capture)
		/// AliasMap을 사용하여 Alias → Tag 변환 후 태그값 읽기
		/// </summary>
		public static async System.Threading.Tasks.Task<(bool success, string error)> CaptureAsPreset(
			PresetData template, string variantName, string saveName)
		{
			try
			{
				var srcVariant = FindVariant(template, variantName);
				if (srcVariant == null)
					return (false, "Variant '" + variantName + "' not found.");

				// saveName 프리셋이 이미 있으면 로드, 없으면 새로 생성
				var targetPreset = LoadPreset(saveName);
				if (targetPreset == null)
				{
					targetPreset = new PresetData { PresetName = saveName };
					// AliasMap 복사
					for (int i = 0; i < template.AliasMap.Count; i++)
					{
						targetPreset.AliasMap.Add(new AliasEntry
						{
							Alias = template.AliasMap[i].Alias,
							Tag = template.AliasMap[i].Tag
						});
					}
				}

				// 대상 variant 찾기 또는 생성
				var targetVariant = FindVariant(targetPreset, variantName);
				if (targetVariant == null)
				{
					targetVariant = new PresetVariant { Name = variantName };
					targetPreset.Variants.Add(targetVariant);
				}

				// 현재 태그값 읽기 (AliasMap 기반)
				targetVariant.Items.Clear();
				for (int i = 0; i < srcVariant.Items.Count; i++)
				{
					var srcItem = srcVariant.Items[i];
					string tag = FindTagByAlias(template.AliasMap, srcItem.Alias);
					var newItem = new PresetItem { Alias = srcItem.Alias };

					try
					{
						if (!string.IsNullOrEmpty(tag) && TagLib.IsTagExist(tag))
						{
							int[] tagPos = new int[1];
							var tp = TagLib.GetStructPublic(tag, ref tagPos);
							object currObj = tp.GetCurr();
							newItem.Value = currObj != null ? currObj.ToString() : srcItem.Value;
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

				string err;
				bool ok = SavePreset(targetPreset, out err);
				return (ok, err);
			}
			catch (Exception ex)
			{
				return (false, ex.Message);
			}
		}

		#endregion

		#region Helpers

		static string SanitizeFileName(string name)
		{
			char[] invalid = Path.GetInvalidFileNameChars();
			string result = name;
			for (int i = 0; i < invalid.Length; i++)
				result = result.Replace(invalid[i], '_');
			return result;
		}

		/// <summary>
		/// CSV 헤더 행 감지 (파싱된 필드 값 기반)
		/// StartsWith 대신 정확한 필드 값 비교로 데이터 행 오탐 방지
		/// </summary>
		static bool IsCsvHeaderLine(string[] cols)
		{
			if (cols.Length >= 5)
			{
				// 최신 포맷 헤더: Preset,Variant,Alias,Tag,Value
				string f0 = cols[0].Trim();
				string f1 = cols[1].Trim();
				if (f0.Equals("Preset", StringComparison.OrdinalIgnoreCase)
					&& f1.Equals("Variant", StringComparison.OrdinalIgnoreCase))
					return true;
			}
			if (cols.Length >= 4)
			{
				// 중간 포맷 헤더: Preset,Variant,Tag,Value
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
				// 구 포맷 헤더: TagName,Value
				if (f0.Equals("TagName", StringComparison.OrdinalIgnoreCase)
					&& f1.Equals("Value", StringComparison.OrdinalIgnoreCase))
					return true;
				// 구 레시피 헤더: Name,Description / StepOrder,... / StepName,...
				if (f0.Equals("Name", StringComparison.OrdinalIgnoreCase)
					&& f1.Equals("Description", StringComparison.OrdinalIgnoreCase))
					return true;
				if (f0.Equals("StepOrder", StringComparison.OrdinalIgnoreCase)
					|| f0.Equals("StepName", StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

		static string CsvEscape(string val)
		{
			if (string.IsNullOrEmpty(val)) return "";
			if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
				return "\"" + val.Replace("\"", "\"\"") + "\"";
			return val;
		}

		static string[] CsvParseLine(string line)
		{
			var fields = new List<string>();
			bool inQuote = false;
			string current = "";

			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];
				if (inQuote)
				{
					if (c == '"')
					{
						if (i + 1 < line.Length && line[i + 1] == '"')
						{
							current += '"';
							i++;
						}
						else
						{
							inQuote = false;
						}
					}
					else
					{
						current += c;
					}
				}
				else
				{
					if (c == '"') inQuote = true;
					else if (c == ',') { fields.Add(current); current = ""; }
					else current += c;
				}
			}
			fields.Add(current);
			return fields.ToArray();
		}

		#endregion
	}
}

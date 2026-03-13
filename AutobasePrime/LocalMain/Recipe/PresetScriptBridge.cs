using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;
using Newtonsoft.Json;
using PresetData = LocalMain.PresetManager.PresetData;
using PresetVariant = LocalMain.PresetManager.PresetVariant;
using PresetItem = LocalMain.PresetManager.PresetItem;
using AliasEntry = LocalMain.PresetManager.AliasEntry;

namespace LocalMain
{
	/// <summary>
	/// Preset 스크립트 함수 브리지 — ScriptFunctionPreset 델리게이트 구현
	/// DataGate (bLocalFlag 분기) 경유로 Preset 로드/저장
	/// AliasMap 기반: Alias → Tag 변환 후 태그 읽기/쓰기
	/// </summary>
	internal static class PresetScriptBridge
	{
		/// <summary>
		/// DataGate를 통해 Preset JSON을 가져와 PresetData로 역직렬화
		/// </summary>
		static PresetData LoadPresetViaDataGate(string presetName)
		{
			var gate = new DataGate();
			string json = gate.PresetGet(presetName);
			if (string.IsNullOrEmpty(json)) return null;
			return PresetManager.DeserializePreset(json);
		}

		/// <summary>
		/// PresetData를 JSON으로 직렬화하여 DataGate를 통해 저장
		/// </summary>
		static (bool success, string error) SavePresetViaDataGate(string presetName, PresetData preset)
		{
			try
			{
				string json = JsonConvert.SerializeObject(preset, Formatting.Indented);
				var gate = new DataGate();
				return gate.PresetSave(presetName, json);
			}
			catch (Exception ex)
			{
				return (false, ex.Message);
			}
		}

		/// <summary>
		/// Preset Apply: Preset의 특정 Variant 값을 태그에 쓰기
		/// AliasMap을 사용하여 Alias → Tag 변환 후 SetTagValue 호출
		/// </summary>
		public static async Task<(bool success, string error)> PresetApply(string presetName, string variantName)
		{
			try
			{
				var preset = LoadPresetViaDataGate(presetName);
				if (preset == null)
					return (false, "Preset not found: " + presetName);

				var variant = PresetManager.FindVariant(preset, variantName);
				if (variant == null)
					return (false, "Variant not found: " + variantName);

				int failCount = 0;
				string lastError = null;

				for (int i = 0; i < variant.Items.Count; i++)
				{
					var item = variant.Items[i];
					try
					{
						// AliasMap에서 Alias → Tag 변환
						string tag = PresetManager.FindTagByAlias(preset.AliasMap, item.Alias);
						if (string.IsNullOrEmpty(tag))
						{
							failCount++;
							lastError = item.Alias + ": Alias not found in AliasMap";
							Debug.WriteLine("PresetApply: Alias not in AliasMap - " + item.Alias);
							continue;
						}

						if (!TagLib.IsTagExist(tag))
						{
							failCount++;
							lastError = item.Alias + " (" + tag + "): Tag not found";
							Debug.WriteLine("PresetApply: Tag not found - " + tag + " (alias: " + item.Alias + ")");
							continue;
						}
						await PlcScan.SetTagValue(tag, item.Value, false);
					}
					catch (Exception ex)
					{
						failCount++;
						lastError = item.Alias + ": " + ex.Message;
						Debug.WriteLine("PresetApply tag error [" + item.Alias + "]: " + ex.Message);
					}
				}

				if (failCount > 0)
					return (false, failCount + " tag(s) failed. Last: " + lastError);

				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, ex.Message);
			}
		}

		/// <summary>
		/// Preset Capture: 현재 태그값을 읽어 Preset의 특정 Variant로 저장
		/// DataGate 경유로 template/target 로드 및 저장
		/// </summary>
		public static async Task<(bool success, string error)> PresetCapture(
			string presetName, string variantName, string saveName)
		{
			try
			{
				var template = LoadPresetViaDataGate(presetName);
				if (template == null)
					return (false, "Template preset not found: " + presetName);

				var srcVariant = PresetManager.FindVariant(template, variantName);
				if (srcVariant == null)
					return (false, "Variant '" + variantName + "' not found.");

				// saveName 프리셋이 이미 있으면 로드, 없으면 새로 생성
				var targetPreset = LoadPresetViaDataGate(saveName);
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
				var targetVariant = PresetManager.FindVariant(targetPreset, variantName);
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
					string tag = PresetManager.FindTagByAlias(template.AliasMap, srcItem.Alias);
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

				var saveResult = SavePresetViaDataGate(saveName, targetPreset);
				return (saveResult.success, saveResult.error);
			}
			catch (Exception ex)
			{
				return (false, ex.Message);
			}
		}

		/// <summary>
		/// Preset 이름 목록 반환 (DataGate 경유)
		/// </summary>
		public static List<string> PresetGetList()
		{
			var result = new List<string>();
			try
			{
				var gate = new DataGate();
				var list = gate.PresetGetList();
				for (int i = 0; i < list.Count; i++)
					result.Add(list[i].name);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("PresetScriptBridge.PresetGetList error: " + ex.Message);
			}
			return result;
		}

		/// <summary>
		/// 특정 Preset의 Variant 개수 반환 (DataGate 경유)
		/// </summary>
		public static int PresetGetVariantCount(string presetName)
		{
			try
			{
				var preset = LoadPresetViaDataGate(presetName);
				if (preset == null) return 0;
				return preset.Variants.Count;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("PresetScriptBridge.PresetGetVariantCount error: " + ex.Message);
				return 0;
			}
		}

		/// <summary>
		/// 특정 Preset의 Variant 이름 반환 (인덱스 기반, DataGate 경유)
		/// </summary>
		public static string PresetGetVariantName(string presetName, int index)
		{
			try
			{
				var preset = LoadPresetViaDataGate(presetName);
				if (preset == null) return "";
				if (index < 0 || index >= preset.Variants.Count) return "";
				return preset.Variants[index].Name;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("PresetScriptBridge.PresetGetVariantName error: " + ex.Message);
				return "";
			}
		}
	}
}

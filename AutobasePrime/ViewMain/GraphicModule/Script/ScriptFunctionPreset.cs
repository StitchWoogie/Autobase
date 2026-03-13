using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// Preset 스크립트 함수 — JSON 파일 기반 Quick Mode 레시피
	/// PresetApply("Recipe1", "Chicken") 형태로 Variant 지정 적용
	/// </summary>
	public class ScriptFunctionPreset
	{
		// Delegates (LocalMain에서 구현 연결)
		public delegate Task<(bool success, string error)> DelegatePresetApply(string presetName, string variantName);
		public delegate Task<(bool success, string error)> DelegatePresetCapture(string presetName, string variantName, string saveName);
		public delegate List<string> DelegatePresetGetList();
		public delegate void DelegatePresetReLoad();
		public delegate int DelegatePresetGetVariantCount(string presetName);
		public delegate string DelegatePresetGetVariantName(string presetName, int index);

		public static DelegatePresetApply procPresetApply = null;
		public static DelegatePresetCapture procPresetCapture = null;
		public static DelegatePresetGetList procPresetGetList = null;
		public static DelegatePresetReLoad procPresetReLoad = null;
		public static DelegatePresetGetVariantCount procPresetGetVariantCount = null;
		public static DelegatePresetGetVariantName procPresetGetVariantName = null;

		// 목록 캐시 (스크립트에서 인덱스로 접근)
		static List<string> _cachedList = new List<string>();

		#region Script Functions

		/// <summary>
		/// PresetApply(preset_name, variant_name) → 1 on success
		/// </summary>
		static int Run_PresetApply(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string presetName = (string)args[0];
			string variantName = (string)args[1];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procPresetApply != null)
				{
					var task = procPresetApply(presetName, variantName);
					task.ContinueWith(t =>
					{
						if (t.IsFaulted)
							System.Diagnostics.Debug.WriteLine("PresetApply error: " + t.Exception?.InnerException?.Message);
					}, TaskContinuationOptions.OnlyOnFaulted);
					args[2] = "";
					val = 1;
				}
			}

			return 1;
		}

		/// <summary>
		/// PresetCapture(preset_name, variant_name, save_name) → 1 on success
		/// </summary>
		static int Run_PresetCapture(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string presetName = (string)args[0];
			string variantName = (string)args[1];
			string saveName = (string)args[2];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procPresetCapture != null)
				{
					var task = procPresetCapture(presetName, variantName, saveName);
					task.ContinueWith(t =>
					{
						if (t.IsFaulted)
							System.Diagnostics.Debug.WriteLine("PresetCapture error: " + t.Exception?.InnerException?.Message);
					}, TaskContinuationOptions.OnlyOnFaulted);
					args[3] = "";
					val = 1;
				}
			}

			return 1;
		}

		static int Run_PresetGetList(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				try
				{
					if (procPresetGetList != null)
					{
						_cachedList = procPresetGetList();
						val = _cachedList.Count;
					}
				}
				catch
				{
					val = 0;
				}
			}

			return 1;
		}

		static int Run_PresetGetName(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = "";
			int index = (int)args[0];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (index >= 0 && index < _cachedList.Count)
				{
					val = _cachedList[index];
					args[1] = _cachedList[index];
				}
			}

			return 1;
		}

		/// <summary>
		/// PresetGetVariantCount(preset_name) → int (variant 개수)
		/// </summary>
		static int Run_PresetGetVariantCount(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string presetName = (string)args[0];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procPresetGetVariantCount != null)
				{
					val = procPresetGetVariantCount(presetName);
				}
			}

			return 1;
		}

		/// <summary>
		/// PresetGetVariantName(preset_name, index) → string (variant 이름)
		/// </summary>
		static int Run_PresetGetVariantName(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = "";
			string presetName = (string)args[0];
			int index = (int)args[1];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procPresetGetVariantName != null)
				{
					string name = procPresetGetVariantName(presetName, index);
					val = name;
					args[2] = name;
				}
			}

			return 1;
		}

		#endregion

		public static void PrepareMethod(ScriptExternalRun prepare)
		{
			string prename = "Preset";

			prepare.AddMethod(prename, "PresetApply", "int",
				new ScriptExternalRun.DeleMethod(Run_PresetApply),
				"in:string:preset_name", "in:string:variant_name", "out:string:error");

			prepare.AddMethod(prename, "PresetCapture", "int",
				new ScriptExternalRun.DeleMethod(Run_PresetCapture),
				"in:string:preset_name", "in:string:variant_name", "in:string:save_name", "out:string:error");

			prepare.AddMethod(prename, "PresetGetList", "int",
				new ScriptExternalRun.DeleMethod(Run_PresetGetList));

			prepare.AddMethod(prename, "PresetGetName", "string",
				new ScriptExternalRun.DeleMethod(Run_PresetGetName),
				"in:int:index", "out:string:name");


			prepare.AddMethod(prename, "PresetGetVariantCount", "int",
				new ScriptExternalRun.DeleMethod(Run_PresetGetVariantCount),
				"in:string:preset_name");

			prepare.AddMethod(prename, "PresetGetVariantName", "string",
				new ScriptExternalRun.DeleMethod(Run_PresetGetVariantName),
				"in:string:preset_name", "in:int:index", "out:string:variant_name");
		}
	}
}

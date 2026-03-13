using System;
using System.Threading.Tasks;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// 레시피 스크립트 함수 (ISA-88 Lite: Unit 지원)
	/// </summary>
	public class ScriptFunctionRecipe
	{
		// 기존 Delegates (하위호환)
		public delegate Task<(bool success, string error)> DelegateRecipeDownload(string recipeName);
		public delegate Task<(bool success, string error)> DelegateRecipeUpload(string recipeName);
		public delegate void DelegateRecipeReLoad();

		// 신규 Delegates (Unit 대응)
		public delegate Task<(bool success, string error)> DelegateRecipeDownloadUnit(string recipeName, string unitName);
		public delegate Task<(bool success, string error)> DelegateRecipeUploadUnit(string recipeName, string unitName);
		public delegate bool DelegateRecipeIsExecutingCheck(string recipeName, string unitName);
		public delegate bool DelegateRecipeIsExecutingAny();

		// ISA-88 Batch 제어 Delegates
		public delegate Task<(bool success, string error, string batchId)> DelegateRecipeStart(string recipeName, string batchId, string username);
		public delegate bool DelegateRecipeHold(string recipeName);
		public delegate bool DelegateRecipeRestart(string recipeName);
		public delegate bool DelegateRecipeAbort(string recipeName);
		public delegate string DelegateRecipeGetState(string recipeName);

		public static DelegateRecipeDownload procRecipeDownload = null;
		public static DelegateRecipeUpload procRecipeUpload = null;
		public static DelegateRecipeReLoad procRecipeReLoad = null;
		public static DelegateRecipeDownloadUnit procRecipeDownloadUnit = null;
		public static DelegateRecipeUploadUnit procRecipeUploadUnit = null;
		public static DelegateRecipeIsExecutingCheck procRecipeIsExecutingCheck = null;
		public static DelegateRecipeIsExecutingAny procRecipeIsExecutingAny = null;

		// ISA-88 Batch 제어
		public delegate Task<(bool success, string error, string batchId)> DelegateRecipeStartUnit(string recipeName, string batchId, string username, string unitName);
		public static DelegateRecipeStart procRecipeStart = null;
		public static DelegateRecipeStartUnit procRecipeStartUnit = null;
		public static DelegateRecipeHold procRecipeHold = null;
		public static DelegateRecipeRestart procRecipeRestart = null;
		public static DelegateRecipeAbort procRecipeAbort = null;
		public static DelegateRecipeGetState procRecipeGetState = null;

		// 목록 캐시 (스크립트에서 인덱스로 접근)
		static System.Collections.ArrayList _cachedList = new System.Collections.ArrayList();

		#region 기존 함수 (하위호환)

		static int Run_RecipeDownload(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeDownload != null)
				{
					// Fire-and-Forget: 실행 요청만 하고 즉시 반환 (스크립트 스레드 차단 방지)
					var task = procRecipeDownload(recipeName);
					task.ContinueWith(t =>
					{
						if (t.IsFaulted)
							System.Diagnostics.Debug.WriteLine("RecipeDownload error: " + t.Exception?.InnerException?.Message);
					}, TaskContinuationOptions.OnlyOnFaulted);
					args[1] = "";  // 비동기 실행 중 (오류는 RecipeIsExecuting으로 확인)
					val = 1;       // 실행 요청 수락됨
				}
			}

			return 1;
		}

		static int Run_RecipeUpload(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeUpload != null)
				{
					// Fire-and-Forget: 실행 요청만 하고 즉시 반환
					var task = procRecipeUpload(recipeName);
					task.ContinueWith(t =>
					{
						if (t.IsFaulted)
							System.Diagnostics.Debug.WriteLine("RecipeUpload error: " + t.Exception?.InnerException?.Message);
					}, TaskContinuationOptions.OnlyOnFaulted);
					args[1] = "";
					val = 1;
				}
			}

			return 1;
		}

		static int Run_RecipeGetList(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				try
				{
					var db = DataPostgres.Instance;
					if (db != null)
					{
						var task = db.GetRecipeListAsync();
						task.Wait();
						_cachedList = task.Result;
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

		static int Run_RecipeGetName(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = "";
			int index = (int)args[0];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (index >= 0 && index < _cachedList.Count)
				{
					var info = (RecipeInfo)_cachedList[index];
					val = info.recipe_name;
					args[1] = info.recipe_name;
				}
			}

			return 1;
		}

		static int Run_RecipeIsExecuting(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeIsExecutingAny != null)
				{
					val = procRecipeIsExecutingAny() ? 1 : 0;
				}
			}

			return 1;
		}

		static int Run_RecipeReLoad(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeReLoad != null)
				{
					procRecipeReLoad();
				}
			}

			return 1;
		}

		#endregion

		#region 신규 함수 (Unit 대응)

		static int Run_RecipeDownloadUnit(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];
			string unitName = (string)args[1];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeDownloadUnit != null)
				{
					// Fire-and-Forget: 실행 요청만 하고 즉시 반환
					string unit = string.IsNullOrEmpty(unitName) ? null : unitName;
					var task = procRecipeDownloadUnit(recipeName, unit);
					task.ContinueWith(t =>
					{
						if (t.IsFaulted)
							System.Diagnostics.Debug.WriteLine("RecipeDownloadUnit error: " + t.Exception?.InnerException?.Message);
					}, TaskContinuationOptions.OnlyOnFaulted);
					args[2] = "";
					val = 1;
				}
			}

			return 1;
		}

		static int Run_RecipeUploadUnit(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];
			string unitName = (string)args[1];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeUploadUnit != null)
				{
					// Fire-and-Forget: 실행 요청만 하고 즉시 반환
					string unit = string.IsNullOrEmpty(unitName) ? null : unitName;
					var task = procRecipeUploadUnit(recipeName, unit);
					task.ContinueWith(t =>
					{
						if (t.IsFaulted)
							System.Diagnostics.Debug.WriteLine("RecipeUploadUnit error: " + t.Exception?.InnerException?.Message);
					}, TaskContinuationOptions.OnlyOnFaulted);
					args[2] = "";
					val = 1;
				}
			}

			return 1;
		}

		static int Run_RecipeIsExecutingUnit(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];
			string unitName = (string)args[1];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeIsExecutingCheck != null)
				{
					string unit = string.IsNullOrEmpty(unitName) ? null : unitName;
					val = procRecipeIsExecutingCheck(recipeName, unit) ? 1 : 0;
				}
			}

			return 1;
		}

		#endregion

		#region ISA-88 Batch 제어 함수

		static int Run_RecipeStart(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];
			string batchId = args.Length > 1 ? (string)args[1] : "";

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeStart != null)
				{
					string bid = string.IsNullOrEmpty(batchId) ? null : batchId;
					var task = procRecipeStart(recipeName, bid, null);
					task.ContinueWith(t =>
					{
						if (t.IsFaulted)
							System.Diagnostics.Debug.WriteLine("RecipeStart error: " + t.Exception?.InnerException?.Message);
					}, TaskContinuationOptions.OnlyOnFaulted);
					val = 1;
				}
			}

			return 1;
		}

		static int Run_RecipeHold(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeHold != null)
					val = procRecipeHold(recipeName) ? 1 : 0;
			}

			return 1;
		}

		static int Run_RecipeRestart(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeRestart != null)
					val = procRecipeRestart(recipeName) ? 1 : 0;
			}

			return 1;
		}

		static int Run_RecipeAbort(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeAbort != null)
					val = procRecipeAbort(recipeName) ? 1 : 0;
			}

			return 1;
		}

		static int Run_RecipeStartUnit(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string recipeName = (string)args[0];
			string unitName = (string)args[1];
			string batchId = args.Length > 2 ? (string)args[2] : "";

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procRecipeStartUnit != null)
				{
					string bid = string.IsNullOrEmpty(batchId) ? null : batchId;
					string unit = string.IsNullOrEmpty(unitName) ? null : unitName;
					var task = procRecipeStartUnit(recipeName, bid, null, unit);
					task.ContinueWith(t =>
					{
						if (t.IsFaulted)
							System.Diagnostics.Debug.WriteLine("RecipeStartUnit error: " + t.Exception?.InnerException?.Message);
					}, TaskContinuationOptions.OnlyOnFaulted);
					val = 1;
				}
			}

			return 1;
		}

		static int Run_RecipeGetState(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = "idle";

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				string recipeName = (string)args[0];
				if (procRecipeGetState != null)
					val = procRecipeGetState(recipeName);
			}

			return 1;
		}

		#endregion

		public static void PrepareMethod(ScriptExternalRun prepare)
		{
			string prename = "Recipe";

			// 기존 함수 (하위호환)
			prepare.AddMethod(prename, "RecipeDownload", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeDownload),
				"in:string:recipe_name", "out:string:error");

			prepare.AddMethod(prename, "RecipeUpload", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeUpload),
				"in:string:recipe_name", "out:string:error");

			prepare.AddMethod(prename, "RecipeGetList", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeGetList));

			prepare.AddMethod(prename, "RecipeGetName", "string",
				new ScriptExternalRun.DeleMethod(Run_RecipeGetName),
				"in:int:index", "out:string:name");

			prepare.AddMethod(prename, "RecipeIsExecuting", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeIsExecuting));

			prepare.AddMethod(prename, "RecipeReLoad", "void",
				new ScriptExternalRun.DeleMethod(Run_RecipeReLoad));

			// 신규 함수 (Unit 대응)
			prepare.AddMethod(prename, "RecipeDownloadUnit", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeDownloadUnit),
				"in:string:recipe_name", "in:string:unit_name", "out:string:error");

			prepare.AddMethod(prename, "RecipeUploadUnit", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeUploadUnit),
				"in:string:recipe_name", "in:string:unit_name", "out:string:error");

			prepare.AddMethod(prename, "RecipeIsExecutingUnit", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeIsExecutingUnit),
				"in:string:recipe_name", "in:string:unit_name");

			// ISA-88 Batch 제어 함수
			prepare.AddMethod(prename, "RecipeStart", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeStart),
				"in:string:recipe_name", "in:string:batch_id");

			prepare.AddMethod(prename, "RecipeStartUnit", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeStartUnit),
				"in:string:recipe_name", "in:string:unit_name", "in:string:batch_id");

			prepare.AddMethod(prename, "RecipeHold", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeHold),
				"in:string:recipe_name");

			prepare.AddMethod(prename, "RecipeRestart", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeRestart),
				"in:string:recipe_name");

			prepare.AddMethod(prename, "RecipeAbort", "int",
				new ScriptExternalRun.DeleMethod(Run_RecipeAbort),
				"in:string:recipe_name");

			prepare.AddMethod(prename, "RecipeGetState", "string",
				new ScriptExternalRun.DeleMethod(Run_RecipeGetState),
				"in:string:recipe_name");
		}
	}
}

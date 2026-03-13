using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoLibLocal;

namespace LocalMain
{
	/// <summary>
	/// 레시피 관리: DB CRUD 래핑 + CSV 내보내기/가져오기
	/// </summary>
	public class RecipeManager
	{
		public static ArrayList blockRecipeList = new ArrayList(); // RecipeInfo 목록 캐시

		public static void Init()
		{
			ReLoad();
		}

		/// <summary>
		/// DB에서 레시피 목록 재로드
		/// </summary>
		public static async void ReLoad()
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return;

				blockRecipeList = await db.GetRecipeListAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RecipeManager.ReLoad 오류: {ex.Message}");
			}
		}

		/// <summary>
		/// 레시피 목록 로드 (async)
		/// </summary>
		public static async Task<ArrayList> LoadRecipeListAsync()
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return new ArrayList();

				blockRecipeList = await db.GetRecipeListAsync();
				return blockRecipeList;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RecipeManager.LoadRecipeListAsync 오류: {ex.Message}");
				return new ArrayList();
			}
		}

		/// <summary>
		/// 레시피 상세 로드
		/// </summary>
		public static async Task<RecipeData> LoadRecipeAsync(int recipeId)
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return null;

				return await db.GetRecipeAsync(recipeId);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RecipeManager.LoadRecipeAsync 오류: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// 레시피 이름으로 로드
		/// </summary>
		public static async Task<RecipeData> LoadRecipeByNameAsync(string recipeName)
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return null;

				return await db.GetRecipeByNameAsync(recipeName);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RecipeManager.LoadRecipeByNameAsync 오류: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// 레시피 저장 (트랜잭션)
		/// </summary>
		public static async Task<(int recipeId, string error)> SaveRecipeAsync(RecipeData recipe)
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return (-1, "DB 연결이 없습니다.");

				var result = await db.SaveRecipeAsync(recipe);

				if (result.error == null)
				{
					ReLoad(); // 목록 캐시 갱신
				}

				return result;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RecipeManager.SaveRecipeAsync 오류: {ex.Message}");
				return (-1, ex.Message);
			}
		}

		/// <summary>
		/// 레시피 삭제 (실행 중 체크는 호출측에서 수행)
		/// </summary>
		public static async Task<string> DeleteRecipeAsync(int recipeId)
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return "DB 연결이 없습니다.";

				string error = await db.DeleteRecipeAsync(recipeId);

				if (error == null)
				{
					ReLoad(); // 목록 캐시 갱신
				}

				return error;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RecipeManager.DeleteRecipeAsync 오류: {ex.Message}");
				return ex.Message;
			}
		}

		#region ISA-88 Phase 2: Batch 관련

		/// <summary>
		/// 배치 생성 + 실행 (Standard 모드): CheckEngineRecipe.BatchStart 래핑
		/// </summary>
		public static async Task<(bool success, string error, string batchId)> CreateBatchAsync(
			string recipeName, string batchId = null, string username = null)
		{
			return await CheckEngineRecipe.BatchStart(recipeName, batchId, username);
		}

		/// <summary>
		/// 최근 배치 실행 목록 조회
		/// </summary>
		public static async Task<System.Collections.Generic.List<BatchExecutionRecord>> GetRecentBatchesAsync(
			int? masterRecipeId = null, int limit = 100)
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return new System.Collections.Generic.List<BatchExecutionRecord>();

				return await db.GetBatchExecutionListAsync(masterRecipeId, limit);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RecipeManager.GetRecentBatchesAsync 오류: {ex.Message}");
				return new System.Collections.Generic.List<BatchExecutionRecord>();
			}
		}

		/// <summary>
		/// 배치 이력 조회 — 날짜 범위 필터
		/// </summary>
		public static async Task<System.Collections.Generic.List<BatchExecutionRecord>> GetRecentBatchesAsync(
			int? masterRecipeId, DateTime dateFrom, DateTime dateTo, int limit = 500)
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return new System.Collections.Generic.List<BatchExecutionRecord>();

				return await db.GetBatchExecutionListAsync(masterRecipeId, dateFrom, dateTo, limit);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"RecipeManager.GetRecentBatchesAsync(날짜) 오류: {ex.Message}");
				return new System.Collections.Generic.List<BatchExecutionRecord>();
			}
		}

		#endregion

		#region CSV Export/Import

		/// <summary>
		/// 레시피를 CSV 파일로 내보내기 - RecipeCsvHelper 위임
		/// </summary>
		public static bool ExportToCsv(RecipeData recipe, string filePath, out string error)
		{
			return RecipeCsvHelper.ExportToCsv(recipe, filePath, out error);
		}

		/// <summary>
		/// CSV 파일에서 레시피 가져오기 - RecipeCsvHelper 위임
		/// </summary>
		public static RecipeData ImportFromCsv(string filePath, out string error)
		{
			return RecipeCsvHelper.ImportFromCsv(filePath, out error);
		}

		#endregion
	}
}

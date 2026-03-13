using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Web.Services;
using AutoLibLocal;
using PortalServerWeb.Library;

namespace PortalServerWeb.AutoWeb.Service
{
	/// <summary>
	/// 레시피 웹 서비스 (ASMX)
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class ServiceRecipe : System.Web.Services.WebService
	{
		/// <summary>
		/// 레시피 목록 조회
		/// </summary>
		[WebMethod]
		public DataSet GetRecipeList(out string error)
		{
			error = null;

			try
			{
				string work_dir = ProjectLib.GetWorkDir(Context.Request);
				TotalConfig.sDirWorkProject = work_dir;

				var db = DataPostgres.Instance;
				if (db == null)
				{
					error = "DB 연결이 없습니다.";
					return null;
				}

				ArrayList list = db.GetRecipeListAsync().GetAwaiter().GetResult();

				DataSet ds = new DataSet();
				DataTable dt = new DataTable("RecipeList");
				dt.Columns.Add("recipe_id", typeof(int));
				dt.Columns.Add("recipe_name", typeof(string));
				dt.Columns.Add("description", typeof(string));
				dt.Columns.Add("created_at", typeof(DateTime));
				dt.Columns.Add("updated_at", typeof(DateTime));

				for (int i = 0; i < list.Count; i++)
				{
					RecipeInfo info = (RecipeInfo)list[i];
					dt.Rows.Add(info.recipe_id, info.recipe_name, info.description, info.created_at, info.updated_at);
				}

				ds.Tables.Add(dt);
				return ds;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				Debug.WriteLine("ServiceRecipe.GetRecipeList 오류: " + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// 레시피 상세 조회 (Steps + Items 포함)
		/// </summary>
		[WebMethod]
		public DataSet GetRecipe(int recipeId, out string error)
		{
			error = null;

			try
			{
				string work_dir = ProjectLib.GetWorkDir(Context.Request);
				TotalConfig.sDirWorkProject = work_dir;

				var db = DataPostgres.Instance;
				if (db == null)
				{
					error = "DB 연결이 없습니다.";
					return null;
				}

				RecipeData recipe = db.GetRecipeAsync(recipeId).GetAwaiter().GetResult();
				if (recipe == null)
				{
					error = "레시피를 찾을 수 없습니다.";
					return null;
				}

				return RecipeToDataSet(recipe);
			}
			catch (Exception ex)
			{
				error = ex.Message;
				Debug.WriteLine("ServiceRecipe.GetRecipe 오류: " + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// 레시피 Download (태그에 값 쓰기)
		/// 주의: SCADA 프로세스의 PlcScan/TagLib 접근이 필요하므로
		/// IPC를 통한 LocalMain 연동이 필요합니다.
		/// </summary>
		[WebMethod]
		public bool RecipeDownload(string recipeName, out string error)
		{
			error = null;

			try
			{
				string work_dir = ProjectLib.GetWorkDir(Context.Request);
				TotalConfig.sDirWorkProject = work_dir;

				// TODO: IPC를 통해 실행 중인 LocalMain SCADA 프로세스의
				// CheckEngineRecipe.RecipeDownload()를 호출해야 합니다.
				// 웹 서비스 프로세스에서는 PlcScan/TagLib에 직접 접근할 수 없습니다.
				// ServiceDataTagStatic.SendAndGetData 패턴을 참고하여 구현하세요.
				error = "Recipe Download는 SCADA 프로세스를 통해 실행되어야 합니다. IPC 연동 필요.";
				return false;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				Debug.WriteLine("ServiceRecipe.RecipeDownload 오류: " + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// 레시피 Upload (현재 태그값 → 새 레시피 저장)
		/// 주의: SCADA 프로세스의 TagLib 접근이 필요합니다.
		/// </summary>
		[WebMethod]
		public bool RecipeUpload(string recipeName, out string error)
		{
			error = null;

			try
			{
				string work_dir = ProjectLib.GetWorkDir(Context.Request);
				TotalConfig.sDirWorkProject = work_dir;

				// TODO: IPC를 통해 실행 중인 LocalMain SCADA 프로세스의
				// CheckEngineRecipe.RecipeUpload()를 호출해야 합니다.
				error = "Recipe Upload는 SCADA 프로세스를 통해 실행되어야 합니다. IPC 연동 필요.";
				return false;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				Debug.WriteLine("ServiceRecipe.RecipeUpload 오류: " + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// 레시피 Download Unit (태그에 값 쓰기 - 특정 Unit만)
		/// </summary>
		[WebMethod]
		public bool RecipeDownloadUnit(string recipeName, string unitName, out string error)
		{
			error = null;

			try
			{
				string work_dir = ProjectLib.GetWorkDir(Context.Request);
				TotalConfig.sDirWorkProject = work_dir;

				// TODO: IPC를 통해 CheckEngineRecipe.RecipeDownload(recipeName, unitName) 호출
				error = "Recipe Download Unit은 SCADA 프로세스를 통해 실행되어야 합니다. IPC 연동 필요.";
				return false;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				Debug.WriteLine("ServiceRecipe.RecipeDownloadUnit 오류: " + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// 레시피 Upload Unit (현재 태그값 → 새 레시피 저장 - 특정 Unit만)
		/// </summary>
		[WebMethod]
		public bool RecipeUploadUnit(string recipeName, string unitName, out string error)
		{
			error = null;

			try
			{
				string work_dir = ProjectLib.GetWorkDir(Context.Request);
				TotalConfig.sDirWorkProject = work_dir;

				// TODO: IPC를 통해 CheckEngineRecipe.RecipeUpload(recipeName, unitName) 호출
				error = "Recipe Upload Unit은 SCADA 프로세스를 통해 실행되어야 합니다. IPC 연동 필요.";
				return false;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				Debug.WriteLine("ServiceRecipe.RecipeUploadUnit 오류: " + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// RecipeData → DataSet 변환 (직렬화, Unit 지원)
		/// </summary>
		static DataSet RecipeToDataSet(RecipeData recipe)
		{
			DataSet ds = new DataSet("RecipeDataSet");

			// Recipe 테이블
			DataTable dtRecipe = new DataTable("Recipe");
			dtRecipe.Columns.Add("recipe_id", typeof(int));
			dtRecipe.Columns.Add("recipe_name", typeof(string));
			dtRecipe.Columns.Add("description", typeof(string));
			dtRecipe.Columns.Add("created_at", typeof(DateTime));
			dtRecipe.Columns.Add("updated_at", typeof(DateTime));
			dtRecipe.Rows.Add(recipe.recipe_id, recipe.recipe_name, recipe.description, recipe.created_at, recipe.updated_at);
			ds.Tables.Add(dtRecipe);

			// Unit 테이블 (ISA-88 Lite)
			DataTable dtUnit = new DataTable("Unit");
			dtUnit.Columns.Add("unit_id", typeof(int));
			dtUnit.Columns.Add("unit_name", typeof(string));
			dtUnit.Columns.Add("unit_order", typeof(int));
			dtUnit.Columns.Add("description", typeof(string));

			for (int u = 0; u < recipe.units.Count; u++)
			{
				RecipeUnitData unit = (RecipeUnitData)recipe.units[u];
				dtUnit.Rows.Add(unit.unit_id, unit.unit_name, unit.unit_order, unit.description);
			}
			ds.Tables.Add(dtUnit);

			// Step 테이블 (unit_id 컬럼 추가)
			DataTable dtStep = new DataTable("Step");
			dtStep.Columns.Add("step_id", typeof(int));
			dtStep.Columns.Add("unit_id", typeof(int));
			dtStep.Columns.Add("step_order", typeof(int));
			dtStep.Columns.Add("step_name", typeof(string));
			dtStep.Columns.Add("wait_time_ms", typeof(int));
			dtStep.Columns.Add("timeout_ms", typeof(int));
			dtStep.Columns.Add("condition_tag", typeof(string));
			dtStep.Columns.Add("condition_value", typeof(string));
			dtStep.Columns.Add("condition_type", typeof(string));

			// Item 테이블
			DataTable dtItem = new DataTable("Item");
			dtItem.Columns.Add("item_id", typeof(int));
			dtItem.Columns.Add("step_id", typeof(int));
			dtItem.Columns.Add("tag_name", typeof(string));
			dtItem.Columns.Add("set_value", typeof(string));
			dtItem.Columns.Add("value_type", typeof(string));
			dtItem.Columns.Add("item_order", typeof(int));

			// Recipe 직속 step (unit_id = 0)
			for (int s = 0; s < recipe.steps.Count; s++)
			{
				RecipeStepData step = (RecipeStepData)recipe.steps[s];
				dtStep.Rows.Add(step.step_id, 0, step.step_order, step.step_name,
					step.wait_time_ms, step.timeout_ms,
					step.condition_tag, step.condition_value, step.condition_type);

				for (int i = 0; i < step.items.Count; i++)
				{
					RecipeItemData item = (RecipeItemData)step.items[i];
					dtItem.Rows.Add(item.item_id, step.step_id, item.tag_name,
						item.set_value, item.value_type, item.item_order);
				}
			}

			// Unit 소속 step
			for (int u = 0; u < recipe.units.Count; u++)
			{
				RecipeUnitData unit = (RecipeUnitData)recipe.units[u];
				for (int s = 0; s < unit.steps.Count; s++)
				{
					RecipeStepData step = (RecipeStepData)unit.steps[s];
					dtStep.Rows.Add(step.step_id, unit.unit_id, step.step_order, step.step_name,
						step.wait_time_ms, step.timeout_ms,
						step.condition_tag, step.condition_value, step.condition_type);

					for (int i = 0; i < step.items.Count; i++)
					{
						RecipeItemData item = (RecipeItemData)step.items[i];
						dtItem.Rows.Add(item.item_id, step.step_id, item.tag_name,
							item.set_value, item.value_type, item.item_order);
					}
				}
			}

			ds.Tables.Add(dtStep);
			ds.Tables.Add(dtItem);

			return ds;
		}
	}
}
